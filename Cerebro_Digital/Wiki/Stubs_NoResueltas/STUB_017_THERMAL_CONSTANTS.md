---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-017
aliases:
  - "THERMAL_CONSTANTS.md"
resumen: "Constantes termicas base para el solver y la interpretacion de resultados"
---

# Thermal Constants

## Proposito

Reune constantes y parametros base del modelo termico para evitar dispersion de valores y asegurar coherencia en las simulaciones.

## Contenido minimo

- Conductividad.
- Capacidad termica.
- Coeficientes de intercambio.
- Parametros de material y ambiente.

## Criterios

1. Cada constante debe tener unidad y origen.
2. El conjunto debe coincidir con el solver y los modelos de contacto.
3. No mezclar constantes de ensayo con supuestos de produccion.
4. Cualquier ajuste debe quedar versionado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Boundary Conditions`
- `Thermal_Contact_Model.md`
