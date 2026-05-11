---
tipo: guion_sustentacion
fuente: INF_EST_54-55 (base) + Deep Research Report (principios)
estado: activo
area: sustentacion
version: 70
parte: 1
duracion_objetivo: 15min
tags:
  - tesis
  - guion
  - sustentacion
  - final
  - defensa-oral
---

# INF_EST_70 — Guión Final de Sustentación (Parte 1: 0:00-15:00)

## Versión Integrada

Este guión integra:

1. Estructura evaluativa del deep research report (problema → gap → propuesta → decisiones → implementación → validación → límites → contribución)
2. Principios de storytelling científico (narrativa + rigor)
3. Multimedia learning (una idea central por sección, títulos-sentencia)
4. Entrega conversacional (landmarks, no memorización rígida)
5. Manejo de ansiedad (preparación robusta, protocolo antifalla)

**Convenciones:**

- `[PAUSA 1s]` = silencio breve para digerir
- `[PAUSA 2s]` = silencio deliberado para transición fuerte
- `[VOZ FIRME]` = tono seguro, autoridad técnica
- `[VOZ CONVERSACIONAL]` = cercana, como a un colega
- `[MIRADA JURADO]` = conexión visual directa
- `[GESTO ABIERTO]` = palmas visibles, expansión
- `[SEÑALAR]` = dirigir atención a visual
- `[AVANZAR]` = cambiar diapositiva/acción

---

# MINUTOS 0:00-0:45 — HOOK INICIAL

## Objetivo de Sección

Capturar atención con una fricción observable que justifique escuchar el resto.

---

### Posición inicial

De pie, centrado frente al jurado. Manos en posición "casa" (frente al abdomen, relajadas). Respiración pausada.

### Mostrar en pantalla

**Diapositiva 1:** Lámina técnica del dron Holybro X500 V2 (vista isométrica 2D o manual de referencia).

- Sin exceso de texto.
- Imagen clara, legible a distancia.
- Debe evocar: complejidad, muchas piezas, relaciones no obvias.

### Decir

> `[VOZ CONVERSACIONAL]` Imaginen que les entregan este dron.

`[PAUSA 1s]`

> Y junto con el dron, les entregan un manual.

`[PAUSA 1s]`

> `[SEÑALAR]` El manual muestra cada pieza: nombres, referencias, especificaciones.

`[PAUSA 0.5s]`

> Cumple su función. Es necesario.

`[PAUSA 1s]`

> Pero hay algo que no muestra.

`[VOZ FIRME]` `[PAUSA 2s]`

> `[MIRADA JURADO]` Dónde vive cada pieza dentro del sistema. Qué se conecta con qué. Qué queda oculto debajo de otra parte. Y en qué orden se ensambla todo.

Esa idea no se queda en una anécdota personal. Durante el desarrollo, la tesis fue acercándose a una lógica de herramienta: si una tarea obliga a repetir, buscar, comparar o recordar demasiado, el sistema debe ayudar a reducir esa fricción. Por eso el proyecto terminó incorporando selección, aislamiento, guía de ensamble, puntos de conexión, medición, BOM y checklist como soporte técnico y no como adornos.

La referencia a instrucciones 3D interactivas de otras plataformas industriales queda aquí como inspiración implícita: guías más narrativas, paso a paso, para hardware complejo. La diferencia es que este proyecto no busca una guía de consumo general, sino una capa técnica aplicada al dron y a otros sistemas donde la lectura espacial y la jerarquía de piezas son críticas.

`[PAUSA 1s]`

> Entender un ensamblaje no es solo mirar componentes.

`[VOZ FIRME]` `[ÉNFASIS]`

> Es entender relaciones.

`[PAUSA 2s]` `[MIRADA JURADO]`

> Esa distancia —entre ver una pieza y comprender un sistema— es el punto de partida de esta tesis.

### Acción siguiente

- Hacer una pausa clara.
- Avanzar a siguiente diapositiva sin prisa.
- Volver al centro del espacio.

### Conexión con tesis

Este bloque establece el **problema observado**: el usuario debe completar mentalmente el 3D desde información 2D. Eso es fricción cognitiva. La tesis propone resolver eso.

---

# MINUTOS 0:45-1:30 — SALUDO Y ENCUADRE

## Objetivo de Sección

Formalizar la presentación, nombrar el caso de estudio, señalar el tema sin jerga.

---

### Mostrar en pantalla

**Diapositiva 2:** Portada formal del proyecto.

- Título oficial
- Nombre del autor
- Institución
- Fecha

### Decir

> `[VOZ FIRME]` Buenos días.

`[PAUSA 0.5s]`

> Mi nombre es Alexander Woodcock Salomón.

`[PAUSA 1s]`

> Presento mi proyecto de grado: un visor web 3D interactivo para el dron Holybro X500 V2.

`[PAUSA 1s]`

> `[GESTO ABIERTO]` La pregunta de fondo es sencilla:

`[VOZ CONVERSACIONAL]`

> ¿Cómo pasamos de ver piezas sueltas a entender cómo funcionan juntas?

`[PAUSA 1.5s]` `[MIRADA JURADO]`

### Notas para ensayo

- No leer el título completo de la diapositiva. Hablar con palabras propias.
- La pregunta "¿cómo pasamos..." es deliberadamente sencilla. No mete jerga.
- Volumen normal, no bajo.

### Conexión con tesis

Encuadra el proyecto como respuesta a una pregunta específica, no como lista de features.

---

# MINUTOS 1:30-2:15 — DEL MANUAL AL VISOR: EL CONTRASTE

## Objetivo de Sección

Mostrar visualmente la diferencia entre documentación estática (manual) y experiencia interactiva (visor).

---

### Mostrar en pantalla

**Diapositiva 3 (izquierda):** Manual 2D fragmentado (multiple vistas).  
**Diapositiva 3 (derecha):** Screenshot del visor 3D (modelo navegable, panel inferior con ficha).

- La transición debe ser clara: manual estático → visor interactivo.

### Decir

> En documentación técnica, las piezas aparecen separadas, numeradas, vistas desde ángulos fijos.

`[PAUSA 0.5s]`

> `[MIRADA JURADO]` Esa documentación es necesaria.

`[VOZ CONVERSACIONAL]`

> No es el enemigo.

`[PAUSA 1s]`

> El límite aparece cuando el usuario tiene que completar mentalmente lo que no ve: la profundidad, la jerarquía, dónde se oculta cada cosa.

`[PAUSA 1s]`

> `[AVANZAR A VISOR SCREENSHOT]`

> Este proyecto explora una respuesta:

`[VOZ FIRME]`

> Convertir ese ensamblaje en una experiencia web 3D donde el usuario puede orbitar, seleccionar piezas, aislar grupos y cambiar cómo las ve.

`[PAUSA 1s]`

> El manual muestra piezas.

`[ÉNFASIS]`

> El visor ayuda a leer relaciones.

`[PAUSA 2s]` `[MIRADA JURADO]`

### Acción siguiente

Pausa significativa. Luego transición clara: "Para convertir esa intuición en tesis..."

### Conexión con tesis

Contraste visual directo: antes (manual 2D, fragmentado) → después (visor 3D, integrado). Justifica por qué la solución importa.

---

# MINUTOS 2:15-4:30 — PROBLEMA, PREGUNTA Y OBJETIVOS

## Objetivo de Sección

Definir con precisión qué se investigó. Establecer que hay un problema legítimo que requería estudio.

---

### Mostrar en pantalla

**Diapositiva 4:** Diagrama con tres capas:

- Capa 1 (humana): ícono de cerebro + "Esfuerzo cognitivo alto"
- Capa 2 (técnica): ícono de código/caja + "CAD pesado no es web-compatible"
- Capa 3 (metodológica): ícono de checksuma + "Requiere evaluación rigurosa"

Fuente: INF_EST_60 "Columna Vertebral" - Bloque "Problema".

### Decir

> El problema tiene tres capas simultáneamente.

`[PAUSA 1s]`

> `[SEÑALAR CAPA 1]` Primera: humana.

> Entender hardware complejo desde documentación plana exige recordar, comparar, rotar mentalmente lo que no se ve.

> Eso cuesta esfuerzo cognitivo. Mucho.

`[PAUSA 0.5s]`

> `[SEÑALAR CAPA 2]` Segunda: técnica.

> Un modelo CAD está optimizado para manufactura, no para ejecutarse en tiempo real en un navegador.

> Pesa demasiado. Carga lentamente. Consume memoria.

`[PAUSA 0.5s]`

> `[SEÑALAR CAPA 3]` Tercera: metodológica.

> No basta con construir el visor. Hay que evaluarlo con criterios claros.

> ¿Funciona? ¿Es usable? ¿Genera diferencia real para el usuario?

`[PAUSA 1.5s]` `[MIRADA JURADO]`

> Por eso la pregunta central fue:

`[VOZ FIRME]`

> **"¿Cómo diseñar y evaluar un visor web 3D que ayude a inspeccionar un ensamblaje complejo, sin perder viabilidad técnica en navegador?"**

`[PAUSA 2s]`

> Los objetivos responden a esa pregunta.

`[AVANZAR]`

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 5:** Cuatro objetivos específicos:

1. Diseñar pipeline de optimización de activos 3D
2. Integrar en Unity un sistema de materiales y modos visuales
3. Implementar prototipo interactivo en web
4. Evaluar formalmente con usuarios

### Decir (continuación)

> No son objetivos genéricos.

> Cada uno responde a una restricción del problema:

> `[SEÑALAR]` Objetivo 1: resolver la restricción técnica (peso de CAD).  
> Objetivo 2: resolver la restricción visual (materiales y modos analíticos).  
> Objetivo 3: resolver la restricción de acceso (web, no desktop especializado).  
> Objetivo 4: resolver la incertidumbre (¿funciona?).

`[PAUSA 1s]`

> La ruta es clara: **entender → transformar → implementar → evaluar**.

`[PAUSA 2s]`

> Para eso necesitaba fundamento teórico. No partimos de intuición.

### Conexión con tesis

Establece claramente qué se investigó y por qué. Elimina la duda de que es un proyecto "artístico" o "sin criterio".

---

# MINUTOS 4:30-6:30 — FUNDAMENTO TEÓRICO: COGNICIÓN

## Objetivo de Sección

Explicar por qué 3D es cognitivamente mejor que 2D, sin jerga excesiva.

---

### Mostrar en pantalla

**Diapositiva 6:** Comparación visual:

- Lado izquierdo: icono de cabeza + flecha multiplicada = "2D: alta carga extrínseca"
- Lado derecho: icono de pantalla interactiva + flecha simple = "3D: carga reducida, redirigida a comprensión"

No mostrar fórmulas aún. Solo visuales.

### Decir

> La base teórica viene de psicología cognitiva.

`[VOZ CONVERSACIONAL]`

> Nuestro cerebro tiene una limitación fundamental:

> La memoria de trabajo puede sostener solo unos pocos elementos simultáneamente.

`[PAUSA 1s]`

> Si el usuario está imaginando "¿cómo se ve esto en 3D?" está gastando recursos cognitivos en una tarea que no le suma comprensión del dron.

> Eso es **carga extrínseca**: el esfuerzo innecesario.

`[PAUSA 1s]`

> En una visualización 3D interactiva, parte de ese trabajo pasa a la interfaz.

> El usuario ya no solo imagina. Rota, aísla, inspecciona directamente.

> Los recursos liberados se pueden redirigir a la pregunta que importa:

`[VOZ FIRME]` `[ÉNFASIS]`

> "¿Cómo se relacionan estas piezas?"

`[PAUSA 1.5s]`

> Eso es **carga germana**: el esfuerzo que sí suma.

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 7:** Fórmula simple de cognición:

```
Carga Total = Intrínseca + Extrínseca + Germana

En 2D estático:  Intrínseca (alta) + Extrínseca (muy alta) = Saturación
En 3D interactivo: Intrínseca (igual) + Extrínseca (baja) + Germana (alto) = Comprensión
```

### Decir (continuación)

> La teoría es así:

> La complejidad del dron (intrínseca) no cambia. El dron es igualmente complejo en 2D o 3D.

> Pero cómo presentamos esa información sí cambia cómo la procesa el usuario.

`[PAUSA 1s]`

> Este proyecto está fundado en que una interfaz 3D bien diseñada **reduce fricción sin sacrificar rigor técnico**.

`[PAUSA 2s]` `[MIRADA JURADO]`

### Conexión con tesis

Proporciona justificación teórica para por qué construimos 3D. Anticiparía la evaluación con NASA-TLX (que mide carga de trabajo percibida, proxy de extrínseca + germana).

---

# MINUTOS 6:30-8:30 — RENDIMIENTO Y RESTRICCIONES WEB

## Objetivo de Sección

Explicar que optimizar para web no es vanidad, es una restricción funcional.

---

### Mostrar en pantalla

**Diapositiva 8:** Diagrama simple:

- Entrada: Modelo CAD (icono de archivo pesado, "100+ MB")
- Salida: Build web (icono de navegador, "¿ MB?")
- Flecha con obstáculos: "Frame time, memoria, draw calls"

### Decir

> Para que 3D funcione en navegador, controlamos presupuestos.

> No es porque nos guste optimizar. Es porque el navegador tiene límites que un desktop no tiene.

`[PAUSA 1s]`

> `[SEÑALAR]` Geometría: cada triángulo del modelo cuesta cálculos.

> Materiales: cada textura ocupa memoria.

> Draw calls: cada orden de dibujo a la GPU suma latencia.

`[PAUSA 1s]`

> La meta operativa es **30 FPS**: treinta cuadros por segundo.

> Eso equivale a 33.33 milisegundos por cuadro.

> Si tardamos más, el usuario ve tartamudeo. La experiencia se quiebra.

`[PAUSA 1s]`

> Por eso cada decisión del pipeline —desde dónde importamos el CAD hasta cuántas instancias renderizamos— tiene consecuencia directa en si la app es fluida o no.

`[VOZ FIRME]`

> No es detalle técnico. Es requisito funcional.

`[PAUSA 2s]`

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 9:** Tabla de presupuestos objetivo:
| Métrica | Meta |
|---------|------|
| Frame rate | > 30 FPS |
| Frame time | < 33.33 ms |
| Build size | ~100-150 MB |
| Carga inicial | < 30 s |

### Conexión con tesis

Justifica por qué la arquitectura no es "over-engineered". Cada decisión responde a una restricción real.

---

# MINUTOS 8:30-11:30 — METODOLOGÍA

## Objetivo de Sección

Establecer que el proyecto fue estudiado con rigor académico. Definir qué se midió y cómo.

**NOTA IMPORTANTE:** Este es el bloque más denso. La auditoría v2 (INF_EST_53) lo identificó como problemático. Lo hemos restructurado para fluidez oral.

---

### Mostrar en pantalla

**Diapositiva 10:** Título-sentencia:

> "Usé Design Science Research: construir un artefacto + evaluarlo formalmente"

### Decir

> El enfoque metodológico es **Design Science Research**, o DSR.

`[VOZ CONVERSACIONAL]`

> DSR es un paradigma académico para cuando **construyes algo que resuelve un problema práctico** y lo evalúas con rigor.

`[PAUSA 1s]`

> La ruta de Peffers es: problema → objetivo → diseño → demostración → evaluación → comunicación.

`[PAUSA 0.5s]`

> Eso es exactamente lo que hicimos.

> Identificamos el problema (comprensión de hardware complejo). Definimos el objetivo (visor web 3D). Diseñamos y demostramos el prototipo. Y ahora lo evaluamos.

`[PAUSA 1.5s]`

> La evaluación tiene tres partes.

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 11:** Tres pilares de evaluación:

- Pilar 1: Técnica (¿funciona con fluidez?)
- Pilar 2: Usabilidad (¿puede usarlo alguien?)
- Pilar 3: Experiencia (¿genera diferencia vs alternativa?)

### Decir (continuación)

> `[SEÑALAR PILAR 1]` Primero: evaluación técnica.

> Medimos FPS, frame time, memoria, tiempo de carga en una build congelada.

> Las herramientas: Unity Profiler y Chrome DevTools.

> Esto responde: "¿El prototipo es viable técnicamente?"

`[PAUSA 1s]`

> `[SEÑALAR PILAR 2]` Segundo: usabilidad.

> Usamos **SUS**, System Usability Scale: un cuestionario de 10 ítems sobre si el sistema es usable.

> Responde: "¿Una persona puede usar esto sin frustración?"

`[PAUSA 1s]`

> `[SEÑALAR PILAR 3]` Tercero: experiencia.

> Comparamos dos condiciones: usuario con visor 3D vs usuario con referencia 2D.

> Medimos con **NASA-TLX Raw**: carga de trabajo percibida en seis dimensiones (demanda mental, demanda física, demanda temporal, esfuerzo, frustración, desempeño).

> Y recogimos **Think-Aloud**: el usuario verbaliza qué piensa mientras realiza tareas.

> Esto responde: "¿El visor reduce esfuerzo percibido? ¿Qué dice el usuario?"

`[PAUSA 2s]`

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 12:** Resumen metodológico:
| Aspecto | Instrumento | Participantes | Tareas |
|---------|------------|---------------|--------|
| Técnico | Profiling | N/A | Múltiples escenas |
| Usabilidad | SUS | n | Post-uso |
| Carga/Experiencia | NASA-TLX + Think-Aloud | n | 3-5 tareas comparativas |

### Decir (continuación)

> La muestra es de n participantes

> (Meta institucional: 30. Mínimo operativo: 8-12.)

> Esto es evaluación **descriptiva y formativa**, no experimental con poder estadístico.

> ¿Por qué no experimental? Porque el objetivo es verificar el artefacto, no hacer una comparación estadística de población.

`[PAUSA 1.5s]`

> `[MIRADA JURADO]` En síntesis:

`[VOZ FIRME]`

> El proyecto es **riguroso porque fue evaluado según criterios claros, con instrumentos validados, documentado y replicable**.

`[PAUSA 2s]`

### Notas para ensayo

- NO enumerar Peffers y Hevner como listas. Integrar como narrativa.
- NO mostrar fórmulas SUS o NASA-TLX como "bloques de código". Mostrarlas como tablas que se pueden interpretar.
- El tono debe ser: "Esto es académicamente serio, no over-complicated".

### Conexión con tesis

Anticipa a jurados que pueden no estar familiarizados con DSR. Establece credibilidad académica.

---

# MINUTOS 11:30-13:30 — PIPELINE: DE CAD A WEB

## Objetivo de Sección

Mostrar el camino técnico concreto: qué se hizo en cada herramienta, por qué en ese orden.

---

### Mostrar en pantalla

**Diapositiva 13:** Pipeline visual (diagrama flujo):

```
CAD → Blender → Optimización → Baking → Unity → WebGL Build
      ↓           ↓              ↓        ↓        ↓
   (origen)  (limpieza)    (reducción) (materiales) (web)
```

### Decir

> Convertir CAD en un visor web viable requiere una cadena de pasos.

> Cada paso tiene razón de ser. No se puede saltear.

`[PAUSA 1s]`

> `[SEÑALAR paso 1]` Paso 1: CAD original.

> Viene de la fuente técnica: planos, especificaciones, ensamble completo.

> Pesa mucho. Tiene geometría para manufactura, no para tiempo real.

`[PAUSA 0.5s]`

> `[SEÑALAR paso 2]` Paso 2: Limpieza en Blender.

> Removemos geometría innecesaria: superficies ocultas, detalles de manufactura que no importan visualmente.

> Retopología: creamos una malla nueva, más simple, que mantiene la forma reconocible pero reduce triángulos.

`[PAUSA 0.5s]`

> `[SEÑALAR paso 3]` Paso 3: Optimización.

> Agrupamos piezas, identificamos instancias (tornillos repetidos, por ejemplo), preparamos materiales.

`[PAUSA 0.5s]`

> `[SEÑALAR paso 4]` Paso 4: Baking de texturas.

> En lugar de complicados materiales procedurales, "horneamos" la información visual en texturas estáticas.

> Color, normales, metálico, rugosidad: todo empaquetado en un atlas.

> Resultado: visual coherente, overhead bajo.

`[PAUSA 0.5s]`

> `[SEÑALAR paso 5]` Paso 5: Importación en Unity.

> Traemos el FBX optimizado. Configuramos materiales, jerarquía, instancias, la lógica de selección.

`[PAUSA 0.5s]`

> `[SEÑALAR paso 6]` Paso 6: Build web.

> Unity compila el proyecto a WebAssembly. Resultado: un archivo index.html que abre en cualquier navegador.

`[PAUSA 1.5s]`

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 14:** Comparativa pre/post (visual):

- Izquierda: Modelo CAD original (X MB, Y millones de triángulos, complejo)
- Derecha: Modelo optimizado web (X' MB, Y' triángulos, limpio, legible)

### Decir (continuación)

> El resultado neto:

`[VOZ FIRME]`

> Reducción de X a X' MB. Reducción de Y a Y' triángulos.

> Pero la **legibilidad técnica se preserva**: el usuario sigue viendo un dron reconocible, con colores correctos, con relaciones espaciales intactas.

`[PAUSA 1s]`

> El reto no era abrir un archivo.

`[ÉNFASIS]`

> Era traducir ingeniería a experiencia interactiva.

`[PAUSA 2s]` `[MIRADA JURADO]`

### Conexión con tesis

Demuestra que la optimización no es superficial. Cada paso tiene propósito y consecuencia.

---

# MINUTOS 13:30-15:00 — MATERIALES Y VISUALIZACIÓN

## Objetivo de Sección

Mostrar cómo se ve el resultado técnico. Explicar por qué los materiales importan.

---

### Mostrar en pantalla

**Diapositiva 15:** Grid de cuatro visuales del modelo:

1. Blender (con detalle de malla)
2. Atlas de texturas (4K: color, normales, máscara)
3. Modelo en Unity (preview)
4. Modelo en navegador (web final)

### Decir

> Los materiales no son decoración.

> Un sistema PBR —Physically Based Rendering— permite que el modelo comunique cómo está hecho:

> Fibra de carbono se ve diferente a plástico, que se ve diferente a metal.

`[PAUSA 1s]`

> El atlas es una estrategia de eficiencia:

> En lugar de múltiples texturas pequeñas por pieza, empaquetamos toda la información visual en un archivo grande (4K) que se carga una sola vez.

> Resultado: visual coherente, memoria controlada, setup del shader simple.

`[PAUSA 1s]`

> Decidimos 4K como resolución porque es el mínimo donde los detalles técnicos siguen siendo legibles: conectores, juntas, acabados.

`[PAUSA 1s]`

> El color también comunica:

> Negro y gris para estructuras de carbono. Cobre y dorado para circuitos. Rojo y amarillo para cables.

> Eso no es estética arbitraria.

`[VOZ FIRME]`

> Es legibilidad técnica: el usuario lee el dron "correctamente".

`[PAUSA 2s]`

### Conexión con tesis

Cierra la primera mitad del guión mostrando que el resultado es un balance entre visual coherente e implementación web viable.

---

## TRANSICIÓN A SEGUNDA MITAD

`[PAUSA 2s]` `[MIRADA JURADO]`

> Hasta aquí mostré el fundamento: problema, teoría, pipeline, cómo se ve.

> Ahora paso a cómo **usa** el usuario el visor.

> Porque un modelo bonito sin interacción es solo un display pasivo.

> Lo que construimos es una **herramienta interactiva**.

`[PAUSA 1s]`

> La interacción se organiza en tres módulos.

`[AVANZAR]`

---

**[CONTINÚA EN PARTE 2: 15:00-30:00]**
