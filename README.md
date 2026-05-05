[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/kjGBNOdE)
<div align="center">

# GymTracker Mobile

Il diario di allenamento che trasforma la palestra in una sfida con i tuoi amici.

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/maui/)
[![Firebase](https://img.shields.io/badge/Firebase-Auth%20%7C%20Database-007AFF?logo=firebase&logoColor=white)](https://firebase.google.com/)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1300+%20esercizi-22C55E)](https://rapidapi.com/justin-WFnsXH_t6/api/exercisedb)
[![Stitch UI](https://img.shields.io/badge/UI-Stitch%20Iron%20Rank-7C3AED)](https://stitch.google.com/)
[![Workflow](https://img.shields.io/badge/workflow-Man--in--the--Loop-0F766E)](docs/method/man-in-the-loop.md)
[![Docs](https://img.shields.io/badge/docs-spec%20%7C%20plan%20%7C%20architecture%20%7C%20tests-2563EB)](docs/)

[Panoramica](#panoramica) ·
[Perché GymTracker](#perché-gymtracker) ·
[Funzionalità](#funzionalità) ·
[Architettura](#architettura) ·
[Skill e Workflow](#skill-e-workflow) ·
[Quick Start](#quick-start) ·
[Documenti](#documenti)

</div>

---

## Panoramica

**GymTracker Mobile** è un'app `.NET MAUI` Android-first pensata per chi si allena in palestra e vuole:

- Esplorare un catalogo di **1300+ esercizi** con GIF animate, filtrati per gruppo muscolare e attrezzatura.
- Registrare allenamenti con serie, ripetizioni e peso in pochi secondi.
- Tracciare peso corporeo e misure (petto, vita, braccia, gambe) con grafici di progresso.
- Competere con gli amici tramite **leaderboard**, **streak settimanali** e **confronto diretto** delle statistiche.

L'app usa **ExerciseDB API** come fonte primaria di esercizi, **Firebase** per autenticazione e funzionalità social, e **SQLite** per l'uso offline. L'interfaccia segue il Design System **"Performance Minimalist"** del progetto Stitch **"Iron Rank Fitness Social"** (dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`).

## Perché GymTracker

Andare in palestra senza tracciare i progressi è come guidare senza cruscotto: vai avanti, ma non sai se stai migliorando.

GymTracker risolve tre problemi reali:

| Problema | Soluzione |
| --- | --- |
| Non ricordo i pesi della volta scorsa | Storico completo con dettaglio serie per serie |
| Non so quali esercizi fare per un muscolo | Catalogo 1300+ esercizi con GIF e istruzioni |
| Mi alleno da solo e perdo motivazione | Amici, leaderboard e streak per una competizione sana |

## Funzionalità

### Esplora Esercizi
- **1300+ esercizi** da ExerciseDB API con GIF animate
- Ricerca per nome, filtro per gruppo muscolare (petto, schiena, gambe...) e attrezzatura (manubri, bilanciere, corpo libero...)
- Cache offline SQLite: gli esercizi che usi di più sono sempre disponibili, anche senza rete

### Registra Allenamenti
- Avvia un allenamento in un tap dalla Dashboard
- Aggiungi esercizi, registra serie con peso (kg) e ripetizioni
- Salva tutto con data e ora
- Storico completo navigabile dal più recente

### Traccia il Tuo Corpo
- Registra peso corporeo dopo ogni sessione
- Misura petto, vita, fianchi, braccia, gambe
- Grafici di andamento nel tempo
- Dati 100% locali: mai condivisi con il backend

### Competi con gli Amici
- Cerca amici per username e connettiti
- **Feed**: vedi gli ultimi allenamenti dei tuoi amici
- **Leaderboard**: classifica settimanale per volume di allenamento (kg × ripetizioni)
- **Streak**: chi si allena più giorni di fila?
- **Confronto diretto**: affianca le tue statistiche con quelle di un amico

### Dashboard Intelligente
- Riepilogo rapido: ultimo allenamento, streak, posizione in classifica, peso attuale
- Accesso immediato a: Nuovo Allenamento, Catalogo, Amici
- Grafici di progresso per ogni esercizio

### Piani per Principianti
- 3 piani base precaricati: Full Body 2gg, Split 3gg, Push/Pull/Legs
- Ogni piano indica esercizi, serie e ripetizioni consigliate
- Avvia un allenamento direttamente da un giorno del piano

## Architettura

```
┌─────────────────────────────────────────────────┐
│                  .NET MAUI App                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐       │
│  │  Views   │  │ViewModels│  │ Services │       │
│  │  XAML    │◄─┤ MVVM     │◄─┤ Business │       │
│  │  Stitch  │  │ Toolkit  │  │ Logic    │       │
│  └──────────┘  └──────────┘  └────┬─────┘       │
│                                   │              │
│         ┌─────────────────────────┼──────┐       │
│         │                         │      │       │
│    ┌────▼────┐   ┌──────────┐  ┌──▼───┐ │       │
│    │ SQLite   │   │ Exercise │  │Fire- │ │       │
│    │ (locale) │   │  DB API  │  │base  │ │       │
│    │ cache,   │   │ 1300+    │  │Auth +│ │       │
│    │ workout, │   │ esercizi │  │DB    │ │       │
│    │ body     │   │          │  │      │ │       │
│    └─────────┘   └──────────┘  └──────┘ │       │
└──────────────────────────────────────────┘       │
```

### Stack tecnologico

| Tecnologia | Ruolo |
| --- | --- |
| `.NET MAUI 8+` | Framework cross-platform Android-first |
| `CommunityToolkit.Mvvm` | MVVM: ObservableProperty, RelayCommand |
| `Shell` | Navigazione a tab + route di dettaglio |
| `sqlite-net-pcl` | Persistenza locale (cache, allenamenti, misure) |
| `HttpClient` + `System.Text.Json` | Chiamate API ExerciseDB e Firebase REST |
| `Firebase Auth` | Registrazione e login email/password |
| `Firebase Realtime DB / Firestore` | Dati social (utenti, amici, leaderboard) |
| `Google Stitch` | Design System: dark theme, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter |

### Flusso offline-first

1. Scrivi sempre su SQLite (allenamenti, peso, misure)
2. Sincronizza con Firebase quando la rete è disponibile
3. Esercizi in cache: sempre accessibili, anche senza connessione
4. Peso e misure: solo locali, mai sul cloud

## Skill e Workflow

Il progetto segue il workflow **Man-in-the-Loop**: ogni iterazione è pianificata, costruita, revisionata, testata e documentata. Le skill locali guidano l'agente AI in ogni fase.

| Skill | Fase | Output |
| --- | --- | --- |
| `maui-prd` | Definizione | `docs/spec.md` |
| `prd-to-plan` | Pianificazione | `docs/plan.md`, `docs/architecture.md`, `docs/test-matrix.md` |
| `maui-expert` | Build | Codice MAUI MVVM, XAML, Shell |
| `maui-automatic-testing` | Test | Test ViewModel, Service, Build |
| `man-in-the-loop-workflow` | Intero ciclo | Controllo scope, review, documentazione |

| Fase | Cosa fare | Cosa NON fare |
| --- | --- | --- |
| Planning | Scrivere acceptance criteria verificabili | Mescolare spec, architettura e codice |
| Build | Implementare una feature per iterazione | Aggiungere pacchetti o refactoring non richiesti |
| Review | Verificare MVVM, naming, error handling | Fare review senza aver letto il codice |
| Testing | Testare loading, error, empty, success | Fingere che la build sia un test |
| Docs | Aggiornare `docs/iterations/` a fine iterazione | Scrivere documentazione inutile |

## Quick Start

1. **Clona** il repository
2. Leggi [`docs/method/man-in-the-loop.md`](docs/method/man-in-the-loop.md) per capire il metodo
3. Leggi [`docs/spec.md`](docs/spec.md) per la specifica completa
4. Consulta [`docs/plan.md`](docs/plan.md) per il piano in 8 iterazioni
5. Per sviluppare: avvia la skill `maui-expert` e segui il workflow

### Prerequisiti

- .NET 8 SDK + MAUI workload
- Emulatore Android (o dispositivo fisico)
- Chiave API ExerciseDB ([RapidAPI](https://rapidapi.com/justin-WFnsXH_t6/api/exercisedb))
- Progetto Firebase configurato (Auth + Database)

## Documenti

| Documento | Scopo |
| --- | --- |
| [`docs/spec.md`](docs/spec.md) | PRD completo di GymTracker |
| [`docs/plan.md`](docs/plan.md) | Piano in 8 iterazioni verificabili |
| [`docs/architecture.md`](docs/architecture.md) | Architettura MAUI + Firebase + ExerciseDB |
| [`docs/test-matrix.md`](docs/test-matrix.md) | 50 test pianificati su 8 categorie |
| [`docs/method/man-in-the-loop.md`](docs/method/man-in-the-loop.md) | Metodologia didattica |

## Struttura Repository

```text
.
├── .agents/skills/              # Skill per OpenCode (maui-prd, maui-expert, etc.)
├── docs/                        # Spec, plan, architettura, test, iterazioni
│   ├── spec.md                  # PRD GymTracker
│   ├── plan.md                  # Piano 8 iterazioni
│   ├── architecture.md          # Architettura tecnica
│   ├── test-matrix.md           # Matrice di test
│   ├── method/                  # Metodo Man-in-the-Loop
│   ├── iterations/              # Log iterazioni
│   └── history/                 # Sessioni storiche
├── src/GymTracker.Mobile/       # Progetto MAUI
│   ├── Models/
│   ├── Data/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   └── Resources/
└── README.md
```

## Prossimi Passi

- [x] PRD e pianificazione completati
- [ ] IT-01: Bootstrap MAUI + Shell + Design System "Iron Rank Fitness Social"
- [ ] IT-02: Catalogo esercizi da ExerciseDB API
- [ ] IT-03-08: Allenamento, corpo, social, statistiche, piani

---

<div align="center">

**GymTracker Mobile** — Trasforma ogni ripetizione in progresso. Sfida i tuoi amici. Supera i tuoi limiti.

</div>
