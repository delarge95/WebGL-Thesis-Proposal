---
tipo: "moc"
fuente: "auditoria-conectividad"
estado: "activo"
area: trazabilidad
trace_id: TRC-MOC-AUTO-DASHBOARD_SALUD_ENLACES
---

# Dashboard Salud de Enlaces

> Monitorea archivos sin conexiones y con conectividad débil para mantenimiento continuo.

## Archivos Sin Inlinks
```dataview
TABLE file.link as Archivo, length(file.inlinks) as In, length(file.outlinks) as Out
FROM ""
WHERE length(file.inlinks) = 0
SORT file.path asc
```

## Archivos Con Outlinks Minimos
```dataview
TABLE file.link as Archivo, length(file.inlinks) as In, length(file.outlinks) as Out
FROM ""
WHERE length(file.outlinks) <= 1
SORT length(file.outlinks) asc, file.path asc
```

## Hubs Principales
- [[index]]
- [[MOC_Conectividad_Total]]
- [[MOC_Indice_Alfabetico_Global]]

