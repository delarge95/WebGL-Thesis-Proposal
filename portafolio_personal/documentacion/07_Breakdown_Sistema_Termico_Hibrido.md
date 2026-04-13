# Breakdown: Sistema Térmico Híbrido

## Posicionamiento correcto

Esta pieza debe presentarse como `visualización técnica aplicada`, no como FEA, CFD ni termografía experimental cerrada.

## Valor profesional de la pieza

El sistema térmico es fuerte para portafolio porque conecta:

- lógica de estado del dron;
- simulación heurística por componentes;
- shader de visualización;
- leyenda visible en UI;
- tooling de autoría y verificación.

Eso es exactamente el tipo de integración que sí vende un perfil de Technical Artist orientado a visualización técnica.

## Arquitectura real

La cadena principal que conviene mostrar es:

```text
DroneStateController
-> ThermalSimulationManager
-> ThermalViewController
-> Thermal.shader + leyenda UI
```

Como soporte, también puede mostrarse:

- `ThermalContactGraphAsset`
- `ThermalContactGraphBuilderWindow`

## Qué se puede afirmar con seguridad

- el sistema usa un modelo reducido por componentes;
- depende del estado operativo del dron;
- resuelve temperaturas por nodo y las traduce a una lectura visual útil;
- el shader térmico actúa como capa de visualización, no como solver completo;
- existe tooling para construir o revisar el grafo térmico.

## Qué no debe afirmarse

- FEA en tiempo real;
- simulación física calibrada en unidades completas;
- validación experimental cerrada;
- termografía equivalente a un sistema FLIR real;
- telemetría IoT live si no forma parte de la build final.

## Matemática segura para mencionar

### Calentamiento de primer orden

Es válido explicar que la aproximación temporal sigue una lógica exponencial del tipo:

```text
sourceBlend = 1 - exp(-dt / tau)
sourceDelta = (T_eq - T_actual) * sourceBlend * sourceWeight
```

### Enfriamiento simplificado

También es válido hablar de una disipación proporcional a diferencia de temperatura, exposición y tasa de enfriamiento:

```text
coolingDelta = (T_amb - T_actual) * coolingRate * exposure * dt
```

### Acoplamiento heurístico entre piezas

Es correcto mencionar un acoplamiento proxy inspirado en una relación tipo `A / L`, por ejemplo:

```text
G_hat_ij = max(0.001, s_ij * A_ij / L_ij)
```

La forma correcta de explicarlo es:

- comparte intuición con conducción tipo Fourier;
- no es una conductancia física calibrada en SI;
- fue diseñada para legibilidad interactiva y coherencia con WebGL.

## Qué mostrar en el breakdown

1. Problema: cómo visualizar comportamiento térmico sin romper rendimiento ni sobreprometer física.
2. Arquitectura real del sistema.
3. Lógica matemática segura.
4. Traducción visual a shader y leyenda.
5. Tooling de soporte.
6. Límites honestos del sistema.

## Evidencia recomendada

- `ThermalSimulationManager.cs`
- `ThermalViewController.cs`
- `Thermal.shader`
- `ThermalContactGraphBuilderWindow.cs`
- `AUDITORIA_MATEMATICA_Y_ARQUITECTURA_2026-04-12.md`
- `wolfram_verificaciones.md`
- capturas del modo térmico con leyenda

## Mensaje técnico central

> El valor de este sistema no está en prometer física de alta fidelidad, sino en traducir estado operativo y relaciones térmicas del ensamblaje a una visualización clara, interactiva y coherente con las restricciones reales de Unity WebGL.

## Cierre recomendado para entrevistas

Si te preguntan por el grado de “realismo” del sistema, la respuesta correcta es:

- es una simulación híbrida heurística;
- está pensada para visualización técnica interactiva;
- está bien acoplada a la arquitectura real del visor;
- y su fortaleza principal está en la integración entre datos, shader, UI y tooling.
