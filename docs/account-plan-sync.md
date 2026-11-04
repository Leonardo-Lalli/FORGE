# Schede salvate nell’account — 26 ottobre 2026

La precedente build di prova (etichettata 0.8.6-beta) salvava le schede in SQLite e le separava per utente, ma non le caricava sul server. Un aggiornamento con la stessa firma mantiene normalmente questi dati. Disinstallare l’app o cancellarne i dati li rimuove: accedere di nuovo non poteva recuperare le schede solo locali.

La candidata 0.8.5-beta su develop (versionCode 88; precedentemente provata con etichetta locale 0.8.7-beta) aggiunge una copia delle schede in PocketBase, privata per account, con esercizi, serie, recuperi e note. Le modifiche si salvano prima sul telefono; quando il server è raggiungibile vengono inviate e vengono recuperate quelle già presenti nell’account. Le modifiche offline restano in attesa. Le cancellazioni si propagano senza ricreare schede da dispositivi rimasti indietro.

## Attivazione sul server del proprietario

1. Aprire la console del CT PocketBase Community Scripts esistente (`/opt/pocketbase`, servizio `pocketbase`).
2. Copiare ed eseguire il contenuto del nuovo `artifacts/forge-lxc-update.sh`, oppure trasferire lo script sul CT ed eseguire `bash forge-lxc-update.sh` da root. Lo script riusa l’installazione esistente, arresta il servizio durante la migrazione e salva prima un backup in `/var/backups/forge-profiles-*`.
3. Installare l’APK firmato candidato 0.8.5-beta da develop come aggiornamento, senza disinstallare.
4. Accedere con l’account abituale e aprire Dashboard/Schede con la rete attiva. Se compare l’avviso di sincronizzazione, le modifiche sono ancora locali: usare Riprova dopo aver aggiornato il server o ripristinato la rete.

Questa consegna prepara i file, ma non modifica il server reale. Lo script deve essere quello rigenerato con la nuova migrazione `1792972800_forge_plans.js` e l’hook `forge-plans.pb.js`. Per altre installazioni seguire `docs/self-hosting.md` e passare directory e servizio reali allo script `tools/apply-profile-migration.sh`.

Non disinstallare per verificare il primo caricamento: provare prima con un secondo dispositivo o con un client locale temporaneo. Le schede già eliminate prima di questa funzione non possono essere recuperate dalla sincronizzazione se non esiste un altro backup.

## Verifica e limiti

Test .NET: nuovo database/client recupera schede, note multilinea e serie dall’account; account diverso non vede schede altrui; modifiche/cancellazioni offline si riallineano. Test su PocketBase locale temporaneo: migrazione, autenticazione, privacy della collezione, isolamento, input non valido, cancellazioni e scritture molto ravvicinate. Nessuna verifica sul telefono o sul server del proprietario dichiarata eseguita.

La risoluzione dei conflitti usa l’ultima data di modifica per singola scheda: dispositivi con orologi fortemente sfasati o modifiche simultanee possono richiedere correzioni manuali. La sincronizzazione non è un backup con cronologia. Non comprende la bozza di allenamento in corso, che resta sul telefono. Le cancellazioni conservano un marcatore privato sul server per evitare che client vecchi ricreino le schede.
