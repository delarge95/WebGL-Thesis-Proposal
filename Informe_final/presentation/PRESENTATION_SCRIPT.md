# Guion maestro de sustentacion - TwinSight X500

Estado: canonico para defensa academica.
Fecha de actualizacion: 2026-06-11.
Duracion objetivo: 28:30 de exposicion real + 1:30 de margen.
Fuente autoritativa: `Informe_final/informe_final.pdf`.
Carpeta de apoyo: `Informe_final/presentation/`.

## 1. Regla de uso

Este documento no es un parrafo para memorizar de corrido. Es una partitura oral: cada slide tiene una tesis, una evidencia visual, un guion base, una transicion y una zona de riesgo. La defensa debe sonar conversacional, tecnica y situada.

La columna vertebral de toda la presentacion es:

```text
problema -> brecha -> propuesta -> decisiones tecnicas -> implementacion -> evidencia -> limites -> contribucion
```

Frase de tesis oral:

> TwinSight X500 no se defiende como un gemelo digital operacional. Se defiende como un visual product twin: una capa web 3D, optimizada y semanticamente organizada, que hace legibles piezas, relaciones y modos de inspeccion de un hardware complejo.

## 2. Convenciones orales

- `[pausa]`: detenerse 1 segundo.
- `[pausa larga]`: detenerse 2 segundos.
- `[mirar jurado]`: levantar la vista y cerrar una idea importante.
- `[senalar]`: marcar una zona especifica del visual.
- `[click]`: revelar siguiente capa o pasar slide.
- `[respirar]`: bajar velocidad antes de un dato numerico o una limitacion.

## 3. Apertura antes de iniciar

Antes de hablar:

1. Respirar, pies estables, mirada al jurado.
2. Verificar que la app, el video local y la deck esten abiertos.
3. Mantener en mente tres mensajes:
   - ver piezas no basta; hay que comprender relaciones;
   - el aporte integra pipeline 3D, WebGL, UI de inspeccion y evaluacion formativa;
   - la evidencia es favorable, pero acotada y descriptiva.

Si hay nervios, no acelerar. La primera pausa comunica control.

## 4. Guion principal slide por slide

### Slide 1 - TwinSight X500 convierte un ensamblaje complejo en una experiencia WebGL inspeccionable

Tiempo: 0:00-0:30.

Objetivo: presentar identidad, caso de estudio y alcance sin sobreprometer.

Visual: hero de la app con el dron completo visible, titulo, autor, programa, universidad, enlace o QR de demo.

Guion oral:

> Buenos dias, miembros del jurado. Mi nombre es Alexander Woodcock Salomon y hoy presento TwinSight X500, un prototipo WebGL de visualizacion 3D interactiva para inspeccion tecnica del dron Holybro X500 V2.
>
> La idea central es sencilla: transformar un ensamblaje complejo, que normalmente se consulta en planos, manuales o archivos CAD pesados, en una experiencia web explorable, seleccionable y explicable. [pausa]
>
> Durante la sustentacion voy a defender tres cosas: el problema tecnico y cognitivo que origina el proyecto, las decisiones de implementacion que hicieron viable la app, y la evidencia metodologica que permite interpretarla con honestidad academica.

Clicks/movimiento: no animar demasiado; dejar que el dron sea la primera senal visual.

Transicion:

> Para entender por que esto importa, primero hay que mirar el problema que existia antes de la app.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, introduccion y planteamiento del problema.
- `README.md`, resumen publico y flujo visible.

No decir:

- "Gemelo digital completo".
- "Simulador operacional".
- "Producto final industrial".

### Slide 2 - El reto central es comprender relaciones, no solo ver piezas

Tiempo: 0:30-1:30.

Objetivo: instalar el problema humano y tecnico sin atacar la documentacion 2D.

Visual: comparacion en tres paneles: manual 2D, CAD pesado, app web 3D.

Guion oral:

> La documentacion tecnica tradicional no es un error. Un manual 2D o un plano cumplen una funcion necesaria: nombran piezas, muestran pasos y organizan informacion. El limite aparece cuando el usuario debe reconstruir mentalmente profundidad, orientacion, piezas ocultas y relaciones de ensamblaje.
>
> En un hardware complejo como el Holybro X500 V2, la dificultad no es solamente saber que existe un motor, una montura o tornilleria. La dificultad es entender donde vive cada elemento, como se relaciona con los demas y que cambia cuando lo aislo, lo exploto o lo observo desde otro modo visual. [senalar comparacion]
>
> Por eso el problema no era "falta de informacion". Era una distancia entre informacion disponible y comprension espacial.

Clicks/movimiento: revelar primero manual/CAD y despues la app.

Transicion:

> Esa distancia genera una brecha tecnica: necesitamos fidelidad, legibilidad y rendimiento al mismo tiempo.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, definicion de hardware complejo.
- `Informe_final/figures/chapter1/fig_1_fragmentacion_hardware_complejo.pdf`.

No decir:

- "El 2D no sirve".
- "El 3D siempre es mejor".

### Slide 3 - La brecha esta entre fidelidad tecnica, legibilidad y ejecucion web

Tiempo: 1:30-2:30.

Objetivo: explicar el trade-off del proyecto.

Visual: triangulo con tres vertices: fidelidad tecnica, legibilidad del ensamblaje, rendimiento web.

Guion oral:

> La brecha del proyecto aparece en este triangulo. Si privilegio solo fidelidad, puedo terminar con un CAD muy detallado, pero imposible de ejecutar fluidamente en navegador. Si privilegio solo rendimiento, puedo tener una escena ligera, pero pobre para inspeccion tecnica. Y si privilegio solo legibilidad visual, puedo perder trazabilidad hacia piezas reales.
>
> TwinSight X500 se construyo como una respuesta de equilibrio: conservar una lectura tecnica suficiente del dron, organizar semanticamente sus partes y hacerlo correr como experiencia WebGL. Ese equilibrio es la razon de muchas decisiones posteriores: optimizacion geometrica, taxonomia, bottom sheet, modos de inspeccion y profiler.

Clicks/movimiento: mostrar cada vertice y luego el centro del triangulo.

Transicion:

> Ese equilibrio exige delimitar el alcance. Aqui hay una frontera importante.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, planteamiento del problema y justificacion.
- `Informe_final/chapters/03_marco_metodologico.tex`, delimitacion como visual product twin.

No decir:

- "Se resolvio toda la cadena industrial".
- "El navegador no tiene limites".

### Slide 4 - La tesis responde con un visual product twin, no con un digital twin operacional

Tiempo: 2:30-4:00.

Objetivo: fijar alcance academico y evitar objeciones por sobrepromesa.

Visual: frontera incluido/excluido.

Guion oral:

> Una parte clave de la defensa es llamar al sistema por su nombre correcto. TwinSight X500 no es un digital twin operacional. No recibe telemetria real, no sincroniza estado con un dron fisico, no ejecuta mantenimiento predictivo, no se integra con PLM, CMMS, SCADA o IoT, y Thermal no es una simulacion fisica calibrada.
>
> Lo que si entrega es una capa visual-semantica: un visual product twin. Organiza el producto en piezas, categorias, datos contextuales, herramientas de inspeccion, modos visuales y medicion tecnica de la build. [pausa]
>
> Dicho de forma directa: esta tesis no promete operar el dron desde datos vivos; promete hacer legible un hardware complejo desde la web, y dejar una base trazable para fases futuras.

Clicks/movimiento: revelar incluido, luego excluido, luego la frase "visual product twin".

Transicion:

> Con esa frontera clara, los objetivos se entienden mejor.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, alcance y limitaciones.
- `Informe_final/chapters/03_marco_metodologico.tex`, frontera metodologica.
- `README.md`, Academic Scope y Capability Status.

No decir:

- "TwinSight ya es un gemelo digital".
- "Thermal calcula temperatura real".
- "La app garantiza compatibilidad universal".

### Slide 5 - Los objetivos conectan pipeline 3D, interaccion, rendimiento y evaluacion formativa

Tiempo: 4:00-5:20.

Objetivo: mostrar que la ruta del proyecto cubre construccion y evaluacion.

Visual: matriz 2x2 con OE1 pipeline, OE2 materiales/modos, OE3 prototipo WebGL, OE4 evaluacion.

Guion oral:

> El objetivo general fue desarrollar un prototipo web 3D interactivo basado en Unity Web, orientado a exploracion tecnica, inspeccion y analisis visual del ensamblaje.
>
> Los cuatro objetivos especificos separan el problema en frentes verificables. Primero, disenar un pipeline de optimizacion de activos CAD hacia WebGL. Segundo, integrar materiales y modos visuales en Unity URP. Tercero, implementar la experiencia web con navegacion, seleccion, ficha contextual y herramientas analiticas. Cuarto, evaluar el prototipo con KPIs tecnicos, tareas, NASA-TLX Raw, Think-Aloud y SUS.
>
> La logica es importante: no se evaluo una idea abstracta. Se evaluo un artefacto construido.

Clicks/movimiento: revelar OE por cuadrantes.

Transicion:

> Por eso la metodologia no podia ser solo desarrollo de software; tenia que incluir evaluacion del artefacto.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, objetivos.
- `Informe_final/chapters/03_marco_metodologico.tex`, tipo de investigacion.

No decir:

- "Todos los objetivos son puramente tecnicos".

### Slide 6 - La metodologia usa desarrollo iterativo y validacion descriptiva

Tiempo: 5:20-6:40.

Objetivo: defender DSR y el caracter formativo/descriptivo.

Visual: ciclo DSR aplicado: problema, objetivos, diseno, demostracion, evaluacion, comunicacion.

Guion oral:

> La investigacion se enmarca como aplicada, con enfoque mixto y predominio cualitativo-formativo. El marco principal es Design Science Research, porque el conocimiento se genera construyendo y evaluando un artefacto que responde a un problema practico.
>
> Esto implica una lectura metodologica especifica. No estoy presentando un experimento poblacional con inferencia estadistica fuerte. Presento una evaluacion formativa y descriptiva de una build funcional: que hace, bajo que condiciones tecnicas corre, como la perciben usuarios con perfil afin y que limites quedan.
>
> En una tesis de pregrado de Ingenieria Multimedia, esa decision es defendible si hay trazabilidad, instrumentos claros y honestidad de alcance.

Clicks/movimiento: construir ciclo por etapas.

Transicion:

> Para sostener esa lectura, la evaluacion se diseno por capas, no con una sola metrica.

Evidencia base:

- `Informe_final/chapters/03_marco_metodologico.tex`, marco DSR y fases.
- `Informe_final/figures/chapter3/fig_3_dsrm_aplicado_proyecto.pdf`.

No decir:

- "Se probo causalidad".
- "La muestra representa a toda la poblacion".

### Slide 7 - La evaluacion combina datos tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud

Tiempo: 6:40-8:00.

Objetivo: explicar triangulacion y que mide cada instrumento.

Visual: diagrama de triangulacion con cinco capas.

Guion oral:

> La evaluacion tiene cinco capas. La primera son KPIs tecnicos: FPS, frame time, memoria, build y profiler. La segunda es desempeno en tareas: completitud, ayudas y tiempos para T1, T2 y T3. La tercera es SUS, aplicado solo al prototipo 3D como lectura global de usabilidad. La cuarta es NASA-TLX Raw, aplicado por condicion 3D y 2D para carga de trabajo percibida. La quinta es Think-Aloud, que explica cualitativamente por que aparecen claridad o friccion.
>
> Dos precisiones evitan malinterpretaciones. SUS no compara 3D contra 2D, porque el soporte 2D no es un sistema interactivo equivalente. NASA-TLX Raw tampoco mide directamente carga intrinseca, extrinseca y germana; mide workload percibido y se interpreta desde ese marco.

Clicks/movimiento: revelar cada instrumento y una nota de interpretacion.

Transicion:

> Con el metodo claro, paso al primer reto de ingenieria: convertir CAD pesado en WebGL usable.

Evidencia base:

- `Informe_final/chapters/03_marco_metodologico.tex`, instrumentos y triangulacion.
- `Informe_final/chapters/05_resultados.tex`, resultados SUS/NASA/Think-Aloud.

No decir:

- "NASA mide carga cognitiva teorica de forma directa".
- "SUS demuestra que 3D es mejor que 2D".

### Slide 8 - El pipeline convierte activos tecnicos pesados en geometria legible para WebGL

Tiempo: 8:00-9:30.

Objetivo: mostrar el aporte de Technical Art.

Visual: CAD/STEP -> MoI3D/STEPper/Blender -> limpieza -> retopologia/proxies -> bake -> FBX -> Unity WebGL.

Guion oral:

> El reto 3D no fue importar un modelo y ponerlo en pantalla. Los modelos CAD de manufactura no estan pensados para render en tiempo real. Pueden traer superficies convertidas con n-gons, vertices repetidos, caras internas, piezas repetidas como mallas unicas y detalle geometrico innecesario para inspeccion.
>
> Por eso el pipeline combina rutas de importacion, saneamiento geometrico, remodelado, optimizacion, bake de mapas y exportacion a Unity. La decision tecnica no fue conservar todo el CAD original, sino traducirlo a un activo runtime legible.
>
> La palabra clave es traduccion: de geometria de manufactura a geometria de inspeccion web.

Clicks/movimiento: revelar pipeline por etapas; no mostrar todo de golpe.

Transicion:

> Esa traduccion produce dos cifras geometricas que conviene leer correctamente.

Evidencia base:

- `Informe_final/chapters/04_desarrollo.tex`, pipeline 3D.
- `Informe_final/figures/screenshots_contextual/fig_cad_bake_high_pair.png`.
- `Informe_final/figures/screenshots_contextual/fig_cad_bake_low_pair.png`.

No decir:

- "El CAD original se uso intacto".
- "La optimizacion fue solo decimacion automatica".

### Slide 9 - La reduccion geometrica se lee como presupuesto de activo, no como conteo runtime

Tiempo: 9:30-10:40.

Objetivo: blindar la lectura 95 617 vs 229 054.

Visual: tarjeta comparativa:
- 95 617 triangulos: activo base optimizado exportado.
- 229 054 triangulos estimados: escena runtime instrumentada/profiler.

Guion oral:

> En el informe aparecen dos cifras que no deben compararse como si fueran el mismo tipo de medida. La cifra de 95 617 triangulos corresponde al activo base optimizado exportado. Es el resultado del cierre geometrico principal.
>
> La cifra de 229 054 triangulos estimados corresponde a la escena runtime instrumentada por el profiler. Esa escena incorpora instancias, proxies, assets de apoyo, renderers activos y elementos necesarios para la interaccion.
>
> Entonces, si el jurado ve esas dos cifras, la lectura correcta es: no hay contradiccion. Son niveles de medicion distintos.

Clicks/movimiento: revelar primero cada cifra y luego "no equivalentes".

Transicion:

> La misma idea aplica a la arquitectura: no es una sola malla; es un sistema de datos, UI, escena y medicion.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, resultados formativos de rendimiento y pipeline de activos.
- `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`.

No decir:

- "229 054 invalida la reduccion".
- "95 617 es todo lo que se renderiza siempre".

### Slide 10 - La arquitectura separa UI, seleccion, datos runtime, shaders y medicion

Tiempo: 10:40-12:10.

Objetivo: demostrar que la app es sistema, no escena suelta.

Visual: arquitectura por capas.

Guion oral:

> La aplicacion se organizo por capas. En la superficie esta la UI: Hero, Explore, bottom sheet y los modos Inspect, Analyze y Studio. Debajo esta la coordinacion de estados, seleccion, visibilidad, exploded view, corte transversal, modos visuales y thermal. Luego esta la capa de datos, con piezas, categorias, fichas y assets. Y finalmente estan la escena runtime, shaders y profiler.
>
> Esta separacion importa porque permite que la seleccion de una pieza no sea solo un highlight visual. La seleccion activa informacion, aislamiento, relaciones de ensamblaje y posibles modos de lectura.
>
> Por eso defiendo la app como un sistema interactivo de inspeccion tecnica, no como un viewer pasivo.

Clicks/movimiento: revelar capas de arriba abajo.

Transicion:

> Para que esa arquitectura se entienda, hay que ver la taxonomia que sostiene la seleccion.

Evidencia base:

- `Informe_final/chapters/04_desarrollo.tex`, normalizacion del sistema y arquitectura.
- `README.md`, Main Runtime Systems.
- `Informe_final/Manual_tecnico/manual_tecnico.pdf`.

No decir:

- "Todos los managers son visibles al usuario".
- "La arquitectura por si sola prueba usabilidad".

### Slide 11 - La taxonomia permite seleccionar piezas madre, subpiezas, hotspots y fasteners

Tiempo: 12:10-13:20.

Objetivo: explicar por que hay varios conteos y niveles de lectura.

Visual: jerarquia 28 piezas canonicas -> 30 anchors -> 257 renderers/colliders.

Guion oral:

> La escena tiene varios niveles. Las 28 piezas canonicas son la taxonomia semantica de investigacion. Los 30 anchors son nodos operativos de Unity: las 28 piezas mas grupos sinteticos para fasteners y miscelaneos. Los 257 renderers y colliders describen la granularidad geometrica de la escena.
>
> Esto evita un error comun: confundir una pieza academica, un nodo de escena y un fragmento renderizable. En la app, esa distincion permite seleccionar una pieza madre, entrar a subpiezas, activar hotspots o inspeccionar fasteners sin que la ficha contextual hable de otro nivel.
>
> La taxonomia es la estructura que vuelve tecnicamente coherente la interaccion.

Clicks/movimiento: construir jerarquia.

Transicion:

> Sobre esa taxonomia se monta el flujo publico real.

Evidencia base:

- `Informe_final/chapters/04_desarrollo.tex`, Tabla de reconciliacion 16/28/30/257.
- `Informe_final/figures/chapter4/fig_4_taxonomia_28_30_257.pdf`.

No decir:

- "Hay 257 piezas canonicas".
- "Los fasteners se resuelven todos con el mismo nivel de detalle".

### Slide 12 - El flujo publico se concentra en Explore, seleccion, bottom sheet, Inspect, Analyze y Studio

Tiempo: 13:20-14:35.

Objetivo: cerrar alcance visible de UI y evitar funciones sobrevendidas.

Visual: flujo Hero -> Explore -> seleccion -> bottom sheet -> Inspect/Analyze/Studio.

Guion oral:

> El flujo publico defendible es este: Hero, Explore, seleccion de pieza, bottom sheet, y tres modos principales: Inspect, Analyze y Studio.
>
> El bottom sheet es importante porque convierte una seleccion visual en lectura tecnica. El usuario no solo ve una malla resaltada; lee nombre, categoria, especificaciones, relacion de ensamblaje y nivel de seleccion.
>
> Tambien es importante decir lo que no se muestra como promesa final: medicion, BOM, guias de ensamblaje avanzadas, telemetria o modulos legacy no son parte del flujo publico evaluado, aunque puedan existir como codigo, prototipo anterior o trabajo futuro.

Clicks/movimiento: recorrer flujo de izquierda a derecha.

Transicion:

> Ahora muestro que hacen los dos modos mas orientados a inspeccion: Inspect y Analyze.

Evidencia base:

- `README.md`, Current Visible Flow y Capability Status.
- `Informe_final/presentation/DEMO_SCRIPT.md`.
- `Informe_final/figures/screenshots_contextual/fig_ui_explore_mobile_pc.png`.

No decir:

- "La UI final incluye BOM, mediciones o annotations".
- "Todo lo que existe en codigo esta publicado como feature".

### Slide 13 - Inspect y Analyze reducen ruido visual para leer relaciones de ensamblaje

Tiempo: 14:35-15:55.

Objetivo: conectar herramientas con comprension espacial.

Visual: GIF o secuencia: seleccionar -> aislar -> exploded view -> cut/filter.

Guion oral:

> Inspect y Analyze no son botones decorativos. Son estrategias para reducir ruido visual.
>
> Inspect permite seleccionar, aislar y concentrarse en una pieza o grupo. Analyze permite separar el ensamblaje con exploded view, cortar la escena con un plano y filtrar por categorias funcionales. En un sistema con muchas piezas, esto cambia la tarea: el usuario deja de buscar visualmente en una masa de geometria y empieza a leer relaciones.
>
> El aporte aqui no es "ver mas bonito"; es controlar que informacion queda visible para entender el ensamblaje.

Clicks/movimiento: usar GIF corto o 4 capturas; si hay live demo, solo anticipar.

Transicion:

> Ademas de aislar y separar piezas, la app cambia la forma de leerlas visualmente.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, estado de cierre funcional.
- `Informe_final/figures/screenshots_contextual/fig_explore_isolate_sequence.png`.
- `Informe_final/figures/screenshots_contextual/fig_analyze_tool_outputs.png`.

No decir:

- "La vista explotada reemplaza un procedimiento de ensamble completo".

### Slide 14 - Studio y los shaders convierten el modelo en lecturas visuales complementarias

Tiempo: 15:55-17:15.

Objetivo: explicar Realistic, X-Ray, Solid, Thermal y presets como lecturas.

Visual: grid de modos reales.

Guion oral:

> Studio agrupa lecturas visuales. Realistic conserva materialidad y forma. X-Ray ayuda a leer superposiciones. Solid Color simplifica la escena por color y volumen. Thermal introduce una lectura relativa por componentes. Los presets de entorno permiten observar el modelo bajo condiciones visuales distintas.
>
> La idea no es multiplicar efectos. Cada modo debe responder a una pregunta de inspeccion: que pieza es, donde esta, que relacion tiene, que zonas resaltan, que queda oculto o que conviene aislar.
>
> En la defensa, los modos ocultos o legacy no se presentan como alcance visible. Se mencionan solo si el jurado pregunta por capacidades internas.

Clicks/movimiento: revelar modo por modo, con etiqueta de uso.

Transicion:

> Hay un modo que requiere una aclaracion especial: Thermal.

Evidencia base:

- `Informe_final/chapters/04_desarrollo.tex`, UX/UI y modos visuales.
- `Informe_final/figures/screenshots_contextual/fig_modes_direct_xray_solid_thermal.png`.
- `Informe_final/figures/screenshots_contextual/fig_modes_studio_presets.png`.

No decir:

- "Hay siete modos publicos si no estan en la UI final".
- "Thermal mide temperatura real".

### Slide 15 - Thermal es una visualizacion heuristica, no una simulacion FEA calibrada

Tiempo: 17:15-18:15.

Objetivo: cerrar riesgo tecnico de simulacion sobrevendida.

Visual: captura Thermal con etiqueta "heuristico".

Guion oral:

> Thermal debe explicarse con precision. En esta tesis, Thermal es una visualizacion heuristica por componentes. Sirve para comunicar tendencias relativas y apoyar lectura tecnica, pero no es FEA, no es telemetria y no reemplaza una medicion fisica calibrada.
>
> Su valor dentro del alcance es pedagogico y analitico: ayuda a que el usuario piense el producto por zonas funcionales y estados relativos. Su evolucion natural seria calibrarlo con datos experimentales o telemetria en una fase futura.

Clicks/movimiento: senalar leyenda y poner sello "no FEA".

Transicion:

> Hasta aqui he mostrado el argumento tecnico. Ahora la demo comprime ese argumento en una secuencia observable.

Evidencia base:

- `Informe_final/chapters/01_introduccion.tex`, alcance y limitaciones.
- `Informe_final/chapters/06_conclusiones.tex`, limitaciones y trabajo futuro.
- `Informe_final/figures/screenshots_contextual/fig_thermal_single.png`.

No decir:

- "Predice fallas termicas".
- "Es simulacion fisica".

### Slide 16 - La demo debe probar tres capacidades, no navegar improvisadamente

Tiempo: 18:15-18:45.

Objetivo: preparar al jurado para observar la demo.

Visual: tres bullets grandes: explorar, inspeccionar, analizar.

Guion oral:

> Antes de mostrar la demo, quiero fijar que deben observar. No voy a navegar de forma improvisada. La secuencia prueba tres capacidades: explorar el dron completo, seleccionar una pieza y convertirla en informacion tecnica, y usar herramientas de inspeccion para entender relaciones.
>
> Si uso video, es un respaldo grabado de la build real. Si ejecuto microdemo en vivo, sera solo una confirmacion corta del mismo flujo.

Clicks/movimiento: no sobreanimar.

Transicion:

> Empecemos con el recorrido.

Evidencia base:

- `Informe_final/presentation/DEMO_SCRIPT.md`.
- `Informe_final/presentation/ASSETS_REQUIREMENTS.md`.

No decir:

- "Voy a probar todo".
- "Voy a explorar un poco".

### Slide 17 - Demo: de dron completo a pieza, relacion y modo visual

Tiempo: 18:45-21:15.

Objetivo: demostrar continuidad de la app.

Visual: video de 75-90 s o demo vivo corta.

Guion oral sobre video:

> Primero vemos el dron completo en Explore. La navegacion permite orbit, zoom y paneo sin salir del navegador. [pausa]
>
> Ahora selecciono una pieza. Fijense que no aparece solo un resaltado: aparece el bottom sheet con informacion tecnica y nivel de lectura. [senalar]
>
> Al pasar a Inspect, puedo aislar la pieza y reducir el contexto visual. En Analyze, la vista explosionada, el corte y los filtros permiten leer relaciones que en una vista plana exigirian reconstruccion mental. [pausa]
>
> Finalmente, Studio cambia la lectura visual: Realistic, X-Ray, Solid y Thermal no son efectos aislados; son formas distintas de mirar el mismo ensamblaje.
>
> Esta secuencia demuestra el punto central: la app no solo muestra el dron, organiza la comprension de sus relaciones.

Si falla la demo:

> Para no gastar tiempo en troubleshooting, paso al recorrido grabado. La evidencia que quiero mostrar es esta misma secuencia: exploracion, seleccion, inspeccion y lectura visual.

Clicks/movimiento: despues del video, congelar en una captura final del dron y no seguir jugando con la app.

Transicion:

> La demo muestra funcionamiento. El siguiente paso es revisar que tan trazable fue su rendimiento tecnico.

Evidencia base:

- `Informe_final/presentation/DEMO_SCRIPT.md`.
- `docs/Build/`.
- `docs/index.html`.

No decir:

- "El video demuestra FPS".
- "Si corre aqui, corre igual en todos los equipos".

### Slide 18 - El profiler interno vuelve trazable el rendimiento por escenario y dispositivo

Tiempo: 21:15-22:25.

Objetivo: explicar medicion tecnica sin ahogar con tablas.

Visual: captura profiler + tabla simplificada.

Guion oral:

> Para evitar que el rendimiento quedara en percepcion subjetiva, la app integra un profiler interno capaz de registrar sesiones por escenario y exportarlas en JSON o CSV.
>
> Cada registro asocia dispositivo, navegador, resolucion, escenario, FPS promedio, frame time, memoria, modo visual, factor de explosion, corte, renderers, mallas y triangulos estimados. Es decir, no se reporta un numero suelto: se reporta un numero en contexto.
>
> Esto es importante porque en WebGL el rendimiento depende de hardware, navegador, memoria, cache y escenario activo.

Clicks/movimiento: resaltar columnas de contexto antes de FPS.

Transicion:

> Con esa lectura contextual, el resultado de rendimiento es favorable, pero no universal.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, rendimiento.
- `Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`.
- `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`.
- `Telemetria/Mediciones_WebGL/`.

No decir:

- "El promedio global resume toda la experiencia".
- "El profiler sustituye prueba en dispositivos".

### Slide 19 - El rendimiento es viable, pero no universal en todo movil

Tiempo: 22:25-23:35.

Objetivo: interpretar resultados tecnicos con honestidad.

Visual: barras por dispositivo con linea de 30 FPS.

Guion oral:

> La lectura tecnica es esta: en escritorio la build fue estable; en equipos moviles de gama alta o media-alta se mantuvo cercana o superior a la meta; en el limite inferior Android fue navegable, pero estuvo por debajo de 30 FPS.
>
> El valor academico no esta en prometer compatibilidad universal. Esta en mostrar que el prototipo es viable bajo condiciones documentadas y que sus limites fueron medidos.
>
> Por eso la tesis no dice "funciona en cualquier movil". Dice: funciona en los dispositivos y navegadores evaluados, con restricciones claras en gama baja o media-baja.

Clicks/movimiento: mostrar primero 30 FPS como referencia, luego cada caso.

Transicion:

> Despues de la capa tecnica, el segundo bloque de resultados es la evaluacion con usuarios.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, rendimiento y compatibilidad.
- `Informe_final/figures/screenshots_contextual/fig_device_matrix_clean.png`.

No decir:

- "Mobile ready universal".
- "30 FPS garantizados".

### Slide 20 - La validacion de usuarios muestra recepcion favorable del prototipo 3D

Tiempo: 23:35-24:45.

Objetivo: explicar SUS correctamente.

Visual: grafico SUS, referencia 68 y distribucion.

Guion oral:

> La evaluacion con usuarios incluyo 12 participantes anonimizados, con perfiles afines a ingenieria, software, diseno de interaccion o visualizacion. El SUS se aplico solo al prototipo 3D.
>
> El promedio SUS fue 91,88, con mediana 95, minimo 60, maximo 100 y desviacion estandar 11,24. La referencia de 68 se usa como promedio historico del instrumento, no como umbral absoluto.
>
> La lectura correcta es descriptiva: en esta muestra, el prototipo 3D tuvo una percepcion de usabilidad muy favorable.

Clicks/movimiento: destacar "solo 3D" y "descriptivo".

Transicion:

> Para comparar condiciones, el instrumento clave no fue SUS sino NASA-TLX Raw.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, resultados SUS.
- `Informe_final/figures/chapter5/fig_5_sus_distribucion.pdf`.

No decir:

- "SUS prueba superioridad frente a 2D".
- "68 es aprobado/reprobado".

### Slide 21 - La condicion 3D redujo carga percibida frente al soporte 2D en la muestra

Tiempo: 24:45-26:05.

Objetivo: explicar NASA-TLX Raw, tiempos T1-T3 y convergencia.

Visual: comparativa NASA 3D vs 2D + tiempos T1-T3.

Guion oral:

> NASA-TLX Raw si se aplico por condicion. El promedio del visor 3D fue 8,69 y el del soporte 2D fue 19,89. La diferencia pareada media fue 11,19 puntos a favor del visor, y en los 12 casos la carga percibida fue menor en 3D.
>
> Esto converge con los tiempos de tareas cronometradas. En T1, T2 y T3, el tiempo total medio fue 20,58 segundos en 3D y 54,00 segundos en 2D. La T4 no se cronometro porque era exploratoria guiada.
>
> La lectura no es causal-inferencial. La lectura defendible es: dentro de esta muestra formativa, el visor 3D mostro menor workload percibido y menor tiempo descriptivo en tareas cronometradas.

Clicks/movimiento: mostrar NASA, luego tiempos, luego nota "T4 exploratoria".

Transicion:

> Ahora bien, si todas las tareas se completaron en ambas condiciones, la discusion debe ser cuidadosa.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, tablas de desempeno, NASA-TLX y discusion.
- `Informe_final/figures/chapter5/fig_5_nasatlx_dimensiones.pdf`.

No decir:

- "Se comprobo estadisticamente".
- "La T4 fue mas rapida".
- "NASA mide aprendizaje".

### Slide 22 - La discusion acota el resultado: efecto techo, muestra pequena y compatibilidad limitada

Tiempo: 26:05-27:45.

Objetivo: demostrar criterio de interpretacion.

Visual: matriz "demuestra / no demuestra".

Guion oral:

> La discusion es donde se evita vender de mas. Las cuatro tareas se completaron en ambas condiciones, entonces hay efecto techo en completitud. El hallazgo fuerte no es "solo el 3D permite completar", porque eso no seria cierto. El hallazgo fuerte es que el 3D redujo carga de trabajo percibida y tiempos descriptivos en T1 a T3.
>
> Tambien hay limites: muestra no probabilistica de 12 participantes, validacion formativa, no inferencia poblacional, T4 exploratoria no cronometrada, escritorio como adaptacion funcional y compatibilidad movil acotada a dispositivos probados.
>
> Estos limites no invalidan el trabajo. Definen exactamente que demuestra: viabilidad tecnica, usabilidad percibida favorable y apoyo descriptivo a la inspeccion espacial dentro del alcance evaluado.

Clicks/movimiento: presentar matriz en dos columnas.

Transicion:

> Con esos limites, la contribucion queda mas clara y mas defendible.

Evidencia base:

- `Informe_final/chapters/05_resultados.tex`, discusion.
- `Informe_final/chapters/06_conclusiones.tex`, limitaciones.

No decir:

- "No hay limitaciones importantes".
- "Los resultados generalizan".

### Slide 23 - La contribucion es tecnica, metodologica y comunicativa

Tiempo: 27:45-28:45.

Objetivo: sintetizar aporte.

Visual: tres columnas: tecnica, metodologica, comunicativa.

Guion oral:

> La contribucion se puede leer en tres niveles.
>
> Primero, tecnica: un pipeline CAD/Blender/Unity/WebGL, optimizacion geometrica, taxonomia de escena, UI de inspeccion, shaders y profiler.
>
> Segundo, metodologica: un caso aplicado con DSR, instrumentos diferenciados, datos tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud, interpretados de forma descriptiva.
>
> Tercero, comunicativa: el proyecto convierte un ensamblaje complejo en una experiencia explorable que ayuda a leer relaciones entre piezas, no solo a mirar un modelo.
>
> Para Ingenieria Multimedia, esa integracion es el punto: computacion grafica, interaccion, experiencia de usuario, documentacion tecnica y evaluacion.

Clicks/movimiento: revelar cada columna.

Transicion:

> Cierro volviendo al problema inicial.

Evidencia base:

- `Informe_final/chapters/06_conclusiones.tex`, conclusiones y contribuciones.
- `README.md`, Project Summary.

No decir:

- "La contribucion es una app bonita".

### Slide 24 - Hacer legible el hardware complejo desde la web es el aporte defendible

Tiempo: 28:45-29:30 maximo. Si se va en tiempo, cerrar en 45 segundos.

Objetivo: cerrar con una frase clara y preparar preguntas.

Visual: dron final, tres takeaways.

Guion oral:

> El punto de partida fue una dificultad concreta: la documentacion tecnica y el CAD pesado no siempre permiten comprender rapidamente relaciones espaciales de un hardware complejo.
>
> TwinSight X500 responde con una capa web 3D inspeccionable: no un gemelo digital operacional, sino un visual product twin que organiza piezas, datos, modos visuales y medicion tecnica.
>
> La evidencia muestra una build funcional, rendimiento viable con limites, usabilidad percibida favorable, menor workload en la muestra y hallazgos cualitativos coherentes con comprension espacial.
>
> La contribucion final no es mostrar mas piezas. Es hacer legibles sus relaciones. Muchas gracias. Quedo atento a sus preguntas.

Clicks/movimiento: no animar el cierre despues de "Muchas gracias".

Evidencia base:

- `Informe_final/chapters/06_conclusiones.tex`.
- `Informe_final/informe_final.pdf`.

No decir:

- "Eso seria todo" como cierre principal.
- "Listo, ya esta perfecto".

## 5. Ruta de emergencia por tiempo

Si faltan 5 minutos y aun no se llego a resultados:

1. Saltar detalles de Slide 10 y Slide 11.
2. Hacer demo en video, sin microdemo viva.
3. Fusionar Slides 18 y 19 en una sola explicacion de rendimiento.
4. Mantener siempre Slides 20, 21, 22 y 24.

Frase de corte elegante:

> Para cuidar el tiempo, dejo el detalle tecnico en los slides de respaldo y paso al resultado que interpreta esa implementacion.

## 6. Ruta si el jurado interrumpe durante la exposicion

Si la pregunta es de aclaracion breve:

> Si le parece, lo respondo en una frase y continuo con el hilo principal.

Si la pregunta abre un debate largo:

> Esa pregunta es importante y tengo un slide de respaldo para ella. La retomo con detalle al final para no romper la secuencia de resultados.

Si piden una cifra:

> La cifra que puedo afirmar es la del informe final: [dato]. Si no esta en el informe, en anexos o en el profiler, prefiero no improvisarla.

## 7. Frases ancla seguras

- "No presento un gemelo digital operacional; presento un visual product twin."
- "La evidencia es favorable, pero descriptiva y formativa."
- "SUS se aplica solo al prototipo 3D; NASA-TLX Raw compara workload percibido por condicion."
- "T4 no se cronometro porque era una tarea exploratoria guiada."
- "95 617 y 229 054 triangulos no son metricas equivalentes."
- "Thermal es heuristico, no FEA ni medicion fisica calibrada."
- "La compatibilidad movil esta documentada por dispositivo, no garantizada universalmente."
- "La contribucion no es mostrar mas piezas, sino hacer legibles sus relaciones."

## 8. Slides de respaldo a preparar

- B1. Formula SUS y referencia historica de 68.
- B2. Formula NASA-TLX Raw y orientacion invertida de rendimiento.
- B3. Variables de control: dispositivo, navegador, resolucion, build, cache, orden AB/BA.
- B4. Tabla tecnica completa de rendimiento.
- B5. Profiler interno y export JSON/CSV.
- B6. 95 617 vs 229 054 triangulos.
- B7. Arquitectura ampliada.
- B8. Thermal heuristico.
- B9. Limitaciones metodologicas.
- B10. Trabajo futuro por fases.
