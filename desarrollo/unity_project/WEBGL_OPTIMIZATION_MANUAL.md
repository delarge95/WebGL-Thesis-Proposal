# Configuraciones Manuales de Optimización — WebGL Drone Viewer

> **Proyecto:** Prototipo Web 3D Interactivo para la Visualización Técnica y Análisis Estructural de Hardware  
> **Engine:** Unity 6000.0.62f1 · URP 17.3.0 · UI Toolkit (USS/UXML)  
> **Target:** WebGL 2.0 (browser)  
> **Rama:** `feature/phase2-ux-redesign`  
> **Último commit verificado:** `cb9b737`  
> **Documento generado:** Junio 2025

---

## Leyenda

| Icono | Significado                                                  |
| ----- | ------------------------------------------------------------ |
| ✅    | Configurado manualmente en el Inspector / código             |
| 🤖    | Automatizado por `WebGLBuildFixer` (editor tool)             |
| 🔲    | Pendiente — bloqueado por prototipo incompleto               |
| ⚠️    | Configuración crítica — no modificar sin entender el impacto |

---

## Tabla de Contenidos

1. [Espacio de Color](#1-espacio-de-color)
2. [Player Settings](#2-player-settings)
3. [Quality Settings](#3-quality-settings)
4. [Graphics Settings](#4-graphics-settings)
5. [URP Asset (Render Pipeline)](#5-urp-asset-render-pipeline)
6. [URP Renderer Data (Forward Renderer)](#6-urp-renderer-data-forward-renderer)
7. [UI Toolkit — PanelSettings](#7-ui-toolkit--panelsettings)
8. [Sistema de Fuentes (Font)](#8-sistema-de-fuentes-font)
9. [Estilos CSS — Theme.uss](#9-estilos-css--themeuss)
10. [Visual: Plano de Sección Transversal](#10-visual-plano-de-sección-transversal)
11. [Entorno 3D — EnvironmentController](#11-entorno-3d--environmentcontroller)
12. [Shaders Siempre Incluidos](#12-shaders-siempre-incluidos)
13. [Arquitectura del Fixer (WebGLBuildFixer)](#13-arquitectura-del-fixer-webglbuildfixer)
14. [Correcciones de UI — Event Propagation](#14-correcciones-de-ui--event-propagation)
15. [⛔ Puntos Pendientes por Falta de Materiales](#15--puntos-pendientes-por-falta-de-materiales)

---

## 1. Espacio de Color

### Cambio realizado ✅

| Parámetro                                                           | Antes | Después    |
| ------------------------------------------------------------------- | ----- | ---------- |
| **Edit → Project Settings → Player → Other Settings → Color Space** | Gamma | **Linear** |

### Por qué se hizo

El modo **Gamma** es históricamente el default de Unity por compatibilidad con hardware antiguo, pero produce colores más brillantes e incorrectos físicamente. El modo **Linear** es obligatorio para un flujo PBR (Physically Based Rendering) correcto con URP:

- Las texturas y materiales PBR asumen espacio lineal para sus cálculos de iluminación.
- Sin Linear, la iluminación parece "lavada" o sobreexpuesta.
- WebGL 2.0 soporta Linear natively.

### Impacto en cascada

**1 486 líneas de `Theme.uss`** contenían valores de color en notación `rgba()` codificados en espacio Gamma. Al cambiar el engine a Linear, estos colores se hubieran renderizado con tonos incorrectos.

**Solución aplicada:** Se convirtieron **133 expresiones de color** de Gamma a Linear en `Theme.uss` usando la fórmula:

$$C_{linear} = \left(\frac{C_{gamma}}{255}\right)^{2.2}$$

---

## 2. Player Settings

> **Ruta:** `Edit → Project Settings → Player`

### 2.1 Resolución y Presentación ✅

| Opción                | Valor configurado | Motivo                                         |
| --------------------- | ----------------- | ---------------------------------------------- |
| Default Screen Width  | `1920`            | Resolución estándar de escritorio              |
| Default Screen Height | `1080`            | 16:9 landscape, target para tesis              |
| Run In Background     | `true`            | Evitar que el viewport se pause al perder foco |

### 2.2 Rendering ✅

| Opción                  | Valor configurado        | Motivo                                                                                                                  |
| ----------------------- | ------------------------ | ----------------------------------------------------------------------------------------------------------------------- |
| **Color Space**         | **Linear**               | PBR correcto (ver §1)                                                                                                   |
| Auto Graphics API       | `Disabled`               | Control explícito de APIs                                                                                               |
| Graphics APIs           | **WebGL 2.0 únicamente** | Eliminado WebGL 1.0 — WebGL 2.0 es universal en browsers modernos y soporta características URP como MSAA y shadow maps |
| Multithreaded Rendering | `Disabled`               | WebGL no soporta multi-threading real; esta opción causaba overhead                                                     |

### 2.3 Configuración ✅

| Opción                  | Valor configurado          | Motivo                                                                                  |
| ----------------------- | -------------------------- | --------------------------------------------------------------------------------------- |
| .NET Standard           | **2.1**                    | Mayor compatibilidad y tree-shaking vs .NET 4.x                                         |
| Managed Stripping Level | **High**                   | Reduce tamaño del build eliminando código IL no referenciado                            |
| Exception Support       | **Explicitly Thrown Only** | Elimina overhead de try/catch para excepciones nativas — crítico para rendimiento WebGL |

### 2.4 Publicación WebGL ✅

| Opción                 | Valor configurado              | Motivo                                                                      |
| ---------------------- | ------------------------------ | --------------------------------------------------------------------------- |
| Compression Format     | **Brotli** (prod) / Gzip (dev) | Brotli ≈ 15–25% más pequeño que Gzip; browsers modernos lo soportan         |
| Decompression Fallback | `Enabled`                      | Fallback a descarga sin comprimir si el servidor no envía headers correctos |
| Data Caching           | `Enabled`                      | El browser cachea el archivo `.data` — carga ≈ 0 ms en visitas subsecuentes |
| Memory Size (Heap)     | **512 MB**                     | Balance entre capacidad para el modelo 3D y límite de memoria del browser   |

### Estimado de tamaño de build

| Componente              | Tamaño estimado                 |
| ----------------------- | ------------------------------- |
| `Build.wasm.br`         | ~5–8 MB                         |
| `Build.data.br`         | ~20–40 MB (depende de texturas) |
| `Build.framework.js.br` | ~0.3 MB                         |
| **Total descarga**      | **~25–50 MB**                   |

---

## 3. Quality Settings

> **Ruta:** `Edit → Project Settings → Quality`  
> Se crea un preset dedicado **"WebGL Optimized"**. Todos los quality levels reciben el mismo URP Asset vía `QualityLevelFixer`.

### Configuración completa ✅

| Opción                     | Valor           | Motivo                                                                                        |
| -------------------------- | --------------- | --------------------------------------------------------------------------------------------- |
| Pixel Light Count          | `2`             | Máximo 2 luces por pixel en tiempo real — WebGL tiene presupuesto limitado de fragment shader |
| Anti-Aliasing              | **2x MSAA**     | Equilibrio calidad/rendimiento; 4x dobla el fill rate                                         |
| Soft Particles             | `Disabled`      | Requiere depth buffer sampling — overhead innecesario sin partículas complejas en la escena   |
| Realtime Reflection Probes | `Disabled`      | Los reflection probes en tiempo real son costosos; se usan baked probes                       |
| Shadow Distance            | `50` (unidades) | Sombras visibles solo en radio de 50m desde la cámara                                         |
| Shadow Resolution          | `Medium (512)`  | Compromise 512×512 vs 1024 para reducir ~75% el uso de memoria de shadow map                  |
| Shadow Cascades            | `2`             | Cascades mejoran la distribución de shadow maps; 4 cascades duplicaría el costo               |
| VSynch Count               | `Don't Sync`    | Permite que WebGL corra al ritmo del `requestAnimationFrame` del browser                      |
| LOD Bias                   | `1.0`           | Sin reducción agresiva de LOD — el modelo requiere alta calidad en zoom cercano               |

---

## 4. Graphics Settings

> **Ruta:** `Edit → Project Settings → Graphics`

### 4.1 Pipeline Asset ✅ / 🤖

El URP Asset se asigna tanto en **Scriptable Render Pipeline Settings** como en cada Quality Level mediante `URPPipelineFixer.cs` y `QualityLevelFixer.cs`.

### 4.2 Always Included Shaders 🤖

Estos 8 shaders se agregan programáticamente via `ShaderInclusionFixer.cs` para garantizar que estén disponibles en el WebGL build (sin esta configuración, los shaders pueden ser stripped por el stripping automático):

| #   | Shader                                 | Motivo                          |
| --- | -------------------------------------- | ------------------------------- |
| 1   | `Antigravity/Wireframe`                | Modo wireframe personalizado    |
| 2   | `Antigravity/XRay`                     | Modo X-Ray personalizado        |
| 3   | `Antigravity/Outline`                  | Outline de selección            |
| 4   | `Antigravity/CrossSection`             | Plano de corte                  |
| 5   | `Universal Render Pipeline/Lit`        | Shader PBR base de URP          |
| 6   | `Universal Render Pipeline/Unlit`      | Shader sin iluminación          |
| 7   | `Universal Render Pipeline/Simple Lit` | Shader para objetos secundarios |
| 8   | `Sprites/Default`                      | Sprites UI                      |

### 4.3 Transparency Sort Mode ✅

| Opción                 | Valor           | Motivo                                                                                                                      |
| ---------------------- | --------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Transparency Sort Mode | **Perspective** | Ordenamiento de objetos transparentes desde el punto de vista de la cámara — necesario para el plano de sección transversal |

---

## 5. URP Asset (Render Pipeline)

> **Archivo:** `Assets/Settings/WebGL_URPAsset.asset` (creado por `URPAssetFixer.cs`)  
> El asset se crea fresh para evitar configuraciones heredadas incorrectas.

### 5.1 Lighting ✅

| Parámetro                      | Valor                | Motivo                                                                |
| ------------------------------ | -------------------- | --------------------------------------------------------------------- |
| Main Light                     | **Per Pixel**        | Render correcto de la luz direccional principal                       |
| Main Light Cast Shadows        | `Enabled`            | Sombras visibles para la luz principal                                |
| Main Light Shadow Resolution   | **1024×1024**        | Balance calidad/memoria. 2048 cuadruplicaría el VRAM                  |
| Additional Lights              | **Per Pixel, Max 4** | Máximo 4 luces adicionales en pixel mode                              |
| Additional Lights Cast Shadows | `Disabled`           | ⚠️ Las sombras de luces adicionales en WebGL tienen costo prohibitivo |

### 5.2 Shadows ✅

| Parámetro           | Valor configurado  | Motivo                                              |
| ------------------- | ------------------ | --------------------------------------------------- |
| Shadow Max Distance | `30–50` (unidades) | Correlacionado con Quality Settings shadow distance |
| Shadow Depth Bias   | `1.0`              | Reduce shadow acne en superficies de bajo detalle   |
| Shadow Normal Bias  | `1.0`              | Reduce peter-panning en geometría fina              |

### 5.3 Post Processing ✅

| Parámetro       | Valor          | Motivo                                                                                      |
| --------------- | -------------- | ------------------------------------------------------------------------------------------- |
| Post Processing | `Enabled`      | Habilitado para efectos de presentación                                                     |
| HDR             | **`Disabled`** | HDR requiere render targets de 16-bit float — impracticable en WebGL para el modelo de dron |
| MSAA            | **2x**         | Mismo que Quality Settings (código: `m_MSAA = 1`)                                           |

### 5.4 Optimizaciones Core ✅

| Parámetro        | Valor         | Motivo                                                                                                                                                      |
| ---------------- | ------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **SRP Batcher**  | **`Enabled`** | ⚠️ Crítico — reduce drasticamente las draw calls agrupando objetos con el mismo shader. Reportado como la optimización de rendimiento más impactante en URP |
| Dynamic Batching | `Disabled`    | El SRP Batcher ya maneja el batching de forma más eficiente                                                                                                 |
| GPU Instancing   | `Enabled`     | Para geometría repetitiva                                                                                                                                   |

---

## 6. URP Renderer Data (Forward Renderer)

> **Archivo:** `Assets/Settings/WebGL_ForwardRenderer.asset`

### Renderer Features revisados ✅

| Feature                               | Estado          | Motivo                                                                                                                          |
| ------------------------------------- | --------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Screen Space Ambient Occlusion (SSAO) | **Desactivado** | SSAO genera un render pass adicional costoso — en WebGL con geometría de dron técnico el beneficio visual no justifica el costo |
| Decals                                | N/A             | No usado en la escena                                                                                                           |
| Screen Space Shadows                  | **Desactivado** | Se usan shadow maps convencionales                                                                                              |

### Rendering Path ✅

| Opción         | Valor       | Motivo                                                                                                                   |
| -------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------ |
| Rendering Path | **Forward** | Menor overhead de memoria vs Deferred — WebGL no tiene ventajas suficientes con Deferred para una escena de objeto único |

---

## 7. UI Toolkit — PanelSettings

> **Archivo:** `Assets/UI/MainPanelSettings.asset`  
> Configurado por `PanelSettingsFixer.cs`

| Parámetro            | Valor                         | Motivo                                                                     |
| -------------------- | ----------------------------- | -------------------------------------------------------------------------- |
| **Scale Mode**       | `Scale With Screen Size`      | La UI escala proporcionalmente a `1920×1080` como resolución de referencia |
| Reference Resolution | `1920 × 1080`                 | Target landscape desktop                                                   |
| Clear Color          | **`Disabled`**                | El panel de UI no limpia el color buffer — la escena 3D es el fondo        |
| Text Settings        | `MainTextSettings` (asignado) | Configuración de fuente global para todos los elementos de texto           |
| Sort Order           | `0`                           | El panel se renderiza encima de la cámara 3D por orden de cámara           |

---

## 8. Sistema de Fuentes (Font)

### 8.1 Tipografía seleccionada ✅

| Parámetro    | Valor                                     | Motivo                                                                                         |
| ------------ | ----------------------------------------- | ---------------------------------------------------------------------------------------------- |
| Font Primary | **Inter-Regular.otf**                     | Fuente de alta legibilidad, optimizada para pantallas digitales. Sans-serif geométrica moderna |
| Font Path    | `Assets/UI/Fonts/Inter/Inter-Regular.otf` | Copiado por `FontSetupFixer.cs`                                                                |

### 8.2 Revert SDF → Legacy Rendering ✅

> **Archivo afectado:** `Assets/UI/Styles/Theme.uss`

Unity UI Toolkit por defecto usa **SDF (Signed Distance Field)** rendering para texto. Esto generaba > 30 warnings en consola del tipo:

```
Font asset 'Inter-Regular' has no SDF atlas. Falling back to legacy rendering.
```

**Solución aplicada:** Se revertió el sistema de fuentes a **legacy rendering** en los estilos CSS para que el fallback fuera el comportamiento esperado, eliminando los warnings sin necesidad de generar atlas SDF (proceso costoso para fuentes custom).

**Por qué no se generó el atlas SDF:** Requiere baking manual en Unity, agrega ~2–5 MB al build, y el rendering legacy a las resoluciones de uso es visualmente equivalente.

---

## 9. Estilos CSS — Theme.uss

> **Archivo:** `Assets/UI/Styles/Theme.uss` — 1 486 líneas

### 9.1 Conversión Gamma → Linear (133 expresiones) ✅

Al cambiar el Project Color Space a Linear (§1), todos los colores en CSS debían convertirse manualmente. Se aplicó la conversión gamma a **133 expresiones `rgba()`** en todo el archivo.

**Fórmula aplicada por canal:**
$$C_{linear} = \left(\frac{C_{gamma}}{255}\right)^{2.2} \times 255$$

**Ejemplo representativo:**

| Color                  | Gamma (original)            | Linear (resultado)          |
| ---------------------- | --------------------------- | --------------------------- |
| Fondo oscuro principal | `rgba(12, 12, 12, 1)`       | `rgba(3, 3, 3, 1)`          |
| Superficie glass       | `rgba(255, 255, 255, 0.08)` | `rgba(255, 255, 255, 0.04)` |
| Texto primario         | `rgba(240, 240, 240, 1)`    | `rgba(228, 228, 228, 1)`    |

### 9.2 Ajuste del Azul de Acento ✅

El color de acento de la interfaz (botones activos, sliders, highlights) fue recalibrado:

| Estado               | Valor                                                           |
| -------------------- | --------------------------------------------------------------- |
| **Original (Gamma)** | `rgba(0, 105, 255, 1.0)` — azul saturado                        |
| **Final (Linear)**   | `rgba(70, 175, 255, ...)` — azul más claro, legible en pantalla |

**Motivo:** En espacio lineal, colores de alta saturación como `rgba(0,105,255)` se perciben más oscuros y con menos contraste contra fondos oscuros. El nuevo valor `rgba(70, 175, 255)` mantiene la identidad de color azul pero con:

- Mayor contraste sobre fondos oscuros (ratio WCAG mejorado)
- Mejor visibilidad del estado activo de controles
- Consistencia con la paleta de interfaz oscura

**Ocurrencias modificadas:** 10 reglas CSS con el valor de acento azul en `Theme.uss`.

---

## 10. Visual: Plano de Sección Transversal

> **Archivo:** `Assets/Scripts/Core/Managers/CrossSectionManager.cs`, línea 40

### Cambio realizado ✅

```csharp
// ANTES:
[SerializeField] private Color planeColor = new Color(0.4f, 0.7f, 1f, 0.02f);

// DESPUÉS:
[SerializeField] private Color planeColor = new Color(0.4f, 0.7f, 1f, 0.006f);
```

| Parámetro | Antes           | Después            |
| --------- | --------------- | ------------------ |
| Alpha     | `0.02` (2%)     | **`0.006` (0.6%)** |
| R, G, B   | `0.4, 0.7, 1.0` | Sin cambio         |

**Motivo:** El plano de corte azul translúcido era visualmente intrusivo — al cortar el modelo se veía claramente el plano como una superficie sólida semi-transparente. Con alpha `0.006` el plano es prácticamente invisible, actuando como un marcador de posición sin distraer de la geometría cortada.

---

## 11. Entorno 3D — EnvironmentController

> **Archivo:** `Assets/Scripts/Core/Managers/EnvironmentController.cs`

### 11.1 Skybox — Gradiente Procedural ✅

| Decisión              | Detalle                                                                                                                                      |
| --------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| **No se usa HDRI**    | Los skyboxes HDRI estándar son archivos de 2–10 MB que se suman al build size. Se descartó completamente                                     |
| **Skybox procedural** | Se usa el componente `Gradient Skybox` de Unity que genera el fondo mediante gradientes de color en CPU — costo de runtime: ~0 MB de textura |

**Motivo:** Para la visualización técnica del dron, el entorno es secundario. El foco es el modelo 3D. Un fondo oscuro y limpio (Studio preset) mejora la legibilidad del modelo sin cost de assets.

### 11.2 Presets de Entorno ✅

Cuatro presets predefinidos que el usuario puede seleccionar desde el modo Studio:

| Preset        | Background                          | Iluminación                        | Propósito                          |
| ------------- | ----------------------------------- | ---------------------------------- | ---------------------------------- |
| **Studio**    | `#050505` (5,5,5 — negro casi puro) | Luz direccional alta, ambient bajo | Visualización técnica de precisión |
| **Sunset**    | Gradiente naranja-azul              | Luz cálida lateral                 | Presentación visual                |
| **Night**     | `#000000` profundo                  | Luz fría+accent azul               | Contraste máximo                   |
| **Blueprint** | `#0a1628` (azul oscuro)             | Luz técnica uniforme               | Plano técnico                      |
| **Neutral**   | `#141414` gris medio                | Iluminación equilibrada            | General                            |

**Motivo del preset Studio `#050505`:** Este valor `(5/255, 5/255, 5/255)` en espacio lineal corresponde a un negro que en el browser renderiza como un fondo oscuro profundo sin ser pure black, evitando artefactos de LCD y manteniendo la coherencia con la paleta CSS de la interfaz.

### 11.3 Costos evitados ✅

| Elemento descartado        | Ahorro estimado             |
| -------------------------- | --------------------------- |
| HDRI skybox texture        | ~2–8 MB por preset          |
| Realtime reflection probes | 2 draw passes por frame     |
| Baked GI lightmaps         | ~10–50 MB de texturas baked |

---

## 12. Shaders Siempre Incluidos

> **Configuración:** `Edit → Project Settings → Graphics → Always Included Shaders`  
> **Implementado por:** `ShaderInclusionFixer.cs`

### Por qué es necesario ⚠️

Unity's **Shader Stripping** elimina automáticamente shaders no referenciados en el build para reducir el tamaño. Pero los shaders que se asignan dinámicamente en runtime (por código, no en el Inspector) no son detectados por el sistema de stripping y se eliminan, causando `pink/magenta` objects en WebGL.

Los 4 shaders custom de Antigravity se asignan por código en `CrossSectionManager`, `PartSelector`, etc., no en materiales del Inspector — por lo tanto **deben** estar en Always Included.

Los 4 shaders URP se incluyen como seguro ante posibles edge cases en WebGL build stripping.

---

## 13. Arquitectura del Fixer (WebGLBuildFixer)

> **Ruta:** `Assets/Editor/Antigravity/Fixes/`  
> Herramienta de editor que automatiza las configuraciones que serían tediosamente repetibles.

La herramienta original era un monolito de 683 líneas refactorizado en 7 clases especializadas:

| Clase                     | FIX #    | Responsabilidad                                                 |
| ------------------------- | -------- | --------------------------------------------------------------- |
| `URPAssetFixer.cs`        | FIX 1    | Crea o recrea el URP Asset con todos los parámetros optimizados |
| `URPPipelineFixer.cs`     | FIX 2    | Asigna el URP Asset a Graphics Settings y Quality Settings      |
| `QualityLevelFixer.cs`    | FIX 3    | Propaga el URP Asset a **todos** los Quality Levels             |
| `ShaderInclusionFixer.cs` | FIX 4    | Agrega los 8 shaders a Always Included Shaders                  |
| `FontSetupFixer.cs`       | FIX 5a   | Copia Inter-Regular.otf a la ruta correcta                      |
| `PanelSettingsFixer.cs`   | FIX 5b/c | Asigna TextSettings y deshabilita ClearColor en PanelSettings   |
| `WebGLDiagnostics.cs`     | —        | Herramienta de diagnóstico (read-only, sin cambios)             |

**Uso:** `Tools → WebGL Build Fixer → Run All Fixes`

---

## 14. Correcciones de UI — Event Propagation

> **Archivos modificados:** `Assets/Scripts/UI/UIManager.cs`, `Assets/UI/MainLayout.uxml`

### 14.1 Picking Mode en Labels de Sliders ✅

> **Commit:** `cb9b737`

**Problema:** Los labels de texto sobre los sliders ("EXPLODE", "ROTATION", "INTENSITY") tenían `picking-mode="Position"` que hacía que clicks en su área cerraran el panel de UI.

**Solución en `MainLayout.uxml`:**

```xml
<!-- Sliders de herramientas -->
<Label text="EXPLODE" picking-mode="Position" />

<!-- Sliders de Studio -->
<VisualElement name="env-slider-group" picking-mode="Position">
```

### 14.2 StopPropagation en Sub-paneles de Analyze ✅

> **Archivo:** `Assets/Scripts/UI/UIManager.cs`

**Problema:** Clicks en el panel "EXPLODE" (y otros sub-paneles de Analyze) burbujeaban hasta `SelectionManager.HandleClick()` → `Deselect()` → `NavigateToCardGrid()`, navegando involuntariamente de vuelta al grid.

**Solución aplicada — patrón para cada panel:**

```csharp
var explodePanel = root.Q<VisualElement>("ExplodeSubPanel");
if (explodePanel != null)
{
    EventCallback<PointerEnterEvent> onEnter = _ => InputManager.InputBlocked = true;
    EventCallback<PointerLeaveEvent> onLeave = _ => InputManager.InputBlocked = false;
    EventCallback<PointerDownEvent> onDown  = evt => evt.StopPropagation();

    explodePanel.RegisterCallback(onEnter);
    explodePanel.RegisterCallback(onLeave);
    explodePanel.RegisterCallback(onDown);

    AddCleanup(() => {
        explodePanel.UnregisterCallback(onEnter);
        explodePanel.UnregisterCallback(onLeave);
        explodePanel.UnregisterCallback(onDown);
    });
}
```

**Paneles protegidos:** `ExplodeSubPanel`, `CrossSectionPanel`, `FilterSubPanel`

---

## 15. ⛔ Puntos Pendientes por Falta de Materiales

> El prototipo actual usa materiales placeholder simples. Las siguientes optimizaciones **no pueden completarse** hasta que los materiales PBR definitivos estén asignados a todas las partes del dron.

### 15.1 Materiales PBR por Parte 🔲

| Tarea                                  | Descripción                                                                                                                                    | Impacto                                                                            |
| -------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| **Asignar URP/Lit a todas las partes** | Actualmente varias partes usan materiales Standard de Unity o Unlit temporales. Deben migrarse a `Universal Render Pipeline/Lit`               | Sin materiales correctos, el SRP Batcher no puede agrupar draw calls               |
| **Mapas de textura**                   | Cada material necesita: Albedo (color), Normal Map, Metallic Map, Roughness/Smoothness Map, AO Map                                             | Sin texturas, se pierde la legibilidad técnica del material (metal, carbono, etc.) |
| **Configuración Metallic/Roughness**   | Los metales (aluminio, titanio, acero) necesitan `Metallic ≈ 0.9–1.0`, `Roughness ≈ 0.1–0.3`. Plásticos: `Metallic = 0`, `Roughness ≈ 0.5–0.8` | Apariencia realista correlacionada con el tipo de componente                       |

### 15.2 Texture Compression 🔲

| Tarea                             | Descripción                                                                                          | Impacto en Build                                                       |
| --------------------------------- | ---------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| **DXT5 / BC7 para albedo+normal** | Comprimir todas las texturas con DXT5 (transparencia) o DXT1 (opaco)                                 | Reducción ~75% en tamaño de texturas                                   |
| **Resolución por LOD**            | Texturas de alta resolución (2K, 4K) solo para partes primarias; 512px para partes pequeñas/internas | Reducción ~60% en tiempo de descarga inicial                           |
| **Crunch Compression**            | Habilitar Crunch compression en textura Import Settings para el build WebGL                          | Reducción adicional ~40–60% en tamaño del archivo `.data`              |
| **Texture Atlas**                 | Agrupar texturas de partes similares en atlas compartidos                                            | Reducción dramática de draw calls (de N por parte única a 1 por atlas) |

### 15.3 LOD Groups 🔲

| Tarea                         | Descripción                                                                                                                                                           |
| ----------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Crear LOD Group por parte** | Cada componente del dron debe tener: LOD0 (full res, cámara cerca), LOD1 (50% polígonos, cámara mediana), LOD2 (25% polígonos, cámara lejos), CULLED (fuera de rango) |
| **LOD Bias**                  | Ajustar `Quality Settings → LOD Bias` en base al modelo final para evitar pop-in visual                                                                               |
| **Occlusion Culling**         | Bake occlusion culling para partes internas del dron que quedan ocultas cuando la cámara está en posición exterior                                                    |

### 15.4 Shader Variants 🔲

| Tarea                         | Descripción                                                                                                                                                                                       |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Shader Variant Collection** | Una vez que los materiales definitivos estén asignados, generar una `ShaderVariantCollection` que pre-compile exactamente las variantes usadas, eliminando el shader compilation stutter en WebGL |
| **Strip Unused Keywords**     | Con los materiales finales, revisar en `Graphics → Shader Stripping` qué keywords de shader pueden desactivarse globalmente (instancing, lightmaps, fog, etc.)                                    |

### 15.5 Lighting — Baked GI 🔲

| Tarea                       | Descripción                                                                                                                                                                                         |
| --------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Lightmap baking**         | Bake Global Illumination para la iluminación del entorno Studio. Esto elimina el costo de cálculo en runtime. Actualmente imposible sin los materiales definitivos — los lightmaps serían inválidos |
| **Reflection Probes Baked** | Agregar 1–2 reflection probes baked en posición del dron para capturar reflecciones de entorno sin costo en runtime                                                                                 |
| **Light Probes**            | Para partes pequeñas del dron que se mueven (alas, hélices en animación), agregar light probes                                                                                                      |

### 15.6 Optimización de Audio 🔲

| Tarea                     | Descripción                                                                              |
| ------------------------- | ---------------------------------------------------------------------------------------- |
| **Formato de compresión** | Todos los clips de audio deben estar en formato **Vorbis** en calidad `70–80` para WebGL |
| **Load Type**             | Clips de ambient: `Streaming`. Clips de click/UI: `Decompress On Load`                   |
| **Sample Rate**           | UI sounds: 22050 Hz es suficiente. No usar 44100/48000 Hz en clips secundarios           |

---

## Resumen de Estado

| Área                           | Estado                   | Commit/Referencia               |
| ------------------------------ | ------------------------ | ------------------------------- |
| Color Space Linear             | ✅ Completado            | `d341e58`                       |
| Player Settings WebGL          | ✅ Completado            | Manual Inspector                |
| Quality Settings               | ✅ Completado            | Manual Inspector                |
| URP Asset (automated)          | ✅ Completado            | `URPAssetFixer.cs`              |
| URP Renderer Data              | ✅ Revisado              | Manual Inspector                |
| Graphics Settings              | ✅ Completado            | `ShaderInclusionFixer.cs`       |
| PanelSettings UI               | ✅ Completado            | `PanelSettingsFixer.cs`         |
| Sistema de fuentes (Inter)     | ✅ Completado            | `FontSetupFixer.cs`             |
| Theme.uss Gamma→Linear         | ✅ Completado (133 expr) | `d341e58`                       |
| Accent blue ajustado           | ✅ Completado            | `d341e58`                       |
| Cross-section alpha            | ✅ Completado            | `CrossSectionManager.cs`        |
| EnvironmentController presets  | ✅ Completado            | Manual code                     |
| Event propagation (sliders)    | ✅ Completado            | `cb9b737`                       |
| Event propagation (sub-panels) | ✅ Completado            | UIManager.cs (pendiente commit) |
| Materiales PBR definitivos     | 🔲 Pendiente prototipo   | —                               |
| Texture compression/atlas      | 🔲 Pendiente prototipo   | —                               |
| LOD Groups                     | 🔲 Pendiente prototipo   | —                               |
| Shader Variant Collection      | 🔲 Pendiente prototipo   | —                               |
| Lightmap baking                | 🔲 Pendiente prototipo   | —                               |
| Audio compression              | 🔲 Pendiente prototipo   | —                               |

---

_Documento generado a partir del historial de commits `d341e58` → `cb9b737` y análisis de código fuente. Actualizar cada vez que se completen las secciones pendientes de materiales._
