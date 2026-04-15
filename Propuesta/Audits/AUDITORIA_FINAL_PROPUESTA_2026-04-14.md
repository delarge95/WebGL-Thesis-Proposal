# Auditoria Final de la Propuesta

Fecha: 2026-04-14  
Documento auditado: `E:\WebGL_tesis\Propuesta\final_proposal.tex`  
PDF compilado: `E:\WebGL_tesis\Propuesta\final_proposal.pdf`

## 1. Alcance de esta auditoria

Esta auditoria consolida y verifica la integracion de:

- la auditoria academica de Gemini Deep Research;
- la auditoria academica de ChatGPT Deep Research;
- la auditoria de normas APA 7 / UNAD de ChatGPT Deep Research;
- la verificacion tecnica final sobre Unity Web, WebAssembly, PBR y consistencia metodologica;
- la recompilacion de la propuesta despues de la ultima ronda de correcciones.

El objetivo fue dejar la propuesta lista para entrega, con trazabilidad explicita de:

- hallazgos aceptados;
- hallazgos rechazados por desactualizados o conceptualmente incorrectos;
- correcciones efectivamente integradas en el documento activo;
- y estado final de compilacion, formato y coherencia interna.

## 2. Fuentes de auditoria utilizadas

### 2.1 Auditorias externas

- `D:\Downloads\Auditoría Rigurosa de Propuesta Académica.docx`
- `D:\Downloads\deep-research-report.md`
- `D:\Downloads\deep-research-report (2).md`

### 2.2 Material normativo y de formato

- `E:\WebGL_tesis\External_docs\Norma_APA_7_Edicion (1).pdf`
- `E:\WebGL_tesis\External_docs\Plantilla_Normas_APA_7a_Edicion.pdf`
- `E:\WebGL_tesis\External_docs\FORMATO_DE_PRESENTACIÓN_PROPUESTA_PROYECTO_APLICADO_COMO_ALTERNATIVA_DE_TRABAJO_DE_GRADO (4).pdf`

### 2.3 Fuentes tecnicas y metodologicas incorporadas o verificadas

- Hevner et al. (2004)
- Peffers et al. (2007)
- Brooke (1996)
- Bangor et al. (2008, 2009)
- Hart y Staveland (1988)
- Hart (2006)
- Ericsson y Simon (1993)
- Lazar et al. (2017)
- Kajiya (1986)
- Unity Technologies. `Memory in WebGL`
- Unity Technologies. `Getting started with WebGL development`
- Unity Technologies. `Universal Render Pipeline overview`
- Unity Technologies. `System requirements for Unity 6`
- WebAssembly Community Group. `WebAssembly core specification`

## 3. Hallazgos del audit que fueron aceptados e integrados

### 3.1 Alcance del proyecto y precision semantica

Se acepto e integro la necesidad de corregir:

- la ambiguedad de `analisis estructural` en el titulo;
- el uso promocional o impreciso de `hardware de alto rendimiento`;
- la necesidad de definir operativamente el caso de estudio como `hardware tecnico complejo` o `hardware complejo`.

Resultado integrado:

- el titulo ahora habla de `visualizacion tecnica, inspeccion y analisis visual del ensamblaje`;
- el problema y el marco conceptual definen `hardware complejo` en terminos operativos;
- se elimino la deriva hacia una interpretacion tipo FEA o analisis mecanico-estructural.

### 3.2 DSR y coherencia metodologica

Se mantuvo y reforzo la correccion previa que ya separaba:

- `Peffers et al. (2007)` como proceso DSRM;
- `Hevner et al. (2004)` como directrices de rigor y relevancia.

Tambien se verifico que la propuesta:

- formula una pregunta de investigacion explicita;
- opera con un diseno intra-sujeto 3D vs. 2D;
- usa contrabalanceo AB/BA;
- y define criterios de mejora operativa mediante convergencia de desempeno en tareas, usabilidad, workload percibido y evidencia cualitativa.

### 3.3 SUS, NASA-TLX y Think-Aloud

Se aceptaron e integraron las siguientes correcciones:

- SUS se mantiene como instrumento para el sistema interactivo principal, no como falsa comparacion simetrica 2D/3D;
- el benchmark de `68` se trata como promedio historico aproximado y no como sinonimo automatico de alta usabilidad;
- se suavizo la lectura de `72` para que funcione como referencia contextual favorable y no como umbral universal rigido;
- NASA-TLX se formula como `Raw TLX` y se interpreta como carga de trabajo percibida;
- se deja explicito que NASA-TLX no mide de forma directa las categorias intrinseca, extrinseca y germana de la Teoria de la Carga Cognitiva;
- Think-Aloud queda con instruccion estandarizada, intervencion neutral, trazabilidad observacional y apoyo de audio/captura cuando el entorno lo permita.

### 3.4 Precision tecnica

Se aceptaron e integraron las siguientes correcciones:

- atribucion correcta de la `rendering equation` a `Kajiya (1986)`;
- mantenimiento de `Cook y Torrance (1982)` para BRDF/microfacetas, no para la ecuacion integral;
- precision en la cadena tecnica:
  - IL2CPP transforma IL a C++;
  - el target Web compila despues a WebAssembly mediante el toolchain del build;
- precision sobre memoria:
  - se elimina cualquier limite rigido ficticio;
  - la explicacion se centra en memoria lineal, huella inicial de activos, fragmentacion y necesidad de bloques contiguos;
- inclusion de referencia oficial actual de Unity 6 sobre compatibilidad Web.

## 4. Hallazgos del audit que fueron rechazados o reformulados

### 4.1 Rechazo explicito de la afirmacion "Unity Web no soporta movil"

Este hallazgo fue rechazado como tesis general vigente para la propuesta final.

Motivo:

- corresponde a documentacion historica de Unity WebGL y no al estado actual de la plataforma Web de Unity 6;
- la documentacion oficial actual de Unity 6 incluye navegadores moviles compatibles bajo condiciones especificas;
- por tanto, la propuesta no puede afirmarse correctamente como `desktop-only` si el producto y el diseno contemplan uso web movil.

Decision adoptada:

- la propuesta se formula como aplicacion Web para navegadores compatibles de escritorio y moviles;
- no se promete compatibilidad universal;
- la compatibilidad movil se declara como `compatibilidad esperada`, no como validacion exhaustiva por matriz cerrada de dispositivos;
- la matriz empirica exacta queda reservada para la fase de evaluacion y el informe final.

Evidencia oficial utilizada:

- `Unity Technologies. (s. f.). System requirements for Unity 6.`

## 5. Correcciones efectivamente integradas por archivo

### 5.1 `final_proposal.tex`

- titulo corregido;
- resumen y abstract alineados con `hardware complejo`, `analisis visual del ensamblaje` y `carga de trabajo percibida`;
- palabras clave y descriptores actualizados;
- nombre del asesor verificado como `Gustavo Enrique Vejarano Matiz`.

### 5.2 `sections/planteamiento_problema.tex`

- definicion operativa de `hardware complejo`;
- eliminacion de `hardware de alto rendimiento`;
- precision sobre memoria Web y WebAssembly;
- suavizacion de afirmaciones cuantitativas no respaldadas de forma dura;
- pregunta de investigacion alineada con workload percibido, no con TLX como medida directa de carga cognitiva.

### 5.3 `sections/objetivos.tex`

- objetivo general alineado con `inspeccion y analisis visual del ensamblaje`;
- objetivo especifico final corregido a `carga de trabajo percibida`.

### 5.4 `sections/marco_teorico.tex`

- `Hipotesis de trabajo` sustituida por `Proposicion de trabajo`;
- Kajiya incorporado como referencia correcta de la rendering equation;
- lenguaje afinado para evitar sobrepromesas causales;
- ajustes de terminologia para hablar de comprension del ensamblaje y no de un supuesto analisis estructural mecanico.

### 5.5 `sections/marco_conceptual.tex`

- definicion de `hardware complejo`;
- definicion mas precisa de IL2CPP y su relacion con el target Web.

### 5.6 `sections/justificacion.tex`

- ajuste de IL2CPP / WebAssembly con mayor rigor tecnico;
- incorporacion de la posicion oficial actual sobre compatibilidad Web de Unity;
- sustitucion de `carga cognitiva` por `carga de trabajo percibida` donde se alude a TLX;
- reduccion de lenguaje inflado como `replicable` cuando no estaba operacionalizado.

### 5.7 `sections/estado_del_arte.tex`

- sustitucion de `hardware de alto rendimiento` por `hardware complejo`;
- actualizacion del alcance de Unity Web con navegadores compatibles de escritorio y moviles;
- mantenimiento de la comparativa como benchmarking cualitativo, no como benchmark experimental uniforme.

### 5.8 `sections/metodologia.tex`

- variables dependientes corregidas;
- SUS explicado con formula completa y benchmark prudente;
- NASA-TLX formulado como workload subjetivo;
- explicacion metodologica de Raw TLX, subescalas y orientacion de rendimiento;
- explicacion de compatibilidad Web esperada para escritorio y movil;
- inclusion de `Development Build` y `Autoconnect Profiler` en el plan de medicion tecnica;
- explicacion reforzada del protocolo Think-Aloud y de la triangulacion convergente;
- KPI de carga reformulado en terminos de primera interaccion bajo entorno controlado.

### 5.9 `sections/cronograma.tex`

- tabla ajustada a `tabularx` para eliminar el ultimo residuo de maquetacion del log.

### 5.10 `sections/resultados.tex`

- producto de evaluacion renombrado a `usabilidad y carga de trabajo percibida`;
- ajuste de terminologia de shaders y nota metodologica.

### 5.11 `sections/bibliografia.tex` y `references.bib`

- DOI de Bartlett corregido;
- DOI y venue de Fransson corregidos;
- paginas de Yu corregidas;
- Kajiya incorporado;
- referencia oficial actual de Unity 6 incorporada;
- especificacion oficial de WebAssembly incorporada.

## 6. Verificacion de formato APA 7 / UNAD

La propuesta fue revisada contra la plantilla y el instructivo disponibles en `External_docs`.

### 6.1 Aspectos verificados

- portada institucional y datos generales;
- fuente y maquetacion base de la plantilla activa;
- margenes de 2.54 cm;
- interlineado doble;
- numeracion de pagina en esquina superior derecha;
- tabla de contenido y lista de tablas;
- captions de tablas en formato consistente;
- bibliografia con sangria francesa manual;
- consistencia general con el formato UNAD ya adoptado en la plantilla LaTeX del proyecto.

### 6.2 Resultado

No se detectaron fallas activas de formato que impidan entrega en la propuesta compilada.

## 7. Verificaciones automaticas finales

### 7.1 Estado de compilacion

- `UndefinedRefs = 0`
- `LaTeXWarnings = 0`
- `HyperrefWarnings = 0`
- `Overfull = 0`
- `Underfull = 0`

### 7.2 Busquedas negativas sobre el fuente activo

En los archivos activos compilados por `final_proposal.tex` no quedaron rastros residuales de:

- `analisis estructural`
- `hardware de alto rendimiento`
- `Hipotesis de trabajo`
- `mobile not supported`

La expresion `carga cognitiva` solo permanece en contextos teoricos legitimos:

- Teoria de la Carga Cognitiva como marco conceptual;
- aclaraciones explicitas de que NASA-TLX no mide directamente CLT.

## 8. Observaciones finales

- El mensaje de MiKTeX sobre actualizaciones pendientes del entorno no corresponde a una falla del documento.
- La propuesta queda compilada y alineada con las ultimas decisiones metodologicas, conceptuales, tecnicas y bibliograficas.
- Con esta ronda, los hallazgos sustantivos de Gemini y ChatGPT Deep Research quedaron incorporados en el documento activo, salvo el punto desactualizado sobre supuesto no soporte movil, que fue rechazado con evidencia oficial actual.

## 9. Estado final

La propuesta queda en estado listo para una ultima auditoria externa de alta exigencia o para entrega, con:

- coherencia interna fuerte entre problema, pregunta, objetivos, metodologia y productos esperados;
- bibliografia corregida y actualizada;
- precision tecnica y matematica reforzada;
- terminologia metodologica consistente;
- y compilacion limpia del PDF final.
