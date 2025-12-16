# This File is automatically synchronized from https://github.com/Glatzel/template

. $PSScriptRoot/setup.ps1
if ($env:CI) {
    rustup toolchain install nightly --profile=minimal
    rustup component add rustfmt --toolchain nightly
}
foreach ($file in $args) {
    Set-Location (Split-Path (Resolve-Path $file) -Parent)
    Write-Output "Cargo fmt in: $pwd"
    cargo +nightly fmt --all --quiet
    Set-Location $ROOT
}
