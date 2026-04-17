---
tipo: plan_maestro
dominio: cerebro_digital
estado: activo
fecha: 2026-04-16
objetivo_principal: "Trazabilidad total entre investigacion, scripts Unity, bibliografia y entregables"
---

# Plan de Trazabilidad del Cerebro Digital (Ejecucion sin Unity)

## 1) Verificacion de factibilidad

Si, es posible cumplir tus objetivos con el estado actual del proyecto.

Evidencia en el repositorio:

- Base documental amplia en Propuesta, Informe_final y manuals ya existente.
- MOCs globales ya creados y operativos (incluye conectividad global).
- Inventario de scripts Unity disponible en `desarrollo/unity_project/Assets/**/*.cs` (137 archivos detectados, incluyendo plugins).
- Bibliografia existente en `Propuesta/references.bib` y `Propuesta/justificacion/bibliografia.md`.

Limites reales a considerar:

- Descargar "toda" la bibliografia en PDF no siempre es posible por licencias/paywall.
- Si no hay datos SUS/NASA-TLX reales, el panel solo puede trabajar con estructura y placeholders.
- La conexion semantica no aparece sola: requiere una capa de metadatos obligatorios y matrices de trazabilidad.

## 2) Problema esencial detectado

El vault tiene enlaces, pero no un contrato de trazabilidad uniforme.
Eso produce nodos que "abren poco contenido relevante" porque falta contexto estructurado.

Falta clave:

- IDs de trazabilidad estables.
- Relacion explicita Entregable -> Evidencia -> Script -> Fuente.
- Catalogo semantico de scripts (categoria, padres, dependencias, funciones clave).

## 3) Objetivo de arquitectura

Convertir Obsidian en un grafo util para RAG, donde cada respuesta pueda rastrearse a:

1. Documento de investigacion.
2. Script/codigo asociado.
3. Entregable impactado (Propuesta/Informe/Manual).
4. Fuente bibliografica.

## 4) Modelo de metadatos obligatorio (contrato minimo)

Agregar en todas las notas de trabajo activas:

```yaml
---
tipo: investigacion|entregable|script_card|fuente|moc|plan
area: propuesta|informe_final|manuales|unity|ux_ui|termico
estado: activo|borrador|archivado|sistema
trace_id: TRC-XXXX
entregable_ids: []
script_ids: []
evidencia_ids: []
bib_keys: []
resumen: ""
---
```

Reglas:

- `trace_id` obligatorio para cualquier nota clave.
- `entregable_ids` obligatorio en notas de investigacion que soportan un entregable.
- `script_ids` obligatorio cuando la nota describe comportamiento implementado.
- `bib_keys` obligatorio si hay afirmaciones tecnicas/cientificas.

## 5) Estructura de trazabilidad propuesta

Crear/estandarizar estos nodos de control:

- `Cerebro_Digital/Wiki/Concepts/MOC_Entregables_Global.md`
- `Cerebro_Digital/Wiki/Concepts/MOC_Scripts_Unity.md`
- `Cerebro_Digital/Wiki/Concepts/MOC_Bibliografia_Proyecto.md`
- `Cerebro_Digital/Wiki/Concepts/MATRIZ_TRAZABILIDAD_ENTREGABLES.md`
- `Cerebro_Digital/Wiki/Concepts/CATALOGO_SCRIPTS_UNITY.md`
- `Cerebro_Digital/Wiki/Concepts/REGISTRO_FUENTES_BIBLIOGRAFICAS.md`

## 6) Fases de ejecucion

### Fase A - Normalizacion documental (sin Unity, sin datasets reales)

Objetivo: limpiar estructura semantica.

Tareas:

1. Aplicar contrato YAML minimo a notas criticas.
2. Renombrar notas ambiguas internas (cuando aplique) para que el titulo sea informativo.
3. Enlazar cada entregable principal con 3-10 evidencias de investigacion.
4. Consolidar duplicados evidentes en rutas historicas con `estado: archivado`.

Criterio de salida:

- 100% de notas clave con `trace_id`.
- Cada entregable principal con enlaces salientes a evidencia.

### Fase B - Trazabilidad de entregables

Objetivo: conectar Propuesta, Informe final y Manuales con respaldo tecnico.

Tareas:

1. Construir `MATRIZ_TRAZABILIDAD_ENTREGABLES.md` con columnas:
   - Entregable
   - Seccion
   - Evidencia documental
   - Script/codigo
   - Fuente bibliografica
   - Estado validacion
2. Cubrir al menos:
   - Propuesta (justificacion, metodologia, bibliografia)
   - Informe_final (8 capitulos + validacion)
   - docs/manuals (manual tecnico y usuario)

Criterio de salida:

- 1 matriz central navegable con enlaces bidireccionales reales.

### Fase C - Grafo de scripts Unity (sin ejecutar Unity)

Objetivo: visibilidad de arquitectura de codigo para RAG.

Tareas:

1. Generar `CATALOGO_SCRIPTS_UNITY.md` por categorias:
   - Core/Managers
   - Core/Events
   - Core/Thermal
   - UI/Panels
   - Utils
   - Editor
2. Crear una ficha por script critico (`script_card`) con:
   - Clase principal
   - Responsabilidad
   - Dependencias (`using`, managers relacionados)
   - Eventos emitidos/consumidos
   - Entregables afectados
3. Excluir plugins de terceros del grafo funcional (DOTween y similares) o marcarlos `tipo: sistema`.

Criterio de salida:

- Scripts core conectados a notas de arquitectura y manuales.

### Fase D - Bibliografia y evidencia

Objetivo: base bibliografica util para tesis y RAG.

Tareas:

1. Unificar bibliografia en registro maestro (sin borrar origen):
   - `Propuesta/references.bib`
   - `Propuesta/justificacion/bibliografia.md`
2. Crear `REGISTRO_FUENTES_BIBLIOGRAFICAS.md` con:
   - bib_key
   - DOI/URL
   - estado_descarga (pendiente/parcial/completo/no_disponible)
   - entregables vinculados
3. Descargar en lote solo fuentes legales o abiertas y registrar fallback (resumen + URL + cita) cuando no haya PDF.

Criterio de salida:

- 100% de bib_keys usadas en entregables mapeadas en el registro.

### Fase E - Preparacion RAG sobre grafo

Objetivo: consultas LLM con contexto fiable y trazable.

Tareas:

1. Definir "paquetes de contexto" por pregunta:
   - Entregable target
   - Evidencias
   - Scripts relacionados
   - Fuentes bibliograficas
2. Añadir consultas Dataview para extraer contexto por `trace_id`, `entregable_ids` y `script_ids`.
3. Verificar respuestas con trazabilidad (cada respuesta debe enlazar fuentes internas).

Criterio de salida:

- Prompts de RAG reproducibles y auditables.

## 6.1 Estado de avance actual

El bloque A-E ya quedo cubierto operativamente con los artefactos generados en este sprint:

- Fase A: normalizacion documental y metadatos base.
- Fase B: matriz de trazabilidad de entregables expandida.
- Fase C: catalogo completo de scripts Unity y shaders.
- Fase D: registro bibliografico y lotes de evidencia.
- Fase E: MOC de paquetes de contexto RAG con consultas Dataview.

Artefactos de referencia:

- `Cerebro_Digital/Wiki/Concepts/CATALOGO_SCRIPTS_UNITY_COMPLETO.md`
- `Cerebro_Digital/Wiki/Concepts/MATRIZ_TRAZABILIDAD_ENTREGABLES_COMPLETA.md`
- `Cerebro_Digital/Wiki/Concepts/MOC_RAG_Context_Packages.md`

## 7) KPIs de control

- K1: Cobertura de metadatos = % de notas clave con `trace_id`.
- K2: Cobertura de entregables = % secciones de entregables con evidencia enlazada.
- K3: Cobertura de scripts core = % scripts core con ficha `script_card`.
- K4: Cobertura bibliografica = % bib_keys con estado y enlace.
- K5: Densidad de grafo util = nodos activos con >=2 enlaces semanticos.

Reporte de control y cierre de fase:

- `desarrollo/docs/KPI_CONTROL_TRAZABILIDAD_2026-04-16.md`

Estado de cierre del paso 7:

- Ejecutado y documentado con medicion K1-K5.
- Fase cerrada a nivel operativo para A-E, con brechas residuales explicitadas en K1 y K3 (definicion original de script_card).

## 8) Orden recomendado de ejecucion (practico)

Semana 1:

1. Fase A completa.
2. Esqueleto de Fase B.

Semana 2:

1. Fase B completa.
2. Fase C inicial (catalogo + 20 scripts criticos).

Semana 3:

1. Fase C completa (scripts core).
2. Fase D (registro bibliografico).

Semana 4:

1. Fase E (RAG + consultas Dataview).
2. Auditoria final de trazabilidad.

## 9) Primer sprint accionable (siguiente paso)

Implementar inmediatamente:

1. `MATRIZ_TRAZABILIDAD_ENTREGABLES.md` (estructura base).
2. `CATALOGO_SCRIPTS_UNITY.md` (indice inicial por carpetas).
3. `REGISTRO_FUENTES_BIBLIOGRAFICAS.md` (import desde references.bib).

Con eso se resuelve tu dolor principal: que el vault pase de "nodos sueltos" a "cerebro navegable con evidencia".

## 10) Plan maestro final de organizacion Obsidian

Para el cierre integral de adjuntos, huerfanos y notas no resueltas, ver:

- `desarrollo/docs/PLAN_MAESTRO_FINAL_OBSIDIAN_2026-04-16.md`
