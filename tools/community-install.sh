#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE

set -uo pipefail

FORGE_VERSION="v0.8.0-beta"
FORGE_DIR="$HOME/forge-server"
REPO_URL="https://github.com/Leonardo-Lalli/FORGE.git"
PB_EMAIL="admin@forge.local"
PB_PASS="forgeadmin123"

# ── Colours ────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'
msg_ok()    { echo -e "${GREEN}[ OK ]${NC}  $*"; }
msg_info()  { echo -e "${CYAN}[INFO]${NC}  $*"; }
msg_warn()  { echo -e "${YELLOW}[WARN]${NC}  $*"; }
msg_error() { echo -e "${RED}[ERR ]${NC}  $*"; }
section()   { echo -e "\n${BOLD}── $* ──${NC}"; }

# ── Header ─────────────────────────────────────
echo ""
echo -e "${CYAN}   ███████╗ ██████╗ ██████╗  ██████╗ ███████╗${NC}"
echo -e "${CYAN}   ██╔════╝██╔═══██╗██╔══██╗██╔════╝ ██╔════╝${NC}"
echo -e "${CYAN}   █████╗  ██║   ██║██████╔╝██║  ███╗█████╗  ${NC}"
echo -e "${CYAN}   ██╔══╝  ██║   ██║██╔══██╗██║   ██║██╔══╝  ${NC}"
echo -e "${CYAN}   ██║     ╚██████╔╝██║  ██║╚██████╔╝███████╗${NC}"
echo -e "${CYAN}   ╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚════════╝${NC}"
echo ""
echo -e "           ${BOLD}The Social Workout Tracker${NC}"
echo -e "           ${FORGE_VERSION} — Self-Hosted Setup"
echo ""
echo -e "  PocketBase ~25 MB RAM | ~30 MB disk | Docker"
echo -e "  1,500+ exercises | Social feed | Achievements | Offline"
echo ""

# ── Pre-checks ─────────────────────────────────
section "Pre-checks"

if ! command -v docker &>/dev/null; then
  msg_error "Docker is not installed. Install it: https://docs.docker.com/engine/install/"
fi
msg_ok "Docker $(docker --version | awk '{print $3}' | tr -d ',')"

if ! docker compose version &>/dev/null 2>&1 && ! docker-compose --version &>/dev/null 2>&1; then
  msg_error "Docker Compose not found."
fi
msg_ok "Docker Compose available"

# ── Setup FORGE ────────────────────────────────
section "Setup FORGE"

if [ -d "$FORGE_DIR" ]; then
  msg_info "Directory $FORGE_DIR exists, updating..."
  cd "$FORGE_DIR"
  git pull --ff-only --quiet
else
  msg_info "Cloning $REPO_URL..."
  git clone --quiet "$REPO_URL" "$FORGE_DIR"
  cd "$FORGE_DIR"
fi
msg_ok "Repository ready at $FORGE_DIR"

if [ ! -f "$FORGE_DIR/.env" ]; then
  msg_info "Creating .env..."
  cp "$FORGE_DIR/.env.example" "$FORGE_DIR/.env"
  msg_ok ".env created (using defaults)"
else
  msg_ok ".env already present"
fi

# ── Start PocketBase ───────────────────────────
section "Start PocketBase"

msg_info "Starting Docker containers..."
cd "$FORGE_DIR"
docker compose down --remove-orphans 2>&1 | tail -1
docker compose up -d pocketbase
msg_ok "PocketBase started"

msg_info "Waiting for PocketBase (max 60s)..."
for i in $(seq 1 30); do
  if curl -sf http://localhost:8090/api/health &>/dev/null 2>&1; then
    msg_ok "PocketBase is online"
    break
  fi
  sleep 2
done

# ── Create superuser ───────────────────────────
msg_info "Creating superuser..."
docker compose exec -T pocketbase pocketbase superuser create "$PB_EMAIL" "$PB_PASS" 2>&1 | tail -1
if [ ${PIPESTATUS[0]} -eq 0 ]; then
  msg_ok "Superuser created ($PB_EMAIL)"
else
  msg_ok "Superuser already exists"
fi

# ── Init collections ───────────────────────────
msg_info "Initializing collections..."
docker compose up -d init
sleep 5
docker compose logs init 2>&1 | tail -12
docker compose up -d show-ip
msg_ok "Collections ready"

# ── Detect IP ──────────────────────────────────
msg_info "Detecting server IP..."

detect_ip() {
  # Linux: ip / hostname
  local ip
  ip=$(ip -o -4 addr show scope global 2>/dev/null | head -1 | awk '{print $4}' | cut -d/ -f1)
  [ -n "$ip" ] && { echo "$ip"; return 0; }
  ip=$(hostname -I 2>/dev/null | awk '{print $1}')
  [ -n "$ip" ] && { echo "$ip"; return 0; }
  # Windows: try PowerShell (language-independent)
  ip=$(powershell.exe -NoProfile -Command "(Get-NetIPAddress -AddressFamily IPv4 | Where-Object { \$_.InterfaceAlias -notlike '*Loopback*' -and \$_.IPAddress -notlike '172.*' } | Select-Object -First 1).IPAddress" 2>/dev/null | tr -d '\r\n ')
  [ -n "$ip" ] && { echo "$ip"; return 0; }
  # Last resort: ipconfig regex
  ip=$(ipconfig 2>/dev/null | grep -oE '([0-9]{1,3}\.){3}[0-9]{1,3}' | grep -vE "^(127|172|255|0)\." | head -1)
  [ -n "$ip" ] && { echo "$ip"; return 0; }
  return 1
}

HOST_IP=$(detect_ip)
if [ -z "$HOST_IP" ]; then
  HOST_IP="YOUR-SERVER-IP"
  msg_warn "Could not detect IP. Find it manually: ip addr (Linux) or ipconfig (Windows)"
else
  msg_ok "Server IP: $HOST_IP"
fi

# ── Summary ────────────────────────────────────
echo ""
echo -e "${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${GREEN}FORGE is online!${NC}                                        ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     In the FORGE app, go to Settings >                   ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     PocketBase URL and paste:                            ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}http://${HOST_IP}:8090${NC}                                     ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Admin Panel:                                         ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}http://${HOST_IP}:8090/_/${NC}                                     ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Login: ${PB_EMAIL} / ${PB_PASS}        ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${YELLOW}CHANGE THE ADMIN PASSWORD IMMEDIATELY!${NC}               ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""
msg_ok "Installation complete!"
echo ""
msg_info "Next steps:"
echo "  1. Open the FORGE app on your phone"
echo "  2. Go to Settings > PocketBase URL"
echo "  3. Enter: http://${HOST_IP}:8090"
echo "  4. Sign up and start training!"
echo ""
