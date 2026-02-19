# Comprehensive UI/UX Overhaul — Master Plan v2.1

Unified plan covering 12+ issues. Updated with user feedback from review.

---

## User Review Required

> [!IMPORTANT]
> **Color Palette**: Web uses `--accent: #ffffff` (white). Plan proposes replacing Unity's cyan `rgb(0, 217, 255)` with white. User to confirm.

> [!WARNING]
> **Submenu Redesign**: Major visual overhaul — switching from inline popup menus to a **grid card system** (4 columns, rounded icons + text labels, semi-transparent). This is the most complex change.

---

## Design Reference

![guia_botones.png](C:/Users/alexw/.gemini/antigravity/brain/4a3f515c-36b4-4ece-a131-2581e2acb131/guia_botones.png)

**Key patterns from reference:**
- Dark glassmorphic grid cards with **rounded corners** (NOT pill)
- Each card: **circular icon** (top) + **text label** (bottom)
- Semi-transparent card backgrounds
- Subtle gradient glow behind content
- Bottom bar: unified container with circular icon buttons

---

## Phase 0 — Design System & Research

### 0A. Design Token Specification

| Category | Contents |
|---|---|
| **Color** | Bg, Surface, Accent (white), On-Surface, Muted, Border |
| **Typography** | Inter (body), Space Grotesk (headings), sizes, weights, UPPERCASE rules |
| **Spacing** | 8px grid, margins, paddings, gaps |
| **Sizing** | Button min/max, icon sizes, touch targets (≥48dp) |
| **Radius** | Pill (height/2) for hero buttons, Rounded Rect (12-16px) for submenu cards, Circle (50%) for icons |
| **States** | Default, Hover, Active, Disabled, Selected |
| **Motion** | Duration, easing, transitions |

### 0B. Web Color Palette (Verified)

```css
--bg:        #050505;    --text-main: #ffffff;
--text-muted:#888888;    --accent:    #ffffff;
--border:    rgba(255, 255, 255, 0.1);
```

### 0C. Background Gradient (User Request)
The web landing page has a subtle animated gradient glow behind the hero section. Unity equivalent:
- **Option A**: Custom Shader on a fullscreen quad behind the 3D content — radial gradient with subtle color shifts
- **Option B**: Post-processing bloom on a faint emissive plane
- **Recommended**: Shader-based radial gradient (dark purple/blue tones fading to black) — matches web aesthetic and reference image

---

## Phase 1 — Critical Bug Fixes

### 1.1 Select Device Buttons Clipped Right
**Fix**: Increase `.device-selector` width from `320px` → `400px`

#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)

---

### 1.2 Canvas Background Not Starting Black
**Root Cause**: Camera clear color, Skybox, AND/OR Environment Lighting panel configuration.

#### [MODIFY] [OrbitCameraController.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/Core/OrbitCameraController.cs)
- Set `Camera.main.backgroundColor` to near-black in `Awake()`
- Set `RenderSettings.skybox = null`
- Set `RenderSettings.ambientMode = AmbientMode.Flat` with dark ambient color
- Check `RenderSettings.defaultReflectionMode` — disable if causing bright reflections

> [!NOTE]
> User notes this may also be caused by the Environment panel configuration being "in front" of the background. Will investigate the rendering order and environment settings.

---

### 1.3 Info Panel — Text Clipped & Unwanted "×"
#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- Hide clear button: `.unity-base-text-field__input > .unity-text-element--inner-input-field-component + * { display: none; }`
- Ensure `white-space: normal` and no `max-height` on text containers

#### [MODIFY] [MainLayout.uxml](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml)
- Set `multiline="true"` on all info text `TextField` elements

---

### 1.4 Info Panel — Buttons Too High
#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- Reduce `.ui-shifted { bottom: 56% }` → ~`46%` (buttons just above panel edge)

---

### 1.5 Hidden Submenus Ghost Clicks
#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss) + [UIManager.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs)
- All `--hidden` states: add `display: none` (not just `opacity: 0`)
- Explicit `pickingMode = Ignore` in C# when hiding

---

### 1.6 Explosion Slider — Rough & Off-Center (VERTICAL)
**Clarification**: The slider is off-center **vertically**, not horizontally.

#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- Center dragger vertically: `.glass-slider .unity-base-slider__dragger { top: 50%; margin-top: -16px; }` (half of dragger size)
- Reduce dragger to `32px` for sleeker feel
- Smooth: add `transition-duration: 0.1s`

---

## Phase 2 — Color & Visual Unification

### 2.1 Replace Cyan → White
#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- Find all `rgb(0, 217, 255)` → `rgb(255, 255, 255)`
- Active states: white bg + black text
- Slider dragger: white bg
- Selection label: transparent bg, white text (remove green)

### 2.2 Animated Gradient Background
Subtle radial gradient glow behind the 3D canvas (like the web hero section and reference image).

#### [NEW] Gradient Background Shader or UI Element
- **Research**: Best approach for Unity WebGL (shader quad vs. UI Toolkit gradient vs. post-processing)
- Deep purple/magenta tones fading to black (matching reference image aesthetic)
- Subtle animation (slow pulse or drift)

---

## Phase 3 — Submenu & Bottom Bar Redesign

### 3.1 Unified Bottom Bar Container
**New approach**: One large pill-shaped container holding all icon buttons (instead of separate circles).

#### [MODIFY] [MainLayout.uxml](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml)
- Wrap icon buttons in a single `.bottom-bar-container` (pill shape, same radius/margin as hero buttons)
- Icons inside: circular, evenly spaced

#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- `.bottom-bar-container`: pill shape, semi-transparent bg, white border
- `.bottom-bar-icon`: circular, 48px, no individual border

### 3.2 Freepik Icons for Bottom Bar Buttons
**User request**: Replace text labels with minimal Freepik icons.

#### Buttons needing icons:
| Button | Current Text | Needed Icon |
|---|---|---|
| Home | 🏠 | House/Home |
| Shader/Layer | LAYER | Layers stack |
| Explode | EXP | Expand/Explode arrows |
| Info | INFO | Info circle |
| Reset | RST | Refresh/Reset arrows |
| Environment | ENV | Sun/Globe |
| Pins | PINS | Pin/Marker |

#### Steps:
1. Source free icons from Freepik (SVG, white, minimal/outline style)
2. Import as PNG sprites (64x64, white on transparent)
3. Place in `Assets/UI/Icons/` folder
4. Reference in UXML via `<ui:VisualElement>` with `background-image`

### 3.3 Grid-Based Submenu System (Major Redesign)

**Rules** (from user):
- ✅ Only **one submenu open at a time**
- ✅ **4-column grid** layout, centered
- ✅ Each cell: **circular icon** (top) + **text label** (bottom)
- ✅ Cell style: **rounded rectangle** (12-16px radius), NOT pill
- ✅ **Semi-transparent** card backgrounds
- ✅ White text
- ✅ Slider containers: **half the cell height**, **full width** (4 columns)
- ✅ Follows reference image style

#### New USS Classes:

```css
/* Submenu Grid Container */
.submenu-grid {
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: center;
    width: 90%;
    max-width: 480px; /* 4 × 120px cards */
    /* ... positioning, transitions ... */
}

/* Grid Card */
.submenu-card {
    width: 100px;  /* ~4 cards + gaps in 480px */
    height: 100px;
    margin: 8px;
    background-color: rgba(255, 255, 255, 0.05);
    border-radius: 16px; /* Rounded rect, NOT pill */
    border-width: 1px;
    border-color: rgba(255, 255, 255, 0.1);
    flex-direction: column;
    align-items: center;
    justify-content: center;
}

/* Card Icon (circular) */
.submenu-card-icon {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background-color: rgba(255, 255, 255, 0.1);
    margin-bottom: 8px;
    /* background-image set per button */
}

/* Card Label */
.submenu-card-label {
    font-size: 11px;
    color: white;
    letter-spacing: 1px;
}

/* Slider Container (half height, full width) */
.submenu-slider-row {
    width: 100%; /* Full width of grid */
    height: 50px; /* Half of card height */
    /* ... */
}
```

#### Affected Submenus:
1. **Shader Mode** (Realistic, X-Ray, Blueprint, etc.) → 7 grid cards
2. **Category Filter** (All, Structure, Propulsion, etc.) → 5 grid cards
3. **Environment Panel** (presets + sliders) → Grid cards + slider row

#### [MODIFY] [MainLayout.uxml](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml)
- Replace `.shader-menu`, `.category-menu`, `.env-panel` with `.submenu-grid` containers
- Each option becomes a `.submenu-card` with icon + label

#### [MODIFY] [UIManager.cs](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs)
- **Exclusive submenu logic**: Opening one closes all others
- `CloseAllSubmenus()` method
- Each toggle: `CloseAllSubmenus()` → then open requested

#### [MODIFY] [Theme.uss](file:///e:/WebGL_tesis/desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- Add all new grid/card classes
- Remove old `.shader-menu`, `.category-menu` specific styles (keep hidden states)

---

## Phase 4 — Documentation & Dev Log

### 4.1 Create Folder Structure
```
Informe_final/Desarrollo_App/
├── README.md
├── CHANGELOG.md
├── hojas_de_ruta/
│   ├── HOJA_DE_RUTA_v1.md
│   └── PLAN_UI_OVERHAUL_v2.md
├── design_system/
│   ├── DESIGN_TOKENS.md
│   └── COMPONENT_LIBRARY.md
└── sesiones/
    └── SESSION_LOG.md
```

### 4.2 Generate CHANGELOG.md (from git + session history)
### 4.3 Create DESIGN_TOKENS.md (single source of truth)

---

## Phase 5 — RAG & Knowledge Graph
- Update KIs for design system, color palette, and component specifications
- Update `unity_ui_pro` skill with final design tokens

---

## Execution Order

| # | Task | Priority | Est. |
|---|---|---|---|
| 1 | 1.1 Clipped buttons | 🔴 | 5m |
| 2 | 1.2 Black background + env config | 🔴 | 10m |
| 3 | 1.3 Info panel bugs | 🔴 | 15m |
| 4 | 1.4 Button position | 🟡 | 10m |
| 5 | 1.5 Ghost clicks | 🔴 | 15m |
| 6 | 1.6 Slider vertical centering | 🟡 | 10m |
| 7 | 2.1 Cyan → White | 🔴 | 20m |
| 8 | 2.2 Gradient background | 🟡 | 30m |
| 9 | 3.1 Bottom bar container | 🟡 | 15m |
| 10 | 3.2 Freepik icons | 🟡 | 20m |
| 11 | 3.3 Grid submenu system | 🔴 | 45m |
| 12 | 4.1-4.3 Documentation | 🟢 | 30m |
| 13 | 5.1-5.2 RAG/Skills | 🟢 | 20m |

**Total: ~4 hours**
