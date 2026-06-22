import type { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Self-Hosting',
  description:
    'Self-hosta FORGE con Docker e PocketBase sul tuo server: Raspberry Pi, mini-PC o VPS. Privacy by design, LAN-first, zero costi cloud. Avvia il backend in 30 secondi con docker compose up.',
  alternates: { canonical: '/self-hosting' },
  openGraph: {
    title: 'FORGE — Self-Hosting con Docker',
    description:
      'Il tuo server, le tue regole. Avvia il backend FORGE con PocketBase su Raspberry Pi, mini-PC o VPS. Zero costi cloud, privacy totale.',
    url: '/self-hosting',
  },
}

export default function SelfHostingLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return children
}
