# This File is automatically synchronized from https://github.com/Glatzel/template

if (-not $args) { exit 0 }
. $PSScriptRoot/setup.ps1
dotnet tool install -g csharpier; csharpier format .; csharpier check .
