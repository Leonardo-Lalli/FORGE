# Privacy e profili nel container LXC

Il pacchetto aggiornato comprende anche la cache condivisa: una seconda migrazione aggiunge `exercise_id`, campi moderni di istruzioni/media/muscoli, indice univoco e `forge_catalog_pages`. Un secondo hook recupera il provider fisso e riusa le pagine salvate. Le scritture dirette degli utenti su `excercise` vengono bloccate; l’app popola la cache tramite l’endpoint verificato. Restano presenti campi e record legacy. Vengono salvati URL GIF, non file GIF.

Si possono configurare campi e API Rules a mano dal pannello admin privato, ma occorre replicare le tre migrazioni, compresi gli indici, e installare tutti gli hook nel LXC. I campi da soli non fanno recuperare/cacheare gli esercizi e le regole da sole non validano i like. Lo script evita incongruenze, conserva un backup e non chiede password admin. Non usare gli installer generali per reinstallare il server esistente.

## Distribuzioni FORGE

Installazione Proxmox del progetto: dopo l’inizializzazione, `sudo bash tools/apply-profile-migration.sh /opt/forge-pocketbase forge-pocketbase`. Per Community Scripts i nomi sono `/opt/pocketbase` e `pocketbase`, come nella guida sotto. Gli installer avvisano del passaggio necessario.

Docker Compose monta hook in `/pb_hooks` e migrazioni in `/forge_migrations`, senza eseguirle automaticamente su un database vuoto. Dopo `docker compose run --rm init`, usare `sudo bash tools/apply-docker-profile-migration.sh`: backup di `pb_data` in `artifacts/server-backups`, stop, migrazione, riavvio. Per rollback conservare anche il checkout/hook precedenti prima dell’aggiornamento: la copia dello script contiene il codice del checkout corrente. Questo flusso Docker non è stato eseguito su un daemon Docker nella sessione.

Riavvia l’app dopo la migrazione; la prima apertura del catalogo può richiedere qualche minuto, poi mostra il conteggio dei risultati. Consumo reale del database: `du -h /opt/pocketbase/pb_data/data.db`. Non è stato scaricato o stimato l’intero archivio GIF.

La modifica client è pronta su `develop`. L’utente ha confermato l’applicazione sul LXC delle migrazioni `1791676800` e `1791676900`, con backup `/var/backups/forge-profiles-20261011-231915`. Health HTTP/HTTPS verificato 200 dopo l’aggiornamento. La nuova migrazione `1791677000_forge_connections_privacy.js` e l’hook `forge-connections.pb.js` devono ancora essere installati tramite il pacchetto aggiornato. Le migrazioni già applicate vengono saltate.

Il codice server è in `tools/pocketbase/pb_migrations` e `tools/pocketbase/pb_hooks`. È stato verificato su un PocketBase locale 0.40.5, separato da produzione; usa le API di PocketBase 0.23+. Non aggiornare alla cieca il binario del tuo LXC: controlla prima la versione e fai un backup.

Dalla console Proxmox del container, copia il repository/cartella tools. L’installazione standard [Proxmox Community Scripts](https://github.com/community-scripts/ProxmoxVE/blob/main/install/pocketbase-install.sh) usa `/opt/pocketbase/pocketbase`, il servizio `pocketbase` e la porta interna 8080. Controlla l’installazione reale prima di applicare:

```bash
systemctl cat pocketbase
/opt/pocketbase/pocketbase --version
ls -l /opt/pocketbase/pb_data/data.db
```

Se i percorsi corrispondono e non ci sono directory personalizzate nei flag del servizio, esegui dalla directory contenente `tools`:

```bash
sudo bash tools/apply-profile-migration.sh /opt/pocketbase pocketbase
```

Questi sono esempi: usa i nomi reali della tua installazione. Il terzo argomento opzionale permette una directory `pb_data` diversa. Il servizio deve usare le stesse directory `pb_hooks` e `pb_migrations`: controlla `systemctl cat NOME_SERVIZIO`, soprattutto se ci sono flag personalizzati o Docker invece di systemd.

Per contare il catalogo effettivo del server, senza password e senza modificarlo:

```bash
python3 tools/audit-pocketbase-exercises.py /opt/pocketbase/pb_data/data.db
```

Lo script controlla percorsi/versione, ferma il servizio, copia database e file server in `/var/backups/forge-profiles-DATA`, installa migrazione e hook, esegue `migrate up` e riavvia il servizio se prima era attivo e la migrazione riesce. In caso di errore lo lascia fermo e indica il backup da ripristinare. Non richiede credenziali amministrative e non cambia Nginx, router o porta del server. Il backup contiene dati sensibili: custodiscilo nel container.

Le preferenze non impostate diventano **private**, anche per account esistenti. Per condividere, scegli Pubblico o Solo amici nell'app. Pubblico significa visibile agli utenti autenticati di FORGE; gli anonimi non possono leggere gli allenamenti. Gli amici sono utenti che si seguono reciprocamente con entrambi i follow accettati. La scelta si applica a tutto lo storico tramite le API Rules, senza riscrivere gli allenamenti.

Follower e seguiti hanno una preferenza separata `users.connections_visibility`, con Pubblico, Privato e Solo amici; il valore iniziale è Privato. Il proprietario vede sempre i propri elenchi. L’endpoint autenticato `/api/forge/connections/{userId}` verifica la reciprocità per Solo amici e restituisce solo relazioni accettate. L’API diretta `social_graph` è limitata ai partecipanti: gli elenchi privati non sono aggirabili leggendo tutte le relazioni. Ogni partecipante può comunque vedere la propria relazione con un altro utente. Le richieste possono essere accettate/rifiutate solo dal destinatario e non possono cambiare identità. I badge in evidenza sono limitati a tre; l'app permette di selezionare solo quelli sbloccati. Le API non autorizzano modifiche a note/dati di un allenamento altrui.

Gli elenchi occupano la pagina del profilo, usano risorse dei due temi e mostrano avatar e Segui / Segui già / Richiesta inviata. Il cerchio ElectricBlue, come nella home, apre l’ultimo allenamento con visibilità Pubblico; non è applicata una scadenza di 24 ore. Un follow accettato viene rimosso solo dopo conferma nel popup. Dopo unfollow l’accesso riservato agli amici viene rivalutato.

Il report iniziale del LXC mostrava 805 record e 85 nomi distinti, zero URL media. Il conteggio delle istruzioni va ripetuto: è stato corretto il fallback dal nuovo campo JSON nullo al campo legacy. Nessun record legacy viene eliminato. Aprire il catalogo nell’app aggiornata per popolare la cache completa del provider.

Verifica dopo il riavvio con due/tre account di test: privato non visibile, pubblico visibile, solo amici non visibile con un follow soltanto, visibile dopo follow reciproco. Prova anche avatar, ricerca, primo like e unlike. Se lo schema differisce, la migrazione fallisce e la transazione viene annullata: non cambiare manualmente regole a tentativi sul server live.

Per ripristino completo ferma il servizio, ripristina `pb_data`, `pb_migrations`, `pb_hooks` dallo stesso backup e riavvia. Il downgrade della migrazione mantiene regole restrittive; non riattiva le precedenti regole troppo permissive.

## IP domestico

DuckDNS deve pubblicare un indirizzo perché il client possa risolverlo; offuscare la stringa nell'APK non nasconde il server. Soluzione consigliata: dominio controllato da te con Cloudflare Tunnel, connessione in uscita dal LXC e chiusura delle porte inoltrate 80/443 quando il nuovo percorso è verificato. Un'alternativa è un reverse proxy su VPS con tunnel verso casa. Serve configurazione infrastrutturale: non è stata applicata automaticamente.

Disattiva anche l'aggiornamento/record DuckDNS che punta a casa dopo il passaggio. Il DNS storico può aver già registrato il vecchio IP. Mantieni amministrazione privata. Riferimento: [Cloudflare origin protection](https://developers.cloudflare.com/fundamentals/security/protect-your-origin-server/).

Cloudflare Tunnel è utilizzabile gratuitamente, ma il percorso pubblico stabile standard richiede un dominio gestito su Cloudflare. `leoforge.duckdns.org` è un sottodominio DuckDNS di cui non controlli la zona DNS: non è adatto alla configurazione gratuita standard. Un dominio proprio comporta normalmente il costo della registrazione. [Prerequisiti ufficiali](https://developers.cloudflare.com/tunnel/get-started/). Per prove gratuite senza dominio esistono [Quick Tunnels](https://developers.cloudflare.com/tunnel/get-started/quick-tunnels/), con indirizzo temporaneo `trycloudflare.com`.
