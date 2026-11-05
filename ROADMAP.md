# Roadmap FORGE

La versione corrente approvata è **0.8.5-beta, build 89**, promossa su `main` il 05/11/2026. Le nuove richieste vengono sviluppate su `develop`. Le denominazioni 0.8.6/0.8.7 degli APK locali precedenti non identificano nuove release da pubblicare.

Ogni modifica segue la [regola di tracciamento](docs/roadmap-policy.md): prima data di implementazione, riferimento reale e storico degli aggiornamenti.

## In verifica su develop — build 90

Implementato il **05/11/2026** · riferimento `develop · 0.8.5-beta-develop · build 90`. Tutti gli elementi seguenti sono candidate da provare sul telefono; non modificano la release pubblica build 89.

- Eliminazione delle singole serie con rinumerazione e conteggi aggiornati.
- Filtro avambracci basato sui muscoli primari.
- Timer continuo quando l’app va in background o l’allenamento viene minimizzato; pausa manuale rispettata.
- CSV fino a 5 MiB senza limite di righe, lettura bounded e gruppi per data/nome.
- Salvataggio del solo nome della scheda senza serie confermate; popup con le sole differenze, senza workout fittizi.
- Navigazione dei mesi in Stats con calendario, grafico e aggregati riferiti al mese selezionato.

## Disponibile nella 0.8.5-beta — approvata il 05/11/2026

| Elemento | Implementazione | Ultimo aggiornamento | Riferimento / stato |
| --- | --- | --- | --- |
| Correzione avvio Android dopo il logo | Implementato il 04/11/2026 | 04/11/2026 | v0.8.5-beta · build 89, build 89; firma/callback verificati; versione approvata dal proprietario, limiti del test emulatore documentati |
| Unità kg / libbre in tutta l’app | Implementato il 26/10/2026 | 26/10/2026 | v0.8.5-beta · build 89, approvata dal proprietario |
| Allenamento minimizzabile e recuperabile | Implementato il 16/10/2026 | 26/10/2026 | v0.8.5-beta · build 89; barra con recupero |
| Conferma X / Indietro | Implementato il 16/10/2026 | 26/10/2026 | v0.8.5-beta · build 89; popup segue il tema |
| Note multilinea ed esercizi vuoti esclusi | Implementato il 16/10/2026 | 16/10/2026 | v0.8.5-beta · build 89 |
| Calendario Stats stabile e icona FORGE | Implementato il 16/10/2026 | 16/10/2026 | v0.8.5-beta · build 89 |
| Sincronizzazione schede account | Implementato il 26/10/2026 | 26/10/2026 | v0.8.5-beta · build 89; richiede aggiornamento server, non ancora attivo sul server del proprietario |

## Account e dati

- **Social login**: Google/Microsoft/Apple sign-in oltre a email/password
- **Attivazione della sincronizzazione schede**: codice implementato il 26/10/2026 su develop; aggiornare PocketBase prima della prima copia remota. Vedi [guida](docs/account-plan-sync.md).

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
