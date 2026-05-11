# Auditoria de Claridad de la Propuesta Simplificada

Fecha: 2026-04-23  
Documento auditado: `E:\WebGL_tesis\Propuesta\simplified_proposal\simplified_proposal.tex`  
Objetivo del audit: evaluar si la propuesta simplificada logra ser comprensible para publico no especializado, maximizando alcance y legibilidad sin perder rigor tecnico ni coherencia academica.

## Veredicto General

La propuesta simplificada esta **bien orientada**, pero **todavia no esta realmente simplificada para personas del comun**.

Su principal fortaleza es que conserva la estructura metodologica y tecnica correcta de la propuesta completa. Su principal debilidad es que sigue hablando, en muchas partes, como un documento para jurados o para lectores tecnicos familiarizados con Unity, WebGL, UX research y computacion grafica.

En su estado actual, el documento puede resultar claro para:

- estudiantes de Ingenieria Multimedia;
- docentes del area;
- desarrolladores o artistas tecnicos;
- lectores universitarios con familiaridad basica con tecnologia.

Pero todavia puede resultar dificil para:

- usuarios no tecnicos;
- familiares, jurados externos o lectores generales;
- personas que entienden el problema, pero no el vocabulario tecnico del pipeline.

Conclusión sintetica: **el documento conserva bien el rigor, pero aun no maximiza la accesibilidad.**

## Diagnostico por Criterios

| Criterio | Estado | Comentario |
| --- | --- | --- |
| Problema central comprensible | Bueno | Se entiende que el problema son los limites de la documentacion 2D. |
| Solucion propuesta comprensible | Parcial | Se entiende que es un visor 3D web, pero se explica con demasiado vocabulario tecnico desde el inicio. |
| Lenguaje apto para publico general | Insuficiente | Hay exceso de terminos como WebAssembly, IL2CPP, PBR, draw calls, pipeline, frame time, benchmarking, etc. |
| Conservacion del rigor tecnico | Fuerte | El rigor general no se pierde. |
| Equilibrio entre claridad y rigor | Parcial | El texto privilegia el rigor, pero aun no traduce suficiente para lector no experto. |
| Legibilidad seccion por seccion | Desigual | Planteamiento y justificacion funcionan mejor que objetivos, estado del arte y metodologia. |
| Accesibilidad narrativa | Insuficiente | Falta una capa explicativa mas humana y mas cercana a la experiencia cotidiana del lector. |

## Hallazgos Prioritarios

### 1. El titulo sigue siendo demasiado largo y tecnico

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\simplified_proposal.tex:162`

Problema:

La version simplificada mantiene un titulo correcto academica y tecnicamente, pero no amigable para publico general. Mezcla demasiados bloques de informacion en una sola linea:

- prototipo web 3D interactivo;
- visualizacion tecnica;
- inspeccion;
- analisis visual;
- ensamblaje de hardware complejo;
- pipelines de optimizacion grafica;
- WebAssembly.

Eso vuelve dificil entender de inmediato "de que trata" el proyecto.

Impacto:

El lector comun no entra por la puerta principal del problema, sino por la puerta del vocabulario tecnico.

Recomendacion:

Mantener el titulo academico oficial para entrega, pero introducir desde el resumen o una frase inicial una reformulacion mas humana del tipo:

> En terminos simples, el proyecto busca convertir la documentacion estatica de un dron en una experiencia web 3D interactiva que facilite entender como esta armado y como se relacionan sus piezas.

### 2. El resumen sigue siendo demasiado denso para una version "simplificada"

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\simplified_proposal.tex:195-201`

Problema:

El resumen conserva demasiadas capas tecnicas en poco espacio:

- Unity;
- WebGL;
- WebAssembly;
- teoria de la carga cognitiva;
- DSR;
- SUS;
- NASA-TLX;
- soporte 2D de referencia;
- viabilidad operativa.

Cada una de esas expresiones es valida, pero juntas elevan mucho la carga de lectura.

Impacto:

El lector general entiende que es algo tecnico, pero no necesariamente entiende rapido:

- cual es el problema cotidiano;
- que hace exactamente el prototipo;
- por que seria util;
- y a quien le sirve.

Recomendacion:

El resumen necesita una secuencia mas amigable:

1. problema cotidiano;
2. solucion en una frase simple;
3. caso de uso concreto;
4. como se evaluara;
5. para que sirve el resultado.

### 3. El planteamiento del problema esta bien, pero aun habla mas para evaluadores que para personas del comun

Severidad: Media-Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\planteamiento_problema.tex:3-15`

Fortaleza:

La logica del problema esta clara: los PDF y las imagenes 2D obligan a reconstruir mentalmente relaciones espaciales.

Debilidad:

Expresiones como:

- "hardware tecnico complejo";
- "proceso de optimizacion articulado";
- "criterios verificables";
- "contexto academico";

siguen siendo correctas, pero demasiado institucionales para el lector comun.

Recomendacion:

Antes del parrafo tecnico conviene introducir una escena concreta y facilmente imaginable, por ejemplo:

> Cuando una persona intenta identificar una pieza en un manual 2D, suele tener que comparar varias imagenes y deducir por si sola donde va cada componente.

Eso reduciria mucho la distancia entre el problema real y la formulacion academica.

### 4. La pregunta de investigacion sigue siendo demasiado larga para una propuesta simplificada

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\planteamiento_problema.tex:19`

Problema:

La pregunta mezcla demasiadas capas:

- desempeno en tareas;
- usabilidad percibida;
- carga de trabajo;
- usuarios tecnico-academicos;
- exploracion e identificacion;
- dron Holybro X500 V2;
- comparacion 3D vs 2D;
- viabilidad tecnica en navegador.

Eso la hace metodologicamente valida, pero poco digerible.

Recomendacion:

Mantener la pregunta formal en el documento, pero agregar justo debajo una "pregunta en lenguaje simple", por ejemplo:

> En palabras sencillas: se quiere saber si un visor 3D web ayuda a entender y explorar mejor el dron que un manual 2D tradicional, y si esa solucion puede funcionar bien en un navegador.

### 5. Los objetivos especificos no estan simplificados; solo estan resumidos

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\objetivos.tex:8-14`

Problema:

Los objetivos siguen escritos con logica de jurado tecnico:

- pipeline de optimizacion;
- presupuesto geometrico;
- WebGL;
- retopologia;
- baking;
- URP;
- frame time;
- C# compilado a WebAssembly;
- NASA-TLX Raw.

Eso no es simplificacion real; es compresion de contenido tecnico.

Recomendacion:

Para una propuesta simplificada, cada objetivo deberia poder leerse en dos niveles:

- una version corta y comprensible;
- una version tecnica subordinada o aclaratoria.

Ejemplo de estructura recomendada:

- Objetivo claro: preparar los modelos para que funcionen bien en la web.
- Aclaracion tecnica: esto implica reducir geometria, reorganizar mallas y hornear texturas.

### 6. El estado del arte es correcto, pero demasiado especializado para su funcion comunicativa

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\estado_del_arte.tex:7-48`

Problema:

Para un lector comun, esta seccion se siente como una comparativa entre motores y plataformas mas que como una explicacion de por que se eligio una herramienta adecuada para el proyecto.

Ademas, la tabla comparativa exige entender de antemano:

- huella inicial;
- control del pipeline;
- PBR;
- infraestructura externa;
- pertinencia.

Recomendacion:

La seccion deberia empezar con una conclusion humana antes de la tabla:

> Se evaluaron varias tecnologias. Algunas son mas ligeras, otras mas faciles de publicar y otras mas potentes visualmente. Unity se eligio porque ofrece el mejor equilibrio entre herramientas integradas, control del proyecto y capacidad para construir una experiencia interactiva completa.

Y la tabla deberia traducir o suavizar algunas columnas. Por ejemplo:

- `Huella inicial` -> `Peso aproximado al cargar`
- `Control del pipeline` -> `Que tanto control permite sobre el proyecto`
- `Infraestructura externa` -> `Si necesita servidores o servicios adicionales`

### 7. La justificacion funciona bien para publico universitario, pero aun no para publico amplio

Severidad: Media

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\justificacion.tex:3-26`

Fortaleza:

Explica de forma bastante ordenada por que se escoge Unity y por que el proyecto es pertinente para Ingenieria Multimedia.

Debilidad:

La pertinencia sigue expresada sobre todo desde el lenguaje disciplinar. Falta una frase mas social y mas tangible que responda:

- por que esto le importa a alguien fuera de la carrera;
- que problema de comunicacion tecnica ayuda a resolver;
- por que vale la pena convertir un manual en un visor.

### 8. El glosario ayuda, pero aun esta demasiado cargado para el lector no tecnico

Severidad: Media-Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\marco_conceptual.tex:3-31`

Fortaleza:

La idea de incluir un glosario es acertada.

Debilidad:

El glosario actual prioriza muchos terminos de computacion grafica avanzada:

- BRDF;
- GPU Instancing;
- IL2CPP;
- LOD;
- PBR.

Pero no prioriza tanto terminos mas cercanos al lector general, como:

- visor 3D;
- pieza;
- ensamblaje;
- inspeccion tecnica;
- modelo CAD;
- documentacion 2D.

Recomendacion:

La version simplificada deberia usar un glosario en dos capas:

- primero, conceptos esenciales para entender el proyecto;
- despues, conceptos tecnicos opcionales para quien quiera profundizar.

### 9. La metodologia esta bien explicada para academia, pero todavia no es amigable para lector general

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\metodologia.tex:3-87`

Problema:

Aunque esta mejor simplificada que la metodologia completa, sigue exigiendo demasiada familiaridad con investigacion aplicada y UX research.

Especialmente dificiles para publico general:

- enfoque mixto;
- cualitativo-formativo;
- descriptivo y exploratorio;
- DSR;
- contrabalanceado AB o BA;
- variables independiente y dependiente;
- inferencia poblacional;
- KPIs.

Recomendacion:

La metodologia deberia incluir una explicacion previa del tipo:

> En pocas palabras, el proyecto se va a construir, probar y observar con usuarios para ver si realmente ayuda frente a una alternativa 2D.

Luego si puede entrar a la descripcion mas formal.

### 10. La tabla de variables no esta traducida para lector no especializado

Severidad: Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\metodologia.tex:20-37`

Problema:

La tabla es correcta, pero exige entender conceptos metodologicos como:

- variable independiente;
- variable dependiente;
- criterio de interpretacion.

Recomendacion:

Para una version simplificada, convendria renombrar columnas a algo como:

- `Que se compara`
- `Que se medira`
- `Con que se medira`
- `Como se interpretara`

### 11. La seccion de instrumentos explica bien, pero usa demasiados nombres sin traducirlos del todo

Severidad: Media-Alta

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\metodologia.tex:58-66`

Problema:

SUS, NASA-TLX, Think-Aloud y Unity Profiler se explican de forma correcta, pero todavia desde un tono de lector metodologico.

Recomendacion:

Cada instrumento deberia empezar con una frase funcional simple:

- SUS: una encuesta corta para saber si el visor se siente facil o dificil de usar.
- NASA-TLX: una encuesta para estimar cuanto esfuerzo le exige la tarea al usuario.
- Think-Aloud: pedirle al usuario que diga en voz alta lo que piensa mientras usa el sistema.

### 12. Los resultados esperados estan claros, pero todavia no son cercanos al lenguaje ciudadano

Severidad: Media

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\resultados.tex:11-36`

Problema:

La tabla esta bien estructurada, pero sus beneficiarios y resultados siguen formulados con tono tecnico-institucional.

Ejemplo:

- `Sistema de shaders URP optimizados`
- `Informe de evaluacion de usabilidad y carga de trabajo percibida`

Recomendacion:

Podria mantenerse el nombre tecnico, pero acompañado de una expresion mas humana:

- sistema visual del prototipo;
- informe sobre facilidad de uso y esfuerzo percibido;
- modelos 3D adaptados para funcionar en web.

### 13. Hay al menos un detalle de redaccion/fuente que debe corregirse

Severidad: Media

Ubicacion:

- `E:\WebGL_tesis\Propuesta\simplified_proposal\sections\metodologia.tex:56`

Hallazgo:

Aparece `sesi\\'on` en lugar de `sesion`, lo que sugiere un problema de escape o tipeo en LaTeX.

Impacto:

Es pequeno, pero resta pulcritud en un documento que pretende acercarse a lectores no tecnicos.

## Fortalezas Reales de la Version Simplificada

- Mantiene coherencia con la propuesta final rigurosa.
- No cae en sobrepromesas ni sensacionalismo.
- Conserva buena estructura academica.
- El problema central se entiende razonablemente bien.
- El glosario es una buena decision editorial.
- La metodologia, aunque densa, esta mucho mejor ordenada que en una propuesta tecnica promedio.

## Debilidades Estructurales

- Simplifica por reduccion de volumen, no por traduccion pedagogica suficiente.
- Sigue priorizando vocabulario de especialidad.
- Todavia no diferencia bien entre:
  - lo que necesita el jurado;
  - y lo que necesita un lector general para entender el valor del proyecto.
- No aprovecha suficientes analogias ni ejemplos concretos.
- Tiene mas "precision" que "puertas de entrada" para un lector nuevo.

## Recomendaciones de Reescritura

### Principio 1. Explicar primero el problema vivido y luego el aparato tecnico

La secuencia ideal para lector general es:

1. que problema real existe;
2. por que hoy es dificil;
3. que propone el proyecto;
4. como se evaluara;
5. por que importa.

### Principio 2. Mantener nombres tecnicos, pero traducirlos inmediatamente

Ejemplo:

> NASA-TLX, una herramienta que permite estimar cuanto esfuerzo percibe un usuario al realizar una tarea.

### Principio 3. Reducir densidad de siglas por parrafo

En la propuesta simplificada deberia evitarse acumular en un mismo bloque demasiados terminos como:

- Unity;
- WebGL;
- WebAssembly;
- PBR;
- DSR;
- SUS;
- NASA-TLX.

### Principio 4. Distinguir entre "version para academia" y "version para comprension amplia"

La propuesta simplificada no deberia ser solo la version corta de la formal. Deberia ser una version con otra estrategia de comunicacion:

- mas contextual;
- mas explicativa;
- menos condensada;
- mas cercana al lector.

## Prioridades de Correccion

### Prioridad alta

- resumir el titulo con una frase puente mas humana;
- reescribir el resumen para publico general;
- traducir la pregunta de investigacion a lenguaje simple;
- reescribir objetivos especificos en doble capa: clara + tecnica;
- simplificar estado del arte y su tabla;
- simplificar metodologia antes de la formulacion academica.

### Prioridad media

- reorganizar el glosario en esenciales y avanzados;
- humanizar justificacion y resultados esperados;
- suavizar nombres de tablas e instrumentos en la version simplificada.

### Prioridad baja

- pulir pequenos detalles de tipeo y consistencia editorial;
- revisar si todas las tablas aportan a la comprension o si alguna deberia pasar a anexo.

## Veredicto Final

La propuesta simplificada **si conserva el rigor**, pero **todavia no esta optimizada para llegar a la mayor cantidad de personas**.

Hoy funciona como:

- una propuesta tecnica resumida;

mas que como:

- una propuesta verdaderamente simplificada y pedagogica.

Si el objetivo es que la entienda bien una persona no especialista sin perder seriedad academica, el siguiente paso no es recortar mas contenido, sino **traducir mejor la complejidad**.
