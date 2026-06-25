# Azure App Service .NET 8 Stack beállítása
# Futtasd ezt az Azure Cloud Shell-ben vagy Azure CLI-vel

$resourceGroup = "CRM"
$appName = "BiztvillCRMWeb20260327201621"

# .NET 8 stack beállítása
az webapp config set --resource-group $resourceGroup --name $appName --linux-fx-version "DOTNETCORE|8.0"

# Always On bekapcsolása
az webapp config set --resource-group $resourceGroup --name $appName --always-on true

# HTTP 2.0 engedélyezése
az webapp config set --resource-group $resourceGroup --name $appName --http20-enabled true

Write-Host "Azure App Service konfigurálva .NET 8-ra" -ForegroundColor Green
