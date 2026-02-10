$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$projectSettings = "$projectRoot\ProjectSettings\ProjectSettings.asset"

Write-Host "STARTING WEBGL OPTIMIZATION CHECK"

if (Test-Path $projectSettings) {
    $content = Get-Content $projectSettings
    
    if ($content -match "colorSpace: 1") {
        Write-Host " [OK] Color Space: Linear"
    } else {
        Write-Host " [WARN] Color Space is NOT Linear"
    }

    if ($content -match "managedStrippingLevel: 1") {
        Write-Host " [OK] Stripping Level: Low/Medium"
    } else {
         Write-Host " [INFO] Stripping Level might need adjustment"
    }

    if ($content -match "webGLMemorySize: 256") {
         Write-Host " [WARN] WebGL Memory is default 256MB"
    }
} else {
    Write-Host " [ERROR] ProjectSettings.asset not found"
}

Write-Host "OPTIMIZATION CHECK COMPLETE"
