# Script para cambiar logos de AutoClick.cr
# Uso: .\cambiar-logo.ps1 [numero]
# Ejemplo: .\cambiar-logo.ps1 3

param(
    [Parameter(Mandatory=$true)]
    [ValidateRange(1, 6)]
    [int]$NumeroLogo
)

# Configuración
$LogosPath = ".\wwwroot\images\logos"
$SourcePath = "$LogosPath\LOGOS AUTOCLICK\LOGOS AUTOCLICK-0$NumeroLogo.png"
$DestinationPath = "$LogosPath\autoclick-logo-hd.png"

# Verificar que existe el archivo fuente
if (-not (Test-Path $SourcePath)) {
    Write-Error "❌ Error: No se encontró el archivo $SourcePath"
    exit 1
}

try {
    # Crear backup del logo actual
    if (Test-Path $DestinationPath) {
        $BackupPath = "$LogosPath\autoclick-logo-hd-backup.png"
        Copy-Item $DestinationPath $BackupPath -Force
        Write-Host "📋 Backup creado: autoclick-logo-hd-backup.png" -ForegroundColor Yellow
    }
    
    # Copiar el nuevo logo
    Copy-Item $SourcePath $DestinationPath -Force
    Write-Host "✅ Logo cambiado exitosamente!" -ForegroundColor Green
    Write-Host "🎨 Ahora usando: LOGOS AUTOCLICK-0$NumeroLogo.png" -ForegroundColor Cyan
    Write-Host "🔄 Reinicia la aplicación para ver los cambios" -ForegroundColor Blue
    
} catch {
    Write-Error "❌ Error al cambiar el logo: $($_.Exception.Message)"
    exit 1
}

# Mostrar información del logo seleccionado
Write-Host "`n📊 Información del logo:" -ForegroundColor Magenta
Write-Host "   Archivo: LOGOS AUTOCLICK-0$NumeroLogo.png"
Write-Host "   Ruta: $SourcePath"
Write-Host "   Destino: $DestinationPath"

# Opcional: Mostrar todos los logos disponibles
Write-Host "`n📁 Logos disponibles:" -ForegroundColor Magenta
Get-ChildItem "$LogosPath\LOGOS AUTOCLICK\*.png" | ForEach-Object {
    $Number = $_.Name -replace "LOGOS AUTOCLICK-0", "" -replace ".png", ""
    $Status = if ($Number -eq $NumeroLogo) { "← ACTIVO" } else { "" }
    Write-Host "   $Number. $($_.Name) $Status" -ForegroundColor $(if ($Number -eq $NumeroLogo) { "Green" } else { "White" })
}