# PLAN DE TRABAJO FINAL — Tesis Ingeniería Multimedia UNAD

**Fecha de creación:** 3 de marzo de 2026  
**Autor:** Alexander Woodcock Salomón  
**Asesor:** Deivid Enrique Triviño Lozada  
**Título:** _Diseño y Desarrollo de un Prototipo Web 3D Interactivo para la Visualización Técnica y Análisis Estructural de Hardware de Alto Rendimiento_

---

## 1. ESTADO ACTUAL — MÉTRICAS VERIFICADAS

### 1.1 Codebase (verificado 3 marzo 2026)

| Métrica              | Valor verificado     | Fuente anterior                       | Δ                           |
| -------------------- | -------------------- | ------------------------------------- | --------------------------- |
| Scripts C#           | **91**               | Arquitectura: 94, Manual Técnico: 65+ | -3 / +26                    |
| Líneas C#            | **~14,778**          | CODEBASE_TRUTH: ~14,202               | +576                        |
| Shaders custom       | **9** (1,749 líneas) | Documentado: 8                        | +1 (AnimatedGradientSkybox) |
| Commits Git          | **232**              | CHANGELOG: 88                         | **+144 sin documentar**     |
| ScriptableObjects    | **11** DronePartData | Documentado: 10                       | +1 (VTX)                    |
| Archivos USS         | **5** (3,561 líneas) | -                                     | -                           |
| Archivos UXML        | **4** (502 líneas)   | -                                     | -                           |
| Escenas Unity        | **3**                | -                                     | -                           |
| Modelos 3D (FBX/GLB) | **0 en repo**        | Tesis promete GLB                     | ⚠️                          |
| Archivos audio       | **0**                | AudioManager existe                   | ⚠️                          |
| Prefabs              | **0 visibles**       | Posiblemente en .unity                | -                           |

### 1.2 Timeline del proyecto

| Fecha      | Commits | Fase                                    |
| ---------- | ------- | --------------------------------------- |
| 2025-12-04 | 6       | Fase 0: Inicialización                  |
| 2025-12-05 | 24      | Fase 1: Arquitectura base               |
| 2025-12-08 | 3       | Fase 2: Contenido                       |
| 2025-12-11 | 3       | Fase 2: Refinamiento                    |
| 2026-02-10 | 7       | Fase 3-5: Managers avanzados            |
| 2026-02-11 | 17      | Fase 5-6: UI Toolkit + shaders          |
| 2026-02-12 | 11      | Fase 6-7: Optimización                  |
| 2026-02-18 | 27      | Fase 7-8: WebGL + deploy                |
| 2026-02-19 | 8       | Fase 8: Pulido                          |
| 2026-02-20 | 3       | CHANGELOG/BITACORA dejan de registrar   |
| 2026-02-23 | 23      | FASE 1 refactor (C01-C05)               |
| 2026-02-24 | 5       | FASE 1 cont.                            |
| 2026-02-25 | 4       | FASE 2 UX (H01-H05)                     |
| 2026-02-26 | 18      | FASE 2 cont. (H06-H11)                  |
| 2026-02-27 | 23      | FASE 3 features (F3-00 a F3-07)         |
| 2026-03-02 | 48      | FASE 3 cont. (F3-08 a F3-13) + bugfixes |
| **TOTAL**  | **232** | **3 meses, 16 días activos**            |

### 1.3 Distribución de scripts por directorio

| Carpeta            | Archivos |
| ------------------ | -------- |
| Core/Managers      | 28       |
| UI/ProceduralIcons | 15       |
| UI/Panels          | 11       |
| UI                 | 10       |
| Core/Utils         | 8        |
| Editor/Antigravity | 5        |
| Core/Content       | 4        |
| Core/Events        | 3        |
| Core/Data          | 2        |
| Editor             | 2        |
| Tests/Editor       | 2        |
| UI/Generated       | 1        |

---

## 2. INCONGRUENCIAS DETECTADAS (12 hallazgos)

### 🔴 CRÍTICAS (impactan entregables)

| #    | Documento           | Problema                                                                                                                                                     | Acción requerida                 |
| ---- | ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------- |
| I-01 | CHANGELOG.md        | Solo 88 de 232 commits. Falta TODO Phase 3 (C01-C05, H01-H11, F3-00 a F3-13, bugfixes) = 144 commits sin registrar                                           | Regenerar desde `git log`        |
| I-02 | BITACORA.md         | Detiene en Feb 20, 2026. Falta documentación de Phase 3 completa                                                                                             | Agregar entradas Feb 23 – Mar 3  |
| I-03 | TECHNOLOGY_STACK.md | Dice "Unity 6.3 (6000.3.x)" — real es **Unity 6000.0.62f1**                                                                                                  | Corregir versión                 |
| I-04 | Audit Reports (5)   | Reportan C-01 (VisualMode duplicado), C-02 (triple event), H-03 (UIModeController God Class 561 líneas) como CRÍTICOS — **todos ya solucionados en Phase 3** | Marcar como RESUELTOS o archivar |

### 🟡 IMPORTANTES (afectan calidad académica)

| #    | Documento          | Problema                                                                         | Acción requerida                  |
| ---- | ------------------ | -------------------------------------------------------------------------------- | --------------------------------- |
| I-05 | 01_Arquitectura.md | Reporta 94 scripts, ~12,750 líneas — real: 91 scripts, ~14,778 líneas            | Actualizar métricas               |
| I-06 | manual_tecnico.tex | Menciona "65+ scripts" — real: 91                                                | Actualizar                        |
| I-07 | INDICE_V7          | Capítulos 4-7 completamente vacíos; nivel PhD excesivo para pregrado             | Reducir alcance, llenar contenido |
| I-08 | Modelos 3D         | Tesis promete "Archivos .glb disponibles" pero **0 archivos GLB en repositorio** | Exportar GLBs o eliminar promesa  |

### 🟢 MENORES (documentación incompleta)

| #    | Documento     | Problema                                                                                    | Acción requerida                    |
| ---- | ------------- | ------------------------------------------------------------------------------------------- | ----------------------------------- |
| I-09 | SUS/NASA-TLX  | Instrumentos creados (CUESTIONARIO_SUS.md, CUESTIONARIO_NASA_TLX.md) pero **0 ejecuciones** | Ejecutar con N=8-12 participantes   |
| I-10 | KPIs          | Sin reportes formales de FPS, draw calls, VRAM, TTI                                         | Capturar métricas desde build WebGL |
| I-11 | Audio         | AudioManager.cs existe pero 0 archivos de audio en proyecto                                 | Documentar como feature pendiente   |
| I-12 | CRONOGRAMA.md | Plan de 24 semanas / 550 horas — proyecto real duró ~13 semanas con 16 días activos         | Ajustar a cronograma real           |

---

## 3. INVENTARIO DE ENTREGABLES

### 3.1 Documentos que YA EXISTEN

| Archivo                            | Ubicación                        | Estado                                 | Líneas |
| ---------------------------------- | -------------------------------- | -------------------------------------- | ------ |
| informe_final.tex                  | Informe_final/                   | Esqueleto APA 7, poca contenido real   | 541    |
| manual_tecnico.tex                 | Informe_final/Manual_tecnico/    | Bastante completo                      | 313    |
| manual_usuario.tex                 | Informe_final/Manual_de_usuario/ | Bastante completo                      | 268    |
| final_proposal.tex                 | Propuesta/                       | Propuesta original completa            | -      |
| references.bib                     | Propuesta/                       | Bibliografía base                      | -      |
| CHANGELOG.md                       | Informe_final/Desarrollo_App/    | Incompleto (88/232 commits)            | ~400   |
| BITACORA.md                        | Informe_final/Desarrollo_App/    | Incompleta (para en Feb 20)            | ~300   |
| TECHNOLOGY_STACK.md                | Informe_final/Desarrollo_App/    | Error de versión Unity                 | ~200   |
| 01_Arquitectura_del_Sistema.md     | Informe_final/Desarrollo_App/    | Muy completo, métricas desactualizadas | 1,060  |
| 02_Referencia_Tecnica_Modulos.md   | Informe_final/Desarrollo_App/    | Completo                               | ~800   |
| 03_Pipeline_Renderizado_Shaders.md | Informe_final/Desarrollo_App/    | Completo                               | ~600   |
| CUESTIONARIO_SUS.md                | Informe_final/validacion/        | Listo, sin ejecutar                    | -      |
| CUESTIONARIO_NASA_TLX.md           | Informe_final/validacion/        | Listo, sin ejecutar                    | -      |
| ACADEMIC_ALIGNMENT_REPORT.md       | Informe_final/Desarrollo_App/    | Stale (pre-Phase 3)                    | ~400   |
| ARCHITECTURE_AUDIT_REPORT.md       | Informe_final/Desarrollo_App/    | Stale (pre-Phase 3)                    | ~600   |
| PERFORMANCE_AUDIT_REPORT.md        | Informe_final/Desarrollo_App/    | Stale (pre-Phase 3)                    | ~400   |
| UX_UI_AUDIT_REPORT.md              | Informe_final/Desarrollo_App/    | Stale (pre-Phase 3)                    | ~500   |
| REMEDIATION_PLAN.md                | Informe_final/Desarrollo_App/    | Mayormente ejecutado                   | ~300   |
| 6 Antigravity Reports              | Informe_final/Desarrollo_App/    | Investigación de drones                | ~1,200 |

### 3.2 Documentos que FALTAN CREAR

| Documento                                       | Prioridad     | Descripción                                                                        |
| ----------------------------------------------- | ------------- | ---------------------------------------------------------------------------------- |
| **Cap. 4: Marco Metodológico** (en LaTeX)       | 🔴 CRÍTICA    | Tipo de investigación, enfoque, fases, instrumentos                                |
| **Cap. 5: Desarrollo del Prototipo** (en LaTeX) | 🔴 CRÍTICA    | Arquitectura, implementación, pipeline, shaders — hay MD existentes para convertir |
| **Cap. 6: Resultados y Análisis** (en LaTeX)    | 🔴 CRÍTICA    | Métricas, rendimiento, usabilidad (requiere datos SUS/NASA-TLX)                    |
| **Cap. 7: Conclusiones** (en LaTeX)             | 🔴 CRÍTICA    | Objetivos cumplidos, aportes, trabajo futuro                                       |
| **Reporte de KPIs**                             | 🟡 IMPORTANTE | FPS, draw calls, VRAM, TTI medidos en build real                                   |
| **Reporte Pipeline 3D**                         | 🟡 IMPORTANTE | Before/after polígonos, texel density, LODs                                        |
| **Bibliografía actualizada** (references.bib)   | 🟡 IMPORTANTE | Ampliar con fuentes de Three.js, WebGL, URP                                        |
| **Anexo: CHANGELOG completo**                   | 🟡 IMPORTANTE | Regenerar desde git log                                                            |
| **Anexo: Código fuente relevante**              | 🟢 MENOR      | Fragmentos clave del código                                                        |
| **Presentación de sustentación**                | 🟢 MENOR      | Slides para defensa (existe outline en docs/)                                      |

---

## 4. PLAN DE TRABAJO DETALLADO

### BLOQUE A: Corrección de Incongruencias (2-3 horas)

> Prioridad: HACER PRIMERO — los datos corregidos alimentan todo lo demás

| Tarea                               | Tiempo est. | Detalle                                                                                                                                  |
| ----------------------------------- | ----------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| A-01: Regenerar CHANGELOG.md        | 45 min      | `git log --oneline --reverse` → formatear los 232 commits por fase con fechas                                                            |
| A-02: Actualizar BITACORA.md        | 30 min      | Agregar entradas para Feb 23 – Mar 3 (Phase 3 completo: FASE 1 refactor, FASE 2 UX, FASE 3 features)                                     |
| A-03: Corregir TECHNOLOGY_STACK.md  | 10 min      | "Unity 6.3" → "Unity 6000.0.62f1", verificar otras versiones                                                                             |
| A-04: Actualizar 01_Arquitectura.md | 30 min      | 94→91 scripts, ~12,750→~14,778 líneas, reflejar refactoring Phase 3 (BaseModeHandler, eliminación God Class)                             |
| A-05: Marcar auditorías como stale  | 15 min      | Agregar header a ARCHITECTURE_AUDIT, PERFORMANCE_AUDIT, UX_UI_AUDIT indicando que hallazgos C-01, C-02, H-03 fueron resueltos en Phase 3 |
| A-06: Actualizar manual_tecnico.tex | 20 min      | 65+→91 scripts, agregar nuevos módulos de Phase 3                                                                                        |

### BLOQUE B: Captura de Métricas Reales (1-2 horas)

> Prioridad: HACER ANTES de escribir Cap. 6

| Tarea                                 | Tiempo est. | Detalle                                                                     |
| ------------------------------------- | ----------- | --------------------------------------------------------------------------- |
| B-01: Build WebGL de producción       | 30 min      | Build limpio desde Unity, registrar tamaño final (.wasm, .data, .framework) |
| B-02: Capturar FPS y draw calls       | 20 min      | Chrome DevTools → Performance tab, Stats overlay en build WebGL             |
| B-03: Medir TTI (Time to Interactive) | 15 min      | Lighthouse o Performance API, registrar tiempo de carga                     |
| B-04: Capturar VRAM usage             | 15 min      | Unity Profiler o WebGL memory stats                                         |
| B-05: Documentar polygon counts       | 15 min      | Stats en editor: total tris, batches, SetPass calls por vista               |
| B-06: Crear tabla comparativa         | 15 min      | Antes/después de optimización (Phase 7 vs actual)                           |

### BLOQUE C: Escritura del Informe Final en LaTeX (8-12 horas)

> El informe_final.tex ya tiene el preamble APA 7 correcto. Llenar capítulos.

#### C-01: Capítulo 1 — Introducción (1 hora)

- Contexto del problema (visualización técnica de hardware)
- Planteamiento del problema
- Justificación (por qué WebGL, por qué un drone)
- Objetivos (general + 4 específicos, ya definidos en propuesta)
- Alcance y limitaciones
- **Fuente:** `final_proposal.tex` (ya tiene la mayor parte)

#### C-02: Capítulo 2 — Marco Teórico (1.5 horas)

- Visualización técnica 3D (estado del arte)
- WebGL y evolución del renderizado web (WebGL 1.0 → 2.0, comparar con Three.js, Babylon.js)
- Unity como plataforma WebGL (URP, IL2CPP → WebAssembly)
- UI Toolkit vs IMGUI vs uGUI
- Drones como caso de estudio (Holybro X500 V2, justificación de selección)
- Renderizado por modos (blueprint, wireframe, thermal, x-ray, ghosted)
- **Fuentes:** `External_docs/`, Antigravity Reports 06-10, `3D_Web_Prototypes_for_Technical_Visualization.pdf`

#### C-03: Capítulo 3 — Marco Metodológico (1.5 horas)

- Tipo de investigación: Aplicada, enfoque cuantitativo-cualitativo
- Diseño: Prototipado iterativo (8 fases documentadas en BITACORA)
- Población y muestra: Estudiantes/profesionales ingeniería (N=8-12 target)
- Instrumentos: SUS (10 ítems), NASA-TLX (6 dimensiones)
- Fases del proyecto: citar las 8 fases del BITACORA
- Herramientas: Unity, Blender, VS Code, Git (de TECHNOLOGY_STACK)
- **Fuente:** BITACORA.md, TECHNOLOGY_STACK.md, CUESTIONARIO_SUS.md, CUESTIONARIO_NASA_TLX.md

#### C-04: Capítulo 4 — Desarrollo del Prototipo (3-4 horas) ⭐ CAPÍTULO MÁS EXTENSO

- 4.1 Arquitectura del sistema (capas, Singleton, EventBus, AppStateMachine)
- 4.2 Selección del hardware de estudio (Holybro X500 V2 — Antigravity Reports)
- 4.3 Pipeline de contenido 3D (modelado, retopología, UV, texturas)
- 4.4 Sistema de renderizado (9 shaders, URP, approach de material swapping)
- 4.5 Interfaz de usuario (UI Toolkit, design system, 3 modos: Inspect/Analyze/Studio)
- 4.6 Interacción y navegación (CameraManager, SelectionManager, ExplodeManager)
- 4.7 Optimización WebGL (memoria 256MB, LODs, texture atlas, batching)
- 4.8 Deployment (WebGL build, GitHub Pages, iframe integration)
- **Fuente:** 01_Arquitectura.md, 02_Referencia_Tecnica.md, 03_Pipeline_Shaders.md, Antigravity Reports

#### C-05: Capítulo 5 — Resultados y Análisis (1.5 horas)

- 5.1 Métricas de rendimiento (FPS, draw calls, VRAM, TTI — del Bloque B)
- 5.2 Métricas de código (91 scripts, 14,778 líneas, 9 shaders, 232 commits)
- 5.3 Evaluación de usabilidad (SUS + NASA-TLX — requiere ejecución)
- 5.4 Cumplimiento de objetivos (tabla objetivo↔evidencia)
- 5.5 Análisis comparativo (vs Three.js, vs aplicaciones similares)
- **Fuente:** Bloque B mediciones, datos SUS/NASA-TLX

#### C-06: Capítulo 6 — Conclusiones y Trabajo Futuro (0.5 horas)

- Conclusiones por objetivo
- Aportes del trabajo
- Limitaciones encontradas
- Trabajo futuro (audio, animaciones, mobile AR, multiplayer)
- Recomendaciones

#### C-07: Referencias Bibliográficas (1 hora)

- Migrar/ampliar references.bib
- Asegurar formato APA 7
- Mínimo 25-30 referencias (papers, docs oficiales, libros)

#### C-08: Anexos (1 hora)

- Anexo A: CHANGELOG completo (regenerado)
- Anexo B: Inventario de código (tabla de 91 scripts con descripción)
- Anexo C: Instrumentos de evaluación (SUS + NASA-TLX)
- Anexo D: Resultados de evaluación (datos crudos)
- Anexo E: Capturas de pantalla del prototipo
- Anexo F: Diagramas de arquitectura

### BLOQUE D: Validación con Usuarios (3-4 horas, puede ser paralelo)

> PENDIENTE ACADÉMICO MÁS IMPORTANTE — sin esto, Objetivo 4 queda incumplido

| Tarea                              | Tiempo est. | Detalle                                                                                  |
| ---------------------------------- | ----------- | ---------------------------------------------------------------------------------------- |
| D-01: Reclutar participantes       | 30 min      | N=8-12, perfil: estudiantes o profesionales de ingeniería/diseño                         |
| D-02: Preparar protocolo de prueba | 30 min      | Tareas específicas: navegar, seleccionar pieza, cambiar vista, usar explosión, leer info |
| D-03: Deploy URL pública           | 30 min      | Verificar que GitHub Pages funciona o deploy alternativo                                 |
| D-04: Ejecutar pruebas             | 60-90 min   | Cada participante: 5-8 min tareas + 3 min cuestionarios                                  |
| D-05: Tabular SUS                  | 30 min      | Calcular score SUS (0-100), interpretar según Bangor et al.                              |
| D-06: Tabular NASA-TLX             | 30 min      | 6 dimensiones, weighted average                                                          |
| D-07: Análisis estadístico         | 30 min      | Media, DE, gráficas, interpretación                                                      |

### BLOQUE E: Manuales (2 horas)

> Ya tienen buen avance, necesitan pulido

| Tarea                               | Tiempo est. | Detalle                                                                 |
| ----------------------------------- | ----------- | ----------------------------------------------------------------------- |
| E-01: Actualizar manual_tecnico.tex | 45 min      | Agregar Phase 3 changes, corregir counts, agregar sección de deployment |
| E-02: Actualizar manual_usuario.tex | 45 min      | Agregar capturas reales, verificar instrucciones con build actual       |
| E-03: Compilar PDFs                 | 30 min      | pdflatex × 2 para cada manual, verificar formato                        |

---

## 5. PRIORIZACIÓN Y SECUENCIA RECOMENDADA

### Día 1 (Mañana) — Fundamentos (~6 horas)

```
09:00-09:30  A-03: Corregir TECHNOLOGY_STACK (10 min) + A-05: Marcar auditorías stale (15 min)
09:30-10:15  A-01: Regenerar CHANGELOG.md desde git log
10:15-10:45  A-02: Actualizar BITACORA.md (Phase 3)
10:45-11:15  A-04: Actualizar 01_Arquitectura.md
11:15-11:30  PAUSA
11:30-12:00  B-01: Build WebGL de producción
12:00-12:30  B-02 + B-03 + B-04 + B-05: Capturar todas las métricas
12:30-13:00  B-06: Crear tabla comparativa de métricas
13:00-14:00  ALMUERZO
14:00-15:00  C-01: Cap. 1 Introducción (convertir desde proposal)
15:00-16:30  C-02: Cap. 2 Marco Teórico
16:30-17:00  PAUSA + Revisión
```

### Día 2 — Capítulos Principales (~7 horas)

```
09:00-10:30  C-03: Cap. 3 Marco Metodológico
10:30-11:00  PAUSA
11:00-15:00  C-04: Cap. 4 Desarrollo del Prototipo (con almuerzo)
15:00-15:30  PAUSA
15:30-17:00  C-07: Referencias + C-08: Anexos base
```

### Día 3 — Validación + Resultados (~6 horas)

```
09:00-10:00  D-01 + D-02 + D-03: Preparar validación
10:00-12:00  D-04: Ejecutar pruebas con participantes
12:00-13:00  D-05 + D-06: Tabular resultados
13:00-14:00  ALMUERZO
14:00-15:30  C-05: Cap. 5 Resultados y Análisis (con datos reales)
15:30-16:00  C-06: Cap. 6 Conclusiones
16:00-17:00  E-01 + E-02: Actualizar manuales
```

### Día 4 — Compilación y Entrega (~3 horas)

```
09:00-10:00  Revisión integral del informe
10:00-11:00  E-03: Compilar todos los PDFs
11:00-12:00  Revisión final de formato APA 7, paginación, lista de tablas/figuras
12:00-12:30  Entregar
```

---

## 6. ESTRUCTURA LaTeX RECOMENDADA

```
Informe_final/
├── informe_final.tex          ← Documento maestro (ya existe)
├── references.bib             ← Bibliografía (migrar de Propuesta/)
├── chapters/
│   ├── 01_introduccion.tex
│   ├── 02_marco_teorico.tex
│   ├── 03_marco_metodologico.tex
│   ├── 04_desarrollo.tex      ← Más extenso (~20-30 páginas)
│   ├── 05_resultados.tex
│   └── 06_conclusiones.tex
├── appendices/
│   ├── A_changelog.tex
│   ├── B_inventario_codigo.tex
│   ├── C_instrumentos.tex
│   ├── D_resultados_evaluacion.tex
│   ├── E_capturas.tex
│   └── F_diagramas.tex
├── figures/
│   ├── arquitectura_capas.png
│   ├── screenshot_inspect.png
│   ├── screenshot_analyze.png
│   ├── screenshot_studio.png
│   ├── shader_comparison.png
│   ├── sus_results.png
│   └── ...
├── Manual_tecnico/
│   └── manual_tecnico.tex     ← Ya existe
└── Manual_de_usuario/
    └── manual_usuario.tex     ← Ya existe
```

### Preamble ya configurado:

- `\documentclass[12pt]{article}`
- `\usepackage[spanish]{babel}`
- `\usepackage{times}` (APA 7)
- `\usepackage[margin=2.54cm]{geometry}`
- `\setlength{\parindent}{0pt}`, `\doublespacing`
- Portada con logo UNAD, título, autor, asesor

---

## 7. CONTENIDO DISPONIBLE POR CAPÍTULO (Mapa de Fuentes)

| Capítulo              | Fuentes MD/LaTeX Existentes                                                                             | % Base Disponible                       |
| --------------------- | ------------------------------------------------------------------------------------------------------- | --------------------------------------- |
| 1. Introducción       | `final_proposal.tex` (secciones 1-4), `informe_final.tex` (esqueleto)                                   | **80%** — copiar/adaptar                |
| 2. Marco Teórico      | `External_docs/*.md`, Reports 06-10, `3D_Web_Prototypes.pdf`, `UI_Toolkit_WebGL_Guide.md`               | **60%** — sintetizar                    |
| 3. Marco Metodológico | `BITACORA.md`, `TECHNOLOGY_STACK.md`, `CRONOGRAMA.md`, cuestionarios SUS/NASA-TLX                       | **50%** — redactar                      |
| 4. Desarrollo         | `01_Arquitectura.md` (1060 líneas), `02_Referencia_Tecnica.md`, `03_Pipeline_Shaders.md`, Reports 06-10 | **70%** — convertir MD→LaTeX            |
| 5. Resultados         | Bloque B (por capturar), datos SUS/NASA-TLX (por ejecutar)                                              | **10%** — requiere trabajo nuevo        |
| 6. Conclusiones       | `ACADEMIC_ALIGNMENT_REPORT.md`, objetivos de proposal                                                   | **30%** — redactar basado en resultados |
| Referencias           | `references.bib` (base), papers citados en MD                                                           | **40%** — ampliar                       |
| Anexos                | CHANGELOG, código fuente, cuestionarios                                                                 | **50%** — formatear                     |

---

## 8. OBJETIVOS vs EVIDENCIA (para Cap. 5)

| Objetivo Específico                                          | Evidencia                                                                                                               | Estado      |
| ------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------- | ----------- |
| OE1: Diseñar arquitectura modular web 3D                     | 91 scripts, patrón Singleton+EventBus+StateMachine, 28 Core/Managers                                                    | 🟢 CUMPLIDO |
| OE2: Implementar modos de visualización técnica              | 9 shaders (Blueprint, Wireframe, Thermal, XRay, Ghosted, SolidColor, ClippableLit), 3 modos UI (Inspect/Analyze/Studio) | 🟢 CUMPLIDO |
| OE3: Desarrollar sistema de análisis estructural interactivo | ExplodeManager, SelectionManager, HotspotManager, DronePartData (11 SO), FilterManager                                  | 🟢 CUMPLIDO |
| OE4: Validar usabilidad mediante evaluación heurística       | Instrumentos SUS + NASA-TLX listos, **ejecución pendiente**                                                             | 🟡 PARCIAL  |

---

## 9. RIESGOS Y MITIGACIONES

| Riesgo                                           | Impacto                             | Mitigación                                               |
| ------------------------------------------------ | ----------------------------------- | -------------------------------------------------------- |
| No conseguir N=8 participantes para SUS/NASA-TLX | Alto — Obj. 4 incumplido            | Usar red de compañeros UNAD, LinkedIn, grupos de Discord |
| URL de deploy no funciona                        | Medio — no se puede evaluar         | Tener backup local (localhost + ngrok)                   |
| No hay archivos GLB exportados                   | Bajo — promesa de tesis sin cumplir | Eliminar promesa o exportar rápido desde Blender         |
| Informe excede extensión UNAD                    | Bajo                                | Mantener ~80-100 páginas de cuerpo                       |
| Audio sin implementar                            | Bajo                                | Documentar como "trabajo futuro"                         |

---

## 10. CHECKLIST FINAL ANTES DE ENTREGA

- [ ] informe_final.pdf compilado sin errores
- [ ] manual_tecnico.pdf compilado sin errores
- [ ] manual_usuario.pdf compilado sin errores
- [ ] Formato APA 7 verificado (portada, márgenes, espaciado, citas, referencias)
- [ ] CHANGELOG.md actualizado (232 commits)
- [ ] BITACORA.md completa hasta Mar 3, 2026
- [ ] TECHNOLOGY_STACK.md corregido (Unity 6000.0.62f1)
- [ ] URL pública del prototipo funcionando
- [ ] Datos SUS tabulados (score y análisis)
- [ ] Datos NASA-TLX tabulados (scores por dimensión)
- [ ] Capturas de pantalla de los 7 modos de vista
- [ ] Diagramas de arquitectura incluidos
- [ ] references.bib con 25+ fuentes APA 7
- [ ] Todas las tablas y figuras numeradas y referenciadas
- [ ] Tabla de contenido, lista de tablas, lista de figuras generadas
- [ ] Revisión ortográfica final

---

## 11. NOTAS TÉCNICAS PARA MAÑANA

### Para regenerar CHANGELOG desde git:

```bash
git log --reverse --format="| %h | %ad | %s |" --date=short
```

### Para contar líneas actualizadas:

```powershell
$t=0; gci Assets\Scripts -R -Filter *.cs | %{ $t+=[IO.File]::ReadAllLines($_.FullName).Length }; $t
```

### Para capturar métricas WebGL:

1. Abrir build en Chrome → F12 → Performance → Record → interactuar 30s → Stop
2. Frame rate, scripting time, rendering time
3. Console: `performance.timing.loadEventEnd - performance.timing.navigationStart` (TTI)

### Para compilar LaTeX:

```bash
pdflatex informe_final.tex
bibtex informe_final
pdflatex informe_final.tex
pdflatex informe_final.tex
```

### DronePartData ScriptableObjects (11 piezas confirmadas):

Antenna, Arm_FL, Battery, Camera_FPV, ESC, FlightController, GPS, MainBody, Motor_FL, Propeller_FL, VTX

### Shaders (9 confirmados):

AnimatedGradientSkybox, Blueprint, ClippableLit, Ghosted, SolidColor, Thermal, Wireframe, WireframeWebGL, XRay
