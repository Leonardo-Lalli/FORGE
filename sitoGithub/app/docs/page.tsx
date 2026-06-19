"use client"

import { useState } from "react"
import { SiteHeader } from "@/components/forge/site-header"
import { DocsSection } from "@/components/forge/docs-section"
import { SiteFooter } from "@/components/forge/cta-footer"

export default function DocsPage() {
  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main className="pt-6">
          <DocsSection />
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
