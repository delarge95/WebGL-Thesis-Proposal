# Guia de Demostracion - WebGL Drone Viewer

> Version: 2.0  
> Ultima actualizacion: 2026-05-08  
> Alcance: demo real de la app actual, sin herramientas legacy ni metricas no medidas.

---

## Regla principal

La demo debe mostrar solo el flujo visible:

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

No mostrar como final:

- medicion;
- BOM;
- connection points;
- anotaciones;
- assembly checklist;
- catalogo legacy;
- siete modos visibles si la UI solo expone el subconjunto operativo.

Para el guion oral de 5 minutos usar tambien:

- `Informe_final/presentation/DEMO_SCRIPT.md`
- `docs/manuals/DEMO_SCRIPT.md`

---

## Preparacion

1. Abrir la build WebGL o `MainScene_Final` en Play Mode.
2. Confirmar que no haya pieza seleccionada.
3. Confirmar modo base `Realistic`.
4. Confirmar que `Explode`, `Cut` y filtros esten en estado inicial.
5. Tener video/capturas de respaldo por si la PC o WebGL fallan.

No afirmar FPS, tiempos de carga, reducciones o resultados SUS/NASA-TLX si no fueron medidos en la build congelada.

---

## Recorrido recomendado

### 1. Apertura

Mostrar el dron completo y explicar que la app convierte el ensamblaje Holybro X500 V2 en una experiencia inspeccionable, no solo en un modelo 3D.

### 2. Navegacion

Demostrar orbit, zoom y pan. Aclarar que la camara adapta zoom/pan/orbit segun la escala de analisis para no perder fasteners o subpiezas pequenas.

### 3. Seleccion y bottom sheet

Seleccionar una pieza madre. Abrir la ficha inferior y senalar:

- identificacion;
- especificaciones;
- ensamblaje;
- nivel de seleccion: pieza madre, subpieza, hotspot o fastener.

### 4. Inspect

Mostrar:

- hotspots/pins;
- isolate de pieza madre;
- isolate de fastener individual;
- power/load si aporta al flujo.

En fasteners, aclarar que el sistema puede activar detalle modular temporal bajo demanda y que las mallas finales Blender reemplazaran assets/recetas sin cambiar IDs.

### 5. Analyze

Mostrar:

- explode con slider;
- cross-section;
- filtros por categoria: estructura, propulsion, avionica, sensores, power y fasteners.

### 6. Studio

Mostrar:

- `X-Ray`;
- `Solid`;
- `Thermal`;
- presets de entorno/iluminacion, incluyendo `Blueprint` si esta disponible desde el ciclo de entorno.

Thermal debe presentarse como visualizacion heuristica, no FEA ni medicion fisica calibrada.

### 7. Cierre

Cerrar con la idea:

> "La contribucion no es mostrar mas piezas; es hacer legibles sus relaciones."

---

## Preguntas esperadas

| Pregunta | Respuesta segura |
|----------|------------------|
| Hay 7 modos? | El sistema implementa varios modos, pero la UI final expone el subconjunto operativo. Lo oculto se documenta como capacidad no publicada. |
| Thermal es simulacion fisica? | No. Es una visualizacion heuristica por componentes, no FEA. |
| Los fasteners son definitivos? | La logica Unity si; las mallas detalladas pueden reemplazarse desde Blender por receta/asset. |
| El FBX final ya cierra resultados? | No. Debe verificarse en Unity con reporte de importacion, texturas finales y profiling. |
| Donde estan las metricas? | En capitulo 5 se llenan solo post-freeze con datos medidos. |

---

## Checklist de QA antes de demo publica

- [ ] El modelo completo se ve correctamente.
- [ ] Las helices giran sobre el eje correcto.
- [ ] Seleccion, hover y color selected funcionan.
- [ ] Cada fastener puede aislarse como unidad completa.
- [ ] Una pieza madre aislada conserva sus fasteners reconciliados.
- [ ] El detalle modular de fasteners aparece solo bajo contexto necesario.
- [ ] Zoom, pan y orbit se sienten correctos en dron completo y piezas pequenas.
- [ ] Analyze conserva explode, cut y filtros.
- [ ] Studio conserva modos visibles y entorno.
- [ ] Thermal tiene leyenda y se explica como heuristico.
- [ ] No se muestran herramientas legacy como feature final.
