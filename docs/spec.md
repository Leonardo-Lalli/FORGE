# Specifica di Progetto - GymTracker Mobile

## 1. Visione e contesto

### Problema da risolvere

Chi va in palestra fatica a tenere traccia dei progressi, non ricorda i pesi usati la volta precedente, non sa quali esercizi esistano per un dato muscolo e manca un modo semplice per monitorare l'evoluzione del proprio fisico. Inoltre, allenarsi da soli riduce la motivazione: la competizione sana con gli amici spinge a migliorarsi, ma manca uno strumento semplice che unisca tracking e social in un'unica app.

### Obiettivo del progetto

Realizzare una applicazione `.NET MAUI` Android-first per:
- esplorare un catalogo di esercizi da **ExerciseDB API** (RapidAPI) con ricerca per nome, gruppo muscolare e attrezzatura;
- registrare allenamenti completi con serie, ripetizioni e peso, salvandoli localmente e sincronizzandoli con il backend;
- tracciare il peso corporeo e le misure (circonferenze) nel tempo;
- visualizzare statistiche di progresso e grafici;
- connettersi con amici, confrontare statistiche e competere via **leaderboard**, **feed** e **streak settimanali**;
- consultare piani di allenamento base predefiniti per principianti.

### Utenti target

- principianti che iniziano ad allenarsi in palestra e necessitano di struttura;
- appassionati di fitness che vogliono tracciare i progressi nel tempo;
- gruppi di amici che vogliono motivarsi a vicenda con competizione sana.

## 2. Ambito MVP

### Flusso principale da supportare

1. L'utente apre l'app e atterra sulla **Dashboard** con statistiche rapide e accesso alle funzioni principali.
2. L'utente esplora il **Catalogo esercizi** da ExerciseDB (API remota) con ricerca e filtri. Gli esercizi usati di recente sono disponibili offline via cache SQLite.
3. L'utente avvia un **Nuovo Allenamento**, seleziona esercizi, registra serie/peso/ripetizioni e salva.
4. L'utente registra il **Peso corporeo** e le **misure** (petto, vita, braccia, gambe) dopo l'allenamento.
5. L'utente visualizza le **Statistiche** personali: peso max per esercizio, volume, andamento peso corporeo.
6. L'utente aggiunge **Amici** (via Firebase backend), vede il loro feed di allenamenti e confronta le proprie statistiche in una **Leaderboard** e tramite **Streak** settimanali.

### Funzionalità obbligatorie

- catalogo esercizi da ExerciseDB API con ricerca e filtri (gruppo muscolare, attrezzatura);
- cache locale SQLite per esercizi usati di recente (offline-first);
- registrazione allenamento con data, esercizi, serie, ripetizioni e peso;
- storico allenamenti consultabile con dettaglio;
- tracciamento peso corporeo e misure corporee (petto, vita, fianchi, braccia, gambe);
- dashboard con statistiche aggregate e grafici di progresso;
- autenticazione utente via Firebase (email/password);
- gestione amici: ricerca, richieste, accettazione/rifiuto, lista;
- feed allenamenti amici;
- leaderboard settimanale per volume di allenamento;
- streak settimanale (giorni consecutivi di allenamento);
- confronto diretto statistiche con un amico;
- piani di allenamento base predefiniti per principianti (almeno 3);
- gestione esplicita di loading state, error state, empty state e success state su ogni schermata;
- UI progettata con Stitch (Google) — progetto "Iron Rank Fitness Social", tema "Performance Minimalist" (dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter).

### Funzionalità opzionali future

- post-MVP 1: creazione piani personalizzati e salvataggio piani utente;
- post-MVP 2: timer per pause tra serie con notifiche;
- post-MVP 3: obiettivi settimanali personalizzati e notifiche reminder;
- post-MVP 4: export dati in formato CSV/PDF;
- post-MVP 5: foto progresso fisico con confronto temporale;
- post-MVP 6: integrazione con Google Fit / Apple Health.

### Non-obiettivi

- integrazione con dispositivi smart (orologi, bilance, fasce cardio);
- pagamento o abbonamenti premium;
- recensioni o rating sociali pubblici;
- chat o messaggistica tra amici;
- supporto iOS completo nel primo rilascio (Android-first).

## 3. Scenari d'uso principali

### Scenario 1: Primo allenamento

1. Marco apre l'app, si registra con email e password.
2. Dalla Dashboard, tocca "Nuovo Allenamento".
3. Cerca "panca" nel catalogo esercizi (fetch da ExerciseDB).
4. Seleziona "Barbell Bench Press", aggiunge 4 serie: 60kg x 10, 70kg x 8, 80kg x 6, 85kg x 4.
5. Aggiunge altri 3 esercizi e salva l'allenamento.
6. Inserisce il peso corporeo (78kg) e la misura del petto (102cm).
7. Torna alla Dashboard e vede le statistiche aggiornate.

### Scenario 2: Competizione tra amici

1. Marco aggiunge Luca come amico cercandolo per username.
2. Luca accetta la richiesta.
3. Marco vede nel Feed che Luca ha fatto 5 allenamenti questa settimana.
4. Nella Leaderboard, Marco è secondo dietro Luca per volume totale.
5. Marco vede lo streak di Luca: 12 giorni consecutivi.
6. Marco apre il Confronto Diretto: vede che sulla panca piana Luca alza 90kg, lui 85kg.
7. Marco si motiva a superare Luca la prossima settimana.

### Scenario 3: Uso offline

1. Marco è in palestra senza connessione.
2. Apre l'app: la cache SQLite mostra gli ultimi 50 esercizi usati.
3. Avvia un nuovo allenamento usando gli esercizi in cache.
4. Salva l'allenamento in locale.
5. Quando torna online, l'app sincronizza l'allenamento con Firebase e aggiorna la leaderboard.

## 4. Requisiti funzionali

- FR-01: l'app deve recuperare gli esercizi da ExerciseDB API (RapidAPI) con endpoint per lista per gruppo muscolare, ricerca per nome e dettaglio esercizio.
- FR-02: l'app deve mantenere una cache SQLite locale con gli ultimi N esercizi usati dall'utente, disponibili offline.
- FR-03: l'utente deve poter avviare un nuovo allenamento selezionando esercizi dal catalogo remoto o dalla cache locale.
- FR-04: per ogni esercizio selezionato, l'utente deve poter aggiungere una o più serie con peso (kg) e ripetizioni.
- FR-05: l'app deve salvare l'allenamento completo con data/ora su SQLite locale e sincronizzarlo con Firebase quando online.
- FR-06: l'utente deve poter consultare lo storico degli allenamenti passati (ordinati per data decrescente).
- FR-07: l'utente deve poter registrare il proprio peso corporeo e misure (petto, vita, fianchi, braccia, gambe) con data.
- FR-08: l'app deve mostrare statistiche di progresso: peso max per esercizio, volume totale, andamento peso corporeo, andamento misure.
- FR-09: l'utente deve potersi registrare e autenticare via Firebase Auth (email/password).
- FR-10: l'utente deve poter cercare altri utenti per username e inviare richieste di amicizia.
- FR-11: l'utente deve poter accettare o rifiutare richieste di amicizia ricevute.
- FR-12: l'utente deve poter vedere la lista degli amici.
- FR-13: l'utente deve poter vedere un Feed con gli ultimi allenamenti degli amici.
- FR-14: l'app deve mostrare una Leaderboard settimanale basata sul volume totale di allenamento.
- FR-15: l'app deve tracciare e mostrare lo Streak (giorni consecutivi di allenamento) per l'utente e per gli amici.
- FR-16: l'utente deve poter confrontare direttamente le proprie statistiche con quelle di un amico.
- FR-17: l'utente deve poter consultare piani di allenamento base predefiniti (almeno 3).
- FR-18: l'app deve mostrare sempre stati UI chiari: loading, success, empty, error su ogni schermata.
- FR-19: l'app deve sincronizzare i dati locali con Firebase quando la connessione torna disponibile.
- FR-20: la Dashboard deve mostrare un riepilogo rapido: ultimo allenamento, streak corrente, posizione in leaderboard, peso corporeo attuale.

## 5. Epic, user stories e criteri di accettazione

### EPIC-01 - Autenticazione e profilo utente

**Obiettivo:**

Permettere all'utente di creare un account e accedere all'app.

**User stories:**

- Come nuovo utente, voglio registrarmi con email e password così da creare il mio profilo.
- Come utente registrato, voglio fare login così da accedere ai miei dati.

**Criteri di accettazione:**

- [ ] La registrazione con email/password crea un account Firebase Auth.
- [ ] Il login con credenziali valide porta alla Dashboard.
- [ ] Credenziali errate mostrano un messaggio di errore comprensibile.
- [ ] L'utente autenticato rimane loggato tra le sessioni.

### EPIC-02 - Catalogo esercizi (ExerciseDB API)

**Obiettivo:**

Consentire all'utente di esplorare e cercare esercizi dall'API remota, con cache locale per uso offline.

**User stories:**

- Come utente, voglio vedere tutti gli esercizi disponibili divisi per gruppo muscolare.
- Come utente, voglio cercare un esercizio per nome.
- Come utente, voglio filtrare per attrezzatura (manubri, bilanciere, corpo libero, etc.).
- Come utente, voglio vedere i dettagli di un esercizio (nome, gruppo muscolare, attrezzatura, GIF animata, istruzioni).
- Come utente offline, voglio poter usare gli esercizi che ho già fatto di recente.

**Criteri di accettazione:**

- [ ] Il catalogo carica esercizi da ExerciseDB API (`/exercises/bodyPart/{part}`, `/exercises/name/{name}`).
- [ ] La ricerca per nome filtra i risultati dall'API.
- [ ] Il filtro per attrezzatura (`/exercises/equipment/{type}`) funziona.
- [ ] Ogni esercizio mostra GIF animata (dall'API), nome, gruppo muscolare e attrezzatura.
- [ ] Gli esercizi usati dall'utente vengono salvati in cache SQLite locale.
- [ ] In assenza di rete, il catalogo mostra gli esercizi in cache con un'indicazione "offline".
- [ ] In assenza di rete totale (nessuna cache), viene mostrato un empty state con messaggio.

### EPIC-03 - Registrazione allenamento

**Obiettivo:**

Permettere all'utente di registrare un allenamento completo.

**User stories:**

- Come utente, voglio avviare un nuovo allenamento dalla Dashboard.
- Come utente, voglio aggiungere esercizi all'allenamento selezionandoli dal catalogo.
- Come utente, voglio registrare serie, peso e ripetizioni per ogni esercizio.
- Come utente, voglio salvare l'allenamento e vederlo nello storico.

**Criteri di accettazione:**

- [ ] Dalla Dashboard, un tap su "Nuovo Allenamento" avvia una sessione vuota.
- [ ] L'utente può aggiungere esercizi dal catalogo (API o cache).
- [ ] Per ogni esercizio, l'utente può aggiungere N serie con peso (kg) e ripetizioni.
- [ ] L'utente può rimuovere un esercizio o una serie prima del salvataggio.
- [ ] Al salvataggio, l'allenamento viene scritto su SQLite con data/ora.
- [ ] Se online, l'allenamento viene sincronizzato con Firebase.
- [ ] Se offline, l'allenamento resta in coda di sincronizzazione.
- [ ] L'allenamento compare immediatamente nello storico.

### EPIC-04 - Storico e dettaglio allenamento

**Obiettivo:**

Consentire all'utente di consultare gli allenamenti passati.

**User stories:**

- Come utente, voglio vedere l'elenco degli allenamenti passati in ordine cronologico.
- Come utente, voglio aprire un allenamento per vederne tutti i dettagli.

**Criteri di accettazione:**

- [ ] Lo storico mostra data, numero esercizi e volume totale per ogni allenamento.
- [ ] Gli allenamenti sono ordinati dal più recente.
- [ ] Il dettaglio mostra tutti gli esercizi con serie, peso e ripetizioni.
- [ ] Lo storico è disponibile anche offline (dati locali).

### EPIC-05 - Misure corporee

**Obiettivo:**

Permettere all'utente di tracciare peso e misure nel tempo.

**User stories:**

- Come utente, voglio registrare il mio peso corporeo.
- Come utente, voglio registrare le mie misure (petto, vita, fianchi, braccia, gambe).
- Come utente, voglio vedere l'andamento di peso e misure nel tempo.

**Criteri di accettazione:**

- [ ] L'utente può inserire peso (kg) e data.
- [ ] L'utente può inserire misure corporee con data.
- [ ] Un grafico a linee mostra l'andamento del peso nel tempo.
- [ ] Un grafico mostra l'andamento delle misure nel tempo.
- [ ] I dati sono salvati in SQLite locale (dato sensibile, mai su backend).

### EPIC-06 - Dashboard e statistiche

**Obiettivo:**

Offrire una panoramica immediata dei progressi dell'utente.

**User stories:**

- Come utente, voglio vedere un riepilogo rapido all'apertura dell'app.
- Come utente, voglio vedere le mie statistiche di progresso dettagliate.
- Come utente, voglio vedere grafici di progressione per ogni esercizio.

**Criteri di accettazione:**

- [ ] La Dashboard mostra: ultimo allenamento, streak corrente, posizione in leaderboard, peso attuale.
- [ ] La Dashboard ha accesso rapido a Nuovo Allenamento, Catalogo, Amici.
- [ ] La sezione Statistiche mostra: peso max per esercizio, volume settimanale, allenamenti totali.
- [ ] Grafico progressione peso per esercizio (peso max nel tempo).
- [ ] La Dashboard si aggiorna dopo ogni nuovo allenamento.

### EPIC-07 - Piani di allenamento base

**Obiettivo:**

Offrire piani strutturati per principianti.

**User stories:**

- Come utente principiante, voglio vedere piani di allenamento predefiniti.
- Come utente, voglio avviare un allenamento seguendo un piano.

**Criteri di accettazione:**

- [ ] Almeno 3 piani base precaricati (Full Body 2gg, Split 3gg, Push/Pull/Legs 3gg).
- [ ] Ogni piano indica esercizi, serie e ripetizioni consigliate per ogni giorno.
- [ ] L'utente può avviare un allenamento direttamente da un giorno del piano.

### EPIC-08 - Amici e competizione

**Obiettivo:**

Permettere all'utente di connettersi con amici e competere in modo sano.

**User stories:**

- Come utente, voglio cercare altri utenti per username e inviare richieste di amicizia.
- Come utente, voglio accettare o rifiutare richieste ricevute.
- Come utente, voglio vedere il feed con gli ultimi allenamenti dei miei amici.
- Come utente, voglio vedere la leaderboard settimanale per volume di allenamento.
- Come utente, voglio vedere il mio streak e quello dei miei amici.
- Come utente, voglio confrontare direttamente le mie statistiche con un amico.

**Criteri di accettazione:**

- [ ] Ricerca utenti per username via Firebase.
- [ ] Invio richiesta di amicizia con notifica all'altro utente.
- [ ] Accettazione/rifiuto richieste pendenti.
- [ ] Lista amici consultabile.
- [ ] Feed mostra gli ultimi 20 allenamenti degli amici (data, esercizi, volume).
- [ ] Leaderboard settimanale ordinata per volume totale (peso x ripetizioni).
- [ ] Streak: conteggio giorni consecutivi con almeno un allenamento.
- [ ] Confronto diretto: affianca peso max per esercizio, volume settimanale e streak tra due utenti.
- [ ] Il feed e la leaderboard mostrano uno stato di loading e un empty state se nessun amico.

## 6. Requisiti non funzionali

### UX e stati UI

- Ogni schermata deve esporre sempre uno stato UI chiaro: loading (ActivityIndicator o shimmer), success (dati presenti), empty (messaggio + azione suggerita), error (messaggio + retry).
- L'utente deve poter avviare un allenamento dalla Dashboard con massimo 1 tap.
- Il design dell'interfaccia segue il Design System "Performance Minimalist" creato con Stitch per il progetto "Iron Rank Fitness Social" (dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter).

### Prestazioni percepite

- La Dashboard deve caricarsi in meno di 1 secondo (dati locali).
- Il catalogo esercizi da API deve mostrare risultati entro 3 secondi con rete normale.
- Il salvataggio di un allenamento deve essere percepito come istantaneo.
- Le statistiche aggregate devono essere calcolate in background.

### Affidabilità e gestione errori

- Errori di rete (API ExerciseDB o Firebase) devono mostrare messaggi comprensibili con opzione retry.
- Timeout API dopo 10 secondi con messaggio di errore.
- La cache SQLite deve funzionare anche senza connessione (offline-first per esercizi recenti e allenamenti).
- La sincronizzazione post-offline non deve causare perdita di dati.
- Le credenziali Firebase scadute devono triggerare un re-login trasparente o guidato.

### Privacy e dati

- Peso corporeo e misure rimangono SOLO su SQLite locale, mai sincronizzati con backend.
- I dati di allenamento sono sincronizzati con Firebase solo dopo consenso implicito (login).
- L'API key di ExerciseDB è gestita lato client (RapidAPI header) e non esposta in chiaro nel codice versionato.
- Nessun dato viene condiviso con terze parti oltre a ExerciseDB (per fetch esercizi) e Firebase (per social).

## 7. Vincoli tecnici di progetto

- App `.NET MAUI` con target principale Android; iOS opzionale e secondario.
- Architettura MVVM con `CommunityToolkit.Mvvm`.
- Navigazione basata su Shell.
- `HttpClient` asincrono per chiamate remote (ExerciseDB API, Firebase REST API).
- `System.Text.Json` per il parsing JSON.
- Persistenza locale con SQLite (`sqlite-net-pcl`) per: cache esercizi, allenamenti, peso/misure, piani base.
- `Preferences` per token di autenticazione e preferenze leggere.
- Firebase Auth per autenticazione email/password.
- Firebase Realtime Database o Firestore per dati social (utenti, amici, allenamenti condivisi, leaderboard).
- Gestione esplicita di `IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState` nei ViewModel.
- Nessuna logica REST o business logic nei code-behind.
- Componenti UI: `CollectionView`, `ScrollView`, `Grid`, `Label`, `Button`, `Entry`, `DatePicker`, `Image` (per GIF esercizi).
- Design System Stitch: progetto `5765971046385640743` "Iron Rank Fitness Social", tema "Performance Minimalist" (dark mode, Electric Blue `#007AFF`, Lime Green `#CCFF00`, font Lexend/Inter).

## 8. Metriche di successo

- Un utente alla prima apertura completa il flusso `registrazione -> login -> Dashboard` in meno di 60 secondi.
- Il flusso `nuovo allenamento -> cerca esercizio -> registra 4 serie -> salva` richiede meno di 90 secondi.
- Il catalogo esercizi mostra risultati via API entro 3 secondi con rete 4G.
- L'app è completamente funzionante offline per la registrazione di un allenamento con esercizi in cache.
- Lo storico allenamenti persiste dopo riavvio dell'app.
- La leaderboard si aggiorna entro 5 secondi dall'apertura.
- Lo streak viene calcolato correttamente anche dopo interruzioni di connettività.

## 9. Rischi, dipendenze e questioni aperte

### Rischi

- ExerciseDB API potrebbe cambiare formato risposta o deprecare endpoint. Mitigazione: mapping DTO robusto con fallback.
- Firebase Realtime Database / Firestore richiede configurazione progetto e regole di sicurezza.
- La sincronizzazione offline-online potrebbe generare conflitti (es. due dispositivi). Mitigazione: timestamp e last-write-wins per MVP.
- La GIF degli esercizi potrebbe rallentare lo scroll del catalogo. Mitigazione: lazy loading e placeholder.
- Il tier gratuito di RapidAPI (ExerciseDB) ha limiti di chiamate/mese. Mitigazione: cache SQLite aggressiva.

### Dipendenze

- **ExerciseDB API** (RapidAPI): `exercisedb.p.rapidapi.com` — endpoint per esercizi. Richiede chiave API e header `X-RapidAPI-Key`.
- **Firebase Auth**: autenticazione email/password. Richiede progetto Firebase configurato.
- **Firebase Realtime Database / Firestore**: storage dati social. Richiede configurazione regole.
- **SQLite** (`sqlite-net-pcl`): persistenza locale.
- **Stitch** (Google): strumento di design UI, progetto `5765971046385640743` "Iron Rank Fitness Social".

### Questioni aperte

- TBD: scelta finale tra Firebase Realtime Database e Firestore (impatto su struttura dati e query social).
- TBD: formato esatto delle API ExerciseDB da validare dopo prima chiamata (documentazione RapidAPI).
- TBD: libreria per grafici (custom drawing, Microcharts, o LiveChartsCore).
- TBD: strategia esatta di sincronizzazione offline (coda locale, last-write-wins, merge).

## 10. Passaggio al planning

Il prossimo passo è derivare i documenti di planning:

- `docs/plan.md`
- `docs/architecture.md`
- `docs/test-matrix.md`

La skill consigliata per il passo successivo è `prd-to-plan`.
