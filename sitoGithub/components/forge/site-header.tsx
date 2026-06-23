"use client"

import { useState } from "react"
import { Star, Menu, X } from "lucide-react"
import Link from "next/link"
import { ThemeToggle } from "@/components/forge/theme-toggle"
import { LanguageToggle } from "@/components/forge/language-toggle"
import { VersionBadge } from "@/components/forge/version-badge"
import { useI18n } from "@/lib/i18n"

export function SiteHeader() {
  const { t } = useI18n()
  const [open, setOpen] = useState(false)

  const links = [
    { href: "/features", label: t.nav.features },
    { href: "/self-hosting", label: t.nav.selfHosting },
    { href: "/docs", label: t.nav.docs },
    { href: "/roadmap", label: t.nav.roadmap },
    { href: "/#download", label: t.nav.download },
  ]

  return (
    <header className="sticky top-0 z-50 border-b border-forge-border bg-forge-bg/70 backdrop-blur-md">
      <div className="mx-auto flex h-[68px] w-full max-w-[1120px] items-center justify-between px-6">
        <Link href="/" className="flex items-center gap-3 flex-shrink-0" aria-label="FORGE — home">
          <span className="grid size-9 flex-shrink-0 place-items-center rounded-[9px] border border-forge-cyan text-lg font-extrabold text-forge-cyan glow-cyan" aria-hidden="true">F</span>
          <span className="text-xl font-extrabold tracking-[0.22em]">FORGE</span>
          <VersionBadge />
        </Link>

        <nav className="hidden items-center gap-8 sm:flex">
          {links.map((l) => (
            <Link key={l.href} href={l.href} className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{l.label}</Link>
          ))}
        </nav>

        <div className="flex items-center gap-2">
          <LanguageToggle />
          <ThemeToggle />
          <a href="https://github.com/Leonardo-Lalli/FORGE" target="_blank" rel="noopener"
            className="hidden sm:inline-flex items-center gap-2 rounded-full border border-forge-border px-4 py-2 text-sm text-forge-fg transition-all hover:border-forge-cyan hover:text-forge-cyan">
            GitHub
            <Star className="size-3.5" aria-hidden="true" />
          </a>
          <button
            onClick={() => setOpen(!open)}
            className="sm:hidden grid size-9 place-items-center rounded-lg border border-forge-border text-forge-muted hover:text-forge-fg hover:border-forge-cyan"
            aria-label={open ? "Chiudi menu" : "Apri menu"}
          >
            {open ? <X className="size-5" /> : <Menu className="size-5" />}
          </button>
        </div>
      </div>

      {open && (
        <nav className="sm:hidden border-t border-forge-border bg-forge-bg/95 backdrop-blur-md px-6 pb-4 pt-2">
          {links.map((l) => (
            <Link
              key={l.href}
              href={l.href}
              onClick={() => setOpen(false)}
              className="block py-3 text-sm text-forge-muted border-b border-forge-border/60 last:border-0 transition-colors hover:text-forge-cyan"
            >
              {l.label}
            </Link>
          ))}
          <a href="https://github.com/Leonardo-Lalli/FORGE" target="_blank" rel="noopener"
            onClick={() => setOpen(false)}
            className="flex items-center gap-2 py-3 text-sm text-forge-muted transition-colors hover:text-forge-cyan">
            <Star className="size-3.5" aria-hidden="true" /> GitHub
          </a>
        </nav>
      )}
    </header>
  )
}
