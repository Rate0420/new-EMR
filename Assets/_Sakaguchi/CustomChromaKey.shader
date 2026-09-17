Shader "Custom/ChromaKey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (1,0,1,1)
        _HueThreshold ("Hue Threshold", Range(0,0.5)) = 0.15
        _SatThreshold ("Sat Threshold", Range(0,1)) = 0.2
        _Softness ("Softness", Range(0,1)) = 0.1
        _Spill ("Spill Suppression", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float2 uv:TEXCOORD0; float4 vertex:SV_POSITION; };

            sampler2D _MainTex;
            float4 _KeyColor;
            float _HueThreshold;
            float _SatThreshold;
            float _Softness;
            float _Spill;

            float3 RGBtoHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 hsvCol = RGBtoHSV(col.rgb);
                float3 hsvKey = RGBtoHSV(_KeyColor.rgb);

                // 色相の差（色相環の折り返し対応）
                float hueDiff = abs(hsvCol.x - hsvKey.x);
                hueDiff = min(hueDiff, 1.0 - hueDiff);

                // 彩度が低い（白・グレー・黒）は抜かない
                float satMask = smoothstep(0.0, _SatThreshold, hsvCol.y);

                // 色相と彩度の両方で判定
                float keyAmount = smoothstep(_HueThreshold, _HueThreshold - _Softness, hueDiff) * satMask;

                // スピル抑制：抜ける部分の色かぶりを除去
                float3 spillColor = col.rgb;
                float spillAmount = keyAmount * _Spill;
                spillColor = lerp(col.rgb, float3(
                    col.r * (1.0 - spillAmount * _KeyColor.r),
                    col.g * (1.0 - spillAmount * _KeyColor.g),
                    col.b * (1.0 - spillAmount * _KeyColor.b)
                ), spillAmount);

                col.rgb = spillColor;
                col.a = 1.0 - keyAmount;

                return col;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            ENDCG
        }
    }
}