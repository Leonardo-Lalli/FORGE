"use client"

import { useI18n } from "@/lib/i18n"

export function LanguageToggle() {
  const { lang, toggleLang, t } = useI18n()

  return (
    <button
      type="button"
      onClick={toggleLang}
      aria-label={lang === "it" ? t.controls.switchToEnglish : t.controls.switchToItalian}
      title={lang === "it" ? t.controls.switchToEnglish : t.controls.switchToItalian}
      className="grid h-9 min-w-9 place-items-center rounded-lg border border-forge-border bg-forge-elev px-2.5 font-mono text-xs font-semibold uppercase tracking-wider text-forge-muted transition-colors hover:border-forge-cyan-dim hover:text-forge-cyan"
    >
      {lang === "it" ? "EN" : "IT"}
    </button>
  )
}
