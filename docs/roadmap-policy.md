# Aggiornamenti della roadmap

Da questa iterazione ogni modifica funzionale a un elemento della roadmap deve avere uno storico in italiano e inglese, senza eliminare le annotazioni precedenti. Le etichette di difficoltà non vanno reinserite.

- Mantenere un ID stabile per l’elemento, anche se cambia il nome.
- Per le funzioni future usare `status: planned`: non dichiararle implementate.
- Quando si implementa una funzione compilare `implementedOn`, `updatedOn`, `implementationRef` e aggiungere una voce `history` con data, riferimento Git/release e nota del cambiamento. La pagina mostra «Implementato il GG/MM/AAAA · riferimento».
- Per modifiche successive mantenere la prima data di implementazione e aggiornare `updatedOn`, appendendo una voce a `history`.
- Prima del test del proprietario usare `candidate`, riferimento «develop · candidata 0.8.5-beta». Non indicare una release già pubblicata. Dopo approvazione si può usare il tag effettivo o un commit verificato, senza inventare hash.
- Per funzioni che richiedono ancora un aggiornamento del server usare `server-required` e dichiarare la dipendenza nella nota visibile.
- Aggiornare anche `ROADMAP.md` e il diario di progetto per i cambiamenti pertinenti.

Il controllo `node tools/check-roadmap.mjs` verifica metadati, coerenza italiano/inglese e storico delle modifiche rispetto a HEAD. È eseguito anche prima della build del sito. Non pubblicare la candidata né aggiornare il server reale senza l’autorizzazione del proprietario.
