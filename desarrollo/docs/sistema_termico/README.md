---
tipo: sistema
estado: mantenido
---
#sistema #configuracion_tecnica
# Sistema térmico del dron

## Objetivo

Implementar una simulación térmica híbrida para el Holybro X500 V2 que sea físicamente consistente, visualmente creíble y viable en Unity WebGL móvil. La V1 no busca resolver un FEA térmico completo en tiempo real sobre toda la malla visual, sino combinar:

- solver reducido por componentes canónicos,
- grafo explícito de contactos térmicos,
- propagación visual espacial en piezas críticas,
- y tooling offline para mantener el runtime liviano.

## Estado real al 2026-03-19

Implementado y vigente:

- `ThermalSimulationManager` como solver por nodos térmicos.
- `ThermalViewController` como puente solver-render.
- `Thermal.shader` con soporte espacial por pieza.
- `ThermalContactGraphAsset` como formato serializable de contactos.
- `ThermalContactGraphBuilderWindow` como herramienta de authoring offline.
- `ThermalCanonicalContactGraph.asset` como grafo canónico oficial de la V1.
- leyenda térmica visible en UI a través de `MainLayout.uxml`, `Theme.uss` y `UIAnalyzePanel.cs`.
- `SceneBootstrapper` capaz de levantar los managers térmicos sin tocar escenas serializadas.
- workflow de verificación matemática con WolframAlpha y trazabilidad en `wolfram_verificaciones.md`.

Implementado pero todavía no validado sobre la escena final retopologizada:

- presets térmicos canónicos para motores, ESC, batería, brazos, plates, PDB, power module y Pixhawk.
- propagación de calor por el grafo explícito hacia brazos, plates y stack central.
- carga térmica gobernada por `DroneStateController` y su `SystemLoadFactor`.

No debe afirmarse todavía:

- termografía validada experimentalmente,
- CFD,
- FEA térmico completo,
- precisión cuantitativa cerrada frente a mediciones reales.

## Actualizacion 2026-05-11

El FBX final importado ya no coincide exactamente con todos los nodos intermedios del grafo canonico original: algunas piezas documentadas funcionan como piezas canonicas de estudio, pero no existen como geometria visual runtime. Para evitar que esto corte rutas de conduccion, el solver crea puentes termicos conservadores entre vecinos presentes y mantiene enlaces suplementarios para el stack central, plates, rails y landing gear.

La visualizacion tambien distingue subpiezas granulares dentro de una misma pieza canonica. El solver conserva temperaturas por nodo canonico, pero `ThermalViewController` aplica una escala visual por subpieza y usa focos de contacto dinamicos para plates y carriers estructurales.

Audit relacionado:

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AUDIT_TERMICO_FBX_FINAL_2026-05-11.md`

## Decisiones cerradas

- La simulación es cualitativa avanzada y defendible como aproximación física.
- La transferencia principal es por conducción entre sólidos en contacto.
- La convección se modela de forma simplificada hacia el ambiente.
- La radiación queda fuera de la V1.
- La simulación corre en segundo plano y persiste al cambiar de vista.
- La vista explosionada no altera la física, solo la lectura visual.
- El runtime oficial usa las 28 piezas canónicas de `x500v2_parts_data.json`.
- Los 55 subcomponentes CAD/Blender se documentan como capa de modelado e importación, no como nodos oficiales del solver V1.

## Arquitectura oficial de la V1

### 1. Capa física

Cada pieza relevante se resuelve como un nodo térmico con:

- temperatura actual,
- temperatura de equilibrio según carga,
- capacidad térmica efectiva,
- pérdida al ambiente,
- y enlaces de conducción con piezas vecinas.

Modelo base:

```text
C_i dT_i/dt = Q_i(u, estado) + sum_j G_ij (T_j - T_i) - H_i (T_i - T_amb)
```

Aproximaciones usadas:

```text
G_ij ~= k_eff * A_contacto / L_efectiva
H_i ~= h_i * A_expuesta
```

### 2. Capa de contactos

El camino oficial ya no depende solo del heurístico. El runtime intenta cargar primero:

- `Assets/Resources/ThermalCanonicalContactGraph.asset`

Si ese asset existe y tiene enlaces válidos, el solver usa el grafo explícito. Si falta, cae al fallback heurístico solo como respaldo de desarrollo.

### 3. Capa visual

`ThermalViewController` aplica temperaturas al shader por `MaterialPropertyBlock` y resuelve por pieza:

- modo uniforme,
- modo radial,
- modo axial,
- hotspot local,
- dirección local,
- spread,
- edge cooling,
- propagation.

`ThermalSurfaceProfile` sigue siendo el override manual por pieza. Cuando no existe, el controlador usa presets canónicos por tipo de pieza.

### 4. Capa UI

La leyenda térmica ya existe y debe describirse como `display-range`, no como validación física absoluta. Hoy el control de carga visible al usuario no está reintegrado como slider dedicado en esta rama; la simulación se alimenta del estado del dron y de `SystemLoadFactor` desde `DroneStateController`.

### 5. Tooling offline

- `ThermalContactGraphBuilderWindow` genera candidatos a partir de bounds.
- `ThermalTestSetup.cs` se conserva como harness experimental para pruebas CAD crudas.
- La ruta oficial para la escena final es la geometría retopologizada limpia con nombres canónicos o mapeables.

## Piezas críticas de la V1

Las piezas con mayor exigencia térmica y visual son:

- 4 motores
- 4 ESC
- batería
- PDB
- power module
- Pixhawk
- 4 brazos
- top plate
- bottom plate

En estas piezas se prioriza:

- topología predecible,
- contacto claro,
- loops donde nace o viaja el calor,
- y propagación espacial visual coherente.

## Estado por etapas

- Etapa 0. Fundaciones: completada.
- Etapa 1. Solver por componentes: completada y operativa.
- Etapa 2. Grafo de contactos: completada a nivel de formato y asset canónico; pendiente calibración final sobre geometría retopologizada.
- Etapa 3. Propagación visual: completada en base operativa con presets canónicos; pendiente asignación manual fina de `ThermalSurfaceProfile` en la escena final.
- Etapa 4. Integración oficial: en progreso. La base ya usa grafo explícito y leyenda UI, pero falta validación en la escena definitiva del X500 V2.
- Etapa 5. Validación: pendiente en Unity Editor y en build objetivo.

## Ruta de archivos clave

- Arquitectura: `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
- Handoff: `E:\WebGL_tesis\desarrollo\docs\sistema_termico\AGENT_HANDOFF_THERMAL.md`
- Guía 28+55 de modelado: `E:\WebGL_tesis\desarrollo\docs\sistema_termico\RETOPOLOGIA_POR_PIEZA.md`
- Índice general: `E:\WebGL_tesis\desarrollo\docs\sistema_termico\INDICE_TERMICO.md`
- Matriz documental: `E:\WebGL_tesis\desarrollo\docs\sistema_termico\MATRIZ_ACTUALIZACION_DOCUMENTAL.md`
- Solver: `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
- View controller: `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
- Grafo canónico: `E:\WebGL_tesis\desarrollo\unity_project\Assets\Resources\ThermalCanonicalContactGraph.asset`
- Builder offline: `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Editor\Thermal\ThermalContactGraphBuilderWindow.cs`
- Harness experimental: `E:\WebGL_tesis\desarrollo\unity_project\Assets\Editor\ThermalTestSetup.cs`

## Pendientes reales

- calibrar el grafo canónico frente a la geometría final retopologizada,
- asignar `ThermalSurfaceProfile` manual donde aporte valor real de showcase,
- validar en Unity Editor que el calor viaje de motor/ESC hacia brazo y plates con la escena final,
- perfilar la vista térmica en hardware WebGL móvil,
- y decidir si se reintegra un slider de carga dedicado en UI o si se mantiene la carga gobernada por estados.

## Regla documental

Toda documentación de este subsistema debe distinguir siempre entre:

- implementado,
- integrado en escena/UI,
- validado,
- experimental,
- pendiente.
## Actualizacion 2026-03-31 - Integracion del dron importado

### Runtime y escena

La escena oficial del X500 V2 ya cuenta con una ruta de preparacion del FBX importado. El root `x500v2_Drone` puede recibir `ImportedDroneRuntimeBinder` para reconectar automaticamente energia, thermal, view modes, explode, visibility, catalogo y cross-section.

### UI vigente

El panel de power y el slider de carga ya existen en el modo Inspect. La leyenda termica ya no depende del gradiente USS: se genera en runtime para evitar fallas de UI Toolkit en WebGL.

### Regla V1 para Fasteners y Misc

`Fasteners` y `Misc` son categorias publicas de visualizacion y filtrado. En la simulacion termica V1 no son nodos independientes: heredan temperatura del ensamblaje canonico padre.

## Actualizacion 2026-04-10 - Refinamiento visual final y nueva granularidad del modelo

### Cierre visual del runtime

- `Thermal.shader` ahora usa un shimmer de baja frecuencia, mas lento y mas suave.
- La perturbacion animada queda subordinada a `baseTempRange`, asi que ya no compite con la temperatura calculada por el solver.
- El edge glow deja de empujar la temperatura mostrada y pasa a ser un acento visual sutil sobre el color final.
- `ThermalViewController` reduce la banda termica visual y la variacion base por defecto para limpiar el ruido sobre piezas secundarias.

### Regla de jerarquia canonica

- El aumento de piezas y subpiezas visuales en Blender no redefine el conjunto oficial de prioridad termica de la V1.
- La simulacion sigue priorizando motores, ESC, bateria, PDB, power module, Pixhawk, brazos y plates.
- La granularidad extra del modelo debe leerse como detalle de visualizacion o mapeo de render, no como nuevas fuentes termicas de primer orden.

