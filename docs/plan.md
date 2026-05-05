# Piano di Progetto - GymTracker Mobile

## 1. Sintesi operativa

### Obiettivo del progetto

App .NET MAUI Android-first per tracciare allenamenti in palestra, con catalogo esercizi da ExerciseDB API, cache offline SQLite, statistiche di progresso, e competizione sociale tra amici via Firebase (leaderboard, feed, streak).

### Vincoli principali

- MVVM con CommunityToolkit.Mvvm, Shell navigation, nessuna logica in code-behind.
- Fonti dati remote: ExerciseDB API (RapidAPI) per esercizi, Firebase per social.
- Persistenza locale: SQLite per cache esercizi, allenamenti, peso/misure, piani.
- UI progettata con Stitch ("Iron Rank Fitness Social", tema "Performance Minimalist": dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter).
- Gestione esplicita stati: loading, error, empty, success.

### Dipendenze esterne

- ExerciseDB API (RapidAPI key)
- Firebase project (Auth + Realtime Database / Firestore)
- SQLite (sqlite-net-pcl)

## 2. Sequenza delle iterazioni

| Iterazione | Obiettivo verificabile | Dipendenze | Rischio | Stato |
| --- | --- | --- | --- | --- |
| IT-01 | Bootstrap MAUI + Shell navigation + Design System Stitch | Nessuna | Basso | Pianificata |
| IT-02 | Catalogo esercizi da ExerciseDB API con cache SQLite | IT-01 | Medio | Pianificata |
| IT-03 | Registrazione allenamento con storico e dettaglio | IT-02 | Medio | Pianificata |
| IT-04 | Tracking peso corporeo e misure | IT-01 | Basso | Pianificata |
| IT-05 | Autenticazione Firebase e sincronizzazione allenamenti | IT-03, IT-05 Firebase | Alto | Pianificata |
| IT-06 | Gestione amici, feed e leaderboard | IT-05 | Alto | Pianificata |
| IT-07 | Dashboard, statistiche e streak | IT-03, IT-04, IT-06 | Medio | Pianificata |
| IT-08 | Piani di allenamento base | IT-02, IT-03 | Basso | Pianificata |

## 3. Dettaglio iterazioni

### IT-01 - Bootstrap MAUI e Design System

**Obiettivo verificabile**

Progetto MAUI compilabile su Android con Shell navigation a 5 tab e tema Stitch applicato.

**In scope**

- Creazione progetto `src/GymTracker.Mobile/`
- AppShell con 5 tab: Dashboard, Catalogo, Allenamento, Social, Profilo
- Pagine placeholder per ogni tab con titolo e messaggio di stato
- Applicazione Design System Stitch: dark theme, colori `#007AFF` / `#CCFF00`, font Lexend/Inter
- Dependency injection minima: registrazione ViewModel e pagine in `MauiProgram.cs`
- `BaseViewModel` con `IsBusy`, `ErrorMessage`, `HasData`, `IsEmpty`
- Cartelle `Models/`, `ViewModels/`, `Views/`, `Services/`, `Converters/`

**Out of scope**

- Qualsiasi logica di business, chiamate API, database
- Dati reali nelle pagine
- Navigazione di dettaglio

**File o aree probabili**

- `src/GymTracker.Mobile/GymTracker.Mobile.csproj`
- `src/GymTracker.Mobile/MauiProgram.cs`
- `src/GymTracker.Mobile/App.xaml`, `App.xaml.cs`
- `src/GymTracker.Mobile/AppShell.xaml`, `AppShell.xaml.cs`
- `src/GymTracker.Mobile/Views/*.xaml`
- `src/GymTracker.Mobile/ViewModels/BaseViewModel.cs`
- `src/GymTracker.Mobile/Resources/Styles/`

**Criteri di accettazione**

- [ ] Il progetto compila con `dotnet build` senza errori
- [ ] L'app si avvia su emulatore Android e mostra 5 tab
- [ ] Navigazione tra tab funzionante
- [ ] Il tema dark con colore accent arancione è applicato
- [ ] Ogni tab mostra una pagina placeholder con titolo

**Verifiche principali**

- manuale: avvio su emulatore, navigazione 5 tab
- automatico: `dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj`

**Rischi**

- Configurazione Android SDK / emulatore sulla macchina di sviluppo

---

### IT-02 - Catalogo esercizi da ExerciseDB API

**Obiettivo verificabile**

Catalogo esercizi funzionante con fetch da ExerciseDB API, ricerca, filtri e cache SQLite locale.

**In scope**

- Configurazione `HttpClient` con header RapidAPI (`X-RapidAPI-Key`, `X-RapidAPI-Host`)
- Servizio `ExerciseApiService` con metodi: `GetByBodyPart`, `SearchByName`, `GetByEquipment`, `GetById`
- DTO per risposta ExerciseDB (con mapping difensivo)
- Modello `Exercise` con campi essenziali (id, name, bodyPart, equipment, target, gifUrl, instructions)
- Database SQLite con tabella `ExerciseCache`
- Servizio `ExerciseCacheService`: salva/recupera esercizi usati, limit a 50
- Servizio `ExerciseService` orchestratore: prima cache, poi API, aggiorna cache
- `CatalogViewModel` con ricerca, filtro per gruppo muscolare e attrezzatura
- Pagina Catalogo con `SearchBar` e `CollectionView` raggruppata
- Dettaglio esercizio: nome, GIF, gruppo, attrezzatura, istruzioni
- Stati: loading, error (con retry), empty (nessun risultato), success

**Out of scope**

- Uso degli esercizi in un allenamento (IT-03)
- Sincronizzazione con Firebase (IT-05)
- Piani di allenamento (IT-08)

**File o aree probabili**

- `src/GymTracker.Mobile/Services/ExerciseApiService.cs`
- `src/GymTracker.Mobile/Services/ExerciseCacheService.cs`
- `src/GymTracker.Mobile/Services/ExerciseService.cs`
- `src/GymTracker.Mobile/Models/Exercise.cs`
- `src/GymTracker.Mobile/Models/Dto/ExerciseDto.cs`
- `src/GymTracker.Mobile/Data/DatabaseService.cs`
- `src/GymTracker.Mobile/ViewModels/CatalogViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/ExerciseDetailViewModel.cs`
- `src/GymTracker.Mobile/Views/CatalogPage.xaml`
- `src/GymTracker.Mobile/Views/ExerciseDetailPage.xaml`
- `docs/iterations/it-02-exercise-catalog.md`

**Criteri di accettazione**

- [ ] Catalogo carica esercizi da ExerciseDB API con `bodyPart` filter
- [ ] La ricerca per nome chiama l'API e mostra risultati
- [ ] Il filtro per attrezzatura funziona (dumbbell, barbell, body weight, etc.)
- [ ] Il dettaglio esercizio mostra GIF animata e istruzioni
- [ ] Gli esercizi aperti vengono salvati in cache SQLite
- [ ] In assenza di rete, il catalogo mostra gli esercizi in cache
- [ ] Stati loading, error (con pulsante retry), empty gestiti

**Rischi**

- Formato risposta ExerciseDB API da validare (documentazione RapidAPI potrebbe essere datata)
- Limite chiamate/mese tier gratuito RapidAPI
- Performance scroll catalogo con GIF (lazy loading necessario)
- Gestione header RapidAPI: la chiave NON va versionata in chiaro

---

### IT-03 - Registrazione allenamento e storico

**Obiettivo verificabile**

L'utente può creare un allenamento, aggiungere esercizi con serie/peso/ripetizioni, salvarlo e consultare lo storico.

**In scope**

- Modello `Workout` con `Id`, `Date`, `Duration`, `Notes`, lista `WorkoutExercise`
- Modello `WorkoutExercise` con `ExerciseId`, `ExerciseName`, lista `ExerciseSet`
- Modello `ExerciseSet` con `SetNumber`, `WeightKg`, `Reps`
- `WorkoutRepository` SQLite: CRUD allenamenti
- `ActiveWorkoutViewModel`: gestione stato allenamento in corso, aggiunta/rimozione esercizi
- `WorkoutHistoryViewModel`: lista allenamenti passati ordinati per data
- `WorkoutDetailViewModel`: dettaglio di un allenamento passato
- Pagina `NewWorkoutPage`: selezione esercizi dal catalogo, aggiunta serie
- Pagina `ActiveWorkoutPage`: editing live delle serie durante l'allenamento
- Pagina `WorkoutHistoryPage`: elenco allenamenti passati
- Pagina `WorkoutDetailPage`: dettaglio allenamento
- Validazione input: peso > 0, ripetizioni > 0
- Salvataggio con conferma visiva

**Out of scope**

- Sincronizzazione con Firebase (IT-05)
- Statistiche aggregate (IT-07)

**File o aree probabili**

- `src/GymTracker.Mobile/Models/Workout.cs`
- `src/GymTracker.Mobile/Models/WorkoutExercise.cs`
- `src/GymTracker.Mobile/Models/ExerciseSet.cs`
- `src/GymTracker.Mobile/Data/WorkoutRepository.cs`
- `src/GymTracker.Mobile/ViewModels/ActiveWorkoutViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/WorkoutHistoryViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/WorkoutDetailViewModel.cs`
- `src/GymTracker.Mobile/Views/NewWorkoutPage.xaml`
- `src/GymTracker.Mobile/Views/ActiveWorkoutPage.xaml`
- `src/GymTracker.Mobile/Views/WorkoutHistoryPage.xaml`
- `src/GymTracker.Mobile/Views/WorkoutDetailPage.xaml`
- `docs/iterations/it-03-workout-logging.md`

**Criteri di accettazione**

- [ ] L'utente può avviare un nuovo allenamento vuoto
- [ ] L'utente può aggiungere esercizi dal catalogo all'allenamento
- [ ] Per ogni esercizio può aggiungere N serie con peso e ripetizioni
- [ ] Peso e ripetizioni sono validati (valori positivi)
- [ ] L'utente può rimuovere esercizi o serie
- [ ] Salvataggio su SQLite con data e ora
- [ ] Lo storico mostra allenamenti dal più recente
- [ ] Il dettaglio allenamento mostra tutti gli esercizi con le serie

**Rischi**

- Gestione stato attivo dell'allenamento durante navigazione (non perdere dati se l'utente cambia tab)
- Relazioni SQLite: Workout -> WorkoutExercise -> ExerciseSet vanno gestite correttamente

---

### IT-04 - Tracking peso corporeo e misure

**Obiettivo verificabile**

L'utente può registrare peso e misure corporee e visualizzarne l'andamento nel tempo.

**In scope**

- Modello `BodyWeight`: `Id`, `Date`, `WeightKg`
- Modello `BodyMeasurement`: `Id`, `Date`, `ChestCm`, `WaistCm`, `HipsCm`, `ArmsCm`, `LegsCm`
- `BodyRepository` SQLite per peso e misure
- `BodyViewModel`: input peso, input misure, lista storico
- Pagina `BodyTrackingPage` con form e storico
- Grafico a linee per andamento peso (custom drawing o Microcharts)
- Grafico per andamento misure

**Out of scope**

- Sincronizzazione remota di peso/misure (dati sensibili, restano locali)
- Confronto misure con amici
- Foto progresso

**File o aree probabili**

- `src/GymTracker.Mobile/Models/BodyWeight.cs`
- `src/GymTracker.Mobile/Models/BodyMeasurement.cs`
- `src/GymTracker.Mobile/Data/BodyRepository.cs`
- `src/GymTracker.Mobile/ViewModels/BodyViewModel.cs`
- `src/GymTracker.Mobile/Views/BodyTrackingPage.xaml`
- `docs/iterations/it-04-body-tracking.md`

**Criteri di accettazione**

- [ ] L'utente può inserire peso corporeo con data
- [ ] L'utente può inserire misure (petto, vita, fianchi, braccia, gambe) con data
- [ ] I dati sono salvati su SQLite locale
- [ ] Un grafico mostra l'andamento del peso nel tempo
- [ ] L'elenco storico mostra le ultime registrazioni
- [ ] I dati NON vengono mai inviati al backend

**Rischi**

- Scelta libreria grafici: Microcharts è leggera ma limitata, LiveChartsCore più potente ma più pesante
- Validazione input misure (valori positivi, range realistici)

---

### IT-05 - Autenticazione Firebase e sincronizzazione

**Obiettivo verificabile**

L'utente può registrarsi e fare login con Firebase Auth. Gli allenamenti vengono sincronizzati con Firebase Realtime Database / Firestore.

**In scope**

- Configurazione Firebase project (Auth + Database)
- Servizio `FirebaseAuthService`: `RegisterAsync`, `LoginAsync`, `LogoutAsync`, `GetCurrentUser`
- Modello `User` con `FirebaseUid`, `Username`, `Email`
- `UserRepository` SQLite per profilo locale
- Servizio `SyncService`: coda sincronizzazione, invio allenamenti a Firebase, retry
- `LoginViewModel` e `RegisterViewModel`
- Pagine `LoginPage` e `RegisterPage`
- Salvataggio token in `Preferences` per persistenza sessione
- Meccanismo di retry per sync dopo riconnessione

**Out of scope**

- Funzionalità social (amici, feed, leaderboard) — IT-06
- Login social (Google, Apple) — post-MVP
- Reset password (per MVP usare Firebase console)

**File o aree probabili**

- `src/GymTracker.Mobile/Services/FirebaseAuthService.cs`
- `src/GymTracker.Mobile/Services/SyncService.cs`
- `src/GymTracker.Mobile/Models/User.cs`
- `src/GymTracker.Mobile/Data/UserRepository.cs`
- `src/GymTracker.Mobile/ViewModels/LoginViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/RegisterViewModel.cs`
- `src/GymTracker.Mobile/Views/LoginPage.xaml`
- `src/GymTracker.Mobile/Views/RegisterPage.xaml`
- `docs/iterations/it-05-firebase-auth-sync.md`

**Criteri di accettazione**

- [ ] Registrazione con email/password crea account Firebase
- [ ] Login con credenziali valide reindirizza alla Dashboard
- [ ] Credenziali errate mostrano errore, credenziali vuote bloccano submit
- [ ] L'utente loggato rimane autenticato tra riavvii (token in Preferences)
- [ ] Allenamenti salvati localmente vengono sincronizzati con Firebase quando online
- [ ] Allenamenti creati offline vengono sincronizzati quando la rete torna
- [ ] Logout cancella token e riporta alla Login

**Rischi**

- **Alto**: Configurazione Firebase (google-services.json, Realtime Database rules) richiede setup manuale su Firebase Console
- **Alto**: Sync conflitti (due dispositivi stesso utente) — per MVP usare last-write-wins
- Chiavi Firebase da non versionare
- Dipendenza da connessione per login iniziale

---

### IT-06 - Amici, feed e leaderboard

**Obiettivo verificabile**

L'utente può aggiungere amici, vedere il loro feed allenamenti e la leaderboard settimanale.

**In scope**

- Servizio `SocialService`: `SearchUsers`, `SendFriendRequest`, `AcceptFriendRequest`, `RejectFriendRequest`, `GetFriends`, `GetFriendWorkouts`
- Modelli: `FriendRequest` (senderId, receiverId, status, timestamp)
- `SocialViewModel`: gestione ricerca, richieste, lista amici
- `FriendFeedViewModel`: feed ultimi allenamenti amici
- `LeaderboardViewModel`: classifica settimanale per volume
- `FriendCompareViewModel`: confronto diretto con un amico
- Pagine: `FriendsPage`, `FriendSearchPage`, `FriendFeedPage`, `LeaderboardPage`, `FriendComparePage`
- Logica streak: conteggio giorni consecutivi con allenamenti

**Out of scope**

- Notifiche push per richieste di amicizia (usare polling o refresh manuale per MVP)
- Chat tra amici

**File o aree probabili**

- `src/GymTracker.Mobile/Services/SocialService.cs`
- `src/GymTracker.Mobile/Models/FriendRequest.cs`
- `src/GymTracker.Mobile/Models/LeaderboardEntry.cs`
- `src/GymTracker.Mobile/ViewModels/SocialViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/FriendFeedViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/LeaderboardViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/FriendCompareViewModel.cs`
- `src/GymTracker.Mobile/Views/FriendsPage.xaml`
- `src/GymTracker.Mobile/Views/FriendSearchPage.xaml`
- `src/GymTracker.Mobile/Views/FriendFeedPage.xaml`
- `src/GymTracker.Mobile/Views/LeaderboardPage.xaml`
- `src/GymTracker.Mobile/Views/FriendComparePage.xaml`
- `docs/iterations/it-06-social-competition.md`

**Criteri di accettazione**

- [ ] Ricerca utenti per username funziona
- [ ] Invio richiesta di amicizia con feedback visivo
- [ ] Richieste pendenti mostrate con opzioni accetta/rifiuta
- [ ] Lista amici con nome e statistiche rapide (ultimo allenamento, streak)
- [ ] Feed mostra ultimi 20 allenamenti amici con data, esercizi, volume
- [ ] Leaderboard ordinata per volume settimanale decrescente
- [ ] Confronto diretto affianca peso max, volume e streak tra due utenti
- [ ] Streak calcolato correttamente

**Rischi**

- **Alto**: Richiede backend Firebase funzionante e popolato (IT-05 completata)
- Performance query Firebase per leaderboard con molti utenti
- Gestione stato vuoto: nessun amico, nessun allenamento amici

---

### IT-07 - Dashboard e statistiche

**Obiettivo verificabile**

Dashboard completa con riepilogo rapido, statistiche dettagliate e grafici di progresso.

**In scope**

- `DashboardViewModel`: aggregazione dati da SQLite e Firebase
- Riepilogo: ultimo allenamento, streak, posizione leaderboard, peso attuale
- Accesso rapido: pulsanti Nuovo Allenamento, Catalogo, Amici
- `StatsViewModel`: statistiche aggregate
- Grafico progressione peso per esercizio (peso max nel tempo)
- Grafico volume settimanale
- Navigazione a statistiche da Dashboard

**Out of scope**

- Obiettivi settimanali personalizzati (post-MVP)
- Export dati (post-MVP)

**File o aree probabili**

- `src/GymTracker.Mobile/ViewModels/DashboardViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/StatsViewModel.cs`
- `src/GymTracker.Mobile/Views/DashboardPage.xaml`
- `src/GymTracker.Mobile/Views/StatsPage.xaml`
- `docs/iterations/it-07-dashboard-stats.md`

**Criteri di accettazione**

- [ ] Dashboard mostra ultimo allenamento con data e volume
- [ ] Streak corrente visibile nella Dashboard
- [ ] Posizione in leaderboard aggiornata
- [ ] Peso corporeo più recente visibile
- [ ] Pulsanti rapidi funzionanti
- [ ] Statistiche mostrano peso max per esercizi principali
- [ ] Grafico volume settimanale visibile
- [ ] Dashboard si aggiorna dopo nuovo allenamento

**Rischi**

- Performance query aggregate SQLite con molti dati
- Refresh Firebase per posizione leaderboard

---

### IT-08 - Piani di allenamento base

**Obiettivo verificabile**

L'utente può consultare piani predefiniti e avviare un allenamento da un piano.

**In scope**

- Modello `WorkoutPlan` con `Name`, `Description`, lista `PlanDay`
- Modello `PlanDay` con `DayName`, lista `PlanExercise` (esercizio, serie, rep range)
- 3 piani base precaricati su SQLite:
  1. Full Body 2 giorni (A/B)
  2. Split 3 giorni (Push / Pull / Legs)
  3. Split 3 giorni (Petto-Bicipiti / Schiena-Tricipiti / Gambe-Spalle)
- `WorkoutPlanRepository` SQLite
- `PlansViewModel`: lista piani
- `PlanDetailViewModel`: dettaglio piano con giorni ed esercizi
- Pagine `PlansPage` e `PlanDetailPage`
- "Avvia Allenamento" da un giorno del piano: popola l'allenamento con gli esercizi del giorno

**Out of scope**

- Piani personalizzati creati dall'utente (post-MVP)
- Progressione automatica dei carichi

**File o aree probabili**

- `src/GymTracker.Mobile/Models/WorkoutPlan.cs`
- `src/GymTracker.Mobile/Models/PlanDay.cs`
- `src/GymTracker.Mobile/Models/PlanExercise.cs`
- `src/GymTracker.Mobile/Data/WorkoutPlanRepository.cs`
- `src/GymTracker.Mobile/ViewModels/PlansViewModel.cs`
- `src/GymTracker.Mobile/ViewModels/PlanDetailViewModel.cs`
- `src/GymTracker.Mobile/Views/PlansPage.xaml`
- `src/GymTracker.Mobile/Views/PlanDetailPage.xaml`
- `docs/iterations/it-08-workout-plans.md`

**Criteri di accettazione**

- [ ] 3 piani base precaricati e visibili nella pagina Piani
- [ ] Ogni piano mostra nome, descrizione e numero giorni
- [ ] Il dettaglio mostra gli esercizi per ogni giorno con range ripetizioni consigliato
- [ ] "Avvia Allenamento" da un giorno popola l'allenamento con gli esercizi del piano
- [ ] L'allenamento avviato segue il flusso standard (IT-03)

**Rischi**

- Coerenza nomi esercizi tra piani precaricati e catalogo ExerciseDB (mapping necessario)
- I piani usano nomi esercizi generici che devono matchare con l'API

## 4. Definition of Done per iterazione

Ogni iterazione deve soddisfare:

1. Codice compila senza errori (`dotnet build` OK)
2. Feature testabile manualmente su emulatore Android
3. Stati UI gestiti: loading, error, empty, success per ogni schermata
4. Dati persistono correttamente (SQLite) e, dove previsto, si sincronizzano (Firebase)
5. Documentazione iterazione aggiornata in `docs/iterations/it-XX-nome.md`
6. Test matrix aggiornata con evidenze

## 5. Prossimi passi

1. Avviare IT-01: creazione progetto MAUI + Shell + Design System Stitch
2. Configurare emulatore Android e verificare build
3. Setup Firebase project su console Firebase
4. Ottenere API key ExerciseDB da RapidAPI
