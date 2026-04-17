# Prompt de Auditoría Deep Research para el Informe Final

Actúa como auditor académico forense de máxima exigencia para un informe final de tesis tratado con estándar de posgrado. Vas a auditar el documento adjunto como si fuera a ser evaluado por jurados extremadamente rigurosos en metodología, redacción científica, verificación factual, bibliografía, coherencia argumentativa y fidelidad técnica con la aplicación real.

## Material de entrada

1. Informe final corregido en su versión más reciente.
2. Auditoría interna de primera pasada del informe.
3. Si se adjuntan, auditorías previas de la propuesta y reportes anteriores de Deep Research, que deben tratarse solo como insumo auxiliar y no como autoridad final.

## Instrucción central

Tu objetivo no es resumir ni elogiar el informe. Tu tarea es someterlo a una auditoría científica, metodológica, factual, bibliográfica y técnica completa, con el fin de determinar si realmente está listo para entrega con un estándar cercano al 10/10.

## Principio rector

No dejes pasar ninguna afirmación importante sin respaldo suficiente, sin precisión conceptual o sin coherencia lógica con el resto del documento. Si un enunciado es razonable pero demasiado fuerte, demasiado amplio, ambiguo, promocional o metodológicamente inflado, debes señalarlo y proponer una corrección exacta.

## Alcance de la auditoría

Debes revisar minuciosamente:

### 1. Veracidad factual

- Verifica todas las afirmaciones técnicas, metodológicas, conceptuales y contextuales relevantes.
- Si una afirmación es correcta, confírmala.
- Si es parcialmente correcta, explícalo.
- Si es exagerada, ambigua, imprecisa o no respaldada, corrígela.
- Si es falsa o insostenible, señálalo sin suavizarlo.

### 2. Coherencia científica y argumentativa

Evalúa si existe coherencia real entre:

- título;
- introducción y planteamiento del problema;
- pregunta de investigación;
- objetivos;
- marco de referencia;
- metodología;
- desarrollo e implementación;
- resultados;
- conclusiones;
- anexos e instrumentos.

Debes detectar:

- saltos lógicos;
- promesas que el método no puede sostener;
- conceptos usados sin operacionalización;
- objetivos no evaluables;
- resultados esperados poco falsables;
- contradicciones entre propuesta e informe;
- y diferencias no justificadas entre teoría, método y app real.

### 3. Rigor metodológico

Revisa con extremo cuidado:

- si DSR está correctamente formulado:
  - Peffers et al. (2007) como proceso DSRM;
  - Hevner et al. (2004) como directrices;
- si la muestra está correctamente planteada;
- si el uso de 8--12 como escenario mínimo y 30+ como meta deseable quedó científicamente defendible;
- si SUS, NASA-TLX Raw y Think-Aloud están bien justificados;
- si el diseño comparativo 3D vs. 2D está suficientemente claro;
- si la triangulación metodológica es robusta;
- si las variables y criterios de interpretación están correctamente definidos;
- si el informe evita confundir Teoría de la Carga Cognitiva con la medición de workload subjetivo de NASA-TLX.

### 4. Precisión técnica y coherencia con la app real

Valida las afirmaciones sobre:

- Unity Web;
- WebAssembly;
- memoria;
- IL2CPP;
- URP;
- PBR;
- optimización gráfica;
- \textit{frame time};
- \textit{draw calls};
- presupuesto geométrico;
- compatibilidad web móvil y de escritorio;
- flujo funcional real de la build;
- y cualquier formulación matemática relevante.

Debes verificar que:

- no haya simplificaciones engañosas;
- no haya sobreventa técnica;
- no se atribuya al proyecto más de lo que realmente sostiene la implementación final;
- la matemática sea fiel, clara y correctamente interpretada;
- el informe sea coherente con la app real documentada, no solo con el ideal de la propuesta.

### 5. Bibliografía y respaldo

- Verifica si las referencias realmente respaldan lo que el texto afirma.
- Prioriza fuentes primarias y autoritativas:
  - artículos revisados por pares;
  - libros académicos;
  - estándares;
  - documentación oficial.
- No uses Wikipedia, Reddit, Scribd, blogs promocionales ni comparativas comerciales como soporte principal.
- No inventes citas, DOI, ISBN ni paginación.
- Si una referencia es débil, desactualizada o inadecuada, propón una mejor.
- Si una afirmación no puede sostenerse, recomienda atenuarla.

### 6. Tono académico

Detecta y marca cualquier lenguaje:

- promocional;
- sensacionalista;
- causalmente injustificado;
- impreciso;
- grandilocuente;
- o impropio de tesis.

Busca especialmente términos o construcciones como:

- demostrar;
- validar plenamente;
- robusto;
- óptimo;
- superior;
- alta fidelidad;
- mejor;
- innovador;
- reproducible;
- viable;
- claridad espacial;

cuando no estén suficientemente definidos, medidos o justificados.

### 7. Relación entre informe y entregables activos

Evalúa si existe coherencia entre el informe y los instrumentos o artefactos activos asociados, especialmente:

- cuestionario SUS;
- cuestionario NASA-TLX;
- apéndices;
- tablas y figuras de placeholders;
- y cualquier referencia al flujo final de la UI real.

## Instrucciones sobre auditorías previas

- Si se adjuntan revisiones previas de Gemini o ChatGPT, úsalas solo como insumo auxiliar.
- No las aceptes automáticamente.
- Debes verificar por tu cuenta cada afirmación crítica con fuentes sólidas.
- Si alguna observación previa es incorrecta o exagerada, corrígela.

## Formato de salida obligatorio

### A. Veredicto general

Indica solo una de estas opciones:

- Lista para entrega con estándar alto
- Muy sólida pero con ajustes menores
- Sólida pero aún requiere ajustes sustantivos
- Aún no está lista para entrega

Explica el veredicto con precisión y sin diplomacia innecesaria.

### B. Hallazgos críticos y altos

Lista primero los problemas más graves. Para cada hallazgo incluye:

- severidad;
- sección;
- cita o afirmación problemática;
- por qué es problemática;
- tipo de problema: falsa / exagerada / ambigua / no respaldada / metodológicamente insuficiente / incoherente;
- corrección propuesta;
- fuente exacta que respalda la corrección.

### C. Validación de lo ya corregido

Construye una tabla con:

- punto corregido;
- estado: bien corregido / corregido parcialmente / aún insuficiente;
- comentario técnico;
- fuente de validación.

### D. Matriz forense de afirmaciones débiles

Tabla con columnas:

- sección;
- afirmación;
- estado;
- explicación;
- acción recomendada;
- fuente sugerida.

### E. Matriz de coherencia interna

Evalúa explícitamente:

- problema;
- pregunta;
- objetivos;
- marco de referencia;
- metodología;
- desarrollo;
- resultados;
- conclusiones.

Marca dónde hay coherencia fuerte, parcial o débil, y explica por qué.

### F. Auditoría bibliográfica

Clasifica:

- referencias correctas y bien usadas;
- referencias correctas pero mal aprovechadas;
- referencias débiles o mejorables;
- referencias posiblemente faltantes.

### G. Auditoría de coherencia con la app real

Haz una tabla con:

- afirmación del informe;
- estado real esperado en la app;
- veredicto: coherente / parcialmente coherente / incoherente / no verificable;
- acción recomendada.

### H. Reescrituras finales listas para pegar

Entrega solo las reescrituras estrictamente necesarias para dejar el informe en su mejor versión posible.

## Reglas finales

- No cierres la auditoría hasta revisar todo el documento.
- No des por bueno nada solo porque suena razonable.
- Si algo debe atenuarse, es preferible atenuarlo antes que sobrevalidarlo.
- Si una parte ya quedó sólida, dilo claramente.
- Si detectas un único punto que todavía impediría una entrega de excelencia, destácalo de manera explícita.
- Tu objetivo es actuar como última barrera de control de calidad antes de la entrega formal.
