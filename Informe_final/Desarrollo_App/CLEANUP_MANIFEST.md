# Cleanup Manifest

## Objetivo

Establecer criterios explícitos para depurar el repositorio sin perder trazabilidad académica ni histórica.

## Principios

- No eliminar material con valor probatorio para la tesis sin dejar referencia cruzada.
- Separar claramente producto vigente, documentación histórica y experimentos.
- Preferir archivar y etiquetar antes que borrar.
- No eliminar por ahora los candidatos de descarte seguro: primero deben pasar por una carpeta temporal `trash/`.
- Mover sólo después de que la documentación apunte a la nueva ubicación.

## Mantener en ruta principal

- `desarrollo/unity_project/`
- `docs/`
- `External_docs/`
- `Informe_final/`
- `portafolio_personal/`
- `README.md`
- `CONTRIBUTING.md`
- `CHANGELOG.md` si se consolida a nivel raíz en una fase posterior

## Mantener pero revisar consistencia

- Documentación técnica dentro de `Informe_final/Desarrollo_App/`
- Planes y hojas de ruta activas usadas como soporte de tesis
- Assets de presentación vinculados a la defensa o a GitHub Pages
- `External_docs/` mientras siga siendo referencia explícita para auditorías UX/UI y documentación operativa, pero restringido a material externo y no a documentos fuente del proyecto
- `desarrollo/logs/project_graph.json` como artefacto operativo de análisis del proyecto mientras siga siendo reutilizado por tooling interno
- Manuales, README, guías de demo y matrices de entrega, que deben distinguir explícitamente entre funcionalidades visibles en la UI actual, módulos implementados pero no integrados y pendientes reales de validación

## Mapa documental operativo

- `docs/`: capa documental publicada o publicable del repositorio; concentra el sitio, assets web y manuales expuestos para consulta externa.
- `desarrollo/docs/`: documentación técnica e investigativa de trabajo, vinculada al desarrollo activo del prototipo y su pipeline.
- `docs/manuals/`: documentación publicada o publicable para uso del prototipo, demo, despliegue y validación externa.
- `Informe_final/`: paquete académico formal de tesis, manuales y presentación de defensa.
- `Informe_final/Desarrollo_App/`: capa de auditoría, trazabilidad, bitácora, hojas de ruta y consolidación documental del repositorio.
- `External_docs/`: referencias externas de apoyo; no debe absorber documentación original del proyecto.

### Regla estructural de lectura rápida

- `docs/` responde a: "esto se publica o se consulta desde fuera".
- `desarrollo/docs/` responde a: "esto documenta el trabajo técnico interno".
- `External_docs/` responde a: "esto fue producido fuera del proyecto y se conserva como referencia".

## Distribución operativa adicional

- `Informe_final/presentation/`: paquete de defensa; sólo debe contener outline, sistema visual, prompts y requisitos de assets para diapositivas.
- `Informe_final/validacion/`: instrumentos académicos de evaluación y anexos de validación vinculados a la tesis.
- Duplicados intencionales entre `docs/manuals/` e `Informe_final/validacion/` o manuales académicos se aceptan cuando uno cumple función de consulta web/publicable y el otro pertenece al paquete formal de tesis.
- Duplicados exactos sin función diferenciada entre `docs/manuals/` e `Informe_final/presentation/` deben retirarse de la carpeta de presentación.
- `desarrollo/docs/` puede conservar la fuente técnica de trabajo aunque existan copias exactas en `docs/manuals/` para publicación web, especialmente en `ARCHITECTURE.md` y `DEPLOYMENT_GUIDE.md`.
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/` concentra planes, changelogs y auditorías históricas que no deben seguir sueltos en la raíz de `desarrollo/unity_project/`.
- `Informe_final/Desarrollo_App/hojas_de_ruta/` debe reservar su raíz para planes vigentes e índices locales; cuando convivan documentos de distinta vigencia, el histórico metodológico debe segregarse en una subcarpeta explícita.

## Distribución operativa de `External_docs/`

- `External_docs/academic_templates/`: formatos, plantillas y normas externas de apoyo académico.
- `External_docs/ux_ui_reference/`: guías, auditorías y material visual externo para UX/UI.
- `External_docs/unity_webgl_reference/`: referencias externas de Unity, UI Toolkit y WebGL.

## Candidatos a archivo histórico

- Borradores superseded de presentación o landing page que ya no alimenten `docs/`
- Notas intermedias de investigación que ya estén consolidadas en capítulos finales
- Informes de iteraciones UX reemplazados por versiones más recientes
- Material de experimentación descartado que no participe en el build ni en la tesis final

## Candidatos a eliminación segura

Estos elementos no se eliminarán en la fase actual. Se moverán primero a `trash/` como cuarentena operativa, diferenciándolos de `archive/`, que conserva material con valor histórico, técnico o académico.

- Duplicados exactos de documentos
- Exportaciones temporales generadas por herramientas externas
- Capturas o builds obsoletos fuera del flujo de publicación actual
- Archivos de prueba sin referencias desde código, docs o anexos

## Destinos de reubicación

- `archive/`: material histórico útil para memoria de proyecto, trazabilidad, referencia técnica o evidencia metodológica.
- `trash/`: archivos prescindibles para la operación actual, sin borrado inmediato, en espera de validación final antes de una limpieza definitiva.
- `portafolio_personal/`: carpeta curada de uso personal para extraer piezas, assets, herramientas y documentación orientada a portafolio profesional, incluyendo documentos de organización, checklists y breakdowns, sin mezclarla con el paquete académico ni con el build.

## Verificaciones previas a cualquier limpieza

1. Confirmar que el archivo no está referenciado en la tesis, README, docs web o scripts de build.
2. Confirmar que el archivo no participa en GitHub Pages ni en el build WebGL.
3. Registrar en bitácora la razón de mover, archivar o enviar a `trash/`.
4. Si el archivo tiene valor histórico, moverlo a `archive/` con fecha y motivo.
5. Si el archivo parece prescindible pero aún no se quiere eliminar, moverlo a `trash/` con una nota breve de origen y fecha.

## Próxima acción recomendada

Reintentar el movimiento de logs bloqueados del shader compiler y revisar si queda algún otro artefacto generado fuera de `trash/` que no pertenezca a una ruta activa del proyecto.

## Inventario operativo inicial de `External_docs/` (2026-03-11)

### Distribución aplicada en esta ronda

- `External_docs/academic_templates/base.pdf`
- `External_docs/academic_templates/F-7-9-1.doc`
- `External_docs/academic_templates/FORMATO_DE_PRESENTACIÓN_PROPUESTA_PROYECTO_APLICADO_COMO_ALTERNATIVA_DE_TRABAJO_DE_GRADO (4).pdf`
- `External_docs/academic_templates/Norma_APA_7_Edicion (1).pdf`
- `External_docs/academic_templates/Plantilla_Normas_APA_7a_Edicion.docx`
- `External_docs/academic_templates/Plantilla_Normas_APA_7a_Edicion.pdf`
- `External_docs/ux_ui_reference/guia_botones.png`
- `External_docs/ux_ui_reference/Guía Completa de Diseño UI_UX para Aplicaciones Mó.md`
- `External_docs/unity_webgl_reference/UI_Toolkit_WebGL_Guide.md`
- `External_docs/unity_webgl_reference/Unity Slider Dragger.md`
- `External_docs/unity_webgl_reference/Unity6_WebGL_Advanced.md`
- `External_docs/unity_webgl_reference/Unity6_WebGL_Architecture.md`

### Criterio aplicado en esta sexta pasada

- `academic_templates`: soporte académico externo reutilizable, no generado por el proyecto.
- `ux_ui_reference`: bibliografía operativa y material visual externo usado como referencia de diseño.
- `unity_webgl_reference`: soporte técnico externo para decisiones de implementación web y UI.
- `External_docs/` deja de tratarse como lote ambiguo y pasa a funcionar como repositorio tipificado de referencias externas.

### Ajuste posterior de clasificación (2026-03-11)

- `UX_Audit_Iteration14.md` y `UX_Research_Plan_Iteration12.md` salieron de `External_docs/` al confirmarse que son documentos internos del proyecto, no referencias externas.
- Ambos se reubicaron en `Informe_final/Desarrollo_App/Audits/other/` como parte de la capa de auditoría y trazabilidad metodológica.

## Reclasificación documental puntual de capas pública y externa (2026-03-11)

### Movidos correctamente fuera de su capa impropia

- `trash/docs/INSTALL_STATUS.md`
- `Informe_final/Desarrollo_App/Audits/other/UX_Audit_Iteration14.md`
- `Informe_final/Desarrollo_App/Audits/other/UX_Research_Plan_Iteration12.md`

### Criterio aplicado en esta pasada

- `docs/` no debe exponer estados transicionales de instalación o migración que no formen parte de la documentación pública final.
- `External_docs/` sólo debe conservar referencias creadas fuera del proyecto.
- Auditorías, planes de investigación UX e iteraciones propias deben vivir en `Informe_final/Desarrollo_App/` o en la capa histórica correspondiente.

## Normalización de hojas de ruta y alcance entregable (2026-03-11)

### Reorganización aplicada

- `Informe_final/Desarrollo_App/hojas_de_ruta/README.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/PLAN_TRABAJO_FINAL.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/historico_metodologico/HOJA_DE_RUTA_v1.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/historico_metodologico/MICROINTERACTIONS_PLAN.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/historico_metodologico/PLAN_MODELADO_DRON.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/historico_metodologico/PLAN_UI_OVERHAUL_v2.md`

### Criterio aplicado en esta pasada

- La raíz de `hojas_de_ruta/` sólo debe contener planes todavía útiles para orientar el cierre del proyecto o índices que ayuden a leer la carpeta.
- Los planes de exploración, propuesta o iteración ya superados deben concentrarse en `historico_metodologico/` para conservar trazabilidad sin contaminar la lectura operativa actual.
- La documentación pública y académica debe diferenciar de forma expresa entre funciones visibles en la UI final, módulos presentes en código pero no integrados y pendientes prioritarios todavía en desarrollo.

### Corrección documental asociada

- La herramienta de medición queda clasificada como módulo presente en el proyecto pero no integrado en el prototipo final visible.
- El flujo de encendido con animación y propagación progresiva de calor en la vista `Thermal` queda registrado como pendiente prioritario de integración.

## Inventario operativo de limpieza puntual en `Informe_final/presentation/` (2026-03-11)

### Movidos correctamente a `trash/`

- `trash/Informe_final/presentation/DEMO_SCRIPT.md`

### Criterio aplicado en esta séptima pasada

- `DEMO_SCRIPT.md`: `trash-candidate` por ser un duplicado exacto sin referencias activas dentro del paquete de presentación, manteniéndose la fuente operativa en `docs/manuals/DEMO_SCRIPT.md`.
- `Informe_final/presentation/` queda restringido a materiales de defensa y ya no a documentación operativa publicada.

## Duplicados exactos conservados por función diferenciada (2026-03-11)

- `desarrollo/docs/ARCHITECTURE.md` ↔ `docs/manuals/ARCHITECTURE.md`
- `desarrollo/docs/DEPLOYMENT_GUIDE.md` ↔ `docs/manuals/DEPLOYMENT_GUIDE.md`
- `Informe_final/validacion/CUESTIONARIO_SUS.md` ↔ `docs/manuals/CUESTIONARIO_SUS.md`
- `Informe_final/validacion/CUESTIONARIO_NASA_TLX.md` ↔ `docs/manuals/CUESTIONARIO_NASA_TLX.md`

### Criterio aplicado en esta octava pasada

- `desarrollo/docs/` conserva la fuente de trabajo técnico.
- `docs/manuals/` mantiene espejos publicables o navegables desde la web del repositorio.
- `Informe_final/validacion/` conserva la copia académica formal de los instrumentos de evaluación.
- No se mueven estos duplicados mientras su función siga diferenciada y documentada.

## Inventario operativo inicial de `trash/` (2026-03-11)

### Movidos correctamente a `trash/`

- `trash/desarrollo/unity_project/WEBGL_BUILD_GUIDE.md`
- `trash/desarrollo/unity_project/root_helper_scripts/fix_css.py`
- `trash/desarrollo/unity_project/root_helper_scripts/injector.py`
- `trash/desarrollo/unity_project/root_helper_scripts/injector_cs.py`
- `trash/desarrollo/unity_project/root_helper_scripts/replace_uss.py`
- `trash/desarrollo/unity_project/root_helper_scripts/replace_uss2.py`
- `trash/desarrollo/unity_project/GeneratedAssets_deleted/`
- `trash/desarrollo/unity_project/obj/`
- `trash/blender_files/drone_00(step).blend@`
- `trash/blender_files/drone_00(stp).blend@`
- `trash/blender_files/drone_00.blend2`
- `trash/Informe_final/informe_final.aux`
- `trash/Informe_final/informe_final.log`
- `trash/Informe_final/informe_final.lot`
- `trash/Informe_final/informe_final.out`
- `trash/Informe_final/informe_final.toc`
- `trash/Informe_final/Manual_de_usuario/manual_usuario.aux`
- `trash/Informe_final/Manual_de_usuario/manual_usuario.log`
- `trash/Informe_final/Manual_de_usuario/manual_usuario.out`
- `trash/Informe_final/Manual_de_usuario/manual_usuario.toc`
- `trash/Informe_final/Manual_tecnico/manual_tecnico.aux`
- `trash/Informe_final/Manual_tecnico/manual_tecnico.log`
- `trash/Informe_final/Manual_tecnico/manual_tecnico.out`
- `trash/Informe_final/Manual_tecnico/manual_tecnico.toc`
- `trash/Propuesta/final_proposal.aux`
- `trash/Propuesta/final_proposal.log`
- `trash/Propuesta/final_proposal.lot`
- `trash/Propuesta/final_proposal.out`
- `trash/Propuesta/final_proposal.toc`
- `trash/Propuesta/justificacion/bibliografia_wrapper.aux`
- `trash/Propuesta/justificacion/bibliografia.log`
- `trash/Propuesta/justificacion/bibliografia_wrapper.log`
- `trash/Propuesta/justificacion/bibliografia_wrapper.out`
- `trash/Propuesta/justificacion/sustentacion.aux`
- `trash/Propuesta/justificacion/sustentacion.log`
- `trash/Propuesta/justificacion/sustentacion.out`
- `trash/desarrollo/unity_project/Assets/UI/Styles/MainTheme_OLD.uss`
- `trash/desarrollo/unity_project/Assets/UI/Styles/MainTheme_OLD.uss.meta`
- `trash/desarrollo/unity_project/.output.txt`
- `trash/desarrollo/unity_project/Logs/agent_errors.json`
- `trash/desarrollo/unity_project/Logs/agent_state.json`
- `trash/desarrollo/unity_project/Logs/Packages-Update.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-0.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-1.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-2.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-3.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-4.log`
- `trash/desarrollo/unity_project/Logs/shadercompiler-UnityShaderCompiler.exe-5.log`
- `trash/.cursor/`
- `trash/raiz_repositorio/.git_log.txt`
- `trash/raiz_repositorio/blender_mcp_addon.py`
- `trash/raiz_repositorio/log8.txt`
- `trash/docs/INSTALL_STATUS.md`

### Criterio aplicado en esta primera pasada

- Backups automáticos de Blender: `trash-candidate` por ser artefactos auxiliares reversibles y no formar parte del flujo principal de build o documentación.
- `obj/`: `trash-candidate` por contener cachés, ensamblados temporales, listas absolutas y referencias de compilación generadas por el tooling de Unity/.NET, sin función documental propia en el repositorio.
- `GeneratedAssets_deleted/`: `trash-candidate` por tratarse de un residuo vacío de assets eliminados, sin uso operativo ni valor académico.
- `WEBGL_BUILD_GUIDE.md`: `trash-candidate` por ser una guía suelta no referenciada, redundante frente a la documentación técnica/publicable vigente de despliegue y settings WebGL.
- `root_helper_scripts/*.py`: `trash-candidate` por tratarse de scripts puntuales de parcheo manual sobre archivos UI, sin integración actual en el flujo del proyecto ni referencias activas.
- Logs de compilación LaTeX: `trash-candidate` por ser artefactos intermedios generados al compilar propuesta e informe, sin valor como entregable final del repositorio.
- Auxiliares LaTeX (`*.aux`, `*.out`, `*.toc`, `*.lot`): `trash-candidate` por ser artefactos regenerables de compilación, sin rol como fuente, entregable final ni evidencia metodológica independiente.
- Hoja de estilos `_OLD`: `trash-candidate` por tratarse de una variante superseded sin referencias funcionales detectadas en la UI actual.
- `.output.txt`: `trash-candidate` por ser un volcado auxiliar accidental de salida de terminal dentro del proyecto Unity, sin rol operativo ni académico.
- Logs y estados generados por herramientas: `trash-candidate` por ser artefactos operativos efímeros, útiles sólo como rastro temporal de ejecución.
- Volcados de git o consola en la raíz del repo: `trash-candidate` por ser exportaciones auxiliares sin función como entregable, fuente o documentación estructural.
- `blender_mcp_addon.py`: `trash-candidate` por ser una copia suelta de tooling externo de Blender MCP en la raíz del repositorio, sin referencias activas detectadas ni integración documentada con el flujo actual de tesis, Unity o despliegue.

### Regla de seguimiento

- Los archivos bloqueados no se fuerzan ni se eliminan: se reintenta su movimiento cuando Unity o el proceso asociado libere el handle.
- Todo nuevo movimiento a `trash/` debe añadirse a este inventario o a un inventario sucesor con fecha explícita.
- Validación documental 2026-03-11: el contenido real actual de `trash/` coincide con este inventario; no se detectan acumulaciones sin clasificación explicada en esta ronda.
- Validación operativa 2026-03-11: `obj/`, `GeneratedAssets_deleted/` y los `shadercompiler-UnityShaderCompiler.exe-*.log` ya salieron de `desarrollo/unity_project/`, dejando `Logs/` fuera de la zona activa del proyecto.

## Reubicación estructural de documentación desde `desarrollo/unity_project/` (2026-03-11)

### Reubicados a estructura documental activa

- `desarrollo/docs/WEBGL_BUILD_SETTINGS.md`
- `desarrollo/docs/WEBGL_OPTIMIZATION_MANUAL.md`

### Reubicados a histórico estructurado

- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/PHASE2_UX_PLAN.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/PHASE2_CHANGELOG.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/PHASE3_CHANGELOG.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/REFACTORING_PLAN.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/FASE3_WORKPLAN.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/UX_UI_AUDIT_REPORT.md`

### Criterio aplicado en esta novena pasada

- La raíz de `desarrollo/unity_project/` debe quedar reservada al proyecto Unity activo y a sus archivos de tooling.
- La documentación técnica vigente se reubica en `desarrollo/docs/`.
- Los planes y auditorías históricas del rediseño UX/UI y refactorización se consolidan en `Informe_final/Desarrollo_App/hojas_de_ruta/unity_project_historico/`.
- Scripts puntuales de parcheo y guías sueltas no integradas pasan a `trash/` como cuarentena reversible.

### Validación de cierre de la pasada

- Revisión posterior 2026-03-11: tras la reubicación, la raíz restante de `desarrollo/unity_project/` se limita al proyecto activo, archivos de solución/proyecto y configuración de IDE o tooling.
- Única excepción pendiente: los `shadercompiler-UnityShaderCompiler.exe-*.log` continúan en `desarrollo/unity_project/Logs/` por bloqueo de proceso, sin forzar su eliminación ni su movimiento.

## Reubicación estructural de archivos sueltos del root del repositorio (2026-03-11)

### Reubicados a hojas de ruta

- `Informe_final/Desarrollo_App/hojas_de_ruta/MICROINTERACTIONS_PLAN.md`
- `Informe_final/Desarrollo_App/hojas_de_ruta/PLAN_TRABAJO_FINAL.md`

### Enviados a cuarentena reversible

- `trash/raiz_repositorio/.git_log.txt`
- `trash/raiz_repositorio/log8.txt`

### Criterio aplicado en esta décima pasada

- Los planes de trabajo o diseño con valor metodológico no deben quedar sueltos en la raíz del repositorio cuando ya existe una zona explícita de `hojas_de_ruta/`.
- Los volcados auxiliares de consola o de historial git sin referencias activas se retiran del root y pasan a `trash/` como material reversible y no operativo.

## Reubicación de planeación personal desde `.planning/` (2026-03-11)

### Reubicados a `portafolio_personal/documentacion/planeacion_carrera/`

- `portafolio_personal/documentacion/planeacion_carrera/alexander_woodcock_context.md`
- `portafolio_personal/documentacion/planeacion_carrera/job_search_strategy.md`
- `portafolio_personal/documentacion/planeacion_carrera/master_schedule.md`
- `portafolio_personal/documentacion/planeacion_carrera/plan_audit.md`
- `portafolio_personal/documentacion/planeacion_carrera/portfolio_strategy.md`
- `portafolio_personal/documentacion/planeacion_carrera/task.md`
- `portafolio_personal/documentacion/planeacion_carrera/ticktick_schedule.md`

### Criterio aplicado en esta undécima pasada

- La planeación de carrera, búsqueda laboral y estrategia de portafolio no debe residir como carpeta oculta en la raíz cuando ya existe `portafolio_personal/` como dominio explícito para ese material.
- Las carpetas ocultas de configuración o entorno realmente activas (`.agent/`, `.venv/`) pueden preservarse en raíz cuando siguen cumpliendo una función operativa del workspace; la carpeta `.planning/` no entra en esa categoría porque contiene documentos temáticos, no configuración de herramientas.

## Reubicación de configuración de editor no utilizada desde el root (2026-03-11)

### Enviado a cuarentena reversible

- `trash/.cursor/`

### Criterio aplicado en esta decimocuarta pasada

- Una carpeta oculta de editor sólo debe permanecer en la raíz cuando sigue siendo parte del workspace operativo actual.
- `.cursor/` salió del root porque ya no se usa en este repositorio y no aporta valor documental, académico ni de build; se conserva de forma reversible en `trash/`.
- La validación posterior confirmó que `desarrollo/logs/` sólo contiene `project_graph.json`, que sigue clasificado como artefacto operativo activo y no requiere movimiento.

## Reubicación de tooling externo suelto desde el root (2026-03-11)

### Enviado a cuarentena reversible

- `trash/raiz_repositorio/blender_mcp_addon.py`

### Criterio aplicado en esta duodécima pasada

- Un script o addon externo suelto en la raíz no debe permanecer en la capa principal del repositorio cuando no participa en el build, no está referenciado por documentación activa y no forma parte de la configuración real del workspace.
- `blender_mcp_addon.py` se conserva de forma reversible en `trash/raiz_repositorio/` como copia externa potencialmente útil, pero fuera de la estructura principal de la tesis y del proyecto.

## Reubicación de documentos sueltos desde `desarrollo/` (2026-03-11)

### Reubicados a documentación técnica o investigativa activa

- `desarrollo/docs/investigacion/ANALISIS_RECURSOS_DRON.md`
- `desarrollo/docs/UNITY_SETUP.md`

### Reubicado a hojas de ruta

- `Informe_final/Desarrollo_App/hojas_de_ruta/PLAN_MODELADO_DRON.md`

### Reubicado a archivo histórico

- `archive/desarrollo/CORE_DIRECTIVE.md`

### Criterio aplicado en esta decimotercera pasada

- Los análisis o guías técnicas sueltas en la raíz de `desarrollo/` deben integrarse en `desarrollo/docs/` cuando siguen siendo documentación de soporte del proyecto.
- Los planes metodológicos de modelado con valor de trazabilidad deben consolidarse en `Informe_final/Desarrollo_App/hojas_de_ruta/`.
- Las instrucciones de workflow o persona no integradas al repositorio operativo actual se preservan en `archive/` como contexto histórico, no como documentación vigente de la estructura principal.

## Inventario operativo inicial de `archive/` (2026-03-11)

### Movidos correctamente a `archive/`

- `archive/desarrollo/docs/investigacion/Holybro/cad_addon_iterations/cad_symmetry_addon.py`
- `archive/desarrollo/docs/investigacion/Holybro/cad_addon_iterations/cad_symmetry_addon_v2.py`
- `archive/desarrollo/docs/investigacion/Holybro/cad_addon_iterations/cad_symmetry_addon_v3.py`
- `archive/desarrollo/docs/investigacion/Holybro/cad_addon_iterations/cad_symmetry_addon_v4.py`
- `archive/desarrollo/CORE_DIRECTIVE.md`

### Criterio aplicado en esta segunda pasada

- Iteraciones V1-V4 del addon CAD: `archive-candidate` por su valor metodológico e histórico dentro del pipeline Holybro, pero ya superseded por las versiones `v5_ultimate` y `v6_batch` que siguen en uso referencial.
- `CORE_DIRECTIVE.md`: `archive-candidate` por tratarse de una directiva histórica de interacción/workflow útil como contexto del proceso, pero fuera de la documentación operativa actual del repositorio.

### Rectificación operativa posterior

- `Pixhawk4Mini_QAV250_Kit_QuickStartGuide.pdf` fue devuelto desde `archive/` a `desarrollo/docs/investigacion/Holybro/` tras confirmarse que documenta una pieza fundamental del drone y debe convivir con el resto de manuales activos.

### Clasificación explícita del lote Holybro

- `keep`: `cad_symmetry_addon_v5_ultimate.py`, `cad_symmetry_addon_v6_batch.py`, `CAD_Symmetry_Instancer_Documentation.md`, `generate_inventory.py`, `Holybro X500 V2 Report.md`, `holybro-x500-links.md`, `Holybro_X500_FrameKit_AssemblyGuide.pdf`, `Pixhawk4Mini_QAV250_Kit_QuickStartGuide.pdf`, `X500-Kit_AssemblyManual.pdf`, `Holybro_X500_V2_3D Print/`, `x500v2_blender_inventory.md`, `x500v2_blender_synced_parts.json`, `x500v2_parts_data.json`.
- `archive`: iteraciones superseded del addon.
- `trash`: no aplicar por defecto a la carpeta Holybro salvo artefactos temporales o duplicados inequívocos.
- Validación documental 2026-03-11: el contenido real actual de `archive/` coincide con este inventario y no presenta material adicional sin justificación registrada.

## Inventario operativo inicial de `portafolio_personal/herramientas_fuente/` (2026-03-11)

### Staging técnico curado

- `portafolio_personal/herramientas_fuente/pipeline_holybro/cad_symmetry_addon_v5_ultimate.py`
- `portafolio_personal/herramientas_fuente/pipeline_holybro/cad_symmetry_addon_v6_batch.py`
- `portafolio_personal/herramientas_fuente/pipeline_holybro/generate_inventory.py`
- `portafolio_personal/herramientas_fuente/shaders/Blueprint.shader`
- `portafolio_personal/herramientas_fuente/shaders/ClippableLit.shader`
- `portafolio_personal/herramientas_fuente/shaders/Thermal.shader`
- `portafolio_personal/herramientas_fuente/shaders/WireframeWebGL.shader`
- `portafolio_personal/herramientas_fuente/shaders/XRay.shader`
- `portafolio_personal/herramientas_fuente/ui_tecnica/InputManager.cs`
- `portafolio_personal/herramientas_fuente/ui_tecnica/OrbitCameraController.cs`
- `portafolio_personal/herramientas_fuente/ui_tecnica/ProceduralCloseIcon.cs`
- `portafolio_personal/herramientas_fuente/ui_tecnica/ProceduralIconBase.cs`
- `portafolio_personal/herramientas_fuente/ui_tecnica/UIDetailsSheet.cs`

### Criterio aplicado en esta tercera pasada

- `portfolio-candidate`: piezas con valor explicativo fuerte para breakdown técnico, entrevistas o curaduría profesional, copiadas sin desplazar la fuente original.
- `pipeline_holybro`: evidencia del tooling CAD e inventariado del lote Holybro.
- `shaders`: muestra compacta de modos de visualización con valor técnico y visual.
- `ui_tecnica`: muestra corta de interacción, cámara, iconografía procedural y comportamiento del info sheet.

### Restricción de alcance

- Esta pasada no interviene `blender_files/`.
- Toda ampliación futura de `herramientas_fuente/` debe mantenerse como staging por copia y acompañarse de una justificación breve o manifiesto local.

## Inventario operativo inicial de `portafolio_personal/documentacion/` (2026-03-11)

### Documentos base consolidados

- `portafolio_personal/documentacion/00_Indice_y_Organizacion.md`
- `portafolio_personal/documentacion/01_Checklist_Ejecutivo_Portafolio.md`
- `portafolio_personal/documentacion/02_Fuentes_y_Piezas_Candidatas.md`
- `portafolio_personal/documentacion/03_Checklist_Maestro_Reestructuracion.md`
- `portafolio_personal/documentacion/04_Breakdown_Pipeline_CAD_a_WebGL.md`
- `portafolio_personal/documentacion/05_Breakdown_Sistema_Visualizacion_Interactiva.md`
- `portafolio_personal/documentacion/06_Plan_Activos_Visuales_Portafolio.md`
- `portafolio_personal/documentacion/08_Portafolio_Tech_Artist.md`

### Criterio aplicado en esta cuarta pasada

- `portfolio-doc`: documentos propios de planeación, breakdown y producción visual, derivados del proyecto pero ya separados del paquete académico principal.
- `organización`: índice, checklist, inventario y control operativo de la reestructuración.
- `breakdown`: piezas textuales cortas listas para convertirse en post, deck o reel técnico.

## Inventario operativo inicial de `portafolio_personal/assets_fuente/` (2026-03-11)

### Estructura preparada para producción visual

- `portafolio_personal/assets_fuente/heroes/`
- `portafolio_personal/assets_fuente/screenshots/`
- `portafolio_personal/assets_fuente/clips/`
- `portafolio_personal/assets_fuente/diagramas/`
- `portafolio_personal/assets_fuente/README.md`

### Criterio aplicado en esta quinta pasada

- `portfolio-asset-staging`: carpetas vacías pero ya tipificadas para separar portadas, capturas, clips y diagramas.
- `sin dependencias críticas`: esta estructura no reemplaza assets del proyecto ni introduce nuevas rutas requeridas por el build o la tesis.
