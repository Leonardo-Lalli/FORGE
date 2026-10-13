import type { Metadata } from "next"

export const metadata: Metadata = {
  title: "Funzionalità",
  description: "Schede personali, serie, recuperi per esercizio, consigli a carosello, statistiche e community sullo stesso server. Funzioni e limiti della beta FORGE.",
  alternates: { canonical: "https://leonardo-lalli.github.io/FORGE/features" },
  openGraph: {
    title: "FORGE — Funzionalità",
    description: "Schede personali, serie, recuperi per esercizio, consigli a carosello, statistiche e community sullo stesso server. Funzioni e limiti della beta FORGE.",
    url: "https://leonardo-lalli.github.io/FORGE/features",
    images: ["https://leonardo-lalli.github.io/FORGE/forge-logo.png"],
  },
}

export default function PageLayout({ children }: { children: React.ReactNode }) {
  return children
}
