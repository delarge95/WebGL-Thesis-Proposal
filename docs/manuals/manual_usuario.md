---
tipo: entregable
area: manuales
estado: activo
trace_id: TRC-MAN-USR-0001
entregable_ids: ["MAN-USR-FLUJOS", "MAN-USR-VISTAS", "MAN-USR-PANELES"]
script_ids: ["SCR-UI-001", "SCR-UI-002", "SCR-ONB-001", "SCR-ONB-002"]
bib_keys: ["mayer2005cambridge", "paivio1986mental", "nielsen1994usability"]
resumen: "Manual de usuario alineado con la build final visible del Holybro X500 V2."
---

# Manual de Usuario - Holybro X500 V2 WebGL Viewer

## Alcance

Este manual resume el flujo visible y verificable de la build final del visor tecnico del Holybro X500 V2. Documenta solo lo que el usuario realmente puede usar en la interfaz final.

## Flujo principal

```text
Hero -> Explore -> seleccion -> bottom sheet -> Inspect / Analyze / Studio
```

## Acceso y primer uso

1. Abrir la URL o build local del visor.
2. Esperar la carga inicial.
3. Revisar la pantalla Hero.
4. Pulsar `EXPLORE` para entrar al visor.

La pantalla Hero tambien permite:

- abrir el repositorio fuente;
- volver a lanzar la ayuda inicial;
- activar el onboarding desde `HELP`.

## Onboarding

El visor incluye onboarding integrado dentro de la propia UI. No es un video externo. Es una secuencia animada que explica:

- navegacion en desktop y mobile;
- seleccion y deseleccion por niveles;
- apertura de `Part Info`;
- uso de `Inspect`, `Analyze` y `Studio`;
- funcionamiento de sliders, filtros y modos visuales.

Si el usuario necesita repasarlo, puede volver a abrirlo desde `HELP`.

## Controles de camara

### Desktop

- `Orbit`: clic derecho + arrastrar
- `Pan`: clic central + arrastrar
- `Zoom`: rueda del mouse
- `Reset View`: boton `RESET VIEW`
- `Home`: boton `HOME`

### Mobile

- `Orbit`: un dedo + arrastrar
- `Pan`: dos dedos + arrastrar
- `Zoom`: gesto de pinza

### Comportamiento general

- `Esc` cierra primero la ficha inferior si esta abierta.
- Si la ficha no esta abierta, `Esc` cierra el subpanel activo o desactiva el modo actual.

## Seleccion

### Seleccion basica

1. Hacer clic o tap sobre una pieza visible.
2. El sistema resalta la seleccion.
3. La UI actualiza el contexto superior e inferior.
4. Si la ficha esta cerrada, aparece una barra de previsualizacion para abrir la informacion completa.

### Jerarquia de seleccion

La seleccion no siempre se queda en un solo nivel. El sistema distingue:

- `pieza madre`
- `subpieza`
- `grupo de hotspot`

En la practica:

- el primer clic suele seleccionar la pieza madre;
- un clic adicional sobre geometria interna puede bajar a subpieza;
- una seleccion iniciada desde un pin puede representar un grupo funcional.

Para retroceder:

- un clic sobre el fondo puede regresar un nivel;
- un doble clic sobre el fondo limpia o revierte el aislamiento activo.

## Part Info y bottom sheet

La informacion oficial del componente vive en la ficha inferior (`bottom sheet`).

Se puede abrir:

- desde la pestaña o boton contextual `INFO`;
- desde la barra de previsualizacion inferior;
- mediante gesto ascendente cuando ya existe una seleccion.

La ficha organiza la informacion en:

- `Identification`
- `Specifications`
- `Assembly`

### Casos especiales

- Si la seleccion es un `fastener`, la ficha muestra familia, metrica, longitud, material, referencia CAD y pieza canonica cercana.
- Si la seleccion es una `pieza madre`, la ficha puede mostrar subpiezas documentadas y resumen de fasteners asociados.
- Si la seleccion viene de un `hotspot`, la ficha puede resumir un grupo funcional y no solo una pieza individual.

## Inspect

`Inspect` agrupa herramientas de lectura puntual:

- `Pins`
- `Isolate`
- `Power`
- `Load`

### Pins

- Activa o desactiva hotspots.
- Los hotspots ayudan a ubicar zonas relevantes rapidamente.
- Se pueden ocultar para dejar el modelo mas limpio.

### Isolate

`Isolate` oculta temporalmente el resto del modelo y mantiene visible el nivel activo.

Puede aplicarse a:

- pieza madre
- subpieza
- grupo de hotspot
- fastener individual

Comportamientos importantes:

- si se aisla un fastener, se conserva visible la instancia completa de esa fijacion; es decir, el tornillo o elemento de union completo y no solo una submalla o fragmento de su geometria;
- si se aisla una pieza madre, pueden mantenerse visibles sus fasteners reconciliados;
- el aislamiento puede revertirse con el boton `Isolate` o con doble clic sobre el fondo.

### Power y Load

- `Power` enciende o apaga el estado operativo del dron.
- `Load` ajusta la carga del sistema.

Estados visibles:

- `OFF`
- `STARTING`
- `IDLE`
- `FLYING`
- `SHUTDOWN`

La carga modifica:

- estado operativo;
- lectura de vuelo;
- comportamiento visual del modo termico.

## Analyze

`Analyze` agrupa lectura estructural:

- `Cut`
- `Explode`
- `Filter`

### Cut

- inicia con eje `Y` activo;
- permite hasta dos ejes simultaneos;
- si se activa un tercer eje, el sistema conserva los dos mas recientes;
- `RESET VIEW` limpia el corte.

Controles visibles:

- ejes `X`, `Y`, `Z`
- slider del plano principal
- inversion del plano
- controles secundarios cuando hay dos planos

### Explode

- separa el ensamblaje con un slider;
- revela relaciones espaciales entre piezas;
- puede restaurarse llevando el valor a cero o desactivando la herramienta.

### Filter

Categorias visibles:

- `Airframe`
- `Propulsion`
- `Avionics`
- `Sensors`
- `Power`
- `Fasteners`

Reglas:

- todas inician activas;
- clic simple conmuta una categoria;
- si el usuario apaga la ultima categoria, el sistema vuelve al estado por defecto;
- doble clic deja visible solo una categoria;
- repetir el doble clic sobre la unica categoria activa restaura el conjunto completo.

## Studio

`Studio` agrupa visualizacion, entorno e iluminacion.

### Modos visibles

- `X-Ray`
- `Solid`
- `Thermal`

`Realistic` opera como modo base aunque no aparezca como tarjeta independiente.

### Presets de entorno

El panel de entorno tiene tres ciclos:

- `Studio -> Studio Light -> Blueprint`
- `Day -> Night -> Sunset`
- `White -> Grey -> Black -> Yellow -> Orange -> Green -> Blue -> Purple -> Red`

### Lighting Controls

La UI expone tres sliders:

- `Light Rotation`
- `Light Intensity`
- `Background Tone`

Esto permite cambiar:

- direccion de la luz;
- intensidad sobre el modelo;
- tono general del fondo y del ambiente.

## Modo Thermal

Para activarlo:

1. Entrar a `Studio`.
2. Pulsar `Thermal`.
3. Verificar que aparezca la leyenda termica.

El modo termico debe interpretarse como visualizacion tecnica hibrida, no como FEA ni como medicion fisica calibrada.

## Comportamiento de camara en piezas pequenas

Cuando el usuario selecciona o aisla una pieza:

- la camara recentra segun el tamano real de la seleccion;
- el zoom se adapta al contexto;
- el paneo se amortigua mas en piezas pequenas;
- la orbita conserva suficiente respuesta para inspeccion cercana.

Esto mejora especialmente la lectura de:

- subpiezas pequenas
- fasteners
- componentes internos aislados

## Limites de esta version

Este manual no presenta como flujo final:

- catalogo visible;
- settings visibles;
- measurement como herramienta publicada;
- mallas Blender finales de fasteners si la escena actual todavia usa placeholders.

La sustitucion visual por el modelo Blender final no cambia el flujo del usuario: seleccion, isolate, filtros, hotspots y fasteners deben conservar el mismo comportamiento documentado.
