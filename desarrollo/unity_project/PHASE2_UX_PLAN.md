# Phase 2 — Senior UX Redesign: 3-Mode System

**Proyecto:** WebGL Drone Viewer (Unity 6.0 LTS — UI Toolkit / WebGL)  
**Branch:** `feature/phase2-ux-redesign`  
**Base:** commit `7515117` (feature/phase3-architecture — estable, 0 errores)  
**Fecha inicio:** 23 de febrero de 2026  
**Autor:** GitHub Copilot (agente automatizado)

---

## Principios de Ejecución

### 🔒 Protección de Phase 3/4/5

Todo el trabajo de la fase anterior DEBE preservarse:

| Mecanismo                        | Qué protege                                    | Regla                                                      |
| -------------------------------- | ---------------------------------------------- | ---------------------------------------------------------- |
| `AddCleanup()` + `Dispose()`    | Memory leak prevention                         | **TODO** callback registrado DEBE tener su `AddCleanup()`  |
| `RegisterButtonInputBlockers()` | Mecanismo dual proactivo/reactivo de input      | **NO ELIMINAR** — es arquitecturalmente necesario          |
| `InputManager.InputBlocked`     | Guard proactiva en PointerEnter/Leave           | **MANTENER** en todos los componentes UI interactivos      |
| `IsPointerOverUI()` + Panel.Pick| Guard reactiva por frame                        | **NO MODIFICAR** InputManager.cs                           |
| `RuntimePanelUtils.ScreenToPanel`| Conversión correcta de coordenadas              | **NO REINTRODUCIR** conversiones manuales                  |
| `EventBus` pub/sub              | Desacoplamiento de eventos                      | Usar `EventBus.Publish()` para nuevos eventos              |

### 📋 Workflow por Iteración

1. Implementar los cambios de la iteración
2. Verificar 0 errores de compilación
3. Probar funcionalidad en Unity Editor (Play Mode)
4. Actualizar `PHASE2_CHANGELOG.md` con los cambios de la iteración
5. `git add .` → `git commit` con mensaje descriptivo → `git push origin feature/phase2-ux-redesign`
6. Marcar iteración como ✅ en este documento

### 📝 Changelog

Se generará y mantendrá `PHASE2_CHANGELOG.md` con documentación detallada de cada iteración:
- Archivos modificados/creados/eliminados
- Cambios específicos con snippets de código clave
- Decisiones arquitectónicas y su justificación
- Resultado de compilación por iteración

---

## Contexto: Estado Actual vs Objetivo

### Estado Actual (Bottom Bar)

```
┌──────────────────────────────────────────────────────┐
│  [VIEW]  [LAYERS]  [EXP]  [INFO]  [ENV]             │  ← 5 botones planos
│  ShaderBtn LayerBtn ExplodeBtn InfoBtn EnvBtn        │  ← sin jerarquía
└──────────────────────────────────────────────────────┘
```

**Problemas:**
- 5 botones sin jerarquía = carga cognitiva alta
- `InfoBtn` es redundante (se hace click en piezas directamente)
- No hay agrupación lógica de herramientas
- El corte transversal (`CrossSectionManager`) existe en código pero **NO tiene UI** en el layout principal
- `ViewModeToolbar.cs` y `EngineerToolbar.cs` crean toolbars dinámicos redundantes con el bottom bar

### Objetivo (3-Mode System)

```
┌──────────────────────────────────────────────────────┐
│       [🔍 EXPLORE]  [📐 ANALYZE]  [☀️ STUDIO]       │  ← 3 modos maestros
│        (default)     (technical)   (lighting)        │  ← jerarquía contextual
└──────────────────────────────────────────────────────┘

EXPLORE mode: UI limpia. Click en piezas → BottomSheet con detalles. Hotspots visibles.
ANALYZE mode: Muestra AnalyzeModeContainer con ShaderMenu, CategoryMenu, SliderContainer,
              y NUEVO: CrossSection UI (eje, posición, toggle).
STUDIO mode:  Muestra StudioModeContainer con EnvPanel (presets + sliders).
```

---

## Plan de Iteraciones

### Iteración 1 — Expandir `AppStateMachine` + Nuevos Eventos

**Archivos:** `AppStateMachine.cs`, `EventBus` events (si es necesario)  
**Riesgo:** BAJO — solo agrega, no modifica existente  
**Estado:** ✅ Completada (commit `2673e5a`)

#### Tareas:

1. **Agregar `Analyze` y `Studio` al enum `AppState`:**
   ```csharp
   public enum AppState
   {
       Loading, Intro, Exploration, ExplodedView, FocusMode, Settings, Menu,
       Analyze,  // NUEVO — herramientas técnicas
       Studio    // NUEVO — entorno/iluminación
   }
   ```

2. **Actualizar `CanTransitionTo()` para permitir transiciones:**
   - Desde `Exploration` → `Analyze`, `Studio` (y viceversa)
   - Desde `Analyze` → `Studio` (y viceversa) — cambio directo entre modos
   - Desde `Analyze`/`Studio` → `ExplodedView`, `FocusMode` — permitido
   - `Loading`/`Intro` mantienen restricciones existentes

3. **Agregar convenience methods:**
   ```csharp
   public void EnterAnalyze() => SetState(AppState.Analyze);
   public void EnterStudio() => SetState(AppState.Studio);
   ```

4. **Actualizar `IsInteractive()` para incluir nuevos modos:**
   ```csharp
   public bool IsInteractive()
   {
       return currentState == AppState.Exploration ||
              currentState == AppState.ExplodedView ||
              currentState == AppState.FocusMode ||
              currentState == AppState.Analyze ||
              currentState == AppState.Studio;
   }
   ```

#### Verificación:
- [ ] 0 errores de compilación
- [ ] Estados existentes no afectados
- [ ] `CanTransitionTo()` permite Exploration ↔ Analyze ↔ Studio

---

### Iteración 2 — Reestructurar `MainLayout.uxml` + Nuevos Estilos USS

**Archivos:** `MainLayout.uxml`, `Theme.uss`  
**Riesgo:** MEDIO — modifica layout visible, los IDs de botones cambian  
**Estado:** ✅ Completada (commit `70b2a01`)

#### Tareas:

1. **Reemplazar `.actions-row` con 3 botones de modo:**
   ```xml
   <ui:VisualElement class="actions-row" picking-mode="Ignore">
       <ui:Button name="ModeExploreBtn" class="icon-button mode-btn" tooltip="Explore Mode">
           <ui:Label text="EXPLORE" class="button-icon-text" picking-mode="Ignore"/>
       </ui:Button>
       <ui:Button name="ModeAnalyzeBtn" class="icon-button mode-btn" tooltip="Analyze Mode">
           <ui:Label text="ANALYZE" class="button-icon-text" picking-mode="Ignore"/>
       </ui:Button>
       <ui:Button name="ModeStudioBtn" class="icon-button mode-btn" tooltip="Studio Mode">
           <ui:Label text="STUDIO" class="button-icon-text" picking-mode="Ignore"/>
       </ui:Button>
   </ui:VisualElement>
   ```

2. **Crear `AnalyzeModeContainer`** — envuelve herramientas técnicas:
   ```xml
   <ui:VisualElement name="AnalyzeModeContainer" class="mode-container mode--hidden" picking-mode="Ignore">
       <!-- Se mueven aquí: ShaderMenu, CategoryMenu, SliderContainer -->
       <!-- NUEVO: CrossSectionPanel (ver Iteración 5) -->
   </ui:VisualElement>
   ```

3. **Crear `StudioModeContainer`** — envuelve herramientas de entorno:
   ```xml
   <ui:VisualElement name="StudioModeContainer" class="mode-container mode--hidden" picking-mode="Ignore">
       <!-- Se mueve aquí: EnvPanel -->
   </ui:VisualElement>
   ```

4. **Agregar estilos USS para el sistema de modos:**
   - `.mode-btn` — estilo base para botones de modo (más anchos que `icon-button`)
   - `.mode-btn--active` — estado activo (highlight, underline o glow)
   - `.mode-container` — contenedor de modo (position: absolute, bottom)
   - `.mode--hidden` — `display: none`

5. **Eliminar botones obsoletos:** `ShaderBtn`, `LayerBtn`, `ExplodeBtn`, `InfoBtn`, `EnvBtn`
   - Sus IDs ya NO existirán en el UXML
   - El C# que los referencia se actualizará en iteraciones posteriores

#### Decisiones de diseño:
- Los popup menus (ShaderMenu, CategoryMenu, EnvPanel) se **mueven DENTRO** de sus mode containers
- Dejan de ser popups flotantes y pasan a ser **contenido contextual del modo**
- El `PopupBlocker` puede mantenerse para el modo Analyze (click fuera cierra menús internos)
- El `SliderContainer` (explosión) se mueve a AnalyzeModeContainer

#### Verificación:
- [ ] UXML válido (no rompe Unity)
- [ ] Los 3 botones de modo renderizan correctamente
- [ ] Mode containers están ocultos por defecto

---

### Iteración 3 — Refactorizar `UIPopupController` → `UIModeController`

**Archivos:** `UIPopupController.cs` → rename a `UIModeController.cs`, `UIManager.cs` (actualizar referencia)  
**Riesgo:** ALTO — refactoring de controlador central  
**Estado:** ✅ Completada (commit `f125f6f`, fusionada con Iteración 4)

#### Tareas:

1. **Renombrar** `UIPopupController.cs` → `UIModeController.cs` (y clase interna)

2. **Cambiar responsabilidad principal:**
   
   | Antes (UIPopupController)                          | Después (UIModeController)                              |
   | -------------------------------------------------- | ------------------------------------------------------- |
   | Toggle individual de ShaderMenu, CategoryMenu, Env | Activar/desactivar mode containers completos            |
   | Stacking dinámico con `RepositionPopups()`          | Show/hide por modo (sin stacking entre modos)           |
   | Mutual exclusion entre popups individuales          | Mutual exclusion entre modos (solo 1 activo)            |
   | PopupBlocker para cerrar menús                     | PopupBlocker para click fuera del modo Analyze          |

3. **API del nuevo `UIModeController`:**

   ```csharp
   public class UIModeController
   {
       // Constructor recibe: root, analyzeModeContainer, studioModeContainer, popupBlocker
       
       public void ActivateMode(AppState mode)    // Muestra el container correcto
       public void DeactivateAllModes()            // Oculta todos los containers
       
       // Submenú interno de Analyze (herencia del popup system):
       public void ToggleShaderMenu()              // Dentro de AnalyzeModeContainer
       public void ToggleCategoryMenu()            // Dentro de AnalyzeModeContainer
       public void SetSliderVisible(bool visible)  // Dentro de AnalyzeModeContainer
       
       // Category filters (se mantienen):
       public void SetCategoryFilter(string category, Button clickedBtn)
       
       // Hotspots (se mantiene):
       public void ToggleHotspots()
       
       // Sheet coordination (se mantiene):
       public void SetSheetOpenState(bool isOpen)
       
       public void Dispose()
   }
   ```

4. **Lógica de `ActivateMode()`:**
   ```
   Explore → hide AnalyzeModeContainer + hide StudioModeContainer
   Analyze → show AnalyzeModeContainer + hide StudioModeContainer
   Studio  → hide AnalyzeModeContainer + show StudioModeContainer
   ```

5. **Preservar `AddCleanup()` pattern** en toda la clase

#### Consideraciones:
- `UIAnalyzePanel` y `UIEnvironmentPanel` siguen siendo instanciados — no cambian
- La lógica interna de shader cards y env presets permanece intacta
- El `PopupBlocker` se mantiene para cerrar submenús dentro de Analyze mode
- `RepositionPopups()` se simplifica — ya no necesita stacking entre menús de diferentes modos

#### Verificación:
- [ ] 0 errores de compilación
- [ ] `UIModeController` responde a cambios de `AppState`
- [ ] Explore mode oculta todo correctamente
- [ ] `Dispose()` limpia todos los callbacks

---

### Iteración 4 — Rewire `UIManager.cs`

**Archivos:** `UIManager.cs`  
**Riesgo:** ALTO — coordinador central  
**Estado:** ✅ Completada (commit `f125f6f`, fusionada con Iteración 3)

#### Tareas:

1. **Eliminar referencias a botones obsoletos:**
   - Quitar: `shaderBtn`, `explodeBtn`, `envBtn`, `infoBtn`, `layerBtn`
   - Agregar: `modeExploreBtn`, `modeAnalyzeBtn`, `modeStudioBtn`

2. **Wire nuevos botones de modo:**
   ```csharp
   modeExploreBtn.clicked += () => AppStateMachine.Instance?.EnterExploration();
   modeAnalyzeBtn.clicked += () => AppStateMachine.Instance?.EnterAnalyze();
   modeStudioBtn.clicked += () => AppStateMachine.Instance?.EnterStudio();
   ```
   Con `AddCleanup()` para cada uno.

3. **Reemplazar `_popupController` por `_modeController`:**
   ```csharp
   private UIModeController _modeController;
   ```

4. **Actualizar `OnAppStateChanged()`:**
   ```csharp
   private void OnAppStateChanged(AppStateChangedEvent evt)
   {
       // Activar modo en UIModeController
       _modeController.ActivateMode(evt.NewState);
       
       // Actualizar botones activos
       UpdateModeButtonStates(evt.NewState);
       
       // Hotspots: solo en modos interactivos + hero dismissed
       bool isInteractive = AppStateMachine.Instance?.IsInteractive() ?? false;
       bool heroDismissed = _heroController?.HeroDismissed ?? false;
       HotspotManager.Instance?.SetVisible(heroDismissed && isInteractive);
   }
   ```

5. **Implementar `UpdateModeButtonStates()`:**
   ```csharp
   private void UpdateModeButtonStates(AppState state)
   {
       modeExploreBtn?.EnableInClassList("mode-btn--active", 
           state == AppState.Exploration || state == AppState.ExplodedView || state == AppState.FocusMode);
       modeAnalyzeBtn?.EnableInClassList("mode-btn--active", state == AppState.Analyze);
       modeStudioBtn?.EnableInClassList("mode-btn--active", state == AppState.Studio);
   }
   ```

6. **Mantener `RegisterButtonInputBlockers()`** — se ejecuta sobre TODOS los `<Button>` del root, así que los nuevos botones de modo se registran automáticamente.

7. **Actualizar `OnHeroReturned()` y `OnResetClicked()`** para usar nuevos nombres/lógica.

8. **Limpiar wiring de botones de categoría:**
   - Los `BindCat()` calls se mantienen pero ahora los botones están dentro de `AnalyzeModeContainer`
   - El query `root.Q<Button>("CatBtn_All")` sigue funcionando porque busca por nombre, no por jerarquía

#### Verificación:
- [ ] 0 errores de compilación
- [ ] Click en ModeExploreBtn → `AppState.Exploration`
- [ ] Click en ModeAnalyzeBtn → `AppState.Analyze` → muestra AnalyzeModeContainer
- [ ] Click en ModeStudioBtn → `AppState.Studio` → muestra StudioModeContainer
- [ ] Botón activo recibe clase `.mode-btn--active`
- [ ] `AddCleanup()` registrado para todos los nuevos callbacks
- [ ] Hero return resetea correctamente al modo Explore

---

### Iteración 5 — Cross-Section UI en Analyze Mode

**Archivos:** `MainLayout.uxml` (agregar panel), `UIModeController.cs` o nuevo `UICrossSectionPanel.cs`, `CrossSectionManager.cs` (ya existe)  
**Riesgo:** MEDIO — funcionalidad nueva pero `CrossSectionManager` ya tiene la lógica  
**Estado:** ✅ Completada (commit `66673f3`)

#### Contexto:

`CrossSectionManager.cs` ya implementa:
- Toggle on/off del corte
- Selección de eje (X, Y, Z)
- Posición del plano de corte (float)
- Inversión de dirección
- Plano visual (quad transparente)
- Shaders globales `_ClipPlane` / `_ClipEnabled`

**Lo que falta:** Una UI en el layout principal que permita al usuario controlar estos parámetros.

#### Tareas:

1. **Agregar `CrossSectionPanel` al UXML** dentro de `AnalyzeModeContainer`:
   ```xml
   <!-- Cross-Section Controls (inside AnalyzeModeContainer) -->
   <ui:VisualElement name="CrossSectionPanel" class="submenu-container" picking-mode="Ignore">
       <ui:Label text="CROSS SECTION" class="submenu-title" picking-mode="Ignore" />
       
       <!-- Toggle On/Off -->
       <ui:VisualElement class="cross-section-toggle-row">
           <ui:Button name="CrossSectionToggleBtn" class="submenu-card" tooltip="Toggle Cross Section">
               <ui:VisualElement class="submenu-icon icon-cross-section" picking-mode="Ignore"/>
               <ui:Label text="CUT" class="submenu-label" picking-mode="Ignore"/>
           </ui:Button>
       </ui:VisualElement>
       
       <!-- Axis Selection -->
       <ui:VisualElement class="cross-section-axis-row">
           <ui:Button name="CrossAxisX" class="axis-btn" text="X" />
           <ui:Button name="CrossAxisY" class="axis-btn axis-btn--active" text="Y" />
           <ui:Button name="CrossAxisZ" class="axis-btn" text="Z" />
           <ui:Button name="CrossAxisInvert" class="axis-btn" text="⇄" tooltip="Invert Direction" />
       </ui:VisualElement>
       
       <!-- Position Slider -->
       <ui:VisualElement class="cross-section-slider-row">
           <ui:Label text="POSITION" class="env-slider-label" picking-mode="Ignore" />
           <ui:Slider name="CrossSectionSlider" low-value="-2" high-value="2" value="0" 
                       class="glass-slider" picking-mode="Position" />
       </ui:VisualElement>
   </ui:VisualElement>
   ```

2. **Crear `UICrossSectionPanel.cs`** (nuevo sub-controlador):
   ```csharp
   public class UICrossSectionPanel
   {
       // Constructor recibe VisualElement del CrossSectionPanel
       // Bind toggle button → CrossSectionManager.Instance.ToggleCrossSection()
       // Bind axis buttons → CrossSectionManager.Instance.SetAxis(CrossSectionAxis.X/Y/Z)
       // Bind invert button → CrossSectionManager.Instance.SetInverted(!current)
       // Bind slider → CrossSectionManager.Instance.SetPosition(value)
       // Actualiza estados visuales (botón activo, slider habilitado solo cuando cross-section está on)
       // AddCleanup() para todos los callbacks
       public void Dispose()
   }
   ```

3. **Agregar estilos USS** para el panel de cross-section:
   - `.cross-section-toggle-row`, `.cross-section-axis-row`, `.cross-section-slider-row`
   - `.axis-btn` — botones pequeños para selección de eje (pill shape)
   - `.axis-btn--active` — eje seleccionado
   - Input blockers en el slider (PointerEnter/Leave/Down → InputBlocked + StopPropagation)

4. **Instanciar en `UIManager.InitializeUI()`:**
   ```csharp
   _crossSectionPanel = new UICrossSectionPanel(root.Q<VisualElement>("CrossSectionPanel"));
   AddCleanup(() => _crossSectionPanel.Dispose());
   ```

#### Verificación:
- [ ] Panel visible cuando Analyze mode está activo
- [ ] Toggle activa/desactiva el corte visual en el modelo 3D
- [ ] Cambio de eje funciona (X, Y, Z)
- [ ] Slider de posición mueve el plano de corte
- [ ] Inversión funciona
- [ ] Slider tiene input blockers (no mueve cámara)
- [ ] `AddCleanup()` registrado para todos los callbacks

---

### Iteración 6 — Cleanup & Integración Final

**Archivos:** Varios  
**Riesgo:** BAJO — limpieza  
**Estado:** ✅ Completada

#### Tareas:

1. **Evaluar `ViewModeToolbar.cs`:**
   - Crea un toolbar dinámico redundante con botones de cross-section, view modes, reset
   - Con el nuevo sistema de modos, sus funciones están cubiertas por `AnalyzeModeContainer`
   - **Decisión:** Evaluar si se puede desactivar/eliminar o si queda como toolbar alternativo
   - Contiene un botón de CrossSection (`OnCrossSectionClicked`) que ahora está en el panel dedicado

2. **Evaluar `EngineerToolbar.cs`:**
   - Crea dropdown con herramientas de ingeniería
   - Evaluar si sus funciones se integran en el sistema de modos o se mantiene separado

3. **Verificar que `KeyboardShortcuts.cs` sigue funcionando:**
   - Los atajos deben respetar el modo activo
   - Ej: E (explode) solo en Analyze mode, no en Studio

4. **Verificar que `SelectionManager` sigue funcionando:**
   - Click en piezas debe funcionar en todos los modos interactivos
   - El `IsPointerOverUI()` guard sigue activo

5. **Test completo de flujo:**
   - Hero → Explore → click parte → sheet → cambiar a Analyze → shaders + categories + cross-section → cambiar a Studio → env presets → volver a Explore → Home → Hero

6. **Actualizar `PHASE2_CHANGELOG.md`** con resumen final y diagrama de arquitectura

#### Verificación:
- [ ] 0 errores de compilación
- [ ] Flujo completo funcional
- [ ] No hay memory leaks (todos los `AddCleanup()` verificados)
- [ ] Input blocking funciona en todos los modos

---

## Diagrama de Arquitectura Objetivo

```
┌──────────────────────────────────────────────────────────┐
│                    UIManager (slim coordinator)           │
│  Lifecycle, event routing, button wiring, input blockers │
│                                                          │
│  ┌───────────────┐  ┌─────────────────────────────────┐  │
│  │ UIDetailsSheet│  │ UIModeController                │  │
│  │ (bottom sheet)│  │ (formerly UIPopupController)    │  │
│  │               │  │                                 │  │
│  │               │  │  ┌── AnalyzeModeContainer ──┐   │  │
│  │               │  │  │ ShaderMenu (UIAnalyze)   │   │  │
│  │               │  │  │ CategoryMenu + filters   │   │  │
│  │               │  │  │ SliderContainer (explode) │   │  │
│  │               │  │  │ CrossSectionPanel (NEW)  │   │  │
│  │               │  │  └──────────────────────────┘   │  │
│  │               │  │                                 │  │
│  │               │  │  ┌── StudioModeContainer ───┐   │  │
│  │               │  │  │ EnvPanel (UIEnvironment) │   │  │
│  │               │  │  └──────────────────────────┘   │  │
│  │               │  │                                 │  │
│  └───────────────┘  └─────────────────────────────────┘  │
│                                                          │
│  ┌─────────────────┐  ┌────────────────────────────────┐ │
│  │UIHeroController │  │ UICrossSectionPanel (NEW)      │ │
│  │(hero/landing)   │  │ (binds CrossSectionManager)    │ │
│  └─────────────────┘  └────────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
        AppStateMachine  InputManager  SelectionManager
        (Exploration,    (.InputBlocked, (.HandleClick,
         Analyze,         .IsPointerOverUI) .HandleHover)
         Studio, ...)
              │
              ▼
        CrossSectionManager  ViewModeManager  EnvironmentController
        (_ClipPlane,         (7 shader modes) (presets, light)
         _ClipEnabled)
```

---

## Riesgos y Mitigaciones

| Riesgo                                               | Probabilidad | Mitigación                                                    |
| ---------------------------------------------------- | ------------ | ------------------------------------------------------------- |
| Botones obsoletos referenciados en C# → NullRef      | ALTA         | Actualizar C# en misma iteración que UXML                     |
| PopupBlocker deja de funcionar con mode containers   | MEDIA        | Mantener PopupBlocker dentro de AnalyzeModeContainer          |
| Cross-section shaders no compatibles con view modes  | MEDIA        | Verificar que `_ClipEnabled` funciona con todos los 7 shaders |
| `RegisterButtonInputBlockers()` no detecta nuevos btn| BAJA         | Se ejecuta en `InitializeUI()` DESPUÉS de crear todo el DOM   |
| Stacking de popups dentro de Analyze se rompe        | MEDIA        | Simplificar: todos los submenús de Analyze visibles a la vez  |
| `ViewModeToolbar.cs` colisiona con nuevo layout      | MEDIA        | Evaluar en Iteración 6 — posiblemente desactivar              |

---

## Archivos Afectados (estimación)

| Archivo                       | Acción          | Iteración |
| ----------------------------- | --------------- | --------- |
| `AppStateMachine.cs`          | Modificado      | 1         |
| `MainLayout.uxml`             | **Reescrito**   | 2, 5      |
| `Theme.uss`                   | Modificado      | 2, 5      |
| `UIPopupController.cs`        | **Renombrado/Reescrito** → `UIModeController.cs` | 3 |
| `UIManager.cs`                | Modificado      | 4         |
| `UICrossSectionPanel.cs`      | **Nuevo**       | 5         |
| `CrossSectionManager.cs`      | Sin cambios     | —         |
| `UIAnalyzePanel.cs`           | Sin cambios     | —         |
| `UIEnvironmentPanel.cs`       | Sin cambios     | —         |
| `UIDetailsSheet.cs`           | Sin cambios     | —         |
| `UIHeroController.cs`         | Sin cambios     | —         |
| `ViewModeToolbar.cs`          | Evaluar         | 6         |
| `EngineerToolbar.cs`          | Evaluar         | 6         |
| `KeyboardShortcuts.cs`        | Posible ajuste  | 6         |

---

## Tracking de Progreso

| Iteración | Descripción                              | Commit  | Estado      |
| --------- | ---------------------------------------- | ------- | ----------- |
| 1         | Expandir AppStateMachine                 | 2673e5a | ✅ Completada |
| 2         | Reestructurar MainLayout.uxml + USS      | 70b2a01 | ✅ Completada |
| 3+4       | UIModeController + Rewire UIManager      | f125f6f | ✅ Completada |
| 5         | Cross-Section UI en Analyze Mode         | 66673f3 | ✅ Completada |
| 6         | Cleanup & Integración Final              | —       | ✅ Completada |
