---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-073
aliases:
  - "MAPEO_TERMICO_PIEZA_PROPIEDADES.md"
resumen: "Mapeo entre piezas y propiedades termicas para alimentar el modelo y la lectura"
---

# Mapeo Termico Pieza Propiedades

## Proposito

Relaciona piezas concretas con sus propiedades termicas para alimentar el solver y facilitar diagnstico por componente.

## Datos utiles

- Identificador de pieza.
- Material y masa.
- Conductividad y capacidad termica.
- Relacion con condiciones de contacto.

## Criterios

1. El mapeo debe ser trazable a una pieza real.
2. No duplicar propiedades sin justificacion.
3. Debe ser util para el solver y para la documentacion.
4. Cualquier valor derivado debe quedar marcado como tal.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Thermal_Properties_by_Component.md`
- `Thermal_Contact_Model.md`
