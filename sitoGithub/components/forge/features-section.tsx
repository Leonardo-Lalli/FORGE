"use client"

import { useI18n } from "@/lib/i18n"

export function FeaturesSection() {
  const { t } = useI18n()
  return (
    <section id="funzionalita" className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="mb-14 max-w-[600px]">
          <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{t.features.eyebrow}</p>
          <h2 className="mb-4 text-balance text-[clamp(28px,5vw,40px)] font-extrabold leading-[1.1] tracking-[-0.02em]">{t.features.title}</h2>
          <p className="text-pretty text-[17px] text-forge-muted">{t.features.subtitle}</p>
        </div>
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {t.features.items.map((item, i) => (
            <div key={i} className="rounded-[14px] border border-forge-border bg-forge-elev p-6 transition-all hover:-translate-y-0.5 hover:border-forge-cyan-dim">
              <h3 className="mb-2 text-lg font-bold">{item.title}</h3>
              <p className="text-[15px] leading-relaxed text-forge-muted">{item.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}
