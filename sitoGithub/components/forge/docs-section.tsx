"use client"

import { useState } from "react"
import {
  BookText,
  Boxes,
  GitBranch,
  ScrollText,
  CheckCircle2,
  CircleDashed,
  XCircle,
  AlertTriangle,
} from "lucide-react"
import type { LucideIcon } from "lucide-react"

type TabId = "spec" | "architettura" | "roadmap" | "diario"

const tabs: { id: TabId; label: string; icon: LucideIcon }[] = [
  { id: "spec", label: "Specifica", icon: BookText },
  { id: "architettura", label: "Architettura", icon: Boxes },
  { id: "roadmap", label: "Roadmap", icon: GitBranch },
  { id: "diario", label: "Diario", icon: ScrollText },
]

/* ---------------------------------- Specifica ---------------------------------- */

const targetUsers = [
  "Principianti che iniziano ad allenarsi e cercano struttura",
  "Appassionati di fitness che vogliono tracciare i progressi nel tempo",
  "Gruppi di amici che vogliono motivarsi con una competizione sana",
]

const mvpFeatures = [
  "Autenticazione PocketBase (email/password/username) con auto-login",
  "Catalogo esercizi ExerciseDB con ricerca e filtri per muscolo e attrezzatura",
  "Registrazione allenamenti: serie, ripetizioni, peso e rest timer configurabile",
  "Dashboard con streak settimanale, squad activity e today card",
  "Feed sociale: follow, like e richieste con notifiche",
  "Statistiche: grafico volume, top lifts, calendario e filtri temporali",
  "Profilo con avatar, bio e storico allenamenti",
  "Piani di allenamento salvabili (Quick Start o da piano)",
  "Doppio tema chiaro/scuro con toggle runtime",
]

const requirements = [
  { id: "FR-01", text: "Recupero esercizi da ExerciseDB API con lista e ricerca", done: true },
  { id: "FR-02", text: "Cache esercizi su PocketBase con URL immagini risolti", done: true },
  { id: "FR-05", text: "Salvataggio allenamento completo su PocketBase", done: true },
  { id: "FR-08", text: "Statistiche di progresso: volume, top lifts, calendario", done: true },
  { id: "FR-13", text: "Feed allenamenti amici con like", done: true },
  { id: "FR-15", text: "Streak settimanale in Dashboard e Profilo", done: true },
  { id: "FR-07", text: "Tracking peso corporeo e misure", done: false },
  { id: "FR-14", text: "Leaderboard settimanale", done: false },
]

/* --------------------------------- Architettura -------------------------------- */

const stack = [
  { tech: ".NET MAUI 10", role: "Framework cross-platform Android-first" },
  { tech: "CommunityToolkit.Mvvm 8.4", role: "MVVM: ObservableProperty, RelayCommand, Messenger" },
  { tech: "Shell", role: "Navigazione 3 tab + 8 route di dettaglio" },
  { tech: "PocketBase", role: "Auth, database remoto, file storage, social graph" },
  { tech: "ExerciseDB v1", role: "Catalogo 1.500+ esercizi con GIF, gratuito" },
  { tech: "SQLite", role: "Cache offline: workout, esercizi, piani, achievement" },
  { tech: "SecureStorage", role: "Password e dati sensibili (Android Keystore)" },
  { tech: "Nginx Proxy Manager", role: "Reverse proxy HTTPS, Let's Encrypt, rate limit" },
]

const collections = [
  { name: "users", fields: "email, password, name, bio, avatar" },
  { name: "logged_workouts", fields: "user, name, date, exercises, volume, duration, likes, liked_by" },
  { name: "social_graph", fields: "from_user, from_name, to_user, status" },
  { name: "excercise", fields: "name, bodyPart, equipment, instructions, imageUrl, level" },
]

const repoTree = `src/Forge/
├── App.xaml(.cs)            # Entry point + auto-login
├── AppShell.xaml(.cs)       # TabBar + route di dettaglio
├── MauiProgram.cs           # DI composition root
├── Models/                  # Entità dominio e DTO
├── Services/                # PocketBase, ExerciseDB, Theme, Sync...
├── ViewModels/              # MVVM (BaseViewModel + 11 VM)
├── Views/                   # Pagine XAML pure
├── Converters/
└── Resources/               # Styles, Fonts, Raw/forge.env`

/* ----------------------------------- Roadmap ----------------------------------- */

type Status = "done" | "partial" | "skip"

const iterations: { id: string; goal: string; status: Status }[] = [
  { id: "IT-01", goal: "Bootstrap MAUI + Shell 3-tab + Design System + ThemeService", status: "done" },
  { id: "IT-02/03/08", goal: "Catalogo esercizi + Allenamento + Piani salvabili", status: "done" },
  { id: "IT-05/06/07", goal: "Auth PocketBase + Social + Dashboard + Stats + Profilo", status: "done" },
  { id: "IT-FEAT-01", goal: "SQLite offline (DatabaseService, sync auto)", status: "done" },
  { id: "IT-FEAT-03", goal: "ExerciseDB v1 free API (no API key, 1.500 esercizi)", status: "done" },
  { id: "IT-FEAT-06", goal: "Achievement system: 48 badge in 6 categorie", status: "done" },
  { id: "IT-SEC-01/02/03", goal: "Security audit, hardening e certificate pinning", status: "done" },
  { id: "Push Notifications", goal: "FCM hook pronto, SDK non compatibile con .NET 10", status: "partial" },
  { id: "IT-04", goal: "Tracking peso corporeo e misure", status: "skip" },
]

const statusMeta: Record<Status, { label: string; icon: LucideIcon; cls: string }> = {
  done: { label: "Completata", icon: CheckCircle2, cls: "text-forge-cyan border-forge-cyan-dim bg-forge-cyan/[0.06]" },
  partial: { label: "Parziale", icon: AlertTriangle, cls: "text-amber-300 border-amber-300/25 bg-amber-300/[0.06]" },
  skip: { label: "Posticipata", icon: XCircle, cls: "text-forge-dim border-forge-border bg-forge-elev" },
}

/* ------------------------------------ Diario ----------------------------------- */

const journal = [
  {
    phase: "IT-01",
    title: "Bootstrap MAUI e Design System",
    body: "Progetto .NET MAUI con Shell a 3 tab, temi runtime chiaro/scuro e font Google. Decisione chiave: 3 tab invece di 5 per una UI più pulita.",
  },
  {
    phase: "IT-02/03/08",
    title: "Catalogo, Allenamento e Piani",
    body: "Integrazione ExerciseDB, flusso allenamento completo con rest timer e piani salvabili. Risolti short URL bloccati dagli ISP e crash da CollectionView annidate.",
  },
  {
    phase: "IT-05/06/07",
    title: "Auth, Social, Dashboard, Stats e Profilo",
    body: "PocketBase self-hosted al posto di Firebase, feed con like, statistiche e profilo. Streak calcolata settimanalmente con tolleranza di 7 giorni.",
  },
  {
    phase: "IT-FEAT-01 → 07",
    title: "Offline, ExerciseDB v1, CSV, Foto, Achievement",
    body: "SQLite + sync automatico, migrazione all'API gratuita (1.500 esercizi), import/export CSV, foto progresso e un sistema di 48 achievement.",
  },
  {
    phase: "IT-SEC-01 → 03",
    title: "Sicurezza e Hardening",
    body: "Password su SecureStorage, IHttpClientFactory, token fuori dagli URL, certificate pinning Let's Encrypt e API Rules con row-level ownership.",
  },
]

/* --------------------------------- Componenti UI -------------------------------- */

function Eyebrow({ children }: { children: React.ReactNode }) {
  return (
    <p className="mb-3 font-mono text-xs uppercase tracking-[0.16em] text-forge-cyan">{children}</p>
  )
}

function Card({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return (
    <div className={`rounded-[14px] border border-forge-border bg-forge-elev p-6 ${className}`}>{children}</div>
  )
}

function SpecPanel() {
  return (
    <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
      <Card>
        <Eyebrow>// Visione</Eyebrow>
        <h3 className="mb-2.5 text-lg font-bold">Tracking + social in un&apos;unica app</h3>
        <p className="text-[15px] leading-relaxed text-forge-muted">
          FORGE risolve i tre ostacoli di chi va in palestra: ricordare i carichi, sapere quali esercizi fare e restare
          motivati. Un&apos;app .NET MAUI Android-first che unisce catalogo esercizi, registrazione allenamenti,
          statistiche e una dimensione sociale con feed e like.
        </p>
        <div className="mt-5">
          <Eyebrow>// Utenti target</Eyebrow>
          <ul className="flex flex-col gap-2.5">
            {targetUsers.map((u) => (
              <li key={u} className="flex items-start gap-2.5 text-[14px] text-forge-muted">
                <span className="mt-0.5 flex-shrink-0 text-forge-cyan" aria-hidden="true">
                  ▸
                </span>
                <span>{u}</span>
              </li>
            ))}
          </ul>
        </div>
      </Card>

      <Card>
        <Eyebrow>// Ambito MVP</Eyebrow>
        <h3 className="mb-3.5 text-lg font-bold">Funzionalità implementate</h3>
        <ul className="flex flex-col gap-2.5">
          {mvpFeatures.map((f) => (
            <li key={f} className="flex items-start gap-2.5 text-[14px] text-forge-muted">
              <CheckCircle2 className="mt-0.5 size-4 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
              <span>{f}</span>
            </li>
          ))}
        </ul>
      </Card>

      <Card className="lg:col-span-2">
        <Eyebrow>// Requisiti funzionali</Eyebrow>
        <div className="grid grid-cols-1 gap-x-8 gap-y-3 sm:grid-cols-2">
          {requirements.map((r) => (
            <div key={r.id} className="flex items-start gap-3 border-b border-forge-border/60 pb-3">
              {r.done ? (
                <CheckCircle2 className="mt-0.5 size-4 flex-shrink-0 text-forge-cyan" aria-hidden="true" />
              ) : (
                <CircleDashed className="mt-0.5 size-4 flex-shrink-0 text-forge-dim" aria-hidden="true" />
              )}
              <div>
                <span className="font-mono text-xs text-forge-cyan">{r.id}</span>
                <p className={`text-[14px] ${r.done ? "text-forge-muted" : "text-forge-dim line-through"}`}>
                  {r.text}
                </p>
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  )
}

function ArchPanel() {
  return (
    <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
      <Card>
        <Eyebrow>// Stack tecnologico</Eyebrow>
        <div className="flex flex-col">
          {stack.map((s, i) => (
            <div
              key={s.tech}
              className={`grid grid-cols-[auto_1fr] items-baseline gap-x-4 py-2.5 ${
                i !== stack.length - 1 ? "border-b border-forge-border/60" : ""
              }`}
            >
              <span className="font-mono text-[13px] font-semibold text-forge-fg">{s.tech}</span>
              <span className="text-[13px] text-forge-muted">{s.role}</span>
            </div>
          ))}
        </div>
      </Card>

      <div className="flex flex-col gap-5">
        <Card>
          <Eyebrow>// Struttura repository</Eyebrow>
          <pre className="overflow-x-auto rounded-lg bg-forge-code p-4 font-mono text-[12.5px] leading-[1.7] text-forge-muted">
            <code className="whitespace-pre">{repoTree}</code>
          </pre>
        </Card>
        <Card>
          <Eyebrow>// Navigazione Shell</Eyebrow>
          <div className="flex flex-wrap gap-2">
            {["Dashboard", "Feed", "Stats"].map((t) => (
              <span
                key={t}
                className="rounded-full border border-forge-cyan-dim bg-forge-cyan/[0.06] px-3 py-1 font-mono text-xs text-forge-cyan"
              >
                {t}
              </span>
            ))}
            {["startSession", "activeWorkout", "workoutDetail", "achievements", "profile", "settings"].map((t) => (
              <span
                key={t}
                className="rounded-full border border-forge-border px-3 py-1 font-mono text-xs text-forge-muted"
              >
                {t}
              </span>
            ))}
          </div>
        </Card>
      </div>

      <Card className="lg:col-span-2">
        <Eyebrow>// Collezioni PocketBase</Eyebrow>
        <div className="overflow-x-auto">
          <table className="w-full border-collapse text-left">
            <thead>
              <tr className="border-b border-forge-border">
                <th className="py-2.5 pr-4 font-mono text-xs uppercase tracking-wider text-forge-dim">Collection</th>
                <th className="py-2.5 font-mono text-xs uppercase tracking-wider text-forge-dim">Campi principali</th>
              </tr>
            </thead>
            <tbody>
              {collections.map((c) => (
                <tr key={c.name} className="border-b border-forge-border/60">
                  <td className="py-2.5 pr-4 align-top font-mono text-[13px] font-semibold text-forge-cyan">
                    {c.name}
                  </td>
                  <td className="py-2.5 font-mono text-[13px] text-forge-muted">{c.fields}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  )
}

function RoadmapPanel() {
  return (
    <div className="flex flex-col gap-3">
      {iterations.map((it) => {
        const meta = statusMeta[it.status]
        const Icon = meta.icon
        return (
          <div
            key={it.id}
            className="flex flex-col gap-3 rounded-[14px] border border-forge-border bg-forge-elev p-5 sm:flex-row sm:items-center sm:justify-between"
          >
            <div className="flex items-start gap-4">
              <span className="font-mono text-[13px] font-semibold text-forge-cyan sm:w-32 sm:flex-shrink-0">
                {it.id}
              </span>
              <span className="text-[15px] text-forge-fg">{it.goal}</span>
            </div>
            <span
              className={`inline-flex w-fit flex-shrink-0 items-center gap-1.5 rounded-full border px-3 py-1 text-xs font-medium ${meta.cls}`}
            >
              <Icon className="size-3.5" aria-hidden="true" />
              {meta.label}
            </span>
          </div>
        )
      })}
    </div>
  )
}

function DiarioPanel() {
  return (
    <div className="relative pl-6 sm:pl-8">
      <div className="absolute bottom-2 left-[7px] top-2 w-px bg-forge-border sm:left-[9px]" aria-hidden="true" />
      <div className="flex flex-col gap-7">
        {journal.map((entry) => (
          <div key={entry.phase} className="relative">
            <span
              className="absolute -left-6 top-1 grid size-4 place-items-center rounded-full border border-forge-cyan bg-forge-bg sm:-left-8"
              aria-hidden="true"
            >
              <span className="size-1.5 rounded-full bg-forge-cyan glow-cyan" />
            </span>
            <p className="mb-1 font-mono text-xs uppercase tracking-[0.14em] text-forge-cyan">{entry.phase}</p>
            <h3 className="mb-1.5 text-lg font-bold">{entry.title}</h3>
            <p className="text-[15px] leading-relaxed text-forge-muted">{entry.body}</p>
          </div>
        ))}
      </div>
    </div>
  )
}

export function DocsSection() {
  const [active, setActive] = useState<TabId>("spec")

  return (
    <section id="documentazione" className="px-6 py-20">
      <div className="mx-auto w-full max-w-[1120px]">
        <div className="mb-12 max-w-[640px]">
          <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Documentazione</p>
          <h2 className="mb-4 text-balance text-[clamp(30px,5vw,44px)] font-extrabold leading-[1.1] tracking-[-0.02em]">
            Tutto il progetto, in chiaro.
          </h2>
          <p className="text-pretty text-[17px] text-forge-muted">
            Specifica, architettura, roadmap e diario di sviluppo di FORGE — la stessa documentazione che guida il
            progetto open-source.
          </p>
        </div>

        {/* Tab nav */}
        <div
          className="mb-8 flex flex-wrap gap-2 border-b border-forge-border"
          role="tablist"
          aria-label="Sezioni della documentazione"
        >
          {tabs.map(({ id, label, icon: Icon }) => {
            const isActive = active === id
            return (
              <button
                key={id}
                role="tab"
                type="button"
                aria-selected={isActive}
                aria-controls={`panel-${id}`}
                id={`tab-${id}`}
                onClick={() => setActive(id)}
                className={`-mb-px inline-flex items-center gap-2 border-b-2 px-4 py-3 text-sm font-medium transition-colors ${
                  isActive
                    ? "border-forge-cyan text-forge-cyan"
                    : "border-transparent text-forge-muted hover:text-forge-fg"
                }`}
              >
                <Icon className="size-4" aria-hidden="true" />
                {label}
              </button>
            )
          })}
        </div>

        {/* Panels */}
        <div role="tabpanel" id={`panel-${active}`} aria-labelledby={`tab-${active}`}>
          {active === "spec" && <SpecPanel />}
          {active === "architettura" && <ArchPanel />}
          {active === "roadmap" && <RoadmapPanel />}
          {active === "diario" && <DiarioPanel />}
        </div>
      </div>
    </section>
  )
}
