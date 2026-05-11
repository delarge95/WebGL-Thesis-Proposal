---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
area: desarrollo
tags:
  - tesis
  - shaders
  - view-modes
  - pbr
  - blueprint
---

# INF EST 33 Shaders, View Modes y Entornos

## Idea central

Los modos visuales transforman el modelo en distintas formas de lectura tecnica. No son filtros esteticos. Cada modo responde a una pregunta del usuario:

- como se ve realmente;
- que hay dentro;
- como se separan las piezas;
- donde esta el calor relativo;
- como leerlo como plano tecnico.

## Modos principales

La app implementa varios modos, pero la defensa debe separar implementacion interna de UI visible. En la UI final visible se presentan `Realistic` como base y tarjetas como `X-Ray`, `Solid Color` y `Thermal`. `Blueprint` se comunica mejor como entorno/preset tecnico cuando se activa desde Studio; `Wireframe` y `Ghosted` deben describirse como capacidades ocultas o no publicadas si no estan expuestas.

### Realistic

Estado base. Usa materiales PBR para leer color, metal, plastico, rugosidad y forma.

### X-Ray

Permite ver relaciones internas mediante transparencia o lectura parcial del interior.

### Blueprint

Representa el modelo con lenguaje de plano: lineas, contornos, grilla, fondo tecnico y siluetas. En la narrativa de defensa se trata como entorno o preset tecnico si se activa desde Studio, no como tarjeta publica obligatoria.

### Solid Color

Reduce ruido material para leer forma y contraste.

### Wireframe

Muestra estructura de malla. Es util tecnicamente, pero no necesariamente ideal para usuario final.

### Ghosted

Transparencia analitica para mantener contexto sin saturar.

### Thermal

Visualiza temperatura relativa o tendencia termica heuristica por componente.

## Entornos

Los entornos modifican lectura visual:

- Day;
- Sunset;
- Night;
- Studio;
- Studio Light;
- Blueprint;
- colores base como blanco, gris, negro, amarillo, naranja, verde, azul, purpura y rojo.

Estos presets ayudan a evaluar contraste y presentacion, no son simulacion fisica completa de iluminacion.

## Explicaciones complejas

### PBR y URP

Unity URP proporciona el modelo de materiales. El proyecto no implementa desde cero la ecuacion de renderizado. Usa URP como base y agrega modos de inspeccion.

### Fresnel

Fresnel describe como cambia la reflexion segun el angulo de vista. En visualizacion analitica puede ayudar a resaltar bordes o siluetas.

### Plano de corte

Un plano divide el espacio. Para cada punto del modelo, el shader puede calcular si esta de un lado o del otro. Si esta del lado descartado, no se renderiza.

Idea simple:

```text
si distancia_al_plano < 0, ocultar fragmento
```

Forma vectorial:

```text
distancia = dot(posicion_fragmento - punto_plano, normal_plano)
```

Variables:

- `posicion_fragmento`: punto de la superficie que el shader esta evaluando.
- `punto_plano`: punto de referencia por donde pasa el plano de corte.
- `normal_plano`: direccion perpendicular al plano.
- `dot`: producto punto; mide cuanto apunta un vector en la direccion de otro.

Lectura aplicada:

Si el usuario mueve el plano de corte hacia el centro del dron, cada fragmento calcula si queda delante o detras del plano. Los fragmentos del lado descartado desaparecen, permitiendo ver interior, distribucion de piezas o separacion entre niveles del ensamblaje.

Ejemplo:

```text
distancia = -0.12 -> ocultar
distancia =  0.08 -> renderizar
```

Esto explica por que el clipping no "rompe" la malla original: solo decide que fragmentos se dibujan en pantalla.

### Fresnel para siluetas

Forma conceptual:

```text
F = F0 + (1 - F0) * (1 - dot(v, n))^5
```

Variables:

- `F`: intensidad del efecto Fresnel.
- `F0`: reflectancia base.
- `v`: direccion hacia la camara.
- `n`: normal de la superficie.
- `dot(v, n)`: alineacion entre mirada y superficie.

En el visor:

Cuando una superficie se ve de frente, `dot(v, n)` es alto y el borde se resalta menos. Cuando se ve de lado, el valor baja y el borde puede resaltarse. Esto ayuda a que X-Ray, Ghosted o Blueprint conserven lectura de silueta aunque la geometria sea transparente o simplificada.

### Blueprint

Blueprint combina fondo, grilla, contornos y color de linea para convertir el 3D en lectura tecnico-diagramatica.

Logica visual:

```text
color_final = mezcla(color_base, color_linea, intensidad_borde + intensidad_grilla)
```

Variables:

- `color_base`: fondo o tono tecnico principal.
- `color_linea`: color de contorno o trazo.
- `intensidad_borde`: valor que aumenta en siluetas y discontinuidades.
- `intensidad_grilla`: patron que simula plano tecnico.

Ejemplo del proyecto:

Blueprint no pretende reemplazar un plano CAD. Su funcion es transformar el dron en una lectura de inspeccion: siluetas limpias, contraste alto y un lenguaje visual cercano a documentacion tecnica.

### Entornos como control de lectura

Los presets de entorno pueden entenderse como:

```text
lectura_visual = material + luz + fondo + contraste + modo_visual
```

Si el fondo y el material tienen contraste bajo, el usuario pierde bordes. Por eso Day, Night, Sunset, Studio y Blueprint no son solo decoracion: permiten probar legibilidad en distintas condiciones visuales.

## Terminos importantes

- Shader: programa de GPU que define apariencia.
- Material: conjunto de propiedades visuales aplicadas a una malla.
- URP: Universal Render Pipeline de Unity.
- PBR: renderizado basado en fisica.
- Fresnel: variacion de reflexion segun angulo.
- Clipping: ocultar partes de geometria segun una condicion.
- Wireframe: visualizacion de aristas de la malla.
- X-Ray: modo de transparencia o lectura interna.
- Blueprint: lectura tecnica tipo plano.
- Preset: configuracion guardada de apariencia.
- View mode: modo de visualizacion.

## Preguntas dificiles de defensa

### Por que no mostrar todos los modos en la UI final?

Porque mas opciones no siempre mejoran la experiencia. Algunos modos estan implementados pero ocultos para no saturar el flujo final.

### El modo Blueprint es final?

Esta implementado como lectura tecnica, pero no debe describirse como tarjeta visible si la UI final no lo expone de esa manera. En la app actual se entiende mejor desde el ciclo de entorno/preset.

### El thermal es shader o simulacion?

Es ambas cosas en capas distintas: una logica calcula valores termicos heurísticos y el shader los traduce a color.

## Fuentes relacionadas

- [[Estrategia_Shaders_WebGL]]
- [[INF_EST_02_Marco_Referencia]]
- [[INF_EST_34_Sistema_Termico_Hibrido]]
- Unity URP documentation.
