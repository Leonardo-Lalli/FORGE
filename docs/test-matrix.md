# Test Matrix — FORGE (GymTracker Mobile)

## Stato: Eseguiti e verificati

| ID | Test | Categoria | Esito | Evidenza |
|----|------|-----------|-------|----------|
| TM-01 | Build progetto senza errori | Build | ✅ | `dotnet build -f net10.0-android` → 0 errori |
| TM-02 | Avvio app e navigazione 3 tab | Navigation | ✅ | Dashboard → Feed → Stats, swipe tra tab |
| TM-03 | Tema dark/light toggle | UI | ✅ | Settings → toggle, tutte le pagine cambiano colore |
| TM-04 | Login con credenziali valide | Auth | ✅ | LoginPage → Dashboard |
| TM-05 | Login con credenziali errate | Auth | ✅ | Messaggio errore visibile |
| TM-06 | Registrazione nuovo utente | Auth | ✅ | Account creato su PocketBase, redirect Dashboard |
| TM-07 | Auto-login dopo riavvio | Auth | ✅ | Token salvato in Preferences, salta LoginPage |
| TM-08 | Logout | Auth | ✅ | Settings → Logout → LoginPage |
| TM-09 | Ricerca esercizi da ExerciseDB API | API | ✅ | Search bar in ActiveWorkout, risultati con immagini |
| TM-10 | Filtro per gruppo muscolare | API | ✅ | Chip muscoli, fetch API corretto |
| TM-11 | Filtro per attrezzatura | API | ✅ | Chip attrezzatura, fetch API corretto |
| TM-12 | Cache esercizi su PocketBase | Persistenza | ✅ | URL immagini risolti salvati in `excercise` |
| TM-13 | Avvio allenamento libero (Quick Start) | Workflow | ✅ | StartSession → Quick Start → ActiveWorkout |
| TM-14 | Avvio allenamento da piano salvato | Workflow | ✅ | StartSession → Protocol card → ActiveWorkout con esercizi |
| TM-15 | Aggiunta serie con peso e reps | Input | ✅ | Entry numeriche, validazione |
| TM-16 | Completamento serie (✓) | UI | ✅ | Tap cerchio → LimeGreen fill |
| TM-17 | Salvataggio allenamento su PocketBase | Persistenza | ✅ | `logged_workouts` popolato con exercise_data |
| TM-18 | Dashboard: streak settimanale | Logica | ✅ | Conteggio settimane consecutive, reset dopo 7+ giorni |
| TM-19 | Dashboard: Squad Activity | UI | ✅ | Avatar amici con allenamenti recenti |
| TM-20 | Dashboard: START WORKOUT → StartSession | Navigazione | ✅ | Pulsante funzionante |
| TM-21 | Feed: ricerca utenti live | UI | ✅ | Search bar con debounce 400ms, risultati |
| TM-22 | Feed: follow/unfollow utente | Social | ✅ | Bottone Follow → Requested, aggiornamento UI |
| TM-23 | Feed: post allenamenti amici | Social | ✅ | Lista workout con nome, esercizi, volume, durata |
| TM-24 | Feed: like/unlike allenamento | Social | ✅ | Cuore ♡ → ♥ LimeGreen, conteggio istantaneo |
| TM-25 | Stats: filtri temporali | UI | ✅ | WEEK/MONTH/3M/YEAR/ALL, feedback visivo |
| TM-26 | Stats: grafico volume | UI | ✅ | Barre settimanali, etichette data, divisori mese |
| TM-27 | Stats: top lifts | Logica | ✅ | 5 esercizi con peso max da exercise_data |
| TM-28 | Stats: calendario | UI | ✅ | Giorni mese, pallini su giorni allenamento |
| TM-29 | Profilo: avatar utente | UI | ✅ | Foto PocketBase o iniziali, tappabile per upload |
| TM-30 | Profilo: stat totali | Logica | ✅ | Workouts, Volume, Streak, ♥ Likes ricevuti |
| TM-31 | Profilo: recent forges | UI | ✅ | Lista allenamenti con titolo, data, durata, like, volume |
| TM-32 | Profilo: edit nome e bio | Input | ✅ | Overlay edit, salva su PocketBase |
| TM-33 | Profilo: upload foto | Input | ✅ | FilePicker → multipart upload → refresh avatar |
| TM-34 | Notifiche: friend requests | Social | ✅ | Lista richieste, ACCEPT/REJECT |
| TM-35 | Notifiche: like notifications | Social | ✅ | "User X liked your workout Y" visibile |
| TM-36 | Cambio tema runtime | UI | ✅ | Tutte le pagine, Shell inclusa, colori aggiornati |
| TM-37 | Stato loading su fetch dati | UI | ✅ | ActivityIndicator su Feed, Stats, Profilo |
| TM-38 | Stato empty: nessun dato | UI | ✅ | Messaggio "Nessun dato" o "Follow athletes..." |
| TM-39 | Stato error: rete assente | UI | ✅ | Messaggio errore con descrizione |
| TM-40 | Navigazione profilo da Dashboard | Navigazione | ✅ | Tap avatar "EL" → ProfilePage |
| TM-41 | Navigazione profilo da Feed | Navigazione | ✅ | Tap avatar → ProfilePage |
| TM-42 | Navigazione profilo da Stats | Navigazione | ✅ | Tap avatar → ProfilePage |

## Copertura per area

| Area | Test eseguiti | Stato |
|------|---------------|-------|
| Build | 1 | ✅ |
| Navigazione | 4 | ✅ |
| Auth | 5 | ✅ |
| API (ExerciseDB) | 4 | ✅ |
| Social (PocketBase) | 8 | ✅ |
| UI / Stati | 10 | ✅ |
| Input | 3 | ✅ |
| Persistenza | 3 | ✅ |
| Logica (streak, stats, likes) | 4 | ✅ |
| **TOTALE** | **42** | ✅ |

## Casi limite verificati

- **Ricerca vuota**: messaggio "No users found"
- **Nessun amico**: messaggio "Follow athletes to see their workouts here"
- **Nessun allenamento salvato**: stato empty in Stats e Profilo
- **Streak a 0 dopo pausa >7 giorni**: verificato con date simulate
- **Like già messo**: toggle unlike senza duplicati
- **Utente non loggato**: Stats e Profilo mostrano dati offline o messaggio
- **Login con credenziali vuote**: validazione lato ViewModel
- **Entry peso/reps vuote**: gestione gracefully (0 default)
