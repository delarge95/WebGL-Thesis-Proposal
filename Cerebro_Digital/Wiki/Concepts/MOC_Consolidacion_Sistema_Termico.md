---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_SISTEMA_TERMICO
---

# MOC de Consolidacion Sistema Termico

> Bloque D3 para solver, validacion matematica, thermal rendering y logs del sistema termico.

## Criterio

- Convertir en nota real si describe ecuaciones, pruebas, estados o propiedades fisicas.
- Mantener como alias tecnico si solo referencia un artefacto de apoyo.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "Thermal") OR contains(file.name, "THERMAL") OR contains(file.name, "Heat") OR contains(file.name, "Solver") OR contains(file.name, "Boundary") OR contains(file.name, "Contact") OR contains(file.name, "Energy") OR contains(file.name, "Load_Profile") OR contains(file.name, "Convergence")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_111_Solver_Architecture]]
- [[STUB_123_Thermal_Solver_State_Machine]]
- [[STUB_124_THERMAL_SYSTEM_AUDIT_REPORT]]
- [[STUB_004_shader_custom_thermal]]
- [[STUB_016_Thermal_Colormap_Design]]
- [[STUB_007_Boundary_Conditions]]
- [[STUB_009_Energy_Conservation_Proof]]
- [[STUB_010_Frame_Time_Analysis]]
- [[STUB_017_THERMAL_CONSTANTS]]
- [[STUB_018_Thermal_Contact_Model]]
- [[STUB_026_Arquitectura_Termica_Dron]]
- [[STUB_033_Bug_Fixes_Thermal_Solver]]
- [[STUB_039_Convergence_Analysis]]
- [[STUB_060_GPU_Texture_Streaming_Thermal]]
- [[STUB_073_MAPEO_TERMICO_PIEZA_PROPIEDADES]]
- [[STUB_079_MOC_Flujo_Termico_Final]]
- [[STUB_119_Thermal_Load_Profile]]
- [[STUB_120_Thermal_Properties_by_Component]]
- [[STUB_121_Thermal_Regression_Tests]]
- [[STUB_122_Thermal_Simulation_Tests]]
- [[STUB_125_Thermal_System_Demo_Assets]]
- [[STUB_126_Thermal_System_Development_Log]]

