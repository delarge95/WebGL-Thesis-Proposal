---
tipo: plan_operativo
area: bibliografia
estado: activo
trace_id: TRC-BIB-MVP-0001
resumen: "Estrategia de bibliografia por lotes sin bloquear el avance del proyecto"
---

# Plan Operativo de Bibliografia (MVP)

## Objetivo

Mantener trazabilidad academica fuerte sin depender de descargar el 100% de PDFs.

## Regla base

Una fuente se considera util para el proyecto si cumple al menos 1 de estos niveles:

- Nivel A (Completo): PDF local o HTML local + DOI/URL + nota de evidencia.
- Nivel B (Suficiente): DOI/URL verificable + cita formal + nota de evidencia.
- Nivel C (Minimo): referencia completa + enlace oficial + estado no_disponible justificado.

## Opcion futura de adjuntos

Cuando una fuente pase de Nivel B a Nivel A, actualizar su ficha con:

- `adjunto_pdf` o `adjunto_html`
- `fecha_adjunto`

Esto permite empezar rapido con trazabilidad y completar archivos locales despues, sin rehacer estructura.

## Lotes de trabajo recomendados

### Lote 1 (Rapido, alto impacto, 60-90 min)

Objetivo: asegurar fuentes oficiales y abiertas.

- unity2024memory
- unity2024webglinterop
- unity2024webglgettingstarted
- unity2024urp
- unityndsystemrequirements
- khronos2011webgl
- webassemblyspec
- nielsen2000fiveusers

### Lote 2 (Metodologia y UX, 90-120 min)

- brooke1996sus
- bangor2008sus
- bangor2009sus
- hart1988nasa
- hart2006nasatlx
- peffers2007dsrm
- hevner2004design

### Lote 3 (Graficos y fundamentos tecnicos, 90-120 min)

- kajiya1986rendering
- cook1982reflectance
- schlick1994brdf
- walter2007microfacet
- karis2013real
- yu2023survey
- fransson2024webgpu

### Lote 4 (Libros y fuentes con posible restriccion)

Si no hay acceso legal a PDF, marcar no_disponible y completar nota de evidencia.

- akenine2018realtime
- bowman2004ui3d
- mayer2005cambridge
- lazar2017hci
- norman2013doet
- paivio1986mental
- ware2012viz

## Evidencia minima por fuente (obligatoria)

Crear una nota por fuente con:

1. Bib key
2. Cita corta
3. DOI/URL
4. 2-4 bullets de aporte al proyecto
5. Entregables que respalda
6. Estado de acceso (completo/parcial/no_disponible)

## Criterio de avance semanal

- Semana activa valida si cierras 8-12 fuentes en estado util (Nivel A o B).
- Objetivo mensual: 30+ fuentes trazadas con evidencia minima.

## Resultado esperado

No te bloqueas por descarga total y mantienes cadena de trazabilidad util para tesis y RAG.
