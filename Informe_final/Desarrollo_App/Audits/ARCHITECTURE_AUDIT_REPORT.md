# 🏗️ ARCHITECTURE AUDIT REPORT

## Technical & Architecture Deep Audit — Drone Viewer WebGL App

**Auditor Role:** Lead Unity Engineer & Technical Architect  
**Date:** 2025-07-15  
**Branch:** `feature/phase2-ux-redesign` (commit `1efb9fc`)  
**Scope:** All C# scripts inside `Assets/Scripts/` (90 files, ~14,202 lines)  
**Engine:** Unity 6.0 LTS · URP · UI Toolkit · WebGL/IL2CPP → WASM

> **⚠️ STALE (Mar 3, 2026):** Este reporte fue generado contra commit `1efb9fc` (Feb 19). Desde entonces se implementaron H01 (ServiceLocator), H02 (BaseModeHandler extraction), H03 (EventBus leak detection), H04 (10 archivos muertos eliminados), H08 (Update→coroutines), y C01–C05 (code quality). Múltiples hallazgos de este reporte ya están resueltos. Ver `CHANGELOG.md` secciones Feb 23 – Mar 3.

---

## Executive Summary

The codebase demonstrates a well-organized modular structure with clear separation of concerns (Core/Managers, Events, Content, Data, UI/Panels, Utils). However, the architecture relies heavily on the Singleton pattern (~20+ Singleton/PersistentSingleton managers), contains a dormant God Class risk in `ExplodedViewManager`, exhibits triple event-publishing redundancy, and has several areas where tighter adherence to SOLID principles would improve maintainability and testability.

**Overall Grade: B+** — Solid for a thesis-scope MVP; requires targeted refactoring for production readiness.

| Severity    | Count  |
| ----------- | ------ |
| 🔴 Critical | 2      |
| 🟠 High     | 5      |
| 🟡 Medium   | 8      |
| 🔵 Low      | 6      |
| **Total**   | **21** |

---

## 1. Findings by Severity

### 🔴 CRITICAL (2)

#### C-01: Duplicate Visual-Mode System in `ExplodedViewManager`

**File:** `Assets/Scripts/Core/Content/ExplodedViewManager.cs`  
**Lines:** ~145–225 (VisualMode enum, `SetVisualMode()`, material dictionary)

`ExplodedViewManager` contains a complete **second** view-mode system (`VisualMode` enum with 7 modes, its own material dictionary, `CycleVisualMode()`, `SetVisualMode()`) that directly duplicates the authoritative `ViewModeManager`. The `ExplodedViewManager.VisualMode` enum mirrors `ViewMode` 1:1 but lives in a different namespace, creating a high risk of:

- **State desynchronization** — calling `ExplodedViewManager.SetVisualMode(XRay)` does NOT update `ViewModeManager.CurrentMode`, and vice versa.
- **Material leaks** — two independent material dictionaries (`ExplodedViewManager.materials` and `ViewModeManager`'s cached materials) can instantiate duplicate Material objects on the GPU.
- **Bug surface** — any new contributor could call the wrong API.

**Impact:** Runtime visual inconsistency, doubled GPU memory for shader materials, maintenance confusion.

**Fix:**

1. Delete the entire `VisualMode` enum, `materials` dictionary, `SetVisualMode()`, `CycleVisualMode()`, and `GetMaterialForMode()` from `ExplodedViewManager`.
2. Ensure all call sites use `ViewModeManager.Instance.SetViewMode()` exclusively.
3. If `ExplodedViewManager` needs to react to mode changes, subscribe to `ViewModeManager.OnModeChanged`.

---

#### C-02: Triple Event Publishing on State Change

**File:** `Assets/Scripts/Core/Managers/AppStateMachine.cs`

When the app changes state, `AppStateMachine` publishes **three separate notifications**:

1. `OnStateChanged?.Invoke(newState)` — C# Action delegate
2. `EventBus.Publish(new StateChangedEvent(previousState, newState))` — typed event (prev+new)
3. `EventBus.Publish(new AppStateChangedEvent(newState))` — typed event (only new)

Subscribers are inconsistent about which channel they listen to:

- `UIManager` subscribes to `AppStateChangedEvent`
- `ExplodedViewManager` subscribes to `AppStateChangedEvent`
- Other code subscribes to `StateChangedEvent`
- `UIModeController` listens to the `OnStateChanged` Action

**Impact:** Cognitive overhead for developers, risk of handling the same state change twice, subtle ordering bugs.

**Fix:**

1. Choose **one** canonical method — recommend `EventBus.Publish(new StateChangedEvent(prev, next))` since it carries the most data.
2. Remove the `OnStateChanged` Action and `AppStateChangedEvent` struct.
3. Migrate all subscribers to `StateChangedEvent`.

---

### 🟠 HIGH (5)

#### H-01: Singleton Overuse (~20+ Singletons)

**Files:** All managers under `Assets/Scripts/Core/Managers/`

The following classes all inherit from `Singleton<T>` or `PersistentSingleton<T>`:

| Category       | Singletons                                                                                                                                                                                                                      |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Core Managers  | `AppStateMachine`, `SelectionManager`, `OrbitCameraController`, `ViewModeManager`, `CrossSectionManager`, `EnvironmentController`, `ExplodedViewManager`, `PartVisibilityManager`, `DroneStateController`, `ModularPartsSystem` |
| Infrastructure | `InputManager`, `WebGLOptimizer`, `QualityManager`, `ErrorHandler`, `AudioManager`, `NotificationManager`, `AccessibilityManager`, `AnalyticsManager`                                                                           |
| UI             | `UIManager`, `UIAnimator`, `LoadingController`, `HotspotManager`                                                                                                                                                                |

**Problems:**

- **Hidden dependencies** — any class can call `Foo.Instance.Bar()` without declaring the dependency, making the dependency graph invisible.
- **Testing impossibility** — Singletons cannot be mocked or replaced in unit tests.
- **Initialization order fragility** — `Awake()` order between Singletons is undefined; `UIManager.EnsureManagers()` auto-creates missing Singletons as a workaround, masking scene configuration errors.

**Impact:** Untestable code, fragile startup sequence, high coupling.

**Fix (Pragmatic for MVP scope):**

1. For this thesis project, full DI is overkill. Instead, introduce a **ServiceLocator** via a single `GameContext` class that registers/resolves managers, providing centralized dependency declaration.
2. At minimum, document the initialization order in a dedicated `BootSequence` script.
3. Long-term: migrate to a DI container (e.g., VContainer or Zenject) for production.

---

#### H-02: `GameManager` Name Suggests Central Coordinator — Actually a Minimal Debug Shell _(Downgraded from High → Low/Informational)_

**File:** `Assets/Scripts/Core/Managers/GameManager.cs` (62 lines)

`GameManager` is a `PersistentSingleton` that deliberately provides **only debug logging utilities**. Its XML documentation comment explicitly states: _"State management is handled by AppStateMachine — this class exists solely for debug support."_ This is an intentional architectural decision: `AppStateMachine` is the true coordinator (9 states, EventBus publishing), and `GameManager` avoids duplicating that responsibility.

**Reassessment:** The original audit called this "nearly empty" and an "architectural red herring." Upon closer inspection, the class is **minimal by design**, not accidentally empty. The naming may cause brief confusion for new developers, but the XML doc immediately clarifies the delegation pattern.

**Impact:** Low — naming is the only issue; functionality is correctly placed in `AppStateMachine`.

**Fix (Optional):**

1. Rename to `DebugManager` to better communicate its purpose, or
2. Add a `[Tooltip]` or header comment reinforcing that `AppStateMachine` is the coordinator.
3. No functional change needed — the current design is sound.

---

#### H-03: `UIModeController` Tracks Excessive State

**File:** `Assets/Scripts/UI/Panels/UIModeController.cs`

`UIModeController` manages at least 7+ state fields:

- `_isSheetOpen`, `_isExploded`, `_isIsolated`
- `_hotspotsEnabled`, `_activeCategories` (List)
- `_activeMode` (ActiveMode enum), `_subLevel` (SubLevel enum)
- Plus implicit state from which sub-panel is visible

This violates the **Single Responsibility Principle** — the controller simultaneously handles mode switching, sub-menu navigation, category filtering, isolation state, hotspot toggling, and explosion state.

**Impact:** Difficult to reason about state transitions, high bug surface when adding new modes.

**Fix:**

1. Extract category filtering into `CategoryFilterController`.
2. Extract sub-panel navigation into a `SubPanelNavigator`.
3. Keep `UIModeController` focused solely on mode activation/deactivation.

---

#### H-04: EventBus Has No Weak Reference Support or Leak Detection

**File:** `Assets/Scripts/Core/Events/EventBus.cs`

The `EventBus` stores subscribers as strong references in `Dictionary<Type, List<Delegate>>`. If a subscriber (e.g., a MonoBehaviour) subscribes in `OnEnable` but forgets to unsubscribe in `OnDisable`, the EventBus holds a strong reference preventing garbage collection → **memory leak**.

The current implementation has `try-catch` per subscriber and `lock` for thread safety, which is good. However:

- No **diagnostic/debug mode** to detect leaked subscriptions at runtime.
- No `ClearAll()` for scene transitions.
- `GetSubscriberCount<T>()` exists but is never called proactively.

**Impact:** Potential memory leaks on scene reload or long WebGL sessions.

**Fix:**

1. Add `#if UNITY_EDITOR || DEBUG` block that logs a warning when `GetSubscriberCount<T>()` exceeds a threshold.
2. Add `ClearAll()` and call it during scene transitions.
3. Consider using `WeakReference<Delegate>` for non-critical subscribers.

---

#### H-05: Two Obsolete Toolbars Still in Codebase — `ViewModeToolbar` & `EngineerToolbar`

**Files:** `Assets/Scripts/UI/ViewModeToolbar.cs`, `Assets/Scripts/UI/EngineerToolbar.cs`

Both classes are marked `[System.Obsolete]`:

- `ViewModeToolbar`: comment says "Replaced by UIModeController + UIAnalyzePanel (Phase 2 UX Redesign)". References stale APIs like `AppStateMachine.Instance.SetState()` and `PartCatalogUI.Instance?.Toggle()`.
- `EngineerToolbar`: also deprecated, superseded by the new 3-mode system.

**Impact:** Dead code increases build size (IL2CPP will include it unless stripping is High), confuses contributors.

**Fix:** Delete both `ViewModeToolbar.cs` and `EngineerToolbar.cs`, and any references to `PartCatalogUI` if that class is also dead.

---

### 🟡 MEDIUM (8)

#### M-01: Inconsistent Event Unsubscription Patterns

Multiple files use different cleanup patterns:

- `UIManager`, `UIHeroController`, `UIDetailsSheet`, `UICrossSectionPanel` → `_cleanupActions` list pattern (✅ Best)
- `ExplodedViewManager` → `OnDestroy()` directly (⚠️ Less reliable in WebGL)
- `DetailsPanelController` → `OnDisable()` (✅ OK but differs from peers)

**Fix:** Standardize on the `_cleanupActions` disposal pattern across all UI controllers. For MonoBehaviours, use `OnDisable()` consistently.

---

#### M-02: `ErrorHandler` Creates UI Entirely in C#

**File:** `Assets/Scripts/Core/Managers/ErrorHandler.cs`

`ErrorHandler.CreateErrorUI()` builds an entire error panel via C# code (`new VisualElement()`, inline styles). This bypasses the USS theming system and creates an inconsistent styling source.

**Fix:** Move the error panel to `MainLayout.uxml` (hidden by default) and style via `Theme.uss`.

---

#### M-03: `LoadingController` Creates UI Entirely in C#

**File:** `Assets/Scripts/UI/LoadingController.cs`

Same issue as M-02. The loading screen is built via C# with inline styles.

**Fix:** Define in UXML, style in USS.

---

#### M-04: `OrbitCameraController` Handles Both Mouse and Touch Inline

**File:** `Assets/Scripts/Core/Managers/OrbitCameraController.cs`

`HandleInput()` branches on `Input.touchCount` vs mouse, with duplicated orbit/pan/zoom logic across both paths. Touch sensitivity uses magic numbers (`0.2f`, `0.5f`, `5f`).

**Fix:** Extract an `IInputProvider` interface with `MouseInputProvider` and `TouchInputProvider` implementations.

---

#### M-05: `SelectionManager` vs `UIManager` Double-Click Split

Both `SelectionManager` (hover detection, raycast) and `UIManager` (double-click detection with `DOUBLE_CLICK_THRESHOLD = 0.35f`) participate in selection handling. The double-click → isolate logic lives in `UIManager`, but selection events originate from `SelectionManager`.

**Fix:** Move double-click detection to `SelectionManager` where the click originates, and publish a `PartDoubleClickedEvent`.

---

#### M-06: Magic Numbers Throughout the Codebase

Examples:
| File | Magic Number | Purpose |
|------|-------------|---------|
| `OrbitCameraController` | `targetY = 20f`, `targetDistance = 10f` | Reset defaults |
| `UIManager` | `DOUBLE_CLICK_THRESHOLD = 0.35f` | Double-click window |
| `WebGLOptimizer` | `Time.frameCount % 300` | Memory check interval |
| `ExplodedViewManager` | `0.5f` | Default explosion factor |
| `HighlightSystem` | `pulseSpeed = 2f` | Pulse animation speed |
| `UIDetailsSheet` | Swipe threshold `50` pixels | Gesture detection |

**Fix:** Extract to `[SerializeField]` fields or a central `AppConstants` static class.

---

#### M-07: `DronePartData` ScriptableObject Has Redundant Properties

**File:** `Assets/Scripts/Core/Data/DronePartData.cs`

The class defines both public fields (`partName`, `weightKg`, `materialType`) and C# properties (`PartName`, `Weight`, `MaterialType`) that simply return the same fields. It also has `weight`/`material` setter properties aliasing `weightKg`/`materialType`.

**Impact:** Confusing API — callers can use `data.partName`, `data.PartName`, or `data.weight` interchangeably.

**Fix:** Remove the property wrappers or make the fields private and expose only properties.

---

#### M-08: `QualityManager.AdjustQuality()` Is a No-Op

**File:** `Assets/Scripts/Core/Managers/QualityManager.cs`

The entire adaptive resolution system (`ScalableBufferManager.ResizeBuffers()`) is commented out, making `QualityManager` a class that only calculates FPS but takes no action.

**Fix:** Either implement adaptive resolution properly or delete `QualityManager`.

---

### 🔵 LOW (6)

#### L-01: `UIManager.EnsureManagers()` Silently Auto-Creates Missing Singletons

While self-healing is pragmatic, it masks scene-setup errors.

**Fix:** Log a warning: `Debug.LogWarning($"[UIManager] Auto-created missing manager.");`

---

#### L-02: `StaticInstance<T>.OnApplicationQuit()` Unreliable on WebGL

The base class sets `Instance = null` and `Destroy(gameObject)` in `OnApplicationQuit()`. In WebGL, this method is not reliably called (browser tab close bypasses it).

**Fix:** For WebGL, rely on `OnDisable()` for cleanup instead.

---

#### L-03: `HighlightSystem.PulseRoutine()` Allocates `MaterialPropertyBlock`

A new `MaterialPropertyBlock` is created inside the coroutine. While lightweight, it should be cached.

**Fix:** Move to class field, initialize in `Awake()`.

---

#### L-04: Inconsistent Namespace Organization

Most code uses `WebGL.Core.*`, `WebGL.UI.*`, etc. But `HotspotManager` and `SmartHotspot` are in the **global namespace**.

**Fix:** Move to `WebGL.UI` or `WebGL.Core.Managers`.

---

#### L-05: `ExplodablePart.Start()` Guard Logic Is Weak

```csharp
if (targetPosition == Vector3.zero && transform.localPosition != Vector3.zero)
    Initialize();
```

Fails silently if the part is intentionally at the origin.

**Fix:** Use a `bool _isInitialized` flag instead.

---

#### L-06: Dead Code — Commented-Out Fields

Multiple files contain commented-out fields:

- `QualityManager`: `// private float timeSinceLastCheck = 0f;`
- `ExplodedViewManager`: `// private bool isAnimating = false;`
- `HighlightSystem`: `// private bool isHovered = false;`

**Fix:** Delete; rely on version control for history.

---

## 2. Architecture Diagram (Current State)

```
┌──────────────────────────────────────────────────────────────────┐
│                     Singleton Layer (~20+)                        │
│                                                                   │
│  ┌────────────┐  ┌────────────────┐  ┌───────────────────────┐  │
│  │GameManager │  │AppStateMachine │  │  SelectionManager     │  │
│  │(minimal:   │  │(9 states,      │  │  (raycast + hover +   │  │
│  │debug only) │  │ 3 event paths!)│  │   EventBus publish)   │  │
│  └────────────┘  └───────┬────────┘  └──────────┬────────────┘  │
│                           │                       │               │
│  ┌───────────────┐  ┌────┴────────┐  ┌──────────┴────────────┐  │
│  │ViewModeManager│  │  EventBus   │  │OrbitCameraController  │  │
│  │(7 modes,      │  │  (pub/sub,  │  │(mouse+touch inline,   │  │
│  │ mat cache)    │  │   lock,     │  │ damped orbit/pan/zoom)│  │
│  └───────────────┘  │   try-catch)│  └───────────────────────┘  │
│                      └────────────┘                               │
│  ┌────────────────┐  ┌─────────────────┐  ┌──────────────────┐  │
│  │CrossSectionMgr │  │ExplodedViewMgr  │  │EnvironmentCtrl   │  │
│  │(dual-plane     │  │⚠ DUAL VisMode!  │  │(5 presets,       │  │
│  │ clipping)      │  │(explosion +     │  │ procedural       │  │
│  └────────────────┘  │ stale vis mgmt) │  │ gradients)       │  │
│                      └─────────────────┘  └──────────────────┘  │
│                                                                   │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐   │
│  │WebGLOptimizer  │  │QualityManager  │  │ InputManager     │   │
│  │(mem warnings)  │  │(FPS only—noop) │  │(UI detection,    │   │
│  └────────────────┘  └────────────────┘  │ InputBlocked)    │   │
│                                           └──────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
                              │
                    EventBus (typed pub/sub)
                              │
┌──────────────────────────────────────────────────────────────────┐
│                          UI Layer                                 │
│                                                                   │
│  ┌──────────┐                                                    │
│  │UIManager │── delegates to ──┬→ UIDetailsSheet                 │
│  │(Singleton│                  ├→ UIModeController (⚠ fat state) │
│  │ coord.)  │                  ├→ UIHeroController                │
│  └──────────┘                  ├→ UIAnalyzePanel                  │
│                                ├→ UIEnvironmentPanel              │
│                                ├→ UICrossSectionPanel             │
│                                └→ HotspotManager                  │
│                                                                   │
│  ⚠ ErrorHandler     → builds UI in C# (not UXML/USS)            │
│  ⚠ LoadingController→ builds UI in C# (not UXML/USS)            │
│  ⚠ ViewModeToolbar  → [Obsolete] still in codebase              │
│  ⚠ DetailsPanelCtrl → Subscribes EventBus but may be unused     │
└──────────────────────────────────────────────────────────────────┘
```

---

## 3. SOLID Compliance Scorecard

| Principle                     | Score | Assessment                                                                                                                                                                                                      |
| ----------------------------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **S** — Single Responsibility | 6/10  | `UIModeController` has 5+ responsibilities. `ExplodedViewManager` mixes explosion logic with duplicate visual mode logic. `UIManager` was refactored well but still handles isolation + hotspot initialization. |
| **O** — Open/Closed           | 7/10  | `ViewModeManager` supports 7 modes via enum switch — adding a mode requires modifying the switch. Could use Strategy pattern. Environment presets are clean.                                                    |
| **L** — Liskov Substitution   | 9/10  | Singleton hierarchy (`StaticInstance<T>` → `Singleton<T>` → `PersistentSingleton<T>`) is elegant and respects LSP.                                                                                              |
| **I** — Interface Segregation | 5/10  | Zero interfaces in the entire codebase. All communication via concrete `Singleton.Instance` references.                                                                                                         |
| **D** — Dependency Inversion  | 4/10  | Every manager calls `FooManager.Instance.Bar()`. High-level modules depend on low-level concrete implementations.                                                                                               |

**Overall SOLID Score: 6.2/10**

---

## 4. Memory Leak Risk Assessment

| Component                                   | Risk      | Current Mitigation                                                                                         |
| ------------------------------------------- | --------- | ---------------------------------------------------------------------------------------------------------- |
| EventBus subscriptions                      | 🟡 Medium | Most UI classes use `_cleanupActions` ✅; some MonoBehaviours rely on `OnDestroy` ⚠️ (unreliable in WebGL) |
| Material instances (`new Material()`)       | 🟡 Medium | `ViewModeManager` caches ✅; `ExplodedViewManager` also caches (duplicate) ⚠️                              |
| `MaterialPropertyBlock` allocations         | 🟢 Low    | Per-invocation but lightweight                                                                             |
| Coroutines (`HighlightSystem.PulseRoutine`) | 🟢 Low    | `StopPulse()` called on deselect ✅                                                                        |
| `Application.logMessageReceived`            | 🟢 Low    | `ErrorHandler` unsubscribes in `OnDestroy` ✅                                                              |
| UI Toolkit event callbacks                  | 🟢 Low    | Consistent `AddCleanup()` + `UnregisterCallback()` across all panels ✅                                    |

**Overall Leak Risk: Low-Medium** — The `_cleanupActions` pattern is excellent. Main risk is the `ExplodedViewManager` duplicate material cache.

---

## 5. Prioritized Refactoring Roadmap

### Phase A — Quick Wins (1–2 hours)

| #   | Task                                                                              | Fixes   |
| --- | --------------------------------------------------------------------------------- | ------- |
| A1  | Delete `ExplodedViewManager.VisualMode` system entirely                           | 🔴 C-01 |
| A2  | Consolidate to single `StateChangedEvent`, remove `AppStateChangedEvent` + Action | 🔴 C-02 |
| A3  | Delete `ViewModeToolbar.cs` and dead `PartCatalogUI` references                   | 🟠 H-05 |
| A4  | Remove all commented-out code                                                     | 🔵 L-06 |
| A5  | Add namespace to `HotspotManager`/`SmartHotspot`                                  | 🔵 L-04 |

### Phase B — Structural Improvements (3–5 hours)

| #   | Task                                                          | Fixes         |
| --- | ------------------------------------------------------------- | ------------- |
| B1  | Extract `CategoryFilterController` from `UIModeController`    | 🟠 H-03       |
| B2  | Add EventBus `ClearAll()` + leak diagnostics in DEBUG         | 🟠 H-04       |
| B3  | Move `ErrorHandler`/`LoadingController` UI to UXML+USS        | 🟡 M-02, M-03 |
| B4  | Standardize to `_cleanupActions` pattern everywhere           | 🟡 M-01       |
| B5  | Move double-click detection to `SelectionManager`             | 🟡 M-05       |
| B6  | Extract magic numbers to `AppConstants` or `[SerializeField]` | 🟡 M-06       |

### Phase C — Architecture Evolution (Post-Thesis, Optional)

| #   | Task                                                    | Fixes     |
| --- | ------------------------------------------------------- | --------- |
| C1  | Introduce `ServiceLocator` or mini DI container         | 🟠 H-01   |
| C2  | Extract `IInputProvider` from `OrbitCameraController`   | 🟡 M-04   |
| C3  | Add interfaces for inter-manager communication          | ISP + DIP |
| C4  | Strategy pattern for `ViewModeManager` mode application | OCP       |

---

## 6. Positive Observations

1. **Exemplary UI Toolkit event management**: The `_cleanupActions` + `AddCleanup()` + `Dispose()` pattern is consistently applied across all 6 UI panel classes (`UIDetailsSheet`, `UIModeController`, `UIHeroController`, `UIAnalyzePanel`, `UIEnvironmentPanel`, `UICrossSectionPanel`). This is a best practice that prevents memory leaks — excellent engineering.

2. **Well-designed EventBus**: Thread-safe with `lock`, `try-catch` per subscriber, typed events using structs (value types → zero GC allocation). The `GetSubscriberCount<T>()` diagnostic method shows forethought.

3. **Elegant Singleton hierarchy**: The 3-tier `StaticInstance<T>` → `Singleton<T>` → `PersistentSingleton<T>` with clear DontDestroyOnLoad semantics is a clean, reusable pattern.

4. **Active God Class dismantling**: The `UIManager` header documents "Phase 3 Step 2: God Class Dismantling" — demonstrating active awareness and effort. UIManager now cleanly delegates to 6 sub-controllers with a coordinator-only role.

5. **Procedural environment system**: `EnvironmentController` creates gradients and lighting presets procedurally (zero asset cost) — ideal for WebGL build size.

6. **Defensive null checks**: Nearly all `Instance?.Method()` calls use null-conditional operators, preventing NullReferenceExceptions from Singleton initialization order issues.

7. **InputManager as single source of truth**: The centralized `InputBlocked` static flag (replacing the old scattered `GlobalInputBlocked`) shows good refactoring practice.

8. **Clean data model**: `DronePartData` ScriptableObject is comprehensive (35+ fields covering specs, assembly info, visuals, connections) and serves the thesis requirement for technical visualization well.

---

## 7. Codebase Metrics Summary

| Metric               | Value   | Assessment                                            |
| -------------------- | ------- | ----------------------------------------------------- |
| Total C# files       | 90      | Appropriate for scope                                 |
| Total lines of code  | ~14,202 | Clean for a thesis MVP                                |
| Singleton count      | ~20+    | ⚠️ High (see H-01)                                    |
| Custom shaders       | 9       | Good — all WebGL-optimized                            |
| Event types (struct) | 4       | ✅ Clean, zero-allocation                             |
| UI sub-controllers   | 6       | ✅ Well-decomposed                                    |
| Dead/obsolete files  | 3+      | `ViewModeToolbar`, `EngineerToolbar`, `PartCatalogUI` |
| Namespaces used      | 7       | ✅ Good organization                                  |
| Unit test coverage   | 0%      | ⚠️ No tests found                                     |
| AppState count       | 9       | Matches feature set                                   |
| ViewMode count       | 7       | Matches thesis requirement                            |

---

_Generated by Lead Unity Engineer Audit — Pillar 1 of 4_
