---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-004
aliases:
  - "shader_custom_thermal.md"
resumen: "Shader termico personalizado para representar temperatura de forma legible"
---

# Custom Thermal Shader

## Proposito

Convierte valores termicos en una visualizacion legible y estable, manteniendo correspondencia entre dato fisico y lectura visual.

## Criterios de uso

- La escala debe ser interpretable y consistente.
- El shader no debe ocultar discontinuidades relevantes.
- La paleta debe evitar ambiguedades al comparar estados.
- El coste de render debe quedar controlado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Thermal_Colormap_Design.md`
- `Thermal_Solver_State_Machine.md`
