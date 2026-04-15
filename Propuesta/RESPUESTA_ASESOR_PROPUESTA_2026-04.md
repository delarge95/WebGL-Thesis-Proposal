# Respuesta al Tutor sobre la Revision de la Propuesta

Fecha: 2026-04-14

Este documento resume como se atendieron las observaciones formuladas sobre la propuesta del proyecto de grado. El formato utilizado es: observacion -> correccion aplicada -> evidencia -> estado.

---

## 1. Formalizacion metodologica

**Observacion del asesor**  
Se solicito fortalecer la formalizacion metodologica, especificando con mayor claridad el tipo de investigacion.

**Correccion aplicada**  
La metodologia fue reescrita para explicitar que el proyecto corresponde a una investigacion aplicada, con enfoque mixto y predominio cualitativo-formativo, enmarcada en *Design Science Research*. Tambien se distinguio de forma explicita entre el proceso DSRM de Peffers et al. (2007) y las directrices de rigor y relevancia de Hevner et al. (2004).

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- secciones nuevas o reforzadas: tipo y enfoque de investigacion, marco DSR, desarrollo iterativo por fases, muestra, diseno comparativo, instrumentos y procedimiento de analisis

**Estado**  
Corregido a nivel documental.

---

## 2. Variables y operacionalizacion

**Observacion del asesor**  
Se indico la necesidad de definir y operacionalizar variables de manera explicita.

**Correccion aplicada**  
Se incorporaron la variable independiente de tipo de visualizacion, las variables dependientes de usabilidad percibida, carga de trabajo percibida y rendimiento tecnico, y las variables de control relacionadas con experiencia, perfil, entorno de prueba, navegador, cache y tipo de build.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- subsecciones: variable independiente, variables dependientes, variables de control

**Estado**  
Corregido a nivel documental.

---

## 3. Procedimiento de analisis de datos

**Observacion del asesor**  
Se requirio mayor claridad sobre el procedimiento de analisis de datos derivados de las pruebas de usabilidad y carga de trabajo percibida.

**Correccion aplicada**  
Se documento el uso de SUS y NASA-TLX Raw, su forma de calculo, su criterio de interpretacion y el esquema de analisis cuantitativo y cualitativo. Tambien se definio el analisis de KPIs tecnicos del build Web, el diseno intra-sujeto 3D vs. 2D con contrabalanceo y la triangulacion de hallazgos con *Think-Aloud*.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- subsecciones: diseno comparativo y condiciones de prueba, instrumentos de recoleccion, procedimiento de analisis de datos, triangulacion de hallazgos

**Estado**  
Corregido a nivel documental. La obtencion de resultados finales sigue sujeta a la ejecucion de las pruebas con usuarios y al cierre del build.

---

## 4. Coherencia entre problema, objetivos y solucion

**Observacion del asesor**  
La propuesta fue valorada como coherente, pero con espacio para reforzar la relacion entre problema, solucion y validacion.

**Correccion aplicada**  
La propuesta ahora vincula con mayor precision el problema de comprension espacial y lectura tecnica con el uso del visor Web 3D, la pregunta de investigacion, las tareas de prueba, los criterios de mejora operativa y las metricas de validacion.

**Evidencia concreta**  
- `Propuesta/sections/planteamiento_problema.tex`
- `Propuesta/sections/objetivos.tex`
- `Propuesta/sections/metodologia.tex`
- explicitacion de la pregunta de investigacion, la unidad de analisis, el protocolo de tareas y la convergencia entre desempeno en tareas, workload percibido, usabilidad y KPIs tecnicos

**Estado**  
Fortalecido.

---

## 5. Ajustes de referencias APA

**Observacion del asesor**  
Se solicitaron correcciones formales en el uso de normas APA dentro de las referencias.

**Correccion aplicada**  
Se normalizaron referencias clave, se corrigio el caso `three.js`, se incorporaron Brooke (1996), Hart y Staveland (1988), Kajiya (1986), la referencia oficial actual de Unity 6 para compatibilidad web y la especificacion oficial de WebAssembly. Tambien se corrigieron errores factuales de DOI y paginacion identificados en auditorias externas.

**Evidencia concreta**  
- `Propuesta/sections/bibliografia.tex`
- `Propuesta/references.bib`

**Estado**  
Corregido.

---

## 6. Precision terminologica y alcance real del proyecto

**Observacion del asesor**  
Aunque la coherencia general era favorable, se requeria mayor precision para evitar ambiguedades metodologicas y tecnicas.

**Correccion aplicada**  
Se sustituyo la referencia generica a `hardware de alto rendimiento` por una definicion operativa de `hardware complejo`, se corrigio el titulo para eliminar la ambiguedad de `analisis estructural`, y se distinguio entre teoria de la carga cognitiva como marco interpretativo y carga de trabajo percibida como variable medida con NASA-TLX.

**Evidencia concreta**  
- `Propuesta/final_proposal.tex`
- `Propuesta/sections/planteamiento_problema.tex`
- `Propuesta/sections/marco_teorico.tex`
- `Propuesta/sections/marco_conceptual.tex`
- `Propuesta/sections/metodologia.tex`

**Estado**  
Corregido a nivel conceptual y terminologico.

---

## 7. Estado real de cierre

Las observaciones sobre estructura metodologica, coherencia conceptual y referencias ya fueron atendidas en la documentacion de propuesta. No obstante, los resultados empiricos finales del proyecto todavia dependen de cuatro actividades de cierre:

1. aplicar las optimizaciones pendientes del pipeline CAD -> Unity;
2. rerun de auditorias de cobertura/jerarquia sobre la escena final;
3. congelar el build definitivo;
4. ejecutar las pruebas de usabilidad, SUS, NASA-TLX Raw y reporte final de hallazgos.

Mientras estas actividades no finalicen, el capitulo de resultados del informe debe mantenerse como pendiente metodologico y no como resultado cerrado.
