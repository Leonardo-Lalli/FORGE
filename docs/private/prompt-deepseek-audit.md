# PROMPT PER DEEPSEEK V4 PRO — ANALISI APPROFONDITA FORGE

> Ragionamento: MAX | Temperatura: 0.1 | Risposta in italiano

---

## CONTESTO

Sei un security auditor senior e code reviewer specializzato in applicazioni mobile .NET MAUI, backend PocketBase self-hosted su homelab, e sicurezza di rete domestica. Devi analizzare la repository GitHub di FORGE, un'app Android per il tracking di allenamenti in palestra con funzionalità social.

**Importante:** L'app è sviluppata da uno studente, ospitata su un server casalingo (Proxmox container, Nginx Proxy Manager, DuckDNS). La repository sta per diventare **pubblica** su GitHub. L'audit deve essere scrupoloso, approfondito e attento come se dovessi proteggere il server di casa dello studente da attacchi reali.

---

## ARCHITETTURA DEL SISTEMA

```
Telefono Android (app MAUI)
    ↓ HTTPS
Nginx Proxy Manager (reverse proxy, Let's Encrypt, DuckDNS)
    ↓ HTTP interno
PocketBase (container Proxmox/Debian, porta 8080)
    ├── Auth (email/password, JWT)
    ├── Collection: users (email, name, bio, avatar, fcm_token)
    ├── Collection: logged_workouts (user, name, date, exercises, exercise_data, volume, duration, likes, liked_by, photos)
    ├── Collection: social_graph (from_user, from_name, to_user, status)
    └── Collection: excercise (exercise_id, name, bodyPart, equipment, instructions, imageUrl)
    ↓
SQLite locale (telefono)
    ├── local_workouts (con IsDraft flag)
    ├── cached_exercises
    ├── saved_plans
    └── achievements
```

**Stack:**
- .NET MAUI 10 (Android-first), MVVM con CommunityToolkit.Mvvm 8.4
- PocketBase self-hosted su container Debian/Proxmox
- Nginx Proxy Manager con Let's Encrypt su DuckDNS
- ExerciseDB v1 free API (oss.exercisedb.dev)
- SQLite (sqlite-net-pcl) per cache offline
- xUnit per test (36 test)

---

## FILE DA ANALIZZARE (TUTTI)

### Codice sorgente (C#)
Analizza OGNI file `.cs` in `src/GymTracker.Mobile/`:
- `MauiProgram.cs` — DI composition root, HttpClient configuration
- `App.xaml.cs` — startup flow, BuildSecrets loading
- `AppShell.xaml.cs` — route registration
- `Services/PocketBaseService.cs` — auth, CRUD, social, file upload, JWT token management
- `Services/ExerciseDbApiService.cs` — API client, cache, search
- `Services/DatabaseService.cs` — SQLite, 4 tabelle
- `Services/AchievementService.cs` — tracking achievement
- `Services/BuildSecrets.cs` — .env loading
- `Services/SyncService.cs` — offline sync
- `Services/ConnectivityService.cs` — network monitoring
- `Services/ThemeService.cs` — theme management
- `Services/WorkoutSession.cs` — workout state
- `Services/PlanService.cs` — plan CRUD SQLite
- `Services/CsvImportService.cs` / `CsvExportService.cs` — import/export
- `Models/*.cs` — tutti i modelli dati
- `Models/Dto/*.cs` — DTO per API
- `ViewModels/*.cs` — tutti i 12 ViewModel
- `Views/*.xaml` — tutte le 10 pagine XAML
- `Views/*.xaml.cs` — tutti i code-behind
- `Converters/*.cs` — 3 converter
- `Platforms/Android/AndroidManifest.xml` — permessi Android
- `Platforms/Android/MainActivity.cs`, `MainApplication.cs`
- `GymTracker.Mobile.csproj` — configurazione build, MauiImage, secrets

### Tool e script
- `tools/ExerciseImporter/Program.cs` — import esercizi su PocketBase
- `tools/pb_hooks/push.pb.js` — hook PocketBase per FCM

### Test
- `tests/GymTracker.Mobile.Tests/*.cs` — 36 test xUnit

### Documentazione (TUTTI i file .md)
- `README.md` — presentazione GitHub
- `AGENTS.md` — regole progetto
- `docs/spec.md` — specifica prodotto
- `docs/plan.md` — piano iterazioni
- `docs/architecture.md` — architettura
- `docs/project-journal.md` — diario sviluppo
- `docs/security-hardening.md` — guida sicurezza
- `docs/pocketbase-schema.md` — schema PocketBase
- `docs/push-notifications.md` — setup FCM
- `docs/api-notes.md` — note API
- `docs/test-matrix.md` — matrice test
- `docs/Iterazioni-IA.md` — cronologia modifiche
- `docs/prompt-log.md` — log prompt AI
- `docs/consegna-presentazione.md` — prompt presentazione
- `docs/deployment.md` — deployment guide
- `docs/demo-script.md` — script demo
- `.env.example` — template variabili ambiente
- `.gitignore` — file esclusi

---

## OBIETTIVI DELL'ANALISI

### 1. SICUREZZA DEL HOMELAB (CRITICO)

L'app si connette a un server casalingo. Analizza:

**Esposizione del server:**
- Il dominio DuckDNS (`leoforge.duckdns.org`) è hardcoded o embedded nell'APK?
- Qualcuno che decompila l'APK può scoprire l'IP/hostname del server di casa?
- Ci sono riferimenti a IP interni (192.168.x.x), vecchi domini, o hostname che rivelano la topologia di rete?
- Il file `.env` viene compilato nell'APK? Se sì, cosa contiene?
- Il file `gymtracker.env` in `Resources/Raw/` è nel `.gitignore`? È stato committato per errore?
- Il vecchio dominio `pocketbase.server-casa-leo.duckdns.org` appare ancora in qualche file?

**Attacchi al server:**
- Un utente malevolo può fare brute-force sul login PocketBase? C'è rate limiting?
- Il pannello admin PocketBase (`/_/`) è accessibile dall'esterno?
- Le API Rules di PocketBase sono sufficientemente restrittive? Un utente autenticato può leggere i dati di un altro utente?
- Può un utente creare record con `user` falsificato (impersonazione)?
- Ci sono endpoint senza autenticazione?
- Il token JWT è esposto in URL, log, o header non sicuri?

**Dati sensibili:**
- Le password degli utenti sono memorizzate in modo sicuro sul telefono? (SecureStorage vs Preferences)
- Il token JWT è memorizzato in modo sicuro?
- Il database SQLite è cifrato? Se no, cosa contiene di sensibile?
- Le foto degli allenamenti (base64) sono accessibili ad altre app sul telefono?
- `android:allowBackup` è true o false? Cosa viene backing up?

**Rete:**
- Tutte le comunicazioni sono HTTPS?
- Ci sono chiamate HTTP in chiaro?
- C'è certificate pinning?
- L'app gestisce correttamente i certificati self-signed o scaduti?

### 2. SICUREZZA DEGLI UTENTI (CRITICO)

L'app gestisce dati personali (allenamenti, foto, social graph). Analizza:

**Privacy:**
- Un utente può vedere i workout/foto di un altro utente senza autorizzazione?
- Le foto degli allenamenti sono visibili nel feed pubblico o solo agli amici?
- I dati dell'utente (email, nome) sono esposti ad altri utenti senza consenso?
- Il search endpoint degli utenti filtra correttamente i risultati?

**Integrità dei dati:**
- Può un utente modificare i workout di un altro utente?
- Può un utente manipolare il contatore di like?
- I dati inviati al server sono validati lato client? E lato server?
- Cosa succede se l'utente invia dati malevoli (JSON malformato, base64 enorme, SQL injection)?

**Gestione errori:**
- Gli errori dell'API rivelano informazioni sensibili (stack trace, nomi di file, struttura del DB)?
- I messaggi di errore sono user-friendly o tecnici?
- Cosa succede se il server è irraggiungibile? L'app crasha o gestisce graceful?

### 3. ROBUSTEZZA E AFFIDABILITÀ

**Gestione stati:**
- Ogni ViewModel gestisce loading, error, empty, success states?
- Cosa succede se PocketBase è offline ma l'utente cerca di salvare un workout?
- Il SyncService gestisce correttamente i conflitti?
- Cosa succede se il token JWT scade durante una sessione?

**Race conditions:**
- Ci sono race condition nel salvataggio workout (PocketBase + SQLite)?
- Il caricamento del draft può sovrascrivere un workout appena salvato?
- Il timer dell'allenamento può continuare dopo la chiusura?

**Memory leaks:**
- I CancellationTokenSource vengono sempre cancellati?
- I event handler vengono deregistrati?
- WeakReferenceMessenger viene usato correttamente?

**Edge cases:**
- Cosa succede con 0 esercizi, 0 serie, peso 0, reps 0?
- Cosa succede se l'utente ha 1000+ workout?
- Cosa succede se la foto è > 10MB?
- Cosa succede se il nome del workout contiene caratteri speciali o emoji?

### 4. PERCEZIONE PUBBLICA E DUBBI DEGLI UTENTI

L'app sta per diventare pubblica su GitHub. Analizza cosa potrebbero pensare/pensare gli utenti:

**Trasparenza:**
- Il README spiega chiaramente dove vanno i dati degli utenti?
- È chiaro che il server è self-hosted su un homelab e non su un cloud professionale?
- Gli utenti sanno che le loro foto di allenamento vengono salvate su un server casalingo?
- È documentato cosa succede ai dati se il server viene spento?

**Affidabilità percepita:**
- Un utente che vede "self-hosted" e "DuckDNS" si fida?
- La mancanza di SSL certificate pinning è un problema percepito?
- L'app ha un aspetto professionale o sembra un progetto studentesco?
- I messaggi di errore sono rassicanti o tecnici/spaventosi?

**Privacy concerns:**
- Gli utenti potrebbero preoccuparsi che lo sviluppatore (studente) può vedere i loro dati?
- PocketBase admin può vedere tutti i dati? È documentato?
- Le foto base64 nei workout sono visibili all'admin del server?

**Dubbi specifici da chiarire nei file .md:**
- Il README dovrebbe avere una sezione "Privacy" che spiega dove vanno i dati
- Il README dovrebbe avere una sezione "Sicurezza" che spiega le misure adottate
- Il README dovrebbe menzionare che il server è self-hosted e quali sono le implicazioni
- La documentazione dovrebbe spiegare come gli utenti possono cancellare i propri dati
- Dovrebbe esserci un DISCLAIMER che l'app è un progetto didattico, non un prodotto commerciale
- La licenza dovrebbe essere chiara (MIT? Custom? Nessuna?)

### 5. ANALISI DEI FILE .MD — CORREZIONI E APPROFONDIMENTI

Per OGNI file .md nella repository, analizza:

**Accuratezza:**
- I file .md riflettono lo stato attuale del codice o sono obsoleti?
- Ci sono riferimenti a feature rimosse (es. Wger, ExerciseApiService, PlanStore)?
- Ci sono riferimenti a URL/IP/domini che non dovrebbero essere pubblici?
- Le metriche riportate (numero di ViewModel, Services, ecc.) sono accurate?

**Sicurezza informativa:**
- I file .md contengono informazioni che potrebbero aiutare un attaccante?
- I file .md rivelano la struttura del server, porte, IP, domini?
- `docs/Iterazioni-IA.md` contiene l'IP interno `192.168.1.23`?
- `docs/security-hardening.md` contiene config che rivela la topologia di rete?
- `docs/pocketbase-schema.md` rivela la struttura del DB che potrebbe essere sfruttata?

**Professionismo:**
- Il tono è appropriato per una repo pubblica?
- Ci sono informazioni troppo tecniche che confondono gli utenti non tecnici?
- Ci sono informazioni troppo vaghe che non ispirano fiducia?
- La documentazione è coerente tra i vari file?

**Correzioni suggerite:**
- Suggerisci modifiche specifiche al README per migliorare la percezione
- Suggerisci l'aggiunta di sezioni mancanti (Privacy, Sicurezza, Disclaimer, Licenza)
- Suggerisci quali file .md dovrebbero essere rimossi o resi privati
- Suggerisci quali informazioni sensibili dovrebbero essere redatte

### 6. ANALISI DEL CODICE — VULNERABILITÀ SPECIFICHE

Per ogni file `.cs`, controlla:

**PocketBaseService.cs (file più critico):**
- Il token JWT è gestito in modo sicuro?
- `GetFileUrl()` espone il token in URL?
- `SaveCredentialsAsync()` ha fallback insicuri?
- `TryAutoLoginAsync()` legge password da posizioni insicure?
- `Logout()` pulisce TUTTI i dati sensibili?
- `EnsureAuthAsync()` ha race conditions?
- `CreateRecordAsync()` valida l'input?
- `UploadAvatarAsync()` valida il file?
- Il metodo `GetHttp()` crea correttamente l'HttpClient?
- Ci sono metodi che inviano dati senza autenticazione?

**ActiveWorkoutViewModel.cs:**
- `SaveToPocketBaseAsync()` imposta `user` dal client (server dovrebbe validare)
- `SaveDraftAsync()` salva dati sensibili in SQLite non cifrato
- Le foto base64 sono validate (dimensione, formato)?
- `TakePhotoCommand` e `PickPhotoCommand` gestiscono i permessi correttamente?
- Il timer può causare memory leak se non cancellato?

**DatabaseService.cs:**
- SQL injection in `DeleteExercisesByPrefixAsync` (usa LIKE con parametro)?
- Il database è cifrato?
- `ALTER TABLE` migration è sicura?
- Cosa succede se la migrazione fallisce a metà?

**BuildSecrets.cs:**
- Il file `.env` è embedded nell'APK?
- `ConcurrentDictionary` è sufficientemente thread-safe?
- `LoadAsync()` gestisce correttamente gli errori?
- Cosa succede se il file non esiste?

**AchievementService.cs:**
- `OnWorkoutSavedAsync()` ha race conditions con il messenger?
- `LoadStatesAsync()` è thread-safe?
- I dati degli achievement possono essere manipolati?

**CsvImportService.cs:**
- Validazione dell'input CSV (injection, dimensione, formato)?
- Cosa succede con un CSV malevolo?

**ExerciseDbApiService.cs:**
- L'API esterna è chiamata in modo sicuro?
- Il caching è sicuro (injection, avvelenamento cache)?
- Rate limiting client-side?

### 7. CONFIGURAZIONE E BUILD

**GymTracker.Mobile.csproj:**
- Il target `CopyEnvFile` copia `.env` nell'APK?
- `MauiImage` include tutte le immagini correttamente?
- Ci sono file sensibili inclusi per errore?

**AndroidManifest.xml:**
- `allowBackup` è false?
- I permessi sono minimi necessari? (INTERNET, CAMERA, VIBRATE, POST_NOTIFICATIONS)
- `usesCleartextTraffic` è false?

**.gitignore:**
- `.env` è escluso?
- `gymtracker.env` è escluso?
- `*.apk` è escluso?
- Ci sono file sensibili non esclusi?

---

## OUTPUT RICHIESTO

Struttura la tua risposta in queste sezioni:

### SEZIONE 1: Executive Summary
- Numero totale di vulnerabilità trovate per severità (CRITICAL/HIGH/MEDIUM/LOW)
- Rischio complessivo per l'homelab (1-10)
- Rischio complessivo per gli utenti (1-10)
- Top 5 problemi da risolvere PRIMA di rendere la repo pubblica

### SEZIONE 2: Vulnerabilità per file
Per ogni vulnerabilità trovata:
```
[SEVERITY] File: percorso:linea
Descrizione: cosa c'è che non va
Impatto: cosa può succedere
Fix: come correggere (con codice se necessario)
```

### SEZIONE 3: Esposizione Homelab
- Cosa può scoprire un attaccante dall'APK decompilato
- Cosa può scoprire un attaccante dalla repo GitHub pubblica
- Cosa può scoprire un attaccante dal traffico di rete
- Mappa degli attacchi possibili al server casalingo

### SEZIONE 4: Percezione pubblica
- Cosa penserebbe un utente non tecnico vedendo la repo
- Cosa penserebbe un security researcher vedendo la repo
- Cosa penserebbe un potenziale datore di lavoro vedendo la repo
- Dubbi specifici che gli utenti potrebbero avere

### SEZIONE 5: Correzioni file .md
Per ogni file .md che necessita correzioni:
```
File: percorso
Problema: cosa c'è che non va
Correzione: cosa cambiare (testo specifico)
```

### SEZIONE 6: Codice da correggere
Per ogni file .cs che necessita correzioni:
```
File: percorso
Problema: descrizione
Codice attuale: [snippet]
Codice corretto: [snippet]
```

### SEZIONE 7: Checklist pre-pubblicazione
Lista di TODO da completare prima di rendere la repo pubblica, in ordine di priorità.

### SEZIONE 8: Raccomandazioni strategiche
- Cosa aggiungere al README per ispirare fiducia
- Quale licenza usare
- Se aggiungere un CONTRIBUTING.md
- Se aggiungere un PRIVACY.md
- Se aggiungere un DISCLAIMER.md
- Altre raccomandazioni

---

## ISTRUZIONI FINALI

- Sii ESTREMAMENTE specifico: cita numeri di riga, nomi di file, nomi di variabili
- Non assumere nulla: verifica ogni singola affermazione leggendo il codice
- Pensa come un attaccante: cosa faresti per compromettere questo sistema?
- Pensa come un utente: ti fideresti di questa app? Perché sì o perché no?
- Pensa come un recruiter: cosa pensi dello sviluppatore vedendo questa repo?
- Se trovi un problema, proponi SEMPRE una soluzione concreta con codice
- Prioritizza l'impatto reale sopra le teorie: cosa può succedere DAVVERO?
- Considera che il server è un container Proxmox in casa: se viene compromesso, l'attaccante ha accesso alla rete domestica
- Considera che gli utenti sono persone reali che si fidano dell'app con i loro dati personali e foto

Inizia l'analisi ora. Leggi ogni file. Sii meticoloso. Non tralasciare nulla.
