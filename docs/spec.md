# Specifica di Progetto - GymTracker Mobile

## 1. Visione e contesto

### Problema da risolvere

Chi va in palestra spesso fatica a tenere traccia dei progressi, non ricorda i pesi usati la volta precedente e manca un modo semplice per monitorare l'evoluzione del proprio allenamento. Per un progetto didattico MAUI serve una app per registrare esercizi, pesi e progressi con navigazione chiara, dati locali persistenti e funzionalità social.

### Obiettivo del progetto

Realizzare una applicazione `.NET MAUI` Android-first per tracciare gli allenamenti con serie, ripetizioni e peso, consultare un catalogo di esercizi, monitorare il peso corporeo, visualizzare statistiche di progresso, competere con amici e usare piani di allenamento base per principianti.

### Utenti target

- principianti che iniziano ad allenarsi in palestra e necessitano di struttura;
- appassionati di fitness che vogliono tracciare i progressi nel tempo;
- utenti che vogliono competere in modo costruttivo con amici.

## 2. Ambito MVP

### Flusso principale da supportare

1. L'utente apre l'app e può iniziare un nuovo allenamento selezionando esercizi dal catalogo.
2. L'utente registra serie, ripetizioni e peso per ogni esercizio.
3. L'utente può inserire il proprio peso corporeo dopo l'allenamento.
4. L'utente visualizza i progressi nel tempo tramite statistiche.
5. L'utente può aggiungere amici e vedere i loro allenamenti per competere.
6. L'utente può seguire piani di allenamento base predefiniti.

### Funzionalità obbligatorie

- catalogo esercizi organizzato per gruppo muscolare;
- registrazione allenamento con data, esercizi, serie, ripetizioni e peso;
- storico allenamenti consultabile;
- tracciamento peso corporeo nel tempo;
- dashboard statistiche con grafici di progresso;
- gestione amici (richieste, accettazione, lista);
- visualizzazione allenamenti amici;
- piani di allenamento base per principianti;
- gestione esplicita di loading state, error state, empty state e success state.

### Funzionalità opzionali future

- post-MVP 1: programmi personalizzabili e salvataggio piani utente;
- post-MVP 2: timer per pause tra serie;
- post-MVP 3: obiettivi settimanali e notifiche reminder;
- post-MVP 4: export dati in formato CSV.

### Priorità roadmap post-MVP

1. programmi personalizzabili;
2. timer pause;
3. obiettivi settimanali;
4. export dati.

### Non-obiettivi

- acquisizione di dati di terze parti per esercizi;
- integrazione con dispositivi smart (orologi, bilance);
- pagamento o abbonamenti premium;
- recensioni o rating sociali pubblici;
- supporto offline completo per funzionalità social nel primo rilascio.

## 3. Requisiti funzionali

- FR-01: l'app deve mostrare un catalogo di esercizi organizzato per gruppo muscolare (petto, schiena, spalle, braccia, gambe, addominali).
- FR-02: l'utente deve poter avviare un nuovo allenamento selezionando esercizi dal catalogo.
- FR-03: per ogni esercizio selezionato, l'utente deve poter aggiungere una o più serie con peso e ripetizioni.
- FR-04: l'app deve salvare l'allenamento completo con data e ora.
- FR-05: l'utente deve poter consultare lo storico degli allenamenti passati.
- FR-06: l'utente deve poter registrare il proprio peso corporeo.
- FR-07: l'app deve mostrare statistiche di progresso (peso usato nel tempo per esercizio, peso corporeo nel tempo).
- FR-08: l'utente deve poter inviare richieste di amicizia ad altri utenti.
- FR-09: l'utente deve poter accettare o rifiutare richieste di amicizia ricevute.
- FR-10: l'utente deve poter vedere gli allenamenti degli amici.
- FR-11: l'utente deve poter consultare piani di allenamento base predefiniti per principianti.
- FR-12: l'app deve mostrare sempre stati UI chiari: loading, success, empty, error.

## 4. Epic, user stories e criteri di accettazione

### EPIC-01 - Catalogo esercizi

**Obiettivo:**

Consentire all'utente di selezionare esercizi da un elenco strutturato.

**User stories:**

- Come utente, voglio vedere un elenco di esercizi divisi per gruppo muscolare così da trovare rapidamente quello che mi serve.
- Come utente, voglio cercare un esercizio per nome così da trovarlo anche senza conoscere il gruppo muscolare.

**Criteri di accettazione:**

- [ ] Il catalogo mostra gli esercizi raggruppati per gruppo muscolare.
- [ ] Toccando un gruppo muscolare, vengono mostrati i relativi esercizi.
- [ ] Toccando un esercizio, l'app mostra nome, gruppo muscolare e descrizione breve.

### EPIC-02 - Registrazione allenamento

**Obiettivo:**

Permettere all'utente di registrare un allenamento completo con esercizi, serie, ripetizioni e peso.

**User stories:**

- Come utente, voglio avviare un nuovo allenamento così da registrare la mia sessione.
- Come utente, voglio aggiungere esercizi all'allenamento con le relative serie.
- Come utente, voglio salvare l'allenamento così da consultarlo in seguito.

**Criteri di accettazione:**

- [ ] L'utente può avviare un nuovo allenamento dalla schermata home.
- [ ] L'utente può aggiungere esercizi selezionandoli dal catalogo.
- [ ] Per ogni esercizio, l'utente può aggiungere una o più serie con peso e ripetizioni.
- [ ] L'utente può salvare l'allenamento con data e ora correnti.
- [ ] L'allenamento salvato compare nello storico.

### EPIC-03 - Storico e dettaglio allenamento

**Obiettivo:**

Consentire all'utente di consultare gli allenamenti passati.

**User stories:**

- Come utente, voglio vedere l'elenco degli allenamenti passati così da avere una panoramica.
- Come utente, voglio aprire un allenamento passato per vedere i dettagli.

**Criteri di accettazione:**

- [ ] Lo storico mostra gli allenamenti in ordine dal più recente al meno recente.
- [ ] Ogni elemento mostra data, durata (se calcolata) e numero esercizi.
- [ ] Toccando un elemento, vengono mostrati tutti i dettagli (esercizi, serie, ripetizioni, peso).

### EPIC-04 - Progressione peso corporeo

**Obiettivo:**

Permettere all'utente di tracciare il proprio peso corporeo nel tempo.

**User stories:**

- Come utente, voglio registrare il mio peso corporeo dopo l'allenamento.
- Come utente, voglio vedere l'andamento del mio peso nel tempo.

**Criteri di accettazione:**

- [ ] L'utente può registrare un nuovo peso corporeo.
- [ ] L'app mostra un grafico dell'andamento del peso nel tempo.
- [ ] Il grafico è consultabile dalla dashboard.

### EPIC-05 - Statistiche e progressi

**Obiettivo:**

Mostrare all'utente i progressi nel tempo.

**User stories:**

- Come utente, voglio vedere il peso massimo sollevato per ogni esercizio.
- Come utente, voglio vedere l'andamento degli allenamenti nel tempo.

**Criteri di accettazione:**

- [ ] La dashboard mostra statistiche aggregate degli allenamenti.
- [ ] Per ogni esercizio, viene mostrato il peso massimo storico.
- [ ] Viene mostrato il numero di allenamenti negli ultimi 7 giorni.

### EPIC-06 - Gestione amici

**Obiettivo:**

Permettere all'utente di connettersi con altri utenti per competere.

**User stories:**

- Come utente, voglio inviare richieste di amicizia ad altri utenti.
- Come utente, voglio accettare o rifiutare richieste di amicizia.
- Come utente, voglio vedere gli allenamenti dei miei amici.

**Criteri di accettazione:**

- [ ] L'utente può cercare altri utenti per username.
- [ ] L'utente può inviare una richiesta di amicizia.
- [ ] L'utente riceve una notifica per nuove richieste di amicizia.
- [ ] L'utente può accettare o rifiutare richieste pendenti.
- [ ] L'utente può vedere la lista degli amici.
- [ ] L'utente può vedere gli ultimi allenamenti degli amici.

### EPIC-07 - Piani di allenamento

**Obiettivo:**

Offrire piani strutturati per principianti.

**User stories:**

- Come utente, voglio vedere piani di allenamento predefiniti.
- Come utente, voglio seguire un piano per principianti.

**Criteri di accettazione:**

- [ ] L'app fornisce almeno 2 piani base per principianti (schema 2 giorni, schema 3 giorni).
- [ ] Ogni piano indica quali esercizi fare per ogni sessione.
- [ ] L'utente può avviare un allenamento da un piano.

## 5. Requisiti non funzionali

### UX e stati UI

- L'utente deve poter iniziare un allenamento dalla schermata home con un tap.
- Le schermate devono mostrare sempre stati chiari: loading, success, empty, error.
- Le liste devono essere scorrevoli e responsive.

### Prestazioni percepite

- L'apertura dell'app e la navigazione tra schermate devono essere immediate.
- Il salvataggio di un allenamento deve completarsi senza ritardi percepibili.
- Le statistiche devono caricarsi in pochi secondi.

### Affidabilità e gestione errori

- Gli errori di rete devono mostrare messaggi comprensibili con opzione di retry.
- Gli errori di parsing non devono causare crash.
- La perdita di connettività durante un'operazione deve essere gestita gracefully.

### Privacy e dati

- I dati degli allenamenti sono memorizzati localmente con SQLite.
- I dati social (amici, richieste) richiedono backend e connettività.
- Il peso corporeo è un dato sensibile e rimane solo sul dispositivo dell'utente.

## 6. Vincoli tecnici di progetto

- App `.NET MAUI` con target principale Android; iOS opzionale e secondario.
- Architettura MVVM.
- Navigazione basata su Shell.
- `CommunityToolkit.Mvvm` per la gestione dei ViewModel.
- `HttpClient` asincrono per chiamate remote al backend.
- `System.Text.Json` per il parsing.
- Persistenza locale con SQLite per allenamenti, esercizi, peso corporeo; `Preferences` per impostazioni leggere.
- Gestione esplicita di `IsBusy`, error state, empty state nei ViewModel.
- Nessuna logica REST o business logic nei code-behind.
- UI basata su `CollectionView`, `ScrollView`, `Grid`, `Label`, `Button`, `Entry`, `DatePicker`.

## 7. Metriche di successo

- Un utente alla prima apertura riesce a completare il flusso `nuovo allenamento -> aggiungi esercizio -> registra serie -> salva` senza spiegazioni.
- Lo storico degli allenamenti persiste correttamente dopo il riavvio dell'app.
- Il grafico del peso corporeo visualizza correttamente i dati inseriti.
- Le richieste di amicizia vengono inviate e ricevute correttamente (con backend attivo).

## 8. Rischi, dipendenze e questioni aperte

### Rischi

- Il backend per funzionalità social potrebbe non essere disponibile o richiedere autenticazione reale.
- I dati degli amici potrebbero non essere sincronizzati in tempo reale.
- La gestione di many-to-many per esercizi in SQLite potrebbe richiedere attenzione.

### Dipendenze

- Backend remoto per gestione amici e competizioni.
- SQLite per persistenza locale.
- Grafico/chart library per visualizzazioni statistiche (oppure implementazione custom).

### Questioni aperte

- TBD: Servizio backend effettivo da usare (custom, Firebase, mock).
- TBD: Formato esatto delle API per amici e allenamenti.
- TBD: Se usare una chart library oimplementare grafici semplici custom.

## 9. Passaggio al planning

Il prossimo passo è derivare i documenti di planning:

- `docs/plan.md`
- `docs/architecture.md`
- `docs/test-matrix.md`

La skill consigliata per il passo successivo è `prd-to-plan`.