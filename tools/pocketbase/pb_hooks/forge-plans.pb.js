routerAdd('POST', '/api/forge/plans/sync', (e) => {
    const owner = e.auth.id;
    const changes = e.requestInfo().body.plans;
    if (!Array.isArray(changes) || changes.length > 1000 || JSON.stringify(changes).length > 5000000)
        throw new BadRequestError('Invalid plan batch.');
    const seen = new Set();
    // Preserve sub-millisecond precision from .NET timestamps when edits arrive rapidly.
    const stamp = value => new Date(Date.parse(value)).toISOString().slice(0,19) + '.' + ((value.match(/\.(\d+)/) || [,''])[1].padEnd(7,'0').slice(0,7));
    for (const plan of changes) {
        if (!plan || typeof plan.id !== 'string' || !plan.id.length || plan.id.length > 200 || seen.has(plan.id) ||
            plan.userId !== owner || typeof plan.name !== 'string' || plan.name.length > 300 ||
            typeof plan.notes !== 'string' || plan.notes.length > 20000 ||
            !Number.isInteger(plan.restSeconds) || plan.restSeconds < 0 || plan.restSeconds > 300 ||
            typeof plan.isDeleted !== 'boolean' || typeof plan.updatedAt !== 'string' || typeof plan.createdAt !== 'string' || !Number.isFinite(Date.parse(plan.updatedAt)) ||
            !Number.isFinite(Date.parse(plan.createdAt)) || Date.parse(plan.updatedAt) > Date.now() + 300000 ||
            typeof plan.exercisesJson !== 'string' || plan.exercisesJson.length > 800000)
            throw new BadRequestError('Invalid plan.');
        let exercises;
        try { exercises = JSON.parse(plan.exercisesJson); } catch (_) { throw new BadRequestError('Invalid exercises.'); }
        if (!Array.isArray(exercises) || exercises.length > 200) throw new BadRequestError('Invalid exercises.');
        seen.add(plan.id);
    }
    $app.runInTransaction((tx) => {
        for (const plan of changes) {
            const existing = tx.findRecordsByFilter('forge_plans', 'user = {:owner} && local_id = {:id}', '', 1, 0, {owner:owner,id:plan.id})[0];
            if (existing) {
                const saved = JSON.parse(existing.getString('payload'));
                if (stamp(saved.updatedAt) >= stamp(plan.updatedAt)) continue;
            }
            const record = existing || new Record(tx.findCollectionByNameOrId('forge_plans'));
            record.set('user',owner);
            record.set('local_id',plan.id);
            record.set('payload',plan);
            tx.save(record);
        }
    });
    const plans = $app.findRecordsByFilter('forge_plans', 'user = {:owner}', '', 0, 0, {owner:owner})
        .map(record => JSON.parse(record.getString('payload')));
    return e.json(200,{plans:plans});
}, $apis.requireAuth('users'));
