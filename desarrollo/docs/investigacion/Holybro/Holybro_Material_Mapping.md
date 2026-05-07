# Mapeo Exhaustivo de Materiales (Holybro X500 V2)

Este documento contiene la lista categorizada de materiales físicos presentes en el dron Holybro X500 V2, basada estrictamente en la auditoría de activos (`AUDIT_EXHAUSTIVO_INFO_PANEL`) y los registros de geometría de Blender. 

Su propósito es servir como mapa técnico para el agrupamiento de UVs (Atlas) y la configuración de Shaders PBR en Substance Painter o Blender.

---

## 1. Materiales Físicos Base (Para PBR)

El modelo se divide en **7 perfiles de materiales físicos**:

1. **Fibra de Carbono Twill (2x2):** Material dieléctrico, reflectividad anisotrópica dependiente del ángulo. **Crítico en UV:** Requiere alineación ortogonal perfecta en los mapas UV para que el trenzado fluya sin cortes.
2. **Nylon Reforzado / Plástico Estructural (Azul y Negro):** Plástico mate, rugosidad media-alta. Soporta altas presiones.
3. **Aluminio Anodizado / CNC (Negro/Metalizado):** Metal mecanizado. Parámetro *Metallic* en 1.0, *Roughness* medio/alto para dar un acabado negro "difuso" que reacciona a la luz como metal, no como plástico.
4. **Acero Inoxidable y Acero Galvanizado:** Metales plateados reflectivos. *Metallic* 1.0, *Roughness* muy bajo.
5. **Latón / Cobre:** Metales tintados presentes en tuercas ciegas y embobinados internos de los motores.
6. **Silicona / Goma / Espuma EVA:** Absorbedores de impacto. 100% dieléctricos (*Metallic* 0.0), mate total (*Roughness* 1.0).
7. **Hardware Electrónico (PCB FR4, PVC, Termoplásticos):** Mezcla de dieléctricos (placas texturizadas) y conductores (soldaduras de estaño, conectores dorados).

---

## 2. Matriz de Asignación por Pieza

A continuación se detalla la asignación de materiales para cada pieza única (`BAKE_MASTERS_LOW`). Muchas piezas complejas requieren el enmascarado de varios materiales.

| Pieza Distinta (Categoría) | Nombre en Blender | Material Primario | Material Secundario | Material Terciario |
| :--- | :--- | :--- | :--- | :--- |
| **Plato Inferior** | `x500v2_bottom_plate` | Fibra de Carbono Twill | - | - |
| **Plato Superior** | `x500v2_platform_board` | Fibra de Carbono Twill | - | - |
| **Brazos (Tubos)** | `CARBON-FIBER-TUBE.001_low` | Fibra de Carbono Twill | - | - |
| **Tren Aterrizaje (Tubos)** | `CARBON-FIBER-TUBE300.001_low` | Fibra de Carbono Twill | - | - |
| **Placas Base Motor** | `BAN-DJ-DIAN-F2.001_low` | Fibra de Carbono Twill | - | - |
| **Base Inferior Central** | `HMX5V-DIGAI-DIANJIZUO-MUJU.001_low` | Fibra de Carbono Twill | - | - |
| **Abrazaderas Brazo** | `HMX5V-JIBI-JIA-MUJU.001_low` | Plástico Inyección (Azul) | - | - |
| **Soportes Estructurales** | `HMX5V-ZUO-DJ-MUJU.001_low` | Plástico Inyección (Negro/Azul)| - | - |
| **Abrazaderas Tubo** | `JIA-GUAN.001_low` | Plástico Inyección (Negro) | - | - |
| **Uniones Tren (Cruz)** | `JIA-LIANJIE.001_low` | Plástico Inyección (Negro) | - | - |
| **Conectores Tren** | `JIAO-LIANJIE.001_low` | Plástico Inyección (Negro) | - | - |
| **Tapones de Tubos** | `GUAN-CHENG.001_low` | Plástico Inyección (Negro) | - | - |
| **Mástil Cámara/GPS** | `ZHIJIA-CAMERA-INTEL.001_low` | Plástico Inyección (Negro) | - | - |
| **Motores 2216** | `DJ-2216-KV880.001_low` | Aluminio Anodizado Negro | Latón / Cobre (Bobinas) | Acero Inox (Eje) |
| **Controlador de Vuelo** | `x500v2_pixhawk6c` | Aluminio Anodizado Negro | Plástico Inyección | PCB FR4 |
| **Tornillería (Socket Cap)** | `GB70-M...` (Varios tamaños) | Acero Inoxidable | - | - |
| **Tornillería (Countersunk)**| `M25-6-CHEN...`, `M3-16-CHEN...`| Acero Inoxidable | - | - |
| **Tornillería (Pan Head)** | `M3-10-PAN...`, `M3-14-PAN...` | Acero Inoxidable | - | - |
| **Tuercas (Flange, Cap)** | `ZSLM-M3-FALAN...`, `ZSLM-M25...` | Acero Galvanizado | - | - |
| **Tuerca Nyloc** | `LM-M3-NILONG.001_low` | Acero Galvanizado | Nylon (Anillo Azul/Blanco)| - |
| **Tuercas Ciegas** | `LM-M3-DING...`, `ZSLM-M3-DING...`| Latón | - | - |
| **Separadores (Standoffs)** | `NILONGZHU-M...` (M2.5 y M3) | Nylon Reforzado (Negro Mate) | - | - |
| **Antivibradores** | `HUAN-GUIJIAO.001_low` | Silicona (Negro Mate) | - | - |
| **Espuma Tren de Aterrizaje**| `JIAO-EVA.001_low` | Espuma EVA (Negro) | Pintura (Franjas Rojas) | - |
| **Tapones de Patas** | `PYLONS-X500.001_low` | Goma / Caucho | - | - |
| **Tapones Protectores** | `MAO-JIAO.001_low` | Goma / Caucho | - | - |
| **Módulo GPS M10** | `x500v2_gps_m10` | Plástico Brillante/Mate | Calcomanía (Logo Blanco) | PCB FR4 |
| **Placa de Distribución (PDB)**| `x500v2_pdb` | PCB FR4 | Plástico (Conectores XT60)| Estaño (Soldaduras) |
| **Telemetría** | `x500v2_telemetry_radio` | Plástico Rugoso | - | - |
| **Receptor RC** | `x500v2_rc_receiver` | Plástico Protector | PCB FR4 | - |
| **Módulo de Poder** | `x500v2_power_module` | PCB FR4 | Plástico / Goma | - |
| **Batería LiPo 4S** | `x500v2_battery` | PVC Termocontraíble | Goma (Cables de Carga) | Plástico (Conector) |
