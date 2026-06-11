using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using System;

namespace WebGL.Core.Rendering
{
    public class EdgeDetectionFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            public Material edgeDetectionMaterial;
            [ColorUsage(false)] public Color edgeColor = new Color(0.85f, 0.9f, 1f, 1f);
            [Range(0.5f, 5f)] public float edgeThickness = 1f;
            [Range(0.0001f, 0.05f)] public float depthThreshold = 0.001f;
            [Range(0.1f, 1f)] public float normalThreshold = 0.5f;
            public bool useDepthEdges = true;
            public bool useNormalEdges = true;
            public bool showEdgesOnly = false;
            [ColorUsage(false)] public Color backgroundColor = Color.white;
        }

        public Settings settings = new Settings();
        private EdgeDetectionPass edgePass;

        // Static toggle controlled by ViewModeManager
        public static bool GlobalEnabled { get; set; } = false;
        public static Color OverrideEdgeColor { get; set; } = new Color(0.85f, 0.9f, 1f, 1f);
        public static float OverrideEdgeThickness { get; set; } = -1f;

        public override void Create()
        {
            edgePass = new EdgeDetectionPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!GlobalEnabled) return;
            if (settings.edgeDetectionMaterial == null) return;
            if (renderingData.cameraData.cameraType != CameraType.Game) return;
            renderer.EnqueuePass(edgePass);
        }

        private class EdgeDetectionPass : ScriptableRenderPass
        {
            private Settings settings;
            private bool _renderGraphErrorLogged;

            private static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
            private static readonly int ThicknessId = Shader.PropertyToID("_Thickness");
            private static readonly int DepthThresholdId = Shader.PropertyToID("_DepthThreshold");
            private static readonly int NormalThresholdId = Shader.PropertyToID("_NormalThreshold");

            public EdgeDetectionPass(Settings settings)
            {
                this.settings = settings;
                renderPassEvent = settings.renderPassEvent;
                ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            }

            private void SetMaterialProperties(Material mat)
            {
                mat.SetColor(EdgeColorId, OverrideEdgeColor);
                mat.SetFloat(ThicknessId, OverrideEdgeThickness > 0f ? OverrideEdgeThickness : settings.edgeThickness);
                mat.SetFloat(DepthThresholdId, settings.depthThreshold);
                mat.SetFloat(NormalThresholdId, settings.normalThreshold);

                if (settings.useDepthEdges)
                    mat.EnableKeyword("_DEPTHEDGES_ON");
                else
                    mat.DisableKeyword("_DEPTHEDGES_ON");

                if (settings.useNormalEdges)
                    mat.EnableKeyword("_NORMALEDGES_ON");
                else
                    mat.DisableKeyword("_NORMALEDGES_ON");
            }

            // ── RenderGraph path (Unity 6 default) ──
            private class PassData
            {
                public Material material;
                public TextureHandle source;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var mat = settings.edgeDetectionMaterial;
                if (mat == null) return;

                SetMaterialProperties(mat);

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraColorHandle = resourceData.activeColorTexture;

                if (!cameraColorHandle.IsValid()) return;

                TextureDesc descriptor;
                try
                {
                    descriptor = renderGraph.GetTextureDesc(cameraColorHandle);
                }
                catch (Exception ex)
                {
                    if (!_renderGraphErrorLogged)
                    {
                        _renderGraphErrorLogged = true;
                        Debug.LogWarning($"[EdgeDetectionFeature] Skipping pass due to invalid source texture handle: {ex.Message}");
                    }
                    return;
                }

                if (descriptor.width <= 0 || descriptor.height <= 0)
                {
                    return;
                }

                descriptor.name = "_EdgeDetectionTemp";
                descriptor.clearBuffer = false;
                var tempTexture = renderGraph.CreateTexture(descriptor);

                try
                {
                    // Blit source → temp with edge detection material
                    RenderGraphUtils.BlitMaterialParameters blitToTemp =
                        new(cameraColorHandle, tempTexture, mat, 0);
                    renderGraph.AddBlitPass(blitToTemp, "Edge Detection Blit");

                    // Blit temp → source (copy back)
                    RenderGraphUtils.BlitMaterialParameters blitBack =
                        new(tempTexture, cameraColorHandle, Blitter.GetBlitMaterial(TextureDimension.Tex2D), 0);
                    renderGraph.AddBlitPass(blitBack, "Edge Detection Copy Back");
                }
                catch (Exception ex)
                {
                    if (!_renderGraphErrorLogged)
                    {
                        _renderGraphErrorLogged = true;
                        Debug.LogWarning($"[EdgeDetectionFeature] RenderGraph edge pass failed and was skipped: {ex.Message}");
                    }
                }
            }

        }
    }
}
