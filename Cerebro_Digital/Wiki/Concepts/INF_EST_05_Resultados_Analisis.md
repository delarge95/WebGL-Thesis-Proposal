---
tipo: modulo_estudio
fuente: Informe_final/chapters/05_resultados.tex
estado: activo
capitulo: 5
area: informe-final
tags:
  - tesis
  - resultados
  - placeholders
  - validacion
---

# INF EST 05 Resultados y Analisis

## Idea central

Este modulo debe estudiarse como una version casi final del capitulo de resultados. Para practicar la sustentacion, la lectura debe anticipar el cierre esperable del proyecto: un prototipo Web 3D funcional, optimizado y evaluado con KPIs tecnicos, SUS, NASA-TLX Raw y Think-Aloud.

La regla interna es estricta: donde todavia falte el dato real post-freeze, se usa un marcador de reemplazo como `[REEMPLAZAR CON DATO REAL POST-FREEZE]`. En la sustentacion final ese marcador no debe aparecer; debe sustituirse por el valor medido, la fecha, la build y el entorno de prueba.

## Datos que deben reemplazarse antes de entrega final

El capitulo queda escrito con tono final, pero exige sustituir marcadores internos por:

- build web final congelada;
- auditorias de jerarquia y cobertura sobre esa build;
- comparacion pre/post optimizacion;
- KPIs tecnicos medidos;
- validacion de onboarding;
- aplicacion de SUS;
- NASA-TLX Raw por condicion;
- Think-Aloud;
- tablas y figuras con datos fechados y trazables.

## Como leer resultados provisionales sin sobreprometer

Durante el ensayo puede decirse:

> La estructura de resultados ya esta definida y el texto esta preparado para la version final. Los valores marcados internamente como reemplazables se actualizaran con la build congelada, sin cambiar el marco metodologico ni los criterios de interpretacion.

No conviene decir:

> El capitulo esta abierto y no hay resultados.

Esa frase debilita la sustentacion porque comunica ausencia de cierre. La version correcta es que el capitulo tiene arquitectura analitica final y datos pendientes de sustitucion controlada.

## Resultados del pipeline de activos

Esta seccion debe mostrar como cambio el modelo despues del saneamiento:

- peso inicial vs peso optimizado;
- geometria inicial vs geometria final;
- familias criticas;
- efecto del sistema modular de fasteners;
- impacto de texturas y materiales.

Lectura final probable:

- el pipeline CAD -> limpieza -> retopologia selectiva -> assets Unity redujo la friccion de despliegue web;
- el sistema modular de tornillos evito duplicar geometria pequena de alta repeticion;
- la normalizacion 28/30/257 permitio separar taxonomia academica, anchors de escena y fragmentacion tecnica de render;
- la viabilidad web se sostiene si el peso, la memoria y el frame time quedan dentro del presupuesto medido.

Pregunta clave de defensa: que evidencia demuestra que el pipeline mejoro la viabilidad web?

Respuesta esperada: no una opinion, sino una tabla pre/post con `[PESO_INICIAL]`, `[PESO_FINAL]`, `[RENDERERS_INICIALES]`, `[RENDERERS_FINALES]`, `[FRAME_TIME_PRE]`, `[FRAME_TIME_POST]` y observaciones de calidad visual.

## Resultados tecnicos

Aqui deben entrar:

- FPS;
- frame time;
- draw calls;
- memoria;
- tiempo de carga;
- presupuesto geometrico;
- factor de forma;
- navegador;
- dispositivo;
- estado de cache.

Lectura final probable:

- si el frame time promedio es menor o igual a 33.33 ms, la app cumple la meta de 30 FPS promedio;
- si el percentil 95 supera mucho esa meta, debe explicarse que hay picos perceptibles aunque el promedio sea aceptable;
- si la primera carga es alta pero la segunda carga mejora por cache, se deben reportar ambos casos;
- si movil y escritorio difieren, la conclusion debe separar compatibilidad esperada de rendimiento observado.

Ejemplo defendible:

> En la build `[BUILD_ID]`, bajo `[NAVEGADOR]` en `[DISPOSITIVO]`, el visor obtuvo `[FPS_PROMEDIO]` FPS promedio y `[FRAME_TIME_PROMEDIO]` ms de frame time. Esto indica `[CUMPLE/NO_CUMPLE]` la meta operativa de 30 FPS, con la salvedad de `[P95_FRAME_TIME]` ms en percentil 95.

### Formula FPS y frame time

```text
FPS = 1000 / frame_time_ms
```

Variables:

- `FPS`: cuadros por segundo percibidos por el usuario.
- `frame_time_ms`: milisegundos que tarda un cuadro en renderizarse.
- `1000`: conversion de segundos a milisegundos.

Ejemplo del proyecto:

```text
FPS = 1000 / 33.33 = 30
```

Si el modelo optimizado baja de 45 ms a 28 ms por cuadro, el cambio no es solo numerico: el usuario pasa de una interaccion pesada a una respuesta visual mas fluida para orbitar, aislar piezas o activar modos de inspeccion.

## Usabilidad

SUS se aplica solo al prototipo 3D. Debe reportarse:

- puntaje por participante;
- promedio;
- rango;
- interpretacion descriptiva;
- lectura frente a 68 como referencia historica;
- precaucion por tamano y tipo de muestra.

Lectura final probable:

- SUS no se aplica al soporte 2D porque el 2D no es un sistema interactivo equivalente;
- el puntaje SUS describe aceptacion y facilidad percibida del visor 3D;
- 68 es promedio historico, no "buena usabilidad";
- un resultado cercano o superior a 72 puede presentarse como senal favorable, sin convertirlo en prueba universal.

Ejemplo defendible:

> El visor 3D obtuvo un SUS promedio de `[SUS_PROMEDIO]`. Este valor se interpreta descriptivamente frente al promedio historico de 68 y al criterio deseable de 72 o superior. Dado el tipo de muestra, el resultado orienta la lectura formativa del prototipo, no una generalizacion poblacional.

### Formula SUS aplicada

```text
SUS = suma_ajustada x 2.5
```

Para items impares:

```text
aporte = respuesta - 1
```

Para items pares:

```text
aporte = 5 - respuesta
```

Variables:

- `respuesta`: valor de 1 a 5 marcado por el participante.
- `aporte`: contribucion ajustada de cada item.
- `suma_ajustada`: suma de los diez aportes.
- `2.5`: factor que transforma el rango 0-40 en escala 0-100.

Ejemplo:

Si un participante suma 31 puntos ajustados:

```text
SUS = 31 x 2.5 = 77.5
```

En el proyecto, ese valor indicaria que la interfaz 3D fue percibida como usable, pero todavia debe cruzarse con comentarios Think-Aloud para saber por que funciono o donde fallo.

## Workload percibido

NASA-TLX Raw compara 3D y 2D por condicion. Deben reportarse:

- promedio global por condicion;
- dimensiones individuales;
- variabilidad entre participantes;
- relacion con tareas observadas;
- explicacion cualitativa de resultados.

Lectura final probable:

- si 3D obtiene menor workload que 2D, se interpreta como menor esfuerzo subjetivo para las tareas evaluadas;
- si 3D obtiene workload similar o mayor, se revisa si la interactividad anadio friccion, si hubo problemas de onboarding o si el 2D era suficiente para la tarea;
- la dimension de rendimiento debe estar orientada para que valores mas altos signifiquen peor percepcion, manteniendo coherencia con las demas dimensiones.

Ejemplo defendible:

> En NASA-TLX Raw, la condicion 3D registro `[TLX_3D]` y la condicion 2D registro `[TLX_2D]`. La diferencia descriptiva de `[DIFERENCIA_TLX]` puntos sugiere `[LECTURA]`, especialmente en las dimensiones `[DIMENSIONES_CLAVE]`.

### Formula NASA-TLX Raw

```text
NASA_TLX_Raw = (DM + DF + DT + R + E + F) / 6
```

Variables:

- `DM`: demanda mental, cuanto esfuerzo cognitivo percibio el usuario.
- `DF`: demanda fisica, cuanto esfuerzo corporal o motor percibio.
- `DT`: demanda temporal, cuanta presion de tiempo sintio.
- `R`: rendimiento autopercibido orientado de forma que 0 sea desempeno perfecto y 100 desempeno deficiente.
- `E`: esfuerzo total requerido.
- `F`: frustracion durante la tarea.

Ejemplo:

```text
NASA_TLX_Raw = (35 + 10 + 25 + 20 + 30 + 15) / 6 = 22.5
```

En el proyecto, un valor menor en 3D puede indicar que la exploracion interactiva ayudo a reducir esfuerzo percibido. Pero si `DT` sube, podria significar que el usuario entiende mejor el ensamblaje, aunque tarda mas por estar aprendiendo los controles.

## Think-Aloud

Esta parte explica los numeros:

- donde se confundieron los usuarios;
- que acciones fueron intuitivas;
- que iconos o modos causaron duda;
- como afecto el onboarding;
- que partes del sistema redujeron o aumentaron esfuerzo.

Lectura final probable:

- Think-Aloud explica el por que detras de SUS y NASA-TLX;
- comentarios sobre "ahora entiendo donde va esta pieza" fortalecen la lectura espacial;
- comentarios como "no se que hace este icono" senalan problemas de UI;
- dudas repetidas en controles indican necesidad de ajustar onboarding, tooltips o jerarquia visual.

Ejemplo defendible:

> Los comentarios se codificaron en categorias como orientacion espacial, comprension de iconos, friccion de navegacion y reconocimiento de piezas. La categoria con mayor recurrencia fue `[CATEGORIA_CLAVE]`, lo que coincide con `[DATO_CUANTITATIVO_RELACIONADO]`.

## Preguntas dificiles de defensa

### Por que hay marcadores internos de reemplazo?

Porque el material de practica se redacta como version final anticipada. Los marcadores internos evitan inventar datos y obligan a reemplazar cada valor con evidencia real antes de entrega o sustentacion.

### Cuando se puede cerrar definitivamente el capitulo 5?

Cuando la misma build congelada tenga mediciones tecnicas, pruebas de usuario y evidencia cualitativa trazable.

### Que pasa si los resultados no favorecen al 3D?

Debe reportarse. El objetivo es evaluar formativamente, no forzar una conclusion. Un resultado mixto puede ser academicamente valioso si explica limites y mejoras necesarias.

### Que significa que un resultado sea trazable?

Que se pueda saber cuando se midio, en que build, con que dispositivo, bajo que navegador, con que protocolo y con que instrumento.

## Fuentes para estudiar mas

- Brooke (1996), SUS.
- Bangor et al. (2008, 2009), interpretacion SUS.
- Hart y Staveland (1988), NASA-TLX.
- Hart (2006), uso de NASA-TLX.
- Ericsson y Simon (1993), Think-Aloud.
- Lazar et al. (2017), metodos HCI.

## Explicaciones complejas dentro de la seccion

### Marcador interno de reemplazo

Un marcador interno de reemplazo es una senal de trabajo, no una frase para dejar en el documento final. Indica que el parrafo ya tiene forma argumentativa final, pero espera el dato medido.

Ejemplo:

```text
El prototipo obtuvo [SUS_PROMEDIO] puntos en SUS.
```

Cuando exista el dato real, el marcador debe convertirse en:

```text
El prototipo obtuvo 76.8 puntos en SUS.
```

### Dato trazable

Un dato trazable debe responder:

- que se midio;
- con que herramienta;
- en que build;
- en que fecha;
- con que equipo;
- bajo que navegador;
- con que condicion de prueba.

### Comparacion pre/post

La comparacion pre/post en pipeline debe mostrar cambio real: antes y despues de optimizar. Puede incluir peso, vertices, renderers, draw calls, memoria o tiempo de carga.

## Terminos importantes de la seccion

- KPI: indicador clave de desempeno.
- Post-freeze: despues de congelar la build final.
- FPS promedio: media de fluidez durante prueba.
- Frame time: tiempo por cuadro.
- Draw calls: ordenes de render enviadas a GPU.
- Cache: almacenamiento del navegador que puede acelerar cargas posteriores.
- Distribucion: forma en que se reparten los datos.
- Rango: valor minimo y maximo.
- Categoria analitica: grupo usado para clasificar observaciones cualitativas.
