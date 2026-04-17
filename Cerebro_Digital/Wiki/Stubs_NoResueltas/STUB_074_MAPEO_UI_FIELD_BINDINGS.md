---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-074
aliases:
  - "MAPEO_UI_FIELD_BINDINGS"
resumen: "Mapeo de campos UI a datos para mantener sincronizacion entre vista y modelo"
---

# Mapeo UI Field Bindings

## Proposito

Relaciona campos de UI con datos de dominio para asegurar que los cambios en vista y modelo permanezcan sincronizados.

## Campos relevantes

- Nombre de campo.
- Fuente de datos.
- Regla de transformacion.
- Dependencias de refresco.

## Criterios

1. El mapeo debe ser facil de auditar.
2. No ocultar conversiones importantes.
3. Debe servir para depurar bindings.
4. La nota debe conectarse con el panel de propiedades.

## Relacion con el resto del grafo

- `MOC_Consolidacion_UX_UI`
- `DataBinding System`
- `Property Panel Data Flow`
