---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-050
aliases:
  - "Explode_View_Animation.md"
resumen: "Secuencia de explosion de vistas para separar piezas y leer la estructura del ensamblaje"
---

# Explode View Animation

## Proposito

La explosion de vista separa componentes del ensamblaje de forma controlada para facilitar inspeccion, explicacion y verificacion de montaje.

## Comportamiento esperado

- La separacion debe ser reversible y predecible.
- Las piezas deben moverse siguiendo un patron claro, no aleatorio.
- La secuencia debe poder activarse por pasos o como animacion completa.
- El estado de explosion debe convivir con selection e inspect.

## Criterios

1. No perder la relacion espacial entre piezas hermanas.
2. Mantener legibilidad del ensamblaje original al volver al estado compacto.
3. Evitar intersecciones visuales que oculten elementos importantes.
4. Registrar la secuencia en la documentacion de UX/UI cuando se use como patron pedagogico.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `MOC_UX_UI_Complete`
- `Selection_System.md`
