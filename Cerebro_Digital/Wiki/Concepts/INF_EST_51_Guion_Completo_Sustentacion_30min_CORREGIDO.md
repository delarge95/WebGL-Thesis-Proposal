---
tipo: guion_sustentacion
fuente: Informe_final/informe_final.pdf
estado: activo
area: sustentacion
version: 4 (Corregida con Storytelling)
duracion_objetivo: 30min
tags:
  - tesis
  - guion
  - storytelling
  - sustentacion
  - ensayo
  - oratoria
---

# INF EST 51 Guion Completo de Sustentación en 30 Minutos (Versión Narrativa)

## Propósito

Este documento es la versión pulida del guion de sustentación. Se ha reestructurado utilizando principios de oratoria de Nancy Duarte y Matt Abrahams, optimizando la fluidez fonética para que el lenguaje sea 100% humano y conversacional. Se ha ordenado el desarrollo de la app como una "historia de uso" lógica (Inspección -> Desarmado -> Operación/Térmico).

La idea central de toda la sustentación es:
> **Ver piezas no basta. Hay que comprender relaciones espaciales y de operación.**

## Convenciones de Oratoria (Recordatorio)
- `[PAUSA 1s]`: silencio breve para digerir la idea.
- `[PAUSA 2s]`: silencio deliberado para una transición fuerte.
- `[VOZ FIRME]`: tono seguro, estable, marcando autoridad.
- `[VOZ CONVERSACIONAL]`: tono cercano, como si le contaras a un colega un descubrimiento.
- `[MIRADA JURADO]`: conexión visual directa.

---

# Guion Canónico

## 0:00-0:50 - Gancho Inicial

### En pantalla debe verse
- Una lámina técnica 2D del dron o una vista explotada estática. Sin texto excesivo.

### Posición y gesto
- Centro del espacio. Pies firmes. `[MANOS CASA]`.
- Mirar la lámina medio segundo y luego mirar al jurado directamente.

### Decir
> `[VOZ CONVERSACIONAL]` Imaginen que tienen en sus manos este dron... y junto a él, les entregan este manual.

`[PAUSA 1s]`

> El manual es exhaustivo. Muestra cada componente, detalla referencias y usa líneas para indicar dónde va cada pieza. Cumple su función informativa.

`[PAUSA 1s]`

> Pero leer esa información plana tiene un costo mental. Entender la profundidad, calcular qué pieza queda oculta detrás de otra o visualizar el ensamblaje desde un ángulo distinto, exige que el usuario reconstruya el modelo 3D en su cabeza.

> `[MIRADA JURADO]` El verdadero cuello de botella para entender un hardware complejo no es la falta de información... es el esfuerzo cognitivo necesario para procesarla espacialmente.

`[PAUSA 2s]`

> Esa distancia, entre tener la información plana y lograr una comprensión espacial del sistema, es el punto de partida de esta tesis.

### Puente
`[AVANZAR A PORTADA]`

## 0:50-1:30 - Saludo

### Decir
> `[VOZ FIRME]` Buenos días. Mi nombre es Alexander Woodcock Salomón.

> Hoy presento mi proyecto de grado: un visor web para inspeccionar y comprender el ensamblaje de un dron real, el Holybro X500 V2.

> `[GESTO ABIERTO]` El título formal está a mis espaldas. Pero la pregunta de fondo que nos reúne hoy es más sencilla: ¿cómo pasamos de ver piezas sueltas y estáticas, a entender cómo funcionan juntas?

> Para responder a eso, veamos el contraste directo.

## 1:30-2:00 - Del Manual al Visor

### En pantalla debe verse
- Izquierda: manual 2D. Derecha: visor 3D en funcionamiento.

### Decir
> En la documentación tradicional, las piezas aparecen separadas, vistas desde ángulos fijos. La documentación no es el enemigo, es necesaria.

> El límite aparece cuando es el usuario quien tiene que reconstruir mentalmente la profundidad y las conexiones. 

`[AVANZAR A VISOR]`

> Este proyecto explora una alternativa: convertir ese ensamblaje en una experiencia web 3D interactiva. Aquí, la persona no imagina... la persona explora. 

> `[VOZ FIRME]` El manual muestra piezas. El visor ayuda a leer el sistema.

## 2:00-4:30 - Problema, Pregunta y Objetivos

### Decir
> El problema tiene tres capas.

> `[SENALAR]` Primera: humana. Entender hardware complejo desde documentación plana exige recordar, comparar y reconstruir mentalmente.
> Segunda: técnica. Un modelo CAD está pensado para manufactura, no para ejecutarse en un navegador.
> Tercera: metodológica. No basta con construir el visor. Hay que evaluarlo con criterios claros.

`[PAUSA 1s]`

> `[MIRADA JURADO]` Por eso la pregunta de investigación fue: ¿cómo diseñar y evaluar un visor web 3D que ayude a inspeccionar este ensamblaje, sin perder viabilidad técnica en el navegador?

`[AVANZAR]`

> Los objetivos nos marcaron la ruta: entender el estado del arte, transformar el modelo CAD para la web, construir la arquitectura de la app y, finalmente, evaluar tanto el rendimiento técnico como la experiencia de quien lo usa.

## 4:30-6:30 - Fundamento: Cognición, Interacción y Rendimiento

### Decir
> `[VOZ CONVERSACIONAL]` Toda esta ruta no partió de intuiciones. Partió de la teoría de carga cognitiva, que nos dice algo muy simple: nuestra memoria a corto plazo se agota rápido.

> En la documentación 2D, gran parte de la energía mental se gasta en reconstruir espacialmente las piezas. Al pasar a un entorno 3D, la carga espacial es resuelta por la renderización gráfica, pero el esfuerzo se traslada a interactuar con el sistema. El reto entonces cambia: la interfaz debe facilitar la manipulación de forma tan directa, que el usuario invierta su energía en analizar el dron, y no en descifrar cómo operar la aplicación.

`[AVANZAR]`

> Pero esta solución interactiva se enfrenta directamente con la barrera del rendimiento. 

> `[GESTO ABIERTO]` Un modelo CAD industrial está construido matemáticamente mediante superficies paramétricas precisas. Para renderizarlo en WebGL, debe ser convertido a una malla poligonal. Si esa conversión (*teselación*) no se controla, el número de vértices satura el hilo de renderizado y el tiempo por cuadro se dispara. 
> `[ACENTO: conservar]` Optimizar este modelo requirió un proceso técnico de retopología y horneado de texturas (*baking*). Esto permitió reducir drásticamente el peso geométrico y mantener la cuenta de llamadas de dibujado (*draw calls*) bajo control, asegurando 30 cuadros por segundo sin comprometer la legibilidad técnica.

## 6:30-10:00 - Metodología

### Decir
> Para construir esto con rigor, aplicamos la metodología *Design Science Research*. En palabras sencillas: investigar mediante la creación de un artefacto y evaluarlo rigurosamente.

> Y esta evaluación debía medir dos mundos.

> `[SENALAR]` Por un lado, los fierros: los FPS, el uso de memoria, el tiempo de carga.
> Por otro lado, la persona: usamos el cuestionario SUS para medir la usabilidad de la interfaz, y la escala NASA-TLX para medir el esfuerzo mental percibido durante tareas específicas.

`[PAUSA 1s]`

> `[VOZ FIRME]` Quiero ser claro en este punto: NASA-TLX no mide directamente la teoría cognitiva. Mide qué tan exigente o frustrante sintió el usuario la tarea. Acompañamos esto con la técnica de *Think-Aloud*, escuchando a los usuarios mientras navegaban, para entender el *porqué* detrás de los números.

## 10:00-12:00 - Del CAD a la Web (El Reto Técnico)

### Decir
> Llegó el momento de construirlo. Y la primera barrera fue darme cuenta de que el modelo CAD era solo la materia prima, no el producto final.

> El CAD está optimizado para la precisión de manufactura, mientras que WebGL requiere bajo peso en memoria y tiempos de dibujo rápidos. 

> `[GESTO ABIERTO]` Esto exigió diseñar un *pipeline* estructurado: teselación controlada en MoI3D, reconstrucción topológica en Blender y transferencia de detalles (*baking* de normales) en Marmoset. No utilizamos conversores automáticos; cada componente fue procesado para cumplir con el presupuesto de rendimiento que exige un navegador.

## 12:00-15:30 - El Lenguaje de la App: Piezas y Módulos

### Decir
> Una vez que logramos que el dron viviera en la web, el reto fue hacerlo entendible. 

> `[MIRADA JURADO]` La interfaz la diseñé con filosofía *mobile-first*. Si funciona en el espacio reducido de un celular, funcionará perfecto en escritorio. Todo se organiza en un flujo de uso lógico que acompaña al usuario paso a paso.

### 1. Primer Contacto: Onboarding e Inspect
> Antes de exigirle a alguien que inspeccione el dron, la aplicación le enseña cómo hacerlo. Mediante un **Onboarding procedural**, dibujado por código, el sistema le muestra micromovimientos en pantalla. Reduce la incertidumbre antes del primer toque.

> La interacción principal es la selección directa. Al tocar una pieza, se activa el módulo **Inspect** y se despliega el *Info Panel*. 
> Este panel expone la jerarquía real del ensamblaje: identifica si el objeto seleccionado es un sujetador (*fastener*), una subpieza o el ensamble padre. Funciona como el puente entre la selección geométrica y el acceso a los datos técnicos. Desde aquí, el usuario puede aislar componentes específicos o navegar utilizando *Hotspots* predefinidos.

### 2. Desarmado: Analyze
> Una vez que entendemos qué es cada pieza, el siguiente paso lógico es separarlas. Para eso construí el módulo **Analyze**. 
> Aquí pasamos de observar a deconstruir. Activamos vistas explosivas para entender cómo encaja el chasis, o usamos un plano de Corte Transversal donde, mediante un cálculo matemático en el *shader*, ocultamos la mitad del dron para revelar sus entrañas sin destruir la malla 3D original. También implementamos filtros para aislar toda la tornillería o solo los motores con un solo clic.

### 3. Operación y Simulación: Studio, Encendido y Térmico
> Pero un dron no es una estatua. Es una máquina que opera. Así llegamos al módulo **Studio**. 

> Lo primero que hicimos fue dotar al dron de vida: **la secuencia de encendido**. Al activarlo, el estado pasa de reposo a operación, y las hélices comienzan a rotar. 

> En este estado de operación se integra el **Modo Térmico**. 
> `[MIRADA JURADO]` Es fundamental aclarar que no es una simulación de elementos finitos (FEA). Es una representación visual basada en una heurística de transferencia térmica. Al encenderse, los motores generan calor; la aplicación calcula esta aproximación en tiempo real y, a través de un *shader* personalizado, muestra cómo esa temperatura se transfiere hacia los brazos del chasis, codificando la variable numérica en escalas de color comprensibles.

> Además, en este módulo podemos cambiar estilos de renderizado, como rayos X o un entorno completo de diseño tipo *Blueprint*, para evaluar el contraste y la legibilidad.

## 18:30-19:30 - Arquitectura Subyacente

### Decir
> Para sincronizar la interfaz de usuario, la manipulación 3D y los modos de visualización, se requirió una arquitectura basada en eventos.
> El desarrollo incluyó controladores *runtime* para gestionar el estado visual y la selección, además de *scripts* analíticos en el editor de Unity. Estos validadores automatizan la auditoría de *colliders*, materiales y metadatos de cada pieza, previniendo errores estructurales antes de compilar la versión de producción en WebGL.

## 22:30-25:00 - Resultados (La Evidencia)

### Decir
> Con el sistema andando, le hicimos las preguntas difíciles a los datos. Los resultados se dividen en tres áreas.

> `[SENALAR]` Primero, **rendimiento técnico**. En la optimización, reducimos el peso en un `[REDUCCION_PESO]`%. Logramos que la aplicación corriera a `[FPS_PROMEDIO]` cuadros por segundo, manteniendo un *frame time* estable que permite que la interacción móvil sea fluida.

> Segundo, **usabilidad**. Evaluamos el visor 3D usando SUS, obteniendo un puntaje de `[SUS_PROMEDIO]`, superando el umbral base de 68 y demostrando que la interfaz es intuitiva.

> Tercero, **esfuerzo percibido**. Con NASA-TLX comparamos la lectura en 2D vs nuestro visor 3D. El visor obtuvo `[TLX_3D]` frente a `[TLX_2D]` del manual, y los comentarios del *Think-Aloud* corroboraron que la reducción de carga visual espacial fue el factor determinante para aliviar el esfuerzo del usuario.

## 25:00-28:00 - Aportes, Límites y Trabajo Futuro

### Decir
> Integrando todo esto, el proyecto entrega aportes muy claros: 
> 1. Un pipeline validado para llevar CAD complejo a WebGL. 
> 2. Una arquitectura de visor con tres módulos progresivos (Inspección, Análisis, Estudio). 
> 3. Herramientas avanzadas como la animación de encendido y la aproximación del modo térmico.

> `[MIRADA JURADO]` Pero toda buena investigación reconoce sus límites. Nuestra validación térmica es heurística y visual, no calibrada físicamente. La muestra de usuarios nos permite una validación formativa, no generalizada poblacionalmente. Además, elementos hipercomplejos como el cableado interno se omitieron por razones de rendimiento.

> Estos límites son exactamente donde inicia el trabajo futuro: integraciones con motores de termodinámica reales, soporte para múltiples idiomas y la anhelada automatización del pipeline de modelos CAD.

## 28:00-29:00 - Cierre

### En pantalla debe verse
- Imagen inicial del manual transicionando hacia el visor interactivo iluminado. Frase en pantalla: "De piezas aisladas a relaciones espaciales".

### Decir
> `[VOZ CONVERSACIONAL]` Volvamos por un instante a la imagen del inicio. Al manual.

> Este proyecto no busca reemplazar la documentación técnica tradicional. Busca darle vida.

> `[MIRADA JURADO]` La contribución real de esta tesis no es solamente haber programado una aplicación con tres módulos de visualización. Es el puente metodológico y técnico que hemos construido para llevar un modelo de ingeniería complejo, directamente a la mente de quien necesita entenderlo, utilizando nada más que un navegador web.

`[PAUSA 2s]`

> Hemos pasado de mirar piezas aisladas... a explorar relaciones espaciales dinámicas.

`[PAUSA 2s]`

> `[VOZ FIRME]` Y esa es la diferencia vital entre simplemente entregar un modelo en 3D... y construir una verdadera herramienta de comprensión técnica.

`[MIRADA JURADO]` Muchas gracias.
