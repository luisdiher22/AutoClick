# ========================================================================
# Configurar ONVO Pay con keys de PRUEBA en Azure (Producción)
# ========================================================================
# ADVERTENCIA: Esto configurará keys de TEST en producción.
# Los pagos serán simulados y no se procesarán transacciones reales.
# ========================================================================

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host "CONFIGURANDO ONVO PAY - KEYS DE PRUEBA EN PRODUCCION" -ForegroundColor Yellow
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "ADVERTENCIA: Esto configurara KEYS DE TEST en produccion." -ForegroundColor Red
Write-Host "Los pagos seran simulados hasta que cambies a keys LIVE." -ForegroundColor Red
Write-Host ""

$continuar = Read-Host "Estas seguro? (escribe 'SI' para continuar)"

if ($continuar -ne "SI") {
    Write-Host ""
    Write-Host "Operacion cancelada." -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "Configurando Application Settings en Azure..." -ForegroundColor Yellow
Write-Host ""

# Configurar Secret Key
Write-Host "[1/3] Configurando OnvoPay__SecretKey..." -ForegroundColor Gray
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__SecretKey="onvo_test_secret_key_HDeP2VKZsEn5B9vx8iZExxjXNiFL_TM6SO9kdKc_IqlU2CsuKn0BD_DiCgQTsWvfuM47OyeP4KuFvt9ml1eY-g" `
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
    --settings OnvoPay__PublishableKey="onvo_test_publishable_key_h8X3OFE1Zl8bNZuQeSDffeSj49ECa6UJIWBJ4Exc681b1jXBIPaz9cSqsFW0olnKls0lrFgGBy8HKzSv_sBRwA" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "    [OK] Publishable Key configurada" -ForegroundColor Green
} else {
    Write-Host "    [ERROR] Fallo al configurar Publishable Key" -ForegroundColor Red
    exit 1
}

# Configurar Webhook Secret
Write-Host "[3/3] Configurando OnvoPay__WebhookSecret..." -ForegroundColor Gray
az webapp config appsettings set `
    --name AutoClick `
    --resource-group AutoClick `
    --settings OnvoPay__WebhookSecret="webhook_secret_oWeUDYVMMgP2e-sl" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "    [OK] Webhook Secret configurado" -ForegroundColor Green
} else {
    Write-Host "    [ERROR] Fallo al configurar Webhook Secret" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host "CONFIGURACION COMPLETADA" -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Las keys de PRUEBA estan activas en produccion." -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANTE:" -ForegroundColor Yellow
Write-Host "  1. La app se reiniciara automaticamente (tarda ~30 segundos)" -ForegroundColor White
Write-Host "  2. Usa tarjetas de prueba para pagos:" -ForegroundColor White
Write-Host "     - 4242424242424242 (aprobada)" -ForegroundColor Gray
Write-Host "     - 4000000000000002 (declinada)" -ForegroundColor Gray
Write-Host "  3. Configura el webhook en ONVO Dashboard:" -ForegroundColor White
Write-Host "     URL: https://autoclick.cr/api/onvowebhook" -ForegroundColor Gray
Write-Host "     Secret: webhook_secret_oWeUDYVMMgP2e-sl" -ForegroundColor Gray
Write-Host ""
Write-Host "Para volver a keys LIVE, ejecuta: .\ConfigurarOnvoLive.ps1" -ForegroundColor Cyan
Write-Host ""
