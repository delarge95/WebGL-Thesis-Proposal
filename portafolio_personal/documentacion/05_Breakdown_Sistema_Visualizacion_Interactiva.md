# Breakdown: Sistema de Visualizacion Interactiva

## Que se debe mostrar

La pieza central del portafolio es el visor final del Holybro X500 V2. Su valor esta en que integra:

- navegacion 3D;
- seleccion de piezas;
- ficha tecnica en bottom sheet;
- modos `Inspect / Analyze / Studio`;
- view modes y thermal legend;
- arquitectura de soporte para mantener coherencia entre UI y escena.

## Flujo real de producto

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

## Features defendibles

- seleccion e inspeccion de componentes reales;
- hotspots, isolate y power/load;
- explode, cut y filtros de categoria;
- Studio con `X-Ray`, `Solid Color` y `Thermal`;
- leyenda termica visible;
- Hero saneado y enlazado al repositorio.

## Features que no deben salir como final

- catalogo visible;
- settings visibles;
- medicion visible;
- atajos de teclado no verificados;
- audio como parte del producto cerrado.

## Mensaje central

El valor no es solo visual. El visor resuelve una lectura tecnica del ensamblaje y demuestra capacidad para conectar UI, runtime, shaders y arquitectura en una sola experiencia web.
