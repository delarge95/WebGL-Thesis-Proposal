---
tipo: catalogo_scripts
area: unity
estado: activo
trace_id: TRC-SCRIPTS-0001
resumen: "Catalogo semantico inicial de scripts Unity por categoria funcional"
---

# Catalogo de Scripts Unity

> Alcance inicial: scripts propios bajo `desarrollo/unity_project/Assets`.
> Nota: plugins de terceros quedan fuera del grafo funcional principal.
> Version completa y generada: [[CATALOGO_SCRIPTS_UNITY_COMPLETO]]

## Core/Managers (31)

- [AppStateMachine](desarrollo/unity_project/Assets/Scripts/Core/Managers/AppStateMachine.cs)
- [ViewModeManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/ViewModeManager.cs)
- [SelectionManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs)
- [CrossSectionManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/CrossSectionManager.cs)
- [PartVisibilityManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/PartVisibilityManager.cs)
- [PartCatalogManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/PartCatalogManager.cs)
- [AssemblyGuideManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/AssemblyGuideManager.cs)
- [BillOfMaterialsManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/BillOfMaterialsManager.cs)
- [AnalyticsManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/AnalyticsManager.cs)
- [QualityManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/QualityManager.cs)

## Core/Events (4)

- [EventBus](desarrollo/unity_project/Assets/Scripts/Core/Events/EventBus.cs)
- [CoreEvents](desarrollo/unity_project/Assets/Scripts/Core/Events/CoreEvents.cs)
- [ThermalEvents](desarrollo/unity_project/Assets/Scripts/Core/Events/ThermalEvents.cs)
- [StateChangedEvent](desarrollo/unity_project/Assets/Scripts/Core/Events/StateChangedEvent.cs)

## Core/Thermal (4)

- [ThermalSimulationManager](desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalSimulationManager.cs)
- [ThermalViewController](desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalViewController.cs)
- [ThermalSurfaceProfile](desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalSurfaceProfile.cs)
- [ThermalContactGraphAsset](desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalContactGraphAsset.cs)

## UI/Panels (12)

- [UIModeController](desarrollo/unity_project/Assets/Scripts/UI/Panels/UIModeController.cs)
- [UIAnalyzePanel](desarrollo/unity_project/Assets/Scripts/UI/Panels/UIAnalyzePanel.cs)
- [UICrossSectionPanel](desarrollo/unity_project/Assets/Scripts/UI/Panels/UICrossSectionPanel.cs)
- [UIEnvironmentPanel](desarrollo/unity_project/Assets/Scripts/UI/Panels/UIEnvironmentPanel.cs)
- [UIDetailsSheet](desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs)
- [OnboardingController](desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingController.cs)
- [OnboardingAnimationView](desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingAnimationView.cs)
- [InspectModeHandler](desarrollo/unity_project/Assets/Scripts/UI/Panels/InspectModeHandler.cs)
- [AnalyzeModeHandler](desarrollo/unity_project/Assets/Scripts/UI/Panels/AnalyzeModeHandler.cs)

## Core/Utils (9)

- [SceneBootstrapper](desarrollo/unity_project/Assets/Scripts/Core/Utils/SceneBootstrapper.cs)
- [ServiceLocator](desarrollo/unity_project/Assets/Scripts/Core/Utils/ServiceLocator.cs)
- [WebGLProfiler](desarrollo/unity_project/Assets/Scripts/Core/Utils/WebGLProfiler.cs)
- [ImportedDroneRuntimeBinder](desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs)
- [DroneAssembler](desarrollo/unity_project/Assets/Scripts/Core/Utils/DroneAssembler.cs)

## Editor (19)

- [ObsidianBuildTelemetryPostprocessor](desarrollo/unity_project/Assets/Editor/ObsidianBuildTelemetryPostprocessor.cs)
- [ImportDroneModel](desarrollo/unity_project/Assets/Editor/ImportDroneModel.cs)
- [ImportedDroneCoverageAudit](desarrollo/unity_project/Assets/Editor/ImportedDroneCoverageAudit.cs)
- [WebGLBuildFixer](desarrollo/unity_project/Assets/Editor/Antigravity/WebGLBuildFixer.cs)
- [WebGLBatchBuildEntry](desarrollo/unity_project/Assets/Editor/Antigravity/WebGLBatchBuildEntry.cs)

## Scripts criticos para trazabilidad de entregables

| Script ID    | Script                                                                            | Rol                                                | Entregables vinculados                                                                                                 |
| ------------ | --------------------------------------------------------------------------------- | -------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| SCR-APP-001  | [AppStateMachine](desarrollo/unity_project/Assets/Scripts/Core/Managers/AppStateMachine.cs)         | Control del estado global de la app                | [04_desarrollo](Informe_final/chapters/04_desarrollo.tex), [manual_tecnico](docs/manuals/manual_tecnico.md)                                              |
| SCR-VIEW-001 | [ViewModeManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/ViewModeManager.cs)         | Orquesta modos de vista (analyze, inspect, studio) | [04_desarrollo](Informe_final/chapters/04_desarrollo.tex), [manual_usuario](docs/manuals/manual_usuario.md)                                              |
| SCR-SEL-001  | [SelectionManager](desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs)        | Gestion de seleccion y contexto de pieza           | [04_desarrollo](Informe_final/chapters/04_desarrollo.tex), [MAPEO_UI_FIELD_BINDINGS_2026-04-15](desarrollo/docs/investigacion/Holybro/MAPEO_UI_FIELD_BINDINGS_2026-04-15.md) |
| SCR-THM-001  | [ThermalSimulationManager](desarrollo/unity_project/Assets/Scripts/Core/Thermal/ThermalSimulationManager.cs) | Simulacion termica principal                       | [04_desarrollo](Informe_final/chapters/04_desarrollo.tex), [INDICE_TERMICO](desarrollo/docs/sistema_termico/INDICE_TERMICO.md)                           |
| SCR-EVT-001  | [EventBus](desarrollo/unity_project/Assets/Scripts/Core/Events/EventBus.cs)                  | Backbone de eventos desacoplados                   | [02_Referencia_Tecnica_Modulos](Informe_final/Desarrollo_App/Documentacion_Tecnica/02_Referencia_Tecnica_Modulos.md), [ARCHITECTURE](docs/manuals/ARCHITECTURE.md)    |

## Script Cards iniciales (Sprint 1.1)

- [[SCR-APP-001_AppStateMachine]]
- [[SCR-VIEW-001_ViewModeManager]]
- [[SCR-SEL-001_SelectionManager]]
- [[SCR-CROSS-001_CrossSectionManager]]
- [[SCR-THM-001_ThermalSimulationManager]]
- [[SCR-EVT-001_EventBus]]
- [[SCR-UI-001_UIModeController]]
- [[SCR-ONB-001_OnboardingController]]
- [[SCR-ONB-002_OnboardingAnimationView]]
- [[SCR-SAVE-001_SaveSystem]]
- [[SCR-PERF-001_WebGLProfiler]]

## Script Cards adicionales (Sprint 1.1)

- [[SCR-CONT-001_MaterialController]]
- [[SCR-CONT-002_ExplodedViewManager]]
- [[SCR-CONT-003_HighlightSystem]]
- [[SCR-UI-002_UIManager]]
- [[SCR-UI-003_DetailsPanelController]]
- [[SCR-UI-004_HotspotManager]]
- [[SCR-UI-005_PartCatalogUI]]
- [[SCR-UI-006_EnhancedInfoPanel]]
- [[SCR-UI-007_SettingsPanel]]
- [[SCR-UI-008_LoadingController]]

## Estado de avance

- Sprint inicial de `script_card` criticos: completado (20 fichas).
- Version completa con cobertura de scripts C#, editor tools, microinteracciones y shaders: [[CATALOGO_SCRIPTS_UNITY_COMPLETO]].
- Plugins de terceros: clasificados como `tipo: sistema` fuera del grafo funcional principal.




