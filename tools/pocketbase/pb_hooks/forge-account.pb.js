routerAdd('POST', '/api/forge/account/delete', (e) => {
    const password = e.requestInfo().body.password;
    const user = $app.findRecordById('users', e.auth.id);
    if (typeof password !== 'string' || !user.validatePassword(password)) throw new ForbiddenError('Password incorrect.');
    $app.runInTransaction((tx) => {
        for (const name of ['logged_workouts', 'forge_push_devices', 'forge_notifications']) {
            for (const record of tx.findRecordsByFilter(name, 'user = {:id}', '', 0, 0, { id: user.id })) tx.delete(record);
        }
        for (const record of tx.findRecordsByFilter('social_graph', 'from_user = {:id} || to_user = {:id}', '', 0, 0, { id: user.id })) tx.delete(record);
        tx.delete(tx.findRecordById('users', user.id));
    });
    return e.json(200, { deleted: true });
}, $apis.requireAuth('users'));
