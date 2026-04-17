# PROMPT / ESQUEMA DE COMPORTAMIENTO (LLM-Wiki)

> **Este archivo es la configuración para el LLM (Antigravity). Define cómo debe mantener este cerebro digital.**

## 1. Reglas Core (Para la IA)
- **Cero Duplicación:** Los documentos fuente (`Raw Sources`) ya viven en otras partes del repositorio (`/desarrollo/docs`, `/Informe_final` etc.). Nunca muevas, modifiques ni borres un archivo fuente. Solo léelos y extrae la síntesis para crear páginas de Wiki.
- **Formato y Enlaces:** La Wiki vive en `Cerebro_Digital`. Al crear una página nueva, debes utilizar la notación `[[NombreDeLaPagina]]` para referenciar conceptos interconectados.
- **FrontMatter:** Cada página nueva de conocimiento DEBE llevar YAML frontmatter así:
```yaml
---
tipo: "entidad" # o "concepto"
fuente: "Ruta/Relativa/Al/Archivo/Original"
---
```

## 2. Flujo de Ingesta (Ingest)
Cuando el usuario pida documentar y procesar un nuevo avance, error, script o paper:
1. Genera o actualiza una página `.md` dentro de `Cerebro_Digital/`.
2. Incluye resúmenes, código clave o explicaciones arquitectónicas.
3. Asegura dejar al menos 2 enlaces `[[ ]]` interconectando esto con otras piezas del proyecto (ej: enlazar un bug de Unity 6.3 con la página de WebGL).
4. Agrega obligatoriamente una línea en `Cerebro_Digital/log.md` usando el formato:
`## [YYYY-MM-DD] ingest | Título de lo Ingerido`

## 3. Flujo de Chequeo (Lint)
Ocasionalmente, el usuario te pedirá hacer un *Health-Check*. Tu trabajo será buscar en el `Cerebro_Digital`:
- Archivos que referencian `[[Algo]]` pero el archivo 'Algo.md' no se ha creado.
- Información que ha quedado obsoleta (ej. si reportas que usabas Unity 5 y ahora usamos Unity 6).
