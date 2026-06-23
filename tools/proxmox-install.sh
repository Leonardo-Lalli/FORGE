#!/usr/bin/env bash

# Copyright (c) 2025-2026 Leonardo Lalli
# Author: Leonardo Lalli
# License: MIT | https://github.com/Leonardo-Lalli/FORGE/blob/main/LICENSE
# Source: https://github.com/Leonardo-Lalli/FORGE
#
# FORGE — Self-hosted Workout Tracker per Proxmox LXC
# Installa PocketBase nativamente (no Docker) e pre-configura
# admin, collezioni e API rules. Pronto all'uso in 30 secondi.

source /dev/stdin <<<"$FUNCTIONS_FILE_PATH"
color
verb_ip6
catch_errors
setting_up_container
network_check
update_os

PB_VERSION="0.25.9"
PB_ARCH=$(arch_resolve)
PB_USER="pocketbase"
PB_DIR="/opt/forge-pocketbase"
PB_EMAIL="admin@forge.local"
PB_PASS="forgeadmin123"

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

systemctl enable -q --now forge-pocketbase
msg_ok "Servizio avviato su porta 8090"

msg_info "Attesa avvio PocketBase..."
for i in $(seq 1 30); do
  if curl -sf http://localhost:8090/api/health &>/dev/null; then break; fi
  sleep 2
done
msg_ok "PocketBase online"

msg_info "Creazione superuser..."
"$PB_DIR/pocketbase" superuser create "$PB_EMAIL" "$PB_PASS" 2>/dev/null && \
  msg_ok "Superuser creato" || msg_ok "Superuser gia presente"

msg_info "Configurazione collezioni + API rules..."

# Authenticate (try both old and new PocketBase API)
TOKEN=$(curl -sf -X POST http://localhost:8090/api/collections/_superusers/auth-with-password \
  -H "Content-Type: application/json" \
  -d "{\"identity\":\"$PB_EMAIL\",\"password\":\"$PB_PASS\"}" \
  | grep -o '"token":"[^"]*"' | head -1 | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  TOKEN=$(curl -sf -X POST http://localhost:8090/api/admins/auth-with-password \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$PB_EMAIL\",\"password\":\"$PB_PASS\"}" \
    | grep -o '"token":"[^"]*"' | head -1 | cut -d'"' -f4)
fi

if [ -z "$TOKEN" ]; then
  msg_warn "Auth API fallita. Crea collezioni da Admin Panel:"
  msg_warn "Login: $PB_EMAIL / $PB_PASS"
else
  AUTH="Authorization: Bearer ${TOKEN}"
  API="http://localhost:8090/api/collections"

  create_collection() {
    local name="$1" json_file="$2"
    if curl -sf "$API" -H "$AUTH" | grep -q "\"name\":\"${name}\""; then
      msg_ok "  $name (esiste)"
      return
    fi
    curl -sf -X POST "$API" -H "$AUTH" -H "Content-Type: application/json" -d "@$json_file" >/dev/null && \
      msg_ok "  $name" || msg_warn "  $name (fallito)"
  }

  # Write JSON payloads to temp files (cleaner than inline escaping)
  cat > /tmp/forge_logged_workouts.json <<'ENDJSON'
{"name":"logged_workouts","type":"base","schema":[{"name":"user","type":"relation","required":true,"options":{"collectionId":"users","cascadeDelete":false}},{"name":"user_name","type":"text","required":false},{"name":"name","type":"text","required":true},{"name":"date","type":"date","required":true},{"name":"exercises","type":"json","required":false},{"name":"exercise_data","type":"json","required":false},{"name":"volume","type":"number","required":false},{"name":"duration","type":"number","required":false},{"name":"likes","type":"number","required":false},{"name":"liked_by","type":"json","required":false},{"name":"photos","type":"json","required":false}],"listRule":"@request.auth.id != '' && user = @request.auth.id","viewRule":"@request.auth.id != ''","createRule":"@request.auth.id != '' && user = @request.auth.id","updateRule":"@request.auth.id != '' && (user = @request.auth.id || @request.auth.id ?= liked_by)","deleteRule":"@request.auth.id != '' && user = @request.auth.id"}
ENDJSON

  cat > /tmp/forge_social_graph.json <<'ENDJSON'
{"name":"social_graph","type":"base","schema":[{"name":"from_user","type":"relation","required":true,"options":{"collectionId":"users","cascadeDelete":false}},{"name":"from_name","type":"text","required":false},{"name":"to_user","type":"relation","required":true,"options":{"collectionId":"users","cascadeDelete":false}},{"name":"status","type":"text","required":true}],"listRule":"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)","viewRule":"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)","createRule":"@request.auth.id != '' && from_user = @request.auth.id","updateRule":"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)","deleteRule":"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)"}
ENDJSON

  cat > /tmp/forge_excercise.json <<'ENDJSON'
{"name":"excercise","type":"base","schema":[{"name":"name","type":"text","required":true},{"name":"bodyPart","type":"text","required":false},{"name":"equipment","type":"text","required":false},{"name":"instructions","type":"json","required":false},{"name":"imageUrl","type":"url","required":false},{"name":"category","type":"text","required":false}],"listRule":"@request.auth.id != ''","viewRule":"@request.auth.id != ''","createRule":"@request.auth.id != ''","updateRule":"@request.auth.id = ''","deleteRule":"@request.auth.id = ''"}
ENDJSON

  create_collection "logged_workouts" "/tmp/forge_logged_workouts.json"
  create_collection "social_graph"    "/tmp/forge_social_graph.json"
  create_collection "excercise"       "/tmp/forge_excercise.json"
  rm -f /tmp/forge_*.json
fi

# --- MOTD ---
IP=$(hostname -I | awk '{print $1}')
cat <<EOF >> /etc/motd

   FORGE PocketBase
   App:  http://${IP}:8090
   Admin Panel: http://${IP}:8090/_/
   Login: $PB_EMAIL / $PB_PASS
EOF

msg_ok "Installazione completata!"

echo ""
echo "============================================"
echo "  FORGE PocketBase e online!"  
echo ""
echo "  App URL:  http://${IP}:8090"
echo "  Admin:    http://${IP}:8090/_/"
echo "  Login:    $PB_EMAIL / $PB_PASS"
echo ""
echo "  CAMBIA SUBITO LA PASSWORD ADMIN!"
echo "============================================"
echo ""

motd_ssh
customize
cleanup_lxc
