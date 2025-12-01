# Script para verificar los banderines en la base de datos
$connectionString = "Server=tcp:dbautoclick.database.windows.net,1433;Initial Catalog=autoclickdb;Persist Security Info=False;User ID=CloudSA76f59e3a;Password=AutoClick2025!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

$query = @"
SELECT TOP 10 
    Id, 
    Marca,
    Modelo,
    Ano,
    BanderinAdquirido, 
    BanderinesAdquiridos,
    PlanVisibilidad,
    Activo
FROM Autos
WHERE (BanderinAdquirido > 0 OR BanderinesAdquiridos IS NOT NULL)
ORDER BY FechaCreacion DESC
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    $reader = $command.ExecuteReader()
    
    Write-Host "=== Autos con Banderines ===" -ForegroundColor Green
    Write-Host ""
    
    while ($reader.Read()) {
        Write-Host "ID: $($reader['Id'])" -ForegroundColor Cyan
        Write-Host "  Auto: $($reader['Marca']) $($reader['Modelo']) $($reader['Ano'])"
        Write-Host "  BanderinAdquirido: $($reader['BanderinAdquirido'])"
        Write-Host "  BanderinesAdquiridos: $($reader['BanderinesAdquiridos'])"
        Write-Host "  PlanVisibilidad: $($reader['PlanVisibilidad'])"
        Write-Host "  Activo: $($reader['Activo'])"
        Write-Host ""
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host "Consulta completada exitosamente" -ForegroundColor Green
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
