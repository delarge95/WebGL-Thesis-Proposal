---
tipo: "entidad"
fuente: "desarrollo/docs/investigacion/Holybro/CAD_Fastener_Optimization_Plan.md"
---
# Addon: Fastener Proxy Automator

Esquema del script de Python para automatizar la Fase 2 de la estrategia de [[Optimizacion_CAD_WebGL]].

- **Función:** Automatiza el reemplazo masivo de geometría estática pesada exportada de MoI3D. Construye proxies LOD1 extremadamente livianos basados en el ratio geométrico detectado de la pieza.
- **Archivos de código relacionados:** Se basa en la lógica que desarrollamos en `cad_proxy_automator.py` y `cad_thread_retopo.py`.
