# Cross-Reference: Blender → Unity — Holybro X500 V2

**Fecha:** 2026-04-01  
**Fuente Blender:** Colecciones `subcategories/` (working) + `temp/` (proxies pendientes)  
**Fuente Unity:** 28 nodos canónicos de `RETOPOLOGIA_POR_PIEZA.md`

> Las colecciones `Just Imported`, `Ngons`, `HaveToRemake`, y `Cleaned` son copias de trabajo/respaldo y **no se exportan**. Solo se listan las piezas en `subcategories/` que son las definitivas de trabajo.

---

## Leyenda

| Símbolo | Significado |
|---|---|
| ✅ | Presente en ambos (Blender + Unity naming definido) |
| 🟡 PROXY | Pieza temporal/blocking — necesita modelo real |
| ⚠️ NO EN UNITY | Pieza en Blender sin nodo canónico Unity equivalente (hereda del padre) |
| 🔴 FALTA EN BLENDER | Nodo canónico Unity que no tiene pieza en Blender |

---

## 01 — Avionics (subcategories/Electronics/FlightController)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `DIKE-PIXHAWK6C-LV-C1` | `x500v2_pixhawk6c` (sub-part: top cover) | ✅ Merge → `x500v2_pixhawk6c` |
| 2 | `IMU-PIXHAWK6C` | `x500v2_pixhawk6c` (sub-part: IMU) | ✅ Merge → `x500v2_pixhawk6c` |
| 3 | `MIANKE-PIXHAWK6C-LV-C1` | `x500v2_pixhawk6c` (sub-part: base shell) | ✅ Merge → `x500v2_pixhawk6c` |
| 4 | `PCB-PIXHAWK6C-F1` | `x500v2_pixhawk6c` (sub-part: main PCB) | ✅ Merge → `x500v2_pixhawk6c` |

> **Instrucción Unity:** Las 4 piezas del Pixhawk se exportan como hijas de un empty `x500v2_pixhawk6c` o se fusionan en un solo mesh. El solver térmico ve un único nodo `x500v2_pixhawk6c`.

---

## 02 — Sensors & Comms (subcategories/Electronics/GPS + temp/)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `GAN-GPSV5-ZHIJIA` | `x500v2_gps_m10` (sub: antenna) | ✅ Merge → `x500v2_gps_m10` |
| 2 | `GPS-ZHIJIA-ZHUANJIETOU` | `x500v2_gps_m10` (sub: mast joint) | ✅ Merge → `x500v2_gps_m10` |
| 3 | `GPS-ZHIJIA-ZUO` | `x500v2_gps_m10` (sub: mast base) | ✅ Merge → `x500v2_gps_m10` |
| 4 | `GPSV5-ZHIJIA-LUOMAO` | `x500v2_gps_m10` (sub: securing nut) | ✅ Merge → `x500v2_gps_m10` |
| 5 | `GPSV5-ZHIJIA-TUOPAN` | `x500v2_gps_m10` (sub: top tray) | ✅ Merge → `x500v2_gps_m10` |
| 6 | `x500v2_gps_m10_PROXY` *(temp/Electronics_GPS)* | `x500v2_gps_m10` | 🟡 PROXY — tiene 1,282 verts, sustituir por merge de las 5 anteriores |
| 7 | `x500v2_telemetry_radio_PROXY` *(temp/Electronics_Radio)* | `x500v2_telemetry_radio` | 🟡 PROXY (8 verts) — necesita modelo real |
| 8 | `x500v2_rc_receiver_PROXY` *(temp/Electronics_Receiver)* | `x500v2_rc_receiver` | 🟡 PROXY (8 verts) — necesita modelo real |

---

## 03 — Power Distribution (subcategories/Electronics/PDB + PowerModule + temp/)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `BM06B-WO` *(PDB)* | `x500v2_pixhawk6c` (sub: JST connector) | ✅ Merge → `x500v2_pixhawk6c` ó ⚠️ puede ir a `x500v2_pdb` |
| 2 | `PCB-PM06` *(PDB)* | `x500v2_pdb` or `x500v2_power_module` | ✅ → `x500v2_power_module` (es la PCB del PM06) |
| 3 | `TOU-XT60H-M-14AWG` *(PowerModule)* | `x500v2_power_module` (sub: XT60 plug) | ✅ Merge → `x500v2_power_module` |
| 4 | `X500-TAO-XT60` *(PowerModule)* | `x500v2_power_module` (sub: XT60 holder) | ✅ Merge → `x500v2_power_module` |
| 5 | `x500v2_battery_PROXY` *(temp/Electronics_Battery)* | `x500v2_battery` | 🟡 PROXY (8 verts) — necesita modelo real |
| 6 | `x500v2_power_module_PROXY` *(temp/Electronics_PowerModule)* | `x500v2_power_module` | 🟡 PROXY (8 verts) — sustituir por merge de PCB-PM06 + XT60 |
| — | 🔴 **FALTA** | `x500v2_pdb` | 🔴 Sin pieza en Blender — necesita modelo real o proxy |

---

## 04 — Propulsion System (subcategories/Propulsion/Motor + temp/)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `DJ-2216-KV880` *(Motor, 163K verts!)* | `x500v2_motor_FL` | ✅ Instanciar ×4 → `_FL`, `_FR`, `_BL`, `_BR` |
| 2 | `x500v2_prop_FL` *(temp/Propulsion_Propeller)* | `x500v2_prop_FL` | 🟡 PROXY (48 verts) — necesita modelo real |
| 3 | `x500v2_prop_FR` *(temp/Propulsion_Propeller)* | `x500v2_prop_FR` | 🟡 PROXY (48 verts) — necesita modelo real |
| 4 | `x500v2_prop_BL` *(temp/Propulsion_Propeller)* | `x500v2_prop_BL` | 🟡 PROXY (48 verts) — necesita modelo real |
| 5 | `x500v2_prop_BR` *(temp/Propulsion_Propeller)* | `x500v2_prop_BR` | 🟡 PROXY (48 verts) — necesita modelo real |
| — | 🔴 **FALTA** | `x500v2_esc_FL` | 🔴 Sin pieza — necesita modelo × 4 |
| — | 🔴 **FALTA** | `x500v2_esc_FR` | 🔴 Sin pieza — necesita modelo × 4 |
| — | 🔴 **FALTA** | `x500v2_esc_BL` | 🔴 Sin pieza — necesita modelo × 4 |
| — | 🔴 **FALTA** | `x500v2_esc_BR` | 🔴 Sin pieza — necesita modelo × 4 |

---

## 05 — Skeleton / Airframe (subcategories/Structure/)

### 05a — Frame (subcategories/Structure/Frame)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `BOTTOM-PLATE-X500-V5` | `x500v2_bottom_plate` | ✅ Rename |
| 2 | `TOP-PLATE-X500-V5` | `x500v2_top_plate` | ✅ Rename |
| 3 | `PLATFORM-PLAT-X500` | `x500v2_platform_board` | ✅ Rename |
| 4 | `BATTERY-MOUNTING-PLAT` | `x500v2_rails_battery` (sub: mounting plate) | ✅ Merge → `x500v2_rails_battery` |
| 5 | `BATTERY-PAD` | `x500v2_rails_battery` (sub: silicone pad) | ✅ Merge → `x500v2_rails_battery` |
| 6 | `PYLONS-X500` | `x500v2_rails_battery` (sub: carbon rails) | ✅ Merge → `x500v2_rails_battery` |
| 7 | `GUAN-CHENG` | `x500v2_rails_battery` (sub: rail clip) | ✅ Merge → `x500v2_rails_battery` |
| 8 | `HUAN-GUIJIAO` (73K verts!) | ⚠️ Silicone dampener — hereda de padre | ⚠️ NO EN UNITY — hereda `x500v2_bottom_plate` |
| 9 | `JIA-GUAN` | ⚠️ Cable clamp — hereda de padre | ⚠️ NO EN UNITY — hereda padre más cercano |
| 10 | `JIA-LIANJIE` | ⚠️ Frame clip — hereda de padre | ⚠️ NO EN UNITY — hereda padre más cercano |
| 11 | `GAI-GUANGLIU` | ⚠️ Optical flow cover — hereda | ⚠️ NO EN UNITY — hereda `x500v2_bottom_plate` |
| 12 | `ZHIJIA-CAMERA-INTEL` | ⚠️ Camera bracket — hereda | ⚠️ NO EN UNITY — hereda `x500v2_bottom_plate` |

### 05b — Arm (subcategories/Structure/Arm)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `CARBON-FIBER-TUBE300` | `x500v2_arm_*` (sub: carbon tube) | ✅ Instanciar ×4 → merge con arm assembly |
| 2 | `HMX5V-DIGAI-DIANJIZUO-MUJU` (72K!) | `x500v2_arm_*` (sub: motor mount bottom) | ✅ Instanciar ×4 → merge con arm assembly |
| 3 | `HMX5V-GUAN-DINGWEI` | `x500v2_arm_*` (sub: tube stopper) | ✅ Instanciar ×4 → merge con arm assembly |
| 4 | `HMX5V-JIBI-JIA-MUJU` (100K!) | `x500v2_arm_*` (sub: frame clamp) | ✅ Instanciar ×4 (×2 per arm) → merge con arm |
| 5 | `HMX5V-ZUO-DJ-MUJU` (58K!) | `x500v2_arm_*` (sub: motor mount top) | ✅ Instanciar ×4 → merge con arm assembly |
| 6 | `BAN-DJ-DIAN-F2` | `x500v2_arm_*` (sub: motor base plate) | ✅ Instanciar ×4 → merge con arm assembly |

> **⚠️ El ensamblaje Arm tiene ~255K verts por instancia antes de retopología.** Necesita decimación agresiva.

### 05c — Landing Gear (subcategories/Structure/Landing)

| # | Nombre en Blender | Nodo Canónico Unity | Estado |
|---|---|---|---|
| 1 | `CARBON-FIBER-TUBE` | `x500v2_landing_gear` (sub: main tube) | ✅ Merge → `x500v2_landing_gear` |
| 2 | `JIAO-EVA` (46K verts!) | `x500v2_landing_gear` (sub: EVA pad) | ✅ Merge → `x500v2_landing_gear` |
| 3 | `JIAO-LIANJIE` | `x500v2_landing_gear` (sub: end cap) | ✅ Merge → `x500v2_landing_gear` |
| 4 | `MAO-JIAO` | `x500v2_landing_gear` (sub: T-connector) | ✅ Merge → `x500v2_landing_gear` |

---

## 06 — Fasteners (subcategories/Structure/Fastener + temp/)

| # | Nombre en Blender | Nodo Canónico Unity | Estado | Verts |
|---|---|---|---|---|
| 1 | `GB70-M25-10` | ⚠️ hereda padre | ⚠️ No nodo propio | 158,004 |
| 2 | `GB70-M25-12` | ⚠️ hereda padre | ⚠️ No nodo propio | 277,272 |
| 3 | `GB70-M25-6` | ⚠️ hereda padre | ⚠️ No nodo propio | 287,702 |
| 4 | `GB70-M3-21-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 51,475 |
| 5 | `GB70-M3-25-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 66,370 |
| 6 | `GB70-M3-38` | ⚠️ hereda padre | ⚠️ No nodo propio | **814,784** |
| 7 | `GB70-M3-6` | ⚠️ hereda padre | ⚠️ No nodo propio | 196,284 |
| 8 | `GB70-M3-8-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 138,525 |
| 9 | `LM-M3-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 30,791 |
| 10 | `LM-M3-NILONG` | ⚠️ hereda padre | ⚠️ No nodo propio | 7,190 |
| 11 | `M25-6-CHEN-LIU` | ⚠️ hereda padre | ⚠️ No nodo propio | 149,295 |
| 12 | `M3-10-PAN-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 57,995 |
| 13 | `M3-14-PAN` | ⚠️ hereda padre | ⚠️ No nodo propio | 105,060 |
| 14 | `M3-16-CHEN-LIU` | ⚠️ hereda padre | ⚠️ No nodo propio | 55,102 |
| 15 | `NILONGZHU-M25-5` | ⚠️ hereda padre | ⚠️ No nodo propio | 24,632 |
| 16 | `NILONGZHU-M3-5` | ⚠️ hereda padre | ⚠️ No nodo propio | 23,804 |
| 17 | `ZSLM-M25` | ⚠️ hereda padre | ⚠️ No nodo propio | 38,663 |
| 18 | `ZSLM-M3-DING` | ⚠️ hereda padre | ⚠️ No nodo propio | 77,317 |
| 19 | `ZSLM-M3-FALAN` | ⚠️ hereda padre | ⚠️ No nodo propio | 190,072 |
| 20 | `x500v2_battery_strap_1_PROXY` *(temp/Structure_Fastener)* | ⚠️ hereda `x500v2_battery` | 🟡 PROXY (8 verts) |  |
| 21 | `x500v2_battery_strap_2_PROXY` *(temp/Structure_Fastener)* | ⚠️ hereda `x500v2_battery` | 🟡 PROXY (8 verts) |  |

> **⚠️ ALERTA DE VÉRTICES:** Los fasteners suman **~2.47 MILLONES** de vértices en `subcategories/`. Necesitan decimación extrema (>95%) o ser reemplazados por instancias de meshes simplificados.

---

## Resumen: 28 Nodos Canónicos Unity — Estado de Cobertura

| # | Nodo Canónico Unity | Piezas Blender Mapeadas | Estado |
|---|---|---|---|
| 1 | `x500v2_bottom_plate` | `BOTTOM-PLATE-X500-V5` | ✅ Rename |
| 2 | `x500v2_top_plate` | `TOP-PLATE-X500-V5` | ✅ Rename |
| 3 | `x500v2_arm_FL` | Tube + mounts + clamps (×1 instance) | ✅ Merge + instance |
| 4 | `x500v2_arm_FR` | ↑ same meshes, different position | ✅ Merge + instance |
| 5 | `x500v2_arm_BL` | ↑ | ✅ Merge + instance |
| 6 | `x500v2_arm_BR` | ↑ | ✅ Merge + instance |
| 7 | `x500v2_landing_gear` | Tube + EVA + end caps + T-connectors | ✅ Merge |
| 8 | `x500v2_platform_board` | `PLATFORM-PLAT-X500` | ✅ Rename |
| 9 | `x500v2_rails_battery` | Mounting plate + pad + pylons + clips | ✅ Merge |
| 10 | `x500v2_pdb` | — | 🔴 **FALTA** |
| 11 | `x500v2_power_module` | `PCB-PM06` + `TOU-XT60H-M-14AWG` + `X500-TAO-XT60` | ✅ Merge |
| 12 | `x500v2_pixhawk6c` | 4 piezas (cover + shell + IMU + PCB) + `BM06B-WO` | ✅ Merge |
| 13 | `x500v2_gps_m10` | 5 piezas (antenna + mast + tray + nut + base) | ✅ Merge |
| 14 | `x500v2_telemetry_radio` | `x500v2_telemetry_radio_PROXY` | 🟡 PROXY only |
| 15 | `x500v2_motor_FL` | `DJ-2216-KV880` (instance) | ✅ Instance |
| 16 | `x500v2_motor_FR` | ↑ | ✅ Instance |
| 17 | `x500v2_motor_BL` | ↑ | ✅ Instance |
| 18 | `x500v2_motor_BR` | ↑ | ✅ Instance |
| 19 | `x500v2_esc_FL` | — | 🔴 **FALTA** |
| 20 | `x500v2_esc_FR` | — | 🔴 **FALTA** |
| 21 | `x500v2_esc_BL` | — | 🔴 **FALTA** |
| 22 | `x500v2_esc_BR` | — | 🔴 **FALTA** |
| 23 | `x500v2_prop_FL` | `x500v2_prop_FL` (PROXY) | 🟡 PROXY only |
| 24 | `x500v2_prop_FR` | `x500v2_prop_FR` (PROXY) | 🟡 PROXY only |
| 25 | `x500v2_prop_BL` | `x500v2_prop_BL` (PROXY) | 🟡 PROXY only |
| 26 | `x500v2_prop_BR` | `x500v2_prop_BR` (PROXY) | 🟡 PROXY only |
| 27 | `x500v2_battery` | `x500v2_battery_PROXY` | 🟡 PROXY only |
| 28 | `x500v2_rc_receiver` | `x500v2_rc_receiver_PROXY` | 🟡 PROXY only |

---

## Dashboard de Cobertura

| Estado | Count | % |
|---|---|---|
| ✅ Con modelo real | 17 | 61% |
| 🟡 Solo PROXY (blocking) | 6 | 21% |
| 🔴 Falta completamente | 5 | 18% |
| **Total** | **28** | 100% |

### Faltantes Críticos (🔴)

1. **`x500v2_pdb`** — PDB (Power Distribution Board). No tiene pieza CAD ni proxy.
2. **`x500v2_esc_FL/FR/BL/BR`** — 4 ESCs. No tienen pieza CAD ni proxy.

### Solo Proxy (🟡) — Necesitan modelo real

1. **`x500v2_battery`** — 8 verts (cube placeholder)
2. **`x500v2_telemetry_radio`** — 8 verts (cube placeholder)
3. **`x500v2_rc_receiver`** — 8 verts (cube placeholder)
4. **`x500v2_prop_FL/FR/BL/BR`** — 48 verts each (disc placeholders)

---

## Piezas Blender SIN nodo Unity (⚠️ heredan temperatura del padre)

| Nombre Blender | Padre térmico sugerido |
|---|---|
| `HUAN-GUIJIAO` (dampeners ×8) | `x500v2_bottom_plate` |
| `JIA-GUAN` (cable clamps ×8) | Padre más cercano |
| `JIA-LIANJIE` (frame clips ×2) | Padre más cercano |
| `GAI-GUANGLIU` (optical flow cover) | `x500v2_bottom_plate` |
| `ZHIJIA-CAMERA-INTEL` (camera bracket) | `x500v2_bottom_plate` |
| 19 tipos de fasteners (tornillos/tuercas) | Padre más cercano |
| `x500v2_battery_strap_1/2_PROXY` | `x500v2_battery` |
