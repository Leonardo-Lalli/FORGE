# Architettura - GymTracker Mobile

## 1. Obiettivo architetturale

Costruire una app `.NET MAUI` Android-first per il tracking degli allenamenti che integri tre fonti dati con responsabilità chiare:

- **ExerciseDB API** (RapidAPI): catalogo esercizi remoto;
- **Firebase** (Auth + Realtime Database/Firestore): autenticazione e dati social;
- **SQLite** (locale): cache esercizi, allenamenti, peso/misure, piani, profilo.

L'architettura deve supportare l'uso offline per la registrazione allenamenti (con esercizi in cache) e la sincronizzazione asincrona con Firebase quando la rete torna disponibile. Il design dell'interfaccia segue il Design System "Performance Minimalist" del progetto Stitch "Iron Rank Fitness Social" (`5765971046385640743`): dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter.

## 2. Struttura del repository e del progetto

### Cartelle principali

```
src/GymTracker.Mobile/
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs
├── MauiProgram.cs                  # DI composition root
├── Models/                         # Entità dominio e DTO
│   ├── Exercise.cs
│   ├── Workout.cs
│   ├── WorkoutExercise.cs
│   ├── ExerciseSet.cs
│   ├── BodyWeight.cs
│   ├── BodyMeasurement.cs
│   ├── User.cs
│   ├── FriendRequest.cs
│   ├── LeaderboardEntry.cs
│   ├── WorkoutPlan.cs
│   ├── PlanDay.cs
│   └── Dto/                        # DTO per API esterne
│       ├── ExerciseDbDto.cs
│       ├── FirebaseWorkoutDto.cs
│       └── FirebaseUserDto.cs
├── Data/                           # Accesso dati locale
│   ├── DatabaseService.cs          # SQLiteAsyncConnection singleton
│   ├── ExerciseCacheRepository.cs
│   ├── WorkoutRepository.cs
│   ├── BodyRepository.cs
│   ├── UserRepository.cs
│   └── WorkoutPlanRepository.cs
├── Services/                       # Business logic e API client
│   ├── ExerciseApiService.cs       # HTTP client ExerciseDB
│   ├── ExerciseCacheService.cs     # Logica cache esercizi
│   ├── ExerciseService.cs          # Orchestratore API + cache
│   ├── FirebaseAuthService.cs      # Auth Firebase REST
│   ├── FirebaseDatabaseService.cs  # CRUD Firebase Realtime DB
│   ├── SyncService.cs              # Coda sincronizzazione offline
│   ├── SocialService.cs            # Logica amici/competizione
│   └── ConnectivityService.cs      # Monitor connessione
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── CatalogViewModel.cs
│   ├── ExerciseDetailViewModel.cs
│   ├── ActiveWorkoutViewModel.cs
│   ├── WorkoutHistoryViewModel.cs
│   ├── WorkoutDetailViewModel.cs
│   ├── BodyViewModel.cs
│   ├── StatsViewModel.cs
│   ├── SocialViewModel.cs
│   ├── FriendFeedViewModel.cs
│   ├── LeaderboardViewModel.cs
│   ├── FriendCompareViewModel.cs
│   ├── PlansViewModel.cs
│   └── PlanDetailViewModel.cs
├── Views/
│   ├── LoginPage.xaml
│   ├── RegisterPage.xaml
│   ├── DashboardPage.xaml
│   ├── CatalogPage.xaml
│   ├── ExerciseDetailPage.xaml
│   ├── NewWorkoutPage.xaml
│   ├── ActiveWorkoutPage.xaml
│   ├── WorkoutHistoryPage.xaml
│   ├── WorkoutDetailPage.xaml
│   ├── BodyTrackingPage.xaml
│   ├── StatsPage.xaml
│   ├── FriendsPage.xaml
│   ├── FriendSearchPage.xaml
│   ├── FriendFeedPage.xaml
│   ├── LeaderboardPage.xaml
│   ├── FriendComparePage.xaml
│   ├── PlansPage.xaml
│   └── PlanDetailPage.xaml
├── Converters/
│   ├── BoolToVisibilityConverter.cs
│   ├── InverseBoolConverter.cs
│   └── DateTimeFormatConverter.cs
├── Resources/
│   ├── Styles/
│   │   ├── Colors.xaml
│   │   └── Styles.xaml
│   ├── Fonts/
│   └── Images/
└── Platforms/Android/
    ├── AndroidManifest.xml
    └── MainApplication.cs
```

### Responsabilità per area

- `Views/`: XAML puro, binding, stili. Zero logica di business.
- `ViewModels/`: stato UI (IsBusy, ErrorMessage, HasData, IsEmpty), comandi, orchestrazione servizi.
- `Services/`: logica di business, chiamate API, caching, sincronizzazione.
- `Data/`: accesso SQLite, repository pattern leggero.
- `Models/`: entità dominio e DTO separati. I DTO vivono in `Models/Dto/`.
- `Converters/`: value converters per binding XAML.
- `Resources/`: stili, colori, font del Design System Stitch.

## 3. Pattern applicativi

- **MVVM**: `CommunityToolkit.Mvvm` con `[ObservableProperty]` e `[RelayCommand]`.
- **Shell Navigation**: tab bar principale + route di dettaglio.
- **Dependency Injection**: `MauiProgram.cs` come composition root, servizi singleton, ViewModel transient.
- **Repository pattern**: leggero, senza astrazioni generiche eccessive.
- **Offline-first**: scrittura sempre su SQLite, sincronizzazione Firebase asincrona.
- **XAML**: compiled bindings con `x:DataType` dove possibile.

## 4. Componenti principali

### Views

| Pagina | Tab | Descrizione |
| --- | --- | --- |
| `LoginPage` / `RegisterPage` | Fuori Shell | Autenticazione iniziale |
| `DashboardPage` | Tab 1 | Riepilogo rapido, accesso funzioni |
| `CatalogPage` | Tab 2 | Catalogo esercizi con ricerca e filtri |
| `ActiveWorkoutPage` | Tab 3 | Allenamento in corso o storico |
| `FriendsPage` | Tab 4 | Social: amici, feed, leaderboard |
| `ProfilePage` | Tab 5 | Profilo, logout, impostazioni |
| `ExerciseDetailPage` | Detail | Dettaglio esercizio con GIF |
| `WorkoutDetailPage` | Detail | Dettaglio allenamento passato |
| `BodyTrackingPage` | Detail | Peso e misure |
| `StatsPage` | Detail | Statistiche complete |
| `LeaderboardPage` | Detail | Classifica settimanale |
| `FriendComparePage` | Detail | Confronto diretto |
| `PlansPage` / `PlanDetailPage` | Detail | Piani allenamento |

### ViewModels

Ogni ViewModel eredita da `BaseViewModel` che espone:

- `[ObservableProperty] bool isBusy`
- `[ObservableProperty] string errorMessage`
- `[ObservableProperty] bool hasData`
- `[ObservableProperty] bool isEmpty`
- `[ObservableProperty] bool hasError`

Esempi chiave:

- `DashboardViewModel`: aggrega dati da SQLite (ultimo allenamento, peso, streak) e Firebase (posizione leaderboard).
- `CatalogViewModel`: orchestra `ExerciseService` (API + cache), gestisce ricerca testuale e filtri.
- `ActiveWorkoutViewModel`: stato mutabile dell'allenamento in corso, aggiunta/rimozione esercizi e serie.
- `SocialViewModel`: ricerca utenti, invio/gestione richieste, lista amici.
- `LeaderboardViewModel`: query Firebase ordinata per volume, refresh.
- `SyncService`: coda interna di allenamenti da sincronizzare, osservabile per UI status (sync pending, syncing, synced, error).

### Services

| Servizio | Responsabilità |
| --- | --- |
| `ExerciseApiService` | HTTP client ExerciseDB (RapidAPI header, endpoint bodyPart/name/equipment) |
| `ExerciseCacheService` | Salva/recupera da SQLite gli esercizi usati (max 50) |
| `ExerciseService` | Orchestratore: chiama API, salva in cache, restituisce al VM |
| `FirebaseAuthService` | REST API Firebase Auth: signUp, signIn, refresh token |
| `FirebaseDatabaseService` | CRUD su Realtime Database/Firestore per allenamenti, utenti, richieste |
| `SyncService` | Coda di sincronizzazione: invia allenamenti locali a Firebase, retry, stato |
| `SocialService` | Logica amici: ricerca, richieste, feed, leaderboard |
| `ConnectivityService` | Wrapper `IConnectivity` MAUI per monitorare stato rete |

### Models e DTO

I modelli di dominio sono separati dai DTO di integrazione:

```csharp
// Modelli dominio
public class Exercise { string Id, Name, BodyPart, Equipment, Target, GifUrl, List<string> Instructions; }
public class Workout { string Id, DateTime Date, TimeSpan? Duration, List<WorkoutExercise> Exercises; }
public class WorkoutExercise { string ExerciseId, string ExerciseName, List<ExerciseSet> Sets; }
public class ExerciseSet { int SetNumber; double WeightKg; int Reps; }
public class BodyWeight { string Id, DateTime Date; double WeightKg; }
public class BodyMeasurement { string Id, DateTime Date; double ChestCm, WaistCm, HipsCm, ArmsCm, LegsCm; }
public class User { string FirebaseUid, Username, Email; }
public class FriendRequest { string Id, SenderId, ReceiverId, Status, DateTime Timestamp; }
```

```csharp
// DTO ExerciseDB
public class ExerciseDbDto { string id, name, bodyPart, equipment, target, gifUrl, List<string> instructions; }
```

I DTO Firebase dipendono dalla scelta Realtime Database vs Firestore (TBD in IT-05).

## 5. Navigazione

### Route Shell

```xml
<Shell>
  <TabBar>
    <ShellContent Route="dashboard" Title="Home" ContentTemplate="{DataTemplate views:DashboardPage}" />
    <ShellContent Route="catalog" Title="Esercizi" ContentTemplate="{DataTemplate views:CatalogPage}" />
    <ShellContent Route="workout" Title="Allenamento" ContentTemplate="{DataTemplate views:ActiveWorkoutPage}" />
    <ShellContent Route="social" Title="Amici" ContentTemplate="{DataTemplate views:FriendsPage}" />
    <ShellContent Route="profile" Title="Profilo" ContentTemplate="{DataTemplate views:ProfilePage}" />
  </TabBar>
</Shell>

<!-- Route di dettaglio registrate in AppShell.cs -->
Routing.RegisterRoute("exerciseDetail", typeof(ExerciseDetailPage));
Routing.RegisterRoute("workoutDetail", typeof(WorkoutDetailPage));
Routing.RegisterRoute("bodyTracking", typeof(BodyTrackingPage));
Routing.RegisterRoute("stats", typeof(StatsPage));
Routing.RegisterRoute("leaderboard", typeof(LeaderboardPage));
Routing.RegisterRoute("friendCompare", typeof(FriendComparePage));
Routing.RegisterRoute("plans", typeof(PlansPage));
Routing.RegisterRoute("planDetail", typeof(PlanDetailPage));
```

### Parametri di navigazione

- `ExerciseDetailPage`: `exerciseId` (obbligatorio)
- `WorkoutDetailPage`: `workoutId` (obbligatorio)
- `FriendComparePage`: `friendId`, `friendName`
- `PlanDetailPage`: `planId`

La navigazione verso `LoginPage` / `RegisterPage` avviene fuori Shell (prima del `Shell.Current.GoToAsync`). Dopo login, `App.Current.MainPage = new AppShell()`.

## 6. Stato della UI

### Loading

- `ActivityIndicator` durante fetch API (catalogo, login, sync).
- Shimmer/skeleton placeholder per caricamento iniziale Dashboard (IT-07).
- Loading non bloccante per sync in background.

### Error

- Messaggio errore + pulsante "Riprova" su tutte le pagine con fetch remoto.
- Errori API ExerciseDB: messaggio specifico ("Impossibile caricare gli esercizi. Controlla la connessione.").
- Errori Firebase Auth: messaggi Firebase tradotti in italiano.
- Errori di sync mostrati come badge/piccolo avviso, non bloccanti.

### Empty

- Catalogo senza risultati: "Nessun esercizio trovato. Prova un'altra ricerca."
- Storico vuoto: "Nessun allenamento registrato. Inizia il tuo primo allenamento!" con pulsante CTA.
- Nessun amico: stato vuoto con CTA "Cerca amici".
- Feed amici vuoto (amici presenti ma senza allenamenti recenti).

### Success

- Dati caricati correttamente mostrati senza messaggi di conferma invasivi.
- Salvataggio allenamento: breve toast / snackbar di conferma.
- Sync completato: badge verde o assenza di badge errore.

## 7. Dati e integrazioni

### ExerciseDB API (RapidAPI)

```
Base URL: https://exercisedb.p.rapidapi.com
Headers: X-RapidAPI-Key, X-RapidAPI-Host
```

| Endpoint | Uso |
| --- | --- |
| `GET /exercises/bodyPart/{part}` | Esercizi per gruppo muscolare |
| `GET /exercises/bodyPartList` | Lista gruppi muscolari disponibili |
| `GET /exercises/name/{name}` | Ricerca per nome |
| `GET /exercises/equipment/{type}` | Filtro per attrezzatura |
| `GET /exercises/exercise/{id}` | Dettaglio esercizio singolo |

Il servizio `ExerciseApiService` è l'unico punto di contatto con l'API. Gestisce timeout (10s), retry (1 tentativo), e mapping DTO -> `Exercise`.

### Firebase

**Autenticazione**: Firebase Auth REST API per email/password.

- `POST https://identitytoolkit.googleapis.com/v1/accounts:signUp`
- `POST https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword`

Il token ID viene salvato in `Preferences` e usato per autenticare le richieste al database.

**Database**: Realtime Database o Firestore (TBD in IT-05).

Struttura prevista (Firestore):

```
/users/{uid}              # profilo utente: username, email
/friendRequests/{id}      # richieste: from, to, status, timestamp
/friendships/{uid}        # subcollection: friendUids
/workouts/{uid}           # subcollection: allenamenti condivisi
```

La struttura esatta sarà definita in IT-05 dopo la scelta Realtime DB vs Firestore.

### Persistenza locale (SQLite)

Tabelle SQLite (`sqlite-net-pcl`):

| Tabella | Campi chiave | Note |
| --- | --- | --- |
| `ExerciseCache` | Id, Name, BodyPart, Equipment, GifUrl, LastUsed | Max 50 righe, LRU |
| `Workout` | Id, Date, Duration, Notes, IsSynced | IsSynced per coda sync |
| `WorkoutExercise` | Id, WorkoutId, ExerciseId, ExerciseName | FK a Workout |
| `ExerciseSet` | Id, WorkoutExerciseId, SetNumber, WeightKg, Reps | FK a WorkoutExercise |
| `BodyWeight` | Id, Date, WeightKg | Dati sensibili, solo locale |
| `BodyMeasurement` | Id, Date, ChestCm, WaistCm, HipsCm, ArmsCm, LegsCm | Dati sensibili, solo locale |
| `User` | FirebaseUid, Username, Email | Cache profilo locale |
| `WorkoutPlan` | Id, Name, Description | Piani precaricati |
| `PlanDay` | Id, PlanId, DayName, Order | FK a WorkoutPlan |
| `PlanExercise` | Id, PlanDayId, ExerciseName, Sets, MinReps, MaxReps | FK a PlanDay |

Una sola `SQLiteAsyncConnection` condivisa, inizializzata da `DatabaseService` (singleton).

### Flusso offline-first

1. L'utente apre il catalogo: `ExerciseService` prova API → se fallisce, carica da `ExerciseCache`.
2. L'utente avvia allenamento: esercizi presi da catalogo (API o cache).
3. Salvataggio: scrittura SQLite con `IsSynced = false`.
4. `SyncService` ascolta `IConnectivity`: quando rete disponibile, itera workout con `IsSynced = false` e li invia a Firebase.
5. Dopo sync riuscito: `IsSynced = true`.
6. Peso e misure: solo SQLite, mai sincronizzati.

## 8. Dependency injection e composition root

```csharp
// MauiProgram.cs
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

// HTTP client ExerciseDB
builder.Services.AddHttpClient<ExerciseApiService>(client =>
{
    client.BaseAddress = new Uri("https://exercisedb.p.rapidapi.com");
    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "exercisedb.p.rapidapi.com");
});

// Firebase
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddSingleton<FirebaseDatabaseService>();

// Services
builder.Services.AddSingleton<ExerciseCacheService>();
builder.Services.AddSingleton<ExerciseService>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddSingleton<SocialService>();

// Repositories
builder.Services.AddSingleton<ExerciseCacheRepository>();
builder.Services.AddSingleton<WorkoutRepository>();
builder.Services.AddSingleton<BodyRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<WorkoutPlanRepository>();

// ViewModels (transient)
builder.Services.AddTransient<DashboardViewModel>();
builder.Services.AddTransient<CatalogViewModel>();
builder.Services.AddTransient<ActiveWorkoutViewModel>();
// ... tutti i ViewModel

// Pages (transient)
builder.Services.AddTransient<DashboardPage>();
builder.Services.AddTransient<CatalogPage>();
// ... tutte le Pages
```

I ViewModel ricevono i servizi via costruttore. I comandi usano `[RelayCommand]` con `async Task`.

## 9. Error handling e logging

- Le eccezioni di rete (`HttpRequestException`, `TaskCanceledException`) sono intercettate nei servizi API e tradotte in stati `ErrorMessage` nei ViewModel.
- Le eccezioni di parsing (`JsonException`) sono intercettate nel mapping DTO e generano stato di errore con messaggio "Dati non disponibili. Riprova più tardi."
- Gli errori SQLite sono loggati e propagati come `ErrorMessage` generico.
- Firebase Auth errori: mappatura dei codici Firebase (`EMAIL_EXISTS`, `INVALID_PASSWORD`, etc.) a messaggi in italiano.
- Logging via `ILogger<T>` standard di .NET, senza telemetria esterna.
- La chiave API ExerciseDB e le chiavi Firebase NON sono versionate: gestite via file di configurazione escluso da git (es. `appsettings.local.json` o `Constants.cs` escluso).

## 10. Decisioni aperte e TBD

- **TBD**: Realtime Database vs Firestore — scelta da fare in IT-05 dopo valutazione:
  - Realtime DB: più semplice, query limitate, buono per leaderboard semplici.
  - Firestore: query più potenti, subcollection, meglio per feed e confronti.
- **TBD**: Libreria grafici — Microcharts (leggera, open source) vs LiveChartsCore (più potente, MAUI support). Da valutare in IT-04.
- **TBD**: Strategia esatta di sync conflitti — per MVP, last-write-wins basato su timestamp. Se emergono conflitti reali, valutare merge in IT-05.
- **TBD**: Nome progetto nel file system — `src/GymTracker.Mobile/` è proposto. Confermare in IT-01.
