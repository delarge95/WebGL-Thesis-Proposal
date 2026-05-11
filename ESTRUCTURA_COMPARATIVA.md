# Comparativa y Reestructuracion de Carpetas (Version Unificada y Detallada)
Fecha: 10 de Mayo de 2026

## 1. Estructura de Carpetas Actual Multicefala (As-Is)
Esta topografia refleja la realidad actual escaneada capilarmente. Notese la redundancia de documentacion entre desarrollo, External_docs, raiz y Cerebro_Digital.

`	ext
e:/WebGL_tesis/
|-- archive/
|   |-- desarrollo/
|   \-- logs_historicos/
|-- blender_files/
|   |-- References/
|   \-- welded/
|-- Cerebro_Digital/
|   |-- Templates/
|   \-- Wiki/
|       |-- Concepts/
|       |-- Entities/
|       |-- Entregables/
|       \-- Stubs_NoResueltas/
|-- Datos/
|   \-- Sujetos_Prueba/
|-- desarrollo/
|   |-- docs/                 # Documentacion tecnica en competencia con Obsidian
|   |-- logs/
|   \-- unity_project/
|       |-- Assets/
|       |-- Packages/
|       \-- ProjectSettings/
|-- docs/                     # Frontend Vite
|   |-- Build/
|   |-- manuals/
|   \-- src/
|-- Excalidraw/               # Huerfano
|-- External_docs/
|   |-- academic_templates/
|   |-- Bibliografia/
|   |-- unity_webgl_reference/
|   \-- ux_ui_reference/
|-- Informe_final/            # LaTeX
|   |-- Audits/
|   |-- chapters/
|   |-- Desarrollo_App/
|   |-- figures/
|   |-- Manual_de_usuario/
|   |-- Manual_tecnico/
|   |-- presentation/
|   \-- validacion/
|-- Investigacion/
|   \-- Notas_Conceptuales/   # Huerfano
|-- logs/
|-- MOC_Ecuaciones/           # Huerfano
|-- portafolio_personal/
|   |-- assets_fuente/
|   |-- documentacion/
|   |-- entregables_finales/
|   \-- herramientas_fuente/
|-- Propuesta/                # LaTeX
|-- Telemetria/
|   \-- Unity_Builds/
|-- trash/
\-- [Archivos Sueltos en Raiz] (CONTRIBUTING.md, MOC_UI_UX_Complete.md, README.md, etc.)
`

## 2. Nueva Estructura de Carpetas Objetivo (To-Be)
Consolidacion absoluta. Obsidian domina la documentacion y el analisis. El codigo se aisla. Los binarios y modelos se agrupan ordenadamente. Los archivos raiz se purgan hacia trash/ y archive/.

`	ext
e:/WebGL_tesis/
|-- Cerebro_Digital/                  # [CENTRO ABSOLUTO DE CONOCIMIENTO Y RAG]
|   |-- Excalidraw/                   # <-- REUBICADO desde la raiz.
|   |-- MOC_Ecuaciones/               # <-- REUBICADO desde la raiz.
|   |-- Templates/
|   \-- Wiki/
|       |-- Concepts/                 # <-- ABSORBE: desarrollo/docs/ e Investigacion/Notas_Conceptuales/
|       |-- Entities/
|       |-- Entregables/
|       \-- Stubs_NoResueltas/
|
|-- desarrollo/                       # [NUCLEO ESTRICTO DE CODIGO UNITY]
|   \-- unity_project/
|       |-- Assets/
|       |-- Packages/
|       \-- ProjectSettings/
|
|-- docs/                             # [NUCLEO FRONTEND WEB]
|   |-- Build/                        # Binarios de Unity WebGL
|   |-- manuals/                      # Manuales web estaticos
|   \-- src/                          # Codigo React/Vite
|
|-- Entornos_3D_y_Datos/              # [MODULO DE BINARIOS Y DATA] <-- NUEVO CONTENEDOR LOGICO
|   |-- blender_files/                # <-- REUBICADO desde la raiz. Agrupa meshes.
|   |   |-- References/
|   |   \-- welded/
|   |-- Datos/                        # <-- REUBICADO desde la raiz. Agrupa csv y sujetos.
|   |   \-- Sujetos_Prueba/
|   \-- Telemetria/                   # <-- REUBICADO desde la raiz. Agrupa perf logs.
|       \-- Unity_Builds/
|
|-- External_docs/                    # [MODULO ACADEMICO / SEGUNDA FUENTE]
|   |-- academic_templates/
|   |-- Bibliografia/
|   |-- unity_webgl_reference/
|   \-- ux_ui_reference/
|
|-- Informe_final/                    # [ENTORNO LATEX INTACTO]
|   |-- Audits/
|   |-- chapters/
|   |-- Desarrollo_App/
|   |-- figures/
|   |-- Manual_de_usuario/
|   |-- Manual_tecnico/
|   |-- presentation/
|   \-- validacion/
|
|-- Propuesta/                        # [ENTORNO LATEX INTACTO]
|
|-- portafolio_personal/              # [PORTAFOLIO PERSONAL INTACTO]
|   |-- assets_fuente/
|   |-- documentacion/
|   |-- entregables_finales/
|   \-- herramientas_fuente/
|
|-- archive/                          # [REPOSITORIO HISTORICO]
|   |-- desarrollo_historico/         # <-- ABSORBE: desarrollo/logs/ obsoletos
|   \-- logs_historicos/              # <-- ABSORBE: logs/ sueltos de la raiz
|
|-- trash/                            # [PAPELERA ESTATICA]
|   |-- Archivos_Sueltos_Borrados/    # <-- ABSORBE: Clones de la raiz (CONTRIBUTING, MOCs repetidos)
|   \-- LaTeX_logs_residuales/        # <-- Absorbe .aux .out residuales
|
\-- README.md                         # [UNICA PRESENTACION]
`

## 3. Contraste con los Lineamientos Historicos Obsoletos

### A) El problema con desarrollo/ESTRUCTURA_CARPETAS.md (Plan 2025)
- **Error del documento**: Fragmentaba el conocimiento intentando documentar Unity "desde" Unity y guardaba los renders de Blender tambien alli adentro. Genero carpetas docs y logs adyacentes al codigo.
- **Resolucion (To-Be)**: La capilaridad demostrada arriba elimina desarrollo/docs y desarrollo/logs. El codigo de Unity (en unity_project) queda completamente aislado. Todo plan markdown y bitacora viaja a Cerebro_Digital/Wiki/Concepts.

### B) El problema con Cerebro_Digital/SYSTEM_SCHEMA.md (Directiva "Antigravity")
- **Error del documento**: En su Regla 1 ("Cero Duplicacion") prohibia que la wiki alterara o moviera "Raw Sources". Esto obligo a crear clones absurdos (como tener Excalidraw, MOC_Ecuaciones e Investigacion sueltos en la raiz para "no tocarlos").
- **Resolucion (To-Be)**: Como se observa en la estructura To-Be, Excalidraw/, MOC_Ecuaciones/ y la carpeta raiz huérfana Investigacion/ se mueven a Cerebro_Digital/. Esto rehabilita el soporte grafico y matematico de Obsidian sin duplicar datos en el disco.
