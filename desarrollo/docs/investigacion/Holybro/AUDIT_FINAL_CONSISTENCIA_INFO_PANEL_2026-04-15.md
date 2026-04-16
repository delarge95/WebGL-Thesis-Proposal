# AUDITORIA FINAL DE CONSISTENCIA INFO PANEL (2026-04-15)

## Estado de Cierre
- Assets evaluados: **257**
- Inconsistencias masa piezas madre (>0.005 kg): **12**
- Inconsistencias masa hotspots (>0.005 kg): **0**
- Registros con peso <= 0: **6**
- Registros con campos criticos vacios: **0**

## Verificacion Piezas Madre
| parentId | parentName | esperadoKg | actualKg | deltaKg | hijos |
|---|---|---:|---:|---:|---:|
| x500v2_platform_board | Platform Board | 0.117000 | 0.083000 | 0.034000 | 34 |
| x500v2_rc_receiver | RC Receiver | 0.053000 | 0.037000 | 0.016000 | 16 |
| x500v2_power_module | Power Module PM02 V3 | 0.070000 | 0.056000 | 0.014000 | 14 |
| x500v2_landing_gear | Landing Gear | 0.116000 | 0.104000 | 0.012000 | 12 |
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.083000 | 0.073000 | 0.010000 | 10 |
| x500v2_motor_BL | Motor 2216 KV920 BL | 0.093000 | 0.083000 | 0.010000 | 10 |
| x500v2_motor_BR | Motor 2216 KV920 BR | 0.093000 | 0.083000 | 0.010000 | 10 |
| x500v2_motor_FR | Motor 2216 KV920 FR | 0.093000 | 0.083000 | 0.010000 | 10 |
| x500v2_bottom_plate | Carbon Fiber Bottom Plate | 0.102000 | 0.093000 | 0.009000 | 9 |
| x500v2_arm_BR | Arm Tube Back-Right | 0.057000 | 0.048000 | 0.009000 | 9 |
| x500v2_arm_FR | Arm Tube Front-Right | 0.054000 | 0.046000 | 0.008000 | 8 |
| x500v2_pdb | Power Distribution Board | 0.012000 | 0.006000 | 0.006000 | 6 |
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.008000 | 0.004000 | 0.004000 | 4 |
| x500v2_arm_BL | Arm Tube Back-Left | 0.039000 | 0.036000 | 0.003000 | 3 |
| x500v2_arm_FL | Arm Tube Front-Left | 0.039000 | 0.036000 | 0.003000 | 3 |
| x500v2_esc_FL | ESC BLHeli-S 20A FL | 0.013000 | 0.011000 | 0.002000 | 2 |
| x500v2_esc_BL | ESC BLHeli-S 20A BL | 0.013000 | 0.011000 | 0.002000 | 2 |
| x500v2_esc_BR | ESC BLHeli-S 20A BR | 0.013000 | 0.011000 | 0.002000 | 2 |
| x500v2_esc_FR | ESC BLHeli-S 20A FR | 0.013000 | 0.011000 | 0.002000 | 2 |
| x500v2_telemetry_radio | SiK Telemetry Radio V3 | 0.021000 | 0.019000 | 0.002000 | 2 |

## Verificacion Hotspots
| hotspotId | hotspotName | esperadoKg | actualKg | deltaKg | refs |
|---|---|---:|---:|---:|---:|
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.073000 | 0.073000 | 0.000000 | 11 |
| x500v2_pixhawk6c | Pixhawk 6C Autopilot | 0.051000 | 0.051000 | 0.000000 | 5 |
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.004000 | 0.004000 | 0.000000 | 4 |
| x500v2_pdb | Power Distribution Board | 0.006000 | 0.006000 | 0.000000 | 6 |

## Registros con Peso <= 0
| id | partName | weightKg |
|---|---|---:|
| x500v2_fastener_group | Fasteners Group | 0.000000 |
| x500v2_misc_camera_mount | x500v2 misc camera mount | 0.000000 |
| x500v2_misc_frame_connector_0 | x500v2 misc frame connector 0 | 0.000000 |
| x500v2_misc_frame_connector_1 | x500v2 misc frame connector 1 | 0.000000 |
| x500v2_misc_group | Misc Group | 0.000000 |
| x500v2_misc_light_cover | x500v2 misc light cover | 0.000000 |
