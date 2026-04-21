# README Curado del Proyecto

## Holybro X500 V2 WebGL Technical Viewer

Interactive WebGL viewer for technical exploration of a real drone platform, built with Unity and focused on selection, analytical visualization, runtime consistency and applied thermal rendering.

## Qué es

Este proyecto desarrolla un visor técnico WebGL para el Holybro X500 V2. La aplicación permite:

- explorar el dron en 3D;
- seleccionar componentes;
- consultar información contextual en un `bottom sheet`;
- usar herramientas de inspección y análisis;
- cambiar entre modos visuales técnicos;
- visualizar un modo térmico híbrido con leyenda.

## Flujo principal

```text
Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio
```

## Qué hace visible la build final

- Hero alineado con el caso Holybro X500 V2
- onboarding animado y ligero para explicar controles reales en desktop y mobile
- selección de piezas
- `bottom sheet` de detalle
- hotspots
- isolate
- power/load
- explode
- cross-section
- filtros por categoría
- `Realistic`, `X-Ray`, `Solid Color` y `Thermal`
- leyenda térmica visible
- fasteners con metadata por instancia y detalle bajo demanda

## Stack técnico

- Unity WebGL
- Universal Render Pipeline
- UI Toolkit
- C#
- shaders HLSL para visualización técnica

## Componentes técnicos destacados

### Runtime y arquitectura

- `UIManager`
- `SelectionManager`
- `UIDetailsSheet`
- `ViewModeManager`
- `CrossSectionManager`
- `DroneStateController`
- `ImportedDroneRuntimeBinder`
- `OnboardingController`
- `OnboardingAnimationView`
- `FastenerRegistry`
- `FastenerInspectionManager`

### Sistema térmico

- `ThermalSimulationManager`
- `ThermalViewController`
- `Thermal.shader`
- `ThermalContactGraphBuilderWindow`

### Tooling de editor

- `ProjectSetupWizard`
- `ImportedDroneCoverageAudit`
- `ThermalContactGraphBuilderWindow`

## Taxonomía operativa del proyecto

El proyecto usa tres capas de lectura para evitar confusiones entre semántica de investigación e implementación técnica:

- `28` piezas canónicas
- `30` anchors de escena
- `257` renderers/colliders auditados

## Qué problema resuelve

El reto principal no era solo mostrar un modelo 3D, sino convertir un activo técnico complejo en una experiencia web clara y mantenible.

Para ello, el proyecto aborda:

- normalización semántica;
- reparación runtime de jerarquía importada;
- visualización analítica;
- coherencia entre escena, UI y datos;
- onboarding guiado alineado con los controles reales;
- fasteners tecnicos sin cargar detalle denso en toda la escena;
- restricciones reales de Unity WebGL.

## Visualización técnica

Los modos visuales del proyecto están pensados como herramientas de lectura, no solo como cambios cosméticos.

La build final visible publica:

- `Realistic`
- `X-Ray`
- `Solid Color`
- `Thermal`

Existen otros modos implementados como profundidad técnica adicional, pero no se presentan aquí como parte de la UX final visible.

## Sistema térmico

El modo térmico del proyecto debe entenderse como una simulación híbrida heurística orientada a visualización técnica aplicada.

No se presenta como:

- FEA
- termografía calibrada
- solver físico experimental cerrado

Sí se presenta como:

- integración entre estado operativo, simulación reducida, shader y UI;
- visualización clara de comportamiento térmico a nivel de componente;
- herramienta útil para inspección interactiva.

## Tooling y validación

Una parte importante del trabajo fue construir herramientas de soporte para:

- setup del proyecto;
- auditoría de cobertura de escena;
- construcción y revisión del grafo térmico;
- consistencia entre taxonomía, runtime y visualización.

## Estado editorial honesto

Este README no vende como finales:

- audio implementado;
- suite completa de ensamblaje visible;
- catálogo visible;
- settings visibles;
- measurement publicada;
- Houdini como pipeline central de la build final.

## Enfoque del proyecto

Este trabajo se posiciona mejor como:

- technical visualization
- runtime tooling
- shader systems
- CAD-to-WebGL thinking

## Assets recomendados para acompañar este README

- Hero final
- clip corto del onboarding
- selección + `bottom sheet`
- modo `Thermal` con leyenda
- fastener con metadata y detalle runtime
- captura de tooling
- diagrama corto de arquitectura

## Resumen corto

> A Unity WebGL technical viewer for the Holybro X500 V2 that combines runtime import repair, technical view modes, contextual part inspection and hybrid thermal visualization in a coherent interactive workflow.
