# Breakdown: Pipeline CAD a Unity WebGL

## Posicionamiento

Esta pieza debe venderse como un caso de `Technical Art + optimization + systems thinking`, no como simple modelado 3D ni como promesa de que toda la optimizacion ya quedo cerrada.

## Problema

El proyecto parte de geometria tecnica compleja, procedente de referencias CAD y de ensamblajes con muchas subpiezas, que debe transformarse en una experiencia WebGL navegable, legible y mantenible.

Los retos principales son:

- jerarquias importadas inconsistentes;
- granularidad geometrica excesiva;
- fasteners y subpiezas con alto costo para WebGL;
- necesidad de conservar semantica tecnica sin convertir el pipeline en una caja negra.

## Que si se puede afirmar hoy

- Existe una ruta defendible `CAD -> Blender -> Unity -> WebGL`.
- El proyecto usa una taxonomia semantica canonica de `28` piezas, una capa de `30` anchors y una capa tecnica de `257` renderers/colliders.
- El runtime final depende de saneamiento del import mediante `ImportedDroneRuntimeBinder`.
- La build final visible se beneficia de tooling y auditorias para mantener coherencia entre datos, escena y UX.
- La capa Unity de fasteners ya quedo implementada con `20` familias, `168` instancias y catalogos reconciliados para detalle bajo demanda.
- Hay investigacion tecnica seria sobre optimizacion CAD-to-WebGL y fasteners, ya convertida en un blueprint util para el cierre.

## Que no debe afirmarse todavia

- metricas finales de optimizacion post-reimport si la build congelada aun no existe;
- comparativas definitivas before/after no medidas;
- Houdini como ruta oficial de la version final;
- PiXYZ, Simplygon o InstaLOD como herramientas realmente usadas si solo viven en el research consultado;
- que la geometria Blender final de fasteners ya esta cerrada cuando la escena documentada sigue usando proxies temporales.

## Ruta defendible actual

### 1. Normalizacion semantica

El catalogo `x500v2_parts_data.json` fija las `28` piezas canonicas. Esta es la capa academica y de producto que debe usarse en cualquier explicacion publica.

### 2. Normalizacion de escena

La escena final opera con `30` anchors:

- 28 piezas canonicas;
- `x500v2_fastener_group`;
- `x500v2_misc_group`.

### 3. Granularidad render

La escena auditada contiene `257` renderers/colliders. Esta cifra sirve para hablar de granularidad tecnica, no para redefinir la taxonomia del dron.

### 4. Runtime repair

`ImportedDroneRuntimeBinder` repara la jerarquia importada en runtime, reubica elementos huerfanos, sintetiza grupos y reconstruye caches necesarias para:

- seleccion;
- explode;
- visibilidad;
- cross-section;
- view modes;
- sistema termico;
- catalogos e inspeccion de fasteners.

### 5. Fasteners como subsistema

Hoy ya puede afirmarse esto sin exagerar:

- Unity conserva proxies ligeros para fasteners en reposo.
- La app resuelve metadata real por instancia mediante `holybro_fastener_families.json`, `holybro_fastener_instances.json` y `holybro_fastener_reconciliation.json`.
- Solo el fastener seleccionado genera detalle procedural temporal en runtime.
- El contrato `familyId / instanceId / recipeKey` permite reemplazar luego los placeholders por mallas Blender finales sin reescribir la logica.

### 6. Resultado web publicable

El resultado vendible no es la "malla limpia" por si sola, sino la transicion completa desde activo tecnico complejo hasta visor web interactivo usable.

## Que aporta el blueprint de Holybro al breakdown

El documento `CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md` si aporta valor al portafolio, pero como research de diseno de pipeline y no como evidencia de todo lo ya implementado.

Los puntos mas utiles para integrar en la narrativa son:

- por que `LOD Groups` son mejores que un enfoque dinamico tipo `Zoom-Swap` para WebGL;
- por que los fasteners requieren estrategia especifica y no solo decimate generico;
- por que una libreria modular de fasteners con proxies e instancing es mas realista que cientos de mallas unicas densas;
- por que la optimizacion debe leerse junto a restricciones de draw calls, memoria, garbage collection y legibilidad.

## Que puede mencionarse como R&D bien argumentado

- evaluacion de `LOD Groups`;
- estrategia de librerias modulares para fasteners;
- decimacion planar y collapse en Blender;
- posibilidad de usar herramientas como Quad Remesher o soluciones CAD-native como referencias del espacio de diseno.

## Que no debe presentarse como implementado

- pipelines automatizados externos no integrados en la build final;
- cifras cerradas de reduccion si no se midieron;
- mallas Blender finales de fasteners si la escena publicada sigue mostrando placeholders temporales.

## Estructura recomendada del breakdown

1. Problema de entrada.
2. Restricciones WebGL.
3. Taxonomia `28 / 30 / 257`.
4. Saneamiento runtime con `ImportedDroneRuntimeBinder`.
5. Fasteners como subsistema de optimizacion.
6. Research de pipeline y decisiones de optimizacion.
7. Resultado navegable en browser.
8. Pendientes honestos antes del freeze final.

## Evidencia util

- `x500v2_parts_data.json`;
- `holybro_fastener_families.json`;
- `holybro_fastener_instances.json`;
- `holybro_fastener_reconciliation.json`;
- `ImportedDroneRuntimeBinder.cs`;
- `FastenerRegistry.cs`;
- `FastenerInspectionManager.cs`;
- `VALIDACION_FUNCIONAL_FINA_2026-04-09.md`;
- diagrama de arquitectura o de flujo de datos;
- capturas del dron ya integrado en la app;
- comparativa visual de malla/proxy/resultado si y solo si existe evidencia clara.

## Activos visuales recomendados

- una lamina del problema original de import y granularidad;
- un diagrama del pipeline CAD -> Unity -> WebGL;
- una imagen explicando `28 / 30 / 257`;
- una captura del resultado final en la app;
- una lamina opcional sobre fasteners como hotspot de optimizacion;
- una captura del flujo proxy -> detail para un fastener seleccionado.

## Mensaje de venta

La pieza no debe sonar a "importe un modelo a Unity". Debe sonar a esto:

> Converti un activo tecnico complejo en un visor WebGL navegable mediante normalizacion semantica, saneamiento runtime, tooling de validacion y una estrategia explicita de optimizacion para geometria y fasteners.
