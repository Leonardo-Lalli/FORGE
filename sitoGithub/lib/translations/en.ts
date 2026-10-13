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
    "subtitle": "The next releases, phase by phase. A public timeline of the features we want to build — from usability improvements to the B2B module for gyms.",
    "legend": {
      "difficulty": "Difficulty"
    },
    "difficulty": {
      "low": "Low",
      "med": "Medium",
      "high": "High",
      "vhigh": "Very high"
    },
    "releases": [
      {
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
    "phasesEyebrow": "// Summary",
    "phasesTitle": "The three pillars ahead",
    "phases": [
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
    "b2b": {
      "eyebrow": "// FORGE Local Business",
      "title": "A B2B module for gyms",
      "body": "The self-hosted architecture lets a gym owner launch the backend on a local mini-PC and create a private management and social system for the whole facility.",
      "advantages": [
        {
          "title": "Zero SaaS costs",
          "desc": "No subscriptions or per-seat licenses. From 50 to 5,000 members, software cost is €0/month."
        },
        {
          "title": "Local community",
          "desc": "People training in the same room see each other's PRs in real time, with the gym's internal rankings."
        },
        {
          "title": "Local data control",
          "desc": "Local hosting helps control data. Privacy and compliance also depend on access, consent, backups and server operations."
        },
        {
          "title": "Offline / LAN First",
          "desc": "If the internet drops, the app keeps working over local Wi-Fi. Local access depends on the gym network and server."
        }
      ],
      "future": [
        {
          "title": "Trainer Dashboard",
          "desc": "A web interface where personal trainers assign plans and protocols to members."
        },
        {
          "title": "Local TV Leaderboard",
          "desc": "Project the ranking of PRs lifted in real time onto a gym screen."
        },
        {
          "title": "QR Code Onboarding",
          "desc": "Scan the QR at the gym and the app configures itself with the local server."
        },
        {
          "title": "Multi-tenant",
          "desc": "Multiple gyms on the same server with full data isolation."
        }
      ],
      "futureTitle": "Future B2B developments"
    },
    "backlog": {
      "eyebrow": "// Backlog",
      "title": "Ideas & experiments",
      "items": [
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
          "desc": "Levels, XP, competitive seasons"
        },
        {
          "name": "AI coaching",
          "desc": "Automatic suggestions based on historical data"
        },
        {
          "name": "Exercise videos",
          "desc": "Embedded YouTube videos alongside the GIFs"
        }
      ]
    },
    "closing": "Build your body. Challenge your friends. Forge your legend."
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
