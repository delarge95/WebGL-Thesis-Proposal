\# Plan de Optimización Agresiva (550MB → 15MB)



Este plan define el flujo de trabajo técnico y automatizado para reducir drásticamente el peso geométrico originario de exportaciones CAD (3D MoI) con destino a Unity WebGL. Se adopta una estrategia de \*\*Instanciamiento Proxy en Blender\*\* complementada con un \*\*Ensamblaje Modular Tiling en Unity\*\*.



\## Resumen del Diagnóstico de Escena (Vía MCP)

\- Existen \*\*24 familias únicas de Hardware\*\* (tornillos `GB70`, `M3`, tuercas `FALAN`, etc.).

\- Componen entre \*\*240 a 300 instancias\*\* esparcidas por la escena.

\- Sólo los Fasteners consumen cerca de \*\*3.5 a 4 Millones de Vértices\*\* totales. Aislar y agilizar esta geometría es la prioridad absoluta para el alcance en WebGL.



\---



\## FASE 1: El Sistema Array/Tiling en Unity (Tornillo Modular)



Intentar escalar (`Scale`) una rosca o generar sus vértices matemáticamente durante el Runtime (C#) destruiría el "Pitch" (Paso) de la hélice, provocando roscas elongadas erróneamente y errores topológicos en los empalmes del cabezote. \*\*La solución AAA para esto es el Patrón Tiling (Apilamiento)\*\*.



\### Estructura de la Librería de Piezas Dummys (Para hacer en Blender)

En tu colección `Remaking`, por cada "Familia" de métrica (ej. M3, M2.5), vas a modelar separadamente:

1\. \*\*Top Mesh:\*\* Un cabezote High-Poly (ej. `M3\\\_Head\\\_Socket` o `M3\\\_Head\\\_Button`) que incluye el margen de descenso y el inicio suave de la rotación del primer hilo de rosca.

2\. \*\*Middle Mesh:\*\* Exactamente \*\*1 vuelta (360°)\*\* del cuerpo de la rosca. Sus vértices deben iniciar en $Z=0$ y terminar cerrando el ciclo en su altura correspondiente al Paso (Ej. $Z=0.5mm$).

3\. \*\*Bottom Mesh:\*\* La punta del tornillo, donde el corte de la hélice desciende hacia el interior.



\### Lógica C# de Auto-Ensamblaje

Desarrollaremos un script `FastenerBuilder.cs` en Unity que leerá qué largo de tornillo se requiere. En vez de escalar, el script usará `GPU Instancing` para copiar y apilar el \*\*Middle Mesh\*\* iterativamente.

\* Ejemplo para un tornillo de 6 vueltas: Instancia 1 Head, luego un bucle `for` que posiciona e instancia 6 Middle Meshes consecutivos (sin rotarlos, simplemente apilándolos arriba uno de otro en el eje Z), y 1 Bottom Mesh final.

\* Todo queda perfectamente continuo, realista en la física de los metales y empacado con \*\*LOD Groups\*\* para que desde lejos, Unity simplemente apague el modelo High-Poly y devuelva visualmente el Proxy de bajo nivel que crearemos a continuación.



\---



\## FASE 2: La Destrucción y Proxy Inteligente (Blender Pipeline)



No podemos dejar la geometría densa original en Blender. Un Addon de Python automatizará la sustitución de esos 300 componentes pesados por Proxies ultralivianos (LOD1/LOD2), los cuales no son simples cilindros rectos, sino aproximaciones morfológicamente idénticas al cabezote del original.



\### Lógica de Reemplazo del Addon

Cuando inicies el script, este ofrecerá configurar \*\*Tipos de Geometría Proxy\*\*, basándose en las 3 tipologías universales dictadas en la imagen de referencia:

\* \*\*Socket Head:\*\* (Capa cilíndrica recta).

\* \*\*Button/Pan Head:\*\* (Cabezal en Domo o Esférico biselado).

\* \*\*Countersunk/Flat:\*\* (Cabezal cónico hundido).



\*\*El Algoritmo paso a paso por Tornillo:\*\*

1\. \*\*Medición Analítica de Bounding Box\*\*: El script separará matemáticamente el tornillo en 2 mitades. Calculará el Diámetro/Altura específica de la Cabecera, y el Diámetro/Altura específica del Cuerpo/Caña.

2\. \*\*Construcción Procedural del Proxy\*\*:

&#x20;  - Se levantará un cilindro base de \*\*12 Lados\*\* para la caña midiendo el largo requerido.

&#x20;  - Dependiendo del tipo de cabezote seleccionado (o auto-detectado si logramos perfilarlo por Bounding Box), el script extruirá y escalará los vértices superiores (Cono para Countersunk, Domos segmentados para Button, Cilindro puro para Socket).

3\. \*\*Optimización Topológica de la Tapa\*\*: La parte plana superior se resolverá con un "Triangle Fan" (Uniendo todo a un vértice central), lo cual matemáticamente reduce la cantidad de polígonos versus una topología de grid de quads sin sacrificar ningún sombreado en WebGL. \*Para Proxies LOD1, 12 a 8 lados geométricos es la perfección absoluta entre silueta y peso.\*

4\. \*\*Metadatos y Heredación\*\*: El Proxy hereda la posición, rotación mediante \*eigenvectors\* PCA (para alinear eje con el original) y nominará el objeto como `SYS\\\_FASTENER\\\_M3\\\_Button\\\_8mm`.

5\. \*\*Eliminación\*\*: Se esconde o elimina la geometría NURBS pesada de ese tornillo procesado.



\---



\## FASE 3: Hoja de Ruta Táctica para Ejecutar Esto



1\. \*\*Terminar Remaking Manual\*\*: Modela las versiones perfectas (High Poly) de los \*Top/Middle/Bottom\* meshes de tus métricas principales (M3, M2.5) asegurándote que el Tiling encaje en 360 grados.

### Lógica de Reemplazo del Addon

Cuando inicies el script, este ofrecerá configurar **Tipos de Geometría Proxy**, basándose en las 3 tipologías universales dictadas en la imagen de referencia:

* **Socket Head:** (Capa cilíndrica recta).
* **Button/Pan Head:** (Cabezal en Domo o Esférico biselado).
* **Countersunk/Flat:** (Cabezal cónico hundido).

**El Algoritmo paso a paso por Tornillo:**

1. **Medición Analítica de Bounding Box**: El script separará matemáticamente el tornillo en 2 mitades. Calculará el Diámetro/Altura específica de la Cabecera, y el Diámetro/Altura específica del Cuerpo/Caña.
2. **Construcción Procedural del Proxy**:
   - Se levantará un cilindro base de **12 Lados** para la caña midiendo el largo requerido.
   - Dependiendo del tipo de cabezote seleccionado (o auto-detectado si logramos perfilarlo por Bounding Box), el script extruirá y escalará los vértices superiores (Cono para Countersunk, Domos segmentados para Button, Cilindro puro para Socket).
3. **Optimización Topológica de la Tapa**: La parte plana superior se resolverá con un "Triangle Fan" (Uniendo todo a un vértice central), lo cual matemáticamente reduce la cantidad de polígonos versus una topología de grid de quads sin sacrificar ningún sombreado en WebGL. *Para Proxies LOD1, 12 a 8 lados geométricos es la perfección absoluta entre silueta y peso.*
4. **Metadatos y Heredación**: El Proxy hereda la posición, rotación mediante *eigenvectors* PCA (para alinear eje con el original) y nominará el objeto como `SYS_FASTENER_M3_Button_8mm`.
5. **Eliminación**: Se esconde o elimina la geometría NURBS pesada de ese tornillo procesado.

---

## FASE 3: Hoja de Ruta Táctica para Ejecutar Esto

1. **Terminar Remaking Manual**: Modela las versiones perfectas (High Poly) de los *Top/Middle/Bottom* meshes de tus métricas principales (M3, M2.5) asegurándote que el Tiling encaje en 360 grados.
2. **Yo desarrollaré el Addon "Fastener Proxy Automator" (Blender)**: Programaré la herramienta que leerá, medirá, extruirá los cabezotes geométricos de 12 lados (identicos a la imagen de referencia optimizada) y auto-reemplazará los tornillos en Batch.
3. **Limpieza Batch Superficies**: Haremos un pase rápido de Decimación Plana (Angle Limit ~7°) para cables y tuercas simples.
4. **Exportar a Unity**: El FBX viaja masivamente optimizado.
5. **Implementar Tooling Unity**: Escribiremos el sistema Modular (`FastenerBuilder`) en C# que interpreta los marcadores `SYS_FASTENER` e inyecta la geometría Tiling (High Poly) cuando la cámara de inspección requiera el LOD0.

---

## FASE 4: Catálogo Técnico Ultradetallado de Fasteners (Extracto del MCP de Blender)

A continuación se detalla la lista exhaustiva de toda la tornillería y herrajes (Fasteners) encontrados en el ensamblaje actual del dron. Estos datos técnicos fueron extraídos directamente de la geometría del archivo en Blender (Bake Masters) y cruzados con normativas ISO/DIN/GB para que el agente de Unity/Blender tenga medidas exactas para el reemplazo algorítmico y procedural.

### 1. Familia: Socket Head Cap Screws (Cabeza Cilíndrica Allen - GB70 / DIN 912)
La familia `GB70` corresponde a tornillos de cabeza cilíndrica con hueco hexagonal.
- **Forma de la cabeza:** Cilíndrica recta (ideal para Proxy LOD en forma de cilindro puro de 12 lados).
- **Inicio de rosca:** Típicamente a ras del vástago, justo debajo del cabezal.

| ID Familia | Tamaño | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `GB70-M25-6` | M2.5 x 6mm | 24 | 4.43 x 4.46 x 8.24 mm | **Ø Hilo:** 2.5mm, **Pitch:** 0.45mm, **Ø Cabezal:** ~4.5mm, **Alto Cabezal:** 2.5mm |
| `GB70-M25-10` | M2.5 x 10mm | 8 | 4.44 x 4.46 x 12.24 mm | **Ø Hilo:** 2.5mm, **Pitch:** 0.45mm |
| `GB70-M25-12` | M2.5 x 12mm | 12 | 4.44 x 4.46 x 14.24 mm | **Ø Hilo:** 2.5mm, **Pitch:** 0.45mm |
| `GB70-M3-6` | M3 x 6mm | 16 | 5.40 x 5.40 x 8.70 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ø Cabezal:** ~5.4mm, **Alto Cabezal:** 3.0mm |
| `GB70-M3-8-DING`| M3 x 8mm | 12 | 5.40 x 5.40 x 10.70 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-21-DING`| M3 x 21mm | 2 | 5.40 x 5.40 x 20.70 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-25-DING`| M3 x 25mm | 2 | 5.40 x 5.40 x 24.70 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-38` | M3 x 38mm | 16 | 5.40 x 5.40 x 40.70 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm |

### 2. Familia: Countersunk / Flat Head Screws (Cabeza Avellanada/Plana - CHEN-LIU)
La etiqueta `CHEN-LIU` o equivalente indica un cabezote avellanado para quedar a ras de la superficie (como ISO 10642 / DIN 7991).
- **Forma de la cabeza:** Cono truncado hundido (Proxy LOD: Cilindro con extremo superior escalado radialmente).
- **Inicio de rosca:** A nivel de la superficie plana del cabezal en la medición total.

| ID Familia | Tamaño | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `M25-6-CHEN-LIU` | M2.5 x 6mm | 12 | 4.81 x 4.81 x 5.90 mm | **Ø Hilo:** 2.5mm, **Pitch:** 0.45mm, **Ø Cabezal máx:** ~4.8mm |
| `M3-16-CHEN-LIU` | M3 x 16mm | 2 | 5.80 x 5.80 x 15.80 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ø Cabezal máx:** ~5.8mm |

### 3. Familia: Pan Head / Button Head Screws (Cabeza Domo - PAN)
Tornillos de cabeza abovedada (Button / Pan Head, típicamente ISO 7380).
- **Forma de la cabeza:** Hemisferio aplanado o domo (Proxy LOD: Cilindro con extrusión superior segmentada para curvar).
- **Inicio de rosca:** Debajo de la base plana del domo.

| ID Familia | Tamaño | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `M3-10-PAN-DING`| M3 x 10mm | 4 | 5.60 x 5.60 x 10.10 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ø Cabezal:** ~5.6mm |
| `M3-14-PAN` | M3 x 14mm | 4 | 5.60 x 5.60 x 15.80 mm | **Ø Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ø Cabezal:** ~5.6mm |

### 4. Familia: Nuts & Lock Nuts (Tuercas y Tuercas de Seguridad Autoblocantes)
Tuercas hexagonales estándar y con inserto de nylon (ZSLM - Tuerca Autoblocante, DIN 985).
- **Forma:** Hexagonal (Proxy LOD: Cilindro puro de 6 lados exactos).

| ID Familia | Tipo | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `LM-M3-DING` | Tuerca Hex M3 | 8 | 5.40 x 6.24 x 2.50 mm | **Hex Ancho (Flat):** 5.5mm, **Pitch Int:** 0.50mm, **Grosor:** 2.5mm |
| `LM-M3-NILONG` | Tuerca Nylon M3 | 2 | 5.40 x 6.24 x 2.50 mm | *Material Nylon* |
| `ZSLM-M25` | Autoblocante M2.5 | 4 | 4.50 x 5.19 x 3.17 mm | **Hex Ancho:** 5.0mm, **Pitch Int:** 0.45mm, **Grosor:** ~3.2mm |
| `ZSLM-M3-DING` | Autoblocante M3 | 8 | 5.40 x 6.24 x 3.80 mm | **Hex Ancho:** 5.5mm, **Pitch Int:** 0.50mm, **Grosor:** ~3.8mm |
| `ZSLM-M3-FALAN`| Tuerca Brida M3 | 16 | 7.60 x 7.60 x 4.80 mm | **Brida (Flange) Ø:** ~7.6mm, **Grosor:** 4.8mm |

### 5. Familia: Standoffs / Spacers (Separadores de Nylon - NILONGZHU)
Separadores utilizados típicamente para montar PCBs (como la Pixhawk o la PDB).
- **Forma:** Cilindros rectos o columnas hexagonales.
- **Conexiones:** Usualmente de tipo Hembra-Hembra o Macho-Hembra.

| ID Familia | Tipo | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `NILONGZHU-M25-5`| Separador M2.5 | 4 | 4.50 x 5.00 x 5.19 mm | **Paso:** 0.45mm |
| `NILONGZHU-M3-5`| Separador M3 | 4 | 5.00 x 5.40 x 6.24 mm | **Paso:** 0.50mm |

### Resumen para Agentes Especializados
- **Topología Obligatoria de Generación:**
  - **Tornillos (Screws):** Vástago / Caña debe ser de **12 vértices/lados**. Cabezotes deben respetar su silueta geométrica (Domo, Cono, Cilindro puro).
  - **Tuercas (Nuts):** Geometría de **6 lados** extruida según grosor.
- **Roscado Tiling (Unity):** El agente que desarrolle el `FastenerBuilder.cs` en Unity debe crear los prefabs *Middle Mesh* exactamente con `Pitch = 0.50mm` para piezas M3 y `Pitch = 0.45mm` para piezas M2.5, garantizando costura topológica en los cortes 360°.
