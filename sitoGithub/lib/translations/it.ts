export const it = {
  "nav": {
    "features": "Funzionalità",
    "selfHosting": "Self-Hosting",
    "docs": "Documentazione",
    "roadmap": "Roadmap",
    "download": "Download",
    "backHome": "Torna alla home"
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
    "subtitle": "Tutti gli obiettivi del progetto, senza date o versioni future. Le priorità possono cambiare con i feedback; data e versione compariranno solo dopo il rilascio.",
    "plannedTitle": "Obiettivi da sviluppare e consolidare",
    "plannedNote": "Alcuni obiettivi estendono funzioni già presenti. Le idee sono da valutare, non promesse di consegna.",
    "groups": [
      {
        "theme": "Stabilità e privacy",
        "features": [
          {
            "name": "Accesso e salvataggi affidabili",
            "desc": "Consolidare login, avatar, bozze, recupero sessioni, schede per account e sincronizzazione anche senza rete."
          },
          {
            "name": "Privacy verificata sul server",
            "desc": "Verificare allenamenti e follower pubblici, privati o per amici reciproci, oltre a cancellazione account e isolamento dei dati."
          },
          {
            "name": "Catalogo trasparente",
            "desc": "Controllare duplicati, istruzioni, GIF raggiungibili e cache condivisa, con conteggi reali."
          },
          {
            "name": "Backup completo e ripristino",
            "desc": "Conservare e ripristinare schede, note, foto, serie e carichi: il CSV attuale non è un backup completo."
          },
          {
            "name": "Origine protetta",
            "desc": "Valutare proxy o tunnel con dominio dedicato per non esporre l’IP domestico nel DNS pubblico."
          }
        ]
      },
      {
        "theme": "QoL & Usabilità",
        "features": [
          {
            "name": "Incremento rapido pesi (+/-)",
            "desc": "Pulsanti +2.5kg accanto al campo peso. Press-and-hold = +5kg"
          },
          {
            "name": "Copia set precedente",
            "desc": "Tasto \"Duplica\" che clona kg×reps nella serie successiva"
          },
          {
            "name": "Calcolatore 1RM",
            "desc": "Tap su un top lift → mostra massimale stimato (formula Epley)"
          },
          {
            "name": "Timer a voce",
            "desc": "TTS Android: \"Serie completata, 90 secondi di pausa\""
          },
          {
            "name": "Filtro ricerca esercizi migliorato",
            "desc": "Filtri muscolo e attrezzatura già presenti: consolidare copertura del catalogo, paginazione e risultati."
          }
        ]
      },
      {
        "theme": "Release Stabile",
        "features": [
          {
            "name": "Notifiche push FCM",
            "desc": "Client e worker implementati: verificare consegna reale con app chiusa per follow, like e richieste."
          },
          {
            "name": "Localizzazione completa",
            "desc": "File .resx per IT/EN/ES/DE/ZH — tutte le stringhe tradotte"
          },
          {
            "name": "Body tracking",
            "desc": "Registra peso corporeo, misure, %BF. Grafico nel tempo"
          },
          {
            "name": "Leaderboard settimanale",
            "desc": "Classifica volume tra amici, reset ogni lunedì"
          },
          {
            "name": "Auto-build APK nelle Release",
            "desc": "Workflow firmato manuale già presente: verificare la build CI e valutare l’automazione delle release."
          }
        ]
      },
      {
        "theme": "Social & Community",
        "features": [
          {
            "name": "Commenti ai workout",
            "desc": "Gli amici possono commentare i tuoi allenamenti nel feed"
          },
          {
            "name": "Sfide 1-vs-1",
            "desc": "\"Sfida Marco a volume totale questa settimana\" → notifica + classifica"
          },
          {
            "name": "Condivisione workout",
            "desc": "Share sheet Android: immagine riassuntiva dell'allenamento"
          },
          {
            "name": "Badge stagionali",
            "desc": "Achievement limitati nel tempo (es. \"Allenati a Natale\")"
          },
          {
            "name": "Storie workout",
            "desc": "Evolvere gli allenamenti pubblici e le foto in storie temporanee, rispettando la visibilità scelta."
          }
        ]
      },
      {
        "theme": "Health & Wear OS",
        "features": [
          {
            "name": "Widget Android",
            "desc": "\"START WORKOUT\" direttamente dalla home screen del telefono"
          },
          {
            "name": "Wear OS companion",
            "desc": "Timer rest + tracking serie dallo smartwatch"
          },
          {
            "name": "Integrazioni salute",
            "desc": "Valutare API salute supportate per sincronizzare allenamenti e dati autorizzati."
          },
          {
            "name": "Heart rate display",
            "desc": "Mostra battito cardiaco durante l'allenamento (se supportato)"
          },
          {
            "name": "Contatore passi",
            "desc": "Widget passi giornalieri nella dashboard"
          }
        ]
      },
      {
        "theme": "Advanced Features",
        "features": [
          {
            "name": "PDF report",
            "desc": "Export report allenamento con grafici, tabelle e foto"
          },
          {
            "name": "Allenamenti predefiniti",
            "desc": "Ampliare gli esempi già disponibili con template Push/Pull/Legs e Upper/Lower, senza rigenerarli se eliminati."
          },
          {
            "name": "Progressive overload tracker",
            "desc": "L'app suggerisce quanto aumentare rispetto alla sessione precedente"
          },
          {
            "name": "Grafici avanzati",
            "desc": "Volume per muscolo nel tempo, PR timeline, frequenza settimanale"
          },
          {
            "name": "Modifica workout post-salvataggio",
            "desc": "Correggi un peso sbagliato dopo aver salvato"
          },
          {
            "name": "Ricerca workout nello storico",
            "desc": "Cerca per nome esercizio, data, volume"
          }
        ]
      },
      {
        "theme": "Platform & Infrastruttura",
        "features": [
          {
            "name": "Google Play Store",
            "desc": "Pubblicazione sullo store usando la firma stabile già creata, dopo i controlli richiesti."
          },
          {
            "name": "Backup automatico",
            "desc": "Export automatico settimanale dei dati su Google Drive"
          },
          {
            "name": "SQLite encryption",
            "desc": "Database cifrato con SQLCipher"
          },
          {
            "name": "Multi-server switch",
            "desc": "Profili multipli: switch tra server self-hosted diversi"
          },
          {
            "name": "Web dashboard",
            "desc": "Interfaccia web per visualizzare i dati da PC"
          },
          {
            "name": "REST API pubblica",
            "desc": "API documentata per integrazioni di terze parti"
          },
          {
            "name": "Modalità completamente locale",
            "desc": "Allenarsi senza account o server, con collegamento e sincronizzazione cloud facoltativi."
          },
          {
            "name": "Aggiornamenti dall’app",
            "desc": "Scaricare e avviare l’installazione di aggiornamenti firmati senza cercare manualmente l’APK."
          }
        ]
      },
      {
        "theme": "Palestre e trainer",
        "features": [
          {
            "name": "Trainer Dashboard",
            "desc": "Interfaccia web dove i personal trainer assegnano schede e protocolli agli iscritti."
          },
          {
            "name": "Local TV Leaderboard",
            "desc": "Proietta su uno schermo della palestra la classifica dei PR sollevati in tempo reale."
          },
          {
            "name": "QR Code Onboarding",
            "desc": "Scansiona il QR in palestra e l'app si configura da sola con il server locale."
          },
          {
            "name": "Multi-tenant",
            "desc": "Più palestre sullo stesso server con isolamento completo dei dati."
          }
        ]
      },
      {
        "theme": "Idee ed esperimenti",
        "features": [
          {
            "name": "AI form check",
            "desc": "Analisi posturale via fotocamera durante l'esercizio"
          },
          {
            "name": "Voice input",
            "desc": "\"Cento chili, otto rep\" invece di digitare"
          },
          {
            "name": "Social login",
            "desc": "Google/Apple sign-in oltre a email/password"
          },
          {
            "name": "Mappe palestre",
            "desc": "Mappa delle palestre vicine con recensioni community"
          },
          {
            "name": "Marketplace piani",
            "desc": "Utenti condividono e vendono piani di allenamento"
          },
          {
            "name": "Nutrizione tracker",
            "desc": "Calorie e macro insieme al tracking degli allenamenti"
          },
          {
            "name": "Apple Watch",
            "desc": "Supporto iOS + companion watchOS"
          },
          {
            "name": "Gamification 2.0",
            "desc": "Livelli e XP esistono già: valutare stagioni competitive e ricompense aggiuntive."
          },
          {
            "name": "Coaching AI",
            "desc": "Suggerimenti automatici basati sui dati storici"
          },
          {
            "name": "Video esercizi",
            "desc": "Video YouTube integrati oltre alle GIF"
          },
          {
            "name": "Ricompense per record delle serie",
            "desc": "Medagliette per record personali di peso, volume o ripetizioni: solo idea futura, ancora da implementare."
          }
        ]
      }
    ],
    "completedTitle": "Già rilasciato",
    "completedNote": "Funzioni presenti nella beta; verifiche e limiti sono descritti nel README.",
    "completed": [
      {
        "version": "0.8.5 beta",
        "date": "13 ottobre 2026",
        "features": [
          "Schede personali, esempi una sola volta e note automatiche degli esercizi già presenti.",
          "Recuperi per esercizio, notifica del timer e valori della sessione precedente.",
          "Consigli di esecuzione che cambiano automaticamente in un carosello di step.",
          "Achievement retroattivi supportati dallo storico, livello XP e badge nel profilo.",
          "Profili, follow, like, foto e inviti tra utenti dello stesso server.",
          "Visibilità di allenamenti e follower e cancellazione account con backend aggiornato.",
          "Calendario, volume e progressione per esercizio; firma stabile dell’APK."
        ]
      }
    ]
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
