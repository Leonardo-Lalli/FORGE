# IT-02 — Catalogo Esercizi, Allenamento e Piani

**Data:** IT-02 + IT-03 + IT-08 (accorpate)
**Branch:** `mockup`
**Stato:** ✅ Completata

## Obiettivo verificabile

Catalogo esercizi funzionante da ExerciseDB API, registrazione allenamento completa, piani di allenamento salvabili.

## In scope

### Catalogo (ex IT-02)
- `ExerciseApiService` con HttpClient e header RapidAPI
- Ricerca per nome, filtro per gruppo muscolare e attrezzatura (chip orizzontali)
- DTO `ExerciseDbDto` con primaryMuscles, secondaryMuscles, equipment, instructions, images
- Cache esercizi su PocketBase collection `excercise` con URL immagini risolti
- `ResolveImageUrlAsync()` per seguire redirect HTTP (short URL bloccati da ISP)
- Immagini lazy-loading con overlay numero ordine esercizio

### Allenamento (ex IT-03)
- `ActiveWorkoutPage` ridisegnata da mockup `allenamento_attivo_nero_opaco_minimal_v2`
- `WorkoutPlan` con `WorkoutExercise` (nome, note, restSeconds) e `ExerciseSet` (peso, reps, completato)
- `ExerciseSet` come `ObservableObject` con `[ObservableProperty]` — tap sul cerchio ✓ riempie LimeGreen
- Entry numeriche per KG e REPS
- `BindableLayout` invece di `CollectionView` annidate (evita crash Android)
- Progress bar dinamica, nome scheda editabile
- Rest timer configurabile (5-600s), timer per esercizio
- Salvataggio su PocketBase `logged_workouts` con `exercise_data` JSON
- `WorkoutSavedMessage` via `WeakReferenceMessenger` per refresh Dashboard/Stats/Profilo
- Bottone ADD SET stile mockup

### Piani (ex IT-08)
- `StartSessionPage` con Quick Start, Create New Plan, Your Protocols
- `PlanStore`: salvataggio piani su `Preferences` come JSON
- 3 piani demo: Push Power, Leg Day Protocol, Core Stabilization
- Eliminazione piani salvati (✕ rossa)

## File creati/modificati

| File | Descrizione |
|------|-------------|
| `Services/ExerciseApiService.cs` | HTTP ExerciseDB, cache su PocketBase, resolve URL |
| `Models/Dto/ExerciseDbDto.cs` | DTO con tutti i campi ExerciseDB |
| `Models/WorkoutPlan.cs` | WorkoutPlan, WorkoutExercise, ExerciseSet |
| `ViewModels/ActiveWorkoutViewModel.cs` | Gestione allenamento attivo, ricerca, salvataggio |
| `ViewModels/StartSessionViewModel.cs` | Quick Start, Create Plan, Your Protocols |
| `Views/ActiveWorkoutPage.xaml` | UI allenamento completa |
| `Views/StartSessionPage.xaml` | UI Start Session |
| `Messages/WorkoutSavedMessage.cs` | Messaggio per refresh |

## Criteri di accettazione

- [x] Ricerca esercizi da ExerciseDB API per nome, muscolo e attrezzatura
- [x] Immagini esercizi caricate e cache su PocketBase
- [x] Avvio allenamento libero e da piano salvato
- [x] Aggiunta serie con peso e ripetizioni, completamento con ✓
- [x] Rimozione esercizi e serie
- [x] Salvataggio allenamento su PocketBase con data, volume, durata
- [x] Piani salvati persistenti tra riavvii
- [x] UI coerente con mockup per entrambi i temi

## Rischi e mitigazioni

| Rischio | Mitigazione |
|---------|-------------|
| Short URL ExerciseDB bloccati da ISP | `ResolveImageUrlAsync()` segue redirect HTTP, cache URL risolti su PocketBase |
| `CollectionView` annidate crash Android | Sostituite con `BindableLayout` su `VerticalStackLayout` |
| `Entry Mode=TwoWay` con `double`/`int` crash | Usati `Label` read-only + pulsanti `−`/`+` (poi migrati a Entry per mockup) |
| `ExerciseSet` non osservabile → UI non reagisce | Esteso `ObservableObject` con `[ObservableProperty]` |
