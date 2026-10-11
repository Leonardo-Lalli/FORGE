#!/usr/bin/env bash
set -euo pipefail
# Run from a reviewed repository checkout after `docker compose run --rm init`.
repo_root=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)
cd "$repo_root"
[ "$(id -u)" -eq 0 ] || { echo 'Run as root to back up Docker-owned database files.' >&2; exit 1; }
[ -f pb_data/data.db ] || { echo 'Initialize the existing database first; nothing changed.' >&2; exit 1; }
version=$(docker compose exec -T pocketbase pocketbase --version | awk '{print $NF}')
[ "$(printf '%s\n' 0.23.0 "$version" | sort -V | head -n1)" = 0.23.0 ] || { echo 'PocketBase 0.23+ required.' >&2; exit 1; }
backup="$repo_root/artifacts/server-backups/$(date +%Y%m%d-%H%M%S)"
mkdir -p "$backup"; chmod 700 "$backup"
docker compose stop pocketbase
cp -a -- pb_data "$backup/pb_data"
# Existing hook/migration sources must also be retained when replacing their deployment.
cp -a -- tools/pocketbase "$backup/server-code"
if docker compose run --rm --no-deps pocketbase migrate up --dir=/pb_data --migrationsDir=/forge_migrations --hooksDir=/pb_hooks; then
    docker compose up -d --no-deps pocketbase
    echo "Profile/privacy and shared catalog migration applied. Backup: $backup"
else
    echo "Migration failed. Service remains stopped; restore backup $backup before retrying." >&2
    exit 1
fi
