$ROOT = git rev-parse --show-toplevel
Set-Location $PSScriptRoot
Set-Location ..
dotnet test -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
Set-Location $ROOT
