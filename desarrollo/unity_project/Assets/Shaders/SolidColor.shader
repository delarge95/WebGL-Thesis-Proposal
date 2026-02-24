Shader "WebGL/SolidColor"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1.0)
        
        [Header(Shading)]
        _AmbientIntensity("Ambient Intensity", Range(0, 1)) = 0.3
        _DiffuseIntensity("Diffuse Intensity", Range(0, 1)) = 0.7
        _SpecularIntensity("Specular Intensity", Range(0, 1)) = 0.3
        _SpecularPower("Specular Power", Range(1, 128)) = 32
        
        [Header(Outline)]
        [Toggle(_OUTLINE_ON)] _OutlineOn("Enable Outline", Float) = 1
        _OutlineColor("Outline Color", Color) = (0.2, 0.2, 0.2, 1.0)
        _OutlineWidth("Outline Width", Range(0, 0.05)) = 0.005
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
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
            #pragma shader_feature_local _OUTLINE_ON

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
                half4 _BaseColor;
                half _AmbientIntensity;
                half _DiffuseIntensity;
                half _SpecularIntensity;
                half _SpecularPower;
                half4 _OutlineColor;
                half _OutlineWidth;
            CBUFFER_END

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                #ifdef _OUTLINE_ON
                float3 positionOS = IN.positionOS.xyz + IN.normalOS * _OutlineWidth;
                OUT.positionCS = TransformObjectToHClip(positionOS);
                OUT.positionWS = TransformObjectToWorld(positionOS);
                #else
                OUT.positionCS = float4(0, 0, 0, 0); // Degenerate triangle
                OUT.positionWS = float3(0, 0, 0);
                #endif

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

                return _OutlineColor;
            }
            ENDHLSL
        }

        // Main color pass
        Pass
        {
            Name "SolidColor"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                float fogFactor : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _AmbientIntensity;
                half _DiffuseIntensity;
                half _SpecularIntensity;
                half _SpecularPower;
                half4 _OutlineColor;
                half _OutlineWidth;
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
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);

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
                half3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                
                // Get main light
                Light mainLight = GetMainLight();
                half3 lightDir = mainLight.direction;
                
                // Ambient
                half3 ambient = _BaseColor.rgb * _AmbientIntensity;
                
                // Diffuse (Lambert)
                half ndotl = saturate(dot(normalWS, lightDir));
                half3 diffuse = _BaseColor.rgb * ndotl * _DiffuseIntensity * mainLight.color;
                
                // Specular (Blinn-Phong)
                half3 halfDir = normalize(lightDir + viewDirWS);
                half ndoth = saturate(dot(normalWS, halfDir));
                half3 specular = pow(ndoth, _SpecularPower) * _SpecularIntensity * mainLight.color;
                
                half3 color = ambient + diffuse + specular;
                color = MixFog(color, IN.fogFactor);
                
                return half4(color, 1.0);
            }
            ENDHLSL
        }

        // Shadow caster
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

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

            float3 _LightDirection;

            // Global clipping (set by CrossSectionManager)
            float4 _GlobalClipPlane;
            float _GlobalClipEnabled;

            Varyings ShadowVert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = positionWS;
                OUT.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                return OUT;
            }

            half4 ShadowFrag(Varyings IN) : SV_Target
            {
                // Cross-section clipping
                if (_GlobalClipEnabled > 0.5)
                {
                    float clipDist = dot(IN.positionWS, _GlobalClipPlane.xyz) + _GlobalClipPlane.w;
                    if (clipDist < 0) discard;
                }

                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
