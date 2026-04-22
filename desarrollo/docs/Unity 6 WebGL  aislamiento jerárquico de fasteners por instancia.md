# Unity 6 WebGL: aislamiento jerárquico de fasteners por instancia

## Resumen y modelo mental

El problema se debe a un desalineamiento entre la identidad lógica de la instancia (catálogo JSON, `instanceId`, `parentCanonicalPartId`) y la representación gráfica real en escena (renderers que agrupan varias piezas o fasteners reparentados en runtime).
Cuando se intenta aislar una sola instancia dentro de una pieza madre, la lógica de visibilidad opera sobre el nivel de `Renderer` o `GameObject`, no sobre “subinstancias” dentro de un mismo mesh, por lo que terminan visibles varios fasteners a la vez.
La solución pasa por: (1) hacer explícito un modelo de identidad por instancia desacoplado de la jerarquía visual, (2) detectar cuándo un `Renderer` representa más de una instancia, (3) introducir un stack de aislamiento que opere sobre un conjunto bien definido de proxies de instancia, y (4) adaptar la representación visual (separar meshes, usar colliders sintéticos, detail visuals, o selección por triángulo) sin destruir el rendimiento WebGL.

## Mejores prácticas para aislar una sola instancia

En Unity, la forma más determinista de aislar una instancia es que cada fastener “inspeccionable” tenga su propio `GameObject` con su `Renderer`, aunque comparta `Mesh` y `Material` con otros fasteners del mismo tipo.
Desactivar solo el componente `Renderer` (`renderer.enabled = false`) es una práctica estándar para control visual fino sin afectar scripts, física o jerarquía, mientras `SetActive(false)` se reserva para desactivar lógica completa.[^1]
Para inspección, conviene que el pipeline de importación cree un `GameObject` por instancia de fastener aunque reutilice el mismo `Mesh` asset; esto mantiene el número de draw calls bajo (por batching/instancing) pero permite aislamiento por instancia a nivel de renderer.
Si el importador CAD ha combinado varios fasteners en un solo mesh, se puede corregir en el propio import (ideal) o realizar una pasada de postproceso en editor que duplique el mesh y filtre triángulos por instancia, generando un `Mesh` por fastener sólo para los objetos que necesitan aislamiento fino.

## Detección de clics en renderers multi‑instancia

Si el mesh de fasteners está combinado y se usa un `MeshCollider`, `RaycastHit.triangleIndex` indica el índice del triángulo golpeado.[^2]
Unity documenta que `triangleIndex` solo es válido cuando el collider golpeado es un `MeshCollider` y su `sharedMesh` no es nulo, lo que permite mapear el triángulo concreto a una subinstancia lógica.[^2]
A partir del `triangleIndex`, se recuperan los tres índices de vértices (`mesh.triangles[triangleIndex*3..+2]`) y se usa una tabla precomputada `triangleToInstanceId[]` para asociar cada triángulo a un `instanceId` de fastener.
Esa tabla se puede generar en el importador o en una fase de bake: para cada fastener lógico, se marca en un array qué rangos de triángulos le pertenecen; luego, en runtime, `FastenerRegistry` resuelve `instanceId` desde el triángulo cuando el click cae sobre un renderer combinado.

### Alternativa: selección por buffer de IDs

Un enfoque más genérico es tener una cámara off‑screen que renderiza la escena con un shader unlit donde cada instancia pinta un color único que codifica su ID.
Al hacer clic, se proyecta la coordenada de pantalla y se hace un `ReadPixels` en un render texture 1x1; el color leído se decodifica a `instanceId` lógico.
En WebGL, `ReadPixels` es relativamente costoso, pero si se usa un RT de muy baja resolución y se hace solo al clicar (no cada frame), suele ser aceptable.
Este enfoque desacopla totalmente la selección de la estructura de meshes/colliders y funciona incluso si los fasteners comparten mesh y renderer.

## Mantener identidad por instancia tras reparenting en runtime

La identidad de instancia (catálogo JSON, `FastenerRegistry`) no debe depender de la jerarquía en escena; debe ser un grafo lógico separado con referencias a `Transform`/`Renderer` actuales.
Es conveniente introducir un componente ligero por instancia, por ejemplo `FastenerProxy : MonoBehaviour`, que contenga `instanceId`, `parentCanonicalPartId` y punteros a los renderers/colliders físicos que representan visualmente ese fastener.
Cuando `ImportedDroneRuntimeBinder` reparenta fasteners dentro de la pieza madre, solo se actualiza el `Transform.parent` del `FastenerProxy`, pero el `instanceId` permanece estable y `FastenerRegistry` sigue resolviendo la misma entrada.
`SelectionManager` y `FastenerInspectionManager` nunca deberían operar directamente sobre `Renderer` o `GameObject` crudos; deben pedir siempre al registry la `FastenerInstance` (con metadata + referencias visuales) a partir de un click o de un nodo en la jerarquía.

### Estructura de datos sugerida

- `FastenerInstance`
  - `string instanceId`
  - `string parentCanonicalPartId`
  - `Transform transform`
  - `Renderer[] renderers`
  - `Collider[] colliders`
  - `MeshTriangleRange[] triangleRanges` (si hay renders combinados)
- `FastenerRegistry`
  - `Dictionary<string, FastenerInstance> byInstanceId`
  - `Dictionary<Renderer, List<FastenerInstance>> byRenderer` (para detección de renderers multi‑instancia)
  - `Dictionary<int, string> triangleToInstanceId` (índice de triángulo global → `instanceId`)

`ImportedDroneRuntimeBinder` se convierte en la única capa que conoce cómo mapear los datos de import (paths, canonical IDs) a estos `FastenerInstance`, lo que reduce el acoplamiento con el resto del sistema.

## Patrones de selección y isolation stack jerárquico

Para soportar navegación tipo "profundizar" (assembly → pieza madre → subpieza → fastener), conviene modelar un stack explícito de contextos de selección/aislamiento.
Cada contexto del stack describe un conjunto de instancias visibles y un conjunto de instancias enfocadas (focus) además de el modo de visualización (normal, ghosted, hidden, highlighted).
`PartVisibilityManager` debería aplicar la visibilidad final como composición de todos los contextos del stack, en lugar de mutar `renderer.enabled` ad‑hoc en distintos sistemas.

### Ejemplo de estructura de contexto

```csharp
struct IsolationContext
{
    public string Id; // por ejemplo, "Part:ArmTubeFR" o "Fastener:12345"
    public HashSet<string> VisibleInstanceIds;
    public HashSet<string> FocusInstanceIds;
    public IsolationMode Mode; // Normal, Isolate, GhostOthers, HideOthers
}

class IsolationStack
{
    private readonly Stack<IsolationContext> _stack = new();

    public void Push(IsolationContext ctx) { _stack.Push(ctx); Recompute(); }
    public void Pop() { _stack.Pop(); Recompute(); }

    private void Recompute()
    {
        // Calcula el estado de visibilidad final para cada instanceId
    }
}
```

`PartVisibilityManager` traduce el estado final por `instanceId` a operaciones sobre renderers: asignar materiales ghosted, cambiar layers, o en última instancia togglear `Renderer.enabled` cuando sea necesario.
Este patrón evita que, al aislar primero la pieza madre y después un fastener, el segundo aislamiento "pise" o invalide el contexto del primero, permitiendo profundizar y retroceder en la pila de aislamiento.

## Detectar si un renderer representa varias instancias

Un criterio directo es inspeccionar `FastenerRegistry.byRenderer[renderer]`; si hay más de una `FastenerInstance` asociada al mismo `Renderer`, ese renderer es multi‑instancia.
Además, si se usa la estrategia de `triangleToInstanceId`, basta con comprobar que varios `instanceId` aparecen asociados a triángulos cuyo `MeshRenderer` es el mismo.
En runtime, se puede añadir un modo debug donde, al seleccionar un renderer, se muestre en un panel todos los `instanceId` asociados a él, junto con su `parentCanonicalPartId` y el rango de triángulos.
Eso permite reproducir exactamente el caso problemático donde un click sobre un fastener en modo aislado está realmente golpeando un renderer que contiene otros fasteners hermanos.

### Uso de triangleIndex para debug

La documentación de Unity muestra un ejemplo donde se usa `RaycastHit.triangleIndex` para dibujar los bordes del triángulo golpeado, recuperando vértices desde `mesh.triangles` y `mesh.vertices`.[^2]
Se puede adaptar ese patrón para colorear dinámicamente el triángulo golpeado, o bien para validar si el triángulo mapeado a un `instanceId` coincide realmente con el fastener esperado.
En un build de debug, esto se puede visualizar con `Debug.DrawLine` o cambiando temporalmente el material del submesh correspondiente si se genera un mesh por instancia de forma dinámica.

## Alternativas cuando el renderer agrupa varias instancias

### Proxy roots por instancia

Mantener un `GameObject` vacío por fastener (con su `FastenerProxy` y colliders) aunque el mesh visible esté combinado en un `Renderer` padre.
En aislamiento, se puede ocultar el mesh combinado y, para la instancia seleccionada, instanciar un prefab de "detail visual" que contiene un mesh individual del fastener, posicionado en el anchor de ese proxy.
Esto permite mantener batching para vista general (mesh combinado) y al mismo tiempo tener control fino en modo inspección mediante detail visuals.

### Detail visuals procedurales

Otra variante es generar proceduralmente un mesh pequeño (por ejemplo, un cilindro y cabeza hexagonal) para visualización de detalle a partir de parámetros del catálogo (diámetro, longitud, tipo de cabeza), sin depender del mesh CAD original.
En modo inspección, se apaga el renderer combinado y se enciende el detail visual del fastener seleccionado; al salir del modo, se destruye el detail visual y se vuelve a mostrar el mesh combinado.
Esto reduce la necesidad de almacenar meshes separados por instancia y puede abaratar memoria en WebGL, a costa de un poco de CPU para la generación procedimental.

### Colliders/anchors sintéticos por instancia

Incluso si el mesh visual es combinado, se pueden crear colliders sintéticos simples (cápsulas, cajas) por instancia, como hijos de cada `FastenerProxy`.
La selección se hace raycasteando contra esos colliders simples; una vez identificado el `instanceId`, se decide cómo visualizar la selección: cambiando material del mesh combinado, mostrando un detail visual o aplicando un efecto de outline.
En WebGL es preferible usar colliders primitivos en lugar de `MeshCollider` masivos por rendimiento y consumo de memoria.

### Submeshes y materiales

Si el importer puede generar un submesh por fastener o por grupo pequeño, cada submesh se puede asociar a un material distinto en `Renderer.sharedMaterials`.
Unity no permite desactivar submeshes individuales, pero se puede usar un material que escriba profundidad sin color para los submeshes no seleccionados, o un shader que descarte píxeles según un mask index, de forma que solo el fastener seleccionado quede visible.
Esto requiere extender los shaders de URP o estándar, pero puede mantener el número de draw calls estable si todos los submeshes comparten el mismo shader.

## Recomendaciones específicas para rendimiento en WebGL

En WebGL, la CPU es el cuello de botella habitual: el dispatch de comandos WebGL es más lento que en un build nativo, por lo que se recomienda minimizar draw calls usando batching e instancing.[^3]
Unity documenta que, cuando se usa WebGL, la práctica recomendada es evitar grandes cantidades de draw calls por frame y aprovechar instancing y batching en shaders para mantener datos en GPU.[^3]
Por ello, la arquitectura debe balancear: fasteners que realmente necesitan aislamiento fino (instancias con su propio renderer) frente a fasteners puramente decorativos que pueden ir en meshes combinados sin soporte de aislamiento per‑instance.

### Visibilidad y toggling

Desactivar `Renderer.enabled` es barato y no corta scripts ni física, pero sí sigue ejecutando toda la lógica de Update y física, lo que en WebGL puede penalizar si se hace masivamente.[^4][^1]
Para piezas enteras o grupos grandes que desaparecen en modos de inspección, puede ser más eficiente usar `SetActive(false)` en un parent para evitar overhead de scripts en objetos completamente ocultos.[^1]
Para cambios frecuentes de visibilidad (hover, selección) a nivel de fastener individual, `Renderer.enabled` o cambios de material vía `MaterialPropertyBlock` son preferibles, evitando constantes activaciones/desactivaciones del `GameObject` entero.

### Física y selección

Los `MeshCollider` son más costosos que colliders primitivos, y en WebGL conviene limitar su uso a pocas piezas donde realmente se necesite selección por triángulo.
El resto de fasteners deberían seleccionarse con colliders primitivos aproximados o mediante el buffer de IDs de selección.
También es recomendable ajustar la matriz de colisión (Layer Collision Matrix) para que solo las capas relevantes interactúen, reduciendo el coste de la simulación física, algo especialmente importante en plataformas Web y móviles.[^5]

## Pasos concretos de debugging

1. **Instrumentar la ruta de selección**  
   - En `SelectionManager`, loggear para cada click: `hit.collider`, `hit.triangleIndex` (si aplica), `renderer`, lista de `FastenerInstance` asociados a ese renderer e `instanceId` final resuelto.  
   - Confirmar si en los casos erróneos el renderer tiene múltiples instancias asociadas o si la resolución de triángulo→instancia está fallando.

2. **Vista debug de fasteners por pieza madre**  
   - Construir, vía `FastenerRegistry`, un panel que liste todos los fasteners de la pieza madre actual: `instanceId`, `parentCanonicalPartId`, `GameObject`/renderer, tipo de collider, tiene/ no tiene triangle mapping, etc.  
   - Marcar explícitamente en esa lista cuáles comparten renderer, para ver visualmente los grupos multi‑instancia que pueden romper el aislamiento.

3. **Modo debug de triángulo golpeado**  
   - Activar en builds de desarrollo una opción que, tras un click, coloree el triángulo golpeado usando el patrón de `RaycastHit.triangleIndex` descrito en la documentación de Unity.[^2]
   - Verificar visualmente que el triángulo realmente pertenece al fastener que el usuario cree estar seleccionando; si no, la causa puede ser un mismatch entre anchors, metadata y el mesh combinado.

4. **Verificar coherencia del isolation stack**  
   - Añadir logs cuando se hace push/pop de contextos en el stack de aislamiento: qué conjunto de `instanceId` queda visible y cuál queda oculto.  
   - Forzar pruebas de flujo: aislar pieza madre → aislar fastener → retroceder un nivel; comprobar que la visibilidad resultante coincide con lo esperado y que ningún sistema externo está mutando `renderer.enabled` fuera de `PartVisibilityManager`.

5. **Pruebas específicas WebGL**  
   - Construir un build WebGL de desarrollo con profiler conectado y medir impacto de: (a) número de renderers de fasteners, (b) cantidad de colliders, y (c) coste de cualquier `ReadPixels` o uso de `MeshCollider`.  
   - Ajustar la estrategia de selección/aislamiento (más colliders primitivos vs menos `MeshCollider`, más instancing vs más renderers individuales) en función de los resultados, respetando las recomendaciones oficiales de limitar draw calls y usar técnicas de batching/instancing.[^3]

## Recomendaciones de arquitectura

- Introducir un `FastenerProxy` por instancia que encapsule identidad (metadata) y referencias visuales, desacoplando la lógica de identidad de la jerarquía de renderers.
- Centralizar la visibilidad en un `IsolationStack` + `PartVisibilityManager` que calcule un estado final por `instanceId` y lo traduzca a operaciones sobre renderers, evitando toggles dispersos de `Renderer.enabled`.
- Detectar explícitamente renderers multi‑instancia y, para ellos, optar entre: (a) mapeo triángulo→instancia con `MeshCollider` limitado, (b) buffer de IDs de selección off‑screen, o (c) colliders sintéticos + detail visuals.
- Ajustar el pipeline de importación para que fasteners que se van a inspeccionar tengan `GameObject` propio, compartiendo mesh y material para permitir batching/instancing en WebGL.[^3]
- Separar el modo "vista general" (meshes combinados y pocos colliders) del modo "inspección" (más renderers por instancia, detail visuals, debug) para no pagar el coste completo en todas las vistas.

---

## References

1. [Hide and Unhide Game Objects in Unity: The Developer's Complete ...](https://outscal.com/blog/hide-and-unhide-game-objects-in-unity) - Master how to hide and unhide GameObjects in Unity with this complete guide. Learn the difference be...

2. [Scripting API: RaycastHit.triangleIndex - Unity - Manual](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/RaycastHit-triangleIndex.html) - Triangle index is only valid if the collider that was hit is a MeshCollider. // This script draws a ...

3. [Web performance considerations - Unity - Manual](https://docs.unity3d.com/6000.4/Documentation/Manual/webgl-performance.html) - When using WebGL API for rendering, the CPU side dispatch of WebGL operations is slower than in nati...

4. [WebGL performance considerations - Unity - Manual](https://docs.unity3d.com/es/2018.3/Manual/webgl-performance.html) - WebGL performance considerations. What kind of performance can you expect on WebGL? This is a bit di...

5. [Optimization for web, XR & mobile games in Unity 6 - YouTube](https://www.youtube.com/watch?v=2J0kDtUGlrY) - ... Render Pipeline (URP) and the XR Interaction Toolkit. We'll cover key techniques to identify the...

