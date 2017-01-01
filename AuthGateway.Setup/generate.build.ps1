#Set-ExecutionPolicy Unrestricted -Scope Process

$hgrev = hg id -i | Out-String;
$hgrev = $hgrev -replace "`t|`n|`r|\+",""
$date = Get-Date -format yyyyMMdd.HHmm

$basename = [System.IO.Path]::GetFileNameWithoutExtension($inputFile)
$extension = [System.IO.Path]::GetExtension($inputFile)
$outdir = Split-Path $outputDir -parent
$outBuildsDir = Join-Path $outdir "Builds"
$outputFile = Join-Path $outBuildsDir ($basename + "." + $date + "."+ $hgrev + "." + $arch + $extension)

Move-Item $inputFile $outputFile
