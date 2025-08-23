# This File is automatically synchronized from https://github.com/Glatzel/template

if (-not $args) { exit 0 }
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
$ROOT = git rev-parse --show-toplevel
Set-Location $ROOT
dotnet tool install -g csharpier; csharpier format .; csharpier check .
