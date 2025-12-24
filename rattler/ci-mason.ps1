$ROOT = git rev-parse --show-toplevel
& $ROOT/scripts/build.ps1 -Release
Set-Location $PSScriptRoot
pixi run rattler-build build
Set-Location $ROOT
