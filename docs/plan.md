# Piano di Progetto - FORGE

> Stato corrente al 13 ottobre 2026: beta in preparazione sul branch experiments. Le sezioni iniziali conservano la pianificazione storica; per implementazione e limiti attuali consulta [architettura](architecture.md), [preparazione beta](release-readiness.md) e l’aggiornamento finale di questo documento. Il catalogo corrente usa ExerciseDB OSS, non RapidAPI; SQLite e SecureStorage sostituiscono le precedenti ipotesi di persistenza.

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
- PocketBase self-hosted (`https://YOUR_SERVER.duckdns.org`)
- Google Fonts (Inter, Lexend, Space Grotesk — inclusi nell'app)

## 2. Sequenza delle iterazioni

| Iterazione | Obiettivo verificabile | Dipendenze | Rischio | Stato |
| --- | --- | --- | --- | --- |
| IT-01 | Bootstrap MAUI + Shell 3-tab + Design System + ThemeService | Nessuna | Basso | ✅ Completata |
| IT-02/03/08 | Catalogo esercizi + Allenamento + Piani salvabili | IT-01 | Medio | ✅ Completata |
| IT-05/06/07 | PocketBase Auth + Social + Dashboard + Stats + Profilo | IT-02/03/08 | Alto | ✅ Completata |
| IT-FEAT-01 | SQLite offline (DatabaseService, LocalWorkout, CachedExercise) | IT-01 | Medio | ✅ Completata |
| IT-FEAT-02 | PlanStore → PlanService (SQLite + auto-migration) | IT-FEAT-01 | Basso | ✅ Completata |
| IT-FEAT-03 | Rimozione 5 ViewModel stub + 5 Page stub | IT-07 | Basso | ✅ Completata |
| IT-FEAT-04 | Wger API → ExerciseDB v1 free API | IT-02 | Alto | ✅ Completata |
| IT-FEAT-05 | CSV import/export in Settings | IT-01 | Basso | ✅ Completata |
| IT-FEAT-06 | WorkoutDetailPage + foto progresso in allenamento | IT-FEAT-01, IT-07 | Medio | ✅ Completata |
| IT-04 | Tracking peso corporeo e misure | IT-01 | Basso | ❌ Posticipata |

## 3. Dettaglio iterazioni

### IT-01 — Bootstrap MAUI e Design System ✅

**Obiettivo verificabile**

Progetto MAUI compilabile su Android con Shell navigation a 3 tab e doppio tema Stitch.

**In scope**

- Creazione progetto `src/Forge/`
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
| Dettaglio workout | ✅ | WorkoutDetailPage: serie, kg, reps, note, foto |
| Foto progresso | ✅ | Camera/gallery in ActiveWorkout, salvate nel workout |
| Achievement system | ✅ | 48 badge, tracking automatico, pagina dedicata, vetrina profilo |
| Offline SQLite | ✅ | DatabaseService: local_workouts, cached_exercises, saved_plans, achievements |
| CSV import/export | ✅ | Backup/ripristino dati in Settings |
| Minimize workout | ✅ | Allenamento non finito salvato come draft, non conta per stats |
| Push notifications | ⚠️ | FCM Android e worker integrati; progetto Firebase e prova reale ancora necessari |
| Body tracking | ❌ | IT-04 non implementata |
| Leaderboard | ❌ | Sostituita da feed + like |
| Confronto diretto | ❌ | Post-MVP |
| Test automatici | ✅ | 36 test xUnit (Converter, Model, DTO, Message)

## 5. Prossimi passi

Priorità aggiornate al 11 ottobre 2026: migrazione PocketBase per privacy e integrità social, prova Android dei profili/badge/recuperi e salvataggi, poi rilasci della nuova `ROADMAP.md`. Implementazione su develop; baseline main `2159d4c`. Test automatici e build documentati in `docs/test-matrix.md`. Nessun deployment o push effettuato. Le proposte storiche sotto sono subordinate a queste priorità.

2. Configurare Nginx per bloccare admin panel `/_/` dall'esterno
3. Valutare se implementare IT-04 (body tracking) in una nuova iterazione
4. Aggiungere leaderboard settimanale tra amici
5. Implementare confronto diretto statistiche tra amici
6. Configurare Firebase e verificare le push Android con app chiusa (`docs/push-notifications.md`)
7. Pubblicazione su Google Play Store

Aggiornamento profili: pagina completa follower/seguiti, foto, allenamenti pubblici e conferma unfollow implementati. Migrazioni 1791676800/1791676900 applicate nel LXC dall’utente; nuova migrazione 1791677000 privacy elenchi pronta e da applicare.

Aggiornamento 12 ottobre: il proprietario ha applicato anche 1791677000. Baseline develop salvata nel commit `305db83`. Implementati calendario per periodo, tutti i badge nel profilo, livello da achievement, eliminazione account, foto ottimizzate e dettagli aggiornati, serie reversibili e progressione esercizi. Nuova migrazione 1791763200 pronta da applicare; Firebase non ancora creato. Cloudflare/domino rimandati su richiesta dell’utente. Verifiche mirate, senza ripetere la suite completa.

## experiments — 12 ottobre 2026
Interfaccia workout, ricerca e pannelli coerenti con i due temi; timer nativo Android. Implementazione completata; build Android riuscita (0 errori, 3 avvisi preesistenti dei binding Firebase) e APK dedicato artifacts/FORGE-experiments.apk. Verifica visiva e recupero in background sul telefono da eseguire.

### Correzioni experiments del 12 ottobre
Refresh locale immediato, filtri muscoli, calendario virtualizzato, consigli rotanti e recupero Android riconoscibile implementati. Sei regressioni mirate superate; APK in preparazione. Verifica sul telefono ancora necessaria.

Build finale Android riuscita: 0 errori e 3 avvisi preesistenti Firebase. APK artifacts/FORGE-experiments.apk aggiornato, tono incluso verificato nel pacchetto. Sei regressioni mirate passate; visualizzazione e suono sul telefono restano da confermare.

### Schede e tipi di serie — experiments
Alla fine di una sessione da scheda, le modifiche a nome, esercizi/ordine, note, recuperi e serie chiedono se aggiornare la scheda o mantenere l’originale. Nessuna modifica automatica durante la sessione; salvare il recupero cambia solo la sessione fino alla scelta finale. La scheda aggiornata mantiene ID e data di creazione e azzera i flag di completamento. Numero SET cliccabile: normale, W warm-up giallo, D drop set viola, F cedimento rosso; tipi persistiti in schede, bozze e storico/dettaglio. Tre test mirati passati (firma, rinomina/esercizi aggiunti, copia senza mutazione e tipi). Prove popup/scelte su telefono ancora da eseguire. Nessun push richiesto.

### Note automatiche — 13 ottobre 2026, experiments
Le note della scheda e degli esercizi già presenti vengono salvate dopo una breve pausa nella digitazione (400 ms), con salvataggio atteso anche quando si minimizza o termina la sessione. Vengono ricaricate alla successiva apertura della scheda. Le sole modifiche alle note non aprono il popup finale; scegliere di mantenere la scheda originale conserva comunque le note. Il salvataggio delle note aggiorna soltanto quelle, senza introdurre esercizi nuovi o modificare nome, ordine, serie e recuperi. Le note di esercizi aggiunti o sostituiti restano nella sessione e nella scheda se viene approvato l’aggiornamento. Nessuna migrazione server necessaria.

Verifiche: cinque regressioni mirate superate, inclusi round trip SQLite, cancellazione delle note, isolamento dell’account e firma del popup. Da verificare sul dispositivo: modificare soltanto le note e terminare senza popup; riaprire la scheda e ritrovarle; modificare anche il nome e scegliere di mantenere l’originale, poi verificare che il nome resti invariato e le note aggiornate.

## Preparazione beta e community — 13 ottobre 2026, experiments

Server FORGE predefinito mantenuto per la beta; un server personale crea una community distinta. Non sono implementate federazione o migrazione automatica degli account. Nel feed vuoto, il messaggio “Sei il primo nella tua community” appare solo se il server conferma che non ci sono altri utenti visibili; se non è verificabile viene usato un messaggio generico. “Invita un amico” apre la condivisione nativa con link al download sul sito e istruzioni per raggiungere il server corrente, senza token né accesso automatico.

README riscritto con origine scolastica e uso dell’AI, funzioni effettive, dati locali/remoti, limiti, installazione e segnalazioni. Sito nei due temi e italiano/inglese aggiornato mantenendo lo stile; roadmap ridotta a beta/stabilità, feedback, idee future. Confronto con Hevy, Strong e wger basato sulle loro fonti ufficiali, senza affermare superiorità di prestazioni o maturità.

Compose costruisce il binario PocketBase ufficiale della versione scelta; setup esplicito e porta loopback, hook/quattro migrazioni con backup, worker push facoltativo e archivio server completo. Corrette directory dei superuser LXC/Docker, timeout degli installer e messaggi che esponevano la password. Workflow di release prepara una bozza firmata usando secret di produzione, senza pubblicazione automatica.

Verifiche di questa fase: due test invito e tre casi di conteggio community passati; build APK Android riuscita con zero errori e tre avvisi preesistenti Firebase; build statica Next.js e TypeScript riuscita; configurazione Compose, sintassi Bash e PowerShell controllate. Nessuna suite completa ripetuta. Prova di nuovo backend Docker/LXC non eseguita (daemon Docker locale non disponibile); consegna push reale e nuove funzioni sul telefono ancora da verificare. Anteprima browser locale non raggiungibile, quindi nessuna verifica visiva interattiva del sito dichiarata.

Stato: file e APK preparati su experiments, nessun nuovo commit/push, pubblicazione del sito o modifica del server live. Adatta a una piccola beta con feedback dopo i controlli finali in docs/release-readiness.md; non ancora dichiarata pronta per un servizio commerciale per palestre.

## Release 0.8.5 beta — autorizzazione e preparazione, 13 ottobre 2026

Il proprietario ha autorizzato la promozione di experiments a main, il push e la prerelease v0.8.5-beta. Aggiunti README con gamification effettiva (100 XP per badge, un livello ogni 500 XP, criteri non implementati dichiarati), schermate reali dell’archivio identificate come precedenti ad alcune UI, e sezioni equivalenti del sito in entrambi i temi e le lingue. Versione app 0.8.5-beta, versionCode 85. Sito Next.js compilato con TypeScript senza errori.

Generata chiave stabile di firma con backup privato ignorato da Git e accesso Windows limitato all’utente. Dopo un blocco della revisione automatica, il proprietario ha autorizzato esplicitamente payload e destinazione dei cinque secret di firma/Firebase Android su GitHub Actions. Nessuna chiave Firebase Admin del server letta o caricata. Le firme Debug di 0.8.0 e delle prove locali differiscono; avviso di backup/reinstallazione aggiunto a README e release notes.

Il monitor server ora produce solo un riepilogo health e non modifica continuamente README/main. Workflow di release manuale prepara una bozza firmata; pubblicazione attuale autorizzata dal proprietario. Nessuna nuova suite locale completa ripetuta. Conferma pubblicazione, firma dell’APK finale e stato CI da registrare dopo le operazioni.
