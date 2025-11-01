# Monitor Active Database Connections
# This script connects to Azure SQL Database and shows active sessions
# Run this to see what's keeping your database awake

param(
    [string]$ServerName = "dbautoclick.database.windows.net",
    [string]$DatabaseName = "autoclickdb"
)

Write-Host "=" -ForegroundColor Cyan
Write-Host "🔍 Azure SQL Database Connection Monitor" -ForegroundColor Cyan
Write-Host "Checking active connections for: $DatabaseName" -ForegroundColor Cyan
Write-Host "=" -ForegroundColor Cyan
Write-Host ""

# SQL query to check active sessions
$query = @"
-- Show all active user sessions
SELECT 
    s.session_id,
    s.login_name,
    s.host_name,
    s.program_name,
    s.login_time,
    s.last_request_start_time,
    s.last_request_end_time,
    DATEDIFF(MINUTE, s.last_request_end_time, GETDATE()) as minutes_since_last_request,
    s.status,
    s.cpu_time,
    s.memory_usage,
    s.total_scheduled_time,
    s.total_elapsed_time,
    c.net_transport,
    c.protocol_type,
    c.client_net_address,
    c.local_net_address,
    c.connection_id,
    c.num_reads,
    c.num_writes
FROM sys.dm_exec_sessions s
LEFT JOIN sys.dm_exec_connections c ON s.session_id = c.session_id
WHERE s.is_user_process = 1
ORDER BY s.last_request_start_time DESC;

-- Connection pool statistics
SELECT 
    COUNT(*) as total_user_connections,
    SUM(CASE WHEN status = 'sleeping' THEN 1 ELSE 0 END) as sleeping_connections,
    SUM(CASE WHEN status = 'running' THEN 1 ELSE 0 END) as active_connections
FROM sys.dm_exec_sessions 
WHERE is_user_process = 1;

-- Database status
SELECT 
    name as database_name,
    state_desc as database_state,
    create_date,
    compatibility_level
FROM sys.databases 
WHERE name = DB_NAME();
"@

try {
    Write-Host "Connecting to database using Azure AD authentication..." -ForegroundColor Yellow
    Write-Host ""
    
    # Using Azure AD authentication (integrated security)
    $connectionString = "Server=$ServerName;Database=$DatabaseName;Authentication=Active Directory Interactive;Encrypt=True;TrustServerCertificate=False;"
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "✅ Connected successfully!" -ForegroundColor Green
    Write-Host ""
    
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $command.CommandTimeout = 30
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null
    
    # Display results
    if ($dataset.Tables.Count -gt 0) {
        Write-Host "📊 Active User Sessions:" -ForegroundColor Green
        Write-Host "-" -ForegroundColor Gray
        if ($dataset.Tables[0].Rows.Count -eq 0) {
            Write-Host "  No active user sessions found. Database may be idle." -ForegroundColor Yellow
        } else {
            $dataset.Tables[0] | Format-Table -AutoSize -Property session_id, login_name, program_name, status, minutes_since_last_request, host_name, client_net_address | Out-String | Write-Host
        }
        Write-Host ""
        
        if ($dataset.Tables.Count -gt 1) {
            Write-Host "📈 Connection Summary:" -ForegroundColor Green
            Write-Host "-" -ForegroundColor Gray
            $dataset.Tables[1] | Format-Table -AutoSize | Out-String | Write-Host
            Write-Host ""
        }
        
        if ($dataset.Tables.Count -gt 2) {
            Write-Host "💾 Database Information:" -ForegroundColor Green
            Write-Host "-" -ForegroundColor Gray
            $dataset.Tables[2] | Format-Table -AutoSize | Out-String | Write-Host
        }
    }
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "=" -ForegroundColor Cyan
    Write-Host "💡 Tips to Reduce vCore Usage:" -ForegroundColor Yellow
    Write-Host "  • Look for sleeping connections with high 'minutes_since_last_request'" -ForegroundColor White
    Write-Host "  • Identify applications in 'program_name' that keep connections open" -ForegroundColor White
    Write-Host "  • Check if Application Insights or monitoring tools are connected" -ForegroundColor White
    Write-Host "  • Ensure your app properly disposes DbContext after each request" -ForegroundColor White
    Write-Host "=" -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Error connecting to database:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Make sure you have:" -ForegroundColor Yellow
    Write-Host "  1. Azure CLI installed and authenticated (az login)" -ForegroundColor White
    Write-Host "  2. Access permissions to the database" -ForegroundColor White
    Write-Host "  3. Your IP allowed in the SQL Server firewall" -ForegroundColor White
    exit 1
}
