# Matriz de Evidencia y Claims del Portafolio

## Objetivo

Separar con claridad:

- qué claim sí puede publicarse;
- con qué evidencia se sostiene;
- qué material visual hace falta;
- y qué no debe decirse aunque exista código, research o intención de producto.

## Matriz principal

| Pieza | Claim publicable | Evidencia principal | Estado | Activo visual necesario | No decir |
| --- | --- | --- | --- | --- | --- |
| Visor WebGL final | Existe un visor técnico interactivo del Holybro X500 V2 con flujo visible y coherente | `UIManager.cs`, `UIHeroController.cs`, `UIDetailsSheet.cs`, `VALIDACION_FUNCIONAL_FINA_2026-04-09.md` | Publicable | Hero, selección, `bottom sheet`, `Inspect/Analyze/Studio` | que catálogo, settings o measurement son parte del flujo final |
| Sistema de visualización | El visor integra modos visuales técnicos y lectura contextual | `ViewModeManager.cs`, `ClippableLit.shader`, `XRay.shader`, `Thermal.shader`, diagramas del capítulo 4 | Publicable | grid de modos visibles, corte transversal, comparativa visual | que todos los modos implementados están expuestos en la UI |
| Sistema térmico híbrido | El proyecto traduce estado operativo a una visualización térmica heurística y legible | `DroneStateController.cs`, `ThermalSimulationManager.cs`, `ThermalViewController.cs`, auditoría matemática | Publicable con guardrails | modo térmico con leyenda, diagrama del subsistema, tooling térmico | FEA, termografía calibrada, validación experimental cerrada |
| Tooling de editor | El proyecto incluye herramientas para setup, auditoría y consistencia de escena | `ProjectSetupWizard.cs`, `ImportedDroneCoverageAudit.cs`, `ThermalContactGraphBuilderWindow.cs` | Publicable | capturas del editor y una explicación corta de cada tool | que el proyecto es solo una demo visual sin tooling |
| CAD -> Unity -> WebGL | Se construyó una ruta técnica para convertir un activo complejo en un visor web navegable | `x500v2_parts_data.json`, `ImportedDroneRuntimeBinder.cs`, blueprint Holybro, auditorías de cobertura | Publicable con gate de métricas | diagrama de pipeline, tabla `28 / 30 / 257`, captura final en browser | métricas finales no medidas, pipeline oficial con herramientas solo exploradas |
| Comunicación técnica | El proyecto se documentó con arquitectura, auditorías y límites honestos | documentación técnica, matriz de desconexiones, auditoría matemática, diagramas del informe | Publicable como apoyo | uno o dos diagramas cortos, tabla de alcance real | que todo lo documentado fue shipping UX visible |

## Claims secundarios útiles

| Claim secundario | Evidencia | Uso recomendado |
| --- | --- | --- |
| El proyecto distingue entre funciones visibles, ocultas y legacy | `MATRIZ_DESCONEXIONES_APP_DOCUMENTACION.md` | Buena señal de honestidad técnica en entrevistas |
| El modo térmico tiene soporte matemático y de verificación | `AUDITORIA_MATEMATICA_Y_ARQUITECTURA_2026-04-12.md`, `wolfram_verificaciones.md` | Útil para TA leads y revisores técnicos |
| La optimización fue tratada como problema de sistema y no como simple decimate | blueprint Holybro y plan de fasteners | Útil para la pieza CAD -> WebGL |

## Material de research: citar como exploración, no como shipping

| Tema | Puede citarse | No debe presentarse como |
| --- | --- | --- |
| `CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md` | blueprint de decisiones de pipeline | implementación completa cerrada |
| `CAD_Fastener_Optimization_Plan.md` | hoja de ruta de optimización de fasteners | fase ya terminada |
| PiXYZ / Simplygon / InstaLOD / Quad Remesher | referencias del espacio de solución | stack oficialmente usado en la build final |
| Houdini | exploración o herramienta evaluada | pipeline central del producto final |

## Capacidades que deben quedar fuera del discurso principal

| Capacidad | Motivo |
| --- | --- |
| audio | no hay assets integrados como feature final visible |
| suite completa de ensamblaje | no forma parte del flujo final publicado |
| `MeasurementTool` visible | existe en código, pero no está expuesto en la UI final |
| catálogo visible | quedó fuera del flujo oficial |
| settings visibles | no están integrados en la experiencia final |

## Regla de cierre

Si una pieza no tiene:

1. evidencia verificable,
2. material visual claro,
3. y una explicación honesta del alcance,

entonces no está lista para publicarse aunque el código exista.
