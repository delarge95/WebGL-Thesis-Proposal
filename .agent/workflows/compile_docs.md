---
description: Compila documentos LaTeX en PDF para la tesis y reportes
---

# `compile_docs` Workflow

Este flujo de trabajo compila los archivos `.tex` en la carpeta `Propuesta` y `Informe_final` utilizando `pdflatex` o una herramienta instalada si está disponible en el entorno nativo.

## Pasos

1. Verifica la existencia de `pdflatex` u otra herramienta LaTeX en el sistema para proceder.
   - Si no está instalada, usa scripts de Python con la librería `pdfkit` o similar como un fallo silencioso (o informa al usuario de la imposibilidad local).

2. Transfórmate al directorio donde reside la `Propuesta`.
// turbo
3. Ejecuta `pdflatex final_proposal.tex -interaction=nonstopmode`. Si requiere bibliografía, corre `pdflatex` dos veces consecutivas.
// turbo
4. Transfórmate al directorio de `Informe_final/Manual_tecnico/`.
// turbo
5. Ejecuta `pdflatex manual_tecnico.tex -interaction=nonstopmode`.

6. Trasládalo al directorio raíz o reporta el éxito al usuario.
