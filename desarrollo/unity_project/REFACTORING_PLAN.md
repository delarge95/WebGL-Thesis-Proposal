# 🗺️ Refactoring Plan — WebGL Drone Viewer

**Project:** WebGL Drone Viewer (Unity 6.0 LTS — UI Toolkit / WebGL)  
**Branch:** `feature/phase3-architecture`  
**Created:** 23 de febrero de 2026  
**Last Updated:** 23 de febrero de 2026  
**Source:** Post-refactoring Architecture Audit Report

---

## 📊 Status Legend

| Icon | Meaning                    |
| ---- | -------------------------- |
| ⬜   | Not started                |
| 🔄   | In progress                |
| ✅   | Completed                  |
| ❌   | Cancelled / Not applicable |

---

## 🏁 Completed Work (Pre-Audit)

These items were completed before this plan was created:

| #    | Task                                                                        | Status | Commit    |
| ---- | --------------------------------------------------------------------------- | ------ | --------- |
| P3-1 | Memory Leak Prevention — `AddCleanup()` pattern                             | ✅     | `04df7a1` |
| P3-2 | God Class Dismantling — UIManager 972→388 lines                             | ✅     | `04df7a1` |
| P3-3 | Input Decoupling — `InputManager.IsPointerOverUI()` via `RuntimePanelUtils` | ✅     | `04df7a1` |
| P4-1 | Extract `GlobalInputBlocked` → `InputManager.InputBlocked`                  | ✅     | `9e24ed5` |
| P4-2 | `OrbitCameraController.HandleInput()` — `IsPointerOverUI()` guard           | ✅     | `9e24ed5` |
| P4-3 | `KeyboardShortcuts` — `InputBlocked` guard                                  | ✅     | `9e24ed5` |
| P4-4 | `SelectionManager.HandleHover()` — double early-out                         | ✅     | `9e24ed5` |
| P5-1 | Remove `ApplyDefaultRenderSettings()` (camera → EnvironmentController)      | ✅     | `1607733` |
| P5-2 | Remove `GlobalInputBlocked` bridge property                                 | ✅     | `1607733` |
| P5-3 | Consolidate `GameManager` → `AppStateMachine` (remove duplicate state)      | ✅     | `1607733` |
| P5-4 | Delete `CameraController.cs` stub (dead code)                               | ✅     | `1607733` |

---

## 📋 Current Plan — Audit-Driven Refactoring

Tasks derived from the Architecture Audit Report, ordered by priority.

### Task 1: Simplify `RegisterButtonInputBlockers()` — Input Blocking

**Severity:** 🟡 MEDIUM (Audit Issue #9)  
**Status:** ⬜ REVERTED — Requires deeper analysis  
**Files:** `UIManager.cs`, `UIEnvironmentPanel.cs`, `UIDetailsSheet.cs`, `InputManager.cs`, `OrbitCameraController.cs`, `SelectionManager.cs`, `KeyboardShortcuts.cs`

**Attempt 1 (commits 4b80bd5, acea37a, e7f79d8) — FAILED, REVERTED:**  
Removed `InputBlocked` entirely and all 4 per-button callbacks, relying solely on `IsPointerOverUI()`. This broke **all submenu button clicks and the InfoBtn** because:

1. `RegisterButtonInputBlockers()` does **two** critical things, not one:

   - `PointerEnter/Leave → InputBlocked = true/false` → blocks `SelectionManager` from processing clicks as 3D background clicks
   - `PointerDown/Up → StopPropagation()` → prevents pointer events from bubbling up to parent panels

2. `IsPointerOverUI()` via `Panel.Pick()` was **not a sufficient replacement** because:

   - Submenus start with `display: none` (via `.submenu--hidden` CSS class)
   - `Query<Button>()` at init time **cannot find buttons inside `display: none` containers**
   - Even when submenus are visible, there may be frame-timing gaps where `Panel.Pick()` doesn't yet reflect the updated layout
   - `InputBlocked` acts as a **proactive** flag (set on hover) while `IsPointerOverUI()` is **reactive** (checked per-frame) — the proactive approach is more reliable for preventing click-through

3. All 7 files were reverted to commit `1607733` (Phase 5 clean state).

**Root cause:** `RegisterButtonInputBlockers()` is NOT just about input blocking — its `StopPropagation` prevents event bubbling within the UI Toolkit panel, and `InputBlocked` provides a reliable proactive guard that survives frame-timing edge cases. Both mechanisms are essential.

**Revised assessment:** This task needs a fundamentally different approach if attempted again. The current dual mechanism (InputBlocked + StopPropagation + IsPointerOverUI) works correctly. Removing parts of it introduces subtle but critical regressions.

---

### Task 2: Verify `UIEnvironmentPanel` / `UIAnalyzePanel` Integration

**Severity:** 🟡 MEDIUM (Audit Issue #10)  
**Status:** ✅ Completed — No code changes needed  
**Files:** `UIEnvironmentPanel.cs`, `UIAnalyzePanel.cs`, `UIManager.cs`

**Problem:**  
Audit flagged these as potential orphans. Need to verify they are properly instantiated, used, and cleaned up.

**Findings (verified by code inspection):**

- [x] `UIManager.InitializeUI()` creates both panels (`new UIAnalyzePanel(...)` line 138, `new UIEnvironmentPanel(...)` line 141)
- [x] Both have `AddCleanup()` pattern in `UIManager`
- [x] Both are `Dispose()`d via `AddCleanup` in `UnsubscribeFromUIEvents()`
- [x] `UIAnalyzePanel.OnViewModeChanged()` is called by `UIManager.OnViewModeChanged()` (line 347), subscribed to `ViewModeManager.Instance.OnModeChanged` (line 301)
- [x] `UIEnvironmentPanel.UpdateEnvPresetActiveState()` is called internally on preset button click (line 99)

**Conclusion:** Both panels are fully integrated. NOT orphans. No action required.

---

### Task 3: Standardize Null-Safety Patterns

**Severity:** 🟡 LOW (Audit Issue #11)  
**Status:** ✅ Completed  
**Files:** 19 files across `Assets/Scripts/UI/` and `Assets/Scripts/Core/`

**Problem:**  
Inconsistent use of null-conditional `?.` vs bare access when calling singletons.

**Changes applied (46 replacements in 19 files):**

- [x] Audit all `XxxManager.Instance.Method()` calls — found ~100+ bare calls vs 28 using `?.`
- [x] Standardize to `XxxManager.Instance?.Method()` pattern
- [x] Convert verbose `if (X.Instance != null) X.Instance.Method()` → `X.Instance?.Method()`
- [x] Convert `if (X.Instance != null) X.Instance.PlayClick()` → `X.Instance?.PlayClick()`
- [x] Property reads with conditions: `SelectionManager.Instance?.HasSelection == true`
- [x] All 19 files compile with 0 errors

**Preserved (not changed):**

- Event subscriptions/unsubscriptions (`Instance.OnEvent +=`) — need special handling
- Early-return guards (`if (X.Instance == null) return;`) — subsequent code depends on existence
- `if/else` blocks with fallback branches (e.g., TweenEngine fade vs immediate hide)
- Patterns that read + compute (e.g., `float current = X.Instance.UIScale; X.SetUIScale(current + 0.1f)`)

**Files modified:**
`UIManager.cs`, `ViewModeToolbar.cs`, `KeyboardShortcuts.cs`, `PartCatalogUI.cs`,
`EnhancedInfoPanel.cs`, `EngineerToolbar.cs`, `UIPopupController.cs`, `UIDetailsSheet.cs`,
`UIEnvironmentPanel.cs`, `UIAnalyzePanel.cs`, `SettingsPanel.cs`, `SmartHotspot.cs`,
`ExplodedViewManager.cs`, `AssemblyChecklist.cs`, `CrossSectionManager.cs`,
`DroneStateController.cs`, `ConnectionPointsViewer.cs`, `ModularPartsSystem.cs`, `DroneAssembler.cs`

---

### Task 4: Input Abstraction Layer (Future — Optional)

**Severity:** ⚪ LOW (Audit Issue #3 remainder)  
**Status:** ⬜ Not started — Deferred  
**Files:** `InputManager.cs`, `OrbitCameraController.cs`, `SelectionManager.cs`

**Problem:**  
Raw `Input.*` polling scattered across multiple files. Not critical for WebGL-only.

**Plan:**

- [ ] Add `OnPointerClick`, `OnDrag`, `OnScroll` events to `InputManager`
- [ ] Refactor `OrbitCameraController` to consume `InputManager` events
- [ ] Refactor `SelectionManager` to consume `InputManager` events
- **Deferred until**: Multi-platform support is needed

---

### Task 5: EventBus WeakReference (Future — Optional)

**Severity:** ⚪ LOW (Audit Issue #5)  
**Status:** ⬜ Not started — Deferred  
**Files:** `EventBus.cs`

**Problem:**  
Strong references prevent GC of destroyed subscribers.

**Plan:**

- [ ] Implement `WeakReference<Delegate>` wrapper
- [ ] Add auto-prune of dead references on `Publish()`
- **Deferred until**: Multi-scene support is added

---

### Task 6: Singleton → Service Locator (Future — Optional)

**Severity:** ⚪ LOW (Audit Issue #4)  
**Status:** ⬜ Not started — Deferred  
**Files:** All managers

**Problem:**  
19+ singletons with direct `Instance` access.

**Plan:**

- [ ] Implement `ServiceLocator<T>` pattern
- [ ] Migrate managers one by one
- **Deferred until**: Unit testing coverage is a thesis requirement

---

## 📈 Progress Tracker

| Metric               | Value                      |
| -------------------- | -------------------------- |
| Total tasks          | 6                          |
| Completed            | 2 (Tasks 2, 3)             |
| In progress          | 0                          |
| Reverted             | 1 (Task 1 — needs rethink) |
| Deferred             | 3 (Tasks 4, 5, 6)          |
| Actionable remaining | 0                          |

---

_This file is the single source of truth for refactoring progress. Updated after every iteration._
