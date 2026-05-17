# Piano di Progetto - FORGE

## 1. Sintesi operativa

### Obiettivo del progetto

App .NET MAUI Android-first per tracciare allenamenti in palestra, con catalogo esercizi da ExerciseDB API, persistenza remota PocketBase, statistiche di progresso, e social tra amici (feed, like, follow).

### Vincoli principali

- MVVM con CommunityToolkit.Mvvm, Shell navigation (3 tab), nessuna logica in code-behind.
- Fonti dati remote: ExerciseDB API (RapidAPI) per esercizi, PocketBase per auth e social.
- Persistenza: PocketBase per dati remoti, Preferences per credenziali e PlanStore.
- UI progettata con Stitch — doppio tema "Cyber-Athletic Elite" (scuro, ciano `#00E5FF`) / "Fitness Core" (chiaro, blu `#003ec7`), font Inter/Lexend/Space Grotesk.
- Gestione esplicita stati: loading, error, empty, success.

### Dipendenze esterne

- ExerciseDB API (RapidAPI key via `.env`)
- PocketBase self-hosted (`https://pocketbase.server-casa-leo.duckdns.org`)
- Google Fonts (Inter, Lexend, Space Grotesk — inclusi nell'app)

## 2. Sequenza delle iterazioni

| Iterazione | Obiettivo verificabile | Dipendenze | Rischio | Stato |
| --- | --- | --- | --- | --- |
| IT-01 | Bootstrap MAUI + Shell 3-tab + Design System + ThemeService | Nessuna | Basso | ✅ Completata |
| IT-02/03/08 | Catalogo esercizi + Allenamento + Piani salvabili | IT-01 | Medio | ✅ Completata |
| IT-05/06/07 | PocketBase Auth + Social + Dashboard + Stats + Profilo | IT-02/03/08 | Alto | ✅ Completata |
| IT-04 | Tracking peso corporeo e misure | IT-01 | Basso | ❌ Non implementata |

## 3. Dettaglio iterazioni

### IT-01 — Bootstrap MAUI e Design System ✅

**Obiettivo verificabile**

Progetto MAUI compilabile su Android con Shell navigation a 3 tab e doppio tema Stitch.

**In scope**

- Creazione progetto `src/GymTracker.Mobile/`
- AppShell con 3 tab: Dashboard, Feed, Stats
- Pagine placeholder per ogni tab
- `ThemeService` con palette dark/light inline e `WriteResources()`
- Toggle tema runtime via DynamicResource
- Font Google Fonts: Inter (body), Lexend (label), Space Grotesk (headline)
- Dependency injection in `MauiProgram.cs`
- `BaseViewModel` con `SetLoading()`, `SetSuccess()`, `SetError()`
- `BuildSecrets` per caricamento chiavi API da `.env`
- Converters: `InverseBoolConverter`, `BoolToVisibilityConverter`
- Cartelle: `Models/`, `ViewModels/`, `Views/`, `Services/`, `Converters/`

**Criteri di accettazione**

- [x] Il progetto compila con `dotnet build` senza errori
- [x] L'app si avvia su emulatore Android e mostra 3 tab
- [x] Navigazione tra tab funzionante
- [x] Tema dark/light applicato con toggle nelle Impostazioni
- [x] DynamicResource su tutte le pagine per cambio tema runtime
- [x] Font Inter, Lexend, Space Grotesk caricati e usati

**File principali**

- `MauiProgram.cs`, `App.xaml`/`App.xaml.cs`, `AppShell.xaml`/`AppShell.xaml.cs`
- `ViewModels/BaseViewModel.cs`, `Services/ThemeService.cs`, `Services/BuildSecrets.cs`
- `Resources/Styles/Styles.xaml`, `Converters/`

---

### IT-02/03/08 — Catalogo Esercizi, Allenamento e Piani ✅

**Nota:** Le iterazioni IT-02, IT-03 e IT-08 sono state accorpate in un'unica iterazione.

**Obiettivo verificabile**

Catalogo esercizi da ExerciseDB API con cache PocketBase, registrazione allenamento completa con rest timer, piani di allenamento salvabili.

**In scope**

- `ExerciseApiService` con HttpClient e header RapidAPI
- Ricerca per nome, filtro per gruppo muscolare e attrezzatura (chip orizzontali)
- DTO `ExerciseDbDto` con primaryMuscles, secondaryMuscles, equipment, instructions, images
- Cache esercizi su PocketBase collection `excercise` con URL immagini risolti
- `ResolveImageUrlAsync()` per seguire redirect HTTP (short URL bloccati da ISP)
- `ActiveWorkoutPage` ridisegnata da mockup
- `WorkoutPlan` con `WorkoutExercise` e `ExerciseSet` (ObservableObject)
- Checkmark ✓ per completamento serie (LimeGreen)
- Rest timer configurabile (5-600s), timer per esercizio
- Salvataggio su PocketBase `logged_workouts` con `exercise_data` JSON
- `StartSessionPage` con Quick Start, Create New Plan, Your Protocols
- `PlanStore`: salvataggio piani su `Preferences` come JSON
- 3 piani demo: Push Power, Leg Day Protocol, Core Stabilization

**Criteri di accettazione**

- [x] Ricerca esercizi da ExerciseDB API per nome, muscolo e attrezzatura
- [x] Immagini esercizi caricate e cache su PocketBase
- [x] Avvio allenamento libero e da piano salvato
- [x] Aggiunta serie con peso e ripetizioni, completamento con ✓
- [x] Rimozione esercizi e serie
- [x] Salvataggio allenamento su PocketBase con data, volume, durata
- [x] Piani salvati persistenti tra riavvii
- [x] UI coerente con mockup per entrambi i temi

**File principali**

- `Services/ExerciseApiService.cs`, `Models/Dto/ExerciseDbDto.cs`, `Models/WorkoutPlan.cs`
- `ViewModels/ActiveWorkoutViewModel.cs`, `ViewModels/StartSessionViewModel.cs`
- `Views/ActiveWorkoutPage.xaml`, `Views/StartSessionPage.xaml`
- `Messages/WorkoutSavedMessage.cs`
- `docs/iterations/it-02-catalogo-allenamento.md`

---

### IT-05/06/07 — Auth PocketBase, Social, Dashboard, Stats e Profilo ✅

**Nota:** Le iterazioni IT-05, IT-06 e IT-07 sono state accorpate in un'unica iterazione.

**Obiettivo verificabile**

Autenticazione PocketBase, feed sociale con like, dashboard con streak, statistiche reali, profilo utente, friend requests con notifiche like.

**In scope**

- `PocketBaseService` con 20+ metodi: auth, CRUD, social, file, like notifications
- `LoginPage` ridisegnata da mockup (login/register con toggle)
- Token in memoria, credenziali in `Preferences` per auto-login
- PocketBase HTTPS via Nginx Proxy Manager + Let's Encrypt
- Ricerca utenti live con debounce 400ms
- Follow/unfollow, accept/reject friend requests
- `FeedPage` con search, follow, feed post, like/unlike (♥ toggle)
- `LikeWorkoutAsync` / `UnlikeWorkoutAsync` con `liked_by` array
- Parse robusto con `JsonDocument` per campo `user` (oggetto o stringa)
- `HomePage` con Squad Activity, Current Streak, Today Card
- `StatsPage` con grafico volume a barre, top lifts, calendario, filtri temporali
- Streak settimanale con tolleranza 7 giorni
- `ProfilePage` con avatar PocketBase, stats grid, Recent Forges, edit overlay
- `UploadAvatarAsync` con multipart form data
- `FriendRequestsPage` con richieste follow + notifiche like aggregate

**Criteri di accettazione**

- [x] Registrazione e login con email/password/username via PocketBase
- [x] Auto-login con credenziali salvate
- [x] Ricerca utenti e invio richieste di follow
- [x] Feed allenamenti amici con like ♥ (toggle, conteggio istantaneo)
- [x] Dashboard con Squad Activity, Current Streak, START WORKOUT
- [x] Stats con grafico volume, top lifts, calendario, filtri temporali
- [x] Profilo con avatar, bio, recent workouts, likes ricevuti, edit
- [x] Friend Requests con ACCEPT/REJECT + notifiche like
- [x] Avatar utente visibile in Dashboard, Feed, Stats e Profilo
- [x] Tema chiaro/scuro funzionante su tutte le nuove pagine

**File principali**

- `Services/PocketBaseService.cs`, `Models/Dto/PocketBaseDto.cs`
- `ViewModels/LoginViewModel.cs`, `HomeViewModel.cs`, `FeedViewModel.cs`, `StatsViewModel.cs`, `ProfileViewModel.cs`, `FriendRequestsViewModel.cs`
- `Views/LoginPage.xaml`, `HomePage.xaml`, `FeedPage.xaml`, `StatsPage.xaml`, `ProfilePage.xaml`, `FriendRequestsPage.xaml`
- `App.xaml.cs` (auto-login flow)
- `docs/iterations/it-03-social-dashboard-profile.md`

---

### IT-04 — Tracking peso corporeo e misure ❌

**Stato:** NON IMPLEMENTATA. Rinviata a post-MVP.

**Obiettivo previsto**

L'utente può registrare peso e misure corporee e visualizzarne l'andamento nel tempo.

**In scope (pianificato)**

- Modello `BodyWeight`: `Id`, `Date`, `WeightKg`
- Modello `BodyMeasurement`: `Id`, `Date`, `ChestCm`, `WaistCm`, `HipsCm`, `ArmsCm`, `LegsCm`
- Repository SQLite per peso e misure
- Pagina `BodyTrackingPage` con form e storico
- Grafico a linee per andamento peso
- Dati SOLO locali, mai sincronizzati con backend

---

## 4. Riepilogo stato implementazione

| Area | Stato | Pagine/Feature |
|------|-------|----------------|
| Bootstrap + Shell 3-tab | ✅ | AppShell, ThemeService, BaseViewModel |
| Autenticazione | ✅ | LoginPage (login/register), auto-login |
| Catalogo esercizi | ✅ | Ricerca API, filtri, immagini, cache PocketBase |
| Allenamento | ✅ | ActiveWorkoutPage, StartSessionPage, rest timer |
| Piani | ✅ | PlanStore, 3 piani demo, avvio da piano |
| Dashboard | ✅ | HomePage: streak, squad, today card |
| Statistiche | ✅ | StatsPage: grafico, top lifts, calendario, filtri |
| Feed sociale | ✅ | FeedPage: search, follow, like/unlike |
| Profilo | ✅ | ProfilePage: avatar, bio, edit, recent workouts |
| Friend requests | ✅ | FriendRequestsPage: follow + like notifications |
| Tema doppio | ✅ | Toggle runtime chiaro/scuro su tutte le pagine |
| Body tracking | ❌ | IT-04 non implementata |
| Leaderboard | ❌ | Sostituita da feed + like |
| Confronto diretto | ❌ | Post-MVP |
| SQLite locale | ❌ | Usato PocketBase remoto, Preferences per PlanStore |

## 5. Prossimi passi

1. Valutare se implementare IT-04 (body tracking) in una nuova iterazione
2. Aggiungere leaderboard settimanale
3. Implementare confronto diretto statistiche tra amici
4. Aggiungere persistenza offline con SQLite
5. Aggiungere test automatici (unit test su ViewModel, integration test su PocketBase)
6. Esplorare notifiche push per like e friend requests
