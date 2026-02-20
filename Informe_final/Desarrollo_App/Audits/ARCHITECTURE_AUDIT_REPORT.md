# 🏗️ Technical Architecture Audit Report

**Date:** February 2026  
**Scope:** Core C# Systems (`UIManager.cs`, `SelectionManager.cs`, `OrbitCameraController.cs`, `EventBus.cs`)  
**Objective:** Identify architectural flaws, structural weaknesses, and potential memory leaks to ensure production readiness.

---

## 🚨 Major Architectural Flaws

### 1. The `UIManager` God Class (Severity: CRITICAL)
*   **Issue**: `UIManager.cs` is over 1,000 lines long and handles vastly different responsibilities. It manages UI element binding, animation logic (sheets sliding), state machine transitions, environment presets, shader toggles, and user input validation all simultaneously.
*   **Solid Violation**: Single Responsibility Principle (SRP).
*   **Impact**: Modifying one aspect of the UI risks breaking unrelated systems. It is effectively unmaintainable in the long term.

### 2. Memory Leaks in UI Toolkit Events (Severity: HIGH)
*   **Issue**: In `UIManager.InitializeUI()`, dozens of events are subscribed (`button.clicked +=`, `element.RegisterCallback<T>`, `slider.RegisterValueChangedCallback`). However, `OnDisable()` only unsubscribes from `EventBus` events. None of the UI elements are properly unregistered.
*   **Impact**: If the `UIManager` script is disabled/enabled, or if the `UIDocument` is re-instantiated, these dangling delegates will keep old objects in memory, causing silent, cumulative memory leaks.

### 3. Tight Coupling & Direct Input Polling (Severity: HIGH)
*   **Issue**: Both `SelectionManager.cs` and `OrbitCameraController.cs` directly poll the legacy `UnityEngine.Input` class in their `Update()` / `LateUpdate()` loops (`Input.GetMouseButtonDown`, `Input.GetAxis`, `Input.touchCount`). Furthermore, `SelectionManager` actively checks `OrbitCameraController.GlobalInputBlocked` (tight coupling between modules).
*   **Impact**: Hard to support multiple platforms (web vs. mobile) cleanly. It also means input handling is scattered across the codebase rather than centralized, leading to race conditions or conflicting input blocks.

### 4. Overuse of the Singleton Pattern (Severity: MEDIUM)
*   **Issue**: The architecture relies heavily on the `Singleton<T>` pattern. `OrbitCameraController.Instance`, `HotspotManager.Instance`, `ViewModeManager.Instance`, `EnvironmentController.Instance` are called directly across various scripts.
*   **Solid Violation**: Dependency Inversion Principle. Scripts depend directly on concrete singleton implementations rather than interfaces.
*   **Impact**: Makes unit testing difficult (cannot mock dependencies). Creates hidden dependencies across the codebase (Spaghetti Code).

### 5. Strong References in `EventBus` (Severity: MEDIUM)
*   **Issue**: The `EventBus.cs` uses `Dictionary<Type, List<Delegate>>` which holds *strong references* to the subscriber functions.
*   **Impact**: If an object subscribes to `EventBus` but is destroyed without calling `EventBus.Unsubscribe` (which is highly likely giving the codebase state), the `EventBus` will prevent the garbage collector from freeing that object.

---

## 🛠️ Concrete Refactoring Plan

### Step 1: Dismantle the `UIManager` God Class
*   Split `UIManager.cs` into distinct, focused controllers:
    *   `UIToolkitBinder.cs`: Solely responsible for `root.Q<T>` and caching references.
    *   `MenuController_Analyze.cs`: Handles Shaders, Categories, and Explode tools logic.
    *   `MenuController_Environment.cs`: Handles lighting and studio presets.
    *   `UIAnimations.cs`: Handles the sliding/translation math for the BottomSheet.

### Step 2: Fix Memory Leaks (Cleanup Logic)
*   Implement explicit cleanup in UI controllers. Every `clicked +=` must have a corresponding `clicked -=` in `OnDisable()`. Every `RegisterCallback` must have an `UnregisterCallback`.

### Step 3: Implement an Input Abstraction Layer
*   Create a centralized `InputManager.cs` that abstracts hardware input (Mouse, Touch).
*   Refactor `SelectionManager` and `OrbitCameraController` to subscribe to generic `InputManager` events (e.g., `OnPointerClick`, `OnDrag`) rather than polling `Input` class natively.

### Step 4: Refactor Singletons to Dependency Injection / Service Locator
*   Transition from `ClassName.Instance` to a Service Locator pattern (or a lightweight DI tool like VContainer). Pass dependencies into constructors or initialization methods rather than allowing global grabs.
