# Cheat Sheet Entrevistas (Junior Tech Art / Tools)

*(Guía rápida para contestar preguntas típicas usando tu experiencia en el desarrollo del Portafolio del Dron Holybro).*

---

### PREGUNTA 1: "Cuéntame de algún problema complejo que hayas resuelto y cómo lo abordaste."

**LA TRAMPA:** Quieren ver *cómo piensas* bajo restricciones, no quieren saber qué botón apretaste en Blender. Quieren escuchar las palabras "Pipeline", "Restricciones" y "Automatización".

**TU RESPUESTA (Pitch del Holybro):** 
>"El reto más grande en mi proyecto reciente (visor WebGL para dron PBR) fue manejar la importación del CAD. Tenía un modelo inmenso, pesado, y con una estructura inmanejable importada en Unity con 257 renderers diferentes. 
>Hacerlo manual era inviable. Así que construí herramientas en C#, como el `ImportedDroneRuntimeBinder`. Este script me permitía tomar la jerarquía rota proveniente de la importación, repararla en *runtime* y usar instanciación procedural para detalles como tornillería (fasteners), ahorrando enormes cantidades de memoria *y* liberando mi tiempo de tareas tediosas. Lo volví predecible y optimizado para navegador."

---

### PREGUNTA 2: "Imagina un Asset que gasta demasiados recursos. ¿Cómo lo optimizarías para mobile/WebGL?"

**LA TRAMPA:** Si dices "Reduzco los polígonos", suenas como un principiante de Modelado 3D. Un Tech Artist ataca *Draw Calls* y *Fill Rate*.

**TU RESPUESTA (Pitch del Tech Artist):**
>"Depende del cuello de botella, y primero revisaría el Profiler. Pero usualmente en WebGL, priorizo bajar las *Draw Calls* y controlar la memoria de textura (VRAM). 
>1) Empiezo haciendo Texture Atlasing masivo, uniendo materiales para que compartan la textura y bajen las llamadas. 
>2) Luego, busco donde un material "Masked" es preferible ante un material con "Alpha/Transparencia" complejo.
>3) Geométricamente, como hice con el Dron Holybro, manejo LOD groups o instancing de metadatos (como mi sistema de Fasteners) que evitan tener millones de triángulos renderizando por parte hasta que el usuario hace zoom *y solicita ese detalle*."

---

### PREGUNTA 3: "Si una herramienta o un proceso falla, ¿Cómo haces Debugging?"

**LA TRAMPA:** Quieren ver si eres tolerante a la frustración y usas metodologías sistemáticas.

**TU RESPUESTA:**
> "Siempre parto del *aislamiento*. Cuando los Shaders termales fallaban (mi sistema híbrido), o si un material PBR no clipeaba bien, aislé un pequeño cubo en una sub-escena o render pipeline, desconecto módulos de lógica C# hasta llegar a la unidad fundamental. Si eso funciona, escalo desde ahí. Además, suelo construir mis propias alertas en Editor, por ejemplo creé mi propio `ImportedDroneCoverageAudit` (un C# window script) precisamente para reportarme qué pasaba si faltaban assets en la escena, antes de lanzar el modo *Play*".

---

### PREGUNTA 4: "¿Cómo garantizas que tu UI sea performante?"

**LA TRAMPA:** Esperan que hables de Atlas de texturas o Canvas Batching. Sorpréndelos con tu enfoque programático.

**TU RESPUESTA:**
> "Para el Holybro Viewer, necesitaba un onboarding visual. La ruta fácil era subir un GIF pesado y ahogar la memoria en WebGL. En lugar de eso, desarrollé un `OnboardingAnimationView` que dibuja las interacciones proceduralmente en runtime usando la API `Painter2D` de Unity Toolkit. Animé trayectorias y clics matemáticamente. Costo de memoria: nulo. Además, mi `EnvironmentController` lee luminancia: si el fondo 3D se vuelve muy blanco, no duplico paneles, el C# emite un evento de contraste que invierte las clases CSS de UI Toolkit en runtime."

---

### PREGUNTA 5: "¿Por qué deberíamos contratarte para QA Automation / Tools Junior si vienes haciendo 3D WebGL?" (El Troyano)

**LA TRAMPA:** Saben que quieres hacer juegos o estar cerca de los artistas. Quieren saber si de verdad entiendes de automatización y no vas a renunciar al mes de aburrimiento.

**TU RESPUESTA:**
>"Me considero un Ingeniero con sensibilidad Visual. Sé lo frustrante que es invertir 100 horas haciendo piezas artísticas para que un *bug* de pipeline rompa los *LODs*, o se corrompan los *UVs* o las nomenclaturas estén mal y se detenga una integración en CI/CD. Vengo de hacer herramientas de Editor personalizadas (como mis Wizards en Unity) y sistemas automatizados en Python (ARA). Me mueve solucionar esos dolores de equipo para que los Artistas Técnicos y los Devs puedan preocuparse de innovar y no de corregir la posición de 400 tornillos en un nivel de juego. Traigo la mentalidad estricta y matemática, sé codear, conozco el motor; y si algo falla en el código, sé *exactamente* donde la geometría se cruza con él."
