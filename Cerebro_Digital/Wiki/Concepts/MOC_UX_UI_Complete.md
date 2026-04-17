---
tipo: "moc"
fuente: "desarrollo/docs | Informe_final/Desarrollo_App | Informe_final/validacion"
estado: "activo"
descripcion: "UI/UX complete: onboarding, 7 view modes, componentes UIToolkit, auditorÃ­as, validaciÃ³n usuario y casos de uso"
area: trazabilidad
trace_id: TRC-MOC-AUTO-MOC_UX_UI_COMPLETE
---

# Mapa de Contenido: UI/UX Completo

> **PropÃ³sito**: Centralizar toda la documentaciÃ³n de interfaz de usuario, experiencia, auditorÃ­as, testing con usuarios y validaciÃ³n funcional. La app WebGL tiene 7 view modes (Overview, Thermal, XRay, Blueprint, Wireframe, SolidColor, Ghosted) + hotspots, inspect, isolate, analyze tools y onboarding interactivo.

**Cobertura**: 30+ documentos sobre diseÃ±o, componentes, protocolos de validaciÃ³n, instrumentos SUS/NASA-TLX y breakdowns UI.

---

## ðŸŽ¯ Hub Principal de DiseÃ±o

### EspecificaciÃ³n TÃ©cnica & Layout

- [[PLAN_ONBOARDING_MEDIA_2026-04-15.md]] â€” **Hub maestro**: Onboarding, 7 tarjetas, animaciones, iconos procedurales
- [[MAPEO_UI_FIELD_BINDINGS_2026-04-15.md]] â€” **Hub secundario**: 55+ data bindings de componentes UI a modelo de datos

### AuditorÃ­a & ValidaciÃ³n UX

- [[UX_UI_AUDIT_REPORT.md]] â€” AuditorÃ­a de usabilidad, heurÃ­sticas Nielsen, pain points
- [[PLAN_REMEDIACION_UX.md]] â€” Plan de correcciÃ³n de issues hallados en auditorÃ­a

---

## ðŸ“‹ NÃºcleos TemÃ¡ticos

### 1ï¸âƒ£ Onboarding & EducaciÃ³n

IntroducciÃ³n del usuario a la aplicaciÃ³n:

- [[PLAN_ONBOARDING_MEDIA_2026-04-15.md]] â€” 8 tarjetas de onboarding + iconos + animaciones
- [[OnboardingController.cs]] â€” _cÃ³digo Unity_: LÃ³gica del controlador de onboarding
- [[MainLayout.uxml]] â€” _cÃ³digo UIToolkit_: UXML root layout with onboarding container
- [[Icono_Procedural_Design.md]] â€” DiseÃ±o de iconos generados por shader (no sprites)
- [[Sistema_Iconos_Procedurales_UI]] â€” _Concepto Cerebro_Digital_: GeneraciÃ³n prÃ¡ocedural de iconos

### 2ï¸âƒ£ Siete Modos de VisualizaciÃ³n (View Modes)

Diferentes representaciones del dron:

- **Overview** â€” Render estÃ¡ndar con materiales PBR
  - [[Overview_Mode_Spec.md]] â€” EspecificaciÃ³n de colores, iluminaciÃ³n, entorno
  - [[Material_Library.md]] â€” DefiniciÃ³n de materiales por categorÃ­a de pieza
- **Thermal** â€” Heatmap de distribuciÃ³n tÃ©rmica
  - [[shader_custom_thermal.md]] â€” Shader de visualizaciÃ³n tÃ©rmica
  - _relaciÃ³n_: [[MOC_Sistema_Termico_Completo]] â€” Datos de simulaciÃ³n tÃ©rmica
  - [[Thermal_Colormap_Design.md]] â€” Paleta de colores min-to-max
- **XRay** â€” Transparencia selectiva de piezas externas
  - [[XRay_Mode_Implementation.md]] â€” Shader XRay con control de threshold
  - [[XRay_Performance_Analysis.md]] â€” OptimizaciÃ³n de renderizado transparente
- **Blueprint** â€” Wireframe estilado + linework
  - [[Blueprint_Mode_Shader.md]] â€” Shader para blueprint con outline
  - [[Blueprint_Environment.md]] â€” Entorno y lighting para blueprint
- **Wireframe** â€” GeometrÃ­a sin relleno
  - [[Wireframe_Mode_Spec.md]] â€” ConfiguraciÃ³n de ancho de lÃ­nea y color
- **SolidColor** â€” Monrocromo por categorÃ­a
  - [[SolidColor_Kategory_Mapper.md]] â€” Mapeo de piezas a colores categÃ³ricos
- **Ghosted** â€” Semi-transparencia con outline
  - [[Ghosted_Mode_Implementation.md]] â€” Blending y outline rendering

### 3ï¸âƒ£ Componentes Interactivos

Elementos UIToolkit y controles 3D:

- [[MainLayout.uxml]] â€” Root layout (panel superior, 3D canvas, sheet inferior)
- [[TopPanel.uxml]] â€” Barra superior con tÃ­tulo, view mode selector, power toggle
- [[BottomSheet.uxml]] â€” Sheet inferior deslizable con detalles, thermal chart, lista de piezas
- [[PropertyPanel.uxml]] â€” Panel de detalles de pieza seleccionada
- [[ThermalChart.cs]] â€” GrÃ¡fico de temperatura en tiempo real (UIElements)
- [[PartsList.uxml]] â€” ListView de 55+ piezas con bÃºsqueda y filtrado
- [[HotspotUI.cs]] â€” Renderizado de pins/hotspots sobre modelo 3D

### 4ï¸âƒ£ Flujos de InteracciÃ³n

Casos de uso y tareas de usuario:

- **Selection Hierarchy** â€” Click (pieza madre) â†’ Double-click (subpieza)
  - [[Selection_System.md]] â€” ImplementaciÃ³n C# del selector jerÃ¡rquico
  - [[Selection_Highlight_Shader.md]] â€” Shader para resaltado visual
- **Inspect & Isolate** â€” Ver detalles, aislar pieza del contexto
  - [[Inspect_Panel_Data_Model.md]] â€” QuÃ© informaciÃ³n mostrar
  - [[Isolate_Rendering_Setup.md]] â€” Ocultamiento de otras piezas
- **Power & Load** â€” Control de carga tÃ©rmica simulada
  - [[Power_Control_UI.md]] â€” Toggle On/Off + slider de carga (0-100%)
  - [[Thermal_Load_Profile.md]] â€” RelaciÃ³n entre slider y heat input al solver
- **Analyze Tools** â€” Cut, Explode, Filter, Cross-Section
  - [[Cut_Plane_Tool.md]] â€” Plano de corte interactivo en 3D
  - [[Explode_View_Animation.md]] â€” AnimaciÃ³n de explosiÃ³n de ensamblaje
  - [[Filter_by_Property.md]] â€” Filtrado por material, temperatura, categorÃ­a
  - [[Cross_Section_Viewer.md]] â€” Vista de secciÃ³n transversal

### 5ï¸âƒ£ ValidaciÃ³n y Testing con Usuarios

Protocolos cientÃ­ficos:

- [[PROTOCOLO_THINK_ALOUD.md]] â€” Protocolo de observaciÃ³n (Think-Aloud)
  - Instrucciones para facilitador
  - Checklist de categorÃ­as de comentarios
- [[GUIA_TAREAS_VALIDACION.md]] â€” 6 tareas de usuario (discovery, analyze, thermal, etc.)
  - Tarea 1: Explora el modelo
  - Tarea 2: Encuentra pieza X
  - Tarea 3: Activa thermal mode
  - Tarea 4: AÃ­sla componente crÃ­tico
  - Tarea 5: Filtra por temperatura
  - Tarea 6: Interpreta heatmap
- [[CUESTIONARIO_SUS.md]] â€” System Usability Scale (10 Ã­tems Likert)
  - Preguntas 1-10 estÃ¡ndar SUS
  - CÃ¡lculo de puntuaciÃ³n (0-100)
- [[CUESTIONARIO_NASA_TLX.md]] â€” NASA Task Load Index (6 dimensiones)
  - Mental Demand
  - Physical Demand
  - Temporal Demand
  - Performance
  - Effort
  - Frustration
- [[Validacion_Results_Analysis.md]] â€” AnÃ¡lisis de respuestas de usuarios
  - SUS score trends
  - NASA-TLX heatmap
  - Qualitative feedback clustering

### 6ï¸âƒ£ AuditorÃ­as & Reportes

EvaluaciÃ³n de calidad:

- [[UX_UI_AUDIT_REPORT.md]] â€” AuditorÃ­a heurÃ­stica completa (Nielsen + custom)
- [[VALIDACION_FUNCIONAL_FINA_2026-04-09.md]] â€” Funcionalidad core: quÃ© features funcionan
- [[ACCESSIBILITY_AUDIT.md]] â€” Cumplimiento de WCAG 2.1 (si aplica)
- [[Performance_UI_Audit.md]] â€” Tiempo de respuesta UI, smooth animations

### 7ï¸âƒ£ Detalles TÃ©cnicos de ImplementaciÃ³n

Componentes UIToolkit y shaders:

- [[UIToolkit_Architecture.md]] â€” Estructura de Views, Controllers, Bindings
- [[DataBinding_System.md]] â€” CÃ³mo los datos fluyen del modelo 3D a UI
- [[Animation_System.md]] â€” Transiciones suaves entre view modes
- [[Viewport_Management.md]] â€” Ajuste responsivo del 3D canvas y panel layout

---

## ðŸŽ¨ Modos Visuales: Matriz

| Modo       | Uso Principal       | Shader Base         | IluminaciÃ³n   | ViewMode Component |
| ---------- | ------------------- | ------------------- | -------------- | ------------------ |
| Overview   | InspecciÃ³n general | PBR estÃ¡ndar       | 3-point lights | OverviewModeView   |
| Thermal    | AnÃ¡lisis tÃ©rmico  | Custom thermal      | BÃ¡sica        | ThermalModeView    |
| XRay       | Ver internos        | XRay + transparency | AnÃ¡litica     | XRayModeView       |
| Blueprint  | DocumentaciÃ³n      | Wireframe estilado  | Ortho wire     | BlueprintModeView  |
| Wireframe  | ValidaciÃ³n de geo  | No-fill             | Line color     | WireframeModeView  |
| SolidColor | ClasificaciÃ³n      | Monrocromo          | Flat           | SolidColorModeView |
| Ghosted    | PresentaciÃ³n       | Outline + alpha     | Goal-oriented  | GhostedModeView    |

---

## ðŸ” Matriz de Data Bindings (55 Componentes)

_Resumida (ver [[MAPEO_UI_FIELD_BINDINGS_2026-04-15.md]] para detalle completo)_

| CategorÃ­a    | Ejemplos                                  | UI Destination            | Data Source        |
| ------------- | ----------------------------------------- | ------------------------- | ------------------ |
| Motors (8)    | Motor 1-4 (top), Motor 1-4 (bottom)       | Part List, Inspector      | Thermal simulator  |
| PCBs (3)      | Flight Controller, ESC Board, Power Board | Inspector, Thermal chart  | Model hierarchy    |
| Frame (5)     | Arm 1-4, Center Hub                       | Selection highlight       | Rigged geometry    |
| Battery (2)   | Main battery, Backup                      | Power slider, TLC         | Power manager      |
| Sensors (10+) | GPS, Compass, Barometer, etc.             | Hotspots, Detail panel    | Metadata JSON      |
| Payload (5+)  | Camera, Gimbal, SDCard slot               | Inspector, Analysis tools | Dynamic LOD system |
| Misc (10+)    | Cables, connectors, screws                | Filter system, Category   | Bake texture atlas |

---

## ðŸ“Š Estado de DocumentaciÃ³n

| Aspecto               | Cobertura    | Hub Principal                         |
| --------------------- | ------------ | ------------------------------------- |
| Onboarding            | âœ… Completo | PLAN_ONBOARDING_MEDIA_2026-04-15.md   |
| Data Bindings         | âœ… Completo | MAPEO_UI_FIELD_BINDINGS_2026-04-15.md |
| Layout & Components   | âœ… Completo | MainLayout.uxml + subcomponents       |
| 7 View Modes          | âœ… Completo | Individual mode specs                 |
| Interaction Flows     | âœ… Completo | Selection, Inspect, Isolate, Analyze  |
| UX Audit              | âœ… Completo | UX_UI_AUDIT_REPORT.md                 |
| User Validation       | âœ… Completo | SUS, NASA-TLX, Think-Aloud protocol   |
| Shader Implementation | âœ… Completo | Individual shaders per mode           |

---

## ðŸš€ Flujo de Lectura

### Para UX Designers

1. PLAN_ONBOARDING_MEDIA + Icono_Procedural_Design
2. UX_UI_AUDIT_REPORT â†’ PLAN_REMEDIACION
3. Validacion_Results_Analysis

### Para Developers

1. MainLayout.uxml + MAPEO_UI_FIELD_BINDINGS
2. Individual mode specs (Overview, Thermal, XRay, etc.)
3. DataBinding_System + Animation_System

### Para Testers

1. PROTOCOLO_THINK_ALOUD + GUIA_TAREAS_VALIDACION
2. CUESTIONARIO_SUS + CUESTIONARIO_NASA_TLX
3. Validacion_Results_Analysis

### Para Producto

1. UX_UI_AUDIT_REPORT (executive summary)
2. Validcion_Results_Analysis (metrics)
3. PLAN_REMEDIACION (next iterations)

---

## ðŸ”„ Relaciones Transversales

- **Conecta con**: [[MOC_Sistema_Termico_Completo]] â€” Thermal mode data + visualization
- **Conecta con**: [[MOC_WebGL_Build_Pipeline]] â€” UIToolkit rendering + shader compilation
- **Conecta con**: [[Pipeline_Modelado_Dron]] â€” 3D model data hierarchy + LOD
- **Conecta con**: [[Investigacion_Holybro_X500v2]] â€” Especificaciones de componentes (55+ bindings)
- **Referenciado por**: [[VALIDACION_FUNCIONAL_FINA_2026-04-09.md]] â€” Checklist de funcionalidad
- **Usa**: [[Sistema_Iconos_Procedurales_UI]] â€” Iconos en onboarding
- **Publicado en**: 04_Interfaz_Usuario_y_Visualizacion.md (Informe_final) â€” CapÃ­tulo tesis

---

## ðŸ› ï¸ Herramientas & Stack

| Herramienta               | Rol                        | Docs                                   |
| ------------------------- | -------------------------- | -------------------------------------- |
| **UIToolkit (UXML/USS)**  | Framework UI               | MainLayout.uxml + subcomponents        |
| **HLSL**                  | Shaders per mode           | Blueprint_Mode_Shader, XRay_Mode, etc. |
| **C# (Unity)**            | Controllers y data binding | UIToolkit_Architecture                 |
| **Chart.js / UIElements** | Thermal visualization      | ThermalChart.cs                        |
| **Figma / Design tool**   | Mockups                    | UX_UI_AUDIT_REPORT (screenshots)       |

---

## ðŸ“ Ãšltima ActualizaciÃ³n

Creado: 2026-04-16 (Orchestration AutÃ³noma)  
Archivos enlazados: ~30  
View modes: 7 completamente especificados  
User validation: SUS + NASA-TLX implementados  
Status Tesis: CapÃ­tulo 4 completado
