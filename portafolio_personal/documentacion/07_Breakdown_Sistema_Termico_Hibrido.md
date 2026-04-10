# Breakdown - Sistema termico hibrido

## Valor para portafolio

Esta pieza ya tiene valor real de portafolio porque combina:

- simulacion aplicada a producto interactivo,
- criterio de optimizacion WebGL movil,
- shader authoring orientado a visualizacion tecnica,
- integracion entre solver, UI, tooling editor y documentacion,
- y una decision de arquitectura honesta entre realismo y costo computacional.

## Que mostrar

- solver por 28 piezas canonicas,
- grafo de contactos explicito (`ThermalCanonicalContactGraph.asset`),
- presets visuales por pieza critica,
- leyenda termica visible en UI,
- comparativa entre fallback heuristico y camino oficial con grafo,
- guia de retopologia 28+55,
- workflow de verificacion con WolframAlpha.

## Material fuente actual

- `desarrollo/docs/sistema_termico/README.md`
- `desarrollo/docs/sistema_termico/RETOPOLOGIA_POR_PIEZA.md`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/`
- `desarrollo/unity_project/Assets/Resources/ThermalCanonicalContactGraph.asset`
- `desarrollo/unity_project/Assets/Scripts/Editor/Thermal/ThermalContactGraphBuilderWindow.cs`
- `desarrollo/unity_project/Assets/Shaders/Thermal.shader`

## Estado

Ya puede presentarse como breakdown tecnico serio en desarrollo activo. La base oficial existe, la leyenda ya esta integrada y el grafo canonico ya fue fijado. Lo que sigue antes de volverlo showcase principal es validar la escena final retopologizada y capturar material visual convincente con el dron definitivo.

## Nota importante

`ThermalTestSetup.cs` no debe venderse como pipeline oficial. Es un harness experimental para CAD bruto y datos dummy.

## Actualizacion 2026-04-10

Para el breakdown final conviene mostrar tambien:

- el refinamiento del shader hacia un shimmer termico mas sutil y mas cinematografico,
- la decision de mantener la jerarquia termica en ensamblajes canonicos aunque el modelo aumente en subpiezas,
- y el criterio de no dejar que el detalle extra del FBX destruya la legibilidad del modo Thermal.
