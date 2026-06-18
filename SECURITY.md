# Security Policy

## ⚠️ Progetto Scolastico

FORGE è un progetto didattico amatoriale sviluppato come parte di un percorso di studi in informatica. Non è un prodotto commerciale né un servizio professionale. Presenta limitazioni note (vedi sotto) e non ha pretese di sicurezza enterprise.

## Limitazioni Note

- **Nessun team di sicurezza dedicato**: le vulnerabilità vengono gestite dallo sviluppatore nel tempo libero
- **Nessun bug bounty**: non offriamo ricompense per la segnalazione di bug
- **Server self-hosted**: il backend gira su hardware casalingo, non garantisce uptime 24/7
- **Nessuna certificazione di sicurezza**: l'app non è stata sottoposta a penetration test professionale

## Supported Versions

| Version | Supported |
|---------|-----------|
| v0.8.0-beta | ✅ |
| < v0.8.0 | ❌ |

## Reporting a Vulnerability

Se trovi una vulnerabilità di sicurezza:

1. **Non** aprire una issue pubblica
2. Contatta lo sviluppatore tramite i contatti GitHub (apri una issue privata o usa la sezione Discussions)
3. Descrivi: vulnerabilità, passi per riprodurla, impatto potenziale

Risponderò appena possibile, compatibilmente con gli impegni scolastici.

## Security Measures Adottate

FORGE adotta comunque le seguenti misure di sicurezza:

- **Autenticazione**: password cifrate con SecureStorage (Android Keystore)
- **HTTPS**: tutte le comunicazioni sono cifrate (Let's Encrypt)
- **API**: regole row-level ownership (ogni utente vede solo i propri dati)
- **Rate limiting**: 5 tentativi di login al minuto
- **Admin panel**: bloccato da accesso esterno (403)
- **Backup Android**: disabilitato (`allowBackup=false`)

Per dettagli tecnici, vedi [`docs/security-hardening.md`](docs/security-hardening.md).
