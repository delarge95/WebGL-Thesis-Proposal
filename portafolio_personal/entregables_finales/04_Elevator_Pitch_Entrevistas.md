# Cheat Sheet Entrevistas (Junior/Mid Tech Art / Tools)

*(Guía rápida para contestar preguntas típicas usando tu experiencia en el desarrollo del Portafolio del Dron Holybro).*

---

### PREGUNTA 1: "Cuéntame de algún problema complejo que hayas resuelto y cómo lo abordaste."
**LA TRAMPA:** Quieren ver *cómo piensas* bajo restricciones, no qué botón apretaste en Blender. Quieren escuchar "Pipeline", "Memoria" y "Headless Testing".
**TU RESPUESTA:** 
>"El reto más grande en mi visor WebGL fue lidiar con el Context Loss del navegador y el overhead de Main Thread por el CAD original (257 renderers). Para evitar que mis artistas/ingenieros configuraran errores manualmente, construí `ImportedDroneCoverageAudit`. Es un validador en C# que audita si la malla hace match con nuestro CSV maestro de referencias antes si quiera de dar Play. Además, eliminé los cuellos de botella del CPU implementando un JSON-Metadata Instancing para la tornillería extrema, bajando draw calls de cientos a decenas."

---

### PREGUNTA 2: "Imagina un Asset que gasta demasiados recursos. ¿Cómo lo optimizarías para mobile/WebGL?"
**LA TRAMPA:** Decir "Reduzco los polígonos" suena muy Junior. Un Mid ataca *Draw Calls*, *Heap Limits* y *Overdraw*.
**TU RESPUESTA:**
>"En WebGL el problema usualmente no es la GPU per-se, sino el CPU Dispatch (límite del main thread de Javascript). Primero abro el Unity Profiler conectado a mi build en navegador o uso el Memory Profiler si rompo el Heap Limit (~2GB). Si las Draw Calls estallan por un asset pesado (como el Drone Holybro), instancio procedimentalmente en vez de agrupar jerárquicamente. Si sufro por fill-rate, busco dónde un Pass de Transparencia o Stencil me hace un Overdraw fatal e intento usar un world-space clipping plane con FIFO como hice en mi shader de PBR Cross-Section."

---

### PREGUNTA 3: "Si una herramienta o un proceso falla, ¿Cómo haces Debugging?"
**LA TRAMPA:** Quieren ver si eres sistemático aislando errores de memoria y si sufres dolor de producción.
**TU RESPUESTA:**
>"Aislamiento radical. Por ejemplo, en mi sistema térmico híbrido sufría problemas para pasar matrices de temperatura individuales sin romper la agrupación (instancing). Aislé un tornillo en una Sandbox, lo pegué a un Frame Debugger de escritorio para revisar el Draw Call batching y noté que los `MaterialPropertyBlocks` me salvaban la vida. Trato siempre de reproducir los bloqueos en el hardware objetivo de menos potencia; en WebGL, testo Safari Mobile desde temprano para cazar leaks de memoria."

---

### PREGUNTA 4: "¿Cómo garantizas que tu UX visual sea performante en entornos restrictivos?"
**LA TRAMPA:** Esperan que hables de Atlas de texturas UI o Canvas Batching básico.
**TU RESPUESTA:**
>"Para el onboarding del prototipo, mi restricción era 0 MB de videos (matan la VRAM WebGL). Construí un `OnboardingAnimationView` que dibuja las interacciones proceduralmente en runtime usando curvas matemáticas la API `Painter2D` de Unity Toolkit. Costo de textura: nulo. Adicionalmente, manejé la cámara mediante un controlador adaptativo: recalcula sensibilidad base, panning curvo y clipping distances leyendo las *bounds* volumétricas de la pieza. Una cámara no puede escrollear igual un chasis de 50cm que una tuerca M3 de 3mm."

---

## 😈 ROUND DIABÓLICO (Preguntas Señorísimas)

### PREGUNTA D1: "¿Cómo resuelves que cada tornillo en pantalla tenga su propia temperatura térmica sin reventarme las draw calls con Batching estático?"
**LA TRAMPA:** Evitar el break del instancing en Renderizado avanzado.
**TU RESPUESTA:**
>"Mantuve el Instancing por GPU enviando los datos específicos por instancia usando `MaterialPropertyBlock` seteados por un script controlador, inyectando un ID único si era necesario para buscar en texturas (o buffers estructurados, si el dispositivo/WebGL 2 lo permite holgadamente). Así, todos los tornillos comparten una misma invocación del Material/Pass, pero sus gradientes térmicos se matizan internamente desde el HLSL usando esa inyección uniforme por frame."

### PREGUNTA D2: "Veo tus Custom Tool Windows (C#). Muy bonitas pero, ¿cómo evitarían que mi equipo de Artistas Junior suba basura al Repo hoy en la noche?"
**LA TRAMPA:** El Lead Pipeline no confía en humanos. Confía en el CI/CD.
**TU RESPUESTA:**
>"Mis validadores (`CoverageAudit`) están diseñados desacoplados de la GUI interna. Fueron estructurados explícitamente para poder correr como tests de Headless Unity o como hooks pre-commit de tipo unit test. Si un Artista cambia un prefab y rompe el link semántico, el Runner lo detectaría sin obligar a nadie a abrir esa ventana bonita en su propio ordenador."

---

### PREGUNTA FINAL: "¿Por qué deberíamos contratarte para Tools/QA Pipeline/Tech si vienes haciendo Tesis de Rendering 3D WebGL?" (El Troyano)
**TU RESPUESTA:**
>"Hacer la Tesis de Rendering 3D en la plataforma más restrictiva que existe (WebGL y navegadores móviles) te destruye la paciencia para el trabajo manual. Como Ingeniero, perdía horas configurando CSVs y lidiando con crashes por Memoria OOM en Safari. Entendí rápido que no saco nada pintando un shader genial si el flujo de QA me ahoga en el intento. Desarrollo automatizaciones (CI/CD / ARA python frameworks) y Tools de Unity *precisamente* porque detesto el sufrimiento operativo y quiero que ustedes —el Core Team— dediquen las horas a crear el juego en sí, en lugar de arreglar por vez milésima la jerarquía rota que importó un third-party."
