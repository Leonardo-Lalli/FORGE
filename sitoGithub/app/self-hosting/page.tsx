"use client"

import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { CheckCircle2, Server, Shield, Wifi, Globe, QrCode, Monitor, Users, Zap, Lock } from "lucide-react"

function Eyebrow({ children }: { children: React.ReactNode }) {
  return <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
}

function Card({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`rounded-[14px] border border-forge-border bg-forge-elev p-6 ${className}`}>{children}</div>
}

export default function SelfHostingPage() {
  const { t } = useI18n()
  const r = t.roadmapPage

  const advantages = [
    { icon: Server, title: "Il tuo server, le tue regole", desc: "Nessun cloud di terze parti. PocketBase gira su un singolo binario con database SQLite, autenticazione e file storage integrati. Lo avvii dove vuoi: Raspberry Pi, NAS, mini-PC, Proxmox, VPS." },
    { icon: Lock, title: "Privacy by design", desc: "I dati degli utenti non attraversano mai internet verso server sconosciuti. Tutto rimane dentro la rete locale. Nessun tracker, nessuna pubblicità, nessuna analytics di terze parti." },
    { icon: Zap, title: "LAN-first, offline-ready", desc: "L'app comunica direttamente col server via Wi-Fi locale. Se internet cade, l'allenamento continua. I workout si salvano in SQLite sul telefono e si sincronizzano appena la rete torna." },
    { icon: Globe, title: "Accesso remoto opzionale", desc: "Con DuckDNS + Nginx Proxy Manager puoi esporre il tuo server su internet con HTTPS (Let's Encrypt) e certificate pinning. I tuoi amici possono seguirti anche da fuori casa." },
  ]

  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main>
          {/* Hero */}
          <section className="px-6 pb-16 pt-20">
            <div className="mx-auto w-full max-w-[1120px]">
              <p className="mb-4 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Self-Hosting</p>
              <h1 className="mb-6 max-w-[700px] text-balance text-[clamp(32px,6vw,52px)] font-extrabold leading-[1.08] tracking-[-0.02em]">
                Il tuo server,{" "}
                <span className="text-forge-cyan text-glow-cyan">le tue regole.</span>
              </h1>
              <p className="max-w-[600px] text-[18px] leading-relaxed text-forge-muted">
                FORGE è costruito attorno a un&apos;idea semplice: i tuoi dati di allenamento dovrebbero appartenere solo a te.
                Con PocketBase self-hosted e Docker, avvii il backend in 30 secondi su qualsiasi macchina Linux.
              </p>
            </div>
          </section>

          {/* How it works */}
          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <Eyebrow>// Come funziona</Eyebrow>
              <h2 className="mb-8 text-[clamp(26px,4vw,34px)] font-extrabold">Tre passi per l&apos;indipendenza</h2>
              <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
                {[
                  { step: "01", title: "Avvia il backend", body: "docker compose up -d sul tuo server. PocketBase parte con database, auth e storage in un container." },
                  { step: "02", title: "Installa l'app", body: "Scarica l'APK, aprilo. Al primo avvio inserisci l'IP del tuo server (es. 192.168.1.50:8090)." },
                  { step: "03", title: "Allenati in libertà", body: "Registrati, inizia ad allenarti. I tuoi dati non lasciano mai la tua rete. Zero costi cloud, per sempre." },
                ].map((s) => (
                  <Card key={s.step}>
                    <span className="mb-4 block font-mono text-4xl font-extrabold text-forge-cyan/20">{s.step}</span>
                    <h3 className="mb-2 text-lg font-bold">{s.title}</h3>
                    <p className="text-[14px] leading-relaxed text-forge-muted">{s.body}</p>
                  </Card>
                ))}
              </div>
            </div>
          </section>

          {/* Vantaggi */}
          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <Eyebrow>// Vantaggi</Eyebrow>
              <h2 className="mb-8 text-[clamp(26px,4vw,34px)] font-extrabold">Perché self-hostare FORGE</h2>
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
                {advantages.map((a, i) => (
                  <Card key={i} className="flex gap-4">
                    <a.icon className="mt-0.5 size-6 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
                    <div>
                      <h3 className="mb-1.5 text-base font-bold">{a.title}</h3>
                      <p className="text-[14px] leading-relaxed text-forge-muted">{a.desc}</p>
                    </div>
                  </Card>
                ))}
              </div>
            </div>
          </section>

          {/* Hardware */}
          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <Eyebrow>// Hardware consigliato</Eyebrow>
              <h2 className="mb-8 text-[clamp(26px,4vw,34px)] font-extrabold">Cosa ti serve</h2>
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
                {[
                  { icon: "🍓", title: "Raspberry Pi 4/5", desc: "~60€. Silenzioso, consuma 5W. Perfetto per uso personale o familiare. Supporta 10-50 utenti attivi." },
                  { icon: "💻", title: "Mini-PC / NUC", desc: "~200€. Più potente, ideale per palestre o community. Supporta 100-500 utenti. Puoi farci girare anche Nginx." },
                  { icon: "☁️", title: "VPS (Hetzner, Linode)", desc: "~5€/mese. Server virtuale in datacenter. Accesso pubblico nativo. Ideale per community online." },
                ].map((h, i) => (
                  <Card key={i}>
                    <span className="mb-3 block text-3xl">{h.icon}</span>
                    <h3 className="mb-2 text-base font-bold">{h.title}</h3>
                    <p className="text-[14px] leading-relaxed text-forge-muted">{h.desc}</p>
                  </Card>
                ))}
              </div>
            </div>
          </section>

          {/* B2B Section */}
          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <Eyebrow>// FORGE Local Business</Eyebrow>
              <h2 className="mb-3 text-[clamp(26px,4vw,36px)] font-extrabold">{r.b2b.title}</h2>
              <p className="mb-10 max-w-[640px] text-[17px] text-forge-muted">{r.b2b.body}</p>

              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 mb-12">
                {r.b2b.advantages.map((a: any, i: number) => (
                  <Card key={i}>
                    <CheckCircle2 className="mb-3 size-5 text-forge-cyan" aria-hidden="true" />
                    <h3 className="mb-1.5 text-base font-bold">{a.title}</h3>
                    <p className="text-[14px] text-forge-muted">{a.desc}</p>
                  </Card>
                ))}
              </div>

              <div className="rounded-[14px] border border-forge-border bg-forge-elev p-8">
                <div className="flex flex-col gap-6 sm:flex-row sm:items-start">
                  <div className="flex-1">
                    <p className="mb-1 font-mono text-xs uppercase tracking-[0.14em] text-forge-cyan">Architettura</p>
                    <pre className="overflow-x-auto font-mono text-[13px] leading-[1.8] text-forge-muted mt-3">
{`┌──────────────────────────┐       ┌──────────────────────────────┐
│   Smartphone clienti     │       │   Mini-PC in reception       │
│   (Wi-Fi palestra)       │───▶   │   Docker: PocketBase + NPM  │
│                          │       │   IP statico: 192.168.1.50  │
└──────────────────────────┘       └──────────────────────────────┘
                                              │
                                    ┌─────────▼──────────┐
                                    │  TV in sala pesi    │
                                    │  Leaderboard live   │
                                    └────────────────────┘`}</pre>
                  </div>
                </div>
              </div>

              <div className="mt-12">
                <h3 className="mb-5 text-xl font-bold">{r.b2b.futureTitle}</h3>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  {r.b2b.future.map((f: any, i: number) => (
                    <Card key={i} className="flex items-start gap-4">
                      <QrCode className="mt-0.5 size-5 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
                      <div>
                        <p className="text-[15px] font-semibold">{f.title}</p>
                        <p className="text-[13px] text-forge-muted mt-0.5">{f.desc}</p>
                      </div>
                    </Card>
                  ))}
                </div>
              </div>
            </div>
          </section>

          {/* Deploy commands */}
          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <Eyebrow>// Quick start</Eyebrow>
              <h2 className="mb-8 text-[clamp(26px,4vw,34px)] font-extrabold">Avvia il backend ora</h2>
              <div className="rounded-[14px] border border-forge-border bg-forge-code p-6 font-mono text-[13px] leading-[1.8] text-forge-code-fg overflow-x-auto">
                <p className="text-forge-dim"># 1. Clona la repo</p>
                <p><span className="text-forge-cyan">git clone</span> https://github.com/Leonardo-Lalli/FORGE.git</p>
                <p><span className="text-forge-cyan">cd</span> FORGE</p>
                <p className="mt-3 text-forge-dim"># 2. Crea il file .env (opzionale per encryption key)</p>
                <p><span className="text-forge-cyan">echo</span> &quot;PB_ENCRYPTION_KEY=your_secret_key&quot; &gt; .env</p>
                <p className="mt-3 text-forge-dim"># 3. Avvia PocketBase con Docker Compose (auto-configura tutto)</p>
                <p><span className="text-forge-cyan">docker compose</span> up -d</p>
                <p className="mt-3 text-forge-dim"># 4. L&apos;IP del server viene mostrato automaticamente</p>
                <p><span className="text-forge-cyan">docker compose</span> logs show-ip</p>
                <p className="text-forge-dim" style={{ fontSize: '11px', opacity: 0.6, marginTop: '4px' }}>Tutto preconfigurato: admin, collezioni, API rules, nessun setup manuale</p>
                <p className="mt-3 text-forge-dim"># 5. Admin Panel gi&agrave; pronto</p>
                <p>Apri http://localhost:8090/_/ → login con admin@forge.local / forgeadmin123</p>
                <p className="text-forge-dim" style={{ fontSize: '11px', opacity: 0.6 }}>Cambia subito la password!</p>
                <p className="mt-3 text-forge-dim"># 6. Configura l&apos;app sul telefono</p>
                <p>Apri FORGE → Setup iniziale → Inserisci l&apos;URL mostrato dallo step 4</p>
              </div>
            </div>
          </section>
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
