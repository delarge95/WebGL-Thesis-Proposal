# Arquitectura React - TwinSight X500

## 📁 Estructura del proyecto

```
src/
├── components/
│   ├── cards/
│   │   ├── Card.jsx                 # Componente genérico de tarjeta
│   │   └── Card.css                 # Estilos base de tarjetas
│   └── sections/
│       ├── Hero.jsx                 # Sección hero/bienvenida
│       ├── Hero.css
│       ├── CapacidadesSection.jsx   # 3 cards de capacidades
│       ├── TecnologiaSection.jsx    # 4 cards de tecnología
│       ├── Section.css              # Estilos compartidos
│       ├── Footer.jsx               # Footer con link a GitHub
│       ├── Footer.css
│       └── animationVariants.js     # Variantes de Framer Motion (7 animaciones)
│
├── hooks/
│   ├── useScrollTrigger.js          # Hook para scroll animations
│   ├── useCardHover.js              # Hook para hover states
│   └── useMediaQuery.js             # Hook para responsive breakpoints
│
├── utils/
│   ├── animations.js                # Funciones helper para animaciones
│   └── helpers.js                   # Utilidades generales
│
├── constants/
│   ├── data.js                      # Data de cards y links
│   ├── animations.js                # Configuración de animaciones
│   └── theme.js                     # Variables de tema y colores
│
├── App.jsx                          # Componente principal
├── App.css                          # Estilos globales de app
├── main.jsx                         # Entry point
└── index.css                        # CSS reset y variables globales
```

## 🎨 Sistema de componentes

### Card Component (Genérico)
- Props: `id`, `title`, `description`, `animationType`, `animationVariants`
- Recibe variantes de Framer Motion específicas por tipo
- Estructura modular con capas de animación

### Secciones
- **Hero**: Animación de entrada secuencial (title → subtitle → CTA)
- **CapacidadesSection**: 3 cards con animaciones únicas
- **TecnologiaSection**: 4 cards con animaciones únicas
- **Footer**: Simple con link a GitHub

## 🎭 Sistema de animaciones

### 7 Animaciones únicas (Framer Motion)

**Capacidades:**
1. **Visualización** (`xray-scan`) - Efecto de escaneo con revelado de capas
2. **Interacción** (`explosive-despiece`) - Despiece explosivo con partículas
3. **Ingeniería** (`blueprint-lines`) - Líneas de medición blueprint

**Tecnología:**
4. **Unity** (`wireframe-render`) - Wireframe → render sólido
5. **WebGL** (`rgb-glitch`) - Glitch RGB + fresnel
6. **WebAssembly** (`binary-compile`) - Compilación binaria
7. **HLSL** (`chromatic-aberration`) - Aberración cromática

### Implementación
- Archivo: `src/components/sections/animationVariants.js`
- Cada animación tiene: `initial`, `animate`, `hover`
- Helper functions: `getCapacidadAnimationVariants()`, `getTecnologiaAnimationVariants()`

## 🎨 Sistema de colores

```css
--color-visualizacion: #3b82f6  (Blue)
--color-interaccion: #10b981    (Green)
--color-ingenieria: #8b5cf6     (Purple)
--color-unity: #f59e0b          (Amber)
--color-webgl: #ef4444          (Red)
--color-webassembly: #06b6d4    (Cyan)
--color-hlsl: #ec4899           (Pink)
```

## 🔧 Hooks personalizados

1. **useScrollTrigger**: Detecta scroll para trigger animations
2. **useCardHover**: Maneja hover states con debouncing
3. **useMediaQuery**: Responsive breakpoints (mobile/tablet/desktop)

## 📦 Data management

Toda la data está centralizada en `constants/data.js`:
- `CAPACIDADES_DATA` - Array de 3 cards
- `TECNOLOGIA_DATA` - Array de 4 cards
- `DOCS_DATA` - Links de documentación
- `GITHUB_URL` - Link del repositorio

## 🚀 Siguiente paso: Implementar animaciones

Los archivos están listos con placeholders (TODO comments) en:
- `src/components/sections/animationVariants.js`

Cada variante necesita:
- Configuración de Framer Motion `whileHover`
- Efectos visuales específicos (CSS o SVG)
- Timing y easing apropiados

## 🛠️ Comandos de desarrollo

```bash
npm run dev      # Servidor local (localhost:3000)
npm run build    # Build producción
npm run preview  # Preview del build
```
