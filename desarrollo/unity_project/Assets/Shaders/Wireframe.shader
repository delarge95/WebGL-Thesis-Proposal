Shader "WebGL/Wireframe"
{
    Properties
    {
        _WireColor("Wire Color", Color) = (0.3, 0.8, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (0.02, 0.02, 0.05, 0.0)
        _WireThickness("Wire Thickness", Range(0, 5)) = 1.0
        _WireSmoothness("Wire Smoothness", Range(0, 10)) = 1.0
        
        [Header(Options)]
        [Toggle(_SHOW_DIAGONALS)] _ShowDiagonals("Show Diagonals", Float) = 0
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
            Name "Wireframe"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            
            #pragma shader_feature_local _SHOW_DIAGONALS
            #pragma shader_feature_local _DEPTH_FADE
            #pragma require geometry

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct v2g
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            struct g2f
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 dist : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _WireColor;
                half4 _BackgroundColor;
                half _WireThickness;
                half _WireSmoothness;
                half _DepthFadeDistance;
            CBUFFER_END

            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                // Calculate edge distances for wireframe
                float2 p0 = IN[0].positionCS.xy / IN[0].positionCS.w;
                float2 p1 = IN[1].positionCS.xy / IN[1].positionCS.w;
                float2 p2 = IN[2].positionCS.xy / IN[2].positionCS.w;
                
                float2 v0 = p2 - p1;
                float2 v1 = p2 - p0;
                float2 v2 = p1 - p0;
                
                float area = abs(v1.x * v2.y - v1.y * v2.x);

                g2f OUT;
                
                OUT.positionCS = IN[0].positionCS;
                OUT.positionWS = IN[0].positionWS;
                OUT.dist = float3(area / length(v0), 0, 0);
                triStream.Append(OUT);
                
                OUT.positionCS = IN[1].positionCS;
                OUT.positionWS = IN[1].positionWS;
                OUT.dist = float3(0, area / length(v1), 0);
                triStream.Append(OUT);
                
                OUT.positionCS = IN[2].positionCS;
                OUT.positionWS = IN[2].positionWS;
                OUT.dist = float3(0, 0, area / length(v2));
                triStream.Append(OUT);
            }

            half4 frag(g2f IN) : SV_Target
            {
                // Find minimum distance to edge
                float minDist = min(min(IN.dist.x, IN.dist.y), IN.dist.z);
                
                #ifdef _SHOW_DIAGONALS
                // Also show diagonal
                float diagDist = (IN.dist.x + IN.dist.y + IN.dist.z) / 3.0;
                minDist = min(minDist, diagDist);
                #endif
                
                // Screen-space thickness adjustment
                float thickness = _WireThickness * 0.001;
                
                // Wire visibility with anti-aliasing
                float wire = 1.0 - smoothstep(thickness, thickness + _WireSmoothness * 0.001, minDist);
                
                // Depth fade
                #ifdef _DEPTH_FADE
                float depth = length(IN.positionWS - _WorldSpaceCameraPos);
                float depthFade = 1.0 - saturate(depth / _DepthFadeDistance);
                wire *= depthFade;
                #endif
                
                // Output
                half4 color = lerp(_BackgroundColor, _WireColor, wire);
                color.a = max(wire, _BackgroundColor.a);
                
                return color;
            }
            ENDHLSL
        }
    }

    // Fallback for platforms without geometry shader support
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Wireframe Fallback"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

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
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _WireColor;
                half4 _BackgroundColor;
                half _WireThickness;
                half _WireSmoothness;
                half _DepthFadeDistance;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Fallback: use fresnel for edge detection
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 3.0);
                
                half4 color = lerp(_BackgroundColor, _WireColor, fresnel);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
