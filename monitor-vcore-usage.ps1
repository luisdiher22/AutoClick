# Monitor vCore Usage for Azure SQL Database
# This script queries Azure Monitor for CPU usage metrics and calculates vCore-seconds consumed

param(
    [string]$ResourceGroup = "AutoClick",
    [string]$ServerName = "dbautoclick",
    [string]$DatabaseName = "autoclickdb",
    [int]$Hours = 24
)

Write-Host "=" -ForegroundColor Cyan
Write-Host "📊 Azure SQL Serverless vCore Usage Monitor" -ForegroundColor Cyan
Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
Write-Host "Time Range: Last $Hours hours" -ForegroundColor Cyan
Write-Host "=" -ForegroundColor Cyan
Write-Host ""

# Get database details
Write-Host "🔍 Fetching database configuration..." -ForegroundColor Yellow

$dbInfo = az sql db show `
    --resource-group $ResourceGroup `
    --server $ServerName `
    --name $DatabaseName `
    --query "{status:status, sku:sku.name, capacity:sku.capacity, tier:sku.tier, minCapacity:minCapacity, autoPauseDelay:autoPauseDelay}" `
    --output json | ConvertFrom-Json

Write-Host "✅ Database Configuration:" -ForegroundColor Green
Write-Host "  Status: $($dbInfo.status)" -ForegroundColor White
Write-Host "  SKU: $($dbInfo.sku)" -ForegroundColor White
Write-Host "  vCores: $($dbInfo.capacity)" -ForegroundColor White
Write-Host "  Min vCores: $($dbInfo.minCapacity)" -ForegroundColor White
Write-Host "  Auto-pause: $($dbInfo.autoPauseDelay) minutes" -ForegroundColor White
Write-Host ""

# Calculate time range
$endTime = Get-Date
$startTime = $endTime.AddHours(-$Hours)
$timespan = "PT${Hours}H"

Write-Host "📈 Querying Azure Monitor for CPU metrics..." -ForegroundColor Yellow

# Query CPU percentage metric
$resourceId = "/subscriptions/(az account show --query id -o tsv)/resourceGroups/$ResourceGroup/providers/Microsoft.Sql/servers/$ServerName/databases/$DatabaseName"

# Get CPU percentage data points
$cpuData = az monitor metrics list `
    --resource $resourceId `
    --metric "cpu_percent" `
    --start-time $startTime.ToString("yyyy-MM-ddTHH:mm:ss") `
    --end-time $endTime.ToString("yyyy-MM-ddTHH:mm:ss") `
    --interval PT1M `
    --aggregation Average `
    --output json 2>$null | ConvertFrom-Json

if ($cpuData -and $cpuData.value -and $cpuData.value[0].timeseries) {
    $dataPoints = $cpuData.value[0].timeseries[0].data
    
    Write-Host "✅ Retrieved $($dataPoints.Count) data points" -ForegroundColor Green
    Write-Host ""
    
    # Calculate vCore-seconds
    $activeMinutes = 0
    $totalCpuPercent = 0
    $maxCpu = 0
    $dataPointsWithActivity = 0
    
    foreach ($point in $dataPoints) {
        if ($point.average -ne $null -and $point.average -gt 0) {
            $activeMinutes++
            $totalCpuPercent += $point.average
            $dataPointsWithActivity++
            if ($point.average -gt $maxCpu) {
                $maxCpu = $point.average
            }
        }
    }
    
    $activeSeconds = $activeMinutes * 60
    $vCoreSeconds = $activeSeconds * $dbInfo.capacity
    $avgCpuPercent = if ($dataPointsWithActivity -gt 0) { $totalCpuPercent / $dataPointsWithActivity } else { 0 }
    
    Write-Host "📊 Usage Statistics (Last $Hours hours):" -ForegroundColor Green
    Write-Host "-" -ForegroundColor Gray
    Write-Host "  Active Minutes: $activeMinutes minutes" -ForegroundColor White
    Write-Host "  Active Time: $([math]::Round($activeMinutes / 60, 2)) hours" -ForegroundColor White
    Write-Host "  vCore-Seconds Used: $vCoreSeconds seconds" -ForegroundColor Yellow
    Write-Host "  Average CPU: $([math]::Round($avgCpuPercent, 2))%" -ForegroundColor White
    Write-Host "  Peak CPU: $([math]::Round($maxCpu, 2))%" -ForegroundColor White
    Write-Host ""
    
    # Monthly projection
    $dailyVCoreSeconds = ($vCoreSeconds / $Hours) * 24
    $monthlyProjection = $dailyVCoreSeconds * 30
    $freeLimit = 100000
    $percentUsed = ($vCoreSeconds / ($freeLimit / 30)) * 100
    
    Write-Host "📅 Monthly Projection:" -ForegroundColor Green
    Write-Host "-" -ForegroundColor Gray
    Write-Host "  Today's Usage: $([math]::Round($dailyVCoreSeconds, 0)) vCore-seconds" -ForegroundColor White
    Write-Host "  Monthly Estimate: $([math]::Round($monthlyProjection, 0)) vCore-seconds" -ForegroundColor Yellow
    Write-Host "  Free Tier Limit: $freeLimit vCore-seconds/month" -ForegroundColor White
    
    if ($monthlyProjection -lt $freeLimit) {
        $remaining = $freeLimit - $monthlyProjection
        Write-Host "  Status: ✅ Within free tier" -ForegroundColor Green
        Write-Host "  Buffer: $([math]::Round($remaining, 0)) vCore-seconds remaining" -ForegroundColor Green
    } else {
        $overage = $monthlyProjection - $freeLimit
        Write-Host "  Status: ⚠️  EXCEEDS free tier!" -ForegroundColor Red
        Write-Host "  Overage: $([math]::Round($overage, 0)) vCore-seconds" -ForegroundColor Red
    }
    Write-Host ""
    
    # Cost estimation (approximate)
    if ($monthlyProjection -gt $freeLimit) {
        $overageSeconds = $monthlyProjection - $freeLimit
        $overageHours = $overageSeconds / 3600
        $costPerVCoreHour = 0.73 # Approximate cost for Gen5 1 vCore in USD
        $estimatedCost = $overageHours * $costPerVCoreHour
        
        Write-Host "💰 Estimated Cost:" -ForegroundColor Yellow
        Write-Host "-" -ForegroundColor Gray
        Write-Host "  Overage Hours: $([math]::Round($overageHours, 2)) hours" -ForegroundColor White
        Write-Host "  Estimated Cost: `$$([math]::Round($estimatedCost, 2)) USD/month" -ForegroundColor Red
        Write-Host ""
    }
    
    Write-Host "=" -ForegroundColor Cyan
    Write-Host "💡 Optimization Tips:" -ForegroundColor Yellow
    Write-Host "  • Auto-pause is set to $($dbInfo.autoPauseDelay) minutes" -ForegroundColor White
    if ($dbInfo.autoPauseDelay -gt 15) {
        Write-Host "    ⚠️  Consider reducing to 10-15 minutes" -ForegroundColor Red
    }
    Write-Host "  • Check connection pooling settings (Max Pool Size)" -ForegroundColor White
    Write-Host "  • Review Application Insights sampling settings" -ForegroundColor White
    Write-Host "  • Run .\monitor-db-connections.ps1 to see active connections" -ForegroundColor White
    Write-Host "=" -ForegroundColor Cyan
    
} else {
    Write-Host "❌ No CPU metrics available for the specified time range" -ForegroundColor Red
    Write-Host "This could mean:" -ForegroundColor Yellow
    Write-Host "  • The database was paused the entire time" -ForegroundColor White
    Write-Host "  • Metrics haven't been collected yet" -ForegroundColor White
    Write-Host "  • There was an error querying Azure Monitor" -ForegroundColor White
}

Write-Host ""
Write-Host "Tip: Run this script daily to track your usage trends" -ForegroundColor Cyan
Write-Host "Example: .\monitor-vcore-usage.ps1 -Hours 24" -ForegroundColor Gray
