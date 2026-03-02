# Propuesta de Reorganización del Repositorio

Basado en la auditoría exhaustiva del proyecto (fase 1-3), se propone la siguiente reestructuración del repositorio para garantizar su escalabilidad, mantenimiento y limpieza académica antes de la defensa de la tesis.

## 1. Problemas Identificados en la Estructura Actual

1. **Auditorías Desactualizadas:** La carpeta `Informe_final/Desarrollo_App/Audits/` contiene archivos como `REMEDIATION_PLAN.md` que refieren a problemas críticos (Enum duplicados, Triple Events) que **ya fueron resueltos en el código**. Mantenerlos allí genera confusión.
2. **Duplicidad Documental:** Existen versiones superpuestas de documentos técnicos entre `Propuesta/` y `Informe_final/`.
3. **Assets Faltantes Sensibles:** Las carpetas del proyecto de Unity carecen por completo de una carpeta `Audio/` e ignoran recursos requeridos por el código.

## 2. Roadmap de Limpieza (Cleanup Roadmap)

El agente ejecutará las siguientes acciones si el usuario lo aprueba:

### Paso 1: Archivo de Deuda Técnica (Archive)
Mover todos los reportes de auditorías antiguas (ej. `REMEDIATION_PLAN.md`, `ARCHITECTURE_AUDIT_REPORT.md` viejos) a una nueva carpeta `Archive/Audits_Phase1/`. Esto preserva la historia sin contaminar la documentación actual.

### Paso 2: Consolidación de Documentación Técnica
Crear un único directorio raíz en `/Docs/` o consolidar firmemente dentro de `Informe_final/` las piezas finales, inyectándoles a todas el reporte final de métricas de la auditoría actual (e.g., 105+ scripts, 34 singletons, Unity 6000.0).

### Paso 3: Reestructuración en Unity
Asegurar que existan las carpetas faltantes dentro de `desarrollo/unity_project/Assets/`:
- `Assets/Audio/SFX/`
- `Assets/Audio/Music/`

Adicionalmente, se recomienda mover los scripts `*Loader*`, `*Optimizer*` al namespace `WebGL.Core.Infrastructure` para quitar peso conceptual a la carpeta de `Managers`.

## 3. Beneficios de esta Propuesta
- **Evita Confusión con Evaluadores:** Si un jurado lee el `REMEDIATION_PLAN.md` actual, pensará que el código base está roto, cuando en realidad el `CODEBASE_TRUTH` demuestra que es altamente eficiente y estable.
- **Preparación para Producción:** Establece una estructura limpia en Unity lista para empaquetar en WebGL de manera óptima.

---
**Acción requerida del usuario:** Aprobar este rediseño para proceder a ejecutarlo operativamente en la siguiente sesión (Mover archivos, crear directorios y purgar referencias obsoletas).
