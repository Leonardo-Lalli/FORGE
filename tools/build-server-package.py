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
    for path in files + [root / 'docs/pocketbase-profile-migration.md']:
        archive.write(path, path.relative_to(root).as_posix())
print('Updated LXC script and archive')
