// Shared metadata cache. No client-supplied content or arbitrary upstream URLs.
routerAdd('GET', '/api/forge/catalog/page', (e) => {
    const cursor = e.request.url.query().get('after') || '';
    if (cursor.length > 256) throw new BadRequestError('Invalid cursor.');
    let page = null;
    try { page = e.app.findFirstRecordByFilter('forge_catalog_pages', 'cursor = {:cursor}', {cursor}); } catch (_) {}
    if (page) return e.json(200, JSON.parse(page.getString('payload')));
    if (cursor) {
        try { e.app.findFirstRecordByFilter('forge_catalog_pages', 'next_cursor = {:cursor}', {cursor}); }
        catch (_) { throw new BadRequestError('Cursor not issued by this catalog.'); }
    }
    const response = $http.send({url:'https://oss.exercisedb.dev/api/v1/exercises?limit=25' + (cursor ? '&after=' + encodeURIComponent(cursor) : ''), timeout:15});
    if (response.statusCode !== 200) throw new ApiError(502, 'Exercise provider temporarily unavailable.');
    const payload = response.json;
    if (!payload || !Array.isArray(payload.data) || payload.data.length > 25 || !payload.meta || !(payload.meta.total > 0))
        throw new ApiError(502, 'Invalid exercise catalog response.');
    e.app.runInTransaction((app) => {
        // Another client may have populated this page while the provider call ran.
        try { app.findFirstRecordByFilter('forge_catalog_pages','cursor = {:cursor}',{cursor}); return; } catch (_) {}
        const collection = app.findCollectionByNameOrId('excercise');
        for (const exercise of payload.data) {
            if (!exercise.exerciseId || !exercise.name || typeof exercise.gifUrl !== 'string' || !exercise.gifUrl.startsWith('https://') || !Array.isArray(exercise.instructions))
                throw new ApiError(502, 'Incomplete provider exercise.');
            let record;
            try { record = app.findFirstRecordByFilter('excercise','exercise_id = {:id}',{id:exercise.exerciseId}); }
            catch (_) { record = new Record(collection); }
            const list = value => Array.isArray(value) ? value.join(', ') : String(value || '');
            record.set('exercise_id',exercise.exerciseId); record.set('name',exercise.name);
            record.set('bodyPart',list(exercise.bodyParts)); record.set('equipment',list(exercise.equipments));
            record.set('instructions',exercise.instructions); record.set('imageUrl',exercise.gifUrl);
            record.set('category',list(exercise.bodyParts)); record.set('targetMuscles',list(exercise.targetMuscles));
            record.set('secondaryMuscles',list(exercise.secondaryMuscles));
            // Also populate legacy installations, preserving their field types.
            const legacyImage = collection.fields.getByName('Image_url');
            if (legacyImage && legacyImage.type !== 'file') record.set('Image_url',exercise.gifUrl);
            const legacy = collection.fields.getByName('instruction');
            if (legacy) record.set('instruction',legacy.type === 'json' ? exercise.instructions : exercise.instructions.join('\n'));
            app.save(record);
        }
        const stored = new Record(app.findCollectionByNameOrId('forge_catalog_pages'));
        stored.set('cursor',cursor); stored.set('next_cursor',payload.meta.nextCursor || ''); stored.set('payload',payload);
        app.save(stored);
    });
    return e.json(200,payload);
}, $apis.requireAuth('users'));

routerAdd('GET','/api/forge/catalog/status',(e) => {
    let total = 0;
    try { total = JSON.parse(e.app.findFirstRecordByFilter('forge_catalog_pages','cursor = ""').getString('payload')).meta.total; } catch (_) {}
    const cached = e.app.countRecords('excercise',$dbx.exp("exercise_id != ''"));
    return e.json(200,{cached,total,complete:total > 0 && cached >= total});
},$apis.requireAuth('users'));
