# Auditoría Primera Pasada del Informe Final
## Fecha: 2026-04-15

## Objetivo

Migrar al informe final las correcciones críticas consolidadas en la propuesta final, preservando la información ya madura del documento y adaptándola con coherencia metodológica, técnica, bibliográfica y terminológica.

## Fuentes canónicas utilizadas

- `Propuesta/final_proposal.tex`
- `Propuesta/sections/planteamiento_problema.tex`
- `Propuesta/sections/justificacion.tex`
- `Propuesta/sections/objetivos.tex`
- `Propuesta/sections/estado_del_arte.tex`
- `Propuesta/sections/marco_teorico.tex`
- `Propuesta/sections/marco_conceptual.tex`
- `Propuesta/sections/metodologia.tex`
- `Propuesta/sections/bibliografia.tex`
- `Propuesta/Audits/*`

## Correcciones integradas al informe final

### 1. Título y narrativa global

- Se sustituyó el título anterior para eliminar la ambigüedad de `análisis estructural`.
- Se reemplazó la categoría promocional `hardware de alto rendimiento` por `hardware complejo` o `hardware técnico complejo`, con definición operativa consistente.
- Se alinearon resumen, abstract, descriptores y datos generales con la propuesta final corregida.

### 2. Marco de referencia

- Se reemplazó el benchmarking cuantitativo especulativo por una comparación cualitativa defendible.
- Se actualizó la justificación de selección de Unity Web para basarla en control del pipeline, tooling, extensibilidad y modos analíticos, no en una promesa simplista de menor peso de build.
- Se incorporó la postura actual sobre compatibilidad web de Unity 6: escritorio y algunos navegadores móviles compatibles, sin prometer compatibilidad universal.
- Se corrigió la atribución de la `rendering equation` a Kajiya (1986).
- Se mantuvo Cook-Torrance como fundamento de BRDF microfacetada.
- Se corrigió la explicación de IL2CPP y su relación con C++ y WebAssembly.

### 3. Metodología

- Se explicitó la separación entre:
  - `Peffers et al. (2007)` como proceso DSRM.
  - `Hevner et al. (2004)` como directrices de rigor y relevancia.
- Se consolidó el enfoque como mixto con predominio cualitativo-formativo.
- Se mantuvo la meta deseable de 30 participantes y el mínimo operativo de 8 a 12, con justificación desde HCI/UX.
- Se consolidó el diseño intra-sujeto 3D vs. 2D con contrabalanceo AB/BA.
- Se reemplazó el uso metodológico de `carga cognitiva` por `carga de trabajo percibida` cuando el dato proviene de NASA-TLX.
- Se reforzó la interpretación de SUS:
  - 68 como promedio histórico.
  - 70--72 como rango favorable.
  - ochentas como valores claramente superiores.
- Se reforzó la explicación de NASA-TLX Raw y Think-Aloud.

### 4. Resultados, conclusiones y apéndices

- El capítulo de resultados quedó reanclado a placeholders coherentes con la metodología corregida.
- `carga cognitiva` fue sustituida por `carga de trabajo percibida` en los contextos de TLX.
- Se corrigieron tablas y apéndices para reflejar el flujo visible real de la build final.
- Se mantuvieron las figuras metodológicas en apéndices.

### 5. Instrumentos activos de validación

- Se reescribió `CUESTIONARIO_NASA_TLX.md` para tratar NASA-TLX como medida de workload percibido, no como medición directa de CLT.
- Se reescribió `CUESTIONARIO_SUS.md` para:
  - corregir interpretación del puntaje;
  - retirar tareas que ya no pertenecen al flujo visible final;
  - alinear el protocolo con la build real.

### 6. Referencias

- Se corrigieron referencias críticas:
  - Bartlett \& Dorribo Camba (DOI correcto).
  - Fransson et al. (venue GEM y DOI correcto).
  - Yu et al. (paginación correcta 379--394).
- Se añadieron referencias faltantes para:
  - Kajiya (1986)
  - Peffers et al. (2007)
  - Lazar et al. (2017)
  - Hart (2006)
  - Bangor (2008)
  - Unity 6 system requirements
  - WebAssembly core specification

## Estado de compilación

Archivo compilado:

- `Informe_final/informe_final.pdf`

Resultado del log:

- `UndefinedRefs = 0`
- `LaTeXWarnings = 0`
- `Overfull = 0`
- `Underfull = 136`

## Lectura del estado final de esta pasada

- Los `Underfull \hbox` restantes son residuales y esperables por la combinación de `\RaggedRight`, tablas multipárrafo, títulos de columnas y placeholders editoriales. No están rompiendo contenido, referencias ni estructura del PDF.
- No se detectaron referencias indefinidas ni warnings generales de LaTeX.
- El informe quedó listo para una auditoría externa fuerte en Deep Research.

## Pendientes para la siguiente ronda

- Auditoría externa profunda con ChatGPT Deep Research o Gemini Deep Research.
- Integración de observaciones de esa auditoría en segunda pasada.
- Revisión posterior del informe con foco en resultados reales una vez exista build congelada y trabajo de campo ejecutado.
