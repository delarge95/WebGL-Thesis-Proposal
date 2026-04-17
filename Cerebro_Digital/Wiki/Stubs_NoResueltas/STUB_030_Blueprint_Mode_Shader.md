---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-030
aliases:
  - "Blueprint_Mode_Shader.md"
resumen: "Shader para modo blueprint con lectura tecnica y alto contraste"
---

# Blueprint Mode Shader

## Proposito

Genera una vista tipo blueprint para inspeccion tecnica del ensamblaje con alto contraste y lectura clara de contornos.

## Comportamiento esperado

- Resaltar estructura sin saturar detalles.
- Funcionar como modo de lectura tecnica, no decorativa.
- Mantener consistencia con otros modos de visualizacion.
- No romper rendimiento en escenas densas.

## Criterios

1. La paleta debe ser reconocible y estable.
2. El efecto no debe esconder la geometria importante.
3. Debe convivir con isolate, ghosted y selection.
4. Registrar dependencias de shader y postprocess.

## Relacion con el resto del grafo

- `MOC_Consolidacion_WebGL`
- `Wireframe_Mode_Spec.md`
- `Overview_Mode_Spec.md`
