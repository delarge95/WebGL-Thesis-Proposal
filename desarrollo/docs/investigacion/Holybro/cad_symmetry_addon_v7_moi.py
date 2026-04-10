import bpy
import numpy as np
import mathutils
import math
import time

bl_info = {
    "name": "CAD Symmetry Instancer V7 (MoI3D ICP)",
    "author": "Antigravity",
    "version": (7, 0),
    "blender": (4, 0, 0),
    "location": "View3D > N Panel > Holybro CAD",
    "description": "Instancer para topologías irregulares de MoI3D usando ICP y PCA.",
    "category": "Object",
}

# --- ICP ALGORITHM ---
def get_pca_axes(verts):
    cov = np.cov(verts, rowvar=False)
    evals, evecs = np.linalg.eigh(cov)
    idx = np.argsort(evals)[::-1]
    evecs = evecs[:, idx]
    if np.linalg.det(evecs) < 0: 
        evecs[:, 2] = -evecs[:, 2]
    return evecs

def align_icp(v_master, v_slave, max_iter=30):
    pca_m = get_pca_axes(v_master)
    pca_s = get_pca_axes(v_slave)
    
    R_init = mathutils.Matrix(pca_m.T).inverted() @ mathutils.Matrix(pca_s.T)
    R_init = R_init.to_3x3()
    
    kd = mathutils.kdtree.KDTree(len(v_master))
    for i, v in enumerate(v_master): kd.insert(v, i)
    kd.balance()
    
    flip_x = mathutils.Matrix.Rotation(math.pi, 3, 'X')
    flip_y = mathutils.Matrix.Rotation(math.pi, 3, 'Y')
    flip_z = mathutils.Matrix.Rotation(math.pi, 3, 'Z')
    
    best_R = None
    best_err = float('inf')
    
    # Probar varias orientaciones iniciales (Screws pueden estar invertidos 180 grados por PCA)
    test_rots = [
        R_init,
        flip_x @ R_init,
        flip_y @ R_init,
        flip_z @ R_init
    ]
    
    for R_start in test_rots:
        R = R_start.copy()
        current_slave = [R @ mathutils.Vector(v) for v in v_slave]
        
        for _ in range(max_iter):
            src_points = []
            dst_points = []
            for i, sv in enumerate(current_slave):
                co, index, dist = kd.find(sv)
                src_points.append(sv)
                dst_points.append(co)
                
            src = np.array(src_points)
            dst = np.array(dst_points)
            H = np.dot(src.T, dst)
            U, S, Vt = np.linalg.svd(H)
            R_step = np.dot(Vt.T, U.T)
            if np.linalg.det(R_step) < 0:
                Vt[2,:] *= -1
                R_step = np.dot(Vt.T, U.T)
            
            R_mat = mathutils.Matrix(R_step)
            R = R_mat @ R
            current_slave = [R_mat @ v for v in current_slave]
            
        err = 0
        for sv in v_slave:
            tv = R @ mathutils.Vector(sv)
            co, index, dist = kd.find(tv)
            err += dist
        
        if err < best_err:
            best_err = err
            best_R = R
            
    return best_R, best_err

# --- OPERATORS ---

class CAD_OT_icp_instancer(bpy.types.Operator):
    bl_idname = "cad.icp_instancer"
    bl_label = "Instanciar (MoI3D ICP)"
    bl_description = "Alinea y vincula mallas inconsistentes exportadas desde CAD."
    bl_options = {'REGISTER', 'UNDO'}
    
    @classmethod
    def poll(cls, context):
        return context.active_object is not None and context.active_object.type == 'MESH' and len(context.selected_objects) > 1

    def execute(self, context):
        master = context.active_object
        slaves = [obj for obj in context.selected_objects if obj != master and obj.type == 'MESH']
        
        if not slaves:
            self.report({'WARNING'}, "Selecciona al menos un esclavo.")
            return {'CANCELLED'}
        
        # Opcional: Centrar origenes si no lo están. MoI3D parts suelen estar al 0 pero desplazados,
        # sin embargo, en nuestro test vimos que el local center of mass es (0,0,0) asegurando que ya
        # tienen el origen en el centroide de la geometria. Lo dejaremos intacto por seguridad.
        
        v_master_raw = np.array([v.co for v in master.data.vertices])
        cm_master = np.mean(v_master_raw, axis=0)
        v_master = v_master_raw - cm_master
        
        count = 0
        start_time = time.time()
        
        for slave in slaves:
            # Backup
            if "backup_mesh" not in slave:
                bm = slave.data.copy()
                bm.name = slave.data.name + "_BACKUP"
                slave["backup_mesh"] = bm.name
                slave["backup_matrix"] = [v for r in slave.matrix_world for v in r]
            
            v_slave_raw = np.array([v.co for v in slave.data.vertices])
            cm_slave = np.mean(v_slave_raw, axis=0)
            v_slave = v_slave_raw - cm_slave
            
            # Chequear que la cantidad de vertices no sea ridículamente diferente
            if abs(len(v_slave_raw) - len(v_master_raw)) > len(v_master_raw) * 0.5:
                self.report({'WARNING'}, f"Saltando {slave.name}: dif de vértices masiva.")
                continue
                
            R, err = align_icp(v_master, v_slave)
            
            # Intercambiamos la malla al maestro
            slave.data = master.data
            
            # v_master = R @ v_slave  =>  v_slave = R.inv() @ v_master
            R_inv_4x4 = R.inverted().to_4x4()
            
            # Compensar Centros de Masa locales en la matriz si es necesario
            # S_world_new @ v_master_raw == S_world_old @ v_slave_raw
            # Sabemos que v_slave = R_inv @ v_master
            # S_world_old @ (v_slave + cm_slave) == S_world_new @ (v_master + cm_master)
            
            # Por simplicidad, y ya que vimos que cm = (0,0,0) en Holybro 3D MoI:
            trans_local = mathutils.Matrix.Translation(mathutils.Vector(cm_slave) - R_inv_4x4 @ mathutils.Vector(cm_master))
            
            slave.matrix_world = slave.matrix_world @ trans_local @ R_inv_4x4
            count += 1
            
        t = time.time() - start_time
        self.report({'INFO'}, f"ICP Finalizado: {count} instancias creadas en {t:.2f}s.")
        return {'FINISHED'}

class CAD_OT_revert_undo_v7(bpy.types.Operator):
    bl_idname = "cad.revert_undo_v7"
    bl_label = "RESTAURAR MESH ORIGINAL"
    bl_options = {'REGISTER', 'UNDO'}
    def execute(self, context):
        for o in context.selected_objects:
            if "backup_mesh" in o and o["backup_mesh"] in bpy.data.meshes:
                old = o.data
                o.data = bpy.data.meshes[o["backup_mesh"]]
                if old.users == 0 and old.name.endswith("_BACKUP") == False: 
                    bpy.data.meshes.remove(old)
            if "backup_matrix" in o:
                flat = list(o["backup_matrix"])
                o.matrix_world = mathutils.Matrix((flat[0:4], flat[4:8], flat[8:12], flat[12:16]))
            for k in ["backup_mesh", "backup_matrix"]:
                if k in o: del o[k]
        self.report({'INFO'}, "Mallas Restauradas")
        return {'FINISHED'}

# --- PANEL ---
class CAD_PT_panel_v7(bpy.types.Panel):
    bl_label = "CAD Instancer V7 (MoI3D ICP)"
    bl_idname = "CAD_PT_panel_v7"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'
    def draw(self, context):
        l = self.layout
        
        b = l.box()
        b.label(text="Resolutor de Inconsistencias (Para MoI3D)", icon='MESH_DATA')
        b.label(text="Master = Objeto Activo (Selecciónelos todos).")
        b.operator("cad.icp_instancer", icon='LINKED', text="Ejecutar ICP a Instancias")
        
        l.separator()
        l.box().operator("cad.revert_undo_v7", icon='LOOP_BACK')

classes = (CAD_OT_icp_instancer, CAD_OT_revert_undo_v7, CAD_PT_panel_v7)
def register():
    for c in classes: bpy.utils.register_class(c)
def unregister():
    for c in reversed(classes): bpy.utils.unregister_class(c)
if __name__ == "__main__": register()
