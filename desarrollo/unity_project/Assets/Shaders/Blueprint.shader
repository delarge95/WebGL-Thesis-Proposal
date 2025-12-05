Shader "WebGL/Blueprint"
{
    Properties
    {
        _LineColor("Line Color", Color) = (0.2, 0.6, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (0.05, 0.1, 0.2, 1.0)
        _GridColor("Grid Color", Color) = (0.15, 0.4, 0.8, 0.3)
        
        _OutlineWidth("Outline Width", Range(0, 0.02)) = 0.005
        _EdgeThreshold("Edge Threshold", Range(0, 1)) = 0.3
        
        [Header(Grid)]
        _GridScale("Grid Scale", Range(1, 100)) = 20
        _GridWidth("Grid Line Width", Range(0.01, 0.1)) = 0.02
        
        [Header(Technical Lines)]
        _FresnelPower("Edge Detection Power", Range(0.1, 5)) = 1.5
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
                
                // Combine
                half3 color = _BackgroundColor.rgb;
                
                // Add grid
                color = lerp(color, _GridColor.rgb, (gridLine + gridLine2) * _GridColor.a * 0.5);
                
                // Add edges
                color = lerp(color, _LineColor.rgb, edge * step(_EdgeThreshold, edge));
                
                // Add slight ambient occlusion simulation
                color *= lerp(0.7, 1.0, ndotv);
                
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

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Expand along normals
                float3 positionOS = IN.positionOS.xyz + IN.normalOS * _OutlineWidth;
                OUT.positionCS = TransformObjectToHClip(positionOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _LineColor;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
