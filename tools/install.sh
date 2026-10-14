#!/usr/bin/env bash
# FORGE guided bootstrap for a NEW Linux installation.
set -euo pipefail
umask 077
fail() { printf 'FORGE: %s\n' "$*" >&2; exit 1; }
if [[ ${1:-} == --help || ${1:-} == -h ]]; then
  printf '%s\n' 'Usage: bash install.sh docker|lxc|proxmox' 'docker: Linux/WSL with Docker Compose already installed' 'lxc: inside a new Debian/Ubuntu systemd container, as root' 'proxmox: on the Proxmox VE node, as root; creates a new CT' 'Prompts for PB_VERSION, PB_ADMIN_EMAIL, PB_ADMIN_PASSWORD when unset.'
  exit 0
fi
mode=${1:-docker}
case "$mode" in docker|lxc|proxmox) ;; *) fail 'Choose docker, lxc or proxmox.';; esac
[[ $(uname -s) == Linux ]] || fail 'Use a Linux shell or WSL; this bootstrap is not a native Windows installer.'
case "$mode" in
  docker) command -v docker >/dev/null || fail 'Install Docker Engine and Compose first.'; docker compose version >/dev/null || fail 'Docker Compose is required.'; [[ $(id -u) == 0 ]] || command -v sudo >/dev/null || fail 'Root or sudo access is required for migration backups.';;
  lxc) [[ $(id -u) == 0 ]] || fail 'Run as root inside the container.'; ! command -v pct >/dev/null || fail 'This is a Proxmox node. Use proxmox mode here, or enter the CT for lxc mode.'; command -v systemctl >/dev/null || fail 'A Debian/Ubuntu systemd container is required.'; [[ ! -e /opt/forge-pocketbase/pb_data/data.db && ! -e /opt/pocketbase/pb_data/data.db ]] || fail 'An existing backend was found. Use the update guide, not a fresh installer.';;
  proxmox) [[ $(id -u) == 0 ]] || fail 'Run as root on the Proxmox node.'; command -v pct >/dev/null && command -v pveam >/dev/null || fail 'Proxmox VE is required.';;
esac
if ! command -v git >/dev/null; then
  [[ $(id -u) == 0 ]] && command -v apt-get >/dev/null || fail 'Install git, then rerun.'
  apt-get update
  apt-get install -y git ca-certificates
fi
ask() {
  [[ -r /dev/tty ]] || fail "Set $1 before running non-interactively."
  read -r -p "$2" "$1" </dev/tty
}
if [[ -z ${PB_VERSION:-} ]]; then ask PB_VERSION 'PocketBase version (numeric, >= 0.23.0): '; fi
[[ $PB_VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]] || fail 'Use a numeric PocketBase version, without v or latest.'
[[ $(printf '%s\n' 0.23.0 "$PB_VERSION" | sort -V | head -n1) == 0.23.0 ]] || fail 'PocketBase 0.23.0 or newer is required.'
if [[ -z ${PB_ADMIN_EMAIL:-} ]]; then ask PB_ADMIN_EMAIL 'Administrator email: '; fi
[[ $PB_ADMIN_EMAIL =~ ^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$ ]] || fail 'Enter an administrator email address.'
if [[ -z ${PB_ADMIN_PASSWORD:-} ]]; then
  [[ -r /dev/tty ]] || fail 'Set PB_ADMIN_PASSWORD before running non-interactively.'
  read -r -s -p 'Administrator password (Enter generates a private random password): ' PB_ADMIN_PASSWORD </dev/tty
  printf '\n'
  if [[ -z $PB_ADMIN_PASSWORD ]]; then PB_ADMIN_PASSWORD=$(od -An -N24 -tx1 /dev/urandom | tr -d ' \n'); fi
fi
[[ ${#PB_ADMIN_PASSWORD} -ge 16 && $PB_ADMIN_PASSWORD != forgeadmin123 ]] || fail 'Use a unique password of at least 16 characters.'
# Keep a single-line Compose .env and avoid ambiguous quote/backslash parsing.
[[ $PB_ADMIN_PASSWORD != *"'"* && $PB_ADMIN_PASSWORD != *'\'* && $PB_ADMIN_PASSWORD != *$'\n'* && $PB_ADMIN_PASSWORD != *$'\r'* ]] || fail 'The setup password cannot contain apostrophes, backslashes or newlines. Leave it empty to generate one.'
export PB_VERSION PB_ADMIN_EMAIL PB_ADMIN_PASSWORD
forge_dir=${FORGE_INSTALL_DIR:-$HOME/forge-server}
repo_url=https://github.com/Leonardo-Lalli/FORGE.git
[[ ! -e "$forge_dir" ]] || fail "Destination already exists: $forge_dir. Use the reviewed checkout/update guide instead."
git clone --depth 1 --branch main "$repo_url" "$forge_dir"
cd "$forge_dir"
export FORGE_INSTALL_DIR="$forge_dir"
# No secrets are printed. This private file also retains generated passwords.
{
  printf 'PB_VERSION=%s\n' "$PB_VERSION"
  printf "PB_ADMIN_EMAIL='%s'\n" "$PB_ADMIN_EMAIL"
  printf "PB_ADMIN_PASSWORD='%s'\n" "$PB_ADMIN_PASSWORD"
  printf 'PB_BIND_ADDRESS=127.0.0.1\nFORGE_PUSH_WORKER_TOKEN=\n'
} > .env
chmod 600 .env
printf 'Configuration saved privately to %s/.env. Keep a protected backup.\n' "$forge_dir"
case "$mode" in
  docker) bash tools/community-install.sh;;
  lxc) bash tools/proxmox-install.sh;;
  proxmox) bash tools/proxmox-create.sh;;
esac
