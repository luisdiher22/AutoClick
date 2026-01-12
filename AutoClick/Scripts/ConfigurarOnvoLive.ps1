# ========================================================================
# Configurar ONVO Pay con keys de PRODUCCION (LIVE) en Azure
# ========================================================================
# IMPORTANTE: Esto configurará keys REALES de producción.
# Los pagos serán reales y se procesarán transacciones verdaderas.
# ========================================================================

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host "CONFIGURANDO ONVO PAY - KEYS DE PRODUCCION (LIVE)" -ForegroundColor Yellow
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "IMPORTANTE: Esto configurara KEYS REALES de produccion." -ForegroundColor Green
Write-Host "Los pagos seran reales y se procesaran transacciones verdaderas." -ForegroundColor Green
Write-Host ""

# Solicitar las keys LIVE al usuario
Write-Host "Ingresa las keys LIVE de ONVO Pay:" -ForegroundColor Yellow
Write-Host ""

$secretKey = Read-Host "Secret Key (onvo_live_secret_key_...)"
$publishableKey = Read-Host "Publishable Key (onvo_live_publishable_key_...)"
$webhookSecret = Read-Host "Webhook Secret (de produccion)"

Write-Host ""

# Validar que no estén vacías
if ([string]::IsNullOrWhiteSpace($secretKey) -or [string]::IsNullOrWhiteSpace($publishableKey)) {
    Write-Host "ERROR: Las keys no pueden estar vacias" -ForegroundColor Red
    exit 1
}

# Validar que sean keys LIVE (no TEST)
if ($secretKey -like "*test*" -or $publishableKey -like "*test*") {
    Write-Host ""
    Write-Host "ADVERTENCIA: Detectamos 'test' en las keys." -ForegroundColor Red
    Write-Host "Estas seguro de que son keys LIVE de produccion?" -ForegroundColor Red
    Write-Host ""
    $continuar = Read-Host "Continuar de todos modos? (escribe 'SI' para continuar)"
    
    if ($continuar -ne "SI") {
        Write-Host "Operacion cancelada." -ForegroundColor Yellow
        exit
    }
}

Write-Host ""
Write-Host "Configurando Application Settings en Azure..." -ForegroundColor Yellow
Write-Host ""

# Configurar Secret Key
Write-Host "[1/3] Configurando OnvoPay__SecretKey..." -ForegroundColor Gray
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__SecretKey="$secretKey" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "    [OK] Secret Key configurada" -ForegroundColor Green
} else {
    Write-Host "    [ERROR] Fallo al configurar Secret Key" -ForegroundColor Red
    exit 1
}

# Configurar Publishable Key
Write-Host "[2/3] Configurando OnvoPay__PublishableKey..." -ForegroundColor Gray
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__PublishableKey="$publishableKey" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "    [OK] Publishable Key configurada" -ForegroundColor Green
} else {
    Write-Host "    [ERROR] Fallo al configurar Publishable Key" -ForegroundColor Red
    exit 1
}

# Configurar Webhook Secret (opcional)
if (-not [string]::IsNullOrWhiteSpace($webhookSecret)) {
    Write-Host "[3/3] Configurando OnvoPay__WebhookSecret..." -ForegroundColor Gray
    az webapp config appsettings set `
        --name AutoClick `
        --resource-group AutoClick `
        --settings OnvoPay__WebhookSecret="$webhookSecret" `
        --output none

    if ($LASTEXITCODE -eq 0) {
        Write-Host "    [OK] Webhook Secret configurado" -ForegroundColor Green
    } else {
        Write-Host "    [ERROR] Fallo al configurar Webhook Secret" -ForegroundColor Red
    }
} else {
    Write-Host "[3/3] Webhook Secret no proporcionado (se mantendra el anterior)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host "CONFIGURACION COMPLETADA" -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Las keys de PRODUCCION (LIVE) estan activas." -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANTE:" -ForegroundColor Yellow
Write-Host "  1. La app se reiniciara automaticamente (tarda ~30 segundos)" -ForegroundColor White
Write-Host "  2. Los pagos seran REALES" -ForegroundColor White
Write-Host "  3. Actualiza el webhook en ONVO Dashboard si es necesario:" -ForegroundColor White
Write-Host "     URL: https://autoclick.cr/api/onvowebhook" -ForegroundColor Gray
Write-Host "     Secret: (el que configuraste)" -ForegroundColor Gray
Write-Host ""
