# Cronograma del Proyecto
## TwinSight X500 - Diagrama Gantt

> **Nota de estado 2026-05-08:** este cronograma es un artefacto de planificacion historica. No define el alcance visible actual de la app. Para el estado real usar el informe final, manuales, `PAQUETE_DE_ENTREGA.md`, `Documentacion_Tecnica/07_Guia_Demo.md` y el paquete de presentacion actualizado. Elementos como "7 modos", measurement, BOM o catalogo deben interpretarse como planeacion/evaluacion historica si no aparecen en la UI final.

---

## Resumen Ejecutivo

| Aspecto | Detalle |
|---------|---------|
| **Duración Total** | 24 semanas (6 meses) |
| **Fecha Inicio** | Julio 2025 |
| **Fecha Fin** | Diciembre 2025 |
| **Metodología** | Design Science Research + Scrum |
| **Sprints** | 12 sprints de 2 semanas |

---

## Diagrama Gantt (ASCII)

```
2024            JUL        AGO        SEP        OCT        NOV        DIC
Semana          1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
                |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
                
FASE 1: INVESTIGACIÓN Y PLANIFICACIÓN
├─ Revisión literatura    ████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
├─ Análisis estado arte   ░░░░████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
├─ Marco teórico          ░░░░░░░░████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
├─ Propuesta inicial      ░░░░░░░░░░░░████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
└─ Aprobación propuesta   ░░░░░░░░░░░░░░██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

FASE 2: PIPELINE 3D
├─ Modelado base Blender  ░░░░░░░░░░░░░░░░████████░░░░░░░░░░░░░░░░░░░░░░░░
├─ Retopología           ░░░░░░░░░░░░░░░░░░░░████████░░░░░░░░░░░░░░░░░░░░
├─ UV Mapping            ░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░░░░░░░░░░░░░
├─ Baking de normales    ░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░░░░░░░░░░░
└─ Exportación FBX       ░░░░░░░░░░░░░░░░░░░░░░░░░░░░██░░░░░░░░░░░░░░░░░░

FASE 3: DESARROLLO UNITY - CORE
├─ Configuración proyecto ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░░░░░░░
├─ Arquitectura base     ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████████░░░░░░░░░░
├─ Sistema de selección  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░░░
├─ Cámara orbital        ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░░░
├─ EventBus sistema      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░
└─ UI Toolkit base       ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░░░░░

FASE 4: DESARROLLO UNITY - FEATURES
├─ Vista explosionada    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████░░░░
├─ 7 Modos visualización ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████████
├─ Shaders HLSL (7)      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██████
├─ Corte transversal     ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████
├─ Catálogo de partes    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████
└─ Simulación drone      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██

FASE 5: HERRAMIENTAS INGENIERÍA
├─ Guía de ensamblaje    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
├─ Herramienta medición  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
├─ Puntos de conexión    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
├─ BOM Export            ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
└─ Anotaciones 3D        ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██

FASE 6: TESTING Y VALIDACIÓN
├─ Pruebas unitarias     ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████
├─ Pruebas usabilidad    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
├─ Medición NASA-TLX     ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
├─ Ajustes finales       ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
└─ Build WebGL final     ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██

FASE 7: DOCUMENTACIÓN
├─ Manual técnico        ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████████░░
├─ Manual usuario        ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████████
├─ Informe final         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██████
└─ Presentación          ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░████

HITOS CLAVE
├─ Propuesta aprobada    ░░░░░░░░░░░░░░▲░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  S7
├─ Pipeline 3D completo  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░▲░░░░░░░░░░░░░░░░░░░  S14
├─ Core funcional        ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▲░░░░░░░░░░░  S18
├─ Features completas    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▲░░░░░  S22
└─ Entrega final         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▲░  S24
```

---

## Detalle por Fase

### Fase 1: Investigación y Planificación (Semanas 1-8)

| Actividad | Sem 1-2 | Sem 3-4 | Sem 5-6 | Sem 7-8 | Entregable |
|-----------|---------|---------|---------|---------|------------|
| Revisión literatura | ████ | ░░░░ | ░░░░ | ░░░░ | Estado del arte |
| Análisis estado arte | ░░░░ | ████ | ░░░░ | ░░░░ | Matriz comparativa |
| Marco teórico | ░░░░ | ░░░░ | ████ | ░░░░ | Fundamentación |
| Propuesta proyecto | ░░░░ | ░░░░ | ░░██ | ██░░ | Documento propuesta |
| Revisión y ajustes | ░░░░ | ░░░░ | ░░░░ | ░░██ | Propuesta aprobada |

**Horas dedicadas:** ~120 horas
**Reuniones con tutor:** 4 sesiones

---

### Fase 2: Pipeline 3D (Semanas 9-14)

| Actividad | Sem 9-10 | Sem 11-12 | Sem 13-14 | Entregable |
|-----------|----------|-----------|-----------|------------|
| Modelado base | ████ | ░░░░ | ░░░░ | Modelo high-poly |
| Retopología | ░░██ | ████ | ░░░░ | Modelo low-poly |
| UV Mapping | ░░░░ | ░░██ | ██░░ | UVs optimizados |
| Baking | ░░░░ | ░░░░ | ████ | Normal maps |
| Export FBX | ░░░░ | ░░░░ | ░░██ | Assets Unity-ready |

**Horas dedicadas:** ~80 horas
**Software:** Blender 4.0

---

### Fase 3: Desarrollo Unity - Core (Semanas 15-18)

| Actividad | Sem 15 | Sem 16 | Sem 17 | Sem 18 | Entregable |
|-----------|--------|--------|--------|--------|------------|
| Config proyecto | ██ | ░░ | ░░ | ░░ | Proyecto URP |
| Arquitectura | ██ | ██ | ░░ | ░░ | Singleton, EventBus |
| Selección | ░░ | ██ | ██ | ░░ | SelectionManager |
| Cámara | ░░ | ░░ | ██ | ██ | OrbitCamera |
| UI base | ░░ | ░░ | ░░ | ██ | UI Toolkit setup |

**Horas dedicadas:** ~100 horas
**Commits:** ~50

---

### Fase 4: Desarrollo Unity - Features (Semanas 19-22)

| Actividad | Sem 19 | Sem 20 | Sem 21 | Sem 22 | Entregable |
|-----------|--------|--------|--------|--------|------------|
| Vista explosionada | ██ | ░░ | ░░ | ░░ | ExplodedViewManager |
| Modos visualización | ██ | ██ | ██ | ░░ | ViewModeManager |
| 7 Shaders HLSL | ░░ | ██ | ██ | ██ | Shaders WebGL |
| Corte transversal | ░░ | ░░ | ██ | ██ | CrossSectionManager |
| Features adicionales | ░░ | ░░ | ░░ | ██ | Catálogo, simulación |

**Horas dedicadas:** ~120 horas
**Commits:** ~80

---

### Fase 5: Herramientas de Ingeniería (Semanas 22-23)

| Actividad | Sem 22 | Sem 23 | Entregable |
|-----------|--------|--------|------------|
| Guía ensamblaje | ██ | ░░ | AssemblyGuideManager |
| Medición | ██ | ░░ | MeasurementTool |
| Conexiones | ░░ | ██ | ConnectionPointsViewer |
| BOM Export | ░░ | ██ | BillOfMaterialsManager |
| Anotaciones | ░░ | ██ | AnnotationSystem |

**Horas dedicadas:** ~40 horas

---

### Fase 6: Testing y Validación (Semanas 23-24)

| Actividad | Sem 23 | Sem 24 | Entregable |
|-----------|--------|--------|------------|
| Tests unitarios | ██ | ░░ | Test reports |
| Usabilidad SUS | ░░ | ██ | Resultados SUS |
| NASA-TLX | ░░ | ██ | Análisis carga cognitiva |
| Ajustes | ░░ | ██ | Versión final |
| Build WebGL | ░░ | ██ | Deploy producción |

**Horas dedicadas:** ~30 horas
**Participantes validación:** 8-12

---

### Fase 7: Documentación (Semanas 20-24, paralelo)

| Documento | Sem 20-21 | Sem 22-23 | Sem 24 | Entregable |
|-----------|-----------|-----------|--------|------------|
| Manual técnico | ████ | ░░░░ | ░░ | 65+ páginas |
| Manual usuario | ░░██ | ████ | ░░ | 40+ páginas |
| Informe final | ░░░░ | ██░░ | ██ | Documento completo |
| Presentación | ░░░░ | ░░██ | ██ | 20 slides |

**Horas dedicadas:** ~60 horas

---

## Distribución de Horas

| Fase | Horas | Porcentaje |
|------|-------|------------|
| Investigación | 120 | 22% |
| Pipeline 3D | 80 | 15% |
| Desarrollo Core | 100 | 18% |
| Features | 120 | 22% |
| Herramientas | 40 | 7% |
| Testing | 30 | 6% |
| Documentación | 60 | 11% |
| **Total** | **550** | **100%** |

---

## Sprints Scrum (12 sprints × 2 semanas)

| Sprint | Semanas | Objetivo | Story Points |
|--------|---------|----------|--------------|
| 1 | 1-2 | Revisión literatura | 13 |
| 2 | 3-4 | Estado del arte | 13 |
| 3 | 5-6 | Marco teórico | 13 |
| 4 | 7-8 | Propuesta | 8 |
| 5 | 9-10 | Modelado 3D | 21 |
| 6 | 11-12 | Retopología + UV | 21 |
| 7 | 13-14 | Baking + Export | 13 |
| 8 | 15-16 | Arquitectura Unity | 21 |
| 9 | 17-18 | Core systems | 21 |
| 10 | 19-20 | View modes + Shaders | 34 |
| 11 | 21-22 | Features + Tools | 34 |
| 12 | 23-24 | Testing + Entrega | 21 |

**Velocity promedio:** 19 story points/sprint

---

## Riesgos Gestionados

| Riesgo | Mitigación | Semana Detectado |
|--------|------------|------------------|
| Complejidad shaders WebGL | Investigación previa, fallbacks | S19 |
| Performance móvil | Optimización continua, LODs | S20 |
| Tiempo validación | Muestra reducida pero significativa | S22 |
| Dependencias externas | Desarrollo modular | S15 |

---

## Notas del Desarrollo

### Bitácora Resumida

- **Julio 2024**: Inicio investigación, revisión de 40+ papers
- **Agosto 2024**: Consolidación marco teórico, propuesta v1
- **Septiembre 2024**: Pipeline 3D completo, primeros assets
- **Octubre 2024**: Core Unity funcional, 30+ scripts
- **Noviembre 2024**: Features avanzadas, 7 shaders, 60+ scripts
- **Diciembre 2024**: Testing, documentación, entrega final

### Herramientas de Seguimiento

- **Control de versiones**: Git + GitHub
- **Project management**: Notion (Kanban)
- **Comunicación**: Correo institucional + Google Meet
- **Documentación**: LaTeX + Markdown

---

*Cronograma Version: 1.0*
*Actualizado: Diciembre 2024*
*Proyecto: TwinSight X500 - UNAD*
