import { Download } from "lucide-react"

export function CtaBand() {
  return (
    <section className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="rounded-[20px] border border-forge-cyan-dim bg-gradient-to-b from-forge-cyan/[0.06] to-forge-cyan/[0.01] px-6 py-16 text-center glow-cyan-soft">
          <h2 className="mb-3.5 text-balance text-[clamp(28px,5vw,40px)] font-extrabold tracking-[-0.02em]">
            Inizia ad allenarti con FORGE oggi.
          </h2>
          <p className="mx-auto mb-8 max-w-[460px] text-forge-muted">
            Gratis, open-source e per sempre tuo. Scarica l&apos;app e prendi il controllo dei tuoi progressi.
          </p>
          <a
            href="#"
            className="inline-flex items-center gap-2.5 rounded-full bg-forge-cyan px-6 py-3.5 text-[15px] font-semibold text-[#001417] glow-cyan transition-all hover:-translate-y-0.5 hover:glow-cyan-strong"
            aria-label="Scarica l'APK per Android"
          >
            <Download className="size-5" aria-hidden="true" />
            Scarica l&apos;APK Android
          </a>
        </div>
      </div>
    </section>
  )
}

export function SiteFooter() {
  return (
    <footer className="mt-10 border-t border-forge-border px-6 py-10">
      <div className="mx-auto flex w-full max-w-[1120px] flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <span
            className="grid size-9 flex-shrink-0 place-items-center rounded-[9px] border border-forge-cyan text-lg font-extrabold text-forge-cyan glow-cyan"
            aria-hidden="true"
          >
            F
          </span>
          <span className="text-base font-extrabold tracking-[0.22em]">FORGE</span>
        </div>

        <p className="text-[13px] text-forge-dim">Costruito con dedizione · Licenza MIT · © 2026 FORGE</p>

        <nav className="flex gap-6" aria-label="Link footer">
          <a
            href="https://github.com"
            target="_blank"
            rel="noopener"
            className="text-[13px] text-forge-muted transition-colors hover:text-forge-cyan"
          >
            GitHub
          </a>
          <a href="#funzionalita" className="text-[13px] text-forge-muted transition-colors hover:text-forge-cyan">
            Funzionalità
          </a>
          <a href="#self-hosting" className="text-[13px] text-forge-muted transition-colors hover:text-forge-cyan">
            Self-Hosting
          </a>
        </nav>
      </div>
    </footer>
  )
}
