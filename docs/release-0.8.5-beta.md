# FORGE 0.8.5-beta

Versione approvata dal proprietario il 5 novembre 2026 e promossa su main. APK Android build 89, stesso certificato di firma delle precedenti build Release. Questa release sostituisce il precedente asset 0.8.5-beta: non sono pubblicate versioni 0.8.6/0.8.7.

## Novità della versione approvata

- Allenamento minimizzabile, barra del recupero e salvataggio/ripristino della sessione per account.
- Conferma X/Indietro con popup a tema, note multilinea ed esercizi non eseguiti esclusi dai conteggi.
- Unità kg/libbre nelle impostazioni, applicata a inserimenti, storico, statistiche, profili, achievement e CSV; dati canonici in kg.
- Icona FORGE e calendario Stats con dimensioni stabili.
- Schede locali con sincronizzazione privata nell’account e coda offline: richiede l’aggiornamento PocketBase incluso nel pacchetto server.
- Correzione dell’avvio Android: callback JNI esplicito e pacchetto rigenerato con verifica dei binari ARM64/x64.
- Roadmap senza etichette di difficoltà, con date e storico.

## Aggiornamento

Installare FORGE.apk come aggiornamento senza disinstallare. ApplicationId com.leoforge.forge, versionCode 89; firma SHA-256 87098c4bf774e2e2512d0635ef6f192de0939de844bc5953f4e6f142a04ebe87. APK SHA-256 805d38ea6263ea9ffbaed6c6d92ae6ee030730e21926f38bdc6399334967b9c0.

Chi usa ancora vecchie build Debug con firma diversa deve conservare i dati prima di una migrazione: il CSV non è un backup completo di schede e foto. Disinstallare elimina i dati locali. Le schede cancellate prima dell’attivazione del backup account non ricompaiono automaticamente.

## Server e limiti

Il pacchetto server contiene hook/migrazioni per le schede account; la pubblicazione non aggiorna il server del proprietario. Seguire docs/account-plan-sync.md e docs/self-hosting.md. Nessun aggiornamento del server reale eseguito qui.

Prove automatiche .NET e controllo firma/callback confezionati; la precedente prova di due aperture nell’emulatore era inconclusiva per ANR anche di System UI. Il proprietario ha successivamente approvato l’app corrente; non si dichiara copertura su tutti i telefoni. Le richieste aggiuntive del 5 novembre (serie eliminabili, avambracci, timer continuo, CSV 5 MiB, riepilogo modifiche e navigazione mesi) appartengono alla successiva build develop e non a questo APK.

Il nome resta beta. Per rendere questa la release corrente anche su /releases/latest, il metadato GitHub prerelease viene disattivato: GitHub non consente il flag Latest sulle prerelease. Riferimento: https://docs.github.com/en/rest/releases/releases#update-a-release.
