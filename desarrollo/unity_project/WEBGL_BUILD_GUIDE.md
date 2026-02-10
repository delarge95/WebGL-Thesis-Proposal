# WebGL Build Settings Guide

> Based on Perplexity research (2025 best practices)

## Player Settings

### Publishing Settings
```
Compression Format: Gzip ✓
Data Caching: ON ✓
```

### Other Settings  
```
Managed Stripping Level: High
Strip Engine Code: ON ✓
API Compatibility: .NET Standard 2.1
IL2CPP Code Generation: OptimizeSize
Exception Handling: Explicit Only
```

### Resolution & Presentation
```
WebGL Memory Size: 512 MB
Run in Background: OFF
```

---

## Graphics Settings

Assign `Assets/Settings/URP_WebGL.asset` as active pipeline for WebGL builds.

---

## Expected Build Size

| Component | Size |
|-----------|------|
| WASM (code) | ~3-4 MB |
| Data (assets) | ~4-5 MB |
| Framework | ~1 MB |
| **Total (Gzip)** | **~8-10 MB** |

---

## Shader Variant Stripping

The `ShaderVariantStripper.cs` automatically removes unused variants:
- Additional lights
- Shadow cascades
- SSAO/Reflections
- Deferred rendering
- XR features

Expected reduction: **10-15%** of build size.

---

## Profiling

Add `WebGLProfiler` component to any GameObject for FPS monitoring.

Chrome DevTools:
1. F12 → Performance tab
2. Record → Interact → Stop
3. Analyze flame graph
