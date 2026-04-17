---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_WEBGL
---

# MOC de Consolidacion WebGL

> Bloque D3 para stubs de pipeline, build, IL2CPP, browser compatibility y performance.

## Criterio

- Convertir en nota real si aporta una regla de build, compatibilidad o optimizacion.
- Mantener como alias tecnico si solo apunta a un archivo historico o legacy.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "WebGL") OR contains(file.name, "IL2CPP") OR contains(file.name, "WASM") OR contains(file.name, "Brotli") OR contains(file.name, "Browser") OR contains(file.name, "GPU") OR contains(file.name, "Frame_Time") OR contains(file.name, "Performance_Rendering") OR contains(file.name, "Optimize")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_003_Optimizacion_Brotli_WebGL]]
- [[STUB_066_IL2CPP_Backend_Config]]
- [[STUB_138_webgl_optimizer]]
- [[STUB_032_Browser_Compatibility_Test]]
- [[STUB_019_04_Arquitectura_Renderizado_URP]]
- [[STUB_030_Blueprint_Mode_Shader]]
- [[STUB_034_Color_Mapping_by_Category]]
- [[STUB_049_Estrategia_Tesis_WebGL]]
- [[STUB_080_Modeling_Tutorial_Comparison]]
- [[STUB_087_Overview_Mode_Spec]]
- [[STUB_093_Performance_Thermal_Render]]
- [[STUB_094_Performance_UI_Audit]]
- [[STUB_095_Portfolio_WebGL_Case_Study]]
- [[STUB_137_WASM_Memory_Limits]]
- [[STUB_140_Wireframe_Mode_Spec]]
- [[STUB_138_webgl_optimizer]]
- [[STUB_139_webgl_scanner]]
- [[STUB_141_Workload_Assessment_Methodology]]
- [[STUB_142_XRay_Mode_Implementation]]
- [[STUB_013_MOC_WebGL]]

