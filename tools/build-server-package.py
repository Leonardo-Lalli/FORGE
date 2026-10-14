"""Build the LXC paste block and archive from current server sources."""
from pathlib import Path
import zipfile

root = Path(__file__).resolve().parents[1]
output = root / 'artifacts'
output.mkdir(exist_ok=True)
files = [root / 'tools/apply-profile-migration.sh', root / 'tools/audit-pocketbase-exercises.py']
files += sorted((root / 'tools/pocketbase').glob('**/*.js'))
block = ["# Copiare tutto nella console LXC di PocketBase (Community Scripts).", "bash <<'FORGE_LXC_UPDATE'", 'set -euo pipefail', 'umask 077', 'forge_stage=$(mktemp -d /tmp/forge-server-update.XXXXXX)', 'mkdir -p "$forge_stage/tools/pocketbase/pb_migrations" "$forge_stage/tools/pocketbase/pb_hooks"']
for index, path in enumerate(files):
    relative = path.relative_to(root).as_posix()
    delimiter = f'FORGE_FILE_{index}'
    block += [f'cat > "$forge_stage/{relative}" <<\'{delimiter}\'', path.read_text(encoding='utf-8').rstrip(), delimiter]
block += ['bash "$forge_stage/tools/apply-profile-migration.sh" /opt/pocketbase pocketbase', 'systemctl is-active pocketbase', 'if command -v python3 >/dev/null; then python3 "$forge_stage/tools/audit-pocketbase-exercises.py" /opt/pocketbase/pb_data/data.db; fi', 'du -h /opt/pocketbase/pb_data/data.db', 'FORGE_LXC_UPDATE']
(output / 'forge-lxc-update.sh').write_text('\n'.join(block) + '\n', encoding='utf-8', newline='\n')
with zipfile.ZipFile(output / 'forge-profile-server-tools.zip', 'w', zipfile.ZIP_DEFLATED) as archive:
    extra = ['docs/pocketbase-profile-migration.md', 'docs/push-notifications.md', 'docs/self-hosting.md',
        'tools/forge-push-worker.py', 'tools/install-push-worker.sh', 'tools/apply-docker-profile-migration.sh',
        'tools/install.sh', 'tools/community-install.sh', 'tools/proxmox-install.sh', 'tools/proxmox-create.sh', 'tools/setup.ps1',
        'tools/init-pocketbase.sh', 'tools/show-ip.sh', 'tools/nginx-pocketbase-hardening.conf',
        'tools/docker/Dockerfile.pocketbase', 'tools/docker/Dockerfile.push', 'tools/docker/entrypoint.sh',
        'docker-compose.yml', '.env.example']
    for path in files + [root / name for name in extra]:
        archive.writestr(path.relative_to(root).as_posix(), path.read_text(encoding='utf-8').replace('\r\n', '\n'))
(output / 'forge-server-tools.zip').write_bytes((output / 'forge-profile-server-tools.zip').read_bytes())
print('Updated LXC script and archive')

# Standalone paste block for the confirmed CT 112 service address.
push_block = ["# Eseguire nel CT PocketBase dopo aver caricato la chiave Firebase Admin.", "bash <<'FORGE_PUSH_INSTALL'", 'set -euo pipefail', 'umask 077', '[ -f /root/forge-firebase-service-account.json ] || { echo "Manca la chiave Firebase Admin in /root/forge-firebase-service-account.json; nessuna modifica eseguita." >&2; exit 1; }', 'forge_push_stage=$(mktemp -d /tmp/forge-push-install.XXXXXX)']
for index, name in enumerate(['forge-push-worker.py', 'install-push-worker.sh']):
    delimiter = f'FORGE_PUSH_FILE_{index}'
    push_block += [f'cat > "$forge_push_stage/{name}" <<\'{delimiter}\'', (root / 'tools' / name).read_text(encoding='utf-8').rstrip(), delimiter]
push_block += ['bash "$forge_push_stage/install-push-worker.sh" /root/forge-firebase-service-account.json http://127.0.0.1:8080/api', 'FORGE_PUSH_INSTALL']
(output / 'forge-push-lxc-install.sh').write_text('\n'.join(push_block) + '\n', encoding='utf-8', newline='\n')
