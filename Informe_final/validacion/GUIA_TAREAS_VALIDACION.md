# Guía de Tareas de Validación
## Protocolo comparativo 3D vs. 2D

---

## Objetivo

Estandarizar la ejecución de tareas para la validación formativa del proyecto, garantizando equivalencia funcional entre:

- **Condición A:** visor web 3D interactivo.
- **Condición B:** soporte 2D de referencia.

Las tareas están diseñadas para comparar:

- desempeño en tareas;
- carga de trabajo percibida;
- observaciones cualitativas de uso.

El cuestionario SUS se aplica **solo después de la condición 3D**.

---

## Documentos asociados

Antes de aplicar esta guía, el evaluador debe preparar:

- `01_CONSENTIMIENTO_INFORMADO.md`;
- `02_HOJA_INSTRUCCIONES_PARTICIPANTE.md`;
- `03_CUESTIONARIO_SUS_PARTICIPANTE.md`;
- `04_CUESTIONARIO_NASA_TLX_PARTICIPANTE.md`;
- `05_FORMATO_REGISTRO_MODERADOR.md`.

Los cuestionarios completos `CUESTIONARIO_SUS.md` y `CUESTIONARIO_NASA_TLX.md` se conservan como versiones de cálculo e interpretación para el evaluador, no como documentos principales de entrega al participante.

---

## Registro previo de sesión

Antes de iniciar, el evaluador debe registrar:

- confirmación de consentimiento informado;
- código del participante;
- fecha y hora;
- dispositivo;
- sistema operativo;
- navegador y versión;
- resolución;
- versión de build o fecha de despliegue;
- estado de caché antes de iniciar;
- secuencia asignada (`AB` o `BA`).

No se debe registrar el nombre real del participante en la matriz de resultados. La identificación debe hacerse mediante código anónimo.

---

## Secuencias de aplicación

### Secuencia AB
1. Condición A: visor 3D.
2. Condición B: soporte 2D.

### Secuencia BA
1. Condición B: soporte 2D.
2. Condición A: visor 3D.

La asignación del orden se realiza por alternancia simple: los participantes con código impar siguen la secuencia AB y los participantes con código par siguen la secuencia BA. Esta regla permite distribuir el posible efecto de aprendizaje o fatiga sin introducir selección subjetiva del moderador.

---

## Instrucción general al participante

> Va a realizar una serie de tareas de exploración e identificación sobre el mismo sistema usando dos medios distintos. No estamos evaluándolo a usted; estamos evaluando qué tan claro resulta cada medio para comprender el ensamblaje. Por favor, realice las tareas como lo haría normalmente y, cuando se le indique, verbalice lo que piensa mientras interactúa.

---

## Piezas objetivo y control de equivalencia

Para mantener comparabilidad, las piezas objetivo deben definirse antes de iniciar la sesión y mantenerse iguales entre participantes. Se recomienda usar un conjunto pequeño de componentes visibles tanto en el visor 3D como en el soporte 2D:

| Rol en la tarea | Pieza sugerida | Motivo |
|---|---|---|
| Componente principal | Batería o módulo de potencia | Fácil de relacionar con función energética |
| Componente de propulsión | Motor o hélice | Permite relación espacial con brazo y montaje |
| Componente central | Pixhawk o placa superior | Permite interpretar ubicación y función de control |
| Componente estructural | Landing gear, rail system o brazo | Permite lectura espacial y de ensamblaje |

El evaluador puede sustituir estas piezas si la build o el soporte 2D usado en la sesión lo exige, pero debe registrar el cambio y mantener la misma pieza en ambas condiciones.

---

## Tareas equivalentes por condición

### Tarea 1: Ubicación de componente

**Objetivo:** localizar un componente específico dentro del ensamblaje.

- En 3D: el participante debe navegar, encontrar la pieza y seleccionarla.
- En 2D: el participante debe ubicar la misma pieza en el soporte de referencia.

**Criterio de éxito:** identifica correctamente la pieza sin ayuda.

**Datos a registrar:**
- tiempo en segundos;
- éxito o fracaso;
- errores;
- ayuda requerida.

---

### Tarea 2: Interpretación funcional

**Objetivo:** explicar la función general del componente seleccionado y su relación con el sistema.

- En 3D: puede apoyarse en la ficha inferior y en la inspección visual.
- En 2D: puede apoyarse en la documentación estática disponible.

**Criterio de éxito:** ofrece una descripción funcional razonablemente coherente con la referencia técnica.

**Datos a registrar:**
- tiempo en segundos;
- calidad de respuesta;
- dudas expresadas;
- apoyo requerido.

**Rúbrica sugerida de calidad de respuesta:**
- `0`: no logra explicar la función.
- `1`: ofrece explicación parcial o ambigua.
- `2`: explica función y relación básica con el sistema.

---

### Tarea 3: Relación espacial

**Objetivo:** identificar cómo se relaciona el componente con elementos cercanos del ensamblaje.

- En 3D: puede usar navegación, selección, \texttt{Inspect}, \texttt{Explode} o \texttt{Cut} si la condición lo permite.
- En 2D: debe apoyarse en las vistas o diagramas del soporte de referencia.

**Criterio de éxito:** describe al menos una relación espacial correcta entre el componente y el ensamblaje.

**Datos a registrar:**
- tiempo en segundos;
- éxito;
- errores de interpretación;
- necesidad de orientación externa.

**Rúbrica sugerida de relación espacial:**
- `0`: no identifica relación espacial correcta.
- `1`: identifica una relación general, pero con dudas o imprecisiones.
- `2`: identifica al menos una relación espacial correcta y la explica con claridad.

---

### Tarea 4: Análisis visual guiado

**Objetivo:** realizar una acción de inspección técnica sobre el sistema.

- En 3D: usar una herramienta visible del visor, por ejemplo \texttt{Explode}, \texttt{Cut} o cambio de modo visual.
- En 2D: interpretar una vista equivalente del soporte estático para resolver la misma pregunta.

**Criterio de éxito:** completa la acción o interpreta correctamente la información equivalente.

**Datos a registrar:**
- éxito;
- dudas;
- errores;
- ayudas requeridas.

La T4 se conserva como tarea exploratoria guiada. No se cronometra y su aporte se analiza mediante observación cualitativa y protocolo Think-Aloud.

---

## Cierre por condición

Después de cada condición:

- registrar éxito, errores, ayudas y observaciones por tarea;
- registrar tiempo en segundos para T1, T2 y T3;
- diligenciar NASA-TLX Raw para esa condición;
- conservar observaciones del evaluador.

Después de la condición 3D:

- aplicar SUS;
- completar observaciones abiertas del participante.

---

## Criterios de ayuda durante tareas

Para no sesgar la prueba, las ayudas deben clasificarse y mantenerse mínimas:

- **Ayuda baja:** repetir la instrucción sin añadir información nueva.
- **Ayuda media:** aclarar el objetivo sin indicar el botón o ruta exacta.
- **Ayuda alta:** indicar una acción concreta para desbloquear al participante.

Si se usa ayuda media o alta, la tarea no debe registrarse como éxito completamente autónomo.

---

## Nota metodológica

Esta guía busca equivalencia funcional entre condiciones, no identidad formal de interfaz. La comparación entre 3D y 2D debe interpretarse de forma descriptiva y triangulada con observación cualitativa, especialmente cuando el tamaño muestral se mantenga en rango formativo.
