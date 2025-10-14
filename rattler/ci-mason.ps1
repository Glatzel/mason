$ROOT = git rev-parse --show-toplevel
& $ROOT/scripts/build -Release
Set-Location $PSScriptRoot
pixi run rattler-build build
Set-Location $ROOT
