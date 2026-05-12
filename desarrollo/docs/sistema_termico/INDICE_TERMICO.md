# Indice del sistema termico

## Uso de este indice

Si el visor del chat falla o abre archivos vacios, usa este documento como mapa de verdad y abre las rutas manualmente desde VS Code.

## Punto de entrada recomendado

1. `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
2. `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AGENT_HANDOFF_THERMAL.md`
3. `E:\WebGL_tesis\desarrollo\docs\sistema_termico\RETOPOLOGIA_POR_PIEZA.md`
4. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
5. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
6. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Resources\ThermalCanonicalContactGraph.asset`
7. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
8. `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AUDIT_TERMICO_FBX_FINAL_2026-05-11.md`

## Archivos clave

### Arquitectura y gobierno documental

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
  - Arquitectura oficial, estado real y pendientes.
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AGENT_HANDOFF_THERMAL.md`
  - Estado resumido para continuidad operativa.
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\RETOPOLOGIA_POR_PIEZA.md`
  - Guia 28+55 para modelado y retopologia.
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\MATRIZ_ACTUALIZACION_DOCUMENTAL.md`
  - Reglas de actualizacion de tesis, manuales y docs del subsistema.
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\wolfram_verificaciones.md`
  - Trazabilidad matematica.
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AUDIT_TERMICO_FBX_FINAL_2026-05-11.md`
  - Audit del FBX final, hallazgos sobre transferencia visible y correcciones implementadas en el runtime termico.

### Runtime oficial

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
  - Solver por nodos y carga del grafo canonico.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
  - Bridge solver-render, presets canónicos y cache de leyenda.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSurfaceProfile.cs`
  - Override manual por pieza cuando haga falta.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Resources\ThermalCanonicalContactGraph.asset`
  - Grafo explicito oficial de la V1.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
  - Visualizacion termica espacial por pieza.

### UI y scene integration

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Panels\UIAnalyzePanel.cs`
  - Muestra/oculta la leyenda termica segun `ViewMode.Thermal`.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Layouts\MainLayout.uxml`
  - Contenedor visual de la leyenda.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss`
  - Estilos y gradiente visual de la leyenda.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\DroneStateController.cs`
  - Fuente actual de `SystemLoadFactor` y estados del dron.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\SceneBootstrapper.cs`
  - Bootstrap runtime de los managers termicos.

### Tooling offline

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Editor\Thermal\ThermalContactGraphBuilderWindow.cs`
  - Generacion o refinamiento offline de contactos.
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Editor\ThermalTestSetup.cs`
  - Harness experimental para CAD bruto. No es el flujo oficial.

## Estado real del sistema

Ya existe:

- shader espacial,
- leyenda termica visible,
- grafo canonico oficial,
- fallback heuristico solo como respaldo,
- presets canónicos para piezas criticas.

El siguiente paso real ya no es "añadir shader espacial" ni "crear la leyenda". El siguiente paso real es:

- validar la escena final retopologizada,
- asignar overrides manuales solo donde aporten valor,
- y medir rendimiento en el target final.
## Actualizacion 2026-03-31

Nuevo punto de entrada operativo para el dron importado:

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\PREPARACION_FBX_IMPORTADO.md`

Nuevos archivos o rutas clave para esta integracion:

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\ImportedDroneRuntimeBinder.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Content\PartRenderCategory.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Editor\SetupImportedDroneThermalTest.cs`

Estado actual del flujo oficial:

- el panel de power y el slider de carga ya existen en UI,
- la leyenda termica usa gradiente runtime, no depende del gradiente USS,
- `Fasteners` y `Misc` ya forman parte de la taxonomia publica de filtros,
- y el dron importado requiere pasar por la etapa de preparacion antes de validar thermal, cut, filter, isolate y explode.

## Actualizacion 2026-04-10

Revisar tambien para el cierre fino de la etapa:

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSurfaceProfile.cs`

Notas vigentes:

- el shimmer termico final ya es mas suave y de baja frecuencia,
- la variacion base por defecto se redujo para no sobrerrepresentar subpiezas nuevas,
- y la prioridad termica oficial sigue concentrada en piezas canonicas aunque la malla visual aumente su granularidad.
