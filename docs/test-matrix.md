# Test Matrix - GymTracker

## 1. Regole di lettura

- Questo documento è derivato dalla fase di planning: le evidenze sotto sono previste e non ancora eseguite.
- `Manuale`, `Automatico ora`, `Automatico più avanti`: usare `Si` o `No`.
- `Evidenza prevista`: comando, nota di verifica o `Da eseguire`.
- `Automatico ora` indica controlli realistici da introdurre durante l'iterazione corrente, senza richiedere una infrastruttura di UI automation avanzata.

## 2. Matrice principale

| ID | Requisito o scenario | Categoria | Manuale | Automatico ora | Automatico più avanti | Iterazione target | Evidenza prevista | Note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| TM-01 | Avvio app e apertura sezioni da Shell (Home, Workout, History, Stats, Profile) | Navigation | Si | No | No | IT-01 | Da eseguire | Smoke iniziale di bootstrap MAUI |
| TM-02 | Build del progetto MAUI e validazione di XAML e route | Build | No | Si | No | IT-01 | `dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj` | Check minimo da ripetere a ogni iterazione |
| TM-03 | Catalogo esercizi mostra almeno 30 esercizi per gruppo muscolare | Data | Si | Si | No | IT-01 | `dotnet test` | Verifica dati precaricati |
| TM-04 | Ricerca esercizio per nome funziona | Input | Si | Si | No | IT-01 | `dotnet test` | Filtro sulla lista |
| TM-05 | Avvio nuovo allenamento e aggiunta esercizi | Workflow | Si | No | No | IT-02 | Da eseguire | Flusso principale |
| TM-06 | Registrazione serie con peso e ripetizioni per esercizio | Data | Si | Si | No | IT-02 | `dotnet test` | Verifica modello ExerciseSet |
| TM-07 | Salvataggio allenamento completo su SQLite | Persistence | Si | Si | No | IT-02 | `dotnet test` | Test repository |
| TM-08 | Storico mostra allenamenti dal più recente | UI state | Si | No | No | IT-02 | Da eseguire | Ordinamento per data |
| TM-09 | Dettaglio allenamento passato consultabile | Navigation | Si | No | No | IT-02 | Da eseguire | Navigazione da History |
| TM-10 | Inserimento peso corporeo | Data | Si | Si | No | IT-03 | `dotnet test` | Entry con validazione |
| TM-11 | Grafico progressione peso corporeo nel tempo | UI state | Si | No | No | IT-03 | Da eseguire | Chart o custom view |
| TM-12 | Dashboard mostra statistiche aggregate | UI state | Si | No | No | IT-04 | Da eseguere | Query aggregate SQLite |
| TM-13 | Peso max storico per esercizio consultabile | Data | Si | Si | No | IT-04 | `dotnet test` | Query aggregation |
| TM-14 | Numero allenamenti ultima settimana | Data | Si | Si | No | IT-04 | `dotnet test` | Query con filter date |
| TM-15 | Layer API configurato per backend social | API | Si | No | No | IT-05 | Da eseguere | HttpClient setup |
| TM-16 | Ricerca altri utenti per username | Social | Si | No | Si | IT-06 | Da eseguire | Richiede backend |
| TM-17 | Invio richiesta amicizia | Social | Si | No | Si | IT-06 | Da eseguire | Richiede backend |
| TM-18 | Accettazione/rifiuto richieste amicizia | Social | Si | No | Si | IT-06 | Da eseguire | Richiede backend |
| TM-19 | Lista amici consultabile | Social | Si | No | Si | IT-06 | Da eseguire | Richiede backend |
| TM-20 | Feed amici mostra ultimi allenamenti | Social | Si | No | Si | IT-06 | Da eseguire | Richiede backend |
| TM-21 | Piani base per principianti presenti | Data | Si | Si | No | IT-07 | `dotnet test` | 2+ piani precaricati |
| TM-22 | Avvio allenamento da piano | Workflow | Si | No | No | IT-07 | Da eseguire | Navigazione con dati precompilati |

## 3. Aree minime da coprire

- Input: peso vuoto, peso non numerico, serie con valori validi.
- Data: coerenza SQLite con relazioni Workout -> ExerciseSets -> Exercise.
- UI: loading, error, empty, success su tutte le schermate.
- Navigation: Shell navigation tra tutte le tab.
- Persistence: allenamenti persistono dopo riavvio app.
- Social: flussi richiedono backend (mock o reale).
- Device: smoke Android, assenza rete per funzionalità locali.

## 4. Note su test automatici

Controlli automatici realistici da introdurre presto:

- `dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj` come smoke check minimo;
- test unitari su ViewModels per validazione input e gestione stati;
- test del service layer SQLite;
- test delle query aggregate per statistiche.

Controlli automatici da rimandare a più avanti:

- UI automation end-to-end;
- test delle funzionalità social (richiedono backend);
- verifiche device-specific più approfondite.

Per una strategia automatica più profonda è opportuno passare successivamente alla skill `maui-automatic-testing`.