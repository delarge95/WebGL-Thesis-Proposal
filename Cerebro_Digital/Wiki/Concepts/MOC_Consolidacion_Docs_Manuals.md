---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_DOCS_MANUALS
---

# MOC de Consolidacion Docs y Manuales

> Bloque D3 para manual tecnico, manual de usuario y entregables documentales de soporte.

## Criterio

- Convertir en nota real si contiene procedimientos, guia de uso o instrucciones operativas.
- Mantener como alias tecnico si la nota solo referencia un archivo externo ya canÃ³nico.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "manual") OR contains(file.name, "MANUAL") OR contains(file.name, "DEPLOYMENT") OR contains(file.name, "ARCHITECTURE") OR contains(file.name, "WEBGL_BUILD_SETTINGS") OR contains(file.name, "WEBGL_OPTIMIZATION_MANUAL")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_003_Optimizacion_Brotli_WebGL]]
- [[STUB_057_GitHub_Pages_Deployment]]
- [[STUB_128_UIToolkit_Architecture]]
- [[STUB_015_PLAN_REMEDIACION_UX]]

