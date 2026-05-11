---
tipo: mapa_claims
estado: activo
area: sustentacion
version: 1
fecha: 2026-05-08
tags:
  - tesis
  - claims
  - sustentacion
  - argumentation
---

# INF_EST_61 — Mapa de Claims de la Defensa

> **Propósito**: Documento que especifica para CADA slide/bloque la afirmación principal, su evidencia y cómo se conecta con la tesis central. Sirve como filtro de coherencia y como base para las Frequently Asked Questions del jurado.

---

## BLOQUE 0 — HOOK (0:00-0:50)

**Claim:**

> El problema fundamental no es falta de información, sino que la información técnica compleja no es fácilmente explorable ni interactiva.

**Evidencia visual:**

- Lámina técnica 2D del dron Holybro X500 V2
- Pregunta retórica: "¿dónde vive cada pieza dentro del sistema?"

**Conexión:**
Enuncia el problema que da origen a la tesis. Genera tensión: "ver piezas ≠ comprender relaciones".

**Pregunta que anticipa:**

- ¿Qué entiendes por "comprender relaciones"?
- **Respuesta preparada:** Entender qué se conecta con qué, cómo se ensamblan, dónde quedan ocultas ciertas piezas y por qué el orden de ensamblaje importa.

---

## BLOQUE 1 — SALUDO FORMAL (0:50-1:30)

**Claim:**

> Esta es mi tesis de grado: un visor web para inspeccionar el dron Holybro X500 V2.

**Evidencia visual:**

- Portada formal
- Identificación clara del proyecto y del caso de estudio

**Conexión:**
Formaliza la presentación y define el caso de estudio concreto (X500 V2 por su complejidad).

**Pregunta que anticipa:**

- ¿Por qué el Holybro X500 V2 específicamente?
- **Respuesta preparada:** Porque tiene densidad de componentes, relaciones espaciales complejas, múltiples subsistemas (estructura, energía, electrónica, control) y es representativo de hardware complejo real.

---

## BLOQUE 2 — DEL MANUAL AL VISOR (1:30-2:00)

**Claim:**

> La documentación técnica es necesaria pero insuficiente. El visor web 3D traduce esa información estática en experiencia interactiva donde el usuario puede explorar, aislar y analizar visualmente.

**Evidencia visual:**

- Manual 2D (fragmentado)
- Captura del visor (integrado, navegable)
- Transición visual manual → visor

**Conexión:**
Conecta el problema (manual fragmentado) con la solución (visor integrado). Introduce la propuesta sin jerga.

**Pregunta que anticipa:**

- ¿Por qué no solo mejorar la documentación 2D?
- **Respuesta preparada:** Porque documentación mejorada sigue siendo estática. La interactividad externaliza el trabajo cognitivo de reconstrucción mental hacia la interfaz.

---

## BLOQUE 3 — PROBLEMA, PREGUNTA Y OBJETIVOS (2:00-4:30)

**Claim 1:**

> El problema tiene tres capas: humana (comprensión requiere esfuerzo), técnica (CAD pesado no cabe en web) y metodológica (hay que evaluarlo rigidamente).

**Evidencia visual:**

- Diagrama de tres capas
- Pregunta de investigación explícita

**Conexión:**
Establece que no es solo un proyecto visual, sino un problema multidimensional que exige rigor.

**Pregunta que anticipa:**

- ¿Cuál es tu pregunta de investigación exacta?
- **Respuesta preparada:** [Leer exactamente de la diapositiva] Cómo diseñar y evaluar un visor web 3D que ayude a inspeccionar este ensamblaje sin perder viabilidad técnica en navegador.

**Claim 2:**

> Los objetivos responden a esa ruta: entender, transformar, implementar, evaluar.

**Evidencia visual:**

- Cuatro objetivos específicos listados
- Cada uno conectado a un resultado esperado

**Conexión:**
Cierra la sección de problema con una hoja de ruta clara del trabajo realizado.

---

## BLOQUE 4 — FUNDAMENTO: COGNICIÓN (4:30-6:30)

**Claim:**

> La teoría de carga cognitiva demuestra que la memoria de trabajo es limitada. Una interfaz 3D bien diseñada reduce el trabajo que el usuario debe hacer imaginando y lo redirige a tareas de verdadera comprensión.

**Evidencia visual:**

- Diagrama: carga cognitiva 2D (alta extrínseca) vs carga 3D (reducida extrínseca, aumentada germana)
- Fórmulas o gráficos que resumen el concepto

**Conexión:**
Proporciona base teórica para por qué 3D es mejor que 2D. Anticipa la evaluación con NASA-TLX.

**Pregunta que anticipa:**

- ¿Mediste la carga cognitiva directamente?
- **Respuesta preparada:** No. La carga cognitiva es un constructo teórico. Lo que medí fue carga de trabajo PERCIBIDA usando NASA-TLX, que es un instrumento validado. La teoría me sirve como marco interpretativo.

---

## BLOQUE 5 — RENDIMIENTO WEB (6:30-8:30)

**Claim:**

> Para que la visualización 3D funcione en navegador, se deben controlar presupuestos de geometría, materiales, draw calls y frame time. 30 FPS es la meta operativa mínima para que la navegación sea fluida.

**Evidencia visual:**

- Fórmula de frame time: frame_time = 1000 / FPS
- Tabla de presupuestos típicos
- Ecuación de costo de dibujo

**Conexión:**
Justifica por qué la optimización no es cosmética sino funcional. Establece métricas cuantificables.

**Pregunta que anticipa:**

- ¿Por qué 30 FPS y no 60?
- **Respuesta preparada:** Porque 30 FPS = ~33 ms por cuadro es un estándar industria para experiencias interactivas fluidas con restricciones de navegador web. 60 es aspiracional pero no es requisito para esta aplicación.

---

## BLOQUE 6 — METODOLOGÍA (8:30-11:30)

### Claim Macro:

> Usé Design Science Research + métodos de Peffers + heurísticas de Hevner para construir y evaluar el artefacto con rigor académico.

### Sub-claims:

**6a. DSR es apropiado aquí**

- Claim: "Construí un artefacto (visor) que resuelve un problema práctico (comprensión de hardware), y lo evalué formalmente."
- Evidencia: Referencia a Peffers, que define DSR como: problema → objetivos → diseño → demostración → evaluación → comunicación.
- Pregunta que anticipa: ¿DSR no es solo para informática?
  - Respuesta: No. DSR es un paradigma para cualquier disciplina que construye artefactos. En Ingeniería Multimedia, es muy relevante.

**6b. Evaluación técnica clara**

- Claim: "Medí rendimiento en términos de FPS, frame time, memoria y carga inicial."
- Evidencia: KPIs específicos y herramientas (Unity Profiler, Chrome DevTools).

**6c. Evaluación de experiencia**

- Claim: "Evalué cómo perciben los usuarios el esfuerzo (NASA-TLX) y la usabilidad (SUS)."
- Evidencia: Instrumentos validados, tamaño de muestra (meta 30; operativo 8-12).

**6d. Método Think-Aloud para cualitativo**

- Claim: "Recogí explicaciones verbales de lo que los usuarios pensaban mientras usaban el visor."
- Evidencia: Protocolo Think-Aloud, grabación, categorización posterior.

**Pregunta que anticipa:**

- ¿Por qué no hiciste un estudio experimental controlado con grupo control?
- **Respuesta preparada:** Porque el objetivo era evaluar el prototipo, no hacer una comparación estadística de grupo. Diseño descriptivo y formativo es apropiado para DSR. Si la institución requiere experimental, eso será trabajo futuro.

---

## BLOQUE 7 — PIPELINE DE OPTIMIZACIÓN (11:30-13:30)

**Claim:**

> Convertir CAD en WebGL viable requiere una cadena: CAD → limpieza en Blender → optimización → baking de texturas → importación en Unity → build web. Cada paso tiene razón de ser y no puede saltarse.

**Evidencia visual:**

- Diagrama del pipeline (5-6 pasos claros)
- Ejemplo antes/después de una pieza
- Tabla de reducciones por etapa

**Conexión:**
Justifica por qué la optimización es central, no secundaria. Conecta con decisión de usar Unity (que integra todo esto).

**Pregunta que anticipa:**

- ¿No podrías simplemente usar Three.js y evitar Unity?
- **Respuesta preparada:** Podrías, pero la fragmentación del pipeline haría más lento el desarrollo y más difícil la depuración. Unity me dio un entorno unificado para este pipeline completo.

---

## BLOQUE 8 — MATERIALES Y TEXTURAS (13:30-14:30)

**Claim:**

> Los materiales no son decoración. Un sistema PBR con atlas de texturas permite que el modelo sea legible (se ve cómo está hecho) y eficiente (bajo overhead de memoria y draw calls).

**Evidencia visual:**

- Atlas de texturas 4K (BaseColor, Normal, Mask)
- Comparación visual: modelo con materiales vs sin materiales
- Especificación técnica de resoluciones

**Conexión:**
Demuestra que cada decisión técnica sirve a un propósito tanto visual como funcional.

**Pregunta que anticipa:**

- ¿No podrías usar texturas más pequeñas?
- **Respuesta preparada:** Sí, pero a costo de legibilidad visual. 4K es el mínimo donde los detalles técnicos (fibra de carbono, conectores, etc.) siguen siendo legibles.

---

## BLOQUE 9 — TRES MÓDULOS: INSPECT, ANALYZE, STUDIO (15:30-18:30)

### Claim Macro:

> La interfaz se organizó en tres módulos móvil-primero, cada uno responde a un tipo de pregunta del usuario.

### Sub-claims:

**9a. Inspect: ¿Qué estoy mirando?**

- Claim: "Seleccionar una pieza muestra su ficha: nombre, categoría, especificaciones, relación con pieza madre."
- Evidencia visual: Screenshot de selección + panel inferior con datos.
- Función: Identificación y contexto técnico.

**9b. Analyze: ¿Cómo se relacionan las piezas?**

- Claim: "Tres herramientas: vista explotada (separa en eje), corte (divide y filtra), categoría (muestra solo un grupo)."
- Evidencia visual: Screenshots de cada modo.
- Función: Inspección de relaciones y descomposición visual.

**9c. Studio: ¿Cómo veo esto?**

- Claim: "Cinco modos visuales (Realistic, X-Ray, Solid Color, Thermal, Blueprint) + cuatro entornos de iluminación + colores de fondo."
- Evidencia visual: Grid de 5-6 modos en un montaje.
- Función: Lectura visual adaptada a tarea del usuario.

**Pregunta que anticipa:**

- ¿Por qué no todo en un menú?
- **Respuesta preparada:** Porque mobile-first design requiere prioridad. Agrupar por función reduce carga cognitiva: el usuario enfocado en inspección no ve botones de análisis.

---

## BLOQUE 10 — ARQUITECTURA Y TOOLING (18:30-19:30)

**Claim:**

> La app no es una escena armada a mano. Una arquitectura con controladores coordinan interfaz, datos y visualización. Herramientas de editor verifican cobertura e integridad.

**Evidencia visual:**

- Diagrama de arquitectura (4-5 capas)
- Screenshots de herramientas de auditoría

**Conexión:**
Demuestra que la implementación no es improvisada, sino architected. Respalda confiabilidad del sistema.

**Pregunta que anticipa:**

- ¿Cuántas líneas de código?
- **Respuesta preparada:** No es la métrica relevante. Lo relevante es si la arquitectura es coherente, extensible y permite que nuevas funciones se añadan sin quebrar las existentes.

---

## BLOQUE 11 — CORTE TRANSVERSAL Y FÓRMULA (19:30-21:00)

**Claim:**

> El corte usa un plano matemático y un shader para decidir qué fragmentos de la geometría se dibujan. La fórmula es simple pero eficaz: `distancia = dot(posición - punto_plano, normal)`.

**Evidencia visual:**

- Fórmula explícita
- Screenshot del modo corte activo
- Explicación paso a paso de qué hace la fórmula

**Conexión:**
Demuestra que incluso herramientas analíticas avanzadas se basan en matemática directa, no en "magia".

**Pregunta que anticipa:**

- ¿Cómo se comporta el corte con instancias?
- **Respuesta preparada:** La fórmula aplica a cada fragmento individualmente, así que instancias se tratan correctamente. Todos los fragmentos con la misma posición relativa al plano tendrán el mismo comportamiento.

---

## BLOQUE 12 — MODO TÉRMICO (21:00-22:30)

**Claim:**

> El modo térmico es una lupa visual de tendencias, NO un análisis por elementos finitos. Muestra qué se calienta más bajo cierta carga, útil para inspección educativa y orientación, no para diseño térmico exacto.

**Evidencia visual:**

- Fórmula de calentamiento: `ΔT = (T_eq - T_actual)(1 - e^(-Δt/τ)) · w_s`
- Screenshot en modo Thermal con leyenda de colores
- Aclaración explícita: "Tendencias, no predicciones exactas"

**Conexión:**
Establece la diferencia entre visualización heurística y simulación científica. Honestidad metodológica.

**Pregunta que anticipa:**

- ¿Esto reemplaza análisis térmico real?
- **Respuesta preparada:** No. Reemplaza la ausencia de visualización. Para decisiones de diseño críticas, necesitarías FEA real. Esto es una ayuda pedagógica y de inspección.

---

## BLOQUE 13 — RESULTADOS (22:30-25:00)

### Claim Macro:

> La evaluación demuestra que el prototipo es técnicamente viable, usable y genera diferencias mensurables en esfuerzo percibido comparado con referencia 2D.

### Sub-claims:

**13a. Técnico: Rendimiento OK**

- Claim: "[FPS_PROMEDIO] FPS bajo [CONDICIÓN], equivalente a [FRAME_TIME_PROMEDIO] ms por cuadro."
- Evidencia: Medición directa, herramientas estándar.
- Pregunta que anticipa: ¿Cómo mediste FPS?
  - Respuesta: Unity Profiler en build web + Chrome DevTools.

**13b. Usabilidad: SUS = [SUS_PROMEDIO]**

- Claim: "El puntaje SUS sugiere que el sistema es usable [si > 68] / aceptable / con oportunidades de mejora."
- Evidencia: Cuestionario de 10 ítems, muestra n=[SAMPLE_SIZE].
- Nota: Incluir benchmark ("68 es promedio histórico").

**13c. Esfuerzo: NASA-TLX muestra diferencia 3D vs 2D**

- Claim: "Carga percibida: 2D=[TLX_2D], 3D=[TLX_3D]. Diferencia: [DIFERENCIA]."
- Evidencia: Instrumento validado, condiciones comparables.
- Precaución: "Diferencia descriptiva, no estadísticamente comprobada aún."

**13d. Cualitativo: Think-Aloud refleja [CATEGORÍA_1, CATEGORÍA_2]**

- Claim: "Las verbalizaciones de usuarios se agruparon en categorías. Las más frecuentes fueron..."
- Evidencia: Grabaciones, transcripción, tabla de recurrencia.

**Pregunta que anticipa:**

- ¿Estos resultados prueban que tu solución es mejor?
- **Respuesta preparada:** No "mejor" en abstracto. Estos resultados demuestran viabilidad técnica, usabilidad aceptable y diferencias en carga percibida bajo las condiciones evaluadas. Para "mejor" necesitarías más participantes y control estadístico.

---

## BLOQUE 14 — LIMITACIONES (25:30-27:00)

**Claim:**

> El proyecto tiene límites claros que no diluyen su valor, sino que lo enmarcan honestamente.

**Sub-claims y evidencia:**

**14a. Thermal es heurística**

- Claim: "No predice temperaturas exactas ni reemplaza FEA."
- Evidencia: Definición de modelo, sin datos de validación física.

**14b. Compatibilidad móvil es esperada, no universal**

- Claim: "El prototipo corre en navegadores compatibles de escritorio y algunos móviles, no en TODOS los dispositivos."
- Evidencia: Documentación de Unity Web, compatibilidad navegador-dependiente.

**14c. Validación aún en progreso**

- Claim: "La muestra de participantes es formativa (8-12 mínimo; meta 30), no representativa de población general."
- Evidencia: Diseño muestral documentado.

**14d. Modelo único**

- Claim: "Es un MVP. Arquitectura preparada para extender a otros modelos, pero no validada aún."
- Evidencia: Estructura código, falta de prueba con segundo modelo.

**Pregunta que anticipa:**

- ¿Entonces es un proyecto incompleto?
- **Respuesta preparada:** No. Es un MVP académico que define y cierra un ciclo: problema claro, solución implementada, evaluación inicial, limitaciones explícitas. Incompletitud explícita es mejor que promesa sobredi

mensionada.

---

## BLOQUE 15 — APORTE Y CIERRE (27:00-30:00)

### Final Claim (La que debe quedar en memoria):

> **"Esa es la diferencia entre mostrar un modelo y construir una herramienta de comprensión técnica."**

**Traducción explícita:**

- Mostrar = display pasivo
- Herramienta de comprensión = interfaz interactiva que externaliza carga cognitiva y apoya inspección

### Sub-claims finales:

**15a. Aporte técnico**

- Claim: "Documenté un pipeline replicable: CAD → Web 3D optimizado."
- Evidencia: Este informe, código disponible, decisiones justificadas.

**15b. Aporte académico (Ingeniería Multimedia)**

- Claim: "Integré cognición, gráficos, UX e ingeniería en un caso único."
- Evidencia: Marco teórico (cognición), computación gráfica (shaders), UX (mobile-first), ingeniería (optimización, WebAssembly).

**15c. Aporte práctico**

- Claim: "Herramienta útil para inspección, capacitación, documentación de hardware."
- Evidencia: Caso real (X500 V2), funcionalidad verificada.

**Pregunta que anticipa:**

- ¿Cuál es el PRÓXIMO paso?
- **Respuesta preparada:** (Honesto) Cerrar la validación de usuarios, medir con más participantes, escalar a otros modelos, explorar integración en documentación técnica real de fabricantes.

---

## TABLA DE VERIFICACIÓN (Para Ensayo)

| Bloque | Claim            | Evidencia presente | Conexión clara                      | Pregunta anticipada      |
| ------ | ---------------- | ------------------ | ----------------------------------- | ------------------------ |
| 0-1    | Hook + Saludo    | ✅                 | ✅                                  | ✅                       |
| 2      | Manual → Visor   | ✅                 | ✅                                  | ✅                       |
| 3      | Problema         | ✅                 | ✅                                  | ✅                       |
| 4      | Teoría cognitiva | ✅                 | ✅                                  | ⚠️ (depende de NASA-TLX) |
| 5      | Rendimiento web  | ✅                 | ✅                                  | ✅                       |
| 6      | Metodología      | ✅                 | ✅                                  | ⚠️ (DSR poco conocido)   |
| 7      | Pipeline         | ✅                 | ✅                                  | ✅                       |
| 8      | Materiales       | ✅                 | ✅                                  | ✅                       |
| 9      | Tres módulos     | ✅                 | ✅                                  | ✅                       |
| 10     | Arquitectura     | ✅                 | ⚠️ (puede parecer sobre-engineered) | ✅                       |
| 11     | Corte            | ✅                 | ✅                                  | ✅                       |
| 12     | Thermal          | ✅                 | ✅                                  | ✅ (crítica esperada)    |
| 13     | Resultados       | ⚠️ (placeholders)  | ✅                                  | ✅                       |
| 14     | Límites          | ✅                 | ✅                                  | ✅ (valida integridad)   |
| 15     | Cierre           | ✅                 | ✅                                  | ✅                       |

✅ = Listo, ⚠️ = Necesita cuidado extra en ensayo
