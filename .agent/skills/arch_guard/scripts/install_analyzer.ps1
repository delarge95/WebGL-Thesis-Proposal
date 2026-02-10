$projectRoot = "e:\WebGL_tesis\desarrollo\unity_project"
$analyzerSource = "e:\WebGL_tesis\.agent\skills\arch_guard\analyzers"
$destPath = "$projectRoot\Assets\Analyzers"

Write-Host "INSTALLING ROSLYN ANALYZER"

# 1. Build the Analyzer
Write-Host "1. Building UnityArchitectGuard.dll..."
Set-Location $analyzerSource
try {
    dotnet build -c Release
} catch {
    Write-Host " [ERROR] Build failed. Is .NET SDK installed?"
    exit
}

# 2. Deploy to Unity
Write-Host "2. Deploying to Assets/Analyzers..."
if (-not (Test-Path $destPath)) {
    New-Item -ItemType Directory -Force -Path $destPath | Out-Null
}

$buildOutput = "$analyzerSource\bin\Release\netstandard2.0\UnityArchitectGuard.dll"
if (Test-Path $buildOutput) {
    Copy-Item $buildOutput $destPath -Force
    Write-Host "SUCCESS: Installed"
} else {
    Write-Host "ERROR: DLL missing"
}

Write-Host "INSTALL COMPLETE"
