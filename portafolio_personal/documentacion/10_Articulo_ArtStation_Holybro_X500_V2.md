# ArtStation Draft: Holybro X500 V2 WebGL Technical Viewer

## Título sugerido

Holybro X500 V2: Technical WebGL Viewer for Interactive Drone Visualization

## Subtítulo sugerido

An interactive WebGL viewer that connects runtime tooling, technical shaders, import repair and applied thermal visualization for a real drone platform.

## Estructura recomendada de publicación

### 1. Hero

Este proyecto parte de una pregunta simple: ¿cómo convertir un ensamblaje técnico complejo en una experiencia WebGL clara, navegable y útil para inspección?

Desarrollé un visor WebGL interactivo para el Holybro X500 V2 que permite explorar el dron por piezas, entender su estructura, aplicar modos de visualización técnica y mantener coherencia entre UI, escena y arquitectura runtime.

**Activos visuales recomendados aquí**

- Hero final del dron
- clip corto de navegación inicial

### 2. El problema

La dificultad no era solo mostrar un modelo 3D. El reto real estaba en conectar varias capas al mismo tiempo:

- una jerarquía importada compleja;
- una taxonomía clara de componentes;
- una UI legible para selección e inspección;
- modos analíticos compatibles con WebGL;
- y una arquitectura suficientemente limpia para mantener el sistema coherente.

En otras palabras, no se trataba de “poner un dron en Unity”, sino de traducir un activo técnico a un producto interactivo defendible.

### 3. El flujo visible del producto

El flujo principal del visor es:

```text
Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio
```

Cada parte del recorrido responde a una necesidad concreta:

- `Hero` presenta el caso real del producto;
- la selección conecta geometría con contexto;
- el `bottom sheet` convierte la pieza seleccionada en información usable;
- `Inspect`, `Analyze` y `Studio` organizan las herramientas de lectura del ensamblaje.

**Activos visuales recomendados aquí**

- selección de pieza
- `bottom sheet`
- grid corto de `Inspect / Analyze / Studio`

### 4. Sistema interactivo

La pieza central de la experiencia es el acoplamiento entre escena, selección y UI.

La app permite:

- seleccionar piezas reales del dron;
- abrir una ficha contextual de detalle;
- activar hotspots;
- aislar componentes;
- inspeccionar estados de energía/carga;
- usar explode, corte transversal y filtros por categoría;
- cambiar entre modos visuales orientados a lectura técnica.

El objetivo fue que la interacción no se sintiera como un catálogo estático, sino como una herramienta de exploración técnica.

### 5. Runtime architecture

Una parte importante del trabajo estuvo en mantener consistencia entre la estructura semántica del dron y la estructura técnica de la escena.

El proyecto usa tres niveles de lectura:

- `28` piezas canónicas de investigación;
- `30` anchors de escena;
- `257` renderers/colliders en la build auditada.

Para sostener esa coherencia, el runtime depende de `ImportedDroneRuntimeBinder`, que repara la jerarquía importada y reconstruye la base operativa necesaria para selección, visibilidad, explode, clipping y sistema térmico.

Este paso fue clave para convertir un modelo técnico en un sistema navegable y mantenible.

**Activos visuales recomendados aquí**

- diagrama corto de arquitectura por capas
- lámina `28 / 30 / 257`

### 6. Technical visualization modes

El visor no se apoya solo en rendering “realista”. También integra modos de lectura técnica para diferentes tareas de inspección.

Los modos visibles principales del cierre son:

- `Realistic`
- `X-Ray`
- `Solid Color`
- `Thermal`

Además existen modos implementados como profundidad técnica adicional, pero no los presento como UX final visible.

Esto fue importante para mantener honestidad entre lo que el sistema puede hacer internamente y lo que realmente publica la interfaz final.

### 7. Applied thermal visualization

Uno de los componentes más interesantes del proyecto fue el sistema térmico híbrido.

No está presentado como FEA ni como simulación física cerrada. Su valor está en otra parte:

- traducir estado operativo del dron a temperatura por componente;
- resolver una simulación heurística por nodos;
- convertir ese estado a un gradiente visual legible;
- y exponer el resultado con una leyenda clara dentro del visor.

La arquitectura se articula entre:

```text
DroneStateController
-> ThermalSimulationManager
-> ThermalViewController
-> Thermal shader + UI legend
```

Este sistema funciona mejor entendido como visualización técnica aplicada: suficiente para lectura interactiva y toma de decisiones visuales, sin sobreprometer fidelidad de laboratorio.

**Activos visuales recomendados aquí**

- modo térmico con leyenda
- diagrama del subsistema térmico

### 8. Tooling

Más allá de la app visible, construí herramientas de soporte para setup, auditoría y consistencia del pipeline.

Las piezas más útiles en este frente son:

- `ProjectSetupWizard`
- `ImportedDroneCoverageAudit`
- `ThermalContactGraphBuilderWindow`

Estas herramientas elevan el proyecto por encima de una simple demo visual, porque muestran pensamiento de producción, validación y mantenimiento.

**Activos visuales recomendados aquí**

- capturas del tooling de editor

### 9. CAD to WebGL pipeline

Otra parte importante del proyecto fue el estudio del pipeline CAD -> Unity -> WebGL.

El trabajo combinó:

- normalización semántica del dron;
- saneamiento runtime de la importación;
- análisis de restricciones WebGL;
- y diseño de una estrategia de optimización para geometría y fasteners.

La investigación de optimización y el blueprint técnico de Holybro forman parte de este proceso como diseño de pipeline y criterio técnico. Los uso como evidencia de cómo se pensó el sistema, no como prueba de que todas las etapas de optimización ya estén cerradas.

### 10. Límites honestos

Hay varias cosas que decidí no vender como parte del producto final:

- audio implementado;
- suite completa de ensamblaje expuesta al usuario final;
- catálogo visible como flujo oficial;
- measurement como feature publicada;
- Houdini como pipeline central de la build final.

Prefiero que el trabajo se lea desde lo que sí es real y verificable:

- el visor;
- la arquitectura;
- los shaders;
- el sistema térmico como visualización aplicada;
- y el tooling que sostiene el pipeline.

### 11. Cierre

Este proyecto resume bien el tipo de problemas que me interesa resolver como Technical Artist:

- traducir sistemas técnicos complejos a experiencias interactivas claras;
- conectar runtime, shading, tooling y documentación;
- y mantener límites honestos entre lo que está shipping, lo que está oculto y lo que pertenece al trabajo futuro.

## Texto corto alternativo para apertura

> I built an interactive WebGL technical viewer for the Holybro X500 V2, combining runtime import repair, technical visualization modes, applied thermal rendering and editor tooling to turn a complex drone assembly into a coherent inspection experience.

## Lista rápida de assets para montar esta publicación

- Hero final
- selección + `bottom sheet`
- grid de modos visibles
- thermal con leyenda
- diagrama de arquitectura
- captura de tooling
- imagen de pipeline CAD -> WebGL
