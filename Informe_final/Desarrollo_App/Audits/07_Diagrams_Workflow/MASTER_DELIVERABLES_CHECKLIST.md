# Master Checklist: Figuras, Screenshots y Tablas Pendientes

Actualizado al 2026-04-07.

Este checklist consolida el estado real del material visual del informe, cruzando:

- Inventario de auditoria previa.
- Diagramas Mermaid 4.1-4.13 definidos.
- Estado actual de inserciones en LaTeX.

## 1) Estado Real Actual

### 1.1 Diagramas (capitulo 4)

Estado: definidos en Mermaid y numerados (4.1-4.13), pero aun no exportados e insertados como imagen final en LaTeX.

- [ ] Figura 4.1 Arquitectura por capas del sistema
- [ ] Figura 4.2 Flujo EventBus Publish/Subscribe
- [ ] Figura 4.3 Maquina de estados de la aplicacion
- [ ] Figura 4.4 Flujo de seleccion de piezas
- [ ] Figura 4.5 Pipeline de shaders por modo
- [ ] Figura 4.6 Flujo de datos DronePartData
- [ ] Figura 4.7 Subsistema termico hibrido
- [ ] Figura 4.8 Restricciones WebGL y mitigaciones
- [ ] Figura 4.9 Herramientas de ensamblaje proyectadas
- [ ] Figura 4.10 Flujo de despliegue y artefactos
- [ ] Figura 4.11 Plantilla de resultados SUS
- [ ] Figura 4.12 Plantilla de resultados NASA-TLX
- [ ] Figura 4.13 Plantilla de KPIs tecnicos

Nota: en LaTeX existe una figura placeholder del pipeline (fig:pipeline-3d), pero no hay imagen final vinculada.

### 1.2 Figuras no diagrama (screenshots y renders)

Estado: no insertadas en el informe. La ubicación sugerida es Cap. 4 para vistas de interfaz/flujo y Cap. 5 para gráficas de resultados.

- [ ] Cap. 4 - Landing/Hero del visor: imagen de apertura de la experiencia.
- [ ] Cap. 4 - Vista principal Realistic: render base del dron con PBR.
- [ ] Cap. 4 - Vista Explosionada: separación de piezas y lectura de ensamblaje.
- [ ] Cap. 4 - Modo X-Ray: transparencia y visibilidad interna.
- [ ] Cap. 4 - Modo Wireframe: lectura de malla y depuración visual.
- [ ] Cap. 4 - Modo Thermal: gradiente térmico en componentes críticos.
- [ ] Cap. 4 - Cross-Section (X/Y/Z): corte transversal con plano activo.
- [ ] Cap. 4 - Panel de catálogo y filtros: exploración de piezas.
- [ ] Cap. 4 - Panel de detalle (bottom sheet): ficha técnica de una pieza.
- [ ] Cap. 4 - Hotspots (hover/selección): punto interactivo sobre el modelo.
- [ ] Cap. 4 - Comparativa de entorno (Studio vs Exterior/Night): presets de iluminación.
- [ ] Cap. 4 - Vista responsive en móvil: adaptación a pantalla pequeña.
- [ ] Cap. 4 - Comparativa retopología: high-poly vs low-poly.

### 1.3 Graficas y tablas de resultados (capitulo 5)

Estado: estructura creada con placeholders; faltan datos reales e imágenes/gráficas finales. La ubicación sugerida es Cap. 5, inmediatamente después de las subsecciones de KPIs, SUS, NASA-TLX y Think-Aloud.

- [ ] Cap. 5 - Tabla final KPIs técnicos con resultados reales.
- [ ] Cap. 5 - Tabla FPS por dispositivo (desktop/tablet/móvil).
- [ ] Cap. 5 - Tabla SUS detallada por participante.
- [ ] Cap. 5 - Gráfica SUS (barras o boxplot).
- [ ] Cap. 5 - Tabla NASA-TLX detallada por dimensión y participante.
- [ ] Cap. 5 - Gráfica NASA-TLX comparativa (radar o barras).
- [ ] Cap. 5 - Tabla Think-Aloud (categorías, frecuencia, citas).

### 1.4 Tablas existentes que requieren actualizacion editorial

Estado: existen en LaTeX, pero necesitan verificacion final de consistencia de cifras y narrativa.

- [ ] Tabla Resumen de Artefactos (validar cifras finales y coherencia con repo)
- [ ] Tabla Inventario de Modulos (mantener conteos finales consistentes)
- [ ] Tabla Productos Entregados vs Esperados (actualizar estado de evaluacion)
- [ ] Tabla pipeline 3D en apendice (ya marca Houdini evaluado/descartado; verificar texto final)

### 1.5 Ubicación sugerida para importación Mermaid

Estado: los 13 diagramas Mermaid ya se exportaron a PDF vectorial y deben enlazarse en el capítulo 4 con la numeración 4.1-4.13.

- [ ] [figures/chapter4/fig_4_1.pdf] Figura 4.1 - Arquitectura por capas del sistema.
- [ ] [figures/chapter4/fig_4_2.pdf] Figura 4.2 - Flujo EventBus Publish/Subscribe.
- [ ] [figures/chapter4/fig_4_3.pdf] Figura 4.3 - Máquina de estados de la aplicación.
- [ ] [figures/chapter4/fig_4_4.pdf] Figura 4.4 - Flujo de selección de piezas.
- [ ] [figures/chapter4/fig_4_5.pdf] Figura 4.5 - Pipeline de shaders por modo.
- [ ] [figures/chapter4/fig_4_6.pdf] Figura 4.6 - Flujo de datos DronePartData.
- [ ] [figures/chapter4/fig_4_7.pdf] Figura 4.7 - Subsistema térmico híbrido.
- [ ] [figures/chapter4/fig_4_8.pdf] Figura 4.8 - Restricciones WebGL y mitigaciones.
- [ ] [figures/chapter4/fig_4_9.pdf] Figura 4.9 - Herramientas de ensamblaje proyectadas.
- [ ] [figures/chapter4/fig_4_10.pdf] Figura 4.10 - Despliegue y artefactos.
- [ ] [figures/chapter4/fig_4_11.pdf] Figura 4.11 - Plantilla SUS.
- [ ] [figures/chapter4/fig_4_12.pdf] Figura 4.12 - Plantilla NASA-TLX.
- [ ] [figures/chapter4/fig_4_13.pdf] Figura 4.13 - Plantilla KPIs técnicos.

## 2) Lista Consolidada de TODO lo que Falta

Este es el faltante real a cerrar para el paquete visual final:

1. Exportar los 13 Mermaid a PNG/SVG con nomenclatura estable (fig_4_1 ... fig_4_13).
2. Insertar las 13 figuras en LaTeX (reemplazar placeholder y verificar labels/captions).
3. Producir 13 screenshots/renders no-diagrama del prototipo (UI, modos, analisis, movil, comparativo retopologia).
4. Completar 7 artefactos de resultados en capitulo 5 (tablas y graficas con datos reales).
5. Ejecutar pasada final de consistencia (lista de figuras, lista de tablas, referencias cruzadas y numeracion).

## 3) Priorizacion Recomendada

- P0 (bloqueante de capitulo 4): exportar + insertar figuras 4.1-4.13.
- P1 (bloqueante de capitulo 5): capturar datos reales y cerrar tablas/graficas SUS-NASA-KPI.
- P2 (calidad editorial): screenshots no-diagrama y comparativos visuales.
- P3 (cierre): verificacion de referencias cruzadas, lista de figuras y lista de tablas.

## 4) Criterios de Cierre

Se considera completo cuando:

- [ ] Lista de Figuras contiene 4.1-4.13 mas screenshots/renders adicionales referenciados.
- [ ] Lista de Tablas no tiene placeholders ni campos pendientes.
- [ ] Capitulo 5 no contiene marcadores de pendiente en SUS/NASA-TLX/KPIs.
- [ ] Todas las referencias \ref y captions compilan sin warnings criticos.
