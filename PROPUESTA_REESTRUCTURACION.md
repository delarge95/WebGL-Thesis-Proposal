
## 4. Detalle de Operaciones por Módulo (Mapeo Físico)

### Módulo 1: Limpieza Estructural de la Raíz (/)
La raíz actual contiene muchos fragmentos y MOCs que colisionan con el verdadero Vault de Obsidian.
**Archivos Reubicados a 	rash/ (Papelera por Duplicidad o Error):**
- CONTRIBUTING.md (Duplicado del readme o de la Wiki).
- MOC_UI_UX_Complete.md (Ya existe sano dentro de Cerebro_Digital).
- solving_isolating_problem.md (Archivo descontinuado).
- UV_Unwrapping_Strategy.md (La documentación sana de técnicas CAD vive adentro del Cerebro Digital).

**Archivos Reubicados a rchive/ (Planes Desactualizados):**
- PLAN_ANIMACIONES_ONBOARDING.md (Plan del pasado sobre onboarding, obsoleto).

**Permanece intacto en Raíz:**
- README.md (Archivo de presentación del repositorio).
- dd_metadata.ps1 (Script de automatización auxiliar, puede dejarse o llevar a archivo log de scripts).

### Módulo 2: Unificación de Documentación en Obsidian (Cerebro_Digital/)
El mayor problema actual es que desarrollo/docs/ y External_docs/ compiten con Cerebro_Digital/ al tener también Markdowns conceptuales y de arquitectura que el Vault no puede buscar o indexar dinámicamente si están en subcarpetas separadas como repositorios de texto simple.

**Archivos Reubicados DESDE desarrollo/ HACIA Cerebro_Digital/Wiki/Concepts/:**
- desarrollo/ANALISIS_RECURSOS_DRON.md -> Se migra a Concepts.
- desarrollo/CORE_DIRECTIVE.md -> Al ser la directriz fundacional ("Antigravity persona"), se inyecta en el MOC Principal.
- desarrollo/ESTRUCTURA_CARPETAS.md -> Se fusionará / enviará a trash una vez que ESTE nuevo documento lidere la estructura.

**Archivos Reubicados a rchive/ (Modelos y hojas de ruta en conflicto temporal):**
- desarrollo/HOJA_DE_RUTA.md (de diciembre 2025).
- desarrollo/PLAN_MODELADO_DRON.md (de marzo 2026, ya consumado).
- Logs masivos en desarrollo/logs/ de fechas obsoletas.

**Lo que Permanece en Cerebro_Digital/:**
- Toda su estructura base (index.md, log.md, SYSTEM_SCHEMA.md).
- Todas sus subcarpetas Wiki/Concepts, Entities, Templates, etc.
- **Acción Correctora de Enlaces Internos:** En Obsidian existirá un subdirectorio "Stubs_NoResueltas". Dado que enviaremos ahí links rotos, los repararemos.

### Módulo 3: El Núcleo de Ingeniería (desarrollo/ y docs/)
Con la teoría y el PKM guardado a salvo en Obsidian (Cerebro_Digital), las carpetas estructurales quedan exclusivamente para el código, impidiendo contaminación de metadatos o colisiones en búsquedas globales en el IDE (VS Code).

**Estado Final de desarrollo/:**
- **Contendrá EXCLUSIVAMENTE** unity_project/.
- Dejará de tener README.md o variables sueltas. Si es estrictamente necesario, habrá un README.md que explique "Entorno Unity 6.x. Para documentación técnica diríjase al Cerebro Digital".

**Estado Final de docs/:**
- **Contendrá EXCLUSIVAMENTE** el entorno Vite Frontend y su build compilada.
- Elementos vitales a mantener: package.json, ite.config.js, main.js, styles.css, index.html.
- Subcarpetas intactas: Build/, src/.

### Módulo 4: Ecosistema LaTeX (Informe_final/ y Propuesta/)
El ecosistema académico no interfiere directamente con el código ni con Obsidian al nivel técnico/runtime, pero sí posee una arquitectura pesada propia del compilador LaTeX.

**Operaciones para LaTeX:**
- Mantendrán su estructura intacta chapters/, igures/, .tex, .aux, .toc.
- **Movimiento de basura a 	rash/:** Se descubrió que la carpeta 	rash/ actual ya alberga incontables copias residuales antiguas de Propuesta/ (logs de 2025). Su confinamiento allí se documenta y se reitera que ES la papelera oficial, por lo cual los archivos permanecerán en 	rash/ sin ser eliminados, sirviendo de respaldo muerto.

### Módulo 5: Bibliografía y Media (External_docs/, lender_files/, y resto)
- **External_docs/:** Alberga componentes puramente académicos o secundarios. Como no son "Notas Obsidian", su existencia externa a Cerebro_Digital es válida.
- **lender_files/, Datos/, Telemetria/:** Dada su naturaleza estática (.blend, .csv, metadata bruta) pueden consolidarse dentro de una mega-carpeta contenedora visualmente limpia llamada Entornos_3D_y_Datos (o mantener sus raíces pero reetiquetadas como estáticas).
- **MOC_Ecuaciones/:** Actualmente huérfana en la raíz. **Deberá moverse hacia Cerebro_Digital/Wiki/Concepts/** para que las ecuaciones vivan nutridas y parseadas matemáticamente (KaTeX/MathJax) dentro de Obsidian donde pertenecen.
- **Excalidraw/:** Misma situación. Se reubicará **dentro de Cerebro_Digital/** para que los diagramas sean embebibles usando el plugin de Obsidian.
