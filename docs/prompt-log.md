# Prompt Log — FORGE (GymTracker Mobile)

Documentazione dei prompt significativi usati durante lo sviluppo con AI (OpenCode), con decisioni e motivazioni.

---

## Prompt 1 — Bootstrap del progetto MAUI con Shell e Design System

**Data:** IT-01 (inizio progetto)
**Obiettivo:** Creare la struttura base del progetto .NET MAUI con Shell navigation e tema Stitch.

**Prompt:**
> Crea un progetto .NET MAUI con Shell navigation a 3 tab (Dashboard, Feed, Stats), applica il tema Performance Minimalist con colori Electric Blue #007AFF e Lime Green #CCFF00, font Lexend/Inter. Usa MVVM con CommunityToolkit.Mvvm.

**Risultato:** Progetto compilabile con AppShell, 3 tab funzionanti, BaseViewModel con stati UI, Styles.xaml con DynamicResource per supporto tema.

**Decisione:** 3 tab invece dei 5 pianificati inizialmente — Catalogo e Profilo sono stati integrati come route di dettaglio accessibili da Dashboard/Feed/Stats. Motivazione: UI più pulita, meno tab affollati.

---

## Prompt 2 — Integrazione PocketBase per Auth e Social

**Data:** IT-05/IT-06 (branch `mockup`)
**Obiettivo:** Sostituire Firebase con PocketBase self-hosted per autenticazione e dati social.

**Prompt:**
> Implementa un PocketBaseService che gestisca login, registrazione, auto-login, CRUD su collection logged_workouts, social_graph, excercise. Usa HttpClient con token Bearer. Crea LoginPage e LoginViewModel con stati loading/error/success. Gestisci il flusso di auto-login in App.xaml.cs.

**Risultato:** PocketBaseService con 20+ metodi, LoginPage con mockup design cyber_athletic_elite, auto-login funzionante.

**Correzione AI:** L'AI inizialmente impostava `HttpClient.BaseAddress` dopo la prima richiesta, causando crash `JavaProxyThrowable`. Fix: impostare `BaseAddress` direttamente nel costruttore `HttpClient` in `MauiProgram.cs`.

---

## Prompt 3 — Feed sociale con like e ricerca utenti

**Data:** IT-06 (branch `mockup`)
**Obiettivo:** Implementare il feed allenamenti degli amici con sistema like e ricerca utenti live.

**Prompt:**
> Riscrivi FeedPage basandoti sul mockup feed_elite_nero_opaco. Aggiungi search bar per cercare utenti (live search con debounce 400ms), feed allenamenti dei seguiti con avatar, nome, esercizi, volume, durata. Implementa LikeWorkoutAsync/UnlikeWorkoutAsync su PocketBase con liked_by array. Mostra cuore ♥ che si riempie in LimeGreen quando likato.

**Risultato:** FeedPage completa con search, follow, feed post, like funzionante. Il like usa JsonDocument per parsing robusto dei campi liked_by/likes.

**Correzione AI:** `GetFollowedWorkoutsAsync` inizialmente deserializzava l'intera lista con `ReadFromJsonAsync`, ma PocketBase restituiva il campo `user` come oggetto `{id:"..."}` invece di stringa. Fix: parsing manuale con `JsonDocument` per ogni item.

---

## Prompt 4 — Streak settimanale e statistiche reali

**Data:** IT-07 (branch `mockup`)
**Obiettivo:** Calcolare streak settimanale, visualizzare statistiche reali da PocketBase con grafici.

**Prompt:**
> Implementa StatsPage con dati reali da PocketBase. Aggiungi filtri temporali (WEEK/MONTH/3M/YEAR/ALL), grafico volume a barre settimanali, top lifts calcolati da exercise_data JSON. Il calendario mensile deve mostrare i giorni di allenamento. La streak deve essere settimanale: conta settimane consecutive con almeno un allenamento, reset dopo 7+ giorni senza attività.

**Risultato:** StatsPage con grafico volume dinamico, top lifts, calendario. Streak settimanale funzionante.

**Decisione architetturale (presa dallo sviluppatore, non dall'AI):** La streak è stata implementata come conteggio di settimane consecutive (lunedì-domenica) invece che giorni. Motivazione: la dashboard mostra già "WEEKS" come label, e una streak giornaliera è troppo fragile (basta un giorno di riposo per azzerarla). Con 7 giorni di tolleranza, la streak riflette meglio la costanza reale in palestra.

---

## Prompt 5 — Avatar utente con ImageSource

**Data:** IT-07 (fix)
**Obiettivo:** Mostrare la foto profilo dell'utente nella Dashboard, Feed, Stats e Profilo.

**Prompt:**
> L'avatar utente non si vede nelle pagine. L'URL generato da GetFileUrl include il token PocketBase come query parameter. MAUI gestisce male le URL con parametri query nel binding Image.Source diretto. Usa ImageSource.FromUri() nel ViewModel.

**Risultato:** Tutti gli avatar ora usano `ImageSource.FromUri()` creato nel ViewModel e bindato come `{Binding AvatarSource}`. L'immagine viene caricata correttamente.

**Motivazione tecnica:** `Image.Source` con binding a stringa URL in MAUI Android non gestisce correttamente URL con parametri query (`?token=...`). `ImageSource.FromUri(new Uri(url))` crea un `UriImageSource` che gestisce correttamente il download HTTP con tutti i parametri.

---

## Riepilogo metriche AI

| Metrica | Valore |
|---------|--------|
| Prompt significativi documentati | 5 |
| File totali modificati con AI | 50+ |
| Errori AI corretti con motivazione | 4 |
| Decisioni architetturali prese dallo sviluppatore | 3 |
| Iterazioni completate | 3 principali + fix iterativi |
| Branch usati | mockup → main |
