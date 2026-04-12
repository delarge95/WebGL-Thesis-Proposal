# 08. Sistema termico hibrido

## Estado

Activo, en desarrollo tecnico. No debe describirse todavia como simulacion termodinamica completa ni como FEA en tiempo real.

## Objetivo

Implementar una simulacion termica hibrida, fisicamente inspirada y visualmente creible para el dron Holybro X500 V2 en Unity WebGL.

## Alcance real de la V1

- solver termico reducido por componentes canonicos,
- conduccion entre piezas en contacto,
- conveccion simplificada al ambiente,
- vista termica alimentada por temperatura calculada por pieza,
- propagacion espacial visual en piezas criticas,
- leyenda termica visible en UI,
- authoring offline del grafo de contactos termicos,
- y fallback heuristico solo como respaldo de desarrollo.

## Componentes principales

### ThermalSimulationManager

Responsable del solver por componentes. Construye nodos termicos, evalua carga efectiva, calcula equilibrio, enfriamiento y conduccion.

### ThermalViewController

Puente entre solver y render. Lee temperaturas, cachea la leyenda y envia parametros al shader termico por `MaterialPropertyBlock`.

### ThermalSurfaceProfile

Perfil opcional por pieza critica. Permite definir hotspot, direccion y propagacion visual sin modificar la fisica global.

### ThermalContactGraphAsset

Asset que almacena nodos, metadata de build y enlaces termicos entre piezas.

### ThermalContactGraphBuilderWindow

Herramienta de editor que analiza `ExplodablePart` en la escena, estima cercania/contacto por bounds y genera un `ThermalContactGraphAsset` reutilizable en runtime.

### ThermalCanonicalContactGraph.asset

Asset canónico oficial de la V1. Define la red explicita de contactos para las 28 piezas oficiales del solver.

### Thermal.shader

Shader termico ajustado para recibir banda de temperatura normalizada, modo de propagacion visual, hotspot local, direccion axial local, spread, edge cooling y propagation.

## Modelo matematico operativo

La V1 usa un solver reducido por componentes, no una simulacion FEA. La cadena real del sistema es:

`DroneStateController -> ThermalSimulationManager -> ThermalViewController -> Thermal.shader`

### Calentamiento por equilibrio

Para nodos que no son fuente de calor:

```text
T_eq = T_amb
```

Para fuentes termicas, el equilibrio se interpola entre temperatura ambiente, hover y pico segun la carga efectiva del dron.

La aproximacion temporal usada en runtime es:

```text
sourceBlend = 1 - exp(-Δt / τ)
sourceDelta = (T_eq - T_actual) * sourceBlend * sourceWeight
```

### Enfriamiento ambiental

```text
coolingDelta = (T_amb - T_actual) * coolingRate * exposure * Δt
```

### Acoplamiento entre piezas

La transferencia entre nodos conectados se aproxima mediante:

```text
ΔT_ij = (T_j - T_i) * Ĝ_ij * Δt
Ĝ_ij = max(0.001, s_ij * A_ij / L_ij)
```

donde:

- `A_ij` es el area de contacto proxy,
- `L_ij` es la longitud efectiva del camino termico,
- `s_ij` es un factor heuristico de enlace.

Esta forma comparte la estructura de una conduccion tipo Fourier, pero `s_ij` no es una conductividad termica calibrada en unidades SI. Por eso el sistema debe documentarse como simulacion hibrida heuristica.

## Integracion con el sistema existente

- `DroneStateController` expone `SystemLoadFactor` y estados del dron.
- En esta rama, la carga termica visible se gobierna desde `DroneStateController`; un slider dedicado de usuario sigue siendo una mejora futura, no una capacidad ya cerrada.
- `SceneBootstrapper` asegura la presencia de `ThermalSimulationManager` y `ThermalViewController` en runtime.
- `MainLayout.uxml`, `Theme.uss` y `UIAnalyzePanel.cs` ya sostienen la leyenda termica visible en modo Thermal.
- `ThermalContactGraphBuilderWindow` habilita un workflow de preprocesado offline sin tocar escenas serializadas.
- `ThermalTestSetup.cs` se conserva solo como harness experimental para CAD bruto.

## Limitaciones actuales

- el grafo canónico requiere calibracion final sobre la geometria retopologizada del X500 V2,
- no hay validacion cuantitativa cerrada contra termografias reales,
- no se han asignado aun overrides manuales refinados a todas las piezas criticas,
- la simulacion actual prioriza credibilidad visual y estabilidad WebGL sobre exactitud numerica de alta fidelidad.

## Siguiente hito

Validar el sistema sobre la escena final retopologizada, decidir donde usar `ThermalSurfaceProfile` manual y medir rendimiento del modo Thermal en Unity Editor y build objetivo.
## Actualizacion 2026-03-31

### Integracion del dron importado

La escena oficial ya dispone de una ruta de preparacion para `x500v2_Drone` basada en dos pasos:

1. `Tools > Import Drone Model Into Scene`
2. `Tools > Thermal > Prepare Imported Drone For Thermal Test`

Esa preparacion agrega anchors canonicos, categorias por renderer, colliders de seleccion, layer `SelectablePart` y un `ImportedDroneRuntimeBinder` para recachear managers en runtime.

### Estado de energia visible

En la rama actual el modo Inspect ya expone un panel de power con estado textual y slider de carga. `DroneStateController` gobierna `OFF`, `STARTING`, `IDLE`, `FLYING` y alimenta la carga termica visible del solver.

### Fasteners y Misc

`Fasteners` y `Misc` ya forman parte de la taxonomia publica de filtros y pueden aislarse como coleccion visual. No amplian la granularidad fisica del solver: heredan la temperatura del ensamblaje padre.

## Actualizacion 2026-04-10

### Refinamiento visual de cierre

La vista termica recibio un ajuste final para reducir flicker y microcontraste falso en la escena retopologizada:

- ruido animado mas grande, mas lento y mas suavizado,
- menor amplitud de variacion por defecto,
- y brillo de borde desacoplado del valor termico mostrado.

### Regla para el modelo con mas piezas

El incremento de piezas y subpiezas visibles del dron no cambia la jerarquia oficial del solver V1. La lectura termica sigue anclada en los ensamblajes canonicos de mayor interes: motores, ESC, bateria, stack central, brazos y plates.
