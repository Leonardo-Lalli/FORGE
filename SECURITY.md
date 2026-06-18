# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| v0.8.0-beta | ✅ |
| < v0.8.0 | ❌ |

## Reporting a Vulnerability

Se trovi una vulnerabilità di sicurezza, **non** aprire una issue pubblica.

Invia una email a `security@leoforge.duckdns.org` con:
- Descrizione della vulnerabilità
- Passi per riprodurla
- Impatto potenziale

Risponderò entro 48 ore con un piano di fix.

## Security Measures

FORGE adotta le seguenti misure di sicurezza:

- **Autenticazione**: password cifrate con SecureStorage (Android Keystore)
- **HTTPS**: tutte le comunicazioni sono cifrate (Let's Encrypt)
- **API**: regole row-level ownership (ogni utente vede solo i propri dati)
- **Rate limiting**: 5 tentativi di login al minuto
- **Admin panel**: bloccato da accesso esterno (403)
- **Backup**: `android:allowBackup=false`

Per dettagli tecnici, vedi [`docs/security-hardening.md`](docs/security-hardening.md).
