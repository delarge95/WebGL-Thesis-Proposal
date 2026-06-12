# Guia robusta de estudio para sustentacion - TwinSight X500

Estado: sistema de estudio conectado a informe, anexos, app y presentacion.
Fecha: 2026-06-12.
Tiempo objetivo de defensa: 28:30 min.
Fuente academica autoritativa: `Informe_final/informe_final.pdf`.

## Como usar esta guia

1. Leer primero la tarjeta oral en `SPEAKER_CARDS.md`.
2. Revisar el bloque correspondiente del guion maestro en `PRESENTATION_SCRIPT.md`.
3. Verificar el claim en `DEFENSE_EVIDENCE_MAP.md`.
4. Buscar la fuente teorica en `BIBLIOGRAPHY_EVIDENCE_ATLAS.md`.
5. Ensayar preguntas con `JURY_QA_BANK.md`.

Esta guia no debe llevarte a sobre-explicar durante la sustentacion. Sirve para tener profundidad cuando el jurado pregunte.

## 1. Problema, brecha y propuesta

Slides: 1-4.

**Que debes poder explicar**

El problema no es que no existan planos, CAD o documentacion tecnica. El problema defendible es que esa informacion queda fragmentada y exige reconstruccion espacial, especialmente cuando el usuario necesita entender relaciones entre componentes, subensambles y funciones.

**En palabras sencillas**

El dron tiene mucha informacion, pero esta repartida. TwinSight la junta en una experiencia 3D web para que una persona pueda ver, tocar, aislar y entender piezas sin abrir herramientas CAD pesadas.

**Explicacion tecnica**

TwinSight X500 se formula como `visual product twin`: una representacion visual interactiva y semantizada del producto. No recibe telemetria, no ejecuta simulacion fisica y no replica estado operacional. Su aporte esta en la articulacion entre geometria optimizada, taxonomia de piezas, interfaz de inspeccion, modos visuales y medicion WebGL.

**Evidencia directa**

- Informe: `Informe_final/chapters/01_introduccion.tex`.
- Alcance: `Informe_final/chapters/01_introduccion.tex`, seccion de alcance y limitaciones.
- App/documentacion publica: `README.md`.
- Mapa de claims: `Informe_final/presentation/DEFENSE_EVIDENCE_MAP.md`.

**Bibliografia ancla**

- Sweller et al. (2019), PDF p. 2: memoria de trabajo limitada.
- Norman (2013), PDF p. 15: significadores de interfaz.
- Yu et al. (2023), PDF p. 2: integracion de visualizacion 3D en web.

**Si el jurado pregunta "por que esto no es solo un visor 3D?"**

Respuesta corta: porque el sistema no solo renderiza un modelo; estructura piezas, categorias, datos, UI, modos visuales, interacciones y medicion tecnica para apoyar inspeccion.

**Pregunta rara**

"Si el usuario ya sabe leer planos, para que sirve el 3D?"

Respuesta: no reemplaza la lectura tecnica experta; reduce friccion de ubicacion, relacion espacial y consulta contextual en tareas concretas. El soporte 2D sigue siendo referencia valida.

## 2. Objetivos y cierre por evidencia

Slide: 5.

**Que debes poder explicar**

Cada objetivo especifico debe cerrar con una evidencia visible: modelo optimizado, entorno interactivo, validacion formativa y documentacion/anexos trazables.

**En palabras sencillas**

No basta decir "se hizo una app". Hay que mostrar donde quedo cada parte prometida.

**Explicacion tecnica**

La defensa debe leer los objetivos como un contrato verificable. OE1 se evidencia en el pipeline CAD-WebGL y la reduccion/organizacion geometrica. OE2 en la implementacion Unity WebGL, UI, modos y build. OE3 en KPIs tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud. OE4 en informe, manual tecnico, manual de usuario, anexos y repositorio.

**Evidencia directa**

- Objetivos: `Informe_final/chapters/01_introduccion.tex`.
- Desarrollo: `Informe_final/chapters/04_desarrollo.tex`.
- Resultados: `Informe_final/chapters/05_resultados.tex`.
- Manual tecnico: `Informe_final/Manual_tecnico/manual_tecnico.pdf`.
- Manual usuario: `Informe_final/Manual_de_usuario/manual_usuario.pdf`.

**Bibliografia ancla**

- Peffers et al. (2007), PDF p. 2: DSRM.

**Pregunta rara**

"Cual objetivo quedaria mas debil si se cae la demo en vivo?"

Respuesta: OE2 quedaria mas expuesto oralmente, pero no cae si se muestra video/GIF, manuales, capturas, build versionada y evidencia tecnica. La demo viva es apoyo, no unica prueba.

## 3. Metodologia DSR y diseno comparativo

Slides: 6-7.

**Que debes poder explicar**

El proyecto es de desarrollo de artefacto con evaluacion formativa. El diseno comparativo 3D vs 2D es intra-sujeto, con orden AB/BA, tareas equivalentes y lectura descriptiva.

**En palabras sencillas**

Se construyo una solucion, se probo con usuarios y se comparo con un soporte 2D para ver si el 3D hacia mas facil ciertas tareas. No se esta diciendo que esto prueba algo para toda la poblacion.

**Explicacion tecnica**

La muestra final fue de 12 participantes anonimizados, no probabilistica, entre el 20 de mayo y el 3 de junio de 2026. La exposicion se contrabalanceo por alternancia simple: codigos impares AB y codigos pares BA. Se registraron variables de control: codigo anonimo, perfil agregado, experiencia, condicion 3D/2D, orden, dispositivo, sistema operativo, navegador, resolucion, build, cache y archivos de profiler cuando aplicaba.

**Evidencia directa**

- Metodologia: `Informe_final/chapters/03_marco_metodologico.tex`.
- Validacion: `Informe_final/validacion/00_README_VALIDACION_USUARIOS.md`.
- Instrumentos: `Informe_final/validacion/03_CUESTIONARIO_SUS_PARTICIPANTE.md`, `04_CUESTIONARIO_NASA_TLX_PARTICIPANTE.md`, `05_FORMATO_REGISTRO_MODERADOR.md`.

**Bibliografia ancla**

- Peffers et al. (2007), PDF p. 2.
- Brooke (1996), PDF p. 3.
- Hart y Staveland (1988), PDF p. 7.

**Pregunta rara**

"Por que alternancia AB/BA y no aleatorizacion completa?"

Respuesta: por escala formativa y control operativo de la sesion. La alternancia simple evita que todos pasen por el mismo orden y reduce sesgo de aprendizaje/fatiga, sin presentar el estudio como ensayo experimental inferencial.

## 4. Pipeline CAD, optimizacion y taxonomia

Slides: 8-9.

**Que debes poder explicar**

El pipeline transforma un activo CAD pesado en una escena WebGL navegable, organizada y consultable. La taxonomia 28/30/257 sirve para operar la escena, no para afirmar un inventario fisico absoluto.

**En palabras sencillas**

El CAD original no estaba listo para navegador. Hubo que limpiarlo, organizarlo, nombrarlo y prepararlo para que Unity lo pudiera mostrar de forma fluida.

**Explicacion tecnica**

El flujo conecta CAD, Blender/MoI3D/STEPper cuando aplica, optimizacion geometrica, jerarquia semantica, Unity WebGL, shaders y profiler. La defensa debe distinguir activo base optimizado y escena runtime instrumentada. La cifra 95.617 triangulos corresponde al activo base optimizado exportado; 229.054 triangulos corresponde al conteo runtime instrumentado/profiler. No son metricas equivalentes.

**Evidencia directa**

- Desarrollo: `Informe_final/chapters/04_desarrollo.tex`.
- Resultados/rendimiento: `Informe_final/chapters/05_resultados.tex`.
- Tablas WebGL: `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`.
- Manual tecnico: `Informe_final/Manual_tecnico/manual_tecnico.pdf`.

**Bibliografia ancla**

- Yu et al. (2023), PDF p. 2.
- Khronos Group (2011), HTML abstract.

**Pregunta rara**

"Si 229.054 es mayor que 95.617, entonces no optimizaron?"

Respuesta: no. Son capas distintas. 95.617 describe el activo base optimizado; 229.054 describe una escena runtime instrumentada con instancias, proxies, assets de apoyo y renderers adicionales. Compararlas como si fueran la misma metrica seria metodologicamente incorrecto.

## 5. Arquitectura, UI y microinteracciones

Slides: 10-12.

**Que debes poder explicar**

La app separa responsabilidades: datos, escena, seleccion, UI, modos visuales, medicion y build. La UI no es decorativa; canaliza la comprension con Hero, Explore, bottom sheet, Inspect, Analyze y Studio.

**En palabras sencillas**

El usuario no recibe todos los controles de golpe. La app le va dando herramientas segun lo que necesita hacer: mirar, seleccionar, entender, analizar o cambiar el modo visual.

**Explicacion tecnica**

Las microinteracciones finales salen de observacion formativa: tutorial deslizable/cerrable, area tactil del panel protegida, sensibilidad de zoom/orbita ajustada, doble toque estable, navegacion con Atrás por capas y confirmacion de salida. Son decisiones UX que reducen friccion operacional.

**Evidencia directa**

- Desarrollo UI: `Informe_final/chapters/04_desarrollo.tex`.
- Resultados formativos: `Informe_final/chapters/05_resultados.tex`.
- Manual usuario: `Informe_final/Manual_de_usuario/manual_usuario.pdf`.
- Demo: `Informe_final/presentation/DEMO_SCRIPT.md`.

**Bibliografia ancla**

- Norman (2013), PDF p. 15.
- Brooke (1996), PDF p. 3, para lectura posterior de usabilidad percibida.

**Pregunta rara**

"Por que no mostrar todos los modos y botones si existen en codigo?"

Respuesta: porque alcance implementado y alcance publicado no son lo mismo. Lo defendible es la UI final visible y documentada. Capacidades ocultas o legacy pueden existir por trazabilidad tecnica, pero no deben venderse como flujo publico.

## 6. Modos visuales, Analyze y Thermal

Slides: 13-15.

**Que debes poder explicar**

Cada modo visual responde una pregunta distinta: Realistic orienta, X-Ray permite lectura interna, Solid reduce ruido material, Explode separa relaciones, Cut muestra seccion, Thermal comunica tendencia relativa.

**En palabras sencillas**

No son filtros bonitos. Son lentes para hacer preguntas diferentes sobre el dron.

**Explicacion tecnica**

Thermal debe explicarse como sistema heuristico relativo. No hay sensores, telemetria, solucion FEA ni temperatura real. La visualizacion usa jerarquias o pesos de interpretacion para comunicar zonas relativas de interes. Blueprint solo se muestra si aparece en la build evaluada; si no, se trata como preset/lectura no principal o capacidad no publicada.

**Evidencia directa**

- Shaders y entornos: `Informe_final/chapters/04_desarrollo.tex`.
- Sistema termico: `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`.
- Manual tecnico: `Informe_final/Manual_tecnico/manual_tecnico.pdf`.
- Demo: `Informe_final/presentation/DEMO_SCRIPT.md`.

**Bibliografia ancla**

- Khronos Group (2011), WebGL.
- Yu et al. (2023), Web3D.
- Bowman et al. (2004), como apoyo conceptual sin cita textual local verificada.

**Pregunta rara**

"Si Thermal no es fisico, por que incluirlo?"

Respuesta: porque su proposito no es diagnostico termico real, sino lectura visual exploratoria. Ayuda a comunicar una jerarquia relativa dentro del prototipo y queda explicitamente limitada para no sobreprometer.

## 7. Demo y evidencia funcional

Slides: 16-17.

**Que debes poder explicar**

La demo debe mostrar solo el flujo publico y replicable. Debe existir video/GIF de respaldo y una ruta de emergencia si el navegador, la red o la carga fallan.

**En palabras sencillas**

La demo no es improvisacion. Es una prueba guiada de que lo prometido funciona.

**Explicacion tecnica**

El recorrido debe pasar por carga/cache, Explore, seleccion de pieza, bottom sheet, aislamiento, modo visual, herramienta Analyze y retorno al estado base. No se deben activar carpetas, datos personales ni capacidades ocultas. Si se usa build local/publica, debe coincidir con la rama `master` y los artefactos documentados.

**Evidencia directa**

- Demo script: `Informe_final/presentation/DEMO_SCRIPT.md`.
- Requisitos de assets: `Informe_final/presentation/ASSETS_REQUIREMENTS.md`.
- Build/docs: `docs/Build/`, `README.md`.
- Manual usuario: `Informe_final/Manual_de_usuario/manual_usuario.pdf`.

**Pregunta rara**

"Si la demo en vivo falla, invalida el proyecto?"

Respuesta: no, si se tienen artefactos de respaldo: capturas, video/GIF, build documentada, manuales, profiler y anexos. La defensa debe reconocer el fallo tecnico y pasar al respaldo sin improvisar claims.

## 8. Rendimiento, profiler y compatibilidad

Slides: 18-19.

**Que debes poder explicar**

El rendimiento se reporta por dispositivo, navegador, resolucion, cache y escenario. El escritorio es estable; los moviles tienen comportamiento acotado. La compatibilidad no es universal.

**En palabras sencillas**

La app funciona bien en el equipo de escritorio probado y es usable en moviles reales evaluados, pero los celulares mas limitados pueden tener picos o bajar de 30 FPS.

**Explicacion tecnica**

Entorno de escritorio: Intel Core i7-5820K, GTX 980 Ti, 48 GB RAM, viewport 1910x903, Windows 11, Chrome 141. Entorno movil de usuarios: Redmi Note 10S, MIUI Global 14.0.11, Chrome 148, cache cargada. En tablas tecnicas: escritorio 59,8 FPS promedio y 16,7 ms; Redmi Note 10S 26,5 FPS y 40,3 ms; Android Adreno 610 puede bajar a 17,6 FPS y 61,4 ms como limite inferior.

**Evidencia directa**

- Resultados: `Informe_final/chapters/05_resultados.tex`.
- Tablas WebGL: `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`.
- Profiler y mediciones: `Telemetria/Mediciones_WebGL/` si esta disponible en el repo local.

**Bibliografia ancla**

- Khronos Group (2011), WebGL.
- Unity/ WebGL: fuentes registradas en `Investigacion/Notas_Conceptuales/Fuentes/SRC_unity2024memory.md` y `SRC_unity2024webglgettingstarted.md`, sin cita textual local verificada.

**Pregunta rara**

"Por que medir con cache cargada si un usuario real entra por primera vez?"

Respuesta: porque el objetivo reportado es comportamiento operativo de la build durante evaluacion, no benchmark de primera carga fria. La condicion de cache se declara para que el dato sea reproducible y no se extrapole indebidamente.

## 9. Evaluacion con usuarios: tareas, SUS, NASA-TLX Raw

Slides: 20-21.

**Que debes poder explicar**

Las cuatro tareas se completaron en ambas condiciones. Esto genera efecto techo en exito, por lo que la diferencia relevante se analiza en tiempos T1-T3, NASA-TLX Raw y verbalizaciones.

**En palabras sencillas**

Todos pudieron terminar las tareas con 3D y con 2D. La diferencia estuvo en que el 3D se percibio menos pesado y tomo menos tiempo en las tareas cronometradas.

**Explicacion tecnica**

T1, T2 y T3 se cronometraron porque tenian inicio/cierre comparables. T4 fue exploratoria guiada y no se cronometro. Resultados: T1 5,75 s vs 13,00 s; T2 3,50 s vs 18,00 s; T3 11,33 s vs 23,00 s; total T1-T3 20,58 s vs 54,00 s. SUS se aplico solo al 3D: media 91,88, mediana 95,00, DE 11,24. NASA-TLX Raw: 3D 8,69 y 2D 19,89, menor en los 12 casos.

**Evidencia directa**

- Resultados: `Informe_final/chapters/05_resultados.tex`.
- Validacion usuarios: `Informe_final/validacion/usuarios/` si esta disponible localmente.
- Instrumentos: `Informe_final/validacion/03_CUESTIONARIO_SUS_PARTICIPANTE.md`, `04_CUESTIONARIO_NASA_TLX_PARTICIPANTE.md`, `05_FORMATO_REGISTRO_MODERADOR.md`.

**Bibliografia ancla**

- Brooke (1996), SUS.
- Bangor et al. (2009), interpretacion SUS.
- Hart y Staveland (1988), NASA-TLX.
- Hart (2006), NASA-TLX 20 years later.

**Pregunta rara**

"Si todos completaron ambas condiciones, como pueden decir que 3D fue mejor?"

Respuesta: no se dice que fue mejor en exito, porque hubo efecto techo. Se dice que, dentro de esta muestra y tareas, el 3D mostro menor tiempo medio en T1-T3, menor workload percibido y comentarios cualitativos de mejor orientacion espacial.

## 10. Think-Aloud, triangulacion y discusion

Slide: 22.

**Que debes poder explicar**

Think-Aloud no es un adorno cualitativo: explica por que los datos cuantitativos se comportan como se comportan y que fricciones siguen abiertas.

**En palabras sencillas**

Ademas de medir, se escucho que pensaban los usuarios mientras usaban la app. Eso ayudo a entender que funcionaba y que todavia molestaba.

**Explicacion tecnica**

La categoria mas recurrente fue comprension espacial, presente en 11 de 12 participantes; navegacion/control aparecio en 10 de 12. Las fricciones se concentraron en iconos, navegacion movil y seleccion de piezas pequenas. La triangulacion cruza exito, tiempos, SUS, NASA-TLX Raw, Think-Aloud y observacion.

**Evidencia directa**

- Resultados cualitativos: `Informe_final/chapters/05_resultados.tex`.
- Protocolo: `Informe_final/validacion/PROTOCOLO_THINK_ALOUD.md`.
- Guia tareas: `Informe_final/validacion/GUIA_TAREAS_VALIDACION.md`.

**Bibliografia ancla**

- Lazar et al. (2017), HCI, sin cita textual local verificada.
- Norman (2013), significadores.

**Pregunta rara**

"No hay sesgo porque los usuarios sabian que usted hizo la app?"

Respuesta: puede existir sesgo de cortesia o contexto moderado; por eso el informe no generaliza, usa lectura descriptiva y triangula con tiempos, workload, observacion y limites.

## 11. Conclusiones, limites y futuro

Slides: 23-24.

**Que debes poder explicar**

La conclusion debe ser mas estrecha que los datos: TwinSight es entregable como visual product twin WebGL, con rendimiento viable en entornos probados y validacion formativa favorable, no como sistema operacional universal.

**En palabras sencillas**

El proyecto logro que el dron se pueda entender mejor desde una experiencia web 3D, pero todavia no es un sistema conectado al dron real ni una herramienta industrial completa.

**Explicacion tecnica**

Limites: muestra no probabilistica, n=12, sin inferencia estadistica, T4 exploratoria no cronometrada, thermal heuristico, compatibilidad movil acotada, sin telemetria real, sin mantenimiento predictivo, sin FEA termico real. Trabajo futuro: twin manifest, telemetria historica, live digital shadow, modo de servicio, documentacion bilingue, mejoras de rendimiento y accesibilidad.

**Evidencia directa**

- Discusion: `Informe_final/chapters/05_resultados.tex`.
- Conclusiones: `Informe_final/chapters/06_conclusiones.tex`.
- Roadmap: `Informe_final/chapters/ANEXO_ROADMAP_VERSION_BILINGUE_ES_EN_2026-04-15.md`.

**Pregunta rara**

"Que cambiaria si esto fuera tesis de maestria?"

Respuesta: se exigiria mayor control experimental, muestra mas robusta, hipotesis formales, estadistica inferencial, validacion longitudinal o comparacion con usuarios expertos, y probablemente integracion operacional real o telemetria verificable.

## Preguntas rebuscadas para autoauditoria

| Pregunta | Respuesta defendible corta |
| --- | --- |
| Si el soporte 2D tambien tuvo 12/12, cual es el aporte empirico? | Menor tiempo en T1-T3, menor NASA-TLX Raw en 12/12 y verbalizaciones de comprension espacial; no mayor tasa de exito. |
| Por que T4 no tiene tiempo? | Porque es exploratoria guiada; no tenia cierre temporal comparable y se conserva como evidencia cualitativa. |
| Por que no usar SUS para comparar 3D y 2D? | SUS evalua usabilidad global de un sistema; aqui se aplico al prototipo 3D. La comparacion 3D/2D se apoya en tareas y NASA-TLX Raw. |
| NASA-TLX prueba carga cognitiva? | No directamente. Mide carga de trabajo percibida. Se interpreta junto a teoria de carga cognitiva, sin igualarlas. |
| El promedio 68 de SUS es aprobacion? | No. Es referencia historica/descriptiva; no umbral absoluto. |
| Thermal puede usarse para diagnostico? | No. Es heuristico relativo, no FEA ni sensorica real. |
| Blueprint es visible? | Solo se muestra si aparece en la build evaluada. No se promete como tarjeta directa si no esta publicado. |
| Por que WebGL y no app nativa? | Por acceso web, distribucion y pertinencia multimedia; el costo es optimizar y declarar limites de rendimiento. |
| El repo contradice el informe? | La defensa debe tomar el informe como fuente autoritativa; README y docs deben no sobreprometer funciones ocultas. |
| Hay datos personales en anexos? | La matriz usa codigos anonimos y perfiles agregados; cualquier archivo con datos identificables no debe publicarse. |

## Cierre de estudio

La frase final debe conservar alcance:

> TwinSight X500 no resuelve todo el ciclo operacional del dron; resuelve una parte concreta y verificable: hacer legible un ensamblaje complejo desde una experiencia WebGL trazable, con evidencia tecnica y formativa.
