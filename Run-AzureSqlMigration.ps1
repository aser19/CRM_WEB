# Azure SQL Migration Script Runner
# Futtasd ezt a scriptet a migrációk végrehajtásához az Azure SQL adatbázison

param(
	[string]$Server = "biztovill.database.windows.net",
	[string]$Database = "CRM",
	[string]$Username = "vagoadam",
	[string]$Password = "Bizto1116"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Azure SQL Migration Script Runner" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# SQL Server PowerShell modul ellenőrzése
if (-not (Get-Module -ListAvailable -Name SqlServer)) {
	Write-Host "SqlServer PowerShell modul nem található!" -ForegroundColor Red
	Write-Host "Telepítsd a következő paranccsal:" -ForegroundColor Yellow
	Write-Host "Install-Module -Name SqlServer -Scope CurrentUser" -ForegroundColor Yellow
	exit
}

Import-Module SqlServer

$migrationScripts = @(
	"Migration_Step1_AddStatusz.sql",
	"Migration_Step2_ConvertData.sql",
	"Migration_Step3_DropElvegezve.sql",
	"Migration_Step4_AddKarbantartasId.sql",
	"Migration_Step5_Verify.sql"
)

$connectionString = "Server=$Server;Database=$Database;User Id=$Username;Password=$Password;Encrypt=True;TrustServerCertificate=False;"

Write-Host "Kapcsolódás az Azure SQL adatbázishoz..." -ForegroundColor Yellow
Write-Host "Server: $Server" -ForegroundColor Gray
Write-Host "Database: $Database" -ForegroundColor Gray
Write-Host ""

foreach ($script in $migrationScripts) {
	if (-not (Test-Path $script)) {
		Write-Host "HIBA: $script nem található!" -ForegroundColor Red
		continue
	}

	Write-Host "Futtatás: $script" -ForegroundColor Cyan
	try {
		$sqlContent = Get-Content $script -Raw
		Invoke-Sqlcmd -ConnectionString $connectionString -Query $sqlContent -Verbose
		Write-Host "✓ Sikeres: $script" -ForegroundColor Green
		Write-Host ""
	}
	catch {
		Write-Host "✗ HIBA: $script" -ForegroundColor Red
		Write-Host $_.Exception.Message -ForegroundColor Red
		Write-Host ""

		$continue = Read-Host "Folytatod a többi scripttel? (i/n)"
		if ($continue -ne "i") {
			Write-Host "Megszakítva." -ForegroundColor Yellow
			exit
		}
	}
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migráció befejezve!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
