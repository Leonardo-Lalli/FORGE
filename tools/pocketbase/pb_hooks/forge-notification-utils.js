module.exports = {
    enqueue: (user, title, body, route, key) => {
        try {
            const record = new Record($app.findCollectionByNameOrId('forge_notifications'));
            record.set('user', user); record.set('title', title); record.set('body', body); record.set('route', route); record.set('event_key', key);
            // Users with no registered device keep an inbox entry, without blocking delivery.
            record.set('sent', $app.countRecords('forge_push_devices', $dbx.exp('user = {:id}', { id: user })) === 0);
            $app.save(record);
        } catch (err) { console.log('FORGE notification queued already or unavailable.'); }
    }
};
