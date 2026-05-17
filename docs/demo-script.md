# Demo Script — FORGE

> Durata: ~10 minuti. Preparare l'app già avviata su emulatore/device.

## 1. Avvio e Login (1 minuto)

**Mostrare:**
- Avvio dell'app → schermata Login
- Design mockup `login_cyber_athletic_elite_nero_opaco` (tema scuro) / `login_fitness_core_light` (tema chiaro)
- Campi email e password con bordo stile mockup
- Toggle "Already registered? Sign in" / "Don't have an account? Register"

**Dire:**
> FORGE è un'app per tracciare i tuoi allenamenti in palestra e competere con gli amici.
> Al primo avvio vedi la schermata di login. Se sei già registrato, fai auto-login.

**Azione:** Mostrare auto-login (l'app passa direttamente alla Dashboard).

## 2. Dashboard (1 minuto)

**Mostrare:**
- Squad Activity in alto: avatar circolari degli amici con bordo ciano
- Current Streak: numero grande (es. 3) con label "WEEKS"
- Card "Today" con piano di allenamento casuale
- Pulsante ▶ START WORKOUT

**Dire:**
> La Dashboard mostra la tua Squad Activity (gli amici che si sono allenati di recente),
> la tua Current Streak settimanale e un piano di allenamento casuale dalla tua libreria.
> La streak conta le settimane consecutive con almeno un allenamento. Si resetta solo
> dopo 7 giorni senza attività — molto più realistico di una streak giornaliera.

**Azione:** Tap su avatar profilo in alto a sinistra → mostrare che apre il Profilo.
Tornare indietro.

## 3. Start Session e Allenamento (2 minuti)

**Mostrare:**
- Tap su ▶ START WORKOUT → StartSessionPage
- Quick Start card, Create New Plan card, Your Protocols
- Tap su Quick Start → ActiveWorkoutPage vuota

**Dire:**
> Da qui puoi avviare un allenamento libero con Quick Start, creare un nuovo piano,
> o riprendere uno dei tuoi protocolli salvati. I piani includono esercizi, serie, pesi
> e ripetizioni preconfigurati.

**Azione:** Cercare "bench" nella barra ricerca → mostrare risultati con immagini da ExerciseDB.
Tap su un esercizio → card con immagine, nome, bodyPart, suggerimenti.
Aggiungere 2 serie con KG e REPS. Tap sul cerchio ✓ → si riempie di LimeGreen.
Tap su ADD SET → nuova riga. Mostrare il rest timer.

**Dire:**
> La ricerca esercizi chiama ExerciseDB API in tempo reale. Le immagini vengono cachate
> su PocketBase per evitare di riseguire i redirect ogni volta. Il completamento
> di una serie viene mostrato con il cerchio che si riempie.

## 4. Feed e Like (2 minuti)

**Mostrare:**
- Swipe al tab Feed
- Search bar in alto: cercare un utente (es. "leo")
- Risultati con avatar e pulsante Follow
- Feed allenamenti degli amici con: avatar, nome, tempo fa, titolo, esercizi, volume, durata
- Cuore ♡ grigio → tap → ♥ LimeGreen, conteggio aggiornato istantaneamente
- Tap di nuovo → ♡ grigio (unlike)

**Dire:**
> Il Feed mostra gli allenamenti degli utenti che segui. Puoi cercare altri atleti
> con la search bar e seguirli con un tap. Il sistema like funziona con un array
> liked_by su PocketBase: quando metti like, il tuo ID viene aggiunto e il conteggio
> si aggiorna subito sia lato client che server.

## 5. Statistiche (1.5 minuti)

**Mostrare:**
- Swipe al tab Stats
- Card riepilogative: Total Workouts, Volume, Hours
- Filtri WEEK / MONTH / 3M / YEAR — cliccare MONTH → i dati si aggiornano
- Grafico volume a barre settimanali con etichette data e divisori mese
- Top Lifts: 5 esercizi con peso massimo
- Calendario: giorni mese con pallino sui giorni di allenamento

**Dire:**
> Le statistiche mostrano i tuoi progressi con dati reali da PocketBase. I filtri
> temporali permettono di vedere l'andamento su diverse finestre. Il grafico volume
> è generato dinamicamente con BindableLayout. I top lifts vengono calcolati
> estraendo il peso massimo per ogni esercizio dal JSON exercise_data.

## 6. Profilo (1 minuto)

**Mostrare:**
- Tap su ♥ in alto a destra (Stats) → Notifiche
- Friend Requests pendenti
- Like Notifications: "X liked your workout Y"
- Tornare indietro, tap avatar → Profilo
- Avatar (foto o iniziali), nome, tier, bio
- Stats grid: Total Workouts, Total Volume, Week Streak, ♥ Likes ricevuti
- Recent Forges: ultimi allenamenti con durata, like, volume
- Tap su ✏️ → edit overlay con nome, bio, anteprima avatar, pulsante SAVE

**Dire:**
> Il profilo mostra le tue statistiche aggregate e la cronologia allenamenti.
> Puoi modificare nome e bio, e caricare una foto profilo. Le notifiche
> nella pagina dedicata mostrano sia le richieste di amicizia che i like
> ricevuti sui tuoi allenamenti.

## 7. Tema e Impostazioni (1 minuto)

**Mostrare:**
- Tap su ⚙ in alto a destra (da Dashboard o Feed) → Settings
- Toggle Dark Mode / Light Mode → il tema cambia istantaneamente su TUTTE le pagine
- Mostrare Dashboard in tema chiaro, poi Feed, poi Stats
- Tornare al tema scuro
- Pulsante Logout

**Dire:**
> Il tema è gestito da un ThemeService che scrive direttamente i valori colore
> in Application.Current.Resources. Poiché tutte le pagine usano DynamicResource,
> il cambio tema si propaga immediatamente a tutto il visual tree, Shell inclusa.

## 8. Conclusione (30 secondi)

**Dire:**
> FORGE è un'app completa che unisce tracking allenamenti e competizione sociale.
> I punti di forza sono l'architettura MVVM pulita, l'UI curata con doppio tema,
> il sistema like con conteggio istantaneo e la streak settimanale intelligente.
> Tra i limiti: mancano le misure corporee e la leaderboard, che possono essere
> aggiunte in future iterazioni.

---

## Backup plan

Se l'app crasha o la rete non funziona:
1. Usare gli screenshot in `assets/screenshots/` come fallback visivo
2. Spiegare il flusso a voce usando i mockup come riferimento
3. Mostrare il codice delle parti più interessanti (BaseViewModel, LikeWorkout, streak)
