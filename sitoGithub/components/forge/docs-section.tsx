"use client"

import { useState } from "react"
import { BookText, Boxes, GitBranch, ScrollText, CheckCircle2, CircleDashed, XCircle, AlertTriangle } from "lucide-react"
import type { LucideIcon } from "lucide-react"
import { useI18n } from "@/lib/i18n"

type TabId = "spec" | "architettura" | "roadmap" | "diario"
type Status = "done" | "partial" | "skip"

function Eyebrow({ children }: { children: React.ReactNode }) {
  return <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
}

function Card({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`rounded-[14px] border border-forge-border bg-forge-elev p-6 ${className}`}>{children}</div>
}

const statusMeta: Record<Status, { label: string; labelEn: string; icon: LucideIcon; cls: string }> = {
  done: { label: "Completata", labelEn: "Completed", icon: CheckCircle2, cls: "text-forge-cyan border-forge-cyan-dim bg-forge-cyan/[0.06]" },
  partial: { label: "Parziale", labelEn: "Partial", icon: AlertTriangle, cls: "text-amber-300 border-amber-300/25 bg-amber-300/[0.06]" },
  skip: { label: "Posticipata", labelEn: "Deferred", icon: XCircle, cls: "text-forge-dim border-forge-border bg-forge-elev" },
}

export function DocsSection() {
  const { t, lang } = useI18n()
  const [active, setActive] = useState<TabId>("spec")
  const tabs: { id: TabId; label: string; icon: LucideIcon }[] = [
    { id: "spec", label: t.docs.tabs.spec, icon: BookText },
    { id: "architettura", label: t.docs.tabs.architettura, icon: Boxes },
    { id: "roadmap", label: t.docs.tabs.roadmap, icon: GitBranch },
    { id: "diario", label: t.docs.tabs.diario, icon: ScrollText },
  ]

  return (
    <section id="documentazione" className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="mb-12 max-w-[640px]">
          <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">{t.docs.eyebrow}</p>
          <h2 className="mb-4 text-balance text-[clamp(30px,5vw,44px)] font-extrabold leading-[1.1] tracking-[-0.02em]">{t.docs.title}</h2>
          <p className="text-pretty text-[17px] text-forge-muted">{t.docs.subtitle}</p>
        </div>

        <div className="mb-8 flex flex-wrap gap-2 border-b border-forge-border" role="tablist">
          {tabs.map(({ id, label, icon: Icon }) => {
            const isActive = active === id
            return (
              <button key={id} role="tab" type="button" aria-selected={isActive} onClick={() => setActive(id)}
                className={`-mb-px inline-flex items-center gap-2 border-b-2 px-4 py-3 text-sm font-medium transition-colors ${isActive ? "border-forge-cyan text-forge-cyan" : "border-transparent text-forge-muted hover:text-forge-fg"}`}>
                <Icon className="size-4" aria-hidden="true" />
                {label}
              </button>
            )
          })}
        </div>

        <div>
          {active === "spec" && <SpecPanel t={t.docs.spec} />}
          {active === "architettura" && <ArchPanel t={t.docs.arch} />}
          {active === "roadmap" && <RoadmapPanel t={t.docs.roadmap} lang={lang} />}
          {active === "diario" && <DiarioPanel t={t.docs.journal} />}
        </div>
      </div>
    </section>
  )
}

function SpecPanel({ t }: { t: any }) {
  return (
    <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
      <Card>
        <Eyebrow>{t.visionEyebrow}</Eyebrow>
        <h3 className="mb-2.5 text-lg font-bold">{t.visionTitle}</h3>
        <p className="text-[15px] leading-relaxed text-forge-muted">{t.visionBody}</p>
        <div className="mt-5">
          <Eyebrow>{t.targetEyebrow}</Eyebrow>
          <ul className="flex flex-col gap-2.5">
            {t.targetUsers.map((u: string) => (
              <li key={u} className="flex items-start gap-2.5 text-[14px] text-forge-muted">
                <span className="mt-0.5 flex-shrink-0 text-forge-cyan" aria-hidden="true">▸</span>
                <span>{u}</span>
              </li>
            ))}
          </ul>
        </div>
      </Card>
      <Card>
        <Eyebrow>{t.mvpEyebrow}</Eyebrow>
        <h3 className="mb-3.5 text-lg font-bold">{t.mvpTitle}</h3>
        <ul className="flex flex-col gap-2.5">
          {t.mvpFeatures.map((f: string) => (
            <li key={f} className="flex items-start gap-2.5 text-[14px] text-forge-muted">
              <CheckCircle2 className="mt-0.5 size-4 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
              <span>{f}</span>
            </li>
          ))}
        </ul>
      </Card>
      <Card className="lg:col-span-2">
        <Eyebrow>{t.reqEyebrow}</Eyebrow>
        <div className="grid grid-cols-1 gap-x-8 gap-y-3 sm:grid-cols-2">
          {t.requirements.map((r: any) => (
            <div key={r.id} className="flex items-start gap-3 border-b border-forge-border/60 pb-3">
              {r.done ? <CheckCircle2 className="mt-0.5 size-4 flex-shrink-0 text-forge-cyan" aria-hidden="true" /> : <CircleDashed className="mt-0.5 size-4 flex-shrink-0 text-forge-dim" aria-hidden="true" />}
              <div>
                <span className="font-mono text-xs text-forge-cyan">{r.id}</span>
                <p className={`text-[14px] ${r.done ? "text-forge-muted" : "text-forge-dim line-through"}`}>{r.text}</p>
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  )
}

function ArchPanel({ t }: { t: any }) {
  return (
    <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
      <Card>
        <Eyebrow>{t.stackEyebrow}</Eyebrow>
        <div className="flex flex-col">
          {t.stack.map((s: any, i: number) => (
            <div key={s.tech} className={`grid grid-cols-[auto_1fr] items-baseline gap-x-4 py-2.5 ${i !== t.stack.length - 1 ? "border-b border-forge-border/60" : ""}`}>
              <span className="font-mono text-[13px] font-semibold text-forge-fg">{s.tech}</span>
              <span className="text-[13px] text-forge-muted">{s.role}</span>
            </div>
          ))}
        </div>
      </Card>
      <div className="flex flex-col gap-5">
        <Card>
          <Eyebrow>{t.repoEyebrow}</Eyebrow>
          <pre className="overflow-x-auto rounded-lg bg-forge-code p-4 font-mono text-[12.5px] leading-[1.7] text-forge-muted"><code className="whitespace-pre">{t.repoTree}</code></pre>
        </Card>
        <Card>
          <Eyebrow>{t.navEyebrow}</Eyebrow>
          <div className="flex flex-wrap gap-2">
            {t.navTabs.map((tab: string) => (
              <span key={tab} className="rounded-full border border-forge-cyan-dim bg-forge-cyan/[0.06] px-3 py-1 font-mono text-xs text-forge-cyan">{tab}</span>
            ))}
            {t.navRoutes.map((r: string) => (
              <span key={r} className="rounded-full border border-forge-border px-3 py-1 font-mono text-xs text-forge-muted">{r}</span>
            ))}
          </div>
        </Card>
      </div>
      <Card className="lg:col-span-2">
        <Eyebrow>{t.collEyebrow}</Eyebrow>
        <div className="overflow-x-auto">
          <table className="w-full border-collapse text-left">
            <thead><tr className="border-b border-forge-border"><th className="py-2.5 pr-4 font-mono text-xs uppercase tracking-wider text-forge-dim">{t.collHeadName}</th><th className="py-2.5 font-mono text-xs uppercase tracking-wider text-forge-dim">{t.collHeadFields}</th></tr></thead>
            <tbody>
              {t.collections.map((c: any) => (
                <tr key={c.name} className="border-b border-forge-border/60"><td className="py-2.5 pr-4 align-top font-mono text-[13px] font-semibold text-forge-cyan">{c.name}</td><td className="py-2.5 font-mono text-[13px] text-forge-muted">{c.fields}</td></tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  )
}

function RoadmapPanel({ t, lang }: { t: any; lang: string }) {
  return (
    <div className="flex flex-col gap-3">
      {t.iterations.map((it: any) => {
        const meta = statusMeta[it.status as Status]
        const Icon = meta.icon
        return (
          <div key={it.id} className="flex flex-col gap-3 rounded-[14px] border border-forge-border bg-forge-elev p-5 sm:flex-row sm:items-center sm:justify-between">
            <div className="flex items-start gap-4">
              <span className="font-mono text-[13px] font-semibold text-forge-cyan sm:w-32 sm:flex-shrink-0">{it.id}</span>
              <span className="text-[15px] text-forge-fg">{it.goal}</span>
            </div>
            <span className={`inline-flex w-fit flex-shrink-0 items-center gap-1.5 rounded-full border px-3 py-1 text-xs font-medium ${meta.cls}`}>
              <Icon className="size-3.5" aria-hidden="true" />
              {lang === "en" ? meta.labelEn : meta.label}
            </span>
          </div>
        )
      })}
    </div>
  )
}

function DiarioPanel({ t }: { t: any }) {
  return (
    <div className="relative pl-6 sm:pl-8">
      <div className="absolute bottom-2 left-[7px] top-2 w-px bg-forge-border sm:left-[9px]" aria-hidden="true" />
      <div className="flex flex-col gap-7">
        {t.map((entry: any) => (
          <div key={entry.phase} className="relative">
            <span className="absolute -left-6 top-1 grid size-4 place-items-center rounded-full border border-forge-cyan bg-forge-bg sm:-left-8" aria-hidden="true"><span className="size-1.5 rounded-full bg-forge-cyan glow-cyan" /></span>
            <p className="mb-1 font-mono text-xs uppercase tracking-[0.14em] text-forge-cyan">{entry.phase}</p>
            <h3 className="mb-1.5 text-lg font-bold">{entry.title}</h3>
            <p className="text-[15px] leading-relaxed text-forge-muted">{entry.body}</p>
          </div>
        ))}
      </div>
    </div>
  )
}
