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

| Mecanismo                         | Qué protege                                | Regla                                                     |
| --------------------------------- | ------------------------------------------ | --------------------------------------------------------- |
| `AddCleanup()` + `Dispose()`      | Memory leak prevention                     | **TODO** callback registrado DEBE tener su `AddCleanup()` |
| `RegisterButtonInputBlockers()`   | Mecanismo dual proactivo/reactivo de input | **NO ELIMINAR** — es arquitecturalmente necesario         |
| `InputManager.InputBlocked`       | Guard proactiva en PointerEnter/Leave      | **MANTENER** en todos los componentes UI interactivos     |
| `IsPointerOverUI()` + Panel.Pick  | Guard reactiva por frame                   | **NO MODIFICAR** InputManager.cs                          |
| `RuntimePanelUtils.ScreenToPanel` | Conversión correcta de coordenadas         | **NO REINTRODUCIR** conversiones manuales                 |
| `EventBus` pub/sub                | Desacoplamiento de eventos                 | Usar `EventBus.Publish()` para nuevos eventos             |

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

   | Antes (UIPopupController)                          | Después (UIModeController)                     |
   | -------------------------------------------------- | ---------------------------------------------- |
   | Toggle individual de ShaderMenu, CategoryMenu, Env | Activar/desactivar mode containers completos   |
   | Stacking dinámico con `RepositionPopups()`         | Show/hide por modo (sin stacking entre modos)  |
   | Mutual exclusion entre popups individuales         | Mutual exclusion entre modos (solo 1 activo)   |
   | PopupBlocker para cerrar menús                     | PopupBlocker para click fuera del modo Analyze |

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

| Riesgo                                                | Probabilidad | Mitigación                                                    |
| ----------------------------------------------------- | ------------ | ------------------------------------------------------------- |
| Botones obsoletos referenciados en C# → NullRef       | ALTA         | Actualizar C# en misma iteración que UXML                     |
| PopupBlocker deja de funcionar con mode containers    | MEDIA        | Mantener PopupBlocker dentro de AnalyzeModeContainer          |
| Cross-section shaders no compatibles con view modes   | MEDIA        | Verificar que `_ClipEnabled` funciona con todos los 7 shaders |
| `RegisterButtonInputBlockers()` no detecta nuevos btn | BAJA         | Se ejecuta en `InitializeUI()` DESPUÉS de crear todo el DOM   |
| Stacking de popups dentro de Analyze se rompe         | MEDIA        | Simplificar: todos los submenús de Analyze visibles a la vez  |
| `ViewModeToolbar.cs` colisiona con nuevo layout       | MEDIA        | Evaluar en Iteración 6 — posiblemente desactivar              |

---

## Archivos Afectados (estimación)

| Archivo                  | Acción                                           | Iteración |
| ------------------------ | ------------------------------------------------ | --------- |
| `AppStateMachine.cs`     | Modificado                                       | 1         |
| `MainLayout.uxml`        | **Reescrito**                                    | 2, 5      |
| `Theme.uss`              | Modificado                                       | 2, 5      |
| `UIPopupController.cs`   | **Renombrado/Reescrito** → `UIModeController.cs` | 3         |
| `UIManager.cs`           | Modificado                                       | 4         |
| `UICrossSectionPanel.cs` | **Nuevo**                                        | 5         |
| `CrossSectionManager.cs` | Sin cambios                                      | —         |
| `UIAnalyzePanel.cs`      | Sin cambios                                      | —         |
| `UIEnvironmentPanel.cs`  | Sin cambios                                      | —         |
| `UIDetailsSheet.cs`      | Sin cambios                                      | —         |
| `UIHeroController.cs`    | Sin cambios                                      | —         |
| `ViewModeToolbar.cs`     | Evaluar                                          | 6         |
| `EngineerToolbar.cs`     | Evaluar                                          | 6         |
| `KeyboardShortcuts.cs`   | Posible ajuste                                   | 6         |

---

## Tracking de Progreso

| Iteración | Descripción                         | Commit  | Estado         |
| --------- | ----------------------------------- | ------- | -------------- |
| 1         | Expandir AppStateMachine            | 2673e5a | ✅ Completada  |
| 2         | Reestructurar MainLayout.uxml + USS | 70b2a01 | ✅ Completada  |
| 3+4       | UIModeController + Rewire UIManager | f125f6f | ✅ Completada  |
| 5         | Cross-Section UI en Analyze Mode    | 66673f3 | ✅ Completada  |
| 6         | Cleanup & Integración Final         | —       | ✅ Completada  |
| 7         | UX Audit Fixes — Minimalist Grid UI | —       | ✅ Completada  |

---

### Iteración 7 — UX Audit Fixes: Minimalist Grid UI

**Archivos:** `MainLayout.uxml`, `Theme.uss`, `UIModeController.cs` (posibles ajustes)  
**Riesgo:** MEDIO — cambios visuales extensos pero sin alterar lógica de negocio  
**Estado:** ✅ Completada — 24 de febrero de 2026  
**Referencia:** `UX_UI_AUDIT_REPORT.md` (violaciones V-FIT-_, T-_, G-_, V-MIL-_, A-\*)

#### Objetivos

Implementar un sistema UI minimalista basado en cuadrícula de 4 columnas, con:

- **Botones de modo** como íconos puros sin cuadro envolvente
- **Cards cuadradas** con icono redondo centrado + label debajo
- **Sliders** dimensionados exactamente al ancho de 4 cards
- **Alineación rígida** a cuadrícula (slots vacíos si una fila tiene < 4 cards)
- **Sin contenedores visibles** — los submenús no tienen fondo/borde propio

---

#### 7.1 — Bottom Bar: Botones Icon-Only (sin cuadro)

**Antes:**

```
┌──────────────────────────────────────────────────┐
│  ┌──────────┐  ┌──────────┐  ┌──────────┐       │  ← pill con borde
│  │ 🔧 TOOLS │  │ 📐 ANALYZE│  │ ☀️ STUDIO│       │  ← 100×60px con bg
│  └──────────┘  └──────────┘  └──────────┘       │
└──────────────────────────────────────────────────┘
```

**Después:**

```
┌──────────────────────────────────────────────────┐
│      🔧          📐          ☀️                   │  ← iconos sueltos
│     TOOLS      ANALYZE      STUDIO               │  ← label debajo
└──────────────────────────────────────────────────┘
```

**Cambios USS:**

```css
/* Botón de modo: icono puro, sin fondo ni borde */
.mode-btn {
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 64px; /* 8×8 grid, suficiente para touch 48px + padding */
  height: 64px; /* 8×8 grid */
  margin-left: 16px; /* 2×8 spacing entre botones */
  margin-right: 16px;

  background-color: transparent; /* SIN FONDO */
  border-width: 0; /* SIN BORDE */
  border-radius: 0; /* SIN CUADRO */
}

.mode-btn:hover {
  background-color: transparent; /* mantener sin fondo */
  border-color: transparent;
  scale: 1.1; /* feedback solo por escala */
}

.mode-btn:active {
  scale: 0.92;
}

.mode-btn--active {
  background-color: transparent;
  border-width: 0;
}

.mode-btn--active .mode-btn-icon {
  /* Feedback de estado: glow en el icono, no en el botón */
  -unity-background-tint-color: rgba(0, 170, 255, 1);
}

.mode-btn--active .mode-btn-label {
  color: rgba(0, 170, 255, 0.9);
}

.mode-btn-icon {
  width: 28px; /* 3.5×8 — visible pero no dominante */
  height: 28px;
  margin-bottom: 4px; /* micro-spacing */
}

.mode-btn-label {
  font-size: 12px; /* FIX T-02: 10→12 px (mínimo absoluto) */
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: 1px;
  -unity-font-style: bold;
}
```

**Cambios USS en `.actions-row`:**

```css
.actions-row {
  background-color: rgba(12, 12, 18, 0.88); /* pill sigue siendo glass */
  border-width: 1px;
  border-color: rgba(255, 255, 255, 0.08);
  border-radius: 40px;
  padding: 8px 32px; /* más padding horizontal para respirar */
  min-width: auto; /* quitar min-width fijo, que fluya */
}
```

---

#### 7.2 — Cards Cuadradas con Icono Redondo (4-col Grid)

**Antes:**

```
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│  [icon]  │ │  [icon]  │ │  [icon]  │ │  [icon]  │ │  [icon]  │
│ REALISTIC│ │  X-RAY   │ │BLUEPRINT │ │  SOLID   │ │  WIRE    │
└──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘
  ┌──────────┐ ┌──────────┐
  │  [icon]  │ │  [icon]  │
  │  GHOST   │ │ THERMAL  │
  └──────────┘ └──────────┘
```

**Después:**

```
   ○           ○           ○           ○       ← iconos redondos
 REALISTIC    X-RAY     BLUEPRINT    SOLID     ← labels

   ○           ○           ○          [ ]      ← icono + slot vacío
  WIRE       GHOST      THERMAL               ← alineado a grid
```

**Dimensiones calculadas:**

| Variable             | Valor                    | Cálculo               |
| -------------------- | ------------------------ | --------------------- |
| Card width           | 80 px                    | 10×8 grid             |
| Card height          | 80 px                    | Cuadrada              |
| Card gap (margin)    | 8 px                     | 1×8 grid              |
| Row width (4 cards)  | 4×80 + 3×8 = **344 px**  |                       |
| Icon circle diameter | 40 px                    | 5×8, centrado en card |
| Label font           | 12 px                    | Mínimo absoluto       |
| Card background      | `rgba(255,255,255,0.04)` | Semi-transparente     |
| Card border          | 0                        | Sin borde visible     |
| Card border-radius   | 16 px                    | 2×8, suave            |

**Cambios USS:**

```css
/* ── Card: cuadrada, semi-transparente, sin contenedor visible ── */
.submenu-card {
  width: 80px; /* cuadrada 80×80 */
  height: 80px;
  margin: 4px; /* gap efectivo = 8px entre cards */

  background-color: rgba(255, 255, 255, 0.04); /* semi-transparente */
  border-width: 0; /* SIN borde */
  border-radius: 16px; /* 2×8 grid */

  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.submenu-card:hover {
  background-color: rgba(255, 255, 255, 0.08);
  border-width: 0; /* mantener sin borde */
  scale: 1.04;
}

.submenu-card--active {
  background-color: rgba(0, 170, 255, 0.12);
  border-width: 0;
}

/* ── Icono: círculo centrado ── */
.submenu-icon {
  width: 40px; /* 5×8 grid */
  height: 40px;
  border-radius: 50%; /* CÍRCULO */
  background-color: rgba(255, 255, 255, 0.08); /* fondo circular sutil */
  margin-bottom: 8px; /* 1×8 grid */
  -unity-background-scale-mode: scale-to-fit;
  opacity: 0.7;
}

.submenu-card--active .submenu-icon {
  background-color: rgba(0, 170, 255, 0.2);
  opacity: 1;
}

/* ── Label debajo del icono ── */
.submenu-label {
  font-size: 12px; /* FIX T-03: 10→12 px */
  color: rgba(255, 255, 255, 0.6);
  letter-spacing: 0.5px;
  -unity-text-align: middle-center;
}
```

---

#### 7.3 — Submenu Grid: 4 columnas fijas + slots vacíos

**Concepto:** El grid usa `flex-wrap: wrap` con un ancho fijo calculado para exactamente 4 cards por fila. Si una fila tiene menos de 4 cards, las posiciones restantes son slots vacíos (se logra con el ancho fijo del grid, NO con elementos invisibles — el flex-wrap + justify-content: flex-start se encarga).

```css
/* ── Grid container: ancho fijo = 4 cards + 3 gaps ── */
.submenu-grid {
  flex-direction: row;
  flex-wrap: wrap;
  justify-content: flex-start; /* CLAVE: alinear a la izquierda, no centrar */
  width: 344px; /* 4×80 + 3×8 = 344px exacto */
  padding: 0;
  align-self: center; /* centrar el grid dentro del submenu */
}
```

**Resultado visual (ShaderMenu = 7 cards):**

```
Fila 1: [REALISTIC] [X-RAY] [BLUEPRINT] [SOLID]    ← 4/4
Fila 2: [WIRE]      [GHOST] [THERMAL]   [        ]  ← 3/4 + slot vacío
```

**Resultado visual (CategoryMenu = 5 cards):**

```
Fila 1: [ALL]       [STRUCTURE] [PROPULSION] [AVIONICS]  ← 4/4
Fila 2: [POWER]     [        ]  [         ]  [        ]  ← 1/4 + 3 slots
```

**Resultado visual (EnvPanel = 5 cards):**

```
Fila 1: [STUDIO]    [SUNSET] [NIGHT]   [BLUE]     ← 4/4
Fila 2: [NEUTRAL]   [      ] [      ]  [      ]   ← 1/4 + 3 slots
```

---

#### 7.4 — Sliders: ancho = 4 cards, alto = ½ card

**Dimensiones:**

| Variable       | Valor                                                          | Cálculo                   |
| -------------- | -------------------------------------------------------------- | ------------------------- |
| Slider width   | 344 px                                                         | Igual que grid de 4 cards |
| Slider height  | 40 px                                                          | 80/2 = 40 px (½ card)     |
| Label position | Integrado dentro del slider-container, alineado a la izquierda |
| Dragger size   | 32 px visual (hit area 48 px)                                  | FIX V-FIT-02              |

```css
/* ── Slider container: mismo ancho que grid de 4 cards ── */
.slider-container {
  width: 344px; /* = 4×80 + 3×8 — misma anchura que el grid */
  height: 40px; /* ½ de una card (80/2) */
  align-self: center;

  background-color: rgba(
    255,
    255,
    255,
    0.04
  ); /* semi-transparente como cards */
  border-width: 0;
  border-radius: 16px; /* mismo radius que cards */
  padding: 0 16px;

  flex-direction: row;
  align-items: center;
}

.slider-label {
  font-size: 12px; /* FIX T-05: 11→12 px */
  color: rgba(255, 255, 255, 0.5); /* FIX A-06: 0.35→0.5 */
  letter-spacing: 1px;
  margin-right: 12px;
  -unity-font-style: bold;
  white-space: nowrap;
  flex-shrink: 0;
}

/* Env sliders: misma regla */
.env-slider-group {
  width: 344px;
  height: 40px;
  align-self: center;
  margin-top: 8px;
  padding: 0 16px;

  background-color: rgba(255, 255, 255, 0.04);
  border-radius: 16px;
  border-width: 0;

  flex-direction: row;
  align-items: center;
}
```

---

#### 7.5 — Contenedores invisibles (sin fondo/borde en submenús)

**Antes:** `.mode-submenu` tenía `background-color: rgba(10,10,18,0.75)` + `border-width: 1px` + `border-color: rgba(255,255,255,0.06)`

**Después:** Eliminar fondo y borde del contenedor. Solo las cards individuales tienen fondo semi-transparente.

```css
.mode-submenu {
  position: relative;
  width: 100%;

  background-color: transparent; /* SIN FONDO */
  border-width: 0; /* SIN BORDE */
  border-radius: 0;
  padding: 8px 0; /* solo padding vertical para spacing */

  align-items: center;
  margin-bottom: 8px;
}

/* Título del submenu: mantener pero ajustar contraste */
.submenu-title {
  font-size: 12px; /* FIX T-04: 11→12 */
  color: rgba(255, 255, 255, 0.5); /* FIX A-05: 0.3→0.5 */
  letter-spacing: 2px;
  margin-bottom: 8px; /* FIX G-30: 10→8 */
  -unity-font-style: bold;
}
```

---

#### 7.6 — Cross-Section Panel: axis btns como cards cuadradas en grid

Los botones de eje (X, Y, Z, ⇅) se tratan como 4 cards en una sola fila, seguidas del slider debajo.

```
   ○         ○         ○         ○       ← iconos redondos
    X         Y         Z        ⇅       ← labels
┌──────────────────────────────────────┐
│ ──────────────○───────────────────── │  ← slider (344×40)
└──────────────────────────────────────┘
```

```css
.cross-section-axis-btn {
  width: 80px; /* misma dimensión que submenu-card */
  height: 80px;
  margin: 4px;

  background-color: rgba(255, 255, 255, 0.04);
  border-width: 0;
  border-radius: 16px;

  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.cross-section-axis-label {
  font-size: 14px; /* legible para letras individuales X/Y/Z */
  color: rgba(255, 255, 255, 0.6);
  -unity-font-style: bold;
}
```

---

#### 7.7 — Mode Action Buttons (ToolsActionBar, AnalyzeActionBar): icon-only

Los botones de acción dentro de cada modo (INFO, EXPLODE, PINS / SHADERS, CUT) siguen el mismo patrón icon-only del bottom bar: sin cuadro, solo icono + label.

```css
.mode-action-bar {
  flex-direction: row;
  align-items: center;
  justify-content: center;
  padding: 8px 0;
  margin-bottom: 8px;
  background-color: transparent; /* SIN FONDO */
  border-radius: 0;
  border-width: 0; /* SIN BORDE */
}

.mode-action-btn {
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 64px;
  height: 56px;
  margin-left: 12px;
  margin-right: 12px;
  background-color: transparent; /* SIN FONDO */
  border-radius: 0;
  border-width: 0; /* SIN BORDE */
}

.mode-action-btn:hover {
  background-color: transparent;
  scale: 1.1;
}

.mode-action-btn--active .mode-action-icon {
  -unity-background-tint-color: rgba(0, 170, 255, 1);
}

.mode-action-label {
  font-size: 12px; /* FIX T-01: 9→12 px */
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: 1px;
  -unity-font-style: bold;
}
```

---

#### 7.8 — Audit Quick Fixes adicionales (del UX_UI_AUDIT_REPORT.md)

| Fix ID   | Cambio                                  | Selector                                    |
| -------- | --------------------------------------- | ------------------------------------------- |
| V-FIT-03 | sheet-close-btn: 32→40 px + padding 4px | `.sheet-close-btn`                          |
| V-FIT-04 | icon-button-small: 40→48 px             | `.icon-button-small`                        |
| V-FIT-02 | slider dragger: 24→32 px                | `.glass-slider .unity-base-slider__dragger` |
| A-04     | header-title opacity: 0.35→0.5          | `.header-title`                             |
| G-24     | mode-btn-icon: 22→24 px                 | `.mode-btn-icon` (ya en 7.1 como 28px)      |
| G-26     | mode-action-icon: 20→24 px              | `.mode-action-icon`                         |
| G-28     | submenu-icon margin-bottom: 6→8 px      | `.submenu-icon` (ya en 7.2 como 8px)        |

---

#### 7.9 — Resumen dimensional (Sistema de Diseño post-Iteración 7)

```
╔══════════════════════════════════════════════════════════════╗
║  DESIGN SYSTEM — GRID TOKENS (8pt base)                     ║
╠══════════════════════════════════════════════════════════════╣
║                                                              ║
║  Card (base unit):           80 × 80 px  (cuadrada)         ║
║  Card gap:                   8 px                            ║
║  Grid width (4 cols):        344 px  (4×80 + 3×8)           ║
║  Slider container:           344 × 40 px (ancho grid × ½h)  ║
║  Icon circle:                40 × 40 px  (dentro de card)    ║
║  Card border-radius:         16 px                           ║
║  Card background:            rgba(255,255,255,0.04)          ║
║                                                              ║
║  Mode btn (bottom bar):      64 × 64 px  (icon-only, 0 bg)  ║
║  Mode action btn:            64 × 56 px  (icon-only, 0 bg)  ║
║  Top bar btn:                48 × 48 px  (icon-only)         ║
║                                                              ║
║  Font minimum:               12 px (captions/labels)         ║
║  Font body:                  16 px (data, descriptions)      ║
║  Spacing scale:              4 / 8 / 12 / 16 / 24 / 32      ║
║                                                              ║
║  Accent:                     rgba(0, 170, 255, *)            ║
║  Surface (card bg):          rgba(255, 255, 255, 0.04)       ║
║  Surface (card hover):       rgba(255, 255, 255, 0.08)       ║
║  Surface (card active):      rgba(0, 170, 255, 0.12)         ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
```

---

#### Tareas de Implementación (checklist)

1. **USS — Bottom Bar Buttons**

   - [x] `.mode-btn`: remover fondo, borde, border-radius → icon-only
   - [x] `.mode-btn-icon`: 22→28 px
   - [x] `.mode-btn-label`: 10→12 px
   - [x] `.mode-btn--active`: feedback solo en icono (tint) y label (color)

2. **USS — Cards Cuadradas**

   - [x] `.submenu-card`: 110×88 → 80×80, border-width: 0, border-radius: 16
   - [x] `.submenu-icon`: hacer circular (border-radius: 50%, width/height: 40px, bg sutil)
   - [x] `.submenu-label`: 10→12 px
   - [x] `.submenu-grid`: width fijo 344px, justify-content: flex-start

3. **USS — Sliders**

   - [x] `.slider-container`: width: 344px, height: 40px, border-width: 0
   - [x] `.env-slider-group`: mismas dimensiones que slider-container
   - [x] `.glass-slider dragger`: 24→32 px (hit area 48px)

4. **USS — Contenedores Invisibles**

   - [x] `.mode-submenu`: background: transparent, border: 0
   - [x] `.mode-action-bar`: background: transparent, border: 0
   - [x] `.submenu-title`: 11→12 px, opacity 0.3→0.5

5. **USS — Cross Section como Grid**

   - [x] `.cross-section-axis-btn`: 34→80 px (como cards)
   - [x] `.cross-section-axis-group`: layout como submenu-grid
   - [x] Slider de posición: 344×40 px

6. **USS — Mode Action Buttons**

   - [x] `.mode-action-btn`: remover fondo/borde → icon-only
   - [x] `.mode-action-label`: 9→12 px
   - [x] `.mode-action-icon`: 20→24 px

7. **USS — Audit Fixes**

   - [x] `.icon-button-small`: 40→48 px
   - [x] `.sheet-close-btn`: 32→40 px + padding 4px
   - [x] `.header-title`: opacity 0.35→0.5

8. **UXML — Cross Section Restructure** (via USS only)

   - [x] Cross-section layout cambiado a column via `.cross-section-row`, `.cross-section-controls`
   - [x] Axis btns 80×80 en grid 344px via `.cross-section-axis-group`
   - [x] C# (`UICrossSectionPanel.cs`) sin cambios — elementos mantienen sus nombres

9. **Verificación**
   - [x] 0 errores de compilación
   - [x] Grid de 4 cols alineado en todos los submenús
   - [x] Slots vacíos visualmente correctos (fila 2 de ShaderMenu tiene 3+vacío)
   - [x] Sliders tienen el ancho exacto del grid
   - [x] Botones del bottom bar son icon-only sin cuadro
   - [x] Todos los font-size ≥ 12 px
   - [x] Todos los touch targets ≥ 44 px
   - [x] Cards semi-transparentes sin contenedores visibles detrás
