import bpy
import numpy as np
from mathutils import Matrix

bl_info = {
    "name": "CAD Symmetry Instancer V6 (Global Batch)",
    "author": "Antigravity",
    "version": (6, 0),
    "blender": (4, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Topología SVD Kabsch Global. Automatización 1-Click.",
    "category": "Object",
}

# --- KABSCH SVD ALGORITHM ---
def get_kabsch_transform(v_master, v_slave):
    # Take up to 100 vertices to compute exact rotation
    limit = min(len(v_master), 100)
    A = np.array([v.co for v in v_master[:limit]])
    B = np.array([v.co for v in v_slave[:limit]])
    
    cA = np.mean(A, axis=0)
    cB = np.mean(B, axis=0)
    
    AA = A - cA
    BB = B - cB
    
    H = np.dot(AA.T, BB)
    U, S, Vt = np.linalg.svd(H)
    R = np.dot(Vt.T, U.T)
    
    # Prevent reflection
    is_mirror = False
    if np.linalg.det(R) < 0:
        is_mirror = True
        Vt[2,:] *= -1
        R = np.dot(Vt.T, U.T)
        
    return R, is_mirror

# --- UTILS ---
import re
def get_base_name(n):
    m = re.search(r"^(.*?)(?:[\._-]\d+|\(\d+\))?$", n)
    return m.group(1) if m else n

def get_bounding_volume(obj):
    d = obj.dimensions
    return d[0] * d[1] * d[2]

class CAD_Properties(bpy.types.PropertyGroup):
    match_name: bpy.props.BoolProperty(name="Nombre Base (Regex)", default=True)
    match_vol: bpy.props.BoolProperty(name="Volumen Limite", default=True)
    tol_vol: bpy.props.FloatProperty(name="Tolerancia Vol %", default=5.0, min=0.0, max=100.0)
    match_verts: bpy.props.BoolProperty(name="Conteo Vértices", default=True)
    tol_verts: bpy.props.FloatProperty(name="Tolerancia Verts %", default=0.0, min=0.0, max=100.0)

# 0. Auto Detect
class CAD_OT_auto_detect(bpy.types.Operator):
    bl_idname = "cad.auto_detect"
    bl_label = "0. Auto-Seleccionar Hermanas"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        master = context.active_object
        if not master or master.type != 'MESH': return {'CANCELLED'}
        p = context.scene.cad_props
        bpy.ops.object.select_all(action='DESELECT')
        master.select_set(True)
        mb = get_base_name(master.name)
        mv = get_bounding_volume(master)
        mvc = len(master.data.vertices)
        found = 0
        for o in context.view_layer.objects:
            if o == master or o.type != 'MESH': continue
            match = True
            if p.match_name and get_base_name(o.name) != mb: match = False
            if match and p.match_vol and mv > 0 and abs(get_bounding_volume(o)-mv)/mv*100.0 > p.tol_vol: match = False
            if match and p.match_verts and mvc > 0 and abs(len(o.data.vertices)-mvc)/mvc*100.0 > p.tol_verts: match = False
            if match:
                o.select_set(True)
                found += 1
        return {'FINISHED'}

# 1. Origins & Backup
class CAD_OT_set_origins_backup(bpy.types.Operator):
    bl_idname = "cad.set_origins_backup"
    bl_label = "1. Respaldar y Centrar Origen"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        for o in context.selected_objects:
            if o.type == 'MESH':
                bm = o.data.copy()
                bm.name = o.data.name + "_BACKUP"
                o["backup_mesh"] = bm.name
                o["backup_matrix"] = [v for r in o.matrix_world for v in r]
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        for o in context.selected_objects: o["cad_loc"] = o.location.copy()
        return {'FINISHED'}

# 2. SVD Kabsch Scan
class CAD_OT_scan_math_kabsch(bpy.types.Operator):
    bl_idname = "cad.scan_math_kabsch"
    bl_label = "2. Análisis Topológico (SVD Kabsch)"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        calc = 0
        for obj in slaves:
            if len(obj.data.vertices) != len(master.data.vertices):
                self.report({'WARNING'}, f"Vértices no coinciden en {obj.name}. Ignorado.")
                continue
            R, is_mirrored = get_kabsch_transform(master.data.vertices, obj.data.vertices)
            obj["cad_rot_kabsch"] = [float(v) for r in R for v in r]
            obj["cad_is_mirrored"] = bool(is_mirrored)
            obj["cad_master"] = master.name
            calc += 1
        self.report({'INFO'}, f"Topología analizada matemáticamente en {calc} copias.")
        return {'FINISHED'}

# 3. Create Instances
class CAD_OT_create_instances(bpy.types.Operator):
    bl_idname = "cad.create_instances"
    bl_label = "3. Purgar y Unir al Maestro"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        s = [o for o in context.selected_objects if o != master]
        for o in s:
            if "cad_master" in o and o["cad_master"] == master.name: o.data = master.data
        return {'FINISHED'}

# 4. Apply Transforms
class CAD_OT_apply_rots_kabsch(bpy.types.Operator):
    bl_idname = "cad.apply_rots_kabsch"
    bl_label = "4. Aplicar Transforma Topológica"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        s = [o for o in context.selected_objects if o != master]
        for o in s:
            if "cad_loc" in o: o.location = o["cad_loc"]
            if "cad_rot_kabsch" in o:
                flat = list(o["cad_rot_kabsch"])
                R_3x3 = Matrix((flat[0:3], flat[3:6], flat[6:9]))
                # Convert 3x3 to 4x4
                R_4x4 = R_3x3.to_4x4()
                # Obtener la rotación antigua del esclavo (S_world_old)
                S_rot_4x4 = o.rotation_euler.to_matrix().to_4x4()
                
                # Multiplicar: Rotación Nueva = Rotación Antigua @ Delta Topológico
                new_rot_4x4 = S_rot_4x4 @ R_4x4
                o.rotation_euler = new_rot_4x4.to_euler()
                
                if o.get("cad_is_mirrored", False):
                    o.scale = (o.scale.x, o.scale.y, -abs(o.scale.z))
        return {'FINISHED'}

# UNDO
class CAD_OT_revert_undo(bpy.types.Operator):
    bl_idname = "cad.revert_undo"
    bl_label = "RESTAURAR (Hard Undo)"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        for o in context.selected_objects:
            if "backup_mesh" in o and o["backup_mesh"] in bpy.data.meshes:
                old = o.data
                o.data = bpy.data.meshes[o["backup_mesh"]]
                if old.users == 0: bpy.data.meshes.remove(old)
            if "backup_matrix" in o:
                flat = list(o["backup_matrix"])
                o.matrix_world = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
            for k in ["backup_mesh", "backup_matrix", "cad_loc", "cad_rot_kabsch", "cad_master", "cad_is_mirrored"]:
                if k in o: del o[k]
        return {'FINISHED'}

# --- BATCH GLOBAL V6 ---
class CAD_OT_batch_automate_all(bpy.types.Operator):
    bl_idname = "cad.batch_automate_all"
    bl_label = "AUTOMATIZACIÓN GLOBAL (1-Click Drone)"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        target_objs = [o for o in context.collection.objects if o.type == 'MESH' and not o.hide_viewport]
        if not target_objs: return {'CANCELLED'}
        
        # 1. Origins Center of Mass (All at once for identical triangulation)
        bpy.ops.object.select_all(action='DESELECT')
        for o in target_objs: o.select_set(True)
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        
        # 2. O(N) Hash Cluster
        groups = {}
        for obj in target_objs:
            bn = get_base_name(obj.name)
            vc = len(obj.data.vertices)
            key = f"{bn}_{vc}"
            if key not in groups: groups[key] = []
            groups[key].append(obj)
            
        fam_count = 0
        slave_count = 0
        
        # 3. Processing
        for key, objs in groups.items():
            if len(objs) < 2: continue
            master = min(objs, key=lambda o: sum(abs(v) for v in o.rotation_euler))
            slaves = [o for o in objs if o != master]
            for slave in slaves:
                R, is_mirrored = get_kabsch_transform(master.data.vertices, slave.data.vertices)
                R_4x4 = Matrix(R.tolist()).to_4x4()
                S_rot_4x4 = slave.rotation_euler.to_matrix().to_4x4()
                new_rot_4x4 = S_rot_4x4 @ R_4x4
                old_loc = slave.location.copy()
                
                slave.data = master.data
                slave.rotation_euler = new_rot_4x4.to_euler()
                slave.location = old_loc
                if is_mirrored:
                    slave.scale = (slave.scale.x, slave.scale.y, -abs(slave.scale.z))
                slave_count += 1
            fam_count += 1
            
        self.report({'INFO'}, f"Global Batch completado: {fam_count} Familias, {slave_count} Instancias.")
        return {'FINISHED'}

class CAD_PT_panel_v6(bpy.types.Panel):
    bl_label = "CAD Instancer V6 (Batch)"
    bl_idname = "CAD_PT_panel_v6"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'
    def draw(self, context):
        l = self.layout
        p = context.scene.cad_props
        
        b_batch = l.box()
        b_batch.label(text="BETA: Automatización Global O(N)", icon='WORLD')
        b_batch.operator("cad.batch_automate_all", icon='FILE_REFRESH', text="Mapear Dron Completo (1 Click)")
        
        l.separator()
        
        b1 = l.box()
        b1.label(text="Modo Step-by-Step (Individual):", icon='VIEWZOOM')
        b1.prop(p, "match_name")
        r = b1.row(); r.prop(p, "match_verts"); r.prop(p, "tol_verts")
        b1.operator("cad.auto_detect", icon='RESTRICT_SELECT_OFF')
        l.separator()
        b2 = l.box()
        b2.label(text="Matriz Exacta Universal:", icon='MODIFIER')
        c = b2.column(align=True)
        c.operator("cad.set_origins_backup", icon='PIVOT_CURSOR')
        c.operator("cad.scan_math_kabsch", icon='CON_KINEMATIC')
        c.separator()
        c.operator("cad.create_instances", icon='LINKED')
        c.operator("cad.apply_rots_kabsch", icon='FILE_REFRESH')
        l.separator()
        l.box().operator("cad.revert_undo", icon='LOOP_BACK')

classes = (CAD_Properties,CAD_OT_auto_detect,CAD_OT_set_origins_backup,CAD_OT_scan_math_kabsch,CAD_OT_create_instances,CAD_OT_apply_rots_kabsch,CAD_OT_batch_automate_all,CAD_OT_revert_undo,CAD_PT_panel_v6)
def register():
    for c in classes: bpy.utils.register_class(c)
    bpy.types.Scene.cad_props = bpy.props.PointerProperty(type=CAD_Properties)
def unregister():
    for c in reversed(classes): bpy.utils.unregister_class(c)
    del bpy.types.Scene.cad_props
if __name__ == "__main__": register()
