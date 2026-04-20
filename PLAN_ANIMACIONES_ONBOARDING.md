# Plan de animaciones del onboarding

## Objetivo

Convertir las cards del onboarding en una secuencia visual clara y ligera, sin usar images, videos ni gifs. La prioridad es reutilizar iconos, estados de UI Toolkit y el modelo 3D existente para comunicar la interaccion con el menor costo de memoria posible.

## Principios de diseno

- No guardar media pesada por card.
- Reusar los iconos procedurales y los assets ya presentes en UI.
- Animar solo cambios de estado: opacidad, escala, posicion, highlight, estado activo y foco.
- Si una escena necesita contexto 3D, usar el mismo modelo con una camara o recorte distinto, no una captura prerender fija.
- Mantener cada secuencia corta, repetible y legible en 2 a 4 segundos.

## Estrategia tecnica

### Capa 1: cards de onboarding como storyboard

Cada card debe tener 3 piezas:

1. Titulo breve.
2. Copy condensado.
3. Secuencia visual ligera.

La secuencia visual se compone de estados. No se renderiza un video; se alternan clases y textos de apoyo para simular la animacion.

### Capa 2: componentes reutilizables

Crear un sistema de "preview slot" dentro del onboarding que soporte:

- Icono principal.
- Piezas secundarias o marcadores.
- Resaltado por color.
- Thumb o indicador animado.
- Texto de fase corta, si la card lo necesita.

### Capa 3: secuencias por pasos

Cada card puede tener una secuencia de 2 a 3 fases. El controlador ya esta preparado para rotar cues; la siguiente etapa es vincular esa rotacion a cambios visuales concretos.

## Propuesta por card

### 1. Navigate

Visual:

- Fase 1: orbit con icono de rotacion.
- Fase 2: zoom con icono de pinch o rueda.
- Fase 3: pan con icono de desplazamiento lateral.
- El reset view debe aparecer solo como icono, sin texto explicito.

Implementacion:

- Usar un icono central y tres microestados de movimiento.
- Animar el icono con rotacion suave, escala y desplazamiento horizontal/vertical.
- El texto cambia por fases: ORBIT, ZOOM, PAN.

### 2. Selection / levels

Visual:

- Seleccion de una pieza madre.
- Segundo click sobre una subpieza para bajar de nivel.
- Click o tap en el fondo para volver un nivel atras.

Implementacion:

- Resaltar la pieza activa con outline o glow.
- Reducir la opacidad de piezas no activas.
- En el segundo estado, mostrar una pieza interna como si emergiera del conjunto.

### 3. Part Info

Visual:

- Una pieza se selecciona.
- Aparece la pestaña Part Info.
- Se abre el panel con los datos correspondientes.

Implementacion:

- Usar un tab lateral animado con slide-in.
- Mostrar el panel en dos pasos: peeking y open.
- La descripcion debe dejar claro que funciona para mother piece, subpiece y hotspot group.

### 4. Inspect

Visual:

- El menu principal se selecciona.
- Se despliegan sus tres submenus: Pins, Isolate, Power.

Implementacion:

- Hacer una animacion de apertura radial o en abanico, usando los iconos existentes.
- El estado activo del menu debe persistir mientras el submenu esta abierto.

### 5. Pins

Visual:

- Los pins se encienden.
- Luego se resalta un hotspot group en amarillo.

Implementacion:

- Encender el icono de pin.
- Poner un pulso amarillo sobre un cluster de piezas.
- Mantener el cluster con un brillo suave, no un efecto fuerte.

### 6. Isolate

Visual:

- El boton Isolate aísla parent piece, subpiece o hotspot group segun la seleccion activa.
- Volver a pulsar Isolate devuelve un nivel de aislamiento.
- Doble click o tap funciona como alternativa al boton y abre el contexto de informacion de la pieza.

Implementacion:

- Animar el resto de la escena con fade out.
- La pieza activa debe quedar centrada y con mayor contraste.
- El retorno al nivel anterior debe verse como restauracion del grupo previo.

### 7. Power

Visual:

- El drone cambia a on.
- Se indica el modo thermal.
- La simulacion de temperatura empieza a evolucionar.

Implementacion:

- Mostrar estados OFF, IDLE y FLYING.
- Cuando el power esta on, la capa thermal recibe un pulso o gradiente animado.
- No usar una animacion real de calor pesada; basta con un cambio de color procedural y un leve ruido.

### 8. Analyze

Visual:

- Se selecciona el menu Analyze.
- Se abren Cut, Explode y Filter.

Implementacion:

- Repetir el patron de apertura usado en Inspect, pero con iconos analiticos.
- Mantener la semantica de herramientas de estudio interno.

### 9. Cut

Visual:

- Primero aparece un plano.
- Luego dos planos.
- Finalmente un plano inclinado.

Implementacion:

- Construir la secuencia con planos simples y cambios de angulo.
- Mostrar los botones de configuracion mientras el tercer estado aparece.
- Usar lineas y separaciones limpias, no una escena compleja.

### 10. Explode

Visual:

- La vista explota desde estado normal a separada.

Implementacion:

- Separar los grupos con una interpolacion suave.
- El movimiento debe ser radial y legible.
- No llevar las piezas demasiado lejos para que la jerarquia siga siendo entendible.

### 11. Filter

Visual:

- Los filtros se desactivan.
- Luego se aisla una categoria concreta.

Implementacion:

- Mostrar filtros como chips o botones con estado activo/inactivo.
- Destacar una categoria con glow o borde.

### 12. Studio

Visual:

- Se selecciona el menu Studio.
- Se abren Render Mode, Environment y Lighting.

Implementacion:

- Usar el mismo patron visual de apertura que Inspect y Analyze.
- Acompanarlo con iconos de render, cielo y luz.

### 13. Render Mode

Visual:

- Estado base: Realistic.
- Se alterna entre X-Ray, Solid y Thermal.
- Para thermal, el drone debe estar encendido desde Inspect.

Implementacion:

- Cambiar el icono principal y el color de fondo de la preview.
- La secuencia debe explicar la dependencia con Power sin meter texto largo.

### 14. Environment

Visual:

- STUDIO: dark, light y blueprint.
- TIME: day, night y sunset.
- COLOR: presets planos.

Implementacion:

- Reusar el mismo modelo con tres estados de cielo o fondo.
- Cada grupo debe verse como un ciclo de presets, no como un panel saturado.

### 15. Lighting controls

Visual:

- Rotation.
- Object intensity.
- Background tone.

Implementacion:

- Mostrar tres sliders o tres marcadores minimalistas.
- El movimiento debe ser sutil y sincronizado con un highlight del area afectada.

## Como generar las animaciones sin media pesada

### Opcion recomendada

Usar animaciones UI ligeras y estados de clase:

- Toggle de clases CSS/USS.
- Transiciones de opacity, scale y translate.
- Cambios de texto por fase.
- Glow o outline sobre el mismo modelo.

### Opcion secundaria para previews 3D

Si se necesita mas contexto, montar una preview interna con el mismo modelo:

- La misma malla del drone.
- Una camara dedicada.
- Cambios de luz y material por estado.
- Sin exportar capturas ni prerender estatico.

Esto conserva memoria porque no duplica assets ni usa frames almacenados.

## Estructura de implementacion

1. Definir una lista de cards con su secuencia de fases.
2. Asociar cada fase a una clase visual y a un cue corto.
3. Crear un componente reutilizable para previews de iconos y modelos.
4. Implementar transiciones entre fases con un temporizador corto.
5. Conectar cada card a su icono procedimental actual.
6. Validar que el flujo funcione igual en mobile y PC.

## Prioridades de implementacion

1. Switch PC/mobile compacto y estable.
2. Navegacion, seleccion y Part Info.
3. Menus Inspect, Analyze y Studio con apertura animada.
4. Cards de acciones individuales con microanimaciones.
5. Preview 3D ligera solo donde aporte valor real.

## Criterio de exito

La solucion es correcta si:

- Ninguna card depende de video, gif o imagen prerenderizada.
- El usuario entiende cada funcion con una sola lectura.
- Las animaciones ayudan a explicar, pero no distraen.
- El costo de memoria sigue bajo y el onboarding carga rapido.
