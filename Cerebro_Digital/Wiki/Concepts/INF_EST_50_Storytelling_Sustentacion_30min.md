---
tipo: guia_sustentacion
fuente: Informe_final/informe_final.pdf
estado: activo
area: sustentacion
duracion_objetivo: 30min
tags:
  - tesis
  - storytelling
  - sustentacion
  - presentacion
---

# INF EST 50 Storytelling de Sustentacion en 30 Minutos

## Objetivo

Construir una sustentacion que sea tecnicamente rigurosa y narrativamente clara. La meta no es recitar el informe; es llevar al jurado por una historia: un problema real, una decision tecnica compleja, un prototipo funcional, una metodologia honesta y un cierre con limites bien entendidos.

## Tesis narrativa en una frase

> Este proyecto convierte la documentacion estatica de un dron complejo en una experiencia web 3D interactiva, mostrando que la visualizacion tecnica puede apoyarse en pipelines de arte tecnico, UI mobile-first, shaders y evaluacion formativa para hacer mas comprensible un ensamblaje complejo en navegador.

## Arco narrativo

La historia tiene cinco actos:

1. El problema invisible: el usuario tiene que imaginar el 3D desde documentos 2D.
2. El reto tecnico: un CAD industrial no cabe directamente en la web.
3. La construccion del artefacto: pipeline, UI, shaders, runtime y tooling.
4. La evaluacion honesta: DSR, SUS, NASA-TLX Raw, Think-Aloud y KPIs.
5. El cierre responsable: que esta listo, que falta medir y que queda como trabajo futuro.

## Estructura de 30 minutos

### Min 0:00-2:30 Apertura

Objetivo: capturar atencion.

Guion sugerido:

> Cuando una persona abre un manual tecnico de un dron, no solo lee instrucciones. Tiene que reconstruir mentalmente donde esta cada pieza, como se conecta con las demas y que parte del ensamblaje esta observando. Esa reconstruccion espacial es el problema que aborda este proyecto.

Mostrar:

- una imagen o captura de manual 2D;
- una imagen del visor 3D;
- una frase puente: "el problema no era mostrar bonito; era hacer comprensible lo complejo".

Enlaces:

- [[INF_EST_01_Introduccion]]
- [[INF_EST_30_Pipeline_CAD_MAD_T]]

### Min 2:30-5:00 Problema y pregunta de investigacion

Idea clave:

El problema combina comunicacion espacial y restricciones web.

Guion sugerido:

> El reto tenia dos caras. Una humana: entender piezas y relaciones espaciales. Y una tecnica: hacer que un modelo pesado, nacido para CAD, funcionara en un navegador.

Cubrir:

- hardware complejo;
- soporte 2D;
- demanda cognitiva;
- pregunta de investigacion;
- diferencia entre comparar 3D/2D y medir usabilidad del 3D.

Enlaces:

- [[INF_EST_01_Introduccion#Planteamiento del problema]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa#1. El proyecto demuestra que el visor 3D mejora la comprension?]]

### Min 5:00-8:00 Fundamento teorico

Idea clave:

El proyecto no se basa en intuicion. Tiene respaldo en carga cognitiva, HCI, interaccion 3D, usabilidad y renderizado.

Guion sugerido:

> La teoria de carga cognitiva ayuda a explicar por que la documentacion 2D puede ser exigente. Pero la evaluacion no mide directamente esa teoria: mide workload percibido con NASA-TLX. Esa distincion fue central para evitar sobreprometer.

Cubrir:

- carga cognitiva como lente;
- NASA-TLX como workload;
- heuristicas de Nielsen;
- PBR como base visual;
- shaders como lenguaje tecnico.

Enlaces:

- [[INF_EST_02_Marco_Referencia]]
- [[INF_EST_03_Marco_Metodologico#NASA-TLX Raw]]
- [[INF_EST_33_Shaders_ViewModes_Entornos]]

### Min 8:00-12:00 Metodologia

Idea clave:

El proyecto es DSR: se construye y evalua un artefacto.

Guion sugerido:

> No plantee esto como un experimento puro de laboratorio, sino como investigacion aplicada. El artefacto es el visor. El objetivo metodologico es construirlo, demostrarlo, evaluarlo formativamente y documentar que condiciones lo hacen viable.

Cubrir:

- Peffers como proceso;
- Hevner como directrices;
- muestra 30+ ideal y 8-12 minimo operativo;
- SUS solo para prototipo 3D;
- NASA-TLX por condicion;
- Think-Aloud;
- triangulacion.

Enlaces:

- [[INF_EST_03_Marco_Metodologico]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa#3. Por que SUS no se aplica al 2D?]]

### Min 12:00-18:00 Desarrollo: del CAD al visor

Este es el corazon tecnico.

Guion sugerido:

> En esta parte el proyecto deja de ser una idea y se vuelve una cadena concreta de decisiones. El modelo CAD no era el producto final. Era la materia prima. Hubo que traducirlo.

Cubrir:

- rutas de importacion;
- MAD-T adaptado;
- piezas rehechas o modeladas desde cero;
- sistema modular de fasteners;
- software y addons;
- tooling de automatizacion;
- conteos 16/28/30/257.

Momento narrativo fuerte:

> El numero 257 puede sonar como "mas piezas", pero en realidad revela otra cosa: la diferencia entre una taxonomia academica y la fragmentacion tecnica que necesita el motor para renderizar e interactuar.

Enlaces:

- [[INF_EST_04_Desarrollo_Implementacion]]
- [[INF_EST_30_Pipeline_CAD_MAD_T]]
- [[INF_EST_35_Tooling_Arquitectura_Runtime]]

### Min 18:00-22:00 UX/UI, iconos y experiencia

Idea clave:

La interfaz no es maquillaje. Es parte del metodo para reducir friccion.

Guion sugerido:

> Si el usuario debe entender un dron complejo, la interfaz no puede competir con el modelo. Por eso el diseno se concentro en un flujo mobile-first: barra inferior, ficha inferior, acciones por modo y microinteracciones que dan respuesta sin llenar la pantalla de instrucciones.

Cubrir:

- mobile-first;
- desktop como adaptacion funcional;
- onboarding procedural hecho por codigo;
- bottom sheet;
- info panel como traduccion de seleccion a conocimiento;
- jerarquia pieza madre, subpieza, grupo de hotspot y fastener;
- modos Inspect/Analyze/Studio;
- iconos procedurales;
- microinteracciones con resorte.

Enlaces:

- [[INF_EST_31_UX_UI_Mobile_First]]
- [[INF_EST_32_Iconografia_Procedural_Microinteracciones]]

### Min 22:00-25:00 Shaders, Blueprint y Thermal

Idea clave:

Los modos visuales son instrumentos de lectura.

Guion sugerido:

> Cada modo visual responde a una pregunta distinta. Realistic responde como se ve; X-Ray responde que hay dentro; Blueprint responde como leerlo como plano; Thermal responde donde se concentra una tendencia relativa de carga.

Cubrir:

- Realistic;
- X-Ray;
- Blueprint;
- SolidColor;
- Wireframe/Ghosted como ocultos;
- Thermal como heuristico, no FEA;
- formulas termicas resumidas.

Enlaces:

- [[INF_EST_33_Shaders_ViewModes_Entornos]]
- [[INF_EST_34_Sistema_Termico_Hibrido]]

### Min 25:00-27:30 Resultados, lectura y cierre empirico

Idea clave:

La sustentacion debe sonar segura, pero no debe cerrar empiricamente datos que aun no se han medido. La version de ensayo puede conservar marcadores internos de reemplazo; el discurso final solo debe usar datos reales post-freeze.

Guion sugerido:

> Para cerrar el proyecto, los resultados se leen en tres capas: primero, la optimizacion del pipeline; segundo, el rendimiento tecnico de la build Web; tercero, la experiencia de usuario evaluada con SUS, NASA-TLX Raw y Think-Aloud. Esta separacion evita confundir una app que "se ve bien" con una app que tiene evidencia tecnica y formativa.

Cubrir:

- KPIs tecnicos: FPS, frame time, carga, memoria y trazabilidad del dispositivo;
- SUS solo para el prototipo 3D;
- NASA-TLX Raw para comparar workload 3D/2D;
- Think-Aloud como explicacion humana de los numeros;
- marcadores internos `[REEMPLAZAR CON DATO REAL POST-FREEZE]` solo durante preparacion.

Enlaces:

- [[INF_EST_05_Resultados_Analisis]]

### Min 27:30-30:00 Cierre

Guion sugerido:

> La contribucion del proyecto no esta solo en una app que muestra un dron. Esta en documentar como convertir hardware complejo en una experiencia web interactiva, con una arquitectura defendible, una UI pensada para comprension, un pipeline de optimizacion y una metodologia que reconoce sus limites.

Cerrar con tres ideas:

- construccion funcional;
- metodologia honesta;
- continuidad clara.

Enlaces:

- [[INF_EST_06_Conclusiones]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa]]

## Momentos de emocion e intriga

### Momento 1: El manual no esta mal, pero obliga a imaginar

Usar una frase de contraste:

> El manual muestra piezas. El visor muestra relaciones.

### Momento 2: El CAD no era el final, era el problema

Frase:

> El modelo original tenia informacion, pero no tenia todavia forma de experiencia.

### Momento 3: El boton tambien es ingenieria

Frase:

> Incluso los iconos se volvieron parte del pipeline tecnico: no sprites, sino geometria procedural animada.

### Momento 3B: La ficha convierte geometria en conocimiento

Frase:

> Seleccionar una pieza no basta. La ficha inferior explica que es, a que nivel pertenece y como se relaciona con el ensamblaje.

### Momento 4: La honestidad como fortaleza

Frase:

> La tesis no se fortalece diciendo que todo esta cerrado; se fortalece mostrando exactamente que esta verificado, que esta oculto y que falta medir.

## Preguntas que conviene anticipar en vivo

- Por que Unity Web y no Three.js.
- Por que NASA-TLX si se habla de carga cognitiva.
- Por que SUS solo para el 3D.
- Por que 28 piezas y 257 renderers.
- Por que el onboarding fue procedural y no un video.
- Por que la ficha inferior es central para piezas, subpiezas, grupos y fasteners.
- Por que el sistema termico no es FEA.
- Por que hubo marcadores internos de reemplazo durante la preparacion.
- Por que la UI desktop es adaptacion.

Ver respuestas en [[INF_EST_91_Preguntas_Dificiles_Defensa]].

## Lista minima de visuales para la presentacion

- Manual 2D o lamina estatica.
- Captura general del visor.
- Diagrama CAD -> Blender -> Unity -> Web.
- Antes/despues de saneamiento geometrico.
- UI mobile-first.
- Onboarding procedural.
- Bottom sheet de pieza.
- Jerarquia de seleccion: pieza madre, subpieza, grupo y fastener.
- Iconos procedurales.
- Modo X-Ray o Blueprint.
- Modo Thermal con leyenda.
- Tabla de estados funcionales.
- Tabla final de KPIs, SUS, NASA-TLX y categorias Think-Aloud.

## Regla de oro

Practicar el recorrido completo como si la version final ya estuviera lista, pero mantener marcados los datos que deben reemplazarse con medicion real. La exposicion final debe presentar un prototipo funcional avanzado, metodologicamente honesto, con arquitectura, pipeline y resultados trazables; si un dato no esta medido, se dice que queda pendiente post-freeze.

## Referentes de comunicacion aplicados

Estos referentes no se citan como fundamento tecnico de la tesis, sino como guia para construir una sustentacion clara:

- Nancy Duarte: usar contraste entre "lo que ocurre hoy" y "lo que podria ocurrir" para crear arco narrativo.
- Alan H. Monroe: abrir con atencion, mostrar necesidad, presentar satisfaccion, visualizar el cambio y cerrar con accion.
- Chip Heath y Dan Heath: ideas simples, concretas, creibles, emocionales y contadas como historia.
- Garr Reynolds: reducir ruido visual, usar ritmo, pausas y naturalidad.
- Barbara Minto: presentar primero la idea central y luego los argumentos que la sostienen.
