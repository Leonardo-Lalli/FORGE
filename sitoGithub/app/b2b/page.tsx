import type { Metadata } from "next"
import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { B2BPageContent } from "@/components/forge/b2b-page-content"

export const metadata: Metadata = {
  title: "FORGE per le palestre · B2B",
  description: "Una community sul server della palestra e la visione FORGE per trainer e iscritti. Funzioni disponibili e sviluppi B2B pianificati.",
  alternates: { canonical: "https://leonardo-lalli.github.io/FORGE/b2b" },
  openGraph: {
    title: "FORGE per le palestre · B2B",
    description: "Community sul server della palestra e strumenti per trainer pianificati. Scopri la visione B2B di FORGE.",
    url: "https://leonardo-lalli.github.io/FORGE/b2b",
    images: ["https://leonardo-lalli.github.io/FORGE/forge-logo.png"],
  },
}

export default function B2BPage() {
  return (
    <div className="min-h-screen bg-forge-bg text-forge-fg">
      <SiteHeader />
      <B2BPageContent />
      <SiteFooter />
    </div>
  )
}
