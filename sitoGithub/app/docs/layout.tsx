import type { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Documentazione',
  description:
    'Documentazione completa di FORGE: specifica prodotto, piano iterazioni, architettura tecnica MVVM, diario di sviluppo, matrice di test, note API PocketBase e ExerciseDB.',
  alternates: { canonical: '/docs' },
  openGraph: {
    title: 'FORGE — Documentazione',
    description:
      'Spec, architettura, diario di sviluppo, test matrix e note tecniche API.',
    url: '/docs',
  },
}

export default function DocsLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return children
}
