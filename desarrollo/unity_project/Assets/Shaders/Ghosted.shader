Shader "WebGL/Ghosted"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.5, 0.7, 1.0, 0.15)
        _EdgeColor("Edge Color", Color) = (0.6, 0.8, 1.0, 0.8)
        
        [Header(Transparency)]
        _MinAlpha("Min Alpha (Center)", Range(0, 1)) = 0.05
        _MaxAlpha("Max Alpha (Edge)", Range(0, 1)) = 0.4
        _FresnelPower("Fresnel Power", Range(0.1, 5)) = 2.0
        
        [Header(Depth)]
        [Toggle(_DEPTH_FADE)] _DepthFade("Depth Fade", Float) = 1
        _DepthFadeNear("Depth Fade Near", Range(0, 10)) = 0.5
        _DepthFadeFar("Depth Fade Far", Range(1, 50)) = 10
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "Ghosted"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _DEPTH_FADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EdgeColor;
                half _MinAlpha;
                half _MaxAlpha;
                half _FresnelPower;
                half _DepthFadeNear;
                half _DepthFadeFar;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                
                // Fresnel for edge visibility
                half ndotv = saturate(dot(normalWS, viewDirWS));
                half fresnel = pow(1.0 - ndotv, _FresnelPower);
                
                // Color blend based on fresnel
                half3 color = lerp(_BaseColor.rgb, _EdgeColor.rgb, fresnel);
                
                // Alpha blend based on fresnel
                half alpha = lerp(_MinAlpha, _MaxAlpha, fresnel);
                
                // Depth fade
                #ifdef _DEPTH_FADE
                float depth = length(IN.positionWS - _WorldSpaceCameraPos);
                float depthFade = saturate((depth - _DepthFadeNear) / (_DepthFadeFar - _DepthFadeNear));
                alpha *= (1.0 - depthFade * 0.5);
                #endif
                
                color = MixFog(color, IN.fogFactor);
                
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
