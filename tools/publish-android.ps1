[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$KeyStore,
    [Parameter(Mandatory = $true)][string]$KeyAlias,
    [string]$OutputApk = 'artifacts/FORGE-develop.apk',
    [string]$DisplayVersion,
    [int]$VersionCode = 0
)

$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if (!$env:FORGE_KEY_PASS -or !$env:FORGE_STORE_PASS) {
    throw 'Set FORGE_KEY_PASS and FORGE_STORE_PASS in the environment; never pass passwords as arguments.'
}
$signingPath = (Resolve-Path -LiteralPath $KeyStore).Path
$outputPath = [IO.Path]::GetFullPath((Join-Path $root $OutputApk))
if (!$outputPath.StartsWith($root + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'The APK output must stay inside the repository.'
}

# Incremental Release builds have produced wrappers without their native JNI
# entry points. Regenerate both directories and test the packaged binaries.
foreach ($relative in @('src/Forge/bin/Release/net10.0-android', 'src/Forge/obj/Release/net10.0-android')) {
    $target = [IO.Path]::GetFullPath((Join-Path $root $relative))
    if (!$target.StartsWith($root + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase) -or $target -notlike '*net10.0-android') {
        throw 'Unsafe Android intermediate path.'
    }
    if (Test-Path -LiteralPath $target) { Remove-Item -LiteralPath $target -Recurse -Force }
}

$publishArgs = @('publish', (Join-Path $root 'src/Forge/Forge.csproj'), '-f', 'net10.0-android', '-c', 'Release',
    '-p:TargetFrameworks=net10.0-android', '-p:AndroidPackageFormats=apk', '-p:PublishTrimmed=false',
    '-p:RunAOTCompilation=false', '-p:EmbedAssembliesIntoApk=true', '-p:AndroidKeyStore=true',
    "-p:AndroidSigningKeyStore=$signingPath", "-p:AndroidSigningKeyAlias=$KeyAlias",
    '-p:AndroidSigningKeyPass=env:FORGE_KEY_PASS', '-p:AndroidSigningStorePass=env:FORGE_STORE_PASS', '-v', 'minimal')
if ($DisplayVersion) { $publishArgs += "-p:ApplicationDisplayVersion=$DisplayVersion" }
if ($VersionCode -gt 0) { $publishArgs += "-p:ApplicationVersion=$VersionCode" }
& dotnet @publishArgs
if ($LASTEXITCODE -ne 0) { throw 'Android Release publish failed.' }
$signed = Join-Path $root 'src/Forge/bin/Release/net10.0-android/publish/com.leoforge.forge-Signed.apk'
& (Join-Path $PSScriptRoot 'test-android-startup.ps1') -PackageOnly -Apk $signed -OutputDirectory (Join-Path $root 'artifacts/android-package-check')
New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($outputPath)) -Force | Out-Null
Copy-Item -LiteralPath $signed -Destination $outputPath -Force
$hash = (Get-FileHash -LiteralPath $outputPath -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $([IO.Path]::GetFileName($outputPath))" | Set-Content -LiteralPath "$outputPath.sha256" -Encoding ascii
Write-Output "Validated Android Release APK: $outputPath"
