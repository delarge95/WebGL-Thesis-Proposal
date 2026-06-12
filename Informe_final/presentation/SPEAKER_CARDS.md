# Tarjetas orales de sustentacion - TwinSight X500

Estado: operativo para ensayo cronometrado.
Fecha: 2026-06-12.
Duracion meta: 28:30 min + 1:30 min de margen.
Fuente canonica: `Informe_final/informe_final.pdf`.

## Regla de uso

Estas tarjetas son la version oral corta del guion maestro. Si el tiempo se aprieta, se priorizan estas frases sobre `PRESENTATION_SCRIPT.md`. Cada tarjeta debe poder decirse sin leer.

## Tarjetas

| Slide | Tiempo | Tesis oral | Evidencia que debe verse | Riesgo a evitar |
| --- | --- | --- | --- | --- |
| 1 | 0:00-0:45 | Este proyecto convierte documentacion tecnica y CAD pesado del X500 en una experiencia web 3D inspeccionable. No presento un gemelo digital operacional, sino un visual product twin defendible para comprension e inspeccion. | Hero/render final del dron. | Abrir diciendo que es telemetria o simulacion real. |
| 2 | 0:45-1:45 | El problema no es falta de informacion, sino fragmentacion: planos, CAD, tablas y piezas no siempre ayudan a entender relaciones espaciales rapidamente. | Comparacion CAD/documento/app. | Caricaturizar el 2D como inutil. |
| 3 | 1:45-2:45 | La brecha es multimedia: hace falta integrar representacion 3D, interaccion, contexto semantico y rendimiento web en un flujo usable. | Diagrama problema-brecha-propuesta. | Prometer aprendizaje general o transferencia estadistica. |
| 4 | 2:45-3:45 | La respuesta fue TwinSight X500: una capa WebGL para explorar, seleccionar, aislar, analizar y consultar piezas del ensamblaje. | Captura del flujo publico. | Llamarlo digital twin completo. |
| 5 | 3:45-4:45 | Los objetivos se cierran en cuatro frentes: modelo optimizado, visualizacion interactiva, validacion formativa y documentacion trazable. | Tabla objetivo-evidencia. | Dejar objetivos sin resultado asociado. |
| 6 | 4:45-6:00 | La metodologia es DSR aplicada: identificar problema, disenar artefacto, construirlo, evaluarlo y comunicarlo, con iteraciones tecnicas y de usuario. | Ciclo DSR aplicado. | Presentar la validacion como experimento confirmatorio. |
| 7 | 6:00-7:15 | El estudio compara 3D y soporte 2D en tareas equivalentes, con orden AB/BA, muestra no probabilistica de 12 participantes y lectura descriptiva. | Diseno intra-sujeto. | Decir que hay inferencia poblacional. |
| 8 | 7:15-8:30 | La primera decision tecnica fue domesticar el CAD: convertirlo en geometria legible y viable para runtime web sin perder lectura funcional. | Pipeline CAD-Blender-Unity-WebGL. | Vender optimizacion como solo reduccion de peso. |
| 9 | 8:30-9:45 | La taxonomia 28/30/257 organiza la escena por sistemas, categorias y partes; eso permite seleccion, filtros, informacion y lectura espacial. | Taxonomia/jerarquia de piezas. | Confundir taxonomia con conteo fisico definitivo del dron. |
| 10 | 9:45-10:45 | La arquitectura separa escena, datos, UI, modos visuales y medicion; por eso la app no es solo un visor, sino un artefacto inspeccionable. | Diagrama de managers. | Mencionar modulos no publicados como si fueran UI final. |
| 11 | 10:45-11:45 | La interfaz se organiza en Hero, Explore, bottom sheet, Inspect, Analyze y Studio para que el usuario no tenga que recordar todo a la vez. | Capturas de UI. | Explicar la UI como lista de botones. |
| 12 | 11:45-12:45 | Las microinteracciones corrigen fricciones observadas: gestos, cierre de tutorial, seleccion de piezas pequenas, doble toque y navegacion por capas. | Mapa de fixes. | Presentarlas como decoracion. |
| 13 | 12:45-13:45 | Inspect responde que pieza estoy viendo y que significa; Analyze responde como se relaciona con otras piezas. | Demo de seleccion/aislamiento. | Saltar demasiado rapido y perder trazabilidad. |
| 14 | 13:45-15:00 | X-Ray, Solid, Cut, Explode y presets visuales son herramientas de lectura: cada modo responde una pregunta distinta sobre el ensamblaje. | Secuencia de modos. | Prometer Blueprint si no aparece en la build evaluada. |
| 15 | 15:00-16:00 | Thermal es heuristico y relativo: ayuda a visualizar tendencia o foco de carga, pero no equivale a simulacion termica ni FEA. | Captura thermal + aviso de alcance. | Decir temperatura real. |
| 16 | 16:00-17:30 | En la demo debo mostrar el flujo publico completo, sin depender de funciones ocultas: cargar, explorar, seleccionar, aislar y consultar. | Video o microdemo. | Improvisar una ruta no ensayada. |
| 17 | 17:30-19:00 | El segundo bloque de demo muestra analisis: explode, cut, X-Ray/Solid/Thermal y regreso al estado base. | Video o GIF. | Quedar atrapado en una pieza o modo. |
| 18 | 19:00-20:30 | El rendimiento se midio por dispositivo, navegador, resolucion, cache y escenario. En escritorio fue estable; en movil hay compatibilidad acotada. | Tabla FPS/frame time/memoria. | Prometer compatibilidad universal. |
| 19 | 20:30-21:45 | 95.617 triangulos y 229.054 triangulos no contradicen nada: una cifra es activo base optimizado; la otra es escena runtime instrumentada. | Nota junto a tabla. | Comparar ambas cifras como si fueran equivalentes. |
| 20 | 21:45-23:15 | En usuarios, las cuatro tareas se completaron en ambas condiciones; por eso el hallazgo no es exito binario, sino tiempo, workload y comprension. | Tabla tareas + efecto techo. | Decir "el 3D gana" sin matiz. |
| 21 | 23:15-24:45 | T1-T3 fueron cronometradas; T4 fue exploratoria. SUS fue alto en 3D y NASA-TLX Raw fue menor en 3D que en 2D en los 12 casos. | Tabla SUS/NASA/tiempos. | Usar SUS como comparacion 3D vs 2D. |
| 22 | 24:45-27:15 | La discusion integra el efecto techo, menor carga percibida, menores tiempos, comentarios de Think-Aloud y limites de muestra/prototipo. | Matriz de triangulacion. | Convertir tendencias descriptivas en causalidad fuerte. |
| 23 | 27:15-28:00 | La contribucion es tecnica, metodologica y comunicativa: pipeline WebGL, evaluacion formativa y traduccion visual de hardware complejo. | Tres columnas. | Reducir el aporte a "una app bonita". |
| 24 | 28:00-28:30 | TwinSight X500 hace legibles relaciones de un ensamblaje complejo desde la web, con alcance honesto y evidencia trazable. Muchas gracias. | Dron final + tres takeaways. | Alargar el cierre o abrir un nuevo tema. |

## Cortes de emergencia

- Si quedan menos de 7 minutos al llegar a Slide 18, fusionar Slides 18-19 y decir solo: entorno, FPS, memoria, limite movil, triangulos no equivalentes.
- Si quedan menos de 5 minutos al llegar a Slide 20, fusionar Slides 20-22 y decir: efecto techo, T1-T3, SUS, NASA-TLX Raw, Think-Aloud y limites.
- Si el jurado interrumpe en demo, responder primero con alcance: "puedo mostrar lo publicado; las capacidades ocultas no las vendo como alcance visible".
