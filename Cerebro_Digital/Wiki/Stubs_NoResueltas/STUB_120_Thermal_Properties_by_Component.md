---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-120
aliases:
  - "Thermal_Properties_by_Component.md"
resumen: "Propiedades termicas por componente para alimentar el modelo y la comparacion"
---

# Thermal Properties by Component

## Proposito

Agrupa propiedades termicas por componente para comparar piezas y mantener el modelo alineado con la realidad del ensamblaje.

## Campos utiles

- Nombre de componente.
- Material o tratamiento.
- Propiedades termicas relevantes.
- Relacion con la carga y el contacto.

## Criterios

1. Cada componente debe poder rastrearse en la simulacion.
2. Las propiedades deben ser consistentes con la fuente.
3. Debe poder compararse con otros componentes equivalentes.
4. La nota debe ser util para mantener y validar el modelo.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Thermal_Load_Profile.md`
- `Mapeo Termico Pieza Propiedades`
