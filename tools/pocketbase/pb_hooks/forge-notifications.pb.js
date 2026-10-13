routerAdd('POST', '/api/forge/push/device', (e) => {
    const token = e.requestInfo().body.token;
    if (typeof token !== 'string' || token.length < 20 || token.length > 4096) throw new BadRequestError('Invalid device token.');
    const records = $app.findRecordsByFilter('forge_push_devices', 'token = {:token}', '', 1, 0, { token });
    const record = records[0] || new Record($app.findCollectionByNameOrId('forge_push_devices'));
    // A shared phone has one active account per token. Never expose other devices.
    record.set('user', e.auth.id); record.set('token', token); $app.save(record);
    return e.json(200, { registered: true });
}, $apis.requireAuth('users'));
routerAdd('DELETE', '/api/forge/push/device', (e) => {
    for (const record of $app.findRecordsByFilter('forge_push_devices', 'user = {:id} && token = {:token}', '', 0, 0, { id: e.auth.id, token: e.requestInfo().body.token || '' })) $app.delete(record);
    return e.json(200, { removed: true });
}, $apis.requireAuth('users'));
onRecordAfterCreateSuccess((e) => {
    e.next();
    if (e.record.getString('status') === 'pending') require(__hooks + '/forge-notification-utils.js').enqueue(e.record.getString('to_user'), 'Nuova richiesta di follow', 'Una persona vuole seguirti su FORGE.', 'friendRequests', 'follow:' + e.record.id);
}, 'social_graph');
onRecordUpdateRequest((e) => {
    const wasPending = e.record.original().getString('status') === 'pending';
    e.next();
    if (wasPending && e.record.getString('status') === 'accepted') require(__hooks + '/forge-notification-utils.js').enqueue(e.record.getString('from_user'), 'Follow accettato', 'La tua richiesta è stata accettata.', 'profile?userId=' + e.record.getString('to_user'), 'accepted:' + e.record.id);
}, 'social_graph');
onRecordUpdateRequest((e) => {
    const oldLikes = e.record.original().getStringSlice('liked_by');
    e.next();
    for (const id of e.record.getStringSlice('liked_by').filter(id => !oldLikes.includes(id) && id !== e.record.getString('user')))
        require(__hooks + '/forge-notification-utils.js').enqueue(e.record.getString('user'), 'Nuovo like', 'Una persona ha apprezzato il tuo allenamento.', 'workoutDetail?workoutId=' + e.record.id + '&source=pocketbase', 'like:' + e.record.id + ':' + id);
}, 'logged_workouts');
onRecordAfterCreateSuccess((e) => {
    e.next();
    const ownerId = e.record.getString('user');
    const owner = $app.findRecordById('users', ownerId);
    const visibility = owner.getString('workout_visibility');
    if (visibility !== 'public' && visibility !== 'friends') return;
    const followers = $app.findRecordsByFilter('social_graph', 'to_user = {:id} && status = "accepted"', '', 0, 0, { id: ownerId });
    for (const follow of followers) {
        const recipient = follow.getString('from_user');
        if (visibility === 'friends' && $app.countRecords('social_graph', $dbx.exp("from_user = {:owner} AND to_user = {:recipient} AND status = 'accepted'", { owner: ownerId, recipient })) === 0) continue;
        require(__hooks + '/forge-notification-utils.js').enqueue(recipient, 'Nuovo allenamento condiviso', 'Una persona che segui ha completato un allenamento.', 'workoutDetail?workoutId=' + e.record.id + '&source=pocketbase', 'workout:' + e.record.id + ':' + recipient);
    }
}, 'logged_workouts');
routerAdd('POST', '/api/forge/notifications/achievement', (e) => {
    const allowed = ["first_workout", "holy_week", "habit", "weekend_warrior", "no_excuses", "unstoppable", "member", "comeback", "metronome", "anniversary", "max_weight", "club_100", "hercules", "iron_marathon", "steel_plank", "marble_legs", "bench_king", "powerlifter", "ton_week", "early_bird", "night_owl", "punctual", "hour_grinder", "explorer", "all_rounder", "specialist", "full_body", "encyclopedia", "narcissus", "photographer", "social_butterfly", "networker", "gym_buddy", "lone_wolf", "mentor", "champion", "serial_winner", "challenger", "underdog", "founder", "beta_tester", "legend", "veteran", "milestone"];
    const body = e.requestInfo().body;
    const ids = body.ids || [body.id];
    if (!Array.isArray(ids) || ids.length > allowed.length || ids.some(id => !allowed.includes(id))) throw new BadRequestError('Invalid achievement.');
    // Personal announcement only: this endpoint never awards a badge or XP.
    for (const id of new Set(ids)) require(__hooks + '/forge-notification-utils.js').enqueue(e.auth.id, 'Achievement sbloccato', 'Hai raggiunto un nuovo traguardo su FORGE.', 'achievements', 'achievement:' + e.auth.id + ':' + id);
    return e.json(200, { queued: true });
}, $apis.requireAuth('users'));
routerAdd('GET', '/api/forge/push/pending', (e) => {
    const secret = $os.getenv('FORGE_PUSH_WORKER_TOKEN');
    if (!secret || e.request.header.get('X-Forge-Worker') !== secret) throw new ForbiddenError('Worker only.');
    const records = $app.findRecordsByFilter('forge_notifications', 'sent = false && attempts < 10', 'created', 100, 0);
    const items = [];
    for (const r of records) {
        const recipient = r.getString('user');
        const tokens = $app.findRecordsByFilter('forge_push_devices', 'user = {:id}', '', 0, 0, { id:recipient }).map(d => d.getString('token'));
        let allowed = tokens.length > 0;
        const match = /^workoutDetail\?workoutId=([a-zA-Z0-9]+)&source=pocketbase$/.exec(r.getString('route'));
        if (allowed && match) {
            try {
                const workout = $app.findRecordById('logged_workouts', match[1]);
                const ownerId = workout.getString('user');
                const visibility = $app.findRecordById('users', ownerId).getString('workout_visibility');
                allowed = ownerId === recipient || visibility === 'public';
                if (!allowed && visibility === 'friends') {
                    const directions = $app.findRecordsByFilter('social_graph', 'status = "accepted" && ((from_user = {:a} && to_user = {:b}) || (from_user = {:b} && to_user = {:a}))', '', 0, 0, { a:ownerId, b:recipient });
                    allowed = directions.some(d => d.getString('from_user') === ownerId) && directions.some(d => d.getString('from_user') === recipient);
                }
            } catch (err) { allowed = false; }
        }
        if (!allowed) { r.set('sent', true); $app.save(r); continue; }
        items.push({ id:r.id, title:r.getString('title'), body:r.getString('body'), route:r.getString('route'), tokens });
    }
    return e.json(200, { items });
});
routerAdd('POST', '/api/forge/push/result', (e) => {
    const secret = $os.getenv('FORGE_PUSH_WORKER_TOKEN');
    if (!secret || e.request.header.get('X-Forge-Worker') !== secret) throw new ForbiddenError('Worker only.');
    const body = e.requestInfo().body;
    const record = $app.findRecordById('forge_notifications', body.id);
    record.set('attempts', record.getInt('attempts') + 1);
    if (body.sent === true) record.set('sent', true);
    $app.save(record);
    return e.json(200, { updated: true });
});
