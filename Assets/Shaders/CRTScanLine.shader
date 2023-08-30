Shader "Unlit/CRTScanLine"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _LineFrequency("Line Frequency", Range(1, 2000)) = 500
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _LineFrequency;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                
                float lineVal = sin(i.uv.y * _LineFrequency * 3.14159);
                col.rgb *= lineVal * 0.5 + 0.8; // Adjust 0.2 and 0.8 for line strength and base brightness
                
                return col;
            }
            ENDCG
        }
    }
}