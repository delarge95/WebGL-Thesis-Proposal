# Verificaciones WolframAlpha

## Objetivo

Registrar las consultas de WolframAlpha usadas para validar conversiones, ecuaciones y factores geometricos del sistema termico.

## Estado actual

- El workflow local esta instalado en `.agent/skills/wolfram-thermal-verifier/`.
- La automatizacion por API queda lista para usarse cuando se configure `WOLFRAM_APP_ID`.
- Mientras no exista ese AppID, el script genera la consulta y la URL trazable de WolframAlpha.

## Consulta 001 - Factor geometrico de conduccion

Formula del solver:

```text
G_ij ~= conductionScale * A / L
```

Conversion base usada en codigo:

- `A` en `cm^2` se pasa a `m^2`
- `L` en `mm` se pasa a `m`

Consulta generada:

```text
convert (1 cm^2)/(1 mm) to m
```

Artefacto generado:

- `desarrollo/docs/sistema_termico/wolfram_query_conductance_factor.json`

Observacion:

- Esta consulta sirve para confirmar el factor geometrico base implicito en `A/L` antes de multiplicar por la escala de conduccion del material/contacto.
- La confirmacion automatica del resultado numerico queda pendiente de AppID o validacion manual en el enlace generado por el script.

## Consulta 002 - Sin nuevas constantes fisicas congeladas en el builder offline

Estado de esta iteracion:

- `ThermalContactGraphBuilderWindow` calcula candidatos por proximidad de bounds, solape proyectado y longitud de camino minima.
- En esta etapa no se fijo una nueva constante fisica global en codigo; solo se implemento authoring geometrico para el grafo termico.
- Por lo mismo, no fue necesario congelar una nueva conversion o ecuacion de transferencia de calor mas alla de la ya registrada en la Consulta 001.