# ÍNDICE ESTRUCTURAL DEFINITIVO: TESIS DOCTORAL / MAESTRÍA (V7)

Este índice representa el nivel de exigencia máxima (State-of-the-Art) para una tesis de Ingeniería de Hardware, Software y Computación Gráfica. Cubre desde las leyes matemáticas de la percepción visual hasta la termodinámica del hardware emulado y la gestión del heap en WebAssembly.

---

## 📄 PRELIMINARES Y PROTOCOLO ACADÉMICO
- [x] Portada Oficial y Aprobaciones del Jurado.
- [x] Dedicatoria y Agradecimientos.
- [x] Resumen Ejecutivo (Con KPIs duros: Zero-Alloc, <100k Polígonos, N=12 Usuarios).
- [x] Abstract (Traducción técnica certificada).
- [x] Nomenclatura y Acrónimos (PBR, HCI, ASTC, VRAM, ESC, PDB, MLOps).
- [x] Tabla de Contenidos, Índice de Figuras, Índice de Tablas, Índice de Ecuaciones.

---

## 📖 CAPÍTULO 1: LA CRISIS DE LA COMUNICACIÓN TÉCNICA EN LA INDUSTRIA 4.0
- [x] **1.1 Planteamiento del Problema:** La "Rotación Mental" inducida por medios 2D estáticos y la obsolescencia del PDF en líneas de ensamblaje.
- [ ] **1.2 La Falacia de la "Caja Negra" Comercial:** Justificación de por qué el uso de sistemas propietarios (e.g., DJI Phantom) es académicamente estéril frente a plataformas de "Open-Hardware" en la educación mecatrónica.
- [ ] **1.3 El Paradigma del Gemelo Digital Educativo (Educational Digital Twin):** Intersección entre Taxonomía de Bloom (Módulo Psicomotor) y renderizado 3D en tiempo real.
- [x] **1.4 Hipótesis de Investigación:** La interactividad 3D procedimental en plataformas agnósticas (WebGL) reduce estadísticamente la Carga Cognitiva Extrínseca.
- [x] **1.5 Objetivos de Investigación (General y Específicos).**

---

## 📖 CAPÍTULO 2: MARCO TEÓRICO Y ECUACIONES FUNDAMENTALES
- [~] **2.1 Teoría Cognitiva del Aprendizaje Multimedia (Mayer & Sweller):** Modelado del ancho de banda de la memoria de trabajo.
- [ ] **2.2 Ecuaciones Fundamentales del PBR (Physically Based Rendering):**
    - [ ] La Ecuación de Renderizado (Kajiya, 1986).
    - [ ] Modelo Bidirectional Reflectance Distribution Function (BRDF) de Cook-Torrance (Distribución Trowbridge-Reitz GGX, Ecuación de Fresnel-Schlick).
- [ ] **2.3 Fundamentos Matemáticos de Interacción Humano-Computador (HCI):**
    - [ ] Ley de Fitts ($ID = \log_2(2D/W)$) aplicada al dimensionamiento de *Touch Targets* en pantallas táctiles de alta densidad.
    - [ ] Ley de Hick-Hyman para la toma de decisiones en jerarquías de menú Bento Grid.

---

## 📖 CAPÍTULO 3: METODOLOGÍA Y TOOLCHAIN DE INGENIERÍA (CI/CD Y MLOps)
- [x] **3.1 Paradigma Metodológico:** Design Science Research (DSR).
- [ ] **3.2 El Toolchain de Desarrollo (Tech Stack):** 
    - Motor: Unity 6.0 LTS (URP pipeline).
    - Frontend Web: C# 9.0 transcrito mediante IL2CPP a WebAssembly (Wasm).
    - UI: UI Toolkit (Yoga Flexbox Layout Engine).
- [ ] **3.3 Ecosistema de Agentes de Inteligencia Artificial (MLOps):**
    - [ ] Arquitectura del "Antigravity Bridge".
    - [ ] Las 14 Skills Python de Auditoría C#: Análisis léxico de USS (`uss_linter`), validación heurística de UI (`ui_validator`), control de deuda técnica modular (`arch_guard`).
    - [ ] Workflows de compilación continua automatizada LaTeX/PDF.

---

## 📖 CAPÍTULO 4: EL SUJETO FÍSICO: INGENIERÍA INVERSA DEL HOLYBRO X500 V2
*(Este capítulo valida el nivel de Hardware Engineering del proyecto)*
- [ ] **4.1 Arquitectura Abierta PX4 y Ecosistema Auterion:** Por qué el estándar industrial es vital.
- [ ] **4.2 Anatomía Mecatrónica y Parametrización de la Base de Datos (`DronePartData.asset`):**
    - [ ] **Planta Motriz:** Análisis estático de los Motores 2216 KV880/920 ($\sim 250W$ max, tracción nominal) y hélices 1045.
    - [ ] **Potencia y Conmutación:** PDB (Power Distribution Board) con conectores XT60/XT30, y ESCs BLHeli-S 20A sin BEC.
    - [ ] **Cerebro Lógico:** Autopiloto Pixhawk 6C (Arquitectura H7 dual-core).
    - [ ] **Telemetría y Navegación:** Integración de módulo M10 GNSS y Radio SiK V3 (433/915 MHz).
- [ ] **4.3 Análisis Térmico Visual de Superficie:** Creación matemática de un shader procedimental para disipación convectiva (Temperatura de ESCs y Motores operando a $80^\circ C$) usando interpolaciones de ruido Hash y Hermite.

---

## 📖 CAPÍTULO 5: "AERO-GLASS 2026": DISEÑO ESPACIAL Y SISTEMA UI/UX
*(Este capítulo valida el nivel de Product Design e HCI)*
- [ ] **5.1 Evolución Modular y Bento Grids:** El abandono de menús flotantes tradicionales en favor de la grilla de 12 columnas.
- [ ] **5.2 Paleta "Midnight Neon" y Contraste WCAG 2.1 AAA:** Especificación espectral de absorción de luz para fondos *Deep Void* (#050505) y acentos *Neon Cyan*.
- [ ] **5.3 Glassmorphism 2.0 en WebGL:** Costo computacional del *Backdrop Filter Blur* y adición de ruido estocástico (Noise overlay) para prever "color banding".
- [ ] **5.4 Diseño Responsive Espacial (Spatial UI):** Anclajes de *World Space UI*, *Billboarding* (orientación de normales de texto hacia el vector de cámara) y escalado dinámico relativo a la distancia Z.

---

## 📖 CAPÍTULO 6: FÍSICAS, RAYCASTING Y PATRONES DE ARQUITECTURA C#
*(Este capítulo valida el nivel de Software Engineering)*
- [~] **6.1 Patrones de Diseño Implementados (Anclados bajo principios SOLID):**
    - El Anti-Patrón Singleton vs. Dependency Injection. ¿Por qué se usaron `PersistentSingletons` en los Core Managers?
    - Implementación de un `EventBus` libre de colisiones (Thread-Safe Snapshotting `new List<Delegate>(subs)`).
    - Sistema de Estados Finitos (`AppStateMachine`) gobernando la transitividad de datos.
- [ ] **6.2 Matemática Vectorial Multi-Táctil Activa (`OrbitCameraController.cs`):** 
    - Ecuación de distancia euclidiana para `Pinch-To-Zoom` basada en la primera derivada escalar de dos vectores tactiles.
    - Desplazamiento de Centroides (Centroid Elevator Panning).
- [ ] **6.3 El Algoritmo de "View Shift" Trigonométrico:** Desfase vertical del plano focal usando la ecuación de la altura del Frustum: $Offset = distance \times \tan(\theta_{FOV} / 2)$, asegurando que la UI inferior (Dock) no ocluya al dron durante el re-pivoteo.
- [ ] **6.4 Raycasting Dual y Barreras Lógicas (Anti-Ghosting):** Proyección `ScreenPointToRay()` bloqueada por la capa abstracta `InputManager.IsPointerOverUI()`, impidiendo interacciones físicas a través del Canvas UI Toolkit.

---

## 📖 CAPÍTULO 7: TECHNICAL ART, PIPELINE DE RENDER Y OPTIMIZACIÓN EXTREMA
*(Este capítulo valida el nivel de Technical Art y Computer Graphics)*
- [ ] **7.1 Topología y Horneado de Geometría (Retopology & Normal Baking):** Proceso de compresión geométrica desde modelos CAD estáticos ($>$ 2 millones Trisecciones) a representaciones WebGL navegables ($<$ 100k Trisecciones) mediante decimation y proyección de *Tangent-Space Normal Maps*.
- [ ] **7.2 Compresión de Texturas y Formatos de Entrega:** ASTC vs ETC2 para dispositivos móviles, compresión de build WebAssembly usando protocolo Brotli/Gzip en Apache/Nginx.
- [ ] **7.3 El Milagro del "Zero Allocation" en C#:** Mitigación agresiva del recolector de basura (Garbage Collector) usando `MaterialPropertyBlock.SetColor` modificando atributos de instancia GPU sin clonar objetos en memoria Heap.
- [ ] **7.4 El Caso de Estudio: Fuga de Memoria del Shader XRay:** Auditoría cruda de un *Memory Leak* encontrado en `MaterialController.cs` (sobrescritura explícita de `_renderer.material`), análisis de impacto en la VRAM de navegadores V8/Webkit, e implementación de correctivos.
- [ ] **7.5 Hiperplanos Algorítmicos Globables:** Uso de variables shader globales (`_GlobalClipPlane.w`) e inyección del producto punto para ocultamiento geométrico a nivel de fragmento (Aceleración de Section-Cuts).
- [ ] **7.6 Trazado Polinomial Visual:** Matrices espaciales dependientes del centro del Canvas para la pintura a tiempo real del sistema "Escáner" interpolado ($Lerp(a, b, dt \times 25)$).

---

## 📖 CAPÍTULO 8: VALIDACIÓN EMPÍRICA Y ANÁLISIS DE MERCADO
- [~] **8.1 Despliegue del Sistema y Testing Rig.** (El MVP de Landing Page `docs/index.html` con GSAP).
- [~] **8.2 Resultados Cuantitativos (Rendimiento):** 
    - Tiempos de primer *Paint* ($< 3s$).
    - Presupuestos de memoria (VRAM target $\sim$ 250MB para Safari iOS).
    - Tasa de refresco constante $>$ 30 FPS.
- [~] **8.3 Resultados Cuantitativos (Usabilidad y Percepción):**
    - System Usability Scale (SUS) con desviación estándar y varianza.
    - NASA-TLX para evaluar la reducción medible empíricamente de la Carga Cognitiva Intrínseca y Extrínseca.
- [x] **8.4 Conclusiones Finales.**

---

## 📄 REFERENCIAS Y ANEXOS OBLIGATORIOS
- [x] Bibliografía Extensa (Fijada en normativas APA 7ª Edición, estrictamente con DOIs).
- [ ] **Apéndice A:** Códigos Fuente Clave (Fragmentos del Frustum, EventBus, Zero Allocation).
- [ ] **Apéndice B:** Tablas completas del Bill Of Materials (Exportación del `DronePartData`).
- [ ] **Apéndice C:** Árbol de Ejecución de los Agentes IA (Workflows y .yaml).
- [ ] **Apéndice D:** Manual de Despliegue en Servidores Linux/Nginx.
