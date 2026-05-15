# Documento 3 — Pipeline de Renderizado y Shaders

## Aplicación Web Interactiva para Visualización 3D de Drones

### Análisis Exhaustivo del Pipeline Gráfico y Shaders Personalizados

**Proyecto de Grado:** Ingeniería Multimedia — UNAD  
**Plataforma:** Unity 6.0 LTS → WebGL (URP)  
**Rama:** `feature/phase2-ux-redesign`

> Actualizacion 2026-05-08: el pipeline final de activos Blender -> Unity queda definido con texturas externas y export manual. Los mapas runtime recomendados son `X500_BaseColor_4K.png`, `X500_Normal_Final_4K.png` y `X500_Mask_4K.png`, con mask `R=AO`, `G=Roughness`, `B=Curvature`, `A=Metallic`. La fibra de carbono se bakea desde el material Blender existente, no se recrea como material procedural independiente en Unity.

> Nota editorial: este documento profundiza en el pipeline gráfico y en los shaders. La arquitectura canónica del cierre debe leerse junto con `01_Arquitectura_del_Sistema.md` y `02_Referencia_Tecnica_Modulos.md`.

---

## Tabla de Contenidos

1. [Visión General del Pipeline URP](#1-visión-general-del-pipeline-urp)
2. [Sistema Global de Clipping Dual](#2-sistema-global-de-clipping-dual)
3. [Gestión de Materiales en Runtime](#3-gestión-de-materiales-en-runtime)
4. [Shader: ClippableLit](#4-shader-clippablelit)
5. [Shader: Blueprint](#5-shader-blueprint)
6. [Shader: XRay](#6-shader-xray)
7. [Shader: SolidColor](#7-shader-solidcolor)
8. [Shader: Thermal](#10-shader-thermal)
9. [Shader: AnimatedGradientSkybox](#12-shader-animatedgradientskybox)
10. [Restricciones WebGL y Estrategias de Compatibilidad](#13-restricciones-webgl-y-estrategias-de-compatibilidad)
11. [MaterialPropertyBlock: Uso y Justificación](#14-materialpropertyblock-uso-y-justificación)
12. [Inventario de Propiedades Shader Globales](#15-inventario-de-propiedades-shader-globales)

---

## 1. Visión General del Pipeline URP

### 1.1 Universal Render Pipeline en WebGL

La aplicación usa **Universal Render Pipeline (URP)**, el pipeline de renderizado recomendado por Unity para plataformas con recursos limitados como WebGL. URP ejecuta un pipeline de renderizado **single-pass forward** que procesa cada objeto en una sola pasada con todas las luces relevantes, a diferencia del pipeline Built-in que podía requerir múltiples pasadas por luz.

### 1.2 Cadena de Renderizado por Frame

```
Frame Loop (WebGL = single-threaded, ~60Hz target)
│
├── 1. CPU: Update() de todos los MonoBehaviours
│   ├── InputManager → bloqueo de input
│   ├── SelectionManager → raycast hover/click
│   ├── OrbitCameraController.Update() → procesa input de cámara
│   └── DroneStateController.Update() → rota hélices, hovering
│
├── 2. CPU: LateUpdate()
│   ├── OrbitCameraController → interpola posición/rotación final
│   ├── SmartHotspot → proyección 3D→2D (staggered occlusion)
│   └── BillboardBehavior → orienta textos de anotaciones hacia cámara
│
├── 3. GPU: URP Render Pipeline
│   ├── Cull: Frustum culling + occlusion
│   ├── ShadowCaster Pass (solo shaders con "LightMode"="ShadowCaster")
│   │   └── ClippableLit, SolidColor → generan sombras
│   ├── UniversalForward Pass (main rendering)
│   │   ├── Opaque queue (RenderQueue ≤ 2500)
│   │   │   ├── ClippableLit → PBR + dual clip
│   │   │   ├── Blueprint → grid + fresnel
│   │   │   ├── SolidColor → Blinn-Phong + outline
│   │   │   ├── Wireframe → geometry shader edges (Desktop)
│   │   │   └── WireframeWebGL → UV grid (WebGL fallback)
│   │   └── Transparent queue (RenderQueue > 2500)
│   │       ├── XRay → dual-pass (behind + front)
│   │       ├── Ghosted → fresnel alpha blend
│   │       └── Thermal → procedural heat map
│   └── Post-processing (si habilitado)
│
├── 4. GPU: Skybox
│   └── AnimatedGradientSkybox → gradient radial animado
│
└── 5. Debug / overlays legados
    ├── utilidades de inspección histórica o experimental
    └── no forman parte del flujo visible final documentado
```

### 1.3 Integración Shader ↔ Manager

| Manager                 | Shader(s) Usado(s)            | Mecanismo                                                                  |
| ----------------------- | ----------------------------- | -------------------------------------------------------------------------- |
| `ViewModeManager`       | Modos de vista operativos     | `Shader.Find()` → `new Material(shader)` → `renderer.sharedMaterial = mat` |
| `CrossSectionManager`   | ClippableLit (modo Realistic) | `Shader.SetGlobalVector/Float` para planos globales                        |
| `HighlightSystem`       | Cualquiera activo             | `MaterialPropertyBlock.SetColor("_BaseColor"/"_EmissionColor")`            |
| `PartVisibilityManager` | Cualquiera activo             | `MaterialPropertyBlock.SetColor` con alpha (fade)                          |
| `MaterialController`    | XRay específicamente          | `renderer.sharedMaterial = xrayMaterial` (swap directo)                    |
| `EnvironmentController` | AnimatedGradientSkybox        | `RenderSettings.skybox = skyboxMaterial`                                   |

---

## 2. Sistema Global de Clipping Dual

### 2.1 Arquitectura del Sistema

El sistema de corte transversal permite al usuario revelar el interior del modelo cortando la geometría con hasta **2 planos simultáneos**. La implementación se basa en **propiedades de shader globales** que todos los shaders de la aplicación leen, produciendo un corte coherente a través de todos los materiales.

### 2.2 Propiedades Globales

| Propiedad             | Tipo      | Rango | Descripción                                                             |
| --------------------- | --------- | ----- | ----------------------------------------------------------------------- |
| `_GlobalClipPlane`    | `Vector4` | —     | Ecuación del plano `(nx, ny, nz, d)` donde `nx·x + ny·y + nz·z + d = 0` |
| `_GlobalClipEnabled`  | `Float`   | 0 o 1 | Activa/desactiva el primer plano                                        |
| `_GlobalClipPlane2`   | `Vector4` | —     | Ecuación del segundo plano                                              |
| `_GlobalClipEnabled2` | `Float`   | 0 o 1 | Activa/desactiva el segundo plano                                       |

### 2.3 Código HLSL Compartido (Patrón Común)

Todos los 9 shaders implementan **idéntico** bloque de clipping en su fragment shader:

```hlsl
// Declaraciones (fuera del fragment)
float4 _GlobalClipPlane;
float  _GlobalClipEnabled;
float4 _GlobalClipPlane2;
float  _GlobalClipEnabled2;

// Dentro del fragment shader:
float3 worldPos = IN.positionWS;  // o posWorld, worldPosition según shader

// Plano 1
if (_GlobalClipEnabled > 0.5)
{
    float dist = dot(worldPos, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
    if (dist < 0) discard;
}

// Plano 2
if (_GlobalClipEnabled2 > 0.5)
{
    float dist2 = dot(worldPos, _GlobalClipPlane2.xyz) + _GlobalClipPlane2.w;
    if (dist2 < 0) discard;
}
```

### 2.4 Cálculo de la Ecuación del Plano (C# — CrossSectionManager)

```csharp
Vector3 normal = GetNormal(axis, inverted);
Vector3 point = worldCenter + GetAxisVector(axis) * pos;
Vector4 planeEquation = new Vector4(
    normal.x,
    normal.y,
    normal.z,
    -Vector3.Dot(normal, point));

Shader.SetGlobalVector("_GlobalClipPlane", planeEquation);
Shader.SetGlobalFloat("_GlobalClipEnabled", 1f);
```

La forma aplicada en runtime coincide con la ecuación estándar del plano:

```text
n · x + d = 0
```

donde `d = -dot(n, point)`.

### 2.5 ¿Por qué `discard` y no Alpha Clipping?

- **`discard`** elimina el fragmento completamente de la rasterización: no genera profundidad, no genera color, no genera sombra. Es el equivalente GPU de "este pixel no existe".
- **Alpha clipping** (`clip(alpha - threshold)`) mantiene el overhead de calcular todos los parámetros del fragmento antes de descartarlo. Además, alpha clipping interactúa con el blending de formas no deseadas en shaders transparentes.
- El `discard` directo basado en geometría (dot product) es computacionalmente más eficiente: solo una multiplicación + comparación antes de cualquier cálculo de iluminación.

---

## 3. Gestión de Materiales en Runtime

### 3.1 Tres Mecanismos de Modificación

La aplicación emplea tres estrategias complementarias para modificar la apariencia visual:

#### 3.1.1 Reemplazo Completo de Material (`ViewModeManager`)

```csharp
Material newMat = new Material(Shader.Find("WebGL/Blueprint"));
renderer.sharedMaterial = newMat;
```

- **Cuándo:** Al cambiar entre modos de visualización implementados; la UI final expone solo el subconjunto operativo documentado.
- **Impacto:** Reemplaza la referencia de material. Los originales se almacenan en `_originalMaterials` para restauración.
- **Advertencia:** `new Material()` crea una instancia en memoria que debe gestionarse. En esta implementación, el material antiguo (no original) queda huérfano para el GC de Unity.

#### 3.1.2 `MaterialPropertyBlock` Override (`HighlightSystem`, `PartVisibilityManager`)

```csharp
MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
propBlock.SetColor("_BaseColor", highlightColor);
renderer.SetPropertyBlock(propBlock);
```

- **Cuándo:** Hover, selección, fade de visibilidad.
- **Impacto:** Override temporal per-renderer sin clonar material. Todos los renderers que comparten el mismo material mantienen su propia visualización independiente.
- **Ventaja:** Zero allocation de materiales. La misma instancia de `MaterialPropertyBlock` se reutiliza.

#### 3.1.3 Propiedades Globales (`CrossSectionManager`)

```csharp
Shader.SetGlobalVector("_GlobalClipPlane", equation);
```

- **Cuándo:** Activación/posicionamiento de planos de corte.
- **Impacto:** Afecta a TODOS los shaders que declaren esa propiedad. No requiere iteración por renderer.
- **Ventaja:** O(1) independientemente del número de objetos en la escena.

### 3.2 Orden de Precedencia

Cuando múltiples sistemas modifican la apariencia simultáneamente:

```
1. Material base (sharedMaterial) → Determina el shader y propiedades base
2. MaterialPropertyBlock → Override per-renderer (hover/select/fade)
3. Propiedades globales → Aplican sobre cualquier material (clipping)
```

`MaterialPropertyBlock` siempre gana sobre las propiedades del material para las propiedades que define. Las propiedades globales se leen directamente en el shader y no pasan por el sistema de materiales.

---

## 4. Shader: ClippableLit

**Ruta:** `Assets/Shaders/ClippableLit.shader`  
**Líneas:** 253  
**Nombre:** `"WebGL/ClippableLit"`  
**Tags:** `"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"`

### 4.1 Propósito

Shader PBR completo compatible con URP que soporta clipping global dual. Es el reemplazo de `Universal Render Pipeline/Lit` cuando el usuario activa el corte transversal en modo Realistic, ya que el shader URP estándar no implementa las propiedades globales de clipping.

### 4.2 Properties

| Propiedad            | Tipo        | Default   | Descripción                                 |
| -------------------- | ----------- | --------- | ------------------------------------------- |
| `_BaseMap`           | 2D          | "white"   | Textura albedo                              |
| `_BaseColor`         | Color       | (1,1,1,1) | Tint multiplicativo                         |
| `_NormalMap`         | 2D          | "bump"    | Mapa de normales                            |
| `_NormalScale`       | Float       | 1.0       | Intensidad de normales                      |
| `_MetallicMap`       | 2D          | "white"   | Mapa de metalicidad                         |
| `_Metallic`          | Float [0,1] | 0         | Metalicidad base                            |
| `_Smoothness`        | Float [0,1] | 0.5       | Suavidad especular                          |
| `_OcclusionMap`      | 2D          | "white"   | Mapa de oclusión ambiental                  |
| `_OcclusionStrength` | Float [0,1] | 1.0       | Intensidad AO                               |
| `_EmissionColor`     | Color [HDR] | (0,0,0,0) | Color de emisión (para highlight)           |
| `_Tiling`            | Vector      | (1,1,0,0) | X=tilingX, Y=tilingY                        |
| `_ClipEdgeColor`     | Color       | Rojo      | Color del borde de corte                    |
| `_ClipEdgeWidth`     | Float       | 0.005     | Grosor del borde de corte en unidades mundo |

### 4.3 Pass 1: UniversalForward

**Vertex Stage:**

```hlsl
Varyings vert(Attributes IN)
{
    OUT.positionCS = TransformObjectToHClip(IN.posOS);      // Clip space
    OUT.positionWS = TransformObjectToWorld(IN.posOS);       // World space (para clipping)
    OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
    OUT.tangentWS  = float4(TransformObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);
    OUT.uv         = IN.uv * _Tiling.xy + _Tiling.zw;      // Tiling + offset
    return OUT;
}
```

**Fragment Stage — Flujo:**

```
fragment(Varyings IN)
├── Clipping global dual (discard si detrás del plano)
├── if (clipEnabled && dist < _ClipEdgeWidth && dist > 0)
│   └── return _ClipEdgeColor  // Borde rojo en el corte
├── Base color = _BaseMap.Sample(uv) * _BaseColor
├── Normal mapping (TBN matrix reconstruction)
│   └── normalTS = UnpackNormalScale(_NormalMap.Sample(uv), _NormalScale)
│   └── normalWS = normalize(tangentToWorld * normalTS)
├── PBR parameters
│   ├── metallic  = _MetallicMap.Sample(uv).r * _Metallic
│   ├── smoothness = _Smoothness
│   └── occlusion = lerp(1, _OcclusionMap.Sample(uv).r, _OcclusionStrength)
├── URP Lighting
│   ├── GetMainLight(shadowCoord) → diffuse + specular
│   ├── GetAdditionalLightsCount() → loop additional lights
│   └── GlobalIllumination (SH + reflection probes)
└── return float4(finalColor + _EmissionColor, 1)
```

**Borde de corte (`_ClipEdgeColor`):** Cuando un fragmento está MUY cerca del plano de corte (dentro de `_ClipEdgeWidth`) pero del lado visible, se colorea de rojo. Esto crea una línea visual en el borde del corte que ayuda al usuario a percibir dónde está el plano.

### 4.4 Pass 2: ShadowCaster

```hlsl
Tags { "LightMode" = "ShadowCaster" }
```

- Solo escribe profundidad (no color).
- Aplica el mismo clipping dual para que las sombras también se corten coherentemente.
- Usa `ApplyShadowBias(positionWS, normalWS)` de URP.

### 4.5 Pass 3: DepthOnly

Escribe solo Z-buffer. URP lo usa para la textura de profundidad (requerida por contacto de sombras, SSAO, etc.).

---

## 5. Shader: Blueprint

**Ruta:** `Assets/Shaders/Blueprint.shader`  
**Líneas:** 225  
**Nombre:** `"WebGL/Blueprint"`  
**Tags:** `"Queue"="Geometry" "RenderType"="Opaque"`

### 5.1 Propósito

Emula un plano de ingeniería/blueprint con grillas superpuestas, efecto Fresnel en los bordes y un pass de outline (contorno).

### 5.2 Properties

| Propiedad       | Tipo  | Default            | Descripción                        |
| --------------- | ----- | ------------------ | ---------------------------------- |
| `_BaseColor`    | Color | (0.1, 0.2, 0.8, 1) | Color base azul                    |
| `_GridColor`    | Color | (0.3, 0.5, 1.0, 1) | Color de la grilla principal       |
| `_GridScale`    | Float | 10                 | Escala de la grilla principal      |
| `_SubGridScale` | Float | 50                 | Escala de la sub-grilla (más fina) |
| `_FresnelPower` | Float | 3                  | Exponente del efecto Fresnel       |
| `_FresnelColor` | Color | (0.5, 0.7, 1.0, 1) | Color del borde Fresnel            |
| `_OutlineWidth` | Float | 0.003              | Grosor del contorno                |
| `_OutlineColor` | Color | (0.2, 0.4, 0.9, 1) | Color del contorno                 |

### 5.3 Pass 1: Main Pass

**Vertex Stage:** Calcula `positionWS`, `normalWS`, `viewDirWS`. Pasa UVs para ambas grillas.

**Fragment Stage:**

```hlsl
// 1. Clipping global dual
ClipAgainstGlobalPlanes(worldPos);

// 2. Fresnel edge glow
float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
float3 fresnelContrib = _FresnelColor.rgb * fresnel;

// 3. Grid principal (world-space, no UV-based)
float2 gridUV = worldPos.xz * _GridScale;       // Proyección XZ
float2 grid = abs(frac(gridUV - 0.5) - 0.5);   // Diente de sierra simétrico
float gridLine = min(grid.x, grid.y);           // Distancia al borde de celda
float gridMask = 1.0 - smoothstep(0.0, 0.05, gridLine);  // Anti-aliased

// 4. Sub-grid (más fina)
float2 subGridUV = worldPos.xz * _SubGridScale;
float2 subGrid = abs(frac(subGridUV - 0.5) - 0.5);
float subGridLine = min(subGrid.x, subGrid.y);
float subGridMask = 1.0 - smoothstep(0.0, 0.02, subGridLine) * 0.5;

// 5. Composición
float3 color = _BaseColor.rgb;
color = lerp(color, _GridColor.rgb, gridMask);
color = lerp(color, _GridColor.rgb * 0.7, subGridMask);
color += fresnelContrib;

return float4(color, 1.0);
```

**Grilla en world-space:** Las líneas de la grilla se calculan con las coordenadas mundo (`worldPos.xz`) en lugar de UVs. Esto significa que las líneas son **continuas** a través de diferentes meshes, evitando discontinuidades en los bordes de mallas adyacentes.

**`smoothstep` para anti-aliasing:** En lugar de un `step()` binario que produciría bordes pixelados, `smoothstep(0, 0.05, gridLine)` crea una transición suave de 5% del espacio de la celda, produciendo líneas anti-aliaseadas incluso sin MSAA.

### 5.4 Pass 2: Outline

```hlsl
Cull Front  // Renderiza solo caras traseras

Varyings vert(Attributes IN)
{
    // Expande el vértice a lo largo de la normal
    float3 expandedPos = IN.posOS + IN.normalOS * _OutlineWidth;
    OUT.positionCS = TransformObjectToHClip(expandedPos);
    return OUT;
}

float4 frag() : SV_Target
{
    return _OutlineColor;
}
```

**Técnica "Inverted Hull":** Renderiza la mesh una segunda vez, pero:

1. Con `Cull Front` (solo caras traseras visibles).
2. Con vértices expandidos a lo largo de sus normales.
3. Con color sólido.

El resultado visual es un contorno alrededor del objeto. Las caras traseras expandidas son visibles solo donde sobresalen del rendering normal (Cull Back) del Pass 1.

---

## 6. Shader: XRay

**Ruta:** `Assets/Shaders/XRay.shader`  
**Líneas:** 210  
**Nombre:** `"WebGL/XRay"`  
**Tags:** `"Queue"="Transparent" "RenderType"="Transparent"`

### 6.1 Propósito

Efecto de rayos X médico: las partes detrás de otras geometrías son visibles con un tinte translúcido, mientras las partes frontales mantienen un color sólido atenuado.

### 6.2 Properties

| Propiedad      | Tipo  | Default              | Descripción                         |
| -------------- | ----- | -------------------- | ----------------------------------- |
| `_BaseColor`   | Color | (0.0, 0.8, 0.4, 0.3) | Color base + alpha de transparencia |
| `_RimColor`    | Color | (0.0, 1.0, 0.5, 1.0) | Color del borde fresnel             |
| `_RimPower`    | Float | 2.5                  | Exponente Fresnel                   |
| `_InsideColor` | Color | (0.0, 0.3, 0.2, 0.1) | Color de las partes "ocultas"       |
| `_InsideAlpha` | Float | 0.15                 | Alpha de las partes detrás          |
| `_Intensity`   | Float | 1.5                  | Multiplicador de brillo             |

### 6.3 Pass 1: Behind Pass (Z-Fail)

```hlsl
ZTest Greater       // Solo fragmentos DETRÁS de la geometría existente
ZWrite Off          // No modifica el Z-buffer
Blend SrcAlpha OneMinusSrcAlpha  // Alpha blending estándar
Cull Back

frag(Varyings IN) : SV_Target
{
    ClipAgainstGlobalPlanes(IN.positionWS);
    float fresnel = pow(1.0 - saturate(dot(IN.normalWS, IN.viewDirWS)), _RimPower);
    float alpha = _InsideAlpha + fresnel * 0.3;
    return float4(_InsideColor.rgb * _Intensity, alpha);
}
```

**`ZTest Greater`:** Este pass SOLO renderiza fragmentos que están **detrás** de geometría ya escrita en el Z-buffer. Esto produce el efecto "semi-transparente" de las partes ocultas.

### 6.4 Pass 2: Front Pass (Z-Pass Normal)

```hlsl
ZTest LEqual        // Depth test normal
ZWrite On           // Escribe profundidad
Blend SrcAlpha OneMinusSrcAlpha
Cull Back

frag(Varyings IN) : SV_Target
{
    ClipAgainstGlobalPlanes(IN.positionWS);
    float fresnel = pow(1.0 - saturate(dot(IN.normalWS, IN.viewDirWS)), _RimPower);
    float3 color = lerp(_BaseColor.rgb, _RimColor.rgb, fresnel);
    float alpha = _BaseColor.a + fresnel * 0.5;
    return float4(color * _Intensity, alpha);
}
```

**Orden de passes importa:** El Pass 1 (Behind) debe ejecutarse ANTES del Pass 2 (Front). Como el Pass 1 tiene `ZWrite Off`, no modifica el Z-buffer. El Pass 2 sí escribe profundidad. Si se invirtiera el orden, el Pass 1 (ZTest Greater) no encontraría ningún fragmento "detrás" porque el Z-buffer aún no contiene la geometría de este objeto.

---

## 7. Shader: SolidColor

**Ruta:** `Assets/Shaders/SolidColor.shader`  
**Líneas:** 281  
**Nombre:** `"WebGL/SolidColor"`  
**Tags:** `"Queue"="Geometry" "RenderType"="Opaque"`

### 7.1 Propósito

Renderizado Blinn-Phong clásico con color sólido por parte + contorno. Cada parte se colorea según su categoría sin texturas, ideal para identificación rápida de componentes.

### 7.2 Properties

| Propiedad          | Tipo  | Default            |
| ------------------ | ----- | ------------------ |
| `_BaseColor`       | Color | (0.8, 0.2, 0.2, 1) |
| `_SpecularColor`   | Color | (1, 1, 1, 1)       |
| `_Shininess`       | Float | 32                 |
| `_AmbientStrength` | Float | 0.3                |
| `_OutlineWidth`    | Float | 0.002              |
| `_OutlineColor`    | Color | (0.1, 0.1, 0.1, 1) |

### 7.3 Pass 1: Blinn-Phong

```hlsl
frag(Varyings IN) : SV_Target
{
    ClipAgainstGlobalPlanes(IN.positionWS);

    float3 N = normalize(IN.normalWS);
    float3 L = normalize(_MainLightPosition.xyz);
    float3 V = normalize(GetWorldSpaceViewDir(IN.positionWS));
    float3 H = normalize(L + V);  // Half-vector (Blinn)

    // Ambient (constante)
    float3 ambient = _AmbientStrength * _BaseColor.rgb;

    // Diffuse (Lambert)
    float NdotL = max(dot(N, L), 0.0);
    float3 diffuse = NdotL * _BaseColor.rgb * _MainLightColor.rgb;

    // Specular (Blinn-Phong)
    float NdotH = max(dot(N, H), 0.0);
    float3 specular = pow(NdotH, _Shininess) * _SpecularColor.rgb;

    return float4(ambient + diffuse + specular, 1.0);
}
```

**¿Por qué iluminación "legacy" Blinn-Phong?** En modo SolidColor, el objetivo es claridad visual, no fotorrealismo. El modelo Blinn-Phong es:

- Más económico que PBR (sin GGX, sin fresnel, sin energía conservada).
- Más predecible: el highlight especular siempre es una esfera nítida.
- Suficiente para distinguir la forma 3D con iluminación simple.

### 7.4 Pass 2: Outline (Inverted Hull)

Idéntico al Pass 2 de Blueprint: `Cull Front`, expansión de vértices por normal, color sólido.

### 7.5 Pass 3: ShadowCaster

Escribe profundidad con clipping dual. Permite que los objetos en modo SolidColor proyecten sombras coherentes.

---

## 8. Shader: Thermal

**Ruta:** `Assets/Shaders/Thermal.shader`  
**Líneas:** 192  
**Nombre:** `"WebGL/Thermal"`  
**Tags:** `"Queue"="Transparent" "RenderType"="Transparent"`

### 10.1 Propósito

Emula una cámara térmica/infrarroja con mapa de calor procedural (sin texturas) y efecto de escaneo por líneas.

### 10.2 Properties

| Propiedad            | Tipo  | Default                        |
| -------------------- | ----- | ------------------------------ | ----------------------------------- |
| `_ColdColor`         | Color | (0, 0, 0.5, 1) — Azul oscuro   |
| `_MidColor`          | Color | (0, 0.8, 0, 1) — Verde         |
| `_HotColor`          | Color | (1, 0.5, 0, 1) — Naranja       |
| `_WhiteHotColor`     | Color | (1, 1, 0.8, 1) — Blanco cálido |
| `_HeatOffset`        | Float | 0                              | Desplazamiento de temperatura base  |
| `_NoiseScale`        | Float | 5                              | Escala del ruido procedural         |
| `_ScanlineIntensity` | Float | 0.1                            | Intensidad de las líneas de escaneo |
| `_ScanlineScale`     | Float | 200                            | Frecuencia de scanlines             |
| `_AnimSpeed`         | Float | 0.5                            | Velocidad de animación del ruido    |

### 10.3 Función Hash Noise

```hlsl
float hash(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * 0.13);
    p3 += dot(p3, p3.yzx + 3.333);
    return frac((p3.x + p3.y) * p3.z);
}

float noise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    f = f * f * (3.0 - 2.0 * f);  // Smoothstep hermite

    float a = hash(i);
    float b = hash(i + float2(1, 0));
    float c = hash(i + float2(0, 1));
    float d = hash(i + float2(1, 1));

    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}
```

**Noise procedural vs textura:** El ruido se genera algorítmicamente (hash-based) en lugar de sampling una textura de ruido. Ventajas:

- Zero VRAM para la textura de ruido.
- Infinita resolución (no pixelea al acercarse).
- Determinístico para las mismas coordenadas.
- Animable con `_Time.y * _AnimSpeed`.

### 10.4 Fragment Stage — Mapa de Calor

```hlsl
frag(Varyings IN) : SV_Target
{
    ClipAgainstGlobalPlanes(IN.positionWS);

    // Heat value basado en posición Y + ruido
    float heat = IN.positionWS.y * 0.5 + 0.5;  // Normalizar Y a 0-1
    heat += noise(IN.positionWS.xz * _NoiseScale + _Time.y * _AnimSpeed) * 0.3;
    heat = saturate(heat + _HeatOffset);

    // Gradient de 4 colores
    float3 color;
    if (heat < 0.33)
        color = lerp(_ColdColor.rgb, _MidColor.rgb, heat / 0.33);
    else if (heat < 0.66)
        color = lerp(_MidColor.rgb, _HotColor.rgb, (heat - 0.33) / 0.33);
    else
        color = lerp(_HotColor.rgb, _WhiteHotColor.rgb, (heat - 0.66) / 0.34);

    // Scanlines
    float scanline = sin(IN.positionCS.y * _ScanlineScale) * 0.5 + 0.5;
    color *= 1.0 - scanline * _ScanlineIntensity;

    return float4(color, 0.95);
}
```

**Gradiente de 4 niveles:** En una cámara térmica real, el gradiente va de azul (frío) → verde (templado) → naranja (caliente) → blanco (muy caliente). La implementación usa 3 `lerp` secuenciales con umbrales en 33% y 66% del rango de calor.

**Scanlines:** `sin(screenY * 200)` crea líneas horizontales que emulan la estética de cámaras analógicas de visión térmica. La intensidad baja (0.1) las hace sutiles.

---

## 9. Shader: AnimatedGradientSkybox

**Ruta:** `Assets/Content/Shaders/Skybox/AnimatedGradientSkybox.shader`  
**Líneas:** 68  
**Nombre:** `"Skybox/AnimatedGradientSkybox"`  
**Tags:** `"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"`

### 12.1 Propósito

Skybox procedural con gradiente radial que "respira" (pulsa suavemente). Usado como fondo en el preset de entorno "Studio".

### 12.2 Particularidad: CG en lugar de HLSL

Este shader usa **`CGPROGRAM`** (lenguaje CG/Cg de NVIDIA) en lugar de `HLSLPROGRAM` (el estándar de URP). Razón probable: es el único shader de tipo Skybox, que se renderiza fuera del pipeline URP normal. Los shaders de skybox en Unity tradicionalmente usan la API legacy (CG), y el shader functiona correctamente al no depender de funciones URP.

### 12.3 Properties

| Propiedad         | Tipo          | Default               |
| ----------------- | ------------- | --------------------- |
| `_TopColor`       | Color         | (0.15, 0.18, 0.25, 1) |
| `_BottomColor`    | Color         | (0.08, 0.10, 0.16, 1) |
| `_CenterColor`    | Color         | (0.20, 0.23, 0.32, 1) |
| `_GradientRadius` | Float [0,2]   | 0.8                   |
| `_PulseSpeed`     | Float         | 0.3                   |
| `_PulseAmount`    | Float [0,0.5] | 0.05                  |

### 12.4 Fragment Stage

```cg
fixed4 frag(v2f i) : SV_Target
{
    // Normalizar coordenadas UV a -1..1 centrado
    float2 uv = i.uv * 2.0 - 1.0;

    // Corrección de aspecto (evita elipse en viewports no cuadrados)
    uv.x *= _ScreenParams.x / _ScreenParams.y;

    // Distancia radial desde el centro
    float dist = length(uv);

    // "Breathing pulse": el radio oscila suavemente
    float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount;
    float radius = _GradientRadius + pulse;

    // Gradiente: centro → borde
    float t = saturate(dist / radius);

    // Interpolación de 3 colores
    fixed4 color;
    if (t < 0.5)
        color = lerp(_CenterColor, _TopColor, t * 2.0);
    else
        color = lerp(_TopColor, _BottomColor, (t - 0.5) * 2.0);

    return color;
}
```

**Breathing pulse:** `sin(_Time.y * 0.3) * 0.05` produce una oscilación del radio del gradiente de ±5% con periodo de ~20 segundos. Es lo suficientemente lento para ser subliminal — el usuario percibe un fondo "vivo" sin ser consciente de la animación.

---

## 10. Restricciones WebGL y Estrategias de Compatibilidad

### 13.1 Limitaciones de WebGL 2.0 (OpenGL ES 3.0)

| Limitación                        | Impacto en el Proyecto                                       | Estrategia de Mitigación                                                         |
| --------------------------------- | ------------------------------------------------------------ | -------------------------------------------------------------------------------- |
| Sin Geometry Shaders              | `Wireframe.shader` no funciona                               | `WireframeWebGL.shader` como alternativa (UV grid + Fresnel)                     |
| Sin Tessellation Shaders          | No se pueden subdividir mallas en GPU                        | Pre-subdivisión en modelo 3D (offline)                                           |
| Sin Compute Shaders               | No se puede usar GPU compute para partículas/simulación      | Cálculos en CPU (corrutinas)                                                     |
| Precision limitada (`mediump`)    | Posibles artefactos en cálculos de iluminación               | HLSL `half` donde sea aceptable, `float` en cálculos críticos (clip planes)      |
| Texturas limitadas (~16 samplers) | ClippableLit usa 4 texturas (albedo/normal/metallic/AO) — OK | Dentro del límite                                                                |
| Sin MRT (Multiple Render Targets) | No deferred rendering                                        | URP Forward es single-pass by design — compatible                                |
| VRAM limitada (~256-512MB típico) | Texturas + meshes deben caber                                | `WebGLOptimizer` reduce mip levels en mobile; `QualityManager` adapta resolución |
| Single-threaded                   | No hay worker threads para carga async real                  | `Resources.LoadAsync` + corrutinas simulan async en main thread                  |

### 13.2 Fallback de Wireframe

El shader `Wireframe.shader` contiene 2 SubShaders:

```hlsl
SubShader  // Requiere geometry shader (#pragma geometry)
{
    // ... geometry shader wireframe ...
}

SubShader  // Fallback para plataformas sin geometry shader
{
    // ... simulated wireframe via fresnel ...
}
```

Unity selecciona automáticamente el primer SubShader compatible. En Desktop (DirectX/OpenGL 4+), usa el SubShader 1 con geometry shader. En WebGL, usa el SubShader 2 (simple Fresnel). Sin embargo, cuando `ViewModeManager` pide el modo Wireframe en WebGL, usa `"WebGL/WireframeWebGL"` directamente para obtener la mejor aproximación posible.

### 13.3 Consideraciones de Rendimiento

| Shader          | Estado de Blending | Costo Relativo GPU | Notas                                |
| --------------- | ------------------ | ------------------ | ------------------------------------ |
| ClippableLit    | Opaque             | Alto               | PBR completo + 4 texturas + clipping |
| SolidColor      | Opaque             | Bajo               | Blinn-Phong sin texturas             |
| Blueprint       | Opaque             | Medio              | Fresnel + 2 grids + outline pass     |
| Wireframe/WebGL | Opaque             | Bajo               | Sin texturas, solo matemática        |
| XRay            | Transparent        | Medio-Alto         | 2 passes (behind + front)            |
| Ghosted         | Transparent        | Medio              | Fresnel + depth texture read         |
| Thermal         | Transparent        | Medio              | Procedural noise (ALU-bound)         |
| Skybox          | Background         | Mínimo             | 1 pass, sin texturas, math trivial   |

---

## 11. MaterialPropertyBlock: Uso y Justificación

### 14.1 ¿Qué es MaterialPropertyBlock?

`MaterialPropertyBlock` es una estructura de Unity que permite **override de propiedades de material per-renderer** sin crear una instancia de material. Es análogo a CSS inline styles: sobrescribe propiedades específicas sin duplicar la hoja de estilos.

### 14.2 Uso en la Aplicación

| Sistema                                    | Propiedad Overrideada          | Propósito                                      |
| ------------------------------------------ | ------------------------------ | ---------------------------------------------- |
| `HighlightSystem.OnHoverEnter`             | `_BaseColor`                   | Tinte de hover (amarillo translúcido)          |
| `HighlightSystem.PulseRoutine`             | `_BaseColor`, `_EmissionColor` | Pulsación de emisión al seleccionar            |
| `PartVisibilityManager.FadeOut/In`         | `_BaseColor` (alpha)           | Animación de desvanecimiento                   |
| `ViewModeManager.ApplyCategoryColor`       | `_BaseColor`                   | Colores por categoría en modos no-texturizados |
| `DroneStateController.SetStatusLightColor` | `_BaseColor`                   | Color de LEDs de estado                        |

### 14.3 ¿Por qué no `renderer.material` (instancia)?

```csharp
// INCORRECTO para esta aplicación:
renderer.material.color = Color.yellow;
// Esto CLONA el material implícitamente → leak de memoria
// 50 partes × hover frecuente = cientos de materiales huérfanos

// CORRECTO:
MaterialPropertyBlock block = new MaterialPropertyBlock();
block.SetColor("_BaseColor", Color.yellow);
renderer.SetPropertyBlock(block);
// Zero allocaciones de material. La instancia de block se reutiliza.
```

### 14.4 Limitación

`MaterialPropertyBlock` NO puede cambiar el shader. Solo modifica propiedades del shader actual. Para cambiar de shader (e.g., Realistic → Blueprint), se requiere `renderer.sharedMaterial = newMaterial`.

---

## 12. Inventario de Propiedades Shader Globales

| Propiedad             | Tipo    | Establecida por                          | Leída por                                                                             |
| --------------------- | ------- | ---------------------------------------- | ------------------------------------------------------------------------------------- |
| `_GlobalClipPlane`    | Vector4 | `CrossSectionManager.UpdateClipPlanes()` | Todos los 9 shaders                                                                   |
| `_GlobalClipEnabled`  | Float   | `CrossSectionManager.UpdateClipPlanes()` | Todos los 9 shaders                                                                   |
| `_GlobalClipPlane2`   | Vector4 | `CrossSectionManager.UpdateClipPlanes()` | Todos los 9 shaders                                                                   |
| `_GlobalClipEnabled2` | Float   | `CrossSectionManager.UpdateClipPlanes()` | Todos los 9 shaders                                                                   |
| `_MainLightPosition`  | Vector4 | URP (automático)                         | SolidColor (Blinn-Phong)                                                              |
| `_MainLightColor`     | Half4   | URP (automático)                         | SolidColor                                                                            |
| `_ScreenParams`       | Vector4 | Unity (automático)                       | Wireframe (screen-space edges), Skybox (aspect ratio)                                 |
| `_Time`               | Vector4 | Unity (automático)                       | Thermal (noise animation), Skybox (breathing pulse), HighlightSystem (pulse emission) |

---

## Diagrama Resumen: Shader ↔ Manager ↔ Feature

```
                    ┌─────────────────────────────────┐
                    │      ViewModeManager            │
                    │  SetMode(ViewMode) →             │
                    │  Shader.Find("WebGL/[Mode]")     │
                    │  renderer.sharedMaterial = mat   │
                    └───────┬──────┬──────┬───────────┘
                            │      │      │
              ┌─────────────┴──┐   │   ┌──┴─────────────┐
              │                │   │   │                 │
    ┌─────────▼─────────┐ ┌───▼───▼───▼──┐  ┌──────────▼──────────┐
    │  Modo Realistic   │ │  6 Modos     │  │  CrossSectionMgr    │
    │  URP/Lit (PBR)    │ │  Personalizad│  │  Shader.SetGlobal   │
    │  Sin clipping     │ │  Con clipping │  │  _GlobalClipPlane   │
    │  nativo           │ │  nativo       │  │  ─────────────────  │
    │  ──────────────── │ └──────────────┘  │  Si modo=Realistic: │
    │  Si clip activo:  │                   │  SwapToClippableLit  │
    │  → swap a         │                   │  Si modo=otro:       │
    │  ClippableLit     │                   │  ya soporta clip    │
    └───────────────────┘                   └─────────────────────┘
                    ┌─────────────────────────────────┐
                    │     MaterialPropertyBlock       │
                    │  (per-renderer overrides)        │
                    │                                  │
                    │  HighlightSystem → _BaseColor    │
                    │  HighlightSystem → _EmissionColor│
                    │  PartVisibilityMgr → alpha       │
                    │  ViewModeManager → category color│
                    └─────────────────────────────────┘
```

---

_Documento generado como parte de la documentación técnica del proyecto de grado._  
_Rama: `feature/phase2-ux-redesign` — Febrero 2026_
