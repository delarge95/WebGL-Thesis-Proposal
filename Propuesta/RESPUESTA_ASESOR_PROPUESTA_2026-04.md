# Respuesta al Tutor sobre la Revision de la Propuesta

Fecha: 2026-04-11

Este documento resume como se atendieron las observaciones formuladas sobre la propuesta del proyecto de grado. El formato utilizado es: observacion -> correccion aplicada -> evidencia -> estado.

---

## 1. Formalizacion metodologica

**Observacion del asesor**  
Se solicito fortalecer la formalizacion metodologica, especificando con mayor claridad el tipo de investigacion.

**Correccion aplicada**  
La metodologia fue reescrita para explicitar que el proyecto corresponde a una investigacion aplicada, con enfoque mixto y predominio cuantitativo, enmarcada en Design Science Research (DSR) y prototipado iterativo.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- secciones nuevas: tipo y enfoque de investigacion, marco DSR, desarrollo iterativo por fases

**Estado**  
Corregido a nivel documental.

---

## 2. Variables y operacionalizacion

**Observacion del asesor**  
Se indico la necesidad de definir y operacionalizar variables de manera explicita.

**Correccion aplicada**  
Se incorporaron las variables dependientes de usabilidad percibida, carga cognitiva y rendimiento tecnico; la variable independiente tipo de visualizacion; y las variables de control relacionadas con experiencia, perfil y dispositivo.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- subsecciones nuevas: variable independiente, variables dependientes, variables de control

**Estado**  
Corregido a nivel documental.

---

## 3. Procedimiento de analisis de datos

**Observacion del asesor**  
Se requirio mayor claridad sobre el procedimiento de analisis de datos derivados de las pruebas de usabilidad y carga cognitiva.

**Correccion aplicada**  
Se documento el uso de SUS y NASA-TLX Raw, su forma de calculo y el esquema de analisis cuantitativo y cualitativo. Tambien se definio el analisis de KPIs tecnicos del build WebGL y la integracion de hallazgos Think-Aloud.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- subseccion nueva: procedimiento de analisis de datos

**Estado**  
Corregido a nivel documental. La obtencion de resultados finales sigue sujeta a la ejecucion de las pruebas con usuarios y al freeze del build.

---

## 4. Coherencia entre problema, objetivos y solucion

**Observacion del asesor**  
La propuesta fue valorada como coherente, pero con espacio para reforzar la relacion entre problema, solucion y validacion.

**Correccion aplicada**  
La metodologia ahora vincula con mayor precision el problema de comprension espacial y lectura tecnica con el uso del visor WebGL, las tareas de prueba y las metricas de validacion.

**Evidencia concreta**  
- `Propuesta/sections/metodologia.tex`
- explicitacion de la unidad de analisis, protocolo de tareas e instrumentos

**Estado**  
Fortalecido.

---

## 5. Ajustes de referencias APA

**Observacion del asesor**  
Se solicitaron correcciones formales en el uso de normas APA dentro de las referencias.

**Correccion aplicada**  
Se normalizaron referencias clave, se corrigio el caso `three.js`, se incorporaron Brooke (1996) y Hart & Staveland (1988), y se separaron referencias repetidas de Unity con sufijos por anio cuando aplica.

**Evidencia concreta**  
- `Propuesta/sections/bibliografia.tex`
- `Propuesta/references.bib`

**Estado**  
Corregido.

---

## 6. Estado real de cierre

Las observaciones sobre estructura metodologica y referencias ya fueron atendidas en la documentacion de propuesta. No obstante, los resultados empiricos finales del proyecto todavia dependen de cuatro actividades de cierre:

1. aplicar las optimizaciones pendientes del pipeline CAD -> Unity;
2. rerun de auditorias de cobertura/jerarquia sobre la escena final;
3. congelar el build definitivo;
4. ejecutar las pruebas de usabilidad, SUS, NASA-TLX Raw y reporte final de hallazgos.

Mientras estas actividades no finalicen, el capitulo de resultados del informe debe mantenerse como pendiente metodologico y no como resultado cerrado.
