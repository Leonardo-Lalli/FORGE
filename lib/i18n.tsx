"use client"

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react"
import { it } from "@/lib/translations/it"
import { en } from "@/lib/translations/en"

export type Lang = "it" | "en"

export type Dictionary = typeof it

const dictionaries: Record<Lang, Dictionary> = { it, en }

type I18nContextValue = {
  lang: Lang
  setLang: (lang: Lang) => void
  toggleLang: () => void
  t: Dictionary
}

const I18nContext = createContext<I18nContextValue | null>(null)

const STORAGE_KEY = "forge-lang"

export function I18nProvider({ children }: { children: ReactNode }) {
  const [lang, setLangState] = useState<Lang>("it")

  useEffect(() => {
    const stored = window.localStorage.getItem(STORAGE_KEY) as Lang | null
    if (stored === "it" || stored === "en") {
      setLangState(stored)
    } else {
      const browser = navigator.language.toLowerCase().startsWith("en") ? "en" : "it"
      setLangState(browser)
    }
  }, [])

  useEffect(() => {
    document.documentElement.lang = lang
  }, [lang])

  const setLang = (next: Lang) => {
    setLangState(next)
    window.localStorage.setItem(STORAGE_KEY, next)
  }

  const toggleLang = () => setLang(lang === "it" ? "en" : "it")

  return (
    <I18nContext.Provider
      value={{ lang, setLang, toggleLang, t: dictionaries[lang] }}
    >
      {children}
    </I18nContext.Provider>
  )
}

export function useI18n() {
  const ctx = useContext(I18nContext)
  if (!ctx) {
    throw new Error("useI18n must be used within an I18nProvider")
  }
  return ctx
}
