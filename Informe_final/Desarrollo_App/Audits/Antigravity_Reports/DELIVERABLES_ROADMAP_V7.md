# Hoja de Ruta del Proyecto - Nivel Investigador PhD (V7)

Esta hoja de ruta establece el estándar definitivo para la culminación de la tesis. Las iteraciones previas no reflejaban el peso científico real de los sistemas programados. La **V7** define el marco ontológico de una investigación en Ingeniería Mecatrónica y Software de alto nivel.

---

## FASE A: FUNDAMENTACIÓN FÍSICO-MATEMÁTICA Y HARDWARE (Ground Truth)
**Objetivo:** Eliminar la ambigüedad conceptual. El dron no es un asset "decorativo"; es un gemelo digital de un ecosistema en tiempo real.

*   **A.1. Declaración de Ruptura de Paradigm (Bye DJI):** Exposición académica de por qué los sistemas *Caja Negra* obstaculizan la didáctica de ingeniería, justificando matemáticamente la topología modular del ecosistema **Auterion/PX4**.
*   **A.2. Auditoría Integral del Holybro X500 V2:** Documentar en tablas LaTeX los picos de potencia de los estatores 2216 KV880 ($\sim 250W$), consumo parasitario de los ESCs sin BEC, y el ancho de banda serial del Pixhawk 6C / M10 GNSS. Todo anclado a referencias cruzadas reales.
*   **A.3. Ecuaciones Base del Viewport:** 
    *   Formulación del View-Shift trigonométrico ($Offset = distance \times \tan(FOV/2)$).
    *   Derivación euclidiana de toques múltiples para la lógica matricial del *Pinch-to-Zoom* sin bloqueos Gimbal.

## FASE B: INGENIERÍA DE SOFTWARE, IA Y TECHNICAL ART
**Objetivo:** Defender el repositorio como una infraestructura a escala empresarial.

*   **B.1. Exposición del Framework Algorítmico MLOps:** Desvelar y documentar las 14 "Skills" de IA que operan pasivamente sobre el código base (Validación estática de C#, comprobación de sintaxis USS para UI Toolkit, automatización de compilados LaTex).
*   **B.2. Gestión de Entropía C# y Física de Memoria:**
    *   Arquitecturas en disputa: Análisis profundo de `PersistentSingletons` frente a Inyección de Dependencias. Prevención de colisiones concurrentes en el `EventBus`.
    *   Documentación exhaustiva de la barrera física de raycasting vs Eventos DOM (`InputManager.IsPointerOverUI`).
*   **B.3. La Batalla del Heap Allocation (El Leak de XRay):** Se ordenará explícitamente redactar un ensayo técnico sobre el impacto del Garbage Collector en Wasm al instanciar materiales nuevos accidentalmente, versus la elegancia imperativa del modelo de O(1) vía `MaterialPropertyBlock`.
*   **B.4. Renderizado Basado en Física (PBR) y Termodinámica Proxy:**
    *   Uso empírico de la distribución Gaussiana GGX y el factor de oclusión ambiental.
    *   Creación de mapas de estrés calórico en base a shaders generativos con algoritmos *OpenSimplex* / *Hermite*.
    *   Optimización decenal (Tangent Space Baking) y el Hiperplano inyectivo para la navaja de sección transversal (`_GlobalClipPlane.w`).

## FASE C: CIENCIAS DEL COMPORTAMIENTO HUMANO-COMPUTADOR (HCI)
**Objetivo:** Justificar el éxito visual mediante métricas de diseño clínico.

*   **C.1. El Diseño Espacial ("Aero-Glass 2026"):** Inscribir el diseño visual dentro de la literatura HCI.
    *   Uso de Funciones de Desenfoque Estocástico para UI Flotantes (Mitigación del efecto Banding en pantallas mediocres con ruido perceptual).
    *   Mapeo de las retículas *Bento* para la gestión focal óptima bajo la ley de Hick-Hyman.
*   **C.2. Ergonomía Digital:** Dimensionamiento métrico basado en la Ley de Fitts ($ID = \log(2D/W)$) para *Touch Targets* absolutos en dispositivos con densidades variables.
*   **C.3. Pruebas Cuantitativas Rigurosas:** Inserción de gráficas estandarizadas (Desviación tipo poblacional $\sigma$) en el modelo SUS (*System Usability Scale*) y varianza de respuesta neuronal abstracta (NASA-TLX).

## FASE D: COMPILACIÓN CIENTÍFICA (EJECUCIÓN INMEDIATA)
La tesis ya no requerirá redacción humana; las herramientas de automatización ensamblarán los módulos.

1.  El Agente Lógico (`math_solver`) estructurará en LaTeX las ecuaciones de Frustum, Pinch, Matrices Termodinámicas y Leyes de Fitts.
2.  El Agente Integrador (`tech_writer`) indexará los datos del manual original del X500 en las tablas LaTeX y desmenuzará el diagrama arquitectónico MLOps.
3.  El Agente UX (`ux_auditor`) formalizará los postulados del Aero-Glass, insertándolos retrospectivamente en el marco conceptual de Diseño Interactivo (Capítulo 5).

> **Aprobación Restringida:** El informe requiere confirmación en firme sobre el rigor de la V7 antes de compilar la data masiva al `informe_final.tex`.
