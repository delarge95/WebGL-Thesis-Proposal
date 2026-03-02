---
description: Workflow Generator for Automating Multi-Step Repetitive Tasks
---

# `workflow_generator` Skill

## Goal
Crear pipelines en la carpeta `.agent/workflows/` que contengan pasos de ejecución repetibles con anotaciones `// turbo` para que el Agente pueda auto-ejecutar tareas monótonas críticas de forma segura y veloz.

## Capabilities
- Identifica cuellos de botella en la operación del proyecto (e.g. Compilar PDFs, Analizar Logs de Unity).
- Crea archivos `.md` de workflows estandarizados que el Agente puede leer e interpretar vía `/slash-command`.

## Execution Protocol
1. Cuando el usuario solicita un proceso repetitivo (e.g. "compila los PDFs desde los .tex en la carpeta Propuesta"), el Agente invoca este *skill*.
2. El skill genera el archivo `.agent/workflows/[nombre].md`.
3. El archivo utiliza sintaxis numerada (`1. `, `2. `) y anota los comandos seguros con `// turbo` para autorizar ejecución asíncrona acelerada, previniendo cuellos de botella de red/confirmación del usuario en pasos inofensivos (como `pdflatex` compilations o lecturas estáticas).
