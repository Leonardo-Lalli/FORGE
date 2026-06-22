import type { MetadataRoute } from 'next'

const BASE_URL = 'https://leonardo-lalli.github.io/FORGE'

export const dynamic = 'force-static'

export default function sitemap(): MetadataRoute.Sitemap {
  const lastModified = new Date()
  const pages = [
    '',
    '/features',
    '/roadmap',
    '/docs',
    '/self-hosting',
  ]

  return pages.map((path) => ({
    url: `${BASE_URL}${path}`,
    lastModified,
    changeFrequency: path === '' ? 'weekly' : 'monthly',
    priority: path === '' ? 1.0 : path === '/features' ? 0.9 : 0.7,
  }))
}
