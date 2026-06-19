import { Download } from "lucide-react"

export function HeroSection() {
  return (
    <section className="px-6 pb-[90px] pt-[110px] text-center">
      <div className="mx-auto w-full max-w-[1120px]">
        <span className="mb-7 inline-flex items-center gap-2 rounded-full border border-forge-cyan-dim bg-forge-cyan/5 px-3.5 py-1.5 text-[13px] tracking-[0.04em] text-forge-cyan">
          <span className="size-[7px] rounded-full bg-forge-cyan glow-cyan" aria-hidden="true" />
          Open-source · Privacy-first · Self-hostable
        </span>

        <h1 className="mx-auto mb-6 max-w-[16ch] text-balance text-[clamp(44px,8vw,86px)] font-extrabold leading-[1.02] tracking-[-0.02em]">
          Forgia la tua forza.{" "}
          <span className="text-forge-cyan text-glow-cyan">I tuoi dati restano tuoi.</span>
        </h1>

        <p className="mx-auto mb-10 max-w-[600px] text-pretty text-[clamp(16px,2.4vw,19px)] text-forge-muted">
          FORGE è l&apos;app di palestra open-source per tracciare volumi, gestire i recuperi e progredire ad ogni
          sessione — senza account cloud, senza pubblicità, senza compromessi.
        </p>

        <div id="download" className="flex flex-wrap items-center justify-center gap-4">
          <a
            href="https://github.com/Leonardo-Lalli/FORGE/releases/latest/download/FORGE.apk"
            className="inline-flex items-center gap-2.5 rounded-full bg-forge-cyan px-6 py-3.5 text-[15px] font-semibold text-[#001417] glow-cyan transition-all hover:-translate-y-0.5 hover:glow-cyan-strong"
            aria-label="Scarica l'APK per Android"
          >
            <Download className="size-5" aria-hidden="true" />
            Scarica l&apos;APK Android
          </a>
          <a
            href="https://github.com/Leonardo-Lalli/FORGE"
            target="_blank"
            rel="noopener"
            className="inline-flex items-center gap-2.5 rounded-full border border-forge-border px-6 py-3.5 text-[15px] font-semibold text-forge-fg transition-colors hover:border-forge-cyan hover:text-forge-cyan"
          >
            Configura il backend
          </a>
        </div>

        <p className="mt-[22px] font-mono text-[13px] text-forge-dim">
          v1.0.0 · APK ~9 MB · Android 8.0+ · Licenza MIT
        </p>
      </div>
    </section>
  )
}
