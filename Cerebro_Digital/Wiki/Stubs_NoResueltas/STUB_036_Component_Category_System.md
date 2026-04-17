---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-036
aliases:
  - "Component_Category_System.md"
resumen: "Sistema de categorias de componentes para organizar seleccion, color y filtros"
---

# Component Category System

## Proposito

Organiza componentes en categorias estables para alimentar color mapping, filtros y jerarquia de seleccion.

## Reglas de categoria

- Cada componente debe tener una categoria clara.
- La categoria debe ser util para ver, filtrar y analizar.
- Debe soportar cambios sin romper la trazabilidad.
- Conviene evitar categorias redundantes.

## Criterios

1. El sistema debe ser facil de mantener.
2. Debe conectar con mapeo de color y filtros.
3. No confundir tipo, estado y jerarquia.
4. La nota debe servir para ordenar el resto del grafo.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `Color Mapping by Category`
- `Filter by Property`
