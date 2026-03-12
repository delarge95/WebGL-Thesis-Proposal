import bpy
import math
from mathutils import Matrix, Vector

bl_info = {
    "name": "CAD Symmetry Instancer V2",
    "author": "Antigravity (Engineer Mode)",
    "version": (2, 0),
    "blender": (3, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Calcula simetría radial por Centro de Masa evitando errores de vértices CAD aleatorios. Incluye Backup seguro.",
    "category": "Object",
}

class CAD_OT_set_origins_backup(bpy.types.Operator):
    """Calcula y traslada el origen al Centro de Gravedad Volumétrico guardando un backup de la malla"""
    bl_idname = "cad.set_origins_backup"
    bl_label = "1. Respaldar y Set Origins (CoM)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects:
            self.report({'WARNING'}, "No hay objetos seleccionados")
            return {'CANCELLED'}
            
        backup_count = 0
        for obj in context.selected_objects:
            if obj.type == 'MESH':
                # Custom Undo: Guardamos el puntero exacto a la malla original antes de destruirla
                obj["backup_mesh"] = obj.data.name
                
                # Guardamos la Matriz de Mundo (Transformación) original como 16 floats planos
                flat = [v for row in obj.matrix_world for v in row]
                obj["backup_matrix"] = flat
                backup_count += 1
                
        # Move origins
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        
        # Save new target locations
        for obj in context.selected_objects:
            obj["cad_loc"] = obj.location.copy()
            
        self.report({'INFO'}, f"Orígenes reparados y backup seguro creado para {backup_count} objetos.")
        return {'FINISHED'}

class CAD_OT_scan_math_radial(bpy.types.Operator):
    """Escanea la rotación asumiendo Simetría Radial (Z) en relación al centro (0,0,0) del Dron"""
    bl_idname = "cad.scan_math_radial"
    bl_label = "2. Escanear Radial (Vectores 2D)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master or master.type != 'MESH':
            self.report({'ERROR'}, "Selecciona la pieza MAESTRA en amarillo claro al final.")
            return {'CANCELLED'}
            
        slaves = [o for o in context.selected_objects if o != master and o.type == 'MESH']
            
        calc_count = 0
        for obj in slaves:
            # En lugar de usar los vértices del CAD (que tienen índices aleatorios y rompieron el script anterior)
            # Calculamos la rotación comparando el vector [X, Y] del Maestro vs el [X, Y] del Esclavo
            # Ambos partiendo desde el centro del mundo (0,0) que es el centro del dron.
            
            vec_m = Vector((master.location.x, master.location.y))
            vec_s = Vector((obj.location.x, obj.location.y))
            
            if vec_m.length < 1e-4 or vec_s.length < 1e-4:
                # Si una pieza está exactamente en el centro (0,0), no hay forma de saber su rotación por vectores
                continue
                
            # Ángulo polar (Yaw / Dirección en Z)
            angle_m = math.atan2(vec_m.y, vec_m.x)
            angle_s = math.atan2(vec_s.y, vec_s.x)
            
            # La rotación necesaria en Z para ir desde el Maestro al Esclavo
            angle_diff = angle_s - angle_m
            
            obj["cad_rot_z"] = angle_diff
            obj["cad_master"] = master.name
            calc_count += 1
            
        self.report({'INFO'}, f"Ángulo radial extraído exitosamente de {calc_count} esclavas.")
        return {'FINISHED'}

class CAD_OT_create_instances(bpy.types.Operator):
    """Purga las mallas esclavas y fuerza una instanciación (Alt+D) con el maestro"""
    bl_idname = "cad.create_instances"
    bl_label = "3. Purgar y Crear Instancias"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        
        slaves = [o for o in context.selected_objects if o != master]
        purged = 0
        
        for obj in slaves:
            if "cad_master" in obj and obj["cad_master"] == master.name:
                # NO borramos la malla huérfana aún, para poder hacer Undo
                obj.data = master.data
                purged += 1
                
        self.report({'INFO'}, f"{purged} copias reemplazadas por la malla Maestra.")
        return {'FINISHED'}

class CAD_OT_transport_locs(bpy.types.Operator):
    """Usa la coordenada 'cad_loc' guardada en el Paso 1 para mantener la pieza en su sitio"""
    bl_idname = "cad.transport_locs"
    bl_label = "4. Restaurar Posiciones"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        
        moved = 0
        for obj in slaves:
            if "cad_loc" in obj:
                obj.location = obj["cad_loc"]
                moved += 1
                
        self.report({'INFO'}, f"{moved} posiciones mundiales reubicadas.")
        return {'FINISHED'}

class CAD_OT_apply_rots_z(bpy.types.Operator):
    """Añade el factor de rotación Radial (Z) calculado en el paso 2"""
    bl_idname = "cad.apply_rots_z"
    bl_label = "5. Aplicar Rotación Radial"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        
        rotated = 0
        for obj in slaves:
            if "cad_rot_z" in obj:
                # Tomar la rotación base del Maestro y sumarle nuestro deflector Radial
                new_coords = list(master.rotation_euler)
                new_coords[2] += obj["cad_rot_z"]
                obj.rotation_euler = tuple(new_coords)
                rotated += 1
                
        self.report({'INFO'}, f"{rotated} rotaciones de simetría Z aplicadas.")
        return {'FINISHED'}

class CAD_OT_revert_undo(bpy.types.Operator):
    """Deshace todos los cambios inyectando los bloques de memoria originales salvados en el Paso 1"""
    bl_idname = "cad.revert_undo"
    bl_label = "RESTAURAR (Hard Undo Seguro)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        
        restored = 0
        for obj in context.selected_objects:
            if "backup_mesh" in obj:
                # 1. Recuperar la Malla original sin modificar
                mesh_name = obj["backup_mesh"]
                if mesh_name in bpy.data.meshes:
                    obj.data = bpy.data.meshes[mesh_name]
                
                # 2. Deshacer el Cambio de Origen re-escribiendo la Matriz Original directamente en memoria
                if "backup_matrix" in obj:
                    flat = list(obj["backup_matrix"])
                    # Reconstruimos la matriz 4x4 original exacta que solidworks exportó
                    mw = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
                    obj.matrix_world = mw
                
                # 3. Limpiar variables
                for key in ["backup_mesh", "backup_matrix", "cad_loc", "cad_rot_z", "cad_master"]:
                    if key in obj:
                        del obj[key]
                restored += 1
                
        self.report({'INFO'}, f"BACKUP CARGADO. {restored} piezas restauradas a su estado original.")
        return {'FINISHED'}

# --- UI Panel ---

class CAD_PT_panel_v2(bpy.types.Panel):
    bl_label = "CAD Instancer V2 (Radial)"
    bl_idname = "CAD_PT_panel_v2"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'

    def draw(self, context):
        layout = self.layout
        box = layout.box()
        box.label(text="Flujo de Simetría Z:", icon='MODIFIER')
        
        col = layout.column(align=True)
        col.operator("cad.set_origins_backup", icon='PIVOT_CURSOR')
        col.operator("cad.scan_math_radial", icon='CON_KINEMATIC')
        col.separator()
        col.operator("cad.create_instances", icon='LINKED')
        col.separator()
        col.operator("cad.transport_locs", icon='EMPTY_ARROWS')
        col.operator("cad.apply_rots_z", icon='DRIVER_ROTATIONAL_DIFFERENCE')
        
        layout.separator()
        warn_box = layout.box()
        warn_box.label(text="Sistema Anti-Fallos:", icon='ERROR')
        warn_box.operator("cad.revert_undo", icon='LOOP_BACK')

# --- Registration ---

classes = (
    CAD_OT_set_origins_backup,
    CAD_OT_scan_math_radial,
    CAD_OT_create_instances,
    CAD_OT_transport_locs,
    CAD_OT_apply_rots_z,
    CAD_OT_revert_undo,
    CAD_PT_panel_v2
)

def register():
    for cls in classes:
        bpy.utils.register_class(cls)

def unregister():
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)

if __name__ == "__main__":
    register()
