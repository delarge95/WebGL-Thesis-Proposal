"""
Prepare active bake target image nodes for the Holybro X500 V2 runtime materials.

Run inside Blender 5.0.1 from the Text Editor or with:
blender --background <file.blend> --python blender_bake_target_setup.py

This script does not bake, export, save, or alter material outputs. It only creates
target images and active Image Texture nodes so Blender's Bake operator knows where
to write each map.
"""

import os

import bpy

RUNTIME_COLLECTIONS = (
    "BAKE_MASTERS_LOW",
    "ASSEMBLY_INSTANCES_LOW",
    "PRIMITIVE_FASTENER_MASTERS",
    "PRIMITIVE_FASTENER_INSTANCES",
)

BAKE_TARGETS = {
    "base_color": {"image": "X500_BaseColor_4K", "colorspace": "sRGB"},
    "roughness": {"image": "X500_Roughness_4K", "colorspace": "Non-Color"},
    "metallic": {"image": "X500_Metallic_4K", "colorspace": "Non-Color"},
    "normal": {"image": "X500_Normal_Final_4K", "colorspace": "Non-Color"},
    "ao": {"image": "X500_AO_Final_4K", "colorspace": "Non-Color"},
}

# Change HOLYBRO_BAKE_TARGET before running when preparing a specific bake pass.
# Valid values: base_color, roughness, metallic, normal, ao.
ACTIVE_TARGET = os.environ.get("HOLYBRO_BAKE_TARGET", "base_color").strip().lower()
IMAGE_SIZE = int(os.environ.get("HOLYBRO_BAKE_SIZE", "4096"))


def get_runtime_objects():
    seen = set()
    objects = []
    for collection_name in RUNTIME_COLLECTIONS:
        collection = bpy.data.collections.get(collection_name)
        if collection is None:
            print(f"[BakeTargetSetup] Missing collection: {collection_name}")
            continue

        for obj in collection.all_objects:
            if obj.type != "MESH" or obj.name in seen:
                continue
            seen.add(obj.name)
            objects.append(obj)
    return objects


def ensure_image(image_name, colorspace):
    image = bpy.data.images.get(image_name)
    if image is None:
        image = bpy.data.images.new(
            image_name,
            width=IMAGE_SIZE,
            height=IMAGE_SIZE,
            alpha=True,
            float_buffer=False,
        )

    try:
        image.colorspace_settings.name = colorspace
    except TypeError:
        print(f"[BakeTargetSetup] Could not set colorspace {colorspace} on {image_name}")
    return image


def prepare_material(material, image):
    if material is None:
        return False

    material.use_nodes = True
    nodes = material.node_tree.nodes
    node_name = f"__BAKE_TARGET_{image.name}"
    target_node = nodes.get(node_name)
    if target_node is None:
        target_node = nodes.new(type="ShaderNodeTexImage")
        target_node.name = node_name
        target_node.label = node_name

    target_node.image = image

    for node in nodes:
        node.select = False
    target_node.select = True
    nodes.active = target_node
    return True


def main():
    if ACTIVE_TARGET not in BAKE_TARGETS:
        raise ValueError(f"Unknown ACTIVE_TARGET: {ACTIVE_TARGET}")

    target = BAKE_TARGETS[ACTIVE_TARGET]
    image = ensure_image(target["image"], target["colorspace"])
    objects = get_runtime_objects()
    materials = set()

    for obj in objects:
        for slot in obj.material_slots:
            if slot.material is not None:
                materials.add(slot.material)

    prepared = 0
    for material in materials:
        if prepare_material(material, image):
            prepared += 1

    print("[BakeTargetSetup] Active target:", ACTIVE_TARGET)
    print("[BakeTargetSetup] Image:", image.name, image.size[:])
    print("[BakeTargetSetup] Runtime mesh objects:", len(objects))
    print("[BakeTargetSetup] Materials prepared:", prepared)
    print("[BakeTargetSetup] Next step: run the matching Blender bake pass manually.")


if __name__ == "__main__":
    main()
