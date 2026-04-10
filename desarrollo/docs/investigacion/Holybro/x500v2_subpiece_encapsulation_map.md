# Mapa de Encapsulado de Subpiezas (X500 V2)

Fecha: 2026-04-09

Objetivo de esta versión:

- Garantizar que cada subpieza quede encapsulada bajo una pieza madre del tipo correcto.
- Priorizar clasificación por tipo/categoría por encima de posición espacial exacta.
- No dejar fasteners fuera de una pieza madre.

## Criterio Operativo

1. La pieza madre canónica manda (nodo `x500v2_*`).
2. Si una subpieza tiene nombre de familia clara (motor, arm, pixhawk, gps, etc.), se encapsula en esa madre.
3. Si es fastener y tiene sufijo de cuadrante (`_FL/_FR/_BL/_BR`), va al brazo de ese cuadrante.
4. Si es fastener genérico sin cuadrante, se encapsula en `x500v2_bottom_plate` como fallback estructural.
5. La precisión posicional fina se posterga para el modelo final.

## Encapsulado Madre -> Subpiezas

## x500v2_pixhawk6c

- DIKE-PIXHAWK6C-LV-C1
- IMU-PIXHAWK6C
- MIANKE-PIXHAWK6C-LV-C1
- PCB-PIXHAWK6C-F1
- BM06B-WO

## x500v2_gps_m10

- GAN-GPSV5-ZHIJIA
- GPS-ZHIJIA-ZHUANJIETOU
- GPS-ZHIJIA-ZUO
- GPSV5-ZHIJIA-LUOMAO
- GPSV5-ZHIJIA-TUOPAN

## x500v2_power_module

- PCB-PM06
- TOU-XT60H-M-14AWG
- X500-TAO-XT60
- Subpiezas con token `pdb`/`power_module`/`xt60`

## x500v2_rails_battery

- BATTERY-MOUNTING-PLAT
- BATTERY-PAD
- PYLONS-X500
- GUAN-CHENG
- Battery straps / piezas con token `battery` o `strap`

## x500v2_landing_gear

- CARBON-FIBER-TUBE (landing)
- JIAO-EVA
- JIAO-LIANJIE
- MAO-JIAO

## x500v2_bottom_plate

- BOTTOM-PLATE-X500-V5
- GAI-GUANGLIU
- ZHIJIA-CAMERA-INTEL
- Fasteners genéricos sin cuadrante (fallback)

## x500v2_top_plate

- TOP-PLATE-X500-V5

## x500v2_platform_board

- PLATFORM-PLAT-X500

## x500v2_arm_FL / x500v2_arm_FR / x500v2_arm_BL / x500v2_arm_BR

- CARBON-FIBER-TUBE300
- HMX5V-DIGAI-DIANJIZUO-MUJU
- HMX5V-GUAN-DINGWEI
- HMX5V-JIBI-JIA-MUJU
- HMX5V-ZUO-DJ-MUJU
- BAN-DJ-DIAN-F2
- Subpiezas de familia motor/esc/prop con sufijo de cuadrante
- Fasteners con sufijo de cuadrante

## Política de Fasteners (obligatoria)

Fasteners cubiertos por token:

- `fastener`, `screw`, `cap_screw`, `bolt`, `nut`, `washer`, `standoff`, `spacer`
- `gb70`, `lm-`, `zslm`, `nilongzhu`, `chen-liu`, `pan-ding`

Reglas:

- Con cuadrante: se asignan al brazo `x500v2_arm_<QUADRANT>`.
- Sin cuadrante: se asignan a `x500v2_bottom_plate`.
- Resultado esperado: ningún fastener queda fuera de una pieza madre.

## Nota de alcance

Este mapa asegura consistencia tipológica de encapsulado para iteración actual.
La corrección de posición exacta de subpiezas/fasteners se realizará en la fase final de ajuste de modelo.
