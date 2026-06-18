# Changelog

## v0.8.0-beta (2026-06-18)

### Added
- 48 achievement con badge Canva (6 categorie: Costanza, Forza, Orari, Varietà, Social, Elite)
- Tracking automatico achievement (volume, streak, orari, varietà esercizi)
- WorkoutDetailPage con tutte le serie (kg × reps), note, foto, like count
- Foto progresso in allenamento (camera + galleria, max 5 foto, limite 5MB)
- Minimize workout: ✕ salva come draft (non conta per stats), FINISH cancella draft e salva
- Token refresh automatico PocketBase
- Compressione foto: limite 5MB con notifica
- Language selection con feedback visivo (DataTrigger)

### Changed
- PocketBase URL: `leoforge.duckdns.org` (HTTPS, Let's Encrypt)
- Email spostata da Preferences a SecureStorage
- `catch(Exception)` non espongono più `ex.Message` all'utente
- `ParseErrorMessage`: fallback non restituisce più il body raw del server

### Security
- API Rules PocketBase con row-level ownership
- Admin panel `/_/` bloccato da Nginx (403)
- Rate limiting login (5 tentativi/minuto)
- `android:allowBackup=false`
- Token JWT: public → internal, rimosso da URL query parameter
- Dead code rimosso (ExerciseDbDto, 2 converter inutilizzati)
- File sensibili spostati in `docs/private/`

### Fixed
- WorkoutDetailPage: BindingContext mai impostato
- StatsPage: InputTransparent su avatar per navigazione profilo
- HomePage: StreakDescription binding dinamico
- ActiveWorkoutPage: IsVisible SearchError (string → bool)
- 12 catch vuoti con Debug.WriteLine
- ExerciseDbApiService: cursor Uri.EscapeDataString
- LocalWorkout: ExerciseDataJson default corretto `"[]"`

## v0.7.0-alpha (2026-06-17)

### Added
- ExerciseDB v1 free API (1.500+ esercizi con GIF)
- SQLite offline (4 tabelle: local_workouts, cached_exercises, saved_plans, achievements)
- SyncService per auto-push allenamenti pendenti
- CSV import/export in Settings
- PlanService con auto-migrazione da Preferences

### Changed
- Wger API completamente rimossa
- 5 ViewModel stub + 5 Page stub rimossi
- URL fallback PocketBase: rimosso hardcoded → eccezione se non configurato

## v0.6.0 (2026-05-17)

### Added
- PocketBase auth (email/password, JWT, auto-login)
- HTTPS via Nginx Proxy Manager + Let's Encrypt
- Feed sociale: ricerca utenti, follow/unfollow, like ♥
- StatsPage: grafico volume, top lifts, calendario, filtri temporali
- ProfilePage: avatar, bio, recent forges, edit overlay
- Friend requests con notifiche like

### Fixed
- Crash `JavaProxyThrowable` (HttpClient BaseAddress race condition)
- Campi PocketBase polymorfi (JsonDocument parsing)
- CollectionView annidate → BindableLayout

## v0.5.0 (2026-05-13)

### Added
- ExerciseDB API (RapidAPI) con cache PocketBase
- ActiveWorkoutPage: serie kg×reps, checkmark ✓, rest timer
- StartSessionPage: Quick Start, Create Plan, Your Protocols
- 3 piani demo: Push Power, Leg Day Protocol, Core Stabilization
- Google Fonts: Inter, Lexend, Space Grotesk

## v0.1.0 (2026-05-07)

### Added
- Bootstrap .NET MAUI con Shell 3-tab
- ThemeService: doppio tema Cyber-Athletic Elite / Fitness Core
- BuildSecrets: `.env` → `forge.env` via MSBuild
