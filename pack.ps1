# MyStackBlazor — build Tailwind CSS, then pack every NuGet package under src/
# Usage:
#   .\pack.ps1                   # version from version.txt
#   .\pack.ps1 -Version 1.5.0    # override version
#   .\pack.ps1 -SkipTailwind     # use the committed CSS

param(
    [string]$Version = "",
    [switch]$SkipTailwind
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$lib  = Join-Path $root "src\MyStackBlazor"
$out  = Join-Path $root "nupkg"

if (-not $Version) { $Version = (Get-Content (Join-Path $root "version.txt") -Raw).Trim() }

# ── 1. Tailwind CSS (locked version from package-lock.json) ──────────────────
if (-not $SkipTailwind) {
    Write-Host "Building Tailwind CSS..." -ForegroundColor Cyan
    Push-Location $lib
    try {
        if (-not (Test-Path "node_modules")) { npm ci --no-audit --no-fund; if ($LASTEXITCODE -ne 0) { throw "npm ci failed." } }
        npm run build:css
        if ($LASTEXITCODE -ne 0) { throw "Tailwind build failed." }
    } finally { Pop-Location }
}

# ── 2. Build + pack every library (packages + .snupkg symbols) ──────────────
Write-Host "Packing $Version..." -ForegroundColor Cyan
Get-ChildItem (Join-Path $root "src") -Filter *.csproj -Recurse | ForEach-Object {
    dotnet pack $_.FullName -c Release -o $out --nologo /p:Version=$Version /p:SkipTailwind=true
    if ($LASTEXITCODE -ne 0) { throw "Pack failed: $($_.Name)" }
}

Write-Host ""
Write-Host "Packages ready in $out" -ForegroundColor Green
Get-ChildItem $out -Filter "*.$Version.*nupkg" | ForEach-Object { Write-Host "  $($_.Name)" }
Write-Host ""
Write-Host "Releases are published by CI (release-please -> ci.yml, NuGet Trusted Publishing)." -ForegroundColor Yellow
Write-Host "Manual publish, if ever needed:" -ForegroundColor Yellow
Write-Host "  dotnet nuget push `"$out\*.$Version.nupkg`" --api-key <KEY> --source https://api.nuget.org/v3/index.json" -ForegroundColor Yellow
