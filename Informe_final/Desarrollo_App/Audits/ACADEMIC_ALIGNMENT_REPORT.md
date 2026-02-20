# 🎓 Academic Alignment Audit Report

**Date:** February 2026  
**Role:** Academic Peer Reviewer & Thesis Advisor  
**Scope:** Thesis Proposal vs. Current Codebase (MVP)

---

## 🔍 Executive Summary
The project has achieved remarkable technical fidelity and architectural sophistication. However, from an academic standpoint, the project is currently leaning too heavily towards pure software engineering and straying slightly from the explicit promises made in the thesis proposal's "Objetivos" and "Planteamiento del Problema" methodology. 

To pass the thesis evaluation with distinction, the project must shift focus from building new features to **empirical validation** and **methodology documentation**.

---

## 🚫 Missing Deliverables (The Gap)

The following items are explicitly promised in `objetivos.tex` and `resumen.tex` but are currently missing or undocumented:

### 1. Empirical Usability & Cognitive Testing (CRITICAL)
*   **The Promise**: "...pruebas de usabilidad (System Usability Scale - SUS) y cuestionarios de carga mental (NASA-TLX simplificado) con ingenieros/técnicos (N=8-12)..."
*   **The Current State**: The UI has been polished, but there is no mechanism yet for collecting this data.
*   **Actionable Fix**: Create a Google Form (or embedded Typeform) containing the standard 10-question SUS scale and the 6-dimension NASA-TLX scale. Recruit 8-12 participants immediately to interact with the deployed WebGL build.

### 2. Technical Art Pipeline Documentation (HIGH)
*   **The Promise**: "Diseñar un pipeline de optimización de activos 3D que reduzca la carga poligonal... en un 90%... mediante técnicas de baking de mapas normales y retopología."
*   **The Current State**: The codebase reflects the *result* (an optimized drone), but the Academic Jurors evaluate the *process*. 
*   **Actionable Fix**: A dedicated technical document (perhaps an appendix or a specific chapter in `Informe_final`) must be written showing before/after wireframes, polygon counts, normal map baking settings from Blender/Substance Painter, to prove the 90% reduction metric.

### 3. Quantitative Performance Profiling (HIGH)
*   **The Promise**: "...herramientas de perfilado (Unity Profiler)... Time to Interactive (TTI) < 5 segundos en redes 4G."
*   **The Current State**: We have audited the settings, but we need hard data graphs for the final document.
*   **Actionable Fix**: Run the Unity Profiler on the WebGL build. Capture screenshots showing memory heap allocation, Draw Calls, and Frame Time (< 33.33ms) to physically prove the "> 30 FPS" KPI claim in the final thesis text.

---

## ⚠️ Scope Creep Identification

*   **Complex Environment Presets**: The logic for 5 different lighting environments (Studio, Sunset, Blueprint) and custom rotation/intensity is impressive, but it is technically beyond the scope of a pure "Technical/Structural Drone Viewer". 
    *   *Advice*: Freeze development on the Environment/Lighting controls. They are more than sufficient. Focus entirely on the missing academic deliverables.

---

## 🗺️ Academic Roadmap to Defense

To defend this thesis successfully in February:
1.  **Stop Feature Development**: After the current Phase 4 (WebGL Optimization) and Phase 3 (Refactoring) are complete, NO MORE FEATURES.
2.  **Launch the Survey**: Deploy the WebGL app to GitHub Pages immediately, and send the link alongside the SUS / NASA-TLX forms to the target demographic.
3.  **Synthesize Results**: Use the survey data and Unity Profiler screenshots to write the `Resultados` chapter, directly referring back to the KPIs established in the objectives.
