---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-097
aliases:
  - "Power_Distribution_Board.md"
resumen: "Ficha de la placa de distribucion de potencia y su rol en el sistema del dron"
---

# Power Distribution Board

## Proposito

Documenta la placa de distribucion de potencia para entender rutas energeticas, limites y dependencias con otros modulos.

## Campos relevantes

- Entrada y salidas de potencia.
- Capacidad y protecciones.
- Relacion con controladora y ESC.
- Restricciones de integracion.

## Criterios

1. La ficha debe ser util para diagnostico e integracion.
2. Las rutas de potencia deben quedar claras.
3. No mezclar espec con interpretacion sin distinguirlo.
4. Relacionar con baterias y control de potencia.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `Battery Power Analysis`
- `Flight Controller Specs`
