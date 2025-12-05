# 🔬 Investigación Técnica: Arquitectura de Shaders para Visualización

**Fecha:** 30 de Noviembre, 2025  
**Autor:** Alexander Woodcock Salomón  
**Proyecto:** Prototipo WebGL Tesis  
**Estado:** Aprobado para Implementación

---

## 1. Resumen Ejecutivo
Este documento detalla la implementación técnica de los efectos de visualización avanzados (Rayos X, Outlines, Clipping) utilizando **Unity Shader Graph** y **Universal Render Pipeline (URP)**. Estas técnicas son críticas para la funcionalidad de "análisis estructural" del prototipo.

---

## 2. Delineado Técnico (Screen Space Outlines)

El delineado técnico no debe ser geometría invertida (método "Hull") ya que duplica polígonos. Se usará un enfoque de **Post-Procesamiento en Espacio de Pantalla**.

### Implementación en URP
1.  **Requisitos Previos:**
    *   Habilitar `Depth Texture` y `Opaque Texture` en el URP Asset.
    *   Habilitar `Normal Texture` (o reconstruirla desde profundidad si es necesario para optimizar ancho de banda).
2.  **Lógica del Shader (Fullscreen Graph):**
    *   **Detección de Profundidad:** Muestrear el *Scene Depth* en cruz (centro, arriba, abajo, izq, der). Calcular la diferencia (`abs(center - neighbor)`). Si la diferencia supera un `_DepthThreshold`, es un borde.
    *   **Detección de Normales:** Muestrear *Scene Normals*. Calcular el producto punto entre la normal del píxel y sus vecinos. Si el ángulo es agudo (> `_NormalThreshold`), es un borde.
    *   **Combinación:** `max(DepthEdge, NormalEdge)` para obtener el borde final.
3.  **Integración:**
    *   Añadir como **Full Screen Pass Renderer Feature** en la lista de renderizado de URP.
    *   Injection Point: `Before Post Processing`.

---

## 3. Efecto de Rayos X (Ghost Mode)

Permite ver componentes internos a través de la carcasa exterior.

### Arquitectura del Shader (Shader Graph)
1.  **Render State:**
    *   Surface Type: **Transparent**.
    *   Blending Mode: **Additive** (para efecto holográfico) o **Alpha** (para cristal).
    *   **Z-Test:** `Always` o `Greater` (para que se dibuje incluso si está ocluido por geometría normal). *Nota: En Shader Graph URP, esto a veces requiere editar el shader generado o usar un Custom Pass, pero para WebGL simple, usar transparencia estándar con Fresnel suele bastar.*
2.  **Efecto Fresnel:**
    *   Usar nodo `Fresnel Effect`.
    *   Potencia alta (3-5) para que solo los bordes sean visibles.
    *   Color: Cian o Naranja neón.
3.  **Interacción:**
    *   Controlar la opacidad general mediante una variable global `_XRayStrength` modificada por script C#.

---

## 4. Plano de Corte (World Space Clipping)

Permite realizar cortes transversales dinámicos para inspección.

### Implementación Matemática
El shader debe descartar píxeles basándose en su posición en el mundo relativa a un plano imaginario.

**Fórmula:**
$$ Distancia = (PosicionMundo \cdot NormalPlano) - OffsetPlano $$

**Nodos en Shader Graph:**
1.  Input: `Position (World)`.
2.  Property: `_ClipPlaneNormal` (Vector3), `_ClipPlaneOffset` (Float).
3.  Operación: `Dot Product(Pos, Normal) - Offset`.
4.  Lógica de Descarte:
    *   Usar nodo `Step`. Si Distancia < 0, el resultado es 0.
    *   Conectar a **Alpha Clip Threshold** en el Master Node.
    *   Habilitar **Alpha Clipping** en las opciones del Graph.
5.  **Visualización del Corte (Capping):**
    *   Para que el objeto no parezca hueco, se requiere un paso adicional (Stencil Buffer) o renderizar las caras traseras (`Two Sided = True`) con un color plano oscuro para simular el interior sólido.

---

## 5. Animación Procedural (DOTween)

Para la "Vista Explosionada", no usaremos shaders de desplazamiento de vértices (que deforman la malla y pierden precisión técnica), sino manipulación de Transformaciones.

### Arquitectura C#
```csharp
public class ExplodedPart : MonoBehaviour {
    private Vector3 originalPos;
    public Vector3 explodedPos; // Definido manualmente o calculado por dirección desde el centro

    public void SetExplosion(float percentage) {
        // Interpolación lineal simple y eficiente
        transform.localPosition = Vector3.Lerp(originalPos, explodedPos, percentage);
    }
}
```
*   **Optimización:** Usar `DOTween` para transiciones suaves (`DOVirtual.Float`) que controlen el valor `percentage` globalmente, en lugar de tener tweens individuales corriendo en cada tornillo.

---

## 6. Referencias
*   Unity Manual: URP Renderer Features.
*   Tutorial: "Unity Shader Graph Clipping Plane" (Ronja Tutorials).
*   Documentación: DOTween Pro.
