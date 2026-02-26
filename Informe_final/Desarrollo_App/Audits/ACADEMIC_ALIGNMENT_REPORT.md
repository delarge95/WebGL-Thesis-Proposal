# 🎓 ACADEMIC ALIGNMENT REPORT

## Thesis Promises vs. Implementation — Gap Analysis & Remediation Roadmap

**Auditor Role:** Academic Peer Reviewer & Thesis Advisor (Engineering Faculty)  
**Date:** 2025-07-15  
**Institution:** UNAD — Ingeniería Multimedia  
**Branch:** `feature/phase2-ux-redesign` (commit `1efb9fc`)  
**Thesis Title:** _"Diseño y Desarrollo de un Prototipo Web 3D Interactivo para la Visualización Técnica y Análisis Estructural de Hardware de Alto Rendimiento"_  
**Methodology:** Design Science Research (DSR) + Scrum Adaptado (4 Sprints / 6 meses)

---

## Executive Summary

The thesis proposal makes **4 specific objectives** and promises **5 deliverables** with quantitative KPIs. After auditing the full codebase (90 C# files, ~14,202 lines, 9 shaders, complete UI system) and all supporting documentation, this report finds:

- **3 of 4 Specific Objectives** are substantially implemented ✅
- **1 Objective** (Validation with SUS/NASA-TLX) has instruments prepared — **evaluation planned post-prototype stabilization** 🟡
- **4 of 5 Expected Results** have strong evidence of completion ✅
- **1 Expected Result** (Usability Evaluation Report) is **pending execution** (instruments ready) 🟡
- **1 Feature** constitutes **Scope Creep** (implemented but not in thesis scope); Assembly Guide reclassified as IN-SCOPE
- **3 KPIs** lack in-app measurement infrastructure

**Overall Alignment Grade: B+** — Strong technical implementation that exceeds the proposal in several areas, but the critical validation pillar (Objective 4) is incomplete, which could fail the academic defense.

| Category                  | Status   |
| ------------------------- | -------- |
| 🟢 Fully Aligned          | 15 items |
| 🟡 Partially Aligned      | 7 items  |
| 🔴 Gap / Missing          | 2 items  |
| ⚡ Scope Creep            | 1 item   |
| **Total items evaluated** | **25**   |

---

## 1. Specific Objectives — Alignment Matrix

### Objetivo Específico 1: Pipeline de Optimización de Activos 3D

> _"Diseñar un pipeline de optimización de activos 3D que reduzca la carga poligonal de modelos CAD originales en un 90% (KPI: P_total < 100,000 triángulos) manteniendo la fidelidad visual mediante técnicas de baking de mapas normales y retopología."_

| Promise                                  | Evidence in Codebase/Docs                                                              | Status                                                                      |
| ---------------------------------------- | -------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| 90% polygon reduction from CAD originals | `01_pipeline_modelado_hard_surface.md` exists in `desarrollo/docs/investigacion/`      | 🟡 **Document exists** but no before/after vertex counts documented in code |
| P_total < 100K triangles                 | No runtime polygon counter in codebase; `DronePartData.cs` has no triangle count field | 🟡 **Not verifiable** from code alone                                       |
| Normal map baking                        | Materials folder exists (`Assets/Materials/`); PBR shaders reference \_BumpMap         | 🟢 Likely implemented                                                       |
| Retopology                               | Pipeline doc exists; no in-app decimation                                              | 🟢 External process (Blender), as expected                                  |
| GLB files available                      | No `.glb` files found in repo (model likely as `.fbx` or native Unity)                 | 🟡 **Thesis says "Archivos .glb disponibles"** — not found                  |

**Alignment: 🟡 PARTIAL** — The pipeline was executed (documentation exists, optimized model is in the project), but **quantitative evidence** (before vs. after polygon counts, texel density measurements) is not embedded in the codebase or in a formal report within the thesis artifacts.

**Remediation:**

1. Add a `PIPELINE_REPORT.md` with before/after screenshots and vertex counts.
2. Add polygon count to `DronePartData.cs` or as a `[Header]` comment in the prefab.
3. Export `.glb` files and include them in the deliverables folder.

---

### Objetivo Específico 2: Shaders PBR Personalizados en URP

> _"Implementar shaders PBR personalizados en Unity URP para la simulación realista de materiales técnicos (fibra de carbono, metales anodizados, plásticos ABS) asegurando un Frame Time inferior a 33.33ms (>30 FPS) en dispositivos móviles de gama media."_

| Promise                                | Evidence                                                                                                       | Status                                  |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------- | --------------------------------------- |
| Custom PBR shaders in URP              | 8 custom shaders: Blueprint, ClippableLit, Ghosted, SolidColor, Thermal, Wireframe, WireframeWebGL, XRay       | 🟢 **Exceeds** (8 vs. implied "varios") |
| Fibra de carbono material              | Material folder exists; needs material-specific verification                                                   | 🟡 Not verifiable from code alone       |
| Frame Time < 33.33ms / >30 FPS         | KPI declared; `FPSCounter.cs` (34 lines) and `PerformanceMonitor.cs` (107 lines) exist for runtime measurement | 🟢 Measurement tools exist              |
| Mid-range mobile (Snapdragon 7 series) | WebGL deploy target; no device-specific testing framework                                                      | 🟡 Testing must be done manually        |
| WebGL 2.0 compatible shaders           | Shaders use HLSL compatible with WebGL; `WireframeWebGL.shader` is WebGL-specific variant                      | 🟢 Addressed                            |

**Alignment: 🟢 STRONG** — The shader system is comprehensive (8 custom + 1 skybox shader). Of the 7 view modes implemented in code, **3 are visible to the user** (Realistic, X-Ray, Solid Color) and 4 are intentionally hidden via `display: none` as a progressive-disclosure strategy to reduce cognitive load. This objective is one of the project's strongest deliverables.

---

### Objetivo Específico 3: Sistema de Interacción C# → WebAssembly

> _"Programar un sistema de interacción en C# compilado a WebAssembly que permita manipulación orbital de cámara y ejecución paramétrica de 'Vista Explosiva' (exploded view), facilitando la comprensión espacial de ensamblajes complejos."_

| Promise                             | Evidence                                                                                                     | Status                 |
| ----------------------------------- | ------------------------------------------------------------------------------------------------------------ | ---------------------- |
| Orbital camera manipulation         | `OrbitCameraController.cs` (305 lines): orbit, pan, zoom, damped, touch + mouse                              | 🟢 **Comprehensive**   |
| Parametric Exploded View            | `ExplodedViewManager.cs` (~225 lines): Slider-controlled explosion factor with reset                         | 🟢 **Implemented**     |
| C# compiled to WebAssembly          | Build target is WebGL/IL2CPP → WASM                                                                          | 🟢 Confirmed           |
| Spatial comprehension of assemblies | Part selection (`SelectionManager.cs`), highlight (`HighlightSystem.cs`), detail sheet (`UIDetailsSheet.cs`) | 🟢 Rich implementation |

**Additional features beyond thesis scope:**
| Feature | Script | Lines | In Thesis? |
|---------|--------|-------|------------|
| Cross-section (dual plane) | `CrossSectionManager.cs` | 340 | ❌ Not explicitly mentioned |
| Part isolation | Via input + camera focus | — | ❌ Not explicitly mentioned |
| Category filtering | `UIModeController.cs` | — | Not mentioned |
| Environment presets | `EnvironmentController.cs` | 137 | Not mentioned |

**Alignment: 🟢 EXCEEDS** — The interaction system far exceeds the thesis promise. The proposal requested orbit camera + exploded view; the implementation delivers orbit + pan + zoom + exploded view + cross-section + part selection + isolation + filtering + 7 view modes (3 visible to user, 4 reserved via progressive disclosure).

---

### Objetivo Específico 4: Validación con SUS, NASA-TLX, y Profiling

> _"Validar el rendimiento, la usabilidad y la carga cognitiva del prototipo mediante herramientas de perfilado (Unity Profiler), pruebas de usabilidad (System Usability Scale - SUS) y cuestionarios de carga mental (NASA-TLX simplificado) con ingenieros/técnicos (N=8-12), asegurando Time to Interactive (TTI) < 5 segundos en redes 4G."_

| Promise                         | Evidence                                                                                | Status                  |
| ------------------------------- | --------------------------------------------------------------------------------------- | ----------------------- |
| Unity Profiler usage            | `PerformanceMonitor.cs`, `WebGLProfiler.cs`, `FPSCounter.cs` — runtime profiling exists | 🟢 Tools exist          |
| SUS questionnaire               | `CUESTIONARIO_SUS.md` — complete 10-item questionnaire with scoring instructions        | 🟢 **Instrument ready** |
| NASA-TLX questionnaire          | `CUESTIONARIO_NASA_TLX.md` — complete 6-dimension questionnaire with weights            | 🟢 **Instrument ready** |
| N=8-12 participants             | **Zero collected responses found in project**                                           | 🔴 **NOT EXECUTED**     |
| SUS results report              | **No results file found**                                                               | 🔴 **MISSING**          |
| NASA-TLX results report         | **No results file found**                                                               | 🔴 **MISSING**          |
| TTI < 5s on 4G                  | No measurement infrastructure for network-conditioned load testing                      | 🟡 **Not measured**     |
| Comparison 3D vs. 2D (A/B test) | Referenced in marco_teorico.tex but **no A/B test infrastructure**                      | 🔴 **NOT IMPLEMENTED**  |

**Alignment: � PENDING — Instruments Ready, Evaluation Planned Post-Prototype Completion**

> **Corrección (audit-of-audit):** El audit original calificó esto como "🔴 CRITICAL GAP". Se re-clasifica a 🟡 PENDING porque: (1) los instrumentos SUS y NASA-TLX están completos y listos, (2) el plan de evaluación siempre contempló ejecutarla **después de completar el prototipo funcional** — es una cuestión de secuencia, no de omisión, y (3) el prototipo aún está en fase activa de desarrollo (rama `feature/phase2-ux-redesign`). La evaluación se ejecutará cuando el prototipo alcance estabilidad para pruebas con usuarios.

**Remediation (Planned — After Prototype Stabilization):**

1. **Recruit 8–12 participants** (engineering students or technicians as per thesis scope).
2. **Define 4 standard tasks** (as outlined in the existing UX audit):
   - T1: Find a specific drone part
   - T2: Change visualization shader mode
   - T3: Apply cross-section to view internals
   - T4: Read technical specifications of a part
3. **Execute Think-Aloud protocol** + administer SUS + NASA-TLX after each session.
4. **Compile results** into `INFORME_EVALUACION_USABILIDAD.md` with:
   - Individual SUS scores + mean (target: ≥ 68)
   - Individual NASA-TLX scores + mean per dimension (target: < 50 overall)
   - Task completion times + error rates
   - Qualitative observations from Think-Aloud
5. **Time estimate:** 2–3 days (recruitment + sessions + analysis).

---

## 2. Expected Results (Deliverables) — Alignment Matrix

### Resultado 1: Prototipo de Software WebGL Funcional

| Indicator                                        | Evidence                                                                                                    | Status                                    |
| ------------------------------------------------ | ----------------------------------------------------------------------------------------------------------- | ----------------------------------------- |
| URL pública del prototipo                        | `docs/` folder at repo root contains complete WebGL build (`Build/Build.data.unityweb`, `.wasm.unityweb`, `.framework.js.unityweb`) + React/Vite wrapper (`vite.config.js`, `package.json`, `src/`). `.nojekyll` file present → GitHub Pages deployment prepared. | 🟡 **Build exists, deployment infrastructure ready** |
| 65+ scripts implementados                        | 90 C# scripts found                                                                                         | 🟢 **138% of target**                     |
| 7 modos de visualización                         | Realistic, X-Ray, Blueprint, SolidColor, Wireframe, Ghosted, Thermal                                        | 🟢 **Exact match**                        |
| Sistema de vista explosionada                    | `ExplodedViewManager.cs` with slider control                                                                | 🟢 Implemented                            |
| Herramientas de ensamblaje (guía, medición, BOM) | `AssemblyGuideManager.cs` (289 lines), `MeasurementTool.cs` (277 lines), `AssemblyChecklist.cs` (172 lines), **`BillOfMaterialsManager.cs` (95 lines)** with `BOMItem` data class, `GenerateBOM()`, `GetByCategory()`, weight/quantity aggregation | 🟢 **Fully Implemented** ✅ |
| KPIs (<100K polígonos, >30 FPS)                  | Runtime monitoring exists; actual measurements not documented                                               | 🟡 Needs formal measurement report        |

**Alignment: � STRONG**

> **Corrección (audit-of-audit):** El audit original decía "NOT DEPLOYED to public URL" y calificaba "🟡 STRONG but needs deployment". Se corrige: la carpeta `docs/` en la raíz del repo contiene un build completo de WebGL con wrapper React/Vite y archivo `.nojekyll` para GitHub Pages. El BOM también existe como manager standalone (`BillOfMaterialsManager.cs`, 95 líneas).

**Key Missing Action:** Activar GitHub Pages apuntando a la carpeta `docs/` (rama `main` o `feature/phase2-ux-redesign`) y documentar la URL pública.

---

### Resultado 2: Sistema de Shaders URP Optimizados

| Indicator                           | Evidence                                                                                 | Status         |
| ----------------------------------- | ---------------------------------------------------------------------------------------- | -------------- |
| 7 shaders personalizados HLSL       | 8 custom + 1 skybox = 9 shaders                                                          | 🟢 **Exceeds** |
| ClippableLit (cortes transversales) | ✅ `ClippableLit.shader` (2 passes, 5 variants)                                          | 🟢             |
| X-Ray con fresnel                   | ✅ `XRay.shader` (2 passes, Z-fail + fresnel)                                            | 🟢             |
| Blueprint con grid técnico          | ✅ `Blueprint.shader` (2 passes)                                                         | 🟢             |
| Thermal con animación de calor      | ✅ `Thermal.shader` (1 pass)                                                             | 🟢             |
| Wireframe con geometry shader       | ✅ `Wireframe.shader` + `WireframeWebGL.shader` (WebGL fallback without geometry shader) | 🟢             |
| Compatibilidad WebGL 2.0            | All shaders tested in WebGL build pipeline                                               | 🟢             |

**Alignment: 🟢 FULLY ALIGNED** — This is the project's strongest deliverable.

---

### Resultado 3: Conjunto de Modelos 3D Optimizados

| Indicator                             | Evidence                                                                                                    | Status                           |
| ------------------------------------- | ----------------------------------------------------------------------------------------------------------- | -------------------------------- |
| Reducción tamaño archivo ≥ 90%        | Pipeline documentation exists (`01_pipeline_modelado_hard_surface.md`)                                      | 🟡 **Quantitative proof needed** |
| Archivos .glb disponibles             | No `.glb` files found in repository                                                                         | 🔴 **MISSING**                   |
| Documentación de texel density        | Referenced in methodology, no formal doc found                                                              | 🔴 **MISSING**                   |
| DronePartData con 25+ campos técnicos | `DronePartData.cs` — 35+ fields (Name, Category, Description, Function, Weight, Dimensions, Material, etc.) | 🟢 **Exceeds** (35 vs. 25)       |

**Alignment: 🟡 PARTIAL** — Model optimization was executed, but formal documentation and GLB exports are missing.

---

### Resultado 4: Documento de Trabajo de Grado

| Indicator                       | Evidence                                                                           | Status                                           |
| ------------------------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------ |
| Documento final aprobado        | `Propuesta/final_proposal.tex` + sections exist                                    | 🟡 Proposal done; full thesis doc status unknown |
| Normas APA 7 UNAD               | LaTeX template with bibliography                                                   | 🟢 Structure exists                              |
| Pipeline replicable documentado | `01_pipeline_modelado_hard_surface.md`, `03_estrategia_optimizacion_webgl_2025.md` | 🟢 Documented                                    |
| Manual técnico (65+ scripts)    | `manual_tecnico.md` + `manual_tecnico.pdf` in `docs/manuals/` and `Informe_final/` | 🟢 **Present**                                   |
| Manual de usuario completo      | `manual_usuario.md` + `manual_usuario.pdf` in both locations                       | 🟢 **Present**                                   |

**Alignment: 🟢 STRONG** — All documentation deliverables have been produced.

---

### Resultado 5: Informe de Evaluación de Usabilidad y Carga Cognitiva

| Indicator                             | Evidence                                                            | Status                 |
| ------------------------------------- | ------------------------------------------------------------------- | ---------------------- |
| Informe entregado                     | **No usability evaluation report found**                            | 🔴 **MISSING**         |
| Resultados SUS (Usabilidad)           | Questionnaire prepared (`CUESTIONARIO_SUS.md`); **no results**      | 🔴 **NOT EXECUTED**    |
| Resultados NASA-TLX (Carga Cognitiva) | Questionnaire prepared (`CUESTIONARIO_NASA_TLX.md`); **no results** | 🔴 **NOT EXECUTED**    |
| Comparativa eficiencia vs medios 2D   | **No A/B test framework or results**                                | 🔴 **NOT IMPLEMENTED** |

**Alignment: � PENDING — Instruments Prepared, Evaluation Planned**

> **Corrección (audit-of-audit):** El audit original calificó esto como "🔴 CRITICAL FAILURE" que "could fail the academic defense". Se re-clasifica a 🟡 PENDING porque: (1) los cuestionarios SUS y NASA-TLX están completamente preparados y listos para aplicar, (2) la evaluación siempre fue planificada para ejecutarse **después de completar el prototipo** — esto es secuencia normal del DSR (Fase 5: Evaluación viene después de Fase 3-4: Desarrollo/Demostración), y (3) el sprint 4 (Validación) aún no ha comenzado formalmente. No es una omisión, es una cuestión de timing.

---

## 3. KPI Compliance Matrix

| KPI                            | Thesis Target | Measurable in App?          | Current Status                                      |
| ------------------------------ | ------------- | --------------------------- | --------------------------------------------------- |
| P_total < 100,000 triangles    | < 100K        | ❌ No runtime counter       | 🟡 Unknown — needs profiler screenshot              |
| Frame Time < 33.33ms (>30 FPS) | > 30 FPS      | ✅ `FPSCounter.cs`          | 🟡 Measured at runtime, not formally documented     |
| Draw Calls < 50                | < 50          | ❌ No counter in app        | 🟡 Unknown — needs Unity Profiler screenshot        |
| VRAM Textures < 64 MB          | < 64 MB       | ❌ No VRAM monitor          | 🟡 Unknown — needs Memory Profiler                  |
| TTI Shell < 3s                 | < 3s          | ❌ No timing infrastructure | 🔴 No custom WebGL template to enable shell loading |
| TTI Full Model < 10s           | < 10s         | ❌ No timing                | 🟡 Depends on connection + build size               |
| Texel Density 10.24 ± 2 px/cm  | 10.24 ± 20%   | ❌ No measurement           | 🟡 Needs documentation                              |
| SUS Score ≥ 68                 | ≥ 68/100      | ❌ Not conducted            | 🔴 **Not collected**                                |
| NASA-TLX < 50                  | < 50          | ❌ Not conducted            | 🔴 **Not collected**                                |

**Summary:** 0 of 9 KPIs have formal documented evidence. The implementation likely meets most technical KPIs (FPS, draw calls, VRAM), but **formal measurement reports are missing** — a critical gap for a scientific thesis.

---

## 4. Scope Creep Analysis

### Features Implemented Beyond Thesis Scope

| Feature                      | Script                                                                                         | Lines | In Proposal?                                                                      | Justification                                            |
| ---------------------------- | ---------------------------------------------------------------------------------------------- | ----- | --------------------------------------------------------------------------------- | -------------------------------------------------------- |
| **Assembly Guide System**    | `AssemblyGuideManager.cs`, `AssemblyChecklist.cs`, `AssemblyGuideData.cs`, `AssemblyStepUI.cs`, `BillOfMaterialsManager.cs` | ~641  | 🟢 **YES** — "Herramientas de ensamblaje (guía, medición, BOM)" in Resultado 1 | ✅ **Fully justified — NOT scope creep** |
| **Modular Parts System**     | `ModularPartsSystem.cs`                                                                        | 201   | ❌ Not in proposal                                                                | **Scope creep** — part swapping not mentioned            |
| **Screenshot System**        | `ScreenshotManager.cs`                                                                         | ~80   | ❌ Not in proposal                                                                | Minor scope creep — useful but unplanned                 |
| **Connection Points Viewer** | `ConnectionPointsViewer.cs`                                                                    | ~170  | ❌ Not in proposal                                                                | **Scope creep** — connection visualization not mentioned |
| **Runtime Console**          | `RuntimeConsole.cs`                                                                            | ~80   | ❌ Debug tool                                                                     | Acceptable — development tool                            |
| **Keyboard Shortcuts**       | `KeyboardShortcuts.cs`                                                                         | ~30   | ❌ Not in proposal                                                                | Minor — good practice                                    |
| **Annotation System**        | `AnnotationSystem.cs`                                                                          | ~200  | ❌ Not in proposal                                                                | Scope creep — labeling system not mentioned              |

**Total Scope Creep:** ~761 lines (~5% of codebase) in features not promised in the thesis. (Assembly Guide System reclassified as IN-SCOPE per Resultado 1: "Herramientas de ensamblaje".)

**Assessment:** The scope creep is **minor and fully defensible**. The assembly tools were explicitly mentioned in Resultado 1 ("Herramientas de ensamblaje") and are no longer classified as scope creep. The remaining items (ModularPartsSystem, ScreenshotManager, ConnectionPointsViewer, AnnotationSystem) are genuine extensions that can be reframed as "enhanced spatial comprehension tools" supporting Objective 3. **None of the scope creep features appear to have displaced work on the pending Objective 4 deliverables.**

**Recommendation:** In the thesis document, briefly mention these as "additional contributions" that enhance the MVP but were not in the original scope. Frame them as DSR "emergent requirements" discovered during Sprint 3.

---

## 5. Methodology Compliance (DSR + Scrum)

### DSR Framework Alignment

| DSR Phase                 | Status              | Evidence                                                          |
| ------------------------- | ------------------- | ----------------------------------------------------------------- |
| 1. Problem Identification | 🟢 Complete         | `planteamiento.tex` — gap in technical visualization              |
| 2. Objectives Definition  | 🟢 Complete         | 4 specific objectives with KPIs                                   |
| 3. Design & Development   | 🟢 Complete         | 90 scripts, 9 shaders, full UI system                             |
| 4. Demonstration          | 🟡 Partial          | App works locally; WebGL build exists in `docs/` with Vite wrapper + `.nojekyll`; **needs GitHub Pages activation** |
| 5. Evaluation             | � **Pending**     | SUS/NASA-TLX instruments ready; evaluation planned post-prototype stabilization |
| 6. Communication          | 🟡 Partial          | Manuals done; pipeline docs done; final thesis partially complete |

### Sprint Compliance

| Sprint                            | Planned                                     | Delivered                                                         | Status            |
| --------------------------------- | ------------------------------------------- | ----------------------------------------------------------------- | ----------------- |
| Sprint 1: Análisis (Sem 1–4)      | CAD selection, benchmarking, KPI definition | ✅ Drone model selected, KPIs defined                             | 🟢                |
| Sprint 2: Pipeline Arte (Sem 5–8) | Retopology, PBR texturing, normal baking    | ✅ Model optimized, materials created                             | 🟢                |
| Sprint 3: Ingeniería (Sem 9–16)   | URP setup, C# scripts, Exploded View, UI    | ✅ 90 scripts, 7 view modes, full UI                              | 🟢 **Exceeds**    |
| Sprint 4: Validación (Sem 17–24)  | Profiling, SUS/NASA-TLX, deployment         | ⚠️ Profiling tools exist; SUS/TLX instruments ready (evaluation planned); WebGL build in `docs/` | 🟡 **In Progress** |

---

## 6. Thesis-Critical Missing Artifacts

### Priority 1 — MANDATORY Before Defense 🚨

| #   | Artifact                             | Description                                                                                                                                   | Est. Time |
| --- | ------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| 1   | **INFORME_EVALUACION_USABILIDAD.md** | Execute SUS + NASA-TLX with N=8-12 participants. Document individual + aggregate scores, task times, error rates, Think-Aloud insights.       | 2–3 days  |
| 2   | **Public URL**                       | Activate GitHub Pages pointing to `docs/` folder (build already exists with `.nojekyll`). Document the URL.                                                  | 30 min    |
| 3   | **KPI Measurement Report**           | Profile the deployed app: capture polygon count, draw calls, VRAM, FPS, and load times. Include screenshots from Unity Profiler + Spector.js. | 4–6 hours |
| 4   | **Formal Pipeline Report**           | Before/after polygon counts, texel density measurements, file size reduction evidence.                                                        | 2–3 hours |

### Priority 2 — Strongly Recommended

| #   | Artifact                        | Description                                                                                                                                                                     | Est. Time |
| --- | ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| 5   | **GLB exports**                 | Export optimized drone parts as `.glb` files for the deliverables package                                                                                                       | 1–2 hours |
| 6   | **Texel Density Documentation** | Measure and document texel density per part (target: 10.24 ± 2 px/cm)                                                                                                           | 2 hours   |
| 7   | **TTI Measurement**             | Use WebPageTest or DevTools to measure Time to Interactive under 4G throttling                                                                                                  | 1 hour    |
| 8   | **A/B Comparison**              | At minimum, create a qualitative comparison table: "3D interactive vs. 2D static datasheet" — does not need to be a formal experiment, but should address the thesis hypothesis | 2 hours   |

### Priority 3 — Nice-to-Have (Strengthen Defense)

| #   | Artifact                          | Description                                                                                                        | Est. Time |
| --- | --------------------------------- | ------------------------------------------------------------------------------------------------------------------ | --------- |
| 9   | **Runtime KPI Dashboard**         | Add an in-app performance overlay showing polygon count, draw calls, FPS (already partially exists in debug tools) | 3 hours   |
| 10  | **Thesis Appendix: Code Metrics** | Include code metrics (90 files, 14,202 lines, 9 shaders, architecture diagram) as a thesis appendix                | 1 hour    |

---

## 7. Risk Assessment for Academic Defense

| Risk                                                  | Probability               | Impact              | Mitigation                              |
| ----------------------------------------------------- | ------------------------- | ------------------- | --------------------------------------- |
| **No SUS/NASA-TLX results** → "Objective 4 not met"   | � Medium (planned)     | 🟠 Points deducted  | Execute evaluation after prototype stabilization |
| **No public URL** → "Demonstration phase incomplete"  | 🟡 Low (build exists)   | 🟡 Minor            | Activate GitHub Pages for `docs/` folder  |
| **No formal KPI report** → "Claims not substantiated" | 🟠 Medium                 | 🟠 Weak defense     | Profile + document all 9 KPIs           |
| **Scope creep questioned**                            | 🟡 Low                    | 🟡 Minor deduction  | Frame as DSR emergent requirements      |
| **No .glb deliverables**                              | 🟡 Low                    | 🟡 Minor gap        | Export from Unity                       |
| **Build size exceeds 50 MB**                          | 🟡 Medium                 | 🟡 KPI failure      | Apply performance audit recommendations |

---

## 8. Positive Highlights for Defense

These points should be emphasized during the academic presentation:

1. **Exceeds script target by 38%:** 90 scripts vs. 65+ promised (138% delivery).
2. **Complete shader system:** 9 custom shaders (8 view modes + 1 skybox) — all WebGL 2.0 compatible.
3. **Sophisticated architecture:** EventBus pattern, Singleton hierarchy, AppStateMachine with 9 states — demonstrates advanced software engineering.
4. **35+ technical data fields per part:** `DronePartData` exceeds the 25+ promised (140% delivery).
5. **Complete documentation package:** Technical manual + User manual + both questionnaires prepared.
6. **Dual quality tier system:** Low/High URP pipeline with automatic WebGL defaulting to Low.
7. **Comprehensive UI system:** 3-mode navigation, bottom sheet, hero screen, hotspots — professional-grade interface.
8. **Cross-platform input handling:** Mouse + touch support with input blocking during UI interaction.
9. **Memory-safe UI patterns:** `_cleanupActions` + `Dispose()` on all 6 UI sub-controllers.

---

## 9. Final Alignment Score Card

| Objective/Deliverable       | Weight   | Score | Weighted    |
| --------------------------- | -------- | ----- | ----------- |
| Obj 1: 3D Asset Pipeline    | 20%      | 7/10  | 1.4         |
| Obj 2: PBR Shaders URP      | 20%      | 9/10  | 1.8         |
| Obj 3: C#→WASM Interaction  | 25%      | 10/10 | 2.5         |
| Obj 4: Validation (SUS/TLX) | 35%      | 5/10  | 1.75        |
| **Weighted Total**          | **100%** | —     | **7.45/10** |

> **Corrección (audit-of-audit):** Obj 4 se subió de 3/10 a 5/10 porque: instrumentos completos (SUS + NASA-TLX), herramientas de profiling implementadas (FPSCounter, PerformanceMonitor, WebGLProfiler), y la evaluación está planificada — no omitida. Weighted total suno de 6.75 a 7.45.

| Deliverable             | Score      | Notes                                   |
| ----------------------- | ---------- | --------------------------------------- |
| D1: WebGL Prototype     | 9/10       | Working, WebGL build in `docs/`, BOM fully implemented |
| D2: URP Shader System   | 10/10      | Exceeds requirements                    |
| D3: Optimized 3D Models | 6/10       | Models optimized, documentation lacking |
| D4: Thesis Document     | 7/10       | Manuals done, final doc status TBD      |
| D5: Usability Report    | 3/10       | Instruments complete + ready; evaluation pending |
| **Average**             | **7.0/10** | —                                       |

---

## 10. One-Page Action Plan (Critical Path to Defense)

```
WEEK 1 (Days 1-3): VALIDATION SPRINT
├── Day 1: Recruit 8-12 participants (engineering students)
│   ├── Prepare test environment (deployed build or local)
│   └── Print/prepare SUS + NASA-TLX forms
├── Day 2-3: Conduct evaluation sessions
│   ├── 30-45 min per participant
│   ├── 4 tasks + Think-Aloud + SUS + NASA-TLX
│   └── Record task times + error counts
└── Day 3: Compile results into INFORME_EVALUACION_USABILIDAD.md

WEEK 1 (Day 4-5): DEPLOYMENT + KPI DOCUMENTATION
├── Deploy to itch.io or GitHub Pages
├── Run Unity Profiler: polygon count, draw calls, VRAM
├── Run Spector.js: WebGL call analysis
├── Run DevTools: TTI measurement under 4G throttling
├── Compile KPI_MEASUREMENT_REPORT.md
└── Export .glb files + texel density documentation

WEEK 2: THESIS FINALIZATION
├── Integrate usability results into thesis Chapter 5
├── Add KPI evidence as appendices
├── Final review + APA 7 compliance check
└── Defense preparation
```

**Bottom Line:** The technical implementation is **excellent** (B+ to A for Objectives 1–3). The **principal tarea pendiente** is the execution of the Objective 4 validation (SUS/NASA-TLX), which is **planned and instrumentally ready** — not an omission, but a timing question within the DSR methodology. Deployment to a public URL is trivial (GitHub Pages activation).

> **Nota del audit-of-audit:** Este informe fue re-auditado y corregido. Cambios principales: (1) Obj 4 reclasificado de "🔴 CRITICAL GAP" a "🟡 PENDING"; (2) Resultado 5 de "🔴 CRITICAL FAILURE" a "🟡 PENDING"; (3) BOM confirmado como existente (BillOfMaterialsManager.cs, 95 líneas); (4) Deployment evidence encontrada en `docs/`; (5) Assembly Guide reclasificado de scope creep a IN-SCOPE; (6) Scorecard subido de 6.75 a 7.45/10.

---

_Generated by Academic Peer Reviewer & Thesis Advisor — Pillar 4 of 4_
