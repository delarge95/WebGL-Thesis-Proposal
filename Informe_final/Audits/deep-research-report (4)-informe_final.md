# Auditoría forense académica del informe final de tesis

## Veredicto general

**Solida pero aun requiere ajustes sustantivos**

La estructura general mejoró de forma real frente a la pasada previa: el documento ya distingue mejor entre DSRM y directrices de evaluación, limita con más honestidad lo que está ejecutado frente a lo solo evaluado, corrige la compatibilidad móvil a “compatibilidad esperada” y reescribe el capítulo de desarrollo como pipeline completo y no solo como inventario de scripts. Eso está bien corregido y se nota en el cuerpo metodológico y en el capítulo 4. fileciteturn0file0 fileciteturn0file1

El punto que **impide una entrega de excelencia** es uno solo, pero es decisivo: **el capítulo 5 sigue realmente abierto y, por tanto, la evaluación rigurosa del artefacto todavía no está cerrada sobre una build congelada**. El propio informe lo declara de forma explícita y, aun así, las conclusiones ya formulan cierres más fuertes de lo que ese estado permite. En DSR, la evaluación rigurosa del artefacto no es decorativa: Hevner la trata como exigencia central, y sin esa capa cerrada no se puede defender un cierre empírico de estándar alto. fileciteturn0file0 citeturn17view1turn17view2turn2view3

Dicho sin rodeos: **como documento de trabajo avanzado, el informe ya es serio y defendible en varias capas; como documento final listo para jurados muy rigurosos, todavía no**. Lo que falta no es cosmético. Falta cerrar resultados, limpiar contradicciones metodológicas residuales, completar trazabilidad bibliográfica de varias comparativas técnicas, y resolver un problema de verificabilidad externa del artefacto. fileciteturn0file0

## Hallazgos críticos y altos

**Hallazgo crítico**

**Severidad:** Crítica  
**Sección:** Capítulo 5, conclusiones y apéndices de instrumentos  
**Cita o afirmación problemática:** El informe declara que el capítulo 5 “permanece abierto” hasta contar con build congelada, rerun de auditorías, KPIs medidos y aplicación real de SUS, NASA-TLX Raw y Think-Aloud; sin embargo, en conclusiones ya afirma que “se logró construir” el visor y que esa base “respalda la viabilidad funcional del enfoque”.  
**Por qué es problemática:** La primera parte es metodológicamente honesta; la segunda adelanta cierre. En un estándar de jurado exigente, eso genera una incoherencia directa entre estado editorial, diseño metodológico y nivel de cierre argumentativo.  
**Tipo de problema:** Incoherente / metodológicamente insuficiente  
**Corrección propuesta:** Mantener el lenguaje de logros solo a nivel de **implementación preliminar ejecutada**, no de **validación cerrada**. Si el capítulo 5 no se cierra con datos reales sobre build congelada, las conclusiones deben quedar formuladas como cierre técnico parcial, no como cierre empírico.  
**Fuente exacta que respalda la corrección:** Informe final, capítulos 5 y conclusiones. fileciteturn0file0 Hevner exige evaluación rigurosa del artefacto como guía central de DSR. citeturn17view1turn17view2

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Metodología, apéndice H, figuras metodológicas  
**Cita o afirmación problemática:** El cuerpo metodológico define **NASA-TLX Raw** como promedio simple de seis dimensiones y aclara que **no** mide directamente las categorías de la Teoría de la Carga Cognitiva; sin embargo, el apéndice aún muestra “**Agregación ponderada TLX**” y “**Conclusiones de carga cognitiva**”.  
**Por qué es problemática:** Reintroduce exactamente la confusión que el cuerpo del texto ya había corregido. Si se usa la variante Raw, no corresponde hablar de ponderación pareada; y si el instrumento mide workload subjetivo, no debe presentarse como medición directa de carga cognitiva en el sentido de CLT.  
**Tipo de problema:** Incoherente / metodológicamente insuficiente  
**Corrección propuesta:** Sustituir esa figura por una que diga: “promedio simple de las seis dimensiones”, “comparación de workload subjetivo percibido entre condiciones”, y “lectura interpretativa no equivalente a CLT”.  
**Fuente exacta que respalda la corrección:** Informe final, metodología y apéndice H. fileciteturn0file0 El paquete NASA-TLX describe una escala multidimensional de workload; en práctica Raw TLX se reporta promediando las seis dimensiones normalizadas. citeturn6view0turn6view1

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Diseño comparativo 3D vs. 2D  
**Cita o afirmación problemática:** La condición B se describe solo como “soporte 2D de referencia construido a partir de documentación técnica estática del mismo sistema”, pero el informe no fija con precisión qué contiene ese baseline, cuántas vistas incluye, qué nivel de anotación textual tiene, cómo se iguala la información respecto al 3D o qué material exacto se usará por participante.  
**Por qué es problemática:** Sin operacionalización clara de la condición 2D, la comparación intra-sujeto no es plenamente reproducible ni auditable. Un jurado metodológicamente duro va a preguntar con razón: “¿contra qué 2D exactamente se comparó?”.  
**Tipo de problema:** Metodológicamente insuficiente  
**Corrección propuesta:** Añadir una subsección breve que defina el baseline 2D como paquete de láminas o PDF controlado, con lista de vistas, tipo de rotulado, número de páginas/pantallas, equivalencia informativa respecto al 3D y regla de uso durante las tareas.  
**Fuente exacta que respalda la corrección:** Informe final, diseño comparativo y protocolo de tareas. fileciteturn0file0

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Benchmarking de soluciones Web 3D y bibliografía  
**Cita o afirmación problemática:** El texto compara y valora Babylon.js, Pixel Streaming, Spline, Marmoset Viewer, Sketchfab y PlayCanvas, pero la trazabilidad bibliográfica es desigual y, en varios casos, inexistente o insuficiente en la lista final.  
**Por qué es problemática:** No basta con que las comparativas “suenen razonables”. Si el informe evalúa alternativas tecnológicas por control de pipeline, latencia, personalización o adecuación funcional, cada bloque comparativo debe poder rastrearse a documentación oficial o literatura sólida.  
**Tipo de problema:** No respaldada / bibliográficamente insuficiente  
**Corrección propuesta:** Añadir documentación oficial específica para cada tecnología comparada y atenuar todos los juicios valorativos que hoy no estén directamente soportados.  
**Fuente exacta que respalda la corrección:** El benchmarking está en el cuerpo del informe y la bibliografía no refleja de forma simétrica las tecnologías comparadas. fileciteturn0file0 Fuentes oficiales recomendadas: Babylon.js docs, PlayCanvas Editor docs, Spline docs, Marmoset Viewer docs, Sketchfab Viewer API y Pixel Streaming docs. citeturn7search24turn15view5turn15view4turn15view2turn15view6turn7search10

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Apéndice A y verificabilidad externa del artefacto  
**Cita o afirmación problemática:** El apéndice A presenta un repositorio en entity["company","GitHub","code hosting platform"] como “fuente de verdad técnica”, pero la URL consignada no fue accesible durante esta auditoría.  
**Por qué es problemática:** Si el repositorio es privado, cayó, cambió de URL o no está publicado, la tesis pierde una capa clave de verificabilidad externa justo en el punto donde más la necesita: fidelidad entre informe, app y código.  
**Tipo de problema:** No respaldada / problema de reproducibilidad  
**Corrección propuesta:** Sustituir la referencia por un enlace accesible y estable; idealmente, un release congelado, una copia archivada o un depósito con snapshot verificable. Si no habrá acceso externo, el informe debe decirlo de forma explícita y trasladar la “fuente de verdad” a anexos institucionales accesibles a jurados.  
**Fuente exacta que respalda la corrección:** Informe final, apéndice A. fileciteturn0file0 La URL apuntada no fue resoluble en la auditoría. citeturn13view0

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Marco metodológico  
**Cita o afirmación problemática:** El capítulo metodológico afirma que el trabajo se apoyó en DSR “complementado con prototipado iterativo y una **gestión tipo Scrum adaptada** al contexto académico”, pero fuera de esa frase no hay sprints, backlog, ceremonias, entregables ni criterio operacional de Scrum.  
**Por qué es problemática:** Introduce una capa metodológica adicional que luego no existe en el texto. Eso no suma rigor; lo diluye.  
**Tipo de problema:** Incoherente  
**Corrección propuesta:** O bien se elimina “Scrum” del marco metodológico, o bien se documenta una adaptación concreta con artefactos y trazabilidad mínima. En su estado actual, lo correcto es **eliminarlo**.  
**Fuente exacta que respalda la corrección:** Informe final, marco metodológico. fileciteturn0file0

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Variables, KPIs y apéndice H  
**Cita o afirmación problemática:** La pregunta de investigación incorpora “condiciones de rendimiento y memoria”, pero la memoria no queda numéricamente operacionalizada; además, el apéndice introduce “VRAM texturas” como KPI, mientras que la documentación oficial de la plataforma web de entity["company","Unity Technologies","game engine company"] enfatiza heap, asset data, GC y memoria del navegador, no una métrica estándar y cerrada de VRAM para este contexto.  
**Por qué es problemática:** La variable existe en la pregunta, pero no queda cerrada como criterio medible, y el KPI propuesto en el apéndice es metodológicamente frágil para defensa formal.  
**Tipo de problema:** Ambigua / metodológicamente insuficiente  
**Corrección propuesta:** Definir explícitamente qué se medirá como memoria: heap inicial, crecimiento del heap, tamaño `.data`, memoria total observada por navegador y peso de build. Si no se va a medir eso con trazabilidad, eliminar “memoria” de la pregunta y del KPI visual.  
**Fuente exacta que respalda la corrección:** Informe final, variables, tablas de resultados y apéndice H. fileciteturn0file0 La documentación oficial de Unity Web centra la discusión en heap, asset data y browser memory. citeturn2view5turn16view0turn16view1

**Hallazgo alto**

**Severidad:** Alta  
**Sección:** Capítulo 4  
**Cita o afirmación problemática:** El capítulo 4 ya explica mucho mejor el desarrollo, pero todavía concentra una alta densidad de evidencia visual no insertada en puntos críticos del pipeline y de la UI final.  
**Por qué es problemática:** En una auditoría de máxima exigencia, el desarrollo de un artefacto 3D interactivo no debe sostenerse solo con texto y diagramas conceptuales. Los puntos más sensibles del capítulo —importación CAD, retopología, fasteners, UI final, Blueprint, X-Ray, Thermal— siguen dependiendo de placeholders.  
**Tipo de problema:** Editorialmente insuficiente  
**Corrección propuesta:** Cerrar primero las figuras que prueban ejecución real del flujo visible y del pipeline. Si no entran todas, mover los modos ocultos a anexos y priorizar evidencia de lo efectivamente defendible en la build final.  
**Fuente exacta que respalda la corrección:** Informe final, capítulo 4. fileciteturn0file0

## Validación de lo ya corregido

| Punto corregido | Estado | Comentario técnico | Fuente de validación |
|---|---|---|---|
| Separación entre DSRM de Peffers y directrices de Hevner | **Bien corregido** | El informe ya distingue proceso de seis actividades y criterios de evaluación, que es exactamente la separación correcta en DSR. | fileciteturn0file0 citeturn2view3turn17view0 |
| Muestra deseable 30+ y escenario mínimo 8–12 | **Corregido parcialmente** | La defensa formativa del mínimo ya está mejor formulada, pero sigue siendo válida solo para detección de problemas y lectura descriptiva prudente, no para tratar SUS/NASA-TLX como cuantificación generalizable. | fileciteturn0file0 citeturn6view3turn6view4turn15view1 |
| SUS aplicado solo al prototipo 3D | **Bien corregido** | Metodológicamente consistente con el objetivo de medir usabilidad global del artefacto, no de usar SUS como comparativa 3D vs. 2D. | fileciteturn0file0 |
| NASA-TLX Raw tratado como workload subjetivo y no como CLT | **Corregido parcialmente** | El cuerpo del texto lo corrige bien, pero el apéndice H vuelve a contaminarlo con “agregación ponderada” y “conclusiones de carga cognitiva”. | fileciteturn0file0 citeturn6view0turn6view1 |
| Comparación 3D vs. 2D limitada a desempeño y workload | **Bien corregido** | La lógica comparativa ya no intenta usar SUS entre condiciones. Eso es un avance claro. | fileciteturn0file0 |
| Compatibilidad móvil formulada como esperada, no universal | **Bien corregido** | Está alineado con la documentación de Unity Web: hay soporte en ciertos navegadores móviles, pero el comportamiento efectivo depende del navegador, la versión y la memoria disponible. | fileciteturn0file0 citeturn2view0turn2view5 |
| Migración a Unity 6.3 y LOD nativo presentados como evaluados, no cerrados | **Bien corregido** | El texto ya dejó de venderlo como hecho consumado. Eso era indispensable. | fileciteturn0file0 |
| Reescritura del capítulo 4 como pipeline completo | **Corregido parcialmente** | La cobertura textual del proceso es mucho mejor; el déficit actual es de evidencia visual y de algunas tablas de trazabilidad fina. | fileciteturn0file0 fileciteturn0file1 |
| Distinción entre visible, oculto y legacy | **Bien corregido** | Esta es una de las mejores mejoras del informe. Reduce sobre-documentación y evita atribuir a la build más de lo que realmente muestra. | fileciteturn0file0 |
| Normalización 16 → 28 / 30 / 257 | **Bien corregido** | La transición histórica queda por fin explicitada y defendible como convención documental final. | fileciteturn0file0 |

## Matriz forense de afirmaciones débiles

| Sección | Afirmación | Estado | Explicación | Acción recomendada | Fuente sugerida |
|---|---|---|---|---|---|
| Benchmarking Web 3D | “Babylon.js… enfoque enterprise…” | Débil | Es un juicio valorativo válido como impresión de ingeniería, pero no aparece trazado a fuente primaria concreta. | Atenuar o respaldar con documentación oficial de Babylon.js. | Babylon.js docs. citeturn7search24 |
| Benchmarking Web 3D | “PlayCanvas… ecosistema y documentación… menores que los de Unity” | Débil | Sin criterios comparativos explícitos, queda como preferencia del autor. | Reescribir como “implica mayor esfuerzo de adopción para este alcance” o justificar con tabla de criterios. | PlayCanvas docs. citeturn15view5 |
| Benchmarking Web 3D | “Spline… no está pensada para sistemas técnicos con lógica de interacción compleja” | Débil | Es razonable, pero demasiado taxativo si no se apoya en documentación o limitaciones demostradas. | Atenuar a “su modelo de uso está más orientado a experiencias embebidas y flujos web de diseño”. | Spline docs. citeturn15view4 |
| Benchmarking Web 3D | “Marmoset Viewer… margen de personalización funcional es limitado” | Parcialmente respaldada | La documentación oficial sí muestra límites funcionales concretos, pero el informe no los cita. | Reemplazar la frase general por limitaciones específicas documentadas. | Marmoset Viewer docs. citeturn15view2 |
| Benchmarking Web 3D | “Sketchfab… menor control sobre arquitectura, lógica e integración profunda” | Parcialmente respaldada | La API del viewer existe y permite control, pero el nivel de arquitectura propia del sistema no está explicitado en el informe. | Reescribir con mayor precisión técnica. | Sketchfab Viewer API. citeturn15view6 |
| Objetivo específico 1 | “Manteniendo fidelidad visual” | Insuficiente | “Fidelidad visual” no está operacionalizada. No hay criterio observable definido para demostrarla. | Sustituir por “preservando legibilidad formal y de ensamblaje suficiente para inspección técnica”. | Revisión interna + capítulo 4. fileciteturn0file0 |
| Capítulo 4 UI | “La versión con sustento de diseño más robusto es la interfaz mobile-first” | Débil | La idea es correcta, pero “robusto” suena valorativo. | Cambiar por “mejor fundamentada en los principios citados”. | Informe final. fileciteturn0file0 |
| Capítulo 4 térmico | Variable `e` junto a `Δt` en la fórmula de enfriamiento | Ambigua | La notación puede leerse como exponencial y no como multiplicación por factor de exposición. | Renombrar la variable y añadir puntos de multiplicación. | Informe final. fileciteturn0file0 |
| Capítulo 5 / apéndices | Tabla de resultados con P1–P4 y “P5–P12”; esquema “n 8 a 12” | Insuficiente | La maqueta del cierre sigue casada con el escenario mínimo, no con el deseable 30+. | Rediseñar tablas para N abierto o llevar detalle individual a anexos. | Informe final. fileciteturn0file0 |
| Apéndice H | “VRAM texturas” como KPI | Débil | KPI poco defendible si no se define una técnica de medición estable en navegador. | Sustituir por heap, asset data, tamaño `.data`, peso total de build y memoria observada del navegador. | Unity Web memory docs. citeturn2view5turn16view1 |
| Conclusiones | “Respalda la viabilidad funcional del enfoque” | Demasiado fuerte | Es razonable a nivel técnico preliminar, pero todavía no al nivel de cierre empírico. | Atenuar. | Informe final + Hevner Guideline 3. fileciteturn0file0 citeturn17view1 |

## Matriz de coherencia interna

| Eje | Coherencia | Explicación |
|---|---|---|
| Problema ↔ Pregunta | **Fuerte** | El problema de comunicación espacial en documentación 2D está bien traducido a una pregunta sobre diferencias descriptivas 3D vs. 2D, usabilidad del prototipo y viabilidad técnica en navegador. fileciteturn0file0 |
| Pregunta ↔ Objetivos | **Fuerte** | Los objetivos específicos cubren pipeline, implementación del artefacto y evaluación. La arquitectura general está bien alineada con la pregunta. fileciteturn0file0 |
| Objetivos ↔ Metodología | **Parcial** | La lógica DSR está bien planteada, pero dos piezas quedan incompletas: la condición 2D no está suficientemente operacionalizada y la variable “memoria” no queda cerrada con umbral o métrica definida. fileciteturn0file0 |
| Metodología ↔ Desarrollo | **Fuerte** | El capítulo 4 sí documenta la construcción del artefacto con mucha más fidelidad que en versiones previas, y eso mejora claramente la coherencia vertical del informe. fileciteturn0file0 fileciteturn0file1 |
| Metodología ↔ Resultados | **Parcial** | La maqueta del capítulo 5 refleja bien la metodología, pero sigue abierta; además, algunos apéndices todavía contradicen la versión Raw de NASA-TLX y el escenario de muestra 30+. fileciteturn0file0 |
| Desarrollo ↔ Resultados | **Parcial** | El desarrollo ya define qué debiera medirse, pero aún faltan métricas pre/post optimización y evidencia cerrada sobre la build final. fileciteturn0file0 |
| Resultados ↔ Conclusiones | **Débil** | Las conclusiones van un paso por delante del cierre empírico. Esa es la fractura interna más importante del informe. fileciteturn0file0 |
| Apéndices e instrumentos ↔ Metodología | **Parcial** | La mayor parte del aparato instrumental ya está alineada, pero el apéndice H arrastra contradicciones residuales relevantes. fileciteturn0file0 |

## Auditoría específica del capítulo 4

### Lo que el capítulo 4 ya explica bien

El capítulo 4 ya hace varias cosas de manera francamente sólida. La normalización editorial **16 → 28 piezas canónicas → 30 anchors → 257 renderers/colliders** está bien planteada y resuelve un problema clásico de tesis de desarrollo: la coexistencia de cifras históricas incompatibles. También está bien resuelta la explicación de múltiples rutas CAD, el encuadre de MAD-T como **referencia de producción adaptada** y no como marco académico duro, la lógica de fasteners como solución semántica y geométrica, la honestidad sobre mobile-first frente a escritorio, y la distinción entre visible, oculto y legacy. Esa parte ya tiene densidad técnica suficiente para convencer a un jurado de que hubo un proceso real y no un relato superficial. fileciteturn0file0

Otro punto fuerte es la honestidad del subsistema térmico. El capítulo lo desacopla correctamente de cualquier pretensión de FEA en tiempo real y lo formula como simulación híbrida heurística, lo cual es técnicamente mucho más defendible que intentar venderlo como termodinámica físicamente calibrada. El cercado conceptual aquí es bueno. fileciteturn0file0

### Lo que sigue faltando

Lo que falta no es más prosa general; es **evidencia concreta**. El capítulo todavía necesita cerrar, como mínimo, estas piezas:

- una tabla de trazabilidad **familia/pieza → ruta de entrada → acción aplicada → salida runtime**;
- una tabla breve **antes/después** del saneamiento geométrico con triángulos, draw calls o familias sustituidas;
- ejemplos explícitos de piezas **importadas directas**, **pasadas por MoI3D**, **rehechas** y **modeladas desde cero**;
- una comparación **móvil vs. escritorio** del layout realmente ejecutado;
- la evidencia visual mínima del flujo visible final: Hero, selección, bottom sheet, hotspot, Analyze, X-Ray, Cut, Thermal y al menos un preset Studio. fileciteturn0file0

Hoy el capítulo ya dice mucho más verdad que antes, pero todavía no **muestra** suficiente. Para un capítulo de desarrollo en una tesis aplicada, eso importa muchísimo. A fecha de esta auditoría, el documento conserva una densidad alta de placeholders en el capítulo 4; varios están bien ubicados, pero siguen ocupando precisamente los puntos donde un jurado espera pruebas visuales del desarrollo ejecutado. fileciteturn0file0

### Lo que está exagerado o insuficientemente respaldado

La parte más débil no está en la descripción del pipeline interno, sino en algunas frases valorativas alrededor de herramientas y diseño. Expresiones como “más robusto”, “muy alta fidelidad”, “menor control”, “ecosistema menor” o “más adecuada” necesitan o bien fuente explícita, o bien atenuación. Lo mismo ocurre con “fidelidad visual” si no se define qué se preserva exactamente: contorno, ensamblaje, legibilidad de piezas, lectura material o continuidad formal. fileciteturn0file0

También conviene corregir la notación del enfriamiento térmico y la semántica de algunos KPIs del apéndice, porque un jurado técnico sí puede detenerse en esos detalles. La matemática del capítulo es razonable cuando se lee en clave de modelo operativo o heurístico; el problema no es tanto la idea como la notación ambigua en algunos puntos. fileciteturn0file0

### Qué visuales o tablas faltan para que el capítulo quede realmente sólido

Si hubiera que priorizar solo lo indispensable, faltan estas cinco piezas:

1. **Tabla de reconciliación 16/28/30/257** con significado preciso de cada conteo.  
2. **Tabla de rutas CAD por familia** con formato de entrada, herramienta aplicada, decisión y salida.  
3. **Comparativa visual high-poly/CAD vs. versión optimizada** con alguna cifra mínima.  
4. **Comparativa UI móvil vs. escritorio** para respaldar la tesis de mobile-first y adaptación funcional.  
5. **Lámina de flujo visible real** con capturas de la build: Hero, selección, bottom sheet, Analyze, X-Ray, Thermal. fileciteturn0file0

Sin esas piezas, el capítulo 4 ya es técnicamente mucho mejor que antes, pero todavía no alcanza la contundencia visual y probatoria que pide una defensa de posgrado.

## Auditoría bibliográfica

### Referencias correctas y bien usadas

Las referencias más sólidas del informe son, en general, las que sostienen el andamiaje metodológico y técnico de base: Peffers para el proceso DSRM, Hevner para las siete directrices, Brooke/Bangor/Sauro para SUS, Hart y Staveland más Hart 2006 para NASA-TLX, Sweller y Sweller et al. para CLT, y la tríada Kajiya/Cook-Torrance/Schlick/Walter para la parte de PBR. Esa columna vertebral es correcta y, salvo por la contradicción residual del apéndice TLX, está bien usada. fileciteturn0file0 citeturn2view3turn17view0turn17view1turn6view0turn6view2turn9search0turn9search1turn9search14turn9search23

Las referencias oficiales de la plataforma Web de entity["company","Unity Technologies","game engine company"] también son pertinentes para memoria, IL2CPP, compatibilidad de navegador, interacción con browser scripting y URP. Además, la terminología más actual de Unity ya usa “Unity Web platform” y “Web build”, lo cual conviene reflejar de manera coherente en todo el informe para evitar una nomenclatura híbrida entre “Unity Web”, “WebGL” y “Web build”. citeturn2view4turn2view5turn2view0turn16view0turn16view2

### Referencias correctas pero mal aprovechadas

Varias referencias son válidas, pero hoy están mal explotadas o subintegradas. El caso más claro es el benchmarking de herramientas: el texto compara múltiples opciones, pero la bibliografía no acompaña esa comparación con documentación oficial suficiente. También hay referencias de cognición y aprendizaje que podrían respaldar mejor la justificación conceptual, pero en el texto quedan secundarias o apenas visibles. fileciteturn0file0

Un caso puntual: Miller (1956) está arrastrado hacia una justificación sobre organización visual y categorías cromáticas que, tal como está escrita, necesitaría mejor anclaje perceptual o de accesibilidad. Para color/contraste, Ware sirve mejor que Miller. Para memoria de trabajo inmediata y carga informacional, Miller o Cowan sí son pertinentes, pero no exactamente para todo lo que el párrafo actual les hace cargar. fileciteturn0file0

### Referencias débiles o mejorables

Aquí hay cuatro problemas claros.

Primero, **Blender Bros** es una fuente útil como referente de práctica profesional, pero no debe cargarse con peso metodológico académico superior al que el propio texto ya le concede; debe seguir tratándose como heurística de producción y no como autoridad científica. Segundo, la referencia de Nielsen 2000 es útil como tradición de HCI, pero para defender el uso cuantitativo de SUS/NASA-TLX con muestra pequeña conviene acompañarla con literatura más específica de tamaño muestral. Tercero, hay referencias probablemente **no usadas o prescindibles** en la versión actual del informe, como Ries, y otras que parecen estar en la lista pero no se traducen en citación visible sustantiva, como varias de multimedia learning, patrones o WebGPU comparativo. Cuarto, la referencia a repositorio de three.js en entity["company","GitHub","code hosting platform"] puede servir para acreditar existencia del proyecto, pero **no** para sostener comparativas fuertes sobre viabilidad, control de pipeline o costo de implementación. fileciteturn0file0

### Referencias posiblemente faltantes

Faltan, como mínimo, referencias oficiales para las plataformas comparadas en el benchmarking:

- documentación de Babylon.js;  
- documentación de entity["company","PlayCanvas","web engine company"];  
- documentación de entity["company","Spline","3d design company"];  
- documentación de Marmoset Viewer de entity["company","Marmoset","3d software company"];  
- Viewer API de entity["company","Sketchfab","3d platform company"];  
- documentación oficial de Pixel Streaming de entity["company","Epic Games","game engine company"]. citeturn7search24turn15view5turn15view4turn15view2turn15view6turn7search10

También convendría añadir una fuente más específica para el problema de tamaño muestral cuando se reportan **cuestionarios cuantitativos**. El estado actual del informe ya formula bien la prudencia interpretativa, pero si quiere blindarse frente a jurados duros debe citar algo más preciso que la heurística clásica de usuarios mínimos. El estudio de Tullis y Stetson es especialmente útil para explicar por qué 8–12 puede servir como piso operativo, pero no como garantía fuerte de estabilidad inferencial o de benchmark. citeturn15view1turn6view3turn6view4

## Reescrituras finales listas para pegar

### Reescritura para la apertura de conclusiones

Sustituir el primer párrafo de conclusiones por este texto, para que no cierre más de lo que el expediente metodológico permite:

> Las conclusiones de esta versión del informe deben interpretarse como **cierre técnico-documental del desarrollo del artefacto** y no como cierre empírico definitivo de la investigación. A la fecha de esta redacción, el prototipo funcional existe, su arquitectura es trazable y su flujo principal es verificable a nivel de implementación; sin embargo, la validación final del artefacto sigue condicionada a la congelación de la build, la medición de KPIs técnicos y la ejecución del trabajo de campo con SUS, NASA-TLX Raw y protocolo Think-Aloud.

### Reescritura para eliminar la falsa capa Scrum

Sustituir la frase metodológica donde aparece Scrum por esta versión más limpia:

> El trabajo se formuló como investigación aplicada, con enfoque mixto y predominio cualitativo-formativo. Dado que el proyecto articuló diseño, construcción y evaluación de un artefacto digital, se apoyó metodológicamente en Design Science Research, complementado con prototipado iterativo, observación de uso, cuestionarios estandarizados y medición de KPIs técnicos. fileciteturn0file0

### Reescritura para la condición 2D de referencia

Añadir una subsección breve dentro de “Diseño comparativo y condiciones de prueba”:

> **Definición operativa de la condición 2D.** La condición 2D se implementará mediante un paquete controlado de documentación técnica estática del mismo ensamblaje, compuesto por [n] vistas ortogonales y/o axonométricas, rotulado homogéneo de componentes, y una ficha textual equivalente a la información disponible en la condición 3D para las tareas seleccionadas. Su función no es replicar todas las affordances del prototipo, sino ofrecer un medio estático de referencia con equivalencia informativa suficiente para comparar desempeño en tareas y carga de trabajo percibida entre medios de representación. fileciteturn0file0

### Reescritura para el objetivo técnico sobre C#, IL2CPP y WebAssembly

Sustituir cualquier formulación resumida del tipo “sistema en C# compilado a WebAssembly” por esta versión precisa:

> Implementar el prototipo interactivo en la plataforma Web de Unity, con lógica desarrollada en C# y desplegada mediante la cadena de compilación del motor para build web, incluyendo conversión de ensamblados administrados, generación de C++ y ejecución final en entorno WebAssembly. citeturn2view4turn16view0

### Reescritura para Appendix H y la figura de NASA-TLX

Sustituir los rótulos problemáticos de la figura metodológica por esta lógica textual:

> **Esquema de consolidación de NASA-TLX Raw por condición.**  
> Dimensiones: demanda mental, demanda física, demanda temporal, rendimiento invertido, esfuerzo y frustración.  
> Procesamiento: normalización a escala 0–100 y **promedio simple de las seis dimensiones** para cada condición.  
> Interpretación: comparación descriptiva de **workload subjetivo percibido** entre prototipo 3D y referencia 2D.  
> Alcance interpretativo: este instrumento **no** se usará como medición directa de carga cognitiva en el sentido estricto de la Teoría de la Carga Cognitiva. citeturn6view0turn6view1

### Reescritura para la variable memoria y sus KPIs

Sustituir la formulación ambigua de memoria por una operacionalización defendible:

> En este estudio, la variable de memoria se operacionaliza mediante cuatro indicadores observables sobre la build congelada: (a) tamaño inicial del heap configurado para Web, (b) crecimiento efectivo del heap durante la sesión, (c) tamaño de los artefactos de build relevantes para carga inicial —en particular `.data` y archivos asociados— y (d) memoria total observada del proceso en navegador durante el escenario de prueba documentado. No se utilizará “VRAM de texturas” como KPI principal salvo que exista una técnica de medición explícita, estable y trazable para el entorno evaluado. citeturn2view5turn16view1

### Reescritura para “fidelidad visual”

Sustituir en objetivos o capítulo 4 la expresión “manteniendo fidelidad visual” por una formulación evaluable:

> preservando la **legibilidad formal del ensamblaje**, la identificación correcta de piezas y la lectura espacial suficiente para tareas de inspección técnica en navegador. fileciteturn0file0

### Reescritura para la notación del enfriamiento térmico

Sustituir la variable `e` en la ecuación de enfriamiento por un símbolo menos ambiguo y explicitar multiplicación:

> El enfriamiento ambiental se aproxima como  
> \[
> \Delta T_{cooling} = (T_{amb} - T_{actual}) \cdot c_r \cdot \kappa_{exp} \cdot \Delta t
> \]
> donde \(c_r\) es la tasa de enfriamiento y \(\kappa_{exp}\) es el factor de exposición convectiva. Esta formulación se usa como aproximación operativa del solver y no como modelo físico calibrado en unidades SI. fileciteturn0file0