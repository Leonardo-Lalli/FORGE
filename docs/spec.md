# Specifica di Progetto - FORGE

> Stato corrente al 13 ottobre 2026: beta in preparazione sul branch experiments. Le sezioni iniziali conservano la pianificazione storica; per implementazione e limiti attuali consulta [architettura](architecture.md), [preparazione beta](release-readiness.md) e l’aggiornamento finale di questo documento. Il catalogo corrente usa ExerciseDB OSS, non RapidAPI; SQLite e SecureStorage sostituiscono le precedenti ipotesi di persistenza.

## 1. Visione e contesto

### Problema da risolvere

Chi va in palestra fatica a tenere traccia dei progressi, non ricorda i pesi usati la volta precedente, non sa quali esercizi esistano per un dato muscolo e manca un modo semplice per monitorare l'evoluzione del proprio fisico. Inoltre, allenarsi da soli riduce la motivazione: la competizione sana con gli amici spinge a migliorarsi, ma manca uno strumento semplice che unisca tracking e social in un'unica app.

### Obiettivo del progetto

Realizzare una applicazione `.NET MAUI` Android-first per:
- esplorare un catalogo di esercizi da **ExerciseDB API** (RapidAPI) con ricerca per nome, gruppo muscolare e attrezzatura;
- registrare allenamenti completi con serie, ripetizioni e peso, salvandoli localmente e sincronizzandoli con il backend;
- visualizzare statistiche di progresso e grafici;
- connettersi con amici, confrontare statistiche e competere via **feed** e **like**;
- consultare e salvare piani di allenamento personalizzati.

### Utenti target

- principianti che iniziano ad allenarsi in palestra e necessitano di struttura;
- appassionati di fitness che vogliono tracciare i progressi nel tempo;
- gruppi di amici che vogliono motivarsi a vicenda con competizione sana.

## 2. Ambito MVP

### Flusso principale da supportare

1. L'utente apre l'app, si registra o effettua login via PocketBase.
2. L'utente atterra sulla **Dashboard** con streak settimanale, squad activity e accesso rapido alle funzioni.
3. Dalla Dashboard, l'utente avvia un **Nuovo Allenamento** (Start Session) e cerca esercizi dal catalogo ExerciseDB (API remota).
4. L'utente registra serie, peso e ripetizioni, con rest timer configurabile.
5. L'utente salva l'allenamento su PocketBase.
6. L'utente visualizza le **Statistiche** personali: grafico volume, top lifts, calendario allenamenti.
7. L'utente cerca e segue altri utenti, vede il loro **Feed** di allenamenti e mette **like**.
8. L'utente gestisce il proprio **Profilo** con avatar, bio e storico allenamenti.

### Funzionalità implementate

- Autenticazione utente via PocketBase (email/password/username) con auto-login
- Catalogo esercizi da ExerciseDB API con ricerca e filtri (gruppo muscolare, attrezzatura)
- Cache esercizi su PocketBase con risoluzione URL immagini
- Registrazione allenamento con data, esercizi, serie, ripetizioni e peso
- Rest timer configurabile per esercizio
- Salvataggio allenamenti su PocketBase (`logged_workouts`)
- Dashboard con streak settimanale, squad activity, today card
- Gestione amici: ricerca, follow, richieste, accettazione/rifiuto
- Feed allenamenti amici con like/unlike
- Statistiche con grafico volume a barre, top lifts, calendario, filtri temporali
- Profilo utente con avatar (upload), bio, storico allenamenti
- Friend requests con notifiche like
- Piani di allenamento salvabili (PlanStore)
- Avvio allenamento libero (Quick Start) o da piano salvato
- Gestione esplicita di loading state, error state, empty state e success state
- Doppio tema chiaro/scuro con toggle runtime
- UI progettata con Stitch (Google) — tema "Cyber-Athletic Elite" (scuro) / "Fitness Core" (chiaro), font Inter/Lexend/Space Grotesk

### Funzionalità opzionali future

- post-MVP 1: tracciamento peso corporeo e misure (IT-04 non implementata)
- post-MVP 2: leaderboard settimanale
- post-MVP 3: confronto diretto statistiche con amico
- post-MVP 4: obiettivi settimanali personalizzati e notifiche reminder
- post-MVP 5: export dati in formato CSV/PDF
- post-MVP 6: foto progresso fisico con confronto temporale

### Non-obiettivi

- integrazione con dispositivi smart (orologi, bilance, fasce cardio);
- pagamento o abbonamenti premium;
- recensioni o rating sociali pubblici;
- chat o messaggistica tra amici;
- supporto iOS completo nel primo rilascio (Android-first);
- leaderboard e streak tra amici;
- piani di allenamento base predefiniti (sostituiti da piani custom salvabili).

## 3. Scenari d'uso principali

### Scenario 1: Primo allenamento

1. Marco apre l'app, si registra con email, password e username via PocketBase.
2. Dalla Dashboard, tocca "START WORKOUT".
3. Nella Start Session, sceglie Quick Start.
4. Cerca "panca" nel catalogo esercizi (fetch da ExerciseDB + cache PocketBase).
5. Seleziona "Barbell Bench Press", aggiunge 4 serie: 60kg x 10, 70kg x 8, 80kg x 6, 85kg x 4.
6. Aggiunge altri esercizi, configura il rest timer, completa le serie.
7. Salva l'allenamento e torna alla Dashboard aggiornata.

### Scenario 2: Competizione tra amici

1. Marco cerca Luca per username nel tab Feed.
2. Marco invia richiesta di follow a Luca.
3. Luca riceve la richiesta nella pagina Notifiche e accetta.
4. Marco vede nel Feed gli allenamenti di Luca con nome, volume e durata.
5. Marco mette like all'allenamento di Luca.
6. Luca riceve una notifica like.
7. Nella Dashboard, Luca appare nella Squad Activity di Marco.

### Scenario 3: Statistiche personali

1. Marco apre il tab Stats dopo alcune settimane di allenamento.
2. Vede il grafico volume a barre con filtro mensile.
3. Consulta i Top Lifts (peso max per esercizio).
4. Nel calendario, vede i giorni in cui si è allenato.
5. Filtra per periodo: WEEK, MONTH, 3M, YEAR, ALL.

## 4. Requisiti funzionali

- FR-01: l'app recupera esercizi da ExerciseDB API (RapidAPI) con endpoint per lista e ricerca. \[IMPLEMENTATO]
- FR-02: l'app mantiene una cache esercizi su PocketBase con URL immagini risolti. \[IMPLEMENTATO]
- FR-03: l'utente può avviare un nuovo allenamento (Quick Start) o da piano salvato. \[IMPLEMENTATO]
- FR-04: per ogni esercizio, l'utente può aggiungere serie con peso (kg) e ripetizioni. \[IMPLEMENTATO]
- FR-05: l'app salva l'allenamento completo su PocketBase con data, volume, durata, esercizi. \[IMPLEMENTATO]
- FR-06: l'utente può visualizzare lo storico allenamenti nel Profilo. \[IMPLEMENTATO]
- FR-07: ~~l'utente può registrare peso corporeo e misure~~ → NON IMPLEMENTATO (IT-04 posticipata)
- FR-08: l'app mostra statistiche di progresso: grafico volume, top lifts, calendario. \[IMPLEMENTATO]
- FR-09: l'utente può registrarsi e autenticarsi via PocketBase (email/password/username). \[IMPLEMENTATO]
- FR-10: l'utente può cercare altri utenti per username e inviare richieste di follow. \[IMPLEMENTATO]
- FR-11: l'utente può accettare o rifiutare richieste di follow ricevute. \[IMPLEMENTATO]
- FR-12: l'utente può vedere la lista degli amici (following) nella Squad Activity. \[IMPLEMENTATO]
- FR-13: l'utente può vedere un Feed con gli ultimi allenamenti degli amici e mettere like. \[IMPLEMENTATO]
- FR-14: ~~leaderboard settimanale~~ → NON IMPLEMENTATO (sostituito da feed + like)
- FR-15: l'app traccia e mostra lo Streak settimanale per l'utente in Dashboard e Profilo. \[IMPLEMENTATO]
- FR-16: ~~confronto diretto statistiche con amico~~ → NON IMPLEMENTATO (post-MVP)
- FR-17: l'utente può creare e salvare piani di allenamento personalizzati (PlanStore). \[IMPLEMENTATO]
- FR-18: l'app mostra sempre stati UI chiari: loading, success, empty, error su ogni schermata. \[IMPLEMENTATO]
- FR-19: ~~sincronizzazione offline-online~~ → NON IMPLEMENTATO (app richiede connessione)
- FR-20: la Dashboard mostra: streak corrente, squad activity, today card, START WORKOUT. \[IMPLEMENTATO]

## 5. Epic, user stories e criteri di accettazione

### EPIC-01 - Autenticazione e profilo utente ✅

**User stories:**
- Come nuovo utente, voglio registrarmi con email, password e username.
- Come utente registrato, voglio fare login e rimanere autenticato tra le sessioni.

**Criteri di accettazione:**
- [x] La registrazione crea un account PocketBase.
- [x] Il login con credenziali valide porta alla Dashboard.
- [x] Credenziali errate mostrano un messaggio di errore.
- [x] L'utente autenticato rimane loggato tra le sessioni (auto-login).
- [x] Logout cancella credenziali e riporta alla Login.

### EPIC-02 - Catalogo esercizi (ExerciseDB API) ✅

**User stories:**
- Come utente, voglio cercare esercizi per nome, gruppo muscolare e attrezzatura.
- Come utente, voglio vedere le immagini e i dettagli di ogni esercizio.

**Criteri di accettazione:**
- [x] Ricerca esercizi da ExerciseDB API con filtri (muscolo, attrezzatura).
- [x] Immagini esercizi caricate con risoluzione URL (redirect da short URL).
- [x] Cache esercizi su PocketBase per accesso rapido.
- [x] Filtri chip orizzontali: 10 gruppi muscolari + 7 attrezzature.

### EPIC-03 - Registrazione allenamento ✅

**User stories:**
- Come utente, voglio avviare un allenamento dalla Dashboard o Start Session.
- Come utente, voglio aggiungere esercizi, registrare serie/peso/ripetizioni.
- Come utente, voglio salvare l'allenamento e vederlo nello storico.

**Criteri di accettazione:**
- [x] START WORKOUT dalla Dashboard apre Start Session.
- [x] Quick Start avvia allenamento libero, Create New Plan crea piano salvabile.
- [x] Per ogni esercizio, aggiunta N serie con peso e ripetizioni.
- [x] Checkmark ✓ per segnare serie completata.
- [x] Rest timer configurabile (5-600s) per esercizio.
- [x] Salvataggio allenamento su PocketBase con dati completi.
- [x] Allenamento visibile in Profilo e Stats.

### EPIC-04 - Storico e dettaglio allenamento ✅ (parziale)

**User stories:**
- Come utente, voglio vedere gli allenamenti passati nel mio Profilo.

**Criteri di accettazione:**
- [x] Profilo mostra Recent Forges con data, esercizi, volume.
- [ ] Dettaglio allenamento completo con tutte le serie (non implementato).

### EPIC-05 - Misure corporee ❌

**Stato:** NON IMPLEMENTATO. Rinviato a post-MVP.

### EPIC-06 - Dashboard e statistiche ✅

**User stories:**
- Come utente, voglio vedere un riepilogo all'apertura dell'app.
- Come utente, voglio vedere statistiche dettagliate con grafici.

**Criteri di accettazione:**
- [x] Dashboard mostra streak settimanale, squad activity, today card.
- [x] START WORKOUT con accesso rapido.
- [x] Stats mostrano grafico volume a barre (settimanale, mensile, etc.).
- [x] Top Lifts con pesi massimi per esercizio.
- [x] Calendario con giorni di allenamento.
- [x] Filtri temporali: WEEK, MONTH, 3M, YEAR, ALL.

### EPIC-07 - Piani di allenamento ✅

**User stories:**
- Come utente, voglio creare e salvare piani di allenamento personalizzati.
- Come utente, voglio avviare un allenamento da un piano salvato.

**Criteri di accettazione:**
- [x] Piani salvabili con nome, esercizi, serie, pesi, reps.
- [x] 3 piani demo precaricati: Push Power, Leg Day Protocol, Core Stabilization.
- [x] Eliminazione piani salvati.
- [x] Avvio allenamento da piano con esercizi e pesi pre-popolati.

### EPIC-08 - Social e competizione ✅

**User stories:**
- Come utente, voglio cercare altri utenti e seguirli.
- Come utente, voglio vedere il feed allenamenti dei miei amici.
- Come utente, voglio mettere like agli allenamenti.
- Come utente, voglio ricevere notifiche di like e richieste.

**Criteri di accettazione:**
- [x] Ricerca utenti live con debounce 400ms.
- [x] Invio richiesta di follow e accettazione/rifiuto.
- [x] Feed mostra allenamenti amici con nome, volume, durata, esercizi.
- [x] Like/unlike con toggle ♥ e conteggio istantaneo.
- [x] Friend Requests con notifiche like aggregate.
- [x] Squad Activity nella Dashboard mostra avatar amici.
- [x] Avatar utente visibile in Dashboard, Feed, Stats e Profilo.

## 6. Requisiti non funzionali

### UX e stati UI

- Ogni schermata espone uno stato UI chiaro: loading (ActivityIndicator), success (dati presenti), empty (messaggio + azione suggerita), error (messaggio + retry).
- L'utente può avviare un allenamento dalla Dashboard con massimo 2 tap (START WORKOUT → Quick Start).
- Il design dell'interfaccia segue il doppio tema "Cyber-Athletic Elite" (scuro, ciano `#00E5FF`) e "Fitness Core" (chiaro, blu `#003ec7`), font Inter/Lexend/Space Grotesk.
- Toggle tema chiaro/scuro runtime, con propagazione a tutte le pagine via DynamicResource.

### Prestazioni percepite

- La Dashboard si carica in meno di 1 secondo (dati PocketBase).
- Il catalogo esercizi da API mostra risultati entro 3 secondi (con cache PocketBase).
- Il salvataggio di un allenamento è percepito come istantaneo.
- Le statistiche aggregate sono calcolate lato client.

### Affidabilità e gestione errori

- Errori di rete (API ExerciseDB o PocketBase) mostrano messaggi comprensibili con opzione retry.
- Timeout API dopo 10 secondi con messaggio di errore.
- Il parsing JSON è robusto (JsonDocument per PocketBase, case-insensitive per exercise data).
- Le credenziali PocketBase scadute triggerano un re-login guidato.

### Privacy e dati

- Dati di allenamento e profilo salvati su PocketBase self-hosted.
- L'API key di ExerciseDB è iniettata via `.env` a build-time, mai versionata in chiaro.
- Nessun dato condiviso con terze parti oltre a ExerciseDB (per fetch esercizi).

## 7. Vincoli tecnici di progetto

- App `.NET MAUI` con target principale Android; iOS opzionale e secondario.
- Architettura MVVM con `CommunityToolkit.Mvvm`.
- Navigazione basata su Shell (3 tab: Dashboard, Feed, Stats).
- `HttpClient` asincrono per chiamate remote (ExerciseDB API, PocketBase API).
- `System.Text.Json` per il parsing JSON.
- PocketBase self-hosted per autenticazione, dati social e persistenza remota.
- `Preferences` per credenziali auto-login e PlanStore.
- Gestione esplicita di `IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState` nei ViewModel.
- Nessuna logica REST o business logic nei code-behind.
- Componenti UI: `CollectionView`, `BindableLayout`, `ScrollView`, `Grid`, `Label`, `Button`, `Entry`, `Border`, `Image`.
- Design System Stitch: doppio tema "Cyber-Athletic Elite" / "Fitness Core".
- Font Google Fonts: Inter (body), Lexend (label), Space Grotesk (headline).
- Build secrets injection: `.env` → `Resources/Raw/forge.env` via MSBuild.

## 8. Metriche di successo

- Un utente alla prima apertura completa il flusso `registrazione → login → Dashboard` in meno di 60 secondi.
- Il flusso `nuovo allenamento → cerca esercizio → registra 4 serie → salva` richiede meno di 120 secondi.
- Il catalogo esercizi mostra risultati via API entro 3 secondi.
- Lo storico allenamenti persiste dopo riavvio dell'app (su PocketBase).
- Il feed si aggiorna in tempo reale dopo like/follow.
- Lo streak viene calcolato correttamente come settimanale con tolleranza 7 giorni.

## 9. Rischi, dipendenze e questioni aperte

### Rischi

- ExerciseDB API potrebbe cambiare formato risposta o deprecare endpoint. Mitigazione: mapping DTO robusto.
- Short URL ExerciseDB (`encr.pw`, `acesse.dev`) bloccati da ISP italiani. Mitigazione: risoluzione redirect e cache su PocketBase.
- PocketBase self-hosted richiede manutenzione server. Mitigazione: HTTPS via Nginx Proxy Manager + Let's Encrypt.
- `CollectionView` annidate causano crash Android. Mitigazione: uso di `BindableLayout`.

### Dipendenze

- **ExerciseDB API** (RapidAPI): `exercise-db-fitness-workout-gym.p.rapidapi.com` — endpoint per esercizi.
- **PocketBase**: self-hosted su `https://YOUR_SERVER.duckdns.org` per auth, storage e social.
- **Google Fonts**: Inter, Lexend, Space Grotesk (inclusi nell'app).
- **Stitch** (Google): strumento di design UI per mockup.

### Questioni aperte

- TBD: implementazione IT-04 (peso corporeo e misure).
- TBD: leaderboard settimanale (sostituita da feed + like nell'MVP).
- TBD: persistenza locale SQLite per uso offline (attualmente l'app richiede connessione).
- TBD: notifiche push (attualmente le notifiche sono pull su apertura FriendRequestsPage).

## 10. Documentazione collegata

Aggiornamento 11 ottobre 2026: priorità a stabilità e privacy. Gli amici sono follower reciproci con entrambi i follow accettati, spiegato nell’editor account. Visibilità di tutto lo storico pubblico (utenti autenticati), privato o solo amici, applicata sul server. Tutti i badge già sbloccati selezionabili in evidenza; achievement retroattivi sullo storico completo. Schede personali senza limite e due esempi iniziali eliminabili senza rigenerazione. Recupero 0–300 secondi e note per esercizio persistono nelle schede; le sessioni generiche non modificano una scheda. Carichi e ripetizioni suggeriti provengono dall’ultima sessione completata. Evidenza e verifiche manuali restanti in `docs/test-matrix.md`; deployment server in `docs/pocketbase-profile-migration.md`.

- `docs/plan.md` — Piano di progetto con stato iterazioni
- `docs/architecture.md` — Architettura dell'applicazione
- `docs/test-matrix.md` — Matrice di test
- `docs/Iterazioni-IA.md` — Audit report e cronologia modifiche
- `docs/iterations/` — Dettaglio iterazioni completate

Follower e seguiti hanno privacy separata dagli allenamenti (Pubblico, Privato, Solo amici), verificata sul server. Gli elenchi mostrano avatar, follow e cerchio blu per aprire un allenamento pubblico; unfollow richiede conferma coerente con il tema.

## Aggiornamento 12 ottobre 2026

Calendario coerente con settimana/mese/tre mesi/anno/tutto: giorni con allenamenti evidenziati in blu, nome abbreviato e apertura dettagli. Livello account derivato da 100 XP per achievement; serie completate deselezionabili. Il nome nella testata è il nome salvato dell’allenamento. Foto Android ridimensionate, presenti nello storico e nei dettagli; storico proprio include salvataggi locali in attesa di sincronizzazione. Dettaglio esercizio con progressione peso massimo, serie, ripetizioni e volume. Eliminazione account confermata da password e pulizia server. Notifiche social FCM implementate ma attivazione subordinata al progetto Firebase; avvisi locali per recupero e promemoria. Android minimo API 23.

## Workout experiments — 12 ottobre 2026
Pagina allenamento chiara/scura basata sui riferimenti forniti: immagini e consigli, note per esercizio, serie modificabili, recupero selezionabile 0–5 minuti con ruote, azioni elimina/sposta/sostituisci. Nessun pulsante START nella sessione. Timer Android con notifica cronometro, pausa/salta e avviso sonoro/vibrazione al termine anche in background; allarmi precisi abilitabili esplicitamente.

### Workout feedback — 12 ottobre 2026
Aggiornamento immediato locale di profilo/statistiche alla conferma del salvataggio; upload successivo. Calendario multi-mese virtualizzato e mesi a scorrimento. Filtro muscoli basato sui target principali, fallback alla regione solo quando assenti. Consigli ruotano ogni sei secondi durante la sessione. Notifica recupero FORGE con esercizio, miniatura disponibile, progresso, pausa/salta; servizio Android limitato alla durata del recupero e tono incluso con accesso alle impostazioni del canale.

### Schede e tipi di serie — experiments
Alla fine di una sessione da scheda, le modifiche a nome, esercizi/ordine, note, recuperi e serie chiedono se aggiornare la scheda o mantenere l’originale. Nessuna modifica automatica durante la sessione; salvare il recupero cambia solo la sessione fino alla scelta finale. La scheda aggiornata mantiene ID e data di creazione e azzera i flag di completamento. Numero SET cliccabile: normale, W warm-up giallo, D drop set viola, F cedimento rosso; tipi persistiti in schede, bozze e storico/dettaglio. Tre test mirati passati (firma, rinomina/esercizi aggiunti, copia senza mutazione e tipi). Prove popup/scelte su telefono ancora da eseguire. Nessun push richiesto.

## Requisiti e proposta — 13 ottobre 2026

Il proprietario conferma il funzionamento delle ultime modifiche su dispositivo. Nuovo requisito da implementare: le note della scheda e degli esercizi già presenti devono essere salvate automaticamente e caricate alla sessione successiva, senza dipendere dalla conferma di aggiornamento della scheda. Il popup finale resta per nome, esercizi, ordine, recuperi e serie. Le note degli esercizi nuovi restano nello storico della sessione e possono entrare nella scheda quando viene confermata la loro aggiunta.

Ricompense per record personali delle serie: solo idea futura, nessuna implementazione. Modalità locale senza account/server, sincronizzazione facoltativa e aggiornamento dall’app sono proposte in valutazione: dettagli in docs/offline-first-proposal.md. Nessuna di queste modifiche runtime è stata applicata in questa fase.

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
