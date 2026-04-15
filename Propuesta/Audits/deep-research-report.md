# Auditoría académica de máxima exigencia de la propuesta de tesis

## Alcance y criterio de auditoría

Se auditó íntegramente la propuesta corregida (PDF) y se contrastaron afirmaciones metodológicas y técnicas con fuentes académicas, documentación oficial y estándares normativos cuando aplica (DSR/DSRM, SUS, NASA‑TLX, Unity WebGL, memoria, WebAssembly, URP/PBR, y coherencia diseño‑evaluación). Las observaciones del reporte previo (Gemini) se trataron como insumo auxiliar: **ninguna** se asume correcta sin verificación independiente.

## A. Veredicto general

**Parcialmente sólida.**

La versión corregida ya es defendible en lo esencial **solo si** se corrigen fallos verificables de bibliografía (DOI/atribuciones), se formula una **pregunta de investigación explícita**, y se vuelve metodológicamente inequívoco **cómo** se comparará el prototipo 3D frente a la referencia 2D (diseño, orden, administración de instrumentos, y métricas de desempeño). Tal como está, aún hay puntos que un jurado riguroso puede considerar **insuficientemente sustentados o metodológicamente ambiguos**, especialmente por: (i) errores factuales en referencias; (ii) umbrales SUS presentados sin anclaje bibliográfico explícito; (iii) ausencia de definición operativa de “mejorar comprensión” y de controles básicos de sesgo por orden/aprendizaje; (iv) KPIs técnicos sin condiciones de prueba plenamente especificadas.

## B. Hallazgos críticos y altos

### Error factual verificable en referencias y DOI

**Severidad:** Crítico  
**Sección:** Bibliografía (págs. 46–47 del PDF)  
**Afirmación problemática (literal):**  
- “Bartlett, K. A., & Dorribo Camba, J. (2023)… https://doi.org/10.1080/13875868.2021.1987375”  
- “Fransson, E., Hermansson, J., & Hu, Y. (2024)… Proceedings of the 2024 IEEE Conference on Games… https://doi.org/10.1109/CoG60054.2024.10645582”

**Por qué sigue siendo problemática:**  
Son **errores objetivamente comprobables**: el DOI de Bartlett & Dorribo Camba no coincide con el artículo publicado; y el DOI atribuido al paper de WebGPU vs WebGL **corresponde a otro trabajo distinto** (no a WebGPU/WebGL en Godot). En evaluación de posgrado, esto afecta directamente a la credibilidad del aparato crítico: si la bibliografía falla en elementos trazables (DOI/venue), se cuestiona la fiabilidad del resto del documento.

**Corrección propuesta:**  
1) Sustituir DOI de Bartlett & Dorribo Camba por el DOI correcto del artículo (y verificar título, año online y paginación). citeturn13search3turn13search4  
2) Sustituir la referencia de Fransson et al. por la publicación real “GEM 2024” con su DOI correcto (y eliminar la atribución errónea a CoG). citeturn13search10turn14search6turn14search0  
3) Ejecutar una verificación sistemática de **todos** los DOIs y URLs técnicas (Unity/Khronos) antes de entrega.

**Fuente exacta de respaldo:** citeturn13search3turn14search6turn14search0

### Falta de pregunta de investigación explícita y trazabilidad problema→evaluación

**Severidad:** Crítico  
**Sección:** Estructura conceptual (planteamiento/objetivos/metodología; no hay sección de “pregunta”)  
**Afirmación problemática:** No existe una **pregunta de investigación explícita**, pese a que el documento pretende evaluar impacto (usabilidad/carga y, en el marco teórico, comprensión espacial).  

**Por qué sigue siendo problemática:**  
La propuesta declara objetivos de evaluación (“evaluar… usabilidad y carga cognitiva… contrastando resultados”), pero sin una pregunta explícita se debilita la trazabilidad: qué se demuestra, con qué criterio, y qué contaría como evidencia a favor/en contra. En DSRM, el proceso exige una **identificación del problema** y **objetivos de solución**, seguidos por demostración/evaluación; la ausencia de una pregunta explícita no invalida DSR, pero sí deja la defensa más frágil en un tribunal exigente (porque la evaluación queda más interpretativa que criterial). citeturn0search3  

**Corrección propuesta:**  
Añadir una pregunta principal y subpreguntas directamente alineadas con: (i) rendimiento técnico en WebGL; (ii) usabilidad percibida; (iii) carga de trabajo; (iv) desempeño en tareas (exactitud/tiempo) si se afirma comprensión/inspección.

**Fuente exacta de respaldo:** DSRM como proceso con etapas de problema→objetivos→diseño→demostración→evaluación. citeturn0search3

### Comparación 3D vs referencia 2D metodológicamente ambigua

**Severidad:** Alto  
**Sección:** Metodología – tareas e instrumentos (págs. 38–41)  
**Afirmación problemática (representativa):** se propone ejecutar tareas “preferiblemente sobre el prototipo 3D y sobre un soporte 2D de referencia equivalente”, pero no se define con precisión:  
- si el diseño es intra‑sujeto, inter‑sujeto, o mixto;  
- el orden (y control de efectos de aprendizaje/fatiga);  
- si NASA‑TLX y SUS se aplican por condición (3D y 2D) o solo al 3D.

**Por qué sigue siendo problemática:**  
Sin diseño explícito, el “contraste” puede ser atacado como no controlado: cualquier diferencia podría explicarse por **orden**, familiarización, o expectativas. Además, SUS suele administrarse tras tareas representativas y antes de discusión (como en prácticas de laboratorio descritas en JUS) y su interpretación depende del contexto. citeturn8view0  

**Corrección propuesta:**  
- Declarar el diseño: **intra‑sujeto** (recomendable para N pequeño) con **contrabalanceo** del orden (AB/BA).  
- Definir instrumentación:  
  - NASA‑TLX: por condición (3D vs 2D) inmediatamente tras completar las tareas de cada condición. citeturn0search2turn0search13  
  - SUS: aplicar **solo al sistema interactivo** (3D) salvo justificación fuerte para evaluar 2D como “sistema” comparable; si se evalúa 2D, justificar validez de constructo y reportar por separado.

**Fuente exacta de respaldo:** práctica de administrar SUS después de tareas y antes de discusión en estudios formativos; NASA‑TLX como herramienta estándar con seis subescalas para estimar carga de trabajo. citeturn8view0turn0search2turn0search13

### Interpretación de SUS: correcto enfoque “benchmark”, pero umbrales sin soporte explícito y con precisión discutible

**Severidad:** Alto  
**Sección:** Análisis cuantitativo e interpretación SUS (pág. 39)  
**Afirmación problemática (literal):** “Un puntaje de 68 representa aproximadamente el promedio histórico… valores superiores a 72 sugieren una experiencia percibida como deseable…”

**Por qué sigue siendo problemática:**  
- Es correcto tratar **68 como referencia histórica**, pero la literatura reporta benchmarks de forma explícita (p. ej., “mean 68, SD 12.5” como distribución aceptada para benchmarking en múltiples dominios) y conviene citarlo, no dejarlo como afirmación genérica. citeturn9search2  
- La cifra “72” como meta “deseable” no es universal; en escalas por percentil/curva (p. ej. rangos tipo C+/B‑), 72 cae en zona de “ligeramente por encima de la media” y su lectura debe presentarse como **heurística** (percentil aproximado), no como corte normativo. citeturn3search1turn9search2  
- En JUS se reporta un promedio global “~70” en su base de estudios y, además, se describe que 70s son “aceptables”, 80s “buenos”, 90s “excepcionales”; esto contradice una lectura rígida de 68 como único “promedio”. La solución no es elegir un número, sino declarar rango y fuente. citeturn8view0  

**Corrección propuesta:**  
Reescribir el párrafo de SUS para:  
- citar benchmark (68 ± 12.5) y declarar que 68 es percentil ~50 en esa distribución; citeturn9search2  
- presentar metas como rango (p. ej., ≥70 “aceptable”, ≥73 “sobre la media”, ≥80 “alto”) **según escala por percentiles/curva**, evitando formulación dogmática. citeturn3search1turn8view0  

**Fuente exacta de respaldo:** citeturn9search2turn8view0turn3search1

### NASA‑TLX: descripción general correcta, pero falta rigor operativo y trazabilidad de “Raw TLX”

**Severidad:** Alto  
**Sección:** Instrumentos / análisis cuantitativo (págs. 38–40)  
**Afirmación problemática:** se usa “NASA‑TLX Raw” como promedio de subescalas, pero no se explicita (i) si se omite ponderación, (ii) cómo se trata la subescala Performance para coherencia direccional, (iii) en qué punto exacto se administra si hay 2 condiciones.

**Por qué sigue siendo problemática:**  
NASA‑TLX está definido como procedimiento multidimensional y suele reportarse como agregado (ponderado o no ponderado). En auditoría estricta, “Raw TLX = promedio simple” debe declararse como decisión metodológica y mantenerse consistente por condición. citeturn0search2turn0search13  

**Corrección propuesta:**  
Añadir un subapartado operativo:  
- “Se empleará TLX sin ponderación (Raw TLX) como media aritmética de las 6 subescalas (0–100).”  
- “Se administrará TLX por condición (3D y 2D) al finalizar las tareas de cada condición.”  
- “Se documentará el tratamiento de Performance para que el agregado mantenga semántica consistente (mayor = mayor carga).”

**Fuente exacta de respaldo:** descripción oficial de TLX con subescalas y base histórica de desarrollo/uso. citeturn0search2turn0search13

### Think‑Aloud: base correcta, pero falta protocolo para control de sesgos y calidad del dato verbal

**Severidad:** Alto  
**Sección:** Análisis cualitativo (pág. 40)  
**Afirmación problemática:** Se indica aplicación concurrente y categorías de codificación, pero no se define: entrenamiento breve, reglas de intervención del moderador, capturas (audio/video/telemetría), ni control de consistencia de codificación.

**Por qué sigue siendo problemática:**  
El método Think‑Aloud es válido, pero su calidad depende críticamente de consistencia en recogida de verbalizaciones y de un protocolo claro (qué se pide verbalizar, cómo se evita guiar, y cómo se codifica). La literatura clásica sobre análisis de protocolos fundamenta el valor de reportes concurrentes/retrospectivos y la necesidad de metodología rigurosa para inferir procesos cognitivos. citeturn11search0turn11search2turn11search12  

**Corrección propuesta:**  
Incluir:  
- instrucciones estandarizadas (guion),  
- criterio de intervención mínima (recordatorios neutrales “sigue pensando en voz alta”),  
- registro multimodal (audio + captura de pantalla + logs de eventos),  
- y, si hay dos codificadores, acuerdo inter‑codificador; si no, al menos auditoría de codificación (segunda pasada y memo de decisiones).

**Fuente exacta de respaldo:** citeturn11search0turn11search2turn11search12

### Triangulación metodológica: implícita, pero no declarada ni justificada

**Severidad:** Alto  
**Sección:** Metodología (págs. 38–40)  
**Afirmación problemática:** Se plantea combinar SUS + NASA‑TLX + Think‑Aloud, pero no se explicita como estrategia de **triangulación** (qué se triangula y con qué lógica de convergencia/explicación).

**Por qué sigue siendo problemática:**  
En evaluación formativa, la integración de medidas cuantitativas (SUS/TLX) y cualitativas (protocolos) es defendible, pero debe declararse como triangulación y explicar el criterio: convergencia, complementariedad o explicación de discrepancias. Triangulación se reconoce como estrategia para mejorar credibilidad/calidad del análisis cualitativo y para combinar métodos/datos/teorías. citeturn12search0turn12search2  

**Corrección propuesta:**  
Añadir un párrafo explícito:  
- “Se usará triangulación metodológica simultánea: (i) SUS cuantifica usabilidad percibida, (ii) NASA‑TLX cuantifica carga subjetiva, (iii) Think‑Aloud explica mecanismos/causas de puntuaciones y errores.”  
- Definir cómo se resuelven tensiones (p. ej., SUS alto pero TLX alto: interpretación de eficiencia vs esfuerzo).

**Fuente exacta de respaldo:** citeturn12search0turn12search2

### Afirmaciones técnicas sobre Unity WebGL / memoria / WebAssembly correctas en esencia, pero sin citación en los pasajes donde se afirman

**Severidad:** Alto  
**Sección:** Planteamiento del problema / justificación técnica (págs. 10–13)  
**Afirmación problemática (representativa):** se presentan como hechos técnicos: crecimiento de memoria, fragmentación, necesidad de bloques contiguos, y relación IL2CPP→C++→Wasm, pero en los párrafos técnicos no se cita documentación oficial.

**Por qué sigue siendo problemática:**  
Aunque el contenido es **materialmente compatible** con documentación oficial, en estándar de posgrado no basta “que sea verdad”: debe ser **rastreable**. Unity documenta explícitamente que el heap/asset data requieren bloques contiguos y que la fragmentación puede impedir asignación; también describe el pipeline con Emscripten y IL2CPP para WebGL/Wasm. citeturn4search0turn4search3turn4search1  

**Corrección propuesta:**  
Insertar citas directas en los pasajes técnicos:  
- memoria/heap/datos contiguos → documentación “Memory in WebGL”; citeturn4search0turn4search1  
- compilación Emscripten + IL2CPP → “Getting started with WebGL development”; citeturn4search3turn7view3  
- límite estructural de páginas de memoria en Wasm (2^16 páginas) si se menciona límite teórico → especificación WebAssembly. citeturn5search46turn5search2  

**Fuente exacta de respaldo:** citeturn4search0turn4search3turn5search46

### KPIs técnicos insuficientemente especificados (especialmente “tiempo de carga < 10 s”)

**Severidad:** Alto  
**Sección:** KPIs técnicos de referencia (págs. 40–41)  
**Afirmación problemática (literal):** “tiempo de carga total menor a 10 segundos en condiciones de prueba controladas”.

**Por qué sigue siendo problemática:**  
Es un KPI **no falsable** sin condiciones: red (Mbps/latencia), dispositivo, navegador, caché (primera carga vs recurrente), tamaño de .data y compresión, CDN/servidor, etc. Además, Unity WebGL tiene un modelo de carga condicionado por descarga y residencia de datos en memoria (p. ej., .data sin FS real), lo que hace imprescindible declarar el entorno de prueba. citeturn4search0turn4search1  

**Corrección propuesta:**  
Reformular KPI como:  
- “Tiempo total de carga (first load) medido desde ‘start navigation’ hasta ‘first interactive’ bajo: dispositivo X, navegador Y, conexión Z, caché deshabilitada/habilitada, tamaño build A.”  
- Añadir un KPI alternativo robusto: “Tamaño total de descarga comprimida” y “memoria pico” (heap + asset). (Unity describe explícitamente descarga del .data y su permanencia en memoria). citeturn4search0turn4search1  

**Fuente exacta de respaldo:** citeturn4search0turn4search1

## C. Validación de correcciones ya aplicadas

| Punto corregido (vs. auditoría previa) | Estado | Comentario | Fuente de validación |
|---|---|---|---|
| Separación correcta entre directrices DSR (Hevner et al., 2004) y proceso DSRM (Peffers et al., 2007) | Bien corregido | En el PDF se distingue “directrices” vs “modelo procedimental de seis fases”, alineado con DSRM como proceso de 6 pasos. | DSRM 6 pasos descritos en JMIS. citeturn0search3 |
| Reformulación de muestra: objetivo ideal 30, mínimo 8–12, enfoque mixto con predominio cualitativo‑formativo | Bien corregido | La lógica “formativa” con N bajo es defendible; además, existe evidencia clásica de rendimientos decrecientes en detección de problemas con pocos participantes en tests de usabilidad. | Estudios clásicos sobre tamaño muestral en usabilidad (Virzi; Lewis). citeturn10search0turn10search13 |
| Uso de SUS y NASA‑TLX como descriptivo/exploratorio si no se alcanza tamaño mayor | Bien corregido | Es coherente con enfoque de evaluación formativa; si se pretende inferencia, debe justificarse y controlar diseño/orden. | Robustez de SUS en práctica formativa y benchmarking; ejemplo de administración tras tareas. citeturn8view0turn9search2 |
| Corrección de interpretación de 68 como benchmark histórico (no “buena usabilidad”) | Corregido parcialmente | La redacción va en dirección correcta, pero falta citar explícitamente el benchmark y matizar variación (68 vs ~70 según base). | Benchmark “mean 68 (SD 12.5)” reportado explícitamente; datos JUS con media ~70. citeturn9search2turn8view0 |
| Meta “72+” como objetivo deseable | Corregido parcialmente | Puede sostenerse como “por encima de la media” (percentil aproximado) pero no como umbral universal; requiere contextualización (percentiles/curva) y evitar dogmatismo. | Ejemplo de curvas/percentiles y rangos de notas. citeturn3search1turn9search2 |
| Sustitución de premisas groseramente erróneas sobre memoria (p. ej., límites simplistas) por explicación de heap contiguo/fragmentación | Bien corregido (factual), aún insuficiente (citación) | El contenido técnico coincide con documentación Unity (contigüidad/fragmentación), pero debe citarse en el cuerpo donde se afirma. | Unity explica heap contiguo y fallos por fragmentación/contigüidad. citeturn4search0turn4search1 |
| Benchmarking: inclusión de cautelas (no declarar superioridad universal) | Corregido parcialmente | La cautela textual mejora; sin embargo, hay errores factuales en referencias (DOI/venue) y faltan fuentes por afirmación comparativa. | Corrección necesaria por identificación de DOI incorrectos. citeturn14search6turn13search3 |

## D. Matriz de afirmaciones aún débiles o sin respaldo

| Sección | Afirmación | Problema | Acción recomendada | Fuente sugerida |
|---|---|---|---|---|
| Planteamiento del problema | “Modelos CAD… pueden alcanzar cientos de MB o GB” | Generalización sin dato ni rango contextual (tipo de ensamblaje/industria/formato). | Atenuar (“pueden ser muy pesados”) y/o aportar evidencia (estudios de “huge 3D models” y lightweighting). | Revisión sobre lightweighting de modelos grandes para Web3D. citeturn13search12 |
| Justificación técnica | “IL2CPP… C#→C++→WebAssembly” (sin cita en el párrafo) | Factualmente defendible, pero sin rastreabilidad en el lugar donde se afirma. | Insertar cita a documentación Unity WebGL “Technical overview”. | Documentación Unity sobre Emscripten e IL2CPP para Wasm. citeturn4search3 |
| Marco teórico (hipótesis) | “El proyecto busca demostrar/mejorar comprensión espacial” | “Comprensión” no está operacionalizada con métricas objetivas; riesgo de promesa fuerte. | Definir métricas de desempeño (exactitud, tiempo, errores) y limitar a “evidencia formativa”. | JUS advierte necesidad de datos corroborativos (p. ej. éxito en tareas) además de SUS. citeturn8view0 |
| Metodología | “contrastar con soporte 2D equivalente” | Falta diseño experimental explícito (orden/contrabalanceo). | Declarar intra‑sujeto con contrabalanceo y puntos de medida por condición. | Buenas prácticas de administración de SUS tras tareas; TLX como medida estándar. citeturn8view0turn0search2 |
| KPIs técnicos | “carga <10s” | KPI no falsable sin entorno de prueba. | Definir dispositivo, navegador, red, caché, métrica “first interactive”, y tamaños de build. | Unity describe fases de memoria/descarga en WebGL. citeturn4search0 |
| Referencias | DOIs incorrectos detectados | Error factual que invalida verificabilidad. | Corregir DOIs y revalidar toda bibliografía. | Bartlett & Dorribo Camba DOI correcto; Fransson et al. DOI correcto. citeturn13search3turn14search6 |
| URP/PBR | Afirmaciones sobre PBR/ GGX | Generalmente correctas, pero debe citarse URP donde se afirma implementación (GGX/PBS/Lit). | Insertar cita a documentación URP (PBS, GGX, Lit). | Shading models / shaders URP. citeturn6search1turn6search4turn6search12 |
| WebAssembly | Límite/crecimiento de memoria | Si se menciona límite, debe distinguir “límite teórico del core” vs límites de navegador/Unity. | Citar especificación para límite estructural y Unity para límites prácticos del heap en WebGL. | Especificación Wasm (2^16 páginas) + Unity heap hasta 2GB. citeturn5search46turn4search1 |

## E. Matriz de coherencia interna

| Elemento | Evaluación de coherencia | Observación técnica |
|---|---|---|
| Problema → (Pregunta) | **Aún insuficiente** | No hay pregunta explícita; esto debilita trazabilidad de la evaluación. |
| Problema → Objetivo general | **Coherente** | El problema (inspección técnica 2D vs necesidad 3D interactiva) está alineado con construir un visor Web3D. |
| Objetivos específicos → Metodología | **Parcialmente coherente** | Hay instrumentos y tareas, pero el “contraste 3D vs 2D” necesita diseño explícito (orden/condición). |
| Justificación → Elección tecnológica (Unity WebGL/URP) | **Coherente pero mejorable** | La elección es defendible, pero requiere citación directa en pasajes técnicos (pipeline/memoria) para blindaje. citeturn4search0turn4search3turn6search4 |
| Metodología → Resultados esperados | **Parcialmente coherente** | Se prometen conclusiones sobre comprensión/inspección; faltan métricas objetivas o atenuación a evidencia formativa. citeturn8view0 |
| KPIs técnicos → Instrumentación | **Insuficiente** | KPI de carga y umbrales de memoria/draw calls no son reproducibles sin condiciones de prueba y método de medición. citeturn4search0 |

## F. Reescrituras finales listas para pegar

### Pregunta de investigación y subpreguntas

**Pregunta de investigación (propuesta):**  
¿En qué medida un prototipo Web3D interactivo desplegado en navegador (Unity WebGL) permite realizar tareas de inspección y exploración técnica de un ensamblaje mecánico con usabilidad percibida y carga de trabajo subjetiva aceptables, bajo restricciones reales de memoria y rendimiento propias de WebGL?

**Subpreguntas:**  
1) ¿Qué restricciones prácticas de memoria (heap/asset) y rendimiento (frame time) aparecen durante el despliegue WebGL y qué técnicas de optimización resultan más determinantes? (Unity WebGL; memoria y pipeline). citeturn4search0turn4search3  
2) ¿Qué nivel de usabilidad percibida (SUS) obtiene el prototipo y cómo se interpreta respecto de benchmarks (percentiles/curva)? citeturn9search2turn8view0  
3) ¿Qué carga de trabajo subjetiva (NASA‑TLX) reportan los participantes durante tareas específicas del prototipo y cómo se compara con la referencia 2D? citeturn0search2turn0search13  
4) ¿Qué patrones cualitativos (Think‑Aloud) explican errores, fricciones y puntuaciones (SUS/TLX), y qué implicaciones de rediseño se derivan? citeturn11search0turn12search0  

### Ajuste de hipótesis para evitar sobrepromesa

**Texto actual (riesgo):** formulaciones tipo “demostrar” / “validar”.  
**Reescritura recomendada (prudente y defendible):**  
“Desde un enfoque formativo, se **explorará** si un visor 3D interactivo reduce dificultades asociadas a la interpretación de representaciones 2D y facilita la ejecución de tareas de inspección (identificación de piezas, relaciones espaciales y consulta de información), comparado con una referencia 2D equivalente. La evidencia se reportará como indicadores descriptivos (SUS/NASA‑TLX) y como hallazgos cualitativos (Think‑Aloud) que expliquen mecanismos y fricciones observadas, evitando inferencias poblacionales cuando el muestreo no las soporte.”

### Interpretación SUS con anclaje bibliográfico y sin dogma numérico

“Para interpretar SUS, se reportará (i) el puntaje total (0–100), (ii) su lectura como benchmark histórico. En la literatura se reporta como distribución de referencia una media aproximada de 68 (DE 12.5), donde 68 se ubica alrededor del percentil 50; por tanto, valores por encima de ~68–70 se consideran ‘por encima de la media’ en términos comparativos, no como garantía absoluta de ‘buena usabilidad’. Adicionalmente, se presentará una meta operativa como rango (p. ej., ≥70 ‘aceptable’ y ≥73 ‘claramente por encima de la media’), tratándolo como heurística contextual y no como umbral universal.” citeturn9search2turn8view0turn3search1  

### Diseño de comparación 3D vs 2D con control de orden

“Se empleará un diseño intra‑sujeto con contrabalanceo AB/BA para controlar efectos de aprendizaje y fatiga: la mitad de participantes realiza primero tareas con el prototipo 3D y luego con la referencia 2D; la otra mitad en orden inverso. NASA‑TLX se administrará por condición al finalizar las tareas de cada condición; SUS se administrará para el sistema interactivo 3D (o, si excepcionalmente se aplica a 2D, se justificará explícitamente el constructo). Think‑Aloud se llevará de forma concurrente con intervención mínima del moderador y con registro de audio/pantalla y logs de acciones.”

### Corrección de referencias (entradas mínimas)

- Bartlett, K. A., & Dorribo Camba, J. (2023). *The role of a graphical interpretation factor in the assessment of Spatial Visualization: A critical analysis.* Spatial Cognition & Computation, 23(1), 1–30. **DOI:** 10.1080/13875868.2021.2019260. citeturn13search3  
- Fransson, E., Hermansson, J., & Hu, Y. (2024). *A Comparison of Performance on WebGPU and WebGL in the Godot Game Engine.* (GEM 2024). **DOI:** 10.1109/GEM61861.2024.10585437. citeturn14search6turn14search0  

## Cierre de auditoría

Persisten riesgos típicos que un jurado exigente detectará si no se corrigen:

- **Afirmaciones sin fuente en el lugar donde se enuncian**, especialmente en memoria/pipeline WebGL: aunque sean correctas, deben citarse con documentación oficial para blindaje. citeturn4search0turn4search3  
- **Promesas metodológicas excesivas**: palabras como “demostrar/validar” para efectos cognitivos sin métricas objetivas y con N potencialmente bajo.  
- **Sobreventa técnica residual**: metas como “<10 s” sin entorno; “superioridad” implícita si el benchmarking no está adecuadamente trazado. citeturn4search0turn8view0  
- **Saltos lógicos**: “mejorar comprensión” requiere definir evidencia (desempeño en tareas y/o criterios de éxito), no solo percepción (SUS) y carga (TLX). citeturn8view0turn0search2  
- **Higiene bibliográfica**: los DOIs erróneos ya detectados obligan a revisar el 100% de la bibliografía antes de defensa. citeturn13search3turn14search6