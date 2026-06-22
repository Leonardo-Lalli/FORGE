import type { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Roadmap',
  description:
    'La roadmap di FORGE: QoL & usabilità v0.9.0, release stabile v1.0.0 con notifiche push e localizzazione, social & community v1.1.0, health & Wear OS v1.2.0, advanced features v1.3.0, piattaforma & infrastruttura v1.4.0.',
  alternates: { canonical: '/roadmap' },
  openGraph: {
    title: 'FORGE — Roadmap',
    description:
      'Il percorso di sviluppo di FORGE dalle feature QoL al Play Store, modulo B2B per palestre e integrazioni Wear OS.',
    url: '/roadmap',
  },
}

export default function RoadmapLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return children
}
