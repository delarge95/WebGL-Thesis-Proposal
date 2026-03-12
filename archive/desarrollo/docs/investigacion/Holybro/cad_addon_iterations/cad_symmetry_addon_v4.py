import bpy
import math
import re
import numpy as np
from mathutils import Matrix, Vector

bl_info = {
    "name": "CAD Symmetry Instancer V4",
    "author": "Antigravity",
    "version": (4, 0),
    "blender": (4, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Hard-Undo reparado y estrategias de rotación (Neutral, Radial, PCA) para simetría.",
    "category": "Object",
}

# --- PCA Logic for Universal 3D CAD Alignment ---
def get_pca_axes(verts_list):
    if len(verts_list) < 3: return Matrix.Identity(3)
    verts = np.array([v.co for v in verts_list])
    verts -= verts.mean(axis=0)
    cov = np.cov(verts, rowvar=False)
    evals, evecs = np.linalg.eigh(cov)
    idx = np.argsort(evals)[::-1]
    evecs = evecs[:, idx]
    for i in range(3):
        axis = evecs[:, i]
        projections = verts.dot(axis)
        if abs(projections.min()) > abs(projections.max()):
            evecs[:, i] = -axis

    x_axis = Vector(evecs[:, 0]).normalized()
    y_axis = Vector(evecs[:, 1]).normalized()
    z_axis = x_axis.cross(y_axis).normalized()
    y_axis = z_axis.cross(x_axis).normalized()
    return Matrix((x_axis, y_axis, z_axis)).transposed()

# --- Utility Functions ---
def get_base_name(name):
    match = re.search(r"^(.*?)(?:[\._-]\d+|\(\d+\))?$", name)
    return match.group(1) if match else name

def get_bounding_volume(obj):
    d = obj.dimensions
    return d[0] * d[1] * d[2]

# --- UI Properties ---
class CAD_Properties(bpy.types.PropertyGroup):
    match_name: bpy.props.BoolProperty(name="Nombre Base (Regex)", default=True)
    match_vol: bpy.props.BoolProperty(name="Volumen Limítrofe", default=True)
    tol_vol: bpy.props.FloatProperty(name="Tolerancia Vol %", default=5.0, min=0.0, max=100.0)
    match_verts: bpy.props.BoolProperty(name="Conteo de Vértices", default=True)
    tol_verts: bpy.props.FloatProperty(name="Tolerancia Verts %", default=2.0, min=0.0, max=100.0)
    
    rot_strategy: bpy.props.EnumProperty(
        name="Lógica de Rotación",
        description="Elige cómo debe calcularse el giro de las copias instanciadas respecto al Centro del Dron",
        items=[
            ('NONE', "Absoluta (Tornillos / Eje Z Fijo)", "Las piezas mantienen la rotación del padre exacta. Ideal para tornillos que apuntan hacia abajo en todos los brazos."),
            ('RADIAL_Z', "Radial Z (Motores / Brazos)", "Asume giro en Z alrededor del centro del dron. Ideal para piezas rotadas 90/180/270 grados en los brazos."),
            ('PCA_3D', "PCA Universal (Asimétrico)", "Mide componentes matemáticos. Se confunde con cilindros perfectos, usar sólo en mallas complejas y torcidas.")
        ],
        default='NONE'
    )

# --- 0. Auto Detect ---
class CAD_OT_auto_detect(bpy.types.Operator):
    bl_idname = "cad.auto_detect"
    bl_label = "0. Auto-Seleccionar Hermanas"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master or master.type != 'MESH': return {'CANCELLED'}
        props = context.scene.cad_props
        bpy.ops.object.select_all(action='DESELECT')
        master.select_set(True)
        context.view_layer.objects.active = master
        master_base = get_base_name(master.name)
        master_vol = get_bounding_volume(master)
        master_vc = len(master.data.vertices)
        
        found = 0
        for obj in context.view_layer.objects:
            if obj == master or obj.type != 'MESH': continue
            match = True
            if props.match_name and get_base_name(obj.name) != master_base: match = False
            if match and props.match_vol and master_vol > 0:
                if abs(get_bounding_volume(obj) - master_vol) / master_vol * 100.0 > props.tol_vol: match = False
            if match and props.match_verts and master_vc > 0:
                if abs(len(obj.data.vertices) - master_vc) / master_vc * 100.0 > props.tol_verts: match = False
            if match:
                obj.select_set(True)
                found += 1
        return {'FINISHED'}

# --- 1. Backup & Origins ---
class CAD_OT_set_origins_backup(bpy.types.Operator):
    bl_idname = "cad.set_origins_backup"
    bl_label = "1. Respaldar y Centrar Origen"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        for obj in context.selected_objects:
            if obj.type == 'MESH':
                # Deep Copy of the Mesh Data! This is why Undo broke before.
                backup_mesh = obj.data.copy()
                backup_mesh.name = obj.data.name + "_BACKUP"
                obj["backup_mesh"] = backup_mesh.name
                
                # Copy exact 4x4 matrix mapping
                flat = [v for row in obj.matrix_world for v in row]
                obj["backup_matrix"] = flat
                
        # Move origins (mutates current mesh, but backup is safe)
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        
        for obj in context.selected_objects:
            obj["cad_loc"] = obj.location.copy()
        return {'FINISHED'}

# --- 2. Escaneo Matemático ---
class CAD_OT_scan_math(bpy.types.Operator):
    bl_idname = "cad.scan_math"
    bl_label = "2. Extraer Matemática de Giro"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        props = context.scene.cad_props
        
        M_master_pca = None
        if props.rot_strategy == 'PCA_3D':
            M_master_pca = get_pca_axes(master.data.vertices)
            
        for obj in slaves:
            obj["cad_master"] = master.name
            obj["cad_strategy"] = props.rot_strategy
            
            if props.rot_strategy == 'NONE':
                # Las copias conservaran la rotación exacta del maestro. Especial para tornillos que apuntan igual.
                obj["cad_rot_x"] = 0.0
                obj["cad_rot_y"] = 0.0
                obj["cad_rot_z"] = 0.0
                
            elif props.rot_strategy == 'RADIAL_Z':
                vec_m = Vector((master.location.x, master.location.y))
                vec_s = Vector((obj.location.x, obj.location.y))
                if vec_m.length < 1e-4 or vec_s.length < 1e-4: continue
                ang_diff = math.atan2(vec_s.y, vec_s.x) - math.atan2(vec_m.y, vec_m.x)
                obj["cad_rot_x"] = 0.0
                obj["cad_rot_y"] = 0.0
                obj["cad_rot_z"] = ang_diff
                
            elif props.rot_strategy == 'PCA_3D':
                M_slave = get_pca_axes(obj.data.vertices)
                R = (M_slave @ M_master_pca.inverted()).to_4x4()
                flat_R = [v for row in R for v in row]
                obj["cad_rot_pca"] = flat_R
                
        self.report({'INFO'}, f"Estrategia [{props.rot_strategy}] analizada.")
        return {'FINISHED'}

# --- 3. Instancias ---
class CAD_OT_create_instances(bpy.types.Operator):
    bl_idname = "cad.create_instances"
    bl_label = "3. Purgar y Unir al Maestro"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        for obj in slaves:
            if "cad_master" in obj and obj["cad_master"] == master.name:
                obj.data = master.data
        return {'FINISHED'}

# --- 4. Restaurar Transforma ---
class CAD_OT_apply_rots(bpy.types.Operator):
    bl_idname = "cad.apply_rots"
    bl_label = "4. Aplicar Transforma y Rotación"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        
        for obj in slaves:
            if "cad_loc" in obj:
                obj.location = obj["cad_loc"]
                
            strat = obj.get("cad_strategy", 'NONE')
            if strat == 'NONE' or strat == 'RADIAL_Z':
                # Empezar con la rotación del Maestro (las piezas CAD absolutas tienen la misma rot)
                new_coords = list(master.rotation_euler)
                
                # Sumarle la desviación Z si la estrategia era Radial
                new_coords[2] += obj.get("cad_rot_z", 0.0)
                
                obj.rotation_euler = tuple(new_coords)
                
            elif strat == 'PCA_3D':
                if "cad_rot_pca" in obj:
                    try:
                        flat = list(obj["cad_rot_pca"])
                        R = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
                        loc, rot, sca = master.matrix_world.decompose()
                        new_rot = (R @ rot.to_matrix().to_4x4()).to_euler()
                        obj.rotation_euler = new_rot
                    except Exception: pass
        return {'FINISHED'}

# --- UNDO HARD ---
class CAD_OT_revert_undo(bpy.types.Operator):
    bl_idname = "cad.revert_undo"
    bl_label = "RESTAURAR (Hard Undo)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        for obj in context.selected_objects:
            if "backup_mesh" in obj:
                # Recuperar Deep Copy
                mesh_name = obj["backup_mesh"]
                if mesh_name in bpy.data.meshes:
                    old_mesh = obj.data
                    obj.data = bpy.data.meshes[mesh_name]
                    # Limpiar la basura
                    if old_mesh.users == 0:
                        bpy.data.meshes.remove(old_mesh)
                        
                # Recuperar Matriz
                if "backup_matrix" in obj:
                    flat = list(obj["backup_matrix"])
                    obj.matrix_world = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
                    
                for key in ["backup_mesh", "backup_matrix", "cad_loc", "cad_rot_x", "cad_rot_y", "cad_rot_z", "cad_rot_pca", "cad_master", "cad_strategy"]:
                    if key in obj: del obj[key]
        self.report({'INFO'}, "RESTAURADO EXITOSAMENTE AL ESTADO CAD")
        return {'FINISHED'}

# --- UI Panel ---
class CAD_PT_panel_v4(bpy.types.Panel):
    bl_label = "CAD Instancer V4 (Estrategias)"
    bl_idname = "CAD_PT_panel_v4"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'

    def draw(self, context):
        layout = self.layout
        props = context.scene.cad_props
        
        box1 = layout.box()
        box1.label(text="Filtros de Búsqueda:", icon='VIEWZOOM')
        box1.prop(props, "match_name")
        r1 = box1.row(); r1.prop(props, "match_vol"); r1.prop(props, "tol_vol")
        r2 = box1.row(); r2.prop(props, "match_verts"); r2.prop(props, "tol_verts")
        box1.operator("cad.auto_detect", icon='RESTRICT_SELECT_OFF')
        
        layout.separator()
        box2 = layout.box()
        box2.label(text="Resolución de Matrices:", icon='MODIFIER')
        box2.prop(props, "rot_strategy", expand=False)
        box2.label(text="Aplica 1 a 4 en orden:")
        
        c = box2.column(align=True)
        c.operator("cad.set_origins_backup", icon='PIVOT_CURSOR')
        c.operator("cad.scan_math", icon='CON_KINEMATIC')
        c.separator()
        c.operator("cad.create_instances", icon='LINKED')
        c.operator("cad.apply_rots", icon='FILE_REFRESH')
        
        layout.separator()
        box3 = layout.box()
        box3.label(text="Emergencia:", icon='ERROR')
        box3.operator("cad.revert_undo", icon='LOOP_BACK')

# --- Registration ---
classes = (
    CAD_Properties,CAD_OT_auto_detect,CAD_OT_set_origins_backup,
    CAD_OT_scan_math,CAD_OT_create_instances,CAD_OT_apply_rots,
    CAD_OT_revert_undo,CAD_PT_panel_v4
)

def register():
    for cls in classes: bpy.utils.register_class(cls)
    bpy.types.Scene.cad_props = bpy.props.PointerProperty(type=CAD_Properties)

def unregister():
    for cls in reversed(classes): bpy.utils.unregister_class(cls)
    del bpy.types.Scene.cad_props

if __name__ == "__main__":
    register()
