# Breakdown: Sistema de Visualización Interactiva

## Qué debe vender esta pieza

La pieza central del portafolio es el visor final del Holybro X500 V2. Su valor está en que integra producto, interacción 3D, shaders, arquitectura y criterio editorial en una sola experiencia WebGL.

## Flujo real de producto

```text
Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio
```

Ese flujo debe ser el eje del storytelling. Todo lo demás es soporte.

## Problema que resuelve

El visor no solo “muestra un dron”. Resuelve una lectura técnica del ensamblaje al conectar:

- exploración espacial;
- identificación de piezas;
- contexto informativo;
- modos analíticos de visualización;
- controles de inspección y análisis sobre un modelo complejo.

## Features defendibles

- navegación 3D y exploración contextual;
- selección de piezas reales;
- `bottom sheet` de detalle;
- `Inspect` con hotspots, isolate y power/load;
- `Analyze` con explode, cut y filtros por categoría;
- `Studio` con `Realistic`, `X-Ray`, `Solid Color` y `Thermal`;
- leyenda térmica visible;
- Hero saneado y alineado con el caso real del Holybro X500 V2.

## Features implementadas pero no publicadas

- `MeasurementTool`;
- `Blueprint`, `Wireframe` y `Ghosted` como modos implementados pero no visibles en la UI final;
- partes avanzadas del clipping y del sistema térmico;
- paneles o módulos legacy.

Estas capacidades sirven como profundidad técnica secundaria, no como narrativa principal de producto.

## Features que no deben venderse como finales

- catálogo visible;
- settings visibles;
- suite completa de ensamblaje;
- audio como parte cerrada del producto;
- atajos de teclado no verificados como experiencia final.

## Arquitectura mínima que conviene explicar

### Capa de UI y orquestación

- `UIManager`
- `UIHeroController`
- `UIDetailsSheet`

### Capa de interacción

- `SelectionManager`
- `InputManager`
- `OrbitCameraController`

### Capa de visualización

- `ViewModeManager`
- `CrossSectionManager`
- `ExplodedViewManager`
- `PartVisibilityManager`

### Capa de estado y servicios

- `DroneStateController`
- `HotspotManager`
- subsistema térmico

## Qué mostrar en el breakdown

1. Problema de UX y lectura técnica.
2. Flujo real del visor.
3. Cómo la selección conecta escena y UI.
4. Cómo los modos analíticos cambian la lectura del objeto.
5. Qué parte del sistema es visible y qué parte es soporte técnico.

## Evidencia recomendada

- captura del Hero final;
- captura de selección + `bottom sheet`;
- grid o clip corto de `Inspect / Analyze / Studio`;
- vista térmica con leyenda;
- un diagrama corto de flujo de selección o arquitectura por capas;
- una referencia puntual a `SelectionManager`, `UIDetailsSheet` y `ViewModeManager`.

## Mensaje técnico central

El valor no es solo visual. El visor demuestra capacidad para conectar:

- UI de producto;
- runtime interactivo;
- modos de visualización;
- arquitectura mantenible;
- documentación honesta sobre alcance real.

## Pitch corto recomendado

> Diseñé y desarrollé un visor WebGL técnico para el Holybro X500 V2 que permite explorar componentes, inspeccionar piezas, aplicar modos analíticos y mantener coherencia entre UI, escena y sistema visual dentro de las restricciones reales de Unity WebGL.
