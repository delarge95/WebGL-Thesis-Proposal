# Thesis Defense Presentation Outline

## WebGL Drone Viewer: Interactive 3D Hardware Visualization

### Slide 1: Title
- **Title**: Prototipo WebGL de Visualización 3D Interactiva para Hardware de Drones
- **Author**: Alexander Woodcock Salomón
- **Program**: Ingeniería Multimedia
- **Advisor**: Deivid Enrique Triviño Lozada
- **Date**: 2025

---

### Slide 2: Problem Statement
- **Challenge**: Technical information is traditionally communicated through static 2D images
- **Limitations**:
  - Difficult to understand 3D spatial relationships
  - No interactivity or exploration
  - Cognitive overload when processing complex assemblies
- **Visual**: Split image showing 2D manual vs 3D interactive viewer

---

### Slide 3: Objectives
- **General Objective**: Develop PoC WebGL prototype for 3D drone visualization
- **Specific Objectives**:
  1. Create optimized 3D pipeline (retopology, baking, compression)
  2. Implement 7 visualization modes
  3. Develop engineer assembly tools
  4. Validate usability with SUS questionnaire
- **Visual**: Objective icons with checkmarks

---

### Slide 4: Theoretical Framework
- **Key Concepts**:
  - Cognitive Load Theory (Sweller, 1988)
  - Nielsen's Usability Heuristics
  - WebGL & WebAssembly technology
  - 3D Optimization Pipeline
- **Visual**: Mind map of concepts

---

### Slide 5: Methodology
- **Approach**: Design Science Research (DSR) + Agile (Scrum)
- **Phases**:
  1. Research & Planning
  2. 3D Asset Pipeline
  3. Unity development
  4. Testing & Validation
- **Visual**: Timeline/Gantt chart

---

### Slide 6: Technology Stack
| Category | Technology |
|----------|------------|
| Engine | Unity 6.0 LTS |
| Render | URP (WebGL 2.0) |
| UI | UI Toolkit |
| Language | C# 11 / WASM |
| Shaders | HLSL |
| 3D Tools | Blender |

---

### Slide 7: Architecture Overview
- **Design Patterns**:
  - Singleton/PersistentSingleton
  - Event Bus (Pub/Sub)
  - State Machine
  - ScriptableObjects
- **Visual**: Mermaid architecture diagram

---

### Slide 8: Core Features - View Modes
- **7 Visualization Modes**:
  1. Realistic (PBR)
  2. X-Ray (Fresnel)
  3. Blueprint (Grid)
  4. Solid Color
  5. Wireframe
  6. Ghosted
  7. Thermal
- **Visual**: Grid showing each mode screenshot

---

### Slide 9: Core Features - Interaction
- **Exploded View**: Animated part separation
- **Cross Section**: X/Y/Z cutting planes
- **Part Selection**: Click-to-select with highlighting
- **Drone Simulation**: On/Off states, propeller animation
- **Visual**: GIF/video of interactions

---

### Slide 10: Engineer Tools
| Tool | Function |
|------|----------|
| Assembly Guide | Step-by-step instructions |
| Measurement | Distance, angle, radius |
| Connection Points | Screw/wire visualization |
| Bill of Materials | Part list + CSV export |
| Annotations | 3D notes |
| Checklist | Verification tracking |
- **Visual**: Tool interface screenshots

---

### Slide 11: Technical Metrics
| Metric | Target | Achievement |
|--------|--------|-------------|
| Scripts | - | 70+ |
| Lines of Code | - | ~10,000 |
| Shaders | - | 7 HLSL |
| Managers | - | 18 |
| Load Time | <10s | TBD |
| FPS | >30 | TBD |

---

### Slide 12: Custom Shaders
- **7 Custom HLSL Shaders**:
  - ClippableLit (cross-section)
  - XRay (fresnel transparency)
  - Blueprint (technical grid)
  - Thermal (heat gradient)
  - Wireframe (geometry shader)
  - SolidColor, Ghosted
- **Visual**: Shader code snippet + result

---

### Slide 13: 3D Optimization Pipeline
- **Workflow**:
  1. CAD → Blender (retopology)
  2. UV unwrapping + texel density
  3. Normal map baking
  4. Texture compression (ASTC/Basis)
  5. Unity import + LODs
- **Visual**: Before/after polygon count

---

### Slide 14: User Interface
- **Design System**: Glassmorphism
- **Features**:
  - Dark theme with blur effects
  - Responsive layout
  - Touch-friendly controls
  - Accessibility options
- **Visual**: UI screenshot

---

### Slide 15: Live Demo
- **Demo Flow**:
  1. Initial load
  2. Camera navigation
  3. Part selection
  4. View mode switching
  5. Exploded view
  6. Cross section
  7. Engineer tools
- **Duration**: 3-5 minutes

---

### Slide 16: Validation Plan
- **Usability Testing**:
  - Method: SUS (System Usability Scale)
  - Sample: 8-12 participants
  - Target: SUS score > 68 (above average)
- **Performance Testing**:
  - Unity Profiler
  - Browser DevTools
  - Lighthouse audit
- **Cognitive Load**: NASA-TLX questionnaire

---

### Slide 17: Expected Results
1. Functional WebGL prototype (URL)
2. 7 shaders + 70+ scripts
3. Complete documentation
4. Usability evaluation report
5. Replicable optimization pipeline

---

### Slide 18: Conclusions
- Successfully implemented interactive 3D viewer
- Proven viability of WebGL for technical documentation
- Comprehensive architecture ready for extension
- Documentation serves as reference for future projects

---

### Slide 19: Future Work
- VR/AR integration
- Real-time collaborative viewing
- AI-powered part recognition
- Multi-language support
- Mobile-optimized version

---

### Slide 20: Thank You
- **Questions?**
- **Contact**: [email]
- **Repository**: github.com/delarge95/WebGL_tesis
- **Demo URL**: [TBD]

---

## Presentation Tips

### Time Management
- Title + Intro: 2 min
- Problem + Objectives: 3 min
- Theory + Methodology: 4 min
- Implementation: 8 min
- Demo: 5 min
- Results + Conclusions: 3 min
- **Total**: ~25 minutes + Q&A

### Key Points to Emphasize
1. Innovation: WebGL for technical documentation
2. Scope: 70+ scripts, 7 shaders, complete system
3. Practical value: Replicable pipeline
4. User-centered: Usability validation

### Backup Slides
- Detailed code architecture
- Shader implementation details
- Full technology comparison
- Extended demo scenarios
