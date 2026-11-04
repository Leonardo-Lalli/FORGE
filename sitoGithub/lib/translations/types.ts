import type { it } from "./it"

type TranslationShape<T> = T extends string ? string
  : T extends readonly (infer U)[] ? readonly TranslationShape<U>[]
  : T extends object ? { readonly [K in keyof T]: TranslationShape<T[K]> }
  : T

export type Dictionary = TranslationShape<typeof it>
