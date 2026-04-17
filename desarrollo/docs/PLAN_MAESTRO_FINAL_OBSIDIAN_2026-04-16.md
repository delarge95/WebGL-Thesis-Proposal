---
tipo: plan_maestro
dominio: obsidian
estado: activo
fecha: 2026-04-16
objetivo_principal: "Integrar adjuntos, minimizar huerfanos y resolver notas no existentes con trazabilidad total"
---

# Plan Maestro Final de Organizacion Obsidian

## 1) Respuestas directas a tus dudas

### Que son las etiquetas (tags)

Las etiquetas son metadatos cortos para clasificar notas por tema o estado, por ejemplo:

- `#tipo/script_card`
- `#area/informe_final`
- `#estado/borrador`

Funcionan como filtros transversales. No reemplazan enlaces ni MOCs. En una red de conocimiento madura:

- Los enlaces responden "con que se relaciona".
- Las etiquetas responden "de que clase es".

### Que tan complejo es integrar todos los huerfanos y adjuntos

Complejidad real: media-alta, pero totalmente abordable por lotes.

Estado medido del vault:

- Adjuntos detectados (imagenes/PDF/media): 130.
- Notas Markdown detectadas: 393.

Conclusiones:

- Integrar 130 adjuntos es viable en 2 a 4 ciclos de trabajo (inventario -> enlazado minimo -> trazabilidad semantica).
- Reducir huerfanos al minimo es viable si se fuerza una regla: toda nota activa debe tener al menos 1 enlace entrante y 1 saliente, o pertenecer a un MOC.

### Que significa mostrar solo archivos existentes y por que aparecen nodos vacios

Cuando en Obsidian permites ver archivos no existentes, aparecen nodos "fantasma" porque existen enlaces wiki escritos como `[[Titulo X]]`, pero el archivo de esa nota nunca se creo.

Ejemplo de causa:

- En alguna nota escribiste `[[MOC_Flujo_Termico_Final]]`.
- Ese archivo no existe en disco.
- Obsidian dibuja el nodo como no resuelto.

No es un error del grafo: es deuda de contenido.

Como se soluciona:

1. Mantener temporalmente visibles los no resueltos para limpiarlos.
2. Para cada nodo fantasma: crear nota minima o corregir el enlace a la nota real existente.
3. Al cerrar el ciclo, activar filtro de "ocultar no resueltos" en el uso diario.

## 2) Objetivo de cierre

Dejar la boveda en un estado auditable donde:

- Adjuntos: 100% inventariados y enlazados al menos desde una nota o registro.
- Huerfanos: <= 5% de notas activas.
- No resueltos: 0 pendientes criticos (solo permitidos si estan en backlog controlado).
- Trazabilidad: toda afirmacion clave puede rastrearse a evidencia, entregable y fuente.

## 3) Plan operativo final (4 frentes, 8 pasos)

### Frente A - Control de taxonomia (tags + frontmatter)

Paso A1. Definir taxonomia unica de tags

- `#tipo/*` para tipo de nota.
- `#area/*` para dominio.
- `#estado/*` para ciclo de vida.

Paso A2. Estandarizar frontmatter minimo

Campos obligatorios en notas activas:

- `tipo`
- `area`
- `estado`
- `trace_id`

Resultado esperado:

- BÃºsqueda y Dataview consistentes en todo el vault.

### Frente B - Integracion total de adjuntos

Paso B1. Catalogo maestro de adjuntos

Crear o completar:

- `Cerebro_Digital/Wiki/Concepts/REGISTRO_ADJUNTOS_GLOBAL.md`

Columnas recomendadas:

- `asset_id`
- `ruta`
- `tipo_mime_aprox`
- `nota_propietaria`
- `trace_id_rel`
- `estado_enlace` (pendiente|enlazado|validado)

Paso B2. Regla de enlace minimo por adjunto

Cada adjunto debe cumplir al menos una:

- Estar embebido en una nota (`![[archivo.ext]]`).
- Estar referenciado en el registro global con nota propietaria.

Paso B3. Prioridad por impacto

Orden de integracion:

1. PDF de bibliografia y validacion.
2. Imagenes usadas en informe/manual/presentacion.
3. Media secundaria.

Resultado esperado:

- 100% de adjuntos con propietario semantico y trazabilidad minima.

### Frente C - Reduccion maxima de huerfanos

Paso C1. Censo de huerfanos reales

Excluir sistema y archivo historico (`archive/`, `trash/`, `.obsidian/`).

Paso C2. Estrategia de resolucion por lote

Para cada huerfano activo:

1. Enlazarlo desde su MOC de dominio.
2. Agregar 1 enlace saliente a evidencia o entregable.
3. Si no aporta valor, mover a archivado controlado (`estado: archivado`).

Paso C3. Puertas de calidad

Una nota activa no pasa si:

- no tiene `trace_id`, o
- no tiene al menos 2 enlaces semanticos utiles.

Resultado esperado:

- Huerfanos activos reducidos a <= 5%.

### Frente D - Cierre de no existentes (nodos fantasma)

Paso D1. Listado de enlaces no resueltos

Generar backlog unico:

- `desarrollo/docs/BACKLOG_NOTAS_NO_RESUELTAS_2026-04-16.md`

Paso D2. Resolucion asistida

Por cada item:

1. Crear stub minimo si el concepto es valido.
2. Fusionar/redirigir si era duplicado semantico.
3. Corregir typo en enlaces rotos.

Stub minimo recomendado:

- titulo
- resumen (2 a 4 lineas)
- `trace_id`
- enlaces a 1 MOC y 1 evidencia

Paso D3. Politica futura

- No crear nuevos enlaces `wiki` ambiguos sin decidir: crear nota ahora o registrar en backlog.

Resultado esperado:

- 0 enlaces no resueltos criticos.

## 4) Ejecucion sugerida por semanas

Semana 1:

1. Frente A completo.
2. Frente D (listado + 50% resolucion).

Semana 2:

1. Frente B (PDF e imagenes criticas).
2. Frente C (huerfanos en Concepts y Entities).

Semana 3:

1. Frente B restante.
2. Frente C restante.
3. Cierre de Frente D a cero critico.

## 5) KPIs de cierre final

- K-Adj-1: `% adjuntos con nota propietaria`.
- K-Adj-2: `% adjuntos con trace_id_rel`.
- K-Orf-1: `% notas activas huerfanas`.
- K-Res-1: `cantidad de enlaces no resueltos`.
- K-Qual-1: `% notas activas con frontmatter completo`.

Umbrales de aceptacion:

- K-Adj-1 >= 95%
- K-Adj-2 >= 90%
- K-Orf-1 <= 5%
- K-Res-1 = 0 criticos
- K-Qual-1 >= 90%

## 6) Gobernanza para que no vuelva a degradarse

Checklist obligatorio antes de cerrar cualquier nota nueva:

1. Tiene `trace_id`.
2. Tiene tipo/area/estado.
3. Tiene 1 enlace entrante (MOC o indice).
4. Tiene 1 enlace saliente (evidencia/entregable/fuente).
5. Si usa adjunto, esta registrado o embebido.

Con esta gobernanza, la red se mantiene coherente y trazable sin volver a inflarse en nodos vacios u huerfanos.
