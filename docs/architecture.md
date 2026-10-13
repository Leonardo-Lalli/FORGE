# Architettura FORGE

Codice di riferimento della 0.8.5 beta, 13 ottobre 2026: experiments promosso a main su richiesta del proprietario. Il tag v0.8.5-beta identifica la distribuzione; pubblicazione e deployment hanno verifiche separate.

## Componenti

- .NET 10 MAUI, Android API 23+, Shell e MVVM CommunityToolkit.Mvvm, DI e binding XAML compilati.
- SQLite: allenamenti/bozze, schede personali associate all’account, cache esercizi e achievement.
- PocketBase selezionato nelle impostazioni, con istanza pubblica predefinita. Account e social indipendenti per istanza.
- ExerciseDB OSS: metadati, istruzioni e URL media; i file GIF restano sul provider.
- Firebase FCM opzionale: registrazione del client, coda privata server e worker Python con chiave Admin solo sul server. Timer Android locale indipendente da FCM.
- Sito Next.js esportato staticamente, italiano/inglese, doppio tema. Download risolto dalle release GitHub (incluse beta), con fallback all’elenco release.

## Salvataggio e interfaccia

ActiveWorkoutViewModel salva l’allenamento locale prima dell’upload. WorkoutSavedMessage aggiorna profilo e statistiche; WorkoutHistory unisce lo storico remoto già disponibile ai salvataggi locali. SyncService gestisce allenamenti pendenti, senza promessa di sincronizzare ogni entità tra dispositivi.

PlanService gestisce schede SQLite. Le note vengono salvate separatamente dopo 400 ms di pausa e attese prima di minimizzare/terminare; scritture serializzate e limitate alle note degli esercizi corrispondenti. PlanSessionChanges confronta nome, struttura, serie e recuperi, ignorando note, completamento e suggerimenti. Aggiunte/sostituzioni richiedono conferma; la copia della scheda azzera lo stato della sessione.

## Privacy e community

API Rules e hook FORGE sono necessari oltre ai filtri UI. Allenamenti ed elenchi follower hanno visibilità separata: pubblico per utenti autenticati dell’istanza, privato, amici reciproci accettati. Like e aggiornamenti consentiti usano endpoint dedicati. Nessuna federazione, gestione trainer o separazione di più palestre sul medesimo server è implementata.

FeedViewModel mostra lo stato vuoto dopo il caricamento. HasOtherCommunityMembersAsync distingue zero utenti visibili da errore/stato sconosciuto. CommunityInvite apre la condivisione nativa al sito, includendo istruzioni per l’istanza personale senza credenziali. Non registra automaticamente utenti o follow.

Le regole aggiornate sono nei sorgenti tools/pocketbase/pb_migrations e nei relativi hook. Vedi [migrazioni](pocketbase-profile-migration.md); non copiare vecchi esempi di regole di produzione.

## Backend e distribuzione

Compose costruisce il binario ufficiale PocketBase della versione esplicitamente scelta, amd64/arm64; porta su loopback, init esplicito e migrazioni con backup. Worker push in profilo opzionale. Installer LXC differenziano /opt/forge-pocketbase (servizio forge-pocketbase) da Community Scripts /opt/pocketbase (servizio pocketbase).

Il workflow release richiede configurazione Firebase client e firma di produzione nei secret; prepara una bozza APK + archivio server. Il server domestico non garantisce disponibilità. Vedi [installazione](self-hosting.md), [push](push-notifications.md) e [preparazione beta](release-readiness.md).

Modalità senza account, sincronizzazione completa, gestione palestre e aggiornamenti nell’app sono proposte future, separate dall’implementazione attuale. [Roadmap](../ROADMAP.md).
