# Planificación de Iconos Procedurales y Microinteracciones Matemáticas (C#)

**Enfoque:** 100% Código Procedimental (Sin Assets Externos).
**Plataforma definida:** UI Toolkit (Unity 6).

Entendido el objetivo. Tienes toda la razón al inclinarte por la generación en código (Procedural C#). Las ventajas técnicas en WebGL (cero peso, nitidez matemática infinita) y el control granular de las interpolaciones hacen que las animaciones sean mucho más elegantes, orgánicas y complejas que un simple cambio de escala o color en CSS.

En **UI Toolkit**, este enfoque procedural se logra creando clases personalizadas que heredan de `VisualElement` y utilizando sus capacidades geométricas nativas o sub-elementos virtuales animados vía tareas programadas (Scheduled Tasks) y `Mathf.Lerp`.

A continuación, la estructura analizada de tus botones reales y el boceto matemático/visual de cada icono minimalista:

---

## 1. Menú Principal / Utilidades Habituales

### Botón: **Home**
- **Estética Minimalista:** Un triángulo simple (techo) elevado un par de píxeles sobre un rectángulo (base), creados con líneas de 1.5px.
- **Microinteracción Procedural:** 
  - *Hover:* El techo (triángulo) se separa verticalmente usando una curva de resorte (*Spring/Damping math*), logrando un rebote suave. Al mismo tiempo, la línea base del rectángulo se contrae hacia el centro.
  - *Click:* Todo colapsa en un punto temporalmente y rebota a su escala original (squash & stretch).

### Botón: **Reset View**
- **Estética Minimalista:** Dos flechas curvas semicirculares formando un anillo incompleto, o una mirilla de enfoque (rectángulos en las esquinas).
- **Microinteracción Procedural:**
  - *Hover:* El anillo rota 45 grados exponencialmente (`EaseOutExpo`) mientras su grosor de línea se vuelve un poco más fino (tensión visual).
  - *Click:* Una rotación completa (360°) rápida y limpia.

---

## 2. Categoría: Inspect (Exploración Visual)
*Icono del Padre (Menú Inspect): Una lupa construida con un simple círculo y una línea a 45 grados.*
*Animación del Padre:* El círculo funciona como un radar, una onda invisible sale de su centro y "barre" el cristal de la lupa periódicamente usando matemáticas sinusoidales (`Mathf.Sin(Time.time)`).

### Botón: **Info**
- **Estética Minimalista:** Un círculo fino con un pequeño punto suspendido justo por encima de una línea corta central.
- **Microinteracción Procedural:**
  - *Hover:* El punto levita en un movimiento pendular muy corto (Mathf.Sin), mientras que la línea inferior se inclina hacia la iteración del ratón como si siguiera la posición del puntero.

### Botón: **Pins**
- **Estética Minimalista:** La clásica "gota" de ubicación o un simple círculo con un vector de caída hacia abajo.
- **Microinteracción Procedural:**
  - *Hover:* El pin se eleva y dibuja una sombra elíptica procedural debajo de él (un `VisualElement` curvo que cambia de ancho).
  - *Click:* Efecto de "Clavarse": cae violentamente, aplastando su propio eje Y (`transform.scale.y`), simulando impacto, y regresa con un pequeño muelle.

### Botón: **Isolate**
- **Estética Minimalista:** Un pequeño cuadrado sólido en el centro rodeado por un marco cuadrado más grande construido por 4 líneas (brackets `[ ]`).
- **Microinteracción Procedural:**
  - *Hover:* Las líneas exteriores se dividen y se deslizan hacia afuera, desvaneciéndose en alpha, mientras que el cuadrado central crece, demostrando visualmente el concepto de "aislar lo importante y apartar el resto".

---

## 3. Categoría: Analyze (Geometría y Datos)
*Icono del Padre (Menú Analyze): Un hexágono seccionado o una grilla matemática.*
*Animación del Padre:* Lados de la forma geométrica cambian sutilmente de longitud simulando el procesamiento de datos dinámicos.

### Botón: **Cut** (Sección Transversal)
- **Estética Minimalista:** Un círculo vacío interceptado por una fina línea diagonal.
- **Microinteracción Procedural:**
  - *Hover:* Las dos mitades del círculo (arriba/izquierda y abajo/derecha de la línea) se separan físicamente alejándose perpendicularmente del corte, mientras la línea crece ligeramente.
  - *Click:* La línea hace un tajo rápido cruzando de lado a lado.

### Botón: **Explode**
- **Estética Minimalista:** El script clásico que compartiste (tres capas isométricas en forma de diamantes).
- **Microinteracción Procedural:**
  - *Hover (Spread):* Los diamantes se separan de su eje Y suavemente.
  - *Click (Compress):* Colapsan sobre sí mismos temporalmente.

### Botón: **Filter**
- **Estética Minimalista:** Tres líneas horizontales paralelas de distinto grosor y longitud (ancho, medio, corto) apiladas en forma de embudo.
- **Microinteracción Procedural:**
  - *Hover:* Las tres líneas se alinean asimétricamente simulando un ecualizador, deslizándose hacia la derecha y la izquierda suavemente, luego regresando a orden decreciente.

---

## 4. Categoría: Studio (Aspecto Visual y Rendimiento)
*Icono del Padre (Menú Studio): Una flor de apertura de lente de cámara, construida por intersecciones de vectores.*
*Animación del Padre:* Las aspas de la apertura se cierran y se abren un 10% mediante una rotación pivotada matemáticamente calculada.

### Botón: **Shaders** (Control Térmico/X-ray/Wireframe)
- **Estética Minimalista:** Un rectángulo partido diagonalmente; una mitad es sólida y la otra contiene una grilla (`wireframe`).
- **Microinteracción Procedural:**
  - *Hover:* Una línea vectorizada vertical barre el icono de izquierda a derecha. Mientras la línea pasa, la cuadrícula desaparece y se vuelve sólida (o viceversa), ilustrando el concepto de "cambiar de material de renderizado".

### Botón: **Environment** (Entornos de luz, HDRI)
- **Estética Minimalista:** Una esfera/círculo representando un sol eclipsado por un trazo recto horizontal (horizonte).
- **Microinteracción Procedural:**
  - *Hover:* El círculo se eleva sobre la línea del horizonte replicando un amanecer, cambiando su coordenada Y mediante un `Mathf.Lerp` amortiguado. Al salir, el círculo emite un "resplandor" vectorizado procedural de opacidad muy baja.

---

## Metodología de Implementación (Próximos Pasos)

Para lograr esto en Unity usando UI Toolkit, nuestro desarrollo fluirá de la siguiente manera:

1. **La Clase Base Matemática (`ProceduralIconBase.cs`):** 
   Crearemos un componente C# `VisualElement` maestro que manejará la física del muelle (*Spring Physics*) para que ninguna animación se sienta lineal, rígida o "falsa". Todo debe sentirse orgánico (ease-out elástico, pesos inerciales).
2. **La API Geométrica (Painter2D):** 
   En lugar de crear decenas de `GameObjects` por cada línea (que es el equivalente rústico de UGUI), usaremos la API `MeshGenerationContext` propia de UI Toolkit (con comandos análogos al canvas de JavaScript como `LineTo()`, `ArcTo()`, `MoveTo()`) para dibujar las formas en 1 solo *Draw Call* por botón basando sus puntos en variables animadas.
3. **Desarrollo Modular (Icono a Icono):** 
   Comenzaremos, una a una, a escribir las clases hijas (Ej: `ProceduralHomeIcon`, `ProceduralCutIcon`), enlazándolas al `MainLayout.uxml`.
