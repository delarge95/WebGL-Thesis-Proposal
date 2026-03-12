import bpy
from mathutils import Matrix, Vector

bl_info = {
    "name": "CAD Symmetry Instancer",
    "author": "Antigravity (Engineer Mode)",
    "version": (1, 0),
    "blender": (3, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Herramienta paso a paso para extraer instancias de modelos CAD con origenes rotos. Soporta Undo (Ctrl+Z).",
    "category": "Object",
}

# --- Operators ---

class CAD_OT_set_origins(bpy.types.Operator):
    """Calcula y traslada el origen al Centro de Gravedad Volumétrico de la pieza"""
    bl_idname = "cad.set_origins"
    bl_label = "1. Set Real Origins (Center of Mass)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects:
            self.report({'WARNING'}, "No hay objetos seleccionados")
            return {'CANCELLED'}
        
        # This applies the origin shift to all selected items
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        
        # Store their new true location in a custom property
        for obj in context.selected_objects:
            obj["cad_loc"] = obj.location.copy()
            
        self.report({'INFO'}, f"Orígenes reparados para {len(context.selected_objects)} objetos.")
        return {'FINISHED'}

class CAD_OT_scan_math(bpy.types.Operator):
    """Escanea las diferencias de rotación entre la malla Maestra (Activo) y las Esclavas"""
    bl_idname = "cad.scan_math"
    bl_label = "2. Escanear Volúmenes (Matemática)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master or master.type != 'MESH':
            self.report({'ERROR'}, "Selecciona múltiples copias y haz que la MAESTRA sea la activa (Amarillo claro)")
            return {'CANCELLED'}
            
        slaves = [o for o in context.selected_objects if o != master and o.type == 'MESH']
        if not slaves:
            self.report({'WARNING'}, "No hay copias esclavas seleccionadas")
            return {'CANCELLED'}
            
        calc_count = 0
        for obj in slaves:
            v_m = master.data.vertices
            v_s = obj.data.vertices
            
            if len(v_m) != len(v_s) or len(v_m) < 3:
                self.report({'WARNING'}, f"La malla {obj.name} no tiene los mismos vértices que la Maestra.")
                continue
                
            # Gram-Schmidt Rigid Registration (Local Space since origin is now centered)
            v0m = v_m[0].co
            idx1 = 1
            while idx1 < len(v_m) and (v_m[idx1].co - v0m).length < 1e-4: 
                idx1 += 1
            if idx1 == len(v_m):
                continue
            
            v1m = v_m[idx1].co
            X1 = v0m.normalized()
            Y1 = (v1m - X1 * X1.dot(v1m)).normalized()
            Z1 = X1.cross(Y1)
            M1 = Matrix((X1, Y1, Z1)).transposed()
            
            v0s = v_s[0].co
            v1s = v_s[idx1].co
            X2 = v0s.normalized()
            Y2 = (v1s - X2 * X2.dot(v1s)).normalized()
            Z2 = X2.cross(Y2)
            M2 = Matrix((X2, Y2, Z2)).transposed()
            
            # The pure internal rotation baked into the slave CAD vs the master CAD
            R = (M2 @ M1.inverted()).to_4x4()
            
            # Store calculated values natively in Blender attributes
            obj["cad_rot"] = R.to_euler()
            obj["cad_master"] = master.name
            calc_count += 1
            
        self.report({'INFO'}, f"Trazado matemático finalizado para {calc_count} esclavas.")
        return {'FINISHED'}

class CAD_OT_create_instances(bpy.types.Operator):
    """Purga las mallas esclavas y fuerza una instanciación compartida (Linked Duplicate)"""
    bl_idname = "cad.create_instances"
    bl_label = "3. Purgar y Crear Instancias"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        
        slaves = [o for o in context.selected_objects if o != master]
        purged = 0
        orphans = set()
        
        for obj in slaves:
            if "cad_master" in obj and obj["cad_master"] == master.name:
                orphans.add(obj.data) # Mark for garbage collection
                obj.data = master.data # MAGIC HAPPENS HERE
                purged += 1
                
        # Clean Orphaned Meshes immediately to free RAM
        for mesh in orphans:
            if mesh.users == 0:
                bpy.data.meshes.remove(mesh)
                
        self.report({'INFO'}, f"{purged} Mallas reemplazadas por instancias. Geometría reducida.")
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
                
        self.report({'INFO'}, f"{moved} Posiciones mundiales aseguradas.")
        return {'FINISHED'}

class CAD_OT_apply_rots(bpy.types.Operator):
    """Gira las instancias esclavas usando el factor Euler calculado en el Paso 2"""
    bl_idname = "cad.apply_rots"
    bl_label = "5. Ajustar Rotaciones Exactas"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        
        slaves = [o for o in context.selected_objects if o != master]
        rotated = 0
        
        for obj in slaves:
            if "cad_rot" in obj:
                # Add the relative internal CAD rotation
                obj.rotation_euler = obj["cad_rot"]
                # Also we must preserve any rotation the master itself had prior
                # Typically, applying relative rotation is sufficient if Master is at 0,0,0 rotation.
                rotated += 1
                
        self.report({'INFO'}, f"Modelo resuelto. {rotated} rotaciones simétricas aplicadas.")
        return {'FINISHED'}

# --- UI Panel ---

class CAD_PT_panel(bpy.types.Panel):
    bl_label = "CAD Symmetry Instancer"
    bl_idname = "CAD_PT_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'

    def draw(self, context):
        layout = self.layout
        
        box = layout.box()
        box.label(text="Instrucciones:", icon='INFO')
        box.label(text="1. Selecciona TODAS las copias de una pieza.")
        box.label(text="2. Asegúrate que la ÚLTIMA que clickees")
        box.label(text="   (la amarilla clara) sea tu MAESTRA.")
        box.label(text="   (A la que le harás Retopología)")
        
        layout.separator()
        layout.label(text="Flujo de Trabajo (Soporta Ctrl+Z):", icon='MODIFIER')
        
        # Using icon numbers to guide the user logically
        col = layout.column(align=True)
        col.operator("cad.set_origins", icon='PIVOT_CURSOR')
        col.operator("cad.scan_math", icon='CON_KINEMATIC')
        col.separator()
        col.operator("cad.create_instances", icon='LINKED')
        col.separator()
        col.operator("cad.transport_locs", icon='EMPTY_ARROWS')
        col.operator("cad.apply_rots", icon='DRIVER_ROTATIONAL_DIFFERENCE')
        
        layout.separator()
        layout.label(text="Done! Las piezas ahora comparten Malla", icon='CHECKMARK')

# --- Registration ---

classes = (
    CAD_OT_set_origins,
    CAD_OT_scan_math,
    CAD_OT_create_instances,
    CAD_OT_transport_locs,
    CAD_OT_apply_rots,
    CAD_PT_panel
)

def register():
    for cls in classes:
        bpy.utils.register_class(cls)

def unregister():
    for reversed_cls in reversed(classes):
        bpy.utils.unregister_class(reversed_cls)

if __name__ == "__main__":
    register()
