Set-Location $PSScriptRoot
Set-Location ..
$svg=GET-ChildItem ./src/mason/Icon/*.svg
foreach($s in $svg)
{
    magick -background none -size 256x256 $s "$s".Replace(".svg",".png")
}
