---
tipo: nota_consolidada
area: ux_ui
estado: activo
trace_id: STUB-NR-056
aliases:
  - "Ghosted_Mode_Implementation.md"
resumen: "Implementacion del modo ghosted para visualizacion semitransparente con outline"
---

# Ghosted Mode Implementation

## Proposito

El modo ghosted muestra el ensamblaje con semitransparencia y contorno para conservar contexto mientras resalta la pieza seleccionada o una region de interes.

## Comportamiento esperado

- El objeto activo conserva visibilidad dominante.
- El resto del ensamblaje baja su opacidad sin perder silueta.
- El outline ayuda a distinguir limites y superposiciones.
- El modo debe coexistir con selection, isolate y inspect.

## Criterios de implementacion

1. Mantener contraste suficiente sobre fondos claros y oscuros.
2. Evitar sobrecarga visual cuando hay muchas piezas en pantalla.
3. Preservar respuesta estable entre cambios de modo.
4. Documentar cualquier dependencia de shader o blending en la ficha tecnica.

## Relacion con el resto del grafo

- `MOC_UX_UI_Complete`
- `MOC_Consolidacion_UX_UI`
- `Selection_System.md`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

