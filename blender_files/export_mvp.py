import bpy
import sys

# Print all collections
print("Collections:")
for col in bpy.data.collections:
    print(f" - {col.name}")

# Try to find 'instanced' or 'Instance'
target_col = None
for col in bpy.data.collections:
    if 'instance' in col.name.lower():
        target_col = col
        break

if not target_col:
    # If not found, use the first collection containing meshes or fallback to root
    print("WARNING: 'instanced' collection not found. Using all meshes.")
    bpy.ops.object.select_all(action='SELECT')
else:
    print(f"Found collection: {target_col.name}. Selecting objects...")
    bpy.ops.object.select_all(action='DESELECT')
    for obj in target_col.all_objects:
        obj.select_set(True)

output_path = r"e:\WebGL_tesis\desarrollo\unity_project\Assets\Models\x500v2_mvp.fbx"
print(f"Exporting to {output_path}...")
bpy.ops.export_scene.fbx(
    filepath=output_path,
    use_selection=True,
    global_scale=1.0,
    apply_scale_options='FBX_SCALE_ALL',
    object_types={'MESH'},
    use_mesh_modifiers=True,
    mesh_smooth_type='FACE'
)
print("Export complete.")
