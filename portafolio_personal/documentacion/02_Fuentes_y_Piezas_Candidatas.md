# Fuentes y Piezas Candidatas

## Documento maestro

- `portafolio_personal/documentacion/08_Portafolio_Tech_Artist.md`

## Fuente canónica del producto

Estas fuentes mandan sobre cualquier borrador, pitch viejo o research exploratorio:

- `desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIHeroController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/ViewModeManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/CrossSectionManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/DroneStateController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalSimulationManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalViewController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs`
- `desarrollo/docs/investigacion/Holybro/x500v2_parts_data.json`
- `Informe_final/Desarrollo_App/VALIDACION_FUNCIONAL_FINA_2026-04-09.md`
- `Informe_final/Desarrollo_App/MATRIZ_DESCONEXIONES_APP_DOCUMENTACION.md`
- `Informe_final/Desarrollo_App/AUDITORIA_MATEMATICA_Y_ARQUITECTURA_2026-04-12.md`

## Investigación técnica que sí alimenta el portafolio

### Pipeline CAD, optimización y normalización

- `desarrollo/docs/investigacion/Holybro/CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md`
- `desarrollo/docs/investigacion/Holybro/CAD_Fastener_Optimization_Plan.md`
- `desarrollo/docs/investigacion/03_estrategia_optimizacion_webgl_2025.md`
- `desarrollo/docs/investigacion/10_workflow_integracion_cad_unity.md`
- `desarrollo/docs/investigacion/10_analisis_lods_y_retopologia.md`

### Visualización, shaders y arquitectura

- `desarrollo/docs/investigacion/02_arquitectura_shaders_visualizacion.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/03_Pipeline_Renderizado_Shaders.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/01_Arquitectura_del_Sistema.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/02_Referencia_Tecnica_Modulos.md`

### Sistema térmico y validación matemática

- `desarrollo/docs/investigacion/09_analisis_profundo_vista_termica.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`
- `desarrollo/docs/sistema_termico/wolfram_verificaciones.md`
- `Informe_final/Desarrollo_App/AUDITORIA_MATEMATICA_Y_ARQUITECTURA_2026-04-12.md`

## Piezas principales del portafolio

### Pieza 1: Visor WebGL final del Holybro X500 V2

- Valor: pieza central, visible, verificable y directamente ligada al producto final.
- Evidencia base:
  - flujo `Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio`
  - build funcional y capturas reales
  - validación funcional final

### Pieza 2: Sistema de visualización interactiva y view modes

- Valor: demuestra criterio de producto, integración UI-runtime y visualización técnica.
- Evidencia base:
  - `UIManager`
  - `SelectionManager`
  - `UIDetailsSheet`
  - `ViewModeManager`
  - diagramas del capítulo 4

### Pieza 3: Sistema térmico híbrido

- Valor: muestra integración entre simulación heurística, shader y visualización aplicada.
- Evidencia base:
  - `DroneStateController`
  - `ThermalSimulationManager`
  - `ThermalViewController`
  - `Thermal.shader`
  - `ThermalContactGraphBuilderWindow`

### Pieza 4: Caso CAD -> Unity -> WebGL

- Valor: demuestra pipeline, normalización semántica, lectura de restricciones WebGL y pensamiento sistémico.
- Evidencia base:
  - `x500v2_parts_data.json`
  - `ImportedDroneRuntimeBinder`
  - blueprint de Holybro
  - plan de optimización de fasteners
  - auditorías de escena y cobertura

### Pieza 5: Tooling de editor y verificación

- Valor: diferencia el proyecto de una simple demo visual.
- Evidencia base:
  - `ProjectSetupWizard`
  - `ImportedDroneCoverageAudit`
  - `ThermalContactGraphBuilderWindow`
  - `ImportedDroneRuntimeBinder`

### Pieza 6: Comunicación técnica del sistema

- Valor: útil para TA leads y hiring managers que valoran documentación, pensamiento arquitectónico y honestidad técnica.
- Evidencia base:
  - diagramas Mermaid del informe final
  - documentación técnica actualizada
  - matriz de desconexiones
  - auditoría matemática y de arquitectura

## Piezas secundarias o de apoyo

- breakdown de shaders individuales;
- script o extracto corto de `SelectionManager.cs`;
- `ClippableLit.shader`, `XRay.shader`, `Blueprint.shader`, `Thermal.shader`;
- `cad_symmetry_addon_v5_ultimate.py`, `cad_symmetry_addon_v6_batch.py`, `generate_inventory.py` como evidencia de tooling de pipeline.

Estas piezas son buenas como profundidad técnica, pero no deben competir con la pieza principal del visor.

## Research exploratorio: usar con cuidado

Estos documentos y herramientas sirven para contexto, investigación o diseño de futuro, pero no deben presentarse automáticamente como parte implementada de la build final:

- PiXYZ, Simplygon, InstaLOD y Quad Remesher como herramientas consultadas en research;
- Houdini como pipeline explorado;
- sistemas de ensamblaje completos;
- audio;
- measurement visible;
- view modes ocultos como si fueran UX publicada.

## Regla de selección final

Una pieza entra al portafolio público solo si cumple tres condiciones:

1. existe evidencia verificable;
2. su alcance es coherente con la app real;
3. su valor profesional se entiende rápido sin necesidad de leer toda la tesis.
