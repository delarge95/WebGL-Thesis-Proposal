# Exhaustive Audit Report: Codebase Truth vs. Documentation

This report details the discrepancies found by comparing the established factual state of the codebase (`CODEBASE_TRUTH.md`) against the existing technical documentation, manuals, and thesis proposal.

## 1. Discrepancias Numéricas y Métricas

| Documento | Métrica | Valor Documentado | Valor Real (Codebase) | Severidad |
| :--- | :--- | :--- | :--- | :--- |
| `01_Arquitectura_del_Sistema.md` | Total Scripts Core+UI | 94 scripts | **105+ scripts** (incluyendo test/editor scripts) | Baja |
| `01_Arquitectura_del_Sistema.md` | Líneas de Código (Aprox) | ~12,750 | Más de 14,000 | Baja |
| `manual_tecnico.tex` | Total Scripts | 65+ scripts | **~100 scripts** | Alta |
| `manual_tecnico.tex` | Líneas de Código | 9,000+ líneas | **~14,000 líneas** | Alta |
| `resultados.tex` (Propuesta) | Total Scripts | 65+ scripts | **~100 scripts** | Alta |
| Múltiples documentaciones | Patrón Singleton | "25+ Managers" | **34 Singletons** instanciados | Media |

*Nota: Existe una desincronización grave entre el manual técnico/propuesta (basados en un estado muy anterior del proyecto) y el documento de arquitectura (más reciente pero aún inexacto).*

## 2. Discrepancias Arquitectónicas y de Implementación

### 2.1 UI Toolkit Architecture 
La documentación (`01_Arquitectura_del_Sistema.md`) afirma que el `UIManager` orquesta 6 sub-controladores perfectamente delegados (`_modeController`, `_detailsSheet`, `_heroController`, `_analyzePanel`, `_crossPanel`, `_envPanel`). 

**La Realidad (`CODEBASE_TRUTH.md`):**
1. `UIModeController` es un componente masivo, **monolítico de 574 líneas**, que acopla múltiples comportamientos lógicos en lugar de delegarlos limpiamente a sub-componentes.
2. La documentación no menciona los `LoadingOverlay.uxml` y `ErrorOverlay.uxml`, asumiendo erróneamente que esos manejos ocurren procedimentalmente en C#.

### 2.2 Versión de Unity
- `01_Arquitectura` especifica: **Unity 6.0 LTS**
- `manual_tecnico.tex` especifica: **Unity 6.3 LTS**
- `ProjectVersion.txt` especifica: **6000.0.62f1** (Unity 6 release). 
**Resolución requerida:** Estandarizar toda la documentación para decir formalmente "Unity 6 (6000.0.x)".

### 2.3 Sistema de Notificación y Eventos
El documento de arquitectura especifica que no existen "Triple Event Publishing" ni "Duplicate VisualMode". Las auditorias anteriores (e.g. `REMEDIATION_PLAN.md`) las marcaban como críticas, **pero el código base actual ya las tiene solucionadas nativamente**. Los reportes de auditorías viejas deben ser rotulados como resueltos.

## 3. Conformidad Funcional (Promesas vs. Realidad)

Afortunadamente, las promesas funcionales de la propuesta concuerdan fuertemente con la realidad del código:
- **Vista Explosionada:** Documentado y existente (`ExplodedViewManager` y `ExplodablePart` implementan interpolación espacial real).
- **Simulación de Encendido:** Prometido en `manual_usuario.tex` e implementado en `DroneStateController.cs` (arranque simulado, giro progresivo de hélices, sonido acoplado).
- **Cortes Transversales / Shaders PBR:** Prometido e implementado a nivel de URP (`CrossSectionManager.cs` y variables shader globales `_GlobalClipPlane`).
- **Compatibilidad WebGL:** El código usa efectivamente Coroutines en lugar de `System.Threading.Thread` para mantener el hilo principal libre.

### 3.1 Hallazgos Críticos Adicionales
- **Assets de Audio Faltantes:** A pesar de que la arquitectura y los managers como `AudioManager.cs` y `DroneStateController.cs` referencian clips de audio (Startup, Idle, Shutdown, Flying), **ningún archivo `.wav`, `.mp3` o `.ogg` existe en el directorio de `Assets`**. Los recursos físicos se han perdido o no fueron comiteados.
- **Validación SUS y NASA-TLX:** Los documentos en `Informe_final\Validacion` (`CUESTIONARIO_SUS.md` y `CUESTIONARIO_NASA_TLX.md`) son únicamente plantillas en blanco. Las pruebas de usabilidad prometidas en los objetivos **aún no se han ejecutado con usuarios reales**.

## Próximos Pasos (Fase 4 - Generation Plan)
1. **Unificar métricas:** Generar un conteo final y exacto con el *skill* nativo o script de auditoría e inyectarlo en todas partes (Proposal, Technical Manual, Architecture).
2. **Refactor UI?** Si bien no rompe la app, documentar la naturaleza monolítica del `UIModeController` o separar la documentación para reflejar que la vista Inspect/Analyze se maneja allí.
3. Actualizar la versión de Unity formal en los `.tex`.
4. Restituir Assets de Audio faltantes.
5. Ejecutar Pruebas de Usuario (Validación).
