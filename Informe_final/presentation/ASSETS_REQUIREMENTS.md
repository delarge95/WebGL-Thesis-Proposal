# Requisitos de assets para la sustentacion
## TwinSight X500 - Capturas, videos, diagramas y respaldos

Estado: plan de medios para construir el deck.
Fecha de actualizacion: 2026-06-12.
Este documento no es guion oral y no genera diapositivas por si solo.

Archivos relacionados:

- `PRESENTATION_OUTLINE.md`: estructura de slides.
- `PRESENTATION_SCRIPT.md`: guion oral y transiciones.
- `SPEAKER_CARDS.md`: tarjetas orales cortas para ensayo cronometrado.
- `DEFENSE_STUDY_GUIDE.md`: explicaciones simples/tecnicas, evidencia y preguntas por bloque.
- `BIBLIOGRAPHY_EVIDENCE_ATLAS.md`: citas breves verificadas y limites bibliograficos.
- `DEFENSE_EVIDENCE_MAP.md`: trazabilidad de cada slide.
- `DEFENSE_PACKAGE_AUDIT_2026-06-12.md`: auditoria interna y riesgos residuales.
- `JURY_QA_BANK.md`: preguntas probables del jurado.
- `DEMO_SCRIPT.md`: recorrido de demo.

## 1. Politica de evidencia visual

Todo asset usado en la defensa debe cumplir cuatro reglas:

1. Debe provenir de la build real, del informe final o de anexos publicos versionados.
2. Debe apoyar una afirmacion concreta del slide.
3. No debe mostrar datos personales, rutas locales, ventanas privadas ni archivos internos no publicados.
4. No debe mostrar funcionalidades no expuestas como si fueran parte de la UI final.

## 2. Carpeta recomendada para medios nuevos

Cuando se capturen videos o GIFs, guardarlos en:

```text
Informe_final/presentation/media/
```

Convenciones:

- Videos principales: `.mp4`, H.264, 1920x1080, 30 fps.
- Clips cortos: `.mp4` o `.webm`, 6-15 s, sin audio.
- Capturas: `.png`, minimo 1920x1080 para desktop y 1080x1920 para movil si aplica.
- Nombres en minuscula con prefijo: `vid_`, `gif_`, `img_`, `diag_`.

## 3. Videos y microinteracciones

| Archivo propuesto | Duracion | Slide | Captura requerida | Observacion |
|---|---:|---:|---|---|
| `vid_01_demo_compilado.mp4` | 75-90 s | 17 | Recorrido completo: Hero/Explore -> seleccion -> bottom sheet -> isolate -> explode -> cut -> Studio/Thermal | Ruta principal de demo si el vivo falla |
| `gif_01_selection_bottom_sheet.mp4` | 8-12 s | 11-12 | Click/tap en pieza, resaltado y apertura de ficha inferior | Mostrar que la malla se traduce a dato tecnico |
| `gif_02_hotspot_isolate_fastener.mp4` | 8-12 s | 11-13 | Hotspot o pieza madre, aislamiento y fastener contextual | Mostrar jerarquia, no solo seleccion |
| `gif_03_explode_cut_filters.mp4` | 10-15 s | 13 | Explode, corte transversal y filtro por categoria | Mostrar lectura de ensamblaje |
| `gif_04_modes_thermal.mp4` | 10-15 s | 14-15 | Realistic, X-Ray, Solid, Thermal y leyenda | Thermal siempre rotulado como heuristico |
| `gif_05_mobile_interaction_fix.mp4` | 8-12 s | 22 | Tutorial, bottom sheet o gesto movil corregido | Opcional para Think-Aloud y ajustes formativos |
| `vid_backup_full_demo.mp4` | 3-5 min | Backup | Recorrido completo sin cortes | Contingencia si no hay red o WebGL falla |

No grabar herramientas de medicion, BOM, anotaciones, conexiones o modulos ocultos como demostracion principal.

## 4. Capturas estaticas ya disponibles

Estas imagenes ya existen y son candidatas para el deck:

| Archivo | Uso recomendado |
|---|---|
| `Informe_final/figures/screenshots_contextual/fig_ui_hero_mobile_pc.png` | Slide 1, portada o cierre |
| `Informe_final/figures/screenshots_contextual/fig_ui_explore_mobile_pc.png` | Slide 12, flujo visible |
| `Informe_final/figures/screenshots_contextual/fig_ui_info_panel.png` | Slide 11, seleccion y ficha |
| `Informe_final/figures/screenshots_contextual/fig_explore_hotspot_selection.png` | Slide 11, hotspots |
| `Informe_final/figures/screenshots_contextual/fig_explore_isolate_sequence.png` | Slide 13, aislamiento |
| `Informe_final/figures/screenshots_contextual/fig_analyze_tool_outputs.png` | Slide 13, Analyze |
| `Informe_final/figures/screenshots_contextual/fig_modes_direct_xray_solid_thermal.png` | Slide 14, modos visuales |
| `Informe_final/figures/screenshots_contextual/fig_modes_studio_presets.png` | Slide 14, Studio |
| `Informe_final/figures/screenshots_contextual/fig_thermal_single.png` | Slide 15, Thermal |
| `Informe_final/figures/screenshots_contextual/fig_profiler_internal_evidence.png` | Slide 18 o backup B5 |
| `Informe_final/figures/screenshots_contextual/fig_device_matrix_clean.png` | Slide 19, rendimiento por dispositivo |
| `Informe_final/figures/screenshots_contextual/fig_render_optimized_drone_stack.png` | Slide 8 o 9, activo optimizado |
| `Informe_final/figures/screenshots_contextual/fig_cad_bake_high_pair.png` | Slide 8, antes |
| `Informe_final/figures/screenshots_contextual/fig_cad_bake_low_pair.png` | Slide 8, despues |
| `Informe_final/figures/screenshots_contextual/fig_fastener_modular_array.png` | Backup tecnico de fasteners |
| `Informe_final/figures/screenshots_contextual/fig_fastener_modular_pieces.png` | Backup tecnico de fasteners |

Antes de usar capturas del informe, recortarlas o recomponerlas para formato 16:9. No insertar figuras densas del PDF sin redisenarlas para lectura oral.

## 5. Diagramas a producir

| Diagrama | Slide | Proposito | Fuente |
|---|---:|---|---|
| `diag_01_problem_bridge.svg/png` | 2 | Documentacion plana -> reconstruccion mental -> 3D interactivo | Cap. 1 y storytelling |
| `diag_02_scope_boundary.svg/png` | 4 | Visual product twin vs digital twin operacional | Cap. 1, 6 |
| `diag_03_objectives_map.svg/png` | 5 | Objetivo general y OE1-OE4 | Cap. 1 |
| `diag_04_methodology_timeline.svg/png` | 6 | DSR, build, mediciones, usuarios, cierre | Cap. 3 |
| `diag_05_evidence_triangulation.svg/png` | 7 | KPIs, tareas, SUS, NASA-TLX, Think-Aloud | Cap. 3 y 5 |
| `diag_06_pipeline_3d_webgl.svg/png` | 8 | CAD/Blender/bake/FBX/Unity/WebGL | Cap. 4 |
| `diag_07_geometry_runtime_difference.svg/png` | 9 | 95 617 vs 229 054 sin contradiccion | Cap. 5 |
| `diag_08_runtime_architecture.svg/png` | 10 | UI, managers, datos, shaders, profiler | Anexo D |
| `diag_09_taxonomy_selection.svg/png` | 11 | Piezas, anchors, renderers, fasteners | Anexo F, G |
| `diag_10_results_summary.svg/png` | 20-21 | SUS, NASA-TLX y tareas | Cap. 5 |
| `diag_11_limits_matrix.svg/png` | 22 | Demuestra / no demuestra | Discusion, conclusiones |

Los diagramas deben tener titulos-sentencia y maximo dos niveles de detalle. Las versiones completas quedan para backup.

## 6. Graficos de resultados

| Grafico | Datos permitidos | Lectura obligatoria |
|---|---|---|
| SUS | Promedio 91,88; mediana 95,00; n=12 | Usabilidad percibida alta del prototipo 3D, lectura descriptiva |
| NASA-TLX Raw | 3D 8,69; 2D 19,89; diferencia 11,19 | Menor carga percibida en la muestra, no inferencia poblacional |
| Rendimiento | 17,6 FPS limite inferior Android; 30,0 FPS Android media-alta; 58,7 FPS iOS; escritorio segun tabla del informe | Compatibilidad acotada por dispositivo, no universal |
| Geometria | 95 617 triangulos activo base; 229 054 triangulos runtime instrumentado | Metricas distintas, no contradiccion |
| Tareas | 96 registros tarea-condicion completados; T1-T3 cronometradas; T4 exploratoria no cronometrada | Efecto techo y lectura complementaria con tiempos/carga |

No crear tarjetas de metricas sin fuente. Si un dato no esta en capitulo 5, anexos de validacion o profiler exportado, no entra en la ruta principal.

## 7. Matriz slide-asset

| Slide | Asset minimo | Alternativa si falta |
|---:|---|---|
| 1 | Hero de app | `fig_ui_hero_mobile_pc.png` |
| 2 | Comparacion 2D/CAD/app | Diagrama problema + captura app |
| 3 | Trade-off fidelidad/legibilidad/rendimiento | Diagrama nuevo |
| 4 | Frontera visual product twin | Diagrama nuevo |
| 5 | Objetivos | Mapa nuevo |
| 6 | Metodologia | Timeline nuevo |
| 7 | Triangulacion | Diagrama nuevo |
| 8 | Pipeline 3D | Diagrama nuevo + `fig_cad_bake_*` |
| 9 | Geometria | Tarjeta comparativa |
| 10 | Arquitectura | Diagrama nuevo o figura APA redisenada |
| 11 | Taxonomia/seleccion | `fig_ui_info_panel.png` + diagrama |
| 12 | Flujo publico | Secuencia de capturas |
| 13 | Inspect/Analyze | `gif_03_explode_cut_filters.mp4` o `fig_analyze_tool_outputs.png` |
| 14 | Studio/modos | `gif_04_modes_thermal.mp4` o grid de capturas |
| 15 | Thermal | `fig_thermal_single.png` |
| 16 | Demo setup | Slide textual minimo |
| 17 | Demo | `vid_01_demo_compilado.mp4` |
| 18 | Profiler | `fig_profiler_internal_evidence.png` + mini tabla |
| 19 | Rendimiento | Barras por dispositivo |
| 20 | SUS | Grafico simple |
| 21 | NASA-TLX | Comparativa 3D/2D |
| 22 | Discusion/limites | Matriz demuestra/no demuestra |
| 23 | Contribuciones | Tres columnas |
| 24 | Cierre | Hero final + takeaways |

## 8. Checklist de captura

- [ ] Cerrar ventanas con rutas locales, nombres personales o archivos privados.
- [ ] Usar la build publicada o una build local equivalente al cierre academico.
- [ ] Mantener la misma resolucion base para todos los videos del deck.
- [ ] Activar cache antes de grabar si se demostrara rendimiento percibido.
- [ ] Grabar una toma limpia sin narracion y otra de respaldo con cursor visible.
- [ ] Verificar que los clips no muestran herramientas ocultas o legacy.
- [ ] Exportar miniaturas de cada video para usar si el reproductor falla.
- [ ] Guardar hashes o notas de origen si se usan como evidencia critica.

## 9. Criterios visuales

- Fondo oscuro tecnico consistente con `DESIGN_SYSTEM.md`.
- Titulos como afirmaciones, no etiquetas genericas.
- Maximo 40 palabras visibles en slides conceptuales.
- Evitar tablas completas salvo en backup.
- Usar color con significado: azul para sistema, verde para evidencia favorable, naranja para limite o advertencia.
- Evitar screenshots sin recorte, texto pequeno o interfaz no legible.

## 10. Pendientes antes de generar el deck

1. Capturar `vid_01_demo_compilado.mp4`.
2. Capturar los cuatro clips cortos de microinteracciones.
3. Redisenar los once diagramas listados.
4. Definir si la defensa usara demo vivo, video principal o ambos.
5. Preparar 10 slides de backup tecnico al final del deck.
6. Sincronizar el deck final con `PRESENTATION_OUTLINE.md`, `DEMO_SCRIPT.md` y el informe final.
