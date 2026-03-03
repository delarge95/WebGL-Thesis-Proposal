# Configuración WebGL — WebGL Drone Viewer

> **Versión:** 1.0  
> **Última actualización:** Julio 2025  
> **Motor:** Unity 6000.0.62f1 · URP 17.0.3 · WebGL 2.0  
> **Referencia:** `ProjectSettings/ProjectSettings.asset`

---

## 1. Resumen de Configuración

El proyecto está configurado para WebGL 2.0 con compilación IL2CPP a WebAssembly (WASM). Se utiliza el template personalizado `PROJECT:Custom` con un loader y UI de carga propios.

---

## 2. Player Settings — WebGL

### 2.1 Resolución y Presentación

| Parámetro             | Valor            | Notas                                           |
| --------------------- | ---------------- | ----------------------------------------------- |
| Default Canvas Width  | 960              | Resolución inicial del canvas                   |
| Default Canvas Height | 600              | Se escala responsivamente via CSS               |
| Run In Background     | ✅               | Permite ejecución cuando la pestaña pierde foco |
| WebGL Template        | `PROJECT:Custom` | Template con loader y UI personalizados         |

### 2.2 Memoria WebAssembly

| Parámetro             | Valor     | Descripción                          |
| --------------------- | --------- | ------------------------------------ |
| Initial Memory Size   | 32 MB     | Memoria inicial del heap WASM        |
| Maximum Memory Size   | 256 MB    | Límite máximo del heap               |
| Memory Growth Mode    | Geometric | Crece exponencialmente según demanda |
| Geometric Growth Step | 0.2 (20%) | Incremento porcentual por paso       |
| Geometric Growth Cap  | 96 MB     | Máximo incremento por paso           |
| Linear Growth Step    | 16 MB     | Alternativa lineal (no activa)       |

**Justificación:** El crecimiento geométrico permite que la app arranque con poco uso de memoria (32 MB) y escale eficientemente hasta 256 MB solo si el modelo 3D y las texturas lo requieren. El cap de 96 MB previene picos de asignación excesivos.

### 2.3 Compilación IL2CPP

| Parámetro               | Valor                        | Descripción                           |
| ----------------------- | ---------------------------- | ------------------------------------- |
| Linker Target           | WASM                         | WebAssembly (no asm.js)               |
| IL2CPP Configuration    | Master                       | Máxima optimización de código         |
| Exception Support       | Explicitly Thrown Only       | Reduce tamaño del binario             |
| Strip Engine Code       | ✅                           | Elimina código de engine no utilizado |
| Managed Stripping Level | High                         | Elimina código IL no referenciado     |
| Scripting Defines       | `APP_UI_EDITOR_ONLY;DOTWEEN` | Defines de compilación condicional    |

### 2.4 Publicación

| Parámetro              | Valor       | Descripción                                   |
| ---------------------- | ----------- | --------------------------------------------- |
| Compression Format     | Gzip        | Compresión de archivos de build               |
| Decompression Fallback | ❌ Disabled | El servidor debe servir con encoding correcto |
| Name Files As Hashes   | ✅          | Cache busting automático                      |
| Data Caching           | ✅          | Almacena assets en IndexedDB                  |
| Debug Symbols          | ❌          | No incluir en release                         |
| Analyze Build Size     | ❌          | Solo activar para debugging                   |

### 2.5 Rendering

| Parámetro         | Valor            | Descripción                                  |
| ----------------- | ---------------- | -------------------------------------------- |
| Color Space       | Linear           | Rendering PBR correcto                       |
| Auto Graphics API | WebGL 2.0        | OpenGL ES 3.0 subyacente                     |
| Power Preference  | High Performance | Solicita GPU dedicada cuando esté disponible |
| WebGPU            | ❌ Disabled      | No soportado aún por el proyecto             |
| Multithreading    | ❌ Disabled      | SharedArrayBuffer requiere COOP/COEP headers |

---

## 3. URP (Universal Render Pipeline) — Configuración WebGL

### 3.1 URP Asset

| Parámetro         | Valor WebGL                                 |
| ----------------- | ------------------------------------------- |
| Render Scale      | 1.0 (ajustable por WebGLOptimizer)          |
| Main Light        | Per Pixel                                   |
| Additional Lights | Per Pixel (máx. 4)                          |
| Shadows           | Habilitadas (desactivables en móvil)        |
| Shadow Resolution | 1024                                        |
| Shadow Distance   | 50                                          |
| HDR               | ❌ (WebGL 2.0 limitado)                     |
| Anti-Aliasing     | MSAA 2x                                     |
| Post-Processing   | Habilitado (bloom, vignette, color grading) |
| SRP Batcher       | ✅                                          |
| Dynamic Batching  | ❌ (incompatible con SRP Batcher)           |

### 3.2 Quality Settings

| Nivel  | Uso                  | Shadow | Post-FX  | AA      |
| ------ | -------------------- | ------ | -------- | ------- |
| Low    | Mobile/Low-end       | ❌     | ❌       | None    |
| Medium | Desktop promedio     | ✅     | Parcial  | MSAA 2x |
| High   | Desktop GPU dedicada | ✅     | Completo | MSAA 4x |

El `WebGLOptimizer` selecciona automáticamente el nivel según las capacidades detectadas del dispositivo.

---

## 4. WebGL Template Personalizado

### 4.1 Estructura

```
Assets/WebGLTemplates/Custom/
├── index.html          # HTML principal con loader
├── style.css           # Estilos del loader y canvas
├── thumbnail.png       # Preview en Unity Editor
└── TemplateData/
    ├── style.css       # Estilos de la barra de progreso
    ├── favicon.ico     # Favicon del visor
    └── progress-bar-*.png  # Assets de la barra de carga
```

### 4.2 Características del Template

- **Loader personalizado** con barra de progreso y mensajes de estado
- **Detección de WebGL 2.0** con mensaje de fallback si no está soportado
- **Responsive:** el canvas ocupa el 100% del viewport
- **Fullscreen API** integrada
- **Error handling** con mensajes amigables para errores de carga

---

## 5. WebGLOptimizer — Ajustes en Runtime

El sistema `WebGLOptimizer` (singleton persistente) monitorea el rendimiento y ajusta la calidad automáticamente durante la ejecución:

### 5.1 Detección de Dispositivo

```
User-Agent → Detecta: Mobile / Tablet / Desktop
WebGL Renderer string → Detecta: GPU integrada vs. dedicada
```

### 5.2 Ajustes Automáticos

| Condición                   | Acción                                            |
| --------------------------- | ------------------------------------------------- |
| Dispositivo móvil detectado | ↓ Texture mip bias, ↓ LOD bias, ❌ Sombras        |
| FPS < target - 5            | ↓ Render scale (−0.1)                             |
| FPS > target + 10           | ↑ Render scale (+0.05, máx. 1.0)                  |
| Heap > 800 MB               | `GC.Collect()` + `Resources.UnloadUnusedAssets()` |
| GPU integrada               | ❌ Post-procesado, ↓ Particle budget              |

### 5.3 Monitoreo (WebGLProfiler)

| Métrica    | Umbral Good | Umbral OK | Warning  | Critical |
| ---------- | ----------- | --------- | -------- | -------- |
| FPS        | ≥ 55        | ≥ 30      | ≥ 20     | < 20     |
| Frame Time | ≤ 18 ms     | ≤ 33 ms   | ≤ 50 ms  | > 50 ms  |
| Heap       | < 400 MB    | < 600 MB  | < 800 MB | ≥ 800 MB |

---

## 6. Optimizaciones de Texturas

### 6.1 Import Settings Recomendados

| Tipo de Textura      | Max Size | Formato           | Compresión     | Mip Maps |
| -------------------- | -------- | ----------------- | -------------- | -------- |
| Albedo / Diffuse     | 2048     | ASTC 6x6 / DXT5   | Normal Quality | ✅       |
| Normal Map           | 2048     | ASTC 6x6 / DXT5nm | Normal Quality | ✅       |
| Metallic / Roughness | 1024     | ASTC 8x8 / DXT1   | Low Quality    | ✅       |
| AO / Emission        | 1024     | ASTC 8x8 / DXT1   | Low Quality    | ✅       |
| UI Assets            | 512      | RGBA 32bit        | None           | ❌       |
| Skybox / Environment | 2048     | ASTC 6x6 / DXT5   | Normal Quality | ✅       |

### 6.2 Presupuesto de Texturas

- **Objetivo total:** < 20 MB en memoria (descomprimido)
- **WebGL 2.0:** Soporta texturas comprimidas ETC2 / ASTC
- **Fallback:** DXT para desktop, ETC2 para mobile

---

## 7. Shaders — Consideraciones WebGL 2.0

### 7.1 Limitaciones de la Plataforma

| Limitación          | Descripción                            | Mitigación                           |
| ------------------- | -------------------------------------- | ------------------------------------ |
| No Geometry Shaders | WebGL 2.0 no soporta                   | Uso de vertex shaders para wireframe |
| No Compute Shaders  | OpenGL ES 3.0 subyacente               | Cálculos en CPU o vertex shader      |
| No Tessellation     | No disponible en ES 3.0                | Normal maps para detalle             |
| Variantes limitadas | Compilación lenta con muchas variantes | `#pragma skip_variants` agresivo     |
| Precision mediump   | Precisión por defecto en mobile        | `float` explícito donde se necesita  |

### 7.2 Shader Compilation

- **Pre-warm:** Los shaders se pre-compilan en el primer frame via `ShaderVariantCollection`
- **Shader stripping:** El `WebGLBuildFixer` elimina variantes no utilizadas
- **Caché:** WebGL 2.0 cachea shaders compilados por sesión

---

## 8. Configuración de Audio WebGL

### 8.1 Limitaciones de Audio en WebGL

| Limitación             | Descripción                                   |
| ---------------------- | --------------------------------------------- |
| No streaming de audio  | Todo el audio se carga en memoria             |
| Autoplay bloqueado     | Requiere interacción del usuario para iniciar |
| Formatos soportados    | AAC, MP3, OGG Vorbis, WAV                     |
| Compresión recomendada | Vorbis (OGG) para balance tamaño/calidad      |

### 8.2 Import Settings

| Parámetro          | Valor                                                    |
| ------------------ | -------------------------------------------------------- |
| Force To Mono      | ✅ (UI sounds) / ❌ (ambient)                            |
| Load Type          | Decompress On Load (UI) / Compressed In Memory (ambient) |
| Compression Format | Vorbis                                                   |
| Quality            | 70% (UI) / 50% (ambient)                                 |
| Sample Rate        | Optimized                                                |

### 8.3 Presupuesto de Audio

- **Budget total:** < 1.5 MB
- **Clips UI:** < 50 KB cada uno
- **Ambient:** < 500 KB

---

## 9. Parámetros de Build Avanzados

### 9.1 Scripting Defines (WebGL)

```
APP_UI_EDITOR_ONLY    // Excluye código solo-editor de App UI
DOTWEEN               // Habilita DOTween tweening library
```

### 9.2 Variables de Entorno

El proyecto no requiere variables de entorno externas. Toda la configuración se maneja internamente mediante:

- `ProjectSettings/ProjectSettings.asset` — Configuración de Unity
- `WebGLOptimizer` — Ajustes de runtime
- `SaveSystem` → `PlayerPrefs` — Preferencias del usuario (volumen, calidad)

---

## 10. Compatibilidad con Navegadores

### 10.1 APIs Web Utilizadas

| API              | Uso                    | Soporte                           |
| ---------------- | ---------------------- | --------------------------------- |
| WebGL 2.0        | Rendering 3D           | Chrome 56+, Firefox 51+, Edge 79+ |
| WebAssembly      | Código compilado       | Universal (2017+)                 |
| IndexedDB        | Data caching           | Universal                         |
| Web Audio API    | Sistema de audio       | Universal                         |
| Fullscreen API   | Modo pantalla completa | Universal                         |
| Pointer Lock API | Captura de cursor      | Universal                         |
| Gamepad API      | (futuro) Control       | Chrome, Firefox, Edge             |

### 10.2 Navegadores No Soportados

- Internet Explorer (cualquier versión)
- Safari < 15 (WebGL 2.0 parcial)
- Navegadores de consolas de juegos
- Navegadores con WebGL deshabilitado por política
