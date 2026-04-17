<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# Herramientas Esenciales para tu Proyecto LLM-Wiki en Obsidian

Basándome en tu metodología LLM-Wiki de Karpathy y tu perfil técnico (desarrollo 3D, Unity, WebGL, tesis académica), aquí está la guía completa para maximizar tu "Cerebro Digital" en Obsidian.[^1]

## Plugins Fundamentales para LLM-Wiki

### Dataview - El Motor de tu Cerebro Digital

**Por qué es crítico:** Dataview transforma Obsidian en una base de datos consultable, permitiendo crear MOCs (Maps of Content) dinámicos que se auto-actualizan. En lugar de mantener manualmente listas de enlaces, Dataview escanea tu vault y genera listas automáticas basadas en propiedades, tags o backlinks.[^2]

**Casos de uso para tu proyecto:**

- Crear índices automáticos que agrupan archivos huérfanos por carpeta o tipo
- Generar vistas de documentación técnica filtradas por estado (`activo` vs `archivado`)
- Construir dashboards que muestren el progreso de tu tesis
- Listar automáticamente todos los archivos que referencian un concepto específico

**Query ejemplo para agrupar huérfanos:**

```dataview
LIST
FROM "tu_carpeta"
WHERE !outgoing([[]])
GROUP BY folder
```

### Templater - Automatización Avanzada

**Por qué lo necesitas:** Templater te permite crear scripts en JavaScript que manipulan archivos, ejecutan lógica compleja y automatizan tareas repetitivas. Es más poderoso que el sistema de templates nativo de Obsidian.[^3][^4]

**Aplicaciones directas para tu workflow:**

- Auto-inicializar YAML frontmatter en archivos nuevos del `Cerebro_Digital`
- Crear MOCs con estructura predefinida basada en la carpeta donde se ejecuta
- Generar automáticamente enlaces bidireccionales al crear nodos conceptuales
- Ejecutar scripts que escanean y reportan archivos desconectados

**Template ejemplo para inicializar nodos:**

````javascript
---
tipo: "<% tp.system.prompt("Tipo (concepto/moc/entidad)") %>"
fuente: "<% tp.file.folder(true) %>/<% tp.file.title %>"
estado: "activo"
creado: <% tp.date.now("YYYY-MM-DD HH:mm") %>
---

# <% tp.file.title %>

## Conexiones
```dataview
LIST
FROM [<% tp.file.title %>]
````

```

### Breadcrumbs - Jerarquías Semánticas
**Perfecto para tu arquitectura:** Breadcrumbs permite definir relaciones jerárquicas explícitas entre notas (parent/child/sibling). Esto complementa la metodología LLM-Wiki al crear estructuras navegables más allá del grafo plano.[^5][^6]

**Uso recomendado:**
- Establecer jerarquías claras: `Cerebro_Digital/index.md` → `MOC_WebGL` → `nota_específica_shader`
- Visualizar el "árbol genealógico" de conceptos relacionados
- Navegar rápidamente entre niveles de abstracción

## Visualización y Navegación Avanzada

### Obsidian Canvas - Pensamiento Espacial
**Ideal para arquitectura de sistemas:** Canvas te permite crear tableros visuales donde organizas notas, imágenes y conexiones en 2D. Perfecto para mapear la arquitectura de tu aplicación Unity o diseñar la estructura de tu tesis.[^7][^8]

**Casos de uso técnicos:**
- Diagramar la arquitectura de tu proyecto WebGL (componentes, flujo de datos)
- Crear mockups de UI conectados a notas de especificación
- Planificar la estructura de capítulos de tesis con conexiones visuales
- Mapear dependencias entre sistemas (CAD → Unity → WebGL)

### Excalidraw - Diagramas Técnicos
**Herramienta esencial para ingenieros:** Excalidraw integra un editor de diagramas completo dentro de Obsidian. Puedes dibujar arquitecturas, flowcharts, diagramas UML y embedirlos directamente en tus notas con sintaxis `[[diagrama.excalidraw]]`.[^9][^10]

**Ventajas sobre herramientas externas:**
- Los diagramas viven en tu vault (búsquedas, backlinks)
- Puedes enlazar elementos del diagrama a notas específicas
- Exportación automática a PNG para documentación
- Perfecto para documentar pipelines 3D y shader graphs

### Juggl - Grafo Interactivo Mejorado
**Alternativa al grafo nativo:** Juggl ofrece un grafo local interactivo con estilos personalizables, layouts dinámicos y soporte para "tipos de enlace". Puedes expandir selectivamente partes del grafo sin sobrecargarlo.[^11]

**Características únicas:**
- 4 layouts diferentes (fuerza, circular, jerárquico, grid)
- Code fence para embedir vistas de grafo en notas
- Compatible con mobile (útil para revisiones)
- Extensible por otros plugins

### InfraNodus AI Graph View
**Análisis semántico del grafo:** Este plugin transforma tu vault en un knowledge graph con métricas de centralidad, modularidad e influencia. Usa clustering y ciencia de redes para identificar gaps y generar preguntas de investigación.[^12]

**Aplicación para tu tesis:**
- Identificar conceptos clave que necesitan más desarrollo
- Detectar "puentes" conceptuales entre áreas desconectadas
- Visualizar clusters temáticos (3D, AI, WebGL)
- Generar preguntas de investigación basadas en gaps

## Integración con IA Local

### Local GPT - Máxima Privacidad
**IA sin salir de tu máquina:** Local GPT se conecta a Ollama o LM Studio para usar LLMs localmente. Puedes seleccionar texto y ejecutar acciones AI con contexto de enlaces, backlinks y PDFs.[^13][^14]

**Configuración recomendada:**
- Ollama con modelos Gemma 2 (12B) o LLaMA 3.1 (8B)
- Text embedding model: `nomic-embed-text` para búsquedas semánticas
- Custom commands: "Resumir", "Traducir", "Extraer conceptos clave"

### Smart Connections - Búsqueda Semántica
**Encuentra notas por significado, no por palabras:** Smart Connections usa embeddings locales para buscar notas relacionadas conceptualmente, incluso si usas vocabulario diferente.[^15][^16]

**Ventajas críticas:**
- Búsqueda por "fuzzy recall" (recuerdas la idea, no las palabras exactas)
- Encuentra conexiones ocultas entre documentación técnica
- "Smart Lookup" para preguntas en lenguaje natural
- Visualización de clusters semánticos

**Cuándo usar qué:**
- **Obsidian search nativo:** Buscar términos exactos, tags, regex
- **Smart Connections:** Buscar "notas sobre este concepto" sin keywords exactas

### Obsidian Copilot
**Asistente AI integrado:** Copilot se conecta a OpenAI o modelos locales (via LM Studio/Ollama) y ofrece comandos AI directamente en el editor.[^14]

**Comparación con Smart Connections:**
- **Copilot:** Mejor para generación de texto, custom commands, chat
- **Smart Connections:** Mejor para búsqueda semántica y conexiones automáticas

## Control de Versiones y Backup

### Obsidian Git - Versionado Profesional
**Esencial para proyectos serios:** Obsidian Git integra Git directamente en tu vault. Auto-commit, pull/push programados, staging selectivo y diff view integrado.[^17][^18]

**Configuración óptima para tu caso:**
```

- Auto-backup cada 30 minutos
- Commit message: timestamp ISO
- Git pull al iniciar Obsidian
- .gitignore configurado para excluir .obsidian/workspace

````

**Ventajas clave:**
- Historial completo de cambios por archivo
- Sincronización entre dispositivos via GitHub/GitLab
- Rollback fácil ante errores
- Perfecto para colaboración con tu asesor de tesis

**Script alternativo (sin plugin):**[^19]
```bash
#!/usr/bin/env sh
ZK_PATH="/ruta/a/tu/vault"
cd "$ZK_PATH"
git pull
CHANGES_EXIST="$(git status --porcelain | wc -l)"
if [ "$CHANGES_EXIST" -eq 0 ]; then
    exit 0
fi
git add .
git commit -q -m "$(date +"%Y-%m-%d %H:%M:%S")"
git push -q
````

Ejecuta con `cron` cada 30 min.

### Version-Control Plugin

**Versionado granular por archivo:** Este plugin versiona notas individuales (no el vault completo). Crea "ramas" dentro de una nota para experimentar con versiones alternativas.[^20]

**Complementa Git:**

- **Git:** Versiona estructura del vault, cambios a gran escala
- **Version-Control:** Versiona evolución de notas individuales con milestones

## Plugins para Documentación Técnica

### ZotLit - Integración con Zotero

**Imprescindible para investigación académica:** ZotLit accede directamente a tu base de datos Zotero (más rápido que Zotero Integration). Importa papers en bulk con metadata automática.[^21]

**Workflow recomendado:**

1. Importa paper desde Zotero con `ZotLit: Insert literature note`
2. Automáticamente crea nota con frontmatter completo
3. Enlaza el paper a tus MOCs temáticos
4. Usa Dataview para listar papers por tema/autor/año

### PDF++ - Anotación de PDFs

**Lee y anota dentro de Obsidian:** PDF++ transforma Obsidian en un lector PDF completo. Haz highlights, toma notas y salta directamente de nota → ubicación exacta en PDF.[^22][^21]

**Perfecto para tu tesis:**

- Anotar papers técnicos sin salir de Obsidian
- Enlazar highlights de PDFs a notas conceptuales
- Crear literature review notes con citas directas

### Charts Plugin

**Visualizaciones de datos inline:** Inserta gráficas (barras, líneas, pie) directamente en notas usando bloques de configuración.[^23]

**Aplicaciones:**

- Tracking progreso de tesis (capítulos completados)
- Visualizar métricas de performance (FPS, render time)
- Dashboards de productividad

## Gestión de Tareas y Productividad

### Tasks Plugin + GTD No Next Step

**Sistema GTD completo:** Tasks permite gestionar tareas con sintaxis avanzada (fechas, recurrencia, prioridades). GTD No Next Step añade badges visuales a proyectos sin siguiente acción definida.[^24][^25]

**Sintaxis poderosa:**

```markdown
- [ ] Optimizar shader de agua 📅 2026-04-20 ⏫ #next-step
- [ ] Revisar capítulo 3 de tesis 🔁 every week 🔽 #waiting-for
```

**Dataview + Tasks = GTD automático:**[^25]

```dataview
TASK
WHERE contains(text, "#next-step") AND !completed
GROUP BY file.folder
```

## Optimización para Vaults Grandes

### Mejores Prácticas de Performance

**Tu vault crecerá rápido:** Con documentación técnica profunda, tu vault puede llegar a miles de archivos.[^26]

**Recomendaciones:**

1. **Desactiva**: `Settings > Files & Links > Detect all file extensions` (solo si no necesitas)
2. **Excluye carpetas pesadas** del indexado: `Settings > Files & Links > Excluded files`
3. **Usa Dataview con moderación**: Queries complejas impactan performance
4. **Fragmenta MOCs grandes**: En lugar de un MOC gigante, crea sub-MOCs
5. **Git submodules**: Para proyectos externos que referencias

### Estructura Óptima de Carpetas

Basado en LLM-Wiki + tu workflow técnico:

```
Cerebro_Digital/
├── index.md (raíz de todo)
├── Wiki/
│   ├── Concepts/ (nodos conceptuales y MOCs)
│   ├── Entities/ (personas, herramientas, proyectos)
│   └── Templates/ (plantillas Templater)
├── Research/ (papers, literature notes)
├── Projects/
│   ├── Unity_WebGL/
│   ├── Tesis/
│   └── CAD_Models/
└── Assets/
    ├── Images/
    ├── Diagrams/ (Excalidraw)
    └── PDFs/
```

## Herramientas Complementarias

### obsidian.md Sync vs Git

**Sincronización oficial:** Obsidian Sync es de pago pero ofrece:

- Sincronización instantánea
- Versionado por archivo (hasta 1 año)
- Encriptación end-to-end
- Sin configuración técnica

**Git es mejor si:**

- Quieres control total
- Necesitas historial ilimitado
- Prefieres soluciones gratuitas
- Ya usas GitHub para otros proyectos

### Combinación recomendada:

- **Git:** Versionado y backup
- **Obsidian Sync (opcional):** Sincronización rápida entre dispositivos

## API y Desarrollo Custom

### Obsidian API

**Para automatización avanzada:** La API oficial te permite crear plugins custom en TypeScript.[^27][^28]

**Casos de uso para tu perfil:**

- Plugin custom que exporta documentación a formato Unity-compatible
- Script que genera automáticamente enlaces entre archivos CAD y sus referencias
- Integración con APIs externas (GitHub, Unity Cloud)

### User Plugins

**Scripts sin crear plugin completo:** User Plugins te permite usar la API de Obsidian dentro de JavaScript snippets. Ideal para probar ideas sin estructura completa de plugin.[^28]

## Workflow LLM-Wiki Optimizado

Basado en la metodología de Karpathy, aquí está el workflow ideal:[^29][^1]

### 1. Ingesta de Documentación

```bash
# Comando Templater para procesar archivo nuevo
1. LLM lee el documento
2. Extrae conceptos clave
3. Crea nota en Concepts/ con backlinks apropiados
4. Actualiza index.md si es concepto raíz
```

### 2. Construcción de MOCs Dinámicos

**En lugar de enlaces manuales, usa Dataview:**

````markdown
# MOC: WebGL Development

## Core Concepts

```dataview
LIST
FROM [[MOC_WebGL]] AND "Concepts"
WHERE contains(tipo, "concepto")
SORT file.name ASC
```
````

## Tutorials

```dataview
TABLE estado, fuente
FROM [[MOC_WebGL]] AND "Research"
WHERE contains(tipo, "tutorial")
```

````

### 3. Mantenimiento con Linting
**Script de salud del vault:**
```javascript
// Encuentra archivos huérfanos
const orphans = app.vault.getMarkdownFiles()
  .filter(file => {
    const cache = app.metadataCache.getFileCache(file);
    return !cache?.links?.length && !cache?.backlinks?.length;
  });

// Reporta en nota de mantenimiento
````

### 4. Vista Dual (Karpathy Style)

**Configuración óptima:**[^1]

- **Pantalla izquierda:** Claude/ChatGPT/LM Studio (agente LLM)
- **Pantalla derecha:** Obsidian (live preview)
- **Workflow:** LLM edita via API → tú revisas en Obsidian en tiempo real

## Plugins Adicionales Útiles

### Auto Keyword Linker

**Construcción automática de grafo:** Define keywords una vez (ej: "ML", "machine learning") y el plugin convierte automáticamente texto plano en `[[enlaces]]`.[^12]

### Semantic Canvas

**Propiedades visuales:** Edita propiedades de frontmatter visualmente en Canvas. Perfecto para gestionar estados de notas masivamente.[^12]

### Connections

**Relaciones nombradas:** Crea relaciones con nombres específicos como "es padre de", "usa", "extiende". Más semántico que backlinks simples.[^12]

## Checklist de Instalación Prioritaria

Para tu proyecto específico, instala en este orden:

**Día 1 - Fundamentos:**

1. ✅ Dataview
2. ✅ Templater
3. ✅ Obsidian Git
4. ✅ Breadcrumbs

**Día 2 - Visualización:** 5. ✅ Excalidraw 6. ✅ Canvas (core plugin) 7. ✅ Juggl o InfraNodus

**Día 3 - IA y Búsqueda:** 8. ✅ Smart Connections 9. ✅ Local GPT u Obsidian Copilot 10. ✅ ZotLit (si usas Zotero)

**Opcional según necesidad:**

- PDF++ (si lees muchos papers)
- Tasks (si necesitas GTD)
- Charts (si quieres dashboards)
- Auto Keyword Linker (para acelerar linking)

## Recursos Finales

**Tutoriales clave:**

- [LLM Wiki Setup Guide](https://www.youtube.com/watch?v=iXd0t60YmMw) - Tutorial completo de implementación[^29]
- [Dataview + MOCs](https://github.com/seqis/ObsidianMOC) - Guía detallada de MOCs dinámicos[^2]
- [Obsidian for Academics](https://effortlessacademic.com/best-obsidian-plugins-for-academics/) - Workflow para investigación[^21]

**Comunidades:**

- [r/ObsidianMD](https://reddit.com/r/ObsidianMD) - Comunidad activa
- [Obsidian Forum](https://forum.obsidian.md) - Soporte oficial
- [Obsidian Stats](https://www.obsidianstats.com) - Directorio completo de plugins

Tu setup resultante será un "Cerebro Digital" autoorganizable donde el LLM actúa como bibliotecario automático, Dataview mantiene índices vivos, y Git preserva cada iteración de tu conocimiento. ¡Perfecto para escalar tu investigación técnica sin perderte en el caos!
<span style="display:none">[^30][^31][^32][^33][^34][^35][^36][^37][^38][^39][^40][^41][^42][^43][^44][^45]</span>

<div align="center">⁂</div>

[^1]: https://gist.github.com/karpathy/442a6bf555914893e9891c11519de94f

[^2]: https://github.com/seqis/ObsidianMOC

[^3]: https://github.com/silentvoid13/Templater

[^4]: https://www.obsidianstats.com/plugins/templater-obsidian

[^5]: https://www.obsidianstats.com/tags/hierarchy

[^6]: https://www.reddit.com/r/ObsidianMD/comments/r18dnn/the_breadcrumbs_plugin_deserves_to_be_more_popular/

[^7]: https://github.com/Developer-Mike/obsidian-advanced-canvas

[^8]: https://www.youtube.com/watch?v=NPTgZlZjjoM

[^9]: https://www.youtube.com/watch?v=lGh53N_Lj2Q

[^10]: https://forum.obsidian.md/t/excalidraw-full-featured-sketching-plugin-in-obsidian/17367

[^11]: https://forum.obsidian.md/t/juggl-out-now-1-0-1-a-completely-interactive-stylable-and-expandable-graph-view-plugin/9625

[^12]: https://www.obsidianstats.com/tags/knowledge-graph

[^13]: https://www.obsidianstats.com/plugins/local-gpt

[^14]: https://www.youtube.com/watch?v=mZ8TJ59Hj28

[^15]: https://smartconnections.app/smart-connections/lookup/

[^16]: https://smartconnections.app/smart-connections/

[^17]: https://github.com/Vinzent03/obsidian-git

[^18]: https://www.obsidianstats.com/plugins/obsidian-git

[^19]: https://forum.obsidian.md/t/obsidian-github-integration-for-sync-and-version-control/6369

[^20]: https://forum.obsidian.md/t/plugin-version-control/109100

[^21]: https://effortlessacademic.com/best-obsidian-plugins-for-academics/

[^22]: https://www.youtube.com/watch?v=dbWxGVAjcgU

[^23]: https://www.obsibrain.com/blog/top-obsidian-plugins-in-2026-the-essential-list-for-power-users

[^24]: https://www.obsidianstats.com/plugins/gtd-no-next-step

[^25]: https://forum.obsidian.md/t/gtd-with-obsidian-a-ready-to-go-gtd-system-with-task-sequencing-quick-add-template-waiting-on-someday-maybe-and-more/65502

[^26]: https://www.reddit.com/r/ObsidianMD/comments/1rggg4n/power_users_10k_notes_what_breaks_first_when_your/

[^27]: https://github.com/obsidianmd/obsidian-api

[^28]: https://www.obsidianstats.com/tags/api

[^29]: https://www.youtube.com/watch?v=iXd0t60YmMw

[^30]: https://obsidian.md/plugins?search=graph

[^31]: https://www.dsebastien.net/the-must-have-obsidian-plugins-for-2026/

[^32]: https://www.reddit.com/r/ObsidianMD/comments/1r3rhwt/the_musthave_obsidian_plugins_for_2026/

[^33]: https://github.com/yanhuiwu7/obsidian-knowledge-graph

[^34]: https://readwithai.substack.com/p/automated-maps-of-content-in-obsidian

[^35]: https://forum.obsidian.md/t/personal-knowledge-graphs/69264

[^36]: https://obsidian.md/plugins

[^37]: https://tldl.club/en/noted/685645d72c2e78278c2f87a4

[^38]: https://www.youtube.com/watch?v=ajNSLH3Pic0

[^39]: https://www.youtube.com/watch?v=oBUdD83kja4\&vl=en

[^40]: https://www.reddit.com/r/ObsidianMD/comments/15g3f4x/templater_plugin_to_automate_my_workflow/

[^41]: https://www.reddit.com/r/ObsidianMD/comments/1qjtptj/my_obsidian_version_control_and_syncing_workflow/

[^42]: https://forum.obsidian.md/t/the-easiest-way-to-setup-obsidian-git-to-backup-notes/51429

[^43]: https://forum.obsidian.md/t/slow-performance-with-large-vaults/16633

[^44]: https://www.youtube.com/watch?v=HnqXH6z4WrY

[^45]: https://forum.obsidian.md/t/versioning/55944
