# Catalogo esercizi verificato — 11 ottobre 2026

## Ricerca e cache condivisa

Correzioni successive: eliminato il limite di dieci risultati per nome; “Tutti” mostra il catalogo; ricerca e filtri si combinano; i filtri attendono il caricamento già in corso. I duplicati legacy con lo stesso nome sono sostituiti visivamente dalla voce completa del provider, mantenendo distinti gli identificatori del provider. I campi moderni vuoti non nascondono più `instruction`/`Image_url`. I dettagli delle vecchie schede recuperano GIF e istruzioni dalla cache quando esiste una corrispondenza per ID o nome; le istruzioni vanno a capo correttamente.

La migrazione `1791676900_forge_catalog_cache.js` e l’hook `forge-catalog-cache.pb.js` attivano una cache condivisa autenticata. Alla prima inizializzazione del catalogo, il server conserva le pagine del provider e aggiorna `excercise` per `exercise_id` univoco; le richieste successive riusano le pagine salvate. Con il catalogo completo, una nuova installazione dell’app lo carica da PocketBase. L’inizializzazione può popolare tutti i 1.500 esercizi; non è limitata al singolo esercizio selezionato. Non viene eseguito un import durante l’installazione server.

Il client non può inserire contenuti arbitrari: il server prende dati dal provider fisso, accetta solo cursori già emessi e impedisce scritture API dirette degli utenti. I superuser mantengono accesso amministrativo. `forge_catalog_pages` conserva le risposte e non è accessibile direttamente agli utenti. Non è previsto aggiornamento periodico automatico del dataset in questa versione.

La cache contiene testi e URL, **non file GIF**. Gli URL consentono la visualizzazione online, ma il provider può rendere un file non disponibile. Schede demo ed esercizi personalizzati non associati al provider possono non avere media/istruzioni. Senza migrazione rimane il fallback al provider e alla cache locale. La cache server si attiva solo dopo deployment e riavvio dell’app; la resa sul telefono resta da verificare.

Provider utilizzato dal client: ExerciseDB OSS/AscendAPI, `https://oss.exercisedb.dev/api/v1/`. PocketBase `excercise` viene usato anche come cache/fonte personalizzata; non è wger.

Il controllo completo con `tools/audit-exercise-catalog.ps1` ha contato 1.500 identificatori unici, 1.500 URL GIF, zero senza URL GIF e 1.500 con istruzioni testuali. Il provider non offre un campo separato per immagini statiche in questo dataset. La presenza dell’URL non prova disponibilità di ciascun file o qualità dell’esecuzione: le GIF non sono state scaricate singolarmente.

Corretto un problema di paginazione: il provider usa `after`, mentre `cursor` veniva ignorato e poteva ripetere la prima pagina. Il client ora salva la completezza della cache solo raggiunto il totale dichiarato; le istruzioni sono recuperate anche dalla cache durante l’allenamento e le GIF sono animate nella scheda dettaglio.

Il numero dei record effettivi del PocketBase pubblico non è ricavabile dall’elenco anonimo: le regole richiedono autenticazione. Per il conteggio reale nel container usare `python3 tools/audit-pocketbase-exercises.py /opt/pocketbase/pb_data/data.db`. Il controllo è di sola lettura e non stampa dati account o credenziali.

Con 1.500 esercizi il problema prioritario è qualità, ricerca, traduzione e disponibilità offline. [wger](https://github.com/wger-project/wger) offre API REST e contenuti comunitari, con licenze dei dati da rispettare per ciascuna voce: valutabile come integrazione mirata, senza promettere GIF per ogni esercizio. Un’API FORGE propria può aggregare contenuti verificati e gestire cache/versioni, ma non crea automaticamente materiale didattico corretto o diritti di utilizzo. Non è stato sostituito il provider in questa iterazione.
