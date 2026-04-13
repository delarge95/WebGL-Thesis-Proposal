# Auditoría de Diagramas y Correlación Propuesta-Informe

Fecha: 2026-04-12  
Alcance: informe final, propuesta, diagramas Mermaid 4.1-4.13 y checklist maestro de material visual.

## 1. Resultado ejecutivo

La correlación entre propuesta e informe final mejoró de forma sustancial, pero todavía requiere una última pasada editorial en dos frentes:

1. cerrar el contraste entre tablas de planeación de la propuesta y tablas de cierre del informe;
2. ajustar varios Mermaid para que la semántica del diagrama coincida mejor con la app real y con el estado funcional documentado.

En términos de cobertura, el paquete visual ya quedó estructurado:

- 13 diagramas Mermaid insertados;
- 13 placeholders editoriales para screenshots/renders del Capítulo 4;
- 7 placeholders editoriales para tablas y gráficas del Capítulo 5;
- tablas adicionales de correlación, artefactos, pipeline e inventario modular.

## 2. Correlación entre propuesta e informe final

### 2.1 Hallazgo general

No todas las tablas de la propuesta deben migrar literalmente al informe final. Algunas eran artefactos prospectivos de planeación y su ausencia en el documento final es válida si queda justificada.

### 2.2 Estado por tabla

| Tabla de la propuesta | Estado en el informe final | Evaluación |
|---|---|---|
| Comparativa de soluciones Web 3D | Conservada en Cap. 2 | Coherente |
| Cronograma general del proyecto | No migrada; justificada como artefacto de planeación | Coherente si se mantiene la justificación |
| Presupuesto estimado de recursos | No migrada; justificada como artefacto ex ante | Coherente si se mantiene la justificación |
| Resultados o productos esperados | Transformada en productos entregados vs. esperados | Coherente y recomendable |

### 2.3 Riesgo residual

Si se elimina la tabla de correlación propuesta-informe o la de productos entregados vs. esperados, el lector puede interpretar las omisiones como pérdida de coherencia en lugar de evolución metodológica justificada.

## 3. Auditoría de diagramas Mermaid

## 3.1 Hallazgos positivos

- Todos los `.mmd` comparten un mismo bloque `%%{init}%%`, diez `classDef` semánticos y una convención visual estable.
- La cobertura temática es sólida: arquitectura, eventos, estados, selección, shaders, datos, térmico, restricciones WebGL, módulos proyectados y despliegue.
- Las Figuras 4.11-4.13 ya no contaminan el Capítulo 4 como si fueran resultados observados; quedaron correctamente movidas a apéndices.

## 3.2 Hallazgos de precisión y veracidad

### DGM-01 — `fig_4_1` mezcla nodos válidos con un nodo no canónico

El diagrama de arquitectura usa `InputRouter`, pero en la base real existen `InputManager` y `OrbitCameraController`; no se detectó una clase `InputRouter` en `Assets/Scripts`.

Impacto:

- reduce la trazabilidad código-diagrama;
- puede hacer parecer que existe una capa formal de enrutamiento de input que hoy no está materializada con ese nombre.

Corrección recomendada:

- reemplazar `InputRouter` por `InputManager + OrbitCameraController`, o
- relabel como `Input Layer` si se desea una abstracción y aclararlo en el caption.

### DGM-02 — `fig_4_1` sobrepresenta audio como parte del flujo central

`AudioManager` existe en código, pero el cierre documental actual reconoce que no hay assets de audio integrados y que el audio puede quedar como trabajo futuro.

Impacto:

- el diagrama sugiere un subsistema activo y consolidado;
- puede contradecir la narrativa de alcance real del cierre.

Corrección recomendada:

- mover `AudioManager` a una clase visual de capacidad opcional o trabajo futuro;
- o marcarlo explícitamente como servicio presente en código pero no validado como feature final publicada.

### DGM-03 — `fig_4_4` mezcla flujo visible con paneles legacy

El diagrama de selección envía la reacción del sistema a `UIManager + EnhancedInfoPanel + DetailsPanelController`. Sin embargo, la narrativa principal del informe ya fija como salida visible el `bottom sheet`, mientras que `EnhancedInfoPanel` se considera parcial o legacy.

Impacto:

- mezcla runtime visible con legado;
- puede confundir sobre cuál es la interfaz realmente publicada.

Corrección recomendada:

- relabel la salida principal como `UIDetailsSheet (bottom sheet)`;
- dejar `EnhancedInfoPanel` solo si se marca explícitamente como legado o compatibilidad histórica.

### DGM-04 — `fig_4_5` necesita distinguir mejor modo implementado vs. modo expuesto

El pipeline de shaders presenta los siete modos de `ViewModeManager`, pero la UI final no publica todos ellos de manera simultánea.

Impacto:

- el diagrama es técnicamente correcto a nivel de implementación;
- pero narrativamente puede leerse como si todos los modos estuvieran visibles para el usuario final.

Corrección recomendada:

- añadir una nota o leyenda visual: `modos implementados` vs. `modos expuestos en UI final`;
- o cambiar el caption a `Pipeline de modos implementados y su publicación parcial en UI`.

### DGM-05 — `fig_4_9` es correcto, pero necesita una leyenda editorial más fuerte

El diagrama de ensamblaje proyectado ya distingue estados, pero sería útil reforzar visualmente que no forma parte del flujo final visible.

Corrección recomendada:

- añadir en el caption la frase `capacidad proyectada o no integrada`;
- o incorporar una clase visual más contrastante para `proyectado`.

## 3.3 Hallazgos de claridad y estética

### DGM-06 — Estética coherente, pero potencialmente disonante con el estilo APA

Los diagramas usan fondo oscuro, texto claro y acentos cromáticos consistentes. En pantalla funcionan bien, pero pueden chocar con una tesis en fondo blanco, tipografía Times y convenciones APA/UNAD.

Impacto:

- mayor peso visual y consumo de tinta en impresión;
- sensación de sistema visual separado del resto del documento.

Corrección recomendada:

- rerenderizar a una variante clara para entrega final, manteniendo la semántica de clases;
- o conservar fondo oscuro solo si la legibilidad impresa fue revisada y aprobada.

### DGM-07 — Densidad alta en `fig_4_4`

Con 88 líneas de Mermaid y cuatro subgrupos, el diagrama de selección es el más denso del set.

Impacto:

- riesgo de pérdida de legibilidad a tamaño de tesis;
- exceso de detalle si se imprime o se consulta rápido durante sustentación.

Corrección recomendada:

- simplificar verbos y etiquetas largas;
- o dividirlo en flujo principal y reacciones del sistema.

## 4. Estado actual de la fuente Mermaid

Tras esta auditoría, la fuente `.mmd` quedó ajustada en dos niveles:

1. se preparó una paleta clara más coherente con la impresión del informe y con el estilo general del documento;
2. se corrigieron los hallazgos semánticos principales en `fig_4_1`, `fig_4_4`, `fig_4_5` y `fig_4_9`.

En consecuencia, el pendiente principal ya no es redefinir el contenido del diagrama, sino rerenderizar los PDFs y verificar su legibilidad final dentro del documento compilado.

## 5. Limitaciones de esta auditoría

- No se dispuso de CLI Mermaid local para rerenderizar variantes claras durante esta iteración.
- La evaluación estética se apoyó en la fuente `.mmd`, la consistencia del sistema visual y la integración documental, no en una inspección rasterizada página por página del PDF exportado.

## 6. Plan de corrección recomendado

### P0 — Precisión semántica

1. Rerenderizar `fig_4_1` con `InputManager + OrbitCameraController` y audio marcado como opcional.
2. Rerenderizar `fig_4_4` priorizando `UIDetailsSheet (bottom sheet)` sobre paneles legacy.
3. Rerenderizar `fig_4_5` con la distinción entre modos implementados y modos expuestos.
4. Rerenderizar `fig_4_9` reforzando la condición de capacidades no publicadas en la UI final.

### P1 — Legibilidad de entrega

1. Verificar la versión clara de los Mermaid ya preparada en la fuente.
2. Revisar `fig_4_4` a tamaño real de tesis y simplificar texto si se satura.
3. Verificar que `fig_4_9` haga explícita la condición de proyectado/no integrado.

### P2 — Cierre visual del informe

1. Reemplazar los 13 placeholders de screenshots por capturas reales.
2. Diligenciar las 5 tablas y 2 gráficas del Capítulo 5 con datos reales.
3. Ejecutar una pasada final de numeración, captions y listas.

## 7. Veredicto

Los diagramas ya son utilizables y están suficientemente estructurados para sostener la arquitectura del informe. La fuente Mermaid quedó corregida y alineada con la app real en sus hallazgos principales; el cierre pendiente ahora es operativo y visual: rerenderizar, reinsertar los PDFs exportados y validar su legibilidad final dentro del documento compilado.
