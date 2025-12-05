# Presentation Assets Requirements
## WebGL Drone Viewer - Visual Resources Specification

---

## 1. Color Palette Export

### Figma/Design Tool Ready Colors

```json
{
  "palette": {
    "background": {
      "primary": "#0A0E17",
      "secondary": "#111827",
      "card": "#1E293B",
      "cardHover": "#334155"
    },
    "accent": {
      "blue": "#3B82F6",
      "blueLight": "#60A5FA",
      "teal": "#10B981",
      "tealLight": "#34D399",
      "orange": "#F97316",
      "magenta": "#EC4899"
    },
    "text": {
      "primary": "#F8FAFC",
      "secondary": "#94A3B8",
      "muted": "#64748B"
    },
    "border": {
      "default": "#334155",
      "light": "rgba(255,255,255,0.1)"
    }
  },
  "gradients": {
    "primary": "linear-gradient(135deg, #3B82F6 0%, #10B981 100%)",
    "glow": "linear-gradient(180deg, #3B82F6 0%, #EC4899 100%)",
    "mesh": "radial-gradient(at 20% 30%, rgba(59,130,246,0.3) 0%, transparent 50%), radial-gradient(at 80% 70%, rgba(16,185,129,0.3) 0%, transparent 50%)"
  }
}
```

---

## 2. Typography Specifications

### Font Files Required

| Font | Weights | Source |
|------|---------|--------|
| Space Grotesk | 400, 500, 600, 700 | Google Fonts |
| Inter | 400, 500, 600 | Google Fonts |
| JetBrains Mono | 400, 500 | Google Fonts |

### Download Links
- Space Grotesk: https://fonts.google.com/specimen/Space+Grotesk
- Inter: https://fonts.google.com/specimen/Inter
- JetBrains Mono: https://fonts.google.com/specimen/JetBrains+Mono

### Type Scale (px)

```
Display:    72px / 1.1 line-height / Bold
H1:         48px / 1.2 line-height / Bold
H2:         36px / 1.25 line-height / SemiBold
H3:         24px / 1.3 line-height / Medium
Body Large: 20px / 1.5 line-height / Regular
Body:       16px / 1.6 line-height / Regular
Caption:    14px / 1.4 line-height / Medium
Label:      14px / 1.4 line-height / Mono Regular
```

---

## 3. Icon Library

### Recommended Icon Sets (Free)

1. **Phosphor Icons** (Primary)
   - Style: Regular (2px stroke)
   - Size: 24px standard, 32px featured
   - URL: https://phosphoricons.com/

2. **Lucide** (Alternative)
   - Style: Default
   - URL: https://lucide.dev/

### Icons Needed (32 total)

#### Category: General UI
| Icon | Name | Usage |
|------|------|-------|
| 📄 | document | Problem slide |
| 🎯 | crosshair | Objectives |
| ✓ | check-circle | Completed items |
| ⚙️ | settings | Configuration |
| 👆 | pointer | Selection |
| 🔍 | search | Catalog |

#### Category: Project-Specific
| Icon | Name | Usage |
|------|------|-------|
| 🚁 | drone | Hero, features |
| 💻 | code | Development |
| 🧊 | cube | 3D elements |
| 🎨 | palette | Design/shaders |
| 📐 | ruler | Measurement |
| 🔗 | link | Connections |
| 📋 | list | Checklist |
| 📦 | package | Bill of materials |
| ✏️ | edit | Annotations |

#### Category: Technology
| Icon | Name | Usage |
|------|------|-------|
| ⚡ | zap | Unity/Power |
| 🌐 | globe | WebGL |
| 📱 | smartphone | Mobile |
| 🖥️ | monitor | Desktop |
| ☁️ | cloud | Deployment |

#### Category: Navigation
| Icon | Name | Usage |
|------|------|-------|
| ➡️ | arrow-right | Flow diagrams |
| ⬇️ | arrow-down | Hierarchy |
| ↗️ | arrow-up-right | External links |
| 🔀 | shuffle | Pipeline |

#### Category: Status
| Icon | Name | Usage |
|------|------|-------|
| ✅ | check | Success |
| ⚠️ | alert | Warning |
| ❌ | x | Error |
| ⏳ | clock | Timeline |
| 📊 | bar-chart | Metrics |

---

## 4. Diagram Templates

### 4.1 Mind Map (Slide 4)

```
Structure:
                    ┌─────────────┐
                    │   Teoría    │
                    │   Carga     │
                    │  Cognitiva  │
                    └──────┬──────┘
                           │
       ┌───────────────────┼───────────────────┐
       │                   │                   │
┌──────┴──────┐    ┌───────┴───────┐    ┌──────┴──────┐
│    HCI      │────│  Visualización │────│    PBR     │
│    3D       │    │  3D Interactiva│    │  Rendering │
└─────────────┘    └───────────────┘    └─────────────┘
                           │
                    ┌──────┴──────┐
                    │   WebGL     │
                    │   WASM      │
                    └─────────────┘

Visual specs:
- Node shape: Rounded rectangle
- Node fill: #1E293B
- Node border: 1px #334155
- Connection lines: 2px gradient (#3B82F6 → #10B981)
- Center node: Larger, gradient border
```

### 4.2 Timeline (Slide 5)

```
Structure:
┌─────┐         ┌─────┐         ┌─────┐         ┌─────┐
│ 📚  │────────▶│ 🧊  │────────▶│ 💻  │────────▶│ ✓  │
│     │         │     │         │     │         │     │
└──┬──┘         └──┬──┘         └──┬──┘         └──┬──┘
   │               │               │               │
Investigación   Pipeline 3D    Desarrollo     Validación
 Sem 1-4        Sem 5-8       Sem 9-16       Sem 17-20

Visual specs:
- Phase boxes: 80x80px, rounded 16px
- Icon size: 32px centered
- Connector line: 4px height, gradient fill
- Arrow heads: Triangle, 12px
- Labels: Below boxes, centered
- Spacing: 120px between nodes
```

### 4.3 Architecture Layers (Slide 7)

```
Structure:
┌────────────────────────────────────────────────────────┐
│                      UI Layer                          │
│            UIDocument  ·  Stylesheets                  │
├────────────────────────────────────────────────────────┤
│                 Application Layer                      │
│          AppStateMachine  ·  EventBus                  │
├────────────────────────────────────────────────────────┤
│                   Core Managers                        │
│  Selection · ViewMode · Exploded · CrossSection · ... │
├────────────────────────────────────────────────────────┤
│                   Content Layer                        │
│          ExplodablePart  ·  DronePartData             │
├────────────────────────────────────────────────────────┤
│                     Utilities                          │
│       Singleton  ·  TweenEngine  ·  ObjectPooler      │
└────────────────────────────────────────────────────────┘

Visual specs:
- Layer height: 80px
- Corner radius: 12px (only top layer corners)
- Colors by layer (top to bottom):
  - UI: #3B82F6 at 20% opacity
  - App: #8B5CF6 at 20% opacity
  - Core: #10B981 at 20% opacity
  - Content: #F97316 at 20% opacity
  - Utils: #64748B at 20% opacity
- Border between layers: 1px #334155
- Text: White, centered
```

### 4.4 Pipeline Flow (Slide 13)

```
Structure:
┌─────┐    ┌─────┐    ┌─────┐    ┌─────┐    ┌─────┐    ┌─────┐
│ CAD │───▶│Blend│───▶│ UV  │───▶│Bake │───▶│Comp │───▶│Unity│
└──┬──┘    └──┬──┘    └──┬──┘    └──┬──┘    └──┬──┘    └──┬──┘
   │          │          │          │          │          │
 Import   Retopology   Texel    Normal     ASTC/     Import
           & Clean    Density    Maps     Basis      & LOD

Visual specs:
- Step boxes: 64x64px, rounded 12px
- Colors: Gradient from left (#3B82F6) to right (#10B981)
- Arrows: 2px, same gradient
- Labels: Below, 14px caption
- Horizontal spacing: 100px
```

---

## 5. Screenshot/Mockup Requirements

### 5.1 UI Screenshot (Slide 14)

**Dimensions**: 1600 x 900 px
**Content should show**:
- Left sidebar with toolbar
- Central 3D viewport with drone
- Right panel with part info
- Bottom toolbar

**Style**:
- Dark theme matching design system
- Glassmorphism panels
- Blue/teal accents
- Example drone model in viewport

### 5.2 View Mode Comparison (Slide 8)

**Dimensions**: 7 thumbnails at 200 x 120 px each
**Content**:
- Same camera angle for all 7
- Each showing different view mode
- Labels below

### 5.3 Shader Before/After (Slide 12)

**Dimensions**: 600 x 400 px each (side by side)
**Content**:
- Left: Code editor view
- Right: Rendered result

---

## 6. Background Elements

### 6.1 Mesh Gradient Background

**Usage**: Title slide, thank you slide
**Specs**:
```css
background: 
  radial-gradient(ellipse at 20% 30%, rgba(59,130,246,0.15) 0%, transparent 50%),
  radial-gradient(ellipse at 80% 70%, rgba(16,185,129,0.15) 0%, transparent 50%),
  radial-gradient(ellipse at 50% 50%, rgba(236,72,153,0.08) 0%, transparent 50%),
  #0A0E17;
```

### 6.2 Grid Pattern Overlay

**Usage**: Tech slides (6, 12)
**Specs**:
- Dot grid: 4px dots, 32px spacing
- Color: rgba(255,255,255,0.03)
- Alternatively: Fine line grid 1px, 48px spacing

### 6.3 Abstract Drone Silhouette

**Usage**: Title slide background
**Specs**:
- Simplified quadcopter outline
- Color: rgba(255,255,255,0.05)
- Size: Cover 60% of slide
- Position: Center-right, rotated 15°

---

## 7. Component Templates

### 7.1 Glass Card

```css
.glass-card {
  background: rgba(30, 41, 59, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  padding: 24px;
  backdrop-filter: blur(12px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}
```

### 7.2 Gradient Border Card

```css
.gradient-border-card {
  background: #1E293B;
  border-radius: 16px;
  padding: 24px;
  position: relative;
}
.gradient-border-card::before {
  content: '';
  position: absolute;
  inset: -2px;
  border-radius: 18px;
  background: linear-gradient(135deg, #3B82F6, #10B981);
  z-index: -1;
}
```

### 7.3 Stat Card

```
┌─────────────────────┐
│        70+          │  ← 48px, Bold
│      Scripts        │  ← 16px, Secondary
└─────────────────────┘

Width: 200px
Height: 120px
Background: #1E293B
Border-radius: 12px
```

### 7.4 Feature Card

```
┌─────────────────────────────┐
│   [Icon 32px]               │
│                             │
│   Feature Title             │  ← 20px, Bold
│   Brief description here    │  ← 14px, Secondary
│   in one or two lines       │
└─────────────────────────────┘

Width: 280px
Height: 160px
Padding: 24px
Icon color: #3B82F6
```

---

## 8. Animation Presets

### 8.1 Element Entrance
```css
@keyframes fadeUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
/* Duration: 600ms, Easing: ease-out */
```

### 8.2 Scale In
```css
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}
/* Duration: 400ms, Easing: ease-out */
```

### 8.3 Stagger Reveal
```
Element 1: delay 0ms
Element 2: delay 100ms
Element 3: delay 200ms
Element 4: delay 300ms
...
```

---

## 9. Export Checklist

### Before Kimi Generation
- [ ] All fonts downloaded and installed
- [ ] Color palette documented
- [ ] Icon set downloaded (32 icons minimum)
- [ ] Mind map diagram template ready
- [ ] Timeline diagram template ready
- [ ] Architecture diagram template ready
- [ ] Pipeline flow diagram template ready

### After Kimi Generation
- [ ] Replace placeholder images
- [ ] Adjust any off-brand colors
- [ ] Fine-tune typography spacing
- [ ] Add custom diagrams
- [ ] Insert screenshots
- [ ] Review contrast accessibility
- [ ] Export PDF for print
- [ ] Export PPTX for presentation
- [ ] Create PNG backups of each slide

---

## 10. Quick Reference Sheet

```
╔════════════════════════════════════════════════════════════╗
║              PRESENTATION QUICK REFERENCE                  ║
╠════════════════════════════════════════════════════════════╣
║  DIMENSIONS        1920 x 1080 px (16:9)                  ║
║  MARGINS           80px all sides                          ║
║  GRID              12 columns, 24px gutters                ║
╠════════════════════════════════════════════════════════════╣
║  BG PRIMARY        #0A0E17                                 ║
║  ACCENT BLUE       #3B82F6                                 ║
║  ACCENT TEAL       #10B981                                 ║
║  TEXT PRIMARY      #F8FAFC                                 ║
╠════════════════════════════════════════════════════════════╣
║  HEADLINE FONT     Space Grotesk Bold                      ║
║  BODY FONT         Inter Regular                           ║
║  CODE FONT         JetBrains Mono                          ║
╠════════════════════════════════════════════════════════════╣
║  BORDER RADIUS     12-16px                                 ║
║  SHADOW            0 8px 32px rgba(0,0,0,0.3)             ║
║  BLUR              12px backdrop                           ║
╚════════════════════════════════════════════════════════════╝
```

---

*Assets Document Version: 1.0*
*Last Updated: December 2024*
