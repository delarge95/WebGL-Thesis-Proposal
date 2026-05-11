# Bitácora de Desarrollo — Registro Integral de Ejecución

> Documento oficial de seguimiento del proyecto "WebGL Drone Viewer".
> Registro detallado de decisiones técnicas, arquitectura y evolución del sistema.
> Alineado con el Cronograma Oficial (Fases 1-7: H2 2025 | Fase 8: Q1 2026).

---

## Registro de Cambios (Mayo 08, 2026) - Reparacion de escala y jerarquia canonica FBX final

### Objetivo

Corregir los problemas detectados durante QA del FBX final: dron importado demasiado pequeno para la camara, fasteners modulares sobredimensionados, seleccion por grupos incoherente, filtros incompletos, vista explosiva sin desplazamiento correcto, modos Solid/Blueprint saturados y simulacion termica sin propagacion aparente.

### Acciones Realizadas

1. **Calibracion objetiva de escala del FBX en Unity**:
   - _Implementacion_: `ImportDroneModel.cs` mide los bounds del prefab importado y ajusta `ModelImporter.globalScale` hasta un tamano dominante objetivo de `5` unidades Unity.
   - _Resultado_: La escala se corrige en el asset importado, no con `localScale` del root, para que camara, zoom, materiales de modo visual, isolate y explode trabajen con magnitudes coherentes.
2. **Separacion entre dataset canonico y dataset granular Blender**:
   - _Implementacion_: `SetupImportedDroneThermalTest.cs` vuelve a usar `x500v2_parts_data.json` como fuente de anclas interactivas cuando existe cobertura canonica; `x500v2_blender_synced_parts.json` queda como metadata/subpiezas y no como lista principal de seleccion.
   - _Resultado_: La app recupera la taxonomia de `28` piezas canonicas que esperan filtros, Analyze, thermal, hotspots y explode.
3. **Democion de subpiezas Blender como anchors seleccionables**:
   - _Implementacion_: Las entradas `x500v2_blend_*` ya no quedan con `ExplodablePart`, `MaterialController` ni `HighlightSystem` propios.
   - _Resultado_: Las subpiezas granularizadas pueden formar parte fisica de una pieza madre sin competir como piezas madre independientes.
4. **Saneamiento de reparenting runtime**:
   - _Implementacion_: `ImportedDroneRuntimeBinder.cs` deja de robar renderers que ya pertenecen a un anchor canonico estable y separa correctamente motor, ESC, helice, PDB, bateria, rieles, Pixhawk y GPS.
   - _Resultado_: Se evita que brazos, fasteners o elementos centrales absorban piezas de otras familias por coincidencias amplias de nombre.
5. **Fasteners modulares proporcionales al proxy real**:
   - _Implementacion_: `FastenerBuilder.cs` reemplaza minimos absolutos por pisos relativos al tamano del proxy.
   - _Resultado_: El detalle modular no debe aparecer mas grande que el dron cuando el FBX venga en centimetros o con proxies pequenos.
6. **Reduccion de ruido diagnostico**:
   - _Implementacion_: `PartVisibilityManager.cs` deja el diagnostico detallado de isolate detras de `enableIsolationDiagnostics`.
   - _Resultado_: La consola deja de saturarse durante QA normal de isolate/seleccion.
7. **Cobertura canonica frente a geometria faltante**:
   - _Implementacion_: `SetupImportedDroneThermalTest.cs` crea proxies canonicos temporales cuando el FBX final no trae geometria para una pieza canonica, manteniendo filtros, explode y thermal sin afirmar que la malla final ya exista.
   - _Resultado_: Las piezas faltantes en el FBX actual, como PDB, ESCs o receptor RC, pueden permanecer en la taxonomia runtime sin ser confundidas con subpiezas Blender.
8. **Auditoria automatica post-import**:
   - _Implementacion_: Se agrego `RuntimeDroneSceneAuditor.cs` y el importador lo ejecuta al final de `Tools > Import Final Runtime Drone Model`.
   - _Resultado_: Cada import escribe `Reports/runtime_drone_scene_audit.md` con anchors canonicos faltantes, proxies temporales, anchors `x500v2_blend_*` seleccionables, fasteners y tamano dominante.

### Estado Actual

- La compilacion `Core.Player.csproj` se verifico con `0` errores; permanecen advertencias existentes por APIs obsoletas y analizador AG001.
- La compilacion editor via `dotnet` sigue bloqueada por un error externo de `com.unity.ugui` (`DefaultControls.factory` readonly), por lo que la validacion final del importador y del auditor debe realizarse desde Unity Editor.
- Se debe reejecutar `Tools > Import Final Runtime Drone Model` para regenerar escena, assets de piezas, proxies temporales y catalogos de fasteners con la jerarquia corregida.

### Primer Paso Al Retomar La Sesion

1. Esperar a que Unity recompile scripts sin errores propios.
2. Ejecutar `Tools > Import Final Runtime Drone Model`.
3. Verificar que el cuadro muestre escala dominante aproximandose al objetivo, `Meshes importados > 0`, `Helices normalizadas: 4`, `Fasteners normalizados > 0` y `Setup thermal/runtime: OK`.
4. Probar seleccion de brazos, Pixhawk en filtros, explode por piezas madre, isolate por fastener, modos Solid/Blueprint y thermal con el dron encendido.

---

## Registro de Cambios (Mayo 08, 2026) - Correccion de importacion FBX runtime final

### Objetivo

Corregir la importacion del FBX runtime final en Unity, que estaba mostrando `File is corrupted`, `MeshFilters totales: 0`, `matches=0/55`, `Helices normalizadas: 0` y `Fasteners normalizados: 0` pese a reportar falsamente `Setup thermal/runtime: OK`.

### Acciones Realizadas

1. **FBX activo sincronizado con fuente Blender**:
   - _Implementacion_: `Assets/Models/x500v2_runtime_low_final.fbx` fue reemplazado por la fuente real `blender_files/welded/x500v2_runtime_low_final.fbx`; ambos quedaron con el mismo SHA-256.
   - _Resultado_: El asset activo contiene nombres Blender completos como `BOTTOM-PLATE-X500-V5`, `DJ-2216-KV880`, `GB70-M25-10`, `M3-10-PAN-DING` y `HMX5V-ZUO-DJ-MUJU`.
2. **Importador Unity blindado contra falsos positivos**:
   - _Implementacion_: `ImportDroneModel.cs` ahora valida cabecera FBX, evita punteros LFS, sincroniza la fuente externa cuando el asset difiere, guarda respaldos fuera del repositorio, elimina el uso obsoleto de `ModelImporterMaterialLocation.External` y aborta si Unity no produce mallas.
   - _Resultado_: El menu de importacion ya no debe reportar `OK` cuando el FBX esta corrupto o importado sin `MeshFilter/SkinnedMeshRenderer`.
3. **Helices placeholder para el FBX sin blades dedicadas**:
   - _Implementacion_: Si el FBX no trae objetos `propeller`, el importador crea proxies simples `x500v2_prop_*` a partir de los motores `DJ-2216-KV880`.
   - _Resultado_: `DroneStateController` puede seguir animando helices aun cuando el FBX final solo traiga motores o monturas.
4. **Matching Blender/Unity robustecido**:
   - _Implementacion_: `SetupImportedDroneThermalTest.cs` ahora matchea ids y `blenderName` con normalizacion tolerante a mayusculas, guiones, underscores, puntos y sufijos `_low/_PRIM`.
   - _Resultado_: La prueba externa de nombres contra `x500v2_blender_synced_parts.json` cubre `55/55` piezas.
5. **Contencion de Git/LFS**:
   - _Implementacion_: Se detuvo nuevamente solo el lote confirmado de procesos `git add`/`git-lfs filter-process` reactivados por assets pesados; se completo `.gitignore` para ignorar `.blend` y variantes numericas dentro de `blender_files` y `desarrollo/blender_assets`.
   - _Resultado_: No quedo `index.lock`; no se hizo commit, push ni staging deliberado de LFS.
6. **Validacion real del import final en Unity**:
   - _Implementacion_: Tras reejecutar `Tools > Import Final Runtime Drone Model`, Unity reporto importacion efectiva del FBX final con mallas y normalizacion runtime.
   - _Resultado_: El importador confirmo `Meshes importados: 252`, `Helices normalizadas: 4`, `Fasteners normalizados: 170` y `Setup thermal/runtime: OK`; el caso anterior de `matches=0`, `MeshFilters=0` y FBX corrupto queda superado para esta exportacion.
   - _Hallazgo_: El reporte midio `Mesh assets unicos: 252` y `sharedMesh repetido: 0`; por tanto, el FBX actual conserva objetos runtime pero no llego a Unity como instancing de malla compartida.
7. **Cuarentena local anti auto-stage LFS**:
   - _Implementacion_: Se agregaron exclusiones locales en `.git/info/exclude` y se marcaron temporalmente archivos rastreados por LFS como `assume-unchanged` para impedir que el auto-stage de Codex/GUI relance `git-lfs filter-process` durante la validacion de Unity.
   - _Resultado_: La PC deja de quedar bloqueada por lote masivo de LFS; la medida es local y reversible, no cambia archivos del proyecto ni reemplaza un plan formal de versionado de assets.

### Estado Actual

- Unity ya importo el FBX runtime final con `252` meshes, `4` helices normalizadas y `170` fasteners normalizados.
- El siguiente foco deja de ser "FBX corrupto" y pasa a ser QA visual/funcional: escala en centimetros, jerarquia madre-subpiezas-fasteners, filtros, hotspots, seleccion, isolate, Analyze, Studio y animacion de helices.
- La optimizacion de instancing queda como hallazgo medido: el FBX actual no comparte `sharedMesh` entre objetos importados, aunque la funcionalidad runtime queda disponible para validacion.
- La contencion Git/LFS queda como medida local de sesion; antes de versionar assets pesados se debe retirar o revertir la cuarentena de `assume-unchanged` y preparar un commit LFS controlado.

### Primer Paso Al Retomar La Sesion

1. En Unity, guardar la escena final importada solo si el viewport confirma escala, jerarquia y materiales placeholder esperados.
2. Probar en Play Mode seleccion, hover, isolate por fastener, isolate por pieza madre con fasteners asociados, filtros Analyze, hotspots y giro de helices.
3. Decidir si el instancing final se corrige en Blender/export FBX o mediante manifiesto/postproceso Unity que reasigne `sharedMesh` a instancias equivalentes.
4. Mantener desactivado cualquier intento de commit/push LFS hasta definir que assets finales se versionan y con que estrategia de peso.

---

## Registro de Cambios (Mayo 08, 2026) - Guion de sustentacion y contencion Git/LFS

### Objetivo

Actualizar el articulo `INF_EST_51` de la variante de estudio Obsidian para que el guion de sustentacion refleje la app real, el flujo final de demo y la narrativa humana del proyecto; adicionalmente, diagnosticar sin riesgo la saturacion de procesos Git/LFS observada durante la importacion Unity.

### Acciones Realizadas

1. **Guion de sustentacion reestructurado**:
   - _Implementacion_: `Cerebro_Digital/Wiki/Concepts/INF_EST_51_Guion_Completo_Sustentacion_30min.md` fue reemplazado por una version 5.0 con mapa de 30 minutos, demo ordenada, placeholders medibles y checklist de ensayo.
   - _Resultado_: El guion ahora conecta con el publico desde la idea de "pereza productiva" como motor de automatizacion y la enlaza con decisiones de ingenieria reales.
2. **Funcionamiento de app incorporado al relato**:
   - _Implementacion_: La seccion de demo ahora sigue el flujo `inicio -> navegacion -> seleccion -> metadata -> aislamiento -> fasteners modulares -> encendido -> Analyze/Studio`.
   - _Resultado_: Se incluyen encendido del dron, estados `OFF/STARTING/IDLE/LOAD/FLYING`, giro de helices, thermal educativo, corte, explode, hover, seleccion, isolate y detalle bajo demanda de fasteners.
3. **Animaciones de sustentacion definidas**:
   - _Implementacion_: Se agrego una guia de animaciones `A01-A12` para comunicar pipeline, jerarquia, fasteners, encendido, thermal, corte y resultados sin decoracion innecesaria.
   - _Resultado_: La presentacion queda preparada para usar animaciones como herramienta de comprension, no como ornamento.
4. **Diagnostico seguro de procesos Git/LFS**:
   - _Implementacion_: Se inspeccionaron procesos `git.exe` y `sh.exe` antes de actuar; se confirmo que el cuello de botella provenia de multiples `git add -- ...` iniciados sobre archivos pesados Blender/FBX/texturas, cada uno abriendo `git-lfs filter-process`.
   - _Resultado_: Se detuvo un primer lote confirmado de procesos atascados de `git add`/`git-lfs filter-process` sin tocar Unity, Blender ni archivos de trabajo. No se realizo push, commit ni staging deliberado de LFS.

### Estado Actual

- El guion de sustentacion queda actualizado como documento prospectivo final con placeholders para metricas no congeladas.
- Los archivos LFS/pesados no deben subirse ni prepararse para commit en esta sesion; la prioridad es mantener la PC estable mientras Unity termina importacion/QA.
- Quedan procesos Git/LFS rezagados; cualquier cierre adicional debe hacerse solo tras nueva confirmacion explicita y apuntando exclusivamente a `git add`/`git-lfs filter-process`.

### Primer Paso Al Retomar La Sesion

1. Si la PC sigue lenta, revisar procesos Git restantes y detener solo los comandos confirmados como `git add` o `git-lfs filter-process`, nunca Unity/Blender.
2. Revisar las 3 fallas de consola Unity cuando el usuario comparta errores y advertencias clave.
3. No ejecutar `git add`, commit, push ni operaciones LFS hasta que se defina un plan separado para archivos pesados.

---

## Registro de Cambios (Mayo 08, 2026) - Coherencia documental post-fasteners y pipeline Blender/Unity

### Objetivo

Alinear informe final, manuales, paquete de sustentacion, guia de demo y notas de Obsidian con el estado real de la app: fasteners modulares implementados en Unity, placeholders visuales temporales, FBX Blender final preparado para QA y resultados empiricos pendientes de freeze.

### Acciones Realizadas

1. **Paquete de sustentacion saneado**:
   - _Implementacion_: `PRESENTATION_OUTLINE.md`, `KIMI_PROMPT.md`, `ASSETS_REQUIREMENTS.md` y `DEMO_SCRIPT.md` fueron reescritos para evitar claims de `7` modos publicos, medicion, BOM, conexiones, catalogo legacy o metricas no medidas.
   - _Resultado_: La presentacion queda alineada con la UI real: `Realistic` base, `X-Ray`, `Solid`, `Thermal`, entornos, Inspect, Analyze, Studio y fasteners.
2. **Manuales y arquitectura sincronizados**:
   - _Implementacion_: `docs/manuals/README.md`, `docs/manuals/ARCHITECTURE.md`, `docs/manuals/DEMO_SCRIPT.md`, manual tecnico y manual de usuario se alinearon con el flujo visible y los limites actuales.
   - _Resultado_: La documentacion de usuario y tecnica deja de vender herramientas legacy como producto visible.
3. **Informe final actualizado**:
   - _Implementacion_: Capitulos 4, 5, 6 y 8 se ajustaron para documentar el FBX final como preparado para importacion y QA Unity, no como cierre empirico ya medido.
   - _Resultado_: El capitulo de resultados exige manifest Blender, reporte de importacion Unity, QA de jerarquia/grupos/fasteners/helices y mediciones post-freeze antes de llenar valores.
4. **Obsidian y guia de estudio sincronizados**:
   - _Implementacion_: `VARIANTE_ESTUDIO_OBSIDIAN.md`, `INF_EST_04`, `INF_EST_30`, `INF_EST_33` y `INF_EST_50` incorporan masters+instancias, pipeline final Blender, mask `R=AO/G=Roughness/B=Curvature/A=Metallic`, fasteners temporales y regla de no inventar resultados.
   - _Resultado_: La abstraccion de estudio, guion y sustentacion ahora cuentan la misma historia que la app real.

### Estado Actual

- La fuente primaria de narrativa queda en informe final, manuales, paquete de presentacion, guia de demo actualizada y notas Obsidian sincronizadas.
- Las auditorias historicas y consolidaciones antiguas pueden conservar lenguaje legacy, pero no deben usarse como fuente de demo o defensa sin la nota de estado correspondiente.
- Queda pendiente compilar/regenerar PDFs cuando la PC este menos cargada y Unity/Blender no esten ocupando recursos.

## Registro de Cambios (Mayo 08, 2026) - Bake final Blender, export manual y preparacion Unity

### Objetivo

Formalizar el flujo final de materiales y exportacion del dron optimizado desde Blender 5.0.1 hacia Unity, preservando masters e instancias como piezas runtime reales y dejando una ruta segura para reemplazar los fasteners temporales por el sistema modular definitivo.

### Acciones Realizadas

1. **Flujo final Blender -> Unity documentado**:
   - _Implementacion_: Se agrego `Blender_Final_Bake_Export_Unity_Workflow.md` como guia operativa para bake de `BaseColor`, `Roughness`, `Metallic`, normal final, AO final, mask packing y export manual FBX.
   - _Resultado_: El criterio runtime queda fijado como union de `BAKE_MASTERS_LOW`, `ASSEMBLY_INSTANCES_LOW`, `PRIMITIVE_FASTENER_MASTERS` y `PRIMITIVE_FASTENER_INSTANCES`; no se debe exportar solo instancias.
2. **Tooling Blender no destructivo**:
   - _Implementacion_: Se agrego `blender_bake_target_setup.py` para preparar imagenes destino y nodos activos de bake sin guardar, exportar ni modificar outputs finales.
   - _Implementacion_: Se agrego `blender_pack_x500_mask.py` para empaquetar `R=AO`, `G=Roughness`, `B=Curvature`, `A=Metallic` y generar un derivado URP `MetallicSmoothness`.
   - _Implementacion_: Se agrego `blender_runtime_manifest_exporter.py` para reportar masters, instancias, transforms, bounds, candidatos de pieza madre y reconciliacion del conteo de fasteners Blender vs baseline Unity.
   - _Resultado_: Unity podra recibir un manifest verificable sin depender de suposiciones sobre jerarquia o parentesco de fasteners.
3. **Regla de revision para fasteners inciertos**:
   - _Implementacion_: El manifest reporta candidatos cercanos por distancia a bounds/centro, pero deja `review_required` y no escribe `parentCanonicalPartId` automaticamente.
   - _Resultado_: Si un fastener no puede asignarse con confianza, queda temporalmente en `x500v2_fastener_group` y se revisa con evidencia antes de modificar la configuracion.
4. **MCP Blender preparado en Codex**:
   - _Implementacion_: Se agrego la configuracion prevista del servidor `blender-mcp` en `C:\Users\alexw\.codex\config.toml`, apuntando al addon activo en `127.0.0.1:9876`.
   - _Resultado_: La conexion queda pendiente de reiniciar/verificar en Codex; no se interactuo con Blender durante el bloqueo de guardado para evitar riesgo de crasheo.
5. **Preparacion Unity para FBX final**:
   - _Implementacion_: Se copio `x500v2_runtime_low_final.fbx` a `Assets/Models/` con configuracion de import que preserva jerarquia, desactiva animacion/camaras/luces y usa texturas externas.
   - _Implementacion_: `ImportDroneModel.cs` fue reemplazado por un importador final que conserva el modelo anterior como referencia inactiva, instancia el FBX final como `x500v2_Drone`, normaliza helices y agrupa fasteners Blender bajo `x500v2_fastener_group`.
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora expone modo `Headless` para automatizacion sin dialogos, y la deteccion de fasteners reconoce nombres Blender como `GB70`, `PAN`, `CHEN`, `ZSLM`, `LM`, `NILONGZHU` y `HUAN-GUIJIAO`.
   - _Implementacion_: `DroneStateController` calcula el eje de giro de cada helice por bounds locales para no depender de un unico `Vector3.forward` cuando cambia la orientacion del FBX.
   - _Resultado_: La ruta de import Unity queda preparada para ejecutar el swap final y reconstruir seleccion, filtros, isolate, hotspots y fasteners sobre la escena importada.

### Estado Actual

- El flujo de bake/export final queda documentado y listo para ejecutarse manualmente cuando Blender este estable.
- Los scripts son auxiliares y no destructivos: no exportan FBX, no guardan el `.blend` y no asignan fasteners ambiguos.
- La importacion Unity posterior debe usar el FBX runtime final, texturas externas optimizadas y el manifest revisado para actualizar grupos, subgrupos, hotspots y fasteners.
- El FBX final ya esta versionado en `Assets/Models/x500v2_runtime_low_final.fbx`; la ejecucion del menu Unity queda pendiente porque esta sesion no expone un ejecutable `Unity.exe` para batchmode.

### Primer Paso Al Retomar La Sesion

1. Esperar a que Blender termine de guardar y trabajar siempre sobre una copia tipo `ready-to-bake_006_final-material-bake.blend`.
2. Ejecutar los bakes siguiendo la guia nueva y generar `X500_BaseColor_4K.png`, `X500_Normal_Final_4K.png`, `X500_Mask_4K.png`, `Hex_AO.png` y `Hex_Normal_Atlas.png`.
3. En Unity, ejecutar `Tools > Import Final Runtime Drone Model` y revisar `Reports/final_runtime_import_report.md` para confirmar conteo de shared meshes, helices y fasteners.

## Registro de Cambios (Abril 21, 2026) - Onboarding MVP procedural y alineacion del discurso de producto

### Objetivo

Cerrar un MVP real del onboarding dentro de la app, sustituyendo la idea de media por tarjeta por un sistema procedural ligero y coherente con los controles reales de desktop y mobile.

### Acciones Realizadas

1. **Motor visual procedural para onboarding**:
   - _Implementacion_: Se agrego `OnboardingAnimationView.cs` como canvas `Painter2D` dedicado a previews animados por tarjeta.
   - _Implementacion_: El motor cubre `15` cards del onboarding con variantes `PC` y `Mobile`.
   - _Resultado_: El onboarding deja de depender de media pesada o mocks estaticos y pasa a demostraciones runtime ligeras y repetibles.
2. **Integracion real con la UI existente**:
   - _Implementacion_: `OnboardingController.cs`, `MainLayout.uxml`, `Theme.uss` y `ProceduralResetIcon.cs` fueron ajustados para conectar el nuevo stage visual, mantener el layout constante y sincronizar labels/overlays.
   - _Resultado_: El preview queda integrado al flujo real del overlay en lugar de vivir como recurso externo.
3. **Lenguaje gestual unificado por plataforma**:
   - _Implementacion_: Desktop usa cursor procedural, anillos de click, rueda y botones de mouse; mobile usa ripple, drag y pinch abstractos.
   - _Resultado_: El onboarding explica gestos y microinteracciones sin GIFs, videos ni spritesheets.
4. **Pulido fuerte de coherencia visual**:
   - _Implementacion_: Se iteraron timings, continuidad del puntero, orden `move -> click/tap -> response`, sincronizacion de sliders, doble click, trayectorias y holds finales.
   - _Implementacion_: Se rehicieron cards delicadas como `Navigate`, `Part Info`, `Inspect`, `Analyze`, `Cut`, `Explode`, `Filter`, `Studio` y `Environment`.
   - _Resultado_: El sistema queda en estado MVP presentable y mucho mas cercano a una lectura profesional card por card.
5. **Alineacion con el discurso actual del producto**:
   - _Implementacion_: Se consolida el onboarding como parte visible del flujo real y se deja mejor posicionado el subsistema de fasteners en la documentacion y el portafolio.
   - _Resultado_: Queda una narrativa mas coherente entre producto visible, tooling, fasteners y UX guiada.

### Estado Actual

- Existe un onboarding MVP implementado dentro de Unity usando `Painter2D`, sin depender de video, GIF o media pesada.
- El sistema ya ensena navegacion, seleccion, panels, menus, sliders y modos visuales con loops animados por tarjeta.
- La validacion final pendiente ya no es de arquitectura, sino de QA visual fino dentro del editor y en WebGL.

### Primer Paso Al Retomar La Sesion

1. Revisar visualmente el onboarding dentro de Unity/WebGL en desktop y mobile.
2. Capturar clips y screenshots definitivos para portafolio y evidencias.
3. Hacer una ronda final de fixes de percepcion si alguna card sigue viendose acelerada, cargada o ambigua.

## Registro de Cambios (Abril 13, 2026) - Fasteners Unity: metadata modular, inspeccion bajo demanda y catalogos reconciliados

### Objetivo

Implementar la parte Unity del plan de optimizacion de fasteners sin depender aun de la geometria final de Blender, dejando un contrato de datos estable, inspeccion detallada bajo demanda y documentacion coherente con el estado real de la app.

### Acciones Realizadas

1. **Capa de datos modular para fasteners**:
   - _Implementacion_: Se agregaron `FastenerFamilyDefinition`, `FastenerInstanceDefinition`, `FastenerModularRecipe`, `FastenerMetadata` y wrappers JSON en `FastenerDataModels.cs`.
   - _Implementacion_: `DronePartData` fue extendido con `fastenerMetadata` para evitar seguir sobrecargando descripcion, tools y dimensions con texto generico.
   - _Resultado_: La app ya tiene un contrato runtime explicito para familias e instancias de tornilleria.
2. **Catalogos Holybro generados y versionados**:
   - _Implementacion_: Se creo `HolybroFastenerCatalogBuilder` en editor y se materializaron `holybro_fastener_families.json`, `holybro_fastener_instances.json` y `holybro_fastener_reconciliation.json` tanto en `desarrollo/docs/investigacion/Holybro/` como en `Assets/Resources/`.
   - _Resultado_: Quedaron registrados `20` familias, `168` instancias y `9` entradas de reconciliacion a partir de `MainScene_Final` + `x500v2_blender_synced_parts.json`.
3. **Integracion runtime y setup**:
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora genera metadata real por fastener, sella `FastenerRuntimeMarker` y exporta catalogos durante la preparacion de escena.
   - _Implementacion_: `ImportedDroneRuntimeBinder` ahora asegura `FastenerRegistry`, `FastenerInspectionManager` y sella `fastenerFamilyId` / `fastenerInstanceId` sobre objetos detectados como fasteners.
   - _Resultado_: La seleccion de fasteners deja de depender de placeholders narrativos y pasa a consumir IDs y recetas estables.
4. **Inspeccion detallada bajo demanda**:
   - _Implementacion_: Se agregaron `FastenerRegistry`, `FastenerInspectionManager`, `FastenerBuilder` y `FastenerRuntimeMarker`.
   - _Implementacion_: El runtime conserva proxies ligeros en reposo y solo la instancia seleccionada oculta su proxy para mostrar un detalle procedural temporal (`socket cap`, `pan`, `countersunk`, `flange/cap/nyloc nut`, `standoff`, `grommet`, `tube stopper`).
   - _Resultado_: Se valida el flujo de optimizacion sin bloquear el reemplazo futuro por mallas Blender finales.
5. **UI y semantica de detalle**:
   - _Implementacion_: `UIDetailsSheet` ahora muestra categoria/familia tecnica, metrica, longitud, drive, material, CAD source y parent canonical cuando la seleccion pertenece a `Fasteners`.
   - _Implementacion_: La ficha ahora tambien agrega `subComponentNames` para piezas madre, permitiendo listar subpiezas documentadas y resumen de fasteners por ensamblaje.
   - _Resultado_: La ficha ya comunica detalles tecnicos utiles por tornillo y desglose madre -> subpiezas para la jerarquia actual del dron.
6. **Interaccion, hover e isolate cerrados para fasteners**:
   - _Implementacion_: `SelectionManager` resuelve cualquier click sobre geometria de fastener hacia el root completo del fastener en lugar de dejar la seleccion en submeshes parciales.
   - _Implementacion_: `HighlightSystem`, `MaterialController`, `FastenerInspectionManager` y `PartVisibilityManager` fueron ajustados para mantener hover/selected color aun cuando el proxy se oculta y para aislar cada fastener como unidad completa.
   - _Resultado_: La seleccion visual no se pierde al activar detalle procedural y el isolate deja de cortar tornillos de forma incompleta.
7. **Jerarquia madre -> subpiezas materializada**:
   - _Implementacion_: Se genero `holybro_parent_subpieces.json` y se sincronizaron `subComponentNames` sobre las piezas madre en `Assets/Core/Data/X500V2Generated`.
   - _Resultado_: La configuracion de la app ahora refleja la lista real de piezas madre con sus subpiezas documentadas y su resumen de fasteners, manteniendo explicito que la escena sigue siendo temporal/proxy.
8. **Validacion objetiva**:
   - _Implementacion_: Se compilaron `Core.csproj`, `UI.csproj` y `Assembly-CSharp-Editor.csproj`.
   - _Resultado_: Las tres compilaciones cerraron sin errores; quedaron solo warnings legacy/preexistentes de dependencias Unity y complejidad arquitectonica.
9. **Cierre correctivo sobre isolate y camara**:
   - _Implementacion_: `PartVisibilityManager` fue ajustado para distinguir entre aislamiento de fastener individual y aislamiento de pieza madre con fasteners asociados mediante `parentCanonicalPartId`.
   - _Implementacion_: Se elimino el caso donde un fastener aislado seguia mostrando geometria ancestro por evaluacion jerarquica demasiado amplia.
   - _Implementacion_: `OrbitCameraController` ahora usa bounds reales de la seleccion, amplia el rango practico de zoom y reduce el `near clip` para inspeccion comoda de piezas pequenas.
   - _Resultado_: El isolate de fastener queda unitario, el isolate de pieza madre incluye sus fasteners reconciliados y el enfoque de camara deja de quedarse excesivamente lejos en tornilleria y subpiezas pequenas.
10. **Ajuste fino de UX sobre zoom y hover residual**:
   - _Implementacion_: El zoom paso de un minimo global muy agresivo a una ventana dinamica por seleccion, con sensibilidad proporcional al rango disponible para cada pieza enfocada.
   - _Implementacion_: `SelectionManager` limpia el estado de hover al promover una pieza a seleccion y solo reaplica hover despues de deseleccionar si realmente existe un objeto bajo el cursor.
   - _Resultado_: La camara ya no sobrerreacciona en tornilleria pequena y desaparece el tinte azul residual al deseleccionar fasteners aislados desde el background.
11. **Escala adaptativa de navegacion para zoom, pan y orbit**:
   - _Implementacion_: `OrbitCameraController` reduce el minimo practico de acercamiento del dron completo y, cuando existe una seleccion activa, recalcula la ventana de zoom aunque la pieza no este aislada.
   - _Implementacion_: `OrbitCameraController` ahora modula `pan` y `orbit` con base en la escala efectiva de analisis para evitar desplazamientos excesivos sobre fasteners y subpiezas pequenas aisladas.
   - _Resultado_: El dron completo admite un acercamiento adicional controlado, la pieza seleccionada habilita un zoom mas fino incluso dentro del ensamblaje completo y la navegacion deja de perder fasteners por exceso de sensibilidad.
12. **Retune fino de navegacion sobre fasteners aislados**:
   - _Implementacion_: Se sustituyo la respuesta lineal de `pan` y `orbit` por curvas adaptativas en `OrbitCameraController`, reduciendo mas agresivamente el paneo en escalas pequenas y recuperando parte de la sensibilidad angular del orbit.
   - _Resultado_: El fastener aislado deja de desplazarse con demasiada facilidad durante `pan`, mientras que el `orbit` vuelve a sentirse mas reactivo sin volver al comportamiento tosco del rango global antiguo.
13. **Correccion del limite falso al deshacer zoom**:
   - _Implementacion_: `OrbitCameraController` ahora separa el contexto adaptativo por prioridad (`seleccion -> isolate -> contexto base del modelo`) para no heredar una ventana estrecha desde un `target` antiguo cuando ya no corresponde.
   - _Implementacion_: La ventana adaptativa conserva un margen minimo de alejamiento y clampa `targetDistance` al rango vigente para evitar que el zoom quede atrapado en un maximo demasiado pequeno.
   - _Resultado_: Incluso despues de enfocar o inspeccionar piezas pequenas, el usuario puede volver a alejar la camara sin quedar bloqueado por un limite residual.
14. **Refinamiento final de paneo sobre piezas pequenas**:
   - _Implementacion_: Se redujo aun mas el piso y la curva adaptativa de `pan` en `OrbitCameraController` para escalas de inspeccion pequenas.
   - _Resultado_: El desplazamiento lateral deja de sentirse nervioso cuando el usuario analiza tornillos, tuercas y subcomponentes diminutos.
15. **Activacion modular por aislamiento y ensamblaje**:
   - _Implementacion_: `FastenerInspectionManager` dejo de manejar una unica inspeccion seleccionada y ahora resuelve un conjunto activo de fasteners modulares segun el contexto runtime.
   - _Implementacion_: El detalle procedural se activa para el fastener seleccionado, para cualquier fastener aislado aunque ya no este seleccionado y para los fasteners reconciliados de una pieza madre aislada.
   - _Resultado_: El isolate ya no depende de mantener una seleccion dura para mostrar detalle modular y las piezas madre aisladas arrastran sus fasteners a la representacion detallada sin globalizar el reemplazo a toda la escena.
16. **Correccion de resolucion por meshes hijos de fasteners**:
   - _Implementacion_: `FastenerInspectionManager` ahora revisa el `FastenerRuntimeMarker` del root del fastener antes de detener el recorrido en la frontera `ExplodablePart`.
   - _Resultado_: Los clicks o focos sobre geometria hija pueden resolver el fastener completo sin volver a capturar markers de piezas madre o hermanos.

### Estado Actual

- La parte Unity del sistema de fasteners queda implementada a nivel de datos, runtime, UI y tooling de editor.
- La escena actual sigue tratandose como geometria temporal/proxy; el sistema fija `familyId` e `instanceId` para que el reemplazo por assets Blender no exija cambios de codigo.
- `parentCanonicalPartId` queda inferido por proximidad al anchor directo del dron porque la escena actual agrupa fasteners bajo `x500v2_fastener_group`.
- La interaccion actual de fasteners queda cerrada con seleccion completa, color visible durante inspeccion y aislamiento completo por tornillo.
- La jerarquia madre -> subpiezas queda versionada en `holybro_parent_subpieces.json` y replicada en `subComponentNames` para consumo directo desde la app.

### Primer Paso Al Retomar La Sesion

1. Abrir Unity y ejecutar `Tools/Thermal/Prepare Imported Drone For Thermal Test` sobre la escena vigente para refrescar assets/meta si hubo cambios manuales posteriores.
2. Validar en Play Mode seleccion individual de fasteners, detalle en `bottom sheet` y aparicion/desaparicion del placeholder detallado al seleccionar/deseleccionar.
3. Sustituir recetas placeholder por mallas Blender finales usando el mismo `recipeKey`, sin alterar `familyId`, `instanceId` ni los JSON ya fijados.

---

## Registro de Cambios (Abril 08, 2026) - Checkpoint clave de integracion runtime/UI y control de entrega

### Objetivo

Cerrar un checkpoint estable y publicable del prototipo, consolidando:

- split de trabajo en commits tematicos,
- trazabilidad de respaldo,
- alineacion de filtros Analyze con taxonomia de catalogo,
- preparacion de pipeline para dataset granular de piezas,
- y limpieza operativa del repositorio para continuar la etapa final.

### Acciones Realizadas

1. **Operacion Git y control de riesgo**:
   - _Implementacion_: Se genero snapshot completo de seguridad y luego split en 5 bloques tematicos (UI, tooling, thermal-data, modelos pesados, docs).
   - _Implementacion_: Se publicaron branch de split, branch de snapshot y tag de backup en remoto con carga LFS completada.
   - _Resultado_: Quedo trazabilidad total y punto de restauracion verificable.
2. **Correccion y unificacion de filtros Analyze**:
   - _Implementacion_: Se removio el boton `ALL` en filters y se dejo estado inicial con todas las categorias activas.
   - _Implementacion_: Click simple mantiene toggle por categoria; doble click activa modo exclusivo por categoria; doble click nuevamente revierte a estado por defecto (todas activas).
   - _Implementacion_: Se agrego categoria `SensorsComms` al panel Analyze para converger con `PartCatalogManager`.
   - _Resultado_: UX de filtros mas consistente y taxonomia mas alineada al catalogo real.
3. **Pipeline de piezas hacia dataset granular**:
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora prioriza `x500v2_blender_synced_parts.json` (55 piezas unicas) con fallback a `x500v2_parts_data.json` (28 canonicos).
   - _Implementacion_: Se agrego matching por `blenderName` cuando no hay match por `id`.
   - _Implementacion_: Normalizacion de categorias a enum final (`Avionics`, `SensorsComms`, `PowerDistribution`, `PropulsionSystem`, `SkeletonAirframe`, `Fasteners`).
   - _Resultado_: El pipeline ya soporta la ruta correcta para incluir TODAS las piezas en la siguiente pasada de regeneracion.
4. **Documentacion tecnica actualizada**:
   - _Implementacion_: Se complemento `INVENTARIO_PIEZAS_TERMICO_FILTROS_HOTSPOTS.md` (sin borrar historial), agregando estado implementado, nuevo UX de filtros y diagnostico de piezas no seleccionables/centradas.
   - _Resultado_: Queda trazabilidad clara de lo implementado vs lo pendiente de validacion en escena.

### Estado Actual

- La app queda funcional y estabilizada en un checkpoint clave.
- El repo queda limpio/controlado para continuar sin deuda operativa.
- El siguiente trabajo no es de infraestructura Git/UI base, sino de validacion final sobre modelo definitivo y cobertura de piezas.

### Primer Paso Al Retomar La Sesion

1. Ejecutar `Prepare Imported Drone For Thermal Test` sobre escena importada vigente.
2. Validar cobertura real de anclajes (55 unicas / instancias esperadas), piezas no seleccionables y casos centrados en origen.
3. Regenerar/ajustar grafo termico y vista explosiva con el modelo final para cerrar la etapa.

---

## Registro de Cambios (Abril 09, 2026) - Cierre de cobertura jerarquica y estabilizacion del setup

### Objetivo

Cerrar la validacion operativa de jerarquia importada y eliminar falsos negativos del setup/auditoria cuando la escena real use nomenclatura canonica.

### Acciones Realizadas

1. **Reparacion de jerarquia importada**:
   - _Implementacion_: Se reforzo el reparent de auxiliares con estrategia por grupo sintetico (`x500v2_fastener_group`, `x500v2_misc_group`) y por prefijo canonico.
   - _Implementacion_: Se habilito `UnpackPrefabInstance` en el setup para permitir reparent robusto sobre instancias importadas.
   - _Resultado_: Se logro `Renderers huérfanos en primer nivel: 0` y `Anchors sin renderer: 0` en la auditoria final.
2. **Estabilizacion del setup termico/import**:
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora selecciona dataset por cobertura real de escena (synced vs canonico), en lugar de forzar synced siempre.
   - _Resultado_: Ejecucion estable con `Fuente usada: x500v2_parts_data.json (matches: 28/28)`, `Preparadas 28`, `Warnings: 0`.
3. **Auditoria adaptativa**:
   - _Implementacion_: `ImportedDroneCoverageAudit` tambien selecciona la fuente esperada por cobertura real para reportar estado coherente con la escena cargada.
   - _Resultado_: Se mantiene trazabilidad de cobertura sin confundir mismatch de nomenclatura con fallos de runtime.

### Estado Actual

- Jerarquia saneada para seleccion/filtros/hotspots en la escena actual.
- Setup y auditoria con comportamiento consistente frente a escenas canonicas o granulares.
- Rama remota actualizada con commits de hardening y cierre de cobertura.

### Primer Paso Al Retomar La Sesion

1. Ejecutar validacion funcional fina en Play Mode (seleccion, filtros Analyze y hotspots) con captura de evidencias.
2. Si no aparecen regresiones, consolidar cierre documental en paquete final de entrega.

---

## Registro de Cambios (Abril 10, 2026) - Refinamiento final del componente termico

### Objetivo

Cerrar la etapa de presentacion del sistema termico ajustando el acabado visual del shader y dejando alineada la documentacion con la nueva granularidad del dron importado.

### Acciones Realizadas

1. **Refinamiento del shimmer termico**:
   - _Implementacion_: `Thermal.shader` ahora mezcla ruido de baja frecuencia, mas lento y mas suave.
   - _Resultado_: El modo Thermal deja de verse nervioso o granular en exceso sobre la malla retopologizada.
2. **Reduccion de microvariacion por defecto**:
   - _Implementacion_: `ThermalViewController` baja `defaultBandHalfWidth`, `criticalBandHalfWidth`, `passiveBandHalfWidth` y `baseVariation` por categoria canonica; `ThermalSurfaceProfile` adopta un default mas sobrio.
   - _Resultado_: Se conserva legibilidad en motores, ESC, bateria y stack central sin sobrerrepresentar piezas secundarias.
3. **Separacion entre lectura y acento visual**:
   - _Implementacion_: El edge glow deja de sumarse a la temperatura mostrada y pasa a actuar solo como realce visual.
   - _Resultado_: La lectura termica gana coherencia y evita falsos picos en bordes.
4. **Actualizacion documental**:
   - _Implementacion_: Se actualizan README termico, indices, verificaciones, documentacion tecnica, tesis, manual y breakdown.
   - _Resultado_: Queda documentado que el aumento de subpiezas visibles no redefine la jerarquia termica oficial de la V1.

### Estado Actual

- Refinamiento visual del sistema termico: completado a nivel de runtime.
- Regla de prioridad: se mantiene anclada en ensamblajes canonicos aunque la escena aumente de granularidad.
- Pendiente real: validacion final con el modelo optimizado definitivo y capturas de cierre.

---

## Registro de Cambios (Marzo 12, 2026) - Etapa 2: Bootstrap termico, verificacion Wolfram, retopologia

### Objetivo

Completar los cimientos de la Etapa 2 del sistema termico: asegurar que el solver y la capa visual se inicialicen en runtime, verificar las constantes de conductividad con datos de ingenieria, y proveer guia de retopologia al pipeline 3D.

### Acciones Realizadas

1. **Commit baseline del trabajo de Codex**:
   - _Accion_: Se comitio todo el trabajo que Codex dejo sin committear en la branch `thermal-simulation`.
   - _Justificacion_: Crear un punto de restauracion antes de modificaciones adicionales.
2. **Correccion de SceneBootstrapper**:
   - _Implementacion_: Se agrego `EnsureManager<ThermalSimulationManager>` y `EnsureManager<ThermalViewController>` en `SceneBootstrapper.ValidateManagers()`.
   - _Resultado_: El sistema termico ahora se inicializa automaticamente en runtime. La documentacion previa decia que esto estaba hecho, pero no lo estaba.
3. **Verificacion Wolfram V002 — Escalas de conductividad**:
   - _Accion_: Se verificaron las 7 escalas de conductividad de `EstimateMaterialConductivityScale()` contra datos de ingenieria (fuentes citadas).
   - _Resultado_: Las escalas son heuristicas comprimidas (rango 0.18–1.8 vs real 0.017–24.1). Se documento la justificacion de la compresion y se agrego comentario al codigo fuente.
4. **Guia de retopologia por pieza**:
   - _Implementacion_: Se creo `RETOPOLOGIA_POR_PIEZA.md` clasificando las 28 piezas en 3 tiers con presupuesto de poligonos, requisitos UV y funcion termica.
   - _Resultado_: Pipeline 3D tiene criterios claros para retopologizar segun prioridad termica.
5. **Correccion de documentacion**:
   - _Accion_: Se corrigieron afirmaciones falsas en `INDICE_TERMICO.md` sobre el SceneBootstrapper y el shader.
   - _Resultado_: La documentacion ahora refleja el estado real del codigo.

### Estado Actual

- Etapa 2 en progreso: bootstrap funcional, verificacion de constantes iniciada, guia de retopologia entregada.
- Shader pendiente de recibir parametros espaciales (Etapa 3).
- Asignacion de ThermalSurfaceProfile pendiente para piezas criticas.

---

## Registro de Cambios (Marzo 12, 2026) - Fundacion del Sistema Termico Hibrido

### Objetivo

Establecer la base fisica, visual, documental y operativa del subsistema de simulacion termica del Holybro X500 V2 sin comprometer el rendimiento en WebGL movil.

### Acciones Realizadas

1. **Definicion del modelo termico V1**:
   - _Decision_: Se adopto una simulacion hibrida por componentes y no un FEA completo en tiempo real.
   - _Justificacion_: WebGL movil exige priorizar estabilidad de framerate y credibilidad visual sobre solucion numerica de alta fidelidad.
2. **Alineacion del contrato de datos termicos**:
   - _Implementacion_: `DronePartData` fue extendido para exponer temperaturas minima y maxima operativa, hover, pico, warmup, exposicion y escala de conduccion.
   - _Resultado_: El solver y la capa visual ya consumen un contrato termico consistente dentro del codebase actual.
3. **Control de carga y activacion del dron**:
   - _Implementacion_: `DroneStateController` ahora mantiene `SystemLoadFactor`, publica eventos de carga y adapta el comportamiento entre `Idle` y `Flying`.
   - _Implementacion_: `InspectModeHandler` y `MainLayout.uxml` integran un slider de carga sostenida bajo el control de `Power`.
4. **Preprocesado offline del grafo termico**:
   - _Implementacion_: Se creo `ThermalContactGraphBuilderWindow`, una herramienta de editor que analiza bounds de `ExplodablePart` y genera `ThermalContactGraphAsset` con metadata y enlaces termicos candidatos.
   - _Resultado_: El sistema deja de depender conceptualmente solo del fallback heuristico y gana una ruta profesional de authoring offline.
5. **Integracion minima de runtime**:
   - _Implementacion_: `SceneBootstrapper` puede asegurar `ThermalSimulationManager` y `ThermalViewController` sin modificar escenas serializadas.
   - _Resultado_: La base termica puede inicializarse por codigo en escenas que ya usen el bootstrapper.
6. **Gobernanza documental y workflow matematico**:
   - _Implementacion_: Se mantuvo el workflow local de verificacion con WolframAlpha y se actualizaron los documentos tecnicos para reflejar el nuevo hito.
   - _Resultado_: El subsistema termico queda trazable tanto en codigo como en documentacion viva.

### Estado Actual

- Cimientos funcionales de la etapa 1 completados.
- Etapa 2 ya cuenta con una ruta de preprocesado offline del grafo termico en editor.
- Etapa 3 sigue en integracion parcial a nivel visual y de authoring.
- La siguiente prioridad tecnica es calibrar el grafo generado sobre la geometria final del X500 V2 y asignar perfiles termicos a las piezas criticas.

## Fase 1: Investigación y Conceptualización (Julio - Agosto 2025)

### Objetivo

Definir la viabilidad técnica y conceptual de un visualizador interactivo 3D para educación en ingeniería, superando las limitaciones de los manuales estáticos PDF.

### Acciones Realizadas

1.  **Revisión Bibliográfica y del Estado del Arte**:
    - _Acción_: Análisis de 40+ referencias (papers, tesis, herramientas existentes).
    - _Justificación_: Identificar brechas en las herramientas actuales (baja interactividad, requisitos de hardware altos).
    - _Conclusión_: Se opta por **WebGL 2.0** para garantizar acceso universal sin instalación.
2.  **Selección del Stack Tecnológico**:
    - _Decisión_: Unity 6 (URP) vs Three.js.
    - _Razón_: Unity ofrece un pipeline de assets más robusto y herramientas de editor visual para configuraciones complejas (ECS/Prefabs), ideal para un prototipo escalable.
    - _Decisión_: Universal Render Pipeline (URP).
    - _Razón_: Optimización nativa para plataformas móviles y WebGL, permitiendo shaders personalizados ligeros.

---

## Fase 2: Pipeline de Producción 3D (Septiembre - Octubre 2025)

### O

### 2026-03-17: Etapa 5 Thermal Legend y Wolfram (V003, V004)

- **UI Añadida**: Creado ThermalLegendContainer en MainLayout.uxml y estilos con gradiente lineal (linear-gradient) en Theme.uss.
- **Sincronización de UI**: Actualizado UIAnalyzePanel.cs para mostrar la leyenda solo cuando el Shading Mode es "Thermal".
- **Sincronización de Datos**: Actualizado ThermalViewController.cs para inyectar automáticamente displayMinTemperatureC y displayMaxTemperatureC en las etiquetas UI conectadas al visualizador.
- **Wolfram V003/V004**: Validado WarmupSeconds (tau) y coolingRate. El cálculo real muestra que una $\tau$ verdadera tomaría ~7 minutos (410s) en llegar al equilibrio, justificando plenamente el valor acelerado del solver (10-15s) para UX y performance visual interactiva en la app.

---

## Registro de Cambios (Marzo 19, 2026) - Rescate del sistema termico y etapa 4 canonica

### Objetivo

Sanear la rama termica, consolidar el camino oficial del runtime y corregir la documentacion para que refleje el estado real del subsistema.

### Acciones Realizadas

1. **Auditoria de cambios recientes**:
   - _Accion_: Revision tecnica del trabajo heredado para clasificar que se conserva, que se corrige y que queda como experimental.
   - _Resultado_: Se mantiene la arquitectura base del solver, shader, leyenda UI y tooling editor; se eliminan sobrepromesas documentales.
2. **Grafo canónico oficial**:
   - _Implementacion_: Creacion de `Assets/Resources/ThermalCanonicalContactGraph.asset` con la red explicita de contactos para las 28 piezas canonicas.
   - _Resultado_: El runtime ya no depende del fallback heuristico como camino oficial.
3. **Endurecimiento de `ThermalViewController`**:
   - _Implementacion_: Cache de la leyenda UI, presets canónicos por pieza critica y mejor resolucion de hotspots/direccion cuando no existe `ThermalSurfaceProfile` manual.
   - _Resultado_: Mejor estabilidad y mejor lectura visual sin tocar escenas serializadas.
4. **Aislamiento del flujo CAD bruto**:
   - _Implementacion_: `ThermalTestSetup.cs` se reetiqueto como experimental y dev-only.
   - _Resultado_: Queda claro que no forma parte del pipeline oficial del sistema termico.
5. **Gobernanza documental**:
   - _Implementacion_: Reescritura de `README.md`, `AGENT_HANDOFF_THERMAL.md`, `RETOPOLOGIA_POR_PIEZA.md`, indices y matriz documental; actualizacion de docs tecnicas y de portafolio.
   - _Resultado_: La documentacion ahora distingue entre implementado, integrado, validado, experimental y pendiente.

### Estado Actual

- Runtime oficial: solver por 28 piezas canonicas + grafo explicito + shader espacial + leyenda UI.
- Carga termica visible: gobernada por `DroneStateController.SystemLoadFactor` y estados del dron.
- Pendiente real: validar la escena final retopologizada, asignar overrides manuales solo donde aporten valor y medir rendimiento en Unity Editor/build objetivo.

---

## Registro de Cambios (Marzo 31, 2026) - Integracion del dron importado y rescate de runtime

### Objetivo

Cerrar la brecha entre el FBX importado `x500v2_Drone` y los sistemas de seleccion, filtros, explode, cut y thermal.

### Acciones Realizadas

1. **Bootstrap runtime reforzado**:
   - _Implementacion_: `UIManager` ahora puede autocrear `InputManager`, `SelectionManager`, `ExplodedViewManager`, `PartCatalogManager`, `NotificationManager`, `DroneStateController`, `ThermalSimulationManager` y `ThermalViewController` cuando la escena no tenga bootstrap serializado.
   - _Resultado_: La escena queda mas resiliente aunque falte `SceneBootstrapper`.
2. **Binder del dron importado**:
   - _Implementacion_: Se creo `ImportedDroneRuntimeBinder` para reconectar `DroneStateController`, recachear `ViewModeManager`, `ExplodedViewManager`, `PartVisibilityManager`, `PartCatalogManager`, `CrossSectionManager`, `ThermalSimulationManager` y `ThermalViewController`.
   - _Resultado_: El modelo importado puede reengancharse al runtime sin tocar escenas serializadas a mano.
3. **Preparacion del FBX endurecida**:
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora clasifica auxiliares, anade `PartRenderCategory`, crea colliders tipo box por renderer, asigna la layer `SelectablePart` y coloca el binder runtime en el root.
   - _Resultado_: Mejora directa de selection, filter, isolate, thermal y explode sobre el dron importado.
4. **Seleccion y categorias publicas**:
   - _Implementacion_: La seleccion ya resuelve al `ExplodablePart` canonico y la taxonomia publica de filtros queda en `ALL`, `STRUCTURE`, `PROPULSION`, `ELECTRONICS`, `FASTENERS`, `MISC`.
   - _Resultado_: `Fasteners` y `Misc` pasan a ser grupos visuales publicos sin ampliar el solver termico V1.
5. **Leyenda y panel de energia**:
   - _Implementacion_: La leyenda termica ya usa gradiente runtime y el modo Inspect expone el panel de power con estado y slider de carga.
   - _Resultado_: El feedback visual del sistema termico y del estado del dron deja de ser opaco en la UI.

---

## Registro de Cambios (Mayo 08, 2026) - Jerarquia explicita de seleccion y hotspots

### Objetivo

Corregir la contaminacion entre hotspots, piezas canonicas, subpiezas Blender y fasteners primitivos del dron X500 V2 importado.

### Acciones Realizadas

1. **Contrato unico de jerarquia**:
   - _Implementacion_: Se agrego `holybro_selection_hierarchy.json` y `SelectionHierarchy` como referencia comun para hotspots, piezas canonicas, subpiezas y fasteners primitivos.
   - _Resultado_: La seleccion deja de depender de keywords amplias como `rail`, `mount`, `m3` o `dingwei`.
2. **Hotspots explicitos**:
   - _Implementacion_: `HotspotManager` usa membresia canonica fija para `Power Distribution`, `Flight Controller`, `GPS & Compass`, `Propulsion System` y `Battery`.
   - _Resultado_: `GUAN-CHENG` queda fuera de `Power Distribution` y permanece en el sistema de railes/bateria.
3. **Fasteners modulares restringidos**:
   - _Implementacion_: El reemplazo modular se limita a objetos identificados como fasteners primitivos o importados bajo el grupo runtime de fasteners.
   - _Resultado_: Subpiezas CAD como tubos, stoppers, grommets o brackets no se convierten en tornillos por heuristica de nombre.
4. **Seleccion por capas**:
   - _Implementacion_: El primer click sobre un fastener sube a la pieza madre canonica; un segundo click dentro del mismo contexto selecciona el fastener individual.
   - _Resultado_: Se formaliza el flujo pieza madre -> subpieza -> fastener sin colapsar todos los brazos o todos los fasteners.
5. **Restauracion de hotspots y anclas sin heuristicas amplias**:
   - _Implementacion_: El evento de aislamiento de hotspot conserva si debe incluir fasteners asociados y `HotspotManager` deja de usar tokens como `m2`, `m3`, `nut` o `screw` para escoger anclas.
   - _Resultado_: Al volver desde una subcapa al hotspot aislado se preservan los fasteners confirmados y no se privilegian falsos positivos por nombre.

### Estado Actual

- La fuente de verdad runtime queda en la capa canonica `x500v2_*`.
- La jerarquia granular de Blender enriquece seleccion y filtros, pero no reemplaza la semantica canonica.
- Las discrepancias de conteo entre fasteners teoricos y fasteners de escena siguen reportandose para revision, no se asumen manualmente.
- La copia documental de la jerarquia queda registrada en `desarrollo/docs/investigacion/Holybro/holybro_selection_hierarchy.json`.

---

## Registro de Cambios (Mayo 09, 2026) - Limpieza de cimientos para seleccion jerarquica y fasteners

### Objetivo

Eliminar causas estructurales que contaminaban la seleccion por capas antes de reconstruir el flujo final de grupos, subpiezas y fasteners.

### Acciones Realizadas

1. **Correccion de falso fastener estructural**:
   - _Implementacion_: `HMX5V-GUAN-DINGWEI` queda bloqueado como subpieza estructural mediante `SelectionHierarchy.IsKnownStructuralNonFastenerName`.
   - _Resultado_: El stopper de posicionamiento del tubo ya no puede recibir `FastenerRuntimeMarker` ni reemplazo modular de tornillo.
2. **Limpieza y reconciliacion de catalogos Holybro**:
   - _Implementacion_: Se bloqueo `HMX5V-GUAN-DINGWEI` como falso fastener estructural y se reconocieron explicitamente `HUAN-GUIJIAO` y `GPSV5-ZHIJIA-LUOMAO` como fasteners reales cuando provienen del inventario/catalogo.
   - _Resultado_: El catalogo runtime esperado queda en 19 familias y 170 instancias primitivas de fastener, sin entradas falsas de `HMX5V-GUAN-DINGWEI`.
3. **Jerarquia fisica saneada**:
   - _Implementacion_: Las rutas `hierarchyPath` de fasteners apuntan al grupo raiz `x500v2_Drone/x500v2_fastener_group`, no a `x500v2_arm_BR`.
   - _Resultado_: Se evita que el brazo BR absorba semanticamente todos los fasteners de la escena.
4. **Resolucion estable por cuadrante**:
   - _Implementacion_: Los sufijos Blender `.001_low`, `.002_low`, `.003_low`, `.004_low` se traducen explicitamente a BR, FR, FL y BL para piezas repetidas por cuadrante.
   - _Resultado_: La seleccion de subpiezas de brazos deja de depender solo de distancia o posicion en mundo.
5. **Reconocimiento robusto de meshes hijos**:
   - _Implementacion_: `SelectionManager`, `PartVisibilityManager` y `UIManager` resuelven marcadores de fastener en el primer ancestro `ExplodablePart` sin cruzar a ancestros compartidos.
   - _Resultado_: Los fasteners modulares o definitivos con malla hija pueden seguir siendo seleccionables sin contaminar hermanos.
6. **Escala runtime alineada con shaders y fasteners modulares**:
   - _Implementacion_: El importador final calibra el FBX a tamano dominante objetivo `10u` y la auditoria runtime valida ese rango.
   - _Resultado_: El dron importado recupera una escala compatible con los materiales/modos de visualizacion ajustados para el dron anterior y con el tamano de los placeholders modulares.
7. **Colliders de seleccion exactos**:
   - _Implementacion_: El setup editor y el binder runtime reemplazan `BoxCollider` genericos por `MeshCollider` cuando existe `MeshFilter`, dejando box collider solo como fallback.
   - _Resultado_: Se reduce la seleccion cruzada causada por bounds grandes/desplazados del FBX instanciado; un click sobre una subpieza ya no deberia golpear cajas de fasteners o piezas vecinas.

### Estado Actual

- Runtime Player y UI Player compilan sin errores.
- La compilacion editor por `dotnet` sigue bloqueada por `UnityEditor.UI` en `Library/PackageCache`, no por los scripts modificados.
- La proxima validacion debe hacerse reimportando el FBX en Unity y revisando que el reporte indique escala dominante `5u -> 10u` y 170 fasteners normalizados, o que cualquier diferencia quede justificada por la escena actual.

---

## Registro de Cambios (Mayo 10, 2026) - Calibracion visual de escala, camara y Blueprint

### Objetivo

Corregir la primera regresion visible del FBX final: el dron quedaba fuera de la escala operativa esperada por la camara, el zoom y los shaders, y el modo Blueprint se veia como una masa blanca por lineas demasiado gruesas.

### Acciones Realizadas

1. **Camara dependiente de bounds reales**:
   - _Implementacion_: `OrbitCameraController` ahora busca el root activo `x500v2_Drone`, ignora referencias serializadas obsoletas al modelo preservado y calcula la distancia base desde los bounds del dron.
   - _Resultado_: `ResetView` y el arranque dejan de depender de una distancia fija heredada; el modelo completo se encuadra con margen aunque el FBX haya sido reescalado.
2. **Referencia de camara actualizada al importar**:
   - _Implementacion_: `ImportDroneModel` reescribe la referencia serializada del `OrbitCameraController` hacia el nuevo root importado.
   - _Resultado_: Se evita que la escena conserve la camara apuntando al modelo viejo inactivo usado como referencia.
3. **Blueprint sin edge pass de pantalla**:
   - _Implementacion_: `ViewModeManager` desactiva el `EdgeDetectionFeature` para Blueprint y calibra siempre los materiales, incluso si ya estaban serializados en la escena.
   - _Resultado_: El modo Blueprint usa el shader propio como fuente de lineas, sin pintar todas las discontinuidades de normales/profundidad como blanco solido.
4. **Lineas y silueta mas finas**:
   - _Implementacion_: `Blueprint.shader` reduce `_OutlineWidth` a `0.00035`, sube `_EdgeThreshold` a `0.82` y usa `_FresnelPower` `8.0`; `SolidColor.shader` reduce outline a `0.00045`.
   - _Resultado_: La silueta queda mas cercana al grosor esperado para el dron final de `10u` y evita el manchon blanco de la captura.

### Estado Actual

- `Core.Player.csproj` compila sin errores.
- `UI.Player.csproj` compila sin errores.
- La compilacion editor por `dotnet` sigue bloqueada por `UnityEditor.UI` en `Library/PackageCache`; no llego a compilar el assembly editor por esa dependencia externa.
- La validacion visual final debe hacerse dentro de Unity: reimportar el FBX si aplica, abrir Blueprint y confirmar que el dron completo se encuadra y que la linea tecnica queda delgada.

---

## Registro de Cambios (Mayo 10, 2026) - Ajuste fino de lineas e aislamiento de fasteners

### Objetivo

Corregir los ultimos defectos observados tras la importacion final: las lineas de Blueprint/Solid seguian demasiado gruesas y el aislamiento de fasteners heredaba hermanos asociados a la misma pieza madre.

### Acciones Realizadas

1. **Lineas de shader mas delgadas**:
   - _Implementacion_: `Blueprint` reduce `_OutlineWidth` a `0.00008` y sube `_EdgeThreshold` a `0.94`; `SolidColor` reduce `_OutlineWidth` a `0.0001`.
   - _Resultado_: El modo Blueprint/Solid queda ajustado para el dron a `10u` sin depender del edge detection de pantalla.
2. **Aislamiento por capa real**:
   - _Implementacion_: `PartVisibilityManager` solo incluye fasteners asociados cuando el scope aislado es una pieza madre completa; subpiezas y fasteners individuales ya no heredan todos los fasteners del `parentCanonicalPartId`.
   - _Resultado_: Aislar un fastener debe mostrar solo ese fastener; aislar una subpieza ya no arrastra todos los fasteners de la pieza madre.
3. **Fastener como subcapa de su pieza canonica**:
   - _Implementacion_: `UIManager` resuelve un fastener hacia `ParentCanonicalPartId` para la pila de aislamiento, no hacia `x500v2_fastener_group`.
   - _Resultado_: La navegacion de capas queda como pieza canonica -> fastener, evitando restauraciones accidentales al grupo global de fasteners.
4. **Highlight de pieza madre con fasteners**:
   - _Implementacion_: `SelectionManager` aplica un tint suave adicional a los `FastenerRuntimeMarker` cuyo `ParentCanonicalPartId` coincide con la pieza madre seleccionada.
   - _Resultado_: Al seleccionar una pieza madre, sus fasteners asociados quedan visualmente incluidos en el estado de seleccion.

### Estado Actual

- `Core.Player.csproj` compila sin errores.
- `UI.Player.csproj` compila sin errores.
- La prueba visual pendiente en Unity debe confirmar tres casos: seleccion de pieza madre con fasteners resaltados, aislamiento de fastener individual sin hermanos, y Blueprint/Solid con linea fina.

---

## Registro de Cambios (Mayo 10, 2026) - Capas de subpieza y fasteners asociados

### Objetivo

Refinar el sistema de aislamiento jerarquico para que una subpieza pueda mostrarse primero con los fasteners fisicamente asociados a ella y despues aislarse sola, sin mezclar todos los fasteners de la pieza madre.

### Acciones Realizadas

1. **Modo de aislamiento por fastener explicito**:
   - _Implementacion_: `PartVisibilityManager` diferencia tres rutas: pieza madre con fasteners del padre, subpieza con fasteners asociados por contacto y scope estricto sin fasteners adicionales.
   - _Resultado_: La subpieza ya no hereda todos los fasteners de la pieza canonica; solo se agregan fasteners cuya geometria queda en contacto o proximidad controlada con esa subpieza.
2. **Tercer nivel de capa para subpieza sola**:
   - _Implementacion_: `UIManager` registra si la subpieza aislada contiene fasteners asociados; repetir aislamiento sobre la misma subpieza cambia a modo estricto.
   - _Resultado_: El flujo queda como pieza madre -> subpieza con fasteners asociados -> subpieza sola.
3. **Fastener directo con contexto obligatorio**:
   - _Implementacion_: El doble click directo sobre un fastener sin contexto primero entra a la pieza madre; solo despues puede aislarse el fastener. En hotspots que no contienen piezas madre completas se permite la ruta directa definida por el hotspot.
   - _Resultado_: El flujo evita saltos abruptos a fastener individual y conserva la lectura pedagogica de montaje.
4. **Retorno fiel a la capa previa**:
   - _Implementacion_: La pila de UI diferencia si la subpieza aislada viene de una pieza madre o de un hotspot sin pieza madre completa.
   - _Resultado_: Al salir de la subpieza, la app vuelve al hotspot cuando esa era la capa anterior real.
5. **Retroceso simetrico desde subpieza sola o fastener**:
   - _Implementacion_: `UIManager` conserva una capa de retorno intermedia cuando se avanza desde `subpieza + fasteners asociados` hacia `subpieza sola` o `fastener`.
   - _Resultado_: Al retroceder desde la subpieza sola o desde un fastener, la app restaura primero `subpieza + fasteners asociados` y despues permite volver a pieza madre/hotspot.

### Estado Actual

- `Core.Player.csproj` compila sin errores.
- `UI.Player.csproj` compila sin errores al reconstruir referencias de proyecto.
- La validacion visual pendiente debe confirmar ida y vuelta en los tres caminos: pieza madre -> subpieza con fasteners -> subpieza sola; pieza madre -> subpieza con fasteners -> fastener; pieza madre -> fastener.

---

## Registro de Cambios (Mayo 10, 2026) - Fasteners compartidos y proxies ESC heredados

### Objetivo

Corregir la prioridad de fasteners compartidos para que el historial de capas tenga prioridad sobre el padre canonico por defecto, y retirar de runtime los proxies temporales ESC que no forman parte del FBX final importado.

### Acciones Realizadas

1. **Contexto de seleccion para fasteners compartidos**:
   - _Implementacion_: `SelectionManager` conserva `CurrentFullSelection` y permite seleccionar un fastener compartido dentro del contexto actual si hay asociacion geometrica con la pieza/subpieza activa.
   - _Resultado_: Si un fastener tiene padre prioritario A pero el usuario llega desde B, el fastener se aisla dentro del camino de B y el retroceso vuelve a B.
2. **Resolucion contextual en aislamiento UI**:
   - _Implementacion_: `UIManager` resuelve fasteners con un contexto preferido antes de caer a `ParentCanonicalPartId`.
   - _Resultado_: La pila de aislamiento respeta el camino real de entrada: pieza madre, subpieza con fasteners, subpieza sola o fastener.
3. **Fasteners compartidos visibles en pieza madre**:
   - _Implementacion_: `PartVisibilityManager` agrega fasteners asociados por contacto/proximidad al aislamiento de una pieza madre, incluso si su padre prioritario es otra pieza.
   - _Resultado_: Los fasteners que pertenecen a una subpieza tambien aparecen como parte de la pieza madre de esa subpieza.
4. **Supresion de proxies ESC heredados**:
   - _Implementacion_: `SetupImportedDroneThermalTest` deja de crear proxies `x500v2_esc_*` si no existe geometria FBX real y elimina anchors `_runtime_proxy` ESC generados previamente.
   - _Resultado_: Los `ESC BLHeli-S 20A` dejan de aparecer como piezas runtime artificiales cuando no vienen en el modelo importado.

### Estado Actual

- `Core.Player.csproj` compila sin errores.
- `UI.Player.csproj` compila sin errores.
- La compilacion editor por `dotnet` sigue bloqueada por dependencias de paquetes Unity (`UnityEditor.UI` / metadatos generados), no por los cambios runtime verificados.

---

## Registro de Cambios (Mayo 10, 2026) - Limpieza de proxies y mapa de pertenencia runtime

### Objetivo

Alinear la configuracion runtime con el FBX final real: no sintetizar piezas que no existen en el modelo importado y dejar una tabla auditable de piezas madre, hotspots, subpiezas y fasteners basada en los JSON actuales.

### Acciones Realizadas

1. **Supresion de proxies no pertenecientes al FBX final**:
   - _Implementacion_: `SetupImportedDroneThermalTest` ahora omite y elimina proxies runtime para `x500v2_pdb`, `x500v2_rc_receiver` y `x500v2_esc_*`.
   - _Resultado_: `pdb_runtime_proxy`, `rc_receiver_runtime_proxy` y las ESC sinteticas dejan de recrearse al reimportar si Blender no entrega geometria real para ellas.
2. **Hotspots ajustados a piezas presentes**:
   - _Implementacion_: `holybro_selection_hierarchy.json` y el fallback de `SelectionHierarchy` excluyen `x500v2_pdb` del hotspot `Power Distribution` y excluyen `x500v2_esc_*` del hotspot `Propulsion System`.
   - _Resultado_: Los hotspots dejan de apuntar a anchors ausentes o proxies artificiales.
3. **Auditoria coherente con piezas documentadas pero ausentes**:
   - _Implementacion_: `RuntimeDroneSceneAuditor` distingue ids canonicos documentados de ids requeridos por el FBX actual.
   - _Resultado_: La ausencia de `PDB`, `RC Receiver` y ESC ya no se reporta como error si no existe geometria real importada.
4. **Mapa Markdown de pertenencia**:
   - _Implementacion_: Se genero `desarrollo/docs/investigacion/Holybro/holybro_runtime_selection_mapping.md`.
   - _Resultado_: El documento lista piezas madre, hotspots, subpiezas y fasteners prioritarios por pieza madre, marcando explicitamente que la prioridad fina por subpieza se resuelve por contacto runtime y aun no esta serializada como JSON persistente.

### Estado Actual

- `Core.Player.csproj` compila sin errores.
- `UI.Player.csproj` compila sin errores; la primera corrida paralela bloqueo temporalmente `Core.dll`, por lo que se repitio `Core.Player` de forma aislada.
- Tras reimportar desde Unity, el audit deberia dejar de reportar como error la ausencia de `x500v2_esc_*` y deberia eliminar los proxies `pdb_runtime_proxy` / `rc_receiver_runtime_proxy`.

---

## Registro de Cambios (Mayo 10, 2026) - Auditoria UX y mapa de fasteners por subpieza

### Objetivo

Definir una base explicita para saber que fasteners pertenecen a cada subpieza y auditar si la distribucion actual de piezas madre/hotspots favorece una UX clara de inspeccion y montaje.

### Acciones Realizadas

1. **Mapa propuesto `subpieza -> fasteners`**:
   - _Implementacion_: Se genero `desarrollo/docs/investigacion/Holybro/holybro_subpiece_fastener_map.json`.
   - _Resultado_: Los `170` fasteners actuales quedan asignados a subpiezas prioritarias, con `192` relaciones compartidas/contextuales para piezas que comparten tornillos o interfaces.
2. **Niveles de confianza**:
   - _Implementacion_: Cada fila del mapa incluye `confidence` y `reviewReasons`.
   - _Resultado_: Las relaciones seguras se distinguen de las inferidas; los casos `low` quedan listos para confirmar visualmente en Blender/Unity antes de tratarlos como verdad final.
3. **Auditoria UX de seleccion y hotspots**:
   - _Implementacion_: Se genero `desarrollo/docs/investigacion/Holybro/holybro_selection_ux_audit.md`.
   - _Resultado_: El informe identifica problemas de nomenclatura, hotspots demasiado amplios, piezas documentadas sin geometria y oportunidades de mejorar la navegacion por cuadrantes/subsistemas.

### Estado Actual

- El mapa esta documentado y listo para conectarse al runtime como fuente explicita.
- La seleccion actual puede seguir usando contacto/bounds como fallback, pero el siguiente paso recomendado es que Unity lea `holybro_subpiece_fastener_map.json` para hacer deterministica la capa `subpieza + fasteners`.
