"use client"

import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { Sparkles } from "lucide-react"

function Eyebrow({ children }: { children: React.ReactNode }) {
  return <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
}

function Card({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`rounded-[14px] border border-forge-border bg-forge-elev p-6 ${className}`}>{children}</div>
}

type ImplementationInfo = { implementedOn: string; updatedOn: string; implementationRef: string; implementationNote: string; history: readonly { ref: string }[] }
function ImplementationMeta({ item, lang }: { item: ImplementationInfo; lang: string }) {
  const date = (value: string) => value.split("-").reverse().join("/")
  return <>
    {item.implementedOn && <p className="mt-2 text-xs text-forge-cyan">{lang === "it" ? "Implementato il" : "Implemented on"} {date(item.implementedOn)} · {item.implementationRef}</p>}
    {item.updatedOn && item.updatedOn !== item.implementedOn && <p className="mt-1 text-xs text-forge-muted">{lang === "it" ? "Aggiornato il" : "Updated on"} {date(item.updatedOn)} · {item.history.at(-1)?.ref}</p>}
    {item.implementationNote && <p className="mt-1 text-xs text-forge-muted">{item.implementationNote}</p>}
  </>
}

export default function RoadmapPage() {
  const { t, lang } = useI18n()
  const r = t.roadmapPage


  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main>
          <section className="px-6 pb-16 pt-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <div className="mb-12 max-w-[640px]">
                <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{r.eyebrow}</p>
                <h1 className="mb-4 text-balance text-[clamp(30px,5vw,44px)] font-extrabold leading-[1.1] tracking-[-0.02em]">{r.title}</h1>
                <p className="text-pretty text-[17px] text-forge-muted">{r.subtitle}</p>
              </div>

              {/* Release timeline */}
              <div className="relative pl-6 sm:pl-8">
                <div className="absolute bottom-2 left-[7px] top-2 w-px bg-forge-border sm:left-[9px]" aria-hidden="true" />
                <div className="flex flex-col gap-10">
                  {r.releases.map((rel, ri) => (
                    <div key={rel.theme} className="relative">
                      <span className="absolute -left-6 top-2 grid size-4 place-items-center rounded-full border border-forge-cyan bg-forge-bg sm:-left-8" aria-hidden="true">
                        <span className="size-1.5 rounded-full bg-forge-cyan glow-cyan" />
                      </span>
                      <div className="flex flex-wrap items-baseline gap-3 mb-2">
                        <span className="rounded-full border border-forge-border px-2.5 py-0.5 text-xs text-forge-muted">{rel.theme}</span>
                      </div>
                      <div className="mt-4 space-y-3">
                        {rel.features.map((f, fi) => {
                          return (
                            <div key={f.id} className="flex items-start gap-3 rounded-lg border border-forge-border/60 bg-forge-elev p-4">
                              <Sparkles className="mt-0.5 size-4 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
                              <div className="flex-1 min-w-0">
                                <p className="text-[15px] font-semibold text-forge-fg">{f.name}</p>
                                <p className="text-[13px] text-forge-muted mt-0.5">{f.desc}</p>
                                <ImplementationMeta item={f} lang={lang} />
                              </div>
                            </div>
                          )
                        })}
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Summary phases */}
              <div className="mt-20">
                <p className="mb-4 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{r.phasesEyebrow}</p>
                <h2 className="mb-8 text-[clamp(26px,4vw,36px)] font-extrabold">{r.phasesTitle}</h2>
                <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
                  {r.phases.map((p, i) => (
                    <Card key={i}>
                      <span className="mb-3 inline-block rounded-full border border-forge-cyan-dim bg-forge-cyan/[0.06] px-3 py-1 font-mono text-xs text-forge-cyan">{p.tag}</span>
                      <h3 className="mb-2 text-lg font-bold">{p.title}</h3>
                      <p className="text-[14px] leading-relaxed text-forge-muted">{p.body}</p>
                    </Card>
                  ))}
                </div>
              </div>

              {/* Backlog */}
              <div className="mt-20">
                <p className="mb-4 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{r.backlog.eyebrow}</p>
                <h2 className="mb-8 text-[clamp(26px,4vw,36px)] font-extrabold">{r.backlog.title}</h2>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  {r.backlog.items.map((item, i) => (
                    <Card key={item.id}>
                      <div className="flex items-start gap-3">
                        <span className="mt-0.5 text-forge-cyan" aria-hidden="true">▸</span>
                        <div>
                          <p className="text-[15px] font-semibold">{item.name}</p>
                          <p className="text-[13px] text-forge-muted mt-0.5">{item.desc}</p>
                          <ImplementationMeta item={item} lang={lang} />
                        </div>
                      </div>
                    </Card>
                  ))}
                </div>
              </div>

              <p className="mt-20 text-center font-mono text-sm text-forge-cyan">{r.closing}</p>
            </div>
          </section>
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
