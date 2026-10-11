migrate((app) => {
    const exercises = app.findCollectionByNameOrId('excercise');
    for (const name of ['exercise_id', 'targetMuscles', 'secondaryMuscles'])
        if (!exercises.fields.getByName(name)) exercises.fields.add(new TextField({name}));
    if (!exercises.fields.getByName('instructions')) exercises.fields.add(new JSONField({name:'instructions', maxSize:65536}));
    if (!exercises.fields.getByName('imageUrl')) exercises.fields.add(new URLField({name:'imageUrl'}));
    exercises.indexes.push('CREATE UNIQUE INDEX IF NOT EXISTS forge_exercise_provider_id ON excercise (exercise_id) WHERE exercise_id != ""');
    exercises.listRule = '@request.auth.id != ""'; exercises.viewRule = exercises.listRule;
    exercises.createRule = null; exercises.updateRule = null; exercises.deleteRule = null;
    app.save(exercises);
    const pages = new Collection({name:'forge_catalog_pages',type:'base',fields:[
        {name:'cursor',type:'text'}, {name:'next_cursor',type:'text'},
        {name:'payload',type:'json',maxSize:2097152}
    ],indexes:['CREATE UNIQUE INDEX forge_catalog_cursor ON forge_catalog_pages (cursor)']});
    app.save(pages);
}, (app) => {
    app.delete(app.findCollectionByNameOrId('forge_catalog_pages'));
    // Exercise records are retained; API writes remain restricted to superusers.
});
