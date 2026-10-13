#!/usr/bin/env pwsh
<#
.SYNOPSIS
  FORGE — Self-hosted Workout Tracker setup (Windows / macOS / Linux)
.DESCRIPTION
  Clona la repo, avvia PocketBase con Docker, crea superuser e collezioni,
  e mostra l'IP a cui collegare l'app. Credenziali, versione PocketBase e reverse proxy da configurare esplicitamente.
.EXAMPLE
  Invoke-Expression (Invoke-WebRequest -Uri https://raw.githubusercontent.com/Leonardo-Lalli/FORGE/main/tools/setup.ps1).Content
  # oppure
  .\setup.ps1
#>

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($env:PB_ADMIN_EMAIL) -or
    [string]::IsNullOrWhiteSpace($env:PB_ADMIN_PASSWORD) -or
    $env:PB_ADMIN_PASSWORD -eq 'forgeadmin123') {
    throw 'Imposta PB_ADMIN_EMAIL e una PB_ADMIN_PASSWORD unica prima di eseguire il setup.'
}

$forgeVersion = "beta - repository checkout"

Write-Host ""
Write-Host "   ███████╗ ██████╗ ██████╗  ██████╗ ███████╗" -ForegroundColor Cyan
Write-Host "   ██╔════╝██╔═══██╗██╔══██╗██╔════╝ ██╔════╝" -ForegroundColor Cyan
Write-Host "   █████╗  ██║   ██║██████╔╝██║  ███╗█████╗  " -ForegroundColor Cyan
Write-Host "   ██╔══╝  ██║   ██║██╔══██╗██║   ██║██╔══╝  " -ForegroundColor Cyan
Write-Host "   ██║     ╚██████╔╝██║  ██║╚██████╔╝███████╗" -ForegroundColor Cyan
Write-Host "   ╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚════════╝" -ForegroundColor Cyan
Write-Host ""
Write-Host "           Il diario di allenamento sociale" -ForegroundColor White
Write-Host "           ${forgeVersion} — Self-Hosted Setup" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  PocketBase | Docker | Risorse dipendenti da utenti e foto"  -ForegroundColor DarkGray
Write-Host "  Catalogo esercizi | Community su questo server | Achievement" -ForegroundColor DarkGray
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Clone repo ---
$repoDir = Join-Path $env:USERPROFILE "forge-server"
$repoUrl = "https://github.com/Leonardo-Lalli/FORGE.git"

if (Test-Path $repoDir) {
    Write-Host "[INFO]  Directory $repoDir esiste, aggiorno..." -ForegroundColor Yellow
    Set-Location $repoDir
    git pull --ff-only
    if ($LASTEXITCODE -ne 0) { throw 'Aggiornamento repository fallito.' }
} else {
    Write-Host "[INFO]  Clonando $repoUrl..." -ForegroundColor Yellow
    git clone $repoUrl $repoDir
    if ($LASTEXITCODE -ne 0) { throw 'Clone repository fallito.' }
    Set-Location $repoDir
}
Write-Host "[ OK ]  Repository pronta" -ForegroundColor Green

# --- Start PocketBase ---
Write-Host "[INFO]  Avvio PocketBase..." -ForegroundColor Yellow
docker compose up -d --build pocketbase 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Avvio PocketBase fallito.' }

Write-Host "[INFO]  Attesa PocketBase..." -ForegroundColor Yellow
$maxWait = 60
$healthy = $false
for ($i = 0; $i -lt $maxWait; $i += 2) {
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:8090/api/health" -UseBasicParsing -TimeoutSec 2
        if ($r.StatusCode -eq 200) { $healthy = $true; break }
    } catch { }
    Start-Sleep -Seconds 2
}
if (-not $healthy) { throw 'PocketBase non raggiungibile entro 60 secondi.' }
Write-Host "[ OK ]  PocketBase e online" -ForegroundColor Green

# --- Create superuser ---
Write-Host "[INFO]  Creazione superuser..." -ForegroundColor Yellow
$result = docker compose exec -T pocketbase pocketbase superuser create $env:PB_ADMIN_EMAIL $env:PB_ADMIN_PASSWORD --dir=/pb_data 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "[ OK ]  Superuser creato" -ForegroundColor Green
} else {
    Write-Host "Creazione non riuscita; l'initializer verifichera le credenziali dell'account esistente." -ForegroundColor Yellow
}

# --- Start all services ---
Write-Host "[INFO]  Creazione collezioni..." -ForegroundColor Yellow
docker compose run --rm init
if ($LASTEXITCODE -ne 0) { throw 'Inizializzazione fallita. Nessuna conferma di collezioni pronte.' }
Write-Host "[ OK ]  Collezioni iniziali pronte" -ForegroundColor Green
Write-Host "Prima di usare FORGE applica le migrazioni con backup: sudo bash tools/apply-docker-profile-migration.sh (Linux/WSL)." -ForegroundColor Yellow
Write-Host "L’installazione non è completa senza hook e migrazioni. Vedi docs/self-hosting.md." -ForegroundColor Yellow

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

# --- Summary ---
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PocketBase avviato; applica le migrazioni per completare FORGE." -ForegroundColor Green
Write-Host ""
Write-Host "  Nell'app FORGE, vai su Impostazioni > URL PocketBase" -ForegroundColor White
Write-Host "  e incolla questo indirizzo:" -ForegroundColor White
Write-Host "  Usa il dominio HTTPS del reverse proxy configurato." -ForegroundColor Cyan
Write-Host "  La 8090 e locale per default; per un proxy separato configura PB_BIND_ADDRESS e firewall." -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Admin panel: http://localhost:8090/_/" -ForegroundColor DarkGray
Write-Host "  Admin:       $env:PB_ADMIN_EMAIL (password non mostrata)" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Mantieni privato l’accesso amministrativo e conserva i backup." -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
