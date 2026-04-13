# Master Checklist: Figuras, Screenshots y Tablas Pendientes

Actualizado al 2026-04-12.

Este checklist consolida el estado real del material visual del informe, cruzando:

- inventario de auditoría previa;
- diagramas Mermaid 4.1-4.13;
- placeholders editoriales insertados en LaTeX;
- tablas activas y reservas de resultados del Capítulo 5.

## 1) Estado real actual

### 1.1 Diagramas Mermaid

Estado: los 13 diagramas ya están exportados a PDF vectorial y enlazados en el informe final. Las Figuras 4.1-4.10 quedaron integradas en el Capítulo 4 y las Figuras 4.11-4.13 se conservaron en apéndices como esquemas metodológicos, no como resultados observados.

- [x] Figura 4.1 Arquitectura por capas del sistema
- [x] Figura 4.2 Flujo EventBus Publish/Subscribe
- [x] Figura 4.3 Máquina de estados de la aplicación
- [x] Figura 4.4 Flujo de selección de piezas
- [x] Figura 4.5 Pipeline de shaders por modo
- [x] Figura 4.6 Flujo de datos \texttt{DronePartData}
- [x] Figura 4.7 Subsistema térmico híbrido
- [x] Figura 4.8 Restricciones WebGL y mitigaciones
- [x] Figura 4.9 Herramientas de ensamblaje proyectadas
- [x] Figura 4.10 Flujo de despliegue y artefactos
- [x] Figura 4.11 Esquema metodológico SUS
- [x] Figura 4.12 Esquema metodológico NASA-TLX
- [x] Figura 4.13 Esquema metodológico KPIs técnicos

### 1.2 Figuras no diagrama: screenshots y renders

Estado: el Capítulo 4 ya contiene placeholders editoriales para todas las capturas no-diagrama. Deben sustituirse por screenshots reales una vez exista build congelada y set visual definitivo.

- [x] Cap. 4 - Landing/Hero del visor
- [x] Cap. 4 - Vista principal \texttt{Realistic}
- [x] Cap. 4 - Vista explosionada
- [x] Cap. 4 - Modo \texttt{X-Ray}
- [x] Cap. 4 - Modo \texttt{Wireframe} con nota de estado "oculto"
- [x] Cap. 4 - Modo \texttt{Thermal}
- [x] Cap. 4 - Cross-section con plano activo
- [x] Cap. 4 - Filtros por categoría y exploración de piezas
- [x] Cap. 4 - \textit{Bottom sheet} de detalle
- [x] Cap. 4 - Hotspots sobre el modelo
- [x] Cap. 4 - Comparativa de presets de entorno
- [x] Cap. 4 - Vista responsive en móvil
- [x] Cap. 4 - Comparativa de retopología

### 1.3 Gráficas y tablas de resultados

Estado: el Capítulo 5 ya contiene placeholders editoriales para tablas y gráficas. Siguen faltando los datos reales, pero la estructura narrativa y la ubicación final de cada artefacto ya quedaron fijadas.

- [x] Cap. 5 - Tabla final KPIs técnicos
- [x] Cap. 5 - Tabla FPS por dispositivo
- [x] Cap. 5 - Tabla SUS detallada por participante
- [x] Cap. 5 - Gráfica SUS
- [x] Cap. 5 - Tabla NASA-TLX por dimensión y participante
- [x] Cap. 5 - Gráfica NASA-TLX comparativa
- [x] Cap. 5 - Tabla \textit{Think-Aloud}

### 1.4 Tablas editoriales de correlación y cierre

Estado: ya quedaron insertadas en el informe final para sostener coherencia entre propuesta, informe y artefactos activos del proyecto.

- [x] Tabla Resumen de artefactos activos del cierre
- [x] Tabla Inventario sintético de módulos y estado de integración
- [x] Tabla Productos entregados vs. esperados
- [x] Tabla de correlación entre tablas de la propuesta y el informe final
- [x] Tabla pipeline 3D al cierre documental

### 1.5 Ubicación editorial de los diagramas

Estado: estable y ya enlazado en el informe.

- [x] `figures/chapter4/fig_4_1.pdf`
- [x] `figures/chapter4/fig_4_2.pdf`
- [x] `figures/chapter4/fig_4_3.pdf`
- [x] `figures/chapter4/fig_4_4.pdf`
- [x] `figures/chapter4/fig_4_5.pdf`
- [x] `figures/chapter4/fig_4_6.pdf`
- [x] `figures/chapter4/fig_4_7.pdf`
- [x] `figures/chapter4/fig_4_8.pdf`
- [x] `figures/chapter4/fig_4_9.pdf`
- [x] `figures/chapter4/fig_4_10.pdf`
- [x] `figures/chapter4/fig_4_11.pdf`
- [x] `figures/chapter4/fig_4_12.pdf`
- [x] `figures/chapter4/fig_4_13.pdf`

## 2) Lo que aún falta de verdad

1. Sustituir los 13 placeholders de screenshots/renders por capturas reales de la build congelada.
2. Diligenciar con datos reales las 7 reservas editoriales del Capítulo 5.
3. Rerenderizar los diagramas 4.1-4.13 desde la fuente Mermaid ya corregida a versión clara y reauditar su legibilidad en PDF.
4. Ejecutar una pasada final de consistencia sobre lista de figuras, lista de tablas, referencias cruzadas y numeración.

## 3) Priorización recomendada

- P0: capturar datos reales y cerrar tablas/gráficas SUS-NASA-KPI.
- P1: reemplazar placeholders de screenshots por capturas reales.
- P2: rerenderizar la variante clara ya preparada en los archivos `.mmd` y verificar contraste, tamaño de fuente y legibilidad impresa.
- P3: verificación final de listas, captions, referencias cruzadas y paginación.

## 4) Criterios de cierre

Se considera completo cuando:

- [x] La lista de figuras contiene 4.1-4.13 y los placeholders editoriales adicionales ya reservados.
- [ ] La lista de tablas no tiene placeholders ni campos pendientes.
- [ ] El Capítulo 5 no contiene reservas editoriales y ya fue diligenciado con datos reales.
- [ ] Todas las referencias `\ref` y captions compilan sin warnings críticos.
- [ ] Las figuras de captura y los diagramas fueron auditados en su versión visual definitiva.
- [ ] Los PDFs exportados desde Mermaid reflejan la paleta clara y las correcciones semánticas aplicadas en la fuente `.mmd`.
