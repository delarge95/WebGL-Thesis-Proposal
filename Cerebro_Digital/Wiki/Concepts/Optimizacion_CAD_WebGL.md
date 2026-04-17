---
tipo: "concepto"
fuente: "desarrollo/docs/investigacion/Holybro/CAD_Fastener_Optimization_Plan.md"
---
# Optimización Agresiva de CAD para WebGL

Este concepto define la estrategia híbrida (Blender + Unity) para reducir el peso de geometría CAD excesiva (específicamente tornillos y piezas herméticas), vital para asegurar el rendimiento en navegadores web.

## Problema Original
- Modelos originales superan los 3.5 millones de vértices por culpa de ~300 instancias de tornillos pesados.
- Ver documento crudo original: [[CAD_Fastener_Optimization_Plan]]

## Solución Técnica (El Pipeline)
1. **Destrucción y Proxy (Blender):** Uso de un script en Python (ver entidad: [[Fastener_Builder_Addon]]) para detectar la geometría pesada usando cálculos de *Bounding Box* y reemplazarla de forma algorítmica por primitivas optimizadas de 12 caras.
2. **Ensamblaje Modular Tiling (Unity):** Reconstrucción visual en Runtime (solo cuando aplica el LOD0) utilizando un script de C# que clona y apila partes modulares de la rosca (Top, Middle, Bottom).
