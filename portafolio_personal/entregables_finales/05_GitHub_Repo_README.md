# Holybro X500 V2: WebGL Technical Viewer & Pipeline Automation

![Hero Image](assets_fuente/heroes/drone_hero_placeholder.jpg)

## 📌 Project Overview
An interactive WebGL technical viewer built in Unity, solving the pipeline translation from heavy industrial CAD assemblies into performant, browser-based inspection experiences. 

This repository houses the final WebGL client, the custom HLSL shaders for technical rendering, and the C# Editor Tooling required to automate the pipeline.

**Core Focus:** Technical Art, Render Optimization (WebGL), HLSL Shaders, and Editor Automation.

## 🚀 Key Features

### 1. The Interactive Viewer (Runtime)
*   **Contextual Inspection:** Selecting parts retrieves metadata and isolates geometry via `SelectionManager` and `UIDetailsSheet`.
*   **Procedural Fastener Instancing:** A custom metadata-driven system (`FastenerRegistry`) that leaves thousands of screws as lightweight proxies, instancing dense geometry *only* upon user selection to save Gb of VRAM.
*   **Runtime Importer Repair:** `ImportedDroneRuntimeBinder.cs` programmatically repairs broken CAD hierarchies on the fly, rebuilding arrays for visibility layers without manual intervention.

### 2. Applied Technical Rendering (HLSL)
Custom rendering passes built for the Universal Render Pipeline (URP):
*   **Hybrid Thermal Shader:** Maps operational states from `DroneStateController` to heuristic visual heat gradients.
*   **Interactive X-Ray:** Stencil-based inner-mechanism visualization.
*   **PBR Cross-Section Clipping:** High-performance clipping planes retaining PBR lighting responses.

### 3. Editor Tooling & Automation (C#)
Custom tools built to prevent human error and automate QA:
*   `ProjectSetupWizard`: One-click raw import normalization.
*   `ImportedDroneCoverageAudit`: A programmatic unit-tester for scene health that runs before hitting *Play*.

## 🛠️ Tech Stack
*   **Engine:** Unity (URP, WebGL)
*   **Language:** C#, HLSL
*   **UI/UX:** Unity UI Toolkit
*   **Asset Pipeline:** Blender, Python (Automation scripts)

## 📁 Repository Structure
*   `/desarrollo/unity_project/Assets/Scripts/`: Core C# Runtime systems.
*   `/desarrollo/unity_project/Assets/Editor/`: Custom Inspector windows and pipeline tools.
*   `/desarrollo/unity_project/Assets/Shaders/`: HLSL Shader code for X-Ray, Thermal, and Clipping.
*   `/herramientas_fuente/`: Standalone Python scripts for Blender pipeline normalization.

---

> **Looking for the visual deep-dive?** View the complete breakdown of Shaders, Tools, and the WebGL optimization process on my [ArtStation Profile].
