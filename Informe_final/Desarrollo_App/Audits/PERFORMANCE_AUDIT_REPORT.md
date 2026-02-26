# ⚡ PERFORMANCE AUDIT REPORT

## WebGL Optimization & Performance Deep Audit — Drone Viewer WebGL App

**Auditor Role:** Senior Graphics Programmer & WebGL Optimization Expert  
**Date:** 2025-07-15  
**Branch:** `feature/phase2-ux-redesign` (commit `1efb9fc`)  
**Scope:** `ProjectSettings/`, `QualitySettings.asset`, `Assets/Shaders/`, all C# scripts with Update() loops, WebGL build config  
**Target KPIs (from thesis proposal):**

| KPI             | Target                       | Source      |
| --------------- | ---------------------------- | ----------- |
| Polygon count   | < 100,000                    | Thesis §3.3 |
| Frame rate      | > 30 FPS on mid-range mobile | Thesis §3.3 |
| Shell load time | < 3 seconds                  | Thesis §3.3 |
| Full load time  | < 10 seconds                 | Thesis §3.3 |
| Draw calls      | < 50                         | Thesis §3.3 |
| VRAM            | < 64 MB                      | Thesis §3.3 |
| Build size      | < 50 MB (thesis aspiration)  | Thesis §3.3 |

---

## Executive Summary

The project's WebGL build configuration contains **5 Critical** and **7 High** performance issues that, if left unfixed, will likely prevent hitting the thesis KPIs on mid-range mobile. The most impactful are: **Full Exception Support** (massive WASM bloat), **2 GB maximum memory** (far exceeds mobile limits), **no code stripping** (inflated build), **no custom WebGL template** (no shell preloading), and **20 active Update() methods** creating unnecessary per-frame overhead.

The URP pipeline configuration is well-structured (Low/High tiers, SRP Batcher enabled, shadows off in Low), but several critical build-level settings negate these gains.

**Overall Performance Grade: C+** — Good runtime architecture (event-driven, SRP Batcher), undermined by build misconfiguration.

| Severity    | Count  |
| ----------- | ------ |
| 🔴 Critical | 5      |
| 🟠 High     | 7      |
| 🟡 Medium   | 6      |
| 🔵 Low      | 4      |
| **Total**   | **22** |

---

## 1. Build Configuration Audit

### Current WebGL Settings (from `ProjectSettings.asset`)

| Setting                         | Current Value         | Optimal Value                 | Status      |
| ------------------------------- | --------------------- | ----------------------------- | ----------- |
| `webGLExceptionSupport`         | `1` (Full)            | `0` (Explicitly Thrown Only)  | 🔴 CRITICAL |
| `webGLMaximumMemorySize`        | `2048` MB             | `512–1024` MB                 | 🔴 CRITICAL |
| `webGLInitialMemorySize`        | `512` MB              | `256` MB                      | 🟠 HIGH     |
| `webGLCompressionFormat`        | `0` (Gzip)            | `1` (Brotli)                  | 🟠 HIGH     |
| `webGLTemplate`                 | `APPLICATION:Default` | Custom (with shell preloader) | 🔴 CRITICAL |
| `webGLDataCaching`              | `1` (On)              | `1` ✅                        | ✅ OK       |
| `webGLDecompressionFallback`    | `1` (On)              | `1` ✅                        | ✅ OK       |
| `webGLThreadsSupport`           | `0` (Off)             | `0` ✅                        | ✅ OK       |
| `webGLPowerPreference`          | `2` (High)            | `2` ✅                        | ✅ OK       |
| `webGLMemoryGrowthMode`         | `2` (Geometric)       | `2` ✅                        | ✅ OK       |
| `webGLMemoryGeometricGrowthCap` | `96` MB               | `96` ✅                       | ✅ OK       |
| `webGLDebugSymbols`             | `0` (Off)             | `0` ✅                        | ✅ OK       |
| `webGLShowDiagnostics`          | `0` (Off)             | `0` ✅                        | ✅ OK       |
| `webGLNameFilesAsHashes`        | `0` (Off)             | `1` (enables CDN caching)     | 🟡 MEDIUM   |
| `webGLAnalyzeBuildSize`         | `0` (Off)             | `1` (enable during dev)       | 🟡 MEDIUM   |
| `webGLEnableWebGPU`             | `0` (Off)             | `0` ✅ (not yet stable)       | ✅ OK       |

### Code Stripping Settings

| Setting                       | Current Value            | Optimal Value        | Status      |
| ----------------------------- | ------------------------ | -------------------- | ----------- |
| `stripEngineCode`             | `1` (On)                 | `1` ✅               | ✅ OK       |
| `StripUnusedMeshComponents`   | `0` (Off)                | `1` (On)             | 🟠 HIGH     |
| `managedStrippingLevel`       | `{}` (default = Minimal) | `High` for WebGL     | 🔴 CRITICAL |
| `mipStripping`                | `0` (Off)                | `1` (On)             | 🟡 MEDIUM   |
| `il2cppCompilerConfiguration` | `{}` (default = Debug)   | `Master` for release | 🟠 HIGH     |
| `gcIncremental`               | `1` (On)                 | `1` ✅               | ✅ OK       |

---

## 2. Findings by Severity

### 🔴 CRITICAL (5)

#### PERF-C01: Full Exception Support Inflates WASM by 30–50%

**Setting:** `webGLExceptionSupport: 1`

Full exception support compiles complete try-catch/finally + stack trace information into WASM. This typically increases:

- **WASM binary size:** +30–50%
- **Runtime overhead:** +5–15% slower execution
- **Memory consumption:** +10–20 MB for stack trace metadata

**Impact on KPIs:** Directly threatens `<50MB build size` and `>30 FPS` targets.

**Fix:**

```
webGLExceptionSupport: 0  // Explicitly thrown only
```

Note: After changing, test all `try-catch` blocks still work. Only user-thrown exceptions will be caught; engine exceptions will crash silently.

---

#### PERF-C02: 2 GB Maximum Memory — Mobile Browsers Will Crash

**Setting:** `webGLMaximumMemorySize: 2048`

Most mobile browsers enforce a **1 GB practical limit** for WebAssembly linear memory. Setting 2048 MB means:

- iOS Safari: Likely crash on allocation attempt beyond ~1 GB
- Android Chrome: OOM kill on devices with < 4 GB RAM
- Desktop: Works but allocates swap, causing jank

**Fix:**

```
webGLMaximumMemorySize: 1024  // Safe ceiling
webGLInitialMemorySize: 256   // Start small, grow as needed
```

---

#### PERF-C03: No Custom WebGL Template — Missing Shell Preloader

**Setting:** `webGLTemplate: APPLICATION:Default`

The default Unity WebGL template shows a blank screen until the entire WASM + data file finishes downloading. There is **no shell preloader**, no progress bar during download, and no way to control the loading experience.

**Impact on KPIs:** Directly prevents achieving `<3s shell load` — users see nothing for 5–15 seconds on typical connections.

**Fix:**

1. Create `Assets/WebGLTemplates/DroneViewer/index.html` with:
   - Lightweight HTML/CSS loading screen (< 10 KB)
   - Progress bar bound to `UnityLoader.instantiate()` progress callback
   - Preload hints: `<link rel="preload" as="fetch" href="Build/...">` for critical assets
2. Set `webGLTemplate: APPLICATION:DroneViewer`

---

#### PERF-C04: Managed Stripping Level = Minimal (Default)

**Setting:** `managedStrippingLevel: {}` (empty = Minimal)

Minimal stripping removes almost nothing from the .NET assemblies, resulting in:

- **Bloated WASM:** 2–5 MB of unused .NET code compiled to WebAssembly
- **Longer compile times**
- **Larger download**

**Fix:**

```yaml
managedStrippingLevel:
  WebGL: 2 # High stripping
```

After changing, create a `link.xml` to preserve any assemblies used via reflection:

```xml
<linker>
  <assembly fullname="Assembly-CSharp" preserve="all"/>
</linker>
```

---

#### PERF-C05: 20 Active Update() Methods — Excessive Per-Frame Overhead

**Files:** Multiple scripts across codebase

The following 20 scripts have `Update()`, `FixedUpdate()`, or `LateUpdate()` methods:

| Script                     | Method   | Necessity                               |
| -------------------------- | -------- | --------------------------------------- |
| `OrbitCameraController.cs` | Update() | ✅ Required (continuous input)          |
| `InputManager.cs`          | Update() | ✅ Required (input polling)             |
| `SelectionManager.cs`      | Update() | ⚠️ Could be event-driven                |
| `CrossSectionManager.cs`   | Update() | ⚠️ Only needed when active              |
| `ExplodedViewManager.cs`   | Update() | ⚠️ Only needed during animation         |
| `DroneStateController.cs`  | Update() | ⚠️ Could be event-driven                |
| `KeyboardShortcuts.cs`     | Update() | ⚠️ Could use InputSystem callbacks      |
| `MeasurementTool.cs`       | Update() | ⚠️ Only needed when active              |
| `QualityManager.cs`        | Update() | 🔴 Should not have Update (monitor)     |
| `WebGLOptimizer.cs`        | Update() | 🔴 Should use InvokeRepeating/Coroutine |
| `AnnotationSystem.cs`      | Update() | ⚠️ Only needed when visible             |
| `ScreenshotManager.cs`     | Update() | 🔴 Should be event-driven               |
| `FPSCounter.cs`            | Update() | ⚠️ Dev tool — strip in release          |
| `PerformanceMonitor.cs`    | Update() | ⚠️ Dev tool — strip in release          |
| `RuntimeConsole.cs`        | Update() | 🔴 Dev tool — strip in release          |
| `WebGLProfiler.cs`         | Update() | 🔴 Dev tool — strip in release          |
| `HotspotManager.cs`        | Update() | ⚠️ Only when hotspots visible           |
| `LoadingController.cs`     | Update() | ⚠️ Only during loading                  |
| `SmartHotspot.cs`          | Update() | ⚠️ Billboard — use LateUpdate           |
| `TooltipSystem.cs`         | Update() | ⚠️ Only when tooltip visible            |

**Impact:** Each `Update()` call has a C#→native→C# marshalling overhead of ~0.01–0.05ms in IL2CPP/WASM. With 20 active updates, that's **0.2–1.0ms of pure overhead per frame** before any logic executes.

**Fix (Priority Order):**

1. **Strip debug tools:** Use `#if UNITY_EDITOR || DEVELOPMENT_BUILD` to exclude `FPSCounter`, `PerformanceMonitor`, `RuntimeConsole`, `WebGLProfiler` from release builds (saves 4 updates).
2. **Enable/disable pattern:** Make `CrossSectionManager`, `ExplodedViewManager`, `MeasurementTool`, `AnnotationSystem`, `HotspotManager`, `TooltipSystem`, `SmartHotspot`, `LoadingController` disable their `MonoBehaviour` when inactive: `enabled = false`.
3. **Replace polling with events:** `SelectionManager`, `DroneStateController`, `ScreenshotManager` can use `InputSystem` callbacks or `EventBus` subscriptions.
4. **Target:** Reduce active updates from 20 to **3–5** at any given time.

---

### 🟠 HIGH (7)

#### PERF-H01: Gzip Compression Instead of Brotli

**Setting:** `webGLCompressionFormat: 0`

Brotli compression typically achieves 15–25% better compression ratios than Gzip:
| Format | Typical WASM size | Typical data size |
|--------|------------------|------------------|
| None | 30 MB | 20 MB |
| Gzip | 10 MB | 7 MB |
| Brotli | 7–8 MB | 5–6 MB |

**Impact on KPI:** 2–4 MB smaller download → 1–2 seconds faster load on 3G.

**Fix:**

```
webGLCompressionFormat: 1  // Brotli
```

Note: Brotli requires HTTPS. Ensure hosting supports `Content-Encoding: br` headers or enable `webGLDecompressionFallback: 1` (already enabled).

---

#### PERF-H02: StripUnusedMeshComponents Disabled

**Setting:** `StripUnusedMeshComponents: 0`

Unity is shipping mesh data channels (tangents, UV2, UV3, colors) even if shaders don't use them. For a drone model with potentially 100K+ vertices, unused channels waste:

- **Tangents:** 16 bytes/vertex × 100K = **1.5 MB**
- **UV2/UV3:** 8 bytes/vertex × 100K = **0.8 MB each**
- **Colors:** 16 bytes/vertex × 100K = **1.5 MB**

**Total waste estimate:** 2–5 MB of mesh data.

**Fix:**

```
StripUnusedMeshComponents: 1
```

---

#### PERF-H03: IL2CPP Compiler Configuration = Debug (Default)

**Setting:** `il2cppCompilerConfiguration: {}` (empty = Debug)

Debug IL2CPP disables optimizations and adds bounds checking. For WebGL:

- **~20% slower execution** vs Master configuration
- **~10% larger WASM** binary

**Fix:**

```yaml
il2cppCompilerConfiguration:
  WebGL: 2 # Master (maximum optimization)
```

---

#### PERF-H04: Initial Memory 512 MB — Wasteful Allocation

**Setting:** `webGLInitialMemorySize: 512`

The app pre-allocates 512 MB of linear memory at startup. For a drone viewer with a single model, actual usage is likely 128–256 MB. Over-allocation:

- Increases startup time (memory page allocation)
- Wastes address space on 32-bit mobile browsers
- Can trigger OOM on low-memory devices

**Fix:**

```
webGLInitialMemorySize: 256  // Start at 256, grow to 1024 max
```

---

#### PERF-H05: No Asset Bundle Strategy

**Evidence:** `AssetLoader.cs` contains only placeholder comments:

```csharp
// In the future, replace this with Addressables or AssetBundles
// In production, this would be: Addressables.LoadAssetAsync<GameObject>(assetName);
```

All assets are packed into the main `data.unityweb` file. This means:

- **No incremental loading:** Everything downloads before first frame
- **No caching by asset:** Browser must re-download entire data file on any update
- **No LOD streaming:** High-res models load immediately

**Impact on KPI:** Directly threatens `<3s shell load` and `<10s full load`.

**Fix (Phased):**

1. **Phase 1 (Quick win):** Use Addressables to split the main 3D model into a separate bundle loaded after shell. This alone can achieve shell load in <3s.
2. **Phase 2:** Create LOD groups (LOD0: full detail for focus, LOD1: 50% for orbit, LOD2: 10% for thumbnails) as separate Addressable groups.
3. **Phase 3:** Implement `AsyncOperationHandle` loading with progress callback for seamless UX.

---

#### PERF-H06: HDR Enabled on Low Quality — Unnecessary for WebGL

**Setting:** `Low_PipelineAsset.asset: m_SupportsHDR: 1`

HDR rendering requires:

- 16-bit float framebuffer (2× memory vs LDR)
- Tone mapping pass (additional draw call)
- Higher GPU bandwidth

For a technical drone viewer on mobile WebGL, HDR provides minimal visual benefit (no bloom, no exposure adaptation in the current build) at significant cost.

**Fix:** In `Low_PipelineAsset.asset`:

```
m_SupportsHDR: 0
```

Keep HDR enabled in High quality for desktop.

---

#### PERF-H07: QualityManager Is a No-Op

**File:** `QualityManager.cs` (61 lines)

The `QualityManager` has `ScalableBufferManager.ResizeBuffers()` **commented out**. The entire class runs an `Update()` every frame but does nothing useful:

```csharp
void Update()
{
    // Adaptive resolution is disabled
    // ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
}
```

**Impact:** Wastes one Update() call per frame for zero benefit.

**Fix:** Either:

1. **Implement adaptive resolution** (uncomment + add FPS-based scaling logic), or
2. **Remove the class** and its Update() entirely.

---

### 🟡 MEDIUM (6)

#### PERF-M01: Reflection Probe Blending Enabled on Low Quality

**Setting:** `Low_PipelineAsset.asset: m_ReflectionProbeBlending: 1` and `m_ReflectionProbeBoxProjection: 1`

Reflection probe blending and box projection are expensive GPU operations with no visible benefit in a technical viewer using mostly unlit/custom shaders. Disable for Low quality.

---

#### PERF-M02: WebGL Texture Compression Format Not Verified

**Setting:** `m_BuildTargetDefaultTextureCompressionFormat: m_Formats: 05000000`

The format code `05` maps to DXT/S3TC compression which is:

- ✅ Excellent for desktop WebGL (hardware decoded)
- ⚠️ Not supported on all mobile GPUs (some Android devices decompress in software)

**Fix:** Consider using ASTC as the default for WebGL, with DXT fallback. Or use the Unity 6.0 multi-format texture import strategy to ship both ASTC and DXT, letting the runtime choose.

---

#### PERF-M03: Mip Stripping Disabled

**Setting:** `mipStripping: 0`

If the UI textures or certain 3D textures don't use all mip levels, unused mips are still shipped. Enable mip stripping to save 5–15% on texture download size.

---

#### PERF-M04: `webGLNameFilesAsHashes: 0` — No CDN Cache Busting

Build output files use human-readable names. Enable hash naming for:

- Automatic CDN cache invalidation on build updates
- Long cache TTLs (files never change — hash == content)

---

#### PERF-M05: No LOD Groups Detected

No references to `LODGroup` component found in the codebase or settings. The thesis targets `<100K polygons`, but if the full-detail model approaches that limit, there is no fallback for:

- Distant camera (orbit zoom out should use LOD1)
- Mobile devices (could use LOD1 permanently)

**Fix:** Implement 2–3 LOD levels using Unity's LODGroup component or a custom LOD switcher tied to camera distance.

---

#### PERF-M06: 9 Custom Shaders — Variant Explosion Risk

**Shader Analysis:**

| Shader                 | Passes | Variant Keywords | Est. Variants |
| ---------------------- | ------ | ---------------- | ------------- |
| Blueprint              | 2      | 1                | ~4            |
| ClippableLit           | 2      | 5                | ~64           |
| Ghosted                | 1      | 2                | ~8            |
| SolidColor             | 3      | 3                | ~16           |
| Thermal                | 1      | 1                | ~2            |
| Wireframe              | 2      | 2                | ~8            |
| WireframeWebGL         | 1      | 1                | ~2            |
| XRay                   | 2      | 0                | 2             |
| AnimatedGradientSkybox | 1      | 0                | 1             |
| **Total**              | **15** | **15**           | **~107**      |

`ClippableLit` has 5 `multi_compile`/`shader_feature` keywords across 2 passes = up to 64 variants. If all variants are compiled for WebGL, this adds significant WASM + shader cache size.

> **Corrección (audit-of-audit):** Solo 3 de los 9 shaders (Realistic/Lit, X-Ray, Solid Color) están expuestos al usuario; 4 más (Blueprint, Wireframe, Ghosted, Thermal) están ocultos via `display: none` como progressive disclosure. AnimatedGradientSkybox es un skybox de entorno. Sin embargo, **los 9 siguen compilándose para WebGL**, por lo que el riesgo de variant explosion sigue siendo válido a nivel de build — solo su impacto en UX es menor al reportado originalmente.

**Fix:**

1. Replace `multi_compile` with `shader_feature` where possible (only compiles variants actually used by materials).
2. Use `shader_feature_local` for keywords only used per-material.
3. Check `Build Report` (enable `webGLAnalyzeBuildSize: 1`) to see actual compiled variant count.

---

### 🔵 LOW (4)

#### PERF-L01: `vSyncCount: 0` in Low Quality

vSync disabled means the GPU renders as fast as possible, potentially causing:

- Screen tearing
- Unnecessary battery drain on mobile
- Inconsistent frame pacing

For WebGL, the browser's `requestAnimationFrame` already caps at display refresh rate, so this has minimal impact. But explicitly setting `Application.targetFrameRate = 60` in code would be safer.

#### PERF-L02: `asyncUploadTimeSlice: 2` Is Very Low

The default `2ms` time slice for async texture/mesh uploads means large assets upload slowly (one small chunk per frame). Increase to `4–8ms` for faster initial asset loading.

#### PERF-L03: `lodBias: 0.4` on Low Quality May Be Too Aggressive

A LOD bias of 0.4 means LOD transitions happen much closer to the camera. For the drone viewer's orbit camera, this could cause visible LOD popping. Consider `0.7` for a smoother experience.

#### PERF-L04: No `Application.targetFrameRate` Set Explicitly

While `vSyncCount: 0` lets the browser control frame rate, explicitly setting `Application.targetFrameRate = 60` in `WebGLOptimizer.cs` would ensure consistent behavior across platforms.

---

## 3. URP Pipeline Assessment

### Low Quality (WebGL Default)

| Feature                       | Setting           | Assessment                      |
| ----------------------------- | ----------------- | ------------------------------- |
| **SRP Batcher**               | ✅ Enabled        | Good — batches SetPass calls    |
| **Dynamic Batching**          | ❌ Disabled       | OK — SRP Batcher is better      |
| **Main Light Shadows**        | ❌ Disabled       | ✅ Correct for mobile WebGL     |
| **Additional Light Shadows**  | ❌ Disabled       | ✅ Correct                      |
| **MSAA**                      | `1` (Off)         | ✅ Correct                      |
| **HDR**                       | `1` (On)          | ⚠️ Should be Off (see PERF-H06) |
| **Render Scale**              | `1.0`             | ⚠️ Consider 0.75 for mobile     |
| **Reflection Probe Blending** | `1` (On)          | ⚠️ Should be Off (see PERF-M01) |
| **Depth Texture**             | `0` (Off)         | ✅ Correct                      |
| **Opaque Downsampling**       | `1` (2x bilinear) | ✅ OK                           |

### High Quality

| Feature                      | Setting               | Assessment           |
| ---------------------------- | --------------------- | -------------------- |
| **Main Light Shadows**       | ✅ 2048px, 2 cascades | OK for desktop       |
| **Additional Light Shadows** | ✅ 2048px             | OK for desktop       |
| **Shadow Distance**          | 40 units              | OK for small scene   |
| **Soft Shadows**             | ✅ High quality       | Heavy — monitor perf |

**Overall Pipeline Assessment:** Well-structured dual-tier system. Low needs minor tweaks (HDR off, reflection probes off). High is appropriate for desktop.

---

## 4. Memory Budget Analysis

| Component                           | Estimated Size | Notes                                             |
| ----------------------------------- | -------------- | ------------------------------------------------- |
| WASM binary (current)               | ~15–25 MB      | Full exceptions + minimal stripping inflates this |
| WASM binary (optimized)             | ~8–12 MB       | After PERF-C01 + C04 + H03                        |
| Data file (assets)                  | ~10–30 MB      | Drone model + textures + UI                       |
| Linear memory (initial)             | 512 MB         | 🔴 Over-allocated                                 |
| Linear memory (target)              | 256 MB         | After PERF-H04                                    |
| Shader cache                        | ~2–5 MB        | 107 estimated variants                            |
| USS/UXML runtime                    | ~1–2 MB        | UI Toolkit overhead                               |
| **Total download (current est.)**   | **25–55 MB**   | ⚠️ May exceed 50 MB KPI                           |
| **Total download (optimized est.)** | **13–25 MB**   | ✅ Within KPI                                     |

---

## 5. Runtime Performance Infrastructure

### Existing Tools

| Tool                    | File          | Lines | Assessment                                |
| ----------------------- | ------------- | ----- | ----------------------------------------- |
| `WebGLOptimizer.cs`     | Core/Utils    | 128   | Basic memory monitoring + GC triggers     |
| `QualityManager.cs`     | Core/Managers | 61    | No-op (adaptive resolution commented out) |
| `FPSCounter.cs`         | Debug         | 34    | Minimal — frame time display              |
| `PerformanceMonitor.cs` | Debug         | 107   | FPS + memory tracking                     |
| `WebGLProfiler.cs`      | Debug         | 101   | Extended metrics                          |

### Missing Performance Infrastructure

1. **No draw call monitoring** — Thesis KPI requires `<50 draw calls`, but no runtime tracking exists
2. **No frame budget enforcement** — If FPS drops below 30, no automatic quality degradation
3. **No asset loading profiling** — Can't measure shell vs. full load times
4. **No network timing correlation** — Download speed not factored into loading UX
5. **No adaptive resolution** — QualityManager has the code but it's disabled

---

## 6. Optimization Roadmap (Strictly Prioritized)

### 🔥 Phase 1: Build Settings (30 minutes — Highest ROI)

These changes require zero code modifications:

| #   | Action                                          | Expected Impact         | Setting                                 |
| --- | ----------------------------------------------- | ----------------------- | --------------------------------------- |
| 1   | Set exception support to Explicitly Thrown Only | -30% WASM size, +5% FPS | `webGLExceptionSupport: 0`              |
| 2   | Set managed stripping to High                   | -2–5 MB build size      | `managedStrippingLevel: WebGL: 2`       |
| 3   | Set IL2CPP to Master                            | +10–20% runtime perf    | `il2cppCompilerConfiguration: WebGL: 2` |
| 4   | Enable StripUnusedMeshComponents                | -2–5 MB mesh data       | `StripUnusedMeshComponents: 1`          |
| 5   | Reduce max memory to 1024 MB                    | Prevents mobile OOM     | `webGLMaximumMemorySize: 1024`          |
| 6   | Reduce initial memory to 256 MB                 | Faster startup          | `webGLInitialMemorySize: 256`           |
| 7   | Enable mip stripping                            | -5–15% texture size     | `mipStripping: 1`                       |
| 8   | Enable hash file naming                         | Better CDN caching      | `webGLNameFilesAsHashes: 1`             |
| 9   | Disable HDR on Low pipeline                     | -renderer pass, -memory | `m_SupportsHDR: 0`                      |
| 10  | Disable reflection probes on Low                | -GPU overhead           | `m_ReflectionProbeBlending: 0`          |

**Estimated cumulative impact:** 30–50% smaller build, 15–25% faster runtime.

### ⚡ Phase 2: Code Optimization (2–3 hours)

| #   | Action                                                          | Expected Impact           |
| --- | --------------------------------------------------------------- | ------------------------- |
| 1   | Guard debug scripts with `#if DEVELOPMENT_BUILD`                | -4 Update() calls         |
| 2   | Implement enable/disable pattern for optional managers          | -8 Update() calls at idle |
| 3   | Implement or remove QualityManager                              | -1 wasted Update()        |
| 4   | Replace `multi_compile` with `shader_feature` in custom shaders | -50% shader variants      |
| 5   | Switch compression to Brotli                                    | -20% download size        |

**Target:** Reduce active Update() from 20 to 3–5 at idle.

### 🚀 Phase 3: Advanced Optimizations (4–6 hours)

| #   | Action                                            | Expected Impact                |
| --- | ------------------------------------------------- | ------------------------------ |
| 1   | Create custom WebGL template with shell preloader | < 3s perceived load            |
| 2   | Implement Addressables for 3D model split-loading | < 10s full load                |
| 3   | Add LOD groups (2–3 levels)                       | -50% polygon count at distance |
| 4   | Implement adaptive resolution in QualityManager   | Maintain 30 FPS floor          |
| 5   | Add draw call counter for thesis KPI validation   | Measurable metrics             |

---

## 7. Positive Observations

1. **SRP Batcher enabled** — The most impactful WebGL batching optimization is already active.
2. **Dual quality tiers** — Proper Low/High separation with WebGL defaulting to Low.
3. **Shadows off in Low** — Correct decision for mobile WebGL.
4. **MSAA off** — Correct; post-process AA or none is better for WebGL.
5. **Incremental GC enabled** — Prevents frame-time spikes from garbage collection.
6. **WebGLOptimizer exists** — Shows awareness of memory management; needs expansion.
7. **Geometric memory growth** — Better than linear for unpredictable allocation patterns.
8. **Procedural skybox gradients** — Zero texture cost for environment backgrounds.
9. **Data caching enabled** — Repeat visits load from browser cache.
10. **Decompression fallback enabled** — Graceful degradation for servers without proper headers.

---

## 8. KPI Feasibility Assessment (Post-Optimization)

| KPI             | Target    | Feasibility         | Notes                                 |
| --------------- | --------- | ------------------- | ------------------------------------- |
| < 100K polygons | < 100,000 | ✅ Achievable       | Depends on 3D model, not code         |
| > 30 FPS mobile | > 30      | ✅ Likely           | After Phase 1+2 optimizations         |
| < 3s shell load | < 3s      | ⚠️ Requires Phase 3 | Custom template + Addressables needed |
| < 10s full load | < 10s     | ✅ Likely           | After Brotli + stripping              |
| < 50 draw calls | < 50      | ✅ Likely           | SRP Batcher + single model scene      |
| < 64 MB VRAM    | < 64 MB   | ✅ Likely           | Single model + compressed textures    |
| < 50 MB build   | < 50 MB   | ⚠️ Tight            | After all Phase 1 optimizations       |

---

_Generated by Senior Graphics Programmer & WebGL Optimization Expert — Pillar 3 of 4_
