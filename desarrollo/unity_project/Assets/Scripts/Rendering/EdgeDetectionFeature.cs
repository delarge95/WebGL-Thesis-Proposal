using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace WebGL.Rendering
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

        public override void Create()
        {
            edgePass = new EdgeDetectionPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!GlobalEnabled) return;
            if (settings.edgeDetectionMaterial == null) return;
            renderer.EnqueuePass(edgePass);
        }

        private class EdgeDetectionPass : ScriptableRenderPass
        {
            private Settings settings;
            private RTHandle tempRT;

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

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;
                desc.msaaSamples = 1;
                RenderingUtils.ReAllocateHandleIfNeeded(ref tempRT, desc, FilterMode.Bilinear, name: "_EdgeDetectionTemp");
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var mat = settings.edgeDetectionMaterial;
                if (mat == null) return;

                var cmd = CommandBufferPool.Get("Edge Detection");

                // Use override color from ViewModeManager
                mat.SetColor(EdgeColorId, OverrideEdgeColor);
                mat.SetFloat(ThicknessId, settings.edgeThickness);
                mat.SetFloat(DepthThresholdId, settings.depthThreshold);
                mat.SetFloat(NormalThresholdId, settings.normalThreshold);

                // Toggle keywords
                if (settings.useDepthEdges)
                    mat.EnableKeyword("_DEPTHEDGES_ON");
                else
                    mat.DisableKeyword("_DEPTHEDGES_ON");

                if (settings.useNormalEdges)
                    mat.EnableKeyword("_NORMALEDGES_ON");
                else
                    mat.DisableKeyword("_NORMALEDGES_ON");

                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                Blitter.BlitCameraTexture(cmd, source, tempRT, mat, 0);
                Blitter.BlitCameraTexture(cmd, tempRT, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public void Dispose()
            {
                tempRT?.Release();
            }
        }
    }
}
