param(
    [switch]$Release,
    [int[]]$Versions = (2018..2025)
)

Set-Location $PSScriptRoot
Set-Location ..

Remove-Item ./bin -Recurse -ErrorAction SilentlyContinue
foreach ($v in $Versions) {
    $env:RevitVersion = $v
    if ($Release) {
        dotnet build --configuration Debug
    }
    else {
        dotnet build --configuration Release
    }
}
