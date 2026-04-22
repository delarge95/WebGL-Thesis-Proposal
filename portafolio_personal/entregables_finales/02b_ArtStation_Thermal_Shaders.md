# ArtStation Post 2: Applied Technical Rendering & HLSL Shaders

*(Este proyecto muestra tus habilidades como Shader Coder y solucionador matemático. Atrae a Lead Tech Artists y equipos de R&D).*

### THE CHALLENGE
Photorealism is standard, but *functional* rendering is a different beast. For the Holybro X500 V2 Interactive Viewer, the goal was to create shaders that allowed engineers and users to "read" the drone's internal structure and operational telemetry natively in a WebGL browser.

### HIGHLIGHT 1: THE HYBRID THERMAL SHADER
This isn't a pre-rendered ambient occlusion map tinted red. It is a live, heuristic thermal visualization.
*   **The Logic:** A `DroneStateController` (C#) feeds operational load variables to a `ThermalSimulationManager`.
*   **The Render:** The data is passed to a custom HLSL shader that maps nodal heat transfer across the mesh, visually representing telemetry via a dynamic heatmap gradient, paired with an interactive UI legend.

*[Asset: GIF showing standard drone turning into Thermal view, with UI legend updating]*

### HIGHLIGHT 2: INTERACTIVE X-RAY
A technical viewer must allow internal inspection. Using custom Stencil buffer operations and URP rendering layers, I built an X-Ray shader that highlights occluded internal geometry (like wiring and inner motors) without creating visual noise or z-fighting.

*[Asset: Comparison Slider / GIF of X-Ray]*

### HIGHLIGHT 3: PBR CLIPPING & CROSS-SECTIONS
To inspect the internal assembly, I wrote a performant world-space clipping plane shader. Unlike standard clipping shaders, this maintains the PBR lighting responses on the cross-section faces without decimating framerates in WebGL.

---
**SOFTWARE:** Unity (URP, Shader Graph), HLSL, C#.
**TAGS:** #Shaders #HLSL #TechnicalArt #Rendering #Unity3D #VFX
