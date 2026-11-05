# Primo messaggio da incollare nella chat web del progetto

Voglio che tu conosca FORGE prima di propormi un’ottimizzazione. Leggi il contesto e i sorgenti caricati nel progetto. L’app attuale mi piace: preserva prodotto, interfaccia e dati, migliorando affidabilità, velocità, fluidità e manutenibilità dove esiste un problema concreto.

La release approvata è 0.8.5-beta/build 89 su main, commit 9cab71b. Le nuove funzioni sono su develop/build 90 e non sono una nuova release pubblica. Lavora su questa distinzione e non proporre di pubblicare automaticamente develop. Il mio telefono è un Samsung A15 4G; non inventare la versione Android precisa o prestazioni misurate.

Procedi così:

1. Ricostruisci in modo sintetico architettura e flussi reali: avvio/auth/Shell, DI e lifetime, esercizi/schede/sessioni, timer/background/ripristino, SQLite/PocketBase/sync per account, unità, CSV, statistiche, backend e build/release. Cita i file di riferimento. Dichiara i limiti dello snapshot e ciò che non hai potuto leggere.
2. Identifica al massimo otto problemi o ipotesi di ottimizzazione. Per ciascuno indica file/metodo, evidenza, conseguenza per l’utente, priorità, costo/rischio e come verificare il beneficio. Se non hai dati di profiling, scrivi “ipotesi da misurare”, non presentarla come collo di bottiglia accertato.
3. Esamina in particolare cold start, caricamenti e ricerca catalogo, lavoro sul thread UI, CollectionView/media, lifetime del singleton ActiveWorkoutViewModel e sottoscrizioni, debounce/salvataggi, serializzazione, rete/sync e import CSV. Considera errori, duplicati, isolamento account e consumo batteria. Non modificare l’interfaccia soltanto per gusto personale.
4. Ordina gli interventi in tre piccole iterazioni autonome. Scegli la prima per rapporto impatto/rischio e motiva la scelta. Distingui bug da ottimizzazioni e da funzionalità nuove. Non includere automaticamente tutte le idee nello stesso lavoro.
5. Genera **un prompt completo per Codex**, pronto da incollare, per eseguire soltanto la prima iterazione. Deve contenere: problema/obiettivo, branch develop, file da leggere e modificare, vincoli, casi normali/edge case, criteri di accettazione misurabili, test appropriati, documentazione/roadmap e consegna finale. Evita hash inventati e vieta merge/pubblicazione/server live non richiesti.

Nel prompt per Codex includi questi vincoli: preservare schema/dati e isolamento account; kg canonici e conversioni coerenti; timer continuo al background/minimizzazione salvo pausa manuale; salvataggio del solo nome senza serie confermate e senza storico fittizio; riepilogo delle sole differenze; statistiche del mese selezionato con entrambi i limiti; CSV 5 MiB senza limite di righe e lettura bounded. Non introdurre dipendenze o refactor generali non necessari.

Se l’iterazione modifica l’app e richiede un APK, chiedi una Release firmata con lo stesso certificato e com.leoforge.forge, versionCode superiore a quelli già distribuiti (verificarli, non riutilizzare 89/90 a memoria), controllo JNI ARM64/x64 sul pacchetto e confronto firma/versione. Non chiedere o copiare segreti nel prompt. Separare ciò che è testato dal collaudo sul mio telefono; non chiamare “passato” un controllo non eseguito.

Concludi con il prompt Codex e una breve lista dei dati di misura realmente mancanti. Fammi domande soltanto se impediscono di scegliere una prima iterazione utile; altrimenti usa assunzioni esplicite e verificabili.
