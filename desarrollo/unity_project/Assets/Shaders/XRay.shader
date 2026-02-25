Shader "WebGL/XRay"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.3, 0.7, 1.0, 0.3)
        _FresnelPower("Fresnel Power", Range(0.1, 5)) = 2.0
        _FresnelIntensity("Fresnel Intensity", Range(0, 2)) = 1.0
        _RimColor("Rim Color", Color) = (0.5, 0.8, 1.0, 1.0)
        _InnerAlpha("Inner Alpha", Range(0, 1)) = 0.1
        _OuterAlpha("Outer Alpha", Range(0, 1)) = 0.8
        
        [Header(Behind Objects)]
        _BehindColor("Behind Color", Color) = (0.2, 0.5, 0.8, 0.5)
        _BehindIntensity("Behind Intensity", Range(0, 1)) = 0.5
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

        // Pass 1: Render parts behind other geometry (Z-fail)
        Pass
        {
            Name "XRay Behind"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Greater
            Cull Back

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _FresnelPower;
                half _FresnelIntensity;
                half4 _RimColor;
                half _InnerAlpha;
                half _OuterAlpha;
                half4 _BehindColor;
                half _BehindIntensity;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;
            float4 _GlobalClipPlane2;
            float _GlobalClipEnabled2;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Cross-section clipping (dual plane)
                if (_GlobalClipEnabled > 0.5)
                {
                    float clipDist = dot(IN.positionWS, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
                    if (clipDist < 0) discard;
                }
                if (_GlobalClipEnabled2 > 0.5)
                {
                    float clipDist2 = dot(IN.positionWS, _GlobalClipPlane2.xyz) + _GlobalClipPlane2.w;
                    if (clipDist2 < 0) discard;
                }

                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                
                // Fresnel for rim
                half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                
                half4 color = _BehindColor;
                color.a *= _BehindIntensity * fresnel;
                
                return color;
            }
            ENDHLSL
        }

        // Pass 2: Render visible parts with fresnel
        Pass
        {
            Name "XRay Front"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _FresnelPower;
                half _FresnelIntensity;
                half4 _RimColor;
                half _InnerAlpha;
                half _OuterAlpha;
                half4 _BehindColor;
                half _BehindIntensity;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;
            float4 _GlobalClipPlane2;
            float _GlobalClipEnabled2;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Cross-section clipping (dual plane)
                if (_GlobalClipEnabled > 0.5)
                {
                    float clipDist = dot(IN.positionWS, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
                    if (clipDist < 0) discard;
                }
                if (_GlobalClipEnabled2 > 0.5)
                {
                    float clipDist2 = dot(IN.positionWS, _GlobalClipPlane2.xyz) + _GlobalClipPlane2.w;
                    if (clipDist2 < 0) discard;
                }

                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                
                // Fresnel for rim lighting
                half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                fresnel *= _FresnelIntensity;
                
                // Combine base color with rim
                half3 color = lerp(_BaseColor.rgb, _RimColor.rgb, fresnel);
                half alpha = lerp(_InnerAlpha, _OuterAlpha, fresnel);
                
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
