# Kimi Presentation Generation Prompt
## TwinSight X500 - Thesis Defense Deck

Estado: prompt auxiliar para generar o revisar un borrador visual.
Fecha: 2026-06-11.
Canon: usar siempre los archivos maestros de `Informe_final/presentation/`.

> Este prompt no reemplaza el informe ni el guion. Sirve para pedir a un generador de presentaciones un deck visual coherente, sin inventar cifras ni recuperar claims antiguos.

```text
You are a senior academic presentation designer creating a thesis defense deck in Spanish.
Generate a 24-slide defense deck for TwinSight X500.

MANDATORY SOURCE FILES
Use these files as the only content authority:
- Informe_final/presentation/PRESENTATION_OUTLINE.md
- Informe_final/presentation/PRESENTATION_SCRIPT.md
- Informe_final/presentation/DEFENSE_EVIDENCE_MAP.md
- Informe_final/presentation/JURY_QA_BANK.md
- Informe_final/presentation/DEMO_SCRIPT.md
- Informe_final/presentation/ASSETS_REQUIREMENTS.md
- README.md
- Informe_final/informe_final.pdf

PROJECT CONTEXT
- Project: TwinSight X500.
- Program: Ingenieria Multimedia.
- Thesis artifact: Unity WebGL prototype for technical visualization and inspection of the Holybro X500 V2 drone.
- Correct scope: visual product twin / visual-semantic product layer.
- Not correct: operational digital twin, live telemetry platform, FEA simulator, predictive maintenance system.

CORE THESIS
TwinSight X500 transforms static documentation and heavy CAD-derived assets of a complex drone assembly into a browser-based 3D experience for technical inspection. Its contribution is not claiming an operational digital twin, but making parts, relations and visual analysis modes legible through WebGL with technical and formative user evidence.

STRICT FACTUAL RULES
- Do not claim operational digital twin.
- Do not claim live telemetry, predictive maintenance, physical synchronization, PLM/CMMS/SCADA/IoT integration or calibrated FEA.
- Thermal must be labeled as heuristic visualization, not physical simulation.
- Do not show Measurement, BOM, annotations, connection tools or legacy panels as final public UI.
- Public visible flow: Hero -> Explore -> part selection -> bottom sheet -> Inspect / Analyze / Studio.
- Visible tools: orbit/zoom/pan, selection, bottom sheet, hotspots, isolate, explode, cross-section, category filters, Realistic, X-Ray, Solid Color, Thermal and environment presets when verified.
- SUS applies only to the 3D prototype.
- NASA-TLX Raw compares perceived workload by condition 3D vs 2D.
- T1, T2 and T3 are timed. T4 is exploratory guided and not timed.
- 95 617 triangles = optimized base exported asset.
- 229 054 triangles estimated = runtime profiler scene. These numbers are not equivalent.
- Compatibility is device-specific, not universal.
- Results are descriptive and formative, not population-level inferential claims.

DESIGN DIRECTION
- Aspect ratio: 16:9.
- Visual style: academic, technical, precise, not commercial.
- One claim per slide.
- Use title-sentences, not generic section titles.
- Maximum 40 visible words in conceptual slides.
- Use screenshots, diagrams, tables and short video/GIF evidence instead of paragraphs.
- Use progressive reveal for complex diagrams.
- Avoid decorative animation. Use animation only for pipeline, architecture, demo steps or result emphasis.
- Do not use stock photos.
- Do not invent screenshots, FPS, build sizes, load times, reductions or citations.

SLIDE ROUTE
Use exactly this 24-slide route:

1. TwinSight X500 convierte un ensamblaje complejo en una experiencia WebGL inspeccionable.
2. El reto central es comprender relaciones, no solo ver piezas.
3. La brecha esta entre fidelidad tecnica, legibilidad y ejecucion web.
4. La tesis responde con un visual product twin, no con un digital twin operacional.
5. Los objetivos conectan pipeline 3D, interaccion, rendimiento y evaluacion formativa.
6. La metodologia usa desarrollo iterativo y validacion descriptiva.
7. La evaluacion combina datos tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud.
8. El pipeline convierte activos tecnicos pesados en geometria legible para WebGL.
9. La reduccion geometrica se lee como presupuesto de activo, no como conteo runtime.
10. La arquitectura separa UI, seleccion, datos runtime, shaders y medicion.
11. La taxonomia permite seleccionar piezas madre, subpiezas, hotspots y fasteners.
12. El flujo publico se concentra en Explore, seleccion, bottom sheet, Inspect, Analyze y Studio.
13. Inspect y Analyze reducen ruido visual para leer relaciones de ensamblaje.
14. Studio y los shaders convierten el modelo en lecturas visuales complementarias.
15. Thermal es una visualizacion heuristica, no una simulacion FEA calibrada.
16. La demo debe probar tres capacidades, no navegar improvisadamente.
17. Demo: de dron completo a pieza, relacion y modo visual.
18. El profiler interno vuelve trazable el rendimiento por escenario y dispositivo.
19. El rendimiento es viable, pero no universal en todo movil.
20. La validacion de usuarios muestra recepcion favorable del prototipo 3D.
21. La condicion 3D redujo carga percibida frente al soporte 2D en la muestra.
22. La discusion acota el resultado: efecto techo, muestra pequena y compatibilidad limitada.
23. La contribucion es tecnica, metodologica y comunicativa.
24. Cierre: hacer legible el hardware complejo desde la web es el aporte defendible.

BACKUP SLIDES
Add optional backup slides after the main deck:
- B1 SUS formula and interpretation.
- B2 NASA-TLX Raw formula and inverted performance dimension.
- B3 Control variables and AB/BA order.
- B4 Full WebGL performance table.
- B5 Profiler JSON/CSV traceability.
- B6 95 617 vs 229 054 triangles.
- B7 Expanded architecture.
- B8 Heuristic Thermal system.
- B9 Methodological limitations.
- B10 Future work by phases.

REQUIRED MEDIA
Use or reserve placeholders for:
- vid_01_demo_compilado.mp4, 75-90 seconds.
- gif_01_selection_bottom_sheet.mp4.
- gif_02_explode_cut_filters.mp4.
- gif_03_modes_thermal.mp4.
- gif_04_mobile_microinteractions.mp4.
- vid_backup_full_demo.mp4.

OUTPUT REQUIREMENTS
For each slide, provide:
- title-sentence;
- visual composition;
- maximum 3 visible bullets or labels;
- animation/reveal note;
- speaker note summary;
- evidence source path.

Do not output unsupported claims. If evidence is missing, mark the slide element as "requires asset" or "requires verification".
```
