# Holybro X500 V2: Technical WebGL Viewer for Interactive Visualization

*(Borrador final listo para copiar y pegar en la descripción de tu proyecto de ArtStation)*

### THE PROBLEM
How do you translate a dense, heavy industrial CAD model into an interactive, performant WebGL experience without losing technical legibility? 

I built an interactive WebGL technical viewer for the Holybro X500 V2 drone to answer that question. This isn't just a 3D model placed in a basic scene; it is a full interactive ecosystem connecting runtime import repair, technical visualization modes, applied thermal rendering, and editor tooling.

### THE REAL PRODUCT FLOW
The viewer revolves around a cohesive interaction loop:
`Hero -> Explore -> Select Part -> Bottom Sheet Data -> Inspect / Analyze / Studio`

Every element answers a specific user need:
*   **Hero:** Shows the product as it exists in reality.
*   **Selection & Data:** Bridges raw 3D geometry with meaningful context.
*   **View Modes (Studio):** Organizes specific visual tools to “read” the assembly structurally.

### TECHNICAL VISUALIZATION SHADERS (HLSL)
The purpose was to create tools, not just cosmetics. I developed custom Universal Render Pipeline (URP) shaders to allow structural and thermal inspection directly in the browser:
*   **X-Ray:** For internal structural reading.
*   **Solid Color / Technical Form:** Removing visual noise for pure form analysis.
*   **Hybrid Thermal:** A heuristic simulation translating operational states (via a `DroneStateController` and `ThermalSimulationManager`) into clear visual gradients and an interactive UI legend.

*[Asset: Grid comparing Realistic, X-Ray, Solid, Thermal]*

### RUNTIME REPAIR & ARCHITECTURE
Maintaining consistency between the drone's semantic structure and Unity's technical hierarchy was the core challenge.
The project operates on three layers: 28 canonical research parts, 30 scene anchors, and 257 renderers/colliders. To bridge this, I created an `ImportedDroneRuntimeBinder` (C#) that repairs the imported hierarchy in runtime—rebuilding caches for selection, explode operations, and visual clipping.

### EDITOR TOOLING (C#)
To ensure this wasn't just a brittle visual demo, I developed an internal toolset to manage the project pipeline reliably:
*   `ProjectSetupWizard`: Automating the tedious import pipeline.
*   `ImportedDroneCoverageAudit`: Validating scene health programmatically.
*   `ThermalContactGraphBuilderWindow`: A visual tool to define heat transfer logic across the mesh.

*[Asset: Screenshots of Editor Windows]*

### PERFORMANCE & OPTIMIZATION TACTICS
Targeting WebGL (and mobile fallback) meant strict constraints. 
*   **Fastener Instancing Subsystem:** Small details dominate draw calls. The viewer solves fasteners via metadata instances, pulling procedural detail *only* on-demand when user-selected, saving gigahertz of processing while resting.
*   **CAD Normalization:** Focused on semantic merging over blind decimation.

### WHY IT MATTERS
This project encapsulates my core focus as a Technical Artist: bridging complex technical systems with clear, optimized interactive experiences. Connecting runtime tools, shading logic, and honest optimization within strict platform constraints.

---

**SOFTWARE:** Unity (URP, UI Toolkit), C#, HLSL, Blender, Substance.
**TAGS:** #TechnicalArt #Unity3D #WebGL #ShaderGraph #HLSL #Optimization #Interactive #Tools
