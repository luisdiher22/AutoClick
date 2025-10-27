# Script para configurar los contenedores de Azure Storage como públicos
# Esto permite acceso directo a las imágenes sin necesidad de SAS URLs

$storageAccountName = "autoclickstorage"
$resourceGroup = "AutoClickResourceGroup"  # Ajusta si es diferente

Write-Host "Configurando contenedores como públicos..." -ForegroundColor Cyan

# Configurar contenedor de banderines como público
Write-Host "`nConfigurando contenedor 'banderines'..." -ForegroundColor Yellow
az storage container set-permission `
    --name banderines `
    --public-access blob `
    --account-name $storageAccountName `
    --auth-mode login

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Contenedor 'banderines' configurado como público" -ForegroundColor Green
} else {
    Write-Host "✗ Error al configurar contenedor 'banderines'" -ForegroundColor Red
}

# Configurar contenedor de logos como público
Write-Host "`nConfigurando contenedor 'logos'..." -ForegroundColor Yellow
az storage container set-permission `
    --name logos `
    --public-access blob `
    --account-name $storageAccountName `
    --auth-mode login

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Contenedor 'logos' configurado como público" -ForegroundColor Green
} else {
    Write-Host "✗ Error al configurar contenedor 'logos'" -ForegroundColor Red
}

# Verificar la configuración
Write-Host "`n=== Verificando configuración ===" -ForegroundColor Cyan
Write-Host "`nContenedor 'banderines':" -ForegroundColor Yellow
az storage container show `
    --name banderines `
    --account-name $storageAccountName `
    --auth-mode login `
    --query "{name:name, publicAccess:properties.publicAccess}" `
    --output table

Write-Host "`nContenedor 'logos':" -ForegroundColor Yellow
az storage container show `
    --name logos `
    --account-name $storageAccountName `
    --auth-mode login `
    --query "{name:name, publicAccess:properties.publicAccess}" `
    --output table

Write-Host "`n=== URLs de ejemplo ===" -ForegroundColor Cyan
Write-Host "Las imágenes ahora serán accesibles en URLs como:"
Write-Host "https://$storageAccountName.blob.core.windows.net/banderines/[nombre-archivo]" -ForegroundColor Green
Write-Host "https://$storageAccountName.blob.core.windows.net/logos/[nombre-archivo]" -ForegroundColor Green
