"use client"

import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"
import { CheckCircle2, Dumbbell, BarChart3, Users, Trophy, Shield, Palette, Database, FileSpreadsheet, Timer, Camera, Search } from "lucide-react"

function Eyebrow({ children }: { children: React.ReactNode }) {
  return <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
}

export default function FeaturesPage() {
  const { t } = useI18n()

  const features = [
    { icon: Dumbbell, title: "1.500+ Esercizi con GIF", desc: "Catalogo completo da ExerciseDB. Cerca per nome, gruppo muscolare o attrezzatura. Filtri combinabili: muscolo AND attrezzatura. Ogni esercizio ha una GIF animata e istruzioni passo-passo. Cache locale su SQLite per accesso istantaneo anche offline.", status: "✅" },
    { icon: Timer, title: "Allenamento Completo", desc: "Registra serie, ripetizioni e peso per ogni esercizio. Rest timer configurabile (5-600s) con vibrazione a fine recupero. Ghost inputs: i pesi dell'ultima sessione vengono pre-compilati. Modalità Quick Start, Create Plan o avvia da piano salvato.", status: "✅" },
    { icon: Camera, title: "Foto Progresso", desc: "Scatta foto durante l'allenamento (fotocamera o galleria). Max 5 foto per workout. Visualizza il tuo progresso fisico nel tempo. Le foto sono salvate nel record allenamento e visibili nel dettaglio.", status: "✅" },
    { icon: BarChart3, title: "Statistiche Avanzate", desc: "Grafico volume a barre settimanali. Top lifts con pesi massimi per esercizio. Calendario mensile con giorni di allenamento. Filtri temporali: WEEK, MONTH, 3M, YEAR, ALL. Volume totale, ore di allenamento, streak settimanale.", status: "✅" },
    { icon: Users, title: "Feed Sociale", desc: "Cerca e segui altri utenti. Feed con gli allenamenti dei tuoi amici: nome, esercizi, volume, durata, foto. Metti like (♥) ai workout. Ricevi notifiche di like e richieste di follow. Avatar personalizzato con upload.", status: "✅" },
    { icon: Trophy, title: "48 Achievement", desc: "6 categorie: Costanza 📅, Forza 💪, Orari ⏰, Varietà 🧬, Social 🦋, Elite 👑. Tracking automatico a ogni workout salvato. Notifiche di sblocco. Vetrina badge nel profilo. Achievement nascosti da scoprire.", status: "✅" },
    { icon: Shield, title: "Sicurezza Completa", desc: "Password in SecureStorage (Android Keystore). HTTPS con certificate pinning Let's Encrypt. API rules row-level ownership. Rate limiting (5 login/min, 60 req/min). Admin panel bloccato da esterno. Backup Android disabilitato.", status: "✅" },
    { icon: Palette, title: "Doppio Tema", desc: "Tema scuro \"Cyber-Athletic Elite\" (ciano #00E5FF su nero). Tema chiaro \"Fitness Core\" (blu #0052CC su bianco). Modalità Auto: segue il tema di sistema Android. Font Inter, Lexend, Space Grotesk da Google Fonts.", status: "✅" },
    { icon: Database, title: "Offline SQLite + Sync", desc: "4 tabelle SQLite: workout, esercizi, piani, achievement. Sync automatico quando la connessione torna. I workout non salvati restano in coda (PendingSync). Draft: chiudi l'allenamento senza finire e riprendi dopo.", status: "✅" },
    { icon: FileSpreadsheet, title: "Import/Export CSV", desc: "Esporta tutti i tuoi allenamenti in formato CSV. Importa dati da altre app (colonne: Date, WorkoutName, ExerciseName, Sets, Reps, WeightKg, Notes). Validazione automatica: max 2MB e 1000 righe.", status: "✅" },
    { icon: Search, title: "Self-Hosting Docker", desc: "Avvia il backend con docker compose up -d. Funziona su LAN (Wi-Fi di casa/palestra). URL server configurabile da Impostazioni. Primo avvio: SetupPage per inserire IP del server o usare il server predefinito.", status: "✅" },
  ]

  return (
    <div className="relative min-h-screen overflow-x-hidden bg-forge-bg text-forge-fg">
      <div className="pointer-events-none fixed inset-0 z-0 forge-ambient" aria-hidden="true" />
      <div className="pointer-events-none fixed inset-0 z-0 forge-grid" aria-hidden="true" />
      <div className="relative z-10">
        <SiteHeader />
        <main>
          <section className="px-6 pb-16 pt-20">
            <div className="mx-auto w-full max-w-[1120px]">
              <p className="mb-4 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Funzionalità</p>
              <h1 className="mb-6 max-w-[700px] text-balance text-[clamp(32px,6vw,52px)] font-extrabold leading-[1.08] tracking-[-0.02em]">
                Tutto quello che serve{" "}
                <span className="text-forge-cyan text-glow-cyan">per allenarti meglio.</span>
              </h1>
              <p className="max-w-[600px] text-[18px] leading-relaxed text-forge-muted">
                Niente fronzoli, niente paywall, niente pubblicità. Solo gli strumenti essenziali per registrare,
                analizzare e condividere i tuoi allenamenti — open source e per sempre gratuito.
              </p>
            </div>
          </section>

          <section className="px-6 py-12">
            <div className="mx-auto w-full max-w-[1120px]">
              <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                {features.map((f, i) => (
                  <div key={i} className="rounded-[14px] border border-forge-border bg-forge-elev p-6 transition-all hover:-translate-y-0.5 hover:border-forge-cyan-dim">
                    <div className="flex items-start gap-4">
                      <div className="grid size-10 flex-shrink-0 place-items-center rounded-xl border border-forge-cyan-dim bg-forge-cyan/[0.06]">
                        <f.icon className="size-5 text-forge-cyan" aria-hidden="true" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-1">
                          <h3 className="text-base font-bold">{f.title}</h3>
                          <span className="text-xs">{f.status}</span>
                        </div>
                        <p className="text-[14px] leading-relaxed text-forge-muted">{f.desc}</p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </section>

          <section className="px-6 py-16">
            <div className="mx-auto w-full max-w-[1120px]">
              <div className="rounded-[20px] border border-forge-cyan-dim bg-gradient-to-b from-forge-cyan/[0.06] to-forge-cyan/[0.01] px-6 py-12 text-center glow-cyan-soft">
                <Eyebrow>// In numeri</Eyebrow>
                <div className="grid grid-cols-2 gap-8 sm:grid-cols-4 mt-6">
                  {[
                    { value: "1.500+", label: "Esercizi" },
                    { value: "48", label: "Achievement" },
                    { value: "10", label: "Pagine XAML" },
                    { value: "27", label: "Test xUnit" },
                  ].map((s, i) => (
                    <div key={i}>
                      <p className="text-3xl font-extrabold text-forge-cyan">{s.value}</p>
                      <p className="text-sm text-forge-muted mt-1">{s.label}</p>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </section>
        </main>
        <SiteFooter />
      </div>
    </div>
  )
}
