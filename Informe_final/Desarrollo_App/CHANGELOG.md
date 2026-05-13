# CHANGELOG � WebGL Drone Visualization Project

> Registro completo de la evoluci�n del proyecto, desde la concepci�n hasta el estado actual.
> Fuentes: Git history, GitHub, Antigravity sessions, implementation plans, roadmaps.

---

## [2026-05-08] Reparacion Escala y Jerarquia Canonica FBX Final

### Fixed
- `Assets/Editor/ImportDroneModel.cs`: mide bounds del FBX importado y calibra `ModelImporter.globalScale` hacia un tamano dominante objetivo de `5` unidades Unity, evitando que el dron final en centimetros rompa camara, zoom, Solid/Blueprint, explode y escala visual de fasteners.
- `Assets/Editor/SetupImportedDroneThermalTest.cs`: vuelve a priorizar `x500v2_parts_data.json` como fuente de anclas interactivas canonicas; `x500v2_blender_synced_parts.json` queda como fuente granular de metadata/subpiezas.
- `Assets/Editor/SetupImportedDroneThermalTest.cs`: demueve anchors `x500v2_blend_*` para que las subpiezas Blender no compitan como piezas madre seleccionables.
- `Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs`: deja de reparentar renderers que ya pertenecen a un anchor canonico estable y separa motor, ESC, helice, PDB, bateria, rieles, Pixhawk y GPS con reglas menos ambiguas.
- `Assets/Editor/HolybroFastenerCatalogBuilder.cs`: evita asignar fasteners a padres `x500v2_blend_*` y calcula padres contra anchors canonicos.
- `Assets/Scripts/Core/Content/FastenerBuilder.cs`: elimina minimos absolutos que hacian que el detalle modular fuera desproporcionado frente a proxies pequenos.
- `Assets/Scripts/Core/Managers/PartVisibilityManager.cs`: reduce spam de diagnostico de isolate mediante `enableIsolationDiagnostics`.

### Added
- `Assets/Editor/RuntimeDroneSceneAuditor.cs`: auditoria post-import para root activo, bounds, anchors canonicos, proxies temporales, fastener markers, padres `x500v2_blend_*` y conteos criticos.
- `SetupImportedDroneThermalTest.cs`: proxies canonicos temporales para piezas esperadas por la app pero ausentes como geometria dedicada en el FBX actual.

### Notes
- Validacion ejecutada: `dotnet build Core.Player.csproj -nologo -m:1` con `0` errores.
- Validacion editor por `dotnet` no es concluyente porque falla antes en `Library/PackageCache/com.unity.ugui` con `DefaultControls.factory` readonly; la comprobacion real del importador queda en Unity Editor.
- Tras recompilar, se debe ejecutar de nuevo `Tools > Import Final Runtime Drone Model` para regenerar la escena con escala, jerarquia, fastener catalog y assets canonicos corregidos.

## [2026-05-08] Fix Importacion FBX Runtime Final

### Fixed
- `Assets/Models/x500v2_runtime_low_final.fbx`: sincronizado con la fuente Blender real para corregir el asset que Unity estaba importando como corrupto/sin mallas.
- `Assets/Editor/ImportDroneModel.cs`: ahora valida cabecera FBX, evita punteros LFS, sincroniza fuente externa si el asset difiere, guarda respaldos fuera del repositorio, aborta si no hay mallas importadas y deja de usar `ModelImporterMaterialLocation.External`.
- `Assets/Editor/ImportDroneModel.cs`: crea proxies simples `x500v2_prop_*` desde motores `DJ-2216-KV880` cuando el FBX no trae helices dedicadas.
- `Assets/Editor/SetupImportedDroneThermalTest.cs`: matching tolerante para ids/`blenderName` de Blender con mayusculas, guiones, underscores, puntos y sufijos `_low/_PRIM`.

### Changed
- `.gitignore`: se completo la exclusion de `.blend` y variantes numericas en `blender_files` y `desarrollo/blender_assets` para evitar que Git/LFS vuelva a intentar stagear archivos Blender pesados.
- Se aplico una cuarentena local no versionada en `.git/info/exclude` y `assume-unchanged` para archivos LFS rastreados, con el unico objetivo de impedir auto-stage masivo durante la validacion Unity.

### Notes
- Validacion externa de nombres FBX contra `x500v2_blender_synced_parts.json`: `55/55` piezas detectables por la nueva regla de matching.
- Validacion Unity ejecutada desde `Tools > Import Final Runtime Drone Model`: `Meshes importados: 252`, `Helices normalizadas: 4`, `Fasteners normalizados: 170`, `Setup thermal/runtime: OK`.
- Hallazgo medido del import: `Mesh assets unicos: 252`, `sharedMesh repetido: 0`; la exportacion actual conserva objetos y jerarquia runtime, pero no instancing de malla compartida dentro de Unity.
- La cuarentena Git/LFS es local y reversible; antes de preparar commits de assets pesados debe retirarse o revisarse dentro de un plan LFS controlado.

## [2026-05-08] Guion de Sustentacion y Contencion Git/LFS

### Changed
- `Cerebro_Digital/Wiki/Concepts/INF_EST_51_Guion_Completo_Sustentacion_30min.md`: reestructurado como guion 5.0 de 30 minutos, con narrativa de pereza productiva, flujo de demo ordenado, encendido del dron, fasteners modulares, Analyze/Studio, resultados con placeholders y checklist de ensayo.
- El guion ahora incluye una guia de animaciones `A01-A12` para explicar pipeline CAD/WebGL, jerarquia, fasteners, aislamiento, encendido, thermal, corte y resultados.

### Notes
- Se diagnostico saturacion local por multiples procesos `git add -- ...` sobre archivos Blender/FBX/texturas y sus `git-lfs filter-process`; se detuvo solo un lote confirmado de procesos atascados, sin tocar Unity, Blender ni archivos.
- No se debe intentar subir, preparar commit o forzar LFS en esta sesion; los assets pesados requieren un plan separado para evitar cuello de botella.

## [2026-05-08] Coherencia documental post-fasteners y pipeline Blender/Unity

### Changed
- `Informe_final/presentation/*`: se actualizo el paquete de sustentacion para retirar claims obsoletos de `7` modos publicos, medicion, BOM, conexiones, catalogo legacy y metricas no medidas.
- `docs/manuals/DEMO_SCRIPT.md`, `docs/manuals/README.md` y `docs/manuals/ARCHITECTURE.md`: ahora describen el flujo visible real `Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio`.
- `Informe_final/chapters/04_desarrollo.tex`, `05_resultados.tex`, `06_conclusiones.tex` y `08_apendices.tex`: se ajusto el estado del FBX final como preparado para QA Unity, no como resultado empirico cerrado.
- Manuales LaTeX: fecha actualizada a mayo de 2026 y limites alineados con validacion pendiente de FBX, texturas finales, grupos, hotspots, filtros, helices y fasteners.
- Obsidian (`INF_EST_04`, `INF_EST_30`, `INF_EST_33`, `INF_EST_50`) y `VARIANTE_ESTUDIO_OBSIDIAN.md`: se sincronizaron con masters+instancias, mask final, fasteners temporales y regla de no inventar resultados post-freeze.

### Notes
- Los documentos historicos y auditorias antiguas pueden conservar referencias legacy, pero no deben usarse como fuente primaria frente a manuales, informe, presentacion y guion actualizados.

## [2026-05-08] Blender Final Bake, Export Manual y Manifest Runtime

### Added
- `Blender_Final_Bake_Export_Unity_Workflow.md`: guia final para bake de `BaseColor`, `Roughness`, `Metallic`, normal, AO, mask packing y export FBX manual hacia Unity.
- `blender_bake_target_setup.py`: preparacion no destructiva de imagenes/nodos activos para bakes en Blender 5.0.1.
- `blender_pack_x500_mask.py`: empaquetado de `R=AO`, `G=Roughness`, `B=Curvature`, `A=Metallic` y derivado URP `MetallicSmoothness`.
- `blender_runtime_manifest_exporter.py`: manifest no destructivo de masters, instancias, transforms, bounds, candidatos de pieza madre y reconciliacion de fasteners.
- `Assets/Models/x500v2_runtime_low_final.fbx`: FBX runtime final copiado para importacion Unity.
- `Tools > Import Final Runtime Drone Model`: importador final que conserva el modelo anterior como referencia, instancia el FBX final, normaliza helices/fasteners y ejecuta el setup runtime.

### Changed
- `CAD_Bake_Automation_Workflow.md`: queda marcado como historial del flujo CAD/RizomUV/Marmoset; el runtime final ya no excluye masters porque tambien son piezas fisicas del dron.
- La regla operativa de export queda fijada como union de `BAKE_MASTERS_LOW`, `ASSEMBLY_INSTANCES_LOW`, `PRIMITIVE_FASTENER_MASTERS` y `PRIMITIVE_FASTENER_INSTANCES`.
- `DroneStateController`: el eje de giro de helices se resuelve por bounds locales para soportar orientaciones importadas distintas.
- `SetupImportedDroneThermalTest`, `ImportedDroneRuntimeBinder` y `HolybroFastenerCatalogBuilder`: reconocen fasteners con nomenclatura Blender final (`GB70`, `PAN`, `CHEN`, `ZSLM`, `LM`, `NILONGZHU`, `HUAN-GUIJIAO`), no solo nombres temporales `x500v2_fastener.*`.

### Notes
- La exportacion FBX sigue siendo manual desde Blender; Codex solo prepara scripts, reportes y configuracion Unity posterior.
- La configuracion Codex para `blender-mcp` quedo escrita, pero la verificacion con `uvx blender-mcp` queda pendiente de aprobacion/reinicio.
- Si un fastener no tiene asignacion confiable a pieza madre, se reporta con candidatos y no se asigna por suposicion.
- No se pudo ejecutar Unity en batchmode desde esta sesion porque no se encontro `Unity.exe`; la importacion de escena debe ejecutarse desde el menu de Unity indicado.

## [2026-05-06] Saneamiento Geométrico 3D, Material IDs y Modulariación de Fasteners (Blender)

### Added
- Nuevo documento de mapeo `Holybro_Material_Mapping.md` estableciendo 7 perfiles físicos PBR.
- Script automatizado en Blender para aplicar *Material IDs* a 55 piezas basado en colores de *Viewport* (optimización para RizomUV y Marmoset).
- Aislamiento arquitectónico de la tornillería hacia colecciones `FASTENER_MASTERS`, preparándolas para instanciamiento puro en Unity.
- Plan de acción final estructurado en `PLAN_DE_ACCION_FINAL_2026-05-06.md`.

### Fixed
- Corrección del bug de *Apply Transforms* en geometrías High-Poly (batería, GPS, telemetría) ocultas en el viewport, asegurando superposición perfecta de Bounding Boxes.
- Remoción forzada de *Custom Split Normals* en mallas de baja resolución (LOW) para prevención de corrupción del *Tangent Space* en bake.

---

### Added
- Scripts de automatización en Blender para extracción de Masters y saneamiento de Instancias (`CAD_Bake_Automation_Workflow.md`).
- Corrección algorítmica de jerarquías CAD (`Multi-user data` error bypass) preservando *transforms* y compensando escalas nulas o heredadas.
- Nomenclatura automatizada `_low` y `_high` lista para *Quick Loader* de Marmoset Toolbag.

---

## [2026-04-21] Onboarding MVP procedural, gestos reales por plataforma y cierre de UX guiada

### Added

- `Assets/Scripts/UI/Panels/OnboardingAnimationView.cs`: canvas procedural con `Painter2D` para previews animados del onboarding.
- Storyboards runtime para `15` cards del onboarding con variantes `PC` y `Mobile`.
- Soporte de overlays y labels para cards que requieren tabs o panels explicativos dentro del preview.

### Changed

- `OnboardingController.cs`: sincroniza fases, plataforma y stage visual con el onboarding real.
- `MainLayout.uxml` y `Theme.uss`: amplian y estabilizan el viewport vertical del onboarding para lectura tipo mobile.
- `ProceduralResetIcon.cs`: refinado para usar el mismo lenguaje visual del reset dentro del onboarding.

### Fixed

- Sincronizacion general de `cursor/tap -> click -> response` en menus, sliders, filtros, cards de seleccion y cards de environment.
- Continuidad del puntero entre fases del loop para evitar saltos visuales.
- Doble click de `Filter` e `Isolate` para que se lea como doble accion real y no como dos clics desconectados.
- Timings y holds finales en cards de `Navigate`, `Cut`, `Explode`, `Studio` y `Environment`.

### Notes

- La solucion evita `GIF`, video, spritesheets o `SVG runtime` como dependencia principal.
- El siguiente cierre de calidad depende de QA visual en Unity/WebGL y captura de evidencias finales.

## [2026-04-13] Unity Fasteners: metadata modular, catalogos reconciliados e inspeccion proxy->detail

### Added

- `FastenerDataModels.cs`: contratos serializables para familias, instancias, recetas modulares, metadata y reconciliacion.
- `FastenerRegistry.cs`: resolucion runtime `instanceId -> family -> recipe` desde `Resources`.
- `FastenerInspectionManager.cs`: activacion de detalle solo para el fastener seleccionado.
- `FastenerBuilder.cs`: placeholders modulares en Unity para screws, nuts, standoffs, grommets y stoppers.
- `FastenerRuntimeMarker.cs`: sello runtime con `familyId`, `instanceId`, `sceneTypeKey` y flags de inspeccion.
- `holybro_parent_subpieces.json`: catalogo de piezas madre, subpiezas documentadas y resumen de fasteners por parent canonical.
- Catalogos versionados:
  - `desarrollo/docs/investigacion/Holybro/holybro_fastener_families.json`
  - `desarrollo/docs/investigacion/Holybro/holybro_fastener_instances.json`
  - `desarrollo/docs/investigacion/Holybro/holybro_fastener_reconciliation.json`

### Changed

- `DronePartData.cs`: agrega `fastenerMetadata` y helpers para descripcion/assembly info enriquecidos.
- `SetupImportedDroneThermalTest.cs`: deja de crear fasteners genericos y ahora aplica metadata real, markers y export de catalogos.
- `ImportedDroneRuntimeBinder.cs`: garantiza `FastenerRegistry` + `FastenerInspectionManager` y sella metadata de fasteners durante el saneamiento runtime.
- `SelectionManager.cs`: cualquier click sobre geometria de fastener se resuelve al root completo del fastener para evitar aislamientos parciales por submesh.
- `HighlightSystem.cs`, `MaterialController.cs` y `FastenerInspectionManager.cs`: el color de hover/seleccion se reaplica sobre el detalle temporal aunque el proxy quede oculto.
- `PartVisibilityManager.cs`: el isolate de subseleccion ahora reconoce el scope completo del fastener, evita arrastrar geometria ancestro al aislar un fastener individual y, para piezas madre, incluye los fasteners reconciliados por `parentCanonicalPartId`.
- `OrbitCameraController.cs`: el enfoque deja de usar una distancia fija, calcula distancia por bounds reales, reduce el `near clip` y usa una ventana de zoom dinamica por seleccion para que sensibilidad y alcance cambien entre un fastener pequeno y el dron completo.
- `OrbitCameraController.cs`: la seleccion activa ahora tambien gobierna la ventana de zoom sin exigir aislamiento previo, el dron completo recupera algo mas de acercamiento util y `pan`/`orbit` escalan segun el rango efectivo de inspeccion para no sobrerreaccionar sobre fasteners aislados.
- `OrbitCameraController.cs`: se reemplazo la escala lineal final de `pan`/`orbit` por curvas adaptativas; el paneo queda mas contenido en fasteners aislados y la orbita recupera algo de respuesta en inspeccion cercana.
- `OrbitCameraController.cs`: se corrige un limite falso al deshacer zoom; el contexto adaptativo deja de depender de un `target` residual cuando no hay seleccion activa y la ventana mantiene margen suficiente para volver a alejar la camara.
- `OrbitCameraController.cs`: se reduce aun mas la sensibilidad minima de `pan` en inspeccion cercana para fasteners y piezas pequenas.
- `PartVisibilityManager.cs`: expone `GetIsolatedTransform()` para que la camara pueda distinguir entre contexto aislado real y foco heredado de una seleccion antigua.
- `FastenerInspectionManager.cs`: pasa de una unica inspeccion por seleccion a un set activo de fasteners modulares, cubriendo fastener seleccionado, fastener aislado aun sin seleccion y fasteners asociados a una pieza madre aislada.
- `FastenerInspectionManager.cs`: al resolver clicks sobre meshes hijos revisa el marker del root del fastener antes de detenerse en la frontera `ExplodablePart`, evitando perder seleccion sin contaminarse con piezas madre.
- `SelectionManager.cs`: limpia el estado de hover al convertir una pieza en seleccion y evita el tinte azul residual al deseleccionar desde el background.
- `UIDetailsSheet.cs`: muestra subtipo, metrica, longitud, drive, material, CAD source y parent canonical para selecciones de `Fasteners`; para piezas madre agrega ensamblaje y subpiezas desde `subComponentNames`.
- `HolybroFastenerCatalogBuilder.cs`: ahora infiere `parentCanonicalPartId` por proximidad al anchor directo del dron para compensar que la escena actual agrupa los fasteners bajo `x500v2_fastener_group`.
- `Assets/Core/Data/X500V2Generated`: los fastener assets quedaron persistidos con metadata tecnica real y las piezas madre con su desglose `madre -> subpiezas -> fasteners`.

### Validation Snapshot

- Catalogos generados con `20` familias, `168` instancias y `9` issues de reconciliacion.
- Catalogo jerarquico generado con piezas madre del dron y sus subpiezas/fasteners configurados.
- `dotnet build Core.csproj -nologo`: OK.
- `dotnet build UI.csproj -nologo`: OK.
- `dotnet build Assembly-CSharp-Editor.csproj -nologo`: OK.

### Notes

- La geometria de fasteners en `MainScene_Final` se documenta como temporal/proxy, no como malla final de entrega.
- El reemplazo futuro por activos Blender detallados debe hacerse manteniendo `familyId`, `instanceId` y `recipeKey`.
- El isolate por fastener ahora esta definido a nivel de pieza completa, no de submesh.

---

## [2026-04-10] Blender Tooling: MoI3D ICP Instancer (V7)

### Added

- `cad_symmetry_addon_v7_moi.py`: Nuevo addon en Blender (`Holybro CAD` tab) que emplea el algoritmo Iterative Closest Point (ICP) para alinear mallas topológicamente inconsistentes y convertirlas en instancias enlazadas de manera procedural.
- Detección de orientación volumétrica mediante Componentes Principales (PCA) actuando como precondicionador determinístico para ICP.

### Changed

- Actualización completa de `CAD_Symmetry_Instancer_Documentation.md` agregando la retrospectiva desde la Versión V6 a V7 (Migración de SVD Kabsch a O(N^2) Mathutils ICP).
- Se habilitó la conversión de todos los conjuntos de fasteners asimétricos (Ej. M3x8 de piezas de hardware `GB70`) provistos originalmente desde `3D MoI` sin requerir que las tuercas o mallas contengan la misma cantidad de vértices a nivel byte.

## [2026-04-09] Cierre de cobertura jerarquica + setup/auditoria adaptativos por cobertura real

### Added

- Anchors sinteticos de grupo para auxiliares importados:
  - `x500v2_fastener_group`
  - `x500v2_misc_group`
- Telemetria de reasignacion en el setup (grupo sintetico/prefijo/heuristica).

### Changed

- `SetupImportedDroneThermalTest` ahora selecciona automaticamente fuente de datos (`synced` o `canonical`) segun cobertura real en escena.
- `ImportedDroneCoverageAudit` ahora selecciona fuente esperada por cobertura real para evitar falsos negativos de IDs faltantes.

### Fixed

- Reparent de auxiliares sobre escenas importadas con instancia de prefab mediante unpack previo del root.
- Cobertura de jerarquia en escena actual:
  - `Anchors sin renderer: 0`
  - `Renderers huérfanos top-level: 0`
  - `Huérfanos no resueltos por prefijo: 0`

### Validation Snapshot

- Setup: `Fuente usada: x500v2_parts_data.json (matches: 28/28)`, `Preparadas: 28`, `Warnings: 0`.
- Audit: reporte en `desarrollo/unity_project/Reports/imported_drone_coverage_report.md`.

---

## [2026-04-08] Checkpoint clave: split estable, filtros Analyze sin ALL y pipeline piezas 55+

### Added

- Ruta de carga de piezas granular en `SetupImportedDroneThermalTest` con prioridad a `x500v2_blender_synced_parts.json` y fallback automatico a `x500v2_parts_data.json`.
- Soporte de matching por `blenderName` para mejorar anclaje cuando no coincide el `id` canonico.
- Categoria `SensorsComms` expuesta en Analyze Filter UI.

### Changed

- UX de filtros Analyze:
  - Se elimina boton `ALL`.
  - Estado inicial: todas las categorias activas.
  - Click simple: toggle por categoria.
  - Doble click: modo exclusivo por categoria.
  - Doble click sobre categoria ya exclusiva: regreso a estado por defecto (todas activas).
- Normalizacion de categorias del pipeline de piezas hacia taxonomia `PartCatalogManager`.
- Complemento de inventario tecnico con estado implementado y diagnostico operativo (sin eliminar historial previo).

### Ops / Repo

- Split de trabajo en 5 bloques tematicos (UI, tooling, thermal-data, modelos, docs).
- Snapshot de respaldo versionado + tag de backup.
- Push remoto completado con LFS para binarios pesados.

### Next (primer paso al retomar)

- Validar cobertura real en escena importada (anclajes, seleccionabilidad, piezas en origen/centro) y ajustar vista explosiva con modelo final.

---

## [2026-04-10] Refinamiento final del sistema termico

- `style(thermal)`: `Thermal.shader` ahora usa shimmer de baja frecuencia, mas lento y mas suavizado para evitar flicker sobre la malla final.
- `fix(thermal)`: El edge glow deja de alterar la temperatura mostrada y pasa a funcionar como acento visual sutil.
- `style(thermal)`: `ThermalViewController` y `ThermalSurfaceProfile` reducen la variacion base y la amplitud visual por defecto para no sobrerrepresentar subpiezas nuevas.
- `docs(thermal)`: Actualizada la documentacion tecnica, indice termico, bitacora, verificaciones, tesis, manual tecnico y breakdown de portafolio.

## [2026-03-12] Etapa 2: Bootstrap termico, verificacion Wolfram, retopologia

- ix(bootstrap): SceneBootstrapper ahora inicializa ThermalSimulationManager y ThermalViewController en runtime.
- docs(thermal): Verificacion V002 de escalas de conductividad contra datos de ingenieria (7 materiales). Documentado en wolfram_verificaciones.md.
- docs(thermal): Creada guia de retopologia RETOPOLOGIA_POR_PIEZA.md con clasificacion de 28 piezas en 3 tiers.
- ix(docs): Corregidas afirmaciones falsas en INDICE_TERMICO.md sobre SceneBootstrapper y Thermal.shader.
- docs(solver): Agregado comentario V002 a EstimateMaterialConductivityScale() documentando la compresion deliberada de escalas.

## Leyenda de Prefijos

| Prefijo    | Significado                           |
| ---------- | ------------------------------------- |
| `feat`     | Nueva funcionalidad                   |
| `fix`      | Correcci�n de bug                     |
| `style`    | Cambios de estilo/UI                  |
| `docs`     | Documentaci�n                         |
| `chore`    | Mantenimiento                         |
| `refactor` | Reestructuraci�n sin cambio funcional |

---

## 2026-03-12 - Thermal Simulation Foundation

### Trabajo Realizado

- `feat(thermal)`: Se consolido `ThermalSimulationManager` como solver termico reducido por componentes con temperatura ambiente, calentamiento por carga y conduccion entre piezas.
- `feat(thermal)`: Se consolido `ThermalViewController` para conectar temperaturas reales por pieza con el modo de visualizacion termica.
- `feat(thermal)`: Se mantuvo `ThermalSurfaceProfile` como capa de configuracion espacial por pieza critica.
- `feat(thermal)`: Se amplio `ThermalContactGraphAsset` con metadata de build, nodos y notas por enlace.
- `feat(editor)`: Se creo `ThermalContactGraphBuilderWindow` para generar un grafo termico offline desde la geometria de la escena usando bounds de `ExplodablePart`.
- `feat(data)`: `DronePartData` fue alineado con el contrato termico requerido por el solver (`operatingTempMin/Max`, `thermalHover`, `thermalPeak`, `thermalWarmupSeconds`, `thermalExposure`, `thermalConductionScale`, `isThermallyCritical`).
- `feat(drone-state)`: `DroneStateController` ahora expone `SystemLoadFactor`, eventos de carga y sincronizacion de `Idle/Flying` con la carga sostenida.
- `feat(ui)`: Se agrego un slider de carga sostenida en `InspectModeHandler` y `MainLayout.uxml` bajo el control de encendido.
- `feat(runtime)`: `SceneBootstrapper` ahora puede asegurar la presencia de `ThermalSimulationManager` y `ThermalViewController` sin tocar escenas serializadas.
- `docs(thermal)`: Se actualizo la documentacion viva del sistema termico, incluyendo la matriz documental, la referencia tecnica y el breakdown de portafolio.

### Estado Validado

- El sistema termico ya tiene una base de runtime consistente y un camino de preprocesado offline.
- El grafo de contactos puede generarse en editor, pero aun falta calibrarlo sobre la geometria final retopologizada del X500 V2.
- La simulacion no debe presentarse aun como FEA ni como modelo termodinamico cuantitativo validado.
- La verificacion automatica con WolframAlpha sigue condicionada a la configuracion de `WOLFRAM_APP_ID`.

## 2026-03-11 � Ajustes UX: Analyze persistente + Explode desacoplado

### Trabajo Realizado

- `fix(ui)`: Se elimin� el cierre autom�tico de submen�s Analyze al seleccionar piezas o al hacer click en el fondo 3D.
- `fix(explode)`: Se desacopl� el estado de `Explode` del `AppState` global para que no se apague al cambiar entre Analyze y Studio.
- `fix(ui)`: Se restaur� la X del info panel con un icono procedimental y se reajust� su tama�o/posici�n para alinearla con el sistema visual del top bar.
- `fix(ui)`: Se eliminaron cierres no permitidos del info panel, quitando el drag-to-dismiss y evitando que los accesos de info funcionen como toggle de cierre.
- `fix(input)`: Se endureci� `InputManager.IsPointerOverUI()` para usar el panel activo, soportar touch y descartar picks no interactivos o invisibles.
- `style(ui)`: Se fij� la rotaci�n base del icono de cierre en `45�` para que la `X` no se lea como `+`.
- `fix(camera)`: Se corrigi� el paneo para usar los ejes locales de la c�mara en lugar de una mezcla de ejes globales.
- `fix(ui)`: Se aline� el onboarding con los controles reales de c�mara (`right-click` orbit, `middle-click` pan).
- `fix(shader)`: Se aplic� clipping global tambi�n en los passes `DepthOnly` y `DepthNormals` de `Blueprint.shader` para que `Cut` respete el postproceso.
- `fix(camera)`: Se separ� la intenci�n de `pan` y `pinch` en touch para evitar zoom espurio durante gestos de desplazamiento con dos dedos.
- `style(ui)`: Se redise�� el contraste del modo claro usando superficies transl�cidas oscurecidas, tipograf�a m�s legible y controles con mayor definici�n en `Studio Light` y fondos claros.
- `refactor(ui)`: `ProceduralIconBase` pas� a soportar override de paleta por icono y `ProceduralCloseIcon` qued� fijado a una paleta clara para mantener contraste dentro del sheet.
- `docs`: Se a�adi� un documento t�cnico con diagn�stico completo y plan de implementaci�n por fases para los siguientes problemas de la app.

### Archivos Afectados

- `desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Content/ExplodedViewManager.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIModeController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/InputManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/OrbitCameraController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingController.cs`
- `desarrollo/unity_project/Assets/Shaders/Blueprint.shader`
- `desarrollo/unity_project/Assets/Scripts/UI/ProceduralIcons/ProceduralIconBase.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/ProceduralIcons/ProceduralCloseIcon.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `desarrollo/unity_project/Assets/UI/Styles/Theme.uss`
- `desarrollo/docs/investigacion/14_analisis_problemas_app_2026-03-10.md`

### Resultado Validado

- `Cut`, `Filter` y `Explode` ya no se cierran al clickear el background.
- La vista explosionada permanece activa al abrir otros submen�s de Analyze y al alternar entre Analyze y Studio.
- El info panel vuelve a mostrar un bot�n de cierre visible, consistente con el sistema de iconos procedurales y mejor alineado en la esquina superior derecha.
- El info panel ya no se cierra arrastrando el handle ni al volver a pulsar los accesos de info; su cierre queda acotado a la `X`, doble click en el fondo 3D y cambio de modo.
- La `X` recupera una lectura visual correcta en reposo y la detecci�n de UI queda mejor preparada para bloquear input 3D s�lo sobre controles interactivos reales.
- El paneo vuelve a seguir el plano local de la c�mara y el onboarding ya no instruye un gesto de pan incorrecto.
- El modo `Blueprint` ahora entrega profundidad y normales ya recortadas al postproceso, evitando que el contorno reconstruya partes fuera del plano de corte.
- En touch, los gestos de dos dedos ya no aplican pan y zoom a la vez salvo ruido residual m�nimo, priorizando la intenci�n dominante del usuario.
- En fondos claros, el panel inferior y la pill de navegaci�n dejan de verse como bloques blancos lavados y pasan a funcionar como superficies tonales oscuras, legibles y visualmente integradas con el environment.
- La desactivaci�n de `Explode` queda restringida a las dos acciones esperadas por UX:
  - volver a pulsar el bot�n `Explode`
  - llevar el slider a `0`

---

## Fase 0 � Investigaci�n y Selecci�n T�cnica _(Pre-Git)_

> Trabajo documentado en sesiones de Antigravity (conversaci�n `c5f61e42`)

- Investigaci�n de tendencias de portafolio Tech Artist (Hard Surface / Drones)
- Selecci�n de modelo objetivo: Drone Sci-Fi de alto rendimiento
- Definici�n de features t�cnicos: Vista Explosionada, Rayos X, Delineado T�cnico
- Definici�n de estilo UX/UI: Glassmorphism Premium
- Estudio de UI/UX (Moodboard + Referencias T�cnicas)
- Diagramaci�n y Wireframing (Grid System 12 columnas, wireframes de flujo, atomizaci�n UI)

---

## Fase 1 � Planificaci�n y Configuraci�n _(Pre-Git)_

> Documentado en roadmap original (`c5f61e42/task.md`)

- Creaci�n de carpeta `desarrollo/` con estructura de proyecto
- Creaci�n de hoja de ruta detallada (HOJA_DE_RUTA.md)
- Configuraci�n de Git y Git LFS
- Definici�n de Namespaces, Assembly Definitions (Core, UI, Content)

---

## 2025-12-04 � Implementaci�n Inicial del Prototipo

### Core Systems (Fases 3.5�3.16 completadas)

> M�s de 40 managers/sistemas implementados en una sesi�n masiva.
> Documentado en conversaci�n `ae9e51bf`.

**Arquitectura y Datos:**

- `GameManager` (Singleton de estado global)
- `DronePartData` ScriptableObjects (base de datos de partes)
- `EventBus` (sistema de desacoplamiento)
- `Singleton<T>` gen�rico (refactorizado a todos los managers)

**C�mara y Escena:**

- `OrbitCameraController` (Cinemachine � Orbit, Pan, Zoom)
- `SelectionManager` (Raycasting + Highlight visual)
- `ExplodedViewManager` (desplazamiento matem�tico con slider)

**UI Glassmorphism:**

- Design System (USS/Theme.uss)
- Layout Principal (MainLayout.uxml � barra lateral, header)
- Panel de Detalles con Data Binding
- `DetailsPanelController` (conexi�n SelectionManager ? UI)

**Audio y Feedback:**

- `AudioManager` (Singleton + control de volumen + SaveSystem)
- `TooltipSystem` (UI flotante)
- `NotificationManager` (Toast messages)

**Optimizaci�n y Quality:**

- `QualityManager` (Dynamic Resolution)
- `FPSCounter` (Debug Overlay)
- `ObjectPooler` (gesti�n de memoria)
- `MaterialController` (Property Blocks)
- `WebGLOptimizer` (optimizaciones de plataforma)

**Input y Debugging:**

- `InputManager` (abstracci�n de entrada)
- `RuntimeConsole` (log en pantalla)

**Persistencia y Assets:**

- `SaveSystem` (PlayerPrefs Wrapper)
- `AssetLoader` (Async Loading Stub)

**Features Extra:**

- `ScreenshotManager` (captura para tesis)
- `KeyboardShortcuts` (atajos profesionales)
- `PerformanceMonitor` (m�tricas detalladas)
- `SettingsPanel` (UI de configuraci�n)
- `AccessibilityManager` (opciones de accesibilidad)
- `ErrorHandler` (Fallback global)

**View Modes (7 Shaders Custom):**

- `ViewModeManager` � X-Ray, Blueprint, Solid, Realistic, Wireframe, Ghosted, Thermal
- `ViewModeToolbar` (UI de cambio)
- `ClippableLit.shader` (corte transversal)
- `XRay.shader` (Fresnel + doble pass)
- `Blueprint.shader` (grid t�cnico + outline)
- `Thermal.shader` (gradiente de calor animado)
- `Wireframe.shader` (geometry shader)
- `SolidColor.shader` (con outline)
- `Ghosted.shader` (fresnel + depth fade)

**Part Catalog y Visibilidad:**

- `PartCatalogManager` (b�squeda y listado)
- `PartCatalogUI` (scroll, filtros, visibilidad)
- `PartVisibilityManager` (ocultar/mostrar/aislar con fade)

**Advanced Features:**

- `CrossSectionManager` (cortes X/Y/Z con plano visual)
- `DroneStateController` (On/Off, Idle, Flying, StartingUp, ShuttingDown)
- `EnhancedInfoPanel` (Detalles + Hide/Isolate/Focus)
- `ModularPartsSystem` (Slots + Swap con animaciones)

**Engineer Tooling (Fase 3.16):**

- `AssemblyGuideManager` + `AssemblyStepUI` (gu�a paso a paso)
- `MeasurementTool` (distancia, �ngulo, radio)
- `ConnectionPointsViewer` + `ConnectionPointMarker`
- `BillOfMaterialsManager` (CSV export, totales)
- `AnnotationSystem` (notas 3D con billboard)
- `AssemblyChecklist` (verificaci�n con progreso)
- `EngineerToolbar` (acceso r�pido)

### Git Commits (2025-12-04)

| Hash      | Mensaje                                                                    |
| --------- | -------------------------------------------------------------------------- |
| `6d8ddf9` | feat: Complete implementation of WebGL drone visualization prototype       |
| `c935277` | feat: Add scene setup utilities and AssemblyGuideData                      |
| `b7a0e80` | docs: Add README, CONTRIBUTING, setup guide and improve code documentation |
| `1c19ecc` | feat: Add complete project assets and documentation                        |
| `99f38a5` | docs: Add comprehensive presentation design system for Kimi K2             |
| `ad44d00` | docs: Consolidate presentation docs into single folder                     |

---

## 2025-12-05 � Landing Page y GitHub Pages

### Trabajo Realizado

- **Landing Page Premium**: HTML + CSS + GSAP scrollytelling
- **Doc Viewer**: Visor de documentaci�n integrado con soporte Mermaid
- **GitHub Pages**: Configuraci�n y deploy (`.nojekyll`, fix 404)
- **Mobile**: Swipe navigation, zoom Mermaid lightbox
- **M�ltiples iteraciones de fix**: CSS specificity, layout duplicates, JS robustez

### Git Commits (2025-12-05)

| Hash      | Mensaje                                                            |
| --------- | ------------------------------------------------------------------ |
| `2e5a79c` | feat: Add complete project support materials                       |
| `17152fe` | feat: Premium landing page + fix cronograma dates                  |
| `9f92050` | feat: Awwwards-style landing page with GSAP scrollytelling         |
| `f2943c7` | fix: Inline CSS/JS in landing page for better portability          |
| `4fca23f` | fix: Add ScrollToPlugin and link local docs                        |
| `2e0f34b` | fix: Add missing Unity project configuration files                 |
| `a8ce0f0` | fix: Optimize cursor performance and fix features/docs visibility  |
| `71f6b90` | feat: Redesign landing page - Minimalist & Seamless Transition     |
| `720bfee` | feat: Add scroll pinning effect and restore documentation links    |
| `2af00e0` | fix: Increase mobile margins and padding                           |
| `072402d` | fix: Significantly increase mobile margins to 3rem                 |
| `309d7e2` | fix: Resolve CSS specificity issue to ensure mobile margins        |
| `2fb1a4f` | feat: Implement Integrated Doc Viewer with Mermaid Support         |
| `f34af03` | feat: Enhanced Doc Viewer with Nav, Swipe, and Path Fixes          |
| `54eb9a5` | fix: Remove duplicate section and refine layout                    |
| `6b1a651` | fix: Refactor HTML layout to Section > Container                   |
| `5b99de8` | fix: Restore Doc section and restrict grid width                   |
| `ad512e0` | fix: Repair malformed HTML in Features section                     |
| `78e04a7` | fix: Restore advanced Doc Viewer JS logic                          |
| `20a1cc8` | fix: Robust JS logic for DocViewer                                 |
| `c11ee5b` | feat(docs): refine doc viewer, fix layout duplicates, add PDF PDFs |
| `dc0cf8f` | chore: add .nojekyll to fix github pages 404                       |
| `219524c` | chore: remove interfering unity-build workflow to unblock pages    |
| `4fe2e1f` | feat(mobile): add swipe nav, hide arrows, mermaid lightbox         |

---

## 2025-12-08 � Migraci�n React + Framer Motion

### Trabajo Realizado

> Documentado en `MICROINTERACTIONS_PLAN.md`

- Setup React + Vite + Framer Motion
- Arquitectura de componentes para micro-interacciones tem�ticas por card
- 7 animaciones �nicas dise�adas (X-Ray scan, explosion, blueprint, render, shader, compile, color grading)
- Fallback: mantenimiento de versi�n HTML est�tica

### Git Commits (2025-12-08)

| Hash      | Mensaje                                                                      |
| --------- | ---------------------------------------------------------------------------- |
| `0c80d85` | feat: setup React + Framer Motion, fix visibility bug, remove card numbering |
| `85d0845` | fix: temporarily use static HTML deployment for GitHub Pages                 |
| `70df5cc` | feat: complete React architecture with component structure                   |

---

## 2025-12-11 � Compilaci�n Fixes y Git LFS

### Trabajo Realizado

> Documentado en conversaci�n `23c3d7bb` (Debugging Edge Detection)

- Configuraci�n de Git LFS para assets binarios
- Fix de errores de compilaci�n: `DronePartData` propiedades faltantes
- Fix de UI Toolkit syntax errors en 8+ archivos (`borderRadius`, `borderWidth`, `placeholder`)
  - `NotificationManager.cs`, `LoadingController.cs`, `EngineerToolbar.cs`
  - `EnhancedInfoPanel.cs`, `AssemblyStepUI.cs`, `PartCatalogUI.cs`
  - `TooltipSystem.cs`, `ViewModeToolbar.cs`, `SettingsPanel.cs`
- Resolvidos warnings de USS e Input System

### Git Commits (2025-12-11)

| Hash      | Mensaje                                                                |
| --------- | ---------------------------------------------------------------------- |
| `5669c25` | chore: configure Git LFS for binary assets (models, textures, audio)   |
| `06fb92a` | Fix compilation errors: Updated DronePartData, fixed UI Toolkit syntax |
| `479fb69` | Resolved USS warnings and updated task list                            |

---

## 2026-02-10 � UI Profesional + Shaders Integration

### Trabajo Realizado

- **Step 1-2**: Reset button, Info Sheet, Visual Viewport Shift
- **Shader Integration**: Todos los shaders (X-Ray, Blueprint, Thermal, etc.) conectados a UI con cycling
- **High-DPI Mobile**: Redimensionamiento completo (botones 100px, fuentes 24px+)
- **Iteraciones de posicionamiento**: M�ltiples ajustes de UI (530px ? 150px vertical)

### Git Commits (2026-02-10)

| Hash      | Mensaje                                                                                     |
| --------- | ------------------------------------------------------------------------------------------- |
| `d38744f` | feat: Implement Professional UI (Step 1 & 2) - Reset, Info Sheet, Visual Viewport Shift     |
| `47857c5` | Refactor: Renamed UI categories (Electronics/Energy) and fixed UIManager compilation errors |
| `6d0952d` | Feat: Integrated all shaders (XRay, Blueprint, Thermal, etc.) with UI cycling               |
| `26b22c3` | Style: Resized UI for High-DPI Mobile (1320x2860), buttons 100px, fonts 24px+               |
| `12de251` | Style: Vertically spread UI elements for High-DPI mobile                                    |
| `6277396` | Style: Lowered UI groups by 40px to tighten layout                                          |
| `67a93d9` | Style: Aggressively lowered UI groups                                                       |

---

## 2026-02-11 � Polish UI + Smart Hotspots

### Trabajo Realizado

- **Selection Label**: Fade out/in al abrir/cerrar Details Sheet (safe opacity)
- **Floating Details Sheet**: Bottom 260px, botones empujan 480px
- **Smart Hotspots**: Sistema de hotspots Screen-Space (UI Toolkit)
- **URP Asset Tool**: Herramienta de editor para gestionar rendering
- **2026 Minimalist Aesthetic**: Redise�o completo de Hotspots y MainTheme
- **Bug Fixes**: Auto-open sheet, layout z-order, slider hidden, shader buttons

### Git Commits (2026-02-11)

| Hash      | Mensaje                                                             |
| --------- | ------------------------------------------------------------------- |
| `c86f3cc` | Style: Lowered UI to limit (Menu @ 180px, Slider @ 300px)           |
| `5eab07e` | Style: Lowered UI again by 30px                                     |
| `4dc6ce6` | Polish: Selection Label fades out when Details Sheet opens          |
| `32d49b5` | Revert "Polish: Selection Label fades out when Details Sheet opens" |
| `dead62e` | Polish: Selection Label fades out (Safe Opacity Implementation)     |
| `3cc5d6a` | Style: Floating Details Sheet (Bottom 260px)                        |
| `e7e6eac` | Polish: Synced BottomBar and DetailsSheet animation                 |
| `cdb3d1b` | Style: Reduced button shift by 60px                                 |
| `62a2866` | Style: Lowered Sheet assembly by 100px                              |
| `83c1820` | Fix: Restored Details Sheet height to 420px                         |
| `63e02ff` | Feat: Smart Hotspots (Screen-Space UI Toolkit)                      |
| `f6633de` | Fix: Remove duplicate CSS import                                    |
| `d8a606b` | Fix: Resolve CS0246 (Singleton namespace) and USS warnings          |
| `763eb59` | Fix: Resolve CS0246 (SelectionManager, DronePartData)               |
| `6383b06` | Feat: Add URP Asset Tool & Polish UI Spacing (8pt grid)             |
| `17ae084` | feat(ui): Redise�o completo Hotspots y MainTheme (2026 Minimalist)  |

---

## 2026-02-12 � Bug Fixes Iterativos (9 Fases)

### Trabajo Realizado

- **9 fases de bug fixes** resolviendo interacciones UI:
  - Auto-open info sheet (solo hotspot, no part click)
  - Posicionamiento matem�tico y stacking din�mico
  - Z-Order blocking y Thermal Button Layout
  - Info Sheet responsive offset, drag dismiss, zoom block
  - Compilaci�n: constantes duplicadas, campos no usados
  - Click blocking y text selection en Info Sheet
  - CSS warnings y C# picking logic
  - Text selection extendido a Full Info Sheet
  - Block 3D Selection on UI Interaction
- **WebGL Build**: Asegurar tracking del build

### Git Commits (2026-02-12)

| Hash      | Mensaje                                                                |
| --------- | ---------------------------------------------------------------------- |
| `d74f893` | fix(ui): Phase 1-2 bug fixes                                           |
| `acea950` | fix(ui): Bug 1 - only auto-open info sheet on hotspot click            |
| `99fb17e` | Fix UI Layout: Correct math positioning, dynamic stacking, slider init |
| `4c37da1` | Fix UI Interaction: Z-Order blocking (Bug 5) and Thermal Button Layout |
| `076acf6` | Implement Phase 3: Info Sheet Polish (Responsive Offset, Drag Dismiss) |
| `d8eefbf` | Fix Compilation Errors: Duplicate Constants & Unused Fields            |
| `8db5f93` | Implement Phase 5: Info Sheet Click Blocking & Text Selection          |
| `8e46c79` | Fix Syntax & API Warnings (Phase 6)                                    |
| `b26641a` | Fix CSS Warnings & Enforce C# Picking Logic (Phase 7)                  |
| `8c3406c` | Extend Text Selection to Full Info Sheet (Phase 8)                     |
| `4d618d2` | Block 3D Selection on UI Interaction (Phase 9)                         |
| `9007d13` | Ensure WebGL Build is tracked                                          |

---

## 2026-02-18 � UI/UX Overhaul v2.1 (Sesi�n Actual)

### Trabajo Realizado

> Plan documentado en `implementation_plan.md` y `PLAN_UI_OVERHAUL_v2.md`

**Hero Menu Redesign:**

- 4 botones principales (Explore, Select Device, About, Exit)
- Device selector integrado como submenu
- Home button
- Space Grotesk font + Inter global

**Visual Unification (Match Web Landing Page):**

- Accent color: Cyan (`#00D9FF`) ? Blanco (`#FFFFFF`)
- 9 selectores actualizados (`.btn-tag--active`, `.shader-mode-btn--active`, `.env-preset-btn--active`, slider dragger, tag-dot-accent, etc.)
- Background: `#050505` (near-black, matches web)
- Typography: Inter global, Space Grotesk Regular titles
- Button styling: transparent + pill

**Bug Fixes � Phase 1:**

1. Device selector width: `320px ? 400px` (clipping fix)
2. Canvas background: `EnvironmentController.ApplyPreset("Studio")` bg fixed to `#050505`
3. Info panel text wrapping: `white-space: normal` en inner TextField + `align-items: flex-start`
4. Close button: `StopPropagation()` to prevent header toggle re-opening
5. Buttons position: `.ui-shifted 56% ? 46%`
6. Ghost clicks: `display: none` en 4 estados hidden
7. Slider centering: `translate: -50% 0` + padding sim�trico `16px 32px`
8. USS syntax fix: Removed invalid `~` combinator and `overflow: visible`

**Documentation � Phase 4:**

- `Informe_final/Desarrollo_App/` folder structure
- `CHANGELOG.md` (this file)
- `DESIGN_TOKENS.md` (full design system spec)
- `TECHNOLOGY_STACK.md` (all tools and technologies)
- Archived roadmaps (`HOJA_DE_RUTA_v1.md`, `PLAN_UI_OVERHAUL_v2.md`)
- Session log

### Git Commits (2026-02-18)

| Hash      | Mensaje                                                                                    |
| --------- | ------------------------------------------------------------------------------------------ |
| `6b3b3ad` | fix: hero menu hitbox + redesign (4 buttons, device selector, home btn)                    |
| `140185e` | style: visual redesign to match web landing page                                           |
| `42c6e90` | Fix UI bugs: About close, click blocking, info button, bg black, fonts                     |
| `51461c3` | Refactor UX: Unified Bottom Sheet (Delete AboutPanel, DeviceSelector)                      |
| `0229672` | feat(ui): implement Hero Submenus, Space Grotesk font, black bg fix                        |
| `93c4562` | fix(ui): resolve uxml syntax error and remove unused code                                  |
| `332a9b6` | fix(ui): apply Space Grotesk font globally                                                 |
| `e3d72e5` | fix(ui): update hero title to Title Case and refine spacing                                |
| `b03d286` | fix(ui): match web typography (Inter global, Space Grotesk Regular titles)                 |
| `fa62502` | fix(ui): match web button styling (transparent + pill)                                     |
| `ef2ec40` | feat(ui): match web styling (pills, spacing, no scrollbars)                                |
| `dc4d83b` | docs: update master plan v2.1 with user feedback                                           |
| `16dc25f` | fix(ui): Phase 1+2 - white accent, ghost clicks, slider, device width, bg                  |
| `1a95185` | docs: Phase 4 - Create Desarrollo_App folder with CHANGELOG, DESIGN_TOKENS                 |
| `0d6754a` | fix(ui): remove invalid USS syntax (~combinator, overflow:visible)                         |
| `cfd7ee1` | fix(ui): studio bg #050505, slider centered, text wrap, close btn fix                      |
| `55c26cf` | docs: comprehensive CHANGELOG + TECHNOLOGY_STACK (all tools, AI, MCPs, skills, RAG)        |
| `f25d557` | fix(ui): data-value text wrapping (flex:1 + white-space) + slider drag-container centering |
| `6ff1ba2` | fix(uss): remove all picking-mode from USS (not valid USS property)                        |
| `1399a4f` | fix(ui): text clip, slider horizontal fix, device cyan?white, submenu padding              |
| `ab598c5` | fix(ui): TextFieldLabel text clip, slider flex-start, device scroll overflow               |
| `e257066` | feat(visual): animated gradient skybox for Studio preset                                   |
| `cd33225` | feat(visual): update skybox to radial gradient with pulse animation                        |
| `6ecbd7c` | docs: add BITACORA.md formal project log                                                   |
| `5514ad1` | fix(ui): resolve device menu clipping + docs: align Bit�cora to Cronograma                 |
| `3325d4b` | style(ui): force device menu width to 396.5px (scrollbar fix)                              |
| `b0f9ef5` | docs: enhance BITACORA.md with detailed technical rationale                                |

---

## 2026-02-19 � Phase 3: Grid Submenus + Icon System

### Trabajo Realizado

**Grid Navigation System (Phase 3.1�3.3):**

- Migraci�n de Pins al CategoryMenu; Home/Reset restaurados al TopBar
- Implementaci�n de sistema de iconos placeholder para botones
- Grid de 4 columnas para submen�s (shader, category, environment)
- UIManager actualizado para soportar clases de grid submenu

**Polish & Bug Fixes:**

- Ajustes agresivos de layout para card rows
- Fix de SelectionManager background click y altura del layout
- Polish final de UI Layout, Spacing e interacciones

### Git Commits (2026-02-19)

| Hash      | Mensaje                                                                       |
| --------- | ----------------------------------------------------------------------------- |
| `472eb99` | feat(ui): move Pins to CategoryMenu, restore Home/Reset to TopBar (Phase 3.1) |
| `d2a60fa` | feat(ui): implement icon system for buttons (Phase 3.2)                       |
| `1f00e51` | feat(ui): implement 4-column grid system for submenus (Phase 3.3)             |
| `46f03d8` | feat(ui): implemented grid submenus for shader, category, and env panels      |
| `cf49b8b` | fix(ui): update UIManager to support new grid submenu classes                 |
| `65b05b2` | Fix: Aggressive layout adjustments for 4-card rows                            |
| `3bf55a4` | Fix: SelectionManager bg click and Layout height (Phase 8)                    |
| `1ee65f1` | Final Polish: UI Layout, Spacing, and Interaction Fixes Complete              |

---

## 2026-02-20 � Architecture Refactoring: Memory Leaks + God Class

### Trabajo Realizado

> Plan documentado en `implementation_plan.md` y `ARCHITECTURE_AUDIT_REPORT.md`

**Step 1: UI Toolkit Memory Leaks Resolvidos:**

- Implementaci�n de patr�n `AddCleanup` con cola de `System.Action` en `UIManager.cs`.
- Conversi�n de +50 lambdas an�nimos a instancias cacheadas.
- Llamada obligatoria a `UnsubscribeFromUIEvents()` dentro de `OnDisable()`.
- Eliminaci�n total del vector de fuga de memoria en listeners.

**Step 2: Desacoplamiento de UIManager (God Class):**

- Carpeta y namespace `WebGL.UI.Panels` para l�gica local de interfaz.
- Extracci�n de `UIEnvironmentPanel.cs` y `UIAnalyzePanel.cs`.
- Reducci�n de m�s de 120 l�neas en `UIManager`.

**Fix: Hotspot Binding:**

- Restauraci�n de `hotspotBtn` initialization binding roto durante el refactor.

### Git Commits (2026-02-20)

| Hash      | Mensaje                                                                                         |
| --------- | ----------------------------------------------------------------------------------------------- |
| `a1a60ae` | refactor: implement comprehensive UI Toolkit event cleanup in UIManager to prevent memory leaks |
| `8a949ce` | refactor: dismantle UIManager God Class by extracting UIEnvironmentPanel and UIAnalyzePanel     |
| `487072a` | fix(ui): restore missing hotspotBtn initialization binding that broke the pins toggle           |

---

## 2026-02-23 � FASE 1: Arquitectura Limpia + Iteraciones UX 1�6

### Trabajo Realizado

> Plan documentado en `PHASE3_CHANGELOG.md` y `UX_UI_AUDIT_REPORT.md`

**Architecture Refactoring (C01�C05):**

- Refactoring completo de arquitectura: memory leaks, god class dismantling, input decoupling
- Centralizaci�n de input blocking en `InputManager` con UI-awareness
- Cleanup de dead code: `ApplyDefaultRenderSettings`, `GlobalInputBlocked`, `GameManager` state duplication
- Estandarizaci�n de null-safety patterns
- Revert de `RegisterButtonInputBlockers` (su eliminaci�n rompi� submen�s)

**UX Redesign � Iteraciones 1�6 (Sistema de 3 Modos):**

- Expansi�n de `AppStateMachine` con modos Analyze y Studio
- Reestructuraci�n de `MainLayout.uxml` con sistema de 3 modos
- Creaci�n de `UIModeController` + rewire de `UIManager`
- Panel de Cross-Section en modo Analyze
- Integraci�n final: eliminaci�n de `UIPopupController`, limpieza de CSS
- Restauraci�n de funciones v�a Action Bars (Tools/Analyze/Studio)

**Polish:**

- Aesthetic pass: compact UI, glassmorphic panels, tighter spacing
- Fix de info panel, CUT tool, slider persistence, category menu auto-open
- Fix de accesibilidad (`CS0051 ActivateMode`)

### Git Commits (2026-02-23)

| Hash      | Mensaje                                                                                                     |
| --------- | ----------------------------------------------------------------------------------------------------------- |
| `04df7a1` | refactor(phase3): complete architecture refactoring � memory leaks, god class dismantling, input decoupling |
| `9e24ed5` | refactor(phase4): centralize input blocking in InputManager, add UI-awareness to camera and keyboard        |
| `b40e9aa` | docs: update changelog with Phase 4 documentation                                                           |
| `1607733` | refactor(phase5): cleanup dead code � remove ApplyDefaultRenderSettings, GlobalInputBlocked bridge          |
| `4b80bd5` | refactor: remove InputBlocked + RegisterButtonInputBlockers � sole UI guard is now IsPointerOverUI()        |
| `acea37a` | fix: remove duplicate closing brace in SelectionManager.HandleHover()                                       |
| `e7f79d8` | fix: restore StopPropagation on all buttons � fixes broken submenu clicks                                   |
| `b89e2f7` | revert: restore RegisterButtonInputBlockers � removal broke all submenu clicks                              |
| `e86ff95` | refactor: standardize null-safety patterns (Task 3)                                                         |
| `7515117` | docs: update PHASE3_CHANGELOG with Phase 5, Audit Tasks 1�3, and commit history                             |
| `22dd3af` | docs: add Phase 2 UX Redesign plan and changelog scaffold                                                   |
| `2673e5a` | feat(phase2-iter1): expand AppStateMachine with Analyze and Studio modes                                    |
| `70b2a01` | feat(phase2-iter2): restructure MainLayout.uxml with 3-mode system + USS styles                             |
| `f125f6f` | feat(phase2-iter3+4): create UIModeController + rewire UIManager for 3-mode system                          |
| `66673f3` | feat(phase2-iter5): add Cross-Section UI panel in Analyze mode                                              |
| `50dc291` | feat(phase2-iter6): cleanup & integration final � delete UIPopupController, deprecate toolbars              |
| `9f737a0` | fix(ui): restore all functions via Tools/Analyze/Studio action bars                                         |
| `ef7c6a4` | fix: CS0051 ActivateMode accessibility + escape & in UXML tooltip                                           |
| `4b5e08a` | fix: resolve 4 functional issues � info panel, CUT tool, slider, category menu                              |
| `d54e541` | fix: CUT cross-section, slider persistence, category menu auto-open                                         |
| `463df98` | style: aesthetic pass � compact UI, glassmorphic panels, tighter spacing                                    |
| `40686f5` | chore: clean up duplicate comments in Theme.uss                                                             |
| `8421b0b` | docs(ux): add UX/UI audit report and plan Iteration 7                                                       |

---

## 2026-02-24 � FASE 1: Iteraciones UX 7�11 (Grid Minimalista)

### Trabajo Realizado

- **Iter 7**: Implementaci�n de grid UI minimalista: icon-only buttons, 4-col card grid, sliders alineados, containers invisibles
- **Iter 8**: Clipping universal de cross-section + fix slider + axis btn sizing
- **Iter 9**: Fix z-order bottom bar + overhaul de tama�os + cross-section blue
- **Iter 10**: Navegaci�n card-grid unificada, dual-plane cross-section, UX overhaul
- **Iter 11**: Fix 7 quejas de UX � sizes, layout, gestures, filter

### Git Commits (2026-02-24)

| Hash      | Mensaje                                                                           |
| --------- | --------------------------------------------------------------------------------- |
| `b96132d` | style(ux): implement Iteration 7 � minimalist grid UI system                      |
| `59f80d9` | iter8: universal cross-section clipping + slider fix + axis btn sizing            |
| `7396b96` | iter9: fix bottom bar z-order + sizes overhaul + cross-section blue               |
| `e483d70` | Iteration 10: Unified card-grid navigation, dual-plane cross-section, UX overhaul |
| `800f4d0` | iter11: fix 7 UX complaints � sizes, layout, gestures, filter                     |

---

## 2026-02-25 � FASE 2: Iteraciones 12�14 (Modos Inspect/Analyze/Studio)

### Trabajo Realizado

- **Iter 12**: Fix cross-section en Realistic, floating (i) info btn, 3-col grids, hide shaders/env
- **Iter 13**: Major mode restructure ? Inspect/Analyze/Studio + isolate + larger cards + true pill
- **Iter 14**: Fix pill bar style, 3-card fit, isolate SetActive, remove FloatingInfoBtn, device scroll
- **Iter 14-fix**: Actions-row `border-radius: 36px` para pill shape real

### Git Commits (2026-02-25)

| Hash      | Mensaje                                                                                          |
| --------- | ------------------------------------------------------------------------------------------------ |
| `079ba8c` | iter12: fix cross-section in Realistic, floating (i) info btn, 3-col grids, hide shaders/env     |
| `beb8e36` | iter13: Major mode restructure � Inspect/Analyze/Studio + isolate + larger cards + true pill     |
| `345df4d` | iter14: fix pill bar style, 3-card fit, isolate SetActive, remove FloatingInfoBtn, device scroll |
| `d5fda52` | iter14-fix: actions-row border-radius 36px (half of 72px height) for true pill shape             |

---

## 2026-02-26 � FASE 2: Auditor�a WCAG + Iconos Procedurales

### Trabajo Realizado

**Auditor�a de Accesibilidad (WCAG 2.1 AA):**

- Fix de contraste, touch targets, font sizes, grid, dead CSS, border-radius
- Bottom pill m�s grande (112px, iconos de 56px) + re-auditor�a completa
- Fix de device-option hover scale (prevenci�n de clipping)

**Sistema de Iconos Procedurales:**

- `ProceduralCutIcon`: Microinteracci�n de slice diagonal con SpringFloat recovery
- Dise�o completo de 8 iconos procedurales: Info, Filter, Studio, Pins, Inspect, Analyze, Explode, Home
- Integraci�n en `MainLayout.uxml` con routing de PointerEvents al Button padre
- Fix de CSS background-images y TrickleDown phase para pointer events
- Documentaci�n matem�tica y f�sica de la arquitectura de iconos

**Documentaci�n T�cnica:**

- Correcci�n de errores factuales en 4 reportes de auditor�a + plan de remediaci�n
- Documentaci�n comprensiva t�cnica para tesis (`01_Arquitectura`, `02_Manual_Tecnico`, etc.)

### Git Commits (2026-02-26)

| Hash      | Mensaje                                                                                         |
| --------- | ----------------------------------------------------------------------------------------------- |
| `e9a73de` | audit: fix WCAG contrast, touch targets, font sizes, grid, dead CSS, border-radius              |
| `70fed05` | fix: restore actions-row height to 72px (mode-btn 80px overflow)                                |
| `a6841b9` | feat: bigger bottom pill (112px, 56px icons) + full re-audit pass                               |
| `1efb9fc` | fix: remove device-option hover scale to prevent clipping                                       |
| `e027474` | Refactor ProceduralCutIcon: One-click precise diagonal slice microinteraction                   |
| `b15d9ee` | Refactor ProceduralCutIcon: Replace linear return sweep with SpringFloat recovery               |
| `eeb7bb3` | audit: correct factual errors across all 4 audit reports + add remediation plan                 |
| `8bd87aa` | UI: Overhauled procedural icons � Info, Filter, Studio, Pins, Inspect, Analyze                  |
| `4da6e37` | Fix compiler error in ProceduralPinsIcon                                                        |
| `e5781ab` | UI: Add procedural icons to IconGallery test scene UXML                                         |
| `74d3f21` | UI: Complete redesign of Inspect, Pins, Analyze, Studio icons + fixes for Explode, Info, Filter |
| `33cfb21` | UI: Refine Home, Info, Pins, Analyze and completely redesign InspectIcon                        |
| `3f35dc3` | Docs: Comprehensive mathematical & physical architecture documentation for Procedural UI Icons  |
| `59e027c` | UI: Integrate procedural icons into MainLayout.uxml and route PointerEvents to Button           |
| `a35302f` | UI Fix: Safely remove CSS background-images preventing syntax errors                            |
| `fd098b3` | UI Fix: Use TrickleDown phase in ProceduralIconBase for PointerEvents                           |
| `eb5e918` | docs: add comprehensive technical documentation for thesis                                      |
| `dbf7616` | UI Fix: Inverse Pins button initial Active State, slow down procedural physics �2               |

---

## 2026-02-27 � FASE 2: Slider UX + Font Rendering + WebGL Config

### Trabajo Realizado

**Interacciones UI:**

- Delayed scheduling para button click actions (apreciar animaciones)
- Mutual exclusion entre Info Sheet y Card Menus
- Inversi�n de estados normal/hover para ProceduralExplodeIcon

**Font Rendering (5 iteraciones):**

- Asignaci�n de PanelSettings + auto-generaci�n de SDF Font Assets ? fracaso
- Revert a legacy font rendering (`-unity-font-definition: initial`)
- Correcci�n de USS warnings y WebGLBuildFixer

**Slider Hitbox Debugging (11 iteraciones):**

- Redise�o de Slider Draggers: hollow rings ? solid hover ? blue active
- Investigaci�n de Yoga layout vs CSS en UI Toolkit
- Root cause: `position:relative` en UI Toolkit no mueve hit-test rect
- M�ltiples intentos con `position:absolute`, scale, physical dimensions
- Creaci�n y eliminaci�n de `SliderHitboxDebugger.cs` (namespace collision)
- Revert final a styles de `d341e58`
- Fix de `picking-mode=Position` en slider labels

**WebGL Optimization:**

- Configuraci�n de WebGL optimization + conversi�n de colores Gamma?Linear en USS

### Git Commits (2026-02-27)

| Hash      | Mensaje                                                                                   |
| --------- | ----------------------------------------------------------------------------------------- |
| `e54c006` | UI Fix: Delayed scheduling for button clicks, scale header buttons to 64px                |
| `0b905d3` | UI Fix: Card menus instantly hide when Info Sheet is toggled active                       |
| `76cebcf` | fix: assign PanelSettings to UIDocument + auto-generate SDF Font Assets                   |
| `b58cd92` | fix: remove -unity-font-definition:initial that blocks SDF font preview                   |
| `52e55f0` | fix: add -unity-font-definition with SDF font asset URLs                                  |
| `df28a7a` | fix(fonts): revert to legacy font rendering, remove SDF pipeline                          |
| `de8b660` | fix: resolve USS warnings, refactor WebGLBuildFixer, mitigate layout errors               |
| `6e89d72` | UI Fix: Invert normal and hover states for ProceduralExplodeIcon                          |
| `9b188f2` | UI Fix: Redesign Slider Draggers � hollow 24px rings, solid white 32px hover, blue active |
| `b615611` | UI Toolkit & Manager Bugfixes: font-definition initial, USS tint vars                     |
| `2f7358b` | chore: Record manual user modifications to UI styles and UIManager lifecycle              |
| `423cc74` | UI Fix: Scale 0.75 on large 32px hitbox for Slider Dragger                                |
| `8a6bf1c` | UI Fix: Physical width/height instead of scale for Slider Dragger hitbox                  |
| `d341e58` | feat: WebGL optimization config + Gamma-to-Linear USS color conversion                    |
| `b5f5443` | fix: slider UX overhaul + prevent container close on miss-click                           |
| `6bf60e3` | fix: slider dragger hitbox centering + blue click state                                   |
| `a04f097` | fix: slider hitbox centering + active-only blue + container transparency                  |
| `0de1faa` | fix: remove translate:-50% that caused slider hitbox offset                               |
| `80ac231` | fix(slider): add position:absolute to dragger � fixes hitbox misalignment                 |
| `2344495` | fix(slider): use Yoga flex centering instead of top/margin-top offsets                    |
| `664c41a` | fix: rename SliderHitboxDebugger namespace to avoid WebGL.Debug collision                 |
| `36fc49f` | revert: restore slider USS styles to d341e58 original state                               |
| `cb9b737` | fix: set picking-mode=Position on slider labels and Studio env-slider-groups              |

---

## 2026-03-02 � FASE 3: Implementaci�n Completa (C01�C05 + H01�H11 + F3-00 a F3-13)

### Trabajo Realizado

> Jornada m�s productiva del proyecto: **48 commits** en un solo d�a.

**UI Optimization (Pre-Fase):**

- Prevenci�n de event bubbling en sub-panels
- Reemplazo de fondos semi-transparentes por outlines transparentes (WebGL perf)
- Modo Diagonal Cut en herramienta de Cross-Section
- Eliminaci�n de fondos transl�cidos en topbar buttons

**FASE 1 � Code Quality (C01�C05):**

- **C01**: Eliminaci�n de sistema duplicado `VisualMode` de `ExplodedViewManager`
- **C02**: Consolidaci�n de triple publicaci�n de eventos ? canal �nico `StateChangedEvent`
- **C03**: Agrupaci�n de campos del Bottom Sheet en 3 secciones Foldout colapsables
- **C04**: Enforcement de touch targets m�nimos de 44px (WCAG/HIG compliance)
- **C05**: Extracci�n de Loading/Error UI de C# procedural a UXML+USS declarativo

**Design Sheet:**

- Jerarqu�a visual editorial, altura fija, dividers limpios
- Panel m�s alto, pill centrado, foldouts colapsados

**FASE 2 � Architecture Hardening (H01�H11):**

- **H01**: `ServiceLocator` + migraci�n de 4 singletons
- **H02**: Extracci�n de `BaseModeHandler` desde `UIModeController` (InspectHandler, AnalyzeHandler, StudioHandler)
- **H03**: `EventBus` con leak detection + zombie purge autom�tico
- **H04**: Eliminaci�n de 10 archivos de c�digo muerto (ViewModeToolbar, EngineerToolbar, ScreenshotManager, etc.)
- **H06+H07**: Optimizaci�n de ProjectSettings WebGL (StripUnusedMeshComponents, IL2CPP Master, mipStripping)
- **H08**: Eliminaci�n de `Update()` innecesarios ? coroutines/InvokeRepeating
- **H09**: `QualityManager` resoluci�n adaptativa v�a URP renderScale (reflection-based)
- **H10**: Fitts' Law � reducci�n de gap en pill inferior
- **H11**: Onboarding help overlay con bot�n HELP en hero menu

**FASE 3 � Feature Tickets (F3-00 a F3-13):**

- **F3-00**: Hotspot swap fix + slider dragger restyle + disable bg-click deselect
- **F3-01**: Studio render mode � toggle behavior + card visibility
- **F3-02**: Environment cycling � TIME + COLOR buttons
- **F3-03**: Filter � 6� categor�a PAYLOAD (completa grid 3�2)
- **F3-04**: Info ? FAB global (floating action button, visible on selection)
- **F3-05**: Inspect � add MEASURE card + ProceduralMeasureIcon
- **F3-06**: PERF-M02 � reducci�n de `webGLMaximumMemorySize` 512?256 MB
- **F3-07**: Grid 4pt + type ramp normalization
- **F3-08**: Explosion slider � dynamic percentage label
- **F3-09**: Escape key global � cascading dismiss
- **F3-10**: Camera magic numbers ? named constants
- **F3-11**: Double-click unificado � detecci�n movida a `SelectionManager`
- **F3-12**: `DronePartData` limpieza � remove redundant property wrappers
- **F3-13**: WebGL template custom (basado en build existente)

**Bug Fixes:**

- FAB icon full-size, FAB rises con sheet, bg-click deselect, dbl-click exits isolate
- Explode slider inline below cards; toggle resets a 0; bg click keeps sheet
- QualityManager: reflection en lugar de dependencia directa a URP
- ExplodedViewManager: race condition en `Start()` (partes en 0,0,0)
- Ajuste de translate `.ui-shifted` tras cambios de H10

### Git Commits (2026-03-02)

| Hash      | Mensaje                                                                                           |
| --------- | ------------------------------------------------------------------------------------------------- |
| `df66b30` | fix: prevent sub-panel clicks from bubbling; add optimization manual doc                          |
| `85b48a0` | UI Opt: Replace submenu backgrounds with transparent outlines                                     |
| `84883ab` | Feature: Added Diagonal Cut mode to Cross Section tool                                            |
| `71ecf79` | UI Opt: Fix remaining slider containers background to outline styles                              |
| `0d4fd64` | Feature: Clean UI styles � remove slider inner borders, neutralize tool button backgrounds        |
| `de759c6` | UI Opt: Remove translucent background from global topbar buttons                                  |
| `12baf4b` | fix(C01): remove duplicate VisualMode system from ExplodedViewManager                             |
| `47a1600` | fix(C02): consolidate triple event publishing to single StateChangedEvent channel                 |
| `9603d46` | fix(C03): group Bottom Sheet fields into 3 collapsible Foldout sections                           |
| `a5d81a0` | fix(C04): enforce 44px minimum touch targets (WCAG/HIG compliance)                                |
| `b2f6be2` | refactor(C05): extract Loading/Error UI from procedural C# to UXML+USS                            |
| `19d040e` | design(sheet): editorial visual hierarchy, fixed height, clean dividers                           |
| `ec329ea` | design(sheet): taller panel, centered pill, collapsed foldouts                                    |
| `19919e9` | fix(sheet): responsive pill centering + guard inactive coroutine                                  |
| `8f6b11e` | docs(plan): add 30 missing findings from cross-audit analysis                                     |
| `5944d01` | refactor(arch): add ServiceLocator + migrate 4 singletons (H01)                                   |
| `1438817` | refactor(arch): extract mode handlers from UIModeController (H02)                                 |
| `0af9ac6` | feat(arch): add EventBus leak detection + zombie purge (H03)                                      |
| `6fb1bc1` | H04: Eliminar 10 archivos de c�digo muerto                                                        |
| `7ed48d5` | H06+H07: Optimizar ProjectSettings WebGL � StripUnusedMeshComponents, IL2CPP Master               |
| `98842da` | H08: Eliminar Update() innecesarios � CrossSectionManager, LoadingController, ExplodedViewManager |
| `6314c86` | H09: QualityManager resoluci�n adaptativa v�a URP renderScale (reflection-based)                  |
| `5efda91` | H10: Fitts' Law � reducir gap pill inferior                                                       |
| `55b66f3` | FIX: QualityManager � usar reflection en lugar de dependencia directa a URP                       |
| `62aec7c` | FIX: ExplodedViewManager � race condition en Start() causaba partes en (0,0,0)                    |
| `f3d119d` | FIX: Ajustar translate de .ui-shifted tras cambios de H10                                         |
| `671a0a8` | H11: Add first-visit onboarding help overlay with HELP button in hero menu                        |
| `5f32379` | UX: Group HELP into ABOUT submenu, hero typography hierarchy, type ramp                           |
| `f1cf034` | UX: Hero title SemiBold, about-desc lighter gray                                                  |
| `8b543e2` | F3-00: hotspot swap fix + slider dragger restyle + workplan                                       |
| `9134668` | F3-00b: disable background-click deselect + fix slider active blue                                |
| `433b83d` | F3-00c: fix hotspot highlight stacking + slider active blue                                       |
| `fb9453e` | F3-01: Studio render mode � toggle behavior + card visibility                                     |
| `6484975` | F3-02: Environment cycling � TIME + COLOR buttons                                                 |
| `663ebca` | F3-03: Filter � add 6th category PAYLOAD (completes 3�2 grid)                                     |
| `cd6a464` | F3-04: Info ? FAB global � extract ToolInfoBtn to floating action button                          |
| `3e91a3d` | fix: FAB icon full-size, FAB rises with sheet, bg-click deselect                                  |
| `83f0278` | refactor: explode slider inline below cards instead of sub-panel                                  |
| `9bb5bb3` | fix: explode toggle resets to 0, bg click keeps sheet, remove X close btn                         |
| `752d81b` | F3-05: Inspect � add MEASURE card + ProceduralMeasureIcon                                         |
| `da8908c` | F3-06: PERF-M02 � reduce webGLMaximumMemorySize 512?256                                           |
| `3945e18` | F3-07: Grid 4pt + type ramp normalization                                                         |
| `40a2d40` | F3-08: Explosion slider � dynamic percentage label                                                |
| `b1683ba` | F3-09: Escape key global � cascading dismiss                                                      |
| `a959c03` | F3-10: Camera magic numbers ? named constants                                                     |
| `b32b2b5` | F3-11: Double-click unificado � move detection to SelectionManager                                |
| `8a4808e` | F3-12: DronePartData limpieza � remove redundant property wrappers                                |
| `8ba3b50` | F3-13: WebGL template custom (basado en build existente)                                          |

---

## 2026-03-03 � Fixes Finales + Documentaci�n

### Trabajo Realizado

- FAB info visible con Bottom Sheet abierto + explode slider reset a 0
- Explode slider restaura �ltimo valor (o 50% por defecto)
- Plan de trabajo final � auditor�a completa + plan 4 d�as
- Checkpoint pre-refactor: Info Bar UI redesign

### Git Commits (2026-03-03)

| Hash      | Mensaje                                                            |
| --------- | ------------------------------------------------------------------ |
| `f5cd7c9` | fix: FAB info visible con sheet abierto + explode slider reset a 0 |
| `1dcf472` | fix: explode slider restaura �ltimo valor (o 50% por defecto)      |
| `d378d23` | docs: plan de trabajo final � auditor�a completa + plan 4 d�as     |
| `78645a9` | chore: checkpoint before Info Bar UI refactor                      |

---

## Pendiente

- **Informe Final LaTeX**: Redacci�n APA 7 UNAD (Introducci�n, Marco Te�rico, Resultados, Conclusiones)
- **Pruebas de usabilidad**: SUS (System Usability Scale), NASA-TLX
- **Video demostraci�n**: Showreel final
- **Build WebGL final**: Deploy verificado
- **Exportar GLBs**: 11 partes individuales + drone completo

---

## Estad�sticas

| M�trica           | Valor                                                                                                               |
| ----------------- | ------------------------------------------------------------------------------------------------------------------- |
| Total commits     | 234                                                                                                                 |
| Per�odo           | Dec 4, 2025 � Mar 3, 2026 (90 d�as)                                                                                 |
| D�as activos      | 16                                                                                                                  |
| C# Scripts        | 91 (~14,778 l�neas)                                                                                                 |
| Custom Shaders    | 9 (X-Ray, Blueprint, Thermal, Wireframe, WireframeWebGL, SolidColor, Ghosted, ClippableLit, AnimatedGradientSkybox) |
| USS Stylesheets   | 5 (3,561 l�neas)                                                                                                    |
| UXML Layouts      | 4 (502 l�neas)                                                                                                      |
| ScriptableObjects | 11 (DronePartData)                                                                                                  |
| Escenas Unity     | 3                                                                                                                   |

## [Unreleased] - 2026-03-17

### Added

- Thermal Legend UI en MainLayout.uxml con etiquetas dinámicas de temperatura y un gradiente lineal térmico en Theme.uss.
- Sincronización automática de displayMinTemperatureC y displayMaxTemperatureC desde ThermalViewController hacia la UI.

### Changed

- UIAnalyzePanel.cs ahora vincula la visibilidad de la leyenda al seleccionar la opción 'Thermal' del menú Rendering.

### Verified

- Añadidas V003 y V004 en wolfram_verificaciones.md, confirmando matemática y cualitativamente la compresión de las constantes de tiempo térmico (tau) en ThermalSimulationManager.cs por razones de fluidez interactiva en WebGL vs la realidad (7 minutos).

## [Unreleased] - 2026-03-19

### Added

- `ThermalCanonicalContactGraph.asset` como grafo explicito oficial para las 28 piezas canonicas del solver.
- Presets termicos canónicos en `ThermalViewController` para motores, ESC, bateria, brazos, plates y stack central.
- `AGENT_HANDOFF_THERMAL.md` y nueva guia `RETOPOLOGIA_POR_PIEZA.md` con enfoque hibrido 28+55.

### Changed

- `ThermalSimulationManager` ahora resuelve automaticamente un grafo canonico por `Resources` antes de caer al fallback heuristico.
- `ThermalViewController` cachea la leyenda UI y deja de buscar el `UIDocument` en cada refresh.
- `ThermalContactGraphBuilderWindow` apunta por defecto al asset oficial en `Assets/Resources/`.
- `ThermalTestSetup.cs` queda marcado como experimental y fuera del camino oficial.

### Docs

- Sincronizados `README.md`, indices termicos, matriz documental, documentacion tecnica, bitacora, manual tecnico, capitulo 4 y referencias de portafolio.

### Notes

- En esta rama la carga termica oficial sigue gobernada por `DroneStateController`; un slider UI dedicado sigue siendo mejora futura y no debe describirse como ya reintegrado.

## [Unreleased] - 2026-03-31

### Added

- `ImportedDroneRuntimeBinder.cs` para reenganchar el FBX importado al runtime y recachear managers clave.
- `PartRenderCategory.cs` para soportar categorias publicas por renderer (`Structure`, `Propulsion`, `Electronics`, `Fasteners`, `Misc`).
- `PREPARACION_FBX_IMPORTADO.md` como guia operativa del flujo oficial para `x500v2_Drone`.

### Changed

- `UIManager` ahora autocrea managers criticos tambien para escenas sin bootstrap serializado completo.
- `SelectionManager` ahora cae por defecto a la layer `SelectablePart` si el `LayerMask` no fue configurado.
- `SetupImportedDroneThermalTest` ahora asigna layer seleccionable, colliders tipo box, categorias auxiliares y binder runtime.
- `ViewModeManager`, `ExplodedViewManager`, `PartVisibilityManager`, `CrossSectionManager` y `HotspotManager` exponen recacheo o refresh para el dron importado.

### Notes

- `Fasteners` y `Misc` ya son categorias publicas filtrables y aislables como coleccion visual, pero siguen heredando temperatura del ensamblaje padre en el solver V1.
- La validacion final sigue pendiente en Unity Editor con la escena retopologizada completa.

## [Unreleased] - 2026-05-08

### Added

- `holybro_selection_hierarchy.json` como contrato explicito de hotspots, piezas canonicas y subpiezas Blender.
- `SelectionHierarchy` como helper compartido por importacion, runtime, hotspots y auditoria.
- Auditoria adicional para detectar `FastenerRuntimeMarker` en objetos que no provienen de fasteners primitivos.

### Changed

- `HotspotManager` deja de agrupar por keywords y ahora usa membresia canonica explicita.
- `SelectionManager` aplica seleccion por capas: pieza madre, subpieza y fastener individual.
- La deteccion modular de fasteners se limita a `x500v2_fastener_group` y fuentes primitivas equivalentes.
- La restauracion de aislamiento de hotspots conserva el flag de fasteners asociados del grupo original.

### Fixed

- `GUAN-CHENG` ya no entra en `Power Distribution`; la correccion manual vigente lo ubica en `x500v2_landing_gear`.
- `HMX5V-GUAN-DINGWEI` deja de tratarse como tornillo por coincidencia de nombre; `GPSV5-ZHIJIA-LUOMAO` y `HUAN-GUIJIAO` quedan reservados para clasificacion explicita de tuerca/grommet cuando el catalogo Holybro lo confirme.
- El primer click sobre un fastener selecciona su pieza madre; el segundo click dentro del contexto selecciona el fastener aislable.
- `HotspotManager` ya no usa tokens amplios como `m2`, `m3`, `nut` o `screw` para decidir si un miembro debe tratarse como fastener.

## [Unreleased] - 2026-05-09

### Changed

- `SelectionHierarchy` incorpora una lista explicita de no-fasteners estructurales para impedir que `HMX5V-GUAN-DINGWEI` vuelva a entrar al sistema modular.
- `SelectionHierarchy` reconoce explicitamente `HUAN-GUIJIAO` y `GPSV5-ZHIJIA-LUOMAO` como fasteners primitivos catalogados, no como subpiezas estructurales ambiguas.
- El import/setup de Unity restaura `HMX5V-GUAN-DINGWEI` desde grupos de fasteners hacia su brazo canonico cuando detecta escenas contaminadas por importaciones previas.
- Los fasteners de `holybro_fastener_instances.json` usan `x500v2_Drone/x500v2_fastener_group` como ruta fisica raiz y conservan `parentCanonicalPartId` como relacion semantica.
- La resolucion de cuadrantes reconoce sufijos Blender `.001_low`, `.002_low`, `.003_low`, `.004_low` como BR, FR, FL y BL respectivamente.
- `SelectionManager`, `PartVisibilityManager` y `UIManager` resuelven marcadores de fastener en meshes hijos sin cruzar a ancestros compartidos.
- `ImportDroneModel` calibra el FBX final a tamano dominante objetivo `10u`, con mallas legibles para soportar colliders exactos de seleccion.
- `SetupImportedDroneThermalTest` e `ImportedDroneRuntimeBinder` reemplazan colliders tipo caja por `MeshCollider` en renderers con `MeshFilter`; la caja queda solo como fallback.
- `HolybroFastenerCatalogBuilder` asigna padres canonicos de fasteners por cercania a renderers categorizados, no solo por pivotes/anclas canonicas.
- `RuntimeDroneSceneAuditor` ahora advierte si el conteo de `FastenerRuntimeMarker` no coincide con el catalogo de instancias.

### Fixed

- Se eliminaron del catalogo de fasteners las familias falsas `HMX5V-GUAN-DINGWEI.*`; el baseline corregido vuelve a 19 familias y 170 instancias al incluir grommets y la tuerca del GPS como fasteners reales.
- Se evita que `x500v2_arm_BR` absorba todos los fasteners por rutas heredadas del import anterior.
- Se reduce el riesgo de que subpiezas de brazos queden asignadas a cuadrantes aleatorios por depender solo de posicion mundial.
- Se reduce el riesgo de clicks cruzados sobre brazos/fasteners causado por `BoxCollider` sobredimensionados en meshes instanciados del FBX.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -p:BuildProjectReferences=false` sin errores.
- La compilacion editor por `dotnet` queda bloqueada por errores existentes de `UnityEditor.UI` en `Library/PackageCache`.

## [Unreleased] - 2026-05-10

### Changed

- `OrbitCameraController` calibra su distancia inicial, reset y ventana de navegacion desde los bounds reales del root activo `x500v2_Drone`.
- `OrbitCameraController` reemplaza referencias serializadas obsoletas al modelo preservado por el root runtime activo cuando arranca.
- `ImportDroneModel` actualiza la referencia serializada del `OrbitCameraController` al nuevo root importado.
- `ViewModeManager` calibra siempre los materiales Blueprint/Solid aunque ya existan materiales serializados en escena.
- `ViewModeManager` desactiva el edge detection de pantalla en Blueprint para evitar manchas blancas por normales/profundidad en mallas densas.
- `Blueprint.shader` y `SolidColor.shader` reducen el ancho de outline para la escala final del dron.

### Fixed

- El modo Blueprint deja de depender de un doble sistema de lineas que engrosaba la silueta del dron.
- `ResetView` ya no queda atado a una distancia fija pensada para un asset de escala distinta.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -p:BuildProjectReferences=false -m:1` sin errores.
- La compilacion editor por `dotnet` sigue bloqueada por `UnityEditor.UI` en `Library/PackageCache`; es un bloqueo externo al codigo modificado.

## [Unreleased] - 2026-05-10 B

### Changed

- `Blueprint.shader` reduce aun mas el ancho de silueta y restringe el fresnel tecnico para evitar lineas gruesas en el dron escalado a `10u`.
- `SolidColor.shader` reduce el ancho de outline para conservar lectura tecnica sin cubrir superficies pequenas.
- `SelectionManager` aplica highlight suave a fasteners asociados cuando se selecciona una pieza madre canonica.
- `UIManager` trata fasteners como subcapa de su `ParentCanonicalPartId`, no como hijos logicos de `x500v2_fastener_group`.
- `PartVisibilityManager` diferencia aislamiento de pieza madre, subpieza y fastener individual.

### Fixed

- Aislar un fastener individual ya no debe mostrar los demas fasteners de la misma pieza madre.
- Aislar una subpieza ya no arrastra todos los fasteners asociados al padre canonico.
- La seleccion de pieza madre ahora comunica visualmente sus fasteners asociados mediante tint suave.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -p:BuildProjectReferences=false -m:1` sin errores.

## [Unreleased] - 2026-05-10 C

### Changed

- `PartVisibilityManager` agrega modos explicitos de aislamiento: default de pieza madre, subpieza con fasteners asociados y scope estricto.
- `UIManager` permite avanzar de `pieza madre -> subpieza con fasteners asociados -> subpieza sola`.
- `UIManager` conserva una capa de retorno para restaurar `subpieza + fasteners asociados` antes de volver a la pieza madre o hotspot.
- El doble click directo sobre fastener exige contexto de pieza madre, salvo hotspots que no contienen piezas madre completas.

### Fixed

- Aislar una subpieza ya no arrastra todos los fasteners de su pieza madre; solo conserva los asociados por contacto/proximidad geometrica.
- Repetir aislamiento sobre la misma subpieza permite verla sin fasteners asociados.
- Retroceder desde subpieza sola o fastener seleccionado ya no salta directamente a pieza madre.
- La salida desde subpieza aislada vuelve al hotspot cuando esa fue la capa anterior, en lugar de inventar una pieza madre intermedia.
- Los fasteners standalone recuperan visibilidad al limpiar aislamiento.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -m:1` sin errores.

## [Unreleased] - 2026-05-10 D

### Changed

- `SelectionManager` expone el contexto de seleccion actual y permite seleccionar fasteners compartidos dentro de la pieza/subpieza activa cuando hay asociacion geometrica.
- `UIManager` resuelve fasteners con contexto preferido antes de usar su `ParentCanonicalPartId` prioritario.
- `PartVisibilityManager` incluye fasteners compartidos por contacto/proximidad al aislar una pieza madre completa.
- `SetupImportedDroneThermalTest` suprime proxies runtime heredados `x500v2_esc_*` si no existe geometria real importada.

### Fixed

- Un fastener compartido ya no obliga a volver a su pieza madre prioritaria si el usuario entro desde otra pieza valida.
- Los fasteners de una subpieza tambien se consideran parte de la pieza madre de esa subpieza en seleccion, highlight y aislamiento.
- Los placeholders `ESC BLHeli-S 20A` dejan de recrearse como proxies artificiales del runtime final.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -m:1` sin errores.
- La validacion editor por `dotnet` sigue bloqueada por dependencias externas de Unity (`UnityEditor.UI` / metadatos temporales), no por los assemblies runtime verificados.

## [Unreleased] - 2026-05-10 E

### Changed

- `SetupImportedDroneThermalTest` suprime proxies runtime `x500v2_pdb`, `x500v2_rc_receiver` y `x500v2_esc_*` cuando no existen como geometria real del FBX final.
- `RuntimeDroneSceneAuditor` separa ids canonicos documentados de ids obligatorios del FBX actual para no reportar como error piezas catalogadas pero ausentes.
- `holybro_selection_hierarchy.json` excluye `x500v2_pdb` de `Power Distribution` y `x500v2_esc_*` de `Propulsion System` en la configuracion activa.
- Se agrega `holybro_runtime_selection_mapping.md` como tabla auditable de piezas madre, hotspots, subpiezas y fasteners prioritarios.

### Fixed

- `pdb_runtime_proxy` y `rc_receiver_runtime_proxy` dejan de aparecer como piezas sinteticas sobrantes al reimportar.
- El audit deja de exigir anchors ESC si el FBX final no contiene geometria real para ESC.
- Los hotspots ya no apuntan a miembros canonicos ausentes por proxies heredados.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -m:1` sin errores.
- Una corrida paralela inicial produjo un lock temporal de `obj\Debug\Core.dll`; se repitio `Core.Player` de forma aislada y compilo correctamente.

## [Unreleased] - 2026-05-10 F

### Added

- `holybro_subpiece_fastener_map.json` define la propuesta explicita de fasteners prioritarios y compartidos por subpieza.
- `holybro_selection_ux_audit.md` documenta hallazgos UX sobre piezas madre, hotspots, piezas ausentes y mejoras de navegacion.

### Changed

- La documentacion tecnica ahora distingue entre relaciones de fastener con confianza alta/media/baja.
- Se deja establecido que la asignacion fina por subpieza debe serializarse y no depender solo de heuristicas de contacto en runtime.

### Notes

- El mapa asigna 170 fasteners como prioritarios y 192 relaciones compartidas/contextuales.
- Las filas `low` requieren confirmacion visual contra Blender/Unity antes de convertirse en fuente final de runtime.

## [Unreleased] - 2026-05-11 A

### Changed

- `SelectionHierarchy` ya no clasifica `HUAN-GUIJIAO`, `GPSV5-ZHIJIA-LUOMAO` ni `luomao` como fasteners primitivos.
- `HolybroFastenerCatalogBuilder` deja de exportar `x500v2_blend_huan_guijiao` y `x500v2_blend_gpsv5_zhijia_luomao` como familias/instancias modulares.
- `SetupImportedDroneThermalTest` e `ImportedDroneRuntimeBinder` restauran falsos fasteners estructurales con parent/subpiece reales en lugar de forzarlos al grupo de fasteners.
- `holybro_selection_hierarchy.json` agrega `HUAN-GUIJIAO` como subpieza de brazos, `GPSV5-ZHIJIA-LUOMAO` como subpieza GPS y `JIA-LIANJIE` como subpieza de landing gear.
- La documentacion Holybro actualiza la frontera Blender/Unity: piezas runtime en `BAKE_MASTERS_LOW` / `ASSEMBLY_INSTANCES_LOW`, fasteners modulares solo en `PRIMITIVE_FASTENER_MASTERS` / `PRIMITIVE_FASTENER_INSTANCES`.
- Los recursos generados `holybro_fastener_families.json`, `holybro_fastener_instances.json` y `holybro_fastener_reconciliation.json` quedan sincronizados con `18` familias y `161` instancias primitivas.

### Fixed

- `HUAN-GUIJIAO` ya no se comparte como `rubber_grommet` ni activa reemplazo modular.
- `GPSV5-ZHIJIA-LUOMAO` ya no se trata como `lock_nut_M3` modular.
- El mapa documental `holybro_subpiece_fastener_map.json` elimina `rubber_grommet_*` y `x500v2_fastener_lock_nut_M3_001` de las relaciones de fasteners primitivos.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\UI.Player.csproj -nologo -p:BuildProjectReferences=false` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.
- La validacion editor por `dotnet` sigue bloqueada por `UnityEditor.UI` del paquete `com.unity.ugui`, no por los archivos modificados.

## [Unreleased] - 2026-05-11 B

### Added

- `desarrollo/docs/sistema_termico/AUDIT_TERMICO_FBX_FINAL_2026-05-11.md` documenta hallazgos, correcciones y checklist visual del sistema termico sobre el FBX final.

### Changed

- `ThermalSimulationManager` crea puentes termicos conservadores cuando el grafo canonico contiene nodos ausentes en la geometria runtime actual.
- `ThermalSimulationManager` agrega enlaces suplementarios entre power module, Pixhawk, platform board, plates, rails y landing gear para conservar rutas de conduccion sin recrear proxies eliminados.
- `ThermalViewController` cachea bounds completos por pieza canonica para ubicar focos termicos con mayor estabilidad en el FBX granular.
- `ThermalViewController` aplica temperatura visual ajustada por subpieza sin cambiar la temperatura fisica del solver.

### Fixed

- Las plates y carriers estructurales ya no dependen de un patron uniforme que ocultaba zonas de contacto.
- Subpiezas de Pixhawk tipo tapa/carcasa, como `MIANKE-PIXHAWK6C-LV-C1`, dejan de verse igual de calientes que la electronica interna.
- La transferencia visual hacia frames y plates deja de romperse cuando una pieza intermedia documentada no existe como mesh runtime.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode con power activo, carga `60%`-`100%` y modo `Thermal`.

## [Unreleased] - 2026-05-11 C

### Added

- `desarrollo/docs/investigacion/Holybro/holybro_explode_view_audit.md` documenta el funcionamiento actual de explode view y las correcciones necesarias para una vista estetica y pedagogica.

### Changed

- `ThermalViewController` agrega una zona muerta visual de equilibrio termico cerca de la temperatura ambiente.
- El patron espacial del shader termico ahora se activa progresivamente segun delta termico visual, evitando hotspots radiales en piezas frias.

### Fixed

- `top_plate` ya no deberia mostrar un foco radial desplazado cuando el dron esta apagado/frio o en equilibrio ambiente.
- Las piezas criticas ya no reciben una banda visual mas caliente que las pasivas cuando todas estan efectivamente en temperatura ambiente.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: estado frio uniforme, calentamiento con patron radial solo al existir transferencia termica.

## [Unreleased] - 2026-05-11 D

### Changed

- `ExplodedViewManager` usa `explosionPriority` para convertir el factor global de explode en factores locales escalonados por pieza.
- La animacion de explode aplica `SmoothStep` por capa para evitar movimientos lineales bruscos.
- `ImportedDroneRuntimeBinder` calibra presets runtime de direccion/distancia para anchors principales del FBX final.
- Motores y helices salen por cuadrante en explode view en lugar de desplazarse solo hacia arriba.

### Fixed

- La vista explosionada ya no mueve todas las piezas principales simultaneamente.
- La categoria `Fasteners` deja de alterar el rango de prioridades globales de la explosion.

### Documentation

- `holybro_explode_view_audit.md` registra el primer arreglo implementado y el siguiente pulido recomendado.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode para ajustar separacion lateral/altura de props, motores y stack central.

## [Unreleased] - 2026-05-11 E

### Changed

- `ExplodedViewManager` calcula un centro radial comun usando `x500v2_bottom_plate` como referencia principal.
- La direccion de explode de cada pieza principal se deriva de `centro pieza - centro dron`.
- `ExplodablePart` permite recibir targets runtime calculados en world space sin modificar assets.
- Los presets `explosionDirection` quedan como fallback para piezas centradas o sin bounds confiables.

### Fixed

- La base de la vista explosionada deja de depender de direcciones manuales inconexas.
- Las piezas principales ya no deberian moverse en direcciones que no salgan desde el centro del dron.

### Documentation

- `holybro_explode_view_audit.md` agrega el criterio actualizado de validacion radial.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: confirmar que todas las piezas salen radialmente desde el centro y ajustar delays despues.

## [Unreleased] - 2026-05-11 F

### Changed

- La resolucion espacial de cuadrantes usa `ZHIJIA-CAMERA-INTEL` como frente fisico del dron y `BOTTOM-PLATE-X500-V5` como centro.
- `CARBON-FIBER-TUBE300.001/.002` deja de usar sufijos de instancia Blender como propiedad directa de cuadrante.
- `ExplodedViewManager` agrega offsets radiales por renderer estructural dentro de brazos para corregir subpiezas compartidas que viven bajo un anchor temporal.

### Fixed

- `CARBON-FIBER-TUBE300.002_low` ya no deberia salir hacia la derecha solo por estar clasificado por sufijo `.002`.
- Las subpiezas estructurales de brazos pueden compensar el desplazamiento del anchor y moverse hacia su normal radial real desde el centro del dron.

### Documentation

- `holybro_explode_view_audit.md` documenta la referencia frontal basada en `ZHIJIA-CAMERA-INTEL` y la regla especial de tubos compartidos.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: reimportar/rebind y probar el movimiento de `CARBON-FIBER-TUBE300.002_low`.
- `Assembly-CSharp-Editor.csproj` no fue una verificacion limpia por errores externos de `UnityEditor.UI` en `PackageCache` y metadatos temporales faltantes al aislar referencias.

## [Unreleased] - 2026-05-11 G

### Changed

- La clasificacion de subpiezas de brazos deja de usar un patron generico `.001/.002/.003/.004` y pasa a un mapa explicito por familia Blender.
- `ImportedDroneRuntimeBinder` permite corregir objetos entre anchors `x500v2_arm_*` cuando el mapeo explicito contradice una asignacion previa.
- El offset auxiliar de subpiezas estructurales en explode usa una normal radial proyectada al plano horizontal.
- `ImportedDroneRuntimeBinder` recalibra `explosionPriority` runtime segun una secuencia fisica de desmontaje.

### Fixed

- `HMX5V-JIBI-JIA-MUJU.002_low` queda alineada con la misma direccion/categoria fisica que `.001_low`.
- `HMX5V-JIBI-JIA-MUJU.004_low` queda alineada con la misma direccion/categoria fisica que `.003_low`.
- Las subpiezas de brazo elevadas dejan de salir hacia arriba por tener componente vertical alta en su centro de bounds.

### Documentation

- `BITACORA.md` y `holybro_explode_view_audit.md` documentan el criterio de mapa explicito, explode planar y prioridad por dependencia.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: reimportar/rebind y verificar pares `HMX5V-JIBI-JIA-MUJU.001/.002` y `.003/.004`.

## [Unreleased] - 2026-05-11 H

### Changed

- `AuxiliaryExplodeOffset` soporta offsets con timing global, ventana de secuencia y correccion radial separada.
- `ExplodablePart` soporta movimiento compuesto: seguimiento de pieza madre mas separacion local de desmontaje.
- `ExplodedViewManager` aplica un retraso inicial a piezas no fastener para que la tornilleria se libere primero.
- Los anchors de categoria `Fasteners` completan su recorrido en una ventana inicial propia antes de que avance la mayor parte de piezas estructurales.
- Los fasteners como anchors independientes resuelven direccion de explode por eje probable de montaje, con tratamiento distinto para tornillos, tuercas y separadores.
- Motores y helices siguen el recorrido radial del brazo correspondiente y suman separacion vertical propia.
- Fasteners, motores y helices resuelven un parent de movimiento espacial por proximidad para evitar arrastres invertidos o congelados.
- `ImportedDroneRuntimeBinder` asigna perfiles manuales de desmontaje a fasteners y subpiezas mecanicas de brazos.

### Fixed

- Los fasteners ya no dependen exclusivamente de la direccion radial del centro del dron.
- Los fasteners independientes ya no quedan congelados al terminar su salida axial; ahora pueden acompanar el desplazamiento de su pieza principal.
- Las tuercas dejan de comportarse igual que tornillos: tienden a separarse hacia abajo/opuesto al eje de apriete.
- Las helices y motores dejan de tener un destino completamente independiente del brazo.
- Los motores dejan de depender solo del sufijo de instancia para elegir brazo de arrastre.
- `HMX5V-JIBI-JIA-MUJU.001` y `.005` bajan explicitamente.
- `JIA-GUAN` y rubber/grommets dejan de recibir correccion radial temprana que producia zigzag.
- `GUAN-CHENG` sale como parte de `x500v2_landing_gear`; el sistema de rails/battery mount mantiene su salida descendente propia.
- `HMX5V-ZUO-DJ-MUJU`, `HMX5V-DIGAI-DIANJIZUO-MUJU`, `HMX5V-JIBI-JIA-MUJU`, `BAN-DJ-DIAN-F2`, `JIA-GUAN` y `HMX5V-GUAN-DINGWEI` reciben offsets de desmontaje acordes a su rol fisico en el brazo.

### Documentation

- `BITACORA.md` y `holybro_explode_view_audit.md` registran el perfil fisico de desmontaje y sus criterios de validacion.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: revisar separacion final de fasteners, tuercas y abrazaderas para ajustar distancias finas.

## [Unreleased] - 2026-05-11 I

### Changed

- `ExplodedViewManager` aplica una pasada iterativa de clearance final entre bounds de piezas canonicas para reducir contactos en la pose explosionada.
- Se aumento la separacion secundaria de motores/helices y se bajo el offset local de `BAN-DJ-DIAN-F2` para evitar que queden pegados.
- Rubber/grommets que lleguen como fastener primitivo se alinean al vector del tubo/brazo, no al eje vertical de spacer.
- `GUAN-CHENG` pasa de `x500v2_rails_battery` a `x500v2_landing_gear` en jerarquia declarativa, fallback runtime y mapeo termico/categorias.

### Fixed

- `GUAN-CHENG` ya no se desplaza como rail/battery hacia una familia equivocada; ahora sigue el desmontaje descendente del tren de aterrizaje.
- Rubber/grommets dejan de moverse en direcciones inversas o por eje contrario al tubo.
- `BAN-DJ-DIAN-F2` y motores tienen mayor distancia final para evitar contacto visual al final del explode.

### Documentation

- `BITACORA.md` y `holybro_explode_view_audit.md` registran la correccion de clearance, grommets y `GUAN-CHENG`.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo` sin errores.

### Verification Pending

- Validacion visual en Unity Play Mode: confirmar que la pose final no deje `BAN-DJ-DIAN-F2` pegada al motor y que `GUAN-CHENG`/rubber grommets sigan el recorrido esperado.

## [Unreleased] - 2026-05-12 A

### Changed

- `SelectionHierarchy` incorpora companeros de ensamblaje para que `x500v2_arm_*` pueda incluir visualmente `x500v2_motor_*` y `x500v2_prop_*` sin perder la identidad independiente de motor/helice.
- `holybro_selection_hierarchy.json` elimina `CARBON-FIBER-TUBE300`, `JIA-GUAN` y `HUAN-GUIJIAO` del grupo `x500v2_arm`, segun la jerarquia STEP revisada en Blender.
- `holybro_parent_subpieces.json` y los assets `x500v2_arm_*` reflejan brazos HMX5V con motor/helice como componentes asociados.
- La resolucion de cuadrantes por instancia HMX5V corrige el intercambio observado entre `front-left` y `back-right`.

### Fixed

- Seleccionar/aislar un brazo ya no depende de incluir tubos largos, grommets o `JIA-GUAN` como subpiezas del HMX5V.
- El highlight y aislamiento de pieza madre puede incluir motor/helice asociados al brazo, junto con fasteners relacionados.
- `HUAN-GUIJIAO` y rubber grommets dejan de ser forzados por el reparador runtime hacia `x500v2_arm_*`.

### Documentation

- `BITACORA.md`, `holybro_selection_hierarchy.json`, `holybro_parent_subpieces.json` y `holybro_runtime_selection_mapping.md` documentan la revision STEP/MCP y el criterio aplicado.

### Verified

- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1` sin errores.

### Verification Pending

- Reimportar en Unity y validar visualmente cada cuadrante: par `HMX5V-JIBI-JIA-MUJU`, `HMX5V-GUAN-DINGWEI`, `BAN-DJ-DIAN-F2`, motor, helice y fasteners.
## [Unreleased] - 2026-05-12 B

### Changed
- Alineada la jerarquia de seleccion con los empties STEP importados en Blender.
- `BM06B-WO` pasa de Pixhawk a `x500v2_power_module` por pertenecer a `PCBA-PM06_ASM`.
- `JIA-GUAN`, `HUAN-GUIJIAO` y `CARBON-FIBER-TUBE300` pasan a `x500v2_rails_battery`; `GUAN-CHENG` queda en `x500v2_landing_gear`.

### Fixed
- Reducida la dependencia de heuristicas por nombre que mezclaban rails, tren de aterrizaje y brazos.
- Actualizados fallbacks runtime/editor para evitar que una reimportacion regenere agrupaciones antiguas.

### Verification
- Pendiente reimportacion y QA visual en Unity.

## [Unreleased] - 2026-05-12 C

### Changed
- `PLATFORM-PLAT-X500` y la LiPo se integran en `x500v2_rails_battery`; dejan de prepararse como piezas madre independientes.
- `x500v2_prop_*` deja de prepararse como pieza madre independiente y pasa a resolverse como subpieza de `x500v2_arm_*`.
- `x500v2_pixhawk6c` queda restringido a tapa/base/PCB Pixhawk; `IMU-PIXHAWK6C` pasa a `x500v2_misc_group`.
- El catalogo de fasteners aplica reglas manuales por tipo e indice para bottom plate, rails, brazos, motores, GPS, power module y fasteners pendientes.

### Fixed
- Evitada la regeneracion de madres antiguas (`platform_board`, `battery`, `prop_*`) durante reimportacion.
- `holybro_parent_subpieces.json` y `holybro_runtime_selection_mapping.md` se regeneran desde la jerarquia vigente y el catalogo de fasteners corregido.

### Verification
- Validacion JSON correcta para jerarquia, fasteners y parent/subpieces.
- Pendiente ejecutar `Tools -> Import Final Runtime Drone Model` y QA visual de capas.

## [Unreleased] - 2026-05-12 D

### Changed
- La importacion final instancia `x500v2_Drone` con rotacion inicial de 90 grados en Y global para alinear el frente fisico definido por `ZHIJIA-CAMERA-INTEL`.
- `ZHIJIA-CAMERA-INTEL` queda dentro de `x500v2_rails_battery` en jerarquia, fallbacks runtime/editor, assets generados y documentacion.
- `holybro_parent_subpieces.json`, `holybro_runtime_selection_mapping.md` y `X500V2Generated` se resincronizan desde la jerarquia y los fasteners corregidos.

### Fixed
- `x500v2_pixhawk6c` queda sin fasteners asociados.
- Bottom plate, rails/battery mount y landing gear dejan de arrastrar fasteners explicitamente removidos por correccion manual.
- Seleccion/aislamiento ya no asocia fasteners por cercania si el `parentCanonicalPartId` del fastener no pertenece al contexto canonico activo.

### Verification
- Validacion automatica de reglas de agrupacion critica: OK.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo -p:BuildProjectReferences=false` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1 -p:BuildProjectReferences=false` sin errores.

### Verification Pending
- Reimportar en Unity y validar visualmente orientacion frontal, capas de `x500v2_rails_battery`, `x500v2_bottom_plate`, `x500v2_landing_gear` y fasteners pendientes en `x500v2_fastener_group`.

## [Unreleased] - 2026-05-12 E

### Changed
- Los M3x38/flange nut conservan prioridad por brazo, pero top/bottom usan reglas compartidas explicitas para seleccion, highlight e isolate.
- `cap_screw_M25x10` se asigna por brazo: FL 001-002, FR 003-004, BR 005-006, BL 007-008.
- La normalizacion de propellers compensa el naming cruzado del FBX al importar; despues de importar la relacion contextual queda por mismo cuadrante canonico.

### Fixed
- `x500v2_top_plate` vuelve a incluir todos los `cap_screw_M3x38` durante seleccion/aislamiento.
- `x500v2_bottom_plate` vuelve a incluir todos los `cap_screw_M3x38` y todas las `flange_nut_M3` conectadas.
- Los brazos recuperan los dos `cap_screw_M25x10` faltantes y dejan de arrastrar el propeller del cuadrante logico incorrecto.

### Verification
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo -p:BuildProjectReferences=false` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1 -p:BuildProjectReferences=false` sin errores.

## [Unreleased] - 2026-05-12 F

### Changed
- `cap_screw_M3x6` queda corregido por motor: FL 001-004, FR 005-008, BR 009-012 y BL 013-016.
- `countersunk_M3x16_001-002` pasa a `x500v2_rails_battery` y se retira de prioridad bottom plate.
- `cap_screw_M3x8_001-012`, `cap_screw_M3x25_001-002` y `lock_nut_M3_001/008` pasan a `x500v2_landing_gear`.
- `lock_nut_M3_009-012` pasa a `x500v2_power_module`.

### Fixed
- Los brazos ya no intercambian helices entre front/back.
- `x500v2_bottom_plate` incluye como fasteners contextuales compartidos `cap_screw_M3x8_005-012`, `pan_head_M3x14_001-004` y `nylon_standoff_M3x5_001-004`, sin robar prioridad a landing gear/power module.

### Verification
- Validacion automatica de parents y reglas compartidas: OK.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Assembly-CSharp.csproj -nologo -p:BuildProjectReferences=false` sin errores.
- `dotnet build E:\WebGL_tesis\desarrollo\unity_project\Core.Player.csproj -nologo -m:1 -p:BuildProjectReferences=false` sin errores.

## [Unreleased] - 2026-05-12 G

### Fixed
- Se movio la compensacion de propellers cruzados al importador: `NormalizePropellers` intercambia FL<->BL y FR<->BR antes de renombrar.
- La seleccion de brazos queda directa tras reimportar: `arm_FL -> prop_FL`, `arm_BL -> prop_BL`, `arm_FR -> prop_FR`, `arm_BR -> prop_BR`.
- No se tocaron reglas de fasteners ni piezas madre.
