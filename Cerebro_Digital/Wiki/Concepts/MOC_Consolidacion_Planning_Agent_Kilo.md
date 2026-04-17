---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_PLANNING_AGENT_KILO
---

# MOC de Consolidacion Planning, Agent y Kilo

> Bloque D3 para stubs de planes internos, workflows, skills y utilitarios de control.

## Criterio

- Convertir en nota real si describe una regla operativa, un flujo o una capacidad reutilizable.
- Mantener como alias tecnico si solo enlaza una referencia de trabajo histÃ³rica.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "planning") OR contains(file.name, "agent") OR contains(file.name, "kilo") OR contains(file.name, "workflow") OR contains(file.name, "SKILL")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_005_2026-04-16]]
- [[STUB_022_Algo]]
- [[STUB_127_Titulo_X]]
- [[STUB_051_Facilitator_Instructions]]
- [[STUB_144_YouTube_Assembly_Tutorials]]

