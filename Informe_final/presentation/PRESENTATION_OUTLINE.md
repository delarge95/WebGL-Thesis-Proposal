# Estructura base de sustentacion
## TwinSight X500 - Defensa academica de 30 minutos

Estado: estructura maestra, no guion oral.
Fecha de actualizacion: 2026-06-11.
Fuente academica autoritativa: `Informe_final/informe_final.pdf`.
Fuentes de apoyo revisadas: carpeta `Informe_final/presentation/` y documento local de storytelling indicado por el autor.

## 1. Criterio rector

La defensa debe demostrar comprension, criterio tecnico, validez metodologica y honestidad de alcance. La estructura no sera cronologica ni promocional. Sera evaluativa:

```text
problema -> brecha -> propuesta -> decisiones tecnicas -> implementacion -> evidencia -> limites -> contribucion
```

Cada diapositiva debe tener una sola afirmacion defendible. El visual debe actuar como evidencia de esa afirmacion, no como decoracion. Los detalles finos que no caben en 30 minutos se reservan para preguntas o diapositivas de respaldo.

## 2. Tesis oral central

TwinSight X500 transforma documentacion tecnica y activos 3D pesados de un dron Holybro X500 V2 en un prototipo WebGL inspeccionable, desplegado como visual product twin. El aporte no es afirmar un gemelo digital operacional, sino hacer legibles piezas, relaciones y modos de analisis visual desde navegador, con evidencia tecnica y evaluacion formativa de usuarios.

## 3. Tres mensajes que deben recordarse

1. El problema no era falta de informacion, sino dificultad para reconstruir mentalmente relaciones espaciales y funcionales desde documentacion plana o CAD pesado.
2. La solucion combina pipeline 3D, optimizacion WebGL, taxonomia semantica, UI de inspeccion y modos visuales para convertir un ensamblaje complejo en una experiencia tecnica explorable.
3. La evidencia es favorable pero acotada: SUS alto, NASA-TLX Raw menor frente al soporte 2D, tareas completadas, rendimiento viable por dispositivo y limites claros de alcance, muestra y compatibilidad.

## 4. Duracion objetivo

La exposicion debe apuntar a 27-28 minutos reales y dejar 2-3 minutos de margen para carga, transiciones, latencia o una interrupcion menor. La ronda de preguntas se prepara aparte.

| Bloque | Slides | Tiempo | Funcion |
|---|---:|---:|---|
| Apertura y problema | 1-4 | 4:00 | Crear necesidad y ubicar la pregunta |
| Enfoque metodologico | 5-7 | 4:00 | Mostrar que el proyecto fue evaluable |
| Implementacion tecnica | 8-15 | 10:00 | Demostrar criterio de ingenieria multimedia |
| Demo y evidencia visual | 16-17 | 3:00 | Probar el sistema con una secuencia controlada |
| Resultados y discusion | 18-22 | 5:30 | Interpretar datos, no solo mostrarlos |
| Cierre | 23-24 | 2:00 | Contribucion, limites y continuidad |
| **Total** | **24** | **28:30** | Margen restante: 1:30 |

## 5. Ruta principal de diapositivas

| # | Titulo-sentencia | Tiempo | Claim que defiende | Evidencia / contenido | Visual requerido |
|---:|---|---:|---|---|---|
| 1 | TwinSight X500 convierte un ensamblaje complejo en una experiencia WebGL inspeccionable | 0:30 | Presentar identidad y alcance final | Nombre, programa, autor, director, URL publica | Hero limpio de la app con dron visible |
| 2 | El reto central es comprender relaciones, no solo ver piezas | 1:00 | La documentacion plana exige reconstruccion mental costosa | Manual 2D, CAD pesado, necesidad de inspeccion espacial | Comparacion 2D/CAD/app en tres paneles |
| 3 | La brecha esta entre fidelidad tecnica, legibilidad y ejecucion web | 1:00 | Las soluciones parciales no resuelven simultaneamente los tres criterios | Triangulo fidelidad-legibilidad-rendimiento | Diagrama simple de trade-off |
| 4 | La tesis responde con un visual product twin, no con un digital twin operacional | 1:30 | Acotar evita sobrepromesa | Visual product twin, sin telemetria, sin FEA, sin mantenimiento predictivo | Frontera de alcance: incluido / excluido |
| 5 | Los objetivos conectan pipeline 3D, interaccion, rendimiento y evaluacion formativa | 1:20 | Los objetivos cubren construccion y validacion | Objetivo general y OE1-OE4 resumidos | Mapa 2x2 de objetivos |
| 6 | La metodologia usa desarrollo iterativo y validacion descriptiva | 1:20 | El metodo es apropiado para un prototipo formativo | DSR / ciclo construir-evaluar, muestra no probabilistica | Timeline metodologico |
| 7 | La evaluacion combina datos tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud | 1:20 | La evidencia se triangula desde capas distintas | 12 participantes, tareas 3D/2D, instrumentos | Diagrama de triangulacion |
| 8 | El pipeline convierte activos tecnicos pesados en geometria legible para WebGL | 1:30 | La preparacion 3D es aporte tecnico principal | CAD / Blender / bake / FBX / Unity / WebGL | Pipeline animado por etapas |
| 9 | La reduccion geometrica se lee como presupuesto de activo, no como conteo runtime | 1:10 | Evitar confusion 95 617 vs 229 054 | 95 617 triangulos base, 229 054 runtime profiler | Tarjeta comparativa con nota de no equivalencia |
| 10 | La arquitectura separa UI, seleccion, datos runtime, shaders y medicion | 1:30 | La app no es una escena suelta, es un sistema | UI Toolkit, managers, DronePartData, profiler | Diagrama de capas |
| 11 | La taxonomia permite seleccionar piezas madre, subpiezas, hotspots y fasteners | 1:10 | La interaccion depende de estructura semantica | 28 piezas canonicas, 30 anchors, 257 renderers/colliders | Grafo o matriz de jerarquia |
| 12 | El flujo publico se concentra en Explore, seleccion, bottom sheet, Inspect, Analyze y Studio | 1:15 | El alcance visible esta cerrado y no sobrevende modulos ocultos | Flujo Hero -> Explore -> modos | Capturas secuenciales con flechas |
| 13 | Inspect y Analyze reducen ruido visual para leer relaciones de ensamblaje | 1:20 | Aislar, explotar, cortar y filtrar son decisiones de comprension | Isolate, explode, cut, filtros | GIF corto o secuencia de 4 capturas |
| 14 | Studio y los shaders convierten el modelo en lecturas visuales complementarias | 1:20 | Los modos no son decorativos, guian interpretacion | Realistic, X-Ray, Solid, Thermal, presets | Grid de modos reales |
| 15 | Thermal es una visualizacion heuristica, no una simulacion FEA calibrada | 1:00 | El modo termico aporta lectura relativa dentro de limites | Modelo reducido por componentes, leyenda termica | Captura Thermal + etiqueta "heuristico" |
| 16 | La demo debe probar tres capacidades, no navegar improvisadamente | 0:30 | El demo es evidencia comprimida | Explorar, inspeccionar, analizar | Slide de instrucciones de observacion |
| 17 | Demo: de dron completo a pieza, relacion y modo visual | 2:30 | La app realiza lo que el argumento promete | Video local principal o demo vivo corto | Video 90 s + microdemo opcional |
| 18 | El profiler interno vuelve trazable el rendimiento por escenario y dispositivo | 1:10 | Las metricas tecnicas estan ancladas a datos exportados | FPS, frame time, memoria, dispositivo, escenario | Tabla simplificada + captura profiler |
| 19 | El rendimiento es viable, pero no universal en todo movil | 1:10 | La lectura tecnica es honesta por gama de dispositivo | 17,6 FPS limite inferior; 30,0 FPS media-alta; 58,7 FPS iOS | Barras por dispositivo con limite 30 FPS |
| 20 | La validacion de usuarios muestra recepcion favorable del prototipo 3D | 1:10 | SUS aporta lectura global de usabilidad | SUS promedio 91,88; mediana 95,00 | Grafico SUS simple |
| 21 | La condicion 3D redujo carga percibida frente al soporte 2D en la muestra | 1:20 | NASA-TLX Raw favorece al visor 3D descriptivamente | 3D 8,69; 2D 19,89; diferencia 11,19 | Comparativa 3D vs 2D |
| 22 | La discusion acota el resultado: efecto techo, muestra pequena y compatibilidad limitada | 1:40 | La tesis sabe que demuestra y que no demuestra | Tareas completadas, T4 exploratoria, Think-Aloud, limites | Matriz "demuestra / no demuestra" |
| 23 | La contribucion es tecnica, metodologica y comunicativa | 1:00 | El aporte integra artefacto, evidencia y documentacion | Pipeline, visor, validacion, anexos reproducibles | Tres columnas de contribucion |
| 24 | Cierre: hacer legible el hardware complejo desde la web es el aporte defendible | 1:00 | Volver al problema inicial y abrir preguntas | Conclusiones y trabajo futuro por fases | Dron final + tres takeaways |

## 6. Animaciones y microinteracciones recomendadas

Las animaciones deben controlar la carga cognitiva, no adornar. Usarlas solo para revelar complejidad por capas:

| Uso | Donde aplica | Forma recomendada |
|---|---|---|
| Construccion progresiva | Pipeline, arquitectura, metodologia | Aparecer etapa por etapa |
| Antes / despues | Problema, reduccion geometrica, UI | Comparacion con wipe o fade breve |
| Foco visual | Resultados, tablas, triangulacion | Resaltar una cifra o celda por vez |
| Microdemo | Seleccion, bottom sheet, explode, cut, thermal | GIF/MP4 corto sin audio, 6-12 s |
| Transicion narrativa | Cambio de bloque | Slide separador con una pregunta guia |

Evitar animaciones largas, decorativas o que dependan de sincronizacion exacta con un texto oral memorizado.

## 7. Videos y GIFs necesarios

| ID | Duracion | Uso | Contenido minimo | Prioridad |
|---|---:|---|---|---|
| `vid_01_demo_compilado.mp4` | 75-90 s | Slide 17 | Hero/Explore, seleccion, bottom sheet, isolate, explode, cut, Studio/Thermal | Alta |
| `gif_01_selection_bottom_sheet.mp4` | 8-12 s | Slides 11-12 | Seleccionar pieza y apertura de ficha inferior | Alta |
| `gif_02_explode_cut_filters.mp4` | 10-15 s | Slide 13 | Explode, corte transversal y filtros por categoria | Alta |
| `gif_03_modes_thermal.mp4` | 10-15 s | Slides 14-15 | Realistic, X-Ray, Solid, Thermal con leyenda | Alta |
| `gif_04_mobile_microinteractions.mp4` | 8-12 s | Slide 22 o respaldo | Tutorial, bottom sheet, gesto tactil o correccion formativa | Media |
| `vid_backup_full_demo.mp4` | 3-5 min | Respaldo | Recorrido completo sin cortes para contingencia | Alta |

El video principal debe ser transparente: si se usa video, se dice que es respaldo grabado de la build real. La microdemo en vivo solo se ejecuta si el equipo, navegador y red estan estables.

## 8. Imagenes y diagramas base

Reutilizables desde `Informe_final/figures/screenshots_contextual/`:

- `fig_ui_hero_mobile_pc.png`
- `fig_ui_explore_mobile_pc.png`
- `fig_ui_info_panel.png`
- `fig_explore_isolate_sequence.png`
- `fig_analyze_tool_outputs.png`
- `fig_modes_direct_xray_solid_thermal.png`
- `fig_modes_studio_presets.png`
- `fig_thermal_single.png`
- `fig_profiler_internal_evidence.png`
- `fig_render_optimized_drone_stack.png`
- `fig_cad_bake_low_pair.png`
- `fig_cad_bake_high_pair.png`
- `fig_device_matrix_clean.png`

Diagramas que conviene rehacer para pantalla, aunque existan en el informe:

- Problema: documentacion plana -> reconstruccion mental -> inspeccion interactiva.
- Frontera de alcance: visual product twin vs digital twin operacional.
- Metodologia: DSR aplicada, build, medicion tecnica, usuarios.
- Arquitectura runtime: UI Toolkit, managers, datos, escena, shaders, profiler.
- Pipeline 3D: CAD / Blender / bake / FBX / Unity / WebGL.
- Triangulacion de evidencia: tecnicos, tareas, SUS, NASA-TLX Raw, Think-Aloud.

## 9. Slides de respaldo para preguntas

No entran en la ruta principal, pero deben existir al final del deck:

| Backup | Tema | Para responder |
|---:|---|---|
| B1 | Formula SUS y lectura de 68 | Por que SUS no compara 3D vs 2D |
| B2 | Formula NASA-TLX Raw | Por que se usa Raw TLX sin ponderacion pareada |
| B3 | Variables de control | Dispositivo, navegador, resolucion, cache, orden AB/BA |
| B4 | Tabla tecnica completa de rendimiento | FPS, frame time, memoria por dispositivo |
| B5 | Profiler interno y export JSON/CSV | Trazabilidad de mediciones tecnicas |
| B6 | 95 617 vs 229 054 triangulos | Diferencia entre activo base y escena runtime |
| B7 | Arquitectura ampliada | Managers, eventos, datos y shaders |
| B8 | Sistema termico heuristico | Por que no es FEA ni telemetria |
| B9 | Limitaciones metodologicas | Muestra, efecto techo, no inferencia estadistica |
| B10 | Trabajo futuro por fases | Telemetria historica, calibracion, pruebas ampliadas |

## 10. Corte si falta tiempo

Si la defensa se atrasa, cortar en este orden:

1. Reducir slide 8 y 9 a una sola explicacion del pipeline.
2. Reducir slide 13 y 14 a una sola captura compuesta.
3. Usar solo video, sin microdemo en vivo.
4. Mover detalles de profiler y geometria a backup.
5. Mantener siempre problema, alcance, metodologia, resultados, limites y contribucion.

## 11. Control de coherencia antes de generar slides

- No decir que es digital twin operacional.
- No prometer telemetria real, FEA, mantenimiento predictivo ni compatibilidad universal.
- No vender herramientas ocultas o no integradas como UI final.
- No presentar T4 como tarea cronometrada.
- No usar SUS como comparacion 3D vs 2D.
- No interpretar NASA-TLX como medicion directa de carga intrinseca, extrinseca y germana.
- No comparar directamente 95 617 y 229 054 triangulos.
- No inventar FPS, tiempos, pesos de build ni reducciones no trazadas.

## 12. Entregable siguiente

El siguiente paso no es escribir el guion completo. Primero se deben producir o seleccionar los medios visuales indicados en `ASSETS_REQUIREMENTS.md`; luego se construye el deck con titulos-sentencia y, solo despues, se redactan tarjetas orales por slide.
