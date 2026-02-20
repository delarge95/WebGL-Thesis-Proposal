# Implementation Plan: UI/UX Audit Resolution

This plan outlines the steps required to resolve the violations found during the UI/UX audit, aligning the project with the `Guía Completa de Diseño UI_UX para Aplicaciones Móviles`.

## 1. Responsive Layout Refactor
The hardcoded `600px` width applied to the bottom pill and submenus is strictly against the "Mobile-First" (320px viewport) rule.

### Proposed CSS Changes (`Theme.uss`)
*   **`.actions-row` (Pill)**:
    *   Remove `min-width: 600px`.
    *   Add `width: 90%`, `max-width: 600px`.
    *   Modify `padding: 12px 16px;` (reduce horizontal padding to fit 5 icons on mobile).
*   **`.submenu-container` & `.slider-container`**:
    *   Remove `width: 600px`.
    *   Add `width: 90%`, `max-width: 600px`.
*   **`.submenu-grid`**:
    *   Remove `width: 600px`.
    *   Change structure to allow a responsive flex grid. Let the parent container dictate width.
*   **`.submenu-card` (The 4-column issue)**:
    *   Instead of fixed `130px` width which only works for 600px containers, use flexible math.
    *   Add `flex-grow: 1; flex-basis: 21%; max-width: 25%; margin: 2%;`. 
    *   This forces 4 cards per row across ANY screen size.

## 2. Touch Target Corrections
*   **`.sheet-close-btn`**:
    *   Current: `32px`
    *   Target: `44px` height and width.
    *   Adjust `border-radius: 22px;` to remain perfectly circular.

## 3. Typography Enhancements
*   **`.submenu-label`**:
    *   Current: `12px` (fails the 16px test for readability).
    *   Target: `14px` or `16px`. We will try `14px` combined with a slightly larger `.submenu-icon` (e.g., `40px`).

## 4. Spacing Rules (Regla Internal/External)
*   **`.submenu-card`**:
    *   Add `padding: 12px` (Internal padding).
    *   Maintain outer margin for separation.

---

## Phase 2: Senior UX Redesign (The 3-Mode System)
To align with the thesis proposal of reducing "cognitive friction" (carga cognitiva), the UI must evolve from a flat 5-button layout to a minimalist 3-mode hierarchy.

### Step 1: Restructure `MainLayout.uxml`
*   Replace the 5 buttons in `.actions-row` with 3 Mode toggles: `ExploreBtn`, `AnalyzeBtn`, `StudioBtn`.
*   Wrap the existing submenus (Shader, Category, Slider) into a master `AnalyzeMenu` container.

### Step 2: Update `UIManager.cs` Logic
*   Implement a state machine for the UI Modes (Explore, Analyze, Studio).
*   **Explore Mode**: Default. Click parts to open info sheet.
*   **Analyze Mode**: Shows the unified technical panel (combining Shaders, Explode Slider, and Category filters into one organized layout).
*   **Studio Mode**: Shows the Lighting/Environment panel.

### Step 3: Refine CSS (`Theme.uss`)
*   Reduce `.actions-row` `min-width` to a much smaller pill (e.g., `300px`) since it only holds 3 buttons.
*   Design a unified `AnalyzePanel` that elegantly fits the technical tools without overwhelming the screen.

### Step 4: Fix Heuristic Violations (Fitts's Law & Responsiveness)
*   **Target Size**: Increase `.sheet-close-btn` to `44px` width/height and `22px` border-radius to pass mobile accessibility standards.
*   **Liquid Layout**: Replace rigid widths (`600px`) in `.actions-row`, `.submenu-container`, and `.submenu-grid` with `width: 95%; max-width: x;` fluid sizing.
*   **Fluid Grid**: Change `.submenu-card` from static `130px` to `flex-basis: 22%` with percentage margins to ensure exactly 4 cards scale dynamically on small screens without breaking layout.

---

## Phase 3: Technical Architecture Refactoring

Based on the `ARCHITECTURE_AUDIT_REPORT.md`, the codebase requires severe structural improvements to ensure maintainability and eliminate memory leaks.

### Step 1: Resolve UI Toolkit Memory Leaks
*   **Target**: `UIManager.cs`
*   **Action**: Create a comprehensive `UnsubscribeFromUIEvents()` method that explicitly unregisters all `clicked`, `RegisterCallback`, and `RegisterValueChangedCallback` events triggered during `InitializeUI()`. Call this in `OnDisable()`.

### Step 2: Dismantle the `UIManager` God Class
*   **Target**: `UIManager.cs`
*   **Action**: Break the 1000+ line monolith into smaller, SRP-compliant single-purpose classes. 
    *   Separate Environment/Lighting logic into a `UIEnvironmentPanel` class.
    *   Separate Shader/Filters logic into a `UIAnalyzePanel` class.
    *   Keep `UIManager` explicitly as a high-level state coordinator.

### Step 3: Centralize Input Handling (Decoupling)
*   **Target**: `SelectionManager.cs` and `OrbitCameraController.cs`
*   **Action**: Remove direct `UnityEngine.Input` polling. Instead, establish a unified Input layer that broadcasts pure events ensuring UI blocks are respected cleanly without checking `OrbitCameraController.GlobalInputBlocked` directly.

---

## Phase 4: WebGL Optimization (Hitting KPIs)

Based on the `PERFORMANCE_AUDIT_REPORT.md`, these graphics and memory configurations are required to meet the `< 100k Polygons, > 30 FPS, < 3s Load` KPIs.

### Step 1: Prevent WebAssembly OOM Crashes
*   **Target**: `ProjectSettings/ProjectSettings.asset`
*   **Action**: Lower `webGLMaximumMemorySize` from `2048` MB to `512` MB to prevent Safari/Mobile Chrome crashes. Change `webGLInitialMemorySize` to `128` MB.

### Step 2: Texture Compression Pipeline
*   **Target**: WebGL Build Settings
*   **Action**: Force Mobile Texture Compression to **ASTC 6x6** globally to drastically shrink the build size below the 50MB KPI limits.

### Step 3: Implement Asynchronous WebGL Template
*   **Target**: `Assets/WebGLTemplates/`
*   **Action**: Replace the default Unity template with a custom HTML/CSS shell that loads instantly (<1s TTI) while downloading the `wasm` binary asynchronously in the background.

### Step 4: Disable Real-Time Shadows
*   **Target**: `ProjectSettings/QualitySettings.asset` (WebGL 'Low' Tier)
*   **Action**: Completely disable `shadowCascades`. The technical viewer should rely purely on Baked Ambient Occlusion and PBR IBL (Image-Based Lighting) to maintain >30 FPS on mid-range devices.

---

## Phase 5: Academic Validation (The Final Milestone)

Based on the `ACADEMIC_ALIGNMENT_REPORT.md`, empirical validation is required to fulfill the thesis promises before the final defense.

### Step 1: Deploy & Test MVP
*   **Action**: Deploy the fully optimized WebGL build (after Phase 4) to GitHub Pages.
*   **Action**: Create the standard SUS (System Usability Scale) and simplified NASA-TLX (Task Load Index) questionnaires in Google Forms.
*   **Action**: Recruit 8-12 engineers/technicians to participate in the empirical trial.

### Step 2: Extract Profiler Documentation
*   **Action**: Connect Unity Profiler to a mid-range mobile device running the WebGL build.
*   **Action**: Capture definitive proof of performance KPIs: Polycount (<100k), Frame Time (<33.33ms), and initial Memory Heap (<150MB).

### Step 3: Technical Art Pipeline Documentation
*   **Action**: Write the canonical `technical_art_pipeline.md` or directly format the LaTeX chapter detailing the retopology and normal map baking methodology used to achieve the 90% polygon reduction promised in the proposal.
