# Unity 6 WebGL Advanced Optimization 2025

**For:** Drone visualization app with 7 custom shaders + 19 managers  
**Target:** < 10MB compressed | 60fps @ 1080p  
**Last Updated:** December 2025

---

## 1. Geometry Shaders & Wireframe Alternative

### ❌ Geometry Shaders NOT Supported in WebGL 2.0

WebGL 2.0 is based on **OpenGL ES 3.0**, which does NOT support geometry shaders.

```hlsl
#pragma geometry geom    // ❌ WILL NOT COMPILE IN WebGL
```

### ✅ Solution A: Edge Detection Post-Process (RECOMMENDED)

Most performant alternative for wireframe visualization.

```hlsl
Shader "Custom/WireframeEdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            #pragma only_renderers gles3
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            precision highp float;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _EdgeColor;
            float _EdgeThreshold;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = UnityObjectToClipPos(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 color = tex2D(_MainTex, input.uv);
                
                // Sobel edge detection on normal map
                float2 texelSize = _MainTex_TexelSize.xy;
                
                float3 normal = tex2D(_MainTex, input.uv).rgb;
                float3 normalRight = tex2D(_MainTex, input.uv + float2(texelSize.x, 0)).rgb;
                float3 normalUp = tex2D(_MainTex, input.uv + float2(0, texelSize.y)).rgb;
                
                // Sobel kernel
                float sobelX = length(normal - normalRight);
                float sobelY = length(normal - normalUp);
                float edge = sobelX + sobelY;
                
                // Wireframe overlay
                if (edge > _EdgeThreshold)
                    color.rgb = _EdgeColor.rgb;
                else
                    color.rgb *= (1.0 - edge);
                
                return color;
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Lit"
}
```

**Performance:** ~2-5% overhead, very clean wireframe effect

### ✅ Solution B: Barycentric Coordinates

For thin wireframe lines without post-processing.

```hlsl
// Requires barycentric coordinates passed from C#
Shader "Custom/WireframeBarycentric"
{
    Properties
    {
        _WireframeColor ("Wireframe Color", Color) = (1, 1, 1, 1)
        _WireframeWidth ("Width", Range(0, 0.5)) = 0.02
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
    }
    
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            #pragma only_renderers gles3
            
            #pragma vertex vert
            #pragma fragment frag
            
            precision highp float;
            
            float4 _WireframeColor;
            float4 _BaseColor;
            float _WireframeWidth;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 barycentric : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = UnityObjectToClipPos(input.positionOS);
                output.barycentric = input.barycentric;
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Find minimum barycentric coordinate
                float3 d = fwidth(input.barycentric);
                float3 a3 = smoothstep(float3(0, 0, 0), d * _WireframeWidth, input.barycentric);
                float edge = min(min(a3.x, a3.y), a3.z);
                
                half4 color = lerp(_WireframeColor, _BaseColor, edge);
                return color;
            }
            ENDHLSL
        }
    }
}
```

---

## 2. Shader Variant Stripping Implementation

### Complete IPreprocessShaders Script

Create `Assets/Editor/ShaderVariantStripper.cs`:

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
using UnityEditor.Rendering;

public class ShaderVariantStripper : IPreprocessShaders
{
    public int callbackOrder => 0;
    
    // Keywords safe to strip for WebGL drone visualization
    private static readonly HashSet<string> StripKeywordsWebGL = new()
    {
        // Multi-light variants
        "_ADDITIONAL_LIGHTS",
        "_ADDITIONAL_LIGHTS_VERTEX",
        "_ADDITIONAL_LIGHT_SHADOWS",
        
        // Advanced shadow features
        "_MAIN_LIGHT_SHADOWS_CASCADE",
        "_SHADOWS_SOFT",
        
        // Screen space effects (expensive)
        "_SCREEN_SPACE_AMBIENT_OCCLUSION",
        "_SCREEN_SPACE_SHADOWS",
        "_REFLECTION_PROBE_BLENDING",
        "_REFLECTION_PROBE_BOX_PROJECTION",
        
        // Deferred rendering
        "_DBUFFER_MRT1",
        "_DBUFFER_MRT2",
        "_DBUFFER_MRT3",
        
        // Advanced features
        "_PARALLAX_MAPPING",
        "_DETAIL_MULX2",
        "_DETAIL_SCALED",
        "_SURFACE_TYPE_TRANSPARENT",
    };
    
    public void OnProcessShader(
        Shader shader,
        ShaderSnippetData snippet,
        IList<ShaderCompilerData> variants)
    {
        // Only strip for WebGL builds
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            return;
        
        // Iterate backwards to avoid index issues
        for (int i = variants.Count - 1; i >= 0; --i)
        {
            var variant = variants[i];
            
            // Check each strip keyword
            foreach (var keyword in StripKeywordsWebGL)
            {
                if (variant.keywordSet.Contains(new ShaderKeyword(keyword)))
                {
                    Debug.Log($"[ShaderStripper] Removing {shader.name}" +
                             $" variant with keyword: {keyword}");
                    variants.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
```

### Expected Build Size Impact

**Before Stripping:**
- Shader code: ~2-3 MB
- URP shaders: ~1,200 variants
- Custom shaders: ~400 variants

**After Stripping:**
- Shader code: ~0.5-1 MB (60-70% reduction)
- Variants: ~400 total (70% reduction)

---

## 3. Font Subsetting for WebGL

### Using FontTools (Python)

**Install:**
```bash
pip install fonttools
```

### Basic Latin Subsetting (RECOMMENDED)

```bash
# Subset to Basic Latin + Latin-1 Supplement
pyftsubset Inter-Regular.ttf \
  --unicodes=U+0000-007F,U+00A0-00FF \
  --output-file=Inter-Regular-latin.ttf

# Result: 200KB → 40KB (80% reduction)
```

### By Text Content

```bash
# Include only characters used in your UI
pyftsubset SpaceGrotesk-Bold.ttf \
  --text="Drone Visualization 0123456789 %.°/-" \
  --output-file=SpaceGrotesk-Bold-ui.ttf
```

### Batch Script

Create `subset_fonts.sh`:

```bash
#!/bin/bash

INPUT_DIR="Assets/Fonts"
OUTPUT_DIR="Assets/Fonts/Subsetted"

mkdir -p "$OUTPUT_DIR"

for font in "$INPUT_DIR"/*.ttf; do
    name=$(basename "$font" .ttf)
    echo "Subsetting $name..."
    
    pyftsubset "$font" \
        --unicodes="U+0000-007F,U+00A0-00FF" \
        --output-file="$OUTPUT_DIR/${name}-subset.ttf"
done

echo "Done! Subsetted fonts in $OUTPUT_DIR"
```

**Run:**
```bash
chmod +x subset_fonts.sh
./subset_fonts.sh
```

---

## 4. Texture Compression Configuration

### WebGL 2.0 Texture Format Support

| Format | Desktop | Mobile | Recommendation |
|--------|---------|--------|-----------------|
| S3TC/DXT | ✅ Good | ❌ Rare | Desktop fallback |
| ASTC | ⚠️ Rare | ✅ Good | Mobile only |
| ETC2 | ❌ Poor | ✅ Good | Mobile only |
| Basis Universal | ✅ Good | ✅ Good | **BEST** |

### Use Basis Universal (Recommended)

**Setup:**
```bash
# Install basisu
brew install basisu  # macOS
# or: sudo apt install basisu  # Linux
# or: download from https://github.com/BinomialLLC/basis_universal
```

**Convert textures:**
```bash
# Convert PNG to Basis KTX2
basisu your_texture.png -ktx2

# Result: 4MB PNG → 100-150KB KTX2
```

**In Unity:**
1. Import `.ktx2` files normally
2. Inspector: Set compression to "Compressed"
3. Use in shaders as normal texture

**Benefits:**
- 4-8x smaller than PNG/JPEG
- Automatic platform transcoding
- Works on desktop + mobile WebGL
- No special shader code needed

### Per-Texture Settings (For DXT/ETC2)

**Inspector → Texture Import Settings:**

```
Compression:
├── Normal Quality Compressed ✓
├── Format: (Auto - DXT for desktop, ETC2 for mobile)
├── Compression Quality: Normal
└── Crunch Compression: ON ✓
    └── Level: 50 (balance quality/size)

Filter Mode: Bilinear (good for technical viz)
Anisotropic: 1-2 (not 16)

Generate Mip Maps: ON ✓
Mip Map Filter: Box
```

### Expected Compression Savings

**Typical Drone Visualization Textures:**

| Type | Uncompressed | DXT | Basis | Savings |
|------|-------------|-----|-------|---------|
| Diffuse (1024×1024) | 4 MB | 500 KB | 150 KB | 96.2% |
| Normal (1024×1024) | 4 MB | 500 KB | 150 KB | 96.2% |
| Roughness (512×512) | 1 MB | 125 KB | 35 KB | 96.5% |
| Metallic (512×512) | 1 MB | 125 KB | 35 KB | 96.5% |

**For 15 textures:** ~28 MB uncompressed → 2.5 MB Basis (91% reduction)

---

## 5. Code Stripping & IL2CPP Configuration

### Recommended Player Settings

```
Edit → Project Settings → Player → Other Settings:

IL2CPP:
  ├── IL2CPP Code Generation: Faster (Smaller) Builds ✓
  ├── IL2CPP Cpp Compiler Configuration: Release ✓
  └── Enable Incremental GC: ON ✓

Scripting:
  ├── API Compatibility Level: .NET Standard 2.1 ✓
  └── Managed Stripping Level: HIGH ✓

Other:
  ├── Strip Engine Code: ON ✓
  ├── Vertex Attribute Caching: ON ✓
  └── Allow downloads over HTTP: ON ✓
```

### IL2CPP Code Generation Options

```csharp
// Code equivalent
PlayerSettings.SetIl2CppCodeGeneration(
    NamedBuildTarget.WebGL,
    Il2CppCodeGeneration.OptimizeSize);  // Smaller builds

PlayerSettings.managedStrippingLevel = ManagedStrippingLevel.High;
PlayerSettings.stripEngineCode = true;
```

### Managed Stripping Level Comparison

| Level | Size | Safety | Risk | Notes |
|-------|------|--------|------|-------|
| Low | -20% | Very High | None | Use for development |
| Medium | -45% | High | Low | Good balance |
| High | -65% | Medium | Medium | **Recommended WebGL** |

### Safe Practices with High Stripping

```csharp
// ❌ RISKY - Reflection
var type = Type.GetType("MyNamespace.MyClass");
if (type != null) {
    var instance = Activator.CreateInstance(type);
}

// ✅ SAFE - Direct reference
public class MyClass { }
// Reference it directly somewhere to keep it alive
// Or...

// ✅ SAFE - Type registry
public static class TypeRegistry {
    public static Type[] AllManagerTypes = new[] {
        typeof(GameManager),
        typeof(SelectionManager),
        typeof(ExplodedViewManager),
        // ... all your types
    };
}
```

### UI Toolkit + High Stripping

**✅ Fully compatible.** No known issues. UI Toolkit is well-integrated with stripping analysis.

```csharp
// Safe with High stripping
var root = GetComponent<UIDocument>().rootVisualElement;
var button = root.Q<Button>("my-button");
button.clicked += OnClick;  // ✓ Safe
```

---

## 6. WebGL Build Settings

### Compression Format

**Gzip (RECOMMENDED):**
```
✅ Universal browser support
✅ Automatic decompression
✅ ~70-80% compression ratio
✅ No server configuration needed
```

**Brotli:**
```
✅ Better compression (~15% smaller)
⚠️ Slower decompression
❌ Additional server setup required
❌ Not worth complexity for WebGL
```

**Configuration:**
```
Player Settings → Publishing Settings:
├── Compression Format: Gzip ✓
├── Data Caching: ON ✓
└── Large Allocation Header: OFF
```

### Memory Size

```
Player Settings → Resolution:
├── WebGL Memory Size: 512 MB (recommended for drone app)
│   └── Range: 256-1024 MB depending on scene
├── Initial Data URL: [leave blank]
└── Cached Assets Lifetime: 30 (days)
```

**Recommendation:** Start at 512 MB. Increase only if out-of-memory errors occur.

### Exception Handling

```
Player Settings → Other Settings:
├── Exception Handling: Explicit Only ✓
│   ├── None: Smallest, no stack traces (production)
│   ├── Explicit Only: Good balance (RECOMMENDED)
│   └── Full: Large, full debugging (development only)
```

**Impact:**
- None: ~2 MB smaller
- Explicit Only: 2-3 MB overhead
- Full: ~5-6 MB overhead

---

## 7. WebGL Performance Profiling

### Chrome DevTools Method

**Step 1: Open DevTools**
```
Mac: Cmd+Option+I
Windows/Linux: Ctrl+Shift+I
```

**Step 2: Performance Tab**
```
1. Click "Record" (red circle)
2. Interact with drone visualization
3. Click "Stop" after 10-30 seconds
```

**Step 3: Analyze Flame Graph**
```
Timeline shows:
├── Script (C# code execution)
│   ├── EventBus callbacks
│   ├── Manager updates
│   └── UI Toolkit rendering
├── Rendering (GPU submission)
│   ├── Culling
│   ├── Draw calls
│   └── Shader compilation
└── GPU (can't see in profile, infer from timeline)
```

### Key Metrics

| Metric | Target | Status |
|--------|--------|--------|
| Frame Time | < 16.67 ms | 60fps ✓ |
| Main Thread | < 10 ms | Good |
| Scripting | < 5 ms | Good |
| Rendering | < 3 ms | Good |
| GPU Memory | < 512 MB | Good |
| Build Size | 8-10 MB | Target |

### Real-Time Profiling in Game

```csharp
public class WebGLProfiler : MonoBehaviour
{
    private int frames = 0;
    private float lastTime = 0;
    private float updateInterval = 0.5f;
    
    private void Update()
    {
        frames++;
        
        if (Time.time - lastTime >= updateInterval)
        {
            float fps = frames / updateInterval;
            float ms = 1000f / fps;
            long heapMB = GC.GetTotalMemory(false) / (1024 * 1024);
            
            Debug.Log($"FPS: {fps:F1} | " +
                     $"Frame: {ms:F2}ms | " +
                     $"Heap: {heapMB}MB");
            
            frames = 0;
            lastTime = Time.time;
        }
    }
}
```

### Network Profiling

**Network Tab Shows:**
```
drone.wasm:    ~3-4 MB    (largest)
drone.data:    ~5-6 MB    (assets)
drone.js:      ~1 MB      (loader)
Other:         ~0.5 MB

Total compressed: ~8-10 MB
Total uncompressed: ~35-45 MB
```

**Check for:**
- ✓ Gzip Content-Encoding header
- ✓ HTTP/2 (parallel downloads)
- ✓ Cache-Control headers
- ✓ No unnecessary files

---

## Advanced Optimization Checklist

### Pre-Build Checklist

- [ ] Wireframe shader converted to edge detection
- [ ] IPreprocessShaders script created and tested
- [ ] Fonts subsetted to Latin characters only
- [ ] All textures compressed (Basis Universal preferred)
- [ ] No geometry shaders or compute shaders used
- [ ] 19 managers reviewed for reflection usage

### Build Configuration Checklist

- [ ] Managed Stripping Level: High
- [ ] Strip Engine Code: enabled
- [ ] IL2CPP Code Generation: OptimizeSize
- [ ] Gzip Compression: enabled
- [ ] Exception Handling: Explicit Only
- [ ] WebGL Memory: 512 MB
- [ ] API Compatibility: .NET Standard 2.1

### Post-Build Validation

- [ ] Build size: 8-10 MB compressed
- [ ] Frame time: < 16.67 ms (60fps)
- [ ] Memory usage: < 512 MB
- [ ] No shader compilation at runtime
- [ ] Chrome DevTools profiling: 0 warnings/errors
- [ ] Network tab: All files compressed with Gzip

---

## Official Documentation

| Topic | URL |
|-------|-----|
| IPreprocessShaders | https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Build.IPreprocessShaders.OnProcessShader.html |
| Font Subsetting | https://docs.unity3d.com/6000.2/Documentation/Manual/UIE-font-subsetting.html |
| Managed Stripping | https://docs.unity3d.com/2020.1/Documentation/Manual/ManagedCodeStripping.html |
| WebGL Textures | https://docs.unity3d.com/2022.1/Documentation/Manual/webgl-texture-format.html |
| WebGL Memory | https://docs.unity3d.com/Manual/webgl-memory.html |
| Recommended Settings | https://docs.unity.cn/Manual/web-optimization-player.html |

---

## Community Tools

- **Basis Universal**: https://github.com/BinomialLLC/basis_universal
- **FontTools**: https://github.com/fonttools/fonttools
- **Web Texture Formats**: https://www.donmccurdy.com/2024/02/11/web-texture-formats/

---

**Last Updated:** December 2025  
**Compatibility:** Unity 6 LTS | URP 17.3.0 | WebGL 2.0  
**Target:** 60fps @ 1080p | ~8-10 MB compressed | 512 MB heap
