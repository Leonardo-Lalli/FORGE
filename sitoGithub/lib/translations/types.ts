import type { it } from "./it"

type TranslationShape<T> = T extends string ? string
  : T extends readonly (infer U)[] ? readonly TranslationShape<U>[]
  : T extends object ? { readonly [K in keyof T]: K extends "diff" ? "low" | "med" | "high" | "vhigh" : TranslationShape<T[K]> }
  : T

export type Dictionary = TranslationShape<typeof it>
