# This File is automatically synchronized from https://github.com/Glatzel/template

if ($env:CI -and $args) {
    rustup toolchain install nightly --profile=minimal
    rustup component add rustfmt --toolchain nightly
}
$ROOT = git rev-parse --show-toplevel
Set-Location $ROOT
foreach ($file in $args) {
    Set-Location (Split-Path (Resolve-Path $file) -Parent)
    Write-Output "Cargo fmt in: $pwd"
    cargo +nightly fmt --all
}
Set-Location $ROOT
