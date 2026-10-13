import type { Metadata } from "next"

export const metadata: Metadata = {
  title: "Documentazione",
  description: "Origine del progetto, specifiche, architettura, diario di sviluppo e verifiche di FORGE. Documentazione e limiti dichiarati della beta.",
  alternates: { canonical: "https://leonardo-lalli.github.io/FORGE/docs" },
  openGraph: {
    title: "FORGE — Documentazione",
    description: "Origine del progetto, specifiche, architettura, diario di sviluppo e verifiche di FORGE. Documentazione e limiti dichiarati della beta.",
    url: "https://leonardo-lalli.github.io/FORGE/docs",
    images: ["https://leonardo-lalli.github.io/FORGE/forge-logo.png"],
  },
}

export default function PageLayout({ children }: { children: React.ReactNode }) {
  return children
}
