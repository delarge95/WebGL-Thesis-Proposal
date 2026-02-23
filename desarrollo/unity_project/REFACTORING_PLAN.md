# 🗺️ Refactoring Plan — WebGL Drone Viewer

**Project:** WebGL Drone Viewer (Unity 6.0 LTS — UI Toolkit / WebGL)  
**Branch:** `feature/phase3-architecture`  
**Created:** 23 de febrero de 2026  
**Last Updated:** 23 de febrero de 2026  
**Source:** Post-refactoring Architecture Audit Report

---

## 📊 Status Legend

| Icon | Meaning |
|------|---------|
| ⬜ | Not started |
| 🔄 | In progress |
| ✅ | Completed |
| ❌ | Cancelled / Not applicable |

---

## 🏁 Completed Work (Pre-Audit)

These items were completed before this plan was created:

| # | Task | Status | Commit |
|---|------|--------|--------|
| P3-1 | Memory Leak Prevention — `AddCleanup()` pattern | ✅ | `04df7a1` |
| P3-2 | God Class Dismantling — UIManager 972→388 lines | ✅ | `04df7a1` |
| P3-3 | Input Decoupling — `InputManager.IsPointerOverUI()` via `RuntimePanelUtils` | ✅ | `04df7a1` |
| P4-1 | Extract `GlobalInputBlocked` → `InputManager.InputBlocked` | ✅ | `9e24ed5` |
| P4-2 | `OrbitCameraController.HandleInput()` — `IsPointerOverUI()` guard | ✅ | `9e24ed5` |
| P4-3 | `KeyboardShortcuts` — `InputBlocked` guard | ✅ | `9e24ed5` |
| P4-4 | `SelectionManager.HandleHover()` — double early-out | ✅ | `9e24ed5` |
| P5-1 | Remove `ApplyDefaultRenderSettings()` (camera → EnvironmentController) | ✅ | `1607733` |
| P5-2 | Remove `GlobalInputBlocked` bridge property | ✅ | `1607733` |
| P5-3 | Consolidate `GameManager` → `AppStateMachine` (remove duplicate state) | ✅ | `1607733` |
| P5-4 | Delete `CameraController.cs` stub (dead code) | ✅ | `1607733` |

---

## 📋 Current Plan — Audit-Driven Refactoring

Tasks derived from the Architecture Audit Report, ordered by priority.

### Task 1: Remove `RegisterButtonInputBlockers()` + `InputBlocked` — Simplify Input Blocking
**Severity:** 🟡 MEDIUM (Audit Issue #9)  
**Status:** ✅ Completed  
**Files:** `UIManager.cs`, `UIEnvironmentPanel.cs`, `UIDetailsSheet.cs`, `InputManager.cs`, `OrbitCameraController.cs`, `SelectionManager.cs`, `KeyboardShortcuts.cs`

**What was done:**
- [x] Removed `RegisterButtonInputBlockers()` from `UIManager.cs`
- [x] Removed `_onBtnEnter`, `_onBtnLeave` field declarations (InputBlocked handlers)
- [x] **Kept** `_onBtnDown` (StopPropagation) and `_onBtnUp` (StopPropagation) — these are **critical** to prevent click-through
- [x] Replaced with `RegisterButtonStopPropagation()` — same Query\<Button\> loop but only registers StopPropagation, no InputBlocked
- [x] Removed per-slider `PointerEnter/Leave` handlers from `UIEnvironmentPanel.cs` (kept `StopPropagation` for drag isolation)
- [x] Removed per-slider `PointerEnter/Leave` handlers from `UIManager.cs` explosion slider (kept `StopPropagation`)
- [x] Removed per-element `PointerEnter/Leave` handlers from `UIDetailsSheet.cs` (sheet + scrollview)
- [x] Removed `InputManager.InputBlocked` static property (dead code — nobody writes it)
- [x] Removed `InputBlocked` guards from `OrbitCameraController`, `SelectionManager`, `KeyboardShortcuts`
- [x] Verified `IsPointerOverUI()` via `Panel.Pick()` is the sole UI-blocking mechanism
- [x] 0 compile errors across all modified files

**⚠️ Lesson learned:**  
The original `RegisterButtonInputBlockers()` had 4 callbacks. Two were redundant (`InputBlocked` enter/leave), but two were **essential** (`StopPropagation` down/up). Removing all 4 broke button clicks in submenus because PointerDown events bubbled up to parent panels. Fixed by keeping only the StopPropagation pair in a renamed `RegisterButtonStopPropagation()`.

**Impact:**
- UIManager.cs: ~370 lines (slim coordinator, StopPropagation retained)
- **Single mechanism** for UI-blocks-3D: `InputManager.IsPointerOverUI()` via `Panel.Pick()`
- **StopPropagation** on buttons: prevents event bubbling that blocks clicks in nested menus

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
**Status:** ⬜ Not started  
**Files:** Multiple managers

**Problem:**  
Inconsistent use of null-conditional `?.` vs bare access when calling singletons.

**Plan:**
- [ ] Audit all `XxxManager.Instance.Method()` calls
- [ ] Standardize to `XxxManager.Instance?.Method()` pattern
- [ ] Ensure `InputManager.Instance != null` checks are consistent

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

| Metric | Value |
|--------|-------|
| Total tasks | 6 |
| Completed | 2 |
| In progress | 0 |
| Deferred | 3 (Tasks 4, 5, 6) |
| Actionable remaining | 1 (Task 3) |

---

*This file is the single source of truth for refactoring progress. Updated after every iteration.*
