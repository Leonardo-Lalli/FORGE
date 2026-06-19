import { SiteHeader } from "@/components/forge/site-header"
import { HeroSection } from "@/components/forge/hero-section"
import { FeaturesSection } from "@/components/forge/features-section"
import { SelfHostingSection } from "@/components/forge/self-hosting-section"
import { DocsSection } from "@/components/forge/docs-section"
import { CtaBand, SiteFooter } from "@/components/forge/cta-footer"

export default function Page() {
  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      {/* Bagliore ambientale e griglia di sfondo */}
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />

      <div className="relative z-10">
        <SiteHeader />
        <main id="top">
          <HeroSection />
          <FeaturesSection />
          <SelfHostingSection />
          <DocsSection />
          <CtaBand />
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
