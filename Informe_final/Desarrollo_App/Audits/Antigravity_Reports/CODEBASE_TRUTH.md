# THE GROUND TRUTH: CODEBASE STATE

*Date:* 2026-03-02
*Purpose:* Factual mapping of `e:\WebGL_tesis\desarrollo\unity_project` to compare against documentation.
*Rule:* No assumptions. Only what exists in the code natively.

## 1. Project Structure
**Path:** `e:\WebGL_tesis\desarrollo\unity_project\Assets\`

### 1.1 Core Architecture (Scripts)
The application operates on a Manager/Controller pattern heavily reliant on singletons and a central EventBus.
- **State Management:** `AppStateMachine`, `GameManager`, `DroneStateController`
- **Events:** `EventBus`, `CoreEvents`, `StateChangedEvent`
- **Content Controllers:** `ExplodedViewManager`, `HighlightSystem`, `MaterialController`
- **Utility/Misc:** `AssetLoader`, `InputManager`, `AccessibilityManager`
- **Editor Fixes:** Custom WebGL and Graphics fixers (`FixGraphics.cs`, `WebGLBuildFixer.cs`)

### 1.2 UI Implementation (UI Toolkit)
The project utilizes Unity UI Toolkit, **not** Unity UI (uGUI), **not** HTML/CSS frontend.
- **Layouts (UXML):** `MainLayout.uxml`, `LoadingOverlay.uxml`, `ErrorOverlay.uxml`, `IconGallery.uxml`
- **Styles (USS):** `MainTheme.uss`, `Theme.uss`, `Overlays.uss`, `Hotspots.uss`
- **Theme Variables:** CSS custom properties present in `.uss` files.

### 1.3 Rendering & Visuals
The project relies on a Universal Render Pipeline (URP) with custom WebGL-optimized shaders.
- **Shaders:** `AnimatedGradientSkybox.shader`, `Blueprint.shader`, `ClippableLit.shader`, `Ghosted.shader`, `SolidColor.shader`, `Thermal.shader`, `Wireframe.shader`, `WireframeWebGL.shader`, `XRay.shader`.

## 2. Dependencies & Agent Skills
- `DOTween` is used for animations.
- The repository has native agent skills located in `e:\WebGL_tesis\.agent\skills\` (arch_guard, graph_builder, unity_asset_auditor, webgl_optimizer, etc.).

## 3. Factual Architectural Status (As of Analysis)

### 3.1 Singleton Usage (Heavy Pattern)
The application massively overuses the Singleton pattern. There are **34 classes** inheriting from `Singleton<T>` or `PersistentSingleton<T>`. This contrasts with previous audits that claimed there were only 6.
- *Examples:* `AppStateMachine`, `GameManager`, `EventBus`, `UIManager`, `ObjectPooler`, `InputManager`, `ErrorHandler`, `QualityManager`, `PerformanceMonitor`, `AnalyticsManager`, `AccessibilityManager`.

### 3.2 Main Controller (`UIModeController`)
`UIModeController` is monolithic, clocking in at 574 lines of code. It acts as an orchestrator for navigating Inspect, Analyze, and Studio modes, handling event bindings, visual class toggling, and panel navigation entirely inside one file.

### 3.3 Previous "Critical" Issues Resolved Natively
Compared to the `REMEDIATION_PLAN.md` audit, several critical issues *do not exist in the codebase*:
- **Triple Event Publishing:** `AppStateMachine.SetState()` fires exactly one `EventBus.Publish(new StateChangedEvent(previousState, currentState))`.
- **Duplicate VisualMode:** The `VisualMode` enum doesn't exist anymore. Shader mode buttons exist but are not coupled to a conflicting enum.
- **Bottom Sheet Flat Fields:** `MainLayout.uxml` features 3 `<ui:Foldout>` groups ("IDENTIFICATION", "SPECIFICATIONS", "ASSEMBLY") grouping the part details properly.
- **Loading/Error UI in C#:** The project uses `LoadingOverlay.uxml` and `ErrorOverlay.uxml`. They are not procedural C#.

## 4. Performance & WebGL Truth
(To be updated with Project Settings metrics momentarily...)

