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

FORGE_VERSION="v0.8.0-beta"
CT_ID=""
CT_HOSTNAME="forge"
STORAGE="local-lvm"
BRIDGE="vmbr0"
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
clear
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
echo -e "  PocketBase ~25 MB RAM | ~30 MB disk | Native (no Docker)"
echo -e "  1,500+ exercises | Social feed | Achievements | Offline"
echo ""

# ── OS Selection Menu ──────────────────────────
echo -e "${BOLD}Choose the base OS for your FORGE container:${NC}"
echo ""
echo -e "  ${CYAN}1)${NC} Alpine Linux 3.21"
echo -e "     ${YELLOW}OS: ~50 MB RAM | ~200 MB disk | 1 CPU${NC}"
echo -e "     Lightest. Fastest boot. Minimal attack surface."
echo ""
echo -e "  ${CYAN}2)${NC} Debian 12 (Bookworm) ${GREEN}[recommended]${NC}"
echo -e "     ${YELLOW}OS: ~100 MB RAM | ~500 MB disk | 1 CPU${NC}"
echo -e "     Best compatibility. LTS until 2028. Large package ecosystem."
echo ""
echo -e "  ${CYAN}3)${NC} Ubuntu 24.04 (Noble)"
echo -e "     ${YELLOW}OS: ~150 MB RAM | ~800 MB disk | 1 CPU${NC}"
echo -e "     Latest packages. 10 years of security updates."
echo ""
echo -e "  ${YELLOW}+ PocketBase adds ~25 MB RAM and ~30 MB disk to any choice${NC}"
echo ""

while true; do
  read -r -p "$(echo -e "${BOLD}Enter choice [1-3] (default: 2): ${NC}")" CHOICE
  CHOICE=${CHOICE:-2}
  case "$CHOICE" in
    1) msg_error "Alpine is not supported by this systemd installer. Choose Debian or Ubuntu."; OS_LABEL="Alpine 3.21"
       TEMPLATE="alpine-3.21-default_20250210_amd64.tar.xz"
       OS_SETUP="apk add --no-cache curl bash unzip wget"
       PKG_INSTALL="apk add --no-cache"
       break ;;
    2) OS_LABEL="Debian 12"
       TEMPLATE="debian-12-standard_12.7-1_amd64.tar.zst"
       OS_SETUP="apt-get update -qq && apt-get install -y -qq curl unzip wget jq"
       PKG_INSTALL="apt-get install -y -qq"
       break ;;
    3) OS_LABEL="Ubuntu 24.04"
       TEMPLATE="ubuntu-24.04-standard_24.04-2_amd64.tar.zst"
       OS_SETUP="apt-get update -qq && apt-get install -y -qq curl unzip wget jq"
       PKG_INSTALL="apt-get install -y -qq"
       break ;;
    *) echo -e "${RED}Invalid choice. Pick 1, 2, or 3.${NC}" ;;
  esac
done

echo ""
msg_info "Selected: ${OS_LABEL}"

# ── Find next available CT ID ──────────────────
for i in $(seq 100 999); do
  if ! pct status "$i" &>/dev/null; then
    CT_ID=$i
    break
  fi
done
if [ -z "$CT_ID" ]; then
  msg_error "No free container ID found (100-999)."
fi
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
  if pct exec "${CT_ID}" -- echo "ready" &>/dev/null 2>&1; then break; fi
  sleep 2
done
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
pct exec "${CT_ID}" -- /opt/forge-pocketbase/pocketbase superuser create "${PB_EMAIL}" "${PB_PASS}" --dir=/opt/forge-pocketbase/pb_data 2>/dev/null && \
  msg_ok "Superuser created" || msg_ok "Superuser already exists"

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
echo -e "${BOLD}║${NC}     ${YELLOW}CHANGE THE ADMIN PASSWORD IMMEDIATELY!${NC}               ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "  ${CYAN}pct enter ${CT_ID}${NC}        — enter container shell"
echo -e "  ${CYAN}pct stop ${CT_ID}${NC}         — stop container"
echo -e "  ${CYAN}pct start ${CT_ID}${NC}        — start container"
echo ""
msg_ok "Installation complete!"
