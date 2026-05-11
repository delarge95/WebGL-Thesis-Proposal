# Auditoria UX de seleccion, piezas madre, hotspots y fasteners

Fecha: 2026-05-10

## Resumen ejecutivo

- La UX por capas ya tiene buena base, pero todavia necesita una fuente explicita de `subpieza -> fasteners` para no depender exclusivamente de contacto/bounds.
- El mapa propuesto `holybro_subpiece_fastener_map.json` asigna los 170 fasteners actuales a subpiezas prioritarias y relaciones compartidas, con confianza `high`, `medium` o `low`.
- Los hotspots principales ya son mucho mas coherentes, pero `Power Distribution` y `Propulsion System` todavia mezclan conceptos documentados con geometria ausente si no se comunica bien al usuario.
- Las piezas documentadas sin geometria final (`PDB`, `RC Receiver`, `ESC`) no deben aparecer como seleccionables ni como proxies visuales; deben quedar como metadata pendiente o requisitos de modelo.

## Mapa propuesto de fasteners por subpieza

Fuente detallada con IDs de instancia: `holybro_subpiece_fastener_map.json`.

| Pieza madre | Subpieza | Fasteners prioritarios | Fasteners compartidos/contextuales | Confianza | Observacion |
| --- | --- | --- | --- | --- | --- |
| x500v2_arm_BL | BAN-DJ-DIAN-F2 | CountersunkScrew M2.5x6 x4 | SocketCapScrew M2.5x12 x2<br>SocketCapScrew M2.5x6 x4 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BL | CARBON-FIBER-TUBE300 | - | FlangeNut M3 x4<br>RubberGrommet x3<br>SocketCapScrew M2.5x10 x2<br>SocketCapScrew M3x38 x4 | low | Compartido desde JIA-GUAN: M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_BL | HMX5V-DIGAI-DIANJIZUO-MUJU | SocketCapScrew M2.5x6 x4 | CountersunkScrew M2.5x6 x4<br>SocketCapScrew M2.5x12 x2 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BL | HMX5V-GUAN-DINGWEI | RubberGrommet x3 | - | low | Grommet en brazo inferido como stopper/dampener; requiere confirmacion. |
| x500v2_arm_BL | HMX5V-JIBI-JIA-MUJU | FlangeNut M3 x4<br>SocketCapScrew M3x38 x4 | - | medium | M3x38 largo asociado a abrazadera de brazo/frame y tubo. |
| x500v2_arm_BL | HMX5V-ZUO-DJ-MUJU | SocketCapScrew M2.5x12 x2 | CountersunkScrew M2.5x6 x4<br>SocketCapScrew M2.5x6 x4 | medium | M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BL | JIA-GUAN | SocketCapScrew M2.5x10 x2 | - | low | M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_BR | BAN-DJ-DIAN-F2 | - | SocketCapScrew M2.5x12 x3<br>SocketCapScrew M2.5x6 x4 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BR | BOTTOM-PLATE-X500-V5 | - | SocketCapScrew M3x25 x1<br>SocketCapScrew M3x8 x4 | low | Compartido desde HMX5V-JIBI-JIA-MUJU: M3x25 en brazo es anomalo por cantidad; revisar posicion exacta. |
| x500v2_arm_BR | CARBON-FIBER-TUBE300 | - | CapNut M3 x1<br>FlangeNut M3 x4<br>RubberGrommet x1<br>SocketCapScrew M2.5x10 x2<br>SocketCapScrew M3x38 x4 | low | Compartido desde JIA-GUAN: M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_BR | HMX5V-DIGAI-DIANJIZUO-MUJU | SocketCapScrew M2.5x6 x4 | SocketCapScrew M2.5x12 x3 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BR | HMX5V-GUAN-DINGWEI | RubberGrommet x1 | - | low | Grommet en brazo inferido como stopper/dampener; requiere confirmacion. |
| x500v2_arm_BR | HMX5V-JIBI-JIA-MUJU | CapNut M3 x1<br>FlangeNut M3 x4<br>SocketCapScrew M3x25 x1<br>SocketCapScrew M3x38 x4<br>SocketCapScrew M3x8 x4 | - | low | M3x25 en brazo es anomalo por cantidad; revisar posicion exacta. |
| x500v2_arm_BR | HMX5V-ZUO-DJ-MUJU | SocketCapScrew M2.5x12 x3 | SocketCapScrew M2.5x6 x4 | medium | M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_BR | JIA-GUAN | SocketCapScrew M2.5x10 x2 | - | low | M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_BR | TOP-PLATE-X500-V5 | - | SocketCapScrew M3x8 x4 | low | Compartido desde HMX5V-JIBI-JIA-MUJU: M3x8 en brazo puede pertenecer a interfaz con placas; revisar posicion exacta. |
| x500v2_arm_FL | BAN-DJ-DIAN-F2 | CountersunkScrew M2.5x6 x4 | SocketCapScrew M2.5x12 x2<br>SocketCapScrew M2.5x6 x4 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FL | CARBON-FIBER-TUBE300 | - | FlangeNut M3 x8<br>RubberGrommet x3<br>SocketCapScrew M2.5x10 x2<br>SocketCapScrew M3x38 x8 | low | Compartido desde JIA-GUAN: M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_FL | HMX5V-DIGAI-DIANJIZUO-MUJU | SocketCapScrew M2.5x6 x4 | CountersunkScrew M2.5x6 x4<br>SocketCapScrew M2.5x12 x2 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FL | HMX5V-GUAN-DINGWEI | RubberGrommet x3 | - | low | Grommet en brazo inferido como stopper/dampener; requiere confirmacion. |
| x500v2_arm_FL | HMX5V-JIBI-JIA-MUJU | FlangeNut M3 x8<br>SocketCapScrew M3x38 x8 | - | medium | M3x38 largo asociado a abrazadera de brazo/frame y tubo. |
| x500v2_arm_FL | HMX5V-ZUO-DJ-MUJU | SocketCapScrew M2.5x12 x2 | CountersunkScrew M2.5x6 x4<br>SocketCapScrew M2.5x6 x4 | medium | M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FL | JIA-GUAN | SocketCapScrew M2.5x10 x2 | - | low | M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_FR | BAN-DJ-DIAN-F2 | - | SocketCapScrew M2.5x12 x2<br>SocketCapScrew M2.5x6 x4 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FR | CARBON-FIBER-TUBE300 | - | RubberGrommet x1<br>SocketCapScrew M2.5x10 x2 | low | Compartido desde JIA-GUAN: M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_arm_FR | HMX5V-DIGAI-DIANJIZUO-MUJU | SocketCapScrew M2.5x6 x4 | SocketCapScrew M2.5x12 x2 | medium | Compartido desde HMX5V-ZUO-DJ-MUJU: M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FR | HMX5V-GUAN-DINGWEI | RubberGrommet x1 | - | low | Grommet en brazo inferido como stopper/dampener; requiere confirmacion. |
| x500v2_arm_FR | HMX5V-JIBI-JIA-MUJU | - | - | high | - |
| x500v2_arm_FR | HMX5V-ZUO-DJ-MUJU | SocketCapScrew M2.5x12 x2 | SocketCapScrew M2.5x6 x4 | medium | M2.5x12 asociado al conjunto superior/inferior del motor mount. |
| x500v2_arm_FR | JIA-GUAN | SocketCapScrew M2.5x10 x2 | - | low | M2.5x10 queda probablemente ligado a clamp/cable/ESC ausente; requiere confirmacion visual. |
| x500v2_battery | BATTERY | - | - | high | - |
| x500v2_battery | BATTERY-STRAP | - | - | high | - |
| x500v2_bottom_plate | BOTTOM-PLATE-X500-V5 | - | CountersunkScrew M3x16 x2<br>NylocNut M3 x2<br>SocketCapScrew M2.5x6 x8 | low | Compartido desde GAI-GUANGLIU: M2.5x6 probablemente fija cubierta/sensor inferior; confirmar contra posicion. |
| x500v2_bottom_plate | GAI-GUANGLIU | SocketCapScrew M2.5x6 x8 | - | low | M2.5x6 probablemente fija cubierta/sensor inferior; confirmar contra posicion. |
| x500v2_bottom_plate | ZHIJIA-CAMERA-INTEL | CountersunkScrew M3x16 x2<br>NylocNut M3 x2 | - | medium | M3x16/nyloc asignados al bracket inferior de camara; revisar con posicion. |
| x500v2_esc_BL | ESC | - | - | high | - |
| x500v2_esc_BR | ESC | - | - | high | - |
| x500v2_esc_FL | ESC | - | - | high | - |
| x500v2_esc_FR | ESC | - | - | high | - |
| x500v2_gps_m10 | GAN-GPSV5-ZHIJIA | - | - | high | - |
| x500v2_gps_m10 | GPSV5-ZHIJIA-TUOPAN | - | PanHeadScrew M3x10 x4 | medium | Compartido desde GPS-ZHIJIA-ZUO: Pan head M3x10 asignado al soporte/base del mastil GPS. |
| x500v2_gps_m10 | GPS-ZHIJIA-ZHUANJIETOU | CapNut M3 x5 | - | medium | Tuercas M3 asociadas al mastil/articulacion GPS; confirmar contra posicion final. |
| x500v2_gps_m10 | GPS-ZHIJIA-ZUO | PanHeadScrew M3x10 x4 | CapNut M3 x5 | medium | Compartido desde GPS-ZHIJIA-ZHUANJIETOU: Tuercas M3 asociadas al mastil/articulacion GPS; confirmar contra posicion final. |
| x500v2_landing_gear | CARBON-FIBER-TUBE | - | CapNut M3 x6<br>SocketCapScrew M3x21 x2<br>SocketCapScrew M3x8 x4 | medium | Compartido desde MAO-JIAO: Tornillos largos/tuercas asociados a conector T del tren y tubo de carbono. |
| x500v2_landing_gear | JIAO-EVA | - | SocketCapScrew M3x8 x4 | medium | Compartido desde JIAO-LIANJIE: Tornillos M3x8 asociados al extremo/patines del tren. |
| x500v2_landing_gear | JIAO-LIANJIE | SocketCapScrew M3x8 x4 | - | medium | Tornillos M3x8 asociados al extremo/patines del tren. |
| x500v2_landing_gear | MAO-JIAO | CapNut M3 x6<br>SocketCapScrew M3x21 x2 | - | medium | Tornillos largos/tuercas asociados a conector T del tren y tubo de carbono. |
| x500v2_motor_BL | DJ-2216-KV880 | SocketCapScrew M3x6 x4 | - | high | Motor tiene una subpieza mecanica clara y tornillos M3x6 de fijacion directa. |
| x500v2_motor_BR | DJ-2216-KV880 | SocketCapScrew M3x6 x4 | - | high | Motor tiene una subpieza mecanica clara y tornillos M3x6 de fijacion directa. |
| x500v2_motor_FL | DJ-2216-KV880 | SocketCapScrew M3x6 x4 | - | high | Motor tiene una subpieza mecanica clara y tornillos M3x6 de fijacion directa. |
| x500v2_motor_FR | DJ-2216-KV880 | SocketCapScrew M3x6 x4 | - | high | Motor tiene una subpieza mecanica clara y tornillos M3x6 de fijacion directa. |
| x500v2_pdb | PDB | - | - | high | - |
| x500v2_pixhawk6c | BM06B-WO | - | - | high | - |
| x500v2_pixhawk6c | DIKE-PIXHAWK6C-LV-C1 | - | - | high | - |
| x500v2_pixhawk6c | IMU-PIXHAWK6C | - | - | high | - |
| x500v2_pixhawk6c | MIANKE-PIXHAWK6C-LV-C1 | - | - | high | - |
| x500v2_pixhawk6c | PCB-PIXHAWK6C-F1 | - | - | high | - |
| x500v2_platform_board | PLATFORM-PLAT-X500 | CapNut M2.5 x4<br>SocketCapScrew M2.5x12 x4<br>Standoff M2.5x5 x4 | - | high | La pieza madre tiene una unica subpieza de board/plataforma. |
| x500v2_power_module | PCB-PM06 | CapNut M3 x5<br>PanHeadScrew M3x14 x4<br>Standoff M3x5 x4 | SocketCapScrew M3x25 x1<br>SocketCapScrew M3x8 x4 | low | Compartido desde X500-TAO-XT60: Fastener de soporte/holder XT60 inferido por familia; requiere confirmar contra posicion en Blender. |
| x500v2_power_module | TOU-XT60H-M-14AWG | - | SocketCapScrew M3x25 x1<br>SocketCapScrew M3x8 x4 | low | Compartido desde X500-TAO-XT60: Fastener de soporte/holder XT60 inferido por familia; requiere confirmar contra posicion en Blender. |
| x500v2_power_module | X500-TAO-XT60 | SocketCapScrew M3x25 x1<br>SocketCapScrew M3x8 x4 | CapNut M3 x5<br>PanHeadScrew M3x14 x4<br>Standoff M3x5 x4 | low | Fastener de soporte/holder XT60 inferido por familia; requiere confirmar contra posicion en Blender. |
| x500v2_prop_BL | PROP | - | - | high | - |
| x500v2_prop_BL | PROPELLER | - | - | high | - |
| x500v2_prop_BR | PROP | - | - | high | - |
| x500v2_prop_BR | PROPELLER | - | - | high | - |
| x500v2_prop_FL | PROP | - | - | high | - |
| x500v2_prop_FL | PROPELLER | - | - | high | - |
| x500v2_prop_FR | PROP | - | - | high | - |
| x500v2_prop_FR | PROPELLER | - | - | high | - |
| x500v2_rails_battery | BATTERY-MOUNTING-PLAT | CountersunkScrew M2.5x6 x4 | - | medium | Countersunk M2.5x6 asignados al board de montaje; comparten interfaz con railes/clips. |
| x500v2_rails_battery | BATTERY-PAD | - | - | high | - |
| x500v2_rails_battery | GUAN-CHENG | - | CountersunkScrew M2.5x6 x4 | medium | Compartido desde BATTERY-MOUNTING-PLAT: Countersunk M2.5x6 asignados al board de montaje; comparten interfaz con railes/clips. |
| x500v2_rails_battery | PYLONS-X500 | - | CountersunkScrew M2.5x6 x4 | medium | Compartido desde BATTERY-MOUNTING-PLAT: Countersunk M2.5x6 asignados al board de montaje; comparten interfaz con railes/clips. |
| x500v2_rc_receiver | RECEIVER | - | - | high | - |
| x500v2_telemetry_radio | RADIO | - | - | high | - |
| x500v2_telemetry_radio | TELEMETRY | - | - | high | - |
| x500v2_top_plate | TOP-PLATE-X500-V5 | - | - | high | - |

## Hallazgos UX

### Prioridad alta

| Hallazgo | Impacto UX | Recomendacion |
| --- | --- | --- |
| La relacion fina `subpieza -> fasteners` no estaba serializada. | El usuario puede ver capas correctas por contacto, pero no hay trazabilidad academica ni control manual fino. | Usar `holybro_subpiece_fastener_map.json` como fuente explicita y dejar contacto/bounds solo como fallback/auditoria. |
| `Power Distribution` queda con `x500v2_power_module` pero sin PDB real. | El nombre promete mas de lo que se muestra si la PDB no existe en el FBX. | Mantener hotspot, pero mostrar estado `PDB documentada / geometria ausente` o renombrar temporalmente a `Power Module / XT60`. |
| `Propulsion System` es correcto como macro-hotspot, pero demasiado grande para aprendizaje fino. | Puede ser abrumador: 4 brazos + 4 motores + 4 helices + fasteners. | Agregar sub-hotspots o filtros por cuadrante: `Propulsion FL/FR/BL/BR`. |
| Piezas canonicas ausentes (`PDB`, `ESC`, `RC Receiver`) estaban en catalogo/runtime como si fueran piezas reales. | Genera piezas fantasma y desconfianza del usuario. | Mantenerlas documentadas, pero ocultas del runtime seleccionable hasta que Blender entregue geometria real. |

### Prioridad media

| Hallazgo | Impacto UX | Recomendacion |
| --- | --- | --- |
| Inconsistencia de nombres: `Power Module PM02 V3` vs `PM06/XT60`. | El usuario puede pensar que son componentes distintos. | Normalizar copy y metadata a la pieza real importada. |
| Inconsistencia de motores: metadata `KV920` vs Blender `KV880`. | Debilita confianza tecnica. | Confirmar modelo real y dejar un solo valor en UI/documentacion. |
| El hotspot `Battery` mezcla bateria y railes. | Correcto para montaje, pero confuso si el usuario solo quiere la bateria. | Mantener macro-hotspot, pero permitir foco interno `Battery` y `Rails / Mount`. |
| No existe hotspot dedicado a tren de aterrizaje. | Una zona visible y pedagogica queda escondida en filtros estructurales. | Crear hotspot `Landing Gear` con `x500v2_landing_gear` y sus fasteners. |
| Camara/optical flow viven bajo `bottom_plate`. | Correcto estructuralmente, pero invisibiliza sensores/payload. | Crear hotspot opcional `Vision / Payload Mounts` para `GAI-GUANGLIU` y `ZHIJIA-CAMERA-INTEL`. |
| Fasteners low-confidence en brazos (`M2.5x10`, grommets, M3x8/M3x25). | Pueden aparecer en subpieza incorrecta si se usa solo semantica. | Confirmar con posicion Blender/Unity y ajustar prioridad por instancia. |

### Lo que ya esta bien

| Aspecto | Estado |
| --- | --- |
| Hotspots sin keywords vagas | Bien: la configuracion ya no depende de palabras como `battery`, `mount`, `m3` o `fastener`. |
| `GUAN-CHENG` fuera de Power Distribution | Bien: pertenece a railes/bateria, no a la cadena electrica. |
| Fasteners como entidades modulares | Bien: se conservan como primitivos inspeccionables y reemplazables. |
| Historial de capas | Bien: permite volver por el mismo camino cuando hay fasteners compartidos. |

## Propuesta de hotspots refinados

| Hotspot recomendado | Piezas madre | Justificacion UX |
| --- | --- | --- |
| `Power Module / XT60` o `Power Distribution` con aviso de PDB ausente | `x500v2_power_module`; `x500v2_pdb` solo cuando exista geometria | Evita prometer una PDB visible cuando no esta en el FBX. |
| `Flight Controller` | `x500v2_pixhawk6c` | Zona clara, de alto valor pedagogico. |
| `GPS & Compass` | `x500v2_gps_m10` | Correcto; revisar tuercas/fasteners del mastil. |
| `Propulsion All` | brazos, motores, helices | Macro-vista para entender sistema completo. |
| `Propulsion FL/FR/BL/BR` | brazo + motor + helice del cuadrante | Mejor flujo de inspeccion y menor ruido visual. |
| `Battery & Rails` | `x500v2_battery`, `x500v2_rails_battery` | Correcto como macro-grupo de montaje. |
| `Landing Gear` | `x500v2_landing_gear` | Mejora lectura de ensamblaje y fasteners inferiores. |
| `Vision / Payload Mounts` | subpiezas `GAI-GUANGLIU`, `ZHIJIA-CAMERA-INTEL` dentro de bottom plate | Aporta narrativa tecnica para sensores/soportes sin romper pieza madre estructural. |

## Criterio de cierre

- Antes de conectar el mapa al runtime, confirmar las filas `low` contra posicion real en Blender/Unity.
- Una vez confirmado, Unity debe usar prioridad por subpieza desde JSON y solo recurrir a bounds para fasteners no mapeados o nuevos.
- La seleccion de pieza madre debe incluir todos los fasteners prioritarios de sus subpiezas y los compartidos que entren por contexto/historial.
