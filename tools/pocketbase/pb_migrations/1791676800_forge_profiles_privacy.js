// PocketBase 0.23+; deploy together with forge-profile-guards.pb.js.
migrate((app) => {
    const users = app.findCollectionByNameOrId('users');
    if (!users.fields.getByName('workout_visibility')) users.fields.add(new SelectField({ name: 'workout_visibility', values: ['public', 'private', 'friends'], maxSelect: 1 }));
    if (!users.fields.getByName('featured_achievements')) users.fields.add(new JSONField({ name: 'featured_achievements', maxSize: 2048 }));
    users.listRule = '@request.auth.id != ""';
    users.viewRule = '@request.auth.id != ""';
    users.updateRule = '@request.auth.id != "" && id = @request.auth.id';
    app.save(users);
    // Privacy is opt-in: existing and new accounts start private.
    app.db().newQuery("UPDATE users SET workout_visibility = 'private' WHERE workout_visibility = ''").execute();

    const social = app.findCollectionByNameOrId('social_graph');
    social.listRule = '@request.auth.id != "" && (status = "accepted" || from_user = @request.auth.id || to_user = @request.auth.id)';
    social.viewRule = social.listRule;
    social.createRule = '@request.auth.id != "" && from_user = @request.auth.id && to_user != @request.auth.id && status = "pending"';
    const identityGuards = JSON.parse(JSON.stringify(social)).fields.filter(f => f.name !== 'status').map(f => '@request.body.' + f.name + ':changed = false').join(' && ');
    social.updateRule = '@request.auth.id != "" && to_user = @request.auth.id && status = "pending" && (@request.body.status = "accepted" || @request.body.status = "rejected") && ' + identityGuards;
    social.deleteRule = '@request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)';
    app.save(social);

    const workouts = app.findCollectionByNameOrId('logged_workouts');
    const mutual = '(@collection.social_graph:out.from_user ?= user && @collection.social_graph:out.to_user ?= @request.auth.id && @collection.social_graph:out.status ?= "accepted" && @collection.social_graph:back.from_user ?= @request.auth.id && @collection.social_graph:back.to_user ?= user && @collection.social_graph:back.status ?= "accepted")';
    const visible = '(user = @request.auth.id || user.workout_visibility = "public" || (user.workout_visibility = "friends" && ' + mutual + '))';
    workouts.listRule = '@request.auth.id != "" && ' + visible;
    workouts.viewRule = workouts.listRule;
    workouts.createRule = '@request.auth.id != "" && user = @request.auth.id';
    const likesGuards = JSON.parse(JSON.stringify(workouts)).fields.filter(f => !['likes', 'liked_by'].includes(f.name)).map(f => '@request.body.' + f.name + ':changed = false').join(' && ');
    workouts.updateRule = '@request.auth.id != "" && @request.body.user:changed = false && (user = @request.auth.id || (' + visible + ' && ' + likesGuards + '))';
    workouts.deleteRule = '@request.auth.id != "" && user = @request.auth.id';
    for (const field of JSON.parse(JSON.stringify(workouts)).fields.filter(f => f.type === "file")) workouts.fields.getByName(field.name).protected = true;
    app.save(workouts);
}, (app) => {
    // Rollback deliberately fails closed; restore a backup to recover old rules.
    const workouts = app.findCollectionByNameOrId('logged_workouts');
    workouts.listRule = '@request.auth.id != "" && user = @request.auth.id';
    workouts.viewRule = workouts.listRule;
    workouts.updateRule = workouts.listRule + ' && @request.body.user:changed = false';
    app.save(workouts);
});
