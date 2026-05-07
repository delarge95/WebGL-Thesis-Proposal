# Bitácora de Desarrollo — Registro Integral de Ejecución

> Documento oficial de seguimiento del proyecto "WebGL Drone Viewer".
> Registro detallado de decisiones técnicas, arquitectura y evolución del sistema.
> Alineado con el Cronograma Oficial (Fases 1-7: H2 2025 | Fase 8: Q1 2026).

---

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
