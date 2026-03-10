# Guía de Sustentación Oral y Defensa de Propuesta

Este documento sirve como guía de preparación para la sustentación oral de la propuesta de tesis. Incluye posibles preguntas de los jurados (técnicos y metodológicos) y las respuestas sugeridas basadas en el documento.

---

## 1. Planteamiento y Justificación

**Jurado:** "¿Por qué es necesario este proyecto si ya existen visores 3D como Sketchfab o Marmoset?"
**Sustentación:** "Aunque existen herramientas excelentes como Sketchfab, estas son plataformas cerradas (SaaS) que limitan la personalización de la lógica de interacción y la integración con sistemas de datos externos. Mi propuesta busca desarrollar un *pipeline* y un prototipo que permita control total sobre la interactividad (ej. despiece paramétrico, simulación de ensamblaje) y la optimización específica para hardware técnico, algo que las plataformas genéricas no siempre permiten sin costos elevados o restricciones de licencia. Además, el enfoque académico aquí es validar cómo estas interacciones específicas reducen la carga cognitiva, más allá de la simple visualización."

**Jurado:** "¿Cuál es el problema real que está resolviendo? ¿No es solo 'hacer una página web bonita'?"
**Sustentación:** "El problema no es estético, es cognitivo y comunicativo. La documentación técnica actual (PDFs, manuales 2D) impone una alta 'carga cognitiva extrínseca' al usuario, obligándolo a realizar rotaciones mentales complejas para entender cómo encajan las piezas. Esto lleva a errores de interpretación en ingeniería. Mi proyecto busca resolver este problema de comunicación técnica utilizando WebGL para externalizar esa carga cognitiva, permitiendo que el usuario manipule el objeto en lugar de imaginarlo."

## 2. Estado del Arte y Tecnología

**Jurado:** "¿Por qué eligió Unity WebGL en lugar de Three.js, que es más ligero y nativo para la web?"
**Sustentación:** "Es una excelente pregunta. Three.js es fantástico y ligero, pero para aplicaciones de ingeniería que requieren manejo de modelos CAD complejos, Unity ofrece ventajas críticas:
1.  **Pipeline de Importación:** Unity maneja nativamente formatos complejos y tiene herramientas robustas para optimización de mallas (LODs automáticos).
2.  **WebAssembly (WASM):** Unity compila C# a WASM, lo que ofrece un rendimiento más predecible y cercano al nativo para cálculos físicos o lógicos complejos, comparado con JavaScript puro.
3.  **Herramientas Visuales:** Permite iterar más rápido en el diseño de la interfaz y la lógica visual (Shader Graph) sin escribir shaders GLSL desde cero.
El 'costo' es un tamaño de carga inicial mayor, pero he mitigado esto en la propuesta mediante el uso de *Asset Bundles* para carga bajo demanda."

**Jurado:** "Menciona WebGPU en su documento. ¿Por qué no usar WebGPU si es el futuro?"
**Sustentación:** "WebGPU es definitivamente el futuro y ofrece mejor rendimiento, como cito en el trabajo de Fransson et al. (2024). Sin embargo, el soporte en navegadores móviles y de escritorio aún no es universal (100%). Para una herramienta que busca accesibilidad inmediata en la industria hoy, WebGL 2.0 sigue siendo el estándar más compatible y estable. El diseño del proyecto en Unity facilita una migración futura a WebGPU simplemente cambiando el backend gráfico, sin reescribir la lógica."

## 3. Marco Teórico

**Jurado:** "Explique brevemente cómo aplica la Teoría de Carga Cognitiva en su diseño."
**Sustentación:** "La teoría de Sweller divide la carga en tres: intrínseca (dificultad del tema), extrínseca (cómo se presenta) y germana (aprendizaje).
No puedo cambiar la carga intrínseca (el motor es complejo per se).
Mi objetivo es reducir la **carga extrínseca**. Lo hago eliminando la necesidad de rotación mental (el usuario rota el modelo, no su mente) y aplicando el **Principio de Contigüidad Espacial** de Mayer: las etiquetas y explicaciones aparecen *sobre* o *junto* a la parte del modelo, no en una lista separada al pie de página, evitando que el usuario divida su atención."

## 4. Metodología

**Jurado:** "¿Por qué Design Science Research (DSR) y no una metodología experimental tradicional?"
**Sustentación:** "Porque el objetivo principal es crear un **artefacto** (el prototipo de software) que resuelva un problema práctico. DSR es ideal para ingeniería y sistemas porque valida el conocimiento a través de la construcción y evaluación del artefacto. No estoy solo observando un fenómeno (ciencia natural), estoy diseñando una solución. La metodología iterativa (Sprints) encaja perfectamente con el ciclo de 'Generar Diseño -> Construir -> Evaluar' de DSR."

**Jurado:** "¿Cómo va a validar que su prototipo realmente funciona?"
**Sustentación:** "La validación es mixta:
1.  **Técnica (Cuantitativa):** Mediré FPS, uso de memoria y tiempos de carga usando el Profiler de Unity y herramientas de desarrollo del navegador para asegurar que cumple los KPIs (ej. >30 FPS en móvil).
2.  **Usuario (Cualitativa/Cuantitativa):** Realizaré pruebas con usuarios reales (ingenieros/estudiantes) usando el cuestionario SUS (System Usability Scale) para usabilidad y posiblemente el NASA-TLX para medir la carga cognitiva percibida al realizar una tarea de identificación de partes."

## 5. Cronograma y Viabilidad

**Jurado:** "El cronograma parece ajustado. ¿Qué pasa si la optimización de los modelos 3D toma más tiempo del esperado?"
**Sustentación:** "Es un riesgo válido. Por eso he asignado el Sprint 2 completo (Mes 2) específicamente al 'Arte Técnico' (Retopología y Baking). Además, la metodología iterativa me permite reducir el alcance si es necesario: puedo validar el pipeline con un solo sub-ensamblaje complejo en lugar del motor completo, y el aporte científico (el pipeline de optimización) seguiría siendo válido."

**Jurado:** "¿Tiene acceso al hardware necesario para las pruebas?"
**Sustentación:** "Sí, cuento con un equipo de desarrollo con GPU dedicada para la producción y acceso a dispositivos móviles de gama media para las pruebas de validación de los KPIs de rendimiento."

---

## Tips Adicionales para la Presentación

*   **Sea honesto:** Si no sabe una respuesta técnica profunda, no invente. Diga: "Es un punto interesante que no he explorado a fondo, pero mi hipótesis basada en [X] es..."
*   **Enfoque en el valor:** Siempre vuelva a *por qué* esto es útil para un ingeniero o estudiante. No se pierda solo en los polígonos y shaders.
*   **Demo:** Si es posible, tenga videos cortos o GIFs de los prototipos (incluso si son básicos) en las diapositivas. "Show, don't just tell".
