---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-018
aliases:
  - "Thermal_Contact_Model.md"
resumen: "Modelo de contacto termico para estimar transferencia entre piezas y materiales"
---

# Thermal Contact Model

## Proposito

Modela la transferencia termica entre piezas en contacto para capturar mejor la respuesta real del sistema.

## Elementos del modelo

- Superficies de contacto.
- Resistencia termica de interfaz.
- Condiciones de presion o union.
- Efecto de materiales intermedios.

## Criterios

1. El modelo debe indicar que contactos son relevantes.
2. La parametrizacion debe ser reproducible.
3. No sobregeneralizar condiciones que cambian por pieza.
4. La nota debe ayudar a justificar el solver o el ajuste.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Energy Conservation Proof`
- `Thermal_Solver_State_Machine.md`
