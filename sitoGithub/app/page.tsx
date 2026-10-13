import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { HeroSection } from "@/components/forge/hero-section"
import { FeaturesSection } from "@/components/forge/features-section"
import { ScreenshotsSection } from "@/components/forge/screenshots-section"
import { GamificationSection } from "@/components/forge/gamification-section"
import { CommunitySection, DownloadSection, ProjectSection } from "@/components/forge/landing-sections"

export default function Page() {
  return (
    <div className="min-h-screen bg-forge-bg text-forge-fg">
      <SiteHeader />
      <main id="top">
        <HeroSection />
        <FeaturesSection />
        <ScreenshotsSection />
        <GamificationSection />
        <CommunitySection />
        <DownloadSection />
        <ProjectSection />
      </main>
      <SiteFooter />
    </div>
  )
}
