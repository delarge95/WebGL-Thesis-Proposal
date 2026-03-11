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
    - _Acción_: Análisis de 40+ referencias (papers, tesis, herramientas existentes).
    - _Justificación_: Identificar brechas en las herramientas actuales (baja interactividad, requisitos de hardware altos).
    - _Conclusión_: Se opta por **WebGL 2.0** para garantizar acceso universal sin instalación.
2.  **Selección del Stack Tecnológico**:
    - _Decisión_: Unity 6 (URP) vs Three.js.
    - _Razón_: Unity ofrece un pipeline de assets más robusto y herramientas de editor visual para configuraciones complejas (ECS/Prefabs), ideal para un prototipo escalable.
    - _Decisión_: Universal Render Pipeline (URP).
    - _Razón_: Optimización nativa para plataformas móviles y WebGL, permitiendo shaders personalizados ligeros.

---

## Fase 2: Pipeline de Producción 3D (Septiembre - Octubre 2025)

### Objetivo

Crear un gemelo digital del drone "Spectre X" optimizado para renderizado web en tiempo real.

### Acciones Realizadas

1.  **Modelado Hard-Surface (Blender)**:
    - _Acción_: Modelado high-poly de la estructura del drone.
    - _Detalle técnico_: Uso de modificadores booleanos no destructivos para iteración rápida.
2.  **Optimización y Retopología**:
    - _Acción_: Reducción de polígonos (Target: <100k tris).
    - _Justificación_: WebGL tiene límites estrictos de memoria y draw calls.
3.  **Baking de Texturas**:
    - _Acción_: Generación de mapas de Normales, AO y Roughness en texturas atlas 4K.
    - _Razón_: Simular detalle geométrico sin costo de procesamiento de vértices (PBR Workflow).

---

## Fase 3: Arquitectura de Software - Unity Core (Octubre - Noviembre 2025)

### Objetivo

Establecer una base sólida de código escalable y modular.

### Implementación Técnica

1.  **Patrón Singleton & Managers**:
    - _Implementación_: `GameManager`, `AudioManager`, `SelectionManager`.
    - _Por qué_: Centralizar el control de sistemas globales para evitar dependencias cruzadas (Spaghetti code) y facilitar el acceso desde la UI.
2.  **Sistema de Eventos (EventBus)**:
    - _Implementación_: `Action<T>` events para comunicación desacoplada (`OnPartSelected`, `OnViewModeChanged`).
    - _Por qué_: Permite que sistemas dispares (UI, Audio, Renderizado) reaccionen a cambios de estado sin conocerse entre sí, mejorando la mantenibilidad.
3.  **Controlador de Cámara (`OrbitCameraController`)**:
    - _Detalle_: Navegación polar con clamping de ángulos y suavizado (Damping).
    - _Razón UX_: Evitar que el usuario pierda de vista el modelo o la orientación (Gimbal lock).

---

## Fase 4: Desarrollo de Features Avanzadas (Noviembre - Diciembre 2025)

### Objetivo

Implementar las funcionalidades educativas clave del visualizador.

### Funcionalidades

1.  **Vista Explosionada (Exploded View)**:
    - _Lógica_: Desplazamiento vectorial de partes basado en su centroide respecto al pivote del drone.
    - _Implementación_: `ExplodedViewManager` que interpola posiciones locales usando `Mathf.Lerp`.
    - _Por qué_: Permite visualizar componentes internos sin ocultar el contexto estructural.
2.  **Sistema de Shaders de Visualización**:
    - _Desarrollo_: 7 shaders custom HLSL.
      - **Rayos X**: Efecto Fresnel invertido + ZTest Always para ver a través de muros.
      - **Blueprint**: Detección de bordes (Sobel) en espacio de pantalla + Grid procedural.
      - **Térmico**: Mapeo de gradiente de color basado en input normalizado.
    - _Reto Técnico_: Manejo de transparencias en WebGL (Depth sorting issues).
    - _Solución_: Render queues personalizados y multipass shaders.

---

## Fase 5: Herramientas de Ingeniería y Datos (Diciembre 2025)

### Objetivo

Aportar valor técnico y educativo real más allá de la visualización estética.

### Implementación

1.  **Base de Datos (`DronePartData`)**:
    - _Estructura_: ScriptableObjects conteniendo metadatos (Peso, Material, Consumo, Torque).
    - _Por qué_: Separar datos de lógica (Data-Driven Design). Permite editar valores en el Editor sin recompensar código.
2.  **Herramientas de Medición**:
    - _Acción_: Raycasting de punto a punto para calcular distancias Euclidianas en espacio de mundo.
    - _Uso_: Verificar compatibilidad de piezas o dimensiones de ensamblaje.

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

### Registro de Cambios (Marzo 11, 2026) — Ajustes UX Fase 1 y Fase 2

1.  **Persistencia de submenús Analyze ante selección 3D**:
    - _Problema_: Los submenús `Cut`, `Filter` y `Explode` se cerraban al seleccionar piezas o al hacer click en el background.
    - _Causa raíz_: `UIManager.OnPartSelected(...)` ejecutaba `CloseAllMenus()` como efecto colateral del sistema de selección 3D.
    - _Solución Técnica_: Se eliminó el cierre automático desde `UIManager`, desacoplando la navegación de Analyze de los eventos `PartSelectedEvent`.
    - _Resultado_: Los submenús permanecen abiertos durante la interacción normal con el modelo y solo cambian cuando el usuario navega explícitamente entre modos o botones.

2.  **Desacoplamiento de Explode respecto al AppState global**:
    - _Problema_: La vista explosionada se desactivaba al cambiar entre Analyze y Studio o al reabrir Analyze, aunque el usuario no hubiera apagado `Explode`.
    - _Causa raíz_: `ExplodedViewManager` y `UIModeController` apagaban el estado explosivo al salir de `AppState.ExplodedView`.
    - _Solución Técnica_: Se removió el reseteo automático de `SetExplosionFactor(0f)` en `ExplodedViewManager.OnStateChanged(...)` y la limpieza forzada de `SetExplodeState(false)` en `UIModeController.SyncWithAppState(...)`.
    - _Resultado_: Mientras `Explode` esté activo, el botón sigue marcado y el slider permanece visible hasta que el usuario lo apague explícitamente con el botón o llevando el slider a cero.

3.  **Consolidación de diagnóstico y plan de remediación**:
    - _Acción_: Se documentó el análisis técnico completo de problemas de submenús, explode, blueprint/cut, info panel y cámara.
    - _Archivo_: `desarrollo/docs/investigacion/14_analisis_problemas_app_2026-03-10.md`.
    - _Por qué_: Dejar una base verificable para implementar las siguientes fases sin depender de memoria de sesión.

4.  **Restauración visual del cierre del info panel**:
    - _Problema_: La X del panel de información era funcional pero visualmente inestable al depender de un glyph de texto.
    - _Solución Técnica_: Se creó `ProceduralCloseIcon` dentro del sistema de iconos procedurales y se reemplazó el texto del botón por dicho icono en `MainLayout.uxml`.
    - _Refinamiento UX_: El botón de cierre se escaló a 64x64, con icono central mayor y offsets simétricos de 24px desde el borde superior y derecho del panel para alinearlo con el lenguaje visual de `Home` y `Reset`.
    - _Resultado_: El cierre del panel ahora usa el mismo sistema visual del resto de controles premium y mantiene una alineación más limpia en la esquina.

5.  **Endurecimiento de reglas de cierre del info panel**:
    - _Problema_: El panel todavía podía cerrarse por arrastre del handle y por comportamiento toggle del acceso global de info, dos caminos no alineados con la UX definida.
    - _Solución Técnica_: Se eliminó el bloque `drag-to-dismiss` de `UIDetailsSheet.BindInteractions()` y se sustituyó el toggle de `InfoBarPeek` y `ToolInfoBtn` por apertura explícita mediante `ShowInfo()`.
    - _Resultado_: El panel conserva sólo los cierres permitidos por esta fase: botón `X`, doble click sobre el fondo y cambio hacia otro ítem/modo de la barra inferior.

### Registro de Cambios (Febrero 18, 2026)

1.  **Rediseño de Interfaz (UI Toolkit)**:
    - _Acción_: Migración de estilos básicos a un "Design System" unificado (`DESIGN_TOKENS.md`).
    - _Detalle_: Uso de variables CSS (USS) para colores, espaciados y tipografía (`Inter` + `Space Grotesk`).
    - _Por qué_: Garantizar consistencia visual y facilitar cambios globales (e.g., Modo Oscuro/Claro).
2.  **Corrección de Clipping en Menús**:
    - _Problema_: El contenedor de scroll del menú de dispositivos (`Select Device`) cortaba el contenido derecho.
    - _Causa_: El ancho del `ScrollView` y su `content-container` no coincidían con el ancho de los botones hijos por defecto.
    - _Solución_: Se forzó un ancho explícito de **396.5194px** y se eliminaron paddings internos `padding: 0`.
    - _Por qué_: WebGL renderiza UI Toolkit con ligeras variaciones de sub-pixel respecto al Editor; el ancho explícito elimina la ambigüedad y previene la aparición de barras de scroll horizontales indeseadas.

3.  **Shader de Fondo Dinámico (Radial Gradient)**:
    - _Acción_: Implementación de `AnimatedGradientSkybox.shader`.
    - _Lógica_: Gradiente radial en espacio de pantalla (`ComputeScreenPos`) con pulsación sinusoidal.
    - _Por qué_: Replical la estética "Premium" de la landing page web dentro del canvas 3D. Un fondo sólido se sentía vacío ("inexpressive"), mientras que el gradiente aporta profundidad y vida sin distraer.

4.  **Alineación Pixel-Perfect**:
    - _Acción_: Ajuste del botón "Atrás" a `top: 88px`.
    - _Cálculo_: `80px` (margin top) + `32px` (mitad de altura de barra 64px) = `112px` centro. Botón de 48px -> `112 - 24 = 88px`.
    - _Por qué_: La percepción de calidad en UI depende de la alineación precisa y el ritmo visual consistente.

---

### Registro de Cambios (Febrero 20, 2026) — Fase 3: Technical Architecture Refactoring

1.  **Prevención de Fugas de Memoria (UI Toolkit)**:
    - _Problema_: El "God Class" `UIManager` asignaba docenas de lambdas anónimas a los eventos visuales (`RegisterCallback<PointerDownEvent>`) en el método `InitializeUI`, pero carecía de una lógica de limpieza en `OnDisable()`. En UI Toolkit, las referencias fuertes de eventos del DOM causan memory leaks severos si la jerarquía sobrevive pero el script se deshabilita/recarga.
    - _Solución Técnica_: Se implementó un patrón de "Lazy Evaluation Cleanup": una lista `_uiCleanupActions` que registra cada subscripción mediante cierres de variables (closures) y se encarga de ejecutar `UnregisterCallback` o `-=` masivamente. Se transformaron las lambdas críticas de `RegisterButtonInputBlockers` a Action Caches.
    - _Por qué_: Cumplimiento estricto de las KPIs de optimización WebAssembly, garantizando que el Heap allocation se mantiene por debajo de 150MB incluso después de largos tiempos de sesión o cambios de escenas.

2.  **Desacoplamiento Estructural (Single Responsibility)**:
    - _Problema_: `UIManager` funcionaba como un "God Class" acoplando lógica de negocio (cambio de shaders, iluminación global) con lógica de interfaz estricta (animaciones y hojas details).
    - _Solución Técnica_: Se extrajeron los bloques lógicos de `#region Shader System` y `#region Environment Panel` hacia dos nuevas clases abstractas C# `UIAnalyzePanel` y `UIEnvironmentPanel`. El `UIManager` las inicializa pasándoles inyectores `VisualElement` y ellas gestionan internamente bindings y garbage collection.
    - _Por qué_: Reducción masiva de métrica LLOC (Logical Lines of Code), dividiendo dependencias inter-módulo y preparando la base de código para una arquitectura robusta.

---

### Registro de Cambios (Febrero 23–24, 2026) — FASE 1: Arquitectura Limpia + UX Redesign

1.  **Refactoring de Arquitectura (C01–C05)**:
    - _Problema_: Residuos de fases anteriores — función `ApplyDefaultRenderSettings` muerta, puente `GlobalInputBlocked` obsoleto, duplicación de estado entre `GameManager.currentState` y `AppStateMachine`.
    - _Solución Técnica_: Eliminación de 5+ archivos/funciones de código muerto. Centralización de input blocking en `InputManager` con UI-awareness integrado en `OrbitCameraController` y `KeyboardShortcuts`. Estandarización de null-safety (`?.` y `??`).
    - _Incidente_: La eliminación de `RegisterButtonInputBlockers` rompió todos los submenús (clicks propagaban al fondo 3D). Se revirtió y se mantuvo como el guard principal junto con `IsPointerOverUI()`.

2.  **Sistema de 3 Modos (Iteraciones 1–6)**:
    - _Acción_: Reestructuración completa de la arquitectura UX en tres modos operativos: **Inspect** (selección/información), **Analyze** (shaders/cross-section/filtros), **Studio** (entorno/iluminación).
    - _Implementación_: Expansión de `AppStateMachine` con nuevos estados. Creación de `UIModeController` para gestión de transiciones. Reestructuración de `MainLayout.uxml` con action bars específicos por modo.
    - _Por qué_: La interfaz monolítica anterior (todos los controles visibles simultáneamente) violaba el principio de Progressive Disclosure. Los usuarios no necesitan herramientas de análisis mientras inspeccionan piezas.

3.  **Iteraciones UX 7–11 (Grid Minimalista)**:
    - _Acción_: Implementación de sistema grid minimalista con icon-only buttons, 4-col card grid, sliders alineados.
    - _Iter 8_: Clipping universal de cross-section con material shader.
    - _Iter 10_: Navegación card-grid unificada con dual-plane cross-section.
    - _Iter 11_: Resolución de 7 quejas UX (sizes, layout, gestures, filter behavior).
    - _Por qué_: Cumplimiento de principios de diseño iOS/Material (44px touch targets, 8pt grid, consistent spacing).

---

### Registro de Cambios (Febrero 25–26, 2026) — FASE 2: Auditoría WCAG + Iconos Procedurales

1.  **Reestructuración de Modos (Iter 12–14)**:
    - _Acción_: Consolidación final del sistema Inspect/Analyze/Studio. Bottom pill con border-radius verdadero (36px = mitad de 72px altura). Fix de isolate con `SetActive`. Scroll en Device menu.
    - _Resultado_: La pill bar se comporta como una isla flotante con modos claramente diferenciados.

2.  **Auditoría WCAG 2.1 AA**:
    - _Problema_: Múltiples violaciones de contraste WCAG (ratio < 4.5:1), touch targets < 44px, font sizes inconsistentes.
    - _Solución_: Bottom pill ampliada a 112px con iconos de 56px. Corrección de contraste en textos sobre fondos oscuros. Verificación de todos los touch targets interactivos.
    - _Resultado_: Compliance mínimo WCAG 2.1 AA en todos los controles interactivos.

3.  **Sistema de Iconos Procedurales (8 iconos)**:
    - _Acción_: Reemplazo de imágenes raster por iconos procedurales generados vía C# + `VisualElement` en runtime.
    - _Iconos_: Home, Info, Filter, Studio, Pins, Inspect, Analyze, Explode, Measure, Cut.
    - _Física_: Cada icono implementa microinteracciones con `SpringFloat` (constante de resorte + damping), simulando inercia y rebote natural.
    - _Por qué_: Los iconos procedurales son infinitely scalable, animables, y pesan 0 bytes adicionales en el build WebGL. Además, las microinteracciones con física real aportan una capa de sensorialidad que mejora el feedback háptico visual.
    - _Reto_: Routing de `PointerEvent` — UI Toolkit consume los eventos en el `Button` antes de que lleguen al contenido hijo. Solución: `TrickleDown` phase en `ProceduralIconBase`.

4.  **Documentación Técnica**:
    - _Acción_: Creación de `01_Arquitectura_del_Sistema.md`, `02_Manual_Tecnico.md` y documentación de arquitectura matemática/física de iconos procedurales.
    - _Corrección_: Errores factuales en 4 reportes de auditoría (Architecture, Performance, UX/UI, Academic Alignment) + plan de remediación.

---

### Registro de Cambios (Febrero 27, 2026) — FASE 2: Slider UX + WebGL Config

1.  **Debugging de Slider Hitbox (11 iteraciones)**:
    - _Problema_: El dragger del slider era visualmente correcto pero no respondía a clicks en su totalidad. Solo la esquina inferior-derecha era clickeable.
    - _Root Cause_: UI Toolkit usa **Yoga layout** (no CSS). Con `position:relative` (default), `top`/`left` son SOLO visuales — el hit-test rect permanece en el origen del flex layout. Unity's C#-set `style.left` movía el rendering pero no el hit-rect.
    - _Iteraciones_: `position:absolute` → scale tricks → physical width/height → creación de `SliderHitboxDebugger.cs` (worldBound logging) → descubrimiento de namespace collision (`WebGL.Debug` vs `UnityEngine.Debug`) → revert final a styles originales.
    - _Solución final_: `picking-mode: Position` en slider labels para prevenir que clicks en texto propaguen al fondo 3D. Los draggers se dejaron con el styling original que Unity's BaseSlider posiciona correctamente.
    - _Lección_: En UI Toolkit, NUNCA usar `top`/`left`/`margin-top` para posicionar hit-test targets — usar `align-items`/`justify-content` del contenedor padre.

2.  **Font Rendering (5 iteraciones)**:
    - _Problema_: Textos invisibles/cortados en WebGL build.
    - _Intentos_: PanelSettings + SDF Font Assets generados automáticamente → texto correcto en Editor pero distorsionado en build.
    - _Solución_: Revert a legacy font rendering con `-unity-font-definition: initial`.
    - _Por qué_: Unity 6 SDF pipeline tiene bugs conocidos en WebGL cuando se combinan múltiples font families (Inter + Space Grotesk).

3.  **WebGL Optimization Config**:
    - _Acción_: Conversión de colores USS de Gamma a Linear (WebGL usa Linear color space). Configuración de WebGL build settings óptimos.

---

### Registro de Cambios (Marzo 2, 2026) — FASE 3: Implementación Completa

> Día más productivo del proyecto: **48 commits** completando FASE 1 (C01–C05), FASE 2 (H01–H11) y FASE 3 (F3-00 a F3-13).

1.  **FASE 1 — Code Quality (C01–C05)**:
    - **C01**: Eliminación de sistema `VisualMode` duplicado en `ExplodedViewManager` (redundante con `ViewModeManager`).
    - **C02**: Consolidación de triple publicación de eventos de estado → canal único `StateChangedEvent` vía `EventBus`.
    - **C03**: Bottom Sheet reorganizado con 3 secciones Foldout colapsables (General, Technical, Assembly).
    - **C04**: Enforcement de 44px minimum touch targets en toda la UI (WCAG 2.1 / Apple HIG).
    - **C05**: Extracción de Loading/Error UI de código C# procedural a UXML+USS declarativo.

2.  **FASE 2 — Architecture Hardening (H01–H11)**:
    - **H01**: Implementación de `ServiceLocator` + migración de 4 singletons (`AudioManager`, `GameManager`, `SelectionManager`, `DroneStateController`).
    - **H02**: Extracción de `BaseModeHandler` desde `UIModeController` — tres handlers concretos: `InspectHandler`, `AnalyzeHandler`, `StudioHandler`.
    - **H03**: `EventBus` mejorado con leak detection (cuenta de suscriptores por canal) + zombie purge automático (eliminación de delegates a objetos destruidos).
    - **H04**: Eliminación de 10 archivos de código muerto (~2,000 líneas): `ViewModeToolbar`, `EngineerToolbar`, `ScreenshotManager`, `PerformanceMonitor`, `TooltipSystem`, `RuntimeConsole`, `SceneTransitionManager`, `KeyboardShortcuts`, `ObjectPooler`, `WebGLOptimizer`.
    - **H06+H07**: Optimización de `ProjectSettings/ProjectSettings.asset` — `StripUnusedMeshComponents=1`, IL2CPP código de compilación `Master`, `mipStripping=1`, `webGLNameFilesAsHashes=1`.
    - **H08**: Conversión de 6 `Update()` poll-based a coroutines/`InvokeRepeating`: `CrossSectionManager`, `LoadingController`, `ExplodedViewManager`, `MeasurementTool`, `QualityManager`, `WebGLProfiler`.
    - **H09**: `QualityManager` con resolución adaptativa real — URP `renderScale` vía reflection (porque `ScalableBufferManager` no es soportado en WebGL).
    - **H10**: Optimización Fitts' Law — reducción de gap entre bottom pill y borde inferior (padding 16→0, margin 16→8, mode-btn margins 20→12px).
    - **H11**: Overlay de onboarding para primera visita con HELP button en hero menu.
    - _Por qué H01_: Los singletons (`Instance`) crean dependencias implícitas imposibles de testear. `ServiceLocator` permite inyección y mocking.
    - _Por qué H02_: La lógica de los 3 modos estaba entremezclada en `UIModeController` con switches/if-chains. Strategy Pattern permite extensibilidad (agregar modo 4 sin tocar los existentes).
    - _Por qué H03_: Memory leaks silenciosos en EventBus — delegates a MonoBehaviours destruidos permanecían activos, causando `MissingReferenceException` en runtime.

3.  **FASE 3 — Feature Tickets (F3-00 a F3-13)**:
    - **F3-00**: Hotspot swap fix, slider dragger restyle, disable bg-click deselect.
    - **F3-01**: Studio render mode — toggle behavior con card visibility.
    - **F3-02**: Cycling de Environment presets con botones TIME y COLOR.
    - **F3-03**: 6ª categoría de filtro PAYLOAD (completó grid 3×2).
    - **F3-04**: Info → FAB global (Floating Action Button visible on selection).
    - **F3-05**: Card MEASURE en modo Inspect + `ProceduralMeasureIcon`.
    - **F3-06**: Reducción de `webGLMaximumMemorySize` 512→256 MB (PERF-M02).
    - **F3-07**: Grid 4pt + normalización de type ramp.
    - **F3-08**: Explosion slider con label de porcentaje dinámico.
    - **F3-09**: Tecla Escape global con cascading dismiss (sheet → submenu → mode).
    - **F3-10**: Camera magic numbers → constantes nombradas.
    - **F3-11**: Double-click unificado — detección movida de múltiples scripts a `SelectionManager`.
    - **F3-12**: `DronePartData` limpieza — eliminación de property wrappers redundantes.
    - **F3-13**: WebGL template custom basado en build existente.

4.  **Bug Fixes Críticos**:
    - `ExplodedViewManager`: Race condition en `Start()` — partes se posicionaban en (0,0,0) porque `originalPositions` se capturaba antes de que Unity completara la inicialización del transform. Solución: postponer captura 1 frame con `yield return null`.
    - `QualityManager`: Dependencia directa a ensamblado URP causaba errores en builds sin URP. Solución: acceso via `System.Reflection` al renderScale.

---

### Registro de Cambios (Marzo 3, 2026) — Fixes Finales + Plan de Trabajo

1.  **FAB Info Visibility**:
    - _Problema_: El Floating Action Button (info) desaparecía cuando el Bottom Sheet estaba abierto.
    - _Solución_: Lógica corregida para mantener FAB visible mientras el sheet está activo.

2.  **Explode Slider Restore**:
    - _Problema_: Al reactivar el toggle de explosion, el slider siempre reiniciaba a 0% en lugar de restaurar el último valor.
    - _Solución_: Persistencia del último valor aplicado (o 50% por defecto si primera vez).

3.  **Plan de Trabajo Final**:
    - _Acción_: Auditoría completa de ~30 documentos de desarrollo. Identificación de 12 incongruencias entre documentación y código real. Creación de `PLAN_TRABAJO_FINAL.md` con plan detallado de 4 días (5 bloques de trabajo).

---

_Registro mantenido por el Equipo de Desarrollo._
_Última actualización: 3 Marzo 2026_
