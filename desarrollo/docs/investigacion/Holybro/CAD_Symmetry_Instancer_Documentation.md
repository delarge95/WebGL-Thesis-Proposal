# CAD Symmetry Instancer - Documentación Técnica y Arquitectura

Este documento detalla la creación, evolución y lógica matemática del **CAD Symmetry Instancer**, un Addon para Blender desarrollado específicamente para solucionar los cuellos de botella generados por las importaciones de geometría CAD industrial enfocadas hacia pipelines de rendering en tiempo real (WebGL/Unity).

## 1. El Problema Base: Geometría CAD vs. Motores 3D

Cuando un ingeniero diseña un dron en software CAD (SolidWorks, Fusion360, etc.) y se exporta a mallas poligonales (.obj, .fbx, .dae), los sistemas CAD a menudo "hornean" (bake) la información de transformación (posición, rotación y escala) directamente en las coordenadas absolutas de los vértices. 

Como resultado, todas las piezas de un dron se exportan como objetos únicos e independientes con:
- **Origen en (0,0,0) absoluto** (el centro del mundo).
- **Rotación (0,0,0) absoluta**.
- Vértices desplazados milimétricamente en el espacio para lograr la forma final.

**Problemas para el Technical Artist y Unity:**
1. **Rendimiento (Instancing):** Unity no puede aprovechar el *GPU Instancing* porque lee 24 tornillos como 24 mallas distintas, ahogando la memoria y multiplicando los *Draw Calls*.
2. **Interactividad (Exploded View):** Si el centro cívico de la pieza está en `(0,0,0)`, una animación de "despiece" moverá las piezas radialmente desde el centro del dron, no desde su propio volumen, haciendo que las piezas atraviesen otras geometrías antinaturalmente de forma asimétrica.
3. **Mantenimiento (Retopología):** Hacer retopología o generar LODs para 500 piezas individuales tomaría meses. El workflow ideal exige retopologizar un "Maestro" y que los "Esclavos" hereden automáticamente el trabajo.

---

## 2. Evolución Arquitectónica (De V1 a V5)

La creación del script fue iterativa, superando obstáculos matemáticos revelados por la naturaleza inestable de los datos CAD.

### Versión 1: Script de Copiado Directo (Fallido)
- **Lógica:** Seleccionar copias y asignarles `esclavo.data = maestro.data`.
- **Fallo:** Como todas las piezas compartían el origen `(0,0,0)`, al compartir los datos de malla del maestro, los 3 brazos esclavos colapsaban visualmente sobre el brazo maestro en el mismo lugar exacto. Las posiciones espaciales se perdían para siempre.

### Versión 2: Vectorización Radial por Centro de Masa (Mejorada pero Específica)
- **Lógica:** 
  1. Se forzó a que cada pieza moviera su origen a su Centro de Masa Volumétrico (`ORIGIN_CENTER_OF_MASS`). Se guardó la ubicación espacial local resultante (`cad_loc`).
  2. Usando los Vectores X,Y desde el centro `(0,0)` hasta el nuevo centro de masa de la pieza, se calculó por Trigonometría (Arco-Tangente `math.atan2`) la diferencia angular en Z respecto de la pieza maestra.
  3. Se instanciaba y se sumaba ese ángulo.
- **Fallo:** Funcionó perfecto para brazos y motores, que dictan una simetría radial z-céntrica (órbita 2D). Pero los tornillos que no apuntaban al centro, sino hacia abajo o en diagonales, quedaban totalmente torcidos.

### Versión 3: El Error de la Topología Barajada y PCA
- **Lógica:** Para enderezar piezas caóticas, se intentó alinear el "Vértice 0" del esclavo con el "Vértice 0" del maestro usando Álgebra Matricial. Falló porque los exportadores CAD barajan los índices de los vértices (Random Vertex Ordering).
- **Lógica Posterior:** Se introdujo **PCA (Principal Component Analysis)**. Usando Eigenvectors y Covarianza, el script deducía los ejes de grosor, longitud y altura de una masa de vértices para alinearlos. 
- **Fallo:** Un tornillo es un cilindro casi perfecto. PCA no puede distinguir "adelante" de "atrás" en formas isométricas o rotacionalmente simétricas, por tanto, giraba las cabezas de los tornillos 90° o 180° por pura coincidencia matemática.

### Versión 4: Hard-Undo y Estrategias Manuales (Parche de UX)
- **Lógica:** Introducción de un *Deep Copy* del modelo pre-ejecución para salvar al usuario de datos corruptos de deshacer (Ctrl+Z fallando). Introdujo también un combo-box para que el usuario eligiera si aplicar PCA, Radial o Absoluta (ninguna rotación inventada).
- **Conclusión:** Rompía la "Automatización 1-Click". El usuario tenía que pensar qué algoritmo usar por pieza.

### Versión 5 (Ultimate): Topología SVD de Kabsch (La Solución Universal)
- **El Descubrimiento:** Mediante ingeniería inversa a las Mallas CAD en Blender, se descubrió que, si bien el origen topológico está arruinado, **los exportadores de Solidworks SÍ respetan el conteo y orden estricto de índices de vértices idénticamente a través de todas las copias repetidas**.

Con esta certeza topológica, se desarrolló la versión definitiva usando el algoritmo `get_kabsch_transform` (SVD Registration).

---

## 3. Anatomía de la Versión 5 Ultimate: Algoritmo Kabsch y Transformaciones Relativas

El Addon final expone una interfaz (Panel N -> Holybro CAD) que automatiza este flujo resolviendo de forma independiente las dos variables que componen una transformación espacial: **La Matriz del Objeto (World Transform)** y **La Nube de Puntos (Vertex Coordinates)**.

El sistema se compone de una arquitectura de 4 módulos matemáticamente secuenciales:

### Módulo 0: Auto-Detección Heurística 
El proceso de selección en una escena CAD de miles de piezas es propenso al error humano. Este módulo emplea un barrido iterativo evaluando 3 condiciones de viabilidad (Condición AND):
1. **Filtro Léxico (Regex):** `re.search(r"^(.*?)(?:[\._-]\d+|\(\d+\))?$")`. Aisla la raíz nominal de la pieza removiendo sufijos paramétricos clásicos de Blender (`.001`), SolidWorks (`(1)`) o FreeCAD (`_001`). Esto recorta drásticamente el coste de la computación O(N) excluyendo siluetas ajenas a la familia nominal.
2. **Varianza Volumétrica Limítrofe:** Las exportaciones e importaciones de *NURBS* a polígonos suelen sufrir micro-diferencias de triangulación ligadas a errores de Coma Flotante (`float`). Se calcula el volumen envolvente del Bounding Box y se exige una coincidencia según una tolerancia dada por el usuario (usualmente 5%).
3. **Equivalencia Discreta (Isomorfismo Topológico):** Se valida formalmente que `len(Esclavo.vertices) == len(Maestro.vertices)`. Esta es la pre-condición matemática sagrada e indispensable para que el algoritmo Kabsch del Módulo 2 pueda operar: ambas matrices matriciales deben poseer exactamente la misma dimensionalidad *Nx3* para ser operables matricialmente.

### Módulo 1: Hard-Undo (Deep Copy en RAM)
Blender opera su histórico `Ctrl+Z` iterando sobre punteros simbólicos de su API. Modificar arreglos de vértices masivos y la `matrix_world` internamente rompe esta pila.
Para dotar a la herramienta de invulnerabilidad, este módulo elude la API de Undo construyendo un **Fantasma Físico**:
- Intercepta el bloque de datos con `mesh.copy()`, generando una malla aislada sin vincular a la escena (Cero *Users*).
- Extrae la Matriz Absoluta Original de Transformación del objeto mapeando dimensionalmente sus $4 \times 4 = 16$ flotantes a una lista plana en un Propiedad Personalizada (ID Property).
- Si el proceso falla, el Motor de Restauración destruye la geometría corrompida, inyecta el identificador de la memoria fantasma de regreso a la interfaz de usuario, y reinicia la variable matricial original. Es una restauración *hard-coded*.

### Módulo 2: Modelado Analítico mediante Algoritmo SVD de Kabsch (La Solución Universal)

La barrera insalvable de las versiones V1, V2 y V3 radicaba en asumir qué convenciones había usado el software CAD para instanciar (Simetría radial, Orientaciones de Cajas Limitantes, etc). 

El avance crítico de la Versión V5 radica en el descubrimiento intrínseco de los exportadores IGES/STEP a .fbx/.obj: **El "Orden de Índice de Vértice" (Vertex Ordering) rara vez se reconstruye**. La esquina Top-Left superior del tornillo esclavo ocupa la línea `Index[0]` de su array, al igual que la esquina Top-Left superior del tornillo maestro ocupa la posición `Index[0]`. Están en coordenadas espaciales distintas, **pero son correspondientes discretos exactos (pares 1:1)**.

Al tener nubes de puntos perfectamente pareadas, podemos determinar la Matriz de Rotación 3D pura que define su diferencia espacial usando el **Algoritmo Kabsch (Registro SVD o Procrustes Ortogonal)**, implementado de la siguiente manera usando `Numpy`:

1.  **Centrado Universal (Traslación = 0):**
    Para calcular una rotación abstracta, no nos importa en qué coordenadas geográficas están los objetos. Se calcula el **Centroide Analítico** de cada nube de puntos obteniendo su valor Medio (`np.mean`) en $x, y, z$. Luego, a cada vértice de la nube se le resta ese centroide, empujando virtualmente ambas mallas hacia el origen local absoluto `(0,0,0)`.
    ```python
    cA = np.mean(A, axis=0); cB = np.mean(B, axis=0) # Centros de masa
    AA = A - cA; BB = B - cB # Desplazamiento a origen cero absoluto
    ```
2.  **Matriz de Covarianza Cruzada:**
    Para descubrir qué dirección espacial domina a ambas, se multiplica la matriz transpuesta del Master $A^{T}$ (moldeada a Nx3) frente a la matriz Esclava $B$. Esto da a luz a la matriz de dispersión `H`:
    ```python
    H = np.dot(AA.T, BB) 
    ```
3.  **Descomposición en Valores Singulares (SVD):**
    El corazón matemático. SVD (Singular Value Decomposition) descompone la pesada matriz `H` generada por miles de vértices en 3 matrices elementales: Unidades Ortogonales Izquierdas ($U$), Vectores Diagonales de Escala ($S$), y Unidades Ortogonales Derechas ($V^T$).
    ```python
    U, S, Vt = np.linalg.svd(H)
    ```
4.  **Extracción de la Matriz de Rotación Relativa Exacta ($R$):**
    Usando álgebra de matrices ortogonales, obtenemos la matriz definitiva de rotación óptima al multiplicar $V^T$ transpuesta por $U$ transpuesta.
    ```python
    R = np.dot(Vt.T, U.T)
    ```
5.  **Corrección Antirrefractiva (Manejo de Simetría Bilateral Inversa):**
    En ensambles mecánicos (CAD), una pieza izquierda (esclava) suele ser el resultado de aplicar una propiedad `Scale = -1` (efecto espejo) de una pieza derecha (maestra). Si ocurre un efecto espejo topológico cruzado, el Determinante Lineal de la Rotación $R$ arrojará un valor $\det(R) < 0$. El algoritmo intercepta este estado asimétrico, fuerza artificialmente la inversión del Vector Singular en el Eje $Z$, recalculando un giro legítimo y etiquetando la pieza para que el script invierta la escala física de la malla en Blender posteriormente.

### Módulo 4: Teoría de Fusión de Matrices (Instancing No-Destructivo)

Al finalizar Kabsch (Módulo 2), poseemos ahora en el bolsillo del esclavo una Matriz de Rotación Delta Delta $R_{kabsch}$. Este factor simboliza *"Cuánto rotó el autor el interior geográfico del CAD*".

Sin embargo, en el viewport nativo, un objeto en un motor gráfico posee su propio vector de Rotación Objeto `S_World_old` que también está gobernando su postura final. En el caso del Dron Holybro (p. ej. los tornillos `GB70-M3-6`), partes del CAD vinieron con **Toda la rotación asignada limpiamente al Objeto ($R_{kabsch}$ da Matriz Identidad pura), mientras otras piezas híbridas vinieron con coordenadas topológicamente horneadas en local y con el Objeto rotado a 45° a la vez**.

Para reconciliar instanciamientos y evitar destruir cualquiera de estos dos acercamientos del exportador original, la ecuación final que aplica el Módulo 4 en el Python es una **Multiplicación Euclidiana Clásica (`@`)** de dos matrices de Base 4x4.

$$S_{world\_new} = S_{world\_old} \times R_{kabsch}$$

**Lógica Binaria Resolutiva:**
*   **Caso A (Nativos Perfectos):** Si el SolidWorks rotó limpiamente el objeto, sus mallas son vírgenes y clónicas. El escrutinio SVD Topológico deduce que $R_{kabsch} = \text{Matriz Identidad} (1)$. Matemáticamente obtenemos $S_{world} = S_{world} \times 1$. Ninguna rotación maliciosa es empalmada, preservando nativamente la rotación pre-horneada que gozaban los tornillos.
*   **Caso B (Mallas Horneadas Destructivas):** Si las mallas diferían internamente, el SVD arroja el ángulo secreto alfabético $X$. Al instanciar `mesh_slave = mesh_master`, el esclavo "pierde" visiblemente su forma, pero instantáneamente multiplicamos $S_{world} \times X$. La matriz se hincha con el conocimiento perdido del SVD, empujando instantáneamente el esclavo instanciado a la orientación visual idéntica que solidworks demandaba.

---

## 4. WorkFlow Operativo Sugerido

Para cualquier artista técnico asimilando un CAD dron:
1. Aislar partes, identificar repetitivas.
2. Seleccionar 1 sola variante maestra de la pieza en cuestión (Ej: 1 brazo).
3. Presionar **[0. Auto-Seleccionar Hermanas]**.
4. Ejecutar Botones **1, 2, 3, 4** secuencialmente.
5. Iniciar retopología o UV unwrapping en la pieza maestra "amarilla". Visualizar el trabajo completarse replicado instantáneamente en el drone entero.

---

## 5. Versión 7 (MoI3D ICP): El Reto de la Topología Inconsistente

Con la adopción de piezas complejas traídas desde herramientas de partición y modelado matemático explícito como **3D MoI (NURBS)** (ej. sujetadores `GB70-M3-8-DING`), el algoritmo cerrado de Kabsch (Versión V5 y V6) colapsó bajo un nuevo paradigma topológico:

* **Inestabilidad del Meshing:** Dos métricas de un mismo tornillo importado arrojaban diferencias en su conteo total de vértices (`11,542` vs `11,544`, etc.). MoI3D reconstruye la malla poligonal dinámicamente según la orientación espacial de la curva NURBS en la exportación, impidiendo establecer correspondencias 1:1 a lo largo de las caras lógicas idénticas originando un error matemático en la inyección SVD.

Para mitigar esto, se bifurcó a la **Versión 7**, resolviendo el "Pattern Matching" volumétricamente de forma independiente del conteo usando un pipeline híbrido O(N^2):

1. **Alineación de Componentes Principales (PCA)**: Extrayendo la covarianza de la matriz de vértices esclava e interceptando sus _eigenvectors_, el script halla matemáticamente los ejes de masa (largo del tornillo y su normal cilíndrica) sin importar cuántos polígonos representen su cabeza o su caña.
2. **Registro ICP (Iterative Closest Point)**: PCA provee una aproximación excelente pero susceptible a asimetrías de 180 grados sobre el eje propio. Se delegó el *Fine-Tuning* al algoritmo ICP impulsado por `mathutils.kdtree`. Este genera un Point Cloud que evalúa distancias contra el volumen de la pieza maestra iterando la transformación para minimizar el índice de error total de todas las distancias.
3. **Instanciamiento a Malla Maestra**: Con la rotación depurada (errores estadísticamente menores a 0.02 Unidades Blender absolutas frente a cientos de millares de vértices), la malla CAD corrompida se destruye y da lugar a una instancia legítima sin perder control espacial u orientación nativa de las cabezas hexagonales del tornillo, optimizando Draw Calls a una fracción ínfima en Unity y posibilitando el sistema de explosión procedural libre de colisiones irregulares.

---

## 6. Implementaciones Futuras y Roadmap

El éxito de la sincronización de geometrías deja pavimentado el camino para features de empaquetado final:

*   **1-Click LOD Generator:** El instancing provee un master mesh. Iterar sobre el master, duplicar su objeto `GameObject`, renombrarlo convencionalmente `*_LOD0`, `*_LOD1`, `*_LOD2`, e inyectar el Modificador `Decimate` en Blender ajustando el "Ratio" a 1.0, 0.5 y 0.05 respectivamente. Agrupándolos bajo un Empty Padre (LOD Group) que Unity importa de forma nativa sin configurar scripts extra.
*   **Unity One-Click Push Exporter:** A menudo un exporter FBX exporta basura estática oculta, materiales CAD no optimizados y transformadas a escala rara (Ej: Rotation -90 en X) por el Switch Y/Z Forward de Blender a Unity. Un operador final forzaría `Apply Rotation & Scale (Clear Transform)` en todos los parents resolviendo escalas 1.0 puras, exportando el `.FBX` al directorio `Assets/Models/...` de Unity limpiamente.
*   **Bilateral Mirror Support Finito:** Evaluar el factor de Determinante Invertido escalando físicamente (-1 en X/Y/Z) a nivel Blender UI para piezas que no rotan sino que reflejaran su carcasa completa, eliminando artefactos de iluminación post-proceso en Unity (Normal Flipping).
