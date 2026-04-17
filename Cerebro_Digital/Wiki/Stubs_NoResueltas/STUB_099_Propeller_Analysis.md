---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-099
aliases:
  - "Propeller_Analysis.md"
resumen: "Analisis de la helice para relacionar empuje, carga y eficiencia"
---

# Propeller Analysis

## Proposito

Analiza la helice para entender como afecta al empuje, al consumo y a la respuesta total del sistema de propulsion.

## Variables de lectura

- Diametro y paso.
- Empuje relativo.
- Eficiencia en la zona de trabajo.
- Compatibilidad con motor y frame.

## Criterios

1. El analisis debe usar el mismo escenario que motor y bateria.
2. No separar empuje de consumo si se quiere una decision util.
3. La conclusion debe ayudar a ajustar configuracion.
4. La nota debe ser util para comparacion entre variantes.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `Motor_Specs_BLDC.md`
- `Battery_Power_Analysis.md`
