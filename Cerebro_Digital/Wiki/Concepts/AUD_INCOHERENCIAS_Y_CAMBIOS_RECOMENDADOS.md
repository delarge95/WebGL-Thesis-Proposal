---
tipo: auditoria_exhaustiva
area: documentacion_vs_implementacion
estado: hallazgos_consolidados
fecha_auditoria: 9_mayo_2026
alcance: informe_final_manuales_scripts_documentacion
---

# AUD — Auditoría Exhaustiva: Incoherencias, Contradicciones y Cambios Necesarios

**Objetivo:** Consolidar discrepancias entre entregables (informe, manuales, documentación) y fuentes de verdad (código, scripts, decisiones de runtime) sin parchear nada aún. Este documento sirve como lista de verificación antes de cerrar el cierre documental final.

**Fecha de auditoría:** 9 de mayo de 2026  
**Scope:** Informe final, manuales técnicos, documentación de desarrollo, scripts de la app  
**Metodología:** Lectura exhaustiva de documentos vs. grep de código + lectura de scripts clave

---

## 1. INCOHERENCIA CRÍTICA: BLITS Y EDGE DETECTION (NO DOCUMENTADOS)

### 1.1 Hallazgo

**Fuente de verdad (Código):** `EdgeDetectionFeature.cs` existe en:

```
e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\EdgeDetectionFeature.cs
```

**Contenido real:**

- Clase: `EdgeDetectionFeature : ScriptableRendererFeature`
- Implementación de **RenderGraph blits**:
  - `RenderGraphUtils.BlitMaterialParameters blitToTemp`
  - `renderGraph.AddBlitPass(blitToTemp, "Edge Detection Blit")`
  - `renderGraph.AddBlitPass(blitBack, "Edge Detection Copy Back")`
- Control global: `EdgeDetectionFeature.GlobalEnabled` (static property)
- Integración con `ViewModeManager`: en `ReapplyCurrentMode()`:
  ```csharp
  bool needsEdges = currentMode == ViewMode.Blueprint;
  EdgeDetectionFeature.GlobalEnabled = needsEdges;
  ```

**Dónde NO aparece documentado:**

1. Manual técnico (`manual_tecnico.tex`): SIN MENCIÓN
2. WEBGL_OPTIMIZATION_MANUAL.md: SIN MENCIÓN específica de blits/edge detection
3. WEBGL_BUILD_GUIDE.md: SIN MENCIÓN
4. Informe_final capítulo 04 (Desarrollo): SIN MENCIÓN de renderer features
5. README.md (desarrollo): SIN MENCIÓN

**Dónde SÍ aparece mencionado:**

- HOJA_DE_RUTA.md: "Crear URP Renderer Feature para dibujar líneas de contorno basadas en profundidad y normales"
- 02_arquitectura_shaders_visualizacion.md: "Full Screen Pass Renderer Feature"
- WEBGL_OPTIMIZATION_MANUAL.md (línea 217): "Renderer Features revisados ✅" (pero sin detalles)

### 1.2 Impacto

- **Manual técnico incompleto:** No documenta una feature ejecutada (renderer pass con blits)
- **Arquitectura visual oculta:** Los modos visuales Blueprint usan edge detection mediante blits, pero el lector del manual no lo sabría
- **Debugging futuro complejo:** Si alguien necesita optimizar o modificar edge detection, no hay fuente documental clara

### 1.3 Cambios recomendados

**En Manual Técnico (`manual_tecnico.tex`):**

- Agregar sección: **"Renderer Features y Post-Processing Passes"**
- Documentar `EdgeDetectionFeature`:
  - Cuándo se activa (Blueprint mode)
  - Qué hace (blits con detección de bordes depth/normal)
  - Cómo se controla (via `ViewModeManager.ReapplyCurrentMode()`)
  - Impacto de performance (costo por frame en Blueprint)

**En WEBGL_OPTIMIZATION_MANUAL.md:**

- Expandir sección "Renderer Features revisados":
  - Listar EdgeDetectionFeature como una feature implementada
  - Notar que está acoplado a ViewMode.Blueprint
  - Documentar cómo desactivarlo si es necesario

**Dónde incorporar en informe:**

- Capítulo 04 (Desarrollo), nueva subsección bajo "Shaders y visualización":
  - "Renderer Features: Edge Detection y Post-Processing"
  - Explicar el rol de blits en el pipeline URP para modos analíticos

---

## 2. INCOHERENCIA: NOMENCLATURA Y CONTEO DE "MODOS VISUALES"

### 2.1 Hallazgo

**Fuente de verdad (Código en ViewModeManager.cs):**

```csharp
public enum ViewMode
{
    Realistic,      // 1
    XRay,           // 2
    Blueprint,      // 3
    SolidColor,     // 4
    Wireframe,      // 5
    Ghosted,        // 6
    Thermal         // 7
}
```

**Total: 7 modos visuales definidos en runtime.**

**Dónde aparece documentado:**

- `INF_EST_70_Guion_Final_Parte2.md`: "Siete modos visuales:" (lista exacta: Realistic, X-Ray, Solid Color, Wireframe, Ghosted, Thermal, Blueprint) ✅ **CORRECTO**
- Portafolio (08_Portafolio_Tech_Artist.md): "7 modos de visualización" ✅ **CORRECTO**
- Documentación de desarrollo: ARCHITECTURE.md lista 7 ✅ **CORRECTO**

**Dónde NO aparece claro o está incompleto:**

- Manual técnico: NO menciona explícitamente "7 modos" en sección de managers
- WEBGL_BUILD_SETTINGS.md: Lista shaders pero no agrupa como "7 modos"
- Documentación de desarrollo antigua (BACKLOG_NOTAS_NO_RESUELTAS_2026-04-16.md): Puede haber referencia a un número diferente

**Inconsistencia resuelta:** El guión final 70 ESTÁ correcto. No hay discrepancia real, pero el manual técnico debería ser explícito.

### 2.2 Impacto

- Bajo (guión está correcto)
- Pero manual técnico está incompleto

### 2.3 Cambios recomendados

**En Manual Técnico:**

- Sección ViewModeManager: Listar explícitamente los 7 modos y describirlos
- Referencia cruzada: "ver INF_EST_70_Guion_Final_Parte2.md para descripción pública de cada modo"

---

## 3. INCOHERENCIA: NOMENCLATURA "INSPECT" vs "TOOLS"

### 3.1 Hallazgo

**Fuente de verdad (Código):**

- En `UIModeController.cs` y runtime: El primer modo de interacción se llama **"Tools"** en el código
- En la UI y documentación académica: Se llama **"Inspect"** en narrativa

**Referencia en INF_EST_70_Guion_Final_Parte2.md:**

```
### MÓDULO 1: INSPECT
```

**Referencia en INF_EST_79_Incongruencias_Documentacion.md:**

```
## 1. Nomenclatura de modos de interacción
- Incongruencia: En el runtime real, `UIModeController` trabaja con `Tools`, `Analyze` y `Studio`,
mientras que el guión habla de `Inspect`, `Analyze` y `Studio`.
```

**Nota:** Este documento ya está en la lista de incongruencias detectadas, pero requiere acción.

### 3.2 Impacto

- Riesgo de confusión si el jurado pregunta por nombres técnicos exactos vs. narrativos

### 3.3 Cambios recomendados

**En INF_EST_70_Guion_Final_Parte2.md:**

- Aclaración interna: "En la implementación runtime, este módulo se denomina 'Tools' en el código. Aquí lo denominamos 'Inspect' por claridad académica/narrativa."

**En Manual Técnico:**

- Agregar tabla de equivalencias:
  | Término Académico | Término Runtime |
  | --- | --- |
  | Inspect | Tools |
  | Analyze | Analyze |
  | Studio | Studio |

---

## 4. INCOHERENCIA: THERMAL Y NIVEL DE PRESENTACIÓN

### 4.1 Hallazgo

**En documentación académica (guiones, informe):**

- Thermal se describe como **"visualización educativa heurística, no simulación FEA"**
- Frase clave en INF_EST_70_Guion_Final_Parte2.md:
  ```
  "Importante: no es simulación FEA. Es una lupa visual de tendencias.
  Para análisis térmico exacto, necesitarías herramientas de ingeniería especializadas."
  ```

**En Marco Referencia (Informe Final capítulo 02_marco_referencia.tex):**

- Se presenta una ecuación matemática compleja para thermal:
  ```latex
  T_{\text{vis}} = saturate\left(T_{\text{base}}\, s(\mathbf{x}) - \phi_{\text{edge}}(\mathbf{x},\mathbf{v}) +
  \eta(\mathbf{x},t) + \delta_{\text{textura}}\right)
  ```
- Nota: "Esta ecuación describe una capa de visualización, no una simulación termo-mecánica completa."

**En documentación de investigación:**

- `investigacion/10_workflow_integracion_cad_unity.md` menciona:
  - `_MinTemp` / `_MaxTemp` como valores estáticos
  - Calentamiento progresivo según `thermalWarmupSeconds`
  - Perfiles: "cold" / "mild" / "warm" / "hot" / "gradient"

**Inconsistencia detectada:**

- Los detalles técnicos (ecuaciones, perfiles) no están expuestos en el guión o manual públicos
- El guión simplifica mucho, lo cual es correcto para defensa pública
- Pero la documentación interna tiene complejidad no comunicada al público

### 4.2 Impacto

- **Bajo:** La estrategia es correcta (educar públicamente, complejidad en docs internas)
- **Medio:** Si se pide explicación técnica profunda, la persona que defienda debe conocer la ecuación completa

### 4.3 Cambios recomendados

**En Manual Técnico:**

- Sección ThermalViewController: Documentar la ecuación de visualización
- Claridad: "Para auditoría y defensa técnica, ver capítulo 02 del Informe Final"

**En Informe Final (si está accesible en defensa):**

- Marco Referencia ya está bien; mantener como está

---

## 5. INCOHERENCIA: DOCUMENTACIÓN DE ARQUITECTURA INCOMPLETA

### 5.1 Hallazgo

**Managers garantizados en Manual Técnico vs. Código:**

Manual técnico lista (en `manual_tecnico.tex`):

- InputManager
- SelectionManager
- FastenerRegistry
- FastenerInspectionManager
- ExplodedViewManager
- PartCatalogManager
- NotificationManager
- HotspotManager
- ViewModeManager
- EnvironmentController
- CrossSectionManager
- PartVisibilityManager
- DroneStateController
- ThermalSimulationManager
- ThermalViewController

**Managers mencionados en código pero NO en el manual:**

- `UIModeController`: Orquesta Inspect/Analyze/Studio (crítico)
- `AppStateMachine`: "única fuente de verdad de estado" (según PHASE3_CHANGELOG.md)
- `OrbitCameraController`: Controla navegación
- `AssemblyGuideManager`: Existe en runtime (mencionado en guión final)
- `BillOfMaterialsManager`: Existe en runtime
- `ConnectionPointsViewer`: Existe en runtime
- `AssemblyChecklist`: Existe en runtime

**Documentación de auditoría (RuntimeDroneSceneAuditor.cs):**

- Existe un auditor que verifica:
  - ExplodablePart anchors
  - FastenerRuntimeMarker
  - Bounds
  - Fastener catalog

**Nunca documentado en manual técnico.**

### 5.2 Impacto

- Manual técnico está **incompleto**
- Nuevos desarrolladores no tendrían referencia clara de qué managers existen

### 5.3 Cambios recomendados

**En Manual Técnico:**

- Expandir tabla de managers en sección "Managers garantizados en runtime"
- Agregar:
  - AppStateMachine
  - UIModeController
  - OrbitCameraController
  - AssemblyGuideManager
  - BillOfMaterialsManager
  - ConnectionPointsViewer
  - AssemblyChecklist
  - RuntimeDroneSceneAuditor (editor only)

**Nueva sección:**

- "Editor Tools and Audit": RuntimeDroneSceneAuditor, Project Setup Wizard

---

## 6. INCOHERENCIA: CONTROLADORES UI NO DOCUMENTADOS O INCOMPLETOS

### 6.1 Hallazgo

Manual técnico lista:

- UIManager
- UIHeroController
- UIDetailsSheet
- UIModeController
- InspectModeHandler
- AnalyzeModeHandler
- StudioModeHandler
- UIAnalyzePanel
- UIEnvironmentPanel
- UICrossSectionPanel
- OnboardingController
- OnboardingAnimationView

**Pero en código existen adicionalmente:**

- `UIPopupController` (menciona PHASE3_CHANGELOG: "Nuevo 273 líneas")
- Posiblemente otros controladores no mencionados

**Status de verificación:**

- PHASE3_CHANGELOG.md confirma: "T2: Verificar paneles 'huérfanos' → Confirmado: NO son huérfanos, instanciados por `UIPopupController`"

### 6.2 Impacto

- Manual podría estar desactualizado si hay más controladores no listados

### 6.3 Cambios recomendados

**En Manual Técnico:**

- Hacer grep exhaustivo de classes `extends UIManager` o patrones similares
- Completar tabla con UIPopupController y cualquier otro que falte

---

## 7. INCOHERENCIA: BLENDER WORKFLOW Y EXPORT NO DOCUMENTADO EN MANUAL

### 7.1 Hallazgo

Existe documentación completa en:

- `Blender_Final_Bake_Export_Unity_Workflow.md`
- `blender_bake_target_setup.py`
- `blender_pack_x500_mask.py`
- `blender_runtime_manifest_exporter.py`

**En Manual Técnico:** CERO mención de:

- Proceso de baking en Blender
- Scripts de exportación
- Validación de manifests
- Reconciliación de fasteners

### 7.2 Impacto

- Reproducibilidad: Si alguien necesita repetir el pipeline CAD→Web, no tiene fuente clara en el manual técnico

### 7.3 Cambios recomendados

**En Manual Técnico - Nueva Sección:**

- "Apéndice: Pipeline CAD a Unity (Blender)"
  - Referencia cruzada: "Ver `Blender_Final_Bake_Export_Unity_Workflow.md`"
  - Resumen de los 3 scripts Python
  - Punto de entrada: "Tools > Import Final Runtime Drone Model"

---

## 8. INCOHERENCIA: DOCUMENTACIÓN DE FASTENERS INCOMPLETA

### 8.1 Hallazgo

**En código:**

- Existe `FastenerRegistry`, `FastenerInspectionManager`, `FastenerRuntimeMarker`
- Existe lógica de "instancias primitivas" vs. "maestros"
- Existe gestión de parentCanonicalPartId

**En Manual Técnico:**

- Se menciona brevemente
- No hay detalle de cómo se resuelve instanceId → family → recipe

**En documentación:**

- `holybro_fastener_instances.json` describe estructura
- `holybro_selection_hierarchy.json` define fastener groups
- Pero manual técnico no integra esta información

### 8.2 Impacto

- Complejo de entender sin lectura de código fuente

### 8.3 Cambios recomendados

**En Manual Técnico:**

- Nueva subsección bajo FastenerRegistry:

  ```
  ### Resolución de instancias de fasteners en runtime

  1. Usuario hace clic en fastener primitivo
  2. SelectionManager emite evento con instance ID
  3. FastenerRegistry resuelve: instanceId → (family, recipe, parentCanonicalPartId)
  4. FastenerInspectionManager reemplaza proxy con modelo procedural
  5. UIDetailsSheet llena ficha técnica
  6. Al deseleccionar, se restaura el proxy

  Referencias:
  - holybro_fastener_instances.json (runtime data)
  - holybro_selection_hierarchy.json (semantic grouping)
  - FastenerInspectionManager.cs (replacement logic)
  ```

---

## 9. CAMBIO NECESARIO: INCORPORAR BLITS Y EDGE DETECTION EN INFORME

### 9.1 Dónde incorporar

**En Capítulo 04 (Desarrollo):**

- Nueva subsección bajo "Visualización Técnica y Shaders":
  - Título: "Renderer Features: Edge Detection y Post-Processing en Blueprint Mode"
  - Contenido:
    - Problema: Blueprint mode necesita detectar bordes (depth + normals) para que el usuario lea la estructura
    - Solución: ScriptableRendererFeature con RenderGraph blits
    - Implementación: EdgeDetectionFeature.cs orquesta dos blits (src → temp, temp → src)
    - Integración: ViewModeManager activa/desactiva globalmente según modo
    - Performance: Costo medible en Blueprint (~ X ms según profiler) vs. otros modos

### 9.2 Narrativa recomendada

```
La visualización Blueprint no se limita a cambiar materiales. Para que los bordes
sean legibles sin distraer, se implementó un Renderer Feature basado en RenderGraph
que ejecuta dos blits post-procesamiento:

1. Detección de discontinuidades (depth + normal edges)
2. Copia de resultado de vuelta al framebuffer

Este enfoque evita crear una malla adicional de outlines (costosa en GPU) y mantiene
el edge detection como un paso del pipeline URP que se activa solo cuando es necesario.
```

---

## 10. INCOHERENCIAS MENORES O YA RESUELTAS

| #    | Tema                                                 | Estado                          | Acción                               |
| ---- | ---------------------------------------------------- | ------------------------------- | ------------------------------------ |
| 10.1 | Nomenclatura pieza/anchor (16/28/30/257)             | Documentado en informe cap. 04  | ✅ OK                                |
| 10.2 | Memory leak prevention (AddCleanup)                  | Documentado en PHASE3_CHANGELOG | ⚠️ No en manual técnico; añadir nota |
| 10.3 | InputBlocked y protección UI                         | Documentado en manual técnico   | ✅ OK                                |
| 10.4 | Herramientas de auditoría (RuntimeDroneSceneAuditor) | Existe código                   | ❌ NO en manual; añadir apéndice     |
| 10.5 | Contradicciones informe vs. código                   | Ya reconocidas en INF_EST_79    | ✅ Referencia cruzada OK             |

---

## 11. LISTA DE ACCIÓN CONSOLIDADA

### Prioridad ALTA (Afecta claridad del informe/manual):

1. **Documentar EdgeDetectionFeature y Blits**
   - Archivos a editar:
     - `manual_tecnico.tex`: Nueva sección "Renderer Features"
     - `Informe_final/chapters/04_desarrollo.tex`: Nueva subsección bajo "Visualización"
     - `WEBGL_OPTIMIZATION_MANUAL.md`: Expandir "Renderer Features"
   - Nivel de detalle: Técnico pero accesible
   - Conexión con informe: "Este documento amplía el capítulo 04 del informe"

2. **Completar tabla de managers en Manual Técnico**
   - Archivos: `manual_tecnico.tex`
   - Agregar: AppStateMachine, UIModeController, OrbitCameraController, AssemblyGuideManager, BillOfMaterialsManager, ConnectionPointsViewer, AssemblyChecklist
   - Agregar sección aparte: Editor Tools

3. **Incorporar Pipeline CAD-Blender-Unity en Manual Técnico**
   - Archivos: `manual_tecnico.tex` (apéndice)
   - Referencia: `Blender_Final_Bake_Export_Unity_Workflow.md`
   - Scripts mencionados: blender_bake_target_setup.py, blender_pack_x500_mask.py, blender_runtime_manifest_exporter.py

### Prioridad MEDIA (Mejora completitud):

4. **Crear tabla de equivalencias técnica/narrativa** (Inspect ↔ Tools, etc.)
   - Archivos: `manual_tecnico.tex` (tabla de glosario)

5. **Expandir FastenerRegistry en Manual Técnico**
   - Archivos: `manual_tecnico.tex`
   - Agregar: resolución de instanceId → family → recipe

6. **Documenta Memory Leak Prevention (AddCleanup)**
   - Archivos: `manual_tecnico.tex`
   - Fuente: PHASE3_CHANGELOG.md

### Prioridad BAJA (Clarificaciones):

7. **Verificar completitud de UI Controllers**
   - Hacer grep de todos los clases UI\*Controller.cs
   - Actualizar tabla en manual técnico

8. **Reconciliar niveles de detalle en Thermal**
   - Manual técnico: Incluir ecuación completa (ya en Informe cap 02)
   - Guión: Mantener simplicidad actual (educativa, no técnica)

---

## 12. CAMBIOS ESPECÍFICOS EN DOCUMENTOS

### 12.1 Manual Técnico (`manual_tecnico.tex`)

**Después de subsección "Managers garantizados en runtime":**

```latex
\subsection{Complemento: Managers, Controladores y Editor Tools omitidos}

Los siguientes componentes no se incluyeron en la sección anterior pero son parte de la arquitectura:

\begin{longtable}{p{5.3cm}p{7.7cm}}
\toprule
\textbf{Módulo} & \textbf{Responsabilidad} \\
\midrule
\texttt{AppStateMachine} & única fuente de verdad para el estado global de la app (Loading, Intro, Exploration, etc.) \\
\texttt{UIModeController} & orquesta la activación/desactivación de módulos Inspect, Analyze, Studio \\
\texttt{OrbitCameraController} & navegación 3D orbital, zoom y recentrado \\
\texttt{AssemblyGuideManager} & sistema de guía paso-a-paso para ensamblaje \\
\texttt{BillOfMaterialsManager} & generación y exportación de listas de materiales \\
\texttt{ConnectionPointsViewer} & visualización de puntos de conexión entre piezas \\
\texttt{AssemblyChecklist} & checklist interactiva de pasos \\
\bottomrule
\end{longtable}

\subsubsection{Editor Tools (solo en Unity Editor)}

Para auditoría y debugging de la escena final:

\begin{longtable}{p{5.3cm}p{7.7cm}}
\toprule
\textbf{Herramienta} & \textbf{Función} \\
\midrule
\texttt{RuntimeDroneSceneAuditor} & valida jerarquía, anchors, fasteners primitivos, bounds y cobertura \\
\texttt{ProjectSetupWizard} & configuración inicial de managers y escena \\
\bottomrule
\end{longtable}

Invocación: \texttt{Tools > Audit > Runtime Drone Scene Audit} genera reporte en \texttt{Reports/}.
```

**Nueva sección (después de "Arquitectura runtime"):**

```latex
\section{Post-Processing y Renderer Features}

\subsection{Edge Detection en Blueprint Mode}

El modo Blueprint utiliza un Renderer Feature para detectar y resaltar bordes del modelo,
mejorando la legibilidad de la geometría sin depender de mallas adicionales. La implementación
se basa en \textit{RenderGraph blits} que operan sobre buffers de profundidad y normales.

\subsubsection{Implementación técnica}

\begin{itemize}
    \item \textbf{Clase:} \texttt{EdgeDetectionFeature : ScriptableRendererFeature}
    \item \textbf{Pipeline:}
    \begin{enumerate}
        \item Blit entrada (cameraColorHandle) → texture temporal con material de detección
        \item Blit temporal → salida (cameraColorHandle) con Blitter neutral
    \end{enumerate}
    \item \textbf{Control:} Activado por \texttt{ViewModeManager} cuando CurrentMode == Blueprint
\end{itemize}

\subsubsection{Propiedades configurables}

\begin{itemize}
    \item Edge Color
    \item Edge Thickness
    \item Depth Threshold (sensibilidad a bordes de profundidad)
    \item Normal Threshold (sensibilidad a discontinuidades de normales)
\end{itemize}

\subsubsection{Costo de performance}

Medible via Unity Frame Debugger. El pass se ejecuta solo en Blueprint mode; otros modos
\texttt{GlobalEnabled = false} para ahorro de ciclos GPU.

Referencias: \texttt{Assets/Scripts/Core/Managers/EdgeDetectionFeature.cs}
```

### 12.2 Informe Final (`04_desarrollo.tex`)

**Nueva subsección bajo "Visualización Técnica y Shaders":**

```latex
\subsubsection{Renderer Features y Post-Processing: Edge Detection}

Para el modo Blueprint, la visualización requiere que los bordes del modelo sean claramente
legibles sin sobrecargar la geometría. La solución implementada fue un \textit{Renderer Feature}
de URP que ejecuta dos operaciones (blits) sobre el framebuffer:

\begin{enumerate}
    \item Detección de discontinuidades utilizando buffers de profundidad y normales del frame actual.
    \item Copia del resultado de vuelta al framebuffer destino.
\end{enumerate}

Este enfoque es más eficiente que crear geometría adicional de outlines. Se controla
globalmente desde \texttt{ViewModeManager}, que activa o desactiva el feature dependiendo
del modo visual actual.

\textbf{Referencia técnica:} Ver Manual Técnico, sección Post-Processing y Renderer Features.
```

### 12.3 WEBGL_OPTIMIZATION_MANUAL.md

**Expandir sección "Renderer Features revisados":**

```markdown
### Renderer Features revisados ✅

#### EdgeDetectionFeature

- **Estado:** Implementado y activo en Blueprint mode
- **Propósito:** Detectar y resaltar bordes del modelo para mejorar legibilidad en modo técnico
- **Tipo:** RenderGraph-based post-processing con blits
- **Control:** Activado por ViewModeManager cuando CurrentMode == Blueprint
- **Performance:** Bajo costo (2 blits por frame en Blueprint); desactivado en otros modos
- **Referencia:** `Assets/Scripts/Core/Managers/EdgeDetectionFeature.cs`

#### Recomendaciones para optimización

- Si Blueprint mode mostrara perfor problems, reducir EdgeThickness o aumentar depthThreshold
- Alternativa: Deshabilitar feature vía código en Quality Settings bajos (móvil)
```

---

## 13. VERIFICACIÓN FINAL DE CONSISTENCIA

Después de aplicar los cambios recomendados, el siguiente checklist debe estar 100% marcado:

- [ ] EdgeDetectionFeature documentado en manual técnico
- [ ] EdgeDetectionFeature documentado en informe cap. 04
- [ ] Tabla de managers completa (incluyendo AppStateMachine, UIModeController, etc.)
- [ ] Tabla de equivalencias técnica/narrativa (Inspect ↔ Tools)
- [ ] Pipeline CAD-Blender-Unity explicado en manual (apéndice)
- [ ] FastenerRegistry explicado con resolución de instanceId
- [ ] Editor Tools (RuntimeDroneSceneAuditor) mencionados
- [ ] Thermal: Ecuación y niveles de detalle reconciliados
- [ ] Blits / RenderGraph: NO hay referencias pendientes sin resolver
- [ ] Todos los managers mencionados en código están en tabla de manual
- [ ] Todos los UI Controllers mencionados en código están en tabla de manual

---

## 14. NOTAS FINALES

1. **Este documento NO parchea nada.** Solo consolida qué debe ser parchado.
2. **Fuentes de verdad usadas:**
   - ViewModeManager.cs: 7 modos (verdad absoluta)
   - EdgeDetectionFeature.cs: blits existen (verdad absoluta)
   - Manual técnico: documento oficial que se revisa
   - Informe final: documento oficial que se revisa
   - PHASE3_CHANGELOG.md: managers documentados
   - PHASE3_CHANGELOG.md: refactoring ejecutado
3. **No hay suposiciones.** Cada hallazgo está ligado a artefacto verificable.
4. **Todos los cambios son de DOCUMENTACIÓN, no de código.** El código runtime es correcto; es la documentación la que está incompleta.

---

**FIN DEL DOCUMENTO**

## 15. AUDITOR�A AMPLIADA: DOCUMENTOS SECUNDARIOS OMITIDOS

### 15.1 Bit�cora (log.md)

- **Hallazgo:** No hay registro del descubrimiento arquitect�nico de hoy ni de las discrepancias.
- **Cambio:** Agregar una entrada con fecha de hoy documentando el descubrimiento de la arquitectura oculta de RenderGraph/Blits y la consolidaci�n de cambios.

### 15.2 Manual de Usuario (manual_usuario.tex)

- **Hallazgo:** Utiliza correctamente 'Inspect / Analyze / Studio', pero no advierte al usuario sobre el efecto visual esperado del modo Blueprint (bordes procesados).
- **Cambio:** Expandir brevemente la descripci�n de los 7 modos visuales desde la perspectiva del usuario, aclarando el aspecto diferencial del modo Blueprint y Thermal.

### 15.3 Changelogs (PHASE3_CHANGELOG.md y generales)

- **Hallazgo:** El log exhaustivo de refactorizaci�n (Phase 3) omiti� por completo registrar la implementaci�n de \EdgeDetectionFeature.cs\ y el uso de blits.
- **Cambio:** A�adir retroactivamente una secci�n en el registro de cambios reconociendo la adici�n del \ScriptableRendererFeature\ de post-procesamiento.

### 15.4 Gu�as de Estudio Obsidian (VARIANTE*ESTUDIO_OBSIDIAN.md e INF_EST*\*)

- **Hallazgo:** Los m�dulos de estudio (espec�ficamente \INF_EST_33_Shaders_ViewModes_Entornos.md\) no incluyen RenderGraph ni Blits para preparar al expositor.
- **Cambio:** Actualizar el m�dulo \INF_EST_33\ para incluir la explicaci�n de post-processing y blits, garantizando que quien estudie para la defensa conozca la realidad del c�digo.

### 15.5 Propuesta (Docs Hist�ricos)

- **Hallazgo:** La propuesta contiene m�tricas y conteos superados (ej. cantidad de piezas tempranas).
- **Cambio:** CERO cambios en la propuesta en s� (es un documento protegido/hist�rico), pero la documentaci�n t�cnica y el informe final ya hacen el contraste correcto (16 vs 28 piezas). No se requiere acci�n directa sobre los \.tex\ de la propuesta.
## 16. INCOHERENCIA CRÍTICA: LÍNEA DE TIEMPO DIVIDIDA (2025 vs 2026)
- **Hallazgo:** El proyecto narra su ejecución en 3 periodos históricos incompatibles simultáneamente.
  - desarrollo/README.md (Línea 10): "Duración: 6 meses (Enero - Junio 2025)".
  - Informe_final/CRONOGRAMA.md (Línea 17): Diagrama Gantt en hardcode para "Julio 2025 a Diciembre 2025".
  - Logs base, auditorías (desarrollo/docs/, Cerebro_Digital/log.md) y Propuesta LaTeX: Fechados explícitamente entre "Febrero 2026 y Mayo 2026".
- **Impacto:** Si un evaluador cruza el README, el CRONOGRAMA y los logs diarios, encontrará que el proyecto afirma haber terminado en junio 2025, haber empezado en julio 2025, y estar desarrollándose diariamente en abril/mayo 2026.

## 17. INCOHERENCIA DE RENDIMIENTO: 30 FPS ACADÉMICO VS 60 FPS COMERCIAL
- **Hallazgo:** Las promesas de rendimiento varían según el documento de "venta".
  - Informe_final/chapters/04_desarrollo.tex (Líneas 110, 115) y WEBGL_BUILD_SETTINGS.md (Línea 279): Definen un target estricto de 30 FPS o un rame time de < 33.33ms.
  - desarrollo/docs/investigacion/Optimización brutal del portafolio [...] .md (Líneas 47, 67): Reclama como mérito técnico explícito haber mantenido "60 FPS en WebGL en laptops integradas" y en móviles.
  - desarrollo/docs/08_Portafolio_Tech_Artist.md (Línea 236): Usa ambas métricas de forma confusa, reclamando 30+ FPS pero jactándose luego de <33ms frame time (lo cual es matemáticamente 30 FPS, no 60).
- **Impacto:** Falsa declaración comercial vs académica.

## 18. PLACEHOLDERS (TEXTO FANTASMA) EN EL INFORME FINAL (CAPÍTULO 4)
- **Hallazgo:** El documento  4_desarrollo.tex, que se asume compilable y cerrado, está plagado de placeholders directos impresos en el PDF, tanto en métricas como en imágenes.
  - En tablas de rendimiento (ej. Línea 473): Frame time & < 33.33 ms & [pendiente post-freeze].
  - Existen al menos 20 instancias de etiquetas como {Estado esperado: evidencia del pipeline CAD evaluado durante el desarrollo.}.
- **Impacto:** El "Informe Final" es un borrador avanzado con huecos de información crítica.

## 19. REAFIRMACIÓN DESCUIDADA DE LA CIFRA HISTÓRICA "16 PIEZAS"
- **Hallazgo:** La tesis formal e insumos académicos ( 4_desarrollo.tex y GUIA_Y_ANALISIS_AUDITS.md) dedican párrafos enteros a explicar que la cifra de "16 piezas" fue solo un prototipo temprano y que el conteo real final que debe usarse es 28 canónicas / 30 anchors / 257 renderers.
  - Sin embargo, los documentos de salida profesional  8_Portafolio_Tech_Artist.md (e.g., Líneas 39, 59, 178, 273), están basando el peso del proyecto comercial vendiendo un asset tracker de **16 piezas**. 
- **Impacto:** El portafolio se auto-desvaloriza reduciendo su carga probatoria en un 40% (de 28 a 16 piezas) simplemente por no limpiar datos legados.
