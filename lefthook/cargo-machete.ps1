# This File is automatically synchronized from https://github.com/Glatzel/template

if (-not $args) { exit 0 }
. $PSScriptRoot/setup.ps1
foreach ($file in $args) {
    Set-Location (Split-Path (Resolve-Path $file) -Parent)
    Write-Output "Cargo machete in: $pwd"
    cargo machete
}
