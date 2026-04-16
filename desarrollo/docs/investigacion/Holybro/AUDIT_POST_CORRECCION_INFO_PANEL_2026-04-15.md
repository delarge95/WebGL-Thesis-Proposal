# AUDITORIA POST-CORRECCION INFO PANEL (2026-04-15)

## Resumen Ejecutivo
- Assets evaluados: **257**
- Registros con peso <= 0: **6**
- Registros con campos criticos vacios (sin texto): **0**
- Campo description en N/D: **0**
- Campo dimensions en N/D: **0**
- Campo function en N/D: **0**
- Campo materialType en N/D: **0**
- Campo materialProperties en N/D: **0**

## Consistencia Masa Piezas Madre
- Padres evaluados con subpiezas/fasteners hijos: **20**
- Inconsistencias (|esperado - actual| > 0.005 kg): **12**

| parentId | pesoEsperadoKg | pesoActualKg | deltaKg | hijos |
|---|---:|---:|---:|---:|
| x500v2_platform_board | 0.083000 | 0.049000 | 0.034000 | 34 |
| x500v2_rc_receiver | 0.037000 | 0.021000 | 0.016000 | 16 |
| x500v2_power_module | 0.056000 | 0.042000 | 0.014000 | 14 |
| x500v2_landing_gear | 0.104000 | 0.092000 | 0.012000 | 12 |
| x500v2_motor_FL | 0.083000 | 0.073000 | 0.010000 | 10 |
| x500v2_motor_BL | 0.083000 | 0.073000 | 0.010000 | 10 |
| x500v2_motor_BR | 0.083000 | 0.073000 | 0.010000 | 10 |
| x500v2_motor_FR | 0.083000 | 0.073000 | 0.010000 | 10 |
| x500v2_arm_BR | 0.048000 | 0.039000 | 0.009000 | 9 |
| x500v2_bottom_plate | 0.093000 | 0.084000 | 0.009000 | 9 |
| x500v2_arm_FR | 0.046000 | 0.038000 | 0.008000 | 8 |
| x500v2_pdb | 0.012000 | 0.006000 | 0.006000 | 6 |

## Consistencia Masa Hotspots
- Hotspots evaluados con referencias mapeables: **4**
- Inconsistencias (|esperado - actual| > 0.005 kg): **0**

## Registros con peso <= 0 (muestra)
| id | partName | weightKg |
|---|---|---:|
| x500v2_fastener_group | Fasteners Group | 0.000000 |
| x500v2_misc_camera_mount | x500v2 misc camera mount | 0.000000 |
| x500v2_misc_frame_connector_0 | x500v2 misc frame connector 0 | 0.000000 |
| x500v2_misc_frame_connector_1 | x500v2 misc frame connector 1 | 0.000000 |
| x500v2_misc_group | Misc Group | 0.000000 |
| x500v2_misc_light_cover | x500v2 misc light cover | 0.000000 |

## Nota
- Esta auditoria se genero automaticamente tras la fase de correccion prioritaria para validar trazabilidad y cierre parcial.
