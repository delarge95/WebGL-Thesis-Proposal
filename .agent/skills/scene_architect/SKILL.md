---
name: scene_architect
description: Programmatically constructs the Unity Scene to ensure perfect configuration.
---

# Scene Architect

This skill generates the `MainScene_Final.unity` with all required components, ensuring no "magic clicks" are needed.

## Usage

1.  Run the C# action via menu `Antigravity > Build Final Scene`.

## Capabilities

1.  **Scene Generation**: Creates/Overwrites `MainScene_Final`.
2.  **UI Setup**: Attaches `UIDocument` with correct PanelSettings and Source UXML.
3.  **Lighting Setup**: Configures Directional Light and Global Volume (Tone Mapping).
4.  **Camera Setup**: Positions the camera correctly for the initial view.

## Dependencies

- `Assets/UI/MainView.uxml`
- `Assets/UI/MainPanelSettings.asset`
