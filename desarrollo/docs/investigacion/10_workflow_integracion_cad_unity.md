# Workflow de Integración: CAD → Unity WebGL (Holybro X500 V2)

Plan completo para llevar las piezas retopologizadas del drone al proyecto Unity, conectar el JSON de datos, configurar hotspots selectivos, y añadir animación térmica progresiva.

## User Review Required

> [!IMPORTANT]
> **Decisión requerida:** ¿Blender o Houdini para retopología y LODs? El plan describe ambos flujos, pero los scripts de automatización se crearán solo para la herramienta elegida.

> [!WARNING]
> El `Thermal.shader` actual usa `_MinTemp` / `_MaxTemp` como valores estáticos por material. Para la animación progresiva de calentamiento, se necesita añadir un `_HeatFactor` global controlado por script, lo que implica modificar el shader y el `ViewModeManager`.

---

## Fase 0: Preparación del JSON → ScriptableObjects

### Objetivo
Cargar `x500v2_parts_data.json` y generar automáticamente los 28 `DronePartData` ScriptableObject assets en Unity.

### Cambios Propuestos

#### [MODIFY] [DronePartData.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Core/Data/DronePartData.cs)
- Añadir campos térmicos faltantes al ScriptableObject:
  - `operatingTempMin` (float) — temp mínima de operación
  - `thermalHover` (float) — temp en hover estable
  - `thermalPeak` (float) — temp pico bajo carga máxima
  - `thermalProfile` (string) — "cold" / "mild" / "warm" / "hot" / "gradient"
  - `thermalWarmupSeconds` (float) — tiempo que tarda en alcanzar temp pico desde reposo
- Añadir campos para hotspots:
  - `isHotspotTarget` (bool)
  - `hotspotLabel` (string) — etiqueta para hotspot agrupado
  - `hotspotGroupId` (string) — para agrupar piezas bajo un mismo hotspot

#### [NEW] [PartDataImporter.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Editor/PartDataImporter.cs)
- Script de Editor que lee `x500v2_parts_data.json`
- Genera un `DronePartData` asset por cada entrada del JSON en `Assets/Data/Parts/`
- Mapea todos los campos (incluyendo arrays como `requiredTools`, `safetyWarnings`, `prerequisites`)
- Menú: `Tools > Import Part Data from JSON`

---

## Fase 1: Pipeline de Retopología y LODs

### Flujo de trabajo (independiente de herramienta)

```
CAD (.STEP) → Herramienta 3D (retopología manual) → LODs automatizados → Export FBX → Unity Import
```

### Convenciones de Naming (CRÍTICO para automatización)

Cada mesh exportada debe seguir esta convención:

```
{id}_LOD{N}.fbx
```

Ejemplos:
- `x500v2_bottom_plate_LOD0.fbx` (full detail, ~2000 tris)
- `x500v2_bottom_plate_LOD1.fbx` (~1000 tris)
- `x500v2_bottom_plate_LOD2.fbx` (~400 tris)
- `x500v2_motor_FL_LOD0.fbx` (full, ~1500 tris)

El `id` debe coincidir **exactamente** con el campo `id` del JSON.

### Opción A: Blender

1. Importar `.STEP` vía add-on CAD (o convertir con FreeCAD → `.obj`)
2. Retopología manual de cada pieza con nombres de objeto = `id` del JSON
3. Script Python que genera LODs via Decimate modifier:
   - LOD0: mesh manual
   - LOD1: Decimate ratio 0.5
   - LOD2: Decimate ratio 0.2
4. Export batch FBX por pieza y LOD

### Opción B: Houdini

1. SOP Import `.STEP` (vía File SOP o conversión previa)
2. Retopología manual en Model SOP
3. PolyReduce SOP paramétrico para LODs automáticos
4. ROP FBX Output por pieza con naming convention

### Target de polígonos (presupuesto total: < 50,000 tris)

| Pieza | LOD0 | LOD1 | LOD2 |
|:---|:---:|:---:|:---:|
| Plates (×2) | 500 | 250 | 100 |
| Arms (×4) | 300 | 150 | 60 |
| Landing Gear | 800 | 400 | 160 |
| Platform Board | 200 | 100 | 40 |
| Railes + Battery Mount | 400 | 200 | 80 |
| PDB | 300 | 150 | 60 |
| Power Module | 200 | 100 | 40 |
| Pixhawk 6C | 1500 | 750 | 300 |
| GPS M10 + mástil | 800 | 400 | 160 |
| Radio SiK | 300 | 150 | 60 |
| Motores (×4) | 1200 | 600 | 240 |
| ESCs (×4) | 200 | 100 | 40 |
| Hélices (×4) | 600 | 300 | 120 |
| Batería | 400 | 200 | 80 |
| RC Receiver | 200 | 100 | 40 |
| **TOTAL** | **~18,600** | **~9,300** | **~3,720** |

---

## Fase 2: Importación FBX y Configuración LOD en Unity

#### [NEW] [DroneModelSetup.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Editor/DroneModelSetup.cs)
- Script de Editor que busca todos los FBX importados en `Assets/Models/X500V2/`
- Para cada pieza (por `id`):
  - Crea un `GameObject` con componentes `LODGroup`, `ExplodablePart`, `MaterialController`, `HighlightSystem`
  - Configura 3 niveles LOD (100% → 50% → 20% → Cull)
  - Asigna automáticamente el `DronePartData` asset correspondiente (matching por `id`)
- Construye la jerarquía de montaje según `assemblyOrder`
- Configura pivotes basándose en `explosionDirection`
- Menú: `Tools > Setup Drone Model`

### Jerarquía resultante en Unity

```
X500V2_Root
├── Structure
│   ├── x500v2_bottom_plate (LODGroup + ExplodablePart)
│   ├── x500v2_arm_FL (LODGroup + ExplodablePart)
│   │   ├── x500v2_motor_FL
│   │   │   └── x500v2_prop_FL
│   │   └── x500v2_esc_FL
│   ├── x500v2_arm_FR (...)
│   ├── x500v2_arm_BL (...)
│   ├── x500v2_arm_BR (...)
│   ├── x500v2_landing_gear
│   ├── x500v2_top_plate
│   ├── x500v2_platform_board
│   │   └── x500v2_gps_m10
│   └── x500v2_rails_battery
│       └── x500v2_battery
├── Electronics
│   ├── x500v2_pdb
│   ├── x500v2_power_module
│   ├── x500v2_pixhawk6c
│   ├── x500v2_telemetry_radio
│   └── x500v2_rc_receiver
└── DroneStateController (componente)
```

---

## Fase 3: Materiales PBR

Crear un set mínimo de materiales URP/Lit compartidos:

| Material | Piezas que lo usan | Aspecto |
|:---|:---|:---|
| `M_CarbonFiber` | Plates, Arms, Landing Gear, Platform, Railes | Twill weave normal map, roughness ~0.3, metallic 0 |
| `M_AluminumCNC` | Motores (carcasa), Pixhawk carcasa | Metallic 0.9, roughness ~0.2, micro-scratches NM |
| `M_PCB_Green` | PDB, ESCs, Power Module | Verde oscuro, micro-roughness, pistas de cobre dorado |
| `M_PlasticMatte` | GPS carcasa, Radio, Receiver, Hélices, conectores nylon | Low metallic, roughness ~0.6 |
| `M_BatteryWrap` | Batería | Roughness ~0.5, etiqueta como albedo si se desea |
| `M_Copper` | Bobinados motor, pistas PDB (sub-material) | Metallic 0.95, color cobre |

Cada material se asigna automáticamente por `DroneModelSetup.cs` usando el campo `materialType` del JSON como lookup key.

---

## Fase 4: Hotspots Selectivos y Agrupados

### Problema actual
`HotspotManager.SpawnHotspots()` crea un hotspot por **cada** `ExplodablePart` (28 pines = demasiado ruido visual).

### Solución

#### [MODIFY] [HotspotManager.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/HotspotManager.cs)
- Cambiar `SpawnHotspots()` para que solo cree hotspots en piezas donde `partData.isHotspotTarget == true`
- Cuando un hotspot representa un **grupo**, al hacer click seleccionar todas las piezas del grupo (usando `hotspotLabel` como key de agrupación)

#### [MODIFY] [SmartHotspot.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/SmartHotspot.cs)
- Añadir soporte para `groupParts: ExplodablePart[]` (piezas adicionales del grupo)
- Al hacer click: si tiene grupo, seleccionar la pieza principal y destacar el grupo completo
- Usar `hotspotLabel` en vez de `target.name` para la etiqueta del tooltip

### Hotspots propuestos (5 pines)

| Hotspot | Pieza principal | Grupo (piezas incluidas) |
|:---|:---|:---|
| **Flight Controller** | Pixhawk 6C | Power Module, SiK Radio, RC Receiver |
| **Propulsion System** | Motor FL | 4 Motores + 4 ESCs + 4 Hélices |
| **GPS & Compass** | GPS M10 | Platform Board |
| **Power Distribution** | PDB | — (pieza única) |
| **Battery** | Batería | Rail System |

---

## Fase 5: Thermal Shader — Animación de Calentamiento Progresivo

### Comportamiento deseado
1. Dron apagado → todas las piezas en azul frío (temperatura ambiente)
2. El usuario pulsa "Power On" (`DroneStateController.TurnOn()`)
3. Cada pieza comienza a calentarse progresivamente según su `thermalWarmupSeconds`
4. Al llegar a `DroneState.Idle`: cada pieza alcanza su `thermalHover` temperature
5. Si pasa a `DroneState.Flying`: las piezas calientes (`hot/warm`) escalan hasta `thermalPeak`
6. Al apagar: todo vuelve gradualmente a azul

### Cambios necesarios

#### [MODIFY] [Thermal.shader](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Shaders/Thermal.shader)
- Añadir propiedad `_HeatFactor` (0–1) per-material, controlada por `MaterialPropertyBlock`
- Reemplazar `lerp(_MinTemp, _MaxTemp, baseHeat)` por `lerp(_MinTemp, _MaxTemp, baseHeat * _HeatFactor)`
- `_HeatFactor = 0` → todo frío (azul) | `_HeatFactor = 1` → temperatura objetivo

#### [NEW] [ThermalAnimator.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Core/Content/ThermalAnimator.cs)
- Se añade como componente a cada `ExplodablePart`
- Lee del `DronePartData`: `thermalHover`, `thermalPeak`, `thermalWarmupSeconds`, `thermalProfile`
- Escucha `DroneStateController.OnStateChanged`:
  - `Off` → target _HeatFactor = 0
  - `StartingUp` → target = hoverFactor (gradual con curva según `thermalWarmupSeconds`)
  - `Idle` → target = hoverFactor
  - `Flying` → target = peakFactor (usando una rampa escalada)
  - `ShuttingDown` → target = 0 (cooldown gradual)
- Aplica `_HeatFactor` vía `MaterialPropertyBlock` por frame
- Calcula el valor normalizado: `hoverFactor = (thermalHover - ambientTemp) / (peakTemp - ambientTemp)`

#### [MODIFY] [DroneStateController.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Core/Managers/DroneStateController.cs)
- Publicar `OnStateChanged` con `DroneState` (ya lo hace ✅)
- Asegurar que los estados `StartingUp` y `ShuttingDown` duren lo suficiente para la animación térmica

---

## Fase 6: Filtrado por Categoría

### Estado actual
`ExplodedViewManager.SetCategoryFilters()` ya existe y filtra por `part.Data.category`. Las categorías del JSON coinciden con `PartCatalogManager`:
- `Structure`, `Propulsion`, `Electronics`

Solo necesita verificarse que la UI de filtros conecta correctamente con estas categorías. No se requieren cambios de código si la UI ya está ligada.

---

## Resumen de Archivos

| Acción | Archivo | Complejidad |
|:---|:---|:---:|
| **MODIFY** | `DronePartData.cs` | Baja |
| **NEW** | `PartDataImporter.cs` (Editor) | Media |
| **NEW** | `DroneModelSetup.cs` (Editor) | Media-Alta |
| **MODIFY** | `HotspotManager.cs` | Media |
| **MODIFY** | `SmartHotspot.cs` | Media |
| **MODIFY** | `Thermal.shader` | Media |
| **NEW** | `ThermalAnimator.cs` | Media |
| **MODIFY** | `DroneStateController.cs` | Baja |
| **DATA** | `x500v2_parts_data.json` | ✅ Listo |

---

## Verification Plan

### Automated Tests
- Ejecutar test existente: `Unity Editor > Window > Test Runner > DronePartDataTests` — verificar que los campos nuevos no rompen los tests existentes
- Se podría añadir un test para los campos térmicos nuevos (verificar que `thermalPeak >= thermalHover >= 0`)

### Manual Verification (por el usuario en Unity)
1. **JSON Import:** Ejecutar `Tools > Import Part Data from JSON` → verificar que se crean 28 `.asset` files en `Assets/Data/Parts/`
2. **LOD Setup:** Tras importar FBX, ejecutar `Tools > Setup Drone Model` → verificar jerarquía y LODGroup en el Inspector
3. **Hotspots:** Entrar en Play Mode → verificar que solo aparecen 5 pines (no 28) y que al hacer click en "Propulsion System" se seleccionan motores+ESCs+hélices
4. **Thermal Heat-up:** En Play Mode con vista Thermal activa → pulsar Power On → verificar que los motores se calientan progresivamente (azul → rojo) y que las placas estructurales permanecen frías
5. **Filtered View:** Usar los botones de filtro de categoría → verificar que Structure/Propulsion/Electronics se ocultan/muestran correctamente

> [!TIP]
> Los pasos 1–2 son prerequisitos. Los pasos 3–5 requieren primero completar la importación de meshes retopologizadas y la configuración de la escena.
