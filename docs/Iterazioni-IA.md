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

### Metriche aggiornate
| Metrica | Valore |
|---------|--------|
| File C# totali (src) | 18 (+Converters) |
| ViewModel | 10 |
| Views | 12 |
| Services | 4 |
| Models | 1 |
| Converters | 3 |
| NuGet packages | 4 (+sqlite-net-pcl) |
| Branch creati e mergiati | fix, light-mode, light_button_fix |
| Branch attivi | main, app_icon |
| Build status | 0 errori, 0 warning |
