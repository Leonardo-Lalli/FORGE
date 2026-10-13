import type { Dictionary } from "./types"

export const en: Dictionary = {
  "nav": {
    "features": "Features",
    "selfHosting": "Self-Hosting",
    "docs": "Docs",
    "roadmap": "Roadmap",
    "download": "Download",
    "backHome": "Back to home"
  },
  "controls": {
    "toggleThemeToLight": "Switch to light theme",
    "toggleThemeToDark": "Switch to dark theme",
    "language": "Language",
    "switchToEnglish": "Switch to English",
    "switchToItalian": "Switch to Italian"
  },
  "hero": {
    "badge": "0.8.5 beta · Android · Open source",
    "title": "Forge your body.\nTrack every gain.",
    "subtitle": "Log workouts, recover your previous loads and share progress with your community. Use the default FORGE server or your own PocketBase instance.",
    "ctaPrimary": "Download FORGE 0.8.5 beta",
    "ctaSecondary": "View on GitHub",
    "note": "0.8.5 beta · Account on your chosen server · No in-app subscription",
    "updateNote": "Older installations: the signing key changed, so reinstallation may be required. CSV is not a full backup: save plans and photos separately and verify synchronization before removing the app."
  },
  "features": {
    "eyebrow": "// Features",
    "title": "Everything you need to train better.",
    "subtitle": "Implemented workout tools, with beta limitations and remaining checks documented openly.",
    "items": [
      {
        "title": "Volume tracking",
        "desc": "Log sets, reps, and loads. FORGE automatically computes your total volume and top lifts for every exercise."
      },
      {
        "title": "Configurable privacy",
        "desc": "Public, private or mutual-friend workouts and connection lists. Rules are enforced by an updated FORGE backend."
      },
      {
        "title": "Rest timer",
        "desc": "Configurable rest timer between sets, with a notification when it ends, so you keep the right pace without distractions."
      },
      {
        "title": "Rotating exercise tips",
        "desc": "Above each exercise, available execution steps change automatically during the workout. Instructions and images depend on the catalog."
      }
    ]
  },
  "selfHosting": {
    "eyebrow": "// Self-Hosting",
    "title": "Your server, your rules.",
    "subtitle": "PocketBase handles accounts, workouts and social. Install an explicitly selected version and apply FORGE hooks and migrations.",
    "steps": [
      "Separate community on each server",
      "Your own administrator credentials",
      "Updates with backups and migrations"
    ],
    "terminalTitle": "terminal",
    "terminalComment": "# Start the FORGE backend with PocketBase",
    "note": "Requires Docker and Docker Compose. Install FORGE migrations and hooks for privacy and the shared exercise cache: the server stores exercise records, instructions and GIF URLs. GIF files remain on the provider and require connectivity. Keep administration private.",
    "cta": "Self-hosting guide"
  },
  "docsTeaser": {
    "eyebrow": "// Documentation",
    "title": "The whole project, in the open.",
    "subtitle": "FORGE's spec, architecture, roadmap, and development journal — the same documentation that drives the open-source project.",
    "cards": [
      {
        "title": "Spec",
        "desc": "Vision, target users, and the MVP's functional requirements."
      },
      {
        "title": "Architecture",
        "desc": ".NET MAUI stack, repo structure, and PocketBase collections."
      },
      {
        "title": "Roadmap",
        "desc": "Completed iterations and upcoming features."
      },
      {
        "title": "Journal",
        "desc": "Development phases and the problems solved along the way."
      }
    ],
    "cta": "Open the documentation"
  },
  "footer": {
    "tagline": "Build your body. Challenge your friends. Forge your legend.",
    "product": "Product",
    "resources": "Resources",
    "links": {
      "features": "Features",
      "selfHosting": "Self-Hosting",
      "download": "Download APK",
      "docs": "Documentation",
      "roadmap": "Roadmap",
      "github": "GitHub"
    },
    "rights": "Open-source project released under the MIT license."
  },
  "cta": {
    "title": "Ready to start training with FORGE?",
    "subtitle": "Download the beta, choose your community and report what we can improve.",
    "primary": "Download the Android APK",
    "secondary": "Read the documentation"
  },
  "docs": {
    "eyebrow": "// Documentation",
    "title": "The whole project, in the open.",
    "subtitle": "FORGE's spec, architecture, roadmap, and development journal — the same documentation that drives the open-source project.",
    "tabs": {
      "spec": "Spec",
      "architettura": "Architecture",
      "roadmap": "Roadmap",
      "diario": "Journal"
    },
    "spec": {
      "visionEyebrow": "// Vision",
      "visionTitle": "Tracking + social in a single app",
      "visionBody": "FORGE started as Leonardo Lalli’s computer science school project. It grew through gym testing, feedback and extensive AI assistance for code, interfaces, documentation and checks. AI-assisted work requires review; the owner remains responsible for decisions and releases. It is currently an experimental beta, not a professional service with guaranteed availability.",
      "targetEyebrow": "// Target users",
      "targetUsers": [
        "Beginners starting to train and looking for structure",
        "Fitness enthusiasts who want to track progress over time",
        "Groups of friends who want to motivate each other with healthy competition"
      ],
      "mvpEyebrow": "// MVP scope",
      "mvpTitle": "Implemented features",
      "mvpFeatures": [
        "Plans and sets — Personal plans without an app-imposed limit; normal, warm-up, drop and failure sets. Confirm structural plan changes when finishing a session.",
        "Automatic notes — Notes for existing plan exercises save without confirmation and return in the next session.",
        "Rest from 0 to 5 minutes — Per-exercise timer with pause, progress notification and sound. Sound and filters were confirmed on the owner’s Samsung A15; other devices still need verification.",
        "Stats and profiles — Volume, period calendar, per-exercise progression, avatars, workout photos and retroactive badges.",
        "Your community’s social feed — Search, follow, likes and feed within the same server. Share download invitations; no federation between servers."
      ],
      "reqEyebrow": "// Functional requirements",
      "requirements": [
        {
          "id": "BETA",
          "title": "Current status",
          "body": "Core flow tested by the owner; beta needs validation by other users."
        }
      ]
    },
    "arch": {
      "stackEyebrow": "// Tech stack",
      "stack": [
        {
          "tech": ".NET 10 MAUI",
          "role": "Android · MVVM · CommunityToolkit"
        },
        {
          "tech": "PocketBase",
          "role": "Accounts, community, privacy and storage"
        },
        {
          "tech": "SQLite",
          "role": "Plans, cache, drafts and local workouts"
        },
        {
          "tech": "ExerciseDB OSS",
          "role": "Metadata, instructions and GIF URLs"
        },
        {
          "tech": "Firebase FCM",
          "role": "Optional social push; separate worker"
        }
      ],
      "repoEyebrow": "// Repository structure",
      "repoTree": "src/Forge/\n├── App.xaml(.cs)            # Entry point + auto-login\n├── AppShell.xaml(.cs)       # TabBar + detail routes\n├── MauiProgram.cs           # DI composition root\n├── Models/                  # Domain entities and DTOs\n├── Services/                # PocketBase, ExerciseDB, Theme, Sync...\n├── ViewModels/              # MVVM (BaseViewModel + 11 VMs)\n├── Views/                   # Pure XAML pages\n├── Converters/\n└── Resources/               # Styles, Fonts, Raw/forge.env",
      "navEyebrow": "// Shell navigation",
      "navTabs": [
        "Dashboard",
        "Feed",
        "Stats"
      ],
      "navRoutes": [
        "startSession",
        "activeWorkout",
        "workoutDetail",
        "achievements",
        "profile",
        "settings"
      ],
      "collEyebrow": "// PocketBase collections",
      "collHeadName": "Collection",
      "collHeadFields": "Main fields",
      "collections": [
        {
          "name": "users",
          "fields": "email, password, name, bio, avatar"
        },
        {
          "name": "logged_workouts",
          "fields": "user, name, date, exercises, volume, duration, likes, liked_by"
        },
        {
          "name": "social_graph",
          "fields": "from_user, from_name, to_user, status"
        },
        {
          "name": "excercise",
          "fields": "name, bodyPart, equipment, instructions, imageUrl, level"
        }
      ]
    },
    "roadmap": {
      "iterations": [
        {
          "id": "BETA",
          "goal": "Stabilize workouts, notes, timers and saves.",
          "status": "partial"
        },
        {
          "id": "FEEDBACK",
          "goal": "Fix reported issues and improve the catalogue and backups.",
          "status": "partial"
        },
        {
          "id": "FUTURE",
          "goal": "Trainer/member roles, QR invitations and multiple gyms.",
          "status": "skip"
        }
      ],
      "status": {
        "done": "Completed",
        "partial": "Partial",
        "skip": "Deferred"
      }
    },
    "journal": [
      {
        "phase": "Origins",
        "title": "From school project to open beta",
        "body": "FORGE started as Leonardo Lalli’s computer science school project. It grew through gym testing, feedback and extensive AI assistance for code, interfaces, documentation and checks. AI-assisted work requires review; the owner remains responsible for decisions and releases. It is currently an experimental beta, not a professional service with guaranteed availability."
      },
      {
        "phase": "October 2026",
        "title": "Device feedback",
        "body": "Login, workouts, timer sound and filters tested by the owner. Android builds and targeted regressions are documented; they do not prove correctness on every device."
      }
    ]
  },
  "roadmapPage": {
    "eyebrow": "// Roadmap",
    "title": "Where FORGE is heading.",
    "subtitle": "All project goals, without future dates or versions. Priorities may change with feedback; dates and versions will appear only after release.",
    "plannedTitle": "Goals to develop and improve",
    "plannedNote": "Some goals extend existing features. Ideas need evaluation and are not delivery promises.",
    "groups": [
      {
        "theme": "Stability and privacy",
        "features": [
          {
            "name": "Reliable access and saves",
            "desc": "Improve login, avatars, drafts, recovery, per-account plans and synchronization when offline."
          },
          {
            "name": "Verified server privacy",
            "desc": "Verify public, private and mutual-friend workouts and connections, account deletion and data isolation."
          },
          {
            "name": "Transparent catalog",
            "desc": "Check duplicates, written instructions, reachable GIFs and shared caching with real counts."
          },
          {
            "name": "Complete backup and restore",
            "desc": "Preserve plans, notes, photos, sets and weights: the current CSV is not a complete backup."
          },
          {
            "name": "Protected origin",
            "desc": "Consider a proxy or tunnel with a dedicated domain to keep the home IP out of public DNS."
          }
        ]
      },
      {
        "theme": "QoL & Usability",
        "features": [
          {
            "name": "Quick weight increment (+/-)",
            "desc": "+2.5kg buttons next to the weight field. Press-and-hold = +5kg"
          },
          {
            "name": "Copy previous set",
            "desc": "A \"Duplicate\" button that clones kg×reps into the next set"
          },
          {
            "name": "1RM calculator",
            "desc": "Tap a top lift → shows the estimated max (Epley formula)"
          },
          {
            "name": "Voice timer",
            "desc": "Android TTS: \"Set complete, 90 seconds rest\""
          },
          {
            "name": "Improved exercise search filter",
            "desc": "Muscle and equipment filters exist: improve catalog coverage, pagination and result consistency."
          }
        ]
      },
      {
        "theme": "Stable Release",
        "features": [
          {
            "name": "FCM push notifications",
            "desc": "Client and worker exist: verify background delivery for follows, likes and requests."
          },
          {
            "name": "Full localization",
            "desc": ".resx files for IT/EN/ES/DE/ZH — all strings translated"
          },
          {
            "name": "Body tracking",
            "desc": "Log body weight, measurements, %BF. Chart over time"
          },
          {
            "name": "Weekly leaderboard",
            "desc": "Volume ranking among friends, reset every Monday"
          },
          {
            "name": "Auto-build APK in Releases",
            "desc": "A manual signed workflow exists: validate CI packaging and consider release automation."
          }
        ]
      },
      {
        "theme": "Social & Community",
        "features": [
          {
            "name": "Workout comments",
            "desc": "Friends can comment on your workouts in the feed"
          },
          {
            "name": "1-vs-1 challenges",
            "desc": "\"Challenge Marco on total volume this week\" → notification + ranking"
          },
          {
            "name": "Workout sharing",
            "desc": "Android share sheet: a summary image of your workout"
          },
          {
            "name": "Seasonal badges",
            "desc": "Time-limited achievements (e.g. \"Train on Christmas\")"
          },
          {
            "name": "Workout stories",
            "desc": "Extend public workouts and photos into temporary stories while respecting visibility settings."
          }
        ]
      },
      {
        "theme": "Health & Wear OS",
        "features": [
          {
            "name": "Android widget",
            "desc": "\"START WORKOUT\" right from your phone home screen"
          },
          {
            "name": "Wear OS companion",
            "desc": "Rest timer + set tracking from your smartwatch"
          },
          {
            "name": "Health integrations",
            "desc": "Evaluate supported health APIs for authorized workout and health data synchronization."
          },
          {
            "name": "Heart rate display",
            "desc": "Show heart rate during a workout (if supported)"
          },
          {
            "name": "Step counter",
            "desc": "Daily step widget in the dashboard"
          }
        ]
      },
      {
        "theme": "Advanced Features",
        "features": [
          {
            "name": "PDF report",
            "desc": "Export a workout report with charts, tables, and photos"
          },
          {
            "name": "Preset workouts",
            "desc": "Expand existing examples with Push/Pull/Legs and Upper/Lower templates; deleted examples must stay deleted."
          },
          {
            "name": "Progressive overload tracker",
            "desc": "The app suggests how much to increase from the previous session"
          },
          {
            "name": "Advanced charts",
            "desc": "Volume per muscle over time, PR timeline, weekly frequency"
          },
          {
            "name": "Edit workout after saving",
            "desc": "Fix a wrong weight after you've saved"
          },
          {
            "name": "Search workouts in history",
            "desc": "Search by exercise name, date, volume"
          }
        ]
      },
      {
        "theme": "Platform & Infrastructure",
        "features": [
          {
            "name": "Google Play Store",
            "desc": "Store distribution using the existing stable signing key, after the required checks."
          },
          {
            "name": "Automatic backup",
            "desc": "Weekly automatic data export to Google Drive"
          },
          {
            "name": "SQLite encryption",
            "desc": "Database encrypted with SQLCipher"
          },
          {
            "name": "Multi-server switch",
            "desc": "Multiple profiles: switch between different self-hosted servers"
          },
          {
            "name": "Web dashboard",
            "desc": "Web interface to view your data from a PC"
          },
          {
            "name": "Public REST API",
            "desc": "Documented API for third-party integrations"
          },
          {
            "name": "Fully local mode",
            "desc": "Train without an account or server, with optional cloud connection and synchronization."
          },
          {
            "name": "In-app updates",
            "desc": "Download and start installing signed updates without finding the APK manually."
          }
        ]
      },
      {
        "theme": "Gyms and trainers",
        "features": [
          {
            "name": "Trainer Dashboard",
            "desc": "A web interface where personal trainers assign plans and protocols to members."
          },
          {
            "name": "Local TV Leaderboard",
            "desc": "Project the ranking of PRs lifted in real time onto a gym screen."
          },
          {
            "name": "QR Code Onboarding",
            "desc": "Scan the QR at the gym and the app configures itself with the local server."
          },
          {
            "name": "Multi-tenant",
            "desc": "Multiple gyms on the same server with full data isolation."
          }
        ]
      },
      {
        "theme": "Ideas and experiments",
        "features": [
          {
            "name": "AI form check",
            "desc": "Posture analysis via camera during an exercise"
          },
          {
            "name": "Voice input",
            "desc": "\"A hundred kilos, eight reps\" instead of typing"
          },
          {
            "name": "Social login",
            "desc": "Google/Apple sign-in alongside email/password"
          },
          {
            "name": "Gym maps",
            "desc": "Map of nearby gyms with community reviews"
          },
          {
            "name": "Plan marketplace",
            "desc": "Users share and sell workout plans"
          },
          {
            "name": "Nutrition tracker",
            "desc": "Calories and macros alongside workout tracking"
          },
          {
            "name": "Apple Watch",
            "desc": "iOS support + watchOS companion"
          },
          {
            "name": "Gamification 2.0",
            "desc": "Levels and XP exist already: consider competitive seasons and additional rewards."
          },
          {
            "name": "AI coaching",
            "desc": "Automatic suggestions based on historical data"
          },
          {
            "name": "Exercise videos",
            "desc": "Embedded YouTube videos alongside the GIFs"
          },
          {
            "name": "Per-set personal record rewards",
            "desc": "Medals for personal weight, volume or repetition records: a future idea, not implemented."
          }
        ]
      }
    ],
    "completedTitle": "Already released",
    "completedNote": "Features included in the beta; checks and limitations are documented in the README.",
    "completed": [
      {
        "version": "0.8.5 beta",
        "date": "October 13, 2026",
        "features": [
          "Personal plans, one-time examples and automatic notes for exercises already in the plan.",
          "Per-exercise rest timers, timer notifications and previous-session values.",
          "Execution tips that automatically cycle through available steps.",
          "History-based retroactive achievements, XP levels and profile badges.",
          "Profiles, follows, likes, photos and invitations on the same server.",
          "Workout and connection visibility and account deletion with an updated backend.",
          "Calendar, volume and exercise progression; stable APK signing."
        ]
      }
    ]
  },
  "featureDetails": [
    {
      "title": "Plans and sets",
      "desc": "Personal plans without an app-imposed limit; normal, warm-up, drop and failure sets. Confirm structural plan changes when finishing a session."
    },
    {
      "title": "Automatic notes",
      "desc": "Notes for existing plan exercises save without confirmation and return in the next session."
    },
    {
      "title": "Rotating exercise tips",
      "desc": "Above each exercise, available execution steps change automatically during the workout. Instructions and images depend on the catalog."
    },
    {
      "title": "Rest from 0 to 5 minutes",
      "desc": "Per-exercise timer with pause, progress notification and sound. Sound and filters were confirmed on the owner’s Samsung A15; other devices still need verification."
    },
    {
      "title": "Stats and profiles",
      "desc": "Volume, period calendar, per-exercise progression, avatars, workout photos and retroactive badges."
    },
    {
      "title": "Your community’s social feed",
      "desc": "Search, follow, likes and feed within the same server. Share download invitations; no federation between servers."
    },
    {
      "title": "An honest exercise catalogue",
      "desc": "ExerciseDB OSS provider, metadata and GIF URL cache. Server snapshot: 2,305 records, 1,500 with GIF URLs and instructions; duplicates and file availability matter."
    },
    {
      "title": "Server-enforced privacy",
      "desc": "Separate workout and connection visibility. Friends are accepted mutual follows. Migrations are required; self-hosting needs maintenance."
    },
    {
      "title": "Social push needs verification",
      "desc": "FCM client and server worker are implemented. Real background delivery remains unconfirmed; local rest notifications work separately."
    },
    {
      "title": "Partial local support",
      "desc": "Local saves, drafts and workout synchronization. Fully account-free local mode and in-app updates remain future ideas."
    }
  ],
  "story": {
    "title": "From school project to open beta",
    "body": "FORGE started as Leonardo Lalli’s computer science school project. It grew through gym testing, feedback and extensive AI assistance for code, interfaces, documentation and checks. AI-assisted work requires review; the owner remains responsible for decisions and releases. It is currently an experimental beta, not a professional service with guaranteed availability."
  },
  "selfHostingGuide": {
    "title": "A community on your server",
    "intro": "The default server is operated at the owner’s home without guaranteed availability. A personal instance has separate accounts and community; users on different servers cannot see each other.",
    "steps": [
      "Clone the repository and copy .env.example to .env.",
      "Choose PocketBase >= 0.23 and unique administrator credentials.",
      "Start PocketBase, create the superuser and initialize collections.",
      "Apply FORGE hooks and the four migrations with a backup.",
      "Configure the HTTPS reverse proxy and keep administration private.",
      "Enter the address in the app. Social push needs Firebase and a separate worker."
    ],
    "note": "Docker uses an explicitly selected official binary version, not latest. Catalogue caching stores GIF URLs, not files; photos and FCM may use external services. Follow the full guide before using real data.",
    "cta": "Full guide on GitHub"
  },
  "comparison": {
    "title": "Where FORGE stands out",
    "intro": "A comparison of documented features, not a reliability ranking. Checked October 13, 2026.",
    "items": [
      {
        "name": "Hevy",
        "text": "Hevy Free offers four routines; FORGE imposes no plan limit. Hevy has broader social features and more mature tools.",
        "url": "https://help.hevyapp.com/hc/en-us/articles/35119778922263-Hevy-Pro-Subscription-How-to-get-Pro-and-What-Does-It-Include"
      },
      {
        "name": "Strong",
        "text": "Strong Free offers three templates; FORGE has unlimited plans and a customizable backend. Strong offers integrations and Apple Watch support.",
        "url": "https://help.strongapp.io/article/105-about-templates"
      },
      {
        "name": "wger",
        "text": "wger is also open source and self-hosted, including nutrition and measurements. FORGE focuses on Android workouts and community; no general superiority has been measured.",
        "url": "https://github.com/wger-project/wger"
      }
    ]
  },
  "gamification": {
    "eyebrow": "// Gamification",
    "title": "Make each milestone visible.",
    "intro": "Achievements, badges and levels turn your history into visible progress. Supported goals also unlock from workouts you have already completed.",
    "items": [
      {
        "title": "Retroactive achievements",
        "text": "First workout, consistency, volume, duration and variety: implemented criteria use your account’s history."
      },
      {
        "title": "Your level",
        "text": "100 XP per unlocked badge, a level every 500 XP. Recognition of milestones, not a fitness assessment."
      },
      {
        "title": "Profile badges",
        "text": "Choose the unlocked achievements you want to show your community, with their images."
      }
    ],
    "note": "Leaderboards, some catalog criteria and medals for individual set records are not implemented yet."
  },
  "screenshots": {
    "eyebrow": "// Inside FORGE",
    "title": "The app on your phone.",
    "intro": "Workout: a recent capture supplied by the owner, showing rotating tips, sets and notes. Dashboard, stats and profile are archived development captures and may differ from the current UI.",
    "labels": [
      "Dashboard",
      "Workout · recent capture",
      "Statistics",
      "Profile"
    ]
  }
}
