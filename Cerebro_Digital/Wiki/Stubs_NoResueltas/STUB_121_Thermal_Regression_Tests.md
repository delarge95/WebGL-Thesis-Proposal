---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-121
aliases:
  - "Thermal_Regression_Tests.md"
resumen: "Pruebas de regresion termica para detectar cambios no deseados entre versiones"
---

# Thermal Regression Tests

## Proposito

Verifica que el comportamiento termico no empeore cuando cambian solver, materiales o configuracion de la escena.

## Cobertura

- Comparacion contra baseline.
- Deteccion de regresiones por cambio.
- Estabilidad entre iteraciones.
- Validacion de correcciones previas.

## Criterios

1. La prueba debe poder repetirse sobre el mismo caso.
2. Debe quedar claro que cambio disparo la regresion.
3. No mezclar mejora visual con mejora termica.
4. La nota debe apoyar decisiones de versionado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Bug Fixes Thermal Solver`
- `Thermal Simulation Tests`
