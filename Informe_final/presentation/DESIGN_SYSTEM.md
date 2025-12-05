# Thesis Presentation Design System
## WebGL Drone Viewer - Visual Identity Guide

> Complete design system for creating a professional, modern presentation following 2026 design trends.

---

## 1. Design Philosophy

### Core Principles

| Principle | Description |
|-----------|-------------|
| **Minimalism** | Less is more. Every element must earn its place. |
| **Breathing Room** | Generous white space creates elegance and focus. |
| **Visual Hierarchy** | Clear distinction between primary, secondary, and tertiary information. |
| **Consistency** | Unified visual language across all slides. |
| **Accessibility** | 4.5:1 contrast ratio minimum for text legibility. |

### 2026 Design Trends Applied

1. **Neo-brutalism with refinement**: Bold typography with subtle elegance
2. **Glassmorphism 2.0**: Frosted glass effects with depth layers
3. **Gradient meshes**: Organic, flowing color transitions
4. **Micro-interactions implied**: Design suggests motion and interactivity
5. **3D integration**: Seamless blend of 3D renders with 2D graphics
6. **Dark mode first**: Rich dark themes with vibrant accents
7. **Variable fonts**: Dynamic typography that breathes
8. **Asymmetric layouts**: Intentional imbalance for visual interest

---

## 2. Color Palette

### Primary Colors

```
┌─────────────────────────────────────────────────────────────┐
│  DEEP SPACE          ELECTRIC BLUE        CYBER TEAL       │
│  #0A0E17             #3B82F6              #10B981          │
│  Background          Primary Accent       Secondary Accent  │
│  RGB: 10, 14, 23     RGB: 59, 130, 246    RGB: 16, 185, 129│
└─────────────────────────────────────────────────────────────┘
```

### Extended Palette

| Name | Hex | Usage |
|------|-----|-------|
| **Deep Space** | `#0A0E17` | Primary background |
| **Void** | `#111827` | Secondary background |
| **Slate Dark** | `#1E293B` | Card backgrounds |
| **Slate Mid** | `#334155` | Borders, dividers |
| **Slate Light** | `#64748B` | Muted text |
| **Silver** | `#94A3B8` | Secondary text |
| **Cloud** | `#E2E8F0` | Primary text |
| **Pure White** | `#F8FAFC` | Headings, emphasis |
| **Electric Blue** | `#3B82F6` | Primary accent |
| **Electric Blue Glow** | `#60A5FA` | Hover states |
| **Cyber Teal** | `#10B981` | Success, highlights |
| **Sunset Orange** | `#F97316` | Warnings, CTAs |
| **Magenta Pulse** | `#EC4899` | Special emphasis |

### Gradient Definitions

```css
/* Primary Gradient - Hero elements */
background: linear-gradient(135deg, #3B82F6 0%, #10B981 100%);

/* Glow Gradient - Accents */
background: linear-gradient(180deg, #3B82F6 0%, #EC4899 100%);

/* Mesh Gradient - Backgrounds */
background: 
  radial-gradient(at 20% 30%, #3B82F6 0%, transparent 50%),
  radial-gradient(at 80% 70%, #10B981 0%, transparent 50%),
  radial-gradient(at 50% 50%, #EC4899 0%, transparent 50%),
  #0A0E17;
```

---

## 3. Typography

### Font Stack

| Level | Font | Weight | Size | Line Height |
|-------|------|--------|------|-------------|
| **Display** | Space Grotesk | Bold (700) | 72px | 1.1 |
| **H1** | Space Grotesk | Bold (700) | 48px | 1.2 |
| **H2** | Space Grotesk | SemiBold (600) | 36px | 1.25 |
| **H3** | Space Grotesk | Medium (500) | 24px | 1.3 |
| **Body Large** | Inter | Regular (400) | 20px | 1.5 |
| **Body** | Inter | Regular (400) | 16px | 1.6 |
| **Caption** | Inter | Medium (500) | 14px | 1.4 |
| **Label** | JetBrains Mono | Regular (400) | 14px | 1.4 |

### Font Pairing Rationale

- **Space Grotesk**: Geometric sans-serif with character. Technical yet approachable.
- **Inter**: Highly legible at all sizes. Perfect for body text.
- **JetBrains Mono**: For code snippets and technical labels.

### Text Styling Rules

1. **Never use more than 2 font families per slide**
2. **Maximum 3 type sizes per slide**
3. **Left-align body text, center headlines only when isolated**
4. **Line length: 45-75 characters maximum**
5. **Letter spacing: +2% for all-caps text**

---

## 4. Layout Grid System

### Slide Dimensions

```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│   Aspect Ratio: 16:9                                          │
│   Resolution: 1920 x 1080 px                                   │
│   Safe Zone: 1760 x 920 px (80px margin all sides)            │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

### Grid Specifications

```
12-Column Grid
├── Columns: 12
├── Column Width: 120px
├── Gutter: 24px
├── Outer Margin: 80px (left/right), 80px (top/bottom)
└── Content Width: 1760px maximum

Vertical Rhythm
├── Base Unit: 8px
├── Small spacing: 8px (1 unit)
├── Medium spacing: 16px (2 units)
├── Large spacing: 32px (4 units)
├── XL spacing: 64px (8 units)
└── Section spacing: 80px (10 units)
```

### Layout Templates

#### Template A: Title Slide
```
┌──────────────────────────────────────────────────────────────┐
│                                                              │
│                                                              │
│                    [MAIN TITLE]                              │
│                    ───────────                               │
│                    [Subtitle]                                │
│                                                              │
│                                                              │
│  [Logo]                              [Date] · [Author]       │
└──────────────────────────────────────────────────────────────┘
```

#### Template B: Content with Visual (60/40 Split)
```
┌──────────────────────────────────────────────────────────────┐
│  [Title]                                                     │
│                                                              │
│  ┌─────────────────────────┐  ┌──────────────────────────┐  │
│  │                         │  │                          │  │
│  │      TEXT CONTENT       │  │      VISUAL/IMAGE        │  │
│  │      (60% width)        │  │      (40% width)         │  │
│  │                         │  │                          │  │
│  └─────────────────────────┘  └──────────────────────────┘  │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

#### Template C: Full Visual with Overlay
```
┌──────────────────────────────────────────────────────────────┐
│  ════════════════════════════════════════════════════════    │
│  ║                  FULL BLEED IMAGE                    ║    │
│  ║                                                      ║    │
│  ║    ┌──────────────────────────┐                     ║    │
│  ║    │  Glass Panel Overlay     │                     ║    │
│  ║    │  with key text           │                     ║    │
│  ║    └──────────────────────────┘                     ║    │
│  ║                                                      ║    │
│  ════════════════════════════════════════════════════════    │
└──────────────────────────────────────────────────────────────┘
```

#### Template D: Data/Comparison (Grid)
```
┌──────────────────────────────────────────────────────────────┐
│  [Title]                                                     │
│                                                              │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐         │
│  │  Card 1 │  │  Card 2 │  │  Card 3 │  │  Card 4 │         │
│  │         │  │         │  │         │  │         │         │
│  └─────────┘  └─────────┘  └─────────┘  └─────────┘         │
│                                                              │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐         │
│  │  Card 5 │  │  Card 6 │  │  Card 7 │  │  Card 8 │         │
│  └─────────┘  └─────────┘  └─────────┘  └─────────┘         │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## 5. Component Library

### 5.1 Cards

```
┌─────────────────────────────────┐
│  ┌ Glass Card ────────────────┐ │
│  │                            │ │
│  │  Background: #1E293B (85%) │ │
│  │  Border: 1px #334155       │ │
│  │  Border-radius: 16px       │ │
│  │  Padding: 24px             │ │
│  │  Shadow: 0 8px 32px        │ │
│  │          rgba(0,0,0,0.3)   │ │
│  │                            │ │
│  └────────────────────────────┘ │
└─────────────────────────────────┘
```

### 5.2 Buttons

| State | Background | Border | Text |
|-------|------------|--------|------|
| Default | `#3B82F6` | none | `#FFFFFF` |
| Hover | `#60A5FA` | none | `#FFFFFF` |
| Ghost | transparent | `1px #3B82F6` | `#3B82F6` |

```
Button Specs:
├── Height: 48px
├── Padding: 16px 32px
├── Border-radius: 8px
├── Font: Inter Medium 16px
└── Text-transform: none
```

### 5.3 Tags/Badges

```
┌─────────────┐
│  Category   │  ← Small tag
└─────────────┘
├── Height: 28px
├── Padding: 4px 12px
├── Border-radius: 14px (pill)
├── Background: rgba(59, 130, 246, 0.2)
├── Text: #60A5FA
└── Font: Inter Medium 12px
```

### 5.4 Progress Indicators

```
┌──────────────────────────────────────────────────────────────┐
│  ████████████████████░░░░░░░░░░░░░░  65%                    │
└──────────────────────────────────────────────────────────────┘
├── Height: 8px
├── Background: #1E293B
├── Fill: gradient(#3B82F6 → #10B981)
├── Border-radius: 4px
└── Label: Inter Medium 14px, right-aligned
```

### 5.5 Icons

- **Style**: Outlined, 2px stroke
- **Size**: 24px (standard), 32px (featured)
- **Color**: Inherit from text or accent
- **Source**: Phosphor Icons, Lucide, or Heroicons

### 5.6 Dividers

```
Horizontal: 
────────────────────────────────────────
├── Height: 1px
├── Color: #334155
└── Margin: 32px 0

Decorative:
══════════════════════════════════════
├── Height: 2px
├── Color: gradient(#3B82F6 → #10B981)
└── Width: 120px (centered)
```

---

## 6. Visual Elements

### 6.1 Images & Screenshots

| Element | Specification |
|---------|--------------|
| Border-radius | 12px for screenshots |
| Shadow | `0 16px 48px rgba(0,0,0,0.4)` |
| Overlay | Optional gradient for text readability |
| Frame | Optional 1px `#334155` border |

### 6.2 3D Renders

- **Background**: Transparent or matching slide background
- **Lighting**: Consistent 3-point lighting
- **Shadows**: Soft, directional, matching slide atmosphere
- **Angle**: 3/4 view for objects, consistent across slides

### 6.3 Diagrams & Charts

```
Color Mapping for Data Visualization:
├── Primary data:   #3B82F6
├── Secondary data: #10B981
├── Tertiary data:  #EC4899
├── Quaternary:     #F97316
├── Neutral:        #64748B
└── Background:     #1E293B
```

### 6.4 Code Blocks

```
┌─────────────────────────────────────────────────────────────┐
│  ┌─ Code Block ───────────────────────────────────────────┐ │
│  │  Background: #0F172A                                   │ │
│  │  Border: 1px #1E293B                                   │ │
│  │  Border-radius: 8px                                    │ │
│  │  Padding: 24px                                         │ │
│  │  Font: JetBrains Mono 14px                            │ │
│  │  Syntax highlighting: One Dark Pro theme              │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. Animation & Transitions

### Slide Transitions

| Transition | Duration | Easing | Use Case |
|------------|----------|--------|----------|
| Fade | 400ms | ease-out | Default between most slides |
| Slide Left | 500ms | ease-in-out | Sequential content |
| Zoom | 300ms | ease-out | Emphasis slides |
| None | 0ms | - | Demo/video slides |

### Element Animations

| Animation | Description | Duration |
|-----------|-------------|----------|
| Fade Up | Elements fade in while moving up 20px | 600ms |
| Scale In | Elements scale from 0.95 to 1.0 | 400ms |
| Stagger | Sequential reveal with 100ms delay between items | varies |

### Motion Principles

1. **Purpose**: Animation must communicate, not decorate
2. **Subtlety**: Keep movement minimal and elegant
3. **Speed**: Fast enough to feel responsive, slow enough to perceive
4. **Consistency**: Same animation for same type of element

---

## 8. Content Guidelines

### Text Density Rules

| Slide Type | Max Words | Max Bullets |
|------------|-----------|-------------|
| Title | 10 | 0 |
| Section | 15 | 0 |
| Content | 50 | 5 |
| Data | 30 | 6 |
| Quote | 25 | 0 |

### The "Billboard Test"

> If you can't read and understand the slide in 3 seconds from 6 feet away, simplify it.

### Bullet Point Rules

1. **No full sentences** - Use fragments
2. **Parallel structure** - Start each with same word type
3. **Maximum 7 words** per bullet
4. **Icons preferred** over bullet characters
5. **One idea** per bullet

### Visual-to-Text Ratio

- **Target**: 60% visual, 40% text
- **Minimum visual**: 30% of slide area
- **Never**: Full-text slides (except quotes)

---

## 9. Slide-by-Slide Specifications

### Slide 1: Title
- **Layout**: Centered, vertical stack
- **Background**: Mesh gradient with subtle animation
- **Elements**: Title (Display), Subtitle (H3), Author info (Caption)
- **Visual**: Abstract 3D drone silhouette, ghosted, 20% opacity

### Slide 2: Problem Statement
- **Layout**: 50/50 split
- **Left**: Problem text with icon bullets
- **Right**: Visualization of 2D vs 3D comparison
- **Accent**: Red/orange tones for "problem" emphasis

### Slide 3: Objectives
- **Layout**: Numbered cards (2x2 grid)
- **Style**: Glass cards with gradient borders
- **Icons**: Checkmark icon on each
- **Animation**: Stagger reveal

### Slide 4: Theoretical Framework
- **Layout**: Mind map or connected nodes
- **Style**: Organic connections with gradient lines
- **Visual**: Central concept with radiating topics

### Slide 5: Methodology
- **Layout**: Horizontal timeline
- **Style**: Connected steps with progress indicator
- **Visual**: Phase icons connected by gradient line

### Slide 6: Technology Stack
- **Layout**: Icon grid (3x2)
- **Style**: Technology logos with labels
- **Background**: Subtle code pattern overlay

### Slide 7: Architecture
- **Layout**: Full-width diagram
- **Style**: Simplified Mermaid-style flowchart
- **Colors**: Grouped by layer with color coding

### Slide 8: View Modes (7 modes)
- **Layout**: 7-column grid (tight)
- **Style**: Screenshot thumbnails with labels
- **Animation**: Quick carousel or static grid

### Slide 9: Interactions
- **Layout**: 60/40 (text/visual)
- **Visual**: GIF or video placeholder
- **Points**: 4 key features with icons

### Slide 10: Engineer Tools
- **Layout**: 2x3 card grid
- **Style**: Tool icon + name + one-line description
- **Accent**: Teal for productivity tools

### Slide 11: Metrics
- **Layout**: Dashboard style
- **Style**: Big numbers with labels
- **Visual**: Progress bars or gauges

### Slide 12: Shaders
- **Layout**: Before/after comparison
- **Visual**: Shader code snippet + rendered result
- **Style**: Side-by-side panels

### Slide 13: 3D Pipeline
- **Layout**: Horizontal flow
- **Style**: Step-by-step with arrows
- **Visual**: Progress icons

### Slide 14: User Interface
- **Layout**: Full bleed screenshot
- **Style**: Glass overlay for callouts
- **Annotations**: Pointer lines to key features

### Slide 15: Live Demo
- **Layout**: Minimal - large "DEMO" text
- **Style**: Clean transition slide
- **Notes**: Speaker reminder only

### Slide 16: Validation
- **Layout**: Split metrics/methodology
- **Style**: Test results with visual indicators
- **Visual**: SUS score gauge

### Slide 17: Results
- **Layout**: Numbered achievements
- **Style**: Success checkmarks
- **Visual**: Summary icons

### Slide 18: Conclusions
- **Layout**: Bullet-free, statement cards
- **Style**: Quote-like presentation
- **Visual**: Essence of project

### Slide 19: Future Work
- **Layout**: Roadmap timeline
- **Style**: Forward-pointing arrows
- **Mood**: Aspirational

### Slide 20: Thank You
- **Layout**: Centered, minimal
- **Elements**: Thank you, QR code, contact info
- **Visual**: Subtle background animation

---

## 10. File Naming & Organization

```
/presentation/
├── /assets/
│   ├── /images/
│   │   ├── slide-01-hero.png
│   │   ├── slide-08-viewmodes.png
│   │   └── ...
│   ├── /icons/
│   │   └── icon-set.svg
│   ├── /screenshots/
│   │   └── app-screenshot-01.png
│   └── /3d-renders/
│       └── drone-hero.png
├── /fonts/
│   ├── SpaceGrotesk-Variable.ttf
│   ├── Inter-Variable.ttf
│   └── JetBrainsMono-Regular.ttf
└── presentation-final.pptx
```

---

## 11. Export Specifications

| Format | Resolution | Use Case |
|--------|------------|----------|
| PDF | 1920x1080 | Print, sharing |
| PPTX | 1920x1080 | Live presentation |
| PNG (slides) | 3840x2160 | High-res backup |
| MP4 | 1920x1080, 30fps | Video version |

---

## 12. Accessibility Checklist

- [ ] Contrast ratio ≥ 4.5:1 for body text
- [ ] Contrast ratio ≥ 3:1 for large text
- [ ] No information conveyed by color alone
- [ ] All images have alt text
- [ ] Font size minimum 16px
- [ ] Animations can be disabled
- [ ] Logical reading order maintained

---

## Quick Reference Card

```
┌──────────────────────────────────────────────────────────────┐
│                     DESIGN SYSTEM QUICK REF                  │
├──────────────────────────────────────────────────────────────┤
│  COLORS                                                      │
│  Background: #0A0E17    Primary: #3B82F6    Accent: #10B981 │
├──────────────────────────────────────────────────────────────┤
│  TYPOGRAPHY                                                  │
│  Headlines: Space Grotesk Bold                              │
│  Body: Inter Regular                                         │
│  Code: JetBrains Mono                                       │
├──────────────────────────────────────────────────────────────┤
│  SPACING                                                     │
│  Margins: 80px    Gutters: 24px    Base: 8px               │
├──────────────────────────────────────────────────────────────┤
│  COMPONENTS                                                  │
│  Border-radius: 8-16px    Shadow: 0 8px 32px               │
└──────────────────────────────────────────────────────────────┘
```

---

*Document Version: 1.0*  
*Created: December 2024*  
*Project: WebGL Drone Viewer Thesis Presentation*
