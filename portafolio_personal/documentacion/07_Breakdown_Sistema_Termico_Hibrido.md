# Breakdown: Sistema Termico Hibrido

## Posicionamiento correcto

Esta pieza debe presentarse como visualizacion tecnica aplicada, no como FEA ni termografia validada experimentalmente.

## Elementos defendibles

- `ThermalSimulationManager`
- `ThermalViewController`
- `ThermalContactGraphAsset`
- `ThermalContactGraphBuilderWindow`
- leyenda termica visible en la UI
- uso de un grafo explicito de contactos

## Mensaje tecnico

El sistema termico aporta valor porque traduce un estado operativo del dron a una lectura visual util dentro de las restricciones de WebGL. La fortaleza esta en la arquitectura y en la integracion con el visor, no en sobreprometer simulacion fisica de alta fidelidad.

## Lo que debe quedar claro

- es un sistema de visualizacion aplicada;
- prioriza legibilidad y consistencia visual;
- `ThermalTestSetup.cs` no es la ruta oficial del producto final;
- la calibracion experimental queda para trabajo futuro.
