# Repo Restructuring Proposal

## Objetivo

Proponer una estructura final del repositorio que sea clara para jurados, colaboradores técnicos y futuros mantenedores.

## Problema actual

El repositorio combina producto ejecutable, tesis, documentación técnica, materiales de presentación y artefactos históricos con fronteras todavía difusas. Eso complica la lectura académica y el mantenimiento operativo.

## Estructura propuesta

```text
/
|-- README.md
|-- docs/                         # Sitio publicado y build WebGL público
|-- portafolio_personal/          # Curaduría personal de piezas y material derivado para portafolio profesional
|-- desarrollo/
|   |-- unity_project/            # Proyecto Unity fuente
|   |-- docs/                     # Investigación y notas técnicas de trabajo
|   |-- tools/                    # Scripts auxiliares de build o mantenimiento
|-- Informe_final/
|   |-- chapters/                 # Capítulos LaTeX de tesis
|   |-- Manual_de_usuario/
|   |-- Manual_tecnico/
|   |-- Desarrollo_App/
|   |   |-- Documentacion_Tecnica/
|   |   |-- BITACORA.md
|   |   |-- CHANGELOG.md
|   |   |-- CLEANUP_MANIFEST.md
|   |   |-- REPO_RESTRUCTURING_PROPOSAL.md
|-- archive/                      # Material histórico o superseded con valor de trazabilidad
|-- trash/                        # Cuarentena temporal para candidatos a descarte no eliminados aún
```

## Criterios de organización

- `docs/` debe contener sólo lo necesario para publicación web o soporte directo del sitio.
- `portafolio_personal/` debe actuar como staging personal para extraer piezas de portafolio sin desplazar la fuente de verdad del proyecto.
- `desarrollo/` debe concentrar trabajo técnico operativo y material de construcción del prototipo.
- `Informe_final/` debe quedar como paquete académico autoconsistente.
- `archive/` debe absorber versiones desplazadas, reportes antiguos y material experimental que no debe desaparecer.
- `trash/` debe actuar como zona temporal de descarte reversible para archivos prescindibles que todavía no se desean eliminar.

## Beneficios esperados

- Menor ambigüedad entre producto, tesis y materiales auxiliares.
- Mejor lectura por parte de evaluadores externos.
- Menor riesgo de documentar rutas equivocadas en la tesis.
- Base más limpia para un eventual release final o versión pública curada.

## Plan sugerido por fases

1. Congelar la verdad documental actual y corregir referencias rotas.
2. Clasificar archivos con el `CLEANUP_MANIFEST.md`.
3. Crear `archive/` y `trash/` con criterios explícitos de uso.
4. Mover a `archive/` el material histórico claramente superseded y a `trash/` los candidatos prescindibles no eliminados.
5. Ajustar rutas en README, apéndices y documentación técnica.
6. Validar que GitHub Pages y el build Unity no dependan de rutas movidas.

## Riesgos

- Romper referencias internas de la tesis o del sitio web.
- Mover documentación aún citada por apéndices o manuales.
- Confundir material histórico con material de soporte vigente.
- Dejar `trash/` crecer sin criterios de revisión y convertirlo en un segundo archivo muerto.

## Recomendación

Aplicar la reestructuración en una fase separada, después de terminar la reconciliación documental y con una revisión explícita de referencias antes de cada movimiento.
