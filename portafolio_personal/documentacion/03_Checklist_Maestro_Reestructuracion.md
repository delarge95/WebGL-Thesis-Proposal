# Checklist Maestro de Reestructuración

Documento operativo para continuar la reorganización del repositorio con un orden fijo, reversible y trazable.

## Actualización visible de hoy (2026-03-11)

- [x] Curaduría técnica inicial copiada y verificada en `herramientas_fuente/`.
- [x] Breakdown del pipeline CAD→WebGL redactado.
- [x] Breakdown del sistema de visualización interactiva redactado.
- [x] Shot list de activos visuales definida.
- [x] Estructura física de `assets_fuente/` preparada.
- [x] README raíz alineado con la estructura reordenada visible del repositorio.
- [x] `CLEANUP_MANIFEST.md` ampliado con inventario de `documentacion/` y `assets_fuente/`.
- [x] Guía de despliegue y reporte de alineación actualizados para reflejar el build WebGL real publicado en `docs/`.
- [x] Plan de trabajo reorientado explícitamente al cierre de tesis, repositorio y consistencia documental antes que a nuevas piezas de portafolio.
- [x] Documentación técnica adicional revisada para eliminar referencias heredadas a artefactos `*.unityweb` o nombres genéricos de build ya no vigentes.
- [x] Auditoría de rutas obsoletas cerrada en README, documentación técnica y consolidaciones documentales relevantes.
- [x] `archive/` y `trash/` verificados contra su inventario documentado, sin material extra no justificado en esta ronda.
- [x] Segunda pasada conservadora a `trash` ejecutada con logs LaTeX generados y salida auxiliar no operativa fuera de `blender_files/`.
- [x] `External_docs/` identificado como lote activo de referencia y excluido de movimientos prematuros en esta ronda.
- [x] `External_docs/` reorganizado internamente en `academic_templates/`, `ux_ui_reference/` y `unity_webgl_reference/`, con referencias vivas actualizadas.
- [x] `Informe_final/presentation/` depurado de documentación operativa duplicada; `docs/manuals/` queda como fuente publicable del guion de demo.
- [x] Duplicados exactos entre `desarrollo/docs/`, `docs/manuals/` e `Informe_final/validacion/` auditados y clasificados entre redundantes e intencionales.
- [x] Nueva pasada conservadora a `trash` ejecutada sobre artefactos generados de Unity (`obj/` y `GeneratedAssets_deleted/`), manteniendo los shader logs en espera por bloqueo.
- [x] La raíz de `desarrollo/unity_project/` fue depurada de documentación y scripts auxiliares sueltos, reubicando históricos y manuales a sus zonas correctas.
- [x] Validación de cierre sobre `desarrollo/unity_project/`: la raíz restante se confirma como proyecto activo más tooling legítimo; sólo persisten pendientes los shader logs bloqueados.
- [x] Nueva pasada sobre la raíz del repositorio: planes sueltos reubicados a `Informe_final/Desarrollo_App/hojas_de_ruta/` y volcados auxiliares enviados a `trash/raiz_repositorio/`.
- [x] La planeación personal/carrera salió de `.planning/` y quedó consolidada en `portafolio_personal/documentacion/planeacion_carrera/`.
- [x] El addon externo suelto `blender_mcp_addon.py` salió del root y quedó en `trash/raiz_repositorio/` tras validarse como tooling no integrado al flujo actual.
- [x] La raíz de `desarrollo/` perdió cuatro documentos sueltos más: análisis y setup fueron a `desarrollo/docs/`, el plan de modelado a `hojas_de_ruta/` y la directiva histórica a `archive/`.
- [x] `.cursor/` salió del root y quedó registrada en `trash/.cursor/` como tooling de editor no usado en este workspace.
- [x] `desarrollo/logs/` quedó revalidado sin nuevos movimientos: sólo mantiene `project_graph.json` como artefacto operativo activo.
- [x] Nueva pasada conservadora completada sobre auxiliares LaTeX: `Propuesta/` e `Informe_final/` conservan fuentes y PDFs, mientras `.aux`, `.out`, `.toc` y `.lot` pasaron a `trash/`.
- [x] Los `shadercompiler-UnityShaderCompiler.exe-*.log` salieron finalmente de `desarrollo/unity_project/Logs/` y quedaron en `trash/`, cerrando ese pendiente histórico.
- [x] `docs/INSTALL_STATUS.md` salió de la capa pública y quedó en `trash/docs/` como estado transicional ya no apto para publicación.
- [x] Las iteraciones UX internas `UX_Audit_Iteration14.md` y `UX_Research_Plan_Iteration12.md` salieron de `External_docs/` y se consolidaron en `Informe_final/Desarrollo_App/Audits/other/`.
- [x] Se revalidó que `PHASE2_UX_PLAN.md` ya estaba correctamente concentrado en `hojas_de_ruta/unity_project_historico/`.
- [x] Los placeholders reales restantes en guía de demo, especificación de audio y reporte de alineación académica fueron normalizados para que se lean como documentos de entrega y no como borradores.
- [x] El paquete de entrega quedó explicitado en una matriz única y los manuales/guiones públicos dejaron de prometer módulos presentes en código pero no visibles en la UI actual.
- [x] La documentación pública y académica dejó explícito que la herramienta de medición no forma parte del prototipo final visible y registró como pendiente prioritario el flujo de encendido con propagación térmica progresiva.
- [x] `hojas_de_ruta/` quedó indexado y separado entre plan operativo vigente e histórico metodológico, con los planes superseded movidos a `historico_metodologico/`.

## Plan operativo de hoy (Punto 0)

1. Cerrar las incoherencias documentales que aún mezclan estructura vieja y estructura actual del repositorio.
2. Completar una nueva pasada de clasificación fuera de `blender_files/`, distinguiendo con claridad material académico, operativo, histórico y de presentación.
3. Verificar que `archive/` y `trash/` sólo contengan material con criterio explicado y trazado.
4. Dejar `portafolio_personal/` listo como staging derivado, pero subordinado al cierre correcto del paquete de tesis y del repositorio.
5. Registrar todo ajuste estructural o documental en bitácora, changelog y manifiesto.
6. Mantener `hojas_de_ruta/` con raíz mínima: sólo planes vigentes e índices; todo material metodológico superseded debe ir a su zona histórica.

## Backlog del producto (registrado, no foco de esta ronda)

1. Terminar retopología y LODs para importar a Unity.
2. Configurar hotspots, filtros, vista explosiva e información con el modelo real en Unity.
3. Configurar botón de encendido y simulación térmica con propagación de calor entre piezas según material y rangos conocidos.
4. Configurar audio para la app.
5. Hacer pruebas de rendimiento con profiler y en web.
6. Verificar KPIs, identificar brechas y definir plan de cierre para los KPIs no alcanzados.
7. Ejecutar pruebas de usabilidad y análisis de resultados.
8. Documentar todos esos puntos faltantes dentro del informe final, anexos y repositorio.

## Regla de alcance actual

- No tocar `blender_files/` en esta fase.
- La fuente de verdad sigue en las rutas originales del proyecto.
- `portafolio_personal/` funciona como staging y curaduría, no como reemplazo del material fuente.
- Todo movimiento a `archive/` o `trash/` debe quedar registrado en bitácora, changelog y manifiesto cuando corresponda.

## Fase 1. Congelar criterio y zonas activas

- [x] Confirmar la política de `archive/`, `trash/` y `portafolio_personal/`.
- [x] Proteger explícitamente `blender_files/` de esta fase.
- [x] Mantener Holybro como paquete técnico activo salvo iteraciones superseded claras.
- [x] Revisar si existe alguna otra carpeta en producción activa que deba quedar temporalmente fuera de alcance.

## Fase 2. Clasificar por destino antes de mover

- [x] Definir criterios en `CLEANUP_MANIFEST.md`.
- [x] Ejecutar una primera pasada conservadora en `trash/`.
- [x] Ejecutar una primera pasada histórica en `archive/`.
- [x] Hacer una nueva revisión de documentos y assets fuera del flujo principal para marcarlos como `keep`, `archive`, `trash-candidate` o `portfolio-candidate`.
- [x] Separar explícitamente material académico, material operativo y material de presentación cuando aún convivan en la misma carpeta.

## Fase 3. Consolidar documentación de portafolio

- [x] Crear `portafolio_personal/documentacion/`.
- [x] Añadir índice, checklist ejecutivo e inventario inicial de fuentes.
- [x] Reubicar el documento maestro de portafolio dentro de la carpeta personal.
- [x] Extraer desde `desarrollo/docs/` una selección corta de documentos para breakdown técnico.
- [x] Crear un breakdown resumido del pipeline CAD a WebGL orientado a ArtStation y reel.
- [x] Crear un breakdown resumido del sistema de visualización interactiva orientado a perfil técnico.

## Fase 4. Curar herramientas y material técnico reutilizable

- [x] Crear `portafolio_personal/herramientas_fuente/`.
- [x] Crear subcarpetas `pipeline_holybro`, `shaders` y `ui_tecnica`.
- [x] Staging inicial de scripts Holybro, shaders y componentes UI/input representativos.
- [x] Añadir un manifiesto local con propósito y procedencia del material staged.
- [x] Evaluar si conviene sumar más piezas técnicas fuertes, por ejemplo `EdgeDetection.shader`, `Ghosted.shader`, `SelectionManager.cs` o `ViewModeManager.cs`.
- [x] Redactar una justificación breve por pieza para facilitar su uso en portfolio, CV o entrevista.

## Fase 5. Ejecutar movimientos físicos seguros

- [x] Restaurar referencias mal clasificadas cuando se detecta un falso positivo.
- [x] Aplicar próximos movimientos sólo después de verificar referencias en tesis, README, docs web y flujo Unity.
- [x] Evitar mover lotes grandes sin inventario previo.
- [x] Verificar siempre el resultado real con listados de carpeta, no sólo con la salida del terminal.

## Fase 6. Validar consistencia documental y técnica

- [x] Comprobar que README, manuales y capítulos no apunten a rutas obsoletas.
- [x] Confirmar que GitHub Pages y el build WebGL no dependan de archivos ya movidos.
- [x] Revisar que `archive/` y `trash/` no estén acumulando material sin clasificación explicada.
- [x] Confirmar que `portafolio_personal/` mantiene sólo copias curadas o documentos propios, no dependencias críticas del proyecto.

## Fase 7. Cierre de ronda

- [x] Registrar en `BITACORA.md` cada nueva pasada relevante.
- [x] Resumir en `CHANGELOG.md` sólo los hitos que cambian la estructura o el criterio del repositorio.
- [x] Actualizar `CLEANUP_MANIFEST.md` con cualquier inventario nuevo de `archive/`, `trash/` o `portafolio_personal/`.
- [x] Definir la siguiente fase antes de abrir una nueva ronda de movimientos.

## Siguiente tramo recomendado

1. Ejecutar una nueva pasada de clasificación física fuera de `blender_files/`, priorizando exportaciones temporales, scripts raíz sueltos y documentación lateral aún no revisada.
2. Revisar si quedan auxiliares generados adicionales en capas académicas o de presentación fuera de `trash/` y `archive/`.
3. Volver a la curaduría de portafolio sólo después de dejar estable la reestructuración del repositorio y la coherencia del paquete académico.
