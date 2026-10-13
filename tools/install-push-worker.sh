#!/usr/bin/env bash
# Run only after Firebase credentials and the PocketBase internal address are known.
set -euo pipefail
umask 077
credentials=${1:?Usage: install-push-worker.sh /path/service-account.json http://127.0.0.1:PORT/api}
internal_api=${2:?Specify the PocketBase internal API URL}
[[ "$internal_api" =~ ^http://127\.0\.0\.1:[0-9]+/api$ ]] || { echo 'Use the local PocketBase URL, ending in /api.' >&2; exit 1; }
[ -f "$credentials" ] || exit 1
[ "$(id -u)" -eq 0 ] || { echo 'Run inside the LXC as root.' >&2; exit 1; }
script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
service_name=${3:-pocketbase}
[[ "$service_name" =~ ^[a-zA-Z0-9_.@-]+$ ]] || exit 1
systemctl cat "$service_name" >/dev/null
python3 - "$credentials" "$internal_api" <<'PY'
import json, sys, urllib.request
with open(sys.argv[1]) as file:
    data = json.load(file)
if data.get('type') != 'service_account' or not data.get('private_key') or not data.get('client_email'):
    raise SystemExit('Use a Firebase Admin service-account key, not google-services.json.')
with urllib.request.urlopen(sys.argv[2] + '/health', timeout=10) as response:
    if response.status != 200:
        raise SystemExit('PocketBase internal health check failed.')
print('Server credential format and internal PocketBase connection verified.')
PY
apt-get update
apt-get install -y python3-venv
id forge-push >/dev/null 2>&1 || useradd --system --no-create-home --shell /usr/sbin/nologin forge-push
install -d -m 750 -o root -g forge-push /etc/forge-push
install -m 640 -o root -g forge-push "$credentials" /etc/forge-push/service-account.json
install -d -m 755 /opt/forge-push
install -m 644 "$script_dir/forge-push-worker.py" /opt/forge-push/worker.py
python3 -m venv /opt/forge-push/venv
/opt/forge-push/venv/bin/pip install 'firebase-admin>=7,<8'
# The dedicated service user must read the runtime created under umask 077.
# Credentials remain separately protected in /etc/forge-push.
chmod -R a+rX /opt/forge-push
if [ ! -f /etc/forge-push/push.env ]; then
    worker_token=$(python3 -c 'import secrets; print(secrets.token_hex(32))')
    printf 'FORGE_PUSH_WORKER_TOKEN=%s\n' "$worker_token" > /etc/forge-push/push.env
fi
# Keep the shared worker secret stable on subsequent installs.
sed -i '/^GOOGLE_APPLICATION_CREDENTIALS=/d; /^FORGE_INTERNAL_API=/d' /etc/forge-push/push.env
printf 'GOOGLE_APPLICATION_CREDENTIALS=/etc/forge-push/service-account.json\nFORGE_INTERNAL_API=%s\n' "$internal_api" >> /etc/forge-push/push.env
chmod 600 /etc/forge-push/push.env
mkdir -p /etc/systemd/system/"$service_name".service.d
cat > /etc/systemd/system/"$service_name".service.d/forge-push.conf <<'EOF'
[Service]
EnvironmentFile=/etc/forge-push/push.env
EOF
cat > /etc/systemd/system/forge-push.service <<EOF
[Unit]
Description=FORGE Firebase push dispatcher
After=network-online.target ${service_name}.service
Wants=network-online.target
[Service]
User=forge-push
Group=forge-push
EnvironmentFile=/etc/forge-push/push.env
ExecStart=/opt/forge-push/venv/bin/python /opt/forge-push/worker.py
Restart=on-failure
RestartSec=15
NoNewPrivileges=true
PrivateTmp=true
ProtectSystem=strict
ProtectHome=true
[Install]
WantedBy=multi-user.target
EOF
systemctl daemon-reload
systemctl restart "$service_name"
systemctl enable --now forge-push
systemctl is-active "$service_name" forge-push
echo 'FCM worker installed. Test delivery on a registered phone; no router or Nginx changes were made.'
