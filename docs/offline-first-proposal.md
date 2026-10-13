# Proposta: uso locale, server facoltativo e aggiornamenti

13 ottobre 2026 — valutazione richiesta dal proprietario, senza implementazione runtime.

## Obiettivo

Prima apertura con scelta «Usa solo sul telefono» oppure «Collega un server». La prima opzione non richiede account, indirizzo o connessione. La seconda richiede un indirizzo inserito dall’utente e l’accesso al proprio PocketBase. La scelta rimane modificabile nelle impostazioni; nessun ripiego automatico sul dominio domestico.

## Stato e dipendenze

SQLite è già presente per schede e allenamenti, ma il proprietario dei dati e diverse funzioni dipendono ancora dall’account PocketBase. Rimuovere ServerAddress.PublicUrl da solo non abilita la modalità locale. GIF esterne non diventano disponibili offline conservandone solo l’URL. La rimozione del dominio dal client non chiude l’accesso pubblico al server e non nasconde il suo IP nel DNS.

## Iterazioni proposte

1. **Note automatiche — implementate il 13 ottobre su experiments**: note salvate senza conferma e ricaricate nella stessa scheda; le sole modifiche alle note sono escluse dal popup. Cinque regressioni mirate superate, verifica sul dispositivo ancora necessaria.
2. **Modalità locale**: identità locale stabile, accesso senza login, nessuna chiamata automatica a PocketBase o registrazione Firebase; schede, storico, statistiche, achievement, foto e timer persistenti. Catalogo di base incluso o importato; download dei media opzionale. Backup/esportazione e ripristino prima della migrazione dei dati esistenti.
3. **Server facoltativo**: spazio dati distinto per server e account, conferma dell’importazione alla prima connessione, salvataggio locale prima dell’upload, coda di sincronizzazione per tutte le entità supportate, gestione dei conflitti, eliminazioni e tentativi senza duplicati. Social e push social disponibili solo quando collegati a un’istanza compatibile. Disconnessione senza perdita dei dati locali.
4. **Aggiornamenti dall’app**: controllo di versione, note di rilascio, download HTTPS e avvio dell’installazione Android. Pacchetto identificato e firmato coerentemente, versioni incrementali, verifica dell’integrità e migrazioni dei dati locali. GitHub Releases può ospitare i pacchetti senza richiedere navigazione manuale sul sito. Chiave di firma di produzione stabile da gestire prima della distribuzione.

## Verifiche di accettazione proposte

- Primo avvio senza rete: creazione/modifica scheda, allenamento, note, riavvio e storico funzionanti senza account.
- Nessun accesso automatico al server domestico in modalità locale.
- Collegamento, cambio account/server e disconnessione non mescolano o cancellano dati di altri spazi.
- Sincronizzazione ripetuta senza duplicati; gestione esplicita di modifiche ed eliminazioni concorrenti.
- Aggiornamento installato mantiene dati e preferenze; backup locale ripristinabile.

## Idee future separate

Medaglie o feedback per i record personali delle serie: aggiunti alla roadmap, non implementati.

## Riferimenti Android

- https://developer.android.com/google/play/app-updates
- https://developer.android.com/distribute/marketing-tools/alternative-distribution
