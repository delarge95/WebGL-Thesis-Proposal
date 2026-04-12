# Auditoría de Formato y Ortografía de la Propuesta

## Estado de la revisión

Se auditó la propuesta compilada desde `Propuesta/final_proposal.tex` con criterio de:

- integridad visual del PDF
- ortografía y redacción en español de Colombia
- consistencia de portada y resumen
- ubicación correcta de tablas antes de la bibliografía
- limpieza básica de advertencias de compilación

## Correcciones aplicadas

1. **Portada corregida**
- Se corrigió el nombre del asesor a `Gustavo Enrique Vejarano Matiz`.
- Se blindó la portada con acentos escapados en LaTeX para evitar mojibake en compilación.
- Se añadió `fontenc` para mejorar la salida tipográfica en PDF.

2. **Resumen y ficha general corregidos**
- Se corrigieron acentos y caracteres especiales en `Resumen` e `Información General`.
- Se verificó en el PDF recompilado que ya no aparecen cadenas corruptas como `DiseÃ±o`.

3. **Ortografía metodológica corregida**
- Se normalizó la redacción de `Propuesta/sections/metodologia.tex` con tildes y formas correctas de español académico de Colombia.
- Se corrigieron términos como: `análisis`, `investigación`, `definición`, `validación`, `aplicación`, `navegación`, `documentación`, `académico`, `técnico`, `medición`, `dimensión`, `observación`, entre otros.

4. **Tabla fuera de lugar corregida**
- La tabla `Resultados o Productos Esperados` dejaba de comportarse como un float normal y podía terminar después de la bibliografía.
- Se convirtió a `longtable`, permitiendo continuidad en varias páginas dentro de su propia sección.
- Se verificó en el PDF recompilado que la tabla ahora queda en las páginas 40 y 41, y la bibliografía inicia después.

5. **Control de flotantes mejorado**
- Las demás tablas de la propuesta se forzaron con `[H]` para evitar reubicaciones inesperadas.
- Se añadió `\clearpage` antes de la bibliografía como barrera de seguridad.

6. **Paginación y encabezado**
- Se corrigió la duplicación de la página 1 entre portada y resumen.
- Se ajustó `\headheight` para eliminar la advertencia repetitiva de `fancyhdr`.

## Verificaciones realizadas sobre el PDF recompilado

Se confirmó mediante extracción de texto del PDF que:

- la portada muestra correctamente `Diseño`, `Visualización`, `Técnica` y `Análisis`
- el asesor correcto es `Gustavo Enrique Vejarano Matiz`
- el `Resumen` inicia en la página 2
- `Resultados o Productos Esperados` aparece antes de `Referencias Bibliográficas`
- `Referencias Bibliográficas` inicia en la página 42
- las correcciones ortográficas de metodología sí quedaron reflejadas en el PDF

## Hallazgos menores que aún pueden mejorarse

1. **Advertencias `hyperref`**
- Persisten advertencias por anclas de marcadores (`bookmarks`) asociadas a la jerarquía de algunos encabezados en secciones internas.
- No bloquean la compilación ni alteran el contenido visible principal del PDF, pero conviene revisarlas en una pasada posterior de saneamiento fino.

2. **Overfull/Underfull boxes**
- Persisten advertencias menores de ajuste de líneas en algunas tablas y párrafos extensos.
- No generan el problema crítico reportado por el usuario, pero pueden afinarse con una revisión tipográfica adicional.

3. **Lista de tablas**
- La lista de tablas se genera correctamente, pero conviene validar si el formato institucional final realmente exige mantenerla en la propuesta.

## Resultado actual

La propuesta quedó corregida en sus problemas críticos de:

- codificación visible en portada y resumen
- ortografía metodológica
- nombre del asesor
- ubicación incorrecta de tabla después de la bibliografía
- recompilación funcional del PDF
