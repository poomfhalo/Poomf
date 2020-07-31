// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Info"
{   
    Properties
    {
        _Stencil ("Mask stencil", Int) = 6
    }
    SubShader
    {
        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always

            Stencil 
            {
                Ref [_Stencil]
                Comp NotEqual
            }

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
                float1 depth : TEXCOORD1;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.depth = COMPUTE_DEPTH_01;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 result = _Color;

                result.b = i.depth;
                return result;
            }
            ENDCG
        }
    }
}
