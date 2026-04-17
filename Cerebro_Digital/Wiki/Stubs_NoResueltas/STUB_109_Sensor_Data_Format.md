---
tipo: nota_consolidada
area: trazabilidad
estado: borrador
trace_id: STUB-NR-109
aliases:
  - "Sensor_Data_Format.md"
resumen: "Formato de datos de sensores para asegurar parseo, trazabilidad y consumo estable"
---

# Sensor Data Format

## Proposito

Especifica el formato de datos de sensores para evitar ambiguedades al registrar, transmitir y consumir mediciones.

## Campos esperados

- Identificador de sensor.
- Timestamp y unidad.
- Valor o vector de medicion.
- Estado de validez o error.

## Criterios

1. El formato debe ser estable y versionable.
2. Cada campo debe tener definicion clara.
3. Debe permitir validacion automatica basica.
4. Relacionar con calibracion y controladora.

## Relacion con el resto del grafo

- `MOC_Consolidacion_Drone`
- `Sensor Calibration Guide`
- `GPS Compass Specs`
