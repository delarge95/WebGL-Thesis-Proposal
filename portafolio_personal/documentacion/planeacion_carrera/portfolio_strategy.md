# Estrategia de Portafolio: Technical Artist 2026

## Posicionamiento objetivo

Carril principal:

- Technical Artist
- tools + shaders + optimization + technical visualization

Carril secundario:

- real-time visualization engineer
- interactive 3D product visualization

## Qué debe demostrar el portafolio en menos de dos minutos

1. Construyes sistemas interactivos reales, no solo renders.
2. Puedes trabajar entre pipeline artístico, runtime y tooling.
3. Entiendes restricciones técnicas y sabes documentarlas con honestidad.
4. Sabes separar shipping UX, capacidad oculta y trabajo futuro.

## Paquete profesional recomendado

### Paquete A: Visor WebGL final

Pruebas principales:

- Holybro X500 V2 viewer
- flujo `bottom sheet`
- `Inspect / Analyze / Studio`

### Paquete B: Sistema visual y shaders

Pruebas principales:

- `X-Ray`
- `Solid Color`
- `Thermal`
- explicación breve de clipping, view modes y límites de publicación

### Paquete C: Toolsmith value

Pruebas principales:

- `ProjectSetupWizard`
- `ImportedDroneCoverageAudit`
- `ThermalContactGraphBuilderWindow`
- `ImportedDroneRuntimeBinder`

### Paquete D: Caso CAD -> Unity -> WebGL

Pruebas principales:

- taxonomía `28 / 30 / 257`
- normalización del activo
- reparación runtime del import
- resultado final en browser

Gate:

- no publicar métricas finales de optimización hasta completar freeze y medición final.

### Paquete E: Sistema térmico híbrido

Pruebas principales:

- solver reducido + view controller + shader + leyenda
- verificación matemática suficiente para sostener visualización aplicada

Guardrail:

- no vender FEA ni validación experimental cerrada.

## Reglas de mensaje

Siempre explicar:

- qué problema se resolvió;
- qué parte del sistema es visible en la build final;
- qué tooling o arquitectura sostiene la solución;
- qué límites siguen abiertos.

Nunca implicar:

- audio shipped;
- full assembly suite shipped;
- Houdini como pipeline final;
- métricas viejas o conteos históricos;
- módulos inexistentes;
- research exploratorio como implementación cerrada.

## Orden recomendado de publicación

1. Visor WebGL final.
2. Breakdown de sistema interactivo.
3. Breakdown de visualización/shaders.
4. Tooling de editor.
5. Sistema térmico híbrido.
6. Caso CAD -> WebGL con métricas solo cuando el freeze exista.

## Condición de éxito

Un reviewer debería poder concluir:

> Este candidato puede entregar visualización técnica interactiva, construir herramientas de soporte, razonar sobre runtime constraints y comunicar con claridad lo que realmente está shipping.
