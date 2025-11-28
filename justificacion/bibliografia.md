# Análisis Bibliográfico Detallado

Este documento presenta un análisis detallado de las fuentes bibliográficas utilizadas en la propuesta de tesis, justificando su selección, relevancia y aplicación en el documento.

## Akenine-Möller, T., Haines, E., & Hoffman, N. (2018)
**Cita:** Akenine-Möller, T., Haines, E., & Hoffman, N. (2018). *Real-Time Rendering* (4th ed.). CRC Press.
**Año:** 2018
**Enlace:** N/A (Libro impreso/eBook)
**Resumen:** La "biblia" del renderizado en tiempo real. Cubre exhaustivamente los fundamentos matemáticos y algorítmicos del pipeline gráfico, incluyendo rasterización, sombreado, y técnicas avanzadas de iluminación global.
**Relevancia:** Fundamental para fundamentar las decisiones técnicas sobre el pipeline de renderizado WebGL y las técnicas de optimización gráfica (LOD, culling).
**Uso en el documento:** Citado en el Marco Teórico como referencia base para los conceptos de renderizado y optimización gráfica.

## Bartlett, K. A., & Dorribo Camba, J. (2023)
**Cita:** Bartlett, K. A., & Dorribo Camba, J. (2023). The Role of a Graphical Interpretation Factor in the Assessment of Spatial Visualization: A Critical Analysis. *Spatial Cognition & Computation*.
**Año:** 2023
**Enlace:** [https://doi.org/10.1080/13875868.2021.1987375](https://doi.org/10.1080/13875868.2021.1987375)
**Resumen:** Investigación reciente que analiza cómo la interpretación gráfica influye en la evaluación de habilidades espaciales, sugiriendo que las ayudas visuales interactivas pueden mitigar las diferencias individuales.
**Relevancia:** Proporciona evidencia empírica actual (2023) que respalda la hipótesis de que la visualización 3D interactiva facilita la comprensión espacial, clave para la justificación del proyecto.
**Uso en el documento:** Citado en el Marco Teórico para validar la utilidad de modelos 3D interactivos en la reducción de la carga cognitiva asociada a la visualización espacial.

## Bowman, D. A., et al. (2004)
**Cita:** Bowman, D. A., Kruijff, E., LaViola Jr, J. J., \& Poupyrev, I. (2004). *3D User Interfaces: Theory and Practice*. Addison-Wesley.
**Año:** 2004
**Enlace:** N/A (Libro)
**Resumen:** Texto seminal sobre diseño de interfaces de usuario 3D, cubriendo técnicas de selección, manipulación y navegación en entornos virtuales.
**Relevancia:** Base teórica para el diseño de la interacción en el prototipo (rotación orbital, zoom, selección de componentes).
**Uso en el documento:** Referenciado en el Marco Teórico y Metodología para justificar las metáforas de interacción seleccionadas.

## Burley, B. (2012)
**Cita:** Burley, B. (2012). Physically-Based Shading at Disney. *ACM SIGGRAPH 2012 Course Notes*.
**Año:** 2012
**Enlace:** [https://media.disneyanimation.com/uploads/production/publication_asset/48/asset/s2012_pbs_disney_brdf_notes_v3.pdf](https://media.disneyanimation.com/uploads/production/publication_asset/48/asset/s2012_pbs_disney_brdf_notes_v3.pdf)
**Resumen:** Introduce el modelo de sombreado "Disney BRDF", que estandarizó el PBR (Physically Based Rendering) en la industria, simplificando los parámetros de materiales a valores intuitivos (metalness, roughness).
**Relevancia:** El proyecto utiliza Unity URP que implementa un modelo PBR derivado de este trabajo. Es crucial para lograr la fidelidad visual requerida en la visualización de hardware.
**Uso en el documento:** Citado en el Marco Teórico al explicar el modelo de iluminación y materiales utilizado.

## Cook, R. L., & Torrance, K. E. (1982)
**Cita:** Cook, R. L., & Torrance, K. E. (1982). A reflectance model for computer graphics. *ACM Transactions on Graphics*.
**Año:** 1982
**Enlace:** [https://doi.org/10.1145/357290.357293](https://doi.org/10.1145/357290.357293)
**Resumen:** El paper fundacional del renderizado basado en física (microfacet theory), estableciendo cómo la luz interactúa con superficies rugosas y metálicas.
**Relevancia:** Fundamento teórico primario del realismo visual en gráficos por computadora.
**Uso en el documento:** Citado en el Marco Teórico como el origen de los modelos de iluminación modernos utilizados en WebGL.

## Cowan, N. (2001)
**Cita:** Cowan, N. (2001). The magical number 4 in short-term memory: A reconsideration of mental storage capacity.
**Año:** 2001
**Enlace:** N/A
**Resumen:** Actualiza la teoría de Miller, sugiriendo que la capacidad de la memoria de trabajo es más limitada (aprox. 4 items) de lo que se pensaba.
**Relevancia:** Refuerza la necesidad de minimizar la carga cognitiva extrínseca en la interfaz, ya que la memoria de trabajo del usuario es un recurso escaso.
**Uso en el documento:** Citado en el Marco Teórico en la sección de Carga Cognitiva.

## Darken, R. P., & Sibert, J. L. (1996)
**Cita:** Darken, R. P., & Sibert, J. L. (1996). Wayfinding Strategies and Behaviours in Large Virtual Worlds. *CHI '96*.
**Año:** 1996
**Enlace:** [https://doi.org/10.1145/238386.238459](https://doi.org/10.1145/238386.238459)
**Resumen:** Estudio clásico sobre navegación (wayfinding) en entornos virtuales, identificando estrategias y problemas comunes de desorientación.
**Relevancia:** Informa el diseño de la navegación del prototipo para evitar que el usuario se pierda al inspeccionar el modelo 3D.
**Uso en el documento:** Referenciado en el Marco Teórico sobre navegación y orientación espacial.

## Fransson, E., et al. (2024)
**Cita:** Fransson, E., Hermansson, J., & Hu, Y. (2024). A Comparison of Performance on WebGPU and WebGL in the Godot Game Engine. *IEEE CoG 2024*.
**Año:** 2024
**Enlace:** [https://doi.org/10.1109/CoG60054.2024.10645582](https://doi.org/10.1109/CoG60054.2024.10645582)
**Resumen:** Comparativa técnica reciente entre WebGL y WebGPU, demostrando las ventajas de rendimiento de WebGPU en motores de juego modernos.
**Relevancia:** Justifica la elección tecnológica y sitúa el proyecto en la frontera del conocimiento actual, reconociendo a WebGL como el estándar actual pero mirando hacia WebGPU.
**Uso en el documento:** Citado en el Estado del Arte para discutir el futuro del renderizado web y las limitaciones actuales.

## Hegarty, M. (2004)
**Cita:** Hegarty, M. (2004). Mechanical reasoning by mental simulation. *Trends in Cognitive Sciences*.
**Año:** 2004
**Enlace:** N/A
**Resumen:** Explora cómo las personas razonan sobre sistemas mecánicos mediante simulación mental y cómo las visualizaciones externas pueden apoyar este proceso.
**Relevancia:** Directamente aplicable a la visualización de hardware (drones, motores), justificando por qué una visualización interactiva es superior a diagramas estáticos.
**Uso en el documento:** Citado en el Marco Teórico y Planteamiento del Problema.

## Hevner, A. R., et al. (2004)
**Cita:** Hevner, A. R., March, S. T., Park, J., & Ram, S. (2004). Design Science in Information Systems Research. *MIS Quarterly*.
**Año:** 2004
**Enlace:** N/A
**Resumen:** Establece el marco metodológico del "Design Science Research" (DSR), enfocado en la creación y evaluación de artefactos IT para resolver problemas prácticos.
**Relevancia:** Define la metodología de investigación del proyecto (creación de un artefacto - el prototipo - para generar conocimiento).
**Uso en el documento:** Citado en la sección de Metodología como el marco metodológico principal.

## Khronos Group (2011)
**Cita:** Khronos Group. (2011). *WebGL Specification 1.0*.
**Año:** 2011
**Enlace:** [https://www.khronos.org/registry/webgl/specs/latest/1.0/](https://www.khronos.org/registry/webgl/specs/latest/1.0/)
**Resumen:** Especificación técnica oficial de WebGL.
**Relevancia:** Fuente primaria para la definición técnica de la tecnología base del proyecto.
**Uso en el documento:** Citado en el Estado del Arte y Marco Conceptual.

## Mayer, R. E. (2005, 2021)
**Cita:** Mayer, R. E. (2005/2021). *The Cambridge Handbook of Multimedia Learning*.
**Año:** 2005, 2021
**Enlace:** [https://doi.org/10.1017/9781108894333](https://doi.org/10.1017/9781108894333)
**Resumen:** Compendio de la Teoría Cognitiva del Aprendizaje Multimedia (CTML), principios de diseño multimedia (coherencia, señalización, contigüidad espacial).
**Relevancia:** Proporciona los principios de diseño instruccional que guían el desarrollo de la interfaz y la presentación de la información técnica.
**Uso en el documento:** Citado extensamente en el Marco Teórico y utilizado para justificar decisiones de diseño en la Metodología.

## Cabello, R. [mrdoob] (2024)
**Cita:** Cabello, R. [mrdoob]. (2024). *three.js - JavaScript 3D Library*.
**Año:** 2024
**Enlace:** [https://github.com/mrdoob/three.js](https://github.com/mrdoob/three.js)
**Resumen:** Repositorio oficial de Three.js, la biblioteca WebGL más popular.
**Relevancia:** Evidencia la popularidad y madurez de las tecnologías WebGL.
**Uso en el documento:** Citado en el Estado del Arte para el benchmarking de tecnologías.

## Nielsen, J. (1994, 2000)
**Cita:** Nielsen, J. (1994). *Usability Engineering*; Nielsen, J. (2000). Why You Only Need to Test with 5 Users.
**Año:** 1994, 2000
**Enlace:** [https://www.nngroup.com/articles/why-you-only-need-to-test-with-5-users/](https://www.nngroup.com/articles/why-you-only-need-to-test-with-5-users/)
**Resumen:** Fundamentos de la ingeniería de usabilidad y justificación del tamaño de muestra para pruebas cualitativas de usabilidad.
**Relevancia:** Justifica el tamaño de la muestra (N=8-12) propuesto en la metodología y los métodos de evaluación (heurísticas).
**Uso en el documento:** Citado en la Metodología (Diseño de la Validación).

## Norman, D. A. (2013)
**Cita:** Norman, D. A. (2013). *The Design of Everyday Things*.
**Año:** 2013
**Enlace:** N/A
**Resumen:** Principios fundamentales de diseño centrado en el usuario (affordances, signifiers, feedback).
**Relevancia:** Guía el diseño de la interfaz de usuario para asegurar que sea intuitiva y usable.
**Uso en el documento:** Citado en el Marco Teórico (Interacción Humano-Computador).

## Paivio, A. (1986)
**Cita:** Paivio, A. (1986). *Mental Representations: A Dual Coding Approach*.
**Año:** 1986
**Enlace:** N/A
**Resumen:** Teoría de Codificación Dual: el cerebro procesa información verbal y visual por canales separados.
**Relevancia:** Justifica el uso combinado de modelos 3D (visual) y etiquetas/texto (verbal) para mejorar la comprensión y retención.
**Uso en el documento:** Citado en el Marco Teórico como fundamento cognitivo.

## Ries, E. (2011)
**Cita:** Ries, E. (2011). *The Lean Startup*.
**Año:** 2011
**Enlace:** N/A
**Resumen:** Metodología de desarrollo iterativo basada en el ciclo "Construir-Medir-Aprender" y el concepto de MVP (Producto Mínimo Viable).
**Relevancia:** Informa la estrategia de desarrollo ágil del prototipo propuesta en el cronograma.
**Uso en el documento:** Citado en la Metodología.

## Sweller, J. (1988, 2019)
**Cita:** Sweller, J. (1988). Cognitive load during problem solving; Sweller, J., et al. (2019). Cognitive architecture and instructional design.
**Año:** 1988, 2019
**Enlace:** [https://doi.org/10.1007/s10648-019-09465-5](https://doi.org/10.1007/s10648-019-09465-5)
**Resumen:** Teoría de la Carga Cognitiva: tipos de carga (intrínseca, extrínseca, germana) y cómo el diseño instruccional puede optimizarlas.
**Relevancia:** Eje central del Marco Teórico; el objetivo del proyecto es reducir la carga extrínseca mediante visualización 3D.
**Uso en el documento:** Citado extensamente en el Marco Teórico.

## Unity Technologies (2021, 2024)
**Cita:** Documentación oficial y cursos de Unity.
**Año:** 2021, 2024
**Enlace:** [https://docs.unity3d.com/](https://docs.unity3d.com/)
**Resumen:** Documentación técnica sobre WebGL, gestión de memoria y renderizado en Unity.
**Relevancia:** Fuente primaria para los detalles técnicos de implementación y optimización en Unity WebGL.
**Uso en el documento:** Citado en Estado del Arte y Marco Conceptual.

## Yu, G., et al. (2023)
**Cita:** Yu, G., et al. (2023). A survey of real-time rendering on Web3D application.
**Año:** 2023
**Enlace:** [https://doi.org/10.1016/j.vrih.2022.04.002](https://doi.org/10.1016/j.vrih.2022.04.002)
**Resumen:** Estado del arte reciente sobre renderizado Web3D, cubriendo tecnologías, desafíos y tendencias.
**Relevancia:** Proporciona el contexto actual del campo y valida la relevancia de WebGL y WebGPU.
**Uso en el documento:** Citado en el Estado del Arte para contextualizar la investigación.
