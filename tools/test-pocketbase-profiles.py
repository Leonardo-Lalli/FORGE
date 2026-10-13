from pathlib import Path
import json,subprocess,time,urllib.request,urllib.error,uuid,shutil,base64,os
root=Path.cwd(); run=root/'artifacts'/('pb-profile-test-'+uuid.uuid4().hex[:8]); (run/'pb_migrations').mkdir(parents=True); (run/'pb_hooks').mkdir()
fixture="""migrate((app) => {
const users=app.findCollectionByNameOrId('users'); users.fields.add(new TextField({name:'bio'})); users.fields.getByName('avatar').protected=true; users.createRule=''; app.save(users);
const social=new Collection({name:'social_graph',type:'base',fields:[{name:'from_user',type:'text'},{name:'to_user',type:'text'},{name:'from_name',type:'text'},{name:'status',type:'select',values:['pending','accepted','rejected'],maxSelect:1}]}); app.save(social);
const workouts=new Collection({name:'logged_workouts',type:'base',fields:[{name:'user',type:'relation',collectionId:users.id,maxSelect:1},{name:'name',type:'text'},{name:'date',type:'date'},{name:'notes',type:'text'},{name:'volume',type:'number'},{name:'duration',type:'number'},{name:'likes',type:'number'},{name:'liked_by',type:'json'},{name:'exercise_data',type:'json'}]}); app.save(workouts);
app.save(new Collection({name:'excercise',type:'base',fields:[{name:'name',type:'text'},{name:'bodyPart',type:'text'},{name:'equipment',type:'text'},{name:'category',type:'text'},{name:'instruction',type:'text'},{name:'Image_url',type:'url'}]}));
});"""
(run/'pb_migrations'/'1791676700_test_fixture.js').write_text(fixture)
for p in (root/'tools/pocketbase/pb_migrations').glob('*.js'): shutil.copy(p,run/'pb_migrations'/p.name)
for p in (root/'tools/pocketbase/pb_hooks').glob('*.js'): shutil.copy(p,run/'pb_hooks'/p.name)
exe=root/'artifacts/pocketbase-test-runtime/pocketbase.exe'; log=(run/'server.log').open('w'); base='http://127.0.0.1:18097/api/'
worker_secret=uuid.uuid4().hex
p=subprocess.Popen([str(exe),'serve','--http=127.0.0.1:18097', '--dir='+str(run/'pb_data'),'--migrationsDir='+str(run/'pb_migrations'),'--hooksDir='+str(run/'pb_hooks')],stdout=log,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW,env={**os.environ,'FORGE_PUSH_WORKER_TOKEN':worker_secret})
def call(path,method='GET',data=None,token=None,worker=False):
 headers={'Content-Type':'application/json'}
 if token: headers['Authorization']='Bearer '+token
 if worker: headers['X-Forge-Worker']=worker_secret
 req=urllib.request.Request(base+path,data=None if data is None else json.dumps(data).encode(),headers=headers,method=method)
 try:
  with urllib.request.urlopen(req,timeout=10) as response: return response.status, {} if response.status==204 else json.load(response)
 except urllib.error.HTTPError as e: return e.code,json.loads(e.read())
def check(condition,message):
 if not condition: raise AssertionError(message)
 print('PASS '+message,flush=True)
def upload_avatar(user,token):
 boundary=uuid.uuid4().hex
 png=base64.b64decode('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aX1cAAAAASUVORK5CYII=')
 data=('--'+boundary+'\r\nContent-Disposition: form-data; name="avatar"; filename="avatar.png"\r\nContent-Type: image/png\r\n\r\n').encode()+png+('\r\n--'+boundary+'--\r\n').encode()
 req=urllib.request.Request(base+'collections/users/records/'+user,data=data,headers={'Authorization':'Bearer '+token,'Content-Type':'multipart/form-data; boundary='+boundary},method='PATCH')
 with urllib.request.urlopen(req,timeout=10) as response: return json.load(response)['avatar']
try:
 for _ in range(70):
  if p.poll() is not None: raise RuntimeError((run/'server.log').read_text())
  try:
   if call('health')[0]==200: break
  except OSError: time.sleep(.15)
 else: raise RuntimeError('Health timeout')
 accounts={}
 for name in ['alice','bob','carol']:
  password=uuid.uuid4().hex
  code,user=call('collections/users/records','POST',{'email':name+'@test.invalid','password':password,'passwordConfirm':password,'name':name})
  check(code==200,'register '+name+' '+str(user) if code!=200 else 'register '+name)
  _,auth=call('collections/users/auth-with-password','POST',{'identity':name+'@test.invalid','password':password})
  accounts[name]=(user['id'],auth['token'])
  if name=='alice': alice_password=password
 a,at=accounts['alice']; b,bt=accounts['bob']; c,ct=accounts['carol']
 avatar=upload_avatar(a,at)
 file_path='files/users/'+a+'/'+avatar
 try:
  urllib.request.urlopen(base+file_path,timeout=10)
  raise AssertionError('Protected avatar unexpectedly public')
 except urllib.error.HTTPError as error: check(error.code==404,'protected avatar without file token returns 404')
 _,file_auth=call('files/token','POST',{},at)
 with urllib.request.urlopen(base+file_path+'?token='+file_auth['token'],timeout=10) as response: check(response.status==200,'protected avatar loads with short file token')
 check(call('collections/users/records/'+a,token=at)[1]['workout_visibility']=='private','new user defaults private')
 code,workout=call('collections/logged_workouts/records','POST',{'user':a,'name':'Own workout','volume':100,'liked_by':[]},at)
 check(code==200,'owner creates workout '+str(workout) if code!=200 else 'owner creates workout')
 wid=workout['id']; path='collections/logged_workouts/records/'+wid
 check(call(path,token=bt)[0]==404,'private direct view denied')
 check(call('collections/logged_workouts/records',token=bt)[1]['totalItems']==0,'private list filtered')
 check(call(path,token=at)[0]==200,'owner sees private workout')
 code, body = call('collections/users/records/'+a,'PATCH',{'workout_visibility':'public','featured_achievements':['first_workout']},at)
 check(code==200,'profile options saved: '+str(body))
 check(call(path,token=bt)[0]==200,'public visible to authenticated users')
 check(call(path)[0]==404,'anonymous workout view denied')
 check(call(path,'PATCH',{'notes':'overwrite'},bt)[0]==404,'nonowner cannot alter notes')
 check(call(path,'PATCH',{'liked_by':[b],'likes':1},bt)[0]==200,'first like permitted')
 check(call(path,'PATCH',{'liked_by':[b,c],'likes':2},bt)[0]==403,'cannot forge somebody else like')
 check(call(path,'PATCH',{'liked_by':[],'likes':0},bt)[0]==200,'unlike permitted')
 check(call(path,'PATCH',{'user':b},at)[0]==404,'ownership immutable')
 call('collections/users/records/'+a,'PATCH',{'workout_visibility':'friends'},at)
 check(call(path,token=bt)[0]==404,'friends requires reciprocity')
 _,follow=call('collections/social_graph/records','POST',{'from_user':b,'to_user':a,'status':'pending'},bt)
 check(call('collections/social_graph/records/'+follow['id'],'PATCH',{'status':'accepted'},bt)[0]==404,'sender cannot self accept')
 check(call('collections/social_graph/records/'+follow['id'],'PATCH',{'status':'accepted'},at)[0]==200,'recipient accepts')
 check(call(path,token=bt)[0]==404,'one direction not friendship')
 _,back=call('collections/social_graph/records','POST',{'from_user':a,'to_user':b,'status':'pending'},at)
 check(call('collections/social_graph/records/'+back['id'],'PATCH',{'status':'accepted'},bt)[0]==200,'reciprocal follow accepted')
 check(call(path,token=bt)[0]==200,'reciprocal friend sees workout')
 check(call('forge/connections/'+a,token=bt)[1]['allowed'] is False,'connections default private even for friend')
 check(call('collections/users/records/'+a,'PATCH',{'connections_visibility':'friends'},at)[0]==200,'connections privacy saved')
 check(call('forge/connections/'+a,token=bt)[1]['allowed'] is True,'mutual friend can view friends connections')
 check(len(call('forge/connections/'+a,token=bt)[1]['items'])==2,'connections route returns both accepted directions')
 check(call('forge/connections/'+a,token=ct)[1]['allowed'] is False,'unrelated user cannot view friends connections')
 check(call('collections/social_graph/records',token=ct)[1]['totalItems']==0,'direct graph cannot bypass list privacy')
 call('collections/users/records/'+a,'PATCH',{'connections_visibility':'public'},at)
 check(call('forge/connections/'+a,token=ct)[1]['allowed'] is True,'public connections visible to authenticated users')
 check(call('forge/connections/'+a)[0]==401,'anonymous connections denied')
 call('collections/users/records/'+a,'PATCH',{'connections_visibility':'friends'},at)
 check(call(path,token=ct)[0]==404,'unrelated user denied friends workout')
 check(call('collections/logged_workouts/records',token=bt)[1]['totalItems']==1,'friend list includes workout')
 check(call('collections/users/records/'+a,'PATCH',{'featured_achievements':['a','b','c','d']},at)[0]==400,'invalid badge IDs denied')
 check(call('collections/users/records/'+a,'PATCH',{'featured_achievements':['first_workout','holy_week','habit','member']},at)[0]==200,'more than three valid badges allowed')
 check(call(path,'PATCH',{'photos':['data:image/jpeg;base64,aGVsbG8=']},at)[0]==200,'workout photo JSON accepted')
 check(call('forge/push/device','POST',{'token':'test-device-token-alice-12345'},at)[0]==200,'device registration authenticated')
 check(call('forge/push/device','POST',{'token':'test-device-token-alice-12345'})[0]==401,'anonymous device registration denied')
 check(call('collections/forge_push_devices/records',token=bt)[0]==403,'device tokens inaccessible through collection API')
 check(call('forge/push/pending',token=at)[0]==403,'worker queue denies regular users')
 check(call('forge/notifications/achievement','POST',{'ids':['first_workout','habit']},at)[0]==200,'achievement batch queued')
 check(call('forge/notifications/achievement','POST',{'id':'made_up_badge'},at)[0]==400,'unknown achievement notification denied')
 check(call('collections/forge_notifications/records',token=at)[1]['totalItems']>=2,'own notifications visible')
 check(call('collections/forge_notifications/records',token=ct)[1]['totalItems']==0,'other notification inbox private')
 check(len(call('forge/push/pending',worker=True)[1]['items'])==2,'worker can fetch queued achievement batch')
 call('forge/notifications/achievement','POST',{'ids':['first_workout','habit']},at)
 check(len(call('forge/push/pending',worker=True)[1]['items'])==2,'achievement notifications deduplicated')
 call('forge/push/device','POST',{'token':'test-device-token-bob-123456'},bt)
 call('collections/users/records/'+a,'PATCH',{'workout_visibility':'public'},at)
 code, shared=call('collections/logged_workouts/records','POST',{'user':a,'name':'Shared notification test'},at)
 check(code==200,'public workout created for notification check')
 check(any(shared['id'] in i['route'] for i in call('forge/push/pending',worker=True)[1]['items']),'public workout push queued for follower')
 call('collections/users/records/'+a,'PATCH',{'workout_visibility':'private'},at)
 check(not any(shared['id'] in i['route'] for i in call('forge/push/pending',worker=True)[1]['items']),'privacy rechecked before pending push delivery')
 check(call('collections/social_graph/records/'+back['id'],'DELETE',token=at)[0]==204,'sender can unfollow')
 check(call('forge/connections/'+a,token=bt)[1]['allowed'] is False,'unfollow revokes friends connections access')
 check(call('collections/users/records/'+a,'DELETE',token=at)[0]==403,'direct account deletion blocked')
 check(call('forge/account/delete','POST',{'password':'wrong'},at)[0]==403,'account deletion requires correct password')
 check(call(path,token=at)[0]==200,'wrong password leaves account data intact')
 check(call('forge/account/delete','POST',{'password':alice_password},at)[0]==200,'password-confirmed deletion succeeds')
 check(call('collections/users/records/'+b,token=bt)[0]==200,'other account preserved')
 check(call(path,token=bt)[0]==404,'deleted account workout removed')
 (run/'result.json').write_text(json.dumps({'passed':True,'version':'0.40.5','scope':'local disposable server'}))
 print('ALL PROFILE PRIVACY CHECKS PASSED '+str(run),flush=True)
finally:
 p.terminate(); p.wait(timeout=15); log.close()

