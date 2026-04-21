# Arquitectura técnica de animaciones procedurales del onboarding

Este documento describe la implementación técnica del sistema de animaciones del onboarding runtime construido para la aplicación WebGL en Unity UI Toolkit. Su función es equivalente a la documentación creada para los íconos procedurales del menú, pero enfocada en el subsistema que explica gestos, microinteracciones, selección jerárquica, herramientas de análisis y controles visuales dentro del onboarding.

## 1. Objetivo del subsistema

El onboarding no debía depender de video, GIF, spritesheets, SVG externos ni capturas pre-renderizadas. El objetivo técnico fue construir un sistema que:

- explicara interacciones reales de la aplicación;
- reutilizara el lenguaje visual ya presente en la UI;
- funcionara igual en WebGL, desktop y mobile;
- mantuviera peso prácticamente nulo en disco;
- permitiera corregir timing, geometría y feedback visual por código.

La decisión final fue usar un renderer procedural sobre `Painter2D`, de forma análoga a los íconos procedurales, pero a una escala mayor: cada card del onboarding se convirtió en una escena animada compuesta por actor de interacción, objetivo y respuesta del sistema.

## 2. Archivos clave del sistema

La implementación final quedó distribuida en cuatro piezas principales:

- `Assets/Scripts/UI/Panels/OnboardingAnimationView.cs`
- `Assets/Scripts/UI/Panels/OnboardingController.cs`
- `Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `Assets/UI/Styles/Theme.uss`

### Responsabilidad de cada archivo

**`OnboardingController.cs`**

- define las `15` cards del onboarding;
- almacena el copy, `cueSequence`, títulos, descripción y footer;
- controla navegación, switch `PC/mobile`, persistencia y estado del overlay;
- sincroniza el texto con la fase visual activa;
- instancia y alimenta a `OnboardingAnimationView`.

**`OnboardingAnimationView.cs`**

- es el motor procedural de las demos;
- hereda de `VisualElement`;
- renderiza cada escena en `generateVisualContent`;
- calcula fases, tiempos, easing, actor, feedback y resultado;
- abstrae desktop y mobile dentro del mismo motor;
- dibuja botones, sliders, panels, siluetas, cortes, halos, órbitas y gestos.

**`MainLayout.uxml`**

- define el contenedor del onboarding y el stage donde se renderiza la demo;
- reserva overlays auxiliares para labels del tab `INFO` y tarjetas de `STUDIO`.

**`Theme.uss`**

- fija la escala del panel;
- define la jerarquía visual del viewport vertical;
- alinea spacing, footer, zona de texto y overlays del renderer.

## 3. Razón de diseño: por qué procedural y no media pre-renderizada

El sistema se diseñó con la misma filosofía que los íconos del menú:

- nitidez infinita a cualquier resolución;
- cero dependencia de assets rasterizados;
- capacidad de retocar geometría, timing y easing en código;
- coherencia cromática con el overlay real;
- posibilidad de reutilizar los mismos mini íconos y microinteracciones del menú.

En WebGL esto tiene una ventaja adicional: el onboarding no añade paquetes de video, no exige reproducción multimedia y no incrementa el tamaño del build con secuencias prerenderizadas. Todo se compone por líneas, rectángulos, círculos, polígonos y labels runtime.

## 4. Arquitectura general del renderer

La arquitectura se divide en tres niveles.

### 4.1. Nivel de datos: definición de pasos

`OnboardingController` define una lista de `Step` con:

- `caption`
- `cuePrimary`
- `cueSequence`
- `title`
- `description`
- `footerText`
- `visualMode`
- `visualGlyph`

Las cards visibles del sistema son:

1. `Navigate`
2. `Select / Deselect`
3. `Part Info`
4. `Inspect`
5. `Pins`
6. `Isolate`
7. `Power`
8. `Analyze`
9. `Cut`
10. `Explode`
11. `Filter`
12. `Studio`
13. `Render Mode`
14. `Environment`
15. `Lighting Controls`

### 4.2. Nivel de reproducción: timeline por fases

Cada escena se resuelve como una secuencia finita de fases. El método `GetPhaseDurations(OnboardingSceneId scene)` devuelve un arreglo de duraciones en milisegundos. Ejemplo:

- `Navigate`: `3400, 5000, 5400`
- `Cut`: `6200, 4400, 6400`
- `Environment`: `4600, 12000, 5600`

Esto permite que cada card tenga un ritmo propio sin duplicar lógica de reloj en cada método.

### 4.3. Nivel de render: composición procedural

`OnGenerateVisualContent` calcula:

- `phaseIndex`
- `phaseT`
- `loopT`

y delega en un método por escena:

- `DrawNavigateScene`
- `DrawSelectScene`
- `DrawPartInfoScene`
- `DrawModeMenuScene`
- `DrawStudioMenuScene`
- `DrawPinsScene`
- `DrawIsolateScene`
- `DrawPowerScene`
- `DrawCutScene`
- `DrawExplodeScene`
- `DrawFilterScene`
- `DrawRenderModeScene`
- `DrawEnvironmentScene`
- `DrawLightingScene`

Cada uno dibuja la escena completa en un solo canvas procedural.

## 5. Motor temporal y gramática de interacción

La base del sistema es la gramática:

1. mover actor;
2. hacer contacto;
3. ejecutar acción;
4. mostrar respuesta;
5. sostener el estado final lo suficiente para que se lea;
6. cerrar el loop sin saltos bruscos.

### 5.1. Cálculo de timeline

`ComputeTimeline`:

- suma todas las duraciones de la escena;
- calcula el tiempo transcurrido desde `_animationStartTime`;
- hace `mod` para obtener el tiempo dentro del loop;
- ubica en qué fase está la animación;
- normaliza el tiempo interno de esa fase a `0..1`.

De esta forma, la demo no depende de coroutines ni de objetos temporales por escena.

### 5.2. Easing helpers

El sistema usa una familia pequeña de helpers para evitar movimientos lineales:

- `Phase01(t, start, end)`
- `EaseOut(t)`
- `EaseInOut(t)`
- `EaseOutBack(t, overshoot)`

`EaseOutBack` se usa cuando conviene un pequeño sobrepaso, por ejemplo en botones o explosión. `EaseInOut` se reserva para movimientos de ida y vuelta o desplazamientos donde interesa entrada y salida suaves.

### 5.3. Separación entre aproximación, click y acción

Para que la interacción se sienta profesional se separaron explícitamente estas etapas:

- `GetApproachProgress`
- `GetPressProgress`
- `GetPressReleaseProgress`
- `GetActionProgress`
- `GetClickPulseProgress`
- `GetSecondClickProgress`
- `GetDoubleClickActionProgress`

Eso evita uno de los problemas más comunes de UX motion en demos: que el feedback de click salga antes de que el actor llegue al objetivo o que el resultado del sistema ocurra antes del contacto.

### 5.4. Drags bidireccionales y sliders

`EvaluateRoundTripDrag` resuelve el patrón:

- salir desde valor inicial;
- llegar a un pico;
- regresar a un valor final;
- con la misma curva temporal que usa el actor visual.

Este helper es el corazón de:

- `Cut`
- `Explode`
- `Lighting Controls`
- cualquier slider que debía ir a la derecha y volver a su origen.

## 6. Abstracción desktop vs mobile

El onboarding muestra la misma intención de UX con dos actores distintos:

- desktop: cursor simplificado;
- mobile: punto de contacto, ripple o pinch.

La selección de variante depende de `_isTouchMode`.

### 6.1. Actor desktop

Se renderiza con:

- `DrawCursorActor`
- `DrawMouseHint`
- `DrawWheelMouseHint`

La convención visual usada fue:

- click izquierdo: depresión breve del cursor y pulso en el target;
- click derecho o middle click: hint específico del mouse;
- wheel: una rueda grande y legible con flecha de dirección;
- drag: trail dibujado únicamente después de iniciarse el movimiento.

### 6.2. Actor mobile

Se renderiza con:

- `DrawTouchActor`
- `DrawPinchActor`
- `DrawRipple`

La convención visual fue:

- `tap`: punto + ripple;
- `double tap`: doble pulso separado y visible;
- `drag`: punto con estela;
- `pinch`: dos contactos y separación/contracción legible.

La regla principal fue no usar manos ilustradas ni gestos fotorealistas. Todo se mantuvo abstracto y funcional.

## 7. Primitivas visuales reutilizables

La mayor parte del valor técnico del sistema no está en cada card individual sino en las primitivas compartidas.

### 7.1. Feedback de objetivo

- `DrawTargetClickPulse`
- `DrawClickRing`
- `DrawRipple`
- `DrawDragTrail`

Estas funciones separan claramente:

- el instante de contacto;
- el feedback de pulsación;
- la respuesta visual del sistema.

### 7.2. Controles de interfaz

- `DrawMenuButton`
- `DrawStandaloneActionButton`
- `DrawSubmenuCardButton`
- `DrawAxisButton`
- `DrawRenderOptionButton`
- `DrawFilterButton`
- `DrawSlider`

Esto permite que los mismos principios de hover, press, active y release se reutilicen entre cards distintas sin reescribir geometría.

### 7.3. Geometría del dominio

- `DrawDroneSilhouette`
- `DrawWireDrone`
- `DrawExplodedDrone`
- `DrawPowerHeatLayout`
- `DrawSelectionAssembly`
- `GetSelectionAssemblyRects`

El dron simplificado, las piezas del ensamblaje y el layout térmico se convierten en vocabulario visual compartido entre navegación, selección, explosión, filtros y modos de render.

### 7.4. Operaciones geométricas especializadas

Para `Cut` se construyó un pequeño subsistema geométrico propio:

- `DrawNestedHorizontalCutPair`
- `DrawNestedQuarterCutPair`
- `DrawSharedDiagonalCutPair`
- `DrawDiagonalCutBox`
- `DrawCutRectAgainstPlane`
- `DrawRectBorderSegments`

Estas funciones permiten recortar rectángulos contra planos horizontales, verticales y diagonales sin depender de meshes ni geometría 3D real.

## 8. Explicación técnica card por card

### 8.1. `Navigate`

**Objetivo visual**

Explicar orbit, zoom, pan y reset usando el mismo dron simplificado.

**Construcción**

- el dron se dibuja con `DrawDroneSilhouette`;
- la cámara se representa por transformaciones del propio dron:
  - `droneRotation` para orbit;
  - `droneScale` para zoom;
  - `panOffset` para pan;
- el botón `Reset View` se dibuja con `DrawIconButton` usando el mini ícono de reset;
- en desktop aparecen hints grandes de mouse:
  - RMB para orbit;
  - wheel para zoom;
  - middle click para pan.

**Secuencia**

1. orbit
2. zoom in
3. zoom out
4. pan
5. reset

**Punto técnico importante**

El reset no ocurre durante la transformación previa, sino después del click explícito sobre el botón de reset. Esto permite cerrar el loop con una causalidad clara.

### 8.2. `Select / Deselect`

**Objetivo visual**

Explicar la jerarquía de selección:

- una pieza madre;
- varias subpiezas;
- una pieza vecina que no pertenece al grupo.

**Construcción**

- `neighbor`, `parentParts` y `parentGroup` definen la composición;
- `DrawPartNode` dibuja cada pieza;
- `DrawSelectionHalo` se usa solo cuando se está seleccionando, no cuando se está deseleccionando;
- `SelectedFill` y `ParentSelectedFill` mantienen la lectura jerárquica.

**Secuencia**

1. click en la pieza madre;
2. highlight de todo el conjunto;
3. click en la subpieza;
4. highlight intenso solo en la subpieza;
5. click fuera del grupo;
6. regreso progresivo al estado inicial.

**Punto técnico importante**

El click final de background no es decorativo: sincroniza el apagado del highlight y permite que el loop reinicie donde comenzó.

### 8.3. `Part Info`

**Objetivo visual**

Mostrar que el tab `INFO` aparece solo después de seleccionar una pieza y que el panel sube desde abajo.

**Construcción**

- `GetPartInfoRects` calcula posición de tab y bottom sheet;
- `DrawPartInfoPeek` muestra el asomo de la pestaña;
- `DrawBottomSheetPanel` construye el panel completo con su botón circular de cierre;
- `_partInfoTabLabel` se usa como overlay para mantener el texto `INFO` nítido.

**Secuencia**

1. selección de la pieza;
2. aparición del tab;
3. click sobre `INFO`;
4. apertura del panel;
5. click en `X`;
6. click en background;
7. retorno al punto inicial.

**Punto técnico importante**

El puntero no acompaña el panel al cerrarse. Se fija sobre el botón `X`, luego se desplaza al background y cierra el loop limpiamente.

### 8.4. `Inspect`

**Objetivo visual**

Explicar el menú principal `Inspect` y la secuencia de subopciones.

**Construcción**

`DrawModeMenuScene` se parametriza con:

- ícono principal;
- item A;
- item B;
- item C.

En `Inspect` esos ítems son:

- `Pins`
- `Isolate`
- `Power`

**Comportamiento**

- el botón principal crece y reproduce su microinteracción;
- aparecen subbotones grandes y legibles;
- el botón activo conserva el azul hasta que se selecciona el siguiente;
- el cierre del menú se hace en reversa para cerrar el loop.

### 8.5. `Pins`

**Objetivo visual**

Demostrar activación de hotspots y selección de un grupo.

**Construcción**

- el dron se dibuja más grande que en otras cards para dar prioridad a los pins;
- `DrawPinMarker` genera tres marcadores;
- el marcador seleccionado cambia de color y activa un highlight de grupo;
- el loop termina con click en background para deseleccionar.

### 8.6. `Isolate`

**Objetivo visual**

Mostrar el flujo jerárquico real de aislamiento.

**Construcción**

- reutiliza la lógica de ensamblaje de selección;
- parte desde una pieza madre ya seleccionada;
- la segunda interacción es un doble click o doble tap claramente marcado;
- se añade un hold largo al frame final para que el usuario alcance a leer el resultado.

**Secuencia**

1. activar isolate con la pieza madre seleccionada;
2. desaparecer el contexto externo;
3. doble click sobre subpieza;
4. dejar visible solo esa subpieza;
5. volver un nivel;
6. desactivar isolate y restaurar el conjunto.

### 8.7. `Power`

**Objetivo visual**

Explicar:

- encendido;
- variación de carga por slider;
- visual térmica simplificada;
- apagado para cerrar el loop.

**Construcción**

- el dron se representa más grande;
- `DrawPowerHeatLayout` dibuja la distribución térmica simplificada:
  - cuadrados cálidos en hélices;
  - banda central amarilla;
  - bandas azules alargadas;
- el slider usa la misma gramática que la UI real;
- el botón `POWER` se vuelve a clickear al final para apagar.

### 8.8. `Analyze`

`Analyze` reutiliza `DrawModeMenuScene` igual que `Inspect`, pero con:

- `Cut`
- `Explode`
- `Filter`

La secuencia visual es la misma:

- click en el botón del menú;
- expansión del botón activo;
- reproducción de microinteracción de cada subopción;
- cierre en reversa para completar el loop.

### 8.9. `Cut`

**Objetivo visual**

Representar el comportamiento del sistema de corte sin depender del modelo 3D real.

**Construcción**

- dos rectángulos anidados representan el volumen exterior e interior;
- cinco botones controlan:
  - `X`
  - `Y`
  - `Z`
  - `Invert`
  - `Angle`
- un slider controla profundidad del corte.

**Secuencia**

1. click en `X`;
2. aparición del plano horizontal;
3. movimiento del slider con el plano desplazándose;
4. click en `Y`;
5. aparición del plano vertical y recorte al cuarto superior;
6. click en `Angle`;
7. transición a una sola diagonal compartida;
8. click en `Invert`;
9. inversión de la mitad visible.

**Punto técnico importante**

El reto no era solo dibujar líneas, sino recortar correctamente los bordes visibles para evitar residuos y diagonales duplicadas. Por eso se segmentaron los bordes y se separó el dibujo del volumen del dibujo del plano de corte.

### 8.10. `Explode`

**Objetivo visual**

Mostrar separación jerárquica de partes con un slider sincronizado.

**Construcción**

- parte del mismo dron simplificado;
- `DrawExplodedDrone` separa brazos, cuerpo y capas superiores;
- el botón activa un valor base de explosión;
- el slider aparece ya en el punto medio;
- desde ahí el valor aumenta y disminuye sincronizado con la separación física.

**Punto técnico importante**

La separación de piezas no depende solo del click del botón; después del primer estado, el `sliderValue` controla directamente la amplitud de `explode`, lo que hace que el movimiento de la UI y el del modelo parezcan un solo sistema.

### 8.11. `Filter`

**Objetivo visual**

Explicar tres cosas distintas:

- desactivar un filtro;
- reactivarlo;
- aislarlo con doble click;
- restaurar todo.

**Construcción**

- seis botones circulares, ya sin cuadrícula rígida;
- seis piezas simplificadas del dron, una por categoría;
- un índice de foco controla cuál botón/categoría se usa en la demo;
- el doble click es estacionario: ya no existe movimiento del cursor entre ambos clicks.

**Secuencia**

1. click para desactivar;
2. salida corta del puntero;
3. regreso al botón;
4. click para reactivar;
5. pequeño movimiento fuera y dentro;
6. doble click sin mover el puntero;
7. aislamiento del filtro;
8. click final para restaurar;
9. retorno al punto inicial.

### 8.12. `Studio`

**Objetivo visual**

Diferenciar `Studio` de `Inspect` y `Analyze`.

**Construcción**

- primero se muestra el botón inferior del menú `Studio`;
- solo después del click aparecen los paneles superiores;
- `_studioRenderLabel` y `_studioEnvironmentLabel` dibujan los títulos como overlays nítidos;
- los textos crecen junto con sus rectángulos.

Esto evita reciclar la estructura radial de los otros menús y comunica mejor que `Studio` es un espacio de presentación visual, no de inspección operativa.

### 8.13. `Render Mode`

**Objetivo visual**

Explicar la transición de estado entre:

- `XRay`
- `Solid`
- `Thermal`
- vuelta al estado base

**Construcción**

- `DrawWireDrone` se usa para `XRay`;
- `DrawDroneSilhouette` para `Solid`;
- `DrawPowerHeatLayout` reaparece para `Thermal`;
- el botón activo permanece azul hasta el siguiente click, no por simple cambio de fase.

### 8.14. `Environment`

**Objetivo visual**

Mostrar tres subdominios distintos:

- `Studio`
- `Time`
- `Color`

**Construcción**

- el viewport usa fondos interpolados por color;
- `DrawBlueprintLines` activa el look blueprint;
- el sol y la luna siguen una órbita elíptica con `EllipsePoint`;
- la fase `Time` se compone de varios clicks separados:
  - día
  - atardecer
  - noche
- el botón `Color` solo entra después de terminar el bloque de tiempo.

**Punto técnico importante**

El sol y la luna no se mueven en línea recta. La trayectoria se desplaza angularmente sobre una elipse, con un horizonte fijo dentro del recuadro, para que el ciclo se lea como día-atardecer-noche y no como traslación arbitraria.

### 8.15. `Lighting Controls`

**Objetivo visual**

Explicar tres sliders independientes:

- rotación de luz;
- intensidad del objeto;
- tono del fondo.

**Construcción**

- `DrawLightBall` resume la dirección de luz con línea y esfera;
- tres sliders apilados reutilizan el mismo patrón de ida y vuelta;
- `DrawActorTwoSegmentDrag` recorre cada slider con continuidad desde el contacto anterior;
- la esfera amarilla crece y se aleja más de la línea cuando sube la intensidad.

## 9. Relación con los íconos procedurales del menú

Este subsistema no se diseñó aislado. Toma prestadas varias decisiones de la arquitectura de íconos:

- reutiliza el mismo repertorio de mini íconos (`OnboardingMiniIcon`);
- usa el mismo enfoque procedural con `Painter2D`;
- respeta el mismo vocabulario de press, hover, active y release;
- replica microinteracciones en versiones ampliadas cuando la card necesita enseñar la acción con mayor claridad.

En lugar de tratar el onboarding como un video incrustado, se trató como una extensión pedagógica del propio sistema visual de la aplicación.

## 10. Consideraciones de rendimiento y WebGL

Las decisiones de performance fueron deliberadas:

- un solo `VisualElement` renderer por stage;
- overlays mínimos para texto cuando hacía falta nitidez perfecta;
- sin texturas grandes;
- sin paquetes de video;
- sin `SVG` runtime;
- sin dependencia de spritesheets.

`schedule.Execute(UpdateFrame).Every(16)` mantiene el renderer vivo aproximadamente a 60 FPS, pero el costo visual sigue siendo bajo porque la escena solo repinta líneas y polígonos 2D simples.

## 11. Resultado del MVP y estado actual

A abril de 2026 el sistema ya cumple con estos objetivos:

- las `15` cards tienen animación procedural runtime;
- desktop y mobile tienen actor propio;
- los loops son autónomos;
- los menús y sliders se alinean con la UI real;
- el onboarding puede iterarse por código sin regenerar media.

Lo que todavía queda fuera del alcance de este documento es la validación perceptual final con usuarios y la captura formal de evidencias visuales. El sistema ya está implementado; lo pendiente es la última etapa de QA fino, ritmo y legibilidad en runtime real.

## 12. Conclusión

El onboarding terminó convirtiéndose en un segundo subsistema procedural de UI, paralelo al de íconos. La diferencia es que aquí no solo se animan símbolos, sino interacciones completas. Técnicamente, el valor del trabajo no está únicamente en las cards individuales, sino en haber construido una gramática reusable de:

- actor;
- contacto;
- feedback;
- respuesta;
- hold;
- loop.

Ese enfoque permitió demostrar navegación, selección, inspección, análisis y presentación visual con el mismo lenguaje matemático y procedural que ya define el resto de la interfaz.
