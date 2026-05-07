# Plan de Acción Final (Actualizado 2026-05-06)

Este documento contiene la lista lineal y secuencial de las tareas pendientes para el cierre definitivo del proyecto de tesis. Se divide en tareas inmediatas (saneamiento 3D y re-importación) y tareas futuras (validación, documentación y sustentación).

## --Para mañana--

1. [ ] **Primitivas de Tornillería:** Reemplazar tuercas y tornillería con primitivas básicas. Se harán en dos nuevas colecciones: `PRIMITIVE_FASTENER_MASTERS` y `PRIMITIVE_FASTENER_INSTANCES`. Esta será la geometría que se exporte junto al resto del dron a Unity y esas piezas son las que se reemplazarán con el sistema modular cuando se aíslen o seleccionen en Unity.
2. [ ] **Unwrap:** Unwrap teniendo en cuenta los grupos de materiales.
3. [ ] **Bake:** Bake de mapas normales, curvatura y AO (Marmoset Toolbag).
4. [ ] **Texturizado Rápido:** Creación de materiales PBR directamente en Blender (por ahora; si queda tiempo se reharán luego con mayor fidelidad en Substance Painter).
5. [ ] **Validación de Código:** Verificar que las piezas 3D (nombres y jerarquías) coinciden milimétricamente con lo que espera el código en Unity.
6. [ ] **Export/Import Core:** Exportar e importar a Unity los masters y las instancias del dron principal.
7. [ ] **Modelado Modular:** Modelar los módulos "high-detail" definitivos para los fasteners.
8. [ ] **Export/Import Fasteners:** Exportar e importar los módulos a Unity para acoplarlos al sistema de fasteners modulares en código.
9. [ ] **Estructura en Runtime:** Asegurar la correcta agrupación de piezas, subpiezas, hotspots y comportamiento de los filtros en la app.

## --Tareas futuras--

10. [ ] **Explode View:** Corregir vista explosiva en la app (ajustar offsets y anclajes tras el reimport).
11. [ ] **QA App:** Chequeo completo de toda la funcionalidad de la aplicación.
12. [ ] **QA Settings:** Chequeo completo de las configuraciones del proyecto WebGL en Unity.
13. [ ] **Encuestas:** Revisar el formato de las encuestas (SUS, NASA-TLX) y añadir un acceso directo a ellas en el menú de la app.
14. [ ] **Freeze & Metrics:** Congelar Build final y medir métricas (FPS, Draw Calls, Memoria, Carga).
15. [ ] **Despliegue:** Subir la página estática final a GitHub Pages.
16. [ ] **Pruebas de Usabilidad:** Ejecutar sesiones con usuarios.
17. [ ] **Inventario Visual:** Organizar lista de capturas de pantalla, listas de diagramas, y figuras faltantes en el informe.
18. [ ] **Generación Gráfica:** Tomar las capturas definitivas y generar gráficas de las encuestas/métricas.
19. [ ] **Integración Documental:** Añadir las imágenes y gráficas a LaTeX (reemplazando los placeholders en el capítulo 4 y 5).
20. [ ] **Revisión LaTeX:** Revisión exhaustiva del informe final (Undefined Refs, formato, leyendas).
21. [ ] **Preparación Oratoria:** Pulir el guión de sustentación, actualizar las auditorías simuladas de agentes actuando como jurado y preparar defensas para posibles preguntas.
22. [ ] **Diseño Slides:** Crear las diapositivas base.
23. [ ] **Media Slides:** Crear animaciones e imágenes de alto impacto para las diapositivas.
24. [ ] **Ensayo General:** Preparar la sustentación y ensayar hasta quedarse sin voz.
