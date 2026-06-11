# Presentation Assets Requirements
## TwinSight X500 - Visual Resources Specification

---

## Estado

Este documento define los recursos visuales para la defensa usando la realidad actual de la app. Reemplaza requisitos antiguos que pedian siete miniaturas de modos visibles, herramientas de medicion/BOM o stat cards con metricas no trazadas al capitulo 5 o a los anexos de validacion.

---

## 1. Design Tokens

```json
{
  "palette": {
    "background": "#0A0E17",
    "card": "#1E293B",
    "cardHover": "#334155",
    "accentBlue": "#3B82F6",
    "accentTeal": "#10B981",
    "accentOrange": "#F97316",
    "textPrimary": "#F8FAFC",
    "textSecondary": "#94A3B8",
    "border": "#334155"
  },
  "typography": {
    "headline": "Space Grotesk",
    "body": "Inter",
    "mono": "JetBrains Mono"
  }
}
```

Use dark technical visuals with clear contrast. Avoid decorative claims or fake dashboards.

---

## 2. Required Screenshots

| Asset | Purpose | Required State |
|-------|---------|----------------|
| `hero_explore.png` | Introduce the product | Hero or Explore state with full drone visible |
| `selection_bottom_sheet.png` | Explain inspection | A selected piece with bottom sheet metadata |
| `fastener_isolate.png` | Explain fastener detail | Isolated fastener or modular placeholder active |
| `mother_piece_fasteners.png` | Explain hierarchy | Isolated mother piece with associated fasteners |
| `analyze_explode.png` | Explain assembly reading | Explode active with readable separation |
| `analyze_cross_section.png` | Explain shader cut | Cross-section plane active |
| `analyze_filters.png` | Explain functional categories | Category filters visible |
| `studio_modes.png` | Explain visual reading | Realistic, X-Ray, Solid Color or Thermal |
| `thermal_legend.png` | Explain thermal heuristic | Thermal mode with legend |
| `blender_final_model.png` | Explain asset pipeline | Final Blender viewport or exported FBX preview |

Do not use old mockups that show sidebars, right panels or tools absent from the current UI.

---

## 3. Required Diagrams

### 3.1 Conceptual Problem

Manual 2D -> mental reconstruction -> interactive 3D inspection.

### 3.2 Runtime Architecture

UI Toolkit -> managers -> `DronePartData` / fastener registries -> scene objects -> shaders.

### 3.3 App Flow

```text
Hero -> Explore -> Selection -> Bottom Sheet -> Inspect / Analyze / Studio
```

### 3.4 Fastener System

```text
FastenerInstanceDefinition -> FastenerRegistry -> FastenerInspectionManager
                       \-> FastenerBuilder / Blender recipe asset
```

Show the rule: proxy in rest state, modular detail only for selected/isolated/context fasteners.

### 3.5 Blender-to-Unity Pipeline

```text
CAD / reference -> Blender optimization -> UV atlas -> bake
-> BaseColor / Normal / Mask -> FBX runtime -> Unity import report
```

State explicitly that runtime export includes both masters and instances:

- `BAKE_MASTERS_LOW`
- `ASSEMBLY_INSTANCES_LOW`
- `PRIMITIVE_FASTENER_MASTERS`
- `PRIMITIVE_FASTENER_INSTANCES`

---

## 4. Slide-Specific Visual Requirements

| Slide | Visual Needed | Notes |
|-------|---------------|-------|
| 2 Problem | 2D vs 3D comparison | Use a real manual page or neutral 2D schematic and app screenshot |
| 7 Architecture | Layer diagram | Avoid outdated manager counts |
| 8 Visual Reading | 4-mode grid + environment strip | Do not label as "7 public modes" |
| 10 Modules | Inspect/Analyze/Studio cards | Use actual tools only |
| 11 Fasteners | Metadata + isolate visual | Mention placeholders honestly |
| 13 Pipeline | Blender/Unity flow | Include masters+instances rule |
| 16 Validation | SUS, NASA-TLX, Think-Aloud and profiler evidence | Use only values from Chapter 5 or validation annexes |
| 17 Results | Evidence dashboard | Mark non-measured items as pending/future |

---

## 5. Icons

Use Phosphor or Lucide style icons. Required categories:

- Problem: document, brain, cube.
- Method: timeline, check, target.
- App: cursor, layers, eye, filter, scissors/cut, expand/explode.
- Fasteners: screw/nut or generic component icon.
- Pipeline: cube, image, package, upload/import.
- Validation: chart, gauge, user, comment.

Avoid icons for BOM, measurement or connection points unless those items are explicitly shown as future work.

---

## 6. Metrics Cards

Do not create stat cards with unverified numbers.

Allowed labels when no validated value exists:

- `FPS promedio: pendiente sin dato trazable`
- `Frame time: pendiente sin dato trazable`
- `Peso final assets: pendiente sin dato trazable`
- `SUS: usar capitulo 5 o pendiente si es nueva prueba`
- `NASA-TLX Raw: usar capitulo 5 o pendiente si es nueva prueba`

Allowed validated labels for the June 2026 report:

- `SUS promedio: 91,88`
- `SUS mediana: 95,00`
- `NASA-TLX Raw 3D: 8,69`
- `NASA-TLX Raw 2D: 19,89`
- `Diferencia pareada NASA-TLX: 11,19`
- `Rendimiento: citar por dispositivo, no como compatibilidad universal`

Allowed fixed counts only when they are sourced from the current documentation:

- `28 piezas canonicas`
- `30 anchors de escena`
- `20 familias de fasteners`
- `168 instancias baseline Unity`
- `142 fasteners primitivos Blender pendientes de reconciliacion` if the manifest/import report confirms the same value.

---

## 7. Export Checklist

- [ ] Screenshots captured from the actual app/build.
- [ ] No old UI mockups with right panels or tools not visible.
- [ ] No "7 public view modes" claim.
- [ ] No measured values invented outside Chapter 5, validation annexes or current profiler evidence.
- [ ] Thermal labeled as heuristic visual analysis.
- [ ] Fastener placeholders labeled as temporary when shown.
- [ ] Blender import status aligned with the latest Unity report.
- [ ] All diagrams use the same terminology as report, manuals and Obsidian study notes.

---

*Assets Document Version: 2.1*
*Last Updated: 2026-06-10*
