---
tipo: "norma_taxonomia"
fuente: "vault-global"
estado: "activo"
area: "trazabilidad"
trace_id: "TRC-TAX-0001"
---

# Taxonomia de Metadata Wiki

## Objetivo

Definir vocabulario controlado para frontmatter en notas activas de la wiki.

## Campos obligatorios (A2)

- tipo
- area
- estado
- trace_id

## Vocabulario controlado de tipo (A1)

- moc
- concepto
- auditoria
- visualizacion
- nota_consolidada
- script_card
- catalogo_scripts
- entregable_bridge
- matriz_trazabilidad
- registro_bibliografico
- norma_taxonomia

## Vocabulario controlado de area (A1)

- trazabilidad
- ux_ui
- testing_validacion
- sistema_termico
- webgl
- unity
- drone
- informe_final
- external_docs
- docs_manuals
- planning
- bibliografia
- transversal
- auditoria
- rag
- otros

## Reglas operativas

- Toda nota con estado: activo debe incluir los 4 campos obligatorios.
- Los valores de tipo y area deben pertenecer al vocabulario controlado.
- trace_id debe ser unico y estable por nota.
- Si una nota cambia de dominio, actualizar area sin perder trace_id.

## Enlaces de continuidad

- [[MOC_Conectividad_Total]]
- [[MOC_Indice_Alfabetico_Global]]

