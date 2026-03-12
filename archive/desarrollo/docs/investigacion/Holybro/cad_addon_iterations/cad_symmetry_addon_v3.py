import bpy
import math
import re
import numpy as np
from mathutils import Matrix, Vector

bl_info = {
    "name": "CAD Symmetry Instancer V3 (Pro)",
    "author": "Antigravity (Engineer Mode)",
    "version": (3, 0),
    "blender": (3, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Auto-detección con tolerancia paramétrica + Alineación PCA Universal.",
    "category": "Object",
}

# --- PCA Logic for Universal 3D CAD Alignment ---

def get_pca_axes(verts_list):
    if len(verts_list) < 3:
        return Matrix.Identity(3)
    
    verts = np.array([v.co for v in verts_list])
    # Center
    verts -= verts.mean(axis=0)
    #协方差 covariance
    cov = np.cov(verts, rowvar=False)
    evals, evecs = np.linalg.eigh(cov)
    
    # Sort eigenvalues (major, medium, minor)
    idx = np.argsort(evals)[::-1]
    evecs = evecs[:, idx]
    
    # Fix sign ambiguity: make projection max magnitude positive
    for i in range(3):
        axis = evecs[:, i]
        projections = verts.dot(axis)
        if abs(projections.min()) > abs(projections.max()):
            evecs[:, i] = -axis

    # Force strict right-handed orthogonality
    x_axis = Vector(evecs[:, 0]).normalized()
    y_axis = Vector(evecs[:, 1]).normalized()
    z_axis = x_axis.cross(y_axis).normalized()
    # recompute Y to guarantee 90 degrees strictly
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
    match_name: bpy.props.BoolProperty(
        name="Match Base Name (Regex)",
        description="Filter matching using root names (e.g. Screw.001 == Screw)",
        default=True
    )
    match_vol: bpy.props.BoolProperty(
        name="Match Bounding Vol",
        description="Check Bounding Box Volume",
        default=True
    )
    tol_vol: bpy.props.FloatProperty(
        name="Vol Tol %",
        description="Tolerancia de porcentaje de volumen (1.0 = 1%)",
        default=5.0, min=0.0, max=100.0
    )
    match_verts: bpy.props.BoolProperty(
        name="Match Vertices",
        description="Check Vertex Count",
        default=True
    )
    tol_verts: bpy.props.FloatProperty(
        name="Vert Tol %",
        description="Tolerancia de vértices permitida (1.0 = 1%)",
        default=2.0, min=0.0, max=100.0
    )

# --- Auto Detect Operator ---
class CAD_OT_auto_detect(bpy.types.Operator):
    """Escanea la escena buscando copias idénticas paramétricamente al Maestro Activo"""
    bl_idname = "cad.auto_detect"
    bl_label = "Auto-Seleccionar Hermanos"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master or master.type != 'MESH':
            self.report({'ERROR'}, "Selecciona una malla Maestra activa.")
            return {'CANCELLED'}

        props = context.scene.cad_props
        bpy.ops.object.select_all(action='DESELECT')
        master.select_set(True)
        context.view_layer.objects.active = master

        master_base = get_base_name(master.name)
        master_vol = get_bounding_volume(master)
        master_vc = len(master.data.vertices)

        found_count = 0
        
        # Search all active mesh objects in view layer
        for obj in context.view_layer.objects:
            if obj == master or obj.type != 'MESH':
                continue
                
            match = True
            
            if props.match_name:
                obj_base = get_base_name(obj.name)
                if obj_base != master_base:
                    match = False
                    
            if match and props.match_vol:
                obj_vol = get_bounding_volume(obj)
                if master_vol > 0:
                    diff_pct = abs(obj_vol - master_vol) / master_vol * 100.0
                    if diff_pct > props.tol_vol:
                        match = False
                elif obj_vol > 0:
                    match = False

            if match and props.match_verts:
                obj_vc = len(obj.data.vertices)
                if master_vc > 0:
                    diff_pct = abs(obj_vc - master_vc) / master_vc * 100.0
                    if diff_pct > props.tol_verts:
                        match = False
                elif obj_vc > 0:
                    match = False
                    
            if match:
                obj.select_set(True)
                found_count += 1
                
        self.report({'INFO'}, f"Auto-Detect: {found_count} piezas hermanas encontradas.")
        return {'FINISHED'}

# --- Legacy Backup / Origins ---
class CAD_OT_set_origins_backup(bpy.types.Operator):
    bl_idname = "cad.set_origins_backup"
    bl_label = "1. Respaldar y Set Origins (CoM)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        for obj in context.selected_objects:
            if obj.type == 'MESH':
                obj["backup_mesh"] = obj.data.name
                flat = [v for row in obj.matrix_world for v in row]
                obj["backup_matrix"] = flat
        bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='BOUNDS')
        for obj in context.selected_objects:
            obj["cad_loc"] = obj.location.copy()
        return {'FINISHED'}

# --- PCA Scan Math ---
class CAD_OT_scan_math_pca(bpy.types.Operator):
    """Alineación Universal 3D (PCA) sin importar rotaciones CAD aleatorias!"""
    bl_idname = "cad.scan_math_pca"
    bl_label = "2. Escanear Orietaciones (PCA 3D)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        
        # Calculate Universal Master Axes via Principal Component Analysis
        M_master = get_pca_axes(master.data.vertices)
        
        calc_count = 0
        for obj in slaves:
            M_slave = get_pca_axes(obj.data.vertices)
            
            # Matriz de rotación relativa: R = M_slave * Inversa(M_master)
            # Esto captura cualquier rotación de 3 ejes, sea tornillo cabeza abajo o diagonal
            R = (M_slave @ M_master.inverted()).to_4x4()
            
            # Guardamos Flat array para mayor seguridad en los custom props
            flat_R = [v for row in R for v in row]
            obj["cad_rot_pca"] = flat_R
            obj["cad_master"] = master.name
            calc_count += 1
            
        self.report({'INFO'}, f"Matrices PCA extraídas para {calc_count} esclavas.")
        return {'FINISHED'}

# --- Create Instances ---
class CAD_OT_create_instances(bpy.types.Operator):
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
                obj.data = master.data # Instancia (Alt+D share mesh)
                purged += 1
        return {'FINISHED'}

# --- Apply Rotations ---
class CAD_OT_apply_rots_pca(bpy.types.Operator):
    bl_idname = "cad.apply_rots_pca"
    bl_label = "4. Restaurar Transforma PCA"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        master = context.active_object
        if not master: return {'CANCELLED'}
        slaves = [o for o in context.selected_objects if o != master]
        
        rotated = 0
        for obj in slaves:
            if "cad_loc" in obj:
                obj.location = obj["cad_loc"] # Restaurar centro de masa
                
            if "cad_rot_pca" in obj:
                try:
                    flat = list(obj["cad_rot_pca"])
                    R = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
                    
                    # La rotacion final del esclavo es su nueva rotacion relativa R
                    # multiplicada por la rotacion que ya tiene el maestro
                    loc, rot, sca = master.matrix_world.decompose()
                    new_rot = (R @ rot.to_matrix().to_4x4()).to_euler()
                    obj.rotation_euler = new_rot
                except Exception as e:
                    print(e)
                rotated += 1
                
        self.report({'INFO'}, f"{rotated} piezas orientadas espacialmente con éxito.")
        return {'FINISHED'}

class CAD_OT_revert_undo(bpy.types.Operator):
    """Devuelve todo al estado previo al Paso 1 usando las mallas de memoria y coordenadas"""
    bl_idname = "cad.revert_undo"
    bl_label = "RESTAURAR (Hard Undo)"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if not context.selected_objects: return {'CANCELLED'}
        for obj in context.selected_objects:
            if "backup_mesh" in obj:
                mesh_name = obj["backup_mesh"]
                if mesh_name in bpy.data.meshes:
                    obj.data = bpy.data.meshes[mesh_name]
                if "backup_matrix" in obj:
                    flat = list(obj["backup_matrix"])
                    obj.matrix_world = Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
                for key in ["backup_mesh", "backup_matrix", "cad_loc", "cad_rot_pca", "cad_master"]:
                    if key in obj: del obj[key]
        return {'FINISHED'}

# --- UI Panel ---
class CAD_PT_panel_v3(bpy.types.Panel):
    bl_label = "CAD Instancer V3 (Pro-Unity)"
    bl_idname = "CAD_PT_panel_v3"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'

    def draw(self, context):
        layout = self.layout
        props = context.scene.cad_props
        
        box1 = layout.box()
        box1.label(text="Detección Inteligente:", icon='VIEWZOOM')
        box1.prop(props, "match_name")
        
        row1 = box1.row()
        row1.prop(props, "match_vol")
        row1.prop(props, "tol_vol")
        
        row2 = box1.row()
        row2.prop(props, "match_verts")
        row2.prop(props, "tol_verts")
        
        col1 = box1.column()
        col1.operator("cad.auto_detect", icon='RESTRICT_SELECT_OFF')
        
        layout.separator()
        
        box2 = layout.box()
        box2.label(text="Registro Universal 3D (PCA):", icon='MODIFIER')
        box2.label(text="Soporta tornillos, giros y asimetría", icon='INFO')
        
        col2 = box2.column(align=True)
        col2.operator("cad.set_origins_backup", icon='PIVOT_CURSOR')
        col2.operator("cad.scan_math_pca", icon='CON_KINEMATIC')
        col2.separator()
        col2.operator("cad.create_instances", icon='LINKED')
        col2.operator("cad.apply_rots_pca", icon='FILE_REFRESH')
        
        layout.separator()
        warn_box = layout.box()
        warn_box.operator("cad.revert_undo", icon='LOOP_BACK')

# --- Registration ---
classes = (
    CAD_Properties,
    CAD_OT_auto_detect,
    CAD_OT_set_origins_backup,
    CAD_OT_scan_math_pca,
    CAD_OT_create_instances,
    CAD_OT_apply_rots_pca,
    CAD_OT_revert_undo,
    CAD_PT_panel_v3
)

def register():
    for cls in classes:
        bpy.utils.register_class(cls)
    bpy.types.Scene.cad_props = bpy.props.PointerProperty(type=CAD_Properties)

def unregister():
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)
    del bpy.types.Scene.cad_props

if __name__ == "__main__":
    register()
