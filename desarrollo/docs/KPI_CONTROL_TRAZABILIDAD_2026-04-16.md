---
tipo: reporte_kpi
area: trazabilidad
estado: activo
trace_id: TRC-KPI-2026-04-16
resumen: "Cierre de fase con KPIs K1-K5 del plan de trazabilidad"
---

# KPI de Control - Trazabilidad (Paso 7)

Fecha de corte: 2026-04-16

## Metodologia de medicion

- K1: notas clave con `trace_id` sobre total de notas clave de trazabilidad.
- K2: cobertura de secciones del TOC en la matriz completa.
- K3: cobertura de scripts core con `script_card` (definicion original) y cobertura de catalogo total (definicion operativa).
- K4: bib_keys mapeadas con estado en el registro maestro.
- K5: nodos activos con 2 o mas enlaces semanticos.

## Resultados

| KPI                     | Formula                            | Resultado                | Estado   |
| ----------------------- | ---------------------------------- | ------------------------ | -------- |
| K1                      | 66 / 125                           | 52.8%                    | Amarillo |
| K2                      | 130 / 129                          | 100.0% (cobertura total) | Verde    |
| K3 (original)           | 20 / 61                            | 32.8%                    | Amarillo |
| K3 (operativo catalogo) | 137 / 137 scripts C# inventariados | 100.0%                   | Verde    |
| K4                      | 42 / 42                            | 100.0%                   | Verde    |
| K5                      | 34 / 37                            | 91.9%                    | Verde    |

## Lectura de cierre de fase

- Fase A: cubierta funcionalmente, con brecha residual en normalizacion completa de `trace_id` (K1).
- Fase B: cubierta de forma completa a nivel de secciones del informe (K2).
- Fase C: catalogo completo logrado; cobertura de `script_card` aun focalizada en scripts criticos (K3 original en progreso).
- Fase D: registro bibliografico completo con estado por fuente (K4).
- Fase E: paquetes de contexto RAG operativos y trazables; densidad de grafo en nivel alto (K5).

## Artefactos vinculados

- [[Cerebro_Digital/Wiki/Concepts/CATALOGO_SCRIPTS_UNITY_COMPLETO]]
- [[Cerebro_Digital/Wiki/Concepts/MATRIZ_TRAZABILIDAD_ENTREGABLES_COMPLETA]]
- [[Cerebro_Digital/Wiki/Concepts/MOC_RAG_Context_Packages]]
- [[Cerebro_Digital/Wiki/Concepts/REGISTRO_FUENTES_BIBLIOGRAFICAS]]
