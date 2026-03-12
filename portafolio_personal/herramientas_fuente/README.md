# Herramientas Fuente Curadas

Staging técnico para reunir piezas reutilizables del proyecto que sirven como evidencia de pipeline, shader work y UI técnica en portafolio profesional.

## Regla operativa

- La fuente de verdad permanece en sus rutas originales.
- Aquí se conservan copias curadas para análisis, selección y empaquetado de piezas.
- No usar esta carpeta como dependencia del build Unity ni de la tesis.

## Subcarpetas actuales

### `pipeline_holybro/`

- `cad_symmetry_addon_v5_ultimate.py`
- `cad_symmetry_addon_v6_batch.py`
- `generate_inventory.py`

Estas piezas resumen la parte más fuerte del pipeline de ingestión, simetría e inventariado CAD asociado al lote Holybro.

### `shaders/`

- `Blueprint.shader`
- `ClippableLit.shader`
- `EdgeDetection.shader`
- `Ghosted.shader`
- `Thermal.shader`
- `WireframeWebGL.shader`
- `XRay.shader`

Esta selección cubre modos de visualización técnicamente vendibles para breakdowns: blueprint, clipping, detección de bordes, ghosted, térmico, wireframe optimizado para WebGL y rayos X.

### `ui_tecnica/`

- `InputManager.cs`
- `OrbitCameraController.cs`
- `ProceduralCloseIcon.cs`
- `ProceduralIconBase.cs`
- `SelectionManager.cs`
- `UIDetailsSheet.cs`
- `ViewModeManager.cs`

Esta selección concentra piezas útiles para explicar interacción, navegación de cámara, selección por raycast, gestión de modos visuales, iconografía procedural y comportamiento del info sheet.

## Procedencia del material

- `pipeline_holybro/` proviene de `desarrollo/docs/investigacion/Holybro/`.
- `shaders/` proviene de `desarrollo/unity_project/Assets/Shaders/`.
- `ui_tecnica/` proviene de `desarrollo/unity_project/Assets/Scripts/`.

## Próximos candidatos a evaluar

- `CrossSectionManager.cs`

## Línea emergente a vigilar

- El sistema de encendido con animación y propagación térmica progresiva debe entrar a esta curaduría cuando su pieza central de código quede estabilizada.
- Mientras tanto, `Thermal.shader` y `ViewModeManager.cs` funcionan como base técnica ya presentable para explicar esa dirección de trabajo sin sobreprometerla.

## Restricción vigente

- Esta curaduría no toca `blender_files/` mientras siga siendo zona activa de preparación de meshes para Unity.
