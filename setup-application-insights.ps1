# Script para crear Application Insights en Azure
# Ejecuta este script desde PowerShell

# Variables - Modifica estos valores según tu configuración
$resourceGroupName = "tu-resource-group"  # El resource group donde está tu base de datos
$location = "eastus"  # La misma región que tu base de datos
$workspaceName = "autoclick-workspace"
$appInsightsName = "autoclick-insights"

Write-Host "🚀 Creando Application Insights para AutoClick..." -ForegroundColor Cyan

# Paso 1: Crear Log Analytics Workspace
Write-Host "`n📊 Paso 1/3: Creando Log Analytics Workspace..." -ForegroundColor Yellow
az monitor log-analytics workspace create `
    --resource-group $resourceGroupName `
    --workspace-name $workspaceName `
    --location $location

# Paso 2: Crear Application Insights
Write-Host "`n📈 Paso 2/3: Creando Application Insights..." -ForegroundColor Yellow
az monitor app-insights component create `
    --app $appInsightsName `
    --location $location `
    --resource-group $resourceGroupName `
    --workspace $workspaceName `
    --application-type web

# Paso 3: Obtener la Connection String
Write-Host "`n🔑 Paso 3/3: Obteniendo Connection String..." -ForegroundColor Yellow
$connectionString = az monitor app-insights component show `
    --app $appInsightsName `
    --resource-group $resourceGroupName `
    --query connectionString `
    --output tsv

Write-Host "`n✅ Application Insights creado exitosamente!" -ForegroundColor Green
Write-Host "`n📋 GUARDA ESTA CONNECTION STRING:" -ForegroundColor Cyan
Write-Host $connectionString -ForegroundColor White
Write-Host "`n⚠️  Copia esta connection string y agrégala a appsettings.json" -ForegroundColor Yellow
