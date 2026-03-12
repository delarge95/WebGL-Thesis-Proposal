Shader "Hidden/WebGL/EdgeDetection"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            Name "EdgeDetection"

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile_local _ _DEPTHEDGES_ON
            #pragma multi_compile_local _ _NORMALEDGES_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _EdgeColor;
            float _Thickness;
            float _DepthThreshold;
            float _NormalThreshold;

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 texelSize = _BlitTexture_TexelSize.xy * _Thickness;

                half4 original = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                float edge = 0;

                #ifdef _DEPTHEDGES_ON
                // Roberts Cross operator on depth
                float d00 = SampleSceneDepth(uv + float2(-texelSize.x, -texelSize.y));
                float d11 = SampleSceneDepth(uv + float2( texelSize.x,  texelSize.y));
                float d01 = SampleSceneDepth(uv + float2(-texelSize.x,  texelSize.y));
                float d10 = SampleSceneDepth(uv + float2( texelSize.x, -texelSize.y));

                // Linearize depth for better threshold behavior
                float ld00 = Linear01Depth(d00, _ZBufferParams);
                float ld11 = Linear01Depth(d11, _ZBufferParams);
                float ld01 = Linear01Depth(d01, _ZBufferParams);
                float ld10 = Linear01Depth(d10, _ZBufferParams);

                float depthEdge = abs(ld00 - ld11) + abs(ld01 - ld10);
                edge = max(edge, step(_DepthThreshold, depthEdge));
                #endif

                #ifdef _NORMALEDGES_ON
                // Roberts Cross operator on normals
                float3 n00 = SampleSceneNormals(uv + float2(-texelSize.x, -texelSize.y));
                float3 n11 = SampleSceneNormals(uv + float2( texelSize.x,  texelSize.y));
                float3 n01 = SampleSceneNormals(uv + float2(-texelSize.x,  texelSize.y));
                float3 n10 = SampleSceneNormals(uv + float2( texelSize.x, -texelSize.y));

                float normalEdge = length(n00 - n11) + length(n01 - n10);
                edge = max(edge, step(_NormalThreshold, normalEdge));
                #endif

                return lerp(original, _EdgeColor, edge);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
