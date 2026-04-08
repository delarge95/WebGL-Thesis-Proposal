# Thermal documentation maintenance workflow

Use this workflow whenever the drone thermal simulation feature changes.

## Purpose

Keep code, thesis, manuals, technical docs, portfolio materials, and handoff artifacts aligned without overstating implementation status.

## Source of truth

- `desarrollo/docs/sistema_termico/README.md`
- `desarrollo/docs/sistema_termico/AGENT_HANDOFF_THERMAL.md`
- `desarrollo/docs/sistema_termico/RETOPOLOGIA_POR_PIEZA.md`
- `desarrollo/docs/sistema_termico/MATRIZ_ACTUALIZACION_DOCUMENTAL.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`

## Update policy

### Every meaningful iteration

Update:

- `Informe_final/Desarrollo_App/BITACORA.md`
- `Informe_final/Desarrollo_App/CHANGELOG.md`
- thermal docs in `desarrollo/docs/sistema_termico/`
- thermal technical reference in `Informe_final/Desarrollo_App/Documentacion_Tecnica/`

### Only when stable enough to teach or maintain

Update:

- `Informe_final/Manual_tecnico/manual_tecnico.tex`
- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/Desarrollo_App/TECHNOLOGY_STACK.md`
- `portafolio_personal/README.md`
- `portafolio_personal/documentacion/07_Breakdown_Sistema_Termico_Hibrido.md`

### Only when visible in build and usable by end users

Update:

- `Informe_final/Manual_de_usuario/manual_usuario.tex`

### Only with measured evidence

Update:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/chapters/06_conclusiones.tex`

## Honesty rule

Always label the subsystem as one of:

- foundation
- partial integration
- active development
- validated prototype
- experimental

Never claim full thermodynamic simulation, CFD, or full FEA unless that is genuinely implemented and supported by evidence.

## Thermal modeling rule

- Official runtime granularity: 28 canonical parts.
- CAD/Blender detail layer: 55 subcomponents for modeling and import guidance.
- `ThermalTestSetup.cs` is experimental and never part of the official path.

## Math verification rule

Before freezing formulas, constants, or conversions into docs or code:

1. normalize the equation with units
2. verify with the Wolfram workflow
3. record the query and the result trail
4. only then write the value into code or docs