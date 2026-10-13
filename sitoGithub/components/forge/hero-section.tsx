"use client"

import { Download } from "lucide-react"
import { useReleaseDownload } from "./release-download"
import { useI18n } from "@/lib/i18n"

export function HeroSection() {
  const { t } = useI18n()
  const downloadUrl = useReleaseDownload()
  return (
    <section className="px-6 pb-[90px] pt-[110px] text-center">
      <div className="mx-auto w-full max-w-[1120px]">
        <span className="mb-7 inline-flex items-center gap-2 rounded-full border border-forge-cyan-dim bg-forge-cyan/5 px-3.5 py-1.5 text-[13px] tracking-[0.04em] text-forge-cyan">
          <span className="size-[7px] rounded-full bg-forge-cyan glow-cyan" aria-hidden="true" />
          {t.hero.badge}
        </span>

        <h1 className="mx-auto mb-6 max-w-[16ch] text-balance text-[clamp(44px,8vw,86px)] font-extrabold leading-[1.02] tracking-[-0.02em]">
          {t.hero.title.split('\n')[0]}{" "}
          <span className="text-forge-cyan text-glow-cyan">{t.hero.title.split('\n')[1]}</span>
        </h1>

        <p className="mx-auto mb-10 max-w-[600px] text-pretty text-[clamp(16px,2.4vw,19px)] text-forge-muted">
          {t.hero.subtitle}
        </p>

        <div id="download" className="flex flex-wrap items-center justify-center gap-4">
          <a
            href={downloadUrl}
            className="inline-flex items-center gap-2.5 rounded-full bg-forge-cyan px-6 py-3.5 text-[15px] font-semibold text-[#001417] glow-cyan transition-all hover:-translate-y-0.5 hover:glow-cyan-strong"
            aria-label={t.hero.ctaPrimary}
          >
            <Download className="size-5" aria-hidden="true" />
            {t.hero.ctaPrimary}
          </a>
          <a
            href="https://github.com/Leonardo-Lalli/FORGE"
            target="_blank"
            rel="noopener"
            className="inline-flex items-center gap-2.5 rounded-full border border-forge-border px-6 py-3.5 text-[15px] font-semibold text-forge-fg transition-colors hover:border-forge-cyan hover:text-forge-cyan"
          >
            {t.hero.ctaSecondary}
          </a>
        </div>

        <p className="mx-auto mt-6 max-w-[680px] rounded-xl border border-forge-border bg-forge-elev p-4 text-left text-xs leading-relaxed text-forge-muted">{t.hero.updateNote}</p>

        <div className="mx-auto mt-10 max-w-[680px] rounded-[14px] border border-forge-border bg-forge-elev p-6 text-left"><h2 className="mb-3 text-lg font-bold">{t.story.title}</h2><p className="text-sm leading-relaxed text-forge-muted">{t.story.body}</p></div>
        <p className="mt-[22px] font-mono text-[13px] text-forge-dim">
          {t.hero.note}
        </p>
      </div>
    </section>
  )
}
