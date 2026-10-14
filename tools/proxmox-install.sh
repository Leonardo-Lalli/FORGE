#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE
#
# FORGE — Self-hosted Workout Tracker per Proxmox LXC
# Installa PocketBase nativamente (no Docker) e pre-configura
# admin, collezioni, hook e migrazioni FORGE.

set -euo pipefail
[ "$(id -u)" -eq 0 ] || { echo 'Run inside a Debian/Ubuntu LXC as root.' >&2; exit 1; }
command -v systemctl >/dev/null || { echo 'A systemd distribution is required.' >&2; exit 1; }
: "${PB_ADMIN_EMAIL:?Set PB_ADMIN_EMAIL}"
: "${PB_ADMIN_PASSWORD:?Set a unique PB_ADMIN_PASSWORD}"
: "${PB_VERSION:?Set a reviewed PocketBase version}"
[ "$PB_ADMIN_PASSWORD" != 'forgeadmin123' ] || { echo 'Refusing published default password.' >&2; exit 1; }
FORGE_INIT_SCRIPT="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)/init-pocketbase.sh"
[ -f "$FORGE_INIT_SCRIPT" ] || { echo 'Run from a reviewed local repository checkout.' >&2; exit 1; }
apt-get update
apt-get install -y curl jq unzip wget ca-certificates
msg_info() { printf '%s\n' "$*"; }
msg_ok() { printf '%s\n' "$*"; }
msg_error() { printf '%s\n' "$*" >&2; }
arch_resolve() { case "$(uname -m)" in x86_64) echo amd64;; aarch64) echo arm64;; *) exit 1;; esac; }

PB_VERSION="${PB_VERSION}"
PB_ARCH=$(arch_resolve)
PB_USER="pocketbase"
PB_DIR="/opt/forge-pocketbase"
PB_EMAIL="${PB_ADMIN_EMAIL:?Set PB_ADMIN_EMAIL before running the installer}"
PB_PASS="${PB_ADMIN_PASSWORD:?Set a unique PB_ADMIN_PASSWORD before running the installer}"

msg_info "Installazione PocketBase v${PB_VERSION}"

RELEASE_URL="https://github.com/pocketbase/pocketbase/releases/download/v${PB_VERSION}/pocketbase_${PB_VERSION}_linux_${PB_ARCH}.zip"
msg_info "Download da GitHub..."
wget -qO /tmp/pocketbase.zip "$RELEASE_URL" || {
  msg_error "Download fallito (versione: ${PB_VERSION}, arch: ${PB_ARCH})"
  exit 1
}

msg_info "Preparazione directory..."
mkdir -p "$PB_DIR"/{pb_data,pb_public,pb_migrations,pb_hooks}
unzip -qo /tmp/pocketbase.zip -d "$PB_DIR"
rm /tmp/pocketbase.zip
chmod +x "$PB_DIR/pocketbase"
msg_ok "PocketBase installato in $PB_DIR"

if ! id "$PB_USER" &>/dev/null; then
  useradd -r -s /bin/false -d "$PB_DIR" "$PB_USER"
fi
chown -R "$PB_USER:$PB_USER" "$PB_DIR"

msg_info "Creazione servizio systemd..."
cat <<EOF >/etc/systemd/system/forge-pocketbase.service
[Unit]
Description = FORGE PocketBase Backend
After        = network.target

[Service]
Type           = simple
User           = $PB_USER
Group          = $PB_USER
LimitNOFILE    = 4096
Restart        = always
RestartSec     = 5s
StandardOutput = append:$PB_DIR/pb_data/access.log
StandardError  = append:$PB_DIR/pb_data/errors.log
WorkingDirectory = $PB_DIR
ExecStart      = $PB_DIR/pocketbase serve --http=0.0.0.0:8090

[Install]
WantedBy = multi-user.target
EOF

systemctl daemon-reload
systemctl enable -q --now forge-pocketbase
msg_ok "Servizio avviato su porta 8090"

msg_info "Attesa avvio PocketBase..."
healthy=0
for i in $(seq 1 30); do
  if curl -sf http://localhost:8090/api/health &>/dev/null; then healthy=1; break; fi
  sleep 2
done
[ "$healthy" -eq 1 ] || { msg_error "PocketBase health timeout"; exit 1; }
msg_ok "PocketBase online"

msg_info "Creazione superuser..."
"$PB_DIR/pocketbase" superuser create "$PB_EMAIL" "$PB_PASS" --dir="$PB_DIR/pb_data" >/dev/null 2>&1 && \
  msg_ok "Superuser creato" || msg_info "Creazione superuser non riuscita; l’inizializzatore verificherà le credenziali esistenti."

msg_info "Configurazione collezioni + API rules..."

PB_URL=http://127.0.0.1:8090 PB_ADMIN_EMAIL="$PB_EMAIL" PB_ADMIN_PASSWORD="$PB_PASS" sh "$FORGE_INIT_SCRIPT"
msg_info 'Applying FORGE migrations with a backup...'
bash "$(dirname "$FORGE_INIT_SCRIPT")/apply-profile-migration.sh" "$PB_DIR" forge-pocketbase

# --- MOTD ---
IP=$(hostname -I | awk '{print $1}')
cat <<EOF >> /etc/motd

   FORGE PocketBase
   App:  http://${IP}:8090
   Admin Panel: http://${IP}:8090/_/
   Admin: $PB_EMAIL (password not displayed)
EOF

msg_ok "Installazione completata!"

echo ""
echo "============================================"
echo "  FORGE PocketBase e online!"  
echo ""
echo "  App URL:  http://${IP}:8090"
echo "  Admin:    http://${IP}:8090/_/"
echo "  Admin:    $PB_EMAIL (password not displayed)"
echo ""
echo "  Mantieni privata l’amministrazione e conserva i backup."
echo "============================================"
echo ""
