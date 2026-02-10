$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$assetsPath = "$projectRoot\Assets"

Write-Host "`n[STARTING USS/UXML LINT]`n" -ForegroundColor Cyan

# 1. Start USS Scan
$ussFiles = Get-ChildItem -Path $assetsPath -Include *.uss -Recurse
foreach ($file in $ussFiles) {
    $content = Get-Content $file.FullName
    $lineNum = 0
    foreach ($line in $content) {
        $lineNum++
        # Check for shorthand transition with ease
        if ($line -match "transition:\s*.*ease") {
            Write-Host "⚠️ [USS] $($file.Name):$lineNum - Shorthand 'transition' with 'ease' detected. Use 'transition-property' and 'transition-timing-function' separately." -ForegroundColor Yellow
        }
        # Check for var() usage (general warning, acceptable in USS but risky in inline)
        if ($line -match "var\(--") {
            # This is generally OK in USS files, but flagging for awareness
        }
    }
}

# 2. Start UXML Scan
$uxmlFiles = Get-ChildItem -Path $assetsPath -Include *.uxml -Recurse
foreach ($file in $uxmlFiles) {
    $content = Get-Content $file.FullName
    $lineNum = 0
    foreach ($line in $content) {
        $lineNum++
        # Check for inline styles using var()
        if ($line -match "style=.*var\(--") {
            Write-Host "❌ [UXML] $($file.Name):$lineNum - Inline style uses var(). THIS WILL CAUSE PARSING ERRORS." -ForegroundColor Red
        }
        # Check for relative paths in src
        if ($line -match 'src="\.\./') {
            Write-Host "⚠️ [UXML] $($file.Name):$lineNum - Relative path detected. Use 'project://database/Assets/...' for safety." -ForegroundColor Yellow
        }
    }
}

Write-Host "`n[LINT COMPLETE]`n" -ForegroundColor Cyan
