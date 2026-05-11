---
tipo: guion_sustentacion
fuente: INF_EST_54-55 (base) + Deep Research Report (principios)
estado: activo
area: sustentacion
version: 70
parte: 2
duracion_objetivo: 15min
tags:
  - tesis
  - guion
  - sustentacion
  - final
  - defensa-oral
---

# INF_EST_70 — Guión Final de Sustentación (Parte 2: 15:00-30:00)

---

# MINUTOS 15:00-18:30 — LOS TRES MÓDULOS FUNCIONALES

## Objetivo de Sección

Mostrar cómo interactúa el usuario con el visor. Cada módulo responde a una pregunta diferente.

---

### Mostrar en pantalla

**Diapositiva 16:** UI mobile-first: barra inferior con tres íconos.

- Icono 1: **Inspect** (lupa)
- Icono 2: **Analyze** (líneas)
- Icono 3: **Studio** (paleta)

Captura real del visor si es posible.

### Decir

> La interfaz se diseñó mobile-first: pantalla pequeña, espacio limitado, acciones claras.

`[PAUSA 0.5s]`

> Eso obligó a priorizar. Y de la prioridad nació la organización en tres módulos.

`[PAUSA 1s]`

> Cada módulo responde a una pregunta que el usuario podría hacer mientras inspecciona el dron.

---

## MÓDULO 1: INSPECT

### Mostrar en pantalla

**Diapositiva 17:** Screenshot de Inspect:

- Modelo 3D con pieza seleccionada (destacada)
- Panel inferior ("bottom sheet") con ficha: nombre, categoría, especificaciones, pieza madre, fasteners asociados
- Hotspots visibles sobre el modelo

### Decir

> `[SEÑALAR MÓDULO]` **Inspect:** "¿Qué estoy mirando?"

`[VOZ CONVERSACIONAL]`

> El usuario selecciona una pieza sobre el modelo. Instantáneamente aparece una ficha con información:

> Nombre, categoría, grupo funcional, pieza madre (si existe), especificaciones técnicas.

`[PAUSA 1s]`

> Pero Inspect hace más que mostrar datos.

> El usuario también puede **aislar** esa pieza. Cuando aísla, todo lo demás desaparece o se oculta.

> La pieza queda en contexto limpio, con sus fasteners asociados visibles.

`[PAUSA 1s]`

> Y los **hotspots** —marcadores visuales sobre el modelo— guían al usuario sin que tenga que adivinar.

> Funciona como un mapa: "mira aquí, hay algo interesante".

`[PAUSA 1s]`

> **Función de Inspect:** Identificación y contexto técnico sin distracciones.

---

## MÓDULO 2: ANALYZE

### Mostrar en pantalla

**Diapositiva 18:** Grid 2x2 de screenshots:

- Arriba izquierda: Vista explotada (piezas separadas a lo largo de eje)
- Arriba derecha: Corte transversal (plano que divide el modelo)
- Abajo izquierda: Filtro por categoría (solo motores, por ejemplo)
- Abajo derecha: Modelo sin filtro

### Decir

> `[SEÑALAR MÓDULO]` **Analyze:** "¿Cómo se relacionan las piezas?"

> Aquí el usuario deja de mirar piezas aisladas y pasa a **descomponer** el ensamblaje.

`[PAUSA 1s]`

> `[SEÑALAR screenshot 1]` La **vista explotada** separa las piezas a lo largo de un eje geométrico.

> Muestra la secuencia de ensamblaje y revela relaciones de posición que en reposo quedan ocultas.

> Por ejemplo: ver qué se conecta directamente a la placa central, qué viene después.

`[PAUSA 1s]`

> `[SEÑALAR screenshot 2]` El **corte transversal** es un plano matemático que divide visualmente el modelo.

> Lo que queda del lado del observador se ve. Lo del otro lado desaparece.

> Permite inspeccionar el interior sin desmontar nada.

> Es como una radiografía.

`[PAUSA 1s]`

> `[SEÑALAR screenshot 3]` Los **filtros por categoría** permiten ver solo un grupo funcional.

> Por ejemplo: "muéstrame solo los motores". El resto desaparece. Eso reduce ruido visual y ayuda a enfocarse.

`[PAUSA 1s]`

> **Función de Analyze:** Descomposición visual y relaciones entre subsistemas.

---

## MÓDULO 3: STUDIO

### Mostrar en pantalla

**Diapositiva 19:** Grid 3x3 de cinco modos + tres entornos:

**Modos visuales:**

- Realistic (colores normales, materiales)
- X-Ray (transparencia, relaciones internas)
- Solid Color (sin texturas, solo forma y contraste)
- Thermal (gradiente de colores, tendencias de calor)
- Blueprint (líneas, contornos, lectura de plano técnico)

**Entornos:**

- Day (luz neutral, fondo claro)
- Sunset (luz cálida, fondo naranja)
- Blueprint (fondo oscuro, grilla, líneas técnicas)

### Decir

> `[SEÑALAR MÓDULO]` **Studio:** "¿Cómo veo esto?"

> Aquí el usuario controla **cómo se visualiza** el modelo y en qué **contexto de luz** se ve.

`[PAUSA 1s]`

> `[SEÑALAR MODOS]` Siete modos visuales:

> **Realistic:** modo por defecto. Colores y materiales que reflejan cómo se ve el dron físicamente.

`[PAUSA 0.5s]`

> **X-Ray:** el modelo se vuelve semitransparente. Permite ver interior y relaciones de componentes ocultos.

`[PAUSA 0.5s]`

> **Solid Color:** sin texturas, sin reflexiones. Solo forma y contraste. Útil para ver geometría pura sin distracción visual.

`[PAUSA 0.5s]`

> **Wireframe:** malla y aristas visibles para lectura estructural.

`[PAUSA 0.5s]`

> **Ghosted:** vista translúcida para mantener contexto y profundidad.

`[PAUSA 0.5s]`

> **Thermal:** modo analítico que muestra tendencias de calentamiento por componente. Rojo = más calor, azul = más frío.

`[VOZ FIRME]`

> Importante: no es simulación FEA. Es una lupa visual de tendencias. Para análisis térmico exacto, necesitarías herramientas de ingeniería especializadas.

`[PAUSA 1s]`

> **Blueprint:** lectura de plano técnico. Líneas de contorno, grilla de fondo, contraste maximalizado. Evoca un plano técnico tradicional, pero interactivo.

`[PAUSA 0.5s]`

> **Wireframe y Ghosted:** dos modos complementarios para lectura técnica cuando interesa priorizar estructura, profundidad o capas de visualización por encima del color realista.

`[PAUSA 1.5s]`

> `[SEÑALAR ENTORNOS]` Los entornos cambian la iluminación y el fondo.

> Day, Sunset, Night, Studio Light: cada uno comunica un contexto diferente.

`[PAUSA 1s]`

> **Función de Studio:** Control visual y contexto de presentación. El usuario elige cómo leer el modelo.

---

### Transición

`[PAUSA 1.5s]` `[MIRADA JURADO]`

> Los tres módulos —Inspect, Analyze, Studio— no son features aleatorios.

> Cada uno responde a una necesidad cognitiva del usuario:

> Primero, identifica (Inspect). Luego, descompone (Analyze). Finalmente, visualiza como necesita (Studio).

`[PAUSA 1.5s]`

> Para que todo esto funcione, existe una arquitectura detrás.

---

# MINUTOS 18:30-19:30 — ARQUITECTURA Y TOOLING

## Objetivo de Sección

Mostrar que la app no fue improvisada. Hay estructura, control de calidad, verificación.

---

### Mostrar en pantalla

**Diapositiva 20:** Diagrama simplificado de arquitectura (4-5 capas):

```
USER INTERFACE (UI Toolkit, Mobile-first)
     ↓
INTERACTION LAYER (Selection, Camera, Isolate, Hotspots)
     ↓
DATA LAYER (Parts, Metadata, Categories, Fasteners)
     ↓
VISUAL LAYER (Shaders, Materials, Environments)
     ↓
RUNTIME (Unity WebGL + WebAssembly)
```

### Decir

> La app no es una escena armada a mano y listopara la web.

> Tiene una arquitectura donde varios controladores trabajan en coordinación:

`[PAUSA 1s]`

> `[SEÑALAR]` **Capa de UI:** interfaz mobile-first, bottom sheet, barra inferior.

> **Capa de interacción:** selección de piezas, movimiento de cámara, aislamiento, hotspots.

> **Capa de datos:** información de cada pieza, jerarquía padre-hijo, fasteners asociados, metadatos técnicos.

> **Capa visual:** shaders que cambian en runtime, modos analíticos, ambientes.

> **Runtime:** la máquina virtual que ejecuta todo esto en WebAssembly dentro del navegador.

`[PAUSA 1s]`

> Además de arquitectura, hay **herramientas de editor** que verifican la integridad del proyecto:

> Auditorías de cobertura (¿todas las piezas están representadas?), validación de datos (¿los metadatos son consistentes?), recuento de fasteners.

`[VOZ FIRME]`

> No solo importé un modelo. Construí una cadena para comprobar datos, selección y coherencia.

> Y además dejé integradas herramientas de soporte técnico que responden a la misma lógica de fricción mínima: AssemblyGuide, ConnectionPointsViewer, BillOfMaterials, AnnotationSystem, MeasurementTool y AssemblyChecklist.

`[PAUSA 1.5s]`

---

# MINUTOS 19:30-21:00 — CORTE TRANSVERSAL: TÉCNICA VISUAL

## Objetivo de Sección

Explicar una herramienta técnica específica (corte) sin perder la audiencia.

---

### Mostrar en pantalla

**Diapositiva 21:** Modo corte activo:

- Izquierda: modelo sin corte (completo)
- Derecha: modelo con corte (plano visible, parte del otro lado desaparece)

### Decir

> Ya mencioné el corte. Ahora explico brevemente cómo funciona técnicamente.

> Porque si entienden esto, entienden el nivel de control que tenemos sobre visualización.

`[PAUSA 1s]`

> Un plano matemático cruza la escena.

> Para cada fragmento del modelo (cada triángulo pequeño), el shader calcula: ¿este fragmento está del lado de la cámara o del otro?

`[AVANZAR]`

### Mostrar en pantalla

**Diapositiva 22:** Fórmula:

```
distancia = dot(posición_fragmento - punto_plano, normal_plano)

Si distancia < 0: fragmento se oculta
Si distancia >= 0: fragmento se dibuja
```

### Decir (continuación)

> La fórmula es `dot product`: multiplicar componentes de vectores.

> En palabras simples: medimos a qué "lado del plano" está cada fragmento.

> Si está del lado a descartar, no lo dibujamos.

> Si está del lado a mostrar, lo dibujamos normalmente.

`[PAUSA 1s]`

> Resultado: **visualización dinámica sin destruir geometría**.

> El modelo original queda intacto. Solo decidimos qué parte se ve en cada frame.

`[PAUSA 1.5s]`

> `[MIRADA JURADO]` Esto demuestra que incluso herramientas analíticas avanzadas se basan en matemática directa.

> No es magia. Es programación de gráficos con propósito técnico.

---

# MINUTOS 21:00-22:30 — MODO TÉRMICO: VISUALIZACIÓN HEURÍSTICA

## Objetivo de Sección

Explicar qué es el thermal, qué NO es, y por qué se incluye.

**NOTA CRÍTICA:** Este bloque es donde más preguntas escépticas pueden venir. Prepararse para defender honestidad metodológica.

---

### Mostrar en pantalla

**Diapositiva 23:** Vista Thermal del modelo:

- Gradiente de colores (rojo = caliente, azul = frío, verde = neutral)
- Leyenda visible
- Aclaración visual: "Heurístico, no FEA"

### Decir

> `[MIRADA JURADO]` Ahora debo hacer una aclaración importante.

> El modo **Thermal** NO es análisis por elementos finitos.

`[VOZ FIRME]`

> No es simulación física exacta.

> Es una **lupa térmica heurística** por componentes.

`[PAUSA 1s]`

> ¿Qué significa?

> Significa que muestra tendencias visuales basadas en un modelo simplificado:

> "Si la pieza A genera calor y la pieza B está cerca, B se calentará progresivamente."

> "Si hay carga en el motor, las zonas alrededor se calienta más."

`[PAUSA 1s]`

> Es útil para:
>
> - Inspección educativa
> - Orientación visual rápida
> - Apoyo a comprensión del comportamiento esperado del sistema

> NO es útil para:
>
> - Decisiones de diseño térmico críticas
> - Predicción exacta de temperaturas
> - Cumplimiento de estándares térmicos

`[VOZ CONVERSACIONAL]`

> Piénsenlo así: es como usar colores en un mapa climático para entender tendencias regionales, no como un termómetro milimétrico.

`[PAUSA 1.5s]`

> `[VOZ FIRME]` La incluyo porque honestidad metodológica es más importante que impresionar.

> Mejor decir "esto es una herramienta didáctica" que pretender que es rigor termodinámico.

`[PAUSA 2s]`

---

# MINUTOS 22:30-25:00 — RESULTADOS Y EVALUACIÓN

## Objetivo de Sección

Presentar datos. No como números flotantes, sino como respuestas a preguntas.

---

### Mostrar en pantalla

**Diapositiva 24:** Tabla KPIs técnicos

| KPI        | Meta        | Observado        | Estado     |
| ---------- | ----------- | ---------------- | ---------- |
| FPS        | > 30        | [FPS_PROMEDIO]   | ✓ / ⚠️ / ✗ |
| Frame time | < 33.33 ms  | [FRAME_TIME] ms  | ✓ / ⚠️ / ✗ |
| Build size | ~100-150 MB | [PESO_BUILD] MB  | ✓ / ⚠️ / ✗ |
| Load time  | < 30 s      | [TIEMPO_CARGA] s | ✓ / ⚠️ / ✗ |

### Decir

> Los resultados responden a la pregunta: "¿Funciona?"

`[PAUSA 1s]`

> Técnicamente, medimos cuatro indicadores:

> `[SEÑALAR]` **Frame rate:** ¿cuántos cuadros por segundo?

> Meta: más de 30 FPS. Observado: [FPS_PROMEDIO] FPS.

> `[PAUSA 0.5s]`

> **Frame time:** ¿cuántos milisegundos por cuadro?

> Meta: menos de 33.33 ms. Observado: [FRAME_TIME] ms.

> `[PAUSA 0.5s]`

> **Build size:** ¿cuánto pesa el archivo que descarga?

> Meta: 100-150 MB. Observado: [PESO_BUILD] MB.

> `[PAUSA 0.5s]`

> **Load time:** ¿cuánto tarda en cargar?

> Meta: menos de 30 segundos. Observado: [TIEMPO_CARGA] segundos.

`[PAUSA 1.5s]`

> `[VOZ FIRME]` Conclusión técnica:

> El prototipo es **viable en navegador bajo las condiciones evaluadas**.

> Cumple metas de desempeño en dispositivo de referencia, navegador de referencia.

---

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 25:** Usabilidad (SUS)

**System Usability Scale (SUS):**

- Rango: 0-100
- Benchmark histórico: 68 = promedio
- Umbral deseable: 72+
- Resultado: [SUS_PROMEDIO]
- Interpretación: [Bajo/Aceptable/Bueno/Excelente]

### Decir (continuación)

> Segundo aspecto: usabilidad.

> Usamos **SUS**, System Usability Scale: diez preguntas que el usuario responde después de usar el sistema.

> Las preguntas exploran: ¿fue fácil? ¿necesitabas aprender mucho? ¿te gustaría volver a usarlo?

`[PAUSA 1s]`

> El resultado de [SUS_PROMEDIO] sugiere que el sistema es [Bajo/Aceptable/Bueno/Excelente].

> Comparado con benchmark: 68 es promedio. 72 es deseable. [Nuestro valor] es [análisis].

`[PAUSA 1.5s]`

---

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 26:** Carga percibida (NASA-TLX Raw)

**Condición:** 3D vs 2D

| Dimensión              | 2D               | 3D               | Diferencia         |
| ---------------------- | ---------------- | ---------------- | ------------------ |
| Demanda mental         | [2D_mental]      | [3D_mental]      | [DIFF]             |
| Demanda física         | [2D_física]      | [3D_física]      | [DIFF]             |
| Demanda temporal       | [2D_temporal]    | [3D_temporal]    | [DIFF]             |
| Esfuerzo               | [2D_esfuerzo]    | [3D_esfuerzo]    | [DIFF]             |
| Frustración            | [2D_frustración] | [3D_frustración] | [DIFF]             |
| Desempeño              | [2D_desempeño]   | [3D_desempeño]   | [DIFF]             |
|                        |                  |                  |                    |
| **Raw TLX (promedio)** | [TLX_2D]         | [TLX_3D]         | [DIFERENCIA_TOTAL] |

### Decir (continuación)

> Tercer aspecto: esfuerzo percibido.

> Comparamos dos condiciones: el usuario realiza tareas idénticas con el visor 3D y luego con referencia 2D.

> Después de cada condición, responde **NASA-TLX Raw**: escala de carga de trabajo en seis dimensiones.

`[PAUSA 1s]`

> Los resultados:

> Demanda mental, física, temporal, esfuerzo, frustración, desempeño.

> En 2D: promedio de [TLX_2D].

> En 3D: promedio de [TLX_3D].

> Diferencia: [DIFERENCIA_TOTAL] puntos.

`[PAUSA 1s]`

> `[MIRADA JURADO]` Interpretación:

> Si 3D < 2D: el visor reduce carga percibida. La hipótesis se sostiene.

> Si 3D ≈ 2D: el visor es equivalente. Entonces ¿por qué usarlo? (Pero depende de otros factores.)

> Si 3D > 2D: inesperado, exigiría análisis de por qué.

`[PAUSA 1.5s]`

---

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 27:** Experiencia cualitativa (Think-Aloud)

**Categorías emergentes (protocolo Think-Aloud):**

- Categoría 1: [NOMBRE] (n menciones)
- Categoría 2: [NOMBRE] (n menciones)
- Categoría 3: [NOMBRE] (n menciones)
- [otros...]

### Decir (continuación)

> Cuarto aspecto: lo que el usuario dijo.

> Mientras realizaba las tareas, los usuarios verbalizaban lo que pensaban: "Ah, esta pieza se conecta aquí", "No veo dónde va este tornillo", "Con el corte ahora entiendo".

> Grabamos esas verbalizaciones y las categorizamos.

> Las categorías más frecuentes fueron:

> `[SEÑALAR]` [CATEGORÍA_1]: [DESCRIPCIÓN Y RELEVANCIA]

> [CATEGORÍA_2]: [DESCRIPCIÓN Y RELEVANCIA]

> [CATEGORÍA_3]: [DESCRIPCIÓN Y RELEVANCIA]

> `[PAUSA 1s]`

> Esto demuestra que el visor no solo **funciona técnicamente** y **es usable**, sino que genera **comprensión observable** en el usuario.

---

### Cierre de bloque de resultados

`[PAUSA 1.5s]` `[MIRADA JURADO]`

> `[VOZ FIRME]` En síntesis:

> La conclusión no depende de un único número.

> Cruza desempeño técnico, usabilidad percibida, esfuerzo de trabajo, y lo que el usuario expresó verbalmente.

> Juntos, los datos indican que la propuesta es **técnicamente viable, usable y genera diferencia**.

`[PAUSA 2s]`

---

# MINUTOS 25:00-27:00 — LIMITACIONES

## Objetivo de Sección

Honestidad absoluta. Mostrar límites sin sonar defensivo.

---

### Mostrar en pantalla

**Diapositiva 28:** Cuatro limitaciones claras:

1. **Thermal es heurístico, no FEA**
   - No predice temperaturas exactas
   - Reemplaza ausencia de visualización, no análisis real

2. **Compatibilidad móvil es esperada, no garantizada**
   - Depende del navegador, versión, memoria
   - Desktop es plataforma primaria de validación

3. **Validación formativa, no estadísticamente potentada**
   - Muestra n=8-12 (meta 30)
   - Diseño descriptivo, no experimental con control

4. **Modelo único, arquitectura extensible pero no probada**
   - X500 V2 es caso de estudio
   - Escala a otros modelos: pendiente verificación

### Decir

> `[VOZ FIRME]` Voy a ser honesto sobre qué **no funciona** o **aún está incompleto**.

> Porque un proyecto gana credibilidad cuando muestra que conoce sus propios límites.

`[PAUSA 1.5s]`

> `[SEÑALAR 1]` **El thermal no es simulación exacta.**

> Es una visualización educativa. Si necesitas análisis térmico riguroso, usas software de elementos finitos especializado.

> Eso está fuera del alcance de este proyecto.

`[PAUSA 1s]`

> `[SEÑALAR 2]` **Mobile no está universalmente validado.**

> El prototipo corre "esperadamente" en navegadores móviles compatibles. Pero hay una diferencia entre "esperado" y "garantizado en todo dispositivo".

> Si tu dispositivo tiene poca memoria o navegador antiguo, puede no funcionar tan bien.

> Por eso la plataforma de referencia es desktop.

`[PAUSA 1s]`

> `[SEÑALAR 3]` **La validación es formativa, no potentada estadísticamente.**

> Tenemos n participantes. Eso es suficiente para evaluar prototipo y recoger retroalimentación cualitativa.

> No es suficiente para afirmar "3D es estadísticamente significativamente mejor que 2D en población general".

> Para eso necesitarías más participantes y poder estadístico calculado.

`[PAUSA 1s]`

> `[SEÑALAR 4]` **Es un MVP.**

> Arquitectura está lista para extender a otros modelos. Pero la generalización es hipótesis, no validada aún.

> Si mañana le muestras el código a otro ingeniero y dice "apliquemos esto a otro dron", va a funcionar mayormente, pero necesitará ajustes.

`[PAUSA 1.5s]`

> `[MIRADA JURADO]` La pregunta importante no es "¿tiene limitaciones?"

> Todo proyecto tiene límites. La pregunta es: **"¿sabe dónde terminan sus limitaciones?"**

`[VOZ FIRME]`

> La respuesta es: sí.

> Y eso fortalece, no debilita, el proyecto.

`[PAUSA 2s]`

---

# MINUTOS 27:00-30:00 — APORTE Y CIERRE

## Objetivo de Sección

Cerrar con la contribución principal. Dejar una frase que el jurado recuerde.

---

### Mostrar en pantalla

**Diapositiva 29:** Frase central de cierre (grande, clara):

> **"Esa es la diferencia entre mostrar un modelo y construir una herramienta de comprensión técnica."**

### Decir

> `[PAUSA 2s]` `[MIRADA JURADO]`

> Regreso al punto de inicio.

> El problema era: documentación técnica fragmentada obliga a reconstrucción mental costosa.

> La propuesta fue: convertir eso en una experiencia web 3D interactiva.

> El resultado es: un visor que permite explorar, comprender, analizar sin depender de software especializado.

`[PAUSA 1s]`

> `[VOZ FIRME]` **"Esa es la diferencia entre mostrar un modelo y construir una herramienta de comprensión técnica."**

`[PAUSA 1.5s]`

> Mostrar es display pasivo. Aquí.

> Construir es crear una interfaz que **externaliza carga cognitiva** y permite que el usuario se enfoque en lo que importa: comprender relaciones.

---

### Mostrar en pantalla (nueva diapositiva)

**Diapositiva 30:** Aportes sintetizados:

**Aporte técnico**

- Pipeline documentado: CAD → Web 3D
- Decisiones justificadas, trade-offs explícitos
- Metodología replicable

**Aporte académico (Ingeniería Multimedia)**

- Integración: cognición + gráficos + UX + ingeniería
- Caso aplicado con evaluación formal
- Documentación completa (informe, código, tooling)

**Aporte práctico**

- Herramienta usable para inspección, capacitación, documentación
- Patrón escalable a otros hardwares complejos

### Decir (continuación)

> `[VOZ FIRME]` Los aportes del proyecto son tres:

> `[SEÑALAR]` **Técnico:** Documenté un pipeline completo desde CAD industrial hasta un visor web optimizado. Otros equipos pueden replicarlo.

> **Académico:** Integré teoría cognitiva, computación gráfica, diseño UX e ingeniería de software en un caso único. Eso es lo que hace Ingeniería Multimedia.

> **Práctico:** Creer una herramienta que funciona hoy, en navegador, para el X500 V2. Y que puede extenderse.

`[PAUSA 2s]`

> El siguiente paso lógico es:

> Cerrar la validación con más usuarios, medir con rigor estadístico, y escalar a otros modelos.

> Pero eso es trabajo futuro. Este proyecto cierra un ciclo.

`[PAUSA 1.5s]` `[MIRADA JURADO]`

---

### CIERRE FINAL

`[VOZ FIRME]` `[PAUSA 2s]`

> Muchas gracias.

`[PAUSA 2s]` `[SILENCIO FINAL]`

_(Mantener posición, mirada al jurado, sin hablar. Permitir que el silencio selle el final.)_

---

# NOTAS DE ENTREGA PARA ENSAYO FINAL

## Timing y respiración

- Esto DEBE caber en 28-29 minutos reales (no 30 exactos).
- Si sales a 27 min, está bien. Dejas margen para demo o preguntas sorpresa.
- **Marca de tiempo 1:** Min 15 debe ser puntuales (transición a Módulos). Si sales por delante, acelera poco.
- **Marca de tiempo 2:** Min 22:30 debe ser puntual (Resultados). Si atrasas aquí, comprimes limitaciones.

## Entrega conversacional

- NO leas las diapositivas. Habla a la audiencia. Las diapositivas son apoyo visual.
- Pausa genuina después de puntos clave, no relleno ("ehh", "este").
- Si pierdes hilo: fija vista en el slide título, respira, retoma desde ahí.

## Manejo de ansiedad

- Memoriza palabra por palabra: SOLO la apertura (0:00-0:45) y el cierre (27:00-30:00).
- El resto: landmarks. Sabes de qué trata cada bloque.
- Si quedas en blanco: lee el título del slide, respira, continúa.

## Gestos y movimiento

- Cambio de posición solo entre bloques, no dentro.
- Gesto abierto (palmas visibles) para propuestas y momentos de importancia.
- Señalar solo cuando dices "este", "aquí", "mira esto".
- Mirada: comienza mirando jurado, termina en jurado.

## Riesgos anticipados

- **Jurado pregunta "¿por qué NASA-TLX?"** → Respuesta preparada en INF_EST_61
- **Jurado pregunta "¿es mejor que 2D?"** → "Viable y con diferencias observables. Mejor en contexto, no universalmente."
- **Jurado pregunta "¿reemplaza FEA?"** → "No. Es complemento, no reemplazo."

---

**DOCUMENTO FINALIZADO**

Versión 70 del guión incorpora:

- Estructura evaluativa (deep research)
- Storytelling científico
- CTML (una idea principal por bloque)
- Entrega conversacional (landmarks + memorización selectiva)
- Manejo de ansiedad (pausas, respiración, recuperación)
- Coherencia con INF_EST_60 (spine) e INF_EST_61 (claims)
- Integridad académica (limitaciones honestas, aportes reales)

**Listo para ensayo, grabación y defensa.**
