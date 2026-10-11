# Roadmap FORGE

Priorità concordate l’11 ottobre 2026: stabilità e privacy prima delle nuove integrazioni. Le versioni indicano obiettivi, senza date promesse. Una funzione implementata su develop non è automaticamente rilasciata o verificata sul server pubblico.

## v0.9.0-beta — Stabilità & Privacy

- **Accesso affidabile**: Login pubblico e server personali; avatar e recupero sessione verificati.
- **Privacy sul server**: Allenamenti pubblici, privati o per amici reciproci; test delle API Rules.
- **Schede per account**: Schede personali senza limiti; esempi una sola volta; note e recuperi persistenti.
- **Achievement retroattivi**: Ricalcolo dello storico e tre badge scelti nel profilo.
- **Regressioni e grafici**: Grafico volume corretto; build Android e test automatici.

## v1.0.0 — Flusso allenamento verificato

- **Recuperi per esercizio**: Selezione 0–5 minuti, salvata nelle schede.
- **Sessione precedente**: Pesi e ripetizioni suggeriti dallo storico, modificabili.
- **Offline e sincronizzazione**: Verifica su dispositivo di salvataggi, bozze e cambio account.
- **Catalogo esercizi**: Conteggio reale, GIF, istruzioni e paginazione completa.
- **Release Android**: Build APK riproducibile e validazione prima della release.

## v1.1.0 — Social & Community

- **Profili e follow**: Profili ricercabili, follower e seguiti; amici = follow reciproco.
- **Commenti e notifiche**: Commenti e notifiche push dopo il consolidamento della privacy.
- **Sfide e classifiche**: Sfide tra amici e leaderboard con consenso.
- **Condivisione**: Riepiloghi condivisibili senza esporre contenuti privati.

## v1.2.0 — Analytics & Localizzazione

- **Analytics avanzate**: 1RM, volume per muscolo, calendario e storico.
- **Body tracking**: Misure e peso con scelte di riservatezza.
- **Lingue e accessibilità**: Traduzioni complete e test di contrasto e dimensioni testo.
- **Report e backup**: Export PDF e backup con ripristino verificato.

## v1.3.0 — Health & Wearables

- **Widget e Wear OS**: Accesso rapido e companion per timer e serie.
- **Integrazioni salute**: Valutazione delle API supportate e autorizzazioni esplicite.
- **Sensori**: Battito e passi dove supportati dal dispositivo.

## v1.4.0 — Infrastruttura & B2B

- **Origine protetta**: Proxy o tunnel con dominio dedicato; IP domestico fuori dal DNS pubblico.
- **Distribuzione**: Play Store e gestione chiavi di produzione.
- **Dashboard e API**: Web dashboard, API documentata e profili server.
- **Palestre**: Trainer dashboard, QR onboarding e isolamento tra palestre.

## Stato attuale

Login pubblico e nuova schermata confermati sul dispositivo dall’utente. Profilo, schede, recuperi, badge e grafico sono in verifica su develop; privacy live richiede la migrazione PocketBase.

AI coaching, marketplace e nuovi provider esercizi restano esperimenti da valutare dopo affidabilità, dati e licenze.
