# Cuestionario de Usabilidad del Sistema (SUS)
## System Usability Scale - Versión en Español

---

## Instrucciones para el evaluador

Este cuestionario utiliza la escala **System Usability Scale (SUS)** desarrollada por John Brooke (1996). Es una herramienta validada internacionalmente para medir la usabilidad percibida de un sistema.

### Aplicación
1. El participante usa el prototipo por **10 a 15 minutos** realizando las tareas asignadas.
2. Inmediatamente después, completa este cuestionario.
3. Debe responder basándose en su experiencia reciente con el sistema.
4. No se deben modificar respuestas una vez registradas.

### Cálculo del puntaje
1. Para ítems impares (1, 3, 5, 7, 9): restar 1 al valor marcado.
2. Para ítems pares (2, 4, 6, 8, 10): calcular `5 - valor marcado`.
3. Sumar los diez valores ajustados.
4. Multiplicar la suma por 2.5.
5. El resultado final es un puntaje entre 0 y 100.

En notación del informe:

```text
SUS = 2.5 * Σ S_i
```

donde `S_i` corresponde al valor ajustado de cada ítem.

### Interpretación orientadora

| Puntaje | Lectura aproximada |
|---------|--------------------|
| 0-25 | Inaceptable |
| 26-50 | Pobre |
| 51-67 | Bajo / cercano al promedio inferior |
| 68 | Promedio histórico del instrumento |
| 69-72 | Aceptable / favorable |
| 73-80 | Bueno |
| 81-100 | Muy bueno a excelente |

**Nota metodológica:** el valor 68 se toma como referencia de promedio histórico del instrumento y no como prueba automática de “buena usabilidad”. Para este proyecto, valores iguales o superiores a 72 se consideran una referencia más favorable, pero siempre deben interpretarse junto con observación cualitativa, desempeño en tareas y comentarios de usuarios.

---

## Datos del participante

**Código de participante:** _____________ (ej.: P01, P02...)

**Fecha:** _____________

**Perfil:**
- [ ] Ingeniero o técnico en hardware
- [ ] Estudiante de ingeniería
- [ ] Profesional de diseño 3D
- [ ] Otro: _____________

**Experiencia con visualizadores 3D:**
- [ ] Ninguna
- [ ] Básica (menos de 1 año)
- [ ] Intermedia (1 a 3 años)
- [ ] Avanzada (más de 3 años)

**Dispositivo utilizado:**
- [ ] PC de escritorio
- [ ] Laptop
- [ ] Tablet
- [ ] Smartphone

**Navegador utilizado:** ______________________

---

## Tareas completadas

Antes del cuestionario, el participante debe completar las siguientes tareas sobre la build evaluada:

| # | Tarea | Completada |
|---|-------|------------|
| 1 | Navegar alrededor del dron usando los controles disponibles del sistema | [ ] Sí [ ] No |
| 2 | Seleccionar una pieza y revisar su información en el \textit{bottom sheet} | [ ] Sí [ ] No |
| 3 | Utilizar una herramienta de \texttt{Inspect} o \texttt{Analyze} (por ejemplo, \texttt{Isolate}, \texttt{Explode} o \texttt{Cut}) | [ ] Sí [ ] No |
| 4 | Cambiar de modo visual en \texttt{Studio} | [ ] Sí [ ] No |
| 5 | Interpretar la vista térmica y su leyenda, si estuvo incluida en la condición de prueba | [ ] Sí [ ] No |

**Tiempo total de exploración:** _______ minutos

---

## Cuestionario SUS

Por favor, marque el número que mejor represente su opinión para cada afirmación.

**Escala:**
```text
1 = Totalmente en desacuerdo
2 = En desacuerdo
3 = Neutral
4 = De acuerdo
5 = Totalmente de acuerdo
```

---

### 1. Creo que me gustaría usar este sistema con frecuencia.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 2. Encontré el sistema innecesariamente complejo.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 3. Pensé que el sistema era fácil de usar.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 4. Creo que necesitaría el apoyo de un técnico para poder usar este sistema.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 5. Encontré que las diversas funciones de este sistema estaban bien integradas.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 6. Pensé que había demasiada inconsistencia en este sistema.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 7. Imagino que la mayoría de las personas aprenderían a usar este sistema muy rápidamente.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 8. Encontré el sistema muy difícil de usar.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 9. Me sentí muy confiado usando el sistema.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

### 10. Necesité aprender muchas cosas antes de poder empezar a usar este sistema.

```text
Totalmente en desacuerdo  [ 1 ]  [ 2 ]  [ 3 ]  [ 4 ]  [ 5 ]  Totalmente de acuerdo
```

---

## Cálculo del puntaje (uso del evaluador)

| Ítem | Respuesta | Cálculo | Valor ajustado |
|------|-----------|---------|----------------|
| 1 | ___ | ___ - 1 = | ___ |
| 2 | ___ | 5 - ___ = | ___ |
| 3 | ___ | ___ - 1 = | ___ |
| 4 | ___ | 5 - ___ = | ___ |
| 5 | ___ | ___ - 1 = | ___ |
| 6 | ___ | 5 - ___ = | ___ |
| 7 | ___ | ___ - 1 = | ___ |
| 8 | ___ | 5 - ___ = | ___ |
| 9 | ___ | ___ - 1 = | ___ |
| 10 | ___ | 5 - ___ = | ___ |

**Suma de valores ajustados:** ___

**Puntaje SUS = Suma × 2.5 =** ___

---

## Comentarios adicionales (opcional)

**¿Qué fue lo que más le gustó del sistema?**

_________________________________________________________________

_________________________________________________________________

**¿Qué mejoraría del sistema?**

_________________________________________________________________

_________________________________________________________________

**Comentarios adicionales:**

_________________________________________________________________

_________________________________________________________________

---

## Firma

**Participante:** _________________________ **Fecha:** _____________

**Evaluador:** _________________________ **Firma:** _____________

---

*Instrumento basado en Brooke, J. (1996). SUS: A quick and dirty usability scale.*  
*Versión adaptada para el proyecto de visualización técnica web 3D - UNAD 2026.*
