import type { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Funzionalità',
  description:
    '1.500+ esercizi con GIF, registrazione allenamenti con serie e rest timer, statistiche avanzate, feed sociale, 48 achievement, offline SQLite, CSV import/export, self-hosting Docker.',
  alternates: { canonical: '/features' },
  openGraph: {
    title: 'FORGE — Funzionalità',
    description:
      'Tutto quello che serve per allenarti meglio: catalogo esercizi, tracking serie, statistiche, social, achievement, offline sync.',
    url: '/features',
  },
}

export default function FeaturesLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return children
}
