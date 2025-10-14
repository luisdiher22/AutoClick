# PowerShell script to convert all .MOV tag files to .MP4
# Run this after installing FFmpeg

$sourceDir = "C:\Users\luisd\Documents\AutoClick\AutoClick\wwwroot\images\Banderines"
$outputDir = "C:\Users\luisd\Documents\AutoClick\AutoClick\wwwroot\images\Banderines\mp4"

# Create output directory if it doesn't exist
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force
    Write-Host "Created output directory: $outputDir" -ForegroundColor Green
}

# Get all .mov files
$movFiles = Get-ChildItem -Path $sourceDir -Filter "*.mov"

Write-Host "Found $($movFiles.Count) .MOV files to convert" -ForegroundColor Cyan
Write-Host ""

$converted = 0
$failed = 0

foreach ($file in $movFiles) {
    $inputPath = $file.FullName
    $outputFileName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name) + ".mp4"
    $outputPath = Join-Path $outputDir $outputFileName

    Write-Host "Converting: $($file.Name) -> $outputFileName" -ForegroundColor Yellow

    try {
        # FFmpeg conversion with web-optimized settings
        # -c:v libx264 = H.264 video codec (universal browser support)
        # -preset fast = Encoding speed/quality balance
        # -crf 23 = Quality (lower = better quality, 23 is good default)
        # -pix_fmt yuv420p = Pixel format for maximum compatibility
        # -movflags +faststart = Optimize for web streaming
        # -c:a aac = AAC audio codec
        # -b:a 128k = Audio bitrate

        $ffmpegArgs = @(
            "-i", $inputPath,
            "-c:v", "libx264",
            "-preset", "fast",
            "-crf", "23",
            "-pix_fmt", "yuv420p",
            "-movflags", "+faststart",
            "-c:a", "aac",
            "-b:a", "128k",
            "-y",  # Overwrite output files
            $outputPath
        )

        $process = Start-Process -FilePath "ffmpeg" -ArgumentList $ffmpegArgs -NoNewWindow -Wait -PassThru

        if ($process.ExitCode -eq 0) {
            Write-Host "  ✓ Success: $outputFileName" -ForegroundColor Green
            $converted++
        } else {
            Write-Host "  ✗ Failed: $($file.Name) (Exit code: $($process.ExitCode))" -ForegroundColor Red
            $failed++
        }
    } catch {
        Write-Host "  ✗ Error: $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }

    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Conversion Summary:" -ForegroundColor Cyan
Write-Host "  Total files: $($movFiles.Count)" -ForegroundColor White
Write-Host "  Converted: $converted" -ForegroundColor Green
Write-Host "  Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Output directory: $outputDir" -ForegroundColor Yellow

# Option to move converted files to original directory
Write-Host ""
$response = Read-Host "Do you want to move the .mp4 files to the original directory? (Y/N)"
if ($response -eq "Y" -or $response -eq "y") {
    Get-ChildItem -Path $outputDir -Filter "*.mp4" | ForEach-Object {
        $destPath = Join-Path $sourceDir $_.Name
        Move-Item -Path $_.FullName -Destination $destPath -Force
        Write-Host "Moved: $($_.Name)" -ForegroundColor Green
    }
    Write-Host ""
    Write-Host "All .mp4 files moved to: $sourceDir" -ForegroundColor Green

    # Remove empty output directory
    Remove-Item -Path $outputDir -Force
}

Write-Host ""
Write-Host "✓ Conversion complete!" -ForegroundColor Green
Write-Host "Next step: Update AnunciarMiAuto.cshtml to use .mp4 extensions instead of .mov" -ForegroundColor Yellow
