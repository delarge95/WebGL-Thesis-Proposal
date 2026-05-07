# Cerebro Digital: Índice General

Este es el nodo central de la Wiki de tu proyecto `WebGL_tesis`. A medida que documentemos componentes de Unity, scripts, piezas CAD, simulaciones térmicas o investigaciones clínicas, este archivo se actualizará automáticamente.

## Vistas Rápidas (Dataview)

_Ocupa el plugin [Dataview] para mostrar tablas automáticas:_

```dataview
TABLE file.mtime AS "Última Modificación", tags AS "Etiquetas"
FROM "Cerebro_Digital"
WHERE file.name != "index"
AND file.name != "log"
AND file.name != "SYSTEM_SCHEMA"
AND lower(string(tipo)) != "sistema"
AND lower(string(estado)) != "archivado"
SORT file.mtime desc
```

## Entidades y Componentes

_(Archivos enfocados en objetos concretos: scripts, objetos 3D, ecuaciones aisladas, referencias directas a archivos LaTeX)_

- [[ProceduralPinsIcon]]
- [[DronePartDataFixer]]
- [[Reporte_LaTeX_Resultados]]
- [[Fastener_Builder_Addon]]

## Conceptos y Resúmenes

_(Archivos enfocados en ideas generales, reportes sintetizados, conclusiones y debates)_

- [[Estrategia_Tesis_WebGL]]
- [[Optimizacion_Brotli_WebGL]]
- [[Arquitectura_Termica_Dron]]
- [[Optimizacion_CAD_WebGL]]
- [[Pipeline_Modelado_Dron]]
- [[Estrategia_Shaders_WebGL]]
- [[Investigacion_Holybro_X500v2]]
- [[Fisica_Termica_Dron]]
- [[Sistema_Iconos_Procedurales_UI]]
- [[Estabilidad_y_Migracion_Unity6]]

## Mapas de Contenido de Raíz (MOCs)

_(Sub-Índices generados automáticamente para agrupar repositorios de manera orgánica y combatir los nodos sueltos)_

### 🔴 MOCs Estratégicos (Archivos Transversales de Alto Impacto)

- [[MOC_WebGL_Build_Pipeline]] — Settings, build, optimización WebGL, shaders, deployment (55+ archivos)
- [[MOC_Sistema_Termico_Completo]] — Arquitectura térmica FEA, modelado CAD, validación Wolfram, visualización (35+ archivos)
- [[MOC_UX_UI_Complete]] — Onboarding, 7 view modes, auditorías UX, validación usuario SUS/NASA-TLX (30+ archivos)

### 🔵 MOCs Secundarios (Consolidación de Archivos Específicos)

- [[MOC_Drone_Research]] — Investigación Holybro X500 V2, especificaciones, componentes, CAD, data bindings (40+ archivos)
- [[MOC_Testing_Validation]] — Testing con usuarios, protocolos, auditorías funcionales, SUS/NASA-TLX, reportes (25+ archivos)
- [[MOC_Academic_Thesis]] — Estructura LaTeX, 6 capítulos, bibliografía, auditorías académicas, manuales (25+ archivos)
- [[MOC_Informe_Final_Estudio_Profundo]] — Módulos pedagógicos por capítulo, guion completo de sustentación y explicaciones profundas del informe final

### 📚 MOCs de Dominio (Documentación Temática)

- [[MOC_Documentacion_Tecnica]] — Arquitectura, componentes, capítulos de tesis
- [[MOC_Auditorias_y_Planes]] — Reports, auditorías, roadmaps, planes de trabajo
- [[MOC_Validacion_y_Presentacion]] — Testing, protocolos, presentación, defensa
- [[MOC_Portafolio_Personal]] — Case studies, breakdowns, estrategia carrera
- [[MOC_Agentes_Skills]] — Automatización, workflows, custom skills
- [[Cerebro_Digital/Wiki/Concepts/MOC_Entregables_Global]] — Mapa maestro de Propuesta, Informe final y Manuales

### 🧭 MOCs de Cobertura Total (Anti-Huérfanos)

- [[MOC_Conectividad_Total]] — Enlace explícito a todo el vault (cobertura total)
- [[MOC_Indice_Alfabetico_Global]] — Segunda ruta de acceso para elevar conectividad mínima
- [[MOC_Secciones_del_Vault]] — Navegación por secciones/capas del repositorio
- [[DASHBOARD_SALUD_ENLACES]] — Monitor continuo de archivos sin inlinks o con outlinks bajos

## Trazabilidad (Sprint 1)

- [[Cerebro_Digital/Wiki/Concepts/MATRIZ_TRAZABILIDAD_ENTREGABLES]] — Matriz central Entregable -> Evidencia -> Script -> Fuente
- [[Cerebro_Digital/Wiki/Concepts/CATALOGO_SCRIPTS_UNITY]] — Catálogo funcional de scripts Unity por categorías
- [[Cerebro_Digital/Wiki/Concepts/REGISTRO_FUENTES_BIBLIOGRAFICAS]] — Registro maestro de bibliografía y estado de descarga

## Dashboard Operativo (Fase 1)

### Auditoria Excalidraw -> Binarios LaTeX

```dataviewjs
const files = app.vault.getFiles();
const excalidraw = files.filter(f => f.path.endsWith(".excalidraw.md"));
const missing = [];

for (const f of excalidraw) {
	const png = f.path.replace(".excalidraw.md", ".excalidraw.png");
	const svg = f.path.replace(".excalidraw.md", ".excalidraw.svg");
	const hasPng = !!app.vault.getAbstractFileByPath(png);
	const hasSvg = !!app.vault.getAbstractFileByPath(svg);

	if (!hasPng && !hasSvg) {
		missing.push([f.link, f.stat?.mtime ? new Date(f.stat.mtime).toISOString().slice(0, 10) : "N/A", "Falta PNG/SVG"]);
	}
}

dv.header(3, "Excalidraw sin exportacion");
if (missing.length === 0) {
	dv.paragraph("Integridad grafica OK: todos los .excalidraw.md tienen PNG o SVG.");
} else {
	dv.table(["Archivo", "Ultima Modificacion", "Estado"], missing);
}
```

### Alertas de Integracion Unity WebGL

```dataview
TABLE file.link AS "Build", build_time_ms AS "Build Time", total_vertices AS "Vertices", brotli_compression AS "Brotli", status AS "Estado"
FROM "Telemetria/Unity_Builds"
WHERE lower(string(status)) = "failed"
OR lower(string(status)) = "degraded"
OR number(total_vertices) > 20000
SORT file.ctime desc
LIMIT 8
```

### Centro de Mando Tactico (Tasks)

```tasks
not done
tags include #tech-art OR #unity OR #blender OR #tesis
sort by priority
```

## Fase 2: NASA-TLX (DataviewJS + Charts)

```dataviewjs
const pages = dv.pages('"Datos/Sujetos_Prueba"')
	.where(p => (p.tipo || "") === "dataset_sujeto" && (p.encuesta || "") === "NASA-TLX");

const dims = [
	"exigencia_mental",
	"exigencia_fisica",
	"exigencia_temporal",
	"rendimiento",
	"esfuerzo",
	"frustracion"
];

if (pages.length === 0) {
	dv.paragraph("Aun no hay datasets NASA-TLX en Datos/Sujetos_Prueba.");
} else {
	const avg = {};
	for (const d of dims) {
		const vals = pages
			.map(p => Number(p[d]))
			.filter(v => !Number.isNaN(v));
		avg[d] = vals.length ? (vals.reduce((a, b) => a + b, 0) / vals.length) : 0;
	}

	dv.header(3, "Promedios NASA-TLX");
	dv.table(["Dimension", "Promedio"], dims.map(d => [d, avg[d].toFixed(2)]));

	// Si Chart.js esta disponible (por plugin), renderiza radar.
	const hasChart = typeof window.Chart !== "undefined";
	if (hasChart) {
		const canvas = dv.el("canvas", "");
		new window.Chart(canvas, {
			type: "radar",
			data: {
				labels: [
					"Mental",
					"Fisica",
					"Temporal",
					"Rendimiento",
					"Esfuerzo",
					"Frustracion"
				],
				datasets: [{
					label: "NASA-TLX Promedio",
					data: [
						avg.exigencia_mental,
						avg.exigencia_fisica,
						avg.exigencia_temporal,
						avg.rendimiento,
						avg.esfuerzo,
						avg.frustracion
					],
					borderWidth: 2
				}]
			},
			options: {
				scales: {
					r: {
						min: 0,
						max: 20,
						ticks: { stepSize: 2 }
					}
				}
			}
		});
	} else {
		dv.paragraph("Chart.js no disponible en esta vista. Instala/activa Obsidian Charts para radar interactivo.");
	}
}
```
