# Plan de Investigación UX — Iteración 12

## Reordenamiento, Simplificación y Reducción de Carga Cognitiva

**Fecha:** Junio 2025  
**Objetivo:** Analizar la estructura actual de la UI, identificar redundancias y oportunidades de simplificación, y proponer un plan de cambios sin perder funcionalidades.

---

## 1. Inventario Actual de la Interfaz

### 1.1 Estructura de Modos y Opciones

| Modo        | Nivel 1 (Card Grid)         | Nivel 2 (Sub-Panel)                                            | Total opciones                           |
| ----------- | --------------------------- | -------------------------------------------------------------- | ---------------------------------------- |
| **TOOLS**   | INFO, EXPLODE, FILTER, PINS | Explode → Slider                                               | 4 cards + 1 slider + 5 categorías        |
| **ANALYZE** | SHADERS, CUT                | Shaders → 7 modos de render; Cut → 3 ejes + slider + inversión | 2 cards + 7 shaders + ~6 controles corte |
| **STUDIO**  | (Sin card grid)             | 4 presets + 2 sliders                                          | 4 presets + 2 sliders                    |

**Total de elementos interactivos principales:** ~31 botones/sliders distribuidos en 3 modos.

### 1.2 Gestos Disponibles

| Gesto                   | Acción                          |
| ----------------------- | ------------------------------- |
| Tap en pieza            | Seleccionar/deseleccionar pieza |
| Doble-tap en pieza      | Abrir hoja de información       |
| Swipe-up desde pill bar | Abrir hoja de información       |
| Pinch                   | Zoom in/out                     |
| Drag horizontal         | Rotar cámara                    |
| Drag vertical           | Orbitar cámara                  |

### 1.3 Hero Menu (Pre-Load)

| Opción        | Estado                                               |
| ------------- | ---------------------------------------------------- |
| EXPLORE       | Funcional — entra a la app                           |
| SELECT DEVICE | Solo 1 dispositivo activo (los otros deshabilitados) |
| ABOUT         | Panel informativo estático                           |
| EXIT          | Confirmación + salir                                 |

---

## 2. Análisis de Carga Cognitiva

### 2.1 Ley de Hick-Hyman

> El tiempo de decisión aumenta logarítmicamente con el número de opciones.

**Problemas detectados:**

- **SHADERS tiene 7 opciones** en un solo nivel: REALISTIC, X-RAY, BLUEPRINT, SOLID, WIRE, GHOST, THERMAL. La ley de Hick sugiere que 5-7 es el límite superior cómodo, pero para usuarios no técnicos, distinguir entre WIRE/GHOST/SOLID puede ser confuso.
- **TOOLS tiene 4 cards + 5 categorías de filtro**: El paso de card grid → sub-panel → 5 categorías crea 3 niveles de profundidad de decisión.
- **STUDIO con 4 presets** está dentro del rango óptimo.

### 2.2 Ley de Miller (7 ± 2)

La memoria de trabajo humana maneja ~7 items simultáneos.

- **Shaders (7 items):** En el límite. Un usuario casual no necesita o entiende la diferencia entre todos.
- **Card Grids (2-4 items):** Bien dimensionados.
- **Category Filter (5 items):** Aceptable.

### 2.3 Divulgación Progresiva (Progressive Disclosure)

La UI ya implementa divulgación progresiva con el sistema de dos niveles (card grid → sub-panel). Sin embargo:

- **STUDIO** no tiene card grid — expone todo de inmediato. Funciona porque tiene pocas opciones (4 presets + 2 sliders) y son visualmente diferenciables.
- **TOOLS → FILTER** ahora requiere: Tools → Filter card → ver 5 categorías. Esto es correcto.
- **ANALYZE → SHADERS** requiere: Analyze → Shaders card → ver 7 modos. Podría beneficiarse de agrupar los modos más técnicos.

---

## 3. Benchmarking: Viewers 3D Profesionales

### 3.1 Sketchfab (Viewer Web/Móvil)

- **Estructura:** Barra inferior con iconos simples (compartir, VR, zoom, AR)
- **Anotaciones:** Puntos clickeables directamente en el modelo (similar a nuestros PINS)
- **Materiales/Shaders:** NO expuestos — solo el autor los cambia
- **Conclusión:** Sketchfab prioriza la visualización pasiva; nuestro caso es más interactivo (educativo)

### 3.2 Autodesk Viewer (Fusion 360 Mobile)

- **Estructura:** Bottom bar con ~5 iconos (sección, medida, explotar, componentes, ajustes)
- **Sección:** Tap directo → slider aparece. Sin sub-menú intermedio. ✅ (ya implementado)
- **Explotar:** Tap → slider aparece. Sin categorías.
- **Componentes:** Tap → lista tipo árbol con checkboxes. Similar a nuestro FILTER.
- **Conclusión:** Autodesk usa acceso directo sin card grids intermedios.

### 3.3 GrabCAD / eDrawings Mobile

- **Estructura:** Toolbar lateral con iconos pequeños
- **Sección:** Toggle simple + slider
- **Explosión:** Slider directo
- **Vistas:** Preset views (front, back, iso) — no modos de shader artísticos
- **Conclusión:** Enfoque en ingeniería, menos opciones visuales.

### 3.4 Modelo.io / Vectary

- **Enfoque:** Previsualización de productos
- **Controles limitados:** Solo rotación + zoom + AR
- **Sin herramientas analíticas**

### 3.5 Patrones Comunes Identificados

1. **Acceso directo:** Los viewers profesionales evitan card grids intermedios para acciones frecuentes.
2. **Iconos + slider:** La combinación más usada para sección y explosión.
3. **Progresividad:** Las opciones avanzadas (shaders, medidas) se esconden detrás de un botón "more" o ajustes.
4. **Máximo 5-6 botones** en la barra principal — sin niveles jerárquicos para acciones primarias.

---

## 4. Propuestas de Simplificación

### 4.1 Propuesta A: Reducción de Niveles en Tools

**Estado actual:**  
TOOLS → Card Grid (4 opciones) → Sub-Panel

**Propuesta:**  
Dado que INFO y PINS son toggles inmediatos (no abren sub-panel), podrían estar directamente accesibles como iconos en la pill bar o como toggles permanentes, dejando el card grid solo para EXPLODE y FILTER.

| Opción  | Tipo     | Ubicación propuesta                    |
| ------- | -------- | -------------------------------------- |
| INFO    | Toggle   | Mover a la pill bar como icono directo |
| PINS    | Toggle   | Mover a la pill bar como icono directo |
| EXPLODE | Navigate | Card grid (con slider)                 |
| FILTER  | Navigate | Card grid (con categorías)             |

**Beneficio:** Reduce la carga de decisión de 4 a 2 en el card grid. INFO y PINS se acceden con 1 tap en vez de 2.

**Riesgo:** La pill bar se llenaría con más iconos (actualmente 3 modos + potencialmente 2 toggles = 5 botones). Aún dentro del rango aceptable según Hick.

### 4.2 Propuesta B: Agrupación de Shaders por Complejidad

**Estado actual:**  
7 shaders planos en una grilla: REALISTIC, X-RAY, BLUEPRINT, SOLID, WIRE, GHOST, THERMAL

**Propuesta:**  
Agrupar en 2 categorías:

| Grupo             | Shaders                     | Justificación                                 |
| ----------------- | --------------------------- | --------------------------------------------- |
| **Visualización** | REALISTIC, X-RAY, BLUEPRINT | Modos que un estudiante usaría frecuentemente |
| **Técnico**       | SOLID, WIRE, GHOST, THERMAL | Modos especializados para análisis avanzado   |

**Implementación sugerida:** Mostrar los 3 modos principales directamente + botón "MORE" que revela los 4 técnicos. O simplemente mantener los 7 pero con una línea divisoria visual entre los 2 grupos.

**Alternativa minimalista:** Reducir a 5 shaders eliminando GHOST (visualmente similar a X-RAY) y SOLID (poco usado educativamente). Esto queda dentro de 5 ± 2 de Miller.

### 4.3 Propuesta C: Studio — No Necesita Cambios

STUDIO ya tiene una estructura plana y simple:

- 4 presets (Studio, Night, Blueprint, Neutral) → OK, dentro de rango
- 2 sliders (Rotation, Intensity) → Controles directos, sin jerarquía

**Recomendación:** Mantener como está. Es el modo mejor diseñado en términos de carga cognitiva.

### 4.4 Propuesta D: Hero Menu — Simplificación

**Problemas:**

- SELECT DEVICE tiene solo 1 opción real (las otras están deshabilitadas)
- EXIT en una app WebGL es poco convencional (el usuario simplemente cierra la pestaña)
- ABOUT es estático y rara vez consultado

**Propuesta:**

1. **Eliminar EXIT** — innecesario en WebGL. El usuario cierra la pestaña.
2. **Ocultar SELECT DEVICE** hasta tener más de 1 dispositivo — mostrar un badge "Coming Soon" o removerlo.
3. **Simplificar a:** EXPLORE + ABOUT (2 botones). Opcionalmente un tercer botón directo a GITHUB.

**Beneficio:** Reduce de 4 a 2-3 opciones. El hero screen se vuelve una puerta de entrada rápida, no un menú de decisión.

### 4.5 Propuesta E: Gestos como Atajos (No Reemplazos)

Los gestos (doble-tap, swipe-up) deben complementar la UI de botones, no reemplazarla. Actualmente:

- **Doble-tap** → abre INFO (duplica la función del botón INFO)
- **Swipe-up** → abre INFO (otro duplicado)

Esto es correcto según Nielsen (múltiples caminos al mismo destino). No se recomienda eliminar ninguno.

**Propuesta adicional:** Agregar un onboarding sutil (tooltip o coach mark) la primera vez que el usuario toca una pieza: "Double-tap for details" o "Swipe up for info".

---

## 5. Análisis de Prioridad (Impacto vs. Esfuerzo)

| Propuesta                     | Impacto UX | Esfuerzo Dev | Prioridad |
| ----------------------------- | ---------- | ------------ | --------- |
| A. Mover INFO/PINS a pill bar | 🟢 Alto    | 🟡 Medio     | **P1**    |
| B. Agrupar shaders            | 🟡 Medio   | 🟢 Bajo      | **P2**    |
| C. Studio sin cambios         | —          | —            | —         |
| D. Simplificar Hero           | 🟡 Medio   | 🟢 Bajo      | **P2**    |
| E. Onboarding gestos          | 🟢 Alto    | 🟡 Medio     | **P1**    |

---

## 6. Recomendación Final

### Iteración 12 — Cambios Propuestos

1. **[P1] Mover INFO y PINS a la pill bar** como iconos pequeños junto a los 3 botones de modo, reduciendo Tools card grid a 2 cards (EXPLODE, FILTER). INFO se convierte en icono tipo ℹ️ y PINS en 📌 a la derecha de los mode buttons.

2. **[P1] Agregar coach marks** al primer uso: tooltip flotante sobre la pill bar que diga "Desliza hacia arriba o haz doble clic en una pieza para ver detalles".

3. **[P2] Separar shaders visualmente** en 2 filas: primera fila (REALISTIC, X-RAY, BLUEPRINT) y segunda fila (SOLID, WIRE, GHOST, THERMAL) con un sutil separador o etiqueta "Advanced".

4. **[P2] Simplificar Hero** a 2-3 botones: EXPLORE, ABOUT y opcionalmente SELECT DEVICE (con badge de estado).

### Lo que NO se recomienda cambiar:

- **El sistema de 3 modos** (Tools / Analyze / Studio) — está bien alineado con las tareas del usuario.
- **La navegación de dos niveles** (card grid → sub-panel) — implementa correctamente divulgación progresiva.
- **Cross-section sin toggle** — la navegación directa ya es óptima.
- **Studio mode** — ya es simple y efectivo.
- **Filtro de categorías** — 5 categorías es un número manejable y son semánticamente claras.

---

## 7. Métricas de Validación Sugeridas

Para evaluar los cambios una vez implementados:

| Métrica                           | Cómo medir                                       | Meta             |
| --------------------------------- | ------------------------------------------------ | ---------------- |
| Tiempo hasta primera acción       | Timestamp desde EXPLORE hasta primer tap en modo | < 3 segundos     |
| Profundidad de navegación         | Clicks hasta llegar a cualquier función          | ≤ 2 clicks       |
| Tasa de uso de gestos             | Analytics de swipe-up vs. botón INFO             | > 30% usan gesto |
| Tasa de descubrimiento de Shaders | % de usuarios que exploran > 3 modos             | > 50%            |
| Errores de tap                    | Clicks en zonas vacías / fuera de target         | < 5%             |

---

## 8. Referencias

- **Apple Human Interface Guidelines** — developer.apple.com/design
- **Material Design 3** — m3.material.io
- **Hick-Hyman Law** — Laws of UX (lawsofux.com)
- **Miller's Law** — "The Magical Number Seven, Plus or Minus Two" (1956)
- **Progressive Disclosure** — Nielsen Norman Group (nngroup.com)
- **Sketchfab Viewer UX** — sketchfab.com
- **Autodesk Viewer Mobile** — Fusion 360 iOS App
- **Guía Completa de Diseño UI/UX para Aplicaciones Móviles** — Documento interno del proyecto
