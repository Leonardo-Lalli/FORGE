# Architettura — FORGE

## 1. Obiettivo architetturale

Costruire una app `.NET MAUI` Android-first per il tracking degli allenamenti che integri tre fonti dati con responsabilità chiare:

- **ExerciseDB API** (RapidAPI): catalogo esercizi remoto con 1300+ esercizi;
- **PocketBase** (self-hosted): autenticazione, database, file storage e dati social;
- **Preferences / SQLite**: credenziali, piani di allenamento, cache esercizi, achievement.

L'architettura supporta l'uso online per la registrazione allenamenti e la sincronizzazione immediata con PocketBase. Il design dell'interfaccia segue i mockup Stitch con tema scuro "Cyber-Athletic Elite" (ciano #00E5FF) e chiaro "Fitness Core" (blu #003ec7), font Inter / Lexend / Space Grotesk.

## 2. Struttura del repository

```
src/Forge/
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs
├── MauiProgram.cs                  # DI composition root
├── Messages/                       # WeakReferenceMessenger
│   └── WorkoutSavedMessage.cs
├── Models/                         # Entità dominio e DTO
│   ├── WorkoutPlan.cs              # WorkoutPlan, WorkoutExercise, ExerciseSet
│   └── Dto/
│       ├── PocketBaseDto.cs        # PocketBaseAuthResponse, UserRecord, LoggedWorkoutRecord, SocialGraphRecord
│       └── ExerciseDbDto.cs        # DTO per ExerciseDB API
├── Services/
│   ├── PocketBaseService.cs        # Auth, CRUD, social, file, like notifications
│   ├── ExerciseDbApiService.cs     # HTTP ExerciseDB, cache SQLite/PocketBase
│   ├── ThemeService.cs             # Palette dark/light inline, Apply(), WriteResources()
│   ├── BuildSecrets.cs             # .env → Dictionary, ConcurrentDictionary
│   ├── WorkoutSession.cs           # Sessione allenamento attivo
│   ├── PlanService.cs              # CRUD piani su SQLite (migrato da Preferences)
│   ├── DatabaseService.cs          # SQLite 4 tabelle
│   ├── SyncService.cs              # Sync offline→online
│   ├── ConnectivityService.cs      # Monitoraggio rete
│   ├── AchievementService.cs       # Tracking 48 badge
│   ├── CsvImportService.cs         # Import CSV validato
│   └── CsvExportService.cs         # Export CSV con escape
├── ViewModels/
│   ├── BaseViewModel.cs            # IsBusy, ErrorMessage, HasData, IsEmptyState
│   ├── HomeViewModel.cs            # Dashboard: streak, squad, open profile/feed/settings
│   ├── FeedViewModel.cs            # Feed: search users, follow, feed posts, like
│   ├── StatsViewModel.cs           # Statistiche: top lifts, bar chart, calendar, like notifications
│   ├── ProfileViewModel.cs         # Profilo: avatar, stats, recent forges, edit
│   ├── ActiveWorkoutViewModel.cs   # Allenamento: ricerca, esercizi, set, salvataggio, foto progresso
│   ├── StartSessionViewModel.cs    # Quick Start, Create Plan, Your Protocols
│   ├── WorkoutDetailViewModel.cs   # Dettaglio allenamento: serie, kg, reps, note, foto
│   ├── LoginViewModel.cs           # Login, Register, auto-login
│   ├── FriendRequestsViewModel.cs  # Friend requests + like notifications
│   └── SettingsViewModel.cs        # Toggle tema, lingua, CSV import/export, logout
├── Views/
│   ├── HomePage.xaml(.cs)          # Dashboard
│   ├── FeedPage.xaml(.cs)          # Feed &amp; search
│   ├── StatsPage.xaml(.cs)         # Statistiche
│   ├── ProfilePage.xaml(.cs)       # Profilo utente
│   ├── ActiveWorkoutPage.xaml(.cs) # Allenamento attivo
│   ├── StartSessionPage.xaml(.cs)  # Start Session
│   ├── WorkoutDetailPage.xaml(.cs) # Dettaglio allenamento
│   ├── LoginPage.xaml(.cs)         # Login/Register
│   ├── FriendRequestsPage.xaml(.cs)# Notifiche
│   └── SettingsPage.xaml(.cs)      # Impostazioni
├── Converters/
│   └── InverseBoolConverter.cs
└── Resources/
    ├── Styles/Styles.xaml          # Stili globali
    ├── Fonts/                      # Inter, Lexend, Space Grotesk, OpenSans
    ├── AppIcon/                    # appicon.png
    ├── Images/
    └── Raw/                        # forge.env (da .env al build)
```

## 3. Pattern applicativi

- **MVVM**: `CommunityToolkit.Mvvm` con `[ObservableProperty]` e `[RelayCommand]`
- **Shell Navigation**: TabBar (Dashboard, Feed, Stats) + 7 route di dettaglio
- **Dependency Injection**: `MauiProgram.cs` come composition root, servizi singleton, ViewModel/Page transient
- **Messaging**: `WeakReferenceMessenger` per refresh post-salvataggio (`WorkoutSavedMessage`)
- **XAML**: compiled bindings con `x:DataType` su tutte le pagine

## 4. Navigazione Shell

```xml
<TabBar>
    <ShellContent Route="dashboard" ContentTemplate="{DataTemplate views:HomePage}" />
    <ShellContent Route="feed"       ContentTemplate="{DataTemplate views:FeedPage}" />
    <ShellContent Route="stats"      ContentTemplate="{DataTemplate views:StatsPage}" />
</TabBar>
```

```csharp
// Route di dettaglio
Routing.RegisterRoute("activeWorkout", typeof(ActiveWorkoutPage));
Routing.RegisterRoute("workoutDetail", typeof(WorkoutDetailPage));
Routing.RegisterRoute("notifications", typeof(FriendRequestsPage));
Routing.RegisterRoute("settings", typeof(SettingsPage));
Routing.RegisterRoute("profile", typeof(ProfilePage));
Routing.RegisterRoute("startSession", typeof(StartSessionPage));
Routing.RegisterRoute("login", typeof(LoginPage));
Routing.RegisterRoute("friendRequests", typeof(FriendRequestsPage));
```

## 5. PocketBase Service

### Metodi principali

| Metodo | Descrizione |
|--------|-------------|
| `LoginAsync(email, password)` | Auth PocketBase, salva token e user |
| `RegisterAsync(email, password, name)` | Crea account + auth |
| `TryAutoLoginAsync()` | Login con credenziali salvate in Preferences |
| `Logout()` | Cancella token e credenziali |
| `RefreshUserAsync()` | Ricarica record utente (per avatar dopo upload) |
| `UpdateUserAsync(name, bio)` | PATCH nome e bio |
| `UploadAvatarAsync(stream, filename)` | Multipart PATCH file avatar |
| `GetFileUrl(collectionId, recordId, filename)` | URL con token per file PocketBase |
| `GetMyWorkoutsAsync(limit)` | Lista allenamenti utente (JsonDocument parsing) |
| `GetFollowedWorkoutsAsync()` | Allenamenti degli amici seguiti + avatar |
| `SaveWorkoutPlanAsync(name, date, volume, duration, exercises)` | Salva allenamento |
| `LikeWorkoutAsync(workoutId)` | GET liked_by → aggiungi userId → PATCH |
| `UnlikeWorkoutAsync(workoutId)` | GET liked_by → rimuovi userId → PATCH |
| `SearchUsersAsync(query)` | Ricerca utenti per nome |
| `SendFollowRequestAsync(targetUserId)` | Crea social_graph record |
| `AcceptFollowRequestAsync(recordId)` | PATCH status = "accepted" |
| `RejectFollowRequestAsync(recordId)` | PATCH status = "rejected" |
| `GetPendingRequestsAsync()` | Richieste in sospeso (to_user = current) |
| `GetFollowingUserIdsAsync()` | Lista ID utenti seguiti |
| `GetLikeNotificationsAsync()` | Chi ha messo like a quali workout |

### Collezioni PocketBase

| Collection | Campi | API Rules |
|-----------|-------|-----------|
| `users` (built-in) | email, password, name, bio, avatar | Auth PocketBase |
| `logged_workouts` | user, user_name, name, date, exercises (json), exercise_data (json), volume (num), duration (num), likes (num), liked_by (json) | List/Search/View/Update: `@request.auth.id != ""` |
| `social_graph` | from_user, from_name, to_user, status | Create/List: `@request.auth.id != ""` |
| `excercise` | name, bodyPart, equipment, instructions (json), imageUrl (url), category, level, force, mechanic | Create/List: `@request.auth.id != ""` |

## 6. Theme Service

```csharp
public class ThemeService
{
    private readonly Dictionary<string, string> darkPalette;
    private readonly Dictionary<string, string> lightPalette;

    public void Apply(bool isDark)
    {
        var palette = isDark ? darkPalette : lightPalette;
        foreach (var (key, hex) in palette)
            Application.Current.Resources[key] = Color.FromArgb(hex);
        Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }
}
```

- I colori sono definiti inline come `Dictionary<string, string>` (no dipendenze file XAML a runtime)
- `WriteResources()` aggiorna i valori nello stesso dizionario → `DynamicResource` si aggiorna automaticamente
- `UserAppTheme` sync per status bar Android

## 7. Build Secrets

```
.env (root) → MSBuild Copy → Resources/Raw/forge.env → FileSystem.OpenAppPackageFileAsync → Dictionary
```

- `ConcurrentDictionary<string, string>` per thread safety
- `catch (Exception)` generico per robustezza
- `.gitignore` esclude `.env` e `**/Resources/Raw/*.env`
- `.env.example` fornito come template senza valori reali

## 8. Flusso auto-login

```
App.CreateWindow()
  ├── ThemeService.Initialize()
  ├── Mostra SplashPage (2s animazione fade-in)
  └── window.Created → InitializeAsync()
        ├── BuildSecrets.LoadAsync()
        └── SyncService.SyncPendingWorkoutsAsync()
  └── Se primo avvio → SetupPage (configura URL server)
  └── Altrimenti → LoginPage

LoginViewModel costruttore:
  └── Task.Run → TryAutoLoginAsync()
        └── Se successo: window.Page = new AppShell()
```

## 9. Display Models (classi interne ai ViewModel)

| Classe | ViewModel | Campi |
|--------|-----------|-------|
| `SquadMember` | HomeViewModel | Name, Initial, AvatarSource, HasWorkout, HasAvatar |
| `FeedPost` | FeedViewModel | WorkoutId, UserName, Initial, TimeAgo, Title, ExercisesList, Volume, Duration, Likes, IsLiked, HeartIcon, AvatarSource, HasAvatar |
| `UserSearchResult` | FeedViewModel | UserId, Name, Initial, IsFollowing, FollowLabel, AvatarSource, HasAvatar |
| `LiftEntry` | StatsViewModel | Name, Label, Weight |
| `LikeNotification` | StatsViewModel | WorkoutName, LikeCount, Date |
| `CalendarDay` | StatsViewModel | Day, HasWorkout, IsToday |
| `ProfileWorkout` | ProfileViewModel | Title, Date, Duration, Volume, Likes, BorderColor |
| `FriendRequestItem` | FriendRequestsViewModel | RequestId, FromUserId, FromName |
| `LikeNotificationItem` | FriendRequestsViewModel | LikerName, WorkoutName |
| `ProtocolCard` | StartSessionViewModel | Id, Name, ExerciseCount, Duration, HasExercises |
| `ExerciseSearchResult` | ActiveWorkoutViewModel | Id, Name, BodyPart, Equipment, ImageUrl |
| `DetailExercise` | WorkoutDetailViewModel | Name, BodyPart, Equipment, Notes, Sets |
| `DetailSet` | WorkoutDetailViewModel | SetNumber, WeightKg, Reps, IsCompleted |

## 10. Flusso dati tipico (START WORKOUT)

```
1. HomePage: tap "▶ START WORKOUT"
2. Shell.GoToAsync("startSession")
3. StartSessionPage: tap piano salvato o Quick Start
4. Shell.GoToAsync("activeWorkout", { mode, planId })
5. ActiveWorkoutPage: cerca esercizi da ExerciseDB API
6. Aggiungi esercizio → aggiungi set (kg, reps)
7. Tap ✓ per completare set (LimeGreen fill)
8. Tap FINISH → ActiveWorkoutViewModel.SaveWorkoutAsync()
9. PocketBaseService.SaveWorkoutPlanAsync() → POST logged_workouts
10. WeakReferenceMessenger.Send(WorkoutSavedMessage)
11. HomeViewModel/StatsViewModel/ProfileViewModel ricevono messaggio
12. Refresh automatico: LoadAsync() in tutti i ViewModel
```
