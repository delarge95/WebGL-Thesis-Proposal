# Technical Walkthrough: The Math Behind the Drone Viewer

> Este documento es un "Mini-Readme" / Blog Post diseñado para adjuntarse en ArtStation, GitHub y LinkedIn. Sirve como prueba técnica irrefutable para reclutadores Senior de que entiendes la matemática de lo que implementaste.

---

A complex Technical Viewer that runs on constraints (WebGL) cannot rely on brute force. Below are the pseudo-formulations and system designs I used to solve three major usability and rendering challenges for the Holybro X500 V2 Viewer.

## 1. Context-Aware Camera Math (The Anti-Jitter Orbit)

**The Problem:** Standard orbit cameras feel perfect when observing a 50cm drone but break down entirely when inspecting a 3mm fastener. The zoom step becomes too large, panning loses extreme precision, and rotation speed is dizzying.

**The Solution:** An adaptive `OrbitCameraController` that derives its mathematical bounds not from a fixed configuration, but from the live volumetric bounds of the isolated geometry.

**Pseudo-Logic:**
```csharp
// 1. We acquire the actual world-space bounding box of the selected component.
Bounds targetBounds = CalculateTotalBounds(activeSelection);

// 2. We extract the primary contextual scale radius.
float contextScale = targetBounds.extents.magnitude;

// 3. We dynamically shift camera constraints based on this scale.
// A standard target might be ~0.25 (25cm) -> Distance = 0.5m
// A fastener might be ~0.003 (3mm) -> Distance = 0.015m
float minDistanceTarget = contextScale * 1.5f;
float maxDistanceTarget = Mathf.Clamp(contextScale * 10f, 0.1f, globalMaxDistance);

// 4. PAN AND ORBIT CURVES (The Secret to UX)
// Linear clamping is too robotic. Instead, I apply non-linear exponential curves 
// so that when scale is tiny, panning movement drops aggressively while orbit 
// retains enough angular speed to not feel sluggish.
float dynamicPanSensitivity = basePanSens * Mathf.Pow(contextScale, 1.25f);
float dynamicOrbitSensitivity = baseOrbitSens * Mathf.Pow(contextScale, 0.7f);

// 5. Temporal Interpolation
// We use SmoothDamp to prevent nausea when zooming out from a 3mm screw 
// back to the 50cm body array, mitigating jitter during translation.
```

## 2. Zero-Memory Procedural UI Onboarding (Painter2D)

**The Problem:** The app requires an animated tutorial. Video loops or GIF spritesheets consume heavy WebGL memory buffers and look artifacted on high-DPI screens.

**The Solution:** Eliminate raster sequences entirely. I constructed an `OnboardingAnimationView` rendering directly to the UI Canvas using Unity's `Painter2D` runtime API.

**How it works structurally:**
*   A background daemon iterates through keyframes mapped in code.
*   It calculates quadratic bezier curves for the "virtual cursor".
*   On interaction triggers, `Painter2D` mathematically draws concentric circles with increasing alpha drop-off to simulate ripples.
*   **Result:** A hyper-crisp, infinitely scalable UI onboarding sequence that inherently respects the target platform's aspect ratio without loading a single megabyte into the VRAM texture pool.

## 3. The Thermal Nodes Logic (Heuristics without the FEA Overhead)

**The Problem:** Solving Finite Element Analysis (FEA) thermodynamics on a WebGL CPU-thread will kill the application. However, a static unlinked baked texture fails to relate to operational state changes.

**The Solution:** A Hybrid Heuristic Node System.

*   Every functional component is registered as a node. Nodes have specific inherent weights ($w_s$), exposure values ($e$), cooling rates ($k_c$) and default specific equilibrium bases tied to simulated system load.
*   **Newton's Law of Cooling Hybridized:**
    At every tick (governed by the `ThermalSimulationManager` running at a clamped ~20Hz decoupled from the frame renderer), the temperature ($T$) updates based on:
    1.  **Heat Injection (Load vs Equilibrium)**: $\Delta T_{source} = (T_{eq} - T) \cdot (1 - e^{-\Delta t / \tau}) \cdot w_s$ 
    2.  **Environment Dissipation**: $\Delta T_{cooling} = (T_{ambient} - T) \cdot k_c \cdot e \cdot \Delta t$
    3.  **Local Conduction (Thermal Graph Links)**: Summation of heat from linked neighbor nodes weighted by empirical conductance scaling $G_{eff}$.
*   **Rendering:** The computed abstract temperatures are normalized into a 0.0 - 1.0 clamped range and sent to an HLSL `MaterialPropertyBlock`.
*   **Why it Matters:** This avoids massive per-vertex CPU iterations while visually mimicking thermal stress logic depending on whether the drone state is set to `IDLE` or `FLYING`, retaining total instancing batch possibilities since the updates are pure block constants.
