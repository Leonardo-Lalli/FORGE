"""Read-only audit of the actual LXC PocketBase exercise table. No credentials needed.
Usage: python3 audit-pocketbase-exercises.py /absolute/pb_data/data.db
"""
import sqlite3,json,sys,pathlib
path=pathlib.Path(sys.argv[1]).resolve()
connection=sqlite3.connect(path.as_uri()+'?mode=ro',uri=True)
connection.execute('BEGIN')
columns={row[1] for row in connection.execute('PRAGMA table_info(excercise)')}
if not columns: raise SystemExit('excercise table missing')
rows=connection.execute('SELECT * FROM excercise'); names=[col[0] for col in rows.description]
entries=[dict(zip(names,row)) for row in rows]
def instructions(row):
 for key in ('instructions','instruction'):
  value=row.get(key)
  try: value=json.loads(value) if isinstance(value,str) else value
  except (ValueError,TypeError): pass
  if value is None: continue
  if isinstance(value,list):
   if any(item is not None and str(item).strip() for item in value): return True
  elif str(value).strip(): return True
 return False
def media(row): return row.get('imageUrl') or row.get('Image_url') or ''
result={'source':'PocketBase excercise (read-only database snapshot)','records':len(entries),'with_gif_url':sum('.gif' in media(e).lower().split('?')[0] for e in entries),'without_gif_url':sum('.gif' not in media(e).lower().split('?')[0] for e in entries),'with_any_media_url':sum(bool(media(e)) for e in entries),'with_written_instructions':sum(instructions(e) for e in entries),'with_media_and_instructions':sum(bool(media(e)) and instructions(e) for e in entries),'unique_names':len({str(e.get('name','')).strip().casefold() for e in entries}),'media_reachability':'URLs counted, files not downloaded'}
print(json.dumps(result,indent=2)); connection.close()
