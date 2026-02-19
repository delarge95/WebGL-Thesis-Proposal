Shader "Skybox/AnimatedGradientSkybox"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.04, 0.04, 0.04, 1)
        _BottomColor ("Bottom Color", Color) = (0, 0, 0, 1)
        _Speed ("Speed", Range(0, 10)) = 0.5
        _Scale ("Scale", Range(0.1, 10)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;
            half _Speed;
            half _Scale;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float time = _Time.y * _Speed;
                
                // Subtle organic movement
                float wave1 = sin(uv.x * _Scale + time) * 0.1;
                float wave2 = cos(uv.y * _Scale * 0.5 - time * 0.8) * 0.1;
                
                // Gradient with movement
                float t = saturate(uv.y + wave1 + wave2);
                
                // Vignette for premium feel
                float2 center = uv - 0.5;
                float vign = 1.0 - dot(center, center) * 0.5;
                
                fixed4 col = lerp(_BottomColor, _TopColor, t);
                
                // Apply subtle vignette dimming
                return col * vign;
            }
            ENDCG
        }
    }
}
