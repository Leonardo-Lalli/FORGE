# Verifica profili, privacy e allenamenti — 11 ottobre 2026

Aggiornamento ricerca/cache: su richiesta dell’utente non è stata ripetuta la suite completa. Verifica mirata su PocketBase locale con provider simulato: autenticazione, campi legacy/moderni, blocco inserimenti arbitrari, cursori validi, conteggio, assenza duplicati e riutilizzo persistente dopo riavvio con provider offline. Passata. La connessione live del LXC al provider e il flusso Docker restano da verificare dopo deployment; nessun server pubblico modificato. Compilazione Android usata per verificare le correzioni client.

Controllo HTTP HEAD su tre GIF reali del provider (upward facing dog, assisted hanging knee raise, impossible dips): tutte HTTP 200; ciascuna scheda contiene sei istruzioni. Campione di raggiungibilità, non verifica completa delle 1.500 GIF. Build Android finale passata, zero errori e avvisi; sintassi Bash dei quattro script e del blocco LXC verificata. La compilazione non sostituisce una prova sul dispositivo.

Baseline su main: `2159d4c`. Implementazione sul nuovo develop; precedente develop conservato in `codex/develop-before-2026-10-11`. Nessun push o modifica al server pubblico.

| Area | Verifica | Stato |
| --- | --- | --- |
| Avatar | File protetto restituisce 404 senza token, 200 con token file; client riprova senza JWT nell’URL | Test automatici e PocketBase locale passati |
| Achievement | Importazione di 201 allenamenti su due pagine, sblocco retroattivo | Passato |
| Badge | Null precedente compatibile, selezione massima tre, validazione server | Passato; selezione grafica da provare sul telefono |
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
