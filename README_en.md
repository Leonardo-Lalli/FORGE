<div align="center">

> ⚠️ **HOBBY PROJECT FOR EDUCATIONAL PURPOSES** ⚠️  
> This app is developed by a student as a school project.  
> **It is not a commercial product.** The server is self-hosted on home hardware.  
> Expect bugs, limitations, and unplanned downtime.  
> Use for testing and demos, not for important data.

[🇬🇧 English](README_en.md) · [🇮🇹 Italiano](README.md) · [🇪🇸 Español](README_es.md) · [🇨🇳 中文](README_zh.md)

<img src="assets/screenshots/dashboard.jpeg" width="120" style="border-radius:24px" />

# FORGE

### The Social Workout Tracker

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/maui/)
[![PocketBase](https://img.shields.io/badge/PocketBase-Backend-000000?logo=pocketbase)](https://pocketbase.io/)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1.500%2B%20exercises-22C55E)](https://oss.exercisedb.dev)
[![MVVM](https://img.shields.io/badge/Architecture-MVVM%20%7C%20CommunityToolkit-7C3AED)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Tests](https://img.shields.io/badge/Tests-27%20passed-brightgreen)](tests/)
[![License](https://img.shields.io/badge/License-MIT-blue)](LICENSE)

**Turn every rep into progress. Challenge your friends. Push your limits.**

</div>

---

## What is FORGE

FORGE is an Android app for tracking gym workouts with a social twist. Log exercises, sets, and reps. Follow your friends. Unlock achievements. Watch your progress with detailed statistics.

| Problem | FORGE Solution |
|---------|---------------|
| You don't remember last session's weights | Full workout history with set-by-set detail |
| You don't know which exercises target a muscle | 1,500+ exercises catalog with GIFs, search, and filters |
| You train alone and lose motivation | Friends feed, likes, weekly streaks, achievements |

---

## Features

| Category | Feature |
|----------|---------|
| 🏋️ **Workout** | Exercise search (1,500+ with GIFs), sets kg×reps, completion checkmark, rest timer, progress photos, minimize/draft |
| 📊 **Stats** | Volume chart, top lifts, monthly calendar, WEEK/MONTH/3M/YEAR/ALL filters |
| 👥 **Social** | Friends feed, like ♥, follow/unfollow, live user search, friend requests |
| 🏆 **Achievements** | 48 badges to unlock, automatic tracking, profile showcase |
| 👤 **Profile** | Avatar, bio, stats, workout history, unlocked badges |
| 🎨 **UI** | Dual theme light/dark, Inter/Lexend/Space Grotesk fonts, Stitch design |
| 📱 **Offline** | Local SQLite with automatic sync when connection returns |
| 📁 **CSV** | Import/export workouts for backup |
| 🔒 **Security** | SecureStorage for passwords, HTTPS, row-level API rules, admin panel blocked |

---

## Screenshots

<div align="center">

| Dashboard | Feed | Stats |
|:---:|:---:|:---:|
| <img src="assets/screenshots/dashboard.jpeg" width="200" /> | <img src="assets/screenshots/feed.jpeg" width="200" /> | <img src="assets/screenshots/stats.jpeg" width="200" /> |

| Active Workout | Profile | Achievements |
|:---:|:---:|:---:|
| <img src="assets/screenshots/active-workout1.jpeg" width="200" /> | <img src="assets/screenshots/profile.jpeg" width="200" /> | <img src="assets/screenshots/login.jpeg" width="200" /> |

</div>

---

## Architecture

```
┌──────────────────────────────────────────────────┐
│                  .NET MAUI App                    │
│  ┌──────────┐  ┌───────────┐  ┌──────────┐      │
│  │  Views   │  │ ViewModels│  │ Services │      │
│  │  XAML    │◄─┤ MVVM      │◄─┤ Business │      │
│  │  pure    │  │ Toolkit   │  │ Logic    │      │
│  └──────────┘  └───────────┘  └────┬─────┘      │
│                                    │             │
│         ┌──────────────────────────┼──────┐      │
│    ┌────▼────┐   ┌──────────┐  ┌──▼───┐ │      │
│    │PocketBase│  │ExerciseDB│  │SQLite│ │      │
│    │ Auth +   │  │ v1 API   │  │Local │ │      │
│    │ Social   │  │ 1,500+ex │  │Cache │ │      │
│    └─────────┘  └──────────┘  └──────┘ │      │
└─────────────────────────────────────────┘      │
```

### Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET MAUI 10 (Android-first) |
| UI Pattern | MVVM with CommunityToolkit.Mvvm 8.4 |
| Navigation | Shell (3 tabs + 8 routes) |
| Backend | PocketBase self-hosted (auth, social, storage) |
| API | ExerciseDB v1 (1,500+ exercises, free) |
| Persistence | SQLite (sqlite-net-pcl) |
| Tests | xUnit (27 tests) |
| Fonts | Inter, Lexend, Space Grotesk (Google Fonts) |

---

## Development

### Prerequisites
- .NET 10 SDK + MAUI workload
- Android device or emulator (Android 7+)
- PocketBase server (configurable via `.env`)

### Quick Start

```bash
# 1. Clone
git clone https://github.com/USERNAME/FORGE.git
cd FORGE

# 2. Configure
cp .env.example .env
# Edit .env with your PocketBase server URL

# 3. Build
dotnet build src/Forge/Forge.csproj -f net10.0-android

# 4. Test
dotnet test tests/Forge.Tests/

# 5. Publish APK
dotnet publish src/Forge/Forge.csproj -f net10.0-android -c Release /p:AndroidPackageFormats=apk
```

### Structure

```text
├── src/Forge/               # MAUI Project
│   ├── Models/              # Domain entities + DTOs
│   ├── ViewModels/          # MVVM ViewModels (12)
│   ├── Views/               # XAML Views (10)
│   ├── Services/            # Business logic (11)
│   ├── Converters/          # Value converters
│   └── Resources/           # Styles, fonts, images
├── tests/Forge.Tests/       # xUnit Tests (27)
├── tools/
│   └── ExerciseImporter/    # Exercise import to PocketBase
├── docs/                    # Documentation
└── FORGE.apk                # Release APK
```

### Branches

| Branch | Purpose |
|--------|---------|
| `main` | Stable version, distribution-ready |
| `develop` | Active development, new features and fixes |
| `feature/*` | Feature branches (merge into develop) |

---

## Documentation

| Document | Content |
|----------|---------|
| [`docs/spec.md`](docs/spec.md) | Product specification, epics, user stories, acceptance criteria |
| [`docs/plan.md`](docs/plan.md) | Iteration plan with status |
| [`docs/architecture.md`](docs/architecture.md) | Technical architecture |
| [`docs/project-journal.md`](docs/project-journal.md) | Complete development journal |
| [`docs/api-notes.md`](docs/api-notes.md) | API technical notes |

---

## Privacy

Data is stored in two places:
- **On your phone**: local SQLite database (workouts, exercises, achievements, plans)
- **On the FORGE server**: authentication, completed workouts, likes, followers

See [`PRIVACY.md`](PRIVACY.md) for full details.

## Security

| Measure | Detail |
|---------|--------|
| Password | Encrypted with SecureStorage (Android Keystore) |
| Connection | HTTPS with Let's Encrypt |
| Remote DB | Row-level API rules (each user only sees their own data) |
| Admin panel | Blocked from external access |
| Rate limiting | 5 login attempts/minute |
| Android Backup | Disabled (`allowBackup=false`) |

## Disclaimer

**FORGE is an educational project** developed as part of a computer science study program. It is not a commercial product. The server is self-hosted on home hardware and may not always be available. The ExerciseDB API is a free third-party service with usage limitations.

## License

MIT License — see the [LICENSE](LICENSE) file for details.

---

## Attributions

- **ExerciseDB API**: Exercise data and GIFs provided by [ExerciseDB](https://github.com/yuhonas/free-exercise-db) (free, non-commercial use)
- **Badge icons**: created with Canva Pro
- **Fonts**: Inter, Lexend, Space Grotesk by Google Fonts (SIL Open Font License)

---

<div align="center">

**FORGE** — Build your physique. Challenge your friends. Forge your legend.

</div>
