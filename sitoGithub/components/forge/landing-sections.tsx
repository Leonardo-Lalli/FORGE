"use client"

import Link from "next/link"
import { ArrowRight, Download, Users, Building2, Server, ChevronDown } from "lucide-react"
import { useI18n } from "@/lib/i18n"
import { useReleaseDownload } from "./release-download"
import { currentReleaseUrl } from "@/lib/release"

export function CommunitySection() {
  const { t } = useI18n()
  const c = t.landing
  return (
    <section className="px-6 py-16">
      <div className="mx-auto grid max-w-[1120px] gap-8 rounded-3xl border border-forge-border bg-forge-elev p-7 sm:p-10 md:grid-cols-[auto_1fr] md:items-start">
        <div className="grid size-14 place-items-center rounded-2xl bg-forge-cyan/10 text-forge-cyan"><Users className="size-7" aria-hidden="true" /></div>
        <div>
          <h2 className="mb-4 max-w-[650px] text-balance text-3xl font-bold tracking-tight">{c.communityTitle}</h2>
          <p className="max-w-[760px] leading-relaxed text-forge-muted">{c.communityBody}</p>
          <p className="mt-4 text-sm text-forge-muted">{c.communityNote}</p>
        </div>
      </div>
    </section>
  )
}

export function DownloadSection() {
  const { t } = useI18n()
  const c = t.landing
  const downloadUrl = useReleaseDownload()
  return (
    <section id="download" className="scroll-mt-24 px-6 py-20">
      <div className="mx-auto max-w-[1120px]">
        <div className="rounded-3xl border border-forge-cyan-dim bg-forge-cyan/[0.05] px-6 py-10 sm:px-10">
          <h2 className="mb-4 text-balance text-3xl font-extrabold tracking-tight sm:text-4xl">{c.downloadTitle}</h2>
          <p className="mb-7 max-w-[640px] leading-relaxed text-forge-muted">{c.downloadBody}</p>
          <div className="flex flex-wrap items-center gap-5">
            <a href={downloadUrl} className="inline-flex items-center gap-2 rounded-full bg-forge-cyan px-6 py-3.5 text-sm font-semibold text-forge-on-accent transition-transform hover:-translate-y-0.5"><Download className="size-4" aria-hidden="true" />{c.downloadButton}</a>
            <a href={currentReleaseUrl} className="text-sm font-medium text-forge-cyan underline underline-offset-4">{c.releaseLink}</a>
          </div>
          <p className="mt-4 text-xs leading-relaxed text-forge-muted">{c.downloadRequirements}</p>
          <details className="mt-6 max-w-[800px] rounded-xl border border-forge-border bg-forge-elev p-4">
            <summary className="cursor-pointer text-sm font-medium">{c.existingInstall}</summary>
            <p className="mt-3 text-sm leading-relaxed text-forge-muted">{t.hero.updateNote}</p>
          </details>
        </div>
        <ol className="mt-8 grid gap-5 md:grid-cols-3">
          {c.steps.map((step, index) => (
            <li key={step.title} className="rounded-2xl border border-forge-border bg-forge-elev p-6">
              <span className="mb-4 inline-flex size-8 items-center justify-center rounded-full bg-forge-cyan/10 font-mono text-xs font-bold text-forge-cyan">0{index + 1}</span>
              <h3 className="mb-2 font-semibold">{step.title}</h3>
              <p className="text-sm leading-relaxed text-forge-muted">{step.body}</p>
            </li>
          ))}
        </ol>
        <div className="mx-auto mt-14 max-w-[800px]">
          <h3 className="mb-5 text-2xl font-bold">{c.faqTitle}</h3>
          {c.faq.map(item => (
            <details key={item.question} className="group border-b border-forge-border py-5">
              <summary className="flex cursor-pointer list-none items-center justify-between gap-5 font-medium [&::-webkit-details-marker]:hidden">
                {item.question}<ChevronDown className="size-4 shrink-0 text-forge-muted transition-transform group-open:rotate-180" aria-hidden="true" />
              </summary>
              <p className="mt-3 pr-6 text-sm leading-relaxed text-forge-muted">{item.answer}</p>
            </details>
          ))}
          <a href="https://github.com/Leonardo-Lalli/FORGE/issues" className="mt-6 inline-block text-sm text-forge-cyan underline underline-offset-4">{c.feedback}</a>
        </div>
      </div>
    </section>
  )
}

export function ProjectSection() {
  const { t } = useI18n()
  const c = t.landing
  const paths = [
    { title: c.b2bTitle, body: c.b2bBody, label: c.b2bLink, href: "/b2b", icon: Building2 },
    { title: c.hostingTitle, body: c.hostingBody, label: c.hostingLink, href: "/self-hosting", icon: Server },
  ]
  return (
    <section id="progetto" className="scroll-mt-24 border-t border-forge-border px-6 py-20">
      <div className="mx-auto max-w-[1120px]">
        <div className="grid gap-6 md:grid-cols-2">
          {paths.map(item => (
            <article key={item.href} className="rounded-2xl border border-forge-border bg-forge-elev p-7">
              <item.icon className="mb-5 size-6 text-forge-cyan" aria-hidden="true" />
              <h2 className="mb-3 text-2xl font-bold">{item.title}</h2>
              <p className="mb-6 leading-relaxed text-forge-muted">{item.body}</p>
              <Link href={item.href} className="inline-flex items-center gap-2 text-sm font-semibold text-forge-cyan">{item.label}<ArrowRight className="size-4" aria-hidden="true" /></Link>
            </article>
          ))}
        </div>
        <div className="mt-14 max-w-[800px]">
          <h2 className="mb-4 text-2xl font-bold">{c.projectTitle}</h2>
          <p className="mb-5 text-sm leading-relaxed text-forge-muted">{t.story.body}</p>
          <Link href="/docs" className="text-sm text-forge-cyan underline underline-offset-4">{c.projectLink}</Link>
        </div>
      </div>
    </section>
  )
}
