param(
    [string]$WorkspaceRoot = "E:/WebGL_tesis",
    [switch]$SkipCompile
)

$ErrorActionPreference = "Stop"

$src = Join-Path $WorkspaceRoot "Informe_final/Desarrollo_App/Audits/07_Diagrams_Workflow/rendered"
$dst = Join-Path $WorkspaceRoot "Informe_final/figures/chapter4"
$texDir = Join-Path $WorkspaceRoot "Informe_final"

if (!(Test-Path $src)) { throw "Source folder not found: $src" }
if (!(Test-Path $dst)) { throw "Destination folder not found: $dst" }
if (!(Test-Path (Join-Path $texDir "informe_final.tex"))) { throw "Missing informe_final.tex in $texDir" }

Write-Host "Rendering Mermaid chapter-4 figures to PDF..."

1..13 | ForEach-Object {
    $mmd = Join-Path $src ("fig_4_{0}.mmd" -f $_)
    $pdf = Join-Path $dst ("fig_4_{0}.pdf" -f $_)

    if (!(Test-Path $mmd)) {
        throw "Missing source file: $mmd"
    }

    npx -y @mermaid-js/mermaid-cli -i $mmd -o $pdf -b white --pdfFit --scale 2
    if ($LASTEXITCODE -ne 0) {
        throw "Mermaid render failed for: $mmd"
    }
}

Write-Host "Render complete."

if (-not $SkipCompile) {
    Write-Host "Compiling informe_final.tex (2 passes)..."
    Push-Location $texDir
    try {
        pdflatex -interaction=nonstopmode -halt-on-error informe_final.tex > build_rerender_pass1.log
        if ($LASTEXITCODE -ne 0) { throw "Pass 1 failed. See build_rerender_pass1.log" }

        pdflatex -interaction=nonstopmode -halt-on-error informe_final.tex > build_rerender_pass2.log
        if ($LASTEXITCODE -ne 0) { throw "Pass 2 failed. See build_rerender_pass2.log" }

        Write-Host "Compile complete: informe_final.pdf"
    }
    finally {
        Pop-Location
    }
}

Write-Host "Done."