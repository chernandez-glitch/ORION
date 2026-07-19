<#
.SYNOPSIS
    Publica ORION AI en modo self-contained y genera "ORION Setup.exe" con auto-update (Velopack).

.DESCRIPTION
    1. Publica Orion.Presentation self-contained (incluye runtime .NET + Windows App SDK).
    2. Empaqueta el instalador y el feed de releases con la CLI `vpk`.

.PARAMETER Version
    Versión semántica del release (por defecto 0.1.0). Increméntala en cada publicación.

.EXAMPLE
    pwsh build/pack-installer.ps1 -Version 0.2.0
#>
param(
    [string]$Version = "0.1.0",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

# 0) Asegurar la CLI de Velopack (misma versión que el paquete).
if (-not (Get-Command vpk -ErrorAction SilentlyContinue)) {
    dotnet tool install -g vpk --version 1.2.0
    $env:PATH += ";$env:USERPROFILE\.dotnet\tools"
}

# 1) Publicar self-contained.
Write-Host "==> Publicando self-contained ($Runtime)..." -ForegroundColor Cyan
dotnet publish src/Orion.Presentation/Orion.Presentation.csproj `
    -c Release -r $Runtime --self-contained true `
    -p:WindowsAppSDKSelfContained=true `
    -o "publish/$Runtime"

# 2) Empaquetar instalador + feed de actualizaciones.
Write-Host "==> Generando instalador v$Version..." -ForegroundColor Cyan
vpk pack `
    -u Orion `
    -v $Version `
    -p "publish/$Runtime" `
    -e Orion.Presentation.exe `
    -o releases `
    --packTitle "ORION AI" `
    --packAuthors "Grupo Platino"

Write-Host "==> Listo. Instalador en: releases/Orion-win-Setup.exe" -ForegroundColor Green
Write-Host "    Para auto-update, publica el contenido de 'releases/' en el feed configurado" -ForegroundColor Green
Write-Host "    (variable de entorno ORION_UPDATE_FEED o la URL por defecto del composition root)." -ForegroundColor Green

# Firma de código (opcional, recomendado para distribución):
#   Añade a `vpk pack`:  --signParams "/a /f cert.pfx /p PASSWORD /fd sha256 /tr http://timestamp.digicert.com /td sha256"
