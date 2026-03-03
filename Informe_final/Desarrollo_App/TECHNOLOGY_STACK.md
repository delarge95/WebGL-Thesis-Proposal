# Technology Stack — WebGL Drone Visualization

> Inventario completo de tecnologías, herramientas, y servicios utilizados en el desarrollo del proyecto.

---

## 1. Motor de Desarrollo

### Unity 6 LTS
- **Versión**: Unity 6000.0.62f1
- **Render Pipeline**: Universal Render Pipeline (URP) 17.0.3
- **Target Platform**: WebGL 2.0 (IL2CPP → WebAssembly)
- **UI Framework**: UI Toolkit (USS + UXML)
- **Paquetes Unity**:
  - `com.unity.render-pipelines.universal` — URP rendering
  - `com.unity.inputsystem` — New Input System
  - `com.unity.textmeshpro` — Advanced text rendering
  - `com.unity.cinemachine` — Camera system (orbit, pan, zoom)

---

## 2. Lenguajes de Programación

| Lenguaje | Uso | Archivos |
|----------|-----|----------|
| **C#** | Core logic, managers, UI controllers | 91 scripts (~14,778 líneas) |
| **HLSL** | Custom shaders (URP compatible) | 9 shaders (1,749 líneas) |
| **USS** | Styles (Unity Style Sheets, CSS-like) | 5 stylesheets (3,561 líneas) |
| **UXML** | UI layout (XML-based, HTML-like) | 4 layouts (502 líneas) |
| **HTML/CSS/JS** | Landing page web | `docs/` folder |
| **JSX** | React components (Framer Motion cards) | `docs/src/` |

---

## 3. AI-Powered Development Tools

### 3.1 Antigravity (Google DeepMind)

**Rol**: Agente de desarrollo agentic AI principal — pareja de programación.

- **Modo de operación**: PLANNING → EXECUTION → VERIFICATION
- **Conversaciones de desarrollo**: 6+ sesiones documentadas
- **Artifacts generados**: Implementation plans, task lists, walkthroughs, changelogs
- **Capacidades utilizadas**:
  - Escritura y edición de código (C#, USS, UXML, HLSL)
  - Investigación de codebase (grep, file search, outline)
  - Ejecución de comandos (git, npm, Unity)
  - Generación de documentación
  - Debugging y resolución de errores

### 3.2 GitHub Copilot

**Rol**: Auto-completion y sugerencias de código.

- **IDE integration**: Visual Studio / Rider
- **Uso**: Complemento de Antigravity para inline suggestions

### 3.3 Perplexity AI

**Rol**: Investigación técnica y búsqueda de documentación.

- **Uso**: Consultas sobre Unity API, WebGL compatibility, shader syntax, CSS properties
- **Prompts especializados**: Generados por Antigravity para investigación profunda

### 3.4 Kimi K2

**Rol**: Diseño de presentación y material visual.

- **Uso**: Sistema de diseño para presentación
- **Output**: Documentación de presentación en `docs/`

---

## 4. Custom Skills (Antigravity Extensions)

> 10 skills personalizadas creadas en `.agent/skills/` para extender las capacidades de Antigravity dentro del contexto del proyecto.

| Skill | Descripción |
|-------|-------------|
| **arch_guard** | Roslyn Analyzer para enforcer reglas de arquitectura (God Classes, Coupling) |
| **graph_builder** | Construye Knowledge Graph estructural del proyecto Unity para prevenir alucinaciones |
| **scene_architect** | Construcción programática de Unity Scene para configuración perfecta |
| **ui_validator** | Valida assets de UI Toolkit y genera bindings C# seguros |
| **unity_asset_auditor** | Escanea el proyecto por assets faltantes (fuentes, settings, paquetes) |
| **unity_observer** | Monitorea el estado del Unity Editor y errores en tiempo real |
| **unity_ui_pro** | Diseño de interfaces siguiendo Apple HIG y Material Design |
| **uss_linter** | Escanea USS y UXML para compatibilidad Unity 6 |
| **webgl_optimizer** | Valida y optimiza settings para builds WebGL de alto rendimiento |
| **webgl_scanner** | Scanner avanzado para compatibilidad WebGL 2.0 (Shaders, Texturas, Audio) |

---

## 5. RAG (Retrieval-Augmented Generation)

### Knowledge Items System

- **Ubicación**: `<appDataDir>/knowledge/` (Antigravity managed)
- **Formato**: `metadata.json` + `artifacts/` por cada KI
- **Uso**: Contexto persistente entre conversaciones
  - Patrones de arquitectura del proyecto
  - Bugs resueltos y gotchas conocidos
  - Decisiones de diseño y su rationale
  - Estado actual del proyecto

### Conversation Logs

- **Ubicación**: `<appDataDir>/brain/<conversation-id>/`
- **Contenido**: Logs de tareas, artifacts, implementation plans, walkthroughs
- **Uso**: Recuperación de contexto de sesiones anteriores

---

## 6. Knowledge Graph Technology

### graph_builder Skill

- **Propósito**: Mapeo estructural del proyecto Unity
- **Nodos**: Scripts, clases, namespaces, assets, escenas
- **Edges**: Dependencias, herencia, referencias, uso
- **Beneficio**: Prevención de alucinaciones al definir relaciones reales del código

---

## 7. MCP (Model Context Protocol)

### Servers Configurados

MCPs disponibles para ampliar el contexto del agente:

- **File System MCP**: Lectura/escritura de archivos del proyecto
- **Terminal MCP**: Ejecución de comandos del sistema
- **Browser MCP**: Interacción con páginas web (testing visual)
- **Resource MCP**: Acceso a recursos específicos del servidor

---

## 8. Version Control

### Git + GitHub

- **Repositorio**: `delarge95/WebGL-Thesis-Proposal`
- **Branch**: `feature/phase2-ux-redesign`
- **Hosting**: GitHub Pages (`/docs`)
- **Commits totales**: 232+ (verificado Mar 3, 2026)
- **Git LFS**: Configurado para assets binarios (modelos, texturas, audio)

### CI/CD

- **GitHub Pages**: Deploy automático desde `/docs`
- **Workflows**: GitHub Actions (inicialmente incluido, luego removido para simplificar)

---

## 9. Web Technologies (Landing Page)

### Stack Principal

| Tecnología | Versión | Uso |
|------------|---------|-----|
| **HTML5** | — | Estructura semántica |
| **CSS3** | — | Estilos, animaciones, responsive |
| **JavaScript** | ES6+ | Lógica de interacción |
| **GSAP** | 3.x | Scrollytelling, animaciones avanzadas |
| **React** | 18.x | Componentes (migración en progreso) |
| **Framer Motion** | 10.x | Micro-interacciones temáticas |
| **Vite** | 5.x | Build tool y dev server |
| **Mermaid** | — | Diagramas en documentación |

### Fonts
- **Inter** — Tipografía global (UI body text)
- **Space Grotesk** — Títulos y headings

---

## 10. 3D / Shaders (Unity)

### Custom Shaders (HLSL / URP)

| Shader | Técnica | Uso |
|--------|---------|-----|
| `ClippableLit` | Clip plane con stencil | Corte transversal |
| `XRay` | Fresnel + dual pass | Transparencia técnica |
| `Blueprint` | Grid + outline overlay | Vista de planos |
| `Thermal` | Gradiente animado de calor | Mapa térmico |
| `WireframeWebGL` | Barycentric coords (WebGL compat) | Malla visible (WebGL) |
| `Wireframe` | Geometry shader | Malla visible (editor) |
| `SolidColor` | Flat + outline | Vista sólida |
| `Ghosted` | Fresnel + depth fade | Transparencia fantasma |
| `AnimatedGradientSkybox` | Gradient lerp animado | Cielo dinámico |

---

## 11. Design System

### UI Toolkit (Unity)

- **Theme.uss**: Stylesheet principal (~1,808 líneas)
- **MainTheme.uss**: Tema base (376 líneas)
- **Hotspots.uss**: Estilos de hotspots (102 líneas)
- **Overlays.uss**: Overlays y loading (116 líneas)
- **MainLayout.uxml**: Layout principal (~394 líneas)
- **Design Tokens**: Documentados en `DESIGN_TOKENS.md`
  - Colores: Monochrome (`#050505` bg, `#ffffff` accent)
  - Tipografía: Inter 14–20px, Space Grotesk 24–40px
  - Espaciado: Sistema 4/8/12/16/24/32px
  - Bordes: `8px–50%` border-radius
  - Motion: `0.15–0.4s` ease-out transitions

---

## 12. Project Management

### Herramientas de Planificación

| Herramienta | Uso |
|-------------|-----|
| **Antigravity task.md** | Checklists por sesión |
| **implementation_plan.md** | Planes técnicos detallados con review |
| **walkthrough.md** | Documentación post-ejecución |
| **HOJA_DE_RUTA.md** | Roadmap general del proyecto |
| **MICROINTERACTIONS_PLAN.md** | Plan de micro-interacciones (web) |
| **PLAN_MODELADO_DRON.md** | Plan de modelado 3D |

---

## 13. Hardware de Desarrollo

| Componente | Especificación |
|------------|----------------|
| **GPU** | NVIDIA GTX 980 Ti |
| **RAM** | 48 GB |
| **OS** | Windows |
| **IDE** | JetBrains Rider / Visual Studio |

---

## 14. Testing y Validación

### Métodos Utilizados

- **Unity Play Mode**: Testing interactivo en editor
- **WebGL Build**: Build y deploy a GitHub Pages
- **Browser Testing**: Chrome DevTools, Antigravity browser tool
- **Visual Comparison**: Comparación con web landing page para consistency
- **USS Liter**: Validación de sintaxis de hojas de estilo

### Pendientes

- Pruebas de usabilidad (SUS, NASA-TLX)
- Performance profiling (Unity Profiler)
- Testing móvil (touch, responsive)
