"use client"

import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { CheckCircle2, Circle } from "lucide-react"

export default function RoadmapPage() {
  const { t } = useI18n()
  const r = t.roadmapPage

  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main className="px-6 py-20">
          <div className="mx-auto max-w-[1120px]">
            <p className="mb-4 font-mono text-xs uppercase tracking-widest text-forge-cyan">{r.eyebrow}</p>
            <h1 className="mb-5 text-4xl font-extrabold sm:text-5xl">{r.title}</h1>
            <p className="mb-12 max-w-[760px] text-lg text-forge-muted">{r.subtitle}</p>
            <section aria-labelledby="planned-heading">
              <h2 id="planned-heading" className="mb-3 text-2xl font-bold">{r.plannedTitle}</h2>
              <p className="mb-8 text-forge-muted">{r.plannedNote}</p>
              <div className="grid items-start gap-6 md:grid-cols-2 lg:grid-cols-3">
                {r.groups.map(group => (
                  <article key={group.theme} className="rounded-[14px] border border-forge-border bg-forge-elev p-6">
                    <h3 className="mb-6 text-xl font-bold">{group.theme}</h3>
                    <ul className="space-y-5">
                      {group.features.map(feature => (
                        <li key={feature.name} className="flex items-start gap-3">
                          <Circle className="mt-1 size-4 shrink-0 text-forge-muted" aria-hidden="true" />
                          <div>
                            <p className="mb-1 text-sm font-semibold">{feature.name}</p>
                            <p className="text-sm leading-relaxed text-forge-muted">{feature.desc}</p>
                          </div>
                        </li>
                      ))}
                    </ul>
                  </article>
                ))}
              </div>
            </section>
            <section className="mt-16" aria-labelledby="completed-heading">
              <h2 id="completed-heading" className="mb-3 text-2xl font-bold">{r.completedTitle}</h2>
              <p className="mb-8 text-forge-muted">{r.completedNote}</p>
              {r.completed.map(release => (
                <article key={release.version} className="rounded-[14px] border border-forge-border bg-forge-elev p-6">
                  <p className="mb-6 font-mono text-sm text-forge-cyan">{release.version} · {release.date}</p>
                  <ul className="grid gap-4 md:grid-cols-2">
                    {release.features.map(feature => (
                      <li key={feature} className="flex gap-3 text-sm leading-relaxed text-forge-muted">
                        <CheckCircle2 className="mt-1 size-4 shrink-0 text-forge-cyan" aria-hidden="true" />
                        {feature}
                      </li>
                    ))}
                  </ul>
                </article>
              ))}
            </section>
          </div>
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
