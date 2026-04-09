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
