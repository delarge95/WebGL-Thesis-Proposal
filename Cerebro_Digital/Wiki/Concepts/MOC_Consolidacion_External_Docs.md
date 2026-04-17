---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_EXTERNAL_DOCS
---

# MOC de Consolidacion External Docs

> Bloque D3 para referencias externas, guias, papers y material documental de apoyo.

## Criterio

- Convertir en nota real si es una fuente externa que se usa en el marco teorico o tecnico.
- Mantener como alias tecnico si solo conduce a un recurso de lectura ya establecido.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "External") OR contains(file.name, "YouTube") OR contains(file.name, "Academic_Drone_Papers") OR contains(file.name, "UI_UX") OR contains(file.name, "GuÃ­a") OR contains(file.name, "GuÃƒ")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_001_External_docs-GuÃ­a_Completa_de_DiseÃ±o_UI_UX_para_Aplicaciones_MÃ³]]
- [[STUB_002_External_docs-ux_ui_reference-GuÃ­a_Completa_de_DiseÃ±o_UI_UX_para_Aplicaciones_MÃ³]]
- [[STUB_144_YouTube_Assembly_Tutorials]]
- [[STUB_020_Academic_Drone_Papers]]

