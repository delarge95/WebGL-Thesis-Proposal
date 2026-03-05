Shader "Skybox/AnimatedGradientSkybox"
{
    Properties
    {
        _TopColor ("Center Color", Color) = (0.04, 0.04, 0.06, 1)
        _BottomColor ("Edge Color", Color) = (0, 0, 0, 1)
        _Speed ("Pulse Speed", Range(0, 10)) = 0.5
        _Scale ("Radius", Range(0.1, 3.0)) = 1.2
        [Toggle] _PulseEnabled ("Enable Pulse", Float) = 1
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
            half _PulseEnabled;

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
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 centeredUV = uv - 0.5;
                centeredUV.x *= aspect;
                
                float dist = length(centeredUV);
                
                // Max distance from center to screen corner (aspect-corrected)
                float maxDist = length(float2(0.5 * aspect, 0.5));
                
                // Breathing pulse (toggleable)
                float pulse = _PulseEnabled > 0.5
                    ? sin(_Time.y * _Speed) * 0.04
                    : 0.0;
                
                // Normalize to [0,1] where 1 = screen corners.
                // _Scale controls how far center color extends:
                // >1 spreads center beyond corners, <1 compresses.
                float nd = saturate(dist / (maxDist * (_Scale + pulse)));
                
                // Soft gradient: smoothstep for zero-derivative boundaries,
                // then sqrt to stretch the mid-tones wider (opposite of squaring).
                // This makes the transition ultra-gradual — no visible edge anywhere.
                float t = smoothstep(0.0, 1.0, nd);
                t = sqrt(t);
                
                fixed4 col = lerp(_TopColor, _BottomColor, t);
                return col;
            }
            ENDCG
        }
    }
}
