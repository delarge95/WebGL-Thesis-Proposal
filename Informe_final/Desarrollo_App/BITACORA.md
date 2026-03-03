# Bitácora de Desarrollo — Registro Integral de Ejecución

> Documento oficial de seguimiento del proyecto "WebGL Drone Viewer".
> Registro detallado de decisiones técnicas, arquitectura y evolución del sistema.
> Alineado con el Cronograma Oficial (Fases 1-7: H2 2025 | Fase 8: Q1 2026).

---

## Fase 1: Investigación y Conceptualización (Julio - Agosto 2025)

### Objetivo
Definir la viabilidad técnica y conceptual de un visualizador interactivo 3D para educación en ingeniería, superando las limitaciones de los manuales estáticos PDF.

### Acciones Realizadas
1.  **Revisión Bibliográfica y del Estado del Arte**:
    -   *Acción*: Análisis de 40+ referencias (papers, tesis, herramientas existentes).
    -   *Justificación*: Identificar brechas en las herramientas actuales (baja interactividad, requisitos de hardware altos).
    -   *Conclusión*: Se opta por **WebGL 2.0** para garantizar acceso universal sin instalación.
2.  **Selección del Stack Tecnológico**:
    -   *Decisión*: Unity 6 (URP) vs Three.js.
    -   *Razón*: Unity ofrece un pipeline de assets más robusto y herramientas de editor visual para configuraciones complejas (ECS/Prefabs), ideal para un prototipo escalable.
    -   *Decisión*: Universal Render Pipeline (URP).
    -   *Razón*: Optimización nativa para plataformas móviles y WebGL, permitiendo shaders personalizados ligeros.

---

## Fase 2: Pipeline de Producción 3D (Septiembre - Octubre 2025)

### Objetivo
Crear un gemelo digital del drone "Spectre X" optimizado para renderizado web en tiempo real.

### Acciones Realizadas
1.  **Modelado Hard-Surface (Blender)**:
    -   *Acción*: Modelado high-poly de la estructura del drone.
    -   *Detalle técnico*: Uso de modificadores booleanos no destructivos para iteración rápida.
2.  **Optimización y Retopología**:
    -   *Acción*: Reducción de polígonos (Target: <100k tris).
    -   *Justificación*: WebGL tiene límites estrictos de memoria y draw calls.
3.  **Baking de Texturas**:
    -   *Acción*: Generación de mapas de Normales, AO y Roughness en texturas atlas 4K.
    -   *Razón*: Simular detalle geométrico sin costo de procesamiento de vértices (PBR Workflow).

---

## Fase 3: Arquitectura de Software - Unity Core (Octubre - Noviembre 2025)

### Objetivo
Establecer una base sólida de código escalable y modular.

### Implementación Técnica
1.  **Patrón Singleton & Managers**:
    -   *Implementación*: `GameManager`, `AudioManager`, `SelectionManager`.
    -   *Por qué*: Centralizar el control de sistemas globales para evitar dependencias cruzadas (Spaghetti code) y facilitar el acceso desde la UI.
2.  **Sistema de Eventos (EventBus)**:
    -   *Implementación*: `Action<T>` events para comunicación desacoplada (`OnPartSelected`, `OnViewModeChanged`).
    -   *Por qué*: Permite que sistemas dispares (UI, Audio, Renderizado) reaccionen a cambios de estado sin conocerse entre sí, mejorando la mantenibilidad.
3.  **Controlador de Cámara (`OrbitCameraController`)**:
    -   *Detalle*: Navegación polar con clamping de ángulos y suavizado (Damping).
    -   *Razón UX*: Evitar que el usuario pierda de vista el modelo o la orientación (Gimbal lock).

---

## Fase 4: Desarrollo de Features Avanzadas (Noviembre - Diciembre 2025)

### Objetivo
Implementar las funcionalidades educativas clave del visualizador.

### Funcionalidades
1.  **Vista Explosionada (Exploded View)**:
    -   *Lógica*: Desplazamiento vectorial de partes basado en su centroide respecto al pivote del drone.
    -   *Implementación*: `ExplodedViewManager` que interpola posiciones locales usando `Mathf.Lerp`.
    -   *Por qué*: Permite visualizar componentes internos sin ocultar el contexto estructural.
2.  **Sistema de Shaders de Visualización**:
    -   *Desarrollo*: 7 shaders custom HLSL.
        -   **Rayos X**: Efecto Fresnel invertido + ZTest Always para ver a través de muros.
        -   **Blueprint**: Detección de bordes (Sobel) en espacio de pantalla + Grid procedural.
        -   **Térmico**: Mapeo de gradiente de color basado en input normalizado.
    -   *Reto Técnico*: Manejo de transparencias en WebGL (Depth sorting issues).
    -   *Solución*: Render queues personalizados y multipass shaders.

---

## Fase 5: Herramientas de Ingeniería y Datos (Diciembre 2025)

### Objetivo
Aportar valor técnico y educativo real más allá de la visualización estética.

### Implementación
1.  **Base de Datos (`DronePartData`)**:
    -   *Estructura*: ScriptableObjects conteniendo metadatos (Peso, Material, Consumo, Torque).
    -   *Por qué*: Separar datos de lógica (Data-Driven Design). Permite editar valores en el Editor sin recompensar código.
2.  **Herramientas de Medición**:
    -   *Acción*: Raycasting de punto a punto para calcular distancias Euclidianas en espacio de mundo.
    -   *Uso*: Verificar compatibilidad de piezas o dimensiones de ensamblaje.

---

## Fase 6: Testing (Pendiente)

- Pruebas Unitarias para lógica de negocio.
- Validación con usuarios (Pruebas de Usabilidad).

---

## Fase 7: Documentación (En Progreso)

- Bitácora (Este documento).
- Manual Técnico.

---

## Fase 8: Optimización y Rediseño "Premium" (Febrero 2026 - Actual)

### Objetivo
Elevar la calidad visual y la experiencia de usuario (UX) para cumplir estándares de la industria (Awwwards/Apple Design).

### Registro de Cambios (Febrero 18, 2026)

1.  **Rediseño de Interfaz (UI Toolkit)**:
    -   *Acción*: Migración de estilos básicos a un "Design System" unificado (`DESIGN_TOKENS.md`).
    -   *Detalle*: Uso de variables CSS (USS) para colores, espaciados y tipografía (`Inter` + `Space Grotesk`).
    -   *Por qué*: Garantizar consistencia visual y facilitar cambios globales (e.g., Modo Oscuro/Claro).
    
2.  **Corrección de Clipping en Menús**:
    -   *Problema*: El contenedor de scroll del menú de dispositivos (`Select Device`) cortaba el contenido derecho.
    -   *Causa*: El ancho del `ScrollView` y su `content-container` no coincidían con el ancho de los botones hijos por defecto.
    -   *Solución*: Se forzó un ancho explícito de **396.5194px** y se eliminaron paddings internos `padding: 0`.
    -   *Por qué*: WebGL renderiza UI Toolkit con ligeras variaciones de sub-pixel respecto al Editor; el ancho explícito elimina la ambigüedad y previene la aparición de barras de scroll horizontales indeseadas.

3.  **Shader de Fondo Dinámico (Radial Gradient)**:
    -   *Acción*: Implementación de `AnimatedGradientSkybox.shader`.
    -   *Lógica*: Gradiente radial en espacio de pantalla (`ComputeScreenPos`) con pulsación sinusoidal.
    -   *Por qué*: Replical la estética "Premium" de la landing page web dentro del canvas 3D. Un fondo sólido se sentía vacío ("inexpressive"), mientras que el gradiente aporta profundidad y vida sin distraer.

4.  **Alineación Pixel-Perfect**:
    -   *Acción*: Ajuste del botón "Atrás" a `top: 88px`.
    -   *Cálculo*: `80px` (margin top) + `32px` (mitad de altura de barra 64px) = `112px` centro. Botón de 48px -> `112 - 24 = 88px`.
    -   *Por qué*: La percepción de calidad en UI depende de la alineación precisa y el ritmo visual consistente.

---

### Registro de Cambios (Febrero 20, 2026) — Fase 3: Technical Architecture Refactoring

1.  **Prevención de Fugas de Memoria (UI Toolkit)**:
    -   *Problema*: El "God Class" `UIManager` asignaba docenas de lambdas anónimas a los eventos visuales (`RegisterCallback<PointerDownEvent>`) en el método `InitializeUI`, pero carecía de una lógica de limpieza en `OnDisable()`. En UI Toolkit, las referencias fuertes de eventos del DOM causan memory leaks severos si la jerarquía sobrevive pero el script se deshabilita/recarga.
    -   *Solución Técnica*: Se implementó un patrón de "Lazy Evaluation Cleanup": una lista `_uiCleanupActions` que registra cada subscripción mediante cierres de variables (closures) y se encarga de ejecutar `UnregisterCallback` o `-=` masivamente. Se transformaron las lambdas críticas de `RegisterButtonInputBlockers` a Action Caches.
    -   *Por qué*: Cumplimiento estricto de las KPIs de optimización WebAssembly, garantizando que el Heap allocation se mantiene por debajo de 150MB incluso después de largos tiempos de sesión o cambios de escenas.

2.  **Desacoplamiento Estructural (Single Responsibility)**:
    -   *Problema*: `UIManager` funcionaba como un "God Class" acoplando lógica de negocio (cambio de shaders, iluminación global) con lógica de interfaz estricta (animaciones y hojas details).
    -   *Solución Técnica*: Se extrajeron los bloques lógicos de `#region Shader System` y `#region Environment Panel` hacia dos nuevas clases abstractas C# `UIAnalyzePanel` y `UIEnvironmentPanel`. El `UIManager` las inicializa pasándoles inyectores `VisualElement` y ellas gestionan internamente bindings y garbage collection.
    -   *Por qué*: Reducción masiva de métrica LLOC (Logical Lines of Code), dividiendo dependencias inter-módulo y preparando la base de código para una arquitectura robusta.

---

### Registro de Cambios (Febrero 23–24, 2026) — FASE 1: Arquitectura Limpia + UX Redesign

1.  **Refactoring de Arquitectura (C01–C05)**:
    -   *Problema*: Residuos de fases anteriores — función `ApplyDefaultRenderSettings` muerta, puente `GlobalInputBlocked` obsoleto, duplicación de estado entre `GameManager.currentState` y `AppStateMachine`.
    -   *Solución Técnica*: Eliminación de 5+ archivos/funciones de código muerto. Centralización de input blocking en `InputManager` con UI-awareness integrado en `OrbitCameraController` y `KeyboardShortcuts`. Estandarización de null-safety (`?.` y `??`).
    -   *Incidente*: La eliminación de `RegisterButtonInputBlockers` rompió todos los submenús (clicks propagaban al fondo 3D). Se revirtió y se mantuvo como el guard principal junto con `IsPointerOverUI()`.

2.  **Sistema de 3 Modos (Iteraciones 1–6)**:
    -   *Acción*: Reestructuración completa de la arquitectura UX en tres modos operativos: **Inspect** (selección/información), **Analyze** (shaders/cross-section/filtros), **Studio** (entorno/iluminación).
    -   *Implementación*: Expansión de `AppStateMachine` con nuevos estados. Creación de `UIModeController` para gestión de transiciones. Reestructuración de `MainLayout.uxml` con action bars específicos por modo.
    -   *Por qué*: La interfaz monolítica anterior (todos los controles visibles simultáneamente) violaba el principio de Progressive Disclosure. Los usuarios no necesitan herramientas de análisis mientras inspeccionan piezas.

3.  **Iteraciones UX 7–11 (Grid Minimalista)**:
    -   *Acción*: Implementación de sistema grid minimalista con icon-only buttons, 4-col card grid, sliders alineados.
    -   *Iter 8*: Clipping universal de cross-section con material shader.
    -   *Iter 10*: Navegación card-grid unificada con dual-plane cross-section.
    -   *Iter 11*: Resolución de 7 quejas UX (sizes, layout, gestures, filter behavior).
    -   *Por qué*: Cumplimiento de principios de diseño iOS/Material (44px touch targets, 8pt grid, consistent spacing).

---

### Registro de Cambios (Febrero 25–26, 2026) — FASE 2: Auditoría WCAG + Iconos Procedurales

1.  **Reestructuración de Modos (Iter 12–14)**:
    -   *Acción*: Consolidación final del sistema Inspect/Analyze/Studio. Bottom pill con border-radius verdadero (36px = mitad de 72px altura). Fix de isolate con `SetActive`. Scroll en Device menu.
    -   *Resultado*: La pill bar se comporta como una isla flotante con modos claramente diferenciados.

2.  **Auditoría WCAG 2.1 AA**:
    -   *Problema*: Múltiples violaciones de contraste WCAG (ratio < 4.5:1), touch targets < 44px, font sizes inconsistentes.
    -   *Solución*: Bottom pill ampliada a 112px con iconos de 56px. Corrección de contraste en textos sobre fondos oscuros. Verificación de todos los touch targets interactivos.
    -   *Resultado*: Compliance mínimo WCAG 2.1 AA en todos los controles interactivos.

3.  **Sistema de Iconos Procedurales (8 iconos)**:
    -   *Acción*: Reemplazo de imágenes raster por iconos procedurales generados vía C# + `VisualElement` en runtime.
    -   *Iconos*: Home, Info, Filter, Studio, Pins, Inspect, Analyze, Explode, Measure, Cut.
    -   *Física*: Cada icono implementa microinteracciones con `SpringFloat` (constante de resorte + damping), simulando inercia y rebote natural.
    -   *Por qué*: Los iconos procedurales son infinitely scalable, animables, y pesan 0 bytes adicionales en el build WebGL. Además, las microinteracciones con física real aportan una capa de sensorialidad que mejora el feedback háptico visual.
    -   *Reto*: Routing de `PointerEvent` — UI Toolkit consume los eventos en el `Button` antes de que lleguen al contenido hijo. Solución: `TrickleDown` phase en `ProceduralIconBase`.

4.  **Documentación Técnica**:
    -   *Acción*: Creación de `01_Arquitectura_del_Sistema.md`, `02_Manual_Tecnico.md` y documentación de arquitectura matemática/física de iconos procedurales.
    -   *Corrección*: Errores factuales en 4 reportes de auditoría (Architecture, Performance, UX/UI, Academic Alignment) + plan de remediación.

---

### Registro de Cambios (Febrero 27, 2026) — FASE 2: Slider UX + WebGL Config

1.  **Debugging de Slider Hitbox (11 iteraciones)**:
    -   *Problema*: El dragger del slider era visualmente correcto pero no respondía a clicks en su totalidad. Solo la esquina inferior-derecha era clickeable.
    -   *Root Cause*: UI Toolkit usa **Yoga layout** (no CSS). Con `position:relative` (default), `top`/`left` son SOLO visuales — el hit-test rect permanece en el origen del flex layout. Unity's C#-set `style.left` movía el rendering pero no el hit-rect.
    -   *Iteraciones*: `position:absolute` → scale tricks → physical width/height → creación de `SliderHitboxDebugger.cs` (worldBound logging) → descubrimiento de namespace collision (`WebGL.Debug` vs `UnityEngine.Debug`) → revert final a styles originales.
    -   *Solución final*: `picking-mode: Position` en slider labels para prevenir que clicks en texto propaguen al fondo 3D. Los draggers se dejaron con el styling original que Unity's BaseSlider posiciona correctamente.
    -   *Lección*: En UI Toolkit, NUNCA usar `top`/`left`/`margin-top` para posicionar hit-test targets — usar `align-items`/`justify-content` del contenedor padre.

2.  **Font Rendering (5 iteraciones)**:
    -   *Problema*: Textos invisibles/cortados en WebGL build.
    -   *Intentos*: PanelSettings + SDF Font Assets generados automáticamente → texto correcto en Editor pero distorsionado en build.
    -   *Solución*: Revert a legacy font rendering con `-unity-font-definition: initial`.
    -   *Por qué*: Unity 6 SDF pipeline tiene bugs conocidos en WebGL cuando se combinan múltiples font families (Inter + Space Grotesk).

3.  **WebGL Optimization Config**:
    -   *Acción*: Conversión de colores USS de Gamma a Linear (WebGL usa Linear color space). Configuración de WebGL build settings óptimos.

---

### Registro de Cambios (Marzo 2, 2026) — FASE 3: Implementación Completa

> Día más productivo del proyecto: **48 commits** completando FASE 1 (C01–C05), FASE 2 (H01–H11) y FASE 3 (F3-00 a F3-13).

1.  **FASE 1 — Code Quality (C01–C05)**:
    -   **C01**: Eliminación de sistema `VisualMode` duplicado en `ExplodedViewManager` (redundante con `ViewModeManager`).
    -   **C02**: Consolidación de triple publicación de eventos de estado → canal único `StateChangedEvent` vía `EventBus`.
    -   **C03**: Bottom Sheet reorganizado con 3 secciones Foldout colapsables (General, Technical, Assembly).
    -   **C04**: Enforcement de 44px minimum touch targets en toda la UI (WCAG 2.1 / Apple HIG).
    -   **C05**: Extracción de Loading/Error UI de código C# procedural a UXML+USS declarativo.

2.  **FASE 2 — Architecture Hardening (H01–H11)**:
    -   **H01**: Implementación de `ServiceLocator` + migración de 4 singletons (`AudioManager`, `GameManager`, `SelectionManager`, `DroneStateController`).
    -   **H02**: Extracción de `BaseModeHandler` desde `UIModeController` — tres handlers concretos: `InspectHandler`, `AnalyzeHandler`, `StudioHandler`.
    -   **H03**: `EventBus` mejorado con leak detection (cuenta de suscriptores por canal) + zombie purge automático (eliminación de delegates a objetos destruidos).
    -   **H04**: Eliminación de 10 archivos de código muerto (~2,000 líneas): `ViewModeToolbar`, `EngineerToolbar`, `ScreenshotManager`, `PerformanceMonitor`, `TooltipSystem`, `RuntimeConsole`, `SceneTransitionManager`, `KeyboardShortcuts`, `ObjectPooler`, `WebGLOptimizer`.
    -   **H06+H07**: Optimización de `ProjectSettings/ProjectSettings.asset` — `StripUnusedMeshComponents=1`, IL2CPP código de compilación `Master`, `mipStripping=1`, `webGLNameFilesAsHashes=1`.
    -   **H08**: Conversión de 6 `Update()` poll-based a coroutines/`InvokeRepeating`: `CrossSectionManager`, `LoadingController`, `ExplodedViewManager`, `MeasurementTool`, `QualityManager`, `WebGLProfiler`.
    -   **H09**: `QualityManager` con resolución adaptativa real — URP `renderScale` vía reflection (porque `ScalableBufferManager` no es soportado en WebGL).
    -   **H10**: Optimización Fitts' Law — reducción de gap entre bottom pill y borde inferior (padding 16→0, margin 16→8, mode-btn margins 20→12px).
    -   **H11**: Overlay de onboarding para primera visita con HELP button en hero menu.
    -   *Por qué H01*: Los singletons (`Instance`) crean dependencias implícitas imposibles de testear. `ServiceLocator` permite inyección y mocking.
    -   *Por qué H02*: La lógica de los 3 modos estaba entremezclada en `UIModeController` con switches/if-chains. Strategy Pattern permite extensibilidad (agregar modo 4 sin tocar los existentes).
    -   *Por qué H03*: Memory leaks silenciosos en EventBus — delegates a MonoBehaviours destruidos permanecían activos, causando `MissingReferenceException` en runtime.

3.  **FASE 3 — Feature Tickets (F3-00 a F3-13)**:
    -   **F3-00**: Hotspot swap fix, slider dragger restyle, disable bg-click deselect.
    -   **F3-01**: Studio render mode — toggle behavior con card visibility.
    -   **F3-02**: Cycling de Environment presets con botones TIME y COLOR.
    -   **F3-03**: 6ª categoría de filtro PAYLOAD (completó grid 3×2).
    -   **F3-04**: Info → FAB global (Floating Action Button visible on selection).
    -   **F3-05**: Card MEASURE en modo Inspect + `ProceduralMeasureIcon`.
    -   **F3-06**: Reducción de `webGLMaximumMemorySize` 512→256 MB (PERF-M02).
    -   **F3-07**: Grid 4pt + normalización de type ramp.
    -   **F3-08**: Explosion slider con label de porcentaje dinámico.
    -   **F3-09**: Tecla Escape global con cascading dismiss (sheet → submenu → mode).
    -   **F3-10**: Camera magic numbers → constantes nombradas.
    -   **F3-11**: Double-click unificado — detección movida de múltiples scripts a `SelectionManager`.
    -   **F3-12**: `DronePartData` limpieza — eliminación de property wrappers redundantes.
    -   **F3-13**: WebGL template custom basado en build existente.

4.  **Bug Fixes Críticos**:
    -   `ExplodedViewManager`: Race condition en `Start()` — partes se posicionaban en (0,0,0) porque `originalPositions` se capturaba antes de que Unity completara la inicialización del transform. Solución: postponer captura 1 frame con `yield return null`.
    -   `QualityManager`: Dependencia directa a ensamblado URP causaba errores en builds sin URP. Solución: acceso via `System.Reflection` al renderScale.

---

### Registro de Cambios (Marzo 3, 2026) — Fixes Finales + Plan de Trabajo

1.  **FAB Info Visibility**:
    -   *Problema*: El Floating Action Button (info) desaparecía cuando el Bottom Sheet estaba abierto.
    -   *Solución*: Lógica corregida para mantener FAB visible mientras el sheet está activo.

2.  **Explode Slider Restore**:
    -   *Problema*: Al reactivar el toggle de explosion, el slider siempre reiniciaba a 0% en lugar de restaurar el último valor.
    -   *Solución*: Persistencia del último valor aplicado (o 50% por defecto si primera vez).

3.  **Plan de Trabajo Final**:
    -   *Acción*: Auditoría completa de ~30 documentos de desarrollo. Identificación de 12 incongruencias entre documentación y código real. Creación de `PLAN_TRABAJO_FINAL.md` con plan detallado de 4 días (5 bloques de trabajo).

---

*Registro mantenido por el Equipo de Desarrollo.*
*Última actualización: 3 Marzo 2026*
