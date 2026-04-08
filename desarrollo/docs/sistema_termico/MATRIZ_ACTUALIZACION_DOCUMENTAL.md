# Matriz de actualizacion documental del sistema termico

## Objetivo

Definir que documentos se actualizan, cuando y con que nivel de evidencia cada vez que avanza el subsistema termico.

## Reglas generales

- No registrar como terminado lo que solo este prototipado.
- No prometer validacion fisica sin evidencia medible.
- Distinguir siempre entre `implementado`, `integrado`, `validado`, `experimental` y `pendiente`.
- Toda constante, conversion o ecuacion nueva debe quedar trazada en `wolfram_verificaciones.md`.
- Todo artefacto que no pertenezca al camino oficial debe etiquetarse como experimental.

## Actualizacion progresiva por iteracion

Actualizar siempre cuando haya avance tecnico real:

- `Informe_final/Desarrollo_App/BITACORA.md`
- `Informe_final/Desarrollo_App/CHANGELOG.md`
- `desarrollo/docs/sistema_termico/README.md`
- `desarrollo/docs/sistema_termico/AGENT_HANDOFF_THERMAL.md`
- `desarrollo/docs/sistema_termico/RETOPOLOGIA_POR_PIEZA.md`
- `desarrollo/docs/sistema_termico/INDICE_TERMICO.md`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/INDICE_TERMICO_CODIGO.md`
- `desarrollo/docs/sistema_termico/wolfram_verificaciones.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`

## Actualizacion por hito estable

Actualizar cuando la etapa ya sea mantenible y explicable a terceros:

- `Informe_final/Manual_tecnico/manual_tecnico.tex`
- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/Desarrollo_App/TECHNOLOGY_STACK.md`
- `portafolio_personal/README.md`
- `portafolio_personal/documentacion/07_Breakdown_Sistema_Termico_Hibrido.md`

## Actualizacion condicionada por estado de producto

Actualizar solo cuando la experiencia ya sea visible y usable en build:

- `Informe_final/Manual_de_usuario/manual_usuario.tex`

## Actualizacion solo con evidencia validada

No actualizar como resultado final hasta tener datos medidos:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/chapters/06_conclusiones.tex`

## Etiquetado obligatorio

### Implementado

Existe en codigo o asset y puede localizarse en el repo.

### Integrado

Ya participa del flujo real de escena o UI.

### Validado

Ya se comprobo en Unity Editor o build objetivo con observacion real.

### Experimental

Existe, pero esta fuera del camino oficial o usa datos dummy.

### Pendiente

Se documento como siguiente paso, no como capacidad actual.

## Estado actual que debe usarse en la tesis y docs vivas

Documentar el sistema termico hoy como:

- subsistema hibrido V1 en desarrollo activo,
- solver reducido por 28 piezas canonicas,
- grafo de contactos canónico oficial ya creado,
- leyenda termica visible,
- presets visuales canónicos activos,
- validacion final sobre escena retopologizada aun pendiente,
- harness CAD bruto etiquetado como experimental.
## Actualizacion 2026-03-31

Agregar a la trazabilidad operativa del subsistema:

- `desarrollo/docs/sistema_termico/PREPARACION_FBX_IMPORTADO.md`

Registrar como `implementado` e `integrado` cuando aplique:

- binder runtime del dron importado,
- panel de power con slider de carga,
- taxonomia publica de 6 filtros,
- herencia termica de `Fasteners` y `Misc`,
- y preparacion automatizada del FBX importado.
