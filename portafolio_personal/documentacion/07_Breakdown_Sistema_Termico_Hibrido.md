# Breakdown - Sistema Termico Hibrido

## Valor para Portafolio

Esta pieza ya tiene valor real de portafolio porque combina:

- simulacion aplicada a producto interactivo
- criterio de optimizacion WebGL movil
- shader authoring orientado a visualizacion tecnica
- integracion entre fisica reducida, UX, editor tooling y documentacion academica

## Que mostrar

- arquitectura del solver por componentes
- paso del dato termico al shader
- slider de carga sostenida y estados del dron
- `ThermalContactGraphBuilderWindow` como workflow de authoring offline
- comparativa entre fallback heuristico y grafo precomputado
- workflow de verificacion con WolframAlpha

## Material fuente actual

- `desarrollo/docs/sistema_termico/README.md`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/`
- `desarrollo/unity_project/Assets/Scripts/Editor/Thermal/ThermalContactGraphBuilderWindow.cs`
- `desarrollo/unity_project/Assets/Shaders/Thermal.shader`

## Estado

Ya puede presentarse como breakdown tecnico en construccion: existe arquitectura clara, contrato de datos termico, integracion con UI/estado del dron y una herramienta editor para el grafo de contactos. Todavia conviene esperar la calibracion del asset sobre la geometria final y una leyenda termica visible en UI antes de convertirlo en showcase principal.