# 🧠 Deep UX/UI Audit Report (Heuristics & Constraints)

**Date:** February 2026  
**Focus:** Cognitive Load (Miller's Law), Touch Ergonomics (Fitts's Law), and Mobile Constraints.  
**Objective:** Evaluate UI elements against the thesis criteria for minimizing cognitive friction.

---

## 1. Miller's Law (Working Memory Limits)
*   **The Law**: The average person can hold 7 ± 2 items in their working memory. Interfaces should group related actions to avoid overwhelming the user.
*   **Current State (`MainLayout.uxml`)**: The bottom pill presents 5 distinct, unclustered actions (View, Layers, Exp, Info, Env). While mathematically under 7, the *cognitive weight* of each tool is high because they modify entirely different aspects of the 3D scene (Render, Filter, Animation, Data, Lighting).
*   **Verdict**: **FAIL**. The current layout imposes unnecessary cognitive load by forcing users to parse 5 different mental models at once.
*   **Resolution**: Implement the **3-Mode UI** (Explore, Analyze, Studio) to chunk the information into 3 distinct, manageable categories.

## 2. Fitts's Law & Touch Targets
*   **The Law**: The time required to rapidly move to a target area is a function of the ratio between the distance to the target and the width of the target. Mobile standards (Apple HIG / Material Design) dictate a minimum touch target of **44x44px** (or 48x48dp).
*   **Current State (`Theme.uss`)**: 
    *   Primary bottom buttons (`.icon-button`) are **64x64px** (PASS).
    *   Submenu cards are **130x120px** (PASS).
    *   Close button for the Info Sheet (`.sheet-close-btn` line 437) is **32x32px**.
*   **Verdict**: **FAIL**. The `SheetCloseBtn` violates the 44px minimum rule, making it incredibly difficult to dismiss the sheet accurately on physical mobile devices.
*   **Resolution**: Increase `.sheet-close-btn` to `44px` width/height and adjust its internal icon size or padding accordingly.

## 3. Responsive Constraints & Flexbox Layouts
*   **The Constraint**: UIs must adapt fluidly to screen sizes. Hardcoding pixel widths leads to clipping on narrow devices (e.g., iPhone SE portrait is 375px wide).
*   **Current State (`Theme.uss`)**:
    *   `.actions-row` has `min-width: 600px;` (line 165).
    *   `.submenu-container` and `.submenu-grid` are rigid at `width: 600px;` (lines 488, 518).
    *   `.submenu-card` uses rigid `width: 130px;`.
*   **Verdict**: **CRITICAL FAIL**. The UI is effectively forced into a desktop/tablet dimension. On mobile phones, this will clip off the right side of the screen, making the "Env" and "Info" buttons unreachable.
*   **Resolution**: Replace fixed pixel widths with percentage-based flexbox scaling. 
    *   `.actions-row`: `width: 90%; max-width: 600px;`
    *   `.submenu-container`: `width: 95%; max-width: 600px;`
    *   `.submenu-card`: Use `flex-basis: 22%;` with `margin: 1.5%;` so exactly 4 cards fit flexibly within the row.

## 4. The 8pt Grid System
*   **The Constraint**: All margins, paddings, and sizes should be multiples of 8 (or 4 for micro-adjustments).
*   **Current State (`Theme.uss`)**: 
    *   Bottom bar `padding: 12px 32px` (Passes using 4pt increments).
    *   Margin-bottom `24px` (Passes).
*   **Verdict**: **PASS**. The spatial rhythm adheres well to the grid, but the fixed widths break the overall layout container.

---

## 🛠️ Actionable Structural Refactoring Plan

1. **Purge Hardcoded Widths**: Update `Theme.uss` immediately to remove all `600px`, `130px`, and `480px` fixed values on containers.
2. **Fix Accessibility**: Enlarge the `.sheet-close-btn` to `44x44px` with a `22px` `border-radius`.
3. **Execute Mode-Based UI**: Proceed with the "Phase 2" implementation of splitting the bottom bar into Explore, Analyze, and Studio tabs.
