import type { it } from "@/lib/translations/it"

export const en: typeof it = {
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
    note: "Requires Docker and Docker Compose. PocketBase exposes the admin dashboard on port 8090.",
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
        version: "v0.9.0-beta",
        date: "",
        theme: "QoL & Usability",
        features: [
          { name: "Quick weight increment (+/-)", desc: "+2.5kg buttons next to the weight field. Press-and-hold = +5kg", diff: "low" },
          { name: "Copy previous set", desc: 'A "Duplicate" button that clones kg×reps into the next set', diff: "low" },
          { name: "1RM calculator", desc: "Tap a top lift → shows the estimated max (Epley formula)", diff: "low" },
          { name: "Voice timer", desc: 'Android TTS: "Set complete, 90 seconds rest"', diff: "med" },
          { name: "Improved exercise search filter", desc: "Combinable filters (muscle AND equipment)", diff: "low" },
        ],
      },
      {
        version: "v1.0.0",
        date: "",
        theme: "Stable Release",
        features: [
          { name: "FCM push notifications", desc: "Notify when a friend follows you, likes, or accepts a request", diff: "high" },
          { name: "Full localization", desc: ".resx files for IT/EN/ES/DE/ZH — all strings translated", diff: "high" },
          { name: "Body tracking", desc: "Log body weight, measurements, %BF. Chart over time", diff: "med" },
          { name: "Weekly leaderboard", desc: "Volume ranking among friends, reset every Monday", diff: "med" },
          { name: "Auto-build APK in Releases", desc: "GitHub Action that creates an automatic release on each tag", diff: "low" },
        ],
      },
      {
        version: "v1.1.0",
        date: "",
        theme: "Social & Community",
        features: [
          { name: "Workout comments", desc: "Friends can comment on your workouts in the feed", diff: "med" },
          { name: "1-vs-1 challenges", desc: '"Challenge Marco on total volume this week" → notification + ranking', diff: "med" },
          { name: "Workout sharing", desc: "Android share sheet: a summary image of your workout", diff: "med" },
          { name: "Seasonal badges", desc: 'Time-limited achievements (e.g. "Train on Christmas")', diff: "low" },
          { name: "Workout stories", desc: "Photo of the day visible for 24h in the feed (Instagram Stories style)", diff: "high" },
        ],
      },
      {
        version: "v1.2.0",
        date: "",
        theme: "Health & Wear OS",
        features: [
          { name: "Android widget", desc: '"START WORKOUT" right from your phone home screen', diff: "med" },
          { name: "Wear OS companion", desc: "Rest timer + set tracking from your smartwatch", diff: "vhigh" },
          { name: "Google Fit integration", desc: "Sync workout duration + calories with Google Fit", diff: "med" },
          { name: "Heart rate display", desc: "Show heart rate during a workout (if supported)", diff: "high" },
          { name: "Step counter", desc: "Daily step widget in the dashboard", diff: "low" },
        ],
      },
      {
        version: "v1.3.0",
        date: "",
        theme: "Advanced Features",
        features: [
          { name: "PDF report", desc: "Export a workout report with charts, tables, and photos", diff: "med" },
          { name: "Preset workouts", desc: '"Push/Pull/Legs", "Full Body", "Upper/Lower" templates', diff: "low" },
          { name: "Progressive overload tracker", desc: "The app suggests how much to increase from the previous session", diff: "med" },
          { name: "Advanced charts", desc: "Volume per muscle over time, PR timeline, weekly frequency", diff: "med" },
          { name: "Edit workout after saving", desc: "Fix a wrong weight after you've saved", diff: "low" },
          { name: "Search workouts in history", desc: "Search by exercise name, date, volume", diff: "low" },
        ],
      },
      {
        version: "v1.4.0",
        date: "",
        theme: "Platform & Infrastructure",
        features: [
          { name: "Google Play Store", desc: "Official publication with a production keystore", diff: "med" },
          { name: "Automatic backup", desc: "Weekly automatic data export to Google Drive", diff: "med" },
          { name: "SQLite encryption", desc: "Database encrypted with SQLCipher", diff: "med" },
          { name: "Multi-server switch", desc: "Multiple profiles: switch between different self-hosted servers", diff: "low" },
          { name: "Web dashboard", desc: "Web interface to view your data from a PC", diff: "high" },
          { name: "Public REST API", desc: "Documented API for third-party integrations", diff: "high" },
        ],
      },
    ],
    phasesEyebrow: "// Summary",
    phasesTitle: "The three pillars ahead",
    phases: [
      {
        tag: "Phase 1",
        title: "Smart Workout Flow",
        body: "Automatic rest timer with vibration, last-session weights already shown as placeholders (Ghost Inputs), and quick +/- buttons to change loads by 2.5kg without a keyboard.",
      },
      {
        tag: "Phase 2",
        title: "Advanced Analytics",
        body: "Automatic estimated max (1RM with the Epley formula), interactive charts for volume distribution across muscle groups, and a streak calendar for consistency.",
      },
      {
        tag: "Phase 3",
        title: "Self-Hosting Pro & Social",
        body: "Real-time friends feed on the same PocketBase server, and an Offline First mode to save at the gym even without signal and sync once you're back home.",
      },
    ],
    b2b: {
      eyebrow: "// FORGE Local Business",
      title: "A B2B module for gyms",
      body: "The self-hosted architecture lets a gym owner launch the backend on a local mini-PC and create a private management and social system for the whole facility.",
      advantages: [
        { title: "Zero SaaS costs", desc: "No subscriptions or per-seat licenses. From 50 to 5,000 members, software cost is €0/month." },
        { title: "Local community", desc: "People training in the same room see each other's PRs in real time, with the gym's internal rankings." },
        { title: "100% privacy and GDPR", desc: "Health and performance data never leaves the building. A real marketing point." },
        { title: "Offline / LAN First", desc: "If the internet drops, the app keeps working over local Wi-Fi. Zero downtime during workouts." },
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
