import { readFileSync } from "node:fs"
import { execFileSync } from "node:child_process"
import { fileURLToPath } from "node:url"
import path from "node:path"

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..")
const parse = source => JSON.parse(source.slice(source.indexOf("{", source.indexOf("export const")), source.lastIndexOf("}") + 1))
const read = lang => parse(readFileSync(path.join(root, "sitoGithub/lib/translations", `${lang}.ts`), "utf8"))
const features = dictionary => [...dictionary.roadmapPage.releases.flatMap(group => group.features), ...dictionary.roadmapPage.backlog.items]
const fail = text => { throw new Error(`Roadmap: ${text}`) }
const today = new Intl.DateTimeFormat("en-CA", { timeZone: "Europe/Rome", year: "numeric", month: "2-digit", day: "2-digit" }).format(new Date())
const validDate = date => /^\d{4}-\d{2}-\d{2}$/.test(date) && new Date(date).toISOString().slice(0, 10) === date && date <= today
const dictionaries = [read("it"), read("en")]
for (const [index, dictionary] of dictionaries.entries()) {
  const lang = index === 0 ? "it" : "en"
  if (dictionary.roadmapPage.difficulty) fail("difficulty labels must not return")
  let previous = null
  try { previous = features(parse(execFileSync("git", ["show", `HEAD:sitoGithub/lib/translations/${lang}.ts`], { cwd: root, encoding: "utf8", stdio: ["ignore", "pipe", "ignore"] }))) } catch { console.log(`Roadmap ${lang}: Git baseline unavailable; checking current metadata.`) }
  const ids = new Set()
  for (const feature of features(dictionary)) {
    if (!feature.id || ids.has(feature.id)) fail(`missing or duplicate id: ${feature.name}`)
    ids.add(feature.id)
    if ("diff" in feature) fail(`remove difficulty from ${feature.id}`)
    if (!Array.isArray(feature.history)) fail(`missing history for ${feature.id}`)
    if (!["planned", "candidate", "server-required", "released"].includes(feature.status)) fail(`invalid status for ${feature.id}`)
    if (feature.status !== "planned" && (!validDate(feature.implementedOn) || !feature.implementationRef.trim())) fail(`implementation date/reference required for ${feature.id}`)
    if (feature.updatedOn && !validDate(feature.updatedOn)) fail(`invalid update date for ${feature.id}`)
    if (feature.implementedOn && feature.updatedOn < feature.implementedOn) fail(`update predates implementation for ${feature.id}`)
    for (const entry of feature.history) if (!validDate(entry.date) || !entry.ref.trim() || !entry.note.trim()) fail(`incomplete history for ${feature.id}`)
    if (feature.history.length && feature.history.at(-1).date !== feature.updatedOn) fail(`latest history must match update date for ${feature.id}`)
    const before = previous?.find(item => item.id === feature.id || item.name === feature.name)
    const core = item => JSON.stringify([item.name, item.desc, item.status ?? "planned", item.implementedOn ?? ""])
    if (previous && (!before || core(before) !== core(feature)) && !feature.history.length) fail(`record this change in history: ${feature.id}`)
    if (before?.history?.length && core(before) !== core(feature) && feature.history.length <= before.history.length) fail(`append a history entry for ${feature.id}`)
  }
}
const metadata = dictionary => features(dictionary).map(f => [f.id, f.status, f.implementedOn, f.updatedOn])
if (JSON.stringify(metadata(dictionaries[0])) !== JSON.stringify(metadata(dictionaries[1]))) fail("Italian and English implementation metadata differ")
console.log("Roadmap dates, references, statuses and history: OK")
