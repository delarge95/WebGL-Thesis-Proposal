# Demo Script - WebGL Drone Viewer
## Guion para demostracion en vivo coherente con la app real

---

## Estado del guion

Este guion reemplaza versiones anteriores que mencionaban herramientas no publicadas en la UI final, como medicion, BOM, conexiones o catalogo legacy. La demo debe mostrar solo el flujo real: Hero, Explore, seleccion, bottom sheet, Inspect, Analyze, Studio, fasteners e import final Blender cuando ya este validado.

No se deben afirmar FPS, reducciones de peso o tiempos de carga hasta medirlos sobre la build congelada.

---

## Preparacion pre-demo

### Checklist tecnico

- [ ] Abrir la build WebGL o Play Mode con `MainScene_Final`.
- [ ] Restablecer vista inicial del dron completo.
- [ ] Verificar que no haya pieza seleccionada al iniciar.
- [ ] Verificar que el modo base sea `Realistic`.
- [ ] Tener capturas/video de respaldo por si falla la demo en vivo.
- [ ] Confirmar que los controles de zoom, pan y orbit responden sin perder piezas pequenas.

### Estado inicial recomendado

- Vista: `Realistic`.
- Dron completo visible.
- Explode desactivado.
- Cross-section desactivado.
- Ningun filtro activo.
- Ninguna pieza seleccionada.

---

## Guion de demostracion

### [0:00-0:30] Apertura

**Accion:** Mostrar el prototipo en estado inicial.

> "Este es el visor WebGL del Holybro X500 V2. La idea no es solo mostrar un modelo 3D, sino convertir el ensamblaje en una experiencia inspeccionable, filtrable y explicable desde navegador."

**Accion:** Orbitar suavemente el dron.

> "La escena conserva lectura espacial completa y permite explorar el sistema desde cualquier angulo."

---

### [0:30-1:05] Navegacion

**Accion:** Demostrar orbit, zoom y pan.

| Accion | Control |
|--------|---------|
| Orbit | Arrastrar en viewport |
| Zoom | Scroll |
| Pan | Arrastre de paneo configurado |

> "La camara ajusta sensibilidad y rango segun la escala de lo que se analiza. No es lo mismo navegar el dron completo que acercarse a un fastener."

---

### [1:05-1:55] Seleccion y ficha inferior

**Accion:** Seleccionar una pieza madre clara, por ejemplo placa, brazo o motor.

> "Al seleccionar una pieza, la app abre una ficha inferior. Esta ficha no es decorativa: traduce una malla seleccionada a informacion tecnica."

**Accion:** Senalar nombre, categoria, especificaciones y ensamblaje.

> "La seleccion puede representar una pieza madre, una subpieza, un grupo de hotspot o un fastener. La app diferencia esos niveles para que aislamiento, resaltado y datos no se contradigan."

---

### [1:55-2:40] Inspect, aislamiento y fasteners

**Accion:** Usar `Inspect` y aislar una pieza madre con fasteners asociados.

> "Inspect permite limpiar el contexto visual. Si se aisla una pieza madre, se conservan los fasteners reconciliados con esa pieza cuando existe asignacion confiable."

**Accion:** Seleccionar o aislar un fastener individual.

> "Un fastener tambien puede aislarse como unidad completa. Cuando se requiere detalle, el sistema reemplaza el proxy por una representacion modular bajo demanda, sin convertir toda la tornilleria de la escena."

**Nota oral opcional:** Si la escena usa todavia piezas modulares temporales de Unity:

> "La geometria modular visible en esta etapa es temporal; la logica de datos, seleccion e inspeccion ya esta lista para recibir los modulos finales hechos en Blender."

---

### [2:40-3:35] Analyze

**Accion:** Activar vista explosionada y mover el slider.

> "Analyze permite separar visualmente el ensamblaje para leer relaciones entre piezas sin destruir la malla ni perder metadatos."

**Accion:** Activar cross-section.

> "El corte transversal funciona como una decision de render: el shader decide que fragmentos se dibujan segun un plano matematico."

**Accion:** Mostrar filtros por categoria.

> "Los filtros reducen ruido visual por sistemas funcionales: estructura, propulsion, avionica, comunicaciones, distribucion de energia y fasteners."

---

### [3:35-4:30] Studio y modos visuales visibles

**Accion:** Mostrar `X-Ray`, `Solid Color` y `Thermal`.

> "Studio controla la lectura visual. En la UI final se exponen modos como X-Ray, Solid Color y Thermal sobre una base Realistic."

**Accion:** Cambiar entorno o preset si aplica.

> "Blueprint se maneja como preset/lectura de entorno cuando esta disponible, no como una promesa de siete modos visibles al usuario."

**Accion:** Mostrar Thermal solo como lectura heuristica.

> "Thermal no es FEA ni una medicion fisica calibrada. Es una lupa visual por componentes para comunicar tendencias relativas de temperatura."

---

### [4:30-5:00] Cierre

**Accion:** Volver a dron completo.

> "El resultado defendible es una cadena completa: CAD y Blender para preparar geometria, Unity para interaccion y WebGL para acceso. Las metricas finales se reportan solo despues del freeze y del profiling de la build congelada."

> "La contribucion no es mostrar mas piezas; es hacer legibles sus relaciones."

---

## Backup si falla la demo

| Riesgo | Respuesta |
|--------|-----------|
| Carga lenta | Usar capturas o video grabado del flujo real. |
| WebGL falla | Presentar Play Mode o respaldo visual. |
| FPS inestable | No improvisar resultados; explicar que el capitulo 5 se cierra con profiling post-freeze. |
| Fastener ambiguo | Mostrar que el sistema lo reporta para revision y no lo asigna por suposicion. |

---

## Frases seguras

- "La app distingue entre pieza madre, subpieza, hotspot y fastener."
- "Blueprint y modos ocultos se tratan como capacidades implementadas o presets, no como alcance visible si no estan publicados."
- "Thermal es heuristico, no FEA."
- "Los placeholders de fasteners validan el sistema modular; las mallas finales Blender reemplazan assets sin cambiar la logica."
- "Las metricas se reportan despues del freeze de build."

---

*Demo Version: 2.0*
*Last Updated: 2026-05-08*
*Project: WebGL Drone Viewer*
