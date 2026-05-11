---
tipo: entregable
area: manuales
estado: activo
trace_id: TRC-MAN-TEC-0001
entregable_ids: ["MAN-TEC-ARQ", "MAN-TEC-CONFIG", "MAN-TEC-OPT"]
script_ids: ["SCR-APP-001", "SCR-ONB-001", "SCR-ONB-002", "SCR-PERF-001"]
bib_keys: ["hevner2004design", "peffers2007dsrm", "unity2024memory"]
resumen: "Manual tecnico alineado con la build final visible del Holybro X500 V2."
---

# Manual Tecnico - Holybro X500 V2 WebGL Viewer

## Proposito

Este manual resume la arquitectura real de la build final documentada del visor WebGL. Explica:

- flujo visible consolidado;
- capas runtime;
- onboarding;
- seleccion jerarquica;
- fasteners;
- modos visuales;
- entorno e iluminacion;
- lineamientos de build.

## Flujo visible consolidado

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

Todo lo que no encaje en ese flujo debe documentarse como oculto, legado o futuro, no como parte del producto final visible.

## Capas runtime

### Presentacion

- `MainLayout.uxml`
- USS
- `UIDetailsSheet`
- paneles y overlays de UI Toolkit

### Orquestacion

- `UIManager`
- `UIModeController`
- `AppStateMachine`

### Servicios de escena

- `SelectionManager`
- `PartVisibilityManager`
- `ExplodedViewManager`
- `CrossSectionManager`
- `ViewModeManager`
- `EnvironmentController`
- `DroneStateController`
- subsistema termico

### Datos

- `DronePartData`
- assets generados
- catalogo JSON canonico
- catalogos modulares de fasteners

## Pipeline Blender final

El modelo final se entrega desde Blender como un runtime compuesto por masters e instancias. Para no perder piezas fisicas, la exportacion manual debe incluir simultaneamente:

- `BAKE_MASTERS_LOW`
- `ASSEMBLY_INSTANCES_LOW`
- `PRIMITIVE_FASTENER_MASTERS`
- `PRIMITIVE_FASTENER_INSTANCES`

Los mapas recomendados para Unity son `X500_BaseColor_4K.png`, `X500_Normal_Final_4K.png` y `X500_Mask_4K.png`. La mask compacta usa `R=AO`, `G=Roughness`, `B=Curvature` y `A=Metallic`. Si se usa URP Lit estandar, puede generarse ademas `X500_MetallicSmoothness_4K.png`.

El manifest Blender se usa como reporte de importacion y revision: registra transforms, bounds, roles runtime y candidatos de pieza madre para fasteners. No asigna automaticamente un fastener ambiguo a una pieza madre.

## Bootstrap y saneamiento

El arranque de la build depende de dos mecanismos:

1. `UIManager.EnsureManagers()`
2. `ImportedDroneRuntimeBinder`

### `UIManager.EnsureManagers()`

Garantiza la presencia de los servicios minimos que la app necesita para operar.

### `ImportedDroneRuntimeBinder`

Repara la jerarquia importada en runtime:

- resuelve el root del dron;
- reconstruye anchors;
- reubica objetos huerfanos;
- sintetiza grupos como `x500v2_fastener_group` y `x500v2_misc_group`;
- vuelve a conectar caches y managers dependientes.

Sin esta capa, seleccion, filtros, isolate, hotspots, modes y UI pueden quedar incoherentes frente al import real.

## Onboarding

El onboarding visible se implementa con dos piezas:

- `OnboardingController`
- `OnboardingAnimationView`

### `OnboardingController`

- decide cuando mostrar la ayuda;
- coordina pasos, copy y cambio de plataforma;
- conecta el overlay con la UI real;
- puede abrirse en primer uso o desde `HELP`.

### `OnboardingAnimationView`

- dibuja las demos runtime con `Painter2D`;
- resuelve tarjetas para desktop y mobile;
- anima actor, objetivo y respuesta del sistema;
- evita depender de GIFs, video o media pesada.

## Seleccion jerarquica

La seleccion no es plana. El sistema diferencia:

- `pieza madre`
- `subpieza`
- `grupo de hotspot`
- `fastener individual`

### Implicaciones

- la ficha inferior puede cambiar de granularidad segun el nivel activo;
- el isolate puede operar sobre distintos alcances;
- la camara adapta foco y zoom al contexto de seleccion;
- los hotspots pueden representar lectura agrupada y no solo piezas individuales.

## Ficha inferior

Cuando `SelectionManager` publica un `PartSelectedEvent`, `UIManager` delega el contenido a `UIDetailsSheet`.

La ficha:

- actualiza nombre y contexto;
- muestra identificacion, especificaciones y ensamblaje;
- usa `FastenerMetadata` cuando la seleccion es un fastener;
- puede mostrar `subComponentNames` y resumen de fasteners cuando la seleccion es pieza madre;
- presenta lectura grupal cuando la seleccion viene desde un hotspot.

## Hotspots e isolate

### Hotspots

- los construye y muestra `HotspotManager`;
- inician activos en el flujo visible;
- pueden abrir selecciones agrupadas.

### Isolate

`PartVisibilityManager` resuelve aislamiento para:

- pieza madre;
- subpieza;
- grupo de hotspot;
- fastener individual.

Cuando la seleccion es una pieza madre, el isolate puede conservar fasteners reconciliados por `parentCanonicalPartId`.

## Fasteners

### Contrato de datos

La capa de fasteners se apoya en:

- `holybro_fastener_families.json`
- `holybro_fastener_instances.json`
- `holybro_fastener_reconciliation.json`
- `holybro_parent_subpieces.json`

### Runtime

- `FastenerRegistry` resuelve `instanceId -> family -> recipe`
- `FastenerInspectionManager` reemplaza proxies solo en el contexto necesario
- `FastenerBuilder` genera detalle procedural temporal

### Logica visible

- la escena reposa en proxies ligeros;
- al seleccionar un fastener, el sistema resuelve metadata y puede mostrar detalle temporal;
- el hover y la seleccion deben mantenerse incluso cuando el proxy se oculta;
- el isolate trata al fastener como unidad completa y no como submalla parcial.

## Inspect

`InspectModeHandler` controla:

- `Pins`
- `Isolate`
- `Power`
- `Load`

### Estados importantes

- `Power` cambia el estado de `DroneStateController`
- `Load` modifica carga operativa y alimenta el sistema termico
- el isolate puede actuar sobre pieza madre, subpieza, grupo de hotspot o fastener

## Analyze

`AnalyzeModeHandler` usa dos niveles:

- grilla principal de tarjetas;
- subpaneles para `Cross Section` o `Filter`.

### Cut

- inicia con eje `Y`;
- permite hasta dos ejes simultaneos;
- usa estrategia FIFO si entra un tercer eje;
- soporta inversion por plano;
- se limpia por desactivacion total o `RESET VIEW`.

### Explode

- usa slider en linea;
- no abre panel separado;
- conserva el ultimo valor distinto de cero para restauracion.

### Filter

Categorias visibles:

- `SkeletonAirframe`
- `PropulsionSystem`
- `Avionics`
- `SensorsComms`
- `PowerDistribution`
- `Fasteners`

Logica:

- todas activas por defecto;
- clic simple conmuta;
- doble clic activa modo exclusivo;
- apagar la ultima visible restaura el conjunto completo.

## Studio, entorno e iluminacion

`Studio` combina:

- `UIAnalyzePanel`
- `UIEnvironmentPanel`

### Modos visibles

- `X-Ray`
- `Solid`
- `Thermal`

`Realistic` es el modo base. `Blueprint` existe, pero se activa desde el ciclo de entorno y no como tarjeta de shader independiente.

### Ciclos de entorno

- `Studio -> Studio Light -> Blueprint`
- `Day -> Night -> Sunset`
- `White -> Grey -> Black -> Yellow -> Orange -> Green -> Blue -> Purple -> Red`

### Lighting Controls

La UI final expone tres sliders:

- `Light Rotation`
- `Light Intensity`
- `Background Tone`

### `EnvironmentController`

No usa un fondo plano simple. Opera con:

- `AnimatedGradientSkybox`
- color superior e inferior del gradiente
- color e intensidad de luz
- rotacion y pitch
- escala de gradiente
- dither
- pulso opcional
- grid para `Blueprint`

Tambien emite `OnLightBackgroundChanged` para que la UI adapte contraste cuando el fondo se vuelve muy claro.

## Sistema termico

Alcance real:

- visualizacion tecnica hibrida;
- no FEA;
- no termografia calibrada.

Pipeline:

```text
DroneStateController -> ThermalSimulationManager -> ThermalViewController -> shader/UI
```

## Build y despliegue

Para compilar en Unity:

1. Abrir `Build Settings`.
2. Seleccionar `WebGL`.
3. Dejar `Code Optimization` en `Speed`.
4. Usar `Brotli` para build final.
5. Ejecutar `Build And Run`.

## Regla editorial

Este manual no debe presentar como feature final:

- catalogo visible;
- settings visibles;
- measurement publicada;
- media pesada para onboarding;
- mallas Blender definitivas de fasteners si la build aun usa placeholders runtime.
