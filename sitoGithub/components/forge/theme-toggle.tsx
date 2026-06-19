"use client"

import { useEffect, useState } from "react"
import { useTheme } from "next-themes"
import { Moon, Sun } from "lucide-react"
import { useI18n } from "@/lib/i18n"

export function ThemeToggle() {
  const { resolvedTheme, setTheme } = useTheme()
  const { t } = useI18n()
  const [mounted, setMounted] = useState(false)

  useEffect(() => setMounted(true), [])

  const isDark = resolvedTheme === "dark"

  return (
    <button
      type="button"
      onClick={() => setTheme(isDark ? "light" : "dark")}
      aria-label={isDark ? t.controls.toggleThemeToLight : t.controls.toggleThemeToDark}
      title={isDark ? t.controls.toggleThemeToLight : t.controls.toggleThemeToDark}
      className="grid size-9 place-items-center rounded-lg border border-forge-border bg-forge-elev text-forge-muted transition-colors hover:border-forge-cyan-dim hover:text-forge-cyan"
    >
      {mounted ? (
        isDark ? (
          <Sun className="size-[18px]" aria-hidden="true" />
        ) : (
          <Moon className="size-[18px]" aria-hidden="true" />
        )
      ) : (
        <span className="size-[18px]" aria-hidden="true" />
      )}
    </button>
  )
}
