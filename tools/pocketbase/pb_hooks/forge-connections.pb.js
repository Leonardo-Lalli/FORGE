routerAdd('GET', '/api/forge/connections/{userId}', (e) => {
    const id = e.request.pathValue('userId');
    const owner = $app.findRecordById('users', id);
    const viewer = e.auth.id;
    const visibility = owner.getString('connections_visibility');
    const accepted = (from, to) => $app.countRecords('social_graph', $dbx.exp("from_user = {:from} AND to_user = {:to} AND status = 'accepted'", { from, to })) > 0;
    const allowed = id === viewer || visibility === 'public' || (visibility === 'friends' && accepted(id, viewer) && accepted(viewer, id));
    if (!allowed) return e.json(200, { allowed: false, items: [] });
    const records = $app.findRecordsByFilter('social_graph', 'status = "accepted" && (from_user = {:id} || to_user = {:id})', '', 0, 0, { id });
    // Return only fields needed to display this list; pending requests stay private.
    return e.json(200, { allowed: true, items: records.map(r => ({ id: r.id, from_user: r.getString('from_user'), to_user: r.getString('to_user'), status: 'accepted' })) });
}, $apis.requireAuth('users'));
