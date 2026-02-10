# Unity UI Toolkit (UIElements) WebGL 2025 - Complete Technical Guide

**For:** WebGL drone visualization application (Unity 6 / 2023.3+)  
**Aesthetic:** Awwwards-style dark mode with glassmorphism & smooth transitions  
**Last Updated:** December 2025

---

## 1. USS Transition Syntax & Easing Functions

### ✅ Shorthand Syntax (SUPPORTED)

```uss
.panel-fade {
    transition: opacity 0.3s ease;
}

.panel-slide {
    transition: translate 0.5s ease-out;
}

/* Multiple properties */
.card-interactive {
    transition: opacity 0.3s ease, translate 0.3s ease, background-color 0.3s ease;
}
```

### ✅ Individual Properties (SUPPORTED)

```uss
.dashboard-element {
    transition-property: opacity, scale, translate;
    transition-duration: 0.3s, 0.4s, 0.5s;
    transition-delay: 0s, 100ms, 200ms;
    transition-timing-function: ease-in, ease-out, linear;
}
```

### ❌ cubic-bezier() NOT Supported

Instead, use these **built-in easing functions**:

| Easing Function | Use Case |
|-----------------|----------|
| `linear` | Constant speed animations |
| `ease` | Default, slow start & end |
| `ease-in` | Slow start, fast end |
| `ease-out` | Fast start, slow end |
| `ease-in-out` | Slow start & end |
| `ease-in-sine` | Smooth ease-in |
| `ease-out-sine` | Smooth ease-out |
| `ease-in-out-sine` | Smooth both ends |
| `ease-in-cubic` | Cubic acceleration |
| `ease-out-cubic` | Cubic deceleration |
| `ease-in-out-cubic` | Cubic both ways |
| `ease-in-elastic` | Bouncy ease-in |
| `ease-out-elastic` | Bouncy ease-out |
| `ease-in-out-elastic` | Bouncy both ends |
| `ease-in-back` | With overshoot in |
| `ease-out-back` | With overshoot out |
| `ease-in-out-back` | With overshoot both |
| `ease-out-bounce` | Bounce effect |

### **Awwwards Dark Mode Transition Examples**

```uss
/* Glassmorphic card hover effect */
.glass-card {
    background-color: rgba(20, 20, 25, 0.7);
    transition: background-color 0.3s ease-out,
                transform 0.3s ease-out,
                opacity 0.3s ease-out;
}

.glass-card:hover {
    background-color: rgba(30, 30, 40, 0.9);
    transform: translateY(-4px);
    opacity: 1;
}

/* Smooth button interaction */
.btn-primary {
    opacity: 0.85;
    transition: opacity 0.2s ease, background-color 0.2s ease;
}

.btn-primary:hover {
    opacity: 1;
    background-color: var(--color-accent-bright);
}

/* Status indicator pulse */
.status-pulse {
    opacity: 1;
    transition: opacity 0.6s ease-in-out;
}

.status-pulse:hover {
    opacity: 0.6;
}
```

### C# Event-Driven Transitions

```csharp
public class DroneUIController : MonoBehaviour
{
    private VisualElement dronePanel;
    private Label statusText;

    public void OnDroneConnect()
    {
        // Animate opacity
        dronePanel.style.transitionProperty = new List<StylePropertyName>
        {
            new StylePropertyName("opacity"),
            new StylePropertyName("translate")
        };
        dronePanel.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.5f),
            new TimeValue(0.5f)
        };
        dronePanel.style.transitionTimingFunction = new List<EasingFunction>
        {
            EasingFunction.EaseOut(),
            EasingFunction.EaseOut()
        };
        
        dronePanel.style.opacity = 1f;
        dronePanel.style.translate = new Translate(0, 0);
    }

    public void OnDroneDisconnect()
    {
        dronePanel.style.opacity = 0.5f;
        dronePanel.style.translate = new Translate(0, 20);
    }
}
```

---

## 2. USS Gradient Backgrounds

### ❌ Direct CSS Gradients NOT Supported

```uss
/* ❌ THIS DOES NOT WORK */
.panel {
    background-color: linear-gradient(90deg, #1a1a2e, #16213e);
}
```

### ✅ Solution A: Use Vector Images (SVG) with Gradient Fills

**Step 1: Create SVG with gradient in Blender/Figma/Illustrator**

```xml
<!-- gradient-bg.svg -->
<svg viewBox="0 0 1920 1080" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="darkGradient" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:#1a1a2e;stop-opacity:1" />
      <stop offset="50%" style="stop-color:#16213e;stop-opacity:1" />
      <stop offset="100%" style="stop-color:#0f3460;stop-opacity:1" />
    </linearGradient>
  </defs>
  <rect width="1920" height="1080" fill="url(#darkGradient)"/>
</svg>
```

**Step 2: Import as Vector Image in Unity**
- Place `gradient-bg.svg` in `Assets/`
- In Inspector: Set **Generated Asset Type** → **UI Toolkit Vector Image**
- Unity auto-generates `gradient-bg_0.asset`

**Step 3: Reference in USS**

```uss
.dashboard-bg {
    background-image: url("Assets/gradient-bg_0.asset");
    width: 100%;
    height: 100%;
    position: absolute;
}
```

### ✅ Solution B: Programmatic Gradients (C#)

```csharp
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class GradientPanelManager : MonoBehaviour
{
    private VisualElement backgroundPanel;
    
    public void CreateGradientBackground()
    {
        // Create programmatically via Vector Graphics API
        var gradientFill = new GradientFill
        {
            Type = GradientFillType.Linear,
            Stops = new GradientStop[]
            {
                new GradientStop { Color = new Color(0.1f, 0.1f, 0.18f, 1f), StopPercentage = 0f },
                new GradientStop { Color = new Color(0.09f, 0.13f, 0.25f, 1f), StopPercentage = 0.5f },
                new GradientStop { Color = new Color(0.06f, 0.2f, 0.38f, 1f), StopPercentage = 1f }
            }
        };
        
        // This approach requires VectorUtils for sprite generation
        // See: https://docs.unity3d.com/6000.5/Documentation/Manual/ui-systems/work-with-vector-graphics.html
    }
}
```

### ✅ Solution C: Layered Semi-Transparent Elements

```uss
.panel-background {
    background-color: rgba(26, 26, 46, 0.8);
    border-radius: 8px;
    border: 1px solid rgba(255, 255, 255, 0.1);
}

.panel-background::before {
    /* Overlay effect for depth */
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: linear-gradient(135deg, 
        rgba(255, 255, 255, 0.05) 0%,
        rgba(255, 255, 255, 0) 100%);
    border-radius: 8px;
    pointer-events: none;
}
```

---

## 3. Custom Fonts (Google Fonts) in UI Toolkit

### Step 1: Import Font File

1. Get `.ttf` file (e.g., from [fonts.google.com](https://fonts.google.com))
   - Download "Inter" or "Space Grotesk"
   - Place in `Assets/Fonts/` folder

2. In Inspector, verify import settings:
   - Font texture size: 256 × 256 (or higher for high-DPI)
   - Rendering mode: Hinted Raster or Smooth

### Step 2: Create Text Settings Asset

```
Right-click in Project
→ Create → UI Toolkit → Text Settings
→ Name it "DefaultTextSettings"
```

Configure in Inspector:
- Default Font Overrides: [empty unless needed]
- Default Font Size: 14

### Step 3: Create Panel Settings

```
Right-click in Project
→ Create → UI Toolkit → Panel Settings
→ Name it "DashboardPanelSettings"
```

Configure in Inspector:
- **Theme Style Sheet**: Assign your `.uss` file
- **Text Settings**: Assign "DefaultTextSettings"
- **Scale Mode**: Constant Pixel Size (for WebGL)
- **Reference Resolution**: 1920 × 1080

### Step 4: Reference Font in USS

```uss
/* Modern dark theme with custom fonts */
.dashboard {
    --font-primary: "Inter";
    --font-heading: "Space Grotesk";
}

Label {
    -unity-font: url("Assets/Fonts/Inter-Regular.ttf");
    font-size: 14px;
    color: rgba(255, 255, 255, 0.9);
}

.heading {
    -unity-font: url("Assets/Fonts/Space_Grotesk-Bold.ttf");
    font-size: 24px;
    font-weight: 700;
    color: rgba(255, 255, 255, 1);
    letter-spacing: 1px;
}

.caption {
    -unity-font: url("Assets/Fonts/Inter-Light.ttf");
    font-size: 12px;
    color: rgba(255, 255, 255, 0.6);
}
```

### Step 5: Use in UXML

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements" 
         xmlns:uie="UnityEditor.UIElements">
    <ui:VisualElement name="dashboard" class="dashboard">
        <ui:Label text="Drone Status" class="heading"/>
        <ui:Label text="Connected" class="caption"/>
    </ui:VisualElement>
</ui:UXML>
```

### Step 6: Attach to Scene

```csharp
public class UIBootstrap : MonoBehaviour
{
    public PanelSettings panelSettings;
    public TextAsset uxml;
    public StyleSheet uss;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.styleSheets.Add(uss);
    }
}
```

### WebGL Font Performance Tips

- **Texture Atlas**: Unity automatically batches font glyphs (good for WebGL)
- **Font Size Variety**: Keep to 3-4 primary sizes (12px, 14px, 18px, 24px)
- **Local Fonts Preferred**: .ttf files compile into build, no network requests
- **Build Size**: Each font adds ~50-200KB depending on character set
- **Glyph Cache**: UI Toolkit caches rendered glyphs—reusable across scenes

---

## 4. Glassmorphism & Backdrop Blur

### ❌ backdrop-filter: blur() NOT Supported in USS

UI Toolkit does **not** support `backdrop-filter` CSS property.

### ✅ Solution A: Semi-Transparent Layering (Recommended)

```uss
/* Dark glassmorphic card without blur */
.glass-card {
    background-color: rgba(20, 20, 30, 0.4);  /* 40% opacity dark */
    border: 1px solid rgba(255, 255, 255, 0.15);
    border-radius: 12px;
    backdrop-filter: none;  /* UI Toolkit limitation */
    
    /* Fake depth with subtle gradients */
    background-image: linear-gradient(135deg,
        rgba(100, 150, 200, 0.1),
        rgba(50, 80, 120, 0.05));
}

.glass-card::before {
    /* Light rim effect */
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 1px;
    background: linear-gradient(90deg,
        transparent,
        rgba(255, 255, 255, 0.2),
        transparent);
}

.glass-card:hover {
    background-color: rgba(30, 30, 45, 0.6);  /* Increases opacity on hover */
    border-color: rgba(255, 255, 255, 0.25);
}
```

### ✅ Solution B: Custom Blur Shader (Advanced)

Create a custom shader for UI Toolkit elements:

```glsl
// Assets/Shaders/UIBlur.shader
Shader "UI/GlassBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 2
        _BlurIntensity ("Blur Intensity", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        
        CGPROGRAM
        #pragma surface surf Standard alpha
        
        sampler2D _MainTex;
        float _BlurSize;
        float _BlurIntensity;
        
        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 blur = IN.screenPos.xy / IN.screenPos.w;
            float3 col = tex2D(_MainTex, IN.uv_MainTex).rgb;
            
            // Simple blur simulation
            col += tex2D(_MainTex, IN.uv_MainTex + float2(_BlurSize, 0) * 0.001).rgb;
            col += tex2D(_MainTex, IN.uv_MainTex - float2(_BlurSize, 0) * 0.001).rgb;
            col /= 3.0;
            
            o.Albedo = col;
            o.Alpha = 1.0 - _BlurIntensity;
        }
        ENDCG
    }
    Fallback "Transparent/VertexLit"
}
```

Apply in C#:

```csharp
var panelElement = root.Q<VisualElement>("glass-panel");
var panelMaterial = new Material(Shader.Find("UI/GlassBlur"));
panelMaterial.SetFloat("_BlurSize", 5f);
panelMaterial.SetFloat("_BlurIntensity", 0.3f);

// Note: UI Toolkit materials require custom rendering setup
// This is complex and requires panel masking—use Solution A instead
```

### ✅ Solution C: Rendered Texture Background (Performance-Conscious)

Pre-render blurred backgrounds as textures:

```csharp
public class BlurredBackgroundGenerator : MonoBehaviour
{
    public RenderTexture blurredTexture;
    public int blurPasses = 4;
    
    public void GenerateBlurredBackground(Texture source)
    {
        Graphics.Blit(source, blurredTexture);
        
        for (int i = 0; i < blurPasses; i++)
        {
            Graphics.Blit(blurredTexture, blurredTexture, blurMaterial);
        }
    }
}
```

### Best Practice for Awwwards Aesthetic

```uss
/* Dark glassmorphic design without blur */
:root {
    --color-glass-dark: rgba(15, 15, 25, 0.3);
    --color-glass-medium: rgba(25, 25, 40, 0.5);
    --color-glass-light: rgba(40, 40, 60, 0.7);
    --glass-border: rgba(255, 255, 255, 0.1);
    --glass-border-hover: rgba(255, 255, 255, 0.2);
}

.glass-panel {
    background-color: var(--color-glass-dark);
    border: 1px solid var(--glass-border);
    border-radius: 16px;
    padding: 20px;
    transition: all 0.3s ease-out;
    backdrop-filter: none; /* Acknowledge limitation */
}

.glass-panel:hover {
    background-color: var(--color-glass-medium);
    border-color: var(--glass-border-hover);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}
```

---

## 5. EventSystem for Mixed UI Toolkit + 3D

### Scenario A: UI Toolkit Only (Recommended for WebGL)

**✅ Works out-of-the-box. No EventSystem needed.**

```csharp
public class DroneVisualizerUI : MonoBehaviour
{
    private VisualElement root;
    
    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // UI Toolkit handles input automatically
        var button = root.Q<Button>("connect-btn");
        button.clicked += OnConnectClicked;  // Native event system
    }
    
    void OnConnectClicked()
    {
        Debug.Log("Drone connect button clicked!");
    }
}
```

**In Editor**: No extra setup needed. Panel Settings handles rendering.

### Scenario B: Mixed UI Toolkit + 3D Objects + uGUI

**When you add uGUI EventSystem, UI Toolkit automatically adapts:**

```csharp
public class MixedUISetup : MonoBehaviour
{
    void Start()
    {
        // Add EventSystem for uGUI compatibility
        EventSystem eventSystem = gameObject.AddComponent<EventSystem>();
        gameObject.AddComponent<StandaloneInputModule>();
        
        // UI Toolkit creates PanelRaycaster & PanelEventHandler automatically
        // Both systems can coexist peacefully
    }
}
```

**Input handling for 3D objects + UI Toolkit**:

```csharp
public class DroneInteraction : MonoBehaviour
{
    private Camera mainCamera;
    private UIDocument uiDocument;
    
    void Start()
    {
        mainCamera = Camera.main;
        uiDocument = GetComponent<UIDocument>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if click is on UI first
            if (IsPointerOverUI())
            {
                Debug.Log("Clicked on UI");
                return;
            }
            
            // Raycast to 3D objects
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Clicked on {hit.collider.gameObject.name}");
            }
        }
    }
    
    bool IsPointerOverUI()
    {
        // UI Toolkit provides built-in hover detection
        PointerMoveEvent pointerEvent = (PointerMoveEvent)Event.current;
        return pointerEvent != null && pointerEvent.target != null;
        
        // Alternative: Check if root element is under pointer
        // var root = uiDocument.rootVisualElement;
        // return root.worldBound.Contains(Input.mousePosition);
    }
}
```

### Best Practice for WebGL Drone App

```csharp
// Use UI Toolkit's native event system (no EventSystem GameObject)
public class DroneUIManager : MonoBehaviour
{
    public void SetupUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // Register callbacks
        root.Q<Button>("power-btn").clicked += ToggleDronePower;
        root.Q<Slider>("altitude-slider").RegisterValueChangedCallback(OnAltitudeChange);
        root.Q<DropdownField>("flight-mode").RegisterValueChangedCallback(OnFlightModeChange);
        
        // Handle pointer events for hover effects
        root.Q<VisualElement>("drone-card").RegisterCallback<PointerEnterEvent>(OnCardHover);
        root.Q<VisualElement>("drone-card").RegisterCallback<PointerLeaveEvent>(OnCardLeave);
    }
    
    void OnCardHover(PointerEnterEvent evt) => Debug.Log("Card hovered");
    void OnCardLeave(PointerLeaveEvent evt) => Debug.Log("Card left");
    void ToggleDronePower() => Debug.Log("Power toggled");
    void OnAltitudeChange(ChangeEvent<float> evt) => Debug.Log($"Altitude: {evt.newValue}");
    void OnFlightModeChange(ChangeEvent<string> evt) => Debug.Log($"Mode: {evt.newValue}");
}
```

---

## 6. UI Toolkit Over 3D Scene (Overlay)

### Default Behavior (Automatic)

UI Toolkit **automatically renders on top** of 3D scenes in Play Mode.

```csharp
public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Camera renders 3D scene
        var cam3D = gameObject.AddComponent<Camera>();
        cam3D.clearFlags = CameraClearFlags.SolidColor;
        cam3D.backgroundColor = Color.black;
        cam3D.depth = 0;  // Render first
        
        // UI Document renders on top automatically (depth = 1 by default)
        var uiDocument = GetComponent<UIDocument>();
        // No manual depth configuration needed
    }
}
```

### Manual Sorting (If Needed)

```csharp
// UIDocument has built-in Panel Settings for sorting
var panelSettings = GetComponent<UIDocument>().panelSettings;
panelSettings.sortingOrder = 100;  // Render order (higher = on top)
```

### Responsive Layout for Different Screen Sizes

```uss
:root {
    --width-desktop: 1920px;
    --width-tablet: 1024px;
    --width-mobile: 768px;
}

/* Desktop layout */
.dashboard {
    width: 100%;
    height: 100%;
    flex-direction: row;
}

.sidebar {
    width: 300px;
    overflow-y: auto;
}

.viewport {
    flex: 1;
}

/* Tablet layout */
@media (max-width: 1024px) {
    .dashboard {
        flex-direction: column;
    }
    
    .sidebar {
        width: 100%;
        height: auto;
        max-height: 200px;
    }
}

/* Mobile layout */
@media (max-width: 768px) {
    .dashboard {
        padding: 10px;
    }
    
    .sidebar {
        display: none;  /* Hide on mobile */
    }
    
    .viewport {
        flex: 1;
    }
}
```

---

## 7. WebGL Build Considerations

### Platform-Specific Notes (2025)

| Feature | WebGL Status | Notes |
|---------|--------------|-------|
| Basic UI | ✅ Full Support | Fully functional |
| Transitions | ✅ Full Support | Smooth 60fps |
| Custom Fonts | ✅ Full Support | Embedded in build |
| Vector Graphics | ✅ Full Support | SVG import works |
| Shaders | ⚠️ Limited | Some features unavailable |
| Blur Effects | ❌ Not Supported | Use semi-transparent layers |
| iOS WebGL | ✅ Fixed (2024) | Now fully supported |
| Android WebGL | ✅ Full Support | Good performance |

### Responsive Design Pattern for WebGL

```csharp
public class ResponsiveUIManager : MonoBehaviour
{
    private VisualElement root;
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Handle window resize
        root.RegisterCallback<GeometryChangedEvent>(OnWindowResized);
    }
    
    void OnWindowResized(GeometryChangedEvent evt)
    {
        float width = evt.newRect.width;
        float height = evt.newRect.height;
        
        Debug.Log($"Window resized to {width}x{height}");
        
        // Apply responsive classes
        if (width < 768)
            root.AddToClassList("mobile");
        else if (width < 1024)
            root.AddToClassList("tablet");
        else
            root.AddToClassList("desktop");
    }
}
```

### Build Size Optimization

```
UI Toolkit in WebGL build:
- Core UI Toolkit: ~300-400KB
- Custom Fonts (4-6): +150-300KB
- Vector Graphics (SVGs): +50-100KB per asset
- Total UI overhead: ~600-800KB for typical app
```

**Optimization Tips**:
- Use text-only labels instead of textures
- Batch vector graphics into single SVG
- Compress font files (subset characters if possible)
- Use CSS variables for theme colors (no texture overhead)

### Known Limitations & Workarounds

```csharp
// Limitation 1: No browser localStorage in WebGL
// ❌ THIS THROWS SECURITYERROR
// PlayerPrefs.SetString("key", "value");

// ✅ Use in-memory state instead
private Dictionary<string, object> uiState = new();
uiState["drone_connected"] = true;

// Limitation 2: No network requests from UI callbacks
// ❌ DON'T do heavy networking in UI events
void OnButtonClick()
{
    // Avoid UnityWebRequest here
}

// ✅ Queue requests and handle asynchronously
async void OnButtonClickAsync()
{
    var request = UnityWebRequest.Get("https://api.example.com/data");
    await request.SendWebRequest();
}

// Limitation 3: Performance with very large text
// ❌ Avoid rendering massive text blocks
.large-text {
    font-size: 100px;  // CPU-intensive to update frequently
}

// ✅ Use reasonable sizes
.heading {
    font-size: 24px;
}
```

### Performance Testing in WebGL

```csharp
public class UIPerformanceMonitor : MonoBehaviour
{
    private Profiler profiler;
    
    void Update()
    {
        // Monitor UI rendering cost
        long uiMemory = Profiler.GetMonoUsedSizeLong();
        Debug.Log($"UI Memory: {uiMemory / 1024 / 1024}MB");
        
        // Check frame time
        float frameTime = Time.deltaTime * 1000f;
        if (frameTime > 16.67f)  // 60fps target
            Debug.LogWarning($"Frame drop: {frameTime}ms");
    }
}
```

---

## Official Documentation Links

1. **UI Toolkit Main**: https://docs.unity3d.com/6000.3/Documentation/Manual/UIElements.html
2. **USS Transitions**: https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-Transitions.html
3. **Vector Graphics**: https://docs.unity3d.com/6000.5/Documentation/Manual/ui-systems/work-with-vector-graphics.html
4. **Event System**: https://docs.unity3d.com/6000.2/Documentation/Manual/UIE-Runtime-Event-System.html
5. **Best Practices**: https://docs.unity3d.com/6000.4/Documentation/Manual/UIE-best-practices-for-managing-elements.html
6. **Panel Settings**: https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-PanelSettings.html

---

## Community Resources

- **Angry Shark Studio 2025 Guide**: https://www.angry-shark-studio.com/blog/unity-ui-toolkit-vs-ugui-2025-guide/
- **Rangoric.com Font Setup**: https://www.rangoric.com/blog/custom-fonts-with-unity-3d/
- **Reddit: UI Toolkit WebGL**: https://www.reddit.com/r/Unity3D/comments/1hliiob/is_ui_toolkit_ready_for_production_web/
- **Unity Forum**: https://forum.unity.com/forums/ui-toolkit.103/

---

## Complete Drone Visualization Example

```csharp
// Assets/Scripts/DroneUIController.cs
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DroneUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement root;
    private Button connectButton;
    private Slider altitudeSlider;
    private DropdownField flightModeDropdown;
    private Label statusLabel;
    
    void Start()
    {
        root = uiDocument.rootVisualElement;
        InitializeUI();
    }
    
    void InitializeUI()
    {
        connectButton = root.Q<Button>("connect-btn");
        altitudeSlider = root.Q<Slider>("altitude-slider");
        flightModeDropdown = root.Q<DropdownField>("flight-mode");
        statusLabel = root.Q<Label>("status-label");
        
        // Register event callbacks
        connectButton.clicked += OnConnect;
        altitudeSlider.RegisterValueChangedCallback(OnAltitudeChanged);
        flightModeDropdown.RegisterValueChangedCallback(OnFlightModeChanged);
        
        // Hover animations
        root.Q<VisualElement>("drone-panel").RegisterCallback<PointerEnterEvent>(
            _ => AnimatePanel(true));
        root.Q<VisualElement>("drone-panel").RegisterCallback<PointerLeaveEvent>(
            _ => AnimatePanel(false));
    }
    
    void OnConnect()
    {
        statusLabel.text = "Connecting...";
        statusLabel.style.opacity = 0.8f;
        connectButton.style.opacity = 0.6f;
    }
    
    void OnAltitudeChanged(ChangeEvent<float> evt)
    {
        Debug.Log($"Altitude: {evt.newValue}m");
    }
    
    void OnFlightModeChanged(ChangeEvent<string> evt)
    {
        Debug.Log($"Flight Mode: {evt.newValue}");
    }
    
    void AnimatePanel(bool isHovering)
    {
        var panel = root.Q<VisualElement>("drone-panel");
        panel.style.scale = isHovering ? new Scale(new Vector2(1.02f, 1.02f)) : new Scale(Vector2.one);
    }
}
```

---

**Last Updated**: December 2025  
**Compatible**: Unity 6 (2023.3+) | WebGL | All Platforms
