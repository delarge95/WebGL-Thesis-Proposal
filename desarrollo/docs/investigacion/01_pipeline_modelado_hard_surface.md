# 🔬 Investigación Técnica: Pipeline de Modelado Hard Surface y Kitbashing

**Fecha:** 30 de Noviembre, 2025  
**Autor:** Alexander Woodcock Salomón  
**Proyecto:** Prototipo WebGL Tesis  
**Estado:** Aprobado para Implementación

---

## 1. Resumen Ejecutivo
Este documento define el flujo de trabajo optimizado para la creación de assets Hard Surface (Drone Sci-Fi) destinados a WebGL. Se prioriza la calidad visual mediante el uso de **Kitbashing** para detalles mecánicos y **Texturizado Asistido por IA** para superficies realistas, manteniendo un presupuesto estricto de polígonos (<100k tris totales).

---

## 2. Estrategia de Kitbashing: Oleg Ushenok vs. JROTools

### Análisis de Topología y Uso
La investigación indica que los sets de **Oleg Ushenok** son el estándar de oro para "Concept & High-Poly", pero requieren tratamiento específico para motores de juego.

| Característica | Oleg Ushenok Kitbash | JROTools Kitbash | Recomendación |
|----------------|----------------------|------------------|---------------|
| **Densidad** | Media-Alta (Mid-Poly) | Limpia (Sub-D Ready) | Usar Oleg para detalles internos (motores) |
| **Topología** | A veces triangulada/N-gons | Quads limpios | Usar JROTools para formas principales |
| **UVs** | Generalmente incluidas | Incluidas y optimizadas | Re-empaquetar siempre en Atlas |
| **Uso Ideal** | Baking de Normales | Modelado Directo | **Híbrido** |

### Flujo de Trabajo Recomendado (Hybrid Workflow)
1.  **Estructura Principal (Fuselaje):** Modelado manual en Blender usando *Subdivision Surface* para garantizar silueta perfecta y deformación limpia.
2.  **Detalles Mecánicos (Motores/Articulaciones):** Importar piezas de *Oleg Ushenok*.
    *   *Tratamiento:* No intentar retopologizar cada tornillo. Usar estas piezas en el **High-Poly** y hacer "Bake" de su detalle sobre una geometría Low-Poly simplificada (Cilindros/Cajas).
3.  **Cables y Tuberías:** Usar curvas de Blender (Geometry Nodes) para generar cables que conecten las piezas del kitbash, unificando el diseño.

---

## 3. Pipeline de Texturizado con IA (Polycam -> Blender)

La integración de IA permite generar texturas de "desgaste realista" que tomarían horas pintar a mano.

### Paso a Paso Técnico
1.  **Generación en Polycam:**
    *   Prompt: *"Scratched painted aerospace metal, white ceramic coating, slight rust on edges, 4k"*
    *   Output: Mapas Albedo, Normal, Roughness, Displacement.
2.  **Importación en Blender:**
    *   Usar addon **Node Wrangler** (`Ctrl + Shift + T`) en el *Principled BSDF*.
    *   **Importante:** Configurar Color Space de Normal/Roughness a `Non-Color`.
3.  **Corrección de Normales:**
    *   Polycam a veces exporta normales DirectX (-Y). Blender usa OpenGL (+Y).
    *   *Fix:* En el nodo de imagen Normal, añadir curvas RGB y invertir el canal Verde, o usar un nodo "Normal Map" y verificar la orientación.
4.  **Mezcla de Materiales (Masking):**
    *   No usar la textura plana. Usar un *Vertex Color* o un mapa de *Curvature* (bakeado) para mezclar la textura de IA (desgaste) sobre un material base limpio solo en los bordes.

---

## 4. Mejores Prácticas de Topología para Baking

Para asegurar que los detalles del Kitbash se transfieran correctamente al Low-Poly:

1.  **Hard Edges & UV Seams:**
    *   Regla de Oro: **Donde hay un Hard Edge (Sharp), DEBE haber un corte de UV.**
    *   Esto evita artefactos de sombreado negros en los bordes.
2.  **Triangulación:**
    *   Aplicar modificador *Triangulate* al Low-Poly **antes** de exportar a Substance/Unity.
    *   Asegura que la triangulación sea idéntica en el bake y en el motor.
3.  **Cage Baking:**
    *   Usar siempre una "Cage" (versión inflada del low-poly) para capturar detalles de altura sin errores de proyección.

---

## 5. Referencias y Herramientas
*   **Software:** Blender 4.x, Polycam (Web), Unity 6.
*   **Addons Blender:** Hard Ops (Modelado), Node Wrangler (Texturas), TexTools (UVs).
*   **Assets:** Oleg Ushenok Hard Surface Kitbash Vol. 1-3.
