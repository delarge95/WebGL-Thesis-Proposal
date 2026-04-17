---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_TESTING_VALIDATION
---

# MOC de Consolidacion Testing y Validacion

> Bloque D3 para cuestionarios, protocolos, reportes de pruebas y analisis de usuarios.

## Criterio

- Convertir en nota real si define protocolo, instrumento, analisis o evidencia de validacion.
- Mantener como alias tecnico si solo apunta a una version historica.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "SUS") OR contains(file.name, "NASA") OR contains(file.name, "Validation") OR contains(file.name, "Validacion") OR contains(file.name, "Usability") OR contains(file.name, "Testing") OR contains(file.name, "Audit") OR contains(file.name, "Questionnaire") OR contains(file.name, "Heuristic") OR contains(file.name, "Think")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_011_Informe_final-AUDITORIA_PRIMERA_PASADA_INFORME_2026-04-15]]
- [[STUB_012_Informe_final-PROMPT_AUDITORIA_DEEP_RESEARCH_INFORME_FINAL]]
- [[STUB_021_ACCESSIBILITY_AUDIT]]
- [[STUB_051_Facilitator_Instructions]]
- [[STUB_014_Observation_Checklist]]
- [[STUB_061_Heuristic_Evaluation_Details]]
- [[STUB_130_Usability_Issues_Log]]
- [[STUB_131_Usability_Metrics_Report]]
- [[STUB_083_NASA_TLX_Heatmap]]
- [[STUB_113_SUS_Benchmark_Data]]
- [[STUB_118_Testing_Session_Log]]
- [[STUB_133_User_Testing_Summary_Report]]
- [[STUB_134_Validacion_Results_Analysis]]
- [[STUB_090_Performance_Metrics_Summary]]
- [[STUB_091_Performance_Optimization_Iterations]]
- [[STUB_092_Performance_Rendering_Tests]]
- [[STUB_116_Task_Timing_Analysis]]
- [[STUB_129_Usability_Issue_Clustering]]
- [[STUB_143_XRay_Performance_Analysis]]
- [[STUB_024_API_STRUCTURE_AUDIT]]
- [[STUB_035_Combined_Validation_Report]]
- [[STUB_037_Component_Isolation_Audit]]
- [[STUB_042_Cross_Validation_with_ANSYS]]
- [[STUB_059_GPU_Optimization_Opportunities]]
- [[STUB_112_Statistical_Significance_Testing]]
- [[STUB_114_SUS_Score_Analysis]]
- [[STUB_115_SUS_Scoring_Guide]]
- [[STUB_084_NASA_TLX_Scoring_Guide]]

