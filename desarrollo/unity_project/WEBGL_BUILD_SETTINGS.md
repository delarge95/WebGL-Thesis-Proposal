# WebGL Build Settings Reference
## Optimized Configuration for Unity 6.0 LTS

---

## Player Settings Configuration

### Resolution and Presentation

```
Edit > Project Settings > Player > Resolution and Presentation

Default Canvas Width: 1920
Default Canvas Height: 1080
Run In Background: Enabled
```

### WebGL Template

```
WebGL Template: Default (or custom DroneViewer)

Custom template location:
Assets/WebGLTemplates/DroneViewer/
├── index.html
├── style.css
└── TemplateData/
```

---

## Publishing Settings (Critical)

```
Edit > Project Settings > Player > Publishing Settings

Compression Format: Brotli
  ├── Best compression ratio
  ├── Requires server configuration for .br files
  └── Use "Disabled" for testing/development

Decompression Fallback: Enabled
  └── Required for servers that don't support Brotli

Name Files As Hashes: Enabled
  └── Improves caching behavior

Data Caching: Enabled
  └── Faster subsequent loads

Exception Support: None (or Explicitly Thrown Only)
  └── Reduces file size significantly

Memory Size: 512 (MB)
  └── Increase if you get out-of-memory errors
  └── 256 MB minimum, 512 MB recommended for complex scenes

Initial Memory Size: 32 (MB)
  └── Starting allocation, grows as needed

Growth Mode: Geometric
  └── Efficient memory growth pattern

Texture Compression Format: ASTC (for quality) or ETC2 (for compatibility)
  └── ASTC: Better quality, modern devices
  └── ETC2: Wider compatibility
```

---

## Other Settings (Performance)

```
Edit > Project Settings > Player > Other Settings

Rendering:
├── Color Space: Linear
├── Auto Graphics API: Disabled
├── Graphics APIs: WebGL 2.0 only (remove WebGL 1.0)
└── Static Batching: Enabled

Scripting Backend: IL2CPP (default for WebGL)

Managed Stripping Level: High
  └── Removes unused code, smaller build

.NET API Compatibility: .NET Standard 2.1

Script Call Optimization: Fast But No Exceptions
  └── Best performance, but no try/catch in C#

Allow 'unsafe' Code: Disabled (unless needed)

Stack Trace: None (for Release builds)
  ├── Reduces load time
  └── Enable for debugging only
```

---

## Quality Settings

```
Edit > Project Settings > Quality

Recommended WebGL Profile:
├── Pixel Light Count: 2
├── Anti-Aliasing: 2x (MSAA)
├── Soft Particles: Disabled
├── Realtime Reflection Probes: Disabled
├── Soft Vegetation: Disabled
├── Shadows:
│   ├── Shadow Distance: 50
│   ├── Shadow Resolution: Medium
│   └── Shadow Cascades: 2
├── Texture Quality: Full Res
├── Anisotropic Textures: Per Texture
├── LOD Bias: 1.0
└── VSync: Don't Sync (let browser handle)
```

---

## Graphics Settings

```
Edit > Project Settings > Graphics

Scriptable Render Pipeline:
└── URP Asset (WebGL optimized)

Transparency Sort Mode: Perspective

Always Included Shaders:
├── WebGL/ClippableLit
├── WebGL/XRay
├── WebGL/Blueprint
├── WebGL/Thermal
├── WebGL/Wireframe
├── WebGL/SolidColor
├── WebGL/Ghosted
└── Universal Render Pipeline/Lit
```

---

## URP Asset Settings

```
Create URP Asset for WebGL:
Right-click > Create > Rendering > URP Asset (Production)

Recommended Settings:
├── Main Light:
│   ├── Cast Shadows: Enabled
│   └── Shadow Resolution: 1024
├── Additional Lights:
│   ├── Maximum: 4
│   └── Shadows: Disabled
├── Shadows:
│   ├── Max Distance: 50
│   └── Cascade Count: 2
├── Post-processing:
│   └── Enabled (but minimal effects)
├── Anti-aliasing (MSAA): 2x
└── HDR: Disabled (for performance)
```

---

## Build Settings

```
File > Build Settings

Platform: WebGL
Development Build: Disabled (for release)
Autoconnect Profiler: Disabled
Deep Profiling: Disabled
Build and Run: Use for testing
```

### Build Size Optimization

```
Estimated Build Size Targets:
├── .wasm.br: < 15 MB (compressed)
├── .data.br: < 20 MB (compressed)
├── .js: < 2 MB
├── .framework.js: < 500 KB
└── Total: < 40 MB compressed

Uncompressed sizes will be 3-5x larger
```

---

## Texture Import Settings

```
For each texture:
├── Max Size: 1024 (or 512 for small parts)
├── Compression:
│   ├── Normal Quality for Albedo
│   └── High Quality for Normal Maps
├── Generate Mip Maps: Enabled
├── Format Override (WebGL):
│   ├── ASTC 6x6 (good quality/size)
│   └── or ETC2 8 bits (highest compatibility)
└── sRGB: Enabled for albedo, Disabled for normal
```

---

## Audio Import Settings

```
For each audio clip:
├── Force to Mono: Enabled (for UI sounds)
├── Load Type: Decompress On Load (small files)
├── Compression Format: Vorbis
├── Quality: 70%
└── Sample Rate: Preserve or Override to 22050 Hz
```

---

## Script Optimization

### Memory-Friendly Patterns

```csharp
// Use object pools instead of Instantiate/Destroy
ObjectPooler.Instance.GetPooledObject("VFX");

// Cache component references
private Camera _mainCamera;
void Start() => _mainCamera = Camera.main;

// Use StringBuilder for string concatenation
var sb = new StringBuilder();
sb.Append("Part: ").Append(partName);

// Avoid LINQ in hot paths
// LINQ allocates memory and triggers GC
```

### WebGL-Specific Considerations

```csharp
// No threading support
// Avoid: Task.Run, Thread, async/await with threading

// No File I/O
// Use PlayerPrefs or IndexedDB via plugins

// JavaScript interop
[DllImport("__Internal")]
private static extern void OpenURL(string url);
```

---

## Testing Checklist

### Before Build
- [ ] Remove Development Build flag
- [ ] Set Managed Stripping to High
- [ ] Disable Debug.Log in production
- [ ] Verify all shaders compile for WebGL
- [ ] Check texture compression formats

### After Build
- [ ] Test in Chrome (baseline)
- [ ] Test in Firefox
- [ ] Test in Edge
- [ ] Test on mobile browser (Chrome Android)
- [ ] Verify load time < 10 seconds
- [ ] Check FPS with DevTools (target > 30)
- [ ] Test all interactive features
- [ ] Verify no console errors

---

## Common Issues & Fixes

| Issue | Solution |
|-------|----------|
| Black screen | Check WebGL 2.0 support, verify shaders |
| Slow loading | Enable Brotli compression, reduce texture sizes |
| Out of memory | Increase Memory Size in settings |
| Missing textures | Check texture compression format |
| Shaders not working | Add to "Always Included Shaders" |
| Input not working | Verify canvas focus, check Input System |

---

*WebGL Settings Version: 1.0*
*Unity Version: 6.0 LTS*
*Last Updated: December 2024*
