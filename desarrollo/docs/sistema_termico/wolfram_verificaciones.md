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

- [ ] Tiempo de warmup por componente (tau): motor 10s, ESC 6s, battery 15s — comparar con datos de termografia disponibles
- [ ] Tasa de enfriamiento convectivo por exposicion: ¿son los valores (0.05–0.16) plausibles para conveccion natural en aire quieto?
- [ ] Temperatura de pico de motores brushless (~90°C) — confirmar contra datasheets de motores 2216