import { Analytics } from '@vercel/analytics/next'
import type { Metadata, Viewport } from 'next'
import { Geist, Geist_Mono } from 'next/font/google'
import { Providers } from '@/components/providers'
import './globals.css'

const geistSans = Geist({ variable: '--font-geist-sans', subsets: ['latin'] })
const geistMono = Geist_Mono({
  variable: '--font-geist-mono',
  subsets: ['latin'],
})

const siteUrl = 'https://leonardo-lalli.github.io/FORGE'
const ogImage = '/apple-icon.png'

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: {
    default: 'FORGE — Allenati. Traccia. Progredisci.',
    template: '%s — FORGE',
  },
  description:
    "FORGE è l'app di palestra open-source, privacy-first e self-hostable. Traccia i tuoi volumi, gestisci i tempi di recupero e mantieni i tuoi dati al sicuro.",
  keywords: [
    'FORGE', 'app palestra', 'workout tracker', 'gym tracker', 'fitness app',
    'allenamento', 'tracking allenamento', 'self-hosted', 'open source',
    'PocketBase', '.NET MAUI', 'Android fitness', 'gratis', 'privacy-first',
  ],
  authors: [{ name: 'Leonardo Lalli' }],
  creator: 'Leonardo Lalli',
  publisher: 'Leonardo Lalli',
  applicationName: 'FORGE',
  category: 'Health & Fitness',
  formatDetection: { telephone: false },
  alternates: {
    canonical: '/',
    languages: {
      'it-IT': '/',
      'en-US': '/',
      'es-ES': '/',
      'zh-CN': '/',
    },
  },
  openGraph: {
    type: 'website',
    locale: 'it_IT',
    alternateLocale: ['en_US', 'es_ES', 'zh_CN'],
    url: siteUrl,
    siteName: 'FORGE',
    title: 'FORGE — Il diario di allenamento sociale',
    description:
      "App Android open-source per tracciare gli allenamenti in palestra. 1.500+ esercizi, feed sociale, achievement, statistiche. Self-hostable e gratis.",
    images: [
      {
        url: ogImage,
        width: 512,
        height: 512,
        alt: 'FORGE — Workout Tracker',
      },
    ],
  },
  twitter: {
    card: 'summary_large_image',
    title: 'FORGE — Il diario di allenamento sociale',
    description:
      'App Android open-source per tracciare gli allenamenti. 1.500+ esercizi, feed sociale, achievement. Self-hostable e gratis.',
    images: [ogImage],
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      'max-image-preview': 'large',
      'max-snippet': -1,
    },
  },
  icons: {
    icon: [
      {
        url: '/icon-light-32x32.png',
        media: '(prefers-color-scheme: light)',
      },
      {
        url: '/icon-dark-32x32.png',
        media: '(prefers-color-scheme: dark)',
      },
      {
        url: '/icon.svg',
        type: 'image/svg+xml',
      },
    ],
    apple: '/apple-icon.png',
  },
}

export const viewport: Viewport = {
  colorScheme: 'dark light',
  themeColor: [
    { media: '(prefers-color-scheme: light)', color: '#f7f8f8' },
    { media: '(prefers-color-scheme: dark)', color: '#0d0d0d' },
  ],
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html
      lang="it"
      suppressHydrationWarning
      className={`${geistSans.variable} ${geistMono.variable} bg-forge-bg`}
    >
      <body className="font-sans antialiased bg-forge-bg text-forge-fg">
        <Providers>{children}</Providers>
        {process.env.NODE_ENV === 'production' && <Analytics />}
        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{
            __html: JSON.stringify({
              '@context': 'https://schema.org',
              '@type': 'SoftwareApplication',
              name: 'FORGE',
              applicationCategory: 'HealthApplication',
              operatingSystem: 'Android',
              description:
                'App Android open-source per tracciare gli allenamenti in palestra con funzionalità social.',
              author: { '@type': 'Person', name: 'Leonardo Lalli' },
              offers: { '@type': 'Offer', price: '0', priceCurrency: 'EUR' },
              aggregateRating: {
                '@type': 'AggregateRating',
                ratingValue: '5',
                ratingCount: '1',
              },
            }),
          }}
        />
      </body>
    </html>
  )
}
