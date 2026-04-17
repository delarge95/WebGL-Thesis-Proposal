---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-019
aliases:
  - "04_Arquitectura_Renderizado_URP.md"
resumen: "Arquitectura de render URP para organizar pipeline, material y compatibilidad"
---

# Arquitectura Renderizado URP

## Proposito

Define la arquitectura de render sobre URP para mantener una base coherente entre materiales, shaders y compatibilidad WebGL.

## Puntos clave

- Pipeline claro y mantenible.
- Compatibilidad con UI y modos visuales.
- Control de coste por etapa.
- Relacion con build y optimizacion.

## Criterios

1. La arquitectura debe apoyar depuracion y no ocultarla.
2. Cualquier cambio de pipeline debe dejar rastro.
3. El render debe permanecer estable entre modos.
4. Debe ser util para explicar decisiones tecnicas del proyecto.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `Blueprint_Mode_Shader.md`
- `Browser_Compatibility_Test.md`
