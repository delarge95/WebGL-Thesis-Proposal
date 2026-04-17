$files = @(
    "E:\WebGL_tesis\README.md",
    "E:\WebGL_tesis\CONTRIBUTING.md",
    "E:\WebGL_tesis\desarrollo\README.md",
    "E:\WebGL_tesis\desarrollo\docs\README.md",
    "E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md",
    "E:\WebGL_tesis\docs\manuals\README.md",
    "E:\WebGL_tesis\External_docs\README.md",
    "E:\WebGL_tesis\Informe_final\Desarrollo_App\README.md",
    "E:\WebGL_tesis\Informe_final\Desarrollo_App\hojas_de_ruta\README.md",
    "E:\WebGL_tesis\portafolio_personal\README.md",
    "E:\WebGL_tesis\portafolio_personal\assets_fuente\README.md",
    "E:\WebGL_tesis\portafolio_personal\herramientas_fuente\README.md"
)

$header = @"
---
tipo: sistema
estado: mantenido
---
#sistema #configuracion_tecnica

"@

foreach ($f in $files) {
    if (Test-Path $f) {
        $content = Get-Content -Path $f -Raw -Encoding UTF8
        if (-not $content.StartsWith("---")) {
            $newContent = $header + $content
            Set-Content -Path $f -Value $newContent -Encoding UTF8
            Write-Host "Modificado: $f"
        }
    }
}
Write-Host "Done"
