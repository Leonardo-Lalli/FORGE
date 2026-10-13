export const currentVersion = "0.8.5-beta"
export const currentTag = `v${currentVersion}`
export const currentReleaseUrl = `https://github.com/Leonardo-Lalli/FORGE/releases/tag/${currentTag}`

export const currentApkUrl = `https://github.com/Leonardo-Lalli/FORGE/releases/download/${currentTag}/FORGE.apk`

export function isCurrentOrNewer(tag: unknown): boolean {
  if (typeof tag !== "string") return false
  const match = tag.match(/^v?(\d+)\.(\d+)\.(\d+)(?:$|[-+])/)
  if (!match) return false
  const minimum = [0, 8, 5]
  for (let index = 0; index < minimum.length; index++) {
    const part = Number(match[index + 1])
    if (part !== minimum[index]) return part > minimum[index]
  }
  return true
}
