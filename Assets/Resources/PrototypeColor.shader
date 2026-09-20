Shader "LightweightGame/PrototypeColor"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct AppData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct VertexToFragment
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            fixed4 _Color;

            VertexToFragment Vert(AppData input)
            {
                VertexToFragment output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.normal = UnityObjectToWorldNormal(input.normal);
                return output;
            }

            fixed4 Frag(VertexToFragment input) : SV_Target
            {
                fixed3 normal = normalize(input.normal);
                fixed3 ambient = ShadeSH9(float4(normal, 1));
                fixed3 diffuse = _LightColor0.rgb * saturate(dot(normal, normalize(_WorldSpaceLightPos0.xyz)));
                return fixed4(_Color.rgb * saturate(ambient + diffuse * 0.8 + 0.12), _Color.a);
            }
            ENDCG
        }
    }
}
