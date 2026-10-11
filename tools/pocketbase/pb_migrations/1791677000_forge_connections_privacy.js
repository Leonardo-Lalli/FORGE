migrate((app) => {
    const users = app.findCollectionByNameOrId('users');
    if (!users.fields.getByName('connections_visibility')) users.fields.add(new SelectField({ name: 'connections_visibility', values: ['public', 'private', 'friends'], maxSelect: 1 }));
    app.save(users);
    app.db().newQuery("UPDATE users SET connections_visibility = 'private' WHERE connections_visibility = ''").execute();
    // Other people's lists are served by the privacy-aware route, never by unrestricted graph reads.
    const social = app.findCollectionByNameOrId('social_graph');
    social.listRule = '@request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)';
    social.viewRule = social.listRule;
    app.save(social);
}, (app) => {
    const social = app.findCollectionByNameOrId('social_graph');
    social.listRule = '@request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)';
    social.viewRule = social.listRule;
    app.save(social);
});
