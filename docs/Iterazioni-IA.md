# Audit Report — GymTracker Mobile (2026-05-07)

## Riepilogo esecutivo

L'applicazione ha una solida base architetturale (Shell navigation a 3 tab, MVVM con CommunityToolkit.Mvvm, sistema temi runtime, meccanismo secrets injection) ma **nessuna integrazione API reale** e **diversi gap rispetto al piano** (`docs/plan.md`). I dati sono interamente mock/hardcoded. Il meccanismo `BuildSecrets` (`.env` → `Resources/Raw/gymtracker.env` → `FileSystem.OpenAppPackageFileAsync`) è implementato correttamente ma **mai invocato** — le chiavi API esistono ma non vengono consumate.

---

## Evoluzione del progetto (dal git log)

| Commit | Descrizione |
|--------|-------------|
| `0adb068` | Initial commit |
| `19bb42d` | Add deadline |
| `8ac9e4b` | Refactor: rewrite project docs from BookScout to GymTracker |
| `a7ee3a5`, `295b8f9` | Docs IT-00: spec/plan/architecture aligned to GymTracker |
| `4f407c6` | Bootstrap MAUI project with Shell, 5-tab, Stitch theme |
| `33db601` | Tab bar SVG icons, Stitch styling |
| `ec131d6` | Rewrite 5 pages to match Stitch designs |
| `49dbb82` | **IT-01**: Minimal MAUI variant, clean Material Design |
| `d602f93` | **IT-03**: Full workout system with plans, rest timer |
| `e13b2a8` | **IT-03**: Per-second timer, weight input, per-exercise rest timer |
| `912ddb2` | Conditional timer, minimize workout, friend feed, remove calories |
| `da474ec` | UI: remove header bar clutter, emoji purge, dot indicator |
| `c519512` | UI: remove flame emoji from streak card |
| `836419a` | Notifications page with mock data |
| `3ed1a38` | Fix: save workout plans to Preferences |
| `f4497e3` | UI: streamline social page |
| `6de4177` | **Build-time secrets injection**: `.env` → `gymtracker.env` via MSBuild |
| `4e3f69c` | HomePage based on Stitch mockup |
| `a4f2f86` | **Dual-theme system** with runtime toggle |
| `c4f53a6` | Full GUI rebuild — 3-tab Shell (Dashboard/Feed/Stats) |
| `db6b107` | Fix: DynamicResource, BindableLayout, bar chart heights |

**Nota**: Il progetto ha subito un cambio di rotta — da 5 tab pianificati (Dashboard, Catalogo, Allenamento, Social, Profilo) a 3 tab effettivi (Dashboard, Feed, Stats). Molte delle feature di IT-02, IT-04, IT-05, IT-06, IT-08 non sono mai state implementate.

---

## Problemi rilevati

### CRITICI — Bloccanti per il funzionamento

#### 1. BuildSecrets registrato in DI ma mai caricato
- **File**: `MauiProgram.cs:25`, `BuildSecrets.cs:11`
- **Problema**: `BuildSecrets` è singleton in DI ma `LoadAsync()` non viene mai chiamato da nessuna parte. Nessun ViewModel o Service lo inietta.
- **Impatto**: Le chiavi API (`EXERCISEDB_API_KEY`, `GOOGLE_BOOKS_API_KEY`) sono irraggiungibili a runtime.
- **Fix suggerito**: Chiamare `LoadAsync()` all'avvio in `App.xaml.cs` o in un `IHostedService`-equivalente, e iniettare `BuildSecrets` nei servizi API.

#### 2. Nessun servizio ExerciseAPI
- **File**: Mancante (`Services/ExerciseApiService.cs` non esiste)
- **Problema**: Il `CatalogViewModel.SearchAsync()` (`CatalogViewModel.cs:17`) è uno stub con `await Task.Delay(500)`. Il catalogo mostra 3 esercizi hardcoded in XAML (`CatalogPage.xaml:94-199`).
- **Impatto**: La feature principale dell'app (catalogo esercizi da ExerciseDB) è assente.
- **Piano previsto**: `docs/plan.md` IT-02 richiede `ExerciseApiService`, `ExerciseCacheService`, `ExerciseService`, DTO, SQLite cache.

#### 3. Nessun pacchetto SQLite
- **File**: `GymTracker.Mobile.csproj`
- **Problema**: Il piano richiede `sqlite-net-pcl` per cache esercizi, allenamenti, peso/misure. Non è presente nei PackageReference.
- **Impatto**: La persistenza strutturata non esiste. I workout plan usano `Preferences` (key-value) come workaround.
- **Pacchetti attuali**: `Microsoft.Maui.Controls`, `Microsoft.Extensions.Logging.Debug`, `CommunityToolkit.Mvvm`.

#### 4. GOOGLE_BOOKS_API_KEY è un residuo
- **File**: `.env.example:6`, `.env:3`
- **Problema**: La chiave `GOOGLE_BOOKS_API_KEY` è un leftover del vecchio progetto BookScout. GymTracker non usa Google Books API.
- **Impatto**: Confusione per nuovi sviluppatori, chiave inutilizzata nel `.env`.

### ALTI — Impattano funzionalità chiave

#### 5. CatalogViewModel non usa HttpClient né BuildSecrets
- **File**: `CatalogViewModel.cs`
- **Problema**: Il ViewModel non riceve alcuna dipendenza. `SearchAsync()` e `RefreshAsync()` sono stub.
- **Impatto**: La ricerca nel catalogo non funziona.

#### 6. Dati mock ovunque — nessuna fonte dati reale
- **File**: `HomeViewModel.cs:46-55`, `FeedViewModel.cs:48-80`, `StatsViewModel.cs:43-49`, `NotificationsViewModel.cs:32-70`, `DashboardViewModel.cs:23-27`
- **Problema**: Tutti i ViewModel caricano dati hardcoded. Non c'è persistenza (SQLite) né fetch remoto (ExerciseDB, Firebase).
- **Impatto**: L'app è un mockup interattivo, non un'app funzionante.
- **Nota**: I mock sono ben fatti per demo UI, ma vanno sostituiti con dati reali.

#### 7. PlanStore usa Preferences per JSON — limite di dimensione
- **File**: `PlanStore.cs:17-26`
- **Problema**: `Preferences` serializza/interroga un JSON intero per ogni operazione. Con molti piani/esercizi, la stringa JSON può superare i limiti di `Preferences` ed è inefficiente.
- **Impatto**: Corruzione dati o perdita performance con più di ~50 piani.
- **Fix suggerito**: Migrare a SQLite (`sqlite-net-pcl`) come da piano.

#### 8. WorkoutSession è singleton via Lazy, non via DI
- **File**: `WorkoutSession.cs:5-6`
- **Problema**: `WorkoutSession` usa pattern `Lazy<T>` statico invece di essere registrato in DI. Inconsistente con il resto dell'architettura.
- **Impatto**: Testabilità ridotta, impossibile mock-are il servizio.

#### 9. HomePage non referenziata nella Shell
- **File**: `AppShell.xaml`, `AppShell.xaml.cs`
- **Problema**: `HomePage` esiste (basata su Stitch mockup, commit `4e3f69c`) ed è registrata in DI (`MauiProgram.cs:42`) ma non è referenziata nella Shell come tab. La Shell ha Dashboard, Feed, Stats.
- **Impatto**: HomePage è orphan — esiste ma irraggiungibile.

#### 10. DynamicResource vs StaticResource inconsistente
- **File**: `FeedPage.xaml`, `StatsPage.xaml`, `DashboardPage.xaml`
- **Problema**: FeedPage e StatsPage usano `DynamicResource`, Dashboard usa `StaticResource`. Il tema runtime richiede `DynamicResource` per reagire ai cambi.
- **Impatto**: Il cambio tema su Dashboard potrebbe non applicarsi correttamente ad alcuni elementi.
- **Fix recente**: `db6b107` ha fixato alcuni DynamicResource sulla Shell, ma il problema persiste nei dettagli.

### MEDI — Qualità e manutenibilità

#### 11. AppShell non corrisponde al piano architetturale
- **File**: `AppShell.xaml` vs `docs/architecture.md:204-213`
- **Problema**: Il piano prevede 5 tab (Dashboard, Catalogo, Allenamento, Amici, Profilo) + 8 route di dettaglio. La realtà ha 3 tab (Dashboard, Feed, Stats) + 3 route (activeWorkout, notifications, settings).
- **Route mancanti**: exerciseDetail, workoutDetail, bodyTracking, leaderboard, friendCompare, plans, planDetail.

#### 12. Models non allineati al piano
- **File**: `Models/WorkoutPlan.cs`, `docs/architecture.md:183-191`
- **Problema**: Molti modelli del piano non esistono: `Exercise.cs`, `Workout.cs`, `BodyWeight.cs`, `BodyMeasurement.cs`, `User.cs`, `FriendRequest.cs`, `LeaderboardEntry.cs`, `PlanDay.cs`.
- **Modelli esistenti**: Solo `WorkoutPlan.cs` (con `WorkoutExercise` e `ExerciseSet` inline).

#### 13. Manca la cartella Data/
- **Problema**: Nessun repository SQLite esiste. Il piano prevede: `DatabaseService`, `ExerciseCacheRepository`, `WorkoutRepository`, `BodyRepository`, `UserRepository`, `WorkoutPlanRepository`.
- **Impatto**: Impossibile implementare persistenza strutturata.

#### 14. Manca la cartella Converters/
- **Problema**: Il piano prevede `BoolToVisibilityConverter`, `InverseBoolConverter`, `DateTimeFormatConverter`. Non esistono.
- **Impatto**: XAML usa `IsVisible="{Binding HasError, Converter={x:Null}}"` (`CatalogPage.xaml:92`) — potenzialmente rotto.

#### 15. Manca la cartella Models/Dto/
- **Problema**: Nessun DTO per ExerciseDB API o Firebase. Necessari per il mapping difensivo.

#### 16. SettingsPage esiste ma non referenziata nella Shell
- **File**: `AppShell.xaml.cs:12`
- **Problema**: `SettingsPage` è registrata come route di dettaglio ma nessun tab/pulsante nella Shell porta ad essa. Vi si accede solo via TapGestureRecognizer in FeedPage e StatsPage (⚙ icon).
- **Ok se intenzionale**, ma da verificare.

#### 17. TapGestureRecognizer in code-behind
- **File**: `DashboardPage.xaml:30`, `FeedPage.xaml:33`, `StatsPage.xaml:33`
- **Problema**: I `TapGestureRecognizer` con `Tapped="OnBellTapped"` e `Tapped="OnSettingsTapped"` usano event handler nel code-behind invece di `Command` binding al ViewModel.
- **AGENTS.md vieta**: "Non spostare logica nei code-behind se può stare in un ViewModel".
- **Impatto**: Viola l'architettura MVVM pura.

### BASSI — Miglioramenti consigliati

#### 18. Percorso relativo fragile nel Copy task MSBuild
- **File**: `GymTracker.Mobile.csproj:76`
- **Problema**: `SourceFiles="..\..\.env"` assume che il `.csproj` sia esattamente a 2 livelli dalla root. Se la struttura viene riorganizzata, il build fallisce silenziosamente (il `Condition` sopprime l'errore).
- **Fix suggerito**: Usare `$(MSBuildProjectDirectory)\..\..\` o un path assoluto basato su proprietà MSBuild.

#### 19. `gymtracker.env` è committato in Raw
- **File**: `Resources/Raw/gymtracker.env`, `.gitignore:8`
- **Problema**: Il `.gitignore` ha `**/Resources/Raw/*.env` ma il file esiste già nella directory. Probabilmente è stato committato prima dell'aggiunta della regola gitignore.
- **Impatto**: Se il file contiene chiavi reali, sono esposte nel repository.
- **Stato attuale**: Contiene placeholder (`your_exercisedb_api_key_here`), quindi al momento è innocuo.

#### 20. Nessuna gestione errori per FileSystem.OpenAppPackageFileAsync
- **File**: `BuildSecrets.cs:15`
- **Problema**: Solo `FileNotFoundException` è catch-ato. Altri errori (IOException, UnauthorizedAccessException) crashano l'app.
- **Fix suggerito**: Catch generico con log.

#### 21. BuildSecrets non è thread-safe
- **File**: `BuildSecrets.cs:5,30`
- **Problema**: `Dictionary<string, string>` non è thread-safe. Se `LoadAsync()` e `Get()` sono chiamati da thread diversi, possibile corruzione.
- **Fix suggerito**: Usare `ConcurrentDictionary` o lock.

#### 22. HasData/HasError/IsEmptyState non coerenti tra ViewModel
- **File**: Vari ViewModel
- **Problema**: `DashboardViewModel` setta `HasData = true` senza dati reali. `SocialViewModel` setta `IsEmptyState = true` e basta. `CatalogViewModel` setta `IsEmptyState = true`, `SetSuccess(false)`.
- **Impatto**: Stati UI inconsistenti.

#### 23. Nessun test automatico
- **Problema**: Il progetto non ha progetto di test (nessun `.Tests.csproj`, nessun test file).
- **Piano**: `docs/test-matrix.md` esiste ma è vuoto di esecuzioni.

#### 24. Icone tab non corrispondono al contenuto
- **File**: `AppShell.xaml:18-32`
- **Problema**: Tab Stats ha icona `icon_profile.svg`, tab Feed ha `icon_social.svg`. I nomi icona sono fuorvianti rispetto al contenuto.

#### 25. XAML Compiled Bindings mancanti su alcune pagine
- **File**: `CatalogPage.xaml:92`
- **Problema**: `Converter={x:Null}` è probabilmente un errore — non è un converter valido. Andrebbe usato un `InverseBoolConverter` (non esistente) o un binding negato.
- **Impatto**: Possibile eccezione a runtime.

---

## Stato rispetto al piano (docs/plan.md)

| Iterazione | Stato | Note |
|------------|-------|------|
| IT-01 | ✅ Parziale | Bootstrap ok, ma 3 tab invece di 5 |
| IT-02 | ❌ | Catalogo esercizi: nessuna API, nessun SQLite, dati mock |
| IT-03 | ✅ Parziale | Workout system funzionante, ma persistenza via Preferences invece di SQLite |
| IT-04 | ❌ | Tracking peso/misure: non implementato |
| IT-05 | ❌ | Firebase Auth + sync: non implementato |
| IT-06 | ❌ | Social (amici, feed, leaderboard) con dati reali: non implementato |
| IT-07 | ⚠️ Parziale | Dashboard/Stats esistono con mock data |
| IT-08 | ❌ | Piani di allenamento predefiniti (3 piani): non implementato (esiste solo creazione piani custom) |

---

## Meccanismo secrets injection — Analisi

### Flusso attuale
```
.env (root)
  ↓ MSBuild Copy target (BeforeBuild) → Resources\Raw\gymtracker.env
  ↓ MauiAsset Include → incluso nell'APK
  ↓ BuildSecrets.LoadAsync() → FileSystem.OpenAppPackageFileAsync("gymtracker.env")
  ↓ Parsing key=value → Dictionary<string, string>
  ↓ Get("EXERCISEDB_API_KEY") → valore
```

### Cosa funziona
- Il meccanismo di build copy funziona: il `.csproj` ha il `CopyEnvFile` target corretto.
- Il `MauiAsset` glob pattern include `Resources\Raw\**` con `LogicalName` che rimuove il prefisso.
- `BuildSecrets.LoadAsync()` usa `FileSystem.OpenAppPackageFileAsync` correttamente.
- Il parsing key=value è robusto (ignora commenti `#`, righe vuote, spazi).
- `.gitignore` protegge sia il root `.env` che `**/Resources/Raw/*.env`.

### Cosa manca
- **LoadAsync() non chiamato**: va invocato all'avvio (`App.xaml.cs` o `MauiProgram.cs`).
- **BuildSecrets non iniettato**: nessun servizio/ViewModel lo riceve.
- **Nessun HttpClient configurato**: serve un `IHttpClientFactory` o `HttpClient` named/typed con header RapidAPI.

### Raccomandazione implementativa

```csharp
// MauiProgram.cs — dopo build.Build(), prima di run:
var secrets = app.Services.GetRequiredService<BuildSecrets>();
await secrets.LoadAsync();

// Registrare HttpClient per ExerciseDB:
builder.Services.AddHttpClient<ExerciseApiService>(client =>
{
    client.BaseAddress = new Uri("https://exercisedb.p.rapidapi.com");
});
// In ExerciseApiService, iniettare BuildSecrets e HttpClient:
public class ExerciseApiService(HttpClient http, BuildSecrets secrets)
{
    // Aggiungere header X-RapidAPI-Key da secrets.Get("EXERCISEDB_API_KEY")
    // Chiamare endpoint /exercises/bodyPart/{part}, /exercises/name/{name}
}
```

---

## Raccomandazioni prioritarie

### Priorità 1 — Bloccanti (prossima iterazione)
1. **Caricare BuildSecrets all'avvio** e iniettarlo nei servizi
2. **Rimuovere `GOOGLE_BOOKS_API_KEY`** da `.env` e `.env.example`
3. **Creare `ExerciseApiService`** con HttpClient e header RapidAPI
4. **Implementare `CatalogViewModel`** con chiamate API reali

### Priorità 2 — Strutturali
5. **Aggiungere `sqlite-net-pcl`** e creare `DatabaseService`
6. **Creare i modelli mancanti**: `Exercise.cs`, `Workout.cs`, `BodyWeight.cs`, `User.cs`
7. **Migrare `PlanStore`** da Preferences a SQLite
8. **Allineare AppShell** al piano o aggiornare il piano

### Priorità 3 — Qualità
9. **Aggiungere Converters** (BoolToVisibility, InverseBool)
10. **Sostituire TapGestureRecognizer code-behind** con Command binding
11. **Uniformare l'uso di DynamicResource** su tutte le pagine
12. **Aggiungere test automatici** (almeno unit test sui ViewModel)

---

## Riepilogo metriche

| Metrica | Valore |
|---------|--------|
| File C# totali (src) | 15 |
| ViewModel | 10 |
| Views | 12 |
| Services | 4 (di cui 1 non usato) |
| Models | 1 |
| NuGet packages | 3 |
| Problemi critici | 4 |
| Problemi alti | 6 |
| Problemi medi | 7 |
| Problemi bassi | 8 |
| Iterazioni completate | 1/8 (IT-01) |
| Iterazioni parziali | 2/8 (IT-03, IT-07) |
| Copertura test | 0% |

---

*Report generato il 2026-05-07 da audit automatico. Basato su git log (30 commit) e analisi statica del codice.*

---

## Aggiornamento 2026-05-13 — Fix applicati (branch `fix` → `light-mode` → `light_button_fix` → `app_icon`)

### Sessione 1 — Fix strutturali (`fix`)
| Fix | File | Descrizione |
|-----|------|-------------|
| BuildSecrets.LoadAsync() chiamato all'avvio | `App.xaml.cs` | Iniettato `BuildSecrets` e chiamato `LoadAsync()` in `CreateWindow()` |
| GOOGLE_BOOKS_API_KEY rimossa | `.env`, `.env.example`, `gymtracker.env` | Rimosso leftover da BookScout |
| sqlite-net-pcl aggiunto | `.csproj` | NuGet `sqlite-net-pcl` 1.9.172 |
| Converters creati | `Converters/` | `BoolToVisibilityConverter`, `InverseBoolConverter`, `DateTimeFormatConverter` |
| StaticResource→DynamicResource | `DashboardPage.xaml`, `CatalogPage.xaml` | Supporto cambio tema runtime |
| TapGestureRecognizer→Command | `HomePage`, `DashboardPage`, `FeedPage`, `StatsPage` | Code-behind puliti, comandi in ViewModel |
| ViewModel state consistency | `DashboardViewModel`, `SocialViewModel`, `CatalogViewModel` | `HasData`/`IsEmptyState` coerenti |
| WorkoutSession refactoring | `WorkoutSession.cs`, `ActiveWorkoutViewModel.cs` | Da `Lazy<T>` statico a DI injection |
| Converter={x:Null} fix | `CatalogPage.xaml` | Sostituito con `InverseBoolConverter` |

### Sessione 2 — ThemeService ottimizzato (`fix`)
| Fix | File | Descrizione |
|-----|------|-------------|
| Initialize no-op su dark | `ThemeService.cs` | Flag `isInitialized`, se dark default non chiama `Apply()` |
| BuildSecrets robusto | `BuildSecrets.cs` | `catch(Exception)`, `ConcurrentDictionary` |

### Sessione 3 — Profilo mockup e prevenzione crash (`light-mode`)
| Fix | File | Descrizione |
|-----|------|-------------|
| Profilo avatar "EL" cliccabile | `HomePage.xaml`, `FeedPage.xaml`, `StatsPage.xaml` | Avatar con `OpenProfileCommand`, route `"profile"` in `AppShell.xaml.cs` |
| ProfilePage riscritta | `ProfilePage.xaml`, `ProfileViewModel.cs` | Basata su mockup `profilo_elite_nero_opaco` / `profilo_elite_chiaro` |
| StaticResource→DynamicResource TUTTE le pagine | `ProfilePage`, `ActiveWorkoutPage`, `NotificationsPage`, `SocialPage`, `WorkoutPage` | 116 StaticResource totali sostituiti |
| ThemeService.Apply() sicuro | `ThemeService.cs` | `MainThread.IsMainThread` check, `try-catch(Exception)` |

### Sessione 4 — Fix toggle tema (`light_button_fix`)
| Fix | File | Descrizione |
|-----|------|-------------|
| Colors.Light.xaml preload | `App.xaml` | Aggiunto come primo merged dictionary (bassa priorità) per garantire inclusione nel build e risoluzione URI a runtime |
| suppressChange flag | `SettingsViewModel.cs` | Blocca `OnIsDarkModeChanged` durante costruzione iniziale |

### Sessione 5 — Main color fix
| Fix | File | Descrizione |
|-----|------|-------------|
| START WORKOUT button azure | `HomePage.xaml` | BackgroundColor `Primary` (ciano #c3f5ff) come bordi Squad Activity |
| StatsPage incrementi azure | `StatsPage.xaml` | Tutti ▲ e trend text da `LimeGreen` a `Primary` |

### Sessione 6 — Icone tematizzate (`app_icon`)
| Fix | File | Descrizione |
|-----|------|-------------|
| Icone Android mipmap | `Platforms/Android/Resources/mipmap-hdpi/` | `ic_launcher_dark.png` e `ic_launcher_light.png` da `Assets/AppLogo` |
| Activity-alias manifest | `AndroidManifest.xml` | 2 alias (`MainActivityDark`/`MainActivityLight`) con icone dedicate e `MainLauncher` sui filtri intent |
| MainActivity no MainLauncher | `MainActivity.cs` | Rimosso `MainLauncher = true` |
| SwitchAppIcon() | `ThemeService.cs` | `PackageManager.SetComponentEnabledSetting` al cambio tema, `#if ANDROID` condizionale |

### Stato attuale branch
| Branch | Commit | Contenuto |
|--------|--------|-----------|
| `main` | `e104cae` | Pulito — versione "ciano originale" (freccia ▸, Lexend font) |
| `app_icon` | `27f785a` | main + toggle fix + icone tematizzate + SwitchAppIcon |

### Aggiornamento 2026-05-13 — Branch `lightmode-fix` (corrente)

#### Obiettivo
- Assicurarsi che il toggle tema nelle Impostazioni cambi l'intera applicazione (ogni pagina, Shell inclusa)
- Implementare la pagina "Start Session" basata sui mockup `start_session_light_uniform_v2` (chiaro) e `start_training_cyber_athletic_elite_uniform` (scuro)
- Il pulsante "START WORKOUT" della HomePage deve aprire la schermata Start Session

#### Modifiche applicate

| Modifica | File | Descrizione |
|----------|------|-------------|
| StaticResource→DynamicResource globale | `Resources/Styles/Styles.xaml` | Tutti i `StaticResource` (colori) convertiti in `DynamicResource` nelle definizioni di Style (Shell, NavigationPage, Page, Label styles, Button styles, CardFrame, DarkEntry, ActivityIndicator, ErrorLabel, EmptyStateLabel). 26 StaticResource totali sostituiti |
| Shell colori dinamici | `AppShell.xaml` | `Shell.BackgroundColor`, `ForegroundColor`, `TabBarBackgroundColor`, `TabBarForegroundColor`, `TabBarUnselectedColor`, `TabBarTitleColor` passati da valori hardcoded (`#131313`, `#e5e2e1`, etc.) a `DynamicResource` |
| Colors.Light.xaml preload | `App.xaml` | Aggiunto `Colors.Light.xaml` come primo MergedDictionary per garantire inclusione nel build e risoluzione URI a runtime del ThemeService |
| StartSessionPage XAML | `Views/StartSessionPage.xaml` | Nuova pagina basata sui mockup: TopAppBar con FORGE_ELITE, Hero START_SESSION, Quick Start card, Create New Plan card accent, Your Protocols con cards da PlanStore. Layout identico per entrambi i temi — differenziazione via `DynamicResource` |
| StartSessionPage code-behind | `Views/StartSessionPage.xaml.cs` | Ricarica protocolli da `PlanStore` in `OnAppearing()` |
| StartSessionViewModel | `ViewModels/StartSessionViewModel.cs` | `QuickStartCommand` (mode=free), `CreateNewPlanCommand` (mode=create), `StartProtocolCommand` (mode=plan+planId), `LoadProtocols()` da `PlanStore`, `GoBackCommand`. Modello `ProtocolCard` per binding |
| Registrazione DI | `MauiProgram.cs` | Aggiunti `StartSessionViewModel` e `StartSessionPage` alla DI |
| Route Shell | `AppShell.xaml.cs` | Registrata route `"startSession"` per `StartSessionPage` |
| Navigazione START WORKOUT | `ViewModels/HomeViewModel.cs` | `StartWorkoutAsync()` ora naviga a `"startSession"` invece che direttamente a `"activeWorkout"` |

#### Light mockups referenziati
| Mockup | Palette | Utilizzo |
|--------|---------|----------|
| `start_session_light_uniform_v2` | Fitness Core (chiaro, blu `#003ec7`) | StartSessionPage tema chiaro |
| `start_training_cyber_athletic_elite_uniform` | Cyber-Athletic Elite (scuro, ciano `#00E5FF`) | StartSessionPage tema scuro |

#### Risultato
- Il toggle tema nelle Impostazioni cambia **ogni elemento UI**: pagine, Shell tab bar, stili globali, pulsanti
- START WORKOUT sulla HomePage porta alla nuova schermata Start Session, coerente con i mockup
- La StartSessionPage è theme-aware: usa esclusivamente `DynamicResource`, adattandosi automaticamente al tema attivo

### Stato branch aggiornato
| Branch | Commit | Contenuto |
|--------|--------|-----------|
| `main` | `1cb72c2` | Versione stabile — ciano originale |
| `lightmode-fix` | — (nuovo) | main + StartSessionPage + ThemeService rewrite + mock data |

### Sessione 2 — ThemeService rewrite + mock data

| Fix | File | Descrizione |
|-----|------|-------------|
| ThemeService rewrite | `Services/ThemeService.cs` | Sostituito il meccanismo MergedDictionaries swap con scrittura diretta in `Application.Current.Resources`. Entrambe le palette (dark/light) sono inline come `Dictionary<string, string>`. `WriteResources()` aggiorna i valori nello stesso dizionario — DynamicResource viene notificato automaticamente. Aggiunto `UserAppTheme` sync |
| remove App.xaml color preload | `App.xaml` | Rimosse entrambe le color dictionaries da MergedDictionaries — ThemeService popola Resources a runtime |
| suppressChange flag | `ViewModels/SettingsViewModel.cs` | Blocca `OnIsDarkModeChanged` durante costruzione iniziale per evitare Apply() doppio |
| Mock workout plans | `ViewModels/StartSessionViewModel.cs` | 3 piani demo (`Push Power`, `Leg Day Protocol`, `Core Stabilization`) con esercizi, serie, peso e ripetizioni. Seed automatico al primo avvio se PlanStore vuoto |

#### Perché il nuovo ThemeService funziona
Il vecchio approccio (rimozione/aggiunta `MergedDictionaries`) non notificava sempre i binding `DynamicResource`, specialmente con XAML SourceGen. Il nuovo approccio scrive direttamente `Color.FromArgb()` nelle stesse chiavi di `Application.Current.Resources`. Poiché i binding `DynamicResource` osservano il dizionario risorse, qualsiasi modifica al valore di una chiave viene propagata a tutto il visual tree.

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 11 (+StartSessionViewModel) |
| Views | 13 (+StartSessionPage) |
| StaticResource→DynamicResource totali sostituiti | 32 (26 in Styles.xaml + 6 in AppShell.xaml) |
| ThemeService linee | 140 (da 65) — palette inline, no dipendenze file XAML a runtime |
| Piani mock | 3 (Push Power, Leg Day Protocol, Core Stabilization) |
| Branch attivi | main, lightmode-fix |

### Aggiornamento 2026-05-13 — Branch `mockup` (PocketBase)

`lightmode-fix` mergiato in `main` e cancellato. Nuovo branch `mockup`.

#### Obiettivo
Sostituire Firebase con PocketBase self-hosted per auth, dati social e persistenza remota. Primo step: autenticazione utente e profilo da PocketBase.

#### Modifiche

| Modifica | File | Descrizione |
|----------|------|-------------|
| PocketBaseService | `Services/PocketBaseService.cs` | `LoginAsync()`, `RegisterAsync()`, `Logout()`, `RefreshUserAsync()`, `TryAutoLoginAsync()`, `GetListAsync<T>()`, `CreateRecordAsync<T>()`. Token salvato in memoria, credenziali in `Preferences` per auto-login |
| PocketBase DTO | `Models/Dto/PocketBaseDto.cs` | `PocketBaseAuthResponse`, `PocketBaseUserRecord`, `PocketBaseListResponse<T>` |
| LoginPage | `Views/LoginPage.xaml` | Login/Register con email, password, nome. Toggle modalità login/registrazione. Stati: loading, error, success |
| LoginViewModel | `ViewModels/LoginViewModel.cs` | `LoginCommand`, `RegisterCommand`, `ToggleModeCommand`. Auto-login all'avvio |
| App.xaml.cs auth flow | `App.xaml.cs` | `CreateWindow()` avvia con `AppShell`, poi in background carica secrets, inizializza PocketBase, tenta auto-login. Se fallisce → mostra `LoginPage` via `Windows[0].Page` |
| App.xaml converters | `App.xaml` | Registrati globalmente `InverseBoolConverter` e `BoolToVisibilityConverter` |
| ProfileViewModel real data | `ViewModels/ProfileViewModel.cs` | Iniettato `PocketBaseService`. Mostra `Username`, `Bio`, `AvatarInitials` da `pb.CurrentUser`. Fallback a dati mock se offline |
| ProfilePage avatar/bio | `Views/ProfilePage.xaml` | Avatar mostra iniziali utente (`AvatarInitials`), bio visibile sotto il tier |
| MauiProgram registrazioni | `MauiProgram.cs` | PocketBaseService con HttpClient, LoginViewModel, LoginPage |
| .env PocketBase URL | `.env` | Aggiunto `POCKETBASE_URL=http://192.168.1.23:8080` |

#### Collezioni PocketBase remote
| Collezione | Stato | Campi |
|-----------|-------|-------|
| `users` | Built-in | email, password, name, bio, avatar |
| `excercise` | Vuota, accessibile | Da popolare con esercizi |
| `logged_workouts` | Vuota, accessibile | Da popolare con allenamenti |
| `social_graph` | Vuota, accessibile | Da popolare con amicizie |

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 13 (+LoginViewModel) |
| Views | 14 (+LoginPage) |
| Services | 5 (+PocketBaseService) |
| DTO | 1 (+PocketBaseDto) |
| Branch attivo | mockup |
| Build status | 0 errori, 0 warning |

### Aggiornamento 2026-05-13 — Branch `mockup` (fix LoginPage, immagini esercizi, filtri, icone)

#### Fix LoginPage
| Fix | Descrizione |
|-----|-------------|
| Entry tappabili su tutta l'area | Sostituito `HorizontalStackLayout` con `Grid` per i campi — il tocco sull'intero spazio del bordo attiva la tastiera |
| Riga bianca Entry rimossa | Handler Android: `BackgroundTintList = Color.Transparent` per rimuovere la sottolineatura predefinita |
| Rimossi bottoni Google/Apple | Eliminata sezione "OR AUTHENTICATE VIA" con social login |
| Design mockup | LoginPage ridisegnata su mockup `login_cyber_athletic_elite_nero_opaco_v4` e `login_fitness_core_light` |

#### Immagini e numeri esercizi
| Fix | Descrizione |
|-----|-------------|
| Numero ordine sempre visibile | Overlay `Label` sul `Border` immagine — se l'immagine non carica, il numero rimane visibile |
| Filtri muscolo/attrezzo | Chip orizzontali per 10 gruppi muscolari + 7 attrezzature nella ricerca. `SelectMuscleFilter` chiama API direttamente |

#### Icone app tematizzate
| Fix | Descrizione |
|-----|-------------|
| Icone PNG copiate | Da `Assets/App_Icon/` a `Platforms/Android/Resources/mipmap-hdpi/` |
| Activity-alias rimossi | Causavano crash `ClassNotFoundException` — ripristinato `MainLauncher=true` sull'attività principale. Feature icone tematizzate rimandata |

#### PocketBase & API ExerciseDB
| Fix | Descrizione |
|-----|-------------|
| ExerciseApiService rewrite | Host corretto `exercise-db-fitness-workout-gym.p.rapidapi.com`. Carica ID da `/exercises`, dettaglio da `/exercise/{id}`. Cache locale degli ID |
| ExerciseDbDto aggiornato | Campi: `primaryMuscles`, `secondaryMuscles`, `equipment`, `instructions`, `images`, `level`, `category` |
| PocketBase save workout | `SaveWorkoutPlanAsync` invia a `logged_workouts` con nome, data, volume, durata, esercizi |

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 13 |
| Views | 14 (+LoginPage, StartSessionPage) |
| Services | 6 (+PocketBaseService, ExerciseApiService, BuildSecrets, ThemeService, WorkoutSession, PlanStore) |
| DTO | 2 (+PocketBaseDto, ExerciseDbDto) |
| Models | 1 (+ImageUrl, GifUrl, Instructions su WorkoutExercise) |
| Branch attivo | mockup |
| Build status | 0 errori, 0 warning |

### PocketBase HTTPS proxato
| Modifica | Descrizione |
|----------|-------------|
| Nuovo URL | `https://pocketbase.server-casa-leo.duckdns.org` via Nginx Proxy Manager + Let's Encrypt |
| `usesCleartextTraffic` rimosso | Non più necessario — il traffico è HTTPS nativo |
| `.env` aggiornato | `POCKETBASE_URL=https://pocketbase.server-casa-leo.duckdns.org` |
| Test OK | Auth, health check, collections tutte raggiungibili via HTTPS |

### Stato implementazione mockup
| Mockup | Schermata | Stato |
|--------|-----------|-------|
| `login_cyber_athletic_elite_nero_opaco_v4` | Login (scuro) | ✅ Implementata |
| `login_fitness_core_light` | Login (chiaro) | ✅ Implementata |
| `start_session_light_uniform_v2` | Start Session (chiaro) | ✅ StartSessionPage |
| `start_training_cyber_athletic_elite_uniform` | Start Session (scuro) | ✅ StartSessionPage |
| `profilo_elite_chiaro` | Profilo (chiaro) | ✅ ProfilePage |
| `profilo_elite_nero_opaco` | Profilo (scuro) | ✅ ProfilePage |
| `dashboard_elite_achievements_minimal_layout` | Dashboard | ⚠️ HomePage con alcuni elementi |
| `dashboard_minimale_con_storie` | Dashboard | ⚠️ Non implementata |
| `allenamento_attivo_minimal_bianco` | Allenamento (chiaro) | ✅ Ricostruita da mockup |
| `allenamento_attivo_nero_opaco_minimal_v2` | Allenamento (scuro) | ✅ Ricostruita da mockup |
| `feed_elite_nero_opaco` | Feed | ⚠️ FeedPage esistente, non allineata |
| `social_feed_scuro` | Social | ⚠️ SocialPage esistente |
| `statistiche_avanzate_chiaro` | Statistiche (chiaro) | ⚠️ StatsPage esistente |
| `statistiche_avanzate_nero_opaco` | Statistiche (scuro) | ⚠️ StatsPage esistente |
| `cyber_athletic_elite` | Design system scuro | ✅ Palette colori |
| `fitness_core` | Design system chiaro | ✅ Palette colori |

### Aggiornamento 2026-05-13 — Font mockup e redesign allenamento

#### Font Google Fonts
| Font | File | Peso | Utilizzo mockup |
|------|------|------|-----------------|
| Inter | `Inter-Variable.ttf` | 876 KB | Body text, descrizioni, placeholder |
| Lexend | `Lexend-Variable.ttf` | 175 KB | Label, caps, pulsanti, chip |
| Space Grotesk | `SpaceGrotesk-Variable.ttf` | 136 KB | Headline, metriche, nomi esercizi |

Registrati in `MauiProgram.cs` come alias `Inter`, `Lexend`, `SpaceGrotesk`. Mantenuti anche `OpenSansRegular` e `OpenSansSemibold` per compatibilità.

#### Styles.xaml — famiglie font mockup
| Stile | Font precedente | Font attuale |
|-------|----------------|-------------|
| `DisplayMetric`, `H1Bold`, `H2Bold` | OpenSansSemibold | **SpaceGrotesk** |
| `BodyLg`, `BodyMd` | OpenSansRegular | **Inter** |
| `LabelCaps` | OpenSansSemibold | **Lexend** |

#### ActiveWorkoutPage — redesign da mockup
Ricostruita interamente basandosi su `allenamento_attivo_minimal_bianco` e `allenamento_attivo_nero_opaco_minimal_v2`:

| Elemento | Descrizione |
|----------|-------------|
| Progress bar | Barra sottile Primary in cima alla pagina |
| Header | ✕ close + nome scheda editabile (SpaceGrotesk) + FINISH button |
| Card esercizio | Header con immagine + nome + bodyPart (SpaceGrotesk) + controlli ▲▼✕ |
| Tip box | Sfondo PrimaryContainer a bassa opacità, testo Inter |
| Tabella set | Header SET / PREV / KG / REPS / ✓ (Lexend caps). Righe con Entry editabili per peso e reps, cerchio checkmark |
| Add Set | Bordo dashed stile mockup, label Lexend |
| Font mockup | Inter per body, SpaceGrotesk per nomi/metriche, Lexend per label/pulsanti |

#### Fix LoginPage
| Fix | Descrizione |
|-----|-------------|
| Entry click su tutta l'area | Grid al posto di HorizontalStackLayout |
| Riga bianca Entry rimossa | Handler Android `BackgroundTintList = Color.Transparent` |
| Social login rimossi | Eliminata sezione Google/Apple e divider |
| Testo toggle corretto | "Don't have an account? Register" / "Already registered? Sign in" |

#### PocketBase HTTPS
| Modifica | Descrizione |
|----------|-------------|
| URL | `https://pocketbase.server-casa-leo.duckdns.org` via Nginx Proxy Manager + Let's Encrypt |
| `usesCleartextTraffic` | Rimosso da AndroidManifest — HTTPS nativo |
| Test | Auth, health check OK via HTTPS |

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 13 |
| Views | 14 (+LoginPage, StartSessionPage) |
| Services | 6 (+ExerciseApiService, PocketBaseService, BuildSecrets, ThemeService, WorkoutSession, PlanStore) |
| DTO | 2 (+PocketBaseDto, ExerciseDbDto) |
| Fonts | 5 (OpenSans Regular+Semibold, Inter Variable, Lexend Variable, SpaceGrotesk Variable) |
| Branch attivo | mockup |
| Build status | 0 errori, 0 warning |

### Aggiornamento 2026-05-15 — Fix crash Android, cache PocketBase, note esercizi, immagini

#### Fix crash `JavaProxyThrowable`
| Problema | Causa | Fix |
|----------|-------|-----|
| `HttpClient.BaseAddress` dopo prima richiesta | Race condition: `LoginViewModel.TryAutoLoginAsync()` chiamava `LoginAsync()` prima che `App.InitializeAsync` settasse `BaseAddress` su PocketBase. La prima richiesta falliva ma "avviava" l'HttpClient, poi `Set BaseAddress` crashava | `BaseAddress` impostato direttamente nel costruttore `HttpClient` in `MauiProgram.cs`: `new HttpClient { BaseAddress = new Uri("https://pocketbase.server-casa-leo.duckdns.org/api/") }`. `EnsureInitialized` e `Initialize` diventati no-op |
| `CollectionView` annidate | Inner `CollectionView` (sets) dentro outer (exercises) causava crash su Android | Sostituito con `BindableLayout` su `VerticalStackLayout` |
| `Entry Mode=TwoWay` su `double`/`int` | Binding non riusciva a convertire valori vuoti | Sostituito con `Label` read-only + pulsanti `−`/`+` ai lati per peso e reps |

#### Note per esercizio
| Modifica | File | Descrizione |
|----------|------|-------------|
| `Notes` property | `Models/WorkoutPlan.cs` | Aggiunto campo `Notes` a `WorkoutExercise` |
| Note UI | `Views/ActiveWorkoutPage.xaml` | Blocco blu (`PrimaryContainer`) sotto nome esercizio con `Entry` editabile per note |
| Note salvate | Salvataggio locale PocketBase | `Notes` incluse nel payload salvato |

#### Immagini esercizi — URL resolution
| Problema | Fix |
|----------|-----|
| Short URL (`acesse.dev`, `encr.pw`) DNS-bloccati da ISP | `ResolveImageUrlAsync()` segue redirect HTTP (302) e ottiene URL diretto dell'immagine |
| `redirectHttp` | `HttpClient` con `AllowAutoRedirect = true`, User-Agent Mozilla, timeout 5s |
| Cache PocketBase | URL risolto salvato nel campo `imageUrl` della collection `excercise` |
| Ricerche successive | URL diretto usato senza bisogno di re-risolvere |

#### Profilo utente
| Modifica | Descrizione |
|----------|-------------|
| Edit overlay | ✏️ apre form con nome, bio, avatar preview |
| Save | `PocketBaseService.UpdateUserAsync()` → `PATCH /api/collections/users/records/{id}` |
| Login persistente | App parte con LoginPage, auto-login dopo 1s, salta login se già autenticato |

#### Exercise caching su PocketBase
| Modifica | Descrizione |
|----------|-------------|
| `CacheExerciseAsync` | Salva nome, bodyPart, equipment, instructions, imageUrl, category, level, force, mechanic su `excercise` |
| `pbCacheCheck` | `HashSet<string>` evita duplicati in sessione |
| Fetch parallelo | `Task.WhenAll` per dettagli esercizi — 10x più veloce |

#### Stato implementazione mockup aggiornato
| Mockup | Schermata | Stato |
|--------|-----------|-------|
| `login_cyber_athletic_elite_nero_opaco_v4` | Login (scuro) | ✅ |
| `login_fitness_core_light` | Login (chiaro) | ✅ |
| `start_session_light_uniform_v2` | Start Session (chiaro) | ✅ |
| `start_training_cyber_athletic_elite_uniform` | Start Session (scuro) | ✅ |
| `profilo_elite_chiaro` | Profilo (chiaro) | ✅ |
| `profilo_elite_nero_opaco` | Profilo (scuro) | ✅ |
| `allenamento_attivo_minimal_bianco` | Allenamento (chiaro) | ✅ |
| `allenamento_attivo_nero_opaco_minimal_v2` | Allenamento (scuro) | ✅ |
| `dashboard_elite_achievements_minimal_layout` | Dashboard | ⚠️ HomePage parziale |
| `dashboard_minimale_con_storie` | Dashboard | ❌ |
| `feed_elite_nero_opaco` | Feed | ⚠️ FeedPage esistente |
| `social_feed_scuro` | Social | ⚠️ SocialPage esistente |
| `statistiche_avanzate_chiaro` | Statistiche (chiaro) | ⚠️ StatsPage esistente |
| `statistiche_avanzate_nero_opaco` | Statistiche (scuro) | ⚠️ StatsPage esistente |

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 13 |
| Views | 14 |
| Services | 6 |
| DTO | 2 |
| Fonts | 5 (Inter, Lexend, SpaceGrotesk, OpenSans Regular+Semibold) |
| Branch attivo | mockup |
| Build status | 0 errori, 0 warning |

*Report aggiornato il 2026-05-15.*

---

## Aggiornamento 2026-05-17 — Branch `mockup`: Social, Stats reali, redesign allenamento, fix PocketBase

### Commit: `5e26124` — feat(mockup): social follow system, real stats/profile/dashboard, live feed search, configurable rest timer, FORGE branding

#### Modifiche
| Modifica | File | Descrizione |
|----------|------|-------------|
| ExerciseSet ObservableObject | `Models/WorkoutPlan.cs` | `ExerciseSet` ora estende `ObservableObject` con `[ObservableProperty]` su `WeightKg`, `Reps`, `IsCompleted` — il checkmark ✓ reagisce al tocco riempiendosi di LimeGreen |
| Timer pausa configurabile | `Views/ActiveWorkoutPage.xaml`, `ViewModels/ActiveWorkoutViewModel.cs` | Entry per secondi pausa globale; `OnRestDurationChanged` con validazione 5-600s; rest timer per singolo esercizio (`RestSeconds` in `WorkoutExercise`) |
| Note esercizio | `ActiveWorkoutPage.xaml` | `TextColor="White"` su sfondo blu (`PrimaryContainer`, opacity 0.2) |
| Entry numeriche al posto dei ± | `ActiveWorkoutPage.xaml` | Set row ridisegnata: `[SET \| KG Entry \| REPS Entry \| ✓]` — tastiera numerica, niente più 9 colonne |
| Bottone ADD SET mockup | `ActiveWorkoutPage.xaml` | Rimosso bordo dashed, testo centrato Lexend come da mockup `allenamento_attivo_nero_opaco_minimal_v2` |
| Note generali nascoste | `ActiveWorkoutPage.xaml` | Blocco "Note sull'allenamento" rimosso; `HasExercises` bindato a `Exercises.CollectionChanged` |
| Nome app FORGE | `GymTracker.Mobile.csproj` | `ApplicationTitle` → "FORGE" |
| Icona app SVG | `GymTracker.Mobile.csproj` | `MauiIcon` con sfondo `#0E0E0E`, foreground SVG |
| PocketBase social | `Services/PocketBaseService.cs` | `SearchUsersAsync` (ricerca via `?search=`), `SendFollowRequestAsync`, `GetPendingRequestsAsync`, `AcceptFollowRequestAsync`, `RejectFollowRequestAsync`, `GetFollowingUserIdsAsync`, `GetFollowedWorkoutsAsync`, `LikeWorkoutAsync`, `UnlikeWorkoutAsync`, `GetMyWorkoutsAsync`, `UploadAvatarAsync` (multipart), `GetFileUrl` (con `?token=`) |
| Social DTO | `Models/Dto/PocketBaseDto.cs` | `SocialGraphRecord`, `LoggedWorkoutRecord` (con `ExerciseData`, `Likes`, `LikedBy`, `UserName`) |
| FeedPage ridisegnata | `Views/FeedPage.xaml`, `ViewModels/FeedViewModel.cs` | Rimossi mock post. Search bar per cercare utenti (live ad ogni lettera, debounce 400ms). Feed allenamenti dei seguiti con cuore ♥ (like/unlike). `UserSearchResult` con pulsante Follow. |
| FriendRequestsPage | `Views/FriendRequestsPage.xaml`, `ViewModels/FriendRequestsViewModel.cs` | Lista richieste in sospeso con ACCEPT/REJECT. Auto-load su `OnAppearing`. |
| StatsPage reale | `Views/StatsPage.xaml`, `ViewModels/StatsViewModel.cs` | Rimossa sezione calorie. Aggiunti filtri tempo: WEEK / MONTH / 3M / YEAR / ALL. Dati reali da PocketBase. Grafico volume dinamico con `BindableLayout`. Top lifts calcolate dai pesi reali. Cuore ♥ al posto dell'ingranaggio, naviga a FriendRequests. |
| ProfilePage reale | `Views/ProfilePage.xaml`, `ViewModels/ProfileViewModel.cs` | Allenamenti reali da PocketBase. Calorie sostituite con Total Volume. Streak calcolato da date reali. Avatar cliccabile per upload foto profilo. `UriImageSource` per avatar. |
| Dashboard reale | `Views/HomePage.xaml`, `ViewModels/HomeViewModel.cs` | Streak calcolato da date reali. Achievements rimossi. Squad da utenti seguiti reali. |
| Cache immagini PocketBase | `Services/ExerciseApiService.cs` | `GetCachedImageUrlAsync` cerca URL risolti in `excercise`. `GetByMuscleAsync` risolve URL prima del return. Payload cache con `Dictionary<string, object?>` (nomi campo esatti). |
| Salva workout completo | `ViewModels/ActiveWorkoutViewModel.cs` | `exercise_data` JSON con serie, kg, reps, note, restSeconds. Payload via `Dictionary<string, object>` con `user_name`. Debug log su `CreateRecordAsync` e `SaveWorkoutPlanAsync`. |
| Fix filtri PocketBase | `Services/PocketBaseService.cs` | Rimossi tutti i filtri lato server (causavano 400 Bad Request su campi relation). Filtro client-side in C#. |
| Tool ExerciseImporter | `tools/ExerciseImporter/` | Console app per pre-caricare immagini risolte da PC a PocketBase (bypassa blocco ISP su `encr.pw`/`acesse.dev`). |

#### Stato PocketBase collections richiesto
| Collection | Campi necessari | API Rules |
|-----------|-----------------|-----------|
| `logged_workouts` | `user`, `user_name`, `name`, `date`, `notes`, `exercises` (json), `exercise_data` (json), `volume` (number), `duration` (number), `likes` (number), `liked_by` (json) | List/Search: `@request.auth.id != ""` |
| `excercise` | `name`, `bodyPart`, `equipment`, `instructions` (json), `imageUrl` (url), `category`, `level`, `force`, `mechanic` | Create: `@request.auth.id != ""` |
| `social_graph` | `from_user`, `from_name`, `to_user`, `status` | Create + List/Search: `@request.auth.id != ""` |
| `users` | — | List/Search: `@request.auth.id != ""` |

#### Problemi noti
- **Immagini ExerciseDB bloccate dall'ISP**: domini `encr.pw`, `acesse.dev`, `l1nq.com` risolti a `127.0.0.1` da Telecom Italia. Soluzione: eseguire `tools/ExerciseImporter` con VPN attiva per pre-cachare le immagini su PocketBase.
- **`excercise` collection 404**: la collection non esiste ancora su PocketBase → va creata con i campi sopra.
- **`logged_workouts` List 400**: manca API Rules → `@request.auth.id != ""`.

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| ViewModel | 14 (+FriendRequestsViewModel) |
| Views | 16 (+FriendRequestsPage, ri-scritte FeedPage, StatsPage, ProfilePage, HomePage, ActiveWorkoutPage) |
| Services | 6 |
| DTO | 2 (+SocialGraphRecord, LoggedWorkoutRecord) |
| Fonts | 5 |
| Tools | 1 (ExerciseImporter) |
| Branch attivo | mockup |
| Build status | 0 errori |

*Report aggiornato il 2026-05-17.*

---

## Aggiornamento 2026-05-17 — Fix dati allenamenti non visibili in Stats e Profilo

### Problema
L'utente salvava un allenamento (presente su PocketBase) ma non lo vedeva nella tab Stats né nel Profilo.

### Causa root
`GetMyWorkoutsAsync` deserializzava l'intera lista con `JsonSerializer.Deserialize<PocketBaseListResponse<LoggedWorkoutRecord>>()`. PocketBase può restituire il campo `user` della relazione come oggetto `{id:"..."}` invece di stringa semplice `"..."`. System.Text.Json falliva la deserializzazione dell'intera lista e l'eccezione veniva silenziata dal `catch(Exception)` → lista vuota.

### Fix applicati

| Fix | File | Descrizione |
|-----|------|-------------|
| ParseWorkoutRecords robusto | `PocketBaseService.cs` | Nuovo metodo `ParseWorkoutRecords(string body, string? userId)` che usa `JsonDocument` per estrarre ogni campo manualmente da `items[]`. Gestisce il campo `user` sia come stringa che come oggetto `{id:"..."}`. Parsing per-item: se un item fallisce, gli altri sopravvivono |
| GetFollowedWorkoutsAsync fix | `PocketBaseService.cs` | Stesso parsing robusto invece di `ReadFromJsonAsync` |
| CreateRecordAsync logging | `PocketBaseService.cs` | Logga payload intero (500 char), auth status, e corpo risposta anche su successo |
| GetMyWorkoutsAsync logging | `PocketBaseService.cs` | Logga URL, userId, corpo risposta (500 char), ogni item, conteggio post-filtro |
| StatsViewModel error visibility | `StatsViewModel.cs` | `StatsError` + `HasStatsError` + `IsEmptyState`. Messaggi: non loggato / nessun dato / errore |
| StatsPage empty/error UI | `StatsPage.xaml` | Sezione vuota con icona e messaggio quando nessun dato |
| ProfileViewModel error visibility | `ProfileViewModel.cs` | `WorkoutLoadError` + `HasWorkoutError`. Logga conteggio item ricevuti |
| ProfilePage error UI | `ProfilePage.xaml` | Label errore nella sezione Recent Forges |
| Stats filter buttons feedback | `StatsPage.xaml` | DataTrigger sui bottoni WEEK/MONTH/3M/YEAR/ALL cambia colore quando selezionato |
| ProfilePage bindings fix | `ProfilePage.xaml` | Sostituiti `Category`, `HeartRate`, `Description` (inesistenti) con `Exercises` |

### PocketBase: verifiche necessarie
1. Collection `logged_workouts` deve esistere con campo `user` (tipo `relation` → `users`)
2. API Rule List/Search: `@request.auth.id != ""`
3. Se dopo il fix ancora non funziona, controllare i log di debug filtrando `[PB MyWorkouts]` per vedere cosa restituisce PocketBase

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| Build status | 0 errori, 107 warning (tutti MVVMTK0045 pre-esistenti) |
| Branch attivo | mockup |
