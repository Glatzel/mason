param(
    [switch]$Release,
    [int[]]$Versions = (2018..2025)
)

Set-Location $PSScriptRoot
Set-Location ..

Remove-Item ./bin -Recurse -ErrorAction SilentlyContinue

if ($Release) {
    $Versions | ForEach-Object -Parallel {
        $env:RevitVersion = $_
        $result = dotnet build --configuration $Release 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "$result`n[$env:RevitVersion] ✅ Build succeeded"
        }
        else {
            Write-Host "$result`n[$env:RevitVersion] ❌ Build failed"
        }
    } 
}
else {
    $Versions | ForEach-Object -Parallel {
        $env:RevitVersion = $_
        $result = dotnet build --configuration Debug 2>&1
        $result | Out-String | Write-Host
        if ($LASTEXITCODE -eq 0) {
            Write-Host "$result`n[$env:RevitVersion] ✅ Build succeeded"
        }
        else {
            Write-Host "$result`n[$env:RevitVersion] ❌ Build failed"
        }
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
