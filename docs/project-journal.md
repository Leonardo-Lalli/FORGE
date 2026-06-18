# FORGE — Diario di Sviluppo

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
- Progetto `src/GymTracker.Mobile/` con struttura MVVM
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
- Rimossa `EXERCISEDB_API_KEY` da `.env`, `.env.example` e `gymtracker.env` (chiave inutilizzata embedded nell'APK)
- Rimosso fallback URL hardcoded → eccezione se non configurato
- Password: rimosso fallback `Preferences` (solo `SecureStorage`)
- Token JWT: rimosso da URL query parameter (`?token=...`)
- `android:allowBackup=false` (blocca backup Google Drive dei dati sensibili)
- `Logout()`: pulisce anche vecchie password in `Preferences`

**Da applicare sul server (documentato in `docs/security-hardening.md`):**
- API Rules PocketBase con row-level ownership
- Nginx: bloccare admin panel `/_/` dall'esterno
- Rate limiting per endpoint auth
- Security headers (HSTS, X-Content-Type-Options, X-Frame-Options)

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

**36 test xUnit** in `tests/GymTracker.Mobile.Tests/`:
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
| Iterazioni completate | 12 |
| Pagine XAML | 10 |
| ViewModel | 12 |
| Services | 11 |
| Modelli/DTO | 6 |
| Tabelle SQLite | 4 |
| Collezioni PocketBase | 4 |
| Achievement | 48 (38 con badge Canva) |
| Test automatici | 36 (100% passati) |
| Font | 5 (Inter, Lexend, Space Grotesk, OpenSans Regular+Semibold) |
| Converter | 3 |

---

## 6. Sicurezza e Hardening

Abbiamo condotto un audit di sicurezza con 18 vulnerabilità identificate. Ecco lo stato attuale:

### Fix Applicati nel Codice ✅

| # | Severità | Fix |
|---|----------|-----|
| 1 | CRIT | API key ExerciseDB rimossa da APK e file sorgente |
| 2 | HIGH | URL hardcoded rimosso → eccezione se non configurato |
| 3 | HIGH | Password: solo SecureStorage, nessun fallback Preferences |
| 4 | HIGH | Token JWT rimosso da URL query parameter |
| 5 | HIGH | `android:allowBackup=false` (blocca backup automatico) |
| 6 | MED | Logout pulisce anche vecchie password in Preferences |

### Da Applicare sul Server 📋

| # | Severità | Azione | File di riferimento |
|---|----------|--------|---------------------|
| 7 | CRIT | API Rules PocketBase con row-level ownership | `docs/security-hardening.md` |
| 8 | HIGH | Nginx: bloccare `/_/` dall'esterno | `docs/security-hardening.md` |
| 9 | HIGH | Nginx: rate limiting endpoint auth | `docs/security-hardening.md` |
| 10 | MED | Nginx: security headers (HSTS, X-Frame-Options) | `docs/security-hardening.md` |
| 11 | LOW | Ruotare chiave ExerciseDB su RapidAPI (se ancora attiva) | rapidapi.com |

---

## 7. Prossimi Passi

### Immediato (questa settimana)
1. Applicare API Rules PocketBase — vedi `docs/security-hardening.md`
2. Configurare Nginx per bloccare admin panel e aggiungere rate limiting

### Breve termine
3. Body tracking (peso e misure corporee) — IT-04
4. Leaderboard settimanale tra amici
5. Compressione foto workout (limite dimensione upload)
6. Feedback visivo per language selection in Settings

### Medio termine
7. Token refresh automatico PocketBase
8. Confronto diretto statistiche con amico
9. PDF export report allenamenti

### Lungo termine (bloccato da dipendenze esterne)
10. Notifiche push FCM (in attesa di supporto .NET 10 per Firebase SDK)
11. Pubblicazione su Google Play Store

---

## 📁 Documentazione Collegata

| Documento | Scopo |
|-----------|-------|
| [`docs/spec.md`](docs/spec.md) | Specifica prodotto con epic, user stories, criteri accettazione |
| [`docs/plan.md`](docs/plan.md) | Piano iterazioni con stato |
| [`docs/architecture.md`](docs/architecture.md) | Architettura tecnica dettagliata |
| [`docs/api-notes.md`](docs/api-notes.md) | Note tecniche API PocketBase e ExerciseDB |
| [`docs/security-hardening.md`](docs/security-hardening.md) | Guida sicurezza PocketBase + Nginx |
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
