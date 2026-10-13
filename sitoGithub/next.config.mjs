import path from 'node:path'
import { fileURLToPath } from 'node:url'
/** @type {import('next').NextConfig} */
const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ''
const nextConfig = {
  output: 'export',
  turbopack: { root: path.dirname(fileURLToPath(import.meta.url)) },
  basePath,
  typescript: {
    ignoreBuildErrors: false,
  },
  images: {
    unoptimized: true,
  },
}

export default nextConfig
