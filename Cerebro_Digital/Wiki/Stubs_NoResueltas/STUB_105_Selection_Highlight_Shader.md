---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-105
aliases:
  - "Selection_Highlight_Shader.md"
resumen: "Shader o tecnica de resaltado para marcar la seleccion activa sin perder contexto"
---

# Selection Highlight Shader

## Proposito

El resaltado de seleccion debe distinguir la pieza activa de forma estable, visible y compatible con otros modos de visualizacion.

## Comportamiento esperado

- La seleccion debe leerse sobre cualquier fondo o material base.
- El resaltado no debe borrar textura ni silueta salvo que el modo lo exija.
- Debe coexistir con ghosted, isolate y inspect sin generar parpadeos.
- El patron visual debe ser consistente entre objetos similares.

## Criterios

1. Priorizar contraste antes que saturacion del efecto.
2. Evitar depender de un unico color si el estado debe sobrevivir a accesibilidad o temas visuales distintos.
3. Mantener estabilidad de borde y contour cuando la camara cambia.
4. Registrar la tecnica elegida si condiciona rendimiento o compatibilidad WebGL.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `MOC_UX_UI_Complete`
- `Ghosted_Mode_Implementation.md`
