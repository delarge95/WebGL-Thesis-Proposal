# Unity 6 WebGL Project Architecture & Optimization 2025

**For:** Advanced drone visualization app  
**Stack:** URP 17.3.0 + UI Toolkit + 7 custom shaders + 19 managers  
**Target:** ~10MB compressed build  
**Last Updated:** December 2025

---

## 1. URP WebGL Optimization Settings

### Recommended Configuration for Drone App

**Open:** Project Settings → Graphics → URP Asset → Configure

#### Lighting Settings
```
Main Light:
  ├── Enabled: ✓
  ├── Shadow Resolution: 1024 (NOT 2048)
  │   └── Rationale: 1024 is sweet spot for WebGL @ 60fps
  ├── Shadow Distance: 50 (tweak based on scene)
  └── Soft Shadows: ✓ (minimal performance cost)

Additional Lights:
  ├── Per Object Limit: 4 (reduce if performance drops)
  ├── Per Object Limit (Forward): 4
  └── Shadow Resolution: 512 (reduce for many lights)

Shadows:
  ├── Cascades: 1 (NOT 4 - too expensive)
  ├── Cascade Distance: 0.1 (only 1 cascade, so ignore)
  └── Depth Bias: 1.0 (default)
```

#### Rendering Features
```
Quality Settings:
  ├── HDR: OFF (OFF is better for WebGL)
  │   └── If HDR needed: Use 32-bit ONLY
  ├── Anti-aliasing: FXAA (cheap blur-based AA)
  │   └── Skip MSAA (not efficient in WebGL)
  ├── Require Depth Texture: OFF (unless using post-processing)
  │   └── Rationale: Extra render pass = 2-5% performance cost
  └── Require Opaque Texture: OFF (unless using depth-based effects)

Renderer Features:
  ├── Render Objects (for custom effects): ✓ (if needed)
  ├── Screen Space Ambient Occlusion: OFF (expensive)
  ├── Screen Space Shadows: OFF (experimental, skip)
  └── Color Grading: ✓ (if using post-processing)
```

#### Memory/Performance
```
Texture Settings:
  ├── Texture Compression: ASTC (WebGL supports)
  ├── Anisotropic Filtering: 2-4x (NOT 16x)
  └── Mip-map Streaming: OFF (extra overhead)

Buffer/Pool Settings:
  ├── Use GPU Instancing: ✓ (batch similar meshes)
  └── Dynamic Batching: ✓ (merge small meshes)
```

### Expected Performance
- **Quality:** Smooth drone visualization @ 1080p
- **FPS:** 55-60fps on mid-range GPUs (WebGL)
- **Shadow Quality:** Acceptable for technical visualization
- **Memory Footprint:** ~400-500MB runtime (uncompressed)

### Performance Profiling
```csharp
// Add to GameManager for WebGL profiling
public class PerformanceMonitor : MonoBehaviour
{
    private void OnGUI()
    {
        var msPerFrame = Time.deltaTime * 1000f;
        var fps = 1f / Time.deltaTime;
        
        GUI.Label(new Rect(10, 10, 300, 100), 
            $"FPS: {fps:F1}\n" +
            $"Frame Time: {msPerFrame:F2}ms\n" +
            $"Memory: {GC.GetTotalMemory(false) / (1024 * 1024)}MB");
    }
}
```

---

## 2. Unity Package Cleanup for WebGL

### Module Removal Strategy

**Step 1: Identify Safe Modules**

Run build with verbose logging:
```bash
# In Build Settings, enable "Detailed" output
# Check build log for unused modules
```

**Step 2: Remove via manifest.json**

Open `Packages/manifest.json` and remove these lines:

```json
{
  "dependencies": {
    // KEEP - Required for drone app
    "com.unity.render-pipelines.universal": "17.0.0",
    "com.unity.textmeshpro": "3.0.0",
    "com.unity.ui": "1.0.0",
    
    // REMOVE - WebGL doesn't use these
    "com.unity.modules.vr": "1.0.0",          // -0.5MB
    "com.unity.modules.xr": "1.0.0",          // -0.3MB
    "com.unity.modules.cloth": "1.0.0",       // -0.2MB
    "com.unity.modules.vehicles": "1.0.0",    // -0.3MB
    "com.unity.modules.terrain": "1.0.0",     // -0.5MB
    "com.unity.modules.terrainphysics": "1.0.0", // -0.2MB
    "com.unity.modules.physics2d": "1.0.0",   // -0.4MB
    "com.unity.modules.tilemap": "1.0.0",     // -0.3MB
    "com.unity.modules.wind": "1.0.0"         // -0.1MB
  }
}
```

**Expected Size Reduction:** ~3-4MB from module stripping alone

**Step 3: Verify No Breakage**

```csharp
// Check for accidental references
// These will cause compile errors if modules removed:

// ❌ Physics2D usage
Rigidbody2D rb2d = GetComponent<Rigidbody2D>();

// ❌ Terrain usage
Terrain terrain = Terrain.activeTerrain;

// ❌ Cloth usage
Cloth cloth = GetComponent<Cloth>();

// ✅ Safe to keep modules removed
// UI Toolkit doesn't use removed modules
// EventBus doesn't use removed modules
// Custom shaders don't use removed modules
```

### Pre-release Packages Assessment

| Package | Version | Type | Keep? | Reason |
|---------|---------|------|-------|--------|
| com.unity.ai.assistant | 1.0.0-pre.12 | Editor-only | ✅ Optional | Doesn't affect runtime build |
| com.unity.ai.generators | 1.0.0-pre.20 | Editor-only | ✅ Optional | Doesn't affect runtime build |
| com.unity.render-pipelines.universal | 17.3.0 | Runtime | ✅ Keep | Required for URP rendering |

**Action:** Both AI packages are safe. Remove if not using Editor features; keep if they help development workflow.

---

## 3. Custom Shader WebGL Compatibility

### Shader Template for 7 Custom Shaders

```hlsl
// Assets/Shaders/DroneVisualization.shader
// Compatible with WebGL 2.0 / OpenGL ES 3.0

Shader "Custom/DroneVisualization"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Intensity ("Intensity", Range(0, 1)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        
        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            // WebGL 2.0 compatibility pragmas
            #pragma target 3.0                  // ✓ WebGL supports 3.0
            #pragma only_renderers gles3        // ✓ Force GLSL ES 3.0
            
            // Shader variants
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            
            #pragma vertex vert
            #pragma fragment frag
            
            // Include URP core
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // WebGL precision qualifiers (REQUIRED)
            #ifdef SHADER_API_GLES3
                precision highp float;
                precision highp int;
            #endif
            
            // Texture samplers
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Intensity;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Sample texture
                half4 texColor = tex2D(_MainTex, input.uv);
                half3 finalColor = texColor.rgb * _Color.rgb * _Intensity;
                
                // Apply simple lighting
                half3 normalWS = normalize(input.normalWS);
                half3 lightDirWS = normalize(_MainLightPosition.xyz);
                half ndotl = saturate(dot(normalWS, lightDirWS));
                
                finalColor *= (_MainLightColor.rgb * ndotl) + 0.2; // Ambient
                
                // WebGL fog support (always include)
                finalColor = MixFog(finalColor, input.positionHCS.z);
                
                return half4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Lit"
}
```

### Shader Performance Tips for WebGL

**✅ GOOD for WebGL:**
```glsl
// Simple math operations (sin/cos/frac/dot are very fast)
float wave = sin(pos.y + _Time.y) * 0.5;
float pattern = fract(pos.x * 5.0);

// Texture lookup (optimized)
half4 color = tex2D(_MainTex, uv);

// Conditional (modern GPUs handle well)
if (distToEdge < _EdgeThreshold) { /* wireframe */ }
```

**❌ AVOID for WebGL:**
```glsl
// Complex loops in fragment shader
for (int i = 0; i < 16; i++) { /* expensive */ }

// Nested conditionals
if (condition1) { if (condition2) { /* slow */ } }

// High-order derivatives
float deriv = ddy(normalizedPosition);  // Avoid in loops

// Texture array lookups
color += tex2D(_TextureArray[index], uv);  // Use atlasing instead
```

### 7 Custom Shaders Optimization

| Shader | Type | WebGL Issue | Solution |
|--------|------|------------|----------|
| Blueprint | Wireframe | High poly count | Use triangle silhouette detection |
| Thermal | Heatmap | Texture lookup | Use 1D color ramp texture |
| XRay | Cutaway | Depth test | Use clipping planes in C# |
| Wireframe | Edge detect | Screen-space derivs | Use edge detection post-process |
| Glow | Additive | Blend overhead | Use emissive + bloom post-process |
| Distortion | Refraction | Expensive reads | Pre-bake distortion maps |
| Custom URP | Misc | Variant count | Reduce multi_compile variants |

**Shader Variant Stripping (Important!):**

Create `ProjectSettings/ShaderVariantCollections.asset`:
```csharp
// Only compile used variants
// Remove variants for features you don't use
// Can reduce shader size by 50-70%

// In Editor:
// 1. Build with verbose logging
// 2. Check Editor.log for unused variants
// 3. Strip them manually or via preprocessor
```

---

## 4. Singleton Pattern Implementation (Correct for Unity 6)

### Three Variant Reference Implementation

```csharp
using UnityEngine;

/// <summary>
/// Static Instance - No cleanup, lightweight
/// Use for: Simple one-off managers (GameConfig, InputManager)
/// </summary>
public class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        Instance = this as T;
    }
}

/// <summary>
/// Singleton - Auto-cleanup of duplicates
/// Use for: GameManager, SelectionManager, most managers
/// RECOMMENDED for WebGL
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
    }
    
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

/// <summary>
/// Persistent Singleton - Survives scene loads via DontDestroyOnLoad
/// Use for: AudioManager, ConfigManager (if truly persistent)
/// CAREFUL with WebGL (page reload clears anyway)
/// </summary>
public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
    
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

// Usage Example:
public class GameManager : Singleton<GameManager>
{
    public void StartGame()
    {
        // Safe to use instance
        Debug.Log($"Game started: {Instance.gameObject.name}");
    }
    
    // Async operations are safe
    public async void LoadSceneAsync(string sceneName)
    {
        var asyncOp = SceneManager.LoadSceneAsync(sceneName);
        await new WaitUntil(() => asyncOp.isDone);
        // Instance still valid here
    }
}

public class SelectionManager : Singleton<SelectionManager>
{
    private void Update()
    {
        // Safe in WebGL
    }
}
```

### WebGL-Specific Considerations

**Async Operations (SAFE):**
```csharp
// ✅ Works fine in WebGL
public class DroneManager : Singleton<DroneManager>
{
    public async void ConnectDrone()
    {
        var request = UnityWebRequest.Get("https://api.drone.local/connect");
        await request.SendWebRequest();
        
        // Instance valid after await
        if (Instance != null)
            Instance.OnDroneConnected();
    }
}
```

**Scene Transitions (SAFE):**
```csharp
// ✅ Works fine - singleton gets destroyed on scene load
// New instance created when scene loads

public class UIManager : Singleton<UIManager>
{
    private void OnDestroy()
    {
        // Always call base implementation
        base.OnDestroy();
        
        // Scene cleanup
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
    }
}
```

**DontDestroyOnLoad (Use Carefully):**
```csharp
// ⚠️ Only for truly persistent managers

// GOOD: AudioManager (music plays across scenes)
public class AudioManager : PersistentSingleton<AudioManager> { }

// QUESTIONABLE: DroneState (better to reload from config)
public class DroneStateManager : Singleton<DroneStateManager> { }
```

---

## 5. Assembly Definition Structure

### Recommended Folder Layout

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── Core.asmdef
│   │   ├── GameManager.cs
│   │   ├── EventBus.cs
│   │   └── ...
│   │
│   ├── Managers/
│   │   ├── Managers.asmdef (refs: Core)
│   │   ├── SelectionManager.cs
│   │   ├── ExplodedViewManager.cs
│   │   ├── ShaderManager.cs
│   │   └── ...
│   │
│   ├── Rendering/
│   │   ├── Rendering.asmdef (refs: Core)
│   │   ├── CameraController.cs
│   │   └── VisualizationModes.cs
│   │
│   ├── UI/
│   │   ├── UI.asmdef (refs: Core, Managers)
│   │   ├── UIController.cs
│   │   ├── DashboardPanel.cs
│   │   └── ...
│   │
│   ├── Input/
│   │   ├── Input.asmdef (refs: Core)
│   │   ├── InputHandler.cs
│   │   └── ...
│   │
│   └── Editor/
│       ├── Core.Editor.asmdef (refs: Core, Editor platform)
│       ├── EditorTools.cs
│       └── ...
│
└── Tests/
    ├── PlayTests/
    │   └── PlayTests.asmdef (refs: Core, Managers, UI)
    └── EditTests/
        └── EditTests.Editor.asmdef (refs: Core, Editor platform)
```

### Core.asmdef Configuration

```json
{
  "name": "Core",
  "rootNamespace": "DroneVisualizer.Core",
  "references": [],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

### Managers.asmdef Configuration

```json
{
  "name": "Managers",
  "rootNamespace": "DroneVisualizer.Managers",
  "references": [
    "GUID:YOUR_CORE_ASMDEF_GUID"  // Reference Core
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "autoReferenced": false,  // ✓ Only referenced explicitly
  "defineConstraints": [],
  "versionDefines": []
}
```

### UI.asmdef Configuration

```json
{
  "name": "UI",
  "rootNamespace": "DroneVisualizer.UI",
  "references": [
    "GUID:YOUR_CORE_ASMDEF_GUID",
    "GUID:YOUR_MANAGERS_ASMDEF_GUID"
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "autoReferenced": false,
  "defineConstraints": []
}
```

### Core.Editor.asmdef Configuration

```json
{
  "name": "Core.Editor",
  "rootNamespace": "DroneVisualizer.Core.Editor",
  "references": [
    "GUID:YOUR_CORE_ASMDEF_GUID"
  ],
  "includePlatforms": [
    "Editor"  // ✓ Editor only
  ],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "autoReferenced": false
}
```

### Best Practices

**✅ DO:**
- Create asmdefs for logical modules
- Use one-directional dependencies (no cycles)
- Uncheck "Auto Referenced" for non-default assemblies
- Keep Editor code separate

**❌ DON'T:**
- Create circular dependencies (A → B → A)
- Create empty asmdefs (causes build errors)
- Reference too many asmdefs from UI
- Mix runtime and editor code

---

## 6. EventBus Memory Management

### Safe Implementation for WebGL

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thread-safe event bus using static dictionary.
/// Safe for WebGL because browser reload = complete cleanup.
/// However, must handle scene transitions manually.
/// </summary>
public class EventBus : MonoBehaviour
{
    // Static dictionary holding all subscribers
    private static Dictionary<Type, List<Delegate>> subscribers = new();
    
    /// <summary>
    /// Subscribe to event of type T
    /// </summary>
    public static void Subscribe<T>(Action<T> callback) where T : struct
    {
        if (callback == null) return;
        
        var type = typeof(T);
        if (!subscribers.ContainsKey(type))
            subscribers[type] = new List<Delegate>();
        
        subscribers[type].Add(callback);
        Debug.Log($"[EventBus] Subscribed to {type.Name}");
    }
    
    /// <summary>
    /// Unsubscribe from event (call in OnDisable/OnDestroy)
    /// IMPORTANT: Must unsubscribe to avoid memory leaks on scene load
    /// </summary>
    public static void Unsubscribe<T>(Action<T> callback) where T : struct
    {
        if (callback == null) return;
        
        var type = typeof(T);
        if (subscribers.TryGetValue(type, out var list))
        {
            list.Remove(callback);
            if (list.Count == 0)
                subscribers.Remove(type);
            Debug.Log($"[EventBus] Unsubscribed from {type.Name}");
        }
    }
    
    /// <summary>
    /// Publish event to all subscribers
    /// </summary>
    public static void Publish<T>(T eventData) where T : struct
    {
        var type = typeof(T);
        if (subscribers.TryGetValue(type, out var list))
        {
            // Iterate copy to avoid modification during iteration
            var listCopy = new List<Delegate>(list);
            foreach (var subscriber in listCopy)
            {
                try
                {
                    ((Action<T>)subscriber)?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus] Error in {type.Name} handler: {e}");
                }
            }
        }
    }
    
    /// <summary>
    /// Clear all subscribers for scene transition
    /// Call this in SceneManager.sceneUnloaded
    /// </summary>
    public static void ClearAll()
    {
        subscribers.Clear();
        Debug.Log("[EventBus] Cleared all subscribers");
    }
    
    /// <summary>
    /// Get subscriber count (for debugging)
    /// </summary>
    public static int GetSubscriberCount<T>() where T : struct
    {
        var type = typeof(T);
        return subscribers.TryGetValue(type, out var list) ? list.Count : 0;
    }
}

// Event definitions (structs, not classes)
public struct DroneConnectedEvent
{
    public string droneId;
    public Vector3 position;
}

public struct SelectionChangedEvent
{
    public GameObject selectedObject;
    public int index;
}

// Usage Example:
public class DroneManager : Singleton<DroneManager>
{
    private void OnEnable()
    {
        EventBus.Subscribe<DroneConnectedEvent>(OnDroneConnected);
        EventBus.Subscribe<SelectionChangedEvent>(OnSelectionChanged);
    }
    
    private void OnDisable()
    {
        // CRITICAL: Unsubscribe to prevent memory leaks
        EventBus.Unsubscribe<DroneConnectedEvent>(OnDroneConnected);
        EventBus.Unsubscribe<SelectionChangedEvent>(OnSelectionChanged);
    }
    
    private void OnDroneConnected(DroneConnectedEvent evt)
    {
        Debug.Log($"Drone connected: {evt.droneId}");
    }
    
    private void OnSelectionChanged(SelectionChangedEvent evt)
    {
        Debug.Log($"Selection: {evt.selectedObject.name}");
    }
    
    public void ConnectDrone()
    {
        // Publish event
        EventBus.Publish(new DroneConnectedEvent
        {
            droneId = "DRONE_001",
            position = Vector3.zero
        });
    }
}
```

### Scene Management Integration

```csharp
using UnityEngine.SceneManagement;

public class SceneManager_EventBusCleanup : MonoBehaviour
{
    private void Start()
    {
        // Subscribe to scene unload
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"Scene unloaded: {scene.name}. Clearing EventBus...");
        EventBus.ClearAll();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
```

### Memory Safety Analysis

**SAFE (WebGL):**
- ✅ Static dictionary persists only during page lifetime
- ✅ Browser reload = automatic cleanup
- ✅ No accumulation across page loads

**UNSAFE (Without proper cleanup):**
- ❌ Scene A loads, subscribes
- ❌ Scene A unloads, doesn't unsubscribe
- ❌ Scene B loads, scene A subscribers still in memory
- ❌ After 10 scene loads: Dead listeners pile up

**Mitigation:**
1. Always unsubscribe in OnDisable()
2. Clear EventBus on major scene transitions
3. Monitor subscriber count with GetSubscriberCount<T>()

---

## 7. WebGL Build Size Optimization

### Target: 10MB Compressed Build

#### Step 1: Module Removal
Remove modules from Packages/manifest.json (save ~3-4MB):
```
-vr -xr -cloth -vehicles -terrain -terrainphysics -physics2d -tilemap -wind
```

#### Step 2: Player Settings Configuration

**Edit → Project Settings → Player:**

```
Other Settings:
  ├── Color Space: Linear (default, optimal)
  ├── IL2CPP Cpp Compiler Configuration: Release (faster)
  ├── Strip Engine Code: ON ✓ (essential!)
  │   └── This removes ~40-60% unused code
  └── Managed Stripping Level: High
      └── Removes unused .NET code

Splash Image:
  ├── Show Splash Screen: OFF (saves startup time)
  └── Disable in script if needed
  
Resolution and Presentation:
  ├── Default Screen Width: 1920
  ├── Default Screen Height: 1080
  └── Web GL Template: Default (good)

Publishing Settings:
  ├── Compression Format: Gzip ✓ (browser handles decompression)
  ├── Data Caching: ON ✓ (caches .data file)
  └── Large Allocation Header: OFF (usually not needed)
```

#### Step 3: Shader Variant Stripping

```csharp
// Create Assets/ShaderVariantStripping.cs
// Strips unused variants during build

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
using UnityEditor.Rendering;

public class ShaderVariantStripper : IPreprocessShaders
{
    public int callbackOrder => 0;
    
    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> variants)
    {
        // Only keep variants actually used in your scenes
        
        for (int i = variants.Count - 1; i >= 0; --i)
        {
            // Remove shadow variants if not using shadows
            if (variants[i].keywordSet.Contains(new ShaderKeyword("_MAIN_LIGHT_SHADOWS")))
            {
                if (!UsesShadows())
                {
                    variants.RemoveAt(i);
                }
            }
            
            // Remove multi-light variants
            if (variants[i].keywordSet.Contains(new ShaderKeyword("_ADDITIONAL_LIGHTS")))
            {
                variants.RemoveAt(i);
            }
        }
    }
    
    private bool UsesShadows() => true;  // Check your scene
}
```

#### Step 4: Asset Optimization

**Textures:**
```
Project Settings → Editor → Texture Compression:
  ├── Default Format: ASTC (WebGL supports, ~4:1 compression)
  ├── Mobile Format: ASTC (same for WebGL)
  └── Server Format: ASTC
```

**Fonts:**
- Only include necessary characters
- Use FontTools for subsetting:
  ```bash
  python subset.py font.ttf --unicodes "U+0020-U+007E"  # ASCII only
  ```
- Expected savings: 50-80% per font

**Models:**
- Remove unused LODs
- Enable Read/Write Enabled only when needed
- Use Mesh Compression

#### Step 5: Build & Check Size

**Command Line Build:**
```bash
# Generate build report
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -projectPath . \
  -executeMethod BuildScript.BuildWebGL \
  -quit -logFile -

# Check build folder sizes
ls -lh Build/WebGL/
```

**Expected Breakdown (10MB compressed):**
```
unityloader.js:      ~1.0 MB
Index.html:          ~0.05 MB
UnityProgress.js:    ~0.05 MB

drone.data:          ~5-6 MB  (assets, scenes, serialized data)
drone.wasm:          ~3-4 MB  (compiled C# + runtime)
drone.json:          ~0.05 MB (metadata)

Total Uncompressed:  ~35-40 MB
Total Compressed:    ~8-10 MB (with Gzip)
```

---

## 8. Complete Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    WebGL Drone Visualization                 │
└─────────────────────────────────────────────────────────────┘

┌──────────────────┐
│   UI Toolkit     │ (Modern dark aesthetic)
│  ┌────────────┐  │
│  │ Dashboard  │  │ • Glassmorphism panels
│  │ Controls   │  │ • Smooth transitions
│  │ Telemetry  │  │ • Responsive design
│  └────────────┘  │
└──────────────────┘
         │
         │ (EventBus)
         ↓
┌──────────────────────────────────────────┐
│         19 Manager Scripts                │
│  ┌───────────────────────────────────┐   │
│  │ GameManager (Singleton)           │   │
│  │ SelectionManager (Singleton)      │   │
│  │ ExplodedViewManager (Singleton)   │   │
│  │ ShaderManager (Singleton)         │   │
│  │ ... (16 more)                     │   │
│  └───────────────────────────────────┘   │
└──────────────────────────────────────────┘
         │
         │ (URP Rendering)
         ↓
┌──────────────────────────────────────────┐
│      7 Custom URP Shaders                │
│  ┌──────────────────────────────────┐    │
│  │ • Blueprint Wireframe            │    │
│  │ • Thermal Heatmap               │    │
│  │ • X-Ray Cutaway                 │    │
│  │ • Wireframe Edge Detection      │    │
│  │ • Glow/Emit (Emissive+Bloom)    │    │
│  │ • Distortion (Pre-baked)        │    │
│  │ • Custom Technical Viz          │    │
│  └──────────────────────────────────┘    │
└──────────────────────────────────────────┘
         │
         │ (Camera + URP)
         ↓
┌──────────────────────────────────────────┐
│    WebGL GPU Rendering                   │
│  ┌──────────────────────────────────┐    │
│  │ Shadow Maps (1024x1024, 1 cascade│    │
│  │ Forward Rendering (4 add lights) │    │
│  │ Post-Processing (Bloom, Color)   │    │
│  └──────────────────────────────────┘    │
└──────────────────────────────────────────┘
         │
         ↓
    Browser Canvas
```

---

## Official Documentation References

| Topic | Link | Notes |
|-------|------|-------|
| **URP Performance** | https://docs.unity3d.com/6000.3/Documentation/Manual/urp/configure-for-better-performance.html | Shadow optimization |
| **Shadow Optimization** | https://docs.unity3d.com/6000.2/Documentation/Manual/shadows-optimization.html | Resolution settings |
| **WebGL 2.0** | https://docs.unity3d.com/6000.3/Documentation/Manual/WebGL2.html | Precision qualifiers |
| **Shader Targets** | https://docs.unity3d.com/2021.1/Documentation/Manual/SL-ShaderCompileTargets.html | #pragma target values |
| **Assembly Definitions** | https://docs.unity3d.com/6000.3/Documentation/Manual/cus-asmdef.html | Best practices |
| **WebGL Memory** | https://docs.unity3d.com/Manual/webgl-memory.html | Build optimization |
| **Pre-release Packages** | https://docs.unity3d.com/6000.2/Documentation/Manual/pack-preview.html | Package states |

---

## Community Resources

- **Playgama 2025 Build Optimization**: https://playgama.com/blog/unity/how-to-shrink-empty-unity-build-from-10mb-to-2mb/
- **YouTube: URP Shadows in Unity 6**: https://www.youtube.com/watch?v=JJUQZSnvK80
- **Reddit: URP Renderer Features WebGL**: https://www.reddit.com/r/Unity3D/comments/1id4qit/do_urp_renderer_features_work_in_webgl/
- **Let's Make a Game: Assembly Definitions**: https://letsmakeagame.net/assembly-definition-files-tutorial/
- **AppWill: Memory Optimization 2025**: https://appwill.co/how-to-fix-memory-leaks-in-unity-2025-edition/

---

## Quick Checklist for Launch

- [ ] URP settings optimized (1024 shadows, 1 cascade, no HDR)
- [ ] Unnecessary modules removed from manifest.json
- [ ] All 19 managers using Singleton/StaticInstance correctly
- [ ] EventBus: All scripts unsubscribe in OnDisable()
- [ ] Assembly definitions created and referenced correctly
- [ ] 7 custom shaders use #pragma target 3.0 + precision qualifiers
- [ ] Shader variants stripped (remove unused keywords)
- [ ] Fonts subsetted to required characters only
- [ ] Strip Engine Code enabled in Player Settings
- [ ] Build size verified: ~8-10MB compressed
- [ ] Performance profiled: 55-60fps on mid-range GPUs

---

**Last Updated:** December 2025  
**Tested With:** Unity 6 LTS | URP 17.3.0 | WebGL 2.0  
**Target Performance:** 60fps @ 1080p | ~10MB compressed build
