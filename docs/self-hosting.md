# Installare e aggiornare il backend FORGE

13 ottobre 2026. Server e community distinti per istanza; nessuna federazione. Non eseguire gli installer Docker su un LXC Community Scripts già configurato.

## Avvio guidato per nuove installazioni

Gli installer sono mantenuti da FORGE; non sono ancora integrati ufficialmente in Proxmox Community Scripts.

```bash
# Linux/WSL con Docker e Compose già installati:
bash -c "$(curl -fsSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/install.sh)" -- docker
# Dentro un nuovo LXC Debian/Ubuntu, come root:
bash -c "$(curl -fsSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/install.sh)" -- lxc
# Sul nodo Proxmox, come root, per creare un nuovo CT:
bash -c "$(curl -fsSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/install.sh)" -- proxmox
```

Il bootstrap raccoglie PB_VERSION, PB_ADMIN_EMAIL e PB_ADMIN_PASSWORD, oppure usa quelli già esportati. Una password lasciata vuota viene generata casualmente e salvata senza mostrarla. Il checkout e il file privato `.env` sono in `~/forge-server` (modificabile con FORGE_INSTALL_DIR). La password di setup non accetta apostrofi, backslash o ritorni a capo; ciò evita interpretazioni ambigue del file. Per parametri personalizzati del nuovo CT usa FORGE_CT_STORAGE, FORGE_CT_BRIDGE, FORGE_CT_HOSTNAME e FORGE_CT_OS (1 Debian, 2 Ubuntu).

È un percorso per nuove installazioni: non sovrascrive destinazioni esistenti e non aggiorna il server Community Scripts del proprietario. Versione PocketBase, HTTPS, proxy e push vanno scelti/configurati; le nuove combinazioni guidate richiedono ancora una prova completa su Docker/LXC. Per aggiornare un server esistente segui i paragrafi sotto.

## Docker su Linux

Clona il repository della versione che vuoi installare. Copia `.env.example` in `.env`, scegli una versione numerica PocketBase >= 0.23.0 e credenziali amministrative uniche. Il file `.env` deve rimanere privato. Se usi le istruzioni shell seguenti, usa valori quotati secondo la sintassi shell; è un tuo file di configurazione locale.

```bash
cp .env.example .env
# Modifica .env prima di continuare.
chmod 600 .env
set -a
. ./.env
set +a
docker compose up -d --build pocketbase
docker compose exec -T pocketbase pocketbase superuser create "$PB_ADMIN_EMAIL" "$PB_ADMIN_PASSWORD" --dir=/pb_data
docker compose run --rm init
sudo bash tools/apply-docker-profile-migration.sh
```

Se il superuser esiste già, non ripetere la creazione; l’inizializzatore verifica le credenziali. Non inserire password direttamente nel testo dei comandi condivisi. Le operazioni di migrazione fermano il servizio, copiano `pb_data` in `artifacts/server-backups`, applicano le migrazioni e riavviano solo dopo successo. Un fallimento lascia il servizio fermo per permettere diagnosi e ripristino, senza dichiarare l’installazione riuscita.

`docker compose up -d` avvia soltanto PocketBase: `init` e `show-ip` sono servizi di setup espliciti. Hook, privacy profili/social, cache condivisa, privacy follower e coda notifiche richiedono le quattro migrazioni. Non sostituire questa sequenza con il solo file Compose scaricato: servono anche Dockerfile, script e hook del repository o dell’archivio server.

L’immagine viene costruita dal binario ufficiale PocketBase della versione scelta, per amd64/arm64. Mantieni la stessa versione durante l’installazione e la prova iniziale; valida un aggiornamento su una copia dei dati prima di cambiare PB_VERSION. Non avviare automaticamente una versione più recente su dati di produzione. PB_ENCRYPTION_KEY, se impostata, viene passata esplicitamente al binario; conserva anche questa chiave nel backup privato.

## Indirizzo nell’app e reverse proxy

La porta 8090 è su `127.0.0.1` per default, raggiungibile dal server stesso. **Non** inserire quell’indirizzo nel telefono: usa il dominio HTTPS del reverse proxy. Se il proxy è su un’altra macchina, imposta PB_BIND_ADDRESS sull’IP LAN del backend e limita la 8090 via firewall al proxy. Per uso esclusivamente LAN configura consapevolmente l’indirizzo raggiungibile dai telefoni. La porta admin non deve essere pubblica; HTTPS, credenziali, aggiornamenti e regole server sono responsabilità dell’amministratore.

## LXC e Proxmox

- Nuova installazione FORGE: `tools/proxmox-install.sh` nel CT; usa `/opt/forge-pocketbase`, servizio `forge-pocketbase`. Imposta PB_VERSION e credenziali, poi applica la configurazione inclusa dall’installer.
- Creazione di un nuovo CT dal nodo: `tools/proxmox-create.sh`, solo dopo aver verificato ID, storage e parametri. Non usarlo per aggiornare CT esistenti.
- Installazione Proxmox Community Scripts esistente del proprietario: `/opt/pocketbase`, servizio `pocketbase`, porta interna 8080. Aggiornamento da checkout: `sudo bash tools/apply-profile-migration.sh /opt/pocketbase pocketbase`.

La migrazione crea il backup, copia hook/migrazioni e non cambia Nginx o router. Il pacchetto LXC generato con `tools/build-server-package.py` mantiene questi percorsi per il server del proprietario; su un’altra istanza modifica i parametri, non indovinare il percorso di pb_data.

## Push social facoltative

Il timer locale non richiede Firebase. Per push social usa un progetto Firebase compatibile con il client Android e tieni la chiave Firebase Admin solo sul server.

Docker: conserva la chiave fuori dal repository, imposta FORGE_FIREBASE_KEY_FILE al suo percorso e FORGE_PUSH_WORKER_TOKEN a un valore casuale lungo. Ricrea PocketBase per caricare il token e avvia il worker:

```bash
docker compose up -d pocketbase
docker compose --profile push up -d --build push
```

Lo stesso token deve essere presente nel backend e nel worker. Non serve esporre una nuova porta sul router. Una chiave mancante non viene creata automaticamente dal Compose. Nel LXC usa `tools/install-push-worker.sh /percorso/chiave.json http://127.0.0.1:PORT/api [nome-servizio]`; il terzo parametro è `pocketbase` per default, `forge-pocketbase` per installazioni FORGE. Il file Android `google-services.json` non è una chiave amministrativa.

Un servizio `active` non dimostra la ricezione: verifica un vero follow/like su un account di prova con app chiusa. Consulta [la guida push](push-notifications.md).

## Aggiornamento, backup e verifiche

Conserva codice/hook della vecchia versione e un backup prima di aggiornare. Verifica health, login, privacy con due account, follow/like e ripristino dei dati in una copia separata. Nel backup includi le foto, impostazioni e segreti necessari in una destinazione privata. Il catalogo condiviso conserva descrizioni e URL GIF, non scarica i file GIF.

Il nuovo Compose e gli installer sono sottoposti a controlli statici; una prova completa Docker/LXC resta richiesta prima della distribuzione. Non dichiarare un backend pronto sulla base della sola sintassi degli script.
