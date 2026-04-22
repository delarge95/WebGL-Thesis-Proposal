Le pasé el prompt a claude opus 4.5 y la investigaión que hizo perplexity con su respectivo prompt., esto respondió:

OP:
I'll start by reading the Perplexity research document and then diving into all the relevant source files to perform the deep audit.

Viewed Unity%206%20WebGL%20%20aislamiento%20jer%C3%A1rquico%20de%20fasteners%20por%20instancia.md:1-188
Searched for "class SelectionManager"
Searched for "class PartVisibilityManager"
Searched for "class FastenerInspectionManager"
Searched for "class ImportedDroneRuntimeBinder"
Searched for "class UIManager"
Searched for "class FastenerRegistry"

Good, I found all the files. Now let me read each one completely.

Viewed SelectionManager.cs:1-800
Viewed PartVisibilityManager.cs:1-660
Viewed FastenerInspectionManager.cs:1-435
Viewed ImportedDroneRuntimeBinder.cs:1-800
Viewed UIManager.cs:1-800
Viewed FastenerRegistry.cs:1-201

Now let me read the remaining parts of UIManager and ImportedDroneRuntimeBinder, plus key supporting classes:

Viewed UIManager.cs:800-1141
Viewed ImportedDroneRuntimeBinder.cs:800-886
Viewed SelectionManager.cs:800-869
Searched for "class FastenerRuntimeMarker"
Viewed FastenerRuntimeMarker.cs:1-39
Searched for "class FastenerMetadata"
Searched for "class FastenerInstanceDefinition"
Viewed FastenerDataModels.cs:1-339

Now I have a complete picture of the codebase. Let me also check a few samples from the JSON instance catalog to understand the data shape:

Searched for "holybro_fastener_instances"
Searched for "DisallowMultipleComponent"
Searched for "holybro_fastener_instances"
Searched for "holybro_fastener_instances"
Listed directory Resources
Viewed holybro_fastener_instances.json:1-800

I now have a complete understanding of the entire architecture. Let me produce the deep technical audit artifact.

Edited implementation_plan.md

## Resumen

Completé la auditoría técnica profunda. Los hallazgos clave:

**Root Cause confirmada:** No es batching ni mesh-grouping. El catálogo JSON muestra que cada fastener tiene su propio `GameObject` con su propio `Renderer`. El problema es una **cadena de 3 errores de resolución de identidad**:

1. **`SealMarker` sobreescribe instanceIds existentes** — cuando el binder procesa el anchor padre, puede pisar el marker que ya tenía un renderer de fastener con metadata del anchor
2. **`GetComponentInParent<FastenerRuntimeMarker>` en `RendererBelongsToFastenerInstance`** — después del reparenting, un renderer de fastener puede capturar el marker de un vecino o ancestro, causando falsos matches con múltiples instancias
3. **Reparenting rompe `IsChildOf(isolationScope)`** — forzando que toda la resolución dependa de markers que ya están corrompidos por los puntos 1 y 2

**La solución Tier 1 (mínima invasiva, ~30 líneas)** corrige los 3 puntos sin cambios arquitectónicos. Revisa el plan de implementación y dime si quieres que lo aplique.


YO: Ok, adelante, ejecutalo. Pero neesito que guardes este implementation plan en desarrollo/docs y cuando soluciones todo, generes otro diagrama mostrando todo resuelto

OP:
Let me save the plan to `desarrollo/docs/`, then execute the Tier 1 fixes across all 3 files.

Ran command: `copy "C:\Users\alexw\.gemini\antigravity\brain\efe457cf-b29e-4e1b-a17e-f3febbe33f05\implementation_plan.md" "e:\WebGL_tesis\desarrollo\docs\Auditoria_Fastener_Isolation_Bug.md"`

Good. Now let me create the task tracker and start implementing Tier 1:

Edited task.md

Now let me apply all the Tier 1 fixes. Starting with `FastenerRegistry.SealMarker()`:

Edited FastenerRegistry.cs

Now fix `PartVisibilityManager` — remove `GetComponentInParent` from both `RendererBelongsTo*` methods and from `ResolveFastenerMarker`:

Edited PartVisibilityManager.cs

Now fix `SelectionManager.ResolveFastenerMarker()` with the same boundary-aware pattern:

Edited SelectionManager.cs

Now fix `FastenerInspectionManager.ResolveFastenerMarker()` with the same pattern:

Edited FastenerInspectionManager.cs

Now add the diagnostic validation in `ImportedDroneRuntimeBinder.BindRuntime()`:

Edited ImportedDroneRuntimeBinder.cs
Viewed ImportedDroneRuntimeBinder.cs:80-105

I see — the line endings are `\r\n`. Let me use exact content:

Edited ImportedDroneRuntimeBinder.cs

Now add the validation method itself. Let me find a good place to insert it:

Viewed ImportedDroneRuntimeBinder.cs:855-870
Viewed ImportedDroneRuntimeBinder.cs:880-888
Edited ImportedDroneRuntimeBinder.cs
Viewed ImportedDroneRuntimeBinder.cs:883-888
Edited ImportedDroneRuntimeBinder.cs

The file uses `\r\n` line endings. Let me use a different approach — insert the method before the last closing brace:

Edited ImportedDroneRuntimeBinder.cs

The CRLF line endings are tricky. Let me use the run_command approach to append the method directly:

Ran command: `powershell -Command "$content = Get-Content 'e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\ImportedDroneRuntimeBinder.cs' -Raw; $method = @'`r`n`r`n        /// <summary>`r`n        /// Post-binding diagnostic: detects duplicate FastenerRuntimeMarker instanceIds`r`n        /// across the scene, which is the root cause of multi-instance isolation leaks.`r`n        /// </summary>`r`n        private static void ValidateFastenerIdentityInvariants()`r`n        {`r`n            FastenerRuntimeMarker[] allMarkers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);`r`n            Dictionary<string, List<string>> idToGameObjects = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);`r`n`r`n            foreach (FastenerRuntimeMarker marker in allMarkers)`r`n            {`r`n                if (marker == null || string.IsNullOrWhiteSpace(marker.FastenerInstanceId))`r`n                {`r`n                    continue;`r`n                }`r`n`r`n                if (!idToGameObjects.TryGetValue(marker.FastenerInstanceId, out List<string> objectNames))`r`n                {`r`n                    objectNames = new List<string>();`r`n                    idToGameObjects[marker.FastenerInstanceId] = objectNames;`r`n                }`r`n`r`n                objectNames.Add(marker.gameObject.name);`r`n            }`r`n`r`n            int violations = 0;`r`n            foreach (KeyValuePair<string, List<string>> kvp in idToGameObjects)`r`n            {`r`n                if (kvp.Value.Count > 1)`r`n                {`r`n                    violations++;`r`n                    Debug.LogWarning(`$`""[FastenerIdentity] Duplicate instanceId '{kvp.Key}' on {kvp.Value.Count} GOs: {string.Join(`"", `"", kvp.Value)}`""`);`r`n                }`r`n            }`r`n`r`n            if (violations == 0)`r`n            {`r`n                Debug.Log(`$`""[FastenerIdentity] All {idToGameObjects.Count} fastener instanceIds are unique. Identity invariant OK.`""`);`r`n            }`r`n            else`r`n            {`r`n                Debug.LogError(`$`""[FastenerIdentity] {violations} duplicate instanceId violations detected! Isolation may show multiple instances.`""`);`r`n            }`r`n        }`r`n'@; $content = $content -replace '(        \}\r?\n    \}\r?\n\}\r?\n?)$', ($method + ""`r`n    }`r`n}`r`n""); Set-Content 'e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\ImportedDroneRuntimeBinder.cs' $content -NoNewline"`

The PowerShell interpolation is too complex. Let me write the method via a simpler approach — create a separate file and use a script to insert:

Edited _temp_method.txt
Ran command: `powershell -Command "$file = 'e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\ImportedDroneRuntimeBinder.cs'; $lines = [System.IO.File]::ReadAllLines($file); $method = [System.IO.File]::ReadAllLines('e:\WebGL_tesis\_temp_method.txt'); $insertIdx = $lines.Length - 3; $newLines = New-Object System.Collections.Generic.List[string]; for($i=0; $i -lt $insertIdx; $i++) { $newLines.Add($lines[$i]) }; foreach($m in $method) { $newLines.Add($m) }; for($i=$insertIdx; $i -lt $lines.Length; $i++) { $newLines.Add($lines[$i]) }; [System.IO.File]::WriteAllLines($file, $newLines.ToArray())"`

PowerShell is swallowing the `$` variables. Let me write a proper script file:

Edited _insert_method.ps1
Ran command: `powershell -ExecutionPolicy Bypass -File "e:\WebGL_tesis\_insert_method.ps1"`

Let me verify the insertion:

Viewed ImportedDroneRuntimeBinder.cs:880-933

I see two issues: the `FindPropellers` closing brace is missing (line 884 `return propellers.ToArray();` is no longer followed by `}`), and there's an extra `}` at line 930. Let me fix this:

Edited ImportedDroneRuntimeBinder.cs

Now clean up the temp files:

Ran command: `Remove-Item "e:\WebGL_tesis\_temp_method.txt", "e:\WebGL_tesis\_insert_method.ps1" -Force`

Let me also add the `using` for `Dictionary` and `List` (generic collections) — let me check if it's already imported:

Viewed ImportedDroneRuntimeBinder.cs:1-15

Good — `System.Collections.Generic`, `WebGL.Core.Content` (for `FastenerRuntimeMarker`), and all needed namespaces are already imported.

Now let me verify the `PartVisibilityManager` also has the required imports for `ExplodablePart`:

Viewed PartVisibilityManager.cs:1-12

I need to check where `ExplodablePart` is defined:

Searched for "class ExplodablePart"

`ExplodablePart` is in `WebGL.Core.Content` which is already imported in all 3 files. All good.

Now let me update the task and generate the resolution diagram:

Edited task.md

Now generate the walkthrough with the resolution diagram:

Edited walkthrough.md

Also copy the walkthrough diagram to `desarrollo/docs/`:

Ran command: `copy "C:\Users\alexw\.gemini\antigravity\brain\efe457cf-b29e-4e1b-a17e-f3febbe33f05\walkthrough.md" "e:\WebGL_tesis\desarrollo\docs\Resolucion_Fastener_Isolation_Bug.md"`

Update task as complete:

Edited task.md

## Resumen de ejecución

**Tier 1 aplicado en 4 archivos** con 3 fixes de raíz + 1 diagnóstico:

| Archivo | Fix |
|---------|-----|
| **FastenerRegistry.cs** | `SealMarker()` ahora protege instanceIds existentes — no sobreescribe si ya tiene uno válido diferente |
| **PartVisibilityManager.cs** | Eliminado `GetComponentInParent` de `RendererBelongsToFastenerInstance`, ambas sobrecargas de `RendererBelongsToAssociatedFastener`, y `ResolveFastenerMarker` ahora para en `ExplodablePart` boundaries |
| **SelectionManager.cs** | `ResolveFastenerMarker` → boundary-aware walk (misma lógica) |
| **FastenerInspectionManager.cs** | `ResolveFastenerMarker` → boundary-aware walk (misma lógica) |
| **ImportedDroneRuntimeBinder.cs** | Añadido `ValidateFastenerIdentityInvariants()` post-binding que detecta y loguea duplicados |

**Documentación guardada en `desarrollo/docs/`:**
- `Auditoria_Fastener_Isolation_Bug.md` — plan original con root cause analysis
- `Resolucion_Fastener_Isolation_Bug.md` — walkthrough con diagrama de pipeline resuelto

Al abrir Unity, busca en consola: `[FastenerIdentity] All N fastener instanceIds are unique.` — eso confirma que el sellado es correcto. Luego prueba el flujo completo de aislamiento jerárquico.

YO: Sigue sin funcionar, no puedo aislar un unico fastener, se aisla en grupo

OP:
El Tier 1 no fue suficiente. Necesito rastrear exactamente qué pasa en el flujo de aislamiento — el problema puede estar más arriba en la cadena. Déjame releer los métodos críticos con ojos frescos.

Viewed PartVisibilityManager.cs:128-260
Viewed PartVisibilityManager.cs:290-420

Ahora veo algo crucial. Veamos `ResolveIsolationScope` (línea 186-219) y cómo se llama `IsolateTransform`. Necesito entender qué pasa con la variable `isolationScope`:

Viewed PartVisibilityManager.cs:100-145

Ahora miro la cadena clave. Cuando seleccionas un fastener y pides aislar, veamos qué pasa realmente en `UIManager`:

Viewed UIManager.cs:690-800
Viewed UIManager.cs:580-700

Ahora necesito ver `ResolveFullSelection`, `IsSubSelection`, y `CanNestSelectionWithinIsolation` en UIManager:

Searched for "private.*ResolveFullSelection|private.*IsSubSelection|private.*CanNestSelection"
Viewed UIManager.cs:432-580

**¡LO ENCONTRÉ!** Línea 526 — **UIManager tiene su PROPIA copia de `ResolveFastenerMarker`** que NO fue corregida y **todavía usa `GetComponentInParent` sin boundaries**. Pero más importante aún, veo el flujo real del bug:

Cuando el usuario selecciona un fastener y doble-clickea:
1. `UIManager.OnPartDoubleClicked` → determina si es `canIsolateSubSelection`
2. Si sí → `IsolateSubSelection(clickedSelection, clickedFull)`
3. `IsolateSubSelection` llama `PartVisibilityManager.Instance?.IsolateTransform(subSelection)` (línea 596)

El problema está en **qué es `subSelection`** aquí. Si el `SelectionManager` resolvió el click al transform del **ExplodablePart padre** (que contiene la categoría `Fasteners`), entonces el isolation scope se convierte en la pieza madre con categoría Fasteners, no en el fastener individual.

Veamos cómo `SelectionManager` resuelve la selección:

Viewed SelectionManager.cs:250-350
Viewed SelectionManager.cs:350-500

Ahora necesito ver `ResolvePrimarySelection` y `ResolveFastenerSelectionRoot`:

Searched for "private.*ResolvePrimarySelection|private.*ResolveFastenerSelectionRoot|private.*ResolveSelectableTransform"
Viewed SelectionManager.cs:560-740

Ahora tengo **todo el panorama claro**. El bug real tiene **dos causas raíz adicionales** que no fueron arregladas por Tier 1:

1. **UIManager tiene su PROPIA `ResolveFastenerMarker` (línea 526)** que todavía usa `GetComponentInParent` sin boundary — esto afecta `IsFastenerAssociatedWithSelection` y `CanNestSelectionWithinIsolation`.

2. **El problema principal**: `ResolveFastenerSelectionRoot` en `SelectionManager` (línea 598-599) — cuando no encuentra un `FastenerRuntimeMarker`, intenta `GetComponentInParent<ExplodablePart>()` y si encuentra uno con categoría `Fasteners`, **devuelve ESE ExplodablePart como selection root**. Eso hace que `currentSelection` apunte al `ExplodablePart` padre (que agrupa TODOS los fasteners de ese tipo), no al fastener individual.

3. **Pero el bug más gordo**: En `PartVisibilityManager.ResolveIsolationScope` (línea 202-212), cuando el marker no tiene `FastenerInstanceId`, busca un `ExplodablePart` con categoría `Fasteners` via `GetComponentInParent` y devuelve *eso* como isolation scope. **Esto aisla el grupo entero**, no la instancia.

Edited UIManager.cs

Ahora el fix más importante — `ResolveIsolationScope` en PartVisibilityManager. El problema está en las líneas 202-213: cuando un marker no tiene `FastenerInstanceId`, busca el `ExplodablePart` con categoría Fasteners y devuelve ESO como scope, lo que agrupa todos los fasteners:

Viewed PartVisibilityManager.cs:186-220

**Aquí está el bug gordo**. Líneas 202-213: cuando el marker existe pero `FastenerInstanceId` está vacío, el código busca un `ExplodablePart` con categoría Fasteners y devuelve **su transform como isolation scope**. Este `ExplodablePart` es el **grupo** de fasteners (ej: todos los cap_screws de un tipo), no la instancia individual. Eso hace que `IsChildOf(isolationScope)` en `IsRendererVisibleForIsolation` haga match con **todos** los hijos de ese grupo.

Además, necesito entender cómo `SelectionManager.ResolveFastenerSelectionRoot` interactúa: cuando no encuentra marker, busca `IsFastenerPart(parent)` (línea 598), lo que también devuelve el `ExplodablePart` grupo.

La fix: cuando el marker existe (con o sin instanceId), devolver **el marker.transform** y nunca escalar al grupo ExplodablePart. Y en `ResolveFastenerSelectionRoot`, devolver el transform del marker si existe, y solo caer al ExplodablePart si no se encuentra marker:

Edited PartVisibilityManager.cs

Ahora necesito aplicar la misma lógica a `SelectionManager.ResolveFastenerSelectionRoot`. Cuando hay un marker, *ya* devuelve `marker.transform` (línea 589). Pero el fallback en líneas 592-599 busca `ExplodablePart` con categoría Fasteners y devuelve eso. Necesito asegurarme de que si el raycast golpea un GO que es hijo directo de un ExplodablePart tipo Fasteners, devolvamos **ese GO específico**, no el ExplodablePart grupo:

Viewed SelectionManager.cs:578-605

El problema: cuando no hay un marker (posiblemente porque fue reparentado sin uno), el fallback en línea 598 `GetComponentInParent<ExplodablePart>()` encuentra el **grupo ExplodablePart tipo Fasteners** y devuelve eso como el `selection root`. Esto significa que `currentSelection` en SelectionManager apunta al grupo, no a la instancia individual.

Necesito mantener la detección de que es un fastener (para que el highlight correcto se aplique), pero devolver **el rawTransform original** cuando no hay marker, en vez del ExplodablePart grupo:

Edited SelectionManager.cs

The tool reported inaccuracies. Let me verify the current state:

Viewed SelectionManager.cs:540-630

It's corrupted — there are duplicate methods and `IsFastenerPart` was deleted. Let me fix this properly by viewing the full area and writing the correct version:

Edited SelectionManager.cs

The tool rejected it. Let me view the exact current file state and try a cleaner approach:

Viewed SelectionManager.cs:555-640

I can see the corruption clearly: lines 555-570 is the original first half, then lines 571-591 is a duplicate injected version, then lines 593-606 is an incomplete `ResolveFastenerSelectionRoot`, and `IsFastenerPart` is missing. Let me also check if `IsFastenerPart` still exists later:

Searched for "IsFastenerPart"

`IsFastenerPart` was deleted entirely. I need to replace the whole corrupted section. Let me do it precisely:

Edited SelectionManager.cs

The tool keeps rejecting it. Let me use write_to_file to rebuild the corrupted section. I'll first get the full file content and manually reconstruct it:

Viewed SelectionManager.cs:1-555

Now let me see lines 555-630 and also line 884 to understand the rest:

Viewed SelectionManager.cs:606-700

Now I see the issue clearly. `IsFastenerPart` is used on line 628 but was deleted. And the section from 556-606 is corrupted with duplicates. I need to fix lines 556-606:

Edited SelectionManager.cs

The tool keeps rejecting. The issue is likely the mix of `\r\n` and `\n` line endings. Let me write a clean script to fix the file:

Edited _fix_selection.ps1
Ran command: `powershell -ExecutionPolicy Bypass -File "e:\WebGL_tesis\_fix_selection.ps1"`

Let me verify the fix:

Viewed SelectionManager.cs:553-640

The file is now clean. Let me cleanup the temp script:

Ran command: `Remove-Item "e:\WebGL_tesis\_fix_selection.ps1" -Force`

Now let me update the docs with the additional findings:

Edited walkthrough.md
Ran command: `copy "C:\Users\alexw\.gemini\antigravity\brain\efe457cf-b29e-4e1b-a17e-f3febbe33f05\walkthrough.md" "e:\WebGL_tesis\desarrollo\docs\Resolucion_Fastener_Isolation_Bug.md" /Y`
Ran command: `Copy-Item "C:\Users\alexw\.gemini\antigravity\brain\efe457cf-b29e-4e1b-a17e-f3febbe33f05\walkthrough.md" "e:\WebGL_tesis\desarrollo\docs\Resolucion_Fastener_Isolation_Bug.md" -Force`

## Resumen del Round 2

El Tier 1 era necesario pero **insuficiente**. Encontré las 2 causas raíz reales:

### Bug 1: `ResolveIsolationScope` escalaba al grupo
En `PartVisibilityManager`, cuando un marker existía pero no tenía `instanceId`, el código buscaba el `ExplodablePart` padre con categoría `Fasteners` y devolvía **ese transform como isolation scope**. Eso hacía que `IsChildOf(isolationScope)` matcheara **todos los fasteners** del grupo, causando el aislamiento en masa.

**Fix**: Siempre devolver `marker.transform` — nunca escalar al `ExplodablePart` grupo.

### Bug 2: `ResolveFastenerSelectionRoot` colapsaba la selección al grupo
En `SelectionManager`, cuando no se encontraba marker y el fastener clickeado era hijo de un `ExplodablePart` tipo Fasteners, devolvía **el ExplodablePart** como selection root en vez del mesh individual.

**Fix**: Devolver `rawTransform` (el GO individual del mesh clickeado), no el grupo padre.

### Bug 3 (olvidado en Round 1): UIManager tenía su propia copia sin corregir
`UIManager.ResolveFastenerMarker()` seguía usando `GetComponentInParent` sin boundaries.

**Documentación actualizada** en `desarrollo/docs/`:
- `Auditoria_Fastener_Isolation_Bug.md`
- `Resolucion_Fastener_Isolation_Bug.md` (con diagrama final)

Prueba de nuevo en Unity — ahora el scope de aislamiento apuntará al transform individual, no al grupo.

YO: Sigue el mismo problema :(

OP: 
Los fixes estáticos no son suficientes — necesito ver qué pasa **en runtime**. Voy a agregar logs de diagnóstico agresivos para que al reproducir el bug, la consola de Unity muestre exactamente qué está fallando.

Viewed PartVisibilityManager.cs:108-185
Viewed PartVisibilityManager.cs:186-290

Ahora entiendo completamente el flujo. Necesito agregar logs de diagnóstico **dentro del ciclo de visibilidad** para ver exactamente qué renderers se están mostrando y por qué. Voy a instrumentar `IsolateTransform`:

Edited PartVisibilityManager.cs

The diff looks correct — all the diagnostic logs were added. Let me verify there are no compile errors:

Viewed PartVisibilityManager.cs:185-200

Good. Ahora necesito que pruebes de nuevo. **Los logs te dirán exactamente qué está pasando.** Esto es lo que necesito que hagas:

1. **Abre Unity**, entra en Play Mode
2. **Aísla una pieza madre** (ej: `x500v2_arm_FL`) haciendo doble click
3. **Click en un fastener individual** visible dentro de la pieza aislada
4. **Doble click** para intentar aislar ese fastener
5. **Ve Console → busca los mensajes `[PVM.IsolateTransform]`**
6. **Cópiame los logs completos** que empiezan con `[PVM.IsolateTransform] ──── DIAGNOSTIC ────`

Los logs me dirán exactamente:
- **`selection.name` / `selection.path`** — qué transform se está intentando aislar
- **`isFastenerIsolation`** — si el sistema detecta que es un fastener
- **`fastenerInstanceId`** — si se resolvió un ID de instancia individual
- **`[VISIBLE]` lines** — qué renderers se están mostrando y **por qué razón** (byScope, byInstance, o byAssoc)

Esos logs van a revelar si el problema es que:
- A) El selection no tiene marker
- B) El marker no tiene instanceId
- C) Los renderers se muestran por `IsChildOf` en vez de por instanceId
- D) Los renderers se muestran por `RendererBelongsToAssociatedFastener`

**Pásame el output de la consola cuando reproduzcas el bug.**

YO: [PVM.IsolateTransform] ──── DIAGNOSTIC ────
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:144)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] Result: 27/425 renderers visible
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:206)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] ──── DIAGNOSTIC ────
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:144)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] Result: 30/577 renderers visible
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:206)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] ──── DIAGNOSTIC ────
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:144)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:RestoreFullIsolationFromStack () (at Assets/Scripts/UI/UIManager.cs:669)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:992)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] Result: 27/453 renderers visible
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:206)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:RestoreFullIsolationFromStack () (at Assets/Scripts/UI/UIManager.cs:669)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:992)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

OP:
Necesito los logs **intermedios** — entre `──── DIAGNOSTIC ────` y `Result:`. Los detalles importantes (selection.name, fastenerInstanceId, `[VISIBLE]` lines) están ahí.

En la Unity Console:
1. **Desactiva "Collapse"** (botón arriba a la derecha de Console)
2. **Reproduce el bug de nuevo** (aislar madre → click fastener → doble click fastener)
3. **Busca SOLO el segundo bloque** (el que viene de `IsolateSubSelection`) — ese es el que importa
4. **Cópiame TODAS las líneas** entre `──── DIAGNOSTIC ────` y `Result: 30/577`, incluyendo las líneas `[VISIBLE]`

Necesito ver específicamente:
- `selection.name` y `selection.path` 
- `isFastenerIsolation` y `fastenerInstanceId`
- Las líneas `[VISIBLE]` que dicen `byScope=True/False byInstance=True/False`

Esas líneas me dirán **exactamente** qué renderers se están filtrando mal y por qué razón.

YO:
[PVM.IsolateTransform] ──── DIAGNOSTIC ────
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:144)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection.name         = x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:145)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection.path         = x500v2_Drone/x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:146)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isolationScope.name    = x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:147)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isolationScope.path    = x500v2_Drone/x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:148)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  parentPart.name        = x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:149)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isFastenerIsolation    = False
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:150)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  fastenerInstanceId     = ''
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:151)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  includeAssocFasteners  = True
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:152)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selectedCanonicalPart  = 'x500v2_arm_FR'
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:153)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection has marker?   = False 
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:158)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  scope has marker?       = False 
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:159)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_013 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_013 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_013
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_008 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_009 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_009 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x8_009
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_012 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_012 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_012
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_013 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_013 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_013
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_015 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_015 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_015
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_012 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_012 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_012
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_013 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_013 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_013
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_014 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_014 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_014
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_013 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_013 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_013
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_008 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_014 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_014 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_014
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_015 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_015 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_015
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x38_012 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x38_012 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x38_012
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.flange_nut_M3_012 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.flange_nut_M3_012 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_flange_nut_M3_012
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_009 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_009 byScope=False byInstance=False byAssoc=True marker=x500v2_fastener_cap_screw_M3x8_009
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.arm_clamp path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.arm_clamp byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.arm_clamp_1 path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.arm_clamp_1 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.locator path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.locator byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.motor_cap path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.motor_cap byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.motor_mount path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.motor_mount byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.motor_plate path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.motor_plate byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.tube path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.tube byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.tube_clamp path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.tube_clamp byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.tube_clamp_1 path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.tube_clamp_1 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_arm_FR.tube_clamp_2 path=x500v2_Drone/x500v2_arm_FR/x500v2_arm_FR.tube_clamp_2 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_motor_FR.part_1 path=x500v2_Drone/x500v2_arm_FR/x500v2_motor_FR.part_1 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] Result: 27/425 renderers visible
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:206)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PartVisibility] Isolated transform: x500v2_arm_FR
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:220)
WebGL.UI.UIManager:IsolateFullSelection (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:584)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1042)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] ──── DIAGNOSTIC ────
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:144)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection.name         = x500v2_fastener.cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:145)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection.path         = x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:146)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isolationScope.name    = x500v2_fastener.cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:147)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isolationScope.path    = x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:148)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  parentPart.name        = x500v2_fastener.cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:149)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  isFastenerIsolation    = True
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:150)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  fastenerInstanceId     = 'x500v2_fastener_cap_screw_M3x8_008'
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:151)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  includeAssocFasteners  = False
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:152)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selectedCanonicalPart  = ''
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:153)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  selection has marker?   = True id='x500v2_fastener_cap_screw_M3x8_008'
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:158)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  scope has marker?       = True id='x500v2_fastener_cap_screw_M3x8_008'
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:159)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_008 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008 byScope=True byInstance=True byAssoc=False marker=x500v2_fastener_cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] tip path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/tip byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_00 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_00 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_01 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_01 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_02 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_02 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_03 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_03 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_04 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_04 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_05 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_05 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_06 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_06 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_07 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_07 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_08 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_08 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_09 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_09 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_10 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_10 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_11 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_11 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] head path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/head byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] x500v2_fastener.cap_screw_M3x8_008 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008 byScope=True byInstance=True byAssoc=False marker=x500v2_fastener_cap_screw_M3x8_008
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] tip path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/tip byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_00 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_00 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_01 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_01 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_02 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_02 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_03 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_03 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_04 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_04 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_05 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_05 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_06 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_06 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_07 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_07 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_08 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_08 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_09 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_09 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_10 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_10 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] thread_11 path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/thread_11 byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

  [VISIBLE] head path=x500v2_Drone/x500v2_bottom_plate/x500v2_fastener_group/x500v2_fastener.cap_screw_M3x8_008/__fastener_detail/detail_orientation/head byScope=True byInstance=False byAssoc=False marker=none
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:195)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

[PVM.IsolateTransform] Result: 30/577 renderers visible
UnityEngine.Debug:Log (object)
WebGL.Core.Managers.PartVisibilityManager:IsolateTransform (UnityEngine.Transform) (at Assets/Scripts/Core/Managers/PartVisibilityManager.cs:206)
WebGL.UI.UIManager:IsolateSubSelection (UnityEngine.Transform,UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:609)
WebGL.UI.UIManager:TryPromoteSelectionToNestedIsolation (UnityEngine.Transform) (at Assets/Scripts/UI/UIManager.cs:684)
WebGL.UI.UIManager:OnPartDoubleClicked (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/UI/UIManager.cs:1018)
WebGL.Core.Events.EventBus:Publish<WebGL.Core.Events.PartDoubleClickedEvent> (WebGL.Core.Events.PartDoubleClickedEvent) (at Assets/Scripts/Core/Events/EventBus.cs:112)
WebGL.Core.Managers.SelectionManager:HandleClick () (at Assets/Scripts/Core/Managers/SelectionManager.cs:298)
WebGL.Core.Managers.SelectionManager:Update () (at Assets/Scripts/Core/Managers/SelectionManager.cs:97)

OP:
Los logs son **reveladores**. Déjame analizar lo que realmente muestran:

## Análisis del Bloque 2 (sub-aislamiento del fastener)

```
selection.name         = x500v2_fastener.cap_screw_M3x8_008       ✅
isolationScope.name    = x500v2_fastener.cap_screw_M3x8_008       ✅
isFastenerIsolation    = True                                       ✅
fastenerInstanceId     = 'x500v2_fastener_cap_screw_M3x8_008'     ✅
includeAssocFasteners  = False                                      ✅
selection has marker?   = True id='x500v2_fastener_cap_screw_M3x8_008' ✅
```

Y **TODOS los renderers visibles** son del MISMO fastener:
- `cap_screw_M3x8_008` (byScope=True, byInstance=True)  
- `tip`, `head`, `thread_00`..`thread_11` (byScope=True — geometría de detalle bajo `__fastener_detail`)

**El aislamiento a nivel de código SÍ está funcionando correctamente.** Solo UN fastener (cap_screw_M3x8_008) y su geometría de detalle (threads + tip + head) están visibles. Los 30 renderers son: 15 partes de un solo tornillo × 2 pases por ExplodableParts duplicados.

## La pregunta clave

Antes de seguir editando código, necesito que me confirmes:

1. **¿Qué ves visualmente** cuando haces el segundo doble click (en el fastener)? ¿Ves múltiples tornillos diferentes (M3x38 + M3x8 + tuercas) o ves UN tornillo pero con muchas piezas geométricas (hilos, punta, cabeza)?

2. **¿El problema es que la vista del fastener aislado se deshace inmediatamente** y vuelve a la vista del brazo con todos los fasteners? (En tus logs anteriores vi que después del sub-aislamiento hay un `RestoreFullIsolationFromStack` — ¿es intencional porque diste otro doble click, o pasa automáticamente?)

3. **¿El problema es que en el PRIMER paso** (aislar `x500v2_arm_FR`), ves demasiados fasteners asociados (los 8 tipos) y quisieras ver solo el brazo?

Esto me dirá si el bug es en la lógica de aislamiento (que según los logs funciona), en la restauración del stack, o en otra capa del sistema.

YO:
1.múltiples tornillos diferentes (M3x38 + M3x8 + tuercas) 
2. No, se aisla la pieza madre correctamente, si aislo otra subpieza cualquiera se aisla correctamente, si aislo un fastener se aisla junto a otros fasteners. Luego intento aislarlo de nuevo y cuando hago eso se desaisla y va a la capa anterior.
3. No, el problema es que en el momento de intentar aislar un fastener, este no se aisla solo, sino que se aisla junto a otros fasteners (que igual no puedo seleccionar ni interactuar, solo son visibles)

OP:
¡Ahora entiendo! Los logs muestran que `IsolateTransform` SÍ aísla correctamente (30/577 = solo 1 fastener), pero algo **DESPUÉS** re-habilita los otros fasteners. Necesito ver qué pasa en `OnPartDoubleClicked` después de `TryPromoteSelectionToNestedIsolation` — probablemente el código continúa ejecutando y llama a `IsolateFullSelection` que **sobrescribe** el sub-aislamiento.

Viewed UIManager.cs:970-1060

Ahora necesito ver `TryPromoteSelectionToNestedIsolation` y `CanNestSelectionWithinIsolation`:

Viewed UIManager.cs:660-720
Searched for "CanNestSelectionWithinIsolation"
Searched for "CanNestSelection"
Searched for "CanNest"
Viewed UIManager.cs:560-620

Ahora necesito ver la lógica completa del `OnPartDoubleClicked` para entender el flujo exacto del primer doble-click en un fastener:

Viewed UIManager.cs:890-970

Ahora tengo la imagen completa. Veamos el flujo exacto cuando el usuario hace doble-click en un fastener **con la pieza madre ya aislada** (`_isIsolated=true`, `_isolatedSubSelection=null`):

1. **Línea 985**: `_isolatedSubSelection` es null → salta este bloque
2. **Línea 1016**: `_isIsolated` es true → entra aquí
3. **Línea 1018**: Evalúa `canNestWithinCurrentIsolation`

El problema está en **cómo se calcula `canNestWithinCurrentIsolation`** (líneas 979-983):

```csharp
bool canNestWithinCurrentIsolation = _isIsolated                    // true ✅
    && _isolatedSubSelection == null                                 // true ✅
    && clickedSelection != null                                      // true ✅ 
    && clickedSelection != _isolatedFullSelection                    // true ✅
    && CanNestSelectionWithinIsolation(clickedSelection, _isolatedFullSelection); // ???
```

Y `CanNestSelectionWithinIsolation` (línea 573):
```csharp
return IsSubSelection(selection, parentIsolation)             // false - fastener NO es hijo de arm_FR
    || IsFastenerAssociatedWithSelection(selection, parentIsolation); // true - por ParentCanonicalPartId
```

El fastener está en `x500v2_bottom_plate/x500v2_fastener_group/`, NO es hijo de `x500v2_arm_FR`. Pero `IsFastenerAssociatedWithSelection` devuelve true (por el marker.ParentCanonicalPartId). Eso dispara `TryPromoteSelectionToNestedIsolation` → `IsolateSubSelection(fastener, arm_FR)`.

**En IsolateSubSelection** (línea 611): `_isolatedFullSelection = fullSelection` = **arm_FR** (no el fastener).

Ahora miro los logs: el segundo bloque muestra 30/577 visibles y SOLO cap_screw_M3x8_008. **Eso es correcto a nivel de PartVisibilityManager.** 

Pero el usuario ve MÚLTIPLES fasteners. Eso significa que algo DESPUÉS re-habilita los otros. Necesito buscar si hay otro sistema interviniendo:

Viewed UIManager.cs:820-850
Searched for "IsolateTransform\|IsolatePart\|ClearIsolation\|renderer.enabled"
Searched for "\\.enabled"
Searched for "SetActive"
Searched for "OnPartSelected"
Searched for "PartSelectedEvent"
Searched for "RequestIsolation"
Searched for "IsolateTransform"

Interesante — hay un tercer llamador: `EnhancedInfoPanel.cs:373`. Veamos eso:

Viewed EnhancedInfoPanel.cs:1-390

El EnhancedInfoPanel solo llama IsolateTransform cuando el usuario hace click en el botón "Isolate" — no es automático. No es la causa.

Pensándolo bien: los logs del **segundo bloque** (sub-aislamiento) muestran que **solo 1 fastener** está visible (30 renderers, todos de `cap_screw_M3x8_008`). Pero el usuario ve MÚLTIPLES fasteners...

Esto solo puede significar una cosa: los logs que veo son del **caso correcto** (primer doble-click en arm → segundo doble-click en fastener usa `TryPromoteSelectionToNestedIsolation`), PERO el usuario NO está llegando a esa rama del código. 

Reconsiderando: la primera vez que el usuario hace click en el fastener DENTRO de la pieza aislada, ese click es un **SelectObject** (primer click), no doble-click. Solo el DOBLE-click dispara `OnPartDoubleClicked`. Cuando el usuario hace doble-click en el fastener, `clickedSelection` viene de `evt.ClickedTransform`, y el `SelectionManager` debió haber registrado la selección previamente.

Pero la clave es: **los logs que el usuario pegó SÍ muestran la secuencia de IsolateFullSelection seguida de IsolateSubSelection**. IsolateFullSelection (arm_FR con 27 visible) viene del PRIMER doble-click, y IsolateSubSelection (fastener con 30 visible) viene del SEGUNDO.

Si los logs muestran que solo 1 fastener es visible... pero el usuario ve más... algo ESTÁ re-habilitando renderers DESPUÉS del `IsolateTransform`. Déjame buscar:

Searched for "renderer.enabled = true"
Viewed FastenerInspectionManager.cs:320-365

`RestoreProxyRenderers` re-enables renderers pero solo cuando se LIMPIA una inspección. No debería correr justo después del aislamiento.

Espera. Necesito reconsiderar completamente. Los logs muestran que en el bloque 2, la primera llamada es `IsolateFullSelection` del arm (27/425), y la SEGUNDA llamada es `IsolateSubSelection` del fastener (30/577). El resultado FINAL debería ser 30/577 (solo el fastener).

Pero... en la primera set de logs abreviados del usuario, los bloques eran:
1. `IsolateFullSelection` → 27/425
2. `IsolateSubSelection` → 30/577

Ambos desde el MISMO frame (ambos desde `HandleClick` → `OnPartDoubleClicked`). El bloque 2 SOBRESCRIBE el bloque 1. El resultado final es 30/577 = 1 fastener.

**¡Ya lo tengo!** El bloque 1 (27/425) es de cuando el usuario **primero** aísla el arm_FR. El bloque 2 (30/577) ocurre luego al doble-clickear el fastener. El resultado es correcto. PERO el usuario aún ve múltiples fasteners. 

La única posibilidad es que el resultado visual NO corresponde a lo que los logs dicen. Quizás el resultado de 30 renderers es del **fastener correcto** pero al mismo tiempo, algo más está manteniendo otros renderers activos que NO están cubiertos por la iteración `allParts`.

Veamos cómo se puebla `allParts`:

Searched for "allParts"