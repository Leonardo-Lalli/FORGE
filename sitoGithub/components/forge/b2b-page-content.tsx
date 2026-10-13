"use client"

import Link from "next/link"
import { ArrowRight, Building2, GitBranch, Check } from "lucide-react"
import { useI18n } from "@/lib/i18n"

export function B2BPageContent() {
  const { t } = useI18n()
  const b = t.b2b
  const [first, second] = b.title.split("\n")
  return (
    <main className="px-6 py-16 sm:py-20">
      <div className="mx-auto max-w-[1120px]">
        <div className="grid items-center gap-10 lg:grid-cols-[1.1fr_0.9fr]">
          <div>
            <p className="mb-5 text-xs font-semibold uppercase tracking-widest text-forge-cyan">{b.eyebrow}</p>
            <h1 className="mb-6 text-balance text-4xl font-extrabold leading-tight tracking-tight sm:text-6xl">{first}<br /><span className="text-forge-cyan">{second}</span></h1>
            <p className="mb-6 max-w-[600px] text-lg leading-relaxed text-forge-muted">{b.body}</p>
            <span className="inline-block rounded-full border border-forge-cyan-dim bg-forge-cyan/[0.06] px-3 py-1.5 text-xs font-medium text-forge-cyan">{b.status}</span>
          </div>
          <aside className="rounded-3xl border border-forge-border bg-forge-elev p-8">
            <Building2 className="mb-6 size-10 text-forge-cyan" aria-hidden="true" />
            <h2 className="mb-4 text-2xl font-bold">{b.currentTitle}</h2>
            <p className="mb-6 leading-relaxed text-forge-muted">{b.currentBody}</p>
            <Link href="/#download" className="inline-flex items-center gap-2 text-sm font-semibold text-forge-cyan">{b.appLink}<ArrowRight className="size-4" aria-hidden="true" /></Link>
          </aside>
        </div>
        <section className="mt-16 grid gap-5 md:grid-cols-2" aria-label={b.currentTitle}>
          {b.advantages.map(item => (
            <article key={item.title} className="rounded-2xl border border-forge-border bg-forge-elev p-6">
              <Check className="mb-4 size-5 text-forge-cyan" aria-hidden="true" />
              <h3 className="mb-2 text-lg font-bold">{item.title}</h3>
              <p className="text-sm leading-relaxed text-forge-muted">{item.desc}</p>
            </article>
          ))}
        </section>
        <section className="mt-16">
          <h2 className="mb-4 text-3xl font-bold">{b.futureTitle}</h2>
          <p className="mb-8 max-w-[760px] leading-relaxed text-forge-muted">{b.note}</p>
          <div className="grid gap-5 md:grid-cols-2">
            {b.future.map(item => (
              <article key={item.title} className="flex items-start gap-4 rounded-2xl border border-forge-border bg-forge-elev p-6">
                <GitBranch className="mt-1 size-5 shrink-0 text-forge-cyan" aria-hidden="true" />
                <div><h3 className="mb-2 font-semibold">{item.title}</h3><p className="text-sm leading-relaxed text-forge-muted">{item.desc}</p></div>
              </article>
            ))}
          </div>
        </section>
        <section className="mt-16 rounded-3xl border border-forge-cyan-dim bg-forge-cyan/[0.05] p-7 sm:p-10">
          <h2 className="mb-4 text-2xl font-bold">{b.cta}</h2>
          <p className="mb-6 max-w-[760px] leading-relaxed text-forge-muted">{b.ctaBody}</p>
          <div className="flex flex-wrap gap-4">
            <a href="https://github.com/Leonardo-Lalli/FORGE/issues/new?title=Feedback%20B2B" className="inline-flex items-center gap-2 rounded-full bg-forge-cyan px-5 py-3 text-sm font-semibold text-forge-on-accent">{b.cta}<ArrowRight className="size-4" aria-hidden="true" /></a>
            <Link href="/self-hosting" className="inline-flex items-center rounded-full border border-forge-border bg-forge-elev px-5 py-3 text-sm font-medium">{b.hostingLink}</Link>
          </div>
        </section>
      </div>
    </main>
  )
}
