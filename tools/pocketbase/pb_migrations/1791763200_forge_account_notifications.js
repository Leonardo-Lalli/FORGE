migrate((app) => {
    const users = app.findCollectionByNameOrId('users');
    users.deleteRule = null; // Account deletion goes through password-confirmed cleanup.
    app.save(users);
    const workouts = app.findCollectionByNameOrId('logged_workouts');
    const photos = workouts.fields.getByName('photos');
    if (!photos) workouts.fields.add(new JSONField({ name: 'photos', maxSize: 6291456 }));
    else if (photos.type === 'json') photos.maxSize = Math.max(photos.maxSize, 6291456);
    app.save(workouts);
    const devices = new Collection({ name: 'forge_push_devices', type: 'base', fields: [
        {name:'user',type:'relation',collectionId:users.id,maxSelect:1,required:true}, {name:'token',type:'text',required:true,max:4096}
    ], indexes: ['CREATE UNIQUE INDEX forge_push_token ON forge_push_devices (token)'] });
    app.save(devices);
    const notifications = new Collection({ name: 'forge_notifications', type: 'base', fields: [
        {name:'user',type:'relation',collectionId:users.id,maxSelect:1,required:true}, {name:'title',type:'text'}, {name:'body',type:'text'},
        {name:'route',type:'text'}, {name:'event_key',type:'text'}, {name:'sent',type:'bool'}, {name:'read',type:'bool'}, {name:'attempts',type:'number'},
        {name:'created',type:'autodate',onCreate:true,onUpdate:false}
    ], indexes: ['CREATE UNIQUE INDEX forge_notification_event ON forge_notifications (event_key)'] });
    notifications.listRule = '@request.auth.id != "" && user = @request.auth.id';
    notifications.viewRule = notifications.listRule;
    notifications.updateRule = notifications.listRule + ' && @request.body.user:changed = false && @request.body.title:changed = false && @request.body.body:changed = false && @request.body.route:changed = false && @request.body.event_key:changed = false && @request.body.sent:changed = false && @request.body.attempts:changed = false && @request.body.created:changed = false';
    app.save(notifications);
}, (app) => {
    // Keep account deletion and notification records private on rollback.
});
