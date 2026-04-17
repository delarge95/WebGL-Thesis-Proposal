---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-077
aliases:
  - "Maya_Blender_CAD_Import.md"
resumen: "Importacion CAD entre Maya y Blender para mantener escala, jerarquia y malla"
---

# Maya Blender CAD Import

## Proposito

Describe el flujo de importacion entre Maya y Blender para conservar unidades, transformaciones y estructura del modelo.

## Puntos clave

- Escala y orientacion.
- Limpieza de jerarquia.
- Nombres y materiales.
- Preparacion para exportacion posterior.

## Criterios

1. El intercambio debe evitar perdida de informacion util.
2. La nota debe señalar donde aparecen transformaciones o unidades problemáticas.
3. La importacion debe ser repetible.
4. Conviene dejar claro el software dominante en cada etapa.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `CAD_Import_Pipeline.md`
- `Frame_CAD_Specifications.md`
