# Handoff técnico del sistema térmico

## Propósito

Este handoff deja el estado real del subsistema térmico después de la auditoría de rescate. Su objetivo es que el siguiente agente u operador continúe desde una base honesta, sin repetir pruebas fallidas ni asumir capacidades que todavía no están validadas.

## Resultado de la auditoría

### Se conserva

- `ThermalSimulationManager` como solver térmico reducido por componentes.
- `Thermal.shader` con soporte espacial por pieza.
- `ThermalViewController` como puente solver-render.
- `ThermalContactGraphAsset` como formato serializable.
- `ThermalContactGraphBuilderWindow` como authoring offline.
- `SceneBootstrapper` levantando managers térmicos en runtime.
- `SystemLoadFactor` en `DroneStateController`.
- Leyenda térmica como parte real de la UI.

### Se corrigió

- La documentación ya no afirma que el sistema esté `100% calibrado` o `100% listo`.
- La ruta oficial ya no es el fallback heurístico, sino el `ThermalCanonicalContactGraph.asset` cargado por `Resources`.
- `ThermalViewController` ahora cachea la leyenda UI y deja de buscar el `UIDocument` en cada refresh.
- La propagación visual base ya no depende solo de heurísticas genéricas; usa presets canónicos por pieza crítica.
- Se documentó que la carga térmica hoy depende del estado del dron y de `SystemLoadFactor`, no de un slider dedicado ya visible al usuario en esta rama.

### Se aisló como experimental

- `Assets/Editor/ThermalTestSetup.cs`

Este archivo se conserva para pruebas de estrés con CAD crudo, pero queda fuera del camino oficial. Usa nombres no canónicos y `DronePartData` dummy, por lo que no debe citarse como solución final ni como prueba válida del sistema térmico.

## Estado operativo actual

### Runtime oficial

El runtime oficial ya puede:

- construir nodos térmicos desde `ExplodablePart + DronePartData`,
- cargar un grafo canónico explícito desde `Assets/Resources/ThermalCanonicalContactGraph.asset`,
- aplicar temperaturas al shader,
- mostrar leyenda térmica en UI,
- y usar presets térmicos canónicos para motores, ESC, batería, brazos, plates y stack central.

### Lo que todavía falta para cerrar la etapa restante

1. Validar el comportamiento sobre la escena final retopologizada del X500 V2.
2. Asignar `ThermalSurfaceProfile` manuales solo donde el override aporte valor real frente al preset canónico.
3. Perfilar y medir el rendimiento de la vista térmica en Unity Editor y build objetivo.

## Decisión de modelado vigente

- Solver oficial: 28 piezas canónicas de `x500v2_parts_data.json`.
- Modelado e importación: 55 subcomponentes CAD/Blender documentados como capa de detalle.
- El solver V1 no migra a FEA ni a resolución térmica por malla completa.

## Qué no debe volver a hacerse

- No usar CAD bruto con nombres no canónicos como camino principal.
- No vender la V1 como termografía validada experimentalmente.
- No describir el sistema como FEA completo o CFD.
- No mezclar piezas minúsculas del CAD con nodos térmicos oficiales sin un criterio de costo/beneficio claro.

## Archivos fuente de verdad

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\RETOPOLOGIA_POR_PIEZA.md`
- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\MATRIZ_ACTUALIZACION_DOCUMENTAL.md`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Resources\ThermalCanonicalContactGraph.asset`

## Siguiente paso recomendado

Importar o conectar la geometría retopologizada final, comprobar que cada pieza crítica mantenga su `partId` canónico o un mapeo claro, y validar visualmente la propagación desde motor/ESC hacia brazos y plates con la vista térmica activa.