---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-135
aliases:
  - "ViewMode_Switching_Tests.md"
resumen: "Pruebas de cambio entre modos de vista para validar continuidad y estado"
---

# ViewMode Switching Tests

## Proposito

Verifica que el cambio entre modos de vista conserve estado, contexto y comportamiento consistente sin romper la experiencia.

## Cobertura

- Cambio entre overview, wireframe, blueprint, ghosted e isolate.
- Persistencia de selección y foco.
- Coherencia visual al volver al modo anterior.
- Rendimiento al alternar repetidamente.

## Criterios

1. Cada cambio debe ser predecible.
2. La prueba debe indicar si el estado se conserva o se reinicia.
3. No debe haber parpadeo o inconsistencias.
4. La nota debe enlazar con los modos concretos afectados.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `Overview_Mode_Spec.md`
- `Isolate_Rendering_Setup.md`
