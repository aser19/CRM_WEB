# Egyszerű Azure zip deploy
$ErrorActionPreference = "Stop"

Write-Host "🔨 Building Release..." -ForegroundColor Cyan
dotnet publish BiztvillCRM.Web\BiztvillCRM.Web.csproj -c Release -o .\publish

if (-not (Test-Path .\publish)) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "📦 Creating zip..." -ForegroundColor Cyan
if (Test-Path .\app.zip) { Remove-Item .\app.zip -Force }
Compress-Archive -Path .\publish\* -DestinationPath .\app.zip -Force

Write-Host "☁️ Deploying to Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip `
  --resource-group BiztvillCRMWeb20260327201621ResourceGroup `
  --name BiztvillCRMWeb20260327201621 `
  --src .\app.zip

Write-Host "✅ Deploy complete! Visit: https://biztvillcrmweb20260327201621-fqduhxh2e8b0fbdf.westeurope-01.azurewebsites.net" -ForegroundColor Green

# Cleanup
Remove-Item .\app.zip -Force
Remove-Item .\publish -Recurse -Force
