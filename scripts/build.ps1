Set-Location $PSScriptRoot
Set-Location ..
Remove-Item ./bin -Recurse -ErrorAction SilentlyContinue
foreach ($v in 2018..2025) {
    $env:RevitVersion=$v
    Write-Output "Build for Revit $v"
    dotnet build --configuration Release
}
Compress-Archive -Path ./bin/Mason -DestinationPath ./bin/Mason.zip -PassThru
