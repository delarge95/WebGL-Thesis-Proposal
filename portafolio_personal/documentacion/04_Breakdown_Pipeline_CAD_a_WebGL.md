# Breakdown: Pipeline CAD a Unity WebGL

## Posicionamiento

Esta pieza debe venderse como un caso de `Technical Art + optimization + systems thinking`, no como simple modelado 3D ni como promesa de que toda la optimización ya quedó cerrada.

## Problema

El proyecto parte de geometría técnica compleja, procedente de referencias CAD y de ensamblajes con muchas subpiezas, que debe transformarse en una experiencia WebGL navegable, legible y mantenible.

Los retos principales son:

- jerarquías importadas inconsistentes;
- granularidad geométrica excesiva;
- fasteners y subpiezas con alto costo para WebGL;
- necesidad de conservar semántica técnica sin convertir el pipeline en una caja negra.

## Qué sí se puede afirmar hoy

- Existe una ruta defendible `CAD -> Blender -> Unity -> WebGL`.
- El proyecto usa una taxonomía semántica canónica de `28` piezas, una capa de `30` anchors y una capa técnica de `257` renderers/colliders.
- El runtime final depende de saneamiento del import mediante `ImportedDroneRuntimeBinder`.
- La build final visible se beneficia de tooling y auditorías para mantener coherencia entre datos, escena y UX.
- Hay investigación técnica seria sobre optimización CAD-to-WebGL y fasteners, ya convertida en un blueprint útil para el cierre.

## Qué no debe afirmarse todavía

- métricas finales de optimización post-reimport si la build congelada aún no existe;
- comparativas definitivas before/after no medidas;
- Houdini como ruta oficial de la versión final;
- PiXYZ, Simplygon o InstaLOD como herramientas realmente usadas si solo viven en el research consultado;
- que la fase de fasteners y LODs ya está cerrada cuando el propio proyecto la mantiene como pendiente real.

## Ruta defendible actual

### 1. Normalización semántica

El catálogo `x500v2_parts_data.json` fija las `28` piezas canónicas. Esta es la capa académica y de producto que debe usarse en cualquier explicación pública.

### 2. Normalización de escena

La escena final opera con `30` anchors:

- 28 piezas canónicas;
- `x500v2_fastener_group`;
- `x500v2_misc_group`.

### 3. Granularidad render

La escena auditada contiene `257` renderers/colliders. Esta cifra sirve para hablar de granularidad técnica, no para redefinir la taxonomía del dron.

### 4. Runtime repair

`ImportedDroneRuntimeBinder` repara la jerarquía importada en runtime, reubica elementos huérfanos, sintetiza grupos y reconstruye cachés necesarias para:

- selección;
- explode;
- visibilidad;
- cross-section;
- view modes;
- sistema térmico.

### 5. Resultado web publicable

El resultado vendible no es la “malla limpia” por sí sola, sino la transición completa desde activo técnico complejo hasta visor web interactivo usable.

## Qué aporta el blueprint de Holybro al breakdown

El documento `CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md` sí aporta valor al portafolio, pero como research de diseño de pipeline y no como evidencia de todo lo ya implementado.

Los puntos más útiles para integrar en la narrativa son:

- por qué `LOD Groups` son mejores que un enfoque dinámico tipo `Zoom-Swap` para WebGL;
- por qué los fasteners requieren estrategia específica y no solo decimate genérico;
- por qué una librería modular de fasteners con proxies e instancing es más realista que cientos de mallas únicas densas;
- por qué la optimización debe leerse junto a restricciones de draw calls, memoria, garbage collection y legibilidad.

## Qué puede mencionarse como R&D bien argumentado

- evaluación de `LOD Groups`;
- estrategia de librerías modulares para fasteners;
- decimación planar y collapse en Blender;
- posibilidad de usar herramientas como Quad Remesher o soluciones CAD-native como referencias del espacio de diseño.

## Qué no debe presentarse como implementado

- pipelines automatizados externos no integrados en la build final;
- cifras cerradas de reducción si no se midieron;
- un sistema final de fasteners modulares si sigue en curso.

## Estructura recomendada del breakdown

1. Problema de entrada.
2. Restricciones WebGL.
3. Taxonomía `28 / 30 / 257`.
4. Saneamiento runtime con `ImportedDroneRuntimeBinder`.
5. Research de optimización y decisiones de pipeline.
6. Resultado navegable en browser.
7. Pendientes honestos antes del freeze final.

## Evidencia útil

- `x500v2_parts_data.json`;
- `ImportedDroneRuntimeBinder.cs`;
- `ImportedDroneCoverageAudit.cs`;
- `VALIDACION_FUNCIONAL_FINA_2026-04-09.md`;
- diagrama de arquitectura o de flujo de datos;
- capturas del dron ya integrado en la app;
- comparativa visual de malla/proxy/resultado si y solo si existe evidencia clara.

## Activos visuales recomendados

- una lámina del problema original de import y granularidad;
- un diagrama del pipeline CAD -> Unity -> WebGL;
- una imagen explicando `28 / 30 / 257`;
- una captura del resultado final en la app;
- una lámina opcional sobre fasteners como hotspot de optimización.

## Mensaje de venta

La pieza no debe sonar a “importé un modelo a Unity”. Debe sonar a esto:

> Convertí un activo técnico complejo en un visor WebGL navegable mediante normalización semántica, saneamiento runtime, tooling de validación y una estrategia explícita de optimización para geometría y fasteners.
