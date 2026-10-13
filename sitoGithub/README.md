# Sito FORGE

Sito ufficiale Next.js con esportazione statica su GitHub Pages, due temi e testi in italiano/inglese.

## Percorsi

- `/`: presentazione dell’app, schermate reali, achievement, community, download guidato e FAQ.
- `/features`: funzioni della beta, limiti e confronto con app simili.
- `/b2b`: visione per le palestre, funzioni attuali e strumenti gestionali pianificati.
- `/roadmap`: timeline degli obiettivi e AI form check negli esperimenti, senza date/versioni future.
- `/docs`: origine, specifiche, architettura e diario.
- `/self-hosting`: installazione e manutenzione del proprio backend.

B2B e roadmap sono pagine distinte. Nessuna sezione presenta le funzioni future come già disponibili.

## Sviluppo

Da questa cartella: `npm ci`, poi `npm run dev`. Per l’esportazione destinata a GitHub Pages impostare `NEXT_PUBLIC_BASE_PATH=/FORGE` e usare `npm run build`; i file sono in `out/`.

Testi: `lib/translations/it.ts` e `en.ts`. Layout e componenti: `app/` e `components/forge/`. Immagini: `public/`; la foto workout è un’acquisizione recente del proprietario, le altre immagini sono archiviate e dichiarate come tali.

Il download usa l’APK della release corrente come fallback e controlla le release GitHub per trovare versioni successive, incluse le beta. Non caricare chiavi di firma o configurazioni Firebase nel sito.

## Pubblicazione

Il workflow `.github/workflows/deploy-pages.yml` compila ed esporta il sito su `gh-pages`. Si avvia manualmente o quando viene pubblicata una release; un push su main da solo non avvia il deploy.

Prima di pubblicare: verificare la build, la distinzione tra funzioni disponibili e pianificate, i link con prefisso `/FORGE`, navigazione mobile, temi e lingue. Le modifiche al sito non sostituiscono gli APK delle release.
