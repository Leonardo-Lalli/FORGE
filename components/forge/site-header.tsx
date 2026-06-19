import { Star } from "lucide-react"

export function SiteHeader() {
  return (
    <header className="sticky top-0 z-50 border-b border-forge-border bg-forge-bg/70 backdrop-blur-md">
      <div className="mx-auto flex h-[68px] w-full max-w-[1120px] items-center justify-between px-6">
        <a href="#top" className="flex items-center gap-3" aria-label="FORGE — home">
          {/* Spazio logo (placeholder): sostituisci con il tuo SVG/PNG */}
          <span
            className="grid size-9 flex-shrink-0 place-items-center rounded-[9px] border border-forge-cyan text-lg font-extrabold text-forge-cyan glow-cyan"
            aria-hidden="true"
          >
            F
          </span>
          <span className="text-xl font-extrabold tracking-[0.22em]">FORGE</span>
        </a>

        <nav className="flex items-center gap-8" aria-label="Navigazione principale">
          <a
            href="#funzionalita"
            className="hidden text-sm text-forge-muted transition-colors hover:text-forge-fg sm:inline"
          >
            Funzionalità
          </a>
          <a
            href="#self-hosting"
            className="hidden text-sm text-forge-muted transition-colors hover:text-forge-fg sm:inline"
          >
            Self-Hosting
          </a>
          <a
            href="#documentazione"
            className="hidden text-sm text-forge-muted transition-colors hover:text-forge-fg sm:inline"
          >
            Documentazione
          </a>
          <a
            href="#download"
            className="hidden text-sm text-forge-muted transition-colors hover:text-forge-fg sm:inline"
          >
            Download
          </a>
          <a
            href="https://github.com"
            target="_blank"
            rel="noopener"
            className="inline-flex items-center gap-2 rounded-full border border-forge-border px-4 py-2 text-sm text-forge-fg transition-all hover:border-forge-cyan hover:glow-cyan-soft"
          >
            GitHub
            <Star className="size-3.5" aria-hidden="true" />
          </a>
        </nav>
      </div>
    </header>
  )
}
