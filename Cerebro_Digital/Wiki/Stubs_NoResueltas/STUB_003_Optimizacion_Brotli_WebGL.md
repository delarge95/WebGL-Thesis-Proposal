---
tipo: nota_consolidada
area: webgl
estado: activo
trace_id: STUB-NR-003
aliases:
  - "Optimizacion_Brotli_WebGL"
resumen: "Optimizacion de Brotli para reducir el peso de entrega WebGL"
---

# Optimizacion Brotli WebGL

## Proposito

Esta nota consolida la estrategia de compresion Brotli para activos WebGL, con foco en reducir tiempos de descarga y mejorar la primera carga sin alterar la trazabilidad del build.

## Alcance

- Compresion de `.wasm`, `.data` y `.js`.
- Evaluacion de impacto sobre tamaño final y tiempo de descarga.
- Integracion con pipeline de despliegue.

## Criterios de uso

1. Aplicar solo a artefactos de distribucion.
2. Verificar que el hosting sirva los encabezados y extensiones correctas.
3. Registrar comparacion entre build comprimido y no comprimido.

## Relacion con el grafo

- `MOC_WebGL_Build_Pipeline`
- `MOC_Consolidacion_WebGL`
- `WEBGL_OPTIMIZATION_MANUAL.md`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

