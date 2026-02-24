Shader "WebGL/WireframeWebGL"
{
    Properties
    {
        _WireColor("Wire Color", Color) = (0.3, 0.8, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (0.02, 0.02, 0.05, 0.0)
        _WireThickness("Wire Thickness", Range(0.001, 0.1)) = 0.02
        _FresnelPower("Fresnel Power", Range(0.1, 5)) = 3.0
        
        [Header(Depth)]
        [Toggle(_DEPTH_FADE)] _DepthFade("Depth Fade", Float) = 1
        _DepthFadeDistance("Depth Fade Distance", Range(1, 100)) = 20
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
            Name "WireframeWebGL"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma shader_feature_local _DEPTH_FADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _WireColor;
                half4 _BackgroundColor;
                half _WireThickness;
                half _FresnelPower;
                half _DepthFadeDistance;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Cross-section clipping
                if (_GlobalClipEnabled > 0.5)
                {
                    float clipDist = dot(IN.positionWS, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
                    if (clipDist < 0) discard;
                }

                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                
                // Edge detection via fresnel (works on all WebGL devices)
                half ndotv = saturate(dot(normalWS, viewDirWS));
                half fresnel = pow(1.0 - ndotv, _FresnelPower);
                
                // UV-based grid pattern for additional wireframe effect
                float2 gridUV = frac(IN.uv * 10.0);
                float gridLine = step(gridUV.x, _WireThickness) + step(gridUV.y, _WireThickness);
                gridLine = saturate(gridLine);
                
                // Combine fresnel edges with grid
                float wire = saturate(fresnel + gridLine * 0.3);
                
                // Depth fade
                #ifdef _DEPTH_FADE
                float depth = length(IN.positionWS - _WorldSpaceCameraPos);
                float depthFade = 1.0 - saturate(depth / _DepthFadeDistance);
                wire *= depthFade;
                #endif
                
                // Final color
                half4 color = lerp(_BackgroundColor, _WireColor, wire);
                color.a = max(wire * _WireColor.a, _BackgroundColor.a);
                
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
