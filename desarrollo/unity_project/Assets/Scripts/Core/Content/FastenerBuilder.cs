using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Data;

namespace WebGL.Core.Content
{
    public static class FastenerBuilder
    {
        private const string ModuleResourcePath = "FastenerModules/modular_screw_parts";
        private const float DefaultFineThreadAxialScale = 0.90f;
        public const string DetailRootName = "__fastener_detail";

        private static Material cachedModuleMaterial;
        private static readonly Dictionary<Material, Material> sanitizedModuleMaterials = new Dictionary<Material, Material>();

        private struct AxisAssignment
        {
            public int lengthAxis;
            public int crossAxisA;
            public int crossAxisB;
        }

        private struct ModuleAxisAssignment
        {
            public int axialAxis;
            public int radialAxisA;
            public int radialAxisB;
        }

        private struct ScrewDimensions
        {
            public float headLength;
            public float shaftLength;
            public float tipLength;
            public float unitsPerMillimeter;
        }

        private struct ScrewFrame
        {
            public Vector3 center;
            public Vector3 axisDirection;
            public float totalLength;
            public float crossDiameter;
            public bool headAtPositiveEnd;
        }

        public static bool CanBuildModular(FastenerMetadata metadata)
        {
            if (metadata == null)
            {
                return false;
            }

            if (IsScrewBuilderType(metadata.builderType))
            {
                return true;
            }

            string searchText = BuildMetadataSearchText(metadata);
            if (ContainsAny(searchText, "nut", "standoff", "grommet", "stopper", "luomao", "nilongzhu", "zslm", "nylon"))
            {
                return false;
            }

            return ContainsAny(searchText, "cap_screw", "cap-screw", "pan_head", "pan-head", "countersunk", "gb70");
        }

        public static GameObject BuildDetailVisual(Transform proxyRoot, FastenerMetadata metadata)
        {
            if (proxyRoot == null || metadata == null)
            {
                return null;
            }

            if (!CanBuildModular(metadata))
            {
                return null;
            }

            Renderer[] proxyRenderers = GetProxyRenderers(proxyRoot);
            string builderType = string.IsNullOrWhiteSpace(metadata.builderType) ? "SocketCapScrew" : metadata.builderType;
            if (!TryResolveScrewFrame(proxyRoot, proxyRenderers, metadata, out ScrewFrame screwFrame))
            {
                Bounds localBounds = CalculateLocalBounds(proxyRoot, proxyRenderers);
                Vector3 size = localBounds.size;
                if (size.sqrMagnitude <= 0.000001f)
                {
                    size = Vector3.one * 0.01f;
                    localBounds = new Bounds(Vector3.zero, size);
                }

                AxisAssignment axes = ResolveAxes(size, metadata);
                float fallbackLength = GetAxisValue(size, axes.lengthAxis);
                float fallbackDiameter = Mathf.Max(GetAxisValue(size, axes.crossAxisA), GetAxisValue(size, axes.crossAxisB));
                float fallbackDominant = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
                float fallbackTinyFloor = Mathf.Max(fallbackDominant * 0.02f, 0.00002f);

                screwFrame = new ScrewFrame
                {
                    center = localBounds.center,
                    axisDirection = AxisVector(axes.lengthAxis),
                    totalLength = Mathf.Max(fallbackLength, fallbackDiameter * 0.8f, fallbackTinyFloor),
                    crossDiameter = Mathf.Max(fallbackDiameter, fallbackLength * 0.12f, fallbackTinyFloor * 0.5f),
                    headAtPositiveEnd = ResolveHeadAtPositiveLengthEnd(proxyRoot, proxyRenderers, axes, localBounds)
                };
            }

            float dominantSize = Mathf.Max(screwFrame.totalLength, screwFrame.crossDiameter);
            float tinyFloor = Mathf.Max(dominantSize * 0.02f, 0.00002f);
            float totalLength = Mathf.Max(screwFrame.totalLength, screwFrame.crossDiameter * 0.8f, tinyFloor);
            float crossDiameter = Mathf.Max(screwFrame.crossDiameter, totalLength * 0.12f, tinyFloor * 0.5f);
            crossDiameter = ResolveStableCrossDiameter(metadata, builderType, totalLength, crossDiameter);
            bool headAtPositiveEnd = screwFrame.headAtPositiveEnd;

            Material sourceMaterial = ResolveModuleMaterial(ResolveMaterial(proxyRenderers));

            GameObject detailRoot = new GameObject(DetailRootName);
            detailRoot.transform.SetParent(proxyRoot, false);
            detailRoot.transform.localPosition = Vector3.zero;
            detailRoot.transform.localRotation = Quaternion.identity;
            detailRoot.transform.localScale = Vector3.one;

            GameObject orientationRoot = new GameObject("detail_orientation");
            orientationRoot.transform.SetParent(detailRoot.transform, false);
            orientationRoot.transform.localPosition = screwFrame.center;
            orientationRoot.transform.localRotation = Quaternion.FromToRotation(Vector3.up, screwFrame.axisDirection.normalized);
            orientationRoot.transform.localScale = Vector3.one;

            switch (builderType)
            {
                case "PanHeadScrew":
                    if (!TryBuildModularScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, builderType, headAtPositiveEnd, 0.20f, 0.64f, 0.10f, 1.05f, 0.54f))
                    {
                        BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.20f, 0.64f, 0.10f, 1.05f, 0.54f, addDome: true, countersunkHead: false);
                    }
                    break;

                case "CountersunkScrew":
                    if (!TryBuildModularScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, builderType, headAtPositiveEnd, 0.18f, 0.68f, 0.10f, 1.08f, 0.54f))
                    {
                        BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.18f, 0.68f, 0.10f, 1.08f, 0.54f, addDome: false, countersunkHead: true);
                    }
                    break;

                case "SocketCapScrew":
                default:
                    if (!TryBuildModularScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, builderType, headAtPositiveEnd, 0.24f, 0.60f, 0.10f, 1.0f, 0.56f))
                    {
                        BuildScrewTiled(orientationRoot.transform, sourceMaterial, totalLength, crossDiameter, metadata, 0.24f, 0.60f, 0.10f, 1.0f, 0.56f, addDome: false, countersunkHead: false);
                    }
                    break;
            }

            return detailRoot;
        }

        private static bool TryBuildModularScrewTiled(
            Transform parent,
            Material material,
            float totalLength,
            float crossDiameter,
            FastenerMetadata metadata,
            string builderType,
            bool headAtPositiveEnd,
            float headRatio,
            float shaftRatio,
            float tipRatio,
            float headDiameterScale,
            float shaftDiameterScale)
        {
            ScrewModuleLibrary library = ScrewModuleLibrary.Load();
            if (library == null || !library.IsValid)
            {
                return false;
            }

            ScrewModuleTemplate headTemplate = ResolveHeadTemplate(library, metadata, builderType);
            if (headTemplate == null)
            {
                return false;
            }

            ScrewDimensions dimensions = ResolveScrewDimensions(metadata, builderType, totalLength, headRatio, shaftRatio, tipRatio);
            float headLength = dimensions.headLength;
            float shaftLength = dimensions.shaftLength;
            float tipLength = dimensions.tipLength;

            float headDiameter = crossDiameter * headDiameterScale;
            float shaftDiameter = crossDiameter * shaftDiameterScale;
            float minimumLength = Mathf.Max(totalLength * 0.01f, 0.000001f);
            float minimumShaftLength = Mathf.Max(totalLength * 0.20f, minimumLength);
            headLength = Mathf.Clamp(headLength, minimumLength, Mathf.Max(totalLength - minimumShaftLength - minimumLength, minimumLength));
            float maxEndLength = Mathf.Max(totalLength - headLength - minimumShaftLength, minimumLength);
            float endLength = Mathf.Clamp(Mathf.Max(tipLength, totalLength * 0.035f), minimumLength, maxEndLength);
            float directionSign = headAtPositiveEnd ? 1f : -1f;
            float tipEndY = headAtPositiveEnd ? -totalLength * 0.5f : totalLength * 0.5f;
            float headEndY = headAtPositiveEnd ? totalLength * 0.5f : -totalLength * 0.5f;
            float headBaseY = headEndY - directionSign * headLength;
            float shaftStart = tipEndY + directionSign * endLength;
            shaftLength = Mathf.Abs(headBaseY - shaftStart);
            if (shaftLength <= minimumLength)
            {
                return false;
            }

            int segments = ResolveModularThreadSegments(metadata, shaftLength, dimensions.unitsPerMillimeter);
            float segmentLength = shaftLength / Mathf.Max(segments, 1);
            Vector3 moduleAxisDirection = headAtPositiveEnd ? Vector3.up : Vector3.down;
            bool fineThread = IsCountersunkScrew(metadata, builderType);
            float threadLengthScale = fineThread ? ResolveFineThreadAxialScale(metadata) : 0.98f;

            float tipCenterY = tipEndY + directionSign * endLength * 0.5f;
            CreateModuleInstance(library.ThreadEnd, parent, "thread_end", material, new Vector3(0f, tipCenterY, 0f), endLength * threadLengthScale, shaftDiameter, moduleAxisDirection);

            for (int i = 0; i < segments; i++)
            {
                float y = shaftStart + directionSign * segmentLength * (i + 0.5f);
                CreateModuleInstance(library.ThreadTurn, parent, $"thread_turn_{i:00}", material, new Vector3(0f, y, 0f), segmentLength * threadLengthScale, shaftDiameter, moduleAxisDirection);
            }

            float headCenterY = headEndY - directionSign * headLength * 0.5f;
            CreateModuleInstance(headTemplate, parent, "head", material, new Vector3(0f, headCenterY, 0f), headLength, headDiameter, moduleAxisDirection);
            return true;
        }

        public static bool IsFastenerDetailTransform(Transform target)
        {
            Transform current = target;
            while (current != null)
            {
                if (string.Equals(current.name, DetailRootName, StringComparison.Ordinal))
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private static Renderer[] GetProxyRenderers(Transform proxyRoot)
        {
            if (proxyRoot == null)
            {
                return Array.Empty<Renderer>();
            }

            Renderer[] allRenderers = proxyRoot.GetComponentsInChildren<Renderer>(true);
            List<Renderer> filtered = new List<Renderer>(allRenderers.Length);
            for (int i = 0; i < allRenderers.Length; i++)
            {
                Renderer renderer = allRenderers[i];
                if (renderer == null || renderer.transform == null || IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                filtered.Add(renderer);
            }

            return filtered.ToArray();
        }

        private static bool TryResolveScrewFrame(
            Transform proxyRoot,
            Renderer[] renderers,
            FastenerMetadata metadata,
            out ScrewFrame frame)
        {
            frame = default;
            if (proxyRoot == null || renderers == null || renderers.Length == 0)
            {
                return false;
            }

            List<Vector3> points = new List<Vector3>(256);
            CollectLocalMeshPoints(proxyRoot, renderers, points);
            if (points.Count < 4)
            {
                return false;
            }

            Vector3 mean = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
            {
                mean += points[i];
            }

            mean /= points.Count;
            if (!TryResolvePrincipalAxis(points, mean, out Vector3 axisDirection))
            {
                return false;
            }

            bool useShortestAxis = string.Equals(metadata.orientationMode, "ShortestAxis", StringComparison.OrdinalIgnoreCase);
            if (useShortestAxis)
            {
                axisDirection = ResolveShortestCardinalAxis(points);
            }

            float minProjection = float.PositiveInfinity;
            float maxProjection = float.NegativeInfinity;
            Vector3 radialSum = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
            {
                float projection = Vector3.Dot(points[i], axisDirection);
                minProjection = Mathf.Min(minProjection, projection);
                maxProjection = Mathf.Max(maxProjection, projection);
                radialSum += points[i] - axisDirection * projection;
            }

            float totalLength = maxProjection - minProjection;
            if (totalLength <= 0.000001f)
            {
                return false;
            }

            Vector3 radialCenter = radialSum / points.Count;
            Vector3 center = radialCenter + axisDirection * ((minProjection + maxProjection) * 0.5f);
            float maxRadiusSq = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                float projection = Vector3.Dot(points[i], axisDirection);
                Vector3 closestPointOnAxis = radialCenter + axisDirection * projection;
                float radiusSq = (points[i] - closestPointOnAxis).sqrMagnitude;
                if (radiusSq > maxRadiusSq)
                {
                    maxRadiusSq = radiusSq;
                }
            }

            float crossDiameter = Mathf.Sqrt(maxRadiusSq) * 2f;
            if (crossDiameter <= 0.000001f)
            {
                return false;
            }

            frame = new ScrewFrame
            {
                center = center,
                axisDirection = axisDirection,
                totalLength = totalLength,
                crossDiameter = crossDiameter,
                headAtPositiveEnd = ResolveHeadAtPositiveEndFromProjectedPoints(points, axisDirection, radialCenter, minProjection, maxProjection)
            };
            return true;
        }

        private static void CollectLocalMeshPoints(Transform root, Renderer[] renderers, List<Vector3> points)
        {
            if (root == null || renderers == null || points == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == null || IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                Mesh mesh = ResolveRendererMesh(renderer, out Transform meshTransform);
                if (mesh == null || meshTransform == null)
                {
                    continue;
                }

                if (!mesh.isReadable)
                {
                    AddMeshBoundsCorners(root, meshTransform, mesh.bounds, points);
                    continue;
                }

                Vector3[] vertices = mesh.vertices;
                if (vertices == null || vertices.Length == 0)
                {
                    AddMeshBoundsCorners(root, meshTransform, mesh.bounds, points);
                    continue;
                }

                for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
                {
                    points.Add(root.InverseTransformPoint(meshTransform.TransformPoint(vertices[vertexIndex])));
                }
            }
        }

        private static void AddMeshBoundsCorners(Transform root, Transform meshTransform, Bounds meshBounds, List<Vector3> points)
        {
            if (root == null || meshTransform == null || points == null)
            {
                return;
            }

            Vector3 min = meshBounds.min;
            Vector3 max = meshBounds.max;
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

            for (int i = 0; i < corners.Length; i++)
            {
                points.Add(root.InverseTransformPoint(meshTransform.TransformPoint(corners[i])));
            }
        }

        private static bool TryResolvePrincipalAxis(IReadOnlyList<Vector3> points, Vector3 center, out Vector3 axis)
        {
            axis = Vector3.up;
            if (points == null || points.Count < 4)
            {
                return false;
            }

            Vector3[] seeds =
            {
                Vector3.right,
                Vector3.up,
                Vector3.forward,
                new Vector3(1f, 1f, 0f).normalized,
                new Vector3(1f, 0f, 1f).normalized,
                new Vector3(0f, 1f, 1f).normalized
            };

            float bestVariance = -1f;
            Vector3 bestAxis = Vector3.up;
            for (int seedIndex = 0; seedIndex < seeds.Length; seedIndex++)
            {
                Vector3 candidate = seeds[seedIndex];
                for (int iteration = 0; iteration < 10; iteration++)
                {
                    Vector3 multiplied = MultiplyCovariance(points, center, candidate);
                    float magnitude = multiplied.magnitude;
                    if (magnitude <= 0.00000001f)
                    {
                        break;
                    }

                    candidate = multiplied / magnitude;
                }

                float variance = ResolveDirectionalVariance(points, center, candidate);
                if (variance > bestVariance)
                {
                    bestVariance = variance;
                    bestAxis = candidate;
                }
            }

            if (bestVariance <= 0.00000001f || bestAxis.sqrMagnitude <= 0.00000001f)
            {
                return false;
            }

            axis = StabilizeAxisSign(bestAxis.normalized);
            return true;
        }

        private static Vector3 MultiplyCovariance(IReadOnlyList<Vector3> points, Vector3 center, Vector3 direction)
        {
            Vector3 result = Vector3.zero;
            if (points == null || points.Count == 0)
            {
                return result;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 delta = points[i] - center;
                result += delta * Vector3.Dot(delta, direction);
            }

            return result / points.Count;
        }

        private static float ResolveDirectionalVariance(IReadOnlyList<Vector3> points, Vector3 center, Vector3 direction)
        {
            if (points == null || points.Count == 0 || direction.sqrMagnitude <= 0.00000001f)
            {
                return 0f;
            }

            Vector3 normalizedDirection = direction.normalized;
            float variance = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                float projection = Vector3.Dot(points[i] - center, normalizedDirection);
                variance += projection * projection;
            }

            return variance / points.Count;
        }

        private static Vector3 StabilizeAxisSign(Vector3 axis)
        {
            Vector3 absolute = new Vector3(Mathf.Abs(axis.x), Mathf.Abs(axis.y), Mathf.Abs(axis.z));
            if (absolute.y >= absolute.x && absolute.y >= absolute.z)
            {
                return axis.y < 0f ? -axis : axis;
            }

            if (absolute.x >= absolute.z)
            {
                return axis.x < 0f ? -axis : axis;
            }

            return axis.z < 0f ? -axis : axis;
        }

        private static Vector3 ResolveShortestCardinalAxis(IReadOnlyList<Vector3> points)
        {
            if (points == null || points.Count == 0)
            {
                return Vector3.up;
            }

            Bounds bounds = new Bounds(points[0], Vector3.zero);
            for (int i = 1; i < points.Count; i++)
            {
                bounds.Encapsulate(points[i]);
            }

            Vector3 size = bounds.size;
            if (size.x <= size.y && size.x <= size.z)
            {
                return Vector3.right;
            }

            if (size.z <= size.x && size.z <= size.y)
            {
                return Vector3.forward;
            }

            return Vector3.up;
        }

        private static bool ResolveHeadAtPositiveEndFromProjectedPoints(
            IReadOnlyList<Vector3> points,
            Vector3 axisDirection,
            Vector3 radialCenter,
            float minProjection,
            float maxProjection)
        {
            if (points == null || points.Count < 4)
            {
                return true;
            }

            float totalLength = maxProjection - minProjection;
            if (totalLength <= 0.000001f)
            {
                return true;
            }

            float negativeLimit = minProjection + totalLength * 0.30f;
            float positiveLimit = maxProjection - totalLength * 0.30f;
            float positiveMaxRadiusSq = 0f;
            float negativeMaxRadiusSq = 0f;
            int positiveSamples = 0;
            int negativeSamples = 0;

            for (int i = 0; i < points.Count; i++)
            {
                float projection = Vector3.Dot(points[i], axisDirection);
                Vector3 closestPointOnAxis = radialCenter + axisDirection * projection;
                float radiusSq = (points[i] - closestPointOnAxis).sqrMagnitude;

                if (projection >= positiveLimit)
                {
                    positiveSamples++;
                    positiveMaxRadiusSq = Mathf.Max(positiveMaxRadiusSq, radiusSq);
                }
                else if (projection <= negativeLimit)
                {
                    negativeSamples++;
                    negativeMaxRadiusSq = Mathf.Max(negativeMaxRadiusSq, radiusSq);
                }
            }

            if (positiveSamples < 3 || negativeSamples < 3)
            {
                return true;
            }

            float positiveRadius = Mathf.Sqrt(positiveMaxRadiusSq);
            float negativeRadius = Mathf.Sqrt(negativeMaxRadiusSq);
            float larger = Mathf.Max(positiveRadius, negativeRadius);
            float smaller = Mathf.Max(Mathf.Min(positiveRadius, negativeRadius), 0.000001f);
            float minimumDifference = Mathf.Max(totalLength * 0.015f, 0.00001f);
            if (larger - smaller < minimumDifference || larger / smaller < 1.18f)
            {
                return true;
            }

            return positiveRadius > negativeRadius;
        }

        private static void CreateModuleInstance(
            ScrewModuleTemplate template,
            Transform parent,
            string name,
            Material material,
            Vector3 targetCenter,
            float targetLength,
            float targetDiameter,
            Vector3 targetAxisDirection)
        {
            if (template == null || !template.IsValid || parent == null)
            {
                return;
            }

            Bounds templateBounds = template.Bounds;
            Vector3 templateSize = templateBounds.size;
            ModuleAxisAssignment axes = ResolveModuleAxes(templateSize);
            float radialDiameter = Mathf.Max(GetAxisValue(templateSize, axes.radialAxisA), GetAxisValue(templateSize, axes.radialAxisB), 0.000001f);
            Vector3 localScale = Vector3.one;
            SetAxisValue(ref localScale, axes.axialAxis, Mathf.Max(targetLength, 0.000001f) / Mathf.Max(GetAxisValue(templateSize, axes.axialAxis), 0.000001f));
            SetAxisValue(ref localScale, axes.radialAxisA, Mathf.Max(targetDiameter, 0.000001f) / radialDiameter);
            SetAxisValue(ref localScale, axes.radialAxisB, Mathf.Max(targetDiameter, 0.000001f) / radialDiameter);
            Quaternion axisRotation = Quaternion.FromToRotation(AxisVector(axes.axialAxis), targetAxisDirection.normalized);

            GameObject instance = new GameObject(name);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localRotation = axisRotation;
            instance.transform.localScale = localScale;
            instance.transform.localPosition = targetCenter - (axisRotation * Vector3.Scale(templateBounds.center, localScale));

            MeshFilter meshFilter = instance.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = template.Mesh;

            MeshRenderer meshRenderer = instance.AddComponent<MeshRenderer>();
            if (material != null)
            {
                Material[] materials = new Material[Mathf.Max(template.Mesh.subMeshCount, 1)];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                meshRenderer.sharedMaterials = materials;
            }
            else if (template.Materials != null && template.Materials.Length > 0)
            {
                meshRenderer.sharedMaterials = template.Materials;
            }

            ConfigureModuleInstance(instance, material, parent.gameObject.layer);
        }

        private static void ConfigureModuleInstance(GameObject instance, Material material, int layer)
        {
            if (instance == null)
            {
                return;
            }

            Transform[] transforms = instance.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.layer = layer;
            }

            Collider[] colliders = instance.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                DestroyObject(colliders[i]);
            }

            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && material != null)
                {
                    renderers[i].sharedMaterial = material;
                }
            }
        }

        private static ScrewModuleTemplate ResolveHeadTemplate(ScrewModuleLibrary library, FastenerMetadata metadata, string builderType)
        {
            if (library == null)
            {
                return null;
            }

            if (IsCountersunkScrew(metadata, builderType))
            {
                return library.HeadChen;
            }

            if (IsPanHeadScrew(metadata, builderType))
            {
                return library.HeadPan;
            }

            return library.HeadDing;
        }

        private static float ResolveFineThreadAxialScale(FastenerMetadata metadata)
        {
            if (metadata != null && metadata.threadPitchMm > 0.0001f)
            {
                return Mathf.Clamp((metadata.threadPitchMm / 0.5f) * 0.98f, 0.72f, 0.98f);
            }

            return DefaultFineThreadAxialScale;
        }

        private static ScrewDimensions ResolveScrewDimensions(
            FastenerMetadata metadata,
            string builderType,
            float totalLength,
            float fallbackHeadRatio,
            float fallbackShaftRatio,
            float fallbackTipRatio)
        {
            if (metadata == null || metadata.lengthMm <= 0.0001f)
            {
                float head = totalLength * fallbackHeadRatio;
                float tip = totalLength * fallbackTipRatio;
                float shaft = Mathf.Max(totalLength * fallbackShaftRatio, totalLength - head - tip);
                NormalizeLengths(totalLength, ref head, ref shaft, ref tip);
                return new ScrewDimensions
                {
                    headLength = head,
                    shaftLength = shaft,
                    tipLength = tip,
                    unitsPerMillimeter = totalLength / Mathf.Max(metadata != null ? metadata.lengthMm : 1f, 0.0001f)
                };
            }

            float diameterMm = metadata.nominalDiameterMm > 0.0001f ? metadata.nominalDiameterMm : ParseMetricDiameter(metadata.metric);
            if (diameterMm <= 0.0001f)
            {
                diameterMm = Mathf.Clamp(metadata.lengthMm * 0.25f, 2.5f, 3f);
            }

            float headHeightMm = ResolveHeadHeightMm(metadata, builderType, diameterMm);
            float tipLengthMm = ResolveTipLengthMm(metadata);
            float shaftLengthMm = Mathf.Max(metadata.lengthMm - tipLengthMm, metadata.lengthMm * 0.82f);
            float referenceLengthMm = Mathf.Max(headHeightMm + shaftLengthMm + tipLengthMm, metadata.lengthMm);
            float unitsPerMm = totalLength / Mathf.Max(referenceLengthMm, 0.0001f);

            float headLength = headHeightMm * unitsPerMm;
            float shaftLength = shaftLengthMm * unitsPerMm;
            float tipLength = tipLengthMm * unitsPerMm;
            NormalizeLengths(totalLength, ref headLength, ref shaftLength, ref tipLength);

            return new ScrewDimensions
            {
                headLength = headLength,
                shaftLength = shaftLength,
                tipLength = tipLength,
                unitsPerMillimeter = unitsPerMm
            };
        }

        private static void NormalizeLengths(float totalLength, ref float headLength, ref float shaftLength, ref float tipLength)
        {
            float normalization = headLength + shaftLength + tipLength;
            if (normalization > 0.00001f)
            {
                float scale = totalLength / normalization;
                headLength *= scale;
                shaftLength *= scale;
                tipLength *= scale;
            }
        }

        private static float ResolveHeadHeightMm(FastenerMetadata metadata, string builderType, float diameterMm)
        {
            if (IsPanHeadScrew(metadata, builderType))
            {
                return Mathf.Max(diameterMm * 0.45f, 1.1f);
            }

            if (IsCountersunkScrew(metadata, builderType))
            {
                return Mathf.Max(diameterMm * 0.55f, 1.2f);
            }

            return Mathf.Max(diameterMm, 1.8f);
        }

        private static float ResolveTipLengthMm(FastenerMetadata metadata)
        {
            if (metadata != null && metadata.threadPitchMm > 0.0001f)
            {
                return Mathf.Clamp(metadata.threadPitchMm * 0.9f, 0.30f, 0.65f);
            }

            return 0.45f;
        }

        private static float ResolveStableCrossDiameter(
            FastenerMetadata metadata,
            string builderType,
            float totalLength,
            float measuredCrossDiameter)
        {
            if (metadata == null || totalLength <= 0.000001f)
            {
                return measuredCrossDiameter;
            }

            float diameterMm = metadata.nominalDiameterMm > 0.0001f
                ? metadata.nominalDiameterMm
                : ParseMetricDiameter(metadata.metric);
            if (diameterMm <= 0.0001f || metadata.lengthMm <= 0.0001f)
            {
                return measuredCrossDiameter;
            }

            float referenceLengthMm = ResolveReferenceLengthMm(metadata, builderType, diameterMm);
            if (referenceLengthMm <= 0.0001f)
            {
                return measuredCrossDiameter;
            }

            float unitsPerMillimeter = totalLength / referenceLengthMm;
            float expectedHeadDiameter = ResolveHeadDiameterMm(metadata, builderType, diameterMm) * unitsPerMillimeter;
            if (expectedHeadDiameter <= 0.000001f)
            {
                return measuredCrossDiameter;
            }

            float lowerBound = expectedHeadDiameter * 0.68f;
            float upperBound = expectedHeadDiameter * 1.35f;
            if (measuredCrossDiameter < lowerBound || measuredCrossDiameter > upperBound)
            {
                return expectedHeadDiameter;
            }

            return measuredCrossDiameter;
        }

        private static float ResolveReferenceLengthMm(FastenerMetadata metadata, string builderType, float diameterMm)
        {
            if (metadata == null)
            {
                return 0f;
            }

            float headHeightMm = ResolveHeadHeightMm(metadata, builderType, diameterMm);
            float tipLengthMm = ResolveTipLengthMm(metadata);
            float shaftLengthMm = Mathf.Max(metadata.lengthMm - tipLengthMm, metadata.lengthMm * 0.82f);
            return Mathf.Max(headHeightMm + shaftLengthMm + tipLengthMm, metadata.lengthMm);
        }

        private static float ResolveHeadDiameterMm(FastenerMetadata metadata, string builderType, float diameterMm)
        {
            if (IsCountersunkScrew(metadata, builderType))
            {
                return Mathf.Max(diameterMm * 1.95f, diameterMm + 2.2f);
            }

            if (IsPanHeadScrew(metadata, builderType))
            {
                return Mathf.Max(diameterMm * 1.75f, diameterMm + 2.0f);
            }

            return Mathf.Max(diameterMm * 1.83f, diameterMm + 2.3f);
        }

        private static int ResolveModularThreadSegments(FastenerMetadata metadata, float shaftLength, float unitsPerMillimeter)
        {
            if (metadata != null && metadata.threadPitchMm > 0.0001f && unitsPerMillimeter > 0.000001f)
            {
                float pitchUnits = metadata.threadPitchMm * unitsPerMillimeter;
                return Mathf.Clamp(Mathf.RoundToInt(shaftLength / Mathf.Max(pitchUnits, 0.000001f)), 2, 56);
            }

            int requestedSegments = metadata != null ? Mathf.Max(metadata.turnCount, 0) : 0;
            if (requestedSegments > 0 && requestedSegments != 12)
            {
                return Mathf.Clamp(requestedSegments, 2, 56);
            }

            return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(6f, 18f, Mathf.Clamp01(shaftLength / 0.05f))), 2, 24);
        }

        private static float ParseMetricDiameter(string metric)
        {
            if (string.IsNullOrWhiteSpace(metric))
            {
                return 0f;
            }

            string clean = metric.Trim().ToUpperInvariant().Replace("M", string.Empty).Replace(",", ".");
            return float.TryParse(clean, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value)
                ? value
                : 0f;
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

        private static bool ResolveHeadAtPositiveLengthEnd(
            Transform proxyRoot,
            Renderer[] renderers,
            AxisAssignment axes,
            Bounds localBounds)
        {
            if (proxyRoot == null || renderers == null || renderers.Length == 0)
            {
                return true;
            }

            if (TryResolveHeadAtPositiveEndFromMeshSamples(proxyRoot, renderers, axes, localBounds, out bool headAtPositiveEnd))
            {
                return headAtPositiveEnd;
            }

            float center = GetAxisValue(localBounds.center, axes.lengthAxis);
            float positiveScore = 0f;
            float negativeScore = 0f;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == null)
                {
                    continue;
                }

                Bounds rendererLocalBounds = CalculateRendererLocalBounds(proxyRoot, renderer);
                Vector3 size = rendererLocalBounds.size;
                if (size.sqrMagnitude <= 0.00000001f)
                {
                    continue;
                }

                float length = Mathf.Max(GetAxisValue(size, axes.lengthAxis), 0.000001f);
                float crossA = Mathf.Max(GetAxisValue(size, axes.crossAxisA), 0.000001f);
                float crossB = Mathf.Max(GetAxisValue(size, axes.crossAxisB), 0.000001f);
                float side = GetAxisValue(rendererLocalBounds.center, axes.lengthAxis) - center;
                float score = crossA * crossB * length;

                if (side >= 0f)
                {
                    positiveScore += score;
                }
                else
                {
                    negativeScore += score;
                }
            }

            if (Mathf.Abs(positiveScore - negativeScore) <= Mathf.Max(positiveScore, negativeScore) * 0.04f)
            {
                return true;
            }

            return positiveScore > negativeScore;
        }

        private static bool TryResolveHeadAtPositiveEndFromMeshSamples(
            Transform proxyRoot,
            Renderer[] renderers,
            AxisAssignment axes,
            Bounds localBounds,
            out bool headAtPositiveEnd)
        {
            headAtPositiveEnd = true;
            if (proxyRoot == null || renderers == null)
            {
                return false;
            }

            float axisMin = GetAxisValue(localBounds.min, axes.lengthAxis);
            float axisMax = GetAxisValue(localBounds.max, axes.lengthAxis);
            float totalLength = Mathf.Abs(axisMax - axisMin);
            if (totalLength <= 0.000001f)
            {
                return false;
            }

            float negativeLimit = axisMin + totalLength * 0.30f;
            float positiveLimit = axisMax - totalLength * 0.30f;
            float centerA = GetAxisValue(localBounds.center, axes.crossAxisA);
            float centerB = GetAxisValue(localBounds.center, axes.crossAxisB);
            float positiveMaxRadiusSq = 0f;
            float negativeMaxRadiusSq = 0f;
            int positiveSamples = 0;
            int negativeSamples = 0;

            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];
                if (renderer == null || renderer.transform == null || IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : null;
                if (mesh == null || !mesh.isReadable)
                {
                    continue;
                }

                Vector3[] vertices = mesh.vertices;
                for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
                {
                    Vector3 local = proxyRoot.InverseTransformPoint(meshFilter.transform.TransformPoint(vertices[vertexIndex]));
                    float axisValue = GetAxisValue(local, axes.lengthAxis);
                    float radialA = GetAxisValue(local, axes.crossAxisA) - centerA;
                    float radialB = GetAxisValue(local, axes.crossAxisB) - centerB;
                    float radiusSq = radialA * radialA + radialB * radialB;

                    if (axisValue >= positiveLimit)
                    {
                        positiveSamples++;
                        if (radiusSq > positiveMaxRadiusSq)
                        {
                            positiveMaxRadiusSq = radiusSq;
                        }
                    }
                    else if (axisValue <= negativeLimit)
                    {
                        negativeSamples++;
                        if (radiusSq > negativeMaxRadiusSq)
                        {
                            negativeMaxRadiusSq = radiusSq;
                        }
                    }
                }
            }

            if (positiveSamples < 3 || negativeSamples < 3)
            {
                return false;
            }

            float positiveRadius = Mathf.Sqrt(positiveMaxRadiusSq);
            float negativeRadius = Mathf.Sqrt(negativeMaxRadiusSq);
            float larger = Mathf.Max(positiveRadius, negativeRadius);
            float smaller = Mathf.Max(Mathf.Min(positiveRadius, negativeRadius), 0.000001f);
            float minimumDifference = Mathf.Max(totalLength * 0.015f, 0.00001f);
            if (larger - smaller < minimumDifference || larger / smaller < 1.18f)
            {
                return false;
            }

            headAtPositiveEnd = positiveRadius > negativeRadius;
            return true;
        }

        private static bool IsScrewBuilderType(string builderType)
        {
            return string.Equals(builderType, "SocketCapScrew", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(builderType, "PanHeadScrew", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(builderType, "CountersunkScrew", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPanHeadScrew(FastenerMetadata metadata, string builderType)
        {
            return string.Equals(builderType, "PanHeadScrew", StringComparison.OrdinalIgnoreCase) ||
                   Contains(BuildMetadataSearchText(metadata), "pan_head") ||
                   Contains(BuildMetadataSearchText(metadata), "pan-head") ||
                   Contains(BuildMetadataSearchText(metadata), "pan");
        }

        private static bool IsCountersunkScrew(FastenerMetadata metadata, string builderType)
        {
            return string.Equals(builderType, "CountersunkScrew", StringComparison.OrdinalIgnoreCase) ||
                   Contains(BuildMetadataSearchText(metadata), "countersunk") ||
                   Contains(BuildMetadataSearchText(metadata), "chen");
        }

        private static string BuildMetadataSearchText(FastenerMetadata metadata)
        {
            if (metadata == null)
            {
                return string.Empty;
            }

            return string.Join(" ",
                metadata.builderType ?? string.Empty,
                metadata.recipeKey ?? string.Empty,
                metadata.sceneTypeKey ?? string.Empty,
                metadata.sourceId ?? string.Empty,
                metadata.blenderName ?? string.Empty,
                metadata.subtype ?? string.Empty,
                metadata.headProfile ?? string.Empty,
                metadata.sceneObjectName ?? string.Empty);
        }

        private static bool ContainsAny(string text, params string[] tokens)
        {
            if (string.IsNullOrWhiteSpace(text) || tokens == null)
            {
                return false;
            }

            for (int i = 0; i < tokens.Length; i++)
            {
                if (Contains(text, tokens[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Contains(string text, string token)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   !string.IsNullOrWhiteSpace(token) &&
                   text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
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
                if (renderer == null || renderer.transform == null || IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                if (TryEncapsulateRendererMeshBounds(root, renderer, ref hasBounds, ref localBounds))
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

        private static Bounds CalculateRendererLocalBounds(Transform root, Renderer renderer)
        {
            if (root == null || renderer == null)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            bool hasMeshBounds = false;
            Bounds meshBounds = default;
            if (TryEncapsulateRendererMeshBounds(root, renderer, ref hasMeshBounds, ref meshBounds))
            {
                return meshBounds;
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

            Bounds localBounds = new Bounds(root.InverseTransformPoint(corners[0]), Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                localBounds.Encapsulate(root.InverseTransformPoint(corners[i]));
            }

            return localBounds;
        }

        private static bool TryEncapsulateRendererMeshBounds(
            Transform root,
            Renderer renderer,
            ref bool hasBounds,
            ref Bounds localBounds)
        {
            Mesh mesh = ResolveRendererMesh(renderer, out Transform meshTransform);
            if (root == null || mesh == null || meshTransform == null)
            {
                return false;
            }

            Bounds meshBounds = mesh.bounds;
            Vector3 min = meshBounds.min;
            Vector3 max = meshBounds.max;
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

            for (int i = 0; i < corners.Length; i++)
            {
                EncapsulateLocalPoint(root.InverseTransformPoint(meshTransform.TransformPoint(corners[i])), ref hasBounds, ref localBounds);
            }

            return true;
        }

        private static Mesh ResolveRendererMesh(Renderer renderer, out Transform meshTransform)
        {
            meshTransform = renderer != null ? renderer.transform : null;
            if (renderer == null)
            {
                return null;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                meshTransform = meshFilter.transform;
                return meshFilter.sharedMesh;
            }

            if (renderer is SkinnedMeshRenderer skinnedRenderer && skinnedRenderer.sharedMesh != null)
            {
                meshTransform = skinnedRenderer.transform;
                return skinnedRenderer.sharedMesh;
            }

            return null;
        }

        private static void EncapsulateLocalPoint(Vector3 point, ref bool hasBounds, ref Bounds bounds)
        {
            if (!hasBounds)
            {
                bounds = new Bounds(point, Vector3.zero);
                hasBounds = true;
                return;
            }

            bounds.Encapsulate(point);
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
                if (renderer == null || renderer.transform == null || IsFastenerDetailTransform(renderer.transform) || renderer.sharedMaterial == null)
                {
                    continue;
                }

                return renderer.sharedMaterial;
            }

            return null;
        }

        private static Material ResolveModuleMaterial(Material sourceMaterial)
        {
            if (sourceMaterial != null && !IsViewModeMaterial(sourceMaterial))
            {
                return GetSanitizedModuleMaterial(sourceMaterial);
            }

            return GetDefaultModuleMaterial();
        }

        private static Material GetSanitizedModuleMaterial(Material sourceMaterial)
        {
            if (sourceMaterial == null)
            {
                return GetDefaultModuleMaterial();
            }

            if (sanitizedModuleMaterials.TryGetValue(sourceMaterial, out Material cachedMaterial) && cachedMaterial != null)
            {
                return cachedMaterial;
            }

            Material moduleMaterial = new Material(sourceMaterial)
            {
                name = sourceMaterial.name + "_ModularFastener_NoMaps_Runtime"
            };

            DisableModuleTextureMaps(moduleMaterial);
            sanitizedModuleMaterials[sourceMaterial] = moduleMaterial;
            return moduleMaterial;
        }

        private static bool IsViewModeMaterial(Material material)
        {
            if (material == null)
            {
                return false;
            }

            string materialName = material.name ?? string.Empty;
            string shaderName = material.shader != null ? material.shader.name ?? string.Empty : string.Empty;
            return ContainsAny(materialName, "SolidColor", "Blueprint", "Wireframe", "Ghosted", "Thermal", "XRay") ||
                   ContainsAny(shaderName, "SolidColor", "Blueprint", "Wireframe", "Ghosted", "Thermal", "XRay");
        }

        private static Material GetDefaultModuleMaterial()
        {
            if (cachedModuleMaterial != null)
            {
                return cachedModuleMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            cachedModuleMaterial = new Material(shader)
            {
                name = "Fastener_Modular_Dark_Metal_Runtime"
            };

            Color baseColor = new Color(0.055f, 0.058f, 0.060f, 1f);
            if (cachedModuleMaterial.HasProperty("_BaseColor"))
            {
                cachedModuleMaterial.SetColor("_BaseColor", baseColor);
            }

            if (cachedModuleMaterial.HasProperty("_Color"))
            {
                cachedModuleMaterial.SetColor("_Color", baseColor);
            }

            if (cachedModuleMaterial.HasProperty("_Metallic"))
            {
                cachedModuleMaterial.SetFloat("_Metallic", 0.75f);
            }

            if (cachedModuleMaterial.HasProperty("_Smoothness"))
            {
                cachedModuleMaterial.SetFloat("_Smoothness", 0.58f);
            }

            if (cachedModuleMaterial.HasProperty("_OutlineOn"))
            {
                cachedModuleMaterial.SetFloat("_OutlineOn", 0f);
            }

            if (cachedModuleMaterial.HasProperty("_OutlineWidth"))
            {
                cachedModuleMaterial.SetFloat("_OutlineWidth", 0f);
            }

            DisableModuleTextureMaps(cachedModuleMaterial);
            return cachedModuleMaterial;
        }

        private static void DisableModuleTextureMaps(Material material)
        {
            if (material == null)
            {
                return;
            }

            ClearTexture(material, "_BumpMap");
            ClearTexture(material, "_NormalMap");
            ClearTexture(material, "_OcclusionMap");
            ClearTexture(material, "_DetailNormalMap");

            if (material.HasProperty("_BumpScale"))
            {
                material.SetFloat("_BumpScale", 0f);
            }

            if (material.HasProperty("_OcclusionStrength"))
            {
                material.SetFloat("_OcclusionStrength", 0f);
            }

            material.DisableKeyword("_NORMALMAP");
            material.DisableKeyword("_OCCLUSIONMAP");
            material.DisableKeyword("_DETAIL_MULX2");
            material.DisableKeyword("_DETAIL_SCALED");
        }

        private static void ClearTexture(Material material, string propertyName)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetTexture(propertyName, null);
            }
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

        private static void SetAxisValue(ref Vector3 value, int axis, float axisValue)
        {
            switch (axis)
            {
                case 0:
                    value.x = axisValue;
                    break;
                case 1:
                    value.y = axisValue;
                    break;
                case 2:
                    value.z = axisValue;
                    break;
            }
        }

        private static Vector3 AxisVector(int axis)
        {
            return axis switch
            {
                0 => Vector3.right,
                2 => Vector3.forward,
                _ => Vector3.up
            };
        }

        private static ModuleAxisAssignment ResolveModuleAxes(Vector3 size)
        {
            float[] values = { Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z) };
            int axialAxis = 0;
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < values[axialAxis])
                {
                    axialAxis = i;
                }
            }

            int radialAxisA = axialAxis == 0 ? 1 : 0;
            int radialAxisB = axialAxis == 2 ? 1 : 2;
            if (radialAxisA == axialAxis)
            {
                radialAxisA = 1;
            }

            if (radialAxisB == axialAxis || radialAxisB == radialAxisA)
            {
                radialAxisB = 2;
            }

            return new ModuleAxisAssignment
            {
                axialAxis = axialAxis,
                radialAxisA = radialAxisA,
                radialAxisB = radialAxisB
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
                UnityEngine.Object.Destroy(collider);
            }

            Renderer renderer = primitive.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static void DestroyObject(UnityEngine.Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(target);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private sealed class ScrewModuleTemplate
        {
            public Mesh Mesh { get; set; }
            public Bounds Bounds { get; set; }
            public Material[] Materials { get; set; }
            public bool IsValid => Mesh != null;
        }

        private sealed class ScrewModuleLibrary
        {
            private static ScrewModuleLibrary cachedLibrary;
            private static bool loadAttempted;

            public ScrewModuleTemplate HeadDing { get; private set; }
            public ScrewModuleTemplate HeadPan { get; private set; }
            public ScrewModuleTemplate HeadChen { get; private set; }
            public ScrewModuleTemplate ThreadTurn { get; private set; }
            public ScrewModuleTemplate ThreadEnd { get; private set; }

            public bool IsValid => HeadDing != null && HeadDing.IsValid &&
                                   HeadPan != null && HeadPan.IsValid &&
                                   HeadChen != null && HeadChen.IsValid &&
                                   ThreadTurn != null && ThreadTurn.IsValid &&
                                   ThreadEnd != null && ThreadEnd.IsValid;

            public static ScrewModuleLibrary Load()
            {
                if (loadAttempted)
                {
                    return cachedLibrary;
                }

                loadAttempted = true;
                GameObject root = Resources.Load<GameObject>(ModuleResourcePath);
                if (root == null)
                {
                    Debug.LogWarning($"[FastenerBuilder] Modular screw asset not found at Resources/{ModuleResourcePath}. Falling back to procedural screws.");
                    return null;
                }

                cachedLibrary = new ScrewModuleLibrary
                {
                    HeadDing = FindTemplate(root, "mod_head_ding"),
                    HeadPan = FindTemplate(root, "mod_head_pan"),
                    HeadChen = FindTemplate(root, "mod_head_chen"),
                    ThreadTurn = FindTemplate(root, "mod_thread_turn"),
                    ThreadEnd = FindTemplate(root, "mod_thread_end")
                };

                if (!cachedLibrary.IsValid)
                {
                    Debug.LogWarning("[FastenerBuilder] Modular screw FBX is missing one or more required objects: mod_head_ding, mod_head_pan, mod_head_chen, mod_thread_turn, mod_thread_end.");
                }

                return cachedLibrary;
            }

            private static ScrewModuleTemplate FindTemplate(GameObject root, string requiredName)
            {
                if (root == null || string.IsNullOrWhiteSpace(requiredName))
                {
                    return null;
                }

                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                string normalizedRequiredName = NormalizeName(requiredName);
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (NormalizeName(transforms[i].name) == normalizedRequiredName)
                    {
                        return CreateTemplateFromTransform(transforms[i]);
                    }
                }

                for (int i = 0; i < transforms.Length; i++)
                {
                    if (NormalizeName(transforms[i].name).Contains(normalizedRequiredName))
                    {
                        return CreateTemplateFromTransform(transforms[i]);
                    }
                }

                return null;
            }

            private static ScrewModuleTemplate CreateTemplateFromTransform(Transform root)
            {
                if (root == null)
                {
                    return null;
                }

                MeshFilter meshFilter = root.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null)
                {
                    meshFilter = root.GetComponentInChildren<MeshFilter>(true);
                }

                if (meshFilter == null || meshFilter.sharedMesh == null)
                {
                    return null;
                }

                MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
                return new ScrewModuleTemplate
                {
                    Mesh = meshFilter.sharedMesh,
                    Bounds = meshFilter.sharedMesh.bounds,
                    Materials = renderer != null ? renderer.sharedMaterials : null
                };
            }

            private static string NormalizeName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return string.Empty;
                }

                string normalized = name.Trim().ToLowerInvariant();
                int suffixIndex = normalized.LastIndexOf('.');
                if (suffixIndex > 0 &&
                    suffixIndex + 1 < normalized.Length &&
                    int.TryParse(normalized.Substring(suffixIndex + 1), out _))
                {
                    normalized = normalized.Substring(0, suffixIndex);
                }

                return normalized;
            }
        }
    }
}
