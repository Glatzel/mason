Set-Location $PSScriptRoot
Remove-Item C:/ProgramData/Autodesk/Revit/Addins/*/mason.addin -ErrorAction SilentlyContinue
$versions = Get-ChildItem ./revit* -Name

foreach ($v in $versions) {
    $v=$v.replace('revit','')
    New-Item "C:/ProgramData/Autodesk/Revit/Addins/$v" -ItemType Directory -ErrorAction SilentlyContinue
    $addindata = [xml](Get-Content "./mason.addin")
    $addindata.RevitAddIns.AddIn.Assembly = "$PWD\revit$v\mason.dll"
    $addindata.Save("C:/ProgramData/Autodesk/Revit/Addins/$v/mason.addin")
}
Write-Output $version
