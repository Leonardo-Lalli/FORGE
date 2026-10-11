"""Focused local check for the shared cache, with a deterministic fake provider. Never touches production."""
from pathlib import Path
import json,subprocess,time,urllib.request,urllib.error,uuid,shutil
root=Path.cwd(); run=root/'artifacts'/('pb-catalog-test-'+uuid.uuid4().hex[:8])
for directory in ['pb_migrations','pb_hooks']: (run/directory).mkdir(parents=True,exist_ok=True)
fixture="""migrate(app => {
 const users=app.findCollectionByNameOrId('users'); users.createRule=''; app.save(users);
 app.save(new Collection({name:'excercise',type:'base',fields:[{name:'name',type:'text'},{name:'bodyPart',type:'text'},{name:'equipment',type:'text'},{name:'category',type:'text'},{name:'instruction',type:'text'},{name:'Image_url',type:'url'}]}));
});"""
(run/'pb_migrations/1791676700_fixture.js').write_text(fixture)
shutil.copy(root/'tools/pocketbase/pb_migrations/1791676900_forge_catalog_cache.js',run/'pb_migrations')
source=(root/'tools/pocketbase/pb_hooks/forge-catalog-cache.pb.js').read_text()
payload={'data':[{'exerciseId':'fixtureA','name':'Squat','gifUrl':'https://example.invalid/squat.gif','instructions':['Controlled movement'],'bodyParts':['upper legs'],'equipments':['body weight'],'targetMuscles':['quads'],'secondaryMuscles':[]}], 'meta':{'total':1,'hasNextPage':False,'nextCursor':''}}
http_line=next(line for line in source.splitlines() if 'const response = $http.send' in line)
hook=run/'pb_hooks/forge-catalog-cache.pb.js'
hook.write_text(source.replace(http_line,'    const response = '+json.dumps({'statusCode':200,'json':payload})+';'))
exe=root/'artifacts/pocketbase-test-runtime/pocketbase.exe'; base='http://127.0.0.1:18102/api/'; log=(run/'server.log').open('w'); process=None
def call(path,method='GET',data=None,token=None):
 headers={'Content-Type':'application/json'}
 if token: headers['Authorization']='Bearer '+token
 request=urllib.request.Request(base+path,data=None if data is None else json.dumps(data).encode(),headers=headers,method=method)
 try:
  with urllib.request.urlopen(request,timeout=10) as response: return response.status,json.load(response)
 except urllib.error.HTTPError as error: return error.code,json.loads(error.read())
def start():
 process=subprocess.Popen([str(exe),'serve','--http=127.0.0.1:18102','--dir='+str(run/'pb_data'),'--migrationsDir='+str(run/'pb_migrations'),'--hooksDir='+str(run/'pb_hooks')],stdout=log,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 for _ in range(70):
  if process.poll() is not None: raise RuntimeError('Temporary PocketBase failed to start; see local server.log')
  try:
   if call('health')[0]==200: return process
  except OSError: time.sleep(.15)
 process.terminate(); raise RuntimeError('Local health timeout')
def check(condition,message):
 if not condition: raise AssertionError(message)
 print('PASS '+message,flush=True)
try:
 process=start(); password=uuid.uuid4().hex
 check(call('collections/users/records','POST',{'email':'catalog@test.invalid','password':password,'passwordConfirm':password})[0]==200,'test account registration')
 token=call('collections/users/auth-with-password','POST',{'identity':'catalog@test.invalid','password':password})[1]['token']
 check(call('forge/catalog/page')[0]==401,'catalog requires authentication')
 status,data=call('forge/catalog/page',token=token)
 check(status==200,'provider page cached '+str(data) if status!=200 else 'provider page cached')
 records=call('collections/excercise/records',token=token)[1]['items']
 check(len(records)==1 and records[0]['imageUrl']==records[0]['Image_url'] and records[0]['instructions']==['Controlled movement'],'modern and legacy fields populated')
 check(call('collections/excercise/records','POST',{'name':'Forged'},token)[0]==403,'client cannot poison shared exercises')
 check(call('forge/catalog/page?after=untrusted',token=token)[0]==400,'unissued cursors denied')
 check(call('forge/catalog/status',token=token)[1]['complete'],'complete count uses provider total')
 process.terminate(); process.wait(timeout=15); process=None
 hook.write_text(source.replace(http_line,"    throw new ApiError(502, 'Fake provider offline');"))
 process=start()
 check(call('forge/catalog/page',token=token)[0]==200,'persistent page works with provider offline after restart')
 check(call('collections/excercise/records',token=token)[1]['totalItems']==1,'repeat page creates no duplicate records')
 print('CATALOG CACHE CHECK PASSED '+str(run),flush=True)
finally:
 if process is not None: process.terminate(); process.wait(timeout=15)
 log.close()
