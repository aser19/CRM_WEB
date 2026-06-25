# Publish és Azure Zip Deploy
# Ez a script publikálja az alkalmazást és feltölti Azure-ba

$publishFolder = "bin\Release\net8.0\publish"
$zipFile = "publish.zip"
$appName = "BiztvillCRMWeb20260327201621"
$resourceGroup = "CRM"

Write-Host "1. Projekt publikálása..." -ForegroundColor Cyan
dotnet publish BiztvillCRM.Web\BiztvillCRM.Web.csproj -c Release -o $publishFolder

if ($LASTEXITCODE -ne 0) {
	Write-Host "Publikálás sikertelen!" -ForegroundColor Red
	exit 1
}

Write-Host "2. ZIP archívum készítése..." -ForegroundColor Cyan
Compress-Archive -Path "$publishFolder\*" -DestinationPath $zipFile -Force

Write-Host "3. Feltöltés Azure-ba..." -ForegroundColor Cyan
az webapp deployment source config-zip --resource-group $resourceGroup --name $appName --src $zipFile

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Sikeres publikálás!" -ForegroundColor Green
	Write-Host "URL: https://$appName.azurewebsites.net" -ForegroundColor Yellow

	# Tisztítás
	Remove-Item $zipFile -Force
} else {
	Write-Host "✗ Publikálás sikertelen!" -ForegroundColor Red
}
