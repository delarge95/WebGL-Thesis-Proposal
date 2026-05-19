# Manual de Usuario

## Introducción
Este manual documenta el uso de la build final del visor WebGL desarrollado para la visualización técnica del dron Holybro X500~V2. Su propósito es orientar al usuario en el recorrido real de la aplicación, explicar qué información puede consultar, cómo interpretar las herramientas visibles y qué límites deben tenerse en cuenta durante la evaluación del sistema.

El documento se concentra únicamente en el flujo verificable de la versión final. Por esa razón no presenta como funciones de usuario herramientas ocultas, experimentales o legadas que todavía existan en código, pero que no formen parte de la interfaz final.

### Alcance
La aplicación permite:

    * ingresar desde la pantalla Hero al visor 3D;
    * navegar el modelo del dron mediante cámara orbital;
    * seleccionar piezas y consultar su ficha técnica en una ficha inferior o *bottom sheet*;
    * inspeccionar el ensamblaje con hotspots, aislamiento y control de estado operativo;
    * analizar el modelo con corte transversal, vista explosionada y filtros funcionales;
    * cambiar modos de visualización, presets de entorno e interpretación térmica.

### Público objetivo
Este manual está orientado a:

    * estudiantes y docentes de ingeniería multimedia o áreas afines;
    * evaluadores académicos del proyecto;
    * usuarios que necesiten comprender la estructura y comportamiento general del dron sin abrir un entorno CAD o un motor 3D.

## Contexto general de la aplicación
La aplicación no es un catálogo genérico de múltiples productos. La versión final está centrada en un único caso de estudio: el dron Holybro X500~V2. El modelo se organiza como una escena interactiva en la que el usuario puede alternar entre lectura descriptiva, análisis estructural y visualización técnica del estado del sistema.

El flujo principal del visor es:

`Hero -> Explore -> selección de pieza -> bottom sheet -> Inspect / Analyze / Studio`

Este flujo resume la estructura real de la interfaz. El botón `HOME` regresa al Hero y el botón `RESET VIEW` restablece la vista principal del dron.

## Requisitos y acceso

### Requisitos mínimos

    * navegador moderno con soporte WebGL~2.0;
    * conexión estable para la carga inicial del contenido;
    * mouse o pantalla táctil para navegación 3D;
    * resolución suficiente para leer la interfaz flotante y la leyenda térmica.

### Ingreso y pantalla Hero

    * Abrir la URL pública del prototipo o el build local de prueba.
    * Esperar la carga inicial del visor.
    * Revisar la pantalla Hero, que presenta el proyecto, el caso de estudio y accesos secundarios.
    * Presionar `EXPLORE` para ingresar al modelo interactivo.

En la versión final, la pantalla Hero también ofrece acceso al repositorio fuente y al tutorial o ayuda inicial. El botón de tutorial activa la guía de onboarding del visor.

### Onboarding inicial y ayuda contextual
La primera vez que el usuario entra al visor, la aplicacion puede mostrar un onboarding guiado. Esta ayuda no es un video externo: es una secuencia de tarjetas animadas dentro de la propia interfaz que explica los controles reales del producto.

El onboarding actual resume:

    * navegacion de camara en desktop y mobile;
    * seleccion y deseleccion por niveles;
    * apertura de `Part Info`;
    * herramientas de `Inspect`, `Analyze` y `Studio`;
    * comportamiento de sliders, filtros y modos visuales.

El usuario puede volver a abrir esta ayuda desde el boton `HELP` del Hero cuando necesite recordar el flujo de uso.

## Estructura general de la interfaz

\begin{table}[htbp!]
\centering
\caption{Zonas principales de la interfaz final}
\begin{tabular}{p{4.2cm}p{9.5cm}}

**Zona** | **Función** 

Hero | Pantalla de entrada, contexto del proyecto y accesos iniciales 

Viewport 3D | Visualización principal del dron y de las herramientas de análisis 

Barra superior | Botones `HOME`, `RESET VIEW` y contexto de selección 

Barra inferior | Accesos a modos principales y acciones contextuales 

Info bar / peek bar | Indicador resumido de la pieza seleccionada cuando la ficha está cerrada 

Bottom sheet | Ficha técnica detallada de la selección actual 

Paneles flotantes | Subpaneles de corte, filtros, visualización y entorno 

Leyenda térmica | Referencia visual del rango de temperatura en el modo `Thermal` 

\end{tabular}
\end{table}

Cuando no hay una pieza seleccionada, la interfaz mantiene el enfoque en el modelo general. Cuando sí existe una selección, el sistema muestra una etiqueta contextual superior, una etiqueta inferior de selección y una barra de previsualización que permite abrir la ficha completa.

## Controles de cámara y navegación

\begin{table}[htbp!]
\centering
\caption{Controles verificados en la build final}
\begin{tabular}{p{4.2cm}p{5.1cm}p{4.1cm}}

**Acción** | **Mouse** | **Táctil** 

Orbitar | clic derecho + arrastrar | un dedo + arrastrar 

Pan | clic central + arrastrar | dos dedos + arrastrar 

Zoom | rueda del mouse | gesto de pinza 

Cerrar ficha o subpanel | tecla `Esc` | cierre desde la interfaz 

Restablecer vista | botón `RESET VIEW` | botón `RESET VIEW` 

Regresar al Hero | botón `HOME` | botón `HOME` 

\end{tabular}
\end{table}

La tecla `Esc` sigue una prioridad interna importante:

    * primero cierra la ficha inferior si está abierta;
    * si la ficha no está abierta, cierra el subpanel activo o desactiva el modo actual;
    * si no hay paneles abiertos, no ejecuta acciones adicionales.

En la versión final no se documentan atajos adicionales de teclado porque no forman parte del flujo visible verificado para la entrega.

## Selección de piezas y consulta de información

### Selección básica

    * Hacer clic sobre una pieza visible del dron.
    * El sistema resalta la selección actual.
    * La interfaz actualiza el contexto superior y la etiqueta inferior con el nombre de la pieza.
    * Si la ficha está cerrada, aparece una barra de previsualización que permite abrir la información completa.

### Jerarquia de seleccion
La seleccion no siempre termina en el primer clic. La build final maneja una jerarquia de lectura:

    * **Pieza madre:** el primer clic sobre una zona visible puede seleccionar el conjunto principal al que pertenece esa geometria.
    * **Subpieza:** si el usuario vuelve a hacer clic sobre un elemento interno de ese conjunto, la seleccion puede descender a un nivel mas especifico.
    * **Grupo de hotspot:** si la seleccion nace desde un pin, la lectura puede representar un sistema o grupo funcional en lugar de una sola pieza.

Para retroceder dentro de esa jerarquia, el usuario puede hacer clic sobre el fondo o usar doble clic sobre el fondo cuando existe aislamiento activo.

### Apertura de la ficha técnica
La información detallada se presenta en una ficha inferior o *bottom sheet*. Esta es la superficie oficial de consulta de la build final y reemplaza cualquier referencia previa a paneles laterales no presentes en la interfaz visible.

La ficha puede abrirse de tres maneras:

    * mediante la pestaña o boton contextual de informacion que aparece cuando ya existe una seleccion;
    * desde la barra de previsualización inferior;
    * mediante un gesto ascendente sobre el área de acciones cuando ya existe una selección.

### Contenido de la ficha
La ficha organiza la información del componente en tres bloques editoriales:

    * **Identification:** nombre de la pieza, categoría, función y descripción.
    * **Specifications:** material, peso, dimensiones, consumo y temperatura de operación cuando el dato existe.
    * **Assembly:** dificultad, herramientas requeridas y tiempo estimado de instalación.

Algunos campos pueden mostrarse como `N/A`. Esto no representa un error de la interfaz; significa que ese dato no se encuentra cargado para la pieza seleccionada dentro de `DronePartData`.

Cuando la selección corresponde a un fastener individual, la ficha añade datos técnicos específicos como familia, métrica, longitud, material, referencia CAD y pieza canónica más cercana dentro del dron. El detalle visual que aparece para estos fasteners en la escena actual es un placeholder ligero de Unity, no la malla final de Blender.

Cuando la selección corresponde a una pieza madre, la ficha puede añadir el desglose del ensamblaje con subpiezas documentadas y el resumen de fasteners configurados para ese conjunto. Esta lista refleja la configuración actual de la app aunque parte de la geometría visible siga siendo temporal.

En los fasteners, el color de *hover* y el color de selección deben mantenerse aunque el proxy temporal se oculte para mostrar el detalle procedural. Esto permite inspeccionar la pieza sin perder confirmación visual de qué instancia está activa.

### Selecciones desde hotspots
Cuando la selección proviene de un hotspot de grupo, la ficha puede resumir un sistema completo y no solo un componente individual. En ese caso:

    * el título puede representar el grupo funcional;
    * la categoría puede mostrarse como grupo de sistema;
    * la descripción puede incluir una lista resumida de componentes asociados.

Este comportamiento es importante porque los hotspots no funcionan solo como marcadores visuales. Tambien permiten entrar a una lectura agrupada del sistema y luego abrir la misma ficha inferior usada por las piezas y subpiezas.

### Interacciones avanzadas de selección
Además del clic simple, la build final conserva dos interacciones útiles:

    * **Segundo clic sobre una geometria interna:** permite pasar de pieza madre a subpieza cuando existe ese nivel.
    * **Doble clic sobre una pieza o subpieza:** aísla el nivel actual y abre la ficha.
    * **Doble clic sobre el fondo:** retrocede un nivel de aislamiento o limpia el aislamiento actual.

Cuando la seleccion activa es un fastener, ese doble clic se interpreta sobre la instancia completa del fastener y no sobre una submalla parcial. De esta manera el aislamiento conserva visible el tornillo o elemento de union completo.

## Modos principales de trabajo
La barra inferior organiza la experiencia en tres modos visibles: `Inspect`, `Analyze` y `Studio`. Cada modo cambia la lectura del modelo, pero el usuario puede seguir seleccionando piezas y abriendo la ficha técnica.

\begin{table}[htbp!]
\centering
\caption{Resumen funcional de los modos visibles}
\begin{tabular}{p{3.2cm}p{10.6cm}}

**Modo** | **Propósito principal** 

`Inspect` | Lectura puntual del ensamblaje, hotspots, aislamiento y estado operativo del dron 

`Analyze` | Exploración estructural mediante corte, explosión y filtros por categoría 

`Studio` | Ajuste de visualización, presets de entorno, control de iluminación y vista térmica 

\end{tabular}
\end{table}

## Modo Inspect
El modo `Inspect` reúne herramientas de lectura puntual sobre el modelo.

\begin{table}[htbp!]
\centering
\caption{Herramientas visibles de Inspect}
\begin{tabular}{p{3cm}p{10.8cm}}

**Herramienta** | **Comportamiento** 

Pins | Activa o desactiva los hotspots visibles sobre el dron. Inicia activa por defecto al entrar por primera vez al visor. 

Isolate | Aísla la selección actual del resto del ensamblaje. Si existe una subselección, permite aislar niveles más específicos. 

Power | Enciende o apaga el estado operativo simulado del dron. 

Load | Ajusta el nivel de carga del sistema mientras el dron está encendido. 

\end{tabular}
\end{table}

### Hotspots
Los hotspots sirven como ayudas visuales para ubicar rápidamente zonas relevantes del sistema. El botón `Pins` conmuta su visibilidad. Si el usuario prefiere una lectura limpia del modelo, puede desactivarlos temporalmente sin perder la posibilidad de seleccionar piezas manualmente.

### Aislamiento
El botón `Isolate` oculta temporalmente el resto del modelo y conserva visible la selección activa. Esta función facilita la lectura espacial de componentes internos o de zonas que quedarían ocultas por el ensamblaje completo.

El comportamiento exacto depende del nivel activo:

    * si la seleccion actual es una pieza madre, se aísla ese conjunto;
    * si la seleccion actual es una subpieza, el aislamiento se aplica a ese nivel mas fino;
    * si la seleccion actual proviene de un hotspot, el aislamiento puede operar sobre el grupo funcional asociado;
    * si la seleccion actual es un fastener, el aislamiento conserva visible la instancia completa de esa fijacion; es decir, el tornillo o elemento de union completo y no solo una submalla o fragmento de su geometria.

Si la seleccion activa es un fastener, el aislamiento se aplica sobre la pieza completa de fijacion. El sistema mantiene visible el tornillo, tuerca, standoff o grommet completo aunque la seleccion se haya originado sobre una subparte de su geometria temporal.

Ademas, cuando ese fastener queda aislado, la app fuerza su reemplazo por el detalle modular temporal aunque la seleccion se limpie despues. De esta forma el aislamiento conserva una lectura detallada y no vuelve al proxy ligero mientras el isolate siga activo.

Si la seleccion activa es una pieza madre, el aislamiento conserva tambien los fasteners asociados que quedaron reconciliados para ese ensamblaje dentro de la configuracion actual de la app.

En ese caso, los fasteners asociados a la pieza madre aislada tambien pueden pasar a su representacion modular temporal. El resto de la tornilleria del dron permanece en proxy para no expandir innecesariamente el costo visual.

El aislamiento puede revertirse de dos maneras:

    * pulsando nuevamente `Isolate`;
    * haciendo doble clic sobre el fondo hasta regresar a la vista general.

### Estado operativo del dron
El botón `Power` no solo cambia un icono de interfaz. También altera el estado operativo interno del dron y sincroniza:

    * la velocidad de hélices;
    * las luces de estado;
    * el comportamiento de vuelo simulado;
    * la carga del sistema usada por la visualización térmica.

La etiqueta de estado puede mostrar los siguientes valores:

    * `OFF`
    * `STARTING`
    * `IDLE`
    * `FLYING`
    * `SHUTDOWN`

### Control de carga
El deslizador de carga representa la intensidad operativa del sistema. Su valor se expresa en porcentaje y sirve como entrada directa para la lógica del dron:

    * valores bajos mantienen el dron encendido pero en reposo operativo;
    * valores más altos llevan el sistema al estado `FLYING`;
    * el incremento de carga también eleva la intensidad térmica de los componentes críticos.

En otras palabras, el usuario puede usar `Power` y `Load` como controles de contexto para estudiar cómo cambia la visualización del sistema bajo distintos niveles de exigencia.

### Enfoque y zoom sobre piezas pequenas
Cuando el usuario selecciona o aisla una pieza, la camara ajusta su enfoque con base en el tamano real de la seleccion. Esto mejora especialmente la inspeccion de fasteners y subpiezas pequenas, ya que el zoom ya no queda limitado a una distancia fija demasiado grande.

La sensibilidad del zoom tambien cambia segun la seleccion activa. El dron completo, una pieza madre y una tuerca pequena no comparten la misma ventana de acercamiento ni el mismo paso de zoom.

Esta adaptacion no depende del aislamiento. Si una pieza esta seleccionada dentro del dron completo, esa seleccion sigue definiendo el rango practico de acercamiento para permitir una inspeccion mas fina sin ocultar el resto del ensamblaje.

El paneo y la orbita tambien reducen su sensibilidad cuando la escala observada es pequena. Esto evita que un fastener aislado o una subpieza diminuta se pierda con un gesto corto de navegacion.

En esta iteracion, el paneo se amortigua mas que la orbita cuando la pieza observada es muy pequena. La intencion es conservar precision de centrado sin volver lenta la lectura angular del fastener.

Si el usuario deja de seleccionar una pieza, la app vuelve a tomar como referencia el contexto general del modelo para el rango de zoom. Con esto se evita que la camara quede atrapada en un limite de alejamiento heredado desde una inspeccion anterior.

El paneo fino sobre piezas muy pequenas fue reducido de nuevo en la iteracion final para privilegiar control de centrado por encima de velocidad de desplazamiento.

## Modo Analyze
El modo `Analyze` agrupa herramientas de lectura estructural. Su navegación usa dos niveles:

    * una grilla principal de tarjetas;
    * un subpanel específico cuando el usuario entra a `Cut` o `Filter`.

### Cut: corte transversal
La herramienta `Cut` activa el subsistema de corte transversal del modelo. Está pensada para explorar componentes internos sin desarmar la geometría.

#### Comportamiento principal

    * Al entrar al panel, el eje `Y` inicia activo por defecto.
    * El usuario puede activar hasta dos ejes simultáneamente.
    * Si activa un tercer eje, el sistema conserva solo los dos más recientes.
    * Si desactiva todos los ejes, el corte queda deshabilitado.
    * `RESET VIEW` también limpia el corte activo.

#### Controles disponibles

    * botones de eje `X`, `Y` y `Z`;
    * deslizador de posición del plano principal;
    * botón de inversión del plano principal;
    * segundo conjunto de controles cuando hay dos ejes activos;
    * botón de combinación cuando existen dos planos simultáneos.

Cuando el segundo plano está disponible, el usuario puede construir cortes diagonales o lecturas compuestas del ensamblaje. Si el modo combinado se activa, el panel secundario se ajusta para reflejar esa lectura conjunta.

### Explode: vista explosionada
La herramienta `Explode` separa el ensamblaje a partir de un deslizador en línea. Su objetivo es revelar relaciones espaciales entre piezas sin abandonar la escena principal.

    * Abrir `Analyze`.
    * Pulsar `Explode`.
    * Ajustar el nivel de separación con el deslizador.
    * Llevar el valor a cero o volver a pulsar la tarjeta para reconstruir el ensamblaje.

El sistema recuerda el último valor distinto de cero. Esto significa que, si el usuario cierra temporalmente la herramienta y la vuelve a abrir, el deslizador puede restaurar el nivel previo de separación.

### Filter: filtros por categoría
La herramienta `Filter` controla la visibilidad por familias funcionales del dron.

\begin{table}[htbp!]
\centering
\caption{Categorías visibles en la build final}
\begin{tabular}{p{5cm}p{7cm}}

**Etiqueta funcional** | **Categoría interna** 

Airframe | `SkeletonAirframe` 

Propulsion | `PropulsionSystem` 

Avionics | `Avionics` 

Sensors | `SensorsComms` 

Power | `PowerDistribution` 

Fasteners | `Fasteners` 

\end{tabular}
\end{table}

Reglas importantes del filtro:

    * por defecto todas las categorías inician activas;
    * un clic conmuta una categoría individual;
    * si el usuario apaga la última categoría activa, el sistema vuelve automáticamente al conjunto por defecto;
    * un doble clic sobre una categoría activa deja visible solo esa familia funcional;
    * si el usuario repite el doble clic sobre la única categoría activa, el sistema restablece el conjunto por defecto.

La categoría `Fasteners` permite localizar y seleccionar tornillería individual. Al hacerlo, la app mantiene la escena ligera en reposo y solo genera detalle temporal para el fastener seleccionado.

## Modo Studio
El modo `Studio` reúne herramientas de representación visual y control de entorno. Es el modo donde el usuario puede cambiar la lectura visual del dron sin modificar la geometría subyacente.

### Modos de visualización visibles
La interfaz final expone tres tarjetas visibles:

    * `X-Ray`
    * `Solid`
    * `Thermal`

El estado base del visor es `Realistic`, aunque ese modo no aparece como tarjeta directa. Si el usuario pulsa nuevamente un modo que ya está activo, la aplicación regresa al modo base.

### Qué hace cada modo

    * **X-Ray:** enfatiza lectura interior y transparencia técnica del ensamblaje.
    * **Solid:** unifica la lectura visual en una presentación controlada por color sólido.
    * **Thermal:** activa la representación térmica híbrida y muestra una leyenda visible en pantalla.

### Presets de entorno
El panel de entorno presenta tres tarjetas: `Studio`, `Time` y `Color`. Cada una funciona como un ciclo de presets, no como un botón único.

\begin{table}[htbp!]
\centering
\caption{Ciclos de presets visibles en Studio}
\begin{tabular}{p{3cm}p{10.8cm}}

**Tarjeta** | **Secuencia de presets** 

`Studio` | `Studio` $\rightarrow$ `Studio Light` $\rightarrow$ `Blueprint` 

`Time` | `Day` $\rightarrow$ `Night` $\rightarrow$ `Sunset` 

`Color` | `White` $\rightarrow$ `Grey` $\rightarrow$ `Black` $\rightarrow$ `Yellow` $\rightarrow$ `Orange` $\rightarrow$ `Green` $\rightarrow$ `Blue` $\rightarrow$ `Purple` $\rightarrow$ `Red` 

\end{tabular}
\end{table}

Consideraciones importantes:

    * la etiqueta visible de cada tarjeta cambia para indicar el preset actual;
    * `Blueprint` puede activarse desde la tarjeta `Studio`, aunque no exista como botón visible de shader independiente;
    * al salir de `Blueprint`, el sistema retorna a la base visual normal del visor.

### Iluminación
El panel también expone tres deslizadores:

    * **Light Rotation:** rota la luz direccional principal;
    * **Light Intensity:** incrementa o disminuye la intensidad de iluminación sobre el modelo;
    * **Background Tone:** aclara u oscurece la atmosfera general del fondo y ajusta la lectura del entorno activo.

Estos deslizadores no solo afectan la luz del modelo, sino también la atmósfera general del preset activo. La interfaz adapta contraste y estilos visuales cuando detecta fondos claros para preservar legibilidad.

## Simulación térmica y lectura visual
La visualización térmica es uno de los modos más importantes del visor final, por lo que conviene usarla con un criterio claro.

### Cómo activarla

    * Ingresar a `Studio`.
    * Pulsar la tarjeta `Thermal`.
    * Confirmar que aparece la leyenda térmica en pantalla.
    * Ajustar, si se desea, el estado operativo del dron desde `Inspect` para observar cambios de comportamiento térmico.

### Cómo interpretar la leyenda
La leyenda visible de la interfaz muestra un rango aproximado de **20\,°C a 100\,°C**. En términos prácticos:

    * los tonos más fríos representan componentes cercanos al ambiente;
    * los tonos intermedios muestran elevación térmica moderada;
    * los tonos más cálidos indican componentes críticos o más exigidos por la carga.

La lectura no equivale a una cámara térmica calibrada ni a una simulación FEA. Se trata de una **visualización térmica heurística basada en componentes**, útil para comunicar zonas calientes, propagación relativa y comportamiento operativo general.

### Relación con potencia y carga
La visualización térmica se alimenta del estado operativo del dron. Por eso:

    * con el dron apagado, las temperaturas tienden a valores cercanos al ambiente;
    * en `IDLE`, ciertos componentes comienzan a elevar su temperatura;
    * en `FLYING` y con carga alta, motores, ESC, batería y electrónica central tienden a mostrar intensidades más altas;
    * elementos estructurales como brazos o placas pueden mostrar difusión del calor sin convertirse necesariamente en fuentes térmicas primarias.

### Qué comunica la vista térmica
La vista térmica permite:

    * localizar rápidamente zonas críticas del sistema;
    * comparar el comportamiento relativo entre familias de componentes;
    * observar cómo cambia la escena al variar la carga del dron;
    * comunicar térmicamente el sistema sin recurrir a una simulación física de alta fidelidad.

### Qué no debe asumirse
El usuario no debe interpretar esta vista como:

    * una termografía medida en hardware real;
    * una predicción precisa de ingeniería térmica;
    * un reemplazo de validación experimental o simulación FEA.

## Recorrido recomendado de uso
Para una evaluación completa del visor, se sugiere el siguiente recorrido:

    * entrar por `EXPLORE`;
    * navegar el modelo y seleccionar piezas representativas;
    * abrir la ficha técnica y revisar *Identification*, *Specifications* y *Assembly*;
    * usar `Inspect` para activar hotspots, aislar piezas y cambiar el estado operativo;
    * usar `Analyze` para aplicar corte, explosión y filtros;
    * pasar a `Studio` para revisar modos de visualización y presets de entorno;
    * finalizar con `Thermal` para interpretar el comportamiento térmico relativo del dron.

## Limitaciones visibles del cierre

    * la herramienta de medición existe en código, pero no está expuesta en la interfaz final;
    * el catálogo de piezas y el panel de configuración no forman parte del flujo visible de usuario;
    * los efectos de audio no se documentan como parte del cierre funcional verificado;
    * la visualización térmica es heurística y no debe presentarse como simulación física exhaustiva;
    * la sustitucion visual por el modelo Blender final no debe cambiar seleccion, aislamiento, filtros, hotspots ni comportamiento de fasteners documentado;
    * el FBX final de Blender y sus texturas deben validarse en Unity antes de tomar capturas o mediciones finales;
    * los resultados finales de usabilidad deben medirse sobre la build congelada definitiva.

## Solución de problemas

### La aplicación no carga

    * verificar la compatibilidad WebGL del navegador;
    * recargar la página;
    * confirmar que la conexión no esté interrumpiendo la descarga inicial de recursos.

### No puedo cerrar un panel

    * usar el botón de cierre de la ficha;
    * presionar `Esc`;
    * usar `RESET VIEW` si persiste una herramienta de análisis activa.

### No veo cambios en la vista térmica

    * confirmar que el modo `Thermal` esté activo;
    * verificar que la leyenda térmica esté visible;
    * encender el dron y ajustar el nivel de carga desde `Inspect`;
    * esperar unos segundos para que la simulación visual actualice el comportamiento térmico.

### El rendimiento disminuye

    * cerrar otras pestañas pesadas del navegador;
    * evitar cambios bruscos de navegación durante la carga inicial;
    * esperar a que el modelo termine de estabilizarse antes de cambiar repetidamente entre herramientas.

## Contacto
**Correo:** awoodcocks@unadvirtual.edu.co

**Repositorio:** \url{https://github.com/delarge95/WebGL-Thesis-Proposal}