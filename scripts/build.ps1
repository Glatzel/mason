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

# Compress only once after all builds
if (Test-Path "./bin/Mason") {
    $zipPath = "./bin/Mason.zip"
    Compress-Archive -Path ./bin/Mason -DestinationPath $zipPath -Force -PassThru
    Write-Output "✅ Archive created at $zipPath"
}
else {
    Write-Warning "⚠️ Mason build output not found."
}
