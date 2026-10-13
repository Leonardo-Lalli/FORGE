# Roadmap FORGE

Tutti gli obiettivi del progetto, senza date o versioni future. Le priorità possono cambiare con i feedback; data e versione compariranno solo dopo il rilascio.

Alcuni obiettivi estendono funzioni già presenti. Le idee sono da valutare, non promesse di consegna.

## Stabilità e privacy

- **Accesso e salvataggi affidabili**: Consolidare login, avatar, bozze, recupero sessioni, schede per account e sincronizzazione anche senza rete.
- **Privacy verificata sul server**: Verificare allenamenti e follower pubblici, privati o per amici reciproci, oltre a cancellazione account e isolamento dei dati.
- **Catalogo trasparente**: Controllare duplicati, istruzioni, GIF raggiungibili e cache condivisa, con conteggi reali.
- **Backup completo e ripristino**: Conservare e ripristinare schede, note, foto, serie e carichi: il CSV attuale non è un backup completo.
- **Origine protetta**: Valutare proxy o tunnel con dominio dedicato per non esporre l’IP domestico nel DNS pubblico.
- **Social login**: Google/Apple sign-in oltre a email/password

## QoL & Usabilità

- **Incremento rapido pesi (+/-)**: Pulsanti +2.5kg accanto al campo peso. Press-and-hold = +5kg
- **Copia set precedente**: Tasto "Duplica" che clona kg×reps nella serie successiva
- **Calcolatore 1RM**: Tap su un top lift → mostra massimale stimato (formula Epley)
- **Timer a voce**: TTS Android: "Serie completata, 90 secondi di pausa"
- **Filtro ricerca esercizi migliorato**: Filtri muscolo e attrezzatura già presenti: consolidare copertura del catalogo, paginazione e risultati.
- **Voice input**: "Cento chili, otto rep" invece di digitare
- **Video esercizi**: Video YouTube integrati oltre alle GIF

## Release Stabile

- **Notifiche push FCM**: Client e worker implementati: verificare consegna reale con app chiusa per follow, like e richieste.
- **Localizzazione completa**: File .resx per IT/EN/ES/DE/ZH — tutte le stringhe tradotte
- **Body tracking**: Registra peso corporeo, misure, %BF. Grafico nel tempo
- **Leaderboard settimanale**: Classifica volume tra amici, reset ogni lunedì
- **Auto-build APK nelle Release**: Workflow firmato manuale già presente: verificare la build CI e valutare l’automazione delle release.

## Social & Community

- **Commenti ai workout**: Gli amici possono commentare i tuoi allenamenti nel feed
- **Sfide 1-vs-1**: "Sfida Marco a volume totale questa settimana" → notifica + classifica
- **Condivisione workout**: Share sheet Android: immagine riassuntiva dell'allenamento
- **Badge stagionali**: Achievement limitati nel tempo (es. "Allenati a Natale")
- **Storie workout**: Evolvere gli allenamenti pubblici e le foto in storie temporanee, rispettando la visibilità scelta.
- **Mappe palestre**: Mappa delle palestre vicine con recensioni community
- **Gamification 2.0**: Livelli e XP esistono già: valutare stagioni competitive e ricompense aggiuntive.
- **Ricompense per record delle serie**: Medagliette per record personali di peso, volume o ripetizioni: solo idea futura, ancora da implementare.

## Health & Wear OS

- **Widget Android**: "START WORKOUT" direttamente dalla home screen del telefono
- **Wear OS companion**: Timer rest + tracking serie dallo smartwatch
- **Integrazioni salute**: Valutare API salute supportate per sincronizzare allenamenti e dati autorizzati.
- **Heart rate display**: Mostra battito cardiaco durante l'allenamento (se supportato)
- **Contatore passi**: Widget passi giornalieri nella dashboard
- **Apple Watch**: Supporto iOS + companion watchOS

## Advanced Features

- **PDF report**: Export report allenamento con grafici, tabelle e foto
- **Allenamenti predefiniti**: Ampliare gli esempi già disponibili con template Push/Pull/Legs e Upper/Lower, senza rigenerarli se eliminati.
- **Progressive overload tracker**: L'app suggerisce quanto aumentare rispetto alla sessione precedente
- **Grafici avanzati**: Volume per muscolo nel tempo, PR timeline, frequenza settimanale
- **Modifica workout post-salvataggio**: Correggi un peso sbagliato dopo aver salvato
- **Ricerca workout nello storico**: Cerca per nome esercizio, data, volume
- **Nutrizione tracker**: Calorie e macro insieme al tracking degli allenamenti

## Platform & Infrastruttura

- **Google Play Store**: Pubblicazione sullo store usando la firma stabile già creata, dopo i controlli richiesti.
- **Backup automatico**: Export automatico settimanale dei dati su Google Drive
- **SQLite encryption**: Database cifrato con SQLCipher
- **Multi-server switch**: Profili multipli: switch tra server self-hosted diversi
- **Web dashboard**: Interfaccia web per visualizzare i dati da PC
- **REST API pubblica**: API documentata per integrazioni di terze parti
- **Modalità completamente locale**: Allenarsi senza account o server, con collegamento e sincronizzazione cloud facoltativi.
- **Aggiornamenti dall’app**: Scaricare e avviare l’installazione di aggiornamenti firmati senza cercare manualmente l’APK.

## Palestre e trainer

- **Trainer Dashboard**: Interfaccia web dove i personal trainer assegnano schede e protocolli agli iscritti.
- **Local TV Leaderboard**: Proietta su uno schermo della palestra la classifica dei PR sollevati in tempo reale.
- **QR Code Onboarding**: Scansiona il QR in palestra e l'app si configura da sola con il server locale.
- **Multi-tenant**: Più palestre sullo stesso server con isolamento completo dei dati.

## Idee ed esperimenti

- **AI form check**: Analizzare un breve video dell’esercizio per segnalare possibili errori di postura o movimento. Esperimento da valutare per fattibilità e accuratezza; non disponibile nell’app.

## Già rilasciato

### 0.8.5 beta · 13 ottobre 2026

- Schede personali, esempi una sola volta e note automatiche degli esercizi già presenti.
- Recuperi per esercizio, notifica del timer e valori della sessione precedente.
- Consigli di esecuzione che cambiano automaticamente in un carosello di step.
- Achievement retroattivi supportati dallo storico, livello XP e badge nel profilo.
- Profili, follow, like, foto e inviti tra utenti dello stesso server.
- Visibilità di allenamenti e follower e cancellazione account con backend aggiornato.
- Calendario, volume e progressione per esercizio; firma stabile dell’APK.

Client FCM e worker push sono implementati, ma la consegna reale con app chiusa resta da verificare. La community è separata per server; nessuna federazione. Le prove e i limiti della beta sono nel README.
