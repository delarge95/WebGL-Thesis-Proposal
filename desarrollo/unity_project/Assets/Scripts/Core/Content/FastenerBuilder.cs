using UnityEngine;
using WebGL.Core.Data;

namespace WebGL.Core.Content
{
    public static class FastenerBuilder
    {
        private struct AxisAssignment
        {
            public int lengthAxis;
            public int crossAxisA;
            public int crossAxisB;
        }

        public static GameObject BuildDetailVisual(Transform proxyRoot, FastenerMetadata metadata)
        {
            if (proxyRoot == null || metadata == null)
            {
                return null;
            }

            Renderer[] proxyRenderers = proxyRoot.GetComponentsInChildren<Renderer>(true);
            Bounds localBounds = CalculateLocalBounds(proxyRoot, proxyRenderers);
            Vector3 size = localBounds.size;
            if (size.sqrMagnitude <= 0.000001f)
            {
                size = Vector3.one * 0.01f;
                localBounds = new Bounds(Vector3.zero, size);
            }

            AxisAssignment axes = ResolveAxes(size, metadata);
            float totalLength = GetAxisValue(size, axes.lengthAxis);
            float crossDiameter = Mathf.Max(GetAxisValue(size, axes.crossAxisA), GetAxisValue(size, axes.crossAxisB));
            totalLength = Mathf.Max(totalLength, crossDiameter * 0.8f, 0.004f);
            crossDiameter = Mathf.Max(crossDiameter, totalLength * 0.2f, 0.002f);

            Material sourceMaterial = ResolveMaterial(proxyRenderers);

            GameObject detailRoot = new GameObject("__fastener_detail");
            detailRoot.transform.SetParent(proxyRoot, false);
            detailRoot.transform.localPosition = Vector3.zero;
            detailRoot.transform.localRotation = Quaternion.identity;
            detailRoot.transform.localScale = Vector3.one;

            GameObject orientationRoot = new GameObject("detail_orientation");
            orientationRoot.transform.SetParent(detailRoot.transform, false);
            orientationRoot.transform.localPosition = localBounds.center;
            orientationRoot.transform.localRotation = RotationForAxis(axes.lengthAxis);
            orientationRoot.transform.localScale = Vector3.one;

            string builderType = string.IsNullOrWhiteSpace(metadata.builderType) ? "SocketCapScrew" : metadata.builderType;
            switch (builderType)
            {
                case "PanHeadScrew":
                    BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.20f, 0.64f, 0.10f, 1.05f, 0.54f, addDome: true, countersunkHead: false);
                    break;

                case "CountersunkScrew":
                    BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.18f, 0.68f, 0.10f, 1.08f, 0.54f, addDome: false, countersunkHead: true);
                    break;

                case "FlangeNut":
                    BuildFlangeNut(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "CapNut":
                    BuildCapNut(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "NylocNut":
                    BuildNylocNut(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "Standoff":
                    BuildStandoff(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "RubberGrommet":
                    BuildRubberGrommet(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "TubeStopper":
                    BuildTubeStopper(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter);
                    break;

                case "SocketCapScrew":
                default:
                    BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.24f, 0.60f, 0.10f, 1.0f, 0.56f, addDome: false, countersunkHead: false);
                    break;
            }

            return detailRoot;
        }

        private static void BuildScrewTiled(
            Transform parent,
            Material material,
            float totalLength,
            float crossDiameter,
            FastenerMetadata metadata,
            float headRatio,
            float shaftRatio,
            float tipRatio,
            float headDiameterScale,
            float shaftDiameterScale,
            bool addDome,
            bool countersunkHead)
        {
            float headLength = totalLength * headRatio;
            float tipLength = totalLength * tipRatio;
            float shaftLength = Mathf.Max(totalLength * shaftRatio, totalLength - headLength - tipLength);
            float normalization = headLength + shaftLength + tipLength;
            if (normalization > 0.00001f)
            {
                float scale = totalLength / normalization;
                headLength *= scale;
                shaftLength *= scale;
                tipLength *= scale;
            }

            float headDiameter = crossDiameter * headDiameterScale;
            float shaftDiameter = crossDiameter * shaftDiameterScale;

            int segments = ResolveMiddleSegments(metadata, totalLength, shaftLength);
            float segmentLength = Mathf.Max(shaftLength / segments, totalLength * 0.03f);
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "tip", material, new Vector3(0f, baseY + tipLength * 0.5f, 0f), tipLength, shaftDiameter * 0.75f);

            float shaftStart = baseY + tipLength;
            for (int i = 0; i < segments; i++)
            {
                float y = shaftStart + segmentLength * (i + 0.5f);
                CreateCylinder(parent, $"thread_{i:00}", material, new Vector3(0f, y, 0f), segmentLength * 0.96f, shaftDiameter);
            }

            float headStart = shaftStart + shaftLength;
            if (countersunkHead)
            {
                CreateCylinder(parent, "head_lower", material, new Vector3(0f, headStart + headLength * 0.25f, 0f), headLength * 0.50f, shaftDiameter * 0.92f);
                CreateCylinder(parent, "head_upper", material, new Vector3(0f, headStart + headLength * 0.75f, 0f), headLength * 0.50f, headDiameter);
                return;
            }

            CreateCylinder(parent, "head", material, new Vector3(0f, headStart + headLength * 0.5f, 0f), headLength, headDiameter);
            if (addDome)
            {
                CreateSphere(parent, "head_dome", material, new Vector3(0f, headStart + headLength * 0.92f, 0f), headDiameter * 0.92f);
            }
        }

        private static void BuildFlangeNut(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float flangeLength = totalLength * 0.34f;
            float bodyLength = totalLength - flangeLength;
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "flange", material, new Vector3(0f, baseY + flangeLength * 0.5f, 0f), flangeLength, crossDiameter);
            CreateCylinder(parent, "body", material, new Vector3(0f, baseY + flangeLength + bodyLength * 0.5f, 0f), bodyLength, crossDiameter * 0.78f);
        }

        private static void BuildCapNut(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float bodyLength = totalLength * 0.62f;
            float domeDiameter = crossDiameter * 0.86f;
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "body", material, new Vector3(0f, baseY + bodyLength * 0.5f, 0f), bodyLength, crossDiameter * 0.78f);
            CreateSphere(parent, "dome", material, new Vector3(0f, baseY + bodyLength + domeDiameter * 0.34f, 0f), domeDiameter);
        }

        private static void BuildNylocNut(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float bodyLength = totalLength * 0.72f;
            float insertLength = totalLength - bodyLength;
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "body", material, new Vector3(0f, baseY + bodyLength * 0.5f, 0f), bodyLength, crossDiameter * 0.76f);
            CreateCylinder(parent, "insert", material, new Vector3(0f, baseY + bodyLength + insertLength * 0.5f, 0f), insertLength, crossDiameter * 0.70f);
        }

        private static void BuildStandoff(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float collarLength = totalLength * 0.16f;
            float bodyLength = Mathf.Max(totalLength - (collarLength * 2f), totalLength * 0.40f);
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "collar_bottom", material, new Vector3(0f, baseY + collarLength * 0.5f, 0f), collarLength, crossDiameter * 0.92f);
            CreateCylinder(parent, "body", material, new Vector3(0f, baseY + collarLength + bodyLength * 0.5f, 0f), bodyLength, crossDiameter * 0.74f);
            CreateCylinder(parent, "collar_top", material, new Vector3(0f, baseY + collarLength + bodyLength + collarLength * 0.5f, 0f), collarLength, crossDiameter * 0.92f);
        }

        private static void BuildRubberGrommet(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float lipLength = totalLength * 0.28f;
            float midLength = totalLength - (lipLength * 2f);
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "lip_bottom", material, new Vector3(0f, baseY + lipLength * 0.5f, 0f), lipLength, crossDiameter);
            CreateCylinder(parent, "waist", material, new Vector3(0f, baseY + lipLength + midLength * 0.5f, 0f), midLength, crossDiameter * 0.66f);
            CreateCylinder(parent, "lip_top", material, new Vector3(0f, baseY + lipLength + midLength + lipLength * 0.5f, 0f), lipLength, crossDiameter);
        }

        private static void BuildTubeStopper(Transform parent, Material material, float totalLength, float crossDiameter)
        {
            float collarLength = totalLength * 0.28f;
            float stemLength = totalLength - collarLength;
            float baseY = -totalLength * 0.5f;

            CreateCylinder(parent, "stem", material, new Vector3(0f, baseY + stemLength * 0.5f, 0f), stemLength, crossDiameter * 0.70f);
            CreateCylinder(parent, "stopper", material, new Vector3(0f, baseY + stemLength + collarLength * 0.5f, 0f), collarLength, crossDiameter);
        }

        private static int ResolveMiddleSegments(FastenerMetadata metadata, float totalLength, float shaftLength)
        {
            int requestedSegments = Mathf.Max(metadata.turnCount, 0);
            if (requestedSegments <= 0 && metadata.threadPitchMm > 0.0001f && metadata.lengthMm > 0.0001f)
            {
                requestedSegments = Mathf.RoundToInt(metadata.lengthMm / metadata.threadPitchMm);
            }

            if (requestedSegments <= 0)
            {
                requestedSegments = Mathf.RoundToInt(Mathf.Lerp(3f, 10f, Mathf.Clamp01(shaftLength / Mathf.Max(totalLength, 0.0001f))));
            }

            return Mathf.Clamp(requestedSegments, 2, 12);
        }

        private static Bounds CalculateLocalBounds(Transform root, Renderer[] renderers)
        {
            if (root == null || renderers == null || renderers.Length == 0)
            {
                return new Bounds(Vector3.zero, Vector3.one * 0.01f);
            }

            bool hasBounds = false;
            Bounds localBounds = default;
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == null)
                {
                    continue;
                }

                Bounds worldBounds = renderer.bounds;
                Vector3 min = worldBounds.min;
                Vector3 max = worldBounds.max;
                Vector3[] corners =
                {
                    new Vector3(min.x, min.y, min.z),
                    new Vector3(min.x, min.y, max.z),
                    new Vector3(min.x, max.y, min.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(max.x, min.y, max.z),
                    new Vector3(max.x, max.y, min.z),
                    new Vector3(max.x, max.y, max.z)
                };

                for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
                {
                    Vector3 localCorner = root.InverseTransformPoint(corners[cornerIndex]);
                    if (!hasBounds)
                    {
                        localBounds = new Bounds(localCorner, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(localCorner);
                    }
                }
            }

            return hasBounds ? localBounds : new Bounds(Vector3.zero, Vector3.one * 0.01f);
        }

        private static Material ResolveMaterial(Renderer[] renderers)
        {
            if (renderers == null)
            {
                return null;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.sharedMaterial == null)
                {
                    continue;
                }

                return renderer.sharedMaterial;
            }

            return null;
        }

        private static AxisAssignment ResolveAxes(Vector3 size, FastenerMetadata metadata)
        {
            bool useShortestAxis = string.Equals(metadata.orientationMode, "ShortestAxis", System.StringComparison.OrdinalIgnoreCase);
            if (!useShortestAxis)
            {
                useShortestAxis = metadata.lengthMm > 0.0001f &&
                                  metadata.nominalDiameterMm > 0.0001f &&
                                  metadata.lengthMm <= metadata.nominalDiameterMm * 1.1f;
            }

            float[] values = { Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z) };
            int lengthAxis = 0;
            for (int i = 1; i < values.Length; i++)
            {
                if (useShortestAxis)
                {
                    if (values[i] < values[lengthAxis])
                    {
                        lengthAxis = i;
                    }
                }
                else if (values[i] > values[lengthAxis])
                {
                    lengthAxis = i;
                }
            }

            int crossAxisA = lengthAxis == 0 ? 1 : 0;
            int crossAxisB = lengthAxis == 2 ? 1 : 2;
            if (crossAxisA == lengthAxis)
            {
                crossAxisA = 1;
            }

            if (crossAxisB == lengthAxis || crossAxisB == crossAxisA)
            {
                crossAxisB = 2;
            }

            return new AxisAssignment
            {
                lengthAxis = lengthAxis,
                crossAxisA = crossAxisA,
                crossAxisB = crossAxisB
            };
        }

        private static float GetAxisValue(Vector3 value, int axis)
        {
            return axis switch
            {
                0 => value.x,
                1 => value.y,
                2 => value.z,
                _ => value.y
            };
        }

        private static Quaternion RotationForAxis(int axis)
        {
            return axis switch
            {
                0 => Quaternion.Euler(0f, 0f, -90f),
                2 => Quaternion.Euler(90f, 0f, 0f),
                _ => Quaternion.identity
            };
        }

        private static void CreateCylinder(Transform parent, string name, Material material, Vector3 localPosition, float length, float diameter)
        {
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ConfigurePrimitive(primitive, parent, name, material, localPosition, new Vector3(diameter, length * 0.5f, diameter));
        }

        private static void CreateSphere(Transform parent, string name, Material material, Vector3 localPosition, float diameter)
        {
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ConfigurePrimitive(primitive, parent, name, material, localPosition, Vector3.one * diameter);
        }

        private static void ConfigurePrimitive(
            GameObject primitive,
            Transform parent,
            string name,
            Material material,
            Vector3 localPosition,
            Vector3 localScale)
        {
            if (primitive == null || parent == null)
            {
                return;
            }

            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = localPosition;
            primitive.transform.localRotation = Quaternion.identity;
            primitive.transform.localScale = localScale;
            primitive.layer = parent.gameObject.layer;

            Collider collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            Renderer renderer = primitive.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}
