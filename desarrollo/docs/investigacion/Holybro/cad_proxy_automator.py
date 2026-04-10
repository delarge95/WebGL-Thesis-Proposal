import bpy
import bmesh
import numpy as np
import mathutils
import math

bl_info = {
    "name": "CAD Fastener Proxy Automator",
    "author": "Antigravity",
    "version": (1, 0),
    "blender": (4, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Transforma tornillería High-Poly en proxies modulares LOD1 (12 lados).",
    "category": "Object",
}

def get_pca_axes(verts):
    cov = np.cov(verts, rowvar=False)
    evals, evecs = np.linalg.eigh(cov)
    idx = np.argsort(evals)[::-1]
    evecs = evecs[:, idx]
    if np.linalg.det(evecs) < 0: 
        evecs[:, 2] = -evecs[:, 2]
    return evecs

def analyze_fastener(obj):
    # Obtener vértices evaluados (con transformaciones)
    depsgraph = bpy.context.evaluated_depsgraph_get()
    eval_obj = obj.evaluated_get(depsgraph)
    mesh = eval_obj.to_mesh()
    
    raw_verts = np.array([obj.matrix_world @ v.co for v in mesh.vertices])
    if len(raw_verts) == 0:
        eval_obj.to_mesh_clear()
        return None
        
    cm = np.mean(raw_verts, axis=0)
    centered_verts = raw_verts - cm
    
    evecs = get_pca_axes(centered_verts)
    local_verts = np.dot(centered_verts, evecs)
    
    z_min, z_max = np.min(local_verts[:, 2]), np.max(local_verts[:, 2])
    total_len = z_max - z_min
    
    bins = 20
    hist_r = np.zeros(bins)
    bin_size = total_len / bins
    
    for v in local_verts:
        r = math.sqrt(v[0]**2 + v[1]**2)
        idx = min(int((v[2] - z_min) / bin_size), bins - 1)
        if r > hist_r[idx]:
            hist_r[idx] = r
            
    head_radius = np.max(hist_r)
    is_head = hist_r > head_radius * 0.75
    head_indices = np.where(is_head)[0]
    
    if len(head_indices) == 0:
        head_indices = [bins-1]
        
    head_is_at_max_z = np.mean(head_indices) > (bins / 2)
    
    if head_is_at_max_z:
        shaft_bins = hist_r[:head_indices[0]]
        z_head_start = z_min + head_indices[0] * bin_size
        z_head_end = z_max
        z_shaft_start = z_min
    else:
        shaft_bins = hist_r[head_indices[-1]+1:]
        z_head_start = z_min
        z_head_end = z_min + (head_indices[-1]+1) * bin_size
        z_shaft_start = z_max
        
    shaft_radius = np.mean(shaft_bins) if len(shaft_bins) > 0 else head_radius * 0.5
    eval_obj.to_mesh_clear()
    
    return {
        "head_radius": head_radius,
        "shaft_radius": shaft_radius,
        "z_head_start": z_head_start,
        "z_head_end": z_head_end,
        "z_shaft_start": z_shaft_start,
        "head_is_at_max_z": head_is_at_max_z,
        "cm": cm,
        "evecs": evecs
    }

def create_proxy_mesh(name, data, proxy_type="SOCKET"):
    bm = bmesh.new()
    
    hr = data["head_radius"]
    sr = data["shaft_radius"]
    zs = data["z_shaft_start"]
    zhs = data["z_head_start"]
    zhe = data["z_head_end"]
    
    flip = not data["head_is_at_max_z"]
    
    # Calcular proyecciones locales a Z=0 para el perfil
    if flip:
        shaft_z = zs - zhe
        head_base = zhs - zhe
        head_top = 0
    else:
        shaft_z = zs - zs
        head_base = zhs - zs
        head_top = zhe - zs
        
    L_shaft = abs(head_base - shaft_z)
    L_head = abs(head_top - head_base)
    
    # Generar perfil radial
    prof = []
    if proxy_type == "SOCKET":
        prof = [(0, 0), (sr, 0), (sr, L_shaft), (hr, L_shaft), (hr, L_shaft + L_head), (0, L_shaft + L_head)]
    elif proxy_type == "BUTTON":
        prof = [(0, 0), (sr, 0), (sr, L_shaft), (hr, L_shaft), 
                (hr, L_shaft + L_head*0.5), (hr*0.7, L_shaft + L_head*0.8), (0, L_shaft + L_head)]
    elif proxy_type == "FLAT":
        prof = [(0, 0), (sr, 0), (sr, L_shaft), (hr, L_shaft + L_head), (0, L_shaft + L_head)]
        
    profile_edges = []
    profile_verts = []
    
    for r, z in prof:
        v = bm.verts.new((r, 0, z if not flip else -z))
        profile_verts.append(v)
        if len(profile_verts) > 1:
            profile_edges.append(bm.edges.new((profile_verts[-2], profile_verts[-1])))
            
    # Girar perfil (Solid of Revolution)
    sides = 12
    bmesh.ops.spin(
        bm,
        geom=profile_verts + profile_edges,
        cent=(0, 0, 0),
        axis=(0, 0, 1),
        dvec=(0, 0, 0),
        angle=math.pi * 2,
        steps=sides,
        use_duplicate=True
    )
    
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0.0001)
    
    # Transformación al mundo
    cm = data["cm"]
    evecs = data["evecs"]
    mat_rot = mathutils.Matrix(evecs).to_4x4()
    
    z_offset = zs if not flip else zhe
    offset_mat = mathutils.Matrix.Translation((0, 0, z_offset))
    final_mat = mathutils.Matrix.Translation(cm) @ mat_rot @ offset_mat
    
    bm.transform(final_mat)
    
    mesh = bpy.data.meshes.new(name)
    bm.to_mesh(mesh)
    bm.free()
    
    for p in mesh.polygons:
        p.use_smooth = True
        
    return mesh

class CAD_OT_proxy_automator(bpy.types.Operator):
    bl_idname = "cad.proxy_automator"
    bl_label = "Aplicar Proxies"
    bl_options = {'REGISTER', 'UNDO'}
    
    head_type: bpy.props.EnumProperty(
        name="Tipo Cabezote",
        items=[('AUTO', 'Automático', ''), 
               ('SOCKET', 'Socket (Cilíndrico)', ''), 
               ('BUTTON', 'Button (Domo)', ''), 
               ('FLAT', 'Flat (Cónico)', '')],
        default='AUTO'
    )
    
    @classmethod
    def poll(cls, context):
        return context.active_object is not None
        
    def execute(self, context):
        selected = [o for o in context.selected_objects if o.type == 'MESH' and not o.hide_viewport]
        if not selected:
            self.report({'WARNING'}, "No hay obj seleccionados")
            return {'CANCELLED'}
            
        count = 0
        for obj in selected:
            if "proxy_backup" not in obj:
                obj["proxy_backup"] = obj.data.name
                
            data = analyze_fastener(obj)
            if not data: continue
            
            t = self.head_type if self.head_type != 'AUTO' else 'SOCKET'
            
            proxy_mesh = create_proxy_mesh(f"M_{t}_{count}", data, t)
            
            # Asignar malla invirtiendo trans local (porque pusimos la malla procedural en Coordenadas Globales)
            proxy_mesh.transform(obj.matrix_world.inverted())
            
            obj.data = proxy_mesh
            length_mm = int(abs(data['z_shaft_start'] - data['z_head_start']) * 1000)
            obj.name = f"SYS_FASTENER_{t}_L{length_mm}"
            count += 1
            
        self.report({'INFO'}, f"Sustituidos {count} tornillos por proxies.")
        return {'FINISHED'}

class CAD_OT_revert_proxy(bpy.types.Operator):
    bl_idname = "cad.revert_proxy"
    bl_label = "Deshacer Proxies"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        for o in context.selected_objects:
            if "proxy_backup" in o and o["proxy_backup"] in bpy.data.meshes:
                old = o.data
                o.data = bpy.data.meshes[o["proxy_backup"]]
                if old.users == 0: bpy.data.meshes.remove(old)
                del o["proxy_backup"]
        return {'FINISHED'}

class CAD_PT_proxy_panel(bpy.types.Panel):
    bl_label = "Fastener Proxy Automator"
    bl_idname = "CAD_PT_proxy_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'
    def draw(self, context):
        layout = self.layout
        box = layout.box()
        box.label(text="Generar Proxies 12-Lados", icon='TOOL_SETTINGS')
        
        row = box.row()
        op1 = row.operator("cad.proxy_automator", text="Auto-Detectar")
        op1.head_type = 'AUTO'
        
        box.label(text="Forzar Tipo:")
        row = box.row()
        op2 = row.operator("cad.proxy_automator", text="Socket")
        op2.head_type = 'SOCKET'
        op3 = row.operator("cad.proxy_automator", text="Button")
        op3.head_type = 'BUTTON'
        
        row = box.row()
        op4 = row.operator("cad.proxy_automator", text="Flat")
        op4.head_type = 'FLAT'
        
        box.separator()
        box.operator("cad.revert_proxy", icon='LOOP_BACK')

def register():
    bpy.utils.register_class(CAD_OT_proxy_automator)
    bpy.utils.register_class(CAD_OT_revert_proxy)
    bpy.utils.register_class(CAD_PT_proxy_panel)

def unregister():
    bpy.utils.unregister_class(CAD_OT_proxy_automator)
    bpy.utils.unregister_class(CAD_OT_revert_proxy)
    bpy.utils.unregister_class(CAD_PT_proxy_panel)

if __name__ == "__main__":
    register()
