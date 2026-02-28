# download-tailwind.ps1
# Downloads the Tailwind CSS Standalone CLI v3 for Windows x64.
# Run once from the solution root before building.

$version = "v3.4.17"
$url = "https://github.com/tailwindlabs/tailwindcss/releases/download/$version/tailwindcss-windows-x64.exe"
$out = Join-Path $PSScriptRoot "tailwindcss.exe"

if (Test-Path $out) {
    Write-Host "tailwindcss.exe already exists at $out - skipping download." -ForegroundColor Yellow
    exit 0
}

Write-Host "Downloading Tailwind CSS Standalone CLI $version ..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $url -OutFile $out
Write-Host "Saved to: $out" -ForegroundColor Green
