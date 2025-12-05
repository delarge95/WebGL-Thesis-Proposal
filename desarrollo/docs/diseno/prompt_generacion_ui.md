# Prompt para Generación de UI (Google Stitch / v0 / Cursor)

Copia y pega el siguiente prompt en tu herramienta de IA, adjuntando las 4 imágenes de la carpeta `assets/ui_design`.

---

## 📋 The Prompt

**Role:** Expert Senior Frontend Engineer & UI Designer.
**Objective:** Create a "Mobile-First" responsive web interface for a High-Tech 3D Drone Viewer.
**Tech Stack:** HTML5, Modern CSS (Flexbox/Grid, CSS Variables, Backdrop-filter), Vanilla JavaScript.

### 🖼️ Input Images Analysis
I am providing 4 images. You must strictly follow this hierarchy:

1.  **STRUCTURAL BASE (Priority 1):** Use `ui_grid_system_layout.png` and `ui_wireframe_flow.png`.
    *   These define the **Layout Structure**, **Spacing**, and **Navigation Flow**.
    *   Strictly adhere to the 12-column logic (adapted to 4-column for mobile) and the placement of the "Bottom Dock" and "Data Panel".
    *   Ignore the visual style of these images (they are flat wireframes); only take the *structure*.

2.  **VISUAL & FUNCTIONAL REFERENCE (Priority 2):** Use `drone_ui_main_concept.png` and `drone_ui_features_showcase.png`.
    *   These define the **Aesthetics** (Colors, Glassmorphism, Neon Glows) and **Features**.
    *   **Style:** "Midnight Neon". Background `#050505`. Accents `#00F0FF` (Cyan) and `#FF4D00` (Orange).
    *   **Glassmorphism:** Use `backdrop-filter: blur(20px)` and `background: rgba(255,255,255,0.05)` for all panels.
    *   **Features to Include:**
        *   **Exploded View:** A control (toggle/slider) to expand drone parts.
        *   **X-Ray Mode:** A toggle to see internal components.
        *   **Bento Grid:** A data panel showing telemetry (Battery, Signal, Motor Status).

### 📱 Task: Mobile Interface Implementation
Build the **Mobile Version** first.

**Requirements:**
1.  **Viewport:** Fullscreen mobile layout (100vh).
2.  **Navigation:**
    *   Place the **Control Dock** at the bottom of the screen (Floating Glass Bar).
    *   Include icons for: `Reset`, `Explode`, `X-Ray`, `Measure`.
3.  **Data Panel:**
    *   Instead of a permanent side panel (desktop), make it a **Drawer/Modal** that slides up from the bottom when requested.
    *   Use the "Bento Grid" layout inside this drawer for data visualization.
4.  **Typography:**
    *   Headings: `Rajdhani` (Google Fonts) - Uppercase, Technical.
    *   Body: `Inter` or `Satoshi` - Clean, legible.
5.  **3D Container:**
    *   Leave a central `div` with ID `#canvas-container` that takes up the full background. This is where the WebGL canvas will live.

**Output:**
Provide the complete, clean `index.html` and `style.css` code. Ensure the CSS is modular and uses variables for colors.
