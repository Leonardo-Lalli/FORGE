<div align="center">

<img src="assets/screenshots/dashboard.jpeg" width="120" style="border-radius:24px" />

# FORGE

### Il diario di allenamento sociale

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/maui/)
[![PocketBase](https://img.shields.io/badge/PocketBase-Backend-000000?logo=pocketbase)](https://pocketbase.io/)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1.500%2B%20esercizi-22C55E)](https://oss.exercisedb.dev)
[![MVVM](https://img.shields.io/badge/Architecture-MVVM%20%7C%20CommunityToolkit-7C3AED)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Tests](https://img.shields.io/badge/Tests-36%20passed-brightgreen)](tests/)
[![License](https://img.shields.io/badge/License-MIT-blue)](LICENSE)

**Trasforma ogni ripetizione in progresso. Sfida i tuoi amici. Supera i tuoi limiti.**

</div>

---

## Cos'è FORGE

FORGE è un'app Android per il tracking degli allenamenti in palestra, con un'anima social. Registra esercizi, serie e ripetizioni. Segui i tuoi amici. Sblocca achievement. Guarda i tuoi progressi con statistiche dettagliate.

| Problema | Soluzione FORGE |
|----------|----------------|
| Non ricordi i pesi della volta scorsa | Storico allenamenti con dettaglio serie per serie |
| Non sai quali esercizi fare per un muscolo | Catalogo 1.500+ esercizi con GIF, ricerca e filtri |
| Ti alleni da solo e perdi motivazione | Feed amici, like, streak settimanali, achievement |

---

## Funzionalità

| Categoria | Feature |
|-----------|---------|
| 🏋️ **Allenamento** | Ricerca esercizi (1.500+ con GIF), serie kg×reps, checkmark completamento, rest timer, foto progresso, minimize/draft |
| 📊 **Statistiche** | Grafico volume, top lifts, calendario mensile, filtri WEEK/MONTH/3M/YEAR/ALL |
| 👥 **Social** | Feed amici, like ♥, follow/unfollow, ricerca utenti live, richieste di amicizia |
| 🏆 **Achievement** | 48 badge da sbloccare, tracking automatico, vetrina nel profilo |
| 👤 **Profilo** | Avatar, bio, statistiche, storico allenamenti, badge sbloccati |
| 🎨 **UI** | Doppio tema chiaro/scuro, font Inter/Lexend/Space Grotesk, design Stitch |
| 📱 **Offline** | SQLite locale con sync automatico quando torna la connessione |
| 📁 **CSV** | Import/export allenamenti per backup |
| 🔒 **Sicurezza** | SecureStorage per password, HTTPS, API rules row-level, admin panel bloccato |

---

## Screenshot

<div align="center">

| Dashboard | Feed | Stats |
|:---:|:---:|:---:|
| <img src="assets/screenshots/dashboard.jpeg" width="200" /> | <img src="assets/screenshots/feed.jpeg" width="200" /> | <img src="assets/screenshots/stats.jpeg" width="200" /> |

| Active Workout | Profile | Achievements |
|:---:|:---:|:---:|
| <img src="assets/screenshots/active-workout1.jpeg" width="200" /> | <img src="assets/screenshots/profile.jpeg" width="200" /> | <img src="assets/screenshots/login.jpeg" width="200" /> |

</div>

---

## Architettura

```
┌──────────────────────────────────────────────────┐
│                  .NET MAUI App                    │
│  ┌──────────┐  ┌───────────┐  ┌──────────┐      │
│  │  Views   │  │ ViewModels│  │ Services │      │
│  │  XAML    │◄─┤ MVVM      │◄─┤ Business │      │
│  │  puro    │  │ Toolkit   │  │ Logic    │      │
│  └──────────┘  └───────────┘  └────┬─────┘      │
│                                    │             │
│         ┌──────────────────────────┼──────┐      │
│    ┌────▼────┐   ┌──────────┐  ┌──▼───┐ │      │
│    │PocketBase│  │ExerciseDB│  │SQLite│ │      │
│    │ Auth +   │  │ v1 API   │  │Locale│ │      │
│    │ Social   │  │ 1.500+ex │  │Cache │ │      │
│    └─────────┘  └──────────┘  └──────┘ │      │
└─────────────────────────────────────────┘      │
```

### Stack

| Layer | Tecnologia |
|-------|-----------|
| Framework | .NET MAUI 10 (Android-first) |
| UI Pattern | MVVM con CommunityToolkit.Mvvm 8.4 |
| Navigation | Shell (3 tab + 8 route) |
| Backend | PocketBase self-hosted (auth, social, storage) |
| API | ExerciseDB v1 (1.500+ esercizi, gratuito) |
| Persistenza | SQLite (sqlite-net-pcl) |
| Test | xUnit (36 test) |
| Font | Inter, Lexend, Space Grotesk (Google Fonts) |

---

## Download APK

L'APK Release compilato è disponibile nella root del repository:

📦 **[FORGE.apk](FORGE.apk)**

> Richiede Android 7.0+ e connessione Internet per le API.

---

## Sviluppo

### Prerequisiti
- .NET 10 SDK + MAUI workload
- Dispositivo Android o emulatore (Android 7+)
- Server PocketBase (configurabile via `.env`)

### Setup rapido

```bash
# 1. Clona
git clone https://github.com/Leonardo-Lalli/FORGE.git
cd FORGE

# 2. Configura
cp .env.example .env
# Modifica .env con l'URL del tuo server PocketBase

# 3. Build
dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android

# 4. Test
dotnet test tests/GymTracker.Mobile.Tests/

# 5. Pubblica APK
dotnet publish src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android -c Release /p:AndroidPackageFormats=apk
```

### Struttura

```text
├── src/GymTracker.Mobile/       # Progetto MAUI
│   ├── Models/                  # Entità dominio + DTO
│   ├── ViewModels/              # MVVM ViewModels (12)
│   ├── Views/                   # XAML Views (10)
│   ├── Services/                # Business logic (11)
│   ├── Converters/              # Value converters (3)
│   └── Resources/               # Stili, font, immagini
├── tests/GymTracker.Mobile.Tests/  # Test xUnit (36)
├── tools/
│   ├── ExerciseImporter/        # Import esercizi su PocketBase
│   └── pb_hooks/                # Hook PocketBase (FCM)
├── docs/                        # Documentazione
└── FORGE.apk                    # APK Release
```

### Branch

| Branch | Scopo |
|--------|-------|
| `main` | Versione stabile, pronta per la distribuzione |
| `develop` | Sviluppo attivo, nuove feature e fix |
| `feature/*` | Feature branch (da mergiare in develop) |

---

## Documentazione

| Documento | Contenuto |
|-----------|-----------|
| [`docs/spec.md`](docs/spec.md) | Specifica prodotto, epic, user stories, criteri accettazione |
| [`docs/plan.md`](docs/plan.md) | Piano iterazioni con stato |
| [`docs/architecture.md`](docs/architecture.md) | Architettura tecnica |
| [`docs/project-journal.md`](docs/project-journal.md) | Diario di sviluppo completo |
| [`docs/security-hardening.md`](docs/security-hardening.md) | Guida sicurezza PocketBase + Nginx |
| [`docs/test-matrix.md`](docs/test-matrix.md) | Matrice 42 test manuali |
| [`docs/api-notes.md`](docs/api-notes.md) | Note tecniche API |

---

<div align="center">

**FORGE** — Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.

</div>
