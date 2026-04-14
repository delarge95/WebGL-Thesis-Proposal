import bpy
import bmesh
import mathutils
import math

bl_info = {
    "name": "CAD Thread Retopo (BoltFactory Edition)",
    "author": "Antigravity",
    "version": (4, 0),
    "blender": (4, 0, 0),
    "location": "View3D > Edit Mode > N Panel",
    "description": "Reemplaza roscas CAD defectuosas con geometría BoltFactory limpia.",
    "category": "Mesh",
}


class CAD_OT_thread_retopo_bolt(bpy.types.Operator):
    bl_idname = "cad.thread_retopo_bolt"
    bl_label = "Inyectar Rosca Perfecta"
    bl_options = {'REGISTER', 'UNDO'}

    @classmethod
    def poll(cls, context):
        return context.mode == 'EDIT_MESH' and context.active_object is not None

    def execute(self, context):
        import numpy as np

        obj = context.active_object
        bm = bmesh.from_edit_mesh(obj.data)

        sel_verts = [v for v in bm.verts if v.select]
        if len(sel_verts) < 10:
            self.report({'WARNING'}, "Selecciona los vértices de la rosca defectuosa (mínimo 10).")
            return {'CANCELLED'}

        coords = np.array([v.co for v in sel_verts])
        cm = np.mean(coords, axis=0)

        # ── PASO 1: Detectar eje del cilindro ──────────────────────────
        # Para un cilindro, las dos direcciones radiales tienen varianza
        # similar y el eje longitudinal tiene varianza diferente.
        # Encontramos el eje cuya varianza es la "oveja negra".
        var_x = np.var(coords[:, 0])
        var_y = np.var(coords[:, 1])
        var_z = np.var(coords[:, 2])

        variances = np.array([var_x, var_y, var_z])
        axis_labels = ['X', 'Y', 'Z']

        # Calcular qué par tiene la razón de varianza más cercana a 1
        ratios = {}
        ratios[('X', 'Y')] = max(var_x, var_y) / max(min(var_x, var_y), 1e-30)
        ratios[('X', 'Z')] = max(var_x, var_z) / max(min(var_x, var_z), 1e-30)
        ratios[('Y', 'Z')] = max(var_y, var_z) / max(min(var_y, var_z), 1e-30)

        # El par con la razón más cercana a 1.0 son los ejes radiales
        best_pair = min(ratios, key=lambda k: abs(ratios[k] - 1.0))
        all_axes = {'X', 'Y', 'Z'}
        cyl_axis_label = (all_axes - set(best_pair)).pop()
        cyl_axis_idx = axis_labels.index(cyl_axis_label)

        radial_indices = [axis_labels.index(a) for a in best_pair]

        # ── PASO 2: Medir diámetro con Kasa Circle Fit ────────────────
        # Proyectar al plano perpendicular al eje
        u = coords[:, radial_indices[0]]
        v = coords[:, radial_indices[1]]
        w = coords[:, cyl_axis_idx]  # a lo largo del eje

        # Ajuste algebraico de círculo (Kasa)
        A = np.column_stack([2 * u, 2 * v, np.ones_like(u)])
        B = u ** 2 + v ** 2
        p, _, _, _ = np.linalg.lstsq(A, B, rcond=None)
        cu, cv, c_off = p
        radius = np.sqrt(max(c_off + cu ** 2 + cv ** 2, 0))

        diameter = radius * 2.0
        length = w.max() - w.min()
        w_min = w.min()

        # Centro 3D de la base del cilindro
        center_3d = np.copy(cm)
        center_3d[radial_indices[0]] = cu
        center_3d[radial_indices[1]] = cv
        center_3d[cyl_axis_idx] = w_min

        self.report({'INFO'},
            f"Medición: eje={cyl_axis_label}, diámetro={diameter:.4f}, largo={length:.4f}")

        # ── PASO 3: Destruir geometría defectuosa ──────────────────────
        # Borrar vértices seleccionados (y sus caras/aristas vinculadas)
        bmesh.ops.delete(bm, geom=sel_verts, context='VERTS')
        bmesh.update_edit_mesh(obj.data)
        bpy.ops.object.mode_set(mode='OBJECT')

        # ── PASO 4: Generar rosca con BoltFactory ─────────────────────
        try:
            import addon_utils
            addon_utils.enable("bl_ext.blender_org.boltfactory")
        except:
            pass

        if not hasattr(bpy.ops.mesh, 'bolt_add'):
            self.report({'ERROR'}, "Extensión BoltFactory no disponible.")
            bpy.ops.object.mode_set(mode='EDIT')
            return {'CANCELLED'}

        # BoltFactory trabaja en MILÍMETROS internamente (divide por 1000)
        # Nuestras medidas están en metros (unidades internas de Blender)
        # → Multiplicamos por 1000 para convertir m → mm
        dia_mm = diameter * 1000
        len_mm = length * 1000
        auto_pitch = dia_mm * 0.15
        minor_dia = dia_mm * 0.80

        try:
            bpy.ops.mesh.bolt_add(
                bf_Model_Type='bf_Model_Bolt',
                bf_Head_Type='bf_Head_None',
                bf_Bit_Type='bf_Bit_None',
                bf_Shank_Length=0,
                bf_Shank_Dia=dia_mm,
                bf_Thread_Length=len_mm,
                bf_Major_Dia=dia_mm,
                bf_Minor_Dia=minor_dia,
                bf_Pitch=auto_pitch,
                bf_Crest_Percent=10,
                bf_Root_Percent=10,
            )
        except Exception as e:
            self.report({'ERROR'}, f"BoltFactory falló: {e}")
            bpy.ops.object.mode_set(mode='EDIT')
            return {'CANCELLED'}

        bolt_obj = context.active_object

        # ── PASO 5: Posicionar y orientar el bolt ─────────────────────
        # BoltFactory genera el bolt a lo largo de +Z centrado en el origen.
        # Necesitamos rotarlo al eje correcto y trasladarlo al centro.

        # Rotación: de Z → eje detectado
        if cyl_axis_label == 'Z':
            rot_euler = mathutils.Euler((0, 0, 0))
        elif cyl_axis_label == 'X':
            rot_euler = mathutils.Euler((0, math.radians(90), 0))
        elif cyl_axis_label == 'Y':
            rot_euler = mathutils.Euler((math.radians(-90), 0, 0))

        rot_mat = rot_euler.to_matrix().to_4x4()
        trans_mat = mathutils.Matrix.Translation(mathutils.Vector(center_3d))

        # Componer: primero rotar, luego trasladar, todo en espacio local del objeto padre
        bolt_obj.matrix_world = obj.matrix_world @ trans_mat @ rot_mat

        bpy.context.view_layer.update()

        # ── PASO 6: Unir geometrías ──────────────────────────────────
        bolt_obj.select_set(True)
        obj.select_set(True)
        context.view_layer.objects.active = obj
        bpy.ops.object.join()

        bpy.ops.object.mode_set(mode='EDIT')
        self.report({'INFO'},
            f"✓ Rosca inyectada: Eje {cyl_axis_label}, Ø{diameter*1000:.1f}mm, L{length*1000:.1f}mm")

        return {'FINISHED'}


class CAD_PT_thread_panel(bpy.types.Panel):
    bl_label = "Thread Retopo Automator"
    bl_idname = "CAD_PT_thread_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'Holybro CAD'

    def draw(self, context):
        layout = self.layout
        layout.label(text="Retopología de Roscas CAD", icon='MOD_SCREW')
        if context.mode != 'EDIT_MESH':
            layout.label(text="Entra a Edit Mode y selecciona la rosca", icon='ERROR')
        else:
            layout.operator("cad.thread_retopo_bolt",
                            text="Inyectar Rosca Limpia", icon='MOD_BUILD')


def register():
    bpy.utils.register_class(CAD_OT_thread_retopo_bolt)
    bpy.utils.register_class(CAD_PT_thread_panel)


def unregister():
    bpy.utils.unregister_class(CAD_OT_thread_retopo_bolt)
    bpy.utils.unregister_class(CAD_PT_thread_panel)


if __name__ == "__main__":
    register()
