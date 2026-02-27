# Análisis de Usuario Objetivo y Propuesta de Dron 

**Fecha:** 25 de Febrero, 2026
**Contexto:** Revisión de la Propuesta de Tesis y re-alineación de los recursos 3D del proyecto interactivo WebGL.

---

## 1. El Verdadero Usuario Objetivo (Basado en la Propuesta)

Tras analizar los documentos `all_sections_content.txt` y `final_proposal.tex`, es evidente que **el enfoque del proyecto NO es el consumidor final, sino un público técnico y especializado.**

El marco teórico de la tesis se fundamenta en la **Teoría de la Carga Cognitiva** y la necesidad de reducir la carga para la comprensión de *estructura interna y relaciones jerárquicas* (ej: rotor -> motor -> ESC -> batería). 

*   **Perfil del Usuario:** Ingenieros, técnicos de mantenimiento, estudiantes de robótica y desarrolladores de ensamblajes mecatrónicos.
*   **Problema a Resolver:** Los medios tradicionales (PDF, imágenes 2D) exigen demasiada "rotación mental".
*   **Solución (MVP):** Un visor 3D interactivo con "despiece funcional" (Exploded View) que demuestre **arquitectura interna**.

## 2. Incongruencia del DJI Phantom 4 Pro con la Tesis

El usuario ("una cagada") tiene toda la razón al descartar el Phantom 4 Pro. A la luz de los objetivos de la tesis, el Phantom es la peor elección posible por tres razones:

1.  **Es una "Caja Negra" (Black Box):** Es un dron de electrónica de consumo. A DJI no le interesa que el interior se entienda o modifique. 
2.  **No enseña estándares industriales:** La electrónica interna del Phantom es una PCB propietaria deformada para encajar en la carcasa plástica. Mostrar eso en el visor **no aporta valor educativo** a un ingeniero, porque en la industria real los sistemas son modulares (Flight Controller estándar, PDB separada, ESCs separados).
3.  **Filosofía Contraria:** El Phantom oculta la ingeniería detrás de estética; la tesis busca revelar la ingeniería a través de la estética.

## 3. Alternativas Propuestas (Alto Rendimiento + Modularidad + Open Hardware)

Para que el modelo encaje con los intereses técnicos del proyecto, demuestre hardware de alto nivel y cuente de forma natural con "piezas explotables" (Exploded View) para el código existente en `DroneAssembler.cs`, se proponen estos tres candidatos:

### Alternativa 1: TBS Source One V5 (Estándar FPV/Cinemático de Alto Rendimiento)
*   **¿Qué es?:** Es el marco (frame) de fibra de carbono más famoso de la comunidad *Free Style* y cinemática de alto rendimiento.
*   **Por qué encaja:** Su "Stack" central está totalmente expuesto. Para propósitos educativos es ideal porque muestra claramente cómo interactúa el Controlador de Vuelo (FC) encima del ESC 4-en-1, junto con los gruesos cables AWG que van a los motores *brushless* expuestos. 
*   **Disponibilidad:** Todos sus archivos CAD son 100% de código abierto (Open Source en GitHub).
*   **Estética:** Agresiva, fibra de carbono, detalles en aluminio anodizado de color. Muy "High-Tech".

### Alternativa 2: Holybro X500 V2 (El Estándar Industrial / Académico)
*   **¿Qué es?:** Es el kit de referencia oficial de la plataforma de desarrollo abierto de drones (PX4).
*   **Por qué encaja:** Representa rigurosidad técnica. Tiene tubos de fibra de carbono montados sobre placas *Power Distribution Board* (PDB). Arriba lleva el famoso controlador de vuelo **Pixhawk** (mencionado previamente en tus apuntes como Pixhawk 4), módulos GPS levantados, y brazos completamente modulares.
*   **Experiencia UI:** Al hacer click en "Despiece" en tu aplicación Unity, este dron se separa en brazos, platos centrales, controlador lógico, y bahía inferior. Es el ejemplo *exacto* de topología modular que quieres enseñar.

### Alternativa 3: Freefly Astro (UAV Comercial de Mapeo y Topografía de Extrema Gama)
*   **¿Qué es?:** Un dron de alta gama para industrias pesadas construido alrededor del ecosistema Auterion/PX4.
*   **Por qué encaja:** Da la escala de "alto rendimiento" empresarial masivo en lugar de hobby o universidad. También tiene brazos plegables modulares que pueden mostrar articulaciones mecánicas complejas en WebGL para impresionar a nivel técnico.

---

### Recomendación del Technical Artist

Te sugiero que nos movamos a la **Alternativa 1 (TBS Source One con placa Pixhawk/SpeedyBee)** o la **Alternativa 2 (Holybro X500 V2)**. 

Ambos justifican científicamente (en tu marco teórico) el valor del visor 3D para entender conexiones físicas modulares en sistemas dinámicos robóticos, alejándonos del juguete sellado que representa DJI. ¿Cuál de estos rubores (FPV cinemático o Investigación/Académico estilo Pixhawk) sientes que representa mejor la estética visual y técnica que quieres en tu portafolio?
