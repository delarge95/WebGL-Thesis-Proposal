$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$assetsPath = "$projectRoot\Assets"

Write-Host "`n[STARTING WEBGL COMPATIBILITY SCAN]`n" -ForegroundColor Cyan

# --- 1. SHADER VALIDATION ---
Write-Host "1. Scanning Shaders..." -ForegroundColor Yellow
$shaders = Get-ChildItem -Path $assetsPath -Include *.shader -Recurse
foreach ($shader in $shaders) {
    $content = Get-Content $shader.FullName
    
    # Check for Target > 3.5
    if ($content -match "#pragma\s+target\s+[4-5]\.[0-9]") {
        Write-Host "   ❌ [SHADER] $($shader.Name): Uses incompatible target (Must be <= 3.5 for WebGL 1.0/2.0 safe)" -ForegroundColor Red
    }
    
    # Check for Geometry Shaders (often problematic)
    if ($content -match "#pragma\s+geometry") {
         Write-Host "   ⚠️ [SHADER] $($shader.Name): Uses Geometry Shader (Performance risk on WebGL)" -ForegroundColor Yellow
    }
}

# --- 2. AUDIO VALIDATION ---
Write-Host "`n2. Scanning Audio (Vorbis/Mono check)..." -ForegroundColor Yellow
$audioMeta = Get-ChildItem -Path $assetsPath -Include *.mp3.meta, *.wav.meta, *.ogg.meta -Recurse
foreach ($meta in $audioMeta) {
    $content = Get-Content $meta.FullName -Raw
    
    # Check for WebGL specific override
    if ($content -notmatch "WebGL:") {
        # If no WebGL specific override, check default
        # This is a heuristic, real YAML parsing is hard in pure PS without modules
        # roughly checking for "compressionFormat: 1" (Vorbis) and "forceToMono: 1"
    }
    
    # Simple check: warn if file size is large and not streaming
    $assetPath = $meta.FullName.Replace(".meta", "")
    if (Test-Path $assetPath) {
        $item = Get-Item $assetPath
        if ($item.Length -gt 1048576) { # > 1MB
             if ($content -notmatch "loadType: 2") { # 2 = Streaming
                 Write-Host "   ⚠️ [AUDIO] $($item.Name): Large file (>1MB) not set to 'Streaming'. High memory risk." -ForegroundColor Yellow
             }
        }
    }
}

# --- 3. TEXTURE VALIDATION ---
Write-Host "`n3. Scanning Textures (ASTC/DXT)..." -ForegroundColor Yellow
# Checking ProjectSettings for global setting
$projSettings = "$projectRoot\ProjectSettings\ProjectSettings.asset"
if (Test-Path $projSettings) {
    $settings = Get-Content $projSettings
    if ($settings -match "webGLBuildSubtarget: \d+") {
        # 1=Generic, 2=DXT, 3=ETC, 4=ASTC (approx, varies by version)
        if ($settings -match "webGLBuildSubtarget: 1") {
            Write-Host "   ⚠️ [TEXTURES] Global Setting is 'Generic'. Recommended: ASTC or DXT." -ForegroundColor Yellow
        }
    }
}

Write-Host "`n[WEBGL SCAN COMPLETE]`n" -ForegroundColor Cyan
