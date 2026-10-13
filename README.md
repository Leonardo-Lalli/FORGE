# FORGE

**Diario di allenamento Android con una community sul server che scegli.**

[![.NET MAUI](https://img.shields.io/badge/.NET_10-MAUI-512BD4)](https://learn.microsoft.com/dotnet/maui/)
[![Stato](https://img.shields.io/badge/versione-0.8.5_beta-blue)](docs/release-readiness.md)
[![Licenza](https://img.shields.io/badge/codice-MIT-blue)](LICENSE)

[Sito e download](https://leonardo-lalli.github.io/FORGE/#download) · [Release pubblicate](https://github.com/Leonardo-Lalli/FORGE/releases) · [Segnala un problema](https://github.com/Leonardo-Lalli/FORGE/issues) · [Roadmap](ROADMAP.md)

## Il progetto

FORGE nasce come progetto studentesco di informatica di **Leonardo Lalli**. È cresciuto attraverso prove sul telefono e in palestra, correzioni e feedback, fino alla beta **0.8.5**, aperta a chi vuole provarla e contribuire con feedback.

Lo sviluppo impiega estesamente strumenti di **intelligenza artificiale**, incluso Codex, per implementazione, interfacce, revisione, documentazione e verifiche. L’AI è uno strumento di lavoro: non certifica la correttezza o la sicurezza del prodotto. Scelte, revisione e pubblicazione restano responsabilità del proprietario del progetto. Build, verifiche mirate e prove sul dispositivo sono documentate, con i controlli ancora mancanti.

L’obiettivo è unire registrazione degli allenamenti e motivazione tra amici. Una possibile evoluzione è l’uso da parte delle palestre per i propri iscritti: **gestione trainer, iscrizioni e più palestre non sono ancora implementate**.

## Stato reale

**Versione 0.8.5 beta** · codice di riferimento su **main** · Android. La beta è pensata inizialmente per pochi utenti disponibili a segnalare problemi, senza garanzie di disponibilità del server pubblico. Le modifiche successive a un tag non sono automaticamente incluse nel suo APK.

Il proprietario ha confermato sul proprio telefono login, flusso di allenamento, suono del recupero e correttezza apparente dei filtri. Build Android e regressioni mirate passano. Le ultime note automatiche e l’invito community richiedono ancora la prova sul dispositivo aggiornato. Non sono stati provati tutti i telefoni Android né certificata la sicurezza dell’intera infrastruttura.

## Cosa puoi fare

- Creare schede personali senza un limite imposto dall’app, oppure iniziare un allenamento libero.
- Registrare serie, carichi e ripetizioni; segnare serie normali, warm-up, drop set o cedimento.
- Ritrovare i valori della sessione precedente e salvare automaticamente le note degli esercizi già presenti nella scheda.
- Scegliere se aggiornare la scheda quando cambi nome, esercizi, ordine, serie o recuperi durante una sessione.
- Impostare un recupero per esercizio da **0 a 5 minuti**, con notifica Android di avanzamento, pausa e avviso finale.
- Consultare storico, dettagli, foto degli allenamenti, volume, calendario e progressione per esercizio.
- Personalizzare avatar e badge sbloccati retroattivamente nel profilo.
- Cercare utenti, seguire persone, vedere allenamenti condivisi e mettere like **sullo stesso server**.
- Scegliere visibilità pubblica, privata o per amici reciproci per allenamenti ed elenchi follower. Gli amici sono follow reciproci accettati.
- Condividere un invito che apre il sito di download e spiega quale server usare.
- Eliminare il proprio account con conferma della password, se il backend ha le migrazioni aggiornate.

Sono disponibili due temi, chiaro e scuro. Le schermate non costituiscono consigli professionali di allenamento.

## Gamification: progressi che diventano traguardi

Gli achievement rendono visibili la costanza e i progressi senza richiedere di ricominciare da zero: quelli valutabili dallo storico si sbloccano **retroattivamente**, anche per allenamenti completati in precedenza sull’account corrente.

- **Costanza:** primo allenamento, allenamenti totali e frequenza mensile.
- **Volume e durata:** traguardi di volume per sessione/settimana/mese e ore di allenamento.
- **Varietà e foto:** esercizi diversi provati e foto aggiunte agli allenamenti.
- **Profilo:** scegli quali badge sbloccati mostrare e consulta il tuo livello.

Il livello usa **100 XP per achievement sbloccato** e aumenta ogni **500 XP**. Riaprire la schermata non assegna XP aggiuntivi. Gli XP rappresentano i badge, non una misura della forma fisica.

Alcune voci del catalogo richiedono criteri non ancora implementati, ad esempio classifiche e sfide: non promettiamo che tutti i badge siano già sbloccabili. Medaglie per i record delle singole serie restano un’idea futura, come richiesto durante lo sviluppo. Non ci sono classifiche globali tra server.

<p>
  <img src="src/Forge/Resources/Images/achievements/primopasso.jpg" alt="Achievement Primo Passo" width="110" />
  <img src="src/Forge/Resources/Images/achievements/habit.jpg" alt="Achievement Abitudinario" width="110" />
  <img src="src/Forge/Resources/Images/achievements/max_weight.jpg" alt="Achievement Peso Massimo" width="110" />
</p>

## Schermate dell’app

Schermate reali presenti nell’archivio del progetto, scattate durante lo sviluppo. **Non sono una rappresentazione esatta di tutte le schermate della 0.8.5**: workout, profilo, colori e calendario sono stati aggiornati successivamente. Nuove acquisizioni sul dispositivo sostituiranno queste immagini.

<p>
  <img src="assets/screenshots/dashboard.jpeg" alt="Dashboard FORGE, schermata di sviluppo" width="200" />
  <img src="assets/screenshots/allenamento%20attivo%202.jpeg" alt="Allenamento attivo FORGE, schermata di sviluppo" width="200" />
  <img src="assets/screenshots/stats.jpeg" alt="Statistiche FORGE, schermata di sviluppo" width="200" />
  <img src="assets/screenshots/profile.jpeg" alt="Profilo FORGE, schermata di sviluppo" width="200" />
</p>

## Download e primo utilizzo

1. Apri il [sito ufficiale](https://leonardo-lalli.github.io/FORGE/#download) oppure le [release GitHub](https://github.com/Leonardo-Lalli/FORGE/releases).
2. Scegli l’APK della versione che vuoi provare e installalo su **Android 6.0 / API 23 o successivo**. Android può richiedere l’autorizzazione a installare dalla fonte scelta.
3. Registrati o accedi. Se non configuri un server personale, l’app utilizza `https://leoforge.duckdns.org`.
4. Per un’istanza personale, inserisci il suo URL nelle impostazioni del server e crea un account su quell’istanza.

**Passaggio dalle vecchie build:** la 0.8.0 pubblicata e gli APK di prova usavano certificati Debug diversi. La 0.8.5 avvia la firma stabile di release; Android può richiedere una reinstallazione. Esporta prima i dati locali dalle impostazioni e conserva le schede: non disinstallare senza backup. Gli allenamenti già sincronizzati restano sul server.

Gli aggiornamenti richiedono attualmente un nuovo APK: **l’aggiornamento direttamente dall’app non è ancora implementato**. Mantieni backup dei dati prima di cambiare installazione; una firma diversa può impedire l’aggiornamento della versione già presente.

## Server e community

Il server predefinito è self-hosted a casa del proprietario. È una possibilità per provare la beta, non un servizio con disponibilità garantita. Un dominio pubblico non è di per sé una prova di insicurezza, ma un servizio esposto richiede aggiornamenti, regole di accesso e manutenzione.

Ogni istanza PocketBase ha i propri account e la propria community. **Gli utenti su server diversi non possono seguirsi attraverso FORGE.** L’invito non trasferisce account, non accetta automaticamente follow e non configura il server sul telefono del destinatario: indica i passaggi da seguire. Un indirizzo LAN funziona soltanto dove quella rete è raggiungibile.

Se sei il solo utente del server, il feed offre una schermata di benvenuto e un invito; schede, statistiche e profilo restano utili. Il primo membro è identificato solo quando il server conferma l’assenza di altri utenti visibili: un errore di connessione non equivale a una community vuota.

## Dati, privacy e limiti

| Dati / funzione | Comportamento attuale |
| --- | --- |
| Schede | SQLite sul telefono, associate all’account; non promettiamo sincronizzazione delle schede tra dispositivi |
| Allenamenti | Salvataggio locale e sincronizzazione PocketBase; bozze locali |
| Social e profili | Dati dell’istanza collegata; privacy applicata dal backend aggiornato |
| Catalogo | ExerciseDB OSS con cache locale/condivisa di metadati, istruzioni e URL dei media |
| GIF | File ospitati dal provider; conservarne l’URL non garantisce disponibilità offline |
| Push social | Client Firebase FCM e worker disponibili; richiedono configurazione e prova di consegna |
| Timer | Notifica locale Android, indipendente dal worker social |

Lo snapshot del server riportato dal proprietario il 12 ottobre 2026 contiene **2.305 record, 1.325 nomi distinti, 1.500 record con URL GIF e istruzioni e 805 senza**. Sono conteggi del database, non una garanzia di 2.305 esercizi unici o di file GIF tutti raggiungibili. Vedi [catalogo e limiti](docs/exercise-catalog.md).

La modalità completamente locale senza account/server è **solo una proposta**. Non confonderla con la persistenza locale già presente. FCM e i media esterni introducono dipendenze di terze parti; self-hosting non significa automaticamente assenza di connessioni esterne.

## Self-hosting

Serve PocketBase **>= 0.23**, con una versione scelta esplicitamente, collezioni iniziali, **hook e quattro migrazioni FORGE**. Avviare soltanto PocketBase non basta. Il Compose costruisce un’immagine dal binario ufficiale della versione scelta; non usa un tag `latest` di terze parti.

```bash
git clone https://github.com/Leonardo-Lalli/FORGE.git
cd FORGE
cp .env.example .env
# Compila .env: PB_VERSION, PB_ADMIN_EMAIL e PB_ADMIN_PASSWORD.
# Poi segui la sequenza completa, incluso il backup/migrazione:
```

Segui [la guida Docker e LXC](docs/self-hosting.md). I percorsi sono diversi per un’installazione FORGE e per il container Proxmox Community Scripts. Le migrazioni creano un backup e mantengono gli account senza una scelta di visibilità privati. Le push richiedono un progetto Firebase compatibile con il client e una chiave amministrativa conservata **solo sul server**: [guida push](docs/push-notifications.md).

La porta Docker 8090 è pubblicata su loopback per default. Configura HTTPS e il reverse proxy per gli accessi esterni, mantieni privata l’amministrazione e non pubblicare password, token, `.env`, `pb_data` o chiavi Firebase. L’installer non configura router o Nginx automaticamente. Prima di aggiornare PocketBase, valida la versione scelta su una copia dei dati e verifica il ripristino del backup.

## Sviluppo

Stack: .NET 10 MAUI, MVVM con CommunityToolkit.Mvvm, PocketBase, SQLite, ExerciseDB OSS, Firebase FCM facoltativo; sito Next.js esportato su GitHub Pages.

```bash
dotnet workload install maui-android
dotnet restore src/Forge/Forge.csproj -p:TargetFramework=net10.0-android
dotnet build src/Forge/Forge.csproj -f net10.0-android
dotnet test tests/Forge.Tests/Forge.Tests.csproj
```

Usa SDK Android/JDK compatibili con il workload. Per FCM Android configura `google-services.json` per `com.leoforge.forge` seguendo la guida push; non è la chiave Firebase Admin. Per una distribuzione pubblica serve una chiave di firma di produzione stabile. L’APK locale di verifica è distinto dalla release di produzione.

Struttura: `src/Forge` app, `tests/Forge.Tests` regressioni, `tools` installazione/backend, `sitoGithub` sito, `docs` specifiche e verifiche. Documentazione: [architettura](docs/architecture.md), [matrice verifiche](docs/test-matrix.md), [preparazione beta](docs/release-readiness.md), [roadmap](ROADMAP.md).

## Confronto con altre app

Il vantaggio di FORGE oggi è **scegliere e modificare il proprio backend, creare una community separata e usare schede senza limite applicativo o abbonamento nell’app**. Non affermiamo superiorità generale per affidabilità, integrazioni o facilità d’installazione.

Hevy limita la versione gratuita a quattro routine; Strong a tre template. FORGE non impone questi limiti. Hevy offre però un social più articolato e Strong integrazioni e strumenti già consolidati. wger è già open source e self-hosted: non siamo unici in questo e wger copre anche alimentazione e misure. Fonti ufficiali e confronto dettagliato in [app simili](docs/comparison.md), verificato il 13 ottobre 2026.

## Contribuire e segnalare problemi

Apri una [issue](https://github.com/Leonardo-Lalli/FORGE/issues) indicando versione APK, modello del telefono, versione Android, passaggi per riprodurre il problema e comportamento atteso. Non includere credenziali, token, dati personali di altri utenti o chiavi server. Contributi piccoli e verificabili sono benvenuti; segnala anche limiti della documentazione.

## Licenza e attribuzioni

Il codice FORGE è distribuito con [licenza MIT](LICENSE). Catalogo, GIF, font e altre risorse di terze parti conservano le proprie licenze: la MIT del repository non concede automaticamente diritti su quei contenuti. Il provider attualmente usato è [ExerciseDB OSS](https://oss.exercisedb.dev/), non wger. Prima di distribuire cataloghi/media o offrire un servizio commerciale, verifica le condizioni dei contenuti scelti.
