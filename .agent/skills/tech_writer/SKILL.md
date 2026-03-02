---
description: Technical Writer Assistant for Thesis Documentation Output
---

# `tech_writer` Skill

## Goal
Asegurar consistencia absoluta entre la realidad del repositorio (métricas, nombres de clases, arquitecturas de Unity) y los documentos formales en Markdown o LaTeX.

## Capabilities
- Mapeo de términos globales (e.g. Forzar que siempre diga "Unity 6 (6000.0.x LTS)").
- Parsing de archivos `.tex` y reescritura de conteos de métricas ("9000 líneas" $\rightarrow$ "N líneas reales calculadas").
- Cumplimiento de Normas APA 7.0 al redactar bibliografía y citas dentro de los `.tex`.

## Execution Protocol
1. Lee `CODEBASE_TRUTH.md` y `AUDIT_REPORT.md` como únicas fuentes de verdad absolutas.
2. Lee los archivos `.tex` y ubica discrepancias (e.g., listas de Singletons desactualizadas, scripts erróneos).
3. Usa la herramienta `multi_replace_file_content` para actualizar los bloques de texto garantizando la no destrucción de delimitadores estructurales LaTeX (`\section`, `\begin{itemize}`).
