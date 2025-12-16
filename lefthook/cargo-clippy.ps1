# This File is automatically synchronized from https://github.com/Glatzel/template

. $PSScriptRoot/setup.ps1
if ($env:CI) {
    rustup toolchain install stable --profile=minimal
    rustup component add clippy --toolchain stable
}
foreach ($file in $args) {
    $dir = (Split-Path (Resolve-Path $file) -Parent)
    Set-Location $dir
    Write-Output "Cargo clippy in: $pwd"
    if (Test-Path ./scripts/setup.ps1) {
        &./scripts/setup.ps1
        Set-Location $dir
    }
    cargo +stable clippy --fix --all-features --quiet
    cargo +stable clippy --all-features --quiet -- -Dwarnings
    Set-Location $ROOT
}
