# Plan Maestro Final de Saneamiento Tesis-App-Portafolio

Estado: activo  
Fecha: 2026-04-11  
Alcance: entregables activos del cierre de tesis, UI visible de la app, manuales, documentacion tecnica y portafolio.

---

## 1. Respuesta al asesor sobre la propuesta

### Observacion 1: formalizacion metodologica insuficiente
- **Correccion aplicada:** la propuesta ahora explicita tipo de investigacion, enfoque mixto con predominio cuantitativo, marco DSR + prototipado iterativo, variables, definiciones conceptuales y operacionales, muestra, tareas, instrumentos y procedimiento de analisis.
- **Evidencia activa:** `Propuesta/sections/metodologia.tex`
- **Estado:** resuelto a nivel documental; la fase empirica final sigue pendiente de ejecucion.

### Observacion 2: falta operacionalizacion de variables
- **Correccion aplicada:** se fijan como variables dependientes la usabilidad percibida (SUS), la carga cognitiva (NASA-TLX Raw) y el rendimiento tecnico; como variable independiente, el tipo de visualizacion (3D interactiva vs soporte 2D); y como variables de control, experiencia, perfil y dispositivo.
- **Evidencia activa:** `Propuesta/sections/metodologia.tex`
- **Estado:** resuelto documentalmente.

### Observacion 3: falta procedimiento de analisis de datos
- **Correccion aplicada:** se documenta el calculo de SUS, NASA-TLX Raw, el analisis descriptivo, la comparacion entre condiciones, el uso de trazas think-aloud y el criterio de reporte de KPIs.
- **Evidencia activa:** `Propuesta/sections/metodologia.tex`
- **Estado:** resuelto documentalmente; faltan resultados reales post-freeze.

### Observacion 4: ajustes APA
- **Correccion aplicada:** se corrige `three.js`, se incorporan Brooke (1996) y Hart \& Staveland (1988), y se diferencian referencias repetidas de Unity con sufijos por a\~no.
- **Evidencia activa:** `Propuesta/sections/bibliografia.tex`, `Propuesta/references.bib`
- **Estado:** resuelto.

---

## 2. Matriz de correcciones por entregable

| Entregable | Problema principal | Accion activa | Estado |
| --- | --- | --- | --- |
| Propuesta | metodologia insuficiente | reescritura metodologica y APA | en curso / base ya corregida |
| Informe final cap. 4 | cifras obsoletas, TODOs, narrativa mezclada | reescritura segun build final visible | en curso |
| Informe final cap. 5 | placeholders sin datos reales | dejar bloqueado hasta freeze + usabilidad | en curso |
| Informe final cap. 6 | conclusiones infladas o incompletas | reescritura con limitaciones reales | en curso |
| Informe final cap. 8 | apendices obsoletos y claims inestables | saneamiento y reclasificacion de material | en curso |
| Manual de usuario | controles y pantallas incorrectas | reescritura desde UI real | en curso |
| Manual tecnico | modulos inexistentes y conteos mezclados | reescritura por capas reales | en curso |
| Documentacion tecnica | arquitectura inflada y legacy mezclado | rehacer 01 y 02 desde codigo real | en curso |
| Portafolio | sobreventa y conteos viejos | reanclar a evidencia verificable | en curso |

---

## 3. Matriz verdad app vs documentacion

### Convenciones canonicas
- `28` piezas canonicas de investigacion tomadas de `desarrollo/docs/investigacion/Holybro/x500v2_parts_data.json`
- `30` anchors de escena: `28` piezas + `x500v2_fastener_group` + `x500v2_misc_group`
- `257` renderers/colliders auditados en escena
- `257` assets `.asset` generados en `desarrollo/unity_project/Assets/Core/Data/X500V2Generated`
- scripts:
  - `95` runtime propios en `Assets/Scripts` excluyendo `Editor/Tests`
  - `103` scripts bajo `Assets` excluyendo tests
  - `129` `.cs` totales incluyendo editor/plugins

### Flujo visible de la build final
- Hero -> Explore -> seleccion -> bottom sheet
- modos visibles: `Inspect`, `Analyze`, `Studio`
- herramientas visibles:
  - hotspots
  - isolate
  - power/load
  - explode
  - cross-section
  - filtros por categorias
  - thermal con leyenda

### Implementado pero oculto
- measurement
- view modes o shaders no expuestos en la UI final
- segundo plano de corte
- partes del flujo termico avanzado no expuestas al usuario final

### Legacy o no integrado
- `PartCatalogUI`
- `SettingsPanel`
- `LoadingController`
- partes de `EnhancedInfoPanel`
- suite de ensamblaje, BOM, anotaciones y connections si no entran a la UI final

### Desconexiones obligatorias a corregir
- `16 piezas` no puede quedar como cifra final, salvo como antecedente historico marcado
- el manual de usuario no puede seguir describiendo `clic izquierdo orbit`, `F3`, `F12`, `E`, `1-6` ni catalogo visible
- la ficha de pieza real es `bottom sheet`, no panel lateral derecho
- el Hero no puede mostrar `DJI Phantom 4 Pro`, `Boston Dynamics Spot` ni `CubeSat 3U`
- el boton `VIEW PROJECT ON GITHUB` debe quedar cableado o dejar de presentarse como accion real
- el tooltip de Inspect no puede seguir prometiendo `Measure` si el boton esta oculto
- no debe usarse el conteo bruto de assets generados como taxonomia academica

---

## 4. Plan de diagramas y arquitectura

### Paquete de diagramas prioritario
1. arquitectura por capas del runtime real
2. bootstrap y saneamiento por `ImportedDroneRuntimeBinder`
3. estados reales de `AppStateMachine`
4. flujo input -> seleccion -> detalle -> bottom sheet
5. orquestacion `Inspect / Analyze / Studio`
6. pipeline de view modes y shaders
7. clipping global y cross-section
8. subsistema termico hibrido
9. normalizacion `28 / 30 / 257`
10. despliegue y verificacion

### Regla editorial
- las figuras de resultados SUS, NASA-TLX y KPIs salen del capitulo 4
- los capitulos nucleares solo documentan la build visible
- lo oculto, experimental o futuro pasa a anexos o trabajo futuro

---

## 5. Alineacion de portafolio

### Perfil objetivo
- junior / mid Technical Artist
- foco: `tools + shaders + optimization + technical visualization`

### Evidencia que si se puede vender
- visor WebGL final del Holybro X500 V2
- sistema de view modes y shaders
- thermal como visualizacion aplicada, no FEA
- tooling real de editor:
  - `ProjectSetupWizard`
  - `ImportedDroneCoverageAudit`
  - `ThermalContactGraphBuilderWindow`
- caso CAD -> Unity -> WebGL cuando termine el reimport final

### Claims que deben bajar de rango
- audio implementado
- pipeline de Houdini como ruta oficial final
- suite completa de ensamblaje expuesta
- metricas antiguas de scripts y piezas
- modulos no integrados o inexistentes

---

## 6. Riesgos vigentes del cierre
- faltan aplicar optimizaciones CAD/import pendientes
- falta rerun de auditoria jerarquica y cobertura tras reimport final
- audio aun no tiene assets cargados
- el capitulo 5 no puede cerrarse sin freeze + KPIs + SUS/NASA-TLX + think-aloud

### Decision por defecto
- si el audio no entra antes del freeze, se reclasifica como trabajo futuro y se retiran claims de audio final

---

## 7. Criterios de cierre
- no quedan TODOs, placeholders ni texto corrupto en entregables activos
- ningun documento activo presenta como final una funcion oculta, legacy o futura
- toda cifra academica parte de la convencion unica `28 / 30 / 257` y `95 / 103 / 129`
- la respuesta al tutor enlaza observacion -> correccion -> evidencia -> estado
- el portafolio final solo contiene afirmaciones verificables desde repo, build o capturas
