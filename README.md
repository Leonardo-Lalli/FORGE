https://github.com/user-attachments/assets/28498cc5-b1e4-4de4-bce0-1627f3fb002d

<div align="center">

> [Visit the official website](https://leonardo-lalli.github.io/FORGE/) — multilingual, full docs, direct download

> **HOBBY PROJECT FOR EDUCATIONAL PURPOSES**
> This app is developed by a student as a school project.
> **It is not a commercial product.** The server is self-hosted on home hardware.
> There may be bugs, limitations, and unplanned downtime.
> Use for testing and demos, not for critical data.

<img src="assets/screenshots/dashboard.jpeg" width="120" style="border-radius:24px" />

[English](README.md) · [Italiano](README.md?lang=it) · [Español](README_es.md) · [中文](README_zh.md)

# FORGE

### The Social Workout Tracker

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/maui/)
[![PocketBase](https://img.shields.io/badge/PocketBase-Backend-000000?logo=pocketbase)](https://pocketbase.io/)
[![DuckDNS](https://img.shields.io/badge/DuckDNS-DDNS-FF5722?logo=duckdns)](https://duckdns.org)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1.500%2B%20exercises-22C55E)](https://oss.exercisedb.dev)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](#self-hosting)
[![MVVM](https://img.shields.io/badge/Architecture-MVVM%20%7C%20CommunityToolkit-7C3AED)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Tests](https://img.shields.io/badge/Tests-27%20passed-brightgreen)](tests/)
[![License](https://img.shields.io/badge/License-MIT-blue)](LICENSE)

[![GitHub stars](https://img.shields.io/github/stars/Leonardo-Lalli/FORGE?style=social&label=Star)](https://github.com/Leonardo-Lalli/FORGE/stargazers)
[![GitHub downloads](https://img.shields.io/github/downloads/Leonardo-Lalli/FORGE/total?label=Download)](https://github.com/Leonardo-Lalli/FORGE/releases)
[![Last commit](https://img.shields.io/github/last-commit/Leonardo-Lalli/FORGE?label=Updated)](https://github.com/Leonardo-Lalli/FORGE/commits)
[![GitHub release](https://img.shields.io/github/v/release/Leonardo-Lalli/FORGE?label=Release)](https://github.com/Leonardo-Lalli/FORGE/releases/latest)

**Turn every rep into progress. Challenge your friends. Push your limits.**

### Quick Links
[APK Download](#download-apk) · [Self-Hosting](#self-hosting) · [Screenshots](#screenshots) · [Project Status](#project-status) · [Security](#security)

---

[Watch the demo video](assets/Videos/hailuo-2_3_sculptural_clay_art_Create_a_premium_mobile_app_splash_screen_for_a_fitness_and_-0.mp4)

</div>

---

## What is FORGE

FORGE is an Android app for tracking gym workouts, with a social twist. Log exercises, sets, and reps. Follow your friends. Unlock achievements. Watch your progress with detailed statistics.

| Problem | FORGE Solution |
|---------|---------------|
| You don't remember your weights from last time | Full workout history with set-by-set detail |
| You don't know which exercises target a muscle | 1,500+ exercise catalog with GIFs, search and filters |
| You train alone and lose motivation | Friends feed, likes, weekly streaks, achievements |

### Why FORGE vs Strong / Hevy / FitNotes?

| Feature | FORGE | Strong | Hevy | FitNotes |
|---------|:-----:|:------:|:----:|:--------:|
| Price | **Free** | ~80€/yr | ~40€/yr | Free (no social) |
| Open source | ✅ | ❌ | ❌ | ❌ |
| Self-hosting backend | ✅ | ❌ | ❌ | ❌ |
| 1,500+ exercises with GIFs | ✅ | ~1,000 | ~400 | ~100 |
| Social feed (like, follow) | ✅ | ❌ | ✅ | ❌ |
| Achievements / gamification | **48 badges** | ❌ | ❌ | ❌ |
| Progress photos | ✅ | ❌ | ✅ | ❌ |
| Offline + sync | ✅ | ❌ | Premium | ✅ (local) |
| CSV import/export | ✅ | Premium | Premium | ✅ |
| Privacy-first | ✅ | ❌ | ❌ | ✅ |

---

## Project Status

| Status | Category | Detail |
|:------:|----------|--------|
| ✅ | Workout | 1,500+ exercise search with GIFs, sets kg×reps, checkmark, rest timer, minimize/draft |
| ✅ | Statistics | Volume chart, top lifts, calendar, WEEK/MONTH/3M/YEAR/ALL filters |
| ✅ | Social | Friends feed, like ♥, follow/unfollow, live user search, friend requests |
| ✅ | Achievements | 48 badges with automatic tracking, profile showcase |
| ✅ | Profile | Avatar upload, bio, workout history, unlocked badges |
| ✅ | Theme | Dual light/dark theme, Inter/Lexend/Space Grotesk fonts |
| ✅ | Offline | Local SQLite + auto sync when connection returns |
| ✅ | CSV | Import/export workouts with validation |
| ✅ | Security | Certificate pinning, HTTPS, SecureStorage, API rules row-level, rate limiting, blocked admin panel |
| 🟡 | Photos | Supported (max 3MB, base64 in record). No server-side compression |
| 🟡 | Connectivity | App requires network for login and new exercise search. Local cache for seen exercises |
| 🔴 | Push notifications | Not yet implemented (Firebase SDK doesn't support .NET 10) |
| 🔴 | Body tracking | Weight and measurements not yet implemented |
| 🔴 | Leaderboard | Ranking among friends not implemented (replaced by feed + likes) |
| 🚧 | Certificate pinning | Implemented client-side; needs real server testing |

---

## Screenshots

<div align="center">

| Dashboard | Feed | Stats |
|:---:|:---:|:---:|
| <img src="assets/screenshots/dashboard.jpeg" width="200" /> | <img src="assets/screenshots/feed.jpeg" width="200" /> | <img src="assets/screenshots/stats.jpeg" width="200" /> |

| Active Workout | Profile | Login |
|:---:|:---:|:---:|
| <img src="assets/screenshots/active-workout1.jpeg" width="200" /> | <img src="assets/screenshots/profile.jpeg" width="200" /> | <img src="assets/screenshots/login.jpeg" width="200" /> |

</div>

---

## Self-Hosting

FORGE is privacy-first: run the backend **on your own server**. Your data stays in your home.

### One-liner (pick your OS)

```bash
# Linux / macOS / Git Bash (Docker)
bash <(curl -sSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/community-install.sh)
```

```powershell
# Windows PowerShell (Docker)
Invoke-Expression (Invoke-WebRequest -Uri https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/setup.ps1).Content
```

```bash
# Proxmox VE — run INSIDE an existing LXC container (Debian/Alpine)
# Installs PocketBase natively as a systemd service, no Docker
bash <(curl -sSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/proxmox-install.sh)
```

```bash
# Proxmox VE — run on the HYPERVISOR (creates a new LXC container from scratch)
# Container specs: 512 MB RAM | 4 GB disk | 2 CPU cores
# OS footprint: Alpine ~50 MB | Debian ~100 MB | Ubuntu ~150 MB (+25 MB for PocketBase)
bash <(curl -sSL https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/proxmox-create.sh)
```

> A single command. Downloads, installs, configures admin and collections, shows the IP.
> Linux/Windows scripts use **Docker**. Proxmox scripts install PocketBase **natively**.

**Resource usage:** PocketBase uses ~25 MB RAM idle and ~30 MB disk.
With 100 active users it reaches ~80 MB RAM. Runs on Raspberry Pi 4, old PC, 1 GB VPS.

### Step by step (all OS)

```bash
git clone https://github.com/Leonardo-Lalli/FORGE.git && cd FORGE
docker compose up -d pocketbase
docker compose exec -T pocketbase pocketbase superuser create admin@forge.local forgeadmin123
docker compose up -d
docker compose logs init
docker compose logs show-ip
```

PocketBase comes pre-configured with:
- **Superuser** pre-created (`admin@forge.local` / `forgeadmin123` — change immediately!)
- **Collections** `logged_workouts`, `social_graph`, `excercise` with row-level API rules

### Configure the app

1. Install the APK on your phone
2. Open FORGE → Settings → **PocketBase URL**
3. Enter `http://<YOUR-PC-IP>:8090` (e.g. `http://192.168.1.50:8090`)
4. Tap **SAVE**

The app will use your server instead of the default one. Works on LAN without a public domain.

### Admin panel

`http://localhost:8090/_/` — login with admin@forge.local / forgeadmin123

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

## Download APK

The Release APK is available on [GitHub Releases](https://github.com/Leonardo-Lalli/FORGE/releases).

To build from source:
```bash
dotnet publish src/Forge/Forge.csproj -f net10.0-android -c Release /p:AndroidPackageFormats=apk
```

> Requires Android 7.0+ and internet connection for APIs.

---

## Development

### Prerequisites
- .NET 10 SDK + MAUI workload
- Android device or emulator (Android 7+)
- PocketBase server (configurable via `.env`)

### Quick start

```bash
git clone https://github.com/Leonardo-Lalli/FORGE.git
cd FORGE
cp .env.example .env
# Edit .env with your PocketBase server URL
dotnet build src/Forge/Forge.csproj -f net10.0-android
dotnet test tests/Forge.Tests/
dotnet publish src/Forge/Forge.csproj -f net10.0-android -c Release /p:AndroidPackageFormats=apk
```

### Structure

```text
├── src/Forge/               # MAUI Project
│   ├── Models/              # Domain entities + DTOs
│   ├── ViewModels/          # MVVM ViewModels (12)
│   ├── Views/               # XAML Views (10)
│   ├── Services/            # Business logic (13)
│   ├── Converters/          # Value converters
│   └── Resources/           # Styles, fonts, images
├── tests/Forge.Tests/       # xUnit Tests (27)
├── docker-compose.yml       # Self-hosted PocketBase backend
├── tools/                   # Installation & setup scripts
└── docs/                    # Documentation
```

### Branches

| Branch | Purpose |
|--------|---------|
| `main` | Stable, distribution-ready |
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
| [`docs/test-matrix.md`](docs/test-matrix.md) | 42 manual tests matrix |
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
| Connection | HTTPS with Let's Encrypt + certificate pinning |
| Remote DB | Row-level API rules (each user only sees their own data) |
| Admin panel | Blocked from external access (403) |
| Rate limiting | 5 login attempts/min, 60 req/min API |
| Android Backup | Disabled (`allowBackup=false`) |

## Disclaimer

**FORGE is an educational project** developed as part of a computer science study program. It is not a commercial product. The server is self-hosted on home hardware and may not always be available. The ExerciseDB API is a free third-party service with usage limitations.

## Community & Support

<div align="center">

**Like FORGE? Leave a star!**

[![Star FORGE](https://img.shields.io/github/stars/Leonardo-Lalli/FORGE?style=social)](https://github.com/Leonardo-Lalli/FORGE/stargazers)

[Star](https://github.com/Leonardo-Lalli/FORGE/stargazers) · [Bug report](https://github.com/Leonardo-Lalli/FORGE/issues/new?template=bug_report.md) · [Feature request](https://github.com/Leonardo-Lalli/FORGE/issues/new?template=feature_request.md) · [Discussions](https://github.com/Leonardo-Lalli/FORGE/discussions)

</div>

## License

MIT License — see [LICENSE](LICENSE).

---

## Attributions

- **ExerciseDB API**: Exercise data and GIFs by [ExerciseDB](https://github.com/yuhonas/free-exercise-db) (free, non-commercial use)
- **Badge icons**: created with Canva Pro
- **Fonts**: Inter, Lexend, Space Grotesk by Google Fonts (SIL Open Font License)

---

<div align="center">

**FORGE** — Build your physique. Challenge your friends. Forge your legend.

</div>
