# Design Tokens — WebGL Drone Viewer

Sistema de diseño unificado. Fuente de verdad para todos los estilos de la aplicación Unity.

> Archivo USS de referencia: `Assets/UI/Styles/Theme.uss`

---

## Colores

| Token | Valor | Uso |
|---|---|---|
| **Background** | `rgb(5, 5, 5)` `#050505` | Canvas, Hero, fondo general |
| **Surface** | `rgba(18, 18, 18, 0.92)` | Paneles, sheets, cards |
| **Primary/Accent** | `rgb(255, 255, 255)` | Estados activos, slider, CTA |
| **On-Surface** | `rgb(255, 255, 255)` | Texto principal |
| **On-Surface-Dim** | `rgba(255, 255, 255, 0.4)` | Texto secundario/muted |
| **Border** | `rgba(255, 255, 255, 0.1)` | Bordes por defecto |
| **Border-Hover** | `rgba(255, 255, 255, 0.2)` | Bordes en hover |
| **Selection BG** | `rgba(255, 255, 255, 0.15)` | Fondo de ítems seleccionados |

> **Decisión**: El acento es **blanco puro**, alineado con la landing page web (`--accent: #ffffff`). Sin cyan ni verde.

---

## Tipografía

| Rol | Fuente | Peso | Tamaño | Case |
|---|---|---|---|---|
| **Títulos (H1)** | Space Grotesk | Regular | 32px | UPPERCASE |
| **Subtítulos** | Inter | Regular | 18px | Normal |
| **Cuerpo** | Inter | Regular | 16px | Normal |
| **Etiquetas** | Inter | Regular | 14px | UPPERCASE |
| **Caption** | Inter | Regular | 12px | Normal |
| **Botones** | Space Grotesk | Regular | 16px | UPPERCASE |

- `letter-spacing: 4px` en botones principales
- `letter-spacing: 1px` en etiquetas de categoría

---

## Espaciado (Grid de 8px)

| Token | Valor | Uso |
|---|---|---|
| `spacing-xs` | 4px | Gaps mínimos |
| `spacing-sm` | 8px | Padding interno, gaps |
| `spacing-md` | 16px | Margins entre elementos |
| `spacing-lg` | 24px | Padding de paneles |
| `spacing-xl` | 32px | Separación entre secciones |
| `spacing-2xl` | 48px | Hero spacing |

---

## Dimensiones

### Botones

| Componente | Ancho | Alto | Min Touch |
|---|---|---|---|
| **Hero Button (wide)** | 380px | 56px | ≥48dp ✅ |
| **Category Tag** | auto (min 100px) | 48px | ≥48dp ✅ |
| **Icon Button** | 64px | 64px | ≥48dp ✅ |
| **Icon Button Small** | 40px | 40px | Toolbar only |
| **Submenu Card** (futuro) | 100px | 100px | ≥48dp ✅ |

### Paneles

| Componente | Ancho | Alto |
|---|---|---|
| **Device Selector** | 400px | auto |
| **Details Sheet** | 100% | max 55% |
| **Slider Container** | 496px | auto |

---

## Border Radius

| Token | Valor | Uso |
|---|---|---|
| **Pill** | `height / 2` | Hero buttons (28px), device options (32px) |
| **Rounded Rect** | 12-16px | Paneles, cards de submenú |
| **Small** | 8px | Tags, inputs |
| **Circle** | 50% | Icon buttons, slider dragger |
| **Large Pill** | 60px | Shader/env buttons |

---

## Estados

| Estado | Background | Border | Color |
|---|---|---|---|
| **Default** | `transparent` / `rgba(255,255,255,0.05)` | `rgba(255,255,255,0.08)` | `white` |
| **Hover** | `rgba(255,255,255,0.08)` | `rgba(255,255,255,0.2)` | `white` |
| **Active/Selected** | `rgb(255,255,255)` | `rgb(255,255,255)` | `rgb(10,10,10)` (negro) |
| **Disabled** | `rgba(255,255,255,0.02)` | `rgba(255,255,255,0.04)` | `rgba(255,255,255,0.2)` |

---

## Movimiento

| Propiedad | Duración | Easing |
|---|---|---|
| **Hover** | 0.2s | ease-out |
| **State change** | 0.25s | ease-out |
| **Panel show/hide** | 0.3s | ease-out |
| **Slider dragger** | 0.15s | ease |

---

*Última actualización: 2026-02-18*
