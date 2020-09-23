dotnet publish -c Release
iisreset /stop
xcopy /s /Y .\bin\Release\netcoreapp3.1\publish\*.* c:\Websites\AutoIncidentTracking\
del c:\Websites\AutoIncidentTracking\appSettings.Development.json
iisreset /start