# Privacy Policy — FORGE

## Raccolta Dati

Quando usi FORGE, vengono raccolti i seguenti dati:

| Dato | Dove viene salvato | Scopo |
|------|-------------------|-------|
| Email | Server PocketBase | Autenticazione e login |
| Nome utente | Server PocketBase | Identificazione nel feed e nel profilo |
| Password | Solo sul telefono (SecureStorage) | Autenticazione |
| Allenamenti (esercizi, serie, kg, reps) | Server PocketBase + telefono (SQLite) | Storico e statistiche |
| Foto allenamento | Server PocketBase + telefono (SQLite) | Diario progresso fisico |
| Like e follow | Server PocketBase | Funzionalità social |
| Avatar | Server PocketBase | Immagine profilo |

## Dove Sono i Dati

I dati risiedono in due posti:
- **Sul tuo telefono**: database SQLite locale (workout, esercizi, achievement, piani)
- **Sul server FORGE**: un server self-hosted su hardware casalingo che esegue PocketBase

Il server si trova in Italia ed è gestito dallo sviluppatore del progetto.

## Chi Può Vedere i Tuoi Dati

- **Tu**: accesso completo ai tuoi dati tramite l'app
- **Altri utenti**: vedono il tuo nome, avatar, allenamenti (nome, volume, durata, esercizi) e like ricevuti nel feed
- **Sviluppatore**: in quanto amministratore del server, ha accesso tecnico a tutti i dati
- **Nessun altro**: i dati NON sono condivisi con terze parti, non sono venduti, non sono usati per pubblicità

## Foto

Le foto che scatti durante l'allenamento sono salvate come parte del workout. Sono visibili a chiunque abbia accesso al dettaglio di quel workout (amici che vedono il tuo feed). Non vengono elaborate, condivise o usate per scopi diversi dalla visualizzazione nell'app.

## Cancellazione Dati

Per cancellare i tuoi dati:
1. Apri l'app → Impostazioni → Logout
2. Contatta lo sviluppatore per la cancellazione completa dell'account

La cancellazione rimuove i dati dal server PocketBase. I dati in locale sul telefono vengono rimossi disinstallando l'app.

## Sicurezza

- Password: cifrata con SecureStorage (Android Keystore), mai in chiaro
- Connessione: HTTPS con Let's Encrypt
- Database remoto: ogni utente vede solo i propri dati (API rules row-level)
- Il server NON è un servizio cloud professionale: è un computer in casa

## Limitazioni

- Il server potrebbe non essere sempre online (rete domestica, manutenzione)
- I dati non hanno backup automatici professionali
- L'app non è stata sottoposta a penetration test o certificazioni di sicurezza
- Questa non è una privacy policy legalmente vincolante — è una dichiarazione di trasparenza

## Contatti

Per domande sulla privacy o richieste di cancellazione: apri una issue su GitHub.

---

*Ultimo aggiornamento: 18 giugno 2026*
