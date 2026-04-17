---
tipo: moc
fuente: obsidian
estado: activo
area: trazabilidad
fecha: 2026-04-16
trace_id: TRC-MOC-AUTO-MOC_CONSOLIDACION_DRONE
---

# MOC de Consolidacion Drone

> Bloque D3 para stubs de componentes, CAD, materiales, sensores y analisis del Holybro X500 V2.

## Criterio

- Convertir en nota real si el tema aporta una ficha util de componente o una justificacion tecnica.
- Mantener como alias tecnico si solo resuelve una referencia a una pieza o recurso externo.

## Consultas

```dataview
TABLE file.link as Stub, trace_id, estado
FROM "Cerebro_Digital/Wiki/Stubs_NoResueltas"
WHERE contains(file.name, "Drone") OR contains(file.name, "Holybro") OR contains(file.name, "Motor") OR contains(file.name, "Battery") OR contains(file.name, "Sensor") OR contains(file.name, "CAD") OR contains(file.name, "Propeller") OR contains(file.name, "Material") OR contains(file.name, "Part_List") OR contains(file.name, "Arm_")
SORT file.name ASC
```

## Referencias de arranque

- [[STUB_053_Flight_Controller_PCB]]
- [[STUB_062_Holybro_Official_Documentation]]
- [[STUB_086_Open_Source_Drone_Designs]]
- [[STUB_020_Academic_Drone_Papers]]
- [[STUB_053_Flight_Controller_PCB]]
- [[STUB_025_Arm_Material_Analysis]]
- [[STUB_028_Battery_Power_Analysis]]
- [[STUB_047_Empirical_Motor_Heat_Profile]]
- [[STUB_055_Frame_CAD_Specifications]]
- [[STUB_072_LOD_Optimization_Drone]]
- [[STUB_008_CAD_Import_Pipeline]]
- [[STUB_046_DronePartDataFixer]]
- [[STUB_063_Holybro_X500v2_Official_Specs]]
- [[STUB_081_Motor_ESC_Analysis]]
- [[STUB_108_Sensor_Calibration_Guide]]
- [[STUB_020_Academic_Drone_Papers]]
- [[STUB_031_Breakdown_Pipeline_CAD_a_WebGL]]
- [[STUB_070_Kitbash_vs_CAD_Analysis]]
- [[STUB_082_Motor_Specs_BLDC]]
- [[STUB_099_Propeller_Analysis]]
- [[STUB_075_Material_Library]]
- [[STUB_076_Material_Type_Classification]]
- [[STUB_077_Maya_Blender_CAD_Import]]
- [[STUB_088_Part_List_Database]]
- [[STUB_054_Flight_Controller_Specs]]
- [[STUB_058_GPS_Compass_Specs]]
- [[STUB_097_Power_Distribution_Board]]
- [[STUB_109_Sensor_Data_Format]]

