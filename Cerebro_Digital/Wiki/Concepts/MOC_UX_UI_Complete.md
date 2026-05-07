---
tipo: "moc"
fuente: "desarrollo/docs | Informe_final/Desarrollo_App | Informe_final/validacion"
estado: "activo"
descripcion: "Mapa actualizado de UX/UI: mobile-first, onboarding procedural, ficha inferior, jerarquia de seleccion, modos visuales y validacion"
area: trazabilidad
trace_id: TRC-MOC-AUTO-MOC_UX_UI_COMPLETE
---

# MOC UX/UI Completo

## Proposito

Este mapa centraliza la documentacion de interfaz, experiencia de usuario, onboarding, seleccion, modos visuales, testing y validacion funcional. Su referencia es la build final saneada, no los planes historicos que puedan contener cifras o modulos anteriores.

Flujo visible canonico:

```text
Hero -> Onboarding / Explore -> seleccion -> ficha inferior -> Inspect / Analyze / Studio
```

## Lectura de estado

- Expuesto en UI final: Hero, onboarding, seleccion, ficha inferior, Inspect, Analyze, Studio, hotspots, isolate, power/load, explode, cross-section, filtros, Thermal y leyenda.
- Implementado pero oculto: modos o controles que existen en codigo pero no se presentan como flujo final.
- Legado/no integrado: paneles o modulos que permanecen en repo pero no sostienen la experiencia final.
- Trabajo futuro: extensiones no cerradas para entrega.

## Nucleos tematicos

### 1. UX/UI mobile-first

- [[INF_EST_31_UX_UI_Mobile_First]] - explicacion pedagogica del enfoque mobile-first, adaptacion desktop y reglas de jerarquia visual.
- [[INF_EST_04_Desarrollo_Implementacion#UX/UI mobile-first]] - contexto dentro del capitulo de desarrollo.

Idea de defensa:

> La version con mayor respaldo teorico es mobile-first. Desktop funciona, pero se conserva como adaptacion funcional del layout movil.

### 2. Onboarding procedural

- [[INF_EST_31_UX_UI_Mobile_First#Onboarding como reduccion de incertidumbre]]
- [[INF_EST_32_Iconografia_Procedural_Microinteracciones#Onboarding procedural]]
- [[PLAN_ONBOARDING_MEDIA_2026-04-15.md]] - plan historico de onboarding.
- [[OnboardingController.cs]] - controlador de tarjetas, texto, estado y plataforma.
- [[OnboardingAnimationView]] - demos procedurales dibujadas con `Painter2D`.

Lectura correcta:

> El onboarding no es video ni GIF. Es ayuda contextual dibujada por codigo para explicar gestos, seleccion, ficha, modos y sliders con el mismo lenguaje visual de la app.

### 3. Ficha inferior e info panel

- [[INF_EST_31_UX_UI_Mobile_First#Info panel como centro semantico]]
- [[INF_EST_35_Tooling_Arquitectura_Runtime#UIDetailsSheet]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa#12. Por que la ficha inferior es tan importante?]]

La ficha inferior organiza:

- identificacion;
- especificaciones;
- ensamblaje;
- datos disponibles y `N/A`;
- nivel de lectura: pieza madre, subpieza, grupo de hotspot o fastener.

Idea de defensa:

> La ficha inferior es el puente entre seleccionar geometria y comprender informacion tecnica.

### 4. Jerarquia de seleccion

- [[INF_EST_35_Tooling_Arquitectura_Runtime#Jerarquia de seleccion]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa#13. Que diferencia hay entre pieza madre, subpieza, grupo de hotspot y fastener?]]

Niveles:

- pieza madre;
- subpieza;
- grupo de hotspot;
- fastener.

Esta jerarquia incide en ficha, camara, highlight e isolate. Si esos sistemas no comparten el mismo nivel de lectura, la experiencia se vuelve inconsistente.

### 5. Iconografia procedural y microinteracciones

- [[INF_EST_32_Iconografia_Procedural_Microinteracciones]]
- [[Sistema_Iconos_Procedurales_UI]]
- `desarrollo/docs/investigacion/11_plan_iconos_ui.md`
- `desarrollo/docs/investigacion/12_plan_iconos_procedurales_cs.md`
- `desarrollo/docs/investigacion/13_matematicas_iconos_procedurales.md`

Idea de defensa:

> Los iconos no son decoracion. Son feedback tecnico: confirman estado, anticipan accion y conservan nitidez sin sprites pesados.

### 6. Modos visuales y entornos

- [[INF_EST_33_Shaders_ViewModes_Entornos]]
- [[INF_EST_04_Desarrollo_Implementacion#Shaders y entornos]]

Lectura final:

- Realistic: modo base.
- X-Ray: lectura interna por transparencia.
- Solid Color: lectura limpia de forma/categoria.
- Thermal: visualizacion heuristica con leyenda.
- Blueprint: entorno tecnico, no shader independiente final.
- Wireframe/Ghosted: documentar segun estado real de exposicion.

### 7. Flujos de interaccion

- Onboarding: ensenar antes de exigir.
- Seleccion: convertir geometria en contexto.
- Inspect: hotspots, ficha, isolate y power/load.
- Analyze: explode, corte y filtros.
- Studio: modos visuales, entornos, luz y Thermal.

### 8. Validacion UX

- [[INF_EST_03_Marco_Metodologico]]
- [[INF_EST_05_Resultados_Analisis]]
- [[INF_EST_91_Preguntas_Dificiles_Defensa]]

Instrumentos:

- SUS: usabilidad percibida del visor 3D.
- NASA-TLX Raw: workload percibido por condicion.
- Think-Aloud: explicacion cualitativa de errores, dudas y estrategias.

## Preguntas clave

- Por que mobile-first si tambien corre en escritorio.
- Por que el onboarding se hizo por codigo.
- Por que la ficha inferior es central.
- Que significa seleccionar pieza madre, subpieza, grupo o fastener.
- Por que Blueprint es entorno.
- Por que algunos modos no se exponen en UI final.

## Relaciones transversales

- [[MOC_Informe_Final_Estudio_Profundo]]
- [[INF_EST_20_Catalogo_Secciones_y_Cobertura]]
- [[INF_EST_50_Storytelling_Sustentacion_30min]]
- [[INF_EST_51_Guion_Completo_Sustentacion_30min]]
- [[MOC_WebGL_Build_Pipeline]]
- [[MOC_Sistema_Termico_Completo]]
