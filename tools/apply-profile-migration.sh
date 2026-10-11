#!/usr/bin/env bash
set -euo pipefail
# Usage: sudo bash tools/apply-profile-migration.sh /absolute/pocketbase-dir service-name [absolute/pb_data]
: "${1:?Pass the directory containing your existing pocketbase executable}"
: "${2:?Pass the existing systemd service name}"
pb_root=$(realpath -- "$1")
service_name=$2
pb_data=$(realpath -- "${3:-$pb_root/pb_data}")
script_root=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
[ "$(id -u)" -eq 0 ] || { echo 'Run in the LXC console as root.' >&2; exit 1; }
[ -x "$pb_root/pocketbase" ] && [ -f "$pb_data/data.db" ] || { echo 'Executable or existing database missing; nothing changed.' >&2; exit 1; }
[[ "$service_name" =~ ^[a-zA-Z0-9_.@-]+$ ]] || exit 1
systemctl cat "$service_name" >/dev/null
version=$("$pb_root/pocketbase" --version | awk '{print $NF}')
[ "$(printf '%s\n' 0.23.0 "$version" | sort -V | head -n1)" = 0.23.0 ] || { echo 'PocketBase 0.23+ required. Upgrade separately with backup first.' >&2; exit 1; }
backup="/var/backups/forge-profiles-$(date +%Y%m%d-%H%M%S)"
was_active=0
applied=0
if systemctl is-active --quiet "$service_name"; then was_active=1; fi
restart_existing() {
    if [ "$applied" -eq 1 ] && [ "$was_active" -eq 1 ]; then systemctl start "$service_name";
    elif [ "$applied" -eq 0 ]; then echo "Migration not completed; service remains stopped. Review the error and backup at $backup before restarting." >&2; fi
}
trap restart_existing EXIT
systemctl stop "$service_name"
mkdir -p "$backup"
cp -a -- "$pb_data" "$backup/pb_data"
for folder in pb_migrations pb_hooks; do
    if [ -d "$pb_root/$folder" ]; then cp -a -- "$pb_root/$folder" "$backup/$folder"; fi
    mkdir -p "$pb_root/$folder"
done
cp -- "$script_root"/pocketbase/pb_migrations/*.js "$pb_root/pb_migrations/"
cp -- "$script_root"/pocketbase/pb_hooks/*.pb.js "$pb_root/pb_hooks/"
"$pb_root/pocketbase" migrate up --dir="$pb_data" --migrationsDir="$pb_root/pb_migrations" --hooksDir="$pb_root/pb_hooks"
applied=1
# Preserve the existing service user's ability to read the new files.
chown --reference="$pb_data" "$pb_root"/pb_migrations/*forge*.js "$pb_root"/pb_hooks/forge*.pb.js
echo "Migration applied. Backup: $backup"
echo 'All accounts without a visibility choice now start PRIVATE. Choose Public or Friends in the app.'
echo 'Friends means an accepted follow in BOTH directions. Nginx and router settings were not changed.'
echo 'Shared exercise metadata/URL cache installed. Restart the app to populate it; GIF files remain on the provider.'
echo 'Follower/following visibility is separate and defaults PRIVATE. Change it in Edit account.'
