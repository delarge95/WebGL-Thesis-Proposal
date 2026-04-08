# Verificaciones Wolfram — Sistema Termico

## Protocolo

Toda ecuacion, constante o conversion de unidades se verifica con el workflow `wolfram-thermal-verifier` antes de congelarse en codigo o documentacion.

Cuando no hay `WOLFRAM_APP_ID` disponible, se usa el modo manual (URL de website) junto con fuentes de ingenieria validadas.

---

## V001 — Factor geometrico de conduccion

- **Fecha**: 2026-03-12
- **Expresion**: `G = k * A / L`
- **Contexto**: Formula base de conduccion usada en `EstimateConductanceFromGraph()` linea 710-720 de `ThermalSimulationManager.cs`
- **Wolfram URL**: `https://www.wolframalpha.com/input?i=0.2+W%2F%28m+K%29+*+4+cm%5E2+%2F+2+mm`
- **Estado**: Verificado — la formula es Fourier standard para conduccion unidimensional estacionaria

---

## V002 — Escalas de conductividad por material

- **Fecha**: 2026-03-12
- **Contexto**: `EstimateMaterialConductivityScale()` lineas 664-708 de `ThermalSimulationManager.cs`
- **Proposito**: Validar que las escalas heuristicas del solver guarden proporcion con la conductividad termica real de cada material

### Datos de referencia (fuentes de ingenieria)

| Material | k real W/(m·K) | Rango | Fuente |
|----------|---------------|-------|--------|
| Cobre | 380–401 | bien establecido | Vedantu, Wikipedia, bestpcbs.com |
| Aluminio 6061 | 152–180 | ~167 a 20°C | zhonghaoaluminum.com, makeitfrom.com |
| Acero 304 | 16.2–21.5 | ~16.2 a 20°C | azom.com, unifiedalloys.com |
| Fibra de carbono (CFRP) | 2–3 (en fibra), 0.5–0.6 (through-thickness) | altamente anisotropico | ResearchGate, dragonplate.com |
| FR4 PCB | 0.25–0.35 (XY), 0.10–0.20 (Z) | anisotropico | allpcb.com, raypcb.com |
| LiPo (celda) | 0.33–0.66 (cross-plane) | celda pouch | ResearchGate |
| Nylon 6/66 | 0.2–0.4 | ~0.28 a 23°C | theplasticshop.co.uk, smithmetal.com |

### Wolfram URLs generadas

```
wolfram_verify.py "thermal conductivity of copper in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of aluminum 6061 in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of steel 304 in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of carbon fiber in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of FR4 PCB in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of lithium polymer battery cell in W/(m K)" --endpoint website
wolfram_verify.py "thermal conductivity of nylon 6 in W/(m K)" --endpoint website
```

### Ratios reales normalizados a acero 304 (k=16.2)

| Material | k ref. | Ratio real vs acero | Ratio en solver | Comentario |
|----------|--------|--------------------:|----------------:|------------|
| Cobre | 390 | 24.1 | 1.8 | Comprimido deliberadamente |
| Aluminio | 167 | 10.3 | 1.4 | Comprimido deliberadamente |
| Acero | 16.2 | 1.0 | 1.0 | Referencia — coincide |
| Fibra carbono | 2.5 | 0.15 | 0.65 | Sobreestimado en solver |
| FR4 PCB | 0.30 | 0.019 | 0.40 | Significativamente sobreestimado |
| LiPo | 0.50 | 0.031 | 0.18 | Sobreestimado en solver |
| Nylon | 0.28 | 0.017 | 0.20 | Sobreestimado en solver |

### Analisis

Las escalas del solver usan un rango comprimido deliberado (0.18–1.8) en lugar del rango real (0.017–24.1). Esto es una decision de diseño documentada: comprimir el rango evita que piezas metalicas dominen completamente la conduccion visual mientras piezas aislantes muestren gradientes imperceptibles.

**Justificacion**: En el rango real, una pieza de cobre tendria 1400× mas conduccion que una de nylon. En WebGL movil a 23 FPS, eso causaria:
1. Partes metalicas que alcanzan equilibrio en 1 frame (visualmente instantaneo)
2. Partes aislantes que nunca muestran cambio visible durante una sesion

**Conclusion**: Las escalas son heuristicas deliberadas, no errores. Deben etiquetarse asi en el codigo. El rango comprimido prioriza credibilidad visual sobre exactitud numerica absoluta, lo cual es consistente con el alcance declarado de la V1.

**Accion**: Agregar comentario en `EstimateMaterialConductivityScale()` documentando que los valores son escalas comprimidas, no conductividad real.

---

## Proximas verificaciones pendientes

- [x] Tiempo de warmup por componente (tau): motor 10s, ESC 6s, battery 15s — comparar con datos de termografia disponibles (VER V003)
- [x] Tasa de enfriamiento convectivo por exposicion: ¿son los valores (0.05–0.16) plausibles para conveccion natural en aire quieto? (VER V004)
- [ ] Temperatura de pico de motores brushless (~90°C) — confirmar contra datasheets de motores 2216
---

## V003  Constante de tiempo termico (tau) de un motor brushless

- **Fecha**: 2026-03-17
- **Contexto**: 
ode.WarmupSeconds en ThermalSimulationManager.cs y los valores en x500v2_blender_synced_parts.json (ej: motor tau = 10s)
- **Proposito**: Verificar si el tiempo de calentamiento del motor (10s) es fisicamente realista o una heuristica visual.

### Calculo Real 
Un motor 2216 pesa ~65g, mayormente aluminio y cobre ( \approx 600$ J/kgK). Area superficial  \approx 0.0038$ m. Conveccion mixta con flujo del rotor  \approx 25$ W/mK.
- Wolfram query: (0.065 kg * 600 J/(kg K)) / (25 W/(m^2 K) * 0.0038 m^2)
- Real $\tau \approx 410$ segundos (casi 7 minutos para alcanzar equilibrio).

### Analisis
Si usaramos el $\tau$ real de 410s, el usuario tendria que observar la simulacion interactiva durante 7 minutos para ver los colores estabilizarse. Para propósitos educativos e interactivos en una app de WebGL, eso es inaceptable.
El valor de WarmupSeconds = 10 significa que el solver está intencionalmente acelerado ~40x respecto a la realidad para efectos de visualizacion inmediata.

**Conclusion**: Es una escala heuristica intencional para experiencia de usuario (UX).

---

## V004  Tasa de enfriamiento convectivo (Cooling Rate)

- **Fecha**: 2026-03-17
- **Contexto**: defaultCoolingRate = 0.08f en ThermalSimulationManager.cs
- **Proposito**: Verificar la proporcion respecto a la ley de enfriamiento de Newton.

### Analisis
La ley de Newton establece /dt = -k(T - T_{env})$, donde  = 1/\tau$.
Si el solver usa  = 0.08$, el $\tau$ equivalente de enfriamiento es /0.08 = 12.5$ segundos.
Nuevamente, esto esta perfectamente alienado con la decision documentada en V003 de comprimir el tiempo $\tau$ a la escala de ~10-15 segundos para la visualizacion interactiva en tiempo real.

**Conclusion**: Matematicamente coherente con la escala de tiempo acelerada del sistema.

---

## V005 - Grafo canonico de contactos y factor geometrico

- **Fecha**: 2026-03-19
- **Contexto**: `ThermalCanonicalContactGraph.asset`
- **Proposito**: Dejar trazado que el asset canonico congela proxies geometricos de `A_contacto` y `L_efectiva` para la V1, no mediciones CAD metrologicas finales.

### Regla usada

El asset sigue la misma aproximacion documentada en V001:

```text
G_ij ~= k_eff * A_contacto / L_efectiva
```

### Decision documentada

- `contactAreaCm2` y `pathLengthMm` del asset canonico son valores proxy orientados a una simulacion cualitativa avanzada.
- El objetivo del asset es reemplazar el exito aparente del fallback heuristico por una red explicita y editable.
- La calibracion final contra geometria retopologizada sigue pendiente.

### Conclusion

El asset canonico es fisicamente coherente a nivel dimensional y metodologico, pero todavia no debe presentarse como calibracion geometrica final del X500 V2.