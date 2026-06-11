# Presentation Package Index
## TwinSight X500 - Thesis Defense

> Master index for presentation documents aligned with the current Unity app and Blender pipeline.

---

## Documentation Structure

```text
presentation/
|-- PRESENTATION_OUTLINE.md  <- index
|-- DESIGN_SYSTEM.md         <- visual identity and specs
|-- KIMI_PROMPT.md           <- generation prompt
|-- ASSETS_REQUIREMENTS.md   <- screenshots, diagrams, exports
|-- DEMO_SCRIPT.md           <- live demo script
```

---

## Quick Start

### Step 1: Review Design System

Read [DESIGN_SYSTEM.md](./DESIGN_SYSTEM.md) for palette, typography, spacing and component language.

### Step 2: Generate or update slides

Use [KIMI_PROMPT.md](./KIMI_PROMPT.md) as the current prompt. It intentionally avoids outdated claims such as public "7 view modes", measurement tools, BOM, connection points or unsupported performance results.

### Step 3: Add project assets

Use [ASSETS_REQUIREMENTS.md](./ASSETS_REQUIREMENTS.md) to capture screenshots from the real app:

- Hero / Explore state.
- Bottom sheet after selecting a piece.
- Fastener isolate and modular detail.
- Analyze: explode, cross-section and category filters.
- Studio: Realistic, X-Ray, Solid Color, Thermal and environment presets.
- Blender-to-Unity pipeline diagram.

### Step 4: Final Review

- Keep each slide below 50 words when possible.
- Distinguish visible UI, implemented-but-hidden features and future work.
- Report only metrics that match Chapter 5, validation tables or current profiler evidence. Leave every other metric as pending/future.
- Verify that demo, manuals, report chapters and Obsidian study notes tell the same story.

---

## Slide Overview (20 slides)

| # | Title | Layout | Key Visual |
|---|-------|--------|------------|
| 1 | Title | Centered | Drone silhouette or final viewport |
| 2 | Problem | 50/50 split | 2D manual vs interactive 3D |
| 3 | Objectives | 2x2 cards | General + specific objectives |
| 4 | Theory | Mind map | Cognitive load, HCI, PBR, WebGL |
| 5 | Methodology | Timeline | DSR phases and evaluation |
| 6 | Tech Stack | Icon grid | Unity, WebGL, C#, URP, UI Toolkit, Blender |
| 7 | Architecture | Layers | Runtime managers and data flow |
| 8 | Visual Reading | Comparison grid | Realistic, X-Ray, Solid, Thermal, environments |
| 9 | Core Flow | 4 cards | Select, bottom sheet, isolate, hotspots |
| 10 | Analyze / Inspect / Studio | 2x3 grid | Actual published workflows |
| 11 | Technical State | Dashboard | Validated metrics and stated limits |
| 12 | Shaders | Split | Cross-section, X-Ray, Thermal |
| 13 | 3D Pipeline | Flow | CAD, Blender, bake, manifest, Unity |
| 14 | UI | Full bleed | Real screenshot with callouts |
| 15 | Demo | Minimal | "Demo en vivo" |
| 16 | Validation | Split | SUS, NASA-TLX, Think-Aloud, profiler |
| 17 | Results and Limits | Checklist | Chapter 5 evidence and pending limits |
| 18 | Conclusions | 3 cards | Defensible contributions |
| 19 | Future Work | Roadmap | Calibrated thermal, more devices, automation |
| 20 | Thank You | Centered | QR/contact |

---

## Time Management

| Section | Slides | Duration |
|---------|--------|----------|
| Introduction | 1-2 | 2 min |
| Objectives and theory | 3-4 | 3 min |
| Methodology | 5-6 | 2 min |
| Implementation | 7-14 | 10 min |
| Demo | 15 | 5 min |
| Validation and results | 16-17 | 3 min |
| Conclusions and future work | 18-19 | 3 min |
| Closing | 20 | 2 min |
| **Total** | 20 | **~30 min** |

---

## Coherence Rules

- Treat `Realistic`, `X-Ray`, `Solid Color` and `Thermal` as visible visual modes in the current UI.
- Treat `Blueprint` primarily as an environment/preset or implemented reading mode depending on the current build state, not as one of seven public mode cards unless it is actually visible.
- Treat `Wireframe` and `Ghosted` as implemented/hidden or legacy capabilities unless explicitly re-enabled.
- Treat measurement, BOM, annotations and connection tools as non-integrated or future work, not demo features.
- Treat fastener modular meshes in Unity as temporary placeholders until Blender modules replace them through recipe/assets.
- Treat the final Blender FBX import as prepared/pending QA until the Unity scene import report confirms hierarchy, instancing, propeller axes and group behavior.
- Treat Chapter 5 values as the only closed empirical baseline for defense: SUS mean 91.88, NASA-TLX Raw mean 8.69 for the 3D prototype and 19.89 for the 2D reference, plus the device-specific FPS table in the validation annexes.

---

## Pre-Presentation Checklist

### Preparation

- [ ] Screenshots captured from the actual app, not old mockups.
- [ ] Demo path rehearsed using [DEMO_SCRIPT.md](./DEMO_SCRIPT.md).
- [ ] Build or Play Mode tested.
- [ ] Backup screenshots/video ready.
- [ ] Metrics cross-checked against Chapter 5, validation annexes or current profiler evidence.

### Content Review

- [ ] No "7 visible modes" claim unless UI truly exposes all seven.
- [ ] No measurement/BOM/catalog demo unless the final UI exposes them.
- [ ] Thermal described as heuristic, not FEA.
- [ ] Fastener placeholders described honestly.
- [ ] Blender final import status matches the current Unity QA state.

---

## Related Files

| File | Description |
|------|-------------|
| [DESIGN_SYSTEM.md](./DESIGN_SYSTEM.md) | Visual identity guide |
| [KIMI_PROMPT.md](./KIMI_PROMPT.md) | Current presentation generation prompt |
| [ASSETS_REQUIREMENTS.md](./ASSETS_REQUIREMENTS.md) | Screenshot and diagram requirements |
| [DEMO_SCRIPT.md](./DEMO_SCRIPT.md) | Current live demo script |
| [../VARIANTE_ESTUDIO_OBSIDIAN.md](../VARIANTE_ESTUDIO_OBSIDIAN.md) | Study map linked to Obsidian |

---

*Package Version: 3.1*
*Updated: 2026-06-10*
*Project: TwinSight X500 Thesis*
