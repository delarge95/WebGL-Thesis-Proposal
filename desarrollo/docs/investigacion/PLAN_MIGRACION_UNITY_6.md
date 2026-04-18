# Plan de Migracion a Unity 6.4 LTS (Mesh LOD + WebGL + Metricas)

Este documento reemplaza el plan anterior a 6.3 y fija la ruta oficial de migracion a **Unity 6000.4.3f1**.

El objetivo ya no es solo "subir de version", sino hacerlo con una puerta de control por oleadas para medir el efecto real de los Mesh LODs en la app antes de seguir ampliando su uso.

## Objetivos

- Migrar el proyecto desde `6000.0.62f1` a `6000.4.3f1`.
- Mantener `WebGL 2`, `WebGPU` desactivado y `web threading` desactivado en esta fase.
- Introducir `Mesh LOD` con un esquema de `piloto + oleadas`.
- Registrar metricas comparables por fase para tomar decisiones con evidencia.

## Decisiones fijas

- Version objetivo: `6000.4.3f1`.
- La migracion no activa `threadsSupport`.
- La migracion no activa `WebGPU` en produccion.
- Los Mesh LODs se aplican primero con el generador de Unity.
- Si una familia falla visual o funcionalmente, pasa a fallback con `_LOD0/_LOD1/_LOD2`.

## Fases

### Fase 0: Baseline 6.0

- Capturar `baseline_6_0.md`.
- Medir tamano de `.data.br`, `.wasm.br`, `.framework.js.br` y total descargable.
- Medir carga en frio y en caliente.
- Medir FPS, peor frame time y heap con `WebGLProfiler`.
- Medir tris, verts y batches en tres presets de camara:
  - Preset A: inspeccion cercana de pieza interactiva.
  - Preset B: dron completo a distancia media.
  - Preset C: dron completo a distancia lejana.

### Fase 1: Salto de motor a 6.4

- Abrir el proyecto en Unity 6.4.3f1.
- Resolver compatibilidad de importacion, scripts, URP, shaders y UI Toolkit.
- Generar build Web funcional sin activar Mesh LOD.
- Capturar `baseline_6_4_sin_lod.md`.

### Fase 2: Ola 0 / piloto Mesh LOD

- Aplicar Mesh LOD a motores, brazos y landing gear.
- Validar visualmente en escena y funcionalmente en runtime.
- Capturar `wave_0_piloto.md`.

### Fase 3: Ola 1

- Aplicar Mesh LOD a repetidos medianos y subcomponentes visibles a media distancia.
- Capturar `wave_1.md`.

### Fase 4: Ola 2

- Aplicar Mesh LOD a fasteners, separadores, soportes pequenos y piezas repetidas menores.
- Capturar `wave_2.md`.

### Fase 5: Ola 3

- Aplicar Mesh LOD a piezas estructurales grandes solo si el profiling sigue mostrando cuello de botella geometrico.
- Capturar `wave_3.md`.

## Gate de avance por oleada

Para pasar de una fase a la siguiente deben cumplirse todas estas reglas:

- No se permite regresion funcional.
- No se permite incremento de tamano descargable mayor a `3%` respecto a la fase anterior.
- En `Preset B` o `Preset C` debe existir al menos una mejora:
  - `15%+` menos tris, o
  - `10%+` mejor FPS promedio, o
  - `10%+` mejor frame time.
- En `Preset A` no se permite degradacion mayor a `5%` en FPS o frame time.

Si una ola no cumple el gate:

- se congela el rollout;
- se revisa familia por familia;
- se decide entre `ajustar` o `rollback parcial`.

## Pruebas obligatorias

En cada fase:

- carga de escena principal;
- seleccion de piezas;
- hotspots;
- cambio Analyze/Studio;
- explode;
- cut;
- thermal view;
- clipping de `Thermal.shader`;
- navegacion UI Toolkit.

En cada ola de Mesh LOD:

- transicion cerca/media/lejos;
- ausencia de popping severo;
- ausencia de desapariciones indebidas;
- consistencia de materiales, normales, outline y clipping;
- consistencia de interaccion cuando cambia el LOD.

## Herramientas del repo

El repo ya incorpora el soporte operativo para este plan:

- `Assets/Scripts/Core/Utils/WebGLProfiler.cs`
  - ahora soporta sesiones y resumenes reutilizables para benchmarks.
- `Assets/Scripts/Core/Utils/MigrationBenchmarkRunner.cs`
  - ejecuta presets A/B/C, escenarios Idle/Interactive y deja JSON de resultados.
- `Assets/Editor/Antigravity/Fixes/MigrationMetricsReporter.cs`
  - escribe snapshots de build y configuracion.
- `Assets/Scripts/Tests/Editor/MigrationSmokeTests.cs`
  - valida version objetivo, flags WebGL, escena principal y artefactos de migracion.
- `Reports/MigrationMetrics/`
  - contiene las plantillas oficiales de baseline y de cada oleada.

## Estado de la migracion

Implementacion de plan en repo: lista.

Pendiente para completar la migracion real:

- abrir el proyecto con Unity 6.4.3f1 instalado;
- permitir que Unity regenere paquetes, Library y metadatos de version;
- ejecutar el baseline real en editor/build;
- empezar la Ola 0 con assets piloto.
