<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# Guía Completa de Diseño UI/UX para Aplicaciones Móviles

Esta es una investigación profunda sobre las dimensiones, medidas, normativas y estándares profesionales para crear aplicaciones móviles con UX/UI de primer nivel.

## 📏 Normativas Oficiales y Fuentes de Información

### **Fuentes Primarias**

Las dos fuentes oficiales más importantes para diseño móvil son:

1. **Apple Human Interface Guidelines (HIG)**[^1][^2]
    - URL oficial: `developer.apple.com/design`
    - Normas para iOS, iPadOS, watchOS
    - Actualizado constantemente por Apple
2. **Material Design (Google)**[^3][^4]
    - URL oficial: `material.io` y `m3.material.io`
    - Normas para Android y multiplataforma
    - Material Design 3 es la versión actual (2026)

### **Por Qué Seguir Estas Normativas**

- **Consistencia del sistema**: Los usuarios esperan patrones familiares[^5][^6]
- **Accesibilidad**: Cumplen con WCAG (Web Content Accessibility Guidelines)[^2][^7]
- **Cumplimiento de App Store**: Apple rechaza apps que no siguen HIG[^2]
- **Usabilidad comprobada**: Basadas en investigación y pruebas de usuario[^8]

***

## 🎯 Áreas de Toque (Touch Targets)

### **Tamaños Mínimos Oficiales**

| Plataforma | Tamaño Mínimo | Tamaño Recomendado | Razón |
| :-- | :-- | :-- | :-- |
| **iOS (Apple HIG)** | 44×44 pt | 44×44 pt o más | Investigación muestra 25%+ errores en tamaños menores [^2][^9] |
| **Android (Material Design)** | 48×48 dp | 48×48 dp o más | Estándar para todos los dispositivos Android [^8][^10] |
| **Microsoft (Fluent Design)** | 34×34 px + espacio | 48×48 px con padding | Requiere espacio adicional alrededor [^8] |

### **Investigación de Usabilidad**

Estudios muestran que el rango óptimo es **42-72 píxeles**:[^11]

- **42 px**: Tamaño mínimo absoluto (9×9 mm físicos)
- **60 px**: Tamaño preferido por la mayoría de usuarios
- **72 px**: Máxima precisión, preferido por usuarios mayores
- Botones menores a 42px resultan en 25%+ de errores de toque[^11][^2]


### **Espaciado Entre Elementos Interactivos**

```
Espaciado entre botones relacionados: 8-12 px
Espaciado entre grupos de botones: 16-24 px
Espaciado alrededor de acciones primarias: 24 px mínimo
```


***

## 🔤 Tipografía Móvil

### **Tamaños de Fuente Recomendados**

#### **iOS (San Francisco Font)**

| Elemento | Tamaño | Uso |
| :-- | :-- | :-- |
| **Texto de cuerpo** | 17 pt | Lectura principal [^12] |
| **Títulos principales** | 28-34 pt | Encabezados de primer nivel [^12] |
| **Títulos secundarios** | 22-28 pt | Subtítulos [^12] |
| **Texto de botones** | 17-19 pt | Acciones interactivas [^12] |
| **Captions/pequeño** | 12-15 pt (mínimo) | Texto secundario [^12] |

#### **Android (Roboto/Material Design)**

| Elemento | Tamaño | Uso |
| :-- | :-- | :-- |
| **Texto de cuerpo** | 16 sp | Estándar para lectura [^12] |
| **Texto secundario** | 14 sp | Información complementaria [^12] |
| **Captions mínimo** | 12 sp | No ir más pequeño [^12] |
| **Títulos de página** | 28-40 px | Jerarquía visual clara [^13] |

### **Reglas de Tipografía Universal**

```
Tamaño mínimo de cuerpo: 16 px (nunca menor) [cite:32][cite:35]
Títulos: 1.3x - 1.618x más grandes que el cuerpo [cite:32]
Interlineado: 120-150% del tamaño de fuente [cite:32][cite:41]
Contraste mínimo: 4.5:1 para texto normal, 3:1 para texto grande [cite:17][cite:19]
```


### **Accesibilidad Tipográfica**

- Soportar **Dynamic Type** (iOS) y **font scaling** (Android)[^7][^12]
- No usar fuentes decorativas para texto legible[^7]
- WCAG recomienda mínimo 14pt, pero expertos sugieren 16pt[^12]
- Muchos usuarios ajustan el tamaño del sistema a más grande por defecto[^12]

***

## 📐 Sistema de Grilla y Espaciado

### **Sistema de Grilla de 8pt (Estándar de la Industria)**

El sistema más usado en diseño móvil es el **8pt grid system**:[^14][^15][^16]

```
Base: 8 px
Múltiplos: 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 96, 128...
```


#### **Por Qué 8pt?**

- Se divide perfectamente en tamaños de pantalla comunes[^14]
- Funciona bien con diferentes densidades de píxeles[^14]
- Soportado oficialmente por iOS (Human Interface Guidelines)[^15]
- Flexible para espacios pequeños y grandes[^14]


### **Sistema de Grilla de 4pt (Material Design)**

**Material Design usa base de 4dp**:[^17][^15]

```
Base: 4 dp
Múltiplos: 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 48, 56, 64...
```


#### **Cuándo Usar 4pt vs 8pt**

| Sistema | Plataforma | Ventaja |
| :-- | :-- | :-- |
| **4pt** | Android (Material Design) | Control fino, diseños más densos [^15] |
| **8pt** | iOS, Web | Más respiro visual, alineado con HIG [^15] |

### **Categorías de Espaciado (Material Design 3)**

Según la documentación oficial:[^17]

#### **Micro Spacing (4-12 dp)**

```
4 dp: Padding mínimo, iconos
8 dp: Padding interno de componentes, botones
12 dp: Espaciado compacto en listas
```


#### **Standard Spacing (16-24 dp)**

```
16 dp: EL MÁS COMÚN - padding y margins estándar
20 dp: Espaciado medio para contenido relacionado
24 dp: Margins grandes, separación de secciones
```


#### **Macro Spacing (28-64 dp)**

```
28-40 dp: Separación de features principales
48-64 dp: Secciones de layout, margins de áreas de contenido
```


#### **Extended Spacing (72-128 dp)**

```
72-96 dp: Margins para pantallas grandes
128 dp: Espaciado máximo para layouts anchos
```


### **Regla Internal ≤ External**

El espacio **interno** (padding) debe ser **menor o igual** que el espacio **externo** (margin):[^16]

```
Ejemplo correcto:
Padding interno de card: 16 px
Margin entre cards: 24 px ✓

Ejemplo incorrecto:
Padding interno: 32 px
Margin entre cards: 16 px ✗
```


***

## 📱 Resoluciones y Breakpoints

### **iOS: Dimensiones en Points (pt)**

| Dispositivo | Ancho × Alto (pt) | Scale Factor |
| :-- | :-- | :-- |
| iPhone 16 Pro Max | 430 × 932 | @3x |
| iPhone 16 Pro / 16 | 393 × 852 | @3x |
| iPhone SE | 375 × 667 | @2x |
| iPad Pro 12.9" | 1024 × 1366 | @2x |

[^18][^19]

**Rango mínimo para diseño iOS**: 320pt (iPhone SE antiguo) hasta 1024pt+ (iPad)[^9]

### **Android: Breakpoints (Material Design)**

Sistema de breakpoints en dp:[^20]


| Breakpoint | Dispositivo | Columnas | Gutter |
| :-- | :-- | :-- | :-- |
| **0-360 dp** | Teléfono pequeño | 4 | 16 dp |
| **360-400 dp** | Teléfono mediano | 4 | 16 dp |
| **400-480 dp** | Teléfono grande | 4 | 16 dp |
| **480-600 dp** | Teléfono XL / Landscape | 8 | 16/24 dp |
| **600-840 dp** | Tablet pequeña | 8-12 | 24 dp |
| **840+ dp** | Tablet grande / Desktop | 12 | 24 dp |

### **Conversión dp a px**

Fórmula Android:[^21]

```
px = dp × (dpi / 160)

Ejemplo en pantalla 240 dpi:
48 dp × (240/160) = 48 × 1.5 = 72 px físicos
```


***

## 🎨 Componentes UI: Especificaciones Exactas

### **Botones**

```
Altura estándar iOS: 44 pt (mínimo tappable)
Altura estándar Android: 48 dp

Altura visual recomendada: 56-64 px [cite:51]
Padding interno: 8-16 px horizontal [cite:44]
Border radius: 8-12 px (tendencia moderna)
```


### **Barra de Navegación**

```
iOS Navigation Bar:
  - Primera fila: 44 pt alto
  - Segunda fila: 58 pt alto
  - Status bar: 59 pt alto [cite:23][cite:26]

Android App Bar:
  - Altura estándar: 56-64 px [cite:51]
```


### **Cards**

```
Padding interno: 16 px (estándar) [cite:44][cite:51]
Border radius: 8-16 px
Shadow: Elevation 2-4 dp (Material)
Margin entre cards: 8-16 px vertical
```


### **Campos de Texto (Text Fields)**

```
Altura mínima: 48 dp (Android) / 44 pt (iOS)
Padding interno: 12-16 px
Espacio entre campos relacionados: 8-16 px [cite:51]
```


### **Bottom Navigation**

```
Altura: 56 px (Android)
Altura safe area: 44-88 pt (iOS, dependiendo de modelo) [cite:47]
Íconos: 24×24 dp/pt
```


***

## 🛡️ Safe Areas y Margins

### **iOS Safe Areas**

Introducido en iOS 11, reemplaza top/bottom layout guides:[^22][^23]

```swift
// Safe area protege de:
- Notch / Dynamic Island
- Bordes redondeados
- Home indicator (barra inferior)
- Status bar
```

**Layout Margins por defecto (iOS)**:[^24][^22]

- **Compact (iPhone portrait)**: 16 pt
- **Regular (iPad)**: 20 pt


### **Android System Margins**

Material Design recomienda:[^25]

```
Screen margins: 16-24 dp (lateral)
Top/bottom margins: 16-24 dp
```


### **Desktop/Tablet**

Para artboards de escritorio 1440px:[^16]

```
Container margin: 60 px cada lado
```


***

## ♿ Accesibilidad (Cumplimiento Obligatorio)

### **Contraste de Color (WCAG)**

```
Texto normal: Mínimo 4.5:1 [cite:17][cite:19]
Texto grande (18pt+): Mínimo 3:1 [cite:17][cite:19]
Nunca comunicar info SOLO con color [cite:17]
```


### **Touch Targets Accesibles**

```
Mínimo absoluto: 44×44 pt (iOS) / 48×48 dp (Android)
Recomendado: 48×48 px o mayor [cite:30]
Usuarios con discapacidad motora: 72 px preferido [cite:28]
```


### **Screen Readers**

- **iOS VoiceOver**: Etiquetar todos los elementos con `accessibilityLabel`[^2]
- **Android TalkBack**: Usar `contentDescription`[^7]
- Indicar tipo de elemento (botón, link, header) con traits[^2]


### **Dynamic Type / Font Scaling**

- Soportar zoom hasta 200% sin pérdida de funcionalidad[^13]
- Layouts deben adaptarse a texto más grande[^7][^2]
- Usar unidades relativas (`em`, `rem`, `sp`) en lugar de absolutas[^26]

***

## 📐 Patrones de Layout

### **Diseño Mobile-First**

Comenzar con viewport más pequeño: **320px de ancho**[^26]

```
Proceso:
1. Diseñar para 320-375 px (móvil pequeño)
2. Escalar a 375-414 px (móvil estándar)
3. Adaptar a 600+ px (tablet)
4. Expandir a 1024+ px (desktop)
```


### **Thumb Zone (Zona del Pulgar)**

Elementos críticos en el **tercio inferior** de la pantalla:[^6][^7]

```
Zona verde (fácil alcance): Parte inferior central
Zona amarilla (alcance moderado): Mitad de pantalla
Zona roja (difícil alcance): Esquinas superiores
```

**Implementación:**

- Navegación primaria: Bottom navigation bar
- Acciones principales: Floating Action Button (FAB) abajo derecha
- Evitar botones críticos en esquinas superiores[^7]


### **Jerarquía Visual**

```
Usar contraste de tamaño y peso:
  Hero elements: 32-48 pt
  Body text: 14-16 pt
  Captions: 12 pt
```


***

## 🎨 Diseño de Iconografía

### **Tamaños de Íconos**

```
iOS App Icon (Home screen): 180×180 px
iOS App Store: 1024×1024 px
Settings: 87×87 px
Notifications: 114×114 px
Spotlight search: 120×120 px
```


### **Íconos en UI**

```
Touch target: 44×44 pt (iOS) / 48×48 dp (Android)
Contenido del ícono: Mínimo 24×24 pt/dp [cite:20]
Mantener 10pt de padding interno mínimo
```


### **SF Symbols (iOS)**

- Sistema de iconografía nativa de Apple[^7]
- Auto-adaptan a Dynamic Type
- Consistentes con el sistema operativo

***

## 🧪 Herramientas y Recursos

### **Design Systems Pre-hechos**

1. **Material Design Components**[^4]
    - Librería oficial de Google
    - Componentes listos para usar
    - Theming personalizable
2. **SF Symbols** (iOS)[^7]
    - Más de 5000 íconos
    - Integración nativa

### **Herramientas de Diseño**

- **Figma**: Templates de iOS/Android con especificaciones exactas[^27]
- **Typescale**: Calculadora de escalas tipográficas[^28]
- **Design Lint**: Plugin para verificar consistencia de espaciado[^27]


### **Testing de Accesibilidad**

```
Herramientas de contraste:
- WebAIM Contrast Checker
- Stark (plugin Figma)

Simuladores:
- Xcode Accessibility Inspector (iOS)
- Android Accessibility Scanner
```


***

## 📋 Checklist de Diseño Profesional

### **Espaciado**

- [ ] Sistema de grilla consistente (4pt o 8pt)
- [ ] Regla internal ≤ external aplicada
- [ ] Espaciado entre elementos: múltiplos de base


### **Touch Targets**

- [ ] Mínimo 44×44 pt (iOS) / 48×48 dp (Android)
- [ ] Espaciado 8-12px entre elementos interactivos
- [ ] Padding 16-24px alrededor de acciones primarias


### **Tipografía**

- [ ] Texto de cuerpo: mínimo 16px
- [ ] Interlineado: 120-150% del tamaño
- [ ] Jerarquía clara con escala consistente
- [ ] Soporte para Dynamic Type / Font Scaling


### **Accesibilidad**

- [ ] Contraste 4.5:1 (texto normal) / 3:1 (texto grande)
- [ ] Etiquetas para screen readers en todos los elementos
- [ ] Touch targets cumplen tamaño mínimo
- [ ] Info no comunicada solo con color


### **Layout**

- [ ] Safe areas respetadas (iOS)
- [ ] System margins aplicados (16-24pt/dp)
- [ ] Elementos críticos en thumb zone
- [ ] Responsive desde 320px hasta tablets


### **Componentes**

- [ ] Botones: altura 44-56px
- [ ] Cards: padding interno 16px
- [ ] Navigation bar: altura estándar
- [ ] Border radius consistente (8-12px)

***

## 📚 Referencias Clave

**Documentación Oficial:**

1. Apple Human Interface Guidelines: `developer.apple.com/design`[^1]
2. Material Design 3: `m3.material.io`[^3]
3. WCAG Accessibility: `w3.org/WAI/WCAG21/quickref/`

**Artículos Fundamentales:**

- iOS Design Guidelines Illustrated[^19][^18]
- Material Design Spacing Methods[^25]
- Mobile Typography Best Practices[^29][^28]
- The 8pt Grid System Guide[^16][^14]

***

## 💡 Conclusión

El diseño móvil profesional no es arbitrario - está basado en:

1. **Investigación ergonómica**: Tamaños de dedos, zonas de alcance, legibilidad
2. **Normativas de plataforma**: HIG y Material Design son obligatorias
3. **Accesibilidad**: WCAG garantiza usabilidad para todos
4. **Sistemas matemáticos**: Grilla 8pt crea consistencia visual
5. **Testing de usuario**: Los estándares vienen de pruebas reales

**La clave es consistencia**: Elegir un sistema (4pt o 8pt), seguir las normas de la plataforma, y aplicarlo religiosamente en toda la app. Esto crea interfaces que se sienten profesionales, intuitivas y de calidad de producción.
<span style="display:none">[^30][^31][^32][^33][^34][^35][^36][^37][^38][^39][^40][^41][^42][^43][^44][^45]</span>

<div align="center">⁂</div>

[^1]: https://developer.apple.com/design/human-interface-guidelines

[^2]: https://www.nadcab.com/blog/apple-human-interface-guidelines-explained

[^3]: https://m2.material.io/design/guidelines-overview

[^4]: https://www.geeksforgeeks.org/android/introduction-to-material-design-in-android/

[^5]: https://www.anoda.mobi/ux-blog/ultimate-guide-mobile-app-design-guidelines

[^6]: https://www.andacademy.com/resources/blog/ui-ux-design/complete-guide-to-app-ui-design/

[^7]: https://gegobyteapps.com/resources/mobile-app-design-best-practices

[^8]: https://www.designmonks.co/blog/perfect-mobile-button-size

[^9]: https://moldstud.com/articles/p-a-comprehensive-guide-to-apple-human-interface-guidelines-for-ios-apps

[^10]: https://climbtheladder.com/10-button-size-mobile-app-best-practices/

[^11]: https://uxmovement.com/mobile/optimal-size-and-spacing-for-mobile-buttons/

[^12]: https://thisisglance.com/learning-centre/how-do-i-choose-the-right-font-size-for-my-mobile-app

[^13]: https://robustbranding.com/font-size-guidelines-for-mobile-readability/

[^14]: https://www.rejuvenate.digital/news/designing-rhythm-power-8pt-grid-ui-design

[^15]: https://cieden.com/book/sub-atomic/spacing/choosing-a-spacing-system

[^16]: https://cieden.com/book/sub-atomic/spacing/spacing-best-practices

[^17]: https://pub.dev/documentation/material_design/latest/material_design/M3SpacingToken.html

[^18]: https://learnui.design/blog/ios-design-guidelines-templates.html

[^19]: https://www.learnui.design/blog/ios-design-guidelines-templates.html

[^20]: https://m1.material.io/layout/responsive-ui.html

[^21]: https://stackoverflow.com/questions/10112780/android-minimum-button-height-48dp-9mm

[^22]: https://blog.smartnsoft.com/layout-guide-margins-insets-and-safe-area-demystified-on-ios-10-11-d6e7246d7cb8?gi=6f1d112be1f4

[^23]: https://useyourloaf.com/blog/safe-area-layout-guide/

[^24]: https://app.uxcel.com/courses/apple-hig/layout-fundamentals-spacing-482/apples-margin-guidelines-8925

[^25]: https://m2.material.io/design/layout/spacing-methods.html

[^26]: https://nextnative.dev/blog/mobile-app-ui-design-best-practices

[^27]: https://designguide.dhammike.com

[^28]: https://www.toptal.com/designers/typography/typography-for-mobile-apps

[^29]: https://www.linkedin.com/pulse/typography-best-practices-mobile-apps-priyank-gandhi-xww0f

[^30]: https://developer.android.com/design

[^31]: https://developers.googleblog.com/en/this-is-material-design/

[^32]: https://developers.openai.com/apps-sdk/concepts/ui-guidelines/

[^33]: https://smart-interface-design-patterns.com/articles/accessible-tap-target-sizes/

[^34]: https://stackoverflow.com/questions/35329071/android-button-size-changes-at-different-devices-width-is-set-to-dp

[^35]: https://developer.amazon.com/docs/fire-tablets/ft-button-size-and-placement.html

[^36]: https://developer.android.com/training/multiscreen/screendensities

[^37]: https://m3.material.io/foundations/layout/understanding-layout/spacing

[^38]: https://m3.material.io/foundations/layout/understanding-layout/overview

[^39]: https://developer.android.com/codelabs/adaptive-material-guidance

[^40]: https://thisisglance.com/learning-centre/what-spacing-rules-create-better-mobile-app-layouts

[^41]: https://www.linkedin.com/posts/hamadtanveer_tips-to-master-ui-spacing-in-mobile-app-design-activity-7359803156874035200-S2xg

[^42]: https://mui.com/material-ui/customization/spacing/

[^43]: https://developers.google.com/cars/design/automotive-os/design-system/layout

[^44]: https://www.reddit.com/r/androiddev/comments/1lfgzu0/material_design_3_unclear_about_spacing_between/

[^45]: https://engineering.monday.com/how-uilayoutguide-can-help-you-manage-ui-complexity/

