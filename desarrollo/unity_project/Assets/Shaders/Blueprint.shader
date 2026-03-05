Shader "WebGL/Blueprint"
{
    Properties
    {
        _LineColor("Line Color", Color) = (0.85, 0.9, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (0.08, 0.18, 0.38, 1.0)
        _GridColor("Grid Color", Color) = (0.4, 0.55, 0.8, 0.2)
        
        _OutlineWidth("Outline Width", Range(0, 0.02)) = 0.003
        _EdgeThreshold("Edge Threshold", Range(0, 1)) = 0.15
        
        [Header(Grid)]
        _GridScale("Grid Scale", Range(1, 100)) = 20
        _GridWidth("Grid Line Width", Range(0.01, 0.1)) = 0.02
        
        [Header(Technical Lines)]
        _FresnelPower("Edge Detection Power", Range(0.1, 5)) = 2.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        // Main pass: Blueprint style with edge detection
        Pass
        {
            Name "Blueprint"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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
                float fogFactor : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _LineColor;
                half4 _BackgroundColor;
                half4 _GridColor;
                half _OutlineWidth;
                half _EdgeThreshold;
                half _GridScale;
                half _GridWidth;
                half _FresnelPower;
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
                OUT.uv = IN.uv;
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);

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
                
                // Edge detection using fresnel
                half ndotv = saturate(dot(normalWS, viewDirWS));
                half edge = pow(1.0 - ndotv, _FresnelPower);
                
                // Grid pattern on surface (world space)
                float2 gridUV = IN.positionWS.xz * _GridScale;
                float2 gridFrac = frac(gridUV);
                float gridLine = step(gridFrac.x, _GridWidth) + step(gridFrac.y, _GridWidth);
                gridLine = saturate(gridLine);
                
                // Secondary grid (finer)
                float2 gridUV2 = IN.positionWS.xz * _GridScale * 5;
                float2 gridFrac2 = frac(gridUV2);
                float gridLine2 = step(gridFrac2.x, _GridWidth * 0.5) + step(gridFrac2.y, _GridWidth * 0.5);
                gridLine2 = saturate(gridLine2) * 0.3;
                
                // Combine: dark blue base with flat matte shading
                half3 color = _BackgroundColor.rgb;
                
                // Very flat face shading — minimal reflectance
                color *= lerp(0.85, 1.0, ndotv);
                
                // Add grid (subtle)
                half gridAlpha = (gridLine + gridLine2) * _GridColor.a * 0.4;
                color = lerp(color, _GridColor.rgb, gridAlpha);
                
                // Edge lines — smooth blend for cleaner look
                half edgeMask = smoothstep(_EdgeThreshold, _EdgeThreshold + 0.15, edge);
                color = lerp(color, _LineColor.rgb, edgeMask);
                
                // Blueprint paper grain — unified with skybox dither
                float2 screenUV = IN.positionCS.xy;
                float gn1 = frac(sin(dot(screenUV, float2(12.9898, 78.233))) * 43758.5453);
                float gn2 = frac(sin(dot(screenUV, float2(39.346, 11.135))) * 23421.6312);
                float grain = (gn1 + gn2 - 1.0) * 0.003;
                color += grain;
                
                color = MixFog(color, IN.fogFactor);
                
                return half4(color, 1.0);
            }
            ENDHLSL
        }

        // Outline pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Front
            ZWrite On

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
                float3 positionWS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _LineColor;
                half4 _BackgroundColor;
                half4 _GridColor;
                half _OutlineWidth;
                half _EdgeThreshold;
                half _GridScale;
                half _GridWidth;
                half _FresnelPower;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;
            float4 _GlobalClipPlane2;
            float _GlobalClipEnabled2;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Expand along normals
                float3 positionOS = IN.positionOS.xyz + IN.normalOS * _OutlineWidth;
                OUT.positionCS = TransformObjectToHClip(positionOS);
                OUT.positionWS = TransformObjectToWorld(positionOS);

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

                return half4(_LineColor.rgb * 0.7, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
