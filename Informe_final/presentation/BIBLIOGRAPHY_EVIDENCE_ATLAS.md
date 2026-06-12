# Atlas bibliografico de evidencia para defensa - TwinSight X500

Estado: guia de estudio y trazabilidad oral.
Fecha: 2026-06-12.
Fuente academica: `Informe_final/chapters/07_referencias.tex`.

## Regla de uso y copyright

Este atlas no reemplaza las referencias APA del informe. Sirve para responder "de donde sale esto?" con una ruta local, una pagina PDF o una linea HTML y una cita textual breve. Las citas textuales se mantienen cortas como anclas de localizacion; para exponer, se debe parafrasear y volver al informe.

Las rutas bajo `External_docs/` son rutas locales de estudio. No implican que los PDFs bibliograficos deban publicarse en GitHub; si el jurado pide ver una fuente, se debe abrir la copia local o la URL/DOI oficial correspondiente.

Cuando una fuente aparece sin cita textual verificada, no debe usarse como cita literal durante la sustentacion hasta revisar el PDF o URL original.

## Tabla de anclajes bibliograficos

| Tema defendible | Fuente APA del informe | Archivo local o ruta | Localizacion verificada | Cita textual breve | Como usarla en defensa | Limite de interpretacion |
| --- | --- | --- | --- | --- | --- | --- |
| Carga cognitiva y memoria de trabajo | Sweller, van Merrienboer y Paas (2019) | `External_docs/Bibliografia/sweller2019_article_cognitivearchitectureandinstru.pdf` | PDF p. 2 | "limited working memory which can only process" | Explicar por que la reconstruccion espacial desde documentos 2D puede demandar memoria de trabajo. | No afirmar que NASA-TLX mide carga intrinseca/extrinseca/germana directamente. |
| Demandas innecesarias de informacion | Sweller, van Merrienboer y Paas (2019) | `External_docs/Bibliografia/sweller2019_article_cognitivearchitectureandinstru.pdf` | PDF p. 2 | "unnecessary demands are imposed" | Justificar la reduccion de esfuerzo por contexto visual, seleccion directa y organizacion de informacion. | No concluir causalidad fuerte con n=12. |
| Reduccion de carga durante solucion de problemas | Sweller (1988) | `External_docs/Bibliografia/Sweller - 1988 - Cognitive load during problem solving.pdf` | PDF p. 6 | "substantially decrease cognitive load" | Usarla como antecedente teorico general de diseno que reduce esfuerzo cognitivo. | No presentar el proyecto como prueba educativa longitudinal. |
| Metodo DSRM | Peffers et al. (2007) | `External_docs/Bibliografia/A design science research methodology for information systems research Peffers.pdf` | PDF p. 2 | "design science research methodology (DSRM)" | Defender la estructura problema -> artefacto -> evaluacion -> comunicacion. | No convertir DSR en experimento controlado estadistico. |
| SUS como escala breve | Brooke (1996) | `External_docs/Bibliografia/Sus_a_quick_and_dirty_usability_scale.pdf` | PDF p. 3 | "simple, ten-item scale" | Explicar por que SUS se aplico al prototipo 3D como lectura global de usabilidad. | No usar SUS como comparacion directa 3D vs 2D. |
| SUS como vista global subjetiva | Brooke (1996) | `External_docs/Bibliografia/Sus_a_quick_and_dirty_usability_scale.pdf` | PDF p. 3 | "global view of subjective assessments" | Defender SUS 91,88 como percepcion global favorable, no como eficiencia objetiva. | No decir que 68 es umbral absoluto de aprobacion. |
| SUS en multiples productos | Bangor et al. (2009) | `External_docs/Bibliografia/JUS_Bangor_May2009.pdf` | PDF p. 2 | "variety of products or services" | Explicar que SUS se usa en productos y servicios diversos, por eso es pertinente al prototipo. | Interpretacion descriptiva por muestra no probabilistica. |
| Lectura del promedio SUS | Bangor et al. (2009) | `External_docs/Bibliografia/JUS_Bangor_May2009.pdf` | PDF p. 8 | "average SUS score" | Usar la referencia historica como punto de lectura, no como frontera matematica. | No decir "aprobado" solo por superar 68. |
| NASA-TLX multidimensional | Hart (2006) | `External_docs/Bibliografia/NASA-Task Load Index (NASA-TLX) 20 Years Later.pdf` | PDF p. 2 | "multi-dimensional scale designed" | Explicar NASA-TLX Raw como carga de trabajo percibida, no como medicion fisiologica. | No confundir workload con carga cognitiva teorica directa. |
| NASA-TLX tiene seis escalas | Hart y Staveland (1988) | `External_docs/Bibliografia/Development of NASA-TLX.pdf` | PDF p. 7 | "six component scales" | Defender el promedio de seis dimensiones en Raw TLX. | No aplicar ponderacion pareada si el informe usa Raw TLX. |
| NASA-TLX y ausencia de redline universal | Hart (2006) | `External_docs/Bibliografia/NASA-Task Load Index (NASA-TLX) 20 Years Later.pdf` | PDF p. 5 | "defining a useful redline" | Justificar por que no se usa un umbral universal de aceptabilidad. | No vender 8,69 como "carga ideal" universal. |
| Ratings subjetivos y workload | Hart y Staveland (1988) | `External_docs/Bibliografia/Development of NASA-TLX.pdf` | PDF p. 5 | "subjective ratings may come closest" | Explicar por que se aceptan instrumentos subjetivos en workload. | No reemplaza mediciones fisiologicas o cognitivas directas. |
| Significadores de interfaz | Norman (2013) | `External_docs/Bibliografia/the design of everyday things Don Norman.pdf` | PDF p. 15 | "Signifiers are the most important addition" | Defender iconos, bottom sheet, estados y feedback como ayudas de interpretacion. | No reducir UX a estetica visual. |
| Aprendizaje multimedia como palabras + imagenes | Mayer, fuente local de apoyo | `External_docs/Bibliografia/The_Cambridge_Handbook_of_Multimedia_Learning.pdf` | PDF p. 24 | "build mental representations from words and pictures" | Fuente de apoyo para explicar al jurado no tecnico por que visual y texto se complementan. | Fuente de apoyo: no se debe citar como bibliografia canonica si no esta en `07_referencias.tex`. |
| Web3D en aplicaciones web | Yu et al. (2023) | `External_docs/Bibliografia/A survey of real-time rendering on Web3D application Geng YU.pdf` | PDF p. 2 | "3D visualization technology has started to integrate into the web" | Contextualizar el uso de visualizacion 3D en navegador. | No implica que cualquier dispositivo soporte el mismo rendimiento. |
| WebGL y canvas | Khronos Group (2011) | `External_docs/Bibliografia/WebGL Specification Khronos.html` | HTML abstract, lineas aprox. 76-83 | "HTML 5 canvas element" | Explicar WebGL como contexto de render dentro del navegador. | No prometer capacidades identicas a nativo. |
| WebGL y OpenGL ES | Khronos Group (2011) | `External_docs/Bibliografia/WebGL Specification Khronos.html` | HTML abstract, lineas aprox. 82-83 | "conforms closely to the OpenGL ES 2.0 API" | Defender la base tecnica de renderizado web. | La especificacion no valida automaticamente rendimiento del prototipo. |
| SUS como satisfaccion post-test | Sauro y Lewis (2016) | `External_docs/Bibliografia/dokumen.pub_quantifying-the-user-experience-practical-statistics-for-user-research-2nd.pdf` | PDF p. 14 | "posttest user satisfaction with a short set of ratings" | Explicar por que SUS se aplica despues de interactuar con el prototipo. | No usarlo para inferencia causal con esta muestra. |
| 3D user interfaces | Bowman et al. (2004) | `External_docs/Bibliografia/3D User Interfaces Theory and Practice Bowman Kruijff.pdf` | Cita textual no verificada localmente | Sin cita literal verificada | Usar como fuente conceptual para interaccion 3D, navegacion, seleccion y manipulacion. | Revisar pagina exacta antes de citar textual ante jurado. |
| HCI y evaluacion con usuarios | Lazar et al. (2017) | `External_docs/Bibliografia/Research Methods in Human Computer Interaction by Jonathan Lazar, Jinjuan Feng and Harry Hochheiser (Auth.) (z-lib.org).pdf` | Cita textual no verificada localmente | Sin cita literal verificada | Usar como respaldo general de metodos HCI y estudios con usuarios. | No usar como frase textual sin pagina. |
| Design science en IS | Hevner et al. (2004) | `Investigacion/Notas_Conceptuales/Fuentes/SRC_hevner2004design.md` | Nota local parcial, sin PDF completo verificado | Sin cita literal verificada | Usar junto con Peffers para justificar el enfoque de artefacto. | Peffers es la fuente textual fuerte disponible localmente. |
| WebGL/Unity memoria y build | Unity Technologies, fuentes oficiales registradas | `Investigacion/Notas_Conceptuales/Fuentes/SRC_unity2024memory.md`; `SRC_unity2024webglgettingstarted.md` | URLs oficiales registradas, sin texto completo local | Sin cita literal verificada | Usar como respaldo tecnico de restricciones WebGL y memoria si el jurado pregunta. | Revisar documentacion oficial en linea antes de citar literalmente. |

## Respuestas rapidas por pregunta bibliografica

**"De donde sale la idea de carga cognitiva?"**

Del marco teorico de Sweller. La defensa debe decir: la tesis no mide carga cognitiva teorica directa; usa NASA-TLX Raw como workload subjetivo y lo conecta con la idea de reducir reconstruccion espacial innecesaria.

**"Por que SUS?"**

Brooke define SUS como escala simple de diez items y Bangor/Sauro/Lewis ayudan a interpretar el puntaje. En el informe se aplica solo al prototipo 3D.

**"Por que NASA-TLX Raw?"**

Hart y Staveland proponen seis dimensiones de workload. La variante Raw evita ponderacion pareada y sirve para una lectura formativa rapida.

**"Por que DSR?"**

Peffers et al. soportan una metodologia centrada en artefactos: problema, objetivo, diseno/desarrollo, demostracion/evaluacion y comunicacion.

**"Por que WebGL?"**

Khronos permite ubicar WebGL como contexto de renderizado para `canvas`; Yu et al. ubican el crecimiento de visualizacion 3D en la web. La tesis agrega evidencia tecnica propia de rendimiento por dispositivo.
