#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE

# ──────────────────────────────────────────────
# FORGE — Self-hosted Workout Tracker
# ──────────────────────────────────────────────
# PocketBase backend con auto-configurazione.
# Dopo l'esecuzione avrai:
#   • PocketBase in ascolto su porta 8090
#   • Admin pre-creato: admin@forge.local / forgeadmin123
#   • Collezioni pre-configurate con API rules
#   • Nessuna configurazione manuale necessaria
# ──────────────────────────────────────────────

set -uo pipefail

FORGE_VERSION="v0.8.0-beta"

# ── Colori ────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

# ── Header ────────────────────────────────────
echo ""
echo -e "${CYAN}   ███████╗ ██████╗ ██████╗  ██████╗ ███████╗${NC}"
echo -e "${CYAN}   ██╔════╝██╔═══██╗██╔══██╗██╔════╝ ██╔════╝${NC}"
echo -e "${CYAN}   █████╗  ██║   ██║██████╔╝██║  ███╗█████╗  ${NC}"
echo -e "${CYAN}   ██╔══╝  ██║   ██║██╔══██╗██║   ██║██╔══╝  ${NC}"
echo -e "${CYAN}   ██║     ╚██████╔╝██║  ██║╚██████╔╝███████╗${NC}"
echo -e "${CYAN}   ╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚════════╝${NC}"
echo ""
echo -e "           ${BOLD}Il diario di allenamento sociale${NC}"
echo -e "           ${FORGE_VERSION} — Self-Hosted Setup"
echo ""
echo -e "  ${CYAN}PocketBase ~25 MB RAM${NC} | ~30 MB disco | Docker"
echo -e "  1.500+ esercizi | Feed social | Achievement | Offline"
echo ""

# ── Funzioni helper (stile community-scripts) ──
msg_info()    { echo -e "${CYAN}[INFO]${NC}  $*"; }
msg_ok()      { echo -e "${GREEN}[ OK ]${NC}  $*"; }
msg_warn()    { echo -e "${YELLOW}[WARN]${NC}  $*"; }
msg_error()   { echo -e "${RED}[ERR ]${NC}  $*"; }
section()     { echo -e "\n${BOLD}── $* ──${NC}"; }

# ── Pre-checks ────────────────────────────────
section "Pre-checks"

if ! command -v docker &>/dev/null; then
  msg_error "Docker non è installato."
  msg_info  "Installalo: https://docs.docker.com/engine/install/"
  exit 1
fi
msg_ok "Docker $(docker --version | awk '{print $3}' | tr -d ',')"

if ! docker compose version &>/dev/null 2>&1 && ! docker-compose --version &>/dev/null 2>&1; then
  msg_error "Docker Compose non trovato."
  exit 1
fi
msg_ok "Docker Compose disponibile"

# ── Setup FORGE ───────────────────────────────
section "Setup FORGE"

FORGE_DIR="$HOME/forge-server"
REPO_URL="https://github.com/Leonardo-Lalli/FORGE.git"

if [ -d "$FORGE_DIR" ]; then
  msg_info "Directory $FORGE_DIR esiste gia, aggiorno..."
  cd "$FORGE_DIR"
  git pull --ff-only --quiet
else
  msg_info "Clonando $REPO_URL..."
  git clone --quiet "$REPO_URL" "$FORGE_DIR"
  cd "$FORGE_DIR"
fi
msg_ok "Repository pronta in $FORGE_DIR"

# ── Configurazione .env ───────────────────────
if [ ! -f "$FORGE_DIR/.env" ]; then
  msg_info "Creando .env..."
  cp "$FORGE_DIR/.env.example" "$FORGE_DIR/.env"
  msg_ok ".env creato (usa i default)"
else
  msg_ok ".env già presente"
fi

# ── Avvio PocketBase ──────────────────────────
section "Avvio PocketBase"

msg_info "Avvio container Docker..."
cd "$FORGE_DIR"
docker compose down --remove-orphans 2>&1 | tail -1
docker compose up -d pocketbase
msg_ok "PocketBase avviato"

# ── Attesa servizi ────────────────────────────
msg_info "Attesa PocketBase (max 60s)..."
for i in $(seq 1 30); do
  if curl -sf http://localhost:8090/api/health &>/dev/null 2>&1; then
    msg_ok "PocketBase e online"
    break
  fi
  sleep 2
done

# ── Creazione superuser e collezioni ───────────
msg_info "Creazione admin superuser..."
docker compose exec -T pocketbase pocketbase superuser create admin@forge.local forgeadmin123 2>&1 | tail -1 && \
  msg_ok "Superuser creato (admin@forge.local)" || \
  msg_ok "Superuser gia esistente"

msg_info "Inizializzazione collezioni..."
docker compose up -d init
sleep 5
docker compose logs init 2>&1 | tail -12
docker compose up -d show-ip
msg_ok "Collezioni create"

# ── IP del server ─────────────────────────────
msg_info "Rilevamento IP del server..."
HOST_IP=$(ip -o -4 addr show scope global 2>/dev/null | head -1 | awk '{print $4}' | cut -d/ -f1)
if [ -z "$HOST_IP" ]; then
  HOST_IP=$(hostname -I 2>/dev/null | awk '{print $1}')
fi
if [ -z "$HOST_IP" ]; then
  HOST_IP=$(wmic nicconfig where IPEnabled=true get IPAddress 2>/dev/null | grep -oE '[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+' | grep -vE "172\.|127\.|255\.|0\.0\.0\.0" | head -1)
fi
if [ -z "$HOST_IP" ]; then
  HOST_IP="INDIRIZZO-NON-TROVATO"
fi
msg_ok "IP server: $HOST_IP"

# ── Riepilogo finale ──────────────────────────
echo ""
echo -e "${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${GREEN}FORGE e online!${NC}                                    ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Nell'app FORGE, vai su Impostazioni >                ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     URL PocketBase e incolla:                             ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}http://${HOST_IP}:8090${NC}                                     ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Admin Panel:                                         ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${CYAN}→ http://${HOST_IP}:8090/_/${NC}                                 ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     Login: admin@forge.local / forgeadmin123             ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}     ${YELLOW}CAMBIA SUBITO LA PASSWORD ADMIN!${NC}                       ${BOLD}║${NC}"
echo -e "${BOLD}║${NC}                                                          ${BOLD}║${NC}"
echo -e "${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""

# ── Fine ──────────────────────────────────────
msg_ok "Installazione completata!"
echo ""
msg_info "Prossimi passi:"
echo "  1. Apri l'app FORGE sul telefono"
echo "  2. Vai su Impostazioni → URL PocketBase"
echo "  3. Inserisci: http://${HOST_IP}:8090"
echo "  4. Registrati e inizia ad allenarti!"
echo ""
