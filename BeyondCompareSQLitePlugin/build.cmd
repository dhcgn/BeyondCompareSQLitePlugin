dotnet publish -r win10-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true