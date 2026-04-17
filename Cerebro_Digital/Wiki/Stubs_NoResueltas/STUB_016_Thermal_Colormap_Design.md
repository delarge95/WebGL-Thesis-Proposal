---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-016
aliases:
  - "Thermal_Colormap_Design.md"
resumen: "Diseño de mapa de color termico para una lectura clara y consistente"
---

# Thermal Colormap Design

## Proposito

Diseña una escala cromatica que traduzca temperatura en lectura intuitiva sin romper comparabilidad entre vistas o sesiones.

## Reglas de diseño

- La escala debe ser monotona y facil de interpretar.
- Debe evitar saltos confusos entre intervalos cercanos.
- Ha de conservar contraste para zonas criticas.
- La leyenda debe dejar claro el rango util.

## Criterios

1. El colormap debe ser estable entre versiones.
2. La lectura no debe depender de una sola combinacion de fondo.
3. La nota debe advertir si la paleta sacrifica precision por claridad.
4. Debe conectar con el shader termico usado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Sistema_Termico`
- `Custom Thermal Shader`
- `THERMAL_CONSTANTS.md`
