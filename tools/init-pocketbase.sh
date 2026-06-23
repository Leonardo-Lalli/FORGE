#!/bin/sh
# FORGE — PocketBase collection initializer
# Esegue dopo che il superuser e stato creato (via CLI o web UI)
# Idempotente: se le collection esistono gia, non le ricrea.

PB_URL="${PB_URL:-http://pocketbase:8090}"
PB_ADMIN_EMAIL="${PB_ADMIN_EMAIL:-admin@forge.local}"
PB_ADMIN_PASSWORD="${PB_ADMIN_PASSWORD:-forgeadmin123}"

echo "[FORGE Init] Waiting for PocketBase..."
until curl -sf "${PB_URL}/api/health" >/dev/null 2>&1; do sleep 2; done
echo "[FORGE Init] PocketBase is up."

# --- Auth as admin (try both old v0.22 and new v0.23+ endpoints) ---
get_token() {
  # v0.22 format: email+password
  curl -sf -X POST "$1" -H "Content-Type: application/json" \
    -d "$2" 2>/dev/null | grep -o '"token":"[^"]*"' | head -1 | cut -d'"' -f4
}

ADMIN_TOKEN=$(get_token "${PB_URL}/api/admins/auth-with-password" \
  "{\"email\":\"${PB_ADMIN_EMAIL}\",\"password\":\"${PB_ADMIN_PASSWORD}\"}")

if [ -z "$ADMIN_TOKEN" ]; then
  ADMIN_TOKEN=$(get_token "${PB_URL}/api/collections/_superusers/auth-with-password" \
    "{\"identity\":\"${PB_ADMIN_EMAIL}\",\"password\":\"${PB_ADMIN_PASSWORD}\"}")
fi

if [ -z "$ADMIN_TOKEN" ]; then
  # v0.23+/v0.24+ might use email field still
  ADMIN_TOKEN=$(get_token "${PB_URL}/api/collections/_superusers/auth-with-password" \
    "{\"email\":\"${PB_ADMIN_EMAIL}\",\"password\":\"${PB_ADMIN_PASSWORD}\"}")
fi

if [ -z "$ADMIN_TOKEN" ]; then
  echo ""
  echo "[FORGE Init] ============================================"
  echo "[FORGE Init] Superuser non trovato. Creane uno prima:"
  echo "[FORGE Init]"
  echo "[FORGE Init]   docker compose exec -T pocketbase pocketbase superuser create ${PB_ADMIN_EMAIL} ${PB_ADMIN_PASSWORD}"
  echo "[FORGE Init]"
  echo "[FORGE Init] Oppure apri http://localhost:8090/_/"
  echo "[FORGE Init] e crea l'admin da web UI."
  echo "[FORGE Init] Poi rilancia: docker compose up -d init"
  echo "[FORGE Init] ============================================"
  echo ""
  exit 1
fi

echo "[FORGE Init] Authenticated."

AUTH="Authorization: Bearer ${ADMIN_TOKEN}"
CT="Content-Type: application/json"

# --- Get users collection ID ---
COLLECTIONS=$(curl -sf "${PB_URL}/api/collections" -H "$AUTH")
USERS_ID=$(echo "$COLLECTIONS" | grep -o '{"[^}]*"name":"users"[^}]*}' | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4)

if [ -z "$USERS_ID" ]; then
  USERS_ID=$(echo "$COLLECTIONS" | grep -o '"id":"[^"]*"' | while read -r pair; do
    echo "$pair"
  done | head -1 | cut -d'"' -f4)
fi

if [ -z "$USERS_ID" ]; then
  echo "[FORGE Init] ERROR: Cannot find users collection!"
  echo "[FORGE Init] Raw: ${COLLECTIONS}"
  exit 1
fi
echo "[FORGE Init] Users collection ID: ${USERS_ID}"

# --- Helper: create collection if not exists ---
create_if_missing() {
  local name="$1" json="$2"
  if curl -sf "${PB_URL}/api/collections" -H "$AUTH" | grep -q "\"name\":\"${name}\""; then
    echo "[FORGE Init] Collection '${name}' already exists, skipping."
  else
    curl -sf -X POST "${PB_URL}/api/collections" -H "$AUTH" -H "$CT" -d "$json" >/dev/null
    echo "[FORGE Init] Created collection: ${name}"
  fi
}

# --- logged_workouts ---
create_if_missing "logged_workouts" "{
  \"name\":\"logged_workouts\",
  \"type\":\"base\",
  \"schema\":[
    {\"name\":\"user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"${USERS_ID}\",\"cascadeDelete\":false}},
    {\"name\":\"user_name\",\"type\":\"text\",\"required\":false},
    {\"name\":\"name\",\"type\":\"text\",\"required\":true},
    {\"name\":\"date\",\"type\":\"date\",\"required\":true},
    {\"name\":\"exercises\",\"type\":\"json\",\"required\":false},
    {\"name\":\"exercise_data\",\"type\":\"json\",\"required\":false},
    {\"name\":\"volume\",\"type\":\"number\",\"required\":false},
    {\"name\":\"duration\",\"type\":\"number\",\"required\":false},
    {\"name\":\"likes\",\"type\":\"number\",\"required\":false},
    {\"name\":\"liked_by\",\"type\":\"json\",\"required\":false},
    {\"name\":\"photos\",\"type\":\"json\",\"required\":false}
  ],
  \"listRule\":\"@request.auth.id != '' && user = @request.auth.id\",
  \"viewRule\":\"@request.auth.id != ''\",
  \"createRule\":\"@request.auth.id != '' && user = @request.auth.id\",
  \"updateRule\":\"@request.auth.id != '' && (user = @request.auth.id || @request.auth.id ?= liked_by)\",
  \"deleteRule\":\"@request.auth.id != '' && user = @request.auth.id\"
}"

# --- social_graph ---
create_if_missing "social_graph" "{
  \"name\":\"social_graph\",
  \"type\":\"base\",
  \"schema\":[
    {\"name\":\"from_user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"${USERS_ID}\",\"cascadeDelete\":false}},
    {\"name\":\"from_name\",\"type\":\"text\",\"required\":false},
    {\"name\":\"to_user\",\"type\":\"relation\",\"required\":true,\"options\":{\"collectionId\":\"${USERS_ID}\",\"cascadeDelete\":false}},
    {\"name\":\"status\",\"type\":\"text\",\"required\":true}
  ],
  \"listRule\":\"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)\",
  \"viewRule\":\"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)\",
  \"createRule\":\"@request.auth.id != '' && from_user = @request.auth.id\",
  \"updateRule\":\"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)\",
  \"deleteRule\":\"@request.auth.id != '' && (from_user = @request.auth.id || to_user = @request.auth.id)\"
}"

# --- excercise ---
create_if_missing "excercise" "{
  \"name\":\"excercise\",
  \"type\":\"base\",
  \"schema\":[
    {\"name\":\"name\",\"type\":\"text\",\"required\":true},
    {\"name\":\"bodyPart\",\"type\":\"text\",\"required\":false},
    {\"name\":\"equipment\",\"type\":\"text\",\"required\":false},
    {\"name\":\"instructions\",\"type\":\"json\",\"required\":false},
    {\"name\":\"imageUrl\",\"type\":\"url\",\"required\":false},
    {\"name\":\"category\",\"type\":\"text\",\"required\":false}
  ],
  \"listRule\":\"@request.auth.id != ''\",
  \"viewRule\":\"@request.auth.id != ''\",
  \"createRule\":\"@request.auth.id != ''\",
  \"updateRule\":\"@request.auth.id = ''\",
  \"deleteRule\":\"@request.auth.id = ''\"
}"

echo ""
echo "[FORGE Init] Collections ready: logged_workouts, social_graph, excercise"
echo "[FORGE Init] DONE."
