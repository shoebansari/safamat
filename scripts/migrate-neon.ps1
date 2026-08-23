# Run EF migrations against Neon (connection in scripts/neon.connection — not committed)
# Or set: $env:NEON_CONNECTION="Host=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"

param(
    [string]$ConnectionString = $env:NEON_CONNECTION
)

if (-not $ConnectionString) {
    $connFile = Join-Path $PSScriptRoot "neon.connection"
    if (Test-Path $connFile) {
        $ConnectionString = (Get-Content $connFile -Raw).Trim()
    }
}

if (-not $ConnectionString) {
    Write-Error "Set NEON_CONNECTION, create scripts/neon.connection, or pass -ConnectionString"
    exit 1
}

$root = Split-Path $PSScriptRoot -Parent
Push-Location "$root\backend\Matrimonial.AdminApi"

Write-Host "Applying migrations to Neon..."
dotnet ef database update --connection $ConnectionString

Pop-Location
Write-Host "Done. Set the same string in Render as ConnectionStrings__DefaultConnection"
