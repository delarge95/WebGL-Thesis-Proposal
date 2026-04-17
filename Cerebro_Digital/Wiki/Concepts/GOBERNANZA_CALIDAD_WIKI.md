---
tipo: "gobernanza"
fuente: "vault-global"
estado: "activo"
area: "trazabilidad"
trace_id: "TRC-GOV-0001"
---

# Gobernanza de Calidad Wiki

## Objetivo

Evitar degradacion de la red de conocimiento y mantener cierre sostenible de los frentes A, B, C y D.

## KPI de control

- K-Adj-1: porcentaje de adjuntos con nota propietaria. Umbral >= 95.
- K-Adj-2: porcentaje de adjuntos con trace_id_rel. Umbral >= 90.
- K-Orf-1: porcentaje de notas activas huerfanas. Umbral <= 5.
- K-Res-1: enlaces no resueltos criticos de tipo nota. Umbral = 0.
- K-Qual-1: porcentaje de notas activas con frontmatter completo. Umbral >= 90.

## Reglas obligatorias

- Toda nota activa debe tener: tipo, area, estado y trace_id.
- Toda nota activa debe tener al menos 1 enlace entrante y 1 saliente.
- Toda nota activa debe tender a >= 2 enlaces salientes semanticos.
- Todo adjunto nuevo debe registrarse en REGISTRO_ADJUNTOS_GLOBAL el mismo dia.
- Para rutas tecnicas (codigo, assets, archivos con extension), preferir enlace markdown estandar y no wikilink semantico.

## Cadencia operativa

- Diario (ligero): control de K-Qual-1 y K-Orf-1 en notas activas.
- Semanal (completo): control de los 5 KPIs y actualizacion de backlog tecnico.
- Pre-entrega: corrida completa y snapshot de KPIs en documento de cierre.

## Procedimiento de degradacion

Cuando un KPI incumpla su umbral:

1. Abrir ticket de remediacion en el backlog correspondiente.
2. Corregir primero activos criticos (estado: activo).
3. Revalidar KPI afectado.
4. Registrar causa raiz y accion preventiva.

## Politica para no resueltos

- Critico: target tipo nota sin archivo/alias resoluble. Debe corregirse a cero.
- Tecnico: target de ruta/archivo en catalogos (codigo, .tex, .cs, etc.). Se permite como deuda controlada con plan de normalizacion.

## Referencias de control

- [[TAXONOMIA_METADATA_WIKI]]
- [[REGISTRO_ADJUNTOS_GLOBAL]]
- [[MOC_Conectividad_Total]]
- [[BACKLOG_NOTAS_NO_RESUELTAS_2026-04-16]]

