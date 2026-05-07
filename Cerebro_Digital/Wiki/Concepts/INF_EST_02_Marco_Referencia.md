---
tipo: modulo_estudio
fuente: Informe_final/chapters/02_marco_referencia.tex
estado: activo
capitulo: 2
area: informe-final
tags:
  - tesis
  - marco-referencia
  - pbr
  - webgl
---

# INF EST 02 Marco de Referencia

## Idea central

El marco de referencia ubica el proyecto dentro de la visualizacion Web 3D y explica las bases teoricas que justifican las decisiones de diseno, interaccion, renderizado y evaluacion.

En palabras simples: este capitulo responde "que sabemos antes de construir" y "con que lentes vamos a interpretar el proyecto".

## Estado del arte

### Que se compara

El informe revisa tecnologias para experiencias 3D web:

- Three.js;
- Babylon.js;
- Unity Web;
- Unreal Pixel Streaming;
- Spline;
- Marmoset Viewer;
- Sketchfab;
- PlayCanvas.

La comparacion no es un benchmark experimental. Es una comparacion cualitativa para decidir que tecnologia se ajusta mejor al caso.

### Como defender la seleccion de Unity Web

La defensa correcta no es que Unity sea universalmente superior. La defensa es contextual:

- Three.js y Babylon.js son muy fuertes, pero exigirian construir mas infraestructura propia.
- Pixel Streaming puede verse muy bien, pero depende de servidores y red.
- Sketchfab, Spline y Marmoset son utiles para exhibicion, pero limitan logica propia.
- Unity integra editor, materiales, UI, shaders, perfiles y build web.

La palabra clave es equilibrio. Unity fue seleccionado por equilibrio entre control, velocidad de implementacion y compatibilidad con un pipeline de Technical Art.

## Marco teorico

### Teoria de la Carga Cognitiva

Esta teoria explica por que un documento 2D puede ser dificil: la memoria de trabajo humana es limitada. Cuando el usuario debe imaginar rotaciones, posiciones y relaciones de piezas, parte de su energia mental se gasta en reconstruir el objeto, no en entenderlo.

Importante: NASA-TLX no mide directamente las categorias de la Teoria de la Carga Cognitiva. NASA-TLX mide workload percibido. La teoria ayuda a interpretar, pero el instrumento mide otra cosa.

### Navegacion espacial e interaccion 3D

La interaccion 3D debe evitar desorientar. Por eso son importantes:

- camara orbital;
- punto de interes;
- zoom controlado;
- seleccion clara;
- feedback visual;
- paneles o fichas que reduzcan memoria del usuario.

La interfaz actua como apoyo cognitivo: no solo muestra informacion, tambien ayuda a pensar.

### Heuristicas de usabilidad

Nielsen aporta reglas generales para que el sistema sea comprensible:

- el usuario debe saber que esta pasando;
- los nombres deben parecerse al lenguaje real del dominio;
- se debe poder salir de acciones o modos;
- la interfaz debe mostrar opciones para evitar memorizar comandos.

## Fundamentos matematicos

### Optimizacion grafica

La optimizacion web no es solo "bajar calidad". Es decidir que se mantiene y que se simplifica para que el sistema siga siendo legible y fluido.

Terminos clave:

- presupuesto poligonal: cuantos triangulos puede manejar la escena;
- densidad de texel: cuanta resolucion de textura corresponde a una zona del modelo;
- draw calls: cuantas ordenes de dibujo envia la CPU a la GPU;
- frame time: cuanto tarda cada cuadro.

### PBR

PBR significa renderizado basado en fisica. No quiere decir que el proyecto simule todo el mundo real. Quiere decir que los materiales siguen reglas coherentes de luz, rugosidad y reflexion.

El informe menciona:

- Kajiya para la ecuacion de renderizado;
- Cook-Torrance para la BRDF especular;
- Schlick para Fresnel;
- GGX para microfacetas.

En defensa, conviene aclarar que la app no implementa desde cero toda la teoria PBR. Unity URP provee la base; el proyecto la usa y la extiende con shaders de inspeccion.

### Shaders de inspeccion

Los shaders propios no buscan solo realismo. Sirven para leer tecnicamente el modelo:

- corte transversal;
- X-Ray;
- Blueprint;
- contornos;
- visualizacion termica.

El valor de estos modos es que convierten geometria compleja en informacion visual interpretable.

## Preguntas dificiles de defensa

### Si Three.js es mas ligero, por que no usarlo?

Porque el objetivo no era solo mostrar un modelo. El proyecto necesitaba tooling integrado, UI, shaders, seleccion, estados, profiling, tooling y build web. Three.js era viable, pero exigia construir mas infraestructura desde cero.

### El PBR garantiza realismo?

Garantiza coherencia fisica relativa en el modelo de materiales, no realismo absoluto. La fidelidad final depende de texturas, iluminacion, escala, assets y configuracion del motor.

### El modo termico es una simulacion cientifica?

No. Es una visualizacion heuristica aplicada, util para representar tendencias relativas de carga y temperatura. No reemplaza FEA ni medicion fisica calibrada.

## Explicaciones complejas dentro de la seccion

### Por que una teoria puede ser marco y no medicion

La Teoria de la Carga Cognitiva ayuda a interpretar por que una representacion 3D podria reducir esfuerzo de reconstruccion mental. Pero eso no significa que el proyecto mida directamente carga intrinseca, extrinseca y germana. Esa distincion evita mezclar fundamento teorico con instrumento empirico.

### Relacion entre Kajiya y Cook-Torrance

Kajiya formula la ecuacion general de renderizado: como se transporta la luz en una escena. Cook-Torrance describe una forma concreta de modelar reflexion especular sobre microfacetas. La app usa URP como implementacion de pipeline; el informe explica las bases para justificar el comportamiento visual.

### Formula conceptual de frame time

```text
FPS = 1000 / frame_time_ms
```

Si el frame time es 33.33 ms, entonces:

```text
FPS aproximado = 1000 / 33.33 = 30
```

Por eso la meta de 30 FPS se expresa tambien como frame time menor o igual a 33.33 ms.

### Ecuacion de renderizado de Kajiya

Forma conceptual:

```text
L_o(x, omega_o) = L_e(x, omega_o) + integral_Omega f_r(x, omega_i, omega_o) L_i(x, omega_i) (n . omega_i) d omega_i
```

Lectura por partes:

- `L_o`: luz que sale desde un punto de la superficie hacia la camara.
- `L_e`: luz emitida por la superficie, si el material emite luz.
- `f_r`: BRDF, es decir, como el material transforma luz entrante en luz saliente.
- `L_i`: luz que llega al punto desde una direccion.
- `n . omega_i`: factor angular; la luz de frente aporta mas que la luz rasante.
- `integral_Omega`: suma continua de todas las direcciones de luz posibles sobre el hemisferio visible.

Como se usa en el proyecto:

La app no resuelve esta integral manualmente. Unity URP la aproxima mediante su pipeline de iluminacion, materiales e iluminacion ambiental. En la tesis, la ecuacion sirve para explicar por que color, rugosidad, metalicidad, normales e iluminacion cambian la lectura visual de piezas tecnicas.

Ejemplo aplicado:

Una pieza negra rugosa del frame no refleja igual que un tornillo metalico. Aunque ambos tengan la misma geometria, `f_r` cambia. Por eso el PBR ayuda a distinguir materiales y no solo siluetas.

### BRDF Cook-Torrance en palabras claras

Forma simplificada del termino especular:

```text
f_specular = (D * F * G) / (4 * (n . v) * (n . l))
```

Variables:

- `D`: distribucion de microfacetas; controla cuantas microcaras estan orientadas para reflejar luz hacia la camara.
- `F`: Fresnel; aumenta la reflexion cuando el angulo de vista es rasante.
- `G`: geometria o enmascaramiento; representa cuanto se tapan unas microfacetas a otras.
- `n`: normal de la superficie.
- `v`: direccion hacia la camara.
- `l`: direccion hacia la luz.

Como afecta al visor:

- mayor rugosidad dispersa el brillo y reduce reflejos concentrados;
- mayor metalicidad cambia la relacion entre color base y reflexion;
- Fresnel puede hacer mas visibles bordes y siluetas, algo util en modos analiticos;
- normales mal importadas desde CAD pueden producir brillos incorrectos, por eso la limpieza del modelo importa.

### Presupuesto grafico como ecuacion de decision

Una forma practica de explicar optimizacion es:

```text
costo_frame = costo_geometria + costo_materiales + costo_sombras + costo_UI + costo_scripts
```

No es una formula fisica universal. Es un modelo mental de ingenieria para diagnosticar rendimiento.

En este proyecto:

- `costo_geometria` aumenta con triangulos, submeshes y renderers;
- `costo_materiales` aumenta con shaders complejos, transparencias y variantes;
- `costo_sombras` aumenta si muchas luces o objetos generan sombras;
- `costo_UI` aumenta con paneles, blur, animaciones y repintado;
- `costo_scripts` aumenta con actualizaciones por frame, busquedas y eventos mal controlados.

Ejemplo:

Si el modo X-Ray usa transparencia, el costo puede subir porque el orden de render y el overdraw se vuelven mas exigentes. Por eso algunos modos no se exponen todos al usuario final: no es solo UX, tambien es presupuesto grafico.

## Terminos importantes de la seccion

- Estado del arte: revision de alternativas existentes.
- Benchmarking cualitativo: comparacion argumentada sin medicion experimental propia.
- PBR: modelo de materiales basado en reglas fisicas.
- BRDF: funcion que describe reflexion de luz.
- Fresnel: variacion de reflexion segun angulo.
- GGX: distribucion de microfacetas usada en materiales rugosos.
- IBL: iluminacion basada en imagenes.
- Shader: programa de GPU para apariencia visual.
- Clipping: ocultamiento de fragmentos mediante condicion espacial.
- Workload percibido: esfuerzo subjetivo reportado por el usuario.

## Fuentes para estudiar mas

- Akenine-Moller, Haines y Hoffman, Real-Time Rendering.
- Kajiya (1986), rendering equation.
- Cook y Torrance (1982), BRDF.
- Schlick (1994), Fresnel.
- Bowman et al. (2004), 3D user interfaces.
- Nielsen (1994), heuristicas de usabilidad.
- Unity URP documentation.
