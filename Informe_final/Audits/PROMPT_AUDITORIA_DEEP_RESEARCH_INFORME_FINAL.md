# Prompt de Auditoria Deep Research para el Informe Final

Actua como auditor academico forense de maxima exigencia para un informe final de tesis tratado con estandar de posgrado. Vas a auditar el documento adjunto como si fuera a ser evaluado por jurados extremadamente rigurosos en metodologia, redaccion cientifica, verificacion factual, bibliografia, coherencia argumentativa, fidelidad tecnica con la aplicacion real y calidad editorial del capitulo de desarrollo.

## Material de entrada

1. Informe final corregido en su version mas reciente.
2. Auditoria interna de primera pasada del informe.
3. Si se adjuntan, auditorias previas de propuesta o reportes anteriores de Deep Research, que deben tratarse solo como insumo auxiliar y no como autoridad final.

## Instruccion central

Tu objetivo no es resumir ni elogiar el informe. Tu tarea es someterlo a una auditoria cientifica, metodologica, factual, bibliografica, tecnica y editorial completa, con el fin de determinar si realmente esta listo para entrega con un estandar cercano al 10/10.

## Condicion editorial clave

Este informe final debe ser tratado como **documento aun en desarrollo**, no como documento cerrado. Por ello:

- no penalices automaticamente la presencia de placeholders si estan claramente marcados y ubicados en secciones coherentes;
- si detectas placeholders mal ubicados, insuficientes, excesivos o incoherentes con el texto, debes decirlo;
- evalua si el informe deja claro que el capitulo 5 sigue abierto y que ciertos resultados dependen de la build congelada y del trabajo de campo real;
- distingue entre problemas de estructura editorial pendiente y afirmaciones tecnicas o metodologicas incorrectas.

## Principio rector

No dejes pasar ninguna afirmacion importante sin respaldo suficiente, sin precision conceptual o sin coherencia logica con el resto del documento. Si un enunciado es razonable pero demasiado fuerte, demasiado amplio, ambiguo, promocional o metodologicamente inflado, debes senalarlo y proponer una correccion exacta.

## Alcance de la auditoria

Debes revisar minuciosamente:

### 1. Veracidad factual

- Verifica todas las afirmaciones tecnicas, metodologicas, conceptuales y contextuales relevantes.
- Si una afirmacion es correcta, confirmala.
- Si es parcialmente correcta, explicalo.
- Si es exagerada, ambigua, imprecisa o no respaldada, corrigela.
- Si es falsa o insostenible, senalalo sin suavizarlo.

### 2. Coherencia cientifica y argumentativa

Evalua si existe coherencia real entre:

- titulo;
- introduccion y planteamiento del problema;
- pregunta de investigacion;
- objetivos;
- marco de referencia;
- metodologia;
- desarrollo e implementacion;
- resultados;
- conclusiones;
- apendices e instrumentos.

Debes detectar:

- saltos logicos;
- promesas que el metodo no puede sostener;
- conceptos usados sin operacionalizacion;
- objetivos no evaluables;
- resultados esperados poco falsables;
- contradicciones entre propuesta e informe;
- y diferencias no justificadas entre teoria, metodo y app real.

### 3. Rigor metodologico

Revisa con extremo cuidado:

- si DSR esta correctamente formulado:
  - Peffers et al. (2007) como proceso DSRM;
  - Hevner et al. (2004) como directrices;
- si la muestra esta correctamente planteada;
- si el uso de 8-12 como escenario minimo y 30+ como meta deseable quedo cientificamente defendible;
- si SUS, NASA-TLX Raw y Think-Aloud estan bien justificados;
- si el diseno comparativo 3D vs. 2D esta suficientemente claro;
- si la triangulacion metodologica es robusta;
- si las variables y criterios de interpretacion estan correctamente definidos;
- si el informe evita confundir Teoria de la Carga Cognitiva con la medicion de workload subjetivo de NASA-TLX;
- si el capitulo 5 realmente responde a los KPIs y objetivos vigentes del proyecto.

### 4. Precision tecnica y fidelidad con la app real

Valida las afirmaciones sobre:

- Unity Web;
- WebAssembly;
- memoria;
- IL2CPP;
- URP;
- PBR;
- optimizacion grafica;
- frame time;
- draw calls;
- presupuesto geometrico;
- compatibilidad web movil y de escritorio;
- flujo funcional real de la build;
- modos visibles, ocultos y legacy;
- subsistema termico;
- y cualquier formulacion matematica relevante.

Debes verificar que:

- no haya simplificaciones enganiosas;
- no haya sobreventa tecnica;
- no se atribuya al proyecto mas de lo que realmente sostiene la implementacion final;
- la matematica sea fiel, clara y correctamente interpretada;
- el informe sea coherente con la app real documentada, no solo con el ideal de la propuesta.

### 5. Auditoria quirurgica del capitulo 4

Esta es una prioridad especial.

Debes evaluar si el capitulo de desarrollo realmente explica, con suficiente detalle y honestidad:

- la evolucion del proyecto;
- el paso de 16 piezas historicas a la normalizacion 28 / 30 / 257;
- el pipeline CAD a runtime;
- los distintos metodos de importacion y cuando se usaron;
- el uso adaptado del marco MAD-T;
- las piezas rehechas, reconstruidas o modeladas desde cero;
- el sistema modular de tornillos y elementos repetitivos;
- los addons y scripts de automatizacion;
- la logica mobile-first de UX/UI;
- la diferencia entre la UI movil con respaldo teorico fuerte y la version de escritorio como adaptacion funcional;
- la iconografia procedural y las microinteracciones en codigo;
- los shaders, el modo Blueprint y los environments;
- la arquitectura runtime y el saneamiento de activos importados;
- las limitaciones reales y decisiones descartadas o evaluadas.

Senala con precision si el capitulo 4:

- sigue teniendo vacios;
- mezcla teoria con implementacion sin marcar la diferencia;
- documenta como ejecutado algo que solo fue evaluado;
- o deja sin explicar procesos relevantes del proyecto.

### 6. Bibliografia y respaldo

- Verifica si las referencias realmente respaldan lo que el texto afirma.
- Prioriza fuentes primarias y autoritativas:
  - articulos revisados por pares;
  - libros academicos;
  - estandares;
  - documentacion oficial.
- No uses Wikipedia, Reddit, Scribd, blogs promocionales ni comparativas comerciales como soporte principal.
- No inventes citas, DOI, ISBN ni paginacion.
- Si una referencia es debil, desactualizada o inadecuada, propon una mejor.
- Si una afirmacion no puede sostenerse, recomienda atenuarla.

### 7. Tono academico

Detecta y marca cualquier lenguaje:

- promocional;
- sensacionalista;
- causalmente injustificado;
- impreciso;
- grandilocuente;
- o impropio de tesis.

Busca especialmente terminos o construcciones como:

- demostrar;
- validar plenamente;
- robusto;
- optimo;
- superior;
- alta fidelidad;
- mejor;
- innovador;
- reproducible;
- viable;
- claridad espacial;

cuando no esten suficientemente definidos, medidos o justificados.

### 8. Placeholders, tablas y figuras

Evalua especificamente:

- si los placeholders del capitulo 4 y del capitulo 5 estan ubicados donde corresponde;
- si falta algun placeholder clave para entender el desarrollo del proyecto;
- si hay tablas o figuras redundantes;
- si las reservas editoriales son coherentes con los entregables pendientes;
- y si la lista de figuras y tablas mantiene sentido con el estado actual del informe.

## Instrucciones sobre auditorias previas

- Si se adjuntan revisiones previas de Gemini o ChatGPT, usalas solo como insumo auxiliar.
- No las aceptes automaticamente.
- Debes verificar por tu cuenta cada afirmacion critica con fuentes solidas.
- Si alguna observacion previa es incorrecta, desactualizada o exagerada, corrigela.

## Formato de salida obligatorio

### A. Veredicto general

Indica solo una de estas opciones:

- Lista para entrega con estandar alto
- Muy solida pero con ajustes menores
- Solida pero aun requiere ajustes sustantivos
- Aun no esta lista para entrega

Explica el veredicto con precision y sin diplomacia innecesaria.

### B. Hallazgos criticos y altos

Lista primero los problemas mas graves. Para cada hallazgo incluye:

- severidad;
- seccion;
- cita o afirmacion problematica;
- por que es problematica;
- tipo de problema: falsa / exagerada / ambigua / no respaldada / metodologicamente insuficiente / incoherente;
- correccion propuesta;
- fuente exacta que respalda la correccion.

### C. Validacion de lo ya corregido

Construye una tabla con:

- punto corregido;
- estado: bien corregido / corregido parcialmente / aun insuficiente;
- comentario tecnico;
- fuente de validacion.

### D. Matriz forense de afirmaciones debiles

Tabla con columnas:

- seccion;
- afirmacion;
- estado;
- explicacion;
- accion recomendada;
- fuente sugerida.

### E. Matriz de coherencia interna

Evalua explicitamente:

- problema;
- pregunta;
- objetivos;
- metodologia;
- desarrollo;
- resultados;
- conclusiones.

Marca donde hay coherencia fuerte, parcial o debil, y explica por que.

### F. Auditoria especifica del capitulo 4

Entrega una subseccion aparte con:

- lo que el capitulo 4 ya explica bien;
- lo que sigue faltando;
- lo que esta exagerado o insuficientemente respaldado;
- y que visuales o tablas faltan para que el capitulo quede realmente solido.

### G. Auditoria bibliografica

- referencias correctas y bien usadas;
- referencias correctas pero mal aprovechadas;
- referencias debiles o mejorables;
- referencias posiblemente faltantes.

### H. Reescrituras finales listas para pegar

Entrega solo las reescrituras estrictamente necesarias para dejar el informe en su mejor version posible.

## Reglas finales

- No cierres la auditoria hasta revisar todo el documento.
- No des por bueno nada solo porque suena razonable.
- Si algo debe atenuarse, prefiero que lo atenuues antes de que lo sobrevalides.
- Si una parte ya quedo solida, dilo claramente.
- Si detectas un unico punto que todavia impediria una entrega de excelencia, destacalo de manera explicita.
- Tu objetivo es actuar como ultima barrera de control de calidad antes del cierre formal del informe.
