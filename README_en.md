# FORGE — Android workout tracker

FORGE began as Leonardo Lalli’s computer science school project and grew with extensive AI-assisted development, device feedback and documented checks. Version **0.8.5 beta** is based on **main**. Changes after a release tag are not automatically included in its APK.

Log workouts, create personal plans without an app-imposed limit, use per-exercise rest timers and follow users on the same PocketBase server. The default instance is operated at the owner’s home; personal servers are supported. Cross-server social, full account-free local mode and in-app updates are not implemented. Real background FCM delivery still needs verification.

[Download and website](https://leonardo-lalli.github.io/FORGE/#download) · [Canonical README and current setup instructions](README.md) · [Roadmap](ROADMAP.md) · [License](LICENSE)

During workouts, available exercise instructions rotate automatically as a carousel of execution steps.

The app has no subscription, but hosting has costs. Code is MIT; exercise/media and other third-party assets retain their own licenses. AI assistance does not certify correctness or security.

## Gamification and screenshots

Achievements supported by workout history unlock retroactively. Each unlocked badge contributes 100 XP; every 500 XP adds a level. Unlocked badges can be displayed on the profile. Some catalog criteria, including leaderboards and challenges, are not implemented; per-set record medals remain future ideas.

[Real development screenshots and detailed limitations](README.md#schermate-dellapp) are available in the canonical README. The workout image is a recent owner-supplied capture; the other screenshots predate parts of the current UI. Earlier Debug APKs use different certificates; back up local data before reinstalling the release build.

The current CSV export is a summary, not a full backup of plans, photos, sets or weights. Before reinstalling, verify workout synchronization and save plans/notes/photos separately.
