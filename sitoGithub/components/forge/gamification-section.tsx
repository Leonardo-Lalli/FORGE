"use client"

import { useI18n } from "@/lib/i18n"

const badges = ["primopasso.jpg", "habit.jpg", "max_weight.jpg"]
const basePath = process.env.NEXT_PUBLIC_BASE_PATH || ""

export function GamificationSection() {
  const { t } = useI18n()
  const content = t.gamification

  return (
    <section className="px-6 py-20">
      <div className="mx-auto max-w-[1120px]">
        <p className="mb-3 font-mono text-xs text-forge-cyan">{content.eyebrow}</p>
        <h2 className="mb-5 text-3xl font-extrabold">{content.title}</h2>
        <p className="mb-8 max-w-[680px] text-forge-muted">{content.intro}</p>
        <div className="grid gap-5 md:grid-cols-3">
          {content.items.map((item, index) => (
            <article key={item.title} className="rounded-[14px] border border-forge-border bg-forge-elev p-6">
              <img src={`${basePath}/achievements/${badges[index]}`} alt="" width={80} height={80} loading="lazy" className="mb-5 size-20 rounded-xl object-cover" />
              <h3 className="mb-3 text-lg font-bold">{item.title}</h3>
              <p className="text-sm leading-relaxed text-forge-muted">{item.text}</p>
            </article>
          ))}
        </div>
        <p className="mt-5 max-w-[760px] text-xs leading-relaxed text-forge-dim">{content.note}</p>
      </div>
    </section>
  )
}
