# ArtStation Post 1: Interactive WebGL Technical Viewer (The Masterpiece)

*(Este será tu proyecto principal en ArtStation. Atrae a los Hiring Managers de producto y optimización).*

### THE PROBLEM
How do you translate a dense, heavy industrial CAD model into an interactive, performant WebGL experience without losing technical legibility? 

I built this interactive WebGL technical viewer for the Holybro X500 V2 drone to answer that question. 

### THE REAL PRODUCT FLOW
The viewer revolves around a cohesive interaction loop:
`Hero -> Explore -> Select Part -> Bottom Sheet Data -> Inspect / Analyze / Studio`

### PROCEDURAL UI ONBOARDING (NO VIDEOS ALLOWED)
To guide the user without loading heavy video files into the WebGL build, I developed an internal `OnboardingAnimationView`. It doesn't play GIFs; it renders procedural demonstrations of the interactions natively in the browser using Unity's `Painter2D` API. This keeps the footprint tiny and ensures the UI scales perfectly on mobile and desktop.

### CONTEXT-AWARE CAMERA MATH
A major UX flaw in 3D viewers is camera behavior scale. Scrolling the mouse wheel on a drone should behave differently than scrolling on a 3mm screw. I wrote a custom `OrbitCameraController` that recalculates its focus bounds, pan curves, and orbit sensitivity in real-time based on the exact geometric volume of the currently isolated part. If you isolate a tiny fastener, the camera dynamically creates a micro-interaction volume explicitly for it.

### PERFORMANCE & OPTIMIZATION (THE FASTENER SUBSYSTEM)
Targeting WebGL (and mobile fallback) meant strict constraints. 
Small details like screws and fasteners usually dominate draw calls and kill WebGL performance. I developed a system where fasteners are procedural instances driven by metadata. They pull dense geometry *only* on-demand when user-selected, saving gigabytes of processing while resting.

### RUNTIME REPAIR
Importing 257 renderers directly from CAD is a mess. I created an `ImportedDroneRuntimeBinder` (C#) that repairs the imported hierarchy in runtime—rebuilding caches for selection, explode operations, and visual clipping on the fly.

---
**SOFTWARE:** Unity (WebGL, URP, UI Toolkit), C#, Blender.
**TAGS:** #TechnicalArt #Unity3D #WebGL #Optimization #Interactive
*(Nota: Añade un link al final invitando a ver los posts de Shaders y Herramientas que estarán en tu mismo perfil).*
