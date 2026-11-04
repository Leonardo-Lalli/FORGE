migrate((app) => {
    const users = app.findCollectionByNameOrId('users');
    const plans = new Collection({name:'forge_plans',type:'base',fields:[
        {name:'user',type:'relation',collectionId:users.id,maxSelect:1,required:true,cascadeDelete:true},
        {name:'local_id',type:'text',required:true,max:200},
        {name:'payload',type:'json',maxSize:1048576}
    ],indexes:['CREATE UNIQUE INDEX forge_plan_owner_id ON forge_plans (user, local_id)']});
    // No collection API access: the authenticated sync endpoint owns validation and conflict handling.
    app.save(plans);
}, (app) => {
    app.delete(app.findCollectionByNameOrId('forge_plans'));
});
