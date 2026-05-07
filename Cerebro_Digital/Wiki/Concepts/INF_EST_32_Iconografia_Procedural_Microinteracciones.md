---
tipo: modulo_estudio
fuente: desarrollo/docs/investigacion/11_plan_iconos_ui.md
estado: activo
area: desarrollo
tags:
  - tesis
  - iconos
  - microinteracciones
  - ui-toolkit
  - painter2d
---

# INF EST 32 Iconografia Procedural y Microinteracciones

## Idea central

El proyecto no uso iconos como imagenes decorativas. Desarrollo iconos procedurales en codigo, dibujados con UI Toolkit y `Painter2D`, con microinteracciones controladas por fisica ligera.

Esto convierte la iconografia en parte del sistema tecnico: los botones pueden responder, animarse y mantenerse nitidos sin depender de sprites pesados.

La misma logica tambien sostiene el onboarding procedural: no se dibujan solo iconos, tambien pequenas demos de interaccion que explican gestos, botones y respuestas del sistema.

## Proceso de decision

La planeacion inicial considero SVG por su nitidez y bajo peso. Luego el proyecto evoluciono hacia iconos procedurales C#.

La razon:

- cero peso de assets graficos;
- nitidez vectorial;
- animaciones parametrizadas;
- estados consistentes;
- control total dentro del runtime.

## Como se dibuja un icono procedural

1. Se crea una clase que hereda de `VisualElement`.
2. Se usa `MeshGenerationContext` para acceder al dibujo.
3. Se dibujan lineas, arcos o formas con `Painter2D`.
4. Variables animadas alteran escala, posicion, rotacion o separacion.
5. El icono se repinta cuando cambia su estado.

## Onboarding procedural

El onboarding usa la misma familia tecnica que los iconos: UI Toolkit, `Painter2D` y dibujo runtime. La diferencia es que aqui el dibujo no representa solo un boton, sino una mini escena pedagogica.

Cada tarjeta se puede entender asi:

```text
actor -> gesto -> objetivo -> feedback
```

Donde:

- `actor` puede ser cursor, dedo, rueda de mouse o indicador tactil;
- `gesto` puede ser clic, tap, drag, pinch o scroll;
- `objetivo` puede ser una pieza, un slider, un boton o el viewport;
- `feedback` puede ser highlight, apertura de ficha, ripple, cambio de modo o movimiento de camara.

Esto es importante para la defensa porque muestra que la ayuda inicial no fue un recurso externo. Fue desarrollada como parte del sistema tecnico de UI, con la misma logica de nitidez, bajo peso y coherencia visual.

## Formula base: resorte amortiguado

La microinteraccion usa un sistema tipo masa-resorte:

```text
F = -k(x - x_target) - c v
```

Donde:

- `F` es la fuerza aplicada;
- `k` es la rigidez del resorte;
- `x` es el valor actual;
- `x_target` es el valor objetivo;
- `c` es amortiguacion;
- `v` es velocidad.

En palabras simples: el valor no cambia de golpe. Se mueve hacia su objetivo con inercia y frenado. Por eso el boton se siente vivo y no robotico.

Como afecta cada variable:

- si `k` aumenta, el icono llega mas rapido al objetivo, pero puede sentirse brusco;
- si `c` aumenta, el movimiento se frena mas rapido y pierde rebote;
- si `v` es alta, el icono tiene inercia visible;
- si `x_target` cambia por hover o click, el icono reacciona sin saltar instantaneamente.

Ejemplo del proyecto:

En un boton de `Explode`, `x_target` puede representar separacion entre capas. Al activar el modo, las capas no saltan a su posicion final; se desplazan con una transicion amortiguada que comunica despiece.

## Ejemplos de iconos

### Home

Se dibuja como casa minimalista. En hover crece; en click se comprime y vuelve.

### Reset

Usa flecha circular. La microinteraccion puede rotar para comunicar retorno al origen.

### Info

El punto de la "i" puede rebotar con squash and stretch para sugerir informacion activa.

### Cut

Usa una herramienta o linea diagonal. La animacion comunica seccion o tajo.

### Explode

Usa capas isometricas. Al interactuar, las capas se separan como un despiece.

### Shaders

Puede usar barrido visual para comunicar cambio de modo de render.

## Explicaciones complejas

### Squash and stretch

Principio de animacion donde un objeto se aplasta y estira para parecer fisico. Si sube rapido, se alarga; si impacta, se aplasta. El volumen visual se conserva compensando ancho y alto.

### Matriz de rotacion 2D

Para rotar puntos se usa:

```text
x' = x cos(theta) - y sin(theta)
y' = x sin(theta) + y cos(theta)
```

Esto permite dibujar una forma base y girarla matematicamente sin crear otra imagen.

Variables:

- `x`, `y`: punto original del icono.
- `x'`, `y'`: punto transformado despues de rotar.
- `theta`: angulo de rotacion.
- `cos(theta)` y `sin(theta)`: componentes que redistribuyen el punto en los ejes.

Ejemplo:

El icono Reset puede usar una flecha circular. En vez de guardar varios sprites, el codigo rota puntos o grupos de puntos para comunicar retorno al estado inicial.

### Paralaje 2.5D

El icono Explode simula profundidad con capas desplazadas en Y. No hay 3D real; se usa una ilusion visual para representar separacion.

Forma conceptual:

```text
posicion_capa = posicion_base + direccion * separacion * indice_capa
```

Variables:

- `posicion_base`: punto inicial del grupo.
- `direccion`: eje o vector hacia donde se separan las capas.
- `separacion`: intensidad de la microinteraccion.
- `indice_capa`: orden de cada capa.

Aplicacion:

El icono prepara mentalmente al usuario para el comportamiento del modo Explode. La microinteraccion del boton anticipa la accion que ocurrira en el modelo.

### Por que esto reduce friccion

La microinteraccion cumple tres funciones:

- confirmacion: el sistema recibio la accion;
- anticipacion: el usuario entiende que tipo de cambio ocurrira;
- continuidad: el paso entre estados se siente conectado, no abrupto.

En defensa:

> La iconografia procedural no fue un lujo visual. Fue una solucion de Technical Art que unio optimizacion, consistencia grafica y feedback interactivo.

### Por que el onboarding tambien es Technical Art

Porque traduce una accion abstracta en una escena visual entendible. No basta con escribir "arrastra para orbitar"; el usuario ve el gesto, el objetivo y la respuesta. Esa explicacion visual reduce dependencia de memoria y hace mas clara la primera experiencia.

## Terminos importantes

- Icono procedural: icono dibujado por codigo.
- Painter2D: API de dibujo vectorial en UI Toolkit.
- MeshGenerationContext: contexto usado para generar geometria visual.
- VisualElement: unidad basica de UI Toolkit.
- Hover: estado cuando el cursor pasa sobre un elemento.
- Press/click: estado de pulsacion.
- Spring: resorte matematico.
- Damping: amortiguacion del movimiento.
- Squash and stretch: aplastamiento y estiramiento visual.
- Lerp: interpolacion entre dos valores.
- Path: trayecto vectorial formado por lineas o curvas.
- Onboarding procedural: ayuda inicial dibujada por codigo.
- Actor: cursor, dedo o elemento que ejecuta la accion en una demo.
- Feedback: respuesta visible del sistema ante una accion.

## Preguntas dificiles de defensa

### Por que dibujar iconos en codigo?

Porque reduce peso, mantiene nitidez, permite animaciones parametrizadas y evita depender de sprites o videos en WebGL.

### Esto aporta academicamente?

Si, porque integra UI, programacion grafica, optimizacion y experiencia de usuario. No es adorno; es una decision de Technical Art.

### No era mas facil usar SVG?

Si, pero el enfoque procedural dio mas control sobre microinteracciones y redujo dependencia de assets externos.

### Por que no usar videos o GIFs para el onboarding?

Porque aumentan peso, duplican mantenimiento y pueden quedar desalineados con la interfaz real. El onboarding procedural se actualiza dentro del mismo sistema de UI.

## Fuentes y documentos relacionados

- `desarrollo/docs/investigacion/11_plan_iconos_ui.md`
- `desarrollo/docs/investigacion/12_plan_iconos_procedurales_cs.md`
- `desarrollo/docs/investigacion/13_matematicas_iconos_procedurales.md`
- [[Sistema_Iconos_Procedurales_UI]]
- [[INF_EST_31_UX_UI_Mobile_First]]
