---
tipo: auditoria_incongruencias
area: sustentacion + documentacion + runtime
estado: pendiente_de_resolucion
fuente: README + runtime Unity + INF_EST_51/70 + deep research
---

# INF_EST_79 - Incongruencias de documentación y guión por resolver

Este documento registra las incongruencias detectadas entre la documentación, el runtime real de la app y las versiones del guión de sustentación. La intención no es criticar, sino dejar trazable qué debe unificarse antes de cerrar la versión final.

## 1. Nomenclatura de modos de interacción

- **Incongruencia:** En el runtime real, `UIModeController` trabaja con `Tools`, `Analyze` y `Studio`, mientras que el guión habla de `Inspect`, `Analyze` y `Studio`.
- **Impacto:** Puede confundir al jurado si pregunta por la arquitectura real de la interfaz.
- **Resolución sugerida:** Aclarar explícitamente que `Inspect` es el nombre narrativo del modo `Tools`.

## 2. Estado global de la app vs nombres narrativos

- **Incongruencia:** `AppStateMachine` maneja `Loading`, `Intro`, `Exploration`, `ExplodedView`, `FocusMode`, `Settings`, `Menu`, `Analyze` y `Studio`, pero el guión simplifica la experiencia a tres bloques principales.
- **Impacto:** El jurado puede interpretar que el sistema solo tiene tres estados.
- **Resolución sugerida:** Mencionar que la defensa usa tres macro-modos de trabajo, pero la app real tiene una máquina de estados más amplia.

## 3. Modos de visualización incompletos en el guión

- **Incongruencia:** El runtime define siete modos de visualización: `Realistic`, `XRay`, `Blueprint`, `SolidColor`, `Wireframe`, `Ghosted` y `Thermal`.
- **Impacto:** En el guión anterior solo se listaban cinco, omitiendo `Wireframe` y `Ghosted`.
- **Resolución sugerida:** Dejar siete modos en el guión y explicar que algunos son complementarios para lectura estructural.

## 4. Herramientas técnicas no reflejadas por completo

- **Incongruencia:** El proyecto sí implementa `AssemblyGuideManager`, `ConnectionPointsViewer`, `BillOfMaterialsManager`, `AnnotationSystem`, `MeasurementTool` y `AssemblyChecklist`, pero el guión aún no los integra con suficiente claridad.
- **Impacto:** El guión subrepresenta una parte importante de la lógica de reducción de fricción y apoyo técnico.
- **Resolución sugerida:** Incluirlas como herramientas reales de soporte, no solo como futuros posibles.

## 5. Modo térmico: interpretación metodológica

- **Incongruencia:** La documentación y el runtime muestran un `ThermalSimulationManager` y un `ThermalViewController`, pero el guión debe evitar que parezca FEA o simulación física exacta.
- **Impacto:** Riesgo de sobreafirmación técnica.
- **Resolución sugerida:** Mantener la fórmula de presentación como heurística visual y decir explícitamente que no sustituye análisis térmico de ingeniería.

## 6. Selección y aislamiento con fasteners

- **Incongruencia:** La selección real tiene comportamiento específico para fasteners: primer clic puede llevar al padre canónico y el segundo clic puede descender al fastener, con estados apilados de aislamiento.
- **Impacto:** Si el guión resume demasiado, puede parecer que la selección es simplemente pieza simple -> aislamiento simple.
- **Resolución sugerida:** Explicar que el sistema distingue madre, subpieza y fastener, y que el aislamiento conserva contexto asociativo.

## 7. Hotspots y aislamiento asociado

- **Incongruencia:** Los hotspots pueden incluir fasteners asociados y el aislamiento puede apilarse con estados previos.
- **Impacto:** Si se omite, el jurado puede pensar que el sistema rompe contexto al aislar.
- **Resolución sugerida:** Mantener en el guión la idea de aislamiento contextual, no aislamiento destructivo.

## 8. Alcance del proyecto frente a la documentación pública

- **Incongruencia:** El README del repositorio describe más herramientas y modos que los que suelen aparecer en un resumen oral corto.
- **Impacto:** El guión puede quedar corto si no selecciona qué capacidades sí deben mostrarse como parte central de la tesis.
- **Resolución sugerida:** Elegir un subconjunto defendible: selección, aislamiento, modos visuales, corte, explode, thermal, guía de ensamble y medición/BOM.

## 9. Métricas finales todavía sujetas a freeze

- **Incongruencia:** El guión mantiene placeholders para FPS, peso del build, tiempo de carga, conteo de piezas, conteo de fasteners y validación de uso.
- **Impacto:** No es un error, pero sí un punto de cierre pendiente.
- **Resolución sugerida:** Reemplazar placeholders solo cuando exista evidencia final congelada.

## 10. Referencia a instrucciones 3D industriales

- **Incongruencia:** La idea se quiere dejar como inspiración implícita, no como comparación de marca o benchmark literal.
- **Impacto:** Si se nombra demasiado explícitamente, puede sonar a referencia comercial no necesaria.
- **Resolución sugerida:** Mantenerla como inspiración conceptual: guías 3D narrativas, paso a paso, para hardware complejo.

## Lista de cierre pendiente

- [ ] Unificar `Inspect` vs `Tools` en todo el material.
- [ ] Alinear el número de modos visuales entre guión, README y demo.
- [ ] Insertar herramientas de soporte técnico en el bloque de aporte/arquitectura del guión.
- [ ] Revisar si alguna afirmación de thermal puede interpretarse como simulación física exacta.
- [ ] Confirmar que el demo en vivo incluya al menos un ejemplo de guía de ensamble, BOM o puntos de conexión.
- [ ] Sustituir placeholders finales solo tras medición congelada.

## Estado actual

- **Guión 70:** ya actualizado para integrar la motivación de pereza productiva y la referencia implícita a guías 3D.
- **Pendiente siguiente:** validar que la versión oral conserve el balance entre lo humano, lo técnico y lo verificable.
