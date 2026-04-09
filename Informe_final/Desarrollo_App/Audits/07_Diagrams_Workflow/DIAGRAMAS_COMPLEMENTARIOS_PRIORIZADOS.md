# Diagramas Complementarios Priorizados para Informe Final

Fecha: 2026-04-07
Alcance: Complementar la carpeta 07_Diagrams_Workflow con una hoja de ruta concreta de diagramas para Cap. 4 y soporte de Cap. 5.

---

## 1) Diagnostico rapido basado en evidencia

Estado del proyecto (fuentes revisadas):

- Informe final con estructura academica activa y lista de figuras habilitada.
- Capitulo 4 con secciones tecnicas maduras y placeholder de pipeline pendiente.
- Capitulo 5 aun depende de mediciones reales y pruebas SUS/NASA-TLX.
- Manual tecnico y manual de usuario con buen detalle funcional, pero con problemas de codificacion de caracteres y fechas/versionado en algunas secciones.
- Capa app web en docs en estado hibrido (sitio estatico + base React/Vite).
- Metricas de codigo Unity verificadas por terminal: 104 scripts C#, 16938 lineas C#.

Implicacion para diagramas:

- Se pueden producir ya todos los diagramas estructurales y de flujo de Cap. 4 (sin esperar mediciones).
- Los graficos de resultados de Cap. 5 deben quedar solo como plantillas de estructura (sin datos definitivos).

---

## 2) Cobertura actual de la carpeta 07

Documentos existentes:

- diagram_research_report.md.resolved
- implementation_plan.md.resolved
- implementation_plan_diagrams.md.resolved
- MASTER_DELIVERABLES_CHECKLIST.md

Cobertura ya definida:

- Pipeline de optimizacion 3D.
- Arquitectura de software.
- Logica de subsistema termico.
- Flujo de estados de usuario.

Brecha detectada:

- Falta una priorizacion operativa por nivel de criticidad y trazabilidad explicita a secciones del informe final.
- Falta definir diagramas complementarios clave para narrativa tecnica completa (eventos, interaccion, datos, restricciones WebGL).

---

## 3) Propuesta de diagramas complementarios (nuevos)

### P0 (obligatorios para cerrar Cap. 4)

1. Diagrama de flujo EventBus y desacoplamiento

- Objetivo: mostrar publicadores, suscriptores, tipos de evento y ciclo de vida.
- Seccion destino: 4.1 / 4.2 (Flujo de comunicacion).
- Valor: hace visible el patron central de arquitectura.

2. Diagrama de maquina de estados de aplicacion (AppStateMachine)

- Objetivo: transiciones entre Loading, Exploration, PartFocus, ExplodedView, Settings, etc.
- Seccion destino: 4.1 y sistemas core.
- Valor: conecta UI, logica y control del flujo.

3. Diagrama de flujo de seleccion de pieza

- Objetivo: Input -> Raycast -> SelectionManager -> Highlight/UI/Analytics/EventBus.
- Seccion destino: Sistemas Core + UI.
- Valor: explica la interaccion principal del usuario.

4. Diagrama de arquitectura por capas (C4 simplificado)

- Objetivo: Presentacion, Logica, Servicios, Datos + runtime WebGL.
- Seccion destino: 4.1 Arquitectura del sistema.
- Valor: facilita lectura evaluativa del capitulo tecnico.

### P1 (altamente recomendados)

5. Diagrama de pipeline de shaders por modo de visualizacion

- Objetivo: mapear 7 modos visuales a shaders y parametros clave.
- Seccion destino: 4.x Modos de visualizacion + Shaders personalizados.
- Valor: justifica decision tecnica de render.

6. Diagrama del flujo de datos DronePartData

- Objetivo: ScriptableObject -> Catalogo -> Panel info -> Hotspots -> filtros.
- Seccion destino: Catalogo y gestion de piezas.
- Valor: une datos, UI e interaccion.

7. Diagrama del subsistema termico hibrido (modular)

- Objetivo: ThermalSimulationManager, ThermalViewController, ContactGraphAsset, BuilderWindow, entrada de carga.
- Seccion destino: Subsistema termico (en desarrollo activo).
- Valor: diferencia claramente alcance actual vs trabajo futuro.

8. Diagrama de restricciones WebGL y mitigaciones

- Objetivo: limitacion -> impacto -> solucion aplicada.
- Seccion destino: Restricciones de WebGL y soluciones.
- Valor: refuerza aporte ingenieril del trabajo.

### P2 (apoyo para anexos/manuales)

9. Diagrama de herramientas proyectadas de ensamblaje

- Objetivo: estado por modulo (implementado codigo / no integrado UI / futuro).
- Seccion destino: Herramientas de ensamblaje proyectadas + anexos.
- Valor: evita sobredeclarar implementacion.

10. Diagrama de despliegue y artefactos

- Objetivo: Unity Build -> WebGL template -> docs/Build -> hosting.
- Seccion destino: Manual tecnico / anexos de deployment.
- Valor: mejora mantenibilidad y replicabilidad.

---

## 4) Matriz de prioridad para ejecucion inmediata

- Sprint Diagramas A (inmediato): P0-1, P0-2, P0-3, P0-4
- Sprint Diagramas B (seguido): P1-5, P1-6, P1-7, P1-8
- Sprint Diagramas C (si hay tiempo): P2-9, P2-10

Resultado minimo viable para informe final:

- 8 diagramas (todos P0 + todos P1)

---

## 5) Trazabilidad recomendada informe <-> diagrama

- Figura 4.1: Arquitectura por capas (P0-4)
- Figura 4.2: EventBus y flujo de comunicacion (P0-1)
- Figura 4.3: Maquina de estados AppStateMachine (P0-2)
- Figura 4.4: Flujo de seleccion de pieza (P0-3)
- Figura 4.5: Pipeline de optimizacion 3D (existente en plan)
- Figura 4.6: Pipeline de shaders por modo (P1-5)
- Figura 4.7: Flujo de datos DronePartData (P1-6)
- Figura 4.8: Subsistema termico hibrido (P1-7)
- Figura 4.9: Restricciones WebGL y mitigaciones (P1-8)

Nota: las figuras de resultados (SUS, NASA-TLX, KPIs) deben quedar como estructura pendiente de datos reales en Cap. 5.

---

## 6) Criterio de calidad para pasar a Mermaid

Cada diagrama debe incluir:

- Alcance (que cubre y que no cubre).
- Nodos con nombres tecnicos reales de clases/modulos.
- Flechas con verbo de relacion (publica, suscribe, transforma, renderiza, persiste).
- Nota de estado cuando aplique: Implementado / Parcial / Proyectado.
- Version corta para informe y version extendida para anexo.

---

## 7) Entregable listo para siguiente paso

Con este documento, la carpeta 07_Diagrams_Workflow queda complementada con:

- Priorizacion ejecutable.
- Mapeo a capitulos y figuras.
- Definicion de diagrama minimo viable (8 diagramas).
- Base para iniciar la generacion de Mermaid en el siguiente ciclo.
