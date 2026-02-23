# Phase 2 — Senior UX Redesign: Changelog

**Proyecto:** WebGL Drone Viewer (Unity 6.0 LTS — UI Toolkit / WebGL)  
**Branch:** `feature/phase2-ux-redesign`  
**Plan:** `PHASE2_UX_PLAN.md`  
**Fecha inicio:** 23 de febrero de 2026

---

## Resumen de Iteraciones

| Iteración | Descripción                              | Commit  | Fecha | Estado      |
| --------- | ---------------------------------------- | ------- | ----- | ----------- |
| 1         | Expandir AppStateMachine                 | 2673e5a | 23-feb-2026 | ✅ Completada |
| 2         | Reestructurar MainLayout.uxml + USS      | 70b2a01 | 23-feb-2026 | ✅ Completada |
| 3+4       | UIModeController + Rewire UIManager      | f125f6f | 23-feb-2026 | ✅ Completada |
| 5         | Cross-Section UI en Analyze Mode         | 66673f3 | 23-feb-2026 | ✅ Completada |
| 6         | Cleanup & Integración Final              | —       | 23-feb-2026 | ✅ Completada |

---

## Iteración 1 — Expandir AppStateMachine

**Commit:** `2673e5a`  
**Archivos modificados:** 3  

### Cambios realizados:

#### `AppStateMachine.cs`
- **Enum `AppState`:** Agregados `Analyze` y `Studio` al final (después de `Menu`)
- **`CanTransitionTo()`:** Reescrito con agrupación lógica:
  - `Loading` → solo puede ir a `Intro`
  - `Intro` → solo puede ir a `Exploration`
  - `Exploration`/`Analyze`/`Studio`/`ExplodedView`/`FocusMode` → pueden ir a cualquier estado excepto `Loading` e `Intro`
  - `Settings`/`Menu` → pueden ir a cualquier estado excepto `Loading` e `Intro`
- **Métodos de conveniencia:** `EnterAnalyze()` y `EnterStudio()` (delegando a `SetState()`)
- **`IsInteractive()`:** Ahora retorna `true` para `Analyze` y `Studio` además de `Exploration`, `ExplodedView` y `FocusMode`

#### `SelectionManager.cs`
- **`ShouldProcessInput()`:** Reemplazado check hardcodeado de 3 estados por llamada centralizada a `AppStateMachine.Instance.IsInteractive()`. Esto garantiza que los nuevos modos se reconocen automáticamente.

#### `UIManager.cs`
- **`OnAppStateChanged()`:** Variable `isInteractive` expandida para incluir `AppState.Analyze` y `AppState.Studio`, asegurando que hotspots y slider responden correctamente en los nuevos modos.

### Decisiones arquitectónicas:
- `SelectionManager` ahora usa `IsInteractive()` centralizado → cualquier futuro estado interactivo se propaga automáticamente sin tocar `SelectionManager`
- `UIManager` mantiene check explícito (no usa `IsInteractive()`) porque necesita lógica específica por estado (ej: slider solo en ExplodedView)
- Las transiciones son permisivas entre modos maestros para permitir UX fluida

### Resultado de compilación: ✅ 0 errores

---

## Iteración 2 — Reestructurar MainLayout.uxml + USS

**Commit:** `70b2a01`  
**Archivos modificados:** 2  

### Cambios realizados:

#### `MainLayout.uxml`
- **`.actions-row`:** Reemplazados 5 botones planos (ShaderBtn, LayerBtn, ExplodeBtn, InfoBtn, EnvBtn) por 3 botones de modo: `ModeExploreBtn`, `ModeAnalyzeBtn`, `ModeStudioBtn`
- **`AnalyzeModeContainer`:** Nuevo contenedor que agrupa ShaderMenu, CategoryMenu y SliderContainer (anteriormente en nivel raíz)
- **`StudioModeContainer`:** Nuevo contenedor que agrupa EnvPanel (anteriormente en nivel raíz)
- **`PopupBlocker`:** Insertado dentro de cada mode container para cierre con click externo
- **Estructura:** TopBar → AnalyzeModeContainer → StudioModeContainer → BottomBar → BottomSheet → Hero

#### `Theme.uss`
- Nuevas clases: `.mode-btn`, `.mode-btn--active`, `.mode-btn--explore`, `.mode-btn--analyze`, `.mode-btn--studio`
- Nuevas clases: `.mode-container`, `.mode--hidden`, `.mode-submenu`
- Colores de modo: verde (Explore), azul (Analyze), púrpura (Studio)

### Resultado de compilación: ✅ 0 errores

---

## Iteración 3+4 — UIModeController + Rewire UIManager

**Commit:** `f125f6f`  
**Archivos modificados:** 2 (1 nuevo, 1 modificado)  

### Cambios realizados:

#### `UIModeController.cs` (NUEVO — reemplaza UIPopupController)
- **Responsabilidad:** Gestión completa del sistema de 3 modos (Explore/Analyze/Studio)
- **API pública:** `ActivateMode()`, `DeactivateAllModes()`, `ToggleShaderMenu()`, `ToggleCategoryMenu()`, `SetSliderVisible()`, `SetCategoryFilter()`, `ToggleHotspots()`, `SetSheetOpenState()`, `CloseAllMenus()`, `SyncWithAppState()`
- **Gestión visual:** Show/hide de mode containers, toggle de clases `.mode-btn--active` en botones
- **PopupBlocker:** Cierra submenús del modo activo al hacer click fuera
- **Exclusión mutua:** Solo un modo activo a la vez; submenús mutuamente excluyentes dentro de cada modo
- **Patrón AddCleanup/Dispose:** Implementado para prevenir memory leaks

#### `UIManager.cs`
- **Eliminados:** Referencias a `shaderBtn`, `explodeBtn`, `envBtn` (botones obsoletos)
- **Agregados:** Integración con `UIModeController` en vez de `UIPopupController`
- **Botones de modo:** `ModeExploreBtn`, `ModeAnalyzeBtn`, `ModeStudioBtn` cableados vía UIModeController
- **`OnAppStateChanged()`:** Delegación a `_modeController.SyncWithAppState()`
- **`OnHeroReturned()`:** Llama `_modeController.DeactivateAllModes()` + `CloseAllMenus()`

### Resultado de compilación: ✅ 0 errores

---

## Iteración 5 — Cross-Section UI en Analyze Mode

**Commit:** `66673f3`  
**Archivos modificados:** 4 (1 nuevo, 3 modificados)  

### Cambios realizados:

#### `UICrossSectionPanel.cs` (NUEVO)
- **Responsabilidad:** Interfaz de usuario para herramienta de corte transversal (cross-section)
- **Vinculación:** Conecta con `CrossSectionManager` API existente
- **Controles:** Toggle on/off, selección de eje (X/Y/Z), slider de posición, botón invertir
- **Input guards:** `InputManager.InputBlocked` en interacciones con slider
- **Patrón AddCleanup/Dispose:** Implementado

#### `MainLayout.uxml`
- **CrossSectionPanel:** Agregado dentro de `AnalyzeModeContainer` con toggle, botones de eje, slider y botón de inversión

#### `Theme.uss`
- Nuevos estilos: `.cross-section-panel`, `.cross-section-toggle`, `.cross-section-toggle--active`, `.cross-section-row`, `.axis-btn-group`, `.axis-btn`, `.axis-btn--active`, `.cross-slider`, `.invert-btn`

#### `UIManager.cs`
- **`_uiCrossSectionPanel`:** Nuevo campo + inicialización + `AddCleanup()`

### Resultado de compilación: ✅ 0 errores

---

## Iteración 6 — Cleanup & Integración Final

**Commit:** (este commit)  
**Archivos modificados:** 5 (1 eliminado, 4 modificados)  

### Cambios realizados:

#### `UIPopupController.cs` (ELIMINADO)
- Código muerto — ya no referenciado por ningún archivo desde Iteración 3+4
- Funcionalidad completamente absorbida por `UIModeController.cs`

#### `ProjectSetupWizard.cs`
- **Desactivados:** `ViewModeToolbar` y `EngineerToolbar` del setup automático (comentados con nota explicativa)

#### `ViewModeToolbar.cs` (MARCADO OBSOLETO)
- Agregado atributo `[System.Obsolete]` + XMLDoc explicativo
- Funcionalidad cubierta por UIAnalyzePanel + UICrossSectionPanel en el nuevo sistema de modos
- Archivo conservado como referencia, no instanciado en runtime

#### `EngineerToolbar.cs` (MARCADO OBSOLETO)
- Agregado atributo `[System.Obsolete]` + XMLDoc explicativo
- Herramientas de ingeniería a evaluar para futura integración en modos
- Archivo conservado como referencia, no instanciado en runtime

#### `Theme.uss`
- **Eliminados:** 5 selectores huérfanos (`#ShaderBtn`, `#LayerBtn`, `#ExplodeBtn`, `#InfoBtn`, `#EnvBtn`)
- Comentario explicativo sobre el reemplazo por el sistema de 3 modos

#### `KeyboardShortcuts.cs`
- **Atajos de modo:** Agregados `7` (Explore), `8` (Analyze), `9` (Studio) para cambio rápido de modo
- **`ToggleExplodedView()`:** Ahora verifica `IsInteractive()` y permite explosión desde Analyze además de Exploration
- **Métodos nuevos:** `SwitchToExplore()`, `SwitchToAnalyze()`, `SwitchToStudio()` con guards de estado

### Verificaciones:
- ✅ 0 errores de compilación (global)
- ✅ Sin referencia rota a UIPopupController
- ✅ Todos los `AddCleanup()` verificados en sub-controllers
- ✅ `InputManager.InputBlocked` guards en sliders (explosion + cross-section)
- ✅ `RegisterButtonInputBlockers()` cubre todos los botones del árbol UXML

### Resultado de compilación: ✅ 0 errores
