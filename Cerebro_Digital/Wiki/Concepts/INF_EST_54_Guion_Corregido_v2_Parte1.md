---
tipo: guion_sustentacion
fuente: INF_EST_51_Guion_Completo_Sustentacion_30min
estado: activo
area: sustentacion
version: 2
parte: 1
duracion_objetivo: 15min30s
tags:
  - tesis
  - guion
  - sustentacion
  - oratoria
  - partitura
---

# INF EST 54 Guion Corregido v2 Parte 1

Guion corregido de sustentacion, bloques 0:00 a 15:30. Formato partitura con indicaciones de posicion, gesto, mirada, voz, pausas y puentes narrativos.

Documentos relacionados:

- [[INF_EST_53_Auditoria_Oratoria_v2_Diagnostico]]
- [[INF_EST_55_Guion_Corregido_v2_Parte2]]
- [[INF_EST_51_Guion_Completo_Sustentacion_30min]]

---

# Guion Corregido v2 — Sustentacion 30 min (Parte 1: 0:00-15:30)

> **Convenciones:** `[PAUSA 1s]` silencio breve · `[PAUSA 2s]` silencio deliberado · `[RESPIRAR]` inhalar · `[VOZ BAJA]` volumen bajo · `[VOZ FIRME]` tono seguro · `[ÉNFASIS]` resaltar palabra · `[MIRADA JURADO]` mirar al jurado · `[MIRADA PÚBLICO]` abrir al auditorio · `[GESTO ABIERTO]` palmas visibles · `[SEÑALAR]` dirigir atención · `[AVANZAR]` cambiar diapositiva.

---

## 0:00–0:50 · Gancho Inicial

**Mostrar:** Lámina técnica 2D del dron. Todavía no mostrar la app.

**Posición:** De pie, centrado. Manos en casa. Mirar primero la lámina, luego al jurado.

**Decir:**

> Imaginen que les entregan este dron y un manual.

`[PAUSA 1s]`

> El manual muestra cada pieza, con nombres y referencias. `[SEÑALAR]` Cumple su función.

`[PAUSA 1s]`

> Pero hay algo que no muestra: dónde vive cada pieza dentro del sistema. Qué se conecta con qué. Qué queda oculto debajo de otra parte.

`[MIRADA JURADO]`

> `[ÉNFASIS]` Entender un ensamblaje no es solo mirar componentes. Es entender relaciones.

`[PAUSA 2s]`

> Esa distancia —entre ver una pieza y comprender un sistema— es el punto de partida de esta tesis.

**Acción:** Pausar. Avanzar a portada. Volver al centro.

---

## 0:50–1:30 · Saludo

**Mostrar:** Portada formal.

**Decir:**

> `[VOZ FIRME]` Buenos días. Mi nombre es Alexander Woodcock Salomón.

> Hoy presento mi proyecto de grado: un visor web para inspeccionar el ensamblaje de un dron real, el Holybro X500 V2.

> `[GESTO ABIERTO]` El título formal está en la portada. La pregunta de fondo es más sencilla: ¿cómo pasamos de ver piezas sueltas a entender cómo funcionan juntas?

**Acción:** No leer el título. Sonreír ligeramente. Avanzar.

---

## 1:30–2:00 · Del Manual al Visor

**Mostrar:** Manual 2D → captura del visor (transición visual).

**Decir:**

> En documentación técnica, las piezas aparecen separadas, numeradas y vistas desde ángulos fijos.

> Esa documentación es necesaria. No es el enemigo.

`[PAUSA 1s]`

> El límite aparece cuando el usuario tiene que completar mentalmente la profundidad, la jerarquía y las conexiones.

`[AVANZAR A VISOR]`

> Este proyecto explora una respuesta: convertir ese ensamblaje en una experiencia web 3D donde el usuario puede explorar, aislar piezas y cambiar cómo las ve.

> `[VOZ FIRME]` El manual muestra piezas. El visor ayuda a leer relaciones.

**Puente:**

> Para convertir esa intuición en tesis, primero había que formular bien el problema.

---

## 2:00–4:30 · Problema, Pregunta y Objetivos

**Mostrar:** Diapositiva con tres capas: humana, técnica, metodológica. Luego pregunta de investigación.

**Decir:**

> El problema tiene tres capas.

> `[SEÑALAR]` Primera: humana. Entender hardware complejo desde documentación plana exige recordar, comparar y reconstruir mentalmente.

> Segunda: técnica. Un modelo CAD está pensado para manufactura, no para ejecutarse en un navegador.

> Tercera: metodológica. No basta con construir el visor. Hay que evaluarlo con criterios claros.

`[PAUSA 1s]`

> `[MIRADA JURADO]` Por eso la pregunta fue: ¿cómo diseñar y evaluar un visor web 3D que ayude a inspeccionar este ensamblaje, sin perder viabilidad técnica en navegador?

`[AVANZAR]`

> Los objetivos siguen esa misma ruta: entender el caso, transformar el modelo, implementar el prototipo y evaluar rendimiento y experiencia de uso.

**Puente:**

> Esa ruta necesitaba respaldo teórico. No partimos de intuición.

---

## 4:30–6:30 · Fundamento: Cognición, Interacción y Rendimiento

**Mostrar:** Diagrama 2D → reconstrucción mental → esfuerzo. Luego 3D → manipulación directa.

**Decir:**

> La teoría de carga cognitiva parte de una idea simple: nuestra memoria de trabajo es limitada.

> Si el usuario debe imaginar una pieza, rotarla mentalmente y compararla con otra vista, parte de su esfuerzo se va en reconstruir el espacio.

`[AVANZAR]`

> En una visualización interactiva, parte de ese trabajo pasa a la interfaz. El usuario ya no solo imagina: rota, aísla y compara directamente.

> Desde interacción humano-computador, esto se traduce en reglas prácticas: mostrar estado, usar controles consistentes y no obligar a memorizar.

`[PAUSA 1s]`

> `[MIRADA PÚBLICO]` ¿Eso significa que el 3D siempre es mejor que el 2D?

> `[VOZ FIRME]` No. Este proyecto evalúa diferencias descriptivas en un caso concreto: tareas, esfuerzo percibido, usabilidad y retroalimentación cualitativa de los usuarios.

`[AVANZAR]`

**Mostrar:** Fórmula FPS en diapositiva.

> Pero hay otra cara del problema: el rendimiento.

> `[GESTO ABIERTO]` La idea es simple: si cada cuadro de la pantalla tarda demasiado en dibujarse, la interacción se vuelve lenta.

> `[SEÑALAR]` Si el tiempo por cuadro es 33 milisegundos, tenemos 30 cuadros por segundo. Si el modelo pesa demasiado, ese tiempo sube.

> `[ÉNFASIS]` Optimizar no fue bajar calidad. Fue decidir qué conservar para que el dron siguiera siendo legible en la web.

**Puente:**

> Con esa base teórica, la investigación necesitaba un método: construir, demostrar y evaluar.

---

## 6:30–10:00 · Metodología

**Mostrar:** Diagrama DSRM.

**Decir:**

> La metodología es Design Science Research. En simple: investigar construyendo y evaluando un artefacto.

> `[GESTO ABIERTO]` El proceso tiene una ruta clara: identificar el problema, definir objetivos, diseñar, demostrar, evaluar y comunicar. Se revisa con criterios de rigor y relevancia.

`[PAUSA 1s]`

> La evaluación cruza dos mundos.

> En lo técnico: fluidez, tiempo de carga, memoria y dispositivo.

> En experiencia: usabilidad del visor, esfuerzo percibido y retroalimentación cualitativa.

`[AVANZAR]` **Mostrar:** Tabla de instrumentos.

> `[MIRADA JURADO]` Aquí necesito una precisión.

> Cuando hable de esfuerzo percibido, me refiero a lo que siente el usuario durante la tarea: qué tan demandante, frustrante o exigente le resultó.

> Eso se evalúa con un cuestionario llamado NASA-TLX en su versión Raw: seis preguntas que promedian dimensiones de esfuerzo.

`[PAUSA 1s]`

> `[VOZ FIRME]` NASA-TLX no mide directamente la teoría de carga cognitiva. Mide esfuerzo de trabajo percibido. La teoría explica el problema; el instrumento mide evidencia concreta.

`[AVANZAR]` **Mostrar:** SUS.

> Para usabilidad, se usa SUS: diez preguntas que producen un puntaje de 0 a 100.

> El 68 es un promedio histórico, no un certificado de buena usabilidad. Para este proyecto, una lectura favorable estaría cerca o por encima de 72.

> `[ÉNFASIS]` SUS se aplica solo al visor 3D, porque es el sistema interactivo que se evalúa. El soporte 2D funciona como condición de referencia para tareas, no como sistema equivalente.

`[PAUSA 1s]`

> Y la retroalimentación cualitativa se recoge con el protocolo Think-Aloud: el usuario verbaliza lo que piensa mientras usa la herramienta. Eso explica lo que los números por sí solos no cuentan.

> `[MIRADA JURADO]` La meta ideal es contar con 30 participantes o más. Si el número es menor, el estudio se mantiene como evaluación formativa, sin generalizar a toda una población.

**Puente:**

> Para poder medir todo eso, primero había que construir una versión web real. Ahí empieza el desarrollo.

---

## 10:00–12:00 · Del CAD a la Web

**Mostrar:** Diagrama CAD → MoI3D/Blender → Unity → Web. Captura antes/después.

**Decir:**

> `[VOZ FIRME]` El desarrollo empezó con una dificultad central: el modelo CAD no era el producto final. Era materia prima.

> Un modelo CAD está pensado para precisión y fabricación. Un visor web necesita otra cosa: lectura visual, bajo peso y respuesta en tiempo real.

> `[SEÑALAR]` Por eso probé rutas distintas: conversión directa, control de teselación desde MoI3D, limpieza manual y reconstrucción de piezas.

> También usé herramientas de producción 3D profesional, desde Blender hasta baking en Marmoset. No son accesorios; hacen parte real de la cadena de trabajo.

`[PAUSA 1s]`

> `[ÉNFASIS]` El reto no era abrir un archivo. Era traducir ingeniería a experiencia interactiva.

**Puente:**

> Esa traducción obligó a tomar decisiones pieza por pieza.

---

## 12:00–15:30 · Piezas, MAD-T y Tornillos Modulares

**Mostrar:** Captura de proceso. Tabla de conteos. Ejemplo de tornillos.

**Decir:**

> Para limpiar y reconstruir geometría, adapté un marco técnico de producción llamado MAD-T, de Blender Bros.

> `[GESTO ABIERTO]` Lo uso como guía de producción, no como metodología científica. La metodología del proyecto es Design Science.

> Algunas piezas se limpiaron. Otras se reconstruyeron. Y otras se modelaron desde cero porque su versión original no servía para tiempo real.

`[AVANZAR]`

> Un punto clave fueron los tornillos. Parecen pequeños, pero se repiten muchas veces.

> Si cada tornillo se guarda como geometría única, el costo crece rápido. La solución fue modularizar: reutilizar familias y conservar lectura visual.

`[AVANZAR]` **Mostrar:** Conteos 28 / 30 / 257.

> `[MIRADA JURADO]` Por eso los conteos se leen por capas.

> `[SEÑALAR]` Veintiocho piezas canónicas: las entidades de investigación.

> Treinta nodos de escena: los puntos de anclaje donde vive cada pieza.

> Doscientos cincuenta y siete elementos técnicos: fragmentos de geometría, colliders y assets que el motor necesita para renderizar e interactuar.

`[PAUSA 1s]`

> `[VOZ FIRME]` No son contradicciones. Son niveles distintos: investigación, escena y ejecución.

**Puente:**

> Una vez el modelo fue viable en web, el siguiente reto fue hacerlo entendible para una persona.
