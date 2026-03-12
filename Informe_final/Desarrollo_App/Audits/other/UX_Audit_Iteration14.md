# UX/UI Audit — Iteration 14 (Rev. 2)

**Fecha:** 26 de febrero de 2026  
**Base:** Guía Completa de Diseño UI/UX para Aplicaciones Móviles (Apple HIG + Material Design 3 + WCAG 2.1)  
**Archivos auditados:** `Theme.uss` (~1534 líneas), `MainLayout.uxml` (~353 líneas)  
**Revisión:** Re-audit completo tras rediseño del bottom pill (112px, iconos 56px)

---

## 1. Áreas de Toque (Touch Targets)

**Estándar:** Mínimo 44×44pt (iOS) / 48×48dp (Android). Rango óptimo 42-72px.

| Elemento                  | Tamaño     | Mínimo | Estado                |
| :------------------------ | :--------- | :----- | :-------------------- |
| `.actions-row` (pill)     | 112px alto | —      | ✅ Contenedor         |
| `.mode-btn`               | 120×96     | 48×48  | ✅                    |
| `.mode-btn-icon`          | 56×56      | —      | ✅ (match card icons) |
| `.icon-button`            | 64×64      | 44×44  | ✅                    |
| `.icon-button-small`      | 48×48      | 44×44  | ✅                    |
| `.mode-action-btn`        | 72×64      | 48×48  | ✅                    |
| `.submenu-card`           | 148×148    | 48×48  | ✅                    |
| `.btn-wide`               | 380×56     | 44×44  | ✅                    |
| `.cross-section-axis-btn` | 104×56     | 48×48  | ✅                    |
| `.device-option`          | 100%×64    | 48×48  | ✅                    |
| `.btn-icon`               | 48×48      | 44×44  | ✅                    |
| `.btn-back`               | 44×44      | 44×44  | ✅                    |
| `.sheet-close-btn`        | 44×44      | 44×44  | ✅                    |
| `.glass-slider .dragger`  | 44×44      | 44×44  | ✅                    |

**Espaciado entre botones interactivos:**

- `.mode-btn` margin 20px cada lado → 40px entre ellos ✅ (>8px mín.)
- `.icon-button` margin 8px cada lado → 16px ✅
- `.mode-action-btn` margin 12px cada lado → 24px ✅

---

## 2. Tipografía

**Estándar:** Cuerpo mínimo 16px. Captions 12px absoluto, 16px recomendado.

| Elemento                      | Tamaño | Mín. (16px) | Estado |
| :---------------------------- | :----- | :---------- | :----- |
| `.hero-title`                 | 64px   | —           | ✅     |
| `.hero-subtitle`              | 24px   | 16px        | ✅     |
| `.about-title`                | 28px   | 16px        | ✅     |
| `.panel-title`                | 36px   | 16px        | ✅     |
| `.sheet-title`                | 22px   | 16px        | ✅     |
| `.header-title`               | 20px   | 16px        | ✅     |
| `.submenu-title`              | 20px   | 16px        | ✅     |
| `.about-desc (2nd)`           | 20px   | 16px        | ✅     |
| `.device-option`              | 20px   | 16px        | ✅     |
| `.btn-back`                   | 20px   | 16px        | ✅     |
| `.submenu-label`              | 18px   | 16px        | ✅     |
| `.mode-btn-label`             | 18px   | 16px        | ✅     |
| `.mode-action-label`          | 18px   | 16px        | ✅     |
| `.slider-label`               | 18px   | 16px        | ✅     |
| `.env-slider-label`           | 18px   | 16px        | ✅     |
| `.cross-section-axis-label`   | 18px   | 16px        | ✅     |
| `.data-label` / `.data-value` | 17px   | 16px        | ✅     |
| `.btn`                        | 16px   | 16px        | ✅     |
| `.selection-label`            | 16px   | 16px        | ✅     |
| `.part-description`           | 16px   | 16px        | ✅     |
| `.section-title`              | 16px   | 16px        | ✅     |
| `.sheet-hint`                 | 16px   | 16px        | ✅     |
| `.about-desc (1st)`           | 16px   | 16px        | ✅     |
| `.about-credit`               | 16px   | 16px        | ✅     |
| `.panel-subtitle`             | 16px   | 16px        | ✅     |
| `.sheet-close-btn`            | 16px   | 16px        | ✅     |

---

## 3. Contraste de Color (WCAG 2.1)

**Estándar:** 4.5:1 texto normal (<18pt), 3:1 texto grande (≥18pt). Fondo: `#0a0a0a`.

| Elemento                    | Color  | Size    | Large? | Ratio  | Req   | Estado |
| :-------------------------- | :----- | :------ | :----- | :----- | :---- | :----- |
| `.hero-subtitle`            | α 0.65 | 24px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.header-title`             | α 0.65 | 20px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.mode-btn-label`           | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.mode-action-label`        | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.submenu-label`            | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.submenu-title`            | α 0.65 | 20px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.slider-label`             | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.env-slider-label`         | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.cross-section-axis-label` | α 0.65 | 18px    | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.reset-icon-label`         | α 0.65 | inherit | Sí     | ~8.5:1 | 3:1   | ✅     |
| `.app-logo`                 | α 0.6  | 24px    | Sí     | ~8:1   | 3:1   | ✅     |
| `.data-label`               | α 0.65 | 17px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.data-value`               | α 0.8  | 17px    | No     | ~12:1  | 4.5:1 | ✅     |
| `.sheet-hint`               | α 0.65 | 16px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.section-title`            | α 0.65 | 16px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.part-description`         | α 0.65 | 16px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.selection-label`          | α 0.75 | 16px    | No     | ~11:1  | 4.5:1 | ✅     |
| `.about-desc (1st)`         | α 0.65 | 16px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.about-desc (2nd)`         | α 0.7  | 20px    | Sí     | ~10:1  | 3:1   | ✅     |
| `.about-credit`             | α 0.6  | 16px    | No     | ~8:1   | 4.5:1 | ✅     |
| `.panel-subtitle`           | α 0.65 | 16px    | No     | ~8.5:1 | 4.5:1 | ✅     |
| `.sheet-close-btn`          | α 0.5  | 16px    | No     | ~5.1:1 | 4.5:1 | ✅     |
| `.btn-ghost color`          | α 0.7  | 16px    | No     | ~10:1  | 4.5:1 | ✅     |
| `.device-option`            | α 0.8  | 20px    | Sí     | ~12:1  | 3:1   | ✅     |

**Nota:** `.device-option:disabled` usa α 0.25 (~1.9:1) — aceptable para estados deshabilitados per WCAG.

---

## 4. Sistema de Grilla (8pt primary / 4pt fine)

Todos los valores de spacing verificados contra múltiplos de 4:

| Propiedad                          | Valor    | Múltiplo 4? | Estado |
| :--------------------------------- | :------- | :---------- | :----- |
| `.actions-row` height              | 112px    | ✅ (28×4)   | ✅     |
| `.actions-row` border-radius       | 56px     | ✅ (14×4)   | ✅     |
| `.actions-row` padding             | 0 48px   | ✅          | ✅     |
| `.actions-row` min-width           | 480px    | ✅          | ✅     |
| `.mode-btn`                        | 120×96   | ✅          | ✅     |
| `.mode-btn` margin                 | 20px     | ✅ (5×4)    | ✅     |
| `.mode-btn-icon`                   | 56×56    | ✅ (14×4)   | ✅     |
| `.mode-btn-icon` mb                | 8px      | ✅          | ✅     |
| `.mode-action-btn`                 | 72×64    | ✅          | ✅     |
| `.mode-action-icon`                | 40×40    | ✅          | ✅     |
| `.submenu-grid` width              | 476px    | ✅ (119×4)  | ✅     |
| `.cross-section-axis-group` width  | 476px    | ✅          | ✅     |
| `.cross-section-slider` width      | 476px    | ✅          | ✅     |
| `.env-slider-group` width          | 476px    | ✅          | ✅     |
| `.selection-label` padding         | 8px 16px | ✅          | ✅     |
| `.submenu-container` border-radius | 16px     | ✅          | ✅     |
| `.glass-slider .dragger`           | 44×44    | ✅          | ✅     |

**Anterior 474px**: corregido a 476px (4pt-aligned).  
**Anterior `.submenu-container` br 14px**: corregido a 16px.

---

## 5. Regla Internal ≤ External

| Componente           | Padding       | Margin/Gap  | Estado       |
| :------------------- | :------------ | :---------- | :----------- |
| `.submenu-card`      | flex-centered | 4px         | ✅           |
| `.details-sheet`     | 32px          | full-width  | ✅           |
| `.hero-submenu`      | 0 48px        | full-screen | ✅           |
| `.glass-panel`       | 24px          | contextual  | ✅           |
| `.submenu-container` | 16px          | contextual  | ✅           |
| `.device-selector`   | 12px          | mt 24px     | ✅ (12 ≤ 24) |

---

## 6. Consistencia de Border-Radius

Consolidado a 5 niveles funcionales:

| Nivel      | Valor | Uso                                                                                                                          |
| :--------- | :---- | :--------------------------------------------------------------------------------------------------------------------------- |
| Circle     | 50%   | `.icon-button`, `.btn-back`, `.sheet-close-btn`, `.btn-icon`, `.submenu-icon`, `.slider-dragger`                             |
| Pill-LG    | 56px  | `.actions-row` (half of 112px)                                                                                               |
| Pill-SM    | 28px  | `.btn`, `.btn-wide`, `.device-option` (half of 56px)                                                                         |
| Surface-LG | 24px  | `.details-sheet` top corners                                                                                                 |
| Surface-MD | 16px  | `.submenu-card`, `.cross-section-slider`, `.slider-container`, `.env-slider-group`, `.selection-label`, `.submenu-container` |
| Surface-SM | 12px  | `.glass-panel`, `.about-panel`, `.details-panel`, `.sidebar`, `.device-selector`, `.cross-section-axis-btn`                  |
| Decorative | 2px   | `.sheet-handle`, `.slider-tracker`                                                                                           |

---

## 7. Especificaciones de Componentes (Guide Compliance)

| Componente        | Guía           | Implementación                                  | Estado      |
| :---------------- | :------------- | :---------------------------------------------- | :---------- |
| Botones altura    | 44-56px        | `.btn-wide` 56px                                | ✅          |
| Bottom Nav altura | 56px (Android) | `.actions-row` 112px (custom pill)              | ✅ Custom   |
| Cards padding     | 16px           | `.submenu-container` 16px                       | ✅          |
| Icons in UI       | min 24×24      | `.mode-btn-icon` 56px, `.mode-action-icon` 40px | ✅          |
| Border-radius     | 8-12px modern  | 12-16px surface                                 | ✅          |
| Nav icons         | 24×24 standard | 56×56 (oversized for WebGL)                     | ✅ Enhanced |

---

## 8. Thumb Zone & Safe Areas

- ✅ Navegación primaria en `.bottom-bar` (zona inferior)
- ✅ Pill de modos en thumb zone central-inferior
- ✅ Home/Reset en zona superior (acciones secundarias — aceptable)
- ✅ Safe margins: top 24px, bottom 16px, lateral 24px
- ✅ Mode actions y submenus sobre el pill (alcance de pulgar)

---

## 9. Design Tokens (Actualizados)

```
Background:     rgb(5, 5, 5)             #050505
Surface:        rgba(18, 18, 18, 0.92)   #121212
Primary:        rgb(255, 255, 255)       White
On-Surface:     rgb(255, 255, 255)       White
On-Surface-Dim: rgba(255, 255, 255, 0.65) WCAG 4.5:1 on #0a0a0a
Border:         rgba(255, 255, 255, 0.1)
Border-Hover:   rgba(255, 255, 255, 0.2)
Radius-Pill:    half of height (56→28, 112→56)
Radius-LG:      24px (bottom sheet)
Radius-MD:      16px (cards, sliders, containers)
Radius-SM:      12px (glass panels, sidebar)
```

---

## Resumen

| Categoría           | Estado         | Notas                                        |
| :------------------ | :------------- | :------------------------------------------- |
| Touch Targets       | ✅ 14/14 pass  | Todos ≥44px. Dragger slider 32→44px          |
| Tipografía          | ✅ 26/26 pass  | Todos ≥16px. `.sheet-close-btn` 14→16px      |
| Contraste WCAG      | ✅ 24/24 pass  | Mínimo α 0.5 (5.1:1). Mayoría α 0.65 (8.5:1) |
| Grilla 4pt/8pt      | ✅ All aligned | 474→476px ×4, br 14→16, dragger 32→44        |
| Internal ≤ External | ✅             | Sin violaciones                              |
| Border-Radius       | ✅ 5 niveles   | Coherente y documentado                      |
| Componentes guía    | ✅             | Botones, cards, nav cumplen                  |
| Thumb Zone          | ✅             | Acciones primarias en zona inferior          |
| **TOTAL**           | **✅ PASS**    | **0 fallos, 0 warnings**                     |
