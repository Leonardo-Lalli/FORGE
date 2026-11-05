# Contesto tecnico e di prodotto FORGE

Snapshot del 5 novembre 2026. Questo documento descrive il progetto: non attribuisce alla chat web accesso automatico al repository o al telefono. Per analisi puntuali usare anche i file sorgente di testo del pacchetto.

## Versioni e decisioni del proprietario

- Repository: https://github.com/Leonardo-Lalli/FORGE. Sito: https://leonardo-lalli.github.io/FORGE/.
- `main`: versione approvata dal proprietario, commit `9cab71b`, tag pubblico `v0.8.5-beta`, APK build **89**. Nome FORGE 0.8.5-beta. È la release corrente/Latest; il flag GitHub prerelease è disattivato per consentire Latest, ma il prodotto resta beta.
- `develop`: modifiche successive richieste il 5 novembre, APK **0.8.5-beta-develop**, versionCode **90**. Non è una nuova release pubblica. Il proprietario vuole provarlo prima di un’altra promozione.
- Dispositivo di riferimento del proprietario: Samsung A15 4G, ultima One UI disponibile per lui; versione Android esatta non dichiarata.
- Il proprietario apprezza l’app attuale: preservare interfaccia, funzionalità e dati. Chiede miglioramenti mirati, non una riscrittura completa.
- Le precedenti etichette locali 0.8.6/0.8.7 erano prove; non devono diventare automaticamente nuove release.

## Prodotto

Diario di allenamento Android open source, nato come progetto scolastico e cresciuto con prove in palestra. Schede, sessioni libere, recupero, storico, progressi, foto, achievement e community sullo stesso server. Gli account di istanze PocketBase diverse sono separati: non c’è federazione.

Flusso: setup del server pubblico o personale → autenticazione email/password → Dashboard/Feed/Stats tramite Shell. Dashboard e StartSession aprono una scheda, una sessione libera, la creazione di una scheda o il recupero di una bozza. Una sessione libera non deve adottare accidentalmente esercizi o PlanId della bozza precedente.

Funzionalità esistenti: serie normali/warm-up/drop/failure, suggerimenti dai carichi precedenti, consigli di esecuzione, note multilinea, temi chiaro/scuro, foto, profilo, follow/like, visibilità pubblica/privata/amici reciproci. Achievement retroattivi supportati dallo storico; 100 XP per badge, un livello ogni 500 XP. Non tutti i criteri del catalogo sono implementati. Google/Microsoft/Apple login, trainer/B2B, wearable, AI form check e aggiornamento automatico dall’app sono obiettivi di roadmap, non funzioni disponibili.

## Stack e struttura reale

.NET 10 MAUI, Android-first, MVVM con CommunityToolkit.Mvvm 8.4.2. XAML con `x:DataType` e `MauiXamlInflator=SourceGen`. Target Android `net10.0-android`, minimo API 23, compilazione locale target API 36. Il progetto dichiara anche target iOS/MacCatalyst/Windows condizionati al sistema host: non assumere che siano collaudati o distribuiti.

Dipendenze rilevanti dal csproj: Microsoft.Maui.Controls con MauiVersion del workload, Microsoft.Extensions.Http/Logging.Debug 10.0.9, sqlite-net-base 1.9.172, SQLitePCLRaw.bundle_e_sqlite3 3.0.5, Firebase Messaging Android 125.1.3 e relativi AndroidX. Le versioni vanno verificate nel file, non aggiornate a memoria.

```text
src/Forge/
  App.xaml.cs                 finestra, tema, avvio e salvataggio al background
  AppShell.xaml(.cs)          tab Dashboard/Feed/Stats e rotte
  MauiProgram.cs             registrazioni DI, font, handler Android
  Views/                     pagine XAML; code-behind principalmente wiring
  ViewModels/                stato, comandi e navigazione delle schermate
  Models/ e Models/Dto/       dominio locale, modelli UI e payload server
  Services/                  SQLite, PocketBase, sync, CSV, timer, statistiche
  Controls/                  mini barra, stato sync, selettore recupero
  Platforms/Android/         applicazione, activity, timer service, receiver, push
  Resources/                 temi, icone, font, media
tests/Forge.Tests/           xUnit net10.0 con sorgenti condivise e stub MAUI
tools/pocketbase/            hook e migrazioni del backend
tools/                      installer, worker push, audit e controlli pacchetti
sitoGithub/                 Next.js 16.2.6, React 19, TypeScript, Tailwind
docs/                       specifica, architettura, piano, test e guide server
.github/workflows/          test, release manuale, Pages e monitor server
```

Rotte registrate: activeWorkout, settings, profile, startSession, login, friendRequests, workoutDetail, achievements. Le tab sono dashboard, feed e stats. Le radici non autenticate Setup/Login sono assegnate a Window.Page nel flusso esistente; le pagine autenticate navigano con Shell. Non descrivere il codice esistente come se usasse un’architettura diversa.

DI: ActiveWorkoutViewModel, WorkoutSession, servizi DB/rete/sync/tema/unità sono singleton; gli altri ViewModel e le pagine sono principalmente transient. Attenzione a reset dello stato, handler e sottoscrizioni quando una sessione o un account cambiano.

## Sessioni, serie e timer

`ActiveWorkoutViewModel` gestisce modalità free/plan/create/resume, esercizi, ricerca, serie, media, dialoghi, timer e completamento. È un file esteso: possibile area di refactoring, da misurare e suddividere preservando i flussi.

`WorkoutSession` espone stato condiviso e mini barra. `MiniWorkoutBar` è presente nelle pagine autenticate e mostra il **recupero**, non la durata dell’allenamento. Minimizzare permette di navigare nell’app. La X/Indietro richiede una scelta esplicita per evitare perdita involontaria.

Le bozze sono record `LocalWorkout.IsDraft`, identificati stabilmente e isolati per UserId. Salvataggi serializzati tramite gate; debounce delle modifiche, salvataggi periodici e su background. `CompleteWorkoutAsync` salva lo storico ed elimina solo la bozza della sessione in una transazione. Un esercizio conta se ha almeno una serie confermata con Reps > 0; corpo libero con peso zero è valido.

In develop il background **salva senza mettere in pausa**: il tempo dell’allenamento deriva da startTime e quello di recupero dalla scadenza del servizio. Tornando nell’app i valori vengono riallineati; la pausa resta una scelta manuale. Minimizzare dentro l’app mantiene il countdown. Il recupero di una bozza dopo la ricreazione completa del processo conserva il precedente comportamento di ripristino fermo: non confondere background con force-stop, riavvio del telefono o nuova istanza del processo.

Android: `ForgeRestTimer` conserva deadline monotona basata su SystemClock.ElapsedRealtime e flag pausa nelle Preferences; `ForgeRestTimerService` è foreground specialUse con notifica progressiva, chronometer e wake lock limitato. AlarmManager e receiver gestiscono fine/pausa/salta. `NotificationService` fa da facciata e ha fallback non Android. Notifica locale del recupero indipendente da Firebase. Permessi Android per notifiche/allarmi e restrizioni del produttore richiedono prove reali; non promettere esecuzione indefinita dopo force-stop del sistema.

Develop aggiunge eliminazione della singola serie, rinumerazione e ricalcolo dei conteggi, anche quando resta un esercizio senza serie. Il filtro avambracci considera muscoli primari e alias del catalogo; non deve includere bicipiti soltanto perché la regione è genericamente il braccio.

## Modifica delle schede

`WorkoutPlan` contiene UserId, Id, Name, Notes, CreatedAt/UpdatedAt, RestSeconds e lista WorkoutExercise. Ogni esercizio ha un Id della riga distinto dall’ExerciseId del catalogo, note, ordine, recupero, media/istruzioni e serie. Ogni serie ha Id, SetNumber, Kind, WeightKg, Reps e stato della sessione.

`PlanSessionChanges` produce firma strutturale, copie indipendenti e riepilogo contestuale. Le note si salvano automaticamente sugli esercizi ancora corrispondenti per identità; non devono aggiornare le note dell’esercizio precedente dopo una sostituzione.

In develop si può cambiare soltanto il nome e terminare senza spuntare serie. Il popup elenca solo le differenze presenti (nome, esercizi aggiunti/rimossi/sostituiti, serie aggiunte/rimosse, carichi, ripetizioni, tipo, ordine, recuperi). Nome-only non deve mostrare modifiche a esercizi o serie. Se non è stata eseguita alcuna serie, il salvataggio aggiorna la scheda e chiude la bozza senza creare un workout fittizio. Con serie eseguite resta distinta la scelta di aggiornare la scheda dal salvataggio dell’allenamento.

## Persistenza, identità e sincronizzazione

`DatabaseService` apre SQLite in AppDataDirectory/forge.db con inizializzazione protetta. Tabelle principali: local_workouts, cached exercises, saved plans e achievement state. Le migrazioni additive tramite CreateTable devono preservare dati esistenti.

`LocalWorkout`: UserId, PlanId, Name, Date, ExercisesJson, ExerciseDataJson, Volume in kg, Duration in minuti, Notes, PhotosJson, likes, PendingSync/LastSynced; bozze con DraftElapsedSeconds, DraftRestSeconds, DraftStateVersion. Non caricare le bozze come workout completati.

`SavedPlan`: metadati proprietario, ExercisesJson, recupero, PendingSync e IsDeleted. `PlanService` salva prima localmente, migra vecchie Preferences, accoda modifiche offline e tombstone. `/api/forge/plans/sync` è autenticato e privato per account; conflitti gestiti per timestamp UTC, conferme server verificate per proprietà e presenza degli elementi. `PlanSyncStatus` rende visibile quando manca la copia remota.

`PocketBaseService` + partial Profiles/Plans centralizzano HTTP, autenticazione, profili e account. HttpClientFactory e CertificatePinningHandler per il backend. SecureStorage/Preferences per stato locale dove previsto dal codice. `SyncService` sincronizza solo utente corrente, ignora bozze, serializza le esecuzioni e usa ID stabili tramite WorkoutSyncPayload. `WorkoutHistory` unisce locale/remoto evitando duplicati per identità normalizzata. Non mescolare dati di account diversi; verificare anche i cambi account durante richieste in volo.

La sincronizzazione schede richiede hook e migrazione `1792972800_forge_plans.js`. La pubblicazione GitHub **non aggiorna il server reale**. La presenza del codice non prova che il backend personale/pubblico sia già migrato. Disinstallare cancella i dati locali: le schede eliminate prima del primo backup remoto non sono recuperabili automaticamente.

## Unità, CSV e statistiche

Tutti i dati persistiti, i confronti e i payload restano in **kg**. `WeightUnitService` gestisce kg/lb in UI, input e CSV. Le proprietà DisplayWeight/PreviousDisplay non diventano campi persistiti. Cambio unità via messaggi deboli e aggiornamento delle viste; evitare doppie conversioni e drift dopo toggle ripetuti.

`CsvWorkoutWriter` esporta una riga per serie, intestazioni WeightKg/WeightLb, quote escapate e note multilinea; supporta riepiloghi legacy privi di dettaglio. Non è un backup completo delle schede o delle foto e non conserva la durata reale: l’importer la stima dalle righe.

Develop: limite **5 MiB**, niente limite di righe; lettura streaming bounded anche su stream non seekable e conteggio UTF-8 nel servizio. Colonne Date/ExerciseName obbligatorie, WorkoutName/Notes opzionali; righe consecutive della stessa data e nome raggruppate in un workout. `CsvImportService` salva localmente e usa l’upload stabile esistente; fallimenti remoti restano in coda. I test coprono oltre 1.000 righe, oltre cinque sessioni, volume, optional columns e limite in byte. Non assumere compatibilità universale con formati Hevy/Strong o dialetti separati da punto e virgola: controllare il parser. Il campo Sets e le garanzie di round trip vanno analizzati nel sorgente prima di promettere supporto a formati aggregati esterni.

Stats usa `WorkoutHistory`, `StatsPeriod` e `VolumeChart`. In develop, filtro mese con mese selezionato e due limiti [primo giorno, primo giorno del mese successivo). Freccia sinistra/swipe sinistra vanno al mese precedente, destra al successivo fino al corrente. Conteggi, ore, volume, grafico, top lifts e notifiche like seguono lo stesso periodo. Calendario sempre di 42 celle/sei righe. Periodi settimana, tre mesi, anno e totale mantengono il loro significato; non limitare un mese storico con la data di oggi o includere il mese successivo.

## Backend e sito

PocketBase self-hosted, endpoint pubblico configurato nel client e URL personale modificabile. Migrazioni: profili/privacy, cache catalogo, connessioni, account/notifiche e schede. I dati esercizi/media arrivano dal catalogo/cache e dal provider esterno; disponibilità/duplicati vanno considerati.

Docker Compose espone PocketBase sulla porta 8090 con bind di default loopback, volumi persistenti e hook. Init e migrazioni sono espliciti; push è un profilo opzionale con worker Python e chiave Firebase Admin montata solo server. Installer distinti per Docker, LXC esistente e creazione Proxmox; non è un’integrazione ufficiale Community Scripts. Non proporre comandi generici che sovrascrivono database o segreti.

Sito Next.js con export statico, traduzioni it/en e roadmap con Id, status, implementedOn, updatedOn, implementationRef, implementationNote e history. Non reintrodurre difficoltà alta/media. `tools/check-roadmap.mjs` verifica coerenza, date e storico. I nuovi elementi di develop sono candidate, non released.

## Build, test e aggiornamenti

`dotnet test tests/Forge.Tests/Forge.Tests.csproj --no-restore`: xUnit con sorgenti condivise, SQLite temporanei e stub MAUI. Non è un test UI completo dell’app Android. Baseline approvata: 119 test; dopo le nuove modifiche: 138 passati al momento della preparazione. `npm run build` verifica roadmap/TypeScript/export statico.

APK di release: publish net10.0-android Release, APK, trimming/AOT disattivati, assembly embedded e chiave stabile privata. MainApplication dichiara OnCreate esplicito. Un vecchio APK aveva wrapper JNI privi delle funzioni native: compilare non bastava. Usare `tools/publish-android.ps1`, che elimina gli intermedi Release Android, pubblica e verifica il file confezionato ARM64/x64 prima di copiarlo. Le build incrementali hanno riprodotto il difetto; non rimuovere questo controllo senza nuove evidenze. Verifica separata con `tools/test-android-startup.ps1 -PackageOnly -Apk <file>`. Lo stesso script ha modalità runtime su emulator serial per due aperture e controllo UI.

ApplicationId **com.leoforge.forge**. Per aggiornare senza disinstallare: stesso ApplicationId e certificato, versionCode crescente. Fingerprint pubblico del certificato SHA-256: `87098c4bf774e2e2512d0635ef6f192de0939de844bc5953f4e6f142a04ebe87`. Non generare un’altra chiave e non inserire password, keystore, google-services.json o chiavi Admin nel contesto.

Una prova precedente dell’emulatore Android 16 era inconclusiva per ANR anche di System UI/Bluetooth; non dichiararla passata. Il proprietario ha approvato la build 89 sul proprio uso dell’app. Le nuove interazioni di build 90 richiedono prova sul telefono. Test, firma e compilazione sono evidenze diverse, da riportare separatamente.

## Vincoli per ottimizzazioni future

Intervenire in develop con una slice alla volta: piano, criteri misurabili, implementazione, review, test, documentazione. Niente nuovi pacchetti, migrazioni invasive, framework UI o riscritture senza motivazione concreta. Rispettare MVVM, DI, binding compilati e Shell; non spostare logica in OnAppearing/code-behind. Distinguere caricamento, errore e vuoto. Non introdurre cicli di rete o timer ridondanti e non fare parsing pesante sul thread UI senza misurare.

Prima di ottimizzare raccogliere evidenze su avvio a freddo, tempo/allocazioni del catalogo e ricerca, CollectionView, ActiveWorkoutViewModel singleton e sottoscrizioni, salvataggi debounce/gate, import CSV e upload seriali, riletture SQLite/rete, download media e consumo del timer. Questi sono **punti da analizzare**, non colli di bottiglia già dimostrati. Proporre metriche e strumenti; non inventare benchmark.

Mai cancellare dati dell’utente per risolvere un bug. Server live, main/tag/release sono destinazioni da modificare solo quando autorizzate. L’approvazione del 5 novembre riguardava build 89; non autorizza automaticamente la pubblicazione delle nuove funzioni di develop.
