Shader "WebGL/Thermal"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map (for heat variation)", 2D) = "white" {}
        
        [Header(Temperature Range)]
        _MinTemp("Min Temperature", Range(0, 1)) = 0.2
        _MaxTemp("Max Temperature", Range(0, 1)) = 0.8
        _TempVariation("Temperature Variation", Range(0, 1)) = 0.14
        
        [Header(Colors)]
        _ColdColor("Cold Color", Color) = (0, 0, 0.5, 1)
        _MidColor("Mid Color", Color) = (1, 0.5, 0, 1)
        _HotColor("Hot Color", Color) = (1, 1, 0, 1)
        _WhiteHotColor("White Hot Color", Color) = (1, 1, 1, 1)
        
        [Header(Effect)]
        _NoiseScale("Noise Scale", Range(1, 50)) = 4.25
        _NoiseSpeed("Noise Speed", Range(0, 2)) = 0.18
        _EdgeGlow("Edge Glow", Range(0, 2)) = 0.28
        
        [Header(Spatial Thermal)]
        _ThermalMode("Mode (0=Uniform 1=Radial 2=Axial)", Float) = 0
        _ThermalHotspotOS("Hotspot OS", Vector) = (0, 0, 0, 0)
        _ThermalDirectionOS("Direction OS", Vector) = (0, 1, 0, 0)
        _ThermalSpread("Spread", Float) = 0.1
        _ThermalEdgeCooling("Edge Cooling", Float) = 0.22
        _ThermalBaseVariation("Base Variation", Float) = 0.05
        _ThermalPropagation("Propagation", Float) = 1.0
        
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

        Pass
        {
            Name "Thermal"
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
                float3 positionOS : TEXCOORD5;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half _MinTemp;
                half _MaxTemp;
                half _TempVariation;
                half4 _ColdColor;
                half4 _MidColor;
                half4 _HotColor;
                half4 _WhiteHotColor;
                half _NoiseScale;
                half _NoiseSpeed;
                half _EdgeGlow;
                half _ThermalMode;
                half4 _ThermalHotspotOS;
                half4 _ThermalDirectionOS;
                half _ThermalSpread;
                half _ThermalEdgeCooling;
                half _ThermalBaseVariation;
                half _ThermalPropagation;
                half4 _BaseColor;
                half4 _EmissionColor;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;
            float4 _GlobalClipPlane2;
            float _GlobalClipEnabled2;

            // Simple noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float smoothThermalNoise(float2 baseUv, float timeOffset)
            {
                float primary = noise(baseUv + float2(timeOffset, timeOffset * 0.42));
                float secondary = noise(baseUv * 0.55 + float2(12.73 - timeOffset * 0.28, -4.61 + timeOffset * 0.19));
                float blended = lerp(primary, secondary, 0.35);
                return smoothstep(0.2, 0.8, blended);
            }

            half4 GetThermalColor(half temp)
            {
                // 4-color gradient: cold -> mid -> hot -> white hot
                if (temp < 0.33)
                {
                    return lerp(_ColdColor, _MidColor, temp * 3.0);
                }
                else if (temp < 0.66)
                {
                    return lerp(_MidColor, _HotColor, (temp - 0.33) * 3.0);
                }
                else
                {
                    return lerp(_HotColor, _WhiteHotColor, (temp - 0.66) * 3.0);
                }
            }

            // Spatial temperature modulation:
            // Mode 0 (Uniform): temperature is constant across the surface
            // Mode 1 (Radial): temperature decreases with distance from hotspot
            // Mode 2 (Axial): temperature decreases along the direction axis from hotspot
            half ComputeSpatialFactor(float3 posOS)
            {
                half mode = _ThermalMode;
                half3 hotspot = _ThermalHotspotOS.xyz;
                half3 direction = normalize(_ThermalDirectionOS.xyz + half3(0.0001, 0.0001, 0.0001));
                half spread = max(_ThermalSpread, 0.01);
                half propagation = _ThermalPropagation;

                // Uniform mode: no spatial variation
                if (mode < 0.5)
                {
                    return 1.0;
                }

                float3 offset = posOS - hotspot;

                half dist;
                if (mode < 1.5)
                {
                    // Radial: spherical distance from hotspot
                    dist = length(offset);
                }
                else
                {
                    // Axial: distance along the direction axis from hotspot
                    dist = abs(dot(offset, direction));
                }

                // Normalized falloff: 1.0 at hotspot, 0.0 at spread distance
                half falloff = saturate(1.0 - dist / spread);
                // Apply propagation as a power curve for shaping
                falloff = pow(falloff, max(1.0 / propagation, 0.2));

                return falloff;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                OUT.positionOS = IN.positionOS.xyz;

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
                
                // Base temperature band from solver (via MaterialPropertyBlock)
                half baseTempCenter = (_MinTemp + _MaxTemp) * 0.5;
                half baseTempRange = max(_MaxTemp - _MinTemp, 0.001);

                // Spatial temperature modulation (radial/axial/uniform)
                half spatialFactor = ComputeSpatialFactor(IN.positionOS);
                
                // Edge cooling: reduce temperature at surface edges (fresnel)
                half ndotv = saturate(dot(normalWS, viewDirWS));
                half edgeCool = (1.0 - ndotv) * _ThermalEdgeCooling;
                
                // Edge glow: visual highlight at silhouette edges
                half edgeGlow = (1.0 - ndotv) * _EdgeGlow;
                
                // Keep shimmer broad and subtle so it reads like thermal drift, not flicker.
                float2 noiseUV = IN.positionWS.xz * max(_NoiseScale, 0.001);
                float noiseTime = _Time.y * _NoiseSpeed;
                half heatNoise = (smoothThermalNoise(noiseUV, noiseTime) - 0.5) * _ThermalBaseVariation * baseTempRange * 0.55;
                
                // Sample base texture for micro-variation
                half baseHeat = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).r;
                half texVariation = (baseHeat - 0.5) * _TempVariation;
                
                // Combine: solver temperature × spatial falloff + perturbations
                half temperature = baseTempCenter * spatialFactor;
                temperature -= edgeCool * baseTempRange;
                temperature += heatNoise;
                temperature += texVariation * baseTempRange * 0.18;
                temperature = saturate(temperature);
                
                // Get color from thermal gradient
                half4 color = GetThermalColor(temperature);
                color.rgb = lerp(color.rgb, _WhiteHotColor.rgb, edgeGlow * 0.035);
                
                // Subtle scanline effect for technical look
                float scanline = sin(IN.positionCS.y * 2.0) * 0.008 + 0.992;
                color.rgb *= scanline;
                
                // Selection/hover highlight (driven by HighlightSystem)
                half thermalLum = dot(color.rgb, half3(0.299, 0.587, 0.114));
                half3 contrastTint = lerp(_BaseColor.rgb, half3(0,0,0), thermalLum);
                half highlightBlend = saturate(1.0 - _BaseColor.a);
                color.rgb = lerp(color.rgb, contrastTint, highlightBlend * 0.65);
                color.rgb += _EmissionColor.rgb * (0.3 + highlightBlend * 0.4);
                
                color.rgb = MixFog(color.rgb, IN.fogFactor);
                
                return color;
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
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target { return 0; }
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
                float3 normalWS : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);
                return half4(normal * 0.5 + 0.5, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
