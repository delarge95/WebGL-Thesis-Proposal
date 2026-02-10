$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$assetsPath = "$projectRoot\Assets"
$uiPath = "$assetsPath\UI"
$settingsPath = "$assetsPath\Settings"

Write-Host "`n[STARTING UNITY ASSET AUDIT]`n" -ForegroundColor Cyan

# 1. Fonts Check
$fontsPath = "$uiPath\Fonts"
if (Test-Path $fontsPath) {
    $fonts = Get-ChildItem -Path $fontsPath -Include *.ttf, *.otf -Recurse
    if ($fonts.Count -gt 0) {
        Write-Host "✅ Fonts found: $($fonts.Count) files." -ForegroundColor Green
    } else {
        Write-Host "⚠️ Fonts folder exists but is empty!" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ MISSING: Assets/UI/Fonts folder." -ForegroundColor Red
}

# 2. Panel Settings Check
$panelSettings = Get-ChildItem -Path $assetsPath -Include *PanelSettings*.asset -Recurse
if ($panelSettings) {
    Write-Host "✅ Panel Settings found: $($panelSettings.Name)" -ForegroundColor Green
} else {
    Write-Host "❌ MISSING: Panel Settings Asset (Required for UI Toolkit)." -ForegroundColor Red
}

# 3. URP Asset Check
$urpAsset = Get-ChildItem -Path $assetsPath -Include URP_WebGL.asset -Recurse
if ($urpAsset) {
    Write-Host "✅ URP WebGL Asset found." -ForegroundColor Green
} else {
    Write-Host "❌ MISSING: URP_WebGL.asset." -ForegroundColor Red
}

# 4. Global Settings Check
$globalSettings = Get-ChildItem -Path $assetsPath -Include UniversalRenderPipelineGlobalSettings.asset -Recurse
if ($globalSettings) {
    Write-Host "✅ URP Global Settings found." -ForegroundColor Green
} else {
    Write-Host "⚠️ MISSING: UniversalRenderPipelineGlobalSettings.asset (Unity might regenerate this)." -ForegroundColor Yellow
}

# 5. USS/UXML Check
$ussFiles = Get-ChildItem -Path $assetsPath -Include *.uss -Recurse
$uxmlFiles = Get-ChildItem -Path $assetsPath -Include *.uxml -Recurse

if ($ussFiles.Count -gt 0) {
    Write-Host "✅ USS Files found: $($ussFiles.Count)" -ForegroundColor Green
    # Quick check for 'ease' in transitions (Unity 6 bug)
    foreach ($file in $ussFiles) {
        $content = Get-Content $file.FullName
        if ($content -match "transition:\s*.*ease") {
            Write-Host "⚠️ POTENTIAL ISSUE: $($file.Name) contains 'ease' in shorthand transition (Unity 6 incompatible)." -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "❌ MISSING: USS Stylesheets." -ForegroundColor Red
}

if ($uxmlFiles.Count -gt 0) {
    Write-Host "✅ UXML Files found: $($uxmlFiles.Count)" -ForegroundColor Green
} else {
    Write-Host "❌ MISSING: UXML Layouts." -ForegroundColor Red
}

Write-Host "`n[AUDIT COMPLETE]`n" -ForegroundColor Cyan
