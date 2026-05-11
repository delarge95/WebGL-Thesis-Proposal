\# Plan de OptimizaciÃ³n Agresiva (550MB â†’ 15MB)

> Actualizacion Unity 2026-05-09: la implementacion runtime ya no usa el conteo preliminar de 240-300 fasteners como fuente operativa. Tras limpiar falsos positivos y reconocer grommets/tuercas catalogadas, el baseline activo queda en 19 familias y 170 instancias primitivas de fastener. `HMX5V-GUAN-DINGWEI` se trata como subpieza estructural del brazo, no como tornillo ni como reemplazo modular; `HUAN-GUIJIAO` y `GPSV5-ZHIJIA-LUOMAO` se tratan como fasteners solo por clasificacion explicita del catalogo. Las discrepancias restantes deben revisarse desde `holybro_fastener_reconciliation.json` y no asumirse por nombre.




Este plan define el flujo de trabajo tÃ©cnico y automatizado para reducir drÃ¡sticamente el peso geomÃ©trico originario de exportaciones CAD (3D MoI) con destino a Unity WebGL. Se adopta una estrategia de \*\*Instanciamiento Proxy en Blender\*\* complementada con un \*\*Ensamblaje Modular Tiling en Unity\*\*.



\## Resumen del DiagnÃ³stico de Escena (VÃ­a MCP)

\- Existen \*\*24 familias Ãºnicas de Hardware\*\* (tornillos `GB70`, `M3`, tuercas `FALAN`, etc.).

\- Componen entre \*\*240 a 300 instancias\*\* esparcidas por la escena.

\- SÃ³lo los Fasteners consumen cerca de \*\*3.5 a 4 Millones de VÃ©rtices\*\* totales. Aislar y agilizar esta geometrÃ­a es la prioridad absoluta para el alcance en WebGL.



\---



\## FASE 1: El Sistema Array/Tiling en Unity (Tornillo Modular)



Intentar escalar (`Scale`) una rosca o generar sus vÃ©rtices matemÃ¡ticamente durante el Runtime (C#) destruirÃ­a el "Pitch" (Paso) de la hÃ©lice, provocando roscas elongadas errÃ³neamente y errores topolÃ³gicos en los empalmes del cabezote. \*\*La soluciÃ³n AAA para esto es el PatrÃ³n Tiling (Apilamiento)\*\*.



\### Estructura de la LibrerÃ­a de Piezas Dummys (Para hacer en Blender)

En tu colecciÃ³n `Remaking`, por cada "Familia" de mÃ©trica (ej. M3, M2.5), vas a modelar separadamente:

1\. \*\*Top Mesh:\*\* Un cabezote High-Poly (ej. `M3\\\_Head\\\_Socket` o `M3\\\_Head\\\_Button`) que incluye el margen de descenso y el inicio suave de la rotaciÃ³n del primer hilo de rosca.

2\. \*\*Middle Mesh:\*\* Exactamente \*\*1 vuelta (360Â°)\*\* del cuerpo de la rosca. Sus vÃ©rtices deben iniciar en $Z=0$ y terminar cerrando el ciclo en su altura correspondiente al Paso (Ej. $Z=0.5mm$).

3\. \*\*Bottom Mesh:\*\* La punta del tornillo, donde el corte de la hÃ©lice desciende hacia el interior.



\### LÃ³gica C# de Auto-Ensamblaje

Desarrollaremos un script `FastenerBuilder.cs` en Unity que leerÃ¡ quÃ© largo de tornillo se requiere. En vez de escalar, el script usarÃ¡ `GPU Instancing` para copiar y apilar el \*\*Middle Mesh\*\* iterativamente.

\* Ejemplo para un tornillo de 6 vueltas: Instancia 1 Head, luego un bucle `for` que posiciona e instancia 6 Middle Meshes consecutivos (sin rotarlos, simplemente apilÃ¡ndolos arriba uno de otro en el eje Z), y 1 Bottom Mesh final.

\* Todo queda perfectamente continuo, realista en la fÃ­sica de los metales y empacado con \*\*LOD Groups\*\* para que desde lejos, Unity simplemente apague el modelo High-Poly y devuelva visualmente el Proxy de bajo nivel que crearemos a continuaciÃ³n.



\---



\## FASE 2: La DestrucciÃ³n y Proxy Inteligente (Blender Pipeline)



No podemos dejar la geometrÃ­a densa original en Blender. Un Addon de Python automatizarÃ¡ la sustituciÃ³n de esos 300 componentes pesados por Proxies ultralivianos (LOD1/LOD2), los cuales no son simples cilindros rectos, sino aproximaciones morfolÃ³gicamente idÃ©nticas al cabezote del original.



\### LÃ³gica de Reemplazo del Addon

Cuando inicies el script, este ofrecerÃ¡ configurar \*\*Tipos de GeometrÃ­a Proxy\*\*, basÃ¡ndose en las 3 tipologÃ­as universales dictadas en la imagen de referencia:

\* \*\*Socket Head:\*\* (Capa cilÃ­ndrica recta).

\* \*\*Button/Pan Head:\*\* (Cabezal en Domo o EsfÃ©rico biselado).

\* \*\*Countersunk/Flat:\*\* (Cabezal cÃ³nico hundido).



\*\*El Algoritmo paso a paso por Tornillo:\*\*

1\. \*\*MediciÃ³n AnalÃ­tica de Bounding Box\*\*: El script separarÃ¡ matemÃ¡ticamente el tornillo en 2 mitades. CalcularÃ¡ el DiÃ¡metro/Altura especÃ­fica de la Cabecera, y el DiÃ¡metro/Altura especÃ­fica del Cuerpo/CaÃ±a.

2\. \*\*ConstrucciÃ³n Procedural del Proxy\*\*:

&#x20;  - Se levantarÃ¡ un cilindro base de \*\*12 Lados\*\* para la caÃ±a midiendo el largo requerido.

&#x20;  - Dependiendo del tipo de cabezote seleccionado (o auto-detectado si logramos perfilarlo por Bounding Box), el script extruirÃ¡ y escalarÃ¡ los vÃ©rtices superiores (Cono para Countersunk, Domos segmentados para Button, Cilindro puro para Socket).

3\. \*\*OptimizaciÃ³n TopolÃ³gica de la Tapa\*\*: La parte plana superior se resolverÃ¡ con un "Triangle Fan" (Uniendo todo a un vÃ©rtice central), lo cual matemÃ¡ticamente reduce la cantidad de polÃ­gonos versus una topologÃ­a de grid de quads sin sacrificar ningÃºn sombreado en WebGL. \*Para Proxies LOD1, 12 a 8 lados geomÃ©tricos es la perfecciÃ³n absoluta entre silueta y peso.\*

4\. \*\*Metadatos y HeredaciÃ³n\*\*: El Proxy hereda la posiciÃ³n, rotaciÃ³n mediante \*eigenvectors\* PCA (para alinear eje con el original) y nominarÃ¡ el objeto como `SYS\\\_FASTENER\\\_M3\\\_Button\\\_8mm`.

5\. \*\*EliminaciÃ³n\*\*: Se esconde o elimina la geometrÃ­a NURBS pesada de ese tornillo procesado.



\---



\## FASE 3: Hoja de Ruta TÃ¡ctica para Ejecutar Esto



1\. \*\*Terminar Remaking Manual\*\*: Modela las versiones perfectas (High Poly) de los \*Top/Middle/Bottom\* meshes de tus mÃ©tricas principales (M3, M2.5) asegurÃ¡ndote que el Tiling encaje en 360 grados.

### LÃ³gica de Reemplazo del Addon

Cuando inicies el script, este ofrecerÃ¡ configurar **Tipos de GeometrÃ­a Proxy**, basÃ¡ndose en las 3 tipologÃ­as universales dictadas en la imagen de referencia:

* **Socket Head:** (Capa cilÃ­ndrica recta).
* **Button/Pan Head:** (Cabezal en Domo o EsfÃ©rico biselado).
* **Countersunk/Flat:** (Cabezal cÃ³nico hundido).

**El Algoritmo paso a paso por Tornillo:**

1. **MediciÃ³n AnalÃ­tica de Bounding Box**: El script separarÃ¡ matemÃ¡ticamente el tornillo en 2 mitades. CalcularÃ¡ el DiÃ¡metro/Altura especÃ­fica de la Cabecera, y el DiÃ¡metro/Altura especÃ­fica del Cuerpo/CaÃ±a.
2. **ConstrucciÃ³n Procedural del Proxy**:
   - Se levantarÃ¡ un cilindro base de **12 Lados** para la caÃ±a midiendo el largo requerido.
   - Dependiendo del tipo de cabezote seleccionado (o auto-detectado si logramos perfilarlo por Bounding Box), el script extruirÃ¡ y escalarÃ¡ los vÃ©rtices superiores (Cono para Countersunk, Domos segmentados para Button, Cilindro puro para Socket).
3. **OptimizaciÃ³n TopolÃ³gica de la Tapa**: La parte plana superior se resolverÃ¡ con un "Triangle Fan" (Uniendo todo a un vÃ©rtice central), lo cual matemÃ¡ticamente reduce la cantidad de polÃ­gonos versus una topologÃ­a de grid de quads sin sacrificar ningÃºn sombreado en WebGL. *Para Proxies LOD1, 12 a 8 lados geomÃ©tricos es la perfecciÃ³n absoluta entre silueta y peso.*
4. **Metadatos y HeredaciÃ³n**: El Proxy hereda la posiciÃ³n, rotaciÃ³n mediante *eigenvectors* PCA (para alinear eje con el original) y nominarÃ¡ el objeto como `SYS_FASTENER_M3_Button_8mm`.
5. **EliminaciÃ³n**: Se esconde o elimina la geometrÃ­a NURBS pesada de ese tornillo procesado.

---

## FASE 3: Hoja de Ruta TÃ¡ctica para Ejecutar Esto

1. **Terminar Remaking Manual**: Modela las versiones perfectas (High Poly) de los *Top/Middle/Bottom* meshes de tus mÃ©tricas principales (M3, M2.5) asegurÃ¡ndote que el Tiling encaje en 360 grados.
2. **Yo desarrollarÃ© el Addon "Fastener Proxy Automator" (Blender)**: ProgramarÃ© la herramienta que leerÃ¡, medirÃ¡, extruirÃ¡ los cabezotes geomÃ©tricos de 12 lados (identicos a la imagen de referencia optimizada) y auto-reemplazarÃ¡ los tornillos en Batch.
3. **Limpieza Batch Superficies**: Haremos un pase rÃ¡pido de DecimaciÃ³n Plana (Angle Limit ~7Â°) para cables y tuercas simples.
4. **Exportar a Unity**: El FBX viaja masivamente optimizado.
5. **Implementar Tooling Unity**: Escribiremos el sistema Modular (`FastenerBuilder`) en C# que interpreta los marcadores `SYS_FASTENER` e inyecta la geometrÃ­a Tiling (High Poly) cuando la cÃ¡mara de inspecciÃ³n requiera el LOD0.

---

## FASE 4: CatÃ¡logo TÃ©cnico Ultradetallado de Fasteners (Extracto del MCP de Blender)

A continuaciÃ³n se detalla la lista exhaustiva de toda la tornillerÃ­a y herrajes (Fasteners) encontrados en el ensamblaje actual del dron. Estos datos tÃ©cnicos fueron extraÃ­dos directamente de la geometrÃ­a del archivo en Blender (Bake Masters) y cruzados con normativas ISO/DIN/GB para que el agente de Unity/Blender tenga medidas exactas para el reemplazo algorÃ­tmico y procedural.

### 1. Familia: Socket Head Cap Screws (Cabeza CilÃ­ndrica Allen - GB70 / DIN 912)
La familia `GB70` corresponde a tornillos de cabeza cilÃ­ndrica con hueco hexagonal.
- **Forma de la cabeza:** CilÃ­ndrica recta (ideal para Proxy LOD en forma de cilindro puro de 12 lados).
- **Inicio de rosca:** TÃ­picamente a ras del vÃ¡stago, justo debajo del cabezal.

| ID Familia | TamaÃ±o | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `GB70-M25-6` | M2.5 x 6mm | 24 | 4.43 x 4.46 x 8.24 mm | **Ã˜ Hilo:** 2.5mm, **Pitch:** 0.45mm, **Ã˜ Cabezal:** ~4.5mm, **Alto Cabezal:** 2.5mm |
| `GB70-M25-10` | M2.5 x 10mm | 8 | 4.44 x 4.46 x 12.24 mm | **Ã˜ Hilo:** 2.5mm, **Pitch:** 0.45mm |
| `GB70-M25-12` | M2.5 x 12mm | 12 | 4.44 x 4.46 x 14.24 mm | **Ã˜ Hilo:** 2.5mm, **Pitch:** 0.45mm |
| `GB70-M3-6` | M3 x 6mm | 16 | 5.40 x 5.40 x 8.70 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ã˜ Cabezal:** ~5.4mm, **Alto Cabezal:** 3.0mm |
| `GB70-M3-8-DING`| M3 x 8mm | 12 | 5.40 x 5.40 x 10.70 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-21-DING`| M3 x 21mm | 2 | 5.40 x 5.40 x 20.70 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-25-DING`| M3 x 25mm | 2 | 5.40 x 5.40 x 24.70 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm |
| `GB70-M3-38` | M3 x 38mm | 16 | 5.40 x 5.40 x 40.70 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm |

### 2. Familia: Countersunk / Flat Head Screws (Cabeza Avellanada/Plana - CHEN-LIU)
La etiqueta `CHEN-LIU` o equivalente indica un cabezote avellanado para quedar a ras de la superficie (como ISO 10642 / DIN 7991).
- **Forma de la cabeza:** Cono truncado hundido (Proxy LOD: Cilindro con extremo superior escalado radialmente).
- **Inicio de rosca:** A nivel de la superficie plana del cabezal en la mediciÃ³n total.

| ID Familia | TamaÃ±o | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `M25-6-CHEN-LIU` | M2.5 x 6mm | 12 | 4.81 x 4.81 x 5.90 mm | **Ã˜ Hilo:** 2.5mm, **Pitch:** 0.45mm, **Ã˜ Cabezal mÃ¡x:** ~4.8mm |
| `M3-16-CHEN-LIU` | M3 x 16mm | 2 | 5.80 x 5.80 x 15.80 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ã˜ Cabezal mÃ¡x:** ~5.8mm |

### 3. Familia: Pan Head / Button Head Screws (Cabeza Domo - PAN)
Tornillos de cabeza abovedada (Button / Pan Head, tÃ­picamente ISO 7380).
- **Forma de la cabeza:** Hemisferio aplanado o domo (Proxy LOD: Cilindro con extrusiÃ³n superior segmentada para curvar).
- **Inicio de rosca:** Debajo de la base plana del domo.

| ID Familia | TamaÃ±o | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `M3-10-PAN-DING`| M3 x 10mm | 4 | 5.60 x 5.60 x 10.10 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ã˜ Cabezal:** ~5.6mm |
| `M3-14-PAN` | M3 x 14mm | 4 | 5.60 x 5.60 x 15.80 mm | **Ã˜ Hilo:** 3.0mm, **Pitch:** 0.50mm, **Ã˜ Cabezal:** ~5.6mm |

### 4. Familia: Nuts & Lock Nuts (Tuercas y Tuercas de Seguridad Autoblocantes)
Tuercas hexagonales estÃ¡ndar y con inserto de nylon (ZSLM - Tuerca Autoblocante, DIN 985).
- **Forma:** Hexagonal (Proxy LOD: Cilindro puro de 6 lados exactos).

| ID Familia | Tipo | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `LM-M3-DING` | Tuerca Hex M3 | 8 | 5.40 x 6.24 x 2.50 mm | **Hex Ancho (Flat):** 5.5mm, **Pitch Int:** 0.50mm, **Grosor:** 2.5mm |
| `LM-M3-NILONG` | Tuerca Nylon M3 | 2 | 5.40 x 6.24 x 2.50 mm | *Material Nylon* |
| `ZSLM-M25` | Autoblocante M2.5 | 4 | 4.50 x 5.19 x 3.17 mm | **Hex Ancho:** 5.0mm, **Pitch Int:** 0.45mm, **Grosor:** ~3.2mm |
| `ZSLM-M3-DING` | Autoblocante M3 | 8 | 5.40 x 6.24 x 3.80 mm | **Hex Ancho:** 5.5mm, **Pitch Int:** 0.50mm, **Grosor:** ~3.8mm |
| `ZSLM-M3-FALAN`| Tuerca Brida M3 | 16 | 7.60 x 7.60 x 4.80 mm | **Brida (Flange) Ã˜:** ~7.6mm, **Grosor:** 4.8mm |

### 5. Familia: Standoffs / Spacers (Separadores de Nylon - NILONGZHU)
Separadores utilizados tÃ­picamente para montar PCBs (como la Pixhawk o la PDB).
- **Forma:** Cilindros rectos o columnas hexagonales.
- **Conexiones:** Usualmente de tipo Hembra-Hembra o Macho-Hembra.

| ID Familia | Tipo | Instancias | Bounding Box Real (W x D x H) | Specs. Normalizadas |
|------------|--------|------------|-------------------------------|---------------------|
| `NILONGZHU-M25-5`| Separador M2.5 | 4 | 4.50 x 5.00 x 5.19 mm | **Paso:** 0.45mm |
| `NILONGZHU-M3-5`| Separador M3 | 4 | 5.00 x 5.40 x 6.24 mm | **Paso:** 0.50mm |

### Resumen para Agentes Especializados
- **TopologÃ­a Obligatoria de GeneraciÃ³n:**
  - **Tornillos (Screws):** VÃ¡stago / CaÃ±a debe ser de **12 vÃ©rtices/lados**. Cabezotes deben respetar su silueta geomÃ©trica (Domo, Cono, Cilindro puro).
  - **Tuercas (Nuts):** GeometrÃ­a de **6 lados** extruida segÃºn grosor.
- **Roscado Tiling (Unity):** El agente que desarrolle el `FastenerBuilder.cs` en Unity debe crear los prefabs *Middle Mesh* exactamente con `Pitch = 0.50mm` para piezas M3 y `Pitch = 0.45mm` para piezas M2.5, garantizando costura topolÃ³gica en los cortes 360Â°.
