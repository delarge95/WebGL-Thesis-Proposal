Shader "WebGL/Blueprint"
{
    Properties
    {
        _LineColor("Line Color", Color) = (0.85, 0.9, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (0.08, 0.18, 0.38, 1.0)
        _GridColor("Grid Color", Color) = (0.4, 0.55, 0.8, 0.2)
        
        _OutlineWidth("Outline Width", Range(0, 0.05)) = 0.0015
        _EdgeThreshold("Edge Threshold", Range(0, 1)) = 0.56
        
        [Header(Grid)]
        _GridScale("Grid Scale", Range(1, 100)) = 20
        _GridWidth("Grid Line Width", Range(0.005, 0.05)) = 0.015
        
        [Header(Technical Lines)]
        _FresnelPower("Edge Detection Power", Range(0.1, 8)) = 6.5
        
        [HideInInspector] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _EmissionColor("Emission Color", Color) = (0, 0, 0, 0)
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
                half4 _BaseColor;
                half4 _EmissionColor;
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
                
                // Fully unlit flat base — no light response at all
                half3 color = _BackgroundColor.rgb;
                
                // Screen-space grid (unified with background, anti-aliased)
                float2 screenUV = IN.positionCS.xy / _ScreenParams.xy;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 gridBase = float2(screenUV.x * aspect, screenUV.y) * _GridScale;
                
                // Distance to nearest grid line center (continuous, no wrap discontinuity)
                float2 fw = fwidth(gridBase);
                float2 d = abs(frac(gridBase - 0.5) - 0.5);
                float2 aa = smoothstep(fw * 1.5, fw * 0.5, d);
                float gridLine = max(aa.x, aa.y);
                
                // Sub-grid (5x finer)
                float2 gridBase2 = gridBase * 5.0;
                float2 fw2 = fwidth(gridBase2);
                float2 d2 = abs(frac(gridBase2 - 0.5) - 0.5);
                float2 aa2 = smoothstep(fw2 * 1.5, fw2 * 0.5, d2);
                float gridLine2 = max(aa2.x, aa2.y) * 0.3;
                
                half gridAlpha = saturate(gridLine + gridLine2) * _GridColor.a * 0.22;
                color = lerp(color, _GridColor.rgb, gridAlpha);
                
                // Silhouette edge detection (Fresnel, unlit — no light dependency)
                half ndotv = saturate(dot(normalWS, viewDirWS));
                half edge = pow(1.0 - ndotv, _FresnelPower);
                half edgeMask = smoothstep(_EdgeThreshold, _EdgeThreshold + 0.07, edge);
                color = lerp(color, _LineColor.rgb, edgeMask);
                
                // Blueprint paper grain — unified with skybox dither
                float2 grainSeed = IN.positionCS.xy;
                float gn1 = frac(sin(dot(grainSeed, float2(12.9898, 78.233))) * 43758.5453);
                float gn2 = frac(sin(dot(grainSeed, float2(39.346, 11.135))) * 23421.6312);
                float grain = (gn1 + gn2 - 1.0) * 0.003;
                color += grain;
                
                // Selection/hover highlight (driven by HighlightSystem)
                half highlightBlend = saturate(1.0 - _BaseColor.a);
                color = lerp(color, _BaseColor.rgb, highlightBlend * 0.18);
                color += _EmissionColor.rgb * 0.08;
                
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
                half4 _BaseColor;
                half4 _EmissionColor;
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

                // Selection-aware outline
                half3 outlineColor = lerp(_LineColor.rgb, _BaseColor.rgb, saturate(1.0 - _BaseColor.a) * 0.25);
                return half4(outlineColor + _EmissionColor.rgb * 0.05, 1.0);
            }
            ENDHLSL
        }

        // Depth-only pass for edge detection prepass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;
            float4 _GlobalClipPlane2;
            float _GlobalClipEnabled2;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
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

                return 0;
            }
            ENDHLSL
        }

        // DepthNormals pass for normal-based edge detection
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormalsOnly" }

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
                float3 normalWS : TEXCOORD1;
            };

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
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
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

                float3 normal = normalize(IN.normalWS);
                return half4(normal * 0.5 + 0.5, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
