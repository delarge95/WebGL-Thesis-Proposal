---
tipo: nota_consolidada
area: ux_ui
estado: activo
trace_id: STUB-NR-041
aliases:
  - "Cross_Section_Viewer.md"
resumen: "Vista de seccion transversal para inspeccion interna del ensamblaje"
---

# Cross Section Viewer

## Proposito

Esta nota consolida la vista de seccion transversal usada para cortar el modelo y revisar capas internas, alineaciones y relaciones espaciales entre componentes.

## Comportamiento esperado

- El plano de corte debe ser controlable por el usuario.
- La visualizacion debe preservar contexto suficiente del ensamblaje.
- El feedback debe indicar claramente la posicion y orientacion del corte.
- Debe poder combinarse con inspect, isolate y selection.

## Reglas de uso

1. Activar el corte solo cuando el modo lo permita.
2. Mantener coherencia con el sistema de seleccion.
3. Registrar limitaciones de rendimiento si el shader o la malla lo requieren.

## Evidencia relacionada

- `Cut_Plane_Tool.md`
- `Isolate_Rendering_Setup.md`
- `MOC_UX_UI_Complete`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

