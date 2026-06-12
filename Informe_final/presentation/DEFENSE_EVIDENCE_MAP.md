# Mapa de evidencias de sustentacion - TwinSight X500

Estado: canonico para enlazar informe, app, documentacion, presentacion y preguntas.
Fecha de actualizacion: 2026-06-11.
Fuente autoritativa: `Informe_final/informe_final.pdf`.

## 1. Jerarquia de autoridad

Cuando exista conflicto entre fuentes, usar este orden:

1. `Informe_final/informe_final.pdf` y fuentes LaTeX en `Informe_final/chapters/`.
2. Anexos, validacion, manuales y figuras bajo `Informe_final/`.
3. App publica y documentacion en `docs/`.
4. README publico.
5. Documentacion tecnica interna versionada, solo si no contradice el informe.
6. Notas de estudio locales o historicas, solo como apoyo personal, no como fuente publica.

Regla para defensa:

> Si una cifra, feature o afirmacion no aparece en informe, anexo, build, manual, profiler o README publico vigente, no se afirma como resultado cerrado.

## 2. Rutas maestras

| Tipo | Ruta | Uso en defensa |
|---|---|---|
| Informe final PDF | `Informe_final/informe_final.pdf` | Fuente academica principal. |
| Introduccion, problema, alcance | `Informe_final/chapters/01_introduccion.tex` | Problema, preguntas, objetivos, alcance, visual product twin. |
| Metodologia | `Informe_final/chapters/03_marco_metodologico.tex` | DSR, muestra, variables, instrumentos, tareas, etica. |
| Desarrollo | `Informe_final/chapters/04_desarrollo.tex` | Pipeline, taxonomia, UI, arquitectura, shaders, tooling. |
| Resultados | `Informe_final/chapters/05_resultados.tex` | Rendimiento, SUS, NASA-TLX Raw, tiempos, Think-Aloud, discusion. |
| Conclusiones | `Informe_final/chapters/06_conclusiones.tex` | Contribuciones, limitaciones, trabajo futuro. |
| Referencias | `Informe_final/chapters/07_referencias.tex` | Bibliografia formal del informe. |
| Apendices | `Informe_final/chapters/08_apendices.tex` | Indice de anexos y rutas. |
| Validacion tecnica | `Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md` | Procedimiento de profiler y mediciones WebGL. |
| Tablas rendimiento | `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex` | FPS, frame time, memoria, triangulos runtime. |
| Manual usuario | `Informe_final/Manual_de_usuario/manual_usuario.pdf` | Flujo visible y uso de app. |
| Manual tecnico | `Informe_final/Manual_tecnico/manual_tecnico.pdf` | Arquitectura, build y decisiones tecnicas. |
| Demo publica | `docs/index.html` | Landing/demo publicada por GitHub Pages. |
| Build publica | `docs/Build/` | Artefactos WebGL publicados. |
| Manuales publicos | `docs/manuals/` | Copias publicas del informe/manuales. |
| README | `README.md` | Limite publico: visible_ui, oculto, legacy, futuro. |
| Guion oral | `Informe_final/presentation/PRESENTATION_SCRIPT.md` | Tarjetas orales slide por slide. |
| Demo | `Informe_final/presentation/DEMO_SCRIPT.md` | Recorrido de demostracion. |
| Assets | `Informe_final/presentation/ASSETS_REQUIREMENTS.md` | Videos, GIFs y capturas requeridas. |
| Preguntas | `Informe_final/presentation/JURY_QA_BANK.md` | Banco de respuestas para jurado. |

## 3. Mapa slide -> evidencia

| Slide | Claim defendido | Evidencia documental | Evidencia visual/app | Riesgo que controla |
|---:|---|---|---|---|
| 1 | TwinSight X500 es prototipo WebGL inspeccionable. | `01_introduccion.tex`; `README.md`. | Hero o captura de app. | Evita abrir como pitch comercial. |
| 2 | El reto es comprender relaciones, no solo ver piezas. | `01_introduccion.tex`, definicion de hardware complejo. | Comparacion manual/CAD/app. | No desacreditar documentacion 2D. |
| 3 | La brecha esta entre fidelidad, legibilidad y rendimiento. | `01_introduccion.tex`, planteamiento del problema. | Diagrama de trade-off. | Evita justificar solo estetica. |
| 4 | Es visual product twin, no digital twin operacional. | `01_introduccion.tex`; `03_marco_metodologico.tex`; `README.md`. | Frontera incluido/excluido. | Principal riesgo de sobrepromesa. |
| 5 | Objetivos cubren pipeline, modos, prototipo y evaluacion. | Objetivos en `01_introduccion.tex`. | Matriz OE1-OE4. | Evita objetivos sin resultado. |
| 6 | DSR y validacion descriptiva son adecuados al artefacto. | `03_marco_metodologico.tex`, DSRM y enfoque. | Ciclo DSR aplicado. | Evita pedir inferencia experimental. |
| 7 | La evaluacion triangula tecnicos, tareas, SUS, NASA y Think-Aloud. | `03_marco_metodologico.tex`, instrumentos y triangulacion. | Diagrama de triangulacion. | Evita mal uso de SUS/NASA. |
| 8 | El pipeline traduce CAD a WebGL viable. | `04_desarrollo.tex`, pipeline 3D. | High/low mesh, bake, Blender/Unity. | Evita parecer importacion directa. |
| 9 | 95 617 y 229 054 triangulos no son equivalentes. | `05_resultados.tex`; `07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`. | Tarjeta de cifras. | Evita contradiccion geometrica. |
| 10 | La app separa UI, datos, escena, shaders y profiler. | `04_desarrollo.tex`; `Informe_final/Manual_tecnico/manual_tecnico.pdf`. | Diagrama de arquitectura. | Evita lectura de "viewer simple". |
| 11 | La taxonomia 28/30/257 estructura seleccion y escena. | `04_desarrollo.tex`, reconciliacion de conteos. | Jerarquia de piezas/anchors/renderers. | Evita confusion de conteos. |
| 12 | El flujo publico real es Hero -> Explore -> bottom sheet -> Inspect/Analyze/Studio. | `README.md`; `Informe_final/Manual_de_usuario/manual_usuario.pdf`; `DEMO_SCRIPT.md`. | Capturas de UI. | Evita vender modulos no visibles. |
| 13 | Inspect/Analyze reducen ruido visual para leer relaciones. | `05_resultados.tex`, cierre funcional y Think-Aloud. | Isolate, explode, cut, filtros. | Evita feature-list sin proposito. |
| 14 | Studio/shaders son lecturas visuales complementarias. | `04_desarrollo.tex`; figuras de modos. | Grid Realistic/X-Ray/Solid/Thermal. | Evita presentar efectos como decoracion. |
| 15 | Thermal es heuristico, no FEA ni telemetria. | `01_introduccion.tex`; `06_conclusiones.tex`. | Captura Thermal con etiqueta. | Riesgo tecnico alto si se sobrevende. |
| 16 | La demo prueba tres capacidades concretas. | `DEMO_SCRIPT.md`; `ASSETS_REQUIREMENTS.md`. | Slide "explorar, inspeccionar, analizar". | Evita demo improvisada. |
| 17 | La build realiza el flujo prometido. | `docs/Build/`; `Informe_final/Manual_de_usuario/manual_usuario.pdf`; `DEMO_SCRIPT.md`. | Video o demo vivo. | Evita depender de red o azar. |
| 18 | El profiler hace trazable rendimiento por escenario. | `06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`; tablas de rendimiento. | Profiler screenshot/export. | Evita FPS sin contexto. |
| 19 | Rendimiento viable con limites por dispositivo. | `05_resultados.tex`; `07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`. | Matriz de dispositivos. | Evita promesa universal movil. |
| 20 | SUS muestra recepcion favorable del prototipo 3D. | `05_resultados.tex`, resultados SUS. | Grafico SUS. | Evita comparar SUS 3D vs 2D. |
| 21 | NASA-TLX Raw y tiempos favorecen 3D descriptivamente. | `05_resultados.tex`, NASA y tiempos T1-T3. | Comparativa 3D/2D. | Evita inferencia estadistica. |
| 22 | Discusion acota efecto techo, muestra y compatibilidad. | `05_resultados.tex`, discusion; `06_conclusiones.tex`. | Matriz demuestra/no demuestra. | Controla objeciones metodologicas. |
| 23 | Contribucion tecnica, metodologica y comunicativa. | `06_conclusiones.tex`; `README.md`. | Tres columnas. | Evita reducir aporte a app visual. |
| 24 | Aporte: hacer legibles relaciones de hardware complejo. | `06_conclusiones.tex`; informe completo. | Dron final + takeaways. | Cierre academico, no comercial. |

## 4. Claims numericos autorizados

| Claim | Valor autorizado | Fuente | Como decirlo |
|---|---:|---|---|
| Participantes | 12 anonimizados | `03_marco_metodologico.tex`; `05_resultados.tex` | "Muestra no probabilistica formativa de 12 participantes." |
| Registros tarea-condicion | 96 | `05_resultados.tex` | "Cuatro tareas por condicion, ambas condiciones, 12 participantes." |
| Tareas completadas | 12/12 en T1-T4 por condicion | `05_resultados.tex` | "Hubo efecto techo en completitud." |
| T4 | No cronometrada | `05_resultados.tex` | "T4 fue exploratoria guiada." |
| T1 3D vs 2D | 5,75 s vs 13,00 s | `05_resultados.tex` | "Tiempo medio descriptivo." |
| T2 3D vs 2D | 3,50 s vs 18,00 s | `05_resultados.tex` | "Tiempo medio descriptivo." |
| T3 3D vs 2D | 11,33 s vs 23,00 s | `05_resultados.tex` | "Tiempo medio descriptivo." |
| Total T1-T3 | 20,58 s vs 54,00 s | `05_resultados.tex` | "Diferencia media acumulada de 33,42 s." |
| SUS promedio | 91,88 | `05_resultados.tex` | "SUS solo del prototipo 3D." |
| SUS mediana | 95,00 | `05_resultados.tex` | "Lectura descriptiva." |
| SUS minimo/maximo | 60,00 / 100,00 | `05_resultados.tex` | "Muestra pequena y no probabilistica." |
| NASA 3D promedio | 8,69 | `05_resultados.tex` | "NASA-TLX Raw, workload percibido." |
| NASA 2D promedio | 19,89 | `05_resultados.tex` | "Referencia 2D por condicion." |
| NASA diferencia | 11,19 | `05_resultados.tex` | "Diferencia pareada descriptiva 2D-3D." |
| Casos NASA menor en 3D | 12/12 | `05_resultados.tex` | "Tendencia consistente en esta muestra." |
| Activo base optimizado | 95 617 triangulos | `05_resultados.tex` | "Activo base, no runtime total." |
| Escena runtime profiler | 229 054 triangulos estimados | `07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex` | "Escena instrumentada, no equivalente al activo base." |
| Taxonomia | 28 piezas canonicas | `04_desarrollo.tex` | "Nivel semantico." |
| Anchors | 30 anchors | `04_desarrollo.tex` | "Nivel operativo de escena." |
| Renderers/colliders | 257 | `04_desarrollo.tex` | "Fragmentacion geometrica/runtime." |

## 5. Evidencia por frente

### 5.1 Problema y marco conceptual

Usar para:

- justificar necesidad de pasar de informacion plana a comprension espacial;
- explicar hardware complejo;
- delimitar visual product twin.

Fuentes:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/figures/chapter1/fig_1_fragmentacion_hardware_complejo.pdf`
- `Informe_final/figures/chapter1/fig_1_alcance_visual_product_twin.pdf`

Respuesta corta:

> La tesis no nace porque el manual 2D sea inutil, sino porque el ensamblaje exige reconstruccion espacial que una interfaz 3D puede externalizar parcialmente.

### 5.2 Metodologia

Usar para:

- defender DSR;
- explicar muestra;
- aclarar instrumentos;
- responder por inferencia estadistica.

Fuentes:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/figures/chapter3/fig_3_dsrm_aplicado_proyecto.pdf`
- `Informe_final/figures/chapter3/fig_3_triangulacion_evidencia.pdf`

Respuesta corta:

> La investigacion es aplicada y formativa. Construye y evalua un artefacto; por eso los resultados son descriptivos y trazados, no inferenciales poblacionales.

### 5.3 Implementacion tecnica

Usar para:

- pipeline CAD/WebGL;
- arquitectura;
- taxonomia;
- UI visible;
- shaders y Thermal.

Fuentes:

- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/Manual_tecnico/manual_tecnico.pdf`
- `README.md`
- `docs/Build/`

Respuesta corta:

> El aporte tecnico no es una escena aislada, sino una cadena CAD/Blender/Unity/WebGL con taxonomia, UI, shaders, datos y medicion runtime.

### 5.4 Resultados tecnicos

Usar para:

- FPS;
- frame time;
- memoria;
- compatibilidad;
- triangulos;
- profiler.

Fuentes:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`
- `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`
- `Telemetria/Mediciones_WebGL/`

Respuesta corta:

> El rendimiento debe leerse por dispositivo, navegador, resolucion y escenario. La tesis no promete compatibilidad universal.

### 5.5 Resultados con usuarios

Usar para:

- SUS;
- NASA-TLX Raw;
- tiempos;
- Think-Aloud;
- efecto techo;
- T4.

Fuentes:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/validacion/`
- `docs/manuals/` para copias publicas de instrumentos cuando aplique.

Respuesta corta:

> La evidencia de usuario favorece al 3D en workload y tiempos descriptivos, pero no se interpreta como prueba causal fuerte por el tamano y tipo de muestra.

## 6. Bibliografia por funcion defensiva

No memorizar referencias completas; citar autores o familias cuando aporten criterio.

| Tema | Referencias del informe | Para que sirven en defensa |
|---|---|---|
| Design Science Research | Hevner et al.; Peffers et al. | Justificar construir y evaluar un artefacto. |
| Carga cognitiva | Sweller; Hegarty y Waller | Explicar reconstruccion mental y esfuerzo espacial. |
| HCI/UX 3D | Bowman et al.; Norman; Ware; Darken y Sibert | Justificar navegacion, seleccion, feedback y legibilidad. |
| SUS | Brooke; Bangor et al.; Sauro y Lewis | Explicar calculo, lectura descriptiva y referencia 68. |
| NASA-TLX | Hart y Staveland; Hart | Explicar Raw TLX, subescalas y workload percibido. |
| Think-Aloud | Ericsson y Simon | Justificar verbalizacion concurrente y codificacion cualitativa. |
| Digital twin | Kritzinger et al.; Digital Twin Consortium; Lin et al.; Jones et al. | Delimitar visual product twin vs digital shadow/twin operacional. |
| WebGL/WebAssembly/Unity | Unity Technologies; WebAssembly Community Group; fuentes WebGL | Justificar restricciones de build web y runtime. |
| Documentacion Holybro | Holybro | Soporte del caso de estudio y condicion 2D. |

Ruta formal:

- `Informe_final/chapters/07_referencias.tex`

## 7. Slides de respaldo y preguntas que cubren

| Backup | Evidencia | Preguntas que responde |
|---|---|---|
| B1 SUS | `03_marco_metodologico.tex`; `05_resultados.tex` | Por que SUS solo 3D; que significa 68; como se calcula. |
| B2 NASA | `03_marco_metodologico.tex`; `05_resultados.tex` | Raw TLX, rendimiento invertido, no carga cognitiva directa. |
| B3 Variables de control | `03_marco_metodologico.tex`; `05_resultados.tex` | Dispositivo, navegador, cache, orden AB/BA, build. |
| B4 Rendimiento completo | `07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex` | FPS, frame time, memoria, escenarios. |
| B5 Profiler | `06_GUIA_MEDICIONES_TECNICAS_WEBGL.md` | Como se obtuvo JSON/CSV y que registra. |
| B6 Geometria | `05_resultados.tex`; `04_desarrollo.tex` | 95 617 vs 229 054, 28/30/257. |
| B7 Arquitectura | `Informe_final/Manual_tecnico/manual_tecnico.pdf`; `04_desarrollo.tex` | Managers, eventos, datos, shaders. |
| B8 Thermal | `01_introduccion.tex`; `06_conclusiones.tex` | Heuristico, no FEA, trabajo futuro. |
| B9 Limitaciones | `05_resultados.tex`; `06_conclusiones.tex` | Muestra, efecto techo, no inferencia, movil. |
| B10 Futuro | `06_conclusiones.tex` | Telemetria, twin manifest, mayor muestra, dispositivos. |

## 8. Claims prohibidos o condicionados

| Claim | Estado | Forma segura |
|---|---|---|
| "TwinSight es un digital twin completo" | Prohibido | "Visual product twin; base visual-semantica previa." |
| "Thermal simula fisica real" | Prohibido | "Visualizacion heuristica relativa por componentes." |
| "Funciona en cualquier movil" | Prohibido | "Funciona en dispositivos evaluados, con limites." |
| "SUS demuestra que 3D supera a 2D" | Prohibido | "SUS describe usabilidad del prototipo 3D." |
| "NASA mide carga cognitiva directamente" | Prohibido | "NASA-TLX Raw mide carga de trabajo percibida." |
| "T4 fue cronometrada" | Prohibido | "T4 fue exploratoria guiada y no cronometrada." |
| "95 617 es todo el runtime" | Prohibido | "95 617 es el activo base optimizado." |
| "README historico define el alcance final" | Prohibido | "El informe final es fuente academica autoritativa." |
| "Modulos ocultos son features publicas" | Prohibido | "Visible, oculto, legacy y futuro se diferencian." |

## 9. Checklist antes de cerrar deck

- Cada slide tiene un claim, no un titulo generico.
- Cada claim tiene al menos una evidencia en informe, anexo, app o README.
- Las cifras coinciden con capitulo 5 y anexos.
- El video de demo no muestra Measurement, BOM, annotations ni modulos no publicados.
- La slide de Thermal dice "heuristico".
- La slide de rendimiento dice "por dispositivo" y "no universal".
- La slide de SUS dice "solo prototipo 3D".
- La slide de NASA dice "Raw TLX" y "descriptivo".
- La slide de tareas dice "T4 no cronometrada".
- El cierre repite "visual product twin".
