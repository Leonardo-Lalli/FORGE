#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE

set -euo pipefail

FORGE_VERSION="beta — repository checkout"
FORGE_DIR="$HOME/forge-server"
REPO_URL="https://github.com/Leonardo-Lalli/FORGE.git"
PB_EMAIL="${PB_ADMIN_EMAIL:?Set PB_ADMIN_EMAIL before running the installer}"
PB_PASS="${PB_ADMIN_PASSWORD:?Set a unique PB_ADMIN_PASSWORD before running the installer}"
[ "$PB_PASS" != 'forgeadmin123' ] || { echo 'Refusing the published default password.' >&2; exit 1; }

# ── Colours ────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'
msg_ok()    { echo -e "${GREEN}[ OK ]${NC}  $*"; }
msg_info()  { echo -e "${CYAN}[INFO]${NC}  $*"; }
msg_warn()  { echo -e "${YELLOW}[WARN]${NC}  $*"; }
msg_error() { echo -e "${RED}[ERR ]${NC}  $*"; exit 1; }
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
echo -e "  PocketBase | Docker Compose | Version selected by the administrator"
echo -e "  Exercise catalogue cache | Community on this server | Achievements"
echo ""

# ── Pre-checks ─────────────────────────────────
section "Pre-checks"

if ! command -v docker &>/dev/null; then
  msg_error "Docker is not installed. Install it: https://docs.docker.com/engine/install/"
fi
msg_ok "Docker $(docker --version | awk '{print $3}' | tr -d ',')"

if ! docker compose version &>/dev/null 2>&1; then
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
  msg_info "Creating .env template..."
  cp "$FORGE_DIR/.env.example" "$FORGE_DIR/.env"
  msg_error "Fill .env with a reviewed PB_VERSION and unique administrator credentials, then rerun."
else
  msg_ok ".env already present"
fi

# ── Start PocketBase ───────────────────────────
section "Start PocketBase"

msg_info "Starting Docker containers..."
cd "$FORGE_DIR"
docker compose up -d --build pocketbase
msg_ok "PocketBase started"

msg_info "Waiting for PocketBase (max 60s)..."
healthy=0
for i in $(seq 1 30); do
  if curl -sf http://localhost:8090/api/health &>/dev/null 2>&1; then
    msg_ok "PocketBase is online"
    healthy=1
    break
  fi
  sleep 2
done
[ "$healthy" -eq 1 ] || msg_error 'PocketBase did not become healthy within 60 seconds.'

# ── Create superuser ───────────────────────────
msg_info "Creating superuser..."
if docker compose exec -T pocketbase pocketbase superuser create "$PB_EMAIL" "$PB_PASS" --dir=/pb_data >/dev/null 2>&1; then
  msg_ok "Superuser created ($PB_EMAIL)"
else
  msg_warn "Creation failed; the initializer will verify the existing credentials."
fi

# ── Init collections ───────────────────────────
msg_info "Initializing collections..."
docker compose run --rm init
msg_ok "Collections ready"
msg_info "Applying FORGE migrations with a database backup..."
if [ "$(id -u)" -eq 0 ]; then bash tools/apply-docker-profile-migration.sh;
else sudo bash tools/apply-docker-profile-migration.sh; fi
msg_info "GIF files remain on the provider. Remote social push requires separate Firebase setup."

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

HOST_IP=$(detect_ip) || HOST_IP=""
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
echo -e "${BOLD}║${NC}     PocketBase URL: your HTTPS proxy; local address:                            ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}http://127.0.0.1:8090${NC}                                     ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Admin Panel:                                         ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}http://127.0.0.1:8090/_/${NC}                                     ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Admin: ${PB_EMAIL} (password not displayed)        ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${YELLOW}KEEP ADMIN ACCESS PRIVATE AND BACK UP YOUR DATA${NC}               ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""
msg_ok "Installation complete!"
echo ""
msg_info "Next steps:"
echo "  1. Open the FORGE app on your phone"
echo "  2. Go to Settings > PocketBase URL"
echo "  3. Enter your HTTPS reverse-proxy URL (127.0.0.1 is for this server only)"
echo "  4. Sign up and start training!"
echo ""
