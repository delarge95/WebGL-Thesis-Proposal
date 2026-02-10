---
name: unity_asset_auditor
description: Scans the Unity project for critical missing assets (fonts, settings, packages) and misconfigurations.
---

# Unity Asset Auditor

This skill performs a comprehensive scan of the Unity project to ensure all required assets for the "Operation Rescue" are present and correctly configured.

## Usage

Run this skill whenever you suspect missing assets or after pulling changes from version control.

## Capabilities

1.  **Font Validation**: Checks for required .ttf/.otf files in `Assets/UI/Fonts`.
2.  **Settings Validation**: Verifies existence of `PanelSettings` and `URP_WebGL` assets.
3.  **Package Validation**: Checks `manifest.json` for required and forbidden packages.
4.  **Meta File Integrity**: Scans for missing meta files ("ghost" assets).

## How to Run

Execute the `audit_assets.ps1` script located in the `scripts` directory of this skill.

```powershell
./scripts/audit_assets.ps1
```
