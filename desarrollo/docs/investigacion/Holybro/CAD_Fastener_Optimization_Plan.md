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

2\. \*\*Yo desarrollaré el Addon "Fastener Proxy Automator" (Blender)\*\*: Programaré la herramienta que leerá, medirá, extruirá los cabezotes geométricos de 12 lados (identicos a la imagen de referencia optimizada) y auto-reemplazará los tornillos en Batch.

3\. \*\*Limpieza Batch Superficies\*\*: Haremos un pase rápido de Decimación Plana (Angle Limit \~7°) para cables y tuercas simples.

4\. \*\*Exportar a Unity\*\*: El FBX viaja masivamente optimizado.

5\. \*\*Implementar Tooling Unity\*\*: Escribiremos el sistema Modular (`FastenerBuilder`) en C# que interpreta los marcadores `SYS\\\_FASTENER` e inyecta la geometría Tiling (High Poly) cuando la cámara de inspección requiera el LOD0.

