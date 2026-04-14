import bpy
import bmesh
import mathutils
import math

bl_info = {
    "name": "CAD Thread Retopo (BoltFactory Edition)",
    "author": "Antigravity",
    "version": (6, 0),
    "blender": (4, 0, 0),
    "location": "View3D > Edit Mode > N Panel",
    "description": "Reemplaza roscas CAD defectuosas con geometría BoltFactory limpia.",
    "category": "Mesh",
}


class CAD_OT_thread_retopo_bolt(bpy.types.Operator):
    bl_idname = "cad.thread_retopo_bolt"
    bl_label = "Inyectar Rosca Perfecta"
    bl_options = {'REGISTER', 'UNDO'}

    pitch_mm: bpy.props.FloatProperty(
        name="Pitch (mm)",
        description="Paso de rosca en mm. M3=0.5, M4=0.7, M5=0.8, M6=1.0, M8=1.25, M10=1.5",
        default=0.5,
        min=0.1,
        max=5.0,
        precision=2,
    )

    @classmethod
    def poll(cls, context):
        return context.mode == 'EDIT_MESH' and context.active_object is not None

    def execute(self, context):
        import numpy as np

        obj = context.active_object
        bm = bmesh.from_edit_mesh(obj.data)
        bm.edges.ensure_lookup_table()
        bm.faces.ensure_lookup_table()
        bm.verts.ensure_lookup_table()

        sel_faces = [f for f in bm.faces if f.select]
        if len(sel_faces) < 5:
            self.report({'WARNING'},
                "Selecciona TODAS las caras de la rosca (incluyendo tapas superior e inferior).")
            return {'CANCELLED'}

        # ═══════════════════════════════════════════════════════════════
        # PASO 1: EXTRAER BOUNDARY LOOPS de la selección
        # Las aristas donde una cara seleccionada toca una no-seleccionada
        # son exactamente el borde de conexión con el resto de la pieza.
        # ═══════════════════════════════════════════════════════════════
        boundary_edges = []
        for e in bm.edges:
            lf = e.link_faces
            if len(lf) == 2 and lf[0].select != lf[1].select:
                boundary_edges.append(e)

        if len(boundary_edges) < 6:
            self.report({'WARNING'},
                "No se encontraron bordes de conexión suficientes. "
                "¿Seleccionaste solo una parte de la rosca?")
            return {'CANCELLED'}

        # Agrupar boundary edges en loops por conectividad
        loops = self._find_edge_loops(boundary_edges)

        if len(loops) < 2:
            self.report({'WARNING'}, "Se necesitan al menos 2 boundary loops (tapa sup + inf).")
            return {'CANCELLED'}

        # ═══════════════════════════════════════════════════════════════
        # PASO 2: MEDIR las boundary loops
        # ═══════════════════════════════════════════════════════════════
        axis_labels = ['X', 'Y', 'Z']

        # Detectar eje cilíndrico por varianza de los verts seleccionados
        sel_coords = np.array([v.co for v in bm.verts if v.select])
        var_per_axis = [np.var(sel_coords[:, i]) for i in range(3)]
        ratios = {
            (0, 1): max(var_per_axis[0], var_per_axis[1]) / max(min(var_per_axis[0], var_per_axis[1]), 1e-30),
            (0, 2): max(var_per_axis[0], var_per_axis[2]) / max(min(var_per_axis[0], var_per_axis[2]), 1e-30),
            (1, 2): max(var_per_axis[1], var_per_axis[2]) / max(min(var_per_axis[1], var_per_axis[2]), 1e-30),
        }
        best_pair = min(ratios, key=lambda k: abs(ratios[k] - 1.0))
        cyl_axis_idx = ({0, 1, 2} - set(best_pair)).pop()
        radial_indices = list(best_pair)

        # Calcular info de cada boundary loop
        loop_data = []
        for loop_verts in loops:
            coords_l = np.array([v.co for v in loop_verts])
            w_val = np.mean(coords_l[:, cyl_axis_idx])
            u_vals = coords_l[:, radial_indices[0]]
            v_vals = coords_l[:, radial_indices[1]]

            # Kasa Circle Fit para centro y radio exactos
            A = np.column_stack([2 * u_vals, 2 * v_vals, np.ones_like(u_vals)])
            B = u_vals**2 + v_vals**2
            p, _, _, _ = np.linalg.lstsq(A, B, rcond=None)
            cu, cv, c_off = p
            r = np.sqrt(max(c_off + cu**2 + cv**2, 0))

            loop_data.append({
                'verts': loop_verts,
                'edges': [e for e in boundary_edges
                          if set(v.index for v in e.verts).issubset(
                              set(v.index for v in loop_verts))],
                'w': w_val,
                'radius': r,
                'cx': cu,
                'cy': cv,
                'count': len(loop_verts),
            })

        # Ordenar por posición en el eje (de menor a mayor)
        loop_data.sort(key=lambda x: x['w'])

        bottom_loop = loop_data[0]
        top_loop = loop_data[-1]

        thread_length = abs(top_loop['w'] - bottom_loop['w'])
        thread_cx = (bottom_loop['cx'] + top_loop['cx']) / 2
        thread_cy = (bottom_loop['cy'] + top_loop['cy']) / 2

        # ═══════════════════════════════════════════════════════════════
        # Extraer Major/Minor diameters.
        # Major (crestas): radio máximo de los vértices seleccionados.
        # Minor (raíz): radio de los boundary loops (en esta rosca CAD,
        # las raíces llegan exactamente al mismo radio que los boundaries).
        # ═══════════════════════════════════════════════════════════════
        sel_u = sel_coords[:, radial_indices[0]]
        sel_v = sel_coords[:, radial_indices[1]]
        sel_radii = np.sqrt((sel_u - thread_cx)**2 + (sel_v - thread_cy)**2)

        thread_r_max = np.percentile(sel_radii, 99)  # Major (crests)
        thread_r_min = min(bottom_loop['radius'], top_loop['radius'])  # Minor = boundary

        # Centro de la base en coordenadas locales 3D
        center_base = [0.0, 0.0, 0.0]
        center_base[radial_indices[0]] = thread_cx
        center_base[radial_indices[1]] = thread_cy
        center_base[cyl_axis_idx] = bottom_loop['w']
        center_base = mathutils.Vector(center_base)

        major_dia_mm = thread_r_max * 2 * 1000
        minor_dia_mm = thread_r_min * 2 * 1000
        len_mm = thread_length * 1000
        pitch_mm = self.pitch_mm

        self.report({'INFO'},
            f"Medición: Major Ø{major_dia_mm:.1f}mm, Minor Ø{minor_dia_mm:.1f}mm, "
            f"L={len_mm:.1f}mm, depth={((thread_r_max-thread_r_min)*1000):.2f}mm")

        # ═══════════════════════════════════════════════════════════════
        # PASO 3: BORRAR selección (la rosca vieja)
        # Los boundary verts quedan intactos porque pertenecen también
        # a caras no-seleccionadas.
        # ═══════════════════════════════════════════════════════════════
        bmesh.ops.delete(bm, geom=sel_faces, context='FACES')
        bmesh.update_edit_mesh(obj.data)
        bpy.ops.object.mode_set(mode='OBJECT')

        # ═══════════════════════════════════════════════════════════════
        # PASO 4: GENERAR BOLT con BoltFactory
        # ═══════════════════════════════════════════════════════════════
        try:
            import addon_utils
            addon_utils.enable("bl_ext.blender_org.boltfactory")
        except:
            pass

        if not hasattr(bpy.ops.mesh, 'bolt_add'):
            self.report({'ERROR'}, "Extensión BoltFactory no disponible.")
            bpy.ops.object.mode_set(mode='EDIT')
            return {'CANCELLED'}

        try:
            bpy.ops.mesh.bolt_add(
                bf_Model_Type='bf_Model_Bolt',
                bf_Head_Type='bf_Head_None',
                bf_Bit_Type='bf_Bit_None',
                bf_Shank_Length=0,
                bf_Shank_Dia=major_dia_mm,
                bf_Thread_Length=len_mm,
                bf_Major_Dia=major_dia_mm,
                bf_Minor_Dia=minor_dia_mm,
                bf_Pitch=pitch_mm,
                bf_Crest_Percent=50,
                bf_Root_Percent=0,
            )
        except Exception as e:
            self.report({'ERROR'}, f"BoltFactory falló: {e}")
            bpy.ops.object.mode_set(mode='EDIT')
            return {'CANCELLED'}

        bolt_obj = context.active_object
        bolt_mesh = bolt_obj.data

        # ═══════════════════════════════════════════════════════════════
        # PASO 4b: POST-PROCESAMIENTO del perfil de rosca
        # BoltFactory genera un perfil que puede tener radios intermedios.
        # Con Crest=50%, Root=0% ya tenemos un perfil cercano al trapezoidal,
        # pero snapeamos los radios intermedios para obtener:
        #   root(r_min) → [1 edge] → crest(r_max) → [1 edge] → root
        # Terminación: solo la primera cresta (bottom) se reduce parcialmente.
        # ═══════════════════════════════════════════════════════════════
        r_max_local = thread_r_max
        r_min_local = thread_r_min
        r_mid_local = (r_max_local + r_min_local) / 2

        bolt_z_min = min(v.co.z for v in bolt_mesh.vertices)
        bolt_z_max = max(v.co.z for v in bolt_mesh.vertices)
        pitch_local = pitch_mm / 1000.0  # en Blender units (metros)

        for vert in bolt_mesh.vertices:
            r = math.sqrt(vert.co.x**2 + vert.co.y**2)
            if r < 0.00001:
                continue

            if r > r_mid_local:
                # Vértice de cresta → snap a r_max
                target_r = r_max_local

                # Taper bottom: primera cresta parcial (50-80% del crest height)
                if vert.co.z < bolt_z_min + pitch_local:
                    t = (vert.co.z - bolt_z_min) / max(pitch_local, 1e-10)
                    t = max(0, min(1, t))
                    # Primera cresta alcanza ~70% de la altura total
                    partial_r = r_min_local + t * (r_max_local - r_min_local) * 0.7
                    target_r = min(target_r, max(partial_r, r_min_local))

                scale = target_r / r
                vert.co.x *= scale
                vert.co.y *= scale

        bolt_mesh.update()

        # ═══════════════════════════════════════════════════════════════
        # PASO 5: TRANSFORMAR vértices del bolt a LOCAL space
        # BoltFactory genera con eje Z, centrado en origen.
        # ═══════════════════════════════════════════════════════════════
        if cyl_axis_idx == 2:  # Z
            rot_mat = mathutils.Matrix.Identity(3)
        elif cyl_axis_idx == 0:  # X
            rot_mat = mathutils.Euler((0, math.radians(90), 0)).to_matrix()
        elif cyl_axis_idx == 1:  # Y
            rot_mat = mathutils.Euler((math.radians(-90), 0, 0)).to_matrix()

        for vert in bolt_mesh.vertices:
            rotated = rot_mat @ vert.co
            vert.co = rotated + center_base

        bolt_mesh.update()

        # ═══════════════════════════════════════════════════════════════
        # PASO 6: UNIR bolt con mesh
        # ═══════════════════════════════════════════════════════════════
        bolt_obj.matrix_world = obj.matrix_world.copy()
        bpy.context.view_layer.update()

        bolt_obj.select_set(True)
        obj.select_set(True)
        context.view_layer.objects.active = obj
        bpy.ops.object.join()

        # ═══════════════════════════════════════════════════════════════
        # PASO 7: AUTO-BRIDGE
        # Ahora el mesh tiene:
        # - Los 2 boundary loops originales (del PASO 1) que siguen intactos
        # - El bolt como isla flotante con sus propios boundary loops
        # Debemos:
        # a) Eliminar el cap inferior del bolt
        # b) Encontrar los boundary loops del bolt (top/bottom)
        # c) Escalar/alinear bolt loops con mesh loops
        # d) Bridge
        # ═══════════════════════════════════════════════════════════════
        bpy.ops.object.mode_set(mode='EDIT')
        bm = bmesh.from_edit_mesh(obj.data)
        bm.edges.ensure_lookup_table()
        bm.verts.ensure_lookup_table()
        bm.faces.ensure_lookup_table()

        # 7a. Eliminar cap faces del bolt
        # El cap está formado por caras INTERNAS del cilindro (dentro del minor radius)
        # cerca de la base del bolt.
        cap_faces = []
        cap_w_limit = center_base[cyl_axis_idx] + thread_length * 0.25
        minor_r_local = thread_r_min * 0.98  # un poco menos que minor para atrapar solo el cap
        for f in bm.faces:
            fc = [v.co for v in f.verts]
            fcu = np.mean([c[radial_indices[0]] for c in fc])
            fcv = np.mean([c[radial_indices[1]] for c in fc])
            fcw = np.mean([c[cyl_axis_idx] for c in fc])

            dist_r = math.sqrt((fcu - thread_cx)**2 + (fcv - thread_cy)**2)
            # Solo caras DENTRO del radio menor y cerca de la base
            if dist_r < minor_r_local and fcw < cap_w_limit:
                cap_faces.append(f)

        if cap_faces:
            bmesh.ops.delete(bm, geom=cap_faces, context='FACES')
            bmesh.update_edit_mesh(obj.data)
            bm = bmesh.from_edit_mesh(obj.data)
            bm.edges.ensure_lookup_table()
            bm.verts.ensure_lookup_table()

        # 7b. Encontrar TODOS los boundary loops actuales
        all_boundary = [e for e in bm.edges if not e.is_manifold and not e.is_wire]

        if len(all_boundary) < 4:
            self.report({'WARNING'}, "Insuficientes bordes abiertos para bridge.")
            return {'FINISHED'}

        all_loops = self._find_edge_loops(all_boundary)

        # 7c. Clasificar cada loop
        loop_info = []
        for lv in all_loops:
            coords_l = np.array([v.co for v in lv])
            w = np.mean(coords_l[:, cyl_axis_idx])
            cx_l = np.mean(coords_l[:, radial_indices[0]])
            cy_l = np.mean(coords_l[:, radial_indices[1]])
            radii = np.sqrt(
                (coords_l[:, radial_indices[0]] - cx_l)**2 +
                (coords_l[:, radial_indices[1]] - cy_l)**2)
            r = np.mean(radii)

            # Distancia del centro de este loop al centro del bolt
            dc = math.sqrt((cx_l - thread_cx)**2 + (cy_l - thread_cy)**2)

            loop_info.append({
                'verts': lv, 'w': w, 'radius': r, 'count': len(lv),
                'cx': cx_l, 'cy': cy_l, 'dist_center': dc,
            })

        # Solo considerar loops concéntricos con el bolt
        max_dist = bottom_loop['radius'] * 1.5
        concentric = [li for li in loop_info if li['dist_center'] < max_dist]

        if len(concentric) < 2:
            self.report({'WARNING'},
                f"Solo {len(concentric)} loop(s) concéntricos. Se necesitan ≥2.")
            return {'FINISHED'}

        # Separar mesh loops (del agujero original) y bolt loops
        # Los mesh loops tienen el mismo vertex count que las boundary originales
        mesh_target_count = bottom_loop['count']
        mesh_loops = [li for li in concentric if li['count'] == mesh_target_count]
        bolt_loops = [li for li in concentric if li['count'] != mesh_target_count]

        # Ordenar ambos por W
        mesh_loops.sort(key=lambda x: x['w'])
        bolt_loops.sort(key=lambda x: x['w'])

        # Emparejar: bottom bolt ↔ bottom mesh, top bolt ↔ top mesh
        pairs = []
        if bolt_loops and mesh_loops:
            # Bolt bottom → mesh bottom (lowest W)
            if len(bolt_loops) >= 1 and len(mesh_loops) >= 1:
                pairs.append((bolt_loops[0], mesh_loops[0]))
            # Bolt top → mesh top (highest W)
            if len(bolt_loops) >= 2 and len(mesh_loops) >= 2:
                pairs.append((bolt_loops[-1], mesh_loops[-1]))
            elif len(bolt_loops) >= 2 and len(mesh_loops) == 1:
                # Solo hay 1 mesh loop — intentar con el segundo bolt
                pass

        if not pairs:
            self.report({'WARNING'}, "No se pudieron emparejar loops.")
            return {'FINISHED'}

        # 7d. Escalar bolt loops y alinear W
        for bolt_li, mesh_li in pairs:
            scale_factor = mesh_li['radius'] / max(bolt_li['radius'], 1e-10)
            target_w = mesh_li['w']

            for v in bolt_li['verts']:
                # Escalar radialmente
                du = v.co[radial_indices[0]] - bolt_li['cx']
                dv = v.co[radial_indices[1]] - bolt_li['cy']
                v.co[radial_indices[0]] = bolt_li['cx'] + du * scale_factor
                v.co[radial_indices[1]] = bolt_li['cy'] + dv * scale_factor
                # Alinear en W
                v.co[cyl_axis_idx] = target_w

        bmesh.update_edit_mesh(obj.data)

        # 7e. Bridge cada par
        for bolt_li, mesh_li in pairs:
            bpy.ops.mesh.select_all(action='DESELECT')
            bm = bmesh.from_edit_mesh(obj.data)
            bm.edges.ensure_lookup_table()

            bolt_idx = set(v.index for v in bolt_li['verts'])
            mesh_idx = set(v.index for v in mesh_li['verts'])

            for e in bm.edges:
                i0, i1 = e.verts[0].index, e.verts[1].index
                in_bolt = i0 in bolt_idx and i1 in bolt_idx
                in_mesh = i0 in mesh_idx and i1 in mesh_idx
                if (in_bolt or in_mesh) and not e.is_manifold:
                    e.select = True

            bmesh.update_edit_mesh(obj.data)

            try:
                bpy.ops.mesh.bridge_edge_loops()
            except Exception as ex:
                self.report({'WARNING'}, f"Bridge falló: {ex}")

        # Limpieza final
        bpy.ops.mesh.select_all(action='SELECT')
        bpy.ops.mesh.remove_doubles(threshold=0.00001)
        bpy.ops.mesh.select_all(action='DESELECT')

        turns = len_mm / pitch_mm
        self.report({'INFO'},
            f"✓ Rosca soldada: Ø{major_dia_mm:.1f}mm, L{len_mm:.1f}mm, "
            f"~{turns:.0f} vueltas, {len(pairs)} bridges")

        return {'FINISHED'}

    def _find_edge_loops(self, edges):
        """Agrupa edges en loops conectados por vértices compartidos."""
        from collections import defaultdict
        vert_to_edges = defaultdict(list)
        for e in edges:
            for v in e.verts:
                vert_to_edges[v].append(e)

        visited = set()
        loops = []
        for start_e in edges:
            if start_e in visited:
                continue
            loop_verts = set()
            queue = [start_e]
            while queue:
                edge = queue.pop()
                if edge in visited:
                    continue
                visited.add(edge)
                for v in edge.verts:
                    loop_verts.add(v)
                    for adj in vert_to_edges[v]:
                        if adj not in visited:
                            queue.append(adj)
            if loop_verts:
                loops.append(list(loop_verts))
        return loops


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
            layout.label(text="Entra a Edit Mode, selecciona toda la rosca", icon='ERROR')
            layout.label(text="(incluyendo tapas superior e inferior)")
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
