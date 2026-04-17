---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-034
aliases:
  - "Color_Mapping_by_Category.md"
resumen: "Mapa de color por categoria para codificar estados y familias de piezas"
---

# Color Mapping by Category

## Proposito

Asigna colores de forma consistente a categorias o estados para que el usuario pueda reconocer familias y diferencias sin leer texto adicional.

## Reglas de uso

- Cada categoria debe mantener el mismo color.
- El sistema debe funcionar con accesibilidad basica.
- Los colores no deben entrar en conflicto con otros modos.
- La leyenda debe ser simple y estable.

## Criterios

1. La asignacion debe ser predecible y documentada.
2. Evitar depender de diferencias sutiles que se pierden en pantalla.
3. La codificacion debe sobrevivir a cambios de vista.
4. Si una categoria cambia, el mapeo debe quedar versionado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `Component_Category_System.md`
- `SolidColor_Kategory_Mapper.md`
