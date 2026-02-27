# 🚀 Master Audit Prompts: The 4 Pillars of the Drone Viewer App

This document contains highly specialized, context-aware prompt templates. You can use these to instruct me (or any other Advanced AI like Claude 3.5 Sonnet / GPT-4o) to execute a deep, rigorous audit of the project.

Each prompt is designed to scan the codebase, external references, and configuration files to guarantee that the final product is a **perfect, production-ready scientific MVP**.

---

## 🏗️ Pillar 1: Technical & Architecture Audit Prompt
*Use this to ensure the code is maintainable, scalable, and follows Senior C# Unity Standards.*

**Prompt:**
> "Act as a Lead Unity Engineer and Technical Architect. Perform a deep Technical Audit of the current Unity project, focusing specifically on the C# scripts inside `Assets/Scripts/`. 
> 
> **Instructions:**
> 1. Read the core systems: `SelectionManager.cs`, `UIManager.cs`, `OrbitCameraController.cs`, and the EventBus system.
> 2. Look for **God Classes**, tight coupling, memory leaks (unsubscribed events), and violations of SOLID principles.
> 3. Verify if we are correctly using Dependency Injection or singletons, and if `UI Toolkit` events are properly managed.
> 4. Generate an `ARCHITECTURE_AUDIT_REPORT.md` that lists every major architectural flaw, categorized by severity (Critical, High, Medium, Low), and provide a concrete refactoring plan to fix them."

---

## 🎨 Pillar 2: UX/UI Design & Cognitive Load Audit Prompt
*Use this to ensure the interface is world-class, accessible, and minimizes mental friction.*

**Prompt:**
> "Act as a Senior UX/UI Researcher and Product Designer. Read the thesis objective from `Propuesta/final_proposal.tex` which emphasizes reducing **cognitive friction** for engineers. Read `External_docs/Guía Completa de Diseño UI_UX para Aplicaciones Mó.md` for styling logic.
> 
> **Instructions:**
> 1. Audit the `Assets/Scripts/UI/Layouts/MainLayout.uxml` and `Assets/UI/Styles/Theme.uss` files.
> 2. Evaluate the interface based on 'Miller's Law' (working memory limits) and 'Fitts's Law' (target size/distance).
> 3. Does the current layout impose high cognitive load? Are there elements that overwhelm the user?
> 4. Are the constraints (8pt grid, 44px min touch target, proper flexbox layouts) correctly implemented?
> 5. Generate a `UX_UI_AUDIT_REPORT.md` detailing every design violation and structural UX flaw, followed by a precise plan to implement a minimalist, 'Mode-based' UI."

---

## ⚡ Pillar 3: WebGL Optimization & Performance Audit Prompt
*Use this to ensure the app hits strict technical KPIs (>30FPS, <150MB heap, extremely fast load times).*

**Prompt:**
> "Act as a Senior Graphics Programmer and WebGL Optimization Expert. Read the specific KPIs from `Propuesta/final_proposal.tex` (Target: < 100,000 polygons, > 30 FPS on mid-range mobile, < 3s shell load, < 50MB build).
> 
> **Instructions:**
> 1. Audit the `Assets/WebGLTemplates/`, `ProjectSettings/ProjectSettings.asset`, and `ProjectSettings/QualitySettings.asset`.
> 2. Look for pipeline bottlenecks: Draw calls, Material properties, Texture compression settings (ASTC/DXT), and Lighting configurations.
> 3. Evaluate the WebAssembly heap size, memory allocations, and Asset Bundle strategy (if any).
> 4. Are we baking correctly? Is the Unlit/PBR mix optimal?
> 5. Generate a `PERFORMANCE_AUDIT_REPORT.md` listing every graphics and memory inefficiency, and output a strictly prioritized step-by-step optimization plan detailing exactly which tools and settings to change."

---

## 🎓 Pillar 4: Academic & Thesis Alignment Audit Prompt
*Use this to guarantee the project perfectly fulfills the promises made to the university.*

**Prompt:**
> "Act as an Academic Peer Reviewer and Thesis Advisor from the Engineering Faculty.
> 
> **Instructions:**
> 1. Read `Propuesta/final_proposal.tex`, specifically the 'Resumen', 'Planteamiento del Problema', and 'Objetivos'.
> 2. Compare the promises made in the text with the current state of the app (the features implemented in the codebase).
> 3. Identify any **Scope Creep** (features we built that don't serve the thesis) and any **Missing Deliverables** (promises made in the proposal that we haven't implemented yet, e.g., SUS usability metrics implementation, specific technical artifacts, or the 90% polygon reduction pipeline documentation).
> 4. Generate an `ACADEMIC_ALIGNMENT_REPORT.md` that highlights the gaps between the actual product and the written proposal, providing an actionable roadmap to ensure the project passes the final academic evaluation with distinction."
