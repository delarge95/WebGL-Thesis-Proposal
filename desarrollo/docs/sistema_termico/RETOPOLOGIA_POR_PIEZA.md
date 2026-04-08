# Guía de retopología por pieza - Holybro X500 V2

## Propósito

Este documento le dice a un modelador junior exactamente cómo preparar la geometría del dron para que funcione bien con:

- el runtime WebGL,
- la lectura visual del producto,
- y la propagación térmica del sistema híbrido.

La regla principal es simple:

- no optimizar pensando en `ngons`,
- optimizar pensando en la triangulación final que Unity realmente va a renderizar.

Unity triangula igual cualquier `ngon`. Por eso el ahorro real no viene de dejar caras grandes ambiguas, sino de controlar la topología final, la densidad donde importa y la separación correcta entre piezas canónicas.

## Capa A - Reglas globales

### Reglas obligatorias de exportación

- No exportar `ngons` en la malla final.
- En piezas críticas usar malla `quad-dominant`.
- En piezas secundarias se permiten triángulos controlados.
- No fusionar fuentes térmicas canónicas con sus disipadores solo para ahorrar polígonos.
- Aplicar transforms antes de exportar.
- Mantener normales consistentes y malla manifold.
- Conservar grosor físico real cuando ese grosor afecta lectura térmica o de contacto.
- Pivotes coherentes con la función de la pieza.
- Escala final en metros, consistente con Unity.

### Naming

Las piezas que correspondan a nodos oficiales del solver deben conservar o mapear a estos IDs:

- `x500v2_bottom_plate`
- `x500v2_top_plate`
- `x500v2_arm_FL`, `x500v2_arm_FR`, `x500v2_arm_BL`, `x500v2_arm_BR`
- `x500v2_landing_gear`
- `x500v2_platform_board`
- `x500v2_rails_battery`
- `x500v2_pdb`
- `x500v2_power_module`
- `x500v2_pixhawk6c`
- `x500v2_gps_m10`
- `x500v2_telemetry_radio`
- `x500v2_motor_FL`, `x500v2_motor_FR`, `x500v2_motor_BL`, `x500v2_motor_BR`
- `x500v2_esc_FL`, `x500v2_esc_FR`, `x500v2_esc_BL`, `x500v2_esc_BR`
- `x500v2_prop_FL`, `x500v2_prop_FR`, `x500v2_prop_BL`, `x500v2_prop_BR`
- `x500v2_battery`
- `x500v2_rc_receiver`

### UVs

- Piezas críticas: UV limpio, sin overlaps, padding razonable.
- Piezas secundarias: UV limpio o simétrico, con overlaps solo si no afecta lectura.
- Piezas mínimas: atlas o auto-unwrap aceptable.

### Contacto térmico

Si una pieza transmite calor a otra, su zona de contacto debe ser clara en geometría y fácil de leer en bounds y malla. No hace falta microdetalle, pero sí:

- cara o banda de contacto definida,
- grosor razonable,
- loops cerca del anclaje,
- y pivote/orientación consistentes.

## Capa B - Topología por las 28 piezas canónicas

### Tabla maestra

| Pieza canónica | Rol térmico | Patrón visual esperado | Nivel topológico | Guía concreta |
| --- | --- | --- | --- | --- |
| `x500v2_bottom_plate` | Hub estructural y disipador pasivo | Uniform | Secundario alto | Plano con agujeros y grosor real. Quads limpios alrededor de agujeros, bordes y uniones con brazos/rails. No gastar densidad en zonas planas muertas. |
| `x500v2_top_plate` | Stack central pasivo | Uniform | Secundario alto | Igual criterio que bottom plate. Mantener limpia la zona donde apoyan Pixhawk, radio y platform board. |
| `x500v2_arm_*` x4 | Canal principal de conducción desde motor/ESC hacia el centro | Axial | Crítico | Quad-dominant. Al menos 6 segmentos longitudinales. Loops adicionales en extremo motor y extremo hub. No colapsar la longitud útil del tubo. |
| `x500v2_landing_gear` | Sumidero pasivo muy expuesto | Uniform | Secundario | Puede usar tris controlados. Mantener forma legible y puntos de contacto con bottom plate. |
| `x500v2_platform_board` | Plataforma pasiva | Uniform | Mínimo/secundario | Superficie limpia y ligera. No necesita densidad térmica alta. |
| `x500v2_rails_battery` | Puente de conducción entre bottom plate y batería | Axial | Secundario alto | Mantener continuidad longitudinal y puntos de apoyo definidos. Loops en zonas de apoyo con batería y fijación a la base. |
| `x500v2_pdb` | Fuente electrónica secundaria | Radial suave | Secundario alto | PCB plana con suficiente definición central. Mantener grosor realista y borde estable. |
| `x500v2_power_module` | Fuente electrónica secundaria | Radial suave | Secundario alto | Caja/PCB compacta. Subdividir ligeramente la cara donde van componentes. |
| `x500v2_pixhawk6c` | Fuente electrónica secundaria relevante | Radial suave | Secundario alto | Mantener tapa, base y volumen general limpios. Si se colapsa detalle, nunca perder el volumen principal. |
| `x500v2_gps_m10` | Térmicamente menor | Uniform | Mínimo | Se puede simplificar bastante. Si se conserva mástil, que sea limpio y ligero. |
| `x500v2_telemetry_radio` | Térmicamente menor | Uniform | Mínimo | Caja simple con malla baja y UV funcional. |
| `x500v2_motor_*` x4 | Fuente térmica primaria | Radial | Crítico | Quad-dominant. Mantener cilindro limpio, loops concéntricos y densidad extra cerca de carcasa y base de montaje. No usar `ngons` en tapas finales. |
| `x500v2_esc_*` x4 | Fuente térmica primaria/secundaria | Axial | Crítico | PCB o cuerpo rectangular limpio. Densidad media con loops paralelos al eje largo. Detalle extra solo donde se ubican MOSFETs o zona de fijación. |
| `x500v2_prop_*` x4 | Sin rol térmico relevante | Uniform | Mínimo | Muy low poly, silueta correcta y balance visual. No dedicar loops térmicos. |
| `x500v2_battery` | Fuente térmica primaria moderada | Radial | Crítico | Caja limpia con quads estables. Añadir 2-3 cortes longitudinales o transversales para que el gradiente no se vea plano. |
| `x500v2_rc_receiver` | Electrónica menor | Uniform | Mínimo | Caja simple. Puede heredar temperatura visual del stack central si hace falta. |

### Reglas por tier

#### Tier crítico

Aplica a:

- motores,
- ESC,
- batería,
- brazos.

Reglas:

- `quad-dominant`.
- Sin `ngons` finales.
- Loops concentrados en hotspots y contactos.
- UV limpio.
- No fusionar con el padre térmico.

#### Tier secundario alto

Aplica a:

- bottom plate,
- top plate,
- rails battery,
- PDB,
- power module,
- Pixhawk,
- landing gear.

Reglas:

- Quads preferidos, tris controlados aceptables.
- Mantener claros los volúmenes y superficies de apoyo.
- Reducir densidad en áreas muertas.

#### Tier mínimo

Aplica a:

- props,
- radio,
- receiver,
- GPS,
- platform board,
- fijaciones menores heredadas.

Reglas:

- Low poly agresivo.
- Tris aceptables.
- Sin obsesionarse con loops térmicos.
- Si una pieza es demasiado pequeña para justificar simulación propia, debe heredar temperatura del padre canónico.

## Capa C - Mapeo 55 CAD/Blender -> 28 piezas canónicas

### Mapeo principal por ensamblaje

| Nodo canónico | Subcomponentes CAD/Blender relacionados | Qué hacer en modelado | Dueño térmico |
| --- | --- | --- | --- |
| `x500v2_bottom_plate` | `BOTTOM-PLATE-X500-V5`, `X500-TAO-XT60`, clips y tornillería asociada a base | Mantener la plate como pieza limpia y separada. XT60 holder y fijaciones pueden quedar como hijos visuales low poly o heredar temperatura. | `x500v2_bottom_plate` |
| `x500v2_top_plate` | `TOP-PLATE-X500-V5`, fijaciones del stack superior | Mantener la plate separada. Tornillería y separadores pueden quedar en tier mínimo o heredar temperatura. | `x500v2_top_plate` |
| `x500v2_arm_*` | `CARBON-FIBER-TUBE300`, `HMX5V-GUAN-DINGWEI`, `HMX5V-JIBI-JIA-MUJU`, `HMX5V-ZUO-DJ-MUJU`, `HMX5V-DIGAI-DIANJIZUO-MUJU`, `BAN-DJ-DIAN-F2`, `JIA-GUAN` | El tubo principal debe quedar limpio y priorizado. Las abrazaderas, soportes y clamps pueden preservarse como piezas visuales separadas o fusionarse visualmente, pero el solver sigue viendo un brazo canónico. | `x500v2_arm_FL/FR/BL/BR` |
| `x500v2_landing_gear` | `CARBON-FIBER-TUBE`, `JIAO-EVA`, `JIAO-LIANJIE`, `MAO-JIAO` | Mantener legibilidad del conjunto, pero con malla ligera. Almohadillas y tapas pueden heredar temperatura. | `x500v2_landing_gear` |
| `x500v2_platform_board` | `PLATFORM-PLAT-X500` | Puede ser una pieza simple y ligera. | `x500v2_platform_board` |
| `x500v2_rails_battery` | `BATTERY-MOUNTING-PLAT`, `BATTERY-PAD`, `PYLONS-X500`, `GUAN-CHENG` | Mantener la superficie de apoyo y las barras/rieles legibles. El pad puede ser visualmente separado pero térmicamente heredado si hace falta. | `x500v2_rails_battery` |
| `x500v2_pixhawk6c` | `DIKE-PIXHAWK6C-LV-C1`, `MIANKE-PIXHAWK6C-LV-C1`, `IMU-PIXHAWK6C`, `PCB-PIXHAWK6C-F1`, `BM06B-WO` | Mantener el volumen principal claro. Conectores pequeños y detalle interno pueden heredar. Si el showcase lo requiere, shell y PCB pueden ser submallas dentro del mismo nodo canónico. | `x500v2_pixhawk6c` |
| `x500v2_power_module` | `PCB-PM06`, `TOU-XT60H-M-14AWG` | Mantener la PCB o carcasa principal separada. El conector puede quedar como hijo visual low poly. | `x500v2_power_module` |
| `x500v2_gps_m10` | `GAN-GPSV5-ZHIJIA`, `GPS-ZHIJIA-ZHUANJIETOU`, `GPS-ZHIJIA-ZUO`, `GPSV5-ZHIJIA-LUOMAO`, `GPSV5-ZHIJIA-TUOPAN` | La antena y el mástil pueden quedar como ensamblaje visual. Para el solver sigue siendo un nodo GPS único. | `x500v2_gps_m10` |
| `x500v2_motor_*` | `DJ-2216-KV880` | Mantener el motor como pieza separada sí o sí. | `x500v2_motor_FL/FR/BL/BR` |
| Fijaciones menores heredadas | `GB70-*`, `LM-*`, `M25-*`, `M3-*`, `NILONGZHU-*`, `ZSLM-*`, `JIA-LIANJIE`, `GAI-GUANGLIU`, `ZHIJIA-CAMERA-INTEL` | No crear nodos térmicos propios en V1. Dejar low poly o integrarlos visualmente al ensamblaje padre. | Padre canónico más cercano |

### Subcomponentes sin correspondencia completa en el JSON sincronizado de 55

En el dataset CAD/Blender actual no aparecen claramente representados como piezas dedicadas:

- `x500v2_pdb`
- `x500v2_esc_*`
- `x500v2_prop_*`
- `x500v2_battery`
- `x500v2_telemetry_radio`
- `x500v2_rc_receiver`

Instrucción para modelado:

- estas piezas siguen siendo nodos oficiales del solver,
- deben mantenerse o autorizarse como piezas separadas en la escena final,
- no deben perderse por falta de correspondencia en el lote CAD sincronizado actual.

## Capa D - Checklist de entrega para modelador

Antes de dar una pieza por aprobada, revisar:

- ¿La pieza conserva el `partId` canónico o un nombre mapeable?
- ¿La pieza crítica quedó sin `ngons` finales?
- ¿La densidad está donde nace o viaja el calor y no en superficies muertas?
- ¿Los contactos físicos importantes se leen bien en malla y bounds?
- ¿El pivote es coherente con montaje, animación o lectura técnica?
- ¿Las transforms están aplicadas?
- ¿La escala está correcta para Unity?
- ¿La malla es manifold y tiene normales sanas?
- ¿El UV es limpio si la pieza es crítica o secundaria alta?
- ¿La pieza debe llevar `ThermalSurfaceProfile` manual o basta el preset canónico?
- ¿La pieza participa en el grafo de contactos o solo hereda temperatura?

## Decisión final para el modelador

Si dudas entre una malla con `ngons` y una malla limpia en quads/tris controlados, elige siempre la segunda. En este proyecto importa más una triangulación predecible y una topología legible para el gradiente térmico que un ahorro falso de polígonos basado en `ngons`.