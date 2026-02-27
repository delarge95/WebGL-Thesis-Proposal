# Documento 1 — Arquitectura del Sistema

## Aplicación Web Interactiva para Visualización 3D de Drones  
### Documentación Técnica de Nivel de Maestría

**Proyecto de Grado:** Ingeniería Multimedia — UNAD  
**Plataforma:** Unity 6.0 LTS → WebGL (IL2CPP → WebAssembly)  
**Render Pipeline:** Universal Render Pipeline (URP)  
**UI Framework:** UI Toolkit (UIElements, USS, UXML)  
**Rama:** `feature/phase2-ux-redesign`  
**Repositorio:** `delarge95/WebGL-Thesis-Proposal`

---

## Tabla de Contenidos

1. [Visión General de la Arquitectura](#1-visión-general-de-la-arquitectura)
2. [Stack Tecnológico y Justificación](#2-stack-tecnológico-y-justificación)
3. [Diagrama de Capas](#3-diagrama-de-capas)
4. [Patrón Singleton — Jerarquía de 3 Niveles](#4-patrón-singleton--jerarquía-de-3-niveles)
5. [Bus de Eventos (EventBus)](#5-bus-de-eventos-eventbus)
6. [Máquina de Estados de la Aplicación (AppStateMachine)](#6-máquina-de-estados-de-la-aplicación-appstatemachine)
7. [Secuencia de Inicialización (SceneBootstrapper)](#7-secuencia-de-inicialización-scenebootstrapper)
8. [Sistema de Entrada Unificado (InputManager)](#8-sistema-de-entrada-unificado-inputmanager)
9. [Flujo de Datos — Selección de Parte](#9-flujo-de-datos--selección-de-parte)
10. [Comunicación entre Módulos](#10-comunicación-entre-módulos)
11. [Gestión de Materiales y Shaders](#11-gestión-de-materiales-y-shaders)
12. [Arquitectura de la Interfaz de Usuario](#12-arquitectura-de-la-interfaz-de-usuario)
13. [Pipeline de Renderizado y Optimización WebGL](#13-pipeline-de-renderizado-y-optimización-webgl)
14. [Patrones de Diseño Aplicados](#14-patrones-de-diseño-aplicados)
15. [Decisiones de Diseño y Justificaciones](#15-decisiones-de-diseño-y-justificaciones)
16. [Inventario Completo del Código Fuente](#16-inventario-completo-del-código-fuente)

---

## 1. Visión General de la Arquitectura

La aplicación es un visor interactivo 3D de drones que se ejecuta íntegramente en el navegador web mediante la compilación de Unity a WebAssembly. Su propósito es permitir a ingenieros y estudiantes explorar, analizar y estudiar la anatomía de un dron con herramientas de visualización profesional: vista explosionada, corte transversal dual, 7 modos de renderizado (shader swapping), guía de ensamblaje paso a paso, sistema de medición, anotaciones 3D, y catálogo de partes con filtrado multi-criterio.

### Principios Arquitectónicos Fundamentales

| Principio | Implementación |
|---|---|
| **Responsabilidad Única (SRP)** | Cada Manager maneja un único dominio (cámara, selección, vista, corte, etc.) |
| **Inversión de Dependencias** | Los módulos no se referencian directamente entre sí; se comunican vía `EventBus` estático |
| **Abierto/Cerrado (OCP)** | Los modos de visualización se extienden añadiendo un enum y un shader, sin modificar `ViewModeManager` |
| **Composición sobre Herencia** | Los `ExplodablePart` son MonoBehaviours composicionales con `DronePartData` (ScriptableObject) |
| **Manager Singleton** | Acceso global controlado a sistemas transversales (audio, notificaciones, errores) |
| **Arquitectura por Capas** | Core → Content → UI, con comunicación ascendente vía eventos |

### Métricas del Código Fuente

| Categoría | Archivos | Líneas Aproximadas |
|---|---:|---:|
| C# Scripts (Core + UI) | 94 | ~12,750 |
| Shaders HLSL/CG | 9 | ~1,544 |
| UXML (Layout Declarativo) | 2 | ~401 |
| USS (Estilos) | — | (referenciado pero no contabilizado) |
| **Total** | **105** | **~14,695** |

---

## 2. Stack Tecnológico y Justificación

### Motor: Unity 6.0 LTS

Se seleccionó Unity 6 (LTS) como motor principal por las siguientes razones técnicas:

1. **Compilación WebGL/IL2CPP → WASM:** Unity compila el código C# a C++ (via IL2CPP) y luego a WebAssembly, produciendo un binario que se ejecuta directamente en el navegador sin plugins. Esto es fundamental para la tesis, cuyo objetivo es la accesibilidad vía navegador.

2. **Universal Render Pipeline (URP):** URP es un pipeline configurable y liviano, diseñado para plataformas de rendimiento limitado. A diferencia de HDRP (High Definition), URP es compatible con WebGL y permite shaders personalizados con la biblioteca `ShaderLibrary/Core.hlsl`.

3. **UI Toolkit (UIElements):** A partir de Unity 2021, UI Toolkit reemplaza gradualmente a UGUI para interfaces de usuario. Utiliza un modelo declarativo (UXML) + hojas de estilo (USS) inspirado en HTML/CSS, lo que facilita la creación de interfaces responsivas y reduce el acoplamiento con el sistema de coordenadas 3D.

### Compilación WebGL y Restricciones

| Restricción WebGL | Impacto en la Arquitectura |
|---|---|
| **Sin hilos (single-threaded)** | No hay `System.Threading.Thread`; las operaciones asíncronas usan Coroutines |
| **Sin acceso a filesystem** | Persistencia limitada a `PlayerPrefs` (que mapea a `localStorage` en el navegador) |
| **Sin Geometry Shaders en WebGL 2.0** | `Wireframe.shader` incluye un SubShader fallback sin `#pragma geometry` para WebGL → `WireframeWebGL.shader` |
| **Heap limitado (~2 GB max)** | `WebGLOptimizer` monitorea el heap y fuerza `GC.Collect()` cuando supera 800 MB |
| **Sin `System.IO.File`** | `ScreenshotManager` bifurca: en Editor usa `File.WriteAllBytes`, en WebGL usa base64/JS interop |
| **No hay `Application.Quit()`** | `UIHeroController` usa `Application.ExternalEval("window.history.back()")` como alternativa |

---

## 3. Diagrama de Capas

La arquitectura sigue un modelo de 4 capas con dependencias estrictamente descendentes:

```
┌─────────────────────────────────────────────────────────────────┐
│                      CAPA DE PRESENTACIÓN (UI)                  │
│  UIManager ← UIModeController, UIDetailsSheet, UIHeroController │
│  UIAnalyzePanel, UICrossSectionPanel, UIEnvironmentPanel        │
│  HotspotManager, SmartHotspot, AssemblyStepUI, PartCatalogUI    │
│  SettingsPanel, LoadingController, NotificationManager          │
│  SceneTransitionManager, EnhancedInfoPanel, UIAnimator          │
│  MainLayout.uxml + Theme.uss                                    │
├─────────────────────────────────────────────────────────────────┤
│                      CAPA DE CONTENIDO (Content)                │
│  ExplodablePart, HighlightSystem, MaterialController            │
│  DronePartData (ScriptableObject), AssemblyGuideData            │
│  DronePartSetup, DroneAssembler, SmartHotspot                   │
├─────────────────────────────────────────────────────────────────┤
│                      CAPA DE LÓGICA (Core/Managers)             │
│  AppStateMachine, GameManager, SelectionManager                 │
│  ViewModeManager, OrbitCameraController, CrossSectionManager    │
│  ExplodedViewManager, PartVisibilityManager, InputManager       │
│  EnvironmentController, DroneStateController, AssemblyGuideManager│
│  MeasurementTool, AnnotationSystem, ModularPartsSystem          │
│  ConnectionPointsViewer, AssemblyChecklist, BillOfMaterialsManager│
│  PartCatalogManager, ScreenshotManager, KeyboardShortcuts       │
│  CameraPresets, AudioManager, CursorManager                     │
│  AnalyticsManager, ErrorHandler, QualityManager                 │
│  WebGLOptimizer, SaveSystem, AssetLoader                        │
├─────────────────────────────────────────────────────────────────┤
│                      CAPA DE INFRAESTRUCTURA                    │
│  Singleton<T> (StaticInstance → Singleton → PersistentSingleton) │
│  EventBus (publicación/suscripción tipada, thread-safe)         │
│  TweenEngine (animación procedural por Coroutines)              │
│  ObjectPooler, SceneBootstrapper                                │
│  9 Shaders HLSL personalizados (WebGL/*)                        │
├─────────────────────────────────────────────────────────────────┤
│                 PLATAFORMA (Unity 6 + URP + WebGL)              │
│  IL2CPP → WebAssembly │ URP ShaderLibrary │ UI Toolkit Runtime  │
└─────────────────────────────────────────────────────────────────┘
```

### Reglas de Dependencia

- **UI → Core:** La capa UI accede a managers vía `Singleton.Instance` y se suscribe a eventos del `EventBus`.
- **Core → Content:** Los managers referencian componentes de contenido (`ExplodablePart`, `HighlightSystem`) por tipo genérico (`FindObjectsByType<T>`).
- **Content → Data:** Los MonoBehaviours de contenido referencian `ScriptableObjects` (`DronePartData`, `AssemblyGuideData`) por serialización de Unity.
- **Infraestructura → Ninguno:** Las clases base (`Singleton`, `EventBus`, `TweenEngine`) no tienen dependencias ascendentes.
- **Prohibido:** La capa Core NO referencia directamente a la capa UI. La comunicación ascendente se realiza exclusivamente mediante eventos.

---

## 4. Patrón Singleton — Jerarquía de 3 Niveles

El archivo `Singleton.cs` (58 líneas) define una jerarquía genérica de 3 niveles que forma la columna vertebral de la gestión de instancias en toda la aplicación. Esta jerarquía es una implementación del patrón Singleton adaptada a la arquitectura de componentes de Unity (MonoBehaviour lifecycle).

### 4.1 Nivel 1: `StaticInstance<T>`

```csharp
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
```

**Comportamiento:** Si se instancia un segundo objeto con el mismo componente, el nuevo **reemplaza** al anterior como `Instance`. No destruye al anterior activamente.

**Caso de uso:** Componentes donde la instancia más reciente es siempre la válida (útil en hot-reload durante desarrollo).

### 4.2 Nivel 2: `Singleton<T>`

```csharp
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}
```

**Comportamiento:** Si ya existe una instancia, el nuevo objeto **se destruye a sí mismo**. Garantiza unicidad dentro de la escena.

**Uso en la aplicación:** La mayoría de los managers de escena: `SelectionManager`, `ViewModeManager`, `CrossSectionManager`, `ExplodedViewManager`, `AnnotationSystem`, `BillOfMaterialsManager`, `ModularPartsSystem`, `ConnectionPointsViewer`, `AssemblyChecklist`, `PartCatalogManager`, `ScreenshotManager`, `KeyboardShortcuts`, `CameraPresets`, `CursorManager`, `QualityManager`, `PerformanceMonitor`.

### 4.3 Nivel 3: `PersistentSingleton<T>`

```csharp
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
            DontDestroyOnLoad(gameObject);
    }
}
```

**Comportamiento:** Además de la unicidad de `Singleton<T>`, marca el GameObject con `DontDestroyOnLoad`, haciéndolo persistente entre cambios de escena.

**Uso en la aplicación:** Managers que deben sobrevivir transiciones: `GameManager`, `InputManager`, `AudioManager`, `NotificationManager`, `AccessibilityManager`, `ErrorHandler`, `WebGLOptimizer`, `AnalyticsManager`, `ObjectPooler`, `RuntimeConsole`, `TweenEngine`, `LoadingController`, `TooltipSystem`, `SceneTransitionManager`.

### 4.4 Justificación del Patrón

| Alternativa Considerada | Razón de Descarte |
|---|---|
| **Service Locator** | Mayor complejidad sin beneficio: la app tiene escena única, no requiere resolución dinámica de servicios |
| **Dependency Injection (Zenject/VContainer)** | Overhead excesivo para WebGL; aumenta el tamaño del .wasm compilado |
| **Acceso por `FindObjectOfType`** | Rendimiento inaceptable: O(n) por búsqueda vs O(1) de campo estático |
| **Propiedades estáticas sin MonoBehaviour** | No participan del lifecycle de Unity (Awake, Update, OnDestroy); no pueden usar Coroutines |

La jerarquía de 3 niveles ofrece el equilibrio correcto entre accesibilidad global (`Instance`), seguridad de unicidad (destrucción de duplicados), y persistencia selectiva (`DontDestroyOnLoad`), todo dentro del modelo de componentes de Unity.

---

## 5. Bus de Eventos (EventBus)

El `EventBus` (170 líneas) implementa el patrón **Publicar/Suscribir** (Publish/Subscribe) con tipado genérico, permitiendo comunicación desacoplada entre módulos sin referencias directas.

### 5.1 Estructura Interna

```csharp
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers 
        = new Dictionary<Type, List<Delegate>>();
    private static readonly object _lock = new object();
}
```

- **Clave del diccionario:** `typeof(T)` — cada tipo de evento tiene su propia lista de suscriptores.
- **Valor:** `List<Delegate>` — almacena `Action<T>` casteadas a `Delegate` para tipo-seguridad en tiempo de ejecución.
- **`_lock`:** Objeto de sincronización para thread-safety (por precaución, aunque Unity WebGL es single-threaded).

### 5.2 API Pública

| Método | Firma | Descripción |
|---|---|---|
| `Subscribe` | `Subscribe<T>(Action<T> callback)` | Agrega un callback a la lista de suscriptores para el tipo `T`. Verifica no-duplicados. |
| `Unsubscribe` | `Unsubscribe<T>(Action<T> callback)` | Remueve el callback de la lista. Limpia listas vacías. |
| `Publish` | `Publish<T>(T eventData)` | Notifica a todos los suscriptores de `T` con los datos del evento. |
| `Clear` | `Clear()` | Limpia todos los suscriptores de todos los tipos. |
| `ClearEvent` | `ClearEvent<T>()` | Limpia suscriptores de un tipo específico. |
| `HasSubscribers` | `HasSubscribers<T>() : bool` | Verifica si hay suscriptores registrados para `T`. |
| `GetSubscriberCount` | `GetSubscriberCount<T>() : int` | Retorna la cantidad de suscriptores para `T`. |

### 5.3 Mecanismo de Publicación (Detalle)

```csharp
public static void Publish<T>(T eventData)
{
    List<Delegate> snapshotList;
    lock (_lock)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var subs)) return;
        snapshotList = new List<Delegate>(subs); // COPIA antes de iterar
    }
    
    foreach (var subscriber in snapshotList)
    {
        try
        {
            ((Action<T>)subscriber)(eventData);
        }
        catch (Exception e)
        {
            Debug.LogError($"EventBus error: {e.Message}");
        }
    }
}
```

**Decisiones de diseño críticas:**

1. **Copy-before-iterate:** Se copia la lista de suscriptores antes de iterar para prevenir `InvalidOperationException` si un suscriptor se desuscribe durante la publicación (patrón defensivo estándar en Observer).

2. **try/catch por suscriptor:** Un error en un suscriptor NO impide la ejecución de los demás. Esto evita que un bug en un módulo UI derrumbe toda la cadena de eventos.

3. **Lock granular:** El `lock` solo protege el acceso al diccionario, no la invocación de callbacks (que ocurre fuera del lock), evitando deadlocks.

### 5.4 Tipos de Eventos Definidos

| Evento | Campos | Publicado por | Consumido por |
|---|---|---|---|
| `PartSelectedEvent` | `DronePartData Data`, `bool FromHotspot` | `SelectionManager`, `PartCatalogManager` | `UIManager`, `UIDetailsSheet`, `EnhancedInfoPanel`, `DetailsPanelController`, `AnalyticsManager` |
| `AppStateChangedEvent` | `AppState NewState` | `AppStateMachine` | `UIManager`, `UIModeController`, `ExplodedViewManager` |
| `ViewModeChangedEvent` | `bool IsExploded` | (No confirmado en uso activo) | — |
| `StateChangedEvent` | `AppState PreviousState`, `AppState NewState` | `AppStateMachine` | (Disponible para suscripción) |

### 5.5 Comparativa con Alternativas

| Alternativa | Desventaja vs EventBus Implementado |
|---|---|
| **UnityEvent (Inspector)** | No tipado genéricamente; requiere wiring manual en el Inspector; serialize overhead |
| **C# events directos** | Acoplan emisor y suscriptor (el suscriptor necesita referencia al emisor) |
| **MessagePipe / UniRx** | Dependencias externas que aumentan el tamaño del build WebGL |
| **ScriptableObject Events** | Assets en disco que complican el versionado; no ofrecen type-safety en compilación |

---

## 6. Máquina de Estados de la Aplicación (AppStateMachine)

La `AppStateMachine` (243 líneas) controla los 9 modos operativos de la aplicación mediante un enum `AppState` y una tabla de transiciones válidas, implementando el patrón **Finite State Machine (FSM)**.

### 6.1 Estados Definidos

```csharp
public enum AppState
{
    Loading,       // 0 — Carga inicial de assets
    Intro,         // 1 — Pantalla Hero/Landing
    Exploration,   // 2 — Navegación libre 3D
    ExplodedView,  // 3 — Vista explosionada activa
    FocusMode,     // 4 — Enfoque en parte individual
    Settings,      // 5 — Panel de configuración
    Menu,          // 6 — Menú principal
    Analyze,       // 7 — Modo análisis (shaders, corte)
    Studio         // 8 — Modo estudio (entorno, iluminación)
}
```

### 6.2 Tabla de Transiciones

La tabla se define en un `Dictionary<AppState, AppState[]>` inicializado en el constructor. Solo las transiciones explícitamente listadas son permitidas:

| Estado Origen | Transiciones Permitidas |
|---|---|
| `Loading` | → `Intro` |
| `Intro` | → `Exploration` |
| `Exploration` | → `ExplodedView`, `FocusMode`, `Settings`, `Menu`, `Analyze`, `Studio` |
| `ExplodedView` | → `Exploration`, `FocusMode`, `Settings` |
| `FocusMode` | → `Exploration`, `ExplodedView` |
| `Settings` | → `Exploration`, `ExplodedView`, `Menu` |
| `Menu` | → `Exploration`, `Settings`, `Intro` |
| `Analyze` | → `Exploration`, `Settings` |
| `Studio` | → `Exploration`, `Settings` |

**Propiedad clave:** `IsInteractive()` retorna `true` solo si el estado actual es `Exploration`, `ExplodedView`, `FocusMode`, `Analyze` o `Studio`. El `InputManager` y los atajos de teclado consultan esta propiedad antes de procesar entrada del usuario.

### 6.3 Mecanismo de Cambio de Estado

```csharp
public bool ChangeState(AppState newState)
{
    if (CurrentState == newState) return false;
    if (!CanTransitionTo(newState)) return false;

    AppState previousState = CurrentState;
    CurrentState = newState;

    // PUBLICACIÓN DUAL de eventos
    OnStateChanged?.Invoke(CurrentState);                          // C# event directo
    EventBus.Publish(new StateChangedEvent(previousState, newState)); // EventBus tipado
    EventBus.Publish(new AppStateChangedEvent(newState));             // EventBus simplificado

    return true;
}
```

**Publicación dual:** Se publica tanto un evento C# directo (`OnStateChanged`) como dos eventos del `EventBus`. Esto permite que los módulos que ya tienen referencia a `AppStateMachine` (como `UIModeController`) usen el event directo (más eficiente), mientras que módulos remotos (como `ExplodedViewManager`) usen el EventBus sin acoplar.

### 6.4 Métodos de Conveniencia

La máquina de estados expone métodos con nombres semánticos que encapsulan las transiciones más comunes:

```csharp
public void EnterExploration() => ChangeState(AppState.Exploration);
public void EnterAnalyze()     => ChangeState(AppState.Analyze);
public void EnterStudio()      => ChangeState(AppState.Studio);
public void EnterFocusMode()   => ChangeState(AppState.FocusMode);
public void EnterExplodedView()=> ChangeState(AppState.ExplodedView);
public void OpenSettings()     => ChangeState(AppState.Settings);
public void OpenMenu()         => ChangeState(AppState.Menu);
```

Estos métodos simplifican el código cliente al ocultar los detalles de la transición y la validación.

---

## 7. Secuencia de Inicialización (SceneBootstrapper)

`SceneBootstrapper` (174 líneas) es el primer script en ejecutarse (via `[DefaultExecutionOrder(-100)]`) y asegura que los 18 managers requeridos existan en la escena antes de que cualquier otro sistema comience a operar.

### 7.1 Proceso de Bootstrap

```
[Awake con Order -100]
    │
    ├── ValidateManagers()
    │   ├── EnsureManager<GameManager>()
    │   ├── EnsureManager<InputManager>()
    │   ├── EnsureManager<AppStateMachine>()
    │   ├── EnsureManager<SelectionManager>()
    │   ├── EnsureManager<ViewModeManager>()
    │   ├── EnsureManager<OrbitCameraController>()
    │   ├── EnsureManager<CrossSectionManager>()
    │   ├── EnsureManager<ExplodedViewManager>()
    │   ├── EnsureManager<PartVisibilityManager>()
    │   ├── EnsureManager<EnvironmentController>()
    │   ├── EnsureManager<DroneStateController>()
    │   ├── EnsureManager<AnnotationSystem>()
    │   ├── EnsureManager<ConnectionPointsViewer>()
    │   ├── EnsureManager<AudioManager>()
    │   ├── EnsureManager<NotificationManager>()
    │   ├── EnsureManager<ErrorHandler>()
    │   ├── EnsureManager<AnalyticsManager>()
    │   └── EnsureManager<WebGLOptimizer>()
    │
    └── LogSystemStatus()  // Log confirmando N managers activos
```

### 7.2 `EnsureManager<T>()`

```csharp
private void EnsureManager<T>() where T : MonoBehaviour
{
    if (FindAnyObjectByType<T>() == null)
    {
        var go = new GameObject($"[{typeof(T).Name}]");
        go.AddComponent<T>();
        Debug.LogWarning($"SceneBootstrapper: Auto-created missing {typeof(T).Name}");
    }
}
```

**Filosofía:** "Fail-safe, not fail-fast." Si un manager no existe en la escena (el artista o diseñador no lo colocó), el bootstrapper lo crea automáticamente en un GameObject vacío. Esto evita `NullReferenceException` durante la ejecución, pero registra un warning para que el equipo corrija la omisión.

### 7.3 Justificación del `DefaultExecutionOrder(-100)`

Unity ejecuta los métodos `Awake()` en orden no determinístico por defecto. Al asignar `-100` al bootstrapper, se garantiza que ejecute antes que cualquier otro `Awake()`, estableciendo las dependencias antes de que otros sistemas intenten acceder a `Singleton.Instance`.

---

## 8. Sistema de Entrada Unificado (InputManager)

`InputManager` (145 líneas) es un `PersistentSingleton` que actúa como **único punto de verdad** para el bloqueo de entrada 3D, resolviendo un problema común en aplicaciones con superposición UI+3D.

### 8.1 Problema Resuelto

En una aplicación donde la interfaz 2D se superpone a una escena 3D, los clicks del usuario pueden ser consumidos simultáneamente por la UI y por el sistema de raycast 3D. Sin un sistema centralizado, cada manager tendría que verificar independientemente si el click fue "para la UI" o "para el 3D", generando lógica duplicada y bugs difíciles de reproducir.

### 8.2 Mecanismo de Bloqueo

```csharp
// Bandera estática — single source of truth
public static bool InputBlocked { get; set; }

// Detección de puntero sobre UI (compatible con UI Toolkit + ScaleWithScreenSize)
public bool IsPointerOverUI()
{
    if (_panel == null) CachePanel();
    if (_panel == null) return false;

    Vector2 screenPos = Input.mousePosition;
    screenPos.y = Screen.height - screenPos.y;  // Flip Y para UI Toolkit
    Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_panel, screenPos);
    VisualElement picked = _panel.Pick(panelPos);

    return picked != null && picked != _panel.visualTree;
}
```

**Detalle técnico:** UI Toolkit usa un sistema de coordenadas con Y invertido respecto a `Input.mousePosition` de Unity. La conversión `screenPos.y = Screen.height - screenPos.y` seguida de `RuntimePanelUtils.ScreenToPanel()` produce coordenadas panel-space correctas. El `Pick()` devuelve el elemento más profundo bajo el puntero; si es `null` o el `visualTree` raíz (que tiene `picking-mode="Ignore"`), el puntero está sobre espacio 3D vacío.

### 8.3 `IsDragging3D`

```csharp
public bool IsDragging3D { get; private set; }

void Update()
{
    // Solo comienza drag 3D si el click empezó fuera de la UI
    if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
        IsDragging3D = true;
    if (Input.GetMouseButtonUp(1))
        IsDragging3D = false;
}
```

Esta propiedad permite al `OrbitCameraController` saber si debe procesar la rotación orbital. Si el usuario empezó el drag sobre un botón de la UI, `IsDragging3D` será `false` y la cámara no rotará.

### 8.4 Integración con Módulos

Los siguientes sistemas consultan `InputManager` antes de procesar entrada:

- `SelectionManager` → `if (InputBlocked || IsPointerOverUI()) return;` en el raycast
- `OrbitCameraController` → `if (InputBlocked) return;` en la rotación/zoom
- `KeyboardShortcuts` → `if (InputBlocked) return;` en el Update
- `MeasurementTool` → `if (InputBlocked) return;` en el placement de puntos
- `UIDetailsSheet` → Establece `InputBlocked = true` en `PointerEnter`, `false` en `PointerLeave`
- `UICrossSectionPanel` → Establece `InputBlocked = true` en sliders durante arrastre
- `UIEnvironmentPanel` → Establece `InputBlocked = true` en sliders

---

## 9. Flujo de Datos — Selección de Parte

El flujo más complejo de la aplicación es la selección de una parte del dron, que involucra 6 sistemas en cadena:

```
[Usuario hace Click en 3D]
    │
    ▼
SelectionManager.HandleClick()
    ├── InputManager.InputBlocked? → ABORT
    ├── InputManager.IsPointerOverUI()? → ABORT
    ├── Physics.Raycast(ray, layerMask)
    │   └── Hit? → ExplodablePart component?
    │       ├── NO → DeselectCurrentPart()
    │       └── YES ──┐
    │                  ▼
    ├── SWAP SELECTION (sin flicker de deselección)
    │   ├── previousPart.HighlightSystem.OnDeselect()
    │   └── newPart.HighlightSystem.OnSelect()
    │
    └── EventBus.Publish(new PartSelectedEvent(data, fromHotspot: false))
            │
            ├──→ UIManager.OnPartSelected()
            │       ├── UIDetailsSheet.PopulatePartData(data)
            │       └── UIDetailsSheet.Open()
            │
            ├──→ EnhancedInfoPanel.OnPartSelected()
            │       └── ShowPartInfo(explodablePart, data)
            │
            ├──→ AnalyticsManager.TrackPartSelected(data.partName)
            │
            └──→ DetailsPanelController.OnPartSelected()
                    └── UpdateDetails(data)
```

### Doble-Click (Isolate + Focus)

El `UIManager` implementa detección de doble-click con un umbral de 0.35 segundos:

```csharp
if (Time.time - _lastClickTime < 0.35f && _lastClickedData == evt.Data)
{
    // DOUBLE CLICK → Aislar parte + abrir sheet
    PartVisibilityManager.Instance.IsolatePart(explodablePart);
    OpenDetailsSheet(evt.Data);
}
_lastClickTime = Time.time;
_lastClickedData = evt.Data;
```

### Click desde Hotspot

Cuando el usuario hace click en un `SmartHotspot` (UI 2D), el flujo es similar pero con `fromHotspot: true`:

```
SmartHotspot.OnClick()
    └── SelectionManager.SelectObject(target, fromHotspot: true)
        └── (mismo flujo que arriba, pero sin raycast)
```

---

## 10. Comunicación entre Módulos

La aplicación utiliza 3 mecanismos de comunicación, cada uno seleccionado según el caso de uso:

### 10.1 EventBus (Desacoplamiento Total)

**Cuándo:** Comunicación entre capas (Core → UI), o entre managers sin relación directa.

```
SelectionManager ──[PartSelectedEvent]──→ UIDetailsSheet
AppStateMachine  ──[AppStateChangedEvent]──→ ExplodedViewManager
PartCatalogManager ──[PartSelectedEvent]──→ UIManager
```

### 10.2 Eventos C# Directos (Acoplamiento Controlado)

**Cuándo:** Un manager tiene pocos suscriptores conocidos y la relación es 1:N estable.

```
ViewModeManager.OnModeChanged ──→ UIAnalyzePanel.OnViewModeChanged()
ViewModeManager.OnModeChanged ──→ CrossSectionManager.OnViewModeChanged()
AssemblyGuideManager.OnStepChanged ──→ AssemblyStepUI.UpdateUI()
PartCatalogManager.OnFilterChanged ──→ PartCatalogUI.RefreshPartsList()
```

### 10.3 Acceso Directo vía Singleton.Instance

**Cuándo:** Operaciones imperativas punto-a-punto donde el EventBus sería overhead innecesario.

```
UIHeroController → OrbitCameraController.Instance.ResetView()
KeyboardShortcuts → CameraPresets.Instance.ApplyPreset(index)
UIDetailsSheet → InputManager.InputBlocked = true
UICrossSectionPanel → CrossSectionManager.Instance.SetPlanePosition()
```

### 10.4 Diagrama de Dependencias Principales

```
                    ┌────────────────────┐
                    │  AppStateMachine   │
                    └────────┬───────────┘
                             │ StateChangedEvent
              ┌──────────────┼──────────────────┐
              ▼              ▼                   ▼
     ┌─────────────┐  ┌──────────┐     ┌───────────────┐
     │  UIManager   │  │ExplodedView│    │UIModeController│
     └──────┬──────┘  │ Manager   │    └───────────────┘
            │          └──────────┘
            │ PartSelectedEvent                    
     ┌──────┴──────┐                    
     ▼             ▼                    
┌──────────┐ ┌──────────────┐         
│UIDetails │ │EnhancedInfo  │         
│Sheet     │ │Panel         │         
└──────────┘ └──────────────┘         

    ┌────────────────┐
    │ViewModeManager │
    └───────┬────────┘
            │ OnModeChanged (C# event)
     ┌──────┴──────┐
     ▼             ▼
┌──────────┐ ┌────────────────┐
│UIAnalyze │ │CrossSection    │
│Panel     │ │Manager         │
└──────────┘ └────────────────┘
```

---

## 11. Gestión de Materiales y Shaders

El sistema de visualización permite alternar entre 7 modos de renderizado en tiempo real, cada uno con un shader HLSL personalizado. La arquitectura de materiales tiene 3 niveles de abstracción:

### 11.1 Jerarquía de Control de Materiales

```
ViewModeManager (global)
    │
    ├── CacheRenderers() 
    │   └── Almacena originalMaterials[Renderer] = material
    │
    ├── SetMode(ViewMode) → CreateDefaultMaterials()
    │   ├── Shader.Find("WebGL/XRay")   → Material temporal
    │   ├── Shader.Find("WebGL/Blueprint") → Material temporal
    │   ├── ... (uno por modo)
    │   └── Asigna sharedMaterial a cada Renderer
    │
    └── MaterialPropertyBlock
        └── SetColor("_BaseColor", colorPorCategoria)
            └── Per-renderer override sin instanciar material

CrossSectionManager (modo Realistic)
    │
    ├── SwapToClippable()
    │   └── Para cada Renderer:
    │       1. Lee PBR properties del material actual (baseMap, normal, metallic...)
    │       2. Crea material con shader "WebGL/ClippableLit"
    │       3. Copia TODAS las properties PBR al nuevo material
    │       4. Asigna como sharedMaterial
    │
    └── RestoreOriginals()
        └── Restaura materiales originales cacheados

MaterialController (per-part)
    │
    ├── SetColor(color) → MaterialPropertyBlock local
    ├── ToggleXRay() → Swap de sharedMaterial ↔ xrayMaterial
    └── ResetProperties() → Limpia PropertyBlock
```

### 11.2 MaterialPropertyBlock vs Material Instance

Unity ofrece dos formas de modificar la apariencia de un Renderer sin afectar a otros que comparten el mismo material:

1. **Material Instance (`renderer.material`):** Crea una copia del material en memoria. Cada instancia consume GPU memory para su textura de propiedades.

2. **`MaterialPropertyBlock`:** Override por renderer que se inyecta en el draw call sin crear una copia del material. Cero allocations en heap.

La aplicación usa `MaterialPropertyBlock` para:
- Colores de categoría en `ViewModeManager` (7 categorías × N renderers)
- Highlight de hover/select en `HighlightSystem` (emission + base color pulsante)
- Fade de opacidad en `PartVisibilityManager` (alpha con smoothstep)

Y usa material instances (asignación a `sharedMaterial`) solo cuando el **shader completo** cambia (cambio de modo de visualización o swap a `ClippableLit`).

### 11.3 Sistema de Clipping Global

Todos los 9 shaders personalizados comparten un bloque común de variables globales establecidas por `CrossSectionManager`:

```hlsl
// Declarado en cada shader (NO en CBUFFER — es propiedad global)
float4 _GlobalClipPlane;     // Normal (xyz) + Distance (w)
float  _GlobalClipEnabled;   // 0 o 1
float4 _GlobalClipPlane2;    // Segundo plano de corte
float  _GlobalClipEnabled2;
```

Estas variables se establecen mediante `Shader.SetGlobalVector()` y `Shader.SetGlobalFloat()` en C#, lo que las inyecta en **todos** los shaders activos simultáneamente, sin necesidad de iterar sobre materiales.

---

## 12. Arquitectura de la Interfaz de Usuario

### 12.1 Modelo de UI: Mixto UXML + Programático

La interfaz combina dos enfoques:

| Enfoque | Archivos | Uso |
|---|---|---|
| **Declarativo (UXML)** | `MainLayout.uxml` (353 líneas) | Layout principal: Hero, TopBar, BottomSheet, BottomBar con 3 modos, paneles de sub-herramientas |
| **Programático (C#)** | Múltiples managers | Notificaciones, tooltips, loading overlay, panel de errores, assembly guide UI, part catalog UI |

### 12.2 Estructura del MainLayout.uxml

```
Root (picking-mode="Ignore")
├── WorldSpaceContainer           ← Hotspots 3D→2D
├── TopBar                        ← HomeBtn ⌂ | ContextLabel | ResetViewBtn
├── BottomSheet                   ← Panel deslizable de detalles de parte
│   ├── Handle (drag)
│   ├── Header (titulo + close)
│   └── SheetContent_Details
│       ├── DataGrid (7 campos: Category, Function, Material...)
│       ├── AssemblySection (Difficulty, Tools, Time)
│       └── Description
├── BottomBar                     ← Sistema de 3 modos
│   ├── ToolsModeContainer (INSPECT)
│   │   └── Cards: INFO, PINS, ISOLATE
│   ├── AnalyzeModeContainer (ANALYZE)
│   │   ├── Cards: CUT, EXPLODE, FILTER
│   │   └── Sub-paneles Level 2:
│   │       ├── CrossSectionPanel (ejes, sliders, inversión)
│   │       ├── ExplodeSubPanel (slider 0–1)
│   │       └── FilterSubPanel (categorías: ALL, STRUCTURE, PROPULSION...)
│   ├── StudioModeContainer (STUDIO)
│   │   ├── Shader modes (Realistic, X-Ray, Solid, Blueprint, Wire, Ghost, Thermal)
│   │   ├── Environment presets (Studio, Night, Blue, Neutral)
│   │   └── Sliders (rotación luz, intensidad)
│   └── Mode buttons (INSPECT | ANALYZE | STUDIO)
└── HeroContainer                 ← Pantalla de inicio
    ├── HeroMain (EXPLORE, SELECT DEVICE, ABOUT, EXIT)
    ├── HeroSubmenu_Devices
    ├── HeroSubmenu_About
    └── HeroSubmenu_Exit
```

### 12.3 Coordinación: UIManager como Orquestador

`UIManager` (430 líneas) es el punto de entrada de la UI. No implementa lógica de UI; **delega** a 6 sub-controladores especializados:

```csharp
// En Initialize():
_modeController = new UIModeController(root, ...);
_detailsSheet   = new UIDetailsSheet(root, camera);
_heroController = new UIHeroController(root, ...);
_analyzePanel   = new UIAnalyzePanel(root);
_crossPanel     = new UICrossSectionPanel(root);
_envPanel       = new UIEnvironmentPanel(root);
```

**Responsabilidades del UIManager:**
1. Obtener la referencia al `UIDocument` y al ROOT del Visual Tree.
2. Instanciar y cablear los sub-controladores.
3. Suscribirse a `PartSelectedEvent` y delegar a `UIDetailsSheet`.
4. Registrar el bloqueo de `InputManager.InputBlocked` en todos los botones interactivos.
5. Implementar detección de doble-click para aislamiento de parte.
6. Coordinar el cleanup (dispose) de todos los sub-controladores vía un patrón `AddCleanup`.

### 12.4 UIModeController — Sistema de 3 Modos

`UIModeController` (561 líneas) implementa la navegación modal con 2 niveles:

**Nivel 1 — Modos principales:**
- `INSPECT` (Tools): Tarjetas toggle (Info, Pins, Isolate)
- `ANALYZE`: Tarjetas navegación (Cut, Explode, Filter) → abren sub-paneles
- `STUDIO`: Panel único con shaders + presets de entorno + sliders

**Nivel 2 — Sub-paneles (solo ANALYZE):**
- `CrossSectionPanel`: Control dual-plane
- `ExplodeSubPanel`: Slider de explosión
- `FilterSubPanel`: Botones de categoría

**Máquina de estados del toggle de modo:**

```
Click ModeBtn →
  ¿Modo ya activo?
  ├── SÍ y sub-panel abierto → cerrar sub-panel, volver a grid de cards
  └── SÍ y en grid → desactivar modo completo
  └── NO → activar modo, desactivar el anterior
```

### 12.5 UIDetailsSheet — Bottom Sheet con Gestos

`UIDetailsSheet` (354 líneas) implementa un panel deslizable desde el borde inferior con:

- **Drag-to-dismiss:** Umbral de 50px de arrastre vertical hacia abajo cierra el sheet.
- **Swipe-up-to-open:** Si el sheet está cerrado y el usuario arrastra hacia arriba, se abre.
- **Input blocking:** `PointerEnter` en el sheet establece `InputManager.InputBlocked = true`, previniendo que el scroll del sheet rote la cámara 3D.
- **Viewport shift:** Al abrirse, llama `OrbitCameraController.SetViewportShift(0.15f)` para desplazar la cámara y evitar que el sheet oculte el modelo.
- **PopulatePartData:** Llena 12 labels con datos de `DronePartData` (categoría, función, material, peso, dimensiones, potencia, temperatura, dificultad, herramientas, tiempo).

---

## 13. Pipeline de Renderizado y Optimización WebGL

### 13.1 Shaders Personalizados

La aplicación define 9 shaders HLSL bajo el namespace `WebGL/*`, todos compatibles con URP y WebGL 2.0:

| Shader | Técnica Principal | Passes |
|---|---|---|
| `WebGL/ClippableLit` | PBR completo (URP `UniversalFragmentPBR`) + clip dual | Main + ShadowCaster |
| `WebGL/Blueprint` | Fresnel edge + dual world-space grid + AO simulado | Main + Outline |
| `WebGL/XRay` | Dual-pass: Z-fail (behind) + Z-pass (front) + fresnel | Behind + Front |
| `WebGL/SolidColor` | Blinn-Phong (ambient + Lambert + specular) | Outline + Main + ShadowCaster |
| `WebGL/Wireframe` | Geometry shader + screen-space distance to edge | Main (+ Fallback SubShader) |
| `WebGL/WireframeWebGL` | Fresnel edge + UV grid (sin geometry shader) | Main |
| `WebGL/Ghosted` | Fresnel alpha + depth fade | Main |
| `WebGL/Thermal` | Procedural noise + 4-color gradient + scanlines | Main |
| `Skybox/AnimatedGradientSkybox` | Radial gradient + breathing pulse | Main (CG, no HLSL) |

### 13.2 Sistema de Clipping Dual-Plane

El sistema de corte transversal permite hasta 2 planos de corte simultáneos controlados por `CrossSectionManager`:

```
C# (CrossSectionManager):
    Shader.SetGlobalVector("_GlobalClipPlane", planeEquation);
    Shader.SetGlobalFloat("_GlobalClipEnabled", 1.0f);
    
                    ↓ (inyección global en GPU)

HLSL (en CADA shader):
    if (_GlobalClipEnabled > 0.5)
    {
        float clipDist = dot(positionWS, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
        if (clipDist < 0) discard;  // Descarta fragmento
    }
```

La ecuación del plano `(nx, ny, nz, d)` define que todo punto cuyo `dot(point, normal) + d < 0` es descartado. Los quads visuales del plano de corte se posicionan mediante la misma ecuación transformada.

### 13.3 Optimizaciones WebGL

`WebGLOptimizer` (106 líneas) aplica optimizaciones automáticas:

| Optimización | Detalle |
|---|---|
| **Resolución adaptativa** | Reduce `QualitySettings.renderScale` en 0.1 si FPS < target−5 |
| **Monitoreo de heap** | Cada 300 frames verifica `Profiler.GetTotalReservedMemoryLong()`; si > 800MB fuerza `GC.Collect()` + `Resources.UnloadUnusedAssets()` |
| **Textura mip bias** | En móvil WebGL incrementa `QualitySettings.globalTextureMipmapLimit` para reducir VRAM |
| **Sombras condicionales** | Desactiva sombras en dispositivos móviles (`SystemInfo.graphicsDeviceType`) |
| **LOD bias** | Reduce `QualitySettings.lodBias` en plataformas limitadas |
| **Particle budget** | Limita `QualitySettings.particleRaycastBudget` |

---

## 14. Patrones de Diseño Aplicados

| Patrón (GoF/Arquitectónico) | Implementación en la Aplicación |
|---|---|
| **Singleton** | `Singleton<T>` / `PersistentSingleton<T>` — jerarquía de 3 niveles |
| **Observer (Pub/Sub)** | `EventBus` con tipado genérico + eventos C# directos en managers |
| **State (FSM)** | `AppStateMachine` con 9 estados y tabla de transiciones |
| **Strategy** | `ViewModeManager` intercambia shader/material sin modificar la lógica de rendering |
| **Facade** | `UIManager` coordina 6 sub-controladores sin exponer la complejidad interna |
| **Mediator** | `UIManager` media entre eventos de Core y actualizaciones de UI |
| **Command** | `DroneStateController` ejecuta secuencias de startup/shutdown como "comandos" corrutina |
| **Template Method** | `Singleton<T>.Awake()` → `base.Awake()` con hooks en cada nivel |
| **Null Object** | `SceneBootstrapper.EnsureManager()` crea managers vacíos en lugar de fallar |
| **Object Pool** | `ObjectPooler` con reciclaje round-robin |
| **Flyweight** | `ViewModeManager` comparte un material por modo (no por renderer); override vía `MaterialPropertyBlock` |
| **Bridge** | `ClippableLit.shader` actúa como puente entre el modo Realistic (PBR) y el sistema de clipping |

---

## 15. Decisiones de Diseño y Justificaciones

### 15.1 ¿Por qué EventBus estático y no inyección de dependencias?

En una aplicación de escena única compilada a WebGL, el overhead de un contenedor DI (como Zenject) no se justifica:
- Aumenta el tamaño del binario .wasm (cada KB cuenta en tiempo de descarga).
- Añade indirección en la resolución de dependencias que complica el debugging en WebGL (donde no hay depurador paso-a-paso).
- El EventBus estático ofrece O(1) para publicar, tipado por genéricos, y es trivial de entender.

### 15.2 ¿Por qué UI Toolkit sobre UGUI?

- **Rendimiento:** UI Toolkit rasteriza a una textura única (atlas) en lugar de generar geometry per-element como UGUI.
- **Responsive:** USS soporta `flex-grow`, `flex-shrink`, porcentajes y `auto`, facilitando la adaptación a diferentes resoluciones de navegador.
- **Separación:** UXML/USS separan estructura y estilo del código C#, similar al paradigma web.
- **Futuro:** Unity ha declarado UI Toolkit como el sucesor de UGUI; nuevas features solo llegan a Toolkit.

### 15.3 ¿Por qué `sharedMaterial` en lugar de `material`?

Acceder a `renderer.material` en Unity crea una instancia del material en memoria. En una escena con 200+ renderers y 7 modos de visualización, esto significaría 1,400 copias de materiales. Usar `sharedMaterial` + `MaterialPropertyBlock` reduce la huella de memoria a 7 materiales + N property blocks (que son structs ligeros en la GPU).

### 15.4 ¿Por qué duplicar WireframeWebGL sin Geometry Shader?

WebGL 2.0 (basado en OpenGL ES 3.0) **no soporta geometry shaders**. El shader `Wireframe.shader` utiliza `#pragma geometry` para calcular distancias a aristas en screen-space. Para WebGL, el fallback `WireframeWebGL.shader` aproxima el wireframe mediante:
1. **Fresnel edge detection** (bordes de silueta) — detecta aristas por ángulo NdotV.
2. **UV-based grid** — usa las coordenadas UV del mesh para dibujar una cuadrícula 10×10.

El resultado es visualmente distinto (no son aristas reales del mesh), pero funcional para la visualización exploratoria.

### 15.5 ¿Por qué AnimatedGradientSkybox usa CG y no HLSL?

El skybox se renderiza en el **Background queue** antes que cualquier objeto URP. Unity permite mezclar CG (legacy built-in) y HLSL (URP) en la misma escena siempre que no compartan passes. El skybox, al tener su propio pass aislado, puede usar `UnityCG.cginc` sin conflictos. Se eligió CG por simplicidad: `UnityObjectToClipPos` y `ComputeScreenPos` son wrappers directos sin boilerplate de URP.

---

## 16. Inventario Completo del Código Fuente

### 16.1 Core — Infraestructura (5 archivos, ~658 líneas)

| Archivo | Líneas | Clase | Función |
|---|---:|---|---|
| `Singleton.cs` | 58 | `StaticInstance<T>`, `Singleton<T>`, `PersistentSingleton<T>` | Jerarquía genérica de singletons |
| `EventBus.cs` | 170 | `EventBus` (static) | Pub/Sub tipado con thread-safety |
| `CoreEvents.cs` | 29 | `PartSelectedEvent`, `AppStateChangedEvent`, `ViewModeChangedEvent` | DTOs de eventos del dominio |
| `StateChangedEvent.cs` | 17 | `StateChangedEvent` | DTO con PreviousState + NewState |
| `AppStateMachine.cs` | 243 | `AppStateMachine : Singleton<>` | FSM de 9 estados con transiciones validadas |

### 16.2 Core — Managers (20 archivos, ~5,014 líneas)

| Archivo | Líneas | Singleton | Función |
|---|---:|---|---|
| `GameManager.cs` | 68 | Persistent | Shell de coordinación global + debug flag |
| `SelectionManager.cs` | 308 | Singleton | Raycast hover/click, highlight, selección con swap |
| `ViewModeManager.cs` | 327 | Singleton | 7 modos shader con cache + MaterialPropertyBlock |
| `OrbitCameraController.cs` | 383 | Singleton | Orbit/Pan/Zoom con mouse+touch, damping, smart pivot |
| `CrossSectionManager.cs` | 466 | Singleton | Dual-plane clipping + material swap (ClippableLit) |
| `ExplodedViewManager.cs` | 263 | Singleton | Vista explosionada con Lerp + filtros de categoría |
| `PartVisibilityManager.cs` | 211 | Singleton | Fade in/out + aislamiento de partes |
| `EnvironmentController.cs` | 152 | Singleton | 5 presets de iluminación + skybox |
| `InputManager.cs` | 145 | Persistent | Bloqueo de input unificado + IsPointerOverUI |
| `DroneStateController.cs` | 263 | Singleton | 5 estados del dron (animación/audio) |
| `AssemblyGuideManager.cs` | 290 | Singleton | Guía paso a paso con highlight + cámara |
| `MeasurementTool.cs` | 278 | Singleton | Distancia/Ángulo/Radio con raycast + line renderer |
| `AnnotationSystem.cs` | 196 | Singleton | Anotaciones 3D (esfera + línea + texto billboard) |
| `BillOfMaterialsManager.cs` | 103 | Singleton | BOM agrupado por parte + export CSV |
| `ModularPartsSystem.cs` | 165 | Singleton | Intercambio animado de piezas modulares |
| `ConnectionPointsViewer.cs` | 163 | Singleton | Visualización de puntos de conexión coloreados |
| `AssemblyChecklist.cs` | 147 | Singleton | Checklist de verificación de ensamblaje |
| `PartCatalogManager.cs` | 155 | Singleton | Catálogo filtrable (búsqueda + categoría + material) |
| `ScreenshotManager.cs` | 82 | Singleton | Captura PNG a base64 (WebGL) o archivo (Editor) |
| `KeyboardShortcuts.cs` | 139 | Singleton | Atajos de teclado (1-9, E, Esc, Ctrl±, F1-F3) |

### 16.3 Core — Managers (Secundarios) (10 archivos, ~847 líneas)

| Archivo | Líneas | Singleton | Función |
|---|---:|---|---|
| `CameraPresets.cs` | 59 | Singleton | 6 ángulos predefinidos (Front, Back, Left, Right, Top, Iso) |
| `AnalyticsManager.cs` | 117 | Persistent | Tracking de eventos + tiempo de vista por parte |
| `AudioManager.cs` | 118 | Persistent | 6 SFX + música + volumen con persistencia |
| `NotificationManager.cs` | 79 | Persistent | Toast notifications con fade in/out |
| `AccessibilityManager.cs` | 100 | Persistent | UI Scale + High Contrast + Reduced Motion |
| `CursorManager.cs` | 71 | Singleton | 7 tipos de cursor personalizados |
| `ErrorHandler.cs` | 108 | Persistent | Panel de error auto-captura de excepciones |
| `QualityManager.cs` | 54 | Singleton | Resolución adaptativa por FPS |
| `SaveSystem.cs` | 33 | Static | PlayerPrefs wrapper (volumen + calidad) |
| `AssetLoader.cs` | 36 | MonoBehaviour | Resources.LoadAsync placeholder |
| `WebGLOptimizer.cs` | 106 | Persistent | Optimizaciones WebGL + monitoreo heap |

### 16.4 Core — Utilidades (7 archivos, ~667 líneas)

| Archivo | Líneas | Función |
|---|---:|---|
| `SceneBootstrapper.cs` | 174 | Auto-creación de 18 managers + validación |
| `TweenEngine.cs` | 154 | Animación procedural: Float, Vector3, Color + 8 easings |
| `DroneAssembler.cs` | 187 | Creación procedural del dron con 7 tipos de parte |
| `DronePartSetup.cs` | 108 | Utilidad de editor para setup rápido de ExplodablePart |
| `FPSCounter.cs` | 33 | Display IMGUI de FPS con colores adaptativos |
| `ObjectPooler.cs` | 62 | Pool genérico con reciclaje round-robin |
| `PerformanceMonitor.cs` | 108 | Overlay IMGUI de rendimiento (FPS + memoria + quality) |
| `RuntimeConsole.cs` | 67 | Consola in-game para debugging WebGL |
| `WebGLProfiler.cs` | 85 | Profiler simplificado con umbrales de estado |

### 16.5 Content (3 archivos, ~359 líneas)

| Archivo | Líneas | Función |
|---|---:|---|
| `ExplodablePart.cs` | 57 | MonoBehaviour: posición explosionada + referencia a DronePartData |
| `HighlightSystem.cs` | 121 | Hover (tint) + Select (emission pulse) via MaterialPropertyBlock |
| `MaterialController.cs` | 61 | Toggle XRay + color override per-renderer |

### 16.6 Data (2 archivos, ~202 líneas)

| Archivo | Líneas | Función |
|---|---:|---|
| `DronePartData.cs` | 116 | ScriptableObject: 8 grupos de campos (specs, materiales, ensamblaje) |
| `AssemblyGuideData.cs` | 86 | ScriptableObject: pasos de ensamblaje con validación OnValidate |

### 16.7 UI (14 archivos, ~2,923 líneas)

| Archivo | Líneas | Función |
|---|---:|---|
| `UIManager.cs` | 430 | Orquestador de 6 sub-controladores |
| `UIModeController.cs` | 561 | 3 modos (Inspect/Analyze/Studio) con cards + sub-paneles |
| `UIDetailsSheet.cs` | 354 | Bottom sheet con drag-to-dismiss + 12 campos de datos |
| `UIHeroController.cs` | 188 | Landing screen + 3 submenús |
| `UIAnalyzePanel.cs` | 102 | 7 botones de modo shader |
| `UICrossSectionPanel.cs` | 249 | Control dual-plane con FIFO deselection |
| `UIEnvironmentPanel.cs` | 115 | Sliders de luz + presets |
| `HotspotManager.cs` | 96 | Spawn y gestión de SmartHotspots |
| `SmartHotspot.cs` | 160 | Hotspot 3D→2D con occlusion staggered |
| `AssemblyStepUI.cs` | 310 | Panel lateral de guía de ensamblaje |
| `PartCatalogUI.cs` | 243 | Panel izquierdo de catálogo filtrable |
| `SettingsPanel.cs` | 148 | Modal de configuración (audio + accesibilidad) |
| `LoadingController.cs` | 112 | Overlay de carga con progreso suavizado |
| `TooltipSystem.cs` | 80 | Tooltip flotante que sigue el cursor |
| `SceneTransitionManager.cs` | 116 | Fade in/out + crossfade |
| `EnhancedInfoPanel.cs` | 272 | Panel de información detallada de parte |
| `DetailsPanelController.cs` | 56 | Controller UXML-driven ligero |
| `UIAnimator.cs` | 56 | Fade in/out por corrutina |
| `UIConstants.cs` | 28 | Constantes de nombres UXML auto-generadas |

### 16.8 Archivos Deprecados

| Archivo | Líneas | Estado |
|---|---:|---|
| `EngineerToolbar.cs` | 171 | `[Obsolete]` — Reemplazado por sistema de 3 modos (Phase 2) |
| `ViewModeToolbar.cs` | 182 | `[Obsolete]` — Reemplazado por `UIModeController + UIAnalyzePanel` |

### 16.9 Shaders (9 archivos, ~1,544 líneas)

| Archivo | Líneas | Técnica |
|---|---:|---|
| `ClippableLit.shader` | 253 | PBR URP + dual clip + edge color |
| `Blueprint.shader` | 225 | Fresnel + dual grid + outline |
| `SolidColor.shader` | 281 | Blinn-Phong + outline + shadow |
| `Wireframe.shader` | 255 | Geometry shader + fallback SubShader |
| `XRay.shader` | 210 | Dual-pass Z-fail/Z-pass + fresnel |
| `Thermal.shader` | 192 | Noise + 4-gradient + scanlines |
| `WireframeWebGL.shader` | 132 | Fresnel + UV grid (sin geometry) |
| `Ghosted.shader` | 135 | Fresnel alpha + depth fade |
| `AnimatedGradientSkybox.shader` | 68 | Radial gradient + pulse (CG legacy) |

### 16.10 UXML/CSS

| Archivo | Líneas | Contenido |
|---|---:|---|
| `MainLayout.uxml` | 353 | Layout principal completo de la aplicación |
| `IconGallery.uxml` | 71 | Galería de iconos procedurales (debug/editor) |
| `Theme.uss` | — | Estilos CSS (referenciado, no contabilizado) |

---

*Documento generado como parte de la documentación técnica del proyecto de grado.*  
*Rama: `feature/phase2-ux-redesign` — Febrero 2026*
