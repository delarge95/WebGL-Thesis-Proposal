---
tipo: guion_sustentacion
fuente: INF_EST_51_Guion_Completo_Sustentacion_30min
estado: activo
area: sustentacion
version: 2
parte: 2
duracion_objetivo: 13min30s
tags:
  - tesis
  - guion
  - sustentacion
  - oratoria
  - partitura
---

# INF EST 55 Guion Corregido v2 Parte 2

Guion corregido de sustentacion, bloques 15:30 a 29:00. Incluye los tres modulos (Inspect, Analyze, Studio), corte transversal, termico, resultados, cierre, version ultra breve de 15 min y lista de mejoras aplicadas.

Documentos relacionados:

- [[INF_EST_53_Auditoria_Oratoria_v2_Diagnostico]]
- [[INF_EST_54_Guion_Corregido_v2_Parte1]]
- [[INF_EST_51_Guion_Completo_Sustentacion_30min]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa]]

---

# Guion Corregido v2 — Sustentacion 30 min (Parte 2: 15:30-29:00)

---

## 15:30–18:30 · Los Tres Módulos de la App

**Mostrar:** Captura mobile con barra inferior → íconos de Inspect, Analyze, Studio.

**Decir:**

> La interfaz se diseñó mobile-first: primero pantalla pequeña, tacto y acciones claras. En móvil hay poco espacio, y eso obliga a priorizar.

> `[GESTO ABIERTO]` De ahí sale la barra inferior, la ficha desplegable y la organización por módulos.

`[PAUSA 1s]`

> `[VOZ FIRME]` La app se organiza en tres módulos funcionales. Cada uno agrupa herramientas según el tipo de pregunta que el usuario necesita responder.

`[AVANZAR]` **Mostrar:** Captura de Inspect con hotspot visible y ficha inferior.

> `[SEÑALAR]` El primero es Inspect. Aquí el usuario selecciona una pieza y obtiene su ficha técnica en el panel inferior.

> Pero no solo puede seleccionar. También puede aislar: ocultar todo lo demás para ver esa pieza en contexto limpio.

> Y si necesita profundizar, puede aislar por capas: primero un grupo, luego una pieza dentro de ese grupo, y luego un sujetador dentro de esa pieza.

> Los hotspots señalan puntos de interés directamente sobre el modelo. Funcionan como anclas visuales que invitan al usuario a explorar sin adivinar.

`[AVANZAR]` **Mostrar:** Captura de Analyze con vista explosiva o corte transversal.

> `[SEÑALAR]` El segundo módulo es Analyze. Aquí el usuario pasa de mirar a descomponer.

> La vista explosiva separa las piezas a lo largo de un eje, mostrando la secuencia de ensamblaje y las relaciones de posición.

> El corte transversal usa un plano que divide visualmente el modelo: lo que queda de un lado se ve, lo del otro desaparece. No se destruye geometría; solo se decide qué fragmentos se dibujan.

> Y los filtros por categoría permiten ver solo un grupo funcional: por ejemplo, solo motores, solo brazos o solo tornillería. Esto reduce ruido visual y ayuda a enfocarse.

`[AVANZAR]` **Mostrar:** Captura de Studio con selector de shader y selector de entorno.

> `[SEÑALAR]` El tercer módulo es Studio. Aquí el usuario controla cómo se ve el modelo y en qué contexto.

> Los modos de visualización cambian el shader aplicado al dron. El modo por defecto es Realistic, que usa materiales basados en física: color, metal, rugosidad, reflejo.

> Solid Color elimina el ruido de los materiales y deja solo forma y contraste. X-Ray permite ver relaciones internas por transparencia. Y Thermal muestra la lupa térmica que explico más adelante.

> Los entornos controlan la iluminación y el fondo. Day, Sunset, Night y Studio cambian la luz ambiental. Studio Light añade una configuración neutra de presentación. Y Blueprint transforma todo el contexto visual en lectura de plano técnico: fondo, grilla, siluetas y contraste alto.

`[PAUSA 1s]`

> `[MIRADA JURADO]` Blueprint vive como entorno y no como shader independiente porque su efecto depende del fondo, la grilla y el contexto completo, no solo del material de la pieza. Es una decisión de diseño: separar lo que modifica la pieza de lo que modifica la escena completa.

> Finalmente, hay colores de fondo configurables: blanco, gris, negro y una paleta de colores sólidos que ayudan a evaluar contraste y legibilidad.

**Puente:**

> Un detalle más sobre la interfaz: los íconos no son imágenes pegadas. Se dibujan por código, lo que mantiene nitidez y permite animarlos según estado. Una microinteracción no es adorno: es feedback que confirma que la acción fue reconocida.

> Pero para que interfaz, piezas y datos funcionaran juntos, la app necesitaba arquitectura.

---

## 18:30–19:30 · Arquitectura y Tooling

**Mostrar:** Diagrama de bootstrap / controladores. Herramientas de editor.

**Decir:**

> La app no es una escena armada a mano. Tiene una arquitectura donde varios controladores coordinan interfaz, selección, modos visuales y datos en tiempo de ejecución.

> `[SEÑALAR]` Las herramientas de editor ayudaron a convertir el modelo importado en una estructura navegable y verificable: auditorías de cobertura, construcción del grafo térmico y validación de datos por pieza.

> `[VOZ FIRME]` No solo importé un modelo. Construí una cadena para comprobar datos, selección y coherencia.

> Esta separación importa: una cosa es lo visible para el usuario, otra las herramientas de desarrollo y otra el código que quedó como legado o trabajo futuro.

**Puente:**

> Con esa arquitectura, quiero detenerme en dos capacidades visuales que merecen explicación técnica: el corte y el modo térmico.

---

## 19:30–21:00 · Corte Transversal y Fórmula

**Mostrar:** Modo corte activo → fórmula de corte.

**Decir:**

> Ya mencioné que el corte divide visualmente el modelo. La mecánica es directa.

> `[SEÑALAR]` Un plano matemático cruza la escena. Para cada fragmento del modelo, el shader calcula de qué lado queda. Si queda del lado descartado, no se dibuja.

`[AVANZAR]` **Mostrar:** Fórmula.

```text
distancia = dot(posición_fragmento - punto_plano, normal_plano)
```

> `[GESTO ABIERTO]` Si la distancia es negativa, el fragmento se oculta. Si es positiva, se renderiza.

> `[VOZ FIRME]` No se destruye la malla original. Solo se decide qué parte aparece en pantalla.

**Puente:**

> El último modo visual lleva la lectura a una capa adicional: el comportamiento térmico.

---

## 21:00–22:30 · Modo Térmico

**Mostrar:** Vista Thermal con leyenda. Gráfico de piezas calientes.

**Decir:**

> `[MIRADA JURADO]` El modo térmico necesita una precisión desde el inicio.

> No es análisis por elementos finitos. `[ÉNFASIS]` Es una lupa térmica por componentes.

> Sirve para mostrar tendencias relativas: qué se calienta más, qué influye en qué y cómo cambia el color con la carga.

`[AVANZAR]` **Mostrar:** Fórmula de calentamiento.

```text
ΔT_source = (T_eq − T_actual)(1 − e^(−Δt / τ)) · w_s
```

> La lógica es así: si la pieza está lejos de su equilibrio, cambia más. Si ya está cerca, cambia menos. La constante tau controla la velocidad. Y el peso de la fuente ajusta su influencia.

> `[SEÑALAR LEYENDA]` Luego, el shader convierte el valor en color. La matemática se vuelve lectura visual.

`[PAUSA 1s]`

> `[VOZ FIRME]` Esto no predice temperaturas exactas. Muestra tendencias para apoyar la inspección.

**Puente:**

> Con el prototipo construido, la pregunta final es: ¿qué evidencia deja?

---

## 22:30–25:00 · Resultados

**Mostrar:** Tabla de KPIs → gráfica pre/post → SUS → NASA-TLX → categorías Think-Aloud.

**Decir:**

> Los resultados responden en tres capas.

> `[SEÑALAR]` Primera: ¿la app funciona con fluidez?

> En la build `[BUILD_ID]`, bajo `[NAVEGADOR]` en `[DISPOSITIVO]`, el prototipo registró `[FPS_PROMEDIO]` cuadros por segundo y `[FRAME_TIME_PROMEDIO]` milisegundos por cuadro. La meta era 30 cuadros, equivalente a 33 milisegundos.

> `[SEÑALAR GRÁFICA]` La optimización redujo `[REDUCCIÓN_PESO]` el peso de assets y mejoró `[MEJORA_FRAME_TIME]` el tiempo por cuadro.

`[PAUSA 1s]`

> Segunda: ¿una persona puede usarlo?

> SUS se aplicó solo al visor 3D. El resultado fue `[SUS_PROMEDIO]`, comparado con 68 como referencia histórica y 72 como umbral deseable.

`[AVANZAR]`

> Tercera: ¿cuánto esfuerzo percibió el usuario?

> NASA-TLX comparó dos condiciones. El visor 3D obtuvo `[TLX_3D]`; el soporte 2D obtuvo `[TLX_2D]`. La diferencia fue `[DIFERENCIA_TLX]`.

`[PAUSA 1s]`

> La retroalimentación cualitativa, recogida con el protocolo Think-Aloud, explicó esos números. Las categorías con mayor recurrencia fueron `[CATEGORÍA_1]`, `[CATEGORÍA_2]` y `[CATEGORÍA_3]`.

> `[VOZ FIRME]` La conclusión no depende de un único número. Cruza desempeño, esfuerzo percibido, usabilidad y lo que los usuarios expresaron durante las tareas.

**Puente:**

> Al juntar técnica y experiencia, aparecen los aportes reales del proyecto.

---

## 25:00–26:30 · Integración: Cuatro Aportes

**Mostrar:** Diapositiva "Cuatro aportes".

**Decir:**

> Integrando todo, el proyecto deja cuatro aportes.

> Uno: un pipeline documentado para transformar CAD complejo en assets web.

> Dos: un visor 3D con tres módulos funcionales — inspección, análisis y presentación visual.

> Tres: modos de visualización y herramientas de lectura técnica: aislamiento, vista explosiva, corte, filtros, entornos y lupa térmica.

> Cuatro: una evaluación formativa con evidencia técnica y de usuario.

`[PAUSA 1s]`

> `[VOZ FIRME]` El resultado no es "3D siempre es mejor". Es más preciso: para este caso, con este protocolo y esta build, el visor ofrece una alternativa viable para comunicar hardware complejo.

**Puente:**

> Esa afirmación también exige reconocer límites.

---

## 26:30–28:00 · Limitaciones y Trabajo Futuro

**Mostrar:** Tabla limitación → continuidad.

**Decir:**

> `[MIRADA JURADO]` Las limitaciones también hacen parte de la contribución.

> La validación no pretende representar a toda una población.

> El modo térmico es heurístico, no calibrado.

> Algunas funciones quedaron implementadas pero ocultas, como Wireframe o Ghosted, para no saturar la interfaz.

> Y la interfaz de escritorio funciona, pero nace de una base mobile-first.

`[PAUSA 1s]`

> Como trabajo futuro quedan líneas concretas derivadas de estos límites.

> `[SEÑALAR]` Primero: una muestra mayor de participantes y pruebas en más dispositivos.

> Segundo: soporte multi-idioma para ampliar el alcance de la herramienta.

> Tercero: inclusión del cableado del dron, que hoy no forma parte del modelo por complejidad geométrica.

> Cuarto: automatización del cuello de botella más costoso del proyecto — la optimización manual de modelos CAD para web. Ese proceso fue artesanal pieza por pieza; documentarlo y automatizarlo abriría el pipeline a otros ensamblajes.

> Y quinto: una posible extensión térmica con datos calibrados, pasando de visualización heurística a modelo validado con mediciones reales.

`[PAUSA 1s]`

> `[VOZ FIRME]` La tesis no se fortalece diciendo que todo está cerrado. Se fortalece mostrando qué está verificado y qué falta medir.

**Puente:**

> Con eso, vuelvo a la imagen del inicio.

---

## 28:00–29:00 · Cierre

**Mostrar:** Imagen inicial del manual → transición al visor → frase final.

**Decir:**

> Al inicio vimos una idea: ver piezas no basta para comprender relaciones.

> Este proyecto no elimina la documentación técnica. La complementa con una forma interactiva de leer piezas, estados y modos visuales.

> `[MIRADA JURADO]` La contribución no es solo la app.

> Es el camino documentado: del CAD a la web, tres módulos de inspección, análisis y presentación, modos visuales y una evaluación con límites claros.

`[PAUSA 2s]`

> `[ÉNFASIS]` `[VOZ FIRME]` Esa es la diferencia entre mostrar un modelo y construir una herramienta de comprensión técnica.

`[PAUSA 2s]`

> `[MIRADA JURADO]` Muchas gracias.

**Acción:** No moverse durante la frase final. Mantener la mirada en el jurado. Pausar antes de recibir preguntas. No decir "eso sería todo".

---

# Versión Ultra Breve — 15 minutos

| Tiempo | Bloque | Contenido |
|--------|--------|-----------|
| 0:00–1:00 | Gancho + saludo | "Imaginen…" + nombre + pregunta central |
| 1:00–2:30 | Problema y objetivos | Tres capas + pregunta + objetivos en una frase |
| 2:30–4:00 | Método e instrumentos | DSR + SUS + NASA-TLX + Think-Aloud (sin fórmulas) |
| 4:00–7:00 | CAD a web | Pipeline + MAD-T + conteos 28/30/257 |
| 7:00–10:00 | Tres módulos + shaders | Inspect/Analyze/Studio + modos + entornos + térmico |
| 10:00–12:30 | Resultados | KPIs + SUS + TLX + categorías Think-Aloud |
| 12:30–14:00 | Aportes + límites | Cuatro aportes + trabajo futuro |
| 14:00–15:00 | Cierre | Una frase final |

---

# Lista Consolidada de Mejoras v2

1. **"Pensamiento en voz alta" eliminado.** Se usa "protocolo Think-Aloud" la primera vez y "retroalimentación cualitativa" en las siguientes.
2. **Tres módulos presentados con estructura completa:**
   - **Inspect:** selección, ficha técnica, aislamiento por capas, hotspots.
   - **Analyze:** vista explosiva, corte transversal, filtros por categoría.
   - **Studio:** modos de visualización (shaders) + entornos (iluminación y fondos).
3. **Shaders corregidos:** Realistic como modo por defecto; Solid Color incluido; X-Ray y Thermal mencionados en su módulo correcto (Studio).
4. **Blueprint justificado como entorno**, no como shader independiente: su efecto depende del fondo, la grilla y el contexto completo, no solo del material.
5. **Entornos explicados:** Day, Sunset, Night, Studio, Studio Light, Blueprint y paleta de colores base. Se aclara que controlan iluminación y fondo, no simulación física.
6. **Vista explosiva explicada** como herramienta de Analyze que separa piezas para mostrar secuencia de ensamblaje.
7. **Filtros por categoría explicados** como herramienta de Analyze para aislar grupos funcionales.
8. **Hotspots explicados** como anclas visuales sobre el modelo en modo Inspect.
9. **Aislamiento por capas explicado** como función progresiva: grupo → pieza → sujetador.
10. **Trabajo futuro ampliado:** multi-idioma, cableado del dron, automatización del cuello de botella CAD, extensión térmica calibrada, más dispositivos.
11. **Apertura con micro-tensión concreta** ("imaginen que les entregan este dron").
12. **NASA-TLX reubicado** en metodología con contexto natural.
13. **Herramientas de producción comprimidas** sin catálogo de 5 nombres.
14. **Definición de runtime eliminada** por condescendiente.
15. **Cierre con frase única final.**
16. **Tiempo total ajustado** a ~29 min + 1 min buffer.

---

# Preguntas que Deben Prepararse

- ¿Por qué Unity Web y no Three.js?
- ¿Por qué mobile-first si también funciona en escritorio?
- ¿Por qué SUS solo para 3D?
- ¿Por qué NASA-TLX no equivale a carga cognitiva?
- ¿Por qué 28 piezas no contradicen 257 assets?
- ¿Por qué Thermal no es FEA?
- ¿Por qué Blueprint es entorno y no shader?
- ¿Cómo se asegura que la comparación 2D no sea injusta?
- ¿Qué pasa si los resultados no favorecen al 3D?
- ¿Qué funciones son finales, ocultas, legacy o futuras?
- ¿Por qué no se incluyó el cableado?
- ¿Por qué la optimización CAD fue manual?
