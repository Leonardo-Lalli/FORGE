"use client"

import { useI18n } from "@/lib/i18n"

const screenshots = ["dashboard.jpeg", "workout-current.jpg", "stats.jpeg", "profile.jpeg"]
const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ""

export function ScreenshotsSection() {
  const { t } = useI18n()
  const content = t.screenshots

  return (
    <section id="schermate" className="scroll-mt-24 px-6 py-16">
      <div className="mx-auto max-w-[1120px]">
        <p className="mb-3 font-mono text-xs text-forge-cyan">{content.eyebrow}</p>
        <h2 className="mb-5 text-3xl font-extrabold">{content.title}</h2>
        <p className="mb-8 max-w-[760px] text-forge-muted">{content.intro}</p>
        <div className="grid grid-cols-2 gap-4 md:grid-cols-4 md:gap-6">
          {screenshots.map((image, index) => (
            <figure key={image}>
              <a href={`${basePath}/screenshots/${image}`} target="_blank" rel="noopener noreferrer" className="relative block overflow-hidden rounded-[18px] border border-forge-border bg-forge-elev transition-colors hover:border-forge-cyan">
                <img src={`${basePath}/screenshots/${image}`} alt={content.labels[index]} width={945} height={2048} loading="lazy" className="h-auto w-full" />
                {image === "workout-current.jpg" && <span aria-hidden="true" className="pointer-events-none absolute left-[17%] top-0 z-10 h-[4%] w-[20%] bg-[#121212]" />}
              </a>
              <figcaption className="mt-3 text-center text-sm text-forge-muted">{content.labels[index]}</figcaption>
            </figure>
          ))}
        </div>
      </div>
    </section>
  )
}
