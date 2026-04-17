---
tipo: nota_consolidada
area: sistema_termico
estado: activo
trace_id: STUB-NR-123
aliases:
  - "Thermal_Solver_State_Machine.md"
resumen: "Maquina de estados del solver termico y sus transiciones"
---

# Thermal Solver State Machine

## Proposito

Esta nota consolida la maquina de estados del solver termico, que gobierna la secuencia de inicializacion, calentamiento, estabilizacion y enfriamiento.

## Estados base

- `idle`: sin simulacion activa.
- `heating`: aplicacion de carga termica.
- `stabilizing`: iteracion hasta convergencia.
- `cooling`: disipacion y retorno a estado base.
- `error`: inconsistencias de entrada o divergencia.

## Transiciones

1. `idle -> heating` cuando se activa una condicion de simulacion.
2. `heating -> stabilizing` cuando el sistema alcanza un rango util.
3. `stabilizing -> cooling` al retirar la carga o finalizar la sesion.
4. `any -> error` ante parametros invalidos o datos incompletos.

## Relacion con el grafo

- `Solver_Architecture.md`
- `Performance_Thermal_Render.md`
- `Thermal_Regression_Tests.md`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

