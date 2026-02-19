# Bitácora de Desarrollo — Registro de Ejecución

> Documento oficial de seguimiento del proyecto "WebGL Drone Viewer".
> Alineado con el Cronograma Oficial (Fases 1-7).
> Nota: Las fechas corresponden al año fiscal 2025 según planificación, con extensión post-tesis en 2026.

---

## Fase 1: Investigación y Planificación (Julio - Agosto 2025)

### Estado: COMPLETADO
- **Revisión de Literatura**: Análisis de 40+ referencias sobre visualización técnica y WebGL.
- **Marco Teórico**: Definición de estándares (ISO 10110 para óptica, Apple HIG para UI).
- **Propuesta**: Aprobación del alcance (Visualizador Interactivo de Drones de Alto Rendimiento).

---

## Fase 2: Pipeline 3D (Septiembre - Octubre 2025)

### Estado: COMPLETADO
- **Modelado**: Creación del drone "Spectre X" (High-poly en Blender).
- **Optimización**: Retopología y baking de texturas (4K PBR) para web.
- **Exportación**: Validación de FBX y configuración de prefabs en Unity.

---

## Fase 3: Desarrollo Unity - Core (Octubre - Noviembre 2025)

### Estado: COMPLETADO
- **Arquitectura**: Implementación de `GameManager`, `EventBus`, y Singletons.
- **Interacción**: Sistema de cámara orbital (`OrbitCameraController`) y selección de objetos (`SelectionManager`).
- **UI Base**: Configuración inicial de UI Toolkit (USS/UXML).

---

## Fase 4: Desarrollo Features (Noviembre - Diciembre 2025)

### Estado: COMPLETADO
- **Despiece**: Algoritmo de expansión de partes (`ExplodedViewManager`).
- **Shaders**: Desarrollo de 7 modos de visualización (Rayos X, Blueprint, Térmico, etc.).
- **Datos**: Base de datos de componentes (`DronePartData`).

---

## Fase 5: Herramientas de Ingeniería (Diciembre 2025)

### Estado: COMPLETADO
- **Guía de Ensamblaje**: Sistema paso a paso.
- **Herramientas de Medición**: Cálculo de distancias en 3D.
- **Lista de Materiales (BOM)**: Exportación de datos.
- **Anotaciones**: Etiquetas 3D flotantes.

---

## Fase 6: Testing y Validación (Diciembre 2025)

### Estado: PENDIENTE
- [ ] Pruebas Unitarias (Coverage > 80%).
- [ ] Pruebas de Usabilidad (Cuestionario SUS).
- [ ] Análisis de Carga Cognitiva (NASA-TLX).
- [ ] Optimización de Frames (Profiler Report).

---

## Fase 7: Documentación (Diciembre 2025)

### Estado: EN PROGRESO
- [x] Informe Final (Bitácora).
- [ ] Manual Técnico (Arquitectura y API).
- [ ] Manual de Usuario (Guía de uso).
- [ ] Presentación Final (Diapositivas).

---

## Fase 8: Extensión Post-Entrega / Validación Final (Febrero 2026 - Actual)

> **Nota**: Fase adicional para elevar la calidad visual y UX al estándar "Awwwards".

### Hitos Realizados (Sesión 18/02/2026)
1.  **Rediseño Visual (v2.1)**:
    -   Unificación de paleta con Web Landing Page (`#050505` bg, acento blanco).
    -   Implementación de **Radial Gradient Skybox** shader (fondo dinámico).
    -   Tipografía `Space Grotesk` (Títulos) + `Inter` (Cuerpo).
2.  **Refinamiento UX**:
    -   Hero Menu tipo "App" con submenús integrados.
    -   Corrección de clipping en menús desplegables (Select Device).
    -   Alineación pixel-perfect de elementos de navegación.
3.  **Correcciones Técnicas**:
    -   `ScrollView` layout fix (Zero margins/padding).
    -   Elimilación de advertencias USS (`picking-mode`).

### Pendientes Inmediatos
- [ ] Rediseño Barra Inferior (Unified Pill Container).
- [ ] Implementación de Iconos Minimalistas (Freepik).
- [ ] Grid Submenus para selección de partes.

---

*Última actualización: 18 de Febrero, 2026*
