# FORGE — Diario di Sviluppo

> Stato corrente al 13 ottobre 2026: beta in preparazione sul branch experiments. Le sezioni iniziali conservano la pianificazione storica; per implementazione e limiti attuali consulta [architettura](architecture.md), [preparazione beta](release-readiness.md) e l’aggiornamento finale di questo documento. Il catalogo corrente usa ExerciseDB OSS, non RapidAPI; SQLite e SecureStorage sostituiscono le precedenti ipotesi di persistenza.

> **Trasforma ogni ripetizione in progresso. Sfida i tuoi amici. Supera i tuoi limiti.**

Benvenuti nel diario di sviluppo di FORGE, l'app .NET MAUI Android-first per il tracking degli allenamenti in palestra con funzionalità social. Questo documento racconta l'intero percorso: dalle specifiche iniziali al prodotto attuale, iterazione per iterazione, decisione per decisione.

---

## 📋 Indice

1. [Visione e Contesto](#1-visione-e-contesto)
2. [Cronologia delle Iterazioni](#2-cronologia-delle-iterazioni)
3. [Architettura e Stack Tecnologico](#3-architettura-e-stack-tecnologico)
4. [Problemi Risolti e Lezioni Apprese](#4-problemi-risolti-e-lezioni-apprese)
5. [Stato Attuale del Prodotto](#5-stato-attuale-del-prodotto)
6. [Sicurezza e Hardening](#6-sicurezza-e-hardening)
7. [Prossimi Passi](#7-prossimi-passi)

---

## 1. Visione e Contesto

### Il Problema

Chi va in palestra affronta tre ostacoli principali:

| Problema | Impatto |
|----------|---------|
| Non ricorda i pesi usati la volta precedente | Progresso rallentato, nessuna progressione strutturata |
| Non sa quali esercizi fare per un dato muscolo | Allenamenti improvvisati, risultati mediocri |
| Si allena da solo e perde motivazione | Drop-out dopo poche settimane |

### La Soluzione

FORGE unisce **tracking**, **catalogo esercizi** e **social** in un'unica app:
- 1.500+ esercizi con immagini GIF, ricerca e filtri per muscolo/attrezzatura
- Registrazione allenamenti con serie, ripetizioni, peso e rest timer
- Feed sociale: segui amici, metti like, competi con streak settimanali
- Statistiche di progresso con grafici, top lifts e calendario
- Achievement system con 48 badge da sbloccare

### Utenti Target

- Principianti che iniziano la palestra e cercano struttura
- Appassionati di fitness che vogliono tracciare i progressi
- Gruppi di amici che vogliono motivarsi a vicenda

### Vincoli Tecnici

- **Framework**: .NET MAUI 10, Android-first (iOS opzionale)
- **Architettura**: MVVM con CommunityToolkit.Mvvm
- **Navigazione**: Shell (3 tab: Dashboard, Feed, Stats)
- **Backend**: PocketBase self-hosted (auth, social graph, workout storage)
- **Persistenza**: SQLite locale + PocketBase remoto
- **Metodo**: Man-in-the-Loop con AI supervisionata

---

## 2. Cronologia delle Iterazioni

### IT-01 — Bootstrap MAUI e Design System ✅
**Data**: Inizio progetto — **Branch**: `main`

La fondazione. Abbiamo creato il progetto .NET MAUI compilabile su Android con Shell navigation a 3 tab (Dashboard, Feed, Stats), un sistema di temi runtime (chiaro/scuro) e i font Google Fonts (Inter, Lexend, Space Grotesk).

**Cosa abbiamo costruito:**
- Progetto `src/Forge/` con struttura MVVM
- AppShell con TabBar e 7 route di dettaglio
- `ThemeService`: palette dark ("Cyber-Athletic Elite", ciano #00E5FF) e light ("Fitness Core", blu #003ec7)
- `BaseViewModel` con stati UI espliciti: `IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState`
- `BuildSecrets`: iniezione variabili d'ambiente da `.env` a build-time
- Dependency injection via `MauiProgram.cs`

**Decisione chiave**: 3 tab invece dei 5 pianificati — UI più pulita, Catalogo e Profilo integrati come route di dettaglio.

---

### IT-02/03/08 — Catalogo Esercizi, Allenamento e Piani ✅
**Data**: Metà maggio 2026 — **Branch**: `mockup`

Il cuore dell'app. Abbiamo integrato l'API ExerciseDB per il catalogo esercizi, costruito il flusso di allenamento completo e implementato i piani di allenamento salvabili.

**Cosa abbiamo costruito:**
- `ExerciseApiService`: client HTTP per ExerciseDB (RapidAPI) con cache su PocketBase
- Ricerca esercizi per nome, filtro per gruppo muscolare e attrezzatura (chip orizzontali)
- `ActiveWorkoutPage`: interfaccia allenamento con tabella set (kg × reps, checkmark ✓), rest timer configurabile, note per esercizio
- `StartSessionPage`: Quick Start, Create New Plan, Your Protocols
- Salvataggio allenamenti su PocketBase (`logged_workouts`)
- 3 piani demo: Push Power, Leg Day Protocol, Core Stabilization

**Problema risolto**: I domini short URL di ExerciseDB (`encr.pw`, `acesse.dev`) erano bloccati dagli ISP italiani (Telecom Italia li risolveva a 127.0.0.1). Abbiamo implementato la risoluzione URL via redirect HTTP e la cache su PocketBase.

**Problema risolto**: `CollectionView` annidate (esercizi dentro esercizi) causavano crash su Android. Sostituite con `BindableLayout` su `VerticalStackLayout`.

---

### IT-05/06/07 — Auth PocketBase, Social, Dashboard, Stats e Profilo ✅
**Data**: Fine maggio 2026 — **Branch**: `mockup`

La dimensione sociale. Abbiamo sostituito Firebase con PocketBase self-hosted per autenticazione e dati social, costruito il feed, le statistiche e il profilo utente.

**Cosa abbiamo costruito:**
- `PocketBaseService` con 20+ metodi: auth, CRUD, social, file, like notifications
- HTTPS via Nginx Proxy Manager + Let's Encrypt su DuckDNS
- `LoginPage`: registrazione e login con auto-login (SecureStorage per password)
- `FeedPage`: ricerca utenti live (debounce 400ms), follow/unfollow, feed allenamenti con like ♥
- `StatsPage`: grafico volume a barre, top lifts, calendario mensile, filtri temporali (WEEK/MONTH/3M/YEAR/ALL)
- `HomePage`: streak settimanale, squad activity, today card, START WORKOUT
- `ProfilePage`: avatar PocketBase (upload), bio, stats grid, recent forges, edit overlay
- `FriendRequestsPage`: richieste follow + notifiche like aggregate

**Problema risolto**: `HttpClient.BaseAddress` dopo prima richiesta crashava l'app (`JavaProxyThrowable`). Causa: race condition tra `LoginViewModel.TryAutoLoginAsync()` e `App.InitializeAsync`. Fix: `BaseAddress` impostato direttamente nel costruttore `HttpClient` in `MauiProgram.cs`.

**Problema risolto**: PocketBase restituiva il campo `user` come oggetto `{id:"..."}` invece di stringa. `ReadFromJsonAsync<LoggedWorkoutRecord>` falliva silenziosamente. Fix: parsing manuale con `JsonDocument` per ogni item.

**Decisione architetturale**: Streak calcolata come settimane consecutive (lunedì-domenica) con tolleranza 7 giorni, invece che giorni. Una streak giornaliera è troppo fragile (basta un giorno di riposo per azzerarla).

---

### IT-VULN-01 — Sicurezza e Robustezza ✅
**Data**: Inizio giugno 2026 — **Branch**: `develop`

Abbiamo fatto un audit di sicurezza completo e applicato hardening su tutta l'app.

**Fix applicati:**
- Password: `SecureStorage` (Android Keystore) con fallback rimosso da `Preferences`
- HTTP: `IHttpClientFactory` con 3 client named (pocketbase, exercisedbv1, redirect-resolver)
- Token JWT: rimosso da URL query parameter (appariva nei log del server)
- `BuildSecrets`: caricato in `App.CreateWindow()` prima di mostrare la UI
- Debug logging: ridotto con `#if DEBUG`, massimo 50 caratteri nei log

---

### IT-BUG-01 — Pulizia del Codice ✅
**Data**: Inizio giugno 2026 — **Branch**: `develop`

Abbiamo corretto 22 bug e anti-pattern identificati dall'audit automatico.

**Fix applicati:**
- 22 blocchi `catch(Exception)` vuoti → `Debug.WriteLine` con messaggio descrittivo
- 9 chiamate fire-and-forget → `ContinueWith(TaskContinuationOptions.OnlyOnFaulted)`
- Race condition startup: `BuildSecrets.LoadAsync()` chiamato prima di `LoginPage`
- Calendar `IsToday`: refresh a mezzanotte per il cambio giorno
- ExerciseDB cache: gestione errori nei catch block

---

### IT-FEAT-01 — Offline SQLite + Sync ✅
**Data**: Metà giugno 2026 — **Branch**: `excercise-db`

Abbiamo aggiunto la persistenza offline con SQLite e la sincronizzazione automatica quando la connessione torna disponibile.

**Cosa abbiamo costruito:**
- `DatabaseService`: 4 tabelle SQLite (`local_workouts`, `cached_exercises`, `saved_plans`, `achievements`)
- `LocalWorkout`, `CachedExercise`, `SavedPlan`: modelli SQLite con indici
- `ConnectivityService`: monitoraggio stato connessione
- `SyncService`: auto-push allenamenti pendenti quando online
- Migrazione `PlanStore` da `Preferences` (JSON) a `PlanService` (SQLite) con auto-migrazione

---

### IT-FEAT-02 — Rimozione Codice Morto ✅
**Data**: Metà giugno 2026 — **Branch**: `excercise-db`

Abbiamo rimosso 5 ViewModel stub e 5 Page stub che erano placeholder mai implementati:
`DashboardViewModel`, `CatalogViewModel`, `WorkoutViewModel`, `SocialViewModel`, `NotificationsViewModel`
e le loro pagine corrispondenti.

---

### IT-FEAT-03 — ExerciseDB v1 Free API ✅
**Data**: Metà giugno 2026 — **Branch**: `excercise-db`

Abbiamo migrato dall'API ExerciseDB a pagamento (RapidAPI) all'API gratuita v1 (`oss.exercisedb.dev`).

**Cosa è cambiato:**
- **Nessuna API key** richiesta (eliminata la chiave RapidAPI dal progetto)
- **1.500 esercizi** con GIF URLs diretti (`static.exercisedb.dev`) — niente più short URL bloccati
- **Ricerca server-side** via `?name=` e cache SQLite locale
- **Filtri client-side** su cache SQLite (l'API gratuita non supporta filtri lato server)
- **Paginazione cursor** anche se limitata (la versione gratuita ha limitazioni)
- Wger API completamente rimossa dal codice e dalla cache SQLite

---

### IT-FEAT-04 — CSV Import/Export ✅
**Data**: Metà giugno 2026 — **Branch**: `excercise-db`

Abbiamo aggiunto la funzionalità di import/export dati in formato CSV per backup e portabilità.

**Cosa abbiamo costruito:**
- `CsvExportService`: esporta allenamenti in CSV con `Date,WorkoutName,ExerciseName,Sets,Reps,WeightKg,Notes`
- `CsvImportService`: importa dati CSV validando il formato (supporta colonne in italiano e inglese)
- Integrazione in `SettingsPage` con `FilePicker` (Android) e `Share` per l'export

---

### IT-FEAT-05 — Workout Detail Page + Foto Progresso ✅
**Data**: 17 giugno 2026 — **Branch**: `feature/test-import-fcm`

Abbiamo creato la pagina di dettaglio allenamento e aggiunto la possibilità di scattare foto durante il workout.

**Cosa abbiamo costruito:**
- `WorkoutDetailPage`: visualizzazione completa di ogni allenamento passato con tutti gli esercizi, serie (kg × reps, checkmark ✓), note, foto e like count
- Navigazione: tap su workout in Profilo e Feed → apre il dettaglio
- Foto progresso: pulsanti **Scatta** (fotocamera) e **Galleria** in ActiveWorkoutPage
- Max 5 foto per allenamento, salvate come base64 nel record PocketBase e SQLite
- Visualizzazione foto con overlay fullscreen nel dettaglio

**Bug fix correlati:**
- `WorkoutDetailPage` non aveva `BindingContext` → pagina sempre vuota (fix: costruttore riceve ViewModel)
- Bottone ✕ nell'overlay foto non funzionava (fix: `InputTransparent="True"`)
- Label "View All" in Profilo sembrava cliccabile ma non faceva nulla (fix: rimossa)

---

### IT-FEAT-06 — Achievement System (48 Badge) ✅
**Data**: 17 giugno 2026 — **Branch**: `feature/achievements-fix`

Abbiamo implementato un sistema di achievement gamificato per aumentare l'engagement e la motivazione.

**Cosa abbiamo costruito:**
- **48 achievement** in 6 categorie: Costanza (📅), Forza (💪), Orari (⏰), Varietà (🧬), Social (🦋), Elite (👑)
- `AchievementService`: tracking automatico — ogni workout salvato controlla volume, streak, orari, varietà esercizi
- **Card su HomePage**: contatore sbloccati + progress bar globale + "PROSSIMO" achievement (quello più vicino al completamento, con icona, nome, descrizione e mini progress bar)
- **AchievementsPage**: lista completa filtrata per categoria, progress bar per ogni achievement, sezione "Sbloccati di recente"
- **Vetrina profilo**: scroll orizzontale con tutti i badge sbloccati, visibile agli amici
- **38 badge Canva**: icone personalizzate create con Canva Pro (200×200px, stile navy+ciano)

**Bug fix correlati:**
- 38 immagini achievement rinominati in lowercase (MAUI non accetta uppercase nei nomi file)
- Git tracciava ancora i nomi uppercase (fix: `git rm --cached` + `git add`)
- Aggiunto `MauiImage` glob per la sottocartella `achievements/`

---

### IT-FEAT-07 — Minimize Workout (Draft) ✅
**Data**: 18 giugno 2026 — **Branch**: `feature/achievements-fix`

Abbiamo risolto un problema UX critico: chiudere un allenamento senza finire lo salvava comunque, falsando le statistiche.

**Cosa abbiamo costruito:**
- ✕ (chiudi): salva il workout come **draft** in SQLite (`IsDraft=true`) — NON conta per achievement/streak/stats
- **FINISH**: cancella il draft e salva definitivamente su PocketBase
- **Riapri Quick Start**: carica automaticamente l'ultimo draft con tutti gli esercizi, serie, foto
- Massimo 1 draft attivo: i vecchi vengono cancellati quando ne salvi uno nuovo
- Achievement calcolati solo su workout non-draft (`!w.IsDraft`)

---

### IT-SEC-01 — Security Audit e Hardening ✅
**Data**: 18 giugno 2026 — **Branch**: `feature/achievements-fix`

Abbiamo condotto un audit di sicurezza completo (18 vulnerabilità identificate) e applicato fix critici.

**Fix applicati nel codice:**
- Rimossa `EXERCISEDB_API_KEY` da `.env`, `.env.example` e `forge.env` (chiave inutilizzata embedded nell'APK)
- Rimosso fallback URL hardcoded → eccezione se non configurato
- Password: rimosso fallback `Preferences` (solo `SecureStorage`)
- Token JWT: rimosso da URL query parameter (`?token=...`)
- `android:allowBackup=false` (blocca backup Google Drive dei dati sensibili)
- `Logout()`: pulisce anche vecchie password in `Preferences`

- API Rules PocketBase con row-level ownership
- Nginx: bloccare admin panel `/_/` dall'esterno
- Rate limiting per endpoint auth
- Security headers (HSTS, X-Content-Type-Options, X-Frame-Options)

---

### IT-SEC-02 — Secondo Audit e Fix Funzionali ✅
**Data**: 18 giugno 2026 — **Branch**: `feature/achievements-fix`

Secondo audit di sicurezza approfondito con focus su bug funzionali (avatar/achievement non visibili) e hardening backend.

**Fix applicati nel codice:**

| # | Severità | Fix | File |
|---|----------|-----|------|
| 1 | CRIT | `GetFileUrl()` ora include `?token=` per autenticare le richieste file PocketBase | `PocketBaseService.cs:39-45` |
| 2 | CRIT | Icone achievement: rimosso prefisso `achievements/` per compatibilità MAUI | `Achievement.cs` (48 righe) |
| 3 | HIGH | `.env` copiato nell'APK solo in Debug, non in Release | `Forge.csproj:65` |
| 4 | HIGH | `TryAutoLoginAsync`: migrazione Preferences→SecureStorage one-shot, poi solo SecureStorage | `PocketBaseService.cs:356-385` |
| 5 | HIGH | Foto workout: limite raw ridotto a 3MB per evitare overflow base64 su PocketBase | `ActiveWorkoutViewModel.cs:127-138` |
| 6 | MEDIUM | Auto-refresh token JWT su risposta 401 in `GetFollowedWorkoutsAsync` e `TryFetchWorkoutsAsync` | `PocketBaseService.cs` |
| 7 | MEDIUM | Ricerca utenti con CancellationToken per prevenire race condition | `FeedViewModel.cs:56-110` |
| 8 | MEDIUM | Ricerca esercizi con CancellationToken per cancellare richieste obsolete | `ActiveWorkoutViewModel.cs:149-167` |
| 9 | MEDIUM | CSV import: validazione dimensione (2MB) e numero righe (1000 max) | `CsvImportService.cs:18-32` |
| 10 | MEDIUM | CSV export: escape virgolette doppie nei valori | `CsvExportService.cs:34-35` |

**Da configurare sul server (PocketBase Admin UI):**

| # | Azione |
|---|--------|
| 1 | Aggiornare API Rules `logged_workouts` con row-level ownership (`user = @request.auth.id`) |
| 2 | Aggiornare API Rules `social_graph` con controllo `from_user`/`to_user` |
| 3 | Impostare API Rules `excercise`: solo admin può Update/Delete |
| 4 | Impostare API Rules `users`: solo admin può Delete, solo owner può Update |
| 5 | Configurare Nginx: bloccare `/_/`, rate limit auth 5r/m, security headers |

**Da configurare sul server (Nginx):**
Vedi la sezione "Backend Hardening" qui sotto per la configurazione completa.

---

### IT-SEC-03 — Certificate Pinning e Pulizia Finale ✅
**Data**: 18 giugno 2026 — **Branch**: `feature/achievements-fix`

Terzo audit: implementato certificate pinning e corretti dati obsoleti nei documenti.

**Fix applicati:**

| # | Fix | File |
|---|-----|------|
| 1 | Certificate pinning: validazione catena Let's Encrypt per HttpClient PocketBase | `CertificatePinningHandler.cs` (nuovo) |
| 2 | Dominio reale rimosso da `CHANGELOG.md` | `CHANGELOG.md:16` |
| 3 | `FORGE.apk` rimosso da Git, aggiunto a `.gitignore` | `.gitignore`, staging |
| 4 | Metriche README corrette: 27 test, 1 converter, link APK→Releases | `README.md` |
| 5 | `architecture.md`: service names aggiornati (`PlanService`, `ExerciseDbApiService`) | `docs/architecture.md` |
| 6 | Badge DuckDNS aggiunto nei README | `README.md` |
| 7 | Tabella stato funzionalità (✅🟡🔴🚧) aggiunta | `README.md` |

**Certificate Pinning — come funziona:**
- `CertificatePinningHandler` estende `HttpClientHandler`
- Valida che il certificato server sia emesso da Let's Encrypt (root ISRG Root X1/X2 o intermediate R3-R10, E5-E10)
- Rifiuta certificati auto-firmati, scaduti, o emessi da altre CA
- Protegge da MITM su Wi-Fi pubblici compromessi

---

### Push Notifications (parziale) ⚠️
**Data**: Giugno 2026 — **Branch**: `feature/test-import-fcm`

Abbiamo preparato l'infrastruttura per le notifiche push, ma l'implementazione è bloccata.

**Cosa è pronto:**
- PocketBase hook JavaScript (`tools/pb_hooks/push.pb.js`) per inviare notifiche FCM
- 3 trigger: nuovo like, nuova richiesta follow, richiesta accettata
- Documentazione completa in `docs/push-notifications.md`
- `UpdateFcmTokenAsync()` in `PocketBaseService`

**Blocco**: I pacchetti `Xamarin.Firebase.Messaging` non supportano .NET 10 (richiedono net8.0-android). In attesa di aggiornamento dei pacchetti.

---

### Test Automatici ✅
**Data**: Giugno 2026 — **Branch**: `feature/test-import-fcm`

**36 test xUnit** in `tests/Forge.Tests/`:
- **Converter test** (9): `InverseBoolConverter`, `BoolToVisibilityConverter`, `DateTimeFormatConverter`
- **Model test** (16): `ExerciseSet`, `WorkoutExercise`, `WorkoutPlan`
- **DTO test** (9): `ExerciseDbV1Dto` (array/string parsing, lista, dettaglio)
- **Message test** (2): `WorkoutSavedMessage`, `PocketBaseDto`

---

## 3. Architettura e Stack Tecnologico

```
┌──────────────────────────────────────────────┐
│              .NET MAUI 10 App                 │
│  ┌──────────┐  ┌───────────┐  ┌──────────┐  │
│  │  Views   │  │ ViewModels│  │ Services │  │
│  │  XAML    │◄─┤ MVVM      │◄─┤ Business │  │
│  │  puro    │  │ Toolkit   │  │ Logic    │  │
│  └──────────┘  └───────────┘  └────┬─────┘  │
│                                    │         │
│         ┌──────────────────────────┼──────┐  │
│         │                     ┌────▼───┐ │  │
│    ┌────▼────┐   ┌──────────┐ │ SQLite │ │  │
│    │PocketBase│  │ExerciseDB│ │ locale │ │  │
│    │ Auth +   │  │ v1 API   │ │ cache  │ │  │
│    │ Social   │  │ 1500+ ex │ └────────┘ │  │
│    └─────────┘  └──────────┘             │  │
└──────────────────────────────────────────┘
```

### Stack Completo

| Tecnologia | Ruolo |
|-----------|-------|
| `.NET MAUI 10` | Framework cross-platform Android-first |
| `CommunityToolkit.Mvvm 8.4` | MVVM: ObservableProperty, RelayCommand, WeakReferenceMessenger |
| `Shell` | Navigazione 3 tab + 8 route di dettaglio |
| `PocketBase` (self-hosted) | Auth, database remoto, file storage, social graph |
| `ExerciseDB v1` (oss.exercisedb.dev) | Catalogo 1.500+ esercizi con GIF, gratuito |
| `SQLite` (sqlite-net-pcl) | Cache offline: workout, esercizi, piani, achievement |
| `SecureStorage` | Password e dati sensibili (Android Keystore) |
| `System.Text.Json` | Parsing DTO, serializzazione JSON |
| `Google Fonts` | Inter (body), Lexend (label), Space Grotesk (headline) |
| `Nginx Proxy Manager` | Reverse proxy HTTPS, Let's Encrypt, rate limiting |
| `DuckDNS` | DNS dinamico per il server casalingo |
| `Canva Pro` | Design badge achievement (38 icone) |

### Navigation Shell

```
AppShell (TabBar)
├── Tab 1: Dashboard  →  HomePage
├── Tab 2: Feed       →  FeedPage
└── Tab 3: Stats      →  StatsPage

Route di dettaglio:
├── "startSession"    →  StartSessionPage
├── "activeWorkout"   →  ActiveWorkoutPage
├── "workoutDetail"   →  WorkoutDetailPage
├── "achievements"    →  AchievementsPage
├── "profile"         →  ProfilePage
├── "settings"        →  SettingsPage
├── "friendRequests"  →  FriendRequestsPage
└── "login"           →  LoginPage
```

### Collezioni PocketBase

| Collection | Campi principali | Regole API |
|-----------|-----------------|------------|
| `users` | email, password, name, bio, avatar, fcm_token | Auth PocketBase |
| `logged_workouts` | user, name, date, exercises, exercise_data, volume, duration, likes, liked_by, photos | List/View/Create/Update auth |
| `social_graph` | from_user, from_name, to_user, status | Create/List auth |
| `excercise` | exercise_id, name, bodyPart, equipment, instructions, imageUrl, category | List auth, Update/Delete admin |

### Database SQLite (4 tabelle)

| Tabella | Modello | Uso |
|---------|---------|-----|
| `local_workouts` | `LocalWorkout` | Workout offline + draft + sync |
| `cached_exercises` | `CachedExercise` | Cache esercizi da API |
| `saved_plans` | `SavedPlan` | Piani allenamento salvati |
| `achievements` | `AchievementState` | Progresso achievement |

---

## 4. Problemi Risolti e Lezioni Apprese

### Problemi Tecnici

| # | Problema | Causa | Soluzione |
|---|----------|-------|-----------|
| 1 | Short URL ExerciseDB bloccati da ISP | Telecom Italia risolve `encr.pw`/`acesse.dev` a 127.0.0.1 | URL resolution via redirect HTTP → cache PocketBase → migrazione a ExerciseDB v1 con GIF diretti |
| 2 | Crash `JavaProxyThrowable` | `HttpClient.BaseAddress` dopo prima richiesta | `BaseAddress` impostato nel costruttore `HttpClient` in `MauiProgram.cs` |
| 3 | `CollectionView` annidate crash Android | Bug noto MAUI | Sostituite con `BindableLayout` su `VerticalStackLayout` |
| 4 | Campi PocketBase polymorfi | `user` restituito come oggetto o stringa | Parsing con `JsonDocument` per ogni item |
| 5 | 22 `catch(Exception)` vuoti | Codice generato senza gestione errori | `Debug.WriteLine` con messaggio descrittivo |
| 6 | 9 chiamate fire-and-forget | `Task.Run` senza gestione eccezioni | `ContinueWith(OnlyOnFaulted)` |
| 7 | Filtri ExerciseDB non funzionanti | API gratuita non supporta filtri server-side | Filtro client-side su cache SQLite (istantaneo) |
| 8 | Immagini achievement non visibili | Git teneva nomi uppercase, MAUI li rifiuta | `git rm --cached` + lowercase rename |
| 9 | `WorkoutDetailPage` vuota | `BindingContext` mai impostato | Costruttore riceve `WorkoutDetailViewModel` |
| 10 | Allenamenti non finiti contati | ✕ chiamava `FinishWorkoutCommand` | ✕ ora salva come draft (non conta), FINISH cancella draft e salva |

### Lezioni Apprese

1. **API gratuite hanno limitazioni serie** — ExerciseDB v1 ignora la paginazione e ha rate limit aggressivo. Non fare affidamento su API esterne gratuite per dati critici.
2. **PocketBase richiede API rules esplicite** — senza row-level ownership, qualsiasi utente può leggere/modificare i dati degli altri. Implementare sempre.
3. **SecureStorage > Preferences per dati sensibili** — mai mettere password in `SharedPreferences`.
4. **Il token JWT non va mai in URL** — finisce nei log del server, nella history del browser, nei referer header.
5. **MAUI ha requisiti precisi sui nomi file** — solo lowercase, alfanumerici e underscore per le immagini.
6. **Git su Windows è case-insensitive** — i rename uppercase→lowercase richiedono `git rm --cached` + `git add`.

---

## 5. Stato Attuale del Prodotto

### Screenshot e Flusso Utente

```
Login → Dashboard → Start Session → Active Workout → Finish → Stats/Profile
  │         │              │                │
  │         ├─ Streak       ├─ Quick Start   ├─ Ricerca esercizi
  │         ├─ Squad        ├─ Create Plan   ├─ Serie (kg × reps)
  │         ├─ Today Card   └─ Protocols     ├─ Foto progresso
  │         ├─ Achievements                  ├─ Rest timer
  │         └─ START WORKOUT                 └─ Minimize (draft)
  │
  ├─ Feed → Search Users → Follow → Like → Friend Requests
  └─ Profile → Avatar → Recent Forges → Badge Showcase → Edit
```

### Metriche del Progetto

| Metrica | Valore |
|---------|--------|
| Iterazioni completate | 15 |
| Pagine XAML | 10 |
| ViewModel | 12 |
| Services | 13 |
| Modelli/DTO | 6 |
| Tabelle SQLite | 4 |
| Collezioni PocketBase | 4 |
| Achievement | 48 (38 con badge Canva) |
| Test automatici | 27 (100% passati) |
| Font | 5 (Inter, Lexend, Space Grotesk, OpenSans Regular+Semibold) |
| Converter | 1 |

---

## 6. Sicurezza e Hardening

Due audit di sicurezza completati (IT-SEC-01 e IT-SEC-02). Totale 28 vulnerabilità identificate, tutte risolte lato codice. Ecco lo stato attuale:

### Fix Applicati nel Codice ✅

| # | Severità | Fix |
|---|----------|-----|
| 1 | CRIT | API key ExerciseDB rimossa da APK e file sorgente |
| 2 | CRIT | Avatar e file PocketBase: `GetFileUrl()` include `?token=` per autenticazione |
| 3 | CRIT | Icone achievement: rimosso prefisso `achievements/` per compatibilità MAUI |
| 4 | HIGH | URL hardcoded rimosso → eccezione se non configurato |
| 5 | HIGH | Password: solo SecureStorage, migrazione one-shot da Preferences |
| 6 | HIGH | `.env` NON copiato nell'APK in Release (solo Debug) |
| 7 | HIGH | Token JWT non più esposto in URL non necessari |
| 8 | HIGH | `android:allowBackup=false` (blocca backup automatico) |
| 9 | HIGH | Foto workout: limite ridotto a 3MB per prevenire overflow JSON PocketBase |
| 10 | MEDIUM | Auto-refresh token JWT su risposta 401 in endpoint critici |
| 11 | MEDIUM | Ricerca utenti ed esercizi con CancellationToken (no race condition) |
| 12 | MEDIUM | CSV import: validazione dimensione (2MB) e righe (1000 max) |
| 13 | MEDIUM | CSV export: escape virgolette doppie nei valori |
| 14 | MEDIUM | Logout pulisce anche vecchie password in Preferences |

### Da Applicare sul Server 📋

**Per istruzioni complete, vedi la sezione [#backend-hardening](#backend-hardening-pocketbase--nginx) in fondo a questo documento.**

| # | Azione | Dove |
|---|--------|------|
| 1 | API Rules `logged_workouts`: `user = @request.auth.id` | PocketBase Admin UI |
| 2 | API Rules `social_graph`: controllo `from_user`/`to_user` | PocketBase Admin UI |
| 3 | API Rules `excercise`: solo admin Update/Delete | PocketBase Admin UI |
| 4 | API Rules `users`: solo owner Update, solo admin Delete | PocketBase Admin UI |
| 5 | Nginx: bloccare `/_/`, rate limit auth, security headers | Nginx Proxy Manager |

---

## 7. Prossimi Passi

### Immediato (questa settimana)
1. ~~**Configurare backend**: API Rules PocketBase + Nginx hardening~~ ✅ COMPLETATO (18 giugno 2026)

### Breve termine
2. Body tracking (peso e misure corporee) — IT-04
3. Leaderboard settimanale tra amici
4. Compressione foto workout lato server
5. Feedback visivo per language selection in Settings

### Medio termine
6. Certificate pinning per HttpClient PocketBase
7. Confronto diretto statistiche con amico
8. PDF export report allenamenti

### Lungo termine (bloccato da dipendenze esterne)
9. Notifiche push FCM (in attesa di supporto .NET 10 per Firebase SDK)
10. Pubblicazione su Google Play Store

---

## 8. Backend Hardening — PocketBase + Nginx

Configurazioni da applicare sul server per completare la messa in sicurezza.

### API Rules PocketBase

Apri **PocketBase Admin UI → Collections → [collezione] → Edit → API Rules** e imposta:

#### logged_workouts

```
List/Search: @request.auth.id != "" && user = @request.auth.id
View:        @request.auth.id != ""
Create:      @request.auth.id != "" && user = @request.auth.id
Update:      @request.auth.id != "" && (user = @request.auth.id || @request.auth.id ?= liked_by)
Delete:      @request.auth.id != "" && user = @request.auth.id
```

#### social_graph

```
List/Search: @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
View:        @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
Create:      @request.auth.id != "" && from_user = @request.auth.id
Update:      @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
Delete:      @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
```

#### excercise (catalogo pubblico)

```
List/Search: @request.auth.id != ""
View:        @request.auth.id != ""
Create:      @request.auth.id != ""
Update:      @request.auth.id = ""
Delete:      @request.auth.id = ""
```

#### users

```
List/Search: @request.auth.id != ""
View:        @request.auth.id != ""
Update:      @request.auth.id != "" && id = @request.auth.id
Delete:      @request.auth.id = ""
```

### Configurazione Nginx

Aggiungi al file di configurazione Nginx (Nginx Proxy Manager → Edit Proxy Host → Advanced):

```nginx
# Blocca PocketBase Admin Panel da accesso esterno
location /_/ {
    deny all;
    return 403;
}

# Rate limiting per auth endpoint
limit_req_zone $binary_remote_addr zone=auth:10m rate=5r/m;

location /api/collections/users/auth-with-password {
    limit_req zone=auth burst=3 nodelay;
    proxy_pass http://pocketbase:8090;
}

# Rate limiting per API generica
limit_req_zone $binary_remote_addr zone=api:10m rate=60r/m;

location /api/ {
    limit_req zone=api burst=20 nodelay;
    proxy_pass http://pocketbase:8090;
}

# Security headers globali
add_header Strict-Transport-Security "max-age=63072000; includeSubDomains" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-Frame-Options "DENY" always;
add_header X-XSS-Protection "1; mode=block" always;

# Limite dimensione upload (foto, avatar)
client_max_body_size 5m;

# Nascondi versione server
server_tokens off;
```

### PocketBase serve HTTPS

Assicurati che PocketBase sia avviato solo su HTTPS:

```bash
./pocketbase serve --https=0.0.0.0:8090
```

NON usare `--http` in produzione. Blocca la porta HTTP nel firewall:

```bash
ufw deny 8080
```

---

## 📁 Documentazione Collegata

| Documento | Scopo |
|-----------|-------|
| [`docs/spec.md`](docs/spec.md) | Specifica prodotto con epic, user stories, criteri accettazione |
| [`docs/plan.md`](docs/plan.md) | Piano iterazioni con stato |
| [`docs/architecture.md`](docs/architecture.md) | Architettura tecnica dettagliata |
| [`docs/api-notes.md`](docs/api-notes.md) | Note tecniche API PocketBase e ExerciseDB |
| [`docs/pocketbase-schema.md`](docs/pocketbase-schema.md) | Schema collezioni PocketBase |
| [`docs/push-notifications.md`](docs/push-notifications.md) | Setup notifiche push FCM |
| [`docs/test-matrix.md`](docs/test-matrix.md) | Matrice 42 test manuali eseguiti |
| [`docs/Iterazioni-IA.md`](docs/Iterazioni-IA.md) | Audit report e cronologia modifiche AI |
| [`docs/prompt-log.md`](docs/prompt-log.md) | Log prompt significativi con decisioni |
| [`docs/consegna-presentazione.md`](docs/consegna-presentazione.md) | Prompt per presentazione Canva/Gemini |
| [`docs/method/man-in-the-loop.md`](docs/method/man-in-the-loop.md) | Metodologia Man-in-the-Loop |

---

> **FORGE** — Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.
>
> *Documento generato il 18 giugno 2026. Ultimo aggiornamento: branch `feature/achievements-fix`.*

## 12 ottobre 2026 — develop

Su richiesta del proprietario baseline committata con 305db83; nessun push. Nuove richieste implementate: calendario per periodo, tutti i badge e livello achievement, foto/storico/serie reversibili, grafici esercizi, account deletion, icona Android e integrazione notifiche. Preferenza recepita: niente suite completa ripetuta, solo controlli mirati e build. L’utente non ha Firebase; attivazione esterna documentata in push-notifications.md. Cloudflare rimandato. Prime tre migrazioni LXC confermate dall’output del proprietario, quarta preparata.

Firebase configurato nel client: google-services.json presente, validato per com.leoforge.forge ed escluso da Git. Il proprietario conferma migrazione 1791763200 applicata nel CT 112 e PocketBase attivo; backup 20261012-085930. Preparazione APK con Firebase senza rieseguire le suite. Attivazione worker e ricezione FCM ancora da verificare.

## 12 ottobre 2026 — experiments
Branch experiments creato da develop conservando le modifiche precedenti. Nuovo stile workout nei due temi, menu coerenti, consigli e immagini, note, ricerca separata e aggiornamenti lista raggruppati, sostituzione senza perdere esercizio se annullata. Timer Android con cronometro nativo e allarme locale, indipendente da Firebase; permesso allarmi precisi richiesto solo tramite azione utente. PocketBase e forge-push confermati active dal proprietario; ricezione FCM reale ancora non provata. Nessun push o nuovo commit richiesto.

Verifica finale: SignAndroidPackage con EmbedAssembliesIntoApk=true riuscito, 0 errori e 3 avvisi dei binding Firebase. XML e diff controllati. APK con le ultime sorgenti in artifacts/FORGE-experiments.apk. Verifica visiva e notifica del recupero sul telefono ancora da eseguire.

### Correzioni successive experiments
Recepito feedback del telefono: suono mancante, calendario lento oltre tre mesi, refresh tardivo e filtri contaminati dai muscoli secondari. Upload non blocca più il messaggio di salvataggio; refresh dei dati locali conserva lo storico remoto già caricato. Calendario virtualizzato, target principali nei filtri, consigli rotanti. Notifica con barra/nome/miniatura quando scaricabile, servizio foreground specialUse e wake lock limitato ai cinque minuti del recupero (rilasciato in pausa/uscita), tono locale e shortcut al canale. Sei test mirati passati, nessuna suite completa ripetuta; suono reale ancora da verificare.

Build finale Android riuscita: 0 errori e 3 avvisi preesistenti Firebase. APK artifacts/FORGE-experiments.apk aggiornato, tono incluso verificato nel pacchetto. Sei regressioni mirate passate; visualizzazione e suono sul telefono restano da confermare.

12 ottobre, ritocco experiments: ruote native del recupero misurate a larghezza piena per allineare minuti/secondi alle intestazioni; follower/seguiti in un unico riquadro con contatori, separatore e colori dinamici. Il proprietario conferma suono funzionante e filtri apparentemente corretti sul Samsung A15. Allineamento e profilo da verificare sul dispositivo dopo aggiornamento APK.

### Schede e tipi di serie — experiments
Alla fine di una sessione da scheda, le modifiche a nome, esercizi/ordine, note, recuperi e serie chiedono se aggiornare la scheda o mantenere l’originale. Nessuna modifica automatica durante la sessione; salvare il recupero cambia solo la sessione fino alla scelta finale. La scheda aggiornata mantiene ID e data di creazione e azzera i flag di completamento. Numero SET cliccabile: normale, W warm-up giallo, D drop set viola, F cedimento rosso; tipi persistiti in schede, bozze e storico/dettaglio. Tre test mirati passati (firma, rinomina/esercizi aggiunti, copia senza mutazione e tipi). Prove popup/scelte su telefono ancora da eseguire. Nessun push richiesto.

Verifica finale: APK Android firmato con assembly inclusi generato senza errori (3 avvisi Firebase già presenti). artifacts/FORGE-experiments.apk aggiornato. Tre regressioni mirate superate; nessun push.

## 13 ottobre 2026 — valutazione su experiments

Il proprietario conferma le ultime funzioni sul telefono. Documentati il requisito delle note salvate automaticamente, la proposta di modalità locale/server facoltativo e aggiornamenti dall’app, e le ricompense per record solo tra le idee future. In questa fase aggiornati solo documenti: nessuna modifica runtime, build, commit o push.

### Note automatiche — 13 ottobre 2026, experiments
Le note della scheda e degli esercizi già presenti vengono salvate dopo una breve pausa nella digitazione (400 ms), con salvataggio atteso anche quando si minimizza o termina la sessione. Vengono ricaricate alla successiva apertura della scheda. Le sole modifiche alle note non aprono il popup finale; scegliere di mantenere la scheda originale conserva comunque le note. Il salvataggio delle note aggiorna soltanto quelle, senza introdurre esercizi nuovi o modificare nome, ordine, serie e recuperi. Le note di esercizi aggiunti o sostituiti restano nella sessione e nella scheda se viene approvato l’aggiornamento. Nessuna migrazione server necessaria.

Verifiche: cinque regressioni mirate superate, inclusi round trip SQLite, cancellazione delle note, isolamento dell’account e firma del popup. Da verificare sul dispositivo: modificare soltanto le note e terminare senza popup; riaprire la scheda e ritrovarle; modificare anche il nome e scegliere di mantenere l’originale, poi verificare che il nome resti invariato e le note aggiornate.

Build finale Android e firma APK riuscite: 0 errori, 3 avvisi preesistenti dei binding Firebase. artifacts/FORGE-experiments.apk aggiornato. Nessuna verifica UI su dispositivo eseguita in questa sessione; cinque test mirati superati.

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
