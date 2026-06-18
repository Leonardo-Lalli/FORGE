# PocketBase Schema — FORGE

## Come aggiungere il campo `photos` alla collection `logged_workouts`

### Passaggi (Admin UI PocketBase)

1. Apri PocketBase Admin: `https://YOUR_SERVER.duckdns.org/_/`
2. Autenticati con email/password admin
3. Vai su **Collections** → **logged_workouts**
4. Clicca **Edit collection** (icona matita)
5. Clicca **+ New field**
6. Seleziona tipo **JSON**
7. Imposta:
   - **Name**: `photos`
   - **Options**: lascia vuoto (default)
8. Clicca **Create**
9. Clicca **Save** in fondo alla pagina

### Verifica

Dopo aver aggiunto il campo, i workout salvati conterranno l'array `photos` visibile:
- Nell'Admin UI come colonna nella lista record
- Nelle risposte API come `"photos": ["data:image/jpeg;base64,..."]`

### Campo `photos` nella collection `excercise` (opzionale)

Per l'import dei 1.500 esercizi, la collection `excercise` deve avere i seguenti campi:

| Campo | Tipo | Note |
|-------|------|------|
| `exercise_id` | Plain text | ID ExerciseDB v1 (es. "01qpYSe") |
| `name` | Plain text | Nome esercizio capitalizzato |
| `bodyPart` | Plain text | Gruppo muscolare principale |
| `equipment` | Plain text | Attrezzatura |
| `instructions` | JSON | Array di stringhe con step |
| `imageUrl` | URL | URL GIF diretta (static.exercisedb.dev) |
| `category` | Plain text | Categoria (stesso di bodyPart) |
| `targetMuscles` | Plain text | Muscoli target |
| `secondaryMuscles` | Plain text | Muscoli secondari |

Se i campi non esistono, PocketBase li accetta comunque come JSON libero.

### Note tecniche

- PocketBase accetta campi non dichiarati nello schema: il payload JSON con `"photos": [...]` viene archiviato correttamente anche senza il campo schema
- Aggiungere il campo schema serve per: visibilità in Admin UI, validazione JSON, ricerca/indicizzazione
- Il campo `photos` nell'app è un array di stringhe base64 con prefisso `data:image/jpeg;base64,`
- Massimo 5 foto per allenamento (limite imposto lato app)
