Shader "Custom/FanIndicator"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,0.5)
        _Angle("Angle", Range(0, 360)) = 90
        _Radius("Radius", Float) = 5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _Angle;
            float _Radius;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 world : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 center = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                float3 dir = i.world - center;

                float dist = length(dir);
                if (dist > _Radius)
                    discard;

                dir.y = 0;
                float3 forward = normalize(mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1)));
                float angle = degrees(acos(dot(normalize(dir), forward)));
                if (angle > _Angle * 0.5)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
