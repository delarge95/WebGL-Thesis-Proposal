# Guia de mediciones tecnicas WebGL

## Proposito

Este documento define el metodo operativo para medir el rendimiento tecnico de la build WebGL del prototipo Holybro X500 V2 usando el tooling real implementado en la app. Su funcion es asegurar que las cifras del informe final sean trazables, repetibles y comparables entre dispositivos, navegadores y escenarios.

La guia complementa el capitulo metodologico del informe final, las tablas de resultados del capitulo 5 y los anexos de cierre. No reemplaza los instrumentos de usabilidad, SUS, NASA-TLX Raw ni Think-Aloud; corresponde solo a la capa tecnica de evaluacion.

## Tooling disponible

### 1. WebGLProfiler

Archivo fuente:

`desarrollo/unity_project/Assets/Scripts/Core/Utils/WebGLProfiler.cs`

Es la herramienta principal para mediciones sobre la build WebGL publicada. Se crea automaticamente despues de cargar la escena si no existe otro `WebGLProfiler` activo. Tambien lo garantiza `SceneBootstrapper`.

Caracteristicas reales del tooling:

- overlay compacto visible como `PERF <fps> FPS`;
- tecla `F8` para expandir/colapsar en escritorio;
- click/tap sobre el overlay para expandirlo;
- muestreo cada `0.5 s`;
- inicio automatico de una sesion `scene_base` al arrancar;
- maximo de 32 sesiones completadas en memoria;
- botones rapidos de escenario:
  - `scene_base`;
  - `selection_isolate`;
  - `explode`;
  - `cut`;
  - `thermal_studio`;
  - `mobile_free`;
- boton `Stop + Save Session`;
- boton `Clear Sessions`;
- boton unico `Download JSON + CSV`;
- descarga automatica de dos archivos desde el navegador: un `.json` y un `.csv`;
- cierre automatico de la sesion activa si el evaluador pulsa `Download JSON + CSV` mientras todavia se esta grabando.

Campos principales exportados:

- `sessionId`;
- `scenario`;
- `startedAtUtc`;
- `endedAtUtc`;
- `durationSeconds`;
- `appUptimeAtSessionStartSeconds`;
- `appUptimeAtSessionEndSeconds`;
- `sampleCount`;
- `averageFps`;
- `averageFrameMs`;
- `worstFrameMs`;
- `lowestSampledFps`;
- `averageHeapMb`;
- `peakHeapMb`;
- `averageAllocatedMemoryMb`;
- `peakAllocatedMemoryMb`;
- `averageReservedMemoryMb`;
- `peakReservedMemoryMb`;
- `screenWidth`;
- `screenHeight`;
- `screenDpi`;
- `platform`;
- `deviceType`;
- `deviceModel`;
- `operatingSystem`;
- `browserUrl`;
- `unityVersion`;
- `appVersion`;
- `sceneName`;
- `qualityLevel`;
- `renderScale`;
- `viewMode`;
- `explosionFactor`;
- `crossSectionEnabled`;
- `rendererCount`;
- `enabledRendererCount`;
- `meshCount`;
- `estimatedTriangleCount`;
- `graphicsDeviceName`;
- `graphicsDeviceType`;
- `systemMemoryMb`;
- `graphicsMemoryMb`.

La salida CSV usa exactamente estos campos y debe conservarse sin renombrar columnas para facilitar trazabilidad.

### 2. MigrationBenchmarkRunner

Archivo fuente:

`desarrollo/unity_project/Assets/Scripts/Core/Utils/MigrationBenchmarkRunner.cs`

Es una herramienta complementaria para benchmark controlado en Editor o runtimes no WebGL. No sustituye la medicion sobre la build publicada.

Uso esperado:

- ejecutar en Unity Editor o build no WebGL;
- agregar o localizar un `MigrationBenchmarkRunner` en escena;
- usar el menu contextual `Populate Default Presets` si no hay presets;
- ejecutar `Run Migration Benchmark`;
- revisar consola y reporte generado.

Valores por defecto:

- escenarios: `Idle` e `Interactive`;
- repeticiones por escenario: `3`;
- duracion idle: `30 s`;
- duracion interactive: `60 s`;
- presets:
  - `Preset A`: inspeccion cercana de una pieza interactiva;
  - `Preset B`: dron completo a distancia media;
  - `Preset C`: dron completo a distancia larga.

El escenario `Interactive` automatiza camara, seleccion, Blueprint, Thermal, Realistic, corte y explosion parcial. El reporte contiene `runs`, `aggregates` y `notes`.

Ruta de salida en Editor/no WebGL:

`desarrollo/unity_project/Reports/MigrationMetrics/runtime/benchmark_<fecha>.json`

Limitacion importante:

En WebGL runtime no hay escritura directa de archivo para este runner. Por eso, para evidencia oficial de la build web publicada se usa `WebGLProfiler` y su boton de descarga JSON/CSV.

## Alcance de la medicion

Las mediciones oficiales deben ejecutarse sobre una build publicada o congelada para evaluacion, idealmente la misma version desplegada en GitHub Pages.

Se mide:

- rendimiento en tiempo real;
- estabilidad de frame time;
- memoria reportada por Unity/WebGL;
- resolucion, DPR/render scale, navegador, sistema operativo y dispositivo;
- comportamiento por escenario funcional;
- tiempo de carga inicial y carga con cache;
- peso de artefactos principales de build;
- contexto tecnico de la sesion.

No se mide:

- temperatura fisica real del dron;
- termografia calibrada;
- simulacion FEA;
- prediccion de fallas;
- compatibilidad universal de todos los navegadores o dispositivos existentes.

## Politica de calidad vigente

La build final prioriza resolucion alta y estabilidad visual:

- `QualityManager` mantiene `renderScale = 1.0`;
- el template WebGL limita `devicePixelRatio` a `min(window.devicePixelRatio, 2)`;
- la resolucion movil no debe bajarse para la medicion final salvo que se documente explicitamente como condicion alternativa.

Lectura de estado del profiler:

| Estado | FPS |
|---|---:|
| GOOD | >= 55 |
| OK | >= 30 |
| WARNING | >= 20 |
| CRITICAL | < 20 |

La meta operativa principal es mantener 30 FPS o mas, equivalente a un frame time promedio de 33.33 ms o menos.

## Preparacion previa

1. Confirmar URL exacta de la build.
2. Registrar commit, fecha de build y fecha de medicion.
3. Verificar que la app carga sin errores criticos de consola.
4. Registrar dispositivo, navegador, version, sistema operativo, resolucion y conexion.
5. Definir si la medicion es `cold cache` o `warm cache`.
6. Para `cold cache`, limpiar cache o usar una ventana limpia/incognito y registrar el metodo.
7. Para `warm cache`, hacer una carga previa y medir desde la segunda carga.
8. No mezclar `cold cache` y `warm cache` en una misma cifra agregada.
9. Esperar a que la app sea interactiva antes de iniciar sesiones de rendimiento, salvo en el escenario especifico de carga inicial.

## Como usar el WebGLProfiler en la build

1. Abrir la build WebGL.
2. Localizar el overlay `PERF` en la esquina superior izquierda.
3. Expandirlo con click/tap o `F8`.
4. Si la sesion automatica `scene_base` ya empezo durante la carga, detenerla y limpiar sesiones antes de las mediciones formales, salvo que se quiera conservar como evidencia de arranque.
5. Esperar 3 a 5 segundos de estabilizacion.
6. Pulsar el boton del escenario a medir.
7. Ejecutar la interaccion definida durante 45 a 60 segundos.
8. Pulsar `Stop + Save Session`.
9. Repetir para cada escenario.
10. Al terminar, pulsar `Download JSON + CSV`.
11. Verificar que el navegador descargo dos archivos:
    - `x500v2_perf_<escenario-o-all_sessions>_<fecha_utc>_<plataforma>_<dispositivo>.json`;
    - `x500v2_perf_<escenario-o-all_sessions>_<fecha_utc>_<plataforma>_<dispositivo>.csv`.

Nombres recomendados:

- `perf_<fecha>_<dispositivo>_<navegador>_<build>.csv`;
- `perf_<fecha>_<dispositivo>_<navegador>_<build>.json`;
- `perf_<fecha>_<dispositivo>_<navegador>_notas.md`.

Ubicacion recomendada:

`Telemetria/Unity_Builds/Mediciones_WebGL/`

Si se mide en movil, revisar la carpeta de descargas del navegador o del sistema. Algunos navegadores pueden pedir permiso para multiples descargas; permitir la descarga cuando aparezca la solicitud. Las cifras definitivas del informe deben provenir de los archivos JSON/CSV descargados siempre que sea posible.

## Escenarios oficiales del overlay

Cada escenario debe durar entre 45 y 60 segundos. Evitar cambiar de escenario a mitad de captura.

### S0. Carga inicial

Este escenario no lo mide completamente el `WebGLProfiler`, porque el profiler solo opera cuando Unity ya cargo. Se mide manualmente con cronometro y herramientas del navegador.

Procedimiento:

1. Abrir la landing o URL directa.
2. Iniciar cronometro al solicitar la experiencia.
3. Detener cronometro cuando la app permite orbitar o seleccionar.
4. Registrar si fue `cold cache` o `warm cache`.
5. Registrar errores de consola o pantallas de carga anormales.

Metricas:

- tiempo hasta primera interaccion;
- estado de cache;
- URL;
- artefactos principales;
- errores de consola.

### S1. `scene_base`

Objetivo: medir rendimiento base sin herramientas analiticas activas.

Procedimiento:

1. Restaurar vista normal.
2. Mantener modo `Realistic`.
3. Pulsar `scene_base`.
4. Orbitar, panear y hacer zoom suavemente.
5. No activar corte, explosion ni cambio de shader.
6. Guardar sesion.

### S2. `selection_isolate`

Objetivo: medir seleccion, panel de informacion, hotspots y aislamiento.

Procedimiento:

1. Pulsar `selection_isolate`.
2. Seleccionar piezas principales y subpiezas.
3. Abrir y cerrar el panel de informacion.
4. Usar scroll o foldouts si estan disponibles.
5. Probar doble tap/doble click para aislar y desaislar.
6. Guardar sesion.

Registrar tambien:

- si la seleccion funciona fuera del panel;
- si el panel bloquea el canvas solo dentro de su rectangulo visible;
- si hotspots y doble tap responden correctamente.

### S3. `explode`

Objetivo: medir costo de vista explosionada.

Procedimiento:

1. Activar `Analyze > Explode`.
2. Pulsar `explode`.
3. Medir idealmente tres sesiones separadas:
   - explosion 30 %;
   - explosion 60 %;
   - explosion 100 %.
4. Como alternativa, capturar una sesion continua moviendo el slider lentamente.
5. Guardar sesion.

Registrar visualmente:

- piezas en contacto;
- colisiones;
- movimientos anormales;
- estabilidad de FPS.

### S4. `cut`

Objetivo: medir costo de corte transversal.

Procedimiento:

1. Activar `Analyze > Cut`.
2. Pulsar `cut`.
3. Mover el plano lentamente.
4. Cambiar eje o invertir solo si forma parte de la prueba.
5. Guardar sesion.

Registrar:

- parpadeos;
- errores visuales;
- estabilidad del shader;
- impacto en FPS/frame time.

### S5. `thermal_studio`

Objetivo: medir Studio y visualizacion termica heuristica.

Procedimiento:

1. Entrar en `Studio`.
2. Activar `Thermal`.
3. Pulsar `thermal_studio`.
4. Navegar la escena y observar motores, bateria, electronica, helices, estructura y fasteners.
5. Guardar sesion.

Nota:

El campo `viewMode` del CSV/JSON registra el modo visual activo. Si se quiere medir `X-Ray`, `Solid View` o `Blueprint`, usar el boton mas cercano (`thermal_studio` o `mobile_free`) y documentar explicitamente el modo en notas, o ejecutar mediciones manuales separadas conservando el campo `viewMode`.

### S6. `mobile_free`

Objetivo: medir una sesion mixta de interaccion real, especialmente en movil.

Procedimiento:

1. Pulsar `mobile_free`.
2. Ejecutar durante 60 segundos:
   - orbit;
   - pan;
   - zoom;
   - seleccion;
   - apertura/cierre del panel;
   - cambio de modo visual;
   - explosion o corte breve.
3. Guardar sesion.

Este escenario sirve como prueba de estabilidad funcional. No reemplaza los escenarios aislados.

## Uso complementario de MigrationBenchmarkRunner

Usar esta herramienta cuando se necesite comparar builds o hacer regresion controlada desde Unity Editor.

Procedimiento resumido:

1. Abrir `MainScene_Final`.
2. Crear un GameObject llamado `MigrationBenchmarkRunner` si no existe.
3. Agregar el componente `MigrationBenchmarkRunner`.
4. En el inspector, usar `Populate Default Presets`.
5. Revisar:
   - `repetitionsPerScenario = 3`;
   - `idleDurationSeconds = 30`;
   - `interactiveDurationSeconds = 60`;
   - `runIdleScenario = true`;
   - `runInteractiveScenario = true`.
6. Ejecutar `Run Migration Benchmark` desde el menu contextual del componente.
7. Revisar consola.
8. Guardar el reporte JSON si fue generado.

Interpretacion:

- `Idle` mide camara estable sobre el target.
- `Interactive` simula camara, seleccion, modos visuales, corte y explosion.
- `aggregates` resume promedios y peores casos por preset/escenario.
- `notes` debe revisarse siempre; puede indicar dependencias no resueltas.

Este benchmark no es la evidencia principal para rendimiento WebGL movil, porque no corre necesariamente en el navegador/dispositivo final. Su valor esta en comparar builds y detectar regresiones.

## Plantilla minima de registro

Usar una tabla por dispositivo y fecha:

| Campo | Valor |
|---|---|
| Codigo de sesion |  |
| Fecha/hora local |  |
| Commit/build |  |
| URL o ruta |  |
| Dispositivo |  |
| Sistema operativo |  |
| Navegador/version |  |
| Resolucion |  |
| DPR/render scale |  |
| Conexion |  |
| Estado de cache | cold / warm |
| Escenario |  |
| Duracion |  |
| FPS promedio |  |
| Frame time promedio |  |
| Peor frame time |  |
| FPS minimo muestreado |  |
| Memoria promedio/pico |  |
| Errores de consola |  |
| Observaciones |  |

## Reglas de interpretacion

1. Reportar siempre el contexto junto a la cifra.
2. No mezclar escritorio y movil en un promedio unico sin separarlos.
3. No mezclar primera carga sin cache con carga posterior con cache.
4. Si el navegador no expone memoria real del proceso, reportar solo memoria disponible desde Unity/WebGL y aclarar la limitacion.
5. `systemMemoryMb` y `graphicsMemoryMb` pueden ser aproximados o no representativos en WebGL; se reportan como contexto, no como medicion fina.
6. Usar `averageFps`, `averageFrameMs`, `worstFrameMs` y `lowestSampledFps` juntos. Un promedio alto no elimina picos problematicos.
7. Conservar `sampleCount`; sesiones demasiado cortas o con pocas muestras no deben usarse como evidencia principal.
8. Acompanar resultados con capturas, CSV/JSON exportado y observaciones del evaluador.
9. No convertir observaciones cualitativas de fluidez en cifras no medidas.
10. Si hay menos dispositivos de los deseados, declarar el alcance como revision tecnica formativa, no como compatibilidad universal.

## Evidencias que deben conservarse

Por cada bloque de mediciones guardar:

- CSV descargado con `Download JSON + CSV`;
- JSON descargado con `Download JSON + CSV`;
- captura del overlay;
- captura de consola si hay errores;
- tabla consolidada por dispositivo;
- fecha, commit y URL;
- notas de comportamiento perceptible;
- si aplica, reporte `MigrationBenchmarkRunner` en `Reports/MigrationMetrics/runtime`.

## Relacion con el informe final

Esta guia alimenta directamente:

- Capitulo 3: metodologia, variables, instrumentos y metas operativas.
- Capitulo 5: tablas de KPIs tecnicos, FPS por dispositivo, compatibilidad y graficas consolidadas.
- Apendices: documentacion tecnica activa y esquemas de consolidacion de resultados.

Las tablas del capitulo 5 deben mantenerse alineadas con valores reales, fechados y trazables a sesiones concretas del profiler. Si se repite una medicion, se actualizan simultaneamente el archivo exportado, la tabla consolidada y la nota metodologica correspondiente.
