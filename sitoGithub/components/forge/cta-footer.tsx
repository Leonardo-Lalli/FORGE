"use client"

import Link from "next/link"
import { useI18n } from "@/lib/i18n"

const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ""

export function SiteFooter() {
  const { t } = useI18n()
  const groups = [
    { label: t.footer.product, links: [
      { href: "/features", label: t.nav.features },
      { href: "/#download", label: t.nav.download },
      { href: "/b2b", label: t.nav.b2b },
      { href: "/roadmap", label: t.nav.roadmap },
    ] },
    { label: t.footer.resources, links: [
      { href: "/docs", label: t.nav.docs },
      { href: "/self-hosting", label: t.nav.selfHosting },
      { href: "https://github.com/Leonardo-Lalli/FORGE", label: "GitHub" },
      { href: "https://github.com/Leonardo-Lalli/FORGE/issues", label: t.landing.feedback.replace(" →", "") },
    ] },
  ]
  return (
    <footer className="border-t border-forge-border px-6 py-12">
      <div className="mx-auto max-w-[1120px]">
        <div className="grid gap-10 md:grid-cols-[1.4fr_1fr_1fr]">
          <div>
            <Link href="/" className="inline-flex items-center gap-3"><img src={`${basePath}/forge-logo.png`} alt="" width={36} height={36} className="size-9 rounded-xl" /><span className="text-lg font-extrabold tracking-[0.15em]">FORGE</span></Link>
            <p className="mt-4 max-w-[340px] text-sm leading-relaxed text-forge-muted">{t.footer.tagline}</p>
          </div>
          {groups.map(group => <nav key={group.label} aria-label={group.label}><p className="mb-4 text-sm font-semibold">{group.label}</p><ul className="space-y-3">{group.links.map(link => <li key={link.href}><Link href={link.href} className="text-sm text-forge-muted transition-colors hover:text-forge-cyan">{link.label}</Link></li>)}</ul></nav>)}
        </div>
        <p className="mt-10 border-t border-forge-border pt-6 text-xs text-forge-muted">{t.footer.rights}</p>
      </div>
    </footer>
  )
}
