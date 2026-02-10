---
name: webgl_optimizer
description: Validates and optimizes project settings for high-performance WebGL builds.
---

# WebGL Optimizer

This skill checks the project configuration against WebGL best practices, ensuring optimal performance and compatibility.

## Usage

Run this skill before attempting a WebGL build.

## Capabilities

1.  **Color Space Check**: Ensures `Linear` color space is used for high-fidelity rendering.
2.  **Lightmap Encoding**: Verifies `High Quality` lightmaps are enabled.
3.  **Compression Check**: Recommends ASTC/DXT texture compression.
4.  **Stripping Level**: Suggests `Medium` or `High` managed stripping level for smaller builds.

## How to Run

Execute the `optimize_webgl.ps1` script located in the `scripts` directory of this skill.

```powershell
./scripts/optimize_webgl.ps1
```
