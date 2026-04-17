---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-088
aliases:
  - "Part_List_Database.md"
resumen: "Base de datos de piezas para controlar inventario, estados y relaciones"
---

# Part List Database

## Proposito

Organiza el inventario de piezas del dron para saber que existe, donde se usa y que relaciones mantiene con el resto del ensamblaje.

## Campos utiles

- Identificador de pieza.
- Categoria o familia.
- Estado de version o revision.
- Relacion con documentos tecnicos.
- Observaciones de montaje o sustitucion.

## Criterios

1. La base de datos debe ser consistente con las fichas tecnicas.
2. Cada pieza debe poder rastrearse a su fuente.
3. No duplicar componentes sin motivo.
4. La estructura debe servir para mantenimiento y comparacion.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `DronePartDataFixer`
- `Flight_Controller_PCB.md`
