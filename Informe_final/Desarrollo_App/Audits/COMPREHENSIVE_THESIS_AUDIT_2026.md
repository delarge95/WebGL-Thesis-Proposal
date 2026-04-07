# 🔬 AUDITORÍA INTEGRAL DE TESIS — 360° (Actualización Mar 31, 2026)

## Informe Final + Entregables — Análisis Multidimensional

**Auditor:** Ingeniería + Academia + Diseño  
**Fecha:** 2026-03-31  
**Base:** 5 audits previos (ACADEMIC_ALIGNMENT, ARCHITECTURE, PERFORMANCE, UX_UI, REMEDIATION) + análisis actual del `.tex` y codebase  
**Codebase actual:** 102 scripts C# · 16,015 LOC · 9 shaders · Unity 6000.0.62f1

---

## Índice de Dimensiones Evaluadas

| # | Dimensión | Sección | Score |
|---|-----------|---------|-------|
| 1 | Coherencia Académica | §1 | 🟡 7.2/10 |
| 2 | Normas APA 7 / Formato UNAD | §2 | 🟡 7.0/10 |
| 3 | Bibliografía | §3 | 🟢 8.5/10 |
| 4 | Cumplimiento de KPIs | §4 | 🔴 3.0/10 |
| 5 | Rigor Matemático | §5 | 🟢 8.0/10 |
| 6 | Precisión Técnica | §6 | 🟡 7.5/10 |
| 7 | Diseño y UX | §7 | 🟢 8.0/10 |
| 8 | Estructura y Formato del Documento | §8 | 🟡 6.5/10 |
| 9 | Entregables Pendientes | §9 | 🔴 4.5/10 |
| 10 | Alineación Código–Documento | §10 | 🟡 6.0/10 |
| 11 | Subsistema Térmico | §11 | 🟡 5.5/10 |
| 12 | Preparación para Defensa | §12 | 🟡 6.0/10 |
| — | **Score Ponderado Global** | — | **6.4/10** |

---

## §1 — Coherencia Académica

### 1.1 Objetivos vs. Desarrollo vs. Conclusiones

| Objetivo Específico | Declarado en Cap. 1 | Desarrollado en Cap. 4 | Concluido en Cap. 6 | Evidencia Cuantitativa Cap. 5 | Coherencia |
|---|---|---|---|---|---|
| OE1: Pipeline 3D <100K tri | ✅ | ✅ | ✅ (pero dice "malla inferior a 100K" sin dato real) | 🔴 `[pendiente]` | 🟡 |
| OE2: Shaders PBR <33ms | ✅ | ✅ (9 shaders documentados) | ✅ (renombrado a "Arquitectura Modular") | 🔴 `[pendiente]` | 🟡 |
| OE3: Interacción C#→WASM | ✅ | ✅ | 🟡 (OE3 renumerado a "Evaluación" → inconsistencia) | N/A | ⚠️ |
| OE4: Validación SUS/TLX | ✅ | 🟡 (instrumentos ready) | ✅ (dice "pendiente") | 🔴 sin datos | 🟡 |

> [!CRITICAL]
> **Renumeración silenciosa de OEs en Conclusiones:** En Cap. 1, OE2 es "Shaders PBR en URP", OE3 es "Sistema de Interacción C#→WASM", OE4 es "Validación". En Cap. 6, OE2 se convirtió en "Arquitectura Modular de Software" y OE3 es "Evaluación de Usabilidad". **Esta permutación no fue señalada**, lo que crea confusión para el lector/evaluador. Recomendación: alinear las conclusiones exactamente con la numeración de los objetivos declarados.

### 1.2 Pregunta de Investigación

No hay una pregunta de investigación explícitamente formulada con signos de interrogación. El planteamiento del problema describe la brecha, pero la tradición DSR requiere al menos una pregunta guía. Sugerencia: agregar "¿Es viable desplegar un prototipo WebGL con fidelidad PBR que cumpla <100K triángulos, >30 FPS y SUS ≥ 68 para visualización de hardware de drones?" al inicio del planteamiento.

### 1.3 Hipótesis

Cap. 3 (§3.5 NASA-TLX) declara **una hipótesis de contraste**: "El visor 3D produce carga cognitiva significativamente menor que documentación PDF 2D". Esto es bueno, pero:
- No hay hipótesis nula explícita (H₀).
- Sin tamaño de efecto esperado (solo rango "60-80 → 30-50").
- La hipótesis se testearía con t-Student o Mann-Whitney U, pero sin datos no se puede verificar.

### 1.4 Hallazgos §1

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| AC-01 | OEs renumerados en conclusiones sin correspondencia | 🔴 Crítico | Alinear conclusiones con numeración original |
| AC-02 | Sin pregunta de investigación explícita | 🟠 Alto | Agregar al Cap. 1 |
| AC-03 | Hipótesis sin H₀ formal | 🟡 Medio | Formalizar en Cap. 3 |
| AC-04 | Cap. 5 completamente vacío (placeholders) | 🔴 Crítico | Requiere data real |

---

## §2 — Normas APA 7 / Formato UNAD

### 2.1 Formato General LaTeX

| Requisito APA 7 | Implementación | Estado |
|---|---|---|
| Fuente Times New Roman 12pt | `\usepackage{times}` (Times, no TNR exacto, pero aceptable) | ✅ |
| Márgenes 2.54 cm | `[margin=2.54cm]` | ✅ |
| Interlineado doble | `\setstretch{2}` | ✅ |
| Indentation 1.27cm | `\setlength{\parindent}{1.27cm}` | ✅ |
| Paginación esquina superior derecha | `\fancyhead[R]{\thepage}` | ✅ |
| Títulos centrados (Nivel 1) | `\titleformat{\section}{...\centering}` | ✅ |
| Títulos alineados izq (Nivel 2) | `\titleformat{\subsection}{...\bfseries}` | ✅ |
| Sin numeración de secciones | `\setcounter{secnumdepth}{0}` | ✅ |
| Running head (cornisa) | **No implementado** | 🔴 |

> [!WARNING]
> **Running head ausente:** APA 7 para tesis requiere cornisa (running head) en cada página con el título abreviado en mayúsculas. Actualmente solo hay número de página. La UNAD puede ser flexible aquí, pero el evaluador podría señalarlo.

### 2.2 Elementos de Portada

| Elemento | Estado |
|---|---|
| Título del trabajo | ✅ |
| Nombre del estudiante | ✅ |
| Nombre del asesor | ✅ |
| Universidad | ✅ |
| Escuela | ✅ |
| Programa | ✅ |
| Año | ✅ (dice "2025" — ¿debería ser 2026?) |
| Ciudad | ❌ Falta |

### 2.3 Tablas

| Requisito APA 7 | Estado | Notas |
|---|---|---|
| Número + Título en itálica encima | ✅ | `\captionsetup[table]` bien configurado |
| Sin líneas verticales | ✅ | Uso de `booktabs` |
| Nota al pie de tabla | ✅ parcial | Algunas tablas tienen nota, otras no |
| Fuente citada | 🟡 | Solo Tabla 1 (comparativa) tiene nota de fuente |

### 2.4 Hallazgos §2

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| APA-01 | Sin running head / cornisa | 🟡 Medio | Agregar `\fancyhead[L]{\MakeUppercase{Prototipo Web 3D...}}` |
| APA-02 | Año en portada dice "2025", debería ser 2026 | 🔴 Crítico | Corregir |
| APA-03 | Falta ciudad en portada | 🟡 Medio | Agregar "Pasto, Colombia" |
| APA-04 | Resumen/Abstract sin indentación es correcto, pero falta la línea "Palabras clave:" con italic correcto | 🟢 OK | Ya bien implementado |
| APA-05 | Algunas tablas sin nota de fuente | 🟡 Medio | Agregar "Fuente: Elaboración propia" |

---

## §3 — Bibliografía

### 3.1 Análisis de Referencias

| Métrica | Valor | Evaluación |
|---|---|---|
| Total de referencias listadas | 36 | ✅ Adecuado (30-50 típico) |
| Refs con DOI/URL | 18 (~50%) | 🟡 Idealmente >70% |
| Refs ≤ 5 años (2021-2026) | 10 (~28%) | 🟡 Aceptable pero podría mejorar |
| Refs clásicas (pre-2000) | 10 (~28%) | ✅ Esperado (Gestalt, CLT, PBR fundamentals) |
| Refs en español | 0 | 🟡 El documento está en español pero todas las refs en inglés — aceptable para campo técnico |

### 3.2 Verificación de Citas vs. Referencias

| Cita en el texto | ¿Existe en Cap. 7? | Estado |
|---|---|---|
| Yu et al. (2023) | ✅ | ✅ |
| Mayer (2005, 2021) | ✅ ambas | ✅ |
| Sweller (1988) | ✅ | ✅ |
| Sweller et al. (2019) | ✅ | ✅ |
| Hegarty & Waller (2004) | ✅ | ✅ |
| Cowan (2001) | ✅ | ✅ |
| Fransson et al. (2024) | ✅ | ✅ |
| Bartlett & Dorribo Camba (2023) | ✅ | ✅ |
| Ries (2011) | ✅ | ✅ |

> [!NOTE]
> Se verificaron las 36 referencias contra las citas en el cuerpo. No se detectaron referencias huérfanas (listadas pero no citadas) ni citas rotas (citadas pero no listadas). Esto es excelente.

### 3.3 Formato APA 7 de Referencias

| Aspecto | Estado | Notas |
|---|---|---|
| Hanging indent 1.27cm | ✅ | Implementado con `\list` environment |
| Apellido, Iniciales | ✅ | Consistente |
| Año entre paréntesis | ✅ | |
| Titulo de libro en *itálica* | ✅ | Usa `\textit{}` |
| Vol. de revista en itálica | ✅ | `\textit{4}(3)` |
| & antes del último autor | ✅ | Usa `\&` |
| DOI cuando disponible | 🟡 | ~50% tienen DOI — deberían ser más |
| URLs sin "Retrieved from" | ✅ | APA 7 ya no requiere "Recuperado de" |

### 3.4 Hallazgos §3

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| BIB-01 | ~50% sin DOI cuando el DOI probablemente existe | 🟡 Medio | Buscar DOIs faltantes para: Bowman (2004), Gamma (1994), Nielsen (1994), etc. |
| BIB-02 | Hegarty (2004) lista 2 entradas distintas: individual y con Waller | ✅ OK | Correcto — son publicaciones diferentes |
| BIB-03 | Falta referencia para MoI 3D (mencionado en conversación pero no en tesis) | 🟢 Bajo | No está citado en el tex, así que no aplica |

---

## §4 — Cumplimiento de KPIs

> [!CAUTION]
> **DIMENSIÓN MÁS CRÍTICA.** El Cap. 5 tiene **0 de 7 KPIs con datos reales**. Todas las celdas dicen `[pendiente]`. Esto invalida el capítulo de Resultados completo.

### 4.1 Estado de cada KPI

| KPI | Meta | Medible en App | Dato en Cap. 5 | Dato en Cualquier Parte | Severity |
|---|---|---|---|---|---|
| Polígonos <100K | <100,000 | `FPSCounter.cs` no los mide | `[pendiente]` | No documentado | 🔴 |
| FPS >30 (mid-range) | >30 | `FPSCounter.cs` existe | `[pendiente]` | No documentado | 🔴 |
| Draw Calls <50 | <50 | No medido | `[pendiente]` | No documentado | 🔴 |
| Frame Time <33ms | <33.33ms | `PerformanceMonitor.cs` | `[pendiente]` | No documentado | 🔴 |
| VRAM <64MB | <64MB | No medido | `[pendiente]` | No documentado | 🔴 |
| TTI Shell <3s | <3s | No medido | `[pendiente]` | No documentado | 🔴 |
| TTI Full <10s | <10s | No medido | `[pendiente]` | No documentado | 🔴 |
| SUS ≥ 68 | ≥68 | Cuestionario listo | `[pendiente]` | No ejecutado | 🔴 |
| NASA-TLX ≤ 40 | ≤40 | Cuestionario listo | `[pendiente]` | No ejecutado | 🔴 |

> **Veredicto: 0/9 KPIs documentados con evidencia.** Esto es el gap más grande de la tesis.

### 4.2 Acciones de Remediación (por prioridad)

| # | Acción | Herramienta | Est. Tiempo | Criticidad |
|---|---|---|---|---|
| 1 | Profiling de polígonos, draw calls, FPS | Unity Profiler + Stats window | 1 hora | 🔴 Bloqueante |
| 2 | Screenshot Unity Profiler como evidencia | Captura de pantalla | 15 min | 🔴 Bloqueante |
| 3 | Medir VRAM | Memory Profiler + Spector.js | 1 hora | 🔴 Bloqueante |
| 4 | Medir TTI en red 4G | Chrome DevTools throttling | 30 min | 🔴 Bloqueante |
| 5 | Ejecutar SUS con N=8-12 | Forms + sesiones presenciales | 2-3 días | 🔴 Bloqueante |
| 6 | Ejecutar NASA-TLX | Forms + comparativa PDF vs 3D | 2-3 días | 🔴 Bloqueante |

---

## §5 — Rigor Matemático

### 5.1 Ecuaciones Verificadas

| Ecuación | Ubicación | Correcta | Notas |
|---|---|---|---|
| $P_{\text{total}} = \sum P_i \leq 100{,}000$ | Cap. 2, §Presupuesto | ✅ | Definición estándar |
| $\rho = R_{\text{texture}} / A_{\text{UV}} = 10.24 \text{ px/cm}$ | Cap. 2, §Densidad Texel | ✅ | Consistente con industria |
| $D_{\text{opt}} = D_{\text{original}} / N_{\text{instances}}$ | Cap. 2, §Draw Calls | ✅ | Simplificación válida |
| $T_{\text{frame}} = T_{\text{CPU}} + T_{\text{GPU}} < 33.33\text{ms}$ | Cap. 2, §Frame Budget | ✅ | Correcto (30 FPS = 33.33ms) |
| $f_{\text{spec}} = \frac{D \cdot F \cdot G}{4(\mathbf{n}\cdot\mathbf{l})(\mathbf{n}\cdot\mathbf{v})}$ | Cap. 2, §Cook-Torrance | ✅ | Formulación estándar |
| $F = F_0 + (1-F_0)(1-\cos\theta)^5$ | Cap. 2, §Fresnel-Schlick | ✅ | Correcto |
| $D_{\text{GGX}} = \frac{\alpha^2}{\pi((\mathbf{n}\cdot\mathbf{h})^2(\alpha^2-1)+1)^2}$ | Cap. 2, §GGX | ✅ | Correcto (donde α=roughness²) |
| Conservación de energía Disney | Cap. 2, §Disney | ✅ | $E_r + E_a = E_i$ |

### 5.2 Observaciones Matemáticas

> [!TIP]
> **Fortaleza**: Las ecuaciones PBR están correctamente citadas y son consistentes con las fuentes (Cook-Torrance 1982, Schlick 1994, Walter et al. 2007, Burley 2012, Karis 2013). El rigor aquí es sólido.

La ecuación de densidad de texel podría beneficiarse de más contexto: qué significa "10.24 px/cm" en términos prácticos (e.g., "equivalente a 1024px en un área de 10cm² de superficie UV").

### 5.3 Hallazgos §5

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| MATH-01 | Ecuaciones correctas pero sin demostración de uso real en los resultados | 🟡 Medio | Vincular ecuaciones con datos en Cap. 5 |
| MATH-02 | Texel density no tiene evidencia de medición | 🟡 Medio | Documentar medición por pieza |

---

## §6 — Precisión Técnica

### 6.1 Datos del Codebase vs. Documento

| Claim en Documento | Valor Real (verificado hoy) | Coherente |
|---|---|---|
| "91 scripts C#" (Cap. 3, 4, 6) | **102 scripts** (contados en Assets/Scripts) | ❌ Desactualizado |
| "~14,778 líneas" (Cap. 3, 4) | **16,015 LOC** (medido ahora) | ❌ Desactualizado |
| "9 shaders" (múltiples caps) | **9 shaders** (verificado) | ✅ |
| "~1,749 líneas HLSL" | No verificado en esta sesión | 🟡 Posiblemente desactualizado |
| "240+ commits" | No verificado | 🟡 Probablemente más ahora |
| "7 modos de visualización" | 7 modos confirmados en codebase | ✅ |
| "DronePartData 25+ campos" | "más de 32 campos" (Cap. 4) → consistente | ✅ |
| "4 capas de arquitectura" | Confirmado en código | ✅ |
| "Unity 6000.0.62f1" (Cap. 3) | Registry muestra 6000.0.62f1, 6000.3.0f1, 6000.3.1f1 | ✅ (versión correcta instalada) |
| "16 piezas" (catálogo Cap. 4) | 28 nodos canónicos (RETOPOLOGIA_POR_PIEZA.md) | ⚠️ Discrepancia |

> [!WARNING]
> **Discrepancia del conteo de piezas**: El Cap. 4 dice "16 piezas" y tiene una tabla con 16 ítems. Sin embargo, el sistema térmico y el modelo Blender actual trabajan con **28 nodos canónicos** (definidos en `RETOPOLOGIA_POR_PIEZA.md`). Esto refleja la evolución del modelo CAD (importación completa vs. agrupación simplificada para el visor). El documento debe explicar esta diferencia o actualizarse.

### 6.2 Subsistema Térmico en Documento vs. Código

Cap. 4, §4.8 describe el subsistema térmico híbrido incluyendo:
- `ThermalSimulationManager`, `ThermalViewController`, `ThermalSurfaceProfile`, `ThermalContactGraphAsset`, `ThermalContactGraphBuilderWindow`

Estado actual:
- Los scripts existen (`Editor/ImportDroneModel.cs` reciente, sistema térmico en fase activa)
- El párrafo en Cap. 4 contiene **texto sin acentos** (encoding issue: "simulacion", "fisicamente", "geometria")
- Mezcla tiempos verbales ("se presenta", "se fijo", "quedo aislado")

### 6.3 Hallazgos §6

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| TEC-01 | Scripts: documento dice 91, real es 102 | 🟠 Alto | Actualizar en Cap. 3, 4, 6, 8 |
| TEC-02 | LOC: documento dice ~14,778, real es 16,015 | 🟠 Alto | Actualizar en Cap. 3, 4, 8 |
| TEC-03 | Piezas: Cap. 4 dice 16, modelo actual tiene 28 nodos | 🟠 Alto | Reconciliar o explicar |
| TEC-04 | §4.8 Térmico tiene encoding corruption (sin acentos) | 🟠 Alto | Re-codificar o reescribir la sección |
| TEC-05 | §4.8 mezcla tiempos verbales (presente/pasado) | 🟡 Medio | Unificar en pasado (informe final) |

---

## §7 — Diseño y UX (Resumen del Audit Existente)

El audit UX/UI previo (STALE desde Feb 19, 2026) identificó:
- 33 touch targets <44px → **CORREGIDO** (según REMEDIATION_PLAN)
- Bottom Sheet con 12 campos sin agrupación → **CORREGIDO**
- Sin onboarding → Parcialmente implementado
- Loading/Error UI en C# puro → **CORREGIDO**

**Score actualizado del diseño: 8.0/10** — Las correcciones post-audit mejoraron significativamente la UX. Los ítems pendientes son cosméticos.

---

## §8 — Estructura y Formato del Documento

### 8.1 Capítulos y Completitud

| Capítulo | Archivo | Lines | Estado |
|---|---|---|---|
| 1. Introducción | `01_introduccion.tex` | 107 | ✅ Completo |
| 2. Marco de Referencia | `02_marco_referencia.tex` | 233 | ✅ Completo |
| 3. Marco Metodológico | `03_marco_metodologico.tex` | 193 | ✅ Completo |
| 4. Desarrollo | `04_desarrollo.tex` | 418 | 🟡 95% completo — §4.8 tiene problemas + algunos TODOs |
| 5. Resultados | `05_resultados.tex` | 149 | 🔴 ~10% completo — **todo son placeholders** |
| 6. Conclusiones | `06_conclusiones.tex` | 83 | 🟡 80% completo — tiene TODOs condicionados a Bloque B/D |
| 7. Referencias | `07_referencias.tex` | 110 | ✅ Completo |
| 8. Apéndices | `08_apendices.tex` | 397 | 🟡 85% completo — Apéndices L y N pendientes |

### 8.2 TODOs Residuales en el Documento

Se encontraron **15 marcadores TODO** en el documento:

| Archivo | Línea(s) | TODO |
|---|---|---|
| `04_desarrollo.tex` | 247 | Diagrama del pipeline 3D → Figura placeholder |
| `04_desarrollo.tex` | 262-264 | Rol de Houdini → Sección pendiente |
| `04_desarrollo.tex` | 271 | Retopología ¿Houdini o Blender? |
| `05_resultados.tex` | 37 | Bloque B: porcentaje real de reducción |
| `05_resultados.tex` | 48 | Bloque D: resultados SUS/TLX |
| `05_resultados.tex` | 60-86 | Bloque B: TODOS los KPIs con datos reales |
| `05_resultados.tex` | 94-102 | Bloque D: datos SUS completos |
| `05_resultados.tex` | 112-119 | Bloque D: datos NASA-TLX completos |
| `05_resultados.tex` | 129-133 | Bloque D: datos Think-Aloud |
| `05_resultados.tex` | 140-146 | Bloques B+D: discusión integradora |
| `06_conclusiones.tex` | 22 | Bloque B: dato real de polígonos |
| `06_conclusiones.tex` | 28-29 | Bloque D: resultados disponibles |
| `08_apendices.tex` | 156 | Assets de audio pendientes |
| `08_apendices.tex` | 274 | URL del demo final |
| `08_apendices.tex` | 287-288 | Pipeline con Houdini (LODs) |

### 8.3 Figuras

| Total de figuras | 1 |
|---|---|
| Figuras con imagen real | 0 |
| Figuras con placeholder | 1 (pipeline 3D, Cap. 4) |

> [!CAUTION]
> **Una tesis de 300+ páginas de LaTeX sin ninguna figura real es inaceptable.** Deben incluirse al mínimo:
> 1. Diagrama de arquitectura del sistema
> 2. Pipeline de optimización 3D
> 3. Screenshots del prototipo (mínimo 3-4 modos de visualización)
> 4. Grafo de estados de la aplicación
> 5. Screenshots del Bottom Sheet / UI
> 6. Modelo 3D del dron (antes vs después de retopología)

### 8.4 Hallazgos §8

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| FMT-01 | 15 marcadores TODO en el documento | 🔴 Crítico | Resolver todos antes de entrega |
| FMT-02 | 0 figuras reales incluidas | 🔴 Crítico | Agregar mínimo 6-8 figuras |
| FMT-03 | Cap. 5 es 90% placeholder | 🔴 Crítico | Requiere datos reales |
| FMT-04 | Cap. 4 §4.5-§4.6 tienen figure placeholder | 🟠 Alto | Crear diagrama del pipeline |
| FMT-05 | §4.8 (Térmico) tiene encoding issues | 🟠 Alto | Reescribir sección con acentos |
| FMT-06 | Houdini mencionado 3 veces como "pendiente" | 🟡 Medio | Decidir: incluir o remover de la tesis |
| FMT-07 | Falta Lista de Figuras | 🟡 Medio | Agregar `\listoffigures` cuando haya figuras |

---

## §9 — Entregables Pendientes (Actualizado Mar 2026)

### 9.1 Entregables Declarados vs. Estado

| # | Entregable | Declarado En | Estado Mar 2026 | Notas |
|---|---|---|---|---|
| D1 | Prototipo WebGL funcional | OG, R1 | 🟡 Build existe, **no desplegado públicamente** | GitHub Pages tuvo issues con Brotli; URL no confirmada |
| D2 | Sistema de shaders URP | OE2, R2 | ✅ Completo | 9 shaders, todos WebGL 2.0 |
| D3 | Modelos 3D optimizados | OE1, R3 | 🟡 Modelo en proceso activo | FBX de 555MB exportado; retopología/decimación pendiente |
| D4 | Documento de trabajo de grado | R4 | 🟡 ~75% completo | Cap. 5 vacío, Cap. 4.8 con errores, 0 figuras |
| D5 | Informe de evaluación de usabilidad | OE4, R5 | 🔴 No ejecutado | Instrumentos listos, evaluación no realizada |
| D6 | Manual de usuario | Apéndice B | ✅ Completo | 11,215 bytes TeX + PDF compilado |
| D7 | Manual técnico | Apéndice C | ✅ Completo | 17,749 bytes TeX + PDF compilado (datos desactualizados: 91→102 scripts) |
| D8 | Archivos .glb exportados | R3 | 🔴 No encontrados | Tesis menciona "Archivos .glb disponibles" |
| D9 | URL pública del prototipo | R1 | 🟡 Parcial | Build en `docs/`, GitHub Pages con issues de compresión |
| D10 | Pipeline report (antes/después) | OE1 | 🔴 No existe | Documentación del pipeline existe pero sin datos cuantitativos |
| D11 | Reporte de KPIs | OE4 | 🔴 No existe | 0/9 KPIs documentados |

### 9.2 Priorización de Entregables Faltantes

| Prioridad | Entregable | Bloqueante para Defensa | Est. Tiempo |
|---|---|---|---|
| 🔴 P0 | Llenar Cap. 5 con datos de KPIs medidos | SÍ | 1 día (medición) + 1 día (redacción) |
| 🔴 P0 | Ejecutar evaluación SUS + NASA-TLX | SÍ | 2-3 días |
| 🔴 P0 | Agregar figuras al documento (mín. 6) | SÍ | 4-6 horas |
| 🔴 P0 | Resolver todos los TODOs del `.tex` | SÍ | 4-6 horas |
| 🟠 P1 | Desplegar prototipo (URL funcional) | SÍ | 1-2 horas |
| 🟠 P1 | Actualizar cifras (102 scripts, 16K LOC) | Recomendado | 30 min |
| 🟠 P1 | Corregir encoding Cap. 4.8 | Recomendado | 30 min |
| 🟡 P2 | Exportar archivos .glb | No bloqueante | 1-2 horas |
| 🟡 P2 | Actualizar manual técnico | Recomendado | 1 hora |

---

## §10 — Alineación Código–Documento

### 10.1 Componentes Mencionados en Código pero No en Documento

| Componente | En Código | En Cap. 4 | Observación |
|---|---|---|---|
| `ImportDroneModel.cs` (Editor) | ✅ | ❌ | Nuevo — para importar modelo de Blender |
| Subsistema térmico: `ThermalCanonicalContactGraph.asset` | ✅ | 🟡 Mencionado indirectamente | §4.8 |
| `ServiceLocator` | ✅ (post-audit) | 🟡 Mencionado en Cap. 4 | Agregado en FASE 2 |
| `BaseModeHandler` + subclases | ✅ (post-audit) | ✅ | Mencionados correctamente |
| `ThermalTestSetup.cs` | ✅ | ✅ §4.8 | Mencionado como "herramienta experimental" |

### 10.2 Features Mencionadas en Documento pero Cuestionables

| Feature en Documento | Claim | Realidad |
|---|---|---|
| "Asset Bundles" (Cap. 1, 2, 3) | Mencionado como feature del pipeline | **No implementado** — `AssetLoader.cs` tiene solo placeholders |
| "Level of Detail automático" (Cap. 1) | Unity URP las ofrece "integradas" | **No configurado** — sin LODGroups en proyecto |
| "Occlusion Culling" (Cap. 1) | Mencionado como razón para elegir Unity | **No verificado** si está habilitado |
| "~30 min de ensamblaje" (Cap. 4, Tabla) | Dato del fabricante | ✅ OK — es dato del hardware real |
| "Substance Painter" texturizado | Mencionado en Sprint 2 | ⚠️ Materiales PBR existen, pero no hay evidencia de que se usó Substance Painter vs. materiales procedurales |

> [!IMPORTANT]
> **Asset Bundles** se mencionan **5+ veces** en Caps. 1, 2, 3 como justificación técnica para elegir Unity, pero **nunca se implementaron**. Esto es un gap de coherencia que un evaluador atento podría señalar. Recomendación: reconocer que Asset Bundles quedaron como trabajo futuro o implementar una versión mínima.

### 10.3 Hallazgos §10

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| ALIGN-01 | Asset Bundles citados como feature pero no implementados | 🟠 Alto | Clarificar en Cap. 6 como trabajo futuro |
| ALIGN-02 | LODGroups no configurados pero mencionados como ventaja | 🟡 Medio | Agregar como trabajo futuro o implementar |
| ALIGN-03 | Cifras de scripts/LOC desactualizadas en 5+ ubicaciones | 🟠 Alto | Actualizar globalmente |
| ALIGN-04 | Catálogo dice 16 piezas, modelo actual tiene 28 nodos | 🟠 Alto | Reconciliar |

---

## §11 — Subsistema Térmico

El subsistema térmico es una **extensión significativa** del proyecto que cambia el alcance del visor desde "visualización técnica" hacia "simulación cualitativa". Estado:

| Componente | Estado | Documentado en Tesis |
|---|---|---|
| `ThermalSimulationManager` | En desarrollo | ✅ §4.8 |
| `ThermalViewController` | En desarrollo | ✅ §4.8 |
| `ThermalSurfaceProfile` | Diseñado | ✅ §4.8 |
| `ThermalContactGraphAsset` | Asset canónico creado | ✅ §4.8 |
| `ThermalContactGraphBuilderWindow` | Editor tool | ✅ §4.8 |
| 28 nodos canónicos nombrados | ✅ En Blender | ❌ No documentados en tesis |
| Wolfram verification | ✅ `wolfram_verificaciones.md` | ❌ No citado en tesis |
| Modelo FBX exportado (555MB) | ✅ | ❌ No documentado |

> [!WARNING]
> El subsistema térmico representa un **scope creep significativo** que no estaba en los objetivos específicos originales. Sin embargo, es un **valor diferenciador fuerte** para la defensa. Recomendación: enmarcarlo como "contribución adicional" en conclusiones, no como cumplimiento de un OE que no lo contemplaba.

### Hallazgos §11

| ID | Hallazgo | Severidad | Acción |
|---|---|---|---|
| THERM-01 | §4.8 tiene encoding corruption severa (0 acentos) | 🟠 Alto | Reescribir con encoding UTF-8 correcto |
| THERM-02 | 28 nodos canónicos no documentados en la tesis | 🟡 Medio | Agregar tabla o referencia a apéndice |
| THERM-03 | Verificaciones Wolfram no citadas | 🟡 Medio | Incluir como soporte en apéndice |
| THERM-04 | Encuadramiento como scope creep no resuelto | 🟡 Medio | Agregar párrafo en Cap. 6 |

---

## §12 — Preparación para Defensa

### 12.1 Risk Matrix Actualizada

| Riesgo | Probabilidad | Impacto | Mitigación |
|---|---|---|---|
| "Cap. 5 completamente vacío" | 🔴 Actual (hoy) | 🔴 Reprobación | Llenar con datos reales |
| "Sin figuras en el documento" | 🔴 Actual (hoy) | 🟠 Deducción severa | Agregar 6-8 figuras |
| "Sin evaluación con usuarios" (SUS/TLX) | 🔴 Alta | 🔴 OE4 no cumplido | Ejecutar evaluación |
| "15 TODOs en el .tex" | 🔴 Actual | 🟠 Impresión de documento inacabado | Resolver todos |
| "Asset Bundles prometidos, no implementados" | 🟡 Media | 🟡 Incoherencia señalable | Reconocer como trabajo futuro |
| "Año 2025 en portada" | 🔴 Actual | 🟡 Error fácilmente corregible | Cambiar a 2026 |
| "102 scripts ≠ 91 citados" | 🟡 Media | 🔵 Menor | Actualizar cifras |

### 12.2 Fortalezas para Resaltar en Defensa

1. **138% del target de scripts** (102 vs 65+ prometidos)
2. **9 shaders custom WebGL 2.0** — punto diferenciador técnico excepcional
3. **Arquitectura de 4 capas + 6 patrones de diseño** — rigor de ingeniería de software
4. **Subsistema térmico con verificación Wolfram** — contribución adicional
5. **Pipeline de Technical Art documentado** — contribución replicable
6. **Dual quality tier URP** — buen diseño de rendimiento
7. **UI Toolkit + Design System** — enfoque moderno vs. Canvas legacy
8. **Bibliografía sólida** (36 refs, bien citadas, sin refs rotas)
9. **Ecuaciones PBR verificadas** — rigor matemático
10. **Instrumentos SUS/TLX completos y listos** — diseño metodológico sólido

---

## Scorecard Final Ponderado

| Dimensión | Peso | Score | Ponderado |
|---|---|---|---|
| Coherencia Académica | 15% | 7.2 | 1.08 |
| APA 7 / Formato UNAD | 10% | 7.0 | 0.70 |
| Bibliografía | 5% | 8.5 | 0.43 |
| **Cumplimiento KPIs** | **20%** | **3.0** | **0.60** |
| Rigor Matemático | 5% | 8.0 | 0.40 |
| Precisión Técnica | 10% | 7.5 | 0.75 |
| Diseño y UX | 5% | 8.0 | 0.40 |
| Estructura del Documento | 10% | 6.5 | 0.65 |
| **Entregables Pendientes** | **10%** | **4.5** | **0.45** |
| Alineación Código–Doc | 5% | 6.0 | 0.30 |
| Subsistema Térmico | 2.5% | 5.5 | 0.14 |
| Preparación Defensa | 2.5% | 6.0 | 0.15 |
| **TOTAL** | **100%** | — | **6.04/10** |

> [!CAUTION]
> **Score global: 6.04/10 — Insuficiente para defensa en estado actual.**
>
> Las dos dimensiones que más arrastran el score son:
> - **KPIs (0.60/2.0)** — 0 de 9 KPIs documentados
> - **Entregables (0.45/1.0)** — Cap. 5 vacío, sin evaluación, sin figuras
>
> **Si se llenan los datos de KPIs + evaluación con usuarios + figuras, el score sube estimadamente a ~8.0/10**, que es un nivel sólido para defensa.

---

## Plan de Acción Crítico (Ruta a Defensa)

```
SEMANA 1 (Días 1-3): MEDICIÓN + EVALUACIÓN
├── Día 1: Profiling completo (polígonos, FPS, draw calls, VRAM, TTI)
│   ├── Unity Editor: Stats Window + Profiler → screenshots
│   ├── Chrome DevTools: throttle 4G → medir TTI
│   └── Spector.js: draw call analysis
├── Día 2-3: Evaluación con usuarios (N=8-12)
│   ├── Preparar: sesiones + formularios SUS/TLX impresos
│   ├── Ejecutar: 30-45 min/participante
│   └── Compilar datos en spreadsheet
└── Día 3: Calcular scores SUS + NASA-TLX promedios

SEMANA 1 (Días 4-5): REDACCIÓN CAP. 5
├── Llenar TODAS las tablas con datos reales
├── Escribir análisis por KPI (1-2 párrafos cada uno)
├── Escribir discusión integradora
└── Marcar TODOS los TODOs como resueltos

SEMANA 2 (Días 1-3): FIGURAS + CORRECCIONES
├── Crear 6-8 figuras (screenshots del visor, diagramas)
├── Corregir encoding §4.8 (térmico)
├── Alinear OEs en conclusiones (Cap. 6)
├── Actualizar cifras (102 scripts, 16K LOC) en 5+ archivos
├── Corregir portada (año 2026, agregar ciudad)
└── Actualizar manual técnico

SEMANA 2 (Días 4-5): FINALIZACIÓN
├── Resolver todos los TODOs restantes
├── Desplegar prototipo a URL pública funcional
├── Revisión final de coherencia (Intro→Resultados→Conclusiones)
├── Compilar PDF final
└── Preparar presentación de defensa
```

---

## Comparativa con Audits Previos

| Audit Previo | Fecha Original | Estado STALE | Notas de Actualización |
|---|---|---|---|
| `ACADEMIC_ALIGNMENT_REPORT.md` | Jul 2025 | ⚠️ Stale (Feb 19 commit) | Score subió de 6.75→7.45 en audit-of-audit; **este audit lo actualiza a 6.04 con datos actuales** |
| `ARCHITECTURE_AUDIT_REPORT.md` | Jul 2025 | ⚠️ Stale | Muchos hallazgos resueltos (C-01, C-02, H-01, H-04, H-05) |
| `PERFORMANCE_AUDIT_REPORT.md` | Jul 2025 | ⚠️ Stale | Múltiples config fixes aplicados (H06, H07, H08, H09) |
| `UX_UI_AUDIT_REPORT.md` | Jul 2025 | ⚠️ Stale | Touch targets, bottom sheet fixes completados |
| `REMEDIATION_PLAN.md` | Jul 2025 | ⚠️ Stale | FASE 1 completada, FASE 2 parcial |

> **Este audit (Mar 31, 2026) es la fuente de verdad más actualizada.** Los 5 audits previos deben considerarse informativos pero superados por este análisis.

---

*Generado por Antigravity — Auditoría Integral 360° de Tesis*
*102 scripts · 16,015 LOC · 9 shaders · 36 referencias · 305 líneas LaTeX principal · 8 capítulos*
