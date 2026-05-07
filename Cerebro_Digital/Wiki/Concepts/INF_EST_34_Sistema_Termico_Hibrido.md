---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
area: desarrollo
tags:
  - tesis
  - termico
  - simulacion
  - wolfram
---

# INF EST 34 Sistema Termico Hibrido

## Idea central

El modo termico no es FEA ni termodinamica calibrada. Es una visualizacion heuristica aplicada: calcula temperaturas relativas por componente y las traduce a colores para apoyar la inspeccion.

La defensa correcta es hablar de modelo reducido por componentes.

## Arquitectura

La cadena funcional es:

- `DroneStateController`: aporta estado de carga o potencia.
- `ThermalSimulationManager`: actualiza temperaturas por nodo.
- `ThermalViewController`: traduce temperaturas a UI, shader y leyenda.

## Formula 1: temperatura de equilibrio

Si la pieza no genera calor:

```text
T_eq = T_amb
```

Si genera calor, su equilibrio se calcula segun carga efectiva entre temperatura ambiente, hover y pico.

Forma conceptual para una pieza activa:

```text
T_eq = T_amb + carga * (T_pico - T_amb)
```

Variables:

- `T_eq`: temperatura hacia la que tiende la pieza si la carga se mantiene.
- `T_amb`: temperatura ambiente del escenario.
- `carga`: valor normalizado entre 0 y 1 que representa demanda relativa.
- `T_pico`: temperatura maxima heuristica definida para esa pieza.

Ejemplo del proyecto:

Si el motor tiene `T_amb = 25`, `T_pico = 80` y `carga = 0.6`:

```text
T_eq = 25 + 0.6 * (80 - 25) = 58
```

Eso no significa que el motor real mediria exactamente 58 grados. Significa que, en la visualizacion, ese motor debe tender a una zona mas caliente que una pieza pasiva cuando el usuario aumenta potencia o carga.

## Formula 2: calentamiento exponencial

```text
Delta T_source = (T_eq - T_actual) (1 - e^(-Delta t / tau)) w_s
```

Lectura simple:

- si la temperatura actual esta lejos del equilibrio, cambia mas;
- si ya esta cerca, cambia menos;
- `tau` controla que tan rapido se acerca;
- `w_s` ajusta el peso de esa fuente de calor.

Como incide cada variable:

- si `T_eq - T_actual` es grande, el cambio inicial es mas fuerte;
- si `Delta t` aumenta, el paso de simulacion permite un cambio mayor;
- si `tau` es grande, la pieza se calienta lentamente;
- si `tau` es pequeno, la pieza responde rapido;
- si `w_s` sube, esa fuente pesa mas en el resultado.

Ejemplo:

```text
T_eq = 58
T_actual = 30
Delta t = 1
tau = 10
w_s = 1
Delta T_source = (58 - 30) * (1 - e^(-1/10)) * 1
Delta T_source aproximado = 28 * 0.095 = 2.66
```

La temperatura pasaria de 30 a 32.66 en ese paso. La curva evita saltos bruscos de color en el shader termico.

## Formula 3: enfriamiento

```text
Delta T_cooling = (T_amb - T_actual) c_r phi_exp Delta t
```

Lectura simple:

- si la pieza esta mas caliente que el ambiente, tiende a enfriarse;
- `c_r` regula la velocidad;
- `phi_exp` representa exposicion convectiva;
- `Delta t` ajusta el cambio por tiempo.

Como incide cada variable:

- si `T_actual` es mayor que `T_amb`, el termino es negativo y enfria;
- si `c_r` sube, el enfriamiento se acelera;
- si `phi_exp` es alto, la pieza esta mas expuesta al ambiente;
- si `phi_exp` es bajo, la pieza esta mas encerrada o protegida.

Ejemplo:

```text
T_amb = 25
T_actual = 50
c_r = 0.02
phi_exp = 0.8
Delta t = 1
Delta T_cooling = (25 - 50) * 0.02 * 0.8 * 1 = -0.4
```

La pieza baja 0.4 unidades en ese paso. En el visor, esto suaviza la vuelta del color rojo/naranja hacia tonos mas frios.

## Formula 4: transferencia entre piezas

```text
Delta T_ij = (T_j - T_i) G_hat_ij Delta t
```

Lectura simple:

- si una pieza vecina esta mas caliente, puede transferir calor;
- la diferencia de temperatura dirige el flujo;
- `G_hat_ij` es un acoplamiento heuristico, no una conductancia SI calibrada.

Como incide cada variable:

- si `T_j` es mayor que `T_i`, la pieza `i` recibe calor;
- si `T_j` es menor, `i` pierde calor hacia `j`;
- si `G_hat_ij` crece, las piezas se influyen mas;
- si `Delta t` crece, el paso transfiere mas calor.

Ejemplo:

```text
T_j = 60
T_i = 40
G_hat_ij = 0.03
Delta t = 1
Delta T_ij = (60 - 40) * 0.03 * 1 = 0.6
```

La pieza `i` sube 0.6 unidades por influencia de su vecina. En el proyecto, esto permite que piezas cercanas a fuentes activas muestren una tendencia termica relacionada, no colores aislados sin coherencia espacial.

## Formula 5: acoplamiento geometrico

```text
G_hat_ij = max(0.001, s_ij A_ij / L_ij)
```

Lectura simple:

- mas area de contacto implica mayor acoplamiento;
- mas distancia o camino termico implica menor acoplamiento;
- `s_ij` ajusta la relacion por tipo de enlace;
- el minimo evita que el enlace desaparezca por completo.

Variables:

- `s_ij`: factor de tipo de contacto o relacion funcional entre piezas.
- `A_ij`: area aproximada de contacto o proximidad.
- `L_ij`: distancia efectiva o longitud de camino termico.
- `max(0.001, ...)`: piso minimo para mantener conectividad debil.

Ejemplo:

```text
s_ij = 0.5
A_ij = 0.02
L_ij = 0.10
G_hat_ij = max(0.001, 0.5 * 0.02 / 0.10)
G_hat_ij = 0.1
```

Lectura del proyecto:

Dos piezas con contacto amplio y cercania pueden tener transferencia relativa alta. Dos piezas separadas o con contacto minimo mantienen relacion baja. Esto traduce jerarquia y proximidad del ensamblaje en comportamiento visual termico.

## Como se conecta con el shader

El modelo numerico produce una temperatura por componente. El shader no "calcula fisica"; convierte valores a color:

```text
temperatura_normalizada = saturate((T_actual - T_min) / (T_max - T_min))
color = gradiente(temperatura_normalizada)
```

Variables:

- `T_actual`: valor termico calculado para la pieza.
- `T_min`: limite inferior de la leyenda.
- `T_max`: limite superior de la leyenda.
- `saturate`: limita el valor al rango 0-1.
- `gradiente`: mapa de color, por ejemplo azul -> verde -> amarillo -> rojo.

Ejemplo:

Si `T_min = 25`, `T_max = 80` y `T_actual = 52.5`:

```text
temperatura_normalizada = (52.5 - 25) / (80 - 25) = 0.5
```

El shader usa la mitad del gradiente. Visualmente, eso puede aparecer como tono intermedio, no como zona fria ni pico caliente.

## Terminos importantes

- FEA: analisis por elementos finitos.
- Heuristico: aproximado y orientado a utilidad practica.
- Nodo termico: componente representado como unidad termica.
- Lumped-component model: modelo reducido por componentes.
- Temperatura ambiente: referencia base del entorno.
- Tau: constante de tiempo de calentamiento.
- Conveccion: transferencia de calor hacia el ambiente por fluido/aire.
- Conductancia: facilidad de transferencia termica entre cuerpos.
- Proxy: representacion simplificada de una geometria o contacto.
- Gradiente: cambio gradual de valor, aqui traducido a color.

## Preguntas dificiles de defensa

### Esto es una simulacion real?

Es una simulacion heuristica de visualizacion, no una simulacion fisica calibrada. Es real como herramienta visual del prototipo, no como instrumento experimental de termodinamica.

### Por que usar formulas si no es FEA?

Porque incluso una visualizacion heuristica necesita logica coherente. Las formulas ordenan calentamiento, enfriamiento y transferencia relativa.

### Que valido Wolfram?

El audit matematico reviso que la forma general de las relaciones fuera razonable para una visualizacion: calentamiento exponencial, enfriamiento proporcional y acoplamiento inspirado en area sobre longitud.

## Fuentes relacionadas

- [[Fisica_Termica_Dron]]
- [[MOC_Sistema_Termico_Completo]]
- [[INF_EST_33_Shaders_ViewModes_Entornos]]
- `Informe_final/Desarrollo_App/AUDITORIA_MATEMATICA_Y_ARQUITECTURA_2026-04-12.md`
