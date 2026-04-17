---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_OTROS
---

# MOC de Consolidacion Otros

> Bloque D3 residual para stubs de baja frecuencia o nomenclatura ambigua.

## Criterio

- Revisar caso por caso.
- Si el nombre es ambiguo, validar si corresponde a typo, archivo legado o concepto realmente nuevo.
- Mantener solo lo que tenga valor semantico claro.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE tipo = "nota_stub" OR tipo = "nota_consolidada"
SORT file.name ASC
```

## Regla

Todo lo que no entre en un dominio especÃ­fico debe decidirse con una de estas tres salidas:

1. Nota real.
2. Alias tÃ©cnico.
3. EliminaciÃ³n del stub tras corregir el enlace origen.

## Referencias de control

- [[STUB_006_archivo]]
- [[STUB_048_enlaces]]
- [[STUB_071_links]]
- [[STUB_027_Audio_Video_Artifacts]]
- [[STUB_085_NombreDeLaPagina]]

