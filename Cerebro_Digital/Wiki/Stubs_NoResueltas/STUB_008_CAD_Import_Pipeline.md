---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-008
aliases:
  - "CAD_Import_Pipeline.md"
resumen: "Pipeline de importacion CAD para preservar escala, mallas y estructura del modelo"
---

# CAD Import Pipeline

## Proposito

Describe el flujo de importacion del CAD para conservar escala, jerarquia, materiales y compatibilidad con el resto del pipeline.

## Etapas utiles

- Limpieza y normalizacion del archivo origen.
- Reasignacion de nombres y jerarquias.
- Validacion de escala y unidades.
- Preparacion para uso en visualizacion o ensamblaje.

## Criterios

1. El pipeline debe ser repetible con el mismo resultado.
2. Debe conservarse la trazabilidad hacia el CAD original.
3. Cualquier simplificacion debe ser explicitada.
4. La importacion debe respetar el uso final del modelo.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `Frame_CAD_Specifications.md`
- `Maya_Blender_CAD_Import.md`
