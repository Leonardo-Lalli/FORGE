"use client"

import Link from "next/link"
import { ArrowDown, ArrowRight, Download } from "lucide-react"
import { useI18n } from "@/lib/i18n"

const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ""

export function HeroSection() {
  const { t } = useI18n()
  const [first, second] = t.hero.title.split("\n")
  return (
    <section className="px-6 pb-16 pt-12 sm:pt-20 lg:pb-24">
      <div className="mx-auto grid max-w-[1120px] items-center gap-12 lg:grid-cols-[1.2fr_0.8fr] lg:gap-16">
        <div>
          <p className="mb-6 inline-flex items-center gap-2 rounded-full border border-forge-cyan-dim bg-forge-cyan/5 px-3.5 py-1.5 text-xs font-medium text-forge-cyan">
            <span className="size-1.5 rounded-full bg-forge-cyan" aria-hidden="true" />
            {t.hero.badge}
          </p>
          <h1 className="mb-6 text-balance text-[clamp(42px,6vw,72px)] font-extrabold leading-[1.06] tracking-[-0.04em]">
            {first}<br /><span className="text-forge-cyan">{second}</span>
          </h1>
          <p className="mb-8 max-w-[530px] text-pretty text-lg leading-relaxed text-forge-muted">{t.hero.subtitle}</p>
          <div className="flex flex-wrap gap-3">
            <Link href="/#download" className="inline-flex items-center gap-2 rounded-full bg-forge-cyan px-6 py-3.5 text-sm font-semibold text-forge-on-accent transition-transform hover:-translate-y-0.5">
              <Download className="size-4" aria-hidden="true" />{t.hero.ctaPrimary}<ArrowRight className="size-4" aria-hidden="true" />
            </Link>
            <Link href="/#schermate" className="inline-flex items-center gap-2 rounded-full border border-forge-border bg-forge-elev px-5 py-3.5 text-sm font-medium transition-colors hover:border-forge-cyan">
              {t.hero.ctaSecondary}<ArrowDown className="size-4" aria-hidden="true" />
            </Link>
          </div>
          <p className="mt-5 text-xs leading-relaxed text-forge-muted">{t.hero.note}</p>
        </div>
        <figure className="relative mx-auto w-full max-w-[310px] lg:max-w-[290px]">
          <div className="absolute inset-8 -z-10 rounded-full bg-forge-cyan/15 blur-3xl" aria-hidden="true" />
          <div className="overflow-hidden rounded-[36px] border-[7px] border-[#24282c] bg-[#121212] shadow-2xl">
            <img src={`${basePath}/screenshots/workout-current.jpg`} alt={t.hero.imageLabel} width={945} height={2048} fetchPriority="high" className="h-auto w-full" />
          </div>
          <figcaption className="mt-4 text-center text-xs text-forge-muted">{t.hero.imageLabel}</figcaption>
        </figure>
      </div>
    </section>
  )
}
