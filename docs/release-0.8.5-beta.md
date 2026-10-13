# FORGE 0.8.5 beta

Beta Android per provare FORGE e segnalare problemi. Codice, README e sito sono allineati al tag v0.8.5-beta.

## Novità

- Allenamento nei due temi, immagini e consigli rotanti, menu per sostituzione/ordine/rimozione degli esercizi.
- Recupero per esercizio 0–5 minuti, timer Android con avanzamento, suono e vibrazione.
- Note salvate automaticamente nelle schede; conferma separata per modifiche strutturali e nome.
- Serie normali, warm-up, drop set e cedimento; suggerimenti dai carichi precedenti.
- Profili, foto, dettagli allenamenti, follower/seguiti e privacy pubblica/privata/amici reciproci.
- Achievement supportati retroattivi, livello da badge e badge selezionabili nel profilo.
- Calendario/statistiche e refresh aggiornati, filtri esercizi corretti.
- Invito condivisibile alla community con link al sito; stato vuoto utile sul server personale.
- Documentazione e sito riscritti, gamification e schermate reali di sviluppo, roadmap semplificata.
- Pacchetto server completo: Docker, installer LXC, hook, quattro migrazioni e worker FCM opzionale.

## Installazione e aggiornamento

Scarica FORGE.apk e installa su Android 6.0/API 23 o successivo. Usa il server predefinito o imposta la tua istanza.

**Firma nuova:** la 0.8.0 pubblicata e gli APK di prova avevano certificati Debug diversi. La 0.8.5 usa una chiave di release stabile e versionCode 85. Android potrebbe richiedere una reinstallazione: esporta prima i dati locali dalle impostazioni e conserva le schede. Disinstallare senza backup elimina i dati locali; gli allenamenti già sincronizzati restano sul server.

Per il backend leggi docs/self-hosting.md: non basta avviare PocketBase o scaricare soltanto Compose. Le push social richiedono Firebase compatibile, chiave Admin solo server e worker. Il timer locale non richiede Firebase.

## Limiti della beta

- Prove sul dispositivo del proprietario, nessuna garanzia su tutti i telefoni.
- Note automatiche/inviti recenti ancora da provare sul telefono; push FCM reale e nuova installazione Docker/LXC non verificate.
- Schede locali associate all’account, nessuna promessa di sincronizzazione tra dispositivi o modalità completamente locale senza account.
- Community separate per server, nessuna federazione o gestione trainer/palestre completa.
- Alcuni criteri del catalogo achievement non implementati; medaglie per record delle singole serie solo idea futura.
- GIF ospitate dal provider; conteggi del database includono duplicati e record senza media.
- Schermate pubblicate tratte dall’archivio di sviluppo e precedenti ad alcuni aggiornamenti UI.
- Aggiornamento dall’app non implementato; server predefinito senza disponibilità garantita.

Segnala problemi nelle issue indicando versione, Android, modello e passaggi per riprodurli, senza credenziali o dati privati.
