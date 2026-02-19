# Bitácora de Desarrollo — WebGL Drone Visualization

> Registro oficial del proceso de desarrollo, desafíos técnicos y soluciones implementadas.
> Fuente: Repositorio Git y documentación interna.

---

## 1. Definición del Proyecto (Noviembre 2025)

### Objetivo
Desarrollar una herramienta educativa interactiva basada en WebGL para la visualización y análisis técnico de componentes de drones, superando las limitaciones de los manuales estáticos tradicionales.

### Alcance Inicial
- Modelo 3D de alta fidelidad.
- Interacción orbital y zoom.
- Despiece (Exploded View).
- Modos de visualización técnica (Rayos X, Blueprint, Térmico).

### Decisiones de Diseño
- **Estilo**: Minimalismo "Premium" inspirado en Apple HIG y interfaces Sci-Fi (Glassmorphism).
- **Stack Tecnológico**: Unity 6 (URP), WebGL 2.0.

---

## 2. Fase de Prototipado y Core (Diciembre 2025)

### Hitos
- **Implementación de Managers**: Desarrollo de una arquitectura modular basada en Singletons (`GameManager`, `AudioManager`, etc.).
- **Sistema de Shaders**: Creación de 7 shaders personalizados (HLSL/ShaderGraph) para los modos de visualización.
  - *Reto*: Rendimiento en WebGL con transparencias múltiples.
  - *Solución*: Optimización de `ClippableLit` y uso de `PropertyBlocks`.
- **Integración Web**: Desarrollo de landing page con GSAP para presentación del proyecto.

---

## 3. Fase de Refinamiento UI/UX (Enero - Febrero 2026)

### Desafíos de Interacción
- La UI inicial presentaba conflictos de "raycast blocking" (clicks atravesaban la UI hacia el modelo 3D).
- **Solución**: Implementación de `PointerEnter/Exit` events en UI Toolkit para bloquear el input 3D.

### Mobile & High-DPI
- Adaptación de la interfaz para pantallas de alta densidad (Retina/Mobile).
- **Ajuste**: Aumento de zonas táctiles (min 48px -> 100px en mobile) y escalado dinámico de fuentes.

---

## 4. Fase de Pulido Visual y "Awwwards" Quality (Febrero 18, 2026) -> (Sesión Actual)

### Rediseño Hero Menu
- Transformación del menú principal hacia un estilo "App-like" con navegación fluida.
- Integración de selectores de dispositivo y menús de información como *Submenus* de pantalla completa.

### Unificación Visual (v2.1)
- **Problemática**: Inconsistencia entre la Landing Page (Web) y la App WebGL.
- **Acción**:
  - Unificación de paleta de colores (`#050505` background, blanco puro como acento).
  - Reemplazo de tipografías por `Inter` (UI) y `Space Grotesk` (Títulos).
  - Implementación de **Animated Gradient Skybox** shader para replicar el fondo dinámico de la web en el entorno 3D.

### Optimización de UI Toolkit
- Resolución de advertencias de consola (`picking-mode`, referencias nulas).
- Mejora de `ScrollView` clipping y alineación de textos en el Panel de Información.

---

## Próximos Pasos (Hoja de Ruta)

1. **Rediseño Bottom Bar**: Unificación en contenedor "Pill" flotante.
2. **Iconografía**: Implementación de iconos minimalistas (Freepik/SVG).
3. **Grid Submenus**: Sistema de navegación por categorías más visual.
