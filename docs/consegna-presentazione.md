# CONSEGNA PER GEMINI — Presentazione FORGE

## Istruzioni per l'AI

Crea una **presentazione professionale** in formato Google Slides (o PowerPoint) che racconti il **percorso di sviluppo** e il **capolavoro finale** dell'app FORGE. La presentazione deve essere in **italiano**, visivamente accattivante, coerente con il brand dell'app (ciano #00E5FF per tema scuro, blu #003ec7 per tema chiaro), e deve seguire la struttura sotto.

---

## Struttura della presentazione

### Slide 1 — Copertina
**Titolo:** FORGE — Il tuo diario di allenamento sociale
**Sottotitolo:** Trasforma ogni ripetizione in progresso. Sfida i tuoi amici. Supera i tuoi limiti.
**Elementi visivi:** Logo FORGE, sfondo scuro con accento ciano #00E5FF
**Info:** Nome studente, corso, anno accademico, data

---

### Slide 2 — Il Problema
**Titolo:** Perché serviva FORGE?
**Contenuto:**
- Chi va in palestra fatica a tenere traccia dei progressi e non ricorda i pesi usati la volta precedente
- Non sa quali esercizi fare per un dato muscolo
- Allenarsi da soli riduce la motivazione: manca uno strumento che unisca tracking e social
**Elementi visivi:** 3 icone esplicative (peso, ricerca, squadra) + breve testo

---

### Slide 3 — La Soluzione
**Titolo:** Cosa fa FORGE?
**Contenuto:**
- Catalogo di 1.500+ esercizi con immagini GIF, ricerca e filtri (muscolo, attrezzatura)
- Registrazione allenamenti completi: serie, ripetizioni, peso, rest timer
- Statistiche di progresso: grafico volume, top lifts, calendario allenamenti
- Feed sociale: segui amici, metti like, competizione sana con streak
- Piani di allenamento salvabili e riutilizzabili
**Elementi visivi:** Elenco puntato compatto + screenshot dell'app (Dashboard, Active Workout, Feed)

---

### Slide 4 — Stack Tecnologico
**Titolo:** L'architettura dietro FORGE
**Contenuto (tabella o diagramma):**

| Tecnologia | Ruolo |
|------------|-------|
| .NET MAUI 10 | Framework cross-platform Android-first |
| CommunityToolkit.Mvvm 8.4 | MVVM, ObservableProperty, RelayCommand, WeakReferenceMessenger |
| Shell Navigation | 3 tab (Dashboard, Feed, Stats) + 7 route di dettaglio |
| PocketBase (self-hosted) | Auth email/password, database remoto, file storage, social graph |
| ExerciseDB v1 API (oss.exercisedb.dev) | Catalogo 1.500+ esercizi con GIF, filtri, paginazione |
| SQLite (sqlite-net-pcl) | Cache offline esercizi, piani, allenamenti |
| System.Text.Json | Parsing DTO, serializzazione |
| Preferences / SecureStorage | Token JWT, credenziali, settings |
| Google Fonts | Inter (body), Lexend (label), Space Grotesk (headline) |

**Elementi visivi:** Diagramma architetturale: Views → ViewModels → Services → PocketBase / ExerciseDB / SQLite

---

### Slide 5 — Metodo di Lavoro: Man-in-the-Loop
**Titolo:** Come abbiamo lavorato con l'AI
**Contenuto:**
- **Man-in-the-Loop**: l'AI non opera da sola, ogni iterazione segue un ciclo a 5 fasi:
  1. **Planning**: obiettivo verificabile, criteri di accettazione, branch Git dedicato
  2. **Build**: codice generato con prompt strutturati (contesto, vincoli, formato atteso)
  3. **Review**: codice letto, confrontato con specifica, corretto se necessario
  4. **Testing**: verifica casi normali e limite, test manuali
  5. **Documentazione e Git**: aggiornamento docs, commit semantici, merge
- Iterazioni atomiche: una feature per volta, mai refactoring ampi
- Documentazione continua: spec.md, plan.md, architecture.md, test-matrix.md, prompt-log.md

**Elementi visivi:** Diagramma del ciclo Man-in-the-Loop (Planning → Build → Review → Test → Doc/Git)

---

### Slide 6 — Percorso Iterativo
**Titolo:** Da zero a app completa: le iterazioni
**Contenuto (timeline visiva):**

| Iterazione | Obiettivo | Stato |
|------------|-----------|-------|
| IT-01 | Bootstrap MAUI + Shell 3-tab + Design System + ThemeService | ✅ |
| IT-02/03/08 | Catalogo esercizi + Allenamento + Piani salvabili | ✅ |
| IT-05/06/07 | PocketBase Auth + Social + Dashboard + Stats + Profilo | ✅ |
| IT-VULN-01 | Sicurezza: SecureStorage, IHttpClientFactory, JWT refresh | ✅ |
| IT-BUG-01 | 22 empty catch → Debug.WriteLine, 9 fire-and-forget → ContinueWith | ✅ |
| IT-FEAT-01 | SQLite offline (DatabaseService, LocalWorkout, CachedExercise) | ✅ |
| IT-FEAT-02 | PlanStore → PlanService (SQLite + auto-migration) | ✅ |
| IT-FEAT-03 | Rimozione 5 ViewModel stub + 5 Page stub | ✅ |
| IT-FEAT-04 | Wger API → ExerciseDB v1 free API | ✅ |
| IT-FEAT-05 | CSV import/export in Settings | ✅ |
| IT-04 | Tracking peso corporeo e misure | ❌ Posticipato |

**Elementi visivi:** Timeline con colori diversi per iterazioni completate (verde) e posticipate (rosso)

---

### Slide 7 — Design e UX
**Titolo:** Doppio tema, un'identità
**Contenuto:**
- **Doppio tema**: "Cyber-Athletic Elite" (scuro, ciano #00E5FF) e "Fitness Core" (chiaro, blu #003ec7)
- **Toggle runtime**: cambio tema istantaneo su tutte le pagine, Shell inclusa
- **Font Google Fonts**: Inter (body), Lexend (label/caps), Space Grotesk (headline/metriche)
- **Stati UI espliciti**: loading (ActivityIndicator), success (dati presenti), empty (messaggio + azione), error (messaggio + retry)
- **Design da Stitch (Google)**: mockup professionali, icone SVG, palette coerenti

**Elementi visivi:** Side-by-side di due schermate (tema scuro vs tema chiaro) — es. Login, Dashboard o Active Workout

---

### Slide 8 — Funzionalità Chiave: Workout
**Titolo:** Registrare un allenamento in pochi secondi
**Contenuto (flusso illustrato):**
1. Dashboard → START WORKOUT
2. Start Session → Quick Start o piano salvato
3. Ricerca esercizio (nome, muscolo, attrezzatura) — cache SQLite locale (istantaneo), API fallback
4. Aggiunta serie con peso e ripetizioni, checkmark ✓ per completamento
5. Rest timer configurabile per esercizio
6. FINISH → salvataggio su PocketBase + sincronizzazione offline via SyncService
**Elementi visivi:** Screenshot sequenziali o mockup del flusso: Dashboard → Start Session → Active Workout → Conferma

---

### Slide 9 — Funzionalità Chiave: Social
**Titolo:** Competizione sana con gli amici
**Contenuto:**
- Ricerca utenti live con debounce 400ms
- Richieste di follow: invio, accettazione, rifiuto
- Feed allenamenti degli amici con like/unlike (♥ toggle, conteggio istantaneo)
- Streak settimanale: settimane consecutive con allenamento (tolleranza 7 giorni)
- Notifiche: richieste di amicizia + like ricevuti
- Avatar utente visibile in Dashboard, Feed, Stats e Profilo
**Elementi visivi:** Screenshot del Feed con like, Friend Requests, Squad Activity

---

### Slide 10 — Funzionalità Chiave: Statistiche
**Titolo:** I numeri parlano
**Contenuto:**
- Total Workouts / Volume / Hours — card riepilogative con trend ▲
- Grafico volume a barre settimanali (4 mesi)
- Top Lifts: i 5 esercizi con peso massimo sollevato
- Calendario mensile con pallino sui giorni di allenamento
- Filtri temporali: WEEK / MONTH / 3M / YEAR / ALL
**Elementi visivi:** Screenshot StatsPage con grafico, top lifts, calendario

---

### Slide 11 — Sicurezza e Offline
**Titolo:** Robustezza e resilienza
**Contenuto:**
- **SecureStorage**: password e token JWT su Android Keystore con fallback Preferences
- **IHttpClientFactory**: 3 client named (pocketbase, exercisedbv1, redirect-resolver) per handler pooling
- **JWT token refresh**: EnsureAuthAsync rinnova automaticamente i token scaduti
- **SQLite offline**: DatabaseService con 3 tabelle (local_workouts, cached_exercises, saved_plans)
- **SyncService**: auto-push allenamenti pendenti quando la connessione ritorna
- **CSV import/export**: backup/ripristino dati utente in formato CSV
- **BuildSecrets**: PocketBase URL e API keys iniettate a build-time, mai in chiaro nel codice

**Elementi visivi:** Diagramma del flusso offline-online: SQLite → SyncService → PocketBase

---

### Slide 12 — Sfide e Soluzioni Tecniche
**Titolo:** Cosa è andato storto e come l'abbiamo risolto
**Contenuto (elenchi puntati):**

1. **ISP-blocking di ExerciseDB**: i domini `encr.pw`, `acesse.dev`, `l1nq.com` erano risolti a 127.0.0.1 da Telecom Italia → risolti con URL resolution (redirect HTTP) e cache su PocketBase, infine migrato a ExerciseDB v1 free API con GIF dirette
2. **PocketBase `user` field polymorfo**: restituito come stringa O oggetto `{id:"..."}` → parsing robusto con `JsonDocument` per ogni campo
3. **CollectionView annidate crashano su Android** → sostituite con `BindableLayout` su `VerticalStackLayout`
4. **22 empty catch blocks** → `Debug.WriteLine` con logging condizionale `#if DEBUG`
5. **9 fire-and-forget async** → `ContinueWith(TaskContinuationOptions.OnlyOnFaulted)`
6. **Filtri ExerciseDB non funzionanti**: API gratuita non supporta filtri lato server → filtro client-side su cache SQLite (istantaneo, offline)
7. **Race condition startup**: BuildSecrets non caricato prima di LoginPage → caricato in `App.CreateWindow()` prima di mostrare la UI

**Elementi visivi:** Bullet points con icona ⚠️ per problema e ✅ per soluzione

---

### Slide 13 — Metriche di Progetto
**Titolo:** Numeri e statistiche
**Contenuto (tabella):**

| Metrica | Valore |
|---------|--------|
| Iterazioni completate | 10 (su 11 pianificate) |
| ViewModel | 14 |
| Views/XAML | 16 |
| Services | 8 (PocketBase, ExerciseDbApi, Database, Sync, Plan, Theme, BuildSecrets, WorkoutSession) |
| Models/DTO | 6 (WorkoutPlan, CachedExercise, LocalWorkout, ExerciseDbV1Dto, PocketBaseDto, SavedPlan) |
| Test manuali eseguiti | 42 (tutti superati) |
| Font Google Fonts | 5 (Inter, Lexend, Space Grotesk, OpenSans Regular+Semibold) |
| Converters | 3 (InverseBool, BoolToVisibility, DateTimeFormat) |
| Collezioni PocketBase | 4 (users, logged_workouts, social_graph, excercise) |
| Esercizi in cache | 1.500+ (ExerciseDB v1) |
| API ExerciseDB | 1 (oss.exercisedb.dev/api/v1, gratuita, no key) |
| Iterazione posticipata | 1 (IT-04: body tracking) |

---

### Slide 14 — Demo Flow
**Titolo:** FORGE in azione
**Contenuto:** Screenshot-gallery del flusso completo dell'app:

1. **Login** → registrazione con email/nome/password
2. **Dashboard** → streak, squad, today card, START WORKOUT
3. **Start Session** → Quick Start / Create New Plan / Your Protocols
4. **Active Workout** → ricerca esercizi con filtri (petto, bilanciere…), serie con kg×reps, checkmark ✓, rest timer
5. **Feed** → ricerca utenti, follow, like ♥
6. **Stats** → grafico volume, top lifts, calendario, filtri temporali
7. **Profilo** → avatar, stats, recent forges, edit
8. **Settings** → toggle tema chiaro/scuro, CSV export, logout

**Elementi visivi:** Grid 2×4 di screenshot dell'app, numerati 1-8

---

### Slide 15 — Prossimi Passi
**Titolo:** Dove andiamo da qui
**Contenuto:**
- Implementare IT-04: tracking peso corporeo e misure
- Importare tutti i 1.500 esercizi in PocketBase per eliminare dipendenza API
- Dettaglio allenamento completo (visualizzazione serie passate)
- Notifiche push per like e richieste di amicizia
- Leaderboard settimanale tra amici
- Foto progresso fisico con confronto temporale
- Export PDF report
- Pubblicazione su Google Play Store

---

### Slide 16 — Chiusura
**Titolo:** FORGE — Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.
**Contenuto:**
- Link al repository: `github.com/USERNAME/FORGE`
- Stack: .NET MAUI + PocketBase + ExerciseDB v1 + SQLite
- Metodo: Man-in-the-Loop con AI (OpenCode + Gemini)
- Ringraziamenti

**Elementi visivi:** Logo FORGE, sfondo scuro gradiente, link QR code al repo

---

## Linee guida di stile

1. **Colori primari**: Ciano #00E5FF (scuro), Blu #003ec7 (chiaro), Sfondo #0E0E0E (scuro) o #F5F5F5 (chiaro)
2. **Font**: Inter per testo, Lexend per label, Space Grotesk per titoli e metriche
3. **Stile grafico**: Pulito, atletico, minimal — ispirato al design "Cyber-Athletic Elite"
4. **Screenshot dell'app**: includere screenshot reali del tema scuro (più impattante visivamente)
5. **Diagrammi**: preferire schema angular (box con frecce) per architettura e flussi
6. **Icone**: usare icone Material Design o Font Awesome (dumbbell, chart, users, heart, trophy)
7. **Lingua**: italiano per tutto il testo della presentazione, inglese per nomi tecnici (ViewModel, API, ecc.)
8. **Quantità slide**: circa 16 slide (puoi unire o dividere se serve, ma mantieni l'ordine logico)
9. **Rapporto testo/visivo**: minimo 40% visivo (screenshot, diagrammi, icone), massimo 60% testo
10. **Note speaker**: per ogni slide, aggiungere note per l'oratore con dettagli tecnici aggiuntivi

---

## Contesto completo del progetto

### Nome app
FORGE — Il tuo diario di allenamento sociale

### Repository
github.com/USERNAME/FORGE (privato)

### Framework
.NET MAUI 10, Android-first, MVVM con CommunityToolkit.Mvvm, Shell navigation

### Backend
PocketBase self-hosted (https://YOUR_SERVER.duckdns.org) per auth, social graph, workout storage

### API
ExerciseDB v1 free (oss.exercisedb.dev) — 1.500+ esercizi con GIF, filtri per muscolo/attrezzatura, senza API key

### Persistenza
SQLite offline (local_workouts, cached_exercises, saved_plans) + PocketBase remoto + Preferences/SecureStorage locali

### Iterazioni significative
- IT-01: Bootstrap MAUI + Shell + Design System
- IT-02/03/08: Catalogo esercizi + Allenamento + Piani
- IT-05/06/07: PocketBase Auth + Social + Dashboard + Stats + Profilo
- IT-VULN-01: Sicurezza (SecureStorage, IHttpClientFactory, JWT refresh)
- IT-BUG-01: 22 fix (empty catch, fire-and-forget, race condition, calendar midnight)
- IT-FEAT-01: SQLite offline + SyncService
- IT-FEAT-02: PlanService con auto-migration da Preferences
- IT-FEAT-03: Rimozione codice morto (5 stub VM + 5 stub Page)
- IT-FEAT-04: Wger → ExerciseDB v1 free API
- IT-FEAT-05: CSV import/export

### Sfide risolte
1. ISP-blocking di short URL ExerciseDB → URL resolution + cache + migrazione a ExerciseDB v1 con GIF dirette
2. PocketBase `user` field polymorfo (stringa o oggetto) → JsonDocument parsing per-item
3. CollectionView annidate crash Android → BindableLayout
4. 22 empty catch → Debug.WriteLine condizionale
5. 9 fire-and-forget → ContinueWith(OnlyOnFaulted)
6. Filtri ExerciseDB non supportati server-side → filtro client-side su cache SQLite
7. Race condition startup → BuildSecrets caricato prima della UI
8. PlanStore su Preferences → migrato a SQLite con auto-migration

### Documentazione
- docs/spec.md — Specifica completa (epic, user stories, criteri accettazione)
- docs/plan.md — Piano iterazioni con stato
- docs/architecture.md — Architettura tecnica dettagliata
- docs/test-matrix.md — 42 test manuali eseguiti
- docs/Iterazioni-IA.md — Audit report e cronologia modifiche completa
- docs/prompt-log.md — Log dei prompt significativi con decisioni
- docs/api-notes.md — Note tecniche su PocketBase e ExerciseDB API
- docs/method/man-in-the-loop.md — Metodologia Man-in-the-Loop

### Test
42 test manuali eseguiti e superati (auth, API, social, UI, persistenza, logica streak/stats)

### Stato attuale
App funzionale completa: auth, catalogo 1.500+ esercizi, allenamento, statistiche, social, profilo, piani, offline, CSV export, doppio tema chiaro/scuro. Pronta per la demo e la consegna.