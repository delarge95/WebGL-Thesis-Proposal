# Breakdown: Pipeline CAD a WebGL

Resumen corto para convertir el pipeline del proyecto en una pieza de portafolio clara, vendible y fácil de reutilizar en ArtStation, reel, CV o entrevista técnica.

## Pieza

- **Título recomendado:** CAD-to-WebGL Pipeline for an Interactive Drone Digital Twin
- **Formato recomendado:** post largo de ArtStation + 4 a 6 slides + una versión resumida para LinkedIn
- **Ángulo de venta:** no es solo modelado 3D; es resolución de un problema real de pipeline, optimización y runtime.

## Problema que resuelve

El proyecto parte de geometría CAD industrial que no está lista para un motor en tiempo real ni para WebGL. El reto principal fue transformar activos pesados, con pivotes y transformaciones problemáticas, en un modelo interactivo y navegable dentro del presupuesto de un visor en navegador.

## Flujo resumido

1. **Referencia industrial y despiece técnico**
   - Se trabajó con documentación del Holybro X500 V2 y sus componentes para mantener coherencia técnica entre piezas, nomenclatura y ensamblaje.
2. **Conversión de activos CAD**
   - Los archivos CAD se llevaron a un flujo de malla poligonal utilizable, detectando desde el inicio problemas de pivote, duplicación y transformaciones horneadas.
3. **Normalización y simetría de piezas repetidas**
   - Se desarrolló tooling propio para detectar familias repetidas, sincronizar mallas y preparar una base más limpia para retopología, instancing y mantenimiento.
4. **Optimización geométrica**
   - El pipeline apuntó a reducir el peso de la geometría desde escala CAD a una versión viable para WebGL y navegación en hardware limitado.
5. **Texturizado y materiales**
   - Los assets pasan a un estado listo para shading y lectura visual dentro de Unity URP.
6. **Integración en Unity y despliegue WebGL**
   - El resultado final no es un render estático: es un visor 3D interactivo ejecutándose en navegador.

## Cuellos de botella que hacen fuerte esta pieza

- Los datos CAD no llegan listos para GPU instancing ni para exploded view natural.
- El runtime objetivo es WebGL, así que el presupuesto de memoria y draw calls es más estricto que en desktop.
- El valor no está sólo en el asset final, sino en el pipeline que hace repetible su transformación.

## Qué conviene mostrar

- Una vista del activo original o del ensamblaje CAD.
- Una lámina del problema de pivotes, duplicados o geometría horneada.
- Un diagrama simple del flujo: CAD → limpieza → simetría/instancing → optimización → shading → Unity → WebGL.
- Una comparación antes/después de polígonos o peso de la escena.
- Un clip corto del resultado corriendo en navegador.

## Mensaje central para portafolio

La pieza demuestra capacidad para conectar modelado técnico, tooling, optimización y entrega final en tiempo real. El diferencial no es haber importado un modelo a Unity, sino haber diseñado un flujo que vuelve ese modelo utilizable en un producto interactivo real.

## Claims reutilizables

- Diseñé un pipeline completo desde CAD industrial hasta un visor WebGL interactivo.
- Resolví problemas de simetría, pivotes e instancing con tooling propio en Blender.
- Convertí un activo pesado y técnico en una escena optimizada para navegador.
- Integré el resultado final en Unity URP con criterios de visualización y rendimiento.

## Evidencia técnica asociada

- `desarrollo/docs/investigacion/Holybro/CAD_Symmetry_Instancer_Documentation.md`
- `desarrollo/docs/ARCHITECTURE.md`
- `desarrollo/docs/DEPLOYMENT_GUIDE.md`
- `portafolio_personal/herramientas_fuente/pipeline_holybro/`

## Activos pendientes para cerrar esta pieza

- Diagrama visual limpio del pipeline.
- Capturas comparativas antes/después.
- Un clip o GIF del visor WebGL funcionando.
- Una cifra final estable del presupuesto geométrico y del objetivo de runtime a mostrar públicamente.
