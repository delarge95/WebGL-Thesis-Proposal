---
tipo: moc
area: rag
estado: activo
trace_id: TRC-RAG-0001
resumen: "Paquetes de contexto para consultas trazables sobre el grafo"
---

# MOC de Paquetes de Contexto RAG

> Esta pagina define como recuperar contexto por `trace_id`, `entregable_ids`, `script_ids` y `bib_keys`.

## Paquetes base

### Paquete por entregable
```dataview
TABLE trace_id, estado, resumen
FROM "Cerebro_Digital"
WHERE contains(entregable_ids, "INF-C04") OR contains(entregable_ids, "INF-C05") OR contains(entregable_ids, "MAN-USR-ARRANQUE")
SORT file.name ASC
```

### Paquete por script
```dataview
TABLE trace_id, script_path, entregable_ids, resumen
FROM "Cerebro_Digital/Wiki/Entities/Scripts"
WHERE trace_id
SORT file.name ASC
```

### Paquete por bibliografia
```dataview
TABLE bib_key, estado_descarga, entregable_ids, resumen
FROM "Cerebro_Digital/Wiki/Concepts"
WHERE tipo = "fuente"
SORT bib_key ASC
```

### Paquete por fase del plan
```dataview
TABLE trace_id, estado, resumen
FROM "desarrollo/docs"
WHERE contains(file.name, "PLAN_TRAZABILIDAD_CEREBRO_DIGITAL_2026-04-16") OR contains(file.name, "PLAN_OPERATIVO_BIBLIOGRAFIA_MVP_2026-04-16")
SORT file.name ASC
```

## Flujo de uso

1. Identificar el entregable objetivo.
2. Recuperar las notas puente de capitulo.
3. Incluir scripts y fuentes conectadas.
4. Verificar que la respuesta cite nodos internos y no solo texto libre.
## Enlaces de continuidad
- [[MOC_Conectividad_Total]]
- [[MOC_Indice_Alfabetico_Global]]


