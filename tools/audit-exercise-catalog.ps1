param([string]$OutputPath = 'artifacts/exercise-catalog-audit.json')
$ErrorActionPreference = 'Stop'
$items = [Collections.Generic.Dictionary[string,object]]::new()
$cursor = ''
$seenCursors = [Collections.Generic.HashSet[string]]::new()
do {
    $url = 'https://oss.exercisedb.dev/api/v1/exercises?limit=25'
    if ($cursor) { $url += '&after=' + [Uri]::EscapeDataString($cursor) }
    Start-Sleep -Milliseconds 1500
    $r = Invoke-RestMethod $url -TimeoutSec 30 -MaximumRetryCount 3 -RetryIntervalSec 20
    foreach ($exercise in $r.data) { $items[$exercise.exerciseId] = $exercise }
    $cursor = $r.meta.nextCursor
    if ($r.meta.hasNextPage -and (!$cursor -or !$seenCursors.Add($cursor))) { throw 'Pagination stalled' }
} while ($r.meta.hasNextPage)
$all = @($items.Values)
$withGif = @($all | Where-Object { $_.gifUrl }).Count
$withInstructions = @($all | Where-Object { @($_.instructions | Where-Object { $_ -and $_.Trim() }).Count -gt 0 }).Count
$report = [ordered]@{
    CheckedAtUtc = [DateTime]::UtcNow.ToString('o'); Source = 'ExerciseDB v1'; AdvertisedTotal = $r.meta.total
    UniqueExercises = $all.Count; WithGifUrl = $withGif; WithoutGifUrl = $all.Count - $withGif
    WithWrittenInstructions = $withInstructions
    GifAndInstructions = @($all | Where-Object { $_.gifUrl -and @($_.instructions | Where-Object { $_ -and $_.Trim() }).Count -gt 0 }).Count
    StaticImageField = 'Not provided by this dataset'; MediaReachability = 'URLs counted; media not individually downloaded'
    PocketBaseCount = 'Requires an authenticated account; not inferred from anonymous filtered responses'
}
$directory = Split-Path -Parent $OutputPath
if ($directory) { New-Item -ItemType Directory -Force -Path $directory | Out-Null }
$report | ConvertTo-Json | Set-Content $OutputPath
$report | ConvertTo-Json


