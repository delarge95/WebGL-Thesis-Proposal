---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
area: desarrollo
tags:
  - tesis
  - ux
  - ui
  - mobile-first
  - sustentacion
---

# INF EST 31 UX UI Mobile First

## Idea central

La UI no es decoracion. En este proyecto, la interfaz es parte del sistema de comprension tecnica. Ayuda al usuario a orientarse, seleccionar, leer informacion, cambiar modos y reducir la necesidad de recordar instrucciones.

La version con mayor respaldo teorico es mobile-first. La version desktop es funcional, pero se conserva como adaptacion del modelo movil.

## Por que mobile-first

Mobile-first obliga a priorizar:

- acciones esenciales;
- jerarquia clara;
- controles tactiles;
- paneles compactos;
- texto justo;
- reduccion de menus profundos.

Esto encaja con una app de inspeccion tecnica porque el usuario necesita mirar el modelo, no pelear con la interfaz.

## Estructura de la UI

### Hero

Puerta de entrada. Debe presentar el caso y permitir pasar al visor sin ruido.

### Bottom bar

Agrupa modos principales. Es persistente y tactil.

### Bottom sheet

Ficha inferior de detalle. Muestra informacion de la pieza seleccionada sin tapar por completo la escena.

### Onboarding

Capa de entrada que ensena gestos, seleccion, ficha de pieza, modos y sliders antes de que el usuario dependa de ensayo y error.

### Modos

- Inspect: lectura de pieza, hotspots, aislamiento y carga.
- Analyze: explode, corte y filtros.
- Studio: shaders, entornos y vista termica.

## Explicaciones complejas

### Jerarquia visual

Jerarquia visual significa que el ojo entiende que mirar primero, segundo y tercero. En la app:

- el modelo es protagonista;
- la barra inferior ofrece modos;
- el bottom sheet aparece cuando hay seleccion;
- los botones secundarios no compiten con la escena.

Regla aplicada:

```text
prioridad_visual = relevancia_de_tarea + estado_contextual - ruido_visual
```

No es una formula psicometrica; es una forma de explicar la decision de diseno. El modelo 3D tiene maxima prioridad porque es el objeto de inspeccion. La UI aparece por contexto: seleccionar una pieza abre detalle; cambiar de modo revela acciones pertinentes.

Ejemplo:

Si el usuario esta en Inspect, no necesita ver todas las opciones de Studio. Mostrar todo al mismo tiempo aumentaria ruido visual y podria elevar workload percibido.

### Onboarding como reduccion de incertidumbre

El onboarding responde a una pregunta de UX: como evitar que el usuario llegue al modelo sin saber que acciones son posibles. En una app 3D, esta friccion inicial es critica porque el usuario puede confundirse entre orbitar, seleccionar, abrir paneles o cambiar modos.

En este proyecto, el onboarding esta hecho por codigo. Esa decision importa porque:

- permite adaptar el mensaje a desktop y mobile sin producir videos separados;
- mantiene el mismo lenguaje visual de la UI real;
- reduce peso frente a GIFs o clips;
- facilita actualizar la ayuda cuando cambian botones, gestos o sliders.

La estructura pedagogica de cada tarjeta es:

```text
actor -> accion -> objetivo -> respuesta del sistema
```

Ejemplo:

El actor puede ser un cursor o un dedo. La accion puede ser `tap`, `drag`, `pinch` o rueda de mouse. El objetivo puede ser una pieza, un boton o un slider. La respuesta puede ser resaltado, apertura de ficha, cambio de modo o movimiento de camara.

### Info panel como centro semantico

La ficha inferior no es solo una tarjeta de datos. Es el mecanismo que responde: "que estoy mirando?".

Su funcion UX es unir tres capas:

- percepcion: el usuario ve una pieza resaltada;
- semantica: la app identifica si es pieza madre, subpieza, grupo de hotspot o fastener;
- accion: desde esa lectura puede aislar, consultar, comparar o cambiar de modo.

Esto evita que la interfaz sea una coleccion de botones. La seleccion crea contexto, y el contexto decide que informacion y que acciones tienen sentido.

Ejemplo:

Si el usuario toca un brazo del dron, la ficha puede mostrar la pieza madre. Si vuelve a profundizar, puede mostrar una subpieza. Si entra desde un hotspot, puede mostrar un grupo funcional. Si selecciona un fastener, puede mostrar familia, metrica, longitud y relacion con la pieza canonica mas cercana.

### Leyes Gestalt

Las leyes Gestalt ayudan a agrupar visualmente:

- proximidad: elementos cercanos parecen relacionados;
- similitud: elementos parecidos parecen del mismo grupo;
- continuidad: el ojo sigue direcciones y flujos;
- figura-fondo: el usuario distingue paneles de escena.

Aplicacion al visor:

- proximidad: botones de un mismo modo se agrupan para indicar relacion funcional;
- similitud: botones con forma, tamano y estilo comun se perciben como parte del mismo sistema;
- figura-fondo: el bottom sheet debe separarse del modelo sin bloquearlo por completo;
- continuidad: el flujo Hero -> Explore -> seleccion -> detalle mantiene secuencia clara.

### Por que desktop es adaptacion

El layout movil se puede usar en escritorio, pero escritorio tiene otras reglas: mayor espacio, cursor, hover, atajos, paneles laterales y densidad informativa distinta. El informe reconoce que no se desarrollo un sistema desktop igual de riguroso.

Como defenderlo:

> La decision mobile-first fue coherente con el alcance y con la necesidad de priorizar interaccion directa. La version desktop conserva funcionalidad y consistencia, pero no se sobredeclara como sistema desktop especializado.

### Ley de Fitts aplicada

La Ley de Fitts explica que el tiempo para alcanzar un objetivo depende de su tamano y distancia. En palabras simples: un boton pequeno y lejano es mas dificil de tocar.

Forma conceptual:

```text
tiempo_movimiento = a + b * log2(distancia / ancho + 1)
```

Variables:

- `distancia`: que tan lejos esta el control desde la posicion actual del dedo o cursor.
- `ancho`: tamano efectivo del boton.
- `a` y `b`: constantes empiricas del contexto.

Aplicacion:

La barra inferior y botones tactiles grandes reducen distancia funcional y aumentan area de toque. Esto es especialmente importante en movil, donde el dedo oculta parte de la pantalla y los errores de toque son mas probables.

## Terminos importantes

- UX: experiencia completa del usuario al usar el sistema.
- UI: interfaz visible con la que el usuario interactua.
- Mobile-first: disenar primero para pantalla movil y tactil.
- Bottom bar: barra inferior de navegacion.
- Bottom sheet: panel inferior deslizable o emergente.
- Onboarding: guia inicial integrada en la app.
- Info panel: vista de informacion contextual de la seleccion.
- Pieza madre: componente principal al que pertenece una seleccion.
- Subpieza: elemento interno o nivel mas especifico de una pieza.
- Grupo de hotspot: seleccion agrupada que nace desde un marcador visual.
- Jerarquia visual: orden de importancia perceptiva.
- Feedback visual: respuesta visible del sistema ante una accion.
- Affordance: pista que sugiere como usar algo.
- Estado activo: indicador de que un modo o boton esta seleccionado.
- Friccion: dificultad o esfuerzo adicional para completar una tarea.

## Preguntas dificiles de defensa

### Por que admitir que desktop no tiene el mismo rigor?

Porque es honesto y metodologicamente mas fuerte. La version desktop funciona, pero el trabajo de diseno sistematico se concentro en mobile-first. Reconocer el limite evita sobrepromesa.

### Como se relaciona UX con carga de trabajo?

Una UI clara puede reducir esfuerzo de orientacion. Si el usuario no tiene que recordar donde esta todo, puede concentrarse mas en entender el ensamblaje.

### Como se prueba que la UI funciona?

Con tareas, observacion, Think-Aloud, SUS para el prototipo y analisis de errores o bloqueos.

### Por que la ficha inferior es tan importante?

Porque es el punto donde la seleccion visual se convierte en comprension tecnica. Sin ficha, el usuario ve geometria resaltada; con ficha, entiende nombre, categoria, datos y nivel de ensamblaje.

### Por que hacer onboarding por codigo?

Porque el onboarding debe vivir con la app. Si se hace como video, se vuelve pesado y puede quedar desactualizado. Dibujarlo con `Painter2D` mantiene ligereza, nitidez y consistencia con la UI real.

## Fuentes relacionadas

- Norman (2013), The Design of Everyday Things.
- Nielsen (1994), heuristicas de usabilidad.
- Ware (2012), percepcion visual.
- Bowman et al. (2004), 3D user interfaces.
- [[MOC_UX_UI_Complete]]
- [[INF_EST_04_Desarrollo_Implementacion]]
