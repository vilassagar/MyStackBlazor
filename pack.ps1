# MyStackBlazor — build Tailwind CSS then pack the NuGet package
# Usage:
#   .\pack.ps1                  # build + pack (default version from csproj)
#   .\pack.ps1 -Version 1.2.0  # override version

param(
    [string]$Version = "",
    [switch]$SkipTailwind
)

$ErrorActionPreference = "Stop"
$lib = "src\MyStackBlazor"

# ── 1. Tailwind CSS ──────────────────────────────────────────────────────────
if (-not $SkipTailwind) {
    Write-Host "Building Tailwind CSS..." -ForegroundColor Cyan
    Push-Location $lib
    npx @tailwindcss/cli -i ./tailwind/input.css -o ./wwwroot/css/mystackblazor.css --minify
    if ($LASTEXITCODE -ne 0) { Write-Error "Tailwind build failed." }
    Pop-Location
}

# ── 2. dotnet build ──────────────────────────────────────────────────────────
Write-Host "Building library..." -ForegroundColor Cyan
$buildArgs = @("build", $lib, "-c", "Release", "--nologo")
if ($Version) { $buildArgs += "/p:Version=$Version" }
dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { Write-Error "Build failed." }

# ── 3. dotnet pack ───────────────────────────────────────────────────────────
Write-Host "Packing NuGet..." -ForegroundColor Cyan
$packArgs = @("pack", $lib, "-c", "Release", "--no-build", "--nologo")
if ($Version) { $packArgs += "/p:Version=$Version" }
dotnet @packArgs
if ($LASTEXITCODE -ne 0) { Write-Error "Pack failed." }

$nupkgVer = if ($Version) { $Version } else {
    (Select-Xml -Path "$lib\MyStackBlazor.csproj" -XPath "//Version").Node.InnerText
}
$nupkg = "nupkg\MyStackBlazor.$nupkgVer.nupkg"

Write-Host ""
Write-Host "Package ready: $nupkg" -ForegroundColor Green
Write-Host ""
Write-Host "To publish to NuGet.org:" -ForegroundColor Yellow
Write-Host "  dotnet nuget push $nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Yellow
