"use client"
import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { CheckCircle2 } from "lucide-react"
export default function RoadmapPage() {
 const {t}=useI18n(); const r=t.roadmapPage;
 return <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
 <div className="pointer-events-none fixed inset-0 forge-ambient" aria-hidden="true"/><div className="pointer-events-none fixed inset-0 forge-grid" aria-hidden="true"/>
 <div className="relative z-10"><SiteHeader/><main className="px-6 py-20"><div className="mx-auto max-w-[1120px]">
 <p className="mb-4 font-mono text-xs uppercase tracking-widest text-forge-cyan">{r.eyebrow}</p><h1 className="mb-5 text-5xl font-extrabold">{r.title}</h1><p className="mb-12 max-w-[640px] text-lg text-forge-muted">{r.subtitle}</p>
 <div className="grid gap-6 md:grid-cols-3">{r.releases.map(rel=><article key={rel.version} className="rounded-[14px] border border-forge-border bg-forge-elev p-7"><p className="mb-3 font-mono text-sm text-forge-cyan">{rel.version}</p><h2 className="mb-6 text-xl font-bold">{rel.theme}</h2><ul className="space-y-5">{rel.features.map(f=><li key={f} className="flex gap-3 text-sm leading-relaxed text-forge-muted"><CheckCircle2 className="mt-1 size-4 shrink-0 text-forge-cyan"/>{f}</li>)}</ul></article>)}</div>
 </div></main><SiteFooter/></div></div>
}
