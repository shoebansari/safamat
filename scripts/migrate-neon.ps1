# Run EF migrations against Neon (set connection string first)
# Example:
#   $env:NEON_CONNECTION="Host=xxx.neon.tech;Database=neondb;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
#   .\scripts\migrate-neon.ps1

param(
    [string]$ConnectionString = $env:NEON_CONNECTION
)

if (-not $ConnectionString) {
    Write-Error "Set NEON_CONNECTION env var or pass -ConnectionString"
    exit 1
}

$root = Split-Path $PSScriptRoot -Parent
Push-Location "$root\backend\Matrimonial.AdminApi"

Write-Host "Applying migrations to Neon..."
dotnet ef database update --connection $ConnectionString

Pop-Location
Write-Host "Done. Restart Render service after this."
