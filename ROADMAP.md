# FORGE — Roadmap

## v0.9.0-beta (Settembre 2026) — QoL & Usabilità

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **Incremento rapido pesi (+/-)** | Pulsanti +2.5kg accanto al campo peso. Press-and-hold = +5kg | Bassa |
| **Copia set precedente** | Tasto "Duplica" che clona kg×reps nella serie successiva | Bassa |
| **Calcolatore 1RM** | Tap su un top lift → mostra massimale stimato (formula Epley) | Bassa |
| **Timer a voce** | TTS Android: "Serie completata, 90 secondi di pausa" | Media |
| **Filtro ricerca esercizi migliorato** | Filtri combinabili (muscolo AND attrezzatura) | Bassa |

## v1.0.0 (Ottobre 2026) — Release Stabile

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **Notifiche push FCM** | Notifica when amico ti segue, mette like, accetta richiesta | Alta |
| **Localizzazione completa** | File `.resx` per IT/EN/ES/DE/ZH — tutte le stringhe tradotte | Alta |
| **Body tracking** | Registra peso corporeo, misure, %BF. Grafico nel tempo | Media |
| **Leaderboard settimanale** | Classifica volume tra amici, reset ogni lunedì | Media |
| **Auto-build APK nelle Release** | GitHub Action che crea release automatica a ogni tag | Bassa |

## v1.1.0 (Novembre 2026) — Social & Community

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **Commenti ai workout** | Gli amici possono commentare i tuoi allenamenti nel feed | Media |
| **Sfide 1-vs-1** | "Sfida Marco a volume totale questa settimana" → notifica + classifica | Media |
| **Condivisione workout** | Share sheet Android: immagine riassuntiva dell'allenamento | Media |
| **Badge stagionali** | Achievement limitati nel tempo (es. "Allenati a Natale") | Bassa |
| **Storie workout** | Foto del giorno visibili 24h nel feed (stile Instagram Stories) | Alta |

## v1.2.0 (Dicembre 2026) — Health & Wear OS

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **Widget Android** | "START WORKOUT" direttamente dalla home screen del telefono | Media |
| **Wear OS companion** | Timer rest + tracking serie dallo smartwatch | Molto alta |
| **Integrazione Google Fit** | Sincronizza durata allenamento + calorie con Google Fit | Media |
| **Heart rate display** | Mostra battito cardiaco durante l'allenamento (se dispositivo supportato) | Alta |
| **Contatore passi** | Widget passi giornalieri nella dashboard | Bassa |

## v1.3.0 (Gennaio 2027) — Advanced Features

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **PDF report** | Export report allenamento con grafici, tabelle e foto | Media |
| **Allenamenti predefiniti** | Template "Push/Pull/Legs", "Full Body", "Upper/Lower" | Bassa |
| **Progressive overload tracker** | L'app suggerisce quanto aumentare rispetto alla sessione precedente | Media |
| **Grafici avanzati** | Volume per muscolo nel tempo, PR timeline, frequenza settimanale | Media |
| **Modifica workout post-salvataggio** | Correggi un peso sbagliato dopo aver salvato | Bassa |
| **Ricerca workout nello storico** | Cerca per nome esercizio, data, volume | Bassa |

## v1.4.0 (Febbraio 2027) — Platform & Infrastruttura

| Feature | Descrizione | Difficoltà |
|---------|-------------|------------|
| **Google Play Store** | Pubblicazione ufficiale con keystore di produzione | Media |
| **Backup automatico** | Export automatico settimanale dei dati su Google Drive | Media |
| **SQLite encryption** | Database cifrato con SQLCipher | Media |
| **Multi-server switch** | Profili multipli: switch tra server self-hosted diversi | Bassa |
| **Web dashboard** | Interfaccia web per visualizzare i dati da PC | Alta |
| **REST API pubblica** | API documentata per integrazioni di terze parti | Alta |

---

## Sintesi Roadmap (pochi punti chiave)

- **🚀 Fase 1: Smart Workout Flow** — Timer di recupero automatico con vibrazione, pesi dell'ultima sessione già visibili come placeholder (Ghost Inputs) e tasti rapidi `+/-` per variare i carichi di 2.5kg senza tastiera.
- **📊 Fase 2: Analytics Avanzate** — Calcolo automatico del massimale stimato (1RM con formula Epley), grafici interattivi per la distribuzione del volume sui gruppi muscolari e calendario delle streak per la costanza.
- **🏠 Fase 3: Self-Hosting Pro & Social** — Ottimizzazione dei dati in tempo reale nel feed degli amici sullo stesso server PocketBase, modalità Offline First per salvare in palestra anche senza campo e sincronizzare al rientro a casa.

---

## 🏢 FORGE Local Business — Modulo B2B per Palestre

### Visione

FORGE non è solo un'app per il singolo atleta: la sua architettura self-hosted permette a un **proprietario di palestra** di lanciare il backend su un mini-PC locale e creare un **sistema gestionale/social privato** per tutta la sua struttura.

```
+---------------------------+          +------------------------------+
| App FORGE (.NET MAUI)     |          | Mini-PC / Server Locale      |
| Connessa al Wi-Fi Locale  |  ----->  | - Docker Engine              |
|                           |          | - PocketBase Backend (8090)  |
+---------------------------+          +------------------------------+
```

### Vantaggi per il Titolare

| Vantaggio | Dettaglio |
|-----------|-----------|
| **Zero costi SaaS** | Niente abbonamenti AWS o licenze per postazione. Che ci siano 50 o 5.000 iscritti, il costo software è 0€/mese. Solo corrente elettrica per il mini-PC. |
| **Community locale (Feed di Sala)** | I clienti che si allenano nella stessa sala vedono i PR degli altri iscritti in tempo reale. Classifiche interne della palestra (es. "Top Squat del Mese"). Condivisione protocolli creati dai personal trainer della struttura. |
| **Privacy e GDPR al 100%** | I dati su salute e performance fisica non lasciano mai l'edificio. Punto di marketing: *"La nostra palestra rispetta la tua privacy, i tuoi dati rimangono qui dentro"*. |
| **Funzionamento Offline / LAN First** | Se la connessione internet globale si interrompe, l'app continua a funzionare. Gli smartphone comunicano con il server PocketBase via Wi-Fi locale. Zero downtime durante gli allenamenti. |

### Implementazione Tecnica

1. **Hardware**: mini-PC con Linux, IP statico, connesso alla rete locale della palestra
2. **Deployment**: `docker compose -f docker-compose.gym.yml up -d`
3. **Rete**: DNS locale sul router (es. `forge.palestra.local`) o IP statico diretto
4. **Onboarding clienti**: codice QR affisso in bacheca → scansionandolo l'app configura automaticamente l'URL del server della palestra

### Sviluppi Futuri B2B

| Modulo | Descrizione |
|--------|-------------|
| **Trainer Dashboard** | Interfaccia web amministrativa dove i personal trainer assegnano schede e protocolli direttamente ai profili degli iscritti |
| **Local TV Leaderboard** | Modulo per proiettare su schermo/TV della palestra la classifica dei PR sollevati in tempo reale durante la giornata |
| **QR Code Onboarding** | Scansiona il QR in palestra → l'app si configura da sola con il server locale |
| **Multi-tenant** | Supporto per più palestre sullo stesso server con isolamento dati |

---

## Backlog (Ideas & Experiments)

| Feature | Descrizione |
|---------|-------------|
| **AI form check** | Analisi posturale via fotocamera durante l'esercizio |
| **Voice input** | "Cento chili, otto rep" invece di digitare |
| **Social login** | Google/Apple sign-in oltre a email/password |
| **Mappe palestre** | Mappa delle palestre vicine con recensioni community |
| **Marketplace piani** | Utenti condividono e vendono piani di allenamento |
| **Nutrizione tracker** | Calorie e macro alongside workout tracking |
| **Apple Watch** | Supporto iOS + watchOS companion |
| **Gamification 2.0** | Livelli, XP, stagioni competitive |
| **Coaching AI** | Suggerimenti automatici basati sui dati storici |
| **Video esercizi** | Video YouTube integrati oltre alle GIF |

---

### Priorità suggerita per i prossimi 3 mesi

```
Ottobre    → Incremento rapido + Copia set + 1RM calculator
Novembre   → Widget Android + PDF report + Template predefiniti
Dicembre   → Notifiche push + Wear OS (se fattibile)
```

---

> **FORGE** — Costruisci il tuo fisico. Sfida i tuoi amici. Forgia la tua leggenda.
