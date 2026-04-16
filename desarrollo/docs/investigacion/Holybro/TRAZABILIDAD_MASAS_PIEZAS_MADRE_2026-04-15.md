# TRAZABILIDAD DE MASAS PIEZAS MADRE Y HOTSPOTS (2026-04-15)

## Criterio de Consistencia Aplicado
- Formula madre estricta: peso_final_madre = peso_base_fuente + suma(pesos_hijos_directos).
- Formula hotspot: peso_final_hotspot = suma(hijos_directos) + suma(subComponentNames mapeables).
- Fuente base principal: x500v2_blender_synced_parts.json.

## Resultado Global
- Piezas madre actualizadas: **20**
- Hotspots actualizados: **3**
- Registros restantes con peso <= 0: **6**

## Trazabilidad Piezas Madre
| parentId | parentName | pesoAnteriorKg | pesoBaseKg | sumaHijosKg | pesoFinalKg | deltaKg | hijos | hijosConPeso | hijosSinPeso | fuente |
|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---|
| x500v2_platform_board | Platform Board | 0.049000 | 0.049000 | 0.034000 | 0.083000 | 0.034000 | 34 | 34 | 0 | peso asset + hijos asset |
| x500v2_rc_receiver | RC Receiver | 0.021000 | 0.021000 | 0.016000 | 0.037000 | 0.016000 | 16 | 16 | 0 | peso asset + hijos asset |
| x500v2_power_module | Power Module PM02 V3 | 0.042000 | 0.042000 | 0.014000 | 0.056000 | 0.014000 | 14 | 14 | 0 | peso asset + hijos asset |
| x500v2_landing_gear | Landing Gear | 0.092000 | 0.092000 | 0.012000 | 0.104000 | 0.012000 | 12 | 12 | 0 | peso asset + hijos asset |
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.073000 | 0.073000 | 0.010000 | 0.083000 | 0.010000 | 10 | 10 | 0 | peso asset + hijos asset |
| x500v2_motor_BL | Motor 2216 KV920 BL | 0.073000 | 0.073000 | 0.010000 | 0.083000 | 0.010000 | 10 | 10 | 0 | peso asset + hijos asset |
| x500v2_motor_BR | Motor 2216 KV920 BR | 0.073000 | 0.073000 | 0.010000 | 0.083000 | 0.010000 | 10 | 10 | 0 | peso asset + hijos asset |
| x500v2_motor_FR | Motor 2216 KV920 FR | 0.073000 | 0.073000 | 0.010000 | 0.083000 | 0.010000 | 10 | 10 | 0 | peso asset + hijos asset |
| x500v2_arm_BR | Arm Tube Back-Right | 0.039000 | 0.039000 | 0.009000 | 0.048000 | 0.009000 | 9 | 9 | 0 | peso asset + hijos asset |
| x500v2_bottom_plate | Carbon Fiber Bottom Plate | 0.084000 | 0.084000 | 0.009000 | 0.093000 | 0.009000 | 9 | 9 | 0 | peso asset + hijos asset |
| x500v2_arm_FR | Arm Tube Front-Right | 0.038000 | 0.038000 | 0.008000 | 0.046000 | 0.008000 | 8 | 8 | 0 | peso asset + hijos asset |
| x500v2_pdb | Power Distribution Board | 0.006000 | 0.006000 | 0.006000 | 0.012000 | 0.006000 | 6 | 6 | 0 | peso asset + hijos asset |
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.004000 | 0.004000 | 0.004000 | 0.008000 | 0.004000 | 4 | 4 | 0 | peso asset + hijos asset |
| x500v2_arm_BL | Arm Tube Back-Left | 0.033000 | 0.033000 | 0.003000 | 0.036000 | 0.003000 | 3 | 3 | 0 | peso asset + hijos asset |
| x500v2_arm_FL | Arm Tube Front-Left | 0.033000 | 0.033000 | 0.003000 | 0.036000 | 0.003000 | 3 | 3 | 0 | peso asset + hijos asset |
| x500v2_telemetry_radio | SiK Telemetry Radio V3 | 0.017000 | 0.017000 | 0.002000 | 0.019000 | 0.002000 | 2 | 2 | 0 | peso asset + hijos asset |
| x500v2_esc_FL | ESC BLHeli-S 20A FL | 0.009000 | 0.009000 | 0.002000 | 0.011000 | 0.002000 | 2 | 2 | 0 | peso asset + hijos asset |
| x500v2_esc_BL | ESC BLHeli-S 20A BL | 0.009000 | 0.009000 | 0.002000 | 0.011000 | 0.002000 | 2 | 2 | 0 | peso asset + hijos asset |
| x500v2_esc_BR | ESC BLHeli-S 20A BR | 0.009000 | 0.009000 | 0.002000 | 0.011000 | 0.002000 | 2 | 2 | 0 | peso asset + hijos asset |
| x500v2_esc_FR | ESC BLHeli-S 20A FR | 0.009000 | 0.009000 | 0.002000 | 0.011000 | 0.002000 | 2 | 2 | 0 | peso asset + hijos asset |

## Trazabilidad Hotspots
| hotspotId | hotspotName | pesoAnteriorKg | pesoFinalKg | deltaKg | hijosUsados | subComponentNamesMapeados | fuente |
|---|---|---:|---:|---:|---:|---:|---|
| x500v2_motor_FL | Motor 2216 KV920 FL | 0.083000 | 0.073000 | -0.010000 | 10 | 1 | hijos asset + mapeo blenderName en x500v2_blender_synced_parts.json |
| x500v2_pdb | Power Distribution Board | 0.012000 | 0.006000 | -0.006000 | 6 | 0 | hijos asset + mapeo blenderName en x500v2_blender_synced_parts.json |
| x500v2_battery | LiPo Battery 4S 5000mAh | 0.008000 | 0.004000 | -0.004000 | 4 | 0 | hijos asset + mapeo blenderName en x500v2_blender_synced_parts.json |
| x500v2_pixhawk6c | Pixhawk 6C Autopilot | 0.051000 | 0.051000 | 0.000000 | 0 | 5 | hijos asset + mapeo blenderName en x500v2_blender_synced_parts.json |

## Registros Residuales con Peso <= 0
| id | partName | weightKg |
|---|---|---:|
| x500v2_fastener_group | Fasteners Group | 0.000000 |
| x500v2_misc_camera_mount | x500v2 misc camera mount | 0.000000 |
| x500v2_misc_frame_connector_0 | x500v2 misc frame connector 0 | 0.000000 |
| x500v2_misc_frame_connector_1 | x500v2 misc frame connector 1 | 0.000000 |
| x500v2_misc_group | Misc Group | 0.000000 |
| x500v2_misc_light_cover | x500v2 misc light cover | 0.000000 |
