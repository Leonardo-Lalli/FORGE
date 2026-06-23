#!/bin/sh
# FORGE — show the server IP after PocketBase is up
# Runs inside the show-ip container (alpine)

apk add --no-cache --quiet iproute2 >/dev/null 2>&1

# Detect host IP
HOST_IP=$(ip -o -4 addr show scope global | head -1 | awk '{print $4}' | sed 's/\/.*//')
if [ -z "$HOST_IP" ]; then
  HOST_IP=$(ip route get 1.1.1.1 2>/dev/null | grep -oP 'src \K\S+')
fi
if [ -z "$HOST_IP" ]; then
  HOST_IP='[esegui: ip addr show]'
fi

echo ''
echo ''
echo '╔══════════════════════════════════════════════════════════╗'
echo '║                                                          ║'
echo '║     FORGE PocketBase e online!                            ║'
echo '║                                                          ║'
echo '║     Connetti la app al server:                           ║'
echo "║     → http://${HOST_IP}:8090                                     ║"
echo '║                                                          ║'
echo '║     Admin Panel (solo da questa macchina):               ║'
echo '║     → http://localhost:8090/_/                                     ║'
echo '║                                                          ║'
echo '╚══════════════════════════════════════════════════════════╝'
echo ''
echo ''
