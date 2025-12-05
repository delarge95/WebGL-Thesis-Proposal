# WebGL Drone Viewer - System Architecture

This document describes the technical architecture of the WebGL Drone Viewer application.

## High-Level Architecture

```mermaid
graph TB
    subgraph "User Interface Layer"
        UI[UI Toolkit Components]
        USS[USS Stylesheets]
    end
    
    subgraph "Application Layer"
        ASM[AppStateMachine]
        GM[GameManager]
        EB[EventBus]
    end
    
    subgraph "Core Managers"
        SM[SelectionManager]
        VM[ViewModeManager]
        EM[ExplodedViewManager]
        CM[CrossSectionManager]
        PM[PartCatalogManager]
        DSC[DroneStateController]
    end
    
    subgraph "Engineer Tools"
        AGM[AssemblyGuideManager]
        MT[MeasurementTool]
        CPV[ConnectionPointsViewer]
        BOM[BillOfMaterialsManager]
        AS[AnnotationSystem]
        AC[AssemblyChecklist]
    end
    
    subgraph "Content Layer"
        EP[ExplodablePart]
        HS[HighlightSystem]
        DPD[DronePartData]
    end
    
    subgraph "Utilities"
        SG[Singleton/PersistentSingleton]
        TE[TweenEngine]
        OP[ObjectPooler]
    end
    
    UI --> ASM
    UI --> EB
    ASM --> GM
    EB --> SM
    EB --> VM
    EB --> EM
    SM --> EP
    SM --> HS
    EP --> DPD
    VM --> EP
    EM --> EP
    AGM --> DPD
    MT --> EP
    CPV --> EP
    SG --> GM
    SG --> SM
    TE --> EM
```

## Design Patterns

### 1. Singleton Pattern

```mermaid
classDiagram
    class Singleton~T~ {
        <<abstract>>
        -static T _instance
        +static T Instance
        #virtual void Awake()
    }
    
    class PersistentSingleton~T~ {
        <<abstract>>
        #override void Awake()
        +DontDestroyOnLoad()
    }
    
    class GameManager {
        +AppState CurrentState
        +SetState(state)
    }
    
    class SelectionManager {
        +Transform CurrentSelection
        +SelectObject(obj)
        +Deselect()
    }
    
    Singleton~T~ <|-- PersistentSingleton~T~
    PersistentSingleton~T~ <|-- GameManager
    Singleton~T~ <|-- SelectionManager
```

### 2. Event Bus Pattern

```mermaid
sequenceDiagram
    participant Publisher
    participant EventBus
    participant Subscriber1
    participant Subscriber2
    
    Subscriber1->>EventBus: Subscribe<PartSelectedEvent>
    Subscriber2->>EventBus: Subscribe<PartSelectedEvent>
    
    Publisher->>EventBus: Publish(PartSelectedEvent)
    
    EventBus->>Subscriber1: OnPartSelected(event)
    EventBus->>Subscriber2: OnPartSelected(event)
```

### 3. State Machine

```mermaid
stateDiagram-v2
    [*] --> Loading
    Loading --> Intro
    Intro --> Exploration
    
    Exploration --> ExplodedView: Toggle Explode
    ExplodedView --> Exploration: Toggle Explode
    
    Exploration --> FocusMode: Select Part
    FocusMode --> Exploration: Deselect
    
    Exploration --> Settings: Open Settings
    Settings --> Exploration: Close
    
    Exploration --> Menu: Open Menu
    Menu --> Exploration: Close
```

## Component Hierarchy

```mermaid
graph LR
    subgraph "Scene Hierarchy"
        Root[Scene Root]
        
        Root --> Managers[_GameManagers]
        Root --> UI[MainMenu_UI]
        Root --> Camera[CameraRig]
        Root --> Light[Lighting]
        Root --> Drone[Drone_Root]
        
        Managers --> GM2[GameManager]
        Managers --> SM2[SelectionManager]
        Managers --> AM[AudioManager]
        Managers --> VMM[ViewModeManager]
        Managers --> EMM[ExplodedViewManager]
        
        UI --> UID[UIDocument]
        UI --> VT[ViewModeToolbar]
        UI --> IP[InfoPanel]
        UI --> PC[PartCatalog]
        UI --> ET[EngineerToolbar]
        
        Camera --> MC[Main Camera]
        MC --> OCC[OrbitCameraController]
        MC --> CP[CameraPresets]
        
        Drone --> Body[Body]
        Drone --> Motors[Motors]
        Drone --> Props[Propellers]
        Drone --> Elec[Electronics]
    end
```

## Data Flow

```mermaid
flowchart TD
    subgraph "Input"
        Mouse[Mouse Input]
        Touch[Touch Input]
        Keys[Keyboard Input]
    end
    
    subgraph "Processing"
        IM[InputManager]
        SM3[SelectionManager]
        CAM[CameraController]
    end
    
    subgraph "State"
        ASM2[AppStateMachine]
        DPD2[DronePartData]
    end
    
    subgraph "Output"
        Render[Rendering]
        Audio[Audio]
        UIOUT[UI Update]
    end
    
    Mouse --> IM
    Touch --> IM
    Keys --> IM
    
    IM --> SM3
    IM --> CAM
    
    SM3 --> ASM2
    SM3 --> DPD2
    
    ASM2 --> Render
    ASM2 --> UIOUT
    DPD2 --> UIOUT
    SM3 --> Audio
```

## Shader Pipeline

```mermaid
graph LR
    subgraph "View Modes"
        R[Realistic/PBR]
        X[X-Ray]
        B[Blueprint]
        S[Solid Color]
        W[Wireframe]
        G[Ghosted]
        T[Thermal]
    end
    
    subgraph "Shader Files"
        CL[ClippableLit.shader]
        XS[XRay.shader]
        BS[Blueprint.shader]
        SS[SolidColor.shader]
        WS[Wireframe.shader]
        GS[Ghosted.shader]
        TS[Thermal.shader]
    end
    
    subgraph "Features"
        PBR[PBR Lighting]
        CLIP[Cross Section]
        FRES[Fresnel]
        GRID[Grid Pattern]
        GEO[Geometry Shader]
    end
    
    R --> CL --> PBR
    R --> CL --> CLIP
    X --> XS --> FRES
    B --> BS --> GRID
    B --> BS --> FRES
    S --> SS --> PBR
    W --> WS --> GEO
    G --> GS --> FRES
    T --> TS --> FRES
```

## Module Dependencies

```mermaid
graph BT
    subgraph "Foundation"
        Utils[Utils Layer]
        Events[Events Layer]
    end
    
    subgraph "Core"
        Data[Data Layer]
        Content[Content Layer]
        Managers[Managers Layer]
    end
    
    subgraph "Presentation"
        UI2[UI Layer]
        Shaders[Shaders]
    end
    
    Utils --> Events
    Events --> Data
    Events --> Managers
    Data --> Content
    Data --> Managers
    Content --> Managers
    Managers --> UI2
    Shaders --> Content
```

## File Organization

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── Content/        # Scene components (ExplodablePart, etc.)
│   │   ├── Data/           # ScriptableObjects
│   │   ├── Events/         # EventBus, event definitions
│   │   ├── Managers/       # Singleton managers
│   │   └── Utils/          # Helpers (Singleton, TweenEngine)
│   ├── UI/                 # UI Toolkit components
│   └── Tests/              # Unit tests
│       └── Editor/
├── Shaders/                # Custom HLSL shaders
├── UI/
│   └── Styles/             # USS stylesheets
└── Data/
    └── Parts/              # DronePartData assets
```

## Key Metrics

| Metric | Value |
|--------|-------|
| Total Scripts | 70+ |
| Lines of Code | ~10,000 |
| Managers | 18 |
| Shaders | 7 |
| View Modes | 7 |
| Engineer Tools | 6 |
| Unit Tests | 2 files |

## Technologies

- **Engine**: Unity 6.0 LTS
- **Render Pipeline**: Universal Render Pipeline (URP)
- **UI Framework**: UI Toolkit
- **Target**: WebGL 2.0 / WebAssembly
- **Language**: C# 11
- **Shader Language**: HLSL
