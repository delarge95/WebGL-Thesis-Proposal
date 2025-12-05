# Unity Project Setup Guide

This guide explains how to properly set up the Unity project from scratch.

## Prerequisites

- **Unity Hub** installed
- **Unity 6.0 LTS** or higher
- **Git** (for version control)

## Step 1: Create Unity Project

1. Open **Unity Hub**
2. Click **New Project**
3. Select **3D (URP)** template
4. Set project name: `unity_project`
5. Set location: `[repo]/desarrollo/`
6. Click **Create project**

Unity will create the project with Universal Render Pipeline pre-configured.

## Step 2: Copy Scripts

After Unity creates the project:

1. The `Assets/Scripts/` folder already exists in the repository
2. Unity will automatically detect and compile the scripts
3. Wait for compilation to complete (check bottom right of Unity)

## Step 3: Copy Shaders

1. The `Assets/Shaders/` folder contains 7 custom shaders
2. Unity will compile them automatically
3. Check the Console for any shader errors

## Step 4: Configure Project Settings

### Quality Settings
1. **Edit > Project Settings > Quality**
2. Delete all quality levels except "High" and "Low"
3. Set "High" as default for Desktop
4. Set "Low" as default for WebGL

### Graphics Settings
1. **Edit > Project Settings > Graphics**
2. Ensure URP is assigned as the default render pipeline

### Player Settings
1. **Edit > Project Settings > Player**
2. **Company Name**: UNAD
3. **Product Name**: WebGL Drone Viewer
4. **WebGL Settings**:
   - Compression Format: Brotli
   - Decompression Fallback: Enabled
   - Memory Size: 512 (increase if needed)

### Tags and Layers
Create these layers if they don't exist:
- `SelectablePart` (Layer 8)
- `UI` (Layer 5 - usually exists)
- `Ignore Raycast` (Layer 2 - usually exists)

## Step 5: Create Scene Structure

### Option A: Use Editor Menu (Recommended)
1. Open Unity
2. Go to menu: **WebGL > Create Scene Structure**
3. This automatically creates all required GameObjects

### Option B: Manual Setup
Create these GameObjects in hierarchy:

```
Scene
├── _GameManagers (add all manager components)
├── MainMenu_UI (add UIDocument + UI components)
├── CameraRig
│   └── Main Camera (tag: MainCamera)
├── Lighting
│   ├── Directional Light
│   └── Fill Light
└── Drone_Root
    └── [Your drone model here]
```

## Step 6: Import Drone Model

1. Export your drone from Blender as FBX:
   - Select all parts
   - File > Export > FBX
   - Settings: Apply Modifiers, Apply Transform
   
2. Place FBX in `Assets/Models/`

3. In Unity, drag model under `Drone_Root`

4. For each part:
   - Add `ExplodablePart` component
   - Create `DronePartData` asset (right-click > Create > WebGL > Part Data)
   - Assign the data to the part

## Step 7: Configure Materials

1. Create a `Materials` folder in Assets
2. For each part:
   - Create URP/Lit material
   - Assign textures (Albedo, Normal, Metallic)
   - Apply to renderer

For View Modes to work:
- The ViewModeManager will auto-create materials for special modes
- Or assign custom materials in the Inspector

## Step 8: Test in Editor

1. Press **Play**
2. Test controls:
   - Left-click drag: Rotate camera
   - Scroll: Zoom
   - Click on parts: Select
   - Press E: Toggle exploded view
   - Press 1-6: Change camera preset

3. Check Console for errors

## Step 9: Build for WebGL

1. **File > Build Settings**
2. Select **WebGL**
3. Click **Switch Platform**
4. Click **Player Settings** and configure:
   - Resolution: 1920x1080
   - WebGL Template: Default or Custom
   - Compression: Brotli (for production)
5. Click **Build and Run**

## Troubleshooting

### Scripts not compiling
- Check Console for errors
- Ensure all namespaces match folder structure
- Verify Unity version is 6.0+

### Shaders not working
- Check Console for shader errors
- Verify URP is configured
- Shaders require WebGL 2.0

### Parts not selectable
- Ensure parts have Colliders
- Check SelectionManager layer mask
- Verify camera exists and is tagged MainCamera

### Low FPS in WebGL
- Reduce polygon count
- Use LODs
- Compress textures
- Disable shadows on mobile

## File Structure After Setup

```
unity_project/
├── Assets/
│   ├── Materials/
│   ├── Models/
│   │   └── Drone.fbx
│   ├── Scripts/
│   │   ├── Core/
│   │   └── UI/
│   ├── Shaders/
│   ├── Data/
│   │   └── Parts/
│   │       └── [DronePartData assets]
│   └── Scenes/
│       └── Main.unity
├── Packages/
├── ProjectSettings/
└── [other Unity folders]
```

## Next Steps

After setup:
1. Configure each part's DronePartData
2. Set up assembly guide steps
3. Create UI themes/styles
4. Add audio clips
5. Test WebGL build
6. Deploy to web server
