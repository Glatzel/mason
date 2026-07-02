$ROOT = git rev-parse --show-toplevel
Set-Location $PSScriptRoot
Set-Location ..
dotnet test --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --report-junit --report-junit-filename test.junit.xml
Set-Location $ROOT
