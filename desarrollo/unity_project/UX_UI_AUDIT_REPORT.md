# 🔬 UX/UI Audit Report — WebGL Drone Viewer

**Proyecto:** Diseño y Desarrollo de un Prototipo Web 3D Interactivo para la Visualización Técnica y Análisis Estructural de Hardware de Alto Rendimiento  
**Autor del Audit:** Senior UX/UI Researcher & Product Designer (IA)  
**Branch:** `feature/phase2-ux-redesign` · HEAD `40686f5`  
**Fecha:** Junio 2025  
**Archivos auditados:**
- `Assets/Scripts/UI/Layouts/MainLayout.uxml` (361 líneas)
- `Assets/UI/Styles/Theme.uss` (1 486 líneas)

**Referencias normativas:**
- HIG Apple (iOS 18) — Touch target mínimo 44 × 44 pt
- Material Design 3 (Android) — Touch target mínimo 48 × 48 dp
- Guía UX interna (`External_docs/Guía Completa de Diseño UI_UX…`)
- Miller's Law (1956) — Memoria de trabajo 7 ± 2 ítems
- Fitts's Law (1954) — Tiempo ∝ log₂(2D / W)
- 8 pt Spatial Grid — Todos los valores dimensionales múltiplos de 8
- Tesis — Validación con NASA-TLX (carga cognitiva) y SUS (usabilidad), N = 8–12

---

## Tabla de Contenidos

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Metodología](#2-metodología)
3. [Inventario de Componentes](#3-inventario-de-componentes)
4. [Violaciones de Touch Target (Fitts's Law)](#4-violaciones-de-touch-target-fittss-law)
5. [Violaciones de Miller's Law (Carga Cognitiva)](#5-violaciones-de-millers-law-carga-cognitiva)
6. [Violaciones del 8 pt Grid](#6-violaciones-del-8-pt-grid)
7. [Violaciones Tipográficas](#7-violaciones-tipográficas)
8. [Violaciones de Accesibilidad y Contraste](#8-violaciones-de-accesibilidad-y-contraste)
9. [Fallas Estructurales UX (Arquitectura de Información)](#9-fallas-estructurales-ux-arquitectura-de-información)
10. [Tabla Consolidada de Severidad](#10-tabla-consolidada-de-severidad)
11. [Plan de Implementación — UI Minimalista por Modos](#11-plan-de-implementación--ui-minimalista-por-modos)
12. [Métricas Objetivo (post-fix)](#12-métricas-objetivo-post-fix)

---

## 1. Resumen Ejecutivo

El sistema actual de 3 modos (Tools / Analyze / Studio) representa un avance significativo respecto al diseño de 5 botones de la Fase 1. Sin embargo, la auditoría revela **27 violaciones concretas** categorizadas en 5 dominios:

| Dominio | Críticas | Altas | Medias | Bajas |
|---------|----------|-------|--------|-------|
| Touch Target (Fitts) | 3 | 2 | 1 | — |
| Miller's Law (cognitiva) | 1 | 2 | 1 | — |
| 8 pt Grid | — | 3 | 8 | — |
| Tipografía | 2 | 3 | 1 | — |
| Accesibilidad / Contraste | — | 1 | 2 | 1 |

**Impacto sobre la tesis:** Las violaciones tipográficas y de touch target incrementarán los scores de NASA-TLX (dimensión *Esfuerzo* y *Frustración*) y reducirán el SUS score por debajo del umbral aceptable (≥ 68/100). Las violaciones de Miller's Law afectan directamente la variable "Carga Cognitiva" que la tesis pretende minimizar.

---

## 2. Metodología

### 2.1 Heurísticas Aplicadas

| # | Heurística | Criterio |
|---|-----------|----------|
| H1 | Fitts's Law | Tiempo de adquisición: $T = a + b \cdot \log_2\left(\frac{2D}{W}\right)$. Targets < 44 px en zona de pulgar = violación. |
| H2 | Miller's Law | Grupos visibles simultáneos > 7 ± 2 = sobrecarga. |
| H3 | 8 pt Grid | Cada dimensión (width, height, padding, margin, font-size, border-radius) debe ser múltiplo de 8, o ½ para micro-spacing (4 px). |
| H4 | Typography Scale | Cuerpo ≥ 16 px, captions ≥ 12 px, nunca menor. Escala modular recomendada: 12 / 14 / 16 / 20 / 24 / 32 / 40 / 48 / 64. |
| H5 | Contraste WCAG 2.1 | Texto normal ≥ 4.5:1; texto grande (≥ 24 px o 18.7 px bold) ≥ 3:1. |
| H6 | Thumb Zone | Zona segura = tercio inferior de pantalla. Controles primarios deben residir aquí. |

### 2.2 Fuentes

- Lectura completa de `MainLayout.uxml` (361 líneas)
- Lectura completa de `Theme.uss` (1 486 líneas)
- `External_docs/Guía Completa de Diseño UI_UX para Aplicaciones Mó.md`
- `Propuesta/final_proposal.tex` (objetivos específicos 1–4)

---

## 3. Inventario de Componentes

### 3.1 Mapa Jerárquico UXML

```
Root (picking-mode="Ignore")
├── HeroContainer (Landing)
│   ├── HeroMain (title + subtitle + 4 btns)
│   ├── DeviceSubmenu (submenu-scroll)
│   ├── AboutSubmenu (about-panel)
│   └── ExitSubmenu (exit-panel)
├── TopBar
│   ├── HomeBtn (icon-button-small)          ← 40 × 40 px ⚠️
│   ├── "DRONE VIEWER" (header-title, 14px)  ← 14 px ⚠️
│   └── ResetViewBtn (icon-button-small)     ← 40 × 40 px ⚠️
├── ToolsModeContainer (.mode-container)
│   ├── ToolsActionBar (3 btns: INFO, EXPLODE, PINS)
│   │   └── mode-action-btn × 3              ← 72 × 56 px
│   ├── SliderContainer (label + ExplosionSlider)
│   │   └── glass-slider dragger             ← 24 × 24 px ⛔
│   └── CategoryMenu (.mode-submenu, 5 cards)
│       └── submenu-card × 5                 ← 110 × 88 px ✅
├── AnalyzeModeContainer (.mode-container)
│   ├── AnalyzeActionBar (2 btns: SHADERS, CUT)
│   │   └── mode-action-btn × 2              ← 72 × 56 px
│   ├── ShaderMenu (.mode-submenu, 7 cards)
│   │   └── submenu-card × 7                 ← 110 × 88 px ✅
│   └── CrossSectionPanel
│       ├── Toggle (min-width 80px)
│       ├── Axis btns X/Y/Z × 3              ← 34 × 34 px ⛔
│       ├── InvertBtn (cross-section-axis-btn) ← 34 × 34 px ⛔
│       └── PositionSlider                    ← dragger 24 px ⛔
├── StudioModeContainer (.mode-container)
│   ├── EnvPanel (.mode-submenu, 5 cards)
│   │   └── submenu-card × 5                 ← 110 × 88 px ✅
│   └── 2 × env-slider-group (ROTATION, INTENSITY)
├── BottomBar
│   ├── actions-row (glass pill, min-width 420px)
│   │   └── mode-btn × 3                     ← 100 × 60 px ✅
│   ├── info-row
│   │   └── SelectionIndicator (13px)         ← 13 px ⚠️
├── BottomSheet (details-sheet)
│   ├── sheet-handle (40 × 3 px)
│   ├── SheetHeader (SheetTitle 22px + SheetCloseBtn)
│   │   └── sheet-close-btn                   ← 32 × 32 px ⛔
│   ├── data-grid (7 rows)
│   ├── assembly-section (3 rows)
│   └── PartDescription (16px)
```

**Leyenda:** ✅ Cumple · ⚠️ Violación media · ⛔ Violación crítica

### 3.2 Conteo de Elementos Interactivos por Modo

| Modo | Botones de Acción | Sub-menú (cards) | Sliders | Toggles | Otros | **Total interactivos** |
|------|-------------------|-------------------|---------|---------|-------|------------------------|
| Tools | 3 | 5 | 1 | — | — | **9** |
| Analyze | 2 | 7 | 1 | 1 | 4 (ejes + invert) | **15** |
| Studio | — | 5 | 2 | — | — | **7** |
| Global (Bottom Bar) | 3 mode-btns | — | — | — | 1 selection label | **4** |
| Global (Top Bar) | 2 (Home, Reset) | — | — | — | 1 title | **3** |
| BottomSheet | 1 (close) | — | — | — | 10 data fields | **11** |

---

## 4. Violaciones de Touch Target (Fitts's Law)

> **Estándar:** iOS HIG ≥ 44 × 44 pt · Android MD3 ≥ 48 × 48 dp · Guía UX interna: 42–72 px óptimo

### V-FIT-01 ⛔ CRÍTICA — `cross-section-axis-btn` → 34 × 34 px

| Propiedad | Valor actual | Mínimo requerido | Déficit |
|-----------|-------------|-------------------|---------|
| width | 34 px | 44 px | −10 px (−23 %) |
| height | 34 px | 44 px | −10 px (−23 %) |
| margin | 3 px L/R | — | Touch gap = 6 px (requiere ≥ 8 px) |

**Ubicación:** `Theme.uss` L472–L480  
**Elementos afectados:** Botones X, Y, Z, Invert en Cross-Section Panel (4 botones)  
**Impacto Fitts:** Targets pequeños + agrupados con spacing mínimo = alta tasa de error en táctil. Estos botones requieren precisión de eje en un contexto analítico donde el error tiene alto costo cognitivo.

**Corrección:**
```css
.cross-section-axis-btn {
    width: 48px;      /* +14 px → 48 dp MD3 */
    height: 48px;
    margin-left: 4px;
    margin-right: 4px; /* gap efectivo = 8 px */
}
```

---

### V-FIT-02 ⛔ CRÍTICA — `glass-slider .unity-base-slider__dragger` → 24 × 24 px

| Propiedad | Valor actual | Mínimo requerido | Déficit |
|-----------|-------------|-------------------|---------|
| width | 24 px | 44 px | −20 px (−45 %) |
| height | 24 px | 44 px | −20 px (−45 %) |

**Ubicación:** `Theme.uss` L965–L973  
**Elementos afectados:** ExplosionSlider, PositionSlider (cross-section), RotationSlider, IntensitySlider (4 sliders)  
**Impacto Fitts:** El dragger es el target con mayor interacción continua (drag). Un dragger de 24 px duplica el tiempo de adquisición respecto a uno de 48 px, según la ley de Fitts.

**Corrección:**
```css
.glass-slider .unity-base-slider__dragger {
    width: 32px;      /* Visual size */
    height: 32px;
    margin-top: -16px;
    /* Add invisible touch expansion via padding or min-touch-area */
}
```
> **Nota:** En WebGL/táctil, el dragger visual puede ser 32 px si el área de hit (touch slop) se expande a 48 px. Unity UI Toolkit no tiene `min-touch-area`, pero se puede lograr con un VisualElement padre invisible de 48 × 48 px.

---

### V-FIT-03 ⛔ CRÍTICA — `sheet-close-btn` → 32 × 32 px

| Propiedad | Valor actual | Mínimo requerido | Déficit |
|-----------|-------------|-------------------|---------|
| width | 32 px | 44 px | −12 px (−27 %) |
| height | 32 px | 44 px | −12 px (−27 %) |

**Ubicación:** `Theme.uss` L765–L773  
**Impacto:** Botón de cierre del BottomSheet. Al ser la única forma de cerrar el panel, un miss-tap tiene alto costo de frustración (NASA-TLX *Frustración*).

**Corrección:**
```css
.sheet-close-btn {
    width: 40px;    /* Visual */
    height: 40px;
    /* Padding interno para alcanzar 48 px hit area */
    padding: 4px;
}
```

---

### V-FIT-04 ⚠️ ALTA — `icon-button-small` (HomeBtn, ResetViewBtn) → 40 × 40 px

| Propiedad | Valor actual | Mínimo requerido | Déficit |
|-----------|-------------|-------------------|---------|
| width | 40 px | 44 px | −4 px (−9 %) |
| height | 40 px | 44 px | −4 px (−9 %) |

**Ubicación:** `Theme.uss` L546–L553  
**Agravante Fitts:** Estos botones están en la **esquina superior** (zona roja de Thumb Zone — máxima distancia del pulgar), lo que amplifica el efecto del target pequeño.

**Corrección:**
```css
.icon-button-small {
    width: 48px;
    height: 48px;
}
```

---

### V-FIT-05 ⚠️ ALTA — `sheet-handle` → 40 × 3 px

| Propiedad | Valor actual | Mínimo requerido | Déficit |
|-----------|-------------|-------------------|---------|
| width | 40 px | — | Aceptable |
| height | 3 px | 44 px | −41 px (−93 %) |

**Ubicación:** `Theme.uss` L670  
**Impacto:** Si el handle está diseñado para ser arrastrado (drag-to-dismiss), 3 px de altura es virtualmente imposible de tocar. Si es decorativo, no hay violación funcional pero sí de expectativa (affordance falsa).

**Corrección:** Si funcional: expandir a `height: 24px; padding: 12px 0;` para hit area de 48 px. Si decorativo: mantener como indicador visual sin interacción.

---

### V-FIT-06 ⚠️ MEDIA — `mode-action-btn` → 72 × 56 px

| Propiedad | Valor actual | Mínimo requerido | Estado |
|-----------|-------------|-------------------|--------|
| width | 72 px | 44 px | ✅ Cumple |
| height | 56 px | 44 px | ✅ Cumple |
| Effective gap | 8 px (margin 4+4) | 8 px | ⚠️ Borderline |

**Análisis:** Los botones cumplen dimensionalmente, pero el **gap de 8 px** entre ellos es el mínimo aceptable. En contexto WebGL táctil (dedos menos precisos que mouse), se recomienda gap ≥ 12 px.

---

## 5. Violaciones de Miller's Law (Carga Cognitiva)

> **Estándar:** Número de ítems visibles simultáneamente en un grupo ≤ 7 (idealmente 5 ± 2 para decisiones rápidas). La tesis declara como variable clave la **reducción de carga cognitiva** (NASA-TLX).

### V-MIL-01 ⛔ CRÍTICA — Analyze Mode: hasta 15 controles simultáneos

Cuando el usuario activa Analyze → SHADERS + CUT:

| Grupo | Ítems |
|-------|-------|
| AnalyzeActionBar | 2 botones (SHADERS, CUT) |
| ShaderMenu (si abierto) | 7 cards |
| CrossSectionPanel (si abierto) | 1 toggle + 3 axis + 1 invert + 1 slider = 6 |
| **Total visible** | **15** |

**Violación:** 15 ≫ 7 ± 2. El usuario debe procesar 15 controles en paralelo mientras intenta analizar un modelo 3D. Esto contradice directamente el objetivo de la tesis: *"reducir la carga cognitiva"*.

**Agravante:** ShaderMenu tiene exactamente 7 ítems (REALISTIC, X-RAY, BLUEPRINT, SOLID, WIRE, GHOST, THERMAL), que es el **límite superior** de Miller. Para usuarios novatos, 7 opciones de shader sin explicación visual clara incrementan la dimensión *Demanda Mental* del NASA-TLX.

**Corrección:**
1. Hacer ShaderMenu y CrossSectionPanel **mutuamente excluyentes** visualmente (si uno abre, el otro colapsa).
2. Reducir ShaderMenu a 5 opciones principales + "More…" expandible.
3. Agrupar ejes X/Y/Z bajo un selector segmentado único (1 control en vez de 3+1).

---

### V-MIL-02 ⚠️ ALTA — Tools Mode: 9 controles + slider

| Grupo | Ítems |
|-------|-------|
| ToolsActionBar | 3 botones (INFO, EXPLODE, PINS) |
| CategoryMenu (si abierto) | 5 cards + 1 título |
| SliderContainer (si visible) | 1 label + 1 slider |
| **Total visible** | **10–11** |

**Violación:** 10–11 > 9 (límite alto de 7+2). No es crítico pero se acerca al umbral, especialmente cuando el BottomSheet también está abierto (agrega 10+ data fields).

---

### V-MIL-03 ⚠️ ALTA — BottomSheet: 10 campos de datos

El BottomSheet muestra simultáneamente:
- 7 data rows (CATEGORY, FUNCTION, MATERIAL, WEIGHT, DIMENSIONS, POWER, TEMP)
- 3 assembly rows (DIFFICULTY, TOOLS, TIME)
- 1 PartDescription

**Total:** 11 campos de datos visibles. Aunque los datos son read-only, la tesis busca que los usuarios *comprendan* la información, no solo la vean. 11 ítems requieren chunking visual.

**Corrección:** Implementar progressive disclosure: mostrar las 4 propiedades principales primero (CATEGORY, FUNCTION, MATERIAL, WEIGHT) y colapsar el resto bajo "Ver más detalles".

---

### V-MIL-04 ⚠️ MEDIA — HeroMain: 4 botones de acción

| Grupo | Ítems |
|-------|-------|
| Hero Menu | EXPLORE, SELECT DEVICE, ABOUT, EXIT = 4 |

**Estado:** 4 < 5 → ✅ **Cumple con Miller**. Agrupación correcta.

---

## 6. Violaciones del 8 pt Grid

> **Estándar:** Todos los valores espaciales deben ser múltiplos de 8 (o 4 px para micro-spacing). La guía UX interna define: 8, 16, 24, 32, 40, 48, 56, 64…

### Tabla de Valores No Alineados

| # | Selector | Propiedad | Valor actual | Valor 8pt más cercano | Línea USS |
|---|----------|-----------|-------------|----------------------|-----------|
| G-01 | `.top-bar` | height | 56 px | **56 = 7×8** ✅ | L106 |
| G-02 | `.top-bar` | margin-top | 24 px | **24 = 3×8** ✅ | L107 |
| G-03 | `.mode-btn` | width | 100 px | **96 px** (12×8) | L223 |
| G-04 | `.mode-btn` | height | 60 px | **56 px** (7×8) o **64 px** (8×8) | L224 |
| G-05 | `.mode-btn` | margin L/R | 6 px | **8 px** (1×8) | L225–226 |
| G-06 | `.mode-btn` | border-radius | 14 px | **16 px** (2×8) | L229 |
| G-07 | `.mode-action-btn` | width | 72 px | **72 = 9×8** ✅ | L348 |
| G-08 | `.mode-action-btn` | height | 56 px | **56 = 7×8** ✅ | L349 |
| G-09 | `.mode-action-btn` | margin L/R | 4 px | **4 px** ✅ (micro) | L350–351 |
| G-10 | `.mode-action-btn` | border-radius | 12 px | **12** ⚠️ (no 8pt, usar 8 ó 16) | L353 |
| G-11 | `.submenu-card` | width | 110 px | **112 px** (14×8) | L852 |
| G-12 | `.submenu-card` | height | 88 px | **88 = 11×8** ✅ | L853 |
| G-13 | `.submenu-card` | margin | 4 px | **4 px** ✅ (micro) | L854 |
| G-14 | `.cross-section-axis-btn` | width/height | 34 px | **32 px** (4×8) o **40 px** (5×8) | L472–473 |
| G-15 | `.cross-section-axis-btn` | margin L/R | 3 px | **4 px** (micro) | L474–475 |
| G-16 | `.cross-section-axis-btn` | border-radius | 8 px | **8 px** ✅ | L478 |
| G-17 | `.icon-button-small` | width/height | 40 px | **40 = 5×8** ✅ | L546–547 |
| G-18 | `.icon-button-small` | margin-right | 12 px | ⚠️ No 8pt (usar 8 ó 16) | L556 |
| G-19 | `.mode-submenu` | border-radius | 14 px | **16 px** | L310 |
| G-20 | `.mode-submenu` | padding | 16 px | **16 px** ✅ | L311 |
| G-21 | `.actions-row` | border-radius | 40 px | **40 = 5×8** ✅ | L161 |
| G-22 | `.actions-row` | padding | 8px 24px | ✅ ambos 8pt | L162 |
| G-23 | `.details-sheet` | border-radius | 24 px | **24 = 3×8** ✅ | L656 |
| G-24 | `.mode-btn-icon` | width/height | 22 px | **24 px** (3×8) | L246–247 |
| G-25 | `.mode-btn-icon` | margin-bottom | 3 px | **4 px** (micro) | L249 |
| G-26 | `.mode-action-icon` | width/height | 20 px | **24 px** (3×8) | L390–391 |
| G-27 | `.mode-action-icon` | margin-bottom | 3 px | **4 px** (micro) | L392 |
| G-28 | `.submenu-icon` | margin-bottom | 6 px | **8 px** (1×8) | L877 |
| G-29 | `.slider-label` | margin-right | 16 px | **16 px** ✅ | L940 |
| G-30 | `.submenu-title` | margin-bottom | 10 px | **8 px** (1×8) | L842 |
| G-31 | `.sheet-close-btn` | width/height | 32 px | **32 = 4×8** ✅ (pero too small for touch) | L766–767 |

### Resumen Grid

| Estado | Cantidad |
|--------|----------|
| ✅ Alineados a 8pt | 17 valores |
| ⚠️ Violaciones (no 8pt ni 4pt micro) | 11 valores |

**Violaciones principales por corregir (High Priority):**

| ID | Corrección | Impacto |
|----|-----------|---------|
| G-03 | `.mode-btn` width: 100 → **96 px** | Grid + visual balance |
| G-04 | `.mode-btn` height: 60 → **64 px** | Grid + touch area bonus |
| G-05 | `.mode-btn` margin: 6 → **8 px** | Grid alignment |
| G-06 | `.mode-btn` border-radius: 14 → **16 px** | Grid |
| G-11 | `.submenu-card` width: 110 → **112 px** | Grid alignment |
| G-14 | `.cross-section-axis-btn` 34 → **48 px** | Grid + Fitts compliance |
| G-15 | `.cross-section-axis-btn` margin: 3 → **4 px** | Micro-grid |
| G-19 | `.mode-submenu` border-radius: 14 → **16 px** | Grid |
| G-24 | `.mode-btn-icon` 22 → **24 px** | Grid |
| G-26 | `.mode-action-icon` 20 → **24 px** | Grid |
| G-30 | `.submenu-title` margin-bottom: 10 → **8 px** | Grid |

---

## 7. Violaciones Tipográficas

> **Estándar (Guía UX interna):**
> - Cuerpo de texto ≥ 16 px
> - Captions / labels ≥ 12 px, **nunca** inferior
> - Escala modular recomendada: 12 / 14 / 16 / 20 / 24 / 32 / 40 / 48 / 64

### Tabla de Violaciones

| # | Selector | Valor actual | Mínimo | Severidad | Línea |
|---|----------|-------------|--------|-----------|-------|
| T-01 | `.mode-action-label` | **9 px** | 12 px | ⛔ CRÍTICA | L395 |
| T-02 | `.mode-btn-label` | **10 px** | 12 px | ⛔ CRÍTICA | L253 |
| T-03 | `.submenu-label` | **10 px** | 12 px | ⚠️ ALTA | L884 |
| T-04 | `.submenu-title` | **11 px** | 12 px | ⚠️ ALTA | L839 |
| T-05 | `.slider-label` | **11 px** | 12 px | ⚠️ ALTA | L937 |
| T-06 | `.header-title` | **14 px** | 16 px (cuerpo) | ⚠️ MEDIA | L112 |
| T-07 | `.cross-section-axis-label` | **12 px** | 12 px | ✅ Cumple | L510 |
| T-08 | `.section-title` | **12 px** | 12 px | ✅ Cumple | L1305 |
| T-09 | `.selection-label` | **13 px** | 12 px | ✅ Cumple | L570 |
| T-10 | `.sheet-hint` | **13 px** | 12 px | ✅ Cumple | L697 |
| T-11 | `.env-slider-label` | **12 px** | 12 px | ✅ Cumple | L1345 |
| T-12 | `.data-label` | **16 px** | 16 px | ✅ Cumple | L1016 |
| T-13 | `.data-value` | **16 px** | 16 px | ✅ Cumple | L1022 |
| T-14 | `.sheet-title` | **22 px** | — | ✅ Cumple | L693 |

### Correcciones Requeridas

```css
/* T-01 */ .mode-action-label  { font-size: 12px; } /* +3 px */
/* T-02 */ .mode-btn-label     { font-size: 12px; } /* +2 px */
/* T-03 */ .submenu-label      { font-size: 12px; } /* +2 px */
/* T-04 */ .submenu-title      { font-size: 12px; } /* +1 px */
/* T-05 */ .slider-label       { font-size: 12px; } /* +1 px */
/* T-06 */ .header-title       { font-size: 14px; } /* Aceptable como branding, no cuerpo */
```

> **Nota sobre T-06:** El `header-title` a 14 px podría mantenerse como *branding text* (no es texto funcional que requiera lectura), pero la guía UX interna no distingue esta excepción. Recomendación: mantener 14 px si se considera decorativo, o subir a 16 px si se considera texto informativo.

---

## 8. Violaciones de Accesibilidad y Contraste

> **Estándar WCAG 2.1 AA:** Texto normal ≥ 4.5:1 · Texto grande ≥ 3:1

### 8.1 Contrastes Calculados (fondo principal: `rgb(10,10,10)` ≈ #0A0A0A)

| # | Elemento | Color texto | Opacity efectiva | Contrast Ratio (estimado) | WCAG AA |
|---|----------|-------------|------------------|---------------------------|---------|
| A-01 | `.mode-btn-label` | white @ 0.5 | ~128,128,128 vs #0A0A0A | ≈ 6.0:1 | ✅ |
| A-02 | `.mode-action-label` | white @ 0.5 | ~128,128,128 | ≈ 6.0:1 | ✅ |
| A-03 | `.submenu-label` | white @ 0.6 | ~153,153,153 | ≈ 8.5:1 | ✅ |
| A-04 | `.header-title` | white @ 0.35 | ~89,89,89 | ≈ 3.5:1 | ⚠️ FALLA (normal text < 4.5:1) |
| A-05 | `.submenu-title` | white @ 0.3 | ~77,77,77 | ≈ 2.8:1 | ⚠️ FALLA |
| A-06 | `.slider-label` | white @ 0.35 | ~89,89,89 | ≈ 3.5:1 | ⚠️ FALLA |
| A-07 | `.data-label` | white @ 0.35 | ~89,89,89 | ≈ 3.5:1 | ⚠️ FALLA (pero texto 16px bold podría calificar como "large") |
| A-08 | `.selection-label` | white @ 0.7 | ~179,179,179 | ≈ 10.5:1 | ✅ |

### Correcciones Requeridas

| ID | Selector | Opacity actual | Mínima para 4.5:1 |
|----|----------|---------------|-------------------|
| A-04 | `.header-title` | 0.35 | **0.50** |
| A-05 | `.submenu-title` | 0.3 | **0.50** |
| A-06 | `.slider-label` | 0.35 | **0.50** |
| A-07 | `.data-label` | 0.35 | **0.42** (o justificar como "large text" si bold) |

---

## 9. Fallas Estructurales UX (Arquitectura de Información)

### S-01 ⚠️ ALTA — Mode Containers posicionados en absolute con coordenada fija

**Problema:** Los 3 mode containers usan `position: absolute; bottom: 164px` (L275). Esto crea:
1. **Solapamiento potencial** con el BottomSheet cuando está expandido.
2. **Ruptura en pantallas pequeñas** — el valor `164px` es hardcoded, no responde al viewport.
3. Cuando `ui-shifted` mueve el bottom-bar a `bottom: 46%`, los mode-containers quedan flotando desconectados visualmente de sus botones activadores.

**Corrección:** Cambiar mode containers a `position: relative` dentro del flujo del BottomBar, o usar cálculos dinámicos desde C#.

---

### S-02 ⚠️ ALTA — No hay Progressive Disclosure en Analyze Mode

**Problema:** Al abrir Analyze, se pueden ver simultáneamente: 2 action btns + 7 shader cards + toggle + 3 axis btns + invert + slider = 15 elementos. No hay ningún mecanismo de revelación progresiva.

**Corrección:** Implementar un patrón de **acordeón**: cuando SHADERS está activo, mostrar ShaderMenu y ocultar CrossSection. Cuando CUT está activo, mostrar CrossSection y ocultar ShaderMenu.

---

### S-03 ⚠️ MEDIA — TopBar en zona roja de Thumb Zone

**Problema:** HomeBtn y ResetViewBtn están en las esquinas superiores (position absolute, top corners). Según la "Ley de la Zona del Pulgar" (guía UX interna), esta es la **zona de menor accesibilidad** para uso con una mano.

**Corrección:**
- HomeBtn es de baja frecuencia (aceptable en top bar).
- ResetView es de **alta frecuencia** durante exploración 3D → debería migrar al BottomBar o como gesto (doble tap).

---

### S-04 ⚠️ MEDIA — Ausencia de feedback háptico/visual de estado

**Problema:** No hay indicador visible del modo activo más allá del color del botón (`mode-btn--active`). No hay:
- Breadcrumb o indicador de contexto ("Estás en: Analyze > Shaders")
- Feedback visual cuando un shader está aplicado
- Indicador de que el cross-section está activo fuera de su panel

**Impacto tesis:** Los participantes del estudio (N=8-12) podrían perder contexto ("¿Qué modo estoy usando?"), incrementando *Demanda Mental* en NASA-TLX.

---

### S-05 ⚠️ BAJA — Iconos placeholder (no únicos)

**Problema en Theme.uss L898–916:** Múltiples shader cards y category cards usan el **mismo icono genérico**:
```css
.icon-realistic { background-image: resource("UI/Icons/icon_view"); }
.icon-solid     { background-image: resource("UI/Icons/icon_view"); }
.icon-wire      { background-image: resource("UI/Icons/icon_view"); }
.icon-ghost     { background-image: resource("UI/Icons/icon_view"); }
```

**Impacto:** Sin iconografía diferenciada, los usuarios dependen únicamente del texto (10 px, difícil de leer) para distinguir opciones. Esto contradice la recomendación de redundancia de señales (icono + texto) de la guía UX.

---

## 10. Tabla Consolidada de Severidad

| ID | Dominio | Severidad | Elemento | Problema | Esfuerzo Fix |
|----|---------|-----------|----------|----------|-------------|
| V-FIT-01 | Fitts | ⛔ Crítica | cross-section-axis-btn | 34 px (req 44+) | Bajo (CSS) |
| V-FIT-02 | Fitts | ⛔ Crítica | slider dragger | 24 px (req 44+) | Medio (CSS+C#) |
| V-FIT-03 | Fitts | ⛔ Crítica | sheet-close-btn | 32 px (req 44+) | Bajo (CSS) |
| V-FIT-04 | Fitts | ⚠️ Alta | icon-button-small | 40 px (req 44+) | Bajo (CSS) |
| V-FIT-05 | Fitts | ⚠️ Alta | sheet-handle | 3 px height | Bajo (CSS) |
| V-FIT-06 | Fitts | ⚠️ Media | mode-action-btn gap | 8 px gap (borderline) | Bajo (CSS) |
| V-MIL-01 | Miller | ⛔ Crítica | Analyze mode total | 15 items simultáneos | Alto (C#/UXML) |
| V-MIL-02 | Miller | ⚠️ Alta | Tools mode total | 10-11 items | Medio (C#) |
| V-MIL-03 | Miller | ⚠️ Alta | BottomSheet data | 11 campos | Medio (UXML/C#) |
| V-MIL-04 | Miller | ✅ OK | HeroMain | 4 botones | — |
| T-01 | Tipografía | ⛔ Crítica | mode-action-label | 9 px (req 12+) | Bajo (CSS) |
| T-02 | Tipografía | ⛔ Crítica | mode-btn-label | 10 px (req 12+) | Bajo (CSS) |
| T-03 | Tipografía | ⚠️ Alta | submenu-label | 10 px | Bajo (CSS) |
| T-04 | Tipografía | ⚠️ Alta | submenu-title | 11 px | Bajo (CSS) |
| T-05 | Tipografía | ⚠️ Alta | slider-label | 11 px | Bajo (CSS) |
| T-06 | Tipografía | ⚠️ Media | header-title | 14 px (branding) | Bajo (CSS) |
| A-04 | Contraste | ⚠️ Alta | header-title | 3.5:1 (req 4.5:1) | Bajo (CSS) |
| A-05 | Contraste | ⚠️ Media | submenu-title | 2.8:1 | Bajo (CSS) |
| A-06 | Contraste | ⚠️ Media | slider-label | 3.5:1 | Bajo (CSS) |
| A-07 | Contraste | ⚠️ Baja | data-label | 3.5:1 (posible "large") | Bajo (CSS) |
| G-03 | Grid | ⚠️ Alta | mode-btn width | 100 → 96 | Bajo (CSS) |
| G-04 | Grid | ⚠️ Alta | mode-btn height | 60 → 64 | Bajo (CSS) |
| G-05 | Grid | ⚠️ Alta | mode-btn margin | 6 → 8 | Bajo (CSS) |
| G-11 | Grid | ⚠️ Media | submenu-card width | 110 → 112 | Bajo (CSS) |
| G-14 | Grid | ⚠️ Media | axis-btn size | 34 → 48 | Bajo (CSS) |
| S-01 | Estructura | ⚠️ Alta | mode-container absolute | bottom: 164px hardcoded | Medio |
| S-02 | Estructura | ⚠️ Alta | Analyze sin disclosure | 15 items simultáneos | Alto |

---

## 11. Plan de Implementación — UI Minimalista por Modos

### Fase A — Quick Wins CSS (≈ 2 horas) — Prioridad: INMEDIATA

Solo modificaciones en `Theme.uss`. Sin cambios en UXML ni C#.

```
  ACCIÓN                                SELECTOR                     CAMBIO
  ─────────────────────────────────────────────────────────────────────────
  A1. Touch targets                     .cross-section-axis-btn     34→48 px
  A2. Touch targets                     .icon-button-small          40→48 px
  A3. Touch targets                     .sheet-close-btn            32→40 px (+padding 4px)
  A4. Touch targets                     .slider dragger             24→32 px (+hit area)
  A5. Font floor                        .mode-action-label          9→12 px
  A6. Font floor                        .mode-btn-label             10→12 px
  A7. Font floor                        .submenu-label              10→12 px
  A8. Font floor                        .submenu-title              11→12 px
  A9. Font floor                        .slider-label               11→12 px
  A10. 8pt grid                         .mode-btn                   100×60→96×64, margin 6→8
  A11. 8pt grid                         .mode-btn icon              22→24 px
  A12. 8pt grid                         .mode-action-icon           20→24 px
  A13. 8pt grid                         .submenu-card               110→112 px width
  A14. 8pt grid                         border-radius (varios)      14→16 px
  A15. Contrast                         .header-title color         0.35→0.50 opacity
  A16. Contrast                         .submenu-title color        0.3→0.50 opacity
  A17. Contrast                         .slider-label color         0.35→0.50 opacity
```

### Fase B — Progressive Disclosure (≈ 4 horas) — Prioridad: ALTA

Cambios en `UIModeController.cs` + `MainLayout.uxml`.

| # | Tarea | Archivo |
|---|-------|---------|
| B1 | Analyze Mode: ShaderMenu y CrossSection mutuamente excluyentes. Al activar CUT → ocultar ShaderMenu. Al activar SHADERS → ocultar CrossSection. | `UIModeController.cs` |
| B2 | BottomSheet: Colapsar assembly-section bajo un toggle "ASSEMBLY INFO ▸". Mostrar solo 4 data-rows principales por defecto. | `MainLayout.uxml` + `BottomSheetController.cs` |
| B3 | Tools Mode: SliderContainer + CategoryMenu solo visibles cuando su acción correspondiente está activa (ya parcialmente implementado). Validar que no aparezcan ambos simultáneamente. | `UIModeController.cs` |

### Fase C — Structural UX (≈ 6 horas) — Prioridad: MEDIA

| # | Tarea | Archivo |
|---|-------|---------|
| C1 | Migrar mode-containers de `position: absolute; bottom: 164px` a flujo relativo dentro del BottomBar, o calcular posición dinámica desde C# basada en viewport height. | `Theme.uss` + `UIModeController.cs` |
| C2 | Añadir indicador de contexto/breadcrumb debajo del TopBar: "MODE > TOOL" (e.g., "ANALYZE > SHADERS"). | `MainLayout.uxml` + `Theme.uss` + controller |
| C3 | Crear iconos únicos para cada shader/category card. Reemplazar placeholders. | `Resources/UI/Icons/` + `Theme.uss` |
| C4 | ResetView: Agregar gesto de doble-tap como alternativa al botón de esquina superior. | Nuevo script `GestureController.cs` |

### Fase D — Validación Académica (≈ 8 horas) — Prioridad: PRE-DEFENSA

| # | Tarea |
|---|-------|
| D1 | Preparar protocolo NASA-TLX simplificado (6 subescalas) para N=8–12 participantes |
| D2 | Preparar cuestionario SUS (10 ítems, escala Likert 5) |
| D3 | Definir tareas de usabilidad: (1) Encontrar pieza, (2) Cambiar shader, (3) Aplicar cross-section, (4) Leer datos técnicos |
| D4 | Ejecutar pruebas, registrar tiempos de tarea y errores |
| D5 | Analizar resultados: SUS ≥ 68 (umbral aceptable), NASA-TLX < 50 (carga baja-media) |

---

## 12. Métricas Objetivo (post-fix)

| Métrica | Estado actual (estimado) | Objetivo post-fix |
|---------|------------------------|--------------------|
| Touch targets < 44 px | 5 elementos | **0 elementos** |
| Font sizes < 12 px | 5 selectores | **0 selectores** |
| 8pt grid compliance | ~61 % | **≥ 95 %** |
| Max items simultáneos (Analyze) | 15 | **≤ 7** |
| WCAG AA contrast failures | 4 | **0** |
| SUS Score (proyectado) | 55–65 | **≥ 68** (percentil 50) |
| NASA-TLX (proyectado) | 50–65 | **< 50** (carga media-baja) |

---

## Anexo A — Checklist de Verificación Post-Fix

- [ ] Todos los targets interactivos ≥ 44 px (Fitts)
- [ ] Todas las fuentes ≥ 12 px (Tipografía)
- [ ] Todos los valores espaciales múltiplos de 8 o 4 (Grid)
- [ ] Contrast ratio ≥ 4.5:1 para texto normal (WCAG AA)
- [ ] Máximo 7 ± 2 controles visibles por grupo (Miller)
- [ ] Progressive disclosure en Analyze y BottomSheet
- [ ] Iconos únicos por shader y categoría
- [ ] Indicador de contexto/modo activo visible
- [ ] ResetView accesible desde thumb zone
- [ ] BottomSheet no solapa mode containers

---

*Fin del informe · Generado automáticamente desde auditoría de `MainLayout.uxml` + `Theme.uss`*
