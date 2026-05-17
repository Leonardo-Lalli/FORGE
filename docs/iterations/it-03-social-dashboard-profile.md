# IT-03 — Social, Dashboard, Statistiche e Profilo

**Data:** IT-05 + IT-06 + IT-07 (accorpate)
**Branch:** `mockup`
**Stato:** ✅ Completata

## Obiettivo verificabile

Autenticazione PocketBase, feed sociale con like, dashboard con streak, statistiche reali, profilo utente, notifiche like.

## In scope

### Autenticazione (ex IT-05)
- `PocketBaseService` con `LoginAsync()`, `RegisterAsync()`, `Logout()`, `TryAutoLoginAsync()`
- `LoginPage` ridisegnata da mockup `login_cyber_athletic_elite_nero_opaco_v4` / `login_fitness_core_light`
- Token salvato in memoria, credenziali in `Preferences` per auto-login
- `BuildSecrets.LoadAsync()` chiamato all'avvio
- PocketBase HTTPS via Nginx Proxy Manager + Let's Encrypt

### Social (ex IT-06)
- `SearchUsersAsync` con live search e debounce 400ms
- `SendFollowRequestAsync`, `AcceptFollowRequestAsync`, `RejectFollowRequestAsync`
- `GetPendingRequestsAsync`, `GetFollowingUserIdsAsync`
- `FriendRequestsPage` con ACCEPT/REJECT + notifiche like
- `FeedPage` ridisegnata con search, follow, feed post, like/unlike
- `LikeWorkoutAsync` / `UnlikeWorkoutAsync` con `liked_by` array e `likes` count
- Parse robusto con `JsonDocument` per gestire `user` come oggetto o stringa

### Dashboard e Statistiche (ex IT-07)
- `HomePage` con Squad Activity (avatar amici), Current Streak (settimanale), Today Card
- `StatsPage` con filtri temporali, grafico volume a barre, top lifts, calendario
- `TopLifts` da `exercise_data` JSON con `JsonSerializerOptions { PropertyNameCaseInsensitive = true }`
- Streak settimanale: conteggio settimane consecutive, reset dopo 7+ giorni
- Dashboard card piano random tappabile → avvia allenamento con esercizi/pesi/reps

### Profilo
- `ProfilePage` con avatar PocketBase, stats grid (workouts, volume, streak, likes), Recent Forges
- `UpdateUserAsync` per nome e bio
- `UploadAvatarAsync` con multipart form data
- `GetLikeNotificationsAsync` per notifiche like
- Edit overlay con anteprima avatar, nome, bio

## File creati/modificati

| File | Descrizione |
|------|-------------|
| `Services/PocketBaseService.cs` | 20+ metodi: auth, CRUD, social, file, like notifications |
| `Models/Dto/PocketBaseDto.cs` | PocketBaseAuthResponse, UserRecord, LoggedWorkoutRecord, SocialGraphRecord |
| `ViewModels/LoginViewModel.cs` | Login/Register/ToggleMode/Auto-login |
| `ViewModels/HomeViewModel.cs` | Streak, Squad, RandomPlan, OpenProfile/Feed/Settings |
| `ViewModels/FeedViewModel.cs` | FeedPost, UserSearchResult, LikeWorkout, FollowUser, OpenProfile |
| `ViewModels/StatsViewModel.cs` | TopLifts, BarChart, Calendar, LikeNotifications, TimeFilters |
| `ViewModels/ProfileViewModel.cs` | ProfileWorkout, Load/Edit/ChangeAvatar, Streak |
| `ViewModels/FriendRequestsViewModel.cs` | FriendRequest + LikeNotification, Accept/Reject |
| `Views/LoginPage.xaml` | Login/Register UI mockup |
| `Views/HomePage.xaml` | Dashboard UI completa |
| `Views/FeedPage.xaml` | Feed UI con search + post |
| `Views/StatsPage.xaml` | Stats UI con grafico + calendario |
| `Views/ProfilePage.xaml` | Profilo UI con edit overlay |
| `Views/FriendRequestsPage.xaml` | Notifiche UI con friend requests + like |
| `App.xaml.cs` | Auto-login flow, secrets loading |

## Criteri di accettazione

- [x] Registrazione e login con email/password via PocketBase
- [x] Auto-login con credenziali salvate
- [x] Ricerca utenti e invio richieste di amicizia
- [x] Feed allenamenti amici con like ♥ (toggle, conteggio istantaneo)
- [x] Dashboard con Squad Activity, Current Streak (settimanale), START WORKOUT
- [x] Stats con grafico volume, top lifts, calendario, filtri temporali
- [x] Profilo con avatar, bio, recent workouts, likes ricevuti, edit
- [x] Friend Requests con ACCEPT/REJECT + notifiche like
- [x] Avatar utente visibile in Dashboard, Feed, Stats e Profilo
- [x] Tema chiaro/scuro funzionante su tutte le nuove pagine

## Problemi risolti

| Problema | Soluzione |
|----------|-----------|
| `HttpClient.BaseAddress` dopo prima richiesta → crash | Impostato nel costruttore in `MauiProgram.cs` |
| `CollectionView` annidate crash Android | Sostituite con `BindableLayout` |
| Campo `user` restituito come oggetto da PocketBase | Parsing `JsonDocument` con gestione stringa/oggetto |
| Avatar non visibile (URL con token) | `ImageSource.FromUri()` invece di stringa URL diretta |
| Streak resettata a mezzanotte | Ricalcolata come settimanale con tolleranza 7 giorni |
| Doppio `IsBusy` in FriendRequestsViewModel | Rimosso dal derivato, usato `SetLoading()` del base |
| `DynamicResource` per converter in CatalogPage | Corretto in `StaticResource` |
| Border duplicato in HomePage Squad template | Pulito XAML |
