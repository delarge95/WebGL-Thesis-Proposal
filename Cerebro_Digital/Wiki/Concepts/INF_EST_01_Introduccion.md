---
tipo: modulo_estudio
fuente: Informe_final/chapters/01_introduccion.tex
estado: activo
capitulo: 1
area: informe-final
tags:
  - tesis
  - introduccion
  - problema
---

# INF EST 01 Introduccion

## Idea central

La introduccion explica por que el proyecto existe: la documentacion tecnica basada en imagenes 2D y PDF obliga al usuario a reconstruir mentalmente relaciones espaciales complejas. En un dron multicomponente como el Holybro X500 V2, esa reconstruccion no es trivial porque hay piezas estructurales, electronicas, energeticas y de control distribuidas en un ensamblaje tridimensional.

La solucion planteada es un visor web 3D interactivo que traslada parte de esa carga de interpretacion al sistema: el usuario puede rotar, seleccionar, aislar, analizar y leer piezas dentro del propio modelo.

## Que debe entender una persona no especialista

El problema no es que los manuales 2D sean inutiles. El problema es que obligan a imaginar en la cabeza como encajan las piezas. Cuando el sistema tiene muchas partes, esa imaginacion se vuelve costosa y propensa a error.

El visor 3D busca hacer visible lo que el PDF deja implicito:

- donde esta cada pieza;
- como se relaciona con las demas;
- que subsistema representa;
- que ocurre al separarla, aislarla o verla bajo un modo de analisis.

## Secciones del capitulo

### Planteamiento del problema

Aqui se define la tension principal: hay una necesidad real de comprension espacial, pero tambien restricciones tecnicas de la web.

La frase clave es que el proyecto no consiste solo en "poner un modelo 3D en internet". La tarea real es convertir un modelo CAD pesado en una experiencia interactiva que funcione en un navegador.

El informe final ya no presenta la pregunta de investigacion como una sola oracion extensa. La divide en dos preguntas complementarias:

- una pregunta principal sobre diferencias descriptivas entre visor 3D y soporte 2D en desempeno de tareas, workload percibido y viabilidad tecnica en navegador;
- una pregunta complementaria sobre el nivel de usabilidad percibida del prototipo 3D medido con SUS.

Esta division mejora la defensa porque separa la comparacion 3D/2D de la evaluacion de usabilidad del sistema interactivo.

Conceptos que se deben dominar:

- [[INF_EST_90_Glosario_Global#Hardware complejo]]
- [[INF_EST_90_Glosario_Global#CAD]]
- [[INF_EST_90_Glosario_Global#WebGL]]
- [[INF_EST_90_Glosario_Global#WebAssembly]]
- [[INF_EST_90_Glosario_Global#Frame time]]

### Justificacion

La justificacion responde por que Unity Web fue una decision razonable. La respuesta corta es: no fue elegido por ser lo mas liviano, sino por integrar herramientas de desarrollo, UI, materiales, shaders, perfilado y compilacion web en un mismo entorno.

Esta es una defensa importante. Si un jurado pregunta por Three.js, Babylon.js o Unreal Pixel Streaming, la respuesta no debe ser "Unity es mejor". La respuesta correcta es que Unity fue pertinente para este caso por equilibrio entre control, tiempo, herramientas integradas y coherencia con el perfil de Ingenieria Multimedia.

### Objetivos

Los objetivos se distribuyen en cuatro capas:

- preparar activos 3D para web;
- implementar materiales, modos visuales y herramientas de inspeccion;
- construir la interaccion del prototipo;
- evaluar rendimiento, desempeno, usabilidad y workload percibido.

La clave es no presentar la evaluacion como prueba absoluta de superioridad del 3D. Es una evaluacion formativa y descriptiva.

### Alcance y limitaciones

Esta seccion protege el proyecto de sobrepromesas. Aclara que:

- hay un modelo de caso de estudio;
- la validacion empirica final depende de la build congelada;
- algunas herramientas existen pero no estan expuestas;
- el escritorio es adaptacion funcional de una UI mobile-first;
- la compatibilidad movil es esperada, no universal.

## Preguntas dificiles de defensa

### Por que no bastaba con mejorar el manual PDF?

Porque el problema no es solo de redaccion o diagramacion. La dificultad principal esta en reconstruir relaciones tridimensionales. Un PDF puede mejorar, pero sigue siendo estatico. El visor 3D permite manipular la representacion.

### El proyecto demuestra que 3D es mejor que 2D?

No en sentido causal fuerte. El informe plantea una comparacion descriptiva entre condicion 3D y soporte 2D para observar diferencias en desempeno y workload percibido. La usabilidad se mide solo para el prototipo 3D mediante SUS.

### Por que Unity si pesa mas que una biblioteca web?

Porque el proyecto necesitaba una cadena integrada de arte tecnico, shaders, UI, runtime y profiling. Unity aumenta la huella inicial, pero reduce el costo de construir desde cero muchas piezas del sistema.

### Que significa viabilidad en este proyecto?

Viabilidad no significa "funciona en cualquier dispositivo". Significa que, bajo condiciones documentadas, el prototipo puede cargarse, ejecutarse, mantener interaccion estable y cumplir metas operativas como FPS, frame time, memoria y coherencia funcional.

## Debilidades que conviene reconocer

- La validacion empirica aun depende de pruebas finales.
- La compatibilidad movil debe cerrarse con dispositivos reales.
- La UI de escritorio es funcional, pero no fue disenada con el mismo rigor especifico que la version mobile-first.
- El caso de estudio es un dron especifico; la generalizacion requiere nuevos casos.

## Explicaciones complejas dentro de la seccion

### Demanda cognitiva

En el informe, demanda cognitiva no significa "el usuario es incapaz". Significa que la tarea exige memoria de trabajo: comparar piezas, imaginar rotaciones, recordar posiciones y conectar vistas separadas. El visor 3D busca mover parte de ese esfuerzo desde la mente del usuario hacia la interfaz.

### Viabilidad tecnica

Viabilidad tecnica no es una palabra decorativa. Debe entenderse como una condicion verificable: el prototipo debe cargar, mantenerse interactivo, respetar metas de FPS/frame time y conservar consistencia entre piezas, datos y UI.

### Compatibilidad esperada

Compatibilidad esperada significa que la tecnologia permite ese escenario en navegadores compatibles, pero el proyecto debe medirlo por dispositivo y navegador antes de afirmar compatibilidad verificada.

## Terminos importantes de la seccion

- Hardware complejo: sistema con muchas piezas y relaciones espaciales.
- Comunicacion espacial: forma en que un documento o interfaz permite entender ubicacion y relacion entre piezas.
- Soporte 2D: manual, plano, lamina o PDF estatico.
- Visor 3D: aplicacion que permite navegar un modelo tridimensional.
- WebAssembly: tecnologia para ejecutar codigo compilado en navegador.
- WebGL: tecnologia para dibujar 3D acelerado por GPU en navegador.
- Build congelada: version estable usada para medir resultados.
- MVP: version minima viable para probar el enfoque principal.

## Fuentes para estudiar mas

- Sweller (1988), sobre teoria de carga cognitiva.
- Hegarty y Waller (2004), sobre habilidades espaciales.
- Yu et al. (2023), sobre renderizado Web3D.
- Unity Technologies, documentacion Web y WebGL.
- WebAssembly Community Group, especificacion base de WebAssembly.
