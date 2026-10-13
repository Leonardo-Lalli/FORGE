#!/bin/sh
set -eu
if [ -n "${PB_ENCRYPTION_KEY:-}" ]; then
    exec pocketbase "$@" --encryptionEnv=PB_ENCRYPTION_KEY
fi
exec pocketbase "$@"
