---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-087
aliases:
  - "Overview_Mode_Spec.md"
resumen: "Especificacion del modo overview para vista general y orientacion espacial"
---

# Overview Mode Spec

## Proposito

Define el modo overview como referencia general para ubicarse en el ensamblaje y entender su estructura global.

## Reglas de uso

- Debe ofrecer contexto sin saturar detalle.
- La transicion desde otros modos debe ser clara.
- El estado general debe mantenerse estable.
- Debe convivir con selection e isolate.

## Criterios

1. La vista general debe servir como punto de retorno.
2. No debe ocultar la estructura relevante.
3. La nota debe indicar como se diferencia de blueprint o wireframe.
4. Cualquier efecto visual debe ser facil de desactivar.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `Blueprint_Mode_Shader.md`
- `Wireframe_Mode_Spec.md`
