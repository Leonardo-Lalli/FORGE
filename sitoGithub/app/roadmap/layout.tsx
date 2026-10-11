import type { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Roadmap',
  description:
    'Roadmap FORGE: stabilità e privacy prima, poi flusso allenamento, social, analytics, wearable e infrastruttura.',
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
