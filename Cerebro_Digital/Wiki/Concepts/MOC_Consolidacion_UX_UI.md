---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_UX_UI
---

# MOC de Consolidacion UX/UI

> Bloque D3 para stubs de interfaz, interaccion, validacion y accesibilidad.

## Criterio

- Convertir en nota real si describe una funcion, patron o prueba reutilizable.
- Mantener como alias tecnico si solo resuelve un enlace legado.
- Fusionar si ya existe otra nota equivalente en `MOC_UX_UI_Complete`.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "UI") OR contains(file.name, "UX") OR contains(file.name, "Selection") OR contains(file.name, "Viewport") OR contains(file.name, "XRay") OR contains(file.name, "Ghosted") OR contains(file.name, "Wireframe") OR contains(file.name, "Animation") OR contains(file.name, "Inspect") OR contains(file.name, "Cut_Plane") OR contains(file.name, "DataBinding") OR contains(file.name, "ProceduralPins")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_021_ACCESSIBILITY_AUDIT]]
- [[STUB_056_Ghosted_Mode_Implementation]]
- [[STUB_106_Selection_System]]
- [[STUB_041_Cross_Section_Viewer]]
- [[STUB_050_Explode_View_Animation]]
- [[STUB_067_Inspect_Panel_Data_Model]]
- [[STUB_105_Selection_Highlight_Shader]]
- [[STUB_136_Viewport_Management]]
- [[STUB_040_Cross_Browser_Compatibility_Tests]]
- [[STUB_043_Cut_Plane_Tool]]
- [[STUB_044_Data_Model_Schema]]
- [[STUB_045_DataBinding_System]]
- [[STUB_052_Filter_by_Property]]
- [[STUB_068_Isolate_Rendering_Setup]]
- [[STUB_100_Property_Panel_Data_Flow]]
- [[STUB_101_Qualitative_Data_Coding]]
- [[STUB_102_Raw_Response_Data]]
- [[STUB_103_Recommendations_from_Qualitative]]
- [[STUB_107_Selection_System_Tests]]
- [[STUB_132_User_Journey_Mapping]]
- [[STUB_104_Reporte_LaTeX_Resultados]]
- [[STUB_110_SolidColor_Kategory_Mapper]]
- [[STUB_117_Technical_Artist_Breakdown]]
- [[STUB_135_ViewMode_Switching_Tests]]
- [[STUB_029_Blueprint_Environment]]
- [[STUB_036_Component_Category_System]]
- [[STUB_038_Component_Selection_Hierarchy]]
- [[STUB_064_Hybrid_Approach_Justification]]
- [[STUB_065_Icono_Procedural_Design]]
- [[STUB_069_Iteration_Recommendations]]
- [[STUB_074_MAPEO_UI_FIELD_BINDINGS]]
- [[STUB_078_Memory_Profile_Analysis]]
- [[STUB_089_Participant_Demographics]]
- [[STUB_023_Animation_System]]
- [[STUB_096_Power_Control_UI]]
- [[STUB_098_ProceduralPinsIcon]]

