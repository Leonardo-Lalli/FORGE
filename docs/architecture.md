# Architettura FORGE

Codice di riferimento della 0.8.5 beta, 13 ottobre 2026: experiments promosso a main su richiesta del proprietario. Il tag v0.8.5-beta identifica la distribuzione; pubblicazione e deployment hanno verifiche separate.

## Componenti

- .NET 10 MAUI, Android API 23+, Shell e MVVM CommunityToolkit.Mvvm, DI e binding XAML compilati.
- SQLite: allenamenti/bozze, schede personali associate all’account, cache esercizi e achievement.
- PocketBase selezionato nelle impostazioni, con istanza pubblica predefinita. Account e social indipendenti per istanza.
- ExerciseDB OSS: metadati, istruzioni e URL media; i file GIF restano sul provider.
- Firebase FCM opzionale: registrazione del client, coda privata server e worker Python con chiave Admin solo sul server. Timer Android locale indipendente da FCM.
- Sito Next.js esportato staticamente, italiano/inglese, doppio tema. Download risolto dalle release GitHub (incluse beta), con fallback all’elenco release.

## Salvataggio e interfaccia

ActiveWorkoutViewModel salva l’allenamento locale prima dell’upload. WorkoutSavedMessage aggiorna profilo e statistiche; WorkoutHistory unisce lo storico remoto già disponibile ai salvataggi locali. SyncService gestisce allenamenti pendenti, senza promessa di sincronizzare ogni entità tra dispositivi.

PlanService gestisce schede SQLite. Le note vengono salvate separatamente dopo 400 ms di pausa e attese prima di minimizzare/terminare; scritture serializzate e limitate alle note degli esercizi corrispondenti. PlanSessionChanges confronta nome, struttura, serie e recuperi, ignorando note, completamento e suggerimenti. Aggiunte/sostituzioni richiedono conferma; la copia della scheda azzera lo stato della sessione.

## Privacy e community

API Rules e hook FORGE sono necessari oltre ai filtri UI. Allenamenti ed elenchi follower hanno visibilità separata: pubblico per utenti autenticati dell’istanza, privato, amici reciproci accettati. Like e aggiornamenti consentiti usano endpoint dedicati. Nessuna federazione, gestione trainer o separazione di più palestre sul medesimo server è implementata.

FeedViewModel mostra lo stato vuoto dopo il caricamento. HasOtherCommunityMembersAsync distingue zero utenti visibili da errore/stato sconosciuto. CommunityInvite apre la condivisione nativa al sito, includendo istruzioni per l’istanza personale senza credenziali. Non registra automaticamente utenti o follow.

Le regole aggiornate sono nei sorgenti tools/pocketbase/pb_migrations e nei relativi hook. Vedi [migrazioni](pocketbase-profile-migration.md); non copiare vecchi esempi di regole di produzione.

## Backend e distribuzione

Compose costruisce il binario ufficiale PocketBase della versione esplicitamente scelta, amd64/arm64; porta su loopback, init esplicito e migrazioni con backup. Worker push in profilo opzionale. Installer LXC differenziano /opt/forge-pocketbase (servizio forge-pocketbase) da Community Scripts /opt/pocketbase (servizio pocketbase).

Il workflow release richiede configurazione Firebase client e firma di produzione nei secret; prepara una bozza APK + archivio server. Il server domestico non garantisce disponibilità. Vedi [installazione](self-hosting.md), [push](push-notifications.md) e [preparazione beta](release-readiness.md).

Modalità senza account, sincronizzazione completa, gestione palestre e aggiornamenti nell’app sono proposte future, separate dall’implementazione attuale. [Roadmap](../ROADMAP.md).


## Stato dell’allenamento — 16 ottobre 2026

`ActiveWorkoutViewModel` è singleton per mantenere un’unica sessione durante la navigazione Shell; le pagine dell’allenamento restano transient. Il ViewModel reimposta gli stati delle finestre e verifica l’account prima di riutilizzare la sessione. `WorkoutSession`, registrato anche nelle risorse app, espone proprietà osservabili e comando di ripresa alla `MiniWorkoutBar` condivisa nelle pagine autenticate. Il login recupera soltanto i metadati della bozza per mostrare la barra; il contenuto viene caricato quando si apre l’allenamento.

La bozza usa un ID stabile. `DraftStateVersion`, `DraftElapsedSeconds` e `DraftRestSeconds` distinguono i nuovi snapshot dalle bozze precedenti, che usano ancora `Duration`. Il gate del ViewModel serializza autosalvataggi, eliminazione e completamento. `DatabaseService.CompleteWorkoutAsync` salva lo storico e rimuove la sua bozza nella stessa transazione SQLite; le altre bozze restano intatte. `WorkoutCompletion` centralizza la definizione di serie eseguita e il recupero della durata.

Gli eventi `Window.Stopped`/`Resumed` inoltrano al ViewModel la sospensione dei timer e il ritorno alla modalità minimizzata. Il code-behind della pagina intercetta soltanto il back nativo e lo inoltra al comando di conferma. Nessuna logica di persistenza nelle pagine. Un arresto improvviso può perdere le modifiche degli ultimi 200 ms o fino a cinque secondi del cronometro: la chiusura ordinaria richiede un salvataggio aggiuntivo.

## Sincronizzazione delle schede — 26 ottobre 2026

SavedPlan aggiunge PendingSync e IsDeleted; PlanService conserva il salvataggio locale come confine di successo, serializza scritture/sync con planWriteGate e sincronizza in lettura, dopo modifiche/note/cancellazioni e alla riconnessione tramite SyncService. Una migrazione per utente mette in coda anche le schede preesistenti. Le schede dimostrative non vengono generate se il cloud non risponde e non ci sono dati locali: evita di mascherare schede ancora da recuperare. Le righe cancellate sono nascoste dalla UI ma conservate per propagare la cancellazione.

POST `/api/forge/plans/sync` richiede autenticazione users, controlla il proprietario, valida il batch prima della transazione e conserva una riga privata forge_plans per coppia utente/ID locale. Nessun accesso diretto alle API CRUD della collezione. Il payload conserva il modello completo; relazione utente con cascadeDelete. Conflitti risolti per data UTC di modifica, preservando precisione sotto il millisecondo nei timestamp .NET. Il client richiede una risposta con conferma di tutti gli ID inviati prima di segnare le copie locali come sincronizzate; risposte per account diversi vengono rifiutate.

La nuova PlanSyncStatus usa il PlanService singleton come contesto e mostra errori/retry in Home e StartSession. MiniWorkoutBar mostra MiniRestLabel anziché ElapsedTime; la durata rimane disponibile soltanto dentro l’allenamento. Vedi `docs/account-plan-sync.md` per installazione e limiti (orologi dei dispositivi, conflitti, nessuna cronologia di backup, bozza allenamento ancora locale).

## Unità di peso e storico roadmap — 26 ottobre 2026

WeightUnitService singleton persiste weight_unit nelle Preferences e trasmette WeightUnitChangedMessage tramite WeakReferenceMessenger. I ViewModel ricevono il servizio per DI e riformattano i propri dati numerici in cache senza richieste HTTP. Le registrazioni deboli evitano di trattenere pagine transient. Tutta la persistenza resta in kg, con fattore esatto 1 lb = 0,45359237 kg.

ExerciseSet espone DisplayWeight e PreviousDisplay ignorati da JSON/SQLite: la prima proiezione è bidirezionale e protegge dal writeback del valore arrotondato quando cambia l’unità; il valore kg non viene ricalcolato a ogni toggle. I dettagli/volumi ricalcolano solo le stringhe di visualizzazione. I progressi di forza/volume cambiano valore ma non l’altezza relativa delle barre. Soglie achievement e rapporti di avanzamento rimangono canonici.

CsvWorkoutWriter esporta le serie disponibili nell’unità selezionata, preserva i riepiloghi legacy e non esporta le bozze. SplitRows/ParseRow preservano note multilinea, virgole e virgolette. CsvImportService riconverte WeightLb/WeightLbs/PesoLb in kg; colonne kg e generiche mantengono la semantica storica. Il CSV non costituisce backup completo di foto, schede o stato dell’app.

La roadmap ha ID stabili, status, implementedOn, updatedOn, implementationRef e history. tools/check-roadmap.mjs verifica date (giorno Europe/Rome), riferimenti, coerenza bilingue e nuove modifiche rispetto a HEAD quando Git è disponibile. Il build del sito esegue il controllo prima di Next.js/TypeScript. docs/roadmap-policy.md definisce l’obbligo per le iterazioni future.

## Android Release: registrazione nativa — 4 novembre 2026

`MainApplication.OnCreate` dichiara esplicitamente il callback di avvio e delega a `MauiApplication.OnCreate`. Il wrapper JNI viene generato sul tipo concreto anziché dipendere soltanto da quello ereditato. Non contiene logica di dominio o navigazione. La configurazione Release mantiene le ottimizzazioni Android standard; verificare l’APK firmato a runtime dopo cambi di SDK e rigenerare i file intermedi quando cambia la configurazione JNI. L’errore iniziale osservato è `MauiApplication.n_onCreate` mancante. Riferimento tecnico: https://github.com/dotnet/maui/issues/35209.
