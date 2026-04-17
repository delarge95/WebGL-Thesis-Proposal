---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-038
aliases:
  - "Component_Selection_Hierarchy.md"
resumen: "Jerarquia de seleccion de componentes para full part, subpiece e inspeccion"
---

# Component Selection Hierarchy

## Proposito

Define la jerarquia de seleccion para distinguir entre pieza completa, subpieza y niveles superiores de agrupacion.

## Reglas de seleccion

- La seleccion debe respetar la jerarquia del ensamblaje.
- El usuario debe entender que nivel esta activo.
- Debe coexistir con isolate y ghosted.
- La transicion entre niveles debe ser predecible.

## Criterios

1. La jerarquia debe evitar ambiguedades al hacer click.
2. Debe ser facil pasar entre full part y subpiece.
3. La nota debe apoyar la logica de selection tests.
4. Cualquier excepcion debe documentarse.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `Selection System Tests`
- `Selection Highlight Shader`
