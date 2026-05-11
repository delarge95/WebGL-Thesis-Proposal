# Automatización del Flujo de Trabajo para Baking de Instancias CAD (Blender ➔ RizomUV ➔ Marmoset)

> Nota operativa 2026-05-08: este documento conserva el historial del flujo CAD/RizomUV/Marmoset. Para la entrega final Blender -> Unity, usar `Blender_Final_Bake_Export_Unity_Workflow.md`. En la escena final los masters tambien son piezas fisicas del dron; por tanto, no se debe exportar solo instancias.

## 1. El Problema: Baking de Ensamblajes CAD Masivos

Al importar modelos CAD (archivos STEP) de ingeniería (como el dron Holybro X500v2), el resultado es un ensamblaje jerárquico masivo. Las piezas repetitivas (tornillos M3, tuercas, tubos de fibra de carbono, separadores) se importan como **instancias** que comparten la misma malla base (Mesh Data) en memoria.

Si intentamos exportar todo este ensamblaje directamente a un software de *baking* (como Marmoset Toolbag o Substance Painter) para transferir los detalles del modelo de alta resolución (High Poly) al modelo optimizado (Low Poly), nos encontramos con graves problemas:
1. **Oclusión (AO) Errónea:** Las piezas se proyectarán sombras mutuamente, arruinando el mapa de Ambient Occlusion.
2. **Artefactos de Normales:** Las normales se cruzarán entre piezas cercanas.
3. **Pérdida de Rendimiento:** El software de baking colapsará intentando procesar miles de objetos redundantes.
4. **Error de Escalas en Blender:** Marmoset y Unity exigen que las piezas tengan escala `(1.0, 1.0, 1.0)`. Sin embargo, al intentar hacer `Ctrl+A > Apply Scale` en Blender, el programa arroja el error **"Cannot apply to a multi-user data"**, bloqueando el proceso para proteger las instancias.

## 2. La Solución (El Flujo de Trabajo AAA)

En estudios profesionales, la solución no es colapsar el modelo ni destruir las instancias, sino aplicar un flujo de trabajo de **Extracción de Masters**:

1. **Extracción:** Se toma un único ejemplar de cada pieza repetida (el "Master").
2. **Saneamiento:** Se corrigen las escalas matemáticamente para burlar la restricción de Blender, sin romper el ensamblaje visual original.
3. **Nomenclatura Estricta:** Se nombran los Masters con sufijos exactos (`_low` y `_high`).
4. **Exportación Aislada:** Se envían **solo** los Masters a desenvolver en RizomUV y luego a hornear en Marmoset Toolbag. Marmoset usará los sufijos para aislar cada pieza en un "Bake Group" independiente, garantizando un bakeo limpio sin importar dónde estén ubicadas en el espacio 3D.
5. **Reensamblaje:** En Unity, la textura horneada se aplica al Material. Dado que todas las instancias originales (exportadas directamente desde Blender a Unity) comparten ese mismo Material y Malla, el bakeo se propaga instantáneamente a los miles de tornillos, ahorrando meses de trabajo manual.

---

## 3. Desarrollo de los Scripts de Automatización

Para evitar hacer este proceso manualmente para cientos de piezas, desarrollamos dos scripts de Python en Blender (ejecutados a través de la integración de Antigravity MCP).

### Script 1: CAD Bake Preparer (Extracción y Saneamiento)

Este script automatiza la extracción de Masters, la nomenclatura y la corrección del problema de *Multi-user data*.

**¿Cómo funciona la corrección de escala?**
En lugar de forzar a Blender con `Ctrl+A`, el script altera la geometría base:
1. Toma la escala local del Master (ej. `S = 0.001`).
2. Multiplica las coordenadas `(X, Y, Z)` de todos los vértices dentro de la malla por esa escala (`v.co *= S`). Esto hace que la malla real sea del tamaño correcto.
3. Busca todas las instancias en la escena que usan esa malla y resetea su escala en el panel de Transformación a `1.0`. Visualmente, la pieza mantiene su tamaño idéntico, pero el problema técnico desaparece.

```python
import bpy
import re

def process_cad_collection(source_col_name, master_col_name, instance_col_name, suffix):
    source_col = bpy.data.collections.get(source_col_name)
    master_col = bpy.data.collections.new(master_col_name)
    instance_col = bpy.data.collections.new(instance_col_name)
    bpy.context.scene.collection.children.link(master_col)
    bpy.context.scene.collection.children.link(instance_col)

    counts = {}
    processed_meshes = set()
    masters, instances = [], []

    for obj in list(source_col.all_objects):
        if obj.type != 'MESH' or not obj.data: continue
            
        mesh_name = obj.data.name
        
        # 1. Aplicar Escala a nivel de Vértices y Resetear Instancias
        if mesh_name not in processed_meshes:
            processed_meshes.add(mesh_name)
            masters.append(obj)
            
            scale = obj.scale.copy()
            if abs(scale.x - 1.0) > 0.0001:
                for v in obj.data.vertices:
                    v.co.x *= scale.x; v.co.y *= scale.y; v.co.z *= scale.z
            
            for other_obj in bpy.data.objects:
                if other_obj.type == 'MESH' and other_obj.data == obj.data:
                    other_obj.scale = (1.0, 1.0, 1.0)
        else:
            instances.append(obj)

        # 2. Renombrar automáticamente (ej. BASE.001_low)
        base_name = re.sub(r'\.\d{3,4}$', '', obj.name)
        base_name = re.sub(r'_low$|_high$', '', base_name)
        if base_name not in counts: counts[base_name] = 1
        obj.name = f"{base_name}.{counts[base_name]:03d}{suffix}"
        counts[base_name] += 1

    # 3. Mover a Colecciones
    for obj in masters: master_col.objects.link(obj); source_col.objects.unlink(obj)
    for obj in instances: instance_col.objects.link(obj); source_col.objects.unlink(obj)

process_cad_collection("lvl10.001", "BAKE_MASTERS_LOW", "ASSEMBLY_INSTANCES_LOW", "_low")
process_cad_collection("lvl1000.001", "BAKE_MASTERS_HIGH", "ASSEMBLY_INSTANCES_HIGH", "_high")
```

### Script 2: Parche Matemático de Jerarquías CAD (El Bug de las Escalas Dispares)

Tras ejecutar el primer script, notamos un problema: **13 tornillos de repente medían 5 metros de ancho.**

**¿Por qué ocurrió?**
El Script 1 asumía que todas las instancias de un tornillo tenían la misma escala mundial. Sin embargo, en el archivo CAD importado, la mayoría de los tornillos estaban dentro de un *Empty* padre con escala `0.001`, mientras que esos 13 tornillos específicos habían sido movidos a la raíz de la escena (escala padre `1.0`). 
Al aplicar la escala de la malla base (que se hizo 1000 veces más grande) y fijar el objeto a `1.0`, los tornillos dentro del padre se compensaron correctamente (`1000 * 0.001 = 1`), pero los 13 sueltos explotaron en tamaño (`1000 * 1.0 = 1000`).

**La solución algorítmica:**
Escribimos un segundo script que calcula exactamente qué escala local necesita tener cada instancia para que su tamaño en el mundo 3D (`matrix_world.to_scale()`) sea matemáticamente idéntico al del Master, independientemente de qué padre tenga.

```python
import bpy
from mathutils import Vector

def fix_instance_scales(master_col_name, instance_col_name):
    master_col = bpy.data.collections.get(master_col_name)
    instance_col = bpy.data.collections.get(instance_col_name)
    mesh_target_scales = {}
    
    # Obtener el tamaño "correcto" en el mundo del Master
    for master in master_col.objects:
        if master.type == 'MESH' and master.data:
            mesh_target_scales[master.data.name] = master.matrix_world.to_scale()
            
    # Forzar a las instancias a igualar ese tamaño en el mundo
    for inst in instance_col.objects:
        if inst.type == 'MESH' and inst.data and inst.data.name in mesh_target_scales:
            target_world_scale = mesh_target_scales[inst.data.name]
            parent_world_scale = inst.parent.matrix_world.to_scale() if inst.parent else Vector((1.0, 1.0, 1.0))
            
            # Calcular la escala local necesaria (Escala Objetivo / Escala del Padre)
            required_local_scale = Vector((
                target_world_scale.x / parent_world_scale.x if parent_world_scale.x != 0 else 1.0,
                target_world_scale.y / parent_world_scale.y if parent_world_scale.y != 0 else 1.0,
                target_world_scale.z / parent_world_scale.z if parent_world_scale.z != 0 else 1.0
            ))
            inst.scale = required_local_scale

fix_instance_scales("BAKE_MASTERS_LOW", "ASSEMBLY_INSTANCES_LOW")
```

---

## 4. Resultado y Próximos Pasos del Pipeline

Con estos scripts ejecutados:
1. De 244 mallas importadas, se extrajeron **exactamente 55 Masters Low Poly** y **55 Masters High Poly**.
2. Las **189 instancias restantes** mantienen sus ubicaciones precisas y ahora sus escalas son 100% compatibles con exportación.

**Siguientes pasos:**
1. Exportar la colección `BAKE_MASTERS_LOW` a **RizomUV**, preservando las costuras (Edge Seams) marcadas manualmente en Blender, para realizar un empaquetado (Pack) de máxima eficiencia.
2. Cargar los Masters emparejados (`_low` y `_high`) en el Quick Loader de **Marmoset Toolbag 4**.
3. Configurar Marmoset para usar Tangentes **MikkTSpace** y Normales **OpenGL** (estándar de Unity).
4. Llevar todo a Unity y disfrutar de un rendimiento brutal con *GPU Instancing*.
