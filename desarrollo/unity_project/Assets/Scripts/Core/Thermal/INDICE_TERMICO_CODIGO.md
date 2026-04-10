# Indice de codigo termico

## Orden recomendado de lectura

1. `ThermalSimulationManager.cs`
2. `ThermalViewController.cs`
3. `ThermalContactGraphAsset.cs`
4. `ThermalSurfaceProfile.cs`
5. `ThermalContactGraphBuilderWindow.cs`
6. `DroneStateController.cs`
7. `UIAnalyzePanel.cs`
8. `Thermal.shader`

## Mapa rapido

### Solver y contactos

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
  - Solver oficial por nodos.
  - Intenta cargar `ThermalCanonicalContactGraph.asset` y cae al fallback solo si falta.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalContactGraphAsset.cs`
  - Formato serializable del grafo.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Resources\ThermalCanonicalContactGraph.asset`
  - Grafo explicito oficial de la V1.

### Visual y overrides

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
  - Cachea la leyenda UI.
  - Traduce temperaturas a `MaterialPropertyBlock`.
  - Usa presets canónicos cuando no hay `ThermalSurfaceProfile`.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSurfaceProfile.cs`
  - Override manual por pieza para hotspot, direccion, spread y propagation.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
  - Consume uniform, radial y axial por pieza.

### UI relacionada

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Panels\UIAnalyzePanel.cs`
  - Controla visibilidad de la leyenda en modo Thermal.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Layouts\MainLayout.uxml`
  - Tiene `ThermalLegendContainer`.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss`
  - Define el gradiente de la leyenda.

### Estado y bootstrap

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\DroneStateController.cs`
  - Expone `SystemLoadFactor` y estados del dron. En esta rama la carga termica visible se gobierna desde aqui, no desde un slider UI dedicado.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\SceneBootstrapper.cs`
  - Garantiza que los managers existan en runtime.

### Tooling offline

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Editor\Thermal\ThermalContactGraphBuilderWindow.cs`
  - Builder offline del grafo.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Editor\ThermalTestSetup.cs`
  - Experimental. Solo para pruebas con CAD bruto y datos dummy.

## Documentos cercanos al codigo

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AGENT_HANDOFF_THERMAL.md`
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\RETOPOLOGIA_POR_PIEZA.md`
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\MATRIZ_ACTUALIZACION_DOCUMENTAL.md`

## Estado real

Ya no falta crear la leyenda ni el shader espacial. Lo que falta es cerrar el uso sobre la escena final retopologizada y calibrar donde haga falta.
## Actualizacion 2026-03-31

Revisar tambien:

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\PREPARACION_FBX_IMPORTADO.md`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\ImportedDroneRuntimeBinder.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Content\PartRenderCategory.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Editor\SetupImportedDroneThermalTest.cs`

Notas de integracion vigentes:

- `ThermalViewController` ya genera la leyenda por textura runtime.
- `DroneStateController` ya gobierna el panel de power y el slider de carga visible.
- `SetupImportedDroneThermalTest` clasifica auxiliares, anade colliders, asigna layer seleccionable y coloca el binder runtime.

## Actualizacion 2026-04-10

Puntos de codigo que cierran esta etapa:

- `Thermal.shader` reduce el ruido animado y lo subordina a la banda termica real.
- `ThermalViewController` baja `bandHalfWidth` y `baseVariation` por defecto para limpiar la lectura en piezas no prioritarias.
- `ThermalSurfaceProfile` hereda un default mas sobrio para nuevos overrides manuales.

Regla de lectura:

- mas renderers o mas subpiezas no implican mas fuentes termicas principales,
- la jerarquia oficial sigue anclada en motores, ESC, bateria y electronica central,
- y el detalle adicional del modelo debe mapearse visualmente sin romper esa prioridad.
