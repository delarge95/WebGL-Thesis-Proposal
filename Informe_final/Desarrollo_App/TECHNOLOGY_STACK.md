# Technology Stack � WebGL Drone Visualization

> Inventario completo de tecnolog�as, herramientas, y servicios utilizados en el desarrollo del proyecto.

---

## 1. Motor de Desarrollo

### Unity 6 LTS

- **Versi�n**: Unity 6000.0.62f1
- **Render Pipeline**: Universal Render Pipeline (URP) 17.0.3
- **Target Platform**: WebGL 2.0 (IL2CPP -> WebAssembly)
- **UI Framework**: UI Toolkit (USS + UXML)
- **Paquetes Unity**:
  - `com.unity.render-pipelines.universal` � URP rendering
  - `com.unity.inputsystem` � New Input System
  - `com.unity.textmeshpro` � Advanced text rendering
  - `com.unity.cinemachine` � Camera system (orbit, pan, zoom)

---

## 2. Lenguajes de Programaci�n

| Lenguaje        | Uso                                    | Archivos                     |
| --------------- | -------------------------------------- | ---------------------------- |
| **C#**          | Core logic, managers, UI controllers   | 91 scripts (~14,778 l�neas)  |
| **HLSL**        | Custom shaders (URP compatible)        | 9 shaders (1,749 l�neas)     |
| **USS**         | Styles (Unity Style Sheets, CSS-like)  | 5 stylesheets (3,561 l�neas) |
| **UXML**        | UI layout (XML-based, HTML-like)       | 4 layouts (502 l�neas)       |
| **HTML/CSS/JS** | Landing page web                       | `docs/` folder               |
| **JSX**         | React components (Framer Motion cards) | `docs/src/`                  |

---

## 3. AI-Powered Development Tools

### 3.1 Antigravity (Google DeepMind)

**Rol**: Agente de desarrollo agentic AI principal � pareja de programaci�n.

- **Modo de operaci�n**: PLANNING -> EXECUTION -> VERIFICATION
- **Conversaciones de desarrollo**: 6+ sesiones documentadas
- **Artifacts generados**: Implementation plans, task lists, walkthroughs, changelogs
- **Capacidades utilizadas**:
  - Escritura y edici�n de c�digo (C#, USS, UXML, HLSL)
  - Investigaci�n de codebase (grep, file search, outline)
  - Ejecuci�n de comandos (git, npm, Unity)
  - Generaci�n de documentaci�n
  - Debugging y resoluci�n de errores

### 3.2 GitHub Copilot

**Rol**: Auto-completion y sugerencias de c�digo.

- **IDE integration**: Visual Studio / Rider
- **Uso**: Complemento de Antigravity para inline suggestions

### 3.3 Perplexity AI

**Rol**: Investigaci�n t�cnica y b�squeda de documentaci�n.

- **Uso**: Consultas sobre Unity API, WebGL compatibility, shader syntax, CSS properties
- **Prompts especializados**: Generados por Antigravity para investigaci�n profunda

### 3.4 Kimi K2

**Rol**: Dise�o de presentaci�n y material visual.

- **Uso**: Sistema de dise�o para presentaci�n
- **Output**: Documentaci�n de presentaci�n en `docs/`

### 3.5 OpenAI Codex

**Rol**: Agente de desarrollo para implementacion, mantenimiento documental y coordinacion tecnica del subsistema termico.

- **Uso**: arquitectura del sistema termico, implementacion C#, integracion shader/UI, workflows de verificacion y gobernanza documental
- **Output**: codigo de runtime, documentacion tecnica viva, indices de navegacion y workflows locales para mantenimiento ordenado

---

## 4. Custom Skills (Antigravity Extensions)

> 10 skills personalizadas creadas en `.agent/skills/` para extender las capacidades de Antigravity dentro del contexto del proyecto.

| Skill                   | Descripci�n                                                                          |
| ----------------------- | ------------------------------------------------------------------------------------ |
| **arch_guard**          | Roslyn Analyzer para enforcer reglas de arquitectura (God Classes, Coupling)         |
| **graph_builder**       | Construye Knowledge Graph estructural del proyecto Unity para prevenir alucinaciones |
| **scene_architect**     | Construcci�n program�tica de Unity Scene para configuraci�n perfecta                 |
| **ui_validator**        | Valida assets de UI Toolkit y genera bindings C# seguros                             |
| **unity_asset_auditor** | Escanea el proyecto por assets faltantes (fuentes, settings, paquetes)               |
| **unity_observer**      | Monitorea el estado del Unity Editor y errores en tiempo real                        |
| **unity_ui_pro**        | Dise�o de interfaces siguiendo Apple HIG y Material Design                           |
| **uss_linter**          | Escanea USS y UXML para compatibilidad Unity 6                                       |
| **webgl_optimizer**     | Valida y optimiza settings para builds WebGL de alto rendimiento                     |
| **webgl_scanner**       | Scanner avanzado para compatibilidad WebGL 2.0 (Shaders, Texturas, Audio)            |
| **wolfram-thermal-verifier** | Workflow local para validar ecuaciones, unidades y factores del subsistema termico |

---

## 5. RAG (Retrieval-Augmented Generation)

### Knowledge Items System

- **Ubicaci�n**: `<appDataDir>/knowledge/` (Antigravity managed)
- **Formato**: `metadata.json` + `artifacts/` por cada KI
- **Uso**: Contexto persistente entre conversaciones
  - Patrones de arquitectura del proyecto
  - Bugs resueltos y gotchas conocidos
  - Decisiones de dise�o y su rationale
  - Estado actual del proyecto

### Repo-local thermal knowledge

- **Ubicaci�n**: `.agent/workflows/thermal_documentation_maintenance.md` y `desarrollo/docs/sistema_termico/`
- **Uso**: mantener trazabilidad del subsistema termico, su politica documental, la guia 28+55 de retopologia, el handoff tecnico y el workflow de verificacion con WolframAlpha
- **Beneficio**: continuidad entre iteraciones sin depender solo del contexto de la sesion y sin perder la frontera entre camino oficial y herramientas experimentales

### Conversation Logs

- **Ubicaci�n**: `<appDataDir>/brain/<conversation-id>/`
- **Contenido**: Logs de tareas, artifacts, implementation plans, walkthroughs
- **Uso**: Recuperaci�n de contexto de sesiones anteriores

---

## 6. Knowledge Graph Technology

### graph_builder Skill

- **Prop�sito**: Mapeo estructural del proyecto Unity
- **Nodos**: Scripts, clases, namespaces, assets, escenas
- **Edges**: Dependencias, herencia, referencias, uso
- **Beneficio**: Prevenci�n de alucinaciones al definir relaciones reales del c�digo

---

## 7. MCP (Model Context Protocol)

### Servers Configurados

MCPs disponibles para ampliar el contexto del agente:

- **File System MCP**: Lectura/escritura de archivos del proyecto
- **Terminal MCP**: Ejecuci�n de comandos del sistema
- **Browser MCP**: Interacci�n con p�ginas web (testing visual)
- **Resource MCP**: Acceso a recursos espec�ficos del servidor

---

## 8. Version Control

### Git + GitHub

- **Repositorio**: `delarge95/WebGL-Thesis-Proposal`
- **Branch**: `feature/phase2-ux-redesign`
- **Hosting**: GitHub Pages (`/docs`)
- **Commits totales**: 232+ (verificado Mar 3, 2026)
- **Git LFS**: Configurado para assets binarios (modelos, texturas, audio)

### CI/CD

- **GitHub Pages**: Deploy autom�tico desde `/docs`
- **Workflows**: GitHub Actions (inicialmente incluido, luego removido para simplificar)

---

## 9. Web Technologies (Landing Page)

### Stack Principal

| Tecnolog�a        | Versi�n | Uso                                   |
| ----------------- | ------- | ------------------------------------- |
| **HTML5**         | �       | Estructura sem�ntica                  |
| **CSS3**          | �       | Estilos, animaciones, responsive      |
| **JavaScript**    | ES6+    | L�gica de interacci�n                 |
| **GSAP**          | 3.x     | Scrollytelling, animaciones avanzadas |
| **React**         | 18.x    | Componentes (migraci�n en progreso)   |
| **Framer Motion** | 10.x    | Micro-interacciones tem�ticas         |
| **Vite**          | 5.x     | Build tool y dev server               |
| **Mermaid**       | �       | Diagramas en documentaci�n            |

### Fonts

- **Inter** � Tipograf�a global (UI body text)
- **Space Grotesk** � T�tulos y headings

---

## 10. 3D / Shaders (Unity)

### Custom Shaders (HLSL / URP)

| Shader                   | T�cnica                           | Uso                    |
| ------------------------ | --------------------------------- | ---------------------- |
| `ClippableLit`           | Clip plane con stencil            | Corte transversal      |
| `XRay`                   | Fresnel + dual pass               | Transparencia t�cnica  |
| `Blueprint`              | Grid + outline overlay            | Vista de planos        |
| `Thermal`                | Gradiente animado de calor        | Mapa t�rmico           |
| `WireframeWebGL`         | Barycentric coords (WebGL compat) | Malla visible (WebGL)  |
| `Wireframe`              | Geometry shader                   | Malla visible (editor) |
| `SolidColor`             | Flat + outline                    | Vista s�lida           |
| `Ghosted`                | Fresnel + depth fade              | Transparencia fantasma |
| `AnimatedGradientSkybox` | Gradient lerp animado             | Cielo din�mico         |

---

## 11. Design System

### UI Toolkit (Unity)

- **Theme.uss**: Stylesheet principal (~1,808 l�neas)
- **MainTheme.uss**: Tema base (376 l�neas)
- **Hotspots.uss**: Estilos de hotspots (102 l�neas)
- **Overlays.uss**: Overlays y loading (116 l�neas)
- **MainLayout.uxml**: Layout principal (~394 l�neas)
- **Design Tokens**: Documentados en `DESIGN_TOKENS.md`
  - Colores: Monochrome (`#050505` bg, `#ffffff` accent)
  - Tipograf�a: Inter 14�20px, Space Grotesk 24�40px
  - Espaciado: Sistema 4/8/12/16/24/32px
  - Bordes: `8px�50%` border-radius
  - Motion: `0.15�0.4s` ease-out transitions

---

## 12. Project Management

### Herramientas de Planificaci�n

| Herramienta                   | Uso                                   |
| ----------------------------- | ------------------------------------- |
| **Antigravity task.md**       | Checklists por sesi�n                 |
| **implementation_plan.md**    | Planes t�cnicos detallados con review |
| **walkthrough.md**            | Documentaci�n post-ejecuci�n          |
| **HOJA_DE_RUTA.md**           | Roadmap general del proyecto          |
| **MICROINTERACTIONS_PLAN.md** | Plan de micro-interacciones (web)     |
| **PLAN_MODELADO_DRON.md**     | Plan de modelado 3D                   |

---

## 13. Hardware de Desarrollo

| Componente | Especificaci�n                  |
| ---------- | ------------------------------- |
| **GPU**    | NVIDIA GTX 980 Ti               |
| **RAM**    | 48 GB                           |
| **OS**     | Windows                         |
| **IDE**    | JetBrains Rider / Visual Studio |

---

## 14. Testing y Validaci�n

### M�todos Utilizados

- **Unity Play Mode**: Testing interactivo en editor
- **WebGL Build**: Build y deploy a GitHub Pages
- **Browser Testing**: Chrome DevTools, Antigravity browser tool
- **Visual Comparison**: Comparaci�n con web landing page para consistency
- **USS Liter**: Validaci�n de sintaxis de hojas de estilo

### Pendientes

- Pruebas de usabilidad (SUS, NASA-TLX)
- Performance profiling (Unity Profiler)
- Testing m�vil (touch, responsive)
