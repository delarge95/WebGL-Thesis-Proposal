# Breakdown: Sistema de Visualizacion Interactiva

## Que debe vender esta pieza

La pieza central del portafolio es el visor final del Holybro X500 V2. Su valor esta en que integra producto, interaccion 3D, shaders, arquitectura y criterio editorial en una sola experiencia WebGL.

## Flujo real de producto

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

Ese flujo debe ser el eje del storytelling. Todo lo demas es soporte.

## Problema que resuelve

El visor no solo "muestra un dron". Resuelve una lectura tecnica del ensamblaje al conectar:

- exploracion espacial;
- identificacion de piezas;
- contexto informativo;
- modos analiticos de visualizacion;
- controles de inspeccion y analisis sobre un modelo complejo.

## Features defendibles

- navegacion 3D y exploracion contextual;
- seleccion de piezas reales;
- `bottom sheet` de detalle;
- `Inspect` con hotspots, isolate y power/load;
- `Analyze` con explode, cut y filtros por categoria;
- `Studio` con `Realistic`, `X-Ray`, `Solid Color` y `Thermal`;
- onboarding procedural con gestos y microinteracciones coherentes con la app real;
- leyenda termica visible;
- fasteners seleccionables con metadata tecnica por instancia;
- detalle bajo demanda para fasteners sin cargar malla densa globalmente;
- Hero saneado y alineado con el caso real del Holybro X500 V2.

## Features implementadas pero no publicadas como producto final

- `MeasurementTool`;
- `Blueprint`, `Wireframe` y `Ghosted` como modos implementados pero no visibles en la UI final;
- partes avanzadas del clipping y del sistema termico;
- paneles o modulos legacy.

Estas capacidades sirven como profundidad tecnica secundaria, no como narrativa principal de producto.

## Features que no deben venderse como finales

- catalogo visible;
- settings visibles;
- suite completa de ensamblaje;
- audio como parte cerrada del producto;
- atajos de teclado no verificados como experiencia final;
- mallas Blender finales de fasteners si la escena demostrada aun usa placeholders temporales.

## Arquitectura minima que conviene explicar

### Capa de UI y orquestacion

- `UIManager`
- `UIHeroController`
- `UIDetailsSheet`

### Capa de interaccion

- `SelectionManager`
- `InputManager`
- `OrbitCameraController`

### Capa de visualizacion

- `ViewModeManager`
- `CrossSectionManager`
- `ExplodedViewManager`
- `PartVisibilityManager`
- `FastenerInspectionManager`

### Capa de estado y servicios

- `DroneStateController`
- `HotspotManager`
- `FastenerRegistry`
- subsistema termico

## Que mostrar en el breakdown

1. Problema de UX y lectura tecnica.
2. Flujo real del visor, incluyendo onboarding de primer uso.
3. Como la seleccion conecta escena y UI.
4. Como los modos analiticos cambian la lectura del objeto.
5. Como los fasteners se mantienen ligeros en reposo y solo muestran detalle al seleccionarse.
6. Como el onboarding reduce friccion y explica menus, sliders y gestos sin video.
7. Que parte del sistema es visible y que parte es soporte tecnico.

## Evidencia recomendada

- captura del Hero final;
- clip corto del onboarding procedural;
- captura de seleccion + `bottom sheet`;
- grid o clip corto de `Inspect / Analyze / Studio`;
- vista termica con leyenda;
- una captura de fastener seleccionado con detalle procedural activo;
- un diagrama corto de flujo de seleccion o arquitectura por capas;
- una referencia puntual a `SelectionManager`, `UIDetailsSheet`, `ViewModeManager`, `FastenerRegistry` y `FastenerInspectionManager`.

## Mensaje tecnico central

El valor no es solo visual. El visor demuestra capacidad para conectar:

- UI de producto;
- runtime interactivo;
- modos de visualizacion;
- onboarding de producto;
- arquitectura mantenible;
- optimizacion consciente para fasteners y subpiezas;
- documentacion honesta sobre alcance real.

## Pitch corto recomendado

> Diseñe y desarrolle un visor WebGL tecnico para el Holybro X500 V2 que permite explorar componentes, inspeccionar piezas, aplicar modos analiticos y mantener coherencia entre UI, escena y sistema visual dentro de las restricciones reales de Unity WebGL.
