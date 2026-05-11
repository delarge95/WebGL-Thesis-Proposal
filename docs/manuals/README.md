---
tipo: sistema
estado: mantenido
---

# WebGL Drone Visualization Prototype

> Visor WebGL/Unity para inspeccion tecnica interactiva del dron Holybro X500 V2. Este README resume el alcance real documentado de la app y evita describir herramientas ocultas o legacy como si fueran flujo final de usuario.

## Overview

La app actual se organiza alrededor de este flujo:

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

Funciones visibles y defendibles:

- Seleccion jerarquica de pieza madre, subpieza, grupo de hotspot y fastener.
- Ficha inferior con identificacion, especificaciones y ensamblaje.
- Hotspots, isolate, power/load.
- Analyze con explode, cross-section y filtros por categoria.
- Studio con modo base `Realistic`, modos visibles `X-Ray`, `Solid` y `Thermal`, y presets de entorno.
- Sistema modular de fasteners con proxies ligeros y detalle bajo demanda.
- Pipeline Blender/Unity preparado para importar el runtime final con masters + instancias.

No presentar como flujo final visible:

- Measurement tool.
- BOM/export de lista de materiales.
- Connection points.
- Anotaciones 3D.
- Assembly checklist.
- Catalogo legacy como panel de usuario.
- Siete modos publicados simultaneamente.

## Project Structure

```text
WebGL_tesis/
|-- desarrollo/
|   |-- unity_project/
|   |   |-- Assets/
|   |   |   |-- Scripts/
|   |   |   |   |-- Core/
|   |   |   |   |-- UI/
|   |   |   |-- Shaders/
|   |   |   |-- Models/
|   |-- docs/
|       |-- investigacion/Holybro/
|-- Informe_final/
|   |-- Manual_tecnico/
|   |-- Manual_de_usuario/
|   |-- presentation/
|-- Cerebro_Digital/
|   |-- Wiki/Concepts/
```

## Core Runtime

| System | Current role |
|--------|--------------|
| `UIManager` | Coordinates visible UI, managers, details sheet and mode handlers. |
| `SelectionManager` | Resolves selection and promotes fastener clicks to the complete fastener root. |
| `UIDetailsSheet` | Shows bottom sheet metadata and fastener-specific fields. |
| `PartVisibilityManager` | Handles isolate for mother pieces, subpieces, hotspot groups and individual fasteners. |
| `FastenerRegistry` | Resolves `instanceId -> family -> recipe` without name parsing at runtime. |
| `FastenerInspectionManager` | Activates modular detail only for selected/isolated/context fasteners. |
| `ExplodedViewManager` | Controls explode and category visibility. |
| `CrossSectionManager` | Applies shader clipping/cut planes. |
| `ViewModeManager` | Orchestrates implemented view modes. |
| `EnvironmentController` | Controls skybox, lighting and Blueprint/environment presets. |
| `DroneStateController` | Controls operational state, load and propeller animation. |
| `ThermalSimulationManager` | Runs heuristic component-level thermal simulation. |
| `ThermalViewController` | Sends thermal values to shader/UI legend. |
| `ImportedDroneRuntimeBinder` | Repairs imported hierarchy and reconnects managers to the drone root. |

## Data Sources

- `x500v2_parts_data.json`: canonical semantic part catalog.
- `DronePartData`: generated Unity data assets.
- `holybro_fastener_families.json`: modular fastener family definitions.
- `holybro_fastener_instances.json`: scene fastener instances and stable IDs.
- `holybro_fastener_reconciliation.json`: aliases, unresolved cases and dataset/scene discrepancies.
- `holybro_parent_subpieces.json`: mother part -> subpieces -> fasteners mapping.

## Blender Final Pipeline

The final runtime FBX must include both masters and instances because both contribute physical pieces to the drone:

- `BAKE_MASTERS_LOW`
- `ASSEMBLY_INSTANCES_LOW`
- `PRIMITIVE_FASTENER_MASTERS`
- `PRIMITIVE_FASTENER_INSTANCES`

Recommended texture set:

- `X500_BaseColor_4K.png`
- `X500_Normal_Final_4K.png`
- `X500_Mask_4K.png`

Packed mask:

- `R = AO`
- `G = Roughness`
- `B = Curvature`
- `A = Metallic`

If a fastener cannot be assigned confidently to a mother piece, it remains in review and must be reported with candidates, distance and reason. Do not assume.

## Controls

| Action | Desktop | Mobile |
|--------|---------|--------|
| Orbit | Viewport drag configured by camera controller | One finger drag |
| Pan | Configured pan drag | Two finger drag |
| Zoom | Mouse wheel | Pinch |
| Select | Click | Tap |
| Reset | `RESET VIEW` | `RESET VIEW` |
| Return | `HOME` | `HOME` |

Camera behavior is adaptive: zoom, pan and orbit scale with the active selection or isolate context so fasteners and small parts remain inspectable.

## Visual Modes

Visible UI:

- `Realistic` as base.
- `X-Ray`.
- `Solid`.
- `Thermal`.
- Environment presets including `Studio`, `Studio Light`, `Day`, `Night`, `Sunset`, color presets and `Blueprint` when available from the environment cycle.

Implemented/hidden or not part of final user flow:

- `Wireframe`.
- `Ghosted`.
- Some advanced/legacy visual paths.

## Thermal Scope

Thermal is a heuristic visual analysis by components. It is not FEA, calibrated thermography or a physical prediction model. It helps communicate relative trends and component-level heat behavior during inspection.

## Validation Status

The final report must not include performance, file-size, SUS or NASA-TLX values until they are measured on the frozen build. Before freeze, dashboards and tables should use explicit placeholders such as `pending post-freeze`.

## Documentation

- Technical manual: `Informe_final/Manual_tecnico/`.
- User manual: `Informe_final/Manual_de_usuario/`.
- Presentation package: `Informe_final/presentation/`.
- Holybro workflow docs: `desarrollo/docs/investigacion/Holybro/`.
- Obsidian study notes: `Cerebro_Digital/Wiki/Concepts/`.

## License

Academic thesis project. All rights reserved.

**Author**: Alexander Woodcock Salomon  
**Program**: Multimedia Engineering  
**Year**: 2026
