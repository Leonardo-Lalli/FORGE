import type { Metadata } from "next"

export const metadata: Metadata = {
  title: "Roadmap",
  description: "Gli obiettivi di FORGE e le idee da esplorare, senza date di rilascio promesse.",
  alternates: { canonical: "https://leonardo-lalli.github.io/FORGE/roadmap" },
  openGraph: {
    title: "FORGE — Roadmap",
    description: "Gli obiettivi di FORGE e le idee da esplorare, senza date di rilascio promesse.",
    url: "https://leonardo-lalli.github.io/FORGE/roadmap",
    images: ["https://leonardo-lalli.github.io/FORGE/forge-logo.png"],
  },
}

export default function PageLayout({ children }: { children: React.ReactNode }) {
  return children
}
