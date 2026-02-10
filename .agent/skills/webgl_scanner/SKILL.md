---
name: webgl_scanner
description: Advanced scanner for WebGL 2.0 compatibility (Shaders, Textures, Audio).
---

# WebGL Scanner

This skill performs a deep scan of the project assets to ensure strict compatibility with WebGL 2.0.

## Usage

Run this skill before builds to prevent runtime crashes or graphical glitches.

## Capabilities

1.  **Shader Validation**: Parses `.shader` files for `#pragma target` > 3.5.
2.  **Texture Audit**: Checks if textures in `Build Settings` are compressed (ASTC/DXT).
3.  **Audio Audit**: Validates that audio clips are set to `ForceToMono` and `Vorbis` (critical for WebGL memory).

## How to Run

Execute the python script in `scripts/scan_webgl.py`.

```bash
python scripts/scan_webgl.py --project-path "e:/WebGL_tesis/desarrollo/unity_project"
```
