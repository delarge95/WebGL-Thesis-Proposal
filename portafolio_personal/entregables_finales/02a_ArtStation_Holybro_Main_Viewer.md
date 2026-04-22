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

### PERFORMANCE & WEBGL CONSTRAINTS (THE FASTENER SUBSYSTEM)
Targeting WebGL (and mobile fallback) meant strict constraints. The Unity Manual specifically states that CPU-side dispatch of WebGL operations is a major bottleneck on main threads. 
Small details like screws and fasteners usually dominate draw calls and kill WebGL stability. I developed a system where fasteners are procedural instances driven by metadata. They pull dense geometry *only* on-demand when user-selected, saving gigabytes of processing while resting. 

By aggressively implementing dynamic batching mitigations and metadata instances, I reduced overall draw calls from [X,XXX] down to [YYY], hitting a solid [60 FPS] average across standard enterprise laptops integrated graphics.
Furthermore, testing against Safari and strict browser heap limits led me to develop rendering downgrades and custom handling to avoid total Context Loss memory leaks.

---
**SOFTWARE:** Unity 6 (WebGL, URP, UI Toolkit), C#, Blender.
**TAGS:** #TechnicalArt #Unity3D #WebGL #Optimization #Interactive
*(📌 Technical Deep Dive: Find the pseudo-code for my Adaptive Camera Math & WebGL mitigations at [Link to Tech Cheat Sheet repo/gist])*
