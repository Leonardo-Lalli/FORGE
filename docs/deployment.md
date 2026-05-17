# Deploy Guide — FORGE

## Prerequisiti

- .NET 10 SDK con MAUI workload installato
- Server PocketBase configurato e raggiungibile
- Chiave API ExerciseDB da [RapidAPI](https://rapidapi.com/justin-WFnsXH_t6/api/exercisedb)
- Dispositivo Android o emulatore (Android 7.0+)

## Configurazione ambiente

### 1. File `.env`

Crea un file `.env` nella root del progetto:

```env
EXERCISEDB_API_KEY=la_tua_chiave_rapidapi
POCKETBASE_URL=https://tuo-server-pb.dominio.com
```

Il file viene copiato automaticamente in `Resources/Raw/gymtracker.env` al build e caricato a runtime da `BuildSecrets`.

### 2. PocketBase

Il server PocketBase deve avere le seguenti collection con API Rules:

**Collection `logged_workouts`:**
- Campi: `user` (relation → users), `user_name` (text), `name` (text), `date` (text), `exercises` (json), `exercise_data` (json), `volume` (number), `duration` (number), `likes` (number), `liked_by` (json)
- API Rules: List/Search/View/Update → `@request.auth.id != ""`

**Collection `social_graph`:**
- Campi: `from_user` (relation → users), `from_name` (text), `to_user` (relation → users), `status` (text)
- API Rules: Create/List → `@request.auth.id != ""`

**Collection `excercise`:**
- Campi: `name` (text), `bodyPart` (text), `equipment` (text), `instructions` (json), `imageUrl` (url), `category` (text), `level` (text), `force` (text), `mechanic` (text)
- API Rules: Create/List → `@request.auth.id != ""`

### 3. HTTPS

L'app comunica via HTTPS. Il server PocketBase deve avere un certificato SSL valido (es. Let's Encrypt via Nginx Proxy Manager).

## Build APK Release

```bash
# Pulisci build precedenti
dotnet clean src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android

# Pubblica APK Release
dotnet publish src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android -c Release /p:AndroidPackageFormats=apk
```

L'APK viene generato in:
```
src/GymTracker.Mobile/bin/Release/net10.0-android/publish/com.companyname.gymtracker.mobile-Signed.apk
```

## Installazione su dispositivo

```bash
# Installa APK su dispositivo/emulatore connesso
adb install src/GymTracker.Mobile/bin/Release/net10.0-android/publish/com.companyname.gymtracker.mobile-Signed.apk
```

Oppure trasferisci l'APK sul dispositivo e aprilo per installarlo.

## Permessi Android

L'app richiede i seguenti permessi (definiti in `AndroidManifest.xml`):

| Permesso | Motivo |
|----------|--------|
| `INTERNET` | Chiamate API ExerciseDB e PocketBase |
| `ACCESS_NETWORK_STATE` | Rilevamento stato connessione |

Nessun permesso aggiuntivo richiesto (no fotocamera, no contatti, no posizione).

## Configurazione per sviluppo

Per debugging su dispositivo/emulatore:

```bash
dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android -t:Run
```

## Note sulla privacy

- **Peso corporeo e misure**: non implementati nella versione attuale
- **Dati di allenamento**: salvati su PocketBase, accessibili solo dall'utente autenticato
- **Foto profilo**: salvate su PocketBase, accessibili via URL con token
- **Credenziali**: salvate localmente in `Preferences` per auto-login
- **API Key ExerciseDB**: gestita lato client, non esposta in chiaro nel codice versionato (`.env` in `.gitignore`)
- **Nessun dato condiviso con terze parti** oltre a ExerciseDB (fetch esercizi) e PocketBase (backend self-hosted)
