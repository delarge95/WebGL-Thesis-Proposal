# Demo Script - WebGL Drone Viewer
## Guión para Demostración en Vivo (5 minutos)

---

## Preparación Pre-Demo

### Checklist Técnico
- [ ] URL del prototipo abierta en Chrome/Firefox
- [ ] Pantalla completa (F11)
- [ ] Conexión a internet estable
- [ ] Mouse funcionando correctamente
- [ ] Backup PDF con capturas por si falla internet
- [ ] Segundo navegador listo con demo backup

### Estado Inicial
- Vista: Realistic
- Zoom: 100%
- Rotación: Vista frontal 3/4
- Ninguna pieza seleccionada
- Vista compacta (no explosionada)

---

## Guión de Demostración

### [0:00-0:30] Introducción Visual

**Acción:** Mostrar el prototipo en estado inicial

> "Este es nuestro prototipo de visualización 3D interactiva. Como pueden ver, tenemos un drone de alta performance renderizado en tiempo real en el navegador, sin plugins ni instalación."

**Acción:** Rotar lentamente el drone 360°

> "El modelo se puede explorar desde cualquier ángulo usando el mouse."

---

### [0:30-1:15] Navegación Básica

**Acción:** Demostrar controles

> "La navegación es intuitiva:"

| Acción | Control |
|--------|---------|
| Click izquierdo + arrastrar | "Rotar" |
| Scroll | "Zoom in/out" |
| Click derecho + arrastrar | "Desplazar (Pan)" |

**Acción:** Usar presets de cámara (1-6)

> "También tenemos ángulos predefinidos para vistas técnicas: frontal, lateral, superior..."

---

### [1:15-2:00] Selección e Información

**Acción:** Click en el motor

> "Al hacer click en cualquier componente, obtenemos información técnica detallada."

**Acción:** Señalar el panel de información

> "Aquí vemos: nombre, categoría, peso, material, dimensiones, y especificaciones de instalación como torque y herramientas necesarias."

**Acción:** Seleccionar la batería

> "Noten las advertencias de seguridad específicas para componentes críticos como la batería LiPo."

---

### [2:00-2:45] Modos de Visualización

**Acción:** Cambiar a modo X-Ray

> "Tenemos 7 modos de visualización especializados. El modo X-Ray permite ver componentes internos..."

**Acción:** Cambiar a Blueprint

> "El modo Blueprint muestra una vista técnica estilo plano de ingeniería..."

**Acción:** Cambiar a Thermal

> "Y el modo térmico simula zonas de calor para análisis de disipación."

**Acción:** Volver a Realistic

> "Cada modo tiene un shader HLSL personalizado optimizado para WebGL."

---

### [2:45-3:30] Vista Explosionada

**Acción:** Activar vista explosionada (slider o botón E)

> "La vista explosionada separa los componentes para visualizar la estructura interna..."

**Acción:** Mover slider de explosión

> "El nivel de separación es controlable con este slider."

**Acción:** Seleccionar pieza en vista explosionada

> "Incluso en estado explosionado, cada pieza mantiene su información contextual."

**Acción:** Volver a vista compacta

---

### [3:30-4:15] Herramientas de Ingeniería

**Acción:** Activar herramienta de medición

> "Las herramientas de ingeniería incluyen medición de distancias entre puntos..."

**Acción:** Medir distancia entre dos puntos

> "Útil para verificar espacios y tolerancias."

**Acción:** Mostrar puntos de conexión

> "Los puntos de conexión visualizan dónde van tornillos, cables y snaps."

**Acción:** Mostrar catálogo de partes

> "Y el catálogo permite buscar y filtrar componentes por categoría."

---

### [4:15-4:45] Corte Transversal

**Acción:** Activar cross-section en eje Y

> "El corte transversal permite ver secciones del modelo en cualquier eje."

**Acción:** Mover plano de corte

> "Ideal para inspeccionar cómo encajan los componentes internos."

**Acción:** Desactivar corte

---

### [4:45-5:00] Cierre

**Acción:** Volver a vista completa, rotación suave

> "Todo esto corriendo a más de 30 FPS en el navegador, sin instalación, accesible desde cualquier dispositivo con WebGL 2.0."

> "¿Alguna pregunta sobre la demostración?"

---

## Backup: Tareas Alternativas

Si el tiempo sobra o se solicita ver más:

| Tarea | Tiempo |
|-------|--------|
| Simular encendido del drone | 30s |
| Exportar lista de materiales (BOM) | 20s |
| Mostrar anotaciones 3D | 20s |
| Mostrar accesibilidad (alto contraste) | 20s |

---

## Troubleshooting

| Problema | Solución |
|----------|----------|
| Carga lenta | "La primera carga descarga assets, las siguientes son más rápidas" |
| FPS bajo | "En dispositivos más potentes el rendimiento mejora" |
| Error de WebGL | Cambiar a backup PDF/video |
| Mouse no responde | Verificar foco del navegador |

---

## Frases Clave a Recordar

- "Sin plugins, sin instalación"
- "Optimizado para web con WebAssembly"
- "Shaders personalizados HLSL"
- "Arquitectura basada en patrones de diseño"
- "Más de 70 scripts, 10,000 líneas de código"
- "7 modos de visualización, cada uno con propósito técnico específico"

---

*Demo Version: 1.0*
*Duration: 5 minutes*
*Project: WebGL Drone Viewer*
