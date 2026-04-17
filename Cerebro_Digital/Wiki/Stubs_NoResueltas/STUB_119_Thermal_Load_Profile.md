---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-119
aliases:
  - "Thermal_Load_Profile.md"
resumen: "Perfil de carga termica para estimar demanda por escenarios y piezas"
---

# Thermal Load Profile

## Proposito

Registra como se distribuye la carga termica en distintos escenarios para comparar demanda y puntos calientes.

## Variables utiles

- Escenario de carga.
- Duracion de la carga.
- Picos y estabilidad.
- Piezas mas afectadas.

## Criterios

1. El perfil debe ser comparable entre iteraciones.
2. Debe indicar que cargas son sostenidas y cuales puntuales.
3. No mezclar perfil de entorno con perfil de pieza.
4. La nota debe alimentar decisiones de ajuste.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Thermal_Properties_by_Component.md`
- `Boundary Conditions`
