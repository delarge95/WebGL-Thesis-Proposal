# Bitácora de Desarrollo — Registro Integral de Ejecución

> Documento oficial de seguimiento del proyecto "WebGL Drone Viewer".
> Registro detallado de decisiones técnicas, arquitectura y evolución del sistema.
> Alineado con el Cronograma Oficial (Fases 1-7: H2 2025 | Fase 8: Q1 2026).

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
### 2026-03-17: Etapa 5  Thermal Legend y Wolfram (V003, V004)
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
