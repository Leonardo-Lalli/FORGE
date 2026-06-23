$ErrorActionPreference = "SilentlyContinue"

# Match EXACT app filter logic
$filters = @{
  "Petto" = @("chest","pectorals")
  "Schiena" = @("back","lats","upper back","spine","traps","trapezius","rhomboids","rear delt","infraspinatus","teres")
  "Spalle" = @("shoulders","delts","deltoids","front delt","side delt")
  "Bicipiti" = @("biceps","brachialis")
  "Tricipiti" = @("triceps")
  "Gambe" = @("upper legs","lower legs","glutes","hamstrings","quads","quadriceps","calves","abductors","hip flexors","adductors")
  "Addominali" = @("waist","abs","core","obliques","rectus abdominis")
  "Cardio" = @("cardio","cardiovascular system")
}

$equipFilters = @("body weight","dumbbell","barbell","cable","machine","band","kettlebell")

$all = @(); $noGif = @(); $cursor = ""; $page = 0
Write-Host "Fetching ALL exercises..." -ForegroundColor Cyan

while ($page -lt 60) {
  $url = "https://oss.exercisedb.dev/api/v1/exercises?limit=25"
  if ($cursor) { $url += "&cursor=$cursor" }
  $ok = $false
  for ($r = 0; $r -lt 5; $r++) {
    try { $resp = Invoke-RestMethod -Uri $url -TimeoutSec 20; $ok = $true; break }
    catch { Start-Sleep 3 }
  }
  if (-not $ok) { Write-Host "STOP at page $page ($($all.Count))" -ForegroundColor Yellow; break }
  $new = @($resp.data).Count
  foreach ($e in $resp.data) {
    if ([string]::IsNullOrWhiteSpace($e.gifUrl)) { $noGif += $e } else { $all += $e }
  }
  if ($page % 5 -eq 0) { Write-Host "  Page $page : $($all.Count) with GIF, $($noGif.Count) without" }
  if (-not $resp.meta.hasNextPage) { break }
  $cursor = $resp.meta.nextCursor; $page++
  Start-Sleep 0.4
}

Write-Host "`nTOTAL: $($all.Count) with GIF | $($noGif.Count) without GIF | $($all.Count+$noGif.Count) fetched" -ForegroundColor Cyan

# Filter coverage test
$filterCoverage = @{}
$unmatched = @()
foreach ($ex in $all) {
  $bp = [string[]]$ex.bodyParts
  $ta = [string[]]$ex.targetMuscles
  $sm = [string[]]$ex.secondaryMuscles
  $matched = $false
  foreach ($fn in $filters.Keys) {
    foreach ($t in $filters[$fn]) {
      if ($bp -contains $t -or $ta -contains $t -or $sm -contains $t) { 
        $filterCoverage[$ex.exerciseId] = $true; $matched = $true; break 
      }
    }
    if ($matched) { break }
  }
  if (-not $matched) { $unmatched += $ex }
}

Write-Host "`n=== FILTER COVERAGE ===" -ForegroundColor Cyan
Write-Host "Matched: $($filterCoverage.Count) / $($all.Count)" -ForegroundColor $(if($filterCoverage.Count -eq $all.Count){'Green'}else{'Red'})

Write-Host "`n=== PER-FILTER COUNTS (with GIF only) ===" -ForegroundColor Cyan
foreach ($fn in $filters.Keys) {
  $terms = $filters[$fn]
  $c = ($all | Where-Object { 
    $bp=[string[]]$_.bodyParts; $ta=[string[]]$_.targetMuscles; $sm=[string[]]$_.secondaryMuscles
    ($terms | Where-Object { $bp -contains $_ -or $ta -contains $_ -or $sm -contains $_ }).Count -gt 0
  }).Count
  Write-Host "  $($fn.PadRight(12)) : $c" -ForegroundColor $(if($c -gt 0){'Green'}else{'Red'})
}

Write-Host "`n=== EQUIPMENT COUNTS (with GIF only) ===" -ForegroundColor Cyan
foreach ($eq in $equipFilters) {
  $c = ($all | Where-Object { ([string[]]$_.equipments) -contains $eq }).Count
  Write-Host "  $($eq.PadRight(12)) : $c" -ForegroundColor $(if($c -gt 0){'Green'}else{'Red'})
}

if ($unmatched.Count -gt 0) {
  Write-Host "`n=== FIRST 15 UNMATCHED (with GIF) ===" -ForegroundColor Yellow
  $unmatched | Select-Object -First 15 | ForEach-Object {
    Write-Host "  $($_.name)" -ForegroundColor White
    Write-Host "    bp: [$($_.bodyParts -join ', ')]  tm: [$($_.targetMuscles -join ', ')]  sm: [$($_.secondaryMuscles -join ', ')]" -ForegroundColor Gray
    Write-Host "    eq: [$($_.equipments -join ', ')]" -ForegroundColor DarkGray
  }
}

if ($noGif.Count -gt 0) {
  Write-Host "`n=== FIRST 10 WITHOUT GIF ===" -ForegroundColor Yellow
  $noGif | Select-Object -First 10 | ForEach-Object {
    Write-Host "  $($_.name)" -ForegroundColor White
    Write-Host "    bp: [$($_.bodyParts -join ', ')]  tm: [$($_.targetMuscles -join ', ')]" -ForegroundColor Gray
  }
}

# All unique terms
$allBp = @{}; $allTm = @{}; $allSm = @{}; $allEq = @{}
foreach ($ex in $all) {
  foreach ($b in $ex.bodyParts) { $allBp["$b"]++ }
  foreach ($t in $ex.targetMuscles) { $allTm["$t"]++ }
  foreach ($s in $ex.secondaryMuscles) { $allSm["$s"]++ }
  foreach ($e in $ex.equipments) { $allEq["$e"]++ }
}
Write-Host "`n=== ALL BODY PARTS ===" -ForegroundColor Cyan
$allBp.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object { Write-Host "  $($_.Key) : $($_.Value)" }
Write-Host "`n=== ALL TARGET MUSCLES ===" -ForegroundColor Cyan
$allTm.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object { Write-Host "  $($_.Key) : $($_.Value)" }
Write-Host "`n=== ALL SECONDARY ===" -ForegroundColor Cyan
$allSm.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object { Write-Host "  $($_.Key) : $($_.Value)" }
Write-Host "`n=== ALL EQUIPMENT ===" -ForegroundColor Cyan
$allEq.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object { Write-Host "  $($_.Key) : $($_.Value)" }
