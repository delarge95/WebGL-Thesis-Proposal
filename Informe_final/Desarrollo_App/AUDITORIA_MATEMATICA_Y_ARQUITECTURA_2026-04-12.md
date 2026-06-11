# Auditoría matemática y de arquitectura

## Fecha
2026-04-12

## Objetivo
Verificar que las fórmulas, la lógica operativa y la arquitectura descritas en la propuesta y en el informe final sean coherentes con la aplicación real documentada en `MainScene_Final` y con el comportamiento del código runtime.

## Fuentes canónicas auditadas
- `Propuesta/sections/metodologia.tex`
- `Propuesta/sections/marco_teorico.tex`
- `Informe_final/chapters/02_marco_referencia.tex`
- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/03_Pipeline_Renderizado_Shaders.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalSimulationManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalViewController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalContactGraphAsset.cs`
- `desarrollo/unity_project/Assets/Scripts/Editor/Thermal/ThermalContactGraphBuilderWindow.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/DroneStateController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/CrossSectionManager.cs`
- `desarrollo/unity_project/Assets/Shaders/ClippableLit.shader`
- `desarrollo/unity_project/Assets/Shaders/XRay.shader`
- `desarrollo/unity_project/Assets/Shaders/Blueprint.shader`
- `desarrollo/unity_project/Assets/Shaders/Thermal.shader`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`
- registros históricos internos de verificación térmica, no publicados en la superficie pública del repositorio

## Workflow de verificación con Wolfram
Se reutilizó el verificador local del proyecto:

- Script: `.agent/skills/wolfram-thermal-verifier/scripts/wolfram_verify.py`
- Modo usado: `website`
- Motivo: no hay `WOLFRAM_APP_ID` configurado en el entorno actual, por lo que la verificación se trazó con URLs reproducibles y contraste manual.

### Consultas registradas
- Conducción base:
  - Query: `0.2 W/(m K) * 4 cm^2 / 2 mm`
  - URL: <https://www.wolframalpha.com/input?i=0.2+W%2F%28m+K%29+%2A+4+cm%5E2+%2F+2+mm>
  - Cálculo local equivalente: `0.04`
- Constante de tiempo térmica de referencia:
  - Query: `(0.065 kg * 600 J/(kg K)) / (25 W/(m^2 K) * 0.0038 m^2)`
  - URL: <https://www.wolframalpha.com/input?i=%280.065+kg+%2A+600+J%2F%28kg+K%29%29+%2F+%2825+W%2F%28m%5E2+K%29+%2A+0.0038+m%5E2%29>
  - Cálculo local equivalente: `410.526315789474 s`
- Escala temporal de enfriamiento del solver:
  - Query: `1 / (0.08 1/s)`
  - URL: <https://www.wolframalpha.com/input?i=1+%2F+%280.08+1%2Fs%29>
  - Cálculo local equivalente: `12.5 s`

## Hallazgos principales

### 1. SUS
Estado: `coherente`

La formulación correcta del SUS es:

```text
SUS = (sumatoria de contribuciones ajustadas de 10 ítems) * 2.5
```

con las reglas:

- ítems impares: `R_j - 1`
- ítems pares: `5 - R_j`

La propuesta y el informe final quedaron corregidos para explicitar este procedimiento, en lugar de limitarse a una referencia abreviada de “ajustar ítems y multiplicar por 2.5”.

### 2. NASA-TLX Raw
Estado: `coherente`

La fórmula documentada:

```text
NASA-TLX_Raw = (1/6) * sum D_i
```

es correcta para la variante `Raw TLX`, siempre que:

- `D_i` represente el puntaje observado en cada una de las seis dimensiones;
- y la dimensión de rendimiento esté orientada para que un valor alto siga significando mayor carga.

En el cuestionario activo del proyecto, `Rendimiento` se diligencia como `0 = perfecto` y `100 = fallido`, por lo que puede promediarse directamente con las otras cinco dimensiones sin transformación adicional.

### 3. Think-Aloud
Estado: `coherente y mejor especificado`

La propuesta y el informe final ahora describen el protocolo como verbalización concurrente durante tareas representativas, con codificación posterior de:

- comprensión espacial;
- navegación y control;
- interpretación de interfaz;
- correspondencia terminológica;
- errores o bloqueos;
- sugerencias de mejora.

Esto alinea el instrumento cualitativo con la ejecución real esperada de las pruebas.

### 4. Optimización gráfica
Estado: `coherente con correcciones discursivas`

#### 4.1 Presupuesto poligonal

La relación:

```text
P_total = sum P_i
```

es correcta como presupuesto geométrico operativo. Debe leerse como criterio de control del proyecto, no como ley del motor.

#### 4.2 Densidad de texel

La formulación antigua `R_texture / A_UV` con unidades `px/cm` era dimensionalmente inconsistente. La documentación quedó corregida a una forma operativa del tipo:

```text
rho = R_texture / L_mundo
```

cuando se usa una longitud de referencia. Esto es fiel a la intención del pipeline: uniformidad visual y no una magnitud ejecutada por la app en runtime.

#### 4.3 Draw calls e instancing

La fórmula antigua:

```text
D_optimizado = D_original / N_instances
```

era demasiado fuerte y se corrigió. La app real no permite sostener una división exacta universal. La forma coherente es:

```text
D_base ~= sum n_k
D_efectivo ~= sum b_k
```

dependiendo de compatibilidad entre mallas, materiales y pasadas.

#### 4.4 Frame time

La aproximación rígida `T_frame = T_CPU + T_GPU` se corrigió a una formulación operativa más fiel:

```text
T_frame ~= max(T_CPU, T_GPU)
```

con posible costo adicional de sincronización. Esto coincide mejor con el comportamiento de pipelines gráficos modernos.

### 5. PBR y renderizado físicamente basado
Estado: `coherente con la app real si se documenta como fundamento de URP`

#### 5.1 Ecuaciones teóricas válidas

Las ecuaciones de:

- ecuación de render;
- BRDF especular de Cook-Torrance;
- Fresnel de Schlick;
- distribución GGX;
- conservación de energía en flujo `metallic-roughness`

son válidas como fundamento matemático del modelo PBR usado por la aplicación.

#### 5.2 Límite importante

La app final **no** implementa desde cero una BRDF microfacetada completa en un shader propio. El modo `Realistic` usa `ClippableLit.shader`, que construye `SurfaceData` y delega el cálculo base a:

```text
UniversalFragmentPBR(inputData, surfaceData)
```

Conclusión:

- sí es correcto explicar Cook-Torrance, Schlick y GGX;
- no es correcto afirmar que el proyecto escribió manualmente toda la BRDF desde cero;
- sí es correcto afirmar que el aporte propio fue integrar el flujo PBR de URP con clipping global y modos analíticos complementarios.

### 6. Matemática de shaders realmente implementados
Estado: `coherente`

#### 6.1 Corte transversal

`CrossSectionManager` construye el plano como:

```text
plane = (n_x, n_y, n_z, -dot(n, p))
```

y los shaders aplican:

```text
dot(worldPos, plane.xyz) + plane.w < 0
```

Esto coincide con la forma estándar `n·x + d = 0`.

#### 6.2 X-Ray y Blueprint

Los modos técnicos usan una intensidad de borde basada en:

```text
fresnel = pow(1 - saturate(dot(normalWS, viewDirWS)), power)
```

Por tanto, es correcto documentar estos modos como visualizaciones analíticas apoyadas en realce de contorno dependiente de normal y vista.

#### 6.3 Thermal shader

El shader térmico no resuelve la física completa; visualiza una temperatura normalizada combinando:

- temperatura base suministrada por el subsistema térmico;
- factor espacial radial/axial;
- enfriamiento de borde;
- ruido procedural;
- microvariación por textura.

Por ello, es correcto documentarlo como capa de visualización térmica y no como solver termodinámico completo.

### 7. Solver térmico runtime
Estado: `coherente con la app, heurístico y no FEA`

La implementación real en `ThermalSimulationManager.cs` responde a un modelo reducido por componentes y no a una simulación física de alta fidelidad.

#### 7.1 Equilibrio por carga

La temperatura de equilibrio depende del estado operativo y de `SystemLoadFactor`, expuesto por `DroneStateController`.

- Si la pieza no es fuente de calor, el equilibrio es `T_amb`.
- Si la pieza sí es fuente, la temperatura objetivo se interpola entre ambiente, hover y pico.

#### 7.2 Calentamiento temporal

La actualización de fuente usa:

```text
sourceBlend = 1 - exp(-dt / tau)
sourceDelta = (T_eq - T_actual) * sourceBlend * sourceWeight
```

Esto coincide con una aproximación exponencial de primer orden, adecuada para un solver interactivo.

#### 7.3 Enfriamiento

La disipación ambiental implementada es:

```text
coolingDelta = (T_amb - T_actual) * coolingRate * exposure * dt
```

La verificación `1 / 0.08 = 12.5 s` confirma que la escala temporal por defecto del enfriamiento es deliberadamente acelerada y consistente con una política interactiva.

#### 7.4 Acoplamiento entre piezas

La app usa dos niveles de acoplamiento:

- fallback manual con enlaces predefinidos;
- grafo térmico autoritativo con `contactAreaCm2`, `pathLengthMm` y `conductionScale`.

La función runtime usa una relación del tipo:

```text
G_hat_ij = max(0.001, s_ij * A_ij / L_ij)
```

Conclusión:

- sí comparte la lógica estructural de una relación tipo Fourier `k * A / L`;
- no debe presentarse como conductancia física calibrada en SI;
- debe describirse como coeficiente heurístico o proxy geométrico de conducción.

### 8. Arquitectura real del modo térmico
Estado: `coherente`

La cadena real es:

1. `DroneStateController`
2. `ThermalSimulationManager`
3. `ThermalViewController`
4. `Thermal.shader` y UI de leyenda

Esa arquitectura coincide con la app real y con la documentación técnica actualizada.

## Correcciones que debían reflejarse en la tesis
- Explicar SUS con el ajuste de ítems impares y pares.
- Explicar que `D_i` en NASA-TLX es el puntaje de cada dimensión.
- Documentar Think-Aloud como protocolo concurrente con categorías analíticas.
- Corregir la fórmula de draw calls para no presentarla como división exacta.
- Corregir la formulación de densidad de texel para que sea dimensionalmente consistente.
- Presentar el PBR como fundamento del flujo de URP, no como BRDF escrita íntegramente desde cero.
- Presentar el sistema térmico como simulación híbrida heurística, no como FEA.
- Separar explícitamente matemática de solver y matemática de visualización en el modo térmico.

## Veredicto
Sí existe coherencia y consistencia entre la tecnología usada por la app real y los fundamentos matemáticos documentados, **siempre que** se mantengan estas restricciones discursivas:

- la optimización gráfica se describa como presupuesto y criterio operativo, no como promesa de que todas las técnicas teóricas están activas en la build final;
- el PBR se presente como fundamento matemático del flujo URP empleado por la app;
- los shaders analíticos se expliquen con la matemática realmente implementada (planos de corte, Fresnel de borde y mapeo visual térmico);
- el sistema térmico se describa como híbrido, heurístico y orientado a visualización interactiva.

Con esas precisiones, la propuesta, el informe final y la aplicación comparten lógica, arquitectura y semántica matemática de forma consistente.
