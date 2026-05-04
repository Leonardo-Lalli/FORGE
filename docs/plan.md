# Piano di Progetto - GymTracker Mobile

## Panoramica

- **Nome progetto**: GymTracker
- **Tipo**: App mobile .NET MAUI Android-first
- **Core functionality**: Diario per allenamenti in palestra con tracking esercizi, pesi, progressi e funzionalità social
- **Target utenti**: Principianti e appassionati di fitness che vogliono tracciare i progressi

## Architecture

### Stack tecnologico

- **Framework**: .NET MAUI 8.x
- **Target**: Android (iOS secondario)
- **Pattern**: MVVM con CommunityToolkit.Mvvm
- **Navigazione**: Shell
- **Persistenza locale**: SQLite (sqlite-net-pcl)
- **HTTP**: HttpClient con System.Text.Json
- **Backend**: API remota per social (amici, competizioni)

### Struttura progetto

```
/Platforms/Android
/Platforms/iOS        (opzionale)
/src
  /Models              # Entità (Exercise, Workout, Set, BodyWeight, User, Friend)
  /Data               # Repositories, Database context
  /Services           # Business logic, API clients
  /ViewModels         # MVVM ViewModels
  /Views/Pages        # UI XAML
  /Views/Controls     # Custom controls
  /Converters        # Value converters
  /Resources         # Assets, styles
/tests               # Unit tests opzionali
docs
```

## Iterazioni

### Iteration 1: Struttura base e catalogo esercizi

**Obiettivo**: Creare progetto MAUI funzionante con shell navigation e catalogo esercizi

**Scope**:
- Setup progetto MAUI con target Android
- Shell navigation con tabbar (Home, Workout, History, Stats, Profile)
- Database SQLite con Migrations per Exercise, MuscleGroup
- Catalogo esercizi precompilato (30-40 esercizi)
- Pagina Catalogo con raggruppamento per gruppo muscolare
- Loading state / empty state / error state

**Criteri di accettazione**:
- [ ] L'app compila e gira su Android emulator
- [ ] Shell navigation funziona tra le tab principali
- [ ] Database SQLite si crea correttamente al primo avvio
- [ ] Il catalogo esercizi mostra almeno 30 esercizi divisi per gruppo muscolare
- [ ] Ricerca esercizio funziona

**Rischi**:
- Dati esercizi iniziali devono essere coerenti e completi

---

### Iteration 2: Registrazione allenamento

**Obiettivo**: Permettere all'utente di registrare un allenamento completo

**Scope**:
- Modello Workout con data, durata, collezione di ExerciseSet
- Modello ExerciseSet con Exercise, Weight, Reps, SetNumber
- Pagina Nuovo Allenamento con aggiunta esercizi
- Pagina Dettaglio esercizio per Serie/Peso/Reps
- Salvataggio allenamento su SQLite
- Pagina Storico Allenamenti
- Pagina Dettaglio Allenamento passato

**Criteri di accettazione**:
- [ ] L'utente può avviare un nuovo allenamento
- [ ] L'utente può aggiungere esercizi dal catalogo
- [ ] Per ogni esercizio può aggiungere serie con peso e ripetizioni
- [ ] L'allenamento viene salvato su SQLite
- [ ] Lo storico mostra gli allenamenti passati
- [ ] Il dettaglio di un allenamento passato è consultabile

**Rischi**:
- Gestione corretta delle relazioni in SQLite (Workout -> ExerciseSets -> Exercise)

---

### Iteration 3: Tracking peso corporeo

**Obiettivo**: Permettere all'utente di tracciare il peso corporeo

**Scope**:
- Modello BodyWeight con data e peso
- Pagina per inserimento peso corporeo
- Storage su SQLite
- Grafico semplice (line chart custom) per progressione peso
- Visualizzazione nella dashboard

**Criteri di accettazione**:
- [ ] L'utente può inserire il peso corporeo
- [ ] Il peso viene salvato su SQLite
- [ ] Un grafico mostra l'andamento nel tempo

**Rischi**:
- Implementazione chart custom o use library

---

### Iteration 4: Statistiche e dashboard

**Obiettivo**: Mostrare statistiche di progresso all'utente

**Scope**:
- Query aggregate per esercizio (peso max storico, volume totale)
- Statistiche settimanali (numero allenamenti)
- Dashboard principale con schede informative
- Pagina Statistiche dedicata

**Criteri di accettazione**:
- [ ] Dashboard mostra numero allenamenti ultima settimana
- [ ] Dashboard mostra peso max per esercizi principali
- [ ] Pagina statistiche mostra dettagli completi

**Rischi**:
- Query efficienti su SQLite con dati crescenti

---

### Iteration 5: Backend e autenticazione (TBD)

**Obiettivo**: Preparare il layer per funzionalità social

**Scope**:
- Modello User con ID, username
- Servizio API astratto (da implementare con backend reale)
- Autenticazione base (login/register mock o reale)
- Struttura per FriendRequest

**Criteri di accettazione**:
- [ ] Layer API configurato
- [ ] Modelli utente definiti

**Rischi**:
- Definizione API backend non ancora disponibile

---

### Iteration 6: Gestione amici

**Obiettivo**: Permettere all'utente di aggiungere amici e vedere i loro allenamenti

**Scope**:
- Pagina Ricerca utenti
- Invio richieste di amicizia
- Accettazione/rifiuto richieste
- Lista amici
- Pagina Feed amici (ultimi allenamenti)
- Competizione: comparing stats con amici

**Criteri di accettazione**:
- [ ] L'utente può cercare altri utenti
- [ ] L'utente può inviare richiesta di amicizia
- [ ] L'utente riceve e gestisce richieste pendenti
- [ ] La lista amici è consultabile
- [ ] Gli allenamenti degli amici sono visibili

**Rischi**:
- Backend required per funzionamento reale

---

### Iteration 7: Piani di allenamento

**Obiettivo**: Offrire piani strutturati per principianti

**Scope**:
- Modello WorkoutPlan con elenco esercizi
- 2 piani base precaricati (Schema A/B 2 giorni, Schema 3 giorni)
- Pagina Piani con lista
- Pagina Dettaglio piano
- Avvio allenamento da piano

**Criteri di accettazione**:
- [ ] Almeno 2 piani predefiniti sono presenti
- [ ] L'utente può consultare i dettagli di ogni piano
- [ ] L'utente può avviare un allenamento da un piano

**Rischi**:
- Dettagli esercizi nei piani devono essere coerenti con catalogo

---

## Test matrix

Vedi `docs/test-matrix.md` per la matrice di test dettagliata.

## Definition of Done per iterazione

Ogni iterazione deve soddisfare:

1. Codice compila senza errori
2. Feature testabile manualmente
3. Stati UI gestiti (loading, error, empty, success)
4. Dati persistono correttamente su SQLite
5. Documentazione iterazione aggiornata in `docs/iterations/`

## Prossimi passi

1. Executable build verificato sull'emulatore Android
2. Shell navigation funzionante
3. Iterazione 1 completata