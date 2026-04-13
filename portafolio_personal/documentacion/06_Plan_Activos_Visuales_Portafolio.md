# Plan de Activos Visuales para Portafolio

Documento operativo para convertir los breakdowns técnicos en material visual convincente y preciso para ArtStation, LinkedIn, reel, README y entrevistas.

## Objetivo

Definir qué capturas, clips, diagramas y comparativas hacen falta para que el portafolio dependa de evidencia real y no solo de texto.

## Regla editorial

Solo deben capturarse y publicarse vistas realmente alineadas con la build visible final. No usar:

- placeholders;
- UI legacy;
- features ocultas como si fueran oficiales;
- resultados de usabilidad inventados o prematuros.

## Paquete visual mínimo por pieza

### 1. Visor WebGL final

Capturas obligatorias:

- Hero final del Holybro X500 V2;
- vista principal en `Realistic`;
- selección + `bottom sheet`;
- `Inspect`;
- `Analyze`;
- `Studio`;
- `Thermal` con leyenda visible.

Clip obligatorio:

- flujo completo desde Hero hasta una pieza seleccionada y un cambio de modo visual.

### 2. Sistema de visualización y shaders

Capturas obligatorias:

- comparativa `Realistic` vs `X-Ray`;
- comparativa `Solid Color` vs `Thermal`;
- detalle de corte transversal;
- detalle del `bottom sheet` con pieza seleccionada.

Activos opcionales:

- `Blueprint`, `Wireframe` o `Ghosted` solo como slide secundaria de profundidad técnica, claramente marcados como modos implementados/no visibles en la UI final.

### 3. Sistema térmico híbrido

Capturas obligatorias:

- modo térmico con leyenda;
- comparación entre modo base y modo térmico;
- close-up de una pieza caliente y otra fría;
- diagrama del subsistema térmico o del flujo `DroneStateController -> ThermalSimulationManager -> ThermalViewController`.

### 4. CAD -> Unity -> WebGL

Capturas obligatorias:

- lámina del problema de entrada;
- diagrama de pipeline;
- imagen o tabla explicando `28 / 30 / 257`;
- captura del resultado final en browser.

Activos opcionales:

- comparativa de retopología o fasteners, si y solo si hay evidencia limpia y verificable.

### 5. Tooling de editor

Capturas obligatorias:

- `ProjectSetupWizard`;
- `ImportedDroneCoverageAudit`;
- `ThermalContactGraphBuilderWindow`.

Estas capturas son muy valiosas porque diferencian el proyecto de una demo visual tradicional.

## Diagramas reutilizables del informe final

Las siguientes figuras del informe son buenas candidatas para adaptarse al portafolio:

- arquitectura por capas;
- flujo de selección de piezas;
- pipeline de shaders por modo;
- arquitectura del subsistema térmico;
- restricciones WebGL y mitigaciones;
- flujo de despliegue y artefactos.

No usar como pieza pública principal:

- esquemas metodológicos de SUS, NASA-TLX o KPI;
- gráficos de resultados si la medición final aún no está cerrada.

## Relación con los placeholders del informe

El informe final ya reservó placeholders editoriales que sirven como checklist de captura. Antes de cerrar el portafolio, esas reservas deben traducirse a activos reales equivalentes para:

- Hero;
- `Realistic`;
- `Exploded`;
- `X-Ray`;
- `Thermal`;
- cross-section;
- filtros por categoría;
- `bottom sheet`;
- hotspots;
- vista responsive;
- comparativa de retopología.

## Activos que no deben priorizarse

- settings;
- catálogo legacy;
- measurement visible;
- audio;
- paneles no integrados;
- resultados experimentales aún no cerrados.

## Entregables mínimos por canal

### ArtStation

- 1 portada hero;
- 1 clip o GIF del flujo principal;
- 1 diagrama del pipeline o arquitectura;
- 1 comparativa de view modes;
- 1 slide de tooling;
- 1 slide de sistema térmico.

### LinkedIn

- 1 carrusel de 4 a 6 slides;
- 1 clip corto;
- texto breve con problema, solución y stack.

### README o página personal

- resumen del problema;
- demo visual;
- arquitectura corta;
- tooling;
- límites honestos.

## Orden recomendado de producción

1. Hero.
2. Flujo principal del visor.
3. View modes visibles.
4. Sistema térmico.
5. Tooling de editor.
6. Pipeline CAD -> WebGL.
7. Diagramas resumidos.

## Carpeta sugerida para staging visual

Cuando empiece la producción visual, organizar en `assets_fuente/` por bloques:

- `viewer_runtime/`
- `view_modes/`
- `thermal/`
- `tooling/`
- `cad_pipeline/`
- `diagrams/`

Así se evita mezclar capturas de producto, editor, browser y documentación.
