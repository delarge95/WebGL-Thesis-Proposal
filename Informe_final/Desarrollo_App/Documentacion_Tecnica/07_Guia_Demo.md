# Guía de Demostración — WebGL Drone Viewer

> **Versión:** 1.0  
> **Última actualización:** Julio 2025  
> **Público objetivo:** Jurados académicos, evaluadores de usabilidad, partes interesadas  
> **Duración estimada:** 15-20 minutos

---

## 1. Acceso al Visor

<!-- TODO: Actualizar URL cuando se realice el despliegue final -->

1. Abrir un navegador moderno (Chrome 110+, Firefox 113+ o Edge 110+)
2. Navegar a: **[URL pendiente de despliegue final]**
3. Esperar la carga del visor (~10-20 segundos según la conexión)
4. El modelo 3D del dron aparecerá centrado en pantalla

> **Nota:** Si el visor no carga, verificar que WebGL 2.0 esté habilitado en el navegador. Escribir `chrome://gpu` en Chrome para verificar.

---

## 2. Recorrido Guiado

### Etapa 1 — Navegación Básica (2-3 min)

**Objetivo:** Familiarizarse con los controles de cámara.

| Acción          | Control (Mouse)            | Control (Touch)       |
| --------------- | -------------------------- | --------------------- |
| Orbitar         | Clic izquierdo + arrastrar | Un dedo + arrastrar   |
| Zoom            | Scroll rueda               | Pinch (dos dedos)     |
| Pan (desplazar) | Clic central + arrastrar   | Dos dedos + arrastrar |
| Reset cámara    | Doble clic en fondo        | Doble tap en fondo    |

**Demostrar:**

1. Orbitar alrededor del dron en 360°
2. Acercarse a una hélice con zoom
3. Desplazar la vista lateralmente con pan
4. Resetear la cámara al hacer doble clic

---

### Etapa 2 — Selección e Información de Piezas (2-3 min)

**Objetivo:** Mostrar la interacción con componentes individuales.

1. **Hacer clic** en cualquier pieza del dron (ej. "Motor Brushless")
2. Observar:
   - La pieza se resalta visualmente
   - Aparece el **panel de información** con datos técnicos
   - Se muestra: nombre, descripción, material, peso, función
3. **Hacer clic en otra pieza** para cambiar la selección
4. **Hacer clic en el fondo** para deseleccionar

**Puntos clave a destacar:**

- Cada pieza contiene datos técnicos reales del dron
- Los datos provienen de `ScriptableObject` (DronePartData)
- La información es extensible sin modificar código

---

### Etapa 3 — Modos de Visualización (3-4 min)

**Objetivo:** Recorrer los 7 modos de visualización disponibles.

Ubicar los botones de modo en la barra lateral izquierda y activar cada uno:

| #   | Modo            | Qué muestra                     | Uso educativo         |
| --- | --------------- | ------------------------------- | --------------------- |
| 1   | **Standard**    | Renderizado PBR realista        | Vista por defecto     |
| 2   | **X-Ray**       | Transparencia para ver interior | Componentes internos  |
| 3   | **Wireframe**   | Malla poligonal                 | Estructura geométrica |
| 4   | **Blueprint**   | Estilo plano técnico azul       | Documentación técnica |
| 5   | **Thermal**     | Mapa de calor falso             | Análisis térmico      |
| 6   | **Ghosted**     | Semi-transparente               | Relación espacial     |
| 7   | **Solid Color** | Color uniforme sólido           | Silueta y forma       |

**Demostrar:** Cambiar entre al menos 3-4 modos para mostrar la versatilidad. Destacar que cada modo usa shaders HLSL personalizados optimizados para WebGL 2.0.

---

### Etapa 4 — Vista Explosionada (2-3 min)

**Objetivo:** Mostrar la separación animada de componentes.

1. Activar la **vista explosionada** desde el botón correspondiente
2. Observar la animación de separación de todas las piezas
3. Con la vista explosionada activa:
   - Orbitar para ver la distribución espacial
   - Seleccionar piezas individuales — siguen siendo interactivas
4. Desactivar la vista explosionada para ver la animación de regreso

**Puntos clave:**

- Las piezas se separan a lo largo de vectores predefinidos
- La animación usa el sistema de tweening personalizado
- La interactividad se mantiene durante la explosión

---

### Etapa 5 — Cortes Transversales (2-3 min)

**Objetivo:** Demostrar la capacidad de seccionar el modelo.

1. Activar el **modo de corte transversal**
2. Seleccionar **Eje Y** (corte horizontal)
3. Arrastrar el slider para cortar el modelo progresivamente
4. Cambiar a **Eje X** y luego a **Eje Z** para otras orientaciones
5. Observar cómo el interior del dron queda visible
6. Desactivar el corte

**Puntos clave:**

- Funciona en tiempo real gracias a shader clipping (`ClippableLit`)
- Los cortes se persisten al cambiar de modo de visualización
- Útil para inspección de componentes internos no accesibles visualmente

---

### Etapa 6 — Catálogo de Partes (1-2 min)

**Objetivo:** Mostrar la búsqueda y navegación del inventario.

1. Abrir el **catálogo de partes** desde el menú
2. Observar la lista completa de componentes del dron
3. **Buscar** una pieza específica (ej. escribir "motor" en el buscador)
4. **Hacer clic** en una pieza del catálogo — la cámara se centra en ella
5. Observar cómo la pieza se selecciona automáticamente

**Puntos clave:**

- Búsqueda por nombre con filtrado en tiempo real
- Navegación bidireccional: catálogo ↔ modelo 3D
- Muestra la cantidad total de piezas del ensamblaje

---

### Etapa 7 — Herramientas de Ensamblaje (2-3 min)

**Objetivo:** Demostrar las herramientas especializadas para ingeniería.

1. **Puntos de Conexión:** Activar para ver los puntos de ensamblaje entre componentes
2. **Sistema de Anotaciones:** Mostrar notas técnicas asociadas a piezas
3. **Guía de Ensamblaje:** Recorrer los pasos secuenciales de armado
4. **BOM (Bill of Materials):** Mostrar la lista de materiales con cantidades
5. **Checklist de Ensamblaje:** Verificar la presencia de piezas

**Puntos clave:**

- Herramientas orientadas al uso profesional/educativo
- Datos estructurados como ScriptableObjects extensibles
- Interfaz consistente con el design system via UI Toolkit

---

### Etapa 8 — Configuración (1 min)

**Objetivo:** Mostrar las opciones de personalización.

1. Abrir el **panel de configuración** (⚙️)
2. Mostrar los controles disponibles:
   - Volumen de sonido (Master, SFX, Música)
   - Opciones de visualización
3. Ajustar algún parámetro para demostrar la interactividad
4. Cerrar el panel

---

## 3. Puntos Técnicos Destacados (para Jurados)

| Aspecto             | Detalle                                                            |
| ------------------- | ------------------------------------------------------------------ |
| **Arquitectura**    | 4 capas (Core, Features, UI, Data) con 6 patrones de diseño        |
| **Rendimiento**     | ~100K polígonos, target 60 FPS, optimización runtime automática    |
| **Portabilidad**    | WebGL 2.0 — accesible desde cualquier navegador moderno            |
| **Sin instalación** | No requiere plugins, extensiones ni software adicional             |
| **Responsivo**      | Se adapta a diferentes tamaños de pantalla                         |
| **UI Toolkit**      | Sistema de interfaz basado en componentes estilizados con USS      |
| **Shaders**         | 9 shaders HLSL/CG personalizados para los 7 modos de visualización |
| **Persistencia**    | Preferencias y estado se guardan entre sesiones (PlayerPrefs)      |
| **Pipeline 3D**     | Modelos CAD → Retopología → Baking → PBR → WebGL                   |

---

## 4. Preguntas Frecuentes de Evaluadores

**¿Funciona en móviles?**
Sí, con detección automática y ajuste de calidad. Los controles touch están implementados. El rendimiento depende de la capacidad de la GPU del dispositivo.

**¿Los datos del dron son reales?**
Sí, los componentes representan partes reales de un dron con datos técnicos verificados (materiales, pesos, funciones).

**¿Se pueden agregar más drones/modelos?**
La arquitectura es extensible. Agregar un nuevo modelo requiere: (1) asset 3D optimizado, (2) ScriptableObjects con datos de piezas, (3) configuración de vectores de explosión.

**¿Qué tamaño tiene la aplicación?**
Objetivo: < 40 MB comprimido (WASM + datos). La primera carga es la más lenta; las siguientes usan caché del navegador.

**¿Se puede usar offline?**
Una vez cargada, la aplicación funciona sin conexión gracias al Data Caching en IndexedDB. La primera carga sí requiere conexión.

---

## 5. Métricas a Observar Durante la Demo

<!-- TODO: Actualizar con valores reales después del Bloque B -->

| Métrica            | Target                     | Cómo Verificar                            |
| ------------------ | -------------------------- | ----------------------------------------- |
| Tiempo de carga    | < 10 s (conexión 50 Mbps)  | Cronómetro desde URL hasta modelo visible |
| FPS promedio       | ≥ 30 desktop / ≥ 24 mobile | `WebGLProfiler` overlay o DevTools        |
| Tamaño build       | < 40 MB comprimido         | Verificar `docs/Build/`                   |
| Interacción fluida | Sin stuttering visible     | Prueba de órbita continua                 |
| Cambio de modo     | < 500 ms                   | Tiempo de transición visual               |
