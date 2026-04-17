param(
    [Parameter(Mandatory = $true)]
    [string]$UnityExe
)

$projectPath = "E:\WebGL_tesis\desarrollo\unity_project"
$logPath = "E:\WebGL_tesis\desarrollo\docs\unity_batch_build.log"

if (-not (Test-Path $UnityExe)) {
    Write-Error "Unity.exe no encontrado en: $UnityExe"
    exit 1
}

if (-not (Test-Path $projectPath)) {
    Write-Error "Proyecto Unity no encontrado en: $projectPath"
    exit 1
}

& "$UnityExe" -batchmode -quit -projectPath "$projectPath" -executeMethod WebGL.Editor.WebGLBatchBuildEntry.BuildWebGLBatch -logFile "$logPath"
$code = $LASTEXITCODE

Write-Host "ExitCode: $code"
Write-Host "Log: $logPath"

if ($code -ne 0) {
    Write-Error "Build batch fallido. Revisa el log."
    exit $code
}

Write-Host "Build WebGL completado correctamente."
Write-Host "Verifica nuevo archivo en: E:\WebGL_tesis\Telemetria\Unity_Builds"
