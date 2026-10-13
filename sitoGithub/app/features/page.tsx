"use client"

import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { CheckCircle2, Dumbbell, BarChart3, Users, Trophy, Shield, Palette, Database, FileSpreadsheet, Timer, Camera, Search } from "lucide-react"

function Eyebrow({ children }: { children: React.ReactNode }) {
  return <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
}

export default function FeaturesPage() {
  const { t } = useI18n()

  const features = t.featureDetails.map((f, i) => ({ ...f, icon: [Dumbbell, Timer, Camera, BarChart3, Users, Search, Shield, Trophy, Database][i], status: "BETA" }))

  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main>
          <section className="px-6 pb-16 pt-20">
            <div className="mx-auto w-full max-w-[1120px]">
              <p className="mb-4 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Funzionalità</p>
              <h1 className="mb-6 max-w-[700px] text-balance text-[clamp(32px,6vw,52px)] font-extrabold leading-[1.08] tracking-[-0.02em]">
                {t.features.title}{" "}
                <span className="text-forge-cyan text-glow-cyan"></span>
              </h1>
              <p className="max-w-[600px] text-[18px] leading-relaxed text-forge-muted">
                {t.features.subtitle}
              </p>
            </div>
          </section>

          <section className="px-6 py-12">
            <div className="mx-auto w-full max-w-[1120px]">
              <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                {features.map((f, i) => (
                  <div key={i} className="rounded-[14px] border border-forge-border bg-forge-elev p-6 transition-all hover:-translate-y-0.5 hover:border-forge-cyan-dim">
                    <div className="flex items-start gap-4">
                      <div className="grid size-10 flex-shrink-0 place-items-center rounded-xl border border-forge-cyan-dim bg-forge-cyan/[0.06]">
                        <f.icon className="size-5 text-forge-cyan" aria-hidden="true" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-1">
                          <h3 className="text-base font-bold">{f.title}</h3>
                          <span className="text-xs">{f.status}</span>
                        </div>
                        <p className="text-[14px] leading-relaxed text-forge-muted">{f.desc}</p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </section>

        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
