---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-067
aliases:
  - "Inspect_Panel_Data_Model.md"
resumen: "Modelo de datos para un panel de inspeccion con atributos, metadatos y acciones"
---

# Inspect Panel Data Model

## Proposito

Define la estructura minima que necesita un panel de inspeccion para mostrar propiedades, origen, relaciones y estado de la entidad activa.

## Campos utiles

- Identificador de objeto o pieza.
- Jerarquia y dependencia dentro del ensamblaje.
- Estado visual: visible, ghosted, isolate o highlighted.
- Metadatos tecnicos relevantes para la inspeccion.
- Acciones disponibles sobre el elemento seleccionado.

## Criterios

1. Separar datos de presentacion para no mezclar estado visual con dominio.
2. Mantener el modelo estable aunque cambie la vista activa.
3. Permitir extension sin romper consumos previos.
4. Documentar cualquier campo que impacte trazabilidad o validacion.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `MOC_UX_UI_Complete`
- `Selection_System.md`
