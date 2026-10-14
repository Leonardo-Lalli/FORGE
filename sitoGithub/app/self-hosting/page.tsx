"use client"

import { useState } from "react"
import { Copy, Check, Terminal, ArrowRight } from "lucide-react"
import { SiteHeader } from "@/components/forge/site-header"
import { SiteFooter } from "@/components/forge/cta-footer"
import { useI18n } from "@/lib/i18n"

const scriptUrl = "https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/install.sh"

export default function SelfHostingPage() {
  const { t } = useI18n()
  const g = t.selfHostingGuide
  const install = t.selfHostingInstall
  const [mode, setMode] = useState("docker")
  const [copied, setCopied] = useState(false)
  const [copyFailed, setCopyFailed] = useState(false)
  const selected = install.modes.find(item => item.mode === mode)!
  const command = `bash -c "$(curl -fsSL ${scriptUrl})" -- ${mode}`
  async function copyCommand() {
    try { await navigator.clipboard.writeText(command); setCopied(true); setCopyFailed(false) }
    catch { setCopyFailed(true) }
  }
  return (
    <div className="min-h-screen bg-forge-bg text-forge-fg">
      <SiteHeader />
      <main className="px-6 py-16 sm:py-20">
        <div className="mx-auto max-w-[1120px]">
          <p className="mb-4 font-mono text-xs text-forge-cyan">// SELF-HOSTING</p>
          <h1 className="mb-6 text-4xl font-extrabold">{g.title}</h1>
          <p className="mb-10 max-w-[760px] text-lg leading-relaxed text-forge-muted">{g.intro}</p>
          <section className="mb-12 rounded-3xl border border-forge-border bg-forge-elev p-6 sm:p-8">
            <Terminal className="mb-5 size-7 text-forge-cyan" aria-hidden="true" />
            <h2 className="mb-4 text-2xl font-bold">{install.title}</h2>
            <p className="mb-6 max-w-[800px] text-sm leading-relaxed text-forge-muted">{install.body}</p>
            <div className="mb-5 flex flex-wrap gap-2" role="group" aria-label={install.title}>
              {install.modes.map(item => <button key={item.mode} type="button" aria-pressed={mode === item.mode} onClick={() => { setMode(item.mode); setCopied(false); setCopyFailed(false) }} className={`rounded-full border px-4 py-2 text-sm font-medium ${mode === item.mode ? "border-forge-cyan bg-forge-cyan text-forge-on-accent" : "border-forge-border text-forge-muted hover:border-forge-cyan"}`}>{item.name}</button>)}
            </div>
            <p className="mb-4 text-sm font-medium">{selected.where}</p>
            <div className="rounded-2xl bg-forge-code p-5 text-forge-code-fg">
              <div className="mb-4 flex justify-end"><button type="button" onClick={copyCommand} className="inline-flex items-center gap-2 rounded-lg border border-[#394247] px-3 py-2 text-xs">{copied ? <Check className="size-4" aria-hidden="true" /> : <Copy className="size-4" aria-hidden="true" />}{copied ? install.copied : install.copy}</button></div>
              <pre className="overflow-x-auto whitespace-pre-wrap break-all font-mono text-xs leading-relaxed"><code>{command}</code></pre>
            </div>
            {copyFailed && <p role="status" className="mt-3 text-sm text-forge-muted">{install.copyFallback}</p>}
            <p className="mt-5 max-w-[900px] text-xs leading-relaxed text-forge-muted">{install.note}</p>
            <a href="https://github.com/Leonardo-Lalli/FORGE/blob/main/tools/install.sh" className="mt-4 inline-flex items-center gap-2 text-sm text-forge-cyan">{install.source}<ArrowRight className="size-4" aria-hidden="true" /></a>
          </section>
          <details className="rounded-2xl border border-forge-border bg-forge-elev p-6">
            <summary className="cursor-pointer text-lg font-semibold">{install.manualTitle}</summary>
          <ol className="mt-6 grid gap-5 sm:grid-cols-2">{g.steps.map((step,index) => <li key={step} className="rounded-2xl border border-forge-border bg-forge-elev p-6"><span className="mb-3 block font-mono text-forge-cyan">0{index+1}</span><p className="leading-relaxed text-forge-muted">{step}</p></li>)}</ol>
          <p className="my-8 max-w-[850px] text-sm leading-relaxed text-forge-muted">{g.note}</p>
          <a className="inline-flex rounded-full bg-forge-cyan px-6 py-3 text-sm font-semibold text-forge-on-accent" href="https://github.com/Leonardo-Lalli/FORGE/blob/main/docs/self-hosting.md">{g.cta}</a>
          </details>
          <section className="mt-12 rounded-2xl border border-forge-border bg-forge-elev p-6">
            <h2 className="mb-3 text-xl font-bold">{install.communityTitle}</h2>
            <p className="max-w-[850px] text-sm leading-relaxed text-forge-muted">{install.communityBody}</p>
          </section>
        </div>
      </main>
      <SiteFooter />
    </div>
  )
}
