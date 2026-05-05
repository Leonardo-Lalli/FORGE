# Test Matrix - GymTracker Mobile

## 1. Regole di lettura

- Questo documento è derivato dalla fase di planning: le evidenze sono pianificate, non ancora eseguite.
- `Manuale`, `Automatico ora`, `Automatico più avanti`: usare `Si` o `No`.
- `Evidenza prevista`: comando, nota di verifica o `Da eseguire`.
- `Automatico ora` indica controlli realistici da introdurre durante l'iterazione corrente, senza UI automation.

## 2. Matrice principale

| ID | Requisito o scenario | Categoria | Manuale | Automatico ora | Automatico più avanti | Iterazione target | Evidenza prevista | Note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| TM-01 | Build progetto MAUI senza errori | Build | No | Si | No | IT-01 | `dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj` | Smoke minimo da ripetere ogni iterazione |
| TM-02 | Avvio app e navigazione 5 tab Shell | Navigation | Si | No | No | IT-01 | Da eseguire | Verifica visiva su emulatore |
| TM-03 | Tema dark Stitch applicato correttamente | UI state | Si | No | No | IT-01 | Da eseguire | Colori #FF6B2B, font SORA/INTER |
| TM-04 | Fetch esercizi da ExerciseDB API per bodyPart | API | Si | Si | No | IT-02 | `dotnet test` | Mock HTTP per test unitario |
| TM-05 | Ricerca esercizio per nome via API | API | Si | Si | No | IT-02 | `dotnet test` | Test service layer |
| TM-06 | Filtro per attrezzatura funziona | Input | Si | Si | No | IT-02 | `dotnet test` | Verifica parametro equipment |
| TM-07 | Dettaglio esercizio mostra GIF e istruzioni | UI state | Si | No | No | IT-02 | Da eseguire | Verifica load immagine remota |
| TM-08 | Cache SQLite salva esercizi usati | Persistence | Si | Si | No | IT-02 | `dotnet test` | Test ExerciseCacheRepository |
| TM-09 | Catalogo offline mostra esercizi in cache | Persistence | Si | Si | No | IT-02 | `dotnet test` | Simula assenza rete |
| TM-10 | Stato loading durante fetch API | UI state | Si | No | No | IT-02 | Da eseguire | ActivityIndicator visibile |
| TM-11 | Stato error API con pulsante retry | UI state | Si | Si | No | IT-02 | `dotnet test` | Test ViewModel ErrorMessage + retry command |
| TM-12 | Stato empty: nessun risultato ricerca | UI state | Si | Si | No | IT-02 | `dotnet test` | Test ViewModel IsEmpty |
| TM-13 | Avvio nuovo allenamento vuoto | Workflow | Si | No | No | IT-03 | Da eseguire | Tap da Dashboard |
| TM-14 | Aggiunta esercizio con 4 serie (peso + reps) | Data | Si | Si | No | IT-03 | `dotnet test` | Validazione input e creazione model |
| TM-15 | Validazione peso > 0, reps > 0 | Input | Si | Si | No | IT-03 | `dotnet test` | Test validazione ViewModel |
| TM-16 | Rimozione esercizio/serie prima del salvataggio | Data | Si | Si | No | IT-03 | `dotnet test` | Test stato ViewModel |
| TM-17 | Salvataggio allenamento su SQLite | Persistence | Si | Si | No | IT-03 | `dotnet test` | Test WorkoutRepository |
| TM-18 | Storico mostra allenamenti ordinati per data | UI state | Si | Si | No | IT-03 | `dotnet test` | Test ordinamento query SQLite |
| TM-19 | Dettaglio allenamento passato completo | Navigation | Si | No | No | IT-03 | Da eseguire | Navigazione con parametro |
| TM-20 | Inserimento peso corporeo | Data | Si | Si | No | IT-04 | `dotnet test` | Test salvataggio SQLite |
| TM-21 | Inserimento misure corporee | Data | Si | Si | No | IT-04 | `dotnet test` | Test validazione e salvataggio |
| TM-22 | Grafico andamento peso visibile | UI state | Si | No | No | IT-04 | Da eseguire | Verifica rendering chart |
| TM-23 | Peso/misure MAI sincronizzati con backend | Persistence | Si | Si | No | IT-04 | `dotnet test` | Verifica assenza chiamate Firebase per body data |
| TM-24 | Registrazione Firebase Auth email/password | API | Si | Si | No | IT-05 | `dotnet test` | Mock Firebase Auth HTTP |
| TM-25 | Login Firebase con credenziali valide | API | Si | Si | No | IT-05 | `dotnet test` | Mock Firebase Auth HTTP |
| TM-26 | Errore login: credenziali errate | API | Si | Si | No | IT-05 | `dotnet test` | Test mapping errori Firebase -> messaggi IT |
| TM-27 | Token salvato in Preferences e persistito | Persistence | Si | Si | No | IT-05 | `dotnet test` | Test Preferences read/write |
| TM-28 | Sincronizzazione allenamento con Firebase | API | Si | Si | No | IT-05 | `dotnet test` | Test SyncService + FirebaseDatabaseService |
| TM-29 | Coda sync: allenamento offline sincronizzato dopo rete | API | Si | Si | No | IT-05 | `dotnet test` | Test SyncService retry |
| TM-30 | Logout cancella token e reindirizza a Login | Navigation | Si | Si | No | IT-05 | `dotnet test` | Test ViewModel logout |
| TM-31 | Ricerca utenti per username | Social | Si | Si | No | IT-06 | `dotnet test` | Test SocialService |
| TM-32 | Invio richiesta amicizia | Social | Si | Si | No | IT-06 | `dotnet test` | Test SocialService |
| TM-33 | Accettazione richiesta amicizia | Social | Si | Si | No | IT-06 | `dotnet test` | Test cambio stato |
| TM-34 | Rifiuto richiesta amicizia | Social | Si | Si | No | IT-06 | `dotnet test` | Test rimozione richiesta |
| TM-35 | Lista amici consultabile | UI state | Si | Si | No | IT-06 | `dotnet test` | Test ViewModel |
| TM-36 | Feed ultimi 20 allenamenti amici | UI state | Si | No | No | IT-06 | Da eseguire | Richiede dati Firebase popolati |
| TM-37 | Leaderboard settimanale ordinata per volume | Data | Si | Si | No | IT-06 | `dotnet test` | Test ordinamento e calcolo volume |
| TM-38 | Confronto diretto tra due amici | UI state | Si | Si | No | IT-06 | `dotnet test` | Test FriendCompareViewModel |
| TM-39 | Streak: calcolo giorni consecutivi | Data | Si | Si | No | IT-06 | `dotnet test` | Test logica streak |
| TM-40 | Streak corretto dopo interruzione | Data | Si | Si | No | IT-06 | `dotnet test` | Test edge case: buchi nel calendario |
| TM-41 | Stato empty: nessun amico | UI state | Si | Si | No | IT-06 | `dotnet test` | Test ViewModel IsEmpty con CTA |
| TM-42 | Dashboard mostra ultimo allenamento | UI state | Si | Si | No | IT-07 | `dotnet test` | Test aggregazione ViewModel |
| TM-43 | Dashboard mostra streak corrente | UI state | Si | Si | No | IT-07 | `dotnet test` | Test calcolo streak |
| TM-44 | Dashboard mostra posizione leaderboard | UI state | Si | Si | No | IT-07 | `dotnet test` | Test posizione utente |
| TM-45 | Statistiche: peso max per esercizio | Data | Si | Si | No | IT-07 | `dotnet test` | Test query aggregata SQLite |
| TM-46 | Grafico volume settimanale | UI state | Si | No | No | IT-07 | Da eseguire | Verifica rendering |
| TM-47 | Dashboard si aggiorna dopo nuovo allenamento | Workflow | Si | Si | No | IT-07 | `dotnet test` | Test notifica / refresh |
| TM-48 | 3 piani base precaricati su SQLite | Data | Si | Si | No | IT-08 | `dotnet test` | Test seed data |
| TM-49 | Dettaglio piano mostra giorni ed esercizi | UI state | Si | Si | No | IT-08 | `dotnet test` | Test ViewModel |
| TM-50 | Avvia allenamento da piano con esercizi precaricati | Workflow | Si | Si | No | IT-08 | `dotnet test` | Test navigazione + popolamento ActiveWorkout |

## 3. Aree minime da coprire

### Input
- Peso vuoto, zero, negativo, non numerico (bloccare submit)
- Ripetizioni vuote, zero, negative, non numeriche
- Misure corporee vuote o fuori range realistico (>300cm)
- Ricerca esercizi: stringa vuota, caratteri speciali, nome inesistente
- Username duplicato in registrazione

### API (ExerciseDB)
- Risposta 200 con dati validi
- Risposta 200 con array vuoto (nessun esercizio trovato)
- Timeout dopo 10s
- Errore 429 (rate limit RapidAPI) → messaggio + retry after
- Errore 401 (API key invalida) → messaggio configurazione
- JSON malformato o campi mancanti → mapping difensivo

### API (Firebase)
- Registrazione: email già in uso, password debole
- Login: credenziali errate, utente non trovato
- Token scaduto → refresh automatico
- Errore di rete durante sync
- Conflitto sync (stesso allenamento inviato due volte)

### UI state
- Loading: ActivityIndicator su catalog, login, sync, dashboard iniziale
- Error: messaggio + retry su ogni fetch remoto
- Empty: messaggio guidato + CTA su storico, amici, feed, leaderboard
- Success: dati caricati, nessun messaggio invasivo
- Dark theme Stitch coerente su tutte le pagine

### Navigation
- Shell tab bar: 5 tab raggiungibili
- Navigazione di dettaglio con parametri (exerciseId, workoutId, friendId)
- Back navigation: ritorno alla pagina precedente
- Login → Dashboard dopo autenticazione
- Dashboard → Logout → Login
- Navigazione durante allenamento attivo: stato preservato

### Persistence
- SQLite: creazione database al primo avvio
- Esercizi in cache: inserimento, limite 50, LRU
- Allenamenti: CRUD completo, IsSynced flag
- Peso/misure: CRUD, dati solo locali
- Riavvio app: dati locali integri
- Aggiornamento app: migration SQLite se schema cambia

### Device
- Avvio su emulatore Android (IT-01)
- Assenza rete: funzionalità locali integre (cache esercizi, storico, peso)
- Rotazione schermo: UI non perde stato
- Permessi di rete: gestione graceful

## 4. Note su test automatici

### Test realistici subito (per ogni iterazione)
- `dotnet build` come smoke check
- Test unitari su ViewModel: validazione input, gestione stati (IsBusy, ErrorMessage, HasData)
- Test unitari su Service: chiamate HTTP con mock `HttpMessageHandler`
- Test unitari su Repository SQLite: CRUD con database in-memory o file temporaneo
- Test di mapping DTO: parsing JSON con campi mancanti
- Test di logica: streak, ordinamento leaderboard

### Test da rimandare
- UI automation end-to-end (Appium o MAUI.UITesting) — richiede infrastruttura
- Test performance: scroll catalogo con GIF, query SQLite con 1000+ allenamenti
- Test multi-dispositivo: sync conflitti reali
- Test Firebase Rules di sicurezza

### Comandi utili
```bash
# Smoke build
dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj

# Unit test (quando il progetto test esiste)
dotnet test tests/GymTracker.UnitTests/GymTracker.UnitTests.csproj

# Test specifico per service
dotnet test --filter "FullyQualifiedName~ExerciseService"
```

Per una strategia automatica più profonda è opportuno usare la skill `maui-automatic-testing` a fine iterazione.
