# Documento 2 — Referencia Técnica de Módulos

## Aplicación Web Interactiva para Visualización 3D de Drones

### Documentación Exhaustiva Clase por Clase

**Proyecto de Grado:** Ingeniería Multimedia — UNAD  
**Plataforma:** Unity 6.0 LTS → WebGL  
**Rama:** `feature/phase2-ux-redesign`

---

## Tabla de Contenidos

1. [Capa de Infraestructura](#1-capa-de-infraestructura)
2. [Capa de Datos (ScriptableObjects)](#2-capa-de-datos-scriptableobjects)
3. [Capa de Contenido (MonoBehaviours de Escena)](#3-capa-de-contenido-monobehaviours-de-escena)
4. [Capa Core — Máquina de Estados y Eventos](#4-capa-core--máquina-de-estados-y-eventos)
5. [Capa Core — Managers Primarios](#5-capa-core--managers-primarios)
6. [Capa Core — Managers de Herramientas](#6-capa-core--managers-de-herramientas)
7. [Capa Core — Managers de Soporte](#7-capa-core--managers-de-soporte)
8. [Capa Core — Utilidades](#8-capa-core--utilidades)
9. [Capa UI — Orquestación](#9-capa-ui--orquestación)
10. [Capa UI — Sub-Controladores](#10-capa-ui--sub-controladores)
11. [Capa UI — Paneles Secundarios](#11-capa-ui--paneles-secundarios)
12. [Capa UI — Sistemas Transversales](#12-capa-ui--sistemas-transversales)
13. [Archivos Deprecados](#13-archivos-deprecados)

---

## Convenciones de este Documento

- **Visibilidad:** `+` público, `#` protegido, `-` privado
- **Tipo Singleton:** `[S]` = Singleton, `[PS]` = PersistentSingleton, `[—]` = sin singleton
- **Lifecycle hooks:** Se documenta qué métodos de Unity (Awake, Start, Update, OnDestroy, etc.) implementa cada clase.
- **Eventos publicados:** Se indica explícitamente qué eventos emite cada clase y quiénes los consumen.

---

## 1. Capa de Infraestructura

### 1.1 `Singleton.cs` — Jerarquía Genérica de Singletons

**Ruta:** `Assets/Scripts/Core/Singleton.cs` | **Líneas:** 58  
**Namespace:** `WebGL.Core`

#### Clase `StaticInstance<T> : MonoBehaviour where T : MonoBehaviour`

| Miembro                 | Tipo                             | Descripción                                                                              |
| ----------------------- | -------------------------------- | ---------------------------------------------------------------------------------------- |
| `+ Instance`            | `static T { get; private set; }` | Referencia estática a la instancia actual                                                |
| `# Awake()`             | `virtual void`                   | Asigna `this as T` a `Instance`. Si existe otra, **reemplaza** sin destruir la anterior. |
| `# OnApplicationQuit()` | `virtual void`                   | Establece `Instance = null` y `Destroy(gameObject)` para limpieza segura.                |

**Justificación del `as T`:** El cast con `as` en lugar de `(T)` evita `InvalidCastException` si la herencia múltiple genérica produce conflictos; retorna `null` silenciosamente.

#### Clase `Singleton<T> : StaticInstance<T>`

| Miembro     | Tipo            | Descripción                                                                                                           |
| ----------- | --------------- | --------------------------------------------------------------------------------------------------------------------- |
| `# Awake()` | `override void` | Si `Instance != null && Instance != this`, llama `Destroy(gameObject)` y retorna. De otro modo, llama `base.Awake()`. |

**Comportamiento clave:** Garantiza que solo exista una instancia. El segundo GameObject se auto-destruye. Esto es crucial cuando Unity duplica objetos accidentalmente (e.g., cambio de escena con `DontDestroyOnLoad`).

#### Clase `PersistentSingleton<T> : Singleton<T>`

| Miembro     | Tipo            | Descripción                                                                                                                             |
| ----------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `# Awake()` | `override void` | Llama `base.Awake()` (validación de unicidad). Si `Instance == this` (sobrevivió la validación), marca `DontDestroyOnLoad(gameObject)`. |

**Patrón Factory Method implícito:** La cadena `PersistentSingleton.Awake()` → `Singleton.Awake()` → `StaticInstance.Awake()` constituye un Template Method donde cada nivel añade comportamiento sin repetir código.

---

### 1.2 `EventBus.cs` — Sistema de Publicación/Suscripción Tipado

**Ruta:** `Assets/Scripts/Core/EventBus.cs` | **Líneas:** 170  
**Namespace:** `WebGL.Core`

#### Clase `EventBus` (static)

| Miembro          | Tipo                                      | Descripción                                           |
| ---------------- | ----------------------------------------- | ----------------------------------------------------- |
| `- _subscribers` | `static Dictionary<Type, List<Delegate>>` | Mapa de tipo de evento → lista de callbacks suscritos |
| `- _lock`        | `static readonly object`                  | Mutex para acceso thread-safe al diccionario          |

| Método                    | Firma                         | Descripción                                                                                   |
| ------------------------- | ----------------------------- | --------------------------------------------------------------------------------------------- |
| `+ Subscribe<T>`          | `(Action<T> callback) : void` | Agrega callback. Verifica no-duplicados via `Contains()`. Crea la lista si no existe el tipo. |
| `+ Unsubscribe<T>`        | `(Action<T> callback) : void` | Remueve callback. Si la lista queda vacía, elimina la entrada del diccionario.                |
| `+ Publish<T>`            | `(T eventData) : void`        | Copia la lista (snapshot), itera sobre la copia invocando cada callback en try/catch.         |
| `+ Clear`                 | `() : void`                   | Limpia todo el diccionario.                                                                   |
| `+ ClearEvent<T>`         | `() : void`                   | Limpia solo los suscriptores del tipo `T`.                                                    |
| `+ HasSubscribers<T>`     | `() : bool`                   | `true` si hay al menos un suscriptor para `T`.                                                |
| `+ GetSubscriberCount<T>` | `() : int`                    | Cantidad de suscriptores para `T`.                                                            |

**Decisión de diseño — ¿Por qué `Delegate` y no `Action<T>` en el diccionario?** El diccionario necesita un tipo de valor único para almacenar callbacks de diferentes tipos genéricos `T`. `Delegate` es la clase base común. El cast `(Action<T>)subscriber` se realiza al momento de la invocación, manteniendo type-safety en runtime.

**Decisión de diseño — ¿Por qué snapshot antes de iterar?** Si un suscriptor se desuscribe (o suscribe otro) durante la iteración de `Publish`, modificar la lista mientras se itera lanzaría `InvalidOperationException`. La copia defensiva `new List<Delegate>(subs)` previene esto a costa de una allocación de lista por publicación (aceptable dado que los eventos son esporádicos, no per-frame).

---

### 1.3 `TweenEngine.cs` — Motor de Animación Procedural

**Ruta:** `Assets/Scripts/Core/Utils/TweenEngine.cs` | **Líneas:** 154  
**Namespace:** `WebGL.Core.Utils`

#### Clase `TweenEngine : PersistentSingleton<TweenEngine>` [PS]

| Miembro         | Tipo                                                                                                            | Descripción                  |
| --------------- | --------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| `enum EaseType` | `{ Linear, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseOutElastic }` | 8 funciones de interpolación |

| Método                 | Firma                                                                                                                | Descripción                                                                                          |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| `+ TweenFloat`         | `(float from, float to, float duration, Action<float> onUpdate, EaseType ease, Action onComplete) : Coroutine`       | Interpola un float de `from` a `to` durante `duration` segundos. Llama `onUpdate(value)` cada frame. |
| `+ TweenVector3`       | `(Vector3 from, Vector3 to, float duration, Action<Vector3> onUpdate, EaseType ease, Action onComplete) : Coroutine` | Mismo patrón para Vector3.                                                                           |
| `+ TweenPosition`      | `(Transform target, Vector3 to, float duration, EaseType ease, Action onComplete) : Coroutine`                       | Mueve `transform.position` hacia `to`.                                                               |
| `+ TweenLocalPosition` | `(Transform target, Vector3 to, float duration, EaseType ease, Action onComplete) : Coroutine`                       | Mueve `transform.localPosition`.                                                                     |
| `+ TweenScale`         | `(Transform target, Vector3 to, float duration, EaseType ease, Action onComplete) : Coroutine`                       | Escala `transform.localScale`.                                                                       |
| `+ TweenColor`         | `(Color from, Color to, float duration, Action<Color> onUpdate, EaseType ease, Action onComplete) : Coroutine`       | Interpola Color (RGBA).                                                                              |
| `- Evaluate`           | `(float t, EaseType ease) : float`                                                                                   | Switch con fórmulas matemáticas para cada EaseType.                                                  |

**Fórmulas de easing implementadas:**

| EaseType         | Fórmula                                      |
| ---------------- | -------------------------------------------- |
| `Linear`         | `t`                                          |
| `EaseInQuad`     | `t²`                                         |
| `EaseOutQuad`    | `t(2 - t)`                                   |
| `EaseInOutQuad`  | `t < 0.5 ? 2t² : -1 + (4 - 2t)t`             |
| `EaseInCubic`    | `t³`                                         |
| `EaseOutCubic`   | `(t-1)³ + 1`                                 |
| `EaseInOutCubic` | `t < 0.5 ? 4t³ : (t-1)(2t-2)² + 1`           |
| `EaseOutElastic` | `2^(-10t) · sin((t - 0.075) · 2π / 0.3) + 1` |

**Justificación de corrutinas:** En WebGL (single-threaded), la única forma de ejecutar animaciones frame-by-frame es mediante Coroutines de Unity (`IEnumerator` + `yield return null`). `async/await` NO está soportado completamente en WebGL porque `Task.Delay` requiere threading.

---

### 1.4 `ObjectPooler.cs` — Pool de Objetos Genérico

**Ruta:** `Assets/Scripts/Core/Utils/ObjectPooler.cs` | **Líneas:** 62  
**Namespace:** `WebGL.Core.Utils`

#### Clase `ObjectPooler : PersistentSingleton<ObjectPooler>` [PS]

| Miembro            | Tipo                                    | Descripción                                  |
| ------------------ | --------------------------------------- | -------------------------------------------- |
| `+ pools`          | `List<Pool>` (Serialized)               | Configuración de pools (tag, prefab, tamaño) |
| `- poolDictionary` | `Dictionary<string, Queue<GameObject>>` | Mapa runtime: tag → cola de instancias       |

| Método              | Firma                                                    | Descripción                                                                                                                  |
| ------------------- | -------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| `+ InitializePools` | `() : void`                                              | Para cada Pool: instancia `size` copias del prefab, desactivadas, bajo un GameObject padre `Pool_<tag>`. Enqueue en la cola. |
| `+ SpawnFromPool`   | `(string tag, Vector3 pos, Quaternion rot) : GameObject` | Dequeue → SetActive(true) → posicionar → re-enqueue (reciclaje circular).                                                    |

**Patrón round-robin:** La re-inserción inmediata al final de la cola (`Enqueue` después de `Dequeue`) crea un ciclo circular. Si todos los objetos están en uso, el más antiguo se recicla (se reposiciona sin destruir/crear).

---

### 1.5 `SceneBootstrapper.cs` — Inicializador de Escena

**Ruta:** `Assets/Scripts/Core/Utils/SceneBootstrapper.cs` | **Líneas:** 174  
**Namespace:** `WebGL.Core.Utils`

#### Clase `SceneBootstrapper : MonoBehaviour` [—]

**Atributo:** `[DefaultExecutionOrder(-100)]`

| Método                                      | Firma                          | Descripción                                                                                         |
| ------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------- |
| `- Awake`                                   | `void`                         | Llama `ValidateManagers()` + `LogSystemStatus()`.                                                   |
| `- ValidateManagers`                        | `void`                         | Llama `EnsureManager<T>()` para 18 tipos de manager en orden.                                       |
| `- EnsureManager<T>`                        | `void where T : MonoBehaviour` | `FindAnyObjectByType<T>()`. Si null, crea GameObject `[ClassName]` con componente `T`. Log warning. |
| `- LogSystemStatus`                         | `void`                         | Cuenta managers activos con `FindObjectsByType<MonoBehaviour>` y registra en log.                   |
| `[ContextMenu] ValidateSceneSetup`          | `void`                         | Mismo que `ValidateManagers` pero invocable desde el Inspector (editor only).                       |
| `[ContextMenu] CreateDefaultManagersObject` | `void`                         | Crea un único GameObject "Managers" con los 18 componentes. Alternativa a creación individual.      |

---

## 2. Capa de Datos (ScriptableObjects)

### 2.1 `DronePartData.cs` — Datos de Parte del Dron

**Ruta:** `Assets/Scripts/Core/Data/DronePartData.cs` | **Líneas:** 116  
**Namespace:** `WebGL.Core.Data`

#### Clase `DronePartData : ScriptableObject`

ScriptableObject con 8 grupos de propiedades organizadas por `[Header]`:

**General Info:**

| Campo           | Tipo                | Descripción                                                    |
| --------------- | ------------------- | -------------------------------------------------------------- |
| `+ id`          | `string`            | Identificador único de la parte                                |
| `+ partName`    | `string`            | Nombre legible (e.g., "Motor Brushless A2212")                 |
| `+ category`    | `string`            | Categoría (Structure, Propulsion, Electronics, Sensors, Other) |
| `+ partType`    | `string`            | Tipo específico (e.g., "BLDC Motor")                           |
| `+ description` | `string [TextArea]` | Descripción larga de la parte                                  |

**Technical Specs:**

| Campo                | Tipo      | Descripción                           |
| -------------------- | --------- | ------------------------------------- |
| `+ weight`           | `float`   | Peso en gramos                        |
| `+ dimensions`       | `Vector3` | Dimensiones L×W×H en milímetros       |
| `+ powerConsumption` | `float`   | Consumo en watts                      |
| `+ operatingTemp`    | `Vector2` | Rango de temperatura (min, max) en °C |
| `+ voltage`          | `float`   | Voltaje nominal                       |
| `+ current`          | `float`   | Corriente nominal en amperios         |

**Materials:**

| Campo                  | Tipo     | Descripción                                      |
| ---------------------- | -------- | ------------------------------------------------ |
| `+ materialType`       | `string` | Material principal (e.g., "Carbon Fiber", "ABS") |
| `+ materialProperties` | `string` | Propiedades (e.g., "UV resistant, lightweight")  |

**Performance:**

| Campo           | Tipo                   | Descripción                             |
| --------------- | ---------------------- | --------------------------------------- |
| `+ efficiency`  | `float [Range(0,100)]` | Eficiencia porcentual                   |
| `+ reliability` | `float [Range(0,5)]`   | Rating de confiabilidad (0-5 estrellas) |
| `+ lifespan`    | `string`               | Vida útil estimada                      |

**Visuals:**

| Campo         | Tipo     | Descripción                      |
| ------------- | -------- | -------------------------------- |
| `+ partColor` | `Color`  | Color representativo de la parte |
| `+ icon`      | `Sprite` | Icono 2D para catálogo/UI        |

**Exploded View:**

| Campo                  | Tipo      | Descripción                                       |
| ---------------------- | --------- | ------------------------------------------------- |
| `+ explosionDirection` | `Vector3` | Dirección de desplazamiento en vista explosionada |
| `+ explosionDistance`  | `float`   | Distancia máxima de explosión                     |
| `+ explosionOrder`     | `int`     | Orden de animación (menor = primero)              |

**Modular:**

| Campo               | Tipo       | Descripción                          |
| ------------------- | ---------- | ------------------------------------ |
| `+ isModular`       | `bool`     | ¿Puede intercambiarse?               |
| `+ compatibleTypes` | `string[]` | Lista de tipos compatibles para swap |

**Assembly Info:**

| Campo                   | Tipo               | Descripción                              |
| ----------------------- | ------------------ | ---------------------------------------- |
| `+ assemblyOrder`       | `int`              | Orden de ensamblaje                      |
| `+ assemblyDifficulty`  | `int [Range(1,5)]` | Nivel de dificultad (1=fácil, 5=experto) |
| `+ requiredTools`       | `string`           | Herramientas necesarias                  |
| `+ installationTime`    | `float`            | Tiempo estimado en minutos               |
| `+ screwCount`          | `int`              | Cantidad de tornillos para fijar         |
| `+ functionDescription` | `string`           | Función en el ensamblaje                 |

**Connections:**

| Campo                | Tipo       | Descripción                                   |
| -------------------- | ---------- | --------------------------------------------- |
| `+ connectedPartIds` | `string[]` | IDs de partes conectadas                      |
| `+ connectionTypes`  | `string[]` | Tipos de conexión (screw, snap, solder, wire) |

| Método                 | Firma         | Descripción                                                                                                  |
| ---------------------- | ------------- | ------------------------------------------------------------------------------------------------------------ |
| `+ GetFullDescription` | `() : string` | Genera texto formateado multi-línea: nombre, categoría, peso, dimensiones, material, eficiencia, rating (★). |
| `+ GetAssemblyInfo`    | `() : string` | Genera texto de ensamblaje: orden, dificultad, herramientas, tornillos, tiempo.                              |

---

### 2.2 `AssemblyGuideData.cs` — Datos de Guía de Ensamblaje

**Ruta:** `Assets/Scripts/Core/Data/AssemblyGuideData.cs` | **Líneas:** 86  
**Namespace:** `WebGL.Core.Data`

#### Clase `AssemblyGuideData : ScriptableObject`

| Campo                  | Tipo                | Descripción                                           |
| ---------------------- | ------------------- | ----------------------------------------------------- |
| `+ guideName`          | `string`            | Nombre de la guía (e.g., "Basic Quadcopter Assembly") |
| `+ description`        | `string [TextArea]` | Descripción general                                   |
| `+ steps`              | `AssemblyStep[]`    | Array de pasos secuenciales                           |
| `+ totalEstimatedTime` | `float`             | Tiempo total auto-calculado (readonly en inspector)   |
| `+ averageDifficulty`  | `float`             | Dificultad promedio auto-calculada                    |
| `+ allRequiredTools`   | `string`            | Lista unificada de herramientas                       |

| Método         | Firma  | Descripción                                                                                                                                                                                        |
| -------------- | ------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `- OnValidate` | `void` | Recalcula `totalEstimatedTime` (suma de tiempos), `averageDifficulty` (promedio), y `allRequiredTools` (unión sin duplicados con `HashSet`). Se ejecuta automáticamente al modificar en Inspector. |

#### Clase `AssemblyStep` [Serializable]

| Campo              | Tipo                | Descripción                  |
| ------------------ | ------------------- | ---------------------------- |
| `+ stepNumber`     | `int`               | Número de paso               |
| `+ title`          | `string`            | Título del paso              |
| `+ instructions`   | `string [TextArea]` | Instrucciones detalladas     |
| `+ requiredTools`  | `string`            | Herramientas para este paso  |
| `+ estimatedTime`  | `float`             | Tiempo en minutos            |
| `+ difficulty`     | `int [Range(1,5)]`  | Dificultad del paso          |
| `+ targetPartId`   | `string`            | ID de la parte involucrada   |
| `+ cameraAngle`    | `Vector3`           | Ángulo de cámara sugerido    |
| `+ cameraDistance` | `float`             | Distancia de cámara sugerida |
| `+ warningMessage` | `string`            | Advertencia de seguridad     |
| `+ tipMessage`     | `string`            | Consejo útil                 |

---

## 3. Capa de Contenido (MonoBehaviours de Escena)

### 3.1 `ExplodablePart.cs` — Parte Explosionable del Dron

**Ruta:** `Assets/Scripts/Content/ExplodablePart.cs` | **Líneas:** 57  
**Namespace:** `WebGL.Content`

#### Clase `ExplodablePart : MonoBehaviour` [—]

| Campo                | Tipo                         | Descripción                                          |
| -------------------- | ---------------------------- | ---------------------------------------------------- |
| `+ Data`             | `DronePartData` (Serialized) | Referencia al ScriptableObject con datos de la parte |
| `- originalLocalPos` | `Vector3`                    | Posición local almacenada en `Initialize()`          |
| `- targetPosition`   | `Vector3`                    | Posición local destino cuando explosionFactor=1      |
| `- materialCtrl`     | `MaterialController`         | Referencia cacheada                                  |

| Método              | Firma                   | Descripción                                                                                                                                                                                                                         |
| ------------------- | ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ Initialize`      | `() : void`             | Almacena `transform.localPosition` como `originalLocalPos`. Calcula `targetPosition = originalLocalPos + Data.explosionDirection * Data.explosionDistance`. Cachea `MaterialController`. Llamado por `ExplodedViewManager.Start()`. |
| `+ UpdateExplosion` | `(float factor) : void` | `transform.localPosition = Vector3.Lerp(originalLocalPos, targetPosition, factor)`. Interpolación lineal controlada por el slider de explosión.                                                                                     |
| `+ SetXRay`         | `(bool enabled) : void` | Delega a `materialCtrl.ToggleXRay(enabled)`.                                                                                                                                                                                        |

**Diseño por composición:** `ExplodablePart` no contiene lógica de renderizado ni de UI. Solo almacena datos espaciales y delega a `MaterialController` para cambios visuales. Esto permite reusar el mismo componente en cualquier parte del dron.

---

### 3.2 `HighlightSystem.cs` — Sistema de Resaltado Visual

**Ruta:** `Assets/Scripts/Content/HighlightSystem.cs` | **Líneas:** 121  
**Namespace:** `WebGL.Content`  
**Atributo:** `[RequireComponent(typeof(Renderer))]`

#### Clase `HighlightSystem : MonoBehaviour` [—]

| Campo                     | Tipo                    | Descripción                                                           |
| ------------------------- | ----------------------- | --------------------------------------------------------------------- |
| `+ hoverColor`            | `Color`                 | Color de tinte al pasar el cursor (default: amarillo semitranslúcido) |
| `+ selectedEmissionColor` | `Color`                 | Color de emisión al seleccionar (default: cyan)                       |
| `+ pulseSpeed`            | `float`                 | Velocidad de pulsación (Hz)                                           |
| `+ pulseIntensity`        | `float`                 | Intensidad máxima de la emisión pulsante                              |
| `- _renderer`             | `Renderer`              | Referencia cacheada en Awake                                          |
| `- _propBlock`            | `MaterialPropertyBlock` | Instancia reutilizada para evitar allocaciones                        |
| `- _originalColor`        | `Color`                 | Color `_BaseColor` original del material                              |
| `- _isHovered`            | `bool`                  | Estado hover actual                                                   |
| `- _isSelected`           | `bool`                  | Estado de selección actual                                            |
| `- _pulseCoroutine`       | `Coroutine`             | Referencia a la corrutina activa para poder detenerla                 |

| Método           | Firma              | Descripción                                                                                                                                                                                                                             |
| ---------------- | ------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ OnHoverEnter` | `() : void`        | Establece `_isHovered = true`. Aplica `hoverColor` como override de `_BaseColor` via `MaterialPropertyBlock`.                                                                                                                           |
| `+ OnHoverExit`  | `() : void`        | Restaura `_originalColor` si no está seleccionado.                                                                                                                                                                                      |
| `+ OnSelect`     | `() : void`        | Establece `_isSelected = true`. Inicia `PulseRoutine()` como corrutina.                                                                                                                                                                 |
| `+ OnDeselect`   | `() : void`        | Establece `_isSelected = false`. Detiene `PulseRoutine()`. Limpia emisión (negro) y restaura color.                                                                                                                                     |
| `- PulseRoutine` | `() : IEnumerator` | Bucle infinito: calcula `emission = selectedEmissionColor * (sin(Time.time * pulseSpeed) * 0.5 + 0.5) * pulseIntensity`. Aplica como `_EmissionColor` y `_BaseColor` mixto via `MaterialPropertyBlock`. `yield return null` cada frame. |

**Decisión de diseño — `MaterialPropertyBlock` sobre material instance:** Usar `renderer.SetPropertyBlock(propBlock)` permite override per-renderer sin clonar el material. En una escena con 50+ partes donde hover y select ocurren constantemente, esto evita crear y destruir 50+ instancias de material.

**Decisión de diseño — Corrutina para pulso:** La pulsación es una animación continua que debe correr mientras la parte esté seleccionada. Una corrutina con `while(true) { ... yield return null }` es el patrón más eficiente en Unity para animaciones de estado indefinido (vs. Update, que ejecutaría la lógica incluso sin selección).

---

### 3.3 `MaterialController.cs` — Control de Material Per-Parte

**Ruta:** `Assets/Scripts/Content/MaterialController.cs` | **Líneas:** 61  
**Namespace:** `WebGL.Content`  
**Atributo:** `[RequireComponent(typeof(Renderer))]`

#### Clase `MaterialController : MonoBehaviour` [—]

| Campo                 | Tipo                    | Descripción                             |
| --------------------- | ----------------------- | --------------------------------------- |
| `- _renderer`         | `Renderer`              | Cacheado en Awake                       |
| `- _propBlock`        | `MaterialPropertyBlock` | Instancia reutilizable                  |
| `- _originalMaterial` | `Material`              | Material original antes del swap a XRay |
| `+ xrayMaterial`      | `Material` (Serialized) | Material XRay asignado en Inspector     |
| `- _isXRay`           | `bool`                  | Estado actual                           |

| Método              | Firma                   | Descripción                                                                                                                                                |
| ------------------- | ----------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ SetColor`        | `(Color color) : void`  | `_propBlock.SetColor("_BaseColor", color)` → `_renderer.SetPropertyBlock(_propBlock)`. Override no destructivo del color base.                             |
| `+ ResetProperties` | `() : void`             | `_propBlock.Clear()` → `_renderer.SetPropertyBlock(_propBlock)`. Limpia todos los overrides.                                                               |
| `+ ToggleXRay`      | `(bool enabled) : void` | Si `enabled` y no ya en XRay: almacena `_originalMaterial = sharedMaterial`, asigna `xrayMaterial`. Si `!enabled` y en XRay: restaura `_originalMaterial`. |

**Nota sobre `sharedMaterial` vs `material`:** Este componente usa `renderer.sharedMaterial` para leer y `renderer.sharedMaterial = ...` para asignar. Esto modifica la referencia de material SIN crear una instancia. Todos los renderers que compartían el material original seguirán compartiendo el nuevo. Para override individual, se usa `MaterialPropertyBlock`.

---

## 4. Capa Core — Máquina de Estados y Eventos

### 4.1 `AppStateMachine.cs` — Máquina de Estados Finita

**Ruta:** `Assets/Scripts/Core/AppStateMachine.cs` | **Líneas:** 243  
**Namespace:** `WebGL.Core`

#### Enum `AppState`

```
Loading(0), Intro(1), Exploration(2), ExplodedView(3), FocusMode(4),
Settings(5), Menu(6), Analyze(7), Studio(8)
```

#### Clase `AppStateMachine : Singleton<AppStateMachine>` [S]

| Campo                 | Tipo                               | Descripción                                |
| --------------------- | ---------------------------------- | ------------------------------------------ |
| `+ CurrentState`      | `AppState { get; private set; }`   | Estado actual (initial: Loading)           |
| `+ OnStateChanged`    | `event Action<AppState>`           | Evento C# directo                          |
| `- _validTransitions` | `Dictionary<AppState, AppState[]>` | Tabla de transiciones permitidas           |
| `+ debugMode`         | `bool`                             | Habilita logging detallado de transiciones |

| Método                | Firma                        | Descripción                                                                                                                                                |
| --------------------- | ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ ChangeState`       | `(AppState newState) : bool` | Valida transición → actualiza CurrentState → publica 3 notificaciones (OnStateChanged, StateChangedEvent, AppStateChangedEvent). Retorna `true` si cambió. |
| `+ CanTransitionTo`   | `(AppState target) : bool`   | Consulta `_validTransitions[CurrentState].Contains(target)`.                                                                                               |
| `+ IsInteractive`     | `() : bool`                  | `true` si CurrentState ∈ {Exploration, ExplodedView, FocusMode, Analyze, Studio}.                                                                          |
| `+ EnterExploration`  | `() : void`                  | Wrapper para `ChangeState(Exploration)`.                                                                                                                   |
| `+ EnterAnalyze`      | `() : void`                  | Wrapper para `ChangeState(Analyze)`.                                                                                                                       |
| `+ EnterStudio`       | `() : void`                  | Wrapper para `ChangeState(Studio)`.                                                                                                                        |
| `+ EnterFocusMode`    | `() : void`                  | Wrapper para `ChangeState(FocusMode)`.                                                                                                                     |
| `+ EnterExplodedView` | `() : void`                  | Wrapper para `ChangeState(ExplodedView)`.                                                                                                                  |
| `+ OpenSettings`      | `() : void`                  | Wrapper para `ChangeState(Settings)`.                                                                                                                      |
| `+ OpenMenu`          | `() : void`                  | Wrapper para `ChangeState(Menu)`.                                                                                                                          |
| `+ ForceState`        | `(AppState state) : void`    | Cambia estado SIN validar transición. Solo para debugging/reset.                                                                                           |

---

### 4.2 `CoreEvents.cs` — DTOs de Eventos del Dominio

**Ruta:** `Assets/Scripts/Core/Events/CoreEvents.cs` | **Líneas:** 29  
**Namespace:** `WebGL.Core.Events`

| Evento                 | Campos                                   | Constructor                                      | Uso                                                   |
| ---------------------- | ---------------------------------------- | ------------------------------------------------ | ----------------------------------------------------- |
| `PartSelectedEvent`    | `DronePartData Data`, `bool FromHotspot` | `(DronePartData data, bool fromHotspot = false)` | Emitido al seleccionar una parte (click 3D o hotspot) |
| `AppStateChangedEvent` | `AppState NewState`                      | `(AppState newState)`                            | Emitido al cambiar estado de la FSM                   |
| `ViewModeChangedEvent` | `bool IsExploded`                        | `(bool isExploded)`                              | Emitido al entrar/salir de vista explosionada         |

### 4.3 `StateChangedEvent.cs`

**Ruta:** `Assets/Scripts/Core/Events/StateChangedEvent.cs` | **Líneas:** 17

| Campo             | Tipo       | Descripción     |
| ----------------- | ---------- | --------------- |
| `+ PreviousState` | `AppState` | Estado anterior |
| `+ NewState`      | `AppState` | Estado nuevo    |

---

## 5. Capa Core — Managers Primarios

### 5.1 `GameManager.cs` — Coordinador Global

**Ruta:** `Assets/Scripts/Core/Managers/GameManager.cs` | **Líneas:** 68  
**Namespace:** `WebGL.Core.Managers`

#### Clase `GameManager : PersistentSingleton<GameManager>` [PS]

| Campo         | Tipo   | Descripción                                               |
| ------------- | ------ | --------------------------------------------------------- |
| `+ debugMode` | `bool` | Flag maestro para logging detallado en todos los sistemas |

| Método       | Firma                     | Descripción                                            |
| ------------ | ------------------------- | ------------------------------------------------------ |
| `+ DebugLog` | `(string message) : void` | `if (debugMode) Debug.Log(...)` — wrapper centralizado |

**Rol actual:** Shell minimalista. En fases futuras, coordinará la carga de diferentes modelos 3D (Robot, Satellite) y la persistencia de sesión.

---

### 5.2 `SelectionManager.cs` — Selección por Raycast

**Ruta:** `Assets/Scripts/Core/Managers/SelectionManager.cs` | **Líneas:** 308  
**Namespace:** `WebGL.Core.Managers`

#### Clase `SelectionManager : Singleton<SelectionManager>` [S]

| Campo                   | Tipo                           | Descripción                              |
| ----------------------- | ------------------------------ | ---------------------------------------- |
| `+ selectableLayerMask` | `LayerMask` (Serialized)       | Capas que detecta el raycast             |
| `+ maxRaycastDistance`  | `float`                        | Distancia máxima del rayo (default: 100) |
| `- _currentHovered`     | `ExplodablePart`               | Parte bajo el cursor actualmente         |
| `- _currentSelected`    | `ExplodablePart`               | Parte seleccionada actualmente           |
| `- _mainCamera`         | `Camera`                       | Referencia cacheada                      |
| `+ OnPartHovered`       | `event Action<ExplodablePart>` | Evento local de hover                    |
| `+ OnPartSelected`      | `event Action<ExplodablePart>` | Evento local de selección                |

**Lifecycle:** `Start` → cachea `Camera.main`.  
**Update loop:**

```
Update()
├── if (InputManager.InputBlocked) → return
├── HandleHover()
│   ├── Physics.Raycast(camera.ScreenPointToRay(mousePos), layerMask)
│   ├── Hit? → GetComponent<ExplodablePart>()
│   │   ├── Cambió hoveredPart?
│   │   │   ├── old._highlight.OnHoverExit()
│   │   │   └── new._highlight.OnHoverEnter()
│   │   └── CursorManager.SetCursor(Pointer)
│   └── No hit? → CursorManager.SetCursor(Default)
│
└── HandleClick()  [if GetMouseButtonDown(0)]
    ├── InputManager.IsPointerOverUI()? → return
    ├── Physics.Raycast(...)
    │   ├── Hit ExplodablePart?
    │   │   ├── SWAP: deselect old (sin flicker) → select new
    │   │   └── EventBus.Publish(new PartSelectedEvent(data))
    │   └── No hit → DeselectCurrentPart()
    │       └── EventBus.Publish(new PartSelectedEvent(null))
    └── return
```

| Método                  | Firma                                         | Descripción                                                                                                               |
| ----------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| `+ SelectObject`        | `(Transform target, bool fromHotspot) : void` | Selección programática (desde hotspot o catálogo). Busca `ExplodablePart` en el target. Ejecuta swap-selection + publish. |
| `+ DeselectCurrentPart` | `() : void`                                   | Deselecciona la parte actual: `_highlight.OnDeselect()`, `_currentSelected = null`, publish null event.                   |
| `+ GetSelectedPart`     | `() : ExplodablePart`                         | Getter del estado actual.                                                                                                 |

**Patrón Swap Selection:** Para evitar el "flicker" visual de deseleccionar → seleccionar (donde por un frame no hay highlight):

1. Primero se aplica `OnDeselect()` al antiguo.
2. Inmediatamente se aplica `OnSelect()` al nuevo.
3. El `EventBus.Publish` ocurre después de ambos, garantizando que la UI refleje el estado final.

---

### 5.3 `ViewModeManager.cs` — Modos de Visualización

**Ruta:** `Assets/Scripts/Core/Managers/ViewModeManager.cs` | **Líneas:** 327  
**Namespace:** `WebGL.Core.Managers`

#### Enum `ViewMode`

```
Realistic(0), XRay(1), Blueprint(2), SolidColor(3),
Wireframe(4), Ghosted(5), Thermal(6)
```

#### Clase `ViewModeManager : Singleton<ViewModeManager>` [S]

| Campo                  | Tipo                             | Descripción                                        |
| ---------------------- | -------------------------------- | -------------------------------------------------- |
| `+ CurrentMode`        | `ViewMode { get; private set; }` | Modo activo (initial: Realistic)                   |
| `+ OnModeChanged`      | `event Action<ViewMode>`         | Evento C# directo al cambiar modo                  |
| `- _allRenderers`      | `List<Renderer>`                 | Todos los renderers de partes cacheados            |
| `- _originalMaterials` | `Dictionary<Renderer, Material>` | Backup de materiales originales para restaurar     |
| `- _categoryColors`    | `Dictionary<string, Color>`      | Mapeo categoría → color para modos no-texturizados |

| Método                       | Firma                                  | Descripción                                                                                                                                                                                                                        |
| ---------------------------- | -------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ SetMode`                  | `(ViewMode mode) : void`               | Cambia el modo: si Realistic, restaura originales. Si otro, crea material temporal con `Shader.Find("WebGL/XXX")`, asigna a todos los renderers, aplica colores de categoría vía `MaterialPropertyBlock`. Publica `OnModeChanged`. |
| `+ CacheRenderers`           | `() : void`                            | `FindObjectsByType<ExplodablePart>()` → para cada uno, almacena `renderer.sharedMaterial` en `_originalMaterials`.                                                                                                                 |
| `- CreateDefaultMaterials`   | `(ViewMode mode) : Material`           | `new Material(Shader.Find("WebGL/..."))` con fallback a `"Universal Render Pipeline/Lit"` si el shader no se encuentra. Configura propiedades por defecto según el modo.                                                           |
| `- ApplyCategoryColor`       | `(Renderer r, string category) : void` | Busca color en `_categoryColors`. Aplica via `MaterialPropertyBlock.SetColor("_BaseColor", color)`.                                                                                                                                |
| `+ RestoreOriginalMaterials` | `() : void`                            | Recorre `_originalMaterials` y restaura `sharedMaterial` de cada renderer.                                                                                                                                                         |

**Flujo de cambio de modo:**

```
SetMode(Blueprint)
├── if (currentMode == Realistic) CacheRenderers()  // Backup primera vez
├── Material blueprintMat = CreateDefaultMaterials(Blueprint)
│   └── Shader.Find("WebGL/Blueprint") ?? "Universal Render Pipeline/Lit"
├── foreach (renderer in _allRenderers)
│   ├── renderer.sharedMaterial = blueprintMat
│   └── ApplyCategoryColor(renderer, part.Data.category)
├── CurrentMode = Blueprint
└── OnModeChanged?.Invoke(Blueprint)
```

---

### 5.4 `OrbitCameraController.cs` — Cámara Orbital

**Ruta:** `Assets/Scripts/Core/Managers/OrbitCameraController.cs` | **Líneas:** 383  
**Namespace:** `WebGL.Core.Managers`

#### Clase `OrbitCameraController : Singleton<OrbitCameraController>` [S]

| Campo                                 | Tipo                     | Descripción                                    |
| ------------------------------------- | ------------------------ | ---------------------------------------------- |
| `+ target`                            | `Transform` (Serialized) | Punto de órbita (default: centro del dron)     |
| `+ distance`                          | `float`                  | Distancia radial actual                        |
| `+ minDistance/maxDistance`           | `float`                  | Límites de zoom (3–30)                         |
| `+ horizontalAngle/verticalAngle`     | `float`                  | Ángulos de órbita actuales en grados           |
| `+ minVerticalAngle/maxVerticalAngle` | `float`                  | Límites verticales (−89° a +89°)               |
| `+ orbitDamping`                      | `float`                  | Factor de suavizado (0.1 = muy suave)          |
| `+ panSpeed/zoomSpeed/rotateSpeed`    | `float`                  | Velocidades de cada operación                  |
| `- _targetHorizontal/_targetVertical` | `float`                  | Valores destino para interpolación suavizada   |
| `- _targetDistance`                   | `float`                  | Distancia destino                              |
| `- _viewportShift`                    | `float`                  | Desplazamiento horizontal del viewport (0–0.5) |

| Método                   | Firma                                       | Descripción                                                                                                                                                                |
| ------------------------ | ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `- Update`               | `void`                                      | Procesa input: mouse (right=orbit, middle=pan, scroll=zoom) + touch (1-finger orbit, 2-finger pinch+pan). Calcula valores target.                                          |
| `- LateUpdate`           | `void`                                      | Interpola `horizontal/vertical/distance` hacia targets con `Lerp(current, target, damping)`. Calcula posición esférica y aplica a `transform.position` + `LookAt(target)`. |
| `+ FocusOnObject`        | `(Renderer renderer) : void`                | Calcula bounds del renderer, ajusta distancia para que el objeto ocupe ~80% del viewport. Anima suavemente.                                                                |
| `+ SetViewportShift`     | `(float shift) : void`                      | Desplaza la cámara horizontalmente para compensar paneles de UI que cubren parte del viewport. Usado por `UIDetailsSheet` (0.15 al abrir, 0 al cerrar).                    |
| `+ SetTarget`            | `(Transform newTarget) : void`              | Cambia el punto de órbita + recalcula ángulos.                                                                                                                             |
| `+ SetAngles`            | `(float horizontal, float vertical) : void` | Establece ángulos de órbita directamente (usado por `CameraPresets`).                                                                                                      |
| `+ SetDistance`          | `(float d) : void`                          | Establece distancia radial (con clamp a min/max).                                                                                                                          |
| `+ ResetView`            | `() : void`                                 | Restaura target original, ángulos, distancia por defecto.                                                                                                                  |
| `- HandleSmartPivot`     | `() : void`                                 | Raycast desde el centro de la pantalla; si intersecta geometría, ajusta el `target` al punto de impacto para orbitar alrededor del objeto visible.                         |
| `- CalculateElevatorPan` | `(Vector2 delta) : Vector3`                 | Paneo usando `Vector3.up` como eje vertical (en lugar del up local de la cámara), produciendo movimiento "tipo ascensor" que se siente más natural.                        |

**"Elevator Panning":** A diferencia del paneo convencional (que usa los ejes locales de la cámara), el paneo vertical de esta implementación usa `Vector3.up` (world up). Esto significa que mover el mouse hacia arriba SIEMPRE sube el modelo, independientemente del ángulo de la cámara. Es más intuitivo para usuarios no técnicos.

**Damped Lerp en LateUpdate:** La separación input/render en `Update`/`LateUpdate` evita jitter. El `Lerp` con factor de damping produce animación suave sin necesidad de `TweenEngine` (sería overhead para algo que ejecuta cada frame).

---

### 5.5 `CrossSectionManager.cs` — Corte Transversal Dual

**Ruta:** `Assets/Scripts/Core/Managers/CrossSectionManager.cs` | **Líneas:** 466  
**Namespace:** `WebGL.Core.Managers`

#### Clase `CrossSectionManager : Singleton<CrossSectionManager>` [S]

| Campo                   | Tipo                             | Descripción                                            |
| ----------------------- | -------------------------------- | ------------------------------------------------------ |
| `- _planes`             | `ClipPlane[2]`                   | Array de 2 planos de corte (máximo simultáneo)         |
| `- _visualQuads`        | `GameObject[2]`                  | Quads visuales que representan los planos en la escena |
| `- _clippableMaterials` | `Dictionary<Renderer, Material>` | Cache de materiales ClippableLit creados               |
| `- _originalMaterials`  | `Dictionary<Renderer, Material>` | Backup de materiales originales                        |
| `- _activeAxes`         | `List<int>`                      | Ejes activos (0=X, 1=Y, 2=Z), orden FIFO               |
| `+ OnClipStateChanged`  | `event Action<bool>`             | Evento al activar/desactivar clipping                  |

**Struct `ClipPlane`:**

| Campo      | Tipo    | Descripción                              |
| ---------- | ------- | ---------------------------------------- |
| `axis`     | `int`   | 0=X, 1=Y, 2=Z                            |
| `position` | `float` | Posición del plano (−1 a +1 normalizado) |
| `inverted` | `bool`  | ¿Invertir dirección del corte?           |
| `enabled`  | `bool`  | ¿Plano activo?                           |

| Método                | Firma                                     | Descripción                                                                                                                                                                                                                                                                           |
| --------------------- | ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ ToggleAxis`        | `(int axis) : void`                       | Si el eje ya está activo, lo desactiva. Si hay <2 activos, activa. Si hay 2 activos, desactiva el más antiguo (FIFO) y activa el nuevo. Llama `UpdateClipPlanes()`.                                                                                                                   |
| `+ SetPlanePosition`  | `(int planeIndex, float position) : void` | Actualiza la posición del plano seleccionado. Recalcula la ecuación del plano.                                                                                                                                                                                                        |
| `+ InvertPlane`       | `(int planeIndex) : void`                 | Invierte la dirección de corte (flip de la normal).                                                                                                                                                                                                                                   |
| `- UpdateClipPlanes`  | `() : void`                               | Para cada plano activo: calcula la ecuación `(normal, d)` a partir de eje+posición+inversión. Llama `Shader.SetGlobalVector("_GlobalClipPlane", eq)` y `Shader.SetGlobalFloat("_GlobalClipEnabled", 1)`. Posiciona visual quads.                                                      |
| `- SwapToClippable`   | `() : void`                               | Para modo Realistic: itera todos los renderers, lee propiedades PBR del material actual (BaseMap, NormalMap, Metallic, Smoothness, color, tiling, offset), crea material con shader `"WebGL/ClippableLit"`, copia TODAS las propiedades. Almacena originales en `_originalMaterials`. |
| `- RestoreOriginals`  | `() : void`                               | Restaura `_originalMaterials` a cada renderer.                                                                                                                                                                                                                                        |
| `- OnViewModeChanged` | `(ViewMode mode) : void`                  | Suscrito a `ViewModeManager.OnModeChanged`. Si el modo cambió a/desde Realistic mientras hay clipping activo, coordina swap/restore de materiales.                                                                                                                                    |

**FIFO Deselection:** Cuando el usuario activa un 3er eje (solo 2 planos permitidos), el eje más antiguo se desactiva automáticamente. Esto usa `_activeAxes` como una cola FIFO:

```csharp
if (_activeAxes.Count >= 2)
{
    int oldest = _activeAxes[0];
    _activeAxes.RemoveAt(0);  // Dequeue del más antiguo
    DisablePlane(oldest);
}
_activeAxes.Add(newAxis);     // Enqueue del nuevo
```

**Coordinación con ViewModeManager:** El `CrossSectionManager` escucha los cambios de modo de visualización. Si el usuario está en modo Realistic (PBR) y activa el corte, los materiales URP/Lit NO soportan clipping global. Por eso el manager hace swap a `ClippableLit` (que sí lo soporta). Si el modo cambia a XRay, Blueprint, etc. (que ya soportan clipping nativamente), restaura los originales.

---

### 5.6 `ExplodedViewManager.cs` — Vista Explosionada

**Ruta:** `Assets/Scripts/Core/Managers/ExplodedViewManager.cs` | **Líneas:** 263  
**Namespace:** `WebGL.Core.Managers`

#### Clase `ExplodedViewManager : Singleton<ExplodedViewManager>` [S]

| Campo                  | Tipo                       | Descripción                                                          |
| ---------------------- | -------------------------- | -------------------------------------------------------------------- |
| `+ explosionFactor`    | `float [Range(0,1)]`       | Factor de explosión actual (0=cerrado, 1=completamente explosionado) |
| `- _parts`             | `List<ExplodablePart>`     | Todas las partes explosionables                                      |
| `- _categoryFilters`   | `Dictionary<string, bool>` | Filtros activos por categoría                                        |
| `+ OnExplosionChanged` | `event Action<float>`      | Evento al cambiar el factor                                          |

| Método                 | Firma                                       | Descripción                                                                                                                                                                |
| ---------------------- | ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `- Start`              | `void`                                      | `FindObjectsByType<ExplodablePart>()` → `Initialize()` cada una. Suscribe a `EventBus<AppStateChangedEvent>`.                                                              |
| `+ SetExplosionFactor` | `(float factor) : void`                     | Clamp 0–1 → almacena → `UpdateAllParts()` → publica evento.                                                                                                                |
| `- UpdateAllParts`     | `() : void`                                 | Para cada parte: si su categoría está habilitada en `_categoryFilters`, llama `part.UpdateExplosion(explosionFactor)`. Si está filtrada, mueve a posición base (factor=0). |
| `+ SetCategoryFilters` | `(Dictionary<string, bool> filters) : void` | Recibe dict de categoría→visible. Actualiza `_categoryFilters`. Llama `UpdateAllParts()`.                                                                                  |
| `- OnAppStateChanged`  | `(AppStateChangedEvent evt) : void`         | Si el estado sale de `ExplodedView`, resetea `explosionFactor = 0`.                                                                                                        |

**Interpolación:** Cada `ExplodablePart.UpdateExplosion(factor)` ejecuta `Vector3.Lerp(original, target, factor)`. El factor viene del slider de UI (0–1 continuo), produciendo una animación suave controlada por el usuario en tiempo real.

---

### 5.7 `PartVisibilityManager.cs` — Visibilidad y Aislamiento

**Ruta:** `Assets/Scripts/Core/Managers/PartVisibilityManager.cs` | **Líneas:** 211  
**Namespace:** `WebGL.Core.Managers`

#### Clase `PartVisibilityManager : Singleton<PartVisibilityManager>` [S]

| Campo                   | Tipo                      | Descripción                                      |
| ----------------------- | ------------------------- | ------------------------------------------------ |
| `- _allParts`           | `List<ExplodablePart>`    | Cacheadas en Start                               |
| `- _hiddenParts`        | `HashSet<ExplodablePart>` | Partes actualmente ocultas                       |
| `+ fadeDuration`        | `float`                   | Duración de la animación de fade (default: 0.3s) |
| `+ OnVisibilityChanged` | `event Action`            | Evento genérico al cambiar visibilidad           |

| Método                   | Firma                            | Descripción                                                                                                                                           |
| ------------------------ | -------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ HidePart`             | `(ExplodablePart part) : void`   | Inicia corrutina `FadeOut(part)`: anima alpha de 1→0 via `MaterialPropertyBlock("_BaseColor" con alpha)` usando smoothstep, luego `SetActive(false)`. |
| `+ ShowPart`             | `(ExplodablePart part) : void`   | `SetActive(true)` → inicia corrutina `FadeIn(part)`: anima alpha de 0→1.                                                                              |
| `+ ShowAllParts`         | `() : void`                      | Recorre `_hiddenParts`, muestra cada una, limpia el set.                                                                                              |
| `+ IsolatePart`          | `(ExplodablePart part) : void`   | Oculta TODAS las partes excepto `part` via `SetActive(false)` (sin animación para inmediatez).                                                        |
| `+ TogglePartVisibility` | `(ExplodablePart part) : void`   | Toggle entre HidePart/ShowPart.                                                                                                                       |
| `- FadeOut`              | `(ExplodablePart) : IEnumerator` | Loop 0→fadeDuration, calcula `t = elapsed/duration`, aplica `smoothstep(1, 0, t)` como alpha. Al finalizar, `SetActive(false)`.                       |
| `- FadeIn`               | `(ExplodablePart) : IEnumerator` | Inverso: `smoothstep(0, 1, t)`.                                                                                                                       |

**Smoothstep:** `3t² - 2t³` produce una curva de aceleración/desaceleración suave (ease-in-out), más agradable visualmente que un fade lineal.

---

### 5.8 `EnvironmentController.cs` — Control de Entorno

**Ruta:** `Assets/Scripts/Core/Managers/EnvironmentController.cs` | **Líneas:** 152  
**Namespace:** `WebGL.Core.Managers`

#### Clase `EnvironmentController : Singleton<EnvironmentController>` [S]

| Campo              | Tipo                 | Descripción                            |
| ------------------ | -------------------- | -------------------------------------- |
| `+ mainLight`      | `Light` (Serialized) | Luz direccional principal de la escena |
| `- _currentPreset` | `string`             | Nombre del preset activo               |

**Presets definidos (hardcoded):**

| Preset    | Rotación Luz | Intensidad | Color Luz      | Background Cámara             |
| --------- | ------------ | ---------- | -------------- | ----------------------------- |
| Studio    | (50,−30,0)   | 1.0        | Blanco cálido  | AnimatedGradientSkybox shader |
| Sunset    | (15,−45,0)   | 0.8        | Naranja cálido | Color sólido oscuro           |
| Night     | (−20,0,0)    | 0.3        | Azul frío      | Color muy oscuro              |
| Blueprint | (45,45,0)    | 0.6        | Blanco puro    | Color azul oscuro             |
| Neutral   | (50,0,0)     | 0.9        | Blanco neutro  | Color gris medio              |

| Método                | Firma                        | Descripción                                                                                                                                                                                              |
| --------------------- | ---------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ ApplyPreset`       | `(string presetName) : void` | Switch sobre preset: configura rotación, intensidad, color del Light. Configura `camera.clearFlags` y `camera.backgroundColor`. Para Studio, asigna material con shader `Skybox/AnimatedGradientSkybox`. |
| `+ SetLightRotation`  | `(float angle) : void`       | Rota `mainLight.transform.eulerAngles.y` al ángulo dado. Para control fino por slider.                                                                                                                   |
| `+ SetLightIntensity` | `(float intensity) : void`   | `mainLight.intensity = intensity`.                                                                                                                                                                       |
| `+ GetCurrentPreset`  | `() : string`                | Getter.                                                                                                                                                                                                  |

---

### 5.9 `InputManager.cs` — Entrada Unificada

**Ruta:** `Assets/Scripts/Core/Managers/InputManager.cs` | **Líneas:** 145  
**Namespace:** `WebGL.Core.Managers`

#### Clase `InputManager : PersistentSingleton<InputManager>` [PS]

| Campo            | Tipo                         | Descripción                                                                                                                               |
| ---------------- | ---------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `+ InputBlocked` | `static bool`                | **Single source of truth** para bloqueo de entrada 3D. Cualquier sistema UI lo establece a `true` cuando el usuario interactúa con la UI. |
| `+ IsDragging3D` | `bool { get; private set; }` | `true` si el usuario está arrastando con botón derecho y el drag comenzó fuera de la UI.                                                  |
| `- _panel`       | `IPanel`                     | Panel de UI Toolkit cacheado lazily                                                                                                       |
| `- _uiDocument`  | `UIDocument`                 | Referencia cacheada                                                                                                                       |

| Método              | Firma       | Descripción                                                                                                                                                                                                                                      |
| ------------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `+ IsPointerOverUI` | `() : bool` | Convierte `Input.mousePosition` a coordenadas de panel UI Toolkit (flip Y + `RuntimePanelUtils.ScreenToPanel`). Llama `panel.Pick(panelPos)`. Si el elemento picked no es null y no es el root (que tiene `pickingMode=Ignore`), retorna `true`. |
| `- CachePanel`      | `() : void` | `FindAnyObjectByType<UIDocument>()` → `rootVisualElement.panel`. Caching lazy para evitar Find en cada frame.                                                                                                                                    |
| `- Update`          | `void`      | Tracks right-click-down/up para `IsDragging3D`. Solo inicia drag si `!IsPointerOverUI()` al momento del mouse-down.                                                                                                                              |

**¿Por qué no `EventSystem.IsPointerOverGameObject()`?** Esa API es para **UGUI (Canvas)**. UI Toolkit tiene su propio sistema de picking que NO se integra con `EventSystem`. Por eso se implementa `IsPointerOverUI()` manualmente usando la API de `IPanel.Pick()`.

---

### 5.10 `DroneStateController.cs` — Estados del Dron

**Ruta:** `Assets/Scripts/Core/Managers/DroneStateController.cs` | **Líneas:** 263  
**Namespace:** `WebGL.Core.Managers`

#### Enum `DroneState`

```
Off(0), StartingUp(1), Idle(2), Flying(3), ShuttingDown(4)
```

#### Clase `DroneStateController : Singleton<DroneStateController>` [S]

| Campo                             | Tipo                               | Descripción                             |
| --------------------------------- | ---------------------------------- | --------------------------------------- |
| `+ CurrentState`                  | `DroneState { get; private set; }` | Estado actual del dron                  |
| `+ propellers`                    | `Transform[]` (Serialized)         | Transforms de las hélices para rotación |
| `+ statusLights`                  | `Renderer[]` (Serialized)          | LEDs indicadores de estado              |
| `+ propellerSpeed`                | `float`                            | RPM de las hélices                      |
| `+ hoverAmplitude/hoverFrequency` | `float`                            | Parámetros de animación de hovering     |
| `- _audioSource`                  | `AudioSource`                      | Componente de audio                     |
| `+ OnStateChanged`                | `event Action<DroneState>`         | Evento C# directo                       |

| Método                  | Firma                  | Descripción                                                                                                                                                     |
| ----------------------- | ---------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ TogglePower`         | `() : void`            | Si Off → `StartUp()`. Si Idle/Flying → `ShutDown()`.                                                                                                            |
| `- StartUp`             | `() : IEnumerator`     | Secuencia: Off → StartingUp → luces amarillas → ramp-up de velocidad de hélices (ease-in) → Idle → luces verdes. Duración ~2s.                                  |
| `- ShutDown`            | `() : IEnumerator`     | Secuencia inversa: Idle → ShuttingDown → ramp-down → Off → luces rojas → luces apagadas.                                                                        |
| `+ ToggleFlight`        | `() : void`            | Si Idle → Flying. Si Flying → Idle.                                                                                                                             |
| `- Update`              | `void`                 | Si Flying: rota hélices (`propellerSpeed * Time.deltaTime`), aplica `sin(Time.time * hoverFrequency) * hoverAmplitude` como desplazamiento vertical (hovering). |
| `- SetStatusLightColor` | `(Color color) : void` | Aplica color a todos los `statusLights` via `MaterialPropertyBlock`.                                                                                            |

---

## 6. Capa Core — Managers de Herramientas

### 6.1 `AssemblyGuideManager.cs` — Guía de Ensamblaje

**Ruta:** `Assets/Scripts/Core/Managers/AssemblyGuideManager.cs` | **Líneas:** 290  
**Namespace:** `WebGL.Core.Managers`

#### Clase `AssemblyGuideManager : Singleton<AssemblyGuideManager>` [S]

| Campo                | Tipo                             | Descripción                                 |
| -------------------- | -------------------------------- | ------------------------------------------- |
| `+ guideData`        | `AssemblyGuideData` (Serialized) | Datos de la guía activa                     |
| `+ CurrentStepIndex` | `int { get; private set; }`      | Paso actual (0-based)                       |
| `+ CurrentStep`      | `AssemblyStep`                   | Getter: `guideData.steps[CurrentStepIndex]` |
| `+ IsGuideActive`    | `bool`                           | ¿Guía en progreso?                          |
| `- _completedSteps`  | `HashSet<int>`                   | Pasos marcados como completados             |
| `+ OnStepChanged`    | `event Action<int>`              | Al cambiar de paso                          |
| `+ OnStepCompleted`  | `event Action<int>`              | Al completar un paso                        |
| `+ OnGuideCompleted` | `event Action`                   | Al completar todos los pasos                |

| Método                  | Firma       | Descripción                                                                                                                                                                             |
| ----------------------- | ----------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ StartGuide`          | `() : void` | `IsGuideActive = true`, `CurrentStepIndex = 0`, limpia `_completedSteps`, llama `ApplyStep()`.                                                                                          |
| `+ StopGuide`           | `() : void` | Desactiva guía, restaura estado visual.                                                                                                                                                 |
| `+ NextStep`            | `() : void` | `CurrentStepIndex++` si no es el último. Llama `ApplyStep()`.                                                                                                                           |
| `+ PreviousStep`        | `() : void` | `CurrentStepIndex--` si no es el primero.                                                                                                                                               |
| `+ CompleteCurrentStep` | `() : void` | Agrega a `_completedSteps`. Si todos completados, publica `OnGuideCompleted`.                                                                                                           |
| `- ApplyStep`           | `() : void` | Busca la parte target por `targetPartId`. Aplica highlight vía `HighlightSystem.OnSelect()`. Posiciona cámara según `cameraAngle` y `cameraDistance` del paso. Publica `OnStepChanged`. |

---

### 6.2 `MeasurementTool.cs` — Herramienta de Medición

**Ruta:** `Assets/Scripts/Core/Managers/MeasurementTool.cs` | **Líneas:** 278  
**Namespace:** `WebGL.Core.Managers`

#### Enum `MeasureMode`

```
Distance(0), Angle(1), Radius(2)
```

#### Clase `MeasurementTool : Singleton<MeasurementTool>` [S]

| Campo                     | Tipo                   | Descripción                                   |
| ------------------------- | ---------------------- | --------------------------------------------- |
| `+ currentMode`           | `MeasureMode`          | Modo activo                                   |
| `- _points`               | `List<Vector3>`        | Puntos colocados por el usuario               |
| `- _lineRenderer`         | `LineRenderer`         | Línea visual entre puntos                     |
| `- _pointMarkers`         | `List<GameObject>`     | Esferas primitivas posicionadas en los puntos |
| `+ OnMeasurementComplete` | `event Action<string>` | Resultado formateado al completar medición    |

| Método                | Firma                       | Descripción                                                                                                                                                                                                 |
| --------------------- | --------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ SetMode`           | `(MeasureMode mode) : void` | Cambia modo, limpia puntos anteriores.                                                                                                                                                                      |
| `- Update`            | `void`                      | Si `InputBlocked` → return. Click izquierdo → `PlacePoint()`.                                                                                                                                               |
| `- PlacePoint`        | `() : void`                 | Raycast desde mouse. Si hit, almacena punto + crea esfera marker + actualiza LineRenderer. Si se alcanzó el número requerido de puntos (2 para Distance/Radius, 3 para Angle), calcula y publica resultado. |
| `- CalculateDistance` | `() : float`                | `Vector3.Distance(p0, p1)`                                                                                                                                                                                  |
| `- CalculateAngle`    | `() : float`                | `Vector3.Angle(p0-p1, p2-p1)` — ángulo en el vértice p1.                                                                                                                                                    |
| `- CalculateRadius`   | `() : float`                | Distancia del punto al eje del círculo que pasa por los puntos. Calcula circunferencia (`2πr`) y área (`πr²`).                                                                                              |
| `+ FormatDistance`    | `(float meters) : string`   | Auto-escala: <0.01m → mm, <1m → cm, ≥1m → m. Formato a 2 decimales.                                                                                                                                         |
| `+ ClearMeasurements` | `() : void`                 | Destruye markers, limpia LineRenderer, vacía lista.                                                                                                                                                         |

---

### 6.3 `AnnotationSystem.cs` — Anotaciones 3D

**Ruta:** `Assets/Scripts/Core/Managers/AnnotationSystem.cs` | **Líneas:** 196  
**Namespace:** `WebGL.Core.Managers`

#### Clase `AnnotationSystem : Singleton<AnnotationSystem>` [S]

**Clase auxiliar `Annotation`** [Serializable]:

| Campo       | Tipo            |
| ----------- | --------------- |
| `id`        | `string` (GUID) |
| `text`      | `string`        |
| `position`  | `Vector3`       |
| `partId`    | `string`        |
| `createdAt` | `DateTime`      |
| `color`     | `Color`         |

| Método                          | Firma                                                    | Descripción                                                                                                                                                                                              |
| ------------------------------- | -------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ AddAnnotation`               | `(Vector3 pos, string text, string partId) : Annotation` | Crea DTO, instancia visual 3D: esfera coloreada + LineRenderer hacia la superficie + TextMesh con texto billboard. El `BillboardBehavior` (clase interna) rota el texto hacia la cámara cada LateUpdate. |
| `+ RemoveAnnotation`            | `(string id) : void`                                     | Destruye GameObject, remueve de lista y diccionario.                                                                                                                                                     |
| `+ ClearAllAnnotations`         | `() : void`                                              | Remueve todas.                                                                                                                                                                                           |
| `+ Toggle/Show/HideAnnotations` | `() : void`                                              | Visibilidad batch de todos los markers.                                                                                                                                                                  |
| `+ GetAnnotationsForPart`       | `(string partId) : List<Annotation>`                     | Filtro por parte.                                                                                                                                                                                        |

---

### 6.4 `ConnectionPointsViewer.cs` — Visualizador de Conexiones

**Ruta:** `Assets/Scripts/Core/Managers/ConnectionPointsViewer.cs` | **Líneas:** 163  
**Namespace:** `WebGL.Core.Managers`

| Método                                     | Firma                   | Descripción                                                                                       |
| ------------------------------------------ | ----------------------- | ------------------------------------------------------------------------------------------------- |
| `+ FindAllConnections`                     | `() : void`             | Busca `ConnectionPointMarker` en hijos de todos los `ExplodablePart`.                             |
| `+ ShowConnections/HideConnections/Toggle` | `() : void`             | Visibilidad de markers y líneas.                                                                  |
| `- CreateMarkers`                          | `() : void`             | Esferas primitivas coloreadas por tipo: tornillo=gris, snap=verde, soldadura=naranja, cable=azul. |
| `- CreateConnectionLines`                  | `() : void`             | `LineRenderer` entre puntos conectados.                                                           |
| `+ GetColorForType`                        | `(string type) : Color` | Switch expression de C# 8 para mapeo tipo→color.                                                  |

---

### 6.5 `BillOfMaterialsManager.cs` — Lista de Materiales (BOM)

**Ruta:** `Assets/Scripts/Core/Managers/BillOfMaterialsManager.cs` | **Líneas:** 103  
**Namespace:** `WebGL.Core.Managers`

| Método                          | Firma                      | Descripción                                                                                               |
| ------------------------------- | -------------------------- | --------------------------------------------------------------------------------------------------------- |
| `+ GenerateBOM`                 | `() : void`                | Busca `ExplodablePart`, agrupa por `Data.id` (LINQ GroupBy), crea `BOMItem` por grupo con cantidad=count. |
| `+ ExportToCSV`                 | `() : string`              | Header CSV + filas por item + línea de totales (peso total, partes totales).                              |
| `+ GetByCategory/GetByMaterial` | `(string) : List<BOMItem>` | Filtros LINQ.                                                                                             |

**Propiedades calculadas:** `TotalWeight` (Sum), `TotalParts` (Sum quantities), `UniquePartsCount` (Count).

---

### 6.6 `ModularPartsSystem.cs` — Sistema de Partes Modulares

**Ruta:** `Assets/Scripts/Core/Managers/ModularPartsSystem.cs` | **Líneas:** 165  
**Namespace:** `WebGL.Core.Managers`

| Método                 | Firma                                                 | Descripción                                                                                                                                                                                       |
| ---------------------- | ----------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ CanAttach`          | `(ModularSlot slot, DronePartData data) : bool`       | Verifica si `data.partType` está en `slot.compatibleTypes`.                                                                                                                                       |
| `+ SwapPart`           | `(ModularSlot slot, GameObject prefab) : IEnumerator` | 1) Fade alpha 1→0 de parte actual + movimiento vertical hacia arriba. 2) `Destroy(currentPart)`. 3) `Instantiate(prefab)` en `attachPoint`. 4) Fade alpha 0→1. Animaciones con easing cuadrático. |
| `+ RemovePart`         | `(ModularSlot slot) : void`                           | Desinstala sin reemplazo.                                                                                                                                                                         |
| `+ GetCompatibleParts` | `(ModularSlot slot) : List<GameObject>`               | Filtra `availableParts` por compatibilidad.                                                                                                                                                       |

---

### 6.7 `AssemblyChecklist.cs` — Checklist de Verificación

**Ruta:** `Assets/Scripts/Core/Managers/AssemblyChecklist.cs` | **Líneas:** 147  
**Namespace:** `WebGL.Core.Managers`

| Método                | Firma                         | Descripción                                                                               |
| --------------------- | ----------------------------- | ----------------------------------------------------------------------------------------- |
| `+ GenerateChecklist` | `() : void`                   | Busca `ExplodablePart`, agrupa por id, crea ítems de checklist.                           |
| `+ VerifyItem`        | `(string id, int qty) : void` | Marca como verificado con timestamp. Si todos verificados, publica `OnChecklistComplete`. |
| `+ ToggleItem`        | `(string id) : void`          | Toggle verify/unverify.                                                                   |

**Propiedades:** `TotalItems`, `VerifiedCount`, `ProgressPercent`, `IsComplete`.

---

### 6.8 `PartCatalogManager.cs` — Catálogo de Partes

**Ruta:** `Assets/Scripts/Core/Managers/PartCatalogManager.cs` | **Líneas:** 155  
**Namespace:** `WebGL.Core.Managers`

| Campo                                              | Tipo                           | Descripción                 |
| -------------------------------------------------- | ------------------------------ | --------------------------- |
| `- _allParts`                                      | `List<ExplodablePart>`         | Todas las partes            |
| `+ filteredParts`                                  | `List<ExplodablePart>`         | Resultado filtrado actual   |
| `- _searchQuery, _categoryFilter, _materialFilter` | `string`                       | Filtros activos             |
| `+ OnFilterChanged`                                | `event Action`                 | Al cambiar cualquier filtro |
| `+ OnPartFocused`                                  | `event Action<ExplodablePart>` | Al enfocar una parte        |

| Método               | Firma                          | Descripción                                                                                                                  |
| -------------------- | ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------- |
| `+ Search`           | `(string query) : void`        | Establece `_searchQuery`, llama `ApplyFilters()`.                                                                            |
| `+ FilterByCategory` | `(string cat) : void`          | Establece `_categoryFilter`, llama `ApplyFilters()`.                                                                         |
| `+ FilterByMaterial` | `(string mat) : void`          | Establece `_materialFilter`, llama `ApplyFilters()`.                                                                         |
| `+ ClearFilters`     | `() : void`                    | Resetea los 3 filtros, refresca.                                                                                             |
| `- ApplyFilters`     | `() : void`                    | Pipeline LINQ: `_allParts.Where(searchMatch).Where(categoryMatch).Where(materialMatch).ToList()`. Publica `OnFilterChanged`. |
| `+ FocusPart`        | `(ExplodablePart part) : void` | Publica `PartSelectedEvent` via EventBus. Enfoca cámara. Aísla parte.                                                        |

---

## 7. Capa Core — Managers de Soporte

### 7.1 `AudioManager.cs` [PS]

6 SFX (`PlayHover`, `PlayClick`, `PlayExplosionSound`, `PlayTransition`, `PlayError`, `PlaySuccess`) + sistema de música. 3 volúmenes (master/SFX/music) con persistencia en `PlayerPrefs`.

### 7.2 `NotificationManager.cs` [PS]

Toast notifications tipo pill: fondo oscuro, esquina redondeada, posición absoluta superior. Fade-in (0.2s) → display → fade-out (0.5s) via corrutina. UI creada programáticamente.

### 7.3 `AccessibilityManager.cs` [PS]

UI Scale (0.8–1.5), High Contrast mode (CSS class `.high-contrast`), Reduced Motion (CSS class `.reduced-motion`). Persistencia en `PlayerPrefs` con keys `A11y_*`.

### 7.4 `CursorManager.cs` [S]

7 `CursorType` enum con `Texture2D` personalizadas. `Cursor.SetCursor(texture, hotspot, CursorMode.Auto)`. Optimización: skip si tipo ya es el actual.

### 7.5 `ErrorHandler.cs` [PS]

Auto-captura excepciones via `Application.logMessageReceived`. Crea panel fullscreen de error en UI Toolkit con botones Retry/Dismiss. `ShowError(message, retryAction)` para errores manuales.

### 7.6 `AnalyticsManager.cs` [PS]

Tracking local (sin backend). `TrackEvent(name, params)`, `TrackPartSelected(name)` con acumulación de tiempo de vista. `GetSessionSummary()` output en `OnApplicationQuit`.

### 7.7 `QualityManager.cs` [S]

Cada 2 segundos calcula FPS promedio. Si FPS < target−5 → baja `renderScale` 0.1. Si FPS > target+10 → sube 0.05. `ScalableBufferManager.ResizeBuffers()` comentado (incompatibilidad Unity 6).

### 7.8 `WebGLOptimizer.cs` [PS]

`ApplyOptimizations()`: reduce mip level en mobile, desactiva sombras, ajusta LOD bias y particle budget. `CheckMemory()`: cada 300 frames, si heap > 800MB → `GC.Collect()` + `Resources.UnloadUnusedAssets()`.

### 7.9 `ScreenshotManager.cs` [S]

Captura vía `RenderTexture` → `ReadPixels` → `EncodeToPNG`. WebGL: base64 (JS download stub). Editor: `File.WriteAllBytes`. Protección contra capturas simultáneas.

### 7.10 `KeyboardShortcuts.cs` [S]

Atajos: 1-6=presets cámara, 7/8/9=modos Explore/Analyze/Studio, E=explosión, Esc=volver, R=reset, Ctrl±=escala UI, F1=console, F2=FPS, F12=screenshot. Respeta `InputManager.InputBlocked` e `IsInteractive()`.

### 7.11 `CameraPresets.cs` [S]

6 presets (`CameraPreset` struct): Front(0°,0°), Back(180°,0°), Left(−90°,0°), Right(90°,0°), Top(0°,89°), Isometric(45°,35°). Delega a `OrbitCameraController.SetAngles/SetDistance`.

### 7.12 `SaveSystem` (static)

2 claves `PlayerPrefs`: `"MasterVolume"` (float, default 1.0), `"QualityLevel"` (int, default 2). `DeleteAll()`.

### 7.13 `AssetLoader` [—]

Stub: `LoadAssetAsync(name, callback)` → simula 1s delay + `Resources.LoadAsync<GameObject>`. Placeholder para futura migración a Addressables.

---

## 8. Capa Core — Utilidades

### 8.1 `DroneAssembler.cs` [—]

Creación procedural de un dron con 7 tipos: Chassis (Box gris), Flight Controller (Cube rojo), Battery (Cube verde), 4 Arms (cylinders), Motors (cylinders azules), Camera (cube magenta). Cada parte recibe `ExplodablePart` + `MaterialController` + `HighlightSystem` + runtime `DronePartData` ScriptableObject. Para testing sin modelo 3D.

### 8.2 `DronePartSetup.cs` [—]

Context menus de editor: `SetupPart()` asigna componentes necesarios (ExplodablePart, MeshCollider, layer). `AddConnectionPointMarkers()` crea puntos de conexión. `CreatePartDataAsset()` genera ScriptableObject con dimensiones del Renderer bounds. Usa reflexión para campo privado.

### 8.3 `FPSCounter.cs` [—]

Display IMGUI en esquina superior izquierda. FPS suavizado con interpolación exponencial (factor 0.1). Colores: verde ≥50, amarillo ≥30, rojo <30.

### 8.4 `PerformanceMonitor.cs` [S]

Overlay IMGUI (F3 toggle): FPS (coloreado), memoria actual/peak, quality level, resolution scale. `GetPerformanceReport()` para logging.

### 8.5 `RuntimeConsole.cs` [PS]

Console in-game suscrita a `Application.logMessageReceived`. Toggle con backtick (`` ` ``) o 3-finger tap. IMGUI con scroll. Cola de 20 mensajes con colores por tipo (Log=blanco, Error=rojo, Warning=amarillo, Exception=magenta).

### 8.6 `WebGLProfiler.cs` [—]

Profiler simplificado: FPS + frame time + heap size. Logging con emojis de estado (✓/⚠/✘). Umbrales: Good ≥55, OK ≥30, Warning ≥20, Critical <20.

---

## 9. Capa UI — Orquestación

### 9.1 `UIManager.cs` — Orquestador Principal

**Ruta:** `Assets/Scripts/UI/UIManager.cs` | **Líneas:** 430  
**Namespace:** `WebGL.UI`

#### Clase `UIManager : Singleton<UIManager>` [S]

| Campo                | Tipo                  | Descripción                                    |
| -------------------- | --------------------- | ---------------------------------------------- |
| `- _document`        | `UIDocument`          | Componente UIDocument de la escena             |
| `- _root`            | `VisualElement`       | Root del Visual Tree                           |
| `- _modeController`  | `UIModeController`    | Sub-controlador de modos                       |
| `- _detailsSheet`    | `UIDetailsSheet`      | Sub-controlador del bottom sheet               |
| `- _heroController`  | `UIHeroController`    | Sub-controlador del hero/landing               |
| `- _analyzePanel`    | `UIAnalyzePanel`      | Sub-controlador de modos shader                |
| `- _crossPanel`      | `UICrossSectionPanel` | Sub-controlador de corte                       |
| `- _envPanel`        | `UIEnvironmentPanel`  | Sub-controlador de entorno                     |
| `- _cleanupActions`  | `List<Action>`        | Acciones de cleanup para dispose               |
| `- _lastClickTime`   | `float`               | Timestamp del último click (para double-click) |
| `- _lastClickedData` | `DronePartData`       | Datos del último click (para double-click)     |

| Método                          | Firma                            | Descripción                                                                                                                                                              |
| ------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `- Start`                       | `void`                           | Obtiene UIDocument → root. Crea 6 sub-controladores. Suscribe a `PartSelectedEvent` y `AppStateChangedEvent`. Llama `RegisterButtonInputBlockers()`. `EnsureManagers()`. |
| `- OnPartSelected`              | `(PartSelectedEvent evt) : void` | Detección double-click (0.35s): si double → `IsolatePart + OpenSheet`. Si single → `PopulatePartData`.                                                                   |
| `- RegisterButtonInputBlockers` | `() : void`                      | Para cada `Button` en el root, registra `PointerEnter/Leave` que establece `InputManager.InputBlocked`. Previene que clicks en botones también muevan la cámara.         |
| `- EnsureManagers`              | `() : void`                      | Verifica que managers críticos existan (redundante con `SceneBootstrapper`, pero defensivo).                                                                             |
| `+ AddCleanup`                  | `(Action action) : void`         | Agrega acción al cleanup list.                                                                                                                                           |
| `- OnDestroy`                   | `void`                           | Ejecuta todas las cleanup actions. Desuscribe del EventBus.                                                                                                              |

**Patrón AddCleanup:** En lugar de un `OnDestroy` monolítico que conozca todos los detalles de limpieza, cada sub-controlador registra su propia limpieza:

```csharp
// En UIModeController constructor:
uiManager.AddCleanup(() => {
    EventBus.Unsubscribe<AppStateChangedEvent>(OnAppStateChanged);
    // cleanup de callbacks...
});
```

Esto invierte la dependencia de cleanup: el sub-controlador define su propia limpieza, el UIManager solo la ejecuta.

---

## 10. Capa UI — Sub-Controladores

### 10.1 `UIModeController.cs` — Controlador de 3 Modos

**Ruta:** `Assets/Scripts/UI/UIModeController.cs` | **Líneas:** 561  
**Namespace:** `WebGL.UI`

#### Clase `UIModeController` (no MonoBehaviour — clase C# pura) [—]

| Campo               | Tipo                                | Descripción                               |
| ------------------- | ----------------------------------- | ----------------------------------------- |
| `- _root`           | `VisualElement`                     | Root del Visual Tree                      |
| `- _modeContainers` | `Dictionary<string, VisualElement>` | Tools/Analyze/Studio containers           |
| `- _modeButtons`    | `Dictionary<string, Button>`        | ModeToolsBtn/ModeAnalyzeBtn/ModeStudioBtn |
| `- _activeMode`     | `string`                            | Modo activo actual (null = ninguno)       |
| `- _activeSubPanel` | `VisualElement`                     | Sub-panel abierto (en modo Analyze)       |
| `- _cleanupActions` | `List<Action>`                      | Cleanup list                              |

| Método                     | Firma                      | Descripción                                                                                                                                                                                 |
| -------------------------- | -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ HandleModeBtnClick`     | `(string modeName) : void` | Lógica de 3 estados: 1) Si modo activo con sub-panel → cierra sub-panel, vuelve a grid. 2) Si modo activo sin sub-panel → desactiva modo. 3) Si modo inactivo → activa, desactiva anterior. |
| `- ActivateMode`           | `(string mode) : void`     | Muestra container del modo. Oculta los otros. Actualiza estilos de botones activos (CSS class `mode-active`). Llama `SyncAppState()`.                                                       |
| `- DeactivateMode`         | `() : void`                | Oculta todos los containers. Limpia estilos. Cambia a `AppState.Exploration`.                                                                                                               |
| `- SyncAppState`           | `() : void`                | Mapea modo UI → AppState: "tools"→Exploration, "analyze"→Analyze, "studio"→Studio.                                                                                                          |
| `- SyncWithAppState`       | `(AppState state) : void`  | Inverso: si el AppState cambia externamente (e.g., por teclado), actualiza la UI para reflejar el modo correcto.                                                                            |
| `- HandleToolCardClick`    | `(string cardName) : void` | Para modo Tools: toggle cards INFO (muestra sheet), PINS (toggle hotspots), ISOLATE (toggle visibilidad).                                                                                   |
| `- HandleAnalyzeCardClick` | `(string cardName) : void` | Para modo Analyze: abre sub-panel correspondiente (CrossSection, Explode, Filter).                                                                                                          |
| `- SetupCategoryFilters`   | `() : void`                | Registra click handlers para botones de categoría (ALL, STRUCTURE, PROPULSION, AVIONICS, POWER). Llama `ExplodedViewManager.SetCategoryFilters()`.                                          |
| `+ Dispose`                | `() : void`                | Ejecuta cleanup actions, desuscribe eventos.                                                                                                                                                |

---

### 10.2 `UIDetailsSheet.cs` — Bottom Sheet de Detalles

**Ruta:** `Assets/Scripts/UI/UIDetailsSheet.cs` | **Líneas:** 354  
**Namespace:** `WebGL.UI`

#### Clase `UIDetailsSheet` (clase C# pura) [—]

| Campo           | Tipo            | Descripción                                                                                                                                               |
| --------------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `- _sheet`      | `VisualElement` | Elemento `BottomSheet` del UXML                                                                                                                           |
| `- _handle`     | `VisualElement` | Handle de drag                                                                                                                                            |
| `- _camera`     | `Camera`        | Para viewport shift                                                                                                                                       |
| `- _isOpen`     | `bool`          | Estado actual                                                                                                                                             |
| `- _dragStartY` | `float`         | Y al inicio del drag                                                                                                                                      |
| `- _isDragging` | `bool`          | En proceso de drag                                                                                                                                        |
| `- 12 labels`   | `Label`         | PartName, PartCategory, PartFunction, PartMaterial, PartWeight, PartDimensions, PartPower, PartTemp, PartDifficulty, PartTools, PartTime, PartDescription |

| Método                  | Firma                         | Descripción                                                                                                                                           |
| ----------------------- | ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `+ Open`                | `() : void`                   | Añade CSS class `sheet-open`. `InputManager.InputBlocked = true`. `OrbitCameraController.SetViewportShift(0.15f)`.                                    |
| `+ Close`               | `() : void`                   | Quita CSS class. `InputBlocked = false`. `SetViewportShift(0)`.                                                                                       |
| `+ PopulatePartData`    | `(DronePartData data) : void` | Asigna los 12 labels desde los campos de `data`. Formatea peso (g), dimensiones (mm), potencia (W), temperatura (°C), dificultad (★/☆), tiempo (min). |
| `- OnHandlePointerDown` | `(PointerDownEvent) : void`   | Captura pointer, almacena `_dragStartY`.                                                                                                              |
| `- OnHandlePointerMove` | `(PointerMoveEvent) : void`   | Calcula deltaY. Si deltaY > 50px → dismiss (cierra el sheet).                                                                                         |
| `- OnHandlePointerUp`   | `(PointerUpEvent) : void`     | Finaliza drag. Si el sheet estaba cerrado y deltaY < −50 → swipe-up-to-open.                                                                          |
| `- OnHeaderClick`       | `(ClickEvent) : void`         | Toggle open/close al hacer click en el header.                                                                                                        |
| `- OnScrollViewZoom`    | `(WheelEvent) : void`         | `evt.StopPropagation()` — previene que el scroll del sheet active el zoom de la cámara.                                                               |

---

### 10.3 `UIHeroController.cs` — Pantalla de Inicio

**Ruta:** `Assets/Scripts/UI/UIHeroController.cs` | **Líneas:** 188  
**Namespace:** `WebGL.UI`

#### Clase `UIHeroController` (clase C# pura) [—]

| Método           | Firma       | Descripción                                                                                                                                                  |
| ---------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `- BindHeroMain` | `() : void` | Registra clicks para EXPLORE (→ `DismissHero`), SELECT DEVICE (→ show devices submenu), ABOUT (→ show about), EXIT (→ show exit confirmation).               |
| `+ DismissHero`  | `() : void` | Oculta `HeroContainer`, establece `pickingMode = Ignore` para dejar pasar input al 3D. `AppStateMachine.EnterExploration()`.                                 |
| `+ ReturnToHero` | `() : void` | Muestra `HeroContainer`, restablece `pickingMode`. Cierra todos los sub-menús. Resetea cámara y modo de visualización. `AppStateMachine.ChangeState(Intro)`. |
| `- HandleExit`   | `() : void` | WebGL: `Application.ExternalEval("window.history.back()")`. Editor: `EditorApplication.isPlaying = false`.                                                   |

---

### 10.4 `UIAnalyzePanel.cs` — Panel de Modos Shader

**Ruta:** `Assets/Scripts/UI/UIAnalyzePanel.cs` | **Líneas:** 102  
**Namespace:** `WebGL.UI`

| Método                | Firma                    | Descripción                                                                                                                                      |
| --------------------- | ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `- BindButtons`       | `() : void`              | Registra 7 botones (`ShaderMode_Realistic` ... `ShaderMode_Thermal`) → cada uno llama `ViewModeManager.SetMode(mode)`.                           |
| `- OnViewModeChanged` | `(ViewMode mode) : void` | Actualiza botón activo (CSS class `shader-btn-active`). Cambia color de fondo del panel según modo (azul para Blueprint, verde para XRay, etc.). |

---

### 10.5 `UICrossSectionPanel.cs` — Panel de Corte Transversal

**Ruta:** `Assets/Scripts/UI/UICrossSectionPanel.cs` | **Líneas:** 249  
**Namespace:** `WebGL.UI`

| Campo                | Tipo        | Descripción                                |
| -------------------- | ----------- | ------------------------------------------ |
| `- _axisButtons`     | `Button[3]` | Botones X, Y, Z                            |
| `- _positionSliders` | `Slider[2]` | Sliders para posición de cada plano activo |
| `- _invertButtons`   | `Button[2]` | Botones de inversión                       |

| Método                       | Firma                                  | Descripción                                                                                                                   |
| ---------------------------- | -------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| `- OnAxisClick`              | `(int axis) : void`                    | Llama `CrossSectionManager.ToggleAxis(axis)`. Actualiza estilo del botón.                                                     |
| `- OnSliderChanged`          | `(int planeIndex, float value) : void` | `CrossSectionManager.SetPlanePosition(planeIndex, value)`.                                                                    |
| `- OnInvertClick`            | `(int planeIndex) : void`              | `CrossSectionManager.InvertPlane(planeIndex)`.                                                                                |
| `- RegisterSliderInputBlock` | `(Slider s) : void`                    | `PointerDown` → `InputBlocked = true`. `PointerUp` → `InputBlocked = false`. Previene que arrastrar el slider rote la cámara. |

---

### 10.6 `UIEnvironmentPanel.cs` — Panel de Entorno

**Ruta:** `Assets/Scripts/UI/UIEnvironmentPanel.cs` | **Líneas:** 115  
**Namespace:** `WebGL.UI`

| Método          | Firma       | Descripción                                                                                                                                        |
| --------------- | ----------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| `- BindPresets` | `() : void` | 4 botones (Studio, Night, Blueprint, Neutral) → `EnvironmentController.ApplyPreset(name)`.                                                         |
| `- BindSliders` | `() : void` | Slider rotación → `EnvironmentController.SetLightRotation(value * 360)`. Slider intensidad → `SetLightIntensity(value)`. Ambos con input blocking. |

---

## 11. Capa UI — Paneles Secundarios

### 11.1 `HotspotManager.cs` [S]

`Initialize(root)` busca `ExplodablePart[]`, crea `SmartHotspot` por cada una (insertado en `WorldSpaceContainer` del UXML). `LateUpdate` llama `hotspot.Update()` en cada frame para tracking 3D→2D.

### 11.2 `SmartHotspot.cs` [—]

Clase C# pura (no MonoBehaviour). Crea un dot VisualElement + name label. Cada `Update()`: frustum check → occlusion raycast staggered (cada 8 frames, con offset aleatorio para distribuir carga) → `RuntimePanelUtils.ScreenToPanel` → smooth `Lerp` → style position. Click → `SelectionManager.SelectObject(target, fromHotspot: true)`.

### 11.3 `AssemblyStepUI.cs` [S]

Panel lateral derecho (380px) programático con: step badge, título, instrucciones, info cards (tools, time), difficulty stars (★/☆), warnings, tips, progress bar, navegación (Prev/Next/Complete). Observer de `AssemblyGuideManager` eventos. Animaciones con `TweenEngine`.

### 11.4 `PartCatalogUI.cs` [S]

Panel izquierdo (280px) programático con: TextField búsqueda, pill buttons de categoría (ALL, Structure, Propulsion, Electronics, Sensors, Other), contador de resultados, ScrollView de partes (icono + nombre + tipo + toggle visibilidad 👁). Click → `FocusPart()`. Observer de `PartCatalogManager.OnFilterChanged`.

### 11.5 `EnhancedInfoPanel.cs` [S]

Panel derecho (350px): header, secciones Overview y Materials, 3 action buttons (Hide → `HidePart`, Isolate → `IsolatePart`, Focus → `SetTarget + SetDistance`). Observer de `PartSelectedEvent`.

### 11.6 `SettingsPanel.cs` [S]

Modal centrado (400px): 3 sliders audio + 1 slider UI Scale + 2 toggles (High Contrast, Reduced Motion) + Close. Integra `AudioManager`, `AccessibilityManager`, `SaveSystem`.

### 11.7 `LoadingController.cs` [PS]

Fullscreen overlay con barra de progreso (cyan), status label, porcentaje. Progreso suavizado con `Lerp` en Update.

### 11.8 `SceneTransitionManager.cs` [PS]

Overlay fullscreen fade negro. `FadeOut(callback)` → opacidad 0→1. `FadeIn(callback)` → 1→0. `CrossFade(midpoint, complete)` → FadeOut → callback → FadeIn. Guard `isTransitioning`.

### 11.9 `TooltipSystem.cs` [PS]

Tooltip flotante que sigue `Input.mousePosition` (convertida a panel coords). `Show(text)` / `Hide()`.

### 11.10 `DetailsPanelController.cs` [—]

Controller UXML-driven: queries elementos por nombre en UXML existente (`DetailsPanel`, `PartName`, `PartDescription`). Observer de `PartSelectedEvent`. Versión ligera alternativa a `EnhancedInfoPanel`.

### 11.11 `UIAnimator.cs` [S]

Utilidad estática: `FadeIn(element, duration)` y `FadeOut(element, duration)` via corrutinas con lerp de opacidad. Complementa las CSS transitions que no siempre funcionan en UI Toolkit WebGL.

### 11.12 `UIConstants.cs` (Auto-generado)

Clase estática con constantes string para nombres de UXML elements: `AppContainer`, `DetailsPanel`, `ExitBtn`, `ExplosionSlider`, etc. Generado por "Antigravity UI Validator Skill" para evitar magic strings.

---

## 12. Capa UI — Sistemas Transversales

### 12.1 `MainLayout.uxml` — Layout Declarativo Principal

**Ruta:** `Assets/Scripts/UI/Layouts/MainLayout.uxml` | **Líneas:** 353  
**Referencia CSS:** `Assets/UI/Styles/Theme.uss`

**Estructura jerárquica completa:**

```
<ui:UXML>
├── Root (picking-mode="Ignore")
│   ├── WorldSpaceContainer         → Hotspots 3D→2D
│   ├── TopBar
│   │   ├── HomeBtn (⌂)
│   │   ├── TopContextLabel
│   │   └── ResetViewBtn
│   ├── BottomSheet
│   │   ├── SheetHandle
│   │   ├── SheetHeader (SheetTitle + SheetCloseBtn)
│   │   └── SheetContent_Details
│   │       ├── DataGrid
│   │       │   ├── PartCategory, PartFunction, PartMaterial
│   │       │   ├── PartWeight, PartDimensions
│   │       │   └── PartPower, PartTemp
│   │       ├── AssemblySection
│   │       │   └── PartDifficulty, PartTools, PartTime
│   │       └── PartDescription
│   ├── BottomBar
│   │   ├── ToolsModeContainer (INSPECT)
│   │   │   └── Cards: InfoCard, PinsCard, IsolateCard
│   │   ├── AnalyzeModeContainer (ANALYZE)
│   │   │   ├── Cards: CutCard, ExplodeCard, FilterCard
│   │   │   └── Sub-paneles:
│   │   │       ├── CrossSectionPanel (ejes + sliders + inversión)
│   │   │       ├── ExplodeSubPanel (ExplosionSlider 0–1)
│   │   │       └── FilterSubPanel (categorías)
│   │   ├── StudioModeContainer (STUDIO)
│   │   │   ├── Shader modes (7 botones)
│   │   │   ├── Environment presets (4 botones)
│   │   │   └── Sliders (rotación + intensidad)
│   │   └── Mode buttons row
│   │       ├── ModeToolsBtn (INSPECT)
│   │       ├── ModeAnalyzeBtn (ANALYZE)
│   │       └── ModeStudioBtn (STUDIO)
│   └── HeroContainer
│       ├── HeroMain (EXPLORE, SELECT DEVICE, ABOUT, EXIT)
│       ├── HeroSubmenu_Devices (Drone✓, Robot✗, Satellite✗)
│       ├── HeroSubmenu_About (info tesis 2026)
│       └── HeroSubmenu_Exit (confirmación)
</ui:UXML>
```

**`picking-mode="Ignore"` en Root:** El root pasa los clicks al 3D por defecto. Solo los hijos con `picking-mode="Position"` (elementos visibles como botones, paneles) interceptan el input. Esto elimina la necesidad de raycasts manuales para determinar si el click es para la UI o para el 3D.

---

## 13. Archivos Deprecados

### 13.1 `EngineerToolbar.cs` — [Obsolete]

Toolbar circular "🔧" con dropdown de 8 herramientas. Reemplazado por el sistema de 3 modos (Phase 2). Mantenido por compatibilidad pero marcado `[Obsolete("Use el sistema de 3 modos")]`. No se instancia en runtime.

### 13.2 `ViewModeToolbar.cs` — [Obsolete]

Barra inferior con 7 botones de modo + 4 herramientas. Reemplazado por `UIModeController` + `UIAnalyzePanel`. Incluía un `OnResetClicked()` que reseteaba completamente la aplicación (vista, cross-section, visibilidad, cámara, selección, estado).

---

_Documento generado como parte de la documentación técnica del proyecto de grado._  
_Rama: `feature/phase2-ux-redesign` — Febrero 2026_
