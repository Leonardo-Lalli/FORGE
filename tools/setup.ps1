#!/usr/bin/env pwsh
<#
.SYNOPSIS
  FORGE — Self-hosted Workout Tracker setup (Windows / macOS / Linux)
.DESCRIPTION
  Clona la repo, avvia PocketBase con Docker, crea superuser e collezioni,
  e mostra l'IP a cui collegare l'app. Zero configurazione manuale.
.EXAMPLE
  Invoke-Expression (Invoke-WebRequest -Uri https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/setup.ps1).Content
  # oppure
  .\setup.ps1
#>

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  FORGE — Self-Hosted Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Clone repo ---
$repoDir = Join-Path $env:USERPROFILE "forge-server"
$repoUrl = "https://github.com/Leonardo-Lalli/FORGE.git"

if (Test-Path $repoDir) {
    Write-Host "[INFO]  Directory $repoDir esiste, aggiorno..." -ForegroundColor Yellow
    Set-Location $repoDir
    git pull --ff-only 2>$null | Out-Null
} else {
    Write-Host "[INFO]  Clonando $repoUrl..." -ForegroundColor Yellow
    git clone $repoUrl $repoDir 2>$null | Out-Null
    Set-Location $repoDir
}
Write-Host "[ OK ]  Repository pronta" -ForegroundColor Green

# --- Start PocketBase ---
Write-Host "[INFO]  Avvio PocketBase..." -ForegroundColor Yellow
docker compose down --remove-orphans 2>$null | Out-Null
docker compose up -d pocketbase 2>$null | Out-Null

Write-Host "[INFO]  Attesa PocketBase..." -ForegroundColor Yellow
$maxWait = 60
for ($i = 0; $i -lt $maxWait; $i += 2) {
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:8090/api/health" -UseBasicParsing -TimeoutSec 2
        if ($r.StatusCode -eq 200) { break }
    } catch { }
    Start-Sleep -Seconds 2
}
Write-Host "[ OK ]  PocketBase e online" -ForegroundColor Green

# --- Create superuser ---
Write-Host "[INFO]  Creazione superuser..." -ForegroundColor Yellow
$result = docker compose exec -T pocketbase pocketbase superuser create admin@forge.local forgeadmin123 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "[ OK ]  Superuser creato" -ForegroundColor Green
} else {
    Write-Host "[ OK ]  Superuser gia esistente" -ForegroundColor Green
}

# --- Start all services ---
Write-Host "[INFO]  Creazione collezioni..." -ForegroundColor Yellow
docker compose up -d 2>$null | Out-Null
Start-Sleep -Seconds 5
docker compose logs init 2>$null | Out-String -Stream | ForEach-Object { Write-Host $_ }
Write-Host "[ OK ]  Collezioni pronte" -ForegroundColor Green

# --- Detect host IP ---
Write-Host "[INFO]  Rilevamento IP..." -ForegroundColor Yellow
if ($IsLinux -or $IsMacOS) {
    $hostIp = (ip -o -4 addr show scope global 2>$null | Select-Object -First 1 | ForEach-Object { ($_ -split '\s+')[3] -replace '/.*', '' })
    if (-not $hostIp) { $hostIp = (hostname -I 2>$null | ForEach-Object { ($_ -split '\s+')[0] }) }
} else {
    $hostIp = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" -and $_.PrefixOrigin -ne "WellKnown" -and $_.IPAddress -notlike "172.*" } | Select-Object -First 1).IPAddress
    if (-not $hostIp) { $hostIp = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" -and $_.IPAddress -notlike "172.*" } | Select-Object -First 1).IPAddress }
}
if (-not $hostIp) { $hostIp = "INDIRIZZO-NON-TROVATO" }
Write-Host "[ OK ]  IP server: $hostIp" -ForegroundColor Green

# --- Show IP ---
docker compose logs show-ip 2>$null | Out-String -Stream | ForEach-Object { Write-Host $_ }

# --- Summary ---
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  FORGE PocketBase e online!" -ForegroundColor Green
Write-Host ""
Write-Host "  App URL:  http://${hostIp}:8090" -ForegroundColor Cyan
Write-Host "  Admin:    http://localhost:8090/_/" -ForegroundColor White
Write-Host "  Login:    admin@forge.local / forgeadmin123" -ForegroundColor White
Write-Host ""
Write-Host "  CAMBIA SUBITO LA PASSWORD ADMIN!" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
