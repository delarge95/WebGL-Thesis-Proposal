# ⚡ WebGL Performance & Optimization Audit Report

**Date:** February 2026  
**Scope:** Unity ProjectSettings, QualitySettings, WebGL Configuration  
**KPIs Evaluated:** <100k Polygons, >30 FPS (Mid-range Mobile), <3s Shell Load, <50MB Build.

---

## 🚨 Major Optimization Flaws (Bottlenecks)

### 1. WebAssembly Memory Constraints (Severity: CRITICAL)
*   **Issue (`ProjectSettings.asset`)**: 
    *   `webGLInitialMemorySize: 512` (MB) is defined, but `webGLMemorySize: 32` (MB) is also set (likely legacy conflict). 
    *   `webGLMaximumMemorySize: 2048` (2GB) is excessive for mobile Chrome/Safari, which will crash the tab if WebKit memory budgets are exceeded.
*   **KPI Impact**: A heavy starting allocation (512MB) combined with unbounded growth (2GB) will cause Instant Out-Of-Memory (OOM) crashes on iOS Safari and mid-range Androids.
*   **Resolution**: Set `Initial Memory` to 128MB. Enable `Memory Growth`. Set `Maximum Memory` strictly to 512MB to enforce discipline.

### 2. Texture Compression Inefficiencies (Severity: HIGH)
*   **Issue**: The default texture compression is not explicitly forced to ASTC in `ProjectSettings`. ASTC is the standard for mobile WebGL 2.0. Uncompressed or DXT textures will shatter the `<50MB build` KPI and cause massive GPU bus bandwidth bottlenecks.
*   **KPI Impact**: Extremely long download times and Frame Time > 33.33ms due to VRAM bandwidth saturation.
*   **Resolution**: Force `Texture Compression Format: ASTC` for the WebGL build target.

### 3. Deficient WebGL Template Loading (Severity: MEDIUM)
*   **Issue**: `webGLTemplate: APPLICATION:Default` is being used. The default Unity template does not handle background loading of Asset Bundles gracefully, and its loading bar freezes the main thread.
*   **KPI Impact**: Fails the `<3s shell load` KPI. The screen will remain white/grey until the entire 50MB payload downloads.
*   **Resolution**: Create a custom `Assets/WebGLTemplates/` using asynchronous JS promises (or a package like BetterMinimalWebGL) to display a lightweight HTML/CSS shell instantly (< 1s) while WASM downloads in the background.

### 4. Quality Settings (Shadows & SRP) Overkill (Severity: MEDIUM)
*   **Issue (`QualitySettings.asset`)**: The WebGL target uses the 'Low' tier by default, but even 'Low' has `shadowCascades: 1` and `shadowDistance: 20` enabled globally. For a technical drone viewer, real-time shadow casting from multiple parts destroys mobile fill-rate.
*   **KPI Impact**: Prevents achieving > 30 FPS.
*   **Resolution**: Disable real-time shadows completely. Use **Baked Ambient Occlusion (AO)** and baked lighting (Lightprobes/Lightmaps) for the drone. Rely strictly on the PBR shader's environment cubemap for reflections and pseudo-shadowing.

---

## 🛠️ Step-by-Step Optimization Plan

1.  **Memory Diet (Project Settings)**
    *   Modify `ProjectSettings.asset` under WebGL Publishing Settings.
    *   Set Initial Memory to `128MB`, Max Memory `512MB`.
2.  **Asset Pipeline Setup**
    *   Change Android/WebGL texture compression globally to **ASTC 6x6**.
3.  **Custom HTML Shell**
    *   Implement a custom WebGL template to satisfy the 3-second Time-To-Interactive (TTI) KPI for the UI shell.
4.  **Graphics Downgrade**
    *   Turn off Universal Render Pipeline (URP) real-time shadows in the WebGL Quality tier. Ensure the Custom Render Pipeline Asset used by WebGL is stripped of Post-Processing anti-aliasing (use SMAA or none).
