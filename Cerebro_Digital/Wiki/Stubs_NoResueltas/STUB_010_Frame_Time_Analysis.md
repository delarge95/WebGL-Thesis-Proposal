---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-010
aliases:
  - "Frame_Time_Analysis.md"
resumen: "Analisis del tiempo de frame para relacionar simulacion, render y respuesta"
---

# Frame Time Analysis

## Proposito

Mide el tiempo de frame para detectar cuando el sistema termico o visual degrada la capacidad de respuesta del render o la simulacion.

## Variables utiles

- Media y variacion del frame.
- Picos y caidas intermitentes.
- Correlacion con carga termica.
- Impacto por cambios de shader o detalle.

## Criterios

1. La medida debe venir del mismo escenario de prueba.
2. No atribuir un problema termico sin corroboracion.
3. Cualquier optimizacion debe mostrar efecto en frame.
4. La nota debe ayudar a comparar antes y despues.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Custom Thermal Shader`
- `Thermal_Colormap_Design.md`
