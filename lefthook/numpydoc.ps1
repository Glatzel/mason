# This File is automatically synchronized from https://github.com/Glatzel/template

if (-not $args) { exit 0 }
&$PSScriptRoot/setup.ps1
Set-Location ..
numpydoc lint $args --ignore ES01 EX01 GL08 PR04 RT03 SA01 SA04
