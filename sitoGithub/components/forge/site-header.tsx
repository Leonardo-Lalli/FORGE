"use client"

import { useState } from "react"
import { Menu, X, Download } from "lucide-react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { ThemeToggle } from "@/components/forge/theme-toggle"
import { LanguageToggle } from "@/components/forge/language-toggle"
import { useI18n } from "@/lib/i18n"

const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ""

export function SiteHeader() {
  const { t, lang } = useI18n()
  const pathname = usePathname()
  const [open, setOpen] = useState(false)
  const links = [
    { href: "/features", label: t.nav.features },
    { href: "/b2b", label: t.nav.b2b },
    { href: "/roadmap", label: t.nav.roadmap },
    { href: "/docs", label: t.nav.docs },
    { href: "/self-hosting", label: t.nav.selfHosting },
  ]
  const active = (href: string) => pathname === href || pathname === `${basePath}${href}`
  return (
    <header className="sticky top-0 z-50 border-b border-forge-border bg-forge-bg/95 backdrop-blur-md">
      <div className="mx-auto flex h-[72px] max-w-[1120px] items-center justify-between gap-4 px-6">
        <Link href="/" className="flex shrink-0 items-center gap-2.5" aria-label="FORGE — home">
          <img src={`${basePath}/forge-logo.png`} alt="" width={36} height={36} className="size-9 rounded-xl" />
          <span className="text-lg font-extrabold tracking-[0.15em]">FORGE</span>
        </Link>
        <nav className="hidden items-center gap-5 lg:flex" aria-label={lang === "it" ? "Navigazione principale" : "Main navigation"}>
          {links.map(link => <Link key={link.href} href={link.href} aria-current={active(link.href) ? "page" : undefined} className={`text-sm transition-colors hover:text-forge-cyan ${active(link.href) ? "font-semibold text-forge-cyan" : "text-forge-muted"}`}>{link.label}</Link>)}
        </nav>
        <div className="flex shrink-0 items-center gap-2">
          <LanguageToggle /><ThemeToggle />
          <Link href="/#download" className="hidden items-center gap-2 rounded-full bg-forge-cyan px-4 py-2.5 text-xs font-semibold text-forge-on-accent lg:inline-flex"><Download className="size-3.5" aria-hidden="true" />{t.nav.download}</Link>
          <button type="button" onClick={() => setOpen(!open)} className="grid size-9 place-items-center rounded-lg border border-forge-border bg-forge-elev text-forge-muted lg:hidden" aria-label={lang === "it" ? (open ? "Chiudi menu" : "Apri menu") : (open ? "Close menu" : "Open menu")} aria-expanded={open} aria-controls="mobile-navigation">
            {open ? <X className="size-5" /> : <Menu className="size-5" />}
          </button>
        </div>
      </div>
      <nav id="mobile-navigation" hidden={!open} className="border-t border-forge-border bg-forge-bg px-6 pb-4 lg:hidden" aria-label={lang === "it" ? "Navigazione mobile" : "Mobile navigation"}>
        {links.map(link => <Link key={link.href} href={link.href} onClick={() => setOpen(false)} aria-current={active(link.href) ? "page" : undefined} className={`block border-b border-forge-border py-3 text-sm ${active(link.href) ? "font-semibold text-forge-cyan" : "text-forge-muted"}`}>{link.label}</Link>)}
        <Link href="/#download" onClick={() => setOpen(false)} className="mt-4 inline-flex items-center gap-2 rounded-full bg-forge-cyan px-5 py-3 text-sm font-semibold text-forge-on-accent"><Download className="size-4" aria-hidden="true" />{t.nav.download}</Link>
      </nav>
    </header>
  )
}
