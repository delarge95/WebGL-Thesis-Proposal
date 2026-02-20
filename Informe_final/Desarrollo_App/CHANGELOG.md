# CHANGELOG — WebGL Drone Visualization Project

> Registro completo de la evolución del proyecto, desde la concepción hasta el estado actual.
> Fuentes: Git history, GitHub, Antigravity sessions, implementation plans, roadmaps.

---

## Leyenda de Prefijos

| Prefijo | Significado |
|---------|-------------|
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de bug |
| `style` | Cambios de estilo/UI |
| `docs` | Documentación |
| `chore` | Mantenimiento |
| `refactor` | Reestructuración sin cambio funcional |

---

## Fase 0 — Investigación y Selección Técnica *(Pre-Git)*

> Trabajo documentado en sesiones de Antigravity (conversación `c5f61e42`)

- Investigación de tendencias de portafolio Tech Artist (Hard Surface / Drones)
- Selección de modelo objetivo: Drone Sci-Fi de alto rendimiento
- Definición de features técnicos: Vista Explosionada, Rayos X, Delineado Técnico
- Definición de estilo UX/UI: Glassmorphism Premium
- Estudio de UI/UX (Moodboard + Referencias Técnicas)
- Diagramación y Wireframing (Grid System 12 columnas, wireframes de flujo, atomización UI)

---

## Fase 1 — Planificación y Configuración *(Pre-Git)*

> Documentado en roadmap original (`c5f61e42/task.md`)

- Creación de carpeta `desarrollo/` con estructura de proyecto
- Creación de hoja de ruta detallada (HOJA_DE_RUTA.md)
- Configuración de Git y Git LFS
- Definición de Namespaces, Assembly Definitions (Core, UI, Content)

---

## 2025-12-04 — Implementación Inicial del Prototipo

### Core Systems (Fases 3.5–3.16 completadas)

> Más de 40 managers/sistemas implementados en una sesión masiva.
> Documentado en conversación `ae9e51bf`.

**Arquitectura y Datos:**
- `GameManager` (Singleton de estado global)
- `DronePartData` ScriptableObjects (base de datos de partes)
- `EventBus` (sistema de desacoplamiento)
- `Singleton<T>` genérico (refactorizado a todos los managers)

**Cámara y Escena:**
- `OrbitCameraController` (Cinemachine — Orbit, Pan, Zoom)
- `SelectionManager` (Raycasting + Highlight visual)
- `ExplodedViewManager` (desplazamiento matemático con slider)

**UI Glassmorphism:**
- Design System (USS/Theme.uss)
- Layout Principal (MainLayout.uxml — barra lateral, header)
- Panel de Detalles con Data Binding
- `DetailsPanelController` (conexión SelectionManager ↔ UI)

**Audio y Feedback:**
- `AudioManager` (Singleton + control de volumen + SaveSystem)
- `TooltipSystem` (UI flotante)
- `NotificationManager` (Toast messages)

**Optimización y Quality:**
- `QualityManager` (Dynamic Resolution)
- `FPSCounter` (Debug Overlay)
- `ObjectPooler` (gestión de memoria)
- `MaterialController` (Property Blocks)
- `WebGLOptimizer` (optimizaciones de plataforma)

**Input y Debugging:**
- `InputManager` (abstracción de entrada)
- `RuntimeConsole` (log en pantalla)

**Persistencia y Assets:**
- `SaveSystem` (PlayerPrefs Wrapper)
- `AssetLoader` (Async Loading Stub)

**Features Extra:**
- `ScreenshotManager` (captura para tesis)
- `KeyboardShortcuts` (atajos profesionales)
- `PerformanceMonitor` (métricas detalladas)
- `SettingsPanel` (UI de configuración)
- `AccessibilityManager` (opciones de accesibilidad)
- `ErrorHandler` (Fallback global)

**View Modes (7 Shaders Custom):**
- `ViewModeManager` — X-Ray, Blueprint, Solid, Realistic, Wireframe, Ghosted, Thermal
- `ViewModeToolbar` (UI de cambio)
- `ClippableLit.shader` (corte transversal)
- `XRay.shader` (Fresnel + doble pass)
- `Blueprint.shader` (grid técnico + outline)
- `Thermal.shader` (gradiente de calor animado)
- `Wireframe.shader` (geometry shader)
- `SolidColor.shader` (con outline)
- `Ghosted.shader` (fresnel + depth fade)

**Part Catalog y Visibilidad:**
- `PartCatalogManager` (búsqueda y listado)
- `PartCatalogUI` (scroll, filtros, visibilidad)
- `PartVisibilityManager` (ocultar/mostrar/aislar con fade)

**Advanced Features:**
- `CrossSectionManager` (cortes X/Y/Z con plano visual)
- `DroneStateController` (On/Off, Idle, Flying, StartingUp, ShuttingDown)
- `EnhancedInfoPanel` (Detalles + Hide/Isolate/Focus)
- `ModularPartsSystem` (Slots + Swap con animaciones)

**Engineer Tooling (Fase 3.16):**
- `AssemblyGuideManager` + `AssemblyStepUI` (guía paso a paso)
- `MeasurementTool` (distancia, ángulo, radio)
- `ConnectionPointsViewer` + `ConnectionPointMarker`
- `BillOfMaterialsManager` (CSV export, totales)
- `AnnotationSystem` (notas 3D con billboard)
- `AssemblyChecklist` (verificación con progreso)
- `EngineerToolbar` (acceso rápido)

### Git Commits (2025-12-04)

| Hash | Mensaje |
|------|---------|
| `6d8ddf9` | feat: Complete implementation of WebGL drone visualization prototype |
| `c935277` | feat: Add scene setup utilities and AssemblyGuideData |
| `b7a0e80` | docs: Add README, CONTRIBUTING, setup guide and improve code documentation |
| `1c19ecc` | feat: Add complete project assets and documentation |
| `99f38a5` | docs: Add comprehensive presentation design system for Kimi K2 |
| `ad44d00` | docs: Consolidate presentation docs into single folder |

---

## 2025-12-05 — Landing Page y GitHub Pages

### Trabajo Realizado

- **Landing Page Premium**: HTML + CSS + GSAP scrollytelling
- **Doc Viewer**: Visor de documentación integrado con soporte Mermaid
- **GitHub Pages**: Configuración y deploy (`.nojekyll`, fix 404)
- **Mobile**: Swipe navigation, zoom Mermaid lightbox
- **Múltiples iteraciones de fix**: CSS specificity, layout duplicates, JS robustez

### Git Commits (2025-12-05)

| Hash | Mensaje |
|------|---------|
| `2e5a79c` | feat: Add complete project support materials |
| `17152fe` | feat: Premium landing page + fix cronograma dates |
| `9f92050` | feat: Awwwards-style landing page with GSAP scrollytelling |
| `f2943c7` | fix: Inline CSS/JS in landing page for better portability |
| `4fca23f` | fix: Add ScrollToPlugin and link local docs |
| `2e0f34b` | fix: Add missing Unity project configuration files |
| `a8ce0f0` | fix: Optimize cursor performance and fix features/docs visibility |
| `71f6b90` | feat: Redesign landing page - Minimalist & Seamless Transition |
| `720bfee` | feat: Add scroll pinning effect and restore documentation links |
| `2af00e0` | fix: Increase mobile margins and padding |
| `072402d` | fix: Significantly increase mobile margins to 3rem |
| `309d7e2` | fix: Resolve CSS specificity issue to ensure mobile margins |
| `2fb1a4f` | feat: Implement Integrated Doc Viewer with Mermaid Support |
| `f34af03` | feat: Enhanced Doc Viewer with Nav, Swipe, and Path Fixes |
| `54eb9a5` | fix: Remove duplicate section and refine layout |
| `6b1a651` | fix: Refactor HTML layout to Section > Container |
| `5b99de8` | fix: Restore Doc section and restrict grid width |
| `ad512e0` | fix: Repair malformed HTML in Features section |
| `78e04a7` | fix: Restore advanced Doc Viewer JS logic |
| `20a1cc8` | fix: Robust JS logic for DocViewer |
| `c11ee5b` | feat(docs): refine doc viewer, fix layout duplicates, add PDF PDFs |
| `dc0cf8f` | chore: add .nojekyll to fix github pages 404 |
| `219524c` | chore: remove interfering unity-build workflow to unblock pages |
| `4fe2e1f` | feat(mobile): add swipe nav, hide arrows, mermaid lightbox |

---

## 2025-12-08 — Migración React + Framer Motion

### Trabajo Realizado

> Documentado en `MICROINTERACTIONS_PLAN.md`

- Setup React + Vite + Framer Motion
- Arquitectura de componentes para micro-interacciones temáticas por card
- 7 animaciones únicas diseñadas (X-Ray scan, explosion, blueprint, render, shader, compile, color grading)
- Fallback: mantenimiento de versión HTML estática

### Git Commits (2025-12-08)

| Hash | Mensaje |
|------|---------|
| `0c80d85` | feat: setup React + Framer Motion, fix visibility bug, remove card numbering |
| `85d0845` | fix: temporarily use static HTML deployment for GitHub Pages |
| `70df5cc` | feat: complete React architecture with component structure |

---

## 2025-12-11 — Compilación Fixes y Git LFS

### Trabajo Realizado

> Documentado en conversación `23c3d7bb` (Debugging Edge Detection)

- Configuración de Git LFS para assets binarios
- Fix de errores de compilación: `DronePartData` propiedades faltantes
- Fix de UI Toolkit syntax errors en 8+ archivos (`borderRadius`, `borderWidth`, `placeholder`)
  - `NotificationManager.cs`, `LoadingController.cs`, `EngineerToolbar.cs`
  - `EnhancedInfoPanel.cs`, `AssemblyStepUI.cs`, `PartCatalogUI.cs`
  - `TooltipSystem.cs`, `ViewModeToolbar.cs`, `SettingsPanel.cs`
- Resolvidos warnings de USS e Input System

### Git Commits (2025-12-11)

| Hash | Mensaje |
|------|---------|
| `5669c25` | chore: configure Git LFS for binary assets (models, textures, audio) |
| `06fb92a` | Fix compilation errors: Updated DronePartData, fixed UI Toolkit syntax |
| `479fb69` | Resolved USS warnings and updated task list |

---

## 2026-02-10 — UI Profesional + Shaders Integration

### Trabajo Realizado

- **Step 1-2**: Reset button, Info Sheet, Visual Viewport Shift
- **Shader Integration**: Todos los shaders (X-Ray, Blueprint, Thermal, etc.) conectados a UI con cycling
- **High-DPI Mobile**: Redimensionamiento completo (botones 100px, fuentes 24px+)
- **Iteraciones de posicionamiento**: Múltiples ajustes de UI (530px → 150px vertical)

### Git Commits (2026-02-10)

| Hash | Mensaje |
|------|---------|
| `d38744f` | feat: Implement Professional UI (Step 1 & 2) - Reset, Info Sheet, Visual Viewport Shift |
| `47857c5` | Refactor: Renamed UI categories (Electronics/Energy) and fixed UIManager compilation errors |
| `6d0952d` | Feat: Integrated all shaders (XRay, Blueprint, Thermal, etc.) with UI cycling |
| `26b22c3` | Style: Resized UI for High-DPI Mobile (1320x2860), buttons 100px, fonts 24px+ |
| `12de251` | Style: Vertically spread UI elements for High-DPI mobile |
| `6277396` | Style: Lowered UI groups by 40px to tighten layout |
| `67a93d9` | Style: Aggressively lowered UI groups |

---

## 2026-02-11 — Polish UI + Smart Hotspots

### Trabajo Realizado

- **Selection Label**: Fade out/in al abrir/cerrar Details Sheet (safe opacity)
- **Floating Details Sheet**: Bottom 260px, botones empujan 480px
- **Smart Hotspots**: Sistema de hotspots Screen-Space (UI Toolkit)
- **URP Asset Tool**: Herramienta de editor para gestionar rendering
- **2026 Minimalist Aesthetic**: Rediseño completo de Hotspots y MainTheme
- **Bug Fixes**: Auto-open sheet, layout z-order, slider hidden, shader buttons

### Git Commits (2026-02-11)

| Hash | Mensaje |
|------|---------|
| `c86f3cc` | Style: Lowered UI to limit (Menu @ 180px, Slider @ 300px) |
| `5eab07e` | Style: Lowered UI again by 30px |
| `4dc6ce6` | Polish: Selection Label fades out when Details Sheet opens |
| `32d49b5` | Revert "Polish: Selection Label fades out when Details Sheet opens" |
| `dead62e` | Polish: Selection Label fades out (Safe Opacity Implementation) |
| `3cc5d6a` | Style: Floating Details Sheet (Bottom 260px) |
| `e7e6eac` | Polish: Synced BottomBar and DetailsSheet animation |
| `cdb3d1b` | Style: Reduced button shift by 60px |
| `62a2866` | Style: Lowered Sheet assembly by 100px |
| `83c1820` | Fix: Restored Details Sheet height to 420px |
| `63e02ff` | Feat: Smart Hotspots (Screen-Space UI Toolkit) |
| `f6633de` | Fix: Remove duplicate CSS import |
| `d8a606b` | Fix: Resolve CS0246 (Singleton namespace) and USS warnings |
| `763eb59` | Fix: Resolve CS0246 (SelectionManager, DronePartData) |
| `6383b06` | Feat: Add URP Asset Tool & Polish UI Spacing (8pt grid) |
| `17ae084` | feat(ui): Rediseño completo Hotspots y MainTheme (2026 Minimalist) |

---

## 2026-02-12 — Bug Fixes Iterativos (9 Fases)

### Trabajo Realizado

- **9 fases de bug fixes** resolviendo interacciones UI:
  - Auto-open info sheet (solo hotspot, no part click)
  - Posicionamiento matemático y stacking dinámico
  - Z-Order blocking y Thermal Button Layout
  - Info Sheet responsive offset, drag dismiss, zoom block
  - Compilación: constantes duplicadas, campos no usados
  - Click blocking y text selection en Info Sheet
  - CSS warnings y C# picking logic
  - Text selection extendido a Full Info Sheet
  - Block 3D Selection on UI Interaction
- **WebGL Build**: Asegurar tracking del build

### Git Commits (2026-02-12)

| Hash | Mensaje |
|------|---------|
| `d74f893` | fix(ui): Phase 1-2 bug fixes |
| `acea950` | fix(ui): Bug 1 - only auto-open info sheet on hotspot click |
| `99fb17e` | Fix UI Layout: Correct math positioning, dynamic stacking, slider init |
| `4c37da1` | Fix UI Interaction: Z-Order blocking (Bug 5) and Thermal Button Layout |
| `076acf6` | Implement Phase 3: Info Sheet Polish (Responsive Offset, Drag Dismiss) |
| `d8eefbf` | Fix Compilation Errors: Duplicate Constants & Unused Fields |
| `8db5f93` | Implement Phase 5: Info Sheet Click Blocking & Text Selection |
| `8e46c79` | Fix Syntax & API Warnings (Phase 6) |
| `b26641a` | Fix CSS Warnings & Enforce C# Picking Logic (Phase 7) |
| `8c3406c` | Extend Text Selection to Full Info Sheet (Phase 8) |
| `4d618d2` | Block 3D Selection on UI Interaction (Phase 9) |
| `9007d13` | Ensure WebGL Build is tracked |

---

## 2026-02-18 — UI/UX Overhaul v2.1 (Sesión Actual)

### Trabajo Realizado

> Plan documentado en `implementation_plan.md` y `PLAN_UI_OVERHAUL_v2.md`

**Hero Menu Redesign:**
- 4 botones principales (Explore, Select Device, About, Exit)
- Device selector integrado como submenu
- Home button
- Space Grotesk font + Inter global

**Visual Unification (Match Web Landing Page):**
- Accent color: Cyan (`#00D9FF`) → Blanco (`#FFFFFF`)
- 9 selectores actualizados (`.btn-tag--active`, `.shader-mode-btn--active`, `.env-preset-btn--active`, slider dragger, tag-dot-accent, etc.)
- Background: `#050505` (near-black, matches web)
- Typography: Inter global, Space Grotesk Regular titles
- Button styling: transparent + pill

**Bug Fixes — Phase 1:**
1. Device selector width: `320px → 400px` (clipping fix)
2. Canvas background: `EnvironmentController.ApplyPreset("Studio")` bg fixed to `#050505`
3. Info panel text wrapping: `white-space: normal` en inner TextField + `align-items: flex-start`
4. Close button: `StopPropagation()` to prevent header toggle re-opening
5. Buttons position: `.ui-shifted 56% → 46%`
6. Ghost clicks: `display: none` en 4 estados hidden
7. Slider centering: `translate: -50% 0` + padding simétrico `16px 32px`
8. USS syntax fix: Removed invalid `~` combinator and `overflow: visible`

**Documentation — Phase 4:**
- `Informe_final/Desarrollo_App/` folder structure
- `CHANGELOG.md` (this file)
- `DESIGN_TOKENS.md` (full design system spec)
- `TECHNOLOGY_STACK.md` (all tools and technologies)
- Archived roadmaps (`HOJA_DE_RUTA_v1.md`, `PLAN_UI_OVERHAUL_v2.md`)
- Session log

### Git Commits (2026-02-18)

| Hash | Mensaje |
|------|---------|
| `6b3b3ad` | fix: hero menu hitbox + redesign (4 buttons, device selector, home btn) |
| `140185e` | style: visual redesign to match web landing page |
| `42c6e90` | Fix UI bugs: About close, click blocking, info button, bg black, fonts |
| `51461c3` | Refactor UX: Unified Bottom Sheet (Delete AboutPanel, DeviceSelector) |
| `0229672` | feat(ui): implement Hero Submenus, Space Grotesk font, black bg fix |
| `93c4562` | fix(ui): resolve uxml syntax error and remove unused code |
| `332a9b6` | fix(ui): apply Space Grotesk font globally |
| `e3d72e5` | fix(ui): update hero title to Title Case and refine spacing |
| `b03d286` | fix(ui): match web typography (Inter global, Space Grotesk Regular titles) |
| `fa62502` | fix(ui): match web button styling (transparent + pill) |
| `ef2ec40` | feat(ui): match web styling (pills, spacing, no scrollbars) |
| `dc4d83b` | docs: update master plan v2.1 with user feedback |
| `16dc25f` | fix(ui): Phase 1+2 - white accent, ghost clicks, slider, device width, bg |
| `1a95185` | docs: Phase 4 - Create Desarrollo_App folder with CHANGELOG, DESIGN_TOKENS |
| `0d6754a` | fix(ui): remove invalid USS syntax (~combinator, overflow:visible) |
| `cfd7ee1` | fix(ui): studio bg #050505, slider centered, text wrap, close btn fix |

---

## 2026-02-20 — Phase 3: Technical Architecture Refactoring

### Trabajo Realizado

> Plan documentado en `implementation_plan.md` y `ARCHITECTURE_AUDIT_REPORT.md`

**Step 1: UI Toolkit Memory Leaks Resolvidos:**
- Implementación de patrón `AddCleanup` con cola de `System.Action` en `UIManager.cs`.
- Conversión de +50 lambdas anónimas estáticas (`detailsSheet.RegisterCallback`, `explosionSlider.RegisterValueChangedCallback`, `btn.clicked +=`) a instancias cacheadas.
- Llamada obligatoria a `UnsubscribeFromUIEvents()` dentro de `OnDisable()`.
- Eliminación total del vector de fuga de memoria que multiplicaba listeners cada vez que se recargaba el HUD del drone.

### Git Commits (2026-02-20)

| Hash | Mensaje |
|------|---------|
| (pending) | refactor: implement comprehensive UI Toolkit event cleanup in UIManager to prevent memory leaks |

---

## Pendiente

- **Phase 2.2**: Animated gradient background (como web hero)
- **Phase 3**: Grid submenus con Freepik icons, bottom bar container unificado
- **Phase 5**: RAG / Knowledge Graph updates
- **Modelado 3D**: High-poly drone en Blender (pendiente desde Fase 3)
- **Pruebas de usabilidad**: SUS, NASA-TLX
- **Video demostración**: Showreel final

---

## Estadísticas

| Métrica | Valor |
|---------|-------|
| Total commits | 88 |
| Período | Dec 4, 2025 – Feb 18, 2026 (76 días) |
| C# Scripts | ~50+ |
| Custom Shaders | 7 (X-Ray, Blueprint, Thermal, Wireframe, SolidColor, Ghosted, ClippableLit) |
| Managers/Singletons | ~30+ |
| USS Stylesheets | 2 (Theme.uss, MainTheme.uss) |
| UXML Layouts | 1+ (MainLayout.uxml) |
| Custom Skills | 10 (Antigravity) |
| Antigravity Sessions | 6+ |
