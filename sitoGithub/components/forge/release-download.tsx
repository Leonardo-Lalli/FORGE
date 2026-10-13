"use client"

import { useEffect, useState } from "react"
import { currentReleaseUrl, isCurrentOrNewer } from "@/lib/release"

const fallback = "https://github.com/Leonardo-Lalli/FORGE/releases"

export function useReleaseDownload() {
  const [url, setUrl] = useState(currentReleaseUrl)

  useEffect(() => {
    const abort = new AbortController()
    const timeout = setTimeout(() => abort.abort(), 5000)

    fetch("https://api.github.com/repos/Leonardo-Lalli/FORGE/releases?per_page=10", {
      signal: abort.signal,
    })
      .then(response => {
        if (!response.ok) throw Error("Release lookup failed")
        return response.json()
      })
      .then(releases => {
        if (!Array.isArray(releases)) return
        for (const release of releases) {
          if (release.draft || !isCurrentOrNewer(release.tag_name)) continue
          const assets = Array.isArray(release.assets) ? release.assets : []
          const asset = assets.find((item: { name?: string }) => item.name === "FORGE.apk")
            || assets.find((item: { name?: string }) => item.name?.endsWith(".apk"))
          if (asset && typeof asset.browser_download_url === "string"
            && asset.browser_download_url.startsWith(fallback + "/download/")) {
            setUrl(asset.browser_download_url)
            break
          }
        }
      })
      .catch(() => {}) // Keep the release-list link when GitHub is unavailable.
      .finally(() => clearTimeout(timeout))

    return () => {
      abort.abort()
      clearTimeout(timeout)
    }
  }, [])

  return url
}
