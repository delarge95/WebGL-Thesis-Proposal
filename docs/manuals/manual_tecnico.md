# Manual Técnico

## Introducción
Este manual técnico describe la arquitectura real de la build final documentada del visor WebGL para el Holybro X500~V2. Su objetivo es explicar cómo se orquesta la aplicación en tiempo de ejecución, cómo se normaliza el modelo importado, cómo se conectan los modos visibles con los subsistemas internos y cuáles son los límites de la implementación entregada.

El documento no debe leerse como una promesa de todo el código presente en el repositorio. Por el contrario, diferencia explícitamente entre:

    * funcionalidades expuestas en la interfaz final;
    * funcionalidades implementadas pero ocultas;
    * módulos legados o no integrados;
    * trabajo futuro aún no consolidado.

## Convenciones canónicas

### Taxonomía de piezas

    * **28 piezas canónicas:** entidades semánticas definidas en `x500v2\_parts\_data.json`.
    * **30 anchors de escena:** 28 piezas canónicas más `x500v2\_fastener\_group` y `x500v2\_misc\_group`.
    * **257 renderers/colliders:** granularidad técnica del import final auditado.
    * **257 assets generados:** recursos `.asset` producidos para el binding y consumo runtime.

Esta distinción es crítica: una pieza canónica puede estar compuesta por múltiples renderers sin que eso altere la taxonomía académica del sistema.

### Taxonomía de scripts

    * **95** scripts runtime propios en `Assets/Scripts`.
    * **103** scripts bajo `Assets` excluyendo `Editor/Tests`.
    * **129** archivos `.cs` totales incluyendo editor y plugins.

## Verdad funcional del sistema

    * **Escena oficial:** `MainScene\_Final`
    * **Layout UI:** `MainLayout.uxml`
    * **Control principal UI:** `UIManager.cs`
    * **Saneamiento runtime del import:** `ImportedDroneRuntimeBinder.cs`
    * **Documento de contraste funcional:** validación funcional final del proyecto, usada como referencia de cierre y verificación editorial.

### Flujo visible consolidado

`Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio`

Este flujo es la base documental de la versión final. Toda referencia a paneles laterales, catálogos visibles, configuración general del sistema o herramientas no expuestas debe marcarse como oculta, legada o futura, nunca como parte del recorrido principal.

## Arquitectura runtime

### Capas operativas

    * **Presentación.** UI Toolkit, `MainLayout.uxml`, hojas de estilo y controladores de panel.
    * **Orquestación.** `UIManager`, `UIModeController`, `AppStateMachine` y enrutamiento de eventos.
    * **Servicios de escena.** selección, aislamiento, explosión, clipping, view modes, entorno, estado del dron y subsistema térmico.
    * **Datos.** `DronePartData`, assets generados, catálogo JSON canónico y grafo térmico.

### Bootstrap
El arranque del visor combina dos mecanismos complementarios:

    * **`UIManager.EnsureManagers()**.` Garantiza la existencia de los servicios mínimos para operar la build final.
    * **`ImportedDroneRuntimeBinder**.` Repara la jerarquía del dron importado, reubica huérfanos, sincroniza anchors y reconstruye caches dependientes.

El orden funcional esperado puede resumirse así:

    * carga de `MainScene\_Final`;
    * inicialización del `UIDocument`;
    * ejecución de `UIManager.InitializeUI()`;
    * creación o garantía de managers base;
    * inyección del `ImportedDroneRuntimeBinder` sobre `x500v2\_Drone` si hace falta;
    * reconstrucción de servicios dependientes del modelo;
    * activación del flujo visible de interacción.

### Managers garantizados en runtime

p{7.7cm}}

**Módulo** | **Responsabilidad** 

**Módulo** | **Responsabilidad** 

`InputManager` | bloquea input 3D cuando el usuario interactúa con UI y evita click-through sobre la escena 

`SelectionManager` | gestiona selección actual, doble clic y eventos de cambio de pieza 

`FastenerRegistry` | resuelve en runtime la relación `instanceId -> family -> recipe` para fasteners 

`FastenerInspectionManager` | reemplaza solo el proxy seleccionado por detalle procedural temporal y restaura el proxy al deseleccionar 

`ExplodedViewManager` | controla separación del ensamblaje y filtros por categoría 

`PartCatalogManager` | mantiene consulta runtime de piezas, aunque el catálogo no esté expuesto al usuario 

`NotificationManager` | emite mensajes breves de estado y feedback operativo 

`HotspotManager` | construye, inicializa y muestra hotspots 

`ViewModeManager` | orquesta modos de visualización y modo base activo 

`EnvironmentController` | aplica presets de skybox, luz y contraste general 

`CrossSectionManager` | habilita clipping global y planos de corte 

`PartVisibilityManager` | ocultar, mostrar, aislar y restaurar piezas o grupos 

`DroneStateController` | estado operativo, carga, hélices, luces, partículas y audio opcional 

`Thermal\-Simulation\-Manager` | solver térmico reducido por componentes 

`ThermalViewController` | traduce temperaturas a parámetros de shader y leyenda visual 

### Protección de input y convivencia UI/escena
Un rasgo clave de la implementación final es el control explícito de conflicto entre UI y navegación 3D. Botones, sliders, *bottom sheet* y paneles registran eventos de puntero que activan `InputManager.InputBlocked` mientras el usuario interactúa con la interfaz. Este detalle evita que un arrastre sobre un slider, por ejemplo, termine rotando la cámara o seleccionando piezas accidentalmente.

## Controladores UI y responsabilidades

p{7.7cm}}

**Controlador** | **Función principal** 

**Controlador** | **Función principal** 

`UIManager` | coordinador raíz; inicializa subcontroladores, enruta eventos y sincroniza subsistemas 

`UIHeroController` | maneja Hero, retorno al inicio, acceso al repositorio y ayuda inicial 

`UIDetailsSheet` | administra ficha inferior, peek bar, foco de cámara y contenido de selección 

`UIModeController` | activa, desactiva y sincroniza `Inspect`, `Analyze` y `Studio` 

`InspectModeHandler` | gestiona hotspots, isolate, power y carga 

`AnalyzeModeHandler` | gestiona corte, explode y filtros con navegación de dos niveles 

`StudioModeHandler` | contenedor visible del modo `Studio` 

`UIAnalyzePanel` | controla modos de visualización y leyenda térmica 

`UIEnvironmentPanel` | controla presets de entorno y sliders de iluminación 

`UICrossSectionPanel` | controla ejes, inversión y segundo plano de corte 

`OnboardingController` | muestra la ayuda inicial cuando el usuario entra al visor o la solicita 

`OnboardingAnimationView` | renderiza las demos procedurales del onboarding en desktop y mobile mediante `Painter2D` 

### Secuencia de interacción: Hero y entrada al visor
El Hero no es un splash decorativo, sino el primer estado funcional del sistema. Desde `UIManager`, el controlador de Hero emite eventos que:

    * habilitan la inicialización visible de hotspots cuando el usuario entra al visor;
    * muestran la guía de onboarding la primera vez;
    * permiten volver al Hero, cerrar modos activos y restablecer la vista principal.

### Secuencia de interacción: onboarding
El onboarding visible no depende de video ni de media embebida. Su arquitectura actual combina dos piezas:

    * `OnboardingController`, que decide cuándo mostrar la ayuda, qué tarjeta está activa, qué *copy* debe aparecer y cómo cambia la plataforma entre desktop y mobile;
    * `OnboardingAnimationView`, que dibuja cada demostración mediante `Painter2D` como una escena procedural de actor, objetivo y respuesta del sistema.

Este diseño permite mantener la ayuda sincronizada con la UI real. Cada tarjeta del onboarding reutiliza el mismo lenguaje visual de cursores, ripples, sliders, botones y mini iconos del producto, en lugar de depender de GIFs o secuencias externas difíciles de actualizar.

### Secuencia de interacción: selección y ficha inferior
Cuando `SelectionManager` emite un `PartSelectedEvent`, `UIManager` delega el contenido a `UIDetailsSheet`. A partir de ahí ocurre lo siguiente:

    * se actualiza el nombre visible de la pieza en la barra superior e inferior;
    * se llenan campos editoriales desde `DronePartData`;
    * si la ficha está cerrada, aparece la barra de previsualización;
    * si la ficha se abre, la cámara puede recentrar la selección;
    * si no hay selección, la interfaz vuelve a `SELECT A PART`.

La selección visible ya no debe leerse como un único nivel plano. El sistema diferencia:

    * **pieza madre**, cuando la interacción se resuelve sobre el conjunto principal;
    * **subpieza**, cuando el usuario profundiza sobre una geometría interna del conjunto activo;
    * **grupo de hotspot**, cuando la entrada llega desde un pin y representa una lectura agrupada del sistema.

Por esa razón, un clic de fondo puede retroceder un nivel de lectura y un doble clic puede activar aislamiento del nivel actual. Esta jerarquía es importante porque la ficha, la cámara, el isolate y algunos paneles de análisis no siempre trabajan sobre la misma granularidad semántica.

La ficha inferior contiene reglas de presentación importantes:

    * `PartPower` muestra vatios solo si el dato existe;
    * `PartTemp` muestra temperatura operativa si es mayor que cero, o `N/A`;
    * la dificultad se renderiza como calificación visual por estrellas;
    * selecciones provenientes de hotspots pueden presentar títulos y descripciones grupales.

Para fasteners, `SelectionManager` promueve cualquier clic sobre la geometría temporal al root completo del fastener antes de publicar el evento de selección. Esto evita aislamientos parciales por submalla y asegura que el color de *hover* o selección se reaplique correctamente cuando `FastenerInspectionManager` sustituye el proxy por detalle procedural temporal.

A nivel de aislamiento, `PartVisibilityManager` separa varios casos: fastener individual, pieza madre, subpieza y grupo de hotspot. El fastener individual se muestra como unidad completa; la pieza madre puede conservar fasteners reconciliados mediante `parentCanonicalPartId`; la subpieza estrecha el contexto al nivel más fino disponible; y el grupo de hotspot permite una lectura agregada cuando el usuario entra desde un pin en lugar de hacerlo desde geometría directa.

### Secuencia de interacción: tecla Esc
`UIManager` define una prioridad explícita para `Esc`:

    * cerrar la ficha inferior si está abierta;
    * si la ficha no está abierta, delegar al `UIModeController` para cerrar subpaneles o desactivar el modo actual;
    * no realizar acciones adicionales si no hay estados abiertos.

## Flujos funcionales por modo

### Modo Inspect
`InspectModeHandler` controla cuatro elementos visibles:

    * **Hotspots.** Inician activos. Su visibilidad se conmuta desde el gestor de hotspots del sistema.
    * **Isolate.** Solicita aislamiento a través de `UIManager`, que a su vez resuelve si el aislamiento es de pieza madre, subpieza, fastener individual o grupo de hotspot.
    * **Power.** Conmuta el estado de `DroneStateController`.
    * **Load.** Envía un valor normalizado a `DroneStateController.SetLoadCommand()` para actualizar la carga operativa.

El handler también sincroniza la etiqueta de estado con los estados `OFF`, `STARTING`, `IDLE`, `FLYING` y `SHUTDOWN`. La herramienta de medición existe en código, pero su botón permanece oculto en la UI final, por lo que no debe documentarse como función visible de usuario.

### Modo Analyze
`AnalyzeModeHandler` usa una navegación de dos niveles:

    * **Nivel 1:** grilla principal de tarjetas.
    * **Nivel 2:** subpanel de `Cross Section` o subpanel de `Filter`.

La vista explosionada se resuelve de forma distinta: no abre un panel aparte, sino un deslizador en línea debajo de la grilla principal. El handler conserva el último valor distinto de cero para restaurarlo si la herramienta se vuelve a activar.

#### Filtros
Los filtros activos iniciales corresponden a:

    * `SkeletonAirframe`
    * `PropulsionSystem`
    * `Avionics`
    * `SensorsComms`
    * `PowerDistribution`
    * `Fasteners`

Si el usuario apaga la última categoría visible, el handler restablece el conjunto por defecto para evitar un estado completamente vacío. Además, el controlador acepta modo exclusivo con doble clic sobre la categoría.

#### Corte transversal
`UICrossSectionPanel` soporta hasta dos planos simultáneos. La lógica real es:

    * el panel inicia con el eje `Y` activo;
    * el usuario puede activar hasta dos ejes;
    * si selecciona un tercer eje, se elimina el más antiguo mediante estrategia FIFO;
    * cada plano puede invertirse de forma independiente;
    * el segundo bloque de controles solo aparece cuando hay dos ejes activos y el modo combinado no está habilitado;
    * si todos los ejes se desactivan, `CrossSectionManager` deshabilita el corte;
    * el corte persiste al volver desde el subpanel y solo se limpia por deselección total de ejes o `RESET VIEW`.

### Modo Studio
`Studio` combina dos subsistemas:

    * `UIAnalyzePanel`, que gobierna los modos de visualización.
    * `UIEnvironmentPanel`, que gobierna presets de entorno e iluminación.

En la UI final, `Studio` no solo cambia modos visuales. También controla ciclos de entorno y tres deslizadores de iluminación visibles para el usuario: rotación de luz, intensidad sobre el objeto y tono del fondo.

## Datos y normalización del modelo

### Catálogo canónico
`x500v2\_parts\_data.json` es la fuente semántica oficial del proyecto. De este archivo derivan:

    * el conteo canónico de piezas;
    * la categorización funcional;
    * parte de los datos que consume `DronePartData`;
    * la narrativa académica y documental del sistema.

### Catalogos modulares de fasteners
La implementacion Unity incorpora una capa especifica para tornilleria y piezas de fijacion, separada del catalogo canonico de piezas principales. Esta capa se materializa en tres archivos:

    * `holybro\_fastener\_families.json`: define familias modulares con metrica, longitud, material, receta procedural y referencia CAD/Holybro.
    * `holybro\_fastener\_instances.json`: registra instancias presentes en `MainScene\_Final` con `instanceId`, pose local y anchor canonico inferido.
    * `holybro\_fastener\_reconciliation.json`: documenta aliases, overrides semanticos y discrepancias entre escena y dataset synced.
    * `holybro\_parent\_subpieces.json`: fija la relacion pieza madre $\rightarrow$ subpiezas documentadas $\rightarrow$ fasteners configurados y se replica en `subComponentNames` dentro de los assets generados.

En el estado documentado de cierre, estos catalogos registran **20 familias**, **168 instancias** y **9 entradas de reconciliacion**. La geometria visible sigue siendo temporal/proxy, pero el contrato `familyId` / `instanceId` queda estable para sustituir mas adelante los placeholders Unity por mallas Blender definitivas sin alterar la logica runtime.

### Pipeline final Blender a Unity
El reimport final del modelo optimizado se define como una exportacion manual desde Blender 5.0.1. La escena runtime no se compone solo de instancias: debe incluir simultaneamente `BAKE\_MASTERS\_LOW`, `ASSEMBLY\_INSTANCES\_LOW`, `PRIMITIVE\_FASTENER\_MASTERS` y `PRIMITIVE\_FASTENER\_INSTANCES`. Excluir los masters provocaria perdida fisica de piezas del dron.

El paquete de texturas recomendado para Unity queda compuesto por `X500\_BaseColor\_4K.png`, `X500\_Normal\_Final\_4K.png` y `X500\_Mask\_4K.png`. La mascara compacta usa los canales `R=AO`, `G=Roughness`, `B=Curvature` y `A=Metallic`. Para compatibilidad con URP Lit estandar puede generarse tambien `X500\_MetallicSmoothness\_4K.png`.

La importacion posterior debe apoyarse en un manifest de Blender que reporte transformaciones, bounds, roles runtime y candidatos de pieza madre para fasteners. Si un fastener no puede asociarse con confianza a una pieza madre, permanece en revision y no se asigna automaticamente.

### Saneamiento del import
`ImportedDroneRuntimeBinder` ejecuta tareas de estabilización sobre el modelo importado:

    * resuelve el root del dron;
    * construye el mapa de anchors por identificador;
    * reubica objetos huérfanos o mal anidados;
    * crea o reutiliza grupos sintéticos para tornillería y elementos misceláneos;
    * garantiza `FastenerRegistry` y `FastenerInspectionManager`;
    * sella `fastenerFamilyId` y `fastenerInstanceId` sobre fasteners detectados;
    * reconstruye caches de managers dependientes;
    * dispara eventos para regeneración de hotspots y auditorías.

### De semántica a geometría
El sistema trabaja con varias capas de representación:

    * **Semántica:** pieza canónica y sus metadatos.
    * **Escena:** anchor o transform padre usado para binding.
    * **Geometría:** renderers y colliders individuales.
    * **Visualización:** materiales, property blocks, clipping y modos de vista.

Esta separación permite que la UI describa un componente a nivel conceptual aunque el import final lo represente con múltiples submallas.

### Inspeccion bajo demanda de fasteners
La optimizacion de fasteners no se resuelve cargando mallas densas todo el tiempo. El comportamiento implementado es:

    * la escena permanece en reposo con proxies ligeros;
    * al seleccionar un fastener, `FastenerRegistry` resuelve su metadata tecnica;
    * `FastenerInspectionManager` puede activar un conjunto acotado de detalles temporales segun contexto;
    * `FastenerBuilder` crea detalle procedural para el fastener seleccionado, para un fastener aislado aunque ya no permanezca seleccionado y para los fasteners asociados a una pieza madre aislada;
    * al salir del contexto correspondiente, cada detalle se destruye y su proxy original se restaura.

Esta decision mantiene bajo el costo visual de la escena actual y, al mismo tiempo, valida el flujo de inspeccion detallada que luego recibira las mallas finales producidas en Blender.

En paralelo, `HighlightSystem`, `MaterialController` y `PartVisibilityManager` refrescan sus objetivos visuales y su scope de aislamiento para que el fastener siga respondiendo como unidad completa durante *hover*, seleccion e isolate.

La camara tambien deja de usar una distancia fija de inspeccion. `OrbitCameraController` calcula el enfoque a partir de los bounds reales de la seleccion y reduce el *near clip* para permitir acercamientos utiles sobre fasteners y piezas pequenas.

Ademas, la ventana de zoom deja de ser global. La sensibilidad y los limites de acercamiento se recalculan por seleccion para evitar que un rango valido para el dron completo resulte tosco o excesivo sobre una pieza de fijacion pequena.

Ese recalculo ya no depende del isolate: una seleccion activa dentro del ensamblaje completo tambien puede estrechar la ventana de zoom para habilitar acercamientos mas finos sin ocultar el contexto restante.

En el mismo controlador, los factores de `pan` y `orbit` pasan a escalarse con la distancia y el contexto efectivo de inspeccion. Con ello se evita que una sensibilidad correcta para el dron completo resulte excesiva cuando el usuario analiza un fastener o una subpieza muy pequena.

La iteracion mas reciente sustituye la respuesta lineal de ambos factores por curvas separadas: `pan` cae con mayor rapidez en escalas pequenas para priorizar precision de centrado, mientras `orbit` conserva una respuesta angular algo mas alta para no entorpecer la inspeccion visual.

Para evitar bloqueos al deshacer zoom, el controlador ya no usa ciegamente el ultimo `target` enfocado como contexto adaptativo. La prioridad queda definida como seleccion activa, luego aislamiento activo y, en ausencia de ambos, el contexto base del modelo. Ademas, la ventana adaptativa conserva un margen minimo de alejamiento para que una inspeccion cercana no cierre por completo la salida hacia vistas mas generales.

Como refinamiento final, la curva de `pan` para escalas pequenas se endurece aun mas que en la iteracion previa. El objetivo es que el usuario pueda recentrar un fastener aislado con desplazamientos cortos sin expulsarlo facilmente del encuadre.

## Subsistema de visualización y entorno

### Modos de vista
`UIAnalyzePanel` conoce internamente los modos:

    * `Realistic`
    * `XRay`
    * `Blueprint`
    * `SolidColor`
    * `Wireframe`
    * `Ghosted`
    * `Thermal`

Sin embargo, la UI final solo expone directamente `X-Ray`, `Solid Color` y `Thermal`. `Realistic` opera como modo base, mientras que `Blueprint` puede activarse indirectamente desde el ciclo de presets de entorno.

### Lógica de modo base
La distinción entre modo base y modo activo es importante:

    * si el usuario pulsa un modo visual que ya está activo, el sistema regresa al modo base;
    * normalmente el modo base es `Realistic`;
    * si el preset `Blueprint` se activa desde el panel de entorno, el modo base pasa temporalmente a `Blueprint`.

### Panel de entorno
`UIEnvironmentPanel` implementa tres ciclos:

    * `Studio` $\rightarrow$ `Studio Light` $\rightarrow$ `Blueprint`
    * `Day` $\rightarrow$ `Night` $\rightarrow$ `Sunset`
    * `White` $\rightarrow$ `Grey` $\rightarrow$ `Black` $\rightarrow$ `Yellow` $\rightarrow$ `Orange` $\rightarrow$ `Green` $\rightarrow$ `Blue` $\rightarrow$ `Purple` $\rightarrow$ `Red`

El controlador actualiza dinámicamente la etiqueta de cada tarjeta para reflejar el preset vigente y, cuando corresponde, restablece el modo base al salir de `Blueprint`. En paralelo, coordina los tres sliders visibles de `Lighting Controls`: `Light Rotation`, `Light Intensity` y `Background Tone`.

### EnvironmentController
`EnvironmentController` no opera con fondos planos simples. Usa el shader `AnimatedGradientSkybox` y define presets mediante un conjunto de parámetros:

    * color superior e inferior del gradiente;
    * color e intensidad de la luz direccional;
    * rotación y pitch de la luz;
    * escala del gradiente;
    * dither;
    * pulso opcional;
    * activación de grilla para `Blueprint`.

El controlador interpola transiciones entre presets y emite `OnLightBackgroundChanged` cuando detecta que el centro del gradiente alcanza suficiente luminancia para requerir UI adaptativa. `UIManager` consume ese evento para alternar clases visuales y mejorar legibilidad.

Esto significa que el control de iluminación del fondo no es un simple color plano. El sistema modifica la percepción general del entorno mediante skybox animado, gradiente, contraste, intensidad de luz y adaptación de la propia interfaz cuando el fondo se vuelve muy claro.

## Subsistema de estado operativo del dron
`DroneStateController` articula la simulación operativa ligera de la escena. Sus estados son:

    * `Off`
    * `StartingUp`
    * `Idle`
    * `Flying`
    * `ShuttingDown`

### Efectos coordinados por estado
El cambio de estado afecta:

    * velocidad de hélices;
    * iluminación de estado;
    * partículas;
    * desplazamiento de hover;
    * clips de audio, si existen recursos asignados.

### Carga del sistema
La carga se expresa como un valor normalizado $u \in [0,1]$ enviado por `SetLoadCommand`. La lógica relevante es:

    * un umbral aproximado de `0.35` separa reposo operativo y vuelo;
    * el sistema mantiene un piso armado de carga aun con valores bajos cuando el dron está encendido;
    * el valor final de carga alimenta el estado térmico del sistema.

## Subsistema térmico híbrido

### Alcance real
El sistema térmico implementado no es un solver FEA ni una simulación termodinámica de alta fidelidad. Su alcance real es el de una **simulación híbrida heurística por componentes**, diseñada para:

    * comunicar zonas críticas;
    * propagar calor de forma creíble entre componentes vinculados;
    * responder al estado operativo del dron;
    * alimentar una visualización térmica en tiempo real adecuada para WebGL.

### Pipeline térmico

`DroneStateController -> ThermalSimulationManager -> ThermalViewController -> shader/UI`

### Construcción del runtime térmico
`ThermalSimulationManager` recorre los `ExplodablePart` presentes en escena y construye un conjunto de nodos térmicos. Para cada nodo resuelve:

    * identificador de pieza;
    * clase de fuente térmica;
    * temperatura actual y temperatura de equilibrio;
    * temperatura mínima, de hover y pico;
    * tiempo de calentamiento;
    * tasa de enfriamiento;
    * exposición;
    * peso de la fuente;
    * escala de conducción;
    * criticidad térmica.

Luego construye enlaces térmicos mediante dos estrategias:

    * **grafo autoritativo,** si existe un `ThermalContactGraphAsset` válido;
    * **enlaces fallback,** si el grafo no está disponible.

### Parámetros base del solver

    * temperatura ambiente por defecto: **20\,°C**;
    * escala temporal de simulación: **4x**;
    * frecuencia objetivo del solver: **20 Hz**;
    * actualización visual térmica: **12 Hz**.

### Modelo de actualización térmica
En cada paso de simulación, el sistema combina tres contribuciones: acercamiento a equilibrio, enfriamiento hacia ambiente y conducción con vecinos.

#### Aproximación a equilibrio
Primero se calcula un factor de mezcla exponencial:
\[
\alpha = 1 - e^{-\Delta t / \tau}
\]
donde $\Delta t$ es el paso de simulación y $\tau$ representa el tiempo característico de calentamiento.

La contribución de la fuente se calcula como:
\[
\Delta T_{\text{fuente}} = (T_{\text{eq}} - T)\,\alpha\,w_s
\]
donde $T_{\text{eq}}$ es la temperatura de equilibrio para la carga actual, $T$ la temperatura presente y $w_s$ el peso de la fuente térmica.

#### Enfriamiento
El enfriamiento simplificado se expresa como:
\[
\Delta T_{\text{enfriamiento}} = (T_{\text{amb}} - T)\,k_c\,e\,\Delta t
\]
donde $T_{\text{amb}}$ es la temperatura ambiente, $k_c$ la tasa de enfriamiento y $e$ el factor de exposición del componente.

#### Conducción entre nodos
Cada enlace térmico agrega un intercambio:
\[
\Delta T_{A \leftrightarrow B} = (T_B - T_A)\,G_{\text{eff}}\,\Delta t
\]
donde $G_{\text{eff}}$ es la conductancia efectiva del vínculo.

Cuando existe un grafo térmico autoritativo, la conductancia base parte de:
\[
G_{\text{base}} = \max\left(0.001,\; s \cdot \frac{A}{L}\right)
\]
donde $s$ es la escala de conducción del enlace, $A$ el área de contacto y $L$ la longitud del trayecto. Luego el sistema la ajusta con escalas de material, criticidad y parámetros propios de los nodos conectados.

#### Actualización final
La temperatura nueva del nodo se calcula como:
\[
T_{\text{nuevo}} =
\operatorname{clamp}
\left(
T + \Delta T_{\text{fuente}} + \Delta T_{\text{enfriamiento}} + \sum \Delta T_{vecinos},
T_{\min},
T_{\max}
\right)
\]

Este modelo es coherente con la implementación real y suficientemente ligero para ejecutarse en runtime dentro del visor WebGL.

### Relación con el estado operativo
La carga efectiva depende del estado del dron:

    * `Off` y `ShuttingDown` llevan la carga a cero;
    * `StartingUp` establece una carga baja inicial;
    * `Idle` aplica un piso operativo mínimo;
    * `Flying` impone una carga mínima asociada a vuelo.

Con esta lógica, la simulación térmica responde de manera coherente a los controles visibles de `Power` y `Load`.

### Traducción de temperatura a render
`ThermalViewController` no dibuja simplemente un color uniforme por pieza. Primero resuelve un valor normalizado:
\[
n = \operatorname{InverseLerp}(T_{\text{disp,min}}, T_{\text{disp,max}}, T)
\]
con $T_{\text{disp,min}}=20\,^{\circ}\text{C}$ y $T_{\text{disp,max}}=100\,^{\circ}\text{C}$ en la configuración actual visible.

Luego construye una banda térmica:
\[
n_{\min} = \operatorname{clamp01}(n-b),
\qquad
n_{\max} = \operatorname{clamp01}(n+b)
\]
donde $b$ es el ancho medio de banda, distinto para piezas críticas, activas o pasivas.

Finalmente, el controlador envía al shader propiedades por `MaterialPropertyBlock`, entre ellas:

    * rango térmico mínimo y máximo;
    * hotspot local;
    * dirección de propagación;
    * *spread*;
    * enfriamiento de bordes;
    * variación base;
    * propagación.

### Patrones de superficie
La capa visual clasifica la distribución térmica de cada pieza en patrones como:

    * **radial,** típico de motores, batería o núcleo electrónico;
    * **axial,** típico de brazos, ESC o rieles;
    * **uniforme,** típico de placas y ciertos elementos estructurales.

Esta clasificación puede provenir de un perfil explícito de superficie térmica o inferirse desde la pieza canónica y su geometría.

### Leyenda térmica
La leyenda del modo `Thermal` se enlaza desde `ThermalViewController` con tres elementos de UI:

    * etiqueta mínima;
    * etiqueta máxima;
    * gradiente visual generado en runtime.

El gradiente recorre un espectro que va de tonos fríos oscuros a zonas claras de alta energía. Su función es interpretativa, no metrológica.

## Estados funcionales del código

p{8.1cm}}

**Estado** | **Contenido** 

**Estado** | **Contenido** 

Expuesto en UI final | Hero, selección de piezas, *bottom sheet*, `Inspect`, `Analyze`, `Studio`, presets de entorno, leyenda térmica 

Implementado pero oculto | medición, modos `Blueprint`, `Wireframe`, `Ghosted`, `Realistic` como botones, partes avanzadas del flujo térmico 

Código legado o no integrado | `PartCatalogUI`, `SettingsPanel`, `LoadingController`, partes de `EnhancedInfoPanel`, suite de ensamblaje y anotaciones 

Trabajo futuro | optimizaciones finales CAD/import, cierre definitivo de usabilidad, audio final si llega a integrarse y validarse 

## Tooling de editor

    * **ProjectSetupWizard.** Asistente de estructura inicial de escena.
    * **ImportedDroneCoverageAudit.** Auditoría de cobertura entre jerarquía importada y catálogo canónico.
    * **ThermalContactGraphBuilderWindow.** Herramienta offline para generación del grafo térmico autoritativo.
    * **WebGLBuildFixer y utilidades Antigravity.** Ajustes de configuración y build.

## Módulos que no deben documentarse como activos
Este manual excluye como módulos activos finales a:

    * `ViewModeToolbar`
    * `WebGLOptimizer`
    * `ScreenshotManager`
    * `KeyboardShortcuts`
    * `PerformanceMonitor`
    * `RuntimeConsole`
    * `SceneTransitionManager`
    * `TooltipSystem`

## Build, validación y despliegue

    * abrir el proyecto con la versión de Unity documentada en el repositorio;
    * verificar que la escena activa sea `MainScene\_Final`;
    * asegurar que el binding runtime del dron y los managers mínimos estén presentes;
    * generar build WebGL;
    * validar jerarquía, cobertura, flujo visible y leyenda térmica antes de capturas o pruebas de usuario;
    * congelar la build antes de ejecutar resultados finales de usabilidad.

## Limitaciones vigentes del cierre

    * aun falta validar en Unity el FBX runtime final con masters, instancias, fasteners, hotspots, filtros y helices;
    * aun falta aplicar y validar las texturas/materiales finales antes del *freeze* definitivo;
    * los efectos de audio no deben presentarse como parte del cierre funcional verificado si no existen assets finales;
    * la herramienta de medición permanece oculta hasta validar mejor escala y usabilidad;
    * el subsistema térmico es coherente con la app real, pero debe documentarse como simulación híbrida heurística, no como FEA;
    * los resultados académicos finales dependen de KPIs y pruebas de usabilidad posteriores al freeze.