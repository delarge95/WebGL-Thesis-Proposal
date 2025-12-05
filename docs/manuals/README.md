# WebGL Drone Visualization Prototype

> Interactive 3D web prototype for technical visualization of high-performance drones using Unity WebGL, optimized graphics pipelines, and WebAssembly.

[![Unity](https://img.shields.io/badge/Unity-6.0%20LTS-black?logo=unity)](https://unity.com/)
[![WebGL](https://img.shields.io/badge/WebGL-2.0-blue)](https://www.khronos.org/webgl/)
[![C#](https://img.shields.io/badge/C%23-11.0-purple)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Academic-green)](LICENSE)

## 📋 Overview

This project implements an interactive 3D web viewer for drone hardware visualization, featuring:

- **7 View Modes**: Realistic, X-Ray, Blueprint, Solid Color, Wireframe, Ghosted, Thermal
- **Exploded View**: Animated component separation with slider control
- **Part Selection**: Click-to-select with detailed information panels
- **Cross-Section**: Dynamic cutting planes on X/Y/Z axes
- **Engineer Tools**: Measurement, annotations, assembly guides, BOM export
- **Drone Simulation**: On/Off states with propeller animations and status lights

## 🏗️ Project Structure

```
WebGL_tesis/
├── desarrollo/
│   ├── unity_project/
│   │   └── Assets/
│   │       ├── Scripts/
│   │       │   ├── Core/
│   │       │   │   ├── Content/      # ExplodablePart, components
│   │       │   │   ├── Data/         # ScriptableObjects (DronePartData)
│   │       │   │   ├── Events/       # EventBus system
│   │       │   │   ├── Managers/     # 18+ singleton managers
│   │       │   │   └── Utils/        # Helpers (Singleton, TweenEngine)
│   │       │   └── UI/               # UI Toolkit components
│   │       └── Shaders/              # 7 custom HLSL shaders
│   ├── blender_assets/               # 3D models (FBX, Blender)
│   └── docs/                         # Technical documentation
├── Propuesta/                        # Thesis proposal (LaTeX)
├── Informe_final/                    # Final report
│   ├── Manual_tecnico/               # Technical manual
│   └── Manual_de_usuario/            # User manual
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- **Unity 6.0 LTS** or higher
- **Universal Render Pipeline (URP)**
- Modern web browser with WebGL 2.0 support

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/[username]/WebGL_tesis.git
   cd WebGL_tesis
   ```

2. **Open in Unity Hub**
   - Add `desarrollo/unity_project` as a project
   - Unity will import all scripts and shaders

3. **Quick Setup (Unity Editor)**
   - Go to menu: `WebGL > Create Scene Structure`
   - This creates all required GameObjects and managers

4. **Import Your Drone Model**
   - Place FBX file in `Assets/Models/`
   - Parent under `Drone_Root` in the scene
   - Add `ExplodablePart` component to each part
   - Create `DronePartData` assets for each part

### Building for WebGL

1. **File > Build Settings > WebGL**
2. **Player Settings**:
   - Compression: Brotli (production) or Disabled (development)
   - Code Stripping: High
3. **Build and Run**

## 🎮 Controls

| Action | Mouse/Keyboard | Touch |
|--------|---------------|-------|
| Rotate | Left Click + Drag | One Finger + Drag |
| Pan | Right Click + Drag | Two Fingers + Drag |
| Zoom | Scroll Wheel | Pinch |
| Select Part | Left Click | Tap |
| Reset Camera | R | Reset Button |
| Toggle Exploded | E | Explode Button |
| View Presets | 1-6 | Toolbar |

## 🧩 Architecture

### Design Patterns

- **Singleton/PersistentSingleton**: Global managers
- **Event Bus (Pub/Sub)**: Decoupled communication
- **State Machine**: Application flow control
- **ScriptableObjects**: Data-driven configuration

### Core Systems

| System | Description |
|--------|-------------|
| `AppStateMachine` | Controls app states (Loading, Exploration, ExplodedView, etc.) |
| `EventBus` | Publish/Subscribe event system |
| `SelectionManager` | Raycast selection with highlighting |
| `ViewModeManager` | 7 visualization modes with custom shaders |
| `ExplodedViewManager` | Animated part separation |
| `AudioManager` | Sound effects and music control |

### Engineer Tools

| Tool | Description |
|------|-------------|
| `AssemblyGuideManager` | Step-by-step assembly instructions |
| `MeasurementTool` | Distance, angle, and radius measurement |
| `ConnectionPointsViewer` | Visualize screws, snaps, and connections |
| `BillOfMaterialsManager` | Generate and export BOM as CSV |
| `AnnotationSystem` | Add 3D notes with billboard text |
| `AssemblyChecklist` | Track part verification |

## 🎨 Shaders

All shaders are URP-compatible and WebGL 2.0 optimized:

| Shader | Features |
|--------|----------|
| `ClippableLit.shader` | PBR with cross-section support |
| `XRay.shader` | Fresnel transparency, dual-pass |
| `Blueprint.shader` | Technical grid, edge detection |
| `Thermal.shader` | Animated heat gradient |
| `Wireframe.shader` | Geometry shader with fallback |
| `SolidColor.shader` | Blinn-Phong with outline |
| `Ghosted.shader` | Fresnel alpha with depth fade |

## 📊 Technical Specifications

| Metric | Target | Notes |
|--------|--------|-------|
| Polygon Budget | < 100,000 | Per-scene limit |
| FPS (Mobile) | > 30 | Mid-range devices |
| Initial Load | < 3s | Shell/loader |
| Full Load | < 10s | Complete model |
| Draw Calls | < 50 | With batching |

## 📁 Key Files

### Scripts (~70 files, ~10,000 lines)

```
Core/Managers/
├── AppStateMachine.cs        # State machine
├── SelectionManager.cs       # Part selection
├── ExplodedViewManager.cs    # Exploded view
├── ViewModeManager.cs        # 7 view modes
├── PartCatalogManager.cs     # Search & filter
├── CrossSectionManager.cs    # Cutting planes
├── DroneStateController.cs   # On/Off simulation
├── AssemblyGuideManager.cs   # Step-by-step guide
├── MeasurementTool.cs        # 3D measurements
└── ... (18+ managers)

Core/Utils/
├── Singleton.cs              # Generic singleton
├── TweenEngine.cs            # Animation engine
├── EventBus.cs               # Event system
├── SceneBootstrapper.cs      # Auto-setup
└── ProjectSetupWizard.cs     # Editor menu
```

## 🧪 Testing

```bash
# Open Unity and run in Editor
# Or build WebGL and test in browser

# Keyboard shortcuts for testing:
# F3 - Show performance metrics
# F12 - Take screenshot
```

## 📄 Documentation

- **Technical Manual**: `Informe_final/Manual_tecnico/`
- **User Manual**: `Informe_final/Manual_de_usuario/`
- **Thesis Proposal**: `Propuesta/`

## 🙏 Acknowledgments

- Universidad Nacional Abierta y a Distancia (UNAD)
- Advisor: Deivid Enrique Triviño Lozada
- Unity Technologies for the URP pipeline

## 📝 License

This project is part of an academic thesis. All rights reserved.

---

**Author**: Alexander Woodcock Salomón  
**Program**: Multimedia Engineering  
**Year**: 2025
