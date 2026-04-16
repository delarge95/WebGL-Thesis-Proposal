# AUDITORIA DE CIERRE DE CONGRUENCIA INFO PANEL (2026-04-15)

## Regla de Gobernanza Aplicada
- Cuando un mismo ID funciona como pieza madre y hotspot, el peso canonico se gobierna por la formula de pieza madre definida en la trazabilidad.
- Esto evita doble conteo entre vistas y mantiene consistencia en toda la UI con una sola masa por ID.

## Estado Final
- Ajustes de gobernanza aplicados: **3**
- Inconsistencias piezas madre (delta > 0.0005): **0**
- Inconsistencias hotspots (delta > 0.0005): **0**
- Registros con peso <= 0: **6**
- Registros con campos criticos vacios: **0**

## Verificacion Piezas Madre
| parentId | parentName | esperadoKg | actualKg | deltaKg | hijos |
|---|---|---:|---:|---:|---:|
| x500v2_platform_board | Platform Board | 0.083000 | 0.083000 | 0.000000 | 34 |
| x500v2_rc_receiver | RC Receiver | 0.037000 | 0.037000 | 0.000000 | 16 |
| x500v2_power_module | Power Module PM02 V3 | 0.056000 | 0.056000 | 0.000000 | 14 |
| x500v2_landing_gear | Landing Gear | 0.104000 | 0.104000 | 0.000000 | 12 |
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.083000 | 0.083000 | 0.000000 | 10 |
| x500v2_motor_BL | Motor 2216 KV920 BL | 0.083000 | 0.083000 | 0.000000 | 10 |
| x500v2_motor_BR | Motor 2216 KV920 BR | 0.083000 | 0.083000 | 0.000000 | 10 |
| x500v2_motor_FR | Motor 2216 KV920 FR | 0.083000 | 0.083000 | 0.000000 | 10 |
| x500v2_arm_BR | Arm Tube Back-Right | 0.048000 | 0.048000 | 0.000000 | 9 |
| x500v2_bottom_plate | Carbon Fiber Bottom Plate | 0.093000 | 0.093000 | 0.000000 | 9 |
| x500v2_arm_FR | Arm Tube Front-Right | 0.046000 | 0.046000 | 0.000000 | 8 |
| x500v2_pdb | Power Distribution Board | 0.012000 | 0.012000 | 0.000000 | 6 |
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.008000 | 0.008000 | 0.000000 | 4 |
| x500v2_arm_BL | Arm Tube Back-Left | 0.036000 | 0.036000 | 0.000000 | 3 |
| x500v2_arm_FL | Arm Tube Front-Left | 0.036000 | 0.036000 | 0.000000 | 3 |
| x500v2_telemetry_radio | SiK Telemetry Radio V3 | 0.019000 | 0.019000 | 0.000000 | 2 |
| x500v2_esc_FL | ESC BLHeli-S 20A FL | 0.011000 | 0.011000 | 0.000000 | 2 |
| x500v2_esc_BL | ESC BLHeli-S 20A BL | 0.011000 | 0.011000 | 0.000000 | 2 |
| x500v2_esc_BR | ESC BLHeli-S 20A BR | 0.011000 | 0.011000 | 0.000000 | 2 |
| x500v2_esc_FR | ESC BLHeli-S 20A FR | 0.011000 | 0.011000 | 0.000000 | 2 |

## Verificacion Hotspots
| hotspotId | hotspotName | esperadoKg | actualKg | deltaKg | refs | regla |
|---|---|---:|---:|---:|---:|---|
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.008000 | 0.008000 | 0.000000 | 4 | governed_by_mother_formula |
| x500v2_blend_battery_mounting_plat | Battery Mounting Board | 0.025000 | 0.025000 | 0.000000 | 0 | self_weight |
| x500v2_blend_dike_pixhawk6c_lv_c1 | Pixhawk 6C Top Cover | 0.015000 | 0.015000 | 0.000000 | 0 | self_weight |
| x500v2_blend_dj_2216_kv880 | Holybro 2216 KV880 Motor | 0.063000 | 0.063000 | 0.000000 | 0 | self_weight |
| x500v2_blend_gan_gpsv5_zhijia | Holybro M10 GPS Antenna | 0.032000 | 0.032000 | 0.000000 | 0 | self_weight |
| x500v2_gps_m10 | Holybro M10 GPS Module | 0.032000 | 0.032000 | 0.000000 | 0 | self_weight |
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.083000 | 0.083000 | 0.000000 | 10 | governed_by_mother_formula |
| x500v2_pdb | Power Distribution Board | 0.012000 | 0.012000 | 0.000000 | 6 | governed_by_mother_formula |
| x500v2_pixhawk6c | Pixhawk 6C Autopilot | 0.051000 | 0.051000 | 0.000000 | 0 | self_weight |

## Registros con Peso <= 0
| id | partName | weightKg |
|---|---|---:|
| x500v2_fastener_group | Fasteners Group | 0.000000 |
| x500v2_misc_camera_mount | x500v2 misc camera mount | 0.000000 |
| x500v2_misc_frame_connector_0 | x500v2 misc frame connector 0 | 0.000000 |
| x500v2_misc_frame_connector_1 | x500v2 misc frame connector 1 | 0.000000 |
| x500v2_misc_group | Misc Group | 0.000000 |
| x500v2_misc_light_cover | x500v2 misc light cover | 0.000000 |
