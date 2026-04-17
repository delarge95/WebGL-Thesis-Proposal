---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-093
aliases:
  - "Performance_Thermal_Render.md"
resumen: "Rendimiento del render termico y su impacto sobre la estabilidad de frame"
---

# Performance Thermal Render

## Proposito

Analiza el coste del render termico para detectar si la visualizacion condiciona la estabilidad, la fluidez o la calidad de lectura.

## Aspectos observables

- Carga de shader termico.
- Coste de actualizacion por frame.
- Variacion de rendimiento con escena pesada.
- Interaccion con otros modos visuales.

## Criterios

1. El analisis debe separar coste termico y coste visual.
2. Debe indicar si el problema es de datos, shader o escena.
3. Las conclusiones deben apoyar priorizacion tecnica.
4. La nota debe poder compararse entre builds.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `Custom Thermal Shader`
- `Performance Metrics Summary`
