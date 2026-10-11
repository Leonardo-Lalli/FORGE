#!/bin/sh
# PocketBase >= 0.23. Creates missing collections; never overwrites live rules.
set -eu
PB_URL="${PB_URL:-http://pocketbase:8090}"
: "${PB_ADMIN_EMAIL:?Set PB_ADMIN_EMAIL}"
: "${PB_ADMIN_PASSWORD:?Set PB_ADMIN_PASSWORD}"
[ "$PB_ADMIN_PASSWORD" != 'forgeadmin123' ] || { echo 'Refusing the published default password.' >&2; exit 1; }
command -v jq >/dev/null
attempt=0
until curl -fsS --connect-timeout 3 --max-time 5 "${PB_URL}/api/health" >/dev/null 2>&1; do
  attempt=$((attempt + 1))
  [ "$attempt" -lt 30 ] || { echo 'PocketBase health timeout.' >&2; exit 1; }
  sleep 2
done
# jq escapes quotes/backslashes in credentials. No password/token is printed.
payload=$(jq -n --arg identity "$PB_ADMIN_EMAIL" --arg password "$PB_ADMIN_PASSWORD" '{identity:$identity,password:$password}')
ADMIN_TOKEN=$(printf '%s' "$payload" | curl -fsS --max-time 15 \
  "${PB_URL}/api/collections/_superusers/auth-with-password" \
  -H 'Content-Type: application/json' --data-binary @- | jq -er '.token')
unset payload
AUTH="Authorization: Bearer ${ADMIN_TOKEN}"
USERS_REF=$(curl -fsS --max-time 15 "${PB_URL}/api/collections/users" -H "$AUTH" | jq -er '.id')
collections=$(curl -fsS --max-time 15 "${PB_URL}/api/collections?perPage=500" -H "$AUTH")
create_if_missing() {
  if printf '%s' "$collections" | jq -e --arg name "$1" '.items[] | select(.name == $name)' >/dev/null; then
    echo "Existing collection $1: unchanged. Audit its live API rules separately."
  else
    printf '%s' "$2" | curl -fsS --max-time 15 -X POST "${PB_URL}/api/collections" \
      -H "$AUTH" -H 'Content-Type: application/json' --data-binary @- >/dev/null
    echo "Created collection: $1"
  fi
}
# Safest initial workout policy: only the owner may read/write. Friend feed and
# likes require a separately reviewed policy/endpoint; never grant arbitrary
# workout PATCH permissions to users merely because they liked a workout.
create_if_missing logged_workouts "$(jq -n --arg users "$USERS_REF" '{
  name:"logged_workouts",type:"base",fields:[
    {name:"user",type:"relation",required:true,collectionId:$users,maxSelect:1},
    {name:"user_name",type:"text"},{name:"name",type:"text",required:true},
    {name:"date",type:"date",required:true},{name:"notes",type:"text"},
    {name:"exercises",type:"json"},{name:"exercise_data",type:"json",maxSize:1048576},
    {name:"volume",type:"number",min:0},{name:"duration",type:"number",min:0},
    {name:"likes",type:"number",min:0},{name:"liked_by",type:"json"},
    {name:"photos",type:"json",maxSize:8388608}],
  listRule:"@request.auth.id != \"\" && user = @request.auth.id",
  viewRule:"@request.auth.id != \"\" && user = @request.auth.id",
  createRule:"@request.auth.id != \"\" && @request.body.user = @request.auth.id",
  updateRule:"@request.auth.id != \"\" && user = @request.auth.id && @request.body.user:changed = false",
  deleteRule:"@request.auth.id != \"\" && user = @request.auth.id"
}')"
create_if_missing social_graph "$(jq -n --arg users "$USERS_REF" '{
  name:"social_graph",type:"base",fields:[
    {name:"from_user",type:"relation",required:true,collectionId:$users,maxSelect:1},
    {name:"from_name",type:"text"},
    {name:"to_user",type:"relation",required:true,collectionId:$users,maxSelect:1},
    {name:"status",type:"select",required:true,maxSelect:1,values:["pending","accepted","rejected"]}],
  listRule:"@request.auth.id != \"\" && (from_user = @request.auth.id || to_user = @request.auth.id)",
  viewRule:"@request.auth.id != \"\" && (from_user = @request.auth.id || to_user = @request.auth.id)",
  createRule:"@request.auth.id != \"\" && @request.body.from_user = @request.auth.id && @request.body.to_user != @request.auth.id && @request.body.status = \"pending\"",
  updateRule:"@request.auth.id != \"\" && to_user = @request.auth.id && status = \"pending\" && @request.body.from_user:changed = false && @request.body.to_user:changed = false && @request.body.from_name:changed = false && (@request.body.status = \"accepted\" || @request.body.status = \"rejected\")",
  deleteRule:"@request.auth.id != \"\" && (from_user = @request.auth.id || to_user = @request.auth.id)"
}')"
# null = locked/superuser-only; an empty-auth expression admits guests!
create_if_missing excercise '{
  "name":"excercise","type":"base","fields":[
    {"name":"exercise_id","type":"text"},{"name":"name","type":"text","required":true},
    {"name":"bodyPart","type":"text"},{"name":"equipment","type":"text"},
    {"name":"instructions","type":"json"},{"name":"imageUrl","type":"url"},
    {"name":"category","type":"text"},{"name":"targetMuscles","type":"text"},
    {"name":"secondaryMuscles","type":"text"}],
  "listRule":"@request.auth.id != \"\"","viewRule":"@request.auth.id != \"\"",
  "createRule":null,"updateRule":null,"deleteRule":null
}'
echo 'Initialization completed. Existing collections were not modified.'
