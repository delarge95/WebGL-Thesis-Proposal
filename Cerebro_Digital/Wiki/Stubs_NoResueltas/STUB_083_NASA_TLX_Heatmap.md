---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-083
aliases:
  - "NASA_TLX_Heatmap.md"
resumen: "Mapa visual de carga NASA-TLX para detectar patrones de esfuerzo percibido"
---

# NASA TLX Heatmap

## Proposito

Representa la carga mental, fisica y temporal de una sesion de evaluacion para identificar areas donde la experiencia degrada la comprension o el rendimiento.

## Uso esperado

- Agrupar resultados por tarea, usuario o iteracion.
- Detectar picos de esfuerzo percibido en puntos concretos del flujo.
- Comparar sesiones sucesivas para ver si la carga disminuye.
- Alimentar la lectura conjunta con `Usability_Metrics_Report.md`.

## Criterios

1. El mapa debe ser interpretable sin perder la escala original de valores.
2. La codificacion visual debe mantenerse consistente entre sesiones.
3. Cualquier conclusion debe apoyarse en datos de sesion y no solo en color.
4. La visualizacion debe documentar el metodo de agregacion empleado.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Testing_Validation`
- `Usability_Metrics_Report.md`
