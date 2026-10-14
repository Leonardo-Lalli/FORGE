#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE
#
# FORGE — Self-hosted Workout Tracker for Proxmox VE
# Creates a dedicated LXC container and installs PocketBase natively.
# Runs on the Proxmox hypervisor (not inside the container).

set -euo pipefail

FORGE_VERSION="beta — repository checkout"
CT_ID=""
CT_HOSTNAME="${FORGE_CT_HOSTNAME:-forge}"
STORAGE="${FORGE_CT_STORAGE:-local-lvm}"
BRIDGE="${FORGE_CT_BRIDGE:-vmbr0}"
PB_EMAIL="${PB_ADMIN_EMAIL:?Set PB_ADMIN_EMAIL before running the installer}"
PB_PASS="${PB_ADMIN_PASSWORD:?Set a unique PB_ADMIN_PASSWORD before running the installer}"
PB_PORT="8090"
: "${PB_VERSION:?Set a reviewed PocketBase version before installation}"
[ "$PB_PASS" != 'forgeadmin123' ] || { echo 'Refusing published default password.' >&2; exit 1; }
FORGE_INIT_SCRIPT="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)/init-pocketbase.sh"
[ -f "$FORGE_INIT_SCRIPT" ] || { echo 'Run this script from a reviewed local repository checkout.' >&2; exit 1; }

# ── Colours ────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'
msg_ok()    { echo -e "${GREEN}[ OK ]${NC}  $*"; }
msg_info()  { echo -e "${CYAN}[INFO]${NC}  $*"; }
msg_warn()  { echo -e "${YELLOW}[WARN]${NC}  $*"; }
msg_error() { echo -e "${RED}[ERR ]${NC}  $*"; exit 1; }

# ── Header ─────────────────────────────────────
if [[ -t 1 ]]; then clear; fi
echo ""
echo -e "${CYAN}   ███████╗ ██████╗ ██████╗  ██████╗ ███████╗${NC}"
echo -e "${CYAN}   ██╔════╝██╔═══██╗██╔══██╗██╔════╝ ██╔════╝${NC}"
echo -e "${CYAN}   █████╗  ██║   ██║██████╔╝██║  ███╗█████╗  ${NC}"
echo -e "${CYAN}   ██╔══╝  ██║   ██║██╔══██╗██║   ██║██╔══╝  ${NC}"
echo -e "${CYAN}   ██║     ╚██████╔╝██║  ██║╚██████╔╝███████╗${NC}"
echo -e "${CYAN}   ╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚════════╝${NC}"
echo ""
echo -e "           ${BOLD}The Social Workout Tracker${NC}"
echo -e "           ${FORGE_VERSION} — Proxmox VE Installer"
echo ""
echo -e "  PocketBase nativo (senza Docker); risorse dipendenti da utenti e foto"
echo -e "  Community sul server | Catalogo condiviso | Achievement"
echo ""

# ── OS Selection Menu ──────────────────────────
echo -e "${BOLD}Choose the base OS for your FORGE container:${NC}"
echo ""
echo -e "  1) Debian 12 (default)"
echo -e "  2) Ubuntu 24.04"
CHOICE=${FORGE_CT_OS:-}
if [[ -z "$CHOICE" ]]; then
  [[ -r /dev/tty ]] || msg_error 'Set FORGE_CT_OS=1 (Debian) or 2 (Ubuntu) for unattended installation.'
  read -r -p "Choose [1-2] (default: 1): " CHOICE </dev/tty
fi
case "${CHOICE:-1}" in
  1) OS_LABEL="Debian 12"; TEMPLATE_PATTERN='^debian-12-standard_.*amd64.tar.zst$';;
  2) OS_LABEL="Ubuntu 24.04"; TEMPLATE_PATTERN='^ubuntu-24.04-standard_.*amd64.tar.zst$';;
  *) msg_error "Choose Debian or Ubuntu.";;
esac
OS_SETUP="apt-get update -qq && apt-get install -y -qq curl unzip wget jq ca-certificates"
pveam update
TEMPLATE=$(pveam available --section system | awk -v pattern="$TEMPLATE_PATTERN" '$2 ~ pattern { print $2 }' | sort -V | tail -n1)
[ -n "$TEMPLATE" ] || msg_error "No matching template is available; check pveam available."

echo ""
msg_info "Selected: ${OS_LABEL}"

# Select a cluster-wide free ID, including IDs used by QEMU VMs.
CT_ID=$(pvesh get /cluster/nextid)
[[ "$CT_ID" =~ ^[0-9]+$ ]] || msg_error "Proxmox did not return a valid free container ID."
msg_info "Container ID: ${CT_ID}"

# ── Download template if missing ───────────────
if [ ! -f "/var/lib/vz/template/cache/${TEMPLATE}" ]; then
  msg_info "Downloading ${OS_LABEL} template..."
  pveam update &>/dev/null
  pveam download local "${TEMPLATE}" || msg_error "Failed to download template."
fi
msg_ok "Template ready: ${TEMPLATE}"

# ── Create LXC Container ───────────────────────
msg_info "Creating container (ID: ${CT_ID}, RAM: 512MB, Disk: 4GB, CPU: 2)..."

pct create "${CT_ID}" \
  "local:vztmpl/${TEMPLATE}" \
  --hostname "${CT_HOSTNAME}" \
  --storage "${STORAGE}" \
  --rootfs "${STORAGE}:4" \
  --memory 512 \
  --swap 512 \
  --cores 2 \
  --net0 "name=eth0,bridge=${BRIDGE},ip=dhcp" \
  --unprivileged 1 \
  --features nesting=1 \
  --onboot 1 \
  --start 0 \
  &>/dev/null || msg_error "Failed to create container. Check storage '${STORAGE}' exists."

msg_ok "Container ${CT_ID} (${CT_HOSTNAME}) created"

# ── Start container ────────────────────────────
msg_info "Starting container..."
pct start "${CT_ID}" &>/dev/null
sleep 5

# Wait for boot
for i in $(seq 1 20); do
  if pct exec "${CT_ID}" -- echo "ready" &>/dev/null 2>&1; then ready=1; break; fi
  sleep 2
done
[[ ${ready:-0} == 1 ]] || msg_error 'Container boot timeout. The created CT was retained for diagnosis.'
msg_ok "Container is running"

# ── Install dependencies ───────────────────────
msg_info "Installing base packages..."
pct exec "${CT_ID}" -- bash -c "${OS_SETUP}" &>/dev/null
msg_ok "Packages installed"

# ── Install PocketBase ─────────────────────────
PB_VERSION="${PB_VERSION:?Set a reviewed PocketBase version before installation}"
PB_ARCH="amd64"
PB_URL="https://github.com/pocketbase/pocketbase/releases/download/v${PB_VERSION}/pocketbase_${PB_VERSION}_linux_${PB_ARCH}.zip"

msg_info "Downloading PocketBase v${PB_VERSION}..."
pct exec "${CT_ID}" -- bash -c "
  set -euo pipefail
  mkdir -p /opt/forge-pocketbase/{pb_data,pb_public,pb_migrations,pb_hooks}
  cd /opt/forge-pocketbase
  wget -q '${PB_URL}' -O pocketbase.zip
  unzip -qo pocketbase.zip
  rm pocketbase.zip
  chmod +x pocketbase
  useradd -r -s /bin/false -d /opt/forge-pocketbase pocketbase 2>/dev/null || true
  chown -R pocketbase:pocketbase /opt/forge-pocketbase
" 2>/dev/null
msg_ok "PocketBase installed"

# ── Create systemd service ─────────────────────
msg_info "Creating systemd service..."
pct exec "${CT_ID}" -- bash -c "
set -euo pipefail
cat <<'UNIT' > /etc/systemd/system/forge-pocketbase.service
[Unit]
Description = FORGE PocketBase Backend
After        = network.target

[Service]
Type           = simple
User           = pocketbase
Group          = pocketbase
LimitNOFILE    = 4096
Restart        = always
RestartSec     = 5s
WorkingDirectory = /opt/forge-pocketbase
ExecStart      = /opt/forge-pocketbase/pocketbase serve --http=0.0.0.0:${PB_PORT}

[Install]
WantedBy = multi-user.target
UNIT

systemctl daemon-reload
systemctl enable -q --now forge-pocketbase
" 2>/dev/null
msg_ok "Service started on port ${PB_PORT}"

# ── Wait for PocketBase ────────────────────────
msg_info "Waiting for PocketBase..."
healthy=0
for i in $(seq 1 30); do
  if pct exec "${CT_ID}" -- curl -sf "http://localhost:${PB_PORT}/api/health" &>/dev/null; then
    healthy=1
    break
  fi
  sleep 2
done
[ "$healthy" -eq 1 ] || msg_error "PocketBase did not become healthy within 60 seconds."
msg_ok "PocketBase online"

# ── Create superuser ───────────────────────────
msg_info "Creating superuser..."
pct exec "${CT_ID}" -- /opt/forge-pocketbase/pocketbase superuser create "${PB_EMAIL}" "${PB_PASS}" --dir=/opt/forge-pocketbase/pb_data >/dev/null 2>&1 && \
  msg_ok "Superuser created" || msg_warn "Superuser creation failed; initialization will verify existing credentials."

# ── Create collections via init script ─────────
msg_info "Configuring collections..."

pct push "${CT_ID}" "${FORGE_INIT_SCRIPT}" /tmp/init-forge.sh
pct exec "${CT_ID}" -- env PB_URL="http://localhost:${PB_PORT}" \
  PB_ADMIN_EMAIL="${PB_EMAIL}" PB_ADMIN_PASSWORD="${PB_PASS}" sh /tmp/init-forge.sh
msg_ok "Missing collections initialized; existing live rules were not changed."
# Deploy reviewed hooks/migrations into the new CT and use the same backup procedure as existing LXC installs.
forge_code_stage=$(mktemp -d /tmp/forge-ct-code.XXXXXX)
cp -a "$(dirname "$FORGE_INIT_SCRIPT")/pocketbase" "$forge_code_stage/pocketbase"
cp "$(dirname "$FORGE_INIT_SCRIPT")/apply-profile-migration.sh" "$forge_code_stage/apply-profile-migration.sh"
mkdir -p "$forge_code_stage/tools"
mv "$forge_code_stage/pocketbase" "$forge_code_stage/apply-profile-migration.sh" "$forge_code_stage/tools/"
tar -C "$forge_code_stage" -czf "$forge_code_stage/code.tgz" tools
pct push "${CT_ID}" "$forge_code_stage/code.tgz" /tmp/forge-code.tgz
pct exec "${CT_ID}" -- bash -c 'mkdir -p /tmp/forge-code; tar -xzf /tmp/forge-code.tgz -C /tmp/forge-code; bash /tmp/forge-code/tools/apply-profile-migration.sh /opt/forge-pocketbase forge-pocketbase'


# ── Summary ────────────────────────────────────
CT_IP=$(pct exec "${CT_ID}" -- hostname -I | awk '{print $1}')
if [[ -z "$CT_IP" ]]; then
  CT_IP=YOUR-CONTAINER-IP
  msg_warn 'No container address detected; inspect its network settings before connecting the app.'
fi
echo ""
echo -e "${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${GREEN}FORGE is online!${NC}                                        ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Container: ${CYAN}${CT_ID} (${CT_HOSTNAME})${NC}                              ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     OS:        ${OS_LABEL}                                  ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     RAM:       512 MB | Disk: 4 GB | CPU: 2             ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     App URL:   ${CYAN}http://${CT_IP}:${PB_PORT}${NC}                              ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Admin:     ${CYAN}http://${CT_IP}:${PB_PORT}/_/${NC}                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Login:     ${PB_EMAIL} (password not displayed)        ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${YELLOW}KEEP ADMIN ACCESS PRIVATE AND BACK UP YOUR DATA${NC}               ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "  ${CYAN}pct enter ${CT_ID}${NC}        — enter container shell"
echo -e "  ${CYAN}pct stop ${CT_ID}${NC}         — stop container"
echo -e "  ${CYAN}pct start ${CT_ID}${NC}        — start container"
echo ""
msg_ok "Installation complete!"
