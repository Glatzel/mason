# This File is automatically synchronized from https://github.com/Glatzel/template

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$ROOT = git rev-parse --show-toplevel
Set-Location $ROOT
foreach ($f in Get-ChildItem "Cargo.lock" -Recurse) {
    # skip target folder
    if ($f -contains "target") { continue }

    Set-Location $f.Directory.ToString()
    Write-Output "Cargo machete in: $pwd"
    cargo machete
}
Set-Location $ROOT
