Shader "Skybox/AnimatedGradientSkybox"
{
    Properties
    {
        _TopColor ("Center Color", Color) = (0.04, 0.04, 0.06, 1) // Slightly blueish dark center
        _BottomColor ("Edge Color", Color) = (0, 0, 0, 1)    // Pitch black edge
        _Speed ("Pulse Speed", Range(0, 10)) = 0.5
        _Scale ("Radius", Range(0.1, 2.0)) = 0.6            // Smaller radius
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
                // Screen UV centered
                float2 uv = i.screenPos.xy / i.screenPos.w;
                // Correct aspect ratio if possible using _ScreenParams
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 centeredUV = uv - 0.5;
                centeredUV.x *= aspect; // Correct aspect so circle is circular
                
                float dist = length(centeredUV);
                
                // Animation: Breathing Pulse
                float time = _Time.y * _Speed;
                float pulse = sin(time) * 0.02; // Subtle pulse
                float radius = _Scale + pulse;
                
                // Radial Gradient: Center to Edge
                // smoothstep for soft falloff
                float t = smoothstep(0.0, radius, dist);
                
                // Lerp: 0 (Center) -> _TopColor, 1 (Edge) -> _BottomColor
                fixed4 col = lerp(_TopColor, _BottomColor, t);
                
                return col;
            }
            ENDCG
        }
    }
}
