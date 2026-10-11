# Organizzazione del progetto

- `src/Forge`: codice MAUI, ViewModels, servizi e modelli.
- `src/Forge/Resources`: sole risorse runtime (icone, badge, font, palette e splash).
- `assets/design-reference`: mockup HTML/PNG e documenti di design, spostati fuori dal progetto compilato.
- `assets/AppLogo` e `assets/AchievementPalestra`: originali grafici; l'icona attuale con freccia rimane invariata.
- `tests/Forge.Tests`: test automatici, senza accesso al server pubblico.
- `tools`: installazione backend e configurazioni Nginx.
- `docs`: requisiti, metodo, iterazioni e verifiche.

`bin`, `obj`, `artifacts` e i file `.user` sono output/configurazioni locali ignorati da Git. Non devono essere spostati nei sorgenti. I file `.env` non vengono inclusi nell'app.
