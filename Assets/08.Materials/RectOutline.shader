Shader "Custom/RectOutline"
{
    Properties
    {
        _Color("Line Color", Color) = (1,0,0,1)
        _Thickness("Line Thickness", Range(0, 0.5)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _Thickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float thickness = _Thickness;

                // X, Y 모두 테두리 조건에 맞는지 확인
                bool insideH = uv.x > thickness && uv.x < (1.0 - thickness);
                bool insideV = uv.y > thickness && uv.y < (1.0 - thickness);

                if (insideH && insideV)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
