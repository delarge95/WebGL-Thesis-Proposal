---
tipo: modulo_estudio
fuente: Informe_final/chapters/03_marco_metodologico.tex
estado: activo
capitulo: 3
area: informe-final
tags:
  - tesis
  - metodologia
  - dsr
  - sus
  - nasa-tlx
---

# INF EST 03 Marco Metodologico

## Idea central

La metodologia explica como se construye y evalua el proyecto. No es un experimento puro para demostrar causalidad fuerte. Es una investigacion aplicada con enfoque mixto y predominio cualitativo-formativo.

Eso significa que el objetivo principal es construir un artefacto, probarlo, observar como se usa, medir indicadores descriptivos y aprender de esa evidencia.

## DSR en palabras claras

Design Science Research es apropiado cuando la investigacion produce un artefacto: una herramienta, sistema, metodo o prototipo que responde a un problema real.

En este proyecto:

- el problema es la dificultad de comprender hardware complejo desde documentos 2D;
- el artefacto es el visor web 3D;
- la evaluacion revisa si ese artefacto funciona, bajo que condiciones y con que limitaciones.

### Peffers vs Hevner

Esta distincion es importante para defensa:

- Peffers et al. (2007) aportan el proceso DSRM: identificar problema, definir objetivos, disenar, demostrar, evaluar y comunicar.
- Hevner et al. (2004) aportan directrices de rigor y relevancia para juzgar la investigacion.

Peffers organiza el camino. Hevner ayuda a evaluar si el camino fue riguroso.

## Muestra y participantes

La muestra ideal es de 30 o mas participantes para fortalecer la lectura descriptiva. El escenario minimo operativo es de 8 a 12 participantes, defendible para evaluacion formativa en HCI y UX.

La clave es no sobregeneralizar:

- con 8 a 12 se pueden detectar problemas de interaccion y obtener lectura cualitativa;
- con 30 se fortalece la descripcion cuantitativa;
- en ambos casos, si el muestreo es por conveniencia, no se puede afirmar inferencia poblacional fuerte.

## Variables

### Variable independiente

Tipo de visualizacion:

- visor 3D interactivo;
- soporte 2D de referencia.

Es lo que cambia entre condiciones.

### Variables dependientes

Son lo que se observa o mide:

- desempeno en tareas;
- carga de trabajo percibida;
- usabilidad percibida del prototipo 3D;
- rendimiento tecnico.

### Variables de control

Son factores que se deben registrar para interpretar resultados:

- equipo;
- navegador;
- resolucion;
- experiencia previa;
- orden de exposicion;
- estado de cache;
- tipo de build.

## Condicion 2D

La condicion 2D no debe improvisarse. Debe ser un paquete documental controlado basado en manuales Holybro y una lamina de componentes.

La razon es metodologica: si el 2D esta mal construido, la comparacion seria injusta. El soporte 2D debe contener informacion suficiente para realizar tareas equivalentes, pero sin interactividad 3D.

## SUS

SUS mide usabilidad percibida del prototipo 3D. No se usa para comparar 3D contra 2D.

Interpretacion:

- 68 es referencia historica aproximada, no "buena usabilidad";
- 70-72 puede leerse como favorable;
- puntajes en los 80 indican aceptacion claramente superior;
- la interpretacion se mantiene en plano descriptivo.

## NASA-TLX Raw

NASA-TLX Raw mide carga de trabajo percibida. Se aplica por condicion para comparar esfuerzo subjetivo entre 3D y 2D.

Formula:

```text
NASA-TLX Raw = promedio de las seis dimensiones
```

Dimensiones:

- demanda mental;
- demanda fisica;
- demanda temporal;
- rendimiento;
- esfuerzo;
- frustracion.

En el informe, rendimiento se orienta de forma invertida para mantener coherencia direccional: 0 significa desempeno perfecto y 100 desempeno deficiente.

## Think-Aloud

Think-Aloud consiste en pedir al participante que verbalice lo que piensa mientras realiza tareas. Sirve para detectar fricciones que los numeros no explican.

Ejemplo:

- Si alguien tarda mucho, NASA-TLX puede mostrar workload alto.
- Think-Aloud puede explicar por que: no vio un boton, no entendio el icono, se perdio al orbitar o confundio una pieza.

## Triangulacion

Triangular significa cruzar evidencias:

- si el usuario tarda mas;
- reporta mas workload;
- verbaliza confusion;
- y comete errores;

entonces el hallazgo es mas fuerte que si solo aparece en una fuente.

## Explicaciones complejas dentro de la seccion

### Formula SUS

```text
SUS = (suma_ajustada_items_impares + suma_ajustada_items_pares) x 2.5
```

Los items impares se ajustan restando 1 a la respuesta. Los pares se ajustan restando la respuesta a 5. Luego se multiplica por 2.5 para llevar el resultado a escala 0-100.

La idea sencilla: SUS convierte respuestas de 1 a 5 en una lectura global de usabilidad percibida.

Ejemplo aplicado al visor:

```text
Participante A:
items impares ajustados = 18
items pares ajustados = 13
suma_ajustada = 31
SUS = 31 x 2.5 = 77.5
```

Como defenderlo:

- si el usuario marca alto en items positivos, el puntaje sube;
- si marca alto en items negativos, el ajuste lo penaliza;
- el resultado no es porcentaje de "perfeccion", sino escala estandarizada de usabilidad percibida;
- en esta tesis SUS evalua el visor 3D, no el soporte 2D.

Lectura para el proyecto:

Un SUS alto puede indicar que los controles, la ficha inferior, los modos y la seleccion fueron faciles de entender. Pero si Think-Aloud muestra confusion en iconos concretos, no se debe ocultar esa friccion: se reporta como mejora localizada dentro de una aceptacion general favorable.

### Formula NASA-TLX Raw

```text
NASA-TLX Raw = (D1 + D2 + D3 + D4 + D5 + D6) / 6
```

Cada `D` es una dimension. En esta tesis se usa Raw TLX, es decir, promedio simple. No se usa ponderacion pareada.

Version con nombres de dimensiones:

```text
NASA_TLX_Raw = (DM + DF + DT + RD + ES + FR) / 6
```

Variables:

- `DM`: demanda mental. Sube si el usuario debe recordar, comparar, inferir o reconstruir demasiada informacion.
- `DF`: demanda fisica. En web suele ser baja, pero puede subir si los gestos tactiles o de mouse son incomodos.
- `DT`: demanda temporal. Sube si el usuario siente presion o lentitud para completar tareas.
- `RD`: rendimiento autopercibido. En esta tesis se orienta para que valores altos indiquen peor percepcion de desempeno.
- `ES`: esfuerzo. Resume cuanto trabajo sintio que tuvo que invertir.
- `FR`: frustracion. Sube si hubo errores, bloqueo, confusion o perdida de control.

Ejemplo comparativo:

```text
3D = (30 + 10 + 20 + 25 + 30 + 15) / 6 = 21.67
2D = (55 + 5 + 35 + 45 + 50 + 30) / 6 = 36.67
diferencia = 2D - 3D = 15.00 puntos
```

Lectura:

En ese ejemplo, el visor 3D tendria menor workload percibido. Pero la defensa debe mirar dimensiones: si `DF` sube en 3D por gestos, puede ser una senal de que la visualizacion ayuda mentalmente pero exige mas interaccion motora.

### Contrabalanceo AB/BA

Si todos usan primero 2D y luego 3D, el segundo medio podria verse beneficiado por aprendizaje. El contrabalanceo alterna ordenes:

- Grupo 1: 3D -> 2D.
- Grupo 2: 2D -> 3D.

Asi se reduce el sesgo de orden.

### Diferencia descriptiva entre condiciones

Una forma clara de presentar comparacion 3D/2D es:

```text
diferencia = media_2D - media_3D
```

Interpretacion:

- si la diferencia en NASA-TLX es positiva, 3D tuvo menor workload que 2D;
- si la diferencia en tiempo es positiva, 3D fue mas rapido que 2D;
- si la diferencia es pequena, se interpreta como resultado similar, no como victoria automatica;
- si hay variabilidad alta, se reporta el rango o desviacion y se explica con Think-Aloud.

Ejemplo:

```text
tiempo_promedio_2D = 180 s
tiempo_promedio_3D = 135 s
diferencia = 45 s
```

Eso no "demuestra" causalidad universal. Indica que, en la muestra y protocolo evaluados, el visor permitio completar esa tarea con menor tiempo promedio.

### Inferencia poblacional

Inferencia poblacional significa afirmar que los resultados representan a una poblacion amplia. El informe evita eso porque la muestra es por conveniencia. En cambio, reporta resultados descriptivos y formativos.

## Terminos importantes de la seccion

- Investigacion aplicada: investigacion orientada a resolver un problema practico.
- Enfoque mixto: uso combinado de datos cualitativos y cuantitativos.
- Formativo: orientado a detectar problemas y mejorar el sistema.
- DSR: investigacion mediante diseno y evaluacion de artefactos.
- Variable independiente: condicion que cambia.
- Variable dependiente: resultado que se mide.
- Variable de control: factor que se registra o mantiene estable.
- Contrabalanceo: alternancia del orden de condiciones.
- SUS: escala de usabilidad percibida.
- NASA-TLX Raw: promedio simple de carga de trabajo percibida.
- Think-Aloud: verbalizacion concurrente del usuario.
- Triangulacion: cruce de evidencias.

## Preguntas dificiles de defensa

### Por que no hacer un experimento con grupo de control?

Porque el alcance del proyecto es aplicado y formativo. Busca construir y evaluar un artefacto, no demostrar causalidad psicologica generalizable con diseno experimental puro.

### Es valida una muestra de 8 a 12?

Es valida para evaluacion formativa y deteccion de problemas de UX. No es valida para inferencia estadistica poblacional fuerte. El informe lo reconoce.

### Por que SUS solo para 3D?

Porque SUS evalua la usabilidad de un sistema interactivo. El soporte 2D funciona como referencia de tareas, pero no como sistema equivalente de interaccion. La comparacion 3D vs 2D se hace con desempeno y workload.

### NASA-TLX mide carga cognitiva?

No directamente. Mide carga de trabajo percibida. Puede dialogar con la teoria de carga cognitiva, pero no mide sus categorias teoricas de forma directa.

## Fuentes para estudiar mas

- Peffers et al. (2007), DSRM.
- Hevner et al. (2004), directrices DSR.
- Brooke (1996), SUS.
- Bangor et al. (2008, 2009), interpretacion SUS.
- Sauro y Lewis (2016), estadistica UX.
- Hart y Staveland (1988), NASA-TLX.
- Hart (2006), uso posterior de NASA-TLX.
- Ericsson y Simon (1993), verbal reports.
- Lazar et al. (2017), metodos HCI.
