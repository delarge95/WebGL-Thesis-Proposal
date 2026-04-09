# Plan de split en 5 commits + limpieza segura

Fecha: 2026-04-08

## Objetivo

Separar el estado actual en 5 bloques de commit por tema:

1. UI
2. Tooling
3. Data térmica
4. Modelos pesados
5. Docs

sin perder archivos y manteniendo el proyecto funcional.

---

## 0) Backup obligatorio (sin pérdida)

Ejecutar antes de tocar staging:

```powershell
git -C "e:\WebGL_tesis" switch -c wip/spaghetti-snapshot-2026-04-08
git -C "e:\WebGL_tesis" add -A
git -C "e:\WebGL_tesis" commit -m "wip: snapshot completo antes de split por bloques"
```

Opcional adicional:

```powershell
git -C "e:\WebGL_tesis" tag backup/spaghetti-2026-04-08
```

---

## 1) Bloque UI (commit 1)

### Incluye

- desarrollo/unity_project/Assets/Scripts/Core/Events/CoreEvents.cs
- desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs
- desarrollo/unity_project/Assets/Scripts/UI/HotspotManager.cs
- desarrollo/unity_project/Assets/Scripts/UI/SmartHotspot.cs
- desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs
- desarrollo/unity_project/Assets/Scripts/UI/Panels/AnalyzeModeHandler.cs
- desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs
- desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml

### Comandos

```powershell
git -C "e:\WebGL_tesis" add \
  "desarrollo/unity_project/Assets/Scripts/Core/Events/CoreEvents.cs" \
  "desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/HotspotManager.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/SmartHotspot.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/Panels/AnalyzeModeHandler.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs" \
  "desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml"

git -C "e:\WebGL_tesis" commit -m "feat(ui): unifica filtros canónicos y hotspots por grupos con contexto en details sheet"
```

---

## 2) Bloque Tooling (commit 2)

### Incluye

- desarrollo/unity_project/Assets/Editor/ImportDroneModel.cs
- desarrollo/unity_project/Assets/Editor/ImportDroneModel.cs.meta
- desarrollo/unity_project/Assets/Editor/SetupImportedDroneThermalTest.cs
- desarrollo/unity_project/Assets/Editor/SetupImportedDroneThermalTest.cs.meta
- desarrollo/unity_project/Assets/Editor/DronePartDataFixer.cs
- desarrollo/unity_project/Assets/Editor/DronePartDataFixer.cs.meta
- desarrollo/unity_project/Assets/Editor/ThermalTestSetup.cs
- desarrollo/unity_project/Assets/Editor/ThermalTestSetup.cs.meta
- desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs
- desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs.meta
- desarrollo/unity_project/Assets/Scripts/Core/Content/PartRenderCategory.cs
- desarrollo/unity_project/Assets/Scripts/Core/Content/PartRenderCategory.cs.meta
- desarrollo/unity_project/Assets/Scripts/Core/Content/DroneRenderResolver.cs
- desarrollo/unity_project/Assets/Scripts/Core/Content/DroneRenderResolver.cs.meta
- desarrollo/unity_project/Assets/Scripts/Core/Content/AuxiliaryExplodeOffset.cs
- desarrollo/unity_project/Assets/Scripts/Core/Content/AuxiliaryExplodeOffset.cs.meta

### Comandos

```powershell
git -C "e:\WebGL_tesis" add \
  "desarrollo/unity_project/Assets/Editor/ImportDroneModel.cs" \
  "desarrollo/unity_project/Assets/Editor/ImportDroneModel.cs.meta" \
  "desarrollo/unity_project/Assets/Editor/SetupImportedDroneThermalTest.cs" \
  "desarrollo/unity_project/Assets/Editor/SetupImportedDroneThermalTest.cs.meta" \
  "desarrollo/unity_project/Assets/Editor/DronePartDataFixer.cs" \
  "desarrollo/unity_project/Assets/Editor/DronePartDataFixer.cs.meta" \
  "desarrollo/unity_project/Assets/Editor/ThermalTestSetup.cs" \
  "desarrollo/unity_project/Assets/Editor/ThermalTestSetup.cs.meta" \
  "desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs" \
  "desarrollo/unity_project/Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs.meta" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/PartRenderCategory.cs" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/PartRenderCategory.cs.meta" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/DroneRenderResolver.cs" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/DroneRenderResolver.cs.meta" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/AuxiliaryExplodeOffset.cs" \
  "desarrollo/unity_project/Assets/Scripts/Core/Content/AuxiliaryExplodeOffset.cs.meta"

git -C "e:\WebGL_tesis" commit -m "feat(tooling): pipeline oficial de importación y binding runtime; depreca setup térmico experimental"
```

---

## 3) Bloque Data térmica (commit 3)

### Incluye

- desarrollo/unity_project/Assets/Core/Data/X500V2Generated/
- desarrollo/unity_project/Assets/Core/Data/X500V2Generated.meta
- desarrollo/unity_project/Assets/Resources/ThermalCanonicalContactGraph.asset
- desarrollo/unity_project/Assets/Resources/ThermalCanonicalContactGraph.asset.meta

### Opcional (solo si decides mantener legacy mocks)

- desarrollo/unity_project/Assets/Data/Parts/x500v2_esc_*.asset
- desarrollo/unity_project/Assets/Data/Parts/x500v2_esc_*.asset.meta
- desarrollo/unity_project/Assets/Data/Parts/x500v2_pdb.asset
- desarrollo/unity_project/Assets/Data/Parts/x500v2_pdb.asset.meta

### Comandos

```powershell
git -C "e:\WebGL_tesis" add \
  "desarrollo/unity_project/Assets/Core/Data/X500V2Generated" \
  "desarrollo/unity_project/Assets/Core/Data/X500V2Generated.meta" \
  "desarrollo/unity_project/Assets/Resources/ThermalCanonicalContactGraph.asset" \
  "desarrollo/unity_project/Assets/Resources/ThermalCanonicalContactGraph.asset.meta"

git -C "e:\WebGL_tesis" commit -m "feat(thermal-data): agrega dataset canónico X500V2 y grafo térmico de contactos"
```

---

## 4) Bloque Modelos pesados (commit 4)

### Incluye

- desarrollo/unity_project/Assets/Models/
- desarrollo/unity_project/Assets/Models.meta
- blender_files/drone_05-decimating_002.blend
- blender_files/welded/
- blender_files/References/
- blender_files/export_mvp.py

### Recomendación crítica

- No subir binarios >100MB al remoto estándar.
- Usar Git LFS o mantener en rama/dataset local.

### Comandos (si usas LFS)

```powershell
git -C "e:\WebGL_tesis" lfs install
git -C "e:\WebGL_tesis" lfs track "*.fbx"
git -C "e:\WebGL_tesis" lfs track "*.blend"
git -C "e:\WebGL_tesis" add .gitattributes

git -C "e:\WebGL_tesis" add \
  "desarrollo/unity_project/Assets/Models" \
  "desarrollo/unity_project/Assets/Models.meta" \
  "blender_files/drone_05-decimating_002.blend" \
  "blender_files/welded" \
  "blender_files/References" \
  "blender_files/export_mvp.py"

git -C "e:\WebGL_tesis" commit -m "chore(models): incorpora FBX/Blender source para pipeline de importación"
```

---

## 5) Bloque Docs (commit 5)

### Incluye

- desarrollo/unity_project/INVENTARIO_PIEZAS_TERMICO_FILTROS_HOTSPOTS.md
- desarrollo/docs/investigacion/Holybro/x500v2_blender_unity_crossref.md
- desarrollo/docs/investigacion/PLAN_MIGRACION_UNITY_6.md
- desarrollo/docs/sistema_termico/AGENT_HANDOFF_THERMAL.md
- desarrollo/docs/sistema_termico/PREPARACION_FBX_IMPORTADO.md
- Informe_final/Desarrollo_App/Audits/07_Diagrams_Workflow/
- Informe_final/Desarrollo_App/Audits/GUIA_Y_ANALISIS_AUDITS.md
- Informe_final/figures/
- MERMAID_PREVIEW_TEST.md
- git_log*.txt, git_push_error*.txt, git_show_*.txt, temp_git_utf8.txt, tmp_uss.txt, unpushed_stats.txt
- desarrollo/unity_project/Assets/UI/Styles/git_uss_log.txt

### Comandos

```powershell
git -C "e:\WebGL_tesis" add \
  "desarrollo/unity_project/INVENTARIO_PIEZAS_TERMICO_FILTROS_HOTSPOTS.md" \
  "desarrollo/docs/investigacion/Holybro/x500v2_blender_unity_crossref.md" \
  "desarrollo/docs/investigacion/PLAN_MIGRACION_UNITY_6.md" \
  "desarrollo/docs/sistema_termico/AGENT_HANDOFF_THERMAL.md" \
  "desarrollo/docs/sistema_termico/PREPARACION_FBX_IMPORTADO.md" \
  "Informe_final/Desarrollo_App/Audits/07_Diagrams_Workflow" \
  "Informe_final/Desarrollo_App/Audits/GUIA_Y_ANALISIS_AUDITS.md" \
  "Informe_final/figures" \
  "MERMAID_PREVIEW_TEST.md" \
  "git_log.txt" "git_log_utf8.txt" "git_push_error.txt" "git_push_error_utf8.txt" \
  "git_show_37c570c.txt" "git_show_5971d2a.txt" "temp_git_utf8.txt" "tmp_uss.txt" "unpushed_stats.txt" \
  "desarrollo/unity_project/Assets/UI/Styles/git_uss_log.txt" \
  "desarrollo/unity_project/Assets/UI/Styles/git_uss_log.txt.meta"

git -C "e:\WebGL_tesis" commit -m "docs: consolida inventario, handoff térmico, plan de migración y artefactos de auditoría"
```

---

## Limpieza final recomendada

1. Mantener logs temporales fuera de raíz en futuras iteraciones:
   - mover a `logs/dev/` o ignorarlos en `.gitignore`.
2. No mezclar en un mismo commit:
   - scripts runtime,
   - scripts editor,
   - assets generados,
   - binarios pesados,
   - documentación.
3. Flujo sugerido para nuevas tandas:
   - branch por tema,
   - snapshot WIP rápido,
   - PR por bloque.

---

## Unificación ya aplicada

- `Assets/Editor/ThermalTestSetup.cs` quedó como entrada legacy deprecada que redirige al flujo oficial `SetupImportedDroneThermalTest.PrepareImportedDrone()`.
- `Assets/Editor/DronePartDataFixer.cs` movió sus menús a `Legacy` y separó la creación de mocks en un comando independiente para evitar duplicación accidental de datos legacy en cada ejecución.
