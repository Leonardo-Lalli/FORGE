# Preparazione della prima beta

13 ottobre 2026 — preparazione della 0.8.5 beta. Pubblicazione autorizzata dal proprietario; il tag e gli asset GitHub identificano il codice distribuito.

Il flusso base è stato provato dal proprietario. È ragionevole proporre una beta a un piccolo gruppo disponibile a segnalare problemi; non presentarla come un servizio completo per le palestre.

## Evidenze disponibili

- Login, allenamenti, suono timer e filtri confermati dall’utente sul proprio dispositivo.
- Build Android e regressioni mirate documentate in docs/test-matrix.md.
- Migrazioni PocketBase applicate dal proprietario e servizi pocketbase/forge-push confermati attivi.
- Note automatiche e invito community implementati; nuove prove sul telefono ancora necessarie.

## Controlli della beta

- Installare l’APK finale e verificare registrazione, scheda, note, completamento, riavvio e storico.
- Usare due account di prova per verificare pubblici/privati/amici, follow e like.
- Provare un vero evento push con app chiusa: active del worker non conferma la consegna.
- Provare il pacchetto Docker/LXC su un’istanza separata e ripristinare un backup.
- Firma stabile e configurazione Firebase del client: configurate nei secret cifrati GitHub con autorizzazione del proprietario. Conservare il backup privato della chiave per aggiornamenti futuri.
- Versione definita: 0.8.5-beta, versionCode 85, note in release-0.8.5-beta.md. Distribuire l’APK Release firmato, non gli APK Debug di prova.
- Pubblicare il tag della stessa sorgente con APK e pacchetto server; verificare il deployment Pages. Il server live non viene aggiornato dal push del repository.

Nessuna pubblicazione o nuova promessa di disponibilità del server è implicita in questo documento.

La CI di release prepara una **bozza** e richiede: ANDROID_KEYSTORE_BASE64, ANDROID_KEYSTORE_PASSWORD, ANDROID_KEY_ALIAS, ANDROID_KEY_PASSWORD e GOOGLE_SERVICES_JSON_BASE64 nei secret GitHub. Il JSON Firebase Android va codificato in base64 come file, senza includere chiavi Admin. Il workflow riceve tag e versionCode e pubblica solo una bozza APK + archivio server; l’operatore deve verificare e pubblicare consapevolmente la bozza.

## Firma 0.8.5 beta

La nuova firma stabile sostituisce certificati Debug diversi nelle vecchie distribuzioni. Versione Android 0.8.5-beta, versionCode 85. Chiave e password sono conservate in un backup privato esterno al controllo Git; non vanno inserite negli asset della release. Le build Release iniziali mantengono gli assembly senza trimming/AOT per ridurre differenze rispetto al flusso verificato sul telefono. La loro dimensione e le prestazioni non sono ottimizzate o misurate.

L’utente ha autorizzato la pubblicazione di main, experiments e della prerelease. Le prove Docker/LXC e la consegna FCM reale restano limiti dichiarati, non verifiche superate.

I controlli sul telefono, la consegna FCM e l’installazione backend nuova restano raccomandati e non completati. Il proprietario ha scelto una distribuzione beta per raccogliere feedback con questi limiti dichiarati.
