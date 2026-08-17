# Push Matrimonial project to GitHub: https://github.com/shoebansari/safamat

$ErrorActionPreference = "Stop"
Set-Location (Split-Path $PSScriptRoot -Parent)

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "Git is not installed."
}

$remote = "https://github.com/shoebansari/safamat.git"

git remote remove origin 2>$null
git remote add origin $remote
git branch -M main
git push -u origin main

Write-Host ""
Write-Host "Done. Repo: $remote" -ForegroundColor Green
