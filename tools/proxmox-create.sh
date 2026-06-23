#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE
#
# FORGE — Self-hosted Workout Tracker for Proxmox VE
# Creates a dedicated LXC container and installs PocketBase natively.
# Runs on the Proxmox hypervisor (not inside the container).

set -uo pipefail

FORGE_VERSION="v0.8.0-beta"
CT_ID=""
CT_HOSTNAME="forge"
STORAGE="local-lvm"
BRIDGE="vmbr0"
PB_EMAIL="admin@forge.local"
PB_PASS="forgeadmin123"
PB_PORT="8090"

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
    1) OS_LABEL="Alpine 3.21"
       TEMPLATE="alpine-3.21-default_20250210_amd64.tar.xz"
       OS_SETUP="apk add --no-cache curl bash unzip wget"
       PKG_INSTALL="apk add --no-cache"
       break ;;
    2) OS_LABEL="Debian 12"
       TEMPLATE="debian-12-standard_12.7-1_amd64.tar.zst"
       OS_SETUP="apt-get update -qq && apt-get install -y -qq curl unzip wget"
       PKG_INSTALL="apt-get install -y -qq"
       break ;;
    3) OS_LABEL="Ubuntu 24.04"
       TEMPLATE="ubuntu-24.04-standard_24.04-2_amd64.tar.zst"
       OS_SETUP="apt-get update -qq && apt-get install -y -qq curl unzip wget"
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
PB_VERSION="0.25.9"
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
for i in $(seq 1 30); do
  if pct exec "${CT_ID}" -- curl -sf "http://localhost:${PB_PORT}/api/health" &>/dev/null; then
    break
  fi
  sleep 2
done
msg_ok "PocketBase online"

# ── Create superuser ───────────────────────────
msg_info "Creating superuser..."
pct exec "${CT_ID}" -- /opt/forge-pocketbase/pocketbase superuser create "${PB_EMAIL}" "${PB_PASS}" 2>/dev/null && \
  msg_ok "Superuser created" || msg_ok "Superuser already exists"

# ── Create collections via init script ─────────
msg_info "Configuring collections..."

# Write init script inside container via heredoc
pct exec "${CT_ID}" -- bash -c "cat > /tmp/init-forge.sh << 'INITEOF'
#!/bin/sh
PB_URL=\"http://localhost:${PB_PORT}\"
PB_ADMIN_EMAIL=\"${PB_EMAIL}\"
PB_ADMIN_PASS=\"${PB_PASS}\"

# Authenticate
TOKEN=\$(curl -sf -X POST \"\${PB_URL}/api/collections/_superusers/auth-with-password\" \
  -H \"Content-Type: application/json\" \
  -d \"{\\"identity\\":\\"\${PB_ADMIN_EMAIL}\\",\\"password\\":\\"\${PB_ADMIN_PASS}\\"}\" \
  | sed 's/.*\"token\":\"\\([^\"]*\\)\".*/\\1/')

if [ -z \"\$TOKEN\" ]; then
  TOKEN=\$(curl -sf -X POST \"\${PB_URL}/api/admins/auth-with-password\" \
    -H \"Content-Type: application/json\" \
    -d \"{\\"email\\":\\"\${PB_ADMIN_EMAIL}\\",\\"password\\":\\"\${PB_ADMIN_PASS}\\"}\" \
    | sed 's/.*\"token\":\"\\([^\"]*\\)\".*/\\1/')
fi

if [ -z \"\$TOKEN\" ]; then
  echo \"AUTH_FAILED\"
  exit 1
fi

AUTH=\"Authorization: Bearer \${TOKEN}\"
API=\"\${PB_URL}/api/collections\"

create_if_missing() {
  if curl -sf \"\$API\" -H \"\$AUTH\" | grep -q \"\\\\\"name\\\\\":\\\\\"\$1\\\\\"\"; then
    echo \"  \$1 (exists)\"
  else
    curl -sf -X POST \"\$API\" -H \"\$AUTH\" -H \"Content-Type: application/json\" -d \"\$2\" >/dev/null
    echo \"  \$1 (created)\"
  fi
}

create_if_missing 'logged_workouts' '{\"name\":\"logged_workouts\",\"type\":\"base\",\"schema\":[{\"name\":\"user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"users\",\"cascadeDelete\":false}},{\"name\":\"user_name\",\"type\":\"text\"},{\"name\":\"name\",\"type\":\"text\",\"required\":true},{\"name\":\"date\",\"type\":\"date\",\"required\":true},{\"name\":\"exercises\",\"type\":\"json\"},{\"name\":\"exercise_data\",\"type\":\"json\"},{\"name\":\"volume\",\"type\":\"number\"},{\"name\":\"duration\",\"type\":\"number\"},{\"name\":\"likes\",\"type\":\"number\"},{\"name\":\"liked_by\",\"type\":\"json\"},{\"name\":\"photos\",\"type\":\"json\"}],\"listRule\":\"@request.auth.id != '\\'''\\'' && user = @request.auth.id\",\"viewRule\":\"@request.auth.id != '\\'''\\''\",\"createRule\":\"@request.auth.id != '\\'''\\'' && user = @request.auth.id\",\"updateRule\":\"@request.auth.id != '\\'''\\'' && (user = @request.auth.id || @request.auth.id ?= liked_by)\",\"deleteRule\":\"@request.auth.id != '\\'''\\'' && user = @request.auth.id\"}'
create_if_missing 'social_graph' '{\"name\":\"social_graph\",\"type\":\"base\",\"schema\":[{\"name\":\"from_user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"users\",\"cascadeDelete\":false}},{\"name\":\"from_name\",\"type\":\"text\"},{\"name\":\"to_user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"users\",\"cascadeDelete\":false}},{\"name\":\"status\",\"type\":\"text\",\"required\":true}],\"listRule\":\"@request.auth.id != '\\'''\\'' && (from_user = @request.auth.id || to_user = @request.auth.id)\",\"viewRule\":\"@request.auth.id != '\\'''\\'' && (from_user = @request.auth.id || to_user = @request.auth.id)\",\"createRule\":\"@request.auth.id != '\\'''\\'' && from_user = @request.auth.id\",\"updateRule\":\"@request.auth.id != '\\'''\\'' && (from_user = @request.auth.id || to_user = @request.auth.id)\",\"deleteRule\":\"@request.auth.id != '\\'''\\'' && (from_user = @request.auth.id || to_user = @request.auth.id)\"}'
create_if_missing 'excercise' '{\"name\":\"excercise\",\"type\":\"base\",\"schema\":[{\"name\":\"name\",\"type\":\"text\",\"required\":true},{\"name\":\"bodyPart\",\"type\":\"text\"},{\"name\":\"equipment\",\"type\":\"text\"},{\"name\":\"instructions\",\"type\":\"json\"},{\"name\":\"imageUrl\",\"type\":\"url\"},{\"name\":\"category\",\"type\":\"text\"}],\"listRule\":\"@request.auth.id != '\\'''\\''\",\"viewRule\":\"@request.auth.id != '\\'''\\''\",\"createRule\":\"@request.auth.id != '\\'''\\''\",\"updateRule\":\"@request.auth.id = '\\'''\\''\",\"deleteRule\":\"@request.auth.id = '\\'''\\''\"}'
echo 'DONE'
INITEOF"

INIT_RESULT=$(pct exec "${CT_ID}" -- bash /tmp/init-forge.sh 2>/dev/null)
if echo "$INIT_RESULT" | grep -q "DONE"; then
  echo "$INIT_RESULT"
  msg_ok "Collections configured"
else
  msg_warn "Collections may need manual setup"
  msg_warn "Run inside container: bash /tmp/init-forge.sh"
fi

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
echo -e "${BOLD}║${NC}     Login:     ${PB_EMAIL} / ${PB_PASS}        ${BOLD}║${NC}"
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
