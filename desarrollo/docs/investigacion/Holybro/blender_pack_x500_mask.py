"""
Pack final Holybro X500 V2 texture channels into runtime masks.

Run inside Blender 5.0.1 after the debug maps exist on disk.
This writes:
- X500_Mask_4K.png: R=AO, G=Roughness, B=Curvature, A=Metallic
- X500_MetallicSmoothness_4K.png: R=Metallic, A=Smoothness(1-Roughness)

Edit TEXTURE_DIR if your bake output folder changes.
"""

import os
from array import array

import bpy

TEXTURE_DIR = os.environ.get("HOLYBRO_TEXTURE_DIR", r"E:\WebGL_tesis\blender_files\welded")

AO_MAP = "X500_AO_Final_4K.png"
ROUGHNESS_MAP = "X500_Roughness_4K.png"
CURVATURE_MAP = os.environ.get("HOLYBRO_CURVATURE_MAP", "MASTERS_bake_curve.png")
METALLIC_MAP = "X500_Metallic_4K.png"

MASK_OUT = "X500_Mask_4K.png"
URP_OUT = "X500_MetallicSmoothness_4K.png"


def load_image(filename, required=True):
    path = os.path.join(TEXTURE_DIR, filename)
    if not os.path.exists(path):
        if required:
            raise FileNotFoundError(path)
        return None

    image = bpy.data.images.load(path, check_existing=True)
    try:
        image.colorspace_settings.name = "Non-Color"
    except TypeError:
        pass
    return image


def ensure_same_size(images):
    sizes = [(img.name, tuple(img.size)) for img in images if img is not None]
    unique = {size for _, size in sizes}
    if len(unique) != 1:
        raise ValueError(f"Input images do not share the same size: {sizes}")
    return next(iter(unique))


def blank_rgba(pixel_count, fill=0.0):
    return array("f", [fill]) * (pixel_count * 4)


def copy_channel(source_image, source_channel, outputs):
    pixels = array("f", [0.0]) * (source_image.size[0] * source_image.size[1] * 4)
    source_image.pixels.foreach_get(pixels)

    pixel_count = source_image.size[0] * source_image.size[1]
    for i in range(pixel_count):
        value = pixels[i * 4 + source_channel]
        for target_buffer, target_channel, transform in outputs:
            target_buffer[i * 4 + target_channel] = transform(value) if transform else value


def fill_channel(target_buffer, target_channel, value, pixel_count):
    for i in range(pixel_count):
        target_buffer[i * 4 + target_channel] = value


def save_rgba(name, width, height, rgba_values):
    image = bpy.data.images.new(name, width=width, height=height, alpha=True, float_buffer=False)
    image.pixels.foreach_set(rgba_values)
    image.filepath_raw = os.path.join(TEXTURE_DIR, name)
    image.file_format = "PNG"
    try:
        image.colorspace_settings.name = "Non-Color"
    except TypeError:
        pass
    image.save()
    return image.filepath_raw


def main():
    ao = load_image(AO_MAP)
    roughness = load_image(ROUGHNESS_MAP)
    curvature = load_image(CURVATURE_MAP, required=False) if CURVATURE_MAP else None
    metallic = load_image(METALLIC_MAP)

    width, height = ensure_same_size([ao, roughness, curvature, metallic])
    pixel_count = width * height

    mask_pixels = blank_rgba(pixel_count)
    urp_pixels = blank_rgba(pixel_count)

    copy_channel(ao, 0, [(mask_pixels, 0, None)])
    copy_channel(roughness, 0, [(mask_pixels, 1, None), (urp_pixels, 3, lambda value: 1.0 - value)])

    if curvature is not None:
        copy_channel(curvature, 0, [(mask_pixels, 2, None)])
    else:
        fill_channel(mask_pixels, 2, 0.0, pixel_count)

    copy_channel(metallic, 0, [(mask_pixels, 3, None), (urp_pixels, 0, None)])
    fill_channel(urp_pixels, 1, 0.0, pixel_count)
    fill_channel(urp_pixels, 2, 0.0, pixel_count)

    mask_path = save_rgba(MASK_OUT, width, height, mask_pixels)
    urp_path = save_rgba(URP_OUT, width, height, urp_pixels)

    print("[MaskPack] Saved:", mask_path)
    print("[MaskPack] Saved:", urp_path)


if __name__ == "__main__":
    main()
