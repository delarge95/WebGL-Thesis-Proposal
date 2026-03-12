# FASE 3 — Plan de Trabajo (RAG Temporal)

> Generado: 2 marzo 2026  
> Branch: `feature/phase2-ux-redesign`  
> Contexto: Todas las tareas de FASE 1 (C01-C05) y FASE 2 (H01-H11) completadas.

---

## Auditoría: Estado actual FASE 3

### ✅ YA COMPLETADOS (no requieren trabajo)

| ID        | Descripción                          | Evidencia                         |
| --------- | ------------------------------------ | --------------------------------- |
| PERF-M01  | Exception support → ExplicitlyThrown | `webGLExceptionSupport: 1`        |
| PERF-M06  | Shader variant stripping             | `ShaderVariantStripper.cs` activo |
| PERF-M03★ | Mip stripping                        | `mipStripping: 1`                 |
| PERF-M04★ | Hash naming                          | `webGLNameFilesAsHashes: 1`       |
| UX-M09    | Sheet handle + drag-to-dismiss       | `sheet-handle` + threshold 50px   |
| ARCH-M01  | Cleanup/unsubscribe patterns         | `AddCleanup()` estandarizado      |
| ARCH-M08  | QualityManager funcional             | `AdjustQuality()` + renderScale   |

### ⚠️ PARCIALMENTE HECHOS

| ID       | Descripción         | Pendiente                                   |
| -------- | ------------------- | ------------------------------------------- |
| UX-M01   | Grid 4pt            | Normalizar 3px→4px, 6px→8px, 13px→14px      |
| UX-H06   | Type ramp           | Documentar 24px y 36px en ramp              |
| UX-M08   | Escape/Back         | Solo MeasurementTool. Falta: sheet, modos   |
| ARCH-M04 | Camera input        | Falta: magic numbers en touch scaling       |
| ARCH-M06 | Magic numbers       | Falta: constantes en OrbitCameraController  |
| UX-H05★  | Transitions         | 50+ transitions OK. Falta: feedback de modo |
| UX-M07★  | Selection indicator | Funciona. Verificar edge cases              |

### ❌ DESCARTADOS / NO APLICABLES

| ID                            | Razón                         |
| ----------------------------- | ----------------------------- |
| UX-M02★ (Dark/light)          | Decisión de diseño: dark-only |
| UX-M03★ (Hero nav mismatch)   | Dos paradigmas intencionales  |
| UX-M04 (Filter badges)        | Se definirán con modelo real  |
| UX-M05★ (Cross-section guide) | Baja prioridad                |
| PERF-M02★ (ASTC textures)     | Requiere assets definitivos   |
| PERF-M05★ (LOD groups)        | Bloqueado por assets          |

---

## Commits planificados

### F3-00: Hotspot fix + Slider restyle

**Archivos**: `SmartHotspot.cs`, `UIManager.cs`, `Theme.uss`

**Hotspot — corrección de comportamiento**:

- Si el panel de info está abierto y se clickea un hotspot → NO cerrar el panel, solo cambiar la selección (el panel se actualiza con la nueva pieza).
- Si se hace doble-click sobre un hotspot → mismo comportamiento que single-click (selección + panel abierto) PERO además aislar (isolate) la pieza/piezas que representa el hotspot.
- Implementación: agregar detección de doble-click en `SmartHotspot.OnClick()` que publique un flag, y en `UIManager.OnPartSelected()` respetar el estado abierto del sheet cuando `fromHotspot=true`.

**Slider — rediseño del dragger**:

- Normal: solo stroke blanco (`border-width: 2px`, `border-color: rgba(255,255,255,0.6)`), fondo negro (`background-color: rgb(0,0,0)`), 20px.
- Hover: crece a 28px, completamente blanco (`background-color: white`, `border-width: 0`).
- Active/Click: cambia tamaño a 22px, color azul selección (`rgba(70, 175, 255, 1)`), sin borde.

---

### F3-01: Studio — Render Mode simplificado

**Archivos**: `MainLayout.uxml`, `UIAnalyzePanel.cs`

**Cambios**:

- Ocultar card REALISTIC (es el default, no necesita botón).
- Mantener visibles: **X-RAY**, **THERMAL** (habilitar, actualmente `display: none`), **SOLID**.
- Comportamiento toggle: click activa el modo, re-click lo desactiva (vuelve a Realistic).
- La card activa se marca con `submenu-card--active`.
- `UIAnalyzePanel.BindShaderMenuButtons()`: modificar lógica para que si el modo seleccionado ya es el activo, vuelva a `ViewMode.Realistic`.

---

### F3-02: Studio — Environment ciclos

**Archivos**: `MainLayout.uxml`, `UIEnvironmentPanel.cs`, `EnvironmentController.cs`

**Cambios**:

- **STUDIO**: permanece igual (iluminación studio, click normal).
- **NIGHT** → re-nombrar card a **TIME**. Click cicla entre: Day → Night → Sunset → Day...
  - El label de la card se actualiza dinámicamente: "DAY", "NIGHT", "SUNSET".
- **BLUE** → re-nombrar card a **COLOR**. Click cicla entre: White → Grey → Black → Yellow → Orange → Green → Blue → Purple → Red → White...
  - El label de la card se actualiza dinámicamente con el color actual.
- Solo puede haber un preset activo. Si se selecciona otro, el anterior se desactiva.
- `EnvironmentController.ApplyPreset()` ya maneja los presets. Añadir nuevos presets: Day, Sunset, y los fondos de color plano.

---

### F3-03: Filter — 6ª categoría

**Archivos**: `MainLayout.uxml`, `UIManager.cs`

**Cambios**:

- Añadir `CatBtn_Payload` (texto "PAYLOAD") como 6ª card de filtro.
- Esto completa la grid 3×2 (evita cards centradas de a 2).
- Bind en UIManager: `BindCat("CatBtn_Payload", "Payload")`.
- Los filtros se definirán con el modelo real; por ahora placeholder.

---

### F3-04: Inspect — Info → FAB global

**Archivos**: `MainLayout.uxml`, `Theme.uss`, `InspectModeHandler.cs`, `UIManager.cs`

**Cambios UXML**:

- Extraer `ToolInfoBtn` del `ToolsCardGrid`.
- Crear `<ui:VisualElement name="GlobalActionContainer">` como hermano de `BottomBar`.
- Posición absoluta: `bottom: 120px; right: 24px;`.
- Mover `ToolInfoBtn` allí, re-estilizado como FAB circular (`.fab-button`).

**Cambios USS**:

- `.fab-container`: position absolute, bottom 120px, right 24px.
- `.fab-button`: 56×56px circular, glass background, border sutil, box-shadow.
- `.fab-button:hover`: scale 1.05, border más visible.
- `.fab-button--hidden`: opacity 0, pointer-events none.

**Cambios C#**:

- `InspectModeHandler`: eliminar binding de `ToolInfoBtn` (ya no está en el grid).
- `UIManager`: bindear `ToolInfoBtn` (ahora global) directamente. Mostrar/ocultar FAB según haya selección activa.
- FAB persiste en todos los modos (Inspect, Analyze, Studio) si hay pieza seleccionada.

---

### F3-05: Inspect — Añadir MEASURE

**Archivos**: `MainLayout.uxml`, `InspectModeHandler.cs`

**Cambios**:

- Añadir `ToolMeasureBtn` como nueva card en Inspect (ya existe `MeasurementTool.cs`).
- Inspect queda: **PINS**, **ISOLATE**, **MEASURE** (3 cards, grid 3×1).
- `InspectModeHandler`: bindear `ToolMeasureBtn` → toggle `MeasurementTool`.

---

### F3-06: PERF-M02 — Memoria WebGL

**Archivos**: `ProjectSettings/ProjectSettings.asset`

**Cambios**:

- `webGLMaximumMemorySize`: 512 → 256.
- `webGLMemoryGrowthMode: 2` (auto-grow) ya está activo, lo que protege contra OOM.

---

### F3-07: Grid 4pt + Type ramp completar

**Archivos**: `Theme.uss`

**Cambios**:

- Normalizar: `3px → 4px`, `6px → 8px`, `13px → 14px`.
- Documentar en el type ramp comment: `Sheet-title: 24px`, `Panel-title: 36px`.

---

### F3-08: Slider de explosión — label dinámico

**Archivos**: `MainLayout.uxml`, `UIManager.cs` o handler de Analyze

**Cambios**:

- Añadir `<ui:Label name="ExplosionValue" text="0%" class="env-slider-value"/>` junto al slider.
- En el callback `OnExplosionSliderChanged`: actualizar label con `$"{(evt.newValue * 100):F0}%"`.

---

### F3-09: Escape key global

**Archivos**: `UIManager.cs`

**Cambios**:

- En `Update()` o InputSystem: detectar `KeyCode.Escape`.
- Prioridad de cierre: bottom sheet → submenú activo → modo activo → nada.
- Usar `_detailsSheet.IsSheetOpen` y `_modeController` state.

---

### F3-10: Camera magic numbers → constantes

**Archivos**: `OrbitCameraController.cs`

**Qué son magic numbers**: valores numéricos literales dentro del código (ej. `0.2f`, `5f`, `100f`) que no tienen nombre ni explicación. Se extraen a constantes con nombres descriptivos para mejorar legibilidad y mantenimiento.

**Cambios**:

- Extraer: `TOUCH_ORBIT_SCALE = 0.2f`, `TOUCH_PAN_SCALE = 0.01f`, `TOUCH_ZOOM_SCALE = 0.5f`, `MOUSE_SCROLL_SCALE = 5f`, `RAYCAST_MAX_DISTANCE = 100f`, `DEFAULT_TARGET_Y = 20f`, `DEFAULT_DISTANCE = 10f`, `ISOLATED_DISTANCE = 6f`, `CLOSE_DISTANCE = 5f`.

---

### F3-11: Double-click unificado

**Archivos**: `SelectionManager.cs`, `UIManager.cs`

**Cambios**:

- Mover lógica de detección de doble-click de `UIManager` a `SelectionManager`.
- `SelectionManager` publica `PartDoubleClickedEvent` (nuevo evento).
- `UIManager` suscribe: doble-click → isolate + open sheet.
- **Nota**: doble-click es para aislar Y mostrar info (ambas acciones juntas).

---

### F3-12: DronePartData limpieza

**Archivos**: `DronePartData.cs`

**Cambios**:

- Eliminar properties redundantes (`weight`, `material`, `PartName`).
- Dejar solo campos públicos directos.

---

### F3-13: WebGL template custom

**Archivos**: `Assets/WebGLTemplates/Custom/index.html`

**Cambios**:

- Crear template con progress bar personalizada y splash.
- Actualizar `ProjectSettings` para usar template custom.

---

## Orden de ejecución

```
F3-00  Hotspot fix + Slider restyle         ← INMEDIATO
F3-01  Render Mode toggle                   ← Studio cards
F3-02  Environment ciclos                   ← Studio cards
F3-03  Filter 6ª categoría                  ← Analyze cards
F3-04  Info → FAB global                    ← Inspect restructure
F3-05  Inspect + MEASURE                    ← Inspect restructure
F3-06  Memoria 256MB                        ← ProjectSettings
F3-07  Grid 4pt + Type ramp                 ← CSS polish
F3-08  Explosion label dinámico             ← UX polish
F3-09  Escape key                           ← Input
F3-10  Magic numbers                        ← Arch cleanup
F3-11  Double-click unificado               ← Arch cleanup
F3-12  DronePartData                        ← Arch cleanup
F3-13  WebGL template                       ← Build
```

## Dependencias

```
F3-04 (Info→FAB) ──→ F3-05 (Inspect +MEASURE)
F3-00 (Hotspot fix) ──→ F3-11 (Double-click unificado)
El resto son independientes entre sí.
```
