# Final Unity 6 LOD Optimization Report

- Generated at UTC: `2026-06-10 22:22:02`
- Scene: `Assets/Scenes/MainScene_Final.unity`
- Drone root: `x500v2_Drone`
- LODGroups created: `52`
- Renderers skipped: `200`
- LOD0 source triangles in optimized renderers: `204022`
- LOD1 generated triangles: `129658`
- LOD2 generated triangles: `77284`
- LOD thresholds: `0.55 / 0.22 / 0.025` with cross-fade.
- Exclusions: modular fasteners, small meshes, generated LOD children and existing manual LODGroups.

## Optimized Renderers

| Renderer | Category | LOD0 tris | LOD1 tris | LOD2 tris |
|---|---:|---:|---:|---:|
| `x500v2_Drone/x500v2_bottom_plate/BOTTOM-PLATE-X500-V5.001_low` | SkeletonAirframe | 3364 | 2004 | 1372 |
| `x500v2_Drone/x500v2_top_plate/TOP-PLATE-X500-V5.001_low` | SkeletonAirframe | 2148 | 1404 | 748 |
| `x500v2_Drone/x500v2_arm_FL/BAN-DJ-DIAN-F2.001_low` | SkeletonAirframe | 1024 | 632 | 372 |
| `x500v2_Drone/x500v2_arm_FL/HMX5V-DIGAI-DIANJIZUO-MUJU.001_low` | SkeletonAirframe | 1686 | 1048 | 654 |
| `x500v2_Drone/x500v2_arm_FL/HMX5V-JIBI-JIA-MUJU.001_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_FL/HMX5V-JIBI-JIA-MUJU.002_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_FL/HMX5V-ZUO-DJ-MUJU.001_low` | SkeletonAirframe | 3062 | 1878 | 1068 |
| `x500v2_Drone/x500v2_arm_FL/x500v2_prop_FL` | PropulsionSystem | 31148 | 19809 | 11847 |
| `x500v2_Drone/x500v2_arm_FR/BAN-DJ-DIAN-F2.002_low` | SkeletonAirframe | 1024 | 632 | 372 |
| `x500v2_Drone/x500v2_arm_FR/HMX5V-DIGAI-DIANJIZUO-MUJU.002_low` | SkeletonAirframe | 1686 | 1048 | 654 |
| `x500v2_Drone/x500v2_arm_FR/HMX5V-JIBI-JIA-MUJU.003_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_FR/HMX5V-JIBI-JIA-MUJU.004_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_FR/HMX5V-ZUO-DJ-MUJU.002_low` | SkeletonAirframe | 3062 | 1878 | 1068 |
| `x500v2_Drone/x500v2_arm_FR/x500v2_prop_FR` | PropulsionSystem | 31148 | 19809 | 11847 |
| `x500v2_Drone/x500v2_arm_BL/BAN-DJ-DIAN-F2.004_low` | SkeletonAirframe | 1024 | 632 | 372 |
| `x500v2_Drone/x500v2_arm_BL/HMX5V-DIGAI-DIANJIZUO-MUJU.004_low` | SkeletonAirframe | 1686 | 1048 | 654 |
| `x500v2_Drone/x500v2_arm_BL/HMX5V-JIBI-JIA-MUJU.007_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_BL/HMX5V-JIBI-JIA-MUJU.008_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_BL/HMX5V-ZUO-DJ-MUJU.004_low` | SkeletonAirframe | 3062 | 1878 | 1068 |
| `x500v2_Drone/x500v2_arm_BL/x500v2_prop_BL` | PropulsionSystem | 31148 | 19809 | 11847 |
| `x500v2_Drone/x500v2_arm_BR/BAN-DJ-DIAN-F2.003_low` | SkeletonAirframe | 1024 | 632 | 372 |
| `x500v2_Drone/x500v2_arm_BR/HMX5V-DIGAI-DIANJIZUO-MUJU.003_low` | SkeletonAirframe | 1686 | 1048 | 654 |
| `x500v2_Drone/x500v2_arm_BR/HMX5V-JIBI-JIA-MUJU.005_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_BR/HMX5V-JIBI-JIA-MUJU.006_low` | SkeletonAirframe | 1122 | 732 | 422 |
| `x500v2_Drone/x500v2_arm_BR/HMX5V-ZUO-DJ-MUJU.003_low` | SkeletonAirframe | 3062 | 1878 | 1068 |
| `x500v2_Drone/x500v2_arm_BR/x500v2_prop_BR` | PropulsionSystem | 31148 | 19809 | 11847 |
| `x500v2_Drone/x500v2_landing_gear/JIA-LIANJIE.001_low` | SkeletonAirframe | 678 | 396 | 186 |
| `x500v2_Drone/x500v2_landing_gear/JIA-LIANJIE.002_low` | SkeletonAirframe | 678 | 396 | 186 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-EVA.001_low` | SkeletonAirframe | 966 | 602 | 324 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-EVA.002_low` | SkeletonAirframe | 966 | 602 | 324 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-EVA.003_low` | SkeletonAirframe | 966 | 602 | 324 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-EVA.004_low` | SkeletonAirframe | 966 | 602 | 324 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-LIANJIE.001_low` | SkeletonAirframe | 938 | 626 | 364 |
| `x500v2_Drone/x500v2_landing_gear/JIAO-LIANJIE.002_low` | SkeletonAirframe | 938 | 626 | 364 |
| `x500v2_Drone/x500v2_rails_battery/BATTERY-MOUNTING-PLAT.001_low` | PowerDistribution | 908 | 564 | 284 |
| `x500v2_Drone/x500v2_rails_battery/PLATFORM-PLAT-X500.001_low` | PowerDistribution | 2040 | 1200 | 752 |
| `x500v2_Drone/x500v2_rails_battery/PYLONS-X500.001_low` | PowerDistribution | 520 | 336 | 208 |
| `x500v2_Drone/x500v2_rails_battery/PYLONS-X500.002_low` | PowerDistribution | 520 | 336 | 208 |
| `x500v2_Drone/x500v2_rails_battery/ZHIJIA-CAMERA-INTEL.001_low` | PowerDistribution | 1746 | 1010 | 702 |
| `x500v2_Drone/x500v2_power_module/BM06B-WO.001_low` | PowerDistribution | 420 | 268 | 168 |
| `x500v2_Drone/x500v2_power_module/PCB-PM06.001_low` | PowerDistribution | 634 | 394 | 256 |
| `x500v2_Drone/x500v2_power_module/TOU-XT60H-M-14AWG.001_low` | PowerDistribution | 1092 | 742 | 384 |
| `x500v2_Drone/x500v2_power_module/X500-TAO-XT60.001_low` | PowerDistribution | 460 | 304 | 194 |
| `x500v2_Drone/x500v2_pixhawk6c/DIKE-PIXHAWK6C-LV-C1.001_low` | Avionics | 3292 | 2130 | 1204 |
| `x500v2_Drone/x500v2_pixhawk6c/MIANKE-PIXHAWK6C-LV-C1.001_low` | Avionics | 6488 | 4302 | 2508 |
| `x500v2_Drone/x500v2_gps_m10/GPS-ZHIJIA-ZUO.001_low` | SensorsComms | 1628 | 946 | 612 |
| `x500v2_Drone/x500v2_gps_m10/x500v2_gps_m10_PROXY_low` | SensorsComms | 2560 | 1628 | 1052 |
| `x500v2_Drone/x500v2_telemetry_radio/x500v2_telemetry_radio_PROXY_low` | SensorsComms | 1570 | 946 | 560 |
| `x500v2_Drone/x500v2_motor_FL/DJ-2216-KV880.001_low` | PropulsionSystem | 2720 | 1842 | 1134 |
| `x500v2_Drone/x500v2_motor_FR/DJ-2216-KV880.004_low` | PropulsionSystem | 2720 | 1842 | 1134 |
| `x500v2_Drone/x500v2_motor_BL/DJ-2216-KV880.002_low` | PropulsionSystem | 2720 | 1842 | 1134 |
| `x500v2_Drone/x500v2_motor_BR/DJ-2216-KV880.003_low` | PropulsionSystem | 2720 | 1842 | 1134 |

## Skipped Renderers

| Renderer | Reason |
|---|---|
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x10_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_009` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_010` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_011` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_012` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x12_013` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_009` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_010` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_011` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_012` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_013` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_014` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_015` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_016` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_017` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_018` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_019` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_020` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_021` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_022` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_023` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M25x6_024` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x21_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x21_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x25_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x25_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_009` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_010` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_011` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_012` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_013` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_014` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_015` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_016` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_009` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_010` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_011` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_012` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_013` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_014` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_015` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x6_016` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_001` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_002` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_003` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_004` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_005` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_006` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_007` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_009` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_010` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_011` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_012` | Excluded token 'screw'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_001` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_002` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_003` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_004` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_005` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_006` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_007` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_008` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_lock_nut_M3_001` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_lock_nut_M3_002` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_001` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_002` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_003` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_004` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_005` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_006` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_007` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_008` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_009` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_010` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_011` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M25x6_012` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x10_001` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x10_002` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x10_003` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x10_004` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x14_001` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x14_002` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x14_003` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.pan_head_M3x14_004` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M3x16_001` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.countersunk_M3x16_002` | Excluded token 'fastener'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M25x5_001` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M25x5_002` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M25x5_003` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M25x5_004` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M3x5_001` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M3x5_002` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M3x5_003` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.nylon_standoff_M3x5_004` | Excluded token 'standoff'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.self_lock_nut_M25_001` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.self_lock_nut_M25_002` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.self_lock_nut_M25_003` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.self_lock_nut_M25_004` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_009` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_010` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_011` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_012` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_013` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_014` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_015` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.lock_nut_M3_016` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_001` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_002` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_003` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_004` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_005` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_006` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_007` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_008` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_009` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_010` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_011` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_012` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_013` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_014` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_015` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_016` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_arm_FL/HMX5V-GUAN-DINGWEI.001_low` | Below triangle threshold (224). |
| `x500v2_Drone/x500v2_arm_FR/HMX5V-GUAN-DINGWEI.002_low` | Below triangle threshold (224). |
| `x500v2_Drone/x500v2_arm_BL/HMX5V-GUAN-DINGWEI.004_low` | Below triangle threshold (224). |
| `x500v2_Drone/x500v2_arm_BR/HMX5V-GUAN-DINGWEI.003_low` | Below triangle threshold (224). |
| `x500v2_Drone/x500v2_landing_gear/CARBON-FIBER-TUBE.001_low` | Below triangle threshold (128). |
| `x500v2_Drone/x500v2_landing_gear/CARBON-FIBER-TUBE.002_low` | Below triangle threshold (128). |
| `x500v2_Drone/x500v2_landing_gear/GUAN-CHENG.001_low` | Below triangle threshold (192). |
| `x500v2_Drone/x500v2_landing_gear/GUAN-CHENG.002_low` | Below triangle threshold (192). |
| `x500v2_Drone/x500v2_landing_gear/MAO-JIAO.001_low` | Below triangle threshold (180). |
| `x500v2_Drone/x500v2_landing_gear/MAO-JIAO.002_low` | Below triangle threshold (180). |
| `x500v2_Drone/x500v2_landing_gear/MAO-JIAO.003_low` | Below triangle threshold (180). |
| `x500v2_Drone/x500v2_landing_gear/MAO-JIAO.004_low` | Below triangle threshold (180). |
| `x500v2_Drone/x500v2_rails_battery/BATTERY-PAD.001_low` | Below triangle threshold (92). |
| `x500v2_Drone/x500v2_rails_battery/CARBON-FIBER-TUBE300.001_low` | Below triangle threshold (128). |
| `x500v2_Drone/x500v2_rails_battery/CARBON-FIBER-TUBE300.002_low` | Below triangle threshold (128). |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/rubber_grommet` | Excluded token 'rubber_grommet'. |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.001_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.002_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.003_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.004_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.005_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.006_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.007_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/JIA-GUAN.008_low` | Below triangle threshold (216). |
| `x500v2_Drone/x500v2_rails_battery/GAI-GUANGLIU.001_low` | Below triangle threshold (312). |
| `x500v2_Drone/x500v2_rails_battery/x500v2_battery_PROXY_low` | Below triangle threshold (44). |
| `x500v2_Drone/x500v2_pixhawk6c/PCB-PIXHAWK6C-F1.001_low` | Below triangle threshold (344). |
| `x500v2_Drone/x500v2_pixhawk6c/IMU-PIXHAWK6C.001_low` | Below triangle threshold (304). |
| `x500v2_Drone/x500v2_gps_m10/GAN-GPSV5-ZHIJIA.001_low` | Below triangle threshold (56). |
| `x500v2_Drone/x500v2_gps_m10/GPS-ZHIJIA-ZHUANJIETOU.001_low` | Below triangle threshold (222). |
| `x500v2_Drone/x500v2_gps_m10/lock_nut_M3` | Excluded token 'nut'. |
| `x500v2_Drone/x500v2_gps_m10/GPSV5-ZHIJIA-TUOPAN.001_low` | Below triangle threshold (210). |
