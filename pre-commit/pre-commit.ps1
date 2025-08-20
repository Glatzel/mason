# This File is automatically synchronized from https://github.com/Glatzel/template
param ([switch]$update,[switch] $v)
Set-Location $PSScriptRoot/..
if ($update) {
    pixi run --manifest-path ./pre-commit/pixi.toml pre-commit autoupdate
}
if ($v) {
    pixi run --manifest-path ./pre-commit/pixi.toml pre-commit-run --color=always --show-diff-on-failure -v
}
else {
    pixi run --manifest-path ./pre-commit/pixi.toml pre-commit-run --color=always --show-diff-on-failure
}
