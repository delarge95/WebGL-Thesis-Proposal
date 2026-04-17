---
tipo: "moc"
fuente: "desarrollo/ANALISIS_RECURSOS_DRON.md | desarrollo/docs/investigacion/Holybro"
estado: "activo"
descripcion: "InvestigaciÃ³n completa del dron Holybro X500 V2: especificaciones, componentes, anÃ¡lisis de recursos, CAD, integraciÃ³n de datos"
area: trazabilidad
trace_id: TRC-MOC-AUTO-MOC_DRONE_RESEARCH
---

# Mapa de Contenido: InvestigaciÃ³n Drone Holybro X500 V2

> **PropÃ³sito**: Centralizar toda la investigaciÃ³n, anÃ¡lisis y especificaciones del dron Holybro X500 V2 que es el sujeto principal de visualizaciÃ³n en WebGL. Incluye especificaciones tÃ©cnicas, anÃ¡lisis de recursos, componentes CAD, mapeo de datos, y referencias a documentaciÃ³n externa.

**Cobertura**: 40+ investigaciones sobre drone, componentes, modelado, integraciÃ³n de datos UI.

---

## ðŸŽ¯ Hub Principal de InvestigaciÃ³n Drone

### EspecificaciÃ³n Base

- [[ANALISIS_RECURSOS_DRON.md]] â€” **Hub maestro**: AnÃ¡lisis comparativo de recursos, drone elegido, caracterÃ­sticas
- [[Investigacion_Holybro_X500v2]] â€” _Concepto Cerebro_Digital_: Resumen ejecutivo de especificaciones

### Mapeo de Componentes

- [[MAPEO_UI_FIELD_BINDINGS_2026-04-15.md]] â€” **Hub secundario**: 55+ componentes mapeados a UI fields

---

## ðŸ“‹ NÃºcleos TemÃ¡ticos

### 1ï¸âƒ£ Especificaciones TÃ©cnicas Base

Datos fundamentales del dron:

- [[ANALISIS_RECURSOS_DRON.md]] â€” QuÃ© es Holybro X500 V2, por quÃ© fue elegido
- [[Holybro_X500v2_Official_Specs.md]] â€” Especificaciones oficiales (peso, dimensiones, payload)
- [[Motor_ESC_Analysis.md]] â€” AnÃ¡lisis de motores brushless + ESCs (8 motores)
- [[Battery_Power_Analysis.md]] â€” Sistema de baterÃ­a, voltaje, capacidad, time to fly
- [[Flight_Controller_Specs.md]] â€” Flight controller integrado, sensores IMU
- [[GPS_Compass_Specs.md]] â€” GPS/GNSS module, compass, barometer

### 2ï¸âƒ£ AnatomÃ­a de Componentes (28+ Piezas)

Desglose de todas las partes fÃ­sicas:

- [[MAPEO_UI_FIELD_BINDINGS_2026-04-15.md]] â€” 55+ componentes con data bindings
- **Motores (8)**: Motor 1-4 top, Motor 1-4 bottom
  - [[Motor_Specs_BLDC.md]] â€” Especificaciones BLDC, KV, thrust curve
  - [[Propeller_Analysis.md]] â€” HÃ©lices, pitch, diameter, material
- **Frame (5)**: 4 arms + center hub
  - [[Arm_Material_Analysis.md]] â€” Material, espesor, resistencia
  - [[Frame_CAD_Specifications.md]] â€” Dimensiones, mounting points
- **PCBs (3)**: Flight Controller, ESC Board, Power Board
  - [[Flight_Controller_PCB.md]] â€” Layout, sensors, connectors
  - [[Power_Distribution_Board.md]] â€” PDM, voltage regulation, capacitors
- **Sensors (10+)**: GPS, Compass, Baro, IMU, etc.
  - [[Sensor_Calibration_Guide.md]] â€” Procedimiento de calibraciÃ³n
  - [[Sensor_Data_Format.md]] â€” Protocolo de datos de sensores
- **Payload (5+)**: Camera, Gimbal, SD card slot, payload mount
- **Cables & Connectors**: Power, signal, data buses

### 3ï¸âƒ£ InvestigaciÃ³n Comparativa (Recursos)

AnÃ¡lisis de opciones durante diseÃ±o:

- [[ANALISIS_RECURSOS_DRON.md#Recursos_Analizados]] â€” Tabla comparativa de modelos
  - Parrot ANAFI (oficial, solo carcasa)
  - GitHub km5es (F450, Tarot T960)
  - Sketchfab T-FLEX (CAD)
  - GrabCAD Compact (modular)
- [[Kitbash_vs_CAD_Analysis.md]] â€” AnÃ¡lisis: kitbash 3D vs CAD real
- [[Modeling_Tutorial_Comparison.md]] â€” ComparaciÃ³n de tutoriales disponibles
- [[Hybrid_Approach_Justification.md]] â€” Por quÃ© elegir enfoque hÃ­brido CAD + artÃ­stico

### 4ï¸âƒ£ Modelado CAD & ImportaciÃ³n

Proceso de bringing CAD into 3D engine:

- [[CAD_Import_Pipeline.md]] â€” Workflow: STEP â†’ Blender â†’ Retopo â†’ UV â†’ Import Unity
- [[RETOPOLOGIA_POR_PIEZA.md]] â€” _relaciÃ³n_: RetopologÃ­a detallada de las 28 piezas
- [[Maya_Blender_CAD_Import.md]] â€” Herramientas de importaciÃ³n (Import CAD Addon, Mayo)
- [[UV_Unwrapping_Strategy.md]] â€” Estrategia de UV mapping por categorÃ­a de pieza
- [[LOD_Optimization_Drone.md]] â€” Level of Detail strategy para 55+ componentes

### 5ï¸âƒ£ Data Binding: UI â†” Modelo 3D

CÃ³mo los datos fluyen en la app:

- [[MAPEO_UI_FIELD_BINDINGS_2026-04-15.md]] â€” 55+ mappings de componentes a UI
- [[Data_Model_Schema.md]] â€” Estructura JSON/XML de datos del drone
- [[Component_Selection_Hierarchy.md]] â€” JerarquÃ­a de selecciÃ³n: mother part â†’ subpiece
- [[Property_Panel_Data_Flow.md]] â€” QuÃ© informaciÃ³n mostrar por componente seleccionado
- [[Part_List_Database.md]] â€” Database of 55+ piezas con metadata

### 6ï¸âƒ£ CategorizaciÃ³n & Filtrado

Sistemas de organizaciÃ³n de componentes:

- [[Component_Category_System.md]] â€” CategorÃ­as: Motors, PCB, Frame, Sensors, Payload, Cables
- [[Material_Type_Classification.md]] â€” ClasificaciÃ³n por material: metal, plastic, composite, PCB
- [[Thermal_Properties_by_Component.md]] â€” Propiedades tÃ©rmicas de cada categorÃ­a
- [[Color_Mapping_by_Category.md]] â€” CÃ³digo de colores en SolidColor mode

### 7ï¸âƒ£ Referencias Externas

Documentos y tutoriales de terceros:

- [[Holybro_Official_Documentation.md]] â€” Links a documentaciÃ³n oficial
- [[YouTube_Assembly_Tutorials.md]] â€” Tutoriales de ensamblaje
- [[Academic_Drone_Papers.md]] â€” Papers acadÃ©micos sobre drones multirotor
- [[Open_Source_Drone_Designs.md]] â€” DiseÃ±os open-source de referencia

---

## ðŸ”§ Tabla de Especificaciones RÃ¡pida

| Aspecto                 | Valor                        | Referencia                            |
| ----------------------- | ---------------------------- | ------------------------------------- |
| **Plataforma**          | Holybro X500 V2              | ANALISIS_RECURSOS_DRON.md             |
| **Clase**               | Quadcopter multirotor        | Holybro_Official_Specs.md             |
| **Peso**                | ~2.3 kg (con baterÃ­a)        | Battery_Power_Analysis.md             |
| **Payload**             | ~3 kg                        | Holybro_X500v2_Official_Specs.md      |
| **Motores**             | 8x BLDC (quad config)        | Motor_Specs_BLDC.md                   |
| **Control**             | Flight controller integrado  | Flight_Controller_Specs.md            |
| **Sensores**            | IMU, GPS, Compass, Barometer | Sensor_Calibration_Guide.md           |
| **Componentes totales** | 55+ (piezas distintas)       | MAPEO_UI_FIELD_BINDINGS_2026-04-15.md |
| **Piezas canÃ³nicas**    | 28 (modeladas en Blender)    | RETOPOLOGIA_POR_PIEZA.md              |

---

## ðŸ”„ Relaciones Transversales

- **Conecta con**: [[MOC_Sistema_Termico_Completo]] â€” Especificaciones tÃ©rmicas de componentes
- **Conecta con**: [[MOC_UX_UI_Complete]] â€” 55 componentes mapeados a UI field bindings
- **Conecta con**: [[Pipeline_Modelado_Dron]] â€” Proceso CAD â†’ WebGL
- **Conecta con**: [[MOC_WebGL_Build_Pipeline]] â€” Assets del dron importados en WebGL
- **Referenciado por**: [[Investigacion_Holybro_X500v2]] â€” Concepto existente

---

## ðŸ“Š Estado de DocumentaciÃ³n

| Ãrea             | Cobertura   | Hub Principal                         |
| ---------------- | ----------- | ------------------------------------- |
| Especificaciones | âœ… Completo | ANALISIS_RECURSOS_DRON.md             |
| Componentes (55) | âœ… Completo | MAPEO_UI_FIELD_BINDINGS_2026-04-15.md |
| CAD Modeling     | âœ… Completo | CAD_Import_Pipeline.md                |
| Data Binding     | âœ… Completo | MAPEO_UI_FIELD_BINDINGS_2026-04-15.md |
| Specifications   | âœ… Completo | Holybro_Official_Specs.md             |
| Thermal Props    | âœ… Completo | Thermal_Properties_by_Component.md    |
| CategorizaciÃ³n   | âœ… Completo | Component_Category_System.md          |

---

## ðŸš€ Flujo de Lectura Recomendado

### Para Entender el Dron

1. ANALISIS_RECURSOS_DRON.md (visiÃ³n general)
2. Holybro_X500v2_Official_Specs.md (especificaciones)
3. MAPEO_UI_FIELD_BINDINGS (55 componentes)
4. Motor_Specs_BLDC.md + Flight_Controller_Specs.md

### Para Modelado

1. CAD_Import_Pipeline.md
2. RETOPOLOGIA_POR_PIEZA.md
3. UV_Unwrapping_Strategy.md
4. LOD_Optimization_Drone.md

### Para IntegraciÃ³n UI

1. MAPEO_UI_FIELD_BINDINGS_2026-04-15.md
2. Data_Model_Schema.md
3. Property_Panel_Data_Flow.md
4. Component_Selection_Hierarchy.md

---

## ðŸ› ï¸ Herramientas & Software

| Herramienta          | Rol                    | Docs                    |
| -------------------- | ---------------------- | ----------------------- |
| **Blender**          | Modelado CAD + Retopo  | CAD_Import_Pipeline     |
| **Maya** (alt)       | ImportaciÃ³n CAD        | Maya_Blender_CAD_Import |
| **Import CAD Addon** | Mayo import en Blender | CAD_Import_Pipeline     |
| **Unity**            | ImportaciÃ³n de assets  | LOD_Optimization_Drone  |
| **Figma** (opcional) | VisualizaciÃ³n de mapeo | MAPEO_UI_FIELD_BINDINGS |

---

## ðŸ“ Ãšltima ActualizaciÃ³n

Creado: 2026-04-16 (Orchestration AutÃ³noma - Fase 2)  
Archivos enlazados: ~40  
Componentes documentados: 55  
Piezas canÃ³nicas modeladas: 28
