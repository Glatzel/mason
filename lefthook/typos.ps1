# This File is automatically synchronized from https://github.com/Glatzel/template

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
$ROOT = git rev-parse --show-toplevel
Set-Location $ROOT
if ($args) {
    pixi run --no-progress --manifest-path ./lefthook/pixi.toml typos --force-exclude $args
}
Set-Location $ROOT
