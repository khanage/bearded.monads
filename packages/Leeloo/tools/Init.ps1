param($installPath, $toolsPath, $package, $project)

Write-Host $installPath
Write-Host $toolsPath
Write-Host $package
Write-Host $project

$packageRoot = $installPath + "\..\..\"
$leelooPath = $packageRoot + "\leeloo"

if(!(Test-Path($leelooPath))) {
	Write-Host "Creating " $leelooPath

	New-Item -ItemType directory -Path $leelooPath

	$cmdDest    = $packageRoot + "build.cmd"      # This belongs in the root

	$fsxDest    = $leelooPath  + "\build.fsx"
	$nuspecDest = $leelooPath  + "\sample.nuspec"
	$nugetDest  = $leelooPath  + "\nuget.exe"

	Write-Host "Copying files " $cmdDest ", " $fsxDest ", and " $nuspecDest "."

	$cmdPath    = $toolsPath + "\build.cmd"
	$fsxPath    = $toolsPath + "\build.fsx"
	$nuspecPath = $toolsPath + "\sample.nuspec.txt"
		
	Copy-Item $cmdPath $cmdDest
	Copy-Item $fsxPath $fsxDest

	Write-Host "Writing nuspec sample"
	Copy-Item $nuspecPath $nuspecDest
	
	Write-Host "Downloading nuget.exe to " $nugetDest
	Invoke-WebRequest "http://nuget.org/nuget.exe" -OutFile $nugetDest
}