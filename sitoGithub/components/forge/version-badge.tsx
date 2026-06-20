"use client"

import { useEffect, useState } from "react"
import Link from "next/link"

interface Release {
  tag_name: string
  html_url: string
  published_at: string
}

export function VersionBadge() {
  const [version, setVersion] = useState<string>("")
  const [isNew, setIsNew] = useState(false)
  const [url, setUrl] = useState("")

  useEffect(() => {
    const cached = localStorage.getItem("forge_last_version")
    fetch("https://api.github.com/repos/Leonardo-Lalli/FORGE/releases/latest")
      .then(r => r.json())
      .then((data: Release) => {
        if (data?.tag_name) {
          setVersion(data.tag_name)
          setUrl(data.html_url)
          if (cached && cached !== data.tag_name) {
            setIsNew(true)
          }
          localStorage.setItem("forge_last_version", data.tag_name)
        }
      })
      .catch(() => {})
  }, [])

  if (!version) return null

  return (
    <>
      <span className="hidden sm:inline-flex items-center gap-1.5 rounded-full border border-forge-cyan-dim bg-forge-cyan/[0.06] px-2.5 py-0.5 font-mono text-[11px] text-forge-cyan">
        {version}
      </span>
      {isNew && (
        <div className="fixed top-[68px] left-0 right-0 z-40 flex items-center justify-center gap-2 bg-amber-500/90 px-4 py-2 text-sm font-semibold text-black">
          Nuova versione disponibile: {version}
          <a href={url} target="_blank" rel="noopener" className="underline">CHANGELOG</a>
          <button onClick={() => setIsNew(false)} className="ml-2 text-lg leading-none">&times;</button>
        </div>
      )}
    </>
  )
}
