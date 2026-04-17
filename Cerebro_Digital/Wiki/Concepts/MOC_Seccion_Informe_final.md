---
tipo: moc
fuente: seccion/Informe_final
estado: activo
trace_id: TRC-MOC-INF-0001
resumen: "Organizacion y filtrado de Informe_final por categorias documentales"
area: trazabilidad
---

# MOC Seccion: Informe_final

- Volver a catalogo: [[MOC_Secciones_del_Vault]]
- Global: [[MOC_Conectividad_Total]]
- Matriz completa: [[MATRIZ_TRAZABILIDAD_ENTREGABLES_COMPLETA]]

## 1) Documento maestro y cronograma

- [informe_final](Informe_final/informe_final.tex)
- [informe_final](Informe_final/informe_final.toc)
- [informe_final](Informe_final/informe_final.pdf)
- [[CRONOGRAMA]]

## 2) Puentes por capitulo (trazabilidad)

- [[TRC_INF_C01_Introduccion]]
- [[TRC_INF_C02_Marco_Referencia]]
- [[TRC_INF_C03_Marco_Metodologico]]
- [[TRC_INF_C04_Desarrollo]]
- [[TRC_INF_C05_Resultados]]
- [[TRC_INF_C06_Conclusiones]]
- [[TRC_INF_C07_Referencias]]
- [[TRC_INF_C08_Apendices]]

## 3) Auditorias

### 3.1 Auditorias generales

```dataview
LIST FROM "Informe_final/Audits"
SORT file.name ASC
```

### 3.2 Auditorias de Desarrollo_App

```dataview
LIST FROM "Informe_final/Desarrollo_App/Audits"
SORT file.path ASC
```

## 4) Documentacion tecnica

```dataview
LIST FROM "Informe_final/Desarrollo_App/Documentacion_Tecnica"
SORT file.name ASC
```

## 5) Validacion e instrumentos

```dataview
LIST FROM "Informe_final/validacion"
SORT file.name ASC
```

## 6) Presentacion y demo

```dataview
LIST FROM "Informe_final/presentation"
SORT file.name ASC
```

## 7) Hojas de ruta y planes de trabajo

```dataview
LIST FROM "Informe_final/Desarrollo_App/hojas_de_ruta"
SORT file.path ASC
```

## 8) Gestion operativa de desarrollo

- [[README]]
- [[BITACORA]]
- [[CHANGELOG]]
- [[SESSION_LOG]]
- [[PAQUETE_DE_ENTREGA]]
- [[TECHNOLOGY_STACK]]
- [[MATRIZ_DESCONEXIONES_APP_DOCUMENTACION]]
- [[CLEANUP_MANIFEST]]
- [[ALL_FILES_CONSOLIDATED]]

## 9) Filtros rapidos por tipo

- Auditorias: `path includes Informe_final/Audits OR Informe_final/Desarrollo_App/Audits`
- Documentacion tecnica: `path includes Informe_final/Desarrollo_App/Documentacion_Tecnica`
- Validacion: `path includes Informe_final/validacion`
- Presentacion: `path includes Informe_final/presentation`
- Rutas historicas: `path includes Informe_final/Desarrollo_App/hojas_de_ruta`


