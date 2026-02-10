$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$logDir = "$projectRoot\Logs"
$stateFile = "$logDir\agent_state.json"
$errorFile = "$logDir\agent_errors.json"

Write-Host "`n[UNITY BRIDGE STATUS CHCK]`n" -ForegroundColor Cyan

# 1. Check Editor State
if (Test-Path $stateFile) {
    try {
        $stateJson = Get-Content $stateFile -Raw | ConvertFrom-Json
        $status = "IDLE"
        if ($stateJson.isPlaying) { $status = "PLAYING" }
        if ($stateJson.isCompiling) { $status = "COMPILING" }
        
        Write-Host "EDITOR STATE: $status | Scene: $($stateJson.activeScene) | Platform: $($stateJson.platform)" -ForegroundColor Green
        Write-Host "Last Update: $($stateJson.timestamp)" -ForegroundColor DarkGray
    } catch {
        Write-Host "⚠️ Error reading editor state file." -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️ Unity Editor not connected (No state file found). Is the project open?" -ForegroundColor Yellow
}

# 2. Check Recent Errors
if (Test-Path $errorFile) {
    $lines = Get-Content $errorFile
    $count = $lines.Count
    if ($count -gt 0) {
        Write-Host "`n[RECENT ERRORS ($count)]" -ForegroundColor Red
        # Get last 5 lines
        $lastErrors = $lines | Select-Object -Last 5
        foreach ($line in $lastErrors) {
            try {
                $err = $line | ConvertFrom-Json
                Write-Host "[$($err.type)] $($err.message)" -ForegroundColor Red
            } catch {
                Write-Host "Raw Log: $line" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "`n[NO ERRORS LOGGED]" -ForegroundColor Green
    }
}

Write-Host "`n[End of Status Log]" -ForegroundColor Cyan
