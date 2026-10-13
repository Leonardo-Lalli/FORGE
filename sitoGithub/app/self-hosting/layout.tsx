import type { Metadata } from "next"

export const metadata: Metadata = {
  title: "Self-Hosting",
  description: "Installa il backend FORGE con PocketBase sul tuo server. Guida a configurazione, migrazioni, HTTPS, backup e manutenzione.",
  alternates: { canonical: "https://leonardo-lalli.github.io/FORGE/self-hosting" },
  openGraph: {
    title: "FORGE — Self-Hosting",
    description: "Installa il backend FORGE con PocketBase sul tuo server. Guida a configurazione, migrazioni, HTTPS, backup e manutenzione.",
    url: "https://leonardo-lalli.github.io/FORGE/self-hosting",
    images: ["https://leonardo-lalli.github.io/FORGE/forge-logo.png"],
  },
}

export default function PageLayout({ children }: { children: React.ReactNode }) {
  return children
}
