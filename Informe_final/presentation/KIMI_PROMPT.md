# Kimi Presentation Generation Prompt
## TwinSight X500 - Thesis Defense

> Prompt aligned with the current Unity app, final Blender pipeline and academic documentation. It avoids outdated claims about non-visible tools or unmeasured metrics.

---

## Master Prompt

```text
You are a senior presentation designer creating a thesis defense deck.
Generate a 20-slide presentation in Spanish following these specifications.

PROJECT CONTEXT
- Title: "Prototipo WebGL de Visualizacion 3D Interactiva para Hardware de Drones"
- Author: Alexander Woodcock Salomon
- Program: Ingenieria Multimedia - UNAD
- Case study: Holybro X500 V2 drone
- Theme: Interactive technical visualization using Unity, WebGL, WebAssembly, Blender and optimized CAD-to-runtime assets.

STRICT FACTUAL RULES
- Do not claim that all 7 view modes are public UI features.
- Public visual flow: Realistic base, X-Ray, Solid Color, Thermal and environment presets.
- Blueprint may be shown as a technical environment/preset or implemented-but-hidden reading mode, not as a guaranteed public mode card unless verified in the current build.
- Wireframe and Ghosted are implemented/hidden or legacy capabilities, not demo features.
- Do not include Measurement, BOM, connection points, annotations or assembly checklist as final user tools unless explicitly marked as future/non-integrated work.
- Thermal is heuristic visual analysis by components, not FEA and not calibrated physical measurement.
- Fastener modular meshes are temporary Unity placeholders until Blender final modules replace them through recipes/assets.
- Do not report FPS, load time, asset-size reduction, SUS or NASA-TLX values unless they match Chapter 5, validation annexes or current profiler evidence.

DESIGN SYSTEM
- Aspect ratio: 16:9, 1920x1080.
- Background: #0A0E17.
- Card background: #1E293B.
- Primary accent: #3B82F6.
- Secondary accent: #10B981.
- Text primary: #F8FAFC.
- Text secondary: #94A3B8.
- Borders: #334155.
- Headlines: Space Grotesk Bold.
- Body: Inter Regular.
- Code/labels: JetBrains Mono.
- Maximum 50 words per content slide.
- Use diagrams and screenshots more than text.
- Keep the visual style technical, clean and academic.

SLIDES

1. TITLE
- Title, author, program, university and date 2026.
- Background: ghosted drone silhouette or viewport.

2. PROBLEM
- 2D documentation requires mental reconstruction of depth, hidden parts and assembly relations.
- Visual: manual vs interactive 3D.

3. OBJECTIVES
- General objective: develop and evaluate a WebGL 3D prototype for technical inspection of drone hardware.
- Specific objectives: research, CAD-to-runtime pipeline, Unity architecture/UI, evaluation.

4. THEORETICAL FRAMEWORK
- Mind map: cognitive load, 3D HCI, PBR rendering, WebGL/WebAssembly.

5. METHODOLOGY
- Design Science Research.
- Build artifact + evaluate technical and user experience dimensions.

6. TECHNOLOGY STACK
- Unity, C#, URP, UI Toolkit, WebGL/WebAssembly, Blender.

7. ARCHITECTURE
- Layers: UI, application managers, data/assets, scene/runtime, editor tooling.
- Mention event-driven coordination and `DronePartData`.

8. VISUAL READING MODES
- Show visible modes: Realistic, X-Ray, Solid Color, Thermal.
- Show environments/presets separately: Day, Sunset, Night, Studio/Blueprint if available.
- Add note: some modes exist as hidden/technical capacity.

9. CORE INTERACTION FLOW
- Hero -> Explore -> selection -> bottom sheet -> Inspect / Analyze / Studio.
- Visual: flow diagram.

10. INSPECT / ANALYZE / STUDIO
- Inspect: selection, hotspots, isolate, power/load.
- Analyze: explode, cross-section, category filters.
- Studio: visual modes, environments and lighting.

11. FASTENER SYSTEM
- Modular metadata: family, instance, parent part, recipe.
- Lightweight proxy in rest state.
- Modular detail only under selection/isolate/context.
- Blender final modules replace placeholders without changing IDs.

12. SHADERS AND THERMAL
- Cross-section plane, X-Ray, Solid, Thermal.
- Thermal is heuristic and visual, not FEA.

13. 3D PIPELINE
- CAD/MoI3D/Blender -> retopology -> UV atlas -> bake -> external textures -> FBX -> Unity.
- Runtime must include masters and instances: BAKE_MASTERS_LOW, ASSEMBLY_INSTANCES_LOW, PRIMITIVE_FASTENER_MASTERS, PRIMITIVE_FASTENER_INSTANCES.
- Mask: R=AO, G=Roughness, B=Curvature, A=Metallic.

14. UI SCREENSHOT
- Real app screenshot.
- Callouts: bottom sheet, tabs, viewport, category filters.

15. LIVE DEMO
- Minimal slide: "Demostracion en vivo".
- Mention backup screenshots/video.

16. VALIDATION PLAN
- Technical: Unity Profiler, browser tools, build metrics.
- User: SUS for 3D prototype, NASA-TLX Raw for perceived workload, Think-Aloud for qualitative evidence.

17. RESULTS AND LIMITS
- Use Chapter 5 values when included: SUS mean 91,88; NASA-TLX Raw 3D mean 8,69; NASA-TLX Raw 2D mean 19,89; paired difference 11,19.
- Show technical performance as device-specific evidence, not universal compatibility.
- Do not invent values. Use pending/future labels for anything not measured.

18. CONTRIBUTIONS
- CAD-to-WebGL documented pipeline.
- Interactive inspection architecture.
- Modular fastener data and runtime detail.
- Evaluation plan with clear limits.

19. LIMITATIONS AND FUTURE WORK
- More devices and participants.
- Calibrated thermal model.
- Blender final fastener modules.
- CAD pipeline automation.
- Multi-language support.

20. THANK YOU
- "Gracias" / "Preguntas".
- Contact and repository QR placeholder.

CRITICAL OUTPUT RULES
- No stock photos.
- No fake code metrics.
- No unverified performance numbers.
- No features that are not part of the final UI.
- If a capability is hidden or future, label it clearly.
```

---

## Slide Content Reference

Use these exact content anchors when generating or reviewing the deck:

- Core message: "Ver piezas no basta; hay que comprender relaciones espaciales."
- App modules: Inspect, Analyze, Studio.
- Selection levels: pieza madre, subpieza, grupo de hotspot, fastener.
- Fastener principle: proxy ligero en reposo, detalle modular bajo demanda.
- Blender principle: masters + instances both form the real drone and must be exported/imported together.
- Texture principle: BaseColor, Normal and packed Mask are external assets; 4K is tested compressed before downscaling to 2K.
- Validation principle: empirical claims must be traced to Chapter 5, validation annexes or current profiler evidence.

---

## Asset Checklist

- [ ] Real screenshot of Hero/Explore.
- [ ] Real screenshot of selected part with bottom sheet.
- [ ] Real screenshot of isolated fastener or fastener detail.
- [ ] Real screenshot of Analyze filters/explode/cross-section.
- [ ] Real screenshot of Studio visual modes.
- [ ] Diagram of Blender-to-Unity pipeline.
- [ ] Diagram of fastener metadata flow.
- [ ] Evidence dashboard with Chapter 5 values and pending labels only where data is absent.

---

*Prompt Version: 2.1*
*Compatible with: Kimi or equivalent presentation generator*
*Last Updated: 2026-06-10*
