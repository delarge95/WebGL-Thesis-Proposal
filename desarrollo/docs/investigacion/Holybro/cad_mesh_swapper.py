import bpy
import numpy as np
import mathutils

bl_info = {
    "name": "CAD Mesh Swapper",
    "author": "Antigravity",
    "version": (1, 1),
    "blender": (4, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Inyecta la geometría interna limpia en un objeto dañado, conservando su posición global e instancias.",
    "category": "Object",
}

def get_local_mesh_center(mesh):
    if len(mesh.vertices) == 0:
        return mathutils.Vector((0, 0, 0))
    v_co = [v.co for v in mesh.vertices]
    x = sum([v[0] for v in v_co]) / len(v_co)
    y = sum([v[1] for v in v_co]) / len(v_co)
    z = sum([v[2] for v in v_co]) / len(v_co)
    return mathutils.Vector((x, y, z))

class CAD_OT_mesh_swapper(bpy.types.Operator):
    bl_idname = "cad.mesh_swapper"
    bl_label = "Reemplazar Geometría Local"
    bl_options = {'REGISTER', 'UNDO'}
    
    alignment_mode: bpy.props.EnumProperty(
        name="Alineación Local",
        items=[
            ('CENTER', 'Center of Mass (Auto-Centrado)', 'Centra la nueva malla perfecta en el pivote original.'),
            ('RAW', 'Raw Local Transfer (Directo)', 'Transfiere los vértices exactamente como están modelados localmente.')
        ],
        default='CENTER'
    )
    
    keep_target_materials: bpy.props.BoolProperty(
        name="Mantener Materiales Originales",
        default=True
    )
    
    apply_modifiers: bpy.props.BoolProperty(
        name="Aplicar Modificadores del Nuevo",
        default=False
    )
    
    @classmethod
    def poll(cls, context):
        if context.active_object is None or context.active_object.type != 'MESH':
            return False
        selected = [o for o in context.selected_objects if o.type == 'MESH']
        return len(selected) == 2

    def execute(self, context):
        target = context.active_object
        source = [o for o in context.selected_objects if o != target][0]
        
        # 1. Obtener malla limpia
        if self.apply_modifiers:
            depsgraph = context.evaluated_depsgraph_get()
            src_eval = source.evaluated_get(depsgraph)
            new_mesh = bpy.data.meshes.new_from_object(src_eval)
        else:
            new_mesh = source.data.copy()
            
        new_mesh.name = target.data.name + "_SWAPPED"
        
        # 2. ALINEACIÓN GEOMÉTRICA (Conservando Matrix World del Target intacto)
        if self.alignment_mode == 'CENTER':
            src_local_cm = get_local_mesh_center(new_mesh)
            tgt_local_cm = get_local_mesh_center(target.data)
            
            # Offset puramente en el espacio local de las mallas
            offset_local = tgt_local_cm - src_local_cm
            translation_mat = mathutils.Matrix.Translation(offset_local)
            
            # Movemos los vértices de la malla limpia para que su centro coincida con el centro que tenía el dañado
            new_mesh.transform(translation_mat)
            
        # 3. Trasladar Materiales
        new_mesh.materials.clear()
        if self.keep_target_materials:
            for mat in target.data.materials:
                new_mesh.materials.append(mat)
        else:
            for mat in source.data.materials:
                new_mesh.materials.append(mat)
                
        # 4. SUSTITUCIÓN GLOBAL DE INSTANCIAS
        old_mesh = target.data
        affected_instances = [obj for obj in bpy.data.objects if obj.data == old_mesh]
        
        for obj in affected_instances:
            if "swapper_backup" not in obj:
                obj["swapper_backup"] = old_mesh.name
            # Reemplazo de la geometría interna. El obj.matrix_world jamas se toca
            obj.data = new_mesh
            
        self.report({'INFO'}, f"Geometría traspasada a {len(affected_instances)} instancias con éxito.")
        
        # 5. Ocultar el sobrante
        source.hide_viewport = True
        source.hide_render = True
        
        return {'FINISHED'}

class CAD_OT_revert_swapper(bpy.types.Operator):
    bl_idname = "cad.revert_swapper"
    bl_label = "Deshacer Swap Global"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        revert_count = 0
        for obj in bpy.data.objects:
            if "swapper_backup" in obj:
                old_name = obj["swapper_backup"]
                if old_name in bpy.data.meshes:
                    obj.data = bpy.data.meshes[old_name]
                    revert_count += 1
                del obj["swapper_backup"]
        
        self.report({'INFO'}, f"Restauradas {revert_count} instancias a su malla dañada.")
        return {'FINISHED'}

class CAD_PT_swapper_panel(bpy.types.Panel):
    bl_label = "Local Mesh Swapper & Replacer"
    bl_idname = "CAD_PT_swapper_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'
    
    def draw(self, context):
        layout = self.layout
        box = layout.box()
        
        box.label(text="Inyector de Mallas Locales", icon='MESH_DATA')
        
        if context.active_object is None or context.active_object.type != 'MESH':
            box.label(text="Selecciona 2 mallas.", icon='ERROR')
        else:
            selected = [o for o in context.selected_objects if o.type == 'MESH']
            if len(selected) != 2:
                box.label(text="Necesitas exactamente 2 mallas.", icon='INFO')
                box.label(text="(Activo = TARGET. Naranja = SOURCE)", icon='DOT')
            else:
                target = context.active_object
                source = [o for o in selected if o != target][0]
                box.label(text=f"Geo Nueva: {source.name}")
                box.label(text=f"Geo Dañada: {target.name} (Se actualizará)")
                
                op = box.operator("cad.mesh_swapper", text="Inyectar Geometría Local", icon='FILE_REFRESH')
                
        box.separator()
        box.operator("cad.revert_swapper", text="Deshacer Swap Global", icon='LOOP_BACK')

def register():
    bpy.utils.register_class(CAD_OT_mesh_swapper)
    bpy.utils.register_class(CAD_OT_revert_swapper)
    bpy.utils.register_class(CAD_PT_swapper_panel)

def unregister():
    bpy.utils.unregister_class(CAD_OT_mesh_swapper)
    bpy.utils.unregister_class(CAD_OT_revert_swapper)
    bpy.utils.unregister_class(CAD_PT_swapper_panel)

if __name__ == "__main__":
    register()
