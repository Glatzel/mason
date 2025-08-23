# This File is automatically synchronized from https://github.com/Glatzel/template

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
$ROOT = git rev-parse --show-toplevel
Set-Location $ROOT
foreach ($file in $args) {
    $dir = (Split-Path (Resolve-Path $file) -Parent)
    Set-Location $dir
    Write-Output "Cargo clippy in: $pwd"
    if (Test-Path ./scripts/setup.ps1) {
        &./scripts/setup.ps1
        Set-Location $dir
    }
    cargo +stable clippy --fix --all-features
    cargo +stable clippy --all-features -- -Dwarnings
    Set-Location $ROOT
}
