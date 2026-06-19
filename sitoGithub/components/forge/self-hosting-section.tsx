"use client"

import { CheckCircle2 } from "lucide-react"
import { useI18n } from "@/lib/i18n"

export function SelfHostingSection() {
  const { t } = useI18n()
  return (
    <section id="self-hosting" className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="grid grid-cols-1 gap-12 lg:grid-cols-2 lg:items-center">
          <div>
            <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{t.selfHosting.eyebrow}</p>
            <h2 className="mb-4 text-balance text-[clamp(28px,5vw,40px)] font-extrabold leading-[1.1] tracking-[-0.02em]">{t.selfHosting.title}</h2>
            <p className="mb-8 max-w-[480px] text-pretty text-[17px] text-forge-muted">{t.selfHosting.subtitle}</p>
            <div className="flex flex-col gap-3 mb-8">
              {t.selfHosting.steps.map((s, i) => (
                <div key={i} className="flex items-center gap-3">
                  <CheckCircle2 className="size-5 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
                  <span className="text-[15px] text-forge-fg">{s}</span>
                </div>
              ))}
            </div>
            <p className="text-[13px] text-forge-dim">{t.selfHosting.note}</p>
          </div>
          <div className="rounded-[14px] border border-forge-border bg-forge-code p-5 font-mono text-[13px] leading-[1.8] text-forge-code-fg overflow-x-auto">
            <p className="mb-1 text-forge-dim">{t.selfHosting.terminalTitle}</p>
            <p className="text-forge-dim">{t.selfHosting.terminalComment}</p>
            <p className="mt-3">
              <span className="text-forge-cyan">git clone</span> https://github.com/Leonardo-Lalli/FORGE.git
            </p>
            <p>
              <span className="text-forge-cyan">cd</span> FORGE
            </p>
            <p className="mt-2">
              <span className="text-forge-cyan">docker compose</span> up -d
            </p>
          </div>
        </div>
      </div>
    </section>
  )
}
