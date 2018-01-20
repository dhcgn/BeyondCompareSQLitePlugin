$folder = Join-Path $env:APPDATA 'Scooter Software\SQLite Plugin'
if(-not(Test-Path $folder))
{
    New-Item -Path $folder -ItemType directory
}

Get-ChildItem -Path $folder | Remove-Item -Recurse -Force -confirm:$false

$tempFile = "{0}.zip" -f [System.Guid]::NewGuid()

$releasesWebPage = Invoke-WebRequest https://github.com/dhcgn/BeyondCompareSQLitePlugin/releases
$uri = 'https://github.com/'+ ($releasesWebPage.Links | Where-Object{$_.href -like '*download*'} | Select-Object href -First 1 | ForEach-Object{$_.href})
Invoke-WebRequest -Uri $uri -OutFile (Join-Path $folder $tempFile)

Expand-Archive -Path (Join-Path $folder $tempFile) -DestinationPath $folder

Remove-Item -Path (Join-Path $folder $tempFile)

$bcFileFormat = Join-Path $env:APPDATA 'Scooter Software\Beyond Compare 4\BCFileFormats.xml'
if (-not (Test-Path $bcFileFormat)) 
{
    Start-Process -FilePath 'https://github.com/dhcgn/BeyondCompareSQLitePlugin/wiki/Install'
    Exit
}

$pluginIsConfigured = Select-String -Path $bcFileFormat -Pattern (Get-ChildItem -Path $folder -Filter *.exe | ForEach-Object{$_.Name}) -Quiet

if($pluginIsConfigured -eq $false)
{
    Start-Process -FilePath 'https://github.com/dhcgn/BeyondCompareSQLitePlugin/wiki/Install'
}