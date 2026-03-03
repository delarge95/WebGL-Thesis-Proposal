# Portafolio de Tech Artist — Alexander Woodcock Salomón

> **Propósito:** Documento maestro para compilar, organizar y presentar todas las competencias técnicas y artísticas demostradas en este proyecto de tesis como material de portafolio profesional de **Technical Artist**.
>
> **Proyecto de referencia:** *Prototipo Web 3D Interactivo para Visualización Técnica de Drones* — Gemelo Digital del Holybro X500 V2, desplegado en WebGL.

---

## Índice

1. [¿Qué es un Tech Artist y por qué importa?](#1-qué-es-un-tech-artist-y-por-qué-importa)
2. [Tu Perfil Profesional](#2-tu-perfil-profesional)
3. [Inventario de Habilidades Demostrables](#3-inventario-de-habilidades-demostrables)
4. [Piezas de Portafolio Extraíbles del Proyecto](#4-piezas-de-portafolio-extraíbles-del-proyecto)
5. [Cómo Presentar Cada Pieza](#5-cómo-presentar-cada-pieza)
6. [Cómo Vender tu Perfil Tech Artist](#6-cómo-vender-tu-perfil-tech-artist)
7. [Optimización del Perfil de LinkedIn](#7-optimización-del-perfil-de-linkedin)
8. [Portafolio Online: Plataformas y Estructura](#8-portafolio-online-plataformas-y-estructura)
9. [Reel y Material Visual](#9-reel-y-material-visual)
10. [Checklist Final](#10-checklist-final)

---

## 1. ¿Qué es un Tech Artist y por qué importa?

El **Technical Artist** (Tech Artist / TA) es el puente entre el equipo de arte y el equipo de ingeniería en un estudio de desarrollo. Es el rol que traduce la visión artística en soluciones técnicamente viables y performantes.

### Lo que buscan los estudios en un Tech Artist:

| Competencia | Tu proyecto lo demuestra |
|:---|:---|
| Pipeline 3D end-to-end (CAD → Game Engine) | ✅ STEP → Houdini → Blender → Substance → Unity |
| Shader development (HLSL/GLSL) | ✅ 9 shaders personalizados (~1,749 líneas HLSL) |
| Optimización de assets para runtime | ✅ WebGL con presupuesto de 100K polígonos |
| Scripting para herramientas de producción | ✅ 91 scripts C# (~14,778 líneas) |
| UI/UX técnica | ✅ UI Toolkit (UXML + USS), design system completo |
| Profiling y optimización de rendimiento | ✅ WebGLOptimizer, QualityManager, presupuesto de memoria |
| Conocimiento de render pipelines | ✅ URP 17.0.3, Cook-Torrance PBR, IBL |
| Trabajo con datos técnicos reales | ✅ 16 piezas con 32+ campos c/u (DronePartData) |

### Por qué tu proyecto es poderoso como portafolio:

- **No es un simple "modelo bonito"** — es un sistema de ingeniería completo con datos reales
- **Demuestra pensamiento sistémico** — arquitectura de 4 capas con patrones de diseño profesionales
- **Es interactivo y accesible** — cualquier reclutador puede probarlo desde el navegador (WebGL)
- **Cruza disciplinas** — 3D, shaders, UI, datos, optimización, deployment

---

## 2. Tu Perfil Profesional

### Título sugerido (para LinkedIn, CV, portafolio):

> **Technical Artist & 3D Generalist**
> Especializado en pipelines CAD-to-WebGL, shader development y optimización de assets para aplicaciones interactivas en tiempo real.

### Resumen profesional (elevator pitch):

> Tech Artist con experiencia en el pipeline completo de producción 3D: desde modelos CAD industriales (STEP/IGES) hasta aplicaciones WebGL interactivas optimizadas. Combino dominio de herramientas DCC (Blender, Houdini FX, Substance Painter) con programación en C# y HLSL para crear experiencias visuales técnicamente robustas. Mi proyecto de tesis —un gemelo digital interactivo de un dron comercial con 16 piezas, 9 shaders PBR y 7 modos de visualización— fue desarrollado como aplicación WebGL desplegada en navegador, demostrando competencia en optimización de rendimiento, arquitectura de software y diseño de UI.

---

## 3. Inventario de Habilidades Demostrables

### 3.1 Modelado 3D y Pipeline de Assets

| Habilidad | Herramienta | Evidencia del proyecto |
|:---|:---|:---|
| Modelado poligonal y retopología | **Blender 4.x** | Reducción de >2M polígonos CAD a <100K |
| Retopología manual (edge loops funcionales) | Blender | Interfaces de ensamblaje, bordes de tornillos |
| Retopología procedural (Decimate) | Blender | Reducción automática ~95% en áreas uniformes |
| Procesamiento procedural de geometría | **Houdini FX** | Conversión CAD→mesh, PolyReduce, LODs (en desarrollo) |
| Conversión de formatos CAD industriales | Houdini / Blender | STEP/IGES → mesh poligonal optimizada |
| Baking de mapas de normales | Blender | Normal maps 2048×2048, cage baking |
| Texturizado PBR (Metallic/Roughness) | **Substance Painter** | Albedo, Normal, Metallic, Roughness, AO, Emission |
| Materiales realistas específicos | Substance Painter | Fibra de carbono, aluminio anodizado, PCB FR4, plástico ABS |
| Exportación y optimización para game engine | Blender → FBX → Unity | Formato, escalas, pivots, naming conventions |

### 3.2 Shader Development

| Shader | Técnica | Líneas HLSL |
|:---|:---|:---|
| **ClippableLit** | Extensión de URP Lit + plano de corte global (`_ClipPlane`) | ~200 |
| **XRay** | Semi-transparencia con Fresnel, `Blend SrcAlpha OneMinusSrcAlpha` | ~150 |
| **Blueprint** | Detección de bordes Sobel sobre normales, estética de plano técnico | ~200 |
| **Thermal** | Gradiente de color parametrizable (azul→rojo) por temperatura | ~120 |
| **Wireframe** | Geometry Shader (desktop) | ~180 |
| **WireframeWebGL** | Coordenadas baricéntricas (fallback WebGL 2.0 sin Geometry Shaders) | ~200 |
| **SolidColor** | Color plano sin iluminación, configurable | ~80 |
| **Ghosted** | Transparencia extrema + refuerzo de bordes | ~120 |
| **AnimatedGradientSkybox** | Fondo procedural con interpolación de gradientes | ~150 |

**Modelo de iluminación:** Cook-Torrance con distribución GGX + aproximación de Fresnel de Schlick.

### 3.3 Programación y Arquitectura

| Área | Detalle |
|:---|:---|
| **Lenguaje principal** | C# 11 (Unity scripting) — 14,778 líneas |
| **Shading language** | HLSL/CG — 1,749 líneas |
| **UI markup** | UXML + USS (UI Toolkit) — 4,063 líneas |
| **Patrones de diseño** | Singleton, Service Locator, Event Bus (Pub/Sub), Strategy, State Machine, ScriptableObjects |
| **Arquitectura** | 4 capas (Presentación → Lógica → Servicios → Datos) con dependencias descendentes |
| **Principios SOLID** | SRP, Inversión de Dependencias, Abierto/Cerrado, Composición |
| **Compilación** | IL2CPP → WebAssembly (WASM) para navegadores |
| **Versionamiento** | Git + GitHub, 240+ commits, Conventional Commits, rama `feature/phase2-ux-redesign` |

### 3.4 Optimización para Plataforma (WebGL)

| Restricción WebGL | Solución implementada |
|:---|:---|
| Single-threaded | Coroutines asíncronas (sin System.Threading) |
| Sin filesystem | PlayerPrefs → localStorage |
| Sin Geometry Shaders | Shader alternativo con coordenadas baricéntricas |
| Heap limitado (~2 GB) | WebGLOptimizer: monitoreo de heap + GC.Collect() forzado sobre 800 MB |
| Sin System.IO.File | Screenshots vía base64 + JavaScript interop |
| Presupuesto poligonal estricto | Pipeline de retopología: >2M → <100K triángulos |
| Presupuesto de texturas | Compresión ASTC/ETC2, resolución adaptativa |
| Presupuesto de audio | <1.5 MB total, OGG Vorbis, 16 clips |

### 3.5 UI/UX Design

| Elemento | Detalle |
|:---|:---|
| **Framework** | UI Toolkit (UIElements) — equivalente a HTML+CSS |
| **Design System** | Paleta monocromática oscura, tipografía Inter + Space Grotesk, grid de 4px |
| **Componentes** | Bottom Sheet con drag-to-dismiss, toolbar, toast notifications, tooltips, modals |
| **Responsive** | Adaptación automática a diferentes resoluciones |
| **Accesibilidad** | UI Scale, High Contrast mode, Reduce Motion |
| **Controles** | Mouse + touch (1/2/3 dedos), atajos de teclado, gestos pinch-to-zoom |

### 3.6 Datos Técnicos y Gemelo Digital

| Aspecto | Detalle |
|:---|:---|
| **Modelo de referencia** | Holybro X500 V2 (dron real comercial) |
| **Piezas catalogadas** | 16 componentes con 32+ campos técnicos c/u |
| **Datos verificados** | Documentación oficial Holybro, PX4, fichas de fabricante |
| **Sistema de datos** | ScriptableObjects (DronePartData) — datos desacoplados de lógica |
| **Perfiles térmicos** | Datos de temperatura real por componente (~20°C a ~95°C) |
| **Secuencia de ensamblaje** | 10 pasos con prerequisitos, herramientas, torques, seguridad |

### 3.7 Herramientas de Producción (Tooling)

| Herramienta desarrollada | Función |
|:---|:---|
| Vista explosionada con niveles variables | Separación progresiva por orden de ensamblaje |
| Corte transversal en 3 ejes | Plano de corte global compartido por todos los shaders |
| Herramienta de medición 3D | Distancia, ángulo, radio con visualización en tiempo real |
| BOM (Bill of Materials) automático | Generación + exportación CSV |
| Sistema de anotaciones 3D | Notas billboard con persistencia |
| Guía de ensamblaje paso a paso | Navegación, herramientas, dificultad, seguridad, highlight automático |
| Checklist de verificación | Para ensamblaje real del dron |
| Visor de puntos de conexión | Tornillos, snap, soldaduras, cables — codificados por color |
| Motor de animaciones (TweenEngine) | Alternativa liviana a DOTween con 8 tipos de easing |

---

## 4. Piezas de Portafolio Extraíbles del Proyecto

Cada una de estas puede funcionar como una **pieza independiente de portafolio**:

### Pieza 1: "Pipeline CAD-to-WebGL"
- **Qué mostrar:** El flujo completo STEP → Houdini → Blender → Substance → Unity → WebGL
- **Formato:** Diagrama de flujo + Before/After del modelo (wireframe high-poly vs low-poly)
- **Puntos clave:** Reducción >95% de polígonos, baking de normales, materiales PBR realistas
- **Audiencia:** Estudios de videojuegos, empresas de visualización industrial, XR studios

### Pieza 2: "9 Custom Shaders for WebGL"
- **Qué mostrar:** Side-by-side de los 7 modos de visualización + código HLSL del shader más interesante
- **Formato:** Grid de screenshots / GIF animado mostrando cada modo
- **Puntos clave:** Compatibilidad WebGL 2.0 (sin Geometry Shaders), Cook-Torrance PBR, Sobel edge detection
- **Audiencia:** Equipos de rendering, estudios que buscan TAs con conocimiento de shaders

### Pieza 3: "Interactive Digital Twin"
- **Qué mostrar:** Demo en vivo del visor WebGL — selección de piezas, info técnica, exploración
- **Formato:** URL directa al deploy de GitHub Pages + video walkthrough
- **Puntos clave:** 16 piezas reales con datos verificados, 32+ campos por pieza, exploración interactiva
- **Audiencia:** Empresas de ingeniería, defensa, aerospace, educación técnica, digital twins

### Pieza 4: "Exploded View System"
- **Qué mostrar:** Animación de la vista explosionada con niveles variables
- **Formato:** Video/GIF de la explosión progresiva + diagrama de la secuencia de ensamblaje
- **Puntos clave:** 10 niveles de ensamblaje, TweenEngine custom, filtros por categoría
- **Audiencia:** Cualquier estudio que haga visualización técnica o de producto

### Pieza 5: "Thermal Visualization Shader"
- **Qué mostrar:** El shader Thermal con datos reales de temperatura: motores a 95°C en rojo, chasis en azul
- **Formato:** Screenshot con anotaciones técnicas + extracto del código HLSL
- **Puntos clave:** Gradiente basado en datos reales de operación, no decorativo
- **Audiencia:** Empresas de simulación, ingeniería, IoT/digital twins

### Pieza 6: "Cross-Section Tool"
- **Qué mostrar:** Cortes en los 3 ejes revelando el interior del dron
- **Formato:** GIF/video del slider X/Y/Z en acción
- **Puntos clave:** `_ClipPlane` como variable global compartida por todos los shaders — elegancia técnica
- **Audiencia:** Visualización médica, industrial, CAD

### Pieza 7: "UI Design System for 3D Viewers"
- **Qué mostrar:** El sistema de diseño completo: paleta, tipografía, componentes, responsive
- **Formato:** Capturas de la UI + extractos de variables USS + mockup vs resultado
- **Puntos clave:** Monocromático oscuro optimizado para contraste con visor 3D, grid de 4px, accesibilidad
- **Audiencia:** Estudios que valoren la intersección UI/UX + 3D

### Pieza 8: "WebGL Performance Optimization"
- **Qué mostrar:** Métricas antes/después, presupuestos cumplidos, soluciones a restricciones de WebGL
- **Formato:** Tabla de restricciones + soluciones, gráficas de rendimiento
- **Puntos clave:** <100K polígonos, >30 FPS en móvil mid-range, heap management, quality scaling
- **Audiencia:** Cualquier estudio web o mobile que necesite optimización

---

## 5. Cómo Presentar Cada Pieza

### Estructura de presentación (para cada pieza):

```
1. TÍTULO + una línea de contexto
   "Custom HLSL Shaders for WebGL 2.0 — 9 shaders para un gemelo digital interactivo"

2. PROBLEMA / OBJETIVO (2-3 líneas)
   "WebGL 2.0 no soporta Geometry Shaders. Necesitaba 7 modos de visualización
   performantes en navegador, incluyendo wireframe y vista térmica."

3. SOLUCIÓN (3-5 puntos)
   - Implementé coordenadas baricéntricas como fallback para wireframe
   - Cook-Torrance PBR con GGX + Schlick Fresnel
   - Plano de corte global compartido vía _ClipPlane
   ...

4. RESULTADO VISUAL (la estrella)
   - Screenshots/GIFs/Video
   - URL del demo si aplica

5. IMPACTO TÉCNICO (1-2 líneas con números)
   "9 shaders, ~1,749 líneas HLSL, 7 modos de visualización, <33ms frame time"

6. HERRAMIENTAS
   Unity 6, URP 17, HLSL, C#, WebGL 2.0
```

### Reglas de oro:

1. **Muestra, no cuentes.** El primer impacto debe ser visual. Luego viene la técnica.
2. **Cada pieza debe funcionar sola.** Un reclutador puede ver solo una pieza — debe impresionar aislada.
3. **Incluye siempre el "por qué".** No solo "hice un shader"; explica el problema que resolvió.
4. **Los números son tu amigo.** "Reduje de 2M a 85K polígonos" > "Optimicé el modelo".
5. **Demo > Screenshot > Descripción.** Si pueden probarlo, infinitamente mejor.

---

## 6. Cómo Vender tu Perfil Tech Artist

### 6.1 Tu propuesta de valor única (USP)

Tu diferencial sobre otros candidatos Tech Artist:

> **"No soy solo un modelador 3D. Soy alguien que entiende el pipeline completo desde un archivo CAD industrial hasta una aplicación WebGL optimizada que corre en el navegador de un celular."**

Esto es raro. La mayoría de los modeladores 3D saben modelar y texturizar. Pocos saben:
- Escribir shaders HLSL desde cero
- Arquitectar un sistema de software con patrones de diseño
- Optimizar para una plataforma restrictiva como WebGL
- Manejar datos técnicos reales (no assets genéricos de marketplace)
- Diseñar UI/UX para aplicaciones 3D interactivas

### 6.2 Narrativa de entrevista

**Pregunta típica:** *"Cuéntame de un proyecto técnico desafiante."*

> "En mi tesis creé un gemelo digital interactivo de un dron comercial real — el Holybro X500 V2 — como aplicación WebGL que corre en el navegador. El desafío fue convertir modelos CAD industriales de más de 2 millones de polígonos en una escena que corriera a 30+ FPS en un celular, con 7 modos de visualización diferentes y datos técnicos reales.
>
> Desarrollé un pipeline que va desde archivos STEP a través de Houdini y Blender hasta Unity, escribí 9 shaders HLSL personalizados (incluyendo un fallback para wireframe sin Geometry Shaders que WebGL no soporta), y creé un sistema de datos con ScriptableObjects que almacena 32+ campos técnicos verificados para cada una de las 16 piezas.
>
> El resultado es una aplicación de ~14,000 líneas de C# con arquitectura de 4 capas y patrones de diseño profesionales, que cualquiera puede probar desde el navegador."

### 6.3 Keywords para el mercado laboral

Incluir en CV, LinkedIn y portafolio:

**Hard Skills (alta demanda):**
- Technical Artist
- Shader Development (HLSL/GLSL/ShaderLab)
- 3D Pipeline (CAD to Game Engine)
- PBR Materials (Metallic/Roughness workflow)
- WebGL / WebAssembly Optimization
- Unity (URP / HDRP)
- Blender (Retopology, UV, Baking)
- Houdini (Procedural Geometry, LODs)
- Substance Painter (PBR Texturing)
- C# Scripting for Unity
- UI/UX for Real-Time Applications
- Digital Twins / Interactive Visualization
- Performance Profiling & Optimization

**Buzzwords que llaman la atención:**
- Real-Time Rendering
- Cook-Torrance PBR
- Interactive 3D Web Applications
- Cross-Platform (Desktop + Mobile browsers)
- Data-Driven Design (ScriptableObjects)
- SOLID Architecture
- Design Systems

### 6.4 Industrias objetivo

Tu perfil es valioso en estas industrias (ordenadas por fit):

1. **Visualización industrial / product visualization** — Directamente aplicable (gemelos digitales, catálogos 3D)
2. **Videojuegos (AAA / indie)** — Pipeline 3D + shaders + arquitectura
3. **Estudios de XR (VR/AR/MR)** — Optimización de assets, interactividad
4. **Aerospace / Defensa** — Gemelos digitales, simulación, datos técnicos
5. **Educación técnica / e-learning** — Visualización interactiva, WebGL
6. **Automotriz** — Configuradores 3D, visualización de producto
7. **Arquitectura / ArchViz** — WebGL viewers, optimización
8. **IoT / Smart Manufacturing** — Digital twins, sensores, datos en tiempo real

### 6.5 Rangos salariales de referencia (2024-2025)

| Mercado | Junior TA | Mid TA | Senior TA |
|:---|:---|:---|:---|
| USA (remoto) | $55K–75K | $75K–110K | $110K–160K+ |
| Europa (presencial) | €35K–50K | €50K–75K | €75K–110K+ |
| LATAM (remoto para US/EU) | $25K–40K | $40K–65K | $65K–90K+ |
| LATAM (local) | $12K–20K | $20K–35K | $35K–55K |

Tu proyecto te posiciona como **Mid-level TA** en competencias demostradas (pipeline completo + shaders + arquitectura). Con 1-2 años de experiencia en industria, alcanzarías Senior.

---

## 7. Optimización del Perfil de LinkedIn

### 7.1 Headline (título)

**Actual (probable):** `3D Modeler | Blender | Unity | Houdini FX` ← Demasiado genérico.

**Recomendado:**
```
Technical Artist | CAD-to-WebGL Pipeline | Shader Dev (HLSL) | Blender · Houdini · Unity
```

O:
```
Tech Artist & 3D Generalist — Shaders, Optimization & Interactive Visualization | Unity · Blender · Houdini FX
```

### 7.2 About (resumen)

Reemplazar o expandir el resumen actual con algo como:

```
Technical Artist with a full-stack 3D production skillset: from industrial CAD files
(STEP/IGES) to optimized WebGL applications running in-browser.

🔧 Tools: Blender 4.x, SideFX Houdini, Adobe Substance Painter, Unity 6 (URP/HDRP)
💻 Code: C# (14K+ lines), HLSL/CG (custom shaders), UXML/USS (UI Toolkit)
🎯 Focus: Pipeline optimization, shader development, real-time rendering, digital twins

Featured Project — Interactive Drone Digital Twin (WebGL):
• 16 real components of a Holybro X500 V2, each with 32+ verified technical fields
• 9 custom HLSL shaders (X-Ray, Blueprint, Thermal, Wireframe, etc.)
• Full CAD-to-WebGL pipeline: >2M polygons → <100K optimized for mobile browsers
• 91 C# scripts, 4-layer architecture with SOLID principles
• [LINK AL DEPLOY]

Currently completing my Multimedia Engineering thesis at UNAD (Colombia).
Open to Technical Artist, 3D Generalist, or Shader Developer roles — remote or on-site.
```

### 7.3 Featured Section

Agregar al perfil:
1. **Link al deploy WebGL** — para que reclutadores lo prueben en el navegador
2. **Link al repositorio GitHub** — para que vean código y commits
3. **Un PDF/imagen del pipeline diagram** — visualmente atractivo
4. **Un video reel corto (60s)** — mostrando las piezas del portafolio

### 7.4 Experience

Agregar una entrada:

```
Technical Artist / Developer — Thesis Project
UNAD (Universidad Nacional Abierta y a Distancia)
[Fecha inicio] – Present

• Designed and implemented a complete CAD-to-WebGL pipeline for an interactive
  digital twin of the Holybro X500 V2 drone
• Developed 9 custom HLSL shaders for URP, including PBR, X-Ray, Blueprint,
  Thermal visualization, and WebGL-compatible Wireframe
• Built 91 C# scripts (~14,778 lines) with 4-layer architecture using
  Singleton, EventBus, Strategy, and State Machine patterns
• Optimized 3D assets from >2M CAD polygons to <100K triangles while
  maintaining visual fidelity through normal map baking
• Created a full UI design system using Unity UI Toolkit (UXML + USS)
  with responsive layout and accessibility features

Tools: Unity 6, URP, Blender, Houdini FX, Substance Painter, C#, HLSL, WebGL 2.0
```

### 7.5 Skills (ordenar por prioridad)

Asegurar que estas skills estén visibles (pedir endorsements):
1. Technical Art
2. Shader Programming
3. Unity
4. Blender
5. Houdini
6. 3D Modeling
7. PBR Materials
8. WebGL
9. C#
10. HLSL
11. UI/UX Design
12. Performance Optimization
13. Real-Time Rendering
14. Digital Twins

### 7.6 Recomendaciones adicionales para LinkedIn

- **Banner:** Crear un banner personalizado mostrando el visor 3D con un shader llamativo (Blueprint o Thermal)
- **Foto:** Profesional pero accesible. Si estás en un evento de gamedev o con equipo técnico, perfecto
- **Posts sugeridos:**
  - "Escribí 9 shaders custom para WebGL — así resolví el problema del wireframe sin Geometry Shaders" (técnico, con código)
  - "Convertí un archivo CAD industrial de 2M polígonos en un modelo WebGL de 85K — aquí está el pipeline" (with images)
  - "Mi gemelo digital de un dron tiene datos reales de 16 piezas — así se ve la vista térmica" (visual, GIF)
- **Hashtags:** `#TechnicalArtist #GameDev #Shaders #WebGL #Unity3D #Blender3D #Houdini #DigitalTwin #RealTimeRendering #PBR`

---

## 8. Portafolio Online: Plataformas y Estructura

### 8.1 Plataformas recomendadas

| Plataforma | Uso | Prioridad |
|:---|:---|:---|
| **ArtStation** | Portafolio visual principal (estándar de la industria para TA/3D) | 🔴 CRÍTICA |
| **GitHub** | Repositorio de código + README atractivo | 🔴 CRÍTICA |
| **GitHub Pages** | Deploy del visor WebGL (demo interactivo) | 🔴 CRÍTICA |
| **LinkedIn** | Perfil profesional + networking | 🟡 ALTA |
| **Sitio personal** | Landing page con links a todo | 🟢 DESEABLE |
| **YouTube/Vimeo** | Reel y breakdowns | 🟢 DESEABLE |
| **Sketchfab** | Modelo 3D embeddable | 🟢 DESEABLE |

### 8.2 Estructura sugerida del portafolio ArtStation

**Proyecto 1: "Interactive Drone Digital Twin — WebGL"**
```
Thumbnail: Render hero del dron en modo Blueprint o Thermal

Post structure:
1. Hero image (render realista del dron)
2. CTA: "Try it live: [URL]" ← ESTO ES CLAVE — demo interactivo
3. Pipeline diagram (STEP → Houdini → Blender → Substance → Unity → WebGL)
4. Before/After wireframe (high-poly vs low-poly) con conteo de polígonos
5. Material breakdown (Substance Painter screenshots: CF, aluminum, PCB)
6. 7 View Modes grid (Realistic, X-Ray, Blueprint, Thermal, Wireframe, Ghosted, SolidColor)
7. Exploded View animation (GIF/video)
8. Thermal shader close-up con anotaciones
9. UI Design System showcase
10. Architecture diagram (4 layers)
11. Tech stats: "91 scripts, 9 shaders, 16 parts, 32+ fields/part, 240+ commits"
```

**Tags ArtStation:** `unity, webgl, technical-art, shader, hlsl, digital-twin, drone, pbr, blender, houdini, real-time, interactive, visualization`

### 8.3 README de GitHub (showcase)

Asegurar que el README del repositorio incluya:
- Badges (Unity version, license, deploy status)
- GIF animado del visor en acción (primer elemento visual)
- Link al deploy WebGL
- Feature list con screenshots
- Tech stack con versiones
- Architecture diagram
- "How to build" section
- Tabla de piezas del catálogo

---

## 9. Reel y Material Visual

### 9.1 Demo Reel (60-90 segundos)

Estructura sugerida:

```
[0:00-0:05] Logo/nombre + título "Technical Artist Reel"
[0:05-0:15] Hero shot del dron renderizado, rotación suave 360°
[0:15-0:25] Quick cuts de los 7 modos de visualización
[0:25-0:35] Vista explosionada: de ensamblado a completamente separado
[0:35-0:45] Shader Thermal con datos reales + X-Ray view
[0:45-0:55] Corte transversal en 3 ejes
[0:55-1:05] UI interactions: selección de pieza, info panel, catálogo
[1:05-1:15] Pipeline breakdown: CAD → retopología → PBR → WebGL
[1:15-1:20] Stats en pantalla + "Built with Unity, Blender, Houdini, Substance"
[1:20-1:25] Logo + contacto + URL del demo
```

**Música:** Electrónica ambient suave, tech-feel. Free de derechos (Artlist, Epidemic Sound).
**Resolución:** 1920×1080 o 2560×1440. 30fps suave.

### 9.2 Screenshots necesarios

Capturar con F12 (ScreenshotManager) o desde el Editor:

1. **Hero shot** — Dron en modo Realistic, ángulo isométrico, iluminación Studio
2. **Grid de 7 modos** — Mismo ángulo, los 7 modos de visualización
3. **Vista explosionada** — 3 niveles (parcial, medio, completo)
4. **Thermal view** — Close-up mostrando gradiente en motores (rojo) vs chasis (azul)
5. **Blueprint view** — Estilo técnico, gran angular
6. **X-Ray view** — Mostrando componentes internos (Pixhawk, PDB, cables)
7. **Cross-section** — Eje Y mostrando interior
8. **UI showcase** — Bottom sheet con datos de una pieza
9. **Catálogo** — Vista del catálogo filtrado
10. **Puntos de conexión** — Vista con tipos de conexión coloreados

### 9.3 GIFs clave para ArtStation/README

1. Rotación orbital 360° (5 segundos)
2. Transición entre modos de visualización (7 modos en secuencia)
3. Explosión/implosión animada
4. Corte transversal slider (un eje)
5. Hover sobre pieza → selección → info panel

---

## 10. Checklist Final

### Para completar el portafolio

- [ ] **Pipeline diagram** — Crear diagrama visual del flujo CAD→WebGL (Figma, draw.io, o Houdini network screenshot)
- [ ] **Hero screenshots** — Los 10 screenshots listados en §9.2
- [ ] **GIFs** — Los 5 GIFs clave de §9.3
- [ ] **Demo reel** — Video de 60-90s (OBS/Unity Recorder)
- [ ] **ArtStation post** — Publicar con estructura de §8.2
- [ ] **GitHub README** — Actualizar con estructura de §8.3 + badges + GIF
- [ ] **LinkedIn headline** — Cambiar a formato sugerido en §7.1
- [ ] **LinkedIn About** — Expandir con texto sugerido en §7.2
- [ ] **LinkedIn Experience** — Agregar entrada del proyecto (§7.4)
- [ ] **LinkedIn Featured** — Agregar link al deploy + repo + PDF
- [ ] **LinkedIn Skills** — Reordenar las 14 skills clave (§7.5)
- [ ] **LinkedIn banner** — Crear banner personalizado
- [ ] **LinkedIn posts** — Publicar al menos 2 posts técnicos de los sugeridos (§7.6)
- [ ] **Deploy WebGL final** — Asegurar que el URL público funcione
- [ ] **Completar datos de Houdini** — Cuando el pipeline esté finalizado, actualizar pipeline diagram
- [ ] **Completar audio** — Cuando los clips estén listos, actualizar la descripción
- [ ] **Solicitar endorsements** — Pedir a colegas/asesor que endorsen las skills clave en LinkedIn

### Priorización

1. 🔴 **Inmediato:** Deploy WebGL + ArtStation post + LinkedIn update
2. 🟡 **Esta semana:** Hero screenshots + GIFs + GitHub README
3. 🟢 **Próxima semana:** Demo reel + LinkedIn posts + Pipeline diagram
4. ⚪ **Cuando esté listo:** Completar pipeline con Houdini + audio

---

## Nota sobre LinkedIn

> No fue posible acceder al perfil de LinkedIn (`https://www.linkedin.com/in/alexander-woodcock-0132382a6/`) por restricciones de scraping de la plataforma. Las recomendaciones de §7 son genéricas basadas en tu perfil profesional conocido (3D modeler, Blender, Unity, Houdini FX) y las mejores prácticas para perfiles Tech Artist. **Revisa tu perfil actual y aplica las sugerencias que consideres pertinentes.**

---

*Documento generado como parte de la documentación del proyecto de tesis. Última actualización: 2025.*
