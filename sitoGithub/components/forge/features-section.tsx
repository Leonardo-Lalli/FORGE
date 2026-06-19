import { BarChart3, ShieldCheck, Timer } from "lucide-react"
import type { LucideIcon } from "lucide-react"

type Feature = {
  icon: LucideIcon
  title: string
  description: string
}

const features: Feature[] = [
  {
    icon: BarChart3,
    title: "Tracciamento volumi",
    description:
      "Registra serie, ripetizioni e carichi. FORGE calcola il volume totale per gruppo muscolare e ti mostra i trend nel tempo.",
  },
  {
    icon: ShieldCheck,
    title: "Privacy totale",
    description:
      "I dati vivono sul tuo dispositivo o sul tuo server. Nessun tracker, nessuna telemetria, nessun account obbligatorio.",
  },
  {
    icon: Timer,
    title: "Timer di recupero",
    description:
      "Timer di riposo intelligenti tra le serie, con notifiche e preset personalizzabili per ogni esercizio.",
  },
]

export function FeaturesSection() {
  return (
    <section id="funzionalita" className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="mb-14 max-w-[640px]">
          <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Funzionalità</p>
          <h2 className="mb-4 text-balance text-[clamp(30px,5vw,44px)] font-extrabold leading-[1.1] tracking-[-0.02em]">
            Tutto il necessario. Niente di superfluo.
          </h2>
          <p className="text-pretty text-[17px] text-forge-muted">
            Uno strumento essenziale costruito per chi si allena sul serio e tiene alla propria privacy.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-5 md:grid-cols-3">
          {features.map(({ icon: Icon, title, description }) => (
            <article
              key={title}
              className="rounded-[14px] border border-forge-border bg-forge-elev p-7 transition-all hover:-translate-y-1 hover:border-forge-cyan-dim hover:glow-cyan-soft"
            >
              <div
                className="mb-[22px] grid size-12 place-items-center rounded-xl border border-forge-cyan-dim bg-forge-cyan/[0.06] text-forge-cyan"
                aria-hidden="true"
              >
                <Icon className="size-6" />
              </div>
              <h3 className="mb-2.5 text-xl font-bold">{title}</h3>
              <p className="text-[15px] text-forge-muted">{description}</p>
            </article>
          ))}
        </div>
      </div>
    </section>
  )
}
