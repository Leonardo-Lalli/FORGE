param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^emulator-\d+$')]
    [string]$Serial,
    [string]$Adb = 'C:/Program Files (x86)/Android/android-sdk/platform-tools/adb.exe',
    [string]$Apk,
    [ValidateRange(10, 90)]
    [int]$ObserveSeconds = 40,
    [ValidateRange(1, 3)]
    [int]$Launches = 2,
    [string]$ExpectedText = 'Server|System Access',
    [string]$OutputDirectory = (Join-Path $PSScriptRoot '../artifacts/android-startup-smoke')
)

$ErrorActionPreference = 'Stop'
$package = 'com.leoforge.forge'
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

# The Release build uses marshal methods. Check the packaged binaries, because
# a successful build alone did not catch the missing JNI callback regression.
if ($Apk) {
    $archive = [IO.Compression.ZipFile]::OpenRead((Resolve-Path -LiteralPath $Apk).Path)
    try {
        foreach ($abi in @('arm64-v8a', 'x86_64')) {
            $entry = $archive.GetEntry("lib/$abi/libxamarin-app.so")
            if (!$entry) { throw "APK is missing the $abi native application library." }
            $stream = $entry.Open()
            $buffer = [IO.MemoryStream]::new()
            try {
                $stream.CopyTo($buffer)
                $binaryText = [Text.Encoding]::ASCII.GetString($buffer.ToArray())
                if (!$binaryText.Contains('Java_crc643e0735b2d7190866_MainApplication_n_1onCreate__')) {
                    throw "APK is missing the $abi native startup callback. Rebuild Android Release from clean intermediates."
                }
            }
            finally { $stream.Dispose(); $buffer.Dispose() }
        }
    }
    finally { $archive.Dispose() }
    (Get-FileHash -LiteralPath $Apk -Algorithm SHA256).Hash.ToLowerInvariant() |
        Set-Content -LiteralPath (Join-Path $OutputDirectory 'apk-sha256.txt') -Encoding ascii
    Write-Output 'PASS packaged startup callbacks: arm64-v8a and x86_64.'
}

function Invoke-Device([string[]]$Arguments, [switch]$AllowFailure) {
    $result = & $Adb -s $Serial @Arguments 2>&1
    if ($LASTEXITCODE -ne 0 -and !$AllowFailure) { throw "ADB failed: $($Arguments -join ' ')`n$($result -join "`n")" }
    return ($result -join "`n").Trim()
}

# The test never installs, uninstalls or clears application data. Use only an emulator.
$component = Invoke-Device -Arguments @('shell', 'cmd', 'package', 'resolve-activity', '--brief', '-a', 'android.intent.action.MAIN', '-c', 'android.intent.category.LAUNCHER', $package)
$component = ($component -split "`n" | Where-Object { $_ -match '^com\.leoforge\.forge/' } | Select-Object -Last 1)
if (!$component) { throw 'Install the signed Release APK on the test emulator first.' }
$component = $component.Trim()
$results = @()
for ($launch = 1; $launch -le $Launches; $launch++) {
    Invoke-Device -Arguments @('shell', 'am', 'force-stop', $package) | Out-Null
    Invoke-Device -Arguments @('logcat', '-b', 'crash', '-c') | Out-Null
    $started = Invoke-Device -Arguments @('shell', 'am', 'start', '-n', $component)
    if ($started -match 'Error:') { throw $started }
    $until = [DateTime]::UtcNow.AddSeconds($ObserveSeconds)
    $processStartDeadline = [DateTime]::UtcNow.AddSeconds(30)
    $processObserved = $false
    do {
        Start-Sleep -Seconds 2
        $crash = Invoke-Device -Arguments @('logcat', '-d', '-b', 'crash')
        if ($crash -match "Process: $([regex]::Escape($package)),") {
            $crash | Set-Content -LiteralPath (Join-Path $OutputDirectory "launch-$launch-crash.log") -Encoding utf8
            throw "FORGE crashed during launch $launch. See the saved crash log."
        }
        $appProcessIdText = Invoke-Device -Arguments @('shell', 'pidof', $package) -AllowFailure
        if (!$appProcessIdText) {
            if ($processObserved -or [DateTime]::UtcNow -ge $processStartDeadline) {
                throw "FORGE exited or did not start during launch $launch."
            }
            continue
        }
        if (!$processObserved) {
            $processObserved = $true
            $until = [DateTime]::UtcNow.AddSeconds($ObserveSeconds)
        }
    } while ([DateTime]::UtcNow -lt $until)
    $foreground = Invoke-Device -Arguments @('shell', 'dumpsys', 'activity', 'activities')
    if (($foreground -split "`n" | Where-Object { $_ -match 'topResumedActivity|mResumedActivity' }) -notmatch 'com\.leoforge\.forge/') {
        throw "FORGE did not remain in the foreground during launch $launch."
    }
    Invoke-Device -Arguments @('shell', 'uiautomator', 'dump', '--compressed', '/data/local/tmp/forge-startup-smoke.xml') | Out-Null
    $ui = Invoke-Device -Arguments @('shell', 'cat', '/data/local/tmp/forge-startup-smoke.xml')
    $ui | Set-Content -LiteralPath (Join-Path $OutputDirectory "launch-$launch-ui.xml") -Encoding utf8
    if ($ui -notmatch $ExpectedText) { throw "FORGE did not reach the expected setup/login screen during launch $launch." }
    $results += [pscustomobject]@{ launch = $launch; process = $appProcessIdText; observedSeconds = $ObserveSeconds; expectedScreen = $true }
    Write-Output "PASS launch $launch`: process alive, foreground and setup/login visible."
}
$results | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'result.json') -Encoding utf8
