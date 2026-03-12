# CHANGELOG — WebGL Drone Visualization Project

> Registro completo de la evolución del proyecto, desde la concepción hasta el estado actual.
> Fuentes: Git history, GitHub, Antigravity sessions, implementation plans, roadmaps.

---

## Leyenda de Prefijos

| Prefijo    | Significado                           |
| ---------- | ------------------------------------- |
| `feat`     | Nueva funcionalidad                   |
| `fix`      | Corrección de bug                     |
| `style`    | Cambios de estilo/UI                  |
| `docs`     | Documentación                         |
| `chore`    | Mantenimiento                         |
| `refactor` | Reestructuración sin cambio funcional |

---

## 2026-03-12 - Thermal Simulation Foundation

### Trabajo Realizado

- `feat(thermal)`: Se consolido `ThermalSimulationManager` como solver termico reducido por componentes con temperatura ambiente, calentamiento por carga y conduccion entre piezas.
- `feat(thermal)`: Se consolido `ThermalViewController` para conectar temperaturas reales por pieza con el modo de visualizacion termica.
- `feat(thermal)`: Se mantuvo `ThermalSurfaceProfile` como capa de configuracion espacial por pieza critica.
- `feat(thermal)`: Se amplio `ThermalContactGraphAsset` con metadata de build, nodos y notas por enlace.
- `feat(editor)`: Se creo `ThermalContactGraphBuilderWindow` para generar un grafo termico offline desde la geometria de la escena usando bounds de `ExplodablePart`.
- `feat(data)`: `DronePartData` fue alineado con el contrato termico requerido por el solver (`operatingTempMin/Max`, `thermalHover`, `thermalPeak`, `thermalWarmupSeconds`, `thermalExposure`, `thermalConductionScale`, `isThermallyCritical`).
- `feat(drone-state)`: `DroneStateController` ahora expone `SystemLoadFactor`, eventos de carga y sincronizacion de `Idle/Flying` con la carga sostenida.
- `feat(ui)`: Se agrego un slider de carga sostenida en `InspectModeHandler` y `MainLayout.uxml` bajo el control de encendido.
- `feat(runtime)`: `SceneBootstrapper` ahora puede asegurar la presencia de `ThermalSimulationManager` y `ThermalViewController` sin tocar escenas serializadas.
- `docs(thermal)`: Se actualizo la documentacion viva del sistema termico, incluyendo la matriz documental, la referencia tecnica y el breakdown de portafolio.

### Estado Validado

- El sistema termico ya tiene una base de runtime consistente y un camino de preprocesado offline.
- El grafo de contactos puede generarse en editor, pero aun falta calibrarlo sobre la geometria final retopologizada del X500 V2.
- La simulacion no debe presentarse aun como FEA ni como modelo termodinamico cuantitativo validado.
- La verificacion automatica con WolframAlpha sigue condicionada a la configuracion de `WOLFRAM_APP_ID`.
## 2026-03-11 — Ajustes UX: Analyze persistente + Explode desacoplado

### Trabajo Realizado

- `fix(ui)`: Se eliminó el cierre automático de submenús Analyze al seleccionar piezas o al hacer click en el fondo 3D.
- `fix(explode)`: Se desacopló el estado de `Explode` del `AppState` global para que no se apague al cambiar entre Analyze y Studio.
- `fix(ui)`: Se restauró la X del info panel con un icono procedimental y se reajustó su tamaño/posición para alinearla con el sistema visual del top bar.
- `fix(ui)`: Se eliminaron cierres no permitidos del info panel, quitando el drag-to-dismiss y evitando que los accesos de info funcionen como toggle de cierre.
- `fix(input)`: Se endureció `InputManager.IsPointerOverUI()` para usar el panel activo, soportar touch y descartar picks no interactivos o invisibles.
- `style(ui)`: Se fijó la rotación base del icono de cierre en `45°` para que la `X` no se lea como `+`.
- `fix(camera)`: Se corrigió el paneo para usar los ejes locales de la cámara en lugar de una mezcla de ejes globales.
- `fix(ui)`: Se alineó el onboarding con los controles reales de cámara (`right-click` orbit, `middle-click` pan).
- `fix(shader)`: Se aplicó clipping global también en los passes `DepthOnly` y `DepthNormals` de `Blueprint.shader` para que `Cut` respete el postproceso.
- `fix(camera)`: Se separó la intención de `pan` y `pinch` en touch para evitar zoom espurio durante gestos de desplazamiento con dos dedos.
- `style(ui)`: Se rediseñó el contraste del modo claro usando superficies translúcidas oscurecidas, tipografía más legible y controles con mayor definición en `Studio Light` y fondos claros.
- `refactor(ui)`: `ProceduralIconBase` pasó a soportar override de paleta por icono y `ProceduralCloseIcon` quedó fijado a una paleta clara para mantener contraste dentro del sheet.
- `docs`: Se añadió un documento técnico con diagnóstico completo y plan de implementación por fases para los siguientes problemas de la app.

### Archivos Afectados

- `desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Content/ExplodedViewManager.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIModeController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/InputManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/OrbitCameraController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingController.cs`
- `desarrollo/unity_project/Assets/Shaders/Blueprint.shader`
- `desarrollo/unity_project/Assets/Scripts/UI/ProceduralIcons/ProceduralIconBase.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/ProceduralIcons/ProceduralCloseIcon.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `desarrollo/unity_project/Assets/UI/Styles/Theme.uss`
- `desarrollo/docs/investigacion/14_analisis_problemas_app_2026-03-10.md`

### Resultado Validado

- `Cut`, `Filter` y `Explode` ya no se cierran al clickear el background.
- La vista explosionada permanece activa al abrir otros submenús de Analyze y al alternar entre Analyze y Studio.
- El info panel vuelve a mostrar un botón de cierre visible, consistente con el sistema de iconos procedurales y mejor alineado en la esquina superior derecha.
- El info panel ya no se cierra arrastrando el handle ni al volver a pulsar los accesos de info; su cierre queda acotado a la `X`, doble click en el fondo 3D y cambio de modo.
- La `X` recupera una lectura visual correcta en reposo y la detección de UI queda mejor preparada para bloquear input 3D sólo sobre controles interactivos reales.
- El paneo vuelve a seguir el plano local de la cámara y el onboarding ya no instruye un gesto de pan incorrecto.
- El modo `Blueprint` ahora entrega profundidad y normales ya recortadas al postproceso, evitando que el contorno reconstruya partes fuera del plano de corte.
- En touch, los gestos de dos dedos ya no aplican pan y zoom a la vez salvo ruido residual mínimo, priorizando la intención dominante del usuario.
- En fondos claros, el panel inferior y la pill de navegación dejan de verse como bloques blancos lavados y pasan a funcionar como superficies tonales oscuras, legibles y visualmente integradas con el environment.
- La desactivación de `Explode` queda restringida a las dos acciones esperadas por UX:
  - volver a pulsar el botón `Explode`
  - llevar el slider a `0`

---

## Fase 0 — Investigación y Selección Técnica _(Pre-Git)_

> Trabajo documentado en sesiones de Antigravity (conversación `c5f61e42`)

- Investigación de tendencias de portafolio Tech Artist (Hard Surface / Drones)
- Selección de modelo objetivo: Drone Sci-Fi de alto rendimiento
- Definición de features técnicos: Vista Explosionada, Rayos X, Delineado Técnico
- Definición de estilo UX/UI: Glassmorphism Premium
- Estudio de UI/UX (Moodboard + Referencias Técnicas)
- Diagramación y Wireframing (Grid System 12 columnas, wireframes de flujo, atomización UI)

---

## Fase 1 — Planificación y Configuración _(Pre-Git)_

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
- `DetailsPanelController` (conexión SelectionManager ? UI)

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

| Hash      | Mensaje                                                                    |
| --------- | -------------------------------------------------------------------------- |
| `6d8ddf9` | feat: Complete implementation of WebGL drone visualization prototype       |
| `c935277` | feat: Add scene setup utilities and AssemblyGuideData                      |
| `b7a0e80` | docs: Add README, CONTRIBUTING, setup guide and improve code documentation |
| `1c19ecc` | feat: Add complete project assets and documentation                        |
| `99f38a5` | docs: Add comprehensive presentation design system for Kimi K2             |
| `ad44d00` | docs: Consolidate presentation docs into single folder                     |

---

## 2025-12-05 — Landing Page y GitHub Pages

### Trabajo Realizado

- **Landing Page Premium**: HTML + CSS + GSAP scrollytelling
- **Doc Viewer**: Visor de documentación integrado con soporte Mermaid
- **GitHub Pages**: Configuración y deploy (`.nojekyll`, fix 404)
- **Mobile**: Swipe navigation, zoom Mermaid lightbox
- **Múltiples iteraciones de fix**: CSS specificity, layout duplicates, JS robustez

### Git Commits (2025-12-05)

| Hash      | Mensaje                                                            |
| --------- | ------------------------------------------------------------------ |
| `2e5a79c` | feat: Add complete project support materials                       |
| `17152fe` | feat: Premium landing page + fix cronograma dates                  |
| `9f92050` | feat: Awwwards-style landing page with GSAP scrollytelling         |
| `f2943c7` | fix: Inline CSS/JS in landing page for better portability          |
| `4fca23f` | fix: Add ScrollToPlugin and link local docs                        |
| `2e0f34b` | fix: Add missing Unity project configuration files                 |
| `a8ce0f0` | fix: Optimize cursor performance and fix features/docs visibility  |
| `71f6b90` | feat: Redesign landing page - Minimalist & Seamless Transition     |
| `720bfee` | feat: Add scroll pinning effect and restore documentation links    |
| `2af00e0` | fix: Increase mobile margins and padding                           |
| `072402d` | fix: Significantly increase mobile margins to 3rem                 |
| `309d7e2` | fix: Resolve CSS specificity issue to ensure mobile margins        |
| `2fb1a4f` | feat: Implement Integrated Doc Viewer with Mermaid Support         |
| `f34af03` | feat: Enhanced Doc Viewer with Nav, Swipe, and Path Fixes          |
| `54eb9a5` | fix: Remove duplicate section and refine layout                    |
| `6b1a651` | fix: Refactor HTML layout to Section > Container                   |
| `5b99de8` | fix: Restore Doc section and restrict grid width                   |
| `ad512e0` | fix: Repair malformed HTML in Features section                     |
| `78e04a7` | fix: Restore advanced Doc Viewer JS logic                          |
| `20a1cc8` | fix: Robust JS logic for DocViewer                                 |
| `c11ee5b` | feat(docs): refine doc viewer, fix layout duplicates, add PDF PDFs |
| `dc0cf8f` | chore: add .nojekyll to fix github pages 404                       |
| `219524c` | chore: remove interfering unity-build workflow to unblock pages    |
| `4fe2e1f` | feat(mobile): add swipe nav, hide arrows, mermaid lightbox         |

---

## 2025-12-08 — Migración React + Framer Motion

### Trabajo Realizado

> Documentado en `MICROINTERACTIONS_PLAN.md`

- Setup React + Vite + Framer Motion
- Arquitectura de componentes para micro-interacciones temáticas por card
- 7 animaciones únicas diseñadas (X-Ray scan, explosion, blueprint, render, shader, compile, color grading)
- Fallback: mantenimiento de versión HTML estática

### Git Commits (2025-12-08)

| Hash      | Mensaje                                                                      |
| --------- | ---------------------------------------------------------------------------- |
| `0c80d85` | feat: setup React + Framer Motion, fix visibility bug, remove card numbering |
| `85d0845` | fix: temporarily use static HTML deployment for GitHub Pages                 |
| `70df5cc` | feat: complete React architecture with component structure                   |

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

| Hash      | Mensaje                                                                |
| --------- | ---------------------------------------------------------------------- |
| `5669c25` | chore: configure Git LFS for binary assets (models, textures, audio)   |
| `06fb92a` | Fix compilation errors: Updated DronePartData, fixed UI Toolkit syntax |
| `479fb69` | Resolved USS warnings and updated task list                            |

---

## 2026-02-10 — UI Profesional + Shaders Integration

### Trabajo Realizado

- **Step 1-2**: Reset button, Info Sheet, Visual Viewport Shift
- **Shader Integration**: Todos los shaders (X-Ray, Blueprint, Thermal, etc.) conectados a UI con cycling
- **High-DPI Mobile**: Redimensionamiento completo (botones 100px, fuentes 24px+)
- **Iteraciones de posicionamiento**: Múltiples ajustes de UI (530px ? 150px vertical)

### Git Commits (2026-02-10)

| Hash      | Mensaje                                                                                     |
| --------- | ------------------------------------------------------------------------------------------- |
| `d38744f` | feat: Implement Professional UI (Step 1 & 2) - Reset, Info Sheet, Visual Viewport Shift     |
| `47857c5` | Refactor: Renamed UI categories (Electronics/Energy) and fixed UIManager compilation errors |
| `6d0952d` | Feat: Integrated all shaders (XRay, Blueprint, Thermal, etc.) with UI cycling               |
| `26b22c3` | Style: Resized UI for High-DPI Mobile (1320x2860), buttons 100px, fonts 24px+               |
| `12de251` | Style: Vertically spread UI elements for High-DPI mobile                                    |
| `6277396` | Style: Lowered UI groups by 40px to tighten layout                                          |
| `67a93d9` | Style: Aggressively lowered UI groups                                                       |

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

| Hash      | Mensaje                                                             |
| --------- | ------------------------------------------------------------------- |
| `c86f3cc` | Style: Lowered UI to limit (Menu @ 180px, Slider @ 300px)           |
| `5eab07e` | Style: Lowered UI again by 30px                                     |
| `4dc6ce6` | Polish: Selection Label fades out when Details Sheet opens          |
| `32d49b5` | Revert "Polish: Selection Label fades out when Details Sheet opens" |
| `dead62e` | Polish: Selection Label fades out (Safe Opacity Implementation)     |
| `3cc5d6a` | Style: Floating Details Sheet (Bottom 260px)                        |
| `e7e6eac` | Polish: Synced BottomBar and DetailsSheet animation                 |
| `cdb3d1b` | Style: Reduced button shift by 60px                                 |
| `62a2866` | Style: Lowered Sheet assembly by 100px                              |
| `83c1820` | Fix: Restored Details Sheet height to 420px                         |
| `63e02ff` | Feat: Smart Hotspots (Screen-Space UI Toolkit)                      |
| `f6633de` | Fix: Remove duplicate CSS import                                    |
| `d8a606b` | Fix: Resolve CS0246 (Singleton namespace) and USS warnings          |
| `763eb59` | Fix: Resolve CS0246 (SelectionManager, DronePartData)               |
| `6383b06` | Feat: Add URP Asset Tool & Polish UI Spacing (8pt grid)             |
| `17ae084` | feat(ui): Rediseño completo Hotspots y MainTheme (2026 Minimalist)  |

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

| Hash      | Mensaje                                                                |
| --------- | ---------------------------------------------------------------------- |
| `d74f893` | fix(ui): Phase 1-2 bug fixes                                           |
| `acea950` | fix(ui): Bug 1 - only auto-open info sheet on hotspot click            |
| `99fb17e` | Fix UI Layout: Correct math positioning, dynamic stacking, slider init |
| `4c37da1` | Fix UI Interaction: Z-Order blocking (Bug 5) and Thermal Button Layout |
| `076acf6` | Implement Phase 3: Info Sheet Polish (Responsive Offset, Drag Dismiss) |
| `d8eefbf` | Fix Compilation Errors: Duplicate Constants & Unused Fields            |
| `8db5f93` | Implement Phase 5: Info Sheet Click Blocking & Text Selection          |
| `8e46c79` | Fix Syntax & API Warnings (Phase 6)                                    |
| `b26641a` | Fix CSS Warnings & Enforce C# Picking Logic (Phase 7)                  |
| `8c3406c` | Extend Text Selection to Full Info Sheet (Phase 8)                     |
| `4d618d2` | Block 3D Selection on UI Interaction (Phase 9)                         |
| `9007d13` | Ensure WebGL Build is tracked                                          |

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

- Accent color: Cyan (`#00D9FF`) ? Blanco (`#FFFFFF`)
- 9 selectores actualizados (`.btn-tag--active`, `.shader-mode-btn--active`, `.env-preset-btn--active`, slider dragger, tag-dot-accent, etc.)
- Background: `#050505` (near-black, matches web)
- Typography: Inter global, Space Grotesk Regular titles
- Button styling: transparent + pill

**Bug Fixes — Phase 1:**

1. Device selector width: `320px ? 400px` (clipping fix)
2. Canvas background: `EnvironmentController.ApplyPreset("Studio")` bg fixed to `#050505`
3. Info panel text wrapping: `white-space: normal` en inner TextField + `align-items: flex-start`
4. Close button: `StopPropagation()` to prevent header toggle re-opening
5. Buttons position: `.ui-shifted 56% ? 46%`
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

| Hash      | Mensaje                                                                                    |
| --------- | ------------------------------------------------------------------------------------------ |
| `6b3b3ad` | fix: hero menu hitbox + redesign (4 buttons, device selector, home btn)                    |
| `140185e` | style: visual redesign to match web landing page                                           |
| `42c6e90` | Fix UI bugs: About close, click blocking, info button, bg black, fonts                     |
| `51461c3` | Refactor UX: Unified Bottom Sheet (Delete AboutPanel, DeviceSelector)                      |
| `0229672` | feat(ui): implement Hero Submenus, Space Grotesk font, black bg fix                        |
| `93c4562` | fix(ui): resolve uxml syntax error and remove unused code                                  |
| `332a9b6` | fix(ui): apply Space Grotesk font globally                                                 |
| `e3d72e5` | fix(ui): update hero title to Title Case and refine spacing                                |
| `b03d286` | fix(ui): match web typography (Inter global, Space Grotesk Regular titles)                 |
| `fa62502` | fix(ui): match web button styling (transparent + pill)                                     |
| `ef2ec40` | feat(ui): match web styling (pills, spacing, no scrollbars)                                |
| `dc4d83b` | docs: update master plan v2.1 with user feedback                                           |
| `16dc25f` | fix(ui): Phase 1+2 - white accent, ghost clicks, slider, device width, bg                  |
| `1a95185` | docs: Phase 4 - Create Desarrollo_App folder with CHANGELOG, DESIGN_TOKENS                 |
| `0d6754a` | fix(ui): remove invalid USS syntax (~combinator, overflow:visible)                         |
| `cfd7ee1` | fix(ui): studio bg #050505, slider centered, text wrap, close btn fix                      |
| `55c26cf` | docs: comprehensive CHANGELOG + TECHNOLOGY_STACK (all tools, AI, MCPs, skills, RAG)        |
| `f25d557` | fix(ui): data-value text wrapping (flex:1 + white-space) + slider drag-container centering |
| `6ff1ba2` | fix(uss): remove all picking-mode from USS (not valid USS property)                        |
| `1399a4f` | fix(ui): text clip, slider horizontal fix, device cyan?white, submenu padding              |
| `ab598c5` | fix(ui): TextFieldLabel text clip, slider flex-start, device scroll overflow               |
| `e257066` | feat(visual): animated gradient skybox for Studio preset                                   |
| `cd33225` | feat(visual): update skybox to radial gradient with pulse animation                        |
| `6ecbd7c` | docs: add BITACORA.md formal project log                                                   |
| `5514ad1` | fix(ui): resolve device menu clipping + docs: align Bitácora to Cronograma                 |
| `3325d4b` | style(ui): force device menu width to 396.5px (scrollbar fix)                              |
| `b0f9ef5` | docs: enhance BITACORA.md with detailed technical rationale                                |

---

## 2026-02-19 — Phase 3: Grid Submenus + Icon System

### Trabajo Realizado

**Grid Navigation System (Phase 3.1–3.3):**

- Migración de Pins al CategoryMenu; Home/Reset restaurados al TopBar
- Implementación de sistema de iconos placeholder para botones
- Grid de 4 columnas para submenús (shader, category, environment)
- UIManager actualizado para soportar clases de grid submenu

**Polish & Bug Fixes:**

- Ajustes agresivos de layout para card rows
- Fix de SelectionManager background click y altura del layout
- Polish final de UI Layout, Spacing e interacciones

### Git Commits (2026-02-19)

| Hash      | Mensaje                                                                       |
| --------- | ----------------------------------------------------------------------------- |
| `472eb99` | feat(ui): move Pins to CategoryMenu, restore Home/Reset to TopBar (Phase 3.1) |
| `d2a60fa` | feat(ui): implement icon system for buttons (Phase 3.2)                       |
| `1f00e51` | feat(ui): implement 4-column grid system for submenus (Phase 3.3)             |
| `46f03d8` | feat(ui): implemented grid submenus for shader, category, and env panels      |
| `cf49b8b` | fix(ui): update UIManager to support new grid submenu classes                 |
| `65b05b2` | Fix: Aggressive layout adjustments for 4-card rows                            |
| `3bf55a4` | Fix: SelectionManager bg click and Layout height (Phase 8)                    |
| `1ee65f1` | Final Polish: UI Layout, Spacing, and Interaction Fixes Complete              |

---

## 2026-02-20 — Architecture Refactoring: Memory Leaks + God Class

### Trabajo Realizado

> Plan documentado en `implementation_plan.md` y `ARCHITECTURE_AUDIT_REPORT.md`

**Step 1: UI Toolkit Memory Leaks Resolvidos:**

- Implementación de patrón `AddCleanup` con cola de `System.Action` en `UIManager.cs`.
- Conversión de +50 lambdas anónimos a instancias cacheadas.
- Llamada obligatoria a `UnsubscribeFromUIEvents()` dentro de `OnDisable()`.
- Eliminación total del vector de fuga de memoria en listeners.

**Step 2: Desacoplamiento de UIManager (God Class):**

- Carpeta y namespace `WebGL.UI.Panels` para lógica local de interfaz.
- Extracción de `UIEnvironmentPanel.cs` y `UIAnalyzePanel.cs`.
- Reducción de más de 120 líneas en `UIManager`.

**Fix: Hotspot Binding:**

- Restauración de `hotspotBtn` initialization binding roto durante el refactor.

### Git Commits (2026-02-20)

| Hash      | Mensaje                                                                                         |
| --------- | ----------------------------------------------------------------------------------------------- |
| `a1a60ae` | refactor: implement comprehensive UI Toolkit event cleanup in UIManager to prevent memory leaks |
| `8a949ce` | refactor: dismantle UIManager God Class by extracting UIEnvironmentPanel and UIAnalyzePanel     |
| `487072a` | fix(ui): restore missing hotspotBtn initialization binding that broke the pins toggle           |

---

## 2026-02-23 — FASE 1: Arquitectura Limpia + Iteraciones UX 1–6

### Trabajo Realizado

> Plan documentado en `PHASE3_CHANGELOG.md` y `UX_UI_AUDIT_REPORT.md`

**Architecture Refactoring (C01–C05):**

- Refactoring completo de arquitectura: memory leaks, god class dismantling, input decoupling
- Centralización de input blocking en `InputManager` con UI-awareness
- Cleanup de dead code: `ApplyDefaultRenderSettings`, `GlobalInputBlocked`, `GameManager` state duplication
- Estandarización de null-safety patterns
- Revert de `RegisterButtonInputBlockers` (su eliminación rompió submenús)

**UX Redesign — Iteraciones 1–6 (Sistema de 3 Modos):**

- Expansión de `AppStateMachine` con modos Analyze y Studio
- Reestructuración de `MainLayout.uxml` con sistema de 3 modos
- Creación de `UIModeController` + rewire de `UIManager`
- Panel de Cross-Section en modo Analyze
- Integración final: eliminación de `UIPopupController`, limpieza de CSS
- Restauración de funciones vía Action Bars (Tools/Analyze/Studio)

**Polish:**

- Aesthetic pass: compact UI, glassmorphic panels, tighter spacing
- Fix de info panel, CUT tool, slider persistence, category menu auto-open
- Fix de accesibilidad (`CS0051 ActivateMode`)

### Git Commits (2026-02-23)

| Hash      | Mensaje                                                                                                     |
| --------- | ----------------------------------------------------------------------------------------------------------- |
| `04df7a1` | refactor(phase3): complete architecture refactoring — memory leaks, god class dismantling, input decoupling |
| `9e24ed5` | refactor(phase4): centralize input blocking in InputManager, add UI-awareness to camera and keyboard        |
| `b40e9aa` | docs: update changelog with Phase 4 documentation                                                           |
| `1607733` | refactor(phase5): cleanup dead code — remove ApplyDefaultRenderSettings, GlobalInputBlocked bridge          |
| `4b80bd5` | refactor: remove InputBlocked + RegisterButtonInputBlockers — sole UI guard is now IsPointerOverUI()        |
| `acea37a` | fix: remove duplicate closing brace in SelectionManager.HandleHover()                                       |
| `e7f79d8` | fix: restore StopPropagation on all buttons — fixes broken submenu clicks                                   |
| `b89e2f7` | revert: restore RegisterButtonInputBlockers — removal broke all submenu clicks                              |
| `e86ff95` | refactor: standardize null-safety patterns (Task 3)                                                         |
| `7515117` | docs: update PHASE3_CHANGELOG with Phase 5, Audit Tasks 1–3, and commit history                             |
| `22dd3af` | docs: add Phase 2 UX Redesign plan and changelog scaffold                                                   |
| `2673e5a` | feat(phase2-iter1): expand AppStateMachine with Analyze and Studio modes                                    |
| `70b2a01` | feat(phase2-iter2): restructure MainLayout.uxml with 3-mode system + USS styles                             |
| `f125f6f` | feat(phase2-iter3+4): create UIModeController + rewire UIManager for 3-mode system                          |
| `66673f3` | feat(phase2-iter5): add Cross-Section UI panel in Analyze mode                                              |
| `50dc291` | feat(phase2-iter6): cleanup & integration final — delete UIPopupController, deprecate toolbars              |
| `9f737a0` | fix(ui): restore all functions via Tools/Analyze/Studio action bars                                         |
| `ef7c6a4` | fix: CS0051 ActivateMode accessibility + escape & in UXML tooltip                                           |
| `4b5e08a` | fix: resolve 4 functional issues — info panel, CUT tool, slider, category menu                              |
| `d54e541` | fix: CUT cross-section, slider persistence, category menu auto-open                                         |
| `463df98` | style: aesthetic pass — compact UI, glassmorphic panels, tighter spacing                                    |
| `40686f5` | chore: clean up duplicate comments in Theme.uss                                                             |
| `8421b0b` | docs(ux): add UX/UI audit report and plan Iteration 7                                                       |

---

## 2026-02-24 — FASE 1: Iteraciones UX 7–11 (Grid Minimalista)

### Trabajo Realizado

- **Iter 7**: Implementación de grid UI minimalista: icon-only buttons, 4-col card grid, sliders alineados, containers invisibles
- **Iter 8**: Clipping universal de cross-section + fix slider + axis btn sizing
- **Iter 9**: Fix z-order bottom bar + overhaul de tamaños + cross-section blue
- **Iter 10**: Navegación card-grid unificada, dual-plane cross-section, UX overhaul
- **Iter 11**: Fix 7 quejas de UX — sizes, layout, gestures, filter

### Git Commits (2026-02-24)

| Hash      | Mensaje                                                                           |
| --------- | --------------------------------------------------------------------------------- |
| `b96132d` | style(ux): implement Iteration 7 — minimalist grid UI system                      |
| `59f80d9` | iter8: universal cross-section clipping + slider fix + axis btn sizing            |
| `7396b96` | iter9: fix bottom bar z-order + sizes overhaul + cross-section blue               |
| `e483d70` | Iteration 10: Unified card-grid navigation, dual-plane cross-section, UX overhaul |
| `800f4d0` | iter11: fix 7 UX complaints — sizes, layout, gestures, filter                     |

---

## 2026-02-25 — FASE 2: Iteraciones 12–14 (Modos Inspect/Analyze/Studio)

### Trabajo Realizado

- **Iter 12**: Fix cross-section en Realistic, floating (i) info btn, 3-col grids, hide shaders/env
- **Iter 13**: Major mode restructure ? Inspect/Analyze/Studio + isolate + larger cards + true pill
- **Iter 14**: Fix pill bar style, 3-card fit, isolate SetActive, remove FloatingInfoBtn, device scroll
- **Iter 14-fix**: Actions-row `border-radius: 36px` para pill shape real

### Git Commits (2026-02-25)

| Hash      | Mensaje                                                                                          |
| --------- | ------------------------------------------------------------------------------------------------ |
| `079ba8c` | iter12: fix cross-section in Realistic, floating (i) info btn, 3-col grids, hide shaders/env     |
| `beb8e36` | iter13: Major mode restructure — Inspect/Analyze/Studio + isolate + larger cards + true pill     |
| `345df4d` | iter14: fix pill bar style, 3-card fit, isolate SetActive, remove FloatingInfoBtn, device scroll |
| `d5fda52` | iter14-fix: actions-row border-radius 36px (half of 72px height) for true pill shape             |

---

## 2026-02-26 — FASE 2: Auditoría WCAG + Iconos Procedurales

### Trabajo Realizado

**Auditoría de Accesibilidad (WCAG 2.1 AA):**

- Fix de contraste, touch targets, font sizes, grid, dead CSS, border-radius
- Bottom pill más grande (112px, iconos de 56px) + re-auditoría completa
- Fix de device-option hover scale (prevención de clipping)

**Sistema de Iconos Procedurales:**

- `ProceduralCutIcon`: Microinteracción de slice diagonal con SpringFloat recovery
- Diseño completo de 8 iconos procedurales: Info, Filter, Studio, Pins, Inspect, Analyze, Explode, Home
- Integración en `MainLayout.uxml` con routing de PointerEvents al Button padre
- Fix de CSS background-images y TrickleDown phase para pointer events
- Documentación matemática y física de la arquitectura de iconos

**Documentación Técnica:**

- Corrección de errores factuales en 4 reportes de auditoría + plan de remediación
- Documentación comprensiva técnica para tesis (`01_Arquitectura`, `02_Manual_Tecnico`, etc.)

### Git Commits (2026-02-26)

| Hash      | Mensaje                                                                                         |
| --------- | ----------------------------------------------------------------------------------------------- |
| `e9a73de` | audit: fix WCAG contrast, touch targets, font sizes, grid, dead CSS, border-radius              |
| `70fed05` | fix: restore actions-row height to 72px (mode-btn 80px overflow)                                |
| `a6841b9` | feat: bigger bottom pill (112px, 56px icons) + full re-audit pass                               |
| `1efb9fc` | fix: remove device-option hover scale to prevent clipping                                       |
| `e027474` | Refactor ProceduralCutIcon: One-click precise diagonal slice microinteraction                   |
| `b15d9ee` | Refactor ProceduralCutIcon: Replace linear return sweep with SpringFloat recovery               |
| `eeb7bb3` | audit: correct factual errors across all 4 audit reports + add remediation plan                 |
| `8bd87aa` | UI: Overhauled procedural icons — Info, Filter, Studio, Pins, Inspect, Analyze                  |
| `4da6e37` | Fix compiler error in ProceduralPinsIcon                                                        |
| `e5781ab` | UI: Add procedural icons to IconGallery test scene UXML                                         |
| `74d3f21` | UI: Complete redesign of Inspect, Pins, Analyze, Studio icons + fixes for Explode, Info, Filter |
| `33cfb21` | UI: Refine Home, Info, Pins, Analyze and completely redesign InspectIcon                        |
| `3f35dc3` | Docs: Comprehensive mathematical & physical architecture documentation for Procedural UI Icons  |
| `59e027c` | UI: Integrate procedural icons into MainLayout.uxml and route PointerEvents to Button           |
| `a35302f` | UI Fix: Safely remove CSS background-images preventing syntax errors                            |
| `fd098b3` | UI Fix: Use TrickleDown phase in ProceduralIconBase for PointerEvents                           |
| `eb5e918` | docs: add comprehensive technical documentation for thesis                                      |
| `dbf7616` | UI Fix: Inverse Pins button initial Active State, slow down procedural physics ×2               |

---

## 2026-02-27 — FASE 2: Slider UX + Font Rendering + WebGL Config

### Trabajo Realizado

**Interacciones UI:**

- Delayed scheduling para button click actions (apreciar animaciones)
- Mutual exclusion entre Info Sheet y Card Menus
- Inversión de estados normal/hover para ProceduralExplodeIcon

**Font Rendering (5 iteraciones):**

- Asignación de PanelSettings + auto-generación de SDF Font Assets ? fracaso
- Revert a legacy font rendering (`-unity-font-definition: initial`)
- Corrección de USS warnings y WebGLBuildFixer

**Slider Hitbox Debugging (11 iteraciones):**

- Rediseño de Slider Draggers: hollow rings ? solid hover ? blue active
- Investigación de Yoga layout vs CSS en UI Toolkit
- Root cause: `position:relative` en UI Toolkit no mueve hit-test rect
- Múltiples intentos con `position:absolute`, scale, physical dimensions
- Creación y eliminación de `SliderHitboxDebugger.cs` (namespace collision)
- Revert final a styles de `d341e58`
- Fix de `picking-mode=Position` en slider labels

**WebGL Optimization:**

- Configuración de WebGL optimization + conversión de colores Gamma?Linear en USS

### Git Commits (2026-02-27)

| Hash      | Mensaje                                                                                   |
| --------- | ----------------------------------------------------------------------------------------- |
| `e54c006` | UI Fix: Delayed scheduling for button clicks, scale header buttons to 64px                |
| `0b905d3` | UI Fix: Card menus instantly hide when Info Sheet is toggled active                       |
| `76cebcf` | fix: assign PanelSettings to UIDocument + auto-generate SDF Font Assets                   |
| `b58cd92` | fix: remove -unity-font-definition:initial that blocks SDF font preview                   |
| `52e55f0` | fix: add -unity-font-definition with SDF font asset URLs                                  |
| `df28a7a` | fix(fonts): revert to legacy font rendering, remove SDF pipeline                          |
| `de8b660` | fix: resolve USS warnings, refactor WebGLBuildFixer, mitigate layout errors               |
| `6e89d72` | UI Fix: Invert normal and hover states for ProceduralExplodeIcon                          |
| `9b188f2` | UI Fix: Redesign Slider Draggers — hollow 24px rings, solid white 32px hover, blue active |
| `b615611` | UI Toolkit & Manager Bugfixes: font-definition initial, USS tint vars                     |
| `2f7358b` | chore: Record manual user modifications to UI styles and UIManager lifecycle              |
| `423cc74` | UI Fix: Scale 0.75 on large 32px hitbox for Slider Dragger                                |
| `8a6bf1c` | UI Fix: Physical width/height instead of scale for Slider Dragger hitbox                  |
| `d341e58` | feat: WebGL optimization config + Gamma-to-Linear USS color conversion                    |
| `b5f5443` | fix: slider UX overhaul + prevent container close on miss-click                           |
| `6bf60e3` | fix: slider dragger hitbox centering + blue click state                                   |
| `a04f097` | fix: slider hitbox centering + active-only blue + container transparency                  |
| `0de1faa` | fix: remove translate:-50% that caused slider hitbox offset                               |
| `80ac231` | fix(slider): add position:absolute to dragger — fixes hitbox misalignment                 |
| `2344495` | fix(slider): use Yoga flex centering instead of top/margin-top offsets                    |
| `664c41a` | fix: rename SliderHitboxDebugger namespace to avoid WebGL.Debug collision                 |
| `36fc49f` | revert: restore slider USS styles to d341e58 original state                               |
| `cb9b737` | fix: set picking-mode=Position on slider labels and Studio env-slider-groups              |

---

## 2026-03-02 — FASE 3: Implementación Completa (C01–C05 + H01–H11 + F3-00 a F3-13)

### Trabajo Realizado

> Jornada más productiva del proyecto: **48 commits** en un solo día.

**UI Optimization (Pre-Fase):**

- Prevención de event bubbling en sub-panels
- Reemplazo de fondos semi-transparentes por outlines transparentes (WebGL perf)
- Modo Diagonal Cut en herramienta de Cross-Section
- Eliminación de fondos translúcidos en topbar buttons

**FASE 1 — Code Quality (C01–C05):**

- **C01**: Eliminación de sistema duplicado `VisualMode` de `ExplodedViewManager`
- **C02**: Consolidación de triple publicación de eventos ? canal único `StateChangedEvent`
- **C03**: Agrupación de campos del Bottom Sheet en 3 secciones Foldout colapsables
- **C04**: Enforcement de touch targets mínimos de 44px (WCAG/HIG compliance)
- **C05**: Extracción de Loading/Error UI de C# procedural a UXML+USS declarativo

**Design Sheet:**

- Jerarquía visual editorial, altura fija, dividers limpios
- Panel más alto, pill centrado, foldouts colapsados

**FASE 2 — Architecture Hardening (H01–H11):**

- **H01**: `ServiceLocator` + migración de 4 singletons
- **H02**: Extracción de `BaseModeHandler` desde `UIModeController` (InspectHandler, AnalyzeHandler, StudioHandler)
- **H03**: `EventBus` con leak detection + zombie purge automático
- **H04**: Eliminación de 10 archivos de código muerto (ViewModeToolbar, EngineerToolbar, ScreenshotManager, etc.)
- **H06+H07**: Optimización de ProjectSettings WebGL (StripUnusedMeshComponents, IL2CPP Master, mipStripping)
- **H08**: Eliminación de `Update()` innecesarios ? coroutines/InvokeRepeating
- **H09**: `QualityManager` resolución adaptativa vía URP renderScale (reflection-based)
- **H10**: Fitts' Law — reducción de gap en pill inferior
- **H11**: Onboarding help overlay con botón HELP en hero menu

**FASE 3 — Feature Tickets (F3-00 a F3-13):**

- **F3-00**: Hotspot swap fix + slider dragger restyle + disable bg-click deselect
- **F3-01**: Studio render mode — toggle behavior + card visibility
- **F3-02**: Environment cycling — TIME + COLOR buttons
- **F3-03**: Filter — 6ª categoría PAYLOAD (completa grid 3×2)
- **F3-04**: Info ? FAB global (floating action button, visible on selection)
- **F3-05**: Inspect — add MEASURE card + ProceduralMeasureIcon
- **F3-06**: PERF-M02 — reducción de `webGLMaximumMemorySize` 512?256 MB
- **F3-07**: Grid 4pt + type ramp normalization
- **F3-08**: Explosion slider — dynamic percentage label
- **F3-09**: Escape key global — cascading dismiss
- **F3-10**: Camera magic numbers ? named constants
- **F3-11**: Double-click unificado — detección movida a `SelectionManager`
- **F3-12**: `DronePartData` limpieza — remove redundant property wrappers
- **F3-13**: WebGL template custom (basado en build existente)

**Bug Fixes:**

- FAB icon full-size, FAB rises con sheet, bg-click deselect, dbl-click exits isolate
- Explode slider inline below cards; toggle resets a 0; bg click keeps sheet
- QualityManager: reflection en lugar de dependencia directa a URP
- ExplodedViewManager: race condition en `Start()` (partes en 0,0,0)
- Ajuste de translate `.ui-shifted` tras cambios de H10

### Git Commits (2026-03-02)

| Hash      | Mensaje                                                                                           |
| --------- | ------------------------------------------------------------------------------------------------- |
| `df66b30` | fix: prevent sub-panel clicks from bubbling; add optimization manual doc                          |
| `85b48a0` | UI Opt: Replace submenu backgrounds with transparent outlines                                     |
| `84883ab` | Feature: Added Diagonal Cut mode to Cross Section tool                                            |
| `71ecf79` | UI Opt: Fix remaining slider containers background to outline styles                              |
| `0d4fd64` | Feature: Clean UI styles — remove slider inner borders, neutralize tool button backgrounds        |
| `de759c6` | UI Opt: Remove translucent background from global topbar buttons                                  |
| `12baf4b` | fix(C01): remove duplicate VisualMode system from ExplodedViewManager                             |
| `47a1600` | fix(C02): consolidate triple event publishing to single StateChangedEvent channel                 |
| `9603d46` | fix(C03): group Bottom Sheet fields into 3 collapsible Foldout sections                           |
| `a5d81a0` | fix(C04): enforce 44px minimum touch targets (WCAG/HIG compliance)                                |
| `b2f6be2` | refactor(C05): extract Loading/Error UI from procedural C# to UXML+USS                            |
| `19d040e` | design(sheet): editorial visual hierarchy, fixed height, clean dividers                           |
| `ec329ea` | design(sheet): taller panel, centered pill, collapsed foldouts                                    |
| `19919e9` | fix(sheet): responsive pill centering + guard inactive coroutine                                  |
| `8f6b11e` | docs(plan): add 30 missing findings from cross-audit analysis                                     |
| `5944d01` | refactor(arch): add ServiceLocator + migrate 4 singletons (H01)                                   |
| `1438817` | refactor(arch): extract mode handlers from UIModeController (H02)                                 |
| `0af9ac6` | feat(arch): add EventBus leak detection + zombie purge (H03)                                      |
| `6fb1bc1` | H04: Eliminar 10 archivos de código muerto                                                        |
| `7ed48d5` | H06+H07: Optimizar ProjectSettings WebGL — StripUnusedMeshComponents, IL2CPP Master               |
| `98842da` | H08: Eliminar Update() innecesarios — CrossSectionManager, LoadingController, ExplodedViewManager |
| `6314c86` | H09: QualityManager resolución adaptativa vía URP renderScale (reflection-based)                  |
| `5efda91` | H10: Fitts' Law — reducir gap pill inferior                                                       |
| `55b66f3` | FIX: QualityManager — usar reflection en lugar de dependencia directa a URP                       |
| `62aec7c` | FIX: ExplodedViewManager — race condition en Start() causaba partes en (0,0,0)                    |
| `f3d119d` | FIX: Ajustar translate de .ui-shifted tras cambios de H10                                         |
| `671a0a8` | H11: Add first-visit onboarding help overlay with HELP button in hero menu                        |
| `5f32379` | UX: Group HELP into ABOUT submenu, hero typography hierarchy, type ramp                           |
| `f1cf034` | UX: Hero title SemiBold, about-desc lighter gray                                                  |
| `8b543e2` | F3-00: hotspot swap fix + slider dragger restyle + workplan                                       |
| `9134668` | F3-00b: disable background-click deselect + fix slider active blue                                |
| `433b83d` | F3-00c: fix hotspot highlight stacking + slider active blue                                       |
| `fb9453e` | F3-01: Studio render mode — toggle behavior + card visibility                                     |
| `6484975` | F3-02: Environment cycling — TIME + COLOR buttons                                                 |
| `663ebca` | F3-03: Filter — add 6th category PAYLOAD (completes 3×2 grid)                                     |
| `cd6a464` | F3-04: Info ? FAB global — extract ToolInfoBtn to floating action button                          |
| `3e91a3d` | fix: FAB icon full-size, FAB rises with sheet, bg-click deselect                                  |
| `83f0278` | refactor: explode slider inline below cards instead of sub-panel                                  |
| `9bb5bb3` | fix: explode toggle resets to 0, bg click keeps sheet, remove X close btn                         |
| `752d81b` | F3-05: Inspect — add MEASURE card + ProceduralMeasureIcon                                         |
| `da8908c` | F3-06: PERF-M02 — reduce webGLMaximumMemorySize 512?256                                           |
| `3945e18` | F3-07: Grid 4pt + type ramp normalization                                                         |
| `40a2d40` | F3-08: Explosion slider — dynamic percentage label                                                |
| `b1683ba` | F3-09: Escape key global — cascading dismiss                                                      |
| `a959c03` | F3-10: Camera magic numbers ? named constants                                                     |
| `b32b2b5` | F3-11: Double-click unificado — move detection to SelectionManager                                |
| `8a4808e` | F3-12: DronePartData limpieza — remove redundant property wrappers                                |
| `8ba3b50` | F3-13: WebGL template custom (basado en build existente)                                          |

---

## 2026-03-03 — Fixes Finales + Documentación

### Trabajo Realizado

- FAB info visible con Bottom Sheet abierto + explode slider reset a 0
- Explode slider restaura último valor (o 50% por defecto)
- Plan de trabajo final — auditoría completa + plan 4 días
- Checkpoint pre-refactor: Info Bar UI redesign

### Git Commits (2026-03-03)

| Hash      | Mensaje                                                            |
| --------- | ------------------------------------------------------------------ |
| `f5cd7c9` | fix: FAB info visible con sheet abierto + explode slider reset a 0 |
| `1dcf472` | fix: explode slider restaura último valor (o 50% por defecto)      |
| `d378d23` | docs: plan de trabajo final — auditoría completa + plan 4 días     |
| `78645a9` | chore: checkpoint before Info Bar UI refactor                      |

---

## Pendiente

- **Informe Final LaTeX**: Redacción APA 7 UNAD (Introducción, Marco Teórico, Resultados, Conclusiones)
- **Pruebas de usabilidad**: SUS (System Usability Scale), NASA-TLX
- **Video demostración**: Showreel final
- **Build WebGL final**: Deploy verificado
- **Exportar GLBs**: 11 partes individuales + drone completo

---

## Estadísticas

| Métrica           | Valor                                                                                                               |
| ----------------- | ------------------------------------------------------------------------------------------------------------------- |
| Total commits     | 234                                                                                                                 |
| Período           | Dec 4, 2025 – Mar 3, 2026 (90 días)                                                                                 |
| Días activos      | 16                                                                                                                  |
| C# Scripts        | 91 (~14,778 líneas)                                                                                                 |
| Custom Shaders    | 9 (X-Ray, Blueprint, Thermal, Wireframe, WireframeWebGL, SolidColor, Ghosted, ClippableLit, AnimatedGradientSkybox) |
| USS Stylesheets   | 5 (3,561 líneas)                                                                                                    |
| UXML Layouts      | 4 (502 líneas)                                                                                                      |
| ScriptableObjects | 11 (DronePartData)                                                                                                  |
| Escenas Unity     | 3                                                                                                                   |
