const bullets = [
  <>
    Backend leggero basato su <strong className="font-semibold text-forge-fg">PocketBase</strong> (SQLite integrato)
  </>,
  <>
    Setup con un solo comando tramite <strong className="font-semibold text-forge-fg">Docker Compose</strong>
  </>,
  <>Sincronizzazione multi-dispositivo opzionale e crittografata</>,
  <>Zero costi ricorrenti, zero vendor lock-in</>,
]

export function SelfHostingSection() {
  return (
    <section id="self-hosting" className="px-6 py-20">
      <div className="mx-auto grid w-full max-w-[1120px] grid-cols-1 items-center gap-12 lg:grid-cols-[1fr_1.15fr]">
        <div>
          <p className="mb-3.5 font-mono text-[13px] uppercase tracking-[0.18em] text-forge-cyan">// Self-Hosting</p>
          <h2 className="mb-4 text-balance text-[clamp(30px,5vw,44px)] font-extrabold leading-[1.1] tracking-[-0.02em]">
            Il tuo server, le tue regole.
          </h2>
          <p className="text-pretty text-[17px] text-forge-muted">
            Avvia il backend FORGE in pochi secondi con Docker e PocketBase. Un singolo comando per sincronizzare i tuoi
            allenamenti tra dispositivi — completamente self-hosted.
          </p>
          <ul className="mt-7 flex flex-col gap-4">
            {bullets.map((bullet, i) => (
              <li key={i} className="flex items-start gap-3 text-[15px] text-forge-muted">
                <span className="mt-0.5 flex-shrink-0 text-forge-cyan" aria-hidden="true">
                  ▸
                </span>
                <span>{bullet}</span>
              </li>
            ))}
          </ul>
        </div>

        <div
          className="overflow-hidden rounded-[14px] border border-forge-border bg-forge-code shadow-[0_24px_60px_rgba(0,0,0,0.5)] glow-cyan-soft"
          role="img"
          aria-label="Esempio di comandi per avviare il backend FORGE con Docker Compose e PocketBase"
        >
          <div className="flex items-center gap-2 border-b border-forge-border bg-[#101010] px-[18px] py-3.5">
            <span className="size-3 rounded-full bg-[#2c2c2c]" aria-hidden="true" />
            <span className="size-3 rounded-full bg-[#2c2c2c]" aria-hidden="true" />
            <span className="size-3 rounded-full bg-[#2c2c2c]" aria-hidden="true" />
            <span className="ml-2.5 font-mono text-xs text-forge-dim">forge — backend setup</span>
          </div>
          <pre className="overflow-x-auto px-[22px] py-6 font-mono text-[13.5px] leading-[1.85]">
            <code className="whitespace-pre">
              <span className="text-forge-dim"># 1. Clona il repository ufficiale{"\n"}</span>
              <span className="text-forge-cyan">$</span>{" "}
              <span className="text-forge-fg">git clone https://github.com/forge-app/backend.git{"\n"}</span>
              <span className="text-forge-cyan">$</span> <span className="text-forge-fg">cd backend{"\n\n"}</span>
              <span className="text-forge-dim"># 2. Avvia PocketBase in background{"\n"}</span>
              <span className="text-forge-cyan">$</span>{" "}
              <span className="text-forge-fg">
                docker compose up <span className="text-[#7fdce6]">-d</span>
                {"\n\n"}
              </span>
              <span className="text-forge-dim">[+] Running 2/2{"\n"}</span>
              <span className="text-forge-dim"> ✔ Network forge_default Created{"\n"}</span>
              <span className="text-forge-dim"> ✔ Container forge-pocketbase Started{"\n\n"}</span>
              <span className="text-forge-dim"># 3. Backend pronto su http://localhost:8090{"\n"}</span>
              <span className="text-forge-cyan">$</span> <span className="text-forge-fg">docker compose ps</span>
            </code>
          </pre>
        </div>
      </div>
    </section>
  )
}
