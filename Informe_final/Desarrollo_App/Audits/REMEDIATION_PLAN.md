# 🔧 PLAN DE REMEDIACIÓN — PRIORIZADO POR SEVERIDAD

## Basado en los 4 Pilares de Auditoría (Corregidos)

**Fecha:** 2025-07-15  
**Branch:** `feature/phase2-ux-redesign`  
**Principio:** Foundations first → Surface last. Crítico → Alto → Medio → Bajo.  
**Nota:** Este plan se genera tras el "audit-of-audit" que corrigió errores factuales en los 4 reportes originales. Los hallazgos aquí reflejados son los **validados**.

> **⚠️ STALE (Mar 3, 2026):** Este plan fue creado contra el estado del proyecto al Feb 19. Desde entonces, la mayoría de ítems de severidad CRITICAL y HIGH han sido implementados: ServiceLocator (H01), mode handler extraction (H02), EventBus leak detection (H03), dead code removal (H04), Update() elimination (H08), 44px touch targets (C04), WCAG audit, y 14 feature tickets (F3-00 a F3-13). Ver `CHANGELOG.md` secciones Feb 23 – Mar 3 para el estado actual.

---

## Resumen Ejecutivo

| Severidad  | Hallazgos Válidos | Effort Total Est. | Estado        |
| ---------- | ----------------- | ----------------- | ------------- |
| 🔴 Crítico | 5                 | ~6–8 horas        | ✅ Completado |
| 🟠 Alto    | 11                | ~14–18 horas      | 🔄 En curso   |
| 🟡 Medio   | 25                | ~16–22 horas      | 🔲 Pendiente  |
| 🔵 Bajo    | 16                | ~10–14 horas      | 🔲 Pendiente  |
| **Total**  | **57**            | **~46–62 horas**  |               |

> **Nota (2026-03-02):** Tras cruzar exhaustivamente los 4 reportes de auditoría contra este plan,
> se identificaron 30 hallazgos faltantes que han sido incorporados. Los originalmente listados
> se mantienen con su numeración; los nuevos se añaden al final de cada fase.

---

## FASE 1: CRÍTICOS (Foundations — Semana 1)

> Estos bloquean la defensa académica o causan bugs en producción.

### 1.1 🔴 ARCH-C01: Enumeración `VisualMode` Duplicada

- **Pilar:** Arquitectura
- **Problema:** `VisualMode` definida en 2 archivos (`VisualMode.cs` y `ShaderModeConfig.cs`). Ambigüedad en compilación, riesgo de bugs silenciosos.
- **Fix:** Eliminar la definición en `ShaderModeConfig.cs`, mantener la canónica en `VisualMode.cs`. Actualizar `using` statements.
- **Esfuerzo:** 30 min
- **Archivos:** `ShaderModeConfig.cs`, `VisualMode.cs`

### 1.2 🔴 ARCH-C02: Triple Publicación de Eventos

- **Pilar:** Arquitectura
- **Problema:** Cambio de modo publica 3 eventos separados (C# event + EventBus + callback), causando posible triple-rendering.
- **Fix:** Consolidar en un solo canal (preferir EventBus). Eliminar los otros dos dispatch points. Verificar que todos los suscriptores migren al canal único.
- **Esfuerzo:** 1–2 horas
- **Archivos:** `UIModeController.cs`, `AppStateMachine.cs`, managers suscriptores

### 1.3 🔴 UX-C01: Bottom Sheet — 12 Campos sin Agrupación

- **Pilar:** UX/UI
- **Problema:** El panel de detalles muestra ~12 campos en lista plana, excediendo la capacidad de procesamiento visual.
- **Fix:** Agrupar en 3 secciones colapsables: "Identificación" (nombre, categoría, número de parte), "Especificaciones" (dimensiones, peso, material), "Función" (descripción, función).
- **Esfuerzo:** 1.5–2 horas
- **Archivos:** `UIDetailsSheet.cs`, `MainLayout.uxml`, `DetailsSheet.uss`

### 1.4 🔴 UX-C03: Targets Táctiles < 44px

- **Pilar:** UX/UI
- **Problema:** 33 declaraciones de elementos interactivos menores a 44px (mínimo WCAG/Apple HIG).
- **Fix:** Agregar `min-height: 44px` a todos los botones/elementos interactivos en sub-paneles. Auditar con DevTools táctil.
- **Esfuerzo:** 1 hora
- **Archivos:** `*.uss` (múltiples stylesheets)

### 1.5 🔴 UX-C04: Loading/Error UI en C# Puro

- **Pilar:** UX/UI
- **Problema:** Estados de loading y error se crean proceduralmente en C# en lugar de UXML+USS, violando la separación de concerns de UI Toolkit.
- **Fix:** Crear templates UXML para loading spinner y error states. Referenciar desde C# via `VisualTreeAsset.Instantiate()`.
- **Esfuerzo:** 1.5 horas
- **Archivos:** `LoadingUI.cs` (o equivalente), nuevo `LoadingOverlay.uxml`, `ErrorOverlay.uxml`

---

## FASE 1 — ESTADO: ✅ COMPLETADO

> Todos los hallazgos críticos fueron corregidos y pusheados entre commits `12baf4b` y `b2f6be2`.

---

## FASE 2: ALTOS (Structural — Semana 1–2)

> Afectan rendimiento, mantenibilidad o deuda técnica significativa.

### 2.1 🟠 ARCH-H01: Sobreuso de Singletons (6 instancias)

- **Pilar:** Arquitectura
- **Problema:** 6 clases heredan de `Singleton<T>`. Acopla el código y dificulta testing.
- **Fix:** Migrar 3–4 Singletons secundarios a inyección via `ServiceLocator` o constructor. Mantener sólo `GameManager` y `EventBus` como Singletons.
- **Esfuerzo:** 2–3 horas
- **Prioridad real:** Media (funciona, pero es deuda técnica)

### 2.2 🟠 ARCH-H03: `UIModeController` — Clase Monolítica (~400 líneas)

- **Pilar:** Arquitectura
- **Problema:** Controller maneja lógica de 3+ modos de UI. Difícil de mantener.
- **Fix:** Extraer handlers por modo (`InspectModeHandler`, `AnalyzeModeHandler`, `StudioModeHandler`). `UIModeController` se convierte en orquestador.
- **Esfuerzo:** 2–3 horas
- **Archivos:** `UIModeController.cs` → 3–4 archivos nuevos

### 2.3 🟠 ARCH-H04: EventBus sin Detección de Leaks

- **Pilar:** Arquitectura
- **Problema:** `EventBus` no detecta suscriptores que olvidan desuscribirse → memory leaks potenciales.
- **Fix:** Agregar `#if UNITY_EDITOR` check en `OnDestroy` que loguee suscriptores zombies. Agregar contador de suscriptores activos.
- **Esfuerzo:** 1 hora
- **Archivos:** `EventBus.cs`

### 2.4 🟠 ARCH-H05: Archivos Obsoletos No Eliminados

- **Pilar:** Arquitectura
- **Problema:** `ViewModeToolbar.cs`, `EngineerToolbar.cs`, y posiblemente otros → código muerto.
- **Fix:** Buscar usages, confirmar que no se referencian, eliminar. Limpiar `.meta` files.
- **Esfuerzo:** 30 min
- **Archivos:** `ViewModeToolbar.cs`, `EngineerToolbar.cs`

### 2.5 🟠 PERF-H01: `managedStrippingLevel: Minimal` para WebGL

- **Pilar:** Performance
- **Problema:** Stripping mínimo incluye código no usado en el build WASM, inflando tamaño.
- **Fix:** Cambiar a `Medium` o `High` stripping. Crear `link.xml` para preservar tipos necesarios via reflection.
- **Esfuerzo:** 1–2 horas (incluye testing de que nada se rompa)
- **Archivos:** `ProjectSettings/ProjectSettings.asset`, nuevo `link.xml`

### 2.6 🟠 PERF-H02: 20 Métodos `Update()` Activos

- **Pilar:** Performance
- **Problema:** 20+ scripts con `Update()` corriendo cada frame. En WebGL, cada `Update()` es un cruce JS↔WASM.
- **Fix:** Auditar cuáles necesitan realmente Update vs. event-driven. Migrar al menos 10 a callbacks/eventos. Usar polling con intervalo para los restantes.
- **Esfuerzo:** 2–3 horas
- **Archivos:** Múltiples managers y controllers

### 2.7 🟠 PERF-H02\*: `StripUnusedMeshComponents` Desactivado

- **Pilar:** Performance
- **Problema:** `StripUnusedMeshComponents: 0` — tangentes, UV2/UV3, colores de vértice se incluyen aunque los shaders no los usan. ~2–5 MB de mesh data desperdiciada.
- **Fix:** Activar `StripUnusedMeshComponents` en Player Settings. Verificar que ningún shader custom dependa de canales secundarios.
- **Esfuerzo:** 15 min + testing
- **Archivos:** `ProjectSettings/ProjectSettings.asset`

### 2.8 🟠 PERF-H03\*: IL2CPP Compiler Configuration = Debug

- **Pilar:** Performance
- **Problema:** `il2cppCompilerConfiguration: {}` (vacío = Debug por defecto). Build ~20% más lento y ~10% más grande que `Master`.
- **Fix:** Establecer explícitamente `il2cppCompilerConfiguration: 2` (Master) para WebGL.
- **Esfuerzo:** 10 min + testing
- **Archivos:** `ProjectSettings/ProjectSettings.asset`

### 2.9 🟠 PERF-H07\*: QualityManager Es Un No-Op

- **Pilar:** Performance / Arquitectura
- **Problema:** `QualityManager.Update()` corre cada frame calculando FPS pero `ResizeBuffers()` está comentado. Desperdicia 1 `Update()` sin beneficio.
- **Fix:** Eliminar `Update()` o implementar la lógica de resolución adaptativa. Si no se necesita adaptación, desactivar el componente.
- **Esfuerzo:** 30 min
- **Archivos:** `QualityManager.cs`

### 2.10 🟠 UX-H01\*: Fitts's Law — Bottom Pill Gap

- **Pilar:** UX/UI
- **Problema:** Los mode buttons dentro del pill tienen padding que crea gap con el borde inferior de pantalla, aumentando ~20% el tiempo de adquisición por Fitts's Law.
- **Fix:** Reducir padding inferior del pill o anclar al borde inferior cuando no hay sheet activo.
- **Esfuerzo:** 30 min
- **Archivos:** `Theme.uss`, posiblemente `UIManager.cs`

### 2.11 🟠 UX-H03\*: Sin Onboarding / Ayuda Contextual

- **Pilar:** UX/UI
- **Problema:** Sin tutorial de primer uso, sin tooltips `?`, sin leyenda de shortcuts. Crítico para lograr SUS > 68 en evaluación.
- **Fix:** Implementar overlay de primer uso con 3–5 pasos. Agregar botón `?` que muestre leyenda de controles.
- **Esfuerzo:** 2–3 horas
- **Archivos:** Nuevo `OnboardingOverlay.uxml`, `OnboardingController.cs`, `Theme.uss`

---

## FASE 3: MEDIOS (Polish — Semana 2–3)

> Mejoran calidad pero no bloquean funcionalidad.

### 3.1 🟡 UX-M01: 14 Valores Fuera de Grid 4pt

- **Fix:** Redondear a múltiplos de 4px en USS.
- **Esfuerzo:** 30 min

### 3.2 🟡 UX-M04: Sin Badges de Conteo en Filtros

- **Fix:** Agregar `<Label>` con conteo junto a cada botón de categoría.
- **Esfuerzo:** 45 min

### 3.3 🟡 UX-M06: Slider de Explosión sin Etiqueta de Valor

- **Fix:** Agregar label dinámico mostrando % de explosión.
- **Esfuerzo:** 20 min

### 3.4 🟡 UX-M08: Sin Botón "Atrás" / Tecla Escape

- **Fix:** Agregar navegación de retroceso entre modos y sub-paneles.
- **Esfuerzo:** 1 hora

### 3.5 🟡 UX-M09: Sin Indicador Visual de Handle en Bottom Sheet

- **Fix:** Agregar `.sheet-handle` pill indicator en la parte superior del sheet.
- **Esfuerzo:** 15 min

### 3.6 🟡 PERF-M01: `webGLExceptionSupport: FullWithStacktrace`

- **Fix:** Cambiar a `ExplicitlyThrownExceptionsOnly` para producción. Mantener `Full` solo en debug.
- **Esfuerzo:** 5 min + testing

### 3.7 🟡 PERF-M02: Memory Size 512MB

- **Fix:** Reducir `webGLMemorySize` a 256MB o implementar autoGrowMemory.
- **Esfuerzo:** 10 min + testing

### 3.8 🟡 PERF-M03: Sin Custom WebGL Template

- **Fix:** Crear template personalizado con loading bar, favicon, meta tags.
- **Esfuerzo:** 1–2 horas

### 3.9 🟡 PERF-M06: 9 Shaders — Riesgo de Variant Explosion

- **Fix:** Reemplazar `multi_compile` con `shader_feature` en ClippableLit y otros. Solo 3 shaders son user-facing, pero los 9 compilan.
- **Esfuerzo:** 1–2 horas

### 3.10 🟡 UX-H06: 8 Tamaños de Fuente sin Ramp Disciplinado

- **Fix:** Establecer type ramp de 3–4 tamaños (16, 20, 28, 36).
- **Esfuerzo:** 45 min

### 3.11 🟡 ARCH-M01\*: Patrones de Desuscripción Inconsistentes

- **Problema:** Múltiples patrones de cleanup (`_cleanupActions` vs `OnDestroy()` vs `OnDisable()`).
- **Fix:** Estandarizar en un solo patrón (preferir `_cleanupActions` + `OnDestroy`).
- **Esfuerzo:** 1 hora

### 3.12 🟡 ARCH-M04\*: OrbitCameraController Input Duplicado

- **Problema:** Lógica orbit/pan/zoom duplicada entre mouse y touch con magic numbers.
- **Fix:** Extraer `IInputProvider` o unificar la lógica de input.
- **Esfuerzo:** 1–2 horas

### 3.13 🟡 ARCH-M05\*: Double-Click Split entre SelectionManager y UIManager

- **Problema:** Detección de doble-click vive en `UIManager` pero la selección se origina en `SelectionManager`.
- **Fix:** Consolidar lógica de detección en `SelectionManager`.
- **Esfuerzo:** 45 min

### 3.14 🟡 ARCH-M06\*: Magic Numbers en Todo el Codebase

- **Problema:** `targetY = 20f`, `DOUBLE_CLICK_THRESHOLD = 0.35f`, `Time.frameCount % 300`, etc. sin constantes nombradas.
- **Fix:** Extraer a constantes con nombre descriptivo o `[SerializeField]`.
- **Esfuerzo:** 1 hora

### 3.15 🟡 ARCH-M07\*: DronePartData Propiedades Redundantes

- **Problema:** Campos públicos + properties C# que devuelven lo mismo → API confusa.
- **Fix:** Mantener solo properties con backing fields privados, o solo campos `[SerializeField]` públicos.
- **Esfuerzo:** 45 min

### 3.16 🟡 ARCH-M08\*: QualityManager.AdjustQuality() No-Op

- **Problema:** Sistema de resolución adaptativa comentado; la clase solo calcula FPS sin actuar.
- **Fix:** Implementar o eliminar el sistema. Relacionado con PERF-H07\*.
- **Esfuerzo:** 30 min (si se elimina)

### 3.17 🟡 PERF-M02\*: Texture Compression DXT vs ASTC

- **Problema:** DXT no soportado en todos los GPUs móviles. ASTC tiene mejor soporte WebGL.
- **Fix:** Evaluar multi-formato o ASTC como default para WebGL.
- **Esfuerzo:** 30 min (config) + testing visual

### 3.18 🟡 PERF-M03\*: Mip Stripping Desactivado

- **Problema:** `mipStripping: 0` — mips no usados se envían al build. 5–15% ahorro potencial.
- **Fix:** Activar `mipStripping` en Player Settings.
- **Esfuerzo:** 5 min + testing

### 3.19 🟡 PERF-M04\*: Sin Hash File Naming para CDN

- **Problema:** `webGLNameFilesAsHashes: 0` — sin cache busting automático para CDN.
- **Fix:** Activar `webGLNameFilesAsHashes` en Player Settings.
- **Esfuerzo:** 5 min

### 3.20 🟡 UX-H05\*: Sin Feedback Visual en Transiciones de Estado

- **Problema:** Sin toast/notificación ni animación al cambiar de modo. El usuario no sabe si su acción fue registrada.
- **Fix:** Agregar transición visual (fade, scale) y/o toast efímero al cambiar modo.
- **Esfuerzo:** 1–2 horas

### 3.21 🟡 UX-M02\*: Sin Dark/Light Theme Toggle

- **Problema:** Sin tema claro ni modo alto contraste. Relevante para accesibilidad académica.
- **Fix:** Evaluar viabilidad. Si no se implementa, documentar razón (viewer técnico = dark by design).
- **Esfuerzo:** 2–3 horas (si se implementa) / 15 min (si se documenta como decisión de diseño)

### 3.22 🟡 UX-M03\*: Hero Sub-Menus Navigation Mismatch

- **Problema:** Dos paradigmas de navegación: Hero (slide horizontal) vs App (bottom pill + panel vertical).
- **Fix:** Unificar paradigma o documentar como decisión de diseño dual-mode.
- **Esfuerzo:** 1 hora

### 3.23 🟡 UX-M05\*: Cross-Section Dual-Plane Sin Guía

- **Problema:** Sin label explicando "1-2 ejes" ni feedback cuando se activa dual-plane FIFO.
- **Fix:** Agregar tooltip o label contextual.
- **Esfuerzo:** 30 min

### 3.24 🟡 UX-M07\*: Selection Indicator Oculto con Sheet Abierto

- **Problema:** Al abrir el bottom sheet, se oculta el indicador de qué parte está seleccionada.
- **Fix:** Mantener indicador visible (en el header del sheet o como badge).
- **Esfuerzo:** 30 min

### 3.25 🟡 PERF-M05\*: Sin LOD Groups

- **Problema:** Sin LODGroup ni fallback para cámara lejana o dispositivos bajos.
- **Fix:** Bloqueado por materiales pendientes (§15.3 de WEBGL_OPTIMIZATION_MANUAL.md).
- **Esfuerzo:** 2–3 horas (cuando materiales estén listos)
- **Estado:** 🔲 Bloqueado

---

## FASE 4: BAJOS (Nice-to-Have — Semana 3+)

> Mejoras cosméticas y de calidad de código.

### 4.1 🔵 UX-L04: 73 Transiciones sin Tokens Estandarizados

- **Fix:** Estandarizar duraciones a 3 tokens (fast: 150ms, normal: 300ms, slow: 500ms).
- **Esfuerzo:** 1 hora

### 4.2 🔵 UX-L05: Sin Indicador de Scroll en Details Sheet

- **Fix:** Agregar fade gradient o scroll indicator.
- **Esfuerzo:** 30 min

### 4.3 🔵 PERF-L01: vSyncCount: 0 en Low Quality

- **Fix:** Activar vSync o cap FPS a 60 para evitar screen tearing.
- **Esfuerzo:** 5 min

### 4.4 🔵 PERF-L02: Sin Compresión Brotli

- **Fix:** Activar Brotli compression en Build Settings (requiere servidor con support).
- **Esfuerzo:** 10 min + server config

### 4.5 🔵 ARCH-H02 (Downgraded): GameManager — Shell Mínimo

- **Fix:** Informacional. Si se desea, migrar funcionalidad de debug a `DebugManager` dedicado.
- **Esfuerzo:** 30 min (opcional)

### 4.6 🔵 UX-C02: Sin Responsive CSS (0 Media Queries)

- **Fix:** Agregar toggle de clases CSS para breakpoints mobile/tablet/desktop.
- **Esfuerzo:** 2 horas
- **Nota:** Unity UI Toolkit no soporta media queries nativas; requiere C# listener de resolución.

### 4.7 🔵 ARCH-L01\*: UIManager.EnsureManagers() Auto-Create Silencioso

- **Problema:** Auto-crea Singletons faltantes sin warning, enmascarando errores de escena.
- **Fix:** Agregar `Debug.LogWarning` cuando se auto-crea un manager.
- **Esfuerzo:** 15 min

### 4.8 🔵 ARCH-L02\*: StaticInstance OnApplicationQuit No Confiable en WebGL

- **Problema:** `OnApplicationQuit()` no se llama confiablemente en WebGL (cierre de tab del browser).
- **Fix:** Agregar cleanup alternativo via `Application.wantsToQuit` o `window.onbeforeunload` via jslib.
- **Esfuerzo:** 30 min

### 4.9 🔵 ARCH-L04\*: Namespace Inconsistente

- **Problema:** `HotspotManager` y `SmartHotspot` en namespace global en vez de `WebGL.UI`.
- **Fix:** Mover a namespace correcto. Actualizar `using` statements.
- **Esfuerzo:** 20 min

### 4.10 🔵 ARCH-L05\*: ExplodablePart.Start() Weak Guard Logic

- **Problema:** Comparación con `Vector3.zero` falla si la parte está intencionalmente en el origen.
- **Fix:** Usar flag booleano `_isInitialized` en vez de comparación posicional.
- **Esfuerzo:** 15 min

### 4.11 🔵 ARCH-L06\*: Dead Code — Campos Comentados

- **Problema:** Campos comentados en `QualityManager`, `ExplodedViewManager`, `HighlightSystem`.
- **Fix:** Eliminar código muerto comentado.
- **Esfuerzo:** 15 min

### 4.12 🔵 PERF-L02\*: asyncUploadTimeSlice Demasiado Bajo (2ms)

- **Problema:** `asyncUploadTimeSlice: 2` — carga de texturas/meshes muy lenta (2ms/frame).
- **Fix:** Aumentar a 4–8ms en QualitySettings.
- **Esfuerzo:** 5 min

### 4.13 🔵 PERF-L04\*: Sin Application.targetFrameRate Explícito

- **Problema:** Sin `targetFrameRate = 60` explícito; depende solo del browser `requestAnimationFrame`.
- **Fix:** Agregar `Application.targetFrameRate = 60` en `GameManager.Awake()`.
- **Esfuerzo:** 5 min

### 4.14 🔵 UX-L01\*: TopContextLabel Sin Animación

- **Problema:** Cambio de texto instantáneo. Un fade mejoraría calidad percibida.
- **Fix:** Agregar transición de opacidad al cambiar texto.
- **Esfuerzo:** 20 min

### 4.15 🔵 UX-L02\*: Shader Mode Names Demasiado Técnicos

- **Problema:** "SolidColor", "Ghosted", "Thermal" → nombres técnicos poco accesibles.
- **Fix:** Renombrar a "Flat Color", "See-Through", "Heat Map" o equivalentes más claros.
- **Esfuerzo:** 15 min

### 4.16 🔵 UX-L03\*: Sin Haptic Feedback en Touch

- **Problema:** Sin `navigator.vibrate()` para feedback táctil en móviles.
- **Fix:** Agregar vibración breve (10ms) en selección de parte via `.jslib`.
- **Esfuerzo:** 30 min

---

## TAREAS ACADÉMICAS (Paralelas al Desarrollo)

Estas no son bugs de código sino **entregables pendientes para la defensa**:

| #   | Tarea                                               | Prioridad           | Esfuerzo  | Estado                           |
| --- | --------------------------------------------------- | ------------------- | --------- | -------------------------------- |
| A1  | Ejecutar evaluación SUS + NASA-TLX (N=8-12)         | 📌 Tras estabilizar | 2–3 días  | 🟡 Instrumentos listos           |
| A2  | Activar GitHub Pages para `docs/`                   | 📌 Inmediato        | 30 min    | 🟡 Build existe, falta activar   |
| A3  | Reporte formal de KPIs (FPS, polígonos, draw calls) | 📌 Pre-defensa      | 4–6 horas | 🟡 Herramientas existen en app   |
| A4  | Reporte de pipeline (antes/después polígonos)       | 📌 Pre-defensa      | 2–3 horas | 🟡 Pipeline ejecutado, falta doc |
| A5  | Exportar archivos .glb                              | Recomendado         | 1–2 horas | 🔴 No encontrados                |
| A6  | Documentar texel density por parte                  | Recomendado         | 2 horas   | 🔴 Sin documentación             |
| A7  | Medir TTI bajo throttling 4G                        | Recomendado         | 1 hora    | 🔴 Sin medición                  |
| A8  | Actualizar manual técnico (90 scripts, 14,202 LOC)  | Recomendado         | 1 hora    | 🟡 Datos desactualizados         |

---

## Diagrama de Dependencias

```
FASE 1 (Críticos)                    ACADÉMICAS (Paralelas)
├── C01: VisualMode duplicada        ├── A2: Activar GitHub Pages
├── C02: Triple event publish         ├── A4: Reporte pipeline
├── C03: Bottom Sheet 12 campos      └── A8: Actualizar manual
├── C04: Touch targets < 44px
└── C05: Loading/Error en C#
         │
         ▼
FASE 2 (Altos)                        ACADÉMICAS (Post-estabilización)
├── H01: Reducir Singletons           ├── A1: Evaluación SUS/NASA-TLX
├── H02: Refactor UIModeController    ├── A3: Reporte KPIs
├── H03: EventBus leak detection      ├── A5: Exportar .glb
├── H04: Eliminar archivos obsoletos  ├── A6: Texel density
├── H05: Stripping level              └── A7: Medir TTI
└── H06: Reducir Update() calls
         │
         ▼
FASE 3 (Medios) → FASE 4 (Bajos)
```

---

## Métricas de Éxito Post-Remediación

| Métrica                       | Actual  | Objetivo Post-Fix |
| ----------------------------- | ------- | ----------------- |
| Hallazgos Críticos abiertos   | 0 (5→0) | 0 ✅              |
| Hallazgos Altos abiertos      | 11      | 0–2               |
| Build size WebGL (estimado)   | ~50+ MB | < 35 MB           |
| Update() methods activos      | 20      | < 10              |
| Touch targets < 44px          | 33      | 0                 |
| KPIs formalmente documentados | 0/9     | 9/9               |
| Nota ponderada (Scorecard)    | 7.45/10 | ≥ 8.5/10          |

---

_Generado como parte del audit-of-audit — Plan de Remediación basado en hallazgos validados de los 4 pilares._
