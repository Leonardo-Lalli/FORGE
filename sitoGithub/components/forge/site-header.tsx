"use client"

import { Star } from "lucide-react"
import Link from "next/link"
import { ThemeToggle } from "@/components/forge/theme-toggle"
import { LanguageToggle } from "@/components/forge/language-toggle"
import { VersionBadge } from "@/components/forge/version-badge"
import { useI18n } from "@/lib/i18n"

export function SiteHeader() {
  const { t } = useI18n()
  return (
    <header className="sticky top-0 z-50 border-b border-forge-border bg-forge-bg/70 backdrop-blur-md">
      <div className="mx-auto flex h-[68px] w-full max-w-[1120px] items-center justify-between px-6">
        <Link href="/" className="flex items-center gap-3" aria-label="FORGE — home">
          <span className="grid size-9 flex-shrink-0 place-items-center rounded-[9px] border border-forge-cyan text-lg font-extrabold text-forge-cyan glow-cyan" aria-hidden="true">F</span>
          <span className="text-xl font-extrabold tracking-[0.22em]">FORGE</span>
          <VersionBadge />
        </Link>

        <nav className="hidden items-center gap-8 sm:flex">
          <Link href="/features" className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{t.nav.features}</Link>
          <Link href="/self-hosting" className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{t.nav.selfHosting}</Link>
          <Link href="/docs" className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{t.nav.docs}</Link>
          <Link href="/roadmap" className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{t.nav.roadmap}</Link>
          <Link href="/#download" className="text-sm text-forge-muted transition-colors hover:text-forge-fg">{t.nav.download}</Link>
        </nav>

        <div className="flex items-center gap-2">
          <LanguageToggle />
          <ThemeToggle />
          <a href="https://github.com/Leonardo-Lalli/FORGE" target="_blank" rel="noopener"
            className="ml-2 inline-flex items-center gap-2 rounded-full border border-forge-border px-4 py-2 text-sm text-forge-fg transition-all hover:border-forge-cyan hover:text-forge-cyan">
            GitHub
            <Star className="size-3.5" aria-hidden="true" />
          </a>
        </div>
      </div>
    </header>
  )
}
