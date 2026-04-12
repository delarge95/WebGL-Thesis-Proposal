# Referencia Tecnica de Modulos

Inventario tecnico resumido de los modulos relevantes para el cierre, dividido por estado funcional.

## 1. Runtime principal

| Modulo | Estado | Responsabilidad |
| --- | --- | --- |
| `UIManager` | activo | coordinador principal de UI y enrutamiento de eventos |
| `UIHeroController` | activo | Hero, submenus y entrada al visor |
| `UIDetailsSheet` | activo | ficha inferior de pieza seleccionada |
| `UIModeController` | activo | orquesta Inspect, Analyze y Studio |
| `InspectModeHandler` | activo | hotspots, isolate, power/load |
| `AnalyzeModeHandler` | activo | cut, explode, filtros |
| `StudioModeHandler` | activo | contenedor visible del panel Studio |
| `UIAnalyzePanel` | activo | cambio de view modes |
| `UIEnvironmentPanel` | activo | presets y sliders de entorno |
| `UICrossSectionPanel` | activo | controles del plano de corte |
| `SelectionManager` | activo | seleccion de piezas |
| `ExplodedViewManager` | activo | explode y filtros de categoria |
| `PartVisibilityManager` | activo | ocultar, mostrar y aislar |
| `CrossSectionManager` | activo | clipping global |
| `ViewModeManager` | activo | pipeline de modos de vista |
| `DroneStateController` | activo | encendido y carga |
| `ThermalSimulationManager` | activo | simulacion reducida |
| `ThermalViewController` | activo | paso a materiales y leyenda |
| `HotspotManager` | activo | gestion de hotspots |
| `EnvironmentController` | activo | fondo, presets y luz |
| `InputManager` | activo | bloqueo de input sobre UI |
| `ImportedDroneRuntimeBinder` | activo | saneamiento del import y reconstruccion de caches |

## 2. Tooling y editor

| Modulo | Estado | Responsabilidad |
| --- | --- | --- |
| `ProjectSetupWizard` | activo | crea estructura base de escena |
| `ImportedDroneCoverageAudit` | activo | cruza escena y catalogo canonico |
| `ThermalContactGraphBuilderWindow` | activo | genera el grafo termico offline |
| `AssignURPConfig` | activo | soporte de configuracion |
| `FixSceneConfig` | activo | saneamiento de escena |
| `WebGLBuildFixer` | activo | correcciones previas al build |

## 3. Implementado pero oculto

| Modulo | Estado | Motivo |
| --- | --- | --- |
| `MeasurementTool` | oculto | no se expone hasta validar escala util del modelo |
| `Blueprint`, `Wireframe`, `Ghosted` | oculto | implementados en `ViewModeManager` pero no expuestos en la UI final |
| `EnvPreset_Neutral` | oculto | boton presente en UXML con `display: none` |
| partes del flujo termico avanzado | oculto | se preservan como base tecnica, no como feature cerrada |

## 4. Codigo legado o no integrado

| Modulo | Estado | Observacion |
| --- | --- | --- |
| `PartCatalogUI` | legacy/no integrado | no hace parte del flujo visible documentado |
| `SettingsPanel` | legacy/no integrado | no visible en la build final |
| `LoadingController` | legacy/no integrado | no describe el flujo oficial de cierre |
| `EnhancedInfoPanel` | parcial/legacy | parte de la logica quedo fuera del flujo final |
| `AssemblyGuideManager` | no integrado | base existente, sin UI final integrada |
| `AssemblyChecklist` | no integrado | trabajo futuro |
| `BillOfMaterialsManager` | no integrado | trabajo futuro |
| `AnnotationSystem` | no integrado | trabajo futuro |
| `ConnectionPointsViewer` | no integrado | trabajo futuro |
| `ModularPartsSystem` | no integrado | no corresponde al alcance final visible |

## 5. Dependencias externas

| Dependencia | Uso |
| --- | --- |
| Unity 6 / WebGL | build y runtime del prototipo |
| URP | materiales y render base |
| UI Toolkit | interfaz declarativa |
| Browser scripting | integracion puntual con WebGL |
| JSON canonico Holybro | catalogo semantico de piezas |

## 6. Modulos que deben salir de la documentacion activa

Los siguientes nombres no deben reaparecer como modulos activos del producto final salvo evidencia nueva:

- `ViewModeToolbar`
- `WebGLOptimizer`
- `ScreenshotManager`
- `KeyboardShortcuts`
- `PerformanceMonitor`
- `RuntimeConsole`
- `SceneTransitionManager`
- `TooltipSystem`

## 7. Regla de lectura

Cuando un documento cite un modulo, debe indicar una de estas etiquetas:

- `Expuesto en UI final`
- `Implementado pero oculto`
- `Codigo legado/no integrado`
- `Trabajo futuro`
