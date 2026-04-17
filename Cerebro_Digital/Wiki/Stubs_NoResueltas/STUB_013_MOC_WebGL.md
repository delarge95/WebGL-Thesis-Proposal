---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-013
aliases:
  - "MOC_WebGL"
resumen: "Nodo puente para referencias legacy hacia el MOC consolidado de WebGL"
---

# MOC_WebGL

## Proposito

Este nodo mantiene compatibilidad con enlaces historicos que apuntan a MOC_WebGL y los redirige al bloque consolidado actual.

## Regla de mantenimiento

1. Mantener el alias para no romper el grafo legado.
2. Priorizar como referencia canonica a MOC_Consolidacion_WebGL.
3. Evitar crear un segundo MOC paralelo con el mismo alcance.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `MOC_Consolidacion_Stubs_NoResueltas`
