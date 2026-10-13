# Verifica profili, privacy e allenamenti — 11 ottobre 2026

Aggiornamento ricerca/cache: su richiesta dell’utente non è stata ripetuta la suite completa. Verifica mirata su PocketBase locale con provider simulato: autenticazione, campi legacy/moderni, blocco inserimenti arbitrari, cursori validi, conteggio, assenza duplicati e riutilizzo persistente dopo riavvio con provider offline. Passata. La connessione live del LXC al provider e il flusso Docker restano da verificare dopo deployment; nessun server pubblico modificato. Compilazione Android usata per verificare le correzioni client.

Controllo HTTP HEAD su tre GIF reali del provider (upward facing dog, assisted hanging knee raise, impossible dips): tutte HTTP 200; ciascuna scheda contiene sei istruzioni. Campione di raggiungibilità, non verifica completa delle 1.500 GIF. Build Android finale passata, zero errori e avvisi; sintassi Bash dei quattro script e del blocco LXC verificata. La compilazione non sostituisce una prova sul dispositivo.

Baseline su main: `2159d4c`. Implementazione sul nuovo develop; precedente develop conservato in `codex/develop-before-2026-10-11`. Nessun push o modifica al server pubblico.

| Area | Verifica | Stato |
| --- | --- | --- |
| Avatar | File protetto restituisce 404 senza token, 200 con token file; client riprova senza JWT nell’URL | Test automatici e PocketBase locale passati |
| Achievement | Importazione di 201 allenamenti su due pagine, sblocco retroattivo | Passato |
| Badge | Null precedente compatibile, tutti i badge sbloccati selezionabili, ID distinti validati dal server | Passato; utente conferma che la selezione funziona anche sul telefono |
| Schede | Isolamento account, nessun limite, esempi eliminati non rigenerati, note/recuperi persistenti | Passato |
| Recuperi | Zero, 125 secondi, limite cinque minuti; ruote native Android | Logica passata; gesto e temi da verificare sul telefono |
| Valori precedenti | Ultima sessione completata, bozze escluse, modifica rimuove stile suggerito | Passato |
| Volume | Somma giornaliera senza duplicazione, altezze finite anche con volume zero | Passato; visuale da verificare sul telefono |
| Privacy | Privato, pubblico autenticato, follow singolo negato, amici reciproci consentiti | PocketBase locale 0.40.5 passato; prime due migrazioni applicate dal proprietario; nuova privacy elenchi da installare |
| Integrità social | Mittente non può accettarsi; proprietà immutabile; like non modifica altri utenti o note | PocketBase locale passato |
| Sito | TypeScript e build export delle nove pagine statiche | Passato |
| Catalogo | Conteggio paginato del provider | 1.500 record unici, 1.500 URL GIF, 1.500 istruzioni; raggiungibilità di ogni file non verificata |

Suite .NET: 75 test passati. Build Android Debug: zero errori, zero avvisi. La suite segnala NU1900 per il feed audit NuGet non disponibile e CA2022 in un test TLS precedente; il controllo delle vulnerabilità dei pacchetti non è certificato da questo run.

Verifica manuale necessaria su entrambi i temi: cambio avatar; ritorno al proprio profilo dopo visita amico; selezione/rimozione badge; lista follower; ricerca; scrolling del grafico; ruote recupero 0–5 minuti; note e recuperi dopo riapertura scheda; avvio generico senza preferenze precedenti; modifica di carichi suggeriti; minimizzazione/ripresa bozza. La privacy richiede prima la migrazione descritta in `pocketbase-profile-migration.md`.

Aggiornamento follower/seguiti: build Android passata con zero errori e avvisi; verifica mirata PocketBase locale passata per pubblico, privato, amici reciproci, blocco letture dirette del grafo e revoca dopo unfollow. Nessuna nuova esecuzione della suite completa. Avatar, cerchio blu, popup e layout nei due temi da verificare sul telefono. Le prime due migrazioni sono state applicate dal proprietario; health pubblico HTTP e HTTPS 200. La terza migrazione non è ancora installata live.

## Verifiche mirate 12 ottobre 2026

Nove nuovi test superati: confini calendario, quattro metriche su serie completate, esclusione bozze/esercizi diversi e livello account. PocketBase locale usa e getta 0.40.5: controlli mirati superati su avatar, privacy, badge oltre tre, foto JSON, token protetti, coda push autenticata/deduplicata, revoca privacy prima invio ed eliminazione con password senza eliminare altri account. Nessun test distruttivo sul server pubblico e nessuna riesecuzione della suite .NET completa.

Build Android riuscita: zero errori, tre avvisi di API Firebase deprecate nei binding. Icona adattiva con silhouette monocromatica specifica per Android 13+, avvisi locali, gesture calendario/foto/grafici e ricezione FCM reale richiedono verifica sul telefono. Firebase non configurato: non dichiarare push remote operative. Le prime tre migrazioni sono applicate secondo il log fornito dal proprietario; la quarta è da applicare.

## Workout experiments — verifica sul dispositivo
Da verificare nei temi chiaro/scuro: ricerca con filtri e tastiera, GIF/consigli/note, tre puntini con sposta/elimina/sostituisci (anche annullamento), serie e precedenti, recupero 0/30/300 secondi, pausa/ripresa/salta da app e notifica, scadenza a schermo spento con notifiche e allarmi precisi consentiti. Nessuna suite completa ripetuta, come richiesto dal proprietario.

### Workout feedback
Sei regressioni automatiche superate: filtro schiena esclude pettorali e bicipiti come target principali, include lats/latissimus e fallback back; rotazione consigli con ritorno al primo e lista vuota. Da verificare sul dispositivo: completamento e aggiornamento profilo/statistiche senza attesa upload, calendario anno/storico, consigli ogni sei secondi, notifica recupero con barra/miniatura, pausa/salta e tono a schermo spento.

12 ottobre, ritocco experiments: ruote native del recupero misurate a larghezza piena per allineare minuti/secondi alle intestazioni; follower/seguiti in un unico riquadro con contatori, separatore e colori dinamici. Il proprietario conferma suono funzionante e filtri apparentemente corretti sul Samsung A15. Allineamento e profilo da verificare sul dispositivo dopo aggiornamento APK.

### Schede e tipi di serie — experiments
Alla fine di una sessione da scheda, le modifiche a nome, esercizi/ordine, note, recuperi e serie chiedono se aggiornare la scheda o mantenere l’originale. Nessuna modifica automatica durante la sessione; salvare il recupero cambia solo la sessione fino alla scelta finale. La scheda aggiornata mantiene ID e data di creazione e azzera i flag di completamento. Numero SET cliccabile: normale, W warm-up giallo, D drop set viola, F cedimento rosso; tipi persistiti in schede, bozze e storico/dettaglio. Tre test mirati passati (firma, rinomina/esercizi aggiunti, copia senza mutazione e tipi). Prove popup/scelte su telefono ancora da eseguire. Nessun push richiesto.

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

Verifica confezionamento: APK Release 0.8.5-beta, versionCode 85, firma CN=Leonardo Lalli verificata da apksigner; 94.789.711 byte. Build locale completata con tre avvisi Firebase già noti. CI Build & Test del commit dbd2c6d passata (run 38455094135), senza ulteriore suite locale. Corretto avviso aggiornamento: l’export CSV attuale è solo riepilogo, non backup completo di schede, foto o serie.
