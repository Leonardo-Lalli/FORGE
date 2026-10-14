# Roadmap FORGE

Solo le funzioni che dobbiamo ancora aggiungere. Ciò che è già disponibile è descritto nella pagina Funzionalità; qui non fissiamo date o versioni future.

## Account e dati

- **Social login**: Google/Apple sign-in oltre a email/password
- **Sincronizzazione delle schede**: Ritrovare schede e note su più dispositivi collegati allo stesso account.

## Evoluzione dell’allenamento

- **Modalità completamente locale**: Allenarsi senza account o server, con collegamento cloud facoltativo.
- **Voice input**: "Cento chili, otto rep" invece di digitare
- **Video esercizi**: Video YouTube integrati oltre alle GIF

## Social & Community

- **Commenti agli allenamenti**: Conversazioni sotto gli allenamenti condivisi, nel rispetto della visibilità scelta.
- **Sfide e classifiche**: Sfide tra amici e leaderboard con consenso
- **Riepiloghi condivisibili**: Generare un’immagine riepilogativa dell’allenamento da condividere fuori dall’app.
- **Mappe palestre**: Mappa delle palestre vicine con recensioni community
- **Gamification 2.0**: Estendere livelli e XP già presenti con stagioni competitive e nuove ricompense.
- **Ricompense per record delle serie**: Medagliette per record personali di peso, volume o ripetizioni; idea ancora da implementare.

## Analytics & Localizzazione

- **Analytics avanzate**: Massimale stimato 1RM, volume per muscolo e nuovi report di progressione.
- **Body tracking**: Misure e peso con scelte di riservatezza
- **Lingue e accessibilità**: Traduzioni complete e test di contrasto e dimensioni testo
- **Report e backup**: Export PDF e backup con ripristino verificato
- **Nutrizione tracker**: Calorie e macro insieme al tracking degli allenamenti

## Health & Wearables

- **Widget e Wear OS**: Accesso rapido e companion per timer e serie
- **Integrazioni salute**: Valutazione delle API supportate e autorizzazioni esplicite
- **Sensori**: Battito e passi dove supportati dal dispositivo
- **Apple Watch**: Supporto iOS + companion watchOS

## Infrastruttura & B2B

- **Origine protetta**: Proxy o tunnel con dominio dedicato; IP domestico fuori dal DNS pubblico
- **Pubblicazione sul Play Store**: Portare la beta sullo store dopo i controlli richiesti, usando la firma stabile già presente.
- **Dashboard e API**: Web dashboard, API documentata e profili server
- **Palestre**: Trainer dashboard, QR onboarding e isolamento tra palestre
- **Aggiornamenti dall’app**: Scaricare e avviare l’installazione di APK firmati senza cercarli manualmente su GitHub.
- **Proxmox Community Scripts**: Adattare l’installer agli standard del progetto, provarlo su Proxmox e proporlo per revisione. Non ancora integrato ufficialmente.

## Idee ed esperimenti

- **AI form check**: Un esperimento per analizzare un breve video dell’esercizio e segnalare possibili errori di postura o movimento. Fattibilità e accuratezza sono ancora da valutare; la funzione non è disponibile nell’app.

Le funzioni già implementate e i relativi limiti sono nel [README](README.md). L’integrazione con Proxmox Community Scripts è un obiettivo: sono necessarie prove reali e revisione secondo [le linee guida del progetto](https://github.com/community-scripts/ProxmoxVE/blob/main/CONTRIBUTING.md).
