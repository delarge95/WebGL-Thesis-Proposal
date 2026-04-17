# Plan de Micro-Interacciones con Framer Motion

## Drone Viewer - Capacidades y Tecnología

### Contexto Técnico

- **Proyecto actual**: HTML vanilla con GSAP
- **Migración**: Convertir a componentes React con Framer Motion
- **Objetivo**: Animaciones únicas, sutiles y profesionales temáticas por card

---

## 🎯 Arquitectura Propuesta

### Estructura de Archivos

```
docs/
├── index.html (actual - mantener como fallback)
├── src/
│   ├── main.jsx (entry point React)
│   ├── App.jsx (componente principal)
│   ├── components/
│   │   ├── Hero.jsx
│   │   ├── CapacidadesSection.jsx
│   │   ├── TecnologiaSection.jsx
│   │   ├── DocumentacionSection.jsx
│   │   └── cards/
│   │       ├── VisualizacionCard.jsx
│   │       ├── InteraccionCard.jsx
│   │       ├── IngenieriaCard.jsx
│   │       ├── UnityCard.jsx
│   │       ├── WebGLCard.jsx
│   │       ├── WebAssemblyCard.jsx
│   │       └── ShadersCard.jsx
│   └── styles/
│       └── animations.js (variantes de Framer Motion)
└── package.json
```

---

## 🎨 Micro-Interacciones por Card

### CAPACIDADES

#### 1. Visualización Avanzada (01)

**Temática**: Rayos X, capas, profundidad

**Animación**:

- **Hover**: Efecto de "escaneo de rayos X" horizontal
  - Línea brillante azul que cruza de izquierda a derecha
  - Revelar gradualmente un "segundo layer" con información adicional
  - Icono con efecto de "transparencia" progresiva

```jsx
const variants = {
  initial: { opacity: 1 },
  hover: {
    transition: { staggerChildren: 0.1 },
  },
};

const scanLine = {
  initial: { x: "-100%", opacity: 0 },
  hover: {
    x: "100%",
    opacity: [0, 1, 1, 0],
    transition: { duration: 1.2, ease: "easeInOut" },
  },
};

const layerReveal = {
  initial: { opacity: 0, z: -20 },
  hover: {
    opacity: 0.3,
    z: 0,
    transition: { duration: 0.6 },
  },
};
```

**Efecto visual**: Línea azul neón que "escanea" la card, revelando una capa de wireframe o grid detrás del contenido

---

#### 2. Interacción Total (02)

**Temática**: Despiece explosivo, movimiento orbital

**Animación**:

- **Hover**: Efecto de "despiece micro" donde elementos se separan ligeramente
  - Icono se eleva y rota suavemente
  - Título y texto se separan en capas (parallax interno)
  - Partículas sutiles flotando alrededor

```jsx
const explosionVariants = {
  icon: {
    hover: {
      y: -8,
      rotateY: 15,
      scale: 1.1,
      transition: { type: "spring", stiffness: 300 },
    },
  },
  title: {
    hover: {
      y: -4,
      x: 2,
      transition: { delay: 0.05 },
    },
  },
  content: {
    hover: {
      y: 4,
      x: -2,
      transition: { delay: 0.1 },
    },
  },
};

const particles = Array(5)
  .fill()
  .map((_, i) => ({
    initial: { opacity: 0, scale: 0 },
    hover: {
      opacity: [0, 0.6, 0],
      scale: [0, 1, 0],
      x: [0, (i - 2) * 15],
      y: [0, -20 - i * 5],
      transition: {
        duration: 1.5,
        delay: i * 0.1,
        repeat: Infinity,
      },
    },
  }));
```

**Efecto visual**: La card "explota" suavemente con capas separándose y partículas verdes flotando

---

#### 3. Ingeniería (03)

**Temática**: Medición, precisión, blueprint técnico

**Animación**:

- **Hover**: Líneas de medición aparecen dinámicamente
  - Líneas técnicas (como blueprint) dibuján alrededor del contenido
  - Números/métricas aparecen en las esquinas
  - Efecto de "plano técnico" con grid animado

```jsx
const blueprintVariants = {
  grid: {
    initial: { opacity: 0, scale: 0.95 },
    hover: {
      opacity: 0.15,
      scale: 1,
      transition: { duration: 0.4 },
    },
  },
  measureLines: {
    initial: { pathLength: 0, opacity: 0 },
    hover: {
      pathLength: 1,
      opacity: 0.6,
      transition: {
        duration: 0.8,
        ease: "easeInOut",
        staggerChildren: 0.1,
      },
    },
  },
  metrics: {
    initial: { opacity: 0, scale: 0.8 },
    hover: {
      opacity: 0.7,
      scale: 1,
      transition: { delay: 0.3, duration: 0.3 },
    },
  },
};
```

**Efecto visual**: Grid técnico de fondo + líneas de medición en los bordes + números en las esquinas (todo en púrpura)

---

### TECNOLOGÍA

#### 4. Unity 6 LTS (04)

**Temática**: Motor gráfico, procesamiento de escenas, renderizado

**Animación**:

- **Hover**: Efecto de "render en tiempo real"
  - Icono/texto con efecto de "mesh wireframe" que se construye
  - Transición de wireframe → sólido
  - Brillo cálido como si estuviera renderizando

```jsx
const renderVariants = {
  wireframe: {
    initial: { pathLength: 0, opacity: 0 },
    hover: {
      pathLength: 1,
      opacity: [0, 1, 0.5],
      transition: { duration: 1.2 },
    },
  },
  solid: {
    initial: { opacity: 0 },
    hover: {
      opacity: 1,
      transition: { delay: 0.6, duration: 0.6 },
    },
  },
  glow: {
    initial: { opacity: 0, scale: 0.8 },
    hover: {
      opacity: [0, 0.8, 0],
      scale: [0.8, 1.2, 1],
      transition: {
        duration: 1.5,
        repeat: Infinity,
      },
    },
  },
};
```

**Efecto visual**: Wireframe azul que se dibuja y luego se "renderiza" a sólido con un glow

---

#### 5. WebGL 2.0 (05)

**Temática**: Shaders, GPU, fragmentos de renderizado

**Animación**:

- **Hover**: Efecto de "shader distorsión"
  - Ondas de distorsión RGB (glitch sutil)
  - Gradiente animado tipo "fresnel" en los bordes
  - Partículas de luz tipo GPU particles

```jsx
const shaderVariants = {
  rgbSplit: {
    initial: { x: 0 },
    hover: {
      x: [0, -2, 2, 0],
      transition: {
        duration: 0.4,
        repeat: 2,
      },
    },
  },
  fresnel: {
    initial: {
      background: "transparent",
    },
    hover: {
      background: [
        "linear-gradient(45deg, rgba(251, 146, 60, 0) 0%, transparent 50%)",
        "linear-gradient(45deg, rgba(251, 146, 60, 0.15) 0%, transparent 50%)",
        "linear-gradient(90deg, rgba(251, 146, 60, 0.15) 0%, transparent 50%)",
      ],
      transition: {
        duration: 2,
        repeat: Infinity,
      },
    },
  },
};
```

**Efecto visual**: Glitch RGB sutil + gradiente naranja animado en los bordes

---

#### 6. WebAssembly (06)

**Temática**: Código compilado, binario, procesamiento

**Animación**:

- **Hover**: Efecto de "compilación de código"
  - Texto del título se convierte temporalmente en código binario (0s y 1s)
  - Barras de progreso de compilación aparecen
  - Efecto de "escritura de código" en el fondo

```jsx
const compileVariants = {
  textMorph: {
    initial: { opacity: 1 },
    hover: {
      opacity: [1, 0, 0, 1],
      transition: {
        duration: 1.2,
        times: [0, 0.3, 0.7, 1],
      },
    },
  },
  binaryOverlay: {
    initial: { opacity: 0 },
    hover: {
      opacity: [0, 1, 1, 0],
      transition: {
        duration: 1.2,
        times: [0, 0.3, 0.7, 1],
      },
    },
  },
  progressBar: {
    initial: { scaleX: 0 },
    hover: {
      scaleX: [0, 1],
      transition: { duration: 0.8, ease: "easeOut" },
    },
  },
};
```

**Efecto visual**: Título se "convierte" en binario brevemente + barra de progreso índigo se llena

---

#### 7. HLSL Shaders (07)

**Temática**: Código de shaders, efectos visuales, color grading

**Animación**:

- **Hover**: Efecto de "color grading en vivo"
  - Gradiente multicolor que fluye por la card
  - Efecto de "chromatic aberration" en el texto
  - Fragmentos de código shader flotando en el fondo

```jsx
const colorGradingVariants = {
  gradient: {
    initial: {
      backgroundPosition: "0% 50%",
    },
    hover: {
      backgroundPosition: ["0% 50%", "100% 50%", "0% 50%"],
      transition: {
        duration: 3,
        repeat: Infinity,
        ease: "linear",
      },
    },
  },
  chromaticText: {
    initial: {
      textShadow: "0 0 0 transparent",
    },
    hover: {
      textShadow: [
        "0 0 0 transparent",
        "-2px 0 0 rgba(236, 72, 153, 0.5), 2px 0 0 rgba(59, 130, 246, 0.5)",
        "0 0 0 transparent",
      ],
      transition: {
        duration: 1.5,
        repeat: Infinity,
      },
    },
  },
  codeFragments: {
    initial: { opacity: 0, y: 10 },
    hover: {
      opacity: [0, 0.3, 0],
      y: [10, -20, -40],
      transition: {
        duration: 2,
        repeat: Infinity,
        staggerChildren: 0.2,
      },
    },
  },
};
```

**Efecto visual**: Gradiente rosa-púrpura-azul fluyendo + aberración cromática en texto + código flotando

---

## 🔧 Implementación Técnica

### Paso 1: Setup (Preparación)

1. Instalar dependencias:

```bash
npm init -y
npm install react react-dom framer-motion
npm install -D vite @vitejs/plugin-react
```

2. Configurar Vite (`vite.config.js`):

```js
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  base: "/WebGL_tesis/",
  build: {
    outDir: "dist",
  },
});
```

### Paso 2: Estructura de Componentes

Cada card será un componente React con:

- Layout HTML/JSX
- Variantes de Framer Motion definidas
- Elementos decorativos (SVGs para líneas, grids, etc.)
- Estado hover controlado por Framer Motion

### Paso 3: Migración Gradual

1. **Mantener** `index.html` actual como versión estática
2. **Crear** versión React en `/src`
3. **Compilar** con Vite a `/dist`
4. **Actualizar** GitHub Pages para servir `/dist` cuando esté lista

---

## 📋 Consideraciones

### Performance

- Usar `layout` de Framer Motion solo cuando sea necesario
- Optimizar animaciones con `will-change` CSS
- Lazy load de componentes pesados

### Accesibilidad

- Respetar `prefers-reduced-motion`
- Mantener contraste adecuado durante animaciones
- Asegurar que el contenido sea legible sin hover

### Responsividad

- Simplificar animaciones en móvil
- Reducir partículas y efectos complejos en pantallas pequeñas
- Mantener touch interactions fluidas

---

## 🎬 Timeline de Implementación

1. **Setup**: Instalar React, Framer Motion, Vite (15 min)
2. **Estructura**: Crear componentes base (30 min)
3. **Card 01-03**: Implementar Capacidades (1 hora)
4. **Card 04-07**: Implementar Tecnología (1 hora)
5. **Testing**: Ajustar timings y efectos (30 min)
6. **Deploy**: Build y actualizar GitHub Pages (15 min)

**Total estimado**: ~3.5 horas

---

## ✅ Criterios de Éxito

- ✅ Cada card tiene animación única y temática
- ✅ Animaciones son sutiles y profesionales
- ✅ Performance 60fps en desktop
- ✅ Funciona correctamente en móvil (touch)
- ✅ Respeta `prefers-reduced-motion`
- ✅ Mantiene estética minimalista del diseño actual
- ✅ No hay overlapping ni márgenes rotos
