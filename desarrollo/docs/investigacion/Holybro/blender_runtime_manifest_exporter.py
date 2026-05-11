"""
Export a non-destructive runtime manifest from the Holybro X500 V2 Blender scene.

Run inside Blender 5.0.1. It does not export FBX, save the .blend, or change objects.
The manifest captures masters + instances, fastener candidates, bounds and transforms
for Unity-side grouping without assuming uncertain parentage.
"""

import json
import math
import os
import re
from datetime import datetime, timezone

import bpy
from mathutils import Vector

RUNTIME_COLLECTIONS = (
    "BAKE_MASTERS_LOW",
    "ASSEMBLY_INSTANCES_LOW",
    "PRIMITIVE_FASTENER_MASTERS",
    "PRIMITIVE_FASTENER_INSTANCES",
)

ROLE_BY_COLLECTION = {
    "BAKE_MASTERS_LOW": "master_runtime_member",
    "ASSEMBLY_INSTANCES_LOW": "linked_instance",
    "PRIMITIVE_FASTENER_MASTERS": "fastener_master_runtime_member",
    "PRIMITIVE_FASTENER_INSTANCES": "fastener_instance",
}

FASTENER_NAME_RE = re.compile(
    r"(GB70|CHEN|PAN|ZSLM|LM-|NILONGZHU|HUAN-GUIJIAO|FASTENER|SCREW|NUT|BOLT)",
    re.IGNORECASE,
)

OUTPUT_PATH = os.environ.get(
    "HOLYBRO_MANIFEST_OUTPUT",
    r"E:\WebGL_tesis\desarrollo\docs\investigacion\Holybro\holybro_blender_runtime_manifest_preview.json",
)
UNITY_FASTENER_BASELINE_COUNT = int(os.environ.get("HOLYBRO_UNITY_FASTENER_BASELINE", "168"))


def sanitize_id(value):
    value = value or "object"
    value = re.sub(r"[^A-Za-z0-9_\-]+", "_", value)
    value = re.sub(r"_+", "_", value).strip("_")
    return value or "object"


def collection_names_for_object(obj):
    return sorted(collection.name for collection in obj.users_collection)


def world_bbox(obj):
    points = [obj.matrix_world @ Vector(corner) for corner in obj.bound_box]
    minimum = [min(point[i] for point in points) for i in range(3)]
    maximum = [max(point[i] for point in points) for i in range(3)]
    center = [(minimum[i] + maximum[i]) * 0.5 for i in range(3)]
    size = [maximum[i] - minimum[i] for i in range(3)]
    return minimum, maximum, center, size


def matrix_to_rows(matrix):
    return [[round(matrix[row][col], 8) for col in range(4)] for row in range(4)]


def vec(values):
    return [round(float(value), 8) for value in values]


def point_bbox_distance(point, minimum, maximum):
    total = 0.0
    for axis in range(3):
        if point[axis] < minimum[axis]:
            total += (minimum[axis] - point[axis]) ** 2
        elif point[axis] > maximum[axis]:
            total += (point[axis] - maximum[axis]) ** 2
    return math.sqrt(total)


def center_distance(a, b):
    return math.sqrt(sum((a[i] - b[i]) ** 2 for i in range(3)))


def get_runtime_objects():
    entries = []
    seen = set()
    missing_collections = []

    for collection_name in RUNTIME_COLLECTIONS:
        collection = bpy.data.collections.get(collection_name)
        if collection is None:
            missing_collections.append(collection_name)
            continue

        for obj in collection.all_objects:
            if obj.type != "MESH":
                continue
            key = (obj.name, collection_name)
            if key in seen:
                continue
            seen.add(key)
            entries.append((collection_name, obj))

    return entries, missing_collections


def is_fastener_entry(collection_name, obj):
    if collection_name.startswith("PRIMITIVE_FASTENER"):
        return True
    return bool(FASTENER_NAME_RE.search(obj.name))


def object_entry(collection_name, obj, ordinal):
    minimum, maximum, center, size = world_bbox(obj)
    role = ROLE_BY_COLLECTION.get(collection_name, "runtime_member")
    stable_id = f"{sanitize_id(role)}_{ordinal:04d}_{sanitize_id(obj.name)}"

    return {
        "id": stable_id,
        "objectName": obj.name,
        "meshName": obj.data.name if obj.data else "",
        "meshUsers": obj.data.users if obj.data else 0,
        "runtimeRole": role,
        "sourceCollection": collection_name,
        "allCollections": collection_names_for_object(obj),
        "isFastenerCandidate": is_fastener_entry(collection_name, obj),
        "worldMatrix": matrix_to_rows(obj.matrix_world),
        "localPosition": vec(obj.location),
        "localRotationEuler": vec(obj.rotation_euler),
        "localScale": vec(obj.scale),
        "boundsWorld": {
            "min": vec(minimum),
            "max": vec(maximum),
            "center": vec(center),
            "size": vec(size),
        },
    }


def add_fastener_candidates(objects):
    parent_pool = [
        obj for obj in objects
        if not obj["isFastenerCandidate"]
        and obj["runtimeRole"] in ("master_runtime_member", "linked_instance")
    ]

    for obj in objects:
        if not obj["isFastenerCandidate"]:
            continue

        center = obj["boundsWorld"]["center"]
        candidates = []
        for parent in parent_pool:
            p_bounds = parent["boundsWorld"]
            bbox_distance = point_bbox_distance(center, p_bounds["min"], p_bounds["max"])
            c_distance = center_distance(center, p_bounds["center"])
            candidates.append({
                "objectId": parent["id"],
                "objectName": parent["objectName"],
                "runtimeRole": parent["runtimeRole"],
                "bboxDistance": round(bbox_distance, 8),
                "centerDistance": round(c_distance, 8),
                "score": round((bbox_distance * 10.0) + c_distance, 8),
            })

        candidates.sort(key=lambda item: (item["score"], item["bboxDistance"], item["centerDistance"], item["objectName"]))
        obj["parentCandidateReview"] = {
            "status": "review_required",
            "reason": "Fastener parentage is not assigned automatically. Review candidates before writing parentCanonicalPartId.",
            "candidates": candidates[:6],
        }


def main():
    runtime_entries, missing_collections = get_runtime_objects()
    objects = [
        object_entry(collection_name, obj, index)
        for index, (collection_name, obj) in enumerate(runtime_entries)
    ]
    add_fastener_candidates(objects)

    role_counts = {}
    for obj in objects:
        role_counts[obj["runtimeRole"]] = role_counts.get(obj["runtimeRole"], 0) + 1

    blender_fastener_instances = role_counts.get("fastener_instance", 0)
    report = {
        "schemaVersion": 1,
        "sourceBlend": bpy.data.filepath,
        "blenderVersion": bpy.app.version_string,
        "generatedAtUtc": datetime.now(timezone.utc).isoformat(),
        "runtimeCollections": list(RUNTIME_COLLECTIONS),
        "missingCollections": missing_collections,
        "counts": {
            "totalRuntimeObjects": len(objects),
            "byRole": role_counts,
            "fastenerCandidates": sum(1 for obj in objects if obj["isFastenerCandidate"]),
        },
        "fastenerReconciliation": {
            "unityBaselineFastenerInstances": UNITY_FASTENER_BASELINE_COUNT,
            "blenderPrimitiveFastenerInstances": blender_fastener_instances,
            "deltaBlenderMinusUnityBaseline": blender_fastener_instances - UNITY_FASTENER_BASELINE_COUNT,
            "status": "matched" if blender_fastener_instances == UNITY_FASTENER_BASELINE_COUNT else "review_required",
            "rule": "Blender scene defines current exported existence and pose; Unity catalog semantics must be reconciled explicitly before parent assignments.",
        },
        "objects": objects,
        "notes": [
            "Masters are runtime members and must not be excluded from the final Unity import.",
            "Fastener parentage is intentionally reported as candidates only until reviewed.",
        ],
    }

    os.makedirs(os.path.dirname(OUTPUT_PATH), exist_ok=True)
    with open(OUTPUT_PATH, "w", encoding="utf-8") as handle:
        json.dump(report, handle, ensure_ascii=False, indent=2)

    print("[RuntimeManifest] Saved:", OUTPUT_PATH)
    print("[RuntimeManifest] Counts:", report["counts"])
    if missing_collections:
        print("[RuntimeManifest] Missing collections:", ", ".join(missing_collections))


if __name__ == "__main__":
    main()
