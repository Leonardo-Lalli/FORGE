import type { Dictionary } from "./types"

export const en: Dictionary = {
  nav: {
    features: "Features",
    selfHosting: "Self-Hosting",
    docs: "Docs",
    roadmap: "Roadmap",
    download: "Download",
    backHome: "Back to home",
  },
  controls: {
    toggleThemeToLight: "Switch to light theme",
    toggleThemeToDark: "Switch to dark theme",
    language: "Language",
    switchToEnglish: "Switch to English",
    switchToItalian: "Switch to Italian",
  },
  hero: {
    badge: "Open-source · Privacy-first · Self-hostable",
    title: "Forge your body.\nTrack every gain.",
    subtitle:
      "FORGE is the open-source gym app that puts your data first. Track volume and loads, manage rest times, and host everything on your own server.",
    ctaPrimary: "Download the Android APK",
    ctaSecondary: "View on GitHub",
    note: "Android APK · No cloud account required · Free forever",
  },
  features: {
    eyebrow: "// Features",
    title: "Everything you need to train better.",
    subtitle:
      "No fluff, no paywalls. Just the essential tools to log your workouts and improve over time.",
    items: [
      {
        title: "Volume tracking",
        desc: "Log sets, reps, and loads. FORGE automatically computes your total volume and top lifts for every exercise.",
      },
      {
        title: "Total privacy",
        desc: "Your data stays yours. Self-host with PocketBase: no third-party cloud, no trackers, no ads.",
      },
      {
        title: "Rest timer",
        desc: "Configurable rest timer between sets, with a notification when it ends, so you keep the right pace without distractions.",
      },
    ],
  },
  selfHosting: {
    eyebrow: "// Self-Hosting",
    title: "Your server, your rules.",
    subtitle:
      "FORGE uses PocketBase as its backend: a single binary with database, authentication, and file storage. Spin it up with Docker Compose in seconds.",
    steps: [
      "Full backend in a single container",
      "Authentication and database included",
      "Updates with a single command",
    ],
    terminalTitle: "terminal",
    terminalComment: "# Start the FORGE backend with PocketBase",
    note: "Requires Docker and Docker Compose. Install FORGE migrations and hooks for privacy and the shared exercise cache: the server stores exercise records, instructions and GIF URLs. GIF files remain on the provider and require connectivity. Keep administration private.",
    cta: "Self-hosting guide",
  },
  docsTeaser: {
    eyebrow: "// Documentation",
    title: "The whole project, in the open.",
    subtitle:
      "FORGE's spec, architecture, roadmap, and development journal — the same documentation that drives the open-source project.",
    cards: [
      { title: "Spec", desc: "Vision, target users, and the MVP's functional requirements." },
      { title: "Architecture", desc: ".NET MAUI stack, repo structure, and PocketBase collections." },
      { title: "Roadmap", desc: "Completed iterations and upcoming features." },
      { title: "Journal", desc: "Development phases and the problems solved along the way." },
    ],
    cta: "Open the documentation",
  },
  footer: {
    tagline: "Build your body. Challenge your friends. Forge your legend.",
    product: "Product",
    resources: "Resources",
    links: {
      features: "Features",
      selfHosting: "Self-Hosting",
      download: "Download APK",
      docs: "Documentation",
      roadmap: "Roadmap",
      github: "GitHub",
    },
    rights: "Open-source project released under the MIT license.",
  },
  cta: {
    title: "Ready to start training with FORGE?",
    subtitle: "Download the APK, spin up your server, and take control of your workout data.",
    primary: "Download the Android APK",
    secondary: "Read the documentation",
  },
  docs: {
    eyebrow: "// Documentation",
    title: "The whole project, in the open.",
    subtitle:
      "FORGE's spec, architecture, roadmap, and development journal — the same documentation that drives the open-source project.",
    tabs: {
      spec: "Spec",
      architettura: "Architecture",
      roadmap: "Roadmap",
      diario: "Journal",
    },
    spec: {
      visionEyebrow: "// Vision",
      visionTitle: "Tracking + social in a single app",
      visionBody:
        "FORGE tackles the three hurdles every gym-goer faces: remembering loads, knowing which exercises to do, and staying motivated. An Android-first .NET MAUI app that combines an exercise catalog, workout logging, stats, and a social layer with a feed and likes.",
      targetEyebrow: "// Target users",
      targetUsers: [
        "Beginners starting to train and looking for structure",
        "Fitness enthusiasts who want to track progress over time",
        "Groups of friends who want to motivate each other with healthy competition",
      ],
      mvpEyebrow: "// MVP scope",
      mvpTitle: "Implemented features",
      mvpFeatures: [
        "PocketBase authentication (email/password/username) with auto-login",
        "ExerciseDB catalog with search and filters by muscle and equipment",
        "Workout logging: sets, reps, weight, and a configurable rest timer",
        "Dashboard with weekly streak, squad activity, and today card",
        "Social feed: follow, like, and requests with notifications",
        "Stats: volume chart, top lifts, calendar, and time filters",
        "Profile with avatar, bio, and workout history",
        "Savable workout plans (Quick Start or from a plan)",
        "Dual light/dark theme with a runtime toggle",
      ],
      reqEyebrow: "// Functional requirements",
      requirements: [
        { id: "FR-01", text: "Fetch exercises from the ExerciseDB API with list and search", done: true },
        { id: "FR-02", text: "Cache exercises in PocketBase with resolved image URLs", done: true },
        { id: "FR-05", text: "Save a complete workout to PocketBase", done: true },
        { id: "FR-08", text: "Progress stats: volume, top lifts, calendar", done: true },
        { id: "FR-13", text: "Friends' workout feed with likes", done: true },
        { id: "FR-15", text: "Weekly streak in Dashboard and Profile", done: true },
        { id: "FR-07", text: "Body weight and measurements tracking", done: false },
        { id: "FR-14", text: "Weekly leaderboard", done: false },
      ],
    },
    arch: {
      stackEyebrow: "// Tech stack",
      stack: [
        { tech: ".NET MAUI 10", role: "Android-first cross-platform framework" },
        { tech: "CommunityToolkit.Mvvm 8.4", role: "MVVM: ObservableProperty, RelayCommand, Messenger" },
        { tech: "Shell", role: "Navigation: 3 tabs + 8 detail routes" },
        { tech: "PocketBase", role: "Auth, remote database, file storage, social graph" },
        { tech: "ExerciseDB v1", role: "1,500+ exercise catalog with GIFs, free" },
        { tech: "SQLite", role: "Offline cache: workouts, exercises, plans, achievements" },
        { tech: "SecureStorage", role: "Passwords and sensitive data (Android Keystore)" },
        { tech: "Nginx Proxy Manager", role: "HTTPS reverse proxy, Let's Encrypt, rate limit" },
      ],
      repoEyebrow: "// Repository structure",
      repoTree: `src/Forge/
├── App.xaml(.cs)            # Entry point + auto-login
├── AppShell.xaml(.cs)       # TabBar + detail routes
├── MauiProgram.cs           # DI composition root
├── Models/                  # Domain entities and DTOs
├── Services/                # PocketBase, ExerciseDB, Theme, Sync...
├── ViewModels/              # MVVM (BaseViewModel + 11 VMs)
├── Views/                   # Pure XAML pages
├── Converters/
└── Resources/               # Styles, Fonts, Raw/forge.env`,
      navEyebrow: "// Shell navigation",
      navTabs: ["Dashboard", "Feed", "Stats"],
      navRoutes: ["startSession", "activeWorkout", "workoutDetail", "achievements", "profile", "settings"],
      collEyebrow: "// PocketBase collections",
      collHeadName: "Collection",
      collHeadFields: "Main fields",
      collections: [
        { name: "users", fields: "email, password, name, bio, avatar" },
        { name: "logged_workouts", fields: "user, name, date, exercises, volume, duration, likes, liked_by" },
        { name: "social_graph", fields: "from_user, from_name, to_user, status" },
        { name: "excercise", fields: "name, bodyPart, equipment, instructions, imageUrl, level" },
      ],
    },
    roadmap: {
      iterations: [
        { id: "IT-01", goal: "Bootstrap MAUI + 3-tab Shell + Design System + ThemeService", status: "done" },
        { id: "IT-02/03/08", goal: "Exercise catalog + Workout logging + Savable plans", status: "done" },
        { id: "IT-05/06/07", goal: "PocketBase Auth + Social + Dashboard + Stats + Profile", status: "done" },
        { id: "IT-FEAT-01", goal: "Offline SQLite (DatabaseService, auto sync)", status: "done" },
        { id: "IT-FEAT-03", goal: "ExerciseDB v1 free API (no API key, 1,500 exercises)", status: "done" },
        { id: "IT-FEAT-06", goal: "Achievement system: 48 badges across 6 categories", status: "done" },
        { id: "IT-SEC-01/02/03", goal: "Security audit, hardening, and certificate pinning", status: "done" },
        { id: "Push Notifications", goal: "FCM hook ready, SDK not compatible with .NET 10", status: "partial" },
        { id: "IT-04", goal: "Body weight and measurements tracking", status: "skip" },
      ],
      status: { done: "Completed", partial: "Partial", skip: "Deferred" },
    },
    journal: [
      {
        phase: "IT-01",
        title: "MAUI Bootstrap and Design System",
        body: ".NET MAUI project with a 3-tab Shell, runtime light/dark themes, and Google fonts. Key decision: 3 tabs instead of 5 for a cleaner UI.",
      },
      {
        phase: "IT-02/03/08",
        title: "Catalog, Workout, and Plans",
        body: "ExerciseDB integration, full workout flow with a rest timer, and savable plans. Fixed short URLs blocked by ISPs and crashes from nested CollectionViews.",
      },
      {
        phase: "IT-05/06/07",
        title: "Auth, Social, Dashboard, Stats, and Profile",
        body: "Self-hosted PocketBase instead of Firebase, a feed with likes, stats, and profile. Streak computed weekly with a 7-day tolerance.",
      },
      {
        phase: "IT-FEAT-01 → 07",
        title: "Offline, ExerciseDB v1, CSV, Photos, Achievements",
        body: "SQLite + automatic sync, migration to the free API (1,500 exercises), CSV import/export, progress photos, and a system of 48 achievements.",
      },
      {
        phase: "IT-SEC-01 → 03",
        title: "Security and Hardening",
        body: "Passwords in SecureStorage, IHttpClientFactory, tokens out of URLs, Let's Encrypt certificate pinning, and API Rules with row-level ownership.",
      },
    ],
  },
  roadmapPage: {
    eyebrow: "// Roadmap",
    title: "Where FORGE is heading.",
    subtitle:
      "The next releases, phase by phase. A public timeline of the features we want to build — from usability improvements to the B2B module for gyms.",
    legend: { difficulty: "Difficulty" },
    difficulty: { low: "Low", med: "Medium", high: "High", vhigh: "Very high" },
    releases: [
      {
            "version": "v0.9.0-beta",
            "date": "",
            "theme": "Stability & Privacy",
            "features": [
                  {
                        "name": "Reliable access",
                        "desc": "Public and personal servers; verified avatars and session recovery",
                        "diff": "med"
                  },
                  {
                        "name": "Server privacy",
                        "desc": "Public, private or mutual-friend workouts; tested API rules",
                        "diff": "high"
                  },
                  {
                        "name": "Account-owned plans",
                        "desc": "Unlimited personal plans; examples once; persistent notes and rest",
                        "diff": "med"
                  },
                  {
                        "name": "Retroactive achievements",
                        "desc": "History recalculation and three selected profile badges",
                        "diff": "med"
                  },
                  {
                        "name": "Regression checks",
                        "desc": "Correct volume chart, Android builds and automated tests",
                        "diff": "med"
                  }
            ]
      },
      {
            "version": "v1.0.0",
            "date": "",
            "theme": "Verified workout flow",
            "features": [
                  {
                        "name": "Per-exercise rest",
                        "desc": "0–5 minute selection stored in plans",
                        "diff": "med"
                  },
                  {
                        "name": "Previous performance",
                        "desc": "Editable weight and rep suggestions from history",
                        "diff": "med"
                  },
                  {
                        "name": "Offline sync",
                        "desc": "Device validation of saves, drafts and account changes",
                        "diff": "high"
                  },
                  {
                        "name": "Exercise catalog",
                        "desc": "Real counts, GIFs, instructions and complete pagination",
                        "diff": "med"
                  },
                  {
                        "name": "Android release",
                        "desc": "Reproducible APK build with release validation",
                        "diff": "med"
                  }
            ]
      },
      {
            "version": "v1.1.0",
            "date": "",
            "theme": "Social & Community",
            "features": [
                  {
                        "name": "Profiles and follows",
                        "desc": "Searchable profiles and follower lists; friends follow each other",
                        "diff": "med"
                  },
                  {
                        "name": "Comments and notifications",
                        "desc": "Comments and push after privacy is stable",
                        "diff": "high"
                  },
                  {
                        "name": "Challenges and leaderboards",
                        "desc": "Friend challenges with consent",
                        "diff": "med"
                  },
                  {
                        "name": "Sharing",
                        "desc": "Share summaries without exposing private content",
                        "diff": "med"
                  }
            ]
      },
      {
            "version": "v1.2.0",
            "date": "",
            "theme": "Analytics & Localization",
            "features": [
                  {
                        "name": "Advanced analytics",
                        "desc": "1RM, muscle volume, calendar and history",
                        "diff": "med"
                  },
                  {
                        "name": "Body tracking",
                        "desc": "Measurements with privacy controls",
                        "diff": "high"
                  },
                  {
                        "name": "Languages and accessibility",
                        "desc": "Full translations and contrast/font size checks",
                        "diff": "high"
                  },
                  {
                        "name": "Reports and backup",
                        "desc": "PDF export and tested backup restoration",
                        "diff": "med"
                  }
            ]
      },
      {
            "version": "v1.3.0",
            "date": "",
            "theme": "Health & Wearables",
            "features": [
                  {
                        "name": "Widgets and Wear OS",
                        "desc": "Quick access and workout companion",
                        "diff": "vhigh"
                  },
                  {
                        "name": "Health integration",
                        "desc": "Evaluate supported APIs with explicit permission",
                        "diff": "high"
                  },
                  {
                        "name": "Sensors",
                        "desc": "Heart rate and steps on supported devices",
                        "diff": "high"
                  }
            ]
      },
      {
            "version": "v1.4.0",
            "date": "",
            "theme": "Infrastructure & B2B",
            "features": [
                  {
                        "name": "Protected origin",
                        "desc": "Dedicated-domain proxy or tunnel; home IP out of public DNS",
                        "diff": "high"
                  },
                  {
                        "name": "Distribution",
                        "desc": "Play Store and production signing keys",
                        "diff": "med"
                  },
                  {
                        "name": "Dashboard and API",
                        "desc": "Web dashboard, documented API and server profiles",
                        "diff": "high"
                  },
                  {
                        "name": "Gyms",
                        "desc": "Trainer dashboard, QR onboarding and tenant isolation",
                        "diff": "vhigh"
                  }
            ]
      }
],
    phasesEyebrow: "// Summary",
    phasesTitle: "The three pillars ahead",
    phases: [
      {
            "tag": "Phase 1",
            "title": "Stability & Privacy",
            "body": "Public and personal servers; verified avatars and session recovery; Public, private or mutual-friend workouts; tested API rules; Unlimited personal plans; examples once; persistent notes and rest"
      },
      {
            "tag": "Phase 2",
            "title": "Verified workout flow",
            "body": "0–5 minute selection stored in plans; Editable weight and rep suggestions from history; Device validation of saves, drafts and account changes"
      },
      {
            "tag": "Phase 3",
            "title": "Social & Community",
            "body": "Searchable profiles and follower lists; friends follow each other; Comments and push after privacy is stable; Friend challenges with consent"
      }
],
    b2b: {
      eyebrow: "// FORGE Local Business",
      title: "A B2B module for gyms",
      body: "The self-hosted architecture lets a gym owner launch the backend on a local mini-PC and create a private management and social system for the whole facility.",
      advantages: [
        { title: "Zero SaaS costs", desc: "No subscriptions or per-seat licenses. From 50 to 5,000 members, software cost is €0/month." },
        { title: "Local community", desc: "People training in the same room see each other's PRs in real time, with the gym's internal rankings." },
        { title: "Local data control", desc: "Local hosting helps control data. Privacy and compliance also depend on access, consent, backups and server operations." },
        { title: "Offline / LAN First", desc: "If the internet drops, the app keeps working over local Wi-Fi. Local access depends on the gym network and server." },
      ],
      future: [
        { title: "Trainer Dashboard", desc: "A web interface where personal trainers assign plans and protocols to members." },
        { title: "Local TV Leaderboard", desc: "Project the ranking of PRs lifted in real time onto a gym screen." },
        { title: "QR Code Onboarding", desc: "Scan the QR at the gym and the app configures itself with the local server." },
        { title: "Multi-tenant", desc: "Multiple gyms on the same server with full data isolation." },
      ],
      futureTitle: "Future B2B developments",
    },
    backlog: {
      eyebrow: "// Backlog",
      title: "Ideas & experiments",
      items: [
        { name: "AI form check", desc: "Posture analysis via camera during an exercise" },
        { name: "Voice input", desc: '"A hundred kilos, eight reps" instead of typing' },
        { name: "Social login", desc: "Google/Apple sign-in alongside email/password" },
        { name: "Gym maps", desc: "Map of nearby gyms with community reviews" },
        { name: "Plan marketplace", desc: "Users share and sell workout plans" },
        { name: "Nutrition tracker", desc: "Calories and macros alongside workout tracking" },
        { name: "Apple Watch", desc: "iOS support + watchOS companion" },
        { name: "Gamification 2.0", desc: "Levels, XP, competitive seasons" },
        { name: "AI coaching", desc: "Automatic suggestions based on historical data" },
        { name: "Exercise videos", desc: "Embedded YouTube videos alongside the GIFs" },
      ],
    },
    closing: "Build your body. Challenge your friends. Forge your legend.",
  },
}
