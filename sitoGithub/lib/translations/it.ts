export const it = {
  "nav": {
    "features": "Funzionalità",
    "selfHosting": "Self-Hosting",
    "docs": "Documentazione",
    "roadmap": "Roadmap",
    "download": "Download",
    "backHome": "Torna alla home",
    "b2b": "B2B"
  },
  "controls": {
    "toggleThemeToLight": "Passa al tema chiaro",
    "toggleThemeToDark": "Passa al tema scuro",
    "language": "Lingua",
    "switchToEnglish": "Switch to English",
    "switchToItalian": "Passa all'italiano"
  },
  "hero": {
    "badge": "0.8.5 beta · Android · Open source",
    "title": "Forgia il tuo fisico.\nTraccia ogni progresso.",
    "subtitle": "Registra allenamenti, ritrova i tuoi carichi e condividi i progressi con la tua community. Usa il server FORGE predefinito oppure una tua istanza PocketBase.",
    "ctaPrimary": "Scarica FORGE 0.8.5 beta",
    "ctaSecondary": "Vedi su GitHub",
    "note": "0.8.5 beta · Account sul server scelto · Nessun abbonamento nell’app",
    "updateNote": "Vecchie installazioni: la firma è cambiata e potrebbe servire una reinstallazione. Il CSV non è un backup completo: conserva le schede e le foto separatamente e verifica la sincronizzazione prima di rimuovere l’app."
  },
  "features": {
    "eyebrow": "// Funzionalità",
    "title": "Tutto quello che serve per allenarti meglio.",
    "subtitle": "Strumenti di allenamento già implementati, con limiti e verifiche della beta dichiarati nella documentazione.",
    "items": [
      {
        "title": "Tracciamento volumi",
        "desc": "Registra serie, ripetizioni e carichi. FORGE calcola automaticamente il volume totale e i tuoi top lift per ogni esercizio."
      },
      {
        "title": "Privacy configurabile",
        "desc": "Allenamenti ed elenchi follower pubblici, privati o per amici reciproci. Le regole sono applicate dal backend FORGE aggiornato."
      },
      {
        "title": "Timer di recupero",
        "desc": "Timer di recupero configurabile tra le serie, con notifica al termine, così mantieni il ritmo giusto senza distrazioni."
      },
      {
        "title": "Consigli a carosello",
        "desc": "Sopra ogni esercizio, gli step di esecuzione disponibili cambiano automaticamente durante l’allenamento. Istruzioni e immagini dipendono dal catalogo."
      }
    ]
  },
  "selfHosting": {
    "eyebrow": "// Self-Hosting",
    "title": "Il tuo server, le tue regole.",
    "subtitle": "PocketBase gestisce account, allenamenti e social. Installa una versione scelta esplicitamente e applica gli hook e le migrazioni FORGE.",
    "steps": [
      "Community separata per server",
      "Credenziali amministrative personali",
      "Aggiornamenti con backup e migrazioni"
    ],
    "terminalTitle": "terminal",
    "terminalComment": "# Avvia il backend FORGE con PocketBase",
    "note": "Richiede Docker e Docker Compose. Installa anche migrazioni e hook FORGE per privacy e cache condivisa degli esercizi: il server salva metadati degli esercizi, istruzioni e URL GIF. I file GIF restano sul provider e richiedono connettività. Amministrazione da mantenere privata.",
    "cta": "Guida al self-hosting"
  },
  "docsTeaser": {
    "eyebrow": "// Documentazione",
    "title": "Tutto il progetto, in chiaro.",
    "subtitle": "Specifica, architettura, roadmap e diario di sviluppo di FORGE — la stessa documentazione che guida il progetto open-source.",
    "cards": [
      {
        "title": "Specifica",
        "desc": "Visione, utenti target e requisiti funzionali dell'MVP."
      },
      {
        "title": "Architettura",
        "desc": "Stack .NET MAUI, struttura repo e collezioni PocketBase."
      },
      {
        "title": "Roadmap",
        "desc": "Iterazioni completate e funzionalità in arrivo."
      },
      {
        "title": "Diario",
        "desc": "Le fasi di sviluppo e i problemi risolti lungo il percorso."
      }
    ],
    "cta": "Apri la documentazione"
  },
  "footer": {
    "tagline": "Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.",
    "product": "Prodotto",
    "resources": "Risorse",
    "links": {
      "features": "Funzionalità",
      "selfHosting": "Self-Hosting",
      "download": "Download APK",
      "docs": "Documentazione",
      "roadmap": "Roadmap",
      "github": "GitHub"
    },
    "rights": "Progetto open-source rilasciato sotto licenza MIT."
  },
  "cta": {
    "title": "Pronto a iniziare ad allenarti con FORGE?",
    "subtitle": "Scarica la beta, scegli la tua community e segnala ciò che possiamo migliorare.",
    "primary": "Scarica l'APK Android",
    "secondary": "Leggi la documentazione"
  },
  "docs": {
    "eyebrow": "// Documentazione",
    "title": "Tutto il progetto, in chiaro.",
    "subtitle": "Specifica, architettura, roadmap e diario di sviluppo di FORGE — la stessa documentazione che guida il progetto open-source.",
    "tabs": {
      "spec": "Specifica",
      "architettura": "Architettura",
      "roadmap": "Roadmap",
      "diario": "Diario"
    },
    "spec": {
      "visionEyebrow": "// Visione",
      "visionTitle": "Tracking + social in un'unica app",
      "visionBody": "FORGE è nato come progetto studentesco di informatica di Leonardo Lalli. È cresciuto attraverso prove in palestra, feedback e un uso esteso di strumenti AI per codice, interfacce, documentazione e verifiche. Il lavoro assistito dall’AI richiede revisione: la responsabilità delle scelte e delle release resta del proprietario. Oggi è una beta sperimentale, non un servizio professionale con disponibilità garantita.",
      "targetEyebrow": "// Utenti target",
      "targetUsers": [
        "Principianti che iniziano ad allenarsi e cercano struttura",
        "Appassionati di fitness che vogliono tracciare i progressi nel tempo",
        "Gruppi di amici che vogliono motivarsi con una competizione sana"
      ],
      "mvpEyebrow": "// Ambito MVP",
      "mvpTitle": "Funzionalità implementate",
      "mvpFeatures": [
        "Schede e serie — Schede personali senza un limite imposto dall’app; serie normali, warm-up, drop set e cedimento. Conferma le modifiche alla scheda a fine sessione.",
        "Note automatiche — Le note degli esercizi già presenti si salvano nella scheda senza conferma e tornano alla sessione successiva.",
        "Recupero 0–5 minuti — Timer per esercizio con pausa, notifica di avanzamento e avviso sonoro. Suono e filtri confermati sul Samsung A15 del proprietario; comportamento su altri dispositivi da verificare.",
        "Statistiche e profilo — Volume, calendario per periodo, progressione per esercizio, avatar, foto degli allenamenti e badge retroattivi.",
        "Social della tua community — Ricerca, follow, like e feed tra utenti dello stesso server. Inviti condivisibili dal sito; nessuna federazione tra server."
      ],
      "reqEyebrow": "// Requisiti funzionali",
      "requirements": [
        {
          "id": "BETA",
          "title": "Stato attuale",
          "body": "Flusso base provato dal proprietario; beta da validare con altri utenti."
        }
      ]
    },
    "arch": {
      "stackEyebrow": "// Stack tecnologico",
      "stack": [
        {
          "tech": ".NET 10 MAUI",
          "role": "Android · MVVM · CommunityToolkit"
        },
        {
          "tech": "PocketBase",
          "role": "Account, community, privacy e storage"
        },
        {
          "tech": "SQLite",
          "role": "Schede, cache, bozze e allenamenti locali"
        },
        {
          "tech": "ExerciseDB OSS",
          "role": "Metadati, istruzioni e URL GIF"
        },
        {
          "tech": "Firebase FCM",
          "role": "Push social facoltative; worker separato"
        }
      ],
      "repoEyebrow": "// Struttura repository",
      "repoTree": "src/Forge/\n├── App.xaml(.cs)            # Entry point + auto-login\n├── AppShell.xaml(.cs)       # TabBar + route di dettaglio\n├── MauiProgram.cs           # DI composition root\n├── Models/                  # Entità dominio e DTO\n├── Services/                # PocketBase, ExerciseDB, Theme, Sync...\n├── ViewModels/              # MVVM (BaseViewModel + 11 VM)\n├── Views/                   # Pagine XAML pure\n├── Converters/\n└── Resources/               # Styles, Fonts, Raw/forge.env",
      "navEyebrow": "// Navigazione Shell",
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
      "collEyebrow": "// Collezioni PocketBase",
      "collHeadName": "Collection",
      "collHeadFields": "Campi principali",
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
          "goal": "Stabilizzare allenamenti, note, timer e salvataggi.",
          "status": "partial"
        },
        {
          "id": "FEEDBACK",
          "goal": "Correggere i problemi segnalati e migliorare catalogo e backup.",
          "status": "partial"
        },
        {
          "id": "FUTURO",
          "goal": "Ruoli trainer e iscritti, inviti QR e più palestre.",
          "status": "skip"
        }
      ],
      "status": {
        "done": "Completata",
        "partial": "Parziale",
        "skip": "Posticipata"
      }
    },
    "journal": [
      {
        "phase": "Origini",
        "title": "Da progetto scolastico a beta aperta",
        "body": "FORGE è nato come progetto studentesco di informatica di Leonardo Lalli. È cresciuto attraverso prove in palestra, feedback e un uso esteso di strumenti AI per codice, interfacce, documentazione e verifiche. Il lavoro assistito dall’AI richiede revisione: la responsabilità delle scelte e delle release resta del proprietario. Oggi è una beta sperimentale, non un servizio professionale con disponibilità garantita."
      },
      {
        "phase": "Ottobre 2026",
        "title": "Prove su dispositivo",
        "body": "Login, workout, suono del timer e filtri provati dal proprietario. Build Android e regressioni mirate documentate; non equivalgono a una verifica completa su tutti i dispositivi."
      }
    ]
  },
  "roadmapPage": {
    "eyebrow": "// Roadmap",
    "title": "Dove sta andando FORGE.",
    "subtitle": "Le prossime release, fase per fase. Una timeline pubblica delle funzionalità che vogliamo costruire — dai miglioramenti di usabilità al modulo B2B per le palestre.",
    "legend": {
      "difficulty": "Difficoltà"
    },
    "difficulty": {
      "low": "Bassa",
      "med": "Media",
      "high": "Alta",
      "vhigh": "Molto alta"
    },
    "releases": [
      {
        "theme": "Stabilità & Privacy",
        "features": [
          {
            "name": "Accesso affidabile",
            "desc": "Login pubblico e server personali; avatar e recupero sessione verificati",
            "diff": "med"
          },
          {
            "name": "Privacy sul server",
            "desc": "Allenamenti pubblici, privati o per amici reciproci; test delle API Rules",
            "diff": "high"
          },
          {
            "name": "Schede per account",
            "desc": "Schede personali senza limiti; esempi una sola volta; note e recuperi persistenti",
            "diff": "med"
          },
          {
            "name": "Achievement retroattivi",
            "desc": "Ricalcolo dello storico e tre badge scelti nel profilo",
            "diff": "med"
          },
          {
            "name": "Regressioni e grafici",
            "desc": "Grafico volume corretto; build Android e test automatici",
            "diff": "med"
          },
          {
            "name": "Social login",
            "desc": "Google/Apple sign-in oltre a email/password",
            "diff": "high"
          }
        ]
      },
      {
        "theme": "Flusso allenamento verificato",
        "features": [
          {
            "name": "Recuperi per esercizio",
            "desc": "Selezione 0–5 minuti, salvata nelle schede",
            "diff": "med"
          },
          {
            "name": "Sessione precedente",
            "desc": "Pesi e ripetizioni suggeriti dallo storico, modificabili",
            "diff": "med"
          },
          {
            "name": "Offline e sincronizzazione",
            "desc": "Verifica su dispositivo di salvataggi, bozze e cambio account",
            "diff": "high"
          },
          {
            "name": "Catalogo esercizi",
            "desc": "Conteggio reale, GIF, istruzioni e paginazione completa",
            "diff": "med"
          },
          {
            "name": "Release Android",
            "desc": "Build APK riproducibile e validazione prima della release",
            "diff": "med"
          },
          {
            "name": "Voice input",
            "desc": "\"Cento chili, otto rep\" invece di digitare",
            "diff": "med"
          },
          {
            "name": "Video esercizi",
            "desc": "Video YouTube integrati oltre alle GIF",
            "diff": "med"
          }
        ]
      },
      {
        "theme": "Social & Community",
        "features": [
          {
            "name": "Profili e follow",
            "desc": "Profili ricercabili, follower e seguiti; amici = follow reciproco",
            "diff": "med"
          },
          {
            "name": "Commenti e notifiche",
            "desc": "Commenti e notifiche push dopo il consolidamento della privacy",
            "diff": "high"
          },
          {
            "name": "Sfide e classifiche",
            "desc": "Sfide tra amici e leaderboard con consenso",
            "diff": "med"
          },
          {
            "name": "Condivisione",
            "desc": "Riepiloghi condivisibili senza esporre contenuti privati",
            "diff": "med"
          },
          {
            "name": "Mappe palestre",
            "desc": "Mappa delle palestre vicine con recensioni community",
            "diff": "high"
          },
          {
            "name": "Gamification 2.0",
            "desc": "Estendere livelli e XP già presenti con stagioni competitive e nuove ricompense.",
            "diff": "high"
          }
        ]
      },
      {
        "theme": "Analytics & Localizzazione",
        "features": [
          {
            "name": "Analytics avanzate",
            "desc": "1RM, volume per muscolo, calendario e storico",
            "diff": "med"
          },
          {
            "name": "Body tracking",
            "desc": "Misure e peso con scelte di riservatezza",
            "diff": "high"
          },
          {
            "name": "Lingue e accessibilità",
            "desc": "Traduzioni complete e test di contrasto e dimensioni testo",
            "diff": "high"
          },
          {
            "name": "Report e backup",
            "desc": "Export PDF e backup con ripristino verificato",
            "diff": "med"
          },
          {
            "name": "Nutrizione tracker",
            "desc": "Calorie e macro insieme al tracking degli allenamenti",
            "diff": "high"
          }
        ]
      },
      {
        "theme": "Health & Wearables",
        "features": [
          {
            "name": "Widget e Wear OS",
            "desc": "Accesso rapido e companion per timer e serie",
            "diff": "vhigh"
          },
          {
            "name": "Integrazioni salute",
            "desc": "Valutazione delle API supportate e autorizzazioni esplicite",
            "diff": "high"
          },
          {
            "name": "Sensori",
            "desc": "Battito e passi dove supportati dal dispositivo",
            "diff": "high"
          },
          {
            "name": "Apple Watch",
            "desc": "Supporto iOS + companion watchOS",
            "diff": "vhigh"
          }
        ]
      },
      {
        "theme": "Infrastruttura & B2B",
        "features": [
          {
            "name": "Origine protetta",
            "desc": "Proxy o tunnel con dominio dedicato; IP domestico fuori dal DNS pubblico",
            "diff": "high"
          },
          {
            "name": "Distribuzione",
            "desc": "Play Store e gestione chiavi di produzione",
            "diff": "med"
          },
          {
            "name": "Dashboard e API",
            "desc": "Web dashboard, API documentata e profili server",
            "diff": "high"
          },
          {
            "name": "Palestre",
            "desc": "Trainer dashboard, QR onboarding e isolamento tra palestre",
            "diff": "vhigh"
          }
        ]
      }
    ],
    "phasesEyebrow": "// Sintesi",
    "phasesTitle": "I tre pilastri in arrivo",
    "phases": [
      {
        "tag": "Fase 1",
        "title": "Stabilità & Privacy",
        "body": "Login pubblico e server personali; avatar e recupero sessione verificati; Allenamenti pubblici, privati o per amici reciproci; test delle API Rules; Schede personali senza limiti; esempi una sola volta; note e recuperi persistenti"
      },
      {
        "tag": "Fase 2",
        "title": "Flusso allenamento verificato",
        "body": "Selezione 0–5 minuti, salvata nelle schede; Pesi e ripetizioni suggeriti dallo storico, modificabili; Verifica su dispositivo di salvataggi, bozze e cambio account"
      },
      {
        "tag": "Fase 3",
        "title": "Social & Community",
        "body": "Profili ricercabili, follower e seguiti; amici = follow reciproco; Commenti e notifiche push dopo il consolidamento della privacy; Sfide tra amici e leaderboard con consenso"
      }
    ],
    "b2b": {
      "eyebrow": "// FORGE Local Business",
      "title": "Un modulo B2B per le palestre",
      "body": "L'architettura self-hosted permette a un proprietario di palestra di lanciare il backend su un mini-PC locale e creare un sistema gestionale e social privato per tutta la struttura.",
      "advantages": [
        {
          "title": "Software open source",
          "desc": "Codice MIT, senza abbonamento nell’app. Server, manutenzione e gestione hanno comunque dei costi."
        },
        {
          "title": "Community locale",
          "desc": "Gli iscritti sullo stesso server possono condividere gli allenamenti. Le classifiche interne della palestra sono un obiettivo futuro."
        },
        {
          "title": "Controllo locale dei dati",
          "desc": "La configurazione locale aiuta a controllare i dati. Privacy e conformità dipendono da accessi, consensi, backup e gestione del server."
        },
        {
          "title": "Offline / LAN First",
          "desc": "Se internet si interrompe, l'app continua a funzionare via Wi-Fi locale. Il funzionamento locale dipende dalla rete e dal server della palestra."
        }
      ],
      "future": [
        {
          "title": "Trainer Dashboard",
          "desc": "Interfaccia web dove i personal trainer assegnano schede e protocolli agli iscritti."
        },
        {
          "title": "Local TV Leaderboard",
          "desc": "Proietta su uno schermo della palestra la classifica dei PR sollevati in tempo reale."
        },
        {
          "title": "QR Code Onboarding",
          "desc": "Scansiona il QR in palestra e l'app si configura da sola con il server locale."
        },
        {
          "title": "Multi-tenant",
          "desc": "Più palestre sullo stesso server con isolamento completo dei dati."
        }
      ],
      "futureTitle": "Sviluppi futuri B2B",
      "status": "Gestione palestra · Obiettivo futuro",
      "note": "La community sul proprio server è già disponibile. Dashboard trainer, onboarding QR e gestione di più palestre sono sviluppi pianificati."
    },
    "backlog": {
      "eyebrow": "// Backlog",
      "title": "Idee & esperimenti",
      "items": [
        {
          "name": "AI form check",
          "desc": "Un esperimento per analizzare un breve video dell’esercizio e segnalare possibili errori di postura o movimento. Fattibilità e accuratezza sono ancora da valutare; la funzione non è disponibile nell’app."
        }
      ]
    },
    "closing": "Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda."
  },
  "featureDetails": [
    {
      "title": "Schede e serie",
      "desc": "Schede personali senza un limite imposto dall’app; serie normali, warm-up, drop set e cedimento. Conferma le modifiche alla scheda a fine sessione."
    },
    {
      "title": "Note automatiche",
      "desc": "Le note degli esercizi già presenti si salvano nella scheda senza conferma e tornano alla sessione successiva."
    },
    {
      "title": "Consigli a carosello",
      "desc": "Sopra ogni esercizio, gli step di esecuzione disponibili cambiano automaticamente durante l’allenamento. Istruzioni e immagini dipendono dal catalogo."
    },
    {
      "title": "Recupero 0–5 minuti",
      "desc": "Timer per esercizio con pausa, notifica di avanzamento e avviso sonoro. Suono e filtri confermati sul Samsung A15 del proprietario; comportamento su altri dispositivi da verificare."
    },
    {
      "title": "Statistiche e profilo",
      "desc": "Volume, calendario per periodo, progressione per esercizio, avatar, foto degli allenamenti e badge retroattivi."
    },
    {
      "title": "Social della tua community",
      "desc": "Ricerca, follow, like e feed tra utenti dello stesso server. Inviti condivisibili dal sito; nessuna federazione tra server."
    },
    {
      "title": "Catalogo con limiti dichiarati",
      "desc": "Provider ExerciseDB OSS, cache di metadati e URL GIF. Snapshot del server: 2.305 record, 1.500 con URL GIF e istruzioni; duplicati e disponibilità dei file da considerare."
    },
    {
      "title": "Privacy sul backend",
      "desc": "Visibilità separata per allenamenti e follower. Amici = follow reciproci accettati. Le migrazioni sono necessarie; un server self-hosted richiede manutenzione."
    },
    {
      "title": "Push social da verificare",
      "desc": "Client FCM e worker server implementati. La consegna reale con app chiusa non è ancora confermata; il timer locale funziona separatamente."
    },
    {
      "title": "Uso locale parziale",
      "desc": "Salvataggi e bozze locali e sincronizzazione degli allenamenti. La modalità completamente senza account/server e l’aggiornamento dall’app sono idee future."
    }
  ],
  "story": {
    "title": "Da progetto scolastico a beta aperta",
    "body": "FORGE è nato come progetto studentesco di informatica di Leonardo Lalli. È cresciuto attraverso prove in palestra, feedback e un uso esteso di strumenti AI per codice, interfacce, documentazione e verifiche. Il lavoro assistito dall’AI richiede revisione: la responsabilità delle scelte e delle release resta del proprietario. Oggi è una beta sperimentale, non un servizio professionale con disponibilità garantita."
  },
  "selfHostingGuide": {
    "title": "Una community sul tuo server",
    "intro": "Il server predefinito è gestito a casa dal proprietario e non ha disponibilità garantita. Un’istanza personale separa account e community; gli utenti di server diversi non si vedono.",
    "steps": [
      "Clona il repository e copia .env.example in .env.",
      "Scegli una versione PocketBase >= 0.23 e credenziali amministrative uniche.",
      "Avvia PocketBase, crea il superuser e inizializza le collezioni.",
      "Applica gli hook e le quattro migrazioni FORGE con backup.",
      "Configura il reverse proxy HTTPS e mantieni privata l’amministrazione.",
      "Inserisci l’indirizzo nell’app. Per push social servono Firebase e il worker separato."
    ],
    "note": "Docker è stato configurato per una versione esplicita del binario ufficiale, non latest. Il catalogo salva URL GIF, non i file; le foto e FCM possono usare servizi esterni. Segui la guida completa prima di usare dati reali.",
    "cta": "Guida completa su GitHub"
  },
  "comparison": {
    "title": "Dove FORGE si distingue",
    "intro": "Un confronto di funzioni documentate, non una classifica di affidabilità. Verificato il 13 ottobre 2026.",
    "items": [
      {
        "name": "Hevy",
        "text": "Hevy Free offre quattro routine; FORGE non impone un limite alle schede. Hevy ha un social più ampio e strumenti più maturi.",
        "url": "https://help.hevyapp.com/hc/en-us/articles/35119778922263-Hevy-Pro-Subscription-How-to-get-Pro-and-What-Does-It-Include"
      },
      {
        "name": "Strong",
        "text": "Strong Free offre tre template; FORGE offre schede senza limite e un backend personalizzabile. Strong ha integrazioni e supporto Apple Watch.",
        "url": "https://help.strongapp.io/article/105-about-templates"
      },
      {
        "name": "wger",
        "text": "Anche wger è open source e self-hosted, con nutrizione e misure. FORGE si concentra sul flusso Android e sulla community: nessuna superiorità generale misurata.",
        "url": "https://github.com/wger-project/wger"
      }
    ]
  },
  "gamification": {
    "eyebrow": "// Gamification",
    "title": "Ogni traguardo lascia il segno.",
    "intro": "Achievement, badge e livello trasformano lo storico in progressi visibili. Gli obiettivi supportati si sbloccano anche dagli allenamenti già completati.",
    "items": [
      {
        "title": "Achievement retroattivi",
        "text": "Primo allenamento, costanza, volume, durata e varietà: i criteri implementati usano lo storico dell’account."
      },
      {
        "title": "Il tuo livello",
        "text": "100 XP per badge sbloccato, un livello ogni 500 XP. Un riconoscimento dei traguardi, non una valutazione della forma fisica."
      },
      {
        "title": "Badge nel profilo",
        "text": "Scegli quali achievement sbloccati mostrare alla tua community, con le loro immagini."
      }
    ],
    "note": "Classifiche, alcuni criteri del catalogo e medaglie per i record delle singole serie non sono ancora implementati."
  },
  "screenshots": {
    "eyebrow": "// Dentro FORGE",
    "title": "L’app sul telefono.",
    "intro": "Allenamento: acquisizione recente fornita dal proprietario, con consigli a carosello, serie e note. Dashboard, statistiche e profilo sono schermate dell’archivio e possono differire dall’interfaccia attuale.",
    "labels": [
      "Dashboard",
      "Allenamento · acquisizione recente",
      "Statistiche",
      "Profilo"
    ]
  }
} as const
