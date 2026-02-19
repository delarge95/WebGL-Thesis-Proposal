# CHANGELOG — WebGL Drone Viewer

Registro completo de todos los cambios realizados en el proyecto, extraído del historial de Git y complementado con contexto de las sesiones de desarrollo.

---

## 2026-02-18 — UI/UX Overhaul Phase 1+2

| Hash | Descripción |
|---|---|
| `16dc25f` | **fix(ui): Phase 1+2** — Accent cyan→blanco, ghost clicks, slider centrado, ancho device selector, info panel, fondo negro |
| `dc4d83b` | **docs**: Actualización plan maestro v2.1 (grid submenus, iconos, degradado) |
| `ef2ec40` | **feat(ui)**: Match web styling (botones pill, spacing, sin scrollbars) |
| `fa62502` | **fix(ui)**: Match web button styling (transparente + pill) |
| `b03d286` | **fix(ui)**: Match web typography (Inter global, Space Grotesk Regular títulos) |
| `e3d72e5` | **fix(ui)**: Update hero title a Title Case y refinar spacing |
| `332a9b6` | **fix(ui)**: Aplicar fuente Space Grotesk globalmente |
| `93c4562` | **fix(ui)**: Resolver error sintaxis UXML y remover código no usado |
| `0229672` | **feat(ui)**: Implementar Hero Submenus, fuente Space Grotesk, fix fondo negro |
| `51461c3` | **refactor**: UX unificado Bottom Sheet (eliminar AboutPanel, DeviceSelector) |
| `42c6e90` | **fix**: UI bugs — About close, click blocking, info button, bg black, fonts |
| `140185e` | **style**: Rediseño visual para coincidir con landing page web |
| `6b3b3ad` | **fix**: Hero menu hitbox + rediseño (4 botones, device selector, home btn) |

---

## 2026-02-12 — Bug Fixes & Polish

| Hash | Descripción |
|---|---|
| `9007d13` | **chore**: Asegurar que WebGL Build sea trackeado |
| `4d618d2` | **Block 3D Selection** en interacción UI (Phase 9) |
| `8c3406c` | **Extend Text Selection** a Full Info Sheet (Phase 8) |
| `b26641a` | **Fix CSS Warnings** & Enforce C# Picking Logic (Phase 7) |
| `8e46c79` | **Fix Syntax & API Warnings** (Phase 6) |
| `8db5f93` | **Implement Phase 5**: Info Sheet Click Blocking & Text Selection |
| `d8eefbf` | **Fix Compilation Errors**: Duplicate Constants & Unused Fields (Phase 4) |
| `076acf6` | **Implement Phase 3**: Info Sheet Polish (Responsive Offset, Drag Dismiss, Zoom Block) + Hotspot Toggle |
| `4c37da1` | **Fix UI Interaction**: Z-Order blocking + Thermal Button Layout |
| `99fb17e` | **Fix UI Layout**: Correct math positioning, dynamic stacking, slider init bug |
| `acea950` | **fix(ui)**: Bug 1 — solo auto-open info sheet en hotspot click |

---

## 2026-02-11 — UI Development Sprint

| Hash | Descripción |
|---|---|
| `d74f893` | **fix(ui)**: Phase 1-2 bug fixes — remove auto-open sheet, fix layout z-order |
| `17ae084` | **feat(ui)**: Rediseño completo Hotspots y MainTheme (2026 Minimalist Aesthetic) |
| `6383b06` | **feat**: Add URP Asset Tool & Polish UI Spacing (8pt grid) |
| `763eb59` | **fix**: Resolve CS0246 (SelectionManager, DronePartData) + CS0311 (SmartHotspot) |
| `d8a606b` | **fix**: Resolve CS0246 (Singleton namespace) + USS warnings (picking-mode) |
| `f6633de` | **fix**: Remove duplicate CSS import |
| `63e02ff` | **feat**: Smart Hotspots (Screen-Space UI Toolkit) |
| `83c1820` | **fix**: Restored Details Sheet height to 420px |
| `62a2866` | **style**: Lowered Sheet assembly by 100px |
| `cdb3d1b` | **style**: Reduced button shift by 60px |
| `e7e6eac` | **polish**: Synced BottomBar and DetailsSheet animation |
| `3cc5d6a` | **style**: Floating Details Sheet to clear buttons |
| `dead62e` | **polish**: Selection Label fades out |
| `32d49b5` | **revert**: Selection Label fade out |
| `4dc6ce6` | **polish**: Selection Label fades out when Details Sheet opens |
| `5eab07e` | **style**: Lowered UI again by 30px |
| `c86f3cc` | **style**: Lowered UI to limit |

---

## 2026-02-10 — Core UI Implementation

| Hash | Descripción |
|---|---|
| `67a93d9` | **style**: Aggressively lowered UI groups |
| `6277396` | **style**: Lowered UI groups by 40px |
| `12de251` | **style**: Vertically spread UI for High-DPI mobile |
| `26b22c3` | **style**: Resized UI for High-DPI Mobile (1320x2860) |
| `6d0952d` | **feat**: Integrated all shaders (XRay, Blueprint, Thermal, etc.) with UI |
| `47857c5` | **refactor**: Renamed UI categories + fixed UIManager compilation |
| `d38744f` | **feat**: Implement Professional UI (Reset, Info Sheet, Viewport Shift) |

---

## 2025-12-11 — Shader & Compilation Fixes

| Hash | Descripción |
|---|---|
| `479fb69` | **fix**: Resolved USS warnings + updated task list |
| `06fb92a` | **fix**: Compilation errors — DronePartData, UI Toolkit syntax, API changes |
| `5669c25` | **chore**: Configure Git LFS for binary assets |

---

## 2025-12-08 — Landing Page Development

| Hash | Descripción |
|---|---|
| `a74f164` | **feat**: Elaborate micro-interactions (particles, wireframes, glitch) |
| `d798de8` | **feat**: Professional micro-interactions for 7 cards |
| `70df5cc` | **feat**: Complete React architecture |
| `85d0845` | **fix**: Static HTML deployment for GitHub Pages |
| `0c80d85` | **feat**: Setup React + Framer Motion |

---

## 2025-12-05 — Landing Page & Documentation

| Hash | Descripción |
|---|---|
| `4fe2e1f` | **feat(mobile)**: Swipe nav, hide arrows, mermaid lightbox |
| `219524c` | **chore**: Remove interfering unity-build workflow |
| `dc0cf8f` | **chore**: Add .nojekyll for GitHub Pages |
| `c11ee5b` | **feat(docs)**: Doc viewer, fix layout duplicates, PDFs |
| `20a1cc8`–`6b1a651` | Series of fixes for Doc Viewer JS logic and HTML layout |
| `f34af03` | **feat**: Doc Viewer with Nav, Swipe, Path Fixes |
| `2fb1a4f` | **feat**: Integrated Doc Viewer with Mermaid Support |
| `309d7e2`–`2af00e0` | CSS specificity and mobile margin fixes |
| `720bfee` | **feat**: Scroll pinning effect and documentation links |
| `71f6b90` | **feat**: Redesign landing page — Minimalist & Seamless |
| `a8ce0f0` | **fix**: Cursor performance and features/docs visibility |
| `2e0f34b` | **fix**: Missing Unity project config files |
| `4fca23f`–`f2943c7` | ScrollToPlugin, inline CSS/JS |
| `9f92050` | **feat**: Awwwards-style landing page with GSAP scrollytelling |
| `17152fe` | **feat**: Premium landing page + fix cronograma |
| `2e5a79c` | **feat**: Complete project support materials |

---

## 2025-12-04 — Initial Implementation

| Hash | Descripción |
|---|---|
| `ad44d00` | **docs**: Consolidate presentation docs |
| `99f38a5` | **docs**: Comprehensive presentation design system |
| `1c19ecc` | **feat**: Complete project assets and documentation |
| `b7a0e80` | **docs**: README, CONTRIBUTING, setup guide, code docs |
| `c935277` | **feat**: Scene setup utilities and AssemblyGuideData |
| `6d8ddf9` | **feat**: Complete implementation of WebGL drone visualization prototype |

---

*Generado el 2026-02-18. Fuente: `git log` + contexto de sesiones de desarrollo con IA.*
