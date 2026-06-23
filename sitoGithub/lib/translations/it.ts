export const it = {
  nav: {
    features: "Funzionalità",
    selfHosting: "Self-Hosting",
    docs: "Documentazione",
    roadmap: "Roadmap",
    download: "Download",
    backHome: "Torna alla home",
  },
  controls: {
    toggleThemeToLight: "Passa al tema chiaro",
    toggleThemeToDark: "Passa al tema scuro",
    language: "Lingua",
    switchToEnglish: "Switch to English",
    switchToItalian: "Passa all'italiano",
  },
  hero: {
    badge: "Open-source · Privacy-first · Self-hostable",
    title: "Forgia il tuo fisico.\nTraccia ogni progresso.",
    subtitle:
      "FORGE è l'app di palestra open-source che mette i tuoi dati al primo posto. Traccia volumi e carichi, gestisci i tempi di recupero e ospita tutto sul tuo server.",
    ctaPrimary: "Scarica l'APK Android",
    ctaSecondary: "Vedi su GitHub",
    note: "APK Android · Nessun account cloud richiesto · Gratis per sempre",
  },
  features: {
    eyebrow: "// Funzionalità",
    title: "Tutto quello che serve per allenarti meglio.",
    subtitle:
      "Niente fronzoli, niente paywall. Solo gli strumenti essenziali per registrare i tuoi allenamenti e migliorare nel tempo.",
    items: [
      {
        title: "Tracciamento volumi",
        desc: "Registra serie, ripetizioni e carichi. FORGE calcola automaticamente il volume totale e i tuoi top lift per ogni esercizio.",
      },
      {
        title: "Privacy totale",
        desc: "I tuoi dati restano tuoi. Self-hosting con PocketBase: nessun cloud di terze parti, nessun tracker, nessuna pubblicità.",
      },
      {
        title: "Timer di recupero",
        desc: "Timer di recupero configurabile tra le serie, con notifica al termine, così mantieni il ritmo giusto senza distrazioni.",
      },
    ],
  },
  selfHosting: {
    eyebrow: "// Self-Hosting",
    title: "Il tuo server, le tue regole.",
    subtitle:
      "FORGE usa PocketBase come backend: un singolo binario con database, autenticazione e file storage. Avvialo con Docker Compose in pochi secondi.",
    steps: [
      "Backend completo con un solo container",
      "Autenticazione e database inclusi",
      "Aggiornamenti con un singolo comando",
    ],
    terminalTitle: "terminal",
    terminalComment: "# Avvia il backend FORGE con PocketBase",
    note: "Richiede Docker e Docker Compose. PocketBase espone la dashboard admin sulla porta 8090.",
    cta: "Guida al self-hosting",
  },
  docsTeaser: {
    eyebrow: "// Documentazione",
    title: "Tutto il progetto, in chiaro.",
    subtitle:
      "Specifica, architettura, roadmap e diario di sviluppo di FORGE — la stessa documentazione che guida il progetto open-source.",
    cards: [
      { title: "Specifica", desc: "Visione, utenti target e requisiti funzionali dell'MVP." },
      { title: "Architettura", desc: "Stack .NET MAUI, struttura repo e collezioni PocketBase." },
      { title: "Roadmap", desc: "Iterazioni completate e funzionalità in arrivo." },
      { title: "Diario", desc: "Le fasi di sviluppo e i problemi risolti lungo il percorso." },
    ],
    cta: "Apri la documentazione",
  },
  footer: {
    tagline: "Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.",
    product: "Prodotto",
    resources: "Risorse",
    links: {
      features: "Funzionalità",
      selfHosting: "Self-Hosting",
      download: "Download APK",
      docs: "Documentazione",
      roadmap: "Roadmap",
      github: "GitHub",
    },
    rights: "Progetto open-source rilasciato sotto licenza MIT.",
  },
  cta: {
    title: "Pronto a iniziare ad allenarti con FORGE?",
    subtitle: "Scarica l'APK, avvia il tuo server e prendi il controllo dei tuoi dati di allenamento.",
    primary: "Scarica l'APK Android",
    secondary: "Leggi la documentazione",
  },
  docs: {
    eyebrow: "// Documentazione",
    title: "Tutto il progetto, in chiaro.",
    subtitle:
      "Specifica, architettura, roadmap e diario di sviluppo di FORGE — la stessa documentazione che guida il progetto open-source.",
    tabs: {
      spec: "Specifica",
      architettura: "Architettura",
      roadmap: "Roadmap",
      diario: "Diario",
    },
    spec: {
      visionEyebrow: "// Visione",
      visionTitle: "Tracking + social in un'unica app",
      visionBody:
        "FORGE risolve i tre ostacoli di chi va in palestra: ricordare i carichi, sapere quali esercizi fare e restare motivati. Un'app .NET MAUI Android-first che unisce catalogo esercizi, registrazione allenamenti, statistiche e una dimensione sociale con feed e like.",
      targetEyebrow: "// Utenti target",
      targetUsers: [
        "Principianti che iniziano ad allenarsi e cercano struttura",
        "Appassionati di fitness che vogliono tracciare i progressi nel tempo",
        "Gruppi di amici che vogliono motivarsi con una competizione sana",
      ],
      mvpEyebrow: "// Ambito MVP",
      mvpTitle: "Funzionalità implementate",
      mvpFeatures: [
        "Autenticazione PocketBase (email/password/username) con auto-login",
        "Catalogo esercizi ExerciseDB con ricerca e filtri per muscolo e attrezzatura",
        "Registrazione allenamenti: serie, ripetizioni, peso e rest timer configurabile",
        "Dashboard con streak settimanale, squad activity e today card",
        "Feed sociale: follow, like e richieste con notifiche",
        "Statistiche: grafico volume, top lifts, calendario e filtri temporali",
        "Profilo con avatar, bio e storico allenamenti",
        "Piani di allenamento salvabili (Quick Start o da piano)",
        "Doppio tema chiaro/scuro con toggle runtime",
      ],
      reqEyebrow: "// Requisiti funzionali",
      requirements: [
        { id: "FR-01", text: "Recupero esercizi da ExerciseDB API con lista e ricerca", done: true },
        { id: "FR-02", text: "Cache esercizi su PocketBase con URL immagini risolti", done: true },
        { id: "FR-05", text: "Salvataggio allenamento completo su PocketBase", done: true },
        { id: "FR-08", text: "Statistiche di progresso: volume, top lifts, calendario", done: true },
        { id: "FR-13", text: "Feed allenamenti amici con like", done: true },
        { id: "FR-15", text: "Streak settimanale in Dashboard e Profilo", done: true },
        { id: "FR-07", text: "Tracking peso corporeo e misure", done: false },
        { id: "FR-14", text: "Leaderboard settimanale", done: false },
      ],
    },
    arch: {
      stackEyebrow: "// Stack tecnologico",
      stack: [
        { tech: ".NET MAUI 10", role: "Framework cross-platform Android-first" },
        { tech: "CommunityToolkit.Mvvm 8.4", role: "MVVM: ObservableProperty, RelayCommand, Messenger" },
        { tech: "Shell", role: "Navigazione 3 tab + 8 route di dettaglio" },
        { tech: "PocketBase", role: "Auth, database remoto, file storage, social graph" },
        { tech: "ExerciseDB v1", role: "Catalogo 1.500+ esercizi con GIF, gratuito" },
        { tech: "SQLite", role: "Cache offline: workout, esercizi, piani, achievement" },
        { tech: "SecureStorage", role: "Password e dati sensibili (Android Keystore)" },
        { tech: "Nginx Proxy Manager", role: "Reverse proxy HTTPS, Let's Encrypt, rate limit" },
      ],
      repoEyebrow: "// Struttura repository",
      repoTree: `src/Forge/
├── App.xaml(.cs)            # Entry point + auto-login
├── AppShell.xaml(.cs)       # TabBar + route di dettaglio
├── MauiProgram.cs           # DI composition root
├── Models/                  # Entità dominio e DTO
├── Services/                # PocketBase, ExerciseDB, Theme, Sync...
├── ViewModels/              # MVVM (BaseViewModel + 11 VM)
├── Views/                   # Pagine XAML pure
├── Converters/
└── Resources/               # Styles, Fonts, Raw/forge.env`,
      navEyebrow: "// Navigazione Shell",
      navTabs: ["Dashboard", "Feed", "Stats"],
      navRoutes: ["startSession", "activeWorkout", "workoutDetail", "achievements", "profile", "settings"],
      collEyebrow: "// Collezioni PocketBase",
      collHeadName: "Collection",
      collHeadFields: "Campi principali",
      collections: [
        { name: "users", fields: "email, password, name, bio, avatar" },
        { name: "logged_workouts", fields: "user, name, date, exercises, volume, duration, likes, liked_by" },
        { name: "social_graph", fields: "from_user, from_name, to_user, status" },
        { name: "excercise", fields: "name, bodyPart, equipment, instructions, imageUrl, level" },
      ],
    },
    roadmap: {
      iterations: [
        { id: "IT-01", goal: "Bootstrap MAUI + Shell 3-tab + Design System + ThemeService", status: "done" },
        { id: "IT-02/03/08", goal: "Catalogo esercizi + Allenamento + Piani salvabili", status: "done" },
        { id: "IT-05/06/07", goal: "Auth PocketBase + Social + Dashboard + Stats + Profilo", status: "done" },
        { id: "IT-FEAT-01", goal: "SQLite offline (DatabaseService, sync auto)", status: "done" },
        { id: "IT-FEAT-03", goal: "ExerciseDB v1 free API (no API key, 1.500 esercizi)", status: "done" },
        { id: "IT-FEAT-06", goal: "Achievement system: 48 badge in 6 categorie", status: "done" },
        { id: "IT-SEC-01/02/03", goal: "Security audit, hardening e certificate pinning", status: "done" },
        { id: "Push Notifications", goal: "FCM hook pronto, SDK non compatibile con .NET 10", status: "partial" },
        { id: "IT-04", goal: "Tracking peso corporeo e misure", status: "skip" },
      ],
      status: { done: "Completata", partial: "Parziale", skip: "Posticipata" },
    },
    journal: [
      {
        phase: "IT-01",
        title: "Bootstrap MAUI e Design System",
        body: "Progetto .NET MAUI con Shell a 3 tab, temi runtime chiaro/scuro e font Google. Decisione chiave: 3 tab invece di 5 per una UI più pulita.",
      },
      {
        phase: "IT-02/03/08",
        title: "Catalogo, Allenamento e Piani",
        body: "Integrazione ExerciseDB, flusso allenamento completo con rest timer e piani salvabili. Risolti short URL bloccati dagli ISP e crash da CollectionView annidate.",
      },
      {
        phase: "IT-05/06/07",
        title: "Auth, Social, Dashboard, Stats e Profilo",
        body: "PocketBase self-hosted al posto di Firebase, feed con like, statistiche e profilo. Streak calcolata settimanalmente con tolleranza di 7 giorni.",
      },
      {
        phase: "IT-FEAT-01 → 07",
        title: "Offline, ExerciseDB v1, CSV, Foto, Achievement",
        body: "SQLite + sync automatico, migrazione all'API gratuita (1.500 esercizi), import/export CSV, foto progresso e un sistema di 48 achievement.",
      },
      {
        phase: "IT-SEC-01 → 03",
        title: "Sicurezza e Hardening",
        body: "Password su SecureStorage, IHttpClientFactory, token fuori dagli URL, certificate pinning Let's Encrypt e API Rules con row-level ownership.",
      },
    ],
  },
  roadmapPage: {
    eyebrow: "// Roadmap",
    title: "Dove sta andando FORGE.",
    subtitle:
      "Le prossime release, fase per fase. Una timeline pubblica delle funzionalità che vogliamo costruire — dai miglioramenti di usabilità al modulo B2B per le palestre.",
    legend: { difficulty: "Difficoltà" },
    difficulty: { low: "Bassa", med: "Media", high: "Alta", vhigh: "Molto alta" },
    releases: [
      {
        version: "v0.9.0-beta",
        date: "",
        theme: "QoL & Usabilità",
        features: [
          { name: "Incremento rapido pesi (+/-)", desc: "Pulsanti +2.5kg accanto al campo peso. Press-and-hold = +5kg", diff: "low" },
          { name: "Copia set precedente", desc: 'Tasto "Duplica" che clona kg×reps nella serie successiva', diff: "low" },
          { name: "Calcolatore 1RM", desc: "Tap su un top lift → mostra massimale stimato (formula Epley)", diff: "low" },
          { name: "Timer a voce", desc: 'TTS Android: "Serie completata, 90 secondi di pausa"', diff: "med" },
          { name: "Filtro ricerca esercizi migliorato", desc: "Filtri combinabili (muscolo AND attrezzatura)", diff: "low" },
        ],
      },
      {
        version: "v1.0.0",
        date: "",
        theme: "Release Stabile",
        features: [
          { name: "Notifiche push FCM", desc: "Notifica quando un amico ti segue, mette like o accetta una richiesta", diff: "high" },
          { name: "Localizzazione completa", desc: "File .resx per IT/EN/ES/DE/ZH — tutte le stringhe tradotte", diff: "high" },
          { name: "Body tracking", desc: "Registra peso corporeo, misure, %BF. Grafico nel tempo", diff: "med" },
          { name: "Leaderboard settimanale", desc: "Classifica volume tra amici, reset ogni lunedì", diff: "med" },
          { name: "Auto-build APK nelle Release", desc: "GitHub Action che crea release automatica a ogni tag", diff: "low" },
        ],
      },
      {
        version: "v1.1.0",
        date: "",
        theme: "Social & Community",
        features: [
          { name: "Commenti ai workout", desc: "Gli amici possono commentare i tuoi allenamenti nel feed", diff: "med" },
          { name: "Sfide 1-vs-1", desc: '"Sfida Marco a volume totale questa settimana" → notifica + classifica', diff: "med" },
          { name: "Condivisione workout", desc: "Share sheet Android: immagine riassuntiva dell'allenamento", diff: "med" },
          { name: "Badge stagionali", desc: 'Achievement limitati nel tempo (es. "Allenati a Natale")', diff: "low" },
          { name: "Storie workout", desc: "Foto del giorno visibili 24h nel feed (stile Instagram Stories)", diff: "high" },
        ],
      },
      {
        version: "v1.2.0",
        date: "",
        theme: "Health & Wear OS",
        features: [
          { name: "Widget Android", desc: '"START WORKOUT" direttamente dalla home screen del telefono', diff: "med" },
          { name: "Wear OS companion", desc: "Timer rest + tracking serie dallo smartwatch", diff: "vhigh" },
          { name: "Integrazione Google Fit", desc: "Sincronizza durata allenamento + calorie con Google Fit", diff: "med" },
          { name: "Heart rate display", desc: "Mostra battito cardiaco durante l'allenamento (se supportato)", diff: "high" },
          { name: "Contatore passi", desc: "Widget passi giornalieri nella dashboard", diff: "low" },
        ],
      },
      {
        version: "v1.3.0",
        date: "",
        theme: "Advanced Features",
        features: [
          { name: "PDF report", desc: "Export report allenamento con grafici, tabelle e foto", diff: "med" },
          { name: "Allenamenti predefiniti", desc: 'Template "Push/Pull/Legs", "Full Body", "Upper/Lower"', diff: "low" },
          { name: "Progressive overload tracker", desc: "L'app suggerisce quanto aumentare rispetto alla sessione precedente", diff: "med" },
          { name: "Grafici avanzati", desc: "Volume per muscolo nel tempo, PR timeline, frequenza settimanale", diff: "med" },
          { name: "Modifica workout post-salvataggio", desc: "Correggi un peso sbagliato dopo aver salvato", diff: "low" },
          { name: "Ricerca workout nello storico", desc: "Cerca per nome esercizio, data, volume", diff: "low" },
        ],
      },
      {
        version: "v1.4.0",
        date: "",
        theme: "Platform & Infrastruttura",
        features: [
          { name: "Google Play Store", desc: "Pubblicazione ufficiale con keystore di produzione", diff: "med" },
          { name: "Backup automatico", desc: "Export automatico settimanale dei dati su Google Drive", diff: "med" },
          { name: "SQLite encryption", desc: "Database cifrato con SQLCipher", diff: "med" },
          { name: "Multi-server switch", desc: "Profili multipli: switch tra server self-hosted diversi", diff: "low" },
          { name: "Web dashboard", desc: "Interfaccia web per visualizzare i dati da PC", diff: "high" },
          { name: "REST API pubblica", desc: "API documentata per integrazioni di terze parti", diff: "high" },
        ],
      },
    ],
    phasesEyebrow: "// Sintesi",
    phasesTitle: "I tre pilastri in arrivo",
    phases: [
      {
        tag: "Fase 1",
        title: "Smart Workout Flow",
        body: "Timer di recupero automatico con vibrazione, pesi dell'ultima sessione già visibili come placeholder (Ghost Inputs) e tasti rapidi +/- per variare i carichi di 2.5kg senza tastiera.",
      },
      {
        tag: "Fase 2",
        title: "Analytics Avanzate",
        body: "Calcolo automatico del massimale stimato (1RM con formula Epley), grafici interattivi per la distribuzione del volume sui gruppi muscolari e calendario delle streak per la costanza.",
      },
      {
        tag: "Fase 3",
        title: "Self-Hosting Pro & Social",
        body: "Feed amici in tempo reale sullo stesso server PocketBase e modalità Offline First per salvare in palestra anche senza campo e sincronizzare al rientro a casa.",
      },
    ],
    b2b: {
      eyebrow: "// FORGE Local Business",
      title: "Un modulo B2B per le palestre",
      body: "L'architettura self-hosted permette a un proprietario di palestra di lanciare il backend su un mini-PC locale e creare un sistema gestionale e social privato per tutta la struttura.",
      advantages: [
        { title: "Zero costi SaaS", desc: "Niente abbonamenti o licenze per postazione. Da 50 a 5.000 iscritti, il costo software è 0€/mese." },
        { title: "Community locale", desc: "Chi si allena nella stessa sala vede i PR degli altri in tempo reale, con classifiche interne della palestra." },
        { title: "Privacy e GDPR al 100%", desc: "I dati su salute e performance non lasciano mai l'edificio. Un vero punto di marketing." },
        { title: "Offline / LAN First", desc: "Se internet si interrompe, l'app continua a funzionare via Wi-Fi locale. Zero downtime durante gli allenamenti." },
      ],
      future: [
        { title: "Trainer Dashboard", desc: "Interfaccia web dove i personal trainer assegnano schede e protocolli agli iscritti." },
        { title: "Local TV Leaderboard", desc: "Proietta su uno schermo della palestra la classifica dei PR sollevati in tempo reale." },
        { title: "QR Code Onboarding", desc: "Scansiona il QR in palestra e l'app si configura da sola con il server locale." },
        { title: "Multi-tenant", desc: "Più palestre sullo stesso server con isolamento completo dei dati." },
      ],
      futureTitle: "Sviluppi futuri B2B",
    },
    backlog: {
      eyebrow: "// Backlog",
      title: "Idee & esperimenti",
      items: [
        { name: "AI form check", desc: "Analisi posturale via fotocamera durante l'esercizio" },
        { name: "Voice input", desc: '"Cento chili, otto rep" invece di digitare' },
        { name: "Social login", desc: "Google/Apple sign-in oltre a email/password" },
        { name: "Mappe palestre", desc: "Mappa delle palestre vicine con recensioni community" },
        { name: "Marketplace piani", desc: "Utenti condividono e vendono piani di allenamento" },
        { name: "Nutrizione tracker", desc: "Calorie e macro insieme al tracking degli allenamenti" },
        { name: "Apple Watch", desc: "Supporto iOS + companion watchOS" },
        { name: "Gamification 2.0", desc: "Livelli, XP, stagioni competitive" },
        { name: "Coaching AI", desc: "Suggerimenti automatici basati sui dati storici" },
        { name: "Video esercizi", desc: "Video YouTube integrati oltre alle GIF" },
      ],
    },
    closing: "Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.",
  },
} as const
