# Holybro X500 V2 - Bake final Blender y export manual a Unity

## Estado y criterio rector

Este flujo reemplaza el supuesto anterior de exportar solo instancias. En la escena final los masters tambien son piezas fisicas del dron. Por tanto, el runtime final debe considerar la union de:

- `BAKE_MASTERS_LOW`
- `ASSEMBLY_INSTANCES_LOW`
- `PRIMITIVE_FASTENER_MASTERS`
- `PRIMITIVE_FASTENER_INSTANCES`

La exportacion desde Blender sera manual. Codex no debe exportar el FBX via MCP, pero si puede generar reportes, manifests y preparar scripts de verificacion.

## Preparacion del archivo

1. Abrir el archivo con Blender Steam `5.0.1`.
2. Guardar una copia de trabajo, por ejemplo `ready-to-bake_006_final-material-bake.blend`.
3. Confirmar que las cuatro colecciones runtime esten visibles y seleccionables.
4. Mantener excluidas para bake/export las colecciones high, temporales y de diagnostico:
   `BAKE_MASTERS_HIGH`, `ASSEMBLY_INSTANCES_HIGH`, `STEPper`, `Ngons`, `Cutters`, `tools`, pruebas y renders.
5. No aplicar transforms, no borrar masters y no colapsar instancias.

## Targets finales de textura

Los mapas runtime finales son:

- `X500_BaseColor_4K.png`: sRGB.
- `X500_Normal_Final_4K.png`: Non-Color, normal tangent space.
- `X500_Mask_4K.png`: Non-Color, `R=AO`, `G=Roughness`, `B=Curvature`, `A=Metallic`.
- `X500_MetallicSmoothness_4K.png`: derivado opcional para URP Lit, `R=Metallic`, `A=Smoothness`.
- `Hex_AO.png` y `Hex_Normal_Atlas.png`: mapas 256 para cabezotes de fasteners primitivos.

Los mapas debug `X500_AO_Final_4K.png`, `X500_Roughness_4K.png` y `X500_Metallic_4K.png` se conservan para QA, pero no tienen que viajar como texturas runtime separadas si la mask queda validada.

## Bake de Base Color

1. Crear o preparar `X500_BaseColor_4K`, `4096 x 4096`, `8-bit`, `sRGB`.
2. En cada material runtime, dejar activo un nodo `Image Texture` apuntando a esa imagen.
3. Render Engine: `Cycles`.
4. Bake Type: `Diffuse`.
5. Contributions: activar solo `Color`; desactivar `Direct` e `Indirect`.
6. Margin/Padding: `32 px`.
7. Clear Image: activado para el primer bake.
8. Guardar como `X500_BaseColor_4K.png`.

## Bake de Roughness

1. Crear `X500_Roughness_4K`, `4096 x 4096`, `8-bit`, `Non-Color`.
2. Si Blender ofrece pase `Roughness` y captura correctamente el material final, usarlo.
3. Si no captura los nodos procedurales, usar metodo `Emit` en el archivo duplicado:
   conectar el valor final de roughness a un `Emission Shader` y este a `Material Output`.
4. Bake Type: `Emit`.
5. Margin: `32 px`.
6. Guardar como `X500_Roughness_4K.png`.

## Bake de Metallic

1. Crear `X500_Metallic_4K`, `4096 x 4096`, `8-bit`, `Non-Color`.
2. Usar metodo `Emit`:
   conectar el valor final de metallic al color de un `Emission Shader`.
3. Blanco representa metal; negro representa dielectrico.
4. Validar que carbono, plasticos, gomas y espuma queden negros.
5. Guardar como `X500_Metallic_4K.png`.

## Normal final con fibra de carbono

1. Preservar `MASTERS_bake_normal.png` como base historica.
2. Crear `X500_Normal_Final_4K`, `4096 x 4096`, `Non-Color`.
3. Si el material de fibra de carbono tiene bump/normal procedural, dejarlo conectado al input `Normal`.
4. Bake Type: `Normal`.
5. Space: `Tangent`.
6. Margin: `32 px`.
7. Si el bake high-to-low actual contiene detalle geometrico critico, usar ruta segura:
   bakear `Carbon_Detail_Normal_4K` y combinarlo con `MASTERS_bake_normal.png`.
8. Solo reemplazar por `X500_Normal_Final_4K` cuando el resultado conserve detalle geometrico y detalle de fibra.

## AO final con micro-occlusion

1. Preservar `MASTERS_bake_ao.png`.
2. Crear `X500_AO_Final_4K`, `4096 x 4096`, `Non-Color`.
3. Para AO geometrico puro: Bake Type `Ambient Occlusion`, samples `128-256`, margin `32 px`.
4. Para sumar micro-occlusion de fibra/material:
   usar metodo `Emit` y bakear una mezcla equivalente a `AO_base * Carbon_MicroAO * AO_material`.
5. El AO final no debe contener iluminacion direccional.

## Packing de mask

Despues de tener los mapas debug, ejecutar `blender_pack_x500_mask.py` desde Blender 5.0.1 o adaptar rutas dentro del script.

Canales:

- `R = X500_AO_Final_4K.png`
- `G = X500_Roughness_4K.png`
- `B = MASTERS_bake_curve.png` o negro si curvature no se usa.
- `A = X500_Metallic_4K.png`

Tambien se genera `X500_MetallicSmoothness_4K.png` para compatibilidad URP Lit estandar.

Variables opcionales para ejecucion controlada:

- `HOLYBRO_TEXTURE_DIR`: carpeta de entrada/salida de texturas.
- `HOLYBRO_CURVATURE_MAP`: nombre del mapa de curvature. Si se deja vacio, el canal B queda negro.

## Scripts de apoyo no destructivos

Los scripts incluidos en esta carpeta estan pensados para ejecutarse solo cuando Blender este estable. No exportan FBX, no guardan el `.blend` y no asignan parentescos de fasteners por suposicion.

- `blender_bake_target_setup.py`: crea/prepara imagenes destino y deja activo el nodo `Image Texture` correcto en materiales runtime. Usa `HOLYBRO_BAKE_TARGET=base_color|roughness|metallic|normal|ao`.
- `blender_pack_x500_mask.py`: empaqueta `AO`, `Roughness`, `Curvature` y `Metallic` en `X500_Mask_4K.png`, y crea el derivado URP `X500_MetallicSmoothness_4K.png`.
- `blender_runtime_manifest_exporter.py`: genera un manifest de masters, instancias, transforms, bounds y candidatos de pieza madre para fasteners. Usa `HOLYBRO_MANIFEST_OUTPUT` si se quiere escribir el preview fuera del repositorio.

## Export manual FBX

Seleccionar manualmente solo las cuatro colecciones runtime:

- `BAKE_MASTERS_LOW`
- `ASSEMBLY_INSTANCES_LOW`
- `PRIMITIVE_FASTENER_MASTERS`
- `PRIMITIVE_FASTENER_INSTANCES`

Configuracion FBX:

- Selected Objects: `ON`
- Object Types: `Mesh`; incluir `Empty` solo si se necesitan anchors.
- Apply Transform: `OFF`
- Apply Unit: `ON`
- Forward: `-Z Forward`
- Up: `Y Up`
- Add Leaf Bones: `OFF`
- Bake Animation: `OFF`
- Embed Textures: `OFF`

Salida sugerida:

- `X500V2_runtime_low_final.fbx`
- `X500_BaseColor_4K.png`
- `X500_Normal_Final_4K.png`
- `X500_Mask_4K.png`
- `X500_MetallicSmoothness_4K.png` si se usa URP Lit.
- `Hex_AO.png`
- `Hex_Normal_Atlas.png`

## Import en Unity

El FBX final se copia a:

- `Assets/Models/x500v2_runtime_low_final.fbx`

La importacion de escena se ejecuta desde Unity con:

- `Tools > Import Final Runtime Drone Model`

Este tool:

- conserva el root anterior como referencia inactiva bajo `x500v2_ReferenceModels`;
- instancia el FBX final como `x500v2_Drone`;
- preserva jerarquia y nombres del FBX;
- normaliza helices con prefijo `x500v2_prop_`;
- agrupa fasteners primitivos reconocidos (`GB70`, `PAN`, `CHEN`, `ZSLM`, `LM`, `NILONGZHU`) bajo `x500v2_fastener_group`;
- mantiene `HUAN-GUIJIAO` y `GPSV5-ZHIJIA-LUOMAO` como subpiezas runtime porque pertenecen a `BAKE_MASTERS_LOW` / `ASSEMBLY_INSTANCES_LOW`, no a las colecciones primitivas de fasteners;
- ejecuta `PrepareImportedDroneHeadless()` para reconstruir anchors, colliders, metadata, filtros, hotspots y fastener registry;
- genera `Reports/final_runtime_import_report.md` con conteo de shared meshes para verificar si Unity preservo instancias logicas.

La herramienta no elimina el FBX anterior ni borra el modelo anterior de la escena; lo deja como referencia desactivada.

## Manifest y fasteners dudosos

Ejecutar `blender_runtime_manifest_exporter.py` antes o despues del export manual para generar:

- Inventario de masters e instancias.
- Roles runtime.
- Bounds y matrices world.
- Candidatos de pieza madre para cada fastener.
- Lista de fasteners que requieren revision manual.

Regla: si un fastener no puede asignarse con confianza, queda en `x500v2_fastener_group` y se reporta con candidatos. No se asigna por suposicion.

El reporte tambien compara el conteo nuevo de `PRIMITIVE_FASTENER_INSTANCES` contra el baseline Unity anterior de `168` fasteners. La diferencia no se corrige automaticamente; se usa como lista de revision para ajustar catalogos o confirmar cambios de escena.
