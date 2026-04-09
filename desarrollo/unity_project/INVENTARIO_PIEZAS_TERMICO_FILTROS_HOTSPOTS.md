# Inventario técnico de piezas, térmico, filtros y hotspots

Fecha de corte: 2026-04-08

## 1) Fuentes revisadas

- Assets de datos de piezas (legacy y actuales):
  - Assets/Data/Parts
  - Assets/Core/Data/X500V2Generated
- Simulación térmica:
  - Assets/Resources/ThermalCanonicalContactGraph.asset
  - Assets/Scripts/Core/Thermal/ThermalSimulationManager.cs
- Filtros:
  - Assets/Scripts/UI/Panels/AnalyzeModeHandler.cs
  - Assets/Scripts/UI/UIManager.cs
  - Assets/Scripts/Core/Content/ExplodedViewManager.cs
  - Assets/Scripts/Core/Content/PartRenderCategory.cs
  - Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs
  - Assets/Scripts/Core/Managers/PartCatalogManager.cs
- Hotspots:
  - Assets/Scripts/UI/HotspotManager.cs
  - Assets/Scripts/UI/SmartHotspot.cs
  - Bandera isHotspotTarget en DronePartData assets

## 2) Lista de piezas totales (con información/base de datos)

### 2.1 Dataset actual importado (X500V2Generated)

Total: 28 piezas con DronePartData (.asset)

IDs:

- x500v2_arm_BL
- x500v2_arm_BR
- x500v2_arm_FL
- x500v2_arm_FR
- x500v2_battery
- x500v2_bottom_plate
- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_gps_m10
- x500v2_landing_gear
- x500v2_motor_BL
- x500v2_motor_BR
- x500v2_motor_FL
- x500v2_motor_FR
- x500v2_pdb
- x500v2_pixhawk6c
- x500v2_platform_board
- x500v2_power_module
- x500v2_prop_BL
- x500v2_prop_BR
- x500v2_prop_FL
- x500v2_prop_FR
- x500v2_rails_battery
- x500v2_rc_receiver
- x500v2_telemetry_radio
- x500v2_top_plate

### 2.2 Dataset legacy adicional (Assets/Data/Parts)

Total: 16 assets de pieza

IDs:

- ant-001
- arm-fl-001
- battery-001
- body-001
- cam-fpv-001
- esc-001
- fc-001
- gps-001
- motor-fl-001
- prop-fl-001
- vtx-001
- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_pdb

Observación:

- Hay solapamiento entre ambos datasets (x500v2_esc_BL/BR/FL/FR y x500v2_pdb).
- En runtime, PartCatalogManager trabaja con ExplodablePart encontrados en escena (no por carpeta), y en logs del proyecto aparece Found 28 parts para el modelo actual.

## 3) Piezas con simulación térmica

### 3.1 Piezas incluidas en la simulación térmica de contacto (grafo canónico)

Fuente: Assets/Resources/ThermalCanonicalContactGraph.asset

Total: 28 nodos térmicos

Node IDs del grafo:

- x500v2_bottom_plate
- x500v2_top_plate
- x500v2_arm_FL
- x500v2_arm_FR
- x500v2_arm_BL
- x500v2_arm_BR
- x500v2_landing_gear
- x500v2_platform_board
- x500v2_rails_battery
- x500v2_pdb
- x500v2_power_module
- x500v2_pixhawk6c
- x500v2_gps_m10
- x500v2_telemetry_radio
- x500v2_motor_FL
- x500v2_motor_FR
- x500v2_motor_BL
- x500v2_motor_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_prop_FL
- x500v2_prop_FR
- x500v2_prop_BL
- x500v2_prop_BR
- x500v2_battery
- x500v2_rc_receiver

Conclusión:

- Para el modelo actual, la simulación térmica cubre las 28 piezas del dataset generado.

### 3.2 Piezas marcadas como térmicamente críticas (flag isThermallyCritical=1)

En Assets/Core/Data/X500V2Generated:

Total críticas: 18

- x500v2_arm_BL
- x500v2_arm_BR
- x500v2_arm_FL
- x500v2_arm_FR
- x500v2_battery
- x500v2_bottom_plate
- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_motor_BL
- x500v2_motor_BR
- x500v2_motor_FL
- x500v2_motor_FR
- x500v2_pdb
- x500v2_pixhawk6c
- x500v2_power_module
- x500v2_top_plate

## 4) Función de filtrado: filtros existentes y cobertura

## 4.1 Filtros de Analyze Mode (UI actual)

Definición en UIManager + AnalyzeModeHandler:

- ALL
- Structure
- Propulsion
- Electronics
- Fasteners
- Misc

Pipeline técnico:

- UIManager.BindCat(...) envía categoría a UIModeController/AnalyzeModeHandler.
- AnalyzeModeHandler mantiene activeCategories.
- ExplodedViewManager.SetCategoryFilters(...) aplica visibilidad por renderer.
- PartRenderCategory.MatchesAny(...) resuelve pertenencia.

Regla de clasificación principal (ImportedDroneRuntimeBinder.InferDisplayCategory):

- Propulsion si id/nombre contiene motor/prop/esc
- Electronics si contiene battery/gps/pixhawk/pdb/power_module/receiver/telemetry/radio
- Structure si contiene arm/plate/landing/platform/rail
- Si no coincide, Misc

### 4.2 Piezas por filtro (dataset generado actual de 28)

ALL (28)

- todas las piezas del listado de la sección 2.1

Structure (9)

- x500v2_arm_BL
- x500v2_arm_BR
- x500v2_arm_FL
- x500v2_arm_FR
- x500v2_bottom_plate
- x500v2_landing_gear
- x500v2_platform_board
- x500v2_rails_battery
- x500v2_top_plate

Propulsion (12)

- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_motor_BL
- x500v2_motor_BR
- x500v2_motor_FL
- x500v2_motor_FR
- x500v2_prop_BL
- x500v2_prop_BR
- x500v2_prop_FL
- x500v2_prop_FR

Electronics (7)

- x500v2_battery
- x500v2_gps_m10
- x500v2_pdb
- x500v2_pixhawk6c
- x500v2_power_module
- x500v2_rc_receiver
- x500v2_telemetry_radio

Fasteners (0, a nivel pieza raíz)

- sin piezas raíz dedicadas en el dataset actual
- nota: pueden aparecer renderers auxiliares clasificados como Fasteners por nombre (screw/bolt/nut/etc.)

Misc (0, a nivel pieza raíz)

- sin piezas raíz dedicadas en el dataset actual
- nota: pueden aparecer renderers auxiliares clasificados como Misc por nombre (clip/holder/connector/etc.)

Cobertura:

- Sí, las 28 piezas raíz actuales quedan cubiertas por al menos una categoría de Analyze filter.

### 4.3 Filtro del Part Catalog Manager (sistema paralelo)

PartCatalogManager maneja categorías UI distintas (enum PartCategory):

- Avionics
- Sensors & Comms
- Power Distribution
- Propulsion System
- Skeleton & Airframe
- Fasteners

Distribución para X500V2Generated (category enum en assets):

- Avionics: 1
- SensorsComms: 3
- PowerDistribution: 3
- PropulsionSystem: 12
- SkeletonAirframe: 9
- Fasteners: 0

Nota importante:

- Hay dos taxonomías en paralelo:
  - Analyze filter (Structure/Propulsion/Electronics/Fasteners/Misc)
  - Catalog filter (enum PartCategory)
- Actualmente son consistentes en cobertura global, pero no son la misma nomenclatura.

## 5) Hotspots: configuración, cantidad y cobertura

Implementación:

- HotspotManager recorre ExplodablePart en escena.
- Crea hotspot solo si part.Data.isHotspotTarget == true.
- Cada hotspot se instancia con un único ExplodablePart target (SmartHotspot(targetPart)).

Conclusión de diseño:

- Cada hotspot corresponde a una pieza objetivo (1 hotspot = 1 pieza), no a un conjunto.

Hotspots objetivo en dataset X500V2Generated (isHotspotTarget=1):

Total: 13 hotspots

- x500v2_battery
- x500v2_esc_BL
- x500v2_esc_BR
- x500v2_esc_FL
- x500v2_esc_FR
- x500v2_gps_m10
- x500v2_motor_BL
- x500v2_motor_BR
- x500v2_motor_FL
- x500v2_motor_FR
- x500v2_pixhawk6c
- x500v2_power_module
- x500v2_telemetry_radio

Cobertura de hotspots:

- Hotspot directo: 13/28 piezas
- Sin hotspot directo: 15/28 piezas

## 6) Resumen ejecutivo

- Piezas con información/base de datos (modelo actual generado): 28
- Dataset legacy adicional en Data/Parts: 16 assets (con solapamiento parcial)
- Piezas en simulación térmica (grafo): 28
- Piezas térmicamente críticas (flag): 18
- Filtros Analyze disponibles: ALL, Structure, Propulsion, Electronics, Fasteners, Misc
- Cobertura de Analyze por piezas raíz actuales: 100% (28/28 en Structure/Propulsion/Electronics)
- Hotspots actuales: 13, con relación 1:1 hotspot-pieza

## 7) Riesgos/observaciones para la próxima importación

- Existen dos fuentes de datos de piezas (legacy y generated) con posible duplicación de IDs.
- Existen dos taxonomías de filtro (Analyze vs Catalog) que conviene unificar o documentar en runtime.
- Al reemplazar el modelo, habrá que regenerar y validar:
  - Assets/Core/Data/X500V2Generated
  - ThermalCanonicalContactGraph.asset
  - flags isHotspotTarget/isThermallyCritical por pieza
  - correspondencia de categorías para Analyze/Catalog

## 8) Inclusión de TODAS las piezas (fuente Holybro)

Se revisaron las fuentes en `desarrollo/docs/investigacion/Holybro` para responder al requerimiento de incluir todas las piezas, incluyendo tornillería/fasteners.

Fuentes relevantes detectadas:

- `x500v2_parts_data.json` (dataset canónico simplificado): 28 entradas
- `x500v2_blender_synced_parts.json` (dataset granular sincronizado desde Blender): 55 entradas únicas
- `x500v2_blender_inventory.md` (inventario físico con cantidades): 247 instancias de malla

Conclusión operativa:

- Para cubrir TODAS las piezas, el dataset base debe ser `x500v2_blender_synced_parts.json` + cantidades de `x500v2_blender_inventory.md`.
- El dataset de 28 (`x500v2_parts_data.json`) no incluye la granularidad de fasteners/subcomponentes.

## 9) Conteos consolidados (all pieces)

- Piezas únicas canónicas (json simplificado): 28
- Piezas únicas granulares (json Blender synced): 55
- Instancias totales de malla (inventario markdown Holybro): 247

Nota:

- El valor de 255 mencionado en planificación está cerca del inventario actual de 247; para el siguiente modelo importado el total puede cambiar.

## 10) Fasteners incluidos (piezas únicas detectadas)

Fasteners/elementos de fijación identificados en `x500v2_blender_synced_parts.json` (20 IDs únicos):

- x500v2_blend_gb70_m25_10 (Socket Head Cap Screw)
- x500v2_blend_gb70_m25_12 (Socket Head Cap Screw)
- x500v2_blend_gb70_m25_6 (Socket Head Cap Screw)
- x500v2_blend_gb70_m3_21_ding (Socket Head Cap Screw)
- x500v2_blend_gb70_m3_25_ding (Socket Head Cap Screw)
- x500v2_blend_gb70_m3_38 (Socket Head Cap Screw)
- x500v2_blend_gb70_m3_6 (Socket Head Cap Screw)
- x500v2_blend_gb70_m3_8_ding (Socket Head Cap Screw)
- x500v2_blend_gpsv5_zhijia_luomao (GPS Mast Securing Nut)
- x500v2_blend_lm_m3_ding (M3 Dome/Cap Nut)
- x500v2_blend_lm_m3_nilong (M3 Nyloc Nut)
- x500v2_blend_m25_6_chen_liu (M2.5 Micro Screw)
- x500v2_blend_m3_10_pan_ding (M3 Pan/Countersunk Screw)
- x500v2_blend_m3_14_pan (M3 Pan/Countersunk Screw)
- x500v2_blend_m3_16_chen_liu (M3 Pan/Countersunk Screw)
- x500v2_blend_nilongzhu_m25_5 (M2.5 Micro Screw)
- x500v2_blend_nilongzhu_m3_5 (M3 Pan/Countersunk Screw)
- x500v2_blend_zslm_m25 (Self-Locking Flange Nut)
- x500v2_blend_zslm_m3_ding (M3 Dome/Cap Nut)
- x500v2_blend_zslm_m3_falan (Self-Locking Flange Nut)

## 11) Filtros finales requeridos (PartCatalogManager)

Tu requerimiento es correcto: los filtros finales deben ser los de `PartCatalogManager`.

Categorías destino (enum PartCategory):

- Avionics
- SensorsComms
- PowerDistribution
- PropulsionSystem
- SkeletonAirframe
- Fasteners

Estado actual:

- Analyze mode usa taxonomía distinta (`Structure/Propulsion/Electronics/Fasteners/Misc`).
- Para incluir las 55 piezas únicas (y sus 247 instancias), se necesita unificar la taxonomía en runtime hacia PartCatalogManager.

Recomendación de estructura para incluir todo sin duplicar datos:

- Pieza única (`PieceType`): metadatos completos, categoría PartCatalog, flags térmicos/hotspot.
- Instancia (`PieceInstance`): referencia a PieceType + transform/path/sector.

Esto permite:

- Mostrar catálogo por no repetidas (p.ej. 55/74 según dataset final).
- Operar visualización/selección por instancias (247 o el total nuevo del modelo).

## 12) Hotspots de piezas importantes y/o conjuntos

Sí, es totalmente lograble y recomendable.

Modelo recomendado:

- Hotspot tipo `piece`: apunta a una pieza única crítica.
- Hotspot tipo `group`: apunta a un conjunto (ej. tren motriz FL, stack de potencia, conjunto fasteners de brazo).

Ventaja:

- Evita saturación visual cuando se incluyen todos los fasteners.
- Mantiene navegación útil para usuario final.

## 13) Decisión para siguiente iteración

Para el nuevo modelo importado, usar como criterio de cobertura:

- 100% de piezas únicas granulares (incluyendo fasteners)
- 100% de instancias registradas
- 100% de piezas clasificadas en categorías PartCatalogManager
- Hotspots solo en piezas/grupos de alta relevancia técnica

## 14) Estado implementado tras esta iteración (2026-04-08)

Esta sección complementa el inventario previo sin reemplazarlo.

### 14.1 Fuente de datos en el setup térmico/editor

Se actualizó el pipeline de preparación en `SetupImportedDroneThermalTest` para priorizar:

- `x500v2_blender_synced_parts.json` (dataset granular, 55 piezas únicas)

y usar fallback automático a:

- `x500v2_parts_data.json` (dataset canónico de 28)

Además, cuando el ID canónico no coincide directamente con nodos de escena, el setup ahora intenta match por `blenderName`.

Impacto:

- La generación de `DronePartData` en `Assets/Core/Data/X500V2Generated` puede escalar desde 28 hacia el universo granular del JSON synced, dependiendo de coincidencias reales en escena.

### 14.2 Normalización de categorías hacia PartCatalogManager

Se reforzó la normalización de categorías en la generación de assets para converger al enum destino:

- `Avionics`
- `SensorsComms`
- `PowerDistribution`
- `PropulsionSystem`
- `SkeletonAirframe`
- `Fasteners`

Mapeos legacy/synced (ejemplos):

- `Structure` -> `SkeletonAirframe`
- `Propulsion` -> `PropulsionSystem`
- `Electronics` -> `SensorsComms`
- `Power` -> `PowerDistribution`
- `Misc` -> `Uncategorized`

Nota:

- En inferencia por nombre térmico se separó `PowerDistribution` de `SensorsComms` y `Avionics` para mejorar coherencia de filtrado.

### 14.3 Analyze Filter (UI) actualizado

Se agregó la categoría de Analyze faltante para alineación con PartCatalog:

- Nuevo botón: `CatBtn_Sensors` (`SensorsComms`)

Con esto, Analyze dispone de:

- `ALL`
- `SkeletonAirframe`
- `PropulsionSystem`
- `Avionics`
- `SensorsComms`
- `PowerDistribution`
- `Fasteners`

Se actualizaron bindings y estado visual en:

- `UIManager`
- `AnalyzeModeHandler`
- `MainLayout.uxml`
- `InputManager` (detección de UI interactiva)

### 14.4 Estado funcional resultante

Estado actual tras cambios:

- [x] Setup capaz de leer dataset granular (55) con fallback seguro al canónico (28)
- [x] Match de nodos por `id` y por `blenderName`
- [x] Analyze con categoría `SensorsComms` explícita
- [x] Taxonomía de filtros más alineada a `PartCatalogManager`

Pendiente para cerrar cobertura total end-to-end:

- [ ] Verificar en escena importada que todas las entradas granular/synced queden efectivamente ancladas (no solo parseadas)
- [ ] Regenerar y validar `ThermalCanonicalContactGraph.asset` para cobertura térmica equivalente al nuevo universo de piezas
- [ ] Auditar distribución final por categoría tras regeneración para confirmar conteos objetivo (55 únicas / ~247 instancias o nuevo total real)

### 14.5 Cambio UX en filtros Analyze (sin ALL)

Se removió el botón `ALL` del panel Analyze.

Nuevo comportamiento:

- Estado inicial: todas las categorías activas.
- Click simple en categoría: toggle on/off de esa categoría (si se intenta apagar la última activa, se restablece el estado por defecto con todas activas).
- Doble click en categoría:
  - Si no estaba en modo exclusivo, deja solo esa categoría activa.
  - Si ya estaba en modo exclusivo sobre esa misma categoría, vuelve al estado por defecto (todas activas).

Categorías visibles actuales en Analyze:

- `SkeletonAirframe`
- `PropulsionSystem`
- `Avionics`
- `SensorsComms`
- `PowerDistribution`
- `Fasteners`

### 14.6 Diagnóstico técnico: piezas no seleccionables o "en el centro"

En el modelo temporal/importado, las causas más probables detectadas son:

1. Reanclaje heurístico de nodos auxiliares:

- El pipeline intenta reagrupar huérfanos al "mejor anchor" por distancia/nombre.
- Si la heurística falla o el naming difiere, algunas mallas pueden quedar bajo anchors inesperados.

2. Creación de anchors en posición de fallback:

- Cuando no hay bounds válidos del match, el anchor nuevo puede crearse en posición raíz (centro del dron/escena), dando la sensación de piezas colapsadas al centro.

3. Selección basada en collider por renderer:

- Si un renderer no tiene `MeshFilter`/mesh compatible o no recibió collider en el setup, esa parte puede quedar visible pero no seleccionable.

4. Filtros de visibilidad deshabilitan colliders:

- En `ExplodedViewManager`, al ocultar renderers por filtro, también se deshabilitan colliders; una pieza filtrada no se puede seleccionar hasta volver a estar visible.

5. Desfase entre IDs canónicos y nombres Blender:

- Aunque ya hay fallback por `blenderName`, aún pueden existir casos no mapeados 1:1 en este modelo provisional.

### 14.7 Auditoría automática agregada (paso siguiente listo)

Se agregó una auditoría de cobertura ejecutable desde Editor:

- Menú: `Tools/Thermal/Audit Imported Drone Coverage`
- Script: `Assets/Editor/ImportedDroneCoverageAudit.cs`
- Salida: `desarrollo/unity_project/Reports/imported_drone_coverage_report.md`

Qué valida en una sola ejecución:

- IDs esperados desde `x500v2_blender_synced_parts.json` (fallback a `x500v2_parts_data.json`)
- Anchors reales con `ExplodablePart` en `x500v2_Drone`
- IDs faltantes y anchors extra
- Renderers con/sin collider
- Renderers fuera de layer `SelectablePart`
- Anchors cerca del origen y anchors colapsados cerca del root
- Objetos top-level huérfanos con renderer (sin `ExplodablePart`)

Uso recomendado para retomar:

1. Ejecutar `Tools/Thermal/Prepare Imported Drone For Thermal Test`.
2. Ejecutar `Tools/Thermal/Audit Imported Drone Coverage`.
3. Corregir primero: `IDs faltantes`, `anchors sin renderer`, `renderers sin collider`.
4. Repetir auditoría hasta tener cobertura aceptable para cerrar esta fase.
