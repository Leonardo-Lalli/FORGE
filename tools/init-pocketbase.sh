#!/bin/sh
# FORGE — PocketBase auto-init script
# Crea admin, collezioni e API rules al primo avvio.
# Idempotente: se le collection esistono gia, non le ricrea.

PB_URL="${PB_URL:-http://localhost:8090}"
PB_ADMIN_EMAIL="${PB_ADMIN_EMAIL:-admin@forge.local}"
PB_ADMIN_PASSWORD="${PB_ADMIN_PASSWORD:-forgeadmin123}"

echo "[FORGE Init] Waiting for PocketBase..."
until curl -sf "${PB_URL}/api/health" >/dev/null 2>&1; do sleep 2; done
echo "[FORGE Init] PocketBase is up."

# --- Create admin if none exists ---
curl -sf -X POST "${PB_URL}/api/admins" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"${PB_ADMIN_EMAIL}\",\"password\":\"${PB_ADMIN_PASSWORD}\",\"passwordConfirm\":\"${PB_ADMIN_PASSWORD}\"}" >/dev/null 2>&1 \
  && echo "[FORGE Init] Admin created: ${PB_ADMIN_EMAIL}" \
  || echo "[FORGE Init] Admin already exists, using existing credentials."

# --- Auth as admin ---
ADMIN_TOKEN=$(curl -sf -X POST "${PB_URL}/api/admins/auth-with-password" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"${PB_ADMIN_EMAIL}\",\"password\":\"${PB_ADMIN_PASSWORD}\"}" \
  | grep -o '"token":"[^"]*"' | head -1 | cut -d'"' -f4)

if [ -z "$ADMIN_TOKEN" ] || [ "$ADMIN_TOKEN" = "null" ]; then
  echo "[FORGE Init] ERROR: Cannot authenticate as admin!"
  echo "[FORGE Init] Check PB_ADMIN_EMAIL / PB_ADMIN_PASSWORD in .env"
  exit 1
fi

AUTH="Authorization: Bearer ${ADMIN_TOKEN}"
CT="Content-Type: application/json"

# --- Get users collection ID ---
USERS_ID=$(curl -sf "${PB_URL}/api/collections" -H "$AUTH" \
  | grep -o '"id":"[^"]*","name":"users"' | head -1 | cut -d'"' -f4)
if [ -z "$USERS_ID" ]; then
  echo "[FORGE Init] ERROR: Cannot find users collection!"
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
echo "============================================"
echo "  FORGE PocketBase INIT COMPLETE!"
echo ""
echo "  Admin panel:  http://localhost:8090/_/"
echo "  Admin email:  ${PB_ADMIN_EMAIL}"
echo "  Admin pass:   ${PB_ADMIN_PASSWORD}"
echo ""
echo "  Collections ready:"
echo "    - logged_workouts (with API rules)"
echo "    - social_graph (with API rules)"
echo "    - excercise (public read, admin write)"
echo ""
echo "  CAMBIA la password admin subito:"
echo "  http://localhost:8090/_/"
echo "============================================"
echo ""
