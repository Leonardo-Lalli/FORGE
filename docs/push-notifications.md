# Notifiche FORGE — attivazione Android e LXC

Stato 12 ottobre 2026: integrazione Android FCM, registrazione dei dispositivi, coda privata PocketBase e dispatcher pronti. Il proprietario ha creato Firebase e aggiunto un `google-services.json` valido per `com.leoforge.forge`. Il servizio push LXC e la consegna sul telefono restano da verificare: **le push remote non sono ancora confermate attive**. L’APK senza `google-services.json` resta utilizzabile con avvisi locali di fine recupero, achievement e promemoria giornaliero, dopo il permesso in Impostazioni → Notifiche.

## 1. Creare Firebase

Apri [Firebase Console](https://console.firebase.google.com/), crea un progetto e registra un’app Android con package **`com.leoforge.forge`**. Scarica `google-services.json` e mettilo in `src/Forge/Platforms/Android/google-services.json`. Il file è escluso da Git e il progetto lo include automaticamente quando esiste. Poi serve ricompilare e installare l’APK: quello preparato senza questo file non può registrarsi a FCM. Non occorre Analytics per le notifiche.

Riferimenti: [setup Android ufficiale](https://firebase.google.com/docs/android/setup), [Cloud Messaging Android](https://firebase.google.com/docs/cloud-messaging/android/get-started), [integrazione .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/push-notifications?view=net-maui-10.0). Questa versione richiede Android 6/API 23 o superiore.

## 2. Aggiornare PocketBase adesso

Il proprietario ha già applicato la quarta migrazione nel CT 112: backup `/var/backups/forge-profiles-20261012-085930`, servizio `active`. Non occorre rieseguirla per configurare Firebase. Per altre installazioni, copia **tutto** `artifacts/forge-lxc-update.sh` nella console del container LXC. Le prime tre migrazioni già applicate non vengono rieseguite; viene aggiunta `1791763200_forge_account_notifications.js` e aggiornati gli hook. Lo script fa backup, ferma/migra/riavvia il servizio e controlla che sia attivo. Non serve un riavvio separato né modificare Nginx o il router.

La nuova migrazione abilita eliminazione account tramite password e pulizia dei record, foto JSON più grandi e collezioni private per dispositivi/notifiche. Gli hook aggiornati ammettono tutti i 44 ID achievement distinti. Lo script **non crea Firebase e non avvia il dispatcher**; il catalogo GIF rimane una cache di metadati e URL.

## 3. Attivare il dispatcher dopo Firebase

In Firebase → Impostazioni progetto → Account di servizio, genera una chiave privata per il Firebase Admin SDK. Questo JSON è una credenziale del server: trasferiscilo nel LXC come `/root/forge-firebase-service-account.json`, senza metterlo nell’app, nella repository o in chat.

Trasferisci ed estrai `artifacts/forge-profile-server-tools.zip` nel container. Nella cartella estratta verifica l’indirizzo interno effettivo:

```bash
systemctl cat pocketbase
```

Nel CT 112 il proprietario ha confermato `serve --http=0.0.0.0:8080`: l’URL interno è `http://127.0.0.1:8080/api`. Il blocco pronto `artifacts/forge-push-lxc-install.sh` usa questo indirizzo e può essere incollato completo nella console del container, dopo aver trasferito la chiave Admin nel percorso indicato. Per altre installazioni leggi la porta nell’`ExecStart`/configurazione già usata da Nginx. Esegui, sostituendo **PORTA** con quella reale:

```bash
bash tools/install-push-worker.sh /root/forge-firebase-service-account.json http://127.0.0.1:PORTA/api
```

L’installer aggiunge un utente di servizio senza login, un ambiente Python con `firebase-admin`, la credenziale protetta in `/etc/forge-push`, un segreto casuale condiviso con PocketBase e il servizio `forge-push`. Riavvia PocketBase per caricare l’ambiente e avvia il worker. Non tocca le API Rules degli allenamenti, Nginx o porte pubbliche. Richiede accesso Internet dal container per pacchetti e Firebase.

Non cancellare la chiave da Firebase mentre è utilizzata. Dopo aver verificato il servizio puoi rimuovere la copia iniziale da `/root`; quella operativa resta protetta in `/etc/forge-push/service-account.json`.

## 4. Prova sul telefono e log

Installa la build contenente `google-services.json`, accedi e attiva le notifiche nelle impostazioni. Da un secondo account invia una richiesta di follow o un like su un allenamento visibile; verifica la ricezione con l’app in background e che il tocco apra il contenuto corretto.

```bash
systemctl is-active pocketbase forge-push
journalctl -u forge-push -n 50 --no-pager
journalctl -u pocketbase -n 50 --no-pager
```

Eventi social: richiesta, accettazione, like, nuovo allenamento condiviso con chi ti segue. Gli achievement sono calcolati dall’app sul proprio storico e accodati quando l’app rileva lo sblocco; non è implementato un motore di achievement autonomo nel worker. Recuperi e promemoria usano allarmi locali Android, senza dipendere da Firebase. Il recupero usa un cronometro nativo nella notifica (Android 7+, su Android 6 mostra la durata statica), con pausa/salta e avviso breve sonoro/vibrazione. Dal menu recupero si possono abilitare gli allarmi precisi Android: senza questo permesso il sistema può ritardare la scadenza in background. Consentire anche le notifiche; suono e vibrazione rispettano le impostazioni del telefono. I promemoria vanno riattivati dopo un riavvio del dispositivo.

Il dispatcher controlla la coda ogni 30 secondi, usa chiavi evento per evitare duplicati e ricontrolla la privacy dell’allenamento prima dell’invio. Non spedisce nome, note o foto dell’allenamento nella notifica. In caso di interruzione dopo un invio ma prima della conferma possono esserci duplicati; la ricezione FCM dipende anche dalla connettività e dalle impostazioni Android. Logout disiscrive il dispositivo prima di chiudere la sessione; eliminazione account rimuove dispositivi, notifiche, allenamenti e relazioni social.

Verifica eseguita: build Android e controlli su PocketBase locale isolato. Ricezione FCM reale **da verificare dopo configurazione Firebase**; non sono stati inviati messaggi al server pubblico durante i test.

## Recupero experiments: barra e tono
Il recupero avviato dall’utente usa un servizio foreground Android specialUse per aggiornare la notifica ogni secondo con nome esercizio, miniatura HTTPS (massimo 1 MiB, fallback senza immagine), barra e azioni. Un wake lock temporaneo mantiene il conteggio durante il recupero; pausa e termine lo rilasciano. Il canale Fine recupero v2 usa un tono WAV incluso nell’app, distinto dal canale silenzioso del countdown. Dal menu recupero si aprono direttamente le impostazioni suono/vibrazione del canale. Il permesso notifiche da solo non garantisce il suono: valgono volume e impostazioni del telefono. Ricezione/udibilità reali da verificare sul dispositivo.
