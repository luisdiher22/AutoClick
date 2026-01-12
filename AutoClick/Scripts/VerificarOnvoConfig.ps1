# ========================================================================
# Verificar configuracion actual de ONVO Pay en Azure
# ========================================================================

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host "VERIFICANDO CONFIGURACION DE ONVO PAY EN AZURE" -ForegroundColor Cyan
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Consultando Application Settings..." -ForegroundColor Yellow
Write-Host ""

# Obtener todas las settings
$settings = az webapp config appsettings list `
    --name AutoClick `
    --resource-group AutoClick `
    --output json | ConvertFrom-Json

# Filtrar las settings de OnvoPay
$onvoSettings = $settings | Where-Object { $_.name -like "OnvoPay__*" }

if ($onvoSettings.Count -eq 0) {
    Write-Host "ADVERTENCIA: No hay configuracion de OnvoPay en Azure" -ForegroundColor Red
    Write-Host "La aplicacion usara los valores de appsettings.json (probablemente vacios)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Para configurar keys de prueba, ejecuta: .\ConfigurarOnvoTest.ps1" -ForegroundColor Cyan
    Write-Host "Para configurar keys live, ejecuta: .\ConfigurarOnvoLive.ps1" -ForegroundColor Cyan
} else {
    Write-Host "CONFIGURACION ACTUAL:" -ForegroundColor Green
    Write-Host "---------------------------------------------------------------------" -ForegroundColor Gray
    Write-Host ""
    
    $secretKey = ($onvoSettings | Where-Object { $_.name -eq "OnvoPay__SecretKey" }).value
    $publishableKey = ($onvoSettings | Where-Object { $_.name -eq "OnvoPay__PublishableKey" }).value
    $webhookSecret = ($onvoSettings | Where-Object { $_.name -eq "OnvoPay__WebhookSecret" }).value
    
    # Detectar si son keys de test o live
    $esTest = $false
    $esLive = $false
    
    if ($secretKey -like "*test*") {
        $esTest = $true
        $tipoKey = "[TEST]"
        $color = "Yellow"
    } elseif ($secretKey -like "*live*") {
        $esLive = $true
        $tipoKey = "[LIVE - PRODUCCION]"
        $color = "Green"
    } else {
        $tipoKey = "[DESCONOCIDO]"
        $color = "Gray"
    }
    
    Write-Host "Tipo de Keys: $tipoKey" -ForegroundColor $color
    Write-Host ""
    
    if ($secretKey) {
        $preview = $secretKey.Substring(0, [Math]::Min(40, $secretKey.Length)) + "..."
        Write-Host "Secret Key: $preview" -ForegroundColor White
    } else {
        Write-Host "Secret Key: [NO CONFIGURADA]" -ForegroundColor Red
    }
    
    if ($publishableKey) {
        $preview = $publishableKey.Substring(0, [Math]::Min(40, $publishableKey.Length)) + "..."
        Write-Host "Publishable Key: $preview" -ForegroundColor White
    } else {
        Write-Host "Publishable Key: [NO CONFIGURADA]" -ForegroundColor Red
    }
    
    if ($webhookSecret) {
        Write-Host "Webhook Secret: [CONFIGURADO] $webhookSecret" -ForegroundColor White
    } else {
        Write-Host "Webhook Secret: [NO CONFIGURADO]" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "---------------------------------------------------------------------" -ForegroundColor Gray
    Write-Host ""
    
    if ($esTest) {
        Write-Host "MODO: PRUEBAS (TEST)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Los pagos son simulados. Usa tarjetas de prueba:" -ForegroundColor White
        Write-Host "  - 4242424242424242 (aprobada)" -ForegroundColor Gray
        Write-Host "  - 4000000000000002 (declinada)" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Para cambiar a LIVE: .\ConfigurarOnvoLive.ps1" -ForegroundColor Cyan
    } elseif ($esLive) {
        Write-Host "MODO: PRODUCCION (LIVE)" -ForegroundColor Green
        Write-Host ""
        Write-Host "Los pagos son REALES. Se procesaran transacciones verdaderas." -ForegroundColor White
        Write-Host ""
        Write-Host "Para cambiar a TEST: .\ConfigurarOnvoTest.ps1" -ForegroundColor Cyan
    }
}

Write-Host ""
Write-Host "URL del Webhook:" -ForegroundColor Yellow
Write-Host "https://autoclick.cr/api/onvowebhook" -ForegroundColor Gray
Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Cyan
Write-Host ""
