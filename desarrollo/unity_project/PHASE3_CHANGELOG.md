# Phase 3 — Architecture Refactoring: Changelog Detallado

**Proyecto:** WebGL Drone Viewer (Unity 6.0 LTS — UI Toolkit / WebGL)  
**Branch:** `feature/phase3-architecture`  
**Fecha:** 23 de febrero de 2026  
**Autor de los cambios:** GitHub Copilot (agente automatizado)

---

## Resumen Ejecutivo

La Fase 3 abordó tres problemas arquitectónicos críticos en el sistema UI del visor de drones WebGL:

| # | Paso | Problema | Solución | Estado |
|---|------|----------|----------|--------|
| 1 | Memory Leak Prevention | Callbacks de UI Toolkit nunca se des-registraban | Patrón `AddCleanup()` + `UnsubscribeFromUIEvents()` | ✅ Completo |
| 2 | God Class Dismantling | `UIManager.cs` era un monolito de **972 líneas** | Extracción de 3 sub-controladores → reducción a **388 líneas** (~60%) | ✅ Completo |
| 3 | Input Decoupling | `SelectionManager` contenía lógica duplicada de hit-testing UI con conversión de coordenadas incorrecta | Centralización en `InputManager.IsPointerOverUI()` con `RuntimePanelUtils.ScreenToPanel()` | ✅ Completo |

**Resultado de compilación:** ✅ 0 errores en todo el proyecto.

---

## Archivos Modificados

### Resumen de impacto

| Archivo | Ruta | Acción | Líneas Antes | Líneas Después | Δ |
|---------|------|--------|-------------|---------------|---|
| `UIManager.cs` | `Assets/Scripts/UI/` | **Reescrito** | 972 | 388 | −584 (−60%) |
| `InputManager.cs` | `Assets/Scripts/Core/Managers/` | **Reescrito** | ~50 | 135 | +85 |
| `SelectionManager.cs` | `Assets/Scripts/Core/Managers/` | **Modificado** | 335 | 302 | −33 |
| `UIDetailsSheet.cs` | `Assets/Scripts/UI/Panels/` | **Nuevo** | — | 291 | +291 |
| `UIHeroController.cs` | `Assets/Scripts/UI/Panels/` | **Nuevo** | — | 200 | +200 |
| `UIPopupController.cs` | `Assets/Scripts/UI/Panels/` | **Nuevo** | — | 273 | +273 |

**Balance neto:** 972 + 50 + 335 = **1357 líneas antes** → 388 + 135 + 302 + 291 + 200 + 273 = **1589 líneas después** (+232 líneas, pero distribuidas en 6 archivos con responsabilidades claras vs 3 archivos monolíticos).

---

## Paso 1: Memory Leak Prevention

### Problema
Los event handlers de UI Toolkit (`RegisterCallback`, `.clicked +=`) nunca se des-registraban en `OnDisable()`. Esto causaba:
- Suscripciones zombi que persistían entre escenas
- Memory leaks acumulativos en builds WebGL de larga duración
- Comportamientos fantasma (callbacks ejecutándose contra elementos destruidos)

### Solución implementada

**Patrón `AddCleanup` centralizado** en `UIManager.cs` y replicado en cada sub-controlador:

```csharp
// Lista de acciones de limpieza
private List<System.Action> _uiCleanupActions = new List<System.Action>();

// Al registrar cualquier callback:
private void AddCleanup(System.Action cleanupAction)
{
    if (cleanupAction != null) _uiCleanupActions.Add(cleanupAction);
}

// Ejemplo de uso:
EventCallback<PointerEnterEvent> pe = evt => OrbitCameraController.GlobalInputBlocked = true;
element.RegisterCallback(pe);
AddCleanup(() => element.UnregisterCallback(pe));

// En OnDisable() se ejecutan todas:
private void UnsubscribeFromUIEvents()
{
    foreach (var action in _uiCleanupActions) action?.Invoke();
    _uiCleanupActions.Clear();
}
```

### Handlers protegidos

Todos los siguientes handlers ahora tienen su `AddCleanup()` correspondiente:

- **UIManager**: `shaderBtn.clicked`, `explodeBtn.clicked`, `resetBtn.clicked`, `envBtn.clicked`, `layerBtn.clicked`, `hotspotBtn.clicked`, `explosionSlider.RegisterValueChangedCallback`, `explosionSlider PointerEnter/Leave/Down`, cada `CatBtn_*`, cada `Button` (via `RegisterButtonInputBlockers`), todos los sub-controllers `Dispose()`
- **UIDetailsSheet**: `detailsSheet PointerDown/Up/Enter/Leave`, `header ClickEvent`, `handle PointerDown/Up/Leave/Move`, `sheetScroll PointerEnter/Leave`, `sheetCloseBtn ClickEvent`, `infoBtn.clicked`
- **UIHeroController**: `heroExploreBtn.clicked`, `heroDeviceBtn.clicked`, `heroAboutBtn.clicked`, `heroExitBtn.clicked`, `backDevices/About/Exit.clicked`, `exitConfirmBtn.clicked`, `exitCancelBtn.clicked`, `homeBtn.clicked`
- **UIPopupController**: `popupBlocker PointerDownEvent`

---

## Paso 2: God Class Dismantling

### Problema
`UIManager.cs` era una "God Class" de **972 líneas** que mezclaba:
- Gestión del Bottom Sheet (datos de partes, drag-to-dismiss, open/close)
- Sistema de popups (shader menu, category menu, env panel, slider stacking)
- Hero/Landing screen (menú principal, submenús, transiciones)
- Botones de toolbar (explode, reset, shader, env)
- Reacción a AppState + ViewMode
- Inicialización de hotspots
- Auto-creación de managers

### Solución: Extracción de 3 sub-controladores

#### 2.1 — `UIDetailsSheet.cs` (291 líneas, NUEVO)

**Namespace:** `WebGL.UI.Panels`  
**Responsabilidad:** Bottom sheet de detalles de piezas

**Constructor:**
```csharp
public UIDetailsSheet(VisualElement root, Button infoBtn)
```

**API Pública:**

| Método | Descripción |
|--------|-------------|
| `SetSheetState(bool isOpen)` | Abre/cierra el sheet. Maneja clases CSS (`details-sheet--hidden`), picking mode, `ui-shifted` en BottomBar, ocultamiento del label selector, y viewport shift del `OrbitCameraController` |
| `OpenSheet()` | Activa el contenido del sheet (`sheet-content--active`) y llama `SetSheetState(true)` |
| `ToggleInfo()` | Toggle entre abrir/cerrar |
| `PopulatePartData(DronePartData, bool fromHotspot)` | Llena los 12 campos de datos de la pieza. Si `fromHotspot=true`, auto-abre el sheet |
| `UpdatePartIndicator(DronePartData)` | Actualiza el label "SelectionIndicator" con nombre y color |
| `Dispose()` | Limpia todos los callbacks + anula eventos |

**Evento:** `OnSheetStateChanged(bool isOpen)` — UIManager lo conecta a `_popupController.SetSheetOpenState(isOpen)` para que los popups se cierren al abrir el sheet.

**Campos de datos bindeados (12 labels):**
- `SheetTitle`, `PartCategory`, `PartFunction`, `PartMaterial`, `PartDescription`
- `PartWeight`, `PartDimensions`, `PartPower`, `PartTemp`
- `PartDifficulty` (★/☆ system), `PartTools`, `PartAssemblyTime`

**Interacciones vinculadas:**
- `sheet-header` → Click para toggle open/close
- `sheet-handle` → Drag-to-dismiss (threshold: 50px)
- `sheet-scroll` → Bloquea zoom de cámara (`GlobalInputBlocked`)
- `SheetCloseBtn` → Click con `StopPropagation()` para evitar re-toggle
- `BottomSheet` → `StopPropagation()` en PointerDown/Up + `GlobalInputBlocked` en Enter/Leave

---

#### 2.2 — `UIPopupController.cs` (273 líneas, NUEVO)

**Namespace:** `WebGL.UI.Panels`  
**Responsabilidad:** Sistema de popups/submenús con stacking dinámico

**Constructor:**
```csharp
public UIPopupController(
    VisualElement root,
    VisualElement shaderMenu,
    VisualElement categoryMenu,
    VisualElement envPanel,
    VisualElement sliderContainer,
    VisualElement popupBlocker,
    Slider explosionSlider,
    Button hotspotBtn)
```

**API Pública:**

| Método | Descripción |
|--------|-------------|
| `ToggleShaderMenu()` | Toggle del shader menu. Cierra otros menús mutuamente excluyentes |
| `ToggleEnvPanel()` | Toggle del panel de entorno. Cierra otros menús |
| `ToggleCategoryMenu()` | Toggle del menú de categorías. Cierra otros menús |
| `CloseAllMenus()` | Cierra todos los popups + slider |
| `ToggleHotspots()` | Toggle de visibilidad de hotspots via `HotspotManager` |
| `SetCategoryFilter(string, Button)` | Multi-select de categorías (ALL, Structure, Propulsion, Avionics, Power) con delegación a `ExplodedViewManager.SetCategoryFilters()` |
| `SetSliderVisible(bool)` | Muestra/oculta el slider de explosión |
| `SetSheetOpenState(bool)` | Reacciona a cambios del details sheet para ajustar stacking |
| `RepositionPopups()` | **Algoritmo de stacking dinámico** — calcula `style.bottom` para cada popup |
| `Dispose()` | Limpia callbacks |

**Algoritmo de stacking (`RepositionPopups`):**
```
Base = POPUP_BASE_BOTTOM (192px) + (sheet open ? rootHeight * 0.56 + 192 : 0)

Para cada popup visible (bottom → top):
  1. SliderContainer  (56px height)
  2. CategoryMenu     (192px height)
  3. ShaderMenu       (192px height)
  4. EnvPanel         (220px height)
  
  popup.style.bottom = currentBottom
  currentBottom += height + POPUP_GAP (24px)

PopupBlocker visible = anyMenuVisible (excluye slider)
```

**Constantes de layout (grid de 8pt):**
- `POPUP_BASE_BOTTOM = 192f` — padding-bottom(24) + info-row(24) + margin(16) + button(104) + extra-gap(24)
- `POPUP_GAP = 24f` — 8pt × 3

---

#### 2.3 — `UIHeroController.cs` (200 líneas, NUEVO)

**Namespace:** `WebGL.UI.Panels`  
**Responsabilidad:** Hero/Landing screen y submenús

**Constructor:**
```csharp
public UIHeroController(VisualElement root)
```

**API Pública:**

| Método | Descripción |
|--------|-------------|
| `DismissHero()` | Oculta hero container (`hero--hidden` + `display:None` + `pickingMode:Ignore`) |
| `ReturnToHero()` | Restaura hero container y cierra cualquier submenú abierto |
| `OpenHeroSubmenu(SubmenuType)` | Abre submenú específico (Devices, About, Exit) ocultando `HeroMain` |
| `CloseHeroSubmenu()` | Cierra todos los submenús y restaura `HeroMain` |
| `HeroDismissed` (property) | Estado booleano público |
| `Dispose()` | Limpia callbacks + anula eventos |

**Eventos:**
- `OnHeroDismissed` → UIManager inicializa hotspots
- `OnHeroReturned` → UIManager resetea cámara, view mode, app state, cierra sheet/menus

**Submenús gestionados:**
- `HeroSubmenu_Devices` — Información de dispositivos
- `HeroSubmenu_About` — Acerca de
- `HeroSubmenu_Exit` — Confirmación de salida (`Application.Quit()` / `window.history.back()` en WebGL)

**Botones vinculados (con cleanup):**
- `HeroExploreBtn` → `DismissHero()` + `AppStateMachine.EnterExploration()`
- `HeroDeviceBtn` / `HeroAboutBtn` / `HeroExitBtn` → `OpenHeroSubmenu(type)`
- `SubmenuBackBtn_Devices` / `About` / `Exit` → `CloseHeroSubmenu()`
- `ExitConfirmBtn` / `ExitCancelBtn` — Flujo de salida
- `HomeBtn` → `ReturnToHero()`

---

#### 2.4 — `UIManager.cs` Resultante (388 líneas, REESCRITO)

**Responsabilidad retenida (slim coordinator):**

1. **Lifecycle** — `Awake()`, `OnEnable()`, `OnDisable()`
2. **Initialization** — `InitializeUI()` instancia sub-controladores y wires cross-controller events
3. **Event routing** — `OnPartSelected()`, `OnAppStateChanged()`, `OnViewModeChanged()` distribuyen a sub-controladores
4. **Toolbar handlers simples** — `OnExplodeToggle()`, `OnResetClicked()`, `OnExplosionSliderChanged()`
5. **Cross-cutting** — `RegisterButtonInputBlockers()` (todos los `<Button>` bloquean 3D input)
6. **Manager auto-creation** — `EnsureManagers()` crea `HotspotManager`, `ViewModeManager`, `EnvironmentController` si faltan
7. **Cleanup** — `AddCleanup()` + `UnsubscribeFromUIEvents()` + sub-controller `Dispose()`

**Wiring entre sub-controladores (hecho en `InitializeUI()`):**

```
UIDetailsSheet.OnSheetStateChanged(bool)
    └── UIPopupController.SetSheetOpenState(bool)
        └── Cierra todos los popups si sheet se abre

UIHeroController.OnHeroDismissed
    └── UIManager.OnHeroDismissed()
        └── Inicializa hotspots

UIHeroController.OnHeroReturned
    └── UIManager.OnHeroReturned()
        └── Cierra sheet + menus, oculta hotspots, reset cámara/viewmode/state
```

---

## Paso 3: Input Decoupling

### Problema
`SelectionManager.cs` contenía un método privado `IsPointerOverUIToolkit()` que:
1. Llamaba `Object.FindFirstObjectByType<UIDocument>()` **en cada frame** (costoso)
2. Hacía conversión manual incorrecta: `mousePos.y = Screen.height - mousePos.y`
3. **No funcionaba con `ScaleWithScreenSize`** (PanelSettings usa referencia 1920×1080, match 0.5)

La conversión manual solo invertía Y, pero no aplicaba el **factor de escala** que `ScaleWithScreenSize` introduce. Resultado: los hits se desplazaban proporcionalmente a la diferencia entre la resolución real y la referencia.

### Solución implementada

#### 3.1 — `InputManager.cs` (135 líneas, REESCRITO)

**Cambios clave:**

1. **Nueva propiedad `IsDragging3D`** — Track de arrastre en viewport 3D
2. **Método público `IsPointerOverUI()`** — Fuente única de verdad para detección UI
3. **Lazy caching** de `UIDocument` + `IPanel` via `CacheUIDocumentIfNeeded()`

**Algoritmo de `IsPointerOverUI()`:**

```csharp
public bool IsPointerOverUI()
{
    if (_uiPanel == null) return false;

    // RuntimePanelUtils.ScreenToPanel maneja:
    //  - Inversión del eje Y (Screen bottom-left → Panel top-left)
    //  - Factor de escala de ScaleWithScreenSize
    Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, Input.mousePosition);
    VisualElement picked = _uiPanel.Pick(panelPos);

    if (picked == null) return false;

    // TemplateContainer (padre del root) no es UI interactiva
    if (_mainUIDocument != null && picked == _mainUIDocument.rootVisualElement?.parent) 
        return false;

    return true;
}
```

**¿Por qué `RuntimePanelUtils.ScreenToPanel()` es la solución correcta?**

`RuntimePanelUtils.ScreenToPanel(IPanel, Vector2 screenPosition)` es la **única API de Unity** que maneja correctamente **ambas** transformaciones necesarias:

| Transformación | Manual (rota) | RuntimePanelUtils (correcto) |
|---|---|---|
| Inversión Y | `Screen.height - y` | ✅ Incluido |
| ScaleWithScreenSize factor | ❌ No aplicado | ✅ Incluido |
| DPI scaling | ❌ No aplicado | ✅ Incluido |

**Lazy Caching:**
```csharp
private void CacheUIDocumentIfNeeded()
{
    if (_uiPanel != null) return;
    if (_mainUIDocument == null)
        _mainUIDocument = Object.FindFirstObjectByType<UIDocument>();
    if (_mainUIDocument != null && _mainUIDocument.rootVisualElement != null)
        _uiPanel = _mainUIDocument.rootVisualElement.panel;
}
```
- Se ejecuta en `Update()` pero es **O(1)** después del primer frame exitoso
- Evita `FindFirstObjectByType` en cada frame (problema del código anterior)
- Tolera escenas donde `UIDocument` aún no existe al momento de `Awake()`

#### 3.2 — `SelectionManager.cs` (302 líneas, MODIFICADO)

**Cambios:**

1. **Eliminado:** `using UnityEngine.UIElements` (ya no necesario)
2. **Eliminado:** Método privado `IsPointerOverUIToolkit()` (33 líneas, incluía la conversión manual incorrecta)
3. **Reemplazado en `HandleClick()`:**

```csharp
// ANTES (roto con ScaleWithScreenSize):
if (IsPointerOverUIToolkit()) { return; }

// DESPUÉS (delegado a InputManager):
if (InputManager.Instance != null && InputManager.Instance.IsPointerOverUI()) { return; }
```

---

## Diagrama de Arquitectura Resultante

```
┌─────────────────────────────────────────────────────┐
│                    UIManager (388 loc)               │
│  Slim coordinator: lifecycle, event routing, wiring  │
│                                                       │
│  ┌──────────────────┐  ┌─────────────────────────┐   │
│  │ UIDetailsSheet   │  │ UIPopupController       │   │
│  │ (291 loc)        │  │ (273 loc)               │   │
│  │ Bottom sheet,    │──│ Shader/Cat/Env menus,   │   │
│  │ part data,       │  │ stacking, categories,   │   │
│  │ drag-to-dismiss  │  │ hotspots toggle         │   │
│  └──────────────────┘  └─────────────────────────┘   │
│                                                       │
│  ┌──────────────────┐  ┌──────────┐  ┌───────────┐  │
│  │ UIHeroController │  │UIAnalyze │  │UIEnviron- │  │
│  │ (200 loc)        │  │Panel     │  │mentPanel  │  │
│  │ Hero screen,     │  │(103 loc) │  │(118 loc)  │  │
│  │ submenus         │  │Shaders   │  │Presets    │  │
│  └──────────────────┘  └──────────┘  └───────────┘  │
└─────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────┐
│                  Core Managers Layer                  │
│                                                       │
│  ┌──────────────────┐  ┌─────────────────────────┐   │
│  │ InputManager     │  │ SelectionManager        │   │
│  │ (135 loc)        │──│ (302 loc)               │   │
│  │ IsPointerOverUI()│  │ Raycast selection       │   │
│  │ IsDragging3D     │  │ Delegates UI check to   │   │
│  │ RuntimePanelUtils│  │ InputManager             │   │
│  └──────────────────┘  └─────────────────────────┘   │
│                                                       │
│  OrbitCameraController │ AppStateMachine │ EventBus   │
│  ViewModeManager │ HotspotManager │ EnvironmentCtrl  │
└─────────────────────────────────────────────────────┘
```

---

## Contexto Técnico

| Aspecto | Detalle |
|---------|---------|
| **Motor** | Unity 6.0 LTS |
| **Target** | WebGL |
| **UI Framework** | UI Toolkit (UXML/USS) — **no** Canvas/UGUI |
| **PanelSettings** | `ScaleWithScreenSize`, ref 1920×1080, match 0.5 |
| **Rendering** | URP (Universal Render Pipeline) |
| **Patrones** | Singleton / PersistentSingleton, EventBus pub/sub estático |
| **Assemblies** | `Core.asmdef` (WebGL.Core), `UI.asmdef` (WebGL.UI → references Core) |

---

## Notas Importantes

### ¿Por qué la conversión manual `Screen.height - mousePos.y` no funciona?

Con `ScaleWithScreenSize` (PanelSettings), las coordenadas del panel **no** son 1:1 con las coordenadas de pantalla. El panel escala internamente al tamaño de referencia (1920×1080). La conversión manual solo invierte Y pero no aplica la escala, causando que `Panel.Pick()` consulte posiciones desplazadas.

### ¿Por qué los submenús con `picking-mode: Ignore` no bloquean clicks?

Los contenedores de menú en `MainLayout.uxml` tienen `picking-mode="Ignore"`, pero sus hijos (botones) tienen `picking-mode="Position"`. `Panel.Pick()` solo retorna elementos con `Position`, así que los contenedores invisibles no interfieren con la detección de UI.

### Sobre el `PopupBlocker`

El `PopupBlocker` es un VisualElement invisible que cubre toda la pantalla cuando un menú popup está abierto. Su `PointerDownEvent` cierra todos los menús, implementando el patrón "click outside to close".

---

## Historial de Intentos Previos (para contexto)

Antes de esta implementación limpia, hubo **7+ intentos** de solucionar el hit-testing de UI:

| Commit | Enfoque | Resultado |
|--------|---------|-----------|
| `2672806` | Centralizar hit test en InputManager | Base correcta, conversión incorrecta |
| `b2ad72b` | Triple-check coordinate fallback | Over-engineered, no resolvía la escala |
| `f940375` | TrickleDown event capturing | Bypass de Panel.Pick, frágil |
| `12e6887` | `EventSystem.current.IsPointerOverGameObject` | **No funciona con UI Toolkit** |
| `0f85d26` | DOM CSS-box model hover tracking | Complejo, race conditions |
| `520959a` | Container hover observation + Position picking | Parcialmente funcional |
| `177caf9` | Limpieza de errores de sintaxis | Fix de consecuencias |

**La solución final (`RuntimePanelUtils.ScreenToPanel`) es la respuesta canónica de Unity para este problema.** Es una API de una línea que reemplaza todos los intentos manuales de conversión de coordenadas.
