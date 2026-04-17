---
tipo: nota_consolidada
area: ux_ui
estado: activo
trace_id: STUB-NR-106
aliases:
  - "Selection_System.md"
resumen: "Sistema de seleccion jerarquica para el modelo 3D"
---

# Selection System

## Proposito

Esta nota consolida el sistema de seleccion del modelo 3D. Su funcion es permitir click, doble click y navegacion jerarquica entre pieza madre y subpieza con feedback visual claro.

## Reglas funcionales

- Click simple: selecciona la pieza activa.
- Doble click: profundiza en la jerarquia o activa foco contextual.
- Desseleccion: limpia el estado sin perder el contexto del modo actual.
- La seleccion debe emitir eventos consumibles por paneles, hotspots y modo de inspeccion.

## Integraciones necesarias

- Resaltado visual.
- Panel de detalles.
- Vista isolate/ghosted.
- Test cases de seleccion y multi-select.

## Evidencia relacionada

- `Selection_System_Tests.md`
- `Selection_Highlight_Shader.md`
- `Inspect_Panel_Data_Model.md`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

