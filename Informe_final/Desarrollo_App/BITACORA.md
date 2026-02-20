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

---

*Registro mantenido por el Equipo de Desarrollo.*
*Última actualización: 20 Febrero 2026*
