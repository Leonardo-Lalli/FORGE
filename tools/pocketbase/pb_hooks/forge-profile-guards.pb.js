onRecordCreateRequest((e) => {
    if (!e.record.getString('workout_visibility')) e.record.set('workout_visibility', 'private');
    if (!e.record.getString('connections_visibility')) e.record.set('connections_visibility', 'private');
    e.next();
}, 'users');

onRecordUpdateRequest((e) => {
    const allowed = ["first_workout", "holy_week", "habit", "weekend_warrior", "no_excuses", "unstoppable", "member", "comeback", "metronome", "anniversary", "max_weight", "club_100", "hercules", "iron_marathon", "steel_plank", "marble_legs", "bench_king", "powerlifter", "ton_week", "early_bird", "night_owl", "punctual", "hour_grinder", "explorer", "all_rounder", "specialist", "full_body", "encyclopedia", "narcissus", "photographer", "social_butterfly", "networker", "gym_buddy", "lone_wolf", "mentor", "champion", "serial_winner", "challenger", "underdog", "founder", "beta_tester", "legend", "veteran", "milestone"];
    const badges = JSON.parse(e.record.getString('featured_achievements') || '[]') || [];
    if (!Array.isArray(badges) || badges.length > allowed.length || new Set(badges).size !== badges.length || badges.some(id => typeof id !== 'string' || !allowed.includes(id)))
        throw new BadRequestError('Select distinct valid achievement IDs.');
    e.next();
}, 'users');

onRecordUpdateRequest((e) => {
    if (e.auth && e.record.getString('user') !== e.auth.id) {
        const before = e.record.original().getStringSlice('liked_by');
        const after = e.record.getStringSlice('liked_by');
        const otherBefore = before.filter(id => id !== e.auth.id).sort();
        const otherAfter = after.filter(id => id !== e.auth.id).sort();
        if (JSON.stringify(otherBefore) !== JSON.stringify(otherAfter) || new Set(after).size !== after.length)
            throw new ForbiddenError('You may only change your own like.');
        e.record.set('likes', after.length);
    }
    e.next();
}, 'logged_workouts');

