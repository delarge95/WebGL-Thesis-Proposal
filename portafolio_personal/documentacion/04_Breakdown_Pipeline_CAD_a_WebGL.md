# Breakdown: Pipeline CAD a WebGL

## Posicionamiento

Esta pieza debe venderse como un caso de Technical Art y optimizacion, no solo como modelado 3D.

## Problema

El proyecto parte de geometria tecnica que necesita limpieza, jerarquia consistente y adaptacion a un runtime WebGL con restricciones de memoria, draw calls y legibilidad visual.

## Lo que si se puede afirmar hoy

- Existe una ruta de trabajo real CAD -> Unity -> WebGL para el Holybro X500 V2.
- El proyecto usa un catalogo canonico de `28` piezas semanticas y una capa tecnica de `30 / 257`.
- El runtime final depende de saneamiento del import mediante `ImportedDroneRuntimeBinder`.
- El caso es valido como estudio de pipeline y normalizacion de activos.

## Lo que no debe afirmarse todavia

- metricas finales de optimizacion post-reimport si aun no se ha congelado la build;
- Houdini como ruta oficial de la version final;
- cifras definitivas de before/after sin medicion cerrada.

## Flujo recomendado para la pieza

1. problema de entrada: geometria tecnica y jerarquia compleja;
2. normalizacion semantica: `28` piezas canonicas;
3. normalizacion de escena: `30` anchors;
4. granularidad render: `257` renderers/colliders;
5. saneamiento runtime y reconstruccion de caches;
6. resultado final en navegador.

## Evidencia util

- diagrama simple del pipeline;
- capturas del ensamblaje final en la app;
- referencia a `ImportedDroneRuntimeBinder`;
- evidencia del catalogo canonico JSON;
- auditoria funcional final.

## Mensaje de venta

No importaste un modelo a Unity: construiste un flujo para convertir un activo tecnico en un producto interactivo navegable y documentable.
