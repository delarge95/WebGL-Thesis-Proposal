---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-043
aliases:
  - "Cut_Plane_Tool.md"
resumen: "Herramienta de plano de corte para inspeccion interna y lectura de capas"
---

# Cut Plane Tool

## Proposito

Permite abrir una seccion controlada del modelo para inspeccionar interior, relaciones entre capas y posibles interferencias.

## Comportamiento esperado

- El plano debe poder moverse y orientarse con precision.
- El corte no debe destruir contexto visual innecesariamente.
- Debe combinarse con selection e inspect.
- La herramienta debe ser reversible y predecible.

## Criterios

1. El plano de corte debe ser facil de entender por el usuario.
2. La lectura interna no debe ocultar la estructura general.
3. Registrar si el plano afecta rendimiento.
4. Documentar cualquier limitacion con geometria o shader.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `Cross_Section_Viewer.md`
- `Isolate_Rendering_Setup.md`
